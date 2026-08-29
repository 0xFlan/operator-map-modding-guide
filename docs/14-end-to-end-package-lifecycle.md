# End-to-end standalone package lifecycle

## 1. Use this chapter

Use this chapter when you create a map for OPERATOR: Modded Operations. It
connects authoring data to the exact runtime consumer. It also records the
Ukrainian Forest implementation as a worked example.

This is not an overlay procedure. The supported architecture loads the exact
package scene through the shipped Cerberus board. It does not load a donor
mission and hide its geometry.

## 2. Ownership model

| Owner | Owns |
| --- | --- |
| Operator Mod API Core | Manifest schema, canonical paths, byte counts, SHA-256, immutable catalog. |
| Modded Operations | Cerberus UI, preview, selected-map I/O, exact scene launch, generic runtime terrain, player, PVE, PVP, failure-state bridge, restart, and teardown. |
| Data-only package | Manifest, dependency bundles, scene bundle, preview, optional LUT, operations, and scene markers. |
| Optional companion | Exact-map shader/material repair, post-bind grounding, map A*, lighting, interactive initialization, diagnostics, and reverse cleanup. |

A companion does not parse a second manifest. It does not load a second copy
of a framework-owned bundle. It does not call `DebugStartOperation`. It does
not create a parallel mission UI.

The matching preview API Core/host pair ships inside the Modded Operations
framework download. It is not a standalone public download until the API
reaches a full stable release. A map download contains only its data package
and optional companion; it must not duplicate the framework or API.

## 3. From source asset to playable object

For each object, close this complete chain:

```text
installed source owner file and path ID
-> extracted mesh/prefab/material/texture records
-> Unity project GUID and .meta identity
-> prefab renderer and collider topology
-> dependency-bundle asset name
-> scene reference or authored root
-> runtime resident shader or native component binding
-> physical player-camera and interaction proof
```

A filename is not an object identity. Record the source asset file and path
ID. An external shader pointer must be resolved in its target asset file.

For a renderer, record:

1. LOD and mesh identity.
2. Vertex and index counts.
3. Submesh count.
4. Material count and order.
5. Base, normal, mask, opacity, and thickness inputs.
6. UV and vertex-color requirements.
7. Shader compiled name and actual property names.
8. Render queue, sidedness, keywords, tags, and disabled passes.
9. Collider and interaction children.
10. Player-height visual result.

## 4. Bundle split

Build explicit bundle maps:

```csharp
new AssetBundleBuild
{
    assetBundleName = "author_map_assets",
    assetNames = dependencyAssets
},
new AssetBundleBuild
{
    assetBundleName = "author_map_scene",
    assetNames = new[]
    {
        "Assets/Maps/AuthorMap/Scenes/AuthorMap.unity"
    }
}
```

Use `BuildAssetBundleOptions.StrictMode` and
`BuildTarget.StandaloneWindows64` for the pinned game build.

The dependency bundle contains reusable assets and numerical payloads. It
must return zero entries from `GetAllScenePaths`. The scene bundle contains
the streamed scene. It must return exactly the manifest `scenePath`.

List dependency bundles in reference order. Modded Operations loads them from
first to last and loads the scene bundle last.

## 5. Scene identity and marker data

Create:

```text
MAP_ID_<mapId>
SPAWN_SET_<spawnSet>
```

The marker names are case-sensitive ordinal identities. They can be inactive.
Their transform positions do not matter for identity markers.

World marker prefixes are:

| Use | Prefix |
| --- | --- |
| PVE player | `PVE_PlayerSpawn_` |
| PVE enemy | `PVE_EnemySpawn_` |
| PVE HVT when map-specific | `PVE_HVTSpawn_` |
| PVP Team 1 | `PVP_Team1Spawn_` or accepted Team 1 aliases |
| PVP Team 2 | `PVP_Team2Spawn_` or accepted Team 2 aliases |
| FFA | `FFA_Spawn_` |

Use zero-padded suffixes. Ground each marker on live collision. Keep it inside
the bullet wall and navigation graph.

