#nullable enable

using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.FutureProg.Variables;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class IsAnimalFunction : BuiltInFunction
{
	public IsAnimalFunction(IList<IFunction> parameters)
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
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result?.GetObject is not ICharacter character)
		{
			ErrorMessage = "Character parameter in isanimal did not resolve to a character.";
			return StatementResult.Error;
		}

		Result = new BooleanVariable(AnimalLineageHelper.IsAnimal(character));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"isanimal",
				[ProgVariableTypes.Character],
				(pars, _) => new IsAnimalFunction(pars),
				["character"],
				["The character whose race lineage you want to test."],
				"Returns true when the character's race descends from one of the stock non-humanoid animal body families.",
				"Character",
				ProgVariableTypes.Boolean
			)
		);
	}
}
