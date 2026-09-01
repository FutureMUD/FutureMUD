#nullable enable

using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class KeycardScannerGameItemComponentProto : AccessControlReaderGameItemComponentProto,
	IKeycardScannerPrototype
{
	private const string SpecificBuildingHelpText = @"
	#3code add <code>#0 - adds an initially accepted case-sensitive code
	#3code remove <code>#0 - removes an initially accepted code
	#3code clear#0 - removes all initially accepted codes";
	private readonly List<string> _initialCodes = [];

	protected KeycardScannerGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "KeycardScanner")
	{
	}

	protected KeycardScannerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	public IReadOnlyCollection<string> InitialCodes => _initialCodes.AsReadOnly();
	public override string TypeDescription => "KeycardScanner";
	public override string AccessMountType => "KeycardScanner";
	protected override string ComponentDescriptionOLCByline => "This item is a powered keycard access reader";
	public override string ShowBuildingHelp => $"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";
	protected override string ComponentDescriptionOLCAddendum(ICharacter actor) =>
		$"Initially Accepted Codes: {(_initialCodes.Any() ? _initialCodes.Select(x => x.ColourCommand()).ListToString() : "None".ColourError())}\n{AccessControlDescription(actor)}";

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		LoadAccessControlFromXml(root);
		_initialCodes.AddRange(root.Element("Codes")?.Elements("Code").Select(x => x.Value) ?? []);
	}

	protected override XElement SaveAccessSubtypeToXml(XElement root)
	{
		root.Add(new XElement("Codes", _initialCodes.Select(x => new XElement("Code", new XCData(x)))));
		return root;
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (!command.Peek().EqualTo("code"))
		{
			return base.BuildingCommand(actor, command);
		}

		command.PopForSwitch();
		return AccessCodeBuilderHelper.Handle(actor, command, _initialCodes, () => Changed = true, "reader");
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("keycardscanner", true,
			(gameworld, account) => new KeycardScannerGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("KeycardScanner",
			(proto, gameworld) => new KeycardScannerGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("KeycardScanner",
			$"A {"[powered]".Colour(Telnet.Magenta)} keycard {SignalComponentUtilities.SignalGeneratorTag} for access control",
			$"{PoweredMachineBaseGameItemComponentProto.BuildingHelpText}{AccessControlBuildingHelpText}{SpecificBuildingHelpText}");
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) =>
		new KeycardScannerGameItemComponent(this, parent, temporary);

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) =>
		new KeycardScannerGameItemComponent(component, this, parent);

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) =>
		CreateNewRevision(initiator, (proto, gameworld) => new KeycardScannerGameItemComponentProto(proto, gameworld));
}
