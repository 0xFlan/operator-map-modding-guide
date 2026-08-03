# 11. AI navigation, routes, and behavior

Status: `SUPPORTED` for a current-build map-owned A* `GridGraph` and generic
native PVE actor flow. Re-check the installed A* and game interop after each
update.

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
4. Require `astar.data`.
5. Remove only a previous graph that the companion owns.
6. Add a graph with
   `astar.data.AddGraph(Il2CppType.Of<GridGraph>())`.
7. Convert the wrapper with `TryCast<GridGraph>()`.
8. Set center, rotation, aspect, node size, width, and depth.
9. Call `GridGraph.SetDimensions(width, depth, nodeSize)`.
10. Configure slope, step, erosion, neighbors, corner policy, height sampling,
    ground requirement, and obstacle collision.
11. Call `astar.Scan(graph)` for the exact graph.
12. Record graph ownership, center, dimensions, node size, and scan result.

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

The map companion MUST NOT create actors. The generic framework MUST NOT
contain a map name, map coordinate, graph size, or hard-coded population for
one map.

PVP operations omit enemy range fields. PVP MUST have zero PVE actors before
and after restart.

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
