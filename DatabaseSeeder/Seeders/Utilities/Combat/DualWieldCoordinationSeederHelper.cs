#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using TraitOwnerScope = MudSharp.Body.Traits.TraitOwnerScope;

namespace DatabaseSeeder.Seeders;

public sealed record DualWieldCoordinationBindingResult(bool Success, bool Changed, string Message);

/// <summary>
/// Adds only the dedicated coordination check and missing settings. Existing worlds bind a native
/// character skill explicitly by ID; this helper never grants skills or reconciles combat content.
/// </summary>
public static class DualWieldCoordinationSeederHelper
{
	public const string TemplateName = "Dual Wield Coordination Check";
	public const string ExpressionName = "Dual Wield Coordination Formula";
	public const string EnabledSetting = "DualWieldDoubleAttacksEnabled";

	private static readonly Regex InlineBinding = new(
		@"(?<name>\w+):(?<reference>[0-9]+)(?:{(?<flags>[a-z,]+)})*", RegexOptions.IgnoreCase);

	private static readonly int[] DefaultModifiers = [100, 75, 50, 33, 15, 0, -10, -20, -33, -50, -100];

	private static readonly IReadOnlyDictionary<string, string> Defaults = new Dictionary<string, string>
	{
		["DualWieldDoubleAttackWeightMultiplier"] = "0.25",
		["DualWieldDoubleAttackRecoveryMultiplier"] = "1.5",
		["DualWieldCoordinationDifficulty"] = "5",
		["DualWieldCoordinationMajorPassStages"] = "-1",
		["DualWieldCoordinationPassStages"] = "0",
		["DualWieldCoordinationMinorPassStages"] = "0",
		["DualWieldCoordinationMinorFailStages"] = "1",
		["DualWieldCoordinationFailStages"] = "1",
		["DualWieldCoordinationMajorFailStages"] = "2"
	};

	/// <summary>General skill installers defer binding to CombatSeeder without inventing coordination skill.</summary>
	public static Check EnsureUnboundCheck(FuturemudDatabaseContext context)
	{
		var existing = FindCheck(context);
		if (existing is not null)
		{
			return existing;
		}

		var template = EnsureTemplate(context);
		var check = new Check
		{
			Type = (int)CheckType.DualWieldCoordinationCheck,
			CheckTemplate = template,
			MaximumDifficultyForImprovement = (int)Difficulty.Impossible,
			TraitExpression = new TraitExpression { Name = ExpressionName, Expression = "0" }
		};
		context.Checks.Add(check);
		context.SaveChanges();
		return check;
	}

	/// <summary>
	/// Fresh combat installation enables only after binding. An existing enable/disable setting,
	/// custom formula or custom template always wins over fresh-install defaults.
	/// </summary>
	public static DualWieldCoordinationBindingResult InstallFresh(FuturemudDatabaseContext context, TraitDefinition skill) =>
		BindExistingSkill(context, skill.Id, enableFeature: true);

	/// <summary>
	/// Explicit, additive existing-world binding. A literal zero expression with no parameters is the
	/// loader/installer placeholder; every other existing configuration is preserved, including constants.
	/// enableFeature supplies only a missing setting and never changes an explicit false value.
	/// </summary>
	public static DualWieldCoordinationBindingResult BindExistingSkill(FuturemudDatabaseContext context,
		long traitId, bool enableFeature = false)
	{
		var skill = context.TraitDefinitions.SingleOrDefault(x => x.Id == traitId);
		if (skill is null || skill.Type != 0 || skill.OwnerScope != (int)TraitOwnerScope.Character)
		{
			return new(false, false, $"Trait #{traitId} must be an existing character skill. No configuration changed.");
		}

		var check = FindCheck(context);
		if (check is not null && !IsPlaceholder(check.TraitExpression))
		{
			if (!HasCompatibleBinding(context, check, traitId))
			{
				return new(false, false,
					"Existing Dual Wield coordination formula or template preserved. Review its numeric binding explicitly; no configuration changed.");
			}

			var settingsChanged = EnsureSettings(context, enableFeature);
			context.SaveChanges();
			return new(true, settingsChanged, "Existing Dual Wield coordination binding and custom formula preserved.");
		}

		var existingTemplate = FindTemplate(context);
		if (existingTemplate is not null && !HasRequiredPolicy(existingTemplate))
		{
			return new(false, false,
				$"Existing {TemplateName} template is customised and was preserved. Review its policy explicitly; no configuration changed.");
		}

		var template = EnsureTemplate(context);
		// Never rewrite a placeholder expression: another check may share that expression row.
		var expression = new TraitExpression
		{
			Name = ExpressionName,
			Expression = $"dualwield:{traitId}{{nobranch}}"
		};
		expression.TraitExpressionParameters.Add(new TraitExpressionParameters
		{
			Parameter = "dualwield", TraitDefinition = skill, CanImprove = true, CanBranch = false
		});
		if (check is null)
		{
			check = new Check { Type = (int)CheckType.DualWieldCoordinationCheck };
			context.Checks.Add(check);
		}

		check.CheckTemplate = template;
		check.TraitExpression = expression;
		check.MaximumDifficultyForImprovement = (int)Difficulty.Impossible;
		EnsureSettings(context, enableFeature);
		context.SaveChanges();
		return new(true, true, $"Dual Wield coordination bound to native character skill #{traitId}; existing settings preserved.");
	}

