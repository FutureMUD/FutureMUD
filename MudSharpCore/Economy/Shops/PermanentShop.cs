using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Framework;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Effects.Concrete;
using MudSharp.Economy.Payment;
using MudSharp.GameItems.Prototypes;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems.Components;
using MudSharp.Events;
using MudSharp.Models;
using MudSharp.Accounts;
using System.Numerics;
using MailKit.Net.Smtp;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Economy.Shops;

public class PermanentShop : Shop, IPermanentShop
{
	public PermanentShop(Models.Shop shop, IFuturemud gameworld) : base(shop, gameworld)
	{
		foreach (var cell in shop.ShopsStoreroomCells.SelectNotNull(x => gameworld.Cells.Get(x.CellId)))
		{
			AddShopfrontCell(cell);
		}

		_stockroomCell = gameworld.Cells.Get(shop.StockroomCellId ?? 0);
		if (_stockroomCell is not null)
		{
			AddCellToStore(_stockroomCell);
		}

		_workshopCell = gameworld.Cells.Get(shop.WorkshopCellId ?? 0);
		if (_workshopCell is not null)
		{
			AddCellToStore(_workshopCell);
		}

		foreach (var item in shop.ShopsTills)
		{
			_tillItemIds.Add(item.GameItemId);
		}

		Changed = false;

		InitialiseShop();
	}

	public PermanentShop(IEconomicZone zone, ICell originalShopFront, string name) : base(zone, originalShopFront, name, "Permanent")
	{
		AddShopfrontCell(originalShopFront);
		InitialiseShop();
	}

	protected override void InitialiseShop()
	{
		foreach (var cell in AllShopCells)
		{
			cell.Shop = this;
		}
	}

	public override void PostLoadInitialisation()
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

