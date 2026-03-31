#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Construction.Grids;
using MudSharp.Form.Audio;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class CellularPhoneGameItemComponent : GameItemComponent, ITelephone, ITelephoneNumberOwner,
	ICanConnectToTelecommunicationsGrid
{
	private CellularPhoneGameItemComponentProto _prototype;
	private ITelecommunicationsGrid? _grid;
	private string? _phoneNumber;
	private string? _preferredNumber;
	private bool _allowSharedNumber;
	private bool _powered;
	private bool _ringHeartbeatSubscribed;
	private bool _switchedOn;
	private bool _isOffHook;
	private bool _isRinging;
	private ITelephoneCall? _currentCall;
	private AudioVolume? _ringVolumeOverride;
	private IProducePower? _powerSource;

	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (CellularPhoneGameItemComponentProto)newProto;
	}

	public CellularPhoneGameItemComponent(CellularPhoneGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
		_switchedOn = true;
	}

	public CellularPhoneGameItemComponent(Models.GameItemComponent component, CellularPhoneGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public CellularPhoneGameItemComponent(CellularPhoneGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_switchedOn = rhs._switchedOn;
		_preferredNumber = rhs._preferredNumber;
		_allowSharedNumber = rhs._allowSharedNumber;
		_ringVolumeOverride = rhs._ringVolumeOverride;
	}

	private void LoadFromXml(XElement root)
	{
		_switchedOn = bool.Parse(root.Element("SwitchedOn")?.Value ?? "true");
		_preferredNumber = root.Element("PreferredNumber")?.Value;
		_allowSharedNumber = bool.Parse(root.Element("AllowSharedNumber")?.Value ?? "false");
		if (int.TryParse(root.Element("RingVolumeOverride")?.Value, out var ringVolume) &&
		    Enum.IsDefined(typeof(AudioVolume), ringVolume))
		{
			_ringVolumeOverride = (AudioVolume)ringVolume;
		}
		TelecommunicationsGrid =
			Gameworld.Grids.Get(long.Parse(root.Element("Grid")?.Value ?? "0")) as ITelecommunicationsGrid;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new CellularPhoneGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Grid", TelecommunicationsGrid?.Id ?? 0),
			new XElement("SwitchedOn", _switchedOn),
			new XElement("PreferredNumber", _preferredNumber ?? string.Empty),
			new XElement("AllowSharedNumber", _allowSharedNumber),
			new XElement("RingVolumeOverride", _ringVolumeOverride.HasValue ? (int)_ringVolumeOverride.Value : -1)
		).ToString();
	}

	public override void Delete()
	{
		base.Delete();
		TelecommunicationsGrid = null;
		_powerSource?.EndDrawdown(this);
		StopRinging();
	}

	public override void Login()
	{
		base.Login();
		_powerSource = Parent.GetItemType<IProducePower>();
		_powerSource?.BeginDrawdown(this);
	}

	public override void Quit()
	{
		base.Quit();
		_powerSource?.EndDrawdown(this);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type != DescriptionType.Full)
		{
			return base.Decorate(voyeur, name, description, type, colour, flags);
		}

		var sb = new StringBuilder();
		sb.AppendLine(description);
		sb.AppendLine($"It is currently switched {(_switchedOn ? "on".ColourValue() : "off".ColourError())}.");
		sb.AppendLine($"It is {(IsPowered ? "powered".ColourValue() : "not powered".ColourError())}.");
		sb.AppendLine($"Its number is {(PhoneNumber?.ColourValue() ?? "unassigned".ColourError())}.");
		sb.AppendLine($"Its ringer is set to {TelephoneRingSettings.DescribeSetting(RingVolume, true).ColourValue()}.");
		sb.AppendLine(
			$"It is connected to {(TelecommunicationsGrid == null ? "no telecommunications grid".ColourError() : $"grid #{TelecommunicationsGrid.Id.ToString("N0", voyeur)}".ColourValue())}.");
		sb.AppendLine($"It currently {(HasCoverage ? "has signal".ColourValue() : "has no signal".ColourError())}.");
		sb.AppendLine($"It is {(IsOffHook ? "off the hook".ColourError() : "on the hook".ColourValue())}.");
		if (IsConnected)
		{
			sb.AppendLine("It is currently in an active call.");
		}
		else if (IsRinging)
		{
			sb.AppendLine("It is currently ringing.");
		}
		else if (_currentCall != null)
		{
			sb.AppendLine("It is currently dialling another number.");
		}

		return sb.ToString();
	}

	public ITelephoneNumberOwner? NumberOwner => this;
	public string? PhoneNumber => _phoneNumber;

	public string? PreferredNumber
	{
		get => _preferredNumber;
		set
		{
			_preferredNumber = string.IsNullOrWhiteSpace(value) ? null : value;
			Changed = true;
			TelecommunicationsGrid?.RequestNumber(this, _preferredNumber, _allowSharedNumber);
		}
	}

	public bool AllowSharedNumber
	{
		get => _allowSharedNumber;
		set
		{
			_allowSharedNumber = value;
			Changed = true;
			TelecommunicationsGrid?.RequestNumber(this, _preferredNumber, _allowSharedNumber);
		}
	}

	public bool IsPowered => _powered && _switchedOn;
	public bool IsOffHook => _isOffHook;
	public bool HasCoverage => CurrentZones.Any(zone => zone != null && HasCoverageInZone(zone));
	public bool CanReceiveCalls =>
		IsPowered && TelecommunicationsGrid != null && !string.IsNullOrWhiteSpace(PhoneNumber) &&
		!_isOffHook && _currentCall == null && HasCoverage;
	public bool IsRinging => _isRinging;
	public bool IsConnected => (_currentCall?.Participants ?? Array.Empty<ITelephone>()).Contains(this) &&
	                           _currentCall?.IsConnected == true;
	public bool IsEngaged => _currentCall != null || _isOffHook;
	public AudioVolume RingVolume => _ringVolumeOverride ?? _prototype.RingVolume;
	public ITelephoneCall? CurrentCall => _currentCall;
	public IEnumerable<ITelephone> ConnectedPhones =>
		(_currentCall?.Participants ?? Array.Empty<ITelephone>()).Where(x => x != this).ToList();
	public ITelephone? ConnectedPhone => ConnectedPhones.FirstOrDefault();

	public ITelecommunicationsGrid? TelecommunicationsGrid
	{
		get => _grid;
		set
		{
			if (_grid == value)
			{
				return;
			}

			_grid?.LeaveGrid((ITelephoneNumberOwner)this);
			_grid = value;
			_grid?.JoinGrid((ITelephoneNumberOwner)this);
			Changed = true;
		}
	}

	ITelecommunicationsGrid? ICanConnectToTelecommunicationsGrid.TelecommunicationsGrid
	{
		get => TelecommunicationsGrid;
		set => TelecommunicationsGrid = value;
	}

	IGrid ICanConnectToGrid.Grid
	{
		get => TelecommunicationsGrid;
		set => TelecommunicationsGrid = value as ITelecommunicationsGrid;
	}

	string ICanConnectToGrid.GridType => "Telecommunications";
	IEnumerable<ITelephone> ITelephoneNumberOwner.ConnectedTelephones => [this];

	ITelecommunicationsGrid? ITelephoneNumberOwner.TelecommunicationsGrid
	{
		get => TelecommunicationsGrid;
		set => TelecommunicationsGrid = value;
	}

	public void AssignPhoneNumber(string? number)
	{
		_phoneNumber = number;
		Changed = true;
	}

	public bool SwitchedOn
	{
		get => _switchedOn;
		set
		{
			_switchedOn = value;
			Changed = true;
		}
	}

	public double PowerConsumptionInWatts => _switchedOn ? _prototype.Wattage : 0.0;

	public void OnPowerCutIn()
	{
		_powered = true;
	}

	public void OnPowerCutOut()
	{
		_powered = false;
		if (_currentCall != null || _isOffHook)
		{
			EndCall(_currentCall);
		}
	}

	public bool ManualTransmit => true;
	public string TransmitPremote => _prototype.TransmitPremote;

	public void Transmit(SpokenLanguageInfo spokenLanguage)
	{
		if (!IsConnected || !IsPowered || _currentCall == null)
		{
			return;
		}

		_currentCall.RelayTransmission(this, spokenLanguage);
	}

	public void ReceiveTransmission(double frequency, string dataTransmission, long encryption, ITransmit origin)
	{
	}

	public void ReceiveTransmission(double frequency, SpokenLanguageInfo spokenLanguage, long encryption, ITransmit origin)
	{
		if (!IsPowered)
		{
			return;
		}

		Parent.OutputHandler.Handle(
			new LanguageOutput(
				new Emote("@ reproduce|reproduces speech over the line", Parent, Parent),
				spokenLanguage,
				null,
				flags: OutputFlags.PurelyAudible
			)
		);
	}

	public IEnumerable<string> SwitchSettings => ["on", "off", ..TelephoneRingSettings.CellularSettings];

	public bool CanSwitch(ICharacter actor, string setting)
	{
		if (setting.Equals("on", StringComparison.InvariantCultureIgnoreCase))
		{
			return !_switchedOn;
		}

		if (setting.Equals("off", StringComparison.InvariantCultureIgnoreCase))
		{
			return _switchedOn;
		}

		return TelephoneRingSettings.TryGetVolumeForSetting(setting, true, out var volume) &&
		       RingVolume != volume;
	}

	public string WhyCannotSwitch(ICharacter actor, string setting)
	{
		if (TelephoneRingSettings.TryGetVolumeForSetting(setting, true, out _))
		{
			return $"{Parent.HowSeen(actor, true)} is already set to {TelephoneRingSettings.DescribeSetting(RingVolume, true).ColourValue()}.";
		}

		return setting.Equals("on", StringComparison.InvariantCultureIgnoreCase)
			? $"{Parent.HowSeen(actor, true)} is already on."
			: $"{Parent.HowSeen(actor, true)} is already off.";
	}

	public bool Switch(ICharacter actor, string setting)
	{
		if (!CanSwitch(actor, setting))
		{
			return false;
		}

		if (TelephoneRingSettings.TryGetVolumeForSetting(setting, true, out var volume))
		{
			_ringVolumeOverride = volume;
			Changed = true;
			return true;
		}

		_switchedOn = setting.Equals("on", StringComparison.InvariantCultureIgnoreCase);
		Changed = true;
		if (!_switchedOn && (_currentCall != null || _isOffHook))
		{
			EndCall(_currentCall);
		}

		return true;
	}

	public bool CanPickUp(ICharacter actor, out string error)
	{
		if (_isOffHook && _currentCall == null)
		{
			error = "That telephone is already off the hook.";
			return false;
		}

		if (TelecommunicationsGrid == null)
		{
			error = "That telephone is not connected to a telecommunications line.";
			return false;
		}

		if (!IsPowered)
		{
			error = "That telephone is not ready to use right now.";
			return false;
		}

		error = string.Empty;
		return true;
	}

	public bool PickUp(ICharacter actor, out string error)
	{
		if (!CanPickUp(actor, out error))
		{
			return false;
		}

		if (TelecommunicationsGrid != null && !string.IsNullOrWhiteSpace(PhoneNumber))
		{
			if (TelecommunicationsGrid.TryPickUp(this, out error))
			{
				return true;
			}

			if (!error.EqualTo("There is no live call on that line right now."))
			{
				return false;
			}
		}

		_isOffHook = true;
		Changed = true;
		error = string.Empty;
		return true;
	}

	public bool CanDial(ICharacter actor, string number, out string error)
	{
		if (TelecommunicationsGrid == null)
		{
			error = "That telephone is not connected to a telecommunications grid.";
			return false;
		}

		if (!IsPowered || string.IsNullOrWhiteSpace(PhoneNumber))
		{
			error = "That telephone is not ready to make calls right now.";
			return false;
		}

		if (!HasCoverage)
		{
			error = "That cellular phone has no signal.";
			return false;
		}

		if (_currentCall != null)
		{
			error = "That telephone is already in use.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(number))
		{
			error = "You must specify a number to dial.";
			return false;
		}

		error = string.Empty;
		return true;
	}

	public bool Dial(ICharacter actor, string number, out string error)
	{
		if (!CanDial(actor, number, out error))
		{
			return false;
		}

		_isOffHook = true;
		Changed = true;
		return TelecommunicationsGrid!.TryStartCall(this, number, out error);
	}

	public bool CanAnswer(ICharacter actor, out string error)
	{
		if (!_isRinging || _currentCall == null)
		{
			error = "That telephone is not ringing.";
			return false;
		}

		if (!IsPowered)
		{
			error = "That telephone cannot answer calls right now.";
			return false;
		}

		if (!HasCoverage)
		{
			error = "That cellular phone has no signal.";
			return false;
		}

		error = string.Empty;
		return true;
	}

	public bool Answer(ICharacter actor, out string error)
	{
		if (!CanAnswer(actor, out error))
		{
			return false;
		}

		return TelecommunicationsGrid!.TryPickUp(this, out error);
	}

	public bool CanHangUp(ICharacter actor, out string error)
	{
		if (_currentCall == null && !_isOffHook)
		{
			error = "That telephone is not currently in use.";
			return false;
		}

		error = string.Empty;
		return true;
	}

	public bool HangUp(ICharacter actor, out string error)
	{
		if (!CanHangUp(actor, out error))
		{
			return false;
		}

		EndCall(_currentCall);
		error = string.Empty;
		return true;
	}

	public void BeginOutgoingCall(ITelephoneCall call, string number)
	{
		_currentCall = call;
		_isOffHook = true;
		_isRinging = false;
		Changed = true;
	}

	public void ReceiveIncomingCall(ITelephoneCall call)
	{
		_currentCall = call;
		_isOffHook = false;
		_isRinging = true;
		Changed = true;
		Ring();
		StartRinging();
	}

	public void ConnectCall(ITelephoneCall call)
	{
		StopRinging();
		_currentCall = call;
		_isOffHook = true;
		_isRinging = false;
		Changed = true;
	}

	public void NotifyCallProgress(string message)
	{
		Parent.OutputHandler.Send(message);
	}

	public void EndCall(ITelephoneCall? call, bool notifyGrid = true)
	{
		if (call != null && _currentCall != null && !ReferenceEquals(call, _currentCall))
		{
			return;
		}

		var existingCall = _currentCall;
		StopRinging();
		_currentCall = null;
		_isRinging = false;
		_isOffHook = false;
		Changed = true;
		if (notifyGrid && existingCall != null)
		{
			TelecommunicationsGrid?.EndCall(this, existingCall);
		}
	}

	private IEnumerable<Construction.IZone> CurrentZones =>
		Parent.TrueLocations.Select(x => x?.Zone).Where(x => x != null).Cast<Construction.IZone>().Distinct().ToList();

	private bool HasCoverageInZone(Construction.IZone zone)
	{
		return Gameworld.Items
		                .SelectNotNull(x => x.GetItemType<ICellPhoneTower>())
		                .Any(x => x.TelecommunicationsGrid == TelecommunicationsGrid && x.ProvidesCoverage(zone));
	}

	private void Ring()
	{
		if (!_isRinging || !IsPowered)
		{
			return;
		}

		if (RingVolume == AudioVolume.Silent)
		{
			NotifySilentRingWearer();
			return;
		}

		Parent.Handle(
			new AudioOutput(new Emote(_prototype.RingEmote, Parent, Parent), RingVolume,
				flags: OutputFlags.PurelyAudible),
			OutputRange.Local
		);

		foreach (var location in Parent.TrueLocations.Distinct())
		{
			location.HandleAudioEcho("You hear a telephone ringing {0}.", RingVolume, Parent, Parent.RoomLayer);
		}
	}

	private void NotifySilentRingWearer()
	{
		var wornBody = GetWornContainerOwner();
		if (wornBody?.OutputHandler == null)
		{
			return;
		}

		wornBody.OutputHandler.Send("You feel a muted vibration from one of your worn items.");
	}

	private IBody? GetWornContainerOwner()
	{
		if (Parent.InInventoryOf?.WornItems.Contains(Parent) == true)
		{
			return Parent.InInventoryOf;
		}

		IBody? fallbackBody = null;
		var current = Parent.ContainedIn;
		while (current != null)
		{
			if (current.InInventoryOf?.WornItems.Contains(current) == true)
			{
				return current.InInventoryOf;
			}

			fallbackBody ??= current.InInventoryOf;
			current = current.ContainedIn;
		}

		return fallbackBody;
	}

	private void RingHeartbeat()
	{
		if (!_isRinging || !IsPowered)
		{
			StopRinging();
			return;
		}

		Ring();
	}

	private void StartRinging()
	{
		if (_ringHeartbeatSubscribed)
		{
			return;
		}

		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += RingHeartbeat;
		_ringHeartbeatSubscribed = true;
	}

	private void StopRinging()
	{
		if (!_ringHeartbeatSubscribed)
		{
			return;
		}

		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= RingHeartbeat;
		_ringHeartbeatSubscribed = false;
	}

	public void SetRingVolumeOverride(AudioVolume? volume)
	{
		_ringVolumeOverride = volume.HasValue
			? TelephoneRingSettings.NormaliseVolume(volume.Value, true)
			: null;
		Changed = true;
	}
}
