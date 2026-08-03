# Interactive prefab reference

## AssetRipper boundary

The inspected current-build `DoorV2` and `DoorHandleV2` exports retain script
types but lose their custom serialized field values. Some variants have a
`Door Pivot` child. The `PivotTransform` field is still unassigned.

Treat an exported interactive prefab as an evidence shell.

## Preferred method

Clone one complete compatible live native template from the exact installed
build. Clone the complete root. Keep it inactive during map changes. Verify
that all child references remap into the clone.

Do not duplicate a Mirror scene identity. A dynamic object needs a verified
server spawn and client registration path.

## Critical `DoorV2` fields

Wire `PivotTransform`, `DoorModelParent`, `rb`, `DoorPhysicsSync`,
`DoorPhysicsMaterial`, `DoorHitBox`, `latchCollider`, both hinge colliders,
both `DoorHandleV2` components, `NavMeshCut`, both `NodeLink2` fields,
`audioSource`, sound arrays, locked/unlocked meshes, and optional destroyed
door data.

Copy authored lock, latch, AI, damage, damping, rotation, collider,
navigation-cut, and layer-mask values from one compatible native template.

## Handle and damage graph

Bind each handle to the door. Bind `RivalDoorHandle` in both directions. Set
`IsFrontHandle` on exactly one side. Preserve and bind the side-specific
FinalIK handle and center interaction objects, their hand-pose children,
raycast transform, push transform, and allowed distances.

Bind each `ShootableDoorPart` to the owner. Copy each latch or hinge `PartID`
from the template. Bind `DoorHitBox.Door` when present.

## Navigation graph

Put both link endpoints on the live A* graph. Copy link tags, masks, direction,
and cost from the template. Bind walkable and openable links to their exact
door fields. Size the navigation cut from the closed panel.

Native `BotOffmeshLinkHandler` owns bot open and breach traversal.

## Lifecycle

When reconstruction is required, create the complete inactive graph first.
Add components and assign all references before activation. Then let Unity and
Mirror run `Awake`, `Start`, and network-start methods normally.

Do not call private lifecycle methods. Do not claim support until two-sided
interaction, IK, physics, lock/latch, damage, breach, destroyed state, AI
open/breach, host, remote client, late join, restart, and unload pass.
