# 4. Standalone runtime integration

Status: `SUPPORTED` for the previously proved package, exact-scene, world, and
restart boundaries. The current host player-spawn and selected-map prefetch
corrections are `PROVEN-STATIC` until the physical PVE/PVP matrix passes.

Use the standalone method when the map MUST appear in the mission section and
load as its own scene.

The public framework name is **OPERATOR: Modded Operations — Standalone Map
Framework**. This document uses *framework* as the short technical term.

## Ownership model

Keep four owners separate.

| Owner | Owns | MUST NOT own |
| --- | --- | --- |
| Core and catalog | Package validation, immutable identity, deterministic catalog | Map-specific Unity or game code |
| OPERATOR: Modded Operations framework | Private native-style UI, exact bundle and scene load, readiness, `InfiltrationManager`-compatible PVE bridge, shipped `PvpGameode` lifecycle, player creation, shipped failure UI handoff, restart | Map names, shader profiles, terrain dimensions, graph sizes, marker coordinates |
| Data-only package and scene | Manifest, operations, bundles, preview, lighting payload, world, collision, walls, portable assets, markers | Executable code or generic mission UI |
| Optional map companion | Exact-package and exact-scene reconstruction, strict world validation, teardown of map-owned runtime state | Catalog, generic UI, failure UI, other maps |

The package directory is always data-only. A complete map distribution can
also include a separately installed companion DLL.

## Current standalone load order

Use a readiness barrier. Do not use a fixed frame delay as the world contract.

```text
process starts
-> Core validates and freezes data-only packages
-> generic framework creates private native-style UI
-> user selects one immutable package operation
-> framework binds package-owned infiltration data
-> framework starts one selected-map bundle prefetch
-> Confirm joins that request or uses its completed content-ID cache
-> framework verifies dependencies in manifest order
-> framework verifies the exact scene path in the scene bundle
-> framework loads the exact scene additively
-> companion verifies package, map, operation, scene, and build identity
-> framework reconstructs manifest-declared TerrainData and validates collision
-> companion repairs exact-map materials, navigation, grounding, and interactives
-> companion validates the strict world contract
-> framework creates the native-compatible mode owner
-> shipped all-players-loaded readiness completes
-> framework installs current-scene player markers
-> PVE: framework creates or moves players and creates mode-correct actors
-> PVP: shipped PvpGameode respawn and round methods create or move players
-> operation runs
```

Dependency bundles MUST load before the scene bundle. The companion MUST
finish before actor creation. A scene-loaded callback is not a world-ready
signal.

## Package discovery and identity

Core discovers packages only below this directory:

```text
BepInEx/OperatorMods/<package-id>/
```

The directory name MUST equal `packageId`. The package MUST contain no DLL.
Core MUST reject these conditions before Unity loads package content:

- an undeclared file;
- an unsafe relative path;
- a reparse point or path escape;
- a length or SHA-256 mismatch;
- a duplicate or invalid ID;
- an unknown JSON property;
- a scene in a dependency bundle;
- an undeclared scene in the scene bundle.

Core freezes the accepted catalog once during process startup. Change package
files only while OPERATOR is closed. Restart OPERATOR after each package
change.

See [Standalone package format and loading](10-standalone-packages.md).

## UI and selection ownership

The generic framework owns the Modded Operations tab and operation flow. It
uses private clones of shipped visual objects. It MUST NOT add package rows to
the official Active Operations or Operation Simulation arrays.

Each private row binds to one immutable catalog operation. The operation owns
its display name, area, SITREP, preview, infiltration choices, time codes,
mode, player range, spawn set, and PVE population range.

The framework MUST bind package data before it enables the private operation
board. It MUST NOT pass clean package data through a retail setup method that
requires a retail mission graph. It MUST bind Execute, Confirm, Cancel, Back,
selector, and fullscreen controls to private package state.

The current framework implements the presentation boundary with these source
members in `CerberusNativeTabFix.cs`:

