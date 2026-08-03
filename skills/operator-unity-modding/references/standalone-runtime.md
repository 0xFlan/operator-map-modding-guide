# Standalone runtime reference

## Current method

Use one data-only map package, one map-independent **OPERATOR: Modded
Operations — Standalone Map Framework**, one real package-owned scene, and an
optional separately installed exact-scene companion.

The retired MapBridge method overlays a prefab in a retail scene. It does not
provide standalone mission parity.

## Required order

```text
freeze verified catalog
-> bind private mission UI
-> verify and load dependencies
-> verify and load exact scene
-> run exact-scene companion reconstruction
-> pass strict world contract
-> create native-compatible mode owner
-> complete shipped readiness
-> install current-scene spawns
-> create players
-> create mode-correct server PVE actors
```

## Companion world contract

Require exact package, map, operation, scene, and build identity. Require
native materials, usable terrain and collision, one intended graph, every
mission marker inside the wall, every required marker tightly grounded and on
graph, and no missing critical asset.

Fail closed. A scene-loaded event is not a world-ready signal.

## PVE owner

Standalone PVE needs an `InfiltrationManager`-compatible owner,
`InfiltrationManager.instance`, `GameMode.singleton`, and synchronized
`RaidTimer`. The persistent shipped `GameManagerNetwork` owns Mission Failed
UI and Restart Operation.

Keep this bridge in the generic framework. Keep map reconstruction in the
companion.

## Restart

Invalidate the old scene generation. Stop population. Clear current-scene
spawns and mode state. Remove companion graphs, objects, materials, native
data, and callbacks. Unload the scene. Release bundles in reverse order. Build
one fresh generation.

Record normal Restart Operation, death/respawn, and KIA/end-screen Restart as
separate claims.
