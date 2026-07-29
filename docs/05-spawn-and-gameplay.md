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

## Mandatory tests

Run from a normal settled user session:

- first spawn on every team;
- several free-for-all spawns if supported;
- at least one death/respawn;
- terrain raycast/grounded evidence at each test;
- player/camera position after the network client settles;
- collision, route, and boundary checks.

A force-scene visual harness is useful for material diagnosis, but it is not a
substitute for real game-mode spawning.
