# OPERATOR native rendering and gameplay-reference notes

This reference is deliberately evidence-driven. Re-check it against the installed build whenever OPERATOR updates.

## Runtime facts that change the authoring strategy

- OPERATOR currently uses Unity 6000.3.8f1, HDRP, IL2CPP, BepInEx, Mirror, Smooth Sync, and Character Movement Fundamentals.
- Stock vegetation and many props use `Il2CppBRGInstancedRenderer.BRGRenderer` / Unity `BatchRendererGroup` with GPU HZB culling. Those instances often have no `GameObject` or `Renderer`; `FindObjectsOfType<Renderer>()`, renderer raycasts, and ordinary material swaps will not see them.
- Consequently, a native-map comparison must not assume a stock tree can be found as a normal renderer. Capture the actual source scene through a camera, inspect the BRG/data path when necessary, and inspect resident Material objects separately.
- `CameraSettings.SetMaxLod` and `SetVegetationLodBias` are the appropriate game-level LOD controls. Direct custom objects must still be authored from their complete highest-detail assets.

## Verified foliage shader families

| Use | Native shader |
| --- | --- |
| Bark, rock, opaque props | `Shader Graphs/BotD_Graph_Lit` |
| Leaf cards, bushes, grass | `Shader Graphs/BotD_Graph_Lit_TranslucentAlphaCutoff` |
| Far impostors | `Shader Graphs/Book Of The Dead HemiOctahedral Impostor` |

The foliage shader expects a complete material state, not an image with an alpha channel. A known working material inspection must capture at least:

- base-color, normal, and mask bindings;
- `_AlphaCutoff`, far cutoff, cutoff distance;
- surface/queue/material-type values;
- culling and double-sided-normal mode;
- alpha-test/depth/shadow pass state;
- normal/mask and animation keywords;
- color/tint, AO, transmission, and wind values.

Native cutout material settings must come from the matching source material. Typical examples observed in the Ukrainian Forest project were grass at cutoff `0.500`, barberry at `0.150`, and native foliage render queues distinct for grass and tree/bush cards. Those values are examples, not universal defaults.

## Why opaque foliage backgrounds happen

The common failure is not missing PNG transparency. It is a shader/material-contract mismatch: a fresh material may retain an opaque material type, wrong render queue, disabled alpha test, wrong culling, or the wrong property/keyword names even though the atlas contains alpha. Fix the native material state, then prove it in-game. Do not “fix” it with a generic transparent shader; that changes lighting, depth, shadows, and foliage behavior.

## ErrorShader raw-material rule

An extracted OPERATOR `.mat` can be valuable serialized evidence while its live Unity object is `Hidden/InternalErrorShader`. In that state it cannot serve as a source for `CopyPropertiesFromMaterial`: copying it onto a correct installed BotD/HDRP material can erase the working render state and still leave the custom values unavailable.

Use a three-part rehydration path instead:

1. Read the raw record for exact queue, keywords, pass state, numeric controls, and texture provenance.
2. Use a runtime-safe proxy for readable base/alpha, normal, mask, and tint bindings.
3. Create a material from the installed game shader and apply the matching exact family profile.

For the validated Ukrainian Forest profile, oak needs queue `2475`, alpha test, double-sided cards, `_AlphaCutoff=.178`, far cutoff `.245`, material-type mask `34`, procedural branch wind, and its 4K albedo/normal closure. Grass needs its distinct queue `2450`, cutoff `.5`, material-type mask `32`, and grass-specific depth/motion state. These are project-specific examples; re-audit values after an OPERATOR update.

## Standalone bundle transport versus runtime ownership

A real streamed scene can load with all authored transforms, renderers,
Terrain, colliders, and markers yet appear as brown/flat terrain when its
authoring project used portable proxy materials. That is not by itself a
catalog, selector, or scene-address failure. Prove the exact scene path first,
then audit live shader/material state.

