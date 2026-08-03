# FutureMUD Web MUD Client deployment

The client is published through the FutureMUD product release flow and downloaded from [futuremud.com/downloads](https://futuremud.com/downloads). Each MUD operator hosts it for their own game; futuremud.com publishes the release package but does not run the proxy for individual games.

If you are setting this up for the first time on a Windows game server, start with the [plain-English Windows guide](Deployment/LabMUD_Windows_Guide.md). It explains the small amount of web-server terminology, shows the complete LabMUD example, and includes a safe verification and rollback checklist.

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

## Linux automated install

After verifying the website's published SHA-256 checksum, extract the downloaded package to `/opt/mudclient` and run the installer from that directory:

~~~bash
unzip mudclient-1.0.1-linux-x64.zip -d /tmp/mudclient-package
sudo mv /tmp/mudclient-package/mudclient-1.0.1-linux-x64 /opt/mudclient
sudo bash /opt/mudclient/deploy/linux/install-mudclient.sh play.example.com
~~~

The script creates the unprivileged proxy service, writes the exact trusted public origin, adds an isolated Caddy site fragment, validates Caddy before reload, and checks the private health endpoint. The selected domain must already resolve to the host. To connect to a MUD on a private network rather than the same machine:

~~~bash
sudo bash /opt/mudclient/deploy/linux/install-mudclient.sh play.example.com 10.0.0.20 4000
~~~

Use `CADDY_CONFIG` and `CADDY_FRAGMENTS_DIR` for nonstandard Caddyfile locations. The installer saves `.before-mudclient-install` backups of the files it changes; review those and take a normal deployment backup before upgrading an existing installation.

## Build A Release

The production release is created by an annotated `mudclient-v<version>` tag in the FutureMUD repository. The normal product workflow installs the WebAssembly build tools, fails closed if NuGet advisory data is unavailable or any direct/transitive dependency is vulnerable, runs the client tests, produces self-contained Windows x64, Linux x64, and Linux ARM64 packages, smoke-publishes them to a temporary website host, and promotes the validated archives to futuremud.com.

You can also build a local package from a machine with the .NET 10 SDK. For optimized Blazor WebAssembly output, install the WebAssembly build tools once before publishing:

```powershell
dotnet workload install wasm-tools
```

```powershell
.\scripts\Publish-ProductPackage.ps1 -RuntimeIdentifier win-x64 -Version 1.0.1
```

```bash
bash scripts/publish-release.sh linux-x64
```

## Configure The Package

Unzip the release package on the server. The examples below use `/opt/mudclient` on Linux and `C:\MudClient` on Windows.

Edit `proxy/appsettings.json`:

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

Copy the package to `/opt/mudclient`, then adjust ownership:

```bash
sudo useradd --system --home /opt/mudclient --shell /usr/sbin/nologin mudclient
sudo mkdir -p /opt/mudclient
sudo unzip mudclient-linux-x64.zip -d /opt/mudclient-tmp
sudo cp -R /opt/mudclient-tmp/mudclient-linux-x64/. /opt/mudclient/
sudo chown -R mudclient:mudclient /opt/mudclient
sudo chmod +x /opt/mudclient/proxy/MudWebSocketProxy
```

Install the systemd service template:

```bash
sudo cp /opt/mudclient/deploy/linux/mudclient-proxy.service /etc/systemd/system/mudclient-proxy.service
sudo systemctl daemon-reload
sudo systemctl enable --now mudclient-proxy
sudo systemctl status mudclient-proxy
```

The service listens on `http://127.0.0.1:5000`.

## Web Server

Caddy is the simplest cross-platform option. Copy `deploy/Caddyfile`, replace the domain and install path, then run or reload Caddy.

Linux administrators who prefer Nginx can copy `deploy/linux/nginx-mudclient.conf`, replace the domain and install path, enable the site, and add TLS with their normal certificate tooling.

After the web server is running, check:

```bash
curl http://127.0.0.1:5000/health
```

Then open the public web address and connect from the client.

## Windows Service

Unzip `mudclient-win-x64.zip` to `C:\MudClient`, edit `C:\MudClient\proxy\appsettings.json`, then run PowerShell as Administrator:

```powershell
Set-ExecutionPolicy -Scope Process Bypass
C:\MudClient\deploy\windows\install-mudclient-proxy.ps1
```

Use `deploy\windows\Caddyfile` as the matching static-file and websocket reverse-proxy example for Caddy on Windows.

To remove the Windows service:

```powershell
C:\MudClient\deploy\windows\install-mudclient-proxy.ps1 -Uninstall
```

## Troubleshooting

- If the page loads but connecting fails immediately, confirm the public origin in `proxy/appsettings.json` exactly matches the browser address.
- If `/health` fails locally, check the proxy service logs first.
- If `/health` works but the MUD connection fails, verify the MUD is listening on the configured `MudServer` address and port from the web server.
- If HTTPS is enabled, keep the browser endpoint as `/ws`; do not hardcode `ws://` from a secure page.
