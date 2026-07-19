#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DatabaseSeeder.Seeders;

internal enum ChargenFreeKnowledgeProgReconcileStatus
{
	Missing,
	Unchanged,
	Updated,
	Unsafe
}

internal readonly record struct ChargenFreeKnowledgeProgReconcileResult(
	ChargenFreeKnowledgeProgReconcileStatus Status,
	string Message);

internal static partial class ChargenFreeKnowledgeProgReconciler
{
	internal const string ProgName = "ChargenFreeKnowledges";
	internal const string HealthStartMarker = "// <FutureMUD Seeder: Health Free Knowledges>";
	internal const string HealthEndMarker = "// </FutureMUD Seeder: Health Free Knowledges>";
	internal const string CultureStartMarker = "// <FutureMUD Seeder: Culture Free Knowledges>";
	internal const string CultureEndMarker = "// </FutureMUD Seeder: Culture Free Knowledges>";

	private static readonly HashSet<string> HealthAcquisitionProgNames = new(StringComparer.OrdinalIgnoreCase)
	{
		"HealthCanPickBroadMedicalKnowledgeAtChargen",
		"HealthCanPickSurgicalKnowledgeAtChargen",
		"HealthCanPickDiagnosticKnowledgeAtChargen",
		"HealthCanPickCareKnowledgeAtChargen"
	};

	private sealed record GrantRule(string KnowledgeName, string AcquisitionProgName, bool RequiresLiteracy);

	internal static ChargenFreeKnowledgeProgReconcileResult Reconcile(FuturemudDatabaseContext context)
	{
		FutureProg? target = FindTargetProg(context);
		if (target is null)
		{
			return new ChargenFreeKnowledgeProgReconcileResult(
				ChargenFreeKnowledgeProgReconcileStatus.Missing,
				"Character creation has not been seeded; its free-knowledge integration will be reconciled when the Character Creation seeder runs.");
		}

		if (!TryValidateTargetContract(target, out FutureProgsParameter? characterParameter, out string contractError))
		{
			return Unsafe(contractError);
		}

		List<string> issues = [];
		List<GrantRule> healthRules = GetHealthRules(context, issues);
		List<GrantRule> cultureRules = GetCultureRules(context, issues);
		if (!TryBuildReconciledText(target.FunctionText, healthRules, cultureRules, out string reconciledText,
			    out string textError))
		{
			return Unsafe(textError);
		}

		bool parameterNeedsUpgrade =
			ProgVariableTypes.FromStorageString(characterParameter!.ParameterTypeDefinition) != ProgVariableTypes.Chargen;
		bool textChanged = !string.Equals(target.FunctionText, reconciledText, StringComparison.Ordinal);
		if (parameterNeedsUpgrade)
		{
			characterParameter.ParameterTypeDefinition = ProgVariableTypes.Chargen.ToStorageString();
		}

		if (textChanged)
		{
			target.FunctionText = reconciledText;
		}

		string issueText = issues.Count > 0
			? $" Skipped {issues.Count:N0} invalid stock acquisition {(issues.Count == 1 ? "rule" : "rules")}: {string.Join("; ", issues)}"
			: string.Empty;
		if (!parameterNeedsUpgrade && !textChanged)
		{
			return new ChargenFreeKnowledgeProgReconcileResult(
				ChargenFreeKnowledgeProgReconcileStatus.Unchanged,
				issueText.Trim());
		}

		return new ChargenFreeKnowledgeProgReconcileResult(
			ChargenFreeKnowledgeProgReconcileStatus.Updated,
			$"Updated {ProgName} with the installed stock health and script knowledge grants.{issueText}");
	}

	internal static bool HasRepairableHealthDrift(FuturemudDatabaseContext context)
	{
		return HasRepairableDrift(context, health: true);
	}

	internal static bool HasRepairableCultureDrift(FuturemudDatabaseContext context)
	{
		return HasRepairableDrift(context, health: false);
	}

