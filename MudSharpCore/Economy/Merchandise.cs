using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework.Revision;
using MudSharp.Models;

namespace MudSharp.Economy;

public class Merchandise : LateInitialisingItem, IMerchandise
{
	public override string FrameworkItemType => "Merchandise";

	public Merchandise(IMerchandise rhs, string newName)
	{
		Gameworld = rhs.Gameworld;
		Gameworld.SaveManager.AddInitialisation(this);
		_name = newName;
		Shop = rhs.Shop;
		_itemId = rhs.Item.Id;
		BasePrice = rhs.BasePrice;
		DefaultMerchandiseForItem = false;
		PreferredDisplayContainer = rhs.PreferredDisplayContainer;
		_listDescription = (rhs as Merchandise)?._listDescription;
		AutoReorderPrice = rhs.AutoReorderPrice;
		AutoReordering = rhs.AutoReordering;
		PreserveVariablesOnReorder = rhs.PreserveVariablesOnReorder;
	}

	public Merchandise(IShop shop, string name, IGameItemProto proto, decimal price, bool isDefault,
		IGameItem preferredContainer, string customListDescription)
	{
		Gameworld = shop.Gameworld;
		Gameworld.SaveManager.AddInitialisation(this);
		_name = name;
		Shop = shop;
		_itemId = proto.Id;
		BasePrice = price;
		AutoReorderPrice = price;
		DefaultMerchandiseForItem = isDefault;
		PreferredDisplayContainer = preferredContainer;
		PreserveVariablesOnReorder = true;
		_listDescription = customListDescription;
	}

	public Merchandise(MudSharp.Models.Merchandise merch, IShop shop, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Shop = shop;
		_id = merch.Id;
		IdInitialised = true;
		_name = merch.Name;
		DefaultMerchandiseForItem = merch.DefaultMerchandiseForItem;
		BasePrice = merch.BasePrice;
		AutoReordering = merch.AutoReordering;
		AutoReorderPrice = merch.AutoReorderPrice;
		_listDescription = merch.ListDescription;
		_preferredDisplayContainerId = merch.PreferredDisplayContainerId;
		_itemId = merch.ItemProtoId;
		MinimumStockLevels = merch.MinimumStockLevels;
		MinimumStockLevelsByWeight = merch.MinimumStockLevelsByWeight;
		PreserveVariablesOnReorder = merch.PreserveVariablesOnReorder;
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.Merchandises.Find(Id);
		dbitem.AutoReorderPrice = AutoReorderPrice;
		dbitem.AutoReordering = AutoReordering;
		dbitem.BasePrice = BasePrice;
		dbitem.DefaultMerchandiseForItem = DefaultMerchandiseForItem;
		dbitem.ItemProtoId = _itemId;
		dbitem.ListDescription = _listDescription;
		dbitem.MinimumStockLevels = MinimumStockLevels;
		dbitem.MinimumStockLevelsByWeight = MinimumStockLevelsByWeight;
		dbitem.PreferredDisplayContainerId = PreferredDisplayContainer?.Id;
		dbitem.PreserveVariablesOnReorder = PreserveVariablesOnReorder;
		dbitem.Name = Name;
		dbitem.SkinId = _skinId;
		dbitem.ShopId = Shop.Id;
		Changed = false;
	}

