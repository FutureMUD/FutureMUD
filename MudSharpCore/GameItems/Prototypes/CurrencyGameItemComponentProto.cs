using MudSharp.Accounts;
using MudSharp.Economy.Currency;
using MudSharp.Events;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Decorators;
using System.Globalization;

#nullable enable annotations

namespace MudSharp.GameItems.Prototypes;

public class CurrencyGameItemComponentProto : GameItemComponentProto, ICurrencyPilePrototype
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
    public ICurrency? DefaultCurrency { get; private set; }
    public decimal? DefaultAmount { get; private set; }
    public bool HasDefaultPile => DefaultCurrency != null && DefaultAmount.HasValue;
    public override bool ReadOnly => true;
    public override bool PreventManualLoad => !HasDefaultPile;
    public override string TypeDescription => "Currency Pile";

    public static void InitialiseItemType(IFuturemud gameworld)
    {
        ItemPrototype = gameworld.ItemProtos.SingleOrDefault(x =>
            x.Components.OfType<CurrencyGameItemComponentProto>().Any(c => !c.HasDefaultPile));
        if (ItemPrototype == null)
        {
            CurrencyGameItemComponentProto comp = new(gameworld, null);
            gameworld.Add(comp);
            comp.ChangeStatus(RevisionStatus.Current, "Automatically generated", null);
            GameItemProto proto = new(gameworld, null);
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
        var currencyAttribute = root.Attribute("DefaultCurrency");
        var amountAttribute = root.Attribute("DefaultAmount");
        if ((currencyAttribute == null) != (amountAttribute == null))
            throw new FormatException("Currency pile defaults require both DefaultCurrency and DefaultAmount.");
        if (currencyAttribute != null)
        {
            if (!long.TryParse(currencyAttribute.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var currencyId) ||
                !decimal.TryParse(amountAttribute!.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount) || amount <= 0m)
                throw new FormatException("Currency pile defaults require a valid currency ID and positive decimal amount.");
            DefaultCurrency = Gameworld.Currencies.Get(currencyId) ??
                              throw new FormatException($"Currency pile default references unresolved currency #{currencyId}.");
            var coins = DefaultCurrency.FindCoinsForAmount(amount, out var exact);
            if (!exact || coins.Count == 0 || coins.Any(x => x.Value <= 0))
                throw new FormatException("Currency pile default amount must be exactly representable as positive coins.");
            DefaultAmount = amount;
        }
        else
        {
            DefaultCurrency = null;
            DefaultAmount = null;
        }
        XAttribute attribute = root.Attribute("Decorator");
        Decorator = attribute != null
            ? Gameworld.StackDecorators.Get(long.Parse(attribute.Value))
            : Gameworld.StackDecorators.First();
    }

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        command.PopSpeech();
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
        var root = new XElement("Definition", new XAttribute("Decorator", Decorator.Id));
        if (HasDefaultPile)
        {
            root.Add(new XAttribute("DefaultCurrency", DefaultCurrency!.Id));
            root.Add(new XAttribute("DefaultAmount", DefaultAmount!.Value.ToString(CultureInfo.InvariantCulture)));
        }
        return root.ToString(SaveOptions.DisableFormatting);
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
		return CreateNewCurrencyPile(currency, coins.Select(x => (x.Item1, x.Item2)), temporary);
	}

    public static IGameItem CreateNewCurrencyPile(ICurrency currency, IEnumerable<KeyValuePair<ICoin, int>> coins,
        bool temporary = false)
	{
		return CreateNewCurrencyPile(currency, coins.Select(x => (x.Key, x.Value)), temporary);
	}

    public static IGameItem CreateNewCurrencyPile(ICurrency currency, IEnumerable<(ICoin, int)> coins,
        bool temporary = false)
	{
		var counts = coins.GroupBy(x => x.Item1).ToDictionary(x => x.Key, x => checked(x.Sum(y => y.Item2)));
		if (counts.Any(x => x.Value <= 0)) throw new ArgumentOutOfRangeException(nameof(coins));
		if (temporary)
		{
			var preview = new GameItem(ItemPrototype, null, ItemPrototype.BaseItemQuality,
				deferSpellInitialisation: false, currencyPreview: true);
			preview.GetItemType<CurrencyGameItemComponent>().SetPreviewCoins(currency, counts);
			return preview;
		}
		var newItem = ItemPrototype.CreateNew();
		var currencyItem = newItem.GetItemType<ICurrencyPile>();
		currencyItem.Currency = currency;
		currencyItem.AddCoins(counts);
		newItem.Login();
		newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
		return newItem;
	}

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
    {
        if (!HasDefaultPile) return new CurrencyGameItemComponent(this, parent, temporary);
        var coins = DefaultCurrency!.FindCoinsForAmount(DefaultAmount!.Value, out var exact);
        if (!exact || coins.Count == 0 || coins.Any(x => x.Value <= 0))
            throw new InvalidOperationException("Configured currency pile defaults are no longer exactly representable.");
        return new CurrencyGameItemComponent(this, parent, DefaultCurrency,
            coins.Select(x => Tuple.Create(x.Key, x.Value)), temporary);
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
