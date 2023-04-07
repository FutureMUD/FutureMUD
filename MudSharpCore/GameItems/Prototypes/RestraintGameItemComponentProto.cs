using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Prototypes;

public class RestraintGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "Restraint";

	public SizeCategory MinimumCreatureSize { get; set; }
	public SizeCategory MaximumCreatureSize { get; set; }
	public HashSet<LimbType> LimbTypes { get; } = new();
	public Difficulty BreakoutDifficulty { get; set; }
	public Difficulty OverpowerDifficulty { get; set; }

	#region Constructors

	protected RestraintGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"Restraint")
	{
		MinimumCreatureSize = SizeCategory.Small;
		MaximumCreatureSize = SizeCategory.VeryLarge;
		LimbTypes.Add(LimbType.Arm);
		LimbTypes.Add(LimbType.Leg);
		LimbTypes.Add(LimbType.Appendage);
		BreakoutDifficulty = Difficulty.Normal;
		OverpowerDifficulty = Difficulty.Normal;
	}

	protected RestraintGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		MinimumCreatureSize = (SizeCategory)int.Parse(root.Element("MinimumCreatureSize").Value);
		MaximumCreatureSize = (SizeCategory)int.Parse(root.Element("MaximumCreatureSize").Value);
		BreakoutDifficulty = (Difficulty)int.Parse(root.Element("BreakoutDifficulty").Value);
		OverpowerDifficulty = (Difficulty)int.Parse(root.Element("OverpowerDifficulty").Value);
		foreach (var item in root.Elements("LimbType"))
		{
			LimbTypes.Add((LimbType)int.Parse(item.Value));
		}
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("MinimumCreatureSize", (int)MinimumCreatureSize),
			new XElement("MaximumCreatureSize", (int)MaximumCreatureSize),
			new XElement("BreakoutDifficulty", (int)BreakoutDifficulty),
			new XElement("OverpowerDifficulty", (int)OverpowerDifficulty),
			from item in LimbTypes select new XElement("LimbType", (int)item)
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new RestraintGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new RestraintGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Restraint".ToLowerInvariant(), true,
			(gameworld, account) => new RestraintGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Restraint",
			(proto, gameworld) => new RestraintGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Restraint",
			$"Can be used to restrain limbs, combined with a {"[wearable]".Colour(Telnet.BoldYellow)}",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new RestraintGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tminsize <size> - the minimum size of the creature this can restrain\n\tmaxsize <size> - the maximum size of the creature this can restrain\n\toverpower <difficulty> - the difficulty to breakout of these restraints by strength\n\tbreakout <difficulty> - the difficulty to breakout of these restraints by dexterity\n\tlimb arm|leg|head|torso|wing|tail|appendage|genitals - toggles a limb type being bound by this restraint";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "minsize":
			case "min size":
				return BuildingCommandMinSize(actor, command);
			case "maxsize":
			case "max size":
				return BuildingCommandMaxSize(actor, command);
			case "limb":
			case "limbs":
			case "limb types":
			case "limb type":
			case "type":
				return BuildingCommandLimb(actor, command);
			case "breakout":
				return BuildingCommandBreakout(actor, command);
			case "overpower":
				return BuildingCommandOverpower(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandMaxSize(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				$"What size do you want to set to be the maximum size for a creature to effectively be restrained? See {"show sizes".Colour(Telnet.Yellow)} for a list of possible sizes.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<SizeCategory>(out var size))
		{
			actor.OutputHandler.Send(
				$"That is not a valid size. See {"show sizes".Colour(Telnet.Yellow)} for a correct list.");
			return false;
		}

		MaximumCreatureSize = size;
		Changed = true;
		if (MinimumCreatureSize > MaximumCreatureSize)
		{
			MinimumCreatureSize = MaximumCreatureSize;
		}

		actor.Send(
			$"This restraint will now bind creatures of {(MinimumCreatureSize == MaximumCreatureSize ? $"size {MinimumCreatureSize.Describe().Colour(Telnet.Green)}" : $"sizes between {MinimumCreatureSize.Describe().Colour(Telnet.Green)} and {MaximumCreatureSize.Describe().Colour(Telnet.Green)}")}.");
		return true;
	}

	private bool BuildingCommandOverpower(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What difficulty should it be to overpower the strength of this bindings and break them?");
			return false;
		}

		if (!CheckExtensions.GetDifficulty(command.SafeRemainingArgument, out var difficulty))
		{
			actor.Send("That is not a valid difficulty.");
			return false;
		}

		OverpowerDifficulty = difficulty;
		Changed = true;
		actor.Send(
			$"It is now {OverpowerDifficulty.Describe().Colour(Telnet.Green)} to overpower the strength of this binding and break it.");
		return true;
	}

	private bool BuildingCommandBreakout(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What difficulty should it be to get free from these bindings by agility?");
			return false;
		}

		if (!CheckExtensions.GetDifficulty(command.SafeRemainingArgument, out var difficulty))
		{
			actor.Send("That is not a valid difficulty.");
			return false;
		}

		BreakoutDifficulty = difficulty;
		Changed = true;
		actor.Send(
			$"It is now {BreakoutDifficulty.Describe().Colour(Telnet.Green)} to break free of the bindings via agility.");
		return true;
	}

	private bool BuildingCommandLimb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which limb type do you want to toggle being able to be binded by this restraint?");
			return false;
		}

		var cmd = command.SafeRemainingArgument;
		var limbs = Enum.GetValues(typeof(LimbType)).OfType<LimbType>().ToArray();
		if (!limbs.Any(x => x.Describe().EqualTo(cmd) || x.DescribePlural().EqualTo(cmd)))
		{
			actor.Send("There is no such limb type.");
			return false;
		}

		var limb = limbs.First(x => x.Describe().EqualTo(cmd) || x.DescribePlural().EqualTo(cmd));
		if (LimbTypes.Contains(limb))
		{
			LimbTypes.Remove(limb);
			Changed = true;
			actor.Send($"This restraint will no longer bind limbs of type {limb.Describe().Colour(Telnet.Green)}.");
			return true;
		}

		LimbTypes.Add(limb);
		Changed = true;
		actor.Send($"This restraint will now bind limbs of type {limb.Describe().Colour(Telnet.Green)}.");
		return true;
	}

	private bool BuildingCommandMinSize(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				$"What size do you want to set to be the minimum size for a creature to effectively be restrained? See {"show sizes".Colour(Telnet.Yellow)} for a list of possible sizes.");
			return false;
		}

		var cmd = command.SafeRemainingArgument;
		var size = Enum.GetValues(typeof(SizeCategory)).OfType<SizeCategory>();
		SizeCategory target;
		var itemSizeCategories = size as SizeCategory[] ?? size.ToArray();
		if (itemSizeCategories.Any(x => x.Describe().ToLowerInvariant().StartsWith(cmd, StringComparison.Ordinal)))
		{
			target = itemSizeCategories.FirstOrDefault(x =>
				x.Describe().ToLowerInvariant().StartsWith(cmd, StringComparison.Ordinal));
		}
		else
		{
			actor.OutputHandler.Send(
				$"That is not a valid size. See {"show sizes".Colour(Telnet.Yellow)} for a correct list.");
			return false;
		}

		MinimumCreatureSize = target;
		Changed = true;
		if (MaximumCreatureSize < MinimumCreatureSize)
		{
			MaximumCreatureSize = MinimumCreatureSize;
		}

		actor.Send(
			$"This restraint will now bind creatures of {(MinimumCreatureSize == MaximumCreatureSize ? $"size {MinimumCreatureSize.Describe().Colour(Telnet.Green)}" : $"sizes between {MinimumCreatureSize.Describe().Colour(Telnet.Green)} and {MaximumCreatureSize.Describe().Colour(Telnet.Green)}")}.");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item can be used to restrain the {4} of creatures {5}. It is {6} to break out of via feats of agility, and {7} to break out via feats of strength.",
			"Restraint Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			LimbTypes.Select(x => x.DescribePlural()).ListToString(),
			MinimumCreatureSize == MaximumCreatureSize
				? $"of size {MinimumCreatureSize.Describe().Colour(Telnet.Green)}"
				: $"of size between {MinimumCreatureSize.Describe().Colour(Telnet.Green)} and {MaximumCreatureSize.Describe().Colour(Telnet.Green)}",
			BreakoutDifficulty.Describe().Colour(Telnet.Green),
			OverpowerDifficulty.Describe().Colour(Telnet.Green)
		);
	}
}