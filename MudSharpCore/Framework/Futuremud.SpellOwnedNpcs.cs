#nullable enable

using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MudSharp.Framework.Scheduling;

namespace MudSharp.Framework;

public partial class Futuremud
{
	private ISpellOwnedNpcService? _spellOwnedNpcs;
	public ISpellOwnedNpcService SpellOwnedNpcs => _spellOwnedNpcs ??= new SpellOwnedNpcService(this);
	private ISpellOwnedItemService? _spellOwnedItems;
	public ISpellOwnedItemService SpellOwnedItems => _spellOwnedItems ??= new SpellOwnedItemService(this);
	private ISpellOwnedCorpseAnimationService? _spellOwnedCorpseAnimations;
	public ISpellOwnedCorpseAnimationService SpellOwnedCorpseAnimations => _spellOwnedCorpseAnimations ??= new SpellOwnedCorpseAnimationService(this);

	private void ReconcileSpellOwnedNpcDeaths()
	{
		try { SpellOwnedNpcs.ReconcileRetirements(RuntimeClock.UtcNow); }
		catch (Exception ex) { SystemMessage("Spell-owned NPC death reconciliation needs attention: " + ex.Message, true); }
		try { SpellOwnedItems.ReconcileRetirements(RuntimeClock.UtcNow); }
		catch (Exception ex) { SystemMessage("Spell-owned item reconciliation needs attention: " + ex.Message, true); }
		try { SpellOwnedCorpseAnimations.ReconcileRetirements(RuntimeClock.UtcNow); }
		catch (Exception ex) { SystemMessage("Corpse animation reconciliation needs attention: " + ex.Message, true); }
	}
}
