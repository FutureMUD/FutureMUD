#!/usr/bin/env bash
set -euo pipefail

usage() {
	cat <<'EOF'
Usage:
  sudo ./deploy/linux/install-mudclient.sh --archive /path/to/mudclient-X.Y.Z-linux-*.zip [--migrate]
  sudo ./deploy/linux/install-mudclient.sh --archive /path/to/mudclient-X.Y.Z-linux-*.zip --domain play.example.com [--mud-host 127.0.0.1] [--mud-port 4000]

Run this from an extracted MudClient package. The archive is verified against the signed
FutureMUD update manifest before any installed file is changed. --migrate preserves a flat
1.0.1/1.1.0 install and converts it to the versioned release layout.
EOF
}

[[ ${EUID} -eq 0 ]] || { echo 'Run as root.' >&2; exit 1; }

archive=''
domain=''
mud_host='127.0.0.1'
mud_port='4000'
migrate=false
install_root="${MUDCLIENT_INSTALL_ROOT:-/opt/mudclient}"
config_root="${MUDCLIENT_CONFIG_ROOT:-/etc/mudclient}"
migration_legacy_target=''
while [[ $# -gt 0 ]]; do
	case "$1" in
		--archive) archive="${2:?--archive requires a value}"; shift 2 ;;
		--domain) domain="${2:?--domain requires a value}"; shift 2 ;;
		--mud-host) mud_host="${2:?--mud-host requires a value}"; shift 2 ;;
		--mud-port) mud_port="${2:?--mud-port requires a value}"; shift 2 ;;
		--migrate) migrate=true; shift ;;
		--help|-h) usage; exit 0 ;;
		*) usage >&2; exit 2 ;;
	esac
done

[[ -n "$archive" && -f "$archive" ]] || { echo '--archive must name the downloaded release ZIP.' >&2; exit 2; }
package_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
package_name="$(basename "$package_root")"
runtime="${package_name##*-}"
runtime="${runtime#linux-}"
runtime="linux-$runtime"
version="$(sed -nE 's/^mudclient-([0-9]+\.[0-9]+\.[0-9]+)-linux-.*/\1/p' <<<"$package_name")"
[[ -n "$version" ]] || { echo 'The package folder does not have the expected versioned name.' >&2; exit 2; }
chmod +x "$package_root/tools/MudClientDeployment"
manifest="$(mktemp)"; signature="$(mktemp)"
trap 'rm -f "$manifest" "$signature"' EXIT
curl --fail --silent --show-error https://futuremud.com/downloads/mudclient/latest/update-manifest.json -o "$manifest"
curl --fail --silent --show-error https://futuremud.com/downloads/mudclient/latest/update-manifest.sig -o "$signature"
"$package_root/tools/MudClientDeployment" verify --manifest "$manifest" --signature "$signature" --archive "$archive" --runtime "$runtime"

if [[ -e "$install_root/proxy" && ! -L "$install_root/proxy" ]]; then
	[[ "$migrate" == true ]] || { echo 'A flat MudClient installation was found. Re-run with --migrate.' >&2; exit 1; }
	legacy="$install_root/releases/legacy-$(date -u +%Y%m%d%H%M%S)"
	migration_legacy_target="${legacy#"$install_root/"}"
	mkdir -p "$legacy"
	mv "$install_root/proxy" "$legacy/proxy"
	mv "$install_root/web" "$legacy/web"
	mkdir -p "$config_root/proxy" "$config_root/web"
	cp -p "$legacy/proxy/appsettings.json" "$config_root/proxy/appsettings.json"
	[[ ! -f "$legacy/web/wwwroot/appsettings.json" ]] || cp -p "$legacy/web/wwwroot/appsettings.json" "$config_root/web/appsettings.json"
fi

