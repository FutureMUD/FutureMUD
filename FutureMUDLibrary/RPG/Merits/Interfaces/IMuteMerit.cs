using MudSharp.PerceptionEngine;

namespace MudSharp.RPG.Merits.Interfaces {
    public interface IMuteMerit : ICharacterMerit {
        PermitLanguageOptions LanguageOptions { get; }
    }
}