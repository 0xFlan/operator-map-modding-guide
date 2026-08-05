# 7. Validation and release

## Use a layered gate

### Static structure

- package schema, directory closure, lengths, and SHA-256 pass;
- `previewImage` resolves to one declared raw JPEG or PNG and its final byte
  count/hash match `files[]`;
- every operation has complete row/briefing text, one or more infiltrations,
  valid normalized marker anchors, declared time codes, and a default time
  that occurs in the declared list;
- dependency bundles open and contain no scene;
- scene bundle opens and contains only the exact declared scene path;
- expected meshes/materials/textures are present;
- no null material slots;
- texture sizes, color spaces, and mips meet the intended contract;
- terrain/collider data exists and can be bound;
- gameplay-wall dimensions are distinct from any native-terrain visual apron;
- every operation-consumed AI marker is inside the gameplay wall with the
  authored clearance before grounding/navigation tests;
- terrain/exterior height and material-weight functions are continuous across
  their shared seam;
- layout checks cover route, spawn, slope, root embedding, and prop footprint;
- every complete tree has LOD0 bark/trunk submesh bounds, one renderer-free
  family-aware datum with its selected contact X/Z, its native lower-trunk
  collision shape, an absolute correction no greater than `12 m`, and at
  least `0.75` of its full rendered height above the sampled surface;
- each PVP spawn set has separate non-empty Team 1 and Team 2 marker groups;
  current-build team IDs are exactly `1` and `2`;
- each interactive prefab has a complete field/reference closure or is marked
  non-interactive;

### Runtime logs

- exact dependency and scene load order succeeds;
- the preview decoder reports the selected immutable map ID and does not use a
  retail fallback image;
- the private board contains the expected package target records, selected
  time, scene address, map prefab, and player/mode values;
- the shipped infiltration selector clones exactly the declared number of
  package markers and retains each declared label, index, limit, and type;
- companion activation matches the exact package, map, operation, and scene;
- material repair reports installed shader families and critical state;
- LOD audit proves direct content uses the intended source detail;
- target-scene sun/Volume ownership is reported;
- ground and spawn handoff logs identify concrete coordinates;
- PVP logs identify `StandalonePvpGameMode`, the Team 1/Team 2 list counts,
  and `nativePvpLifecycle=true`; a position-only fallback fails this gate;
- each peer logs registration of deterministic mode asset ID `0x4D4F5001` or
  `0x4D4F5002`; the remote peer logs validation and adoption of the matching
  spawned clone;
- one map-scoped A* service/graph is scanned for the exact scene;
- graph dimensions and centre match the gameplay physics/bullet volume rather
  than a larger visual apron;
- every enemy, HVT, and operation-consumed AI marker is tightly grounded and
  on graph, with rejected names/distances logged;
- runtime material auditing reports zero active proxy/error-shader renderers;
- restart replaces rather than duplicates map-owned graph/services/callbacks;
- a second additive-scene load has no IL2CPP object-to-`Transform` cast and
  uses typed `GetChild(index)` traversal for map-owned children;
- each native interactive object reports complete pivot, physics, interaction,
  damage, network, and navigation relationships;
- no private diagnostic mode remains enabled.

### Player-camera and gameplay

- one click selects the intended MODDED OPS row and updates the exact briefing;
- the preparation page, fullscreen map, and infiltration selector show the
  declared map preview with usable crop/aspect;
- every 2D infiltration marker is visibly on the intended preview location and
  remains distinct from the scene's 3D player-spawn contract;
- the time selector contains only the declared values and starts on the
  declared default;
- the first Confirm launches once without closing/reopening the laptop;
- player starts on the intended ground;
- after return to the armory, a second PVE launch creates a new
  `PlayerSpawnedObject`, uses the shipped movement coroutine, and restores
  first-person movement; when recovery is necessary, the log contains only
  the bounded `owned-host-generated-server-recovery` route after the
  request-1 `SpawnPlayer` route;
- team and free-for-all respawns work;
- player does not snap/fall to source-map space;
- foliage has no opaque atlas cards;
- each selected tree family has a complete crown silhouette at close, middle,
  and far player-camera distances; mesh/material/submesh counts alone do not
  satisfy this gate;
- on flat ground and multiple slopes, only the root zone is below terrain and
  at least three quarters of each tree is visibly above terrain;
- rocks/boulders are complete from multiple angles;
- cover is grounded across slopes;
- boundaries are both blocked and visually credible;
- no grass/dirt/rock material transition is visible at a boundary hill from a
  player camera or at a grazing light angle;
- lighting, terrain, and optics match the intended reference at comparable
  settings.
- PVE actors remain grounded and navigate after first load and normal restart;
- PVE actor count is inside the package-declared inclusive range and all actors
  remain inside the gameplay wall where player/AI bullets can interact;
- PVP creates no PVE actors before or after restart;
- PVP freeze time releases; host and remote client start on their authored
  sides; bullet deaths update the native score; the next round respawns both
  teams through the shipped `PvpGameode`; match end, restart, and
  return-to-armory each clear the prior owner;
- Back, Cancel, tab switching, selector, and exact-scene ownership remain
  isolated from official mission rows;
- native KIA/end-screen restart is recorded separately from normal alive
  restart and is never inferred from it.
- each supported `DoorV2` passes front/back interaction, lock/latch, damage,
  breach, AI open/breach, host/client, late join, restart, and unload tests.

## Deployment

1. Verify the game is closed.
2. Back up only the exact owned package and companion destinations.
3. Copy only intended files.
4. Compare source and destination hashes.
5. Record package version, source-state identity, every bundle hash, companion
   hash, runtime versions, operation mode, scene, and QA result.
6. Restore default-safe configuration after private diagnostics.

## Community release package

Prefer a release that contains:

- source code;
- a decompiled snapshot of each final mod DLL, with the input DLL version,
  byte length, SHA-256, and decompiler version;
- build scripts;
- documentation;
- manifests and validators;
- a changelog;
- known limitations;
- file hashes;
- a short installation and rollback procedure.

The authored source is the edit source. The decompiled snapshot is release
verification evidence. Regenerate it from the final DLL. Do not edit the
snapshot and then treat it as the maintained implementation.

When an authorized asset is too large or is not stored in normal Git history,
commit an explicit placeholder manifest. Use labels such as `[PREVIEW IMAGE]`,
`[PREFAB ASSET]`, `[TEXTURE SET]`, `[DEPENDENCY ASSETBUNDLE]`, and
`[SCENE ASSETBUNDLE]`. For each label, state the exact expected install or
Unity-project path, object type, bundle address, source record, and validation
gate. Do not put a zero-byte file at the expected release filename. A strict
package validator must fail until the real payload is present.

Do not publish OPERATOR binaries as mod decompilation evidence. Generated
interop assemblies expose signatures but do not contain the original IL2CPP
method bodies. Publish exact signatures, owner files, asset path IDs,
behavior summaries, and reproducible inspection steps. Keep the installed
game binary and extracted first-party payload outside the mod repository
unless its owner has explicitly authorized that exact redistribution.

For a standalone map that needs runtime reconstruction, ship both removable
parts with explicit ownership: the data-only package under
`BepInEx/OperatorMods/<package-id>` and the map companion under its own
`BepInEx/plugins/<map-plugin>` directory. Never place the companion DLL inside
the package root or the generic framework archive.

Keep local absolute paths, credentials, temporary logs, and test artifacts out
of the release package.
