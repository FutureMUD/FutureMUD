#nullable enable
using MudSharp.Magic;

namespace MudSharp.Body.Implementations;

public partial class Body
{
	private bool _hadEquippedSpellBonuses;
	internal void ReconcileEquippedSpellBonuses()
	{
		var active = EquippedItemBonuses.Bonuses(Actor).Any();
		if (_hadEquippedSpellBonuses || active) ReconcileEmotionalStaminaCapacity();
		_hadEquippedSpellBonuses = active;
	}
}
