#nullable enable

using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class KeycardGameItemComponentProto : GameItemComponentProto, IKeycardPrototype
{
	private readonly List<string> _initialCodes = [];
	protected KeycardGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Keycard")
	{
	}

	protected KeycardGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public IReadOnlyCollection<string> InitialCodes => _initialCodes.AsReadOnly();
	public override string TypeDescription => "Keycard";
	public override string ShowBuildingHelp => @"You can use the following options with this component:

	#3name <name>#0 - renames this component
	#3desc <description>#0 - changes its description
	#3code add <code>#0 - adds an initial case-sensitive code
	#3code remove <code>#0 - removes an initial code
	#3code clear#0 - removes all initial codes";

	protected override void LoadFromXml(XElement root)
	{
		_initialCodes.AddRange(root.Element("Codes")?.Elements("Code").Select(x => x.Value) ?? []);
	}

	protected override string SaveToXml() =>
		new XElement("Definition",
			new XElement("Codes", _initialCodes.Select(x => new XElement("Code", new XCData(x)))))
			.ToString();

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (!command.Peek().EqualTo("code"))
		{
			return base.BuildingCommand(actor, command);
		}

		command.PopForSwitch();
		return AccessCodeBuilderHelper.Handle(actor, command, _initialCodes, () => Changed = true, "keycard");
	}

	public override string ComponentDescriptionOLC(ICharacter actor) =>
		$"{"Keycard Game Item Component".ColourName()} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\nThis component stores up to {AccessCredentialUtilities.MaximumCodes.ToString("N0", actor).ColourValue()} case-sensitive access codes.\nInitial Codes: {(_initialCodes.Any() ? _initialCodes.Select(x => x.ColourCommand()).ListToString() : "None".ColourError())}";

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("keycard", true,
			(gameworld, account) => new KeycardGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Keycard", (proto, gameworld) => new KeycardGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("Keycard", "Stores multiple case-sensitive electronic access codes", string.Empty);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) =>
		new KeycardGameItemComponent(this, parent, temporary);

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) =>
		new KeycardGameItemComponent(component, this, parent);

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) =>
		CreateNewRevision(initiator, (proto, gameworld) => new KeycardGameItemComponentProto(proto, gameworld));
}