release="$install_root/releases/$version"
[[ ! -e "$release" ]] || { echo "Release $version is already staged." >&2; exit 1; }
mkdir -p "$install_root/releases" "$config_root/proxy" "$config_root/web"
cp -a "$package_root" "$release"
[[ -f "$config_root/proxy/appsettings.json" ]] || cp -p "$release/proxy/appsettings.json" "$config_root/proxy/appsettings.json"
[[ -f "$config_root/web/appsettings.json" ]] || cp -p "$release/web/wwwroot/appsettings.json" "$config_root/web/appsettings.json"
if [[ -n "$domain" ]]; then
	cat >"$config_root/proxy/appsettings.json" <<EOF
{
  "Logging": { "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" } },
  "AllowedHosts": "*",
  "MudServer": { "Address": "$mud_host", "Port": $mud_port },
  "WebSocketServer": { "Path": "/ws", "RequireOrigin": true, "AllowedOrigins": [ "https://$domain" ] },
  "ProxyLimits": { "MaximumConcurrentConnections": 200, "MaximumConnectionsPerIp": 20, "MaximumClientMessageBytes": 65536, "MaximumClientMessagesPerSecond": 30, "MaximumClientBytesPerSecond": 131072, "MaximumMudBytesPerSecond": 2097152, "MudConnectionTimeoutSeconds": 10 }
}
EOF
fi
cp -p "$config_root/web/appsettings.json" "$release/web/wwwroot/appsettings.json"
chmod +x "$release/proxy/MudWebSocketProxy" "$release/tools/MudClientDeployment"

if ! id -u mudclient >/dev/null 2>&1; then
	useradd --system --home "$install_root" --shell /usr/sbin/nologin mudclient
fi
chown -R mudclient:mudclient "$install_root/releases" "$config_root/proxy"
install -m 0644 "$release/deploy/linux/mudclient-proxy.service" /etc/systemd/system/mudclient-proxy.service
previous_target=''
[[ ! -L "$install_root/current" ]] || previous_target="$(readlink "$install_root/current")"
[[ -n "$previous_target" || -z "$migration_legacy_target" ]] || previous_target="$migration_legacy_target"
ln -s "releases/$version" "$install_root/current.new"
mv -Tf "$install_root/current.new" "$install_root/current"
[[ -L "$install_root/web" ]] || ln -s current/web "$install_root/web"
[[ -L "$install_root/proxy" ]] || ln -s current/proxy "$install_root/proxy"
if ! systemctl daemon-reload || ! systemctl enable --now mudclient-proxy || ! curl --fail --silent --show-error http://127.0.0.1:5000/health >/dev/null; then
	if [[ -n "$previous_target" ]]; then
		ln -s "$previous_target" "$install_root/current.rollback"
		mv -Tf "$install_root/current.rollback" "$install_root/current"
		systemctl restart mudclient-proxy || true
	fi
	echo 'Activation failed; the prior release has been restored.' >&2
	exit 1
fi
if [[ -n "$domain" ]]; then
	caddy_config="${CADDY_CONFIG:-/etc/caddy/Caddyfile}"
	caddy_fragments="${CADDY_FRAGMENTS_DIR:-/etc/caddy/Caddyfile.d}"
	command -v caddy >/dev/null || { echo 'Caddy is required for the standard first install.' >&2; exit 1; }
	[[ -f "$caddy_config" ]] || { echo "Caddy configuration '$caddy_config' was not found." >&2; exit 1; }
	mkdir -p "$caddy_fragments"
	cat >"$caddy_fragments/mudclient.caddy" <<EOF
$domain {
	root * $install_root/web/wwwroot
	encode gzip zstd
	handle /ws { reverse_proxy 127.0.0.1:5000 }
	handle { try_files {path} {path}/ /index.html; file_server }
}
EOF
	import_line="import $caddy_fragments/*"
	grep -Fqx "$import_line" "$caddy_config" || printf '\n%s\n' "$import_line" >>"$caddy_config"
	caddy validate --config "$caddy_config" --adapter caddyfile
	systemctl reload caddy || systemctl enable --now caddy
fi
mapfile -t releases < <(find "$install_root/releases" -mindepth 1 -maxdepth 1 -type d -printf '%f\n' | grep -Ev '^legacy-' | sort -V)
while (( ${#releases[@]} > 3 )); do
	rm -rf -- "$install_root/releases/${releases[0]}"
	releases=("${releases[@]:1}")
done
echo "MudClient $version is active. Future upgrades: sudo $install_root/current/deploy/linux/update-mudclient.sh"
