# 3b. Modded Operations mission presentation and bundle data

Status: `PROVEN-STATIC` for the current framework source and package schema.
The complete user flow stays a release gate until it passes in a normal game
session.

Use this chapter when you want a custom map to appear in the `MODDED OPS`
mission-laptop section with correct row text, briefing text, preview image,
infiltration markers, time choices, and scene launch data.

## 1. Understand the data flow

The mission presentation is not authored as a second Unity UI prefab in the
map bundle. The package supplies data. OPERATOR: Modded Operations combines
that data with private clones of shipped Cerberus UI objects.

```text
operator-map-package.json
|-- map display name
|-- preview-image path
|-- exact bundle paths and scene address
`-- operations
    |-- row and briefing text
    |-- mode and player/AI limits
    |-- infiltration labels and preview positions
    `-- time codes

verified raw preview file
        +
private shipped UI visual clones
        +
verified dependency and scene bundles
        |
        v
MODDED OPS row -> preparation page -> infiltration selector -> Confirm
        |
        v
exact package scene and native-compatible operation lifecycle
```

Keep these three data locations separate:

| Location | Put this data here | Do not put this data here |
| --- | --- | --- |
| `operator-map-package.json` | IDs, display text, preview path, bundle paths, scene path, operations, infiltration coordinates, time codes, player limits, PVE population | Unity object references, material instances, scene transforms |
| dependency and scene bundles | Unity scene, meshes, portable materials, textures, terrain payloads, lighting records, colliders, markers, authored interactive prefabs | mission-laptop text, raw UI preview file, package hashes |
| package files outside bundles | raw preview image, optional raw external LUT, manifest | executable DLLs or undeclared loose assets |

The optional map companion is a separate BepInEx plugin. It reconstructs only
map-specific native runtime state. It does not create the mission row or own
the generic briefing flow.

## 2. Create stable identities first

Choose the IDs before you build the scene:

```text
packageId:   author.example-map
mapId:       author.example-map.main
operationId: author.example-map.main-pve
spawnSet:    main-pve
```

The package directory name must equal `packageId`. The `mapId` and every
`operationId` must stay below the package namespace. Do not change a published
ID only to change visible text. Change `displayName` for visible text and
increase `version` when package bytes change.

Create these inactive scene metadata objects with exact names:

```text
MAP_ID_author.example-map.main
SPAWN_SET_main-pve
```

Create one `SPAWN_SET_...` object for each distinct operation spawn set. The
framework requires the selected map ID and selected spawn-set marker before it
creates the mode owner.

## 3. Create the preview image

The preview is a normal package file. It is not a texture inside the Unity
AssetBundle.

The current framework method `GetOrLoadPreviewSprite` does these operations:

1. Read `ModdedMapDefinition.PreviewImagePath` with `File.ReadAllBytes`.
2. Create a linear `Texture2D` with `TextureFormat.RGBA32` and no mip chain.
3. Decode the file with `ImageConversion.LoadImage`.
4. Set `TextureWrapMode.Clamp`.
5. Create a centered `Sprite` at 100 pixels per unit.
6. Cache the texture and sprite by immutable map ID.

Use a JPEG or PNG that Unity `ImageConversion.LoadImage` can decode. Use an
aspect ratio that matches the shipped board map. The framework sets
`Image.preserveAspect=true`; a mismatched aspect ratio can leave unused space.
Do not put tactical text at the extreme edge because the board and fullscreen
containers can have different dimensions.

Put the final image in the package, for example:

```text
media/example_map_preview.jpg
```

Set the map field to the same package-relative path:

```json
"previewImage": "media/example_map_preview.jpg"
```

Add the same path to `files[]` with the final byte count and lowercase
SHA-256. Do this after the image is final. The loader rejects one changed byte.

The current Ukrainian Forest evidence example is:

| Property | Value |
| --- | --- |
| Manifest path | `media/ukraine_forest_preview.jpg` |
| Format and dimensions | JPEG, RGB, `3839 x 2016` |
| Bytes | `6385660` |
| SHA-256 | `9d18a3abfa93b8b0f17a721f20731930a618b971f0b1cbd3fb97da3305ff4255` |