For a standalone distribution, keep the package directory data-only and put
required native material/Terrain/navigation reconstruction in a separate
map-scoped BepInEx companion. It must activate only for the exact accepted
package/map/scene, build fresh Materials from installed shader families,
validate zero required active proxy/error shaders, and release its state on
scene unload. Do not add map-specific profiles to OPERATOR: Modded Operations.

See [operator-standalone-map-runtime.md](operator-standalone-map-runtime.md)
for the complete ownership, A*, marker, restart, and packaging contract.

## Camera-bound BRG comparison rule

BRG foliage is submitted for registered game cameras. An offscreen Camera created by a mod can render terrain while omitting official BRG vegetation, or it can render an invalid exposure history. Do not treat that frame as a side-by-side visual failure.

For official comparison, combine:

- source-scene/TerrainBRGRegisterer and material audit;
- a resident player/game camera when a visual frame is required;
- custom-map close-up and player-height captures;
- live material/LOD logs after several rendered frames.

## Spawn timing rule for large map bundles

When the stock template owns high-Y spawn markers and an external bundle loads asynchronously, the player can be created before the replacement terrain exists. Correcting the player after bundle completion is too late for a clean first spawn.

Prime the stock markers to deterministic sampled replacement coordinates as soon as the template scene loads. Supply renderer-free temporary support colliders at those exact coordinates until the real ground instantiates. Keep the normal selector/move hooks and late Smooth Sync repair; the pre-map handoff covers the earlier race, not a replacement for the network-safe path.

## HDRP environment and target-scene sun-ownership rule

Inspect a matching shipped scene's `VolumeProfile`, `HDAdditionalLightData`, light-root component chain, and active scene ownership before changing outdoor presentation. Do not infer a map's sun owner from a type name, from a weather field, or from a controller that happens to exist elsewhere in Resources.

### Decide the owner before mutation

Installed OPERATOR interop exposes `VectorTimeOfDay.singleton`, `SetTimeOfDay(float)`, `SunLight`, and `SunData` (`HDAdditionalLightData`), but it is authoritative only if its resolved active `SunLight` belongs to the target scene. Use this decision tree:

1. Resolve `VectorTimeOfDay.singleton`; read its `SunLight`; verify the Light's `gameObject.scene.handle` equals the target scene. Only then call `SetTimeOfDay(requestedHours)` once, retain that controller/light as the owner, and do not overwrite its transform, Lux, shadow, or HD fields.
2. If no valid target-scene controller sun exists, inspect the target scene for its authored static directional `Nice Sun` root. Preserve the root and its shipped `Light`, `HDAdditionalLightData`, lens-flare, `OnDemandShadowMapUpdate`, and `DirectionalLightResolution` chain. If a high-day profile is required, copy only fields directly recorded from a matching static official scene. A static high-sun record with no serialized clock is a visual profile, not proof of a literal time value.
3. After changing an authored static sun's rotation or shadow-relevant values, invoke its resident on-demand shadow update and directional-resolution methods when present. Directly setting a Light transform can leave stale delayed shadows.
4. Remove stale mod-created sun/fill roots; disable only competing template lights/Volumes while preserving the chosen native sun and player-required lights. Set `RenderSettings.sun` to the chosen Light. Never add a directional fill beside it.
5. If neither owner exists, create exactly one explicitly logged fallback sun. It is non-parity evidence and must not coexist with a fill sun.

`SunSettings` may replicate day/night state and `Mirror_DayNight.CMD_SetDayNight` may expose a local command surface, but neither establishes ownership of a static scene Light. Likewise, rain/sandstorm members on `VectorTimeOfDay` do not make a static map sun weather-driven. `StaticLightingSky` is HDRP static-lighting support, not proof of a weather system.

Do not iterate every `VectorTimeOfDay` object in `Resources`: prefabs or inactive template copies can create a second owner. Use the singleton first and only a compatible active target-scene component as fallback. Verify source construction has no duplicate sun/fill path, then verify a normal player-camera load has one sun/shadow direction.

### Volume and optics evidence

