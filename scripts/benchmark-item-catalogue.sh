#!/usr/bin/env bash
set -euo pipefail
# Use the same implementation and statistical summary on both platforms.
# Requires PowerShell 7, as do the existing source-catalogue audit tools.
task_root="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.." && pwd)"
exec pwsh -NoProfile -File "$task_root/scripts/benchmark-item-catalogue.ps1" "$@"