| Member | Responsibility |
| --- | --- |
| `SelectCatalogOperation` | Freeze the selected operation, reset to its default time, update the briefing/board, and start one selected-map prefetch |
| `FormatCatalogBriefing` | Format display name, area of operation, and SITREP without a retail mission record |
| `UpdateCatalogOperationBoard` | Bind preview, target-package time records, private board fields, confirmation text, and selector controls |
| `GetOrLoadPreviewSprite` | Read and decode the verified raw package preview and cache it by map ID |
| `ReplaceNativeMapPreview` | Replace the preparation and fullscreen map children with the package sprite |
| `BuildPackageInfiltrationMapPrefab` | Create the private preview background and package-owned native infiltration markers |
| `PrimeNativeInfiltrationSelector` | Call the shipped `InfilSelectorDisplayer.SpawnMap` only after private ownership validation |
| `InvokeNativeBoardStart` | Restore the captured player-owned laptop, close Confirm, and call `CerebusOpboard.Start_Operation` in one final frame |

The UI binding sequence is:

```text
catalog operation record
-> cloned private row
-> SelectCatalogOperation
-> briefing text and preview sprite
-> private TARGETPACKAGE_DATA and TARGETPACKAGE_DETAILS[]
-> package infiltration-map prefab
-> preparation and fullscreen board
-> shipped InfilSelectorDisplayer with package markers
-> Confirm modal
-> captured player-owned MissionLaptop
-> CerebusOpboard.Start_Operation
```

The package raw preview stays outside the AssetBundles. The same map-level
sprite appears in the preparation page, fullscreen page, and infiltration
selector. `mapPositionX/Y` only place a 2D selector marker; scene transforms
under the selected `SPAWN_SET_...` contract place players in 3D.

See
[Modded Operations mission presentation and bundle data](03b-modded-operations-presentation.md)
for exact field mappings and the authoring procedure.

The selection MUST stay stable from row click through restart. Do not use a
mutable row index or an official operation identity as package identity.

The framework MAY start one bounded selected-map bundle prefetch after the row
selection is stable. It MUST keep manifest dependency order, validate the
content ID and scene path, and attach Confirm to the same request. It MUST NOT
prefetch all maps. It MUST log the file bytes, per-bundle time, total time, and
remaining Confirm wait. Prefetch moves cold I/O earlier; it does not remove the
bundle bytes.

## Native loading presentation at the additive-scene boundary

The supported OPERATOR build uses `GameManagerNetwork.ShowLoadingScreen` at
RVA `0x00916210`. `GameManagerNetwork.OnAllPlayersLoaded(false)` enters this
method for a vanilla operation. The method activates the shipped loading
canvas, freezes the current player body, clears player velocity, and closes
the infiltration UI. `GameManagerNetwork.HideLoadingScreen` is at RVA
`0x0090E950`.

An additive package scene has one extra boundary. The replacement `GameMode`
does not own the scene-loaded callback until the next Unity frame. During this
gap, the camera can show the package's portable brown proxy before native
material and `TerrainData` reconstruction.

Close the gap with the shipped method. Call it immediately after the exact
package scene passes identity checks and before terrain or material work:

```csharp
private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    ActiveMapOperation operation = activeOperation;
    if (!IsExactPackageScene(operation, scene))
        return;

    ReleaseStandaloneSceneContracts(operation);
    GameManagerNetwork manager = GameManagerNetwork.instance;
    if (manager == null || manager.LoadingScreen == null)
        throw new InvalidOperationException("Native loading presentation is unavailable.");

    manager.ShowLoadingScreen();
    PrepareStandaloneScene(operation, scene);
}
```

Do not clone a loading canvas. Do not hide it from the map companion. The
persistent native `GameManagerNetwork` keeps ownership of the matching hide
transition after the replacement mode reaches the shipped readiness barrier.

