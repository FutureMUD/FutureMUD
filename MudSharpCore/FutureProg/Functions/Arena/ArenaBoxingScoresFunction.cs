#nullable enable

using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.Arena;

internal class ArenaBoxingScoresFunction : BuiltInFunction
{
	public ArenaBoxingScoresFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Number | ProgVariableTypes.Collection;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		List<int> participantSides = GetNumberCollection(ParameterFunctions[0].Result?.GetObject);
		List<int> attackerSides = GetNumberCollection(ParameterFunctions[1].Result?.GetObject);
		List<int> landedHits = GetNumberCollection(ParameterFunctions[2].Result?.GetObject);
		List<int> undefendedHits = GetNumberCollection(ParameterFunctions[3].Result?.GetObject);
		List<string> impactLocations = GetTextCollection(ParameterFunctions[4].Result?.GetObject);

		List<decimal> scores = CalculateScores(participantSides, attackerSides, landedHits, undefendedHits, impactLocations);
		Result = new CollectionVariable(scores.Select(x => (IProgVariable)new NumberVariable(x)).ToList(), ProgVariableTypes.Number);
		return StatementResult.Normal;
	}

	internal static List<decimal> CalculateScores(IReadOnlyCollection<int> participantSides,
		IReadOnlyList<int> attackerSides, IReadOnlyList<int> landedHits, IReadOnlyList<int> undefendedHits,
		IReadOnlyList<string> impactLocations)
	{
		List<int> orderedSides = participantSides
			.Distinct()
			.OrderBy(x => x)
			.ToList();
		if (orderedSides.Count == 0)
		{
			orderedSides = attackerSides
				.Distinct()
				.OrderBy(x => x)
				.ToList();
		}

		Dictionary<int, decimal> scoreLookup = orderedSides.ToDictionary(x => x, _ => 0.0m);
		int rowCount = new[] { attackerSides.Count, landedHits.Count, undefendedHits.Count, impactLocations.Count }.Min();
		for (int i = 0; i < rowCount; i++)
		{
			if (landedHits[i] <= 0 || undefendedHits[i] <= 0 || !impactLocations[i].EqualToAny("head", "torso"))
			{
				continue;
			}

			if (!scoreLookup.ContainsKey(attackerSides[i]))
			{
				scoreLookup[attackerSides[i]] = 0.0m;
			}

			scoreLookup[attackerSides[i]] += 1.0m;
		}

		return scoreLookup
			.OrderBy(x => x.Key)
			.Select(x => x.Value)
			.ToList();
	}

	private static List<int> GetNumberCollection(object? value)
	{
		return value is IList list
			? list.OfType<IProgVariable>().Select(x => System.Convert.ToInt32(x.GetObject)).ToList()
			: [];
	}

	private static List<string> GetTextCollection(object? value)
	{
		return value is IList list
			? list.OfType<IProgVariable>().Select(x => x.GetObject?.ToString() ?? string.Empty).ToList()
			: [];
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"arenaboxingscores",
				[
					ProgVariableTypes.Number | ProgVariableTypes.Collection,
					ProgVariableTypes.Number | ProgVariableTypes.Collection,
					ProgVariableTypes.Number | ProgVariableTypes.Collection,
					ProgVariableTypes.Number | ProgVariableTypes.Collection,
					ProgVariableTypes.Text | ProgVariableTypes.Collection
				],
				(pars, _) => new ArenaBoxingScoresFunction(pars),
				["participantSides", "attackerSides", "landedHits", "undefendedHits", "impactLocations"],
				[
					"The ordered side indices for all event participants.",
					"The side index credited for each scoring snapshot.",
					"A numeric landed-hit flag for each scoring snapshot.",
					"A numeric undefended-hit flag for each scoring snapshot.",
					"The normalised impact location for each scoring snapshot."
				],
				"Returns one boxing point total per side, counting only landed undefended hits to the head or torso.",
				"Arena",
				ProgVariableTypes.Number | ProgVariableTypes.Collection
			)
		);
	}
}
