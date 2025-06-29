#!/bin/bash
set -euo pipefail
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$HOME/.dotnet:$PATH
export DOTNET_EnableWindowsTargeting=true
 dotnet build MudSharpCore/MudSharpCore.csproj -c Release -p:EnableWindowsTargeting=true
