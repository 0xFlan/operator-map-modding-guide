---
name: operator-unity-modding
description: Build, repair, research, and verify native-quality additive OPERATOR Unity mods and crash-resistant local modding APIs. Use for OPERATOR BepInEx/IL2CPP maps, cosmetics, weapons, packages, content identity, Mirror integration, native materials, foliage, spawns, assets, Unity bundles, Creator SDK validation, public releases, or release QA.
---

# Operator Unity Modding

Use this skill for an OPERATOR map or visual mod where the result must behave and render like the shipped game, not merely compile or appear correct in an isolated Unity project.

Use **OPERATOR: Modded Operations — Standalone Map Framework** as the public
framework name. Use **OPERATOR: Modded Operations** as the short name and
**MODDED OPS** as the mission-laptop tab label. Keep *Cerberus* as the name of
the shipped mission-laptop UI location.

Read [references/operator-native-rendering.md](references/operator-native-rendering.md) before changing foliage, materials, lighting, terrain, or player spawns. For a project with a BIBLE or evidence log, read that file completely. Refer to the file as `<PROJECT_EVIDENCE_LOG>` in reusable output.

Read [references/operator-mod-api-research.md](references/operator-mod-api-research.md) before researching or changing a generalized OPERATOR mod API, standalone/additive content registration, Nexus package format, multiplayer content agreement, build adapter, diagnostics plugin, or Creator SDK contract.

Read [references/operator-native-cerberus-ui.md](references/operator-native-cerberus-ui.md) before changing the OPERATOR mission-laptop/Cerberus tabs, operation lists, briefing pages, or shared laptop state.

Read [references/operator-standalone-map-runtime.md](references/operator-standalone-map-runtime.md) before changing a standalone scene/package, map-specific BepInEx companion, portable material transport, runtime TerrainData, A* graph, AI/HVT grounding, map release layout, or Restart/KIA lifecycle claim.

Read [references/operator-interactive-prefabs.md](references/operator-interactive-prefabs.md) before importing, cloning, reconstructing, networking, or validating `DoorV2`, `DoorHandleV2`, FinalIK interaction objects, shootable door parts, or A* door links.

Read [references/implementation-locators.md](references/implementation-locators.md) before citing standalone assembly names, source members, bundle names, scene identity, Ukrainian Forest runtime evidence, or reusable file-system paths.

## Non-negotiable workflow

1. Establish the actual game build, Unity editor version, mod path, authoring project, runtime source, and deployed files. Preserve unrelated user changes. For generalized API work, bind every build or deployment to an exact source-state manifest and exact game/dependency fingerprints; a Git commit alone is insufficient when an input repository is dirty, untracked, or unborn.
2. Inspect the shipped equivalent before changing the mod. Record the native shader, material properties, textures, LOD representation, scene/runtime owner, and player-spawn code path. Do not generalize from names or a Unity preview.
3. Author with complete native prefabs or complete native meshes plus their matching native material closure. Keep source scale, embed bases into the sampled terrain, and reject positions that violate the playable lane or spawn area.
4. For every material, package the base-color, alpha, normal, mask, and authored property table. At runtime bind the exact installed shader family and recreate its native material state; never use a generic Standard/HDRP approximation as the final visual path. If an imported raw `.mat` is `Hidden/InternalErrorShader`, do **not** call `CopyPropertiesFromMaterial` on it: use it as serialized evidence, retain proxy texture bindings, and explicitly rehydrate the live family profile.
   - Resolve `NATIVE_PROXY_*`, library/template wrappers, and `MOD_*` names back to the original native material identity before selecting that profile. A proxy name must never decide whether a material is grass, leaf, bark, or a native special case.
