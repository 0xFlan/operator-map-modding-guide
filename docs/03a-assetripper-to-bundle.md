# 3a. AssetRipper to a loadable map bundle

This is the practical authoring path. Use a local reference export for
inspection and a separate, clean Unity project for the editable map.

## 1. Create a local reference export

1. Copy the relevant installed game data to a separate working location. Do
   not modify the live installation.
2. Use the current AssetRipper release to load the copied game data. Let it
   detect the Unity version; if detection is uncertain, establish the exact
   version from the game's data/logs before continuing. Do not force a nearby
   editor version.
3. Export a **Unity project** for inspection. A project export preserves
   hierarchy, mesh, material, texture, and pointer evidence that a screenshot,
   GLB, or loose PNG export loses.
4. Treat the export as a reference library. Record the source scene, asset
   names, mesh bounds/topology, source material record, texture bindings, LOD
   structure, and collider intent for every asset you intend to use.
5. Keep the reference export separate from the clean map project. It is the
   inspection library; the map project is the repeatable build workspace.

AssetRipper supports a broad range of Unity versions, but output quality and
shader reconstruction vary by game/version. Use the game's actual Unity
release and verify the result in the running game, not only in the exported
editor project. See the official project for current compatibility:
<https://github.com/AssetRipper/AssetRipper>.

## 2. Make a clean Unity authoring project

Use the exact Unity editor version used by the target game whenever possible.
Create a new, clean project for the map; do not build a release bundle directly
from a full AssetRipper export.

Copy/import only the audited closure required by the map:

| Asset type | Keep | Why |
| --- | --- | --- |
| Geometry | Complete prefab or complete highest-detail mesh, authored vertex channels, collider evidence | prevents branch/billboard/LOD substitution |
| Material | Raw serialized record plus portable proxy/identity data | editor shader may be unavailable but the runtime contract still matters |
| Textures | Base/alpha, normal, mask, thickness/height/detail maps, original color-space role, mip chain | prevents grey/flat/opaque runtime materials |
| Terrain | Height/splat/terrain-layer data, collision plan, optional lossless runtime payloads | makes ground real rather than visual only |
| Scene data | Your own prefab layout, markers, bounds, build script, validator | makes the map reproducible |

Do not rename files or regenerate `.meta` files casually: Unity GUID links are
part of the dependency graph. If a source material shows an error shader in
the authoring project, preserve it as serialized evidence; do not use it as a
runtime material template by blindly copying its properties.

## 3. Build the prefab before dressing it

Make one top-level `GameObject` prefab that represents the injected map. Give
it explicit child roots, for example:

    MapRoot/
      GroundCollision
      GroundVisualFallback
      DirectProps
      PerimeterVisuals
      SpawnMarkers
      Bounds

The names are a convention, not a game requirement. The contract is that the
prefab has a stable path, its collision-bearing ground is present before spawn
transfer, and its optional source-scene suppression is limited to separately
audited roots.

Build and test in this order:

1. `Terrain`/collider dimensions, slopes, raycasts, and out-of-bounds walls.
2. Spawn locations and route/camera clearance.
3. Exterior terrain/vegetation buffer that hides the edge behind collision
   bounds.
4. Complete LOD0 trees, bushes, grass, rocks, boulders, and cover grounded to
   the final height function.
5. Matching HDRP light/Volume/material repair path.

If Unity terrain data cannot bind in the target IL2CPP runtime, bundle lossless
readable height and surface-weight payloads and reconstruct a native
`TerrainData`/`TerrainCollider` before handing spawns to the game. Abort the
replacement if collision binding fails.

## 4. Emit a Windows AssetBundle

Use the same Unity generation as the game and build for
`StandaloneWindows64`. Mark the map prefab and every dependency that Unity
cannot discover automatically as bundle inputs. Missing-shader imports often
make Unity omit foliage maps unless you include the audited material texture
closure explicitly.

A minimal editor build script has this shape:

```csharp
var build = new AssetBundleBuild {
    assetBundleName = "my_map_bundle",
    assetNames = new[] {
        "Assets/Maps/MyMap/MyMapRoot.prefab",
        // Explicit material/texture closure paths when needed.
    },
};
BuildPipeline.BuildAssetBundles(
    outputDirectory,
    new[] { build },
    BuildAssetBundleOptions.StrictMode,
    BuildTarget.StandaloneWindows64);
```

Use a deterministic lower-case prefab asset path in the injector config, for
example `assets/maps/mymap/mymaproot.prefab`. Do not rely on a visible prefab
name or "first asset in bundle" behavior.

Before deployment, load the bundle in Unity and inspect:

- `GetAllAssetNames()` contains the expected prefab path;
- the prefab has its intended child roots, colliders, and marker count;
- all direct renderers have a material slot;
- expected textures are present with intended dimensions/mips/color spaces;
- complete boulders/cover pass multi-angle and footprint tests;
- the emitted file targets Windows, not the editor/default platform.

## 5. Hand the bundle to the injector

Copy the bundle to a local path, then configure the generic toolkit with the
exact scene identifier and the exact prefab asset path. Start in overlay mode.
The injector can instantiate a bundle; it cannot know whether your source
map's game-mode spawn owner, terrain data, material family, light owner, or
visual roots are compatible. Follow docs/04-runtime-integration.md and
docs/05-spawn-and-gameplay.md before treating the first load as a map
replacement.
