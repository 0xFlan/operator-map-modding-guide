# OPERATOR standalone map runtime ownership

Use this reference for a map selected through Modded Operations and loaded as
its own scene. It records the validated boundary between declarative packages,
the **OPERATOR: Modded Operations — Standalone Map Framework**, and optional
map-scoped runtime code.

Re-check installed types, signatures, versions, and behavior after every
OPERATOR, BepInEx, Il2CppInterop, or A* update.

## Contents

1. [Ownership model](#ownership-model)
2. [Activation and startup order](#activation-and-startup-order)
3. [Portable material transport](#portable-material-transport)
4. [Runtime Terrain and collision](#runtime-terrain-and-collision)
5. [Runtime A* graph construction](#runtime-a-graph-construction)
6. [Mission-marker grounding](#mission-marker-grounding)
7. [World contract](#world-contract)
8. [Fixed PVE AI profile and foliage sight](#fixed-pve-ai-profile-and-foliage-sight)
9. [Native Standard PVE completion, extraction, and ATAK](#native-standard-pve-completion-extraction-and-atak)
10. [Selected-map cold-load prefetch](#selected-map-cold-load-prefetch)
11. [Manifest-driven multi-scene variants](#manifest-driven-multi-scene-variants-in-modded-operations-0324)
12. [Host player-spawn boundary](#host-player-spawn-boundary)
13. [PVP team-spawn identity](#pvp-team-spawn-identity)
14. [Exact peer agreement and scene generations](#exact-peer-agreement-and-scene-generations)
15. [Restart and teardown](#restart-and-teardown)
16. [Stationary player-camera observer QA](#stationary-player-camera-observer-qa)
17. [Release layout](#release-layout)
18. [Validation sequence](#validation-sequence)
19. [Rejected shortcuts](#rejected-shortcuts)

## Ownership model

Treat a standalone map as up to four coordinated owners:

| Owner | Owns | Must not own |
|---|---|---|
| Core/catalog | package verification, immutable identity, deterministic catalog | map-specific Unity or game code |
| OPERATOR: Modded Operations framework | native laptop/row/board clones, infiltration selector, exact dependency/scene load, readiness, vanilla-compatible mode ownership, package-declared PVE population range, generic player/population, native PVE extraction state and ATAK presentation, shipped failure/success-UI handoff, restart | map names, map shaders, map graph dimensions, map marker coordinates/repair, cloned mission-failure or mission-success UI |
| data-only package | manifest, operations including PVE min/max population and optional schema-v2 fixed PVE AI values, scene/dependency bundles, preview, lighting payloads, authored world/collision/markers, and one Standard-PVE extraction transform/trigger | executable C# or BepInEx hooks |
| optional map companion | exact-scene material, grounding, lighting, navigation, authored foliage-sight activation, interactive, and diagnostic work; teardown of its own graph/material/service state | official catalog mutation, generic terrain payload decoding, generic laptop flow, other maps |

The package directory is always data-only. “The package has no DLL” does not
mean the complete map distribution can never have code. When runtime-only
reconstruction is necessary, install the companion as a separate BepInEx
plugin beside—not inside—the package root.

Prefer scene/bundle data. Add a companion only when a required native object or
installed-game shader/service cannot be transported faithfully.

## Activation and startup order

The companion must fail closed unless all of these identify the same map:

- exact accepted package ID and content/catalog identity when available;
- exact map/operation relationship;
- exact loaded scene path, not only a short scene name;
- exact supported Core, Modded Operations, game, Unity, BepInEx, and Il2CppInterop
  versions/fingerprints required by the implementation.

A safe order is:

```text
Core verifies/freezes package catalog
-> Modded Operations clones/binds private UI
-> shipped infiltration selector receives package-owned map data
-> Confirm captures the exact player-owned laptop and player
-> Modded Operations loads declared dependencies and exact scene
-> map companion verifies exact scene/package ownership
-> framework reconstructs manifest-declared TerrainData and validates collision
-> Modded Operations creates the mode-compatible native owner and enters readiness
-> companion repairs exact-map materials/navigation/grounding and logs its strict diagnostic
-> Modded Operations enters generic mode population
-> operation runs
```

The current framework creates its mode owner after the generic terrain and
walkable-ground gates. The Forest companion waits for that shared TerrainData
before tree grounding and A*. Its world check is an exact-map diagnostic, not
the generic framework owner. PVE actors also wait for all players and a bounded
post-ready delay. Record the real callback order. Do not describe an arbitrary
frame delay as proof of an exact-map diagnostic.

## Portable material transport

An external Unity project generally cannot compile OPERATOR's private HDRP
Shader Graphs. A bundle that loads completely can therefore render brown,
flat, opaque, or with error materials while the correct scene is active.

Use portable materials as transport records:

1. Preserve original raw/native material identity independently of proxy or
   runtime wrapper names.
2. Carry base/alpha, normal, mask, tint, and audited special maps in properties
   the portable authoring shader can serialize.
3. Document every cargo slot whose runtime meaning differs from its proxy
   meaning.
4. In the exact scene, create a new Material from the installed native shader
   family.
5. Apply the audited queue, alpha cutoff, far cutoff, material-type mask,
   culling/double-sided state, passes, keywords, wind, and numeric values.
6. Move cargo textures to their real native property and disable the proxy
   property's visible meaning.
7. Rebind Renderer/Terrain material ownership without mutating a global shared
   native Material.
8. After several rendered frames, count active proxy/error shaders and fail if
   any required renderer remains unreconstructed.

When a generic framework verifies and retains map dependency bundles, it is
the only unload owner. Do not use global loaded-bundle enumeration as ownership
proof. Do not load a second copy of the same bundle. A map-scoped borrower such
as `LoadVerifiedMapDependencyAsset<T>(mapId, assetPath)` must pass a live probe
for the exact asset type before release code depends on it. The `0.3.17` API
returned null for Forest `0.4.12` raw pine `TextAsset` requests. Keep that path
`PROVEN-STATIC` until an isolated live test succeeds.

Treat cross-map bundle retention as bounded ownership, not a process-lifetime
archive. Preserve the exact active/restart map plus an in-flight selected-map
prefetch. After a fresh different-map launch transfers active ownership, evict
prior completed maps with `Unload(false)`. `Operation Room` is not sufficient
proof by itself because it remains the active Unity scene under additive map
scenes. After successful extraction and return, clear the completed operation's
transition owner only after its package scene has released; a stale owner can
consume or veto the next packaged-map Confirm even while Operation Room is
visible. Require a zero package-scene handle and compare every cached and
in-flight scene bundle's `GetAllScenePaths()` inventory against every loaded
scene. An inventory error or match vetoes unload. Same-map alive and KIA
Restart must reuse the resident bundle and never enter cross-map eviction.

Never use `CopyPropertiesFromMaterial` with an `InternalErrorShader` source.
Use the raw record as evidence and reconstruct a fresh live material.

## Runtime Terrain and collision

Treat managed Unity wrappers as potentially fake-null in IL2CPP. Use native-
aware validity before accepting TerrainData, Terrain, or TerrainCollider.

If TerrainData cannot travel safely, declare `runtimeTerrain` in the package.
Modded Operations, not the companion, must:

- package lossless height/alphamap payloads at exact resolution;
- create fresh native TerrainData;
- set size, layers, heights, and alphamaps through IL2CPP-compatible arrays;
- bind the same live TerrainData to Terrain and TerrainCollider;
- publish collision/physics transforms before marker/actor readiness;
- fail the world contract if the binding is unusable.

Do not let a player or AI spawn merely because a visible fallback mesh exists.

If the scene carries a render-only terrain mesh for editor portability, disable
that exact fallback object immediately after the live `Terrain` and
`TerrainCollider` bind succeeds. Do not leave two coincident ground surfaces.
Keep the fallback active when reconstruction fails so the failure remains
visible, but fail readiness and do not spawn actors.

After the final TerrainData is live, align each collision-enabled playable
tree by a family-aware authoring datum computed from its highest-detail
bark/trunk submeshes. Do not use the root transform, the bottom of a generic
collider, or the minimum of the combined crown renderer. A lower leaf card can
corrupt the combined minimum. A broad-crown bark submesh can include high
branches and corrupt the full bark span.

In the exact-version Unity builder, select LOD0 renderers. For each material
slot whose normalized name identifies bark or trunk, read that submesh's
indices and transform the referenced vertices into world space. Define an
explicit per-family reference span and buried fraction. Validate it in a
downhill player-height view. Do not apply one percentage to every family.

The current Ukrainian Forest proof uses child
`NATIVE_TRUNK_GROUND_DATUM_ONE_SIXTH`. Narrow pines use one sixth of the full
rendered LOD0 trunk span and a center terrain sample. Broad-crown
`Oak_White_Desktop_*` trees use the oriented main-stem reference, a `0.25 m`
additional embed, and the lowest terrain sample across a bounded `0.60 m` to
`2.00 m` LOD0 lower-root footprint. Store that contact X/Z in the datum. This
prevents runtime Terrain sampling from lifting a cross-slope oak back to its
root-center height. Require at least `0.75` of the complete rendered tree
above terrain and reject an absolute correction above `12 m`.

At run time, sample terrain at `marker.position` and move the direct tree by
`groundY - marker.position.y`. Do not resample the root center. Run this
bounded correction after TerrainData bind, use typed `GetChild(index)`
traversal in repeat-load IL2CPP paths, and call `Physics.SyncTransforms()`
after the batch. The builder owns render-only perimeter-tree grounding.

## Runtime A* graph construction

A scene AssetBundle does not prove that the standalone session has a resident
`AstarPath` service or a scanned graph. Navigation is live runtime state.

The graph host also needs the native RVO service. Vanilla `level16` stores an
enabled `Pathfinding.RVO.RVOSimulator` with `AstarPath` on `Astar Navmesh`.
The shipped `BOT V2` uses `FollowerEntity` on its moving model child. A graph
without an active RVO simulator can accept markers while every bot remains
stationary.

For the current installed A* IL2CPP surface, the validated shape is:

1. Call `AstarPath.FindAstarPath()` and reuse a compatible target-scene service
   only when ownership is proven. Otherwise create one map-scoped host and add
   `AstarPath`.
2. Require `astar.data` before continuing.
3. Remove only a previous graph owned by this companion before rebuilding.
4. Add the graph through
   `astar.data.AddGraph(Il2CppType.Of<GridGraph>())`, then
   `TryCast<GridGraph>()` and fail if the native wrapper is unavailable.
5. Set centre/rotation/aspect, then call
   `GridGraph.SetDimensions(width, depth, nodeSize)` from the authoritative
   playable physics and bullet-interaction bounds. Never size it from a larger
   render-only terrain or scenery apron.
6. Configure max slope, max step, erosion, neighbour/corner policy, height
   sampling, ground requirement, and capsule/obstacle collision from the map's
   measured traversal contract.
7. Scan the specific graph with `astar.Scan(graph)`.
8. Resolve or add an enabled `RVOSimulator` on the same host and require
   `RVOSimulator.active` before actor readiness.

The runtime graph's authored name belongs to `NavGraph.name`; it is not the
`GameObject.name` of the `AstarPath` host. A live verifier must enumerate the
exact scene-owned `AstarPath.data.graphs`, require exactly one graph with the
expected ordinal name while loaded, and require zero such graphs process-wide
after teardown. Searching the Transform hierarchy for the graph name is a
false-negative gate.

When OPERATOR layers cannot be authored portably, a temporary scan layer is
valid only as a scoped transaction:

```text
save Terrain.layer
-> set graph heightMask to the temporary Terrain layer
-> exclude that layer from obstacle collision
-> move only the Terrain to the temporary layer
-> Physics.SyncTransforms
-> scan
-> finally restore Terrain.layer and sync again
```

Never leave the Terrain on the scan layer, move the whole map hierarchy, or
allow the temporary mask to leak into rendered gameplay.

For an indoor map staged inside a larger warehouse, separate the visible
warehouse ground from the playable-navigation source. Confirm the shipped
scene's active ground renderer/material/collider, then select a non-primitive
native geometry donor with the same proven appearance when built-in primitive
geometry is prohibited. Gate topology, vertex channels, upward normals,
renderer slot order, collider/shared-mesh identity, world coverage, and
elevation. If the apron lies outside sealed kill-house perimeter walls, mark it
explicitly as nonplayable and exclude it from graph-source discovery; never
let the larger visual floor silently expand AI navigation behind the walls.

## Mission-marker grounding

Apply the contract to every marker class the operation can consume: ordinary
enemies, HVTs, bosses, reinforcements, or any map-specific AI role. Validating
only the first generic enemy list is incomplete.

Before nearest-node lookup, test every marker against the map-authored playable
volume with explicit wall clearance. A marker outside that volume must be
quarantined so generic discovery cannot consume it, and the world contract must
fail when the required valid-marker count is no longer available. Graph
coverage is not a substitute for this check: a graph sized from a visual apron
can contain walkable nodes behind a physics/bullet barrier.

For each marker:

1. Resolve the nearest walkable node with the installed A* constraint.
2. Reject a missing node.
3. Measure horizontal correction separately from vertical correction.
4. Reject horizontal correction beyond a small map-authored limit; a distant
   node can silently move encounters into another lane.
5. Move to the node centre plus the exact intended foot clearance.
6. Sync Physics transforms.
7. Prove collision ground within a tight local window around the marker.
8. Prove `AstarPath.IsPointOnNavmesh(marker.position)`.
9. Log rejected marker names and distances and fail the world contract when
   any required marker fails.

A long downward ray is invalid as grounding proof. It can hit terrain tens of
metres below an airborne marker. Use a post-snap check such as a ray starting
only slightly above the marker and bounded to roughly the local foot-clearance
window, adjusted for the actual actor origin/collider.

## World contract

Before generic mode creation, the framework requires the exact scene,
manifest-declared terrain, walkable collision, and compatible marker set. In
addition, the map-specific release diagnostic must require:

- exact scene/package identity;
- expected world root and nontrivial authored transform count;
- required active renderer/collider counts or explicit map-specific minima;
- zero active portable/error-shader renderers;
- usable Terrain/TerrainCollider when the map declares terrain;
- exactly the intended map-owned navigation graph/service count;
- graph centre/dimensions equal the authoritative playable physics/bullet
  volume rather than the visual terrain extent;
- every operation-consumed marker is inside that volume before nav lookup;
- every required mission marker tightly grounded;
- every required mission marker on the resident graph;
- no rejected marker or missing critical asset/material closure.

For PVE, the data-only package owns `minEnemies` and `maxEnemies`. Require
`1 <= minEnemies <= maxEnemies <= 100`. Treat 100 as a global ceiling, not a
required value for every map: the declared maximum must not exceed the minimum
certified safe capacity across every scene variant. Author at least
`maxEnemies` ordinary `PVE_EnemySpawn_` transforms, snap every candidate to the
resident graph, and retain at least two metres of planar separation after the
snap because the native raid manager consumes candidates and applies its own
one-metre spawn exclusion. Inactive utility-marker GameObjects may remain
eligible; active-marker count is telemetry, not capacity. The briefing exposes
an integer selector inside the declared range, captures it atomically on
Confirm, and the host owns that count in multiplayer. PVP omits both fields.
Modded Operations must not hard-code a map's range.

Current version-pinned LOT 12 evidence demonstrates the large-population form
of this contract without changing the global ceiling: package `0.1.24` declares
`10..60`, every one of ten scenes authors 72 tactical candidates with at least
2.05 m pre-runtime planar separation, and the companion certifies at least 60
after graph snapping. A BepInEx generation launched and grounded 60/60 native
server-owned AI, alive Restart destroyed the exact prior 60 roots, and the next
generation validated a fresh 60/60. Treat this as local lifecycle evidence,
not a general performance or multiplayer claim: the same 60-AI run was highly
CPU/memory intensive, and paired peer replication is still a separate gate.
Package `0.1.24` also preloads the exact reconstructed door's 47 unique audio
clips as decompressed-on-load assets before interaction; this is first-use hitch
mitigation, not multiplayer door proof.

On the current candidate, filter `GameManager.AllAITypes` for root `BrainAI`,
root `NetworkIdentity`, `WeaponsAI.SpawnWeapon=true`, and a non-empty
`weaponList`. Give those prefabs and package-valid markers to a scene-owned
`RaidManager`. Call `RaidManager.ServerSpawnAI(false)`. Current-build native
inspection shows that it uses owner-aware
`NetworkServer.Spawn(bot, GameManager.instance.gameObject)` before it applies
`BotSpawnDetails`. The pinned single-player Forest scope has runtime proof for
reciprocal firearm damage. A grenade result alone is not sufficient evidence
for another map or game build.

The shipped `BOT V2` prefab keeps `BrainAI` and `NetworkIdentity` on its
network root. Its `SK_Insurgent_P8` child keeps `AgentController` and the
enabled `FollowerEntity`. The root can remain stationary while the moving
entity searches. Measure displacement with `brain.agent.Agent.position`, not
`brain.transform.position`.

For live BOT V2 grounding proof, use the same native
`brain.agent -> AgentController.Agent -> FollowerEntity.position` chain after
validating the controller/follower entity state, scene ownership, and hierarchy.
Do not substitute the largest runtime `CapsuleCollider.bounds.min`: the shipped
prefabs do not author a body capsule as their locomotion datum, and procedural
body/ragdoll capsules can sit above a correctly grounded navigation agent.

Log actual/expected counts and fail closed. Do not reduce the contract to
“sceneLoaded fired” or “there is a terrain somewhere.”

## Fixed PVE AI profile and foliage sight

Use schema version 2 only when a PVE operation needs fixed map-owned native
AI values. The closed `pveAiProfile` owns detection range, FOV, the effective-
range sentinel, integer wander radius, optional initial-wander-delay cap,
optional native reaction disposition, optional maximum reaction time,
communications, and counter-suppression. Reject it in schema v1 and PVP. Keep
it operation-local. Add no difficulty UI and make no process-global AI write.
Accept `initialWanderDelayMaxSeconds` only from 2 through 60. Treat omission as
a compatibility contract: do not write any AI state at that seam.
Accept `reactionDisposition` only as exact lowercase `defensive`, `offensive`,
or `random`, and apply it to `BotSpawnDetails.reactType` before the shipped
spawn. Accept `maximumReactionTimeSeconds` only from 0.10 through 1.50. Apply
that cap once, server-side, to newly spawned operation brains after `Awake` has
established the native baseline: cap both `_baseReactionTime` and
`ReactionTime`, never raise a faster value, and never modify `reactionTimer`,
difficulty, targets, state, cover, or aim. Omission means zero writes at each
optional seam.

Never populate a native raid from a mixed-team donor array. Resolve the live
player's coherent native team identity, exclude it, group firearm-capable AI
prefabs by exact `StartingTeamStats` object and numeric team ID, and require one
unique strict-largest hostile cohort. Temporarily supply only that cohort to
the synchronous shipped population call and restore the raid's prior array in
`finally`. After spawn, fail closed unless every new brain has the expected
`TeamIdentifier`, `TeamIdentifierReference`, `TargetPool`, `TeamStats`, and
network team identity, and no cohort member appears in another member's native
enemy or possible-target lists. If an otherwise proven donor is unresolved,
repair only through the native network team setter so its shipped team-change
lifecycle owns every list. Never patch damage or directly mutate native team,
friendly, enemy, or target collections.

Measure the authoritative playable combat volume and every accepted player-
to-enemy marker gap before selecting values. Do not measure the larger visual
terrain apron. Keep initial detection below the intended start gap. Keep one
wander radius below the distance that would cross the intended encounter
midpoint.

Apply the profile to each `BotSpawnDetails` before
`RaidManager.ServerSpawnAI(false)`. Current native
`RaidManager.ApplyBotSpawnSettings` transfers detection range, FOV,
communications, counter-suppression, effective range except `-1`, and wander
distance except `-1`. It also copies `BotSpawnDetails.idleState` to
`BrainAI.idleStates`. A wander distance alone does not cause movement. Set
the profiled PVE marker substate to `BrainAI.IdleStates.Wander`; native
`BrainAI.UpdateStateMachine` calls `Wander(dt)` only for that substate. It
does not consume marker `DetectionTimeMultiplier` or `HearingRange` on the
pinned build. `BrainAI.Wander` normally preserves its prefab
`WanderTimer * Patience` delay and then chooses around the current position;
repeated choices can expand a search. When the optional initial cap is present,
apply it only after the shipped server spawn, only to newly added,
non-responding `IdleStates.Wander` brains, and only once per operation brain.
Advance `wanderTime` so 50–100% of the cap remains using a stable server-side
stagger. This wander-delay seam never writes `WanderTimer`, `Patience`, or
`ReactionTime`; never consume
Unity's global random state, and never reapply after the first native move
resets `wanderTime`. This preserves native combat, cover, doors, and every
later wander cycle.

For exact Modded Operations `0.3.23`, static IL inspection proves that
`TrySpawnStandalonePveEnemies` collects every current-scene
`PVE_EnemySpawn_` transform, attaches or reuses `BotSpawnDetails`, calls
`ConfigureStandaloneBotDetails`, and then delegates population to shipped
`RaidManager.ServerSpawnAI(false)`. A non-null `pveAiProfile` selects
`BrainAI.IdleStates.Wander`; the same writer leaves patrol looping false and
does not derive patrol points from arbitrary child names. Use a bounded native
wander profile when a data-only indoor operation needs movement. Do not invent
a map-specific AI controller or claim patrol behavior from marker metadata.

Treat tactical enemy placement as authored physics, not decorative naming.
For every candidate enemy marker, require a clear standing capsule, a walkable
node within the accepted snap tolerance, a plausible ingress-facing direction,
and a nearby cover point backed by a non-trigger collider under an audited
native asset. Use several role classes such as prop ambush, cross-cover,
hallway interdiction, and architectural corner guard. Validate every candidate,
because the native population may select any marker. Repeat the placement
contract after runtime navigation snapping. Then use live `BrainAI.agent.position`,
`_currentCover`, state, seen target, and actor rotation to prove movement and
cover use. Static marker/IL proof is not gameplay proof.

For a profiled PVE operation, record the instance IDs already in
`GameManager.allAI` before the native population call. Track only new IDs
after it. Read the live `WanderTimer * Patience`, remaining first delay,
detection, FOV, wander, and communications values. When a cap is declared,
log its handled/advanced/preserved/skipped counts separately from the native
timer fields. Take bounded read-only snapshots at 0, 10, 30, 60,
90, and 120 seconds. Record movement from spawn, movement toward the captured
insertion, `CurrentSeenTarget`, `CurrentState`, and a linecast with the live
`EyesAI.DetectionLayerMask`. Treat the linecast as geometry evidence. Do not
use it alone as acquisition proof, and do not write an AI field for the test.
`CurrentSeenTarget` can refer to any native target known to the bot. A
non-zero count does not prove local-player detection. Correlate target state
with the same-mask player probe, distance, AI state, and physical firearm
behavior.

Treat foliage sight as exact map content. Inspect the same installed vanilla
prefab and the current `EyesAI` physics mask. Activate an authored inactive
sight-collider child only when prefab evidence proves its name, layer,
collider type, and trigger state. Require exact authored and active counts.
Verify the collision matrix and bullet mask so the collider does not become a
movement or projectile wall. Do not enable it globally.

The Ukrainian Forest `PROVEN-RUNTIME` profile uses a 70 by 140 m playable
volume, 78.87 m nearest solo enemy gap, 45 m detection, 90-degree FOV,
native-prefab effective range, 38 m wander, communications on,
counter-suppression off, and exactly 183 authored barberry triggers on layer
18 `AI_VisionBlock`: 79 from 118 direct bushes and 104 from 156 perimeter
bushes. The repeated prefab cycle is Barberry 2, Barberry 3, then Juniper.
Only both barberry prefabs contain the native inactive `AI Collider` child;
the 91 Juniper instances do not. Do not synthesize Juniper blockers. PVP omits
the profile. First launch and same-process restart accept search timing,
movement, movement toward insertion, and authored foliage obstruction.
Reciprocal firearm behavior, native all-AI-dead completion, extraction, and
success return passed for the pinned single-player Forest scope. Multiplayer
replication remains a separate gate.

## Native Standard PVE completion, extraction, and ATAK

Treat extraction as a split authoring/runtime contract. A Standard-PVE map
must author exactly one transform whose name starts with `PVE_ExfilZone_` and
must attach one positive, enabled `BoxCollider` with `isTrigger=true`. The map
owns the transform, rotation, layer, collider center, and collider size. Put
the trigger at an intentionally reachable location. If extraction must return
the player to insertion, author it at the same terrain-grounded player-spawn
area and size it to include the complete accepted insertion set.

The generic framework must validate that authoring record before gameplay. It
then creates one scene-generation Standard-PVE owner with this exact graph:

```text
StandalonePveGameMode : InfiltrationManager
|- one RaidManager
|- one ExfilZone
|- one copied BoxCollider trigger
|- inactive locked marker
`- inactive native-compatible ATAK Exfil Marker
```

The framework must initialize `RaidManager.exfilZones` with only that
operation-owned `ExfilZone`, set `RaidManager.EXTRACT_TIMER=15`, and restore
the same one-item list after `RaidManager.ServerSpawnAI(false)`. The shipped
population method can repopulate the list from resident donor objects; keeping
that foreign list is not standalone ownership.

Initialize the locked state explicitly:

```text
ExfilZone.NetworkcanExtract = false
GameManagerNetwork.NetworkcanExtract = false
GameManagerNetwork.NetworkisExtracting = false
GameManagerNetwork.NetworkextractionStartTime = 0
GameManagerNetwork.ExfilTime = 15
GameManagerNetwork.SuccessfulOperation = false
```

Do not implement a parallel kill counter. Preserve the shipped
`StandardPVE.UpdateAICount` and `RaidManager` all-AI-dead path. It must observe
the native AI collection reach zero, activate the available exfil marker, and
set both the zone and global extraction permission. Prove that neither flag is
true before the last AI dies.

The ATAK marker is framework presentation, not map geometry. On the pinned
build, reconstruct it from the resident vanilla assets and exact audited
values:

- GameObject `ATAK Exfil Marker`, layer `17`, initially inactive;
- mesh `Marker`, four vertices and triangles `2,1,0,3,2,0`;
- material `ExfilZone`, shader `HDRP/Unlit`, render queue `2501`;
- resident texture `ExfilZone`, `512 x 512`, DXT5;
- local rotation quaternion
  `(-3.0159049e-7,-0.70710683,-0.70710677,3.2782552e-7)`;
- uniform local scale `0.65`;
- vertical texture offset `-0.22`.

Use the exact mesh vertex and UV arrays from
`CreateNativeAtakExfilMarker`; do not replace this marker with a map-authored
icon, UI overlay, or unrelated texture. Re-inspect these values after a game
update.

Completion remains native. The player must physically enter the unlocked
trigger. The shipped occupant sets must report the player, the shipped timer
must run for 15 seconds, and the game must set
`GameManagerNetwork.SuccessfulOperation=true`, unload the additive operation
scene, and show its persistent Mission Successful result. During teardown,
remove only operation-owned runtime assets and singleton references. Do not
clear `SuccessfulOperation` on the success path because the Operation Room
reads it after scene unload.

Do not make a Terrain component a prerequisite for indoor extraction QA. An
authored exfil trigger can sit on a native mesh floor. Resolve the surface that
actually supports the authored trigger. Use an exact-scene Terrain only when
its enabled TerrainCollider covers the trigger's X/Z position and the sampled
height aligns with the trigger base within the authored tolerance; do not pick
an unrelated Terrain merely because it is active. Otherwise cast downward at
the authored trigger, ignore and explicitly reject trigger colliders, require
an upward-facing hit from the exact loaded operation scene, and verify the
expected floor/collider identity and trigger bounds. Move only the single owned
live player through the shipped `GameManager.MovePlayerToSpawn` path. Never set
extraction permission, occupant counts, timer state, `SuccessfulOperation`, or
completion state from the observer.

When infil and exfil share the safe room, the locked trigger can already
contain the player before the last AI dies. Treat that as physical occupancy,
not premature extraction, provided zone/global permissions, extracting state,
and success state remain false. After the native all-AI-dead path unlocks both
permissions, accept a naturally started countdown if the owned player is
still physically inside. Otherwise move out to a separately validated
exact-scene surface and back through the native movement path. In both cases,
require real occupant sets, timer progress for the configured duration,
`SuccessfulOperation`, the shipped success popup and Continue control, exact
scene unload, and the Operation Room. A QA harness that reports “no Terrain”
has diagnosed its own surface assumption, not a map-completion failure.

For Ukrainian Forest package `0.3.21`, the authored root is
`PVE_ExfilZone_00` at `(0.000,0.112,7.000)`. Its trigger center is
`(1.3259258,2.066852,1.6703243)` and its size is
`(25.236944,7.376298,15.531027)`. It covers the north Team-1/player insertion
markers, including the backup marker near Z `12`. An automated stationary
observer recorded 13 initial live AI, no premature unlock, native death to
zero AI, both extraction flags, the exact ATAK layer/mesh/material/shader/
texture, one physical trigger occupant, the 15-second timer, scene unload,
and `SuccessfulOperation=true`. The private observer's forced native damage
correctly produced a non-saving QA status. A subsequent physical user run
confirmed the normal playable extraction flow. Keep the private driver and
its logs out of every release archive.

## Selected-map cold-load prefetch

An external package can be much larger than a shipped mission's post-Confirm
content gate. Do not wait for Confirm before every cold bundle request when a
stable row selection is already available.

For the current generic candidate:

1. Start one selected-map prefetch after the immutable row selection.
2. Load dependency bundles in manifest order, then the scene bundle.
3. Keep the content ID with the cache entry.
4. Unload an incomplete or stale same-map entry before a new request.
5. Do not prefetch all maps.
6. If Confirm occurs during the request, attach the exact laptop, player,
   operation, and time code to that request.
7. Call native start only after scene-path validation succeeds.
8. Log each file name, byte count, request time, total time, and remaining
   Confirm wait.

Unity does not expose a safe cancellation contract for an active
`AssetBundleCreateRequest`. Finish the one bounded request. Do not start an
unbounded speculative queue.

For retained-bundle QA, inspect the generic framework's exact current-map cache
owner and require the expected non-null dependency plus scene bundle handles.
Keep `AssetBundle.GetAllLoadedAssetBundles()` as a process-wide diagnostic only:
on the pinned IL2CPP build its enumerator can report zero while the verified
framework cache still owns and serves both bundles. Never reload a second copy
merely to make that global count nonzero.

This method moves cold I/O earlier. It does not remove the bytes. The generic
contract remains `PROVEN-STATIC` for an untested package. Ukrainian Forest is
`PROVEN-RUNTIME` for one physical Confirm, first launch, same-process restart,
and the exact timings below.

## Manifest-driven multi-scene variants in Modded Operations 0.3.24

`OperatorModAPI` parses a map's closed `sceneVariants` collection. Modded
Operations `0.3.24` is the sole generic selection and validation owner. The
opt-in boundary is exact: only `SceneVariants.Count > 1` enters the variant
path. A single-scene map uses its primary `ScenePath` and does not create,
read, or write selector state, consume variant RNG, or emit variant-selection
logs. Do not infer opt-in from a package name, bundle size, or companion.

For an opted-in map:

1. Scope one persistent shuffle bag to package ID plus map ID. Preserve exact
   variant ID-to-scene-path identity, avoid an immediate repeat, and retain
   history across OPERATOR process restarts.
2. Advance the bag only for a fresh Operation Room Confirm. Store that exact
   choice in pending and active operation state. Alive Restart and the shipped
   KIA Restart path reload the active scene and do not consume another choice.
3. On an asynchronous cold launch, validate the exact dependency and scene
   bundles before committing a selection. A launch using an already verified
   cache may select immediately from the same closed manifest inventory.
4. When maps share a content identity and scene-bundle path and any map in the
   group declares variants, require `AssetBundle.GetAllScenePaths()` to equal
   the exact union of the participating maps' declared scene inventories.
   Reject missing, extra, subset, superset, or unrelated addresses.
5. Fail closed on corrupt selector state and recover only from the bounded
   checksum-protected backup. Reconcile package changes by exact scene path so
   a renamed variant ID does not turn the previous scene into an immediate
   repeat.

Do not put selection back into an exact-map companion. A companion must not
mutate `ScenePath`, choose once at plugin startup, or Harmony-patch
`ValidateLoadedSceneBundle`. Those were bounded compatibility techniques for
`0.3.23`, not the current ownership model.

Treat selector correctness and authored layout diversity as separate release
gates. For every declared pair, derive the complete room/connection graph and
physical room placement from the current serialized scenes. Exclude scene IDs,
human motif names, and cosmetic indices; canonicalize translation plus the
eight 90-degree rotation/reflection transforms. Fail closed on an exact
unlabeled graph isomorphism or physical spatial duplicate, then require a
bounded composite distance across several independent axes such as primary
cycle shape, room program/order, door/open rhythm, footprint, room-size
distribution, loop rank, and structural features. Invoke this audit from the
normal design validator so a later edit cannot silently collapse two variants.
A labeled overhead contact sheet is useful corroboration but remains visual
layout evidence only, not proof of lighting, materials, interaction, AI, or
gameplay.

On OPERATOR Unity `6000.3.8f1`, Kill House runtime checks retained
`KH05_SplitSpine` across alive Restart, selected `KH09_Pinwheel` in the next
process and retained it across alive Restart, and retained `KH08_DoubleBack`
across the shipped KIA Restart path. One exact-process lifecycle then proved
fresh KH10 -> alive Restart KH10 -> native mission completion -> Operation
Room -> fresh KH03 -> KIA Restart KH03. This proves the fresh-Confirm,
alive-retry, death-retry, and cross-process immediate-repeat boundaries for
the pinned single-player build. Multiplayer replication remains a separate
gate. The exact `0.3.24` framework DLL SHA-256 is
`0B61F0C3CCEC667B5FD38BAD7884C8F7349479F61AE3682F4DD4BB08C8243992`.

### IL2CPP component identity after Mirror spawn

After Mirror spawns a scene object, `GetComponents<Component>()` can expose a
native component through a base `UnityEngine.Component` wrapper instead of the
generated managed subtype. Do not use C# `is` checks to validate exact root-
component ordering at that point; the result can vary between otherwise
identical launches. Resolve the expected components directly, then compare
their native `GetInstanceID()` values against the ordered array. This keeps an
exact ordering assertion without depending on IL2CPP wrapper subtype
materialization.

For a ClassInjector-created subtype of `Mirror.NetworkBehaviour`, also audit
the native constructor baseline before the first `NetworkServer.Spawn`.
Current-build Cecil inspection shows that the generated parameterless wrapper
allocates and invokes Mirror's native constructor, while the `IntPtr` wrapper
only attaches to an existing object. An injected PVE/PVP game-mode subtype can
therefore retain `syncObjects=null` even when native components on the same
root have non-null empty lists. Mirror's first observer enters
`NetworkBehaviour.ClearAllDirtyBits` and dereferences that list. On an
operation-owned runtime root, initialize only a missing `syncObjects` to the
exact empty `Il2CppSystem.Collections.Generic.List<Mirror.SyncObject>`
baseline; hard-gate every root behaviour before registration and spawn; record
one attempt before native re-entry; and fail closed instead of retrying a
partially entered spawn. Unspawn, unregister only the deterministic owned ID,
then destroy the owned root/list together. Never restore null or call
`NetworkClient.ClearSpawners()`. This constructor diagnosis and static repair
are `PROVEN-STATIC`; require a fresh lifecycle with zero
`ClearAllDirtyBits` warnings and no `Map Loaded... !BUG!` fallback before
runtime promotion.

## Native loading presentation

After the exact additive scene passes identity checks, call the shipped
`GameManagerNetwork.ShowLoadingScreen()` before terrain or material repair.
The supported build places this method at RVA `0x00916210`. Vanilla
`OnAllPlayersLoaded(false)` uses the same path. The method activates the
shipped loading canvas, freezes the current player body, clears velocity, and
closes the infiltration UI. Leave the matching hide transition at RVA
`0x0090E950` under native `GameManagerNetwork` ownership.

This call closes the one-frame gap before the replacement `GameMode` can own
the readiness barrier. Without it, the package's portable brown proxy can be
visible while the companion rehydrates native shaders and live `TerrainData`.

For a log probe, read `LoadingScreen.activeSelf` and `activeInHierarchy`. Do
not use `LoadingScreenVisible` as the canvas state. Its supported-build getter
at RVA `0x0091A840` returns `_hideLoadingScreenSoon` at offset `0x2A4`.

The Forest dependency and scene bundles total `647869804` bytes. In the exact
combined-package run, the dependency took `24.449 s`, the scene bundle took
`0.833 s`, and verified registration took `25.347 s`. Confirm waited
`23.442 s` for the remaining selected-map work. Vanilla content can already
be resident. Keep verification and the native loading presentation instead
of exposing the proxy.

## Host player-spawn boundary

The persistent `GameManager` player-spawn fields are process-global state. A
standalone generation must capture `GameManager.SpawnPointsInScene`,
`GameManager.instance.Pspawns`, and `GameManager.instance.PnextSpawnIndex`
before it installs package-scene markers. It must assign both the list and
array to the same verified current-scene marker set and set
`PnextSpawnIndex=0`. Current native `nextSpawnPosition` code reads the current
index before it increments it, so an initial value of `1` skips marker zero.

Restore the captured values on operation unload, restart, framework unload, or
transition back to the armory, but only while the current globals still have
the exact list/array identity owned by this operation. Before restoring, remove
captured entries whose owning scene is no longer loaded. This prevents a
standalone scene from leaving destroyed transforms in the armory and avoids
overwriting state that another owner installed later.

An exact-scene companion must also treat any local-player transform hold and
spawn-safety state as scene-generation ownership. On scene load, clear the
prior hold and keep the shared ready/applied flag false. Publish the exact
destination only after `Terrain` and `TerrainCollider` share one non-null
`TerrainData`. A bounded local-owner repair can rescue the known initial root
from an old sky pose or from more than `2 m` below the sampled marker. For an
owned late-player callback, return without moving the player when this
standalone readiness gate does not pass. Do not use a donor-scene or Office
pre-map fallback in the standalone scene. The ready callback and bounded
repair own the move after terrain publication. On scene unload, clear the
held controller/transform, frames, counters, pre-map
support, applied flag, destination scene, and local-move-request flag before
the persistent player returns to the armory. Never retain those fields across
an additive-scene generation.

On the current exact build, request 1 for an owned `PlayerMaster` must enter
`PlayerMaster.SpawnPlayer()`. That command path reaches the shipped spawn body
and then `ClientSpawnBS`, which completes owner-side locomotion, camera, and
input initialization. Calling the generated server body directly for an owned
host on request 1 can create a visible player while skipping that owner-side
completion.

The generated server body is
`PlayerMaster.UserCode_CMDSpawnPlayer__NetworkIdentity`. Native inspection
shows that it selects a shipped `SpawnPoint`, instantiates the shipped player
prefab, calls owner-aware `NetworkServer.Spawn`, assigns the spawned-player
SyncVar and object, and sends the retail spawn RPC.

Use the generated server body on request 1 only for a server-side
`PlayerMaster` that has no local ownership. If an owned host still has no new
`PlayerSpawnedObject` 300 frames after request 1 in a repeat additive-scene
generation, enter the same generated body as one bounded recovery. The first
request has already run `ClientSpawnBS`. Pass the `PlayerMaster` network
identity. Do not construct an independent player prefab and do not call Unity
or Mirror lifecycle methods manually.

Record a spawn attempt before invoking native code, because native code can
complete synchronously and re-enter the adapter. Keep a per-player completed
set and a small attempt ceiling. The current framework uses two total attempts
for an owned host and three for other routes. Clear
both only when the operation generation changes. This prevents an owned
player's successful spawn from being followed by unbounded duplicate requests.

Record the selected route and whether `PlayerSpawnedObject` became non-null.
Then require the shipped `GameManager.MovePlayerToSpawn` path and player-camera
proof above the package terrain. A camera under the terrain with repeated null
spawn requests is a player-spawn failure, not proof that the map scene failed
to load.

This exact-build host route is `PROVEN-STATIC`. It becomes `SUPPORTED` only
after the physical host run produces the player object, movement, correct
camera, and reciprocal combat. Test remote clients and late join separately.

Register a deterministic nonzero Mirror game-mode prefab asset ID on every
peer before the host spawns the run-time owner. Collision-check
`NetworkClient.prefabs`, register the inactive template, spawn with the
asset-ID overload, validate the clone ID and mode on a remote peer, and
unregister during release. Asset ID `0` is host-only evidence.

Do not assume `UnregisterPrefab(GameObject)` can clean a scene-owned template
after scene unload. Unity can destroy the object before the unload callback;
the managed wrapper then compares equal to null while the Mirror dictionary
still contains it. A later `RegisterPrefab` can throw from
`UnityEngine.Object.GetName` and leave the native `MAP LOADED !BUG!` restart
prompt in a loop. Capture the deterministic asset ID before clearing operation
state. During release, remove only that package-owned key from
`NetworkClient.prefabs` and call `NetworkClient.UnregisterSpawnHandler(id)`
even when the object wrapper is fake-null. Before registration, if the same
asset-ID entry exists and compares equal to null, evict the same two keys and
then register. Reject any different live object as a real collision. Never
call `NetworkClient.ClearSpawners()` for this repair because it would clear
vanilla and other-mod registrations.

## PVP team-spawn identity

The map scene owns the marker transforms and marker names. The generic
framework converts only the selected package scene's verified markers to
current-scene `SpawnPoint` components.

On the current exact build, the native PVP team contract is one-based:

- `PvpGameode.StartNewRound()` sets Team 1 spawn points to `Team=1`;
- it sets Team 2 spawn points to `Team=2`;
- `PlayerMaster.MyTeamIdentifier.TeamID` contains the same numeric identity;
- `GameManager.nextSpawnPosition(true, teamId, iteration)` compares that
  identity directly with `SpawnPoint.Team`.

Therefore, assign Team 1 markers to `SpawnPoint.Team=1` and Team 2 markers to
`SpawnPoint.Team=2`. Read `TeamIdentifier.TeamID`. Do not use a zero-based
`0/1` conversion. Do not parse or compare `TeamIdentifier.ToString()`.
Invalidate a cached player-to-marker assignment when it does not match the
player's current team.

Keep the evidence boundary clear. The current standalone owner is
`StandalonePvpGameMode : PvpGameode`. Assign separate non-empty
`Team1SpawnPoints` and `Team2SpawnPoints` native lists. Use the shipped scene
seeds `MaxRounds=13`, `RoundsToWin=7`, and `RoundTime=120`. The retail server
replaces `MaxRounds` and `RoundTime` from the current lobby settings.

Call the shipped `PvpGameode.OnStartClient` body. It establishes the static
owner, calls `GameMode.Initialize`, and starts the native clock on the server.
Call the shipped `Server_AllPlayersLoaded` body. It resolves both team-player
caches, sets the round timer, freezes the players, and starts the shipped
respawn coroutine. Do not replace `StartNewRound`, `PlayerDied`, `EndRound`,
`RespawnPlayers`, score SyncVars, team-death checks, or freeze time.

Supply every reference that the native hooks read: two audio sources, 16
non-empty announcer arrays with retail lengths, `TeleType`, clock/score text,
six outcome roots, two animators, `FadeOut` and `FadeIn`, and win/lose/tie
text. Use a non-null silent clip when retail audio cannot be distributed.

Stop the generic repeated position path after `nativePvpLifecycle=true`. On
teardown, clear `PvpGameode.instance` only when it still points to the
operation-owned component.

Treat the numeric mapping and owner graph as exact-build `PROVEN-STATIC`.
Require a host and remote client on different teams. Prove first spawn,
freeze release, bullet death, score update, round respawn, team switch if it
is supported, opposite-side isolation, zero PVE AI, correct facing, Restart
Operation, and return-to-armory.

## Exact peer agreement and scene generations

Do not enter an external PVE or PVP scene because two peers report the same
version string. Before the native transition, freeze the authenticated
connection objects and require a private, bounded mode-neutral agreement that
covers:

- protocol and capability identity plus an unpredictable session nonce;
- exact game fingerprint;
- package ID/version, canonical content ID, map, operation, mode, spawn set,
  variant, scene path, time code, and player bounds;
- the exact selected-loader suite receipt, receipt-owned manifest sidecar and
  files, plus SHA-256 of loaded Modded Operations, API Core, and API host;
- the optional manifest-declared runtime companion GUID, version, loaded DLL
  SHA-256, ready marker, and failure marker.

The schema-v2 `runtimeCompanion` declaration is a closed map-level contract.
When present, resolve exactly one loaded plugin by GUID, require the declared
version and lowercase SHA-256, then wait in the selected package scene for the
declared ready marker. Fail immediately on the failure marker, duplicate or
ambiguous markers, identity mismatch, or timeout. Keep monitoring the failure
marker after readiness. A map without this declaration remains compatible and
must not acquire an inferred map-name check.

Use multiple forward-only barriers. The remote peer first verifies and preloads the exact package
and commits the exact active operation before acknowledging content readiness.
After scene transition, every peer must register the deterministic mode owner,
install and identity-check its private spawn globals, validate exact scene
ground and markers, and pass the declared companion gate before scene
readiness. Register dynamic owners through a custom spawn handler that
initializes and validates every clone before Mirror deserialization. Only then
may the host spawn one native network owner. Bound owner adoption and shipped
native readiness. Each client-owned player must receive an owner-targeted
marker assignment, run the shipped local movement path, and acknowledge its
root/controller/camera grounded on exact scene support. Never write a remote
client-owned transform directly from the host.

For PVE, the host alone selects the confirmed count and calls the shipped
`RaidManager.ServerSpawnAI(false)` once. Every peer must then acknowledge the
same sorted server-authored AI netIds, asset/team identities, initial quantized
poses, Health, WeaponsAI, native animator, and SmoothSync contract before
gameplay. Do not duplicate AI or replace native movement, bullets, damage, or
death. For PVP, require the native `PvpGameode`, mode-isolated team markers,
and zero PVE AI.

Bind scene readiness to a host-owned nonzero `UInt64` generation epoch. Start
at epoch 1 and increment exactly once for each retained-content Restart. The
remote tracks a separate monotonic local package-scene generation, so
host-first, remote-first, load-before-unload, and a reused Unity scene handle
all map to the same next epoch without reusing old readiness. Retry the exact
request inside a bounded scene deadline. Reject zero, skipped, stale, future,
overflowed, or out-of-phase epochs. Freeze the exact `NetworkConnection`
objects as well as numeric IDs so a reconnected peer cannot inherit another
connection's acknowledgement.

Treat membership change as unsupported, not transparent late join. On peer
replacement, disconnect, timeout, malformed control data, lifecycle exception,
companion failure, plugin unload, or partial native launch, cancel the session
and use the shipped host return or remote disconnect plus exact-scene teardown.
Track native-launch state independently of a scene handle so a handle-zero
transition cannot strand a loaded or loading scene. Never retry a partially
entered native lifecycle.

This architecture is `PROVEN-STATIC` until a real host and separate remote
process pass each complete online matrix. Protocol v6 provides mode-scoped PVE
and PVP agreement, but exact barriers do not prove movement interpolation,
firearm-specific hit registration, health/death replication, score, round
respawn, AI behavior, extraction, doors, Restart, or unload. PVE and PVP must
pass separate paired-log gates; BepInEx and MelonLoader must also pass separate
loader gates. Late join remains unsupported.

## Restart and teardown

### Reversible global-light isolation for additive indoor maps

An additive map's root-local light list is not the whole render environment.
Persistent loader scenes can contribute directionals and `RenderSettings` even
when the package scene contains a disabled sentinel. For an exact indoor map
that declares fixture-only illumination, validate and transact the loaded
environment at scene scope:

- capture and later restore skybox, ambient mode/color/intensity, reflection
  intensity, and external directional enabled/intensity/shadow state;
- set black zero ambient, no skybox, zero reflection intensity, and disable
  only external directionals;
- leave weapon-local spot/point lights alone;
- require authored map light components to descend from visible native fixture
  holders with explicit lit/dim/dark state;
- for overhead fixtures, measure the roof's interior underside across the
  renderer footprint and validate the fixture's rendered top against that
  surface; a downward hit from above measures the exterior top and can embed a
  fixture inside a thick roof while still passing a nominal gap check;
- audit all loaded directionals after mutation, then restore on unload, runtime
  gate failure, and plugin unload.

Do not use this beside a framework-owned global render transaction. Choose one
owner from the manifest/runtime contract and make teardown ownership explicit.
The kill-house exact-build smoke proved this pattern through a normal packaged
standalone PVE load; it did not prove full restart or multiplayer teardown.

Normal Restart Operation must unload the old scene generation and create one
fresh map runtime. The companion must:

- remove its graph through the owning `astar.data.RemoveGraph` path;
- destroy a map-scoped AstarPath host only when the companion created it;
- destroy/restore its material objects according to ownership;
- restore HWS normal/NVG brightness arrays from exact snapshots, put each
  original reticle material reference back before destroying an owned size
  clone, and deduplicate by renderer identity so restart cannot compound size;
- restore visible laser controller/light baselines and original beam materials
  before clearing enhancement identity sets; never include layer-16/IR state;
- restore the previous process-global player-spawn list, array, and index when
  the current values are still the operation-owned values;
- restore the previous NVG colour and remove only the operation-owned runtime
  Volume/profile;
- remove each deterministic package-owned Mirror prefab and spawn-handler key
  by asset ID, including when Unity already destroyed the template wrapper;
- unsubscribe callbacks and clear scene-lifetime handles;
- prevent stale async completions from mutating the new generation;
- prove one graph/service and one callback generation after restart.

The framework separately destroys its operation-owned runtime `TerrainData`
and `TerrainLayer` objects after the companion scene generation releases its
references.

Keep lifecycle claims precise:

- alive/normal Restart proves same-package/same-scene reload;
- death/respawn proves player recreation in the running operation;
- KIA/end-screen Restart proves retail failure UI and persistent end-screen
  state for a standalone operation.

None implies the others. If the KIA path dereferences missing retail
end-screen state, report that as a generic lifecycle gap without retracting a
separately proven exact-scene load or alive restart.

### Verified standalone PVE failure/restart owner

On the current verified build, the persistent shipped `GameManagerNetwork`
already has a complete `MissionFailedPopup`, Current Time, Personal Best,
Status, and Restart Operation control. Do not clone, reparent, or rebind those
objects and do not place them in the map bundle or companion.

Native `GameManagerNetwork.FailOperation()` first dereferences
`InfiltrationManager.instance` and reads the manager's `RaidTimer`. A bare
standalone `GameMode` therefore allows selection, load, player spawn, and AI
spawn but throws after native KIA. The validated generic bridge is:

1. Use an `InfiltrationManager`-compatible component for standalone PVE, while
   keeping other modes on their separately verified owners.
2. Explicitly assign both `InfiltrationManager.instance` and
   `GameMode.singleton` to that component.
3. Initialize and advance the synchronized `NetworkRaidTimer` after the
   shipped all-players-loaded barrier.
4. Suppress native `InfiltrationManager` callbacks that dereference an
   official operation scene graph; route readiness/all-players-loaded through
   the bounded generic adapter instead.
5. On scene unload, restart, or plugin unload, clear both singleton references
   when they still point at the standalone component.
6. Prove the full native sequence: lethal damage, native death,
   `FailOperation`, shipped Mission Failed popup, shipped Restart control, a
   new exact package-scene generation, playable player, and mode-correct AI.

This bridge belongs in generic Modded Operations lifecycle code. Generic
manifest terrain reconstruction also belongs in Modded Operations. Exact-map
material, grounding, lighting, navigation, and marker diagnostics remain
map-companion work. No donor scene is required.

## Stationary player-camera observer QA

When no human can steer the player, use a private environment-gated observer
that enters through the real Cerberus, Confirm, and infiltration-selector
objects. Keep the owned player stationary. Require its real
`PlayerSpawnedObject`, package spawn, declared Cinemachine camera, retail
output camera, mode actors, and exact scene. A free camera or forced scene is
not equivalent gameplay evidence.

Select one exact immutable operation ID from the frozen catalog. Do not select
only the first operation with a matching mode. Refuse an absent ID or a
mode/ID mismatch. Bound the observation time. Use at least 122 seconds when
the profiled AI snapshots through 120 seconds are required.

The private launcher must refuse an already-running OPERATOR process. Record
the executable path, process ID, and start time for every controlled process.
On timeout, use a graceful close first and act only on an exact recorded
process. Collect and hash the initial and restart screenshots, BepInEx logs,
and observer trace. Remove the driver, control files, capture directory, and
process settings after the run. Never include them in a release archive.

### Commit-headroom preflight and pre-map crash attribution

Measure Windows system commit before a long same-process variant/restart sweep,
not only free physical RAM. Record committed bytes, the system commit limit,
and the resulting headroom beside the exact QA process identity. Set a
machine-and-workload-specific minimum from observed peak usage plus safety
margin and refuse to launch below it. The current ten-variant Kill House sweep
uses `30 GiB` minimum commit headroom; this is project evidence, not a universal
OPERATOR default. A measured `55.91 / 77.84 GiB` baseline left only
`21.93 GiB` and was rejected for the repeat run after Windows reported
`Virtual Memory Minimum Too Low`.

Do not attribute a crash that occurs before the Operation Room, map selection,
package-bundle request, scene load, or companion activation to map content.
Correlate the exact executable path, PID, and process start time with the WER
event time, application path/PID when present, fault module and exception or
bucket, plus timestamp-gated game/BepInEx/driver logs. Historical WER buckets
are context only; a prior `UnityPlayer` breakpoint bucket does not identify the
cause of a later process without that correlation. Record the furthest proven
runtime boundary and keep current-hash live proof pending.

Programmatic live-UI event invocation can prove unattended lifecycle and
rendering. It does not replace a physical-pointer test when the click surface
itself changed.

For Forest `0.4.17` and Modded Operations `0.3.20`, the completed movement
baseline used a worked acceptance result with two complete windows. The first
launch created
15 grounded AI and the native restart created 14. The largest absolute
AI-to-Terrain difference was `0.03 m`. At 120 seconds, all 15 and all 14 AI had
moved at least 1 m. Six and four had moved at least 5 m toward insertion.
Maximum displacement was `51.19 m` and `49.34 m`. Both generations recorded
authored layer-18 vegetation hits. The current Forest `0.4.19`, Modded
Operations `0.3.22`, and map package `0.3.21` additionally passed the native
completion/extraction evidence in the preceding section. Require the relevant
machine result to report `passed`, or retain an explicitly named observer
limitation beside separate physical acceptance. Require no private driver
after cleanup.

## Release layout

Use this separation:

```text
OperatorMods/<package-id>/
  operator-map-package.json
  content/...
  media/...
BepInEx/
  plugins/<map-plugin>/
    <map-plugin>.dll
Mods/
  <map-plugin>.dll
```

`OperatorMods` is rooted directly beneath the OPERATOR install and is shared by
both supported loaders. A dual-loader archive may carry both companion DLLs;
only the active loader consumes its own directory. The generic framework
archive must contain no map data or map companion. A map-only archive may
contain its data package plus its own companions, but no Core/Modded Operations
DLL. A complete archive may contain both ownership domains.

Until OperatorModAPI is promoted as a full stable public API, ship its preview
Core and BepInEx host only inside the Modded Operations framework download.
Do not create a standalone preview-API archive or metadata owner, and do not
duplicate those DLLs in a map archive. Install the framework download first,
then the map download.

Keep a multiplayer test transfer separate from a Nexus/public release. Give
every filename and included README an explicit `MULTIPLAYER_TEST_ONLY` / `NOT
NEXUS` label, pin the outer archive hashes for both test machines, and state
the unfinished live gates. A clean archive audit proves transport integrity;
it does not promote online support or update the public release record.

Every archive must exclude QA drivers, auto-launch flags, forced-scene modes,
private logs, and test control files. Validate staged checksums and ZIP entries.

## Validation sequence

1. Validate manifest/schema/file closure and exact scene/dependency contents.
2. Build Core and generic adapter with map-name isolation checks.
3. Build the map companion against the exact installed runtime.
4. Prove unsupported package/scene identities cause no mutation.
5. Use the physical mission laptop: tab isolation, row, Back, Execute, Cancel,
   Confirm, shipped infiltration selector, exact scene.
6. In PVE, prove player count, AI count, all required marker grounding/on-graph,
   playable-wall containment, reciprocal combat across representative routes,
   package population range, visuals, collision, and normal restart. Also
   prove one authored extraction trigger, initial lock, native last-AI unlock,
   exact ATAK presentation, physical trigger occupation, the shipped timer,
   Mission Successful, additive-scene unload, and Operation Room return.
7. In PVP, first prove exact framework/API/companion/package identity,
   content-ready, current scene-generation epoch, companion readiness, native
   owner adoption, and native all-players-loaded completion on both logs. Place
   host and client on different teams. Prove Team 1 and Team 2 first spawn on
   their authored sides, replicated movement, reciprocal firearm-specific
   hit/damage/death, score, one round respawn per team, correct facing, zero
   PVE AI, a fresh epoch on normal Restart, and natural return/unload. Test the
   declared disconnect/late-join policy separately. A two-player run does not
   prove the configured 12-player ceiling under load.
8. Run multiple load/restart/unload generations and prove no duplicate graph,
   callback, service, actor, or scene.
9. Test death/respawn and KIA/end-screen restart as separately named gates;
   for PVE record the `InfiltrationManager` owner/timer reset across restart.
10. Remove the QA plugin and perform a clean startup with release bytes.
11. Revalidate source/deployed/staged/ZIP hashes and archive boundaries.

## Rejected shortcuts

- Putting map-specific shader or A* logic in OPERATOR: Modded Operations.
- Putting `GameManagerNetwork` failure UI or PVE lifecycle singleton/timer
  ownership in the map bundle/companion.
- Rebinding or cloning a complete retail Mission Failed popup when the missing
  state is the native game-mode owner.
- Implementing a framework-only kill counter or auto-success path instead of
  the shipped Standard-PVE all-AI-dead and physical extraction flow.
- Putting the ATAK extraction marker mesh/material or extraction state machine
  in each map companion.
- Clearing `GameManagerNetwork.SuccessfulOperation` while tearing down a
  successfully extracted standalone operation.
- Calling the complete map distribution “data-only” when it needs a companion.
- Putting a companion DLL beneath `OperatorMods` instead of its loader's
  `BepInEx/plugins` or `Mods` directory.
- Leaving a completed map's transition owner active after its package scene has
  released, which can block the next packaged-map Confirm.
- Treating a brown exact scene as proof that the selector loaded the wrong map.
- Treating a successful exact scene load as proof of native materials.
- Treating a scene-bundled navigation artifact as proof of a resident graph.
- Sizing a graph from the visual terrain apron or treating an on-graph marker
  beyond the gameplay wall as valid.
- Hard-coding one map's enemy count or coordinates in Modded Operations code.
- Accepting a 64-metre or otherwise long downward ray as marker grounding.
- Validating enemies but omitting HVT/other mission marker classes.
- Scanning with a temporary layer and failing to restore it in `finally`.
- Leaving the old graph/service alive across restart.
- Claiming death-screen Restart from an alive restart test.
- Treating equal version strings as multiplayer content or executable equality.
- Sending PVP scene readiness before exact companion/world readiness.
- Reusing an unversioned scene-ready acknowledgement after Restart.
- Calling a static/build-reviewed PVP candidate online-supported without a
  real host and separate remote acceptance run.
- Publishing a standalone preview OperatorModAPI archive before the API is a
  full stable public release.
