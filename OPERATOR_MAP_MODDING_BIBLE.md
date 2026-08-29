# OPERATOR Standalone Map Modding BIBLE

Use [`docs/13-exact-implementation-reference.md`](docs/13-exact-implementation-reference.md)
to find the current framework members, companion members, assembly names,
bundle names, scene objects, terrain payload addresses, and verified Ukrainian
Forest material identities. That reference uses privacy-safe path tokens. Do
not put a private drive path or an operating-system account name in a public
procedure.

Use
[`docs/14-end-to-end-package-lifecycle.md`](docs/14-end-to-end-package-lifecycle.md)
for the complete authoring-data, Cerberus, bundle, terrain, player, PVE/PVP,
failure, restart, and teardown chain.

Use
[`docs/15-native-pve-completion-exfil-and-atak.md`](docs/15-native-pve-completion-exfil-and-atak.md)
for the exact StandardPVE enemy-clear, extraction, ATAK, success-screen, and
restart contract.

This document is the normative technical contract for a standalone OPERATOR
map. It is written for human authors and automated engineering tools.

The public framework name is **OPERATOR: Modded Operations — Standalone Map
Framework**. Use **OPERATOR: Modded Operations** as the short product name.
Use **MODDED OPS** as the mission-laptop tab label.

This document uses the language and evidence rules in
[Documentation language and evidence rules](docs/00-writing-standard.md).

## 1. Scope and status

The current authored source checkpoint is Modded Operations `0.3.30` with the
bundled-only Operator Mod API `0.2.0-alpha.7`. Shared source builds isolated
BepInEx and MelonLoader products; install exactly one loader variant. Protocol
v6 adds fail-closed PVP and online-PVE content/scene agreement, exact suite
receipt and sidecar validation, loader-neutral companion identity, a
host-authoritative PVE enemy-count identity, and restart readiness bound to a
host-issued scene generation epoch. Its source and automated contracts are
`PROVEN-STATIC`. They are not evidence from a real host and remote OPERATOR
process. The last hash-pinned public framework publication remains `0.3.28`
with bundled API `0.2.0-alpha.5`, and the last complete runtime framework
candidate remains `0.3.24`.

A separately labeled `0.3.29`/alpha.6 archive exists only to transfer exact
bytes for a controlled multiplayer test. It is not a Nexus release, not a
standalone API release, and not proof of online support. Do not promote it
until a real host and remote client pass content agreement, exact scene
readiness, first spawn, movement, firearm-specific hit registration, score,
round respawn, retained-content restart, unload, and return-to-armory.

The earlier Forest `0.4.19` and Modded Operations `0.3.22` single-player PVE
work remains bounded runtime evidence for one-click Confirm, daylight/night
rendering, repeat launch, same-process KIA restart through the shipped Mission
Failed UI, reciprocal firearm play, native all-enemies-dead extraction unlock,
the exact ATAK exfil marker, physical 15-second extraction, and the Mission
Successful result. It does not promote the current PVP candidate or prove PVE
co-op. Re-test the complete applicable matrix after an OPERATOR, Unity,
BepInEx, Il2CppInterop, A*, Core, framework, package, or companion update.

The current first-Confirm owner retention, repeat-launch player recovery,
same-process KIA restart, deterministic network game-mode identity, tree root
contact, and local render transaction are `PROVEN-RUNTIME` for the tested
single-player scope.
The Forest-only `dense-forest-balanced-v1` behavior is also
`PROVEN-RUNTIME` for one first launch and one same-process native restart. The
two exact-package 120-second runs with the final release bytes created 15 and
14 bots, preserved positive native wander delays, moved every bot at least
1 m, moved 6 and 4 bots at least 5 m toward insertion, and recorded authored
vegetation sight obstruction. Maximum displacement was `51.19 m` and
`49.34 m`.
Owner-aware firearm population and reciprocal firearm behavior are
`PROVEN-RUNTIME` only for the stated pinned single-player Forest scope. The
current protocol-v4 PVP and PVE source remains `PROVEN-STATIC` until each
mode's real two-peer matrix passes. The existence of a PVE agreement path does
not prove remote package/scene equality, AI replication, combat, completion,
extraction, restart, or teardown.

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
| Framework download | Modded Operations plus its exact bundled preview API pair |
| Map download | One data-only map package plus its optional exact-scene companion |

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
| Preview decoder/cache | `GetOrLoadPreviewSprite` |
| Preparation/fullscreen preview binder | `ReplaceNativeMapPreview` |
| Briefing formatter | `FormatCatalogBriefing` |
| Native launch calls | `InfilSelectorDisplayer.SpawnMap`, `CerebusOpboard.Start_Operation` |
| Confirm owner retention | `BeginCatalogOperationLaunch`, `RestoreCapturedLaunchLaptop`, `SetNativeConfirmationLoadingState` |
| Verified dependency-asset loan | `LoadVerifiedMapDependencyAsset<T>(mapId, assetPath)`; framework retains bundle ownership |
| Scene contract gate | `ValidateStandaloneSceneContract` |
| Repeat-launch player recovery | `RequestStandalonePlayerSpawn`, `InvokeGeneratedServerPlayerSpawnBody`, `SpawnAndPositionStandalonePlayers` |
| Deterministic game-mode identity | PVE `0x4D4F5001`, PVP `0x4D4F5002`, `NetworkClient.RegisterPrefab`, `TryAdoptNetworkSpawnedGameMode` |
| PVE count selector | `ChooseStandalonePveEnemyCount` |
| PVE creator | `TrySpawnStandalonePveEnemies`, `RaidManager.ServerSpawnAI(false)` |
| Ukrainian Forest companion assembly | `OperatorUkrainianForest.dll` |
| Ukrainian Forest companion source file | `OperatorUkrainianForestPlugin.cs` |
| Companion exact-scene entry | `ProcessStandalonePackageScene` |
| Companion navigation owner | `EnsureStandaloneNavigationGraph` |
| Companion tree repair | `AlignStandaloneAuthoredTreesToTerrain`; packaged `NATIVE_TRUNK_GROUND_DATUM_ONE_SIXTH`; family-aware pine center and broad-oak bounded root-footprint contact; at least `0.75` of the complete tree above terrain |
| Package ID | `community.ukrainian-forest` |
| Map ID | `community.ukrainian-forest.ukrainian-forest` |
| Dependency bundle | `content/operator_ukrainian_forest` |
| Scene bundle | `content/operator_ukrainian_forest_scene` |
| Scene path | `Assets/Maps/UkrainianForest/Scenes/UkrainianForest.unity` |
| Preview file | `media/ukraine_forest_preview.jpg` |
| PVE operation and range | `community.ukrainian-forest.pve`, `10` through `15` enemies |
| PVP operation and AI count | `community.ukrainian-forest.pvp`, zero AI |
| Terrain object | `NATIVE_Ground_HillyTerrain` |

