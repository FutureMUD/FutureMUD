using MudSharp.Events.Hooks;

namespace MudSharp.Events {
    public delegate bool FutureMUDEventHandler(EventType type, params dynamic[] arguments);

    public interface IHandleEvents {
        bool HooksChanged { get; set; }
        bool HandleEvent(EventType type, params dynamic[] arguments);
        bool HandlesEvent(params EventType[] types);
        bool InstallHook(IHook hook);
        bool RemoveHook(IHook hook);
    }
}