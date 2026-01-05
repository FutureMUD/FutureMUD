using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToCharacterFunction : BuiltInFunction
{
	public IFuturemud Gameworld { get; }
	protected ToCharacterFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		Gameworld = gameworld;
    }

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Character;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var characterid = (long?)(decimal?)ParameterFunctions[0].Result?.GetObject ?? 0L;
		var character = Gameworld.TryGetCharacter(characterid, true);

		Result = character;
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "tocharacter",
            [ProgVariableTypes.Number],
            (pars, gameworld) => new ToCharacterFunction(pars, gameworld),
			["id"],
			["The id of the character to retrieve or load"],
			"Retrieves or loads a character by their ID. Does not add them to the gameworld if it does load them. Can return null if no character with that ID is found.",
			"Lookup",
			ProgVariableTypes.Character
        ));
    }
}