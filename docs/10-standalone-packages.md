# 10. Standalone package format and loading

Status: `SUPPORTED` for the current pre-v1 package contract. The contract can
change before version 1. Pin the exact Core and framework version.

The framework in this document is **OPERATOR: Modded Operations — Standalone
Map Framework**.

The machine-readable contract is
[`schemas/operator-map-package.schema.json`](../schemas/operator-map-package.schema.json).
Start from the annotated
[`templates/operator-map-package.example.json`](../templates/operator-map-package.example.json)
only after you read its
[`template instructions`](../templates/README-package-template.md). The zero
lengths and hashes in that example are placeholders, not release values.

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

## How the map record becomes a mission presentation

`previewImage` is a package-relative raw image path. It is not a Unity asset
address and is not inside `sceneBundle` or a dependency bundle. The framework
reads the verified file bytes, decodes them with
`ImageConversion.LoadImage`, creates one map-owned sprite, and uses that
sprite for:

- the preparation-page map under `OperationBoardUI.MapParent`;
- the fullscreen map under `OperationBoardUI.FullscreenMapParent`;
- the background of the package-owned infiltration map passed to
  `InfilSelectorDisplayer.SpawnMap`.

One map definition has one preview. Every operation in that map shares it.
The current schema has no per-operation preview field. `preview.png` in the
directory example is only an example name. The current Ukrainian Forest uses
`media/ukraine_forest_preview.jpg`.

To replace an image, close OPERATOR, replace the final file, update its exact
`files[]` byte count and SHA-256, increase the package version, validate the
closed directory, rebuild the release archive, and start a new process. Core
freezes the catalog once per process.

See
[Modded Operations mission presentation and bundle data](03b-modded-operations-presentation.md)
for the exact decoder, dimensions example, UI targets, and validation matrix.

## Operation fields

| Field | Type | Requirement |
| --- | --- | --- |
| `operationId` | namespaced ID | Immutable operation identity |
| `displayName` | string | Mission-row name |
| `displayOrder` | integer | 0 to 1023; unique in one map |
| `mode` | `pve` or `pvp` | Selects the PVE bridge or shipped `PvpGameode` contract |
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

PVP MUST omit `minEnemies` and `maxEnemies`. The framework MUST create zero
PVE actors for PVP. It MUST create a `PvpGameode`-derived owner, wire separate
Team 1 and Team 2 spawn lists, and leave rounds, scores, deaths, and respawn to
the shipped native methods. See
[Native mode ownership, PVE, and StandardPVP](03c-native-mode-ownership-and-pvp.md).

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

The two map-position values configure a 2D UI anchor. They do not configure a
3D spawn. The framework assigns both `RectTransform.anchorMin` and
`anchorMax` to `(mapPositionX,mapPositionY)` and sets `anchoredPosition` to
zero. In Unity UI coordinates, `(0,0)` is the lower-left and `(1,1)` is the
upper-right. Array order becomes native `MapInfilMarker.MarkerIndex`.

For each package record, the framework clones the shipped marker visual and
sets these fields from package data:

| Runtime field | Package source |
| --- | --- |
| object name `PACKAGE_INFIL_<id>` | `id` |
| `MarkerIndex` | infiltration array index |
| `InfilName` | `displayName` |
| `MaxPlayers` | `maxPlayers` |
| `IsGroundInfil` | fixed `true` for this schema contract |
| `IsHeliInfil` and `IsExfil` | fixed `false` |

The framework refuses the selector when the shipped clone does not contain
the same number of markers or when its marker index, name, player limit, or
type flags differ from the package.

## Operation field consumption

The manifest is the source for mission text and choices. The scene does not
store these UI values.

