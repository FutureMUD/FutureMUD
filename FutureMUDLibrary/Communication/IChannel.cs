using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Communication {
    public interface IChannel : IFrameworkItem, IHaveFuturemud {
        IEnumerable<string> CommandWords { get; }
        void Send(ICharacter source, string message);
        bool Ignore(ICharacter character);
        bool Acknowledge(ICharacter character);
    }
}