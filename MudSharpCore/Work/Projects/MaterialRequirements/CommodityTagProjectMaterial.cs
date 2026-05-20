#nullable enable

using MudSharp.Character;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Work.Projects.MaterialRequirements;

public class CommodityTagProjectMaterial : MaterialRequirementBase
{
	public CommodityTagProjectMaterial(Models.ProjectMaterialRequirement requirement, IFuturemud gameworld) : base(
		requirement, gameworld)
	{
		var root = XElement.Parse(requirement.Definition);
		RequiredMaterialTag = Gameworld.Tags.Get(long.Parse(root.Element("MaterialTag")?.Value ?? "0"));
		RequiredTag = Gameworld.Tags.Get(long.Parse(root.Element("Tag")?.Value ?? "0"));
		RequiredAmount = double.Parse(root.Element("Amount")?.Value ?? "0");
		MinimumQuality = (ItemQuality)int.Parse(root.Element("Quality")?.Value ?? "0");
		CharacteristicRequirements.LoadFromXml(root.Element("Characteristics"), Gameworld);
	}

	public CommodityTagProjectMaterial(IFuturemud gameworld, IProjectPhase phase) : base(gameworld, phase, "commoditytag")
	{
		RequiredAmount = 1000;
		MinimumQuality = ItemQuality.Terrible;
	}

	protected CommodityTagProjectMaterial(CommodityTagProjectMaterial rhs, IProjectPhase newPhase) : base(rhs, newPhase, "commoditytag")
	{
		RequiredMaterialTag = rhs.RequiredMaterialTag;
		RequiredTag = rhs.RequiredTag;
		RequiredAmount = rhs.RequiredAmount;
		MinimumQuality = rhs.MinimumQuality;
		CharacteristicRequirements.LoadFromXml(rhs.CharacteristicRequirements.SaveToXml(), Gameworld);
	}

