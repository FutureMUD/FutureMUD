#nullable enable

using MudSharp.Computers;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class ElectronicLockGameItemComponent : ProgLockGameItemComponent, IRuntimeConfigurableSignalSinkComponent
{
	private ElectronicLockGameItemComponentProto _prototype;
	private readonly LocalSignalSinkSubscription _binding;
	private LocalSignalBinding? _runtimeBinding;
	private double? _runtimeActivationThreshold;
	private bool? _runtimeActiveWhenAboveThreshold;

	public ElectronicLockGameItemComponent(ElectronicLockGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
		_binding = new LocalSignalSinkSubscription(parent, this, HandleSourceChanged);
	}

	public ElectronicLockGameItemComponent(MudSharp.Models.GameItemComponent component,
		ElectronicLockGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
		_binding = new LocalSignalSinkSubscription(parent, this, HandleSourceChanged);
		LoadRuntimeConfiguration(XElement.Parse(component.Definition));
	}

	public ElectronicLockGameItemComponent(ElectronicLockGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_binding = new LocalSignalSinkSubscription(newParent, this, HandleSourceChanged);
		_runtimeBinding = rhs._runtimeBinding;
		_runtimeActivationThreshold = rhs._runtimeActivationThreshold;
		_runtimeActiveWhenAboveThreshold = rhs._runtimeActiveWhenAboveThreshold;
	}

	public long SourceComponentId => CurrentBinding.SourceComponentId;
	public string SourceComponentName => CurrentBinding.SourceComponentName;
	public string SourceEndpointKey => CurrentBinding.SourceEndpointKey;
	public ISignalSource? UpstreamSource => _binding.UpstreamSource;
	public double CurrentValue { get; private set; }
	public LocalSignalBinding CurrentBinding => _runtimeBinding ?? new LocalSignalBinding(
		_prototype.SourceComponentId,
		_prototype.SourceComponentName,
		_prototype.SourceEndpointKey);
	public double ActivationThreshold => _runtimeActivationThreshold ?? _prototype.ActivationThreshold;
	public bool ActiveWhenAboveThreshold => _runtimeActiveWhenAboveThreshold ?? _prototype.LockWhenAboveThreshold;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ElectronicLockGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (ElectronicLockGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		var root = XElement.Parse(base.SaveToXml());
		SaveRuntimeConfiguration(root);
		return root.ToString();
	}

	public override void FinaliseLoad()
	{
		ReconnectSource();
	}

	public override void Delete()
	{
		_binding.Detach();
		base.Delete();
	}

	public override void Quit()
	{
		_binding.Detach();
		base.Quit();
	}

	public void ReconnectSource()
	{
		_binding.Reconnect(SourceComponentId, SourceComponentName, SourceEndpointKey);
		if (_binding.UpstreamSource is not null)
		{
			return;
		}

		ApplySignalValue(0.0);
	}

	public void ReceiveSignal(ComputerSignal signal, ISignalSource source)
	{
		ApplySignalValue(signal.Value);
	}

	private void ApplySignalValue(double value)
	{
		CurrentValue = value;
		var shouldLock = SignalComponentUtilities.IsActiveSignal(value, ActivationThreshold,
			ActiveWhenAboveThreshold);
		SetLocked(shouldLock, true);
	}

	private void HandleSourceChanged(ISignalSourceComponent source, ComputerSignal signal)
	{
		ReceiveSignal(signal, source);
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
		if (double.IsNaN(threshold) || double.IsInfinity(threshold))
		{
			error = "That is not a valid numeric threshold.";
			return false;
		}

		_runtimeActivationThreshold = threshold;
		Changed = true;
		ApplySignalValue(CurrentValue);
		error = string.Empty;
		return true;
	}

	public void SetActiveWhenAboveThreshold(bool activeWhenAboveThreshold)
	{
		_runtimeActiveWhenAboveThreshold = activeWhenAboveThreshold;
		Changed = true;
		ApplySignalValue(CurrentValue);
	}

	private void LoadRuntimeConfiguration(XElement root)
	{
		var runtimeSourceId = root.Element("RuntimeSourceComponentId");
		if (runtimeSourceId is not null)
		{
			_runtimeBinding = new LocalSignalBinding(
				long.TryParse(runtimeSourceId.Value, out var sourceId) ? sourceId : 0L,
				root.Element("RuntimeSourceComponentName")?.Value ?? string.Empty,
				SignalComponentUtilities.NormaliseSignalEndpointKey(root.Element("RuntimeSourceEndpointKey")?.Value));
		}

		if (double.TryParse(root.Element("RuntimeActivationThreshold")?.Value, out var activationThreshold))
		{
			_runtimeActivationThreshold = activationThreshold;
		}

		if (bool.TryParse(root.Element("RuntimeActiveWhenAboveThreshold")?.Value, out var activeWhenAboveThreshold))
		{
			_runtimeActiveWhenAboveThreshold = activeWhenAboveThreshold;
		}
	}

	private void SaveRuntimeConfiguration(XElement root)
	{
		if (_runtimeBinding is not null)
		{
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
	}
}
