# OPERATOR Standalone Map Modding BIBLE

Use [`docs/13-exact-implementation-reference.md`](docs/13-exact-implementation-reference.md)
to find the current framework members, companion members, assembly names,
bundle names, scene objects, terrain payload addresses, and verified Ukrainian
Forest material identities. That reference uses privacy-safe path tokens. Do
not put a private drive path or an operating-system account name in a public
procedure.

This document is the normative technical contract for a standalone OPERATOR
map. It is written for human authors and automated engineering tools.

The public framework name is **OPERATOR: Modded Operations — Standalone Map
Framework**. Use **OPERATOR: Modded Operations** as the short product name.
Use **MODDED OPS** as the mission-laptop tab label.

This document uses the language and evidence rules in
[Documentation language and evidence rules](docs/00-writing-standard.md).

## 1. Scope and status

The current standalone method is `SUPPORTED` for its tested current-build,
local PVE and PVP scope. Re-test the complete matrix after an OPERATOR, Unity,
BepInEx, Il2CppInterop, A*, Core, or framework update.

The method does not yet prove general multiplayer content agreement, late
join, or all possible maps. Treat these claims as separate gates.

The current first-Confirm owner-retention and owner-aware firearm-population
corrections are `PROVEN-STATIC`. Do not label those corrections `SUPPORTED`
until one physical first Confirm and reciprocal firearm damage pass.

The old retail-scene prefab overlay is `RETIRED` for standalone mission
parity. Keep it only for an explicit diagnostic test. See
[Archived methods](docs/archive/README.md).

## 2. Evidence terms

Use one of these labels for each claim.

| Label | Meaning |
| --- | --- |
| `SUPPORTED` | The complete method passed a current-build runtime test for the stated scope. |
| `PROVEN-STATIC` | Serialized data or installed metadata proves the structure. Runtime behavior is not fully proved. |
| `EXPERIMENTAL` | The method can be implemented, but the complete runtime matrix did not pass. |
| `RETIRED` | Keep the method only for history or diagnostics. |

Do not use one passed claim as proof of another claim. Record exact scene
load, rendering, terrain, collision, spawn, AI, restart, and multiplayer as
separate results.

## 3. Product and identity terms

Use these public names.

| Purpose | Name |
| --- | --- |
| Public product | OPERATOR: Modded Operations — Standalone Map Framework |
| Short product name | OPERATOR: Modded Operations |
| Mission-laptop tab | MODDED OPS |
| Technical component | Modded Operations Framework |
| Combined first-install archive | OPERATOR Modded Operations Starter Pack |

Do not change an existing plugin GUID, assembly name, package ID, map ID, or
operation ID to match a new display name. Treat these values as immutable
technical identities.

Use *Cerberus* only for the shipped mission-laptop location and its native UI
contract. Do not use *Cerberus Mod* as the public framework name.

### 3.1 Current implementation locators

Use these exact technical identities for the current worked reference. Do not
change an identity to match a display name.

| Object | Exact identity |
| --- | --- |
| Generic framework assembly | `OperatorModdedOperations.dll` |
| Generic framework source file | `CerberusNativeTabFix.cs` |
| Native selector builder | `BuildPackageInfiltrationMapPrefab` |
| Native launch calls | `InfilSelectorDisplayer.SpawnMap`, `CerebusOpboard.Start_Operation` |
| Confirm owner retention | `BeginCatalogOperationLaunch`, `RestoreCapturedLaunchLaptop`, `SetNativeConfirmationLoadingState` |
| Scene contract gate | `ValidateStandaloneSceneContract` |
| PVE count selector | `ChooseStandalonePveEnemyCount` |
| PVE creator | `TrySpawnStandalonePveEnemies`, `RaidManager.ServerSpawnAI(false)` |
| Ukrainian Forest companion assembly | `OperatorUkrainianForest.dll` |
| Ukrainian Forest companion source file | `OperatorUkrainianForestPlugin.cs` |
| Companion exact-scene entry | `ProcessStandalonePackageScene` |
| Companion navigation owner | `EnsureStandaloneNavigationGraph` |
| Package ID | `community.ukrainian-forest` |
| Map ID | `community.ukrainian-forest.ukrainian-forest` |
| Dependency bundle | `content/operator_ukrainian_forest` |
| Scene bundle | `content/operator_ukrainian_forest_scene` |
| Scene path | `Assets/Maps/UkrainianForest/Scenes/UkrainianForest.unity` |
| PVE operation and range | `community.ukrainian-forest.pve`, `10` through `15` enemies |
| PVP operation and AI count | `community.ukrainian-forest.pvp`, zero AI |
| Terrain object | `NATIVE_Ground_HillyTerrain` |

