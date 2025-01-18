using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Work.Projects.MaterialRequirements;

public class CommodityProjectMaterial : MaterialRequirementBase
{
	public CommodityProjectMaterial(Models.ProjectMaterialRequirement requirement, IFuturemud gameworld) : base(
		requirement, gameworld)
	{
		var root = XElement.Parse(requirement.Definition);
		RequiredTag = Gameworld.Tags.Get(long.Parse(root.Element("Tag").Value));
		RequiredAmount = int.Parse(root.Element("Amount").Value);
		MinimumQuality = (ItemQuality)int.Parse(root.Element("Quality").Value);
		RequiredMaterial = Gameworld.Materials.Get(long.Parse(root.Element("Material").Value));
	}

	public CommodityProjectMaterial(IFuturemud gameworld, IProjectPhase phase) : base(gameworld, phase, "commodity")
	{
		RequiredAmount = 1000;
		MinimumQuality = ItemQuality.Terrible;
	}

	protected CommodityProjectMaterial(CommodityProjectMaterial rhs, IProjectPhase newPhase) : base(rhs, newPhase, "commodity")
	{
		RequiredTag = rhs.RequiredTag;
		RequiredAmount = rhs.RequiredAmount;
		MinimumQuality = rhs.MinimumQuality;
		RequiredMaterial = rhs.RequiredMaterial;
	}

