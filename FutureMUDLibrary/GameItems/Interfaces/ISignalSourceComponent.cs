#nullable enable

using MudSharp.Computers;

namespace MudSharp.GameItems.Interfaces;

public delegate void SignalChangedEvent(ISignalSourceComponent source, ComputerSignal signal);

public interface ISignalSourceComponent : IGameItemComponent, ISignalSource
{
	long LocalSignalSourceIdentifier { get; }
	ComputerSignal CurrentSignal { get; }
	event SignalChangedEvent? SignalChanged;
}
