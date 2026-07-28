# Asset provenance manifest template

Use one row per asset family or external dependency. Do not put private paths
or unlicensed content in a public manifest.

| Field | Example value |
|---|---|
| Logical asset family | Custom conifer tree set |
| Distribution status | Original work / user-local extraction / licensed third party |
| Allowed in public repository | Yes / No / permission required |
| Included payload | Source only / generated bundle / no payload |
| Mesh/material closure verified | Yes, date and validator reference |
| Runtime shader contract verified | Yes, target build and test reference |
| LOD/collider verified | Yes, method and result |
| Game-camera QA complete | Yes / No |
| Notes | Known limitation or renewal trigger |

If a dependency is not permitted for public release, omit the payload and
document only the lawful local preparation step.
