param(
	[Parameter(Mandatory = $true)][string]$BaseUrl,
	[Parameter(Mandatory = $true)][string]$Token,
	[Parameter(Mandatory = $true)][string]$Product,
	[Parameter(Mandatory = $true)][string]$Version,
	[Parameter(Mandatory = $true)][string]$SourceCommit,
	[Parameter(Mandatory = $true)][string]$ArtifactDirectory,
	[string]$DocumentationCatalogue,
	[string]$UpdateManifest,
	[string]$UpdateManifestSignature,
	[string]$UpdateManifestKeyId
)

$ErrorActionPreference = 'Stop'
$chunkSize = 4MB
$headers = @{ Authorization = "Bearer $Token" }

function Invoke-PublishingRequest([scriptblock]$Send) {
	for ($attempt = 0; ; $attempt++) {
		try {
			return & $Send
		}
		catch [Microsoft.PowerShell.Commands.HttpResponseException] {
			$response = $_.Exception.Response
			if ([int]$response.StatusCode -ne 429 -or $attempt -ge 5) { throw }
			$delay = [Math]::Pow(2, $attempt + 1)
			$retryAfter = $response.Headers.RetryAfter
			if ($null -ne $retryAfter.Delta) {
				$delay = [Math]::Max($delay, $retryAfter.Delta.TotalSeconds)
			} elseif ($null -ne $retryAfter.Date) {
				$delay = [Math]::Max($delay, ($retryAfter.Date - [DateTimeOffset]::UtcNow).TotalSeconds)
			}
			if ($delay -gt 120) { throw }
			Write-Warning "Publishing request rate-limited. Retrying in $([Math]::Ceiling($delay)) seconds."
			Start-Sleep -Seconds ([Math]::Ceiling($delay))
		}
	}
}

function Get-Sha256Bytes([byte[]]$Bytes) {
	$sha256 = [Security.Cryptography.SHA256]::Create()
	try {
		return $sha256.ComputeHash($Bytes)
	}
	finally {
		$sha256.Dispose()
	}
}

$files = Get-ChildItem -LiteralPath $ArtifactDirectory -Filter '*.zip' -File | Sort-Object Name
if ($files.Count -eq 0) {
	throw 'No ZIP archives were found.'
}

$artifacts = @()
foreach ($file in $files) {
	if ($file.BaseName -notmatch '(win-x64|linux-x64|linux-arm64)$') {
		throw "Cannot determine runtime from $($file.Name)."
	}
	$hash = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
	$artifacts += @{
		artifactId = $Matches[1]
		runtime = $Matches[1]
		fileName = $file.Name
		size = $file.Length
		sha256 = $hash
	}
}

$request = @{
	product = $Product
	version = $Version
	sourceCommit = $SourceCommit
	artifacts = $artifacts
}
if ($DocumentationCatalogue) {
	$documentationFile = Get-Item -LiteralPath $DocumentationCatalogue
	$request.documentationCatalogue = @{
		artifactId = 'documentation'
		runtime = 'documentation'
		fileName = $documentationFile.Name
		size = $documentationFile.Length
		sha256 = (Get-FileHash -LiteralPath $documentationFile.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
	}
}
if ($UpdateManifest -or $UpdateManifestSignature -or $UpdateManifestKeyId) {
	if (-not $UpdateManifest -or -not $UpdateManifestSignature -or -not $UpdateManifestKeyId) {
		throw 'Update manifest, signature, and key ID must be provided together.'
	}
	$request.updateManifest = @{
		keyId = $UpdateManifestKeyId
		contentBase64 = [Convert]::ToBase64String([IO.File]::ReadAllBytes((Get-Item -LiteralPath $UpdateManifest).FullName))
		signatureBase64 = [Convert]::ToBase64String([IO.File]::ReadAllBytes((Get-Item -LiteralPath $UpdateManifestSignature).FullName))
	}
}

$draft = Invoke-PublishingRequest {
	Invoke-RestMethod -Method Post -Uri "$BaseUrl/api/publishing/v1/releases" -Headers $headers -ContentType 'application/json' -Body ($request | ConvertTo-Json -Depth 8)
}
$allFiles = @{}
foreach ($file in $files) {
	if ($file.BaseName -notmatch '(win-x64|linux-x64|linux-arm64)$') { throw "Cannot determine runtime from $($file.Name)." }
	$allFiles[$Matches[1]] = $file.FullName
}
if ($DocumentationCatalogue) { $allFiles['documentation'] = (Get-Item -LiteralPath $DocumentationCatalogue).FullName }

foreach ($artifactId in $allFiles.Keys) {
	$filePath = $allFiles[$artifactId]
	$fileInfo = Get-Item -LiteralPath $filePath
	$stream = [System.IO.File]::OpenRead($filePath)
	try {
		$index = 0
		$buffer = [byte[]]::new($chunkSize)
		while (($read = $stream.Read($buffer, 0, $buffer.Length)) -gt 0) {
			if ($draft.receivedChunks.$artifactId -contains $index) {
				$index++
				continue
			}
			$start = [int64]$index * $chunkSize
			$end = $start + $read - 1
			[byte[]]$body = if ($read -eq $buffer.Length) { $buffer } else { $buffer[0..($read - 1)] }
			$digest = [Convert]::ToBase64String((Get-Sha256Bytes $body))
			$chunkHeaders = @{
				Authorization = "Bearer $Token"
				'Content-Range' = "bytes $start-$end/$($fileInfo.Length)"
				Digest = "sha-256=$digest"
			}
			Invoke-PublishingRequest {
				Invoke-RestMethod -Method Put -Uri "$BaseUrl/api/publishing/v1/releases/$($draft.uploadId)/artifacts/$artifactId/chunks/$index" -Headers $chunkHeaders -ContentType 'application/octet-stream' -Body $body
			} | Out-Null
			$index++
		}
	}
	finally {
		$stream.Dispose()
	}
}

Invoke-PublishingRequest {
	Invoke-RestMethod -Method Post -Uri "$BaseUrl/api/publishing/v1/releases/$($draft.uploadId)/complete" -Headers $headers
} | Out-Null
$release = Invoke-PublishingRequest {
	Invoke-RestMethod -Method Post -Uri "$BaseUrl/api/publishing/v1/releases/$($draft.uploadId)/promote" -Headers $headers
}
Write-Output "Promoted $($release.product) $($release.version) from $($release.sourceCommit)."