	private static bool HasRepairableDrift(FuturemudDatabaseContext context, bool health)
	{
		FutureProg? target = FindTargetProg(context);
		if (target is null || !TryValidateTargetContract(target, out FutureProgsParameter? characterParameter, out _))
		{
			return false;
		}

		List<string> issues = [];
		List<GrantRule> healthRules = GetHealthRules(context, issues);
		List<GrantRule> cultureRules = GetCultureRules(context, issues);
		List<GrantRule> targetRules = health ? healthRules : cultureRules;
		if (targetRules.Count == 0)
		{
			return false;
		}

		if (!TryBuildReconciledText(target.FunctionText, healthRules, cultureRules, out string reconciledText, out _))
		{
			return false;
		}

		string startMarker = health ? HealthStartMarker : CultureStartMarker;
		string endMarker = health ? HealthEndMarker : CultureEndMarker;
		string? existingBlock = TryGetManagedBlock(target.FunctionText, startMarker, endMarker);
		string? desiredBlock = TryGetManagedBlock(reconciledText, startMarker, endMarker);
		bool parameterNeedsUpgrade =
			ProgVariableTypes.FromStorageString(characterParameter!.ParameterTypeDefinition) != ProgVariableTypes.Chargen;
		return parameterNeedsUpgrade || !string.Equals(existingBlock, desiredBlock, StringComparison.Ordinal);
	}

	private static ChargenFreeKnowledgeProgReconcileResult Unsafe(string reason)
	{
		return new ChargenFreeKnowledgeProgReconcileResult(
			ChargenFreeKnowledgeProgReconcileStatus.Unsafe,
			$"Warning: {ProgName} was not changed because {reason}");
	}

	private static FutureProg? FindTargetProg(FuturemudDatabaseContext context)
	{
		FutureProg? persisted = context.FutureProgs
			.Include(x => x.FutureProgsParameters)
			.AsEnumerable()
			.FirstOrDefault(x => x.FunctionName.Equals(ProgName, StringComparison.OrdinalIgnoreCase));
		return persisted ?? context.FutureProgs.Local
			.FirstOrDefault(x => x.FunctionName.Equals(ProgName, StringComparison.OrdinalIgnoreCase));
	}