The public authoring code is
[`templates/Editor/BuildStandaloneMapBundles.cs`](templates/Editor/BuildStandaloneMapBundles.cs)
and
[`templates/Editor/ValidateStandaloneMapScene.cs`](templates/Editor/ValidateStandaloneMapScene.cs).
The closed package contract is
[`schemas/operator-map-package.schema.json`](schemas/operator-map-package.schema.json).
The full package version `0.3.17` file records, terrain payload addresses, material
identities, marker families, and load sequence are in the
[exact implementation reference](docs/13-exact-implementation-reference.md).

## 4. Four-owner architecture

Keep these owners separate.

| Owner | MUST own | MUST NOT own |
| --- | --- | --- |
| Core and catalog | Package verification, immutable identity, deterministic catalog | Map-specific Unity or game logic |
| OPERATOR: Modded Operations | Private native-style mission UI, exact bundle and scene load, readiness, `InfiltrationManager`-compatible PVE bridge, shipped `PvpGameode` lifecycle, player creation, shipped failure-UI handoff, restart | Map names, shader profiles, terrain dimensions, graph dimensions, marker coordinates, tree repair |
| Data-only map package and scene | Manifest, operations, bundles, preview, portable assets, world, collision, walls, lighting, markers, PVE population range | Executable code, generic mission UI, global game state |
| Optional exact-scene companion | Native material repair after generic terrain bind, map-owned A* graph, marker diagnostics, map-specific interactive initialization, strict world diagnostics, teardown | Catalog, generic terrain payload decoding, generic mission UI, other maps, shipped failure UI |

The package directory MUST remain data-only. Install a required map companion
as a separate selected-loader product under `BepInEx/plugins` or `Mods`.

The preview API Core and selected BepInEx or MelonLoader host remain separate
runtime owners. The transactional runtime suite owns their exact executable
files and records them in its selected-loader receipt and manifest sidecar. Do
not publish a standalone preview-API download or duplicate Core/framework
files in a map download. The runtime suite does not own or mutate map packages.

Use scene and bundle data when it can preserve the required contract. Add a
companion only for installed-runtime state that the bundle cannot preserve.

## 5. Installed layout

Use this ownership layout.

```text
OPERATOR/
|-- BepInEx/plugins/              # selected only for BepInEx
|   |-- OperatorModAPI/
|   |-- OperatorModdedOperations/
|   `-- <map-plugin>/
|-- Mods/                         # selected only for MelonLoader
|   |-- OperatorModAPI.dll
|   |-- OperatorModAPI.MelonLoader.dll
|   |-- OperatorModdedOperations.MelonLoader.dll
|   `-- <map-plugin>.MelonLoader.dll
`-- OperatorMods/                 # shared data, owned by map distributions
    `-- <package-id>/
        |-- operator-map-package.json
        |-- content/
        |-- lighting/
        `-- media/
