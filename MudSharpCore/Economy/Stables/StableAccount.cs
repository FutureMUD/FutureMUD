using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.Economy.Stables;

public class StableAccount : SaveableItem, IStableAccount
{
	private readonly List<StableAccountUser> _accountUsers = new();
	private string _accountName;
	private IPersonalName _accountOwnerName;
	private decimal _balance;
	private decimal _creditLimit;
	private bool _isSuspended;

	public StableAccount(IStable stable, string accountName, ICharacter accountOwner, decimal creditLimit)
	{
		Gameworld = stable.Gameworld;
		Stable = stable;
		_accountName = accountName;
		AccountOwnerId = accountOwner.Id;
		_accountOwnerName = accountOwner.CurrentName;
		_creditLimit = creditLimit;
		_balance = 0.0M;

		using (new FMDB())
		{
			MudSharp.Models.StableAccount dbitem = new()
			{
				StableId = stable.Id,
				AccountName = accountName,
				AccountOwnerId = accountOwner.Id,
				AccountOwnerName = accountOwner.CurrentName.SaveToXml().ToString(),
				Balance = 0.0M,
				CreditLimit = creditLimit,
				IsSuspended = false
			};
			FMDB.Context.StableAccounts.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;

			dbitem.AccountUsers.Add(new MudSharp.Models.StableAccountUser
			{
				StableAccountId = dbitem.Id,
				AccountUserId = accountOwner.Id,
				AccountUserName = accountOwner.CurrentName.SaveToXml().ToString(),
				SpendingLimit = null
			});
			FMDB.Context.SaveChanges();
		}

		_accountUsers.Add(new StableAccountUser
		{
			Id = accountOwner.Id,
			PersonalName = accountOwner.CurrentName
		});
	}

	public StableAccount(MudSharp.Models.StableAccount account, IStable stable)
	{
		Gameworld = stable.Gameworld;
		Stable = stable;
		_id = account.Id;
		_accountName = account.AccountName;
		AccountOwnerId = account.AccountOwnerId;
		_accountOwnerName = new PersonalName(XElement.Parse(account.AccountOwnerName), Gameworld);
		_balance = account.Balance;
		_creditLimit = account.CreditLimit;
		_isSuspended = account.IsSuspended;

		foreach (var user in account.AccountUsers)
		{
			_accountUsers.Add(new StableAccountUser
			{
				Id = user.AccountUserId,
				PersonalName = new PersonalName(XElement.Parse(user.AccountUserName), Gameworld),
				SpendingLimit = user.SpendingLimit
			});
		}
	}

	public override string FrameworkItemType => "StableAccount";
	public IStable Stable { get; }
	public ICurrency Currency => Stable.Currency;
	public string AccountName => _accountName;
	public long AccountOwnerId { get; private set; }
	public IPersonalName AccountOwnerName => _accountOwnerName;
	public decimal Balance => _balance;
	public decimal CreditLimit => _creditLimit;
	public decimal CreditAvailable => Balance + CreditLimit;
	public IEnumerable<StableAccountUser> AccountUsers => _accountUsers;

	public bool IsSuspended
	{
		get => _isSuspended;
		set
		{
			_isSuspended = value;
			Changed = true;
		}
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.StableAccounts.Find(Id);
		if (dbitem is null)
		{
			Changed = false;
			return;
		}

		dbitem.AccountName = AccountName;
		dbitem.AccountOwnerId = AccountOwnerId;
		dbitem.AccountOwnerName = AccountOwnerName.SaveToXml().ToString();
		dbitem.Balance = Balance;
		dbitem.CreditLimit = CreditLimit;
		dbitem.IsSuspended = IsSuspended;
		FMDB.Context.StableAccountUsers.RemoveRange(dbitem.AccountUsers);
		foreach (var user in _accountUsers)
		{
			dbitem.AccountUsers.Add(new MudSharp.Models.StableAccountUser
			{
				StableAccountId = dbitem.Id,
				AccountUserId = user.Id,
				AccountUserName = user.PersonalName.SaveToXml().ToString(),
				SpendingLimit = user.SpendingLimit
			});
		}

		Changed = false;
	}

	public StableAccountAuthorisationFailureReason IsAuthorisedToUse(ICharacter actor, decimal amount)
	{
		var user = _accountUsers.FirstOrDefault(x => x.Id == actor.Id);
		if (user is null)
		{
			return StableAccountAuthorisationFailureReason.NotAuthorisedAccountUser;
		}

		if (IsSuspended)
		{
			return StableAccountAuthorisationFailureReason.AccountSuspended;
		}

		if (amount > 0.0M)
		{
			if (Balance - amount < -CreditLimit)
			{
				return StableAccountAuthorisationFailureReason.AccountOverbalanced;
			}

			if (user.SpendingLimit.HasValue && amount > user.SpendingLimit.Value)
			{
				return StableAccountAuthorisationFailureReason.UserOverbalanced;
			}
		}

		return StableAccountAuthorisationFailureReason.None;
	}

	public decimal MaximumAuthorisedToUse(ICharacter actor)
	{
		var user = _accountUsers.FirstOrDefault(x => x.Id == actor.Id);
		if (user is null || IsSuspended)
		{
			return 0.0M;
		}

		return Math.Max(0.0M, Math.Min(user.SpendingLimit ?? decimal.MaxValue, CreditAvailable));
	}

	public bool IsAccountOwner(ICharacter actor)
	{
		return actor.Id == AccountOwnerId;
	}

	public void SetAccountOwner(ICharacter actor)
	{
		AccountOwnerId = actor.Id;
		_accountOwnerName = actor.CurrentName;
		Changed = true;
	}

	public void AddAuthorisation(ICharacter actor, decimal? spendingLimit)
	{
		if (_accountUsers.Any(x => x.Id == actor.Id))
		{
			return;
		}

		_accountUsers.Add(new StableAccountUser
		{
			Id = actor.Id,
			PersonalName = actor.CurrentName,
			SpendingLimit = spendingLimit
		});
		Changed = true;
	}

	public void RemoveAuthorisation(StableAccountUser actor)
	{
		_accountUsers.Remove(actor);
		Changed = true;
	}

	public void SetLimit(StableAccountUser user, decimal? spendingLimit)
	{
		user.SpendingLimit = spendingLimit;
		Changed = true;
	}

	public void SetCreditLimit(decimal limit)
	{
		_creditLimit = limit;
		Changed = true;
	}

	public void ChargeAccount(decimal amount)
	{
		_balance -= amount;
		Changed = true;
	}

	public void PayAccount(decimal amount)
	{
		_balance += amount;
		Changed = true;
	}

	public string Show(ICharacter actor)
	{
		StringBuilder sb = new();
		sb.AppendLine($"Stable Account {AccountName.ColourName()} for {Stable.Name.TitleCase().ColourName()}");
		sb.AppendLine($"Owner: {AccountOwnerName.GetName(NameStyle.FullName).ColourName()}");
		sb.AppendLine($"Balance: {Currency.Describe(Balance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Credit Limit: {Currency.Describe(CreditLimit, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Credit Available: {Currency.Describe(CreditAvailable, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Suspended: {IsSuspended.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Authorised Account Users:");
		foreach (var user in _accountUsers)
		{
			sb.AppendLine(
				$"\t{user.PersonalName.GetName(NameStyle.FullName).ColourName()}{(!user.SpendingLimit.HasValue ? " [no limit]".ColourValue() : $" [{Currency.Describe(user.SpendingLimit.Value, CurrencyDescriptionPatternType.ShortDecimal)}]".ColourValue())}");
		}

		return sb.ToString();
	}
}