5. Treat foliage as a special case: validate alpha cutout, double-sided state, queue, material-type mask, shadow/depth passes, normal/map keywords, and native color values. Transparent image files alone do not make foliage transparent in HDRP.
6. Treat spawn correction as a networked runtime behavior, not only a marker-placement task. For a standalone package, install only the current package scene's verified marker list after the companion world contract and before native player creation. On the current exact build, team PVP is one-based: assign `SpawnPoint.Team=1` to Team 1 markers and `SpawnPoint.Team=2` to Team 2 markers, and select them from `PlayerMaster.MyTeamIdentifier.TeamID`. Do not infer a team from `ToString()` or retain a cached marker after the player's team changes. For a legacy retail-scene overlay, patch the selected spawn and move path and keep a bounded late-player safety repair that targets the actual local-player hierarchy and Smooth Sync ownership. If that legacy path loads a large bundle after OPERATOR reads template markers, prime the original markers and invisible support pads before bundle loading. Inspect the mode owner too: Standard PVP (`PvpGameode`) and FFA (`FFA`) can run first-round or respawn paths outside `GameManager.MovePlayerToSpawn`. Audit exact installed signatures, then use an idempotent map-scoped handoff at the game scene callback and relevant game-mode spawn phases.
7. Build, run static layout/material validators, deploy only while the game is closed, and test a real operation. A forced-scene screenshot is diagnostic evidence only; it is not proof that a player spawns on ground or that gameplay rendering is correct.
8. Do not say the mod is ready while a user-reported visual, collision, spawn, or asset problem remains unverified.
9. Keep standalone ownership explicit: Core/catalog and OPERATOR: Modded Operations stay map-independent; the package directory stays data-only; an optional separately installed map companion owns only exact-package/exact-scene runtime reconstruction and tears it down on unload. Do not put a companion DLL inside `BepInEx/OperatorMods` or put map-specific shader/navigation logic in the framework.
10. Match the vanilla game-mode owner by operation mode. For standalone PVE,
    a bare `GameMode` is insufficient: the verified KIA path requires an
    `InfiltrationManager`-compatible component,
    `InfiltrationManager.instance`, and its synchronized `RaidTimer`.
    Suppress unsafe official-scene-only callbacks, maintain only the bounded
    standalone state, and clear static ownership on unload/restart. Keep the
    shipped persistent `GameManagerNetwork` as the owner of Mission Failed UI
    and Restart Operation; never clone that UI into a map bundle/companion or
    load a donor mission to supply it.
11. Preserve native launch and AI ownership. At Confirm, capture the exact
    player-owned `MissionLaptop` and `PlayerNetworking` before asynchronous
    package I/O. Keep the modal in a loading state, then close it and start the
    native operation in one final frame. For PVE, filter registered prefabs for
    root `BrainAI`, root `NetworkIdentity`, enabled weapon spawning, and a
    non-empty weapon list. Create bots through
    `RaidManager.ServerSpawnAI(false)`, not a one-argument manual network spawn.
    Keep these current candidate rules at `PROVEN-STATIC` until a physical
    first Confirm and reciprocal firearm test pass.
12. Define navigation and AI markers from the authoritative playable physics
    and bullet-interaction volume, never a larger render-only terrain/scenery
    apron. The map/package owns marker coordinates; the exact-scene companion
    rejects or quarantines out-of-bounds markers before grounding/nav lookup and
    scans only the playable graph; OPERATOR: Modded Operations consumes the package's PVE
    `minEnemies`/`maxEnemies` without map names or coordinates.
13. Treat normal `DoorV2` objects as authored map/building prefab content. Import an authorized complete source prefab with its original `.meta` and all dependencies, preserve its pivot, physics sync, paired handles, FinalIK objects, damage parts, navigation cut, and both A* links, and let normal scene and Mirror lifecycle initialize it. Do not spawn the normal door graph from a companion. Some AssetRipper exports lose custom fields; reject those damaged exports. Keep run-time cloning or component reconstruction experimental. Preserve serialized dead fields unless the game developer migrates the existing prefab data.
    Run `templates/Editor/ValidateDoorV2Prefab.cs` before the map build. Its
    serialized graph checks are necessary but not sufficient: A* endpoint
    attachment, interaction, damage, replication, late join, restart, and
    unload still require the live door matrix.
14. Treat standalone player-spawn and time-code settings as reversible
    process-global state. Capture and conditionally restore the previous
    `SpawnPointsInScene`, `Pspawns`, `PnextSpawnIndex`, NVG colour, and runtime
    Volume ownership on unload and restart. Initialize the standalone spawn
    index to zero on the current pinned build. Route an owned player through
    `PlayerMaster.SpawnPlayer()` so the shipped `ClientSpawnBS` owner setup
    runs on request 1. If a repeat-generation owned host still has no new
    `PlayerSpawnedObject` after 300 frames, use the exact generated server body
    as a bounded recovery; use it on request 1 only for an unowned server
    player. Record attempts before native calls, cap them per
    player/generation, and retain a completed-player set. Register each
    run-time PVE/PVP game-mode template on every peer with a deterministic,
    collision-checked nonzero Mirror asset ID before host spawn; validate and
    adopt the expected clone on remote peers and unregister on release.
    If a map companion holds a local player transform during initial
    grounding, publish its ready/applied state only after the exact current
    `Terrain` and `TerrainCollider` share one `TerrainData`. Clear the held
    transform/controller, safety frames, counters, applied flag, destination
    scene, and local-move flag on scene unload before armory return.
