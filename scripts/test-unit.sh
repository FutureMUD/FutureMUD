#!/bin/bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
LOCAL_DOTNET_ROOT="$REPO_ROOT/.dotnet-cli/.dotnet"
TEST_PROJECT_MANIFEST="$SCRIPT_DIR/unit-test-projects.txt"

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

TEST_PROJECTS=()
while IFS= read -r test_project || [ -n "$test_project" ]; do
	if [ -z "$test_project" ] || [[ "$test_project" =~ ^[[:space:]]*# ]]; then
		continue
	fi
	TEST_PROJECTS+=("$test_project")
done < "$TEST_PROJECT_MANIFEST"

# Build sequentially to avoid shared-output locks, then run the isolated test hosts in parallel.
for test_project in "${TEST_PROJECTS[@]}"; do
	"$DOTNET" build "$test_project" -c Debug --no-restore -m:1 "-p:NoWarn=NU1902%3BNU1510"
done

TEST_LOG_DIR="$(mktemp -d)"
cleanup_logs() {
	if [[ -n "${TEST_LOG_DIR:-}" && -d "$TEST_LOG_DIR" ]]; then
		rm -f "$TEST_LOG_DIR"/*.log
		rmdir "$TEST_LOG_DIR"
	fi
}
trap cleanup_logs EXIT

PIDS=()
LOGS=()
for index in "${!TEST_PROJECTS[@]}"; do
	log_path="$TEST_LOG_DIR/$index.log"
	"$DOTNET" test "${TEST_PROJECTS[$index]}" -c Debug --no-restore --no-build -m:1 "-p:NoWarn=NU1902%3BNU1510" >"$log_path" 2>&1 &
	PIDS+=("$!")
	LOGS+=("$log_path")
done

failure=0
for index in "${!PIDS[@]}"; do
	if ! wait "${PIDS[$index]}"; then
		failure=1
	fi
	while IFS= read -r output_line || [ -n "$output_line" ]; do
		printf '%s\n' "$output_line"
	done < "${LOGS[$index]}"
done

exit "$failure"