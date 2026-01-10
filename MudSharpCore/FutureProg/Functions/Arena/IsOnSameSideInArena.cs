using System.Collections.Generic;
using System.Linq;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Arena;

internal class IsOnSameSideInArena : BuiltInFunction
{
	private IsOnSameSideInArena(IList<IFunction> parameters)
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
			ErrorMessage = "Character Function in IsOnSameSideInArena Function returned an error: " + characterFunction.ErrorMessage;
			return StatementResult.Error;
		}

		var character = (ICharacter)characterFunction.Result?.GetObject;
		if (character is null)
		{
			ErrorMessage = "Character Function in IsOnSameSideInArena Function did not return a valid character.";
			return StatementResult.Error;
		}

		var targetFunction = ParameterFunctions.ElementAt(1);
		if (targetFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "Target Function in IsOnSameSideInArena Function returned an error: " + targetFunction.ErrorMessage;
			return StatementResult.Error;
		}

		var target = (ICharacter)targetFunction.Result?.GetObject;
		if (target is null)
		{
			ErrorMessage = "Target Function in IsOnSameSideInArena Function did not return a valid character.";
			return StatementResult.Error;
		}

		var characterEffect = character.EffectsOfType<ArenaParticipationEffect>().FirstOrDefault();
		var targetEffect = target.EffectsOfType<ArenaParticipationEffect>().FirstOrDefault();

		if (characterEffect is null || targetEffect is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (characterEffect.ArenaEvent != targetEffect.ArenaEvent)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (characterEffect.ArenaEvent.Participants.First(x => x.Character == character).SideIndex ==
			characterEffect.ArenaEvent.Participants.First(x => x.Character == target).SideIndex)
		{
			Result = new BooleanVariable(true);
			return StatementResult.Normal;
		}

		Result = new BooleanVariable(false);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"isonsamesideinarena",
				[ProgVariableTypes.Character, ProgVariableTypes.Character],
				(pars, gameworld) => new IsOnSameSideInArena(pars),
				["character", "target"],
				["The character to check for arena battle participation.", "The target that you want to check is on the same team"],
				"The function returns true if the given character and target are currently participating in an arena battle and are on the same team.",
				"Arena",
				ProgVariableTypes.Boolean
			)
		);
	}
}
