#nullable enable

using MudSharp.Computers;
using MudSharp.Construction.Boundary;
using MudSharp.Form.Shape;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class SignalDetonatorGameItemComponent : GameItemComponent, IArmableExplosiveTrigger,
	IRuntimeConfigurableSignalSinkComponent, IConsumePower
{
	private SignalDetonatorGameItemComponentProto _prototype;
	private readonly LocalSignalSinkSubscription _binding;
	private LocalSignalBinding? _runtimeBinding;
	private double? _runtimeActivationThreshold;
	private bool? _runtimeActiveWhenAboveThreshold;
	private bool _armed;
	private bool _powered;
	private bool _runtimeActive;
	private bool _hasSignalBaseline;
	private bool _lastSignalActive;
	private bool _suppressTrigger;
	private bool _triggering;
	private IProducePower? _powerSource;
	private bool _topologySubscribed;
	private IGameItem? _signalSourceParent;

	public SignalDetonatorGameItemComponent(SignalDetonatorGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
		_binding = new LocalSignalSinkSubscription(parent, this, HandleSourceChanged);
	}

	public SignalDetonatorGameItemComponent(MudSharp.Models.GameItemComponent component,
		SignalDetonatorGameItemComponentProto proto, IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
		_binding = new LocalSignalSinkSubscription(parent, this, HandleSourceChanged);
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public SignalDetonatorGameItemComponent(SignalDetonatorGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_binding = new LocalSignalSinkSubscription(newParent, this, HandleSourceChanged);
		_runtimeBinding = rhs._runtimeBinding;
		_runtimeActivationThreshold = rhs._runtimeActivationThreshold;
		_runtimeActiveWhenAboveThreshold = rhs._runtimeActiveWhenAboveThreshold;
		_armed = rhs._armed;
		CurrentValue = rhs.CurrentValue;
		_hasSignalBaseline = rhs._hasSignalBaseline;
		_lastSignalActive = rhs._lastSignalActive;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public bool Armed => _armed;
	public long SourceComponentId => CurrentBinding.SourceComponentId;
	public string SourceComponentName => CurrentBinding.SourceComponentName;
	public string SourceEndpointKey => CurrentBinding.SourceEndpointKey;
	public ISignalSource? UpstreamSource => _binding.UpstreamSource;
	public double CurrentValue { get; private set; }
	public LocalSignalBinding CurrentBinding => _runtimeBinding ?? new LocalSignalBinding(
		0L, string.Empty, _prototype.SourceComponentId, _prototype.SourceComponentName, _prototype.SourceEndpointKey);
	public double ActivationThreshold => _runtimeActivationThreshold ?? _prototype.ActivationThreshold;
	public bool ActiveWhenAboveThreshold => _runtimeActiveWhenAboveThreshold ?? _prototype.ActiveWhenAboveThreshold;
	public double PowerConsumptionInWatts => _prototype.RequiresPower && Armed ? _prototype.PowerConsumptionInWatts : 0.0;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (SignalDetonatorGameItemComponentProto)newProto;
		ReconnectSource();
		RefreshPowerDrawdown();
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new SignalDetonatorGameItemComponent(this, newParent, temporary);
	}

	private void LoadFromXml(XElement root)
	{
		_armed = bool.TryParse(root.Element("Armed")?.Value, out var armed) && armed;
		if (double.TryParse(root.Element("CurrentValue")?.Value, out var currentValue) && double.IsFinite(currentValue))
		{
			CurrentValue = currentValue;
		}
		_hasSignalBaseline = bool.TryParse(root.Element("HasSignalBaseline")?.Value, out var hasBaseline) && hasBaseline;
		_lastSignalActive = bool.TryParse(root.Element("LastSignalActive")?.Value, out var lastActive) && lastActive;
		var runtimeSourceId = root.Element("RuntimeSourceComponentId");
		if (runtimeSourceId is not null)
		{
			_runtimeBinding = new LocalSignalBinding(
				long.TryParse(root.Element("RuntimeSourceItemId")?.Value, out var sourceItemId) ? sourceItemId : 0L,
				root.Element("RuntimeSourceItemName")?.Value ?? string.Empty,
				long.TryParse(runtimeSourceId.Value, out var sourceId) ? sourceId : 0L,
				root.Element("RuntimeSourceComponentName")?.Value ?? string.Empty,
				SignalComponentUtilities.NormaliseSignalEndpointKey(root.Element("RuntimeSourceEndpointKey")?.Value));
		}
		if (double.TryParse(root.Element("RuntimeActivationThreshold")?.Value, out var threshold) &&
		    double.IsFinite(threshold))
		{
			_runtimeActivationThreshold = threshold;
		}
		if (bool.TryParse(root.Element("RuntimeActiveWhenAboveThreshold")?.Value, out var activeWhenAbove))
		{
			_runtimeActiveWhenAboveThreshold = activeWhenAbove;
		}
	}

	protected override string SaveToXml()
	{
		var root = new XElement("Definition",
			new XElement("Armed", Armed),
			new XElement("CurrentValue", CurrentValue),
			new XElement("HasSignalBaseline", _hasSignalBaseline),
			new XElement("LastSignalActive", _lastSignalActive));
		if (_runtimeBinding is not null)
		{
			root.Add(new XElement("RuntimeSourceItemId", _runtimeBinding.SourceItemId));
			root.Add(new XElement("RuntimeSourceItemName", new XCData(_runtimeBinding.SourceItemName)));
			root.Add(new XElement("RuntimeSourceComponentId", _runtimeBinding.SourceComponentId));
			root.Add(new XElement("RuntimeSourceComponentName", new XCData(_runtimeBinding.SourceComponentName)));
			root.Add(new XElement("RuntimeSourceEndpointKey", new XCData(_runtimeBinding.SourceEndpointKey)));
		}
		if (_runtimeActivationThreshold.HasValue)
		{
			root.Add(new XElement("RuntimeActivationThreshold", _runtimeActivationThreshold.Value));
		}
		if (_runtimeActiveWhenAboveThreshold.HasValue)
		{
			root.Add(new XElement("RuntimeActiveWhenAboveThreshold", _runtimeActiveWhenAboveThreshold.Value));
		}
		return root.ToString();
	}

	public override void FinaliseLoad()
	{
		// Signal subscriptions begin at login after the item graph is complete.
	}

	public override void Login()
	{
		_runtimeActive = true;
		EnsureTopologySubscriptions();
		base.Login();
		ReconnectSource();
		RefreshPowerDrawdown();
		EvaluateCurrentSignalAfterStateChange();
	}

	public override void Quit()
	{
		_runtimeActive = false;
		RemoveTopologySubscriptions();
		RemoveSignalSourceLocationSubscription();
		_binding.Detach();
		EndPowerDrawdown();
		base.Quit();
	}

	public override void Delete()
	{
		_runtimeActive = false;
		RemoveTopologySubscriptions();
		RemoveSignalSourceLocationSubscription();
		_binding.Detach();
		EndPowerDrawdown();
		base.Delete();
	}

	public bool CanArm(ICharacter actor, string argument)
	{
		return !Armed && string.IsNullOrWhiteSpace(argument) && Parent.IsItemType<IDetonatable>();
	}

	public string WhyCannotArm(ICharacter actor, string argument)
	{
		if (Armed) return $"{Parent.HowSeen(actor, true)} is already armed.";
		if (!Parent.IsItemType<IDetonatable>()) return $"{Parent.HowSeen(actor, true)} has no explosive payload to detonate.";
		if (!string.IsNullOrWhiteSpace(argument)) return "This signal detonator does not take an arming duration or datetime.";
		return $"{Parent.HowSeen(actor, true)} cannot be armed at this time.";
	}

	public bool Arm(ICharacter actor, string argument, IEmote? playerEmote = null)
	{
		if (!CanArm(actor, argument))
		{
			actor.Send(WhyCannotArm(actor, argument));
			return false;
		}

		_armed = true;
		Changed = true;
		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(_prototype.ArmEmote, actor, actor, Parent)).Append(playerEmote));
		RefreshPowerDrawdown();
		EstablishSignalBaseline();
		if (_prototype.ActivationMode == ExplosiveSignalActivationMode.Level)
		{
			EvaluateCurrentSignalAfterStateChange();
		}
		return true;
	}

	public bool CanDisarm(ICharacter actor)
	{
		return Armed && _prototype.CanBeDisarmed;
	}

	public string WhyCannotDisarm(ICharacter actor)
	{
		if (!Armed) return $"{Parent.HowSeen(actor, true)} is not armed.";
		return _prototype.CanBeDisarmed
			? $"{Parent.HowSeen(actor, true)} cannot be disarmed at this time."
			: $"Once armed, {Parent.HowSeen(actor)} cannot be disarmed.";
	}

	public bool Disarm(ICharacter actor, IEmote? playerEmote = null)
	{
		if (!CanDisarm(actor))
		{
			actor.Send(WhyCannotDisarm(actor));
			return false;
		}

		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(_prototype.DisarmEmote, actor, actor, Parent)).Append(playerEmote));
		_armed = false;
		Changed = true;
		EndPowerDrawdown();
		EstablishSignalBaseline();
		return true;
	}

	public void ReconnectSource()
	{
		RemoveSignalSourceLocationSubscription();
		_suppressTrigger = true;
		_binding.Reconnect(CurrentBinding, strictSourceItemId: CurrentBinding.SourceItemId > 0);
		_suppressTrigger = false;
		EnsureSignalSourceTopologySubscription();
		if (_binding.UpstreamSource is null)
		{
			CurrentValue = 0.0;
			_hasSignalBaseline = false;
			return;
		}
		EstablishSignalBaseline();
		if (_prototype.ActivationMode == ExplosiveSignalActivationMode.Level)
		{
			EvaluateCurrentSignalAfterStateChange();
		}
	}

	public void ReceiveSignal(ComputerSignal signal, ISignalSource source)
	{
		CurrentValue = signal.Value;
		var active = SignalComponentUtilities.IsActiveSignal(CurrentValue, ActivationThreshold,
			ActiveWhenAboveThreshold);
		var shouldTrigger = Armed && IsOperational && !_suppressTrigger &&
		                    (_prototype.ActivationMode == ExplosiveSignalActivationMode.Level
			                    ? active
			                    : _hasSignalBaseline && !_lastSignalActive && active);
		_lastSignalActive = active;
		_hasSignalBaseline = true;
		Changed = true;
		if (shouldTrigger)
		{
			TriggerDetonation();
		}
	}

	private void HandleSourceChanged(ISignalSourceComponent source, ComputerSignal signal)
	{
		if (!_suppressTrigger && !SignalComponentUtilities.ItemsAreSignalAccessible(Parent, source.Parent))
		{
			ReconnectSource();
			return;
		}

		ReceiveSignal(signal, source);
	}

	private bool IsOperational => !_prototype.RequiresPower || _powered;

	private void EstablishSignalBaseline()
	{
		_lastSignalActive = SignalComponentUtilities.IsActiveSignal(CurrentValue, ActivationThreshold,
			ActiveWhenAboveThreshold);
		_hasSignalBaseline = _binding.UpstreamSource is not null;
		Changed = true;
	}

	private void EvaluateCurrentSignalAfterStateChange()
	{
		if (!Armed || !IsOperational || _binding.UpstreamSource is null)
		{
			return;
		}
		if (_prototype.ActivationMode == ExplosiveSignalActivationMode.Level &&
		    SignalComponentUtilities.IsActiveSignal(CurrentValue, ActivationThreshold, ActiveWhenAboveThreshold))
		{
			TriggerDetonation();
		}
	}

	private void TriggerDetonation()
	{
		if (_triggering || !Armed)
		{
			return;
		}
		_triggering = true;
		_armed = false;
		Changed = true;
		_binding.Detach();
		EndPowerDrawdown();
		Parent.GetItemType<IDetonatable>()?.Detonate();
	}

	public bool ConfigureSignalBinding(ISignalSourceComponent source, string? endpointKey, out string error)
	{
		_runtimeBinding = SignalComponentUtilities.CreateBinding(source, endpointKey);
		Changed = true;
		ReconnectSource();
		error = string.Empty;
		return true;
	}

	public void ClearSignalBinding()
	{
		_runtimeBinding = null;
		Changed = true;
		ReconnectSource();
	}

	public bool SetActivationThreshold(double threshold, out string error)
	{
		if (!double.IsFinite(threshold))
		{
			error = "That is not a valid numeric threshold.";
			return false;
		}
		_runtimeActivationThreshold = threshold;
		Changed = true;
		EstablishSignalBaseline();
		if (_prototype.ActivationMode == ExplosiveSignalActivationMode.Level)
		{
			EvaluateCurrentSignalAfterStateChange();
		}
		error = string.Empty;
		return true;
	}

	public void SetActiveWhenAboveThreshold(bool activeWhenAboveThreshold)
	{
		_runtimeActiveWhenAboveThreshold = activeWhenAboveThreshold;
		Changed = true;
		EstablishSignalBaseline();
		if (_prototype.ActivationMode == ExplosiveSignalActivationMode.Level)
		{
			EvaluateCurrentSignalAfterStateChange();
		}
	}

	public void OnPowerCutIn()
	{
		_powered = true;
		EstablishSignalBaseline();
		if (_prototype.ActivationMode == ExplosiveSignalActivationMode.Level)
		{
			EvaluateCurrentSignalAfterStateChange();
		}
	}

	public void OnPowerCutOut()
	{
		_powered = false;
		EstablishSignalBaseline();
	}

	private void RefreshPowerDrawdown()
	{
		if (!_runtimeActive || !_prototype.RequiresPower || !Armed)
		{
			EndPowerDrawdown();
			return;
		}
		var source = ResolvePowerSource();
		if (!ReferenceEquals(source, _powerSource))
		{
			_powerSource?.EndDrawdown(this);
			_powerSource = source;
			_powered = false;
		}
		if (!_powered)
		{
			_powerSource?.BeginDrawdown(this);
		}
	}

	private IProducePower? ResolvePowerSource()
	{
		return Parent.GetItemTypes<IProducePower>().FirstOrDefault() ??
		       Parent.AttachedAndConnectedItems.SelectMany(x => x.GetItemTypes<IProducePower>()).FirstOrDefault();
	}

	private void EndPowerDrawdown()
	{
		_powerSource?.EndDrawdown(this);
		_powerSource = null;
		_powered = false;
	}

	private void EnsureTopologySubscriptions()
	{
		if (_topologySubscribed)
		{
			return;
		}
		Parent.OnConnected += ParentOnConnectionChanged;
		Parent.OnDisconnected += ParentOnConnectionChanged;
		Parent.OnLocationChanged += ParentOnLocationChanged;
		_topologySubscribed = true;
	}

	private void RemoveTopologySubscriptions()
	{
		if (!_topologySubscribed)
		{
			return;
		}
		Parent.OnConnected -= ParentOnConnectionChanged;
		Parent.OnDisconnected -= ParentOnConnectionChanged;
		Parent.OnLocationChanged -= ParentOnLocationChanged;
		_topologySubscribed = false;
	}

	private void ParentOnConnectionChanged(IConnectable other, ConnectorType type)
	{
		ReconnectSource();
		RefreshPowerDrawdown();
	}

	private void ParentOnLocationChanged(ILocateable locatable, ICellExit exit)
	{
		ReconnectSource();
	}

	private void EnsureSignalSourceTopologySubscription()
	{
		var sourceParent = (_binding.UpstreamSource as IGameItemComponent)?.Parent;
		if (sourceParent is null && CurrentBinding.SourceItemId > 0)
		{
			sourceParent = Gameworld.TryGetItem(CurrentBinding.SourceItemId, true);
		}

		if (sourceParent is null || ReferenceEquals(sourceParent, Parent))
		{
			return;
		}

		_signalSourceParent = sourceParent;
		_signalSourceParent.OnLocationChanged += SignalSourceParentOnLocationChanged;
		_signalSourceParent.OnConnected += SignalSourceParentOnConnectionChanged;
		_signalSourceParent.OnDisconnected += SignalSourceParentOnConnectionChanged;
	}

	private void RemoveSignalSourceLocationSubscription()
	{
		if (_signalSourceParent is null)
		{
			return;
		}

		_signalSourceParent.OnLocationChanged -= SignalSourceParentOnLocationChanged;
		_signalSourceParent.OnConnected -= SignalSourceParentOnConnectionChanged;
		_signalSourceParent.OnDisconnected -= SignalSourceParentOnConnectionChanged;
		_signalSourceParent = null;
	}

	private void SignalSourceParentOnLocationChanged(ILocateable locatable, ICellExit exit)
	{
		ReconnectSource();
	}

	private void SignalSourceParentOnConnectionChanged(IConnectable other, ConnectorType type)
	{
		ReconnectSource();
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type is DescriptionType.Short or DescriptionType.Full or DescriptionType.Evaluate;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type == DescriptionType.Short && Armed)
		{
			return $"{description} {"(armed)".Colour(Telnet.BoldRed)}";
		}
		if (type == DescriptionType.Full)
		{
			return
				$"{description}\n\nIts signal detonator is {(Armed ? "armed".Colour(Telnet.BoldRed) : "disarmed".Colour(Telnet.Yellow))}{(_prototype.RequiresPower ? $" and {(_powered ? "powered".ColourValue() : "unpowered".ColourError())}" : string.Empty)}.";
		}
		if (type == DescriptionType.Evaluate)
		{
			var thresholdMode = ActiveWhenAboveThreshold ? "at or above" : "below";
			var powerText = _prototype.RequiresPower
				? $" It requires {_prototype.PowerConsumptionInWatts.ToString("N3", voyeur).ColourValue()} watts while armed."
				: " It does not require electrical power.";
			return
				$"It has a {_prototype.ActivationMode.DescribeEnum().ToLowerInvariant().ColourValue()} signal detonator driven by {SignalComponentUtilities.DescribeSignalComponent(CurrentBinding).ColourName()}, active {thresholdMode} {ActivationThreshold.ToString("N2", voyeur).ColourValue()}.{powerText} It {(_prototype.CanBeDisarmed ? "can" : "cannot")} be disarmed after arming.";
		}
		return description;
	}

	public override int DecorationPriority => int.MaxValue - 1;
}
