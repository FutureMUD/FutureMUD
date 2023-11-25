using System.Collections.Generic;
using System.Linq;
using MudSharp.Community;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToClanRankFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	protected ToClanRankFunction(IList<IFunction> parameters, IFuturemud gameworld)
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

		if (ParameterFunctions.Count == 1)
		{
			Result = _gameworld.Clans.SelectMany(x => x.Ranks).Get((long)(decimal)(ParameterFunctions[0].Result?.GetObject ?? 0.0M));
			return StatementResult.Normal;
		}

		var clan = ParameterFunctions[0].Result?.GetObject as IClan;
		if (clan is null)
		{
			Result = new NullVariable(FutureProgVariableTypes.ClanRank);
			return StatementResult.Normal;
		}

		var text = ParameterFunctions[1].Result?.GetObject?.ToString();
		if (text is null)
		{
			Result = new NullVariable(FutureProgVariableTypes.ClanRank);
			return StatementResult.Normal;
		}

		Result = clan.Ranks.FirstOrDefault(x => x.Name.EqualTo(text)) ??
			clan.Ranks.FirstOrDefault(x => x.Titles.Any(y => y.EqualTo(text))) ??
			clan.Ranks.FirstOrDefault(x => x.Abbreviations.Any(y => y.EqualTo(text)));

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"torank",
			new[] { FutureProgVariableTypes.Number },
			(pars, gameworld) => new ToClanRankFunction(pars, gameworld),
			new List<string> { "id" },
			new List<string> { "The ID to look up" },
			"Converts an ID number into the specified type, if one exists",
			"Lookup",
			FutureProgVariableTypes.ClanRank
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"torank",
			new[] { FutureProgVariableTypes.Clan, FutureProgVariableTypes.Text },
			(pars, gameworld) => new ToClanRankFunction(pars, gameworld),
			new List<string> { "clan", "name" },
			new List<string> { "The clan in which you want to search", "The name to look up" },
			"Converts a name into the specified type, if one exists",
			"Lookup",
			FutureProgVariableTypes.ClanRank
		));
	}
}
