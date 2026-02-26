#nullable enable
using System.Collections.Generic;
using System.Linq;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Arena;

internal class ArenaRatingFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private ArenaRatingFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Number;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result?.GetObject is not ICharacter character)
		{
			ErrorMessage = "Character parameter in arenarating did not resolve to a character.";
			return StatementResult.Error;
		}

		var combatantClass = ResolveCombatantClass(ParameterFunctions[1].Result?.GetObject);
		if (combatantClass is null)
		{
			ErrorMessage = "Combatant class parameter in arenarating did not match any arena combatant class.";
			return StatementResult.Error;
		}

		Result = new NumberVariable(_gameworld.ArenaRatingsService.GetRating(character, combatantClass));
		return StatementResult.Normal;
	}

	private ICombatantClass? ResolveCombatantClass(object? input)
	{
		switch (input)
		{
			case decimal numeric:
				return ResolveCombatantClassById((long)numeric);
			case int numeric:
				return ResolveCombatantClassById(numeric);
			case long numeric:
				return ResolveCombatantClassById(numeric);
			case string text:
				return ResolveCombatantClassByText(text);
			default:
				return null;
		}
	}

	private ICombatantClass? ResolveCombatantClassById(long id)
	{
		return _gameworld.CombatArenas
			.SelectMany(x => x.CombatantClasses)
			.FirstOrDefault(x => x.Id == id);
	}

	private ICombatantClass? ResolveCombatantClassByText(string text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return null;
		}

		if (long.TryParse(text, out var idMatch))
		{
			return ResolveCombatantClassById(idMatch);
		}

		var classes = _gameworld.CombatArenas
			.SelectMany(x => x.CombatantClasses)
			.DistinctBy(x => x.Id)
			.ToList();
		return classes.FirstOrDefault(x => x.Name.EqualTo(text)) ??
		       classes.FirstOrDefault(x => x.Name.StartsWith(text, System.StringComparison.InvariantCultureIgnoreCase));
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"arenarating",
				[ProgVariableTypes.Character, ProgVariableTypes.Number],
				(pars, gameworld) => new ArenaRatingFunction(pars, gameworld),
				["character", "combatantClassId"],
				["The character whose arena rating you want to inspect.", "The numeric ID of an arena combatant class."],
				"Returns the arena rating for a character in a specific combatant class.",
				"Arena",
				ProgVariableTypes.Number
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"arenarating",
				[ProgVariableTypes.Character, ProgVariableTypes.Text],
				(pars, gameworld) => new ArenaRatingFunction(pars, gameworld),
				["character", "combatantClass"],
				["The character whose arena rating you want to inspect.", "The name or ID text of an arena combatant class."],
				"Returns the arena rating for a character in a specific combatant class.",
				"Arena",
				ProgVariableTypes.Number
			)
		);
	}
}