For diagnosis, read `manager.LoadingScreen.activeSelf` and
`activeInHierarchy`. Do not interpret `GameManagerNetwork.LoadingScreenVisible`
as the canvas state on this build. Its getter at RVA `0x0091A840` returns the
private `_hideLoadingScreenSoon` byte at offset `0x2A4`. The value can be
`false` immediately after a successful `ShowLoadingScreen` call.

Ukrainian Forest package `0.3.21` loads two verified files with a combined
size of `647940302` bytes. In the accepted combined-package run, the
`630326573`-byte dependency bundle took `23.270 s`, the scene bundle took
`0.807 s`, and verified registration took `24.148 s` total. Confirm waited
`22.119 s` for
the remaining selected-map work. Vanilla missions can appear faster because
their content is installed with the game and can already be resident. This
timing difference is not evidence that the exact scene failed.

## Scene-bundle ownership

The scene bundle MUST contain a real `.unity` scene. A prefab-only address is
not a standalone mission.

The package scene owns:

- world roots and transforms;
- terrain and render geometry;
- collision and gameplay walls;
- lighting and volumes;
- player, enemy, HVT, and other mission markers;
- portable model, texture, and material closure;
- map identity and spawn-set marker objects.

The scene MUST NOT require a hidden retail gameplay scene. The framework MAY
use persistent engine-level services that the shipped operation flow already
owns. It MUST NOT use another mission's world, briefing, spawn list, target
package, or restart scene.

## Exact-scene companion ownership

Install a required map companion here:

```text
BepInEx/plugins/<map-plugin>/<map-plugin>.dll
```

The companion MUST use hard exact dependencies on the tested Core and generic
framework versions. It MUST refuse all other package, map, operation, scene,
or build identities before it mutates state.

The companion MAY own only these exact-scene tasks:

- native material reconstruction;
- runtime `TerrainData` reconstruction;
- map-owned A* service and graph construction;
- mission-marker containment, grounding, and graph validation;
- runtime initialization only for map-specific surfaces, audio, or
  interactives that have a proved run-time contract. A normal `DoorV2` is
  authored prefab content and is not created by the companion;
- teardown of objects and state that the companion owns.

The companion MUST NOT own catalog discovery, mission UI, selection, generic
scene loading, player readiness, PVE actor creation, the `PvpGameode` round
graph, failure UI, or generic mode ownership.

### Borrow assets from a verified dependency

The framework owns every verified dependency-bundle handle and its unload
lifetime. A companion that needs an exact bundle asset must call:

```csharp
TextAsset state = CerberusNativeTabFix
    .LoadVerifiedMapDependencyAsset<TextAsset>(mapId, assetPath);
```

This API searches only dependencies that Core verified and the framework
retained for the exact `mapId`. The returned object is borrowed. The companion
must not unload its bundle.

Do not use `AssetBundle.GetAllLoadedAssetBundles()` to infer package ownership.
Do not call `AssetBundle.LoadFromFile` again for the same large bundle. Global
enumeration does not prove the package owner, and a second load creates a
second memory and teardown path.

## Native material reconstruction

An external Unity project usually cannot compile OPERATOR's private HDRP
Shader Graphs. A complete bundle can therefore render with brown, flat,
opaque, or error materials.

Use a portable material as a transport record:

1. Preserve the original native material identity separately from the proxy
   name.
2. Package base/alpha, normal, mask, tint, and each audited special map.
3. Record the destination property for each proxy cargo slot.
4. In the exact scene, create a new material from the installed native shader
   family.
5. Apply the audited queue, alpha cutoff, culling, passes, keywords, and
   numeric values.
6. Move each cargo texture to its real native property.
7. Disable the proxy meaning of the cargo property.
8. Bind the new material only to map-owned renderers or terrain.
9. After several rendered frames, require zero active proxy or error shaders.

Do not choose a profile from a proxy wrapper name. Do not call
`CopyPropertiesFromMaterial` with an `InternalErrorShader` source. Do not use
an unrelated transparent shader as a foliage repair.

## Runtime terrain reconstruction

