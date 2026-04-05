using MudSharp.Character;
using MudSharp.Communication.Language;
using System;

namespace MudSharp.Magic
{
    public interface ISpeakPhraseTrigger : IMagicTrigger
    {
        (bool Applies, Func<ICharacter> CastFunction) CheckTrigger(ICharacter magician, SpokenLanguageInfo speechInfo);
    }
}