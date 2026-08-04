# Native rendering reference

## Asset closure

Preserve the complete root, child transforms, highest-detail mesh, submesh
order, material slots, base/alpha texture, normal map, mask map, special maps,
colliders, and LOD relationship.

Do not replace a complete object with a branch, root fragment, billboard,
one-sided mesh, or approximate material.

## Portable material transport

An external Unity project cannot usually compile OPERATOR private HDRP shader
graphs. Preserve native material identity and all texture/property data in a
portable proxy. In the exact scene, create a fresh material from the installed
native family and apply the audited render state.

Do not use the proxy name to classify the material. Do not copy from an error
shader.

## Foliage

Transparent pixels do not enable alpha cutout. Match alpha test, cutoff,
double-sided state, render queue, depth and shadow passes, normal and mask
keywords, wind, transmission, and native color values.

Test from the player camera. Require a complete crown at close, middle, and far
distance. Reject a technically complete tree family that reads as bare trunks.

After final yaw and scale, read the lowest finite child renderer bound. Align
that visible base to the sampled terrain with a declared small embed. Reject a
large correction. A source prefab pivot is not a uniform trunk-base contract.

## Terrain

Use one continuous height and surface-weight function across the playable edge
and visual apron. Do not put a terrain-to-mesh material seam on the gameplay
wall. Sample the final collision surface for each tree and prop footprint.

If portable `TerrainData` is fake-null, reconstruct one native object from
lossless height and weight payloads. Bind the same object to render and
collision components. Disable a serialized mesh fallback only after the live
`Terrain` and `TerrainCollider` share that data.

## Lighting

Find the live scene owner before changing a directional light or HDRP volume.
Keep one verified sun. Preserve light units, shadow data, lens flare, exposure,
tone mapping, bloom, white balance, and required LUT data. Use a serialized
fallback when optional data cannot load.

For the current 02:00 worked source, `sharedassets7.assets` profile path `435`
uses ACES and no external day LUT. Exposure path `440` uses compensation
`1.16` and limits `5.065281867980957..9.348570823669434`.
`GameManager.SetNVGColor(0)` selects white phosphor on this exact build.
Capture and restore the prior state.
