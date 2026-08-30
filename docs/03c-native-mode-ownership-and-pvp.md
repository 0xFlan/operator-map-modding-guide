# 3c. Native mode ownership, PVE, and StandardPVP

Status: `PROVEN-STATIC` for OPERATOR Steam build `24091246`, Modded Operations
`0.3.31`, and bundled-only Operator Mod API `0.2.0-alpha.7`. Protocol v6 covers
PVP and online PVE in source, but exact two-process transport, movement,
combat, restart, failure/return, and teardown must still pass each physical
release matrix.

Use this chapter to decide which data belongs in a map scene and which data
belongs in OPERATOR: Modded Operations. It also gives the exact current-build
StandardPVP contract.

## 1. Use the retail mode owner

Do not implement PVP with a plain `GameMode` subclass. The current build maps
the operation modes as follows:

| Manifest mode | Retail owner | Map data that the owner consumes |
| --- | --- | --- |
| `pve` | `InfiltrationManager` plus `RaidManager` | player spawn markers, bot markers, HVT markers, objective/exfil data, navigation |
| `pvp` | `PvpGameode` | Team 1 `SpawnPoint` list, Team 2 `SpawnPoint` list, team identifiers, round settings |
| FFA, when a future schema enables it | `FFA` | FFA `SpawnPoint` list |
| wave survival, when a future schema enables it | `WaveSurvival` | player markers and wave definitions |

The shipped type name is `PvpGameode`. The missing `m` is part of the binary
contract. `PVP` and `Mirror_TeamDeathmatch` are not the retail round owner.

The current framework source is:

```text
<author-workspace>/source/runtime_native_tab_fix/CerberusNativeTabFix.cs
```

The important declaration is:

```csharp
private sealed class StandalonePvpGameMode : PvpGameode
```

Its `Server_AllPlayersLoaded` override calls the retail body:

```csharp
base.Server_AllPlayersLoaded();
```

The framework does not replace `StartNewRound`, `PlayerDied`, `EndRound`,
`RespawnPlayers`, the score SyncVars, or the freeze timer.

## 2. Keep the correct ownership boundary

| System | Own it in the map package | Own it in the framework | Retail game remains owner |
| --- | --- | --- | --- |
| terrain and collision | yes | no | physics execution |
| Team 1 and Team 2 coordinates | yes | no | spawn selection after list wiring |
| PVE enemy coordinates | yes | no | AI actor creation after marker transfer |
| operation IDs and visible text | manifest | catalog parser and UI adapter | shipped UI visuals |
| `PvpGameode` network component | no | yes | native method bodies |
| PVP team-player caches | no | list initialization only | cache population from retail teams |
| PVP scoring and rounds | no | no | yes |
| generic manifest terrain reconstruction | Modded Operations | yes | shared render/collision `TerrainData` before player spawn |
| map-only terrain-material and prop correction | optional companion | no | exact-map renderer and placement state after generic bind |
| process-global spawn list teardown | no | yes | prior retail owner after restore |

This split is close to the retail scene contract. A retail map scene owns
spatial references. A retail mode component owns mode state. The standalone
framework connects the two because an external AssetBundle cannot safely ship
an editor-authored `Assembly-CSharp` network component.

## 3. Author PVP markers in the scene

Create these inactive direct children under the selected map root:

```text
Team1_Spawn_00
Team1_Spawn_01
...
Team2_Spawn_00
Team2_Spawn_01
...
```

The framework also recognizes:

```text
Team1_Backup_Spawn_*
Team2_Backup_Spawn_*
PVP_Team1Spawn_*
PVP_Team2Spawn_*
```

Discovery is mode-isolated. PVE ignores PVP-prefixed and Team 2 markers. PVP
ignores `PVE_PlayerSpawn_` markers. Shared `Team1_Spawn_` and
`Team1_Backup_Spawn_` aliases can remain valid for either mode; explicit PVE
and PVP prefixes belong only to their own mode.

