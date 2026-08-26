#!/usr/bin/env bash
set -euo pipefail

if [[ $EUID -ne 0 ]]; then echo 'Run this installer as root.' >&2; exit 1; fi
PACKAGE_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
VERSION="$(tr -d '\r\n' < "$PACKAGE_ROOT/version.txt")"
HOSTNAME_VALUE="${1:-}"
if [[ -z "$HOSTNAME_VALUE" ]]; then echo 'Usage: install-terrainplanner.sh planner.example.com' >&2; exit 2; fi
INSTALL_ROOT=/opt/futuremud/terrainplanner
CONFIG_ROOT=/etc/futuremud/terrainplanner
DATA_ROOT=/var/lib/futuremud/terrainplanner
RELEASE_ROOT="$INSTALL_ROOT/releases/$VERSION"
PREVIOUS_TARGET="$(readlink -f "$INSTALL_ROOT/current" 2>/dev/null || true)"

id terrainplanner >/dev/null 2>&1 || useradd --system --home-dir "$DATA_ROOT" --shell /usr/sbin/nologin terrainplanner
install -d -o root -g terrainplanner -m 0750 "$INSTALL_ROOT/releases" "$CONFIG_ROOT"
install -d -o terrainplanner -g terrainplanner -m 0700 "$DATA_ROOT/keys"
if [[ ! -f "$CONFIG_ROOT/appsettings.Production.json" ]]; then
	cp "$PACKAGE_ROOT/deploy/appsettings.Production.template.json" "$CONFIG_ROOT/appsettings.Production.json"
	sed -i "s|REPLACE_WITH_DURABLE_KEY_PATH|$DATA_ROOT/keys|; s|REPLACE_WITH_PLANNER_HOSTNAME|$HOSTNAME_VALUE|" "$CONFIG_ROOT/appsettings.Production.json"
	chmod 0640 "$CONFIG_ROOT/appsettings.Production.json"
	echo "Edit $CONFIG_ROOT/appsettings.Production.json and replace REPLACE_WITH_SECRET, then rerun this installer." >&2
	exit 3
fi
if [[ -e "$RELEASE_ROOT" ]]; then echo "Release $VERSION is already installed." >&2; exit 1; fi
install -d -o root -g root -m 0755 "$RELEASE_ROOT"
cp -a "$PACKAGE_ROOT/app" "$PACKAGE_ROOT/tools" "$PACKAGE_ROOT/deploy" "$PACKAGE_ROOT/DEPLOYMENT.md" "$RELEASE_ROOT/"
chmod +x "$RELEASE_ROOT/app/TerrainPlanner" "$RELEASE_ROOT/tools/TerrainPlanner.Deployment"
ln -sfn "$CONFIG_ROOT/appsettings.Production.json" "$RELEASE_ROOT/app/appsettings.Production.json"
ln -sfn "$RELEASE_ROOT" "$INSTALL_ROOT/current"
install -m 0644 "$PACKAGE_ROOT/deploy/linux/terrainplanner.service" /etc/systemd/system/terrainplanner.service
systemctl daemon-reload
systemctl enable terrainplanner.service >/dev/null
systemctl restart terrainplanner.service

healthy=false
for _ in {1..30}; do
	if curl --fail --silent http://127.0.0.1:5010/health/ready >/dev/null; then healthy=true; break; fi
	sleep 1
done
if [[ "$healthy" != true ]]; then
	systemctl stop terrainplanner.service || true
	if [[ -n "$PREVIOUS_TARGET" ]]; then
		ln -sfn "$PREVIOUS_TARGET" "$INSTALL_ROOT/current"
		systemctl restart terrainplanner.service
	else
		rm -f "$INSTALL_ROOT/current"
	fi
	echo 'Health check failed; the previous release was restored.' >&2
	exit 1
fi

find "$INSTALL_ROOT/releases" -mindepth 1 -maxdepth 1 -type d -printf '%T@ %p\n' | sort -nr | tail -n +4 | cut -d' ' -f2- | xargs -r rm -rf --
bash "$PACKAGE_ROOT/deploy/linux/install-caddy-site.sh" "$HOSTNAME_VALUE"
echo "Terrain Planner $VERSION is healthy at https://$HOSTNAME_VALUE."
