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

- Assemblies: BepInEx `OperatorModdedOperations.dll`; MelonLoader
  `OperatorModdedOperations.MelonLoader.dll`.
- Current unshipped multiplayer-test candidate: `0.3.30`. BepInEx is 632,832
  bytes / SHA-256
  `772D9FC1470DCC115B22F8C232C4A3B90D0FC727C5B1D27A0F54ED95E5D1AE86`;
  MelonLoader is 634,368 bytes / SHA-256
  `2C2D11C28BD2D470ABAC83F2FC558476384DA8D79302FA4B1ADD6F845A113F83`.
- Required bundled preview API: `0.2.0-alpha.7`. Core is 204,800 bytes /
  SHA-256
  `5B74AC25B4047D9AB8E9929D136C53B7543DA52B621FAF3D4AF42719D276E21E`;
  BepInEx host is 15,872 bytes /
  `6223553C5406AD3586F23EA2B5F05C6F4626FA62A03E0598667E09A5486E1E3B`;
  MelonLoader host is 24,576 bytes /
  `1CC745AB57A18848F15A47C80FCE5399C4006FE3ADE2C94B449792CFBABFF7FA`.
- This candidate is `PROVEN-STATIC`, not a promoted public release. The public
  decompiler/source-state record remains `0.3.28` with API alpha.5 until a
  release promotion regenerates those artifacts. Do not mix the candidate
  DLL with that prior publication identity.
- Current source includes `CerberusNativeTabFix.cs`, the peer-agreement/runtime
  partials, `PveEnemyCountSelection.cs`, and `FrameworkEvidence.cs`.
- Install directories: BepInEx
  `<OPERATOR_INSTALL>\BepInEx\plugins\OperatorModdedOperations`; MelonLoader
  `<OPERATOR_INSTALL>\Mods`. Install/activate exactly one loader.
- Native selector builder: `BuildPackageInfiltrationMapPrefab`.
- Briefing formatter: `FormatCatalogBriefing`.
- Preview decoder/cache: `GetOrLoadPreviewSprite`.
- Preparation/fullscreen preview binder: `ReplaceNativeMapPreview`.
- Native launch calls: `InfilSelectorDisplayer.SpawnMap` and
  `CerebusOpboard.Start_Operation`.
- Confirm capture: `BeginCatalogOperationLaunch` and `PendingMapLaunch`.
- Confirm owner repair: `RestoreCapturedLaunchLaptop`.
- Confirm loading state: `SetNativeConfirmationLoadingState`.
- Additive-scene native loading gate:
  `ShowNativeLoadingScreenForPackageScene` calls
  `GameManagerNetwork.ShowLoadingScreen()` before runtime terrain/material
  preparation. Verify `LoadingScreen.activeSelf` and `activeInHierarchy`.
  `LoadingScreenVisible` returns `_hideLoadingScreenSoon` on the supported
  build and is not the canvas-state probe.
- Verified dependency-asset loan:
  `LoadVerifiedMapDependencyAsset<T>(mapId, assetPath)`. The framework keeps
  bundle ownership and unload responsibility. This generic cross-plugin path
  is `PROVEN-STATIC`; Forest `0.4.12` returned null for live `TextAsset`
  requests. Require a type-specific live probe before release use.
- Scene gate: `ValidateStandaloneSceneContract`.
- Generic runtime terrain owner: `TryPrepareRuntimeTerrain` and
  `ValidateWalkableGroundContract`.
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
- Per-peer prefab registration uses an exact custom spawn/unspawn handler that
  initializes and validates every dynamic clone before Mirror deserialization.
- Repeat-generation stale-entry repair:
  `EnsureStandaloneBootstrapPrefabRegistered` removes only a fake-null entry
  from `NetworkClient.prefabs` by the deterministic asset ID and calls
  `NetworkClient.UnregisterSpawnHandler(assetId)` before registration.
- Server spawn: `NetworkServer.Spawn` with the selected deterministic asset
  ID.
- Remote game-mode adoption: `TryAdoptNetworkSpawnedGameMode`.
- Native PVP list/default wiring: `ConfigureStandalonePvpController`.
- Native PVP required-reference wiring: `ConfigureStandalonePvpPresentation`.
- PVE range selection: private native-briefing clone plus
  `PveEnemyCountSelection`; the confirmed count is host-authoritative and must
  not modify Tier 1, vanilla operation arrays/ranges, or PVP.
- PVE creation: `TrySpawnStandalonePveEnemies` and
  `RaidManager.ServerSpawnAI(false)`.
