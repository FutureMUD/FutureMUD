#!/bin/bash
set -euo pipefail
curl -sSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
bash /tmp/dotnet-install.sh --channel 9.0 --install-dir "$HOME/.dotnet"
if ! grep -q "DOTNET_ROOT" ~/.bashrc; then
  echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
  echo 'export PATH=$HOME/.dotnet:$PATH' >> ~/.bashrc
fi
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$HOME/.dotnet:$PATH