	protected override void Save(Models.Shop dbitem)
	{
		dbitem.StockroomCellId = StockroomCell?.Id;
		dbitem.WorkshopCellId = WorkshopCell?.Id;
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

	private void AddCellToStore(ICell cell)
	{
		cell.Shop = this;
		cell.CellRequestsDeletion -= Cell_CellRequestsDeletion;
		cell.CellRequestsDeletion += Cell_CellRequestsDeletion;
		cell.CellProposedForDeletion -= Cell_CellProposedForDeletion;
		cell.CellProposedForDeletion += Cell_CellProposedForDeletion;
	}

	private void Cell_CellProposedForDeletion(ICell cell, ProposalRejectionResponse response)
	{
		if (cell == WorkshopCell)
		{
			response.RejectWithReason($"That room is a workshop room for shop #{Id:N0} ({Name.ColourName()})");
			return;
		}

		if (cell == StockroomCell)
		{
			response.RejectWithReason($"That room is a stockroom for shop #{Id:N0} ({Name.ColourName()})");
			return;
		}
	}

	private void Cell_CellRequestsDeletion(object sender, EventArgs e)
	{
		var cell = (ICell)sender;
		RemoveShopfrontCell(cell);
	}

	public void AddShopfrontCell(ICell cell)
	{
		if (!_shopfrontCells.Contains(cell))
		{
			AddCellToStore(cell);
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

	private readonly List<ICell> _shopfrontCells = new();
	public IEnumerable<ICell> ShopfrontCells => _shopfrontCells;
	private ICell _workshopCell;
	private ICell _stockroomCell;
	public ICell WorkshopCell
	{
		get => _workshopCell;
		set
		{
			if (_workshopCell != null && value != _workshopCell)
			{
				RemoveCellFromStore(_workshopCell);
			}

			RemoveCellFromStore(value);
			AddCellToStore(value);
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

			RemoveCellFromStore(value);
			AddCellToStore(value);
			_stockroomCell = value;
			Changed = true;
		}
	}
	public IEnumerable<ICell> AllShopCells => ShopfrontCells.Concat(new[]
	{
		WorkshopCell,
		StockroomCell
	}.WhereNotNull(x => x));

	private readonly HashSet<long> _tillItemIds = new();

	public IEnumerable<IGameItem> TillItems =>
		AllShopCells.SelectMany(x => x.GameItems).Where(x => _tillItemIds.Contains(x.Id));

	private readonly HashSet<long> _displayContainerIds = new();

	public IEnumerable<IGameItem> DisplayContainers => AllShopCells.SelectMany(x => x.GameItems)
																   .Where(x => _displayContainerIds.Contains(x.Id));

	public override IEnumerable<IGameItem> DoAutoRestockForMerchandise(IMerchandise merchandise, List<(IGameItem Item, IGameItem Container)> purchasedItems = null)
	{
		var quantityToRestock = merchandise.MinimumStockLevels - _stockedMerchandiseCounts[merchandise];
		var originalQuantity = quantityToRestock;
		var newItems = new List<IGameItem>();
		var newItemsOriginalContainers = new DictionaryWithDefault<IGameItem, IGameItem>();
		if (merchandise.PreserveVariablesOnReorder && purchasedItems != null)
		{
			foreach (var item in purchasedItems)
			{
				var newItem = item.Item.DeepCopy(true);
				newItem.Skin = merchandise.Skin;
				newItems.Add(newItem);
				newItemsOriginalContainers[newItem] = item.Container;
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

		foreach (var item in newItems)
		{
			item.AddEffect(new ItemOnDisplayInShop(item, this, merchandise));
			SortItemToStorePhysicalLocation(item, merchandise, newItemsOriginalContainers[item]);
			item.HandleEvent(EventType.ItemFinishedLoading, item);
			item.Login();
			_stockedMerchandise.Add(merchandise, item.Id);
			_stockedMerchandiseCounts[merchandise] += item.Quantity;
		}

		AddTransaction(new TransactionRecord(ShopTransactionType.Restock, Currency, this,
			ShopfrontCells.First().DateTime(), null, merchandise.EffectiveAutoReorderPrice * originalQuantity, 0.0M, merchandise));
		return newItems;
	}

	/// <inheritdoc />
	public override void SortItemToStorePhysicalLocation(IGameItem item, IMerchandise merchandise, IGameItem container)
	{
		if (container is not null && container.GetItemType<IContainer>()?.CanPut(item) == true)
		{
			container.GetItemType<IContainer>().Put(null, item, false);
		}
		else if (merchandise.PreferredDisplayContainer is not null &&
				 merchandise.PreferredDisplayContainer.GetItemType<IContainer>()?.CanPut(item) == true)
		{
			merchandise.PreferredDisplayContainer.GetItemType<IContainer>().Put(null, item, false);
		}
		else
		{

			var targetCell = StockroomCell;
			if (targetCell == null)
			{
				targetCell = ShopfrontCells.First();
			}
			targetCell.Insert(item);
		}
	}

	public override IEnumerable<IGameItem> DoAutostockForMerchandise(IMerchandise merchandise)
	{
		var stocked = new List<IGameItem>();
		foreach (var item in AllShopCells.SelectMany(x => x.GameItems).SelectMany(x => x.DeepItems).ToList())
		{
			if (item.AffectedBy<ItemOnDisplayInShop>(this))
			{
				continue;
			}

			var merch = Merchandises.FirstOrDefault(x => x.IsMerchandiseFor(item));
			if (merch == null)
			{
				continue;
			}

			AddToStock(null, item, merch);
			stocked.Add(item);
		}

		return stocked;
	}

	public override IEnumerable<IGameItem> StockedItems(IMerchandise merchandise)
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

	public override IEnumerable<IGameItem> AllStockedItems =>
		ShopfrontCells
		.SelectMany(x => x.Characters)
		.SelectMany(x => x.Body.HeldItems)
		.Concat(
			ShopfrontCells
			.ConcatIfNotNull(StockroomCell)
			.SelectMany(x => x.GameItems)
			.SelectMany(x => x.ShallowItems)
			)
		.Where(x => x.AffectedBy<ItemOnDisplayInShop>());

	public override void CheckFloat()
	{
		var cashInRegister =
			GetCurrencyPilesForShop()
				.Where(x => x.Currency == Currency)
				.Sum(x => x.Coins.Sum(y => y.Item2 * y.Item1.Value)) + CashBalance;
		if (cashInRegister > ExpectedCashBalance)
		{
			AddTransaction(new TransactionRecord(ShopTransactionType.Deposit, Currency, this,
				EconomicZone.ZoneForTimePurposes.DateTime(), null, cashInRegister - ExpectedCashBalance, 0.0M, null));
			ExpectedCashBalance = cashInRegister;
			Changed = true;
			return;
		}

		if (cashInRegister < ExpectedCashBalance)
		{
			AddTransaction(new TransactionRecord(ShopTransactionType.Withdrawal, Currency, this,
				EconomicZone.ZoneForTimePurposes.DateTime(), null, ExpectedCashBalance - cashInRegister, 0.0M, null));
			ExpectedCashBalance = cashInRegister;
			Changed = true;
			return;
		}
	}
	protected override void ShowInfo(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine();
		if (IsEmployee(actor) || actor.IsAdministrator())
		{
			if (actor.IsAdministrator())
			{
				sb.AppendLine($"These are the locations for this store:");
				sb.AppendLine($"\tWorkshop: {WorkshopCell?.GetFriendlyReference(actor) ?? "None".ColourError()}");
				sb.AppendLine($"\tStockroom: {StockroomCell?.GetFriendlyReference(actor) ?? "None".ColourError()}");
				foreach (var cell in ShopfrontCells)
				{
					sb.AppendLine($"\tShopfront: {cell.GetFriendlyReference(actor)}");
				}
			}

			else
			{
				if (actor.Location == WorkshopCell)
				{
					sb.AppendLine("The location you are currently in is the workshop for this store.".ColourCommand());
				}
				else if (actor.Location == StockroomCell)
				{
					sb.AppendLine("The location you are currently in is the stockroom for this store.".ColourCommand());
				}
				else if (ShopfrontCells.Contains(actor.Location))
				{
					sb.AppendLine("The location you are currently in is the shopfront for this store.".ColourCommand());
				}
			}

			if (TillItems.Any())
			{
				sb.AppendLine();
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
	}

	public override (int OnFloorCount, int InStockroomCount) StocktakeMerchandise(IMerchandise whichMerchandise)
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

	protected override (bool Truth, string Reason) CanBuyInternal(ICharacter actor, IMerchandise merchandise, int quantity, IPaymentMethod method, string extraArguments = null)
	{
		if (method is CashPayment)
		{
			if (!TillItems.Any() && StockroomCell is null)
			{
				return (false, "the store is currently missing its till, and so cannot do cash transactions.");
			}
		}

		return (true, string.Empty);
	}

	/// <inheritdoc />
	public override (bool Truth, string Reason) CanSellInternal(ICharacter actor, IMerchandise merchandise, IPaymentMethod method,
		IGameItem item)
	{
		return (true, string.Empty);
	}

	public override IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "shopfront":
				return new CollectionVariable(ShopfrontCells.ToList(), ProgVariableTypes.Location);
			case "storeroom":
				return StockroomCell;
			case "workshop":
				return WorkshopCell;
			case "tills":
				return new CollectionVariable(TillItems.ToList(), ProgVariableTypes.Item);
			default:
				return base.GetProperty(property);
		}
	}

	public override IEnumerable<ICell> CurrentLocations => ShopfrontCells;

	public override bool IsReadyToDoBusiness => TillItems.Any() || StockroomCell is not null;

	public override IReadOnlyDictionary<ICurrencyPile, Dictionary<ICoin, int>> GetCurrencyForShop(decimal amount)
	{
		if (TillItems.Any())
		{
			return Currency.FindCurrency(TillItems.SelectMany(x => x.RecursiveGetItems<ICurrencyPile>()).ToList(), amount);
		}

		if (StockroomCell is not null)
		{
			return Currency.FindCurrency(StockroomCell.GameItems.SelectMany(x => x.RecursiveGetItems<ICurrencyPile>()).ToList(), amount);
		}

		return new Dictionary<ICurrencyPile, Dictionary<ICoin, int>>();
	}

	public override IEnumerable<ICurrencyPile> GetCurrencyPilesForShop()
	{
		var piles = new List<ICurrencyPile>();
		piles.AddRange(TillItems.SelectMany(x => x.RecursiveGetItems<ICurrencyPile>()));
		if (StockroomCell is not null)
		{
			piles.AddRange(StockroomCell.GameItems
			                            .Where(x => !TillItems.Contains(x))
			                            .SelectMany(x => x.RecursiveGetItems<ICurrencyPile>()));
		}
		return piles;
	}

	public override void AddCurrencyToShop(IGameItem currencyPile)
	{
		foreach (var item in TillItems)
		{
			var itemContainer = item.GetItemType<IContainer>();
			if (itemContainer is null)
			{
				continue;
			}

			itemContainer.Put(null, currencyPile);
			return;
		}

		if (StockroomCell is not null)
		{
			StockroomCell.Insert(currencyPile);
			return;
		}

		if (ShopfrontCells.Any())
		{
			ShopfrontCells.First().Insert(currencyPile);
			Gameworld.SystemMessage($"The shop {Name.ColourName()} inserted money on the public ground because it had no tills or stockroom.", true);
			return;
		}

		var currencyItem = currencyPile.GetItemType<ICurrencyPile>();
		Gameworld.SystemMessage($"The shop {Name.ColourName()} junked {currencyItem.Currency.Describe(currencyItem.TotalValue, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} because it had nowhere to put it.", true);
	}
}
