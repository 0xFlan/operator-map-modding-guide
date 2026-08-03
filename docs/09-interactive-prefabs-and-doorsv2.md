# 9. Interactive prefabs and `DoorV2`

Status of the structural data in this document: `PROVEN-STATIC` for the
inspected current build.

Status of a complete native-template clone: `SUPPORTED` only after the map
passes the test matrix in this document.

Status of a door that a companion reconstructs from individual components:
`EXPERIMENTAL` until the same test matrix passes.

Re-check all types and fields after an OPERATOR update.

## The AssetRipper prefab is not a functional door

AssetRipper can export the visible hierarchy, meshes, colliders, interaction
poses, and MonoBehaviour type references. It can fail to reconstruct the
serialized fields of an IL2CPP script.

The inspected `DoorV2` prefab exports have this failure:

- The `DoorV2` MonoBehaviour block contains the script reference.
- The block contains no custom serialized field values.
- The two `DoorHandleV2` blocks have the same loss.
- Some variants contain a child named `Door Pivot`.
- Other variants do not contain a pivot child.
- A pivot child name does not assign `DoorV2.PivotTransform`.
- Several variants omit `MilkRigidbodySync`, door hit-box behavior, navigation
  links, navigation cuts, destroyed-door parts, or audio data.

Thus, the object can look like a door but still have null references. Unity
does not restore these values from child names. A late assignment cannot
reverse all work that `Awake`, `Start`, or Mirror network-start methods did
with invalid data.

## Use a native runtime template when possible

The preferred method is a clone of one verified, complete, live `DoorV2`
object from the same installed build.

1. Find a source door that has the required interaction and network behavior.
2. Record its complete child hierarchy and all component references.
3. Disable the source clone before a map-specific change.
4. Clone the complete root. Do not clone only the visible panel.
5. Verify that each cloned reference points to an object in the cloned graph.
6. Replace only the approved visual mesh, materials, dimensions, and local
   transforms.
7. Keep the native component types and their paired relationships.
8. Give every dynamic network object a valid, unique Mirror identity through
   a verified server-owned spawn path.
9. Activate the clone only after the object graph is complete.

Do not duplicate a scene `NetworkIdentity.sceneId`. Do not assume that a
runtime clone is registered on a remote client. A local host test does not
prove peer spawn, authority transfer, late join, or restart.

If no safe native source exists in the standalone session, use an exact-scene
companion to reconstruct the object. Keep this path `EXPERIMENTAL` until the
full matrix passes.

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

The installed current-build interop exposes these object-reference fields:

| Field | Type | Required relationship |
| --- | --- | --- |
| `PivotTransform` | `Transform` | Hinge-axis transform that rotates the complete moving door graph |
| `DoorModelParent` | `GameObject` | Native-template model parent for the moving door |
| `rb` | `Rigidbody` | Physical door-panel rigid body |
| `DoorPhysicsSync` | `MilkRigidbodySync` | OPERATOR authority and rigid-body synchronization component |
| `DoorPhysicsMaterial` | `PhysicsMaterial` | Physics material copied from a compatible native door |
| `DoorHitBox` | `BoxCollider` | Main physical/hit collider expected by the door |
| `latchCollider` | `BoxCollider` | Latch or lock collider |
| `HingeTopCollider` | `BoxCollider` | Top hinge collider |
| `HingeBottomCollider` | `BoxCollider` | Bottom hinge collider |
| `HandleFront` | `DoorHandleV2` | Front interaction component |
| `HandleBack` | `DoorHandleV2` | Back interaction component |
| `NavMeshCut` | `Pathfinding.NavmeshCut` | Dynamic navigation obstacle for the closed door |
| `DoorWalkableNavLink` | `Pathfinding.NodeLink2` | Link for the verified walk-through state |
| `DoorOpenableNavLink` | `Pathfinding.NodeLink2` | Link for the verified open/breach state |
| `audioSource` | `AudioSource` | Door-local audio source |
| `doorBreach` | `AudioClip[]` | Compatible breach sounds |
| `doorClose` | `AudioClip[]` | Compatible close sounds |
| `doorLocked` | `AudioClip[]` | Compatible locked sounds |
| `doorThud` | `AudioClip[]` | Compatible impact sounds |
| `doorUnlock` | `AudioClip[]` | Compatible unlock sounds |
| `lockedMesh` | `GameObject[]` | Visual objects for the locked state |
| `unlockedMesh` | `GameObject[]` | Visual objects for the unlocked state |
| `DestroyedDoor` | `GameObject` | Optional destroyed-door graph |
| `DestroyedDoorRB` | `Rigidbody[]` | Rigid bodies in the destroyed-door graph |

Do not assign one navigation link to both navigation fields unless a
current-build native reference door uses that relationship. Do not leave an
empty array where runtime code expects an indexed sound or rigid body.

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

Copy these values from one compatible native template. Do not guess values
from the visible mesh.

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
| `raycastTransform` | Native-template ray origin and direction transform |
| `myPushObject` | Native-template push target transform |
| `allowedHandleDistanceToPlayer` | Distance copied from a compatible native template |
| `allowedCenterDistanceToPlayer` | Distance copied from a compatible native template |
| `allowedDistanceToPlayerPull` | Pull distance copied from a compatible native template |
| `allowedDistanceToPlayerPush` | Push distance copied from a compatible native template |
| `allowedDistanceToPlayerDamper` | Damping distance copied from a compatible native template |

Set the rival relationship in both directions. Keep each FinalIK interaction
pose hierarchy. Do not remove its hand and finger pose children as decorative
objects.

`MyLocalPlayer`, `PlayerInteractionSystem`, distance caches, grab flags, and
best-handle selection are runtime state. Do not bind them to an editor object.
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

## Safe reconstruction sequence

Use this sequence only in an exact-scene companion:

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
