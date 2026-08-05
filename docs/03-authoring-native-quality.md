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

### Keep the gameplay boundary separate from the visual terrain boundary

Do not end native `Terrain`/TerrainLayer rendering on the same contour as an
invisible wall. Keep the player-accessible area and the terrain-rendering area
as separate, explicit envelopes:

1. Put the invisible walls at the intended gameplay boundary.
2. Continue the same native `TerrainData`, terrain layers, height function,
   normals, and material family 10-15m beyond those walls as a visual apron.
3. Begin any render-only distant terrain only outside that apron. Sample the
   same world coordinates for heights and surface weights on both sides.
4. Place the horizon vegetation outside the player wall and inspect the wall
   from player height at grazing light angles.

This prevents a player from seeing a hard `Terrain`-to-mesh material switch
where grass suddenly becomes dirt or rock. Do not hide a seam with a narrow
painted color band, a second light, or a different shader family. Use a broad,
continuous world-space blend for grass, dirt, and exposed rock, then validate
weight continuity numerically and from the player camera.

## Use complete asset closures

For every direct asset, retain the complete relationship between:

- full mesh or prefab hierarchy;
- intended highest-detail representation;
- matching material;
- base-color and alpha-bearing data;
- normal, mask, thickness, height, or other required maps;
- collider intent;
- material keywords, render state, color/tint, queue, and property values.

Do not use a branch, root fragment, billboard, loose atlas image, unrelated
primitive, or approximately named material as a substitute for a complete
native asset.

### Select arena props from official-scene evidence

Before importing a tree, rock, fence, barrier, crate, or other cover item,
inspect an official reference scene in AssetRipper and record:

- the exact root GameObject and its full child hierarchy;
- the highest-detail mesh or LOD0, vertex/submesh count, and material slots;
- each required texture/map and the material/shader family that consumes it;
- collider type, bounds, and whether the object is actual cover or decoration;
- source-scene placement count and role; and
- close front, side, rear, and grazing-angle views after it renders in game.

Treat a candidate as rejected if it is an effect, billboard, loose fragment,
open one-sided mesh, incomplete hierarchy, or has an unresolved material
closure. Add a small, intentional set of verified props, then re-run spawn,
route, slope, and performance checks. Do not fill an arena by bulk-instancing
every object discovered in an asset export.

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
   several frames at close, middle, and far distances. Require a complete
   crown silhouette and readable foliage mass. An editor preview, a transparent
   PNG, or valid mesh/submesh/material counts are not proof.

Use alpha **cutout**, not ordinary alpha blending, unless the shipped material
uses blending. Cutout foliage MUST participate in depth and shadows as its
source does; replacing it with an unrelated transparent material commonly fixes
the rectangle while breaking lighting and ordering.

If a tree family still reads primarily as bare trunks through the player
camera, reject it from playable and perimeter placement even when its technical
closure is complete. Substitute an audited complete native family and preserve
its matching materials/textures; do not try to hide the silhouette defect by
adding more instances.

## Terrain, roots, props, and cover

Sample the final collision surface for every placement. Use controlled root
embedding rather than a constant world Y:

- trees need full trunks and terrain contact;
- bushes and grass need small downward embeds;
- rigid cover MUST be tested across its full footprint;
- incomplete or one-sided rock meshes MUST be rejected from a multi-angle
  player-height review.

Use LOD0 bark/trunk submesh vertices as the tree datum. Do not use the prefab
pivot, the whole-renderer minimum, or the bottom of a trunk collider. A low
leaf card or branch can corrupt the whole-renderer minimum. Some native
capsules extend below the modeled roots. Apply this exact method after final
position, rotation, and scale:

```csharp
LOD[] lods = tree.GetComponentInChildren<LODGroup>(true).GetLODs();
Renderer[] renderers = lods[0].renderers;
float trunkMinimumY = float.PositiveInfinity;
float trunkMaximumY = float.NegativeInfinity;

foreach (Renderer renderer in renderers)
{
    Mesh mesh = renderer.GetComponent<MeshFilter>().sharedMesh;
    Vector3[] vertices = mesh.vertices;
    Material[] materials = renderer.sharedMaterials;
    for (int slot = 0; slot < materials.Length; slot++)
    {
        if (!IsRecognizedTrunkMaterial(materials[slot].name))
            continue;
        foreach (int index in mesh.GetIndices(slot))
        {
            float y = renderer.transform.TransformPoint(vertices[index]).y;
            trunkMinimumY = Mathf.Min(trunkMinimumY, y);
            trunkMaximumY = Mathf.Max(trunkMaximumY, y);
        }
    }
}

float referenceSpan = GetFamilyGroundingReferenceSpan(tree, trunkMinimumY, trunkMaximumY);
Vector3 contact = FindFamilySurfaceContact(tree, sampleSurfaceY);
float familyBias = IsBroadCrownOak(tree) ? 0.25f : 0f;
float datumY = trunkMinimumY + referenceSpan * (1f / 6f) + familyBias;
tree.transform.position += Vector3.up * (contact.y - datumY);
```

`IsRecognizedTrunkMaterial` must accept the shipped `pine_bark`,
`Trunk_pine_var4`, `Bark_Mat`, and `Bark_2_Mat` slots. Define the reference and
surface contact per family. The current Forest proof uses renderer-free child
`NATIVE_TRUNK_GROUND_DATUM_ONE_SIXTH`. Pines use a center sample and full
trunk span. Broad oaks use an oriented main-stem span, `0.25 m` bias, and the
lowest sample in a bounded `0.60 m` to `2.00 m` lower-root footprint. Store
the contact X/Z in the child. Fail when its corrected world Y differs from
terrain by more than `0.001 m`, when the full rendered-tree above fraction is
below `0.75`, or when `abs(correction) > 12f`. Keep native colliders for
gameplay, but do not use a generic collider bottom as the datum.

Place the full tree batch first. Synchronize transforms once, apply the
root-contact equation to every tree, and synchronize once more. Do not call
`Physics.SyncTransforms()` once per tree against a large `TerrainCollider`.

For rotated rigid cover, sample the final collision height at the complete
mesh/collider center and all footprint corners after applying the cover's yaw.
If the full footprint cannot sit naturally on the authored grade, relocate or
remove the prop rather than creating an artificial shelf. An axis-aligned
early-out MUST conservatively enclose the rotated footprint; otherwise grass
or props near a rotated support pad can retain the old hillside height and
float.

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

## Convert authoring outputs into package data

Do not stop at a correct Unity scene. Classify each result for the standalone
package:

| Authoring result | Final location |
| --- | --- |
| complete map `.unity` scene and authored object graph | scene bundle |
| complete models/prefabs and portable material/texture closure | dependency bundle |
| address-loaded height/weight/lighting payloads | dependency bundle with exact emitted asset addresses |
| map ID and spawn-set identity | inactive named metadata objects in the scene plus matching manifest fields |
| 3D player/enemy/HVT/team points and facing | scene marker transforms |
| mission name, area, SITREP, mode, player/AI limits | manifest operation record |
| infiltration labels and 2D preview positions | manifest infiltration records |
| raw briefing/infiltration image | package `media/` file outside bundles plus `previewImage` |
| exact scene address and bundle load order | manifest map record |

Build and validate this data with
[AssetRipper to standalone scene bundles](03a-assetripper-to-bundle.md),
[Modded Operations mission presentation and bundle data](03b-modded-operations-presentation.md),
and [Standalone package format and loading](10-standalone-packages.md).
