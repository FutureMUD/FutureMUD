using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.NPC.AI;
using MudSharp.NPC.Templates;

namespace MudSharp.NPC {
    public interface INPC : ICharacter {
        INPCTemplate Template { get; }
        IEnumerable<IArtificialIntelligence> AIs { get; }
        void AddAI(IArtificialIntelligence ai);
        void RemoveAI(IArtificialIntelligence ai);

        void SetupEventSubscriptions();
        void ReleaseEventSubscriptions();
        long? BodyguardingCharacterID { get; set; }
    }
}
