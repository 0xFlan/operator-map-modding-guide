# 3c. Native mode ownership, PVE, and StandardPVP

Status: `PROVEN-STATIC` for OPERATOR Steam build `24091246` and the current
framework source. Host, remote-client, restart, and return-to-armory behavior
must also pass the physical release matrix.

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
| map-only terrain/material reconstruction | optional companion | no | renderer and physics |
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
  "maxPlayers": 16,
  "spawnSet": "example-pvp",
  "infiltrations": [
    {
      "id": "team-one-entry",
      "displayName": "TEAM ONE ENTRY",
      "mapPositionX": 0.5,
      "mapPositionY": 0.2,
      "maxPlayers": 16
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

## 10. Verify PVP before release

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
14. Repeat with a remote client. Both peers must use identical framework,
    package, scene-bundle, dependency-bundle, and companion hashes.

Host-only success does not prove remote-client AssetBundle or Mirror spawn
behavior. Keep the multiplayer release gate open until the remote-client test
passes.
