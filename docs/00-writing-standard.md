# Documentation language and evidence rules

This repository uses the principles of ASD-STE100 Simplified Technical
English, Issue 9. The repository does not reproduce the standard. Get the
current standard from the [official ASD-STE100 site](https://www.asd-ste100.org/).

## Language rules

Use these rules in all technical documents:

- Use one instruction in each sentence.
- Use the active voice when the actor is known.
- Use `MUST` for a requirement.
- Use `MUST NOT` for a prohibition.
- Use `CAN` for a capability.
- Use `MAY` for permission.
- Keep sentences short.
- Put conditions before actions when this makes the sequence clear.
- Use the same term for the same object.
- Define a project-specific technical term before first use.
- Use a numbered list for a required sequence.
- Use a table for exact ownership, field, or status mappings.
- Do not use a vague term such as *thing*, *works*, *correct*, or *ready*
  without a measurable condition.

Code identifiers, Unity type names, shader property names, file names, and
OPERATOR UI labels are approved project technical terms. Do not change their
spelling to make them ordinary English words.

## Evidence labels

Each method has one of these labels:

| Label | Meaning |
| --- | --- |
| `SUPPORTED` | The complete method passed a current-build runtime test for its stated scope. |
| `PROVEN-STATIC` | Installed metadata or serialized data proves the structure. The runtime behavior is not fully proved. |
| `EXPERIMENTAL` | The method is technically possible, but the complete runtime matrix did not pass. |
| `RETIRED` | Keep the method only for historical or diagnostic use. Do not use it for a new standalone mission. |

Do not promote a static observation to `SUPPORTED`. A generated interop member
proves that the member exists. It does not prove that a call is safe. An
AssetRipper object proves that serialized content exists. It does not prove
that the object has valid runtime references.

## Claim rules

A release statement MUST name the tested scope. Use separate claims for:

- exact scene load;
- native material reconstruction;
- terrain and collision;
- player first spawn;
- player respawn;
- PVE actor creation;
- AI navigation;
- normal Restart Operation;
- death/KIA Restart Operation;
- PVP isolation;
- multiplayer host and client behavior.

One passed claim does not imply another claim.

## Public-data rules

Do not publish installed game binaries, extracted game assets, private logs,
credentials, machine-local paths, or private test controls. Publish methods,
schemas, source code that you own, and small original examples. If a workflow
uses an OPERATOR asset, tell the user to obtain it from a legal local install.
