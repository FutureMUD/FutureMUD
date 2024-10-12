using MudSharp.Effects;

namespace MudSharp.Events;

public interface IHandleEventsEffect : IEffect
{
	bool HandleEvent(EventType type, params dynamic[] arguments);
	bool HandlesEvent(params EventType[] types);
}