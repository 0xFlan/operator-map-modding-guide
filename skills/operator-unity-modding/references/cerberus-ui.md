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

Test physical pointer input. Test all tabs, repeated tab switching, Back,
Cancel, Confirm, reopen, restart, and official-row isolation. One physical
click MUST cause one logical transition.
