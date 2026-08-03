# 1. Workspace setup

Create separate locations. Do not use the live game directory as the Unity
project or AssetRipper workspace.

| Location | Purpose |
| --- | --- |
| Game install | Run OPERATOR and host deployed BepInEx components and packages |
| Reference export | Inspect local AssetRipper hierarchy, materials, dependencies, and serialized evidence |
| Unity authoring project | Edit the real map scene, portable assets, payloads, and build scripts |
| Runtime source | Build the exact-build Core, generic framework, and map companion |
| Package staging | Create the final `OperatorMods/<package-id>` tree and hashes |
| Release staging | Build clean map-only or complete archives, checksums, and notes |
| Evidence workspace | Keep logs, screenshots, matrices, and private build fingerprints out of public Git |

## Create the Unity project

1. Determine the exact Unity version from the installed game and export.
2. Install that version through Unity Hub or the Unity archive.
3. Create a clean HDRP-compatible project.
4. Select Windows x86-64 before import or build.
5. Create this structure:

```text
Assets/
|-- Maps/<YourMap>/
|   |-- Scenes/
|   |-- Prefabs/
|   |-- Models/
|   |-- Materials/
|   |-- Textures/
|   |-- Terrain/
|   |-- RuntimePayload/
|   `-- Media/
|-- NativeReference/
|   |-- Models/
|   |-- Materials/
|   `-- Textures/
`-- Editor/
```

Keep the release scene at a stable path:

```text
Assets/Maps/<YourMap>/Scenes/<YourMap>.unity
```

The manifest `scenePath` MUST equal the exact path that
`AssetBundle.GetAllScenePaths()` returns.

## Use deterministic scene roots

Use a clear root structure:

```text
MapRoot
|-- TerrainAndCollision
|-- GameplayWalls
|-- Structures
|-- DirectProps
|-- Foliage
|-- PerimeterVisuals
|-- Lighting
|-- PlayerMarkers
|-- AiMarkers
`-- Metadata
```

Create inactive metadata objects named `MAP_ID_<mapId>` and
`SPAWN_SET_<spawnSet>`. Use zero-padded marker names.

## First standalone build

Create one small scene with these objects:

- one visible ground plane or small terrain;
- one usable ground collider;
- one boundary collider;
- one fallback directional light;
- one `MAP_ID_...` object;
- one `SPAWN_SET_...` object;
- required player markers;
- required PVE enemy markers for a PVE test.

Build a real scene bundle. Create a strict package. Use the physical mission UI
to load it. This test separates package, UI, scene, readiness, and spawn
problems from complex asset problems.

Do not use a prefab-overlay result as this gate.

## Runtime source separation

Keep these projects separate:

- Core and package catalog;
- generic Modded Operations framework;
- map-specific companion.

The map companion can depend on exact Core and framework versions. The generic
framework MUST NOT reference the map companion or a map ID.

## Deployment separation

Stage package data here:

```text
<staging>/BepInEx/OperatorMods/<package-id>/
```

Stage an optional companion here:

```text
<staging>/BepInEx/plugins/<map-plugin>/
```

Close OPERATOR before a copy. Back up only the exact owned destination. Copy
from staging. Compare source and destination SHA-256 values. Start a new game
process after each executable or package change.

Do not copy private logs, test controls, auto-launch files, or AssetRipper
output into staging.
