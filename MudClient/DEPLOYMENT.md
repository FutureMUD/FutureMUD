# FutureMUD Web MUD Client deployment

The client is published through the FutureMUD product release flow and downloaded from [futuremud.com/downloads](https://futuremud.com/downloads). Each MUD operator hosts it for their own game; futuremud.com publishes the release package but does not run the proxy for individual games.

If you are setting this up for the first time on a Windows game server hosted in AWS, start with the [plain-English Windows and AWS guide](Deployment/Windows_AWS_Web_Client_Guide.md). It explains the small amount of web-server terminology, uses safe placeholder values rather than a live game's infrastructure, and includes a verification and rollback checklist.

This project deploys as two pieces:

- `web/` is the Blazor WebAssembly client. Serve `web/wwwroot` from a normal HTTPS web address.
- `proxy/` is the ASP.NET Core websocket-to-telnet proxy. Run it on the same server, bound to `127.0.0.1`, and reverse-proxy `/ws` to it.

Recommended production shape:

```text
Browser -> https://play.example.com -> static Blazor files
Browser -> wss://play.example.com/ws -> local proxy on 127.0.0.1:5000 -> local MUD on 127.0.0.1:4000
```

Only ports `80` and `443` should be public. Keep the proxy port private to the server.

## Host requirements

- A running FutureMUD instance reachable from the host. The recommended configuration keeps both services on one machine and uses `127.0.0.1:4000`.
- A public DNS hostname pointing at the client host, plus inbound TCP ports 80 and 443 for HTTPS. Do not expose the Telnet listener or proxy port.
- Administrator/root access to configure the proxy service and reverse proxy, and a current WebSocket-capable browser for players.
- The appropriate `win-x64`, `linux-x64`, or `linux-arm64` release package.

The proxy is self-contained, so the deployed host does **not** need a separate .NET runtime.

Linux additionally requires systemd, bash, curl, unzip, and Caddy v2 managed by systemd. Windows requires an Administrator PowerShell session and Caddy v2. Caddy is deliberately an explicit prerequisite: it owns automatic HTTPS and must not be silently installed or overwrite an operator's existing web-server configuration.

## One-time migration and automated upgrades

MudClient 1.2.0 is the first release with a safe upgrade layout. Its release files are immutable under `releases/<version>`; `current`, `web`, and `proxy` stay at stable paths so an upgrade does not alter Caddy, DNS, TLS, firewall, MUD, or database configuration. Operator settings are moved to `/etc/mudclient` on Linux and `%ProgramData%\FutureMUD\MudClient` on Windows.

To migrate an existing 1.0.1 or 1.1.0 Linux installation, download and extract the 1.2.2 archive once, then run the installer with the original archive:

~~~bash
unzip mudclient-1.2.2-linux-x64.zip -d /tmp/mudclient-1.2.2
sudo bash /tmp/mudclient-1.2.2/mudclient-1.2.2-linux-x64/deploy/linux/install-mudclient.sh \
  --archive "$PWD/mudclient-1.2.2-linux-x64.zip" --migrate
~~~

The command keeps the old release as a rollback target, copies proxy and browser settings to durable locations, and retains the existing Caddy configuration. It briefly restarts only the private proxy, so connected browser sessions disconnect and can reconnect; the MUD itself is not restarted.

For every later update, run one command as root:

~~~bash
sudo /opt/mudclient/current/deploy/linux/update-mudclient.sh
~~~

Use `--check` to inspect the signed latest manifest without changing files, or `--rollback` to activate the previous local release. The updater verifies an Ed25519-signed manifest and archive SHA-256 before staging, preserves two prior releases, and restores the prior release if the proxy health check fails.

On Windows, extract the 1.2.2 archive once and run this from an elevated PowerShell window:

~~~powershell
& 'C:\staging\mudclient-1.2.2-win-x64\deploy\windows\Install-MudClient.ps1' `
  -ArchivePath 'C:\Users\Administrator\Downloads\mudclient-1.2.2-win-x64.zip' -Migrate
~~~

Later Windows updates use:

~~~powershell
& 'C:\MudClient\current\deploy\windows\Update-MudClient.ps1'
~~~

`-Check` and `-Rollback` are the equivalent diagnostic and local rollback actions. The migration replaces the old proxy scheduled task with the `MudClientProxy` Windows Service but leaves the Caddy task and Caddyfile untouched.

## Linux automated first install

After verifying the website's published SHA-256 checksum, extract the downloaded package to `/opt/mudclient` and run the installer from that directory:

~~~bash
unzip mudclient-1.2.2-linux-x64.zip -d /tmp/mudclient-package
sudo bash /tmp/mudclient-package/mudclient-1.2.2-linux-x64/deploy/linux/install-mudclient.sh \
  --archive "$PWD/mudclient-1.2.2-linux-x64.zip" --domain play.example.com
~~~

The script creates the unprivileged proxy service, writes the exact trusted public origin, adds an isolated Caddy site fragment, validates Caddy before reload, and checks the private health endpoint. The selected domain must already resolve to the host. To connect to a MUD on a private network rather than the same machine:

~~~bash
sudo bash /tmp/mudclient-package/mudclient-1.2.2-linux-x64/deploy/linux/install-mudclient.sh \
  --archive "$PWD/mudclient-1.2.2-linux-x64.zip" --domain play.example.com --mud-host 10.0.0.20 --mud-port 4000
~~~

Use `CADDY_CONFIG` and `CADDY_FRAGMENTS_DIR` for nonstandard Caddyfile locations. The installer saves `.before-mudclient-install` backups of the files it changes; review those and take a normal deployment backup before upgrading an existing installation.

## Build A Release

The production release is created by an annotated `mudclient-v<version>` tag in the FutureMUD repository. The normal product workflow installs the WebAssembly build tools, fails closed if NuGet advisory data is unavailable or any direct/transitive dependency is vulnerable, runs the client tests, produces self-contained Windows x64, Linux x64, and Linux ARM64 packages, smoke-publishes them to a temporary website host, and promotes the validated archives to futuremud.com.

You can also build a local package from a machine with the .NET 10 SDK. For optimized Blazor WebAssembly output, install the WebAssembly build tools once before publishing:

```powershell
dotnet workload install wasm-tools
```

```powershell
.\scripts\Publish-ProductPackage.ps1 -RuntimeIdentifier win-x64 -Version 1.2.2
```

```bash
bash scripts/publish-release.sh linux-x64
```

## Configure The Package

Unzip the release package on the server. The examples below use `/opt/mudclient` on Linux and `C:\MudClient` on Windows.

During a first install, edit the durable proxy configuration at `/etc/mudclient/proxy/appsettings.json` on Linux or `%ProgramData%\FutureMUD\MudClient\proxy\appsettings.json` on Windows:

```json
{
  "MudServer": {
    "Address": "127.0.0.1",
    "Port": "4000"
  },
  "WebSocketServer": {
    "Path": "/ws",
	"RequireOrigin": true,
    "AllowedOrigins": [
      "https://play.example.com"
    ]
	},
	"ProxyLimits": {
	  "MaximumConcurrentConnections": 200,
	  "MaximumConnectionsPerIp": 20,
	  "MaximumClientMessageBytes": 65536,
	  "MaximumClientMessagesPerSecond": 30,
	  "MaximumClientBytesPerSecond": 131072,
	  "MaximumMudBytesPerSecond": 2097152,
	  "MudConnectionTimeoutSeconds": 10
  }
}
```

Set `MudServer` to the telnet address and port as seen from the web server. For the intended same-server install, keep `127.0.0.1`.

Set `AllowedOrigins` to the exact public web origin. Use `http://...` only for local testing; production should be `https://...`.

Keep `RequireOrigin` enabled in production. It rejects originless WebSocket upgrades as well as browser origins not listed in `AllowedOrigins`. The proxy also applies global and per-address connection ceilings, a MUD connect timeout, a maximum client message size, per-connection client message/byte rates, and a generous MUD-to-browser output ceiling. The packaged defaults accommodate normal play and the client's paced multi-line sender; raise them only after reviewing the public-edge capacity and abuse implications.

The Blazor client defaults to:

```json
{
  "WebSocketServer": {
    "Endpoint": "/ws"
  }
}
```

That is the easiest production setup because the browser automatically turns it into `wss://your-site/ws` when the page is served over HTTPS.

The supplied Caddy and Nginx examples restrict framing and browser capabilities, set a no-referrer policy, prevent content-type sniffing, and use a Content Security Policy compatible with Blazor WebAssembly and FutureMUD ANSI/MXP output. Preserve those headers when integrating the client into an existing site. The proxy should remain bound to loopback so only the trusted local reverse proxy can supply forwarded client addresses.

Quick Login usernames and aliases are browser-local preferences. Passwords are deliberately held only in the current tab's memory and are removed from older saved settings when loaded. Let the browser or a password manager retain credentials instead of weakening this boundary.

The client can load a UTF-8 text file of up to 1 MiB into the command box for review. It never sends a selected file automatically. Newline stacking then sends at the configured pace (100 ms by default), while Quick Login retains a separate 750 ms prompt-transition delay. The client and proxy both cap batch/message sizes and rates; these are safety controls, not substitutes for normal FutureMUD account and command permissions.

## Linux Service

The installer creates the unprivileged `mudclient` account and `mudclient-proxy.service`. It launches the stable `proxy` path with `--settings /etc/mudclient/proxy/appsettings.json`; never copy a package over `/opt/mudclient` or hand-edit that generated service during a normal update. The service listens on `http://127.0.0.1:5000`.

## Web Server

Caddy is the simplest cross-platform option. Copy `deploy/Caddyfile`, replace the domain and install path, then run or reload Caddy.

Linux administrators who prefer Nginx can copy `deploy/linux/nginx-mudclient.conf`, replace the domain and install path, enable the site, and add TLS with their normal certificate tooling.

After the web server is running, check:

```bash
curl http://127.0.0.1:5000/health
```

Then open the public web address and connect from the client.

## Windows Service

The installer creates the native `MudClientProxy` service and passes `%ProgramData%\FutureMUD\MudClient\proxy\appsettings.json` as its external settings file. Do not create a scheduled task for the proxy. If the service needs a manual restart after an operator configuration change, use `Restart-Service MudClientProxy` from an elevated PowerShell prompt.

Use the persistent `C:\MudClient\deploy\windows\Caddyfile` as the matching static-file and websocket reverse-proxy example for Caddy on Windows. Its stable web and proxy paths do not change during normal updates.

## Troubleshooting

- If the page loads but connecting fails immediately, confirm the durable proxy `appsettings.json` public origin exactly matches the browser address.
- If `/health` fails locally, check the proxy service logs first.
- If `/health` works but the MUD connection fails, verify the MUD is listening on the configured `MudServer` address and port from the web server.
- If HTTPS is enabled, keep the browser endpoint as `/ws`; do not hardcode `ws://` from a secure page.
