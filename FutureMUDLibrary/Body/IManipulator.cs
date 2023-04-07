using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;

namespace MudSharp.Body {
    public interface IManipulator {
        string WhyCannotOpen { get; }
        string WhyCannotClose { get; }
        bool CanOpen(IOpenable openable);
        void Open(IOpenable openable, ICharacter openableOwner, IEmote playerEmote, bool useCouldLogic = false);

        /// <summary>
        /// Could open tests whether the specified IManipulator could theoretically open an IOpenable, including all possible actions they could take to make it so
        /// </summary>
        /// <param name="openable">The openable in question</param>
        /// <returns>True if through some actions taken this openable could be opened</returns>
        bool CouldOpen(IOpenable openable);

        bool CanClose(IOpenable openable);
        void Close(IOpenable openable, ICharacter openableOwner, IEmote playerEmote);

        bool CanConnect(IConnectable connectable, IConnectable other);
        bool Connect(IConnectable connectable, IConnectable other, IPerceivable ownerConnectable = null, IPerceivable ownerOther = null, IEmote playerEmote = null);
        string WhyCannotConnect(IConnectable connectable, IConnectable other);
        bool CanDisconnect(IConnectable connectable, IConnectable other);
        bool Disconnect(IConnectable connectable, IConnectable other, IPerceivable ownerConnectable = null, IPerceivable ownerOther = null, IEmote playerEmote = null);
        string WhyCannotDisconnect(IConnectable connectable, IConnectable other);
    }
}