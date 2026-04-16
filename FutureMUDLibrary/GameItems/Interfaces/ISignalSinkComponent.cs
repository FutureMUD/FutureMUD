#nullable enable

using MudSharp.Computers;

namespace MudSharp.GameItems.Interfaces;

public interface ISignalSinkComponent : IGameItemComponent, ISignalSink
{
	long SourceComponentId { get; }
	string SourceComponentName { get; }
	void ReconnectSource();
}
