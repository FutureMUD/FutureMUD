using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Community;
using MudSharp.Framework;

namespace MudSharp.FutureProg.Functions.Clan;

internal class ToAppointmentFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	public ToAppointmentFunction(IList<IFunction> parameters, IFuturemud gameworld)
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

		var clan = (IClan)ParameterFunctions.ElementAt(0).Result;
		if (clan == null)
		{
			ErrorMessage = "Clan Function in ToAppointment returned null.";
			return StatementResult.Error;
		}

		Result = ParameterFunctions[0].ReturnType.CompatibleWith(ProgVariableTypes.Text)
			? clan.Appointments.FirstOrDefault(
				x =>
					x.Name.Equals((string)ParameterFunctions.ElementAt(1).Result.GetObject,
						StringComparison.InvariantCultureIgnoreCase))
			: clan.Appointments.FirstOrDefault(
				x => x.Id == (int)(decimal)ParameterFunctions.ElementAt(1).Result.GetObject);

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"toappointment",
			new[] { ProgVariableTypes.Clan, ProgVariableTypes.Number },
			(pars, gameworld) => new ToAppointmentFunction(pars, gameworld)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"toappointment",
			new[] { ProgVariableTypes.Clan, ProgVariableTypes.Text },
			(pars, gameworld) => new ToAppointmentFunction(pars, gameworld)
		));
	}
}