The first team uses integer team ID `1`. The second team uses integer team ID
`2`. Do not use `0` and `1`. `PlayerMaster` passes
`MyTeamIdentifier.TeamID` to the retail spawn selector without converting it.

Place each marker on valid walkable collision. Use a small surface clearance,
such as `0.03 m`. Do not put a marker at a fixed world Y value when the terrain
height changes. Sample the final collision surface during the build.

For Ukrainian Forest, the exact builder call is:

```csharp
AddSpawnCluster(root.transform, "Team1", 0f, 7f, 0f);
AddSpawnCluster(root.transform, "Team1_Backup", -7f, 12f, 8f);
AddSpawnCluster(root.transform, "Team2", 0f, MapLength - 7f, 180f);
AddSpawnCluster(root.transform, "Team2_Backup", 7f, MapLength - 12f, 188f);
```

The source file is:

```text
<author-workspace>/source/runtime_bundle_project/Assets/Editor/BuildHillyUkrainianForestBundle.cs
```

The current build creates exactly ten markers for each team. Team 1 uses the
same side as PVE players. Team 2 uses the same side as the PVE enemy pockets.

Before PVP launch, the framework requires at least
`ceil(maxPlayers / 2)` valid markers for each team. The current vanilla lobby
exposes at most 12 players, so a map that declares 12 must provide at least six
accepted Team 1 markers and six accepted Team 2 markers. Extra valid markers
are permitted. Reinspect the lobby limit after a game update.

## 4. Add the PVP spawn-set identity

Add this inactive scene object:

```text
SPAWN_SET_<spawn-set-id>
```

The Forest PVP record uses:

```text
SPAWN_SET_forest-pvp
```

The manifest operation must use the same value without the prefix:

```json
"spawnSet": "forest-pvp"
```

The framework rejects the scene before it creates the native mode owner when
the exact marker is absent.

## 5. Declare PVP in the manifest

Use this complete schema-version-1 shape:

```json
{
  "operationId": "author.example-map.pvp",
  "displayName": "EXAMPLE MAP PVP",
  "displayOrder": 0,
  "mode": "pvp",
  "areaOfOperation": "EXAMPLE REGION",
  "sitrep": "Team 1 enters from the player side. Team 2 enters from the opposing side. Both teams use the same package build.",
  "minPlayers": 2,
  "maxPlayers": 12,
  "spawnSet": "example-pvp",
  "infiltrations": [
    {
      "id": "team-one-entry",
      "displayName": "TEAM ONE ENTRY",
      "mapPositionX": 0.5,
      "mapPositionY": 0.2,
      "maxPlayers": 12
    }
  ],
  "timeCodes": ["1100", "0200"],
  "defaultTimeCode": "1100"
}
```

Do not add `minEnemies` or `maxEnemies` to a PVP record. The closed schema
rejects them.

The infiltration record places one marker on the briefing image. It does not
choose Team 1 or Team 2 coordinates. The 3D team markers and retail team IDs
control player placement.

## 6. Understand the exact retail StandardPVP defaults

The shipped StandardPVP scene is build index `30`. Its inactive `PVP Game
Mode` root owns a `NetworkIdentity`, `PvpGameode`, and
`ExcludeFromMirrorSpawnable`. The `PvpGameode` component has these serialized
values:

| Field | Shipped value |
| --- | --- |
| `Team1SpawnPoints` | 10 references |
| `Team2SpawnPoints` | 10 references |
| `MaxRounds` | `13` |
| `RoundsToWin` | `7` |
| `currentRound` | `0` |
| `RoundTime` | `120` |
| `RoundTimer` | `0` |
| `Team1Score` | `0` |
| `Team2Score` | `0` |
| `CurrentScoreUI` | `0` |
| `Team1Players` | empty list |
| `Team2Players` | empty list |