- Standard-PVE extraction authoring gate:
  `ValidateStandaloneSceneContract` requires exactly one
  `PVE_ExfilZone_` marker with a positive `BoxCollider` trigger.
- Native extraction owner: `ConfigureStandalonePveController` creates the
  operation-owned `RaidManager`, `ExfilZone`, copied trigger, locked marker,
  and ATAK marker; `ResetStandalonePveExtractionState` establishes the locked
  global state and 15-second timer.
- Native ATAK reconstruction: `CreateNativeAtakExfilMarker` uses the resident
  `ExfilZone` 512-by-512 texture, `HDRP/Unlit`, mesh `Marker`, layer `17`,
  render queue `2501`, scale `0.65`, and the audited vanilla vertex/UV/
  rotation/texture-offset values.
- Success-preserving teardown: `ReleaseStandaloneGameMode` removes only
  operation-owned PVE assets and singleton references and deliberately keeps
  `GameManagerNetwork.SuccessfulOperation` when extraction succeeded.
- Fixed PVE profile writer: `ConfigureStandaloneBotDetails`.
- Fixed PVE profile diagnostic: `FormatPveAiProfile`.
- Optional first-wander cap:
  `TryApplyProfiledPveInitialWanderDelayCap`; it is server-only, new-brain-only,
  and writes only the first `BrainAI.wanderTime` clock when the package field
  is present.
- Live profiled-PVE contract capture:
  `StartProfiledPveAiDiagnostics`.
- Bounded read-only snapshots: `ProcessProfiledPveAiDiagnostics` and
  `LogProfiledPveAiSnapshot`; schedule `0, 10, 30, 60, 90, 120` seconds.
- The contract reads live `WanderTimer * Patience` and remaining first delay;
  the snapshots read movement, movement toward
  insertion, `CurrentSeenTarget`, `CurrentState`, and
  `EyesAI.DetectionLayerMask`. It writes no AI field.
- Native bot movement probe: `GetProfiledPveNavigationPosition` reads
  `BrainAI.agent.position` from the `AgentController` child. Do not read the
  stationary `BrainAI` root transform.
- Required Operator Mod API for the current candidate: `0.2.0-alpha.7`.
  The reaction disposition/time-cap profile remains optional and closed.
- Lifecycle release: `ReleaseStandaloneSceneContracts` and
  `ReleaseStandaloneGameMode`; release captures `BootstrapAssetId` and removes
  its prefab/spawn-handler keys even when `BootstrapPrefabRoot` is fake-null.
- Map-neutral render transaction: `ApplyStandaloneRenderContract`. The
  package selects `native-outdoor-v1`; the framework owns the process-global
  sun, HDRP Volume, white-phosphor selection, and reverse restore. The package
  owns its verified LUT. The map companion must not install a competing global
  Volume.

The framework MUST NOT contain a map name, map coordinate, map-material
profile, or graph size. It MAY contain a named map-neutral render contract
that more than one package can select.

## Ukrainian Forest worked reference

- Package: `community.ukrainian-forest`, version `0.3.21`.
- Map: `community.ukrainian-forest.ukrainian-forest`.
- Dependency bundle: `content/operator_ukrainian_forest`.
- Scene bundle: `content/operator_ukrainian_forest_scene`.
- Dependency bundle: 630,326,573 bytes; SHA-256
  `B58BF6C4E4BC7DDF4AD753F7496E3CA9A116C2AEB685428FAEB8A83CC0C465A1`.
- Scene bundle: 17,613,729 bytes; SHA-256
  `0E67BEE91488C9F66F28454D15A177D0EDA2DEBF6A7944C625932B334F1712A8`.
- Scene:
  `Assets/Maps/UkrainianForest/Scenes/UkrainianForest.unity`.
- Preview: `media/ukraine_forest_preview.jpg`, 6,385,660 bytes, SHA-256
  `9d18a3abfa93b8b0f17a721f20731930a618b971f0b1cbd3fb97da3305ff4255`.
- Verify these bundle identities against the current `files[]` records before
  release. Unity `6000.3.8f1` emitted them after the PVP-Woods lighting
  authoring change, and both in-editor gates plus the external validator passed.
- PVE operation: `community.ukrainian-forest.pve`.
- PVE population range: `10` through `15`, inclusive.
- PVE profile: `dense-forest-balanced-v1`; range `45 m`; FOV `90` degrees;
  native effective-range sentinel `-1`; wander `38 m`; communications on;
  counter-suppression off.
- Playable combat area: X `-35..35`, Z `-22..118`, or `70 by 140 m`.
- Solo player-to-enemy distances: minimum `78.87 m`, median `91.60 m`,
  mean `90.72 m`, maximum `101.79 m`.
