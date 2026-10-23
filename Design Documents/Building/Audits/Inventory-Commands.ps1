# Static syntax inventory only. Uses the Roslyn parser bundled with PowerShell 7.
$ErrorActionPreference = 'Stop'
$auditRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$auditRows = [Collections.Generic.List[object]]::new()
$auditFiles = [Collections.Generic.List[object]]::new()
$auditHandlers = [Collections.Generic.List[object]]::new()
$auditModuleFiles = 0

foreach ($auditPath in (Get-ChildItem -LiteralPath (Join-Path $auditRoot 'MudSharpCore') -Recurse -File -Filter '*.cs' | Sort-Object FullName)) {
	$auditRelative = [IO.Path]::GetRelativePath($auditRoot, $auditPath.FullName).Replace('\', '/')
	$auditInCommands = $auditRelative.StartsWith('MudSharpCore/Commands/')
	$auditSource = [IO.File]::ReadAllText($auditPath.FullName)
	if (-not $auditInCommands -and -not $auditSource.Contains('BuildingCommand')) { continue }
	$auditTree = [Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree]::ParseText($auditSource)
	$auditCount = 0
	foreach ($auditMethod in $auditTree.GetRoot().DescendantNodes()) {
		if ($auditMethod -isnot [Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax]) { continue }
		if ($auditMethod.Identifier.Text -eq 'BuildingCommand' -and $auditMethod.ReturnType.ToString() -eq 'bool') {
			$auditHandlers.Add([ordered]@{ file = $auditRelative; line = $auditMethod.GetLocation().GetLineSpan().StartLinePosition.Line + 1 })
		}
		if (-not $auditInCommands) { continue }
		$auditAttributes = @($auditMethod.AttributeLists.Attributes | Where-Object { $null -ne $_ })
		$auditPlayer = $auditAttributes | Where-Object { $_.Name.ToString() -eq 'PlayerCommand' } | Select-Object -First 1
		if ($null -eq $auditPlayer) { continue }
		$auditNames = @($auditPlayer.ArgumentList.Arguments | ForEach-Object { $_.Expression.Token.ValueText })
		$auditPermission = $auditAttributes | Where-Object { $_.Name.ToString() -eq 'CommandPermission' } | Select-Object -First 1
		$auditHelp = $auditAttributes | Where-Object { $_.Name.ToString() -eq 'HelpInfo' } | Select-Object -First 1
		$auditRows.Add([ordered]@{
			file = $auditRelative
			line = $auditPlayer.GetLocation().GetLineSpan().StartLinePosition.Line + 1
			method = $auditMethod.Identifier.Text
			registration_name = $auditNames[0]
			command_words = @($auditNames | Select-Object -Skip 1)
			permission_attribute = if ($auditPermission) { $auditPermission.ArgumentList.Arguments[0].Expression.ToString().Replace('PermissionLevel.', '') } else { $null }
			help_attribute = $null -ne $auditHelp
			help_topic = if ($auditHelp) { $auditHelp.ArgumentList.Arguments[0].Expression.Token.ValueText } else { $null }
			auto_help = if ($auditHelp) { ($auditHelp.ArgumentList.Arguments | Where-Object { $_.Expression.ToString().StartsWith('AutoHelp.') } | Select-Object -First 1).Expression.ToString().Replace('AutoHelp.', '') } else { $null }
		})
		$auditCount++
	}
	if ($auditInCommands) {
		$auditFiles.Add([ordered]@{ file = $auditRelative; source_lines = $auditSource.Split("`n").Count; command_declarations = $auditCount })
		if ($auditRelative.Contains('/Modules/')) { $auditModuleFiles++ }
	}
}

$auditSummary = [ordered]@{
	command_cs_files = $auditFiles.Count
	module_cs_files = $auditModuleFiles
	source_command_declarations = $auditRows.Count
	declared_command_words = ($auditRows | ForEach-Object { $_.command_words.Count } | Measure-Object -Sum).Sum
	declarations_with_explicit_staff_permission = @($auditRows | Where-Object { $_.permission_attribute -in @('JuniorAdmin', 'Admin', 'SeniorAdmin', 'HighAdmin', 'Founder', 'Guide') }).Count
	declarations_without_help = @($auditRows | Where-Object { -not $_.help_attribute }).Count
	building_command_definitions = $auditHandlers.Count
	building_command_files = @($auditHandlers.file | Sort-Object -Unique).Count
}
$auditData = [ordered]@{
	audit_date = '2026-10-23'
	revision = (git -C $auditRoot rev-parse HEAD).Trim()
	method = 'Roslyn C# syntax inventory of Commands and bool methods named BuildingCommand in MudSharpCore. Counts are source declarations, not unique runtime commands or behavioral certification. Uses default parser preprocessor symbols; does not compile or execute game code.'
	summary = $auditSummary
	commands = @($auditRows.ToArray())
	files = @($auditFiles.ToArray())
	building_command_definitions = @($auditHandlers.ToArray())
}
$auditData | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $PSScriptRoot 'Admin_Building_Command_Inventory_2026-10-23.json') -Encoding utf8
$auditSummary | ConvertTo-Json
$auditRows | Where-Object { -not $_.help_attribute } | ConvertTo-Json -Depth 4
