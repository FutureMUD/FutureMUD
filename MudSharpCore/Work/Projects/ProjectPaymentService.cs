using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace MudSharp.Work.Projects;

public static class ProjectPaymentService
{
	private static readonly object InMemoryLock = new();
	private static readonly List<ProjectPayable> InMemoryPayables = new();

	private static bool UseInMemoryPayables => string.IsNullOrWhiteSpace(FMDB.ConnectionString);

	public static void ClearInMemoryForTests()
	{
		lock (InMemoryLock)
		{
			InMemoryPayables.Clear();
		}
	}

	public static bool CanCreateCash(ICurrency currency, decimal amount, out string error)
	{
		error = string.Empty;
		if (amount <= 0.0M)
		{
			return true;
		}

		if (currency.FindCoinsForAmount(amount, out _).Any())
		{
			return true;
		}

		error = "The currency system could not produce a cash pile for that amount.";
		return false;
	}

	public static bool TryGiveCash(ICharacter actor, ICurrency currency, decimal amount, out string message)
	{
		message = string.Empty;
		if (amount <= 0.0M)
		{
			return true;
		}

		var coins = currency.FindCoinsForAmount(amount, out _);
		if (!coins.Any())
		{
			message = "The currency system could not produce a cash pile for that amount.";
			return false;
		}

		var pile = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency, coins);
		pile.SetOwner(actor);
		if (actor.Body?.CanGet(pile, 0) == true)
		{
			actor.Body.Get(pile, silent: true);
			return true;
		}

		if (actor.Location is not null)
		{
			pile.RoomLayer = actor.RoomLayer;
			actor.Location.Insert(pile, true);
			message = "You couldn't hold the cash, so it has been placed at your feet.";
			return true;
		}

