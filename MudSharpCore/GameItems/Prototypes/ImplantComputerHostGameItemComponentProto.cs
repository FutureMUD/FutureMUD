#nullable enable

using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Prototypes;

public class ImplantComputerHostGameItemComponentProto : ComputerHostGameItemComponentProto,
	IImplantComputerHostPrototype, IImplantReportStatusPrototype, IImplantMachinePrototypeSettings
{
	private ImplantMachinePrototypeSettings _implant = null!;

	public ImplantComputerHostGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Implant Computer Host")
	{
		_implant ??= new ImplantMachinePrototypeSettings(gameworld);
		NetworkPorts = 0;
	}

	protected ImplantComputerHostGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
		_implant ??= new ImplantMachinePrototypeSettings(gameworld);
	}

	public override string TypeDescription => "Implant Computer Host";
	bool IImplantMachinePrototypeSettings.External => _implant.External;
	string IImplantMachinePrototypeSettings.ExternalDescription => _implant.ExternalDescription;
	IBodyPrototype IImplantMachinePrototypeSettings.TargetBody => _implant.TargetBody;
	IBodypart IImplantMachinePrototypeSettings.TargetBodypart => _implant.TargetBodypart;
	double IImplantMachinePrototypeSettings.ImplantSpaceOccupied => _implant.ImplantSpaceOccupied;
	Difficulty IImplantMachinePrototypeSettings.InstallDifficulty => _implant.InstallDifficulty;
	double IImplantMachinePrototypeSettings.ImplantDamageFunctionGrace => _implant.ImplantDamageFunctionGrace;

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		_implant ??= new ImplantMachinePrototypeSettings(Gameworld);
		_implant.Load(root);
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		base.SaveSubtypeToXml(root);
		_implant.Save(root);
		return root;
	}

	public override string ShowBuildingHelp => $"{base.ShowBuildingHelp}{ImplantMachinePrototypeSettings.BuildingHelp}";
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var verb = command.PopForSwitch();
		var result = _implant.BuildingCommand(actor, verb, command, () => Changed = true);
		return result ?? base.BuildingCommand(actor, command.GetUndo());
	}

	public static new void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("implantcomputerhost", true,
			(gameworld, account) => new ImplantComputerHostGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("implant computer host", false,
			(gameworld, account) => new ImplantComputerHostGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Implant Computer Host",
			(proto, gameworld) => new ImplantComputerHostGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("Implant Computer Host", "An implanted computer runtime host",
			ImplantMachinePrototypeSettings.BuildingHelp);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) =>
		new ImplantComputerHostGameItemComponent(this, parent, temporary);
	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) =>
		new ImplantComputerHostGameItemComponent(component, this, parent);
	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) =>
		CreateNewRevision(initiator, (proto, gameworld) => new ImplantComputerHostGameItemComponentProto(proto, gameworld));
}
