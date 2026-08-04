# 5. Spawns and gameplay

## Treat spawns as a runtime behavior

Moving visible spawn markers is rarely enough. A game mode can cache markers
before a plugin callback, choose a transform through another selector, move
the avatar through a separate method, or overwrite a correction after network
ownership arrives.

Trace the full path:

    scene load -> game-mode setup -> spawn selection -> avatar creation
    -> network/client ownership -> final movement -> respawn

Patch or configure only the narrowest responsible seams, and make every
intervention target-scene-specific and idempotent.

## Own and restore native spawn globals

`GameManager.SpawnPointsInScene`, `GameManager.Pspawns`, and the next-spawn
index are process-global state. A standalone scene MUST NOT leave its scene
objects in those fields after unload.

Use this contract:

1. capture all prior values before replacement;
2. install only current-scene `SpawnPoint` objects;
3. set the first index to `0` on the current exact build;
4. record the operation-owned list and array identities;
5. restore a field only when it still equals the operation-owned identity;
6. reject a prior list or array when one member belongs to an unloaded scene;
7. clear all captured and owned references after restoration.

Restore before the package scene destroys its objects. Apply the same release
path on plug-in unload, scene replacement, normal operation unload, and
restart.

For player creation, call native `PlayerMaster.SpawnPlayer()` for the owned
player so `ClientSpawnBS` runs. Use the generated server body only for an
unowned server player. Record an attempt before native entry, bound retries,
and stop after one player object or alive state proves success.

## Large-bundle race

If the source map contains high or off-map spawn locations, the local player
can be created before the replacement ground exists. A post-load teleport
cannot make that clean first spawn safe.

The general solution is:

1. before the bundle begins loading, map source markers to deterministic
   grounded replacement coordinates;
2. supply temporary non-rendering support collision only if it is proven
   necessary;
3. bind the real ground before removing temporary support;
4. reapply the verified marker mapping at relevant game-mode spawn phases;
5. use a bounded local-owner-only repair for late-created network player
   objects, including velocity clear and replication-aware teleport behavior.

Never use a broad repair to move remote players, bots, or every object above
an arbitrary Y threshold.

## AI markers require both ground and navigation

An authored transform is not a usable AI spawn merely because a ray eventually
finds terrain below it. Before generic PVE creation runs:

- define the authoritative playable physics and bullet-interaction volume;
- reject or quarantine every AI marker outside that volume before navigation
  lookup;
- require one scanned graph that covers only the playable region, not a larger
  render-only terrain or scenery apron;
- snap every enemy, HVT, and other operation-consumed AI marker to a nearest
  walkable node within an explicit correction limit;
- place it at only the intended foot clearance above that node;
- verify ground within a tight post-snap tolerance;
- verify `AstarPath.IsPointOnNavmesh` for every marker class;
- repeat the same assertions after Restart Operation.

If any marker is rejected, fail the map contract instead of spawning an actor
that will fall or initialize off graph.

Graph coverage by itself is insufficient. A walkable node can exist outside a
gameplay wall when the graph was sized from a larger visual terrain. That
failure spawns enemies behind the barrier where actors and bullets cannot
interact. Marker placement is map/package data. Exact playable bounds and
runtime graph construction belong to the map companion. OPERATOR: Modded
Operations MUST remain map-independent.

## PVE population range

Declare `minEnemies` and `maxEnemies` on each PVE operation. The generic host
selects an inclusive deterministic count from that range and MUST have at least
`minEnemies` valid scene markers. PVP operations omit both fields. Do not
hard-code one map's count or marker coordinates in the generic adapter.

## Team PVP identity

Do not assume that native team IDs are zero-based. In the current exact build,
`PvpGameode.StartNewRound()` sets Team 1 spawn points to `Team=1` and Team 2
spawn points to `Team=2`. `GameManager.nextSpawnPosition` compares that value
directly with `PlayerMaster.MyTeamIdentifier.TeamID`.

Use `TeamID`. Do not use `ToString()`. Remove a cached marker assignment when
the player's current team does not match that marker. Treat this mapping as
`PROVEN-STATIC` until a host and client prove first spawn and respawn on
opposite teams.

## Mandatory tests

Run from a normal settled user session:

- first spawn on every team;
- one host and one client on different teams, with each player on the authored
  side for that team;
- one death/respawn on each PVP team and one team-switch check when the mode
  supports team switching;
- several free-for-all spawns if supported;
- at least one death/respawn;
- normal alive Restart Operation;
- death/KIA end-screen Restart Operation as a separately recorded result;
- PVE AI grounding and graph coverage before and after restart;
- PVE AI count within the package range and every actor inside the playable
  bullet-interaction volume;
- reciprocal player/AI line-of-fire testing across representative routes;
- terrain raycast/grounded evidence at each test;
- player/camera position after the network client settles;
- collision, route, and boundary checks.

A force-scene visual harness is useful for material diagnosis, but it is not a
substitute for real game-mode spawning.