The public authoring code is
[`templates/Editor/BuildStandaloneMapBundles.cs`](templates/Editor/BuildStandaloneMapBundles.cs)
and
[`templates/Editor/ValidateStandaloneMapScene.cs`](templates/Editor/ValidateStandaloneMapScene.cs).
The closed package contract is
[`schemas/operator-map-package.schema.json`](schemas/operator-map-package.schema.json).
The full version `0.3.6` file records, terrain payload addresses, material
identities, marker families, and load sequence are in the
[exact implementation reference](docs/13-exact-implementation-reference.md).

## 4. Four-owner architecture

Keep these owners separate.

| Owner | MUST own | MUST NOT own |
| --- | --- | --- |
| Core and catalog | Package verification, immutable identity, deterministic catalog | Map-specific Unity or game logic |
| OPERATOR: Modded Operations | Private native-style mission UI, exact bundle and scene load, readiness, native-compatible mode ownership, generic player and PVE/PVP lifecycle, shipped failure-UI handoff, restart | Map names, shader profiles, terrain dimensions, graph dimensions, marker coordinates, tree repair |
| Data-only map package and scene | Manifest, operations, bundles, preview, portable assets, world, collision, walls, lighting, markers, PVE population range | Executable code, generic mission UI, global game state |
| Optional exact-scene companion | Native material and `TerrainData` reconstruction, map-owned A* graph, marker validation, map-specific interactive initialization, strict world validation, teardown | Catalog, generic mission UI, other maps, shipped failure UI |

The package directory MUST remain data-only. Install a required map companion
as a separate BepInEx plugin.

Use scene and bundle data when it can preserve the required contract. Add a
companion only for installed-runtime state that the bundle cannot preserve.

## 5. Installed layout

Use this ownership layout.

```text
BepInEx/
|-- plugins/
|   |-- OperatorModAPI/
|   |-- OperatorModdedOperations/
|   `-- <map-plugin>/
|       `-- <map-plugin>.dll
`-- OperatorMods/
    `-- <package-id>/
        |-- operator-map-package.json
        |-- content/
        |-- lighting/
        `-- media/