	public override IProjectMaterialRequirement Duplicate(IProjectPhase newPhase)
	{
		return new CommodityTagProjectMaterial(this, newPhase);
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Material",
			new XElement("MaterialTag", RequiredMaterialTag?.Id ?? 0),
			new XElement("Tag", RequiredTag?.Id ?? 0),
			new XElement("Amount", RequiredAmount),
			new XElement("Quality", (int)MinimumQuality),
			CharacteristicRequirements.SaveToXml()
		);
	}

	public ITag? RequiredMaterialTag { get; protected set; }
	public ITag? RequiredTag { get; protected set; }
	public double RequiredAmount { get; protected set; }
	public ItemQuality MinimumQuality { get; protected set; }
	public CommodityCharacteristicRequirement CharacteristicRequirements { get; } = new();

	public override double QuantityRequired => RequiredAmount;

	public override bool ItemCounts(IGameItem item)
	{
		return RequiredMaterialTag is not null &&
		       item.GetItemType<ICommodity>() is ICommodity ic &&
		       ic.Material.IsA(RequiredMaterialTag) &&
		       (
			       (RequiredTag is null && ic.Tag is null) ||
			       ic.Tag?.IsA(RequiredTag) == true
		       ) &&
		       CharacteristicRequirements.Matches(ic) &&
		       item.Weight >= RequiredAmount &&
		       item.Quality >= MinimumQuality;
	}

	public override double SupplyItem(ICharacter actor, IGameItem item, IActiveProject project)
	{
		var amount = RequiredAmount - project.MaterialProgress[this];
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
		var amount = RequiredAmount - project.MaterialProgress[this];
		actor.OutputHandler.Send(
			$"You would supply {item.PeekSplitByWeight(amount).HowSeen(actor)} to the {Name.ColourValue()} requirement of that project.");
	}

	public override string DescribeQuantity(ICharacter actor)
	{
		return
			$"{Gameworld.UnitManager.DescribeExact(RequiredAmount, UnitType.Mass, actor)} of material tagged as {RequiredMaterialTag?.FullName.Colour(Telnet.Cyan) ?? "an unknown material tag".ColourError()}{RequiredTag?.Name.Pluralise().LeadingSpaceIfNotEmpty().Colour(Telnet.Cyan) ?? ""} {CharacteristicRequirements.Describe()}";
	}

	protected override IInventoryPlanAction LocateMaterialAction()
	{
		return new InventoryPlanActionHold(Gameworld, 0, 0,
			x => RequiredMaterialTag is not null &&
			     x.GetItemType<ICommodity>() is { } ic &&
			     ic.Material.IsA(RequiredMaterialTag) &&
			     ((ic.Tag is null && RequiredTag is null) || ic.Tag?.IsA(RequiredTag) == true) &&
			     CharacteristicRequirements.Matches(ic),
			null, 0)
		{
			ItemsAlreadyInPlaceOverrideFitnessScore = true,
			QuantityIsOptional = true,
			OriginalReference = "target"
		};
	}

	protected override string HelpText => $@"{base.HelpText}
	#3material <tag>#0 - sets the material tag the commodity must match
	#3materialtag <tag>#0 - sets the material tag the commodity must match
	#3tag <tag|none>#0 - sets the secondary commodity pile tag
	#3piletag <tag|none>#0 - sets the secondary commodity pile tag
	#3amount <##>#0 - sets the weight of the material required
	#3characteristic any|none|<definition> any|<definition> <value>|<definition> remove#0 - sets commodity characteristic requirements
	#3quality <quality>#0 - sets the minimum quality of the materials";

	public override bool BuildingCommand(ICharacter actor, StringStack command, IProjectPhase phase)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "material":
			case "materialtag":
				return BuildingCommandMaterialTag(actor, command);
			case "tag":
			case "piletag":
			case "commoditytag":
				return BuildingCommandPileTag(actor, command);
			case "amount":
			case "number":
			case "quantity":
			case "num":
				return BuildingCommandAmount(actor, command);
			case "quality":
				return BuildingCommandQuality(actor, command);
			case "characteristic":
			case "characteristics":
			case "char":
				return CharacteristicRequirements.BuildingCommand(actor, command, "project material requirement", () => Changed = true);
		}

		return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"), phase);
	}

	private bool BuildingCommandMaterialTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which material tag should the commodity material have to satisfy this material requirement?");
			return false;
		}

		var matchedTags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
		if (matchedTags.Count == 0)
		{
			actor.OutputHandler.Send("There is no such tag.");
			return false;
		}

		if (matchedTags.Count > 1)
		{
			actor.OutputHandler.Send(
				$"Your text matched multiple tags. Please specify one of the following tags:\n\n{matchedTags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {StringColourExtensions.ColourName(x.FullName)}").ListToLines()}");
			return false;
		}

		RequiredMaterialTag = matchedTags.Single();
		Changed = true;
		actor.OutputHandler.Send(
			$"This requirement now accepts commodities made from materials tagged {RequiredMaterialTag.FullName.Colour(Telnet.Cyan)}.");
		return true;
	}

	private bool BuildingCommandPileTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which secondary tag should the commodity have to satisfy this material requirement?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			RequiredTag = null;
			actor.OutputHandler.Send("This requirement will now accept only untagged commodity piles.");
			Changed = true;
			return true;
		}

		var matchedTags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
		if (matchedTags.Count == 0)
		{
			actor.OutputHandler.Send("There is no such tag.");
			return false;
		}

		if (matchedTags.Count > 1)
		{
			actor.OutputHandler.Send(
				$"Your text matched multiple tags. Please specify one of the following tags:\n\n{matchedTags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {StringColourExtensions.ColourName(x.FullName)}").ListToLines()}");
			return false;
		}

		RequiredTag = matchedTags.Single();
		Changed = true;
		actor.OutputHandler.Send(
			$"This requirement now requires a commodity with the {RequiredTag.FullName.Colour(Telnet.Cyan)} secondary tag.");
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
			actor.OutputHandler.Send("What weight of commodity is required to satisfy this requirement?");
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

	protected override bool BuildingCommandShow(ICharacter actor, StringStack command, IProjectPhase phase)
	{
		var sb = new StringBuilder();
		sb.AppendLine(
			$"Commodity Tag Material Requirement {Id.ToString("N0", actor).ColourValue()} - {Name.Colour(Telnet.Cyan)}");
		sb.AppendLine($"Required Material Tag: {RequiredMaterialTag?.FullName.ColourName() ?? "None".ColourError()}");
		sb.AppendLine($"Required Tag: {RequiredTag?.FullName.Colour(Telnet.Cyan) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Required Amount: {Gameworld.UnitManager.DescribeExact(RequiredAmount, UnitType.Mass, actor).ColourValue()}");
		sb.AppendLine($"Minimum Quality: {MinimumQuality.Describe().ColourValue()}");
		sb.AppendLine($"Required Characteristics: {CharacteristicRequirements.Describe()}");
		sb.AppendLine($"Description: {Description}");
		actor.OutputHandler.Send(sb.ToString());
		return true;
	}

	public override (bool Truth, string Error) CanSubmit()
	{
		if (RequiredMaterialTag is null)
		{
			return (false, "You must set a material tag.");
		}

		return base.CanSubmit();
	}

	public override string Show(ICharacter actor)
	{
		return
			$"{Gameworld.UnitManager.DescribeExact(RequiredAmount, UnitType.Mass, actor)} of material tagged as {RequiredMaterialTag?.FullName.ColourName() ?? "an unknown material tag".ColourError()}{RequiredTag?.Name.Pluralise().LeadingSpaceIfNotEmpty().Colour(Telnet.Cyan) ?? ""} {CharacteristicRequirements.Describe()} (>={MinimumQuality.Describe().Colour(Telnet.Green)})";
	}

	public override string ShowToPlayer(ICharacter actor)
	{
		return Show(actor);
	}
}
