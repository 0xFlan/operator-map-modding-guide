from __future__ import annotations

import argparse
import hashlib
import json
import re


SCHEME = "operator-loader-neutral-runtime-pair-v1"
SHA256_RE = re.compile(r"^[0-9a-f]{64}$")


class RuntimeContentIdError(ValueError):
    """Raised when a runtime-pair identity input is not canonical."""


def _bounded_text(value: str, field: str, maximum: int) -> str:
    if not isinstance(value, str) or not value or len(value) > maximum:
        raise RuntimeContentIdError(
            f"{field} must be non-empty and at most {maximum} characters"
        )
    if any(character in value for character in ("\r", "\n", "\0")):
        raise RuntimeContentIdError(f"{field} contains a forbidden character")
    return value


def _sha256(value: str, field: str) -> str:
    if not isinstance(value, str) or SHA256_RE.fullmatch(value) is None:
        raise RuntimeContentIdError(
            f"{field} must be exactly 64 lowercase hexadecimal characters"
        )
    return value


def canonical_runtime_pair(
    plugin_guid: str,
    plugin_version: str,
    bepinex_sha256: str,
    melonloader_sha256: str,
) -> bytes:
    """Return the versioned UTF-8 preimage for one exact dual-loader pair."""

    fields = (
        SCHEME,
        _bounded_text(plugin_guid, "pluginGuid", 128),
        _bounded_text(plugin_version, "pluginVersion", 64),
        _sha256(bepinex_sha256, "sha256"),
        _sha256(melonloader_sha256, "melonLoaderSha256"),
    )
    return "\n".join(fields).encode("utf-8")


def compute_runtime_content_id(
    plugin_guid: str,
    plugin_version: str,
    bepinex_sha256: str,
    melonloader_sha256: str,
) -> str:
    return hashlib.sha256(canonical_runtime_pair(
        plugin_guid,
        plugin_version,
        bepinex_sha256,
        melonloader_sha256,
    )).hexdigest()


def main() -> int:
    parser = argparse.ArgumentParser(
        description=(
            "Compute the canonical loader-neutral content ID for an exact "
            "BepInEx/MelonLoader runtime DLL pair."
        )
    )
    parser.add_argument("--plugin-guid", required=True)
    parser.add_argument("--plugin-version", required=True)
    parser.add_argument("--bepinex-sha256", required=True)
    parser.add_argument("--melonloader-sha256", required=True)
    parser.add_argument(
        "--json",
        action="store_true",
        help="Emit a compact JSON record instead of only the content ID.",
    )
    args = parser.parse_args()

    try:
        content_id = compute_runtime_content_id(
            args.plugin_guid,
            args.plugin_version,
            args.bepinex_sha256,
            args.melonloader_sha256,
        )
    except RuntimeContentIdError as error:
        parser.error(str(error))

    if args.json:
        print(json.dumps({
            "scheme": SCHEME,
            "pluginGuid": args.plugin_guid,
            "pluginVersion": args.plugin_version,
            "sha256": args.bepinex_sha256,
            "melonLoaderSha256": args.melonloader_sha256,
            "runtimeContentId": content_id,
        }, separators=(",", ":")))
    else:
        print(content_id)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
