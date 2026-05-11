#nullable enable
using MudSharp.Character;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.GameItems;

public sealed class CommodityCharacteristicRequirement
{
	private readonly Dictionary<ICharacteristicDefinition, ICharacteristicValue?> _requirements = new();

	public bool RequireNoCharacteristics { get; private set; }
	public IReadOnlyDictionary<ICharacteristicDefinition, ICharacteristicValue?> Requirements => _requirements;
	public bool IsWildcard => !RequireNoCharacteristics && _requirements.Count == 0;

	public static bool CommodityCharacteristicsEqual(ICommodity lhs, ICommodity rhs)
	{
		return lhs.CommodityCharacteristics.Count == rhs.CommodityCharacteristics.Count &&
		       lhs.CommodityCharacteristics.All(x =>
			       rhs.CommodityCharacteristics.TryGetValue(x.Key, out ICharacteristicValue? value) &&
			       value == x.Value);
	}

	public static bool CommodityIdentityEqual(ICommodity lhs, ICommodity rhs)
	{
		return lhs.Material == rhs.Material &&
		       lhs.Tag == rhs.Tag &&
		       lhs.UseIndirectQuantityDescription == rhs.UseIndirectQuantityDescription &&
		       CommodityCharacteristicsEqual(lhs, rhs);
	}

	public void SetWildcard()
	{
		RequireNoCharacteristics = false;
		_requirements.Clear();
	}

	public void SetNone()
	{
		RequireNoCharacteristics = true;
		_requirements.Clear();
	}

	public void SetRequirement(ICharacteristicDefinition definition, ICharacteristicValue? value)
	{
		RequireNoCharacteristics = false;
		_requirements[definition] = value;
	}

	public bool RemoveRequirement(ICharacteristicDefinition definition)
	{
		return _requirements.Remove(definition);
	}

	public bool Matches(ICommodity commodity)
	{
		if (RequireNoCharacteristics)
		{
			return !commodity.CommodityCharacteristics.Any();
		}

		foreach ((ICharacteristicDefinition definition, ICharacteristicValue? requiredValue) in _requirements)
		{
			if (!commodity.CommodityCharacteristics.TryGetValue(definition, out ICharacteristicValue? commodityValue))
			{
				return false;
			}

			if (requiredValue is not null && commodityValue != requiredValue)
			{
				return false;
			}
		}

		return true;
	}

	public bool DeterminesVariable(ICharacteristicDefinition definition)
	{
		return _requirements.ContainsKey(definition);
	}

	public ICharacteristicValue? GetValueForVariable(ICharacteristicDefinition definition, ICommodity commodity)
	{
		return DeterminesVariable(definition) ? commodity.GetCommodityCharacteristic(definition) : null;
	}

	public XElement SaveToXml(string elementName = "Characteristics")
	{
		return new XElement(elementName,
			new XAttribute("mode", RequireNoCharacteristics ? "none" : _requirements.Count == 0 ? "any" : "specific"),
			from requirement in _requirements.OrderBy(x => x.Key.Id)
			select new XElement("Characteristic",
				new XAttribute("definition", requirement.Key.Id),
				new XAttribute("value", requirement.Value?.Id ?? 0L)
			)
		);
	}

	public void LoadFromXml(XElement? element, IFuturemud gameworld)
	{
		SetWildcard();
		if (element is null)
		{
			return;
		}

		var mode = element.Attribute("mode")?.Value ?? "";
		if (mode.EqualTo("none"))
		{
			SetNone();
			return;
		}

		foreach (XElement child in element.Elements("Characteristic"))
		{
			var definitionId = GetLongAttribute(child, "definition");
			if (definitionId == 0)
			{
				definitionId = GetLongAttribute(child, "Definition");
			}

			var valueId = GetLongAttribute(child, "value");
			if (valueId == 0)
			{
				valueId = GetLongAttribute(child, "Value");
			}

			ICharacteristicDefinition? definition = gameworld.Characteristics.Get(definitionId);
			if (definition is null)
			{
				continue;
			}

			ICharacteristicValue? value = valueId == 0 ? null : gameworld.CharacteristicValues.Get(valueId);
			if (value is not null && !definition.IsValue(value))
			{
				continue;
			}

			SetRequirement(definition, value);
		}
	}

	public string Describe()
	{
		if (RequireNoCharacteristics)
		{
			return "with no characteristics".Colour(Telnet.Red);
		}

		if (_requirements.Count == 0)
		{
			return "with any characteristics".ColourValue();
		}

		return $"with {_requirements.OrderBy(x => x.Key.Name).ThenBy(x => x.Key.Id).Select(x =>
			x.Value is null
				? $"{x.Key.Name.ColourName()} = {"any".ColourValue()}"
				: $"{x.Key.Name.ColourName()} = {x.Value.GetValue.ColourValue()}").ListToString()}";
	}

	public bool BuildingCommand(ICharacter actor, StringStack command, string ownerDescription, Action markChanged)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic requirement do you want to set? Use ANY, NONE, <definition> ANY, <definition> <value>, or <definition> REMOVE.");
			return false;
		}

		var first = command.PopSpeech();
		if (first.EqualTo("any"))
		{
			SetWildcard();
			markChanged();
			actor.OutputHandler.Send($"The {ownerDescription} will now accept commodity piles with any commodity characteristics.");
			return true;
		}

		if (first.EqualTo("none"))
		{
			SetNone();
			markChanged();
			actor.OutputHandler.Send($"The {ownerDescription} will now only accept commodity piles with no commodity characteristics.");
			return true;
		}

		ICharacteristicDefinition? definition = actor.Gameworld.Characteristics.GetByIdOrName(first);
		if (definition is null)
		{
			actor.OutputHandler.Send($"There is no characteristic definition identified by {first.ColourCommand()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Do you want to require ANY value, REMOVE this requirement, or specify a characteristic value?");
			return false;
		}

		var second = command.PopSpeech();
		if (second.EqualToAny("remove", "delete", "clear"))
		{
			RemoveRequirement(definition);
			RequireNoCharacteristics = false;
			markChanged();
			actor.OutputHandler.Send($"The {ownerDescription} will no longer require the {definition.Name.ColourName()} commodity characteristic.");
			return true;
		}

		if (second.EqualTo("any"))
		{
			SetRequirement(definition, null);
			markChanged();
			actor.OutputHandler.Send($"The {ownerDescription} will now require commodity piles to have any {definition.Name.ColourName()} characteristic.");
			return true;
		}

		var valueText = command.IsFinished ? second : $"{second} {command.SafeRemainingArgument}";
		ICharacteristicValue? value = GetCharacteristicValue(actor.Gameworld, valueText);
		if (value is null || !definition.IsValue(value))
		{
			actor.OutputHandler.Send($"There is no {definition.Name.ColourName()} characteristic value identified by {valueText.ColourCommand()}.");
			return false;
		}

		SetRequirement(definition, value);
		markChanged();
		actor.OutputHandler.Send($"The {ownerDescription} will now require commodity piles to have {definition.Name.ColourName()} set to {value.GetValue.ColourValue()}.");
		return true;
	}

	public static ICharacteristicValue? GetCharacteristicValue(IFuturemud gameworld, string text)
	{
		return long.TryParse(text, out var value)
			? gameworld.CharacteristicValues.Get(value)
			: gameworld.CharacteristicValues.GetByName(text);
	}

	private static long GetLongAttribute(XElement element, string name)
	{
		return long.TryParse(element.Attribute(name)?.Value, out var value) ? value : 0L;
	}
}
