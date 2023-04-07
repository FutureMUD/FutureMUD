using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp.CharacterCreation.Resources;

internal class TotalPlaytimeResource : ChargenResourceBase
{
	public override string FrameworkItemType => "ChargenResource";

	public TimeSpan AwardInterval { get; set; }
	public double AwardAmount { get; set; }

	public TotalPlaytimeResource(ChargenResource resource) : base(resource)
	{
		AwardInterval = TimeSpan.FromMinutes(resource.MinimumTimeBetweenAwards);
		AwardAmount = MaximumNumberAwardedPerAward;
	}

	public override void UpdateOnSave(ICharacter character, int oldMinutes, int newMinutes)
	{
		if (!character.IsPlayerCharacter || character.IsGuest)
		{
			return;
		}

		var oldMinutesTS = TimeSpan.FromMinutes(oldMinutes);
		var newMinutesTS = TimeSpan.FromMinutes(newMinutes);
		var intervalCount = newMinutesTS / MinimumTimeBetweenAwards - oldMinutesTS / MinimumTimeBetweenAwards;

		if (intervalCount > 0 &&
		    character.Account.AccountResources.ValueOrDefault(this, 0) < GetMaximum(character.Account))
		{
			character.Account.AccountResources[this] =
				character.Account.AccountResources.ValueOrDefault(this, 0) + (int)intervalCount;
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
}