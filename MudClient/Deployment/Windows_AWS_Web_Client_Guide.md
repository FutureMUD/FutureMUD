# Windows and AWS Web Client Setup Guide

This is a deliberately plain-English guide for installing FutureMUD Web MUD Client 1.2.0 on a Windows server hosted in AWS. It adds a small HTTPS website alongside an existing FutureMUD game, without changing the game itself or any other applications on the server.

The names and addresses below are examples. Before using a command, replace `play.example.com` with your own player-facing hostname. Do not put a live server's IP address, AWS instance ID, administrator account name, or credentials in a public guide or repository.

## What each piece does

- **Your MUD** is the existing game program. In this example it accepts normal MUD-client connections on port `4000`.
- **MudWebSocketProxy** is a small, private bridge. The browser talks to it using secure WebSockets; it talks to the MUD only over the server's own `127.0.0.1:4000` loopback address. Players cannot reach it directly.
- **Caddy** is the web-server program. Think of it as the front door: it serves the client files, provides HTTPS automatically, and passes only `/ws` traffic to the private proxy.
- **A Caddyfile** is simply Caddy's plain-text instruction file. In this installation it says: "serve the client files, send `/ws` to the local proxy, and use this hostname for HTTPS."

The intended layout is:

```text
Player browser
    -> https://play.example.com         (Caddy serves the client)
    -> wss://play.example.com/ws        (Caddy forwards only this path)
    -> 127.0.0.1:5000                   (MudWebSocketProxy, private)
    -> 127.0.0.1:4000                   (existing MUD Telnet listener, private)
```

## Before starting

This guide assumes all of the following. Stop and resolve any item that is not true.

1. You are logged into the Windows server with an administrator account.
2. Your public hostname has a DNS record pointing to this server. Caddy must be able to receive public TCP traffic on ports `80` and `443` to obtain and renew an HTTPS certificate.
3. Your MUD is running and accepts connections locally on the address and port that you will configure in Step 3. The same-server default is `127.0.0.1:4000`.
4. No other website is already using TCP ports `80` or `443` on this server. Two web servers cannot share the same port.
5. You have a current backup and a short maintenance window in case a local configuration mistake needs correcting. The normal install does not restart the MUD, database, or unrelated services.

If you use Cloudflare, choose the DNS/proxy arrangement that matches your certificate plan. The simplest first installation is a DNS-only hostname while Caddy obtains its certificate. Do not expose the MUD Telnet port or the proxy port through Cloudflare or directly to the Internet.

## Step 1: Open only the two new public ports in AWS

In the AWS EC2 console, edit the inbound rules for the security group attached to the Windows game server. Add these two TCP rules:

| Port | Source | Why it is needed |
| --- | --- | --- |
| `80` | Your public IPv4/IPv6 policy | Lets Caddy obtain and renew its HTTPS certificate, then redirects visitors to HTTPS. |
| `443` | Your public IPv4/IPv6 policy | Serves the client and its secure WebSocket connection. |

For a normal public game website, the IPv4 source is commonly `0.0.0.0/0`. Add an IPv6 equivalent only if your server and DNS are intended to serve IPv6.

Leave existing game and administration rules alone. Do **not** open the MUD listener (`4000` in this example) or proxy listener (`5000`) to the public Internet.

## Step 1b: Permit Caddy through Windows Firewall

The AWS security group and Windows Firewall are separate doors. Opening ports `80` and `443` in AWS is necessary, but it does not make Windows accept them.

On the Windows server, open **Windows Defender Firewall with Advanced Security**. Under **Inbound Rules**, create two rules:

| Rule type | Protocol | Local port | Action | Profile |
| --- | --- | --- | --- | --- |
| Port | TCP | `80` | Allow the connection | The active server profile(s) |
| Port | TCP | `443` | Allow the connection | The active server profile(s) |

Name them clearly, for example `Web MUD Client HTTP` and `Web MUD Client HTTPS`. Do not add a firewall rule for `5000` or the MUD's Telnet port.

## Step 2: Install once, then use the updater

Open an **Administrator PowerShell** window on the Windows server and run the following. It creates a new, dedicated folder and does not overwrite the game installation.

```powershell
$version = '1.2.0'
$archive = "$env:USERPROFILE\Downloads\mudclient-$version-win-x64.zip"
$staging = "C:\MudClient-$version"

Invoke-WebRequest "https://futuremud.com/downloads/mudclient/$version/mudclient-$version-win-x64.zip" -OutFile $archive
Get-FileHash $archive -Algorithm SHA256
New-Item -ItemType Directory -Path $staging -Force
Expand-Archive -LiteralPath $archive -DestinationPath $staging -Force
& "$staging\mudclient-$version-win-x64\deploy\windows\Install-MudClient.ps1" `
  -ArchivePath $archive -InstallRoot C:\MudClient
