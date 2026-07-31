# Deployment Guide

This project deploys as two pieces:

- `web/` is the Blazor WebAssembly client. Serve `web/wwwroot` from a normal HTTPS web address.
- `proxy/` is the ASP.NET Core websocket-to-telnet proxy. Run it on the same server, bound to `127.0.0.1`, and reverse-proxy `/ws` to it.

Recommended production shape:

```text
Browser -> https://play.example.com -> static Blazor files
Browser -> wss://play.example.com/ws -> local proxy on 127.0.0.1:5000 -> local MUD on 127.0.0.1:4000
```

Only ports `80` and `443` should be public. Keep the proxy port private to the server.

## Build A Release

GitHub releases are created by `.github/workflows/release.yml`.

To publish a tagged release:

```powershell
git tag v0.1.0
git push origin v0.1.0
```

The workflow tests the solution and uploads:

- `mudclient-linux-x64.zip`
- `mudclient-linux-arm64.zip`
- `mudclient-win-x64.zip`

You can also build a local package from a machine with the .NET 10 SDK. For optimized Blazor WebAssembly output, install the WebAssembly build tools once before publishing:

```powershell
dotnet workload install wasm-tools
```

```powershell
.\scripts\publish-release.ps1 -RuntimeIdentifier win-x64
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
    "AllowedOrigins": [
      "https://play.example.com"
    ]
  }
}
```

Set `MudServer` to the telnet address and port as seen from the web server. For the intended same-server install, keep `127.0.0.1`.

Set `AllowedOrigins` to the exact public web origin. Use `http://...` only for local testing; production should be `https://...`.

The Blazor client defaults to:

```json
{
  "WebSocketServer": {
    "Endpoint": "/ws"
  }
}
```

That is the easiest production setup because the browser automatically turns it into `wss://your-site/ws` when the page is served over HTTPS.

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
