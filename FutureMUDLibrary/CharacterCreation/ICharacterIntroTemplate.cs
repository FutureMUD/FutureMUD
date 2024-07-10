using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;

namespace MudSharp.CharacterCreation
{
    public interface ICharacterIntroTemplate : IEditableItem
    {
        bool AppliesToCharacter(ICharacterTemplate template);
        bool AppliesToCharacter(ICharacter character);
        IFutureProg AppliesToCharacterProg { get; }
        int ResolutionPriority { get; }
        List<string> Echoes { get; }
        List<TimeSpan> Delays { get; }
        ICharacterIntro GetCharacterIntro();
    }
}