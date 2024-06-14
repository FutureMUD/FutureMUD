using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;
using MudSharp.NPC;

namespace MudSharp.Work.Crafts.Products;
internal class InputVariableProduct : BaseProduct
{
	public static void RegisterCraftProduct()
	{
		CraftProductFactory.RegisterCraftProductType("InputVariable",
			(product, craft, game) => new InputVariableProduct(product, craft, game));
		CraftProductFactory.RegisterCraftProductTypeForBuilders("inputvariable",
			(craft, game, fail) => new InputVariableProduct(craft, game, fail));
	}

	protected InputVariableProduct(CraftProduct product, ICraft craft, IFuturemud gameworld) : base(product, craft, gameworld)
	{
		var root = XElement.Parse(product.Definition);
		Quantity = int.Parse(root.Element("Quantity")?.Value ?? "1");
		ProductProducedId = long.Parse(root.Element("ProductProducedId").Value);
		Skin = gameworld.ItemSkins.Get(long.Parse(root.Element("Skin")?.Value ?? "0"));
		foreach (var element in root.Elements("Variable"))
		{
			var definition = Gameworld.Characteristics.Get(long.Parse(element.Attribute("definition").Value));
			if (definition is null)
			{
				continue;
			}

			var list = new List<(long, ICharacteristicValue)>();
			foreach (var sub in element.Elements("Specific"))
			{
				var value = Gameworld.CharacteristicValues.Get(long.Parse(sub.Attribute("value").Value));
				if (value is null)
				{
					continue;
				}

				list.Add((long.Parse(sub.Attribute("proto").Value), value));
			}

			VariableSpecifics.AddRange(definition, list);
		}
	}

	protected InputVariableProduct(ICraft craft, IFuturemud gameworld, bool failproduct) : base(craft, gameworld, failproduct)
	{
		Quantity = 1;
	}