Record the matching map's live Volume priority, weight, component overrides, local fog, player-camera state, and light-unit values before calling a profile exact. AssetRipper may identify a `Volume` object while stripped HDRP derived fields remain unreadable. First resolve `Volume.sharedProfile` to its serialized profile PPtr; if necessary, decode raw component fields against the exact installed HDRP source-field order. Record the asset file/path, component identity, and value. A raw decode is valid evidence for only the fields actually mapped; use a private, normal-menu audit for the effective stack and every unresolved component rather than copying a different biome.

For an External tonemapper, resolve the profile's `Texture3D` PPtr too. Preserve its raw half-float payload, exact cubic dimensions, color-space/mip contract, and source hash; create a linear Texture3D asset in the authoring bundle; then load it by the concrete `Texture3D` IL2CPP type, not as a generic Unity object. Validate the emitted bundle's serialized dimensions and graphics format before deployment. An ACES/no-LUT fallback must be explicitly logged as non-parity; never leave HDRP in External mode with a null LUT.

Preserve Volume override state, not merely stored values. `VolumeProfile.Add<T>(true)` can force default-valued fields that the shipped profile intentionally inherited. Create a component with overrides disabled, then call `.Override(...)` only on fields whose source override flag and value were both decoded. This matters for HDRP shadows, layer masks, and color-grading controls where a stored default is not necessarily an active override.

For a stripped binary `VolumeProfile`, retain the complete `Volume.sharedProfile` PPtr chain and raw hash of every source component before decoding fields. Treat a changed PPtr order, component name, raw hash, or external-LUT payload hash as a game-build change that requires re-audit. A private live audit should report each readable parameter as `field={override=<bool>, value=<value>}` from the authored `sharedProfile`; do not call `Volume.profile` first when the goal is source provenance because it can manufacture a mutable instance clone. A component's presence or a stored default is never authorization to copy its SSR, GI, fog, or exposure controls.

For the current Unity 6000.3 HDRP scalar record format, `VolumeComponent.active` is one standalone serialized bool, followed by each `VolumeParameter` as base `m_OverrideState` and then `m_Value`. Do not reverse those words or treat the active flag as a parameter: that shifts every following field and can produce plausible but wrong override decisions. Verify the exact installed core source/interop field order and source-record hash before decoding a new build; preserve only controls whose own override bit is true. An inactive component remains inactive even when individual stored parameters have true override bits; never recreate or enable GI/SSR from its child values alone.

For a fully mapped flat component, include inherited and standalone fields in the byte layout before interpreting the component's declared parameters. `Bloom`, for example, serializes inherited `VolumeComponentWithQuality.quality` before its threshold/intensity/scatter fields; `HDShadowSettings.interCascadeBorders` is a standalone bool rather than a VolumeParameter. Vector2/3/4 values and texture PPtrs have their actual serialized widths, not a four-byte scalar width. Require the parser to consume the exact raw-record length after the final mapped field. Apply every **true** source override that is both decoded and safe to recreate, but do not turn stored inactive settings into overrides: the OPERATOR PVP example copies Bloom quality/threshold/intensity/scatter/anamorphic/resolution/filter controls and lens-flare intensity/streak controls, while deliberately leaving inactive bloom dirt/tint and lens `bloomMip` inherited.

Do not copy biome-specific weather such as a desert duststorm into a forest.

Laser/red-dot differences should first be diagnosed through HDRP exposure, tone mapping, bloom, player camera, and custom-pass ownership. Do not blindly alter an optic emissive material to compensate for a missing environment stack.

For an exact OPERATOR `0200` standalone operation, set the shipped NVG colour
through `GameManager.instance.SetNVGColor(0)` when the current build identifies
index zero as `WhitePhosper`. Do not approximate this with a camera filter.
Capture the previous NVG colour and restore it when the operation unloads.

Match the audited shipped 02:00 Volume rather than adding arbitrary ambient
lights. On the current pinned build, the relevant source is the level-7
`PVP map NIight VOLUME`. Recreate only decoded active overrides. The verified
exposure range is `limitMin=5.0652819`, `limitMax=9.3485708`, with compensation
`1.16`; use ACES tonemapping with no external LUT for this contract. Treat
these numbers as fingerprint-pinned evidence, not universal constants. The
framework owns this generic time-code state and must destroy its runtime
profile during teardown.

