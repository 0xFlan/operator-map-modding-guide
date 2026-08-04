# 12. Model, texture, material, foliage, and terrain contracts

Status: `SUPPORTED` for the documented portable-transport and exact-scene
rehydration method. A specific asset family is supported only after its own
player-camera test passes.

## Model contract

Record these values before import:

- source root and all child transforms;
- source unit scale and handedness;
- pivot position and rotation;
- mesh name and highest authored LOD;
- vertex and index count;
- submesh count and order;
- material slot for each submesh;
- bounds and orientation;
- normals and tangents;
- UV channels;
- vertex color and alpha channels;
- skin, bones, and bind poses when applicable;
- collider type, dimensions, and offset;
- LOD group transition values;
- billboard relationship;
- interactive component graph.

Do not collapse child transforms when a component, material, or pivot depends
on them. Do not merge submeshes when native materials use different shader
families. Do not recalculate normals, tangents, or vertex colors without an
explicit comparison.

Use the highest authored LOD for a directly placed release object. A deliberate
LOD0-only root is valid when its complete renderer hierarchy is present. A
validator MUST treat a mesh hierarchy without `LODGroup` as occupied content.

## Pivot and placement contract

The mesh origin, transform pivot, and physical contact point are different
concepts.

- Use the authored mesh pivot for static props when it matches the source.
- Use the hinge axis for a door pivot.
- Use the actor foot point for a spawn marker.
- Use the complete collider footprint for sloped cover.
- Use the terrain sample at each support point for grounding.

If a rigid prop cannot sit on the measured grade, move or remove it. Do not
create an artificial shelf to hide a bad placement.

## Texture contract

Classify a texture by its native property and data meaning. Do not classify it
only by its file name.

| Texture role | Color space | Alpha | Mips | Important checks |
| --- | --- | --- | --- | --- |
| Base color | sRGB | Preserve when used for cutout or tint | Usually required | RGB tint, alpha channel, wrap, filter, compression |
| Normal map | Linear data | Native channel convention | Required for distance use | Import as normal data, correct strength, no sRGB conversion |
| Mask map | Linear data | Channel data | Usually required | Metallic, AO, detail, smoothness channel mapping |
| Height map | Linear data | Data, not opacity | Depends on use | Bit depth, channel, scale, bias |
| Thickness/transmission | Linear data | Data | Usually required | Native destination property and range |
| Detail map | Follow native contract | Data | Required when tiled | Tiling, blend, channel mapping |
| Tone-map LUT | Linear data | Half-float component | One mip | Exact 3D dimension, raw byte length, no color conversion |
| Terrain height payload | Linear data | Not applicable | Not a sampled art texture | Lossless sample order and exact resolution |
| Terrain weight payload | Linear data | Layer weights | Not a sampled art texture | Exact channel-to-layer order and normalization |

Do not assume that a PNG with transparent pixels will render as foliage. The
material controls alpha testing, depth, shadow, culling, and queue behavior.

Preserve the source mip chain when possible. If Unity regenerates mips, test
alpha coverage at middle and far distance. A correct close texture can lose
the crown when distant cutout mips remove too much leaf coverage.

## Portable material record

Create one transport record for each native material identity.

Record:

- original material name or stable local identity;
- native shader family;
- render queue;
- surface and blend type;
- alpha-test enable and cutoff;
- culling or double-sided state;
- depth and shadow pass state;
- keyword set;
- base color and tint;
- normal scale;
- mask and smoothness controls;
- wind, transmission, and foliage controls;
- UV tiling and offset;
- every source texture property;
- every proxy cargo property and its runtime destination.

The proxy shader only transports data through the external project. It is not
the final shader contract.

## Dependency-bundle representation

Put the complete portable asset closure in a dependency bundle that loads
before the scene bundle. A correct source file is not sufficient when it is
not a Unity dependency or an explicitly address-loaded asset in that bundle.

For each model family, record this closure:

```text
complete prefab or mesh asset path
-> submesh/material slot order
-> raw material identity record
-> portable proxy material
-> base/alpha, normal, mask, and every special texture
-> collider/pivot/LOD dependencies
-> dependency bundle name
-> emitted GetAllAssetNames() addresses for run-time loads
```

The scene can reference a prefab from the dependency bundle. Build the
dependency and scene definitions in one Unity bundle-build operation so Unity
does not embed a second private copy of the same dependency in the scene
bundle. Explicitly include a payload that companion code loads by address even
when no scene renderer references it.

Keep the raw mission preview out of this closure. It is a verified package
file decoded by the generic framework, not a Unity material texture. Keep a
raw external `rgba-half` LUT outside the bundle when the manifest declares it
through `externalTonemapLut`; a serialized `Texture3D` used by map code is a
different asset and belongs in the dependency bundle.

## Exact-scene material rehydration

Use this sequence:

