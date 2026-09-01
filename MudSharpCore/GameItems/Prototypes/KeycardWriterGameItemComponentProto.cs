#nullable enable

using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class KeycardWriterGameItemComponentProto : PoweredMachineBaseGameItemComponentProto, IKeycardWriterPrototype
{
	protected KeycardWriterGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "KeycardWriter")
	{
		Wattage = 50.0;
		UseMountHostPowerSource = true;
	}

	protected KeycardWriterGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "KeycardWriter";
	protected override string ComponentDescriptionOLCByline => "This item is a powered keycard writer";
	protected override string ComponentDescriptionOLCAddendum(ICharacter actor) =>
		$"It is used with {"electrical <writer> writecard ...".ColourCommand()}.";
	protected override XElement SaveSubtypeToXml(XElement root) => root;

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("keycardwriter", true,
			(gameworld, account) => new KeycardWriterGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("KeycardWriter",
			(proto, gameworld) => new KeycardWriterGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("KeycardWriter",
			$"A {"[powered]".Colour(Telnet.Magenta)} machine that programs electronic keycards",
			PoweredMachineBaseGameItemComponentProto.BuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) =>
		new KeycardWriterGameItemComponent(this, parent, temporary);

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) =>
		new KeycardWriterGameItemComponent(component, this, parent);

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) =>
		CreateNewRevision(initiator, (proto, gameworld) => new KeycardWriterGameItemComponentProto(proto, gameworld));
}
