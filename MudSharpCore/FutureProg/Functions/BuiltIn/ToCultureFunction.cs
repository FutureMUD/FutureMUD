using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToCultureFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	public ToCultureFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Culture;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result = ParameterFunctions[0].ReturnType.CompatibleWith(FutureProgVariableTypes.Text)
			? _gameworld.Cultures.Get((string)ParameterFunctions[0].Result.GetObject).FirstOrDefault()
			: _gameworld.Cultures.Get((long)(decimal)ParameterFunctions[0].Result.GetObject);

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"toculture",
			new[] { FutureProgVariableTypes.Number },
			(pars, gameworld) => new ToCultureFunction(pars, gameworld),
			new List<string> { "id" },
			new List<string> { "The ID to look up" },
			"Converts an ID number into the specified type, if one exists",
			"Lookup",
			FutureProgVariableTypes.Culture
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"toculture",
			new[] { FutureProgVariableTypes.Text },
			(pars, gameworld) => new ToCultureFunction(pars, gameworld),
			new List<string> { "name" },
			new List<string> { "The name to look up" },
			"Converts a name into the specified type, if one exists",
			"Lookup",
			FutureProgVariableTypes.Culture
		));
	}
}