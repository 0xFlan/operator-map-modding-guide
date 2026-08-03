# Legacy MapBridge retail-scene overlay

Status: `RETIRED` for vanilla mission-selection parity.

This method loads one local AssetBundle prefab into an already loaded retail
scene. It was useful before the standalone package method existed. It is still
useful for a bounded material, collider, or hierarchy test.

It does not do these tasks:

- register a data-only map package;
- create a Modded Operations mission row;
- bind a package-owned operation board;
- load a package-owned `.unity` scene through the operation flow;
- create standalone PVE or PVP mode ownership;
- provide exact-scene restart ownership;
- remove the retail gameplay scene from the operation.

## Historical sequence

The historical method used this sequence:

1. Build one prefab AssetBundle for `StandaloneWindows64`.
2. Configure MapBridge with an exact retail scene name.
3. Configure the local bundle path and prefab asset path.
4. Start OPERATOR with MapBridge disabled by default.
5. Enable the explicit local configuration.
6. Load the selected retail scene.
7. Instantiate the prefab as an overlay.
8. Repair map-owned rendering or spawn behavior with bounded hooks.

The retail scene still owned services, mode state, scene lifecycle, and other
content. Thus, an overlay result did not prove a standalone map.

## Permitted current use

Use this method only when the test question requires an overlay. Examples:

- Verify that a material proxy contains all required textures.
- Verify a collider shape against an installed physics layer.
- Inspect a complete imported prefab hierarchy in the player camera.
- Reproduce an old overlay defect before migration.

Label all results as overlay results. Do not use the results as proof of a
standalone mission.

## Migration to the current method

1. Put immutable map and operation data in a data-only package.
2. Build a real scene AssetBundle that contains a `.unity` scene.
3. Put world geometry, collision, bounds, and markers in that scene.
4. Let the generic Modded Operations framework own catalog, UI, scene load,
   readiness, mode lifecycle, PVE/PVP population, and restart.
5. Put installed-runtime reconstruction in a separate exact-scene companion.
6. Remove all retail donor-scene requirements.
7. Run the complete standalone validation matrix.

See [Standalone package and load order](../04-runtime-integration.md).
