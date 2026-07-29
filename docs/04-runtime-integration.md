# 4. Runtime integration

## Keep injection configurable

A reusable injector should start disabled and require:

- an explicit target scene name and/or build index;
- an explicit local bundle path;
- an explicit prefab asset path;
- an explicit decision about which source roots, if any, may be suppressed.

Never hard-code a particular map bundle as a default. Never broadly destroy
unknown scene objects. Configuration should be local, reversible, and
documented.

## Use OPERATOR MapBridge

OPERATOR MapBridge is deliberately an injector, not a map generator or a
preconfigured map loader. Its safe workflow is:

1. Build the toolkit against the user's local OPERATOR/BepInEx IL2CPP
   installation and install only its DLL. It ships no map bundle.
2. Start the game once with the toolkit disabled so BepInEx writes its config.
3. With the game closed, set `Enabled=true`, one exact `SceneName` and/or
   `BuildIndex`, `LocalBundlePath`, and the exact lower-case
   `PrefabAssetPath` reported by the bundle. If both scene identifiers are
   supplied, both must match.
4. Keep `SceneRootNamesToDisable` empty for the first test. This is overlay
   mode: the custom prefab is added while the original scene remains alive.
5. Only after a target-scene inventory proves which roots are disposable
   static geometry, add their exact root names one at a time. Never suppress
   spawn, networking, camera, UI, game-mode, lighting, or unknown roots.
6. Test locally, inspect the toolkit log for the exact scene/bundle/prefab
   handoff, then disable the plugin again before changing bundles.

The configured bundle path is local. Relative paths resolve under the
toolkit's plugin directory; absolute local paths are also supported. The
toolkit deliberately does not download a map, discover the first asset in a
bundle, remap spawns, repair materials, or claim that an overlay is playable.

## Do not load a large bundle inside a scene callback

Scene activation callbacks are timing-sensitive. Queue map application, then
load during a normal update after the target scene has settled enough for the
game runtime. Record the scene name/index/handle, bundle path, prefab path,
and failure reason in the log.

## Own only your roots

Give injected objects a distinctive runtime root. Move that root into the
target scene, track its lifetime, and release it when the target unloads.
Avoid changing global state unless the target-specific evidence authorizes it.

## Rehydrate materials intentionally

Unity editor shader GUIDs may not resolve in the installed runtime. A bundle
can therefore load with fallback/flat materials even when it looked correct in
the editor. If the target game needs a material repair stage:

1. identify the native material identity before classifying it;
2. create a material from the installed shader family;
3. restore the dependency closure and source tint;
4. apply the audited property/keyword/render-state profile;
5. audit the final live material after more than one rendered frame.

Do not select a profile from a proxy wrapper name. Do not use a generic
transparent shader as a shortcut for foliage.

## Reconstruct runtime-only data only when proven necessary

Some Unity object types may be present in a bundle container but fail to bind
as usable native objects in the target IL2CPP runtime. Test native-aware
validity, not only a managed null check. If a required collision data object
cannot survive transport, package lossless source payloads and reconstruct it
with target-runtime-compatible arrays before any player/spawn handoff.

If reconstruction fails, abort the replacement rather than leaving players
above a non-colliding surface.
