# 3a. AssetRipper reference to standalone scene bundles

Use AssetRipper output as a local evidence library. Build the release from a
separate clean Unity project.

## 1. Create a local reference export

1. Copy the required installed data to a separate work location.
2. Do not modify the live installation.
3. Load the copy in the current AssetRipper release.
4. Confirm the exact Unity version.
5. Export a Unity project for inspection.
6. Record the source scene and complete closure for each candidate asset.

Record these values:

- root and child hierarchy;
- local transforms and pivot intent;
- highest-detail mesh and all submeshes;
- material-slot order;
- serialized material identity and properties;
- base/alpha, normal, mask, and special texture bindings;
- texture size, mip, color-space, alpha, wrap, and compression roles;
- LOD group and billboard relationship;
- collider type and bounds;
- native component type and serialized field presence.

An exported MonoBehaviour type does not prove that its custom fields survived.
See the [`DoorV2` case](09-interactive-prefabs-and-doorsv2.md).

## 2. Import only the audited closure

| Asset | Keep |
| --- | --- |
| Model | Complete root or complete highest-detail mesh, vertex channels, submesh order, pivot evidence |
| Material | Raw record, original identity, proxy transport material, complete property table |
| Texture | Base/alpha, normal, mask, height, thickness, detail, correct color role and mips |
| Terrain | Height, surface weights, layers, size, origin, collision plan, optional lossless payloads |
| Interactive | Complete hierarchy and field evidence; do not treat a stripped shell as functional |

Keep Unity GUID relationships stable. Do not casually regenerate `.meta` files.

Do not publish extracted game assets unless you have permission. A public map
can use original assets or instruct the user to derive permitted local data.

## 3. Author a real scene

Create one `.unity` scene. Do not use one top-level prefab as the standalone
mission address.

Build in this order:

1. Terrain height and collision.
2. Gameplay walls and bullet-interaction boundary.
3. Player routes and spawn clearance.
4. Player, enemy, HVT, and other mission markers.
5. Visual terrain apron outside the gameplay wall.
6. Complete structures and cover.
7. Full-crown tree families and foliage.
8. Lighting and volumes.
9. Portable material transport records.
10. Metadata objects.

Keep runtime-only reconstruction out of the scene when Unity cannot serialize
it safely. Put the required payload in a dependency bundle and implement the
exact-scene reconstruction in the companion.

## 4. Separate dependency and scene bundles

Build portable assets and payloads as dependency bundles. Build the real scene
as the scene bundle.

Use this order:

```text
dependency bundle 00
-> dependency bundle 01
-> ...
-> scene bundle
```

The manifest lists dependencies in exact load order. A dependency bundle MUST
contain no scene. The scene bundle MUST expose the exact declared scene path.

Classify every final input before you build:

| Input | Destination | Reason |
| --- | --- | --- |
| Real `.unity` map scene | Scene bundle | `maps[].scenePath` must match its exact emitted scene address |
| Meshes and complete prefabs referenced by the scene | Dependency bundle | Load before the scene and preserve one shared asset instance |
| Portable proxy materials and their complete texture closure | Dependency bundle | The exact-scene companion resolves the raw native identity and rehydrates installed shaders |
| Runtime height and surface-weight payload textures | Dependency bundle | Modded Operations loads them from the Core-verified dependency path and creates native `TerrainData` before player spawn. |
| Map lighting records or serialized LUT assets | Dependency bundle | They are Unity assets consumed by the exact-scene presentation path |
| Raw briefing/infiltration preview JPEG or PNG | Package `media/` directory, outside bundles | The framework reads it with `File.ReadAllBytes` and `ImageConversion.LoadImage` |
| Raw external `rgba-half` LUT bytes | Package `lighting/` directory, outside bundles | Core validates its exact dimensions and the runtime builds a `Texture3D` |
| IDs, row text, SITREP, player/AI ranges, infiltrations, time codes | `operator-map-package.json` | These values feed the generic mission UI and mode contract; they are not Unity scene data |

Build all bundle definitions in one `BuildPipeline.BuildAssetBundles` call.
Explicitly list the reusable assets in the dependency definition and list only
the real `.unity` scene in the scene definition. Then inspect the emitted
addresses. Do not assume that an editor path, an AssetRipper export path, and
an emitted bundle address are identical.

For every payload that companion code loads by name, record:

```text
source Unity asset path
-> bundle name
-> exact lowercased GetAllAssetNames() address
-> consuming manifest field or code constant
-> expected type, dimensions, encoding, and SHA-256
```

For a release candidate, clear reusable builder caches and use
`BuildPipeline.BuildAssetBundles` with `BuildAssetBundleOptions.StrictMode |
BuildAssetBundleOptions.ForceRebuildAssetBundle` and
`BuildTarget.StandaloneWindows64`. A prior bundle with the same filename is
not evidence for the current source.

Example build definition:

```csharp
var builds = new[] {
    new AssetBundleBuild {
        assetBundleName = "example_map_assets",
        assetNames = dependencyAssetPaths,
    },
    new AssetBundleBuild {
        assetBundleName = "example_map_scene",
        assetNames = new[] {
            "Assets/Maps/Example/Scenes/Example.unity",
        },
    },
};

BuildPipeline.BuildAssetBundles(
    outputDirectory,
    builds,
    BuildAssetBundleOptions.StrictMode |
        BuildAssetBundleOptions.ForceRebuildAssetBundle,
    BuildTarget.StandaloneWindows64);
```

## 5. Validate emitted bundles

Before package staging, verify:

- dependency asset names and portable closure;
- no scene path in a dependency bundle;
- exactly the expected scene path in the scene bundle;
- Windows target and exact Unity generation;
- no null material slot;
- expected texture size, mip, color-space, and read/write role;
- complete LOD0 model and submesh counts;
- scene metadata and marker counts;
- terrain and collider objects;
- fallback light;
- no undeclared bundle file.

A proxy material can be valid transport even when its editor appearance is not
the final OPERATOR appearance. The runtime companion MUST replace it with a
fresh installed native material and then pass the active-renderer audit.

## 6. Build the package

1. Copy final dependency bundles to `content/` in one package staging root.
2. Copy the scene bundle to `content/`.
3. Copy the final raw mission preview to `media/`. Do not place it inside the
   Unity bundle.
4. Copy an optional raw external LUT to `lighting/`.
5. Write the manifest with exact package/map/operation IDs, display and SITREP
   text, operation modes, player and PVE population ranges, spawn sets,
   infiltration records, time codes, preview path, bundle paths, and exact
   scene address.
6. Confirm that each `mapPositionX/Y` marker position matches the final
   preview crop. These normalized values configure the 2D selector; they do
   not replace the scene's 3D spawn markers.
7. Compute file lengths and lowercase SHA-256 after all copies finish.
8. Sort the file table by ordinal relative path.
9. Run the strict package loader and directory-closure check.
10. Install only while OPERATOR is closed.

See [Standalone package format and loading](10-standalone-packages.md).
For the complete field-to-UI and bundle-content procedure, see
[Modded Operations mission presentation and bundle data](03b-modded-operations-presentation.md).

## 7. First runtime proof

Use the physical Modded Operations tab. Select the package row. Test Back,
Execute, Cancel, selector, Confirm, exact scene, world contract, player spawn,
PVE/PVP isolation, and restart.

Do not diagnose a brown exact scene as a selector failure. Check live proxy and
error shaders. Do not diagnose falling AI as a scene-load failure. Check the
resident graph and marker contract.
