# OPERATOR Map Modding Manual

A technical manual for people who build, package, load, and validate custom
OPERATOR maps. It uses the current standalone Modded Operations method. It
assumes a Windows PC, a legal local OPERATOR installation, Unity, AssetRipper,
and BepInEx IL2CPP.

The framework in this manual is **OPERATOR: Modded Operations — Standalone Map
Framework**. Read the
[OPERATOR Standalone Map Modding BIBLE](OPERATOR_MAP_MODDING_BIBLE.md) before
you make a release package.


## Build a map in this order

1. Read the [language and evidence rules](docs/00-writing-standard.md).
2. Install the tools in [docs/00-toolchain.md](docs/00-toolchain.md).
3. Create the workspace in [docs/01-workspace-setup.md](docs/01-workspace-setup.md).
4. Inspect a shipped reference map with [docs/02-recon-first.md](docs/02-recon-first.md).
5. Export reference assets, create a clean Unity project, and build a Windows
   bundle using [docs/03a-assetripper-to-bundle.md](docs/03a-assetripper-to-bundle.md).
6. Author terrain, full prefabs, foliage, cover, and LOD using
   [docs/03-authoring-native-quality.md](docs/03-authoring-native-quality.md).
7. Wire the exact mission row, briefing, preview, infiltration marker, time,
   bundle, and scene data with
   [docs/03b-modded-operations-presentation.md](docs/03b-modded-operations-presentation.md).
8. Wire PVE or the shipped `PvpGameode` owner with
   [docs/03c-native-mode-ownership-and-pvp.md](docs/03c-native-mode-ownership-and-pvp.md).
9. Build the [standalone package](docs/10-standalone-packages.md).
10. Implement the [standalone runtime flow](docs/04-runtime-integration.md).
11. Implement and test first spawn, respawn, collision, and bounds using
   [docs/05-spawn-and-gameplay.md](docs/05-spawn-and-gameplay.md).
12. Build and test [AI navigation and routes](docs/11-ai-navigation-and-behavior.md).
13. Wire native interactive objects with the
    [`DoorV2` reference method](docs/09-interactive-prefabs-and-doorsv2.md).
14. Match HDRP, sunlight, volumes, fog, and player-camera fidelity using
   [docs/06-hdrp-and-fidelity.md](docs/06-hdrp-and-fidelity.md).
15. Complete the gates in [docs/07-validation-and-release.md](docs/07-validation-and-release.md).
16. Use the [exact implementation reference](docs/13-exact-implementation-reference.md)
    to locate current code, bundles, assets, object names, and verification
    commands.

## Runtime paths

For a map selected from the mission laptop and loaded without a retail donor
map, use the standalone architecture. Use a strict data-only package,
**OPERATOR: Modded Operations**, and an optional exact-scene map companion for
native material, `TerrainData`, navigation, or other runtime-only
reconstruction. Map-specific reconstruction does not belong in the generic
framework.

The scene/package owns marker coordinates, combat walls, tree families, and
PVE `minEnemies`/`maxEnemies`. The exact-scene companion builds navigation
from the authoritative gameplay physics/bullet volume and rejects outside
markers before graph lookup. OPERATOR: Modded Operations consumes the declared
population range and stays free of map coordinates. Tree-family acceptance also requires
close/mid/far player-camera crown silhouettes; structural asset closure alone
does not prove foliage quality.

The MapBridge retail-scene overlay method is `RETIRED` for mission parity. It
remains available for explicit local diagnostics. See the
[archived overlay method](docs/archive/legacy-mapbridge-overlay.md).

## Documentation map

- [Technical BIBLE](OPERATOR_MAP_MODDING_BIBLE.md): normative ownership,
  package, asset, runtime, AI, interactive-object, lifecycle, multiplayer, and
  release contracts.
- [Writing standard](docs/00-writing-standard.md): ASD-STE100 language and
  evidence-status rules.
- [00 Toolchain](docs/00-toolchain.md): required downloads and working order.
- [01 Workspace setup](docs/01-workspace-setup.md): folder layout and baseline
  Unity project setup.
- [02 Recon first](docs/02-recon-first.md): inspect the target scene and game
  systems before changing anything.
