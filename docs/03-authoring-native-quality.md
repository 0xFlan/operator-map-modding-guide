# 3. Authoring a native-quality map

## Build the playable substrate first

Before placing decorative content, create and test:

1. collision-bearing terrain or another verified walkable surface;
2. player-height and camera-height clearances;
3. navigable routes and mode-compatible spawn locations;
4. boundary collision plus an exterior visual buffer;
5. a plan for terrain material, normals, texture mips, and lighting.

An attractive ground mesh with no valid collider is not playable terrain.
Likewise, distant scenery alone is not a boundary system.

## Use complete asset closures

For every direct asset, retain the complete relationship between:

- full mesh or prefab hierarchy;
- intended highest-detail representation;
- matching material;
- base-color and alpha-bearing data;
- normal, mask, thickness, height, or other required maps;
- collider intent;
- material keywords, render state, color/tint, queue, and property values.

Do not use a branch, root fragment, billboard, loose atlas image, generic
primitive, or approximately named material as a substitute for a complete
native asset.

## Foliage is a rendering contract

Opaque rectangular leaf cards are usually not a missing-PNG-transparency
problem. They indicate that the game-specific material contract is wrong.

Validate the target family’s:

- installed shader;
- base/normal/mask bindings;
- alpha test/cutoff;
- render queue and material-type values;
- double-sided/culling mode;
- depth, shadow, and motion-vector behavior;
- wind, transmission, normal, and vertex-AO controls;
- color precedence and tint state.

Raw authoring materials can be useful serialized evidence even if they cannot
render in a portable Unity project. Do not blindly copy an error/fallback
material onto a live installed shader. Create a live material from the
installed family, restore readable maps/tints from a transport proxy, then
apply only values audited for that family.

### Required foliage repair sequence

Use this exact order when leaves, bushes, or grass show as opaque rectangles,
grey cards, or flat shapes in the game:

1. Confirm the mesh is the complete authored LOD0/full prefab, not a branch,
   billboard, or a stripped child.
2. Record the matching shipped material identity and its installed shader.
   In the current OPERATOR family, opaque bark/rock and cutout foliage use
   different BotD shader families; re-check the installed build rather than
   assuming a name or a portable editor shader is authoritative.
3. Package every map the material reads: base color with alpha where used,
   normal, mask, and thickness/height/detail maps when the family requires
   them. Preserve color space and mips.
4. At runtime create the material from the installed shader, restore maps and
   tint, then apply the recorded queue, alpha-test/cutoff, material-type,
   culling/double-sided, depth/shadow, wind, transmission, normal, and
   vertex-color/AO state.
5. Inspect it from a normal player camera after the scene has rendered for
   several frames. An editor preview or a transparent PNG is not proof.

Use alpha **cutout**, not generic alpha blending, unless the shipped material
uses blending. Cutout foliage must participate in depth and shadows as its
source does; replacing it with a generic transparent material commonly fixes
the rectangle while breaking lighting and ordering.

## Terrain, roots, props, and cover

Sample the final collision surface for every placement. Use controlled root
embedding rather than a constant world Y:

- trees need full trunks and terrain contact;
- bushes and grass need small downward embeds;
- rigid cover must be tested across its full footprint;
- incomplete or one-sided rock meshes must be rejected from a multi-angle
  player-height review.

For rotated rigid cover, sample the final collision height at the complete
mesh/collider center and all footprint corners after applying the cover's yaw.
Flatten only the actual support footprint plus a short feather when required.
An axis-aligned early-out must conservatively enclose the rotated footprint;
otherwise grass or props near a rotated support pad can retain the old hillside
height and float.

Avoid rows. Use deterministic but nonuniform position, yaw, spacing, species,
and scale variation. Preserve deliberate paths and spawn/camera clearances.

## LOD and texture quality

Use the highest authored source for direct playable content. Do not turn a
lower LOD or billboard into the map's intended high-quality representation.
Keep original texture resolution, correct color space, compression role,
filtering, anisotropy, and mip chains. Validate the emitted bundle, not only
the authoring inspector.

If official vegetation uses terrain detail or a BatchRendererGroup route, a
direct GameObject fallback can match visible density but not automatically
match interaction, culling, or performance. State that limitation until a
normal player-camera test proves the native path.
