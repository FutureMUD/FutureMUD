# LabMUD Windows Web Client Setup Guide

This is a deliberately plain-English guide for installing FutureMUD Web MUD Client 1.0.1 on the Windows server that runs LabMUD. It does not change the LabMUD game server or its Discord bots. It adds a small website alongside them at `https://game.labmud.com`.

## What each piece does

- **LabMUD** is the existing game program. It continues to listen for normal MUD-client connections on port `4000`.
- **MudClientProxy** is a small, private bridge. The browser talks to it using secure WebSockets; it talks to LabMUD only over the server's own `127.0.0.1:4000` loopback address. Players can never reach it directly.
- **Caddy** is the web-server program. Think of it as the front door: it serves the client files, provides HTTPS automatically, and passes only `/ws` traffic to the private proxy.
- **A Caddyfile** is simply Caddy's plain-text instruction file. In this installation it says: "serve the files in `C:\MudClient\web\wwwroot`, send `/ws` to the local proxy, and use `game.labmud.com` for HTTPS."

The intended layout is:

```text
Player browser
    -> https://game.labmud.com          (Caddy serves the client)
    -> wss://game.labmud.com/ws         (Caddy forwards only this path)
    -> 127.0.0.1:5000                   (MudClientProxy, private)
    -> 127.0.0.1:4000                   (existing LabMUD Telnet listener, private)
```

## Before starting

This guide assumes all of the following. Stop and resolve any item that is not true.

1. You are logged into the LabMUD Windows server with an administrator account.
2. `game.labmud.com` still has its existing **DNS-only** Cloudflare A record pointing to `34.216.97.238`. Do not proxy this record through Cloudflare for this setup.
3. LabMUD is running and accepts connections locally on `127.0.0.1:4000`.
4. No other website is already using TCP ports `80` or `443` on this server. Check this before adding Caddy; two web servers cannot share those ports.
5. You have a short maintenance window in case a local configuration mistake needs correcting. The normal install does not restart LabMUD, MySQL, or either Discord bot.

## Step 1: Open only the two new public ports

In the AWS EC2 console, edit the inbound rules for the security group attached to **LabMUD Game** (`i-0d82189664245217e`). Add these two TCP rules:

| Port | Source | Why it is needed |
| --- | --- | --- |
| `80` | `0.0.0.0/0` | Lets Caddy obtain and renew its HTTPS certificate, then redirects visitors to HTTPS. |
| `443` | `0.0.0.0/0` | Serves the client and secure WebSocket connection. |

Leave the existing LabMUD and administration rules unchanged: TCP `666`, `4000`, `4500`, and restricted RDP `3389`. Do **not** open TCP `5000`; that is deliberately private to the server.

## Step 1b: Permit Caddy through Windows Firewall

The AWS security group and Windows Firewall are separate doors. Opening ports `80` and `443` in AWS is necessary, but it does not make the Windows server accept them.

On the Windows server, open **Windows Defender Firewall with Advanced Security**. Under **Inbound Rules**, create two rules:

| Rule type | Protocol | Local port | Action | Profile |
| --- | --- | --- | --- | --- |
| Port | TCP | `80` | Allow the connection | The active server profile(s) |
| Port | TCP | `443` | Allow the connection | The active server profile(s) |

Name them clearly, for example `LabMUD Web Client HTTP` and `LabMUD Web Client HTTPS`. Do not add a rule for `5000` or `4000`.

## Step 2: Download and unpack the client

Open an **Administrator PowerShell** window on the LabMUD server and run the following. It creates a new, dedicated folder and does not overwrite the game installation.

```powershell
$version = '1.0.1'
$archive = "$env:USERPROFILE\Downloads\mudclient-$version-win-x64.zip"
$staging = "C:\MudClient-$version"

Invoke-WebRequest "https://futuremud.com/downloads/mudclient/$version/mudclient-$version-win-x64.zip" -OutFile $archive
Get-FileHash $archive -Algorithm SHA256
New-Item -ItemType Directory -Path $staging -Force
Expand-Archive -LiteralPath $archive -DestinationPath $staging -Force
New-Item -ItemType Directory -Path C:\MudClient -Force
Copy-Item "$staging\mudclient-$version-win-x64\*" C:\MudClient -Recurse -Force
```

