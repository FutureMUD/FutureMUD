#nullable enable

using MudSharp.Events;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Interfaces;

public interface IMagicItemEventEffect : IHandleEventsEffect
{
	EventType? ItemEventType { get; }
	IFutureProg? ItemEventProg { get; }
}
