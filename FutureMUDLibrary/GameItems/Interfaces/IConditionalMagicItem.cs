using MudSharp.Character;
using MudSharp.Magic;
using System.Collections.Generic;
using MudSharp.Framework;

#nullable enable
namespace MudSharp.GameItems.Interfaces;

/// <summary>A locally accessible, authored source of capability-scoped spell choices.</summary>
public interface ISpellSelectionShrine : IGameItemComponent
{
	long CapabilityId { get; }
	bool SelectSpells(ICharacter actor, IReadOnlyList<long> spells, out string reason);
}

/// <summary>A worn conditional binding. Identity is its configured group, never item prose.</summary>
public interface IConditionalBindingItem : IGameItemComponent
{
	string BindingGroup { get; }
	bool IsBinding(ICharacter wearer);
	bool PreventRemoval(ICharacter wearer, bool applyConsequence);
	bool InterceptSpell(ICharacter wearer, IMagicSpell spell, IReadOnlyList<IPerceivable> targets);
	bool Activate(ICharacter controller, ICharacter wearer, out string reason);
}