	public override IProjectMaterialRequirement Duplicate(IProjectPhase newPhase)
	{
		return new CommodityProjectMaterial(this, newPhase);
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Material",
			new XElement("Material", RequiredMaterial?.Id ?? 0),
			new XElement("Tag", RequiredTag?.Id ?? 0),
			new XElement("Amount", RequiredAmount),
			new XElement("Quality", (int)MinimumQuality)
		);
	}

	public ISolid RequiredMaterial { get; protected set; }
	public ITag RequiredTag { get; protected set; }

	public double RequiredAmount { get; protected set; }

	public ItemQuality MinimumQuality { get; protected set; }

	public override double QuantityRequired => RequiredAmount;

	public override bool ItemCounts(IGameItem item)
	{
		return item.GetItemType<ICommodity>() is ICommodity ic &&
			   ic.Material == RequiredMaterial &&
		       (
			       (RequiredTag is null && ic.Tag is null) ||
			       (ic.Tag?.IsA(RequiredTag) == true)
		       ) &&
			   item.Weight >= RequiredAmount &&
		       item.Quality >= MinimumQuality;
	}

	public override double SupplyItem(ICharacter actor, IGameItem item, IActiveProject project)
	{
		var amount = (RequiredAmount - project.MaterialProgress[this]);
		if (item.DropsWholeByWeight(amount))
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ supply|supplies $1 to meet the {Name.ColourValue()} requirement of the {project.Name.Colour(Telnet.Cyan)} project.",
				actor, actor, item)));
			var weight = item.Weight;
			item.Delete();
			return weight;
		}

		var temp = item.PeekSplitByWeight(amount);
		item.GetItemType<ICommodity>().Weight -= amount;
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"@ supply|supplies $1 to meet the {Name.ColourValue()} requirement of the {project.Name.Colour(Telnet.Cyan)} project.",
			actor, actor, temp)));
		return temp.Weight;
	}

	public override void PeekSupplyItem(ICharacter actor, IGameItem item, IActiveProject project)
	{
		var amount = (RequiredAmount - project.MaterialProgress[this]);
		actor.OutputHandler.Send(
			$"You would supply {item.PeekSplitByWeight(amount).HowSeen(actor)} to the {Name.ColourValue()} requirement of that project.");
	}

	public override string DescribeQuantity(ICharacter actor)
	{
		return
			$"{Gameworld.UnitManager.DescribeExact(RequiredAmount, UnitType.Mass, actor)} of {RequiredMaterial?.Name.Colour(RequiredMaterial.ResidueColour) ?? "an unknown material".ColourError()}{RequiredTag?.Name.Pluralise().LeadingSpaceIfNotEmpty().Colour(Telnet.Cyan) ?? ""}";
	}

	protected override IInventoryPlanAction LocateMaterialAction()
	{
		return new InventoryPlanActionHold(Gameworld, 0, 0, x => x.GetItemType<ICommodity>() is {} ic && ic.Material == RequiredMaterial && ((ic.Tag is null && RequiredTag is null) || (ic.Tag?.IsA(RequiredTag) == true)), null, 0)
		{
			ItemsAlreadyInPlaceOverrideFitnessScore = true, 
			QuantityIsOptional = true,
			OriginalReference = "target"
		};
	}

	#region Overrides of MaterialRequirementBase

	protected override string HelpText => $@"{base.HelpText}
	#3tag <tag>#0 - sets the tag the item that satisfies this requirement needs to have
	#3amount <##>#0 - sets the weight of the material required
	#3material <which>#0 - sets the material required
	#3quality <quality>#0 - sets the minimum quality of the materials";

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
			case "quality":
				return BuildingCommandQuality(actor, command);
			case "material":
				return BuildingCommandMaterial(actor, command);
		}

		return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"), phase);
	}

	private bool BuildingCommandMaterial(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which material should the commodity for this requirement be?");
			return false;
		}

		var material = Gameworld.Materials.GetByIdOrName(command.SafeRemainingArgument);
		if (material is null)
		{
			actor.OutputHandler.Send($"There is no material identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		RequiredMaterial = material;
		Changed = true;
		actor.OutputHandler.Send($"This requirement will now require a commodity of material {material.Name.Colour(material.ResidueColour)} to be satisfied.");
		return true;
	}

	private bool BuildingCommandQuality(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What minimum quality do you want to set? The valid values are {Enum.GetValues<ItemQuality>().ListToColouredString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<ItemQuality>(out var quality))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid quality. The valid values are {Enum.GetValues<ItemQuality>().ListToColouredString()}.");
			return false;
		}

		MinimumQuality = quality;
		Changed = true;
		actor.OutputHandler.Send($"Materials supplied will now need to be of at least {quality.Describe().ColourValue()} quality.");
		return true;
	}

	private bool BuildingCommandAmount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What weight of commodity required to satisfy this requirement?");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.Mass, actor, out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must enter a valid, positive weight of commodity required.");
			return false;
		}

		RequiredAmount = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This requirement now requires {Gameworld.UnitManager.DescribeExact(value, UnitType.Mass, actor).ColourValue()} weight of commodity to be satisfied.");
		return true;
	}

	private bool BuildingCommandTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which tag should the commodity have to satisfy this material requirement?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			RequiredTag = null;
			actor.OutputHandler.Send($"This requirement will now require the unmodified base form of the commodity with no tag.");
			Changed = true;
			return true;
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
				$"Your text matched multiple tags. Please specify one of the following tags:\n\n{matchedtags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {StringColourExtensions.ColourName(x.FullName)}").ListToLines()}");
			return false;
		}

		var tag = matchedtags.Single();

		RequiredTag = tag;
		Changed = true;
		actor.OutputHandler.Send(
			$"This requirement now requires a commodity with the {RequiredTag.FullName.Colour(Telnet.Cyan)} secondary tag.");
		return true;
	}

	protected override bool BuildingCommandShow(ICharacter actor, StringStack command, IProjectPhase phase)
	{
		var sb = new StringBuilder();
		sb.AppendLine(
			$"Simple Material Requirement {Id.ToString("N0", actor).ColourValue()} - {Name.Colour(Telnet.Cyan)}");
		sb.AppendLine($"Required Material: {RequiredMaterial?.Name.Colour(RequiredMaterial.ResidueColour) ?? "None".ColourError()}");
		sb.AppendLine($"Required Tag: {RequiredTag?.Name.Colour(Telnet.Cyan) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Required Amount: {Gameworld.UnitManager.DescribeExact(RequiredAmount, UnitType.Mass, actor).ColourValue()}");
		sb.AppendLine($"Minimum Quality: {MinimumQuality.Describe().ColourValue()}");
		sb.AppendLine($"Description: {Description}");
		actor.OutputHandler.Send(sb.ToString());
		return true;
	}

	public override (bool Truth, string Error) CanSubmit()
	{
		if (RequiredMaterial == null)
		{
			return (false, "You must set a material.");
		}

		return base.CanSubmit();
	}

	public override string Show(ICharacter actor)
	{
		return
			$"{Gameworld.UnitManager.DescribeExact(RequiredAmount, UnitType.Mass, actor)} of {RequiredMaterial?.Name.Colour(RequiredMaterial.ResidueColour) ?? "an unknown material".ColourError()}{RequiredTag?.Name.Pluralise().LeadingSpaceIfNotEmpty().Colour(Telnet.Cyan) ?? ""} (>={MinimumQuality.Describe().Colour(Telnet.Green)})";
	}

	public override string ShowToPlayer(ICharacter actor)
	{
		return
			$"{Gameworld.UnitManager.DescribeExact(RequiredAmount, UnitType.Mass, actor)} of {RequiredMaterial?.Name.Colour(RequiredMaterial.ResidueColour) ?? "an unknown material".ColourError()}{RequiredTag?.Name.Pluralise().LeadingSpaceIfNotEmpty().Colour(Telnet.Cyan) ?? ""} (>={MinimumQuality.Describe().Colour(Telnet.Green)})";
	}
}