#nullable enable

using MudSharp.Computers;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class MicrocontrollerGameItemComponent : PoweredMachineBaseGameItemComponent, IRuntimeProgrammableMicrocontroller
{
	private readonly Dictionary<string, ISignalSourceComponent> _inputSources =
		new(StringComparer.InvariantCultureIgnoreCase);
	private readonly Dictionary<string, double> _inputValues =
		new(StringComparer.InvariantCultureIgnoreCase);
	private readonly List<MicrocontrollerRuntimeInputBinding> _inputBindings = [];
	private ComputerSignal _currentSignal;
	private MicrocontrollerGameItemComponentProto _prototype;
	private IFutureProg? _compiledLogic;
	private string _logicText = string.Empty;
	private string _compileError = string.Empty;

	public MicrocontrollerGameItemComponent(MicrocontrollerGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
		InitialiseRuntimeStateFromPrototype();
	}

	public MicrocontrollerGameItemComponent(MudSharp.Models.GameItemComponent component,
		MicrocontrollerGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
		LoadRuntimeConfiguration(XElement.Parse(component.Definition));
	}

	public MicrocontrollerGameItemComponent(MicrocontrollerGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_currentSignal = rhs._currentSignal;
		_logicText = rhs._logicText;
		_compileError = rhs._compileError;
		_compiledLogic = rhs._compiledLogic;
		_inputBindings.AddRange(rhs._inputBindings);
		foreach (var item in rhs._inputValues)
		{
			_inputValues[item.Key] = item.Value;
		}
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public long LocalSignalSourceIdentifier => Prototype.Id;
	public string EndpointKey => SignalComponentUtilities.DefaultLocalSignalEndpointKey;
	public ComputerSignal CurrentSignal => _currentSignal;
	public event SignalChangedEvent? SignalChanged;
	public double CurrentValue => _currentSignal.Value;
	public TimeSpan? Duration => _currentSignal.Duration;
	public TimeSpan? PulseInterval => _currentSignal.PulseInterval;
	public IReadOnlyDictionary<string, double> Inputs => new ReadOnlyDictionary<string, double>(_inputValues);
	public string LogicText => _logicText;
	public string CompileError => _compileError;
	public bool LogicCompiles => _compiledLogic is not null && string.IsNullOrEmpty(_compileError);
	public IReadOnlyCollection<MicrocontrollerRuntimeInputBinding> InputBindings => _inputBindings.AsReadOnly();

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

	private void LoadRuntimeConfiguration(XElement root)
	{
		var logicElement = root.Element("LogicText");
		var bindings = root.Elements("InputBinding")
			.Select(x => new MicrocontrollerRuntimeInputBinding(
				x.Attribute("variable")?.Value ?? string.Empty,
				new LocalSignalBinding(
					long.TryParse(x.Attribute("sourceid")?.Value, out var sourceId) ? sourceId : 0L,
					x.Attribute("source")?.Value ?? string.Empty,
					SignalComponentUtilities.NormaliseSignalEndpointKey(x.Attribute("endpoint")?.Value)),
				0.0))
			.ToList();

		if (logicElement is null && !bindings.Any())
		{
			InitialiseRuntimeStateFromPrototype();
			return;
		}

		_logicText = logicElement?.Value ?? _prototype.LogicText;
		_inputBindings.Clear();
		_inputBindings.AddRange(bindings);
		(_compiledLogic, _compileError) = CompileLogic(_logicText, _inputBindings.Select(x => x.VariableName));
		SeedInputValues();
	}

	protected override XElement SaveToXml(XElement root)
	{
		root.Add(new XElement("LogicText", new XCData(_logicText)));
		root.Add(_inputBindings.Select(x => new XElement("InputBinding",
			new XAttribute("variable", x.VariableName),
			new XAttribute("sourceid", x.Binding.SourceComponentId),
			new XAttribute("source", x.Binding.SourceComponentName),
			new XAttribute("endpoint", x.Binding.SourceEndpointKey))));
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
		SeedInputValues();
		foreach (var input in _inputBindings)
		{
			var source = SignalComponentUtilities.FindSignalSource(Parent, input.Binding.SourceComponentId,
				input.Binding.SourceComponentName, input.Binding.SourceEndpointKey, this);
			if (source is null)
			{
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
		if (!SwitchedOn || !_onAndPowered || _compiledLogic is null)
		{
			SetOutput(default);
			return;
		}

		var arguments = _inputBindings
			.Select(x => (object)(decimal)(_inputValues.TryGetValue(x.VariableName, out var value) ? value : 0.0))
			.ToArray();

		var result = (double)_compiledLogic.ExecuteDecimal(0.0M, arguments);
		SetOutput(new ComputerSignal(result, null, null));
	}

	public bool SetLogicText(string logicText, out string error)
	{
		var body = logicText?.Trim() ?? string.Empty;
		var (prog, compileError) = CompileLogic(body, _inputBindings.Select(x => x.VariableName));
		if (prog is null)
		{
			error = compileError;
			return false;
		}

		_logicText = body;
		_compiledLogic = prog;
		_compileError = string.Empty;
		Changed = true;
		RecomputeOutput();
		error = string.Empty;
		return true;
	}

	public bool SetInputBinding(string variableName, ISignalSourceComponent source, string? endpointKey, out string error)
	{
		if (ReferenceEquals(source, this))
		{
			error = "A microcontroller cannot bind one of its inputs directly to its own output.";
			return false;
		}

		var normalisedVariableName = variableName?.Trim().ToLowerInvariant() ?? string.Empty;
		if (!MicrocontrollerLogicCompiler.IsValidVariableName(normalisedVariableName))
		{
			error = "That is not a valid microcontroller input variable name.";
			return false;
		}

		var binding = new MicrocontrollerRuntimeInputBinding(
			normalisedVariableName,
			SignalComponentUtilities.CreateBinding(source, endpointKey),
			0.0);
		var updatedBindings = _inputBindings
			.Where(x => !x.VariableName.Equals(normalisedVariableName, StringComparison.InvariantCultureIgnoreCase))
			.ToList();
		updatedBindings.Add(binding);

		var (prog, compileError) = CompileLogic(_logicText, updatedBindings.Select(x => x.VariableName));
		if (prog is null)
		{
			error = compileError;
			return false;
		}

		_inputBindings.Clear();
		_inputBindings.AddRange(updatedBindings.OrderBy(x => x.VariableName, StringComparer.InvariantCultureIgnoreCase));
		_compiledLogic = prog;
		_compileError = string.Empty;
		Changed = true;
		ReconnectSources();
		RecomputeOutput();
		error = string.Empty;
		return true;
	}

	public bool RemoveInputBinding(string variableName, out string error)
	{
		var binding = _inputBindings.FirstOrDefault(x =>
			x.VariableName.Equals(variableName, StringComparison.InvariantCultureIgnoreCase));
		if (binding is null)
		{
			error = "There is no such input binding.";
			return false;
		}

		var updatedBindings = _inputBindings
			.Where(x => !x.VariableName.Equals(variableName, StringComparison.InvariantCultureIgnoreCase))
			.ToList();
		var (prog, compileError) = CompileLogic(_logicText, updatedBindings.Select(x => x.VariableName));
		if (prog is null)
		{
			error = compileError;
			return false;
		}

		_inputBindings.Clear();
		_inputBindings.AddRange(updatedBindings);
		_compiledLogic = prog;
		_compileError = string.Empty;
		Changed = true;
		ReconnectSources();
		RecomputeOutput();
		error = string.Empty;
		return true;
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

	private void InitialiseRuntimeStateFromPrototype()
	{
		_logicText = _prototype.LogicText;
		_compileError = _prototype.CompileError;
		_compiledLogic = _prototype.CompiledLogic;
		_inputBindings.Clear();
		_inputBindings.AddRange(_prototype.Inputs.Select(x => new MicrocontrollerRuntimeInputBinding(
			x.VariableName,
			new LocalSignalBinding(x.SourceComponentId, x.SourceComponentName, x.SourceEndpointKey),
			0.0)));
		SeedInputValues();
	}

	private void SeedInputValues()
	{
		_inputValues.Clear();
		foreach (var input in _inputBindings)
		{
			_inputValues[input.VariableName] = 0.0;
		}
	}

	private (IFutureProg? Prog, string Error) CompileLogic(string logicText, IEnumerable<string> variableNames)
	{
		return MicrocontrollerLogicCompiler.Compile(
			Gameworld,
			$"microcontroller_runtime_{Id}_{DateTime.UtcNow.Ticks}",
			variableNames,
			logicText);
	}
}
