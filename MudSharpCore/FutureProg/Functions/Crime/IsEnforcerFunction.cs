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

internal class IsEnforcerFunction : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"IsEnforcer".ToLowerInvariant(),
				new[]
				{
					FutureProgVariableTypes.Character, FutureProgVariableTypes.LegalAuthority
				}, // the parameters the function takes
				(pars, gameworld) => new IsEnforcerFunction(pars, gameworld),
				new List<string> { "character", "authority" }, // parameter names
				new List<string>
				{
					"The character you want to check",
					"The legal authority to check in, or if null, checks all of the authorities"
				}, // parameter help text
				"Returns true if the character is an enforcer in the specified legal authority or authorities", // help text for the function,
				"Crime", // the category to which this function belongs,
				FutureProgVariableTypes.Boolean // the return type of the function
			)
		);
	}

	#endregion

	#region Constructors

	protected IsEnforcerFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Boolean;
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

		foreach (var authority in authorities)
		{
			if (authority.GetEnforcementAuthority(character) is not null)
			{
				Result = new BooleanVariable(true);
				return StatementResult.Normal;
			}
		}

		Result = new BooleanVariable(false);
		return StatementResult.Normal;
	}
}