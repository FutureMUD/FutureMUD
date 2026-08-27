# FutureMUD Terrain Planner & Engine API deployment

Terrain Planner 2.x is one service: the hosted planner UI and its read-only Engine API are installed, configured, started, upgraded, and rolled back together. It expects the FutureMUD web-client prerequisite to have already installed Caddy and normal service-management support. The only public listener is Caddy on ports 80/443; Kestrel listens only on `127.0.0.1:5010`.

## Before installation

1. Create a DNS record such as `planner.example.com` pointing at the game server.
2. Create a dedicated MySQL login. Grant it `SELECT` only on the FutureMUD `Accounts`, `AuthorityGroups`, `Terrains`, and `Tags` tables. Do not reuse the engine's read/write login.
3. Download the archive for the server runtime and verify its published SHA-256 checksum. Linux installation requires `systemd`, `curl`, `python3`, and Caddy from the web-client prerequisite. Updates performed by the supplied updater additionally verify the signed update manifest before extraction.
4. Back up the game database and the Caddy configuration. The installer also creates a timestamped Caddy backup before editing it.

Example MySQL commands (replace the database, host, login, and generated password):

```sql
CREATE USER 'terrainplanner'@'127.0.0.1' IDENTIFIED BY 'GENERATE_A_LONG_RANDOM_PASSWORD';
GRANT SELECT ON futuremud.Accounts TO 'terrainplanner'@'127.0.0.1';
GRANT SELECT ON futuremud.AuthorityGroups TO 'terrainplanner'@'127.0.0.1';
GRANT SELECT ON futuremud.Terrains TO 'terrainplanner'@'127.0.0.1';
GRANT SELECT ON futuremud.Tags TO 'terrainplanner'@'127.0.0.1';
FLUSH PRIVILEGES;
```

## Linux first installation

Extract the archive. It creates one directory named for the runtime package; change into that directory, then run:

```bash
unzip terrainplanner-2.0.5-linux-x64.zip
cd terrainplanner-2.0.5-linux-x64
sudo bash deploy/linux/install-terrainplanner.sh planner.example.com
```

On the first run the installer creates `/etc/futuremud/terrainplanner/appsettings.Production.json` and stops. Replace `REPLACE_WITH_SECRET` in that root-readable configuration file, then run the same command again. The durable layout is:

- `/opt/futuremud/terrainplanner/releases/<version>` — immutable application releases;
- `/opt/futuremud/terrainplanner/current` — the active release link;
- `/etc/futuremud/terrainplanner` — durable configuration;
- `/var/lib/futuremud/terrainplanner/keys` — durable ASP.NET data-protection keys.

The installer creates the unprivileged `terrainplanner` account, installs a hardened systemd unit, activates the release, checks `/health/ready`, restores the previous link on failure, retains two prior releases, installs an isolated Caddy site fragment, validates Caddy, and restores the Caddyfile if reload fails.

## Windows first installation

Open an elevated PowerShell prompt. The archive extracts to an outer download directory which contains the actual runtime package directory; change into the inner directory before running the installer:

```powershell
$archive = 'C:\Install\terrainplanner-2.0.5-win-x64.zip'
$extractRoot = 'C:\Install\terrainplanner-2.0.5'
Expand-Archive -LiteralPath $archive -DestinationPath $extractRoot -Force
Set-Location "$extractRoot\terrainplanner-2.0.5-win-x64"
.\deploy\windows\Install-TerrainPlanner.ps1 -Hostname planner.example.com
```

On the first run it creates `%ProgramFiles%\FutureMUD\TerrainPlanner\shared\appsettings.Production.json` and stops before creating a release. Replace `REPLACE_WITH_SECRET`, restrict that file to Administrators and SYSTEM, then rerun the same command.

The installer discovers the Web MUD Client's `FutureMUD Web Client HTTPS` scheduled task and uses its active Caddy executable and Caddyfile. It then adds an isolated site fragment, validates the Caddyfile with the Caddyfile adapter, and restores the previous configuration if that fails. If the Web MUD Client was installed unusually and its task cannot be discovered, supply both paths explicitly:

```powershell
.\deploy\windows\Install-TerrainPlanner.ps1 -Hostname planner.example.com `
  -CaddyExecutable 'C:\path\to\caddy.exe' -Caddyfile 'C:\path\to\Caddyfile'
```

The installer creates the `FutureMUDTerrainPlanner` Windows Service as the low-privilege built-in `LocalService` account, enables its service SID, and gives that SID Modify access only to the durable `shared` directory. It creates a stable `current` junction and provides health-check rollback and release retention. Do not change the account to an administrator account.

## Upgrade

Linux:

```bash
sudo /opt/futuremud/terrainplanner/current/deploy/linux/update-terrainplanner.sh linux-x64 planner.example.com
```

Use `linux-arm64` on ARM64 hosts. Windows:

```powershell
& "$env:ProgramFiles\FutureMUD\TerrainPlanner\current\deploy\windows\Update-TerrainPlanner.ps1" -RuntimeIdentifier win-x64 -Hostname planner.example.com
```

The updater downloads `update-manifest.json`, its Ed25519 signature, and the named archive from futuremud.com. The bundled verifier checks the signature, product/version/runtime identity, size, and SHA-256 before the normal health-checked installer activates it. It automatically reuses the Caddy paths from the Web MUD Client scheduled task; if that task cannot be discovered, pass the same `-CaddyExecutable` and `-Caddyfile` arguments shown above.

### Bootstrap an earlier 2.0 installation

If your existing installation is 2.0.0 through 2.0.4, install 2.0.5 once using the normal archive-install method above instead of its bundled updater. Versions 2.0.0 and 2.0.1 used a retired archive route. Version 2.0.2 could stop while configuring the service, 2.0.3 used an API that is unavailable in Windows PowerShell 5.1, and the 2.0.4 Windows Caddy check could add a duplicate site fragment when the hostname was already defined directly in the main Caddyfile. This retains the existing shared configuration, MySQL password, service, and Caddy configuration. From 2.0.5 onwards, use the updater command in the preceding section.

If 2.0.4 reported ambiguous site definition but https://planner.example.com/health/ready responds, the planner service is already healthy: Caddy restored its previous configuration after declining the duplicate fragment. Install 2.0.5 once using the archive method. It detects and preserves the existing direct Caddy site block.

During an upgrade, the installer replaces only the `current` release junction. It never recursively deletes the release that junction points to. The service executable always points at that stable junction, so upgrades restart the existing service without rewriting its command line.

## Verification

```bash
curl --fail http://127.0.0.1:5010/health/live
curl --fail http://127.0.0.1:5010/health/ready
curl --fail https://planner.example.com/health/ready
```

On Windows PowerShell 5.1, opt into TLS 1.2 before the HTTPS check (older defaults can report a misleading SSL/TLS channel error even when Caddy is working):

```powershell
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
Invoke-RestMethod https://planner.example.com/health/ready
```

Then sign in with an existing registered, non-suspended FutureMUD account whose authority is Admin or higher. Confirm terrain and tag palettes load. The service never creates accounts and has no database write endpoint.

## Manual rollback

Stop the service, repoint `current` to one of the two retained releases, and start it again. On Linux use `ln -sfn`; on Windows remove and recreate the directory junction. Verify both health probes afterward. Configuration and session-key material survive because neither lives inside a release directory.
