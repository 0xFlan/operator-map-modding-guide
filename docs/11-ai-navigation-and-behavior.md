# 11. AI navigation, routes, and behavior

Status: `SUPPORTED` for a current-build map-owned A* `GridGraph` and generic
native PVE actor flow. Re-check the installed A* and game interop after each
update.

The owner-retention path and Forest search profile are `PROVEN-RUNTIME` for
the current single-player first launch and native restart. Reciprocal firearm
damage remains a separate acceptance gate.

## Use the native AI stack

OPERATOR bots use the A* Pathfinding Project for map traversal. Unity NavMesh
data alone is not sufficient for this path.

The map owns geometry, collision, boundaries, marker placement, and route
shape. The exact-scene companion owns map-specific graph construction and
marker validation. The generic framework owns native actor creation after
readiness. Native `BrainAI`, `RaidManager`, perception, weapons, cover search,
and off-mesh traversal own behavior.

Do not add a second custom bot brain to repair a missing graph.

## Define three different bounds

Do not use one rectangle for all map purposes.

| Bound | Purpose |
| --- | --- |
| Playable physics bound | Player and AI collision containment |
| Bullet-interaction bound | Reciprocal combat and line-of-fire containment |
| Visual terrain/scenery apron | Render continuity outside the wall |

The navigation graph MUST use the authoritative playable physics and
bullet-interaction volume. It MUST NOT use the larger visual apron.

Keep enemy and HVT markers inside the gameplay wall with explicit clearance.
Check containment before graph lookup. A node behind a bullet barrier is not
a valid spawn.

## Build one live A* graph

Use this current-build sequence:

1. Call `AstarPath.FindAstarPath()`.
2. Reuse a service only when exact-scene ownership is compatible.
3. Otherwise create one map-scoped host and add `AstarPath`.
4. Resolve or add one enabled `Pathfinding.RVO.RVOSimulator` on the same
   host. Require `RVOSimulator.active`.
5. Require `astar.data`.
6. Remove only a previous graph that the companion owns.
7. Add a graph with
   `astar.data.AddGraph(Il2CppType.Of<GridGraph>())`.
8. Convert the wrapper with `TryCast<GridGraph>()`.
9. Set center, rotation, aspect, node size, width, and depth.
10. Call `GridGraph.SetDimensions(width, depth, nodeSize)`.
11. Configure slope, step, erosion, neighbors, corner policy, height sampling,
    ground requirement, and obstacle collision.
12. Call `astar.Scan(graph)` for the exact graph.
13. Record A*, RVO, graph ownership, center, dimensions, node size, and scan
    result.

Vanilla `level16` stores `AstarPath` and an enabled `RVOSimulator` on its
`Astar Navmesh` GameObject. The shipped `BOT V2` uses `FollowerEntity`, so a
valid graph without RVO can still leave every bot stationary.

Measure all values from the map traversal contract. Do not copy graph
dimensions from a visually similar map.

## Temporary terrain scan layer

Use a temporary layer only when the installed layer contract cannot be
authored portably.

```text
save Terrain.layer
-> select one private temporary layer
-> include it in graph height sampling
-> exclude it from obstacle collision
-> move only Terrain to that layer
-> call Physics.SyncTransforms
-> scan the graph
-> finally restore Terrain.layer
-> call Physics.SyncTransforms again
```

The `finally` restoration is mandatory. Do not move the complete map root.
Do not let a temporary scan mask remain active during gameplay.

## Validate every mission marker class

Apply the same contract to ordinary enemies, HVTs, bosses, reinforcements, and
all other roles that the operation can create.

For each marker:

1. Check the authored position against the playable volume and wall clearance.
2. Quarantine an outside marker before generic discovery can use it.
3. Find the nearest walkable node with the installed A* constraint.
4. Reject a missing node.
5. Measure horizontal and vertical correction separately.
6. Reject a horizontal correction greater than the map limit.
7. Move the marker to the node center plus the intended foot clearance.
8. Call `Physics.SyncTransforms`.
9. Use a short local ground test near the marker foot point.
10. Require `AstarPath.IsPointOnNavmesh(marker.position)`.
11. Log the marker name, original point, final point, correction, ground
    delta, and graph result.

Do not use a long downward ray as grounding proof. It can hit terrain many
meters below an actor that will fall at mission start.

If a required marker fails, fail the world contract. Do not silently create an
actor at the nearest distant node.

## PVE population

The package declares `minEnemies` and `maxEnemies`. The framework validates
the range and the valid-marker count. The server selects an inclusive
deterministic count from sorted markers.

