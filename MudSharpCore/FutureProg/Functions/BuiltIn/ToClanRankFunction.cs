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

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.ClanRank;
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
			Result = new NullVariable(ProgVariableTypes.ClanRank);
			return StatementResult.Normal;
		}

		if (ParameterFunctions[1].ReturnType.CompatibleWith(ProgVariableTypes.Text))
		{
			var text = ParameterFunctions[1].Result?.GetObject?.ToString();
			if (text is null)
			{
				Result = new NullVariable(ProgVariableTypes.ClanRank);
				return StatementResult.Normal;
			}

			Result = clan.Ranks.FirstOrDefault(x => x.Name.EqualTo(text)) ??
			         clan.Ranks.FirstOrDefault(x => x.Titles.Any(y => y.EqualTo(text))) ??
			         clan.Ranks.FirstOrDefault(x => x.Abbreviations.Any(y => y.EqualTo(text)));

			return StatementResult.Normal;
		}

		var rankNumber = (int)(decimal)(ParameterFunctions[1].Result?.GetObject ?? 1.0);
		Result = clan.Ranks.FirstOrDefault(x => x.RankNumber == rankNumber) ?? (IProgVariable)new NullVariable(ProgVariableTypes.ClanRank);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"torank",
			new[] { ProgVariableTypes.Number },
			(pars, gameworld) => new ToClanRankFunction(pars, gameworld),
			new List<string> { "id" },
			new List<string> { "The ID to look up" },
			"Converts an ID number into the specified type, if one exists",
			"Lookup",
			ProgVariableTypes.ClanRank
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"torank",
			new[] { ProgVariableTypes.Clan, ProgVariableTypes.Text },
			(pars, gameworld) => new ToClanRankFunction(pars, gameworld),
			new List<string> { "clan", "name" },
			new List<string> { "The clan in which you want to search", "The name to look up" },
			"Converts a name and clan into the specified type, if one exists",
			"Lookup",
			ProgVariableTypes.ClanRank
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"torank",
			new[] { ProgVariableTypes.Clan, ProgVariableTypes.Text },
			(pars, gameworld) => new ToClanRankFunction(pars, gameworld),
			new List<string> { "clan", "rank #" },
			new List<string> { "The clan in which you want to search", "The rank number of the rank" },
			"Converts a rank number and clan into the specified type, if one exists",
			"Lookup",
			ProgVariableTypes.ClanRank
		));
	}
}
