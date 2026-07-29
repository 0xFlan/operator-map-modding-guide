# OPERATOR Map Modding Manual

A technical manual for people building, packaging, injecting, and validating
custom OPERATOR maps by hand. It assumes a Windows PC, a local OPERATOR
installation, Unity, AssetRipper, BepInEx IL2CPP, and the companion generic
OPERATOR MapBridge.

This manual is self-contained. It does not require an AI assistant or the
internal engineering BIBLE.

## Build a map in this order

1. Install the tools in [docs/00-toolchain.md](docs/00-toolchain.md).
2. Create the workspace described in [docs/01-workspace-setup.md](docs/01-workspace-setup.md).
3. Inspect a shipped reference map using [docs/02-recon-first.md](docs/02-recon-first.md).
4. Export reference assets, create a clean Unity project, and build a Windows
   bundle using [docs/03a-assetripper-to-bundle.md](docs/03a-assetripper-to-bundle.md).
5. Author terrain, full prefabs, foliage, cover, and LOD using
   [docs/03-authoring-native-quality.md](docs/03-authoring-native-quality.md).
6. Configure the generic injector using [docs/04-runtime-integration.md](docs/04-runtime-integration.md).
7. Implement and test first-spawn, respawn, collision, and bounds using
   [docs/05-spawn-and-gameplay.md](docs/05-spawn-and-gameplay.md).
8. Match HDRP, sunlight, volumes, fog, and player-camera fidelity using
   [docs/06-hdrp-and-fidelity.md](docs/06-hdrp-and-fidelity.md).
9. Complete the gates in [docs/07-validation-and-release.md](docs/07-validation-and-release.md).

## Toolkit

The [OPERATOR MapBridge](https://github.com/0xFlan/operator-mapbridge)
loads one explicitly configured local bundle prefab into one explicitly
configured game scene. It is disabled by default, contains no map bundle, and
does not select a map automatically.

The injector does not make a map playable by itself. The map author must
provide working terrain/collision, spawns, material recovery, lighting,
boundary behavior, and validation.

## Documentation map

- [00 Toolchain](docs/00-toolchain.md): required downloads and working order.
- [01 Workspace setup](docs/01-workspace-setup.md): folder layout and baseline
  Unity project setup.
- [02 Recon first](docs/02-recon-first.md): inspect the target scene and game
  systems before changing anything.
- [03 Asset authoring](docs/03-authoring-native-quality.md): complete assets,
  terrain, materials, foliage, grounding, props, and LOD.
- [03a AssetRipper to bundle](docs/03a-assetripper-to-bundle.md): reference
  export, asset closure, map prefab, and `StandaloneWindows64` bundle build.
- [04 Runtime integration](docs/04-runtime-integration.md): configure and use
  the generic injector.
- [05 Spawn and gameplay](docs/05-spawn-and-gameplay.md): handoff timing,
  network player order, and test matrix.
- [06 HDRP and fidelity](docs/06-hdrp-and-fidelity.md): sun/Volume ownership,
  terrain, foliage, and player-camera comparison.
- [07 Validation and release](docs/07-validation-and-release.md): static,
  runtime, and in-game gates.
- [08 Troubleshooting](docs/08-troubleshooting.md): symptom-to-layer guide.

## Status vocabulary

- **Authoring candidate:** opens in Unity; not runtime tested.
- **Static candidate:** bundle and structural checks pass; gameplay not proven.
- **Runtime candidate:** loads in a normal session; visual/gameplay QA remains.
- **Release candidate:** normal player-camera, first-spawn, respawn, collision,
  material, lighting, and bounds checks pass.

Do not call a map ready because it looks correct in the Unity editor or in a
forced-camera capture.