```

Compare the hash printed by `Get-FileHash` with the `SHA-256` link beside the Windows download on [futuremud.com/downloads](https://futuremud.com/downloads). They must match exactly.

After this step, the following files must exist:

```text
C:\MudClient\proxy\MudWebSocketProxy.exe
C:\ProgramData\FutureMUD\MudClient\proxy\appsettings.json
C:\MudClient\web\wwwroot\index.html
C:\MudClient\deploy\windows\Caddyfile
```

For an existing 1.0.1 or 1.1.0 installation, use the same command with `-Migrate`. It preserves the old release and existing Caddyfile, moves proxy/browser settings to `%ProgramData%\FutureMUD\MudClient`, and replaces only the old proxy task with a Windows Service.

After that one-time migration, an Administrator updates the client with one command:

```powershell
& 'C:\MudClient\current\deploy\windows\Update-MudClient.ps1'
```

Use `-Check` to inspect the signed latest release or `-Rollback` to return to the previous local release. An update briefly disconnects web-client players while the private proxy restarts, but does not restart the MUD, database, Caddy task, DNS, TLS, AWS security group, or firewall.

## Step 3: Tell the proxy where the MUD is

Open `C:\ProgramData\FutureMUD\MudClient\proxy\appsettings.json` in Notepad and replace its contents with this JSON. Change the `Port` if your MUD uses a port other than `4000`, and replace `play.example.com` with the exact public HTTPS hostname that players will use.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "MudServer": {
    "Address": "127.0.0.1",
    "Port": 4000
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

Do not use your public hostname as the `MudServer` address. That would send the proxy out to the Internet instead of keeping the game connection inside the server.

The allowed origin must match the player URL exactly: it has no trailing slash, no extra `www`, and uses `https` in production.

## Step 4: Make the private proxy start automatically

Use Windows **Task Scheduler**. This avoids leaving a console window open and starts the proxy even when nobody is logged on.

1. In Task Scheduler, select **Create Task** (not "Create Basic Task").
2. On **General**, name it `FutureMUD Mud WebSocket Proxy`, choose **Run whether user is logged on or not**, and choose `SYSTEM` as the account.
3. On **Triggers**, add **At startup**.
4. On **Actions**, add **Start a program** with:

   | Field | Value |
   | --- | --- |
   | Program/script | `C:\MudClient\proxy\MudWebSocketProxy.exe` |
   | Add arguments | `--urls http://127.0.0.1:5000` |
   | Start in | `C:\MudClient\proxy` |

5. On **Settings**, allow the task to be run on demand and set it to restart on failure (for example, after one minute, up to three times).
6. Save the task, right-click it, and choose **Run**. Its status should become **Running**.

The proxy listens only at `127.0.0.1:5000`. It is not supposed to be reachable from another computer.

## Step 5: Install Caddy and give it the Caddyfile

Download the Windows x64 Caddy executable from [caddyserver.com/download](https://caddyserver.com/download/). Put it in a folder that will not be cleaned up later, such as `C:\Caddy\caddy.exe`.

The package already includes the Caddyfile at `C:\MudClient\deploy\windows\Caddyfile`. Open it in Notepad:

```powershell
notepad C:\MudClient\deploy\windows\Caddyfile
```

In its first line, replace:

```text
play.example.com {
```

with your real hostname, for example:

```text
mud.example.com {
```

Do not remove the two `handle` blocks. The first sends only `/ws` to the private proxy; the second serves files and provides the Blazor application's `index.html` fallback. Keeping them separate prevents the website fallback from swallowing a WebSocket request.

If Windows shows a security warning because the executable came from the Internet, open `caddy.exe` **Properties**, tick **Unblock**, and choose **Apply**.

Validate the configuration before starting the service:

```powershell
& 'C:\Caddy\caddy.exe' validate --config C:\MudClient\deploy\windows\Caddyfile --adapter caddyfile
```

Use Task Scheduler again to keep Caddy running:

1. Select **Create Task** and name it `FutureMUD Web Client HTTPS`.
2. On **General**, choose **Run whether user is logged on or not**, select the `SYSTEM` account, and tick **Run with highest privileges**.
3. On **Triggers**, add **At startup**.
4. On **Actions**, add **Start a program** with:

   | Field | Value |
   | --- | --- |
   | Program/script | `C:\Caddy\caddy.exe` |
   | Add arguments | `run --config C:\MudClient\deploy\windows\Caddyfile --adapter caddyfile` |
   | Start in | `C:\Caddy` |

5. On **Settings**, allow on-demand runs and configure the same restart-on-failure policy as the proxy.
6. Save it, select it, and choose **Run**. Caddy will obtain and renew the certificate automatically.

Never copy a certificate, private key, Cloudflare login, or other credential into the client configuration. If the Caddyfile later changes, end the scheduled task, validate the changed file, then run the task again.

## Step 6: Check that players can use it

On the server, this local health check should work:

```powershell
Invoke-WebRequest http://127.0.0.1:5000/health
```

From a different computer and browser:

1. Open your public URL, for example `https://play.example.com`.
2. Confirm the browser shows a normal padlock; do not bypass a certificate warning.
3. Confirm the MUD login banner appears and use a non-staff test account to log in.
4. Disconnect and reconnect once.

If the page loads but the client reports that it cannot connect, first check that the proxy task is **Running**. Then re-check that `AllowedOrigins` exactly matches the browser address.

## Rollback

This takes down only the web client; it does not touch the MUD itself:

1. In Task Scheduler, select `FutureMUD Mud WebSocket Proxy` and `FutureMUD Web Client HTTPS`.
2. Choose **End**, then choose **Disable** for each task.
3. Remove the AWS and Windows Firewall rules for TCP `80` and `443` only if they were added solely for this client.

Leave all existing game and administration rules unchanged.

## Final record to keep privately

Keep the following values in your private administrator notes, not in a public repository:

| Item | Value to record |
| --- | --- |
| Public client address | Your HTTPS player URL |
| Proxy scheduled task | `FutureMUD Mud WebSocket Proxy` |
| Proxy listener | `127.0.0.1:5000` only |
| MUD listener used by proxy | The configured private address and port |
| Client files | `C:\MudClient` |
| Caddy executable path | Your chosen `caddy.exe` location |
| Caddy scheduled task | `FutureMUD Web Client HTTPS` |
| AWS ports added | TCP `80`, TCP `443` |
| DNS and proxy settings | Your current provider configuration |
