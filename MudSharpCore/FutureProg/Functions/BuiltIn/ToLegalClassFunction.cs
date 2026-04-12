using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.RPG.Law;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToLegalClassFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	protected ToLegalClassFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.LegalClass;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		ILegalClass result = null;
		if (ParameterFunctions.Count == 1)
		{
			result = _gameworld.LegalClasses.Get((long)(decimal)ParameterFunctions[0].Result.GetObject);
		}
		else
		{
			var authority = ParameterFunctions[1].Result?.GetObject as ILegalAuthority;
			if (authority is not null)
			{
				var text = (string)ParameterFunctions[0].Result.GetObject;
				result = authority.LegalClasses.FirstOrDefault(x => x.Name.EqualTo(text));
			}
		}

		Result = result is not null
			? result
			: new NullVariable(ReturnType);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tolegalclass",
			[ProgVariableTypes.Number],
			(pars, gameworld) => new ToLegalClassFunction(pars, gameworld),
			["id"],
			["The ID to look up"],
			"Converts an ID number into a legal class, if one exists",
			"Lookup",
			ProgVariableTypes.LegalClass
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tolegalclass",
			[ProgVariableTypes.Text, ProgVariableTypes.LegalAuthority],
			(pars, gameworld) => new ToLegalClassFunction(pars, gameworld),
			["name", "legalauthority"],
			["The legal class name to look up", "The legal authority that scopes the lookup"],
			"Converts a name within a legal authority into a legal class, if one exists",
			"Lookup",
			ProgVariableTypes.LegalClass
		));
	}
}
