using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.NPC.AI;
using System.Collections.Generic;

namespace MudSharp.NPC.Templates
{
#nullable enable
    public interface INPCTemplate : IEditableRevisableItem
    {
        string NPCTemplateType { get; }
        IFutureProg? OnLoadProg { get; }
        IHealthStrategy? HealthStrategy { get; }
        ICharacterCombatSettings? DefaultCombatSetting { get; }
        List<IArtificialIntelligence> ArtificialIntelligences { get; }
        ICharacterTemplate GetCharacterTemplate(ICell? cell = null);
        ICharacter CreateNewCharacter(ICell location);
        INPCTemplate Clone(ICharacter builder);
        string ReferenceDescription(IPerceiver voyeur);
    }
}
