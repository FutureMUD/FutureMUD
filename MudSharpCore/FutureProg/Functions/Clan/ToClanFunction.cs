using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.FutureProg.Functions.Clan;

internal class ToClanFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	public ToClanFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Clan;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result = ParameterFunctions[0].ReturnType.CompatibleWith(FutureProgVariableTypes.Text)
			? _gameworld.Clans.Get((string)ParameterFunctions[0].Result.GetObject).FirstOrDefault()
			: _gameworld.Clans.Get((long)(decimal)ParameterFunctions[0].Result.GetObject);

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"toclan",
			new[] { FutureProgVariableTypes.Number },
			(pars, gameworld) => new ToClanFunction(pars, gameworld)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"toclan",
			new[] { FutureProgVariableTypes.Text },
			(pars, gameworld) => new ToClanFunction(pars, gameworld)
		));
	}
}