15. Keep package mission presentation explicit. The closed manifest owns row
    and briefing text, time choices, infiltration records, and the map-level
    preview path. Keep the raw preview outside Unity bundles, verify its
    length/hash, and use the same decoded sprite for preparation, fullscreen,
    and infiltration-selector views. Schema version 1 has no unrestricted
    per-user image override: changing the image requires a new author-built
    package version with updated `files[]` bytes/hash and content identity.
    Normalized infiltration anchors place 2D
    UI markers; the selected scene spawn set places players in 3D. Put
    reusable/address-loaded Unity assets in dependency bundles and put the
    exact `.unity` scene in the scene bundle.
16. For Standard PVP on the current pinned build, create a
    `PvpGameode`-derived owner, not a plain `GameMode`. Assign separate
    non-empty Team 1 and Team 2 `SpawnPoint` lists. Seed `MaxRounds=13`,
    `RoundsToWin=7`, and `RoundTime=120`. Call the shipped
    `PvpGameode.OnStartClient` and `Server_AllPlayersLoaded` bodies. Supply two
    audio sources, all 16 non-empty clip arrays with retail lengths,
    `TeleType`, timer/score text, six result roots, two animators, exact fade
    strings, and win/lose/tie text. Keep shipped respawn, freeze, death, score,
    round, and end-operation methods. Stop generic repeated movement after
    native PVP activates. Clear `PvpGameode.instance` during teardown only
    when the operation still owns it. Require a host and remote client test.

## Native-art decision rules

- Use the game's own textures and materials. Generated art can supplement only when the user explicitly permits it and it is compatible with the native shader contract.
- Use highest authored LOD for every directly placed object. Do not make a low LOD the "high quality" version.
  A deliberate LOD0-only root is acceptable, but every placement/repair utility must treat its MeshFilter/MeshRenderer hierarchy as occupied even without an LODGroup.
- A tree is not accepted from structural closure alone. Require a complete
  crown silhouette through the normal player camera at close, middle, and far
  distances; reject a family that reads as bare trunks even when its mesh,
  submesh, and material counts are technically valid.
- Ground a combined crown-and-trunk tree from the lowest valid point of its
  visible rendered root system after final position, yaw, scale, and one
  batched `Physics.SyncTransforms()`. Do not use the bottom of a trunk
  collider: an invisible capsule overhang below the modeled roots raises the
  visible tree above a hill. Apply the declared root embed and reject a tree
  below the declared minimum above-ground rendered fraction. The Ukrainian
  Forest contract uses a `0.12 m` embed, `0.75` minimum fraction, and `12 m`
  maximum absolute correction.
- A one-sided/open mesh is not a boulder. Require a complete closed (or bottom-only-open) native LOD0 mesh, matching material, collider, and multi-angle QA.
- For slope-bound cover, measure the full collider/mesh footprint. Reposition or remove a sandbag wall when its sampled terrain span exceeds the allowed contact tolerance; do not hide a floating wall with a vertical offset.
- Avoid perfect rows. Use a deterministic but nonuniform layout with varied lateral position, longitudinal spacing, rotation, and compatible ground embedding; preserve deliberate lanes and spawn clearances.
- For outdoor ground, inspect the shipped Terrain component chain before treating direct MeshRenderers as equivalent. A Terrain with `TerrainBRGRegisterer` and `drawTreesAndFoliage=False` is a BRG/detail-data path; stage any custom detail injection behind a private flag, preserve a direct-native fallback, and do not claim parity or foliage interaction before a player-camera test.
- Establish the target scene's lighting owner before changing a sun or HDRP Volume. Use `VectorTimeOfDay.singleton` -> `SetTimeOfDay(float)` -> `SunLight`/`SunData` only when that live SunLight belongs to the target scene. Otherwise retain the target map's static `Nice Sun` root and apply only fields verified from a matching shipped static scene; preserve its HDRP, lens-flare, on-demand-shadow, and directional-resolution components, then refresh its shadow contract after changing it. `SunSettings`, `Mirror_DayNight`, or weather fields are not ownership proof by themselves.
- Inspect a matching shipped HDRP Volume and `HDAdditionalLightData` before recreating exposure, tone mapping, bloom, or light units. Do not copy a weather effect (for example, a desert dust volume) into an unrelated biome, and do not call a profile from another map an exact match unless its live overrides were recorded. Preserve the selected native light, assign `RenderSettings.sun`, remove/disable competing mod/template directional lights, and never add a directional fill beside it. Create one explicitly logged fallback only when no verified target-scene sun exists, then validate source ownership and a live one-shadow-direction result.
- When a shipped HDRP `Volume.sharedProfile` uses `TonemappingMode.External`, resolve the profile PPtr and its `Texture3D` LUT from the installed asset data. Package the exact raw LUT as a linear, one-mip half-float Texture3D; load it through the native IL2CPP AssetBundle path; validate its dimensions/format in the emitted bundle; and use an explicitly logged safe fallback if it cannot load. Only describe the full Volume as exact after every applied override is directly decoded or live-audited.
- Treat red-dot and laser appearance as a rendering-stack question before changing optic materials: compare the live HDRP exposure/tonemap/bloom/camera contract at matched settings first.