	public override object DatabaseInsert()
	{
		var dbitem = new Models.Merchandise();
		FMDB.Context.Merchandises.Add(dbitem);
		dbitem.Name = Name;
		dbitem.ShopId = Shop.Id;
		dbitem.BasePrice = BasePrice;
		dbitem.AutoReordering = AutoReordering;
		dbitem.AutoReorderPrice = AutoReorderPrice;
		dbitem.PreserveVariablesOnReorder = PreserveVariablesOnReorder;
		dbitem.PreferredDisplayContainerId = PreferredDisplayContainer?.Id;
		dbitem.DefaultMerchandiseForItem = DefaultMerchandiseForItem;
		dbitem.ItemProtoId = _itemId;
		dbitem.MinimumStockLevels = MinimumStockLevels;
		dbitem.MinimumStockLevelsByWeight = MinimumStockLevelsByWeight;
		dbitem.ListDescription = _listDescription;
		dbitem.SkinId = _skinId;
		return dbitem;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		_id = ((MudSharp.Models.Merchandise)dbitem).Id;
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		void SendHelpText()
		{
			if (actor.IsAdministrator())
			{
				actor.OutputHandler.Send(@"You can use the following subcommands:

	#3shop merch set name <name>#0 - sets the name of this merchandise
	#3shop merch set proto <id>|<target>#0 - sets the item prototype associated with this merchandise
	#3shop merch set skin <id>|<name>#0 - sets the skin associated with this merchandise
	#3shop merch set skin clear#0 - clears a skin from this merchandise
	#3shop merch set default#0 - toggles whether this is the default for items of its prototype
	#3shop merch set price <price>#0 - sets the pre-tax price
	#3shop merch set desc clear#0 - clears a custom list description
	#3shop merch set desc <description>#0 - sets a custom list description
	#3shop merch set container clear#0 - clears a preferred display container
	#3shop merch set container <target>#0 - sets a preferred display container
	#3shop merch set reorder off#0 - turns auto-reordering off
	#3shop merch set reorder <price> <quantity> [<weight>]#0 - enables auto-reordering with the specified price, quantity and optional minimum weight
	#3shop merch set preserve#0 - toggles preservation of item variables when reordering".SubstituteANSIColour());
			}
			else
			{
				actor.OutputHandler.Send($@"You can use the following subcommands:

	#3shop merch set name <name>#0 - sets the name of this merchandise
	#3shop merch set proto <target>#0 - sets the item type associated with this merchandise
	#3shop merch set default#0 - toggles whether this is the default for similar items
	#3shop merch set price <price>#0 - sets the pre-tax price
	#3shop merch set desc clear#0 - clears a custom list description
	#3shop merch set desc <description>#0 - sets a custom list description
	#3shop merch set container clear#0 - clears a preferred display container
	#3shop merch set container <target>#0 - sets a preferred display container".SubstituteANSIColour());
			}
		}

		if (command.IsFinished)
		{
			SendHelpText();
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "proto":
				return BuildingCommandProto(actor, command);
			case "default":
				return BuildingCommandDefault(actor, command);
			case "price":
				return BuildingCommandPrice(actor, command);
			case "desc":
			case "description":
				return BuildingCommandDescription(actor, command);
			case "container":
			case "display":
				return BuildingCommandContainer(actor, command);
		}

		if (actor.IsAdministrator())
		{
			switch (command.Last.ToLowerInvariant())
			{
				case "auto":
				case "reorder":
					return BuildingCommandReorder(actor, command);
				case "preserve":
					return BuildingCommandPreserve(actor, command);
				case "skin":
					return BuildingCommandSkin(actor, command);
			}
		}

		SendHelpText();
		return false;
	}

	private bool BuildingCommandSkin(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a skin to go with this merchandise, or use {"clear".ColourCommand()} to clear it.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("clear", "none", "remove", "delete"))
		{
			_skinId = null;
			Changed = true;
			actor.OutputHandler.Send($"This merchandise will no longer have a skin associated with it.");
			return true;
		}

		IGameItemSkin skin;
		if (long.TryParse(command.SafeRemainingArgument, out var id))
		{
			skin = Gameworld.ItemSkins.Get(id);
		}
		else
		{
			skin = Gameworld.ItemSkins.Where(x => x.ItemProto == Item)
			                .GetByNameOrAbbreviation(command.SafeRemainingArgument);
		}

		if (skin is null)
		{
			actor.OutputHandler.Send("There is no such skin.");
			return false;
		}

		if (skin.ItemProto != Item)
		{
			actor.OutputHandler.Send(
				$"{skin.EditHeader().ColourName()} was not designed to be used with {Item.EditHeader().ColourObject()}.");
			return false;
		}

		if (skin.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send($"{skin.EditHeader().ColourName()} is not approved for use.");
			return false;
		}

		_skinId = skin.Id;
		Changed = true;
		actor.OutputHandler.Send($"This merchandise will now use {skin.EditHeader().ColourName()}.");
		return true;
	}

	#region Building Sub Commands

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this merchandise entry?");
			return false;
		}

