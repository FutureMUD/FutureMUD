#nullable enable

using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Prototypes;

public class ImplantComputerTerminalGameItemComponentProto : ComputerTerminalGameItemComponentProto,
	IImplantComputerTerminalPrototype, IImplantReportStatusPrototype, IImplantMachinePrototypeSettings
{
	private ImplantMachinePrototypeSettings _implant = null!;
	public ImplantComputerTerminalGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "Implant Computer Terminal") => _implant ??= new ImplantMachinePrototypeSettings(gameworld);
	protected ImplantComputerTerminalGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto, gameworld) => _implant ??= new ImplantMachinePrototypeSettings(gameworld);
	public override string TypeDescription => "Implant Computer Terminal";
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
	protected override XElement SaveSubtypeToXml(XElement root) { base.SaveSubtypeToXml(root); _implant.Save(root); return root; }
	public override string ShowBuildingHelp => $"{base.ShowBuildingHelp}{ImplantMachinePrototypeSettings.BuildingHelp}";
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var verb = command.PopForSwitch();
		var result = _implant.BuildingCommand(actor, verb, command, () => Changed = true);
		return result ?? base.BuildingCommand(actor, command.GetUndo());
	}
	public static new void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("implantcomputerterminal", true, (g, a) => new ImplantComputerTerminalGameItemComponentProto(g, a));
		manager.AddBuilderLoader("implant terminal", false, (g, a) => new ImplantComputerTerminalGameItemComponentProto(g, a));
		manager.AddDatabaseLoader("Implant Computer Terminal", (p, g) => new ImplantComputerTerminalGameItemComponentProto(p, g));
		manager.AddTypeHelpInfo("Implant Computer Terminal", "A neurally controlled implanted computer terminal", ImplantMachinePrototypeSettings.BuildingHelp);
	}
	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) => new ImplantComputerTerminalGameItemComponent(this, parent, temporary);
	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) => new ImplantComputerTerminalGameItemComponent(component, this, parent);
	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) => CreateNewRevision(initiator, (p, g) => new ImplantComputerTerminalGameItemComponentProto(p, g));
}
