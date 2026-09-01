#nullable enable

using MudSharp.Accounts;
using MudSharp.Form.Shape;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class BiometricScannerGameItemComponentProto : AccessControlReaderGameItemComponentProto,
	IBiometricScannerPrototype
{
	private const string SpecificBuildingHelpText = @"
	#3shape <bodypart shape>#0 - sets the anatomy shape that must be presented";

	protected BiometricScannerGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "BiometricScanner")
	{
		BodypartShape = gameworld.BodypartShapes.FirstOrDefault()!;
	}

	protected BiometricScannerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	public IBodypartShape BodypartShape { get; private set; } = null!;
	public override string TypeDescription => "BiometricScanner";
	public override string AccessMountType => "BiometricScanner";
	protected override string ComponentDescriptionOLCByline => "This item is a powered biometric access reader";
	public override string ShowBuildingHelp => $"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor) =>
		$"Bodypart Shape: {BodypartShape?.Name.ColourName() ?? "Not set".ColourError()}\n{AccessControlDescription(actor)}";

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		LoadAccessControlFromXml(root);
		BodypartShape = Gameworld.BodypartShapes.Get(long.TryParse(root.Element("BodypartShape")?.Value, out var id)
			? id
			: 0L)!;
	}

	protected override XElement SaveAccessSubtypeToXml(XElement root)
	{
		root.Add(new XElement("BodypartShape", BodypartShape?.Id ?? 0L));
		return root;
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (!command.Peek().EqualToAny("shape", "bodypart", "part"))
		{
			return base.BuildingCommand(actor, command);
		}

		command.PopForSwitch();
		if (command.IsFinished)
		{
			actor.Send("Which bodypart shape should this biometric scanner recognise?");
			return false;
		}

		var shape = Gameworld.BodypartShapes.GetByIdOrName(command.SafeRemainingArgument);
		if (shape is null)
		{
			actor.Send("There is no such bodypart shape.");
			return false;
		}

		BodypartShape = shape;
		Changed = true;
		actor.Send($"This scanner will now recognise {shape.Name.ColourName()} anatomy.");
		return true;
	}

	public override bool CanSubmit() => BodypartShape is not null && base.CanSubmit();
	public override string WhyCannotSubmit() => BodypartShape is null
		? "You must set a bodypart shape for this scanner."
		: base.WhyCannotSubmit();

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("biometricscanner", true,
			(gameworld, account) => new BiometricScannerGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("BiometricScanner",
			(proto, gameworld) => new BiometricScannerGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("BiometricScanner",
			$"A {"[powered]".Colour(Telnet.Magenta)} biometric {SignalComponentUtilities.SignalGeneratorTag} for access control",
			$"{PoweredMachineBaseGameItemComponentProto.BuildingHelpText}{AccessControlBuildingHelpText}{SpecificBuildingHelpText}");
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) =>
		new BiometricScannerGameItemComponent(this, parent, temporary);

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) =>
		new BiometricScannerGameItemComponent(component, this, parent);

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) =>
		CreateNewRevision(initiator, (proto, gameworld) => new BiometricScannerGameItemComponentProto(proto, gameworld));
}
