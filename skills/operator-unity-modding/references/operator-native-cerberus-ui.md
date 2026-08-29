# OPERATOR native Cerberus UI

Use these rules when adding to the world-space DreamOS mission laptop.

Use **OPERATOR: Modded Operations — Standalone Map Framework** as the public
framework name. Use *Cerberus* only for this shipped mission-laptop UI
contract.

## Proven shell contract

- Discover `MissionLaptop` through the closed generic
  `Resources.FindObjectsOfTypeAll<T>()` IL2CPP route. An untyped Component scan
  can return misleading generated proxies.
- The interactive surface is `MissionLaptop.osCanvas` plus `uiRaycaster`.
- Clone a shipped `Michsky.DreamOS.PanelButton`; do not substitute plain text or
  a bare unthemed widget.
- Parent a new top tab to the same live `Operation Selection` transform as the
  official buttons.
- Place world-space UI in the parent's local `RectTransform` coordinates. Do
  not convert corners through world coordinates and assign `rect.position`.
- Preserve the native font, animator, state children, sprites, and styling
  components. Disable localization only on genuinely custom fixed text.
- When narrowing buttons, fit only their title `TMP_Text` objects. Do not scale
  the complete button hierarchy or its raycast area.

These shell rules produced a visible native-looking third tab in a physical
OPERATOR laptop test.

## Panel and page contract

DreamOS `WindowPanelManager.PanelItem` contains:

```text
panelName
panelObject : Animator
panelButton : PanelButton
altPanelButton : ButtonManager
```

The manager owns `OpenPanel(string)`, `OpenPanelByIndex(int)`, current/next
panel and button state, animator transitions, indicator movement, and
`onPanelChanged`. Cloning a visible `PanelButton` without a corresponding panel
entry is not native panel-switch parity.

Never bind per-laptop content through a global object-name search. Multiple
`MissionLaptop` instances can coexist; a visible button may otherwise activate
a page under an inactive canvas. Store UI state per laptop instance, bind each
button directly to its same-owner page, and remove it when the owner is
destroyed or its scene unloads.

Require all of these for visibility evidence:

```text
button and page share the intended MissionLaptop owner
owner scene is loaded
osCanvas/uiRaycaster is the accessed interactive surface
page activeSelf == true
page activeInHierarchy == true
```

A listener log or `activeSelf` alone is not click-through proof.

## Official content and shared-state contract

Official operation rows are built by `MissionLaptop.SpawnOperations` from
`OperationsManager.ActiveOperations` and `SimulationOperations`, then
instantiated through `CerebusUiBase.CreateInstance` / `OperationSelectionUI`.
Selection continues through target packages, `CerebusOpboard`, and
`OperationBoardUI`.

The physical laptop opening is local, but the planning screen is shared.
`MissionLaptopNetworkState` replicates page/mode, selection, infil, parameters,
and confirmation state and its hooks update every `MissionLaptopDisplayer`.
Do not mutate only one displayer for multiplayer behavior.

Do not append custom operations to the official arrays until peer-local content
identity, deterministic ordering, integer-index resolution, cache timing, and
missing-package refusal are proven. A local catalog mutation can make shared
indexes resolve to different content on different peers.

## Standalone map ownership boundary

OPERATOR: Modded Operations owns generic catalog-to-UI and operation lifecycle behavior: private
native-style tab/row/board clones, shipped infiltration selector priming, exact
declared dependency/scene load, readiness, vanilla-compatible mode ownership,
player lifecycle, PVE actor creation, a shipped `PvpGameode` round adapter,
native Standard-PVE completion/extraction/ATAK state, and normal/KIA
same-scene Restart Operation. Standalone PVE must provide
`InfiltrationManager.instance` plus its synchronized `RaidTimer`; the shipped
persistent `GameManagerNetwork` remains the owner of Mission Failed UI,
Mission Successful UI, extraction timer state, and its Restart control. Keep
Modded Operations free of map names, private shader profiles,
Terrain dimensions, A* graph parameters, and map marker fixes.

PVE population bounds are declarative operation data: the package supplies
`minEnemies` and `maxEnemies`, and Modded Operations validates and selects an inclusive
deterministic host count from the valid sorted markers. PVP omits the fields.
Keep Modded Operations free of per-map counts, coordinates, pocket names, and spatial
bounds; those belong to the map scene and exact-scene companion.

For Standard PVE, the package scene owns exactly one `PVE_ExfilZone_`
transform and positive `BoxCollider` trigger. The framework copies that
geometry into its operation-owned `ExfilZone`, creates the native-compatible
ATAK marker from resident vanilla assets, starts locked, and preserves the
shipped all-AI-dead unlock, physical occupancy, 15-second extraction, success,
and return flow. The framework must not contain the map's extraction
coordinates. The map companion must not implement a second extraction state
machine or duplicate ATAK marker.