Use OPERATOR's shipped population owner. In the current inspected build,
`RaidManager.ServerSpawnAI(false)` selects `standardAI`, instantiates the bot,
calls `NetworkServer.Spawn(bot, GameManager.instance.gameObject)`, and then
applies `BotSpawnDetails`. Preserve this order and owner argument. Do not
replace it with one-argument `NetworkServer.Spawn(bot)`.

Before the call, restrict `standardAI` to registered prefabs that have root
`BrainAI`, root `NetworkIdentity`, `WeaponsAI.SpawnWeapon=true`, and a
non-empty `WeaponsAI.weaponList`. A grenade result does not prove the firearm
lifecycle. Require reciprocal firearm damage in the physical runtime test.

The map companion MUST NOT create actors. The generic framework MUST NOT
contain a map name, map coordinate, graph size, or hard-coded population for
one map.

PVP operations omit enemy range fields. PVP MUST have zero PVE actors before
and after restart.

## Optional schema-v2 PVE AI profile

A schema-v2 PVE operation can own one fixed `pveAiProfile`. Schema v1 rejects
the object. PVP rejects it. The object has no UI and does not change another
operation.

```json
"pveAiProfile": {
  "id": "woodland-balanced-v1",
  "detectionRangeMeters": 45.0,
  "fieldOfViewDegrees": 90.0,
  "maximumEffectiveRangeMeters": -1.0,
  "wanderDistanceMeters": 38,
  "useComms": true,
  "counterSuppression": false
}
```

Measure the authoritative playable rectangle and every accepted player-to-
enemy marker gap. Report minimum, median, mean, maximum, and the selected solo
spawn gap. Do not tune from the visual TerrainData apron. A good initial
detection range is less than the minimum start gap and large enough to create
contact inside intended routes. Validate the result in the physical camera;
geometry is a tuning input, not final proof.

The current `RaidManager.ApplyBotSpawnSettings` copies detection range, FOV,
communications, counter-suppression, effective range except `-1`, wander
distance except `-1`, and `BotSpawnDetails.idleState` to the native bot. The
last field is essential: it writes marker offset `0x20` to
`BrainAI.idleStates` offset `0x2D4`. Native `BrainAI.UpdateStateMachine` calls
`Wander(dt)` only when `CurrentState` is `Idle` and `idleStates` is `Wander`.
A nonzero radius with the default `Idle` substate produces no movement. Set
the substate only for the profiled PVE operation. It does not consume marker
`DetectionTimeMultiplier` or `HearingRange` in the pinned build. Use `-1` for
maximum effective range when the map must preserve the selected native AI
prefab's value.

`BrainAI.Wander` waits for the prefab's `WanderTimer * Patience`. It then
selects the equivalent of
`RandomNavSphere(currentPosition, 5, WanderDistance)`. It resets the timer and
later repeats from the new position. Preserve the timer and patience unless
separate vanilla evidence requires a change. To create a gradual search, keep
one wander radius below the distance that would cross the intended encounter
midpoint from the nearest starting marker.

## Foliage and native sight layers

A shorter range does not make a bush opaque. Inspect how the same installed
vanilla prefab participates in `EyesAI.TestIfCanSeeAtHeight`. In the pinned
build, `EyesAI` uses a physics linecast. Its mask includes layer 18,
`AI_VisionBlock`.

If the original prefab contains an authored inactive `AI Collider` trigger on
that layer, an exact-scene companion can activate that child. Require its
exact name, layer, collider type, authored count, and active count. Keep it a
trigger. Verify the collision matrix and bullet mask before use. Do not add an
invisible movement or projectile wall. Do not enable the contract globally.

Ukrainian Forest is the worked example. It contains 118 direct and 156
perimeter bushes. Its deterministic Barberry 2, Barberry 3, Juniper cycle
produces 79 direct plus 104 perimeter native barberry blockers, for 183 total.
The remaining 91 Junipers have no native inactive `AI Collider` child. Do not
add a synthetic blocker to them. Its playable volume is 70 by
140 m. The nearest solo enemy gap is 78.87 m. Its 38 m wander radius is less
than half of that gap, 39.44 m. Its fixed 45 m range and 90-degree FOV are a
map-owned PVE profile. The PVP operation omits it.

Log the profile ID and every applied value. Log authored and active sight-
blocker counts and the exact layer. The first launch and same-process native
restart are `PROVEN-RUNTIME` for search timing, displacement, movement toward
insertion, and authored foliage obstruction. Reciprocal firearm damage remains
a separate gate.

Modded Operations `0.3.20` starts one bounded read-only diagnostic when a PVE
operation has `pveAiProfile`. It does not use a map ID. It records only the
new `BrainAI` instances added by the package's native
`RaidManager.ServerSpawnAI(false)` call. It reports the live
`WanderTimer * Patience`, detection range, FOV, wander distance, the live
`idleWander` count, and communications state. It then reports movement and same-mask sight probes at
0, 10, 30, 60, 90, and 120 seconds.