A managed Unity wrapper can be fake-null in IL2CPP. Test native-aware validity
before you accept `TerrainData`, `Terrain`, or `TerrainCollider`.

If `TerrainData` does not survive the bundle boundary:

1. Package a lossless height payload and surface-weight payload.
2. Record the exact resolutions, origin, size, and three terrain layers.
3. Create a new native `TerrainData` in the exact scene.
4. Create IL2CPP-compatible arrays for heights and alphamaps.
5. Set size, resolutions, layers, heights, and alphamaps.
6. Bind the same live `TerrainData` to `Terrain` and `TerrainCollider`.
7. Disable the exact serialized render fallback only after both live
   components bind the reconstructed data.
8. Align complete-tree visible renderer bases to `Terrain.SampleHeight`.
9. Call `Physics.SyncTransforms`.
10. Prove collision and surface height before marker or actor readiness.

Do not let an actor spawn because a visible fallback mesh exists.

Do not use the source prefab pivot as the tree-base contract. Apply final yaw
and scale first. Use LOD0 bark/trunk submesh data and a visually proved
family-aware reference. Store the selected surface-contact X/Z in a
renderer-free child datum. At run time, sample the datum position instead of
the tree-root center. Use a bounded maximum correction and a complete-rendered
above-ground gate.

## Night presentation ownership

Use the exact shipped night Volume and NVG-color source for a night operation.
Do not combine a day LUT with improvised negative exposure.

For the current worked source, `sharedassets7.assets` profile path `435`
(`PVP map NIight VOLUME`) uses ACES without an external LUT. Its Exposure path
`440` uses Automatic Histogram, compensation `1.16`, limits
`5.065281867980957..9.348570823669434`, and adaptation speeds `3/3`.
`GameManager.SetNVGColor(0)` selects the current-build white-phosphor value.

Capture the prior NVG value before the operation. Restore it and destroy every
operation-owned `VolumeProfile` during unload.

## Navigation ownership

A scene bundle does not prove that a live A* service or scanned graph exists.
Navigation is runtime state.

When a companion owns graph construction, it MUST:

1. Resolve or create one map-scoped `AstarPath` service.
2. Resolve or add one enabled `Pathfinding.RVO.RVOSimulator` on the same host
   as `AstarPath`. The shipped `BOT V2` movement child uses
   `FollowerEntity`, which requires the native RVO service.
3. Add and configure the installed native graph type.
4. Size the graph from the authoritative physics and bullet-interaction
   volume.
5. Exclude the render-only terrain and scenery apron.
6. Restore any temporary scan layer in a `finally` block.
7. Scan the exact graph.
8. Reject outside markers before nearest-node lookup.
9. Require tight grounding and `IsPointOnNavmesh` for each marker class.
10. Reject readiness when `RVOSimulator.active` is null or disabled.
11. Remove only the map-owned graph, RVO service, or host on unload.

Graph membership does not prove gameplay-wall containment.

See [AI navigation, routes, and behavior](11-ai-navigation-and-behavior.md).

## Mode ownership

Use the verified native-compatible owner for each operation mode.

For standalone PVE, a bare `GameMode` is insufficient. The shipped
`GameManagerNetwork.FailOperation()` path reads
`InfiltrationManager.instance` and its synchronized `RaidTimer`.

The generic framework MUST do these tasks:

1. Create an `InfiltrationManager`-compatible standalone PVE owner.
2. Assign `InfiltrationManager.instance` and `GameMode.singleton`.
3. Initialize and advance the synchronized network raid timer after readiness.
4. Suppress only official-scene callbacks that require a retail scene graph.
5. Keep the shipped persistent Mission Failed UI and Restart Operation control.
6. Clear both singleton references on unload when they still point to the
   standalone owner.

Do not put this bridge in the package or map companion. Do not clone the
Mission Failed UI. Do not load a donor mission to provide failure state.

For standalone PVP, a bare `GameMode` is also insufficient. The retail owner
is `PvpGameode`. The current framework MUST do these tasks:

