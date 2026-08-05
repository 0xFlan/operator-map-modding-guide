# Exact implementation reference

This document gives exact locators for the current standalone method. It uses
the Ukrainian Forest package as a worked reference. It does not make the
Ukrainian Forest values universal requirements.

The evidence status is `PROVEN-STATIC` unless a section gives a different
status. The identities came from the version `0.3.17` package manifest and the
current framework and companion source. The listed bundles predate the
current `0.4.15` PVP-Woods lighting authoring update. Rebuild them before a
release. A new build can have different byte counts and SHA-256 values.

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
| Validate the developer `DoorV2` graph | [`templates/Editor/ValidateDoorV2Prefab.cs`](../templates/Editor/ValidateDoorV2Prefab.cs) | `Validate`, `ValidateGraph`, `ValidateHandle` |
| Pin the official door source identity | [`templates/Editor/ValidateDoorV2Prefab.cs`](../templates/Editor/ValidateDoorV2Prefab.cs) | `PrefabAssetPath`, `OfficialPrefabGuid`, `RequireOfficialSourceGuid` |
| Reject null, external, shared, or nonreciprocal door references | [`templates/Editor/ValidateDoorV2Prefab.cs`](../templates/Editor/ValidateDoorV2Prefab.cs) | `RequireObjectReference`, `RequireNamedComponentReference`, `ValidateHandle` |
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

| Field | Version `0.3.17` value |
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
| `content/operator_ukrainian_forest` | `630271199` | `09679986bec2abd40a4fe45d2d4559e645a9c30183d6d2926d0709af18475138` |
| `content/operator_ukrainian_forest_scene` | `17598605` | `eda200913a03f478c08d70c75d0cadd91d30bfac30d37525ca8e1f5efc40a6fe` |
| `lighting/AgX_Powerful_RGBAHalf_32.bytes` | `262144` | `71352890a0560d680be154567e5e01cbd9b41fa0eb5997029ec7cedb3a42795f` |
| `media/ukraine_forest_preview.jpg` | `6385660` | `9d18a3abfa93b8b0f17a721f20731930a618b971f0b1cbd3fb97da3305ff4255` |

Recalculate all byte counts and SHA-256 values after each build. Do not copy
these version `0.3.17` values into a different archive. The two bundle values
above are the last emitted pre-`0.4.15` payload identities. They are not final
release identities for the updated builder.

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
| infiltration `id` | `north-forest-edge` | `north-forest-edge` |
| `defaultTimeCode` | `1100` | `1100` |

The framework member `ChooseStandalonePveEnemyCount` selects a value from the
inclusive `minEnemies` to `maxEnemies` range. The map author MUST provide at
least `maxEnemies` valid ordinary enemy markers when one enemy uses one
marker. Ukrainian Forest provides `PVE_EnemySpawn_00` through
`PVE_EnemySpawn_31`.

## Ukrainian Forest scene objects

The source prefab root is `UkrainianForestMap`. The standalone scene renames
its instantiated root to `MOD_UkrainianForest_Runtime`. The exact terrain
child is `NATIVE_Ground_HillyTerrain`.

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

The current scene contains ten Team 1 markers and ten Team 2 markers. Team 1
uses the normal PVE-player side. Its authored local Z range is `5.4` through
`13.2`. Team 2 uses the PVE-enemy side. Its authored local Z range is `82.4`
through `90.2`. The scene builder enforces ten markers per side with
`TeamSpawnCountPerSide`. It also enforces `Z <= 15` for Team 1 and `Z >= 80`
for Team 2 in `VerifyBuiltStandaloneSceneContract`.

The source locators are
`BuildHillyUkrainianForestBundle.cs:184-186`, `:415-417`, and
`:1535-1558`. The independent layout gate is
`tools/validate_hilly_forest_layout.py`. The source line numbers identify the
current private worked example. Recalculate them after a source rewrite.

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

## Exact Ukrainian Forest tree-grounding contract

The bundle authoring source uses these exact members:

