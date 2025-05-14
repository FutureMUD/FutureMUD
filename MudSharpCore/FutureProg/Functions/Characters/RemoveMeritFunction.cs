using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.RPG.Merits;

namespace MudSharp.FutureProg.Functions.Characters;

internal class RemoveMeritFunction : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }
	#region Static Initialisation
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"removemerit",
				new[] { ProgVariableTypes.Character, ProgVariableTypes.Merit }, // the parameters the function takes
				(pars, gameworld) => new RemoveMeritFunction(pars, gameworld),
				new List<string> {
					"who",
					"merit"
				}, // parameter names
				new List<string> {
					"The character to take the merit from",
					"The merit to take from the character"
				}, // parameter help text
				"Takes a merit from a character. Returns true if taken, false if didn't have it.", // help text for the function,
				"Character",// the category to which this function belongs,
				ProgVariableTypes.Boolean // the return type of the function
			)
		);
	}
	#endregion

	#region Constructors
	protected RemoveMeritFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}
	#endregion

	public override ProgVariableTypes ReturnType
	{
		get { return ProgVariableTypes.Boolean; }
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var target = ParameterFunctions[0].Result as ICharacter;
		if (target == null)
		{
			ErrorMessage = "The target parameter in RemoveMerit returned null";
			return StatementResult.Error;
		}

		var merit = (IMerit)ParameterFunctions[1].Result;
		if (merit == null)
		{
			ErrorMessage = "The merit parameter in RemoveMerit returned null";
			return StatementResult.Error;
		}

		if (target.Merits.Contains(merit))
		{
			target.RemoveMerit(merit);
			Result = new BooleanVariable(true);
			return StatementResult.Normal;
		}

		Result = new BooleanVariable(false);
		return StatementResult.Normal;
	}
}