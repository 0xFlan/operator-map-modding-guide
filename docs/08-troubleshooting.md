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
| Trees float above slopes or have deeply buried trunks | sampled surface Y, lowest valid visible renderer Y, hidden collider overhang, complete renderer height, 0.12 m embed, 0.75 above-ground fraction | tree visible-root placement contract |
| Open or invisible boulder | mesh closure, matching material, multi-angle player view | source mesh/material choice |
| Road-like terrain | terrain layer maps, normals, color space, mips, runtime material | terrain material |
| Hard grass/dirt change at a boundary hill | native Terrain extent versus render-only mesh extent, shared world-space weights, material family | terrain/exterior handoff |
| Blank edge/sky | collision terrain extent, visual exterior buffer, boundary position | bounds/exterior |
| Player starts high or falls | source markers, game-mode path, first network-object order, ground availability | spawn handoff |
| Player camera stays under terrain and input does not work | `PlayerMaster.PlayerSpawnedObject`, host server-body execution, current-scene spawn list, shipped `MovePlayerToSpawn` | generic framework player lifecycle |
| First mission works but the second launch has no player object | per-generation request dictionary, request-1 `SpawnPlayer` log, 300-frame wait, `owned-host-generated-server-recovery` log, new `PlayerSpawnedObject` | generic framework repeat-generation Mirror recovery |
| Double sun or odd shadows | scene light hierarchy, controller ownership, mod-created lights | lighting ownership |
| Different red dot/laser | player-camera exposure, tone map, bloom, optic camera/custom pass | environment stack |
| Works in editor but not game | bundle platform/version, installed shader availability, IL2CPP object validity | runtime integration |
| Exact scene loads as brown/flat terrain | live scene path, active proxy/error shaders, texture closure, companion activation | map-owned material reconstruction |
| Enemies appear and fall vertically | resident scanned graph, every enemy/HVT marker height, tight ground delta, on-graph result | map-owned navigation/marker grounding |
| Enemies spawn beyond the barrier | gameplay-wall bounds versus visual terrain apron, marker coordinates/clearance, graph centre/dimensions | map-owned markers and companion navigation bounds |
| Too many or too few PVE enemies | package `minEnemies`/`maxEnemies`, valid marker count, host selection log | package population contract and generic adapter |
| PVP players start on the wrong side | numeric `MyTeamIdentifier.TeamID`, one-based `SpawnPoint.Team`, separate Team 1/Team 2 lists, marker prefixes | package scene markers and framework `PvpGameode` wiring |
| PVP players spawn once but no round starts | mode owner type, `PvpGameode.OnStartClient`, all-players-loaded log, native respawn coroutine, freeze state | framework native PVP lifecycle |
| PVP score or result causes a null exception | two audio sources, 16 non-empty clip arrays, `TeleType`, score/clock text, six result roots, animators, fade strings, status text | framework `ConfigureStandalonePvpPresentation` |
| PVP player snaps back after a native respawn | `nativePvpLifecycle` state and any continuing generic position loop | framework must stop the position-only fallback |
| Host PVP works but remote client does not load or spawn | identical peer hashes, bundle availability, registered asset ID `0x4D4F5002`, Mirror spawn message, expected clone type, adoption log | multiplayer package distribution and framework network lifecycle |
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

A marker being on a scanned node does not prove that it is inside the playable
combat volume. Check wall containment before grounding or navigation. Likewise,
a tree passing mesh/submesh/material closure checks does not prove that its
crown reads correctly through the normal player camera; replace a visually bare
family with an audited complete native family in the map bundle.
