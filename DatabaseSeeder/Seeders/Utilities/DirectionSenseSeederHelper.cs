#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using TraitDefinition = MudSharp.Models.TraitDefinition;
using TraitExpression = MudSharp.Models.TraitExpression;

namespace DatabaseSeeder.Seeders;

public sealed record DirectionSenseBindingResult(bool Success, bool Changed, string Message);

/// <summary>
/// Early check-binding contract for the later Armageddon skill package. Never creates/grants a
/// skill, guesses by display name, activates a world, or changes existing character trait values.
/// </summary>
public static class DirectionSenseSeederHelper
{
	public const string TraitKey = "arm.skill.direction_sense";
	public const string InitialCheckKey = "arm.check.direction_sense.initial";
	public const string RecoveryCheckKey = "arm.check.direction_sense.recovery";
	public static IReadOnlyDictionary<string, CheckType> CheckBindings { get; } =
		new Dictionary<string, CheckType>
		{
			[InitialCheckKey] = CheckType.DirectionSenseCheck,
			[RecoveryCheckKey] = CheckType.DirectionSenseRecoveryCheck
		};

	private static readonly int[] DefaultModifiers = [100, 75, 50, 33, 15, 0, -10, -20, -33, -50, -100];
	private static string TemplateName(bool recovery) => recovery ? "Direction Sense Recovery Check" : "Direction Sense Check";
	private static string ExpressionName(bool recovery) => recovery ? RecoveryCheckKey : InitialCheckKey;
	private static CheckType Type(bool recovery) => recovery ? CheckType.DirectionSenseRecoveryCheck : CheckType.DirectionSenseCheck;

	/// <summary>Skill installers reserve both checks without claiming a stock or magic skill.</summary>
	public static void EnsureUnboundChecks(FuturemudDatabaseContext context)
	{
		foreach (var recovery in new[] { false, true })
		{
			if (FindCheck(context, recovery) is not null) continue;
			context.Checks.Add(new Check
			{
				Type = (int)Type(recovery),
				CheckTemplate = EnsureTemplate(context, recovery),
				TraitExpression = new TraitExpression { Name = ExpressionName(recovery), Expression = "0" },
				MaximumDifficultyForImprovement = (int)Difficulty.Impossible
			});
		}

		context.SaveChanges();
	}

	/// <summary>
	/// The installer must preflight its stable owned-trait manifest before calling this method.
	/// Existing compatible numeric bindings and custom difficulty modifiers retain their IDs.
	/// Conflicting checks/templates cause refusal before any write.
	/// </summary>
	public static DirectionSenseBindingResult BindOwnedTrait(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, long> ownedTraits)
	{
		if (!ownedTraits.TryGetValue(TraitKey, out var traitId))
			return new(false, false, $"Preflight must resolve {TraitKey} to an existing trait ID.");
		var skill = context.TraitDefinitions.SingleOrDefault(x => x.Id == traitId);
		if (skill is null || skill.Type != 0 || skill.OwnerScope != (int)TraitOwnerScope.Character)
			return new(false, false, $"{TraitKey} must resolve to an existing character skill. No configuration changed.");

		foreach (var recovery in new[] { false, true })
		{
			var check = FindCheck(context, recovery);
			var template = FindTemplate(context, recovery);
			if (template is not null && !HasPolicy(template, recovery) ||
			    check is not null && !IsPlaceholder(check) && !IsCompatible(check, skill.Id, recovery))
				return new(false, false, $"Existing {ExpressionName(recovery)} configuration conflicts; preserved without changes.");
		}

		using var transaction = context.Database.IsRelational() && context.Database.CurrentTransaction is null
			? context.Database.BeginTransaction() : null;
		var changed = false;
		var createdParameters = new List<(TraitExpressionParameters Parameter, bool Improve)>();
		foreach (var recovery in new[] { false, true })
		{
			var check = FindCheck(context, recovery);
			if (check is not null && !IsPlaceholder(check)) continue;
			if (check is null)
			{
				check = new Check { Type = (int)Type(recovery) };
				context.Checks.Add(check);
			}

			// Allocate a new expression instead of changing a possibly shared placeholder row.
			var expression = new TraitExpression
			{
				Name = ExpressionName(recovery),
				Expression = $"directionsense:{skill.Id}{{nobranch{(recovery ? ",noimprove" : "")}}}"
			};
			var parameter = new TraitExpressionParameters
			{
				Parameter = "directionsense", TraitDefinition = skill, CanImprove = !recovery, CanBranch = false
			};
			expression.TraitExpressionParameters.Add(parameter);
			createdParameters.Add((parameter, !recovery));
			check.TraitExpression = expression;
			check.CheckTemplate = EnsureTemplate(context, recovery);
			check.MaximumDifficultyForImprovement = (int)Difficulty.Impossible;
			changed = true;
		}

		context.SaveChanges();
		// Legacy bit columns default to true; EF may omit false values during insert.
		// Persist explicit policy on our new rows after generated defaults have been read.
		foreach (var (parameter, improve) in createdParameters)
		{
			parameter.CanBranch = false;
			parameter.CanImprove = improve;
			context.Entry(parameter).Property(x => x.CanBranch).IsModified = true;
			context.Entry(parameter).Property(x => x.CanImprove).IsModified = true;
		}
		if (createdParameters.Count > 0) context.SaveChanges();
		transaction?.Commit();
		return new(true, changed, $"Both Direction Sense checks bind {TraitKey} to character skill #{traitId}; world activation unchanged.");
	}

