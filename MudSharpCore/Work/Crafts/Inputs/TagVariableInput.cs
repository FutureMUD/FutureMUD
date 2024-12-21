using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine.Lists;

namespace MudSharp.Work.Crafts.Inputs;

public class TagVariableInput : TagInput, IVariableInput
{
	protected TagVariableInput(Models.CraftInput input, ICraft craft, IFuturemud gameworld) : base(input, craft,
		gameworld)
	{
		var root = XElement.Parse(input.Definition);
		foreach (var item in root.Elements("Variable"))
		{
			Characteristics.Add(gameworld.Characteristics.Get(long.Parse(item.Value)));
		}
	}

	protected TagVariableInput(ICraft craft, IFuturemud gameworld) : base(craft, gameworld)
	{
	}

	protected override string BuildingHelpString =>
		$"{base.BuildingHelpString}\n\t#3variable <definition>#0 - this input will require and supply this variable definition for the craft";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "variable":
			case "var":
			case "characteristic":
			case "definition":
				return BuildingCommandVariable(actor, command);
			default:
				return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"));
		}
	}

	private bool BuildingCommandVariable(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic definition did you want to add/remove?");
			return false;
		}

		var definition = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.Characteristics.Get(value)
			: Gameworld.Characteristics.GetByName(command.SafeRemainingArgument);

		if (definition == null)
		{
			actor.OutputHandler.Send("There is no such characteristic definition.");
			return false;
		}

		if (Characteristics.Contains(definition))
		{
			Characteristics.Remove(definition);
			actor.OutputHandler.Send(
				$"This input will no longer require or supply the variable {definition.Name.Colour(Telnet.Yellow)}.");
		}
		else
		{
			Characteristics.Add(definition);
			actor.OutputHandler.Send(
				$"This input will now require and supply the variable {definition.Name.Colour(Telnet.Yellow)}.");
		}

		InputChanged = true;
		return true;
	}

	public List<ICharacteristicDefinition> Characteristics { get; } = new();

	public bool DeterminesVariable(ICharacteristicDefinition definition)
	{
		return Characteristics.Contains(definition);
	}

	public ICharacteristicValue GetValueForVariable(ICharacteristicDefinition definition, ICraftInputData data)
	{
		return ((SimpleItemInputData)data).ConsumedItems.First().GetCharacteristic(definition, null);
	}

	public override string InputType => "TagVariable";

	public new static void RegisterCraftInput()
	{
		CraftInputFactory.RegisterCraftInputType("TagVariable",
			(input, craft, game) => new TagVariableInput(input, craft, game));
		CraftInputFactory.RegisterCraftInputTypeForBuilders("tagvariable",
			(craft, game) => new TagVariableInput(craft, game));
	}

	public override bool IsInput(IPerceivable item)
	{
		return item is IGameItem gi &&
		       gi.Tags.Any(x => x.IsA(TargetTag)) &&
		       Characteristics.All(x => gi.CharacteristicDefinitions.Contains(x));
	}

	public override IEnumerable<IPerceivable> ScoutInput(ICharacter character)
	{
		var foundQuantity = 0;
		var foundItems = new List<IGameItem>();
		var returnItems = new List<IPerceivable>();
		foreach (var item in character.DeepContextualItems.Except(character.Body.WornItems)
		                              .Where(x => x.Tags.Any(y => y.IsA(TargetTag)) &&
		                                          Characteristics.All(y => x.CharacteristicDefinitions.Contains(y))))
		{
			foundItems.Add(item);
			if ((foundQuantity += item.Quantity) >= Quantity)
			{
				returnItems.Add(new PerceivableGroup(foundItems));
				foundItems.Clear();
				foundQuantity = 0;
			}
		}

		return returnItems;
	}

	public override string Name
	{
		get
		{
			if (TargetTag == null)
			{
				return $"{Quantity}x an item with {"an unspecified tag".Colour(Telnet.Red)}";
			}

			return
				$"{Quantity}x an item with the {TargetTag.Name.Colour(Telnet.Cyan)} tag [vars: {Characteristics.Select(x => x.Name).ListToString()}]";
		}
	}

	public override string HowSeen(IPerceiver voyeur)
	{
		if (TargetTag == null)
		{
			return $"{Quantity}x an item with {"an unspecified tag".Colour(Telnet.Red)}";
		}

		return
			$"{Quantity}x an item with the {TargetTag.FullName.Colour(Telnet.Cyan)} tag [vars: {Characteristics.Select(x => x.Name).ListToString()}]";
	}

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("TargetTagId", TargetTag?.Id ?? 0),
			new XElement("Quantity", Quantity),
			from item in Characteristics
			select new XElement("Variable", item.Id)
		).ToString();
	}
}