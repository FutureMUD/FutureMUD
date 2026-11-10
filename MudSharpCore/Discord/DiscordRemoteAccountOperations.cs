#nullable enable

using System.Globalization;
using MudSharp.Accounts;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Database;
using Microsoft.EntityFrameworkCore;
using MudSharp.Framework;
using MudSharp.Network;
using MudSharp.PerceptionEngine;

namespace MudSharp.Discord;

internal sealed partial class DiscordRemoteService
{
	private string AddAccountNote(IAccount? actor, long? accountId, string accountName, string subject, string text)
	{
		var targetId = ResolveTargetAccountId(accountId, accountName);
		subject = subject.Trim();
		text = text.Trim();
		if (subject.Length is 0 or > 200 || text.Length is 0 or > 4000)
		{
			throw new ArgumentException("A note needs a subject (up to 200 characters) and text (up to 4,000 characters).");
		}

		using (new FMDB())
		{
			var note = new Models.AccountNote
			{
				AccountId = targetId,
				AuthorId = actor?.Id ?? throw new ArgumentException("Link a staff account before writing notes."),
				Subject = subject,
				Text = text,
				TimeStamp = DateTime.UtcNow
			};
			FMDB.Context.AccountNotes.Add(note);
			FMDB.Context.SaveChanges();
			return $"Staff note #{note.Id} was added to account #{targetId}.";
		}
	}

	private string AdjustAccountResource(IAccount? actor, long? accountId, string accountName,
		string resourceName, int? signedAmount, string reason)
	{
		if (actor is null)
		{
			throw new ArgumentException("Link a staff account before adjusting resources.");
		}
		var targetId = ResolveTargetAccountId(accountId, accountName);
		var amount = signedAmount ?? 1;
		if (amount == 0 || Math.Abs((long)amount) > 100000)
		{
			throw new ArgumentException("Resource amount must be between -100,000 and 100,000, excluding zero.");
		}

		reason = reason.Trim();
		if (reason.Length is 0 or > 4000)
		{
			throw new ArgumentException("Enter a reason of up to 4,000 characters.");
		}

		var resource = _world.ChargenResources.FirstOrDefault(x =>
			x.Name.Equals(resourceName, StringComparison.OrdinalIgnoreCase) ||
			x.Alias.Equals(resourceName, StringComparison.OrdinalIgnoreCase)) ??
			throw new ArgumentException("That chargen resource does not exist.");
		if ((actor.Authority?.Level ?? PermissionLevel.Guest) < resource.PermissionLevelRequiredToAward)
		{
			throw new ArgumentException($"You do not have permission to adjust {resource.PluralName}.");
		}

		var awarding = amount > 0;
		if (awarding && amount > resource.MaximumNumberAwardedPerAward)
		{
			throw new ArgumentException($"The maximum award per operation is {resource.MaximumNumberAwardedPerAward}.");
		}

		using (new FMDB())
		{
			var dbAccount = FMDB.Context.Accounts.FirstOrDefault(x => x.Id == targetId) ??
				throw new ArgumentException("That account no longer exists.");
			var balance = FMDB.Context.AccountsChargenResources.FirstOrDefault(x =>
				x.AccountId == dbAccount.Id && x.ChargenResourceId == resource.Id);
			if (awarding && balance is not null &&
			    DateTime.UtcNow - balance.LastAwardDate <= resource.MinimumTimeBetweenAwards &&
			    (actor.Authority?.Level ?? PermissionLevel.Guest) < resource.PermissionLevelRequiredToCircumventMinimumTime)
			{
				throw new ArgumentException($"That account was awarded {resource.PluralName} too recently.");
			}

			var createBalance = balance is null;
			balance ??= new Models.AccountsChargenResources
			{
				AccountId = dbAccount.Id,
				ChargenResourceId = resource.Id,
				Amount = 0
			};
			if (createBalance)
			{
				FMDB.Context.AccountsChargenResources.Add(balance);
			}
			balance.Amount += amount;
			balance.LastAwardDate = DateTime.UtcNow;

			var note = new Models.AccountNote
			{
				AccountId = dbAccount.Id,
				AuthorId = actor.Id,
				Subject = $"{(awarding ? "Award" : "Deduction")} of {Math.Abs(amount)} {(Math.Abs(amount) == 1 ? resource.Name : resource.PluralName)} by {actor.Name}",
				Text = reason,
				TimeStamp = balance.LastAwardDate
			};
			FMDB.Context.AccountNotes.Add(note);
			FMDB.Context.SaveChanges();

			var liveAccount = _world.TryAccount(dbAccount);
			liveAccount.AccountResources[resource] = balance.Amount;
			liveAccount.AccountResourcesLastAwarded[resource] = balance.LastAwardDate;
			var playerMessage = awarding ? resource.TextDisplayedToPlayerOnAward : resource.TextDisplayedToPlayerOnDeduct;
			if (liveAccount.ControllingContext is not null && !string.IsNullOrEmpty(playerMessage))
			{
				liveAccount.ControllingContext.Send("{0} {1}", "[System Message]".Colour(Telnet.Green), playerMessage);
			}

			_world.SystemMessage($"{actor.Name.Proper()} has {(awarding ? "awarded" : "deducted")} {Math.Abs(amount)} {resource.PluralName} {(awarding ? "to" : "from")} account {dbAccount.Name.Proper()}.", true);
			return $"Adjusted {resource.Name} for account {dbAccount.Name}: {balance.Amount.ToString("N2", CultureInfo.InvariantCulture)}.";
		}
	}

