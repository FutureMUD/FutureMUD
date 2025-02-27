using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine.Lists;

namespace MudSharp.Work.Crafts.Inputs;

public class TagInput : BaseInput, ICraftInputConsumesGameItemGroup
{
	public override string InputType => "Tag";
	public ITag TargetTag { get; set; }
	public int Quantity { get; set; }

	/// <inheritdoc />
	public override bool RefersToTag(ITag tag)
	{
		return TargetTag?.IsA(tag) == true;
	}

	protected TagInput(Models.CraftInput input, ICraft craft, IFuturemud gameworld) : base(input, craft, gameworld)
	{
		var root = XElement.Parse(input.Definition);
		TargetTag = gameworld.Tags.Get(long.Parse(root.Element("TargetTagId").Value));
		Quantity = int.Parse(root.Element("Quantity").Value);
	}

	protected TagInput(ICraft craft, IFuturemud gameworld) : base(craft, gameworld)
	{
		Quantity = 1;
	}

	public static void RegisterCraftInput()
	{
		CraftInputFactory.RegisterCraftInputType("Tag", (input, craft, game) => new TagInput(input, craft, game));
		CraftInputFactory.RegisterCraftInputTypeForBuilders("tag", (craft, game) => new TagInput(craft, game));
	}

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("TargetTagId", TargetTag?.Id ?? 0),
			new XElement("Quantity", Quantity)
		).ToString();
	}

	protected override string BuildingHelpString =>
		@"You can use the following options with this input type:

	#3quality <weighting>#0 - sets the weighting of this input in determining overall quality
	#3tag <id|name>#0 - sets the tag required
	#3quantity <amount>#0 - sets the amount required";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "tag":
			case "item":
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
			actor.OutputHandler.Send("Which tag do you want items to have to satisfy this input?");
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

		TargetTag = tag;
		InputChanged = true;
		actor.OutputHandler.Send($"This input will now target items with the {tag.FullName.Colour(Telnet.Cyan)} tag.");
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
		return TargetTag != null;
	}

	public override string WhyNotValid()
	{
		return "You must first set a target tag for this input.";
	}

	public override string Name
	{
		get
		{
			if (TargetTag == null)
			{
				return $"{Quantity}x an item with {"an unspecified tag".Colour(Telnet.Red)}";
			}

			return $"{Quantity}x an item with the {TargetTag.FullName.Colour(Telnet.Cyan)} tag";
		}
	}

	public override string HowSeen(IPerceiver voyeur)
	{
		if (TargetTag == null)
		{
			return $"{Quantity}x an item with {"an unspecified tag".Colour(Telnet.Red)}";
		}

		return $"{Quantity}x an item with the {TargetTag.FullName.Colour(Telnet.Cyan)} tag";
	}

	#region Implementation of ICraftInput

	public override IEnumerable<IPerceivable> ScoutInput(ICharacter character)
	{
		var foundQuantity = 0;
		var foundItems = new List<IGameItem>();
		var returnItems = new List<IPerceivable>();
		foreach (var item in character.DeepContextualItems.Except(character.Body.WornItems)
		                              .Where(x => x.Tags.Any(y => y.IsA(TargetTag))))
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
		return item is IGameItem gi &&
		       gi.Tags.Any(x => x.IsA(TargetTag));
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

	#endregion
}