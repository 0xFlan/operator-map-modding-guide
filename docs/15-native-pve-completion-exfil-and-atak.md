# Native PVE completion, extraction, and ATAK

Use this procedure when a standalone map supports a StandardPVE operation in
OPERATOR: Modded Operations `0.3.22` or later.

This procedure uses the shipped OPERATOR mission lifecycle. The map supplies
one extraction transform and one physics trigger. Modded Operations supplies
the network-owned `InfiltrationManager`, `RaidManager`, `ExfilZone`, extraction
timer, success screen, restart cleanup, and ATAK visual.

## 1. Inspect the installed game first

Do not copy an extraction design from an old guide or a different OPERATOR
build. Inspect the current installed build.

Use these sources:

```text
<game-root>/BepInEx/interop/Assembly-CSharp.dll
<game-root>/OPERATOR-Win64-Shipping_Data/level16
<game-root>/OPERATOR-Win64-Shipping_Data/sharedassets1.assets
<game-root>/OPERATOR-Win64-Shipping_Data/sharedassets3.assets
```

Inspect the following managed proxy types:

```text
InfiltrationManager
RaidManager
ExfilZone
GameManager
GameManagerNetwork
AtakScreen
atakMarker
```

Inspect native method bodies when a proxy only contains an IL2CPP trampoline.
Record the game executable fingerprint with the evidence.

For the current supported build, `level16` provides the accepted extraction
reference. Its `ATAK Exfil Marker` is part of the serialized exfil prefab. The
game does not create that child from an `atakMarker` component at runtime.

## 2. Keep ownership separate

Use this boundary:

| Owner | Data |
| --- | --- |
| Map scene bundle | extraction name, Transform, BoxCollider center/size, terrain placement |
| Manifest | operation ID, PVE mode, scene path, bundle paths/hashes, infiltration names, player limits, enemy range, optional AI profile |
| Modded Operations | network PVE owner, native raid director, native exfil component, unlock/countdown/success flow, current-build ATAK visual, restart cleanup |
| Map companion | map-specific terrain/material/navigation reconstruction and map-owned cleanup |

Do not serialize a second active `RaidManager` or `ExfilZone` in the standalone
map. Do not add a custom success timer or custom success popup. Do not put map
coordinates in the generic framework source.

## 3. Author exactly one extraction marker

Create one inactive GameObject under the accepted map root:

```text
PVE_ExfilZone_00
```

Add a `BoxCollider`. Set `isTrigger=true`. Use an identity local rotation
unless current-build native evidence requires another rotation.

Example editor code:

```csharp
const string exfilName = "PVE_ExfilZone_00";
Vector3 exfilRoot = new Vector3(exfilX, TerrainSurfaceY(exfilX, exfilZ) + 0.03f, exfilZ);

var exfil = new GameObject(exfilName);
exfil.transform.SetParent(mapRoot, false);
exfil.transform.localPosition = exfilRoot;
exfil.transform.localRotation = Quaternion.identity;

var trigger = exfil.AddComponent<BoxCollider>();
trigger.isTrigger = true;
trigger.center = measuredNativeCenter;
trigger.size = measuredNativeSize;
exfil.SetActive(false);
```

`TerrainSurfaceY` MUST sample the same surface representation that the live
TerrainCollider uses. If the runtime reconstructs TerrainData from a payload,
use the matching full-footprint height function. Do not use a smaller visual
core function.

The marker MUST be inactive. Modded Operations finds inactive scene objects,
copies the data to its inactive network prefab, and controls activation.

## 4. Choose the extraction location

The extraction root can be separate from insertion or can occupy the same
area. A shared insertion/extraction area is valid because the zone and global
extraction flags start false.

When the design requires a return to insertion:

1. Find the complete group of player spawn candidates for that insertion.
2. Choose a trigger that covers all valid candidates.
3. Keep the trigger inside the bullet-blocking playable wall.
4. Keep it on a stable terrain surface.
5. Keep enemy spawn candidates outside the trigger.
6. Test the player at every spawn candidate, not only at the group center.

Compute world bounds from the transform, collider center, and collider size:

