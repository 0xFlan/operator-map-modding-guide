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
-> capture exact player-owned laptop and player at Confirm
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

## Confirm lifetime

Package I/O can outlive the Confirm frame. Keep the exact owned
`MissionLaptop` and its `PlayerNetworking` in the pending launch record. Keep
the modal visible and disabled. Restore only the captured field when the same
laptop released it. Close the modal and call the native board start in the
same final frame.

Treat this correction as `PROVEN-STATIC` until one physical first Confirm
launches without a second laptop interaction.

## Selected-map bundle prefetch

Start one selected-map prefetch after the immutable row selection. Keep
manifest dependency order. Keep the content ID with the cache. Confirm MUST
attach to that request or use its completed cache. Do not prefetch all maps.

Log each file name, byte count, request time, total time, and remaining Confirm
wait. An active `AssetBundleCreateRequest` has no safe cancellation contract.
Finish the one bounded request. Prefetch moves cold I/O earlier; it does not
remove the bytes.

Treat this method as `PROVEN-STATIC` until a physical row-to-Confirm run
records the expected timings and launches once.

## Host player spawn

On the current exact build, `PlayerMaster.SpawnPlayer()` and
`SpawnPlayerServer()` both enter the Mirror command sender. The generated
server method is `PlayerMaster.UserCode_CMDSpawnPlayer__NetworkIdentity`. It
selects a shipped spawn point, instantiates the shipped player prefab, calls
owner-aware `NetworkServer.Spawn`, assigns the spawned-player object, and sends
the retail spawn RPC.

After the server readiness barrier, a host adapter can enter that exact
generated server method when `NetworkServer.active`. Pass the `PlayerMaster`
network identity. Keep the normal command path for a non-server owned client.
Do not create an independent player prefab and do not call lifecycle methods.

Require a non-null `PlayerSpawnedObject`, the shipped
`GameManager.MovePlayerToSpawn` path, and player-camera proof above the package
terrain. Treat this route as `PROVEN-STATIC` until movement, camera, and
reciprocal combat pass in a physical run. Test remote clients separately.

## Native AI population

Filter `GameManager.AllAITypes` for root `BrainAI`, root `NetworkIdentity`,
`WeaponsAI.SpawnWeapon=true`, and a non-empty `weaponList`. Give those prefabs
and the package-valid markers to a scene-owned `RaidManager`. Call
`RaidManager.ServerSpawnAI(false)`. Preserve its current shipped order:
owner-aware network spawn first, then `BotSpawnDetails`.

Treat this correction as `PROVEN-STATIC` until reciprocal firearm damage works
in a physical PVE run. A grenade result is not sufficient.
