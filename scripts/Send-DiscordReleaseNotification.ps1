[CmdletBinding()]
param(
	[Parameter(Mandatory)]
	[ValidateSet('engine', 'seeder', 'discordbot', 'terrainplanner', 'mudclient')]
	[string]$Product,

	[Parameter(Mandatory)]
	[ValidatePattern('^\d+\.\d+\.\d+$')]
	[string]$Version,

	[Parameter(Mandatory)]
	[string]$WebhookUrl,

	[switch]$DryRun
)

$ErrorActionPreference = 'Stop'

$productMetadata = @{
	engine = @{
		Name = 'FutureMUD Engine'
		PatchNotesSlug = 'engine'
	}
	seeder = @{
		Name = 'FutureMUD Database Seeder'
		PatchNotesSlug = 'database-seeder'
	}
	discordbot = @{
		Name = 'FutureMUD Discord Bot'
		PatchNotesSlug = 'discord-bot'
	}
	terrainplanner = @{
		Name = 'FutureMUD Terrain Planner & Engine API'
		PatchNotesSlug = 'terrain-planner'
	}
	mudclient = @{
		Name = 'FutureMUD Web MUD Client'
		PatchNotesSlug = 'mudclient'
	}
}

$metadata = $productMetadata[$Product]
$versionSlug = $Version.Replace('.', '-')
$patchNotesUrl = "https://futuremud.com/patch-notes/$($metadata.PatchNotesSlug)-$versionSlug"
$payload = @{
	content = "**$($metadata.Name) version $Version released.** [Patch notes here]($patchNotesUrl)"
	allowed_mentions = @{
		parse = @()
	}
} | ConvertTo-Json -Depth 3 -Compress

if ($DryRun) {
	$payload
	return
}

$webhookUri = $null
if (-not [Uri]::TryCreate($WebhookUrl, [UriKind]::Absolute, [ref]$webhookUri) -or
	$webhookUri.Scheme -cne 'https' -or
	$webhookUri.Host -cne 'discord.com' -or
	$webhookUri.Query -or
	$webhookUri.Fragment -or
	$webhookUri.AbsolutePath -cnotmatch '^/api/webhooks/\d+/[A-Za-z0-9._-]+$') {
	throw 'The configured Discord webhook URL is not a valid discord.com webhook.'
}

$patchNotesAvailable = $false
for ($attempt = 1; $attempt -le 12; $attempt++) {
	try {
		Invoke-WebRequest -Method Get -Uri $patchNotesUrl -MaximumRedirection 0 | Out-Null
		$patchNotesAvailable = $true
		break
	}
	catch {
		if ($attempt -lt 12) {
			Start-Sleep -Seconds 5
		}
	}
}

if (-not $patchNotesAvailable) {
	throw "Patch notes were not live at $patchNotesUrl after 12 attempts; the Discord notification was not sent."
}

try {
	$confirmedWebhookUri = [UriBuilder]::new($webhookUri)
	$confirmedWebhookUri.Query = 'wait=true'
	Invoke-RestMethod -Method Post -Uri $confirmedWebhookUri.Uri -ContentType 'application/json' -Body $payload | Out-Null
}
catch {
	# Do not include the exception because PowerShell may render the secret webhook URL.
	throw 'Discord rejected the release notification. Check the protected environment secret and webhook configuration.'
}

Write-Host "Published the $($metadata.Name) $Version announcement with patch notes at $patchNotesUrl."
