param(
	[Parameter(Mandatory = $true)]
	[string]$Target,

	[string]$DormantForm,
	[string]$MoveCellId,
	[string]$OutputFile,
	[string]$TranscriptFile,
	[string]$MudSessionScript,
	[string]$Account,
	[string]$Password,
	[string]$Character,
	[switch]$Run,
	[switch]$UseRunning,
	[switch]$KeepRunning,
	[switch]$NoCleanup
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repoRoot = Split-Path -Parent $PSScriptRoot
$scratchRoot = Join-Path $repoRoot '.tmp'

function Initialize-Scratch {
	if (!(Test-Path $scratchRoot)) {
		New-Item -Path $scratchRoot -ItemType Directory | Out-Null
	}
}

function Quote-MudArgument {
	param(
		[Parameter(Mandatory = $true)]
		[string]$Text
	)

	if ([string]::IsNullOrWhiteSpace($Text)) {
		throw 'MUD command arguments cannot be blank.'
	}

	if ($Text -match '\s|"') {
		return '"' + ($Text -replace '"', '\"') + '"'
	}

	return $Text
}

function Resolve-MudSessionScript {
	if (![string]::IsNullOrWhiteSpace($MudSessionScript)) {
		return $MudSessionScript
	}

	if (![string]::IsNullOrWhiteSpace($env:FUTUREMUD_MUD_SESSION_SCRIPT)) {
		return $env:FUTUREMUD_MUD_SESSION_SCRIPT
	}

	$candidate = Join-Path $HOME '.codex\skills\futuremud-mud-tester\scripts\mud_session.py'
	if (Test-Path $candidate) {
		return $candidate
	}

	throw 'No mud_session.py path was supplied. Pass -MudSessionScript or set FUTUREMUD_MUD_SESSION_SCRIPT.'
}

function New-BaselineCommands {
	param(
		[Parameter(Mandatory = $true)]
		[string]$TargetArg
	)

	return @(
		'# Character instance smoke test - baseline checks',
		'# Expected: instance help renders, persisted audit does not crash, target has a primary instance.',
		'help instance',
		'instance audit all',
		"instance list $TargetArg",
		"instance audit $TargetArg",
		"force $TargetArg instances",
		"force $TargetArg focus"
	)
}

function New-ManualCommands {
	param(
		[Parameter(Mandatory = $true)]
		[string]$TargetArg,
		[string]$FormArg,
		[string]$DestinationCell
	)

	$commands = [System.Collections.Generic.List[string]]::new()
	$commands.AddRange([string[]](New-BaselineCommands -TargetArg $TargetArg))

	if ([string]::IsNullOrWhiteSpace($FormArg)) {
		$commands.Add('')
		$commands.Add('# No dormant form was supplied. Re-run this script with -DormantForm <alias-or-body-id> to generate spawn/focus checks.')
		return $commands.ToArray()
	}

	$commands.Add('')
	$commands.Add('# Passive secondary test')
	$commands.Add('# Prerequisite: the form is owned by the target and is not the target''s current embodied body.')
	$commands.Add('# Expected: spawn succeeds, player instances shows the secondary as passive, focus 2 is rejected.')
	$commands.Add("instance spawn $TargetArg $FormArg here temporary passive")
	$commands.Add("instance list $TargetArg")
	$commands.Add('look')
	$commands.Add("force $TargetArg instances")
	$commands.Add("force $TargetArg focus 2")
	$commands.Add('# Copy the spawned instance id from the spawn/list output, then run: instance retire <instance-id>')
	$commands.Add('# Run the focusable section only after the passive instance is retired if you are reusing the same form.')
	$commands.Add('')
	$commands.Add('# Focusable secondary test')
	$commands.Add('# Expected: focus 2 succeeds, commands execute through the secondary, focus primary restores the original body.')
	$commands.Add("instance spawn $TargetArg $FormArg here temporary focusable")
	$commands.Add("instance list $TargetArg")
	$commands.Add("force $TargetArg instances")
	$commands.Add("force $TargetArg focus 2")
	$commands.Add("force $TargetArg look")
	if (![string]::IsNullOrWhiteSpace($DestinationCell)) {
		$commands.Add('# After copying the focusable instance id, optionally run: instance move <instance-id> room ' + $DestinationCell)
	}

	$commands.Add("force $TargetArg focus primary")
	$commands.Add("force $TargetArg instances")
	$commands.Add('# Cleanup: instance retire <focusable-instance-id>')
	$commands.Add("instance audit $TargetArg")
	$commands.Add('instance audit all')

	return $commands.ToArray()
}

function Write-CommandsFile {
	param(
		[Parameter(Mandatory = $true)]
		[AllowEmptyString()]
		[string[]]$Commands,
		[Parameter(Mandatory = $true)]
		[string]$Path
	)

	$parent = Split-Path -Parent $Path
	if (![string]::IsNullOrWhiteSpace($parent) -and !(Test-Path $parent)) {
		New-Item -Path $parent -ItemType Directory | Out-Null
	}

	Set-Content -Path $Path -Value $Commands -Encoding UTF8
}

function Invoke-MudCommands {
	param(
		[Parameter(Mandatory = $true)]
		[string[]]$Commands,
		[Parameter(Mandatory = $true)]
		[string]$PhaseName,
		[Parameter(Mandatory = $true)]
		[string]$SessionScript,
		[switch]$KeepServer
	)

	$phaseFile = Join-Path $scratchRoot "character-instance-$PhaseName.commands.txt"
	Write-CommandsFile -Commands $Commands -Path $phaseFile

	$args = [System.Collections.Generic.List[string]]::new()
	$args.Add($SessionScript)
	$args.Add('--repo')
	$args.Add($repoRoot)
	$args.Add('--commands-file')
	$args.Add($phaseFile)
	$args.Add('--read-after-command')
	$args.Add('1.0')

	if ($UseRunning) {
		$args.Add('--use-running')
	}

	if ($KeepServer) {
		$args.Add('--keep-running')
	}

	if (![string]::IsNullOrWhiteSpace($Account)) {
		$args.Add('--account')
		$args.Add($Account)
	}

	if (![string]::IsNullOrWhiteSpace($Password)) {
		$args.Add('--password')
		$args.Add($Password)
	}

	if (![string]::IsNullOrWhiteSpace($Character)) {
		$args.Add('--character')
		$args.Add($Character)
	}

	$output = & python @args 2>&1
	$text = $output -join [Environment]::NewLine

	if (![string]::IsNullOrWhiteSpace($TranscriptFile)) {
		Add-Content -Path $TranscriptFile -Value "===== $PhaseName =====" -Encoding UTF8
		Add-Content -Path $TranscriptFile -Value $text -Encoding UTF8
	}

	if ($LASTEXITCODE -ne 0) {
		throw "mud_session.py failed during phase '$PhaseName'."
	}

	return $text
}

function Get-SpawnedInstanceIds {
	param(
		[Parameter(Mandatory = $true)]
		[string]$Transcript
	)

	$ids = [System.Collections.Generic.List[string]]::new()
	$cleanTranscript = [regex]::Replace($Transcript, "`e\[[0-9;]*[A-Za-z]", '')
	$matches = [regex]::Matches($cleanTranscript, 'Spawned\s+.+?\s+instance\s+#\s*(?<id>[0-9,]+)', 'IgnoreCase')
	foreach ($match in $matches) {
		$ids.Add($match.Groups['id'].Value.Replace(',', ''))
	}

	return $ids.ToArray()
}

function Invoke-Cleanup {
	param(
		[Parameter(Mandatory = $true)]
		[string[]]$InstanceIds,
		[Parameter(Mandatory = $true)]
		[string]$TargetArg,
		[Parameter(Mandatory = $true)]
		[string]$SessionScript,
		[Parameter(Mandatory = $true)]
		[string]$PhaseName
	)

	if ($NoCleanup -or $InstanceIds.Count -eq 0) {
		return
	}

	$commands = [System.Collections.Generic.List[string]]::new()
	$commands.Add('# Cleanup spawned character instances')
	foreach ($id in $InstanceIds) {
		$commands.Add("instance retire $id")
	}

	$commands.Add("instance list $TargetArg")
	$commands.Add("instance audit $TargetArg")
	Invoke-MudCommands -Commands $commands.ToArray() -PhaseName $PhaseName -SessionScript $SessionScript -KeepServer:$KeepRunning | Out-Null
}

Initialize-Scratch

if ([string]::IsNullOrWhiteSpace($OutputFile)) {
	$OutputFile = Join-Path $scratchRoot 'character-instance-smoke.commands.txt'
}

if ([string]::IsNullOrWhiteSpace($TranscriptFile)) {
	$TranscriptFile = Join-Path $scratchRoot 'character-instance-smoke.transcript.txt'
}

$targetArg = Quote-MudArgument -Text $Target
$formArg = if ([string]::IsNullOrWhiteSpace($DormantForm)) { $null } else { Quote-MudArgument -Text $DormantForm }
$manualCommands = New-ManualCommands -TargetArg $targetArg -FormArg $formArg -DestinationCell $MoveCellId
Write-CommandsFile -Commands $manualCommands -Path $OutputFile
Write-Host "Wrote manual smoke-test command file to $OutputFile"

if (!$Run) {
	Write-Host 'Run with -Run and -MudSessionScript <path-to-mud_session.py> to execute automatically.'
	return
}

$sessionScript = Resolve-MudSessionScript
if (Test-Path $TranscriptFile) {
	Remove-Item -LiteralPath $TranscriptFile -Force
}

if (![string]::IsNullOrWhiteSpace($formArg) -and !$UseRunning) {
	throw 'Automated spawn/focus tests require -UseRunning against an already-running MUD. Baseline-only tests can use the helper launch flow.'
}

Invoke-MudCommands -Commands (New-BaselineCommands -TargetArg $targetArg) -PhaseName 'baseline' -SessionScript $sessionScript -KeepServer:$KeepRunning | Out-Null

if ([string]::IsNullOrWhiteSpace($formArg)) {
	Write-Host "Baseline checks completed. Transcript: $TranscriptFile"
	return
}

$passiveTranscript = Invoke-MudCommands -Commands @(
	'# Passive secondary test',
	"instance spawn $targetArg $formArg here temporary passive",
	"instance list $targetArg",
	'look',
	"force $targetArg instances",
	"force $targetArg focus 2"
) -PhaseName 'passive' -SessionScript $sessionScript -KeepServer:$KeepRunning
$passiveIds = Get-SpawnedInstanceIds -Transcript $passiveTranscript
Invoke-Cleanup -InstanceIds $passiveIds -TargetArg $targetArg -SessionScript $sessionScript -PhaseName 'passive-cleanup'

if ($NoCleanup -and $passiveIds.Count -gt 0) {
	Write-Host "Passive instance was left loaded because -NoCleanup was supplied. Skipping focusable reuse of the same form."
	Write-Host "Transcript: $TranscriptFile"
	return
}

$focusableTranscript = Invoke-MudCommands -Commands @(
	'# Focusable secondary test',
	"instance spawn $targetArg $formArg here temporary focusable",
	"instance list $targetArg",
	"force $targetArg instances",
	"force $targetArg focus 2",
	"force $targetArg look",
	"force $targetArg focus primary",
	"force $targetArg instances"
) -PhaseName 'focusable' -SessionScript $sessionScript -KeepServer:$KeepRunning
$focusableIds = Get-SpawnedInstanceIds -Transcript $focusableTranscript

if (![string]::IsNullOrWhiteSpace($MoveCellId) -and $focusableIds.Count -gt 0) {
	Invoke-MudCommands -Commands @(
		'# Staff move check for focusable secondary',
		"instance move $($focusableIds[0]) room $MoveCellId",
		"instance list $targetArg"
	) -PhaseName 'move' -SessionScript $sessionScript -KeepServer:$KeepRunning | Out-Null
}

Invoke-Cleanup -InstanceIds $focusableIds -TargetArg $targetArg -SessionScript $sessionScript -PhaseName 'focusable-cleanup'
Invoke-MudCommands -Commands @(
	'# Final audit',
	"instance audit $targetArg",
	'instance audit all'
) -PhaseName 'final-audit' -SessionScript $sessionScript -KeepServer:$KeepRunning | Out-Null

Write-Host "Character instance smoke test completed. Transcript: $TranscriptFile"