The briefing infiltration fields are separate:

```json
{
  "id": "north-entry",
  "displayName": "NORTH ENTRY",
  "mapPositionX": 0.5,
  "mapPositionY": 0.2,
  "maxPlayers": 8
}
```

These X/Y values position an icon on the image. They do not position an actor
in the scene.

## 6. Runtime terrain record

Use `runtimeTerrain` when an external-bundle test does not prove stable
serialized `TerrainData` transport. Declare:

- exact terrain root object name;
- verified dependency-bundle path;
- height payload address and encoding;
- weight payload address and encoding;
- heightmap, alphamap, basemap, and detail resolutions;
- world origin and size;
- every layer name and complete texture set;
- tile size, normal scale, metallic, and smoothness.

Modded Operations `TryPrepareRuntimeTerrain` retrieves the exact resident
dependency bundle by Core-verified path. It creates one `TerrainData`. It
binds that same object to `Terrain` and `TerrainCollider`. It then calls
`Physics.SyncTransforms`.

`ValidateWalkableGroundContract` requires each compatible player marker to
raycast to active non-trigger collision. For a declared terrain, it also
requires identity equality between the render terrain, collision terrain, and
operation-owned runtime object.

Keep an authored render-fallback mesh inactive. The framework can disable a
known fallback after a successful bind. Two live ground surfaces can show a
flat plane and can place a player under the intended hills.

## 7. Preview and briefing data

Store a JPEG or PNG outside bundles. The manifest names it with
`previewImage` and closes it in `files[]`.

Runtime data flow is:

```text
Core verifies bytes and SHA-256
-> framework reads exact file
-> ImageConversion.LoadImage
-> one cached Sprite
-> preparation image
-> fullscreen image
-> infiltration-map image
```

An end user does not browse to an arbitrary file in Cerberus. To change the
image, replace it, update path if required, update bytes and SHA-256, increase
package version, and restart the game.

## 8. Selection and prefetch

Selecting a row calls `BeginSelectedMapPrefetch`. It loads only the selected
map. A same-content resident cache is reused. A stale
`PackageContentId` entry is unloaded.

`PendingMapLaunch` stores the ordered bundle paths, current async request,
map identity, selected operation/time, and captured laptop/player owner.

A large cold dependency bundle can take longer than a vanilla map because the
base game can already have retail content resident. Prefetch moves I/O to
selection time. It does not remove the physical byte cost.

For the reference Forest package, the declared dependency and scene bundles
total `647869804` bytes. In the exact combined-package run, the dependency
bundle took `24.449 s`, the scene bundle took `0.833 s`, and verified
registration took `25.347 s` total. Confirm waited `23.442 s` for the
remaining work. Treat this as verified content I/O, not as a reason to show
the scene's portable proxy.

## 9. One-Confirm launch

`BeginCatalogOperationLaunch` revalidates the operation/time pair. It captures
`MissionLaptop` and `PlayerNetworking` before async I/O. Confirm attaches to a
same-map prefetch and disables the confirmation while loading.

`ProcessPendingLaunch` starts one `AssetBundle.LoadFromFileAsync` at a time.
It rejects scenes in dependency bundles and undeclared scenes in the scene
bundle. On failure, it unloads partial objects and restores confirmation.

On success:

```text
InvokeNativeCatalogLaunch
-> restore captured laptop owner
-> update private board data
-> close native confirmation
-> InfilSelectorDisplayer.SpawnMap
-> validate package infiltration markers
-> CerebusOpboard.Start_Operation
```

Do not call `OperationsManager.StartOperation`,
`OperationsManager.CMD_StartOperation`, or
`OperationsManager.DebugStartOperation` directly.

## 10. Scene preparation

`OnSceneLoaded` accepts only the active map's exact path/name and marker
contract. It releases any old generation state, clears player/mode/PVE
tracking, and schedules preparation.