```

Do not put a map companion DLL below `OperatorMods`. Do not put map data in the
runtime suite.

The selected-loader suite receipt owns exact executable runtime paths. A map
download owns only its `OperatorMods/<package-id>` tree. A declared companion
is installed as the selected-loader executable variant and is never inventoried
inside the data-only package.

## 6. Exact-build gate

Before a build or runtime test, record these values:

- OPERATOR executable and data fingerprints;
- Unity version;
- selected loader and exact version;
- selected loader's Il2CppInterop version and public/protected API surface;
- A* Pathfinding Project surface used by the map;
- Core version and binary hash;
- Modded Operations version and binary hash;
- map companion version and binary hash;
- selected suite receipt, exact receipt-owned manifest sidecar, and every
  selected executable path/hash;
- package manifest and every declared file hash;
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

Schema version 2 can declare one optional map-level `runtimeCompanion` when
the map requires an exact-scene selected-loader product:

```json
"runtimeCompanion": {
  "pluginGuid": "author.example-map",
  "pluginVersion": "1.2.3",
  "sha256": "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
  "melonLoaderSha256": "abcdef0123456789abcdef0123456789abcdef0123456789abcdef0123456789",
  "runtimeContentId": "28b80e3a2066ee764a12494eee98a7957f29875d3708f4283a09e4f772be1e47",
  "readyMarkerName": "AUTHOR_EXAMPLE_READY",
  "failureMarkerName": "AUTHOR_EXAMPLE_FAILED"
}
```

This is a closed contract. It binds one non-reserved lowercase namespaced
plugin GUID, one exact SemVer identity, exact BepInEx and MelonLoader DLL
SHA-256 values, one loader-neutral runtime-pair ID, and two distinct exact
scene-object names. `runtimeContentId` is the lowercase SHA-256 of the UTF-8
lines `operator-loader-neutral-runtime-pair-v1`, `pluginGuid`, `pluginVersion`,
`sha256`, and `melonLoaderSha256`, in that order with `\n` separators and no
trailing newline. Generate it with `tools/operator_runtime_content_id.py`.
Omission or explicit JSON `null` means no companion. Schema version 1 rejects
the field.

Core validates and freezes the declaration. It does not discover, load, or
execute the companion. The framework must resolve the already loaded plugin,
verify its exact GUID/version/DLL bytes, and inspect the declared markers only
inside the selected package-scene generation. The ready marker is valid only
after the companion completes its exact-scene work. The failure marker always
wins, including if it appears after ready. A PVP or online PVE peer cannot send
`SceneReady` until this contract passes. Keep the DLL outside the data-only
package tree and outside the package `files[]` inventory.

The manifest supplies the mission presentation. Do not store mission-laptop
text in a Unity scene object and expect the framework to discover it. Use this
mapping:

| Presentation result | Manifest source |
| --- | --- |
| row order | `operations[].displayOrder` |
| row and briefing title | `operations[].displayName` |
| row mode and runtime mode contract | `operations[].mode` |
| row/briefing area | `operations[].areaOfOperation` |
| briefing body | `operations[].sitrep` |
| preparation, fullscreen, and infiltration-selector image | `maps[].previewImage` |
| infiltration marker labels and 2D positions | `operations[].infiltrations[]` |
| infiltration-time choices | `operations[].timeCodes` and `defaultTimeCode` |
| exact scene target | `maps[].sceneBundle` and `maps[].scenePath` |
| 3D player and AI contract | `spawnSet` plus scene metadata/marker objects |

The preview is a verified raw package file outside the Unity bundles. The
framework reads it with `File.ReadAllBytes`, decodes it with
`ImageConversion.LoadImage`, and creates one map-owned sprite. All operations
under one map share that image. Schema version 1 does not have a
per-operation preview.

Each infiltration `mapPositionX/Y` pair is a normalized Unity UI anchor. It
places a 2D native selector marker on the preview; it does not place a player
in the scene. `(0,0)` is the lower-left and `(1,1)` is the upper-right.

The complete author procedure is in
[Modded Operations mission presentation and bundle data](docs/03b-modded-operations-presentation.md).
Use [Native mode ownership, PVE, and StandardPVP](docs/03c-native-mode-ownership-and-pvp.md)
for the exact map/framework/retail ownership boundary and the current-build
`PvpGameode` field contract.

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
1 <= minEnemies <= maxEnemies <= 100
```

A PVP operation MUST omit both enemy fields. PVP MUST create zero PVE actors.

The framework MUST expose the inclusive range only on its private cloned PVE
briefing through the native enemy-count control. It MUST NOT mutate Tier 1,
shipped Active/Simulation operation arrays, vanilla operation objects, vanilla
enemy ranges, or PVP. Confirm MUST atomically capture the displayed value, and
the host is authoritative online. The loaded scene MUST provide at least that
many navigation-valid ordinary enemy markers in a stable name-ordered subset
whose accepted X/Z positions remain at least 2 m apart after snapping.
Inactive utility markers remain eligible; active-marker count is telemetry,
not capacity. Native restart MUST retain the captured count; a fresh operation
selection MAY reset to a bounded default. The framework MUST NOT replace the
confirmed value with a hidden random reroll. `100` is a global hard cap, not a
map entitlement: each operation publishes only its evidence-backed certified
maximum.

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

Use the dependency bundle for reusable or address-loaded Unity assets:

- complete meshes and prefabs;
- portable materials and their base/alpha, normal, mask, height, thickness,
  detail, and special-map closure;
- runtime terrain height/surface-weight payload textures;
- lighting records and serialized map LUTs;
- audio, VFX, and complete authorized interactive dependencies.

Use the scene bundle for the exact `.unity` scene and its authored object
graph. Keep the raw mission preview, optional raw external half-float LUT, and
manifest outside the bundles. Every runtime-loaded Unity asset address MUST
match `AssetBundle.GetAllAssetNames()`. Every scene address MUST match
`AssetBundle.GetAllScenePaths()`.

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

For a combined crown-and-trunk renderer, use only the highest-detail
bark/trunk submesh vertices. Do not use the prefab pivot, the whole-renderer
minimum, or a generic collider bottom. A low leaf card can corrupt the minimum.
A broad-crown bark submesh can include high spreading branches. Keep native
colliders for gameplay. Apply final position, yaw, and scale before you read
vertices. Define and visually test a family-aware reference.

The current Ukrainian Forest proof uses renderer-free child
`NATIVE_TRUNK_GROUND_DATUM_ONE_SIXTH`. A narrow pine uses one sixth of the full
LOD0 trunk span and the center terrain sample. A broad oak uses the oriented
main-stem reference, a `0.25 m` additional embed, and the lowest terrain
contact across a bounded `0.60 m` through `2.00 m` lower-root footprint. Store
that contact X/Z in the datum.

