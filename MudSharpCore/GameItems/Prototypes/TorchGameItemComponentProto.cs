using System;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class TorchGameItemComponentProto : GameItemComponentProto
{
	protected TorchGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Torch")
	{
		IlluminationProvided = 25;
		SecondsOfFuel = 3600;
		RequiresIgnitionSource = false;
		LightEmote = "@ light|lights $1";
		ExtinguishEmote = "@ extinguish|extinguishes $1";
		TenPercentFuelEcho = "$0 begin|begins to splutter";
		FuelExpendedEcho = "$0 have|has completely burned out";
		Changed = true;
	}

	protected TorchGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public double IlluminationProvided { get; protected set; }
	public int SecondsOfFuel { get; protected set; }
	public string LightEmote { get; protected set; }
	public string ExtinguishEmote { get; protected set; }
	public bool RequiresIgnitionSource { get; protected set; }
	public string TenPercentFuelEcho { get; protected set; }
	public string FuelExpendedEcho { get; protected set; }
	public override string TypeDescription => "Torch";

	protected override void LoadFromXml(XElement root)
	{
		IlluminationProvided = double.Parse(root.Element("IlluminationProvided").Value);
		SecondsOfFuel = int.Parse(root.Element("SecondsOfFuel").Value);
		RequiresIgnitionSource = bool.Parse(root.Element("RequiresIgnitionSource").Value);
		LightEmote = root.Element("LightEmote").Value;
		ExtinguishEmote = root.Element("ExtinguishEmote").Value;
		TenPercentFuelEcho = root.Element("TenPercentFuelEcho").Value;
		FuelExpendedEcho = root.Element("FuelExpendedEcho").Value;
	}

	private bool BuildingCommand_Fuel(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many seconds of fuel should this torch have? You can use -1 for unlimited.");
			return false;
		}

		if (!int.TryParse(command.Pop(), out var value))
		{
			actor.Send("How many seconds of fuel should this torch have? You can use -1 for unlimited.");
			return false;
		}

		if (value < 1 && value != -1)
		{
			actor.Send(
				"You must enter a positive number of seconds of fuel for this torch, or use -1 for unlimited.");
			return false;
		}

		SecondsOfFuel = value;
		Changed = true;
		if (SecondsOfFuel == -1)
		{
			actor.Send("This torch will now burn forever.");
		}
		else
		{
			actor.Send("This torch will now have enough fuel to burn for {0:N0} seconds ({1}).", SecondsOfFuel,
				TimeSpan.FromSeconds(SecondsOfFuel).Describe());
		}

		return true;
	}

	private bool BuildingCommand_Illumination(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many lux of illumination should this torch provide when lit?");
			return false;
		}

		if (!double.TryParse(command.Pop(), out var value))
		{
			actor.Send("How many lux of illumination should this torch provide when lit?");
			return false;
		}

		if (value < 1)
		{
			actor.Send("Torches must provide a positive amount of illumination.");
			return false;
		}

		IlluminationProvided = value;
		Changed = true;
		actor.Send("This torch will now provide {0:N2} lux of illumination when lit.", IlluminationProvided);
		return true;
	}

	private bool BuildingCommand_Ignition(ICharacter actor, StringStack command)
	{
		actor.Send(RequiresIgnitionSource
			? "This torch no longer requires an independent ignition source to be lit."
			: "This torch now requires an independent ignition source to be lit.");
		RequiresIgnitionSource = !RequiresIgnitionSource;
		Changed = true;
		return true;
	}

	private bool BuildingCommand_LightEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What emote do you want to set for the lighting of this torch? Use $0 for the lightee, $1 for the torch, and $2 for the ignition source.");
			return false;
		}

		// This line is essentially just leveraging Emote.ScoutTargets to make sure they didn't make a bad emote
		var emote = new Emote(command.RemainingArgument, actor, actor, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		actor.Send("The emote for lighting this torch is now \"{0}\"", command.RemainingArgument);
		LightEmote = command.RemainingArgument;
		Changed = true;
		return true;
	}

	private bool BuildingCommand_ExtinguishEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What emote do you want to set for the extinguishing of this torch? Use $0 for the lightee and $1 for the torch.");
			return false;
		}

		// This line is essentially just leveraging Emote.ScoutTargets to make sure they didn't make a bad emote
		var emote = new Emote(command.RemainingArgument, actor, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		actor.Send("The emote for extinguishing this torch is now \"{0}\"", command.RemainingArgument);
		ExtinguishEmote = command.RemainingArgument;
		Changed = true;
		return true;
	}

	private bool BuildingCommand_TenPercent(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What emote do you want to set for ten percent fuel being reached with this torch? Use $0 for the torch.");
			return false;
		}

		// This line is essentially just leveraging Emote.ScoutTargets to make sure they didn't make a bad emote
		var emote = new Emote(command.RemainingArgument, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		actor.Send("The emote for ten percent fuel for this torch is now \"{0}\"", command.RemainingArgument);
		TenPercentFuelEcho = command.RemainingArgument;
		Changed = true;
		return true;
	}

	private bool BuildingCommand_FuelExpended(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for fuel exhaustion with this torch? Use $0 for the torch.");
			return false;
		}

		// This line is essentially just leveraging Emote.ScoutTargets to make sure they didn't make a bad emote
		var emote = new Emote(command.RemainingArgument, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		actor.Send("The emote for fuel exhaustion for this torch is now \"{0}\"", command.RemainingArgument);
		FuelExpendedEcho = command.RemainingArgument;
		Changed = true;
		return true;
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Pop().ToLowerInvariant())
		{
			case "fuel":
				return BuildingCommand_Fuel(actor, command);
			case "lux":
			case "illumination":
				return BuildingCommand_Illumination(actor, command);
			case "ignition":
				return BuildingCommand_Ignition(actor, command);
			case "lit":
			case "light":
				return BuildingCommand_LightEmote(actor, command);
			case "extinguish":
			case "extinguished":
				return BuildingCommand_ExtinguishEmote(actor, command);
			case "tenpercent":
			case "ten":
				return BuildingCommand_TenPercent(actor, command);
			case "expended":
			case "zeropercent":
			case "zero":
				return BuildingCommand_FuelExpended(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tfuel <seconds> - how many seconds of fuel this torch has once lit\n\tillumination <lux> - the illumination in lux provided by the torch\n\tignition - toggles whether this requires an ignition source to light\n\tlit <emote> - the emote when this torch is lit. Use $0 for the lightee, $1 for the torch, and $2 for the ignition source (if applicable)\n\textinguished <emote> - sets an emote for when the torch is extinguished. $0 for the lightee, $1 for the torch\n\tten <emote> - sets an emote when the torch reaches 10% fuel. Use $0 for the torch item.\n\tzero <emote> - sets an emote when the torch expires. Use $0 for the torch item.";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{7} (#{8:N0}r{9:N0}, {10})\n\nThis is a torch that provides {0:N2} lux of illumination when lit. It has {1:N0} seconds of fuel, and {2} require an ignition source.\n\nWhen lit, it echoes: {3}\nWhen extinguished it echoes: {4}\nAt 10 percent fuel it echoes: {5}\nAt 0 percent fuel it echoes: {6}",
			IlluminationProvided,
			SecondsOfFuel,
			RequiresIgnitionSource ? "does" : "does not",
			LightEmote,
			ExtinguishEmote,
			TenPercentFuelEcho,
			FuelExpendedEcho,
			"Torch Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name
		);
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition", new XElement("IlluminationProvided", IlluminationProvided),
				new XElement("SecondsOfFuel", SecondsOfFuel),
				new XElement("RequiresIgnitionSource", RequiresIgnitionSource),
				new XElement("LightEmote", new XCData(LightEmote)),
				new XElement("ExtinguishEmote", new XCData(ExtinguishEmote)),
				new XElement("TenPercentFuelEcho", new XCData(TenPercentFuelEcho)),
				new XElement("FuelExpendedEcho", new XCData(FuelExpendedEcho))).ToString();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("torch", true,
			(gameworld, account) => new TorchGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Torch", (proto, gameworld) => new TorchGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Torch",
			$"A {"[light source]".Colour(Telnet.BoldPink)} that can burn for some time once {"[lit]".Colour(Telnet.Red)}",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return SecondsOfFuel == -1
			? new BottomlessTorchGameItemComponent(this, parent, temporary)
			: new TorchGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return SecondsOfFuel == -1
			? new BottomlessTorchGameItemComponent(component, this, parent)
			: new TorchGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new TorchGameItemComponentProto(proto, gameworld));
	}
}