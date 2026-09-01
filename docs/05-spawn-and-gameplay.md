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
player on request 1 so `ClientSpawnBS` runs. Record the attempt before native
entry. If a repeat additive-scene host still has no new
`PlayerSpawnedObject` after 300 frames, invoke the exact generated
`UserCode_CMDSpawnPlayer__NetworkIdentity` body for that owned host as a
bounded recovery. Use that body on request 1 only for an unowned server
player. Stop after three total requests or after one player object or alive
state proves success. An owned host stops after two total requests so the
generated-body recovery can run only once. Store request counters on the active scene-generation
owner. Do not reuse them after unload.

When the owned `PlayerNetworking` object exists, use the shipped
`GameManager.MovePlayerToSpawn(position, rotation)` coroutine. Moving only the
network root can leave the first-person camera or controller at the old
armory position.

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

## Publish one exact standalone scene generation

A map companion that shares legacy and standalone spawn hooks MUST publish the
standalone scene at the same readiness boundary as the generic loader. A
loaded scene name alone is not sufficient.

The current Forest companion uses this exact sequence:

1. `OnSceneLoaded` calls
   `ResetStandalonePlayerHandoffState(clearDestinationScene:false)`, records
   the new scene handle, records `mapDestinationScene`, and keeps
   `applied=false`.
2. `ProcessStandalonePackageScene` resolves the exact world root and waits
   until `HasReadyStandaloneTerrain` proves that one `Terrain` and one
   `TerrainCollider` share the same non-null `TerrainData`.
3. It repairs tree/material/navigation state, sets the current scene and
   `applied=true`, opens one bounded `SpawnSafetyFrameWindow`, and inspects the
   local player roots once.
4. `GroundLateNetworkPlayerObjectInstance` resolves the current standalone
   root when a late `PlayerMaster` or `PlayerNetworking` callback arrives. It
   uses a Forest marker only when the exact destination scene is loaded and
   `HasReadyStandaloneTerrain` passes. If this is the standalone scene and
   that gate does not pass, the method returns without moving the player. It
   MUST NOT use the legacy Office pre-map fallback for a standalone package.
5. During the bounded initial window, a known local root is invalid when it
   is at the old sky pose or more than `2 m` below the sampled marker surface.
   The repair uses the shipped move path, clears fall velocity, publishes the
   Smooth Sync teleport, and holds only that exact root briefly.
6. `OnSceneUnloaded` calls
   `ResetStandalonePlayerHandoffState(clearDestinationScene:true)` before it
   clears the scene handle. This removes the controller/transform hold,
   counters, safety frames, pre-map support, `applied` flag, and destination
   scene reference.

The unload step is mandatory because the local player can survive an additive
operation scene. A transform hold that retains the old map position can pin
that persistent player in the armory hallway. The ready flag must also reset
per generation; otherwise a repeat launch can choose a prior fallback pose
and put the camera below the new terrain.

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
exposes that closed range only on its private cloned modded-PVE briefing through
the native enemy-count control. Confirm captures the displayed integer, and the
loaded scene MUST have at least that many navigation-valid ordinary markers in
a stable name-ordered, pairwise-separated subset. Inactive utility markers may
remain eligible; active count is telemetry, not capacity. PVP operations omit
both fields. Never mutate Tier 1, shipped operation arrays, vanilla enemy
ranges, or PVP. Do not hard-code one map's count or marker coordinates in the
generic adapter.

The declared range is the map's complete entitlement. If a release claims a
maximum of 60, set `maxEnemies` to 60, provide at least 60 navigation-safe
candidates, reject any selected value above 60, and verify one host-only native
population call. A higher framework safety ceiling does not increase the
package maximum.

## Team PVP identity

Do not assume that native team IDs are zero-based. In the current exact build,
`PvpGameode.StartNewRound()` sets Team 1 spawn points to `Team=1` and Team 2
spawn points to `Team=2`. `GameManager.nextSpawnPosition` compares that value
directly with `PlayerMaster.MyTeamIdentifier.TeamID`.

Use `TeamID`. Do not use `ToString()`. Create two independent native lists:

```csharp
pvp.Team1SpawnPoints = team1;
pvp.Team2SpawnPoints = team2;
```

Do not append both teams to one list and depend on a later filter. Remove a
cached marker assignment when the player's current team does not match that
marker.

The mode owner MUST derive from `PvpGameode`. It MUST call the shipped
`Server_AllPlayersLoaded` body. That body resolves the native Team 1 and Team
2 player caches and starts the shipped respawn coroutine. After the framework
records `nativePvpLifecycle=true`, it MUST stop any generic repeated move.
Otherwise, a framework correction can fight the retail round respawn.

For Ukrainian Forest, these prefixes and counts are the exact contract:

| Prefix | Count | Side |
| --- | ---: | --- |
| `Team1_Spawn_*` plus `Team1_Backup_Spawn_*` | `10` | same side as PVE players |
| `Team2_Spawn_*` plus `Team2_Backup_Spawn_*` | `10` | same side as PVE enemy pockets |

Treat this mapping as `PROVEN-STATIC` until a host and remote client prove
first spawn, freeze-time release, death, round scoring, and respawn on
opposite teams. See
[Native mode ownership, PVE, and StandardPVP](03c-native-mode-ownership-and-pvp.md).

## Separate the 2D infiltration marker from 3D spawns

`operations[].infiltrations[].mapPositionX/Y` places a selectable marker on
the mission preview. It never changes a scene `Transform`. Use scene marker
objects for 3D player and AI locations.

For one operation, verify all three layers independently:

1. The manifest infiltration label and normalized anchors are correct.
2. The selected `SPAWN_SET_...` scene identity exists.
3. The current-scene player markers raycast to package-owned walkable
   collision and have the expected one-based PVP team IDs.

A correct briefing marker does not prove a safe player spawn.

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
