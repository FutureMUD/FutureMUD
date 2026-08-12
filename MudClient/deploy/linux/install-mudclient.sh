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
if [[ -n "$domain" ]]; then
	[[ "$domain" =~ ^[A-Za-z0-9]([A-Za-z0-9-]{0,61}[A-Za-z0-9])?(\.[A-Za-z0-9]([A-Za-z0-9-]{0,61}[A-Za-z0-9])?)+$ ]] || { echo 'The public domain must be a DNS name such as play.example.com.' >&2; exit 2; }
	[[ "$mud_host" =~ ^[A-Za-z0-9][A-Za-z0-9.-]*$ && "$mud_host" != *..* ]] || { echo 'The MUD host must be an IPv4 address or DNS host name.' >&2; exit 2; }
	[[ "$mud_port" =~ ^[0-9]+$ ]] && (( mud_port >= 1 && mud_port <= 65535 )) || { echo 'The MUD port must be between 1 and 65535.' >&2; exit 2; }
fi
chmod +x "$package_root/tools/MudClientDeployment"
manifest="$(mktemp)"; signature="$(mktemp)"
trap 'rm -f "$manifest" "$signature"' EXIT
curl --fail --silent --show-error https://futuremud.com/downloads/mudclient/latest/update-manifest.json -o "$manifest"
curl --fail --silent --show-error https://futuremud.com/downloads/mudclient/latest/update-manifest.sig -o "$signature"
"$package_root/tools/MudClientDeployment" verify --manifest "$manifest" --signature "$signature" --archive "$archive" --runtime "$runtime" --expected-version "$version"

had_proxy_settings=false
[[ ! -f "$config_root/proxy/appsettings.json" ]] || had_proxy_settings=true
if [[ -e "$install_root/proxy" && ! -L "$install_root/proxy" ]]; then
	[[ "$migrate" == true ]] || { echo 'A flat MudClient installation was found. Re-run with --migrate.' >&2; exit 1; }
	legacy="$install_root/releases/legacy-$(date -u +%Y%m%d%H%M%S)"
	migration_legacy_target="${legacy#"$install_root/"}"
	mkdir -p "$legacy"
	mv "$install_root/proxy" "$legacy/proxy"
	mv "$install_root/web" "$legacy/web"
	mkdir -p "$config_root/proxy" "$config_root/web"
	[[ -f "$config_root/proxy/appsettings.json" ]] || cp -p "$legacy/proxy/appsettings.json" "$config_root/proxy/appsettings.json"
	[[ ! -f "$legacy/web/wwwroot/appsettings.json" || -f "$config_root/web/appsettings.json" ]] || cp -p "$legacy/web/wwwroot/appsettings.json" "$config_root/web/appsettings.json"
	had_proxy_settings=true
fi

release="$install_root/releases/$version"
[[ ! -e "$release" ]] || { echo "Release $version is already staged." >&2; exit 1; }
mkdir -p "$install_root/releases" "$config_root/proxy" "$config_root/web"
cp -a "$package_root" "$release"
[[ -f "$config_root/proxy/appsettings.json" ]] || cp -p "$release/proxy/appsettings.json" "$config_root/proxy/appsettings.json"
[[ -f "$config_root/web/appsettings.json" ]] || cp -p "$release/web/wwwroot/appsettings.json" "$config_root/web/appsettings.json"
if [[ -n "$domain" && "$had_proxy_settings" == false ]]; then
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
# Release scripts and verification tools are invoked by root during upgrades, so the
# unprivileged proxy account must never be able to replace them.
chown root:root "$install_root"
chown -R root:root "$install_root/releases"
chmod -R go-w "$install_root/releases"
chown -R mudclient:mudclient "$config_root/proxy"
install -m 0644 "$release/deploy/linux/mudclient-proxy.service" /etc/systemd/system/mudclient-proxy.service

if [[ -n "$domain" ]]; then
	caddy_config="${CADDY_CONFIG:-/etc/caddy/Caddyfile}"
	caddy_fragments="${CADDY_FRAGMENTS_DIR:-/etc/caddy/Caddyfile.d}"
	fragment_path="$caddy_fragments/mudclient.caddy"
	command -v caddy >/dev/null || { echo 'Caddy is required for the standard first install.' >&2; exit 1; }
	[[ -f "$caddy_config" ]] || { echo "Caddy configuration '$caddy_config' was not found." >&2; exit 1; }
	mkdir -p "$caddy_fragments"
	fragment_created=false
	import_added=false
	if [[ ! -f "$fragment_path" ]]; then
		fragment_created=true
		cat >"$fragment_path" <<EOF
$domain {
	root * $install_root/web/wwwroot
	encode gzip zstd
	header {
		Content-Security-Policy "default-src 'self'; script-src 'self' 'wasm-unsafe-eval'; style-src 'self' 'unsafe-inline'; connect-src 'self' wss:; img-src 'self' https: data:; font-src 'self'; object-src 'none'; base-uri 'self'; frame-ancestors 'none'; form-action 'none'"
		Permissions-Policy "accelerometer=(), autoplay=(), camera=(), display-capture=(), geolocation=(), gyroscope=(), microphone=(), payment=(), picture-in-picture=(), usb=()"
		Referrer-Policy "strict-origin-when-cross-origin"
		Strict-Transport-Security "max-age=31536000; includeSubDomains"
		X-Content-Type-Options "nosniff"
		X-Frame-Options "DENY"
	}
	handle /ws { reverse_proxy 127.0.0.1:5000 }
	handle { try_files {path} {path}/ /index.html; file_server }
}
EOF
	fi
	import_line="import $caddy_fragments/*"
	if ! grep -Fqx "$import_line" "$caddy_config"; then
		caddy_config_backup="$caddy_config.before-mudclient-install"
		cp -p "$caddy_config" "$caddy_config_backup"
		printf '\n%s\n' "$import_line" >>"$caddy_config"
		import_added=true
	fi
	if ! caddy validate --config "$caddy_config" --adapter caddyfile || ! (systemctl reload caddy || systemctl enable --now caddy); then
		[[ "$import_added" == false ]] || cp -p "$caddy_config_backup" "$caddy_config"
		[[ "$fragment_created" == false ]] || rm -f "$fragment_path"
		if [[ "$import_added" == true ]]; then
			systemctl reload caddy || true
		fi
		echo 'Caddy validation or reload failed; the prior Caddy configuration has been restored.' >&2
		exit 1
	fi
fi

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
mapfile -t releases < <(find "$install_root/releases" -mindepth 1 -maxdepth 1 -type d -printf '%f\n' | grep -Ev '^legacy-' | sort -V)
while (( ${#releases[@]} > 3 )); do
	rm -rf -- "$install_root/releases/${releases[0]}"
	releases=("${releases[@]:1}")
done
echo "MudClient $version is active. Future upgrades: sudo $install_root/current/deploy/linux/update-mudclient.sh"
