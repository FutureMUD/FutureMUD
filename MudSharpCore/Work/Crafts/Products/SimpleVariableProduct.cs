using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;

namespace MudSharp.Work.Crafts.Products;

public class SimpleVariableProduct : SimpleProduct
{
	protected SimpleVariableProduct(CraftProduct product, ICraft craft, IFuturemud gameworld) : base(product, craft,
		gameworld)
	{
		var root = XElement.Parse(product.Definition);
		foreach (var item in root.Elements("Variable"))
		{
			Characteristics.Add((gameworld.Characteristics.Get(long.Parse(item.Value)),
				int.Parse(item.Attribute("inputindex").Value)));
		}
	}

	protected SimpleVariableProduct(ICraft craft, IFuturemud gameworld, bool failproduct) : base(craft, gameworld,
		failproduct)
	{
	}

	public List<(ICharacteristicDefinition Definition, int InputIndex)> Characteristics { get; } = new();

	public new static void RegisterCraftProduct()
	{
		CraftProductFactory.RegisterCraftProductType("SimpleVariableProduct",
			(product, craft, game) => new SimpleVariableProduct(product, craft, game));
		CraftProductFactory.RegisterCraftProductTypeForBuilders("variable",
			(craft, game, fail) => new SimpleVariableProduct(craft, game, fail));
	}

