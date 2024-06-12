using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.FutureProg.Variables;

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

		var existing = character.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
		if (existing is not null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
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

		Result = new BooleanVariable(true);
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
				(pars, gameworld) => new ClanInviteFunction(pars),
				new List<string>
				{
					"Character",
					"Clan",
					"Rank",
					"Manager"
				},
				new List<string>
				{
					"The characer to be invited to the clan",
					"The clan to invite them into",
					"The rank for them to be set to",
					"The manager (or person considered to have invited them)"
				},
				"This function adds a character to a clan. It returns false if the character was already in the clan.",
				"Clans",
				FutureProgVariableTypes.Boolean
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
				(pars, gameworld) => new ClanInviteFunction(pars),
				new List<string>
				{
					"Character",
					"Clan",
					"Rank",
				},
				new List<string>
				{
					"The characer to be invited to the clan",
					"The clan to invite them into",
					"The rank for them to be set to",
				},
				"This function adds a character to a clan. It returns false if the character was already in the clan.",
				"Clans",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"claninvite",
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Clan },
				(pars, gameworld) => new ClanInviteFunction(pars),
				new List<string>
				{
					"Character",
					"Clan",
				},
				new List<string>
				{
					"The characer to be invited to the clan",
					"The clan to invite them into",
				},
				"This function adds a character to a clan at the default rank. It returns false if the character was already in the clan.",
				"Clans",
				FutureProgVariableTypes.Boolean
			)
		);
	}
}