using System.Collections.Generic;
using MudSharp.CharacterCreation.Resources;

namespace MudSharp.CharacterCreation {
    public enum ChargenScreenState {
        Incomplete,
        PartiallyComplete,
        Complete
    }

    public interface IChargenScreen {
        ChargenScreenState State { get; }
        ChargenStage AssociatedStage { get; }
        IChargenScreen NextScreen { get; }
        string Display();
        string HandleCommand(string command);
    }
}