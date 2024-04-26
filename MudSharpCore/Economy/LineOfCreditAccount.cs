using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Primitives;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Economy;

public class LineOfCreditAccount : SaveableItem, ILineOfCreditAccount
{
	public LineOfCreditAccount(IShop shop, string accountName, ICharacter accountOwner, decimal outstandingBalanceLimit)
	{
		Gameworld = shop.Gameworld;
		_shop = shop;
		AccountLimit = outstandingBalanceLimit;
		OutstandingBalance = 0.0M;
		AccountName = accountName;
		AccountOwnerId = accountOwner.Id;
		AccountOwnerName = accountOwner.CurrentName;
		IsSuspended = false;
		using (new FMDB())
		{
			var dbitem = new Models.LineOfCreditAccount
			{
				AccountName = accountName,
				AccountOwnerId = accountOwner.Id,
				AccountOwnerName = accountOwner.CurrentName.SaveToXml().ToString(),
				ShopId = shop.Id,
				OutstandingBalance = 0.0M,
				AccountLimit = outstandingBalanceLimit,
				IsSuspended = false
			};
			FMDB.Context.LineOfCreditAccounts.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;

			dbitem.AccountUsers.Add(new Models.LineOfCreditAccountUser
			{
				LineOfCreditAccount = dbitem,
				AccountUserId = accountOwner.Id,
				AccountUserName = accountOwner.CurrentName.SaveToXml().ToString(),
				SpendingLimit = null
			});
			FMDB.Context.SaveChanges();
		}

		_accountUsers.Add(new LineOfCreditAccountUser
			{ Id = accountOwner.Id, PersonalName = accountOwner.CurrentName });
	}

	public LineOfCreditAccount(Models.LineOfCreditAccount account, IShop shop, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = account.Id;
		_shop = shop;
		_accountName = account.AccountName;
		AccountOwnerId = account.AccountOwnerId;
		AccountOwnerName = new PersonalName(XElement.Parse(account.AccountOwnerName), gameworld);
		_accountLimit = account.AccountLimit;
		_outstandingBalance = account.OutstandingBalance;
		_isSuspended = account.IsSuspended;

		foreach (var item in account.AccountUsers)
		{
			_accountUsers.Add(new LineOfCreditAccountUser
			{
				Id = item.AccountUserId,
				PersonalName = new PersonalName(XElement.Parse(item.AccountUserName), gameworld),
				SpendingLimit = item.SpendingLimit
			});
		}
	}

	#region Overrides of FrameworkItem

	public override string FrameworkItemType => "LineOfCreditAccount";

	#endregion

	#region Overrides of SaveableItem

	public override void Save()
	{
		var dbitem = FMDB.Context.LineOfCreditAccounts.Find(Id);
		dbitem.AccountName = AccountName;
		dbitem.AccountOwnerId = AccountOwnerId;
		dbitem.IsSuspended = IsSuspended;
		dbitem.AccountLimit = AccountLimit;
		dbitem.OutstandingBalance = OutstandingBalance;
		FMDB.Context.LineOfCreditAccountUsers.RemoveRange(dbitem.AccountUsers);
		foreach (var user in _accountUsers)
		{
			dbitem.AccountUsers.Add(new Models.LineOfCreditAccountUser
			{
				AccountUserId = user.Id, AccountUserName = user.PersonalName.SaveToXml().ToString(),
				LineOfCreditAccount = dbitem, SpendingLimit = user.SpendingLimit
			});
		}

		Changed = false;
	}

	#endregion

	#region Implementation of ILineOfCreditAccount

	private IShop _shop;

	public ICurrency Currency => _shop.Currency;

	private string _accountName;

	public string AccountName
	{
		get => _accountName;
		init
		{
			_accountName = value;
			Changed = true;
		}
	}

	public LineOfCreditAuthorisationFailureReason IsAuthorisedToUse(ICharacter actor, decimal amount)
	{
		if (_accountUsers.All(x => x.Id != actor.Id))
		{
			return LineOfCreditAuthorisationFailureReason.NotAuthorisedAccountUser;
		}

		if (IsSuspended)
		{
			return LineOfCreditAuthorisationFailureReason.AccountSuspended;
		}

		if (amount > 0.0M)
		{
			if (OutstandingBalance + amount > AccountLimit)
			{
				return LineOfCreditAuthorisationFailureReason.AccountOverbalanced;
			}

			if (amount > _accountUsers.First(x => x.Id == actor.Id).SpendingLimit)
			{
				return LineOfCreditAuthorisationFailureReason.UserOverbalanced;
			}
		}

		return LineOfCreditAuthorisationFailureReason.None;
	}

	public decimal MaximumAuthorisedToUse(ICharacter actor)
	{
		return _accountUsers.First(x => x.Id == actor.Id).SpendingLimit ?? AccountLimit - OutstandingBalance;
	}

	public long AccountOwnerId { get; set; }

	public bool IsAccountOwner(ICharacter actor)
	{
		return actor.Id == AccountOwnerId;
	}

	public void SetAccountOwner(ICharacter actor)
	{
		AccountOwnerId = actor.Id;
		AccountOwnerName = actor.CurrentName;
		Changed = true;
	}

	public IPersonalName AccountOwnerName { get; set; }

	public void AddAuthorisation(ICharacter actor, decimal? spendingLimit)
	{
		_accountUsers.Add(new LineOfCreditAccountUser
		{
			Id = actor.Id,
			PersonalName = actor.CurrentName,
			SpendingLimit = spendingLimit
		});
		Changed = true;
	}

	public void RemoveAuthorisation(LineOfCreditAccountUser actor)
	{
		_accountUsers.Remove(actor);
		Changed = true;
	}

	public void SetLimit(LineOfCreditAccountUser user, decimal? spendingLimit)
	{
		user.SpendingLimit = spendingLimit;
		Changed = true;
	}

	public void SetAccountLimit(decimal spendingLimit)
	{
		AccountLimit = spendingLimit;
		Changed = true;
	}

	public void ChargeAccount(decimal amount)
	{
		OutstandingBalance += amount;
		Changed = true;
	}

	public void PayoffAccount(decimal amount)
	{
		OutstandingBalance -= amount;
		Changed = true;
	}

	private decimal _accountLimit;

	public decimal AccountLimit
	{
		get => _accountLimit;
		set
		{
			_accountLimit = value;
			Changed = true;
		}
	}

	private decimal _outstandingBalance;

	public decimal OutstandingBalance
	{
		get => _outstandingBalance;
		set
		{
			_outstandingBalance = value;
			Changed = true;
		}
	}

	private readonly List<LineOfCreditAccountUser> _accountUsers = new();
	public IEnumerable<LineOfCreditAccountUser> AccountUsers => _accountUsers;

	private bool _isSuspended;

	public bool IsSuspended
	{
		get => _isSuspended;
		set
		{
			_isSuspended = value;
			Changed = true;
		}
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Line of Credit Account {AccountName.ColourName()} for {_shop.Name.TitleCase().ColourName()}");
		sb.AppendLine($"Owner: {AccountOwnerName.GetName(NameStyle.FullName).ColourName()}");
		sb.AppendLine(
			$"Outstanding Balance Limit: {Currency.Describe(AccountLimit, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine(
			$"Current Outstanding Balance: {Currency.Describe(OutstandingBalance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
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

	#endregion
}