## Evidence and self-improvement loop

After each failed test, add a short entry to the project's BIBLE with:

- observed symptom and test context;
- confirmed root cause versus rejected hypotheses;
- exact changed file/property/method;
- validation result and remaining uncertainty.

Promote a finding into this skill's reference only after it is reproducible from shipped data or a successful runtime test. Never turn an untested workaround into a reusable rule. When game updates change shaders, scene structures, or generated interop, repeat the inspection and revise the reference rather than trusting historical values.

## Required verification matrix

| Area | Minimum evidence |
| --- | --- |
| Foliage | Native-map material audit + native-map capture where possible + custom-map close/mid/far crown-silhouette capture + no opaque atlas cards or visually bare tree family |
| Props/rocks | Matching native mesh/material closure + multi-angle complete-shape check + collider/grounding check |
| Terrain | Dimensions, collision/raycast, texture mips, normal maps, and seamless blend validation |
| Spawn | Real operation, local player transform at a terrain point, no late snap/fall, respawn tested |
| LOD | Runtime audit that all direct objects use authored LOD0/highest detail and no quality system silently substitutes a proxy |
| HDRP environment | Shipped scene Volume + light-unit audit, custom map profile audit, and player-side visual check; do not trust an arbitrary offscreen BRG camera |
| Standalone ownership | Exact package/map/scene gating + generic-adapter map-name isolation + companion refusal outside its scene + clean teardown |
| Runtime navigation | Exact playable physics/bullet bounds + pre-nav marker containment + one resident playable-only scanned graph + tight ground and `IsPointOnNavmesh` proof for every enemy/HVT/mission marker before and after restart |
| Interactive doors | Complete reference graph + hinge-axis pivot + two-sided FinalIK interaction + latch/hinge damage + native A* open/breach traversal + host/client/late-join/restart teardown |
| Mission UI/lifecycle | One physical first Confirm without a second laptop interaction + tab/Back/Cancel/selector flow + exact scene + PVE/PVP isolation + reciprocal firearm damage + normal Restart; separately prove native lethal damage -> shipped Mission Failed popup -> shipped Restart control -> fresh playable exact scene, with the mode singleton/timer reset |
| Deployment | Source/deployed hashes match while OPERATOR was closed |

## Safe execution

- Never overwrite a user's authoring scene or unrelated project files.
- Do not deploy while OPERATOR is running.
- Treat OperatorModAPI as a local in-process extension framework, not an authentication, sandbox, anti-cheat, or hostile-code security boundary. Prioritize bounded work, deterministic teardown, exact compatibility refusal, actionable diagnostics, and containment of ordinary mod failures.
- Keep reverse engineering read-only and fingerprint-pinned. Do not execute analyzed code, attach invasive tooling to a user's game, or redistribute DLLs copied from the installed game.
- A QA game instance launched by the agent may be closed only after its executable path, PID, and start time are verified. Use a graceful close first; never force-close a user's game.
- Keep private QA switches environment-gated and out of normal release behavior.
- Remove every private force-scene, capture, material-audit, and auto-launch flag after QA. A normal session must not load a source map, start an operation, disable player cameras, or retain invisible diagnostic support.
- Use tokens such as `<OPERATOR_INSTALL>`, `<AUTHOR_WORKSPACE>`,
  `<PROJECT_EVIDENCE_LOG>`, and `<USER_PROFILE>` in reusable documentation.
  Do not publish a private drive path or an operating-system account name.
