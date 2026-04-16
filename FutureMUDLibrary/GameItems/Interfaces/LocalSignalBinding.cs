#nullable enable

namespace MudSharp.GameItems.Interfaces;

public sealed record LocalSignalBinding(long SourceComponentId, string SourceComponentName, string SourceEndpointKey);

public sealed record MicrocontrollerRuntimeInputBinding(
	string VariableName,
	LocalSignalBinding Binding,
	double CurrentValue);