The map-level image is shared by all operations in that map. Schema version 1
does not have an operation-level preview field. Put PVE and PVP in separate
map definitions only when they must use different images.

### Change the image after authoring

1. Close OPERATOR.
2. Replace the file at the path in `maps[].previewImage`.
3. Measure the new file length.
4. Calculate its lowercase SHA-256.
5. Replace the matching `files[]` values.
6. Increase the package `version`.
7. Run the strict package validator.
8. Rebuild the release ZIP from the final staging directory.
9. Start a new OPERATOR process because Core freezes the package catalog at
   process startup.
10. Test the normal row, preparation page, fullscreen map, infiltration
    selector, and first Confirm.

Do not tell users to edit only the image after installation. That makes the
package fail its integrity check by design.

There is no unrestricted per-user image override in schema version 1. The
supported choices are:

- a map author ships a new package version with a different verified image;
- an author ships a separately identified map record when two operations must
  have different images;
- a user installs one of those author-built variants before OPERATOR starts.

A normal end user does not browse to an arbitrary photo from the mission UI.
This is intentional. The preview participates in package content identity and
multiplayer agreement. A future cosmetic-override feature would need a new,
explicitly non-authoritative path that cannot affect the package content ID.
Do not simulate that feature by bypassing `files[]` verification.

### Preview-author checklist

Use this exact sequence for a new map:

1. Export the final image as JPEG or PNG.
2. Put it below the package root, normally in `media/`.
3. Set `maps[].previewImage` to that package-relative disk path.
4. Add the same path once to top-level `files[]`.
5. Calculate the byte count and SHA-256 from the staged copy, not from an
   earlier source copy.
6. Keep `files[]` in ordinal path order.
7. Increase the package `version`.
8. Run closed-directory verification.
9. Confirm the framework log reports the preview loaded for the immutable
   `mapId`.
10. Inspect the preparation image, fullscreen image, and infiltration
    background in a new process.

The image does not go in `sceneBundle`, `dependencyBundles[]`,
`maps[].scenePath`, or `runtimeTerrain`. Those values use different address
domains.

## 4. Map manifest fields to the mission UI

The current framework uses these mappings:

| Manifest data | Current framework member | User-visible result |
| --- | --- | --- |
| `operations[].displayOrder` | frozen catalog sort | row order for operations in one map |
| `maps[].displayName` | frozen map catalog and diagnostics | map identity used when the framework reports or groups map-owned operations |
| `operations[].displayName` | `RewriteNativeOperationRow`, `FormatCatalogBriefing`, `TARGETPACKAGE_DETAILS.DISPLAY_NAME` | mission row name, briefing title, target package name, confirmation text |
| `operations[].mode` | `RewriteNativeOperationRow`, `CerebusOpboard.GameModeOverride` | PVE/PVP row label and generic mode selection |
| `operations[].areaOfOperation` | `RewriteNativeOperationRow`, `FormatCatalogBriefing`, `TARGETPACKAGE_DATA.OPERATION_AREA_OF_OPERATION` | row area, briefing area, native board area |
| `operations[].sitrep` | `FormatCatalogBriefing` | briefing body |
| `maps[].previewImage` | `GetOrLoadPreviewSprite`, `ReplaceNativeMapPreview`, `BuildPackageInfiltrationMapPrefab` | preparation map, fullscreen map, infiltration-selector background |
| `operations[].infiltrations` | `BuildPackageInfiltrationMapPrefab` | native marker visuals, labels, limits, and positions |
| `operations[].timeCodes` | `TARGETPACKAGE_DETAILS[]`, `ConfigureNativeSelector` | infiltration-time choices |
| `operations[].defaultTimeCode` | `SelectCatalogOperation`, `UpdateCatalogOperationBoard` | initial time selection |
| `maps[].scenePath` | `TARGETPACKAGE_DETAILS.OPERATION_SCENE` and exact bundle validation | scene address passed to the shipped board launch path |
| `operations[].minPlayers` / `maxPlayers` | catalog and operation contract | allowed operation player range |
| `operations[].minEnemies` / `maxEnemies` | server PVE population step | inclusive map-owned enemy count range |
| `operations[].spawnSet` | exact-scene metadata validation | selected marker contract |

