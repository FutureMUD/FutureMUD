using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Work.Crafts.Products;

public class MoneyProduct : BaseProduct
{
	protected MoneyProduct(Models.CraftProduct product, ICraft craft, IFuturemud gameworld) : base(product, craft,
		gameworld)
	{
		var root = XElement.Parse(product.Definition);
		Currency = Gameworld.Currencies.Get(long.Parse(root.Element("Currency").Value));
		Amount = decimal.Parse(root.Element("Amount").Value);
	}

	protected MoneyProduct(ICraft craft, IFuturemud gameworld, bool failproduct) : base(craft, gameworld, failproduct)
	{
		Currency = Gameworld.Currencies.Get(Gameworld.GetStaticLong("DefaultCurrencyID"));
	}

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Currency", Currency?.Id ?? 0),
			new XElement("Amount", Amount)
		).ToString();
	}

	public override ICraftProductData ProduceProduct(IActiveCraftGameItemComponent component,
		ItemQuality referenceQuality)
	{
		var newitem = GameItems.Prototypes.CurrencyGameItemComponentProto.CreateNewCurrencyPile(Currency,
			Currency.FindCoinsForAmount(Amount, out var _).Select(x => Tuple.Create<ICoin, int>(x.Key, x.Value)));
		newitem.RoomLayer = component.Parent.RoomLayer;
		Gameworld.Add(newitem);
		return new SimpleProductData(new[]
		{
			newitem
		});
	}

	public override string ProductType => "MoneyProduct";

	public static void RegisterCraftProduct()
	{
		CraftProductFactory.RegisterCraftProductType("MoneyProduct",
			(product, craft, game) => new MoneyProduct(product, craft, game));
		CraftProductFactory.RegisterCraftProductTypeForBuilders("money",
			(craft, game, fail) => new MoneyProduct(craft, game, fail));
	}

	public ICurrency Currency { get; set; }
	public decimal Amount { get; set; }

	public override string Name => Currency == null
		? "an unspecified amount of currency"
		: Currency.Describe(Amount, CurrencyDescriptionPatternType.Short);

	public override string HowSeen(IPerceiver voyeur)
	{
		return Currency == null
			? "an unspecified amount of currency"
			: Currency.Describe(Amount, CurrencyDescriptionPatternType.Short);
	}

	public override bool IsValid()
	{
		return Currency != null && Amount > 0;
	}

	public override string WhyNotValid()
	{
		if (Currency == null)
		{
			return "You must first set a currency for this product.";
		}

		if (Amount <= 0)
		{
			return "You must set a positive, non-zero amount of currency to load.";
		}

		throw new ApplicationException("Unknown WhyNotValid reason in MoneyProduct.");
	}

	protected override string BuildingHelpText =>
		$"{base.BuildingHelpText}\n\tcurrency <currency> - sets the target currency\n\tamount <amount> - sets the amount of currency";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "currency":
				return BuildingCommandCurrency(actor, command);
			case "amount":
			case "value":
				return BuildingCommandAmount(actor, command);
		}

		return base.BuildingCommand(actor, command);
	}

	private bool BuildingCommandCurrency(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which currency should this product use?");
			return false;
		}

		var currency = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Currencies.Get(value)
			: Gameworld.Currencies.GetByName(command.Last);
		if (currency == null)
		{
			actor.OutputHandler.Send("There is no such currency.");
			return false;
		}

		Currency = currency;
		ProductChanged = true;
		actor.OutputHandler.Send($"This product will now use the {Currency.Name.Colour(Telnet.Cyan)} currency.");
		return true;
	}

	private bool BuildingCommandAmount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much money should this product load?");
			return false;
		}

		if (Currency == null)
		{
			actor.OutputHandler.Send("You should set a currency first, before you set an amount.");
			return false;
		}

		var amount = Currency.GetBaseCurrency(command.SafeRemainingArgument, out var success);
		if (!success)
		{
			actor.OutputHandler.Send(
				$"That is not a valid amount of currency in the {Currency.Name.Colour(Telnet.Cyan)} currency.");
			return false;
		}

		Amount = amount;
		ProductChanged = true;
		actor.OutputHandler.Send(
			$"This product will now load up {Currency.Describe(Amount, CurrencyDescriptionPatternType.Short).Colour(Telnet.Green)} in the {Currency.Name.Colour(Telnet.Cyan)} currency.");
		return true;
	}
}