Compare the hash printed by `Get-FileHash` with the `SHA-256` link beside the Windows download on [futuremud.com/downloads](https://futuremud.com/downloads). They must match exactly.

After this step, the following files must exist:

```text
C:\MudClient\proxy\MudWebSocketProxy.exe
C:\MudClient\proxy\appsettings.json
C:\MudClient\web\wwwroot\index.html
C:\MudClient\deploy\windows\install-mudclient-proxy.ps1
C:\MudClient\deploy\windows\Caddyfile
```

## Step 3: Tell the proxy that it is serving LabMUD

Open `C:\MudClient\proxy\appsettings.json` in Notepad and replace its contents with this exact JSON. The important parts are the local game address and the exact public HTTPS address.

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
      "https://game.labmud.com"
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

Do not use `game.labmud.com` as the `MudServer` address. That would send the proxy back out to the Internet instead of keeping the game connection inside the server.

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

Download the Windows x64 Caddy executable from [caddyserver.com/download](https://caddyserver.com/download/). Put it in a folder that will not be cleaned up later; `C:\Caddy\caddy.exe` is a good choice for a new installation.

> **LabMUD deployment record:** this server's task uses `C:\Users\Administrator\Downloads\caddy_windows_amd64.exe`, because that is where the official executable was downloaded during this installation. Do not delete or move that file unless you first update the scheduled task to its new exact path. A later tidy-up can move it to `C:\Caddy\caddy.exe` and update the task at the same time.

The package already includes the Caddyfile at `C:\MudClient\deploy\windows\Caddyfile`. Open that file in Notepad:

```powershell
notepad C:\MudClient\deploy\windows\Caddyfile
```

In its first line, replace:

```text
play.example.com {
```

with:

```text
game.labmud.com {
```

Do not remove the two `handle` blocks. The first sends only `/ws` to the private proxy; the second serves files and provides the Blazor application's `index.html` fallback. Keeping them separate prevents the website fallback from swallowing a WebSocket request.

If Windows shows a security warning because the file came from the Internet, open `caddy.exe` **Properties**, tick **Unblock**, and choose **Apply**.

Use Task Scheduler again to keep Caddy running:

1. Select **Create Task** and name it `FutureMUD LabMUD Web Client HTTPS`.
2. On **General**, choose **Run whether user is logged on or not**, select the `SYSTEM` account, and tick **Run with highest privileges**.
3. On **Triggers**, add **At startup**.
4. On **Actions**, add **Start a program** with:

   | Field | Value |
   | --- | --- |
   | Program/script | The exact path to your Caddy executable. On LabMUD: `C:\Users\Administrator\Downloads\caddy_windows_amd64.exe` |
   | Add arguments | `run --config C:\MudClient\deploy\windows\Caddyfile --adapter caddyfile` |
   | Start in | The folder containing that executable. On LabMUD: `C:\Users\Administrator\Downloads` |

5. On **Settings**, allow on-demand runs and configure the same restart-on-failure policy as the proxy.
6. Save it, select it, and choose **Run**. Caddy will obtain and renew the certificate automatically.

Never copy a certificate, private key, or Cloudflare login into the client configuration. If the Caddyfile later changes, end the scheduled task, validate the changed file, then run the task again. For LabMUD, validation uses:

```powershell
& 'C:\Users\Administrator\Downloads\caddy_windows_amd64.exe' validate --config C:\MudClient\deploy\windows\Caddyfile --adapter caddyfile
```

## Step 6: Check that players can use it

On the server, these checks should both work:

```powershell
Invoke-WebRequest http://127.0.0.1:5000/health
Invoke-WebRequest https://game.labmud.com/
```

From a different computer and browser:

1. Open `https://game.labmud.com`.
2. Confirm the browser shows a normal padlock; do not bypass a certificate warning.
3. Click **Connect** in the client.
4. Confirm that the LabMUD login banner appears.
5. Log in with a non-staff test character, play briefly, disconnect, and reconnect.

If the page loads but Connect fails immediately, re-check that the `AllowedOrigins` value is exactly `https://game.labmud.com` - no trailing slash, no `www`, and no `http`.

## Rollback

This takes down only the web client; it does not touch LabMUD itself:

In Task Scheduler, select each of the two `FutureMUD ...` tasks, choose **End**, then choose **Disable**. This takes down only the web client; it does not touch LabMUD itself.

Afterward, remove the TCP `80` and `443` AWS inbound rules only if they were added solely for this client. Leave all existing game and RDP rules unchanged.

## Final record to keep

Write down these values with the server's normal administrator notes:

| Item | Expected value |
| --- | --- |
| Public client address | `https://game.labmud.com` |
| Proxy scheduled task | `FutureMUD Mud WebSocket Proxy` |
| Proxy listener | `127.0.0.1:5000` only |
| LabMUD listener used by proxy | `127.0.0.1:4000` |
| Client files | `C:\MudClient` |
| Caddy scheduled task | `FutureMUD LabMUD Web Client HTTPS` |
| Public AWS ports added | TCP `80`, TCP `443` |
| Cloudflare record | Existing DNS-only `game.labmud.com` A record; no change |
