# 9. Interactive prefabs and `DoorV2`

Status of the structural data in this document: `DEVELOPER-SOURCE` for the
official `_DoorV2_BASE.prefab` supplied on 2026-08-03, and `PROVEN-STATIC` for
the current-build code audit.

Status of a door that is an authored instance of that complete prefab:
`SUPPORTED` only after the map passes the test matrix in this document.

Status of a door that a companion creates or reconstructs at run time:
`EXPERIMENTAL`. This is not the normal game method.

Re-check all types and fields after an OPERATOR update.

## Primary finding: the door is authored content

The OPERATOR developers confirmed that doors are part of the prefab or scene.
The game does not create the normal door graph at run time. The official
source file confirms this statement.

Evidence record for the supplied source:

| Item | Value |
| --- | --- |
| File | `_DoorV2_BASE.prefab` |
| Size | `260206` bytes |
| SHA-256 | `BAB5287B2DE809143BBDE71B90F8D0BE454DD724B4DEC110FB4AF1FC0CF06FF6` |
| Meta file size | `154` bytes |
| Meta SHA-256 | `A0A8276DBBEE98B532AA2E8E4392C016C618D6A707899E67F705B100E9B4A2F4` |
| Prefab GUID | `803422c907641034e99a99778ef7d30b` |
| Root name and tag | `_DoorV2_BASE`, tag `Door` |
| `DoorV2` script GUID | `fe92077393168114cbf3320d346950d4` |

Do not publish the developer file or its dependencies unless you have
permission. The hashes and serialized relationships are evidence. They are
not a redistribution license.

The root has three authored children:

- `Door Pivot and rigidbody`;
- `Openable NavMesh Link Source`;
- `Walkable NavMeshLink Source`.

The root also has the Mirror identity, `DoorV2`, `MilkRigidbodySync`, and its
base transform-synchronization behavior. The pivot subtree contains both
handles, the panel rigid body, hit objects, interaction-pose objects, audio,
and the destroyed-door graph. The two navigation-link source objects are
siblings of the pivot. This is one serialized graph. Do not split it into a
visual panel and a run-time component repair pass.

## AssetRipper limitation

AssetRipper can export the visible hierarchy, meshes, colliders, interaction
poses, and MonoBehaviour type references. It can fail to reconstruct the
serialized fields of an IL2CPP script.

Some earlier AssetRipper exports had this failure:

- The `DoorV2` MonoBehaviour block contains the script reference.
- The block contains no custom serialized field values.
- The two `DoorHandleV2` blocks have the same loss.
- Some variants contain a child named `Door Pivot`.
- Other variants do not contain a pivot child.
- A pivot child name does not assign `DoorV2.PivotTransform`.
- Several variants omit `MilkRigidbodySync`, door hit-box behavior, navigation
  links, navigation cuts, destroyed-door parts, or audio data.

This AssetRipper limitation does not describe the official developer prefab.
The official prefab contains the complete non-null serialized graph. If an
export loses these fields, reject the export. Do not treat its missing values
as the native authoring method.

## Use the complete prefab in the map-authoring project

1. Import the complete authorized prefab and its `.meta` file into the Unity
   authoring project. Preserve the GUID.
2. Import or resolve every script and asset dependency before you open or
   save the prefab. A missing script can cause destructive reserialization.
3. Create a prefab instance in the map scene or in a map-owned building
   prefab. Do not copy only the panel.
4. Keep the complete pivot, handles, hit parts, interaction poses, destroyed
   parts, audio, physics synchronization, cut, and both link sources.
5. Position the complete root. Change a child only when the source contract
   for that child is known.
6. Bake or scan the map-owned A* graph with both link endpoints on valid graph
   surfaces.
7. Build the scene bundle with this authored prefab instance present.
8. Let normal scene load initialize the graph. Do not spawn the normal door
   from a BepInEx companion.

Do not assign a duplicate Mirror scene identity or invent a separate network
spawn route. A dynamic run-time clone is a different architecture. It needs
explicit server and client registration proof and stays `EXPERIMENTAL`.

