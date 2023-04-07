using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Prototypes;

public class ImplantBaseGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "ImplantBase";

	#region Constructors

	protected ImplantBaseGameItemComponentProto(IFuturemud gameworld, IAccount originator, string type = "ImplantBase")
		: base(gameworld, originator, type)
	{
		External = true;
		ExternalDescription = "a small portion of a high-tech device";
		TargetBody = Gameworld.BodyPrototypes.Get(Gameworld.GetStaticLong("DefaultBodyForImplants"));
		PowerConsumptionInWatts = 20;
		PowerConsumptionDiscountPerQuality = 1;
		InstallDifficulty = (Difficulty)Gameworld.GetStaticInt("DefaultImplantInstallDifficulty");
		ImplantSpaceOccupied = 1.0;
		ImplantDamageFunctionGrace = Gameworld.GetStaticDouble("DefaultImplantDamageGrace");
	}

	protected ImplantBaseGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		External = bool.Parse(root.Element("External")?.Value ?? "false");
		ExternalDescription = root.Element("ExternalDescription")?.Value ?? "";
		PowerConsumptionInWatts = double.Parse(root.Element("PowerConsumptionInWatts")?.Value ?? "20");
		PowerConsumptionDiscountPerQuality =
			double.Parse(root.Element("PowerConsumptionDiscountPerQuality")?.Value ?? "1");
		TargetBody = Gameworld.BodyPrototypes.Get(long.Parse(root.Element("TargetBody")?.Value ?? "0"));
		var tbpid = long.Parse(root.Element("TargetBodypart")?.Value ?? "0");
		TargetBodypart = TargetBody?.AllBodypartsBonesAndOrgans.FirstOrDefault(x => x.Id == tbpid);
		ImplantSpaceOccupied = double.Parse(root.Element("ImplantSpaceOccupied")?.Value ?? "0.0");
		InstallDifficulty = (Difficulty)int.Parse(root.Element("InstallDifficulty")?.Value ?? "0");
		ImplantDamageFunctionGrace = double.Parse(root.Element("ImplantDamageFunctionGrace")?.Value ??
		                                          Gameworld.GetStaticConfiguration("DefaultImplantDamageGrace"));
	}

	#endregion

	#region Saving

	protected XElement SaveToXmlWithoutConvertingToString(params object[] content)
	{
		return new XElement("Definition",
			new XElement("External", External),
			new XElement("ExternalDescription", new XCData(ExternalDescription ?? "")),
			new XElement("PowerConsumptionInWatts", PowerConsumptionInWatts),
			new XElement("PowerConsumptionDiscountPerQuality", PowerConsumptionDiscountPerQuality),
			new XElement("TargetBody", TargetBody?.Id ?? 0),
			new XElement("TargetBodypart", TargetBodypart?.Id ?? 0),
			new XElement("ImplantSpaceOccupied", ImplantSpaceOccupied),
			new XElement("InstallDifficulty", (int)InstallDifficulty),
			new XElement("ImplantDamageFunctionGrace", ImplantDamageFunctionGrace),
			content
		);
	}

	protected override string SaveToXml()
	{
		return SaveToXmlWithoutConvertingToString().ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ImplantBaseGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ImplantBaseGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("implant", true,
			(gameworld, account) => new ImplantBaseGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ImplantBase",
			(proto, gameworld) => new ImplantBaseGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Implant",
			$"A basic {"[implant]".Colour(Telnet.Pink)} that simply draws power and takes up space but has no direct function",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ImplantBaseGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText = @"You can use the following options with this component:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3body <body>#0 - sets the body prototype this implant is used with
	#3bodypart <bodypart>#0 - sets the bodypart prototype this implant is used with
	#3external#0 - toggles whether this implant is external
	#3externaldesc <desc>#0 - an alternate sdesc used when installed and external
	#3power <watts>#0 - how many watts of power to use
	#3discount <watts>#0 - how many watts of power usage to discount per point of quality
	#3grace <percentage>#0 - the grace percentage of hp damage before implant function reduces
	#3space <#>#0 - the amount of 'space' in a bodypart that the implant takes up
	#3difficulty <difficulty>#0 - how difficult it is for surgeons to install this implant";

	public override string ShowBuildingHelp => BuildingHelpText;

	#region Overrides of EditableItem

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

	#endregion

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
			case "power":
			case "wattage":
			case "watt":
			case "watts":
				return BuildingCommandWattage(actor, command);
			case "discount":
				return BuildingCommandDiscount(actor, command);
			case "space":
				return BuildingCommandSpace(actor, command);
			case "difficulty":
			case "install":
				return BuildingCommandDifficulty(actor, command);
			case "grace":
				return BuildingCommandGrace(actor, command);
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

	private bool BuildingCommandWattage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What wattage do you want this implant to consume? As a point of reference, an actual human heart consumes about 20 Watts of energy.");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.Send("You must enter a valid number of watts for this implant to consume.");
			return false;
		}

		PowerConsumptionInWatts = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This implant will now use {PowerConsumptionInWatts.ToString("N2", actor).ColourValue()} watts of power.");
		return true;
	}

	private bool BuildingCommandDiscount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many less watts should this implant use per point of quality (5 being normal quality)?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.Send("You must enter a valid number of watts per point of quality to discount.");
			return false;
		}

		PowerConsumptionDiscountPerQuality = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This implant will now consume {PowerConsumptionDiscountPerQuality.ToString("N2", actor).ColourValue()} watts less per point of quality.");
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
		var partname = command.SafeRemainingArgument;
		var bodypart = long.TryParse(partname, out var value)
			? parts.FirstOrDefault(x => x.Id == value)
			: parts.FirstOrDefault(x => x.FullDescription().EqualTo(partname)) ??
			  parts.FirstOrDefault(x => x.Name.EqualTo(partname));
		if (bodypart == null)
		{
			actor.Send("There is no bodypart like that for that body type.");
			return false;
		}

		TargetBodypart = bodypart;
		Changed = true;
		actor.Send(
			$"This implant is now designed for the {TargetBodypart.FullDescription().ColourCommand()} bodypart.");
		return true;
	}

	private bool BuildingCommandBody(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which body prototype is this implant for?");
			return false;
		}

		var body = long.TryParse(command.RemainingArgument, out var value)
			? Gameworld.BodyPrototypes.Get(value)
			: Gameworld.BodyPrototypes.GetByName(command.RemainingArgument);
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

		ExternalDescription = command.SafeRemainingArgument;
		Changed = true;
		actor.Send(
			$"This implant will no have the following description when installed and external:\n\n\t{ExternalDescription.ColourObject()}");
		return true;
	}

	private bool BuildingCommandExternal(ICharacter actor, StringStack command)
	{
		External = !External;
		actor.Send($"This implant is {(External ? "now" : "no longer")} external.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandGrace(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What percentage of total damage should penalties to ImplantFunction kick in with this implant?");
			return false;
		}

		if (!command.PopSpeech().TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send("You must enter a percentage.");
			return false;
		}

		if (value < 0.0 || value > 1.0)
		{
			actor.OutputHandler.Send(
				"You must enter a value between 0% (no grace) and 100% (disables function loss from damage)");
			return false;
		}

		ImplantDamageFunctionGrace = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"There will now be a grace period of {ImplantDamageFunctionGrace.ToString("P2", actor).ColourValue()} before implant function loss kicks in.");
		return true;
	}

	#endregion

	protected string ComponentDescriptionOLC(ICharacter actor, string firstline, string addenda)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\n{9}. It is designed for the {4} body. It is installed in the {5} and {6}. It uses {7} watts of power, minus {8} watts per point of quality. It has a grace period of {13} total damage before it loses any functionality. It is {11} to install and takes up {12} space.{10}",
			"ImplantOrgan Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			TargetBody?.Name ?? "Unknown",
			TargetBodypart?.FullDescription() ?? "Unknown",
			External
				? $"shows up externally even when installed, and looks like {ExternalDescription.Colour(Telnet.Yellow)}."
				: "does not show up externally",
			PowerConsumptionInWatts,
			PowerConsumptionDiscountPerQuality,
			firstline,
			addenda,
			InstallDifficulty.Describe().Colour(Telnet.Green),
			ImplantSpaceOccupied.ToString("N2", actor).Colour(Telnet.Green),
			ImplantDamageFunctionGrace.ToString("P2", actor).Colour(Telnet.Green)
		);
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return ComponentDescriptionOLC(actor, "This is a prop implant with no functionality", string.Empty);
	}

	public bool External { get; protected set; }
	public string ExternalDescription { get; protected set; }

	public IBodyPrototype TargetBody { get; protected set; }

	public IBodypart TargetBodypart { get; protected set; }

	public double PowerConsumptionInWatts { get; protected set; }

	public double PowerConsumptionDiscountPerQuality { get; protected set; }

	public double ImplantSpaceOccupied { get; protected set; }

	public Difficulty InstallDifficulty { get; protected set; }

	/// <summary>
	/// A percentage amount deducted from 1.0 of total health damage to the implant before it starts losing FunctionFactor
	/// </summary>
	public double ImplantDamageFunctionGrace { get; protected set; }
}