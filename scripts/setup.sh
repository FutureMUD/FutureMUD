#!/bin/bash
set -euo pipefail
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
BOOTSTRAP_ROOT="$REPO_ROOT/.dotnet-cli"
INSTALL_ROOT="$BOOTSTRAP_ROOT/.dotnet"
INSTALL_SCRIPT="$BOOTSTRAP_ROOT/dotnet-install.sh"

mkdir -p "$BOOTSTRAP_ROOT"
curl -sSL https://dot.net/v1/dotnet-install.sh -o "$INSTALL_SCRIPT"
bash "$INSTALL_SCRIPT" --channel 10.0 --install-dir "$INSTALL_ROOT"
"$INSTALL_ROOT/dotnet" --info
