using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;

namespace MudSharp.FutureProg.Functions.Characters;

internal class GiveAccentFunction : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }
	#region Static Initialisation
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"giveaccent",
				new[] { ProgVariableTypes.Character, ProgVariableTypes.Accent }, // the parameters the function takes
				(pars, gameworld) => new GiveAccentFunction(pars, gameworld),
				new List<string> {
					"who",
					"accent"
				}, // parameter names
				new List<string> {
					"The character to give the accent to",
					"The accent to give the character"
				}, // parameter help text
				"Gives an accent to a character at automatic. Returns true if given, false if already had it.", // help text for the function,
				"Character",// the category to which this function belongs,
				ProgVariableTypes.Boolean // the return type of the function
			)
		);
	}
	#endregion

	#region Constructors
	protected GiveAccentFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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
			ErrorMessage = "The target parameter in GiveAccent returned null";
			return StatementResult.Error;
		}

		var accent = (IAccent)ParameterFunctions[1].Result;
		if (accent == null)
		{
			ErrorMessage = "The accent parameter in GiveAccent returned null";
			return StatementResult.Error;
		}

		if (target.AccentDifficulty(accent, false) == Difficulty.Automatic)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		target.LearnAccent(accent, Difficulty.Automatic);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}