1. Read the preserved native identity.
2. Select the installed native shader family from audited evidence.
3. Create a fresh `Material` from that shader.
4. Apply native render queue and surface state.
5. Apply base color, tint, alpha cutoff, and numeric controls.
6. Bind base/alpha, normal, mask, and special maps to native properties.
7. Apply culling, double-sided, depth, shadow, and required pass state.
8. Apply only the audited native keywords.
9. Remove or disable proxy cargo meaning.
10. Bind the material to one map-owned renderer or terrain owner.
11. Keep a map-owned handle for teardown.
12. Audit the live material after several rendered frames.

Do not mutate a globally shared installed material. Do not use an
`InternalErrorShader` source with `CopyPropertiesFromMaterial`. Do not select
a foliage, bark, terrain, or rock profile from a wrapper name such as
`NATIVE_PROXY_*` or `MOD_*`.

## Foliage material contract

For each leaf material, verify:

- alpha cutout is enabled;
- alpha cutoff matches the native family;
- base-map alpha reaches the correct native property;
- double-sided or culling state matches the source;
- depth and shadow passes are enabled as required;
- normal and mask textures use the correct properties;
- material-type mask matches the native foliage family;
- wind and vertex-color controls match the mesh data;
- transmission or thickness data is present when required;
- render queue matches cutout geometry;
- middle and far mips preserve crown mass.

Ordinary alpha blending is not a cutout repair. It can break depth order,
shadows, and optical sights.

## Tree-family contract

A complete tree family needs:

- trunk and branch geometry;
- leaf-card geometry with valid UV and vertex data;
- matching bark and leaf material identities;
- complete base, normal, mask, and special maps;
- intended LOD or explicit LOD0-only policy;
- collider or explicit non-collision policy;
- stable root placement and scale;
- complete crown silhouette.

Validate each selected family at these views:

1. close front;
2. close side;
3. middle distance;
4. far distance;
5. backlit or grazing-light view;
6. optic view when the map uses long sight lines.

Reject a family that reads mainly as bare trunks. Do this even when mesh,
submesh, and material closure is structurally valid. Use an audited complete
family. Do not hide the defect with more copies of the same tree.

## Foliage placement contract

Use deterministic nonuniform placement. Vary position, yaw, spacing, species,
and compatible scale. Preserve routes, spawn sight lines, door clearance,
boundary visibility, and performance budgets.

Sample the final collision surface for each root. Use a bounded root embed that
matches the tree family. Do not place all objects at one constant world Y.

Keep dense decorative foliage outside critical movement lanes. A decorative
renderer can have no collider. A trunk that acts as cover needs a complete
collider and projectile test.

## Terrain data contract

Record:

- origin;
- width, height, and length;
- heightmap resolution;
- alphamap resolution;
- base-map resolution;
- detail resolution and patch size;
- each terrain layer and exact order;
- diffuse, normal, and mask textures per layer;
- tile size and offset;
- normal scale, metallic, and smoothness;
- height sample encoding and order;
- surface-weight encoding and channel order.

The current runtime package contract can carry a lossless 16-bit normalized
height payload and a three-channel 8-bit normalized surface-weight payload.
The exact manifest declares the encoding, resolution, origin, size, and three
layers.

## Runtime terrain reconstruction

1. Verify the exact package and scene.
2. Load and length-check each declared payload.
3. Create one new native `TerrainData`.
4. Set all declared resolutions before sample arrays.
5. Set the terrain size.
6. Create three native terrain layers from installed-compatible materials and
   declared textures.
7. Decode heights into an IL2CPP-compatible two-dimensional array.
8. Decode and normalize the three surface weights.
9. Set heights and alphamaps.
10. Bind the same `TerrainData` to `Terrain` and `TerrainCollider`.
11. Apply terrain material ownership.
12. Call `Physics.SyncTransforms`.
13. Verify corner, center, spawn, route, and marker height samples.
14. Fail the world contract when render and collision do not agree.

Keep managed wrapper fake-null checks separate from ordinary C# null checks.

## Gameplay edge and visual apron

Use separate envelopes:

1. Put gameplay walls at the intended playable limit.
2. Continue native terrain height, normals, layers, and weights beyond the wall.
3. Keep a visual apron wide enough to hide the edge from the player camera.
4. Start render-only distant terrain outside that apron.
5. Sample the same world-space height and weight functions at the handoff.
6. Put horizon vegetation outside the gameplay wall.
7. Test the boundary at player height and grazing light angles.

Do not place a grass-to-dirt or terrain-to-mesh seam on the gameplay wall. Do
not hide a seam with a narrow color stripe or a second light.

## Runtime audit

After more than one rendered frame, record:

- active renderer count;
- material-slot count;
- null material count;
- proxy shader count;
- error shader count;
- renderer-to-native-family mapping;
- terrain material and layer mapping;
- texture dimensions and native property bindings;
- selected tree-family instance and LOD counts;
- close, middle, and far crown captures;
- terrain render/collider height agreement;
- boundary seam captures;
- teardown material and native-object counts.

Static bundle closure and editor screenshots are not enough.