The current framework uses the same scalar seeds. The retail
`Server_AllPlayersLoaded` body replaces `MaxRounds` and `RoundTime` with the
current lobby PVP settings.

## 7. Supply every reference that native PVP reads

`PvpGameode` is not safe with only two spawn lists. Its native SyncVar hooks
and result RPCs also read:

- `MusicSource` and `AnnouncerSource`;
- 16 non-null announcer arrays;
- a `TeleType` result-text component;
- BLUFOR score, OPFOR score, and clock `TextMeshProUGUI` references;
- six result `GameObject` roots;
- two `Animator` references;
- exact animation-state strings `FadeOut` and `FadeIn`;
- two game-score text fields;
- `WINNING`, `LOSING`, and `TIE` status text fields.

The framework creates these generic presentation objects under
`MODDED_PVP_NATIVE_UI`. It creates one non-null silent clip because the guide
and Nexus package cannot redistribute OPERATOR audio. It fills the clip arrays
with the same lengths as the retail component:

```text
spawn, spawn-short, round-win, game-win, game-lose, round-lose: 3 each
game-draw, round-draw: 1 each
```

The silent clips change only presentation. The retail round state, score,
team death checks, and respawn methods stay active.

## 8. Follow the native lifecycle

The intended sequence is:

```text
package scene loads
  -> terrain and marker validation
  -> package markers receive SpawnPoint components
  -> Team 1 and Team 2 lists are assigned to StandalonePvpGameMode
  -> the framework supplies required presentation references
  -> NetworkIdentity is spawned by the host
  -> PvpGameode.OnStartClient
  -> GameMode.Initialize
  -> readiness coroutines
  -> PvpGameode.Server_AllPlayersLoaded
  -> retail RespawnPlayers coroutine
  -> retail freeze time
  -> retail StartNewRound / death / score / end-round loop
```

Do not run a second continuous player-position loop after native PVP becomes
active. It can fight the retail respawn coroutine and put a player back at an
old marker. The current framework stops its position-only fallback when
`nativePvpLifecycle=true`.

## 9. Restore all global and static owners

One standalone operation temporarily owns:

- `GameManager.SpawnPointsInScene`;
- `GameManager.instance.Pspawns`;
- `GameManager.instance.PnextSpawnIndex`;
- `GameMode.singleton`;
- `PvpGameode.instance`;
- the runtime network root and PVP UI;
- the silent runtime clip.

Capture the prior global lists before replacement. Restore them only when the
current identity still equals the operation-owned identity. Do not restore a
list that contains objects from an unloaded scene. Clear static mode owners
when they still point to the operation component.

This identity check prevents an old operation from overwriting a newer retail
owner during return-to-armory or restart.

## 10. Register the runtime game-mode owner on every peer

An ordinary run-time `NetworkIdentity` with `assetId=0` can appear to work on
the host and still fail on a remote client. Mirror needs a matching registered
prefab identity before it processes the host spawn message.

The current framework creates one inactive template per operation mode and
uses these deterministic IDs:

| Mode | Mirror asset ID |
| --- | ---: |
| PVE | `0x4D4F5001` |
| StandardPVP | `0x4D4F5002` |

Each peer checks `NetworkClient.prefabs` for a collision and then calls
`NetworkClient.RegisterPrefab(template, assetId)`. The host activates and
spawns the operation instance with `NetworkServer.Spawn(instance, assetId,
connection)`. A remote callback checks the clone's asset ID and component
type before it adopts the clone as `GameMode.singleton` or
`PvpGameode.instance`. Release calls `NetworkClient.UnregisterPrefab` for the
operation-owned template. Do not assign one ID to two different prefabs.

## 11. Require exact peer agreement

Modded Operations `0.3.31` source uses private protocol v6 for PVP and online
PVE. It does not infer equality from a package version string.

Before native launch, the host freezes each authenticated remote connection
object and numeric ID. Every peer must match:

- the exact selected-loader suite receipt, receipt-owned manifest sidecar and
  selected files, including loaded framework, API Core, API host, and declared
  companion paths;
- protocol, game build, required capabilities, and session identity;
- package ID, package version, and exact package content identity;
- optional `runtimeCompanion` GUID, version, loader-neutral pair ID, and marker
  contract;
- map, operation, mode, spawn set, scene variant/path, time, and player range;
- for PVE, the declared enemy range and host-confirmed enemy count.

A remote loads and verifies its own package bytes, commits the exact operation,
and only then sends `ContentReady`. The host does not start the native scene
transition until every frozen peer is content-ready.

After transition, each peer must validate the exact package scene, construct
and register the deterministic mode template, install the mode-specific spawn
contract, and pass any declared companion contract before `SceneReady`. PVE
also validates the agreed count against navigation-valid ordinary markers that
remain at least 2 m apart after snapping; inactive utility markers are eligible
and active count is telemetry only. A declared READY marker is insufficient
when plugin identity is wrong. A declared FAILED marker always wins, including
after READY.

Scene readiness is bound to a nonzero unsigned 64-bit epoch. Initial launch is
epoch 1. Each retained-content Restart advances it exactly once. A remote maps
the request to one monotonic local scene generation and cannot reuse a prior
generation's acknowledgement. The host retries the request every 5 seconds
inside the 90-second scene deadline. Zero, stale, future, out-of-phase, and
overflowing epochs fail closed.

The frozen membership cannot change. A late join, disconnect, or replacement
connection aborts even if a numeric connection ID is reused. Late join is not
supported for either agreed mode. This implementation is `PROVEN-STATIC` and
does not by itself prove real PVP combat or PVE AI/gameplay equivalence.

## 12. Verify PVP before release

Record all of these results:

1. One Confirm starts the scene.
2. The controller log names `StandalonePvpGameMode`.
3. The log reports ten Team 1 and ten Team 2 markers for Forest.
4. The all-players-loaded log reports `nativePvpLifecycle=true`.
5. No position-only fallback error occurs.
6. Team 1 starts on the PVE-player side.
7. Team 2 starts on the PVE-enemy side.
8. Both teams can move after freeze time.
9. Bullet damage and deaths update the retail PVP state.
10. The next round respawns each team on its own side.
11. The game ends after the retail win condition.
12. Restart creates one fresh mode owner.
13. Return to the armory clears the mode owner and restores player control.
14. The client log proves registration of asset ID `0x4D4F5002`, receipt of a
    matching clone, and adoption of `StandalonePvpGameMode`.
15. The host and remote logs show the same frozen membership, exact agreement,
    every `ContentReady`, and the current-epoch `SceneReady` set.
16. Repeat with a remote client. Both peers must present an exact valid suite
    receipt/sidecar/file closure and identical package manifest plus every
    declared package file.
17. Fire real weapons in both directions. Record firearm-specific hit/death,
    score, round respawn, retained-content Restart, unload, and armory return.
18. If 12-player support is advertised, run a separate real 12-player matrix;
    six static markers per team prove capacity only, not load behavior.

Host-only success does not prove remote-client AssetBundle or Mirror spawn
behavior. Keep the multiplayer release gate open until the remote-client test
passes.

For online PVE, run a separate two-process matrix. Start both peers together;
prove both leave loading and remain grounded, the host-confirmed count is the
agreed count, the remote adopts the same operation, AI replicate and take real
weapon damage, all-AI-dead completion and extraction work, Restart replaces
the scene on both peers, and failure/return/close tear down cleanly. Repeat
spawn time, frame time, completion, Restart, and teardown at the map's claimed
certified maximum. The global cap is 100, but a map may claim only its smaller
verified maximum. Reject late join or any roster change within the bounded
agreement deadline. Solo, host-only, and join-in-progress success do not close
this gate.
