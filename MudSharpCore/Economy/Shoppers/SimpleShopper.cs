using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Economy.Shoppers;
internal class SimpleShopper : ShopperBase
{
	public static void RegisterLoaders()
	{
		RegisterLoader(
			"simple",
			(game,dbitem) => new SimpleShopper(dbitem, game),
			(game,name,ez) => new SimpleShopper(game,name,ez),
			new SimpleShopper().HelpText,
			"A simple shopper that just spends money with custom logic"
			);
	}

	protected SimpleShopper(Models.Shopper dbitem, IFuturemud gameworld) : base(dbitem, gameworld)
	{
	}

	protected SimpleShopper(SimpleShopper rhs, string name) : base(rhs, name, "simple")
	{
	}

	protected SimpleShopper(IFuturemud gameworld, string name, IEconomicZone ez) : base(gameworld, name, ez)
	{
		BudgetPerShop = 0.0M;
		ItemBuyWeightProg = Gameworld.AlwaysOneProg;
		WillBuyItemProg = Gameworld.AlwaysTrueProg;
		WillShopAtShopProg = Gameworld.AlwaysTrueProg;
		ShopSelectionWeightProg = Gameworld.AlwaysOneProg;
		SkipEmptyShops = true;
		DoDatabaseInsert("simple");
	}

	protected SimpleShopper()
	{
	}

	/// <inheritdoc />
	public override IShopper Clone(string name)
	{
		return new SimpleShopper(this, name);
	}

	public decimal BudgetPerShop { get; private set; }
	public IFutureProg WillShopAtShopProg { get; private set; }
	public IFutureProg ShopSelectionWeightProg { get; private set; }
	public IFutureProg WillBuyItemProg { get; private set; }
	public IFutureProg ItemBuyWeightProg { get; private set; }
	public bool SkipEmptyShops { get; private set; }

	#region Overrides of ShopperBase

	/// <inheritdoc />
	protected override XElement SaveDefinition()
	{
		return new XElement("Shopper",
			new XElement("BudgetPerShop", BudgetPerShop),
			new XElement("WillShopAtShopProg", WillShopAtShopProg.Id),
			new XElement("ShopSelectionWeightProg", ShopSelectionWeightProg.Id),
			new XElement("WillBuyItemProg", WillBuyItemProg.Id),
			new XElement("ItemBuyWeightProg", ItemBuyWeightProg.Id),
			new XElement("SkipEmptyShops", SkipEmptyShops)
		);
	}

