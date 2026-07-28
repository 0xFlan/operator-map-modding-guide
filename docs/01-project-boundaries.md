# 1. Project boundaries

## Separate the reproducible method from the game content

Publishable items usually include:

- original plugin source;
- authoring scripts and validators;
- build instructions;
- scene/layout data you created;
- dependency/provenance manifests;
- test procedures and logs with sensitive information removed.

Do not assume that extracted game models, textures, materials, audio, shaders,
AssetBundles, data files, or binaries are redistributable. A safer workflow is
to require each user to own the game and prepare their own local dependencies
under rules set by the rights holder.

## Define a narrow goal

Before changing files, write down:

- target map scene and intended game mode;
- exact bundle prefab root that will be injected;
- visual goal and playable footprint;
- collision/boundary strategy;
- spawn ownership and required respawn paths;
- asset provenance and permitted distribution scope;
- planned validation evidence.

A map replacement is not merely a visible prefab. It is the interaction of the
scene, game mode, player spawn code, terrain/collision, native rendering
contract, lighting stack, and installed game quality settings.

## Preserve reversibility

- Keep the editable authoring project separate from deployed files.
- Back up the currently installed mod before copying a new DLL or bundle.
- Store source and deployed hashes after every controlled deployment.
- Never overwrite an active game session.
- Keep diagnostic flags and test-only launch code out of normal play.