## TerrainData and TerrainBRG ownership rule

Inspect the stock Terrain GameObject, TerrainData, and components before deciding how vegetation is drawn. Official OPERATOR outdoor maps can attach `TerrainSurface`, `TerrainQualitySwitch`, and `BRGInstancedRenderer.TerrainBRGRegisterer` while `Terrain.drawTreesAndFoliage` is false. In that configuration ordinary direct MeshRenderers are not a faithful equivalent of stock terrain detail/tree submission.

When testing TerrainBRG conversion:

1. Keep it private/opt-in until normal player-camera evidence proves draw submission, cutout/depth/shadows, density, interaction, and performance.
2. Record the shipped registerer's actual authoring controls before selecting custom defaults: `disableBuiltInRendering`, `snapCrossfadeOnStart`, `detailChunkSize`, `preservePrototypeLayers`, `useStreaming`, and `useUnityTransforms`. Also record initialized detail/tree evidence (distance, resolution, coverage mode, loaded detail count, chunk sizes, and registered tree count). Do not infer a streaming or chunk policy from a type's field list.
3. Use a complete native mesh/material root as the `DetailPrototype`; never use a branch, billboard, generated primitive, or loose atlas texture as a replacement source.
4. Preserve authored density locations when possible. For IL2CPP Terrain APIs allocate native rank-2 detail arrays rather than passing a CLR `int[,]`.
5. Bind the registerer to the actual `Terrain` and its live `TerrainData`, configure its documented built-in-rendering contract, then refresh it.
6. Retain a validated direct-native fallback until the BRG route passes player-camera QA. Source presence is not proof of parity or interaction.

The interop surface distinguishes authoring `detailChunkSize` from initialized/derived `_detailChunkSize`, `_patchWorldSize`, and `_patchWorldSizeZ`. Record both categories but write only source-proven authoring controls; copying a derived cache value into a custom registerer changes the renderer contract rather than reproducing it.

`FoliageInteractionController` / `FoliageBendFollow` are a separate HDRP custom-pass route: they queue swept interactions into a global bend render target. A direct grass renderer bends only if its installed material actually samples that contract. Do not attach a follower, claim BRG-equivalent interaction, or use an offscreen camera result as proof without a normal player-camera comparison of the official and custom paths.

## Runtime TerrainData payload rule

An AssetBundle may list a TerrainData asset while its native object fails to bind in the target IL2CPP runtime. Treat a managed Unity wrapper as potentially fake-null: obtain the required component and use Unity's native-aware equality check before assuming a Terrain is usable.

For a map whose TerrainData cannot safely travel in the bundle, package lossless linear/readable/mip-less height and alphamap payload textures at exact resolutions. Reconstruct a fresh native TerrainData at runtime, set TerrainLayers/heights/alphamaps through IL2CPP-native arrays, bind it to both `Terrain` and `TerrainCollider`, and only then transfer spawn markers or remove temporary spawn-support colliders. If that reconstruction fails, abort map replacement rather than allowing a player to spawn above an unbound surface.

If the authored scene includes a render-only terrain mesh fallback, disable
that exact object only after both live Terrain bindings succeed. Two coincident
ground renderers cause flat brown output, depth conflicts, and misleading
collision diagnosis. A fallback is an error signal, not a second gameplay
surface.

## Asset-closure rules

1. Copy a complete dependency closure: mesh/prefab, material, base color, alpha-bearing texture, normal, mask, and any required detail/thickness map.
2. Preserve texture resolution, compression, mip chain, and importer type. A missing mip chain is a quality regression even when the texture looks sharp up close.
3. For a new mesh, use its matching material rather than a similarly named rock/foliage material.
4. Bundle a raw material library so runtime code can recover original serialized values after portable authoring proxies are replaced.
5. Validate the deployed bundle contains the material texture closure; Unity dependency discovery can omit maps from missing-shader imports unless packaging is explicit.

## Player-camera crown-silhouette rule

