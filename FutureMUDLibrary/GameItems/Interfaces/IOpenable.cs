using MudSharp.Body;
using MudSharp.Framework;

namespace MudSharp.GameItems.Interfaces {
    public enum WhyCannotOpenReason {
        AlreadyOpen,
        Locked,
        Jammed,
        NotOpenable,
        Unknown,
        AlternateMechanism
    }

    public enum WhyCannotCloseReason {
        AlreadyClosed,
        Locked,
        Jammed,
        NotOpenable,
        SingleUse,
        Unknown
    }

    public delegate void OpenableEvent(IOpenable openable);

    public interface IOpenable : IGameItemComponent {
        bool IsOpen { get; }
        bool CanOpen(IBody opener);
        WhyCannotOpenReason WhyCannotOpen(IBody opener);
        void Open();
        bool CanClose(IBody closer);
        WhyCannotCloseReason WhyCannotClose(IBody closer);
        void Close();

        event OpenableEvent OnOpen;
        event OpenableEvent OnClose;
    }
}