The current row adapter supplies `30-45 MIN` and the PVE/PVP threat label from
framework presentation logic. Schema version 1 does not expose a package field
for duration or threat. Do not add unknown properties; the closed schema
rejects them.

`FormatCatalogBriefing` creates this exact text structure:

```text
<displayName>

AREA OF OPERATION // <areaOfOperation>

<sitrep>
```

Write `sitrep` as usable mission information. State the objective, expected
contact, map hazard, and coordination requirement. Do not duplicate the title
or area text in the SITREP.

## 5. Place infiltration markers on the preview

Each operation must declare at least one infiltration. An infiltration record
does not move a 3D player spawn. It places and configures one selectable marker
on the 2D preview. The operation `spawnSet` and scene player markers control
the 3D spawn contract.

The runtime creates one private map root named
`MODDED_OPERATIONS_PACKAGE_INFILTRATION_MAP`. It creates a child
`PACKAGE_MAP_PREVIEW`, clones one shipped `MapInfilMarker` visual for each
package record, and replaces every mission-bearing field:

| Package field | `MapInfilMarker` or UI assignment |
| --- | --- |
| `id` | object name `PACKAGE_INFIL_<id>` |
| array order | `MarkerIndex` |
| `displayName` | `InfilName` and selector label |
| `maxPlayers` | `MaxPlayers` |
| `mapPositionX` | `RectTransform.anchorMin.x` and `anchorMax.x` |
| `mapPositionY` | `RectTransform.anchorMin.y` and `anchorMax.y` |

The framework also sets `IsGroundInfil=true`, `IsHeliInfil=false`,
`IsExfil=false`, assigns the private operation board, zeroes current-player
state, and activates the cloned marker.

Unity UI anchors use the lower-left as `(0,0)` and the upper-right as `(1,1)`.
For example:

```json
{
  "id": "south-entry",
  "displayName": "SOUTH ENTRY",
  "mapPositionX": 0.5,
  "mapPositionY": 0.2,
  "maxPlayers": 8
}
```

This puts the marker at the horizontal center and 20 percent above the lower
edge. Verify the position in the normal selector because transparent margins
or an unusual image crop change the visual relationship.

Keep infiltration array order stable. `InfilSelectorDisplayer.SpawnMap`
discovers the active cloned markers, and the framework verifies that marker
count, `MarkerIndex`, name, player limit, and ground-infiltration flags still
equal the package records.

## 6. Declare time choices

Use 24-hour `HHMM` strings. The current schema accepts one to eight unique
values. `defaultTimeCode` must occur in `timeCodes`.

```json
"timeCodes": ["1100", "0200"],
"defaultTimeCode": "1100"
```

For each value, the framework creates a `TARGETPACKAGE_DETAILS` record:

```text
OPERATION_SCENE = maps[].scenePath
DISPLAY_NAME = operations[].displayName
INFILTRATION_TIME = the selected HHMM string
```

A time code is not sufficient lighting data by itself. The map scene or exact
runtime owner must implement the corresponding sun, Volume, fog, sky, and NVG
contract. Restore all changed process-global state on unload.

## 7. Put the correct data in each bundle

Build all related bundle definitions in one Unity build call. This lets Unity
put shared dependencies in the explicit dependency bundle and keeps the scene
bundle focused on the scene.

### Dependency bundle

Put reusable and address-loaded Unity assets here:

- complete meshes and prefabs that the scene references;
- portable material records with original native identity;
- base/alpha, normal, mask, height, thickness, detail, and audited special
  textures;
- runtime terrain height and surface-weight payload textures;
- TerrainLayer data when it survives the target runtime;
- lighting JSON or other map-scoped reconstruction records;
- package-owned LUT assets that use Unity serialization;
- audio clips and VFX assets referenced by authored objects;
- complete authorized interactive prefab dependencies.

