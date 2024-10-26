using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.RPG.Law;
using Exit = MudSharp.Construction.Boundary.Exit;
using Law = MudSharp.RPG.Law.Law;
using LegalAuthority = MudSharp.RPG.Law.LegalAuthority;

namespace MudSharp.Effects.Concrete;

public class ItemOnDisplayInShop : Effect, IDescriptionAdditionEffect, IHandleEventsEffect
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
				if (_merchandise is not null)
				{
					_merchandise.OnDelete -= Merchandise_OnDelete;
					_merchandise.OnDelete += Merchandise_OnDelete;
				}
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
		_shop = Gameworld.Shops.Get(_shopId);
		_merchId = long.Parse(root.Element("Merch").Value);
		_merchandise = Shop?.Merchandises.FirstOrDefault(x => x.Id == _merchId);
		if (_merchandise is not null)
		{
			_merchandise.OnDelete -= Merchandise_OnDelete;
			_merchandise.OnDelete += Merchandise_OnDelete;
		}
	}

	private void Merchandise_OnDelete(object sender, EventArgs e)
	{
		Owner.RemoveEffect(this, true);
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

	/// <inheritdoc />
	/// <inheritdoc />
	public override void RemovalEffect()
	{
		if (!Merchandise.PermitItemDecayOnStockedItems)
		{
			((IGameItem)Owner).StartMorphTimer();
		}
	}

	/// <inheritdoc />
	public override void InitialEffect()
	{
		if (!Merchandise.PermitItemDecayOnStockedItems)
		{
			((IGameItem)Owner).EndMorphTimer();
		}
	}

	/// <inheritdoc />
	public override void Login()
	{
		base.Login();
		if (!Merchandise.PermitItemDecayOnStockedItems)
		{
			((IGameItem)Owner).EndMorphTimer();
		}
	}

	/// <inheritdoc />
	public bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		switch (type)
		{
			case EventType.CharacterLeaveCellItems:
				var ch = (ICharacter)arguments[0];
				var exit = (ICellExit)arguments[2];
				if (exit is null)
				{
					return false;
				}

				if (exit.Origin.Shop == Shop && exit.Destination.Shop != Shop)
				{
					if (!Shop.IsEmployee(ch))
					{
						CrimeExtensions.CheckPossibleCrimeAllAuthorities(ch, CrimeTypes.Theft, null, (IGameItem)Owner, "shoplifting");
						Owner.RemoveEffect(this, true);
					}
					else
					{
						Shop.DisposeFromStock(ch, (IGameItem)Owner);
					}
				}

				break;

		}

		return false;
	}

	/// <inheritdoc />
	public bool HandlesEvent(params EventType[] types)
	{
		return types.Contains(EventType.CharacterLeaveCellItems);
	}
}