Before it schedules preparation, it calls the shipped
`GameManagerNetwork.ShowLoadingScreen()` method. On the supported build, the
method is at RVA `0x00916210`; vanilla
`GameManagerNetwork.OnAllPlayersLoaded(false)` uses the same route. The method
activates the shipped canvas, freezes the current player body, clears
velocity, and closes infiltration UI. The native hide method is at RVA
`0x0090E950` and remains owned by `GameManagerNetwork`.

This call closes the additive-scene one-frame gap before the replacement
`GameMode` can own the all-players-loaded barrier. Without it, the authored
brown proxy can be visible before runtime material and terrain repair.

Validate `LoadingScreen.activeSelf` and `activeInHierarchy`. Do not use the
misnamed `LoadingScreenVisible` property as a canvas probe. Its supported-build
getter at RVA `0x0091A840` returns `_hideLoadingScreenSoon` at offset `0x2A4`.

`PrepareStandaloneScene` uses:

```text
TryPrepareRuntimeTerrain
-> Physics.SyncTransforms
-> ValidateWalkableGroundContract
-> ConfigureStandalonePlayerSpawnContract
-> CreateStandaloneGameplayBootstrap
-> ApplyStandaloneRenderContract
-> mark preparation complete
```

Do not spawn players before this order finishes.

## 11. Optional companion activation

Schema v2 declares a required companion with `runtimeCompanion`: exact plugin
GUID, exact SemVer, lowercase DLL SHA-256, and distinct READY/FAILED
scene-marker names. Core freezes that declaration; it does not load the
plugin. The framework first resolves the already loaded plugin and verifies
its exact identity. Then gate companion scene work on:

- exact streamed scene path;
- exact `MAP_ID_<mapId>`;
- expected authoring marker;
- compatible game and framework version;
- one shared final `TerrainData` when terrain is declared.

Publish the READY marker only after the complete exact-scene contract passes.
Publish FAILED on an unrecoverable world error. FAILED wins even if READY was
already observed. For PVP, this check completes before the peer can send
`SceneReady` for the current epoch.

A companion can then repair resident shaders, terrain materials, trees,
props, A*, lighting, doors, or map-specific markers. It must destroy only its
own runtime objects.

Ukrainian Forest uses this companion order:

```text
RepairBundleMaterials
-> ForcePlayableFoliageHighestDetail
-> wait for shared TerrainData
-> AlignStandaloneAuthoredTreesToTerrain
-> RepairNativeTerrainMaterials
-> Physics.SyncTransforms
-> EnsureStandaloneNavigationGraph
-> LogStandaloneWorldContract
```

`EnsureStandaloneNavigationGraph` creates or reuses the map-scoped
`AstarPath`. It also requires an enabled
`Pathfinding.RVO.RVOSimulator` on the same host. Vanilla `level16` stores this
pair on the `Astar Navmesh` GameObject. The current native `BOT V2` prefab
keeps `BrainAI` on its stationary network root and `AgentController` plus
`FollowerEntity` on the moving `SK_Insurgent_P8` child. Measure search motion
from `brain.agent.position`, not `brain.transform.position`.

The diagnostic is an exact-map release gate. Generic terrain and game-mode
readiness remain framework-owned.

## 12. Material transport and the Forest example

A custom bundle cannot safely serialize an installed first-party shader
pointer as if the bundle were part of `sharedassets21.assets`. Use a portable
proxy and resolve the compiled shader in the running game.

For Forest pines:

| Source object | Owner and path ID | Resident shader |
| --- | --- | --- |
| `Pine_Needle` | `sharedassets21.assets` path `11`, shader pointer `242` | `Shader Graphs/BotD_Graph_Lit_TranslucentAlphaCutoff` |
| `pine_bark` | path `7`, shader pointer `237` | `Shader Graphs/SeedMesh_Tree_Bark` |
| `Trunk_pine_var4` | path `14`, shader pointer `237` | `Shader Graphs/SeedMesh_Tree_Bark` |

Shader pointers `242` and `237` resolve in
`globalgamemanagers.assets`. Renderer order is needle, bark, trunk.

