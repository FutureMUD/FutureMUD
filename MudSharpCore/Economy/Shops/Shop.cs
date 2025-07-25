﻿using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MudSharp.Framework;
using System.Threading.Tasks;
using System.Xml.Linq;
using JetBrains.Annotations;
using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Effects.Concrete;
using MudSharp.Economy.Payment;
using MudSharp.GameItems.Prototypes;
using MudSharp.RPG.Law;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems.Interfaces;
using MudSharp.Events;
using MudSharp.Models;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using System.Security.Cryptography;
using System.Numerics;

namespace MudSharp.Economy.Shops;

public abstract class Shop : SaveableItem, IShop
{
	public static void DoAutopayShopTaxes(IFuturemud gameworld)
	{
		foreach (var shop in gameworld.Shops)
		{
			if (!shop.AutoPayTaxes)
			{
				continue;
			}

			var owed = shop.EconomicZone.OutstandingTaxesForShop(shop);
			if (owed <= 0.0M)
			{
				continue;
			}

			var available = shop.AvailableCashFromAllSources();
			if (available > owed)
			{
				shop.TakeCashFromAllSources(owed, "Automatically paying taxes");
				shop.EconomicZone.PayTaxesForShop(shop, owed);
				continue;
			}

			shop.TakeCashFromAllSources(available, "Automatically paying taxes");
			shop.EconomicZone.PayTaxesForShop(shop, owed);
		}
	}

	public static IShop LoadShop(Models.Shop shop, IFuturemud gameworld)
	{
		switch (shop.ShopType)
		{
			case "Permanent":
				return new PermanentShop(shop, gameworld);
			case "Transient":
				return new TransientShop(shop, gameworld);
			default:
				throw new ApplicationException($"Invalid shop type {shop.ShopType} for shop {shop.Id} ({shop.Name})");
		}
	}

