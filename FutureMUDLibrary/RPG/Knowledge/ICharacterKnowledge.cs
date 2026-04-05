using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using System;

namespace MudSharp.RPG.Knowledge
{
    public interface ICharacterKnowledge : ISaveable, IFrameworkItem
    {
        ICharacter Character { get; set; }
        IKnowledge Knowledge { get; set; }
        DateTime WhenAcquired { get; set; }
        string HowAcquired { get; set; }
        int TimesTaught { get; set; }
        void SetId(long id);
    }
}