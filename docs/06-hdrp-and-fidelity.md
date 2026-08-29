# 6. HDRP, lighting, and fidelity

Status: `PROVEN-STATIC` for the current OPERATOR build and the Ukrainian
Forest reference implementation. Player-camera comparison is still required
for each new map and each supported time code.

This chapter defines the data that must survive the bundle boundary and the
state that the exact runtime owner must reconstruct. A map is not visually
complete when Unity only loads its scene objects. It is complete when the
installed game renders those objects with the intended native shader,
material, light, Volume, fog, and camera contracts.

## Keep authoring data, package data, and live HDRP state separate

Use one authority for each class of data:

| Authority | Required content | Direct consumer |
| --- | --- | --- |
| dependency bundle | texture closure, portable material records, terrain payloads, serialized `Texture3D` assets when permitted, map lighting records | exact-scene companion and framework terrain/material loaders |
| scene bundle | renderers, material slots, lights, Volumes that serialize safely, reflection objects, terrain root, and inactive render-profile markers | Unity scene loader, then the exact-scene companion |
| package manifest | bundle disk paths, exact scene address, optional raw LUT record, optional runtime-terrain addresses | Operator Mod API closed loader and Modded Operations framework |
| package loose files | raw preview image and optional raw RGBA-half LUT | framework file verifier and image/LUT decoder |
| installed OPERATOR build | private shaders, runtime HDRP component types, player camera stack, NVG custom pass | exact-scene companion after the selected scene loads |

Do not put a private installed-game shader in an external Unity project and
assume its GUID will resolve on another computer. Do not put a loose PNG in a
package and assume a scene renderer can resolve it. Bundle every renderer
texture, or declare the texture as a stable address inside an already
declared dependency bundle and load it explicitly.

## Treat bundle materials as transport when private shaders are unavailable

A clean authoring project cannot compile an installed game's private HDRP
shader simply from its name. A scene can therefore be completely present while
terrain and foliage render brown, flat, or as fallback materials. Preserve
material identity and texture/property closure in portable proxies, then let
the exact-scene map companion create fresh materials from installed native
shader families and restore audited queue, alpha, pass, keyword, culling, and
numeric state.

Do not diagnose a proven exact-scene load as a selector failure merely because
its proxies render poorly. Conversely, do not call the scene visually loaded
until active-renderer auditing reports no portable/error shaders after several
player-camera frames.

For each portable material, preserve at least:

- original material name or another immutable native-profile key;
- base color and authored tint;
- base/alpha texture;
- tangent-space normal texture and normal scale;
- mask or metallic/smoothness texture;
- detail, height, bent-normal, thickness, and transmission textures when the
  target native family reads them;
- render queue, surface type, cull and double-sided state;
- alpha cutoff and far-cutoff values;
- required shader keywords and pass enables;
- tiling and offset for each texture property.

The current Forest companion uses these live shader families:

```text
Shader Graphs/BotD_Graph_Lit_TranslucentAlphaCutoff
HDRP/Lit
MilkShaders/Lit-Template
```

It resolves the shader with `Shader.Find` in the installed process, creates a
new material, and then copies the portable record. Important source-to-live
property aliases in `CopyBundleMaterialProperties` are:

| Portable source property | Live destination candidates |
| --- | --- |
| `_BaseColorMap`, `_BaseMap`, `_MainTex`, `_AlbedoMap` | `_BaseColorMap`, `_BaseMap`, `_MainTex` |
| `_NormalMap`, `_BumpMap` | `_NormalMap`, `_BumpMap` |
| `_MaskMap`, `_MetallicGlossMap` | `_MaskMap`, `_MetallicGlossMap` |
| `_DetailMap`, `_DetailAlbedoMap` | `_DetailMap`, `_DetailAlbedoMap` |
| `_HeightMap`, Scots-pine trunk `_ParallaxMap` cargo slot | `_HeightMap` |
| Scots-pine branch `_EmissionMap` cargo slot | `_ThicknessMap` |

The two Scots-pine cargo-slot mappings are a reference-implementation
transport choice. They are not general Unity meanings for `_EmissionMap` or
`_ParallaxMap`. If you create a new portable record format, give thickness
and height explicit fields.

The live foliage audit must report a non-null base texture, normal texture,
and mask texture when the source material has them. It must also report the
expected alpha-test and double-sided keywords. A material name alone is not a
complete binding.

## Preserve alpha foliage as a complete render contract

A pine can have a valid mesh and still show a bare trunk when the branch
material loses its alpha texture, cutoff, double-sided state, or transmission
state. For every leaf or needle material, verify these values after runtime
rehydration:

```text
shader = Shader Graphs/BotD_Graph_Lit_TranslucentAlphaCutoff
base or alpha texture != null
normal texture != null when authored
mask texture != null when authored
_ALPHATEST_ON = true
_DOUBLESIDED_ON = true when the source is two-sided
render queue = the audited native queue
_AlphaCutoff = the audited source value
MOTIONVECTORS and depth-pass state = the audited source state
```

The Forest dependency-bundle closure includes this concrete full-crown pine
source address:

```text
assets/operatornativeassets/ukrainianpinecandidate/source/pine_leaves_4k.png
```

The scene contains 36 direct full-crown playable pines. The bundle validator
requires that count and rejects a tree family that resolves only to a trunk or
short understory. The runtime foliage audit runs after the first frame and at
later frame checkpoints so a delayed owner cannot silently replace a correct
material.

## Find the actual environment owner

Before changing daylight, fog, exposure, or a sun:

- identify the target scene's live directional-light root;
- determine whether a time-of-day controller genuinely owns that light;
- inspect the active HDRP Volume/Profile and its override state;
- preserve the selected native light and remove only conflicting mod-created
  lights;
- set the renderer's sun reference to the selected light;
- refresh any target-owned shadow system after changing a static light.

Do not create a second directional fill light to hide an ownership mistake.
Do not infer ownership from a class name, weather member, or inactive prefab.

The current Forest scene contains these exact render-contract objects:

```text
MOD_UkrainianForest_OutdoorEnvironment
RENDER_PROFILE_NATIVE_OUTDOOR_V1
PVP_MAP_LUT_AgX_Powerful_32_RGBAHalf
Nice Sun
```

The Unity builder that creates and validates them is:

```text
source/runtime_bundle_project/Assets/Editor/BuildHillyUkrainianForestBundle.cs
```

The source lighting record is:

```text
Assets/Maps/UkrainianForest/Lighting/UkrainianForest_Day.json
```

The scene marker `RENDER_PROFILE_NATIVE_OUTDOOR_V1` tells the generic
framework to install the audited native-outdoor profile. It is metadata, not
a replacement for the scene light or package resources. The exact scene
address remains:

```text
Assets/Maps/UkrainianForest/Scenes/UkrainianForest.unity
```

## Copy override state, not just numbers

An HDRP profile can store a value while its override is disabled. Recreating
every stored value changes the scene. Decode or inspect a component's active
state, each parameter's override state, and the target build's field order.
Apply only values directly verified for the target scene.

External color transforms and lookup textures require the correct type, color
space, dimensions, format, and mip contract. If a verified external resource
cannot be loaded, choose an explicitly logged safe fallback rather than
leaving HDRP in an invalid partial state.

For each Volume component, record and restore all of these independently:

1. component `active` state;
2. every `VolumeParameter.overrideState`;
3. every enabled parameter value;
4. profile priority, weight, global/local state, and blend distance;
5. texture type, dimensions, format, color space, and mip count;
6. the owner that may update the value after the first frame.

For the current Forest package, the optional raw LUT record is:

```json
"externalTonemapLut": {
  "path": "lighting/AgX_Powerful_RGBAHalf_32.bytes",
  "dimension": 32,
  "format": "rgba-half"
}
```

That file must be exactly `32 * 32 * 32 * 8 = 262144` bytes. Its current
SHA-256 is
`71352890a0560d680be154567e5e01cbd9b41fa0eb5997029ec7cedb3a42795f`.
The framework creates one linear, one-mip `Texture3D` and uses External tone
mapping for the day operation. If the file or byte count is invalid, package
verification fails before the operation appears.

## Implement each mission time as a complete presentation

`operations[].timeCodes` only exposes choices in the briefing flow. It does
not change the world by itself. The framework must map the selected code to a
complete scene presentation after the exact scene is active.

The current Forest `1100` contract uses the authored `Nice Sun`, a physically
based sky, automatic-histogram exposure, and the verified AgX external LUT.
The current `0200` contract uses the shipped PVP-night evidence:

| State | Current value |
| --- | --- |
| `GameManager.SetNVGColor` | `0`, the current-build white-phosphor choice |
| primary light temperature | `9754 K` |
| primary light intensity | `40 lux` |
| soft ambient directional light | `3500 lux`, no shadows, `6570 K` |
| exposure compensation | `1.16` |
| histogram lower limit | `5.0652819` |
| histogram upper limit | `9.3485708` |
| tone mapping | ACES, not the day external LUT |
| color contrast/saturation | `17.3` / `22` |
| indirect diffuse multiplier | `1` |

The day LUT must not remain active at `0200`. It crushes the low-light signal
before the NVG custom pass can amplify it. The soft ambient source must show
terrain and foliage outside the ECOTI overlay without producing a second set
of hard shadows. The framework captures the previous NVG-color value and
restores it on operation teardown.

Treat every value above as current-build evidence. Reinspect the shipped
night scene and `GameManager.SetNVGColor` after an OPERATOR update.

