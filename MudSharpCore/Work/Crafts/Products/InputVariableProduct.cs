using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Events;
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

			VariableIndexes[definition] = int.Parse(element.Attribute("index").Value);
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
			var newItem = proto.CreateNew(null, Skin, Quantity, variables).First();
			newItem.RoomLayer = component.Parent.RoomLayer;
			Gameworld.Add(newItem);

			if (!Gameworld.GetStaticBool("DisableCraftQualityCalculation"))
			{
				newItem.Quality = referenceQuality;
			}

			if (material != null)
			{
				newItem.Material = material;
			}

			newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
			return new SimpleProductData(new[] { newItem });
		}

		var items = proto.CreateNew(null, Skin, Quantity, variables).ToList();
		foreach (var item in items)
		{
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

			item.HandleEvent(EventType.ItemFinishedLoading, item);
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
		sb.Append(Quantity.ToString("N0", voyeur))
		  .Append("x ");
		var proto = Gameworld.ItemProtos.Get(ProductProducedId);
		if (proto is null)
		{
			sb.Append("an unspecified item".Colour(Telnet.Red));
			return sb.ToString();
		}

		if (Skin is not null && voyeur is ICharacter ch && ch.IsAdministrator())
		{
			sb.Append($"{(Skin.ShortDescription ?? Gameworld.ItemProtos.Get(ProductProducedId).ShortDescription)} (#{proto.Id.ToString("N0", voyeur)}) [reskinned: #{Skin.Id.ToString("N0", voyeur)}]");
		}
		else
		{
			sb.Append(Gameworld.ItemProtos.Get(ProductProducedId).ShortDescription);
			sb.Append($" (#{proto.Id.ToString("N0", voyeur)})");
		}

		return sb.ToString();
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
	#3variable#0 - show detailed information about the variables for this item
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
			return BuildingCommmandVariableDefault(actor);
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

	private bool BuildingCommmandVariableDefault(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Detailed Variable Information for the Product:");
		
		foreach (var (variable, index) in VariableIndexes)
		{
			var input = Craft.Inputs.ElementAtOrDefault(index - 1);
			sb.AppendLine();
			sb.AppendLine($"Variable {variable.Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
			sb.AppendLine();
			if (input is null)
			{
				sb.AppendLine($"Invalid target input {$"$i{index.ToString("N0", actor)}".ColourValue()}.");
				continue;
			}
			sb.AppendLine($"Determined by input {$"$i{index.ToString("N0", actor)}".ColourValue()} ({input.HowSeen(actor)})");
			sb.AppendLine();
			sb.AppendLine("Values:");
			sb.AppendLine();
			foreach (var (proto,value) in VariableSpecifics[variable])
			{
				var iProto = Gameworld.ItemProtos.Get(proto);
				if (iProto is null)
				{
					sb.AppendLine($"\tInvalid item prototye #{proto.ToString("N0", actor)} -> {value.Name.ColourValue()}");
					continue;
				}
				sb.AppendLine($"\t#{proto.ToString("N0", actor)} ({iProto.ShortDescription.Colour(iProto.CustomColour ?? Telnet.Green)}) -> {value.Name.ColourValue()}");
			}
		}
		actor.OutputHandler.Send(sb.ToString());
		return true;
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
