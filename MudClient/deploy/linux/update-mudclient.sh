#!/usr/bin/env bash
set -euo pipefail

install_root="${MUDCLIENT_INSTALL_ROOT:-/opt/mudclient}"
runtime="${MUDCLIENT_RUNTIME:-linux-x64}"
case "${1:-}" in
	--check)
		curl --fail --silent --show-error https://futuremud.com/downloads/mudclient/latest/update-manifest.json
		exit 0 ;;
	--rollback)
		previous="$(find "$install_root/releases" -mindepth 1 -maxdepth 1 -type d -printf '%f\n' | sort -V | tail -n 2 | head -n 1)"
		[[ -n "$previous" ]] || { echo 'No previous release is available.' >&2; exit 1; }
		systemctl stop mudclient-proxy
		ln -s "releases/$previous" "$install_root/current.rollback"
		mv -Tf "$install_root/current.rollback" "$install_root/current"
		systemctl start mudclient-proxy
		curl --fail --silent --show-error http://127.0.0.1:5000/health >/dev/null
		exit 0 ;;
	'') ;;
	*) echo 'Usage: update-mudclient.sh [--check|--rollback]' >&2; exit 2 ;;
esac

tmp="$(mktemp -d)"; trap 'rm -rf "$tmp"' EXIT
manifest="$tmp/manifest.json"; signature="$tmp/manifest.sig"
curl --fail --silent --show-error https://futuremud.com/downloads/mudclient/latest/update-manifest.json -o "$manifest"
curl --fail --silent --show-error https://futuremud.com/downloads/mudclient/latest/update-manifest.sig -o "$signature"
version="$(sed -nE 's/.*"version"[[:space:]]*:[[:space:]]*"([0-9.]+)".*/\1/p' "$manifest" | head -n 1)"
archive="$tmp/mudclient-$version-$runtime.zip"
curl --fail --silent --show-error "https://futuremud.com/downloads/mudclient/latest/$runtime" -o "$archive"
"$install_root/current/tools/MudClientDeployment" verify --manifest "$manifest" --signature "$signature" --archive "$archive" --runtime "$runtime"
unzip -q "$archive" -d "$tmp/package"
package="$(find "$tmp/package" -mindepth 1 -maxdepth 1 -type d | head -n 1)"
systemctl stop mudclient-proxy
if ! bash "$package/deploy/linux/install-mudclient.sh" --archive "$archive"; then
	systemctl start mudclient-proxy || true
	exit 1
fi
