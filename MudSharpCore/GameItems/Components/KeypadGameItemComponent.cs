#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Computers;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class KeypadGameItemComponent : PoweredMachineBaseGameItemComponent, ISelectable, ISignalSourceComponent
{
	private KeypadGameItemComponentProto _prototype;
	private DateTime? _activeUntil;
	private bool _heartbeatSubscribed;
	private ComputerSignal _currentSignal;

	public KeypadGameItemComponent(KeypadGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public KeypadGameItemComponent(MudSharp.Models.GameItemComponent component, KeypadGameItemComponentProto proto,
		IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public KeypadGameItemComponent(KeypadGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_activeUntil = rhs._activeUntil;
		_currentSignal = rhs._currentSignal;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public long LocalSignalSourceIdentifier => Prototype.Id;
	public string EndpointKey => SignalComponentUtilities.DefaultLocalSignalEndpointKey;
	public ComputerSignal CurrentSignal => _currentSignal;
	public event SignalChangedEvent? SignalChanged;
	public double CurrentValue => _currentSignal.Value;
	public TimeSpan? Duration => _currentSignal.Duration;
	public TimeSpan? PulseInterval => _currentSignal.PulseInterval;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new KeypadGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags)
	{
		var state = _activeUntil is not null && _activeUntil > DateTime.UtcNow && SwitchedOn && _onAndPowered
			? "active".ColourValue()
			: "inactive".ColourName();
		return
			$"{description}\n\nIt has an electronic keypad that accepts numeric codes via {"select <item> <digits>".ColourCommand()}, is {(SwitchedOn ? "switched on".ColourValue() : "switched off".ColourError())}, {(_onAndPowered ? "powered".ColourValue() : "not powered".ColourError())}, and is currently {state}.";
	}

	public override int DecorationPriority => 1000;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (KeypadGameItemComponentProto)newProto;
		RefreshSignalState(true);
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		var element = root.Element("ActiveUntilTicks");
		if (element is not null && long.TryParse(element.Value, out var ticks) && ticks > 0)
		{
			_activeUntil = new DateTime(ticks, DateTimeKind.Utc);
		}
	}

	public override void FinaliseLoad()
	{
		if (_activeUntil is null || _activeUntil <= DateTime.UtcNow)
		{
			_activeUntil = null;
		}

		RefreshSignalState(false);
	}

	public override void Login()
	{
		base.Login();
		if (_activeUntil is not null && _activeUntil > DateTime.UtcNow)
		{
			EnsureHeartbeatSubscription();
		}
		else
		{
			_activeUntil = null;
		}

		RefreshSignalState(false);
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

	protected override XElement SaveToXml(XElement root)
	{
		root.Add(new XElement("ActiveUntilTicks", _activeUntil?.Ticks ?? 0));
		return root;
	}

	protected override void OnPowerCutInAction()
	{
		RefreshSignalState(true);
	}

	protected override void OnPowerCutOutAction()
	{
		SetCurrentSignal(default, true);
		HandleDescriptionUpdate();
	}

	public bool CanSelect(ICharacter character, string argument)
	{
		return IsNumericCode(argument);
	}

	public bool Select(ICharacter character, string argument, IEmote playerEmote, bool silent = false)
	{
		if (!IsNumericCode(argument))
		{
			character.Send("You must enter a numeric code on that keypad.");
			return false;
		}

		if (!SwitchedOn)
		{
			character.Send($"{Parent.HowSeen(character)} is switched off.");
			return false;
		}

		if (!_onAndPowered)
		{
			character.Send($"{Parent.HowSeen(character)} does not appear to be powered.");
			return false;
		}

		if (!silent)
		{
			character.OutputHandler.Handle(
				new MixedEmoteOutput(
					new Emote(_prototype.EntryEmote, character, character, Parent),
					flags: OutputFlags.SuppressObscured).Append(playerEmote));
		}

		if (!argument.Trim().Equals(_prototype.Code, StringComparison.InvariantCulture))
		{
			character.Send("Nothing happens.");
			return false;
		}

		_activeUntil = DateTime.UtcNow + _prototype.SignalDuration;
		EnsureHeartbeatSubscription();
		RefreshSignalState(true);
		return true;
	}

	private static bool IsNumericCode(string argument)
	{
		var value = argument?.Trim();
		return !string.IsNullOrEmpty(value) && value.All(char.IsDigit);
	}

	private void HeartbeatTick()
	{
		if (_activeUntil is null || _activeUntil > DateTime.UtcNow)
		{
			return;
		}

		_activeUntil = null;
		RemoveHeartbeatSubscription();
		RefreshSignalState(true);
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

	private void RefreshSignalState(bool markChanged)
	{
		var shouldBeActive = _activeUntil is not null &&
		                     _activeUntil > DateTime.UtcNow &&
		                     SwitchedOn &&
		                     _onAndPowered;
		SetCurrentSignal(shouldBeActive
			? new ComputerSignal(_prototype.SignalValue, _prototype.SignalDuration, null)
			: default, markChanged);
		HandleDescriptionUpdate();
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
