# 4. Standalone runtime integration

Status: `SUPPORTED` for the stated current-build local PVE/PVP scope. Re-check
the exact installed build and run the full matrix before a public release.

Use the standalone method when the map MUST appear in the mission section and
load as its own scene.

## Ownership model

Keep four owners separate.

| Owner | Owns | MUST NOT own |
| --- | --- | --- |
| Core and catalog | Package validation, immutable identity, deterministic catalog | Map-specific Unity or game code |
| Generic Modded Operations framework | Private native-style UI, exact bundle and scene load, readiness, native-compatible mode owner, generic player/PVE/PVP lifecycle, shipped failure UI handoff, restart | Map names, shader profiles, terrain dimensions, graph sizes, marker coordinates |
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
-> framework verifies and loads dependency bundles in manifest order
-> framework verifies the exact scene path in the scene bundle
-> framework loads the exact scene additively
-> companion verifies package, map, operation, scene, and build identity
-> companion reconstructs materials, TerrainData, navigation, and interactives
-> companion validates the strict world contract
-> framework creates the native-compatible mode owner
-> shipped all-players-loaded readiness completes
-> framework installs current-scene player markers
-> framework creates or moves players
-> framework creates mode-correct PVE actors from valid sorted markers
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

The selection MUST stay stable from row click through restart. Do not use a
mutable row index or an official operation identity as package identity.

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
- runtime initialization of map-specific surfaces, audio, or interactive
  objects such as a fully wired `DoorV2`;
- teardown of objects and state that the companion owns.

The companion MUST NOT own catalog discovery, mission UI, selection, generic
scene loading, player readiness, generic PVE/PVP population, failure UI, or
generic mode ownership.

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
7. Call `Physics.SyncTransforms`.
8. Prove collision and surface height before marker or actor readiness.

Do not let an actor spawn because a visible fallback mesh exists.

## Navigation ownership

A scene bundle does not prove that a live A* service or scanned graph exists.
Navigation is runtime state.

When a companion owns graph construction, it MUST:

1. Resolve or create one map-scoped `AstarPath` service.
2. Add and configure the installed native graph type.
3. Size the graph from the authoritative physics and bullet-interaction
   volume.
4. Exclude the render-only terrain and scenery apron.
5. Restore any temporary scan layer in a `finally` block.
6. Scan the exact graph.
7. Reject outside markers before nearest-node lookup.
8. Require tight grounding and `IsPointOnNavmesh` for each marker class.
9. Remove only the map-owned graph or service on unload.

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

## Readiness and actor creation

The framework MUST wait for all of these conditions:

- exact scene loaded;
- exact map and spawn-set markers found;
- companion world contract passed;
- mode owner valid;
- shipped all-players-loaded barrier complete;
- loading screen closed;
- current-scene player spawn list installed.

Only then can the framework create or move players. Only the server can create
PVE actors. PVP MUST create zero PVE actors.

For PVE, the package declares `minEnemies` and `maxEnemies`. The framework
selects one inclusive deterministic count from the valid sorted markers. It
MUST NOT use Unity global random state for this selection. It MUST fail when
fewer than `minEnemies` valid markers remain.

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
-> clear current-scene spawn registration
-> clear standalone mode singleton and timer ownership
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

## Retired overlay method

MapBridge remains a local overlay and diagnostic tool. It is not the current
mission method. See the [archived MapBridge workflow](archive/legacy-mapbridge-overlay.md).
