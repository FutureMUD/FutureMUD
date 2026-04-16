#nullable enable

namespace MudSharp.GameItems.Interfaces;

public sealed record LocalSignalBinding(
	long SourceItemId,
	string SourceItemName,
	long SourceComponentId,
	string SourceComponentName,
	string SourceEndpointKey)
{
	public LocalSignalBinding(long sourceComponentId, string sourceComponentName, string sourceEndpointKey)
		: this(0L, string.Empty, sourceComponentId, sourceComponentName, sourceEndpointKey)
	{
	}
}

public sealed record MicrocontrollerRuntimeInputBinding(
	string VariableName,
	LocalSignalBinding Binding,
	double CurrentValue);
