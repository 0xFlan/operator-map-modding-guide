# 7. Validation and release

## Use a layered gate

### Static structure

- package schema, directory closure, lengths, and SHA-256 pass;
- `previewImage` resolves to one declared raw JPEG or PNG and its final byte
  count/hash match `files[]`;
- every operation has complete row/briefing text, one or more infiltrations,
  valid normalized marker anchors, declared time codes, and a default time
  that occurs in the declared list;
- a strict forced Unity rebuild succeeds from the current source; reusable
  builder caches are cleared and old same-name bundles are not accepted as
  current evidence;
- dependency bundles reopen and contain no streamed scene;
- scene bundles reopen, contain the exact declared scene union, and every
  declared scene loads;
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
  current-build team IDs are exactly `1` and `2`, and each group contains at
  least `ceil(maxPlayers/2)` markers; the current 12-player maximum requires
  at least six per team;
- schema-v2 `runtimeCompanion`, when present, binds one exact plugin GUID,
  SemVer, selected-loader DLL SHA-256, the canonical loader-neutral
  `runtimeContentId` derived from both BepInEx and MelonLoader hashes, and
  distinct exact READY/FAILED scene-marker names;
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
- PVP logs show frozen connection-object membership, exact framework/API/
  package/companion identity, every remote `ContentReady`, and every current-
  epoch `SceneReady` before the single host owner spawn;
- a declared companion passes exact plugin/hash identity and READY only inside
  the selected scene generation; FAILED wins even after READY;
- retained-content PVP Restart advances the nonzero UInt64 scene epoch exactly
  once and rejects stale, future, zero, out-of-phase, and overflowing values;
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
- full foliage packages read naturally at both spawns and through the center;
  their internal spacing is neither a single-tree placeholder nor an
  implausibly condensed knot;
- lived-in debris is visibly dense where the brief requires itâ€”inside and
  around trenches, walls, camps, and wrecksâ€”and is not justified by a global
  object count alone;
- every retained casualty visibly contacts or is slightly buried in the final
  surface while preserving its accepted native pose; bounds-derived gap logs
  alone do not pass this gate;
- every required fire/smoke source has the correct role mapping; smoke reads
  as a continuous expanding plume without obvious puff tiles or square edges
  from close, middle, rear, and crosswind views;
- crater and wreck scorches have no black border, hard square, z-fighting,
  terrain clipping, or through-fog visibility error;
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
  sides; movement replicates; real firearm hits and deaths update the native
  score; the next round respawns both teams through the shipped `PvpGameode`;
  match end, restart, unload, and return-to-armory each clear the prior owner;
- PVP rejects a late join or any connection-object membership change; current
  releases must not advertise late-join support;
- online PVE passes a separate host/remote package, scene, AI placement,
  movement, projectile, damage, completion, restart, and teardown matrix; PVP
  agreement and single-player PVE do not satisfy this gate;
- Back, Cancel, tab switching, selector, and exact-scene ownership remain
  isolated from official mission rows;
- native KIA/end-screen restart is recorded separately from normal alive
  restart and is never inferred from it.
- each supported `DoorV2` passes front/back interaction, lock/latch, damage,
  breach, AI open/breach, host/client, late join, restart, and unload tests.

## Use a private stationary observer when a tester cannot steer the player

An unattended test can use the real owned player and the real retail output
camera without sending movement, aim, or fire input. This gives stronger
evidence than a free camera because the game must still complete player
creation, camera ownership, spawn handoff, readiness, rendering, and restart.

Keep the observer as a separate private BepInEx plug-in. Do not add it to the
framework, map companion, package directory, or release archive. The observer
must use this sequence:

```text
verify that OPERATOR is closed
-> install the private driver with an exact operation ID
-> launch the game and record the exact process path, ID, and start time
-> enter Lone Wolf
-> access the player-owned MissionLaptop
-> open the real Cerberus window and MODDED OPS tab
-> select the exact immutable operation row
-> press Execute and Confirm
-> confirm the shipped infiltration selector
-> wait for exact scene identity and native readiness
-> require the owned player and retail output camera at a declared spawn
-> require the mode-correct grounded actor population
-> capture the stationary player-camera view
-> call the shipped Restart Operation path
-> repeat the scene, player, camera, actor, and capture gates
-> quit, hash evidence, and remove the private driver
```

Do not choose only the first operation with a matching PVE or PVP mode. Read
an exact immutable operation ID from a private process setting or control file
and refuse when it is absent from the frozen catalog. Bound each observation
window. A profiled AI test that records snapshots through 120 seconds needs at
least a 122-second hold. A short hold can prove launch, rendering, grounding,
and restart only.

Do not patch or call a quarantined operation-start method to bypass the UI.
Do not convert this test to a synthetic free-camera scene load. Do not make the
player invulnerable when the test is intended to measure combat. On timeout,
close only a process whose executable path, process ID, and start time match
the process created by the private launcher. Use a graceful close first.

The result record must include the exact operation ID, mode, hold time, driver
SHA-256, captured process identities, log and screenshot hashes, pass/fail
state, and cleanup state. A passing run leaves no private driver, control file,
capture directory, or environment flag in the normal game installation.

Accept only evidence created by the current run. Filter screenshots and logs
by a recorded run-start time and captured process identity before moving them
into the evidence directory. A pre-existing file with a familiar name is not
current visual proof, even when its hash is recorded successfully.

The Forest `0.4.17` and Modded Operations `0.3.20` movement baseline passed
this workflow. The first generation created 15 grounded PVE actors. The
native restart created 14. Both generations kept the maximum absolute
AI-to-Terrain difference at `0.03 m`. At 120 seconds, all actors had moved at
least 1 m. The two maximum displacements were `51.19 m` and `49.34 m`.
Authored vegetation blocked the same-mask sight probe in both generations.
Both stationary retail-camera captures showed the complete forest. The
launcher removed all private QA files after the game exited.

The current completion runs also require and observe one shipped
`RaidManager`, one shipped `ExfilZone`, locked initial extraction,
`GameManager.allAI=0`, native zone/global unlock, the exact current-build ATAK
exfil visual, positive physical occupant counts, the 15-second timer, and the
Mission Successful After Action Report. For the Ukrainian Forest worked
example, the extraction root is the northern insertion root
`(0.000,0.112,7.000)`. See
[Native PVE completion, extraction, and ATAK](15-native-pve-completion-exfil-and-atak.md).

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
`OperatorMods/<package-id>` and each map companion under its loader's own
`BepInEx/plugins/<map-plugin>` or `Mods` location. Never place a companion DLL
inside the package root or the generic framework archive.

Keep local absolute paths, credentials, temporary logs, and test artifacts out
of the release package.

Ship the preview API Core/host pair only inside the matching Modded Operations
framework download until the API becomes a full stable release. Ship each map
as a separate download and do not duplicate the API or framework inside it. A
multiplayer `TEST ONLY` transfer ZIP is not a Nexus release and must not change
support wording or the hash-pinned public publication record.
