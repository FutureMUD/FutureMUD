using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.NPC.AI;

namespace MudSharp.NPC.Templates {
#nullable enable
    public interface INPCTemplate : IEditableRevisableItem {
        string NPCTemplateType { get; }
        IFutureProg? OnLoadProg { get; }
        IHealthStrategy? HealthStrategy { get; }
        List<IArtificialIntelligence> ArtificialIntelligences { get; }
        ICharacterTemplate GetCharacterTemplate(ICell? cell = null);
        ICharacter CreateNewCharacter(ICell location);
        INPCTemplate Clone(ICharacter builder);
        string ReferenceDescription(IPerceiver voyeur);
    }
}