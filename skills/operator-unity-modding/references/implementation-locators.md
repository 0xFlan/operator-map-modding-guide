# Implementation locators

Use this reference when a task needs exact standalone artifact identities.

## Public path tokens

- `<OPERATOR_INSTALL>` contains `OPERATOR.exe` and `BepInEx`.
- `<AUTHOR_WORKSPACE>` is the private mod work directory.
- `<UNITY_PROJECT>` contains `Assets` and `ProjectSettings`.
- `<PACKAGE_CATALOG>` contains one or more package directories.
- `<PACKAGE_ROOT>` contains `operator-map-package.json` for one package.
- `<USER_PROFILE>` is the operating-system user profile directory.

Keep these tokens in reusable output. Do not publish a real user name, a
drive-specific workspace, a private log, or a private test control.

## Generic framework

- Assembly: `OperatorModdedOperations.dll`.
- Current source file: `CerberusNativeTabFix.cs`.
- Install directory:
  `<OPERATOR_INSTALL>\BepInEx\plugins\OperatorModdedOperations`.
- Native selector builder: `BuildPackageInfiltrationMapPrefab`.
- Native launch calls: `InfilSelectorDisplayer.SpawnMap` and
  `CerebusOpboard.Start_Operation`.
- Confirm capture: `BeginCatalogOperationLaunch` and `PendingMapLaunch`.
- Confirm owner repair: `RestoreCapturedLaunchLaptop`.
- Confirm loading state: `SetNativeConfirmationLoadingState`.
- Scene gate: `ValidateStandaloneSceneContract`.
- Gameplay bootstrap: `CreateStandaloneGameplayBootstrap`.
- Current-scene player spawn registration:
  `ConfigureStandalonePlayerSpawnContract`.
- Team-aware player marker selection: `SelectPlayerMarker` and
  `PvpMarkerMatchesTeam`.
- PVE range selection: `ChooseStandalonePveEnemyCount`.
- PVE creation: `TrySpawnStandalonePveEnemies` and
  `RaidManager.ServerSpawnAI(false)`.
- Lifecycle release: `ReleaseStandaloneGameMode`.

The framework MUST NOT contain a map name, map coordinate, shader profile, or
graph size.

## Ukrainian Forest worked reference

- Package: `community.ukrainian-forest`, version `0.3.7`.
- Map: `community.ukrainian-forest.ukrainian-forest`.
- Dependency bundle: `content/operator_ukrainian_forest`.
- Scene bundle: `content/operator_ukrainian_forest_scene`.
- Scene:
  `Assets/Maps/UkrainianForest/Scenes/UkrainianForest.unity`.
- PVE operation: `community.ukrainian-forest.pve`.
- PVE population range: `10` through `15`, inclusive.
- PVP operation: `community.ukrainian-forest.pvp`.
- PVP AI population: zero.
- Scene marker:
  `MAP_ID_community.ukrainian-forest.ukrainian-forest`.
- Spawn sets: `SPAWN_SET_forest-pve` and `SPAWN_SET_forest-pvp`.
- Team 1: native ID `1`, normal PVE-player side, ten markers, local Z range
  `5.4` through `13.2`.
- Team 2: native ID `2`, PVE-enemy side, ten markers, local Z range `82.4`
  through `90.2`.
- Terrain object: `NATIVE_Ground_HillyTerrain`.
- Companion assembly: `OperatorUkrainianForest.dll`.
- Companion source file: `OperatorUkrainianForestPlugin.cs`.
- Exact-scene entry: `ProcessStandalonePackageScene`.
- Navigation owner: `EnsureStandaloneNavigationGraph`.
- Bounds gate: `IsInsideForestPlayableBounds`.
- World audit: `LogStandaloneWorldContract`.
- Teardown: `OnSceneUnloaded`.

The package is a worked reference. Do not treat its IDs, coordinates, or
assets as universal constants.

## Public authoring code

- `templates/Editor/BuildStandaloneMapBundles.cs` builds the dependency bundle
  before the scene bundle.
- `BuildStandaloneMapBundles.VerifyBundle` requires zero scenes in a
  dependency bundle and exactly one declared scene in a scene bundle.
- `templates/Editor/ValidateStandaloneMapScene.cs` validates exact identity,
  the selected manifest operation, terrain collision, markers, and bounds.
- `ValidateStandaloneMapScene.LoadSelectedOperation` uses the selected PVE
  operation's `minEnemies`. It does not require AI markers for PVP.
- `ValidateStandaloneMapScene.ContainsWithClearance` converts world-space wall
  clearance to local units for a scaled `BoxCollider`.
- `schemas/operator-map-package.schema.json` is the closed pre-v1 package
  contract.

Read `docs/13-exact-implementation-reference.md` in the public guide for the
complete asset addresses, terrain layer records, bundle hashes, and runtime
verification matrix.