At run time, calculate `correction = sampledSurfaceYAtDatum - datum.position.y`.
Do not sample the tree root center. Require the child to reach terrain within
`0.001 m`, fail when `abs(correction)>12 m`, require at least `0.75` of the
complete rendered tree above terrain, and synchronize physics once after the
corrected batch. The run-time companion uses the child marker and does not
need mesh readback.

The full pines are `Pine_var10_LOD0.prefab` and
`Pine_var11_LOD0.prefab`. Their combined renderer slots are `Pine_Needle`,
`pine_bark`, and `Trunk_pine_var4`. The three current oak prefabs use
`Bark_Mat`, `Bark_2_Mat`, and their family leaf material in one LOD0
renderer. The builder recognizes the four bark/trunk material names and reads
only their submesh indices. The complete renderer bounds remain a separate
full-tree validation extent.

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
-> framework enters the shipped GameManagerNetwork loading presentation
-> companion verifies exact package, map, operation, scene, and build identity
-> framework reconstructs manifest-declared TerrainData and validates walkable collision
-> companion repairs exact-map materials, navigation, grounding, and interactives
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

On the supported build, use `GameManagerNetwork.ShowLoadingScreen` at RVA
`0x00916210` when the exact additive package scene enters. Vanilla
`OnAllPlayersLoaded(false)` uses this method. It activates the shipped canvas,
freezes the current player body, clears velocity, and closes infiltration UI.
Call it before terrain or material preparation. Keep the matching
`HideLoadingScreen` RVA `0x0090E950` transition under native
`GameManagerNetwork` ownership.

This call closes the one-frame gap before the replacement package `GameMode`
can assert the native readiness barrier. Without the call, the camera can show
the portable brown proxy. Validate the actual canvas through
`LoadingScreen.activeSelf` and `activeInHierarchy`. The property named
`LoadingScreenVisible` is not the canvas state on this build. Its getter at
RVA `0x0091A840` returns `_hideLoadingScreenSoon` at offset `0x2A4`.

Large verified packages can remain slower than vanilla maps on a cold load.
The Forest package reads `647869804` bytes across two bundles. In the exact
combined-package run, the dependency bundle took `24.449 s`, the scene bundle
took `0.833 s`, and verified registration took `25.347 s` total. Confirm
waited `23.442 s` for the remaining selected-map work. Retail maps can use
already resident installed content. Do not remove verification or show the
proxy to hide this difference.

## 16. Mission-laptop UI contract

The framework MUST use private clones of shipped visual objects. It MUST NOT
append package operations to the official Active Operations or Operation
Simulation arrays.

Bind each private tab, page, row, board, modal, selector, Back control, and
Cancel control to the same `MissionLaptop` owner. Do not resolve a private
page through a global object-name search.

Use the shipped infiltration selector with package-owned data. Do not call a
retail setup method that requires a retail mission graph.

The manifest field `maps[].previewImage` is the sole schema-version-1
authority for the map photo. It names a verified package-relative JPEG or PNG
outside the AssetBundles. The same decoded sprite appears on the preparation
page, fullscreen map, and infiltration selector. The file MUST also occur
exactly once in sorted `files[]` with its final byte count and lowercase
SHA-256. A normal end user chooses among author-built package versions; the
mission UI does not browse to an arbitrary local image. Replacing only the
installed image correctly fails package integrity. See
[`docs/03b-modded-operations-presentation.md`](docs/03b-modded-operations-presentation.md)
for the exact file-to-consumer recipe.

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
`PlayerMaster.UserCode_CMDSpawnPlayer__NetworkIdentity`. Request 1 for an
owned player and scene generation MUST call `SpawnPlayer()` so
`ClientSpawnBS` runs. If an owned host still has no new
`PlayerSpawnedObject` after 300 frames, the host can enter the exact generated
body as one bounded repeat-generation recovery. Use that body on request 1
only for an unowned server player. Record each request before native entry and
stop an owned host after two requests, stop other routes after three requests,
or stop any route after one object/alive proof. Require the shipped
`GameManager.MovePlayerToSpawn` path after the owned object exists. This route
is `PROVEN-STATIC` until the physical player, camera, movement, repeat-launch,
and combat gates pass.

A map companion that observes late player-object callbacks MUST own the same
scene-generation boundary. On scene load, clear any prior local-player
transform hold and keep the shared ready/applied flag false. Publish the exact
destination only after the current `Terrain` and `TerrainCollider` share one
non-null `TerrainData`. During one bounded initial window, a known local root
at the old sky pose or more than `2 m` below the sampled marker surface can be
repaired through the shipped move path with velocity clear and Smooth Sync
notification. On scene unload, clear the held controller/transform, frames,
counters, pre-map support, applied flag, destination scene, and local-move
request before the persistent player returns to the armory. Never reuse this
state in the next additive-scene generation.

Runtime PVE and PVP mode owners MUST have a nonzero Mirror prefab identity on
every peer. The current framework uses `0x4D4F5001` for PVE and `0x4D4F5002`
for StandardPVP. Each peer collision-checks `NetworkClient.prefabs` and calls
`NetworkClient.RegisterPrefab` before host spawn. The host calls the asset-ID
`NetworkServer.Spawn` overload. A remote peer validates the received asset ID
and component type before it adopts the clone. Release unregisters the
operation template. Asset ID `0` is not remote-client proof.

Scene unload can destroy that template before the release callback. Unity's
managed wrapper then compares equal to null while
`NetworkClient.prefabs[assetId]` still contains it. A repeat
`NetworkClient.RegisterPrefab` can throw from `UnityEngine.Object.GetName` and
leave the native `MAP LOADED !BUG!` Restart Operation prompt in a loop.
Capture the deterministic asset ID before clearing operation state. Release
MUST remove only that ID from `NetworkClient.prefabs` and call
`NetworkClient.UnregisterSpawnHandler(assetId)` even when the object wrapper
is fake-null. Registration MUST evict the same fake-null entry before it
registers the new template. A different live object is a real collision and
MUST fail closed. Do not call `NetworkClient.ClearSpawners()` because it also
removes vanilla and other-mod registrations.

