#nullable enable

using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MudSharp.Framework.Scheduling;

namespace MudSharp.Framework;

public partial class Futuremud
{
	private ISpellOwnedNpcService? _spellOwnedNpcs;
	public ISpellOwnedNpcService SpellOwnedNpcs => _spellOwnedNpcs ??= new SpellOwnedNpcService(this);

	private void ReconcileSpellOwnedNpcDeaths()
	{
		try { SpellOwnedNpcs.ReconcileRetirements(RuntimeClock.UtcNow); }
		catch (Exception ex) { SystemMessage("Spell-owned NPC death reconciliation needs attention: " + ex.Message, true); }
	}
}
