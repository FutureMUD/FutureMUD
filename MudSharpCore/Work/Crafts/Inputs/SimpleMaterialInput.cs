using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Lists;

namespace MudSharp.Work.Crafts.Inputs;

public class SimpleMaterialInput : BaseInput, ICraftInputConsumesGameItemGroup
{
	protected override string InputType => "SimpleMaterial";

	public ITag TargetMaterialTag { get; set; }
	public ISolid TargetMaterial { get; set; }
	public int Quantity { get; set; }

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("TargetMaterialTag", TargetMaterialTag?.Id ?? 0),
			new XElement("TargetMaterial", TargetMaterial?.Id ?? 0),
			new XElement("Quantity", Quantity)
		).ToString();
	}

	#region Implementation of ICraftInput

	public override IEnumerable<IPerceivable> ScoutInput(ICharacter character)
	{
		var foundQuantity = 0;
		var foundItems = new List<IGameItem>();
		var returnItems = new List<IPerceivable>();
		foreach (var item in character.DeepContextualItems.Except(character.Body.WornItems)
		                              .Where(x => x.IsItemType<IHoldable>() &&
		                                          (TargetMaterial == null || x.Material == TargetMaterial) &&
		                                          (TargetMaterialTag == null || x.Material.IsA(TargetMaterialTag))))
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

	public override bool IsInput(IPerceivable item)
	{
		return item is IGameItem gi && gi.IsItemType<IHoldable>() && gi.Quantity >= Quantity &&
		       (TargetMaterial == null || gi.Material == TargetMaterial) &&
		       (TargetMaterialTag == null || gi.Material.IsA(TargetMaterialTag));
	}

	public override void UseInput(IPerceivable item, ICraftInputData data)
	{
		data.Delete();
	}

	public override double ScoreInputDesirability(IPerceivable item)
	{
		return 1.0;
	}

	public override ICraftInputData ReserveInput(IPerceivable input)
	{
		return new SimpleItemInputData(((PerceivableGroup)input).Members.OfType<IGameItem>(), Quantity);
	}

	public override ICraftInputData LoadDataFromXml(XElement root, IFuturemud gameworld)
	{
		return new SimpleItemInputData(root, gameworld);
	}

	protected override string BuildingHelpString =>
		@"You can use the following options with this input type:
	#3material <material>#0 - sets the material required
	#3tag <tag>#0 - sets the materila tag required
	#3quantity <amount>#0 - sets the amount required";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "material":
				return BuildingCommandMaterial(actor, command);
			case "tag":
				return BuildingCommandTag(actor, command);
			case "quantity":
			case "amount":
			case "number":
			case "num":
				return BuildingCommandQuantity(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which tag should materials that this input targets have to have?");
			return false;
		}

		var matchedtags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
		if (matchedtags.Count == 0)
		{
			actor.OutputHandler.Send("There is no such tag.");
			return false;
		}

		if (matchedtags.Count > 1)
		{
			actor.OutputHandler.Send(
				$"Your text matched multiple tags. Please specify one of the following tags:\n\n{matchedtags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
			return false;
		}

		var tag = matchedtags.Single();

		TargetMaterialTag = tag;
		TargetMaterial = null;
		InputChanged = true;
		actor.OutputHandler.Send(
			$"This input will now require an input with materials tagged as {TargetMaterialTag.FullName.Colour(Telnet.Cyan)}.");
		return true;
	}

	private bool BuildingCommandMaterial(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which material should input targets have to be?");
			return false;
		}

		var material = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.Materials.Get(value)
			: Gameworld.Materials.GetByName(command.SafeRemainingArgument);
		if (material == null)
		{
			actor.OutputHandler.Send("There is no such material.");
			return false;
		}

		TargetMaterialTag = null;
		TargetMaterial = material;
		InputChanged = true;
		actor.OutputHandler.Send(
			$"This input will now require an input with the {TargetMaterial.Name.Colour(material.ResidueColour)} material.");
		return true;
	}

	private bool BuildingCommandQuantity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must enter a valid amount of this item for this input to consume.");
			return false;
		}

		Quantity = value;
		InputChanged = true;
		actor.OutputHandler.Send($"This input will now consume {Quantity} of the target item.");
		return true;
	}

	public override bool IsValid()
	{
		return TargetMaterial != null || TargetMaterialTag != null;
	}

	public override string WhyNotValid()
	{
		return "You must set either the target material, or the target material tag.";
	}

	#endregion

	protected SimpleMaterialInput(CraftInput input, ICraft craft, IFuturemud gameworld) : base(input, craft, gameworld)
	{
		var root = XElement.Parse(input.Definition);
		TargetMaterial = Gameworld.Materials.Get(long.Parse(root.Element("TargetMaterial").Value));
		TargetMaterialTag = Gameworld.Tags.Get(long.Parse(root.Element("TargetMaterialTag").Value));
		Quantity = int.Parse(root.Element("Quantity").Value);
	}

	protected SimpleMaterialInput(ICraft craft, IFuturemud gameworld) : base(craft, gameworld)
	{
		Quantity = 1;
	}

	public static void RegisterCraftInput()
	{
		CraftInputFactory.RegisterCraftInputType("SimpleMaterial",
			(input, craft, game) => new SimpleMaterialInput(input, craft, game));
		CraftInputFactory.RegisterCraftInputTypeForBuilders("simplematerial",
			(craft, game) => new SimpleMaterialInput(craft, game));
	}

	public override string Name
	{
		get
		{
			if (TargetMaterial != null)
			{
				return $"{Quantity}x an item made of {TargetMaterial.Name.Colour(TargetMaterial.ResidueColour)}";
			}

			if (TargetMaterialTag != null)
			{
				return $"{Quantity}x an item with material tagged as {TargetMaterialTag.FullName.Colour(Telnet.Cyan)}";
			}

			return $"{Quantity}x an item with unspecified material properties";
		}
	}

	public override string HowSeen(IPerceiver voyeur)
	{
		if (TargetMaterial != null)
		{
			return $"{Quantity}x an item made of {TargetMaterial.Name.Colour(TargetMaterial.ResidueColour)}";
		}

		if (TargetMaterialTag != null)
		{
			return $"{Quantity}x an item with material tagged as {TargetMaterialTag.FullName.Colour(Telnet.Cyan)}";
		}

		return $"{Quantity}x an item with unspecified material properties";
	}
}