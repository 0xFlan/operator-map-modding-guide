# 8. Troubleshooting

| Symptom | First evidence to collect | Typical responsible layer |
|---|---|---|
| Opaque foliage rectangles | live shader, alpha test, queue, tags, culling, keywords, maps | material rehydration |
| Flat or grey props | final shader, map bindings, tint precedence, bundle dependencies | material closure |
| Low-detail trees | mesh topology, LODGroup state, game quality owner | authored asset/LOD policy |
| Floating trees or cover | terrain samples across the whole footprint, root/collider bounds | placement/grounding |
| Open or invisible boulder | mesh closure, matching material, multi-angle player view | source mesh/material choice |
| Road-like terrain | terrain layer maps, normals, color space, mips, runtime material | terrain material |
| Blank edge/sky | collision terrain extent, visual exterior buffer, boundary position | bounds/exterior |
| Player starts high or falls | source markers, game-mode path, first network-object order, ground availability | spawn handoff |
| Double sun or odd shadows | scene light hierarchy, controller ownership, mod-created lights | lighting ownership |
| Different red dot/laser | player-camera exposure, tone map, bloom, optic camera/custom pass | environment stack |
| Works in editor but not game | bundle platform/version, installed shader availability, IL2CPP object validity | runtime integration |

Do not fix a symptom by adding unrelated geometry, light, or generic shaders.
Trace it to the narrowest responsible layer, add a regression check, and
record what evidence ruled out the alternatives.
