#!/usr/bin/env bash
set -euo pipefail
HOSTNAME_VALUE="${1:-}"
if [[ -z "$HOSTNAME_VALUE" ]]; then echo 'Usage: install-caddy-site.sh planner.example.com' >&2; exit 2; fi
for candidate in /etc/caddy/Caddyfile /usr/local/etc/caddy/Caddyfile; do
	if [[ -f "$candidate" ]]; then CADDYFILE="$candidate"; break; fi
done
if [[ -z "${CADDYFILE:-}" ]] || ! command -v caddy >/dev/null; then
	echo 'Caddy or its Caddyfile was not found. Install the FutureMUD web client prerequisite first.' >&2; exit 1
fi
SITE_DIR="$(dirname "$CADDYFILE")/sites"
SITE_FILE="$SITE_DIR/terrainplanner.caddy"
BACKUP="$CADDYFILE.terrainplanner.bak.$(date +%Y%m%d%H%M%S)"
SITE_BACKUP=''
cp -a "$CADDYFILE" "$BACKUP"
mkdir -p "$SITE_DIR"
if [[ -f "$SITE_FILE" ]]; then SITE_BACKUP="$SITE_FILE.bak.$(date +%Y%m%d%H%M%S)"; cp -a "$SITE_FILE" "$SITE_BACKUP"; fi
cp "$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)/Caddyfile.fragment" "$SITE_FILE"
sed -i "s/{\$TERRAIN_PLANNER_HOSTNAME}/$HOSTNAME_VALUE/" "$SITE_FILE"
if ! grep -Fq 'import sites/*.caddy' "$CADDYFILE"; then printf '\nimport sites/*.caddy\n' >> "$CADDYFILE"; fi
if ! caddy validate --config "$CADDYFILE"; then
	cp -a "$BACKUP" "$CADDYFILE"
	if [[ -n "$SITE_BACKUP" ]]; then cp -a "$SITE_BACKUP" "$SITE_FILE"; else rm -f "$SITE_FILE"; fi
	echo 'Caddy validation failed; configuration restored.' >&2; exit 1
fi
if ! systemctl reload caddy; then
	cp -a "$BACKUP" "$CADDYFILE"
	if [[ -n "$SITE_BACKUP" ]]; then cp -a "$SITE_BACKUP" "$SITE_FILE"; else rm -f "$SITE_FILE"; fi
	caddy validate --config "$CADDYFILE" && systemctl reload caddy
	echo 'Caddy reload failed; configuration restored.' >&2; exit 1
fi
echo "Caddy now proxies https://$HOSTNAME_VALUE to 127.0.0.1:5010. Backup: $BACKUP"
