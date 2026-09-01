#nullable enable

using MudSharp.Body;
using MudSharp.Commands.Modules;
using MudSharp.Computers;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class ImplantComputerTerminalGameItemComponent : ComputerTerminalGameItemComponent, IImplantComputerTerminal,
	IImplantReportStatus
{
	private ImplantComputerTerminalGameItemComponentProto _implantPrototype;
	private ImplantMachineRuntime _implant;
	private string _alias = "terminal";
	private bool _busHeartbeatSubscribed;
	public ImplantComputerTerminalGameItemComponent(ImplantComputerTerminalGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(proto, parent, temporary) { _implantPrototype = proto; _implant = CreateSupport(); }
	public ImplantComputerTerminalGameItemComponent(MudSharp.Models.GameItemComponent component, ImplantComputerTerminalGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent) { _implantPrototype = proto; _implant = CreateSupport(); var root = XElement.Parse(component.Definition); _implant.Load(root); _alias = root.Element("ImplantAlias")?.Value ?? "terminal"; }
	private ImplantComputerTerminalGameItemComponent(ImplantComputerTerminalGameItemComponent rhs, IGameItem parent, bool temporary) : base(rhs, parent, temporary) { _implantPrototype = rhs._implantPrototype; _implant = CreateSupport(); _alias = rhs._alias; }
	private ImplantMachineRuntime CreateSupport() => new(Parent, () => _implantPrototype, () => Changed = true);
	public override IGameItemComponentProto Prototype => _implantPrototype;
	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) => new ImplantComputerTerminalGameItemComponent(this, newParent, temporary);
	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto) { base.UpdateComponentNewPrototype(newProto); _implantPrototype = (ImplantComputerTerminalGameItemComponentProto)newProto; }
	protected override XElement SaveToXml(XElement root) { base.SaveToXml(root); _implant.Save(root); root.Add(new XElement("ImplantAlias", new XCData(_alias))); return root; }
	private bool Operational => IsPowered && FunctionFactor > 0.0;
	public double FunctionFactor => _implant.FunctionFactor(IsPowered);
	public bool External => ((IImplantMachinePrototypeSettings)_implantPrototype).External;
	public string ExternalDescription => ((IImplantMachinePrototypeSettings)_implantPrototype).ExternalDescription;
	public IBodyPrototype TargetBody => ((IImplantMachinePrototypeSettings)_implantPrototype).TargetBody;
	public IBodypart TargetBodypart { get => _implant.TargetBodypart; set => _implant.TargetBodypart = value; }
	public IBody InstalledBody => _implant.InstalledBody!;
	public void InstallImplant(IBody body) { _implant.Install(body); RefreshPowerSourceConnection(); }
	public void RemoveImplant() { UnsubscribeBusHeartbeat(); CloseNeuralSession(); if (base.ConnectedHost is IConnectable host) RawDisconnect(host, true); ReleasePowerSourceConnection(); _implant.Remove(); }
	public double ImplantSpaceOccupied => ((IImplantMachinePrototypeSettings)_implantPrototype).ImplantSpaceOccupied;
	public Difficulty InstallDifficulty => ((IImplantMachinePrototypeSettings)_implantPrototype).InstallDifficulty;
	public string AliasForCommands { get => _alias; set { _alias = value; Changed = true; } }
	public IEnumerable<string> Commands => ["host", "connect", "disconnect"];
	public string CommandHelp => "host <alias|none> - assigns an implanted host\nconnect - opens a normal terminal session\ndisconnect - closes it";
	public void IssueCommand(string command, StringStack arguments)
	{
		var actor = InstalledBody?.Actor;
		if (actor is null || !Operational) { actor?.Send("That implant terminal is unpowered or non-functional."); return; }
		if (command.StartsWith("host", StringComparison.InvariantCultureIgnoreCase)) { CommandHost(actor, arguments); return; }
		if (command.StartsWith("disconnect", StringComparison.InvariantCultureIgnoreCase)) { CloseNeuralSession(); actor.Send("You close the implanted terminal session."); return; }
		if (ImplantComputerUtilities.GetPoweredBus(this) is null) { actor.Send("This terminal has no powered neural data link."); return; }
		if (!ElectronicsModule.TryEnsureProgrammingTerminalSession(actor, this, out var session, out _, out var error)) { actor.Send(error); return; }
		actor.Send($"You open a neural terminal session on {session!.Host.Name.ColourName()}.");
	}
	private void CommandHost(ICharacter actor, StringStack arguments)
	{
		if (arguments.IsFinished) { actor.Send("Which implanted computer host should this terminal use?"); return; }
		var alias = arguments.PopSpeech();
		if (alias.EqualTo("none")) { if (base.ConnectedHost is IConnectable old) RawDisconnect(old, true); actor.Send("The implanted terminal is no longer assigned to a host."); return; }
		var host = ImplantComputerUtilities.ResolveAliased<IImplantComputerHost>(this, alias, out var error);
		if (host is not IConnectable connectableHost) { actor.Send(host is null ? error : "That implant host cannot accept terminal connections."); return; }
		if (ReferenceEquals(base.ConnectedHost, host)) { actor.Send("The implanted terminal is already assigned to that host."); return; }
		if (!connectableHost.FreeConnections.Any(x => x.CompatibleWith(ComputerConnectionTypes.TerminalPlug))) { actor.Send("That implanted host has no free terminal port."); return; }
		if (base.ConnectedHost is IConnectable oldHost) RawDisconnect(oldHost, true);
		Connect(actor, connectableHost); actor.Send($"The implanted terminal is now assigned to {host.Parent.HowSeen(actor).ColourName()}.");
	}
	public override IComputerHost? ConnectedHost
	{
		get
		{
			if (!Operational || base.ConnectedHost is not IImplantComputerHost { Powered: true } host) return null;
			var bus = ImplantComputerUtilities.GetPoweredBus(this);
			return bus is not null && ReferenceEquals(bus, ImplantComputerUtilities.GetPoweredBus(host)) ? host : null;
		}
	}
	public IImplantComputerHost? AssignedComputerHost => base.ConnectedHost as IImplantComputerHost;
	public override bool TryConnectSession(ICharacter actor, out IComputerTerminalSession? session, out string error)
	{
		if (!Operational)
		{
			session = null;
			error = "That implant terminal is unpowered or non-functional.";
			return false;
		}

		return base.TryConnectSession(actor, out session, out error);
	}
	protected override void OnPowerCutInAction()
	{
		base.OnPowerCutInAction();
		if (_busHeartbeatSubscribed) return;
		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += ValidateNeuralConnection;
		_busHeartbeatSubscribed = true;
		ValidateNeuralConnection();
	}
	protected override void OnPowerCutOutAction()
	{
		UnsubscribeBusHeartbeat();
		base.OnPowerCutOutAction();
	}
	public override void Quit()
	{
		UnsubscribeBusHeartbeat();
		base.Quit();
	}
	public override void Delete()
	{
		UnsubscribeBusHeartbeat();
		base.Delete();
	}
	private void ValidateNeuralConnection()
	{
		if (base.ConnectedHost is not null && ConnectedHost is null)
		{
			CloseNeuralSession();
		}
	}
	private void UnsubscribeBusHeartbeat()
	{
		if (!_busHeartbeatSubscribed) return;
		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= ValidateNeuralConnection;
		_busHeartbeatSubscribed = false;
	}
	private void CloseNeuralSession()
	{
		var actor = InstalledBody?.Actor;
		if (actor is null) return;
		DisconnectSession(actor, true);
	}
	public string ReportStatus() => $"\t* Power: {IsPowered.ToColouredString()}\n\t* Host: {(ConnectedHost?.Name ?? "none").ColourName()}\n\t* Session: {Sessions.Any().ToColouredString()}\n";
}