The bark shader reads `_MainTex`, `Normal_vegetation`, and
`mask_vegetation`. Generic `_BaseColorMap`, `_NormalMap`, and `_MaskMap`
names are not sufficient as destinations.

The anti-gloss state uses:

```text
pine_bark:
  Vector1_DDCDCAD2 = 1
  Vector1_16F2F1E4 = .186
  Vector1_813F3AD6 = -1
  _Wetness_sm = 0
  _Vertex_AO_sm = 0

Trunk_pine_var4:
  Vector1_DDCDCAD2 = 0
  Vector1_16F2F1E4 = 0
  Vector1_813F3AD6 = 0
  _Wetness_sm = 0
  _Vertex_AO_sm = 0
```

The shader does not declare generic `_Smoothness` fields. An audit of only
those fields is invalid.

## 13. Tree and prop grounding

Ground a complete mesh, not its GameObject pivot by assumption. For a narrow
tree, a center sample can be valid after visual proof. For a broad root system,
sample the lower trunk/root footprint.

Ukrainian Forest stores one child datum:
`NATIVE_TRUNK_GROUND_DATUM_ONE_SIXTH`. It uses one-sixth of a measured trunk
span and a 0.25 m additional broad-oak embed. Runtime samples final terrain at
the stored datum X/Z.

For a prop with a broad footprint, rotate its mesh bounds and sample center
and corners. Reject steep height spans. Compute root Y from actual
`Mesh.bounds.min.y`. Do not generate a hidden support pad unless the design
explicitly requires built ground.

Always validate at player height from uphill, downhill, and slope-parallel
views.

## 14. Navigation and boundaries

Make graph authority equal gameplay authority. If the bullet walls contain a
70 by 140 m region, do not let the graph include an exterior render buffer.
Every AI marker must be:

1. inside wall bounds;
2. outside boundary clearance;
3. near ground;
4. on a walkable graph node;
5. reachable from the combat region;
6. not separated by a donor bullet blocker.

A visual terrain apron can extend beyond the wall. It must not become actor
authority.

## 15. Player and respawn

`ConfigureStandalonePlayerSpawnContract` creates current-scene
`SpawnPoint` objects and captures:

```text
GameManager.SpawnPointsInScene
GameManager.instance.Pspawns
GameManager.instance.PnextSpawnIndex
GameManager.instance.RandomSpawns
```

The first request calls `PlayerMaster.SpawnPlayer()`. This keeps shipped
camera, input, locomotion, and `ClientSpawnBS` behavior. A bounded generated
command is a repeat-host recovery, not the default.

PVP values are one-based: Team 1 is 1 and Team 2 is 2. The shipped
`PvpGameode` owns round respawn and uses separate team arrays.

## 16. PVE AI and firearm ownership

The host validates the inclusive manifest range and marker count. An accepted
AI prefab has root `BrainAI`, root `NetworkIdentity`,
`WeaponsAI.SpawnWeapon=true`, and a non-empty weapon list.

Use:

```csharp
raid.ServerSpawnAI(false);
```

The current native body calls
`NetworkServer.Spawn(bot, GameManager.instance.gameObject)` before it applies
`BotSpawnDetails`. A manual ownerless spawn can leave grenades working but
firearm damage incomplete.

## 17. PVP owner

Use `StandalonePvpGameMode : PvpGameode`. Supply Team 1 and Team 2 arrays,
audio sources, non-empty clip arrays, timer/score text, result roots,
animators, fade states, outcome text, and round values. The shipped controller
owns freeze, death, score, respawn, and operation end.

Modded Operations `0.3.30` protocol v6 freezes authenticated host/remote membership for PVE and PVP
and requires exact framework/API/package/optional-companion/operation/scene
identity before every remote sends `ContentReady`. After scene transition,
each peer validates the exact scene, native template, mode-owned spawn contract, and
companion READY/FAILED contract before it sends `SceneReady` for the
host-issued nonzero UInt64 scene epoch. Retained-content Restart increments
the epoch exactly once. Stale readiness, membership change, late join,
disconnect, replacement connection, mismatch, timeout, and overflow fail
closed.

