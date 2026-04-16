#nullable enable

using System.Collections.Generic;

namespace MudSharp.GameItems.Interfaces;

public interface IRuntimeProgrammableMicrocontroller : IMicrocontroller
{
	string LogicText { get; }
	string CompileError { get; }
	bool LogicCompiles { get; }
	IReadOnlyCollection<MicrocontrollerRuntimeInputBinding> InputBindings { get; }
	bool SetLogicText(string logicText, out string error);
	bool SetInputBinding(string variableName, ISignalSourceComponent source, string? endpointKey, out string error);
	bool RemoveInputBinding(string variableName, out string error);
}