	private static bool TryValidateTargetContract(FutureProg target, out FutureProgsParameter? characterParameter,
		out string error)
	{
		characterParameter = null;
		ProgVariableTypes returnType = ProgVariableTypes.FromStorageString(target.ReturnTypeDefinition);
		if (returnType != (ProgVariableTypes.Knowledge | ProgVariableTypes.Collection))
		{
			error = $"it does not return a collection of knowledges (found {returnType}).";
			return false;
		}

		if (target.AcceptsAnyParameters)
		{
			error = "it accepts arbitrary parameters instead of the stock character-creation input.";
			return false;
		}

		FutureProgsParameter[] parameters = target.FutureProgsParameters
			.OrderBy(x => x.ParameterIndex)
			.ToArray();
		if (parameters.Length != 1 || parameters[0].ParameterIndex != 0 ||
		    !string.Equals(parameters[0].ParameterName, "ch", StringComparison.OrdinalIgnoreCase))
		{
			error = "it does not have the stock single parameter named ch.";
			return false;
		}

		ProgVariableTypes parameterType = ProgVariableTypes.FromStorageString(parameters[0].ParameterTypeDefinition);
		if (parameterType != ProgVariableTypes.Toon && parameterType != ProgVariableTypes.Chargen)
		{
			error = $"its ch parameter is {parameterType.Describe()} rather than Toon or Chargen.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(target.FunctionText))
		{
			error = "its function text is empty.";
			return false;
		}

		characterParameter = parameters[0];
		error = string.Empty;
		return true;
	}

	private static List<GrantRule> GetHealthRules(FuturemudDatabaseContext context, ICollection<string> issues)
	{
		return context.Knowledges
			.Include(x => x.CanAcquireProg)
				.ThenInclude(x => x.FutureProgsParameters)
			.AsEnumerable()
			.Where(x => x.CanAcquireProg is not null &&
			            HealthAcquisitionProgNames.Contains(x.CanAcquireProg.FunctionName))
			.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
			.GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
			.Select(x => x.First())
			.Where(x => ValidateAcquisitionProg(x.Name, x.CanAcquireProg!, issues))
			.Select(x => new GrantRule(x.Name, x.CanAcquireProg!.FunctionName, false))
			.ToList();
	}

	private static List<GrantRule> GetCultureRules(FuturemudDatabaseContext context, ICollection<string> issues)
	{
		return context.Scripts
			.Include(x => x.Knowledge)
				.ThenInclude(x => x.CanAcquireProg)
					.ThenInclude(x => x.FutureProgsParameters)
			.AsEnumerable()
			.Where(x => x.Knowledge?.CanAcquireProg is not null &&
			            string.Equals(x.Knowledge.Type, "Script", StringComparison.OrdinalIgnoreCase) &&
			            string.Equals(x.Knowledge.CanAcquireProg.FunctionName,
				            ExpectedScriptAcquisitionProgName(x.Name), StringComparison.OrdinalIgnoreCase))
			.OrderBy(x => x.Knowledge.Name, StringComparer.OrdinalIgnoreCase)
			.GroupBy(x => x.Knowledge.Name, StringComparer.OrdinalIgnoreCase)
			.Select(x => x.First())
			.Where(x => ValidateAcquisitionProg(x.Knowledge.Name, x.Knowledge.CanAcquireProg!, issues))
			.Select(x => new GrantRule(x.Knowledge.Name, x.Knowledge.CanAcquireProg!.FunctionName, true))
			.ToList();
	}

	private static bool ValidateAcquisitionProg(string knowledgeName, FutureProg prog, ICollection<string> issues)
	{
		FutureProgsParameter[] parameters = prog.FutureProgsParameters
			.OrderBy(x => x.ParameterIndex)
			.ToArray();
		bool valid = ProgVariableTypes.FromStorageString(prog.ReturnTypeDefinition) == ProgVariableTypes.Boolean &&
		             !prog.AcceptsAnyParameters &&
		             parameters.Length == 2 && parameters[0].ParameterIndex == 0 &&
		             parameters[1].ParameterIndex == 1 &&
		             ProgVariableTypes.Chargen.CompatibleWith(
			             ProgVariableTypes.FromStorageString(parameters[0].ParameterTypeDefinition)) &&
		             ProgVariableTypes.Trait.CompatibleWith(
			             ProgVariableTypes.FromStorageString(parameters[1].ParameterTypeDefinition)) &&
		             UserDefinedFunctionNameRegex().IsMatch(prog.FunctionName);
		if (valid)
		{
			return true;
		}

		issues.Add($"{knowledgeName} ({prog.FunctionName})");
		return false;
	}

	private static string ExpectedScriptAcquisitionProgName(string scriptName)
	{
		return $"CanPick{scriptName.Replace("-", string.Empty).Replace(" ", string.Empty)}ScriptKnowledge";
	}

	private static bool TryBuildReconciledText(string functionText, IReadOnlyCollection<GrantRule> healthRules,
		IReadOnlyCollection<GrantRule> cultureRules, out string reconciledText, out string error)
	{
		string newline = functionText.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
		if (!TryStripManagedBlock(functionText, HealthStartMarker, HealthEndMarker, out string withoutHealth,
			    out error) ||
		    !TryStripManagedBlock(withoutHealth, CultureStartMarker, CultureEndMarker, out string stripped,
			    out error))
		{
			reconciledText = functionText;
			return false;
		}

		MatchCollection anchors = ReturnKnowledgesRegex().Matches(stripped);
		if (anchors.Count == 0)
		{
			reconciledText = functionText;
			error = "it has no final standalone 'return @knowledges' statement.";
			return false;
		}

		Match anchor = anchors[^1];
		if (!string.IsNullOrWhiteSpace(stripped[(anchor.Index + anchor.Length)..]))
		{
			reconciledText = functionText;
			error = "its final standalone 'return @knowledges' statement is followed by other content.";
			return false;
		}

		List<string> blocks = [];
		if (healthRules.Count > 0)
		{
			blocks.Add(BuildBlock(HealthStartMarker, HealthEndMarker, healthRules, newline));
		}

		if (cultureRules.Count > 0)
		{
			blocks.Add(BuildBlock(CultureStartMarker, CultureEndMarker, cultureRules, newline));
		}

		string prefix = stripped[..anchor.Index].TrimEnd(' ', '\t', '\r', '\n');
		string suffix = stripped[anchor.Index..].TrimStart(' ', '\t', '\r', '\n');
		reconciledText = blocks.Count > 0
			? $"{prefix}{newline}{newline}{string.Join(newline + newline, blocks)}{newline}{newline}{suffix}"
			: $"{prefix}{newline}{newline}{suffix}";
		error = string.Empty;
		return true;
	}

	private static string BuildBlock(string startMarker, string endMarker, IEnumerable<GrantRule> rules,
		string newline)
	{
		StringBuilder builder = new();
		builder.AppendLine(startMarker);
		foreach (GrantRule rule in rules)
		{
			string acquisitionCheck = $"@{rule.AcquisitionProgName}(@ch, @skill)";
			string condition = rule.RequiresLiteracy
				? $"@ch.Skills.Any(skill, @skill.Name == \"Literacy\") and @ch.Skills.Any(skill, {acquisitionCheck})"
				: $"@ch.Skills.Any(skill, {acquisitionCheck})";
			builder.Append("if (").Append(condition).AppendLine(")");
			builder.Append("  additem knowledges ToKnowledge(\"")
				.Append(EscapeFutureProgString(rule.KnowledgeName))
				.AppendLine("\")");
			builder.AppendLine("end if");
		}

		builder.Append(endMarker);
		return builder.ToString().Replace("\r\n", "\n").Replace("\n", newline);
	}

	private static string EscapeFutureProgString(string text)
	{
		return text.Replace("\\", "\\\\").Replace("\"", "\\\"");
	}

	private static bool TryStripManagedBlock(string text, string startMarker, string endMarker, out string stripped,
		out string error)
	{
		Regex startRegex = MarkerRegex(startMarker);
		Regex endRegex = MarkerRegex(endMarker);
		MatchCollection starts = startRegex.Matches(text);
		MatchCollection ends = endRegex.Matches(text);
		if (starts.Count == 0 && ends.Count == 0)
		{
			stripped = text;
			error = string.Empty;
			return true;
		}

		if (starts.Count != 1 || ends.Count != 1 || ends[0].Index <= starts[0].Index)
		{
			stripped = text;
			error = $"the managed marker pair beginning '{startMarker}' is missing, duplicated, or out of order.";
			return false;
		}

		int removalEnd = ends[0].Index + ends[0].Length;
		if (removalEnd < text.Length && text[removalEnd] == '\r')
		{
			removalEnd++;
		}

		if (removalEnd < text.Length && text[removalEnd] == '\n')
		{
			removalEnd++;
		}

		stripped = text.Remove(starts[0].Index, removalEnd - starts[0].Index);
		error = string.Empty;
		return true;
	}

	private static string? TryGetManagedBlock(string text, string startMarker, string endMarker)
	{
		Match start = MarkerRegex(startMarker).Match(text);
		Match end = MarkerRegex(endMarker).Match(text);
		if (!start.Success || !end.Success || end.Index <= start.Index)
		{
			return null;
		}

		return text[start.Index..(end.Index + end.Length)];
	}

	private static Regex MarkerRegex(string marker)
	{
		return new Regex($"^[\\t ]*{Regex.Escape(marker)}[\\t ]*$",
			RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);
	}

	[GeneratedRegex("^[A-Za-z][A-Za-z0-9_]*$", RegexOptions.CultureInvariant)]
	private static partial Regex UserDefinedFunctionNameRegex();

	[GeneratedRegex("^[\\t ]*return[\\t ]+@knowledges[\\t ]*$",
		RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant)]
	private static partial Regex ReturnKnowledgesRegex();
}
