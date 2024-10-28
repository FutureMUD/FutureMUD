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

internal class CrimesFunction : BuiltInFunction
{
	protected enum CrimesFunctionMode
	{
		All,
		Known,
		Unknown,
		Resolved
	}

	public IFuturemud Gameworld { get; set; }
	protected CrimesFunctionMode Mode { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"allcrimes",
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.LegalAuthority
				}, // the parameters the function takes
				(pars, gameworld) => new CrimesFunction(pars, gameworld, CrimesFunctionMode.All),
				new List<string> { "character", "authority" }, // parameter names
				new List<string>
				{
					"The character to fetch crimes for", "The authority to check crimes in, or if null, all authorities"
				}, // parameter help text
				"Returns a list of all the crimes a character has committed in the specified authority (or authorities)", // help text for the function,
				"Crime", // the category to which this function belongs,
				ProgVariableTypes.Crime | ProgVariableTypes.Collection // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"knowncrimes",
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.LegalAuthority
				}, // the parameters the function takes
				(pars, gameworld) => new CrimesFunction(pars, gameworld, CrimesFunctionMode.Known),
				new List<string> { "character", "authority" }, // parameter names
				new List<string>
				{
					"The character to fetch crimes for", "The authority to check crimes in, or if null, all authorities"
				}, // parameter help text
				"Returns a list of all the unresolved crimes known to authorities in the specified authority (or authorities)", // help text for the function,
				"Crime", // the category to which this function belongs,
				ProgVariableTypes.Crime | ProgVariableTypes.Collection // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"unknowncrimes",
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.LegalAuthority
				}, // the parameters the function takes
				(pars, gameworld) => new CrimesFunction(pars, gameworld, CrimesFunctionMode.Unknown),
				new List<string> { "character", "authority" }, // parameter names
				new List<string>
				{
					"The character to fetch crimes for", "The authority to check crimes in, or if null, all authorities"
				}, // parameter help text
				"Returns a list of all the unresolved crimes unknown to authorities in the specified authority (or authorities)", // help text for the function,
				"Crime", // the category to which this function belongs,
				ProgVariableTypes.Crime | ProgVariableTypes.Collection // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"resolvedcrimes",
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.LegalAuthority
				}, // the parameters the function takes
				(pars, gameworld) => new CrimesFunction(pars, gameworld, CrimesFunctionMode.Known),
				new List<string> { "character", "authority" }, // parameter names
				new List<string>
				{
					"The character to fetch crimes for", "The authority to check crimes in, or if null, all authorities"
				}, // parameter help text
				"Returns a list of all the resolved crimes in the specified authority (or authorities)", // help text for the function,
				"Crime", // the category to which this function belongs,
				ProgVariableTypes.Crime | ProgVariableTypes.Collection // the return type of the function
			)
		);
	}

	#endregion

	#region Constructors

	protected CrimesFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld, CrimesFunctionMode mode) : base(
		parameterFunctions)
	{
		Gameworld = gameworld;
		Mode = mode;
	}

	#endregion

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Crime | ProgVariableTypes.Collection;
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

		var authorityArg = ParameterFunctions[1].Result?.GetObject as ILegalAuthority;
		var authorities = new List<ILegalAuthority>();
		if (authorityArg is not null)
		{
			authorities.Add(authorityArg);
		}
		else
		{
			authorities.AddRange(Gameworld.LegalAuthorities);
		}

		var crimes = new List<ICrime>();
		switch (Mode)
		{
			case CrimesFunctionMode.All:
				foreach (var authority in authorities)
				{
					crimes.AddRange(authority.KnownCrimesForIndividual(character));
					crimes.AddRange(authority.UnknownCrimesForIndividual(character));
					crimes.AddRange(authority.ResolvedCrimesForIndividual(character));
				}

				break;
			case CrimesFunctionMode.Known:
				foreach (var authority in authorities)
				{
					crimes.AddRange(authority.KnownCrimesForIndividual(character));
				}

				break;
			case CrimesFunctionMode.Unknown:
				foreach (var authority in authorities)
				{
					crimes.AddRange(authority.UnknownCrimesForIndividual(character));
				}

				break;
			case CrimesFunctionMode.Resolved:
				foreach (var authority in authorities)
				{
					crimes.AddRange(authority.ResolvedCrimesForIndividual(character));
				}

				break;
		}

		Result = new CollectionVariable(crimes, ProgVariableTypes.Crime);
		return StatementResult.Normal;
	}
}