Team PVP uses one-based native IDs on this exact build.
`PvpGameode.StartNewRound()` writes `SpawnPoint.Team=1` for Team 1 and
`SpawnPoint.Team=2` for Team 2. The player identity is
`PlayerMaster.MyTeamIdentifier.TeamID`. The framework MUST keep these values
as `1` and `2`. It MUST NOT convert them to `0` and `1` or use
`TeamIdentifier.ToString()`. It MUST invalidate a cached marker after a team
change when that marker does not match the current `TeamID`.

This rule is `PROVEN-STATIC` for the fingerprinted build. The standalone PVP
owner MUST derive from `PvpGameode`. It MUST call the shipped
`PvpGameode.OnStartClient` and `PvpGameode.Server_AllPlayersLoaded` bodies.
It MUST NOT replace `StartNewRound`, `PlayerDied`, `EndRound`,
`RespawnPlayers`, score SyncVars, team-death checks, or freeze-time state.

The map scene MUST own separate Team 1 and Team 2 coordinates. The generic
framework MUST convert those markers to `SpawnPoint` objects and assign two
non-empty `Il2CppSystem.Collections.Generic.List<SpawnPoint>` instances to
`PvpGameode.Team1SpawnPoints` and `Team2SpawnPoints`.

Marker discovery MUST be mode-isolated. PVE can consume its explicit
`PVE_PlayerSpawn_` markers and shared Team 1 aliases, but it MUST ignore
PVP-prefixed markers and every Team 2 marker. PVP can consume its explicit
PVP team markers and shared team aliases, but it MUST ignore
`PVE_PlayerSpawn_`. For PVP, require at least
`ceil(maxPlayers / 2)` accepted markers for each team before launch. The
current vanilla lobby exposes at most 12 players, so a 12-player operation
requires at least six Team 1 and six Team 2 markers. Reinspect this lobby limit
after a game update.

The current retail scalar seeds are `MaxRounds=13`, `RoundsToWin=7`, and
`RoundTime=120`. The retail server replaces `MaxRounds` and `RoundTime` with
the PVP lobby settings at the all-players-loaded barrier.

The native score and result methods read more than spawn lists. The framework
MUST supply two audio sources, all 16 non-empty clip arrays, `TeleType`, clock
and score text, six result objects, two animators, `FadeOut` and `FadeIn`, and
the win/lose/tie status text. Generic silent clips are permitted when retail
audio cannot be distributed. They MUST preserve the shipped array lengths.

After `nativePvpLifecycle=true`, the framework MUST stop its generic
position-only loop. The shipped respawn coroutine owns player placement. On
unload, clear `PvpGameode.instance` only when it still points to the
operation-owned component.

Current Modded Operations `0.3.30` source uses protocol v6 for both PVP and
online PVE. Before native launch, the host freezes authenticated remote
connection objects and IDs. Every peer must validate its selected-loader suite
receipt, the exact receipt-owned manifest sidecar, every selected file record,
and loaded Core/host/framework/declared-companion path. Agreement then binds
the loader-neutral suite identity, game build/capabilities, complete package
content, map, operation, mode, spawn set, scene variant/path, time, player
range, and companion runtime-pair identity. PVE additionally binds the declared
enemy range and host-confirmed count. A remote must preload its exact verified
package and commit the exact operation before it sends `ContentReady`.

After the scene transition, each peer must create and register the exact
native mode template, validate the selected scene and mode-specific spawn
contract, and pass any declared companion READY/FAILED contract before it can
send `SceneReady`. For PVE, scene readiness also validates that the agreed count
does not exceed the navigation-valid, pairwise-spaced marker capacity. The host
owns a nonzero unsigned 64-bit scene-generation
epoch. Initial launch uses epoch 1. Each retained-content Restart increments
it exactly once. A remote maps that request to one monotonic local package
scene generation; a previous generation cannot satisfy the new barrier even
when scene callbacks arrive host-first, remote-first, or load-before-unload.
The request retries every 5 seconds inside the 90-second scene deadline.
Zero, stale, future, out-of-phase, or overflowing epochs fail closed.

Membership is immutable for the agreed session. A join, disconnect, or
replacement connection aborts even when the numeric connection ID is reused.
Late join is not supported. The protocol-v4 implementation is `PROVEN-STATIC`;
it does not itself prove PVP combat or PVE AI replication, completion,
extraction, restart, and teardown in two real processes.

Require a host and client on different teams, first spawn, freeze release,
death/respawn on both teams, score and round progression, correct
opposite-side isolation, Restart Operation, and return-to-armory before
support wording.

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
2. Resolve or add one enabled `Pathfinding.RVO.RVOSimulator` on the same host.
   The shipped `FollowerEntity` movement component requires this service.
3. Add the verified installed graph type.
4. Set dimensions from the playable physics and bullet-interaction volume.
5. Configure slope, step, erosion, neighbor, corner, height, and obstacle
   rules from measured actor limits.
6. Restore each temporary scan-layer change in a `finally` block.
7. Scan the exact map-owned graph.
8. Reject an outside marker before nearest-node lookup.
9. Limit horizontal correction.
10. Use a tight local ground test.
11. Require `AstarPath.IsPointOnNavmesh` for every mission marker class.
12. Reject readiness if `RVOSimulator.active` is null or disabled.

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

