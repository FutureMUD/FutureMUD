using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.FutureProg.Functions.Currency;

internal class ToCurrencyFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	public ToCurrencyFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Currency;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result = ParameterFunctions[0].ReturnType.CompatibleWith(ProgVariableTypes.Text)
			? _gameworld.Currencies.Get((string)ParameterFunctions[0].Result.GetObject).FirstOrDefault()
			: _gameworld.Currencies.Get((long)(decimal)ParameterFunctions[0].Result.GetObject);

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tocurrency",
			new[] { ProgVariableTypes.Number },
			(pars, gameworld) => new ToCurrencyFunction(pars, gameworld)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tocurrency",
			new[] { ProgVariableTypes.Text },
			(pars, gameworld) => new ToCurrencyFunction(pars, gameworld)
		));
	}
}