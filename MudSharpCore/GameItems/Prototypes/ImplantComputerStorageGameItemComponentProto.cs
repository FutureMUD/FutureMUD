#nullable enable

using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Prototypes;

public class ImplantComputerStorageGameItemComponentProto : ComputerStorageGameItemComponentProto,
	IImplantComputerStoragePrototype, IImplantReportStatusPrototype, IImplantMachinePrototypeSettings
{
	private ImplantMachinePrototypeSettings _implant = null!;
	public ImplantComputerStorageGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Implant Computer Storage") => _implant ??= new ImplantMachinePrototypeSettings(gameworld);
	protected ImplantComputerStorageGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld) => _implant ??= new ImplantMachinePrototypeSettings(gameworld);
	public override string TypeDescription => "Implant Computer Storage";
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
	protected override string SaveToXml()
	{
		var root = XElement.Parse(base.SaveToXml());
		_implant.Save(root);
		return root.ToString();
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
		manager.AddBuilderLoader("implantcomputerstorage", true, (g, a) => new ImplantComputerStorageGameItemComponentProto(g, a));
		manager.AddBuilderLoader("implant hard drive", false, (g, a) => new ImplantComputerStorageGameItemComponentProto(g, a));
		manager.AddDatabaseLoader("Implant Computer Storage", (p, g) => new ImplantComputerStorageGameItemComponentProto(p, g));
		manager.AddTypeHelpInfo("Implant Computer Storage", "A general-purpose implanted computer hard drive", ImplantMachinePrototypeSettings.BuildingHelp);
	}
	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) => new ImplantComputerStorageGameItemComponent(this, parent, temporary);
	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) => new ImplantComputerStorageGameItemComponent(component, this, parent);
	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) => CreateNewRevision(initiator, (p, g) => new ImplantComputerStorageGameItemComponentProto(p, g));
}