| Member or constant | Current value or action |
| --- | --- |
| `CompleteTreeTrunkGroundDatumName` | `NATIVE_TRUNK_GROUND_DATUM_ONE_SIXTH` |
| `CompleteTreeBuriedTrunkFraction` | `1f / 6f` |
| `CompleteTreeMinimumAboveGroundFraction` | `0.75f` |
| `BroadCrownOakAdditionalTerrainEmbed` | `0.25f` |
| `BroadCrownOakMinimumRootContactRadius` | `0.60f` |
| `BroadCrownOakMaximumRootContactRadius` | `2.00f` |
| `TryGetCompleteTreeTrunkBounds` | Selects the first `LODGroup` LOD, matches `pine_bark`, `trunk_pine`, `bark_mat`, or `bark_2_mat`, reads each matching mesh submesh index, transforms referenced vertices to world space, and returns finite trunk minimum/maximum Y. |
| `FindCompleteTreeSurfaceContact` | Uses center contact for pines. For broad oaks, selects low LOD0 root-band points and two 16-direction rings, clamps the footprint to `0.60..2.00 m`, and returns its lowest sampled terrain contact. |
| `AlignCompleteTreeRootContactToSurface` | Creates the renderer-free family-aware datum, stores the selected contact X/Z, moves it to sampled terrain, validates exact contact within `0.001 m`, and uses the complete renderer maximum as the separate full-tree gate. |
| maximum authoring correction | `12 m`; a larger correction fails the build |

The controlling calculation in
`Assets/Editor/BuildHillyUkrainianForestBundle.cs` is:

```csharp
if (!TryGetCompleteTreeTrunkBounds(
        instance,
        out var trunkMinimumY,
        out var trunkMaximumY))
{
    throw new InvalidOperationException(
        "Complete tree has no readable LOD0 bark/trunk submesh bounds: " +
        instance.name);
}

var trunkHeight = trunkMaximumY - trunkMinimumY;
TryGetTreeGroundingReferenceSpan(
    instance, trunkHeight, out var referenceSpan, out var referenceRule);
var surfaceContact = FindCompleteTreeSurfaceContact(
    instance, centerSurfaceY, sampleSurfaceY);
var datumRoot = new GameObject(
    "NATIVE_TRUNK_GROUND_DATUM_ONE_SIXTH");
datumRoot.transform.SetParent(instance.transform, true);
var oakAdditionalEmbed = IsBroadCrownOak(instance) ? 0.25f : 0f;
datumRoot.transform.position = new Vector3(
    surfaceContact.x,
    trunkMinimumY + referenceSpan * (1f / 6f) + oakAdditionalEmbed,
    surfaceContact.z);

var correction = surfaceContact.y - datumRoot.transform.position.y;
instance.transform.position += Vector3.up * correction;
```

`TryGetCompleteTreeTrunkBounds` gets `mesh.GetIndices(slot)`. It rejects an
index outside `mesh.vertices`. It transforms each accepted vertex with
`renderer.transform.TransformPoint(vertices[index])`. It then updates only
the trunk minimum and maximum Y. The leaf material slot does not enter this
calculation.

The builder calls `AlignCompleteTreeRootContactToSurface` for each
`NATIVE_TREE_*` root with `HeightAt(x,z)` and for each
`NATIVE_PERIMETER_TREE_*` root with `HeightAtExterior(x,z)`.

The current complete tree sources are:

| Family | Exact prefab asset | Trunk-submesh evidence |
| --- | --- | --- |
| pine 10 | `Assets/OperatorNativeAssets/UkrainianPineCandidate/Prefabs/Pine_var10_LOD0.prefab` | root `CapsuleCollider`; combined renderer slots are `Pine_Needle`, `pine_bark`, and `Trunk_pine_var4` |
| pine 11 | `Assets/OperatorNativeAssets/UkrainianPineCandidate/Prefabs/Pine_var11_LOD0.prefab` | native lower-trunk collider; same three combined-renderer material slots |
| forest oak | `Assets/OperatorNativeAssets/GameObject/Oak_White_Desktop_Forest.prefab` | native lower-trunk collider children; combined LOD0 renderer uses `Bark_Mat`, `Bark_2_Mat`, and `Oak_White_Desktop_Forest_Mat` |
| field oak 4 | `Assets/OperatorNativeAssets/GameObject/Oak_White_Desktop_Field_4.prefab` | native lower-trunk collider children; combined LOD0 renderer uses `Bark_Mat`, `Bark_2_Mat`, and `Oak_White_Desktop_Field_4_Mat` |
| field oak 5 | `Assets/OperatorNativeAssets/GameObject/Oak_White_Desktop_Field_5.prefab` | native lower-trunk collider children; combined LOD0 renderer uses `Bark_Mat`, `Bark_2_Mat`, and `Oak_White_Desktop_Field_5_Mat` |

