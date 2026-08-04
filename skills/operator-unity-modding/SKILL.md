---
name: operator-unity-modding
description: Build, repair, research, and verify native-quality standalone OPERATOR Unity maps and additive BepInEx IL2CPP integrations. Use for data-only map packages, OPERATOR Modded Operations UI, scene bundles, map companions, native HDRP materials, TerrainData, foliage, A* navigation, AI markers, DoorV2, spawns, restart, and release QA.
---

# OPERATOR Unity Modding

Use this skill when an OPERATOR map MUST load through the mission section and
behave like a shipped operation.

Use **OPERATOR: Modded Operations — Standalone Map Framework** as the public
framework name. Use **OPERATOR: Modded Operations** as the short name and
**MODDED OPS** as the mission-laptop label.

Read only the references that match the task:

- Read [references/standalone-runtime.md](references/standalone-runtime.md) for
  packages, load order, ownership, mode state, restart, or teardown.
- Read [references/native-rendering.md](references/native-rendering.md) for
  models, textures, materials, terrain, foliage, LOD, lighting, or cameras.
- Read [references/cerberus-ui.md](references/cerberus-ui.md) for mission tabs,
  rows, boards, selectors, or shared laptop state.
- Read [references/interactive-prefabs.md](references/interactive-prefabs.md)
  for `DoorV2`, `DoorHandleV2`, FinalIK, damage parts, or A* door links.
- Read [references/implementation-locators.md](references/implementation-locators.md)
  for privacy-safe path tokens, exact assembly names, source members, bundle
  names, scene identity, or Ukrainian Forest runtime evidence.

Read the project's complete evidence log before you change a project that has
one. Treat its latest explicit superseding section as controlling.

## Required workflow

1. Identify the exact game, Unity, BepInEx, Il2CppInterop, A*, Core, and
   framework versions.
2. Record the authoring project, runtime source, package source, deployed files,
   and dirty repository state.
3. Inspect one shipped equivalent before you change the mod.
4. Separate `SUPPORTED`, `PROVEN-STATIC`, `EXPERIMENTAL`, and `RETIRED` claims.
5. Preserve complete model, texture, material, collider, LOD, and component
   closure.
6. Keep the package data-only.
7. Keep generic catalog, UI, scene load, readiness, mode, population, and
   restart logic map-independent.
8. Put installed-runtime map reconstruction in a separate exact-scene
   companion.
9. Require the companion world contract before actor creation.
10. Deploy only while OPERATOR is closed.
11. Test through the physical mission UI.
12. Update the evidence log and reusable reference after a reproducible result.

## Standalone ownership rules

- The package owns immutable IDs, operations, file hashes, bundles, preview,
  player range, PVE range, and exact scene path.
- The scene owns world geometry, collision, walls, lighting, portable assets,
  and marker coordinates.
- The generic framework owns private native-style UI, exact bundle and scene
  load, readiness, mode-compatible state, generic player/PVE/PVP population,
  shipped failure UI handoff, and restart.
- The companion owns only exact-scene materials, native data, navigation,
  marker repair, interactive initialization, and teardown.

Do not put a companion DLL below `BepInEx/OperatorMods`. Do not put map names,
coordinates, shader profiles, graph sizes, or door repairs in the generic
framework.

## Rendering rules

- Treat external-project materials as transport records when OPERATOR private
  shaders are unavailable.
- Preserve native identity separately from the proxy name.
- Package base/alpha, normal, mask, tint, and audited special maps.
- Create a fresh runtime material from the installed native shader family.
- Apply audited queue, alpha, culling, pass, keyword, and numeric state.
- Require zero active proxy or error shaders after several rendered frames.
- Do not copy properties from an `InternalErrorShader` source.
- Use cutout, depth, shadow, double-sided, wind, and material-type state that
  matches the native foliage family.
- Accept a tree only when its crown is complete at close, middle, and far
  player-camera distances.
- Use the highest authored LOD for directly placed objects.
- Require complete multi-angle mesh closure and grounded colliders.

## Terrain and navigation rules

- Bind one usable native `TerrainData` to both `Terrain` and
  `TerrainCollider`.