	/// <inheritdoc />
	protected override void LoadFromDefinition(XElement root)
	{
		BudgetPerShop = decimal.Parse(root.Element("BudgetPerShop").Value);
		WillShopAtShopProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WillShopAtShopProg").Value));
		WillBuyItemProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WillBuyItemProg").Value));
		ItemBuyWeightProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("ItemBuyWeightProg").Value));
		ShopSelectionWeightProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("ShopSelectionWeightProg").Value));
		SkipEmptyShops = bool.Parse(root.Element("SkipEmptyShops")?.Value ?? "true");
	}

	/// <inheritdoc />
	protected override string SubtypeHelpText => @"	#3budget <amount>#0 - how much the shopper will spend per trip
	#3shop <prog>#0 - sets the prog that controls whether a shop is selected
	#3shopweight <prog>#0 - sets the prog that determines the priority weight of a shop
	#3item <prog>#0 - sets the prog that controls whether an item is selected
	#3itemweight <prog>#0 - sets the prog that determines the priority weight of a shop
	#3skipempty#0 - skips empty shops when selecting";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "budget":
				return BuildingCommandBudget(actor, command);
			case "shop":
			case "shopprog":
				return BuildingCommandShopProg(actor, command);
			case "shopweight":
			case "shopweightprog":
				return BuildingCommandShopWeightProg(actor, command);
			case "item":
			case "itemprog":
				return BuildingCommandItemProg(actor, command);
			case "itemweight":
			case "itemweightprog":
				return BuildingCommandItemWeightProg(actor, command);
			case "skipempty":
				return BuildingCommandSkipEmpty(actor);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandSkipEmpty(ICharacter actor)
	{
		SkipEmptyShops = !SkipEmptyShops;
		Changed = true;
		actor.OutputHandler.Send($"This shopper will {SkipEmptyShops.NowNoLonger()} skip empty shops when deciding where to spend their money.");
		return true;
	}

	private bool BuildingCommandItemWeightProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What prog should be used to determine the priority weight of an item?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Number,
			[
				[ProgVariableTypes.Shop],
				[ProgVariableTypes.Shop, ProgVariableTypes.Merchandise],
				[ProgVariableTypes.Shop, ProgVariableTypes.Merchandise, ProgVariableTypes.Number]
			]
		).LookupProg();
		if (prog is null)
		{
			return false;
		}

		ItemBuyWeightProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"The {prog.MXPClickableFunctionName()} will now be used to determine the priority weight of an item.");
		return true;
	}

	private bool BuildingCommandItemProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What prog should be used to determine whether this shopper will buy an item?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean, 
			[
				[ProgVariableTypes.Shop],
				[ProgVariableTypes.Shop, ProgVariableTypes.Merchandise],
				[ProgVariableTypes.Shop, ProgVariableTypes.Merchandise, ProgVariableTypes.Number]
			]
			).LookupProg();
		if (prog is null)
		{
			return false;
		}

		WillBuyItemProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"The {prog.MXPClickableFunctionName()} will now be used to determine if this shopper buys an item.");
		return true;
	}

	private bool BuildingCommandShopWeightProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What prog should be used to determine the priority weighting of a particular shop?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Number, [ProgVariableTypes.Shop]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		ShopSelectionWeightProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"The {prog.MXPClickableFunctionName()} will now be used to determine the priority weighting of a shop.");
		return true;
	}

	private bool BuildingCommandShopProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What prog should be used to determine whether this shopper will patronise a particular shop?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean, [ProgVariableTypes.Shop]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		WillShopAtShopProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"The {prog.MXPClickableFunctionName()} will now be used to determine if this shopper patronises a shop.");
		return true;
	}

	private bool BuildingCommandBudget(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the budget per shop for this shopper?");
			return false;
		}

		if (!EconomicZone.Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var value) || value < 0.0M)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid amount of {EconomicZone.Currency.Name.ColourValue()}.");
			return false;
		}

		BudgetPerShop = value;
		Changed = true;
		actor.OutputHandler.Send($"This shopper will now spend up to {EconomicZone.Currency.Describe(value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} per shopping trip.");
		return true;
	}

	/// <inheritdoc />
	protected override string ShowSubtype(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Budget Per Shop: {EconomicZone.Currency.Describe(BudgetPerShop, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Will Shop At Shop: {WillShopAtShopProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Shop Selection Weight: {ShopSelectionWeightProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Will Buy Item: {WillBuyItemProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Item Buy Weight: {ItemBuyWeightProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Skip Empty Shops: {SkipEmptyShops.ToColouredString()}");
		return sb.ToString();
	}

	/// <inheritdoc />
	public override void DoShop()
	{
		var shop = Gameworld.Shops
		                    .Where(x => x.EconomicZone == EconomicZone)
		                    .Where(x => x.IsReadyToDoBusiness && x.IsTrading)
		                    .Where(x => WillShopAtShopProg.ExecuteBool(x))
		                    .Where(x => !SkipEmptyShops || x.AllMerchandiseForVirtualShoppers.Any())
							.GetWeightedRandom(x => ShopSelectionWeightProg.ExecuteDouble(1.0, x));
		if (shop is null)
		{
			DoLogEntry("noshop", "Couldn't find a valid shop to shop at. Skipped shopping.");
			FlushLogEntries();
			return;
		}
		
		var allItems = shop
		               .AllMerchandiseForVirtualShoppers
		               .Where(x => WillBuyItemProg.ExecuteBool(x.Item, x.Merchandise, x.Price))
		               .ToList();
		var budget = BudgetPerShop;

		DoLogEntry("shop", $"Selected the shop {shop.Name} with a budget of {EconomicZone.Currency.Describe(budget, CurrencyDescriptionPatternType.ShortDecimal)} and {allItems.Count:N0} valid {"item".Pluralise(allItems.Count != 1)}");
		while (budget > 0.0M)
		{
			// Update items remaining under budget
			allItems = allItems
			           .Where(x => x.Price <= budget)
			           .ToList();

			// Select a random item
			var item = allItems
				.GetWeightedRandom(x => ItemBuyWeightProg.ExecuteDouble(1.0, x.Item, x.Merchandise, x.Price));
			if (item.Item is null)
			{
				break;
			}

			// Work out how many to buy
			var quantity = 1;
			if (item.Item.Quantity > 1)
			{
				var maxQuantity = (int)Math.Floor(item.Price / item.Item.Quantity);
				if (maxQuantity > 1)
				{
					quantity = RandomUtilities.Random(1, maxQuantity);
				}
			}

			// Write to the log
			DoLogEntry("buy", $"Bought {quantity:N0}x {item.Item.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee)} for {EconomicZone.Currency.Describe(item.Price, CurrencyDescriptionPatternType.ShortDecimal)}");

			// Buy the item
			shop.BuyVirtualShopper(item.Merchandise, item.Item, quantity);
			budget -= quantity * item.Price;
		}
		DoLogEntry("finish", "Finished shopping (ran out of budget or valid items)");
		FlushLogEntries();

		SetupListener();
	}

	/// <inheritdoc />
	public override string ShopperType => "simple";

	#endregion
}
