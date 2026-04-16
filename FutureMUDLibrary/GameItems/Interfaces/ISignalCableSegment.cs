#nullable enable

namespace MudSharp.GameItems.Interfaces;

public interface ISignalCableSegment : ISignalSourceComponent
{
	LocalSignalBinding CurrentBinding { get; }
	long RoutedExitId { get; }
	bool IsRouted { get; }
	bool ConfigureRoute(ISignalSourceComponent source, long exitId, out string error);
	void ClearRoute();
}
