#!/bin/bash
set -euo pipefail
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
LOCAL_DOTNET_ROOT="$REPO_ROOT/.dotnet-cli/.dotnet"
SUITE="${1:?suite required}"
shift
STARTED=$(date +%s)
OUTPUT_MODE=human
TIMEOUT_SECONDS=
ARGS=("$@")
for ((i=0; i<${#ARGS[@]}; i++)); do
	case "${ARGS[i]}" in
		--output-mode) if ((i+1 < ${#ARGS[@]})); then OUTPUT_MODE="${ARGS[i+1]}"; fi ;;
		--timeout-seconds) if ((i+1 < ${#ARGS[@]})); then TIMEOUT_SECONDS="${ARGS[i+1]}"; fi ;;
	esac
done
bootstrap_failure() {
	local kind="$1" detail="$2" code="$3" status=INCONCLUSIVE
	if [[ "$code" == 2 ]]; then status=BLOCKED; fi
	if [[ "$OUTPUT_MODE" == json ]]; then
		printf '{"schema_version":1,"status":"%s","issues":[{"kind":"%s","detail":"%s"}]}\n' "$status" "$kind" "$detail"
	else
		printf 'Test reporting %s: %s\n' "$kind" "$detail" >&2
	fi
	exit "$code"
}
if [[ -n "$TIMEOUT_SECONDS" && ! "$TIMEOUT_SECONDS" =~ ^[1-9][0-9]*$ ]]; then bootstrap_failure INVALID_INVOCATION 'timeout-seconds must be positive' 2; fi
if [[ -z "$TIMEOUT_SECONDS" && "$OUTPUT_MODE" != human ]]; then TIMEOUT_SECONDS=1800; fi
if command -v dotnet >/dev/null 2>&1; then
	DOTNET="$(command -v dotnet)"
elif [ -x "$LOCAL_DOTNET_ROOT/dotnet" ]; then
	DOTNET="$LOCAL_DOTNET_ROOT/dotnet"
	export DOTNET_ROOT="$LOCAL_DOTNET_ROOT"
	export PATH="$LOCAL_DOTNET_ROOT:$PATH"
else
	bootstrap_failure MISSING_SDK 'dotnet was not found; run scripts/setup.sh' 2
fi
BOOTSTRAP_DOTNET="${FUTUREMUD_REPORTER_BOOTSTRAP_DOTNET:-$DOTNET}"
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 DOTNET_NOLOGO=1
cd "$REPO_ROOT"
BOOTSTRAP=".artifacts/test-runs/bootstrap-$(date -u +%Y%m%dT%H%M%SZ)-$$-$RANDOM"
mkdir -p "$BOOTSTRAP/out"
if command -v cygpath >/dev/null 2>&1; then
	INTERMEDIATE="$(cygpath -w "$REPO_ROOT/$BOOTSTRAP/obj")\\"
else
	INTERMEDIATE="$REPO_ROOT/$BOOTSTRAP/obj/"
fi
"$BOOTSTRAP_DOTNET" build "$SCRIPT_DIR/TestReporting/TestReporting.csproj" -c Release -m:1 -p:RestoreBuildInParallel=false -p:NuGetAudit=false -o "$BOOTSTRAP/out" "-p:BaseIntermediateOutputPath=$INTERMEDIATE" "-p:MSBuildProjectExtensionsPath=$INTERMEDIATE" >"$BOOTSTRAP/bootstrap.log" 2>&1 &
BOOTSTRAP_PID=$!
while kill -0 "$BOOTSTRAP_PID" 2>/dev/null; do
	if [[ -n "$TIMEOUT_SECONDS" ]] && (( $(date +%s) - STARTED >= TIMEOUT_SECONDS )); then
		kill -TERM "$BOOTSTRAP_PID" 2>/dev/null || true
		for ((grace=0; grace<5; grace++)); do kill -0 "$BOOTSTRAP_PID" 2>/dev/null || break; sleep 1; done
		kill -KILL "$BOOTSTRAP_PID" 2>/dev/null || true
		wait "$BOOTSTRAP_PID" 2>/dev/null || true
		bootstrap_failure TIMEOUT 'reporter bootstrap exceeded deadline; see bootstrap log' 3
	fi
	sleep 0.2
done
if ! wait "$BOOTSTRAP_PID"; then bootstrap_failure REPORTING_ERROR 'reporter bootstrap failed; see bootstrap log' 3; fi
if [[ -n "$TIMEOUT_SECONDS" ]]; then
	REMAINING=$((TIMEOUT_SECONDS - ($(date +%s) - STARTED)))
	if (( REMAINING <= 0 )); then bootstrap_failure TIMEOUT 'reporter bootstrap exhausted deadline; see bootstrap log' 3; fi
	ARGS+=(--timeout-seconds "$REMAINING")
fi
if command -v cygpath >/dev/null 2>&1; then
	export FUTUREMUD_REPORTER_BOOTSTRAP_LOG="$(cygpath -w "$REPO_ROOT/$BOOTSTRAP/bootstrap.log")"
else
	export FUTUREMUD_REPORTER_BOOTSTRAP_LOG="$REPO_ROOT/$BOOTSTRAP/bootstrap.log"
fi
exec "$DOTNET" "$BOOTSTRAP/out/TestReporting.dll" --repo "$REPO_ROOT" --suite "$SUITE" "${ARGS[@]}"
