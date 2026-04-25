#nullable enable

using MudSharp.Character;
using MudSharp.Computers;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Form.Shape;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class ComputerTerminalGameItemComponent : PoweredMachineBaseGameItemComponent, IComputerTerminal, IConnectable
{
	private readonly List<IComputerTerminalSession> _sessions = [];
	private readonly List<long> _pendingConnectionIds = [];
	private ComputerTerminalGameItemComponentProto _prototype;
	private IConnectable? _connectedHost;

	public ComputerTerminalGameItemComponent(ComputerTerminalGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public ComputerTerminalGameItemComponent(MudSharp.Models.GameItemComponent component,
		ComputerTerminalGameItemComponentProto proto,
		IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
		LoadRuntimeState(XElement.Parse(component.Definition));
	}

	public ComputerTerminalGameItemComponent(ComputerTerminalGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public long TerminalItemId => Parent.Id;
	public IComputerHost? ConnectedHost => _connectedHost as IComputerHost;
	public IEnumerable<IComputerTerminalSession> Sessions => _sessions.ToList();
	public IEnumerable<ConnectorType> Connections => [ComputerConnectionTypes.TerminalPlug];
	public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems =>
		_connectedHost is null
			? Enumerable.Empty<Tuple<ConnectorType, IConnectable>>()
			: new[] { Tuple.Create(ComputerConnectionTypes.TerminalPlug, _connectedHost) };
	public IEnumerable<ConnectorType> FreeConnections => _connectedHost is null ? Connections : Enumerable.Empty<ConnectorType>();
	public bool Independent => true;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ComputerTerminalGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour,
		PerceiveIgnoreFlags flags)
	{
		if (type != DescriptionType.Full)
		{
			return description;
		}

		var sb = new StringBuilder(description);
		sb.AppendLine();
		sb.AppendLine();
		sb.AppendLine(
			$"Its computer terminal is {(SwitchedOn ? "switched on".ColourValue() : "switched off".ColourError())}, {(IsPowered ? "powered".ColourValue() : "not powered".ColourError())}, and {(ConnectedHost is null ? "not connected to any host".ColourError() : $"connected to {ConnectedHost.Name.ColourName()}".ColourValue())}.");
		return sb.ToString();
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (ComputerTerminalGameItemComponentProto)newProto;
	}

	protected override XElement SaveToXml(XElement root)
	{
		root.Add(new XElement("ConnectedItems",
			_connectedHost is null
				? null
				: new XElement("Connection", new XAttribute("id", _connectedHost.Parent.Id))));
		return root;
	}

	public override void FinaliseLoad()
	{
		base.FinaliseLoad();
		foreach (var id in _pendingConnectionIds.ToList())
		{
			var item = Gameworld.TryGetItem(id, true);
			var connectable = item?.GetItemTypes<IConnectable>().FirstOrDefault(x => CanConnect(null, x));
			if (connectable is null)
			{
				continue;
			}

			Connect(null, connectable);
		}

		_pendingConnectionIds.Clear();
	}

	public override void Quit()
	{
		CloseAllSessions();
		base.Quit();
	}

	public override void Delete()
	{
		CloseAllSessions();
		if (_connectedHost is not null)
		{
			RawDisconnect(_connectedHost, true);
		}

		base.Delete();
	}

	protected override void OnPowerCutInAction()
	{
	}

	protected override void OnPowerCutOutAction()
	{
		CloseAllSessions();
	}

	public bool TryConnectSession(ICharacter actor, out IComputerTerminalSession? session, out string error)
	{
		session = null;
		if (!SwitchedOn)
		{
			error = $"{Parent.HowSeen(actor)} is switched off.";
			return false;
		}

		if (!IsPowered)
		{
			error = $"{Parent.HowSeen(actor)} is not powered.";
			return false;
		}

		if (ConnectedHost is null)
		{
			error = $"{Parent.HowSeen(actor)} is not connected to a computer host.";
			return false;
		}

		if (!ConnectedHost.Powered)
		{
			error = $"{ConnectedHost.Name.ColourName()} is not currently powered.";
			return false;
		}

		session = _sessions.FirstOrDefault(x => x.User == actor);
		if (session is not null)
		{
			error = string.Empty;
			return true;
		}

		session = new ComputerTerminalSession
		{
			User = actor,
			Terminal = this,
			Host = ConnectedHost,
			CurrentOwner = ConnectedHost
		};
		_sessions.Add(session);
		error = string.Empty;
		return true;
	}

	public void DisconnectSession(ICharacter actor, bool removeEffect)
	{
		var session = _sessions.FirstOrDefault(x => x.User == actor);
		if (session is null)
		{
			return;
		}

		_sessions.Remove(session);
		if (removeEffect)
		{
			var effect = actor.CombinedEffectsOfType<ComputerTerminalSessionEffect>()
				.FirstOrDefault(x => ReferenceEquals(x.Session.Terminal, this));
			if (effect is not null)
			{
				actor.RemoveEffect(effect);
			}
		}
	}

	public bool TrySelectOwner(ICharacter actor, IComputerExecutableOwner owner, out string error)
	{
		var session = _sessions.FirstOrDefault(x => x.User == actor) as ComputerTerminalSession;
		if (session is null)
		{
			error = "You are not currently using that computer terminal.";
			return false;
		}

		if (!ReferenceEquals(owner, ConnectedHost) &&
		    !ConnectedHost!.MountedStorage.Any(x => ReferenceEquals(x, owner)))
		{
			error = "That computer owner is not available through this terminal.";
			return false;
		}

		session.CurrentOwner = owner;
		error = string.Empty;
		return true;
	}

	public bool TryType(ICharacter actor, string text, out string error)
	{
		var session = _sessions.FirstOrDefault(x => x.User == actor);
		if (session is null)
		{
			error = "You are not currently using that computer terminal.";
			return false;
		}

		return Gameworld.ComputerExecutionService.TrySubmitTerminalInput(session, text, out error);
	}

	public bool CanBeConnectedTo(IConnectable other)
	{
		return other is IComputerHost;
	}

	public bool CanConnect(ICharacter? actor, IConnectable other)
	{
		return _connectedHost is null &&
		       other is IComputerHost &&
		       other.FreeConnections.Any(x => x.CompatibleWith(ComputerConnectionTypes.TerminalPlug));
	}

	public void Connect(ICharacter? actor, IConnectable other)
	{
		if (!CanConnect(actor, other))
		{
			return;
		}

		_connectedHost = other;
		other.RawConnect(this, other.FreeConnections.First(x => x.CompatibleWith(ComputerConnectionTypes.TerminalPlug)));
		Changed = true;
		Parent.ConnectedItem(other, ComputerConnectionTypes.TerminalPlug);
	}

	public void RawConnect(IConnectable other, ConnectorType type)
	{
		_connectedHost = other;
		Changed = true;
	}

	public string WhyCannotConnect(ICharacter? actor, IConnectable other)
	{
		return _connectedHost is not null
			? $"{Parent.HowSeen(actor)} is already connected to a computer host."
			: $"{Parent.HowSeen(actor)} cannot connect to {other.Parent.HowSeen(actor)}.";
	}

	public bool CanBeDisconnectedFrom(IConnectable other)
	{
		return true;
	}

	public bool CanDisconnect(ICharacter actor, IConnectable other)
	{
		return ReferenceEquals(_connectedHost, other);
	}

	public void Disconnect(ICharacter actor, IConnectable other)
	{
		RawDisconnect(other, true);
	}

	public void RawDisconnect(IConnectable other, bool handleEvents)
	{
		if (!ReferenceEquals(_connectedHost, other))
		{
			return;
		}

		CloseAllSessions();
		_connectedHost = null;
		if (handleEvents)
		{
			other.RawDisconnect(this, false);
			Parent.DisconnectedItem(other, ComputerConnectionTypes.TerminalPlug);
			other.Parent.DisconnectedItem(this, ComputerConnectionTypes.TerminalPlug);
		}

		Changed = true;
	}

	public string WhyCannotDisconnect(ICharacter actor, IConnectable other)
	{
		return $"{Parent.HowSeen(actor)} is not connected to {other.Parent.HowSeen(actor)}.";
	}

	private void LoadRuntimeState(XElement root)
	{
		_pendingConnectionIds.AddRange(root.Element("ConnectedItems")?.Elements("Connection")
			.Select(x => long.TryParse(x.Attribute("id")?.Value, out var id) ? id : 0L)
			.Where(x => x > 0) ?? Enumerable.Empty<long>());
	}

	private void CloseAllSessions()
	{
		foreach (var session in _sessions.ToList())
		{
			DisconnectSession(session.User, true);
		}
	}
}
