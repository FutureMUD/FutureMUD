using MudSharp.Character;
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

internal class IsWantedFunction : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"IsWanted".ToLowerInvariant(),
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.LegalAuthority
				}, // the parameters the function takes
				(pars, gameworld) => new IsWantedFunction(pars, gameworld),
				new List<string> { "character", "legalauthority" }, // parameter names
				new List<string>
				{
					"The character whose wanted status you want to know",
					"The legal authority you're checking against, or null if checking against all"
				}, // parameter help text
				"This function determines if the supplied character is wanted for arrest in the specified legal authority", // help text for the function,
				"Crime", // the category to which this function belongs,
				ProgVariableTypes.Boolean // the return type of the function
			)
		);
	}

	#endregion

	#region Constructors

	protected IsWantedFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

		var authorities = ParameterFunctions[1].Result?.GetObject is not ILegalAuthority authority
			? Gameworld.LegalAuthorities.ToList()
			: new List<ILegalAuthority> { authority };
		foreach (var item in authorities)
		{
			var crimes = item.KnownCrimesForIndividual(character).ToList();
			if (crimes.Any(x => !x.BailPosted && x.Law.EnforcementStrategy >= EnforcementStrategy.ArrestAndDetain))
			{
				Result = new BooleanVariable(true);
				return StatementResult.Normal;
			}
		}

		Result = new BooleanVariable(false);
		return StatementResult.Normal;
	}
}