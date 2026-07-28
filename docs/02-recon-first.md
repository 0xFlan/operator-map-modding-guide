# 2. Inspect before intervention

## Learn the actual target

Before authoring a replacement, inspect the installed target scene and its
game-mode path. Record:

- build index and scene name;
- root hierarchy and which roots are safe static geometry;
- player spawn transforms and their coordinate space;
- game-mode methods that choose a spawn and move an avatar;
- terrain, collider, navigation, and out-of-bounds owners;
- light, sky, HDRP Volume, camera, and quality owners;
- ordinary Renderer versus terrain/detail/BatchRendererGroup ownership.

Names are clues, not evidence. A controller with a weather-like name may not
own the active sun. A tree that cannot be found through Renderer enumeration
may be a GPU-instanced terrain/detail object. A marker moved in the hierarchy
may still be ignored by a late game-mode spawn path.

## Build a target inventory

Make a small written inventory for each target map:

| Area | Questions to answer |
|---|---|
| Scene | What exact name/index identifies it? |
| Gameplay | Which mode owns first spawn, respawn, and player movement? |
| Ground | What is the collision-bearing surface? |
| Rendering | Which shader families and HDRP owners are active? |
| Vegetation | Is it GameObject, Terrain detail/tree data, BRG, or mixed? |
| Lighting | Which single root owns the active directional sun? |
| Boundaries | What stops and visually hides the edge? |

Use source and normal game-camera evidence. An editor preview, a static asset
record, or a one-frame offscreen camera capture is not enough by itself.

## Establish a comparison plan

For every visual claim, choose comparable evidence:

- normal player-camera view of a shipped reference scene;
- material/shader property audit where available;
- normal player-camera view of the custom map;
- close and grazing-angle prop checks;
- runtime logs after the map has settled.

This prevents fixes that make one isolated screenshot look better while
breaking alpha cutout, shadows, depth, LOD, player spawn, or a later frame.