- PVP operation: `community.ukrainian-forest.pvp`.
- PVP AI population: zero.
- Scene marker:
  `MAP_ID_community.ukrainian-forest.ukrainian-forest`.
- Spawn sets: `SPAWN_SET_forest-pve` and `SPAWN_SET_forest-pvp`.
- Team 1 marker role: normal PVE-player side, ten markers, authored local
  Z range `5.4` through `13.2`.
- Team 2 marker role: PVE-enemy side, ten markers, authored local Z range
  `82.4` through `90.2`.
- Standard-PVE extraction marker: `PVE_ExfilZone_00` at authored root position
  `(0.000,0.112,7.000)`. Its `BoxCollider` trigger center is
  `(1.3259258,2.066852,1.6703243)` and size is
  `(25.236944,7.376298,15.531027)`. This is the north player/Team-1 insertion
  area, not the south enemy side.
- Current-build team IDs: Team 1 is `1`; Team 2 is `2`.
- Terrain object: `NATIVE_Ground_HillyTerrain`.
- Companion assembly: `OperatorUkrainianForest.dll`.
- Companion version: `0.4.19`.
- Accepted Release DLL: 298,496 bytes; SHA-256
  `85FC1478A8E04FDA37CB2CBAD08D26231F3C4ECABD6A5F19899484948A763E74`.
- Build the exact Release binary from the current source. Verify its hash
  against the release archive checksum file.
- Companion source file: `OperatorUkrainianForestPlugin.cs`.
- Exact-scene entry: `ProcessStandalonePackageScene`.
- Forest sight activation: `ConfigureForestVegetationVisionBlockers`; exactly
  79 direct plus 104 perimeter `AI Collider` triggers on layer 18
  `AI_VisionBlock`, for 183 total. The source scene has 118 direct and 156
  perimeter bushes, but 91 of them are Juniper instances without the native
  blocker child. Never invent a blocker for those Junipers.
- Navigation owner: `EnsureStandaloneNavigationGraph`.
- Bounds gate: `IsInsideForestPlayableBounds`.
- World audit: `LogStandaloneWorldContract`.
- Authored-tree grounding: `AlignStandaloneAuthoredTreesToTerrain`.
- Authored-tree ground datum:
  `NATIVE_TRUNK_GROUND_DATUM_ONE_SIXTH`, computed in Unity from LOD0
  bark/trunk submesh indices and vertices. Recognized slots are `pine_bark`,
  `Trunk_pine_var4`, `Bark_Mat`, and `Bark_2_Mat`. Narrow pines use one sixth
  of the full rendered trunk at the center sample. Broad oaks use the oriented
  main-stem reference, a `0.25 m` embed, and the lowest contact in a bounded
  `0.60 m` to `2.00 m` root footprint. The datum stores that contact X/Z.
  Require `0.75` of the complete rendered tree above terrain. The maximum
  absolute correction is `12 m`.
- Active pine material path: `FindNativeMaterialShader` selects
  `Shader Graphs/BotD_Graph_Lit_TranslucentAlphaCutoff` for `Pine_Needle` and
  `Shader Graphs/SeedMesh_Tree_Bark` for `pine_bark` and
  `Trunk_pine_var4`. `CopyBundleMaterialProperties` binds `_MainTex`,
  `Normal_vegetation`, and `mask_vegetation`. The opaque state uses exact
  hashed `Vector1_DDCDCAD2`, `Vector1_16F2F1E4`, and
  `Vector1_813F3AD6` values plus `_Wetness_sm=0`, `_Vertex_AO_sm=0`, and
  `_cutoff=.25`. The active `0.4.19` path does not depend on raw-TextAsset
  borrowing.
- Exact day donor: `sharedassets11.assets`, `PVP Woods Warehouse`, `Nice Sun`
  GameObject path `5150`, Transform path `14228`, Light path `41512`, and
  `Global Volume Profile 1` path `198`. Use 30,000 lux, 5,500 K, bounce 5,
  bloom `.03`, and lens-flare intensity `.5` with the audited overrides.
- Accepted local lifecycle evidence on 2026-08-04: one-click 11:00 launch,
  non-white and non-glossy pines, operation unload, Lone Wolf re-entry,
  one-click repeat 02:00 launch, above-terrain repeat spawn, four-tube white
  phosphor, and terrain visibility outside ECOTI.
- IL2CPP-safe foliage traversal: index from `0` through
  `foliageRoot.childCount - 1`, then call `foliageRoot.GetChild(index)`.
- Teardown: `OnSceneUnloaded`.

The package is a worked reference. Do not treat its IDs, coordinates, or
assets as universal constants.