The supported `BOT V2` prefab stores `BrainAI` and `NetworkIdentity` on the
root. Its `SK_Insurgent_P8` child stores `AgentController` and the enabled
`FollowerEntity`. The root can remain stationary while the rendered bot and
navigation entity move. Read `brain.agent.position` for displacement. Do not
use `brain.transform.position` as the movement acceptance metric.

Test representative routes. Record each start, goal, path result, path length,
and failed segment.

Test bullets in both directions across routes, doors, walls, cover, and map
boundaries. A visibility result does not prove projectile passage.

## 20. `DoorV2` contract

The OPERATOR developers confirmed that normal doors are already part of the
map or building prefab. Do not create the normal door graph at run time. The
official `_DoorV2_BASE.prefab` supplied on 2026-08-03 proves this contract.
It is 260206 bytes, has SHA-256
`BAB5287B2DE809143BBDE71B90F8D0BE454DD724B4DEC110FB4AF1FC0CF06FF6`,
and uses meta GUID `803422c907641034e99a99778ef7d30b`.

Import an authorized complete source prefab and its `.meta` file with every
dependency resolved. Preserve the full graph and place a prefab instance in
the map scene or map-owned building prefab. Let normal scene and Mirror
lifecycle code initialize it.

Before placement, copy
[`templates/Editor/ValidateDoorV2Prefab.cs`](templates/Editor/ValidateDoorV2Prefab.cs)
to `Assets/Editor`, set its `PrefabAssetPath`, and run **OPERATOR Map >
Validate DoorV2 Prefab**. Require `SUMMARY errors=0` in
`Builds/OperatorDoorValidation/doorv2-prefab-validation.txt`. The code checks
the exact official GUID, component counts, serialized field names, internal
reference ownership, reciprocal handles, pivot descendants, distinct A*
links, sound arrays, destroyed-door rigid bodies, null runtime fields, and
pinned official scalars. It does not replace the live interaction,
navigation, damage, Mirror, late-join, restart, and unload tests.

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

The official file already binds every relationship above. It also serializes
reciprocal handles, FinalIK pose objects, audio arrays, a destroyed-door root,
30 destroyed-door rigid bodies, and distinct sibling link-source objects.
`MyLocalPlayer` and `PlayerInteractionSystem` are correctly null because they
are run-time state.

Preserve current-code compatibility fields even when they are not read.
`DoorModelParent`, `DoorMask`, `navCutOpenSize`, and `navCutCloseSize` occur
only in commented-out code. `RivalDoorHandle` has no current reads,
`GrabbedHandle` is write-only, and `raycastTransform` is an unused run-time
allocation. Do not remove these public serialized fields. Latch and hinge
colliders are live because `SlapChargeExplosive` reads them. `NavMeshCut`,
`canBlowup`, and the dead-door block are also live.

Some AssetRipper exports lose the custom serialized field values. Reject such
an export as a functional source. A child named `Door Pivot` does not bind a
null `PivotTransform`.

Run-time cloning or component reconstruction is `EXPERIMENTAL`, not the
normal map method. Do not call private lifecycle methods to repair an
incomplete graph.

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

The current 02:00 worked source is `sharedassets7.assets` profile path `435`,
name `PVP map NIight VOLUME`. It uses ACES without the day external LUT.
Exposure path `440` uses Automatic Histogram, compensation `1.16`, limits
`5.065281867980957..9.348570823669434`, and speeds `3/3`.
`GameManager.SetNVGColor(0)` selects white phosphor on this exact build.
Capture and restore the old color across the operation lifecycle.

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
- inactive serialized terrain fallback after successful TerrainData bind;
- complete-tree LOD0 bark/trunk submesh bounds and one renderer-free
  family-aware datum aligned to live terrain after final rotation and scale,
  with native trunk/root collision retained, the documented family reference
  and contact X/Z preserved, the maximum correction satisfied, and at least
  `0.75` of the complete rendered tree above terrain;
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
-> restore prior process-global spawn registration when the operation still owns it
-> clear standalone mode singleton and timer ownership
-> restore captured NVG color and destroy operation-owned Volume profiles
-> clear the companion's local-player transform hold, spawn-safety state, applied flag, and destination scene
-> companion removes its graph, material instances, interactives, and callbacks
-> framework releases its runtime TerrainData and TerrainLayer objects
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

For StandardPVP, static source and automated tests currently prove the shape
of exact binary/package/companion agreement, the `ContentReady` barrier, the
epoch-bound `SceneReady` barrier, immutable membership, mode-isolated marker
discovery, bounded refusal, and cleanup. They do not prove Mirror transport or
gameplay between two OPERATOR processes. Before a PVP support claim, test:

- matching exact selected-loader suite receipt/sidecar/file closure, package
  content, optional companion runtime-pair identity, game build, protocol,
  capability, operation, scene, time, player range, and session identity on
  host and remote;
- mismatch refusal before native content activation;
- exact remote package preload and operation commit before `ContentReady`;
- exact scene, mode template, spawn contract, and optional companion readiness
  on every peer before `SceneReady(epoch)`;
- first spawn, movement replication, firearm-specific hit registration, death,
  score, freeze release, round respawn, and native match end;
- six-per-team capacity in a 12-player lobby, then a separate real 12-player
  stress test if 12-player support is claimed;
- retained-content Restart with one new epoch and no stale readiness reuse;
- unload, disconnect, failure abort, and return-to-armory cleanup;
- explicit rejection of late join and every membership change.