		pile.Delete();
		message = "You couldn't receive the cash right now.";
		return false;
	}

	public static void CreateLabourPayable(IActiveProject project, ICharacter worker, IProjectLabourRequirement labour,
		ICurrency currency, decimal amount)
	{
		CreatePayable(project, worker, currency, amount, ProjectPayableType.Labour, labour.Id, labour.Name,
			$"Labour on {labour.Name} for project {project.Name}");
	}

	public static void CreateProjectRefundPayable(IActiveProject project, ICharacter owner, ICurrency currency,
		decimal amount)
	{
		CreatePayable(project, owner, currency, amount, ProjectPayableType.ProjectRefund, null, null,
			$"Unspent project funds returned from {project.Name}");
	}

	private static void CreatePayable(IActiveProject project, ICharacter recipient, ICurrency currency, decimal amount,
		ProjectPayableType type, long? labourRequirementId, string? requirementName, string reason)
	{
		if (amount <= 0.0M)
		{
			return;
		}

		var payable = new ProjectPayable
		{
			ActiveProjectId = project.Id,
			ProjectDefinitionId = project.ProjectDefinition.Id,
			ProjectRevisionNumber = project.ProjectDefinition.RevisionNumber,
			ProjectName = project.Name,
			ProjectOwnerCharacterId = CharacterInstanceIdentityComparer.IdentityId(project.CharacterOwner),
			CharacterId = CharacterInstanceIdentityComparer.IdentityId(recipient),
			CurrencyId = currency.Id,
			Amount = amount,
			PayableType = (int)type,
			ProjectLabourRequirementId = labourRequirementId,
			RequirementName = requirementName,
			Reason = reason,
			EarnedAt = DateTime.UtcNow
		};

		if (UseInMemoryPayables)
		{
			lock (InMemoryLock)
			{
				payable.Id = InMemoryPayables.Count + 1;
				InMemoryPayables.Add(payable);
			}

			return;
		}

		using (new FMDB())
		{
			FMDB.Context.ProjectPayables.Add(payable);
			FMDB.Context.SaveChanges();
		}
	}

	public static IReadOnlyList<ProjectPayable> OutstandingPayablesFor(ICharacter actor)
	{
		var characterId = CharacterInstanceIdentityComparer.IdentityId(actor);
		if (UseInMemoryPayables)
		{
			lock (InMemoryLock)
			{
				return InMemoryPayables
					.Where(x => x.CharacterId == characterId && x.ClaimedAt is null)
					.OrderBy(x => x.EarnedAt)
					.ThenBy(x => x.Id)
					.ToList();
			}
		}

		using (new FMDB())
		{
			return FMDB.Context.ProjectPayables
			           .AsNoTracking()
			           .Where(x => x.CharacterId == characterId && x.ClaimedAt == null)
			           .OrderBy(x => x.EarnedAt)
			           .ThenBy(x => x.Id)
			           .ToList();
		}
	}

	public static bool TryClaimOutstanding(ICharacter actor, IBankAccount? bankAccount, out string message)
	{
		if (bankAccount is not null)
		{
			return TryClaimOutstandingToBank(actor, bankAccount, out message);
		}

		return TryClaimOutstandingCash(actor, out message);
	}

	private static bool TryClaimOutstandingToBank(ICharacter actor, IBankAccount bankAccount, out string message)
	{
		if (!bankAccount.IsAccountOwner(actor))
		{
			message = "You are not the owner of that bank account.";
			return false;
		}

		if (bankAccount.AccountStatus != BankAccountStatus.Active)
		{
			message = $"Bank account {bankAccount.AccountReference} is {bankAccount.AccountStatus.DescribeEnum()} and cannot receive project payments.";
			return false;
		}

		var characterId = CharacterInstanceIdentityComparer.IdentityId(actor);
		if (UseInMemoryPayables)
		{
			lock (InMemoryLock)
			{
				var payables = InMemoryPayables
				               .Where(x => x.CharacterId == characterId && x.ClaimedAt is null &&
				                           x.CurrencyId == bankAccount.Currency.Id)
				               .OrderBy(x => x.EarnedAt)
				               .ThenBy(x => x.Id)
				               .ToList();
				return ClaimBankPayables(actor, bankAccount, payables, out message);
			}
		}

		using (new FMDB())
		{
			var payables = FMDB.Context.ProjectPayables
			                   .Where(x => x.CharacterId == characterId && x.ClaimedAt == null &&
			                               x.CurrencyId == bankAccount.Currency.Id)
			                   .OrderBy(x => x.EarnedAt)
			                   .ThenBy(x => x.Id)
			                   .ToList();
			var result = ClaimBankPayables(actor, bankAccount, payables, out message);
			FMDB.Context.SaveChanges();
			return result;
		}
	}

	private static bool ClaimBankPayables(ICharacter actor, IBankAccount bankAccount, List<ProjectPayable> payables,
		out string message)
	{
		if (!payables.Any())
		{
			message = $"You have no outstanding project payments in {bankAccount.Currency.Name} to deposit into {bankAccount.AccountReference}.";
			return false;
		}

		var total = payables.Sum(x => x.Amount);
		var reference = $"Project payment claim by {actor.PersonalName.GetName(NameStyle.FullName)}";
		bankAccount.DepositFromTransaction(total, reference);
		bankAccount.Bank.CurrencyReserves[bankAccount.Currency] += total;
		bankAccount.Bank.Changed = true;
		foreach (var payable in payables)
		{
			payable.ClaimedAt = DateTime.UtcNow;
			payable.ClaimedBankAccountId = bankAccount.Id;
		}

		message = $"You deposit {bankAccount.Currency.Describe(total, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} in project payments into {bankAccount.AccountReference.ColourName()}.";
		return true;
	}

	private static bool TryClaimOutstandingCash(ICharacter actor, out string message)
	{
		var characterId = CharacterInstanceIdentityComparer.IdentityId(actor);
		if (UseInMemoryPayables)
		{
			lock (InMemoryLock)
			{
				var payables = InMemoryPayables
				               .Where(x => x.CharacterId == characterId && x.ClaimedAt is null)
				               .OrderBy(x => x.EarnedAt)
				               .ThenBy(x => x.Id)
				               .ToList();
				return ClaimCashPayables(actor, payables, out message);
			}
		}

		using (new FMDB())
		{
			var payables = FMDB.Context.ProjectPayables
			                   .Where(x => x.CharacterId == characterId && x.ClaimedAt == null)
			                   .OrderBy(x => x.EarnedAt)
			                   .ThenBy(x => x.Id)
			                   .ToList();
			var result = ClaimCashPayables(actor, payables, out message);
			FMDB.Context.SaveChanges();
			return result;
		}
	}

	private static bool ClaimCashPayables(ICharacter actor, List<ProjectPayable> payables, out string message)
	{
		if (!payables.Any())
		{
			message = "You have no outstanding project payments to claim.";
			return false;
		}

		var messages = new List<string>();
		var claimedAny = false;
		foreach (var group in payables.GroupBy(x => x.CurrencyId).ToList())
		{
			var currency = actor.Gameworld.Currencies.Get(group.Key);
			if (currency is null)
			{
				messages.Add($"Project payments in missing currency #{group.Key:N0} could not be claimed.");
				continue;
			}

			var total = group.Sum(x => x.Amount);
			if (!TryGiveCash(actor, currency, total, out var cashMessage))
			{
				messages.Add($"{currency.Describe(total, CurrencyDescriptionPatternType.ShortDecimal)} could not be paid: {cashMessage}");
				continue;
			}

			foreach (var payable in group)
			{
				payable.ClaimedAt = DateTime.UtcNow;
			}

			claimedAny = true;
			messages.Add($"You collect {currency.Describe(total, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} in project payments.");
			if (!string.IsNullOrWhiteSpace(cashMessage))
			{
				messages.Add(cashMessage.ColourCommand());
			}
		}

		message = messages.ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n");
		return claimedAny;
	}
}
