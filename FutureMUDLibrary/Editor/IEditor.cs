using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.Editor {
    public enum EditorStatus {
        Editing,
        Submitted,
        Cancelled
    }

    public interface IEditor : IHandleCommands, IHandleOutput {
        EditorStatus Status { get; }
    }
}