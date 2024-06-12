using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Clan;

internal class ClanAppointFunction : BuiltInFunction
{
	private ClanAppointFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = (ICharacter)ParameterFunctions.ElementAt(0).Result;
		var clan = (IClan)ParameterFunctions.ElementAt(1).Result;
		var appointment = (IAppointment)ParameterFunctions.ElementAt(2).Result;

		if (clan is null || appointment is null)
		{
			ErrorMessage = "Null clan or appointment in ClanAppoint";
			return StatementResult.Error;
		}

		if (appointment.Clan != clan)
		{
			ErrorMessage = $"Tried to add an appointment from a non-matching clan";
			return StatementResult.Error;
		}

		var existing = character.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
		if (existing is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (existing.Appointments.Contains(appointment))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		existing.Appointments.Add(appointment);
		if (appointment.MinimumRankToHold.RankNumber > existing.Rank.RankNumber)
		{
			existing.SetRank(appointment.MinimumRankToHold);
		}
		existing.Changed = true;
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"clanappoint",
				new[]
				{
					FutureProgVariableTypes.Character, FutureProgVariableTypes.Clan,
					FutureProgVariableTypes.ClanAppointment
				},
				(pars, gameworld) => new ClanAppointFunction(pars),
				new List<string>
				{
					"Character",
					"Clan",
					"Appointment",
				},
				new List<string>
				{
					"The characer to be invited to the clan",
					"The clan to invite them into",
					"The appointment for them to be appointed to",
				},
				"This function appoints a character to a position in a clan they're already in. It returns false if the character was not in the clan.",
				"Clans",
				FutureProgVariableTypes.Boolean
			)
		);
	}
}