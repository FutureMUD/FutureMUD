#!/usr/bin/env bash
set -euo pipefail

install_root="${MUDCLIENT_INSTALL_ROOT:-/opt/mudclient}"
if [[ -n "${MUDCLIENT_RUNTIME:-}" ]]; then
	runtime="$MUDCLIENT_RUNTIME"
else
	case "$(uname -m)" in
		aarch64|arm64) runtime='linux-arm64' ;;
		x86_64|amd64) runtime='linux-x64' ;;
		*) echo "Unsupported Linux architecture '$(uname -m)'. Set MUDCLIENT_RUNTIME explicitly if it is supported." >&2; exit 1 ;;
	esac
fi
deployment_tool="$install_root/current/tools/MudClientDeployment"
[[ -x "$deployment_tool" ]] || { echo 'The deployed MudClient update verifier is unavailable.' >&2; exit 1; }

download_manifest() {
	manifest="$1"
	signature="$2"
	curl --fail --silent --show-error https://futuremud.com/downloads/mudclient/latest/update-manifest.json -o "$manifest"
	curl --fail --silent --show-error https://futuremud.com/downloads/mudclient/latest/update-manifest.sig -o "$signature"
	"$deployment_tool" verify-manifest --manifest "$manifest" --signature "$signature" --runtime "$runtime"
}

rollback() {
	local current_target current_name previous_name
	current_target="$(readlink "$install_root/current")"
	current_name="${current_target##*/}"
	mapfile -t releases < <(find "$install_root/releases" -mindepth 1 -maxdepth 1 -type d -printf '%f\n' | grep -Ev '^legacy-' | sort -V)
	for (( index=0; index<${#releases[@]}; index++ )); do
		if [[ "${releases[index]}" == "$current_name" ]]; then
			(( index > 0 )) || { echo 'No prior release is available.' >&2; return 1; }
			previous_name="${releases[index - 1]}"
			break
		fi
	done
	[[ -n "${previous_name:-}" ]] || { echo 'The active release is not a retained numbered release.' >&2; return 1; }
	systemctl stop mudclient-proxy
	ln -s "releases/$previous_name" "$install_root/current.rollback"
	mv -Tf "$install_root/current.rollback" "$install_root/current"
	if ! systemctl start mudclient-proxy || ! curl --fail --silent --show-error http://127.0.0.1:5000/health >/dev/null; then
		ln -s "$current_target" "$install_root/current.rollback"
		mv -Tf "$install_root/current.rollback" "$install_root/current"
		systemctl restart mudclient-proxy || true
		echo 'Rollback health check failed; the original release has been restored.' >&2
		return 1
	fi
	echo "MudClient rolled back to $previous_name."
}

case "${1:-}" in
	--check)
		tmp="$(mktemp -d)"; trap 'rm -rf "$tmp"' EXIT
		download_manifest "$tmp/manifest.json" "$tmp/manifest.sig"
		exit 0 ;;
	--rollback)
		[[ ${EUID} -eq 0 ]] || { echo 'Run rollback as root.' >&2; exit 1; }
		rollback
		exit $? ;;
	'') ;;
	*) echo 'Usage: update-mudclient.sh [--check|--rollback]' >&2; exit 2 ;;
esac

[[ ${EUID} -eq 0 ]] || { echo 'Run updates as root.' >&2; exit 1; }
tmp="$(mktemp -d)"; trap 'rm -rf "$tmp"' EXIT
manifest="$tmp/manifest.json"; signature="$tmp/manifest.sig"
download_manifest "$manifest" "$signature"
version="$(sed -nE 's/.*"version"[[:space:]]*:[[:space:]]*"([0-9.]+)".*/\1/p' "$manifest" | head -n 1)"
[[ -n "$version" ]] || { echo 'The signed update manifest did not contain a version.' >&2; exit 1; }
if [[ -L "$install_root/current" ]]; then
	current_version="$(basename "$(readlink "$install_root/current")")"
	if [[ "$version" == "$current_version" ]]; then
		echo "MudClient $version is already active."
		exit 0
	fi
	if [[ "$(printf '%s\n%s\n' "$version" "$current_version" | sort -V | tail -n 1)" != "$version" ]]; then
		echo "Refusing unsupported downgrade from $current_version to $version." >&2
		exit 1
	fi
fi
archive="$tmp/mudclient-$version-$runtime.zip"
curl --fail --silent --show-error "https://futuremud.com/downloads/mudclient/latest/$runtime" -o "$archive"
"$deployment_tool" verify --manifest "$manifest" --signature "$signature" --archive "$archive" --runtime "$runtime" --expected-version "$version"
unzip -q "$archive" -d "$tmp/package"
package="$tmp/package/mudclient-$version-$runtime"
[[ -d "$package" ]] || { echo 'The verified archive has an invalid package layout.' >&2; exit 1; }
systemctl stop mudclient-proxy
if ! bash "$package/deploy/linux/install-mudclient.sh" --archive "$archive"; then
	systemctl start mudclient-proxy || true
	exit 1
fi