Do not use the bottom of the native trunk capsule or the combined-renderer
minimum as the trunk datum. A collider can extend below the model. A low leaf
card or branch can be below the root system. Keep collision shapes for
gameplay, and use only the selected bark/trunk submesh vertices for placement.

The map companion uses `AlignStandaloneAuthoredTreesToTerrain` after the
reconstructed `TerrainData` is bound. It samples `Terrain.SampleHeight` at the
packaged datum position, moves each root by `groundY - marker.position.y`,
enforces the `0.75` complete-tree
above-ground fraction and `12 m` limit, calls `Physics.SyncTransforms`, and
logs aligned, missing-datum, rejected, minimum-fraction, and
largest-correction values. Runtime repair processes only the 96
collision-enabled `NATIVE_TREE_*` roots. The deterministic Unity build owns
the 180 render-only `NATIVE_PERIMETER_TREE_*` roots because their colliders are
disabled after authoring. Runtime child traversal uses
`foliageRoot.GetChild(childIndex)`. Do not use `foreach (Transform child in
parent)` in an IL2CPP repeat-launch path; a later additive-scene generation
can expose an `Il2CppSystem.Object` enumerator item and throw an invalid cast.

## Exact 11:00 presentation source

The accepted comparable source is `sharedassets11.assets`, `PVP Woods
Warehouse`:

| Object | Path ID | Exact value |
| --- | ---: | --- |
| `Nice Sun` GameObject | `5150` | Static directional-light owner |
| Transform | `14228` | quaternion `(0.115319036,0.019461127,0.118037127,0.986098409)` |
| Light | `41512` | 30,000 lux, 5,500 K, bounce intensity 5 |
| `Global Volume Profile 1` | `198` | Complete wooded-map post stack |

The Volume component paths and active values are:

| Component | Path ID | Exact applied values |
| --- | ---: | --- |
| Exposure | `190` | AutomaticHistogram, CenterWeighted, fixed 10, compensation 0, limits 8.5 to 11, adaptation 0.5/0.5 |
| Bloom | `196` | quality 3, threshold 0.9, intensity 0.03, scatter 0.893 |
| ScreenSpaceLensFlare | `186` | intensity 0.5, streak 1.55, length 0.022, orientation 0, chromatic aberration 0.6 |
| Tonemapping | `191` | External, full ACES false, exact AgX Powerful LUT |
| ColorAdjustments | `187` | post exposure -0.3, contrast 30, hue 0, saturation -15 |
| WhiteBalance | `197` | temperature -3.6, tint -8.6 |
| LiftGammaGain | `192` | lift W .00827304, gamma W -.09100296, gain W .09100296 |
| HDShadow | `202` | maximum distance 125 |

The rejected donor used 52,241.375 lux, 6,727 K, bloom 0.359, and lens-flare
intensity 1. It produced the blinding-sun report. Copy the comparable wooded
map's complete light and Volume contract. Do not adjust only lux.

## Exact 02:00 presentation source

The installed source is `sharedassets7.assets`, `VolumeProfile` path ID `435`,
name `PVP map NIight VOLUME`. The exact applied component records are:

| Component | Path ID | Exact applied values |
| --- | ---: | --- |
| `Exposure` | `440` | Automatic Histogram; metering `4`; fixed `8.32`; compensation `1.16`; min `5.065281867980957`; max `9.348570823669434`; speeds `3/3` |
| `Tonemapping` | `432` | ACES; full ACES; no external LUT |
| `Bloom` | `431` | quality `1`; threshold `0.9`; intensity `0.3`; scatter `0.2` |
| `ColorAdjustments` | `433` | post exposure not overridden; contrast `17.3`; saturation `22` |
| `IndirectLightingController` | `443` | diffuse `1`; reflection `1`; probe `1` |