```csharp
Vector3 worldCenter = exfil.transform.TransformPoint(trigger.center);
Vector3 halfSize = Vector3.Scale(trigger.size * 0.5f, exfil.transform.lossyScale);
Vector3 minimum = worldCenter - halfSize;
Vector3 maximum = worldCenter + halfSize;
```

This simple formula assumes an axis-aligned trigger. For a rotated trigger,
transform all eight local corners and calculate the world AABB. Also test the
real rotated collider in play mode.

## 5. Validate the scene before the build

Fail the editor build when any condition is false:

```text
exactly one name starts with PVE_ExfilZone_
the marker belongs to the accepted map root
the marker has one BoxCollider
BoxCollider.isTrigger is true
all size axes are positive
the root Y matches the runtime TerrainCollider surface within the chosen tolerance
the complete trigger is inside the playable wall
every PVE player spawn intended for insertion is inside the trigger when insertion and extraction share an area
every enemy spawn is outside the trigger
```

Example surface assertion:

```csharp
float expectedY = TerrainSurfaceY(exfilX, exfilZ) + 0.03f;
if (Mathf.Abs(marker.localPosition.y - expectedY) > 0.01f)
    throw new InvalidOperationException("PVE exfil root is not on the live terrain surface.");
```

Write the exact marker record to a machine-readable verifier report:

```text
pveExfilMarker=<name>
position=(<x>,<y>,<z>)
triggerCenter=(<x>,<y>,<z>)
triggerSize=(<x>,<y>,<z>)
timerSeconds=<seconds>
nativeSource=<scene/build fingerprint>
```

## 6. Let the framework create the native runtime objects

Modded Operations validates the scene in
`CerberusNativeTabFix.ValidateStandaloneSceneContract`. It then calls
`ConfigureStandalonePveController`.

The framework performs these operations:

1. Move its network bootstrap to the authored marker transform.
2. Copy the authored BoxCollider center and size.
3. Add the shipped `ExfilZone` component.
4. Initialize `_occupants`, `linkedInfils`, marker references, and locked
   SyncVars.
5. Add the shipped `RaidManager` component.
6. Assign the current `StandalonePveGameMode : InfiltrationManager`.
7. Set `EXTRACT_TIMER=15` and create one-element `exfilZones`.
8. Set `RaidManager.singleton` to the current scene owner.
9. Reset the matching global extraction fields on `GameManagerNetwork`.

Do not call these steps from a map companion. A second caller can create two
singletons, two occupant sets, or two extraction timers.

## 7. Use the native AI death route

Create PVE enemies through:

```csharp
raid.ServerSpawnAI(false);
```

The current native method selects from `RaidManager.standardAI`, instantiates
the bot, spawns it with the required owner, and applies `BotSpawnDetails`.
Manual one-argument Mirror spawning can produce an AI that throws grenades but
does not complete its firearm or death ownership lifecycle.

After `ServerSpawnAI(false)`, restore the current map's one-element
`raid.exfilZones`. The native startup can discover persistent exfil objects in
`Resources`. A standalone map must not keep those donor zones.

Do not create a custom `aliveEnemyCount` as the mission authority. Normal
weapon damage must use the shipped AI `Health` path. The shipped raid observes
the native population and unlocks extraction when the operation is clear.

## 8. Keep zone and global extraction state consistent

Before each operation generation, reset:

```csharp
exfil._occupants.Clear();
exfil.NetworkPlayersInExfil = 0;
exfil.NetworkcanExtract = false;

network._globalExfilOccupants.Clear();
network.NetworkPlayersInAnyExfil = 0;
network.NetworkcanExtract = false;
network.NetworkisExtracting = false;
network.NetworkextractionStartTime = 0d;
network.ExfilTime = 15f;
network.SuccessfulOperation = false;
```

The server owns these values. Clients receive the SyncVars and RPC results.
Do not let a client decide that all AI are dead or that the extraction timer
has completed.

## 9. Reconstruct the current-build ATAK visual

For the current supported build, the vanilla `level16` marker contract is:

