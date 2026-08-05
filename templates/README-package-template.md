# Package manifest template

Copy `operator-map-package.example.json` to the root of a new package and
rename it to `operator-map-package.json`.

The example is a schema-shape template. It is not an installable package. Its
three file records intentionally use zero lengths and zero hashes. Replace
all example IDs, text, paths, byte counts, and hashes with final values from
your own staging directory.

Use this order:

1. Make the package directory name equal the new `packageId`.
2. Replace `author.example-map`, its map ID, and both operation IDs. Keep all
   IDs under the package namespace.
3. Change visible display and SITREP text.
4. Make `sceneBundle` and `dependencyBundles[]` match the staged bundle file
   paths.
5. Make `scenePath` equal the exact `GetAllScenePaths()` value.
6. Put the final JPEG or PNG at `previewImage`.
7. Match each operation `spawnSet` to one inactive scene object named
   `SPAWN_SET_<spawnSet>`.
8. Match the map ID to one inactive scene object named `MAP_ID_<mapId>`.
9. Set 2D infiltration anchors against the final preview. Do not use them as
   3D player-spawn coordinates.
10. For PVE, set valid `minEnemies` and `maxEnemies`. For PVP, omit both
    fields.
11. After all file copies finish, regenerate `files[]` from every regular
    package file except the manifest. Use ordinal path order, exact final byte
    counts, and lowercase SHA-256.
12. Run the strict package loader before you build or deploy the archive.

Read
[`docs/03b-modded-operations-presentation.md`](../docs/03b-modded-operations-presentation.md)
and [`docs/10-standalone-packages.md`](../docs/10-standalone-packages.md)
before you use this template.
