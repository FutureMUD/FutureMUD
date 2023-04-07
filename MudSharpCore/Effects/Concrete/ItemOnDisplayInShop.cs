using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;

namespace MudSharp.Effects.Concrete;

public class ItemOnDisplayInShop : Effect, IDescriptionAdditionEffect
{
	private long _shopId;
	private IShop _shop;

	public IShop Shop
	{
		get
		{
			if (_shop == null)
			{
				_shop = Gameworld.Shops.Get(_shopId);
			}

			return _shop;
		}
	}

	private long _merchId;
	private IMerchandise _merchandise;

	public IMerchandise Merchandise
	{
		get
		{
			if (_merchandise == null)
			{
				_merchandise = Shop.Merchandises.FirstOrDefault(x => x.Id == _merchId);
			}

			return _merchandise;
		}
	}

	public ItemOnDisplayInShop(IPerceivable owner, IShop shop, IMerchandise merch) : base(owner)
	{
		_shopId = shop.Id;
		_shop = shop;
		_merchId = merch.Id;
	}

	protected ItemOnDisplayInShop(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		_shopId = long.Parse(root.Element("Shop").Value);
		_merchId = long.Parse(root.Element("Merch").Value);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"On display in shop {Shop.Name.Colour(Telnet.Cyan)}.";
	}

	protected override string SpecificEffectType => "ItemOnDisplayInShop";

	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("Shop", _shopId),
			new XElement("Merch", _merchId)
		);
	}

	public override bool Applies(object target)
	{
		if (target is IShop)
		{
			return Shop == target;
		}

		if (target is IMerchandise)
		{
			return Merchandise == target;
		}

		return base.Applies(target);
	}

	public override bool PreventsItemFromMerging(IGameItem effectOwnerItem, IGameItem targetItem)
	{
		return !targetItem.AffectedBy<ItemOnDisplayInShop>(Merchandise);
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("ItemOnDisplayInShop", (effect, owner) => new ItemOnDisplayInShop(effect, owner));
	}

	public string GetAdditionalText(IPerceiver voyeur, bool colour)
	{
		var merch = Merchandise;
		if (merch == null)
		{
			return string.Empty;
		}

		var (truth, reason) = Shop.CanBuy(voyeur as ICharacter, merch, 1, null);
		if (!truth)
		{
			return $"This item is for sale, but you cannot buy it because {reason}.".FluentColour(Telnet.BoldYellow,
				colour);
		}

		var price = Shop.PriceForMerchandise(voyeur as ICharacter, merch, 1);
		return
			$"This item is for sale for {Shop.Currency.Describe(price, CurrencyDescriptionPatternType.ShortDecimal)}."
				.FluentColour(Telnet.BoldYellow, colour);
	}

	public bool PlayerSet => false;
}