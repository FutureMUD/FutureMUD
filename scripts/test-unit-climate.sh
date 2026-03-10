#!/bin/bash
set -euo pipefail
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$HOME/.dotnet:$PATH
export DOTNET_EnableWindowsTargeting=true
dotnet test "MudSharpCore Climate Tests/MudSharpCore Climate Tests.csproj" -c Release -p:EnableWindowsTargeting=true
