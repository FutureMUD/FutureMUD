#!/bin/bash
set -euo pipefail
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$HOME/.dotnet:$PATH
export DOTNET_EnableWindowsTargeting=true
dotnet test "MudSharpCore Unit Tests/MudSharpCore Unit Tests.csproj" -c Release -p:EnableWindowsTargeting=true
