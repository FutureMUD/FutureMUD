#!/bin/bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
LOCAL_DOTNET_ROOT="$REPO_ROOT/.dotnet-cli/.dotnet"

resolve_dotnet() {
	if command -v dotnet >/dev/null 2>&1; then
		echo "dotnet"
		return
	fi

	if [ -x "$LOCAL_DOTNET_ROOT/dotnet" ]; then
		echo "$LOCAL_DOTNET_ROOT/dotnet"
		return
	fi

	echo "dotnet was not found. Run scripts/setup.sh to bootstrap a local SDK." >&2
	exit 1
}

DOTNET="$(resolve_dotnet)"
if [ "$DOTNET" != "dotnet" ]; then
	export DOTNET_ROOT="$LOCAL_DOTNET_ROOT"
	export PATH="$LOCAL_DOTNET_ROOT:$PATH"
fi

export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export DOTNET_NOLOGO=1
cd "$REPO_ROOT"

"$DOTNET" build DatabaseSeeder/DatabaseSeeder.csproj -c Debug --no-restore -m:1 "-p:NoWarn=NU1902;NU1510"
"$DOTNET" test "FutureMUDLibrary Unit Tests/FutureMUDLibrary Unit Tests.csproj" -c Debug --no-restore "-p:NoWarn=NU1902;NU1510"
"$DOTNET" test "ExpressionEngine Unit Tests/ExpressionEngine Unit Tests.csproj" -c Debug --no-restore "-p:NoWarn=NU1902;NU1510"
"$DOTNET" test "DatabaseSeeder Unit Tests/DatabaseSeeder Unit Tests.csproj" -c Debug --no-restore "-p:NoWarn=NU1902;NU1510"
"$DOTNET" test "MudSharpCore Unit Tests/MudSharpCore Unit Tests.csproj" -c Debug --no-restore -m:1 "-p:NoWarn=NU1902;NU1510"
