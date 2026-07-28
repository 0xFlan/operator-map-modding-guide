# OPERATOR Map Modding Guide

An open community guide for planning, authoring, injecting, validating, and
sharing OPERATOR map mods without redistributing game content.

This is intentionally a broad method guide, not a release of a particular map.
It contains no AssetBundles, extracted meshes, textures, materials, audio,
game binaries, private build paths, credentials, screenshots, or proprietary
asset payloads.

## Start here

1. Read docs/01-project-boundaries.md before collecting source material.
2. Read docs/02-recon-first.md before writing an injector or editing a scene.
3. Build the playable ground, collision, and spawn plan before decoration.
4. Use complete, legally obtained native-quality assets and preserve their
   material dependency closure.
5. Test through a normal player/game-mode flow, not only an editor view.
6. Share source code, tools, documentation, manifests, and reproducible
   validation results. Do not share content you are not permitted to publish.

## Companion software

The separate OPERATOR Map Replacement Toolkit is a generic local bundle/prefab
injector. It starts disabled and does not contain or select a forest map.
Injection alone does not make a bundle playable: map authors still own terrain,
spawns, lighting, materials, navigation, collision, and quality assurance.

## Documentation map

- docs/01-project-boundaries.md — legal, ethical, and project boundaries.
- docs/02-recon-first.md — inspect the shipped target before editing.
- docs/03-authoring-native-quality.md — geometry, terrain, props, materials,
  foliage, LOD, and placement.
- docs/04-runtime-integration.md — bundle delivery and runtime ownership.
- docs/05-spawn-and-gameplay.md — safe map handoff and spawn QA.
- docs/06-hdrp-and-fidelity.md — lighting, atmosphere, texture quality, and
  player-camera comparison.
- docs/07-validation-and-release.md — build gates and release artifacts.
- docs/08-troubleshooting.md — symptom-driven debugging.
- docs/ASSET_PROVENANCE_TEMPLATE.md — a non-asset manifest template.

## Status vocabulary

Use precise status language:

- Authoring candidate: opens in Unity; not runtime tested.
- Static candidate: builds and passes structural checks; not gameplay proven.
- Runtime candidate: loads in a normal session; visual/gameplay QA remains.
- Release candidate: all documented player-camera, spawn, respawn, collision,
  and provenance checks passed.

Do not call a map ready because an editor scene or forced camera looks good.

## License

The guide is MIT licensed. It is independent community documentation and is
not affiliated with OPERATOR, its developers, Unity, BepInEx, or other
third-party owners.