	public override ICraftProductData ProduceProduct(IActiveCraftGameItemComponent component, ItemQuality referenceQuality)
	{
		var proto = Gameworld.ItemProtos.Get(ProductProducedId);
		if (proto == null)
		{
			throw new ApplicationException("Couldn't find a valid proto for craft product to load.");
		}

		var variables = new List<(ICharacteristicDefinition Definition, ICharacteristicValue Value)>();
		foreach (var (definition, input) in VariableIndexes)
		{
			var ivi = Craft.Inputs.ElementAt(input-1);
			if (!component.ConsumedInputs.ContainsKey(ivi))
			{
				continue;
			}

			var data = component.ConsumedInputs[ivi].Data as ICraftInputDataWithItems;
            if (data is null)
            {
				continue;
            }

			var targets = VariableSpecifics[definition];
			foreach (var item in data.ConsumedItems)
			{
				var result = targets.FirstOrDefault(x => x.Proto == item.Prototype.Id);
				if (result.Proto == 0)
				{
					continue;
				}

				variables.Add((definition, result.Value));
				break;
			}
		}

		var material = DetermineOverrideMaterial(component);

		if (Quantity > 1 && proto.IsItemType<StackableGameItemComponentProto>())
		{
			var newItem = proto.CreateNew();
			newItem.Skin = Skin;
			newItem.RoomLayer = component.Parent.RoomLayer;
			Gameworld.Add(newItem);
			newItem.GetItemType<IStackable>().Quantity = Quantity;
			newItem.Quality = referenceQuality;
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
			item.Quality = referenceQuality;
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

			foreach (var (definition, input) in VariableIndexes)
			{
				sb.Append(" ").Append(definition.Name).Append(" <- ");
				if (Craft.Inputs.ElementAtOrDefault(input-1) is ICraftInput ici)
				{
					sb.Append(ici.Name).Append($" ($i{input})");
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

		foreach (var (definition, input) in VariableIndexes)
		{
			sb.Append(" ").Append(definition.Name).Append(" <- ");
			if (Craft.Inputs.ElementAtOrDefault(input-1) is ICraftInput ici)
			{
				sb.Append(ici.Name).Append($" ($i{input})");
			}
			else
			{
				sb.Append(" an invalid input".Colour(Telnet.Red));
			}
		}

		var sb2 = new StringBuilder();
		foreach (var (definition, list) in VariableSpecifics)
		{
			sb2.AppendLine($"--Variable {definition.Name}--\n");
			foreach (var (proto, value) in list)
			{
				var item = Gameworld.ItemProtos.Get(proto);
				if (item is null)
				{
					sb2.AppendLine($"An invalid item #{proto.ToString("N0", voyeur)}");
				}
				sb2.AppendLine($"{item.ShortDescription} (#{item.Id.ToString("N0", voyeur)}) = {value.Name}");
			}
		}

		return sb.ToString().MXPSend("look", sb2.ToString());
	}

	public override string ProductType => "InputVariable";

	public DictionaryWithDefault<ICharacteristicDefinition, int> VariableIndexes { get; } = new();
	public CollectionDictionary<ICharacteristicDefinition, (long Proto, ICharacteristicValue Value)> VariableSpecifics { get; } = new();

	public long ProductProducedId { get; set; }
	public int Quantity { get; set; }
	public IGameItemSkin Skin { get; set; }

	/// <inheritdoc />
	public override bool IsItem(IGameItem item)
	{
		return ProductProducedId == item.Prototype.Id && Skin?.Id == item.Skin?.Id;
	}

	/// <inheritdoc />
	public override bool RefersToItemProto(long id)
	{
		return ProductProducedId == id;
	}

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("ProductProducedId", ProductProducedId),
			new XElement("Quantity", Quantity),
			new XElement("Skin", Skin?.Id ?? 0),
			from item in VariableIndexes
			select new XElement("Variable",
				new XAttribute("definition", item.Key.Id),
				new XAttribute("index", item.Value),
				from sub in VariableSpecifics[item.Key]
				select new XElement("Specific", 
					new XAttribute("value", sub.Value.Id),
					new XAttribute("proto", sub.Proto)
				)
			)
		).ToString();
	}

	public override bool IsValid()
	{
		if (ProductProducedId < 1)
		{
			return false;
		}

		foreach (var (definition, index) in VariableIndexes)
		{
			var ivi = Craft.Inputs.ElementAtOrDefault(index-1);
			if (ivi is null)
			{
				return false;
			}
		}

		return true;
	}

	public override string WhyNotValid()
	{
		if (ProductProducedId < 1)
		{
			return "You must first set an item prototype for this product to load.";
		}

		foreach (var (definition, index) in VariableIndexes)
		{
			var ivi = Craft.Inputs.ElementAtOrDefault(index - 1);
			if (ivi is null)
			{
				return $"Craft Input $i{index} determining variable {definition.Name.Colour(Telnet.Green)} was not found.";
			}
		}

		return "An unknown error";
	}

	#region Building Commands
	protected override string BuildingHelpText => $@"{base.BuildingHelpText}
	#3item <id|name>#0 - sets the item prototype to be loaded
	#3skin <id|name>#0 - sets a skin to go with the item prototype
	#3skin clear#0 - clears the skin
	#3quantity <amount>#0 - sets the quantity of that item to be loaded
	#3variable <which> index <##>#0 - sets the input index that determines an output variable
	#3variable <which> remove#0 - removes a variable from the mixture
	#3variable <which> item <which> <value>#0 - sets it so that if the indexed input is this item, set the variable value to this value
	#3variable <which> item delete#0 - removes an item from the variable definition";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "item":
				return BuildingCommandItem(actor, command);
			case "quantity":
			case "amount":
			case "number":
			case "num":
				return BuildingCommandQuantity(actor, command);
			case "skin":
				return BuildingCommandSkin(actor, command);
			case "variable":
				return BuildingCommandVariable(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandVariable(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which variable do you want to change the interaction with?");
			return false;
		}

		var definition = Gameworld.Characteristics.GetByIdOrName(command.PopSpeech());
		if (definition is null)
		{
			actor.OutputHandler.Send("There is no such variable definition.");
			return false;
		}

		switch (command.PopForSwitch())
		{
			case "remove":
				return BuildingCommandVariableRemove(actor, command, definition);
			case "index":
			case "input":
				return BuildingCommandVariableIndex(actor, command, definition);
			case "item":
				if (!VariableIndexes.ContainsKey(definition))
				{
					actor.OutputHandler.Send($"You must first set an index for that variable.");
					return false;
				}
				return BuildingCommandVariableItem(actor, command, definition);
			default:
				actor.OutputHandler.Send("You must either use #3remove#0, #3index#0, or #3item#0.".SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandVariableItem(ICharacter actor, StringStack command, ICharacteristicDefinition definition)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which item prototype do you want to edit for the {definition.Name.ColourValue()} characteristic?");
			return false;
		}

		var item = Gameworld.ItemProtos.GetByIdOrName(command.PopSpeech());
		if (item is null)
		{
			actor.OutputHandler.Send("There is no such item prototype.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must either specify a value for the {definition.Name.ColourValue()} characteristic or use {"delete".ColourCommand()} to remove one.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("delete"))
		{
			VariableSpecifics.RemoveAll(definition, x => x.Proto == item.Id);
			ProductChanged = true;
			actor.OutputHandler.Send($"The {item.EditHeader()} item no longer provides a value for the {definition.Name.ColourValue()} characteristic.");
			return true;
		}

		var value = Gameworld.CharacteristicValues.Where(x => definition.IsValue(x)).GetByIdOrName(command.SafeRemainingArgument);
		if (value is null)
		{
			actor.OutputHandler.Send($"There is no such valid value for the {definition.Name.ColourValue()} characteristic.");
			return false;
		}

		VariableSpecifics.RemoveAll(definition, x => x.Proto == item.Id);
		VariableSpecifics.Add(definition, (item.Id, value));
		ProductChanged = true;
		actor.OutputHandler.Send($"The {item.EditHeader()} item will now provide the {value.Name.ColourValue()} value for the {definition.Name.ColourValue()} characteristic.");
		return true;
	}

	private bool BuildingCommandVariableIndex(ICharacter actor, StringStack command, ICharacteristicDefinition definition)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which input index should be used to determine the value of the {definition.Name.ColourValue()} characteristic?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		if (value < 1 || Craft.Inputs.Count() < value)
		{
			actor.OutputHandler.Send($"This craft has no input with an index of {value.ToString("N0", actor).ColourValue()}.");
			return false;
		}

		VariableIndexes[definition] = value;
		ProductChanged = true;
		actor.OutputHandler.Send($"The value of the {definition.Name.ColourValue()} characteristic will now be determined by which item is used for input #2${value.ToString("N0", actor)}#0.".SubstituteANSIColour());
		return true;
	}

	private bool BuildingCommandVariableRemove(ICharacter actor, StringStack command, ICharacteristicDefinition definition)
	{
		if (!VariableIndexes.ContainsKey(definition))
		{
			actor.OutputHandler.Send($"This product does not have a variable set up for the {definition.Name.ColourValue()} characteristic.");
			return false;
		}

		VariableIndexes.Remove(definition);
		VariableSpecifics.Remove(definition);
		ProductChanged = true;
		actor.OutputHandler.Send($"This product no longer has a definition for the {definition.Name.ColourValue()} characteristic.");
		return true;
	}

	private bool BuildingCommandSkin(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must specify a skin to go with this product or use {"clear".ColourCommand()} to clear an existing one.");
			return false;
		}

		if (ProductProducedId == 0)
		{
			actor.OutputHandler.Send("You must first set an item prototype before you can set any skins.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("clear", "delete", "remove", "none"))
		{
			Skin = null;
			ProductChanged = true;
			actor.OutputHandler.Send("This product will no longer apply any skin to the loaded item.");
			return true;
		}

		IGameItemSkin skin;
		if (long.TryParse(command.SafeRemainingArgument, out var value))
		{
			skin = Gameworld.ItemSkins.Get(value);
		}
		else
		{
			skin = Gameworld.ItemSkins.Where(x => x.ItemProto.Id == ProductProducedId)
							.GetByNameOrAbbreviation(command.SafeRemainingArgument);
		}

		if (skin is null)
		{
			actor.OutputHandler.Send("There is no such item skin.");
			return false;
		}

		if (skin.ItemProto.Id != ProductProducedId)
		{
			actor.OutputHandler.Send(
				$"{skin.EditHeader().ColourName()} is not designed to work with {Gameworld.ItemProtos.Get(ProductProducedId).EditHeader().ColourObject()}.");
			return false;
		}

		if (skin.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send($"{skin.EditHeader().ColourName()} is not approved for use.");
			return false;
		}

		Skin = skin;
		ProductChanged = true;
		actor.OutputHandler.Send($"This product will now apply {skin.EditHeader().ColourName()} to the loaded item.");
		return true;
	}

	private bool BuildingCommandQuantity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must enter a valid amount of this item to be loaded.");
			return false;
		}

		Quantity = value;
		ProductChanged = true;
		actor.OutputHandler.Send($"This product will now produce {Quantity} of the target item.");
		return true;
	}

	private bool BuildingCommandItem(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What item prototype should this item input load?");
			return false;
		}

		var proto = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.ItemProtos.Get(value)
			: Gameworld.ItemProtos.GetByName(command.SafeRemainingArgument);
		if (proto == null)
		{
			actor.OutputHandler.Send("There is no such item prototype.");
			return false;
		}

		ProductProducedId = proto.Id;
		Skin = null;
		ProductChanged = true;
		actor.OutputHandler.Send(
			$"This product will now target item prototype #{ProductProducedId.ToString("N0", actor)} ({proto.ShortDescription.ColourObject()}).");
		return true;
	}
	#endregion
}
