#!/usr/bin/env bash
set -euo pipefail
exec pwsh -NoProfile -File "$(dirname "$0")/measure-compiled-item-stock.ps1" "$@"
