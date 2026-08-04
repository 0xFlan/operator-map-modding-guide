# Implementation locators

Use this reference when a task needs exact standalone artifact identities.

## Public path tokens

- `<OPERATOR_INSTALL>` contains `OPERATOR.exe` and `BepInEx`.
- `<AUTHOR_WORKSPACE>` is the private mod work directory.
- `<UNITY_PROJECT>` contains `Assets` and `ProjectSettings`.
- `<PACKAGE_CATALOG>` contains one or more package directories.
- `<PACKAGE_ROOT>` contains `operator-map-package.json` for one package.
- `<PROJECT_EVIDENCE_LOG>` is the complete project BIBLE or evidence log.
- `<USER_PROFILE>` is the operating-system user profile directory.

Keep these tokens in reusable output. Do not publish a real user name, a
drive-specific workspace, a private log, or a private test control.

## Generic framework

- Assembly: `OperatorModdedOperations.dll`.
- Current candidate version: `0.3.14`.
- Current Release binary: 149,504 bytes, SHA-256
  `d6088d826fb88f9accc3c86da58dbafd9462b7a5cd32f14789ee941fb789b8fe`.
- Current source file: `CerberusNativeTabFix.cs`.
- Install directory:
  `<OPERATOR_INSTALL>\BepInEx\plugins\OperatorModdedOperations`.
- Native selector builder: `BuildPackageInfiltrationMapPrefab`.
- Briefing formatter: `FormatCatalogBriefing`.
- Preview decoder/cache: `GetOrLoadPreviewSprite`.
- Preparation/fullscreen preview binder: `ReplaceNativeMapPreview`.
- Native launch calls: `InfilSelectorDisplayer.SpawnMap` and
  `CerebusOpboard.Start_Operation`.
- Confirm capture: `BeginCatalogOperationLaunch` and `PendingMapLaunch`.
- Confirm owner repair: `RestoreCapturedLaunchLaptop`.
- Confirm loading state: `SetNativeConfirmationLoadingState`.
- Scene gate: `ValidateStandaloneSceneContract`.
- Gameplay bootstrap: `CreateStandaloneGameplayBootstrap`.
- Current-scene player spawn registration:
  `ConfigureStandalonePlayerSpawnContract`.
- First owned-player kickoff: `PlayerMaster.SpawnPlayer()`.
- Repeat-launch host recovery:
  `InvokeGeneratedServerPlayerSpawnBody` calls
  `UserCode_CMDSpawnPlayer__NetworkIdentity` once after the 300-frame grace
  period if the first kickoff did not produce a player object.
- Owned-host request limit: two requests. The first request is the native
  client kickoff. The second request is the generated server-body recovery.
- Team-aware player marker selection: `SelectPlayerMarker` and
  `PvpMarkerMatchesTeam`.
- Native PVP owner: `StandalonePvpGameMode : PvpGameode`.
- Deterministic network prefab asset IDs:
  `StandalonePveGameModeAssetId = 0x4D4F5001` and
  `StandalonePvpGameModeAssetId = 0x4D4F5002`.
- Per-peer prefab registration: `NetworkClient.RegisterPrefab`.
- Server spawn: `NetworkServer.Spawn` with the selected deterministic asset
  ID.
- Remote game-mode adoption: `TryAdoptNetworkSpawnedGameMode`.
- Native PVP list/default wiring: `ConfigureStandalonePvpController`.
- Native PVP required-reference wiring: `ConfigureStandalonePvpPresentation`.
- PVE range selection: `ChooseStandalonePveEnemyCount`.
- PVE creation: `TrySpawnStandalonePveEnemies` and
  `RaidManager.ServerSpawnAI(false)`.
- Lifecycle release: `ReleaseStandaloneSceneContracts` and
  `ReleaseStandaloneGameMode`.

The framework MUST NOT contain a map name, map coordinate, shader profile, or
graph size.

## Ukrainian Forest worked reference

- Package: `community.ukrainian-forest`, version `0.3.11`.
- Map: `community.ukrainian-forest.ukrainian-forest`.
- Dependency bundle: `content/operator_ukrainian_forest`.
- Scene bundle: `content/operator_ukrainian_forest_scene`.
- Scene:
  `Assets/Maps/UkrainianForest/Scenes/UkrainianForest.unity`.
- Preview: `media/ukraine_forest_preview.jpg`, 6,385,660 bytes, SHA-256
  `9d18a3abfa93b8b0f17a721f20731930a618b971f0b1cbd3fb97da3305ff4255`.
- Dependency bundle: 630,238,479 bytes, SHA-256
  `8c93a6ecc80fbc6b387a9b14df9ae4550afc5278298499112e30d03b07dbe3cc`.
- Scene bundle: 17,589,677 bytes, SHA-256
  `6586f96e932dc8184984c3ff2ec79e38f4b5c5be934fb4049e006af6e7843aee`.
- PVE operation: `community.ukrainian-forest.pve`.
- PVE population range: `10` through `15`, inclusive.
- PVP operation: `community.ukrainian-forest.pvp`.
- PVP AI population: zero.
- Scene marker:
  `MAP_ID_community.ukrainian-forest.ukrainian-forest`.
- Spawn sets: `SPAWN_SET_forest-pve` and `SPAWN_SET_forest-pvp`.
- Team 1 marker role: normal PVE-player side, ten markers, authored local
  Z range `5.4` through `13.2`.
- Team 2 marker role: PVE-enemy side, ten markers, authored local Z range
  `82.4` through `90.2`.
- Current-build team IDs: Team 1 is `1`; Team 2 is `2`.
- Terrain object: `NATIVE_Ground_HillyTerrain`.
- Companion assembly: `OperatorUkrainianForest.dll`.
- Companion candidate version: `0.4.6`.
- Current companion Release binary: 285,184 bytes, SHA-256
  `f57b458036ca5594fa9e0cbeaaa102cd9cc8c5410ec6f485157b850d4b67d523`.
- Companion source file: `OperatorUkrainianForestPlugin.cs`.
- Exact-scene entry: `ProcessStandalonePackageScene`.
- Navigation owner: `EnsureStandaloneNavigationGraph`.
- Bounds gate: `IsInsideForestPlayableBounds`.
- World audit: `LogStandaloneWorldContract`.
- Authored-tree grounding: `AlignStandaloneAuthoredTreesToTerrain`.
- Authored-tree root contact: lowest finite visible-renderer `bounds.min.y`;
  keep the native trunk collider but do not use its hidden bottom as the
  placement datum; root embed `0.12` m; minimum above-ground rendered fraction
  `0.75`; maximum correction `12` m.
- IL2CPP-safe foliage traversal: index from `0` through
  `foliageRoot.childCount - 1`, then call `foliageRoot.GetChild(index)`.
- Teardown: `OnSceneUnloaded`.

The package is a worked reference. Do not treat its IDs, coordinates, or
assets as universal constants.
