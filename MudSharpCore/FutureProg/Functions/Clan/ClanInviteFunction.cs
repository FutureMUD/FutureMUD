using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Database;

namespace MudSharp.FutureProg.Functions.Clan;

internal class ClanInviteFunction : BuiltInFunction
{
	private ClanInviteFunction(IList<IFunction> parameters)
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
		var rank = ParameterFunctions.Count >= 3 ? (IRank)ParameterFunctions.ElementAt(2).Result : null;
		var manager = ParameterFunctions.Count == 4 ? (ICharacter)ParameterFunctions.ElementAt(3).Result : null;

		if (character == null || clan == null)
		{
			ErrorMessage = "Null character or clan in ClanInvite";
			return StatementResult.Error;
		}

		if (rank == null)
		{
			rank = clan.Ranks.Any() ? clan.Ranks.First() : null;
		}

		if (rank == null)
		{
			ErrorMessage = "Couldn't find a valid clan rank";
			return StatementResult.Error;
		}

		if (rank.Clan != clan)
		{
			ErrorMessage = $"Tried to add a rank from a non-matching clan";
			return StatementResult.Error;
		}

		using (new FMDB())
		{
			var dbitem = new Models.ClanMembership
			{
				CharacterId = character.Id,
				ClanId = clan.Id,
				RankId = rank.Id,
				PaygradeId = rank.Paygrades.Any() ? rank.Paygrades.First().Id : (long?)null,
				PersonalName = character.CurrentName.SaveToXml().ToString(),
				JoinDate = clan.Calendar.CurrentDate.GetDateString(),
				ManagerId = manager?.Id
			};
			FMDB.Context.ClanMemberships.Add(dbitem);
			FMDB.Context.SaveChanges();
			var newMembership = new ClanMembership(dbitem, clan, character.Gameworld);
			character.AddMembership(newMembership);
			clan.Memberships.Add(newMembership);
		}

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"claninvite",
				new[]
				{
					FutureProgVariableTypes.Character, FutureProgVariableTypes.Clan,
					FutureProgVariableTypes.ClanRank, FutureProgVariableTypes.Character
				},
				(pars, gameworld) => new ClanInviteFunction(pars)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"claninvite",
				new[]
				{
					FutureProgVariableTypes.Character, FutureProgVariableTypes.Clan,
					FutureProgVariableTypes.ClanRank
				},
				(pars, gameworld) => new ClanInviteFunction(pars)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"claninvite",
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Clan },
				(pars, gameworld) => new ClanInviteFunction(pars)
			)
		);
	}
}