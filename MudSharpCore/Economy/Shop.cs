using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Framework;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Effects.Concrete;
using MudSharp.Economy.Payment;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Events;
using MudSharp.Models;

namespace MudSharp.Economy;

public class Shop : SaveableItem, IShop
{
	public Shop(IEconomicZone zone, ICell originalShopFront, string name)
	{
		Gameworld = zone.Gameworld;
		_shopfrontCells.Add(originalShopFront);
		Currency = zone.Currency;
		_name = name;
		EconomicZone = zone;
		IsTrading = true;
		using (new FMDB())
		{
			var dbitem = new Models.Shop();
			FMDB.Context.Shops.Add(dbitem);
			dbitem.Name = name;
			dbitem.CurrencyId = Currency.Id;
			dbitem.EconomicZoneId = zone.Id;
			dbitem.IsTrading = true;
			dbitem.EmployeeRecords = "<Employees/>";
			dbitem.ShopsStoreroomCells.Add(new ShopsStoreroomCell { Shop = dbitem, CellId = originalShopFront.Id });
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		InitialiseShop();
	}

	public Shop(MudSharp.Models.Shop shop, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = shop.Id;
		_name = shop.Name;
		EconomicZone = gameworld.EconomicZones.Get(shop.EconomicZoneId);
		_cashBalance = shop.CashBalance;
		IsTrading = shop.IsTrading;
		Currency = gameworld.Currencies.Get(shop.CurrencyId);
		_shopfrontCells.AddRange(shop.ShopsStoreroomCells.Select(x => gameworld.Cells.Get(x.CellId)));
		_canShopProg = gameworld.FutureProgs.Get(shop.CanShopProgId ?? 0);
		_whyCannotShopProg = gameworld.FutureProgs.Get(shop.WhyCannotShopProgId ?? 0);
		_stockroomCell = gameworld.Cells.Get(shop.StockroomCellId ?? 0);
		_workshopCell = gameworld.Cells.Get(shop.WorkshopCellId ?? 0);
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

		foreach (var item in shop.ShopsTills)
		{
			_tillItemIds.Add(item.GameItemId);
		}

		foreach (var item in shop.LineOfCreditAccounts)
		{
			_lineOfCreditAccounts.Add(new LineOfCreditAccount(item, this, Gameworld));
		}

		InitialiseShop();
	}

	private void InitialiseShop()
	{
		foreach (var cell in AllShopCells)
		{
			cell.Shop = this;
		}
	}

	public void PostLoadInitialisation()
	{
		foreach (var merchandise in Merchandises)
		{
			var stocked = StockedItems(merchandise).ToList();
			_stockedMerchandiseCounts[merchandise] = stocked.Sum(x => x.Quantity);
			foreach (var item in stocked)
			{
				_stockedMerchandise.Add(merchandise, item.Id);
			}
		}
	}

	private IEconomicZone _economicZone;
	private ICurrency _currency;
	private ICell _workshopCell;
	private ICell _stockroomCell;
	private IFutureProg _canShopProg;
	private IFutureProg _whyCannotShopProg;
	private long? _bankAccountId;
	private IBankAccount _bankAccount;
	private decimal _cashBalance;

	public override void Save()
	{
		var dbitem = FMDB.Context.Shops.Find(Id);
		dbitem.Name = _name;
		dbitem.CashBalance = _cashBalance;
		dbitem.BankAccountId = _bankAccountId;
		dbitem.CurrencyId = Currency.Id;
		dbitem.EconomicZoneId = EconomicZone.Id;
		dbitem.IsTrading = IsTrading;
		dbitem.CanShopProgId = CanShopProg?.Id;
		dbitem.WhyCannotShopProgId = WhyCannotShopProg?.Id;
		dbitem.StockroomCellId = StockroomCell?.Id;
		dbitem.WorkshopCellId = WorkshopCell?.Id;
		dbitem.EmployeeRecords = new XElement("Employees",
			from employee in EmployeeRecords
			select employee.SaveToXml()
		).ToString();
		FMDB.Context.ShopsTills.RemoveRange(dbitem.ShopsTills);
		foreach (var item in _tillItemIds)
		{
			var dbtill = FMDB.Context.GameItems.Find(item);
			if (dbtill != null)
			{
				dbitem.ShopsTills.Add(new ShopsTill { Shop = dbitem, GameItemId = item });
			}
		}

		FMDB.Context.ShopsStoreroomCells.RemoveRange(dbitem.ShopsStoreroomCells);
		foreach (var cell in ShopfrontCells)
		{
			dbitem.ShopsStoreroomCells.Add(new ShopsStoreroomCell { Shop = dbitem, CellId = cell.Id });
		}

		Changed = false;
	}

	public override string FrameworkItemType => "Shop";

	public IEconomicZone EconomicZone
	{
		get => _economicZone;
		set
		{
			_economicZone = value;
			Changed = true;
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
	private readonly List<ICell> _shopfrontCells = new();
	public IEnumerable<ICell> ShopfrontCells => _shopfrontCells;

	public ICell WorkshopCell
	{
		get => _workshopCell;
		set
		{
			if (_workshopCell != null && value != _workshopCell)
			{
				RemoveCellFromStore(_workshopCell);
			}

			_workshopCell = value;
			Changed = true;
		}
	}

	public ICell StockroomCell
	{
		get => _stockroomCell;
		set
		{
			if (_stockroomCell != null && value != _stockroomCell)
			{
				RemoveCellFromStore(_stockroomCell);
			}

			_stockroomCell = value;
			Changed = true;
		}
	}

	public decimal CashBalance
	{
		get => _cashBalance;
		set
		{
			_cashBalance = value;
			Changed = true;
		}
	}

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

	private void RemoveCellFromStore(ICell cell)
	{
		cell.Shop = null;
		var time = cell.DateTime();
		foreach (var item in cell.GameItems.SelectMany(x => x.DeepItems))
		{
			if (item.AffectedBy<ItemOnDisplayInShop>())
			{
				DisposeFromStock(null, item);
			}
		}
	}

	public void AddShopfrontCell(ICell cell)
	{
		if (!_shopfrontCells.Contains(cell))
		{
			_shopfrontCells.Add(cell);
			Changed = true;
		}
	}

	public void RemoveShopfrontCell(ICell cell)
	{
		_shopfrontCells.Remove(cell);
		RemoveCellFromStore(cell);
		Changed = true;
	}

	public IEnumerable<ICell> AllShopCells => ShopfrontCells.Concat(new[]
	{
		WorkshopCell,
		StockroomCell
	}.WhereNotNull(x => x));

	private readonly CollectionDictionary<IMerchandise, long> _stockedMerchandise = new();
	private readonly Counter<IMerchandise> _stockedMerchandiseCounts = new();

	public bool IsTrading { get; protected set; }

	private readonly List<IEmployeeRecord> _employeeRecords = new();
	public IEnumerable<IEmployeeRecord> EmployeeRecords => _employeeRecords;

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
		AllShopCells.SelectMany(x => x.Characters).Where(x => IsClockedIn(x));

	private readonly List<IMerchandise> _merchandises = new();
	public IEnumerable<IMerchandise> Merchandises => _merchandises;

	private readonly HashSet<long> _tillItemIds = new();

	public IEnumerable<IGameItem> TillItems =>
		AllShopCells.SelectMany(x => x.GameItems).Where(x => _tillItemIds.Contains(x.Id));

	private readonly HashSet<long> _displayContainerIds = new();

	public IEnumerable<IGameItem> DisplayContainers => AllShopCells.SelectMany(x => x.GameItems)
	                                                               .Where(x => _displayContainerIds.Contains(x.Id));

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

	public void AddTillItem(IGameItem till)
	{
		_tillItemIds.Add(till.Id);
		Changed = true;
	}

	public void RemoveTillItem(IGameItem till)
	{
		_tillItemIds.Remove(till.Id);
		Changed = true;
	}

	public void AddDisplayContainer(IGameItem item)
	{
		_displayContainerIds.Add(item.Id);
		Changed = true;
	}

	public void RemoveDisplayContainer(IGameItem item)
	{
		_displayContainerIds.Remove(item.Id);
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
			actor?.Location.DateTime() ?? ShopfrontCells.First().DateTime(), actor,
			merch.BasePrice * item.Quantity * -1, 0.0M));
		_stockedMerchandise.Add(merch, item.Id);
		_stockedMerchandiseCounts.Add(merch, item.Quantity);
	}

	public void DisposeFromStock(ICharacter actor, IGameItem item)
	{
		item.RemoveAllEffects<ItemOnDisplayInShop>();
		actor?.OutputHandler.Send(
			$"You dispose of {item.HowSeen(actor)} from the for-sale inventory of {Name.TitleCase().Colour(Telnet.Cyan)}.");
		var merch = _merchandises.FirstOrDefault(x => x.IsMerchandiseFor(item));
		AddTransaction(new TransactionRecord(ShopTransactionType.StockLoss, Currency, this,
			actor?.Location.DateTime() ?? ShopfrontCells.First().DateTime(), actor,
			merch?.BasePrice ?? 0.0M * item.Quantity, 0.0M));
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

	public (bool Truth, string Reason) CanBuy(ICharacter actor, IMerchandise merchandise, int quantity,
		IPaymentMethod method, string extraArguments = null)
	{
		if (!IsTrading && !actor.IsAdministrator())
		{
			return (false, "the store is currently closed");
		}

		if ((bool?)CanShopProg?.Execute(actor, merchandise?.Item.Id ?? 0L,
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
				if (!TillItems.Any())
				{
					return (false, "the store is currently missing its till, and so cannot do cash transactions.");
				}

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

		return (true, string.Empty);
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
			item.RemoveAllEffects<ItemOnDisplayInShop>();
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
				CashBalance += price;
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
			ShopfrontCells.First().DateTime(), actor, price - tax, tax));
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
			DoAutostockForMerchandise(merchandise, restockInfo);
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

	public IEnumerable<IGameItem> DoAutostockForMerchandise(IMerchandise merchandise,
		List<(IGameItem Item, IGameItem Container)> purchasedItems = null)
	{
		var quantityToRestock = merchandise.MinimumStockLevels - _stockedMerchandiseCounts[merchandise];
		var originalQuantity = quantityToRestock;
		var newItems = new List<IGameItem>();
		if (merchandise.PreserveVariablesOnReorder && purchasedItems != null)
		{
			foreach (var item in purchasedItems)
			{
				var newItem = item.Item.DeepCopy(true);
				newItem.Skin = merchandise.Skin;
				newItems.Add(newItem);
				quantityToRestock -= newItem.Quantity;
			}
		}

		if (quantityToRestock > 0)
		{
			if (merchandise.Item.Components.Any(x => x is StackableGameItemComponentProto))
			{
				var newItem = merchandise.Item.CreateNew(null);
				newItem.Skin = merchandise.Skin;
				newItem.GetItemType<StackableGameItemComponent>().Quantity = quantityToRestock;
				newItems.Add(newItem);
				Gameworld.Add(newItem);
			}
			else
			{
				for (var i = 0; i < quantityToRestock; i++)
				{
					var newItem = merchandise.Item.CreateNew(null);
					newItem.Skin = merchandise.Skin;
					newItems.Add(newItem);
					Gameworld.Add(newItem);
				}
			}
		}

		if (!newItems.Any())
		{
			return newItems;
		}

		var targetCell = StockroomCell;
		if (targetCell == null)
		{
			targetCell = ShopfrontCells.First();
		}

		foreach (var item in newItems)
		{
			item.AddEffect(new ItemOnDisplayInShop(item, this, merchandise));
			targetCell.Insert(item);
			item.HandleEvent(EventType.ItemFinishedLoading, item);
			_stockedMerchandise.Add(merchandise, item.Id);
			_stockedMerchandiseCounts[merchandise] += item.Quantity;
		}

		AddTransaction(new TransactionRecord(ShopTransactionType.Restock, Currency, this,
			ShopfrontCells.First().DateTime(), null, merchandise.AutoReorderPrice * originalQuantity, 0.0M));
		return newItems;
	}

	private void RecalculateStockedItems(IMerchandise merchandise, int expectedChange)
	{
		var stocked = StockedItems(merchandise).ToList();
		var difference = _stockedMerchandiseCounts.Count(merchandise) - stocked.Sum(x => x.Quantity);
		if (difference > expectedChange)
			// Some items were missing
		{
			_transactionRecords.Add(new TransactionRecord(ShopTransactionType.StockLoss, Currency, this,
				ShopfrontCells.First().DateTime(), null, merchandise.AutoReorderPrice * (difference - expectedChange),
				0.0M));
		}
		else if (difference < expectedChange)
			// Extras were located
		{
			_transactionRecords.Add(new TransactionRecord(ShopTransactionType.Stock, Currency, this,
				ShopfrontCells.First().DateTime(), null,
				merchandise.AutoReorderPrice * (expectedChange - difference) * -1, 0.0M));
		}

		_stockedMerchandiseCounts[merchandise] = stocked.Sum(x => x.Quantity);
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
		return (merchandise.BasePrice + tax) * quantity;
	}

	public (decimal Price, decimal Tax) PriceAndTaxForMerchandise(ICharacter actor, IMerchandise merchandise,
		int quantity)
	{
		// TODO - volume deals
		var tax = Math.Round(
			EconomicZone.SalesTaxes.Where(x => x.Applies(merchandise, actor)).Sum(x => x.TaxValue(merchandise, actor)),
			0, MidpointRounding.AwayFromZero);
		return ((merchandise.BasePrice + tax) * quantity, tax * quantity);
	}

	public (decimal TotalPrice, decimal IncludedTax, bool VolumeDealsExist) GetDetailedPriceInfo(ICharacter actor,
		IMerchandise merchandise)
	{
		var tax = Math.Round(
			EconomicZone.SalesTaxes.Where(x => x.Applies(merchandise, actor)).Sum(x => x.TaxValue(merchandise, actor)),
			0, MidpointRounding.AwayFromZero);
		return (merchandise.BasePrice + tax, tax, false);
		// TODO volume deals
	}

	public void PriceAdjustmentForMerchandise(IMerchandise merchandise, decimal oldValue, ICharacter actor)
	{
		var stock = _stockedMerchandise[merchandise]
		            .Select(x => Gameworld.TryGetItem(x))
		            .ToList();

		var quantity = stock.Sum(x => x.Quantity);
		AddTransaction(new TransactionRecord(ShopTransactionType.PriceAdjustment, Currency, this,
			actor.Location.DateTime(), actor, quantity * (merchandise.BasePrice - oldValue), 0.0M));
	}

	public IEnumerable<IMerchandise> StockedMerchandise =>
		_stockedMerchandise.Where(x => x.Value.Any()).Select(x => x.Key).OrderBy(x => x.Name);

	public IEnumerable<IGameItem> StockedItems(IMerchandise merchandise)
	{
		return
			ShopfrontCells.SelectMany(x => x.Characters).SelectMany(x => x.Body.HeldItems)
			              .Concat(
				              ShopfrontCells
					              .ConcatIfNotNull(StockroomCell)
					              .SelectMany(x => x.GameItems)
					              .SelectMany(x => x.ShallowItems)
			              )
			              .Where(x => x.AffectedBy<ItemOnDisplayInShop>(merchandise));
	}

	public void ShowDeals(ICharacter actor, ICharacter purchaser, IMerchandise merchandise = null)
	{
		actor.OutputHandler.Send("Coming soon.");
	}

	public void ShowList(ICharacter actor, ICharacter purchaser, IMerchandise merchandise = null)
	{
		var sb = new StringBuilder();
		sb.AppendLine(Name.TitleCase().Colour(Telnet.Cyan));
		if (merchandise == null)
		{
			var stockTake = StocktakeAllMerchandise();
			var index = 1;
			sb.AppendLine(StringUtilities.GetTextTable(
				from merch in stockTake
				let priceInfo = GetDetailedPriceInfo(purchaser, merch.Key)
				where merch.Value.InStockroomCount + merch.Value.OnFloorCount > 0
				orderby merch.Key.Name
				select new[]
				{
					index++.ToString("N0", actor),
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
			if (actor != purchaser)
			{
				sb.AppendLine(
					$"Note: Showing prices for an individual other than yourself. Only valid when using their account."
						.Colour(Telnet.Red));
			}

			sb.AppendLine($"Note: Use the {"deals".ColourCommand()} command to follow up on any deals in place.");
			sb.AppendLine(
				$"Note: Use the {"list <item>".ColourCommand()} command to view any variants (e.g. colours) available if desired.");
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

			if (actor.Location == WorkshopCell)
			{
				sb.AppendLine("The location you are currently in is the workshop for this store.");
			}
			else if (actor.Location == StockroomCell)
			{
				sb.AppendLine("The location you are currently in is the stockroom for this store.");
			}
			else
			{
				sb.AppendLine("The location you are currently in is the shopfront for this store.");
			}

			foreach (var item in TillItems)
			{
				sb.AppendLine($"{item.HowSeen(actor, true)} is a till for this store.");
			}

			foreach (var item in DisplayContainers)
			{
				sb.AppendLine($"{item.HowSeen(actor, true)} is a display container for this store.");
			}
		}

		if (IsProprietor(actor) || actor.IsAdministrator())
		{
			sb.AppendLine(
				$"The CanShopProg is set to {(CanShopProg != null ? $"#{CanShopProg.Id.ToString("N0", actor)} {CanShopProg.FunctionName}".FluentTagMXP("send", $"href='show prog {CanShopProg.Id}'") : "None".Colour(Telnet.Red))}.");
			sb.AppendLine(
				$"The WhyCannotShopProg is set to {(WhyCannotShopProg != null ? $"#{WhyCannotShopProg.Id.ToString("N0", actor)} {WhyCannotShopProg.FunctionName}".FluentTagMXP("send", $"href='show prog {CanShopProg.Id}'") : "None".Colour(Telnet.Red))}.");
		}


		actor.OutputHandler.Send(sb.ToString());
	}

	public (int OnFloorCount, int InStockroomCount) StocktakeMerchandise(IMerchandise whichMerchandise)
	{
		if (!_merchandises.Contains(whichMerchandise))
		{
			return (0, 0);
		}

		RecalculateStockedItems(whichMerchandise, 0);
		var floorStock =
			ShopfrontCells.SelectMany(x => x.Characters).SelectMany(x => x.Body.HeldItems)
			              .Concat(
				              ShopfrontCells
					              .SelectMany(x => x.GameItems)
					              .SelectMany(x => x.ShallowItems)
			              )
			              .Where(x => x.AffectedBy<ItemOnDisplayInShop>(whichMerchandise))
			              .Sum(x => x.Quantity);
		return (floorStock, _stockedMerchandiseCounts[whichMerchandise] - floorStock);
	}

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

	public void CheckFloat()
	{
		var cashInRegister = TillItems
		                     .RecursiveGetItems<ICurrencyPile>(false)
		                     .Where(x => x.Currency == Currency)
		                     .Sum(x => x.Coins.Sum(y => y.Item2 * y.Item1.Value));
		if (cashInRegister > CashBalance)
		{
			AddTransaction(new TransactionRecord(ShopTransactionType.Float, Currency, this,
				AllShopCells.First().DateTime(), null, cashInRegister - CashBalance, 0.0M));
			CashBalance = cashInRegister;
			Changed = true;
			return;
		}

		if (cashInRegister < CashBalance)
		{
			AddTransaction(new TransactionRecord(ShopTransactionType.Withdrawal, Currency, this,
				AllShopCells.First().DateTime(), null, CashBalance - cashInRegister, 0.0M));
			CashBalance = cashInRegister;
			Changed = true;
			return;
		}
	}

	public bool IsWelcomeCustomer(ICharacter customer)
	{
		return CanShopProg?.Execute<bool?>(customer, null, new List<string>()) != false;
	}

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
			default:
				actor.OutputHandler.Send(@"Valid options for this command are as follows:

	#3name <name>#0 - renames this shop
	#3can <prog> <whyprog>#0 - sets a prog that determines who can shop here and a prog to give an error reason
	#3trading#0 - toggles whether this shop is trading".SubstituteANSIColour());
				return false;
		}
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

		if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send("You must specify a prog that returns a boolean value.");
			return false;
		}

		if (!prog.MatchesParameters(new[]
		    {
			    FutureProgVariableTypes.Character, FutureProgVariableTypes.Number,
			    FutureProgVariableTypes.Text | FutureProgVariableTypes.Collection
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

		if (!why.ReturnType.CompatibleWith(FutureProgVariableTypes.Text))
		{
			actor.OutputHandler.Send("You must specify a 'why' prog that returns a text value.");
			return false;
		}

		if (!why.MatchesParameters(new[]
		    {
			    FutureProgVariableTypes.Character, FutureProgVariableTypes.Number,
			    FutureProgVariableTypes.Text | FutureProgVariableTypes.Collection
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

	public FutureProgVariableTypes Type => FutureProgVariableTypes.Shop;
	public object GetObject => this;

	public IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "merchandise":
				return new CollectionVariable(Merchandises.ToList(), FutureProgVariableTypes.Merchandise);
			case "shopfront":
				return new CollectionVariable(ShopfrontCells.ToList(), FutureProgVariableTypes.Location);
			case "storeroom":
				return StockroomCell;
			case "workshop":
				return WorkshopCell;
			case "tills":
				return new CollectionVariable(TillItems.ToList(), FutureProgVariableTypes.Item);
			case "employees":
				return new CollectionVariable(_employeeRecords.Select(x => x.EmployeeCharacterId).ToList(),
					FutureProgVariableTypes.Number);
			case "employeesonduty":
				return new CollectionVariable(EmployeesOnDuty.ToList(), FutureProgVariableTypes.Character);
			case "managers":
				return new CollectionVariable(
					_employeeRecords.Where(x => x.IsManager).Select(x => x.EmployeeCharacterId).ToList(),
					FutureProgVariableTypes.Number);
			case "proprietors":
				return new CollectionVariable(
					_employeeRecords.Where(x => x.IsProprietor).Select(x => x.EmployeeCharacterId).ToList(),
					FutureProgVariableTypes.Number);
			case "stock":
				return new CollectionVariable(Merchandises.SelectMany(x => StockedItems(x)).ToList(),
					FutureProgVariableTypes.Item);
		}

		throw new ApplicationException("Invalid property in Shop.GetProperty");
	}

	private static FutureProgVariableTypes DotReferenceHandler(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return FutureProgVariableTypes.Number;
			case "name":
				return FutureProgVariableTypes.Text;
			case "merchandise":
				return FutureProgVariableTypes.Collection | FutureProgVariableTypes.Merchandise;
			case "shopfront":
				return FutureProgVariableTypes.Collection | FutureProgVariableTypes.Location;
			case "storeroom":
				return FutureProgVariableTypes.Location;
			case "workshop":
				return FutureProgVariableTypes.Location;
			case "tills":
				return FutureProgVariableTypes.Collection | FutureProgVariableTypes.Item;
			case "employees":
				return FutureProgVariableTypes.Collection | FutureProgVariableTypes.Number;
			case "employeesonduty":
				return FutureProgVariableTypes.Collection | FutureProgVariableTypes.Character;
			case "managers":
				return FutureProgVariableTypes.Collection | FutureProgVariableTypes.Number;
			case "proprietors":
				return FutureProgVariableTypes.Collection | FutureProgVariableTypes.Number;
			case "stock":
				return FutureProgVariableTypes.Collection | FutureProgVariableTypes.Item;
		}

		return FutureProgVariableTypes.Error;
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "name", FutureProgVariableTypes.Text },
			{ "merchandise", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Merchandise },
			{ "shopfront", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Location },
			{ "storeroom", FutureProgVariableTypes.Location },
			{ "workshop", FutureProgVariableTypes.Location },
			{ "tills", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Item },
			{ "employees", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Number },
			{ "employeesonduty", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Character },
			{ "managers", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Number },
			{ "proprietors", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Number },
			{ "stock", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Item }
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
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Shop, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}