using System;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Events.Hooks {
    public interface IHook : IFrameworkItem, IHaveFuturemud, ISaveable {
        Func<EventType, object[], bool> Function { get; }
        EventType Type { get; }
        string Category { get; set; }
        string InfoForHooklist { get; }
    }
}