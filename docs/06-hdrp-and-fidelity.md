# 6. HDRP, lighting, and fidelity

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

## Copy override state, not just numbers

An HDRP profile can store a value while its override is disabled. Recreating
every stored value changes the scene. Decode or inspect a component's active
state, each parameter's override state, and the target build's field order.
Apply only values directly verified for the target scene.

External color transforms and lookup textures require the correct type, color
space, dimensions, format, and mip contract. If a verified external resource
cannot be loaded, choose an explicitly logged safe fallback rather than
leaving HDRP in an invalid partial state.

## Diagnose optics through the stack

Red dots, lasers, bloom, and perceived brightness depend on exposure, tone
mapping, bloom, player cameras, custom passes, and light setup. Compare them
at matching graphics settings and a normal player camera before editing an
optic material.

## Use player-camera proof

Offscreen cameras can miss camera-bound vegetation or use an invalid HDRP
history. They are diagnostic only. Player-camera evidence should inspect:

- one sun/shadow direction;
- terrain blend at near and middle distance;
- foliage silhouettes, depth, shadows, and wind;
- roughness/normal response on rocks and cover;
- no blank sky/void at boundaries;
- representative optics after the environment is matched.
