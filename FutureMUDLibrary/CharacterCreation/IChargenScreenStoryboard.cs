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
        IEnumerable<(IChargenResource Resource, int Cost)> ChargenCosts(IChargen chargen);
        string Show(ICharacter voyeur);
        bool BuildingCommand(ICharacter actor, StringStack command);
    }
}