	private static Check? FindCheck(FuturemudDatabaseContext context) => context.Checks
		.Include(x => x.CheckTemplate).ThenInclude(x => x.CheckTemplateDifficulties)
		.Include(x => x.TraitExpression).ThenInclude(x => x.TraitExpressionParameters)
		.SingleOrDefault(x => x.Type == (int)CheckType.DualWieldCoordinationCheck);

	private static bool IsPlaceholder(TraitExpression expression) => expression.Expression.Trim() == "0" &&
		!expression.TraitExpressionParameters.Any();

	private static bool HasRequiredPolicy(CheckTemplate template) =>
		template.CheckMethod.Equals("Standard", StringComparison.OrdinalIgnoreCase) && template.ImproveTraits &&
		template.FailIfTraitMissingMode == (short)FailIfTraitMissingType.FailIfAnyMissing &&
		!template.CanBranchIfTraitMissing &&
		Enum.GetValues<Difficulty>().All(x => template.CheckTemplateDifficulties.Any(y => y.Difficulty == (int)x));

	private static bool HasCompatibleBinding(FuturemudDatabaseContext context, Check check, long traitId)
	{
		if (!HasRequiredPolicy(check.CheckTemplate))
		{
			return false;
		}

		var bindings = check.TraitExpression.TraitExpressionParameters.ToDictionary(x => x.Parameter,
			x => (Id: x.TraitDefinitionId, x.CanImprove, x.CanBranch), StringComparer.OrdinalIgnoreCase);
		foreach (Match match in InlineBinding.Matches(check.TraitExpression.Expression))
		{
			if (!long.TryParse(match.Groups["reference"].Value, out var id))
			{
				return false;
			}

			var flags = match.Groups["flags"].Value.Split(',');
			bindings[match.Groups["name"].Value] = (id,
				!flags.Contains("noimprove", StringComparer.Ordinal),
				!flags.Contains("nobranch", StringComparer.Ordinal));
		}

		var formula = new ExpressionEngine.Expression(InlineBinding.Replace(check.TraitExpression.Expression,
			match => match.Groups["name"].Value));
		if (formula.HasErrors() || formula.ParameterNames.Any(x => !bindings.ContainsKey(x)) ||
			formula.FunctionNames.Any(x => !ExpressionEngine.Expression.IsSupportedFunction(x)))
		{
			return false;
		}

		var improving = bindings.Values.Where(x => x.CanImprove).ToArray();
		return improving.Length == 1 && improving[0].Id == traitId &&
			bindings.Values.All(x => !x.CanBranch) &&
			bindings.Values.Select(x => x.Id).Distinct().Count() == bindings.Count &&
			bindings.Values.All(x => context.TraitDefinitions.Any(y => y.Id == x.Id));
	}

	private static CheckTemplate? FindTemplate(FuturemudDatabaseContext context) => context.CheckTemplates
		.Include(x => x.CheckTemplateDifficulties).SingleOrDefault(x => x.Name == TemplateName);

	private static CheckTemplate EnsureTemplate(FuturemudDatabaseContext context)
	{
		var existing = FindTemplate(context);
		if (existing is not null)
		{
			if (!HasRequiredPolicy(existing))
			{
				throw new InvalidOperationException($"The existing {TemplateName} template is customised; it was not overwritten.");
			}

			return existing;
		}

		var standard = context.CheckTemplates.Include(x => x.CheckTemplateDifficulties)
			.FirstOrDefault(x => x.Name == "Skill Check" || x.Name == "SkillCheck");
		var template = new CheckTemplate
		{
			Name = TemplateName, CheckMethod = "Standard", ImproveTraits = true,
			FailIfTraitMissingMode = (short)FailIfTraitMissingType.FailIfAnyMissing, CanBranchIfTraitMissing = false
		};
		foreach (var difficulty in Enum.GetValues<Difficulty>())
		{
			template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{
				Difficulty = (int)difficulty,
				Modifier = standard?.CheckTemplateDifficulties.SingleOrDefault(x => x.Difficulty == (int)difficulty)?.Modifier ??
					DefaultModifiers[(int)difficulty]
			});
		}

		context.CheckTemplates.Add(template);
		return template;
	}

	private static bool EnsureSettings(FuturemudDatabaseContext context, bool enableFeature)
	{
		var changed = false;
		foreach (var (name, value) in Defaults.Append(new KeyValuePair<string, string>(EnabledSetting,
			enableFeature ? "true" : "false")))
		{
			if (context.StaticConfigurations.Any(x => x.SettingName == name))
			{
				continue;
			}

			context.StaticConfigurations.Add(new StaticConfiguration { SettingName = name, Definition = value });
			changed = true;
		}

		return changed;
	}
}
