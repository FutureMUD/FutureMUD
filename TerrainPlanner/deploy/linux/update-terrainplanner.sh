#!/usr/bin/env bash
set -euo pipefail
if [[ $EUID -ne 0 ]]; then echo 'Run this updater as root.' >&2; exit 1; fi
RUNTIME="${1:-}"
HOSTNAME_VALUE="${2:-}"
if [[ ! "$RUNTIME" =~ ^linux-(x64|arm64)$ ]] || [[ -z "$HOSTNAME_VALUE" ]]; then
	echo 'Usage: update-terrainplanner.sh linux-x64|linux-arm64 planner.example.com' >&2; exit 2
fi
WORK="$(mktemp -d)"
trap 'rm -rf -- "$WORK"' EXIT
BASE=https://futuremud.com/downloads/terrainplanner/latest
curl --fail --location "$BASE/update-manifest.json" -o "$WORK/update-manifest.json"
curl --fail --location "$BASE/update-manifest.sig" -o "$WORK/update-manifest.sig"
VERSION="$(python3 -c 'import json,sys; print(json.load(open(sys.argv[1]))["version"])' "$WORK/update-manifest.json")"
ARCHIVE="terrainplanner-$VERSION-$RUNTIME.zip"
curl --fail --location "$BASE/$ARCHIVE" -o "$WORK/$ARCHIVE"
/opt/futuremud/terrainplanner/current/tools/TerrainPlanner.Deployment verify --manifest "$WORK/update-manifest.json" --signature "$WORK/update-manifest.sig" --archive "$WORK/$ARCHIVE" --runtime "$RUNTIME"
python3 -m zipfile -e "$WORK/$ARCHIVE" "$WORK/package"
bash "$WORK/package/terrainplanner-$VERSION-$RUNTIME/deploy/linux/install-terrainplanner.sh" "$HOSTNAME_VALUE"
