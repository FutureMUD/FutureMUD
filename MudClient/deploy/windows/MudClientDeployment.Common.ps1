function Move-MudClientLegacyInstallation {
	param(
		[Parameter(Mandatory = $true)][string]$InstallRoot,
		[Parameter(Mandatory = $true)][string]$ReleaseRoot,
		[switch]$Migrate
	)

	$legacyProxy = Join-Path $InstallRoot 'proxy'
	$legacyWeb = Join-Path $InstallRoot 'web'
	$flatProxy = Get-Item -LiteralPath $legacyProxy -Force -ErrorAction SilentlyContinue
	$flatWeb = Get-Item -LiteralPath $legacyWeb -Force -ErrorAction SilentlyContinue
	$hasFlatProxy = $flatProxy -and (($flatProxy.Attributes -band [System.IO.FileAttributes]::ReparsePoint) -eq 0)
	$hasFlatWeb = $flatWeb -and (($flatWeb.Attributes -band [System.IO.FileAttributes]::ReparsePoint) -eq 0)
	$existingLegacyPaths = @(Get-ChildItem -LiteralPath $ReleaseRoot -Directory -Filter 'legacy-*' -ErrorAction SilentlyContinue |
		Sort-Object Name -Descending |
		Where-Object { (Test-Path -LiteralPath (Join-Path $_.FullName 'proxy')) -or (Test-Path -LiteralPath (Join-Path $_.FullName 'web')) })
	$completeLegacyPath = $existingLegacyPaths |
		Where-Object { (Test-Path -LiteralPath (Join-Path $_.FullName 'proxy')) -and (Test-Path -LiteralPath (Join-Path $_.FullName 'web')) } |
		Select-Object -First 1

	if (-not $hasFlatProxy -and -not $hasFlatWeb) {
		if ($Migrate -and $completeLegacyPath) { return $completeLegacyPath.FullName }
		return $null
	}
	if (-not $Migrate) { throw 'A flat MudClient installation was found. Re-run with -Migrate.' }

	$resumableLegacyPath = $existingLegacyPaths |
		Where-Object {
			$hasLegacyProxy = Test-Path -LiteralPath (Join-Path $_.FullName 'proxy')
			$hasLegacyWeb = Test-Path -LiteralPath (Join-Path $_.FullName 'web')
			(-not ($hasFlatProxy -and $hasLegacyProxy)) -and
			(-not ($hasFlatWeb -and $hasLegacyWeb)) -and
			($hasFlatProxy -or $hasLegacyProxy) -and
			($hasFlatWeb -or $hasLegacyWeb)
		} |
		Select-Object -First 1
	$legacyPath = if ($resumableLegacyPath) {
		$resumableLegacyPath.FullName
	}
	else {
		Join-Path $ReleaseRoot ("legacy-" + (Get-Date -Format 'yyyyMMddHHmmssfff'))
	}
	New-Item -ItemType Directory -Force -Path $legacyPath | Out-Null

	foreach ($legacyComponent in @(
		@{ Name = 'proxy'; Source = $legacyProxy; Present = $hasFlatProxy },
		@{ Name = 'web'; Source = $legacyWeb; Present = $hasFlatWeb }
	)) {
		if (-not $legacyComponent.Present) { continue }
		$legacyDestination = Join-Path $legacyPath $legacyComponent.Name
		if (Test-Path -LiteralPath $legacyDestination) {
			throw "The interrupted migration contains both '$($legacyComponent.Source)' and '$legacyDestination'. Move one aside and retry."
		}
		Move-Item -LiteralPath $legacyComponent.Source -Destination $legacyDestination
	}

	if (-not (Test-Path -LiteralPath (Join-Path $legacyPath 'proxy')) -or -not (Test-Path -LiteralPath (Join-Path $legacyPath 'web'))) {
		throw "The legacy installation is incomplete. Both '$legacyPath\proxy' and '$legacyPath\web' are required to resume migration."
	}
	return $legacyPath
}

function Stop-MudClientLegacyProxyProcess {
	param(
		[Parameter(Mandatory = $true)][string]$ProxyRoot,
		[int]$TimeoutSeconds = 30
	)

	$normalisedProxyRoot = [System.IO.Path]::GetFullPath($ProxyRoot).TrimEnd('\') + '\'
	$deadline = (Get-Date).AddSeconds($TimeoutSeconds)
	do {
		$legacyProcesses = @(Get-Process -Name 'MudWebSocketProxy' -ErrorAction SilentlyContinue |
			Where-Object {
				try {
					$executablePath = $_.Path
					-not [string]::IsNullOrWhiteSpace($executablePath) -and
						[System.IO.Path]::GetFullPath($executablePath).StartsWith(
							$normalisedProxyRoot,
							[System.StringComparison]::OrdinalIgnoreCase)
				}
				catch {
					$false
				}
			})
		if ($legacyProcesses.Count -eq 0) { return }

		$legacyProcesses | Stop-Process -Force -ErrorAction SilentlyContinue
		Start-Sleep -Milliseconds 500
	} while ((Get-Date) -lt $deadline)

	throw "The legacy MudWebSocketProxy process did not release '$ProxyRoot' within $TimeoutSeconds seconds."
}