	private string BanAccount(IAccount? actor, long? accountId, string accountName, string reason)
	{
		if (actor is null)
		{
			throw new ArgumentException("Link a staff account before banning accounts.");
		}
		var targetId = ResolveTargetAccountId(accountId, accountName);
		reason = ValidateReason(reason);
		using (new FMDB())
		{
			var target = FMDB.Context.Accounts.FirstOrDefault(x => x.Id == targetId) ??
				throw new ArgumentException("That account no longer exists.");
			if (target.Id == actor.Id)
			{
				throw new ArgumentException("You cannot ban your own account.");
			}
			if (target.AuthorityGroup.AuthorityLevel >= (int)(actor.Authority?.Level ?? PermissionLevel.Guest))
			{
				throw new ArgumentException("You cannot ban an account with equal or higher authority.");
			}
			if ((AccountStatus)target.AccessStatus == AccountStatus.Suspended)
			{
				throw new ArgumentException("That account is already suspended.");
			}

			target.AccessStatus = (int)AccountStatus.Suspended;
			FMDB.Context.AccountNotes.Add(new Models.AccountNote
			{
				AccountId = target.Id,
				AuthorId = actor.Id,
				Subject = "Account Banned",
				Text = reason,
				TimeStamp = DateTime.UtcNow
			});
			FMDB.Context.SaveChanges();

			var liveAccount = _world.TryAccount(target);
			((Account)liveAccount).AccountStatus = AccountStatus.Suspended;
			lock (_world.Connections)
			{
				var connection = _world.Connections.FirstOrDefault(x => x.ControlPuppet?.Account == liveAccount);
				connection?.AddOutgoing($"Your account has been banned by {actor.Name.TitleCase()}.\n");
				connection?.PrepareOutgoing();
				connection?.SendOutgoing();
				if (connection is IAsyncPlayerConnection asyncConnection)
				{
					asyncConnection.RequestClose(ConnectionCloseMode.Drain);
				}
				else
				{
					connection?.Dispose();
				}
			}

			_world.SystemMessage($"{actor.Name.TitleCase()} has banned account {target.Name.TitleCase()}.", true);
			return $"Account {target.Name} was suspended.";
		}
	}

	private string UnbanAccount(IAccount? actor, long? accountId, string accountName, string reason)
	{
		if (actor is null)
		{
			throw new ArgumentException("Link a staff account before lifting bans.");
		}
		var targetId = ResolveTargetAccountId(accountId, accountName);
		reason = ValidateReason(reason);
		using (new FMDB())
		{
			var target = FMDB.Context.Accounts.FirstOrDefault(x => x.Id == targetId) ??
				throw new ArgumentException("That account no longer exists.");
			if ((AccountStatus)target.AccessStatus != AccountStatus.Suspended)
			{
				throw new ArgumentException("That account is not suspended.");
			}

			target.AccessStatus = (int)AccountStatus.Normal;
			FMDB.Context.AccountNotes.Add(new Models.AccountNote
			{
				AccountId = target.Id,
				AuthorId = actor.Id,
				Subject = "Account Suspension Lifted",
				Text = reason,
				TimeStamp = DateTime.UtcNow
			});
			FMDB.Context.SaveChanges();

			var liveAccount = _world.TryAccount(target);
			((Account)liveAccount).AccountStatus = AccountStatus.Normal;
			_world.SystemMessage($"{actor.Name.Proper()} has lifted the ban on account {target.Name.Proper()}.", true);
			return $"The suspension on account {target.Name} was lifted.";
		}
	}

	private long ResolveTargetAccountId(long? accountId, string accountName)
	{
		if (accountId is > 0 && string.IsNullOrWhiteSpace(accountName))
		{
			return accountId.Value;
		}

		if (accountId.HasValue || string.IsNullOrWhiteSpace(accountName))
		{
			throw new ArgumentException("Supply exactly one account ID or exact account name.");
		}

		accountName = accountName.Trim();
		if (TryParseAccountId(accountName, out var numericId))
		{
			return numericId;
		}

		using (new FMDB())
		{
			return FMDB.Context.Accounts.AsNoTracking()
				.Where(x => x.Name == accountName)
				.Select(x => (long?)x.Id)
				.FirstOrDefault() ?? throw new ArgumentException("That account does not exist.");
		}
	}

	internal static bool TryParseAccountId(string value, out long accountId)
	{
		return long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out accountId) && accountId > 0;
	}

	private static string ValidateReason(string reason)
	{
		reason = reason.Trim();
		return reason.Length is 0 or > 4000
			? throw new ArgumentException("Enter a reason of up to 4,000 characters.")
			: reason;
	}
}
