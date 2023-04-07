using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Clan;

internal class IsClanMemberFunction : BuiltInFunction
{
	public IsClanMemberFunction(IList<IFunction> parameters)
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
		var characterFunction = ParameterFunctions.ElementAt(0);
		var clanFunction = ParameterFunctions.ElementAt(1);
		var thirdFunction = ParameterFunctions.ElementAtOrDefault(2);
		IFunction rankFunction = null, appointmentFunction = null, paygradeFunction = null;
		if (thirdFunction != null)
		{
			switch (thirdFunction.ReturnType)
			{
				case FutureProgVariableTypes.ClanRank:
					rankFunction = thirdFunction;
					paygradeFunction = ParameterFunctions.ElementAtOrDefault(3);
					break;
				case FutureProgVariableTypes.ClanAppointment:
					appointmentFunction = thirdFunction;
					break;
			}
		}

		if (characterFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "Character Function in IsClanMember Function returned an error: " +
			               characterFunction.ErrorMessage;
			return StatementResult.Error;
		}

		if (clanFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "Clan Function in IsClanMember Function returned an error: " + clanFunction.ErrorMessage;
			return StatementResult.Error;
		}

		Func<IClanMembership, bool> function;
		if (rankFunction != null)
		{
			if (rankFunction.Execute(variables) == StatementResult.Error)
			{
				ErrorMessage = "Rank Function in IsClanMember Function returned an error: " +
				               rankFunction.ErrorMessage;
				return StatementResult.Error;
			}

			if (paygradeFunction != null)
			{
				if (paygradeFunction.Execute(variables) == StatementResult.Error)
				{
					ErrorMessage = "Paygrade Function in IsClanMember Function returned an error: " +
					               paygradeFunction.ErrorMessage;
					return StatementResult.Error;
				}

				var rank = (IRank)rankFunction.Result;
				var paygrade = (IPaygrade)paygradeFunction.Result;

				if (rank == null)
				{
					ErrorMessage = "Rank was null in IsClanMember function.";
					return StatementResult.Error;
				}

				if (paygrade == null)
				{
					ErrorMessage = "Paygrade was null in IsClanMember function.";
					return StatementResult.Error;
				}

				function = member =>
					member.Clan.Equals(clanFunction.Result.GetObject) &&
					(rank.RankNumber < member.Rank.RankNumber ||
					 (rank.RankNumber == member.Rank.RankNumber &&
					  rank.Paygrades.IndexOf(member.Paygrade) >= rank.Paygrades.IndexOf(paygrade))
					);
			}
			else
			{
				function =
					member =>
						member.Clan.Equals(clanFunction.Result.GetObject) && rankFunction.Result != null &&
						((IRank)rankFunction.Result.GetObject).RankNumber <= member.Rank.RankNumber;
			}
		}
		else if (appointmentFunction != null)
		{
			if (appointmentFunction.Execute(variables) == StatementResult.Error)
			{
				ErrorMessage = "Appointment Function in IsClanMember Function returned an error: " +
				               appointmentFunction.ErrorMessage;
				return StatementResult.Error;
			}

			function =
				member =>
					member.Clan.Equals(clanFunction.Result.GetObject) && appointmentFunction.Result != null &&
					member.Appointments.Contains((IAppointment)appointmentFunction.Result.GetObject);
		}
		else
		{
			function = member => member.Clan.Equals(clanFunction.Result.GetObject);
		}

		Result = new BooleanVariable(((ICharacter)characterFunction.Result.GetObject).ClanMemberships.Any(function));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"isclanmember",
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Clan },
				(pars, gameworld) => new IsClanMemberFunction(pars)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"isclanmember",
				new[]
				{
					FutureProgVariableTypes.Character, FutureProgVariableTypes.Clan,
					FutureProgVariableTypes.ClanRank
				},
				(pars, gameworld) => new IsClanMemberFunction(pars)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"isclanmember",
				new[]
				{
					FutureProgVariableTypes.Character, FutureProgVariableTypes.Clan,
					FutureProgVariableTypes.ClanAppointment
				},
				(pars, gameworld) => new IsClanMemberFunction(pars)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"isclanmember",
				new[]
				{
					FutureProgVariableTypes.Character, FutureProgVariableTypes.Clan,
					FutureProgVariableTypes.ClanRank, FutureProgVariableTypes.ClanPaygrade
				},
				(pars, gameworld) => new IsClanMemberFunction(pars)
			)
		);
	}
}