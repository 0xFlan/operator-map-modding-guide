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
-> PVE: create players and mode-correct server actors
-> PVP: let shipped PvpGameode own respawn and round placement
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

Invalidate the old scene generation. Stop population. Restore captured
process-global spawn state only while the operation still owns it. Restore the
captured NVG color and destroy run-time Volume profiles. Clear mode state.
Remove companion graphs, objects, materials, native data, and callbacks.
Unload the scene. Release bundles in reverse order. Build one fresh
generation.

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

On the current exact build, capture `GameManager.SpawnPointsInScene`,
`GameManager.Pspawns`, and the next index. Install operation-owned current
scene objects and set the first index to `0`. Native
`GameManager.nextSpawnPosition` at RVA `0x00EF2CA0` reads the current index
before incrementing it. `-1` is invalid.

Call `PlayerMaster.SpawnPlayer()` for the owned player. It performs ownership
checks, enters the Mirror command, and calls `ClientSpawnBS`. The generated
server method is `PlayerMaster.UserCode_CMDSpawnPlayer__NetworkIdentity`. It
selects a shipped spawn point, instantiates the shipped player prefab, calls
owner-aware `NetworkServer.Spawn`, assigns the spawned-player object, and
sends the retail spawn RPC. Enter that generated body directly only for an
unowned server player.

Record request frame and count before native entry. Limit retries. Stop after
the spawned object or alive state proves completion. Do not create an
independent player prefab and do not call lifecycle methods.

Require a non-null `PlayerSpawnedObject`, the shipped
`GameManager.MovePlayerToSpawn` path, and player-camera proof above the package
terrain. Treat this route as `PROVEN-STATIC` until movement, camera, and
reciprocal combat pass in a physical run. Test remote clients separately.

## PVP team spawn

On the current exact build, native PVP team IDs are one-based.
`PvpGameode.StartNewRound()` assigns Team 1 spawn points to `Team=1` and Team
2 spawn points to `Team=2`. `GameManager.nextSpawnPosition` compares that
value directly with `PlayerMaster.MyTeamIdentifier.TeamID`.

Assign `SpawnPoint.Team=1` to Team 1 markers and `SpawnPoint.Team=2` to Team 2
markers. Read `TeamID`. Do not use `0/1` or `ToString()`. Invalidate a cached
marker after a team change when it does not match the current team.

Create `StandalonePvpGameMode : PvpGameode`. Assign separate non-empty native
Team 1 and Team 2 spawn lists. Use the exact current-build scalar seeds
`MaxRounds=13`, `RoundsToWin=7`, and `RoundTime=120`. Call the shipped
`PvpGameode.OnStartClient` and `Server_AllPlayersLoaded` bodies. Keep the
shipped respawn, freeze, death, score, round, and end-operation bodies.

Supply every reference that the native PVP hooks read: two audio sources, 16
non-empty clip arrays with retail lengths, `TeleType`, timer and score text,
six outcome roots, two animators, exact `FadeOut` and `FadeIn` strings, and
win/lose/tie text. A non-null silent clip is valid when retail audio cannot be
distributed.

Stop generic repeated movement after native PVP is active. Clear
`PvpGameode.instance` during teardown when the operation still owns it.

This contract is `PROVEN-STATIC`. Require a host and remote client on
different teams. Prove first spawn, freeze release, death, score, round
respawn on each team, correct side and facing, zero PVE AI, Restart Operation,
and return-to-armory.

## Native AI population

Filter `GameManager.AllAITypes` for root `BrainAI`, root `NetworkIdentity`,
`WeaponsAI.SpawnWeapon=true`, and a non-empty `weaponList`. Give those prefabs
and the package-valid markers to a scene-owned `RaidManager`. Call
`RaidManager.ServerSpawnAI(false)`. Preserve its current shipped order:
owner-aware network spawn first, then `BotSpawnDetails`.

Treat this correction as `PROVEN-STATIC` until reciprocal firearm damage works
in a physical PVE run. A grenade result is not sufficient.

## Exact 02:00 and terrain presentation

For the current worked source, use `sharedassets7.assets` profile path `435`,
`PVP map NIight VOLUME`. Its Exposure path `440` uses Automatic Histogram,
compensation `1.16`, limits `5.065281867980957..9.348570823669434`, and speeds
`3/3`. Tonemapping path `432` uses ACES without the day external LUT.
`GameManager.SetNVGColor(0)` selects white phosphor on this build.

After one reconstructed `TerrainData` owns `Terrain` and `TerrainCollider`,
disable the exact serialized mesh fallback before spawn validation. Align a
complete tree from its lowest finite renderer bound after final yaw and scale,
not from its source pivot. Use a declared small embed and correction limit.