	private static bool IsPlaceholder(Check check) => check.TraitExpression.Expression.Trim() == "0" &&
		!check.TraitExpression.TraitExpressionParameters.Any();

	private static bool IsCompatible(Check check, long traitId, bool recovery)
	{
		if (!HasPolicy(check.CheckTemplate, recovery)) return false;
		var parameters = check.TraitExpression.TraitExpressionParameters.ToArray();
		// Only accept our exact binding. Builder-authored formulas are preserved for explicit review.
		return parameters.Length == 1 && parameters[0].TraitDefinitionId == traitId &&
		       parameters[0].CanImprove == !recovery && !parameters[0].CanBranch &&
		       check.TraitExpression.Expression == $"directionsense:{traitId}{{nobranch{(recovery ? ",noimprove" : "")}}}";
	}

	private static bool HasPolicy(CheckTemplate template, bool recovery) =>
		template.CheckMethod.Equals("Standard", StringComparison.OrdinalIgnoreCase) &&
		template.ImproveTraits == !recovery && !template.CanBranchIfTraitMissing &&
		template.FailIfTraitMissingMode == (short)FailIfTraitMissingType.FailIfAnyMissing &&
		Enum.GetValues<Difficulty>().All(x => template.CheckTemplateDifficulties.Any(y => y.Difficulty == (int)x));

	private static Check? FindCheck(FuturemudDatabaseContext context, bool recovery) => context.Checks
		.Include(x => x.CheckTemplate).ThenInclude(x => x.CheckTemplateDifficulties)
		.Include(x => x.TraitExpression).ThenInclude(x => x.TraitExpressionParameters)
		.SingleOrDefault(x => x.Type == (int)Type(recovery));

	private static CheckTemplate? FindTemplate(FuturemudDatabaseContext context, bool recovery) =>
		context.CheckTemplates.Local.SingleOrDefault(x => x.Name == TemplateName(recovery)) ?? context.CheckTemplates
			.Include(x => x.CheckTemplateDifficulties).SingleOrDefault(x => x.Name == TemplateName(recovery));

	private static CheckTemplate EnsureTemplate(FuturemudDatabaseContext context, bool recovery)
	{
		var template = FindTemplate(context, recovery);
		if (template is not null)
		{
			if (!HasPolicy(template, recovery))
				throw new InvalidOperationException($"Custom {TemplateName(recovery)} policy must be reviewed; it was not overwritten.");
			return template;
		}

		var sourceName = recovery ? "Skill Check No Improvement" : "Skill Check";
		var standard = context.CheckTemplates.Include(x => x.CheckTemplateDifficulties)
			.FirstOrDefault(x => x.Name == sourceName);
		template = new CheckTemplate
		{
			Name = TemplateName(recovery), CheckMethod = "Standard", ImproveTraits = !recovery,
			CanBranchIfTraitMissing = false, FailIfTraitMissingMode = (short)FailIfTraitMissingType.FailIfAnyMissing
		};
		foreach (var difficulty in Enum.GetValues<Difficulty>())
			template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty
			{
				Difficulty = (int)difficulty,
				Modifier = standard?.CheckTemplateDifficulties.SingleOrDefault(x => x.Difficulty == (int)difficulty)?.Modifier ??
				           DefaultModifiers[(int)difficulty]
			});
		context.CheckTemplates.Add(template);
		return template;
	}
}