- [03 Asset authoring](docs/03-authoring-native-quality.md): complete assets,
  terrain, materials, foliage, grounding, props, and LOD.
- [03a AssetRipper to bundle](docs/03a-assetripper-to-bundle.md): reference
  export, asset closure, map prefab, and `StandaloneWindows64` bundle build.
- [03b Modded Operations presentation](docs/03b-modded-operations-presentation.md):
  exact mission row, briefing, preview-image, infiltration-marker, time-code,
  bundle-content, hashing, and in-game presentation workflow.
- [03c Native mode ownership](docs/03c-native-mode-ownership-and-pvp.md):
  exact PVE/PVP owner boundary, one-based team markers, shipped
  `PvpGameode` defaults and references, lifecycle, teardown, and test gates.
- [04 Runtime integration](docs/04-runtime-integration.md): standalone
  ownership, exact load order, readiness, mode owner, restart, and teardown.
- [05 Spawn and gameplay](docs/05-spawn-and-gameplay.md): handoff timing,
  network player order, and test matrix.
- [06 HDRP and fidelity](docs/06-hdrp-and-fidelity.md): sun/Volume ownership,
  terrain, foliage, and player-camera comparison.
- [07 Validation and release](docs/07-validation-and-release.md): static,
  runtime, and in-game gates.
- [08 Troubleshooting](docs/08-troubleshooting.md): symptom-to-layer guide.
- [09 Interactive prefabs and `DoorV2`](docs/09-interactive-prefabs-and-doorsv2.md):
  lost AssetRipper fields, exact object wiring, lifecycle, networking,
  navigation links, and test matrix.
- [10 Standalone packages](docs/10-standalone-packages.md): manifest fields,
  file closure, identity, markers, terrain payloads, and load validation.
- [Package JSON Schema](schemas/operator-map-package.schema.json): closed
  machine-readable pre-v1 manifest contract.
- [Package manifest template](templates/operator-map-package.example.json):
  complete PVE/PVP schema shape with explicit placeholder hashes; read the
  [template instructions](templates/README-package-template.md) before use.
- [11 AI navigation and behavior](docs/11-ai-navigation-and-behavior.md):
  playable-only A* graph, marker grounding, routes, cover, combat, and restart.
- [12 Asset data contracts](docs/12-model-texture-material-terrain.md): model,
  pivot, texture, material, foliage, tree-family, terrain, and runtime-audit
  requirements.
- [13 Exact implementation reference](docs/13-exact-implementation-reference.md):
  path tokens, source members, assembly names, bundle names, Ukrainian Forest
  asset addresses, scene objects, and exact load order.
- [Archived methods](docs/archive/README.md): methods that MUST NOT be used as
  current standalone release proof.
- [Codex skill](skills/operator-unity-modding/SKILL.md): reusable instructions
  for human-supervised AI work on OPERATOR mods.

## Install the Codex skill

Copy the complete `skills/operator-unity-modding` directory to the Codex
skills directory. On Windows, the default destination is:

```text
<USER_PROFILE>\.codex\skills\operator-unity-modding\
```

`<USER_PROFILE>` means the operating-system user profile directory. Keep this
token in public documentation. Do not publish an actual account name.

Keep `SKILL.md`, `agents/openai.yaml`, and the `references` directory together.
Start a new Codex task after installation. Invoke the skill as
`$operator-unity-modding`.

## Status vocabulary

- **Authoring candidate:** opens in Unity; not runtime tested.
- **Static candidate:** bundle and structural checks pass; gameplay not proven.
- **Runtime candidate:** loads in a normal session; visual/gameplay QA remains.
- **Release candidate:** normal player-camera, first-spawn, respawn, collision,
  material, lighting, pre-nav marker containment, playable-only
  navigation/all-marker grounding, package PVE range, reciprocal combat,
  close/mid/far foliage silhouettes, restart, teardown, and bounds checks pass.
  Death/KIA restart remains a separate gate. It MUST use the generic
  framework's native-compatible mode owner and the shipped failure UI. The
  map bundle MUST NOT own this UI.

Do not call a map ready because it looks correct in the Unity editor or in a
forced-camera capture.