## Diagnose optics through the stack

Red dots, lasers, bloom, and perceived brightness depend on exposure, tone
mapping, bloom, player cameras, custom passes, and light setup. Compare them
at matching graphics settings and a normal player camera before editing an
optic material.

Use this fault order:

1. prove that the expected player camera and its HDRP custom passes are
   active;
2. prove that the correct global Volume wins by priority and weight;
3. prove exposure and tone mapping before editing emissive materials;
4. prove the sun and shadow-system owner;
5. prove bloom and post-exposure state;
6. only then inspect the optic, laser, or reticle material.

An ECOTI-visible image with a black area outside the overlay usually means the
base night signal is absent or crushed. It does not by itself prove an ECOTI
material fault.

## Separate fire, smoke, fog, and scorch responsibilities

Fire, smoke, atmospheric fog, and ground scorching are different effects.
Keep a wreck's emissive flame near the fuel/source, make the smoke rise and
expand into a coherent column, let global or local volumetrics provide aerial
perspective, and use a surface-conforming scorch only for ground damage. Do
not swap fire and smoke roles or build a plume from conspicuous independent
camera-facing squares.

For a current HDRP/VFX Graph candidate, investigate Unity's six-way-lit smoke
workflow before inventing a custom billboard shader. Unity describes this as
an efficient approximation used by AAA productions: bake a simulated volume
into two six-direction lightmaps plus alpha/emissive data, then render it with
HDRP's Six Way Smoke Lit output so the plume responds to scene lights. Use
curl-noise turbulence, drag, coherent upward advection, lifetime size/alpha
curves, randomized flipbook phase and rotation, soft-particle depth fading,
camera fade, and fog integration to remove repeated-puff and hard-intersection
artifacts. Useful primary references are:

- <https://unity.com/blog/engine-platform/realistic-smoke-with-6-way-lighting-in-vfx-graph>
- <https://docs.unity3d.com/Packages/com.unity.visualeffectgraph@17.0/manual/Block-Turbulence.html>
- <https://docs.unity3d.com/Packages/com.unity.visualeffectgraph@17.0/manual/Context-OutputSharedSettings.html>
- <https://github.com/Unity-Technologies/VisualEffectGraph-Samples>

HDRP Fog Volume Shader Graph and Local Volumetric Fog can add low-frequency
density and light scattering, but each voxel has a GPU cost and a box-shaped
volume can reveal itself when oversized. Keep volumes bounded, inspect local
volumetric overdraw, and use a 3D density input rather than a uniform box:
<https://github.com/Unity-Technologies/Graphics/blob/master/Packages/com.unity.render-pipelines.high-definition/Documentation~/create-a-fog-volume-shader.md>.

This is an evaluation workflow, not an automatic compatibility claim. Before
shipping a VFX Graph asset, fingerprint the retail player's VFX/HDRP runtime,
author with the matching package generation, prove that the effect and every
shader dependency survive the AssetBundle boundary, and test one isolated
plume in game. Retain the current particle fallback until that spike passes.

Scorch textures and conforming meshes require their own edge gate. Verify
zero-alpha borders, mip behavior, alpha mode, cutout threshold, depth offset,
terrain intersection, and fog occlusion at close, middle, and far distances.
A black halo, flickering ring, visible square, or distant mark that renders
through fog rejects the effect even when renderer/material audits pass.

## Use player-camera proof

Offscreen cameras can miss camera-bound vegetation or use an invalid HDRP
history. They are diagnostic only. Player-camera evidence MUST inspect:

- one sun/shadow direction;
- terrain blend at near and middle distance;
- foliage silhouettes, depth, shadows, and wind;
- roughness/normal response on rocks and cover;
- no blank sky/void at boundaries;
- representative optics after the environment is matched.

Run the proof at `1100` and `0200`. Wait for exposure adaptation before the
final capture. Then inspect the runtime log for:

```text
time=<selected time code>
sunLux=<expected intensity>
nightAmbient=<true only at night>
whitePhosphor=<true at 0200>
externalLut=<true for day, false for current night contract>
```

Also inspect active renderers at the first frame and later checkpoints. The
release gate fails if a portable/error shader returns, a foliage texture
becomes null, or a later controller replaces the operation-owned Volume.

## Restore every process-global change

On restart, abort, return to armory, and plugin unload:

- destroy only the operation-owned ambient light and Volume root;
- destroy operation-created `VolumeProfile` and `Texture3D` objects;
- restore the captured NVG color;
- restore or release the prior render owner according to the captured-state
  record;
- remove callbacks and clear static references;
- do not destroy a shipped light or profile that the operation did not own.

One successful first launch is insufficient. Run day, night, restart, abort,
and return-to-armory in one process. A second operation must not inherit the
first operation's exposure, LUT, white-phosphor choice, or ambient light.
