using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Law;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.FutureProg.Functions.Crime;

internal class InPrisonFunction : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"InPrison".ToLowerInvariant(),
				new[] { ProgVariableTypes.Character, ProgVariableTypes.LegalAuthority }, // the parameters the function takes
				(pars, gameworld) => new InPrisonFunction(pars, gameworld),
				new List<string> { "character", "authority" }, // parameter names
				new List<string>
				{
					"The character who you want to check for being in prison",
					"The legal authority to check in, or null for all"
				}, // parameter help text
				"This function checks if the character is currently serving a prison sentence", // help text for the function,
				"Crime", // the category to which this function belongs,
				ProgVariableTypes.Boolean // the return type of the function
			)
		);
	}

	#endregion

	#region Constructors

	protected InPrisonFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

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
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var authority = ParameterFunctions[1].Result?.GetObject as ILegalAuthority;
		Result = new BooleanVariable(character.EffectsOfType<ServingCustodialSentence>()
		                                      .Any(x => authority is null || x.LegalAuthority == authority));
		return StatementResult.Normal;
	}
}