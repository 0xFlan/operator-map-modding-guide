# 8. Troubleshooting

| Symptom | First evidence to collect | Typical responsible layer |
|---|---|---|
| MODDED OPS row is missing | Core package admission result, package/directory ID equality, closed manifest/schema error | package catalog |
| Row text or briefing is wrong | selected immutable operation ID, `displayName`, `areaOfOperation`, `sitrep`, disabled localization state | package manifest and private UI binding |
| Preparation or fullscreen image is blank | resolved `previewImage` disk path, file hash, `ImageConversion.LoadImage` result, decoded dimensions | package preview and framework decoder |
| Old retail map image remains | private board child replacement and package preview ownership log | framework UI isolation |
| Infiltration marker is in the wrong place | final preview crop, normalized `mapPositionX/Y`, cloned marker anchors | package infiltration record |
| Selector has wrong label/count/limit | infiltration array order and the post-`SpawnMap` `MapInfilMarker` audit | package-to-native selector bridge |
| Confirm works only after reopening the laptop | captured player-owned `MissionLaptop`, in-flight bundle request, final-frame `CerebusOpboard.Start_Operation` log | framework Confirm ownership |
| Opaque foliage rectangles | live shader, alpha test, queue, tags, culling, keywords, maps | material rehydration |
| Flat or grey props | final shader, map bindings, tint precedence, bundle dependencies | material closure |
| Low-detail trees | mesh topology, LODGroup state, game quality owner | authored asset/LOD policy |
| Pine trees read as bare trunks | player-camera crown silhouette at close/mid/far range, source prefab family, leaf submeshes/materials | map-owned tree-family selection |
| Trees float above slopes or have deeply buried trunks | family, sampled contact X/Z/Y, LOD0 bark/trunk slots, submesh vertices, family-aware datum, complete renderer height, 0.75 above-ground fraction | authored family-aware datum and cross-slope contact contract |
| Pine lower trunk or branches are white | raw `pine_bark` and `Trunk_pine_var4` TextAsset load result, complete-state application log, submesh material slots, covering/snow floats, base/normal/mask bindings | verified dependency-asset loan or pine material rehydration |
| Second launch reaches `MAP LOADED !BUG!` and Restart Operation loops | deterministic PVE/PVP asset ID, fake-null value retained in `NetworkClient.prefabs`, cleanup by asset ID, `UnregisterSpawnHandler` call | generic game-mode prefab lifecycle |
| Open or invisible boulder | mesh closure, matching material, multi-angle player view | source mesh/material choice |
| Runtime reports foliage/debris but the area still looks empty | active renderer bounds, material/shader state, cluster internal radius, spawn/center/trench location counts, current player-camera captures | map composition and runtime material ownership |
| Foliage clusters look like tight knots | per-cluster member positions, crown overlap, internal radius/spacing, understory footprint and edge feathering | cluster layout rather than total tree count |
| Dead bodies keep their pose but float | settled skinned-mesh vertices/bounds after the death pose, sampled final surface, translation-only contact offset, low-angle capture | casualty contact measurement; do not rotate the accepted pose |
| Smoke is a stack of puffs or visible squares | output shader/material, flipbook and phase variation, six-way lightmaps, curl turbulence, soft-particle/depth fade, camera fade, fog response | VFX output and motion design, not spawn count alone |
| Fire and smoke appear swapped | source-local emissive flame bounds, smoke velocity/expansion/lifetime, material assignment per output | role mapping between fire and smoke systems |
| Crater has a black outline or renders vividly through fog | source alpha border/RGB dilation, mip edge, blend/cutout mode, threshold, conforming-mesh edge/depth offset, fog/depth pass | crater texture/material/mesh integration |
| Road-like terrain | terrain layer maps, normals, color space, mips, runtime material | terrain material |
| Hard grass/dirt change at a boundary hill | native Terrain extent versus render-only mesh extent, shared world-space weights, material family | terrain/exterior handoff |
| Blank edge/sky | collision terrain extent, visual exterior buffer, boundary position | bounds/exterior |
| Player starts high or falls | source markers, game-mode path, first network-object order, ground availability | spawn handoff |
| Player camera stays under terrain and input does not work | `PlayerMaster.PlayerSpawnedObject`, host server-body execution, current-scene spawn list, shipped `MovePlayerToSpawn` | generic framework player lifecycle |
| First mission works but the second launch has no player object | per-generation request dictionary, request-1 `SpawnPlayer` log, 300-frame wait, `owned-host-generated-server-recovery` log, new `PlayerSpawnedObject` | generic framework repeat-generation Mirror recovery |
| Double sun or odd shadows | scene light hierarchy, controller ownership, mod-created lights | lighting ownership |
| Different red dot/laser | player-camera exposure, tone map, bloom, optic camera/custom pass | environment stack |
| Works in editor but not game | bundle platform/version, installed shader availability, IL2CPP object validity | runtime integration |
| Brown proxy flashes before the detailed map | native loading canvas `activeSelf` and `activeInHierarchy`; exact call order in `OnSceneLoaded` | call shipped `GameManagerNetwork.ShowLoadingScreen()` before terrain/material preparation; let native readiness hide it |
| Exact scene remains brown/flat after readiness | live scene path, active proxy/error shaders, texture closure, companion activation, shared live `TerrainData` | map-owned material and terrain reconstruction |
| Enemies appear and fall vertically | resident scanned graph, every enemy/HVT marker height, tight ground delta, on-graph result | map-owned navigation/marker grounding |
| Enemies spawn beyond the barrier | gameplay-wall bounds versus visual terrain apron, marker coordinates/clearance, graph centre/dimensions | map-owned markers and companion navigation bounds |
| Too many or too few PVE enemies | package `minEnemies`/`maxEnemies`, valid marker count, host selection log | package population contract and generic adapter |
| PVP players start on the wrong side | numeric `MyTeamIdentifier.TeamID`, one-based `SpawnPoint.Team`, separate Team 1/Team 2 lists, marker prefixes | package scene markers and framework `PvpGameode` wiring |
| PVP players spawn once but no round starts | mode owner type, `PvpGameode.OnStartClient`, all-players-loaded log, native respawn coroutine, freeze state | framework native PVP lifecycle |
| PVP score or result causes a null exception | two audio sources, 16 non-empty clip arrays, `TeleType`, score/clock text, six result roots, animators, fade strings, status text | framework `ConfigureStandalonePvpPresentation` |
| PVP player snaps back after a native respawn | `nativePvpLifecycle` state and any continuing generic position loop | framework must stop the position-only fallback |
| Host PVP works but remote client does not load or spawn | identical peer hashes, bundle availability, registered asset ID `0x4D4F5002`, Mirror spawn message, expected clone type, adoption log | multiplayer package distribution and framework network lifecycle |
| PVP refuses before scene transition | exact framework/API DLL hashes, package content identity, optional companion identity, game build/capabilities, selected operation/scene/time/player range, frozen connection objects | protocol-v2 content agreement; version text alone is insufficient |
| Host waits at `ContentReady` | remote package preload/verification result, exact operation commit, rejection/timeout reason | remote content-ready phase |
| Host waits at `SceneReady` | exact selected scene, PVP template registration, `ceil(maxPlayers/2)` marker capacity, spawn contract, companion plugin/hash, READY marker, FAILED marker | current scene-generation readiness phase |
| PVP Restart reuses old readiness or times out | host UInt64 epoch, remote monotonic local scene generation, 5-second retries, 90-second deadline, stale/future/zero/overflow rejection | restart epoch handshake |
| A late join or reconnect aborts PVP | frozen connection-object membership and abort reason | expected fail-closed behavior; late join is unsupported |
| Companion READY exists but PVP still refuses | exact plugin GUID, SemVer, DLL hash, selected-scene ownership, and whether FAILED also exists | `runtimeCompanion` identity and failure precedence |
| PVP works online but PVE co-op differs between peers | selected-loader receipt/sidecar, PVE package/scene/count identity, AI marker selection, movement, projectile and damage logs | Protocol-v4 agreement is only a barrier; PVE still needs its own two-process AI/combat/completion/restart/teardown proof |
| Restart duplicates graphs or callbacks | scene-unload teardown log, graph/service ownership, callback generation | map companion lifecycle |
| Second scene load throws `Il2CppSystem.Object` to `Transform` cast errors | any `foreach (Transform child in parent)` in the map-scoped repeat path; typed `childCount`/`GetChild(index)` traversal | IL2CPP collection interop |
| Alive restart works but KIA restart fails | native death state, failure/end-screen owner, first exception | generic standalone end-screen lifecycle |
| Door looks correct but does not move | `PivotTransform`, rigid body, `MilkRigidbodySync`, component activation order | `DoorV2` graph and lifecycle |
| Door turns around its center | hinge-axis position and `PivotTransform` binding | door pivot |
| Door works from one side only | paired handles, rival references, front flag, FinalIK objects | `DoorHandleV2` graph |
| Bullets do not damage latch or hinges | `ShootableDoorPart.Door`, native `PartID`, collider layers | door damage graph |
| Player opens door but AI stops | both `NodeLink2` fields, endpoints, tags, live graph, navigation cut | A* door traversal |
| Door works for host but snaps for client | unique `NetworkIdentity`, `MilkRigidbodySync`, server spawn and client registration | Mirror ownership |
| Restart creates a second door | scene generation, map-owned object registry, callback teardown | map companion lifecycle |