	protected Shop(IEconomicZone zone, ICell originalShopFront, string name, string type)
	{
		Gameworld = zone.Gameworld;
		Currency = zone.Currency;
		_name = name;
		EconomicZone = zone;
		IsTrading = true;
		AutoPayTaxes = true;
		using (new FMDB())
		{
			var dbitem = new Models.Shop();
			FMDB.Context.Shops.Add(dbitem);
			dbitem.Name = name;
			dbitem.CurrencyId = Currency.Id;
			dbitem.EconomicZoneId = zone.Id;
			dbitem.IsTrading = true;
			dbitem.ShopType = type;
			dbitem.AutopayTaxes = true;
			dbitem.EmployeeRecords = "<Employees/>";
			if (originalShopFront is not null)
			{
				dbitem.ShopsStoreroomCells.Add(new ShopsStoreroomCell { Shop = dbitem, CellId = originalShopFront.Id });
			}
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	protected Shop(Models.Shop shop, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = shop.Id;
		_name = shop.Name;
		_economicZone = gameworld.EconomicZones.Get(shop.EconomicZoneId);
		_cashBalance = shop.CashBalance;
		_expectedCashBalance = shop.ExpectedCashBalance;
		MinimumFloatToBuyItems = shop.MinimumFloatToBuyItems;
		IsTrading = shop.IsTrading;
		Currency = gameworld.Currencies.Get(shop.CurrencyId);
		MarketForPricingPurposes = gameworld.Markets.Get(shop.MarketId ?? 0);
		AutoPayTaxes = shop.AutopayTaxes;
		_canShopProg = gameworld.FutureProgs.Get(shop.CanShopProgId ?? 0);
		_whyCannotShopProg = gameworld.FutureProgs.Get(shop.WhyCannotShopProgId ?? 0);

		_bankAccountId = shop.BankAccountId;
		foreach (var item in shop.Merchandises)
		{
			_merchandises.Add(new Merchandise(item, this, gameworld));
		}

		foreach (var item in shop.ShopTransactionRecords)
		{
			_transactionRecords.Add(new TransactionRecord(item, this, Gameworld));
		}

		var employees = XElement.Parse(shop.EmployeeRecords);
		foreach (var item in employees.Elements())
		{
			_employeeRecords.Add(new EmployeeRecord(item, gameworld));
		}

		foreach (var item in shop.LineOfCreditAccounts)
		{
			_lineOfCreditAccounts.Add(new LineOfCreditAccount(item, this, Gameworld));
		}
	}

	protected virtual void InitialiseShop()
	{
	}

	public virtual void PostLoadInitialisation()
	{
	}

	private IEconomicZone _economicZone;
	private ICurrency _currency;
	private IFutureProg _canShopProg;
	private IFutureProg _whyCannotShopProg;
	private long? _bankAccountId;
	private IBankAccount _bankAccount;
	private decimal _cashBalance;
	private decimal _expectedCashBalance;

	protected abstract void Save(Models.Shop dbitem);

	public sealed override void Save()
	{
		var dbitem = FMDB.Context.Shops.Find(Id);
		dbitem.Name = _name;
		dbitem.CashBalance = _cashBalance;
		dbitem.ExpectedCashBalance = _expectedCashBalance;
		dbitem.BankAccountId = _bankAccountId;
		dbitem.CurrencyId = Currency.Id;
		dbitem.EconomicZoneId = EconomicZone.Id;
		dbitem.IsTrading = IsTrading;
		dbitem.CanShopProgId = CanShopProg?.Id;
		dbitem.WhyCannotShopProgId = WhyCannotShopProg?.Id;
		dbitem.MinimumFloatToBuyItems = MinimumFloatToBuyItems;
		dbitem.MarketId = MarketForPricingPurposes?.Id;
		dbitem.AutopayTaxes = AutoPayTaxes;
		dbitem.EmployeeRecords = new XElement("Employees",
			from employee in EmployeeRecords
			select employee.SaveToXml()
		).ToString();
		Save(dbitem);
		Changed = false;
	}

	public override string FrameworkItemType => "Shop";

	public IEconomicZone EconomicZone
	{
		get => _economicZone;
		set
		{
			var old = _economicZone;
			_economicZone = value;
			Changed = true;
			if (old is not null && old != value)
			{
				var owed = old.OutstandingTaxesForShop(this);
				if (owed > 0.0M)
				{
					var available = AvailableCashFromAllSources();
					if (available > owed)
					{
						TakeCashFromAllSources(owed, "Paying taxes due to change of economic zone");
						old.PayTaxesForShop(this, owed);
					}
					else
					{
						TakeCashFromAllSources(available, "Paying taxes due to change of economic zone");
						old.PayTaxesForShop(this, owed);
						old.ForgiveTaxesForShop(this);
					}
				}
			}

			if (old is not null && value is not null && old != value)
			{
				MarketForPricingPurposes = null;
				if (old.Currency != value.Currency)
				{
					CashBalance *= old.Currency.BaseCurrencyToGlobalBaseCurrencyConversion / value.Currency.BaseCurrencyToGlobalBaseCurrencyConversion;
					ExpectedCashBalance *= old.Currency.BaseCurrencyToGlobalBaseCurrencyConversion / value.Currency.BaseCurrencyToGlobalBaseCurrencyConversion;
					MinimumFloatToBuyItems *= old.Currency.BaseCurrencyToGlobalBaseCurrencyConversion / value.Currency.BaseCurrencyToGlobalBaseCurrencyConversion;
					foreach (var merch in Merchandises)
					{
						merch.ShopCurrencyChanged(old.Currency, value.Currency);
					}
				}
			}
		}
	}

	public ICurrency Currency
	{
		get => _currency;
		set
		{
			_currency = value;
			Changed = true;
		}
	}

	private readonly List<ITransactionRecord> _transactionRecords = new();
	public IEnumerable<ITransactionRecord> TransactionRecords => _transactionRecords;
	public IMarket MarketForPricingPurposes { get; private set; }

	public decimal CashBalance
	{
		get => _cashBalance;
		set
		{
			_cashBalance = value;
			Changed = true;
		}
	}

	public decimal ExpectedCashBalance
	{
		get => _expectedCashBalance;
		set
		{
			_expectedCashBalance = value;
			Changed = true;
		}
	}

	public decimal MinimumFloatToBuyItems { get; set; }

	public bool AutoPayTaxes { get; private set; }

	public IBankAccount BankAccount
	{
		get
		{
			if (_bankAccount is null && _bankAccountId is not null)
			{
				_bankAccount = Gameworld.BankAccounts.Get(_bankAccountId.Value);
			}

			return _bankAccount;
		}
		set
		{
			_bankAccount = value;
			_bankAccountId = value?.Id;
			Changed = true;
		}
	}


	protected readonly CollectionDictionary<IMerchandise, long> _stockedMerchandise = new();
	protected readonly Counter<IMerchandise> _stockedMerchandiseCounts = new();

	public bool IsTrading { get; protected set; }
	public void ToggleIsTrading()
	{
		IsTrading = !IsTrading;
		Changed = true;
	}

	public abstract bool IsReadyToDoBusiness { get; }

	private readonly List<IEmployeeRecord> _employeeRecords = new();
	public IEnumerable<IEmployeeRecord> EmployeeRecords => _employeeRecords;
	public abstract IEnumerable<ICell> CurrentLocations { get; }
	public bool IsEmployee(ICharacter actor)
	{
		return _employeeRecords.Any(x => x.EmployeeCharacterId == actor.Id);
	}

	public bool IsManager(ICharacter actor)
	{
		return _employeeRecords.Any(x => x.EmployeeCharacterId == actor.Id && x.IsManager);
	}

	public bool IsProprietor(ICharacter actor)
	{
		return _employeeRecords.Any(x => x.EmployeeCharacterId == actor.Id && x.IsProprietor);
	}

	public void SetManager(ICharacter actor, bool isManager)
	{
		_employeeRecords.First(x => x.EmployeeCharacterId == actor.Id).IsManager = isManager;
		Changed = true;
	}

	public void SetProprietor(ICharacter actor, bool isProprietor)
	{
		_employeeRecords.First(x => x.EmployeeCharacterId == actor.Id).IsProprietor = isProprietor;
		Changed = true;
	}

	public void ClearEmployees()
	{
		_employeeRecords.Clear();
		Changed = true;
	}

	public bool IsClockedIn(ICharacter actor)
	{
		return _employeeRecords.Any(x => x.EmployeeCharacterId == actor.Id && x.ClockedIn);
	}

	public IEnumerable<ICharacter> EmployeesOnDuty =>
		Gameworld.Actors.Where(x => IsClockedIn(x));

	protected readonly List<IMerchandise> _merchandises = new();
	public IEnumerable<IMerchandise> Merchandises => _merchandises;

	public IFutureProg CanShopProg
	{
		get => _canShopProg;
		set
		{
			_canShopProg = value;
			Changed = true;
		}
	}

	public IFutureProg WhyCannotShopProg
	{
		get => _whyCannotShopProg;
		set
		{
			_whyCannotShopProg = value;
			Changed = true;
		}
	}

	public void AddEmployee(ICharacter actor)
	{
		if (IsEmployee(actor))
		{
			return;
		}

		_employeeRecords.Add(new EmployeeRecord(actor));
		Changed = true;
	}

	public void RemoveEmployee(IEmployeeRecord employee)
	{
		_employeeRecords.Remove(employee);
		Changed = true;
	}

	public void RemoveEmployee(ICharacter actor)
	{
		var record = _employeeRecords.FirstOrDefault(x => x.EmployeeCharacterId == actor.Id);
		if (record != null)
		{
			RemoveEmployee(record);
		}
	}

	public void EmployeeClockIn(ICharacter actor)
	{
		_employeeRecords.First(x => x.EmployeeCharacterId == actor.Id).ClockedIn = true;
		Changed = true;
	}

	public void EmployeeClockOut(ICharacter actor)
	{
		_employeeRecords.First(x => x.EmployeeCharacterId == actor.Id).ClockedIn = false;
		Changed = true;
	}

	public void AddMerchandise(IMerchandise merchandise)
	{
		_merchandises.Add(merchandise);
		Changed = true;
	}

	public void RemoveMerchandise(IMerchandise merchandise)
	{
		_merchandises.Remove(merchandise);
		_stockedMerchandise.Remove(merchandise);
		_stockedMerchandiseCounts.Remove(merchandise);
		merchandise.Delete();
		foreach (var record in TransactionRecords)
		{
			if (record.Merchandise == merchandise)
			{
				record.Merchandise = null;
			}
		}
		Changed = true;
	}

	public void AddTransaction(ITransactionRecord record)
	{
		_transactionRecords.Add(record);
		Changed = true;
	}

	public void AddToStock(ICharacter actor, IGameItem item, IMerchandise merch)
	{
		item.AddEffect(new ItemOnDisplayInShop(item, this, merch));
		actor?.OutputHandler.Send(
			$"You add {item.HowSeen(actor)} to the for-sale inventory of {Name.TitleCase().Colour(Telnet.Cyan)}.");
		AddTransaction(new TransactionRecord(ShopTransactionType.Stock, Currency, this,
			EconomicZone.ZoneForTimePurposes.DateTime(), actor,
			merch.EffectivePrice * item.Quantity * -1, 0.0M, merch));
		_stockedMerchandise.Add(merch, item.Id);
		_stockedMerchandiseCounts.Add(merch, item.Quantity);
	}

	public void DisposeFromStock(ICharacter actor, IGameItem item)
	{
		item.RemoveAllEffects<ItemOnDisplayInShop>(fireRemovalAction: true);
		actor?.OutputHandler.Send(
			$"You dispose of {item.HowSeen(actor)} from the for-sale inventory of {Name.TitleCase().Colour(Telnet.Cyan)}.");
		var merch =
			_merchandises.FirstOrDefault(x => x.IsMerchandiseFor(item)) ??
			_merchandises.FirstOrDefault(x => x.IsMerchandiseFor(item, true));
		AddTransaction(new TransactionRecord(ShopTransactionType.StockLoss, Currency, this,
			EconomicZone.ZoneForTimePurposes.DateTime(), actor,
			merch?.EffectivePrice ?? 0.0M * item.Quantity, 0.0M, merch));
		if (merch != null)
		{
			_stockedMerchandise.Remove(merch, item.Id);
			_stockedMerchandiseCounts.Add(merch, item.Quantity * -1);
		}
	}

	private readonly List<ILineOfCreditAccount> _lineOfCreditAccounts = new();
	public IEnumerable<ILineOfCreditAccount> LineOfCreditAccounts => _lineOfCreditAccounts;

	public void AddLineOfCredit(ILineOfCreditAccount account)
	{
		_lineOfCreditAccounts.Add(account);
		Changed = true;
	}

	public void RemoveLineOfCredit(ILineOfCreditAccount account)
	{
		_lineOfCreditAccounts.Remove(account);
		Changed = true;
	}

	public abstract (bool Truth, string Reason) CanSellInternal(ICharacter actor, IMerchandise merchandise,
		IPaymentMethod method,
		IGameItem item);

	public (bool Truth, string Reason) CanSell(ICharacter actor, IMerchandise merchandise, IPaymentMethod method,
		IGameItem item)
	{
		if (!IsTrading && !actor.IsAdministrator())
		{
			return (false, "the store is currently closed");
		}

		if (CanShopProg?.ExecuteBool(actor, merchandise?.Item.Id ?? 0L,
				merchandise?.Item.Tags.Select(x => x.Name)) == false)
		{
			return (false,
				WhyCannotShopProg.Execute(actor, merchandise?.Item.Id ?? 0L, merchandise.Item.Tags.Select(x => x.Name))
								 ?.ToString() ?? "of an unknown reason");
		}

		var price = merchandise!.EffectivePrice * merchandise.BaseBuyModifier * item.Quantity;

		switch (method)
		{
			case null:
				break;
			case ShopCashPayment cash:
				var cashonhand = cash.AccessibleMoneyForCredit();
				if (cashonhand < price)
				{
					return (false, $"the shop does not have enough cash on hand to pay the sale price of that item.");
				}

				if (cashonhand < MinimumFloatToBuyItems)
				{
					return (false, "the shop is not currently buying items due to insufficient cash on hand.");
				}

				break;
			case LineOfCreditPayment loc:
				switch (loc.Account.IsAuthorisedToUse(actor, 0.0M))
				{
					case LineOfCreditAuthorisationFailureReason.NotAuthorisedAccountUser:
						return (false, "you are not an authorised user of that line of credit account.");
					case LineOfCreditAuthorisationFailureReason.AccountSuspended:
						return (false, "that account has been temporarily suspended.");
				}

				break;
			case BankPayment bp:
				if (bp.Item.BankAccount is null)
				{
					return (false, $"{bp.Item.Parent.HowSeen(actor)} is not tied to an actual bank account.");
				}

				if (!bp.Item.BankAccount.IsAuthorisedPaymentItem(bp.Item))
				{
					return (false, $"{bp.Item.Parent.HowSeen(actor)} is no longer valid for payment.");
				}

				if (bp.Currency != Currency)
				{
					return (false, $"{bp.Item.Parent.HowSeen(actor)} is for a different currency than this shop uses.");
				}

				var cashinbank = bp.AccessibleMoneyForCredit();
				if (cashinbank < price)
				{
					return (false,
						$"the shop does not have enough balance in their bank account to pay the sale price of that item.");
				}

				if (cashinbank < MinimumFloatToBuyItems)
				{
					return (false, "the shop is not currently buying items due to insufficient cash in their bank account.");
				}

				break;
		}

		var stockedItems = StockedItems(merchandise).ToList();
		var stockedQuantity = stockedItems.Sum(x => x.Quantity);
		if (merchandise.MaximumStockLevelsToBuy > 0 && stockedQuantity + item.Quantity > merchandise.MaximumStockLevelsToBuy)
		{
			var maxQuantity = merchandise.MaximumStockLevelsToBuy - stockedQuantity;
			if (maxQuantity <= 0)
			{
				return (false, "the shop already has the maximum amount of those items that it will buy.");
			}

			return (false, $"the shop will buy at most {maxQuantity.ToString("N0", actor).ColourValue()} of those items.");
		}

		var baseReason = CanSellInternal(actor, merchandise, method, item);
		if (!baseReason.Truth)
		{
			return baseReason;
		}

		return (true, string.Empty);
	}

	public void Sell(ICharacter actor, IMerchandise merchandise, IPaymentMethod method, IGameItem item)
	{
		item.AddEffect(new ItemOnDisplayInShop(item, this, merchandise));
		var price = merchandise.EffectivePrice * merchandise.BaseBuyModifier * item.Quantity;
		actor.OutputHandler.Handle(new EmoteOutput(new Emote($"@ sell|sells $1 to {Name.TitleCase().ColourName()} for $2.", actor, actor, item, new DummyPerceivable(Currency.Describe(price, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()))));
		AddTransaction(new TransactionRecord(ShopTransactionType.Purchase, Currency, this,
			EconomicZone.ZoneForTimePurposes.DateTime(), actor,
			 price * -1, 0.0M, merchandise));
		_stockedMerchandise.Add(merchandise, item.Id);
		_stockedMerchandiseCounts.Add(merchandise, item.Quantity);
		item.InInventoryOf?.Take(item);
		item.Location?.Extract(item);
		SortItemToStorePhysicalLocation(item, merchandise, null);
                method.GivePayment(price);
                CrimeExtensions.CheckPossibleCrimeAllAuthorities(actor, CrimeTypes.SellingContraband, null, item, "");
        }

	protected abstract (bool Truth, string Reason) CanBuyInternal(ICharacter actor, IMerchandise merchandise, int quantity,
		IPaymentMethod method, string extraArguments = null);


	public (bool Truth, string Reason) CanBuy(ICharacter actor, IMerchandise merchandise, int quantity,
		IPaymentMethod method, string extraArguments = null)
	{
		if (!IsTrading && !actor.IsAdministrator())
		{
			return (false, "the store is currently closed");
		}

		if (CanShopProg?.ExecuteBool(actor, merchandise?.Item.Id ?? 0L,
				merchandise?.Item.Tags.Select(x => x.Name)) == false)
		{
			return (false,
				WhyCannotShopProg.Execute(actor, merchandise?.Item.Id ?? 0L, merchandise.Item.Tags.Select(x => x.Name))
								 ?.ToString() ?? "of an unknown reason");
		}

		var stockedItems = StockedItems(merchandise).ToList();

		if (!stockedItems.Any())
		{
			return (false, "the store is currently out of stock of that item.");
		}

		if (!string.IsNullOrEmpty(extraArguments))
		{
			var specificStockedItems =
				stockedItems.Where(x => x.HasKeywords(extraArguments.Split('.'), actor, true)).ToList();
			if (!specificStockedItems.Any())
			{
				return (false, "the store does not stock any of that item with the specified keywords.");
			}

			if (specificStockedItems.Sum(x => x.Quantity) < quantity)
			{
				return (false,
					$"the store only has {specificStockedItems.Sum(x => x.Quantity)} of that specified variant of that item in stock.");
			}
		}
		else
		{
			if (stockedItems.Sum(x => x.Quantity) < quantity)
			{
				return (false, $"the store only has {stockedItems.Sum(x => x.Quantity)} of that item in stock.");
			}
		}

		var price = 0.0M;

		switch (method)
		{
			case null:
				break;
			case ShopCashPayment cash:
				price = PriceForMerchandise(actor, merchandise, quantity);
				var cashonhand = cash.AccessibleMoneyForPayment();
				if (cashonhand < price)
				{
					return (false,
						$"you only have {Currency.Describe(cashonhand, CurrencyDescriptionPatternType.Short).ColourValue()} cash on hand, but you need {Currency.Describe(price, CurrencyDescriptionPatternType.Short).ColourValue()}.");
				}

				break;
			case LineOfCreditPayment loc:
				price = PriceForMerchandise(
					loc.Account.IsAccountOwner(actor)
						? actor
						: Gameworld.TryGetCharacter(loc.Account.AccountOwnerId, true), merchandise, quantity);
				switch (loc.Account.IsAuthorisedToUse(actor, price))
				{
					case LineOfCreditAuthorisationFailureReason.NotAuthorisedAccountUser:
						return (false, "you are not an authorised user of that line of credit account.");
					case LineOfCreditAuthorisationFailureReason.AccountOverbalanced:
						return (false,
							$"that purchase would overdraw the account. You can only draw down {Currency.Describe(loc.Account.MaximumAuthorisedToUse(actor), CurrencyDescriptionPatternType.Short).ColourValue()}.");
					case LineOfCreditAuthorisationFailureReason.UserOverbalanced:
						return (false,
							$"that purchase would exceed your allowance for that account. You can only draw down {Currency.Describe(loc.Account.MaximumAuthorisedToUse(actor), CurrencyDescriptionPatternType.Short).ColourValue()}.");
					case LineOfCreditAuthorisationFailureReason.AccountSuspended:
						return (false, "that account has been temporarily suspended.");
				}

				break;
			case BankPayment bp:
				if (bp.Item.BankAccount is null)
				{
					return (false, $"{bp.Item.Parent.HowSeen(actor)} is not tied to an actual bank account.");
				}

				price = PriceForMerchandise(actor, merchandise, quantity);
				if (!bp.Item.BankAccount.IsAuthorisedPaymentItem(bp.Item))
				{
					return (false, $"{bp.Item.Parent.HowSeen(actor)} is no longer valid for payment.");
				}

				if (bp.Currency != Currency)
				{
					return (false, $"{bp.Item.Parent.HowSeen(actor)} is for a different currency than this shop uses.");
				}

				var cashinbank = bp.AccessibleMoneyForPayment();
				if (cashinbank < price)
				{
					return (false,
						$"you only have {Currency.Describe(cashinbank, CurrencyDescriptionPatternType.Short).ColourValue()} available in your account, but you need {Currency.Describe(price, CurrencyDescriptionPatternType.Short).ColourValue()}.");
				}

				break;
		}

		var baseReason = CanBuyInternal(actor, merchandise, quantity, method, extraArguments);
		if (!baseReason.Truth)
		{
			return baseReason;
		}

		return (true, string.Empty);
	}

	public IEnumerable<(IGameItem Item, IMerchandise Merchandise, decimal Price)> AllMerchandiseForVirtualShoppers
	{
		get
		{
			var items = new List<(IGameItem Item, IMerchandise Merchandise, decimal Price)>();
			var allItems = AllStockedItems.ToList();
			foreach (var item in allItems)
			{
				var merch = item.EffectsOfType<ItemOnDisplayInShop>().First().Merchandise;
				var price = PriceForMerchandise(null, merch, 1);
				items.Add((item, merch, price));
			}

			return items;
		}
	}

	public void BuyVirtualShopper(IMerchandise merchandise, IGameItem item, int quantity)
	{
		var restockInfo = new List<(IGameItem Item, IGameItem Container)>();
		if (item.DropsWhole(quantity))
		{
			restockInfo.Add((item, item.ContainedIn));
			item.RemoveAllEffects<ItemOnDisplayInShop>(fireRemovalAction: true);
			item.InInventoryOf?.Take(item);
			item.ContainedIn?.Take(item);
			item.Location?.Extract(item);

			_stockedMerchandise.Remove(merchandise, item.Id);
			item.Delete();
		}
		else
		{
			item.GetItemType<IStackable>().Quantity -= quantity;
		}
		
		var (price, tax) = PriceAndTaxForMerchandise(null, merchandise, quantity);
		AddCurrencyToShop(CurrencyGameItemComponentProto.CreateNewCurrencyPile(Currency, Currency.FindCoinsForAmount(price, out _)));
		AddTransaction(new TransactionRecord(ShopTransactionType.Sale, Currency, this, CurrentLocations.First().DateTime(), null, price - tax, tax, merchandise));
		EconomicZone.ReportSalesTaxCollected(this, tax);
		
		RecalculateStockedItems(merchandise, quantity);
		if (merchandise.AutoReordering && _stockedMerchandiseCounts[merchandise] < merchandise.MinimumStockLevels)
		{
			DoAutoRestockForMerchandise(merchandise, restockInfo);
		}

		foreach (var employee in EmployeesOnDuty)
		{
			if (employee.HandleEvent(EventType.ItemRequiresRestocking, employee, this, merchandise, quantity))
			{
				break;
			}
		}
	}

	public IEnumerable<IGameItem> Buy(ICharacter actor, IMerchandise merchandise, int quantity, IPaymentMethod method,
		string extraArguments = null)
	{
		var stockedItems = StockedItems(merchandise).ToList();
		if (!string.IsNullOrEmpty(extraArguments))
		{
			stockedItems = stockedItems.Where(x => x.HasKeywords(extraArguments.Split('.'), actor, true)).ToList();
		}

		var boughtItems = new List<IGameItem>();
		var quantitySought = quantity;
		foreach (var item in stockedItems)
		{
			if (item.DropsWhole(quantitySought))
			{
				boughtItems.Add(item);
				quantitySought -= item.Quantity;
			}
			else
			{
				var newItem = item.Get(null, quantitySought);
				boughtItems.Add(newItem);
				quantitySought = 0;
			}

			if (quantitySought <= 0)
			{
				break;
			}
		}

		var restockInfo = boughtItems.Select(x => (Item: x, Container: x.ContainedIn)).ToList();
		foreach (var item in boughtItems)
		{
			item.RemoveAllEffects<ItemOnDisplayInShop>(fireRemovalAction: true);
			item.InInventoryOf?.Take(item);
			item.ContainedIn?.Take(item);
			item.Location?.Extract(item);
			_stockedMerchandise.Remove(merchandise, item.Id);
		}

		RecalculateStockedItems(merchandise, quantity);
		decimal price = 0.0M, tax = 0.0M;
		switch (method)
		{
			case ShopCashPayment cash:
				(price, tax) = PriceAndTaxForMerchandise(actor, merchandise, quantity);
				ExpectedCashBalance += price;
				break;
			case LineOfCreditPayment loc:
				(price, tax) = PriceAndTaxForMerchandise(
					loc.Account.IsAccountOwner(actor)
						? actor
						: Gameworld.TryGetCharacter(loc.Account.AccountOwnerId, true), merchandise, quantity);
				break;
			case BankPayment bp:
				(price, tax) = PriceAndTaxForMerchandise(actor, merchandise, quantity);
				break;
		}

		method.TakePayment(price);
		AddTransaction(new TransactionRecord(ShopTransactionType.Sale, Currency, this,
			actor.Location.DateTime(), actor, price - tax, tax, merchandise));
		EconomicZone.ReportSalesTaxCollected(this, tax);

		var couldnothold = false;
		foreach (var item in boughtItems)
		{
			if (actor.Body.CanGet(item, 0))
			{
				actor.Body.Get(item, silent: true);
			}
			else
			{
				actor.Location.Insert(item);
				couldnothold = true;
			}
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote($"$0 bought $1 from the store for $2.", actor, actor,
			new PerceivableGroup(boughtItems),
			new DummyPerceivable(Currency.Describe(price, CurrencyDescriptionPatternType.Short).ColourValue()))));
		if (couldnothold)
		{
			actor.OutputHandler.Send(
				"Warning: You could not hold all of the items, and some of them were placed on the ground.".Colour(
					Telnet.Red));
		}

		actor.HandleEvent(EventType.BuyItemInShop, actor, this, merchandise, boughtItems);
		foreach (var witness in actor.Location.EventHandlers)
		{
			witness.HandleEvent(EventType.WitnessBuyItemInShop, actor, witness, this, merchandise, boughtItems);
		}

		if (merchandise.AutoReordering && _stockedMerchandiseCounts[merchandise] < merchandise.MinimumStockLevels)
		{
			DoAutoRestockForMerchandise(merchandise, restockInfo);
		}

		foreach (var employee in EmployeesOnDuty)
		{
			if (employee.HandleEvent(EventType.ItemRequiresRestocking, employee, this, merchandise, quantity))
			{
				break;
			}
		}

		return boughtItems;
	}

	public abstract IEnumerable<IGameItem> DoAutoRestockForMerchandise(IMerchandise merchandise,
		List<(IGameItem Item, IGameItem Container)> purchasedItems = null);

	public abstract void SortItemToStorePhysicalLocation(IGameItem item, IMerchandise merchandise, IGameItem container);

	public abstract IEnumerable<IGameItem> DoAutostockForMerchandise(IMerchandise merchandise);

	public decimal AvailableCashFromAllSources()
	{
		return
			GetCurrencyPilesForShop()
				.Where(x => x.Currency == Currency)
				.Sum(x => x.TotalValue) +
			CashBalance +
			(BankAccount is not null && BankAccount.Currency == Currency ?
				BankAccount.MaximumWithdrawal()
			: 0.0M)
		;
	}

	public void TakeCashFromAllSources(decimal amount, string paymentReference)
	{
		Changed = true;
		if (CashBalance >= amount)
		{
			ExpectedCashBalance -= amount;
			CashBalance -= amount;
			return;
		}

		ExpectedCashBalance -= CashBalance;
		amount -= CashBalance;
		CashBalance = 0.0M;

		var piles = GetCurrencyPilesForShop()
					.Where(x => x.Currency == Currency)
					.ToList();
		var coinBalance = piles
						  .Sum(x => x.TotalValue);
		if (coinBalance >= amount)
		{
			var targetCoins = Currency.FindCurrency(piles, amount);
			var value = targetCoins.Sum(x => x.Value.Sum(y => y.Value * y.Key.Value));
			var change = value - amount;
			foreach (var item in targetCoins.Where(item =>
						 !item.Key.RemoveCoins(item.Value.Select(x => Tuple.Create(x.Key, x.Value)))))
			{
				item.Key.Parent.Delete();
			}

			if (change > 0.0M)
			{
				AddCurrencyToShop(CurrencyGameItemComponentProto.CreateNewCurrencyPile(Currency, Currency.FindCoinsForAmount(change, out var _)));
			}

			return;
		}

		amount -= coinBalance;
		ExpectedCashBalance -= coinBalance;
		foreach (var pile in piles)
		{
			pile.Parent.Delete();
		}

		if (BankAccount is null || BankAccount.Currency != Currency)
		{
			return;
		}

		var bankBalance = BankAccount.MaximumWithdrawal();
		if (bankBalance >= amount)
		{
			BankAccount.WithdrawFromTransaction(amount, paymentReference);
			return;
		}

		BankAccount.WithdrawFromTransaction(bankBalance, paymentReference);
		CashBalance -= amount - bankBalance;
		ExpectedCashBalance -= amount - bankBalance;
	}

	public virtual IEnumerable<IGameItem> DoAutostockAllMerchandise()
	{
		var items = new List<IGameItem>();
#if DEBUG
		var sw = new Stopwatch();
#endif
		foreach (var merchandise in Merchandises)
		{
#if DEBUG
			sw.Restart();
#endif
			items.AddRange(DoAutostockForMerchandise(merchandise));
#if DEBUG
			sw.Stop();
			$"Autostock for #3{merchandise.Name}#0 (#{merchandise.Id:N0}) - {sw.ElapsedMilliseconds}ms".WriteLineConsole();
#endif
		}

		return items;
	}

	protected void RecalculateStockedItems(IMerchandise merchandise, int expectedChange)
	{
		var stocked = StockedItems(merchandise).ToList();
		var difference = _stockedMerchandiseCounts.Count(merchandise) - stocked.Sum(x => x.Quantity);
		if (difference > expectedChange)
		// Some items were missing
		{
			_transactionRecords.Add(new TransactionRecord(ShopTransactionType.StockLoss, Currency, this,
				EconomicZone.ZoneForTimePurposes.DateTime(), null, merchandise.EffectiveAutoReorderPrice * (difference - expectedChange),
				0.0M, merchandise));
		}
		else if (difference < expectedChange)
		// Extras were located
		{
			_transactionRecords.Add(new TransactionRecord(ShopTransactionType.Stock, Currency, this,
				EconomicZone.ZoneForTimePurposes.DateTime(), null,
				merchandise.EffectiveAutoReorderPrice * (expectedChange - difference) * -1, 0.0M, merchandise));
		}

		_stockedMerchandiseCounts[merchandise] = stocked.Sum(x => x.Quantity);
		_stockedMerchandise.Remove(merchandise);
		_stockedMerchandise.AddRange(merchandise, stocked.Select(x => x.Id));
		Changed = true;
	}

	public (decimal Price, IEnumerable<IGameItem> Items) PreviewBuy(ICharacter actor, IMerchandise merchandise,
		int quantity, IPaymentMethod method, string extraArguments = null)
	{
		var stockedItems = StockedItems(merchandise).ToList();
		if (!string.IsNullOrEmpty(extraArguments))
		{
			stockedItems = stockedItems.Where(x => x.HasKeywords(extraArguments.Split('.'), actor, true)).ToList();
		}

		var boughtItems = new List<IGameItem>();
		var quantitySought = quantity;
		foreach (var item in stockedItems)
		{
			if (quantitySought >= item.Quantity)
			{
				boughtItems.Add(item);
				quantitySought -= item.Quantity;
			}
			else
			{
				var newItem = item.PeekSplit(quantitySought);
				boughtItems.Add(newItem);
				quantitySought = 0;
			}

			if (quantitySought <= 0)
			{
				break;
			}
		}

		var price = 0.0M;
		switch (method)
		{
			case ShopCashPayment cash:
				price = PriceForMerchandise(actor, merchandise, quantity);
				break;
			case LineOfCreditPayment loc:
				price = PriceForMerchandise(
					loc.Account.IsAccountOwner(actor)
						? actor
						: Gameworld.TryGetCharacter(loc.Account.AccountOwnerId, true), merchandise, quantity);
				break;
		}

		return (price, boughtItems);
	}

	public decimal PriceForMerchandise(ICharacter actor, IMerchandise merchandise, int quantity)
	{
		// TODO - volume deals
		var tax = EconomicZone.SalesTaxes.Where(x => x.Applies(merchandise, actor))
							  .Sum(x => x.TaxValue(merchandise, actor));
		return Math.Truncate((merchandise.EffectivePrice + tax) * quantity);
	}

	public (decimal Price, decimal Tax) PriceAndTaxForMerchandise(ICharacter actor, IMerchandise merchandise,
		int quantity)
	{
		// TODO - volume deals
		var tax = Math.Round(
			EconomicZone.SalesTaxes.Where(x => x.Applies(merchandise, actor)).Sum(x => x.TaxValue(merchandise, actor)),
			0, MidpointRounding.AwayFromZero);
		return (Math.Truncate((merchandise.EffectivePrice + tax) * quantity), Math.Truncate(tax * quantity));
	}

	public (decimal TotalPrice, decimal IncludedTax, bool VolumeDealsExist) GetDetailedPriceInfo(ICharacter actor,
		IMerchandise merchandise)
	{
		var tax = Math.Round(
			EconomicZone.SalesTaxes.Where(x => x.Applies(merchandise, actor)).Sum(x => x.TaxValue(merchandise, actor)),
			0, MidpointRounding.AwayFromZero);
		return (merchandise.EffectivePrice + tax, tax, false);
		// TODO volume deals
	}

	public void PriceAdjustmentForMerchandise(IMerchandise merchandise, decimal oldValue, ICharacter actor)
	{
		var stock = _stockedMerchandise[merchandise]
					.SelectNotNull(x => Gameworld.TryGetItem(x))
					.ToList();

		var quantity = stock.Sum(x => x.Quantity);
		AddTransaction(new TransactionRecord(ShopTransactionType.PriceAdjustment, Currency, this,
			actor.Location.DateTime(), actor, quantity * (merchandise.EffectivePrice - oldValue), 0.0M, merchandise));
	}

	public IEnumerable<IMerchandise> StockedMerchandise =>
		_stockedMerchandise.Where(x => x.Value.Any()).Select(x => x.Key).OrderBy(x => x.Name);

	public abstract IEnumerable<IGameItem> AllStockedItems { get; }

	public abstract IEnumerable<IGameItem> StockedItems(IMerchandise merchandise);

	public void ShowDeals(ICharacter actor, ICharacter purchaser, IMerchandise merchandise = null)
	{
		actor.OutputHandler.Send("Coming soon.");
	}

	public void ShowList(ICharacter actor, ICharacter purchaser, string keyword)
	{
		var sb = new StringBuilder();
		sb.AppendLine(Name.TitleCase().Colour(Telnet.Cyan));
		sb.AppendLine($"Merchandise with keyword {keyword.ColourCommand()}:");
		var stockTake = StocktakeAllMerchandise();
		var index = 1;
		var merchIndexes = new Dictionary<IMerchandise, int>();
		foreach (var item in StockedMerchandise.OrderBy(x => x.Name))
		{
			merchIndexes[item] = index++;
		}

		if (Gameworld.GetStaticBool("DisplayTaxInShopList"))
		{
			sb.AppendLine(StringUtilities.GetTextTable(
			from merch in stockTake
			let priceInfo = GetDetailedPriceInfo(purchaser, merch.Key)
			where merch.Value.InStockroomCount + merch.Value.OnFloorCount > 0 &&
				  (merch.Key.ListDescription.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) ||
				   merch.Key.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase)) &&
				  merchIndexes.ContainsKey(merch.Key)
			orderby merch.Key.Name
			select new[]
			{
					merchIndexes[merch.Key].ToString("N0", actor),
					merch.Key.Name,
					merch.Key.ListDescription.ColourObject(),
					Currency.Describe(priceInfo.TotalPrice, CurrencyDescriptionPatternType.Short),
					Currency.Describe(priceInfo.IncludedTax, CurrencyDescriptionPatternType.Short),
					priceInfo.VolumeDealsExist.ToString(actor),
					merch.Key.Item.IsItemType<VariableGameItemComponentProto>().ToString(actor),
					(merch.Value.InStockroomCount + merch.Value.OnFloorCount).ToString("N0", actor)
			},
			new[]
			{
					"#",
					"Name",
					"Description",
					"Price",
					"Included Tax",
					"Deals?",
					"Variants?",
					"Stock"
			},
			actor.LineFormatLength,
			truncatableColumnIndex: 1,
			colour: Telnet.Yellow,
			unicodeTable: actor.Account.UseUnicode
		));
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(
			from merch in stockTake
			let priceInfo = GetDetailedPriceInfo(purchaser, merch.Key)
			where merch.Value.InStockroomCount + merch.Value.OnFloorCount > 0 &&
			      merchIndexes.ContainsKey(merch.Key)
			orderby merch.Key.Name
			select new[]
			{
				merchIndexes[merch.Key].ToString("N0", actor),
					merch.Key.Name,
					merch.Key.ListDescription.ColourObject(),
					Currency.Describe(priceInfo.TotalPrice, CurrencyDescriptionPatternType.Short),
					priceInfo.VolumeDealsExist.ToString(actor),
					merch.Key.Item.IsItemType<VariableGameItemComponentProto>().ToString(actor),
					(merch.Value.InStockroomCount + merch.Value.OnFloorCount).ToString("N0", actor)
			},
			new[]
			{
					"#",
					"Name",
					"Description",
					"Price",
					"Deals?",
					"Variants?",
					"Stock"
			},
			actor.LineFormatLength,
			truncatableColumnIndex: 1,
			colour: Telnet.Yellow,
			unicodeTable: actor.Account.UseUnicode
		));
		}


