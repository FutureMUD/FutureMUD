#!/usr/bin/env bash
set -euo pipefail

configuration="${CONFIGURATION:-Release}"
output_root="${OUTPUT_ROOT:-artifacts/release}"
runtime_identifier="${1:-${RUNTIME_IDENTIFIER:-}}"
skip_tests="${SKIP_TESTS:-0}"

detect_runtime_identifier() {
	local os
	local arch

	case "$(uname -s)" in
		Linux*) os="linux" ;;
		MINGW*|MSYS*|CYGWIN*) os="win" ;;
		*) echo "Automatic runtime detection only supports Windows and Linux. Pass a runtime identifier." >&2; exit 1 ;;
	esac

	case "$(uname -m)" in
		x86_64|amd64) arch="x64" ;;
		aarch64|arm64) arch="arm64" ;;
		*) echo "Unsupported architecture '$(uname -m)'. Pass a runtime identifier." >&2; exit 1 ;;
	esac

	printf "%s-%s" "$os" "$arch"
}

if [[ -z "$runtime_identifier" ]]; then
	runtime_identifier="$(detect_runtime_identifier)"
fi

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$repo_root"

package_name="mudclient-$runtime_identifier"
package_root="$output_root/$package_name"
web_publish="$output_root/_web"
proxy_publish="$output_root/_proxy-$runtime_identifier"
zip_path="$output_root/$package_name.zip"
tar_path="$output_root/$package_name.tar.gz"

rm -rf "$package_root" "$web_publish" "$proxy_publish"
mkdir -p "$output_root"

dotnet restore MudClientSolution.sln

if [[ "$skip_tests" != "1" ]]; then
	dotnet test MudClientSolution.sln -c "$configuration" --no-restore
fi

dotnet publish MudClientBlazor/MudClientBlazor.csproj -c "$configuration" --no-restore -o "$web_publish"
dotnet publish MudWebSocketProxy/MudWebSocketProxy.csproj -c "$configuration" -r "$runtime_identifier" --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=embedded -o "$proxy_publish"

mkdir -p "$package_root/web" "$package_root/proxy"
cp -R "$web_publish"/. "$package_root/web/"
cp -R "$proxy_publish"/. "$package_root/proxy/"
cp -R deploy "$package_root/deploy"
cp DEPLOYMENT.md "$package_root/DEPLOYMENT.md"
printf "Start with DEPLOYMENT.md. The Blazor static site is in web/wwwroot and the websocket proxy is in proxy/.\n" > "$package_root/README.txt"

rm -f "$zip_path" "$tar_path"
if command -v zip >/dev/null 2>&1; then
	(
		cd "$output_root"
		zip -qr "$package_name.zip" "$package_name"
	)
	echo "Release package created:"
	echo "  $package_root"
	echo "  $zip_path"
else
	tar -czf "$tar_path" -C "$output_root" "$package_name"
	echo "Release package created:"
	echo "  $package_root"
	echo "  $tar_path"
fi