## Run the executable source-graph validator

Use
[`templates/Editor/ValidateDoorV2Prefab.cs`](../templates/Editor/ValidateDoorV2Prefab.cs)
before you place or build the door.

1. Copy the file to `Assets/Editor/ValidateDoorV2Prefab.cs` in the authoring
   project.
2. Set `PrefabAssetPath` to the authorized prefab asset path. The default is
   `Assets/Prefabs/_DoorV2_BASE.prefab`.
3. Keep `RequireOfficialSourceGuid=true` for the unmodified developer source.
   Set it to `false` only after you create an authorized variant with a new
   `.meta` GUID.
4. Keep `RequireOfficialScalarValues=true` unless the written variant
   specification changes a pinned scalar.
5. Run **OPERATOR Map > Validate DoorV2 Prefab**.
6. Require `SUMMARY errors=0` in
   `Builds/OperatorDoorValidation/doorv2-prefab-validation.txt`.

The validator uses `SerializedObject.FindProperty` with the exact field names
in this chapter. It has no compile-time reference to private OPERATOR script
types. It checks these items:

- inactive source root and root tag `Door`;
- zero missing MonoBehaviour scripts;
- exactly one `NetworkIdentity`, `DoorV2`, `DoorHitBox`,
  `MilkRigidbodySync`, and `NavmeshCut`;
- exactly two `DoorHandleV2` and `NodeLink2` components;
- exactly three `ShootableDoorPart` components;
- every required `DoorV2` reference is non-null, has the expected type, and
  points into this prefab graph when the field is graph-local;
- front and back handles are distinct, reciprocal, owned by the same door,
  and have opposite `IsFrontHandle` values;
- each handle has local `myPushObject`, `Handle`, and `Center` references;
- `MyLocalPlayer` and `PlayerInteractionSystem` are null source state;
- the rigid body and both handles move below `PivotTransform`;
- openable and walkable links are distinct;
- unlock, locked, close, thud, and breach arrays are non-empty and contain no
  null entry;
- a present `DestroyedDoor` is local and has a non-empty, non-null
  `DestroyedDoorRB` array;
- the official GUID and the pinned values `DoorMask=4545`,
  `PlayerMovementLayerMask=33554436`, `maxRotationY=110`, and `Damping=0.5`
  when their two policy switches are enabled.

This editor result is a structural gate. It cannot prove that link endpoints
attach to the live A* graph, that FinalIK interaction works, or that Mirror,
damage, breach, late join, restart, and unload behave correctly. The live
matrix below remains mandatory.

## Required `DoorV2` object graph

Use this graph as a checklist. Exact object names can differ. Exact reference
types cannot differ.

