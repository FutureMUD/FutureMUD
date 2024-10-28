using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Accounts;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Chargen;

internal class GetResourceFunction : BuiltInFunction, IHaveFuturemud
{
	public GetResourceFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		Gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Number;
		protected set { }
	}

	#region IHaveFuturemud Members

	public IFuturemud Gameworld { get; set; }

	#endregion

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}


		var chargen = (IHaveAccount)ParameterFunctions[0].Result;
		var text = (string)ParameterFunctions[1].Result.GetObject;
		var resource =
			Gameworld.ChargenResources.FirstOrDefault(
				x => x.Name.Equals(text, StringComparison.InvariantCultureIgnoreCase)) ??
			Gameworld.ChargenResources.FirstOrDefault(
				x => x.Alias.Equals(text, StringComparison.InvariantCultureIgnoreCase));
		if (resource == null)
		{
			ErrorMessage = $"{text} is not a valid Chargen Resource.";
			return StatementResult.Error;
		}

		Result = new NumberVariable(chargen.Account.AccountResources[resource]);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"getresource",
				new[] { ProgVariableTypes.Toon, ProgVariableTypes.Text },
				(pars, gameworld) => new GetResourceFunction(pars, gameworld),
				new List<string> { "character", "resource" },
				new List<string> { "The character whose account you want to look up", "The name of the resource you want to look up" },
				"Returns the amount of the specified chargen resources (karma, RPP, BP, etc) that an account has",
				"Chargen",
				ProgVariableTypes.Number
				)
		);
	}
}