```

Do not put a map companion DLL below `BepInEx/OperatorMods`. Do not put map
data in the framework archive.

## 6. Exact-build gate

Before a build or runtime test, record these values:

- OPERATOR executable and data fingerprints;
- Unity version;
- BepInEx version;
- Il2CppInterop version;
- A* Pathfinding Project surface used by the map;
- Core version and binary hash;
- Modded Operations version and binary hash;
- map companion version and binary hash;
- package manifest and file hashes;
- source state and dirty-worktree state.

Fail closed when a required fingerprint does not match. Give the user an
actionable error. Do not install hooks or mutate Unity state after this
failure.

## 7. Package contract

The package root MUST contain `operator-map-package.json`. The package
directory name MUST equal `packageId`.

Use namespaced immutable IDs.

```text
author.package
author.package.map
author.package.operation
```

The manifest MUST declare:

- schema version;
- package ID, display name, and semantic version;
- map ID and display name;
- exact scene-bundle path;
- dependency bundles in load order;
- exact Unity scene path;
- preview image;
- operations;
- complete regular-file closure;
- final file lengths and SHA-256 values.

The `files` array MUST contain each regular package file except the manifest.
Core MUST bind the exact manifest bytes separately into package content
identity.

Core MUST reject an unknown field, undeclared file, unsafe path, reparse
point, path escape, duplicate ID, invalid range, changed file, length
mismatch, or hash mismatch before Unity loads package content.

Use the exact machine-readable contract in
[the package schema](schemas/operator-map-package.schema.json). See
[Standalone package format and loading](docs/10-standalone-packages.md).

## 8. Operation contract

Each operation MUST declare one exact map and spawn set. It MUST declare its
mode, player range, infiltration choices, time codes, display text, and
preview relationship.

A PVE operation MUST declare `minEnemies` and `maxEnemies`. Require this
relationship:

```text
1 <= minEnemies <= maxEnemies <= 64
```

A PVP operation MUST omit both enemy fields. PVP MUST create zero PVE actors.

The framework MUST select an inclusive deterministic PVE count from valid,
sorted markers. It MUST NOT use Unity global random state for this selection.

## 9. Bundle contract

Build dependency bundles before the scene bundle. A dependency bundle MUST
contain no scene.

The scene bundle MUST contain a real `.unity` scene. Record the exact path
from `AssetBundle.GetAllScenePaths()`. Do not select the first scene by list
position.

The scene MUST own:

- the complete world hierarchy;
- terrain and render geometry;
- collision and gameplay walls;
- lighting and volume objects;
- player, enemy, HVT, and other mission markers;
- map and spawn-set metadata;
- portable model, texture, and material records.

A prefab-only bundle is not a standalone mission.

## 10. Asset extraction contract

Treat an AssetRipper export as evidence. Do not treat it as a complete runtime
object.

For each model, preserve:

- complete root and child transforms;
- source scale and handedness;
- pivot and orientation;
- highest authored LOD;
- vertices, indices, submeshes, and material-slot order;
- normals, tangents, UV channels, and vertex colors;
- bounds;
- collider type and dimensions;
- component graph;
- complete material and texture closure.

Do not collapse a hierarchy when a pivot, component, material, or interaction
depends on it. Do not substitute a low LOD for the release asset.

See [AssetRipper to standalone scene bundles](docs/03a-assetripper-to-bundle.md)
and [Asset data contracts](docs/12-model-texture-material-terrain.md).

## 11. Portable material contract

An external authoring project usually cannot compile OPERATOR private HDRP
Shader Graphs. Treat each portable material as a transport record.

Preserve these values for each native material identity:

- native shader family;
- render queue;
- alpha-test state and cutoff;
- culling and double-sided state;
- depth and shadow passes;
- keyword set;
- base color and tint;
- normal, mask, height, thickness, and transmission controls;
- wind and vertex-data controls;
- texture properties;
- each proxy cargo property and native destination property.

In the exact scene, use this sequence:

1. Resolve the original native material identity.
2. Create a new material from the installed native shader family.
3. Apply the audited render state.
4. Move each portable texture to its native property.
5. Disable the proxy meaning of each cargo property.
6. Bind the material only to map-owned renderers or terrain.
7. Keep a map-owned handle for teardown.
8. Audit the live material after several rendered frames.

Do not select a profile from `NATIVE_PROXY_*`, `MOD_*`, or another wrapper
name. Do not call `CopyPropertiesFromMaterial` with an
`InternalErrorShader` source. Do not mutate a globally shared installed
material.

## 12. Foliage and tree-family contract

For each foliage material, verify:

- base-map alpha at the native property;
- alpha-test state and exact cutoff;
- double-sided or culling state;
- depth and shadow passes;
- normal and mask properties;
- material-type mask;
- wind and vertex-color controls;
- transmission or thickness data;
- render queue;
- mip behavior at middle and far distance.

Ordinary alpha blending is not a cutout repair.

A tree family MUST include complete trunk, branch, leaf-card, material,
texture, collider, and LOD closure. Test the crown at close, middle, and far
player-camera distance. Reject a family that reads as bare trunks even when
its static structure is complete.

Use deterministic nonuniform placement. Preserve routes, spawn clearance,
door clearance, sight lines, and performance limits.

## 13. Terrain and collision contract

Record the terrain origin, size, heightmap resolution, alphamap resolution,
base-map resolution, detail resolution, patch size, and ordered terrain
layers.

If serialized `TerrainData` is fake-null or unusable in IL2CPP, use this
runtime sequence:

1. Verify the exact package and scene.
2. Load and length-check lossless height and surface-weight payloads.
3. Create one native `TerrainData`.
4. Set all declared resolutions and size.
5. Create the declared terrain layers.
6. Decode heights into an IL2CPP-compatible rank-two array.
7. Decode and normalize weights into an IL2CPP-compatible rank-three array.
8. Set heights and alphamaps.
9. Bind the same `TerrainData` to `Terrain` and `TerrainCollider`.
10. Apply map-owned terrain material state.
11. Call `Physics.SyncTransforms`.
12. Verify collision and surface height at corners, routes, spawns, and AI
    markers.

Do not create an actor because a visible fallback mesh exists.

## 14. Gameplay volume and visual apron

Define these envelopes separately:

| Envelope | Purpose |
| --- | --- |
| Playable physics volume | Player and AI collision containment |
| Bullet-interaction volume | Reciprocal projectile containment |
| Native terrain apron | Continuous height, normal, and layer result beyond the wall |
| Render-only scenery buffer | Horizon continuity outside the native terrain apron |

Put gameplay walls at the intended playable limit. Continue terrain data
beyond the wall. Do not put a visible terrain or layer seam on the wall.

Use the playable physics and bullet-interaction volume for navigation and
mission-marker validation. Do not use the visual apron for those tasks.

## 15. Standalone load order

Use this order.

```text
Core validates and freezes packages
-> Modded Operations creates private native-style mission UI
-> user selects one immutable package operation
-> framework binds package-owned board and infiltration data
-> framework starts one selected-map bundle prefetch
-> Confirm joins that request or uses the same-content cache
-> framework verifies dependency bundles in manifest order
-> framework verifies and loads the exact scene bundle and scene path
-> companion verifies exact package, map, operation, scene, and build identity
-> companion reconstructs native materials, TerrainData, navigation, and interactives
-> companion passes the strict world contract
-> framework creates the native-compatible mode owner
-> shipped all-players-loaded readiness completes
-> framework installs current-scene player markers
-> framework creates or moves players
-> framework creates mode-correct server PVE actors
-> operation runs
```

A scene-loaded callback is not a world-ready signal. Do not use an arbitrary
frame delay as the authoritative readiness contract.

## 16. Mission-laptop UI contract

The framework MUST use private clones of shipped visual objects. It MUST NOT
append package operations to the official Active Operations or Operation
Simulation arrays.

Bind each private tab, page, row, board, modal, selector, Back control, and
Cancel control to the same `MissionLaptop` owner. Do not resolve a private
page through a global object-name search.

Use the shipped infiltration selector with package-owned data. Do not call a
retail setup method that requires a retail mission graph.

At Confirm, capture the exact player-owned `MissionLaptop` and its exact
`PlayerNetworking` before asynchronous package I/O. Keep the private modal
visible and non-interactable while content loads. If the same laptop releases
only that field, restore the captured owned player. Close the modal and call
`CerebusOpboard.Start_Operation` in the same final frame. Do not make the user
leave and re-enter the laptop.

A stable row selection can start one bounded selected-map bundle prefetch.
Load dependencies in manifest order, then the scene bundle. Keep the content
ID with the cache. Confirm MUST attach to the same request. Do not prefetch all
maps. Log file bytes, per-bundle time, total time, and remaining Confirm wait.

Test with physical pointer input. A direct UnityEvent call is diagnostic
evidence only.

## 17. Player spawn and mode ownership

Install only the selected package scene's player markers before native player
creation. Convert them to the shipped spawn-point contract when the verified
game path requires it.

Wait for exact scene identity, world validation, mode ownership, the shipped
readiness barrier, and the current-scene spawn list.

On the current exact build, `PlayerMaster.SpawnPlayer()` and
`SpawnPlayerServer()` both enter the Mirror command sender. The generated
server implementation is
`PlayerMaster.UserCode_CMDSpawnPlayer__NetworkIdentity`. A host adapter can
enter that exact method after server readiness. Keep the command path for a
non-server owned client. Require a non-null spawned-player object and the
shipped `GameManager.MovePlayerToSpawn` path. This route is `PROVEN-STATIC`
until the physical player, camera, movement, and combat gates pass.

For standalone PVE, a bare `GameMode` is insufficient. The generic framework
MUST provide:

- an `InfiltrationManager`-compatible owner;
- `InfiltrationManager.instance`;
- `GameMode.singleton`;
- a synchronized `RaidTimer`;
- bounded suppression of official-scene-only callbacks;
- cleanup of the singleton and timer state on unload.

Keep the shipped persistent `GameManagerNetwork` as the owner of Mission
Failed UI and Restart Operation. Do not clone the failure UI. Do not load a
donor mission to obtain failure state.

## 18. A* navigation and marker contract

A scene bundle does not prove that a live A* graph exists.

The exact-scene companion MUST:

1. Resolve or create one map-scoped `AstarPath` service.
2. Add the verified installed graph type.
3. Set dimensions from the playable physics and bullet-interaction volume.
4. Configure slope, step, erosion, neighbor, corner, height, and obstacle
   rules from measured actor limits.
5. Restore each temporary scan-layer change in a `finally` block.
6. Scan the exact map-owned graph.
7. Reject an outside marker before nearest-node lookup.
8. Limit horizontal correction.
9. Use a tight local ground test.
10. Require `AstarPath.IsPointOnNavmesh` for every mission marker class.

Apply this contract to enemies, HVTs, bosses, reinforcements, and each other
role that the operation can create.

Do not use a long downward ray as grounding proof. Do not treat graph
membership behind a bullet barrier as a valid spawn.

See [AI navigation, routes, and behavior](docs/11-ai-navigation-and-behavior.md).

## 19. AI behavior and reciprocal combat

Use the shipped AI stack. The map owns route shape, cover geometry,
collision, and markers. The framework owns generic actor creation. Native AI
systems own perception, movement, weapons, cover search, and off-mesh
traversal.

Create generic PVE actors through the shipped owner-aware
`RaidManager.ServerSpawnAI(false)` path. Supply only registered prefabs with
root `BrainAI`, root `NetworkIdentity`, enabled weapon spawning, and a
non-empty weapon list. Do not use a one-argument manual network spawn. A
working grenade does not prove a working firearm owner.

Test representative routes. Record each start, goal, path result, path length,
and failed segment.

Test bullets in both directions across routes, doors, walls, cover, and map
boundaries. A visibility result does not prove projectile passage.

## 20. `DoorV2` contract

An AssetRipper door shell is `PROVEN-STATIC` evidence only. The inspected
exports can contain null `DoorV2` and `DoorHandleV2` fields. A child named
`Door Pivot` does not bind `PivotTransform`.

Prefer a complete compatible live native template. Clone the complete root
while it is inactive.

A complete door graph needs:

- hinge-axis `PivotTransform`;
- `DoorModelParent`;
- panel `Rigidbody`;
- `MilkRigidbodySync`;
- door, latch, and hinge colliders;
- paired front and back `DoorHandleV2` objects;
- side-specific FinalIK interaction objects;
- audio source and compatible sound arrays;
- latch and hinge damage parts;
- locked, unlocked, and optional destroyed visual state;
- `NavmeshCut`;
- distinct verified walkable and openable `NodeLink2` relationships;
- valid Mirror ownership and registration.

Assign every required reference before activation. Let Unity and Mirror call
their normal lifecycle methods. Do not call private lifecycle methods to
repair an incomplete graph.

Require front and back interaction, IK, latch, lock, collision, damage,
breach, destruction, AI open, AI breach, host, client, late join, restart,
and unload tests before a `SUPPORTED` claim.

See [Interactive prefabs and DoorV2](docs/09-interactive-prefabs-and-doorsv2.md).

## 21. HDRP, light, and camera contract

Identify the target scene's actual light and Volume owners. Do not infer
ownership from a component name.

Use one verified directional-light owner. Disable only competing map or
template presentation objects that the standalone contract does not need.
Do not add a second directional fill light.

Apply only source-confirmed Volume overrides. Do not enable an inactive
component because its serialized child fields contain values.

Test materials, terrain, foliage, optics, lasers, exposure, bloom, and shadows
through the normal player camera. An editor view or offscreen camera is not
release evidence.

See [HDRP, lighting, and fidelity](docs/06-hdrp-and-fidelity.md).

## 22. Strict world contract

Before actor creation, require:

- exact package, map, operation, spawn set, and scene identity;
- expected world root and nontrivial authored transform count;
- required active renderer and collider counts;
- zero required proxy or error-shader renderers;
- usable terrain and a matching `TerrainCollider` on the same object when
  declared, with both components bound to the same `TerrainData`;
- one intended map-owned A* graph and service relationship;
- every mission marker inside the gameplay volume before graph lookup;
- every required marker tightly grounded and on the live graph;
- enough ordinary enemy markers for the selected PVE operation's package
  `minEnemies`; a selected PVP operation can have zero AI markers;
- no missing critical model, material, texture, collider, or interactive
  reference.

Treat wall clearance as a world-space distance. If a bounds collider uses a
scaled transform, convert that distance into collider-local units separately
for each checked axis before comparing it with `BoxCollider.size`. Do not
subtract world metres directly from local extents.

Fail closed when one required condition is false. Log expected and actual
values.

## 23. Restart and teardown contract

Use reverse ownership order.

```text
stop mode population
-> invalidate the scene generation
-> clear current-scene spawn registration
-> clear standalone mode singleton and timer ownership
-> companion removes its graph, materials, TerrainData, interactives, and callbacks
-> unload package scene
-> release scene bundle
-> release dependency bundles in reverse order
-> clear operation selection when the operation ends
```

Each asynchronous completion MUST carry a scene-generation ID. It MUST do
nothing when the generation is stale.

Record these lifecycle claims separately:

- normal alive Restart Operation;
- death and respawn in the running operation;
- KIA to shipped Mission Failed UI;
- shipped Restart control to one fresh exact scene;
- scene unload and process restart.

After restart, require one scene generation, graph, callback generation,
mode owner, and expected actor set.

## 24. Multiplayer boundary

A local host result does not prove multiplayer support.

Before a multiplayer claim, test:

- exact host and client package content agreement;
- mismatch refusal before content activation;
- deterministic catalog and operation resolution;
- scene load on each peer;
- player and AI authority;
- interactive-object state;
- late join;
- restart;
- disconnect and teardown;
- version, hash, game-build, protocol, capability, and session identity.

If this matrix does not pass, label the release as local or host-only for the
tested scope.

## 25. Release archive boundaries

Build these archives from clean staging directories.

| Archive | Contents |
| --- | --- |
| Framework-only | Core and OPERATOR: Modded Operations files; no map data or map companion |
| Map-only | Data-only package and its exact-scene companion; no Core or framework DLL |
| Complete starter pack | Framework ownership domain plus one map ownership domain |

Do not include logs, captures, private flags, smoke drivers, forced-scene
controls, auto-launch controls, extracted game assets without permission, or
machine-local paths.

Verify each staged file, checksum file, ZIP entry, installed file, update,
downgrade, removal, and rollback path.

## 26. Minimum release matrix

Require these results for the exact release bytes.

| Area | Required result |
| --- | --- |
| Package | Strict schema, safe paths, complete file closure, exact hashes, immutable identities |
| UI | Physical MODDED OPS tab, row, board, Back, Execute, Cancel, Confirm, selector, official-tab isolation |
| Scene | Exact declared scene; no hidden retail gameplay scene |
| Rendering | Native material reconstruction; zero required proxy/error shaders |
| Terrain | Render and collision agreement at routes, spawns, markers, and edges |
| Foliage | Complete close, middle, and far crown silhouettes |
| Navigation | One playable-only graph; all mission marker classes inside, grounded, and on graph |
| PVE | Count inside package range; actors stay inside walls; reciprocal combat |
| PVP | Zero PVE actors; correct team spawn isolation |
| Interactives | Complete applicable player, AI, network, restart, and unload matrix |
| Lifecycle | Normal restart, respawn, KIA restart, and three clean generations |
| Performance | Stable player-camera frame time at declared actor and foliage load |
| Deployment | Source, stage, ZIP, and installed hashes agree while OPERATOR was closed |

Do not call the package ready while a user-reported visual, collision, spawn,
navigation, combat, interaction, restart, or multiplayer defect is not
retested.

## 27. Common fault isolation

| Symptom | First responsible layer to test |
| --- | --- |
| First Confirm does nothing but a second laptop interaction works | The framework released `MissionLaptop.playerNetworking` during asynchronous package I/O; capture and restore the exact same-laptop owner before native start |
| Exact scene is brown or flat | Portable material reconstruction, not scene selection |
| Actor falls from the sky | Terrain collision, marker foot position, graph grounding, and readiness order |
| Player camera is under terrain and movement does not work | `PlayerMaster.PlayerSpawnedObject`, host server spawn execution, and the shipped move-to-spawn path |
| Actor is outside the wall | Marker containment and graph dimensions use the visual apron |
| Player and AI cannot shoot through the boundary | Bullet-interaction wall or layer mask differs from route intent |
| Pine reads as a bare trunk | Tree-family crown silhouette failed despite static closure |
| Door panel swings but interaction fails | `DoorV2` reference graph is incomplete |
| KIA throws before Mission Failed UI | PVE mode owner, singleton, or `RaidTimer` is missing |
| Restart duplicates state | Scene-generation invalidation or reverse teardown is incomplete |
| Host works but client fails | Content agreement, authority, registration, or late-join path is not proved |

Use [Troubleshooting](docs/08-troubleshooting.md) for the complete
symptom-to-layer table.

## 28. Documentation maintenance

After a reproducible result, update information in this order:

```text
raw evidence
-> exact fingerprint and result
-> regression test or validator
-> project evidence log
-> reusable skill reference
-> public Bible and manual
```

Do not promote an untested workaround. Keep historical methods in the archive
and mark their status clearly.
