﻿using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp.CharacterCreation.Resources;

public class RealtimeRegeneratingResource : ChargenResourceBase
{
	public double AwardAmount { get; set; }

	/// <inheritdoc />
	public RealtimeRegeneratingResource(IFuturemud gameworld, ChargenResource resource) : base(gameworld, resource)
	{
		AwardAmount = MaximumNumberAwardedPerAward;
	}

	/// <inheritdoc />
	public override void UpdateOnSave(ICharacter character, int oldMinutes, int newMinutes)
	{
		// Do nothing
	}

	/// <inheritdoc />
	public override void PerformPostLoadUpdate(ChargenResource resource, IFuturemud gameworld)
	{
		base.PerformPostLoadUpdate(resource, gameworld);
		gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += HeartbeatManagerOnFuzzyMinuteHeartbeat;
	}

	private void HeartbeatManagerOnFuzzyMinuteHeartbeat()
	{
		if (ControlProg?.ExecuteBool() == false)
		{
			return;
		}

		using (new FMDB())
		{
			var accounts = FMDB.Context.Accounts
			                   .Include(x => x.AccountsChargenResources)
			                   .ToList();
			var accountLookup = Futuremud.Games.First().Accounts.ToDictionary(x => x.Id);
			var now = DateTime.UtcNow;
			foreach (var dbaccount in accounts)
			{
				var resource = dbaccount.AccountsChargenResources.FirstOrDefault(x => x.ChargenResourceId == Id);
				if (resource is null)
				{
					resource = new Models.AccountsChargenResources
					{
						Account = dbaccount,
						ChargenResourceId = Id,
						LastAwardDate = DateTime.UtcNow,
						Amount = 0
					};
					dbaccount.AccountsChargenResources.Add(resource);
				}

				resource.Amount += AwardAmount;
				resource.LastAwardDate = now;
				var account = accountLookup[dbaccount.Id];
				account.AccountResources[this] += AwardAmount;
				account.AccountResourcesLastAwarded[this] = now;
			}

			FMDB.Context.SaveChanges();
		}
	}
}