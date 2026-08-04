# OPERATOR Mod API research

Use this reference for generalized additive modding and `OperatorModAPI` work.
Keep private map-specific fixes in their project evidence log.

## Current standalone package checkpoint (2026-08-03)

The generalized local architecture now has one bounded current-build proof.
Core freezes strict data-only map packages. The map-independent **OPERATOR:
Modded Operations — Standalone Map Framework** owns physical mission-laptop
presentation, native infiltration selection,
exact scene loading, readiness, vanilla-compatible mode ownership, generic
PVE/PVP behavior, and normal/KIA restart through the shipped failure UI. A
separately installed, exact-scene map companion may reconstruct native
materials, TerrainData, navigation, and all mission-marker grounding that a
portable bundle cannot preserve.

This advances the older checkpoint below. Local single-player PVE KIA/restart
is proven for the current exact build by an `InfiltrationManager`-compatible
owner plus synchronized `RaidTimer`; it does not prove a public installer,
arbitrary map corpus, two-peer content agreement, late join, cosmetics,
weapons, or a Creator SDK. Preserve those boundaries.

The package directory itself remains data-only. The complete map distribution
may include its own companion under `BepInEx/plugins/<map-plugin>`; never put
that DLL inside `BepInEx/OperatorMods`, the Core archive, or the generic
Modded Operations framework. See
[operator-standalone-map-runtime.md](operator-standalone-map-runtime.md).

## Historical overlay baseline (superseded)

This section records the state before the standalone package proof. Do not use
it as current implementation or support authority. The 2026-08-03 checkpoint
above supersedes all incompatible statements in this section.

- OPERATOR is a Unity IL2CPP game loaded through BepInEx IL2CPP.
- The current inspected build uses Mirror.
- Existing custom forest and grocery-store worlds are Office overlays. Loading
  custom geometry into Office does not prove a standalone map.
- The current forest is an ordinary prefab AssetBundle, not a streamed scene
  AssetBundle.
- Office remains loaded and participates in game-mode, spawning, scene, and
  network lifecycle behavior.
- AssetBundles can carry Unity content, but not a safe arbitrary executable C#
  behavior model. Code modules belong in separately declared BepInEx plugins.
- A public executable-mod registration/startup foundation is now proven on one
  exact OPERATOR build. Its live stop/cleanup acceptance remains pending. No
  stable custom package registry, content handshake, standalone map catalog,
  cosmetic adapter, or additive weapon registry is proven yet.
- Generated interop or toolkit method existence does not prove that a Harmony
  target, factory, catalog mutation, or network extension is runtime-safe.

Revalidate these facts against the active build fingerprint. Do not combine
evidence from different builds without labeling the change.

Classify the exact claim, not the artifact as a whole. Generated interop can
prove a managed member exists; serialized data can prove a field/reference was
authored; a native-body audit can prove a current call relationship; a
read-only trace can prove observed ordering. None alone proves that invocation,
patching, catalog mutation, synchronization, rollback, or public support is
safe. Promote a mutation boundary only after a separate reversible
current-build experiment.

## Archived pre-standalone program checkpoint (2026-07-29)

This checkpoint is historical. It does not supersede the current standalone
package checkpoint at the start of this reference.

As of 2026-07-29, the controlling priority is to map the exact installed game
before further game-specific adapters, standalone content, Unity authoring,
deployment, or packaging. The target OPERATOR build fingerprint is
`1879ff07f73bde85ae971b5382d0a42646fc81dc3710653f6a325ecc752e41f0`,
with Unity `6000.3.8f1`. The controlling map contract is
`reports/OPERATOR_MOD_API_CURRENT_BUILD_CODE_MAP_DESIGN.md`; the private
`reports/OPERATOR_MOD_API_CURRENT_BUILD_BINDING_LEDGER.md` is an initial
ten-track hypothesis ledger, not adapter authority.

The public Evidence tool now has deterministic structural schemas, all-six-kind
typed metadata identities, explicit typed-signature digests, generic
constraints, preserved nested resolution scopes, and a dedicated canonical
JSON writer. Every structural row carries a typed, bounded
`identityPreimage`; the writer recomputes both public IDs from it and the
schema rejects cross-kind or impossible definition scopes. The interop input
layer retains one read handle per assembly, parses through a one-shot
non-owning lease, and requires same-handle before/after hashes plus fresh
ordinal membership verification. The closed operation-profile registry derives
resolver gates transitively and authorizes only exact passed profiles. Its
latest integrated Release checkpoint passed 124 tests with zero failures, but
the generator/index/ledger work continues and a test count is not a support
claim. No promoted current-build structural index or native-body census exists
yet.

