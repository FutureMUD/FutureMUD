using System.Collections.Generic;
using System.Linq;
using MudSharp.Community;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToClanAppointmentFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	protected ToClanAppointmentFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.ClanAppointment;
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
			Result = _gameworld.Clans.SelectMany(x => x.Appointments).Get((long)(decimal)(ParameterFunctions[0].Result?.GetObject ?? 0.0M));
			return StatementResult.Normal;
		}

		var clan = ParameterFunctions[0].Result?.GetObject as IClan;
		if (clan is null)
		{
			Result = new NullVariable(ProgVariableTypes.ClanAppointment);
			return StatementResult.Normal;
		}

		var text = ParameterFunctions[1].Result?.GetObject?.ToString();
		if (text is null)
		{
			Result = new NullVariable(ProgVariableTypes.ClanAppointment);
			return StatementResult.Normal;
		}

		Result = clan.Appointments.FirstOrDefault(x => x.Name.EqualTo(text)) ??
			clan.Appointments.FirstOrDefault(x => x.Titles.Any(y => y.EqualTo(text))) ??
			clan.Appointments.FirstOrDefault(x => x.Abbreviations.Any(y => y.EqualTo(text)));

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"toappointment",
			new[] { ProgVariableTypes.Number },
			(pars, gameworld) => new ToClanAppointmentFunction(pars, gameworld),
			new List<string> { "id" },
			new List<string> { "The ID to look up" },
			"Converts an ID number into the specified type, if one exists",
			"Lookup",
			ProgVariableTypes.ClanAppointment
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"toappointment",
			new[] { ProgVariableTypes.Clan, ProgVariableTypes.Text },
			(pars, gameworld) => new ToClanAppointmentFunction(pars, gameworld),
			new List<string> { "clan", "name" },
			new List<string> { "The clan in which you want to search", "The name to look up" },
			"Converts a name into the specified type, if one exists",
			"Lookup",
			ProgVariableTypes.ClanAppointment
		));
	}
}