1. Create `StandalonePvpGameMode : PvpGameode` with one `NetworkIdentity`.
2. Convert scene markers to `SpawnPoint` components with one-based team IDs.
3. Assign non-empty `Team1SpawnPoints` and `Team2SpawnPoints` lists.
4. Seed `MaxRounds=13`, `RoundsToWin=7`, and `RoundTime=120`.
5. Supply every audio, clip-array, `TeleType`, score, clock, outcome,
   animator, fade-string, and status-text reference that the native hooks read.
6. Call the shipped `PvpGameode.OnStartClient` body for initialization.
7. Call the shipped `PvpGameode.Server_AllPlayersLoaded` body after readiness.
8. Let the shipped `RespawnPlayers`, `StartNewRound`, `PlayerDied`,
   `EndRound`, score SyncVars, and freeze timer own the match.
9. Stop the generic position-only player loop after native PVP is active.
10. Clear `PvpGameode.instance` on unload when it still points to the
    operation-owned component.

The Forest reference has ten Team 1 markers on the PVE-player side and ten
Team 2 markers on the PVE-enemy side. See
[Native mode ownership, PVE, and StandardPVP](03c-native-mode-ownership-and-pvp.md)
for the exact current-build fields and native method evidence.

## Readiness and actor creation

The framework MUST wait for all of these conditions:

- exact scene loaded;
- exact map and spawn-set markers found;
- companion world contract passed;
- mode owner valid;
- shipped all-players-loaded barrier complete;
- loading screen closed;
- current-scene player spawn list installed.

Only then can the PVE bridge create or move players. Native PVP uses the same
barrier, but `PvpGameode` owns its respawn and placement coroutine. Only the
server can create PVE actors. PVP MUST create zero PVE actors.

Before player creation, capture `GameManager.SpawnPointsInScene`,
`GameManager.Pspawns`, and the next-spawn index. Install only the operation's
current-scene values. The exact-build native `nextSpawnPosition` body at RVA
`0x00EF2CA0` reads the current index and then increments it. Set the first
index to `0`. An index of `-1` throws before a spawn is selected.

On the current exact build, owned-player creation MUST call
`PlayerMaster.SpawnPlayer()`. This route performs ownership checks, enters the
Mirror command, and then calls `ClientSpawnBS`. The generated server
implementation is `PlayerMaster.UserCode_CMDSpawnPlayer__NetworkIdentity`.
It selects a shipped spawn point, instantiates the shipped player prefab,
calls owner-aware `NetworkServer.Spawn`, assigns the spawned-player object,
and sends the retail spawn RPC.

On the first request for each player and additive-scene generation, call
`SpawnPlayer()`. If an owned host still has no new `PlayerSpawnedObject` after
300 frames, call the exact generated server implementation as one bounded
repeat-generation recovery. This route repairs stale Mirror command-sender
state after the old map destroyed the previous player object. It does not
replace `ClientSpawnBS`; the first request already ran that client path. Call
the generated body directly on request 1 only for an unowned server player.
Limit an owned-host sequence to two requests: one native kickoff and one
generated-body recovery. Other routes retain a three-request ceiling. Record
the request before native entry.

The run-time PVE and PVP game-mode owners also need remote-peer Mirror
identity. Create one inactive template on each peer. Assign deterministic,
collision-checked asset IDs before host spawn. The current framework uses
`0x4D4F5001` for PVE and `0x4D4F5002` for PVP. Register with
`NetworkClient.RegisterPrefab(template, assetId)`, then spawn on the host with
`NetworkServer.Spawn(instance, assetId, connection)`. A remote peer MUST adopt
only a clone with the expected asset ID and operation mode. Unregister and
destroy the operation-owned template during release. A host-only object with
asset ID `0` is not a valid multiplayer contract.