`GameManager.SetNVGColor`, exact-build RVA `0x00EEE5A0`, maps input `0` to
`WhitePhosper` and input `1` to `GreenPhosper`. The framework captures the
prior value, calls `SetNVGColor(0)` for 02:00, and restores the prior value on
unload.

Do not apply the day `AgX - Powerful` external LUT or the PVP-Woods day
exposure limits `8.5..11` to this night source.

The package selects `native-outdoor-v1`. Modded Operations `0.3.18` is the
single process-global render owner. `ApplyStandaloneRenderContract` applies
the day or night values, loads the package-verified LUT for day only, selects
white phosphor for 02:00, logs every selected control, and restores its prior
state during reverse teardown. Forest `0.4.15` owns scene reconstruction and
the LUT payload. It does not install a competing global Volume.

The exact accepted framework DLL is 151,552 bytes with SHA-256
`71F21527FF959DBCF3C7AD1894937F56A9D931E0BF1A6B038C857249861A745C`.
The mod repository publishes both its authored source and an ILSpy
`10.1.1.8388` snapshot under `decompiled/release-0.3.18`. The archived
`0.3.17` snapshot contains the rejected 52,241.375-lux day value and is not a
current instruction source.

## Framework and companion source locators

The generic framework assembly is `OperatorModdedOperations.dll`. Its current
source file is `CerberusNativeTabFix.cs`. These exact members define the
standalone flow:

| Member | Responsibility |
| --- | --- |
| `BuildPackageInfiltrationMapPrefab` | Build the native selector presentation for a package operation. |
| `BeginSelectedMapPrefetch` | Start one bounded selected-map dependency and scene-bundle load after row selection. |
| `BeginCatalogOperationLaunch` | Capture the selected operation and exact player-owned laptop before asynchronous package I/O. |
| `ProcessPendingLaunch` | Load one verified bundle request at a time, reject role/scene violations, and continue a waiting Confirm automatically. |
| `RestoreCapturedLaunchLaptop` | Restore only the captured `playerNetworking` field when the same laptop released it during loading. |
| `SetNativeConfirmationLoadingState` | Keep the private modal visible and non-interactable until loading succeeds or fails. |
| `LoadVerifiedMapDependencyAsset<T>` | Return one borrowed asset from the exact map's verified retained dependency bundles; keep unload ownership in the framework. |
| `InfilSelectorDisplayer.SpawnMap` | Start the selected native map flow. |
| `CerebusOpboard.Start_Operation` | Start the selected operation through the native board. |
| `ValidateStandaloneSceneContract` | Reject a loaded scene that does not match its declared map and spawn set. |
| `TryPrepareRuntimeTerrain` | Decode manifest-declared height and weights, create terrain layers, and bind one operation-owned `TerrainData` to rendering and collision. |
| `ValidateWalkableGroundContract` | Require every compatible player marker to raycast to package collision and require shared runtime-terrain identity. |
| `CreateStandaloneGameplayBootstrap` | Create generic standalone gameplay state after the scene contract passes. |
| `EnsureStandaloneBootstrapPrefabRegistered` | Register the inactive runtime PVE or PVP game-mode template on every peer with deterministic Mirror asset ID `0x4D4F5001` or `0x4D4F5002`; fail on an existing different prefab with the same ID. |
| `TryAdoptNetworkSpawnedGameMode` | On a remote peer, validate the spawned asset ID and mode, then adopt the Mirror-created clone as the active operation game-mode owner. |
| `ConfigureStandalonePlayerSpawnContract` | Capture prior process-global spawn state, convert only verified current-scene markers to `SpawnPoint`, install the operation-owned list/array, and set the first native index to `0`. |
| `ConfigureStandalonePvpController` | Create separate one-based Team 1 and Team 2 lists, seed exact retail scalar defaults, and assign the shipped `PvpGameode` owner. |
| `ConfigureStandalonePvpPresentation` | Supply the native PVP audio sources, 16 clip arrays, timer and score text, `TeleType`, result roots, animators, fade strings, and win/lose/tie text. |
| `RestoreStandalonePlayerSpawnContract` | Restore the prior spawn list, fallback array, and index only when the current globals still equal this operation's owned objects; reject prior objects from unloaded scenes. |
| `RequestStandalonePlayerSpawn` | On attempt 1, call native `PlayerMaster.SpawnPlayer()` for the owned player so its command and `ClientSpawnBS` route runs. If a repeat additive-scene host still has no new `PlayerSpawnedObject` 300 frames later, call the exact generated `UserCode_CMDSpawnPlayer__NetworkIdentity` server body as the bounded `owned-host-generated-server-recovery` route. Use the same server body directly for an unowned host-side player. |
| `SpawnAndPositionStandalonePlayers` | Record frame and attempt count before native entry, limit an owned host to two attempts and all other routes to three, stop after a player object or alive state proves success, then use shipped `GameManager.MovePlayerToSpawn` for the owned object. |
| `SelectPlayerMarker` | Select a current-scene marker for one `PlayerMaster`; reject PVP players without native team ID `1` or `2`. |
| `PvpMarkerMatchesTeam` | Bind Team 1 prefixes to native ID `1` and Team 2 prefixes to native ID `2`. |
| `ChooseStandalonePveEnemyCount` | Select the inclusive package PVE population range. |
| `TrySpawnStandalonePveEnemies` | Filter registered firearm-capable prefabs and call `RaidManager.ServerSpawnAI(false)` after world readiness. |
| `ReleaseStandaloneGameMode` | Release the mode owner during lifecycle transitions. |
| `ReleaseStandaloneRenderContract` | Restore the captured NVG color and destroy operation-owned run-time Volume profiles. |

