# OPERATOR interactive prefab authoring

Use this reference before work on `DoorV2`, `DoorHandleV2`, FinalIK door
interaction objects, door damage parts, or A* door links.

Re-check the installed interop and an authorized complete source after each
game update.

## Native ownership

The OPERATOR developers state that normal doors are part of the map or
building prefab. The game does not create their complete graph at run time.

The official `_DoorV2_BASE.prefab` inspected on 2026-08-03 is 260206 bytes
with SHA-256
`BAB5287B2DE809143BBDE71B90F8D0BE454DD724B4DEC110FB4AF1FC0CF06FF6`.
Its meta GUID is `803422c907641034e99a99778ef7d30b`. It contains the
Mirror identity, `DoorV2`, `MilkRigidbodySync`, pivot and rigid body, two
handles, FinalIK pose graphs, latch and hinge damage objects, audio, destroyed
parts, `NavMeshCut`, and separate openable and walkable `NodeLink2` sources.

Do not publish the developer prefab or its dependencies without permission.
Hashes and relationships are evidence, not a redistribution license.

## Required method

1. Import the authorized prefab and its original `.meta` file with every
   dependency resolved.
2. Stop if Unity reports a missing script. Do not save a damaged prefab.
3. Copy `templates/Editor/ValidateDoorV2Prefab.cs` to `Assets/Editor`, set
   `PrefabAssetPath`, and run **OPERATOR Map > Validate DoorV2 Prefab**.
4. Place an instance in the map scene or a map-owned building prefab.
5. Preserve the complete physics, interaction, damage, audio, destroyed,
   navigation-cut, and link-source graph.
6. Put both `NodeLink2` endpoints on the map-owned A* graph.
7. Ship the door as authored scene content.
8. Let normal scene and Mirror lifecycle code initialize it.

Do not spawn the normal door from a companion. Do not call private lifecycle
methods. Do not duplicate a Mirror scene identity.

## Exact official reference graph

- `DoorV2=7643917969817043405`;
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
- `audioSource=3977039335228634899`;
- `DestroyedDoor=2211682661131321721`.

`Handle01` is front. It binds `Handle02`, push object
`4371644465376006511`, handle pose `5588754310984811820`, and center pose
`3606588516118885588`. `Handle02` binds `Handle01`, the same push object,
handle pose `4333291165372661062`, and center pose
`6390603237945870828`.

`MyLocalPlayer` and `PlayerInteractionSystem` are correctly null in the
prefab. They are run-time state.

The validator resolves these exact serialized field names without a
compile-time dependency on private OPERATOR types. It fails on a missing
script, wrong component count, null or external graph reference, nonreciprocal
handle pair, shared navigation link, empty sound array, wrong official source
GUID, or wrong pinned official scalar. It emits
`Builds/OperatorDoorValidation/doorv2-prefab-validation.txt`. It cannot prove
live A* attachment, Mirror ownership, FinalIK, damage, restart, or unload.

## Current-code field use

Preserve serialized compatibility fields even when the current build does not
read them:

- `DoorModelParent`, `DoorMask`, `navCutOpenSize`, and `navCutCloseSize` occur
  only in commented-out code.
- `RivalDoorHandle` and `allowedDistanceToPlayerDamper` have no current reads.
- `GrabbedHandle` is write-only. `GrabbedCenter` is live.
- `raycastTransform` is an unused object allocated in `DoorHandleV2.Start`.
- latch and hinge colliders are live in `SlapChargeExplosive`.
- `NavMeshCut`, `canBlowup`, and the dead-door block are live.

Do not remove public serialized fields from source scripts. Existing door
prefabs contain values that cannot be recovered automatically after deletion.

## AssetRipper boundary

Some AssetRipper exports keep type references but lose the custom serialized
`DoorV2` and `DoorHandleV2` fields. Reject such an export as a functional door
source. A named pivot child does not bind a null `PivotTransform`.

If no authorized complete prefab is available, a live-template clone or
component reconstruction is experimental. Build its complete graph inactive,
prove network registration, and then allow normal lifecycle activation.

## Exact-build recovery exception

Use this only when the user explicitly requests reconstruction because the
complete authorized Unity dependencies are unavailable. Pin the installed
Unity version plus executable, `GameAssembly.dll`, and `UnityPlayer.dll`
fingerprints. Reject other builds instead of guessing across an update.

Recover from two independent sources:

1. Parse the official prefab YAML for serialized references, component order,
   scalar values, FinalIK curves, audio order, and source GUIDs.
2. Audit a current installed scene instance for the live hierarchy, added
   meshes, colliders, hit boxes, fracture pieces, and component closure.

Do not treat an exported prefab candidate, a mesh name, or a pivot name as
proof. The audited residential interior instance in the pinned 2026-08 build
has 119 transforms and 316 components. Its functional closure includes one
`DoorV2`, one `MilkRigidbodySync`, two `DoorHandleV2`, four FinalIK
`InteractionObject`, four `InteractionTarget`, three `ShootableDoorPart`, two
`DoorHitBox`, two `NodeLink2`, one `NavmeshCut`, 31 rigid bodies including the
pivot, 30 breached-piece bodies, 35 box colliders, and one audio source.

Preserve these non-obvious bindings:

- root order: `NetworkIdentity`, `DoorV2`,
  `ExcludeFromMirrorSpawnable`, `MilkRigidbodySync` after `Transform`;
- `Center` is the shared push transform;
- `Handle01` is front and uses `InteractionL (4)` plus
  `InteractionL Centre`; `Handle02` uses `InteractionL (3)` plus
  `InteractionL Centre 2`;
- damage IDs are lock `1`, top hinge `2`, bottom hinge `3`;
- sound arrays contain opening `10`, locked `10`, closing `10`, thud `15`,
  and breach `2` clips in numeric order;
- the openable link uses tag `2`; the walkable link uses tag `1`; both use
  graph mask `-1` and separate endpoints;
- the destroyed rigid-body array contains exactly 30 bodies.

For this exact-build recovery path, make every recovered door clip resident
before the shell can be used. Import the opening, locked, closing, thud, and
breach clips as decompressed-on-load audio, disable background loading, and
preload all 47 unique clips while the reconstructed graph is still inactive.
This prevents the first few door interactions from paying synchronous decode
or load cost. Treat it as a pinned reconstruction requirement, not a universal
rule for authorized native door prefabs.

Construct every native gameplay component while the portable shell is
inactive. Reject the shell on any null internal reference, count mismatch,
missing audio clip, visible built-in primitive mesh, or unresolved physics
material. Let `Awake` and `Start` run through activation; do not invoke private
lifecycle methods to compensate for a partial graph.

Treat Mirror as a separate closure. An authoritative instance may use the
audited vanilla asset ID only after proving registration and component order.
A remote client must not also activate its portable shell when Mirror will
instantiate the registered vanilla prefab, or duplicate doors will result.
Keep authoritative spawn and client provisioning behind separate gates and
test host, remote client, and late join before promotion.

Record the source-prefab hash, installed-build fingerprint, scene-instance
audit, recovered dependency manifest, static validation, build log, and live
probe separately. Static graph closure is not gameplay proof.

## Promotion matrix

Require front and back interaction, FinalIK, push/pull/release, lock/latch,
collision, bullet and slap-charge damage, breach, destroyed state, AI open,
AI breach, navigation-cut updates, server authority, remote client, late join,
restart, and scene-unload teardown.
