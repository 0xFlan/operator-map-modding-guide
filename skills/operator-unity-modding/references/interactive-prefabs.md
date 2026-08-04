# Interactive prefab reference

## Primary `DoorV2` rule

The OPERATOR developers state that normal doors are part of the map or
building prefab. The game does not create their complete object graph at run
time. Use an authorized complete source prefab as authored scene content.

The inspected official `_DoorV2_BASE.prefab` is 260206 bytes and has SHA-256
`BAB5287B2DE809143BBDE71B90F8D0BE454DD724B4DEC110FB4AF1FC0CF06FF6`.
Its meta GUID is `803422c907641034e99a99778ef7d30b`. The source contains
the root Mirror identity, `DoorV2`, `MilkRigidbodySync`, pivot and rigid body,
two cross-linked handles, hit parts, FinalIK pose graphs, audio, destroyed
parts, `NavMeshCut`, and distinct openable and walkable `NodeLink2` sources.

Do not redistribute developer source files without permission. A hash and a
serialized relationship are evidence, not a redistribution license.

## Authoring method

1. Import the authorized prefab and its `.meta` file with all dependencies.
2. Preserve the prefab GUID and complete hierarchy.
3. Create a prefab instance in the map scene or a map-owned building prefab.
4. Keep the complete pivot, physics, interaction, damage, audio, destroyed,
   navigation-cut, and navigation-link graphs.
5. Put both link endpoints on the map-owned A* graph.
6. Ship the door as authored scene content.
7. Let normal scene and Mirror lifecycle code initialize it.

Do not spawn the normal door graph from the companion. Do not call private
lifecycle methods. Do not duplicate a Mirror scene identity.

## Exact official relationships

Preserve these `DoorV2` file-ID bindings:

- `PivotTransform=5655903686974966660`;
- `HandleFront=832021001818567380`;
- `HandleBack=4888901979659399756`;
- `DoorModelParent=4558437591915724262`;
- `rb=8147755104391239385`;
- `DoorPhysicsSync=3606556160028973740`;
- `DoorHitBox=5080948964878468503`;
- `latchCollider=204089381160518279`;
- `HingeTopCollider=1421661395952267286`;
- `HingeBottomCollider=1695107359955193892`;
- `DoorOpenableNavLink=3900576627820490214`;
- `DoorWalkableNavLink=6111292927718465430`;
- `NavMeshCut=8569563404815454737`;
- `audioSource=3977039335228634899`.

`Handle01` is the front handle. It binds the owner door, `Handle02`, push
object `4371644465376006511`, handle pose `5588754310984811820`, and center
pose `3606588516118885588`. `Handle02` binds the owner door, `Handle01`, the
same push object, handle pose `4333291165372661062`, and center pose
`6390603237945870828`.

`MyLocalPlayer` and `PlayerInteractionSystem` are correctly null in the
prefab. The game assigns them at run time.

## Current-code dead-field rule

Preserve serialized compatibility fields even when current code does not
read them. The current developer code audit gives these results:

- `DoorModelParent`, `DoorMask`, `navCutOpenSize`, and `navCutCloseSize` are
  used only by commented-out code.
- `RivalDoorHandle` and `allowedDistanceToPlayerDamper` have no current reads.
- `GrabbedHandle` is write-only; `GrabbedCenter` is live.
- `raycastTransform` is an unused object allocated by `DoorHandleV2.Start`.
- latch and hinge colliders are live in `SlapChargeExplosive`.
- `NavMeshCut`, `canBlowup`, and the dead-door block are live.

Do not remove public serialized fields from the source script or prefab.

## AssetRipper boundary

Some AssetRipper exports lose all custom serialized values from `DoorV2` and
`DoorHandleV2`. Reject that export as a functional door source. A child named
`Door Pivot` does not repair a null `PivotTransform` reference.

If an authorized complete prefab is unavailable, a complete live-template
clone or component reconstruction is experimental. Build the graph while it
is inactive, bind all internal references, and prove its server/client
registration before activation.

## Release gate

Require two-sided interaction, IK, physics, latch, lock, bullet damage,
breach, destroyed state, AI open and breach, host, remote client, late join,
restart, and unload tests.
