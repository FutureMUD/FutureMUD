using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToLegalAuthorityFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	protected ToLegalAuthorityFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.LegalAuthority;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result = ParameterFunctions[0].ReturnType.CompatibleWith(ProgVariableTypes.Text)
			? _gameworld.LegalAuthorities.Get((string)ParameterFunctions[0].Result.GetObject).FirstOrDefault()
			: _gameworld.LegalAuthorities.Get((long)(decimal)ParameterFunctions[0].Result.GetObject);

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tolegalauthority",
			new[] { ProgVariableTypes.Number },
			(pars, gameworld) => new ToLegalAuthorityFunction(pars, gameworld),
			new List<string> { "id" },
			new List<string> { "The ID to look up" },
			"Converts an ID number into the specified type, if one exists",
			"Lookup",
			ProgVariableTypes.LegalAuthority
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tolegalauthority",
			new[] { ProgVariableTypes.Text },
			(pars, gameworld) => new ToLegalAuthorityFunction(pars, gameworld),
			new List<string> { "name" },
			new List<string> { "The name to look up" },
			"Converts a name into the specified type, if one exists",
			"Lookup",
			ProgVariableTypes.LegalAuthority
		));
	}
}