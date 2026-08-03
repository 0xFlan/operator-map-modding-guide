# 10. Standalone package format and loading

Status: `SUPPORTED` for the current pre-v1 package contract. The contract can
change before version 1. Pin the exact Core and framework version.

The machine-readable contract is
[`schemas/operator-map-package.schema.json`](../schemas/operator-map-package.schema.json).

## Directory layout

Use this layout:

```text
BepInEx/
|-- OperatorMods/
|   `-- author.example-map/
|       |-- operator-map-package.json
|       |-- content/
|       |   |-- example_map_assets
|       |   `-- example_map_scene
|       |-- lighting/
|       |   `-- outdoor-rgba-half-32.bytes
|       `-- media/
|           `-- preview.png
`-- plugins/
    `-- AuthorExampleMap/
        `-- AuthorExampleMap.dll
```

The DLL is optional. It is not part of the data-only package.

## Manifest top-level fields

The manifest is `operator-map-package.json`.

| Field | Type | Requirement |
| --- | --- | --- |
| `$schema` | string | Exact package-schema identifier required by the installed Core |
| `schemaVersion` | integer | Exact supported schema version |
| `packageId` | namespaced ID | Must equal the package directory name |
| `displayName` | string | 1 to 80 display characters |
| `version` | string | Semantic Versioning 2.0 text |
| `maps` | array | 1 to 16 map definitions |
| `files` | array | 1 to 128 complete file records |

Use lowercase namespaced IDs. Use this shape:

```text
author.package
author.package.map
author.package.operation
```

A map ID and operation ID MUST stay below the owning package namespace.

## Map fields

| Field | Type | Requirement |
| --- | --- | --- |
| `mapId` | namespaced ID | Immutable map identity |
| `displayName` | string | Map name for the UI |
| `sceneBundle` | safe relative path | Bundle that contains the exact `.unity` scene |
| `dependencyBundles` | unique array | Ordinal load-before-scene order |
| `scenePath` | Unity asset path | Exact `Assets/.../*.unity` path in `GetAllScenePaths()` |
| `previewImage` | safe relative path | Declared package image |
| `externalTonemapLut` | object or null | Optional verified raw LUT contract |
| `runtimeTerrain` | object or null | Optional exact runtime reconstruction contract |
| `operations` | array | 1 to 32 operation definitions |

A dependency bundle MUST contain no scene. The scene bundle MUST expose the
exact declared scene path. Do not discover the first scene in the bundle.

## Operation fields

| Field | Type | Requirement |
| --- | --- | --- |
| `operationId` | namespaced ID | Immutable operation identity |
| `displayName` | string | Mission-row name |
| `displayOrder` | integer | 0 to 1023; unique in one map |
| `mode` | `pve` or `pvp` | Selects the generic runtime contract |
| `areaOfOperation` | string | Operation-board area text |
| `sitrep` | string | 1 to 4000 characters |
| `minPlayers` | integer | 1 to 64 |
| `maxPlayers` | integer | 1 to 64 and not less than `minPlayers` |
| `minEnemies` | integer | Required for PVE; 1 to 64 |
| `maxEnemies` | integer | Required for PVE; not less than `minEnemies`; 1 to 64 |
| `spawnSet` | token | Matches one scene `SPAWN_SET_...` marker |
| `infiltrations` | array | 1 to 16 package-owned infiltration choices |
| `timeCodes` | unique array | One or more `HHMM` 24-hour values |
| `defaultTimeCode` | string | Must occur in `timeCodes` |

PVP MUST omit `minEnemies` and `maxEnemies`. The generic framework MUST create
zero PVE actors for PVP.

For a 10-to-15 actor PVE operation, use:

```json
"mode": "pve",
"minEnemies": 10,
"maxEnemies": 15
```

The scene MUST have at least ten valid enemy markers for this example. The
server selects an inclusive deterministic count. The map scene owns the marker
positions. The generic framework owns count selection and actor creation.

## Infiltration fields

| Field | Type | Requirement |
| --- | --- | --- |
| `id` | token | Stable infiltration identity |
| `displayName` | string | Selector label |
| `mapPositionX` | number | Normalized preview position from 0 to 1 |
| `mapPositionY` | number | Normalized preview position from 0 to 1 |
| `maxPlayers` | integer | 1 to 64 |

The framework creates private package-owned board data. It can clone a shipped
marker visual. It MUST replace every mission-bearing value with package data.
It MUST NOT write the official mission selection.

## File closure

Each file record contains:

| Field | Type | Requirement |
| --- | --- | --- |
| `path` | safe relative path | Forward slashes; no absolute path, colon, backslash, `.` segment, or `..` segment |
| `bytes` | integer | Exact final file length |
| `sha256` | string | Exact lowercase SHA-256 of final bytes |

The `files` array MUST contain every regular package file except
`operator-map-package.json`. The loader adds that manifest name to the closed
directory set and binds the exact manifest bytes separately into package
content identity. Keep file entries in strict ordinal path order. Compute
lengths and hashes only after all builds and copies finish.

Core MUST measure and verify files before it exposes the package. It MUST
reject a content change during measurement.

## Package identity

Use immutable logical IDs and content hashes.

- `PackageContentId` binds the manifest and every declared file.
- `CatalogId` binds the accepted catalog in deterministic ID order.
- Moving identical bytes to another safe root MUST NOT change content
  identity.
- A file path, Unity instance ID, mutable list index, or official mission ID
  MUST NOT be the durable package identity.

Multiplayer content agreement needs more than version text. It needs exact
package content, game build, protocol, required capabilities, and session
identity. Do not claim multiplayer agreement before the two-peer mismatch and
late-join matrix passes.

## Scene metadata markers

Create inactive metadata objects with these exact names:

```text
MAP_ID_<mapId>
SPAWN_SET_<spawnSet>
```

Create one spawn-set object for each operation spawn set. The framework MUST
find the exact map ID and selected spawn set before it creates the mode owner.

## Player and AI marker names

Use zero-padded ordinal suffixes. The framework sorts names with ordinal
comparison.

For PVE:

```text
PVE_PlayerSpawn_00
PVE_PlayerSpawn_01
PVE_EnemySpawn_00
PVE_EnemySpawn_01
PVE_HVTSpawn_00
```

Accepted player aliases can include:

```text
Team1_Spawn_00
Team1_Backup_Spawn_00
PVP_Team1Spawn_00
```

For team PVP:

```text
Team1_Spawn_00
Team1_Backup_Spawn_00
Team2_Spawn_00
Team2_Backup_Spawn_00
```

A marker supplies position and facing. Put its foot point on the intended
collision surface. Keep headroom and remove overlapping solid colliders.

The framework converts current-scene player transforms into the shipped
`SpawnPoint` type after world validation. It replaces the active global spawn
list with only the current package scene's points before native player
creation.

## Optional runtime terrain fields

Use `runtimeTerrain` only when serialized `TerrainData` cannot materialize as
a valid native object.

The definition contains:

- exact terrain root object name;
- dependency bundle path;
- Unity asset path for the height payload;
- height encoding;
- Unity asset path for the surface-weight payload;
- surface-weight encoding;
- heightmap, alphamap, base-map, and detail resolutions;
- detail resolution per patch;
- origin and size;
- exactly three layer definitions for the current contract.

Each layer contains a name, diffuse texture, normal texture, mask texture,
tile size, normal scale, metallic value, and smoothness value.

The verified height transport uses lossless 16-bit normalized samples. The
verified three-layer weight transport uses three normalized 8-bit channels.
The companion reconstructs IL2CPP-compatible arrays and binds the same live
`TerrainData` to render and collision components.

## Optional external tone-map LUT

An external LUT definition contains:

- declared package path;
- dimension 16, 32, or 64;
- format `rgba-half`.

The file length MUST equal:

```text
dimension * dimension * dimension * 8 bytes
```

The runtime creates a linear, one-mip half-float `Texture3D` from verified raw
bytes. Use a serialized fallback light and safe tone-map fallback when the LUT
cannot load.

## Load validation sequence

1. Parse the closed manifest with strict size and depth limits.
2. Validate all IDs, text bounds, modes, ranges, and cross-field rules.
3. Resolve every relative path below the package root.
4. Reject reparse points and path escapes.
5. Verify directory closure.
6. Verify every file length and SHA-256.
7. Compute immutable package content identity.
8. Freeze the catalog in exact ordinal ID order.
9. On selection, verify the operation belongs to the selected map.
10. Verify the selected time and infiltration belong to the operation.
11. Load dependency bundles in the declared order.
12. Reject a dependency bundle that contains a scene.
13. Load the scene bundle.
14. Require the exact scene path.
15. Load the exact scene.
16. Validate exact map and spawn-set metadata.
17. Run the companion world contract when the map requires it.
18. Continue to native-compatible mode readiness.

## Build and package sequence

1. Build dependency bundles.
2. Build the scene bundle.
3. Call `GetAllScenePaths()` and record the exact scene path.
4. Run structural scene and bundle validators.
5. Create the final package tree in a staging directory.
6. Compute final lengths and SHA-256 values.
7. Write the final sorted file table.
8. Run the same package loader that Core uses.
9. Close OPERATOR.
10. Install the exact staged bytes.
11. Start a new process.
12. Use the physical mission UI for the runtime matrix.

Do not edit a package after hash generation. Do not deploy while OPERATOR is
running.
