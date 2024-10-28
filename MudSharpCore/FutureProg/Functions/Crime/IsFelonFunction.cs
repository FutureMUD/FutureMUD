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

internal class IsFelonFunction : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"IsFelon".ToLowerInvariant(),
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.LegalAuthority
				}, // the parameters the function takes
				(pars, gameworld) => new IsFelonFunction(pars, gameworld),
				new List<string> { "character", "authority" }, // parameter names
				new List<string>
				{
					"The character to check", "The legal authority in which to check, or all authorities if null"
				}, // parameter help text
				"This function checks to see if the character has ever had a serious crime (one that incurred jail time or capital punishment) recorded against them", // help text for the function,
				"Crime", // the category to which this function belongs,
				ProgVariableTypes.Boolean // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"IsFelon".ToLowerInvariant(),
				new[]
				{
					ProgVariableTypes.Character,
				}, // the parameters the function takes
				(pars, gameworld) => new IsFelonFunction(pars, gameworld),
				new List<string> { "character", }, // parameter names
				new List<string>
				{
					"The character to check"
				}, // parameter help text
				"This function checks to see if the character has ever had a serious crime (one that incurred jail time or capital punishment) recorded against them", // help text for the function,
				"Crime", // the category to which this function belongs,
				ProgVariableTypes.Boolean // the return type of the function
			)
		);
	}

	#endregion

	#region Constructors

	protected IsFelonFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

		var authorityArg =
			ParameterFunctions.Count == 2 ?
			ParameterFunctions[1].Result?.GetObject as ILegalAuthority :
			null;
		var authorities = new List<ILegalAuthority>();
		if (authorityArg is not null)
		{
			authorities.Add(authorityArg);
		}
		else
		{
			authorities.AddRange(Gameworld.LegalAuthorities);
		}

		foreach (var authority in authorities)
		{
			var crimes = authority.ResolvedCrimesForIndividual(character).ToList();
			if (crimes.Any(x => x.HasBeenConvicted && x.CustodialSentenceLength > TimeSpan.Zero))
			{
				Result = new BooleanVariable(true);
				return StatementResult.Normal;
			}
		}

		Result = new BooleanVariable(false);
		return StatementResult.Normal;
	}
}