| Field | Value |
| --- | --- |
| GameObject | `ATAK Exfil Marker` |
| layer | `17` |
| local rotation quaternion | `(-3.0159049e-7,-0.70710683,-0.70710677,3.2782552e-7)` |
| local scale | `(0.65,0.65,0.65)` |
| mesh name | `Marker` |
| mesh vertices | `4` |
| mesh extents | `(9.6,5.4,0)` |
| triangles | `{2,1,0,3,2,0}` |
| material | `ExfilZone` |
| shader | `HDRP/Unlit` |
| queue | `2501` |
| texture | resident `ExfilZone`, `512x512`, DXT5 |
| texture offset | `(0,-0.22)` |

The exact vertices and UVs are in the Modded Operations source method
`CreateNativeAtakExfilMarker`. The framework finds the installed resident
texture by exact name and dimensions. It does not package the game texture.

Assign the created object to `ExfilZone.ExfilMarker` and leave it inactive.
The shipped unlock path activates it. Record the created `Mesh` and `Material`
as operation-owned assets and destroy them during teardown.

Do not infer that the exfil marker uses `atakMarker`. The audited vanilla
object has only Transform, MeshFilter, and MeshRenderer components.

## 10. Preserve the vanilla success flow

After all enemies are dead, require this order:

```text
native live AI population reaches zero
RaidManager unlocks its current exfilZones
zone and global NetworkcanExtract become true
ExfilMarker becomes active on ATAK
living player enters or remains in the physical trigger
zone and global occupant counts become positive
GameManagerNetwork.NetworkisExtracting becomes true
15-second timer reaches zero
GameManagerNetwork.SuccessfulOperation becomes true
missionCompletedPopup displays the After Action Report
Continue returns to the Operation Room
```

Do not unload the scene when the last enemy dies. Do not display a custom
message in place of the shipped After Action Report.

During successful teardown, preserve `GameManagerNetwork.SuccessfulOperation`.
The Operation Room reads it after the map scene unloads.

## 11. Restart cleanup

For failure, normal restart, and the next operation, remove only current
generation state:

```text
current PVE RaidManager and ExfilZone references
current zone/global occupant sets when the operation was not successful
current network-prefab registration and spawn handler by deterministic asset ID
current runtime ATAK Mesh and Material
current map-scene objects, graph, runtime terrain, and map companion caches
current player handoff destination and generation IDs
```

Keep verified AssetBundles resident when the framework's restart design uses
resident bundles. This makes the shipped Restart Operation path faster and
prevents repeated large-file verification.

Always remove the Mirror prefab by asset ID. Unity can destroy the prefab
object before the unload callback. The C# wrapper then compares as null and
cannot supply its old ID. A stale `NetworkClient.prefabs` entry causes the
repeated `MAP LOADED !BUG!` state.

## 12. Required proof

Static proof is not enough. Use the exact release DLLs and bundle bytes.

Require these observations:

```text
StandardPVE, not a simulation
one current RaidManager
one current ExfilZone in raid.exfilZones
one extraction trigger in the authored position
extraction locked while live AI exist
all AI deaths use native Health behavior
native live population reaches zero
zone and global extraction unlock
ATAK marker is active and matches the current-build mesh/material/texture contract
physical occupant counts become positive
native extraction timer runs for the configured duration
native Mission Successful popup appears
Continue returns to the Operation Room
normal second launch succeeds
failure Restart Operation succeeds
no old Mirror ID, occupant, singleton, graph, player transform, or ATAK asset survives
```

If insertion and extraction share an area, test all insertion candidates. Also
test that no success occurs at initial spawn. Then leave the area, clear the
operation, return, and complete extraction.

## 13. Worked reference

Ukrainian Forest package `0.3.21` uses:

```text
marker=PVE_ExfilZone_00
root=(0.000,0.112,7.000)
center=(1.3259258,2.066852,1.6703243)
size=(25.236944,7.376298,15.531027)
Team 1/PVE insertion groups=z 7 and z 12
ordinary enemy side=z >= 80
timer=15 seconds
```

This value set is an exact worked example. It is not a universal coordinate or
trigger size. Measure the current game and the new map.

See also:

- [Runtime integration](04-runtime-integration.md)
- [Spawn and gameplay](05-spawn-and-gameplay.md)
- [Validation and release](07-validation-and-release.md)
- [AI navigation and behavior](11-ai-navigation-and-behavior.md)
- [Exact implementation reference](13-exact-implementation-reference.md)
- [End-to-end package lifecycle](14-end-to-end-package-lifecycle.md)
