using System;

namespace MudSharp.RPG.Merits.Interfaces {
    public interface ISecondWindMerit : ICharacterMerit {
        string Emote { get; }
        string RecoveryMessage { get; }
        TimeSpan RecoveryDuration { get; }
    }
}
