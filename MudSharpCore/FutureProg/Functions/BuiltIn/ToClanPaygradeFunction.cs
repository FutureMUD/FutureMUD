using System.Collections.Generic;
using System.Linq;
using MudSharp.Community;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToClanPaygradeFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	protected ToClanPaygradeFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.ClanPaygrade;
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
			Result = _gameworld.Clans.SelectMany(x => x.Paygrades).Get((long)(decimal)(ParameterFunctions[0].Result?.GetObject ?? 0.0M));
			return StatementResult.Normal;
		}

		var clan = ParameterFunctions[0].Result?.GetObject as IClan;
		if (clan is null)
		{
			Result = new NullVariable(FutureProgVariableTypes.ClanAppointment);
			return StatementResult.Normal;
		}

		var text = ParameterFunctions[1].Result?.GetObject?.ToString();
		if (text is null)
		{
			Result = new NullVariable(FutureProgVariableTypes.ClanAppointment);
			return StatementResult.Normal;
		}

		Result = clan.Paygrades.FirstOrDefault(x => x.Name.EqualTo(text)) ??
			clan.Paygrades.FirstOrDefault(x => x.Abbreviation.EqualTo(text));

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"topaygrade",
			new[] { FutureProgVariableTypes.Number },
			(pars, gameworld) => new ToClanPaygradeFunction(pars, gameworld),
			new List<string> { "id" },
			new List<string> { "The ID to look up" },
			"Converts an ID number into the specified type, if one exists",
			"Lookup",
			FutureProgVariableTypes.ClanPaygrade
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"topaygrade",
			new[] { FutureProgVariableTypes.Clan, FutureProgVariableTypes.Text },
			(pars, gameworld) => new ToClanPaygradeFunction(pars, gameworld),
			new List<string> { "clan", "name" },
			new List<string> { "The clan in which you want to search", "The name to look up" },
			"Converts a name into the specified type, if one exists",
			"Lookup",
			FutureProgVariableTypes.ClanPaygrade
		));
	}
}