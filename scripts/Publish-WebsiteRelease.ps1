param(
	[Parameter(Mandatory = $true)][string]$BaseUrl,
	[Parameter(Mandatory = $true)][string]$Token,
	[Parameter(Mandatory = $true)][string]$Product,
	[Parameter(Mandatory = $true)][string]$Version,
	[Parameter(Mandatory = $true)][string]$SourceCommit,
	[Parameter(Mandatory = $true)][string]$ArtifactDirectory,
	[string]$DocumentationCatalogue
)

$ErrorActionPreference = 'Stop'
$chunkSize = 32MB
$headers = @{ Authorization = "Bearer $Token" }

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

$draft = Invoke-RestMethod -Method Post -Uri "$BaseUrl/api/publishing/v1/releases" -Headers $headers -ContentType 'application/json' -Body ($request | ConvertTo-Json -Depth 8)
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
			$start = [int64]$index * $chunkSize
			$end = $start + $read - 1
			[byte[]]$body = if ($read -eq $buffer.Length) { $buffer } else { $buffer[0..($read - 1)] }
			$digest = [Convert]::ToBase64String((Get-Sha256Bytes $body))
			$chunkHeaders = @{
				Authorization = "Bearer $Token"
				'Content-Range' = "bytes $start-$end/$($fileInfo.Length)"
				Digest = "sha-256=$digest"
			}
			Invoke-RestMethod -Method Put -Uri "$BaseUrl/api/publishing/v1/releases/$($draft.uploadId)/artifacts/$artifactId/chunks/$index" -Headers $chunkHeaders -ContentType 'application/octet-stream' -Body $body | Out-Null
			$index++
		}
	}
	finally {
		$stream.Dispose()
	}
}

Invoke-RestMethod -Method Post -Uri "$BaseUrl/api/publishing/v1/releases/$($draft.uploadId)/complete" -Headers $headers | Out-Null
$release = Invoke-RestMethod -Method Post -Uri "$BaseUrl/api/publishing/v1/releases/$($draft.uploadId)/promote" -Headers $headers
Write-Output "Promoted $($release.product) $($release.version) from $($release.sourceCommit)."
