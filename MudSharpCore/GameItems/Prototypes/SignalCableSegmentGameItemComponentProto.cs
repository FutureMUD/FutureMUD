#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class SignalCableSegmentGameItemComponentProto : GameItemComponentProto
{
	private const string BuildingHelpText = @"You can use the following options with this component:
	name <name> - sets the name of the component
	desc <desc> - sets the description of the component

Signal cable segments are routed at runtime with the electrical command and mirror a source from an adjacent room.";

	public SignalCableSegmentGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Signal Cable Segment")
	{
	}

	protected SignalCableSegmentGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "Signal Cable Segment";

	protected override void LoadFromXml(System.Xml.Linq.XElement root)
	{
	}

	protected override string SaveToXml()
	{
		return new System.Xml.Linq.XElement("Definition").ToString();
	}

	public override string ShowBuildingHelp => BuildingHelpText;

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Signal Cable Segment Game Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\nThis component makes an item act as a one-hop adjacent-room signal cable. Runtime routing chooses the source endpoint and exit path.";
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("signalcable", true,
			(gameworld, account) => new SignalCableSegmentGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("signal cable", false,
			(gameworld, account) => new SignalCableSegmentGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("cablesegment", false,
			(gameworld, account) => new SignalCableSegmentGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Signal Cable Segment",
			(proto, gameworld) => new SignalCableSegmentGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Signal Cable Segment",
			$"Makes an item a one-hop {"[signal cable]".Colour(Telnet.BoldGreen)} that mirrors an adjacent-room source",
			BuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new SignalCableSegmentGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new SignalCableSegmentGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new SignalCableSegmentGameItemComponentProto(proto, gameworld));
	}
}