The clean pinned design re-review reported no P0, four P1, four P2, and two P3
findings and rejected map acceptance. The active corrections preserve raw
signature `CLASS`/`VALUETYPE` kind, externally anchor the exact 207-file
interop inventory, require all 36 domains and ten tracks to close independently
of support labels, bind every consumed game-content container into adapter
activation, compare resolver declarations structurally, gate event-driven
performance, and retain method implementation flags plus parameter defaults.
Do not generate or promote the current-build index until those corrections and
a fresh independent re-review pass.

The earlier runtime gate manifest is stale after source changes and Release
outputs are absent. The post-IVT owned-file deployment and controlled
menu/example/start/post/stop/unload smoke remain deferred behind the map.
Therefore do not claim current runtime acceptance for packages, content
adapters, cosmetics, standalone maps, weapons, multiplayer agreement, or a
Creator SDK.

A map candidate becomes implementation authority only when it binds the exact
game fingerprint, complete interop set, runtime-dependency set, accepted
structural index and ledger, source attestation, complete applicable
observation set, closed operation profile, and independent review with no
unresolved P0-P2 finding. Keep raw generated/native output, game files,
extracted assets, saves/logs, and machine-local paths outside public Git.

## Permanent design constraints

Require all public content to use immutable namespaced package and content IDs.
Do not use a local path, Unity instance ID, mutable list index, or vanilla
content identity as the durable public ID.

Treat evidence gates as orthogonal. A required gate authorizes use only when
its status is exactly `passed`; `not-applicable` never waives it. Generated
interop proves a surface, serialized data proves an authored value/reference,
an Addressables catalog proves a catalog entry, a pinned Unity contract proves
only the exact engine API contract, native layout proves a field relationship,
and native body evidence proves a bounded relationship. Invocation, hook
installation, mutation, teardown, repetition, peer behavior, performance, and
compatibility refusal require separate observations.

Use closed operation profiles. Direct native field reads/writes require exact
native layout. Game method calls/mutations require exact current-build body
relationships. Runtime resolvers inherit every referenced step binding's gates
transitively and must form an acyclic dependency graph. Experimental bindings
remain evidence-harness-only and never enter a production adapter allowlist.

Separate:

- public definitions from native runtime instances;
- data-only packages from executable BepInEx modules;
- official/native, legacy-overlay, and experimental standalone backends;
- static scene content from dynamic Mirror-spawned objects;
- first-person and third-person representations;
- creator-only authoring components from runtime-neutral metadata;
- package validation from native Unity loading;
- Core public interfaces from IL2CPP wrappers and Harmony targets.

Freeze gameplay registries before building a session content manifest or
allowing native catalogs to cache them. Prohibit registration/removal during
an active session.

Treat unsupported capabilities as explicit results. Do not hide a failed
standalone/custom identity path behind vanilla replacement.

## Research sequence

1. Fingerprint the exact OPERATOR executable, `GameAssembly`, Unity player,
   `globalgamemanagers`, generated interop, relevant catalogs, BepInEx,
   doorstop/bootstrap, Il2CppInterop, Harmony, deployed diagnostic/plugin
   binaries, configurations, and flags.
2. Freeze the complete generated-interop membership with retained
   write/delete-denying handles; hash, parse, and re-hash each same handle;
   recheck membership; generate the structural index twice; require
   byte-identical validated output.
3. Build the curated binding, relationship, hazard, hook, and capability
   ledgers plus a complete 36-domain classification for every public
   capability. Keep unresolved required pairs unavailable.
4. Reproduce only the exact-current-build native body/layout relationships
   consumed by an adapter candidate. Exhaust static evidence and passive
   published state before adding a native hook.
5. Use an isolated, default-disabled diagnostics plugin for runtime evidence.
6. Add one hook or mutation per acceptance run; perform a menu smoke before an
   operation test.
7. Run the smallest experiment that resolves one architectural uncertainty.
8. Record the build fingerprint, peer role, source/member citations,
   observation, classification, cleanup, and remaining unknowns.
9. Add a regression test or validator before promoting a capability.
10. Update the project evidence ledger first, this reference second, and public
    human documentation only after the workflow succeeds end to end.

When the evidence workspace is dirty, untracked, or has no commit, a Git SHA
is not a reproducible source identity. Generate a canonical manifest containing
root/nested repository state plus SHA-256 and length for every source, schema,
configuration, script, and document used by the experiment. Store that
manifest hash in every result.

## Diagnostic safety

Require a unique BepInEx GUID, isolated Harmony owner, strict build adapter,
bounded output, explicit main-thread execution, and deterministic
unsubscription/driver destruction.

Prefer:

- Unity scene callbacks;
- verified Mirror lifecycle callbacks;
- passive singleton/current-state snapshots;
- bounded checkpoint scans;
- generated receive methods whose current body/signature is proven.

Reject:

