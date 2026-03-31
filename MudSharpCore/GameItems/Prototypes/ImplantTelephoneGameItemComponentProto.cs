#nullable enable
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class ImplantTelephoneGameItemComponentProto : ImplantBaseGameItemComponentProto
{
	public override string TypeDescription => "ImplantTelephone";
	public string RingText { get; set; }

	#region Constructors

	protected ImplantTelephoneGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "ImplantTelephone")
	{
		RingText = "A shrill internal ringtone sounds through your neural interface.";
	}

	protected ImplantTelephoneGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		RingText = root.Element("RingText")?.Value ?? "A shrill internal ringtone sounds through your neural interface.";
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return SaveToXmlWithoutConvertingToString(
			new XElement("RingText", new XCData(RingText))
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ImplantTelephoneGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
	{
		return new ImplantTelephoneGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public new static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("ImplantTelephone".ToLowerInvariant(), true,
			(gameworld, account) => new ImplantTelephoneGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("Implant Telephone".ToLowerInvariant(), false,
			(gameworld, account) => new ImplantTelephoneGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("TelephoneImplant".ToLowerInvariant(), false,
			(gameworld, account) => new ImplantTelephoneGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ImplantTelephone",
			(proto, gameworld) => new ImplantTelephoneGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"ImplantTelephone",
			$"An {"[implant]".Colour(Telnet.Pink)} cellular telephone that is controlled through a {"[neural interface]".Colour(Telnet.Pink)}",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ImplantTelephoneGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tbody <body> - sets the body prototype this implant is used with\n\tbodypart <bodypart> - sets the bodypart prototype this implant is used with\n\texternal - toggles whether this implant is external\n\texternaldesc <desc> - an alternate sdesc used when installed and external\n\tpower <watts> - how many watts of power to use\n\tdiscount <watts> - how many watts of power usage to discount per point of quality\n\tgrace <percentage> - the grace percentage of hp damage before implant function reduces\n\tspace <#> - the amount of 'space' in a bodypart that the implant takes up\n\tdifficulty <difficulty> - how difficult it is for surgeons to install this implant\n\tring <text> - sets the text shown through the neural interface when an incoming call arrives";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "ring":
			case "ringtext":
			case "incoming":
				return BuildingCommandRing(actor, command);
			default:
				return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"));
		}
	}

	private bool BuildingCommandRing(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What text should this implant telephone show when an incoming call arrives?");
			return false;
		}

		RingText = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"This implant telephone will now use the following incoming call text:\n\n{RingText}");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return ComponentDescriptionOLC(actor, "This is an implant telephone",
			$"It connects to the cellular network when a cell tower serves the user's zone. Incoming calls show the following internal alert:\n{RingText.ColourCommand()}");
	}
}
