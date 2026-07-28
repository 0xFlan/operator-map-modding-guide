# 0. Toolchain and working order

Install the exact Unity editor version used by the target game before building
anything. The links below are the complete manual toolchain for this guide.

## Required

| Tool | Use | Link |
| --- | --- | --- |
| Unity Hub and the exact matching Unity Editor | Open the authoring project and build a Windows AssetBundle. Match the target game's Unity version. | [Unity Hub](https://docs.unity.com/en-us/hub/install-hub), [Editor archive](https://unity.com/releases/editor/archive) |
| AssetRipper | Create a local Unity-project reference export for asset, material, hierarchy, and dependency inspection. | [AssetRipper](https://github.com/AssetRipper/AssetRipper) |
| BepInEx IL2CPP | Load/configure the injection plugin in an IL2CPP Unity game. | [BepInEx IL2CPP install guide](https://docs.bepinex.dev/master/articles/user_guide/installation/unity_il2cpp.html) |
| OPERATOR Map Replacement Toolkit | Configure an explicit target scene, local bundle path, and prefab asset path. It is generic and ships no map. | [Toolkit repository](https://github.com/0xFlan/operator-map-replacement-toolkit) |
| .NET SDK | Build the toolkit source against the user's local generated BepInEx interop assemblies. | [Download .NET](https://dotnet.microsoft.com/download) |
| Unity AssetBundle documentation | Verify the correct `BuildPipeline.BuildAssetBundles` workflow and target platform. | [Unity AssetBundle guide](https://docs.unity.cn/Manual/AssetBundles-Building.html) |

## Optional reference tools

| Tool | Use | Link |
| --- | --- | --- |
| Official/public OPERATOR Modding Toolkit | Reference existing community conventions and compatible workflows; audit before reusing code. | [operator-modding-toolkit](https://github.com/ArchdukePierre/operator-modding-toolkit) |
## Work in this order

1. Build and test the generic Map Replacement Toolkit first. Its configuration
   must name one explicit scene, one local bundle path, and one prefab asset
   path.
2. Follow this guide to create a bundle, then validate it in overlay mode.
3. Move to root-only replacement only after terrain, spawns, materials,
   lighting, and player-camera tests pass.

## Reusable templates released with this guide

- `templates/Editor/BuildLocalMapBundle.cs`: deterministic Windows bundle
  build template with an explicit prefab path and strict build mode.
- `templates/Editor/ValidateMapRoot.cs`: editor-side prefab/root/collider
  contract check before packaging.
- `tools/Deploy-LocalMapCandidate.ps1`: game-closed backup/copy/hash template.

These templates are generic starting points. Set their paths, root names, and
deployment destination for each map.