	public override string ProductType => "SimpleVariableProduct";

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("ProductProducedId", ProductProducedId),
			new XElement("Quantity", Quantity),
			new XElement("Skin", Skin?.Id ?? 0),
			from item in Characteristics
			select new XElement("Variable", new XAttribute("inputindex", item.InputIndex), item.Definition.Id)
		).ToString();
	}

	protected override string BuildingHelpText =>
		$"{base.BuildingHelpText}\n\t#3variable <definition> <input#>#0 - toggles a variable to be set on this item";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "variable":
			case "var":
			case "characteristic":
			case "char":
			case "definition":
				return BuildingCommandVariable(actor, command);
		}

		return base.BuildingCommand(actor, new StringStack($"{command.Last} {command.RemainingArgument}"));
	}

	private bool BuildingCommandVariable(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic definition did you want to change?");
			return false;
		}

		var definition = Gameworld.Characteristics.GetByIdOrName(command.PopSpeech());
		if (definition == null)
		{
			actor.OutputHandler.Send("There is no such characteristic definition.");
			return false;
		}

		if (command.IsFinished)
		{
			if (Characteristics.Any(x => x.Definition == definition))
			{
				Characteristics.RemoveAll(x => x.Definition == definition);
				ProductChanged = true;
				actor.OutputHandler.Send(
					$"This product will no longer be supplied the variable {definition.Name.Colour(Telnet.Yellow)}.");
				return true;
			}
		}

		if (!int.TryParse(command.PopSpeech(), out var ivalue))
		{
			actor.OutputHandler.Send("Which input number do you want to pair this product with?");
			return false;
		}

		var input = Craft.Inputs.ElementAtOrDefault(ivalue - 1);
		if (input == null)
		{
			actor.OutputHandler.Send("There is no such input for this craft.");
			return false;
		}

		if (Characteristics.Any(x => x.Definition == definition))
		{
			Characteristics.RemoveAll(x => x.Definition == definition);
		}

		Characteristics.Add((definition, ivalue - 1));
		actor.OutputHandler.Send(
			$"This input will now be supplied the variable {definition.Name.Colour(Telnet.Yellow)} from the input {input.Name}.");
		ProductChanged = true;
		return true;
	}

	public override bool IsValid()
	{
		return base.IsValid() && Characteristics.All(x =>
			Craft.Inputs.ElementAtOrDefault(x.InputIndex) is IVariableInput ivi &&
			ivi.DeterminesVariable(x.Definition));
	}

	public override string WhyNotValid()
	{
		var sb = new StringBuilder();
		foreach (var (definition, input) in Characteristics.Where(x =>
			         Craft.Inputs.ElementAtOrDefault(x.InputIndex) is IVariableInput ivi &&
			         ivi.DeterminesVariable(x.Definition)))
		{
			sb.AppendLine(
				$"Craft Input $i{input + 1} determining variable {definition.Name.Colour(Telnet.Green)} was not found or was not providing said variable.");
		}

		if (sb.Length > 0)
		{
			return sb.ToString();
		}

		return base.WhyNotValid();
	}

	public override ICraftProductData ProduceProduct(IActiveCraftGameItemComponent component,
		ItemQuality referenceQuality)
	{
		var proto = Gameworld.ItemProtos.Get(ProductProducedId);
		if (proto is null)
		{
			throw new ApplicationException("Couldn't find a valid proto for craft product to load.");
		}

		var variables = new List<(ICharacteristicDefinition Definition, ICharacteristicValue Value)>();
		foreach (var (definition, input) in Characteristics)
		{
			var ivi = Craft.Inputs.ElementAt(input);
			variables.Add((definition,
				((IVariableInput)ivi).GetValueForVariable(definition, component.ConsumedInputs[ivi].Data)));
		}

		var material = DetermineOverrideMaterial(component);

		if (Quantity > 1 && proto.IsItemType<StackableGameItemComponentProto>())
		{
			var newItem = proto.CreateNew();
			newItem.Skin = Skin;
			newItem.RoomLayer = component.Parent.RoomLayer;
			Gameworld.Add(newItem);
			newItem.GetItemType<IStackable>().Quantity = Quantity;

			if (!Gameworld.GetStaticBool("DisableCraftQualityCalculation"))
			{
				newItem.Quality = referenceQuality;
			}

			if (material != null)
			{
				newItem.Material = material;
			}

			var varItem = newItem.GetItemType<IVariable>();
			if (varItem != null)
			{
				foreach (var (definition, value) in variables)
				{
					varItem.SetCharacteristic(definition, value);
				}
			}

			return new SimpleProductData(new[] { newItem });
		}

		var items = new List<IGameItem>();
		for (var i = 0; i < Quantity; i++)
		{
			var item = proto.CreateNew();
			item.Skin = Skin;
			item.RoomLayer = component.Parent.RoomLayer;
			Gameworld.Add(item);

			if (!Gameworld.GetStaticBool("DisableCraftQualityCalculation"))
			{
				item.Quality = referenceQuality;
			}

			if (material != null)
			{
				item.Material = material;
			}

			var varItem = item.GetItemType<IVariable>();
			if (varItem != null)
			{
				foreach (var (definition, value) in variables)
				{
					varItem.SetCharacteristic(definition, value);
				}
			}

			items.Add(item);
		}

		return new SimpleProductData(items);
	}

	public override string Name
	{
		get
		{
			var sb = new StringBuilder();
			sb.Append(Quantity)
			  .Append("x ");

			if (Skin is not null)
			{
				sb.Append((Skin.ShortDescription ?? Gameworld.ItemProtos.Get(ProductProducedId)?.ShortDescription)
					.ConcatIfNotEmpty($" [reskinned: #{Skin.Id:N0}]") ?? "an unspecified item".Colour(Telnet.Red));
			}
			else
			{
				sb.Append(Gameworld.ItemProtos.Get(ProductProducedId)?.ShortDescription ??
				          "an unspecified item".Colour(Telnet.Red));
			}

			foreach (var (definition, input) in Characteristics)
			{
				sb.Append(" ").Append(definition.Name).Append(" <- ");
				if (Craft.Inputs.ElementAtOrDefault(input) is IVariableInput ivi && ivi.DeterminesVariable(definition))
				{
					sb.Append(ivi.Name).Append($" ($i{input + 1})");
				}
				else
				{
					sb.Append(" an invalid input".Colour(Telnet.Red));
				}
			}

			return sb.ToString();
		}
	}

	public override string HowSeen(IPerceiver voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(Quantity)
		  .Append("x ");

		if (Skin is not null && voyeur is ICharacter ch && ch.IsAdministrator())
		{
			sb.Append((Skin.ShortDescription ?? Gameworld.ItemProtos.Get(ProductProducedId)?.ShortDescription)
			          .ConcatIfNotEmpty($" [reskinned: #{Skin.Id.ToString("N0", voyeur)}]") ??
			          "an unspecified item".Colour(Telnet.Red));
		}
		else
		{
			sb.Append(Gameworld.ItemProtos.Get(ProductProducedId)?.ShortDescription ??
			          "an unspecified item".Colour(Telnet.Red));
		}

		foreach (var (definition, input) in Characteristics)
		{
			sb.Append(" ").Append(definition.Name).Append(" <- ");
			if (Craft.Inputs.ElementAtOrDefault(input) is IVariableInput ivi && ivi.DeterminesVariable(definition))
			{
				sb.Append(ivi.Name).Append($" ($i{input+1})");
			}
			else
			{
				sb.Append(" an invalid input".Colour(Telnet.Red));
			}
		}

		return sb.ToString();
	}
}