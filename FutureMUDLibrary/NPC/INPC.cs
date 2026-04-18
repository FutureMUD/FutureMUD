using MudSharp.Character;
using MudSharp.NPC.AI;
using MudSharp.NPC.Templates;
using System.Collections.Generic;

namespace MudSharp.NPC
{
    public interface INPC : ICharacter
    {
        INPCTemplate Template { get; }
        IEnumerable<IArtificialIntelligence> AIs { get; }
        void AddAI(IArtificialIntelligence ai);
        void RemoveAI(IArtificialIntelligence ai);

        void SetupEventSubscriptions();
        void ReleaseEventSubscriptions();
        long? BodyguardingCharacterID { get; set; }
    }
}
