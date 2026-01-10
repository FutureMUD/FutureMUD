using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Arena;

internal class IsInArenaBattle : BuiltInFunction
{
	private IsInArenaBattle(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		var characterFunction = ParameterFunctions.ElementAt(0);
		if (characterFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "Character Function in IsInArenaBattle Function returned an error: " + characterFunction.ErrorMessage;
			return StatementResult.Error;
		}

		var character = (ICharacter)characterFunction.Result?.GetObject;
		if (character is null)
		{
			ErrorMessage = "Character Function in IsInArenaBattle Function did not return a valid character.";
			return StatementResult.Error;
		}

		Result = new BooleanVariable(character.EffectsOfType<ArenaParticipationEffect>().Any());
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"isinarenabattle",
				[ProgVariableTypes.Character],
				(pars, gameworld) => new IsInArenaBattle(pars),
				["character"],
				["The character to check for arena battle participation."],
				"The function returns true if the given character is currently participating in an arena battle.",
				"Arena",
				ProgVariableTypes.Boolean				
			)
		);
	}
}
