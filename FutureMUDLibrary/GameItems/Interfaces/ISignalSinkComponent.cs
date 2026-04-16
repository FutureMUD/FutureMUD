#nullable enable

using MudSharp.Computers;

namespace MudSharp.GameItems.Interfaces;

public interface ISignalSinkComponent : IGameItemComponent, ISignalSink
{
	string SourceComponentName { get; }
	void ReconnectSource();
}
