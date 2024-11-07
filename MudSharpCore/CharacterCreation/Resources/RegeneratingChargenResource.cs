using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Models;
using MudSharp.Character;
using ExpressionEngine;
using MudSharp.Framework;
using MudSharp.Database;

namespace MudSharp.CharacterCreation.Resources;

public class RegeneratingChargenResource : ChargenResourceBase
{
	public TimeSpan AwardInterval => MinimumTimeBetweenAwards;
	public double AwardAmount => MaximumNumberAwardedPerAward;

	public RegeneratingChargenResource(IFuturemud gameworld, ChargenResource resource) : base(gameworld, resource)
	{
	}

	public RegeneratingChargenResource(IFuturemud gameworld, string name, string plural, string alias) : base(gameworld, name, plural, alias)
	{
		MaximumNumberAwardedPerAward = 5;
		MinimumTimeBetweenAwards = TimeSpan.FromMinutes(15);
		MaximumResourceExpression = new Expression("1000");
		DoDatabaseInsert("Regenerating");
	}

	public override void UpdateOnSave(ICharacter character, int oldMinutes, int newMinutes)
	{
		if (!character.IsPlayerCharacter || character.IsGuest)
		{
			return;
		}

		var lastAward = character.Account.AccountResourcesLastAwarded.ValueOrDefault(this, null) ??
		                character.LoginDateTime;
		if (character.LoginDateTime > lastAward)
		{
			lastAward = character.LoginDateTime;
		}

		var intervalCount = (DateTime.UtcNow - lastAward).Ticks / (double)AwardInterval.Ticks;
		if (intervalCount > 0 &&
		    character.Account.AccountResources[this] < GetMaximum(character.Account) &&
		    ControlProg?.ExecuteBool(character) != false
			)
		{
			character.Account.AccountResources[this] += intervalCount;
			character.Account.AccountResourcesLastAwarded[this] = DateTime.UtcNow;
			var dbaccount = FMDB.Context.Accounts.Find(character.Account.Id);
			if (dbaccount == null)
			{
				return;
			}

			var dbaccountresource =
				dbaccount.AccountsChargenResources.FirstOrDefault(
					x => x.ChargenResourceId == Id);
			if (dbaccountresource == null)
			{
				dbaccountresource = new AccountsChargenResources();
				dbaccount.AccountsChargenResources.Add(dbaccountresource);
				dbaccountresource.Amount = 0;
				dbaccountresource.ChargenResourceId = Id;
			}

			dbaccountresource.Amount = character.Account.AccountResources[this];
			dbaccountresource.LastAwardDate = character.Account.AccountResourcesLastAwarded[this].Value;
		}
	}

	/// <inheritdoc />
	public override string TypeName => "Regenerating";
}