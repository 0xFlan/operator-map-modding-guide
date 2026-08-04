# Native Cerberus UI reference

Use private clones of shipped visual objects for the Modded Operations tab,
rows, operation board, selector, modal, Back, Cancel, Confirm, and fullscreen
controls.

Do not append package operations to official mission arrays. Shared indexes
can resolve to different content on peers when catalogs differ.

Bind clean package data before a private board becomes active. Do not pass it
through a retail setup path that requires a retail mission graph. Keep package
selection private and immutable from row click through restart.

The framework owns UI, selection, exact scene loading, readiness,
native-compatible mode state, generic population, failure UI handoff, and
restart. The map companion MUST NOT own these tasks.

## Package data to native UI mapping

Use the closed package fields as the only mission-bearing source:

| Package field | Native presentation result |
| --- | --- |
| operation `displayOrder` | private row order |
| `displayName` | row title, briefing title, target name, Confirm text |
| `areaOfOperation` | row area and briefing area line |
| `sitrep` | briefing body |
| map `previewImage` | preparation map, fullscreen map, infiltration background |
| infiltration array | native `MapInfilMarker` clones and order |
| infiltration `mapPositionX/Y` | normalized UI anchors, not 3D spawns |
| `timeCodes` and `defaultTimeCode` | native time selector and target-package records |
| map `scenePath` | exact operation scene target |

The preview is a verified raw JPEG or PNG outside the AssetBundles. Decode it
with `File.ReadAllBytes` and `ImageConversion.LoadImage`. Set clamp wrapping
and preserve aspect ratio. Cache the texture/sprite by immutable map ID and
destroy both on framework unload. One map-level preview is shared by every
operation in that map.

Build the package infiltration map from a private root, one preview
background, and one shipped `MapInfilMarker` visual clone per manifest record.
Replace `MaxPlayers`, `InfilName`, ground/heli/exfil flags, board owner,
`MarkerIndex`, and selection state. Set both anchor bounds to the normalized
manifest position. Verify the cloned marker count and every mission-bearing
field after `InfilSelectorDisplayer.SpawnMap`.

Do not use the 2D infiltration position as a player-spawn coordinate. The
selected `spawnSet` and current-scene player-marker transforms own 3D spawn.

Current implementation members are `SelectCatalogOperation`,
`FormatCatalogBriefing`, `UpdateCatalogOperationBoard`,
`GetOrLoadPreviewSprite`, `ReplaceNativeMapPreview`,
`BuildPackageInfiltrationMapPrefab`, `PrimeNativeInfiltrationSelector`, and
`InvokeNativeBoardStart` in `CerberusNativeTabFix.cs`.

Test physical pointer input. Test all tabs, repeated tab switching, Back,
Cancel, Confirm, reopen, restart, and official-row isolation. One physical
click MUST cause one logical transition.

Also test the preparation preview, fullscreen preview, every infiltration
label/position/player limit, all time codes, default time, and briefing text.
If preview decoding fails, fail the package presentation. Do not leave a
retail map image in the private board.

Package loading can outlive the Confirm frame. Capture the exact player-owned
`MissionLaptop` and `PlayerNetworking` before I/O. Keep the private modal
visible and disabled. Restore only that captured field on the same laptop when
needed. Close the modal and call the native start in the same final frame.
