#nullable enable

using MudSharp.Computers;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class MicrocontrollerGameItemComponent : PoweredMachineBaseGameItemComponent, IMicrocontroller
{
	private readonly Dictionary<string, ISignalSourceComponent> _inputSources =
		new(StringComparer.InvariantCultureIgnoreCase);
	private readonly Dictionary<string, double> _inputValues =
		new(StringComparer.InvariantCultureIgnoreCase);
	private ComputerSignal _currentSignal;
	private MicrocontrollerGameItemComponentProto _prototype;

	public MicrocontrollerGameItemComponent(MicrocontrollerGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public MicrocontrollerGameItemComponent(MudSharp.Models.GameItemComponent component,
		MicrocontrollerGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
	}

	public MicrocontrollerGameItemComponent(MicrocontrollerGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_currentSignal = rhs._currentSignal;
		foreach (var item in rhs._inputValues)
		{
			_inputValues[item.Key] = item.Value;
		}
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public ComputerSignal CurrentSignal => _currentSignal;
	public event SignalChangedEvent? SignalChanged;
	public double CurrentValue => _currentSignal.Value;
	public TimeSpan? Duration => _currentSignal.Duration;
	public TimeSpan? PulseInterval => _currentSignal.PulseInterval;
	public IReadOnlyDictionary<string, double> Inputs => new ReadOnlyDictionary<string, double>(_inputValues);

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new MicrocontrollerGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override int DecorationPriority => 1000;

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags)
	{
		return
			$"{description}\n\nIts microcontroller is {(SwitchedOn ? "switched on".ColourValue() : "switched off".ColourName())} and currently outputting {CurrentValue.ToString("N2", voyeur).ColourValue()}.";
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (MicrocontrollerGameItemComponentProto)newProto;
	}

	protected override XElement SaveToXml(XElement root)
	{
		return root;
	}

	public override void FinaliseLoad()
	{
		ReconnectSources();
		RecomputeOutput();
	}

	public override void Delete()
	{
		DisconnectSources();
		base.Delete();
	}

	public override void Quit()
	{
		DisconnectSources();
		base.Quit();
	}

	protected override void OnPowerCutInAction()
	{
		RecomputeOutput();
	}

	protected override void OnPowerCutOutAction()
	{
		SetOutput(default);
	}

	private void ReconnectSources()
	{
		DisconnectSources();
		foreach (var input in _prototype.Inputs)
		{
			var source = SignalComponentUtilities.FindSignalSource(Parent, input.SourceComponentName, this);
			if (source is null)
			{
				_inputValues[input.VariableName] = 0.0;
				continue;
			}

			_inputSources[input.VariableName] = source;
			_inputValues[input.VariableName] = source.CurrentValue;
			source.SignalChanged += HandleSourceSignalChanged;
		}
	}

	private void DisconnectSources()
	{
		foreach (var source in _inputSources.Values.Distinct())
		{
			source.SignalChanged -= HandleSourceSignalChanged;
		}

		_inputSources.Clear();
	}

	private void HandleSourceSignalChanged(ISignalSourceComponent source, ComputerSignal signal)
	{
		foreach (var item in _inputSources.Where(x => ReferenceEquals(x.Value, source)).ToList())
		{
			_inputValues[item.Key] = signal.Value;
		}

		RecomputeOutput();
	}

	private void RecomputeOutput()
	{
		if (!SwitchedOn || !_onAndPowered || _prototype.CompiledLogic is null)
		{
			SetOutput(default);
			return;
		}

		var arguments = _prototype.Inputs
			.Select(x => (object)(decimal)(_inputValues.TryGetValue(x.VariableName, out var value) ? value : 0.0))
			.ToArray();

		var result = (double)_prototype.CompiledLogic.ExecuteDecimal(0.0M, arguments);
		SetOutput(new ComputerSignal(result, null, null));
	}

	private void SetOutput(ComputerSignal signal)
	{
		if (SignalComponentUtilities.SignalsEqual(_currentSignal, signal))
		{
			return;
		}

		_currentSignal = signal;
		SignalChanged?.Invoke(this, _currentSignal);
	}
}