A tree family passing mesh, submesh, material, texture, and shader-closure
checks is still not visual proof. Inspect the normal player camera at close,
middle, and far distances and require a complete crown silhouette with readable
foliage mass. If a pine family reads primarily as bare trunks, reject that
family for playable and perimeter placement and substitute an audited complete
native pine family with its exact material/texture closure. Keep a deliberate
mixed canopy rather than masking the defect through sheer instance count.

Record both static closure and the player-camera result. An editor view,
offscreen audit camera, active renderer count, or three-material/three-submesh
assertion cannot replace this gate.

## Spawn reference

The local avatar is distributed across `GameManager.myPlayerNetworking`, `GameManager.myPlayer`, `PlayerNetworking`, `MasterController`, and client-side mirror objects. Player transforms replicate through `Smooth.SmoothSyncMirror`.

A reliable custom-map spawn solution needs all of the following:

1. Replace/transfer the PVP/FFA marker lists to grounded custom markers.
2. Rewrite the spawn selector result and the movement path.
3. Correct late-created local player controller roots, including the networking/persistent scene hierarchy.
4. Clear stale fall velocity and notify owner-side Smooth Sync after a teleport.
5. Ignore stray Office sky geometry when deciding whether a player at an obviously high Y coordinate is “supported.”
6. Test a real operation and at least one respawn; forced-scene QA cannot create the real networked local player.
7. Resolve dynamic interop `Type` objects once outside a spawn-safety/update loop, then re-read only the live singleton/object handle inside the loop. `AccessTools.TypeByName` scans generated IL2CPP assemblies and can repeatedly emit reflection-load warnings or consume time; cache the process-stable type, never a scene-lifetime singleton.

## Inspection before intervention

Before changing code, answer these with source/runtime evidence:

- Is the stock visual a BRG instance, terrain tree, GameObject renderer, or a mixed path?
- What is the matching material’s live shader and property/keyword contract?
- Which scene controller owns spawn selection and movement for the requested game mode?
- Is the observed issue a source asset, authoring proxy, bundle dependency, runtime rebind, lighting/volume, or game-quality issue?

Only then patch the narrowest responsible layer and add a validation that would have caught the original regression.

## Proxy identity and targeted LOD0 import rule

External Unity bundles frequently rename transport materials to `NATIVE_PROXY_*` and runtime-repaired materials to `MOD_*`. Do not use that wrapper name to select the native render profile. Resolve it first to the original native material identity, including template/library wrappers, then choose the shader family, alpha cutoffs, queue, foliage/bark state, and HDRP prop state. A wrapper-name comparison can leave a correct alpha atlas on a generic profile and still produce visibly wrong foliage.

When only a small set of complete trees is needed, avoid a broad AssetRipper Unity-project export: it can pull a whole game dependency graph. Retrieve an audited closure of the exact LOD0 mesh files, source material records, and their base/alpha/normal/mask textures. Import the mesh without decimation, UV/tangent/color rewrite, or rescaling; validate exact vertex count, submesh count, bounds/height, texture dimensions, alpha input, mips, non-streaming status, proxy map bindings, and live bundle contents.

An LOD0-only authored root is valid when the user requires maximum-fidelity direct placement, but later layout/prop-repair code must recognize its MeshFilter/MeshRenderer root as occupied space even though it has no LODGroup. Do not accidentally place boulders, cover, or spawns through it and do not reintroduce an LOD1 fallback merely to satisfy an LODGroup-only utility.

## Native mesh-record and special-map transport rule

Generated prefab names are not proof that a tree is LOD0. Search the installed asset records themselves, inspect each candidate mesh's vertex/index/submesh counts and bounds, then export only the selected mesh/material/texture closure. Preserve every authored vertex channel used by the material contract, including UV1 and vertex colour; a GLB importer that retains only UV0 can make a correct source mesh render incorrectly after native shader rehydration.

Retain the source collider intent too. A complete tree should use a lower-trunk collision shape when that is what the native object uses, not a cylinder or box covering its crown. Keep placement at source scale, compare height to a human-character reference, and embed the root into the sampled terrain.

