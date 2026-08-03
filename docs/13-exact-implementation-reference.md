# Exact implementation reference

This document gives exact locators for the current standalone method. It uses
the Ukrainian Forest package as a worked reference. It does not make the
Ukrainian Forest values universal requirements.

The evidence status is `PROVEN-STATIC` unless a section gives a different
status. The values came from the version `0.3.6` package manifest, the current
framework and companion source, and the emitted bundles. A new build can have
different byte counts and SHA-256 values.

## Path tokens

Use these tokens in instructions, logs, and examples. Do not publish a real
user name or a machine-local workspace path.

| Token | Exact meaning |
| --- | --- |
| `<OPERATOR_INSTALL>` | The directory that contains `OPERATOR.exe` and the installed `BepInEx` directory. |
| `<AUTHOR_WORKSPACE>` | The map author's private work directory. |
| `<UNITY_PROJECT>` | The clean Unity authoring project root. This directory contains `Assets` and `ProjectSettings`. |
| `<PACKAGE_CATALOG>` | The directory that contains one or more package directories. |
| `<PACKAGE_ROOT>` | One package directory. This directory contains `operator-map-package.json`. |
| `<FRAMEWORK_SOURCE>` | The source checkout for OPERATOR: Modded Operations. |
| `<MAP_COMPANION_SOURCE>` | The source checkout for an exact-scene map companion. |
| `<RELEASE_STAGE>` | A clean directory that has the final install-relative tree. |
| `<USER_PROFILE>` | The operating-system user profile directory. Do not replace this token in public documentation. |

Use Windows separators for installed file-system paths. Use forward slashes
for JSON paths and Unity asset addresses.

Do not publish these data:

- a drive-specific private workspace;
- an operating-system account name;
- a private test log;
- an extracted OPERATOR asset;
- a game binary;
- a credential;
- a private test control.

## Installed file locations

The generic framework and a map companion are BepInEx plug-ins. The package is
data-only. Install the files at these exact relative locations:

```text
<OPERATOR_INSTALL>\BepInEx\plugins\OperatorModAPI\OperatorModAPI.dll
<OPERATOR_INSTALL>\BepInEx\plugins\OperatorModdedOperations\OperatorModdedOperations.dll
<OPERATOR_INSTALL>\BepInEx\plugins\<MAP_PLUGIN_ID>\<MAP_PLUGIN_ASSEMBLY>.dll
<OPERATOR_INSTALL>\BepInEx\OperatorMods\<PACKAGE_ID>\operator-map-package.json
```

Every path in `files[].path` is relative to `<PACKAGE_ROOT>`. The Ukrainian
Forest package therefore installs its scene bundle at this location:

```text
<OPERATOR_INSTALL>\BepInEx\OperatorMods\community.ukrainian-forest\content\operator_ukrainian_forest_scene
```

Do not put framework code in `<PACKAGE_ROOT>`. Do not put map coordinates in
`OperatorModdedOperations.dll`.

## Public source locators

Use this table to find the code that implements each public authoring rule.

| Contract | Source file | Exact type or member |
| --- | --- | --- |
| Build dependencies before the scene | [`templates/Editor/BuildStandaloneMapBundles.cs`](../templates/Editor/BuildStandaloneMapBundles.cs) | `BuildStandaloneMapBundles.Build` |
| Select the Windows target | [`templates/Editor/BuildStandaloneMapBundles.cs`](../templates/Editor/BuildStandaloneMapBundles.cs) | `RequireWindowsTarget` |
| Reject a missing Unity asset | [`templates/Editor/BuildStandaloneMapBundles.cs`](../templates/Editor/BuildStandaloneMapBundles.cs) | `RequireAsset` |
| Prove dependency and scene bundle roles | [`templates/Editor/BuildStandaloneMapBundles.cs`](../templates/Editor/BuildStandaloneMapBundles.cs) | `VerifyBundle` |
| Validate exact scene identity | [`templates/Editor/ValidateStandaloneMapScene.cs`](../templates/Editor/ValidateStandaloneMapScene.cs) | `ExpectedScenePath`, `ExpectedMapMarker`, `ExpectedSpawnSetMarker` |
| Select one manifest operation | [`templates/Editor/ValidateStandaloneMapScene.cs`](../templates/Editor/ValidateStandaloneMapScene.cs) | `ExpectedPackageManifestPath`, `ExpectedOperationId`, `LoadSelectedOperation` |
| Validate PVE marker capacity | [`templates/Editor/ValidateStandaloneMapScene.cs`](../templates/Editor/ValidateStandaloneMapScene.cs) | `IsEnemyMarker`, `PackageOperation.minEnemies` |
| Validate terrain collision | [`templates/Editor/ValidateStandaloneMapScene.cs`](../templates/Editor/ValidateStandaloneMapScene.cs) | `Terrain`, same-object `TerrainCollider`, shared `TerrainData` checks in `Validate` |
| Validate world-space wall clearance | [`templates/Editor/ValidateStandaloneMapScene.cs`](../templates/Editor/ValidateStandaloneMapScene.cs) | `ContainsWithClearance` |
| Validate package syntax | [`schemas/operator-map-package.schema.json`](../schemas/operator-map-package.schema.json) | JSON Schema draft 2020-12 root and `$defs` |

Set these values in `BuildStandaloneMapBundles` before a build:

```csharp
private const string OutputDirectory = "Build/StandaloneMap";
private const string DependencyBundleName = "<dependency-bundle-name>";
private const string SceneBundleName = "<scene-bundle-name>";
private const string ScenePath =
    "Assets/Maps/<MapName>/Scenes/<MapName>.unity";
private static readonly string[] ExplicitDependencyAssets =
{
    "Assets/Maps/<MapName>/RuntimePayload/<payload-file>",
};
```

The build code uses these exact Unity API calls:

1. `AssetDatabase.GetDependencies(ScenePath, true)` gets the dependency
   closure.
2. `BuildPipeline.BuildAssetBundles` emits both bundles for
   `BuildTarget.StandaloneWindows64`.
3. `AssetBundle.GetAllScenePaths` proves that the dependency bundle has zero
   scenes.
4. `AssetBundle.GetAllScenePaths` proves that the scene bundle has exactly
   `ScenePath`.

Set these values in `ValidateStandaloneMapScene` before validation:

```csharp
private const string ExpectedScenePath =
    "Assets/Maps/<MapName>/Scenes/<MapName>.unity";
private const string ExpectedMapMarker =
    "MAP_ID_<namespaced-map-id>";
private const string ExpectedSpawnSetMarker =
    "SPAWN_SET_<spawn-set>";
private const string ExpectedPackageManifestPath =
    "<path-from-unity-project-to-package-manifest>";
private const string ExpectedOperationId =
    "<namespaced-operation-id>";
```

The validator selects the map by the exact `scenePath`. It selects the
operation by the exact `operationId`. A PVE operation MUST have at least
`minEnemies` ordinary `PVE_EnemySpawn_` markers. A PVP operation CAN have zero
AI markers.

## Ukrainian Forest package identity

This section records one exact package. Use it to understand the relationship
between IDs, file names, and Unity asset addresses.

| Field | Version `0.3.6` value |
| --- | --- |
| `packageId` | `community.ukrainian-forest` |
| `displayName` | `Ukrainian Forest` |
| `mapId` | `community.ukrainian-forest.ukrainian-forest` |
| map `displayName` | `UKRAINE FOREST` |
| `sceneBundle` | `content/operator_ukrainian_forest_scene` |
| first `dependencyBundles[]` value | `content/operator_ukrainian_forest` |
| `scenePath` | `Assets/Maps/UkrainianForest/Scenes/UkrainianForest.unity` |
| `previewImage` | `media/ukraine_forest_preview.jpg` |
| LUT path | `lighting/AgX_Powerful_RGBAHalf_32.bytes` |
| LUT contract | dimension `32`, format `rgba-half`, `262144` bytes |
| LUT SHA-256 | `71352890a0560d680be154567e5e01cbd9b41fa0eb5997029ec7cedb3a42795f` |

The exact emitted package files had these manifest records:

| Package-relative path | Bytes | SHA-256 |
| --- | ---: | --- |
| `content/operator_ukrainian_forest` | `630286065` | `05436ac4b474a54877a0d3bdaafe029e6c35edc2dbd30898e6e094f2a2138b32` |
| `content/operator_ukrainian_forest_scene` | `17606767` | `6bdd50f61cbce1ca28f79f6c198db45d7fd752d96d2f5468c7b1dc5f5c1e94d1` |
| `lighting/AgX_Powerful_RGBAHalf_32.bytes` | `262144` | `71352890a0560d680be154567e5e01cbd9b41fa0eb5997029ec7cedb3a42795f` |
| `media/ukraine_forest_preview.jpg` | `6385660` | `9d18a3abfa93b8b0f17a721f20731930a618b971f0b1cbd3fb97da3305ff4255` |

Recalculate all byte counts and SHA-256 values after each build. Do not copy
these version `0.3.6` values into a different archive.

## Ukrainian Forest operation records

The scene supports two operations. The package uses one scene and two spawn
set markers.

| Contract | PVP value | PVE value |
| --- | --- | --- |
| `operationId` | `community.ukrainian-forest.pvp` | `community.ukrainian-forest.pve` |
| `mode` | `pvp` | `pve` |
| `spawnSet` | `forest-pvp` | `forest-pve` |
| `minPlayers` | `2` | `1` |
| `maxPlayers` | `16` | `8` |
| `minEnemies` | not permitted | `10` |
| `maxEnemies` | not permitted | `15` |
| infiltration `id` | `south-forest-edge` | `south-forest-edge` |
| `defaultTimeCode` | `1100` | `1100` |

The framework member `ChooseStandalonePveEnemyCount` selects a value from the
inclusive `minEnemies` to `maxEnemies` range. The map author MUST provide at
least `maxEnemies` valid ordinary enemy markers when one enemy uses one
marker. Ukrainian Forest provides `PVE_EnemySpawn_00` through
`PVE_EnemySpawn_31`.

## Ukrainian Forest scene objects

The exact map root is `UkrainianForestMap`. The exact terrain object is
`NATIVE_Ground_HillyTerrain`.

The scene contains these identity objects:

```text
MAP_ID_community.ukrainian-forest.ukrainian-forest
SPAWN_SET_forest-pvp
SPAWN_SET_forest-pve
```

The spawn families use these exact names:

```text
Team1_Spawn_00
Team2_Spawn_00
PVE_EnemySpawn_00 through PVE_EnemySpawn_31
PVE_HVTSpawn_00 through PVE_HVTSpawn_03
```

Additional team markers can use the prefixes that
`ValidateStandaloneMapScene.IsPlayerMarker` accepts. The exact accepted
prefixes are:

```text
PVE_PlayerSpawn_
Team1_Spawn_
Team1_Backup_Spawn_
Team2_Spawn_
Team2_Backup_Spawn_
PVP_Team1Spawn_
PVP_Team2Spawn_
```

`GameplayBounds` MUST have one `BoxCollider`. Place every AI marker inside the
box with the required wall clearance. If the `GameplayBounds` transform has a
scale, convert world-space clearance metres to local X and Z units. The public
validator does this conversion in `ContainsWithClearance`.

The standalone runtime performs containment before A* graph lookup. This
order prevents an outside marker from snapping to a valid node on the wrong
side of a combat wall.

## Ukrainian Forest terrain payload

The `runtimeTerrain` record reconstructs `TerrainData` at runtime. It uses the
dependency bundle `content/operator_ukrainian_forest`.

| Field | Exact value |
| --- | --- |
| `rootObject` | `NATIVE_Ground_HillyTerrain` |
| `heightPayload` | `assets/maps/ukrainianforest/terrain/runtimepayload/ukrainianforest_expandedheight_rg16.png` |
| `heightEncoding` | `rg16-unorm-be` |
| `surfaceWeightsPayload` | `assets/maps/ukrainianforest/terrain/runtimepayload/ukrainianforest_expandedsurfaceweights_rgb.png` |
| `surfaceWeightsEncoding` | `rgb8-unorm` |
| `heightmapResolution` | `1025` |
| `alphamapResolution` | `1024` |
| `baseMapResolution` | `2048` |
| `detailResolution` | `512` |
| `detailResolutionPerPatch` | `16` |
| origin | `x=-47.0`, `y=-2.0`, `z=-36.0` |
| size | width `94.0`, height `32.0`, length `168.0` |

The terrain object MUST have `Terrain` and `TerrainCollider` on the same
`GameObject`. Both components MUST reference the same reconstructed
`TerrainData`. A visible mesh does not satisfy this contract.

The three exact terrain layers are:

| Layer | Diffuse asset address | Normal asset address | Mask asset address | Tile X/Z | Normal scale | Metallic/smoothness |
| --- | --- | --- | --- | --- | ---: | --- |
| `MOD_UkrainianForest_Grass` | `assets/operatornativeassets/texture2d/floor_grass_basecolor.png` | `assets/operatornativeassets/texture2d/floor_grass_normal.png` | `assets/operatornativeassets/texture2d/grassgreen_qheqg2_maskmap.png` | `4.88` / `5.78` | `0.82` | `0.0` / `0.0` |
| `MOD_UkrainianForest_Dirt` | `assets/operatornativeassets/texture2d/dirt_0.png` | `assets/operatornativeassets/texture2d/floor_grass_normal.png` | `assets/operatornativeassets/texture2d/aset_rock_granite_m_rgasy_maskmap.png` | `5.95` / `4.74` | `0.58` | `0.0` / `0.0` |
| `MOD_UkrainianForest_Rock` | `assets/operatornativeassets/texture2d/floor_rock_gray_basecolor.png` | `assets/operatornativeassets/texture2d/floor_rock_gray_normal.png` | `assets/operatornativeassets/texture2d/aset_rock_granite_m_rgasy_maskmap.png` | `4.25` / `5.21` | `0.86` | `0.0` / `0.0` |

These strings are Unity AssetBundle asset addresses. They are not installed
file-system paths. Get the assets from a legal local OPERATOR installation.
Do not publish the extracted textures.

## Ukrainian Forest pine material reference

The full-crown reference meshes are `Pine_var10_LOD0` and
`Pine_var11_LOD0`. A successful asset load does not prove a full crown. Check
the player-camera silhouette at close, middle, and far distances.

The native material slot names include:

```text
Pine_Needle
pine_bark
Trunk_pine_var4
```

The verified texture identities include:

```text
Pine_leaves_4K
Pine_Leaves_Mask
Pine_Leaves_normal
Texture_Bark_pine_BaseColor1
Texture_Bark_pine_MaskMap
Texture_Bark_pine_Normal
Trunk_var_4_basecolor
Trunk_var_4_Mask
Trunk_var_4_normal
```

The installed shader search order uses these exact shader names:

```text
Shader Graphs/BotD_Graph_Lit
Shader Graphs/BotD_Graph_Lit_TranslucentAlphaCutoff
HDRP/Lit
```

Use `Shader Graphs/BotD_Graph_Lit_TranslucentAlphaCutoff` for a verified
native foliage surface when it is available in the installed build. Use
`HDRP/Lit` only as a measured fallback. Preserve alpha cutoff, double-sided
state, normal-map import type, mask-map channel packing, render queue, and
material slot order.

## Framework and companion source locators

The generic framework assembly is `OperatorModdedOperations.dll`. Its current
source file is `CerberusNativeTabFix.cs`. These exact members define the
standalone flow:

| Member | Responsibility |
| --- | --- |
| `BuildPackageInfiltrationMapPrefab` | Build the native selector presentation for a package operation. |
| `BeginCatalogOperationLaunch` | Capture the selected operation and exact player-owned laptop before asynchronous package I/O. |
| `RestoreCapturedLaunchLaptop` | Restore only the captured `playerNetworking` field when the same laptop released it during loading. |
| `SetNativeConfirmationLoadingState` | Keep the private modal visible and non-interactable until loading succeeds or fails. |
| `InfilSelectorDisplayer.SpawnMap` | Start the selected native map flow. |
| `CerebusOpboard.Start_Operation` | Start the selected operation through the native board. |
| `ValidateStandaloneSceneContract` | Reject a loaded scene that does not match its declared map and spawn set. |
| `CreateStandaloneGameplayBootstrap` | Create generic standalone gameplay state after the scene contract passes. |
| `ChooseStandalonePveEnemyCount` | Select the inclusive package PVE population range. |
| `TrySpawnStandalonePveEnemies` | Filter registered firearm-capable prefabs and call `RaidManager.ServerSpawnAI(false)` after world readiness. |
| `ReleaseStandaloneGameMode` | Release the mode owner during lifecycle transitions. |

The Ukrainian Forest companion assembly is `OperatorUkrainianForest.dll`. Its
current source file is `OperatorUkrainianForestPlugin.cs`. These exact members
own map-specific reconstruction:

| Member | Responsibility |
| --- | --- |
| `ProcessStandalonePackageScene` | Gate processing to the exact Ukrainian Forest package scene. |
| `EnsureStandaloneNavigationGraph` | Build or validate the map-owned playable A* graph. |
| `IsInsideForestPlayableBounds` | Reject markers outside the authoritative forest combat volume. |
| `LogStandaloneWorldContract` | Record terrain, collision, marker, and foliage contract evidence. |
| `OnSceneUnloaded` | Remove map-owned runtime state when the package scene unloads. |

These members are implementation evidence. A map author MUST use the public
schema and templates as the authoring interface. A different map companion
MUST use a different exact scene gate and MUST own only its map-specific
reconstruction.

## Exact load order

The loader MUST use this order:

1. Enumerate `<PACKAGE_CATALOG>` directories.
2. Read `<PACKAGE_ROOT>/operator-map-package.json` as data.
3. Validate the closed JSON schema.
4. Reject a duplicate `packageId`, `mapId`, or `operationId`.
5. Resolve each `files[].path` under `<PACKAGE_ROOT>`.
6. Reject an absolute path, a backslash, a drive prefix, or a traversal
   segment in a manifest path.
7. Verify each declared byte count and SHA-256 value.
8. Capture the exact player-owned `MissionLaptop` and its `PlayerNetworking`
   before asynchronous bundle I/O.
9. Keep the private Confirm modal in a disabled loading state.
10. Load `content/operator_ukrainian_forest` as the dependency bundle.
11. Prove that the dependency bundle has zero scene paths.
12. Load `content/operator_ukrainian_forest_scene` as the scene bundle.
13. Prove that the scene bundle has exactly
    `Assets/Maps/UkrainianForest/Scenes/UkrainianForest.unity`.
14. Restore only the captured laptop field when loading released it, then
    close the modal and call `CerebusOpboard.Start_Operation` in one frame.
15. Load that exact scene.
16. Run the exact-scene companion.
17. Reconstruct `TerrainData`, terrain materials, native tree materials, and
    the playable-only A* graph.
18. Prove the map marker, selected spawn set, terrain collision, combat
    bounds, and valid marker capacity.
19. Release the generic mode owner only after the world-ready contract passes.
20. Spawn the player and PVE actors through the shipped owner-aware server
    path.

Do not spawn actors before steps 16 through 18 finish. A brown flat terrain
with falling enemies is evidence that actor creation ran before the complete
world-ready contract or that runtime reconstruction did not finish.

## Exact authoring and verification actions

In Unity, run these menu commands from the public templates:

```text
Tools > OPERATOR > Validate Standalone Map Scene
Tools > OPERATOR > Build Standalone Map Bundles
```

Validate the package catalog, not one package leaf, when the validator expects
to enumerate package directories. Use this input shape:

```text
<PACKAGE_CATALOG>
`-- <PACKAGE_ID>
    |-- operator-map-package.json
    |-- content
    |   |-- <dependency-bundle-name>
    |   `-- <scene-bundle-name>
    |-- lighting
    `-- media
```

Calculate a staged file hash with this PowerShell command:

```powershell
Get-FileHash -Algorithm SHA256 -LiteralPath '<RELEASE_STAGE>\BepInEx\OperatorMods\<PACKAGE_ID>\content\<bundle-name>'
```

Calculate a deployed file hash with this PowerShell command:

```powershell
Get-FileHash -Algorithm SHA256 -LiteralPath '<OPERATOR_INSTALL>\BepInEx\OperatorMods\<PACKAGE_ID>\content\<bundle-name>'
```

The source, staged, archive, and deployed hashes MUST match for one candidate.
Record the candidate version and hash. Do not record a private absolute path.

## Required runtime evidence

A release candidate needs separate proof for each row:

| Gate | Required evidence |
| --- | --- |
| Selector | The mission row, preview, infiltration selector, and one physical first Confirm use the selected package operation without a second laptop interaction. |
| Scene | The loaded scene path is the exact manifest `scenePath`. |
| Terrain | `Terrain` and `TerrainCollider` exist on `NATIVE_Ground_HillyTerrain` and share one `TerrainData`. |
| Materials | Terrain and tree materials use the expected installed shader and texture identities. |
| Foliage | `Pine_var10_LOD0` and `Pine_var11_LOD0` have full crowns at close, middle, and far player-camera distances. |
| PVE population | Each clean launch creates between `10` and `15` ordinary enemies for the Ukrainian Forest PVE operation. |
| Bounds | All selected PVE markers and all spawned actors are inside the playable combat volume before graph lookup. |
| Combat | The player and enemies can damage each other across valid lines of fire. |
| Navigation | All selected AI markers ground to the playable-only A* graph. |
| PVP isolation | The Ukrainian Forest PVP operation creates zero AI actors. |
| Lifecycle | Normal restart, death/KIA restart, scene unload, and a second launch do not keep stale map state. |
| Multiplayer | Host and client load the same package identity and content hashes. |

Do not use an editor screenshot as proof for a runtime gate. Do not use one
passed gate as proof for a different gate.