- Reconstruct it from lossless payloads when the IL2CPP wrapper is fake-null.
- Keep the gameplay wall separate from the visual terrain apron.
- Size the A* graph from the playable physics and bullet-interaction volume.
- Reject outside markers before nearest-node lookup.
- Require a small horizontal correction, a tight ground test, and
  `IsPointOnNavmesh` for every enemy, HVT, boss, and reinforcement marker.
- Do not use a long downward ray as grounding proof.
- Remove the map-owned graph and callbacks on unload.

## Spawn and mode rules

- At Confirm, capture the exact player-owned `MissionLaptop` and
  `PlayerNetworking` before asynchronous package I/O. Keep the modal in a
  loading state. Close it and start the native operation in one final frame.
- Install only the current package scene's player markers before native player
  creation.
- On the current exact build, assign `SpawnPoint.Team=1` to Team 1 markers and
  `SpawnPoint.Team=2` to Team 2 markers. Read
  `PlayerMaster.MyTeamIdentifier.TeamID`. Do not use a zero-based conversion
  or `TeamIdentifier.ToString()`.
- Remove a cached player marker when it does not match the player's current
  team.
- Wait for the world contract and shipped all-players-loaded barrier.
- Create PVE actors only on the server.
- Filter registered AI prefabs for root `BrainAI`, root `NetworkIdentity`,
  enabled `WeaponsAI.SpawnWeapon`, and a non-empty `weaponList`.
- Create bots through the shipped owner-aware
  `RaidManager.ServerSpawnAI(false)` path. Do not use a one-argument manual
  `NetworkServer.Spawn` replacement.
- Select an inclusive deterministic count from package `minEnemies` and
  `maxEnemies`.
- Create zero PVE actors for PVP.
- For PVE KIA, provide an `InfiltrationManager`-compatible owner,
  `InfiltrationManager.instance`, `GameMode.singleton`, and synchronized
  `RaidTimer` in the generic framework.
- Keep the shipped `GameManagerNetwork` Mission Failed UI and Restart control.

## Interactive-object rules

- Treat an AssetRipper interactive prefab as an evidence shell until all
  IL2CPP fields are proved.
- Treat normal `DoorV2` objects as authored map or building prefab content.
  Import an authorized complete source prefab with its original `.meta` and
  dependencies. Preserve the whole graph. Do not spawn the normal door from a
  companion. Keep run-time cloning and component reconstruction experimental.
- If reconstruction is necessary, build the inactive graph before activation.
- Wire the hinge pivot, rigid body, `MilkRigidbodySync`, colliders, paired
  handles, FinalIK objects, damage parts, navigation cut, and both A* links.
- Let Unity and Mirror call their normal lifecycle methods.
- Do not call private lifecycle methods to repair a null graph.
- Do not claim multiplayer support from a host-only test.

## Minimum verification matrix

| Area | Required evidence |
| --- | --- |
| Package | Closed schema, safe paths, exact lengths and SHA-256, exact scene path |
| UI | Physical tab, row, selector, Back, Cancel, Confirm, official-tab isolation |
| World | Exact identity, collision, walls, native materials, terrain, light |
| Foliage | No cards and complete crown at close, middle, and far distance |
| Navigation | One playable-only graph and all marker classes grounded/on graph |
| PVE | Count in package range, all actors inside wall, reciprocal combat |
| PVP | Zero PVE actors; host and client spawn and respawn on their authored opposite team sides |
| Door | Two-sided interaction, damage, AI open/breach, host/client/late join |
| Restart | Normal restart, respawn, and KIA restart recorded separately |
| Teardown | No duplicate graph, callback, actor, door, material, or singleton |
| Deployment | Source, staged, archive, and deployed hashes agree |

Do not call a release ready while a reported visual, collision, spawn,
navigation, interaction, restart, or multiplayer problem is not retested.

## Safety

- Do not publish installed game binaries or extracted assets without permission.
- Keep reverse engineering read-only and fingerprint-pinned.
- Do not deploy while OPERATOR is running.
- Do not terminate a user-started game or Unity process.
- Keep diagnostic auto-launch, forced-scene, and capture controls out of a
  release.
- Preserve unrelated user files and dirty worktree changes.
- Use tokens such as `<OPERATOR_INSTALL>`, `<AUTHOR_WORKSPACE>`, and
  `<USER_PROFILE>` in reusable documentation. Do not publish a private drive
  path or an operating-system account name.
