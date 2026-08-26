[CmdletBinding()]
param([Parameter(Mandatory = $true)][ValidatePattern('^[A-Za-z0-9.-]+$')][string]$Hostname)

$ErrorActionPreference = 'Stop'
$caddy = (Get-Command caddy.exe -ErrorAction SilentlyContinue).Source
$candidates = @("$env:ProgramData\Caddy\Caddyfile", 'C:\Caddy\Caddyfile')
$caddyFile = $candidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
if (-not $caddy -or -not $caddyFile) { throw 'Caddy or its Caddyfile was not found. Install the web client prerequisite first.' }
$siteDirectory = Join-Path (Split-Path $caddyFile -Parent) 'sites'
$siteFile = Join-Path $siteDirectory 'terrainplanner.caddy'
$backup = "$caddyFile.terrainplanner.bak.$(Get-Date -Format yyyyMMddHHmmss)"
$siteBackup = if (Test-Path -LiteralPath $siteFile) { "$siteFile.bak.$(Get-Date -Format yyyyMMddHHmmss)" } else { $null }
Copy-Item -LiteralPath $caddyFile -Destination $backup
New-Item -ItemType Directory -Force -Path $siteDirectory | Out-Null
if ($siteBackup) { Copy-Item -LiteralPath $siteFile -Destination $siteBackup }
$fragment = (Get-Content -LiteralPath (Join-Path $PSScriptRoot '..\Caddyfile.fragment') -Raw).Replace('{$TERRAIN_PLANNER_HOSTNAME}', $Hostname)
[IO.File]::WriteAllText($siteFile, $fragment, [Text.UTF8Encoding]::new($false))
$main = Get-Content -LiteralPath $caddyFile -Raw
if (-not $main.Contains('import sites/*.caddy')) { Add-Content -LiteralPath $caddyFile -Value "`nimport sites/*.caddy" }
& $caddy validate --config $caddyFile
if ($LASTEXITCODE -ne 0) {
	Copy-Item $backup $caddyFile -Force
	if ($siteBackup) { Copy-Item $siteBackup $siteFile -Force } else { Remove-Item $siteFile -Force }
	throw 'Caddy validation failed; configuration restored.'
}
& $caddy reload --config $caddyFile
if ($LASTEXITCODE -ne 0) {
	Copy-Item $backup $caddyFile -Force
	if ($siteBackup) { Copy-Item $siteBackup $siteFile -Force } else { Remove-Item $siteFile -Force }
	& $caddy reload --config $caddyFile
	throw 'Caddy reload failed; configuration restored.'
}
Write-Host "Caddy now proxies https://$Hostname to 127.0.0.1:5010. Backup: $backup"
