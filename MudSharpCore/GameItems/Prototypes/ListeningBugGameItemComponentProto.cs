using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class ListeningBugGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "ListeningBug";

	public double BroadcastFrequency { get; set; }
	public double BroadcastRange { get; set; }
	public double ListenSkillPerQuality { get; set; }
	public double BaseListenSkill { get; set; }
	public double PowerConsumptionInWatts { get; set; }

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("BroadcastFrequency", BroadcastFrequency),
			new XElement("BroadcastRange", BroadcastRange),
			new XElement("ListenSkillPerQuality", ListenSkillPerQuality),
			new XElement("BaseListenSkill", BaseListenSkill),
			new XElement("PowerConsumptionInWatts", PowerConsumptionInWatts)
		).ToString();
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is a listening bug which transmits on the {4:N3} frequency while powered. It has a range of {5} and consumes {6} watts of power. It has a listening skill of {7:N3} + {8:N3} per quality.",
			"Listening Bug Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			BroadcastFrequency,
			Gameworld.UnitManager.Describe(BroadcastRange, UnitType.Length, actor),
			PowerConsumptionInWatts,
			BaseListenSkill,
			ListenSkillPerQuality
		);
	}

	#region Constructors

	protected ListeningBugGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "ListeningBug")
	{
		BroadcastFrequency = 375;
		BroadcastRange = 250;
		ListenSkillPerQuality = 5;
		BaseListenSkill = 35;
		PowerConsumptionInWatts = 0.002;
	}

	protected ListeningBugGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		BroadcastFrequency = double.Parse(root.Element("BroadcastFrequency").Value);
		BroadcastRange = double.Parse(root.Element("BroadcastRange").Value);
		ListenSkillPerQuality = double.Parse(root.Element("ListenSkillPerQuality").Value);
		BaseListenSkill = double.Parse(root.Element("BaseListenSkill").Value);
		PowerConsumptionInWatts = double.Parse(root.Element("PowerConsumptionInWatts").Value);
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ListeningBugGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ListeningBugGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("ListeningBug".ToLowerInvariant(), true,
			(gameworld, account) => new ListeningBugGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ListeningBug",
			(proto, gameworld) => new ListeningBugGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"ListeningBug",
			$"When {"[powered]".Colour(Telnet.Magenta)} will listen and retransmit speech in location",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ListeningBugGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tlisten <#> - the effective listening skill of this bug (otherwise may fail to hear some words)\n\tquality <skill> - bonus listen skill per point of quality\n\twatts <watts> - the power draw of this bug while on\n\trange <distance> - the distance (in KMs/Miles etc) that this bug transmits";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "wattage":
			case "watt":
			case "watts":
			case "power":
				return BuildingCommandWattage(actor, command);
			case "frequency":
				return BuildingCommandFrequency(actor, command);
			case "range":
				return BuildingCommandRange(actor, command);
			case "listen":
				return BuildingCommandListen(actor, command);
			case "quality":
				return BuildingCommandQuality(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandQuality(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What amount of listening skill should be added per point of quality?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value))
		{
			actor.Send("You must enter a valid number for the listening skill per quality of this bug.");
			return false;
		}

		if (value < 0)
		{
			actor.Send("The value for listening skill per quality must be positive.");
			return false;
		}

		ListenSkillPerQuality = value;
		Changed = true;
		actor.Send(
			$"This listening bug will now have a bonus listening skill of {ListenSkillPerQuality:N2} per point of quality.");
		return true;
	}

	private bool BuildingCommandListen(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What should the base listening skill of this bug be?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value))
		{
			actor.Send("You must enter a valid number for the listening skill of this bug.");
			return false;
		}

		if (value < 0)
		{
			actor.Send("The value for default listening skill must be positive.");
			return false;
		}

		BaseListenSkill = value;
		Changed = true;
		actor.Send($"This listening bug will now have a default listening skill of {BaseListenSkill:N2} as a base.");
		return true;
	}

	private bool BuildingCommandWattage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What wattage should this listening bug consume while in use?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must specify a valid number of watts.");
			return false;
		}

		if (value < 0.0)
		{
			actor.Send("The value must be a positive number.");
			return false;
		}

		PowerConsumptionInWatts = value;
		Changed = true;
		actor.Send($"This listening bug now uses {PowerConsumptionInWatts:N5} watts of power while in use.");
		return false;
	}

	private bool BuildingCommandRange(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What range should the transmissions from this listening bug have?");
			return false;
		}

		var value = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.Length, out var success);
		if (!success)
		{
			actor.Send("That is not a valid distance.");
			return false;
		}

		BroadcastRange = value;
		Changed = true;
		actor.Send(
			$"This listening bug will now broadcast at a range of {Gameworld.UnitManager.Describe(BroadcastRange, UnitType.Length, actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandFrequency(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What frequency should this listening bug transmit on?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a valid frequency in MHz.");
			return false;
		}

		if (value < 30 || value > 3000)
		{
			actor.Send("Frequencies must be in the range of 30-3000MHz.");
			return false;
		}

		BroadcastFrequency = value;
		Changed = true;
		actor.Send($"This listening bug will now transmit on the {BroadcastFrequency:N3}MHz frequency by default.");
		return true;
	}

	#endregion
}