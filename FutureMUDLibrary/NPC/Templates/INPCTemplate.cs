using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Construction;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.NPC.AI;

namespace MudSharp.NPC.Templates {
    public interface INPCTemplate : IEditableRevisableItem {
        string NPCTemplateType { get; }
        IFutureProg OnLoadProg { get; }
        List<IArtificialIntelligence> ArtificialIntelligences { get; }
        ICharacterTemplate GetCharacterTemplate(ICell cell = null);
        ICharacter CreateNewCharacter(ICell location);
    }
}