```text
Door root (initially inactive)
|-- Mirror.NetworkIdentity
|-- DoorV2
|-- pivot Transform at the hinge axis
|   |-- door model parent
|   |-- physical door panel
|   |   |-- Rigidbody
|   |   |-- BoxCollider
|   |   `-- MilkRigidbodySync
|   |-- front DoorHandleV2
|   |-- back DoorHandleV2
|   |-- center interaction object
|   |-- latch/lock collider and ShootableDoorPart
|   |-- top hinge collider and ShootableDoorPart
|   |-- bottom hinge collider and ShootableDoorPart
|   |-- AudioSource
|   `-- optional destroyed-door hierarchy and rigid bodies
|-- NavmeshCut
|-- walkable NodeLink2 and endpoint transforms
`-- openable/breachable NodeLink2 and endpoint transforms
```

The pivot MUST be on the physical hinge axis. The panel, handles, lock,
hinges, audio source, and interaction poses MUST move under that pivot. Do not
put the pivot at the geometric center of the panel.

## `DoorV2` reference fields

The official prefab serializes these `DoorV2` relationships:

| Field | Type | Official prefab file ID | Relationship |
| --- | --- | --- | --- |
| `PivotTransform` | `Transform` | `5655903686974966660` | `Door Pivot and rigidbody` |
| `HandleFront` | `DoorHandleV2` | `832021001818567380` | `Handle01`; `IsFrontHandle=1` |
| `HandleBack` | `DoorHandleV2` | `4888901979659399756` | `Handle02`; `IsFrontHandle=0` |
| `DoorModelParent` | `GameObject` | `4558437591915724262` | Preserved authored model-parent relationship |
| `rb` | `Rigidbody` | `8147755104391239385` | Physical door-panel rigid body |
| `DoorPhysicsSync` | `MilkRigidbodySync` | `3606556160028973740` | Root authority and rigid-body synchronization |
| `DoorPhysicsMaterial` | `PhysicsMaterial` | asset GUID `cac3bd50a9182c84fb18cd880ba8f477` | Shared door physics material |
| `DoorHitBox` | `BoxCollider` | `5080948964878468503` | Main panel collider |
| `latchCollider` | `BoxCollider` | `204089381160518279` | Latch collider |
| `HingeTopCollider` | `BoxCollider` | `1421661395952267286` | Top hinge collider |
| `HingeBottomCollider` | `BoxCollider` | `1695107359955193892` | Bottom hinge collider |
| `NavMeshCut` | `Pathfinding.NavmeshCut` | `8569563404815454737` | Dynamic navigation cut |
| `DoorWalkableNavLink` | `Pathfinding.NodeLink2` | `6111292927718465430` | `Walkable NavMeshLink Source` |
| `DoorOpenableNavLink` | `Pathfinding.NodeLink2` | `3900576627820490214` | `Openable NavMesh Link Source` |
| `audioSource` | `AudioSource` | `3977039335228634899` | Door-local audio source |
| `doorBreach` | `AudioClip[]` | Compatible breach sounds |
| `doorClose` | `AudioClip[]` | Compatible close sounds |
| `doorLocked` | `AudioClip[]` | Compatible locked sounds |
| `doorThud` | `AudioClip[]` | Compatible impact sounds |
| `doorUnlock` | `AudioClip[]` | Compatible unlock sounds |
| `lockedMesh` | `GameObject[]` | Visual objects for the locked state |
| `unlockedMesh` | `GameObject[]` | Visual objects for the unlocked state |
| `DestroyedDoor` | `GameObject` | Optional destroyed-door graph |
| `DestroyedDoorRB` | `Rigidbody[]` | Rigid bodies in the destroyed-door graph |

The official prefab also serializes `DoorMask=4545`,
`PlayerMovementLayerMask=33554436`, `maxRotationY=110`, `Damping=0.5`, latch
and hinge health `400`, `canLatch=1`, `IsLatched=1`, two breach clips, complete
unlock/lock/close/thud arrays, one destroyed-door root, and 30 destroyed-door
rigid bodies. Preserve these values until a specific authored door variant
proves a different value.

Do not assign one navigation link to both navigation fields. Do not leave an
empty array where current code indexes a sound or rigid body.

These authored controls also exist:

| Control | Purpose |
| --- | --- |
| `StartLocked` and `StartLockedChance` | Initial lock state and probability |
| `AiCantOpen` and `AiCantOpenChance` | AI opening restriction |
| `Invert` | Native direction convention |
| `canLatch` and `LatchHealth` | Latch behavior and strength |
| `canBlowup` | Explosive-destruction permission |
| `breakOnceMaxRotation` and `maxRotationY` | Rotation limit and break behavior |
| `Damping` and `PredictionDamping` | Physics and client-prediction damping |
| `deadDoorDamping` and `deadDoorAngularDamping` | Destroyed-door damping |
| `deadDoorScrollForce`, `deadDoorSpringStrength`, `deadDoorWalkForce` | Destroyed-door interaction forces |
| `ColliderStartSize` and `ColliderTargetSize` | Collider transition sizes |
| `navCutCloseSize` and `navCutOpenSize` | Navigation-cut sizes by door state |
| `DoorMask` and `PlayerMovementLayerMask` | Installed layer masks |

Copy variant-specific values from an authorized complete source prefab. Do
not guess values from the visible mesh.

## Fields that current code does not consume

The current-build developer code audit identifies serialized compatibility
data that is not read by the active code path:

- `DoorModelParent` is referenced only by commented-out
  `TryAutoFindCollider` code.
- `DoorMask` is referenced only by a commented raycast. Its official value is
  `4545`, which is not a meaningful `LayerMask` by itself.
- `navCutOpenSize` and `navCutCloseSize` are referenced only by the
  commented-out cut-resize block in `HandleAIBlockers`.
- `DoorHandleV2.RivalDoorHandle` has no current reads. The official handles
  still serialize the reciprocal relationship.
- `DoorHandleV2.allowedDistanceToPlayerDamper` has no current reads.
- `DoorHandleV2.GrabbedHandle` is write-only. `GrabbedCenter` is read and is
  not dead.
- `DoorHandleV2.raycastTransform` is allocated in `Start` as an empty
  `GameObject`, but current code does not use it.

Preserve these serialized fields. Removing a public serialized field changes
the prefab data contract across approximately 40 existing door prefabs and
cannot recover the data automatically later.

Do not classify `latchCollider`, `HingeTopCollider`, or
`HingeBottomCollider` as dead. `SlapChargeExplosive` reads them to select the
nearest breach point. `NavMeshCut`, `canBlowup`, and the dead-door block are
also live. `NavMeshCut` is disabled in `DoorDie`, and `DoorHandleV2` consumes
the dead-door state.

These fields are runtime state. Do not use them as authoring inputs:

`CurrentlyLocked`, `IsLatched`, `NetworkCurrentlyLocked`, `NetworkinUse`,
`PredictedRotation`, `TimeSinceLastClientPrediction`, `Velocity`,
`cachedExplosionPosition`, `currentRotation`, `currentTick`, `hinge01_Health`,
`hinge02_Health`, `inUse`, `initialHinge2Health`, `initialHingeHealth`,
`initialLockHealth`, `isDead`, `lastDoorCloseTime`, `lastFrameProcessed`,
`lastRecTick`, `lastReceiveTime`, `prevServRot`, `previousValue`,
`reachedFullSize`, and `targetValue`.

The runtime also caches destroyed-part initial positions and rotations. Let
the native lifecycle initialize those arrays after the graph is valid.

## `DoorHandleV2` wiring

Create one handle component for each side.

| Field | Required value |
| --- | --- |
| `doorV2` | The owning `DoorV2` |
| `RivalDoorHandle` | The handle on the other side |
| `IsFrontHandle` | `true` on exactly one side |
| `Handle` | The side-specific FinalIK `InteractionObject` for handle use |
| `Center` | The side-specific FinalIK `InteractionObject` for center push/pull |
| `myPushObject` | Native-template push target transform |

The official `Handle01` uses `myPushObject=4371644465376006511`,
`Handle=5588754310984811820`, and `Center=3606588516118885588`. Official
`Handle02` uses the same push object, `Handle=4333291165372661062`, and
`Center=6390603237945870828`.

Preserve the reciprocal rival relationship even though current code does not
read it. Keep each FinalIK interaction-pose hierarchy. Do not remove its hand
and finger pose children as decorative objects.

`MyLocalPlayer` and `PlayerInteractionSystem` are correctly null in the
official prefab. The distance caches and grab flags start at zero. These are
runtime state. Do not bind them to an editor object.
The native handle exposes `TryStartInteraction`, `StartHandleInteraction`,
`StartCenterInteraction`, `StopInteraction`, and force-calculation methods.
Do not call these methods to compensate for an incomplete graph.

## Damage-part wiring

Use `ShootableDoorPart` on the latch and hinge damage objects.

| Field | Required value |
| --- | --- |
| `Door` | The owning `DoorV2` |
| `PartID` | The native-template part identifier for latch, top hinge, or bottom hinge |

The installed build also exposes `DoorHitBox.Door`. If the native template
uses this component, bind it to the same owning door.

Do not infer `PartID` from child order. Copy it from a verified compatible
native template.

## Navigation and AI wiring

OPERATOR bots use the A* Pathfinding Project. Unity NavMesh data alone does
not make this door traversable.

1. Put link endpoints on opposite sides of the door.
2. Keep endpoints on the live map-owned graph.
3. Bind each `NodeLink2` to its exact endpoint transform.
4. Copy `pathfindingTag`, `graphMask`, `oneWay`, and `costFactor` from a
   compatible native door.
5. Bind the two verified door links to the correct `DoorV2` fields.
6. Size and position `NavmeshCut` from the closed panel footprint.
7. Verify that door state changes update the cut and link mode.

Native bot traversal uses door walk, open, and breach modes. The native
`BotOffmeshLinkHandler` controls the traversal sequence and door cooldowns.
Do not add a second custom AI door state machine to the map.

## Experimental run-time reconstruction sequence

The normal door is already part of the authored map prefab. Use this sequence
only for research when no authorized complete source graph is available:

1. Verify the exact package ID, map ID, scene path, build, and dependency
   versions.
2. Create or clone the complete door root while it is inactive.
3. Create the hinge pivot and complete moving hierarchy.
4. Create or resolve all colliders, rigid bodies, audio, materials, damage
   parts, interaction objects, and navigation objects.
5. Add the native components while the root is inactive.
6. Assign all reference fields.
7. Copy all authored controls from one compatible native template.
8. Check that no required reference is null or fake-null.
9. Check that every internal reference points to this door graph.
10. Check unique Mirror identity and server ownership.
11. Activate the root.
12. Let Unity and Mirror call `Awake`, `Start`, `OnStartServer`, and
    `OnStartClient` in their normal order.
13. Use the verified server spawn path when the object is dynamic.

Do not call private lifecycle methods such as `DoorStart` or
`SetDoorPhysics`. Do not activate the graph and then add the missing
references.

## Required test matrix

A door is not `SUPPORTED` until all applicable rows pass.

| Test | Required result |
| --- | --- |
| Player front interaction | Handle selection, IK reach, push, pull, release |
| Player back interaction | Same result from the other side |
| Latch and lock | Correct locked, unlock, latch, and feedback behavior |
| Collision | Closed door blocks; open door gives correct clearance |
| Bullet damage | Latch and both hinges route damage to the owning door |
| Breach | Native breach force, sound, damage, and state transition |
| Destroyed state | Parts, rigid bodies, collision, and audio remain valid |
| AI open | Bot uses the native open link and does not stall |
| AI breach | Bot uses the native breach path when required |
| Navigation cut | Closed/open state updates traversability |
| Host | Server owns authoritative state |
| Client | State, physics, sound, and interaction agree with host |
| Late join | New client receives the current door state |
| Restart | One fresh door exists; no stale identity, link, or callback remains |
| Scene unload | Map-owned door, graph links, and callbacks are removed |

If only the visible panel swings, report a partial visual result. Do not report
a native door result.

## Common failure signatures

| Symptom | Probable cause |
| --- | --- |
| Door does not move | Null pivot, rigid body, physics sync, or invalid lifecycle order |
| Panel rotates around its center | Pivot is not on the hinge axis |
| Interaction prompt appears but action fails | Incomplete paired handles, FinalIK objects, or player interaction resolution |
| Only one side works | Rival link, front/back flag, or side-specific interaction objects are wrong |
| Bullets do not damage latch or hinges | Missing `ShootableDoorPart` owner or wrong `PartID` |
| AI stops at the doorway | Missing A* link, wrong tag, endpoint off graph, or stale navigation cut |
| Host works but client snaps | Invalid `NetworkIdentity`, missing `MilkRigidbodySync`, or unproved spawn registration |
| Restart creates two doors | Companion did not remove its scene-generation objects and callbacks |

Do not repair these failures with an animation-only replacement. Fix the
missing native relationship or keep the door non-interactive.
