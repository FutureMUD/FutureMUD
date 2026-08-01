#!/usr/bin/env bash
set -euo pipefail

usage() {
	cat <<'EOF'
Usage: sudo ./deploy/linux/install-mudclient.sh play.example.com [mud-host] [mud-port]

Run this script from an extracted Linux Web MUD Client release package. It creates
the private proxy service, writes its trusted public origin, and installs an
isolated Caddy site fragment. Caddy must already be installed and managed by
systemd. The target domain must resolve to this server before Caddy can issue TLS.
EOF
}

if [[ "${1:-}" == "--help" || "${1:-}" == "-h" ]]; then
	usage
	exit 0
fi

if [[ $# -lt 1 || $# -gt 3 ]]; then
	usage >&2
	exit 2
fi

if [[ $EUID -ne 0 ]]; then
	echo "Run this installer as root (for example, with sudo)." >&2
	exit 1
fi

domain="$1"
mud_host="${2:-127.0.0.1}"
mud_port="${3:-4000}"
install_root="${MUDCLIENT_INSTALL_ROOT:-/opt/mudclient}"
caddy_config="${CADDY_CONFIG:-/etc/caddy/Caddyfile}"
caddy_fragments="${CADDY_FRAGMENTS_DIR:-/etc/caddy/Caddyfile.d}"
service_name="mudclient-proxy"
service_user="mudclient"

if [[ ! "$domain" =~ ^[A-Za-z0-9]([A-Za-z0-9-]{0,61}[A-Za-z0-9])?(\.[A-Za-z0-9]([A-Za-z0-9-]{0,61}[A-Za-z0-9])?)+$ ]]; then
	echo "The public domain must be a DNS name such as play.example.com." >&2
	exit 2
fi
if [[ ! "$mud_host" =~ ^[A-Za-z0-9][A-Za-z0-9.-]*$ ]] || [[ "$mud_host" == *..* ]]; then
	echo "The MUD host must be an IPv4 address or DNS host name." >&2
	exit 2
fi
if [[ ! "$mud_port" =~ ^[0-9]+$ ]] || (( mud_port < 1 || mud_port > 65535 )); then
	echo "The MUD port must be between 1 and 65535." >&2
	exit 2
fi
if ! command -v caddy >/dev/null 2>&1; then
	echo "Caddy is required but was not found. Install Caddy v2, then rerun this installer." >&2
	exit 1
fi
if [[ ! -f "$caddy_config" ]]; then
	echo "Caddy configuration '$caddy_config' was not found. Set CADDY_CONFIG to its path and rerun." >&2
	exit 1
fi

package_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
if [[ "$package_root" != "$install_root" ]]; then
	echo "Copy the extracted package to $install_root first, then run its installer from there." >&2
	exit 1
fi
if [[ ! -x "$install_root/proxy/MudWebSocketProxy" ]]; then
	chmod +x "$install_root/proxy/MudWebSocketProxy"
fi

if ! id -u "$service_user" >/dev/null 2>&1; then
	useradd --system --home "$install_root" --shell /usr/sbin/nologin "$service_user"
fi
chown -R "$service_user:$service_user" "$install_root"

config_path="$install_root/proxy/appsettings.json"
if [[ -f "$config_path" ]]; then
	cp -p "$config_path" "$config_path.before-mudclient-install"
fi
cat >"$config_path" <<EOF
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "MudServer": {
    "Address": "$mud_host",
    "Port": "$mud_port"
  },
  "WebSocketServer": {
    "Path": "/ws",
    "AllowedOrigins": [
      "https://$domain"
    ]
  }
}
EOF
chown "$service_user:$service_user" "$config_path"

install -m 0644 "$install_root/deploy/linux/mudclient-proxy.service" "/etc/systemd/system/$service_name.service"
mkdir -p "$caddy_fragments"
fragment_path="$caddy_fragments/mudclient.caddy"
fragment_backup=""
if [[ -f "$fragment_path" ]]; then
	fragment_backup="$fragment_path.before-mudclient-install"
	cp -p "$fragment_path" "$fragment_backup"
fi
cat >"$fragment_path" <<EOF
$domain {
	root * $install_root/web/wwwroot
	encode gzip zstd
	handle /ws* {
		reverse_proxy 127.0.0.1:5000
	}
	try_files {path} {path}/ /index.html
	file_server
}
EOF

import_line="import $caddy_fragments/*"
import_added=false
if ! grep -Fqx "$import_line" "$caddy_config"; then
	cp -p "$caddy_config" "$caddy_config.before-mudclient-install"
	printf '\n%s\n' "$import_line" >>"$caddy_config"
	import_added=true
fi

if ! caddy validate --config "$caddy_config" --adapter caddyfile; then
	if [[ "$import_added" == true ]]; then
		cp -p "$caddy_config.before-mudclient-install" "$caddy_config"
	fi
	if [[ -n "$fragment_backup" ]]; then
		cp -p "$fragment_backup" "$fragment_path"
	else
		rm -f "$fragment_path"
	fi
	echo "Caddy validation failed. The installer restored the Caddy files it changed." >&2
	exit 1
fi

systemctl daemon-reload
systemctl enable --now "$service_name"
if systemctl is-active --quiet caddy; then
	systemctl reload caddy
else
	systemctl enable --now caddy
fi

curl --fail --silent --show-error http://127.0.0.1:5000/health >/dev/null
echo "Web MUD Client installed. Open https://$domain after DNS points at this server."