PVP requires `ceil(maxPlayers/2)` valid markers per team. The current vanilla
maximum is 12, which requires at least six on each side. Prove the protocol
and gameplay with one host and one remote client; run a separate real
12-player stress matrix before advertising 12-player support.

Protocol v6 gives online PVE a separate content/scene agreement and binds the
host-confirmed count. It remains `PROVEN-STATIC`: require two-process package,
scene, AI replication, movement, projectile, damage, completion, extraction,
Restart on both peers, failure/return, and teardown proof.

## 18. Failure, restart, and leave

Persistent `GameManagerNetwork` owns Mission Failed and Restart Operation UI.
The standalone PVE owner supplies `InfiltrationManager.instance` and
`NetworkRaidTimer`. The framework advances the timer only after all players
load.

On scene unload, release in reverse ownership order:

```text
restore operation-owned spawn globals if identity still matches
-> restore NVG and destroy operation-owned render profiles
-> clear owned mode singletons
-> remove exact Mirror prefab ID and spawn handler
-> destroy owned PVP/PVE utility objects
-> release framework runtime TerrainData and layers
-> clear generation caches
```

Keep verified map bundles resident for shipped restart. A full package release
can unload them later. Never use `NetworkClient.ClearSpawners()`.

For PVP retained-content Restart, preserve content agreement but advance the
host scene epoch exactly once. Require the remote's corresponding monotonic
local scene generation before owner spawn. Do not allow a prior scene-ready
acknowledgement to survive replacement-scene callback reordering.

For StandardPVE, teardown also destroys the operation-owned ATAK mesh and
material. It clears zone/global extraction occupants when the operation was
not successful. It preserves `GameManagerNetwork.SuccessfulOperation` during
a successful map unload so the Operation Room can display and consume the
native result.

The stale Mirror ID caused the repeat `MAP LOADED !BUG!` loop. A stale spawn
global caused the floating armory return. Identity-conditional reverse cleanup
is required.

## 19. Build and package proof

For every release file, record:

```powershell
$item = Get-Item -LiteralPath '<FILE>'
$hash = Get-FileHash -Algorithm SHA256 -LiteralPath $item.FullName
$item.Length
$hash.Hash.ToLowerInvariant()
```

The archive must extract directly into `<OPERATOR_INSTALL>`. Public users
install one matching framework download, which includes the preview API, then
one separate map-only archive. The map-only archive normally contains:

```text
BepInEx/
  plugins/<MAP_COMPANION>/<MAP_COMPANION>.dll
  OperatorMods/<PACKAGE_ID>/
    operator-map-package.json
    content/<DEPENDENCY_BUNDLE>
    content/<SCENE_BUNDLE>
    media/<PREVIEW>
    lighting/<OPTIONAL_LUT>
```

Do not add private absolute paths or an extra archive root.

A separately labeled multiplayer-test ZIP is transfer packaging only. Mark it
`TEST ONLY` and `NOT FOR NEXUS`, pin matching hashes on every test machine,
and keep public release records and support wording unchanged until the live
matrix passes.

## 20. Full acceptance

Use physical input and test:

1. package discovery and correct preview;
2. first Confirm once;
3. correct exact scene and terrain;
4. player position, camera, input, movement;
5. direct PVE bullets in both directions;
6. manifest enemy-count bounds;
7. leave to armory;
8. second launch;
9. alive restart;
10. Mission Failed restart;
11. every time/NVG option;
12. native all-enemies-dead extraction unlock;
13. the exact current-build ATAK exfil marker;
14. physical extraction, the native timer, Mission Successful, and Continue;
15. host and remote PVP exact agreement, content-ready and current-epoch
    scene-ready barriers, sides, movement, firearm hits, death, score, round
    respawn, retained-content restart, unload, and end;
16. map materials, lighting, trees, props, and interactives at player height.

A compile is `PROVEN-STATIC`. A controlled runtime event is
`PROVEN-RUNTIME`. A capability is `SUPPORTED` only after all stated gates
pass.

