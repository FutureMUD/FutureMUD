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

public class ImplantPowerPlantGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "ImplantPowerPlant";

	#region Constructors

	protected ImplantPowerPlantGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "ImplantPowerPlant")
	{
		BaseEfficiencyMultiplier = 1.3;
		EfficiencyGainPerQuality = 0.05;
		External = true;
		ExternalDescription = "a small, polished chrome panel with hinges";
		TargetBody = Gameworld.BodyPrototypes.Get(Gameworld.GetStaticLong("DefaultBodyForImplants"));
		TargetBodypart =
			TargetBody?.AllBodypartsBonesAndOrgans.FirstOrDefault(x =>
				x.Id == Gameworld.GetStaticLong("DefaultBodypartForImplants"));
		InstallDifficulty = (Difficulty)Gameworld.GetStaticInt("DefaultImplantInstallDifficulty");
		ImplantSpaceOccupied = 1.0;
	}

	protected ImplantPowerPlantGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		BaseEfficiencyMultiplier = double.Parse(root.Element("BaseEfficiencyMultiplier")?.Value ?? "1.3");
		EfficiencyGainPerQuality = double.Parse(root.Element("EfficiencyGainPerQuality")?.Value ?? "0.05");
		External = bool.Parse(root.Element("External")?.Value ?? "true");
		ExternalDescription =
			root.Element("ExternalDescription")?.Value ?? "a small, polished chrome panel with hinges";
		TargetBody = Gameworld.BodyPrototypes.Get(long.Parse(root.Element("TargetBody")?.Value ?? "0"));
		var tbpid = long.Parse(root.Element("TargetBodypart")?.Value ?? "0");
		TargetBodypart = TargetBody?.AllBodypartsBonesAndOrgans.FirstOrDefault(x => x.Id == tbpid);
		ImplantSpaceOccupied = double.Parse(root.Element("ImplantSpaceOccupied")?.Value ?? "0.0");
		InstallDifficulty = (Difficulty)int.Parse(root.Element("InstallDifficulty")?.Value ?? "0");
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("BaseEfficiencyMultiplier", BaseEfficiencyMultiplier),
			new XElement("EfficiencyGainPerQuality", EfficiencyGainPerQuality),
			new XElement("External", External),
			new XElement("ExternalDescription", new XCData(ExternalDescription ?? "")),
			new XElement("TargetBody", TargetBody?.Id ?? 0),
			new XElement("ImplantSpaceOccupied", ImplantSpaceOccupied),
			new XElement("InstallDifficulty", (int)InstallDifficulty),
			new XElement("TargetBodypart", TargetBodypart?.Id ?? 0)).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ImplantPowerPlantGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ImplantPowerPlantGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("ImplantPowerPlant".ToLowerInvariant(), true,
			(gameworld, account) => new ImplantPowerPlantGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ImplantPowerPlant",
			(proto, gameworld) => new ImplantPowerPlantGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"ImplantPowerPlant",
			$"An {"[implant]".Colour(Telnet.Pink)} that if {"[powered]".Colour(Telnet.Magenta)} will itself {"[provide power]".Colour(Telnet.BoldMagenta)} for other implants in the body",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ImplantPowerPlantGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tbody <body> - sets the body prototype this implant is used with\n\tbodypart <bodypart> - sets the bodypart prototype this implant is used with\n\texternal - toggles whether this implant is external\n\texternaldesc <desc> - an alternate sdesc used when installed and external\n\tspace <#> - the amount of 'space' in a bodypart that the implant takes up\n\tdifficulty <difficulty> - how difficulty it is for surgeons to install this implant\n\tefficiency <%> - the efficiency with which power supplied is drawn down\n\tquality <%> - an efficiency improvement per point of item quality";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "external":
				return BuildingCommandExternal(actor, command);
			case "externaldesc":
			case "externaldescription":
				return BuildingCommandExternalDescription(actor, command);
			case "body":
				return BuildingCommandBody(actor, command);
			case "bodypart":
				return BuildingCommandBodypart(actor, command);
			case "efficiency":
				return BuildingCommandEfficiency(actor, command);
			case "quality":
				return BuildingCommandQuality(actor, command);
			case "space":
				return BuildingCommandSpace(actor, command);
			case "difficulty":
			case "install":
				return BuildingCommandDifficulty(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandSpace(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.PopSpeech(), out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("You must enter a valid amount of 'space' for this implant to take up.");
			return false;
		}

		ImplantSpaceOccupied = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This implant now occupies {ImplantSpaceOccupied.ToString("N2", actor).Colour(Telnet.Green)} space in its target bodypart.");
		return true;
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How difficult should it be for surgeons to install this implant?");
			return false;
		}

		if (!CheckExtensions.GetDifficulty(command.PopSpeech(), out var difficulty))
		{
			actor.OutputHandler.Send("That is not a valid difficulty.");
			return false;
		}

		InstallDifficulty = difficulty;
		Changed = true;
		actor.OutputHandler.Send(
			$"It will now be {InstallDifficulty.Describe().Colour(Telnet.Green)} for surgeons to install this implant.");
		return true;
	}

	private bool BuildingCommandQuality(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What should be the efficiency improvement per point of quality of this power plant in providing power?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a number for the efficiency improvement. Higher values means more efficient.");
			return false;
		}

		EfficiencyGainPerQuality = value;
		Changed = true;
		actor.Send(
			$"The efficiency improvement per point of quality of this power plant is now {value.ToString("N3", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandEfficiency(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What should be the base efficiency of this power plant in providing power?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a number for the efficiency multiplier. Higher values means less efficient.");
			return false;
		}

		BaseEfficiencyMultiplier = value;
		Changed = true;
		actor.Send(
			$"The base effiency of this power plant (before quality is factored in) is now {value.ToString("N3", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandBodypart(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which bodypart should this implant be installed into?");
			return false;
		}

		if (TargetBody == null)
		{
			actor.Send("You must first set a body for this implant.");
			return false;
		}

		var parts = TargetBody.AllBodypartsBonesAndOrgans.ToList();
		var partName = command.SafeRemainingArgument;
		var bodypart = long.TryParse(partName, out var value)
			? parts.FirstOrDefault(x => x.Id == value)
			: parts.FirstOrDefault(x => x.FullDescription().EqualTo(partName)) ??
			  parts.FirstOrDefault(x => x.Name.EqualTo(partName));
		if (bodypart == null)
		{
			actor.Send("There is no bodypart like that for that body type.");
			return false;
		}

		TargetBodypart = bodypart;
		Changed = true;
		actor.Send($"This implant is now designed for the {TargetBodypart.FullDescription()} bodypart.");
		return true;
	}

	private bool BuildingCommandBody(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which body prototype is this implant for?");
			return false;
		}

		var body = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.BodyPrototypes.Get(value)
			: Gameworld.BodyPrototypes.GetByName(command.SafeRemainingArgument);
		if (body == null)
		{
			actor.Send("There is no body prototype like that.");
			return false;
		}

		if (body == TargetBody)
		{
			actor.Send("This implant is already designed for that body.");
			return false;
		}

		TargetBody = body;
		TargetBodypart = null;
		actor.Send($"This implant is now designed for the {body.Name.Colour(Telnet.Green)} body (#{body.Id})");
		Changed = true;
		return true;
	}

	private bool BuildingCommandExternalDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What description did you want to set for this implant when installed and external?");
			return false;
		}

		ExternalDescription = command.RemainingArgument;
		Changed = true;
		actor.Send(
			$"This implant will now have the following description added to their look description when installed and external:\n\n\t{ExternalDescription.ColourObject()}");
		return true;
	}

	private bool BuildingCommandExternal(ICharacter actor, StringStack command)
	{
		External = !External;
		actor.Send($"This implant is {(External ? "now" : "no longer")} external.");
		Changed = true;
		return true;
	}

	#endregion

	public double BaseEfficiencyMultiplier { get; protected set; }

	public double EfficiencyGainPerQuality { get; protected set; }

	public bool External { get; protected set; }
	public string ExternalDescription { get; protected set; }

	public IBodyPrototype TargetBody { get; protected set; }

	public IBodypart TargetBodypart { get; protected set; }

	public double ImplantSpaceOccupied { get; protected set; }

	public Difficulty InstallDifficulty { get; protected set; }

	public override bool CanSubmit()
	{
		if (TargetBody == null)
		{
			return false;
		}

		if (External && string.IsNullOrEmpty(ExternalDescription))
		{
			return false;
		}

		return base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (TargetBody == null)
		{
			return "You must first set a body.";
		}

		if (External && string.IsNullOrEmpty(ExternalDescription))
		{
			return "If the implant is to be external, it must have an external description.";
		}

		return base.WhyCannotSubmit();
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(
			actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item powers other implants when it is itself powered. It has a base efficiency of [{4:N3} - quality * {5:N3}]. It is installed in the {6} body type in bodypart {7}. It is {9} for surgeons to implant and takes up {10} space. It {8}.",
			"ImplantPowerPlant Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			BaseEfficiencyMultiplier,
			EfficiencyGainPerQuality,
			TargetBody?.Name ?? "Unknown",
			TargetBodypart?.FullDescription() ?? "Unknown",
			External
				? $"shows up externally even when installed, and looks like {ExternalDescription.Colour(Telnet.Yellow)}."
				: "does not show up externally",
			InstallDifficulty.Describe().Colour(Telnet.Green),
			ImplantSpaceOccupied.ToString("N2", actor).Colour(Telnet.Green)
		);
	}
}