- full hierarchy/resource scans per frame;
- mutating catalogs or saves during a census;
- unbounded reflection in update paths;
- forcing scenes from splash/startup;
- treating a generated signature as hook-safety proof;
- bundling private QA flags or launchers in a release.

For the currently documented build, keep the combined Harmony-prefix set on
`OperationsManager.StartOperation`, `CMD_StartOperation`, and
`DebugStartOperation` blacklisted. Its installation immediately preceded a
native `c0000005` menu crash; the exact unsafe member/root cause was not
isolated. Do not Prefix, Postfix, Transpile, Finalize, detour, or directly
invoke those targets. Use passive published state unless a later isolated
acceptance test proves one specific target.

Encode those denials by immutable target IDs plus closed operation/argument/
phase predicates, and apply them across alternative bindings, accessors,
runtime resolvers, shared native bodies, and thunk aliases. Also keep the
quarantined synchronous
`SceneManager.LoadScene(Int32)` call from
`OperatorGroceryStorePlugin.OnSceneLoaded` denied while the active scene is
exactly `ApplicationSplashScreen`; that route preceded a separate
`UnityPlayer.dll` access violation. Do not generalize it into a claim that all
later asynchronous scene probes are causal.

Deploy diagnostics only with OPERATOR closed. Back up and hash the exact
target, test unsupported-fingerprint refusal before any enabled native probe,
prove disabled mode installs no hooks, record source/build/deployed/config
hashes, and remove or restore only owned files while the game is closed. Stop
only a child PID captured from a controlled launcher; never terminate an
unrelated user-started OPERATOR or Unity process.

## Retired native-analysis campaign

The former NativeAudit/security-admission campaign is archived historical
research and is not a prerequisite for this local modding API. Do not revive
its authorization store, approval roles, containment adapters, SBOM gates, or
milestone sequence as product requirements.

When native call relationships must be researched, use the smallest bounded,
read-only, fingerprint-pinned inspection that answers the current capability
question. Record uncertainty honestly, validate any resulting mutation in a
reversible live experiment, and keep proprietary inputs and generated raw
artifacts private.

## Package and Nexus research

Treat extract-into-game-tree installation as a first-class lifecycle:

```text
discover
-> parse within strict bounds
-> canonicalize paths beneath one allowed package root
-> validate schema and IDs
-> verify sizes, hashes, dependencies, and compatibility
-> topologically order
-> open registration
-> register typed definitions
-> freeze
-> build the session digest
-> load selected content through a capability-gated adapter
-> release session resources in reverse order
```

Test Core install, mod install, Core update, mod update, downgrade, missing
dependency, duplicate ID, cycle, hash mismatch, removal, restore, and unknown
game build. Core updates must not own or delete third-party package files.

Reject traversal, absolute paths, symlink/reparse escape, malformed or
oversized manifests, duplicate IDs, cycles, hash mismatch, invalid bundle
entry, incompatible Unity/platform data, and forbidden components before a
native Unity load.

Version strings alone never establish multiplayer equality. Gameplay
agreement must include package/content identity, exact hashes, protocol,
game fingerprint, required capabilities, and a session nonce. The transport
and readiness insertion point remain unproven until an OPERATOR two-peer test.

## Content proof order

Prefer a material-only cosmetic as the first additive content proof after the
shared package/lifecycle foundation. It exercises identity, catalog timing,
renderer ownership, UI/save behavior, remote representation, late join, and
missing-content handling without a new scene or firing implementation. Never
mutate a globally shared native material.

For a standalone map, require Office to remain absent from the loaded-scene
list throughout gameplay. Prove streamed scene double-load, required service
ownership or neutral bootstrap, spawns, physical bounds, one native game mode,
readiness, two-peer lifecycle, and teardown.

When a standalone bundle cannot preserve installed private shaders, TerrainData,
or a resident A* service/graph, permit a separate map-scoped runtime companion
only after exact package/map/scene gating. Require native material rehydration,
a scanned map-owned graph, tight ground plus on-graph validation for every
enemy/HVT/mission marker, and reverse teardown. A long downward ray is not
marker-grounding evidence. Keep normal alive Restart and KIA/end-screen Restart
as separate acceptance claims.

For a first weapon, use a complete dormant native-archetype clone and native
player-context factory. Preserve first/third-person objects, components,
attachments, ammunition, animation-event ABI, IK, UI/save identity, Mirror
authority, drop/death/late join, and cleanup. Do not promise new ballistics,
damage, skeletons, or arbitrary Animator controllers in the initial proof.

## Evidence promotion

Promote a rule into this reference only after direct current-build evidence or
a successful bounded runtime reproduction. Include the failure signature and
rejected hypothesis when that prevents recurrence.

After a material update:

1. validate the skill structure;
2. forward-test the skill against raw artifacts with minimal context when
   practical;
3. confirm experimental capabilities remain labeled uncertain;
4. keep detailed logs and project history outside the skill.