Protocol v6 gives online PVE its own content/scene barrier and binds the
host-confirmed enemy count. Before a PVE online/co-op claim, two distinct
processes must still prove synchronized start, both peers leaving loading,
grounded players, identical agreed count and package closure, AI replication,
network movement, firearm/projectile registration, damage, completion,
extraction, Restart on both peers, disconnect/failure cleanup, and teardown.
Repeat capacity, frame-time, spawn-time, completion, Restart, and teardown at
each claimed map maximum through the global hard cap of 100. A single-player,
host-only, or join-in-progress success does not satisfy this matrix.

If the applicable matrix does not pass, state the exact tested local or static
scope. Do not advertise general online support.

## 25. Release archive boundaries

Build these archives from clean staging directories.

| Archive | Contents |
| --- | --- |
| Framework download | The exact bundled preview API Core/host pair and matching OPERATOR: Modded Operations files; no map data or map companion |
| Map-only | Data-only package and its exact-scene companion; no Core or framework DLL |
| Controlled test transfer | The same ownership split, clearly labeled `TEST ONLY` and `NOT FOR NEXUS`; exact hashes must match every test machine |

Do not publish the preview API as a standalone download. Keep it inside the
matching framework download until the API reaches a full stable release. Do
not duplicate the API or framework in a map archive. Public Nexus distribution
uses one framework download plus one separate map download.

A controlled multiplayer-test archive is not a release candidate. Its file
name and README must state that host-plus-remote proof is pending. It must not
update public release records, decompiler snapshots, or support wording merely
because its ZIP integrity and hashes pass.

Do not include logs, captures, private flags, smoke drivers, forced-scene
controls, auto-launch controls, extracted game assets without permission, or
machine-local paths.

Each public mod repository must contain the complete authored source and one
hash-pinned decompiler snapshot for each final mod DLL. Record the DLL version,
byte length, SHA-256, and decompiler version beside the snapshot. The authored
tree is the edit source. The decompiled tree proves the compiled release
surface.

Represent each omitted large or authorized asset with an explicit bracketed
record, for example `[PREFAB ASSET]`, `[PREVIEW IMAGE]`, `[TEXTURE SET]`,
`[DEPENDENCY ASSETBUNDLE]`, or `[SCENE ASSETBUNDLE]`. The record must give the
exact expected path, type, address, source evidence, and validation rule. Do
not put placeholder bytes at a manifest-declared release filename.

Do not describe generated IL2CPP interop wrappers as decompiled original game
source. Publish the exact generated signature, serialized owner file and path
ID, read-only native behavior evidence, and runtime acceptance result. Keep
game binaries out of the mod repository unless the owner has authorized that
exact redistribution.

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
| PVP agreement | Exact binary/package/companion agreement; remote preload and operation commit; every `ContentReady`; exact-generation `SceneReady(epoch)`; immutable membership; mismatch/timeout/late-join refusal |
| PVP gameplay | Zero PVE actors; `ceil(maxPlayers/2)` markers per side; host and remote first spawn, movement, firearm hits, death, score, round respawn, native end, Restart, unload, and opposite-side isolation; native IDs remain Team 1=`1` and Team 2=`2` |
| PVE online | Separate host/remote package, scene, AI placement, movement, projectile, damage, completion, extraction, restart, and teardown proof; PVP agreement is not evidence |
| Interactives | Complete applicable player, AI, network, restart, and unload matrix |
| Lifecycle | Normal restart, respawn, KIA restart, and three clean generations; PVP Restart advances one scene epoch and rejects stale readiness |
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
| First spawn throws or armory player floats | Invalid first spawn index or package-scene objects retained in process-global `GameManager` spawn fields |
| First mission works but second launch has no player object | Request 1 did not create the new scene generation's `PlayerSpawnedObject`; verify the 300-frame bounded owned-host generated-server recovery and shipped move path |
| 02:00 NVG is black outside ECOTI | Day exposure/LUT applied to the night source or white-phosphor state was not selected |
| Trees on hills float or have buried trunks | A pivot, combined-renderer minimum, generic collider bottom, or root-center resample was used; verify the family-aware LOD0 datum, broad-oak bounded lower-root contact X/Z, and `0.75` complete-tree above-ground gate |
| Pine lower trunk or branches are white | The complete raw bark/trunk state did not apply, a wrong submesh material is bound, or covering defaults remain active; require framework-owned verified raw-state asset loans and inspect both bark material slots |
| Scene loads, then `MAP LOADED !BUG!` Restart Operation loops on the second launch | A destroyed scene-owned game-mode template remained in `NetworkClient.prefabs`; inspect the deterministic asset ID, fake-null eviction before registration, and release cleanup by asset ID plus `UnregisterSpawnHandler` |
| Actor is outside the wall | Marker containment and graph dimensions use the visual apron |
| Player and AI cannot shoot through the boundary | Bullet-interaction wall or layer mask differs from route intent |
| Pine reads as a bare trunk | Tree-family crown silhouette failed despite static closure |
| Door panel swings but interaction fails | `DoorV2` reference graph is incomplete |
| KIA throws before Mission Failed UI | PVE mode owner, singleton, or `RaidTimer` is missing |
| Restart duplicates state | Scene-generation invalidation or reverse teardown is incomplete |
| Host works but client fails | Content agreement, nonzero deterministic Mirror mode asset ID, peer prefab registration, clone adoption, authority, or late-join path is not proved |
| Peer launch refuses before the scene transition | Compare the selected-loader suite receipt, exact manifest sidecar and files, complete package closure, companion runtime-pair identity, game build, mode/count identity, and frozen authenticated connection objects; version text alone is insufficient |
| Host waits at `ContentReady` | A remote did not preload the exact verified package or commit the matching operation before its acknowledgement |
| Host waits at `SceneReady` | Check the selected scene, deterministic mode template, mode-specific marker capacity/spawn contract, declared companion identity, READY marker, and FAILED-marker precedence on every peer |
| Restart accepts an old scene or times out | Inspect the host-issued UInt64 epoch, the remote monotonic local scene generation, retry/deadline logs, and stale/future/overflow refusal |
| A player joins after agreement begins and the operation aborts | Expected fail-closed behavior; current agreement membership is immutable and late join is unsupported |
| PVE agreement passes but remote play differs | Agreement is only `PROVEN-STATIC`; require the separate two-process AI replication, movement, projectile/damage, completion, extraction, restart, and teardown matrix |

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