		var name = command.PopSpeech();
		if (Shop.Merchandises.Except(this).Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"There is already a merchandise for this shop with that name. Names must be unique.");
			return false;
		}

		_name = name;
		Changed = true;
		actor.OutputHandler.Send($"You rename the merchandise entry to {Name.TitleCase().Colour(Telnet.Cyan)}.");
		return true;
	}

	private bool BuildingCommandProto(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which item should this merchandise entry be based off of?");
			return false;
		}

		IGameItemProto proto;
		var text = command.PopSpeech();
		if (actor.IsAdministrator() && long.TryParse(text, out var value))
		{
			proto = Gameworld.ItemProtos.Get(value);
			if (proto == null)
			{
				actor.OutputHandler.Send("There is no such item prototype.");
				return false;
			}
		}
		else
		{
			var target = actor.TargetLocalOrHeldItem(text);
			if (target == null)
			{
				actor.OutputHandler.Send("You don't see anything like that.");
				return false;
			}

			proto = target.Prototype;
		}

		_itemId = proto.Id;
		_skinId = null;
		Changed = true;
		actor.OutputHandler.Send(
			$"This merchandise record will now be based on the item {Item.ShortDescription.ColourObject()}{(actor.IsAdministrator() ? $" ({Item.Id.ToString("N0", actor)}r{Item.RevisionNumber.ToString("N0", actor)})" : "")}");
		return true;
	}

	private bool BuildingCommandDefault(ICharacter actor, StringStack command)
	{
		DefaultMerchandiseForItem = !DefaultMerchandiseForItem;
		Changed = true;
		actor.OutputHandler.Send(
			$"This merchandise record will {(DefaultMerchandiseForItem ? "now" : "no longer")} be used as the default for identical items.");
		return true;
	}

	private bool BuildingCommandPrice(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should the pre-tax price for this merchandise be?");
			return false;
		}

		var amount = Shop.Currency.GetBaseCurrency(command.SafeRemainingArgument, out var success);
		if (!success)
		{
			actor.OutputHandler.Send("That is not a valid price.");
			return false;
		}

		var old = BasePrice;
		BasePrice = amount;
		Changed = true;
		actor.OutputHandler.Send(
			$"This merchandise will now have a pre-tax price of {Shop.Currency.Describe(BasePrice, CurrencyDescriptionPatternType.Short)}.");
		Shop.PriceAdjustmentForMerchandise(this, old, actor);
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What description do you want to give this merchandise for the LIST command? You can use CLEAR to reset it to the item prototype's description.");
			return false;
		}

		if (command.Peek().EqualToAny("clear", "reset", "delete"))
		{
			_listDescription = string.Empty;
			actor.OutputHandler.Send(
				$"You reset the description to be tied to the item prototype's description, which is {ListDescription.ColourObject()}.");
			Changed = true;
			return true;
		}

		_listDescription = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send(
			$"This merchandise will now appear in the LIST command as {ListDescription.ColourObject()}.");
		return true;
	}

	private bool BuildingCommandContainer(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must either supply a target container to set as the preferred display container, or use CLEAR to clear an existing entry.");
			return false;
		}

		if (command.Peek().EqualToAny("clear", "reset", "delete"))
		{
			PreferredDisplayContainer = null;
			actor.OutputHandler.Send($"This merchandise will no longer have a preferred display container.");
			Changed = true;
			return true;
		}

		var targetItem = actor.TargetLocalItem(command.PopSpeech());
		if (targetItem == null)
		{
			actor.OutputHandler.Send("You don't see anything like that here.");
			return false;
		}

		if (!targetItem.IsItemType<IContainer>())
		{
			actor.OutputHandler.Send($"{targetItem.HowSeen(actor, true)} is not a container.");
			return false;
		}

		PreferredDisplayContainer = targetItem;
		Changed = true;
		actor.OutputHandler.Send($"This merchandise will now prefer to be displayed in {targetItem.HowSeen(actor)}.");
		return true;
	}

	private bool BuildingCommandPreserve(ICharacter actor, StringStack command)
	{
		PreserveVariablesOnReorder = !PreserveVariablesOnReorder;
		Changed = true;
		actor.OutputHandler.Send(
			$"This merchandise will {(PreserveVariablesOnReorder ? "now" : "no longer")} preserve its previous variables on reorder.");
		return true;
	}

	private bool BuildingCommandReorder(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either use REORDER OFF to switch reordering functionality off or REORDER <price> <minquantity> [<minweight>].");
			return false;
		}

		if (command.Peek().EqualTo("off"))
		{
			AutoReordering = false;
			Changed = true;
			actor.OutputHandler.Send("This merchandise will no longer be automatically reordered.");
			return true;
		}

		var price = Shop.Currency.GetBaseCurrency(command.PopSpeech(), out var success);
		if (!success)
		{
			actor.OutputHandler.Send("That is not a valid price.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What minimum quantity of items on stock would you like to have for this merchandise?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must enter a quantity greater than or equal to one.");
			return false;
		}

		var weight = 0.0;
		if (!command.IsFinished)
		{
			weight = Gameworld.UnitManager.GetBaseUnits(command.PopSpeech(), Framework.Units.UnitType.Mass,
				out success);
			if (!success)
			{
				actor.OutputHandler.Send("That is not a valid weight.");
				return false;
			}
		}

		AutoReordering = true;
		AutoReorderPrice = price;
		MinimumStockLevels = value;
		MinimumStockLevelsByWeight = weight;
		Changed = true;
		actor.OutputHandler.Send(
			$"This merchandise is now auto-reordering at {Shop.Currency.Describe(AutoReorderPrice, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} with minimum quantity {MinimumStockLevels.ToString("N0", actor).ColourValue()}{(MinimumStockLevelsByWeight > 0.0 ? $" and minimum weight {Gameworld.UnitManager.DescribeExact(MinimumStockLevelsByWeight, Framework.Units.UnitType.Mass, actor).ColourValue()}" : "")}.");
		return true;
	}

	#endregion

	public void ShowToBuilder(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Merchandise Record #{Id.ToString("N0", actor)}: {Name}");
		if (Item != null)
		{
			if (actor.IsAdministrator())
			{
				sb.AppendLine(
					$"Item Proto: {Item.ShortDescription.ColourObject()}{(DefaultMerchandiseForItem ? " [default for item]".Colour(Telnet.BoldWhite) : "")} ({Item.Id.ToString("N0", actor)}r{Item.RevisionNumber.ToString("N0", actor)})");
			}
			else
			{
				sb.AppendLine(
					$"Item Proto: {Item.ShortDescription.ColourObject()}{(DefaultMerchandiseForItem ? " [default for item]".Colour(Telnet.BoldWhite) : "")}");
			}
		}
		else
		{
			sb.AppendLine($"Item Proto: {"Any".Colour(Telnet.BoldCyan)}");
		}

		sb.AppendLine($"List Description: {ListDescription}");
		sb.AppendLine(
			$"Pre-Tax Price: {Shop.Currency.Describe(BasePrice, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine(
			$"Preferred Display Container: {PreferredDisplayContainer?.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee) ?? "None".Colour(Telnet.Red)}");

		// TODO - sales

		if (actor.IsAdministrator())
		{
			sb.AppendLine();
			sb.AppendLine($"Auto Reordering: {AutoReordering.ToString(actor).ColourValue()}");
			if (AutoReordering)
			{
				sb.AppendLine(
					$"Auto Reorder Price: {Shop.Currency.Describe(AutoReorderPrice, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
				sb.AppendLine(
					$"Minimum Stock Levels: {MinimumStockLevels.ToString("N0", actor).ColourValue()}{(MinimumStockLevelsByWeight > 0.0 ? $" by quantity or {Gameworld.UnitManager.DescribeExact(MinimumStockLevelsByWeight, Framework.Units.UnitType.Mass, actor).ColourValue()} by weight" : "")}");
				sb.AppendLine($"Preserve Variables: {PreserveVariablesOnReorder.ToString(actor).ColourValue()}");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	public IShop Shop { get; private set; }
	public bool AutoReordering { get; private set; }
	public decimal AutoReorderPrice { get; private set; }
	public bool PreserveVariablesOnReorder { get; private set; }
	public int MinimumStockLevels { get; private set; }
	public double MinimumStockLevelsByWeight { get; private set; }
	public bool DefaultMerchandiseForItem { get; private set; }
	private long _itemId;
	public IGameItemProto Item => Gameworld.ItemProtos.Get(_itemId);
	private long? _skinId;
	public IGameItemSkin Skin => Gameworld.ItemSkins.Get(_skinId ?? 0);
	public decimal BasePrice { get; private set; }

	public IGameItem PreferredDisplayContainer
	{
		get
		{
			if (_preferredDisplayContainer == null)
			{
				_preferredDisplayContainer = Gameworld.TryGetItem(_preferredDisplayContainerId ?? 0, false);
			}

			return _preferredDisplayContainer;
		}
		private set
		{
			_preferredDisplayContainer = value;
			_preferredDisplayContainerId = value?.Id ?? 0;
		}
	}

	private string _listDescription;
	private long? _preferredDisplayContainerId;
	private IGameItem _preferredDisplayContainer;

	public string ListDescription
	{
		get
		{
			if (!string.IsNullOrEmpty(_listDescription))
			{
				return _listDescription;
			}

			return Skin?.ShortDescription ?? Item.ShortDescription;
		}
	}

	public bool IsMerchandiseFor(IGameItem item)
	{
		return item.EffectsOfType<ItemOnDisplayInShop>().Any(x => x.Merchandise == this) ||
		       (DefaultMerchandiseForItem && Item.Id == item.Prototype.Id && (_skinId is null || item.Skin == Skin))
			;
	}

	public IEnumerable<string> Keywords => ListDescription.Strip_A_An().Split(' ', '-', ',');

	#region IFutureProgVariable Implementation

	public FutureProgVariableTypes Type => FutureProgVariableTypes.Merchandise;
	public object GetObject => this;

	public IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "name":
				return new TextVariable(Name);
			case "id":
				return new NumberVariable(Id);
			case "price":
				return new NumberVariable(BasePrice);
			case "description":
				return new TextVariable(ListDescription);
			case "shop":
				return Shop;
		}

		throw new ApplicationException("Unknown property in Merchandise.GetProperty");
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "name", FutureProgVariableTypes.Text },
			{ "id", FutureProgVariableTypes.Number },
			{ "price", FutureProgVariableTypes.Number },
			{ "description", FutureProgVariableTypes.Text },
			{ "shop", FutureProgVariableTypes.Shop }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "name", "" },
			{ "id", "" },
			{ "price", "" },
			{ "description", "" },
			{ "shop", "" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Merchandise, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}