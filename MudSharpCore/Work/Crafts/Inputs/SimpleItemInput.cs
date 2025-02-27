using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Lists;

namespace MudSharp.Work.Crafts.Inputs;

/// <summary>
/// A simple input looks for a specified quantity of a specifically identified item, and consumes that item on conclusion
/// </summary>
public class SimpleItemInput : BaseInput, ICraftInputConsumesGameItemGroup
{
	public override string InputType => "SimpleItem";

	public long TargetItemId { get; set; }
	public int Quantity { get; set; }

	/// <inheritdoc />
	public override bool RefersToItemProto(long id)
	{
		return TargetItemId == id;
	}

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("TargetItemId", TargetItemId),
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
		                              .Where(x => x.Prototype.Id == TargetItemId))
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
		return item is IGameItem gi && gi.Prototype.Id == TargetItemId;
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

	#3quality <weighting>#0 - sets the weighting of this input in determining overall quality
	#3item <id>#0 - sets the item required
	#3quantity <amount>#0 - sets the amount required";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "item":
				return BuildingCommandItem(actor, command);
			case "quantity":
			case "amount":
			case "number":
			case "num":
				return BuildingCommandQuantity(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
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

	private bool BuildingCommandItem(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What item prototype should this item input consume?");
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

		TargetItemId = proto.Id;
		InputChanged = true;
		actor.OutputHandler.Send(
			$"This input will now target item prototype #{TargetItemId.ToString("N0", actor)} ({proto.ShortDescription.ColourObject()}).");
		return true;
	}

	public override bool IsValid()
	{
		return TargetItemId != 0;
	}

	public override string WhyNotValid()
	{
		return "You must first set a target item.";
	}

	#endregion

	protected SimpleItemInput(CraftInput input, ICraft craft, IFuturemud gameworld) : base(input, craft, gameworld)
	{
		var root = XElement.Parse(input.Definition);
		TargetItemId = long.Parse(root.Element("TargetItemId").Value);
		Quantity = int.Parse(root.Element("Quantity").Value);
	}

	protected SimpleItemInput(ICraft craft, IFuturemud gameworld) : base(craft, gameworld)
	{
		Quantity = 1;
	}

	public static void RegisterCraftInput()
	{
		CraftInputFactory.RegisterCraftInputType("SimpleItem",
			(input, craft, game) => new SimpleItemInput(input, craft, game));
		CraftInputFactory.RegisterCraftInputTypeForBuilders("simple",
			(craft, game) => new SimpleItemInput(craft, game));
	}

	public override string Name =>
		$"{Quantity}x {Gameworld.ItemProtos.Get(TargetItemId)?.ShortDescription ?? "an unspecified item".Colour(Telnet.Red)}";

	public override string HowSeen(IPerceiver voyeur)
	{
		return
			$"{Quantity}x {Gameworld.ItemProtos.Get(TargetItemId)?.ShortDescription ?? "an unspecified item".Colour(Telnet.Red)} (#{TargetItemId.ToString("N0", voyeur)})";
	}
}