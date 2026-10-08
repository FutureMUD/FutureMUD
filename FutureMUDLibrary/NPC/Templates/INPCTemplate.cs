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
    public interface INPCTemplate : IEditableRevisableItem, IProgVariable
    {
        string NPCTemplateType { get; }
        string? UniqueName { get; }
        string? BuilderNotes { get; }
        IFutureProg? OnLoadProg { get; }
        IHealthStrategy? HealthStrategy { get; }
        ICharacterCombatSettings? DefaultCombatSetting { get; }
        List<IArtificialIntelligence> ArtificialIntelligences { get; }
        ICharacterTemplate GetCharacterTemplate(IRoom? room = null);
        ICharacter CreateNewCharacter(IRoom location);
        ICharacter CreateNewCharacter(SpatialLocation location);
		ICharacter CreateSpellOwnedCharacter(SpatialLocation location, MudSharp.Magic.SpellLifecycleOrigin origin);
        IEnumerable<string> ApplyTemplateLoadAdditions(ICharacter character, bool logWarnings = true);
        INPCTemplate Clone(ICharacter builder);
		NPCSkillPackageApplicationResult ApplySkillPackage(INPCSkillPackage package);
        string ReferenceDescription(IPerceiver voyeur);
    }
}