Do not rely only on `NetworkClient.UnregisterPrefab(template)` during scene
release. Unity can destroy a scene-owned template before the callback. Its
wrapper then compares equal to null, but Mirror can retain it under the asset
ID. The next registration can throw from `UnityEngine.Object.GetName` and the
native `MAP LOADED !BUG!` prompt can loop.

Capture the package-owned asset ID before you clear operation state. Remove
only that key from `NetworkClient.prefabs` and call
`NetworkClient.UnregisterSpawnHandler(assetId)` during release, even when the
template wrapper is fake-null. Before registration, evict the same entry only
when it exists and compares equal to null. Reject a different live object as
an asset-ID collision. Never use `NetworkClient.ClearSpawners()` for this
repair because it clears unrelated registrations.

Write the request frame and attempt count before native entry. Limit retries.
Stop after the player-object or alive state proves success. An exception MUST
NOT create one native call on every frame. Treat this route as
`PROVEN-STATIC` until physical player, camera, movement, and combat tests pass.

For PVE, the package declares `minEnemies` and `maxEnemies`. The framework
selects one inclusive deterministic count from the valid sorted markers. It
MUST NOT use Unity global random state for this selection. It MUST fail when
fewer than `minEnemies` valid markers remain.

Every StandardPVE scene MUST also contain exactly one inactive
`PVE_ExfilZone_` marker with a positive trigger BoxCollider. The map owns its
transform and collider. The generic framework copies that data to its
Mirror-owned `InfiltrationManager` bootstrap and creates one shipped
`RaidManager` plus one shipped `ExfilZone`.

Start zone-level and global extraction locked. Use native AI `Health` deaths
and `GameManager.allAI` as the shipped raid input. After native unlock, require
physical player occupancy and the shipped extraction timer. Do not unload the
map or show a custom success popup when the last enemy dies. Read
[Native PVE completion, extraction, and ATAK](15-native-pve-completion-exfil-and-atak.md).

## Failure behavior

Fail closed when a required condition is false. Keep the player in a safe
non-operation state. Give the user one actionable error that contains the
package, map, scene, stage, and reason.

Do not continue with these partial states:

- a brown or error-shader world;
- visible terrain without usable `TerrainCollider` data;
- no resident scanned graph for a PVE map that requires one;
- an enemy marker outside the gameplay wall;
- fewer valid markers than the operation minimum;
- a missing mode singleton or raid timer;
- a scene path that differs from the selected catalog entry.

## Restart and unload order

Use reverse ownership order:

```text
stop mode population
-> invalidate the scene generation
-> restore prior process-global spawn registration only when this operation still owns it
-> unregister the operation-owned Mirror game-mode template
-> destroy operation-owned ATAK mesh/material assets and clear exfil occupants when the operation was not successful
-> clear standalone mode singleton, PvpGameode, and timer ownership
-> restore captured NVG state and destroy run-time Volume profiles
-> clear the companion's player transform/controller hold, spawn-safety window, counters, applied flag, and destination-scene reference
-> companion removes its graph, materials, native data, objects, and callbacks
-> unload package scene
-> release scene bundle
-> release dependency bundles in reverse order
-> clear active package selection when the operation ends
```

An asynchronous completion MUST carry the scene-generation ID. It MUST do
nothing when its generation is stale.

Normal Restart Operation creates one new scene generation. It MUST NOT stack a
second graph, callback set, door, actor list, or material set over the old
generation.

During a successful unload, do not clear
`GameManagerNetwork.SuccessfulOperation`. The shipped Operation Room reads the
result after the additive scene unloads.

For a companion with late-player hooks, keep `applied=false` until the exact
standalone `Terrain` and `TerrainCollider` share one non-null `TerrainData`.
Set it for only that scene generation. Clear it and every held player
transform in `OnSceneUnloaded` before the persistent player returns to the
armory. A new generation must not reuse the previous destination scene or
local-move-request flag.

## Retired overlay method

MapBridge remains a local overlay and diagnostic tool. It is not the current
mission method. See the [archived MapBridge workflow](archive/legacy-mapbridge-overlay.md).