## 29. Fixed PVE AI and foliage sight contract

The current Operator Mod API `0.2.0-alpha.7` contract and Modded Operations
`0.3.30` source support schema version 2. A schema-v2 PVE operation can
declare one immutable `pveAiProfile`. Schema v1 and PVP reject it. The
framework adds no difficulty UI and makes no global AI change.

The profile contains a stable ID, detection range, field of view, maximum
effective range, wander radius, communications flag, and counter-suppression
flag. It can also declare an initial-wander-delay cap from 2 through 60
seconds, a lowercase reaction disposition of `defensive`, `offensive`, or
`random`, and a maximum reaction-time cap from 0.10 through 1.50 seconds.
Omission preserves the corresponding native-prefab behavior; explicit JSON
`null` is invalid.

The generic framework applies the selected operation's required values to
native `BotSpawnDetails` immediately before the shipped
`RaidManager.ServerSpawnAI(false)` owner-aware population path. It applies a
declared reaction disposition before spawn. After spawn, it can advance only
the first native wander delay and cap native base/current reaction time once,
without raising a faster prefab value. It must not change native target lists,
team lists, damage, cover, aim, or later wander cycles.

Tune the profile from the authoritative combat volume, not from the visual
terrain apron. Measure all accepted player-to-enemy marker gaps. Keep the
initial detection range below the intended start gap. Keep one wander radius
below the distance that would cross the intended encounter midpoint. The
current native wander method preserves `WanderTimer * Patience`, then selects
around the current bot position. Repeated choices can create a progressive
search.

Sight occlusion remains map content. Research the exact installed vanilla
prefab and the current `EyesAI` physics mask. An exact-scene companion can
activate an authored native sight-collider child only when the prefab evidence
proves that contract. Require exact layer, collider type, authored count,
active count, collision matrix, and bullet-mask evidence. Do not create an
invisible movement or ballistic wall.

The Ukrainian Forest worked candidate uses a 70 by 140 m playable volume,
78.87 m nearest solo enemy gap, 45 m detection, 90-degree FOV, preserved
native effective range, 38 m delayed native wander, communications on,
counter-suppression off, and 183 authored barberry triggers on layer 18,
`AI_VisionBlock`. The exact split is 79 direct plus 104 perimeter. The source
scene contains 118 direct and 156 perimeter bushes because the placement
cycle also includes 91 Juniper instances. Those Junipers do not ship the
native inactive `AI Collider` child and must not receive a synthetic one. PVP
omits the profile. The implementation, first launch, and same-process restart
are `PROVEN-RUNTIME` for this exact Forest scope.

For each PVE operation that has a profile, Modded Operations tracks only the
new `BrainAI` IDs created by its native population call. It records the live
`WanderTimer * Patience` and profile destinations. It then records movement,
movement toward insertion, native target/state fields, and same-mask sight
probes at 0, 10, 30, 60, 90, and 120 seconds. The gate is the presence of the
profile. It does not contain a worked-example map ID. These reports provide
the accepted physical search and foliage-obstruction evidence for the tested
Forest scope. The current pinned single-player scope also passed reciprocal
firearm damage and native completion/extraction. Two-peer combat remains a
separate gate.

Use [AI navigation, routes, and behavior](docs/11-ai-navigation-and-behavior.md)
for the complete authoring, native-application, foliage, logging, and test
procedure. Use
[`operator-map-package-v2.schema.json`](schemas/operator-map-package-v2.schema.json)
for the machine-readable contract.

## Private unattended observer method

Use a private stationary player-camera observer when no human can steer the
player during QA. The observer must not force a scene or create a free camera.
It must enter Lone Wolf, access the player-owned `MissionLaptop`, open the real
Cerberus window and MODDED OPS tab, select one exact immutable operation ID,
press Execute and Confirm, and confirm the shipped infiltration selector.

After the exact additive scene loads, require the real owned
`PlayerSpawnedObject`, declared package spawn, Cinemachine player camera,
retail output camera, native readiness, and mode-correct actor population.
Keep the player stationary. Capture the initial player-camera result. Then
call the shipped Restart Operation path and repeat every gate for the new
scene generation.

The launcher must refuse an existing OPERATOR process and record the exact
path, process ID, and start time for each process that it creates. It must
bound the observation time, hash all logs/traces/screenshots, and remove its
driver, control files, capture directory, and process settings after the game
exits. Use at least a 122-second hold for an AI profile that records through
120 seconds. A shorter run proves only the gates that it reached.

This driver is QA code. Do not put it in the package directory, framework
archive, map archive, or end-user installation. Programmatic live-UI event
invocation does not replace a physical-pointer test when the UI click surface
itself changed. See
[Validation and release](docs/07-validation-and-release.md) for the exact
sequence and evidence record.
