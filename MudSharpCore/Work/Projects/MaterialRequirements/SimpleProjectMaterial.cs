using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Work.Projects.MaterialRequirements;

public class SimpleProjectMaterial : MaterialRequirementBase
{
	public SimpleProjectMaterial(Models.ProjectMaterialRequirement requirement, IFuturemud gameworld) : base(
		requirement, gameworld)
	{
		var root = XElement.Parse(requirement.Definition);
		RequiredTag = Gameworld.Tags.Get(long.Parse(root.Element("Tag").Value));
		RequiredAmount = int.Parse(root.Element("Amount").Value);
		MinimumQuality = (ItemQuality)int.Parse(root.Element("Quality").Value);
	}

	public SimpleProjectMaterial(IFuturemud gameworld, IProjectPhase phase) : base(gameworld, phase, "simple")
	{
		RequiredAmount = 1;
		MinimumQuality = ItemQuality.Terrible;
	}

	protected SimpleProjectMaterial(SimpleProjectMaterial rhs, IProjectPhase newPhase) : base(rhs, newPhase, "simple")
	{
		RequiredTag = rhs.RequiredTag;
		RequiredAmount = rhs.RequiredAmount;
		MinimumQuality = rhs.MinimumQuality;
	}

	public override IProjectMaterialRequirement Duplicate(IProjectPhase newPhase)
	{
		return new SimpleProjectMaterial(this, newPhase);
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Material",
			new XElement("Tag", RequiredTag?.Id ?? 0),
			new XElement("Amount", RequiredAmount),
			new XElement("Quality", (int)MinimumQuality)
		);
	}

	public ITag RequiredTag { get; protected set; }

	public int RequiredAmount { get; protected set; }

	public ItemQuality MinimumQuality { get; protected set; }

	public override double QuantityRequired => RequiredAmount;

	public override bool ItemCounts(IGameItem item)
	{
		return item.IsA(RequiredTag) && item.Quality >= MinimumQuality;
	}

	public override void SupplyItem(ICharacter actor, IGameItem item, IActiveProject project)
	{
		var amount = (int)(RequiredAmount - project.MaterialProgress[this]);
		if (item.DropsWhole(amount))
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ supply|supplies $1 to meet the {Name.ColourValue()} requirement of the {project.Name.Colour(Telnet.Cyan)} project.",
				actor, actor, item)));
			item.Delete();
			return;
		}

		var temp = item.PeekSplit(amount);
		item.GetItemType<IStackable>().Quantity -= amount;
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"@ supply|supplies $1 to meet the {Name.ColourValue()} requirement of the {project.Name.Colour(Telnet.Cyan)} project.",
			actor, actor, temp)));
	}

	public override void PeekSupplyItem(ICharacter actor, IGameItem item, IActiveProject project)
	{
		var amount = (int)(RequiredAmount - project.MaterialProgress[this]);
		actor.OutputHandler.Send(
			$"You would supply {item.PeekSplit(amount).HowSeen(actor)} to the {Name.ColourValue()} requirement of that project.");
	}

	public override string DescribeQuantity(ICharacter actor)
	{
		return
			$"{RequiredAmount.ToString("N0", actor).ColourValue()}x {RequiredTag?.Name.Colour(Telnet.Cyan) ?? "Unknown".Colour(Telnet.Red)}";
	}

	protected override IInventoryPlanAction LocateMaterialAction()
	{
		return new InventoryPlanActionHold(Gameworld, RequiredTag.Id, 0, null, null, RequiredAmount)
			{ ItemsAlreadyInPlaceOverrideFitnessScore = true, QuantityIsOptional = true };
	}

	#region Overrides of MaterialRequirementBase

	protected override string HelpText => $@"{base.HelpText}
	#3tag <tag>#0 - sets the tag the item that satisifies this requirement needs to have
	#3amount <##>#0 - sets the number of the material required";

	#endregion

	public override bool BuildingCommand(ICharacter actor, StringStack command, IProjectPhase phase)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "tag":
				return BuildingCommandTag(actor, command);
			case "amount":
			case "number":
			case "quantity":
			case "num":
				return BuildingCommandAmount(actor, command);
		}

		return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"), phase);
	}

	private bool BuildingCommandAmount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many items are required to satisfy this requirement?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must enter a valid, positive number of items required.");
			return false;
		}

		RequiredAmount = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This requirement now requires {RequiredAmount} item{(RequiredAmount == 1 ? "" : "s")} to be satisfied.");
		return true;
	}

	private bool BuildingCommandTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which tag should satisfy this material requirement?");
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

		RequiredTag = tag;
		Changed = true;
		actor.OutputHandler.Send(
			$"This requirement now requires an item with the {RequiredTag.FullName.Colour(Telnet.Cyan)} tag.");
		return true;
	}

	protected override bool BuildingCommandShow(ICharacter actor, StringStack command, IProjectPhase phase)
	{
		var sb = new StringBuilder();
		sb.AppendLine(
			$"Simple Material Requirement {Id.ToString("N0", actor).ColourValue()} - {Name.Colour(Telnet.Cyan)}");
		sb.AppendLine($"Required Tag: {RequiredTag?.Name.Colour(Telnet.Cyan) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Required Amount: {RequiredAmount.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Minimum Quality: {MinimumQuality.Describe().ColourValue()}");
		sb.AppendLine($"Description: {Description}");
		actor.OutputHandler.Send(sb.ToString());
		return true;
	}

	public override (bool Truth, string Error) CanSubmit()
	{
		if (RequiredTag == null)
		{
			return (false, "You must set a RequiredTag.");
		}

		return base.CanSubmit();
	}

	public override string Show(ICharacter actor)
	{
		return
			$"{QuantityRequired.ToString("N0", actor)} of item tagged {RequiredTag.FullName.Colour(Telnet.Cyan)} (>={MinimumQuality.Describe().Colour(Telnet.Green)})";
	}

	public override string ShowToPlayer(ICharacter actor)
	{
		return
			$"{QuantityRequired.ToString("N0", actor)} of item tagged {RequiredTag.FullName.Colour(Telnet.Cyan)} (>={MinimumQuality.Describe().Colour(Telnet.Green)})";
	}
}