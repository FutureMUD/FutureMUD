using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.RPG.Merits {
    public interface ICharacterMerit : IMerit {
        IFutureProg ApplicabilityProg { get; }
        IFutureProg ChargenAvailableProg { get; }
        ICharacterMerit ParentMerit { get;}
        string ChargenBlurb { get; }
        string DatabaseType { get; }
        bool ChargenAvailable(IChargen chargen);
        int ResourceCost(IChargenResource resource);
        int ResourceRequirement(IChargenResource resource);
        bool Applies(IHaveMerits owner, IPerceivable target);
        bool DisplayInCharacterMeritsCommand(ICharacter character);
    }
}