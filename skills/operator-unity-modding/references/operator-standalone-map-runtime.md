# OPERATOR standalone map runtime ownership

Use this reference for a map selected through Modded Operations and loaded as
its own scene. It records the validated boundary between declarative packages,
the **OPERATOR: Modded Operations — Standalone Map Framework**, and optional
map-scoped runtime code.

Re-check installed types, signatures, versions, and behavior after every
OPERATOR, BepInEx, Il2CppInterop, or A* update.

## Contents

1. [Ownership model](#ownership-model)
2. [Activation and startup order](#activation-and-startup-order)
3. [Portable material transport](#portable-material-transport)
4. [Runtime Terrain and collision](#runtime-terrain-and-collision)
5. [Runtime A* graph construction](#runtime-a-graph-construction)
6. [Mission-marker grounding](#mission-marker-grounding)
7. [World contract](#world-contract)
8. [Selected-map cold-load prefetch](#selected-map-cold-load-prefetch)
9. [Host player-spawn boundary](#host-player-spawn-boundary)
10. [PVP team-spawn identity](#pvp-team-spawn-identity)
11. [Restart and teardown](#restart-and-teardown)
12. [Release layout](#release-layout)
13. [Validation sequence](#validation-sequence)
14. [Rejected shortcuts](#rejected-shortcuts)

## Ownership model

Treat a standalone map as up to four coordinated owners:

| Owner | Owns | Must not own |
|---|---|---|
| Core/catalog | package verification, immutable identity, deterministic catalog | map-specific Unity or game code |
| OPERATOR: Modded Operations framework | native laptop/row/board clones, infiltration selector, exact dependency/scene load, readiness, vanilla-compatible mode ownership, package-declared PVE population range, generic player/population, shipped failure-UI handoff, restart | map names, map shaders, map graph dimensions, map marker coordinates/repair, cloned mission-failure UI |
| data-only package | manifest, operations including PVE min/max population, scene/dependency bundles, preview, lighting payloads, authored world/collision/markers | executable C# or BepInEx hooks |
| optional map companion | exact-scene runtime reconstruction, strict world validation, teardown of its own graph/material/service state | official catalog mutation, generic laptop flow, other maps |

The package directory is always data-only. “The package has no DLL” does not
mean the complete map distribution can never have code. When runtime-only
reconstruction is necessary, install the companion as a separate BepInEx
plugin beside—not inside—the package root.

Prefer scene/bundle data. Add a companion only when a required native object or
installed-game shader/service cannot be transported faithfully.

## Activation and startup order

The companion must fail closed unless all of these identify the same map:

- exact accepted package ID and content/catalog identity when available;
- exact map/operation relationship;
- exact loaded scene path, not only a short scene name;
- exact supported Core, Modded Operations, game, Unity, BepInEx, and Il2CppInterop
  versions/fingerprints required by the implementation.

A safe order is:

```text
Core verifies/freezes package catalog
-> Modded Operations clones/binds private UI
-> shipped infiltration selector receives package-owned map data
-> Confirm captures the exact player-owned laptop and player
-> Modded Operations loads declared dependencies and exact scene
-> map companion verifies exact scene/package ownership
-> companion reconstructs materials/Terrain/navigation
-> companion validates strict world contract
-> Modded Operations creates the mode-compatible native owner and enters readiness
-> Modded Operations enters generic mode population
-> operation runs
```

If the exact runtime ordering differs, measure it and put a readiness barrier
between reconstruction and actor creation. Do not use arbitrary frame delays as
the authoritative contract.

## Portable material transport

An external Unity project generally cannot compile OPERATOR's private HDRP
Shader Graphs. A bundle that loads completely can therefore render brown,
flat, opaque, or with error materials while the correct scene is active.

Use portable materials as transport records:

1. Preserve original raw/native material identity independently of proxy or
   runtime wrapper names.
2. Carry base/alpha, normal, mask, tint, and audited special maps in properties
   the portable authoring shader can serialize.
3. Document every cargo slot whose runtime meaning differs from its proxy
   meaning.
4. In the exact scene, create a new Material from the installed native shader
   family.
5. Apply the audited queue, alpha cutoff, far cutoff, material-type mask,
   culling/double-sided state, passes, keywords, wind, and numeric values.
6. Move cargo textures to their real native property and disable the proxy
   property's visible meaning.
7. Rebind Renderer/Terrain material ownership without mutating a global shared
   native Material.
8. After several rendered frames, count active proxy/error shaders and fail if
   any required renderer remains unreconstructed.

Never use `CopyPropertiesFromMaterial` with an `InternalErrorShader` source.
Use the raw record as evidence and reconstruct a fresh live material.

## Runtime Terrain and collision

Treat managed Unity wrappers as potentially fake-null in IL2CPP. Use native-
aware validity before accepting TerrainData, Terrain, or TerrainCollider.

If TerrainData cannot travel safely:

- package lossless height/alphamap payloads at exact resolution;
- create fresh native TerrainData;
- set size, layers, heights, and alphamaps through IL2CPP-compatible arrays;
- bind the same live TerrainData to Terrain and TerrainCollider;
- publish collision/physics transforms before marker/actor readiness;
- fail the world contract if the binding is unusable.

Do not let a player or AI spawn merely because a visible fallback mesh exists.

If the scene carries a render-only terrain mesh for editor portability, disable
that exact fallback object immediately after the live `Terrain` and
`TerrainCollider` bind succeeds. Do not leave two coincident ground surfaces.
Keep the fallback active when reconstruction fails so the failure remains
visible, but fail readiness and do not spawn actors.

After the final TerrainData is live, align authored trees by their visible
base, not by the root transform alone. Compute the lowest world-space bound of
the tree's child renderers, sample the live terrain at the root X/Z, and move
the root so that the visible base is slightly below the surface. Use a small,
map-audited embed value and reject an excessive correction. Run the same
algorithm in the authoring builder and as a bounded exact-scene runtime repair;
then call `Physics.SyncTransforms()`.

## Runtime A* graph construction

A scene AssetBundle does not prove that the standalone session has a resident
`AstarPath` service or a scanned graph. Navigation is live runtime state.

For the current installed A* IL2CPP surface, the validated shape is:

1. Call `AstarPath.FindAstarPath()` and reuse a compatible target-scene service
   only when ownership is proven. Otherwise create one map-scoped host and add
   `AstarPath`.
2. Require `astar.data` before continuing.
3. Remove only a previous graph owned by this companion before rebuilding.
4. Add the graph through
   `astar.data.AddGraph(Il2CppType.Of<GridGraph>())`, then
   `TryCast<GridGraph>()` and fail if the native wrapper is unavailable.
5. Set centre/rotation/aspect, then call
   `GridGraph.SetDimensions(width, depth, nodeSize)` from the authoritative
   playable physics and bullet-interaction bounds. Never size it from a larger
   render-only terrain or scenery apron.
6. Configure max slope, max step, erosion, neighbour/corner policy, height
   sampling, ground requirement, and capsule/obstacle collision from the map's
   measured traversal contract.
7. Scan the specific graph with `astar.Scan(graph)`.

When OPERATOR layers cannot be authored portably, a temporary scan layer is
valid only as a scoped transaction:

```text
save Terrain.layer
-> set graph heightMask to the temporary Terrain layer
-> exclude that layer from obstacle collision
-> move only the Terrain to the temporary layer
-> Physics.SyncTransforms
-> scan
-> finally restore Terrain.layer and sync again
```

Never leave the Terrain on the scan layer, move the whole map hierarchy, or
allow the temporary mask to leak into rendered gameplay.

## Mission-marker grounding

Apply the contract to every marker class the operation can consume: ordinary
enemies, HVTs, bosses, reinforcements, or any map-specific AI role. Validating
only the first generic enemy list is incomplete.

Before nearest-node lookup, test every marker against the map-authored playable
volume with explicit wall clearance. A marker outside that volume must be
quarantined so generic discovery cannot consume it, and the world contract must
fail when the required valid-marker count is no longer available. Graph
coverage is not a substitute for this check: a graph sized from a visual apron
can contain walkable nodes behind a physics/bullet barrier.

For each marker:

1. Resolve the nearest walkable node with the installed A* constraint.
2. Reject a missing node.
3. Measure horizontal correction separately from vertical correction.
4. Reject horizontal correction beyond a small map-authored limit; a distant
   node can silently move encounters into another lane.
5. Move to the node centre plus the exact intended foot clearance.
6. Sync Physics transforms.
7. Prove collision ground within a tight local window around the marker.
8. Prove `AstarPath.IsPointOnNavmesh(marker.position)`.
9. Log rejected marker names and distances and fail the world contract when
   any required marker fails.

A long downward ray is invalid as grounding proof. It can hit terrain tens of
metres below an airborne marker. Use a post-snap check such as a ray starting
only slightly above the marker and bounded to roughly the local foot-clearance
window, adjusted for the actual actor origin/collider.

## World contract

Before generic mode population, require at least:

- exact scene/package identity;
- expected world root and nontrivial authored transform count;
- required active renderer/collider counts or explicit map-specific minima;
- zero active portable/error-shader renderers;
- usable Terrain/TerrainCollider when the map declares terrain;
- exactly the intended map-owned navigation graph/service count;
- graph centre/dimensions equal the authoritative playable physics/bullet
  volume rather than the visual terrain extent;
- every operation-consumed marker is inside that volume before nav lookup;
- every required mission marker tightly grounded;
- every required mission marker on the resident graph;
- no rejected marker or missing critical asset/material closure.

For PVE, the data-only package owns `minEnemies` and `maxEnemies`. Require
`1 <= minEnemies <= maxEnemies <= 64` and at least `minEnemies` valid markers.
The generic host chooses an inclusive deterministic count without Unity global
random state. PVP omits both fields. Modded Operations must not hard-code a
map's range.

On the current candidate, filter `GameManager.AllAITypes` for root `BrainAI`,
root `NetworkIdentity`, `WeaponsAI.SpawnWeapon=true`, and a non-empty
`weaponList`. Give those prefabs and package-valid markers to a scene-owned
`RaidManager`. Call `RaidManager.ServerSpawnAI(false)`. Current-build native
inspection shows that it uses owner-aware
`NetworkServer.Spawn(bot, GameManager.instance.gameObject)` before it applies
`BotSpawnDetails`. Keep this claim at `PROVEN-STATIC` until reciprocal firearm
damage passes. A grenade result is not sufficient.

Log actual/expected counts and fail closed. Do not reduce the contract to
“sceneLoaded fired” or “there is a terrain somewhere.”

## Selected-map cold-load prefetch

An external package can be much larger than a shipped mission's post-Confirm
content gate. Do not wait for Confirm before every cold bundle request when a
stable row selection is already available.

For the current generic candidate:

1. Start one selected-map prefetch after the immutable row selection.
2. Load dependency bundles in manifest order, then the scene bundle.
3. Keep the content ID with the cache entry.
4. Unload an incomplete or stale same-map entry before a new request.
5. Do not prefetch all maps.
6. If Confirm occurs during the request, attach the exact laptop, player,
   operation, and time code to that request.
7. Call native start only after scene-path validation succeeds.
8. Log each file name, byte count, request time, total time, and remaining
   Confirm wait.

Unity does not expose a safe cancellation contract for an active
`AssetBundleCreateRequest`. Finish the one bounded request. Do not start an
unbounded speculative queue.

This method moves cold I/O earlier. It does not remove the bytes. Treat it as
`PROVEN-STATIC` until a physical row-to-Confirm run records the expected
timings and launches once.

## Host player-spawn boundary

The persistent `GameManager` player-spawn fields are process-global state. A
standalone generation must capture `GameManager.SpawnPointsInScene`,
`GameManager.instance.Pspawns`, and `GameManager.instance.PnextSpawnIndex`
before it installs package-scene markers. It must assign both the list and
array to the same verified current-scene marker set and set
`PnextSpawnIndex=0`. Current native `nextSpawnPosition` code reads the current
index before it increments it, so an initial value of `1` skips marker zero.

Restore the captured values on operation unload, restart, framework unload, or
transition back to the armory, but only while the current globals still have
the exact list/array identity owned by this operation. Before restoring, remove
captured entries whose owning scene is no longer loaded. This prevents a
standalone scene from leaving destroyed transforms in the armory and avoids
overwriting state that another owner installed later.

On the current exact build, an owned `PlayerMaster` must enter
`PlayerMaster.SpawnPlayer()`. That command path reaches the shipped spawn body
and then `ClientSpawnBS`, which completes owner-side locomotion, camera, and
input initialization. Calling the generated server body directly for an owned
host can create a visible player while skipping that owner-side completion.

The generated server body is
`PlayerMaster.UserCode_CMDSpawnPlayer__NetworkIdentity`. Native inspection
shows that it selects a shipped `SpawnPoint`, instantiates the shipped player
prefab, calls owner-aware `NetworkServer.Spawn`, assigns the spawned-player
SyncVar and object, and sends the retail spawn RPC.

Use the generated server body only for a server-side `PlayerMaster` that has no
local ownership. Pass its network identity. Do not construct an independent
player prefab and do not call Unity or Mirror lifecycle methods manually.

Record a spawn attempt before invoking native code, because native code can
complete synchronously and re-enter the adapter. Keep a per-player completed
set and a small attempt ceiling, such as three attempts per generation. Clear
both only when the operation generation changes. This prevents an owned
player's successful spawn from being followed by unbounded duplicate requests.

Record the selected route and whether `PlayerSpawnedObject` became non-null.
Then require the shipped `GameManager.MovePlayerToSpawn` path and player-camera
proof above the package terrain. A camera under the terrain with repeated null
spawn requests is a player-spawn failure, not proof that the map scene failed
to load.

This exact-build host route is `PROVEN-STATIC`. It becomes `SUPPORTED` only
after the physical host run produces the player object, movement, correct
camera, and reciprocal combat. Test remote clients and late join separately.

## PVP team-spawn identity

The map scene owns the marker transforms and marker names. The generic
framework converts only the selected package scene's verified markers to
current-scene `SpawnPoint` components.

On the current exact build, the native PVP team contract is one-based:

- `PvpGameode.StartNewRound()` sets Team 1 spawn points to `Team=1`;
- it sets Team 2 spawn points to `Team=2`;
- `PlayerMaster.MyTeamIdentifier.TeamID` contains the same numeric identity;
- `GameManager.nextSpawnPosition(true, teamId, iteration)` compares that
  identity directly with `SpawnPoint.Team`.

Therefore, assign Team 1 markers to `SpawnPoint.Team=1` and Team 2 markers to
`SpawnPoint.Team=2`. Read `TeamIdentifier.TeamID`. Do not use a zero-based
`0/1` conversion. Do not parse or compare `TeamIdentifier.ToString()`.
Invalidate a cached player-to-marker assignment when it does not match the
player's current team.

Keep the evidence boundary clear. The current standalone owner is
`StandalonePvpGameMode : PvpGameode`. Assign separate non-empty
`Team1SpawnPoints` and `Team2SpawnPoints` native lists. Use the shipped scene
seeds `MaxRounds=13`, `RoundsToWin=7`, and `RoundTime=120`. The retail server
replaces `MaxRounds` and `RoundTime` from the current lobby settings.

Call the shipped `PvpGameode.OnStartClient` body. It establishes the static
owner, calls `GameMode.Initialize`, and starts the native clock on the server.
Call the shipped `Server_AllPlayersLoaded` body. It resolves both team-player
caches, sets the round timer, freezes the players, and starts the shipped
respawn coroutine. Do not replace `StartNewRound`, `PlayerDied`, `EndRound`,
`RespawnPlayers`, score SyncVars, team-death checks, or freeze time.

Supply every reference that the native hooks read: two audio sources, 16
non-empty announcer arrays with retail lengths, `TeleType`, clock/score text,
six outcome roots, two animators, `FadeOut` and `FadeIn`, and win/lose/tie
text. Use a non-null silent clip when retail audio cannot be distributed.

Stop the generic repeated position path after `nativePvpLifecycle=true`. On
teardown, clear `PvpGameode.instance` only when it still points to the
operation-owned component.

Treat the numeric mapping and owner graph as exact-build `PROVEN-STATIC`.
Require a host and remote client on different teams. Prove first spawn,
freeze release, bullet death, score update, round respawn, team switch if it
is supported, opposite-side isolation, zero PVE AI, correct facing, Restart
Operation, and return-to-armory.

## Restart and teardown

Normal Restart Operation must unload the old scene generation and create one
fresh map runtime. The companion must:

- remove its graph through the owning `astar.data.RemoveGraph` path;
- destroy a map-scoped AstarPath host only when the companion created it;
- destroy/restore its material and Terrain objects according to ownership;
- restore the previous process-global player-spawn list, array, and index when
  the current values are still the operation-owned values;
- restore the previous NVG colour and remove only the operation-owned runtime
  Volume/profile;
- unsubscribe callbacks and clear scene-lifetime handles;
- prevent stale async completions from mutating the new generation;
- prove one graph/service and one callback generation after restart.

Keep lifecycle claims precise:

- alive/normal Restart proves same-package/same-scene reload;
- death/respawn proves player recreation in the running operation;
- KIA/end-screen Restart proves retail failure UI and persistent end-screen
  state for a standalone operation.

None implies the others. If the KIA path dereferences missing retail
end-screen state, report that as a generic lifecycle gap without retracting a
separately proven exact-scene load or alive restart.

### Verified standalone PVE failure/restart owner

On the current verified build, the persistent shipped `GameManagerNetwork`
already has a complete `MissionFailedPopup`, Current Time, Personal Best,
Status, and Restart Operation control. Do not clone, reparent, or rebind those
objects and do not place them in the map bundle or companion.

Native `GameManagerNetwork.FailOperation()` first dereferences
`InfiltrationManager.instance` and reads the manager's `RaidTimer`. A bare
standalone `GameMode` therefore allows selection, load, player spawn, and AI
spawn but throws after native KIA. The validated generic bridge is:

1. Use an `InfiltrationManager`-compatible component for standalone PVE, while
   keeping other modes on their separately verified owners.
2. Explicitly assign both `InfiltrationManager.instance` and
   `GameMode.singleton` to that component.
3. Initialize and advance the synchronized `NetworkRaidTimer` after the
   shipped all-players-loaded barrier.
4. Suppress native `InfiltrationManager` callbacks that dereference an
   official operation scene graph; route readiness/all-players-loaded through
   the bounded generic adapter instead.
5. On scene unload, restart, or plugin unload, clear both singleton references
   when they still point at the standalone component.
6. Prove the full native sequence: lethal damage, native death,
   `FailOperation`, shipped Mission Failed popup, shipped Restart control, a
   new exact package-scene generation, playable player, and mode-correct AI.

This bridge belongs in generic Modded Operations lifecycle code. Material, Terrain,
navigation, and marker reconstruction remain map-companion work. No donor
scene is required.

## Release layout

Use this separation:

```text
BepInEx/
  OperatorMods/<package-id>/
    operator-map-package.json
    content/...
    media/...
  plugins/<map-plugin>/
    <map-plugin>.dll
```

The generic framework archive must contain no map data or map companion. A
map-only archive may contain its data package plus its own companion, but no
Core/Modded Operations DLL. A complete archive may contain both ownership domains.

Every archive must exclude QA drivers, auto-launch flags, forced-scene modes,
private logs, and test control files. Validate staged checksums and ZIP entries.

## Validation sequence

1. Validate manifest/schema/file closure and exact scene/dependency contents.
2. Build Core and generic adapter with map-name isolation checks.
3. Build the map companion against the exact installed runtime.
4. Prove unsupported package/scene identities cause no mutation.
5. Use the physical mission laptop: tab isolation, row, Back, Execute, Cancel,
   Confirm, shipped infiltration selector, exact scene.
6. In PVE, prove player count, AI count, all required marker grounding/on-graph,
   playable-wall containment, reciprocal combat across representative routes,
   package population range, visuals, collision, and normal restart.
7. In PVP, place host and client on different teams. Prove Team 1 and Team 2
   first spawn on their authored sides, one death/respawn per team, correct
   facing, zero PVE AI, and the normal restart contract.
8. Run multiple load/restart/unload generations and prove no duplicate graph,
   callback, service, actor, or scene.
9. Test death/respawn and KIA/end-screen restart as separately named gates;
   for PVE record the `InfiltrationManager` owner/timer reset across restart.
10. Remove the QA plugin and perform a clean startup with release bytes.
11. Revalidate source/deployed/staged/ZIP hashes and archive boundaries.

## Rejected shortcuts

- Putting map-specific shader or A* logic in OPERATOR: Modded Operations.
- Putting `GameManagerNetwork` failure UI or PVE lifecycle singleton/timer
  ownership in the map bundle/companion.
- Rebinding or cloning a complete retail Mission Failed popup when the missing
  state is the native game-mode owner.
- Calling the complete map distribution “data-only” when it needs a companion.
- Putting a companion DLL beneath `BepInEx/OperatorMods`.
- Treating a brown exact scene as proof that the selector loaded the wrong map.
- Treating a successful exact scene load as proof of native materials.
- Treating a scene-bundled navigation artifact as proof of a resident graph.
- Sizing a graph from the visual terrain apron or treating an on-graph marker
  beyond the gameplay wall as valid.
- Hard-coding one map's enemy count or coordinates in Modded Operations code.
- Accepting a 64-metre or otherwise long downward ray as marker grounding.
- Validating enemies but omitting HVT/other mission marker classes.
- Scanning with a temporary layer and failing to restore it in `finally`.
- Leaving the old graph/service alive across restart.
- Claiming death-screen Restart from an alive restart test.
