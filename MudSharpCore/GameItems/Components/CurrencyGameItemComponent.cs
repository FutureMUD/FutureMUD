using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Economy.Currency;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class CurrencyGameItemComponent : GameItemComponent, ICurrencyPile
{
	protected CurrencyGameItemComponentProto _prototype;

	public CurrencyGameItemComponent(CurrencyGameItemComponentProto proto, IGameItem parent, ICurrency currency,
		IEnumerable<Tuple<ICoin, int>> coins, bool temporary = false)
		: this(proto, parent, temporary)
	{
		Currency = currency;
		_coins = coins.ToDictionary(key => key.Item1, value => value.Item2);
	}

	public CurrencyGameItemComponent(CurrencyGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public CurrencyGameItemComponent(MudSharp.Models.GameItemComponent component, CurrencyGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public CurrencyGameItemComponent(CurrencyGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(
		rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		foreach (var coin in rhs._coins)
		{
			_coins.Add(coin.Key, coin.Value);
		}

		Currency = rhs.Currency;
	}

	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new CurrencyGameItemComponent(this, newParent, temporary);
	}

	public override bool PreventsMerging(IGameItemComponent component)
	{
		return component is ICurrencyPile cc && cc.Currency != Currency;
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		switch (type)
		{
			case DescriptionType.Short:
			case DescriptionType.Full:
				return true;
		}

		return false;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Short:
				switch (_coins.Count)
				{
					case 0:
						return "An empty currency pile";
					case 1:
						var generalName = _coins.Select(x => x.Key.GeneralForm).Distinct().Single();
						return _prototype.Decorator.Describe(generalName, generalName, _coins.Sum(x => x.Value));
				}

				if (_coins.Select(x => x.Key.GeneralForm).Distinct().Count() == 1)
				{
					var generalName = _coins.Select(x => x.Key.GeneralForm).Distinct().Single();
					return _prototype.Decorator.Describe(generalName, generalName, _coins.Sum(x => x.Value));
				}

				return _prototype.Decorator.Describe("",
					_coins.Select(x => x.Key.GeneralForm).Distinct().Select(x => x.Pluralise()).ListToString(),
					_coins.Sum(x => x.Value));
			case DescriptionType.Full:
				return
					$"This is a collection of currency. It contains:\n{_coins.Select(x => $"\t{x.Key.ShortDescription.Colour(Telnet.Green)} (x{x.Value})").ListToString(separator: "\n", conjunction: "\n", oxfordComma: false)}";
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}

	public override double ComponentWeight
	{
		get { return _coins.Sum(x => x.Key.Weight * x.Value); }
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (CurrencyGameItemComponentProto)newProto;
	}

	private void LoadFromXml(XElement root)
	{
		Currency = Gameworld.Currencies.Get(long.Parse(root.Attribute("Currency").Value));
		foreach (var item in root.Element("Coins").Elements("Coin"))
		{
			var coin = Currency.Coins.FirstOrDefault(x => x.Id == long.Parse(item.Attribute("Id").Value));
			if (coin == null)
			{
				continue;
			}

			_coins.Add(coin, int.Parse(item.Attribute("Count").Value));
		}
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition", new XAttribute("Currency", Currency?.Id ?? 0),
				new XElement("Coins", new object[]
				{
					from coin in _coins
					select
						new XElement("Coin", new XAttribute("Id", coin.Key.Id), new XAttribute("Count", coin.Value))
				})).ToString();
	}

	#region ICurrencyPile Members

	public ICurrency Currency { get; set; }

	private readonly Dictionary<ICoin, int> _coins = new();

	public IEnumerable<Tuple<ICoin, int>> Coins
	{
		get { return _coins.Select(x => Tuple.Create(x.Key, x.Value)); }
	}

	public decimal TotalValue => _coins.Sum(x => x.Value * x.Key.Value);

	public void AddCoins(IEnumerable<Tuple<ICoin, int>> coins)
	{
		foreach (var coin in coins)
		{
			if (_coins.ContainsKey(coin.Item1))
			{
				_coins[coin.Item1] += coin.Item2;
			}
			else
			{
				_coins[coin.Item1] = coin.Item2;
			}
		}

		HandleDescriptionUpdate();
		Changed = true;
	}

	public void AddCoins(IEnumerable<KeyValuePair<ICoin, int>> coins)
	{
		foreach (var coin in coins)
		{
			if (_coins.ContainsKey(coin.Key))
			{
				_coins[coin.Key] += coin.Value;
			}
			else
			{
				_coins[coin.Key] = coin.Value;
			}
		}

		HandleDescriptionUpdate();
		Changed = true;
	}

	public void AddCoins(IEnumerable<(ICoin, int)> coins)
	{
		foreach (var coin in coins)
		{
			if (_coins.ContainsKey(coin.Item1))
			{
				_coins[coin.Item1] += coin.Item2;
			}
			else
			{
				_coins[coin.Item1] = coin.Item2;
			}
		}

		HandleDescriptionUpdate();
		Changed = true;
	}

	public bool RemoveCoins(IEnumerable<Tuple<ICoin, int>> coins)
	{
		foreach (var coin in coins)
		{
			if (_coins.ContainsKey(coin.Item1))
			{
				_coins[coin.Item1] -= coin.Item2;
			}
		}

		foreach (var item in _coins.Where(x => x.Value < 1).ToList())
		{
			_coins.Remove(item.Key);
		}

		if (!_coins.Any())
		{
			return false;
		}

		HandleDescriptionUpdate();
		Changed = true;
		return true;
	}

	public bool RemoveCoins(IEnumerable<KeyValuePair<ICoin, int>> coins)
	{
		foreach (var coin in coins)
		{
			if (_coins.ContainsKey(coin.Key))
			{
				_coins[coin.Key] -= coin.Value;
			}
		}

		foreach (var item in _coins.Where(x => x.Value < 1).ToList())
		{
			_coins.Remove(item.Key);
		}

		if (!_coins.Any())
		{
			return false;
		}

		HandleDescriptionUpdate();
		Changed = true;
		return true;
	}

	public bool RemoveCoins(IEnumerable<(ICoin, int)> coins)
	{
		foreach (var coin in coins)
		{
			if (_coins.ContainsKey(coin.Item1))
			{
				_coins[coin.Item1] -= coin.Item2;
			}
		}

		foreach (var item in _coins.Where(x => x.Value < 1).ToList())
		{
			_coins.Remove(item.Key);
		}

		if (!_coins.Any())
		{
			return false;
		}

		HandleDescriptionUpdate();
		Changed = true;
		return true;
	}

	#endregion
}