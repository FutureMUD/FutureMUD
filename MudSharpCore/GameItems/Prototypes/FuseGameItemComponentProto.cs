using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class FuseGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "Fuse";

	#region Constructors

	protected FuseGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"Fuse")
	{
		BurnTime = TimeSpan.FromSeconds(10);
		Extinguishable = true;
		IgniteEmote = "@ ignite|ignites the fuse on $1, and it begins to burn down.";
		ExtinguishEmote = "@ extinguish|extinguishes the fuse on $1, which goes out with a sizzle.";
		IgnitedTagAddendum = "#9(lit)#0";
	}

	protected FuseGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
		proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		BurnTime = TimeSpan.FromSeconds(double.Parse(root.Element("BurnTime").Value));
		Extinguishable = bool.Parse(root.Element("Extinguishable").Value);
		IgniteEmote = root.Element("IgniteEmote").Value;
		ExtinguishEmote = root.Element("ExtinguishEmote").Value;
		IgnitedTagAddendum = root.Element("IgnitedTagAddendum").Value;
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("BurnTime", BurnTime.TotalSeconds),
			new XElement("Extinguishable", Extinguishable),
			new XElement("IgniteEmote", new XCData(IgniteEmote)),
			new XElement("ExtinguishEmote", new XCData(ExtinguishEmote)),
			new XElement("IgnitedTagAddendum", new XCData(IgnitedTagAddendum))
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new FuseGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new FuseGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Fuse".ToLowerInvariant(), true,
			(gameworld, account) => new FuseGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Fuse", (proto, gameworld) => new FuseGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Fuse",
			$"Makes an item {"[lightable]".Colour(Telnet.Red)} with the {"[trigger]".Colour(Telnet.Yellow)} effect when burnt down",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new FuseGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tburn <timespan> - the amount of time for the fuse to burn\n\textinguishable - toggles whether the lit fuse can be put out\n\tingnite <emote> - the emote for igniting this fuse\n\textinguish <emote> - the emote for extinguishing this fuse\n\tlit <tag> - the tag to put after the sdesc when lit";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "burn":
			case "time":
			case "timer":
			case "burntime":
			case "burn time":
			case "burn_time":
			case "burn timer":
			case "burn_timer":
				return BuildingCommandBurnTime(actor, command);
			case "extinguishable":
			case "canextinguish":
			case "cancel":
			case "cancellable":
			case "cancelable":
			case "can_extinguish":
			case "can extinguish":
				return BuildingCommandExtinguishable(actor, command);
			case "ignite":
			case "emote":
			case "ignition":
			case "ignite emote":
			case "igniteemote":
			case "ignite_emote":
			case "ignition emote":
			case "ignition_emote":
				return BuildingCommandIgnite(actor, command);
			case "extinguish":
			case "extinguishemote":
			case "extinguish emote":
			case "extinguish_emote":
			case "cancelemote":
			case "cancel emote":
			case "cancel_emote":
				return BuildingCommandExtinguish(actor, command);
			case "tag":
			case "lit":
			case "lit tag":
			case "littag":
			case "lit_tag":
				return BuildingCommandTag(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must either specify a lit tag, or use the keyword none to have no indication.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			IgnitedTagAddendum = string.Empty;
			Changed = true;
			actor.OutputHandler.Send(
				"This fuse will no longer display any indication in its short description that it is lit.");
			return true;
		}

		IgnitedTagAddendum = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send(
			$"This fuse will now display the following short description addendum when lit: {IgnitedTagAddendum.SubstituteANSIColour()}");
		return true;
	}

	private bool BuildingCommandIgnite(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What do you want to set the ignite emote to for this fuse? Use $0 for the igniter and $1 for the item with the fuse.");
			return false;
		}

		var dummy = new Emote(command.RemainingArgument, new DummyPerceiver(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!dummy.Valid)
		{
			actor.OutputHandler.Send(dummy.ErrorMessage);
			return false;
		}

		IgniteEmote = command.RemainingArgument.Fullstop();
		Changed = true;
		actor.OutputHandler.Send($"The ignition emote for this fuse is now: {IgniteEmote.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandExtinguish(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What do you want to set the extinguish emote to for this fuse? Use $0 for the extinguisher and $1 for the item with the fuse.");
			return false;
		}

		var dummy = new Emote(command.RemainingArgument, new DummyPerceiver(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!dummy.Valid)
		{
			actor.OutputHandler.Send(dummy.ErrorMessage);
			return false;
		}

		ExtinguishEmote = command.RemainingArgument.Fullstop();
		Changed = true;
		actor.OutputHandler.Send($"The extinguish emote for this fuse is now: {ExtinguishEmote.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandExtinguishable(ICharacter actor, StringStack command)
	{
		Extinguishable = !Extinguishable;
		Changed = true;
		actor.OutputHandler.Send($"This fuse is {(Extinguishable ? "now" : "no longer")} extinguishable once lit.");
		return true;
	}

	private bool BuildingCommandBurnTime(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a burn time for the fuse.");
			return false;
		}

		if (!TimeSpan.TryParse(command.SafeRemainingArgument, actor, out var value))
		{
			actor.OutputHandler.Send("That is not a valid timespan.");
			return false;
		}

		if (value <= TimeSpan.Zero)
		{
			actor.OutputHandler.Send("You must enter a timespan greater than zero.");
			return false;
		}

		BurnTime = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This fuse will now burn for a total of {BurnTime.Describe(actor).Colour(Telnet.Green)} before detonation.");
		return true;
	}

	#endregion

	public TimeSpan BurnTime { get; protected set; }
	public bool Extinguishable { get; protected set; }
	public string IgniteEmote { get; protected set; }
	public string ExtinguishEmote { get; protected set; }
	public string IgnitedTagAddendum { get; protected set; }

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item is a fuse for a bomb. It burns for {4} before detonation, and {5} be extinguished.\n\nIgnite Emote: {6}\nExtinguish Emote: {7}",
			"Fuse Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			BurnTime.Describe(actor).Colour(Telnet.Green),
			Extinguishable ? "can" : "cannot",
			IgniteEmote.ColourCommand(),
			ExtinguishEmote.ColourCommand()
		);
	}
}