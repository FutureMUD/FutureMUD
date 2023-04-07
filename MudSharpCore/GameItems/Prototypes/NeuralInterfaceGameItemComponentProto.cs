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

public class NeuralInterfaceGameItemComponentProto : ImplantBaseGameItemComponentProto
{
	public override string TypeDescription => "NeuralInterface";

	#region Constructors

	protected NeuralInterfaceGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "NeuralInterface")
	{
	}

	protected NeuralInterfaceGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		PermitsAudio = bool.Parse(root.Element("PermitsAudio").Value);
		PermitsVisual = bool.Parse(root.Element("PermitsVisual").Value);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return SaveToXmlWithoutConvertingToString(new XElement("PermitsAudio", PermitsAudio),
			new XElement("PermitsVisual", PermitsVisual)).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new NeuralInterfaceGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new NeuralInterfaceGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public new static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("NeuralInterface".ToLowerInvariant(), true,
			(gameworld, account) => new NeuralInterfaceGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("NeuralInterface",
			(proto, gameworld) => new NeuralInterfaceGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"NeuralInterface",
			$"An {"[implant]".Colour(Telnet.Pink)} that permits other implants to be {"[neurally interfaced]".Colour(Telnet.Pink)} and controlled with the mind",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new NeuralInterfaceGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tbody <body> - sets the body prototype this implant is used with\n\tbodypart <bodypart> - sets the bodypart prototype this implant is used with\n\texternal - toggles whether this implant is external\n\texternaldesc <desc> - an alternate sdesc used when installed and external\n\tpower <watts> - how many watts of power to use\n\tdiscount <watts> - how many watts of power usage to discount per point of quality\n\tgrace <percentage> - the grace percentage of hp damage before implant function reduces\n\tspace <#> - the amount of 'space' in a bodypart that the implant takes up\n\tdifficulty <difficulty> - how difficulty it is for surgeons to install this implant\n\taudio - toggles whether audio input can be seen.\n\tvisual - toggles whether visual input can be seen.";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "audio":
				return BuildingCommandAudio(actor);
			case "visual":
			case "video":
				return BuildingCommandVisual(actor);
			default:
				return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"));
		}
	}

	private bool BuildingCommandAudio(ICharacter actor)
	{
		PermitsAudio = !PermitsAudio;
		Changed = true;
		actor.OutputHandler.Send($"This item will {(PermitsAudio ? "now" : "no longer")} permit audio inputs.");
		return true;
	}

	private bool BuildingCommandVisual(ICharacter actor)
	{
		PermitsVisual = !PermitsVisual;
		Changed = true;
		actor.OutputHandler.Send($"This item will {(PermitsVisual ? "now" : "no longer")} permit visual inputs.");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return ComponentDescriptionOLC(actor, "NeuralInterface Game Item Component".Colour(Telnet.Cyan),
			$"It {(PermitsAudio ? "supports" : "does not support")} audio input and {(PermitsVisual ? "supports" : "does not support")} visual input.");
	}

	public bool PermitsAudio { get; protected set; } = true;
	public bool PermitsVisual { get; protected set; } = true;
}