Ground a combined crown-and-trunk tree from its highest-detail bark/trunk
submesh vertices. Do not use the whole-renderer minimum. A low branch or leaf
card can select a false root datum. Do not use the bottom of a native
lower-trunk collider: a hidden capsule can also raise the visible tree above a
slope. Keep that collider for gameplay.

After final rotation and scale, select the LOD0 renderer. Match the bark/trunk
material slots. Use `Mesh.GetIndices(slot)` and the referenced vertices to
calculate finite world-space `trunkMinY` and `trunkMaxY`. Ukrainian Forest
uses:

```text
trunkDatumY = trunkMinY + (trunkMaxY - trunkMinY) * 0.25
correctionY = surfaceY - trunkDatumY
```

Create renderer-free child `NATIVE_TRUNK_GROUND_DATUM_25_PERCENT` at that
datum. Move the tree, require marker-to-terrain error no greater than
`0.001 m`, require `0.75` of the trunk and at least `0.75` of the complete
rendered tree above grade, and reject an absolute correction above `12 m`.
Place and synchronize the complete batch. At run time, use the packaged child
after TerrainData reconstruction instead of mesh readback. Use typed index
traversal in IL2CPP repeat-load paths and synchronize physics transforms.

When a native shader uses a texture property that a portable Standard proxy cannot serialize, carry that texture through a documented, otherwise-unused proxy slot only for the audited material identity. At runtime resolve the proxy back to its raw native name, move the texture from the cargo slot to the correct installed BotD/HDRP property, and verify the raw/proxy GUID equality plus the exact runtime restoration call. Never leave a transport slot functioning as its Standard meaning (for example, do not leave a thickness map as emission).

## Game-mode spawn-phase rule

`SceneManager.sceneLoaded` callback order alone is not a sufficient spawn guarantee: `GameManager` may have registered its own callback earlier and can cache template markers before a plugin subscriber runs. After confirming signatures against the installed interop, prefix `GameManager.OnSceneLoaded(Scene, LoadSceneMode)` with the same idempotent, map-scoped anchor handoff used by the plugin callback.

For OPERATOR PVP, inspect `PvpGameode` and `FFA` as separate spawn owners. Their all-players-loaded, first-round, and respawn methods can bypass or precede `GameManager.MovePlayerToSpawn`. Before those game-owned phases, re-prime template anchors; when the custom ground is loaded, transfer the live game-mode spawn inputs again and retain the bounded Smooth Sync/local-root repair. Do not patch unrelated maps, force a scene load, or create a replacement player object. This rule is based on installed API evidence; only promote a particular hook set to a known-good reusable recipe after it produces a recorded real-operation first spawn and respawn.

One further OPERATOR race can occur after spawn selection. Installed-player traces establish this order: `PlayerMaster.SetPlayerSpawnedObject(PlayerNetworking)` records the network object, `RPCSpawnPlayer(PlayerNetworking, Vector3)` can then apply `LastSpawnPoint`, and `OnPlayerObjectSet(NetworkIdentity)` finalizes the owner's object reference; `PlayerNetworking.OnStartClient()` is the parallel component-activation seam. An early transform correction can therefore be overwritten by the client RPC, or run before `isOwned` has settled. When this race is present, postfix all four installed seams as one map-scoped safety net: `SetPlayerSpawnedObject`, `RPCSpawnPlayer`, `OnPlayerObjectSet`, and `PlayerNetworking.OnStartClient`.

Check Mirror ownership (`isOwned`, with `hasAuthority` only as a compatibility fallback), resolve `PlayerMaster.PlayerSpawnedObject`, and recover a root only when it is clearly high above the custom sampled ground. Use the same pre-map support position before TerrainData binds and the real Terrain marker afterward; clear velocity, sync, notify owned Smooth Sync, and use the short grounding hold. Never use this hook to move remote players, bots, or normal grounded roots. The four-hook coverage is shipped-code evidence; a recorded normal-operation first spawn and respawn are still required before treating it as a known-good runtime recipe.
