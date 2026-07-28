# 7. Validation and release

## Use a layered gate

### Static structure

- bundle opens and contains the expected prefab;
- expected meshes/materials/textures are present;
- no null material slots;
- texture sizes, color spaces, and mips meet the intended contract;
- terrain/collider data exists and can be bound;
- layout checks cover route, spawn, slope, root embedding, and prop footprint.

### Runtime logs

- local bundle and prefab load succeeds;
- material repair reports installed shader families and critical state;
- LOD audit proves direct content uses the intended source detail;
- target-scene sun/Volume ownership is reported;
- ground and spawn handoff logs identify concrete coordinates;
- no private diagnostic mode remains enabled.

### Player-camera and gameplay

- player starts on the intended ground;
- team and free-for-all respawns work;
- player does not snap/fall to source-map space;
- foliage has no opaque atlas cards;
- rocks/boulders are complete from multiple angles;
- cover is grounded across slopes;
- boundaries are both blocked and visually credible;
- lighting, terrain, and optics match the intended reference at comparable
  settings.

## Deployment

1. Verify the game is closed.
2. Back up the old plugin/bundle.
3. Copy only intended files.
4. Compare source and destination hashes.
5. Record version, build source revision, bundle hash, runtime hash, test
   mode, scene, and QA result.
6. Restore default-safe configuration after private diagnostics.

## Community release package

Prefer a release that contains:

- source code;
- build scripts;
- documentation;
- manifests and validators;
- a changelog;
- known limitations;
- hashes for files you are permitted to distribute.

Exclude private credentials, local absolute paths, game binaries, and game
assets unless redistribution permission is explicit.