The final release-byte stationary observer accepted both complete windows:

| Generation | Live AI | Native delay | Moved at least 1 m at 120 s | Moved at least 5 m toward insertion | Mean displacement | Maximum displacement |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| First launch | 15 | `9.31..36.78 s` | 15 | 6 | `21.82 m` | `51.19 m` |
| Native restart | 14 | `10.97..33.02 s` | 14 | 4 | `16.96 m` | `49.34 m` |

Both generations recorded authored layer-18 vegetation first hits.
`actualSeenTarget` stayed zero in all 12 snapshots. Keep the target-state
limits below: this result is not a general firearm or acquisition proof.

Require these two log prefixes:

```text
Profiled PVE native AI contract:
Profiled PVE AI snapshot:
```

The snapshot uses the live bot's `EyesAI.DetectionLayerMask`. A layer-18 first
hit is vegetation geometry evidence. `actualSeenTarget` comes from
`BrainAI.CurrentSeenTarget`. Do not treat the synthetic linecast by itself as
proof that the bot acquired or forgot the player. Do not treat movement alone
as proof of a believable route. Review the physical camera and test firearms
in both directions.

`CurrentSeenTarget` can refer to any native target known to the bot. A
non-zero `actualSeenTarget` does not prove that the target is the local player.
Correlate it with the bot-to-player same-mask probe, player distance, state,
and physical reciprocal-firearm behavior. A strict zero-target assertion can
reject a valid restart because it measures all native targets, not only the
player.

The shipped `BOT V2` hierarchy keeps `BrainAI` and `NetworkIdentity` on the
network root. Its `SK_Insurgent_P8` child keeps `AgentController` and the
enabled `FollowerEntity`. The root can remain still while the native entity
moves. Measure displacement from `brain.agent.position`; do not use
`brain.transform.position`.

## Route authoring

A walkable graph does not prove a good route. Author and test these elements:

- at least two approaches to important combat areas when the layout permits;
- enough width for the native agent radius and avoidance behavior;
- slope and step values below the measured actor limits;
- no collider lip at terrain-to-structure transitions;
- no foliage or decorative collider that closes a route;
- clear line of sight where combat is intended;
- hard cover that blocks actual projectiles;
- no spawn pocket behind a one-way bullet barrier;
- no graph connection through a visual-only exterior area;
- no isolated graph island that contains a spawn marker.

Use deterministic route probes. Record start, goal, path result, node count,
path length, and first failed segment.

## Cover behavior

Native dynamic cover search uses scene collision at runtime. A custom map does
not need a second baked cover-point system for the supported path.

Provide complete colliders on real cover. Verify projectile blocking from both
sides. Keep enough clearance for stance changes and movement around the cover.
Do not mark a thin decorative card as cover.

## Doors and off-mesh traversal

Doors need native A* link and navigation-cut relationships. Native bot
traversal uses walk, open, and breach modes. The native
`BotOffmeshLinkHandler` controls door traversal and per-door cooldown state.

See [`DoorV2` wiring](09-interactive-prefabs-and-doorsv2.md). A player-usable
door is not automatically AI-usable.

## Reciprocal combat test

Test combat across representative routes.

1. Put the player and one AI inside the gameplay volume.
2. Confirm both actors are on the live graph and grounded.
3. Test player bullets against AI across the route.
4. Test AI bullets against the player across the same route.
5. Repeat through an open door, near a wall, near a boundary, and around cover.
6. Confirm that invisible collision and bullet masks agree.

If actors can see each other but bullets cannot cross, inspect the gameplay
barrier and layer masks. Do not move the actors outside the wall to avoid the
barrier.

## Restart and teardown

On scene unload or restart, the companion MUST:

1. Invalidate the old scene generation.
2. Remove the graph through the owning `astar.data.RemoveGraph` path.
3. Destroy the `AstarPath` host only when the companion created it.
4. Clear marker-validation state.
5. Unsubscribe map-owned callbacks.
6. Ignore stale asynchronous work.
7. Build exactly one new graph for the new scene generation.

After restart, record one service, one map-owned graph, one marker-validation
generation, and the expected actor count.

## Minimum evidence

Require these results for PVE:

- all marker classes inside the gameplay wall before navigation lookup;
- all required markers grounded within the local tolerance;
- all required markers on the one live graph;
- selected actor count inside the package range;
- all actors inside the gameplay and bullet-interaction volume;
- no vertical fall at creation;
- successful routes between representative encounter areas;
- reciprocal player and AI combat;
- native door open/breach behavior when doors are present;
- the same results after normal restart;
- separately recorded death/respawn and KIA/end-screen restart results.

Do not infer any row from another row.
