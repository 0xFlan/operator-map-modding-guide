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

Use `BuildPipeline.BuildAssetBundles` with `BuildAssetBundleOptions.StrictMode`
and `BuildTarget.StandaloneWindows64`.

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
    BuildAssetBundleOptions.StrictMode,
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

1. Copy final bundles and preview into one package staging root.
2. Write the manifest with exact IDs, operations, spawn sets, and scene path.
3. Compute file lengths and SHA-256 after all copies finish.
4. Sort the file table by ordinal relative path.
5. Run the strict package loader.
6. Install only while OPERATOR is closed.

See [Standalone package format and loading](10-standalone-packages.md).

## 7. First runtime proof

Use the physical Modded Operations tab. Select the package row. Test Back,
Execute, Cancel, selector, Confirm, exact scene, world contract, player spawn,
PVE/PVP isolation, and restart.

Do not diagnose a brown exact scene as a selector failure. Check live proxy and
error shaders. Do not diagnose falling AI as a scene-load failure. Check the
resident graph and marker contract.
