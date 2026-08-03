# 0. Toolchain and work order

Install the exact Unity editor version that the target OPERATOR build uses.
Do not use a nearby editor version for a release build.

## Required tools

| Tool | Use | Source |
| --- | --- | --- |
| Unity Hub and exact Unity Editor | Author and build Windows dependency and scene AssetBundles | [Unity Hub](https://docs.unity.com/en-us/hub/install-hub) and [Unity archive](https://unity.com/releases/editor/archive) |
| AssetRipper | Create a local reference project for hierarchy, asset, material, and dependency inspection | [AssetRipper](https://github.com/AssetRipper/AssetRipper) |
| BepInEx IL2CPP | Load the exact-build Core, generic framework, and optional map companion | [BepInEx IL2CPP guide](https://docs.bepinex.dev/master/articles/user_guide/installation/unity_il2cpp.html) |
| .NET SDK | Build the exact-build BepInEx components against the local interop assemblies | [.NET downloads](https://dotnet.microsoft.com/download) |
| Unity AssetBundle tools | Build strict Windows bundles and inspect scene/asset addresses | [Unity AssetBundle guide](https://docs.unity.cn/Manual/AssetBundles-Building.html) |
| SHA-256 tool | Create and compare source, staged, archive, and deployed file hashes | `Get-FileHash` is included with PowerShell |
| ZIP reader | Inspect final archive entries without installing them | PowerShell and common archive tools are sufficient |

The current standalone runtime also needs compatible Core and
**OPERATOR: Modded Operations — Standalone Map Framework** files. A map that
needs runtime reconstruction also needs its exact-scene companion.

## Optional tools

| Tool | Valid use | Status |
| --- | --- | --- |
| [OPERATOR MapBridge](https://github.com/0xFlan/operator-mapbridge) | Explicit local prefab-overlay diagnostics | `RETIRED` for standalone mission parity |
| [Community OPERATOR Modding Toolkit](https://github.com/ArchdukePierre/operator-modding-toolkit) | Read-only API and architecture reference after current-build verification | Reference only |

## Work in this order

1. Fingerprint the installed game and runtime dependencies.
2. Create a separate AssetRipper reference export.
3. Create a clean exact-version Unity authoring project.
4. Build and validate one minimal real scene bundle.
5. Create and validate one strict data-only package.
6. Verify mission row, selector, exact scene, and player spawn with simple
   materials.
7. Add complete terrain, models, textures, materials, foliage, and collision.
8. Add a map companion only for installed-runtime reconstruction that scene
   data cannot preserve.
9. Add and validate the playable-only A* graph and all mission markers.
10. Add and validate native interactive objects.
11. Run PVE, PVP, restart, teardown, and player-camera gates.
12. Build final archives from a clean staging directory.

## Templates in this repository

- `templates/Editor/BuildStandaloneMapBundles.cs` builds dependencies before
  the real scene bundle.
- `templates/Editor/ValidateStandaloneMapScene.cs` validates scene identity,
  terrain, collision, walls, and marker structure.
- `templates/Editor/BuildLocalMapBundle.cs` is a legacy prefab-overlay build
  template. Do not use it for a standalone mission.
- `templates/Editor/ValidateMapRoot.cs` is a legacy prefab-root validator.
- `tools/Deploy-LocalMapCandidate.ps1` is a legacy MapBridge deployment
  template.
- `docs/13-exact-implementation-reference.md` gives exact source members,
  assembly names, bundle names, scene objects, Ukrainian Forest asset
  addresses, and privacy-safe path tokens.

Set all project-specific paths and IDs before use. In the standalone scene
validator, set the package-manifest path and the exact operation ID. The
validator reads the selected operation mode and `minEnemies` from that
manifest. A PVP operation can have zero AI markers. A PVE operation must have
at least its declared number of ordinary enemy markers. Treat a template as
source, not as a prebuilt release.
