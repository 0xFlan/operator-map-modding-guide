# 1. Workspace setup

Create four separate locations. Do not use the live game folder as the Unity
project or AssetRipper workspace.

| Location | Purpose |
| --- | --- |
| Game install | Run OPERATOR and host BepInEx/plugin output. |
| Reference export | AssetRipper output used for inspection and selective local imports. |
| Unity authoring project | Your editable map prefab, editor scripts, textures, terrain payloads, and bundle output. |
| Release workspace | MapBridge source, guide, backups, hashes, test logs, and final local bundles. |

## Create the Unity project

1. Determine the OPERATOR Unity version from the installed game/reference
   export.
2. Install that editor version through Unity Hub or the Unity archive.
3. Create a new 3D/HDRP-compatible authoring project with that editor.
4. Switch the active target to **Windows 64-bit** before importing/building.
5. Create these folders in `Assets`:

```text
Assets/
  Maps/<YourMap>/
    Prefabs/
    Materials/
    Textures/
    Terrain/
    RuntimePayload/
  NativeReference/
    Meshes/
    Materials/
    Textures/
  Editor/
```

6. Copy `templates/Editor/BuildLocalMapBundle.cs` and
   `templates/Editor/ValidateMapRoot.cs` into `Assets/Editor/`, then set their
   paths to your map prefab and output folder.

## Keep the map root deterministic

Use one map root prefab at a fixed path such as:

```text
Assets/Maps/<YourMap>/Prefabs/<YourMap>Root.prefab
```

Use the same lower-case asset path when configuring the injector. The path in
the bundle matters; the visible GameObject name is not enough.

## First build target

Before importing trees or terrain, make a test prefab containing one cube and
one collider. Build it as a Windows AssetBundle, configure it in overlay mode,
and verify that the toolkit loads the exact prefab in the selected scene. This
separates loader/configuration problems from terrain, spawn, shader, and
foliage problems.

## Deploy a local candidate

With the game closed, use the supplied deployment template to back up the
current plugin files, copy the new DLL and bundle, and compare SHA-256 hashes:

```powershell
.\tools\Deploy-LocalMapCandidate.ps1 `
  -GameDirectory "D:\Games\OPERATOR" `
  -SourcePluginDll ".\OperatorMapBridge.dll" `
  -SourceBundle ".\Builds\your_map_bundle" `
  -PluginFolderName "OperatorMapBridge" `
  -BundleFileName "your_map_bundle" `
  -BuildNotesPath ".\Builds\your_map_bundle.build.txt"
```

Edit every path and file name for the local installation. The script refuses
to copy while the configured game executable is running, creates a timestamped
backup under `MapModBackups`, and prints the destination hashes.
