using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Decorators;

namespace MudSharp.GameItems.Prototypes;

public class CurrencyGameItemComponentProto : GameItemComponentProto
{
	private static readonly string _showString = "Currency Pile Item Component".Colour(Telnet.Cyan) + "\n\n" +
	                                             "This item is a currency pile.\n";

	protected CurrencyGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Currency Pile")
	{
		Decorator = Gameworld.StackDecorators.First();
	}

	protected CurrencyGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public static IGameItemProto ItemPrototype { get; set; }
	public IStackDecorator Decorator { get; private set; }
	public override bool ReadOnly => true;
	public override bool PreventManualLoad => true;
	public override string TypeDescription => "Currency Pile";

	public static void InitialiseItemType(IFuturemud gameworld)
	{
		ItemPrototype = gameworld.ItemProtos.SingleOrDefault(x => x.IsItemType<CurrencyGameItemComponentProto>());
		if (ItemPrototype == null)
		{
			var comp = new CurrencyGameItemComponentProto(gameworld, null);
			gameworld.Add(comp);
			comp.ChangeStatus(RevisionStatus.Current, "Automatically generated", null);
			var proto = new GameItemProto(gameworld, null);
			gameworld.Add(proto);
			HoldableGameItemComponentProto.InitialiseItemType(gameworld);
			proto.AddComponent(gameworld.ItemComponentProtos.Single(x => x is HoldableGameItemComponentProto));
			proto.AddComponent(comp);
			proto.Weight = 0;
			proto.ChangeStatus(RevisionStatus.Current, "Automatically generated", null);
			ItemPrototype = proto;
		}
	}

	protected override void LoadFromXml(XElement root)
	{
		var attribute = root.Attribute("Decorator");
		Decorator = attribute != null
			? Gameworld.StackDecorators.Get(long.Parse(attribute.Value))
			: Gameworld.StackDecorators.First();
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		command.Pop();
		return base.BuildingCommand(actor, command);
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor, "{0} (#{1:N0}r{2:N0}, {3})", "Currency Pile Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name);
	}

	protected override string SaveToXml()
	{
		return "<Definition Decorator=\"" + Decorator.Id + "\"/>";
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		// Do not register builder initialiser
		manager.AddDatabaseLoader("Currency Pile",
			(proto, gameworld) => new CurrencyGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"CurrencyPile",
			$"Marks an item as a {"[system-generated]".Colour(Telnet.Green)} currency pile. {"Do not edit this component or use manually.".Colour(Telnet.Red)}",
			"This component is used in auto-generated items only. It should not and cannot be used in any manually created items, nor should it be edited in any way."
		);
	}

	public static IGameItem CreateNewCurrencyPile(ICurrency currency, IEnumerable<Tuple<ICoin, int>> coins,
		bool temporary = false)
	{
		var newItem = ItemPrototype.CreateNew();
		var currencyItem = newItem.GetItemType<ICurrencyPile>();
		currencyItem.Currency = currency;
		currencyItem.AddCoins(coins);
		return newItem;
	}

	public static IGameItem CreateNewCurrencyPile(ICurrency currency, IEnumerable<(ICoin, int)> coins,
		bool temporary = false)
	{
		var newItem = ItemPrototype.CreateNew();
		var currencyItem = newItem.GetItemType<ICurrencyPile>();
		currencyItem.Currency = currency;
		currencyItem.AddCoins(coins);
		return newItem;
	}

	public static IGameItem CreateNewCurrencyPile(ICurrency currency, IEnumerable<KeyValuePair<ICoin, int>> coins,
		bool temporary = false)
	{
		var newItem = ItemPrototype.CreateNew();
		var currencyItem = newItem.GetItemType<ICurrencyPile>();
		currencyItem.Currency = currency;
		currencyItem.AddCoins(coins);
		return newItem;
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new CurrencyGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new CurrencyGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		throw new NotSupportedException("Currency Piles should not be edited.");
	}
}