Do not fix a symptom by adding unrelated geometry, light, or shaders with an
unrelated rendering contract.
Trace it to the narrowest responsible layer, add a regression check, and
record what evidence ruled out the alternatives.

## Brown proxy decision procedure

Use this order. Do not change bundles until the evidence selects the bundle.

1. Confirm the exact scene path in the log.
2. Confirm that `GameManagerNetwork.ShowLoadingScreen()` ran before
   `TryPrepareRuntimeTerrain` or the map companion's material repair.
3. Read `LoadingScreen.activeSelf` and `activeInHierarchy`. Both must be
   `true` at this boundary.
4. Ignore `LoadingScreenVisible` as a canvas-state probe on the supported
   build. Its RVA `0x0091A840` getter returns `_hideLoadingScreenSoon` at
   offset `0x2A4`.
5. If the brown world was visible only before readiness, fix presentation
   ownership. The portable proxy is expected cargo, but the player must not
   see it.
6. If the brown world remains visible after readiness, count active renderers
   that use portable or error shaders. Confirm that `Terrain` and
   `TerrainCollider` share the same live `TerrainData`. Then inspect texture
   closure and native property destinations.
7. Confirm the world-contract line. Ukrainian Forest requires
   `portableOrErrorShaderRenderers=0` and `valid=True`.

The supported-build vanilla pair is `ShowLoadingScreen` RVA `0x00916210` and
`HideLoadingScreen` RVA `0x0090E950`. Use the shipped pair. Do not create a
map-owned loading overlay.

A marker being on a scanned node does not prove that it is inside the playable
combat volume. Check wall containment before grounding or navigation. Likewise,
a tree passing mesh/submesh/material closure checks does not prove that its
crown reads correctly through the normal player camera; replace a visually bare
family with an audited complete native family in the map bundle.
