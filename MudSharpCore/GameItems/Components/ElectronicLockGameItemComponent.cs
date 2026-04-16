#nullable enable

using MudSharp.Computers;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class ElectronicLockGameItemComponent : ProgLockGameItemComponent, ISignalSinkComponent
{
	private ElectronicLockGameItemComponentProto _prototype;
	private ISignalSourceComponent? _source;

	public ElectronicLockGameItemComponent(ElectronicLockGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public ElectronicLockGameItemComponent(MudSharp.Models.GameItemComponent component,
		ElectronicLockGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
	}

	public ElectronicLockGameItemComponent(ElectronicLockGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public string SourceComponentName => _prototype.SourceComponentName;
	public ISignalSource? UpstreamSource => _source;
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
		DetachSource();
		base.Delete();
	}

	public override void Quit()
	{
		DetachSource();
		base.Quit();
	}

	public void ReconnectSource()
	{
		DetachSource();
		_source = SignalComponentUtilities.FindSignalSource(Parent, SourceComponentName, this);
		if (_source is null)
		{
			return;
		}

		_source.SignalChanged += HandleSourceChanged;
		ReceiveSignal(_source.CurrentSignal, _source);
	}

	public void ReceiveSignal(ComputerSignal signal, ISignalSource source)
	{
		CurrentValue = signal.Value;
		var shouldLock =
			SignalComponentUtilities.IsActiveSignal(signal.Value, _prototype.ActivationThreshold,
				_prototype.LockWhenAboveThreshold);
		SetLocked(shouldLock, true);
	}

	private void HandleSourceChanged(ISignalSourceComponent source, ComputerSignal signal)
	{
		ReceiveSignal(signal, source);
	}

	private void DetachSource()
	{
		if (_source is null)
		{
			return;
		}

		_source.SignalChanged -= HandleSourceChanged;
		_source = null;
	}
}
