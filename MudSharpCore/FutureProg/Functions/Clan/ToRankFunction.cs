using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Community;
using MudSharp.Framework;

namespace MudSharp.FutureProg.Functions.Clan;

internal class ToRankFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	public ToRankFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.ClanRank;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var clan = (IClan)ParameterFunctions.ElementAt(0).Result;
		if (clan == null)
		{
			ErrorMessage = "Clan Function in ToRank returned null.";
			return StatementResult.Error;
		}

		Result = ParameterFunctions[1].ReturnType.CompatibleWith(FutureProgVariableTypes.Text)
			? clan.Ranks.FirstOrDefault(
				x =>
					x.Name.Equals((string)ParameterFunctions.ElementAt(1).Result.GetObject,
						StringComparison.InvariantCultureIgnoreCase))
			: clan.Ranks.FirstOrDefault(x => x.Id == (int)(decimal)ParameterFunctions.ElementAt(1).Result.GetObject);

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"torank",
			new[] { FutureProgVariableTypes.Clan, FutureProgVariableTypes.Number },
			(pars, gameworld) => new ToRankFunction(pars, gameworld)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"torank",
			new[] { FutureProgVariableTypes.Clan, FutureProgVariableTypes.Text },
			(pars, gameworld) => new ToRankFunction(pars, gameworld)
		));
	}
}