If a portable map scene needs installed-shader, TerrainData, navigation, or
marker reconstruction, ship a separate map-scoped companion. It must require
the exact package/map/scene identity and complete its strict world contract
before Modded Operations creates actors. See
[operator-standalone-map-runtime.md](operator-standalone-map-runtime.md).

## Package presentation contract

Keep mission presentation in the closed manifest. `displayOrder` owns row
order; `displayName` owns row/briefing/target/Confirm titles;
`areaOfOperation` and `sitrep` own the briefing; `timeCodes` and
`defaultTimeCode` own the time selector; and `infiltrations[]` owns the native
selector marker labels, array order, limits, and normalized 2D positions.

`maps[].previewImage` is a package-relative raw JPEG or PNG outside the Unity
AssetBundles. Read the verified bytes, decode through
`ImageConversion.LoadImage`, use clamp wrapping, preserve aspect ratio, and
cache the texture/sprite by immutable map ID. Use the same sprite for the
preparation map, fullscreen map, and package infiltration-map background.
One map has one preview under schema version 1.

For every infiltration, clone the shipped `MapInfilMarker` visual but replace
all mission-bearing data: object ID, `MarkerIndex`, `InfilName`, `MaxPlayers`,
ground/heli/exfil flags, board owner, and selection state. Assign both anchor
bounds from `mapPositionX/Y` and zero `anchoredPosition`. `(0,0)` is lower-left
and `(1,1)` is upper-right. This is a 2D selector contract; the selected spawn
set and current-scene marker transforms own the 3D player location.

Current implementation members are `SelectCatalogOperation`,
`FormatCatalogBriefing`, `UpdateCatalogOperationBoard`,
`GetOrLoadPreviewSprite`, `ReplaceNativeMapPreview`,
`BuildPackageInfiltrationMapPrefab`, `PrimeNativeInfiltrationSelector`, and
`InvokeNativeBoardStart` in `CerberusNativeTabFix.cs`.

## Private modded-PVE enemy selector

Expose `minEnemies..maxEnemies` only on the cloned modded-PVE briefing. Reuse
the shipped enemy-count slider presentation, but bind it to private operation
state. Confirm captures the displayed value atomically; Restart retains it;
the next fresh Operation Room selection may choose a new value. Revalidate the
value against the package range, the hard framework ceiling, and exact
navigation-valid marker capacity before the single native population call.

Never write Tier 1 unlock state, shipped Active/Simulation operation arrays,
vanilla operation objects, vanilla enemy ranges, or the PVP briefing. Capture
and reverse-restore only process globals temporarily owned for the active
modded generation, and restore them only when they still contain the values
the framework installed.

## Confirm lifetime

Package I/O can outlive the Confirm frame. Capture the exact player-owned
`MissionLaptop` and its `PlayerNetworking` before I/O. Keep the private modal
visible and disabled while content loads. If the same laptop released only
that field, restore the captured owned player. Close the modal and call
`CerebusOpboard.Start_Operation` in the same final frame. This rule is
`PROVEN-RUNTIME` for the pinned single-player Forest scope: one physical first
Confirm launches without a second laptop interaction. Re-test after a
supported dependency or game build changes.

## Verification gate

- Use physical world-space pointer clicks, not direct UnityEvent invocation.
- Compare Active, Simulation, and custom clicks before/after across manager
  indexes, panel animators, content lists, shared laptop state, and page
  `activeInHierarchy`.
- Exercise normal/hover/pressed/selected label states.
- Switch among all tabs repeatedly, close/reopen the laptop, and reload/rejoin
  the Operation Room.
- Record event source and frame number when more than one click surface exists;
  one physical click must produce one logical transition.
- Prove that the first physical Confirm launches without leaving and re-entering
  the laptop.
- Verify the declared preview on the preparation map, fullscreen map, and
  infiltration selector. Verify every marker label, order, position, and
  player limit plus every declared time code and the default selection.
- Keep forced-open and programmatic probes private and diagnostic. Remove them
  before release.
- Prove package Back and confirmation Cancel, then reopen and confirm that
  Active Operations and Operation Simulation still show their official rows.
- Prove PVE and PVP first load plus normal Restart separately; PVP must create
  zero PVE actors.
- For Standard PVE, prove exactly one map-authored extraction trigger, initial
  lock, native all-AI-dead unlock, exact ATAK marker, physical occupation, the
  shipped 15-second timer, Mission Successful, scene unload, and Operation
  Room return. Confirm that success teardown does not clear
  `GameManagerNetwork.SuccessfulOperation` before the persistent UI reads it.
- Record KIA/end-screen Restart separately. Prove native lethal damage, the
  shipped Mission Failed popup, its shipped Restart control, and a fresh
  playable exact scene. A normal alive restart alone is insufficient. Do not
  clone or rebind failure UI; inspect the native mode singleton/timer contract
  first.
