[CmdletBinding()]
param(
	[Parameter(Mandatory = $true)][ValidatePattern('^[A-Za-z0-9.-]+$')][string]$Hostname,
	[string]$CaddyExecutable,
	[string]$Caddyfile
)

$ErrorActionPreference = 'Stop'

function Get-WebClientCaddyTaskConfiguration {
	if (-not (Get-Command Get-ScheduledTask -ErrorAction SilentlyContinue)) { return $null }

	$task = Get-ScheduledTask -TaskName 'FutureMUD Web Client HTTPS' -ErrorAction SilentlyContinue
	if ($null -eq $task) { return $null }

	foreach ($action in $task.Actions) {
		if ([string]::IsNullOrWhiteSpace($action.Execute) -or [string]::IsNullOrWhiteSpace($action.Arguments)) { continue }
		$configMatch = [regex]::Match($action.Arguments, '(?:^|\s)--config\s+(?:"([^"]+)"|(\S+))')
		if (-not $configMatch.Success) { continue }

		$configPath = if ($configMatch.Groups[1].Success) { $configMatch.Groups[1].Value } else { $configMatch.Groups[2].Value }
		return [pscustomobject]@{
			Executable = $action.Execute
			ConfigPath = $configPath
		}
	}

	return $null
}

if ($CaddyExecutable -and -not (Test-Path -LiteralPath $CaddyExecutable)) { throw "Caddy executable '$CaddyExecutable' was not found." }
if ($Caddyfile -and -not (Test-Path -LiteralPath $Caddyfile)) { throw "Caddyfile '$Caddyfile' was not found." }

$webClientCaddy = Get-WebClientCaddyTaskConfiguration
if (-not $CaddyExecutable -and $webClientCaddy -and (Test-Path -LiteralPath $webClientCaddy.Executable)) {
	$CaddyExecutable = $webClientCaddy.Executable
}
if (-not $Caddyfile -and $webClientCaddy -and (Test-Path -LiteralPath $webClientCaddy.ConfigPath)) {
	$Caddyfile = $webClientCaddy.ConfigPath
}

if (-not $CaddyExecutable) {
	$command = Get-Command caddy.exe -ErrorAction SilentlyContinue
	if ($command) { $CaddyExecutable = $command.Source }
}
if (-not $CaddyExecutable) {
	$CaddyExecutable = @('C:\Caddy\caddy.exe', "$env:ProgramFiles\Caddy\caddy.exe") |
		Where-Object { Test-Path -LiteralPath $_ } |
		Select-Object -First 1
}
if (-not $Caddyfile) {
	$Caddyfile = @("$env:ProgramData\Caddy\Caddyfile", 'C:\Caddy\Caddyfile') |
		Where-Object { Test-Path -LiteralPath $_ } |
		Select-Object -First 1
}
if (-not $CaddyExecutable -or -not $Caddyfile) {
	throw 'Caddy or its active Caddyfile was not found. Supply -CaddyExecutable and -Caddyfile, or install the web-client prerequisite first.'
}

$main = Get-Content -LiteralPath $Caddyfile -Raw
$escapedHostname = [regex]::Escape($Hostname)
if ($main -match "(?m)^\s*$escapedHostname\s*\{") {
	Write-Host "Caddy already contains a site block for $Hostname; preserving the existing configuration."
	return
}

$siteDirectory = Join-Path (Split-Path $Caddyfile -Parent) 'sites'
$siteFile = Join-Path $siteDirectory 'terrainplanner.caddy'
$backup = "$Caddyfile.terrainplanner.bak.$(Get-Date -Format yyyyMMddHHmmss)"
$siteBackup = if (Test-Path -LiteralPath $siteFile) { "$siteFile.bak.$(Get-Date -Format yyyyMMddHHmmss)" } else { $null }
Copy-Item -LiteralPath $Caddyfile -Destination $backup
New-Item -ItemType Directory -Force -Path $siteDirectory | Out-Null
if ($siteBackup) { Copy-Item -LiteralPath $siteFile -Destination $siteBackup }
$fragment = (Get-Content -LiteralPath (Join-Path $PSScriptRoot '..\Caddyfile.fragment') -Raw).Replace('{$TERRAIN_PLANNER_HOSTNAME}', $Hostname)
[IO.File]::WriteAllText($siteFile, $fragment, [Text.UTF8Encoding]::new($false))
if (-not $main.Contains('import sites/*.caddy')) { Add-Content -LiteralPath $Caddyfile -Value "`nimport sites/*.caddy" }
& $CaddyExecutable validate --config $Caddyfile --adapter caddyfile
if ($LASTEXITCODE -ne 0) {
	Copy-Item $backup $Caddyfile -Force
	if ($siteBackup) { Copy-Item $siteBackup $siteFile -Force } else { Remove-Item $siteFile -Force }
	throw 'Caddy validation failed; configuration restored.'
}
& $CaddyExecutable reload --config $Caddyfile --adapter caddyfile
if ($LASTEXITCODE -ne 0) {
	Copy-Item $backup $Caddyfile -Force
	if ($siteBackup) { Copy-Item $siteBackup $siteFile -Force } else { Remove-Item $siteFile -Force }
	& $CaddyExecutable reload --config $Caddyfile --adapter caddyfile
	throw 'Caddy reload failed; configuration restored.'
}
Write-Host "Caddy now proxies https://$Hostname to 127.0.0.1:5010. Backup: $backup"