		if (actor != purchaser)
		{
			sb.AppendLine(
				$"Note: Showing prices for an individual other than yourself. Only valid when using their account."
					.Colour(Telnet.Red));
		}

		sb.AppendLine($"Note: Use the {"deals".ColourCommand()} command to follow up on any deals in place.");
		sb.AppendLine(
			$"Note: Use the {"list variants <item>".ColourCommand()} command to view any variants (e.g. colours) available if desired.");
		actor.OutputHandler.Send(sb.ToString(), false);
		return;
	}

	public void ShowList(ICharacter actor, ICharacter purchaser, IMerchandise merchandise = null)
	{
		var stockTake = StocktakeAllMerchandise();
		var sb = new StringBuilder();
		sb.AppendLine(Name.TitleCase().Colour(Telnet.Cyan));
		var merchIndexes = new Dictionary<IMerchandise, int>();
		var index = 1;
		foreach (var item in StockedMerchandise.OrderBy(x => x.Name))
		{
			merchIndexes[item] = index++;
		}

		if (merchandise == null)
		{
			if (Gameworld.GetStaticBool("DisplayTaxInShopList"))
			{
				sb.AppendLine(StringUtilities.GetTextTable(
				from merch in stockTake
				let priceInfo = GetDetailedPriceInfo(purchaser, merch.Key)
				where merch.Value.InStockroomCount + merch.Value.OnFloorCount > 0 
				      && merchIndexes.ContainsKey(merch.Key)
				orderby merch.Key.Name
				select new[]
				{
					merchIndexes[merch.Key].ToString("N0", actor),
					merch.Key.Name,
					merch.Key.ListDescription.ColourObject(),
					Currency.Describe(priceInfo.TotalPrice, CurrencyDescriptionPatternType.Short),
					Currency.Describe(priceInfo.IncludedTax, CurrencyDescriptionPatternType.Short),
					priceInfo.VolumeDealsExist.ToString(actor),
					merch.Key.Item.IsItemType<VariableGameItemComponentProto>().ToString(actor),
					(merch.Value.InStockroomCount + merch.Value.OnFloorCount).ToString("N0", actor)
				},
				new[]
				{
					"#",
					"Name",
					"Description",
					"Price",
					"Included Tax",
					"Deals?",
					"Variants?",
					"Stock"
				},
				actor.LineFormatLength,
				truncatableColumnIndex: 1,
				colour: Telnet.Yellow,
				unicodeTable: actor.Account.UseUnicode
			));
			}
			else
			{
				sb.AppendLine(StringUtilities.GetTextTable(
				from merch in stockTake
				let priceInfo = GetDetailedPriceInfo(purchaser, merch.Key)
				where merch.Value.InStockroomCount + merch.Value.OnFloorCount > 0 &&
				      merchIndexes.ContainsKey(merch.Key)
				orderby merch.Key.Name
				select new[]
				{
					merchIndexes[merch.Key].ToString("N0", actor),
					merch.Key.Name,
					merch.Key.ListDescription.ColourObject(),
					Currency.Describe(priceInfo.TotalPrice, CurrencyDescriptionPatternType.Short),
					priceInfo.VolumeDealsExist.ToString(actor),
					merch.Key.Item.IsItemType<VariableGameItemComponentProto>().ToString(actor),
					(merch.Value.InStockroomCount + merch.Value.OnFloorCount).ToString("N0", actor)
				},
				new[]
				{
					"#",
					"Name",
					"Description",
					"Price",
					"Deals?",
					"Variants?",
					"Stock"
				},
				actor.LineFormatLength,
				truncatableColumnIndex: 1,
				colour: Telnet.Yellow,
				unicodeTable: actor.Account.UseUnicode
			));
			}


			if (actor != purchaser)
			{
				sb.AppendLine(
					$"Note: Showing prices for an individual other than yourself. Only valid when using their account."
						.Colour(Telnet.Red));
			}

			sb.AppendLine($"Note: Use the {"deals".ColourCommand()} command to follow up on any deals in place.");
			sb.AppendLine(
				$"Note: Use the {"list variants <item>".ColourCommand()} command to view any variants (e.g. colours) available if desired.");
			actor.OutputHandler.Send(sb.ToString(), false);
			return;
		}

		sb.AppendLine($"Variants for merchandise {merchandise.ListDescription.Colour(Telnet.BoldMagenta)}:");
		var stock = StockedItems(merchandise);
		foreach (var item in stock)
		{
			sb.AppendLine($"\t{item.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)}");
		}

		actor.OutputHandler.Send(sb.ToString(), false);
	}

	protected abstract void ShowInfo(ICharacter actor, StringBuilder sb);

	public void ShowInfo(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"This is a shop, called {Name.TitleCase().Colour(Telnet.Cyan)}.");
		sb.AppendLine(
			$"The shop is currently {(IsTrading ? "open".Colour(Telnet.Green) : "closed".Colour(Telnet.Red))}.");

		if (IsProprietor(actor))
		{
			sb.AppendLine("You are a proprietor of this store.");
		}
		else if (IsManager(actor))
		{
			sb.AppendLine("You are a manager of this store.");
		}
		else if (IsEmployee(actor))
		{
			sb.AppendLine("You are an employee of this store.");
		}

		if (IsEmployee(actor) || actor.IsAdministrator())
		{
			if (BankAccount is null)
			{
				sb.AppendLine("This shop does not have a bank account.");
			}
			else
			{
				sb.AppendLine(
					$"This shop uses the bank account {BankAccount.AccountReference.ColourValue()} for transactions.");
			}

			sb.AppendLine($"Minimum Float to Buy Items: {Currency.Describe(MinimumFloatToBuyItems, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
			sb.AppendLine($"Autopay Taxes: {AutoPayTaxes.ToColouredString()}");
			sb.AppendLine($"Market for Pricing: {MarketForPricingPurposes?.Name.ColourValue() ?? "None".Colour(Telnet.Red)}");
		}

		if (IsProprietor(actor) || actor.IsAdministrator())
		{
			sb.AppendLine(
				$"The CanShopProg is set to {(CanShopProg != null ? $"#{CanShopProg.Id.ToString("N0", actor)} {CanShopProg.FunctionName}".FluentTagMXP("send", $"href='show prog {CanShopProg.Id}'") : "None".Colour(Telnet.Red))}.");
			sb.AppendLine(
				$"The WhyCannotShopProg is set to {(WhyCannotShopProg != null ? $"#{WhyCannotShopProg.Id.ToString("N0", actor)} {WhyCannotShopProg.FunctionName}".FluentTagMXP("send", $"href='show prog {CanShopProg.Id}'") : "None".Colour(Telnet.Red))}.");
		}

		if (IsManager(actor) || actor.IsAdministrator())
		{
			sb.AppendLine($"Total Taxes Owing: {EconomicZone.Currency.Describe(EconomicZone.OutstandingTaxesForShop(this), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
			CheckFloat();
			sb.AppendLine($"Float: {EconomicZone.Currency.Describe(ExpectedCashBalance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
			sb.AppendLine($"Virtual Balance: {EconomicZone.Currency.Describe(CashBalance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
			sb.AppendLine($"In Registers: {EconomicZone.Currency.Describe(GetCurrencyPilesForShop().Sum(x => x.TotalValue), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		}

		if (actor.IsAdministrator())
		{
			sb.AppendLine();
			sb.AppendLine("Employees:");
			sb.AppendLine(StringUtilities.GetTextTable(
				from ch in EmployeeRecords.OrderBy(x => x.IsProprietor ? 0 : x.IsManager ? 1 : 2)
				select new List<string>
				{
					ch.EmployeeCharacterId.ToString("N0", actor),
					ch.Name.GetName(NameStyle.FullName),
					ch.IsManager.ToColouredString(),
					ch.IsProprietor.ToColouredString(),
					ch.EmployeeSince.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short),
				},
				new List<string>
				{
					"Id",
					"Name",
					"Manager?",
					"Owner?",
					"Start Date"
				},
				actor,
				Telnet.Yellow
			));
		}

		foreach (var ch in EmployeesOnDuty)
		{
			if (IsEmployee(actor) || actor.IsAdministrator())
			{
				var tag = "";
				if (IsProprietor(ch))
				{
					tag = " [proprietor]".Colour(Telnet.BoldWhite);
				}
				else if (IsManager(ch))
				{
					tag = " [manager]".Colour(Telnet.BoldCyan);
				}

				sb.AppendLine(
					$"{ch.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)}{tag} is currently on duty.");
			}
			else
			{
				sb.AppendLine(
					$"{ch.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} is currently on duty.");
			}
		}

		ShowInfo(actor, sb);
		actor.OutputHandler.Send(sb.ToString());
	}

	public abstract (int OnFloorCount, int InStockroomCount) StocktakeMerchandise(IMerchandise whichMerchandise);

	public Dictionary<IMerchandise, (int OnFloorCount, int InStockroomCount)> StocktakeAllMerchandise()
	{
		CheckFloat();
		var dictionary = new Dictionary<IMerchandise, (int OnFloorCount, int InStockroomCount)>();
		foreach (var merch in Merchandises)
		{
			dictionary[merch] = StocktakeMerchandise(merch);
		}

		return dictionary;
	}

	public abstract IReadOnlyDictionary<ICurrencyPile, Dictionary<ICoin, int>> GetCurrencyForShop(decimal amount);
	public abstract void AddCurrencyToShop(IGameItem currencyPile);
	public abstract IEnumerable<ICurrencyPile> GetCurrencyPilesForShop();
	public abstract void CheckFloat();

	public bool IsWelcomeCustomer(ICharacter customer)
	{
		return CanShopProg?.Execute<bool?>(customer, null, new List<string>()) != false;
	}

	#region  Building Commands
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "can":
			case "canprog":
			case "can_prog":
			case "can prog":
			case "prog":
				return BuildingCommandCanProg(actor, command);
			case "trading":
			case "open":
			case "trades":
				return BuildingCommandTrading(actor);
			case "name":
			case "rename":
				return BuildingCommandName(actor, command);
			case "buyfloat":
			case "minbuyfloat":
			case "minfloat":
				return BuildingCommandBuyFloat(actor, command);
			case "market":
				return BuildingCommandMarket(actor, command);
			case "autopay":
			case "autopaytaxes":
				return BuildingCommandAutopayTaxes(actor);
			default:
				actor.OutputHandler.Send(@"Valid options for this command are as follows:

#3name <name>#0 - renames this shop
#3can <prog> <whyprog>#0 - sets a prog that determines who can shop here and a prog to give an error reason
#3trading#0 - toggles whether this shop is trading
#3market <which>#0 - sets a market to draw pricing multipliers from
#3market none#0 - clears the market pricing
#3minfloat <amount>#0 - sets the minimum float for the shop to buy anything".SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandAutopayTaxes(ICharacter actor)
	{
		AutoPayTaxes = !AutoPayTaxes;
		Changed = true;
		actor.OutputHandler.Send($"This shop will {AutoPayTaxes.NowNoLonger()} automatically pay its taxes.");
		return true;
	}

	private bool BuildingCommandMarket(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a market, or #3none#0 to remove any market.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "delete", "remove"))
		{
			MarketForPricingPurposes = null;
			Changed = true;
			actor.OutputHandler.Send("This shop will no longer use a market for pricing purposes.");
			return false;
		}

		var market = Gameworld.Markets.GetByIdOrName(command.SafeRemainingArgument);
		if (market is null)
		{
			actor.OutputHandler.Send("There is no such market.");
			return false;
		}

		if (market.EconomicZone != EconomicZone)
		{
			actor.OutputHandler.Send("You cannot link to a market with a different economic zone.");
			return false;
		}

		MarketForPricingPurposes = market;
		Changed = true;
		actor.OutputHandler.Send($"This shop will now use the {market.Name.ColourValue()} market for price multipliers on all non-exempted stock.");
		return true;
	}

	private bool BuildingCommandBuyFloat(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var amount))
		{
			actor.OutputHandler.Send("You must enter a valid amount of currency.");
			return false;
		}

		MinimumFloatToBuyItems = amount;
		Changed = true;
		actor.OutputHandler.Send($"This shop will no longer buy any items when its cash float is below {Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this shop?");
			return false;
		}

		_name = command.SafeRemainingArgument.TitleCase();
		Changed = true;
		actor.OutputHandler.Send($"This shop is now called {_name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandTrading(ICharacter actor)
	{
		IsTrading = !IsTrading;
		Changed = true;
		actor.OutputHandler.Send($"This store is {(IsTrading ? "now" : "no longer")} trading.");
		return true;
	}

	private bool BuildingCommandCanProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must specify a prog that returns a boolean and accepts a character, a number and a text collection.");
			return false;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send("You must specify a prog that returns a boolean value.");
			return false;
		}

		if (!prog.MatchesParameters(new[]
			{
				ProgVariableTypes.Character, ProgVariableTypes.Number,
				ProgVariableTypes.Text | ProgVariableTypes.Collection
			}))
		{
			actor.OutputHandler.Send(
				"You must specify a prog that accepts a character, a number and a text collection.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must also supply a prog that returns text (an error message) and accepts a character, a number and a text collection.");
			return false;
		}

		var why = long.TryParse(command.PopSpeech(), out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (why == null)
		{
			actor.OutputHandler.Send("There is no such prog (for the 'why' argument).");
			return false;
		}

		if (!why.ReturnType.CompatibleWith(ProgVariableTypes.Text))
		{
			actor.OutputHandler.Send("You must specify a 'why' prog that returns a text value.");
			return false;
		}

		if (!why.MatchesParameters(new[]
			{
				ProgVariableTypes.Character, ProgVariableTypes.Number,
				ProgVariableTypes.Text | ProgVariableTypes.Collection
			}))
		{
			actor.OutputHandler.Send(
				"You must specify a 'why' prog that accepts a character, a number and a text collection.");
			return false;
		}

		_canShopProg = prog;
		_whyCannotShopProg = why;
		Changed = true;
		actor.OutputHandler.Send(
			$"This shop will now use the {_canShopProg.MXPClickableFunctionNameWithId()} prog to determine who can shop here and the {_whyCannotShopProg.MXPClickableFunctionNameWithId()} prog to deliver an error message to those who can't.");
		return true;
	}
	#endregion
	public void Delete()
	{
		using (new FMDB())
		{
			if (_id != 0)
			{
				Changed = false;
				Gameworld.SaveManager.Abort(this);
				Gameworld.Destroy(this);
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.Shops.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.Shops.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	#region IFutureProgVariable Implementation

	public ProgVariableTypes Type => ProgVariableTypes.Shop;
	public object GetObject => this;

	public virtual IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "merchandise":
				return new CollectionVariable(Merchandises.ToList(), ProgVariableTypes.Merchandise);
			case "shopfront":
				return new CollectionVariable(new List<ICell>(), ProgVariableTypes.Location);
			case "storeroom":
				return new NullVariable(ProgVariableTypes.Location);
			case "workshop":
				return new NullVariable(ProgVariableTypes.Location);
			case "tills":
				return new CollectionVariable(new List<IGameItem>(), ProgVariableTypes.Item);
			case "employees":
				return new CollectionVariable(_employeeRecords.Select(x => x.EmployeeCharacterId).ToList(),
					ProgVariableTypes.Number);
			case "employeesonduty":
				return new CollectionVariable(EmployeesOnDuty.ToList(), ProgVariableTypes.Character);
			case "managers":
				return new CollectionVariable(
					_employeeRecords.Where(x => x.IsManager).Select(x => x.EmployeeCharacterId).ToList(),
					ProgVariableTypes.Number);
			case "proprietors":
				return new CollectionVariable(
					_employeeRecords.Where(x => x.IsProprietor).Select(x => x.EmployeeCharacterId).ToList(),
					ProgVariableTypes.Number);
			case "stock":
				return new CollectionVariable(Merchandises.SelectMany(x => StockedItems(x)).ToList(),
					ProgVariableTypes.Item);
		}

		throw new ApplicationException("Invalid property in Shop.GetProperty");
	}

	private static ProgVariableTypes DotReferenceHandler(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return ProgVariableTypes.Number;
			case "name":
				return ProgVariableTypes.Text;
			case "merchandise":
				return ProgVariableTypes.Collection | ProgVariableTypes.Merchandise;
			case "shopfront":
				return ProgVariableTypes.Collection | ProgVariableTypes.Location;
			case "storeroom":
				return ProgVariableTypes.Location;
			case "workshop":
				return ProgVariableTypes.Location;
			case "tills":
				return ProgVariableTypes.Collection | ProgVariableTypes.Item;
			case "employees":
				return ProgVariableTypes.Collection | ProgVariableTypes.Number;
			case "employeesonduty":
				return ProgVariableTypes.Collection | ProgVariableTypes.Character;
			case "managers":
				return ProgVariableTypes.Collection | ProgVariableTypes.Number;
			case "proprietors":
				return ProgVariableTypes.Collection | ProgVariableTypes.Number;
			case "stock":
				return ProgVariableTypes.Collection | ProgVariableTypes.Item;
		}

		return ProgVariableTypes.Error;
	}

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "merchandise", ProgVariableTypes.Collection | ProgVariableTypes.Merchandise },
			{ "shopfront", ProgVariableTypes.Collection | ProgVariableTypes.Location },
			{ "storeroom", ProgVariableTypes.Location },
			{ "workshop", ProgVariableTypes.Location },
			{ "tills", ProgVariableTypes.Collection | ProgVariableTypes.Item },
			{ "employees", ProgVariableTypes.Collection | ProgVariableTypes.Number },
			{ "employeesonduty", ProgVariableTypes.Collection | ProgVariableTypes.Character },
			{ "managers", ProgVariableTypes.Collection | ProgVariableTypes.Number },
			{ "proprietors", ProgVariableTypes.Collection | ProgVariableTypes.Number },
			{ "stock", ProgVariableTypes.Collection | ProgVariableTypes.Item }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" },
			{ "merchandise", "" },
			{ "shopfront", "" },
			{ "storeroom", "" },
			{ "workshop", "" },
			{ "tills", "" },
			{ "employees", "" },
			{ "employeesonduty", "" },
			{ "managers", "" },
			{ "proprietors", "" },
			{ "stock", "" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Shop, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}