| Package field | Mission use |
| --- | --- |
| `displayOrder` | stable operation row order inside the map |
| `displayName` | row title, briefing title, target-package display name, Confirm text |
| `mode` | PVE/PVP row label and selection of the PVE bridge or shipped `PvpGameode` owner |
| `areaOfOperation` | row area and `AREA OF OPERATION //` briefing line |
| `sitrep` | briefing body |
| `minPlayers`, `maxPlayers` | operation player contract |
| `minEnemies`, `maxEnemies` | inclusive PVE server population range after world readiness |
| `spawnSet` | exact `SPAWN_SET_<spawnSet>` scene metadata and marker contract |
| `infiltrations` | 2D native selector marker list |
| `timeCodes` | native infiltration-time choices and target records |
| `defaultTimeCode` | initially selected time |

The current framework formats the briefing as:

```text
<displayName>

AREA OF OPERATION // <areaOfOperation>

<sitrep>
```

For each time code, it creates one native `TARGETPACKAGE_DETAILS` with
`OPERATION_SCENE=maps[].scenePath`,
`DISPLAY_NAME=operations[].displayName`, and
`INFILTRATION_TIME=<time code>`. Unknown manifest properties do not add new
UI features; the closed schema rejects them.

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

On the current exact OPERATOR build, team IDs are one-based. Set
`SpawnPoint.Team` to `1` for a Team 1 marker and to `2` for a Team 2 marker.
Read the player's numeric identity from
`PlayerMaster.MyTeamIdentifier.TeamID`. Do not convert these values to `0`
and `1`. Do not infer the team from `TeamIdentifier.ToString()`.

If the framework caches a marker for one player, it MUST check the marker
again after a team change. It MUST remove the cached assignment when the
marker does not belong to the current `TeamID`. This numeric rule is
`PROVEN-STATIC` for the fingerprinted build. Reinspect the native PVP methods
after a game update.

The scene marker objects do not need an editor-authored `PvpGameode`
component. The map scene owns the coordinates. The generic framework creates
the network mode owner and assigns the two lists after it validates the exact
map and spawn set. Do not add a dummy `PVP` or `Mirror_TeamDeathmatch`
component; neither type owns the retail round state.

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

The `heightPayload`, `surfaceWeightsPayload`, and every terrain-layer texture
value are Unity asset addresses inside `runtimeTerrain.dependencyBundle`.
They are not package file paths. Build the assets into that dependency bundle,
call `GetAllAssetNames()`, and copy the emitted addresses exactly. The top-level
`files[]` record declares the dependency bundle file itself; it does not list
each Unity asset inside that bundle.

Use this distinction:

| Value | Address domain |
| --- | --- |
| `sceneBundle`, `dependencyBundles[]`, `previewImage`, `externalTonemapLut.path`, `files[].path` | package-relative disk path |
| `scenePath` | exact Unity scene address returned by `GetAllScenePaths()` |
| `runtimeTerrain.heightPayload`, `surfaceWeightsPayload`, and layer texture paths | exact Unity asset address returned by `GetAllAssetNames()` |

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

## Complete presentation and content check

Before you call the package complete, trace every user-visible value to one
source and every runtime asset to one bundle:

| Result | Required source |
| --- | --- |
| MODDED OPS row exists | accepted operation record in the frozen catalog |
| row title/mode/area are correct | `displayName`, `mode`, `areaOfOperation` |
| briefing title/body are correct | `displayName`, `areaOfOperation`, `sitrep` |
| board and fullscreen image are correct | verified raw file at `previewImage` |
| selector background is correct | the same decoded map preview sprite |
| selector marker label and position are correct | infiltration `displayName` and normalized X/Y |
| time choices are correct | `timeCodes` and `defaultTimeCode` |
| Confirm targets the intended scene | verified `sceneBundle` plus exact `scenePath` |
| scene renders | dependency bundle contains the portable asset closure and the companion rehydrates native-only state |
| terrain collides | live `TerrainData` is bound to both render and collider or authored collision is valid |
| players and AI use correct places | selected `spawnSet` metadata plus current-scene 3D markers |

Do not use a preview-image position as a player spawn position. Do not put
mission text in a Unity object and expect the catalog to discover it. Do not
put the raw preview only inside an AssetBundle and expect `previewImage` to
decode it.
