using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using System.Collections.Generic;

namespace MudSharp.CharacterCreation
{
    public interface IChargenScreenStoryboard : ISaveable, IFrameworkItem
    {
        ChargenStage Stage { get; }
        IChargenScreen GetScreen(IChargen chargen);
        // A read-only list of the choices currently eligible for the supplied draft, when the
        // storyboard can safely provide one without constructing or advancing a chargen screen.
        IEnumerable<string> GetDiscordOptions(IChargen chargen) => null;
        IEnumerable<(IChargenResource Resource, int Cost)> ChargenCosts(IChargen chargen);
        string Show(ICharacter voyeur);
        bool BuildingCommand(ICharacter actor, StringStack command);
    }
}
