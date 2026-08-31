#nullable enable

using MudSharp.Body;
using MudSharp.Computers;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class ImplantComputerHostGameItemComponent : ComputerHostGameItemComponent, IImplantComputerHost,
	IImplantReportStatus
{
	private ImplantComputerHostGameItemComponentProto _implantPrototype;
	private ImplantMachineRuntime _implant;
	private string _alias = "computer";

	public ImplantComputerHostGameItemComponent(ImplantComputerHostGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_implantPrototype = proto;
		_implant = CreateSupport();
	}

	public ImplantComputerHostGameItemComponent(MudSharp.Models.GameItemComponent component,
		ImplantComputerHostGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_implantPrototype = proto;
		_implant = CreateSupport();
		var root = XElement.Parse(component.Definition);
		_implant.Load(root);
		_alias = root.Element("ImplantAlias")?.Value ?? "computer";
	}

	private ImplantComputerHostGameItemComponent(ImplantComputerHostGameItemComponent rhs, IGameItem parent,
		bool temporary) : base(rhs, parent, temporary)
	{
		_implantPrototype = rhs._implantPrototype;
		_implant = CreateSupport();
		_alias = rhs._alias;
	}

	private ImplantMachineRuntime CreateSupport() => new(Parent, () => _implantPrototype, () => Changed = true);
	public override IGameItemComponentProto Prototype => _implantPrototype;
	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) =>
		new ImplantComputerHostGameItemComponent(this, newParent, temporary);

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_implantPrototype = (ImplantComputerHostGameItemComponentProto)newProto;
	}

	protected override XElement SaveToXml(XElement root)
	{
		base.SaveToXml(root);
		_implant.Save(root);
		root.Add(new XElement("ImplantAlias", new XCData(_alias)));
		return root;
	}

	public override bool Powered => IsPowered && FunctionFactor > 0.0;
	public double FunctionFactor => _implant.FunctionFactor(IsPowered);
	public bool External => ((IImplantMachinePrototypeSettings)_implantPrototype).External;
	public string ExternalDescription => ((IImplantMachinePrototypeSettings)_implantPrototype).ExternalDescription;
	public IBodyPrototype TargetBody => ((IImplantMachinePrototypeSettings)_implantPrototype).TargetBody;
	public IBodypart TargetBodypart { get => _implant.TargetBodypart; set => _implant.TargetBodypart = value; }
	public IBody InstalledBody => _implant.InstalledBody!;
	public void InstallImplant(IBody body) { _implant.Install(body); RefreshPowerSourceConnection(); }
	public void RemoveImplant() { ReleasePowerSourceConnection(); _implant.Remove(); }
	public double ImplantSpaceOccupied => ((IImplantMachinePrototypeSettings)_implantPrototype).ImplantSpaceOccupied;
	public Difficulty InstallDifficulty => ((IImplantMachinePrototypeSettings)_implantPrototype).InstallDifficulty;
	public string AliasForCommands { get => _alias; set { _alias = value; Changed = true; } }
	public IEnumerable<string> Commands => ["on", "off"];
	public string CommandHelp => "on - switches the implanted host on\noff - switches it off";

	public void IssueCommand(string command, StringStack arguments)
	{
		if (command.StartsWith("on", StringComparison.InvariantCultureIgnoreCase)) Switch(InstalledBody?.Actor!, "on");
		else if (command.StartsWith("off", StringComparison.InvariantCultureIgnoreCase)) Switch(InstalledBody?.Actor!, "off");
		InstalledBody?.Actor.Send($"The implanted computer host is now {(SwitchedOn ? "on" : "off").ColourValue()}.");
	}

	public string ReportStatus() =>
		$"\t* Host power: {Powered.ToColouredString()}\n\t* Files: {(FileSystem?.Files.Count() ?? 0).ToString("N0", InstalledBody?.Actor).ColourValue()}\n\t* Mounted storage: {MountedStorage.Count().ToString("N0", InstalledBody?.Actor).ColourValue()}\n";
}