A dependency bundle must have zero scene paths. Every asset that runtime code
loads by address must have a stable Unity asset path. Record the emitted
lowercase address from `AssetBundle.GetAllAssetNames()` and use that exact
address in the manifest or companion.

The current Ukrainian Forest dependency bundle is
`content/operator_ukrainian_forest`. Important addresses include:

```text
assets/maps/ukrainianforest/terrain/runtimepayload/ukrainianforest_expandedheight_rg16.png
assets/maps/ukrainianforest/terrain/runtimepayload/ukrainianforest_expandedsurfaceweights_rgb.png
assets/maps/ukrainianforest/lighting/ukrainianforest_day.json
assets/maps/ukrainianforest/ukrainianforestmap.prefab
assets/operatornativeassets/ukrainianpinecandidate/source/pine_leaves_4k.png
```

### Scene bundle

Put the real `.unity` scene here. The scene must contain or reference:

- one expected world root;
- terrain render and collision components or the exact reconstruction root;
- gameplay and bullet-interaction walls;
- player, PVE enemy, HVT, PVP team, and other consumed markers;
- inactive `MAP_ID_...` and `SPAWN_SET_...` metadata;
- structures, cover, foliage instances, lights, volumes, reflection data, and
  authored interactive prefab instances;
- a visible diagnostic fallback only when runtime reconstruction needs one.

The scene bundle must expose the exact path in `maps[].scenePath`. Do not use
the first scene returned by the bundle. The current Ukrainian Forest scene
bundle is `content/operator_ukrainian_forest_scene`, and its address is:

```text
Assets/Maps/UkrainianForest/Scenes/UkrainianForest.unity
```

### Package files outside bundles

Keep these files outside Unity bundles:

- the raw preview file from `maps[].previewImage`;
- an optional raw half-float external LUT declared by
  `externalTonemapLut.path`;
- `operator-map-package.json`.

Do not add a loose model or texture that neither the manifest nor a bundle
references. The package directory is closed. Every regular file except the
manifest must occur exactly once in `files[]`.

### Complete Forest file-to-consumer example

This table is the current Forest package closure. It shows why each item is in
its location.

| Package path or bundle address | Container | Direct consumer | Required result |
| --- | --- | --- | --- |
| `operator-map-package.json` | package root | Operator Mod API closed loader | freezes IDs, UI text, operation mode, limits, bundle paths, scene address, preview path, infiltration anchors, time codes, terrain record, and file hashes |
| `media/ukraine_forest_preview.jpg` | loose verified file | `GetOrLoadPreviewSprite` | one decoded sprite for preparation, fullscreen, and infiltration-selector views |
| `lighting/AgX_Powerful_RGBAHalf_32.bytes` | loose verified file | `LoadPackageTonemapLut` | exact `32 x 32 x 32` RGBA-half external tonemap LUT |
| `content/operator_ukrainian_forest` | dependency bundle | framework and Forest companion | portable native assets, terrain payloads, tree/foliage closure, materials, textures, and lighting records |
| `assets/maps/ukrainianforest/terrain/runtimepayload/ukrainianforest_expandedheight_rg16.png` | dependency-bundle address | framework `TryPrepareRuntimeTerrain` | `1025 x 1025` big-endian RG16 height samples for `TerrainData.SetHeights` |
| `assets/maps/ukrainianforest/terrain/runtimepayload/ukrainianforest_expandedsurfaceweights_rgb.png` | dependency-bundle address | framework `TryPrepareRuntimeTerrain` | `1024 x 1024` RGB terrain-layer weights for grass, dirt, and rock |
| `assets/operatornativeassets/texture2d/floor_grass_basecolor.png` | dependency-bundle address | `runtimeTerrain.layers[0].diffuse` | native grass albedo for the reconstructed TerrainLayer |
| `assets/operatornativeassets/texture2d/floor_grass_normal.png` | dependency-bundle address | grass and dirt TerrainLayer records | tangent-space normal data |
| `assets/operatornativeassets/texture2d/grassgreen_qheqg2_maskmap.png` | dependency-bundle address | grass TerrainLayer record | packed terrain mask channels |
| `assets/operatornativeassets/texture2d/dirt_0.png` | dependency-bundle address | dirt TerrainLayer record | dirt albedo |
| `assets/operatornativeassets/texture2d/floor_rock_gray_basecolor.png` | dependency-bundle address | rock TerrainLayer record | rock albedo |
| `assets/operatornativeassets/texture2d/floor_rock_gray_normal.png` | dependency-bundle address | rock TerrainLayer record | rock normal |
| `assets/operatornativeassets/texture2d/aset_rock_granite_m_rgasy_maskmap.png` | dependency-bundle address | dirt and rock TerrainLayer records | packed mask data |
| `content/operator_ukrainian_forest_scene` | scene bundle | shipped scene loader after bundle registration | exposes exactly one declared scene |
| `Assets/Maps/UkrainianForest/Scenes/UkrainianForest.unity` | scene-bundle address | `TARGETPACKAGE_DETAILS.OPERATION_SCENE` and scene validation | owns terrain root, colliders, playable walls, foliage instances, PVE/PVP markers, light, and metadata |

Do not copy the preview image into the dependency bundle and also keep it as a
loose file. That creates two possible authorities. Do not put mission row text
in a Unity `TextAsset`. The manifest is the only mission-presentation data
authority.

### Required scene objects for the same example

The Forest scene must expose these exact object or prefix contracts:

```text
MOD_UkrainianForest_Runtime
NATIVE_Ground_HillyTerrain
MODDED_MAP_METADATA
MAP_ID_community.ukrainian-forest.ukrainian-forest
SPAWN_SET_forest-pve
SPAWN_SET_forest-pvp
SCENE_CONTRACT_STANDALONE_V1
PVE_ENEMY_RANGE_10_15
PVE_EnemySpawn_*
PVE_HVTSpawn_*
Team1_Spawn_*
Team1_Backup_Spawn_*
Team2_Spawn_*
Team2_Backup_Spawn_*
MOD_UkrainianForest_OutdoorEnvironment
RENDER_PROFILE_NATIVE_OUTDOOR_V1
Nice Sun
```

The manifest does not serialize these Unity object references. It supplies
the identity keys. The framework finds and validates the scene-owned objects
after the exact scene loads.

### Empty-folder-to-briefing data recipe

The following table is the minimum complete path from a new package folder to
a launchable Modded Operations row. Complete each row before moving to the
next. “Bundle address” means the lowercase address emitted by Unity. “Disk
path” means a path relative to the package root.

| Step | Authoring input | Final container and field | Runtime proof |
| ---: | --- | --- | --- |
| 1 | stable author/package name | directory name and `packageId` | Core accepts one package namespace |
| 2 | final Unity scene | scene bundle disk file in `files[]`; `maps[].sceneBundle`; exact `maps[].scenePath` | scene bundle has the declared scene and the framework loads that exact address |
| 3 | shared meshes, textures, materials, terrain payloads, and authored prefab closure | zero-scene dependency bundle disk files in `files[]`; ordered `maps[].dependencyBundles[]` | all dependencies load before the scene and every address-loaded asset exists |
| 4 | map identity | `maps[].mapId` plus inactive scene object `MAP_ID_<mapId>` | post-load metadata validation matches both values |
| 5 | operation spawn identity | `operations[].spawnSet` plus inactive scene object `SPAWN_SET_<spawnSet>` | selected operation finds its exact scene marker |
| 6 | map title | `maps[].displayName` | catalog diagnostics and grouping show the map identity |
| 7 | mission title, order, mode, area, SITREP, player limits, and optional PVE enemy range | one complete `maps[].operations[]` record | row and preparation text match; mode owner uses the declared mode |
| 8 | final tactical photo | raw disk file, `maps[].previewImage`, and one `files[]` row | the same verified sprite appears on all three mission presentation surfaces |
| 9 | selectable 2D entries | `operations[].infiltrations[]` | native marker clones show the exact label, order, normalized position, and limit |
| 10 | 3D player/AI positions | scene transforms with supported marker names | current-scene `SpawnPoint` and `RaidManager` inputs are grounded and navigable |
| 11 | time choices | `timeCodes[]`, `defaultTimeCode`, and matching scene/companion render behavior | selector shows each code and the loaded world applies the selected presentation |
| 12 | optional reconstructed terrain | `runtimeTerrain` plus exact payload/texture addresses inside its declared dependency bundle | one live `TerrainData` drives rendering and collision |
| 13 | optional raw external LUT | verified loose file, `externalTonemapLut`, and one `files[]` row | correct `Texture3D` type, size, and tone-map owner |
| 14 | final package closure | sorted `files[]` byte counts and lowercase SHA-256 values | no undeclared file, missing file, hash mismatch, scene mismatch, or path escape |

Do not use a manifest value as a substitute for authored scene content. For
example, `spawnSet` selects a contract; it does not create spawn transforms.
Do not use a scene object as a substitute for catalog data. For example, a
Unity text object does not create a mission row or briefing SITREP.

## 8. Use a complete operation record

This example shows every required schema-version-1 operation field:

```json
{
  "operationId": "author.example-map.main-pve",
  "displayName": "EXAMPLE MAP PVE",
  "displayOrder": 0,
  "mode": "pve",
  "areaOfOperation": "EXAMPLE REGION",
  "sitrep": "Operators enter from the south. Armed contacts hold the north route. Clear the marked combat area and keep the team inside the boundary.",
  "minPlayers": 1,
  "maxPlayers": 8,
  "minEnemies": 10,
  "maxEnemies": 15,
  "spawnSet": "main-pve",
  "infiltrations": [
    {
      "id": "south-entry",
      "displayName": "SOUTH ENTRY",
      "mapPositionX": 0.5,
      "mapPositionY": 0.2,
      "maxPlayers": 8
    }
  ],
  "timeCodes": ["1100", "0200"],
  "defaultTimeCode": "1100"
}
```

For PVP, set `mode` to `pvp` and omit both enemy fields. Do not set them to
zero. The schema requires that PVP does not contain the fields.

## 9. Build, hash, and validate in the correct order

1. Save the final Unity scene and all referenced assets.
2. Run the scene validator.
3. Build dependency bundles and the scene bundle for
   `StandaloneWindows64` with the exact tested Unity version.
4. Inspect every dependency bundle with `GetAllScenePaths()` and require zero
   scene paths.
5. Inspect the scene bundle and require only declared scene addresses.
6. Inspect `GetAllAssetNames()` for every address-loaded payload.
7. Copy the final bundles, raw preview, and optional raw LUT to a new staging
   package.
8. Write the final manifest data, except file length/hash values.
9. Measure every non-manifest file from the staging package.
10. Sort `files[]` by ordinal package-relative path.
11. Write exact byte counts and lowercase SHA-256 values.
12. Run the same closed package loader that Core uses.
13. Build the Nexus archive from the validated staging directory. Keep
    `BepInEx` at the archive root.
14. Close OPERATOR and copy the exact staged bytes to the game installation.
15. Start a new process and use the physical mission laptop.

Do not modify or recompress an image after step 11. Do not build a ZIP from a
different directory than the one that passed package validation.

## 10. Verify every presentation surface

Use one normal launch and record these results:

1. `MODDED OPS` is isolated from official operation arrays.
2. The row order and row text match the selected operation.
3. A single click updates the briefing title, area, and SITREP.
4. The preparation page uses the declared preview.
5. The fullscreen map uses the same preview and keeps its aspect ratio.
6. Every infiltration marker has the declared label, order, position, and
   maximum-player value.
7. The time selector contains only declared time codes and starts at the
   declared default.
8. Cancel returns without changing the official mission selection.
9. Confirm starts once without leaving and reopening the laptop.
10. The loaded scene address equals `maps[].scenePath`.
11. The selected mode, spawn set, and PVE population contract match the
    package.
12. Restart keeps the same immutable package operation and creates one fresh
    scene generation.

If the preview fails to decode, the current framework hides the old map
children and cannot build the package infiltration-map prefab. This is a
package presentation failure. Do not continue by showing a retail mission
image.
