#nullable enable

using MudSharp.Body;
using MudSharp.Computers;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class ImplantComputerStorageGameItemComponent : ComputerStorageGameItemComponent, IImplantComputerStorage,
	IImplantReportStatus
{
	private ImplantComputerStorageGameItemComponentProto _implantPrototype;
	private ImplantMachineRuntime _implant;
	private string _alias = "drive";
	private bool _powered;
	private bool _busHeartbeatSubscribed;
	private IProducePower? _powerSource;
	public ImplantComputerStorageGameItemComponent(ImplantComputerStorageGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(proto, parent, temporary)
	{
		_implantPrototype = proto; _implant = CreateSupport();
	}
	public ImplantComputerStorageGameItemComponent(MudSharp.Models.GameItemComponent component, ImplantComputerStorageGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_implantPrototype = proto; _implant = CreateSupport(); var root = XElement.Parse(component.Definition); _implant.Load(root); _alias = root.Element("ImplantAlias")?.Value ?? "drive";
	}
	private ImplantComputerStorageGameItemComponent(ImplantComputerStorageGameItemComponent rhs, IGameItem parent, bool temporary) : base(rhs, parent, temporary)
	{
		_implantPrototype = rhs._implantPrototype; _implant = CreateSupport(); _alias = rhs._alias;
	}
	private ImplantMachineRuntime CreateSupport() => new(Parent, () => _implantPrototype, () => Changed = true);
	public override IGameItemComponentProto Prototype => _implantPrototype;
	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) => new ImplantComputerStorageGameItemComponent(this, newParent, temporary);
	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto) { base.UpdateComponentNewPrototype(newProto); _implantPrototype = (ImplantComputerStorageGameItemComponentProto)newProto; }
	protected override string SaveToXml()
	{
		var root = XElement.Parse(base.SaveToXml()); _implant.Save(root); root.Add(new XElement("ImplantAlias", new XCData(_alias))); return root.ToString();
	}
	private bool Operational => _powered && FunctionFactor > 0.0;
	private bool HasValidMount => Operational && base.MountedHost is IImplantComputerHost { Powered: true } host &&
		ImplantComputerUtilities.GetPoweredBus(this) is { } bus &&
		ReferenceEquals(bus, ImplantComputerUtilities.GetPoweredBus(host));
	public override bool Mounted => HasValidMount;
	public override IComputerHost? MountedHost => HasValidMount ? base.MountedHost : null;
	public IImplantComputerHost? AssignedComputerHost => base.MountedHost as IImplantComputerHost;
	public override IComputerFileSystem? FileSystem => Operational ? base.FileSystem : null;
	public double FunctionFactor => _implant.FunctionFactor(_powered);
	public bool External => ((IImplantMachinePrototypeSettings)_implantPrototype).External;
	public string ExternalDescription => ((IImplantMachinePrototypeSettings)_implantPrototype).ExternalDescription;
	public IBodyPrototype TargetBody => ((IImplantMachinePrototypeSettings)_implantPrototype).TargetBody;
	public IBodypart TargetBodypart { get => _implant.TargetBodypart; set => _implant.TargetBodypart = value; }
	public IBody InstalledBody => _implant.InstalledBody!;
	public void InstallImplant(IBody body) { _implant.Install(body); ConnectPowerSource(); }
	public void RemoveImplant() { UnsubscribeBusHeartbeat(); if (base.MountedHost is IConnectable host) RawDisconnect(host, true); ReleasePowerSource(); _implant.Remove(); }
	public double ImplantSpaceOccupied => ((IImplantMachinePrototypeSettings)_implantPrototype).ImplantSpaceOccupied;
	public Difficulty InstallDifficulty => ((IImplantMachinePrototypeSettings)_implantPrototype).InstallDifficulty;
	public double PowerConsumptionInWatts => 2.0;
	public void OnPowerCutIn() { _powered = true; SubscribeBusHeartbeat(); ValidateMount(); Changed = true; }
	public void OnPowerCutOut() { _powered = false; UnsubscribeBusHeartbeat(); Gameworld.ComputerExecutionService.DeactivateOwner(this); Changed = true; }
	public override void Login() { base.Login(); if (InstalledBody is not null) ConnectPowerSource(); }
	public override void Quit() { UnsubscribeBusHeartbeat(); ReleasePowerSource(); base.Quit(); }
	public override void Delete() { UnsubscribeBusHeartbeat(); ReleasePowerSource(); base.Delete(); }
	private void ConnectPowerSource()
	{
		var powerSource = Parent.GetItemType<IProducePower>();
		if (ReferenceEquals(powerSource, _powerSource)) return;
		ReleasePowerSource();
		_powerSource = powerSource;
		_powerSource?.BeginDrawdown(this);
	}
	private void ReleasePowerSource()
	{
		_powerSource?.EndDrawdown(this);
		_powerSource = null;
	}
	private void SubscribeBusHeartbeat()
	{
		if (_busHeartbeatSubscribed) return;
		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += ValidateMount;
		_busHeartbeatSubscribed = true;
	}
	private void UnsubscribeBusHeartbeat()
	{
		if (!_busHeartbeatSubscribed) return;
		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= ValidateMount;
		_busHeartbeatSubscribed = false;
	}
	private void ValidateMount()
	{
		if (HasValidMount)
		{
			Gameworld.ComputerExecutionService.ActivateOwner(this);
			return;
		}

		Gameworld.ComputerExecutionService.DeactivateOwner(this);
	}
	public string AliasForCommands { get => _alias; set { _alias = value; Changed = true; } }
	public IEnumerable<string> Commands => ["host"];
	public string CommandHelp => "host <implant alias|none> - mounts or unmounts this drive on an implanted host";
	public void IssueCommand(string command, StringStack arguments)
	{
		var actor = InstalledBody?.Actor;
		if (!Operational || actor is null) { actor?.Send("That implant is unpowered or non-functional."); return; }
		if (arguments.IsFinished) { actor.Send("Which implanted host should mount this drive?"); return; }
		var alias = arguments.PopSpeech();
		if (alias.EqualTo("none")) { if (base.MountedHost is IConnectable old) RawDisconnect(old, true); actor.Send("The implant drive is now unmounted."); return; }
		var hostImplant = ImplantComputerUtilities.ResolveAliased<IImplantComputerHost>(this, alias, out var error);
		if (hostImplant is not IConnectable connectableHost) { actor.Send(hostImplant is null ? error : "That implant host cannot accept storage connections."); return; }
		if (ReferenceEquals(base.MountedHost, hostImplant)) { actor.Send("The implant drive is already mounted to that host."); return; }
		if (!connectableHost.FreeConnections.Any(x => x.CompatibleWith(ComputerConnectionTypes.StoragePlug))) { actor.Send("That implanted host has no free storage port."); return; }
		if (base.MountedHost is IConnectable current) RawDisconnect(current, true);
		Connect(actor, connectableHost); actor.Send($"The implant drive is now mounted to {hostImplant.Parent.HowSeen(actor).ColourName()}.");
	}
	public string ReportStatus() => $"\t* Power: {_powered.ToColouredString()}\n\t* Host: {(base.MountedHost?.Name ?? "none").ColourName()}\n\t* Used: {(base.FileSystem?.UsedBytes ?? 0L).ToString("N0", InstalledBody?.Actor).ColourValue()} bytes\n";
}
