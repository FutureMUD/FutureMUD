using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;

namespace MudSharp.Communication {
    public interface IChannel : IFrameworkItem, IHaveFuturemud, IEditableItem {
        IEnumerable<string> CommandWords { get; }
        void Send(ICharacter source, string message);
        bool Ignore(ICharacter character);
        bool Acknowledge(ICharacter character);
    }
}