The Ukrainian Forest companion assembly is `OperatorUkrainianForest.dll`. Its
current source file is `OperatorUkrainianForestPlugin.cs`. These exact members
own map-specific reconstruction:

| Member | Responsibility |
| --- | --- |
| `ProcessStandalonePackageScene` | Gate processing to the exact Ukrainian Forest package scene. |
| `ResetStandalonePlayerHandoffState` | Clear the old controller/transform hold, spawn-safety frames, diagnostics, local-move flag, redirects, pre-map support, applied flag, and destination scene at standalone load/unload boundaries. |
| `HasReadyStandaloneTerrain` | Require the exact terrain root to contain one `Terrain` and one `TerrainCollider` with the same non-null `TerrainData` before the companion publishes the scene to shared spawn hooks. |
| `GroundLateNetworkPlayerObjectInstance` | Resolve the current standalone root for a late owned player callback. If the exact terrain-ready gate does not pass, return without moving the player. Do not use the legacy Office pre-map fallback in a standalone package scene. |
| `GroundAirbornePlayerControllers` | During the bounded initial handoff, repair a known local root at the old sky pose or more than `2 m` below the sampled live surface. |
| `EnsureStandaloneNavigationGraph` | Build or validate the map-owned playable A* graph. |
| `AlignStandaloneAuthoredTreesToTerrain` | Align the 96 playable trees by packaged `NATIVE_TRUNK_GROUND_DATUM_ONE_SIXTH` after run-time TerrainData bind; sample its stored contact X/Z, keep native trunk collision, enforce the family-aware reference, 75-percent complete-rendered-height gate, 12 m correction limit, and typed child-index traversal. |
| `LoadVerifiedRawPineMaterialAsset` | Rejected `0.4.12` experiment retained for forensic comparison. The active `0.4.15` pine path does not use it. |
| `IsInsideForestPlayableBounds` | Reject markers outside the authoritative forest combat volume. |
| `LogStandaloneWorldContract` | Record terrain, collision, marker, and foliage contract evidence. |
| `OnSceneUnloaded` | Remove map-owned runtime state when the package scene unloads. |

These members are implementation evidence. A map author MUST use the public
schema and templates as the authoring interface. A different map companion
MUST use a different exact scene gate and MUST own only its map-specific
reconstruction.

## Current-build native PVP team contract

The current installed native build has these exact static relationships:

| Native item | Exact locator | Proven behavior |
| --- | --- | --- |
| `GameManager.nextSpawnPosition(bool,int,int)` | RVA `0x00EF2CA0`; direct team comparison near RVA `0x00EF2F15` | Reads the current `Pnext` index before incrementing it; compares the input team ID directly with `SpawnPoint.Team` at native field offset `0x24`. The first index must be `0`, not `-1`. |
| `TeamIdentifier.TeamID` | managed field offset `0x70` | Supplies the player's numeric team identity. |
| `PlayerMaster.MyTeamIdentifier` | managed field offset `0x88` | Supplies the player's `TeamIdentifier`. |
| `PvpGameode.StartNewRound()` | RVA `0x00F1C0E0`; Team 1 writes near `0x00F1C336`; Team 2 writes near `0x00F1C3F6` | Writes `CanSpawnPlayer=true`, then writes Team 1 as `1` and Team 2 as `2`. |

`ConfigureStandalonePlayerSpawnContract` applies the `1/2` assignment.
`SelectPlayerMarker` reads `TeamID`, invalidates a cached marker after a team
mismatch, and filters markers by team.

These addresses and offsets are exact-build `PROVEN-STATIC` evidence. They
are not portable API promises. Reinspect them after an OPERATOR update.

The current standalone bootstrap creates
`StandalonePvpGameMode : PvpGameode`. It assigns two non-empty native spawn
lists, uses `MaxRounds=13`, `RoundsToWin=7`, and `RoundTime=120` as the
serialized seeds, and calls the shipped `OnStartClient` and
`Server_AllPlayersLoaded` bodies. The shipped PVP body remains the owner of
respawn, freeze time, deaths, score SyncVars, rounds, and the match end.

The current runtime also creates every presentation reference that the native
score and music methods read. It uses one non-null silent clip with the exact
retail array lengths because base-game audio is not redistributed.

This is still `PROVEN-STATIC` until a two-peer first-spawn, freeze, death,
score, round-respawn, restart, and return-to-armory test passes. See
[Native mode ownership, PVE, and StandardPVP](03c-native-mode-ownership-and-pvp.md).

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
8. After stable row selection, start one selected-map bundle prefetch. Do not
   prefetch all maps.
9. Load `content/operator_ukrainian_forest` as the dependency bundle.
10. Prove that the dependency bundle has zero scene paths.
11. Load `content/operator_ukrainian_forest_scene` as the scene bundle.
12. At Confirm, capture the exact player-owned `MissionLaptop` and its
    `PlayerNetworking`. Attach them to the same in-flight request or use the
    completed same-content cache.
13. Keep the private Confirm modal in a disabled loading state when bundle
    work remains.
14. Prove that the scene bundle has exactly
    `Assets/Maps/UkrainianForest/Scenes/UkrainianForest.unity`.
15. Restore only the captured laptop field when loading released it, then
    close the modal and call `CerebusOpboard.Start_Operation` in one frame.
16. Load that exact scene.
17. Run the exact-scene companion.
18. Reconstruct `TerrainData`, disable
    `NATIVE_Ground_HillyTerrain_RenderFallback` after successful render and
    collision bind, reconstruct terrain/tree materials, align complete-tree
    visible bases, and build the playable-only A* graph.
19. Prove the map marker, selected spawn set, terrain collision, combat
    bounds, and valid marker capacity.
20. Create the generic mode owner only after the world-ready contract passes.
21. Capture and install the current-scene native player-spawn globals with
    first index `0`.
22. Spawn the player and PVE actors through the shipped owner-aware path.

Do not spawn actors before steps 17 through 19 finish. A brown flat terrain
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
| PVP teams | A host and client on different teams spawn and respawn on their authored opposite sides; Team 1 uses native ID `1` and Team 2 uses native ID `2`. |
| Lifecycle | Normal restart, death/KIA restart, scene unload, and a second launch do not keep stale map state. |
| Multiplayer | Host and client load the same package identity and content hashes. |

Do not use an editor screenshot as proof for a runtime gate. Do not use one
passed gate as proof for a different gate.
