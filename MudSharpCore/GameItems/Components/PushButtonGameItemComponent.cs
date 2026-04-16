#nullable enable

using MudSharp.Character;
using MudSharp.Computers;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class PushButtonGameItemComponent : GameItemComponent, ISelectable, ISignalSourceComponent
{
	private PushButtonGameItemComponentProto _prototype;
	private DateTime? _activeUntil;
	private bool _heartbeatSubscribed;
	private ComputerSignal _currentSignal;

	public PushButtonGameItemComponent(PushButtonGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public PushButtonGameItemComponent(MudSharp.Models.GameItemComponent component, PushButtonGameItemComponentProto proto,
		IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public PushButtonGameItemComponent(PushButtonGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_activeUntil = rhs._activeUntil;
		_currentSignal = rhs._currentSignal;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public ComputerSignal CurrentSignal => _currentSignal;
	public event SignalChangedEvent? SignalChanged;
	public double CurrentValue => _currentSignal.Value;
	public TimeSpan? Duration => _currentSignal.Duration;
	public TimeSpan? PulseInterval => _currentSignal.PulseInterval;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new PushButtonGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags)
	{
		return $"{description}\n\nIt has a push button that responds to {"select".ColourCommand()} {" " + _prototype.Keyword.ColourCommand()} and is currently {(_activeUntil is not null ? "active".ColourValue() : "inactive".ColourName())}.";
	}

	public override int DecorationPriority => 1000;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (PushButtonGameItemComponentProto)newProto;
	}

	public override void FinaliseLoad()
	{
		if (_activeUntil is not null && _activeUntil > DateTime.UtcNow)
		{
			_currentSignal = new ComputerSignal(_prototype.SignalValue, _prototype.SignalDuration, null);
			EnsureHeartbeatSubscription();
			return;
		}

		_activeUntil = null;
		_currentSignal = default;
	}

	public override void Delete()
	{
		RemoveHeartbeatSubscription();
		base.Delete();
	}

	public override void Quit()
	{
		RemoveHeartbeatSubscription();
		base.Quit();
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("ActiveUntilTicks", _activeUntil?.Ticks ?? 0)
		).ToString();
	}

	private void LoadFromXml(XElement root)
	{
		var element = root.Element("ActiveUntilTicks");
		if (element is not null && long.TryParse(element.Value, out var ticks) && ticks > 0)
		{
			_activeUntil = new DateTime(ticks, DateTimeKind.Utc);
		}
	}

	public bool CanSelect(ICharacter character, string argument)
	{
		return !string.IsNullOrWhiteSpace(argument) &&
		       _prototype.Keyword.StartsWith(argument, StringComparison.InvariantCultureIgnoreCase);
	}

	public bool Select(ICharacter character, string argument, IEmote playerEmote, bool silent = false)
	{
		if (!CanSelect(character, argument))
		{
			character.Send("That is not a valid control to press.");
			return false;
		}

		if (!silent)
		{
			character.OutputHandler.Handle(
				new MixedEmoteOutput(
					new Emote(_prototype.PressEmote, character, character, Parent),
					flags: OutputFlags.SuppressObscured).Append(playerEmote));
		}

		_activeUntil = DateTime.UtcNow + _prototype.SignalDuration;
		EnsureHeartbeatSubscription();
		SetCurrentSignal(new ComputerSignal(_prototype.SignalValue, _prototype.SignalDuration, null), true);
		return true;
	}

	private void HeartbeatTick()
	{
		if (_activeUntil is null || _activeUntil > DateTime.UtcNow)
		{
			return;
		}

		_activeUntil = null;
		RemoveHeartbeatSubscription();
		SetCurrentSignal(default, true);
	}

	private void EnsureHeartbeatSubscription()
	{
		if (_heartbeatSubscribed)
		{
			return;
		}

		Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatTick;
		_heartbeatSubscribed = true;
	}

	private void RemoveHeartbeatSubscription()
	{
		if (!_heartbeatSubscribed)
		{
			return;
		}

		Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatTick;
		_heartbeatSubscribed = false;
	}

	private void SetCurrentSignal(ComputerSignal signal, bool markChanged)
	{
		if (SignalComponentUtilities.SignalsEqual(_currentSignal, signal))
		{
			return;
		}

		_currentSignal = signal;
		if (markChanged)
		{
			Changed = true;
		}

		SignalChanged?.Invoke(this, _currentSignal);
	}
}
