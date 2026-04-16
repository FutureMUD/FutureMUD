#nullable enable

using MudSharp.Computers;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class ElectronicLockGameItemComponent : ProgLockGameItemComponent, ISignalSinkComponent
{
	private ElectronicLockGameItemComponentProto _prototype;
	private readonly LocalSignalSinkSubscription _binding;

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
	}

	public ElectronicLockGameItemComponent(ElectronicLockGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_binding = new LocalSignalSinkSubscription(newParent, this, HandleSourceChanged);
	}

	public long SourceComponentId => _prototype.SourceComponentId;
	public string SourceComponentName => _prototype.SourceComponentName;
	public ISignalSource? UpstreamSource => _binding.UpstreamSource;
	public double CurrentValue { get; private set; }

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ElectronicLockGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (ElectronicLockGameItemComponentProto)newProto;
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
		_binding.Reconnect(SourceComponentId, SourceComponentName);
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
		var shouldLock = SignalComponentUtilities.IsActiveSignal(value, _prototype.ActivationThreshold,
			_prototype.LockWhenAboveThreshold);
		SetLocked(shouldLock, true);
	}

	private void HandleSourceChanged(ISignalSourceComponent source, ComputerSignal signal)
	{
		ReceiveSignal(signal, source);
	}
}
