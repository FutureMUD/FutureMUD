#nullable enable

namespace MudSharp.Body.Implementations;

public partial class Body
{
	/// <summary>Use the world's existing capacity prog; do not refill stamina or reset exertion/ticks.</summary>
	internal void ReconcileEmotionalStaminaCapacity()
	{
		var capacity = Character.Character.MaximumStaminaFor(Actor);
		if (!double.IsFinite(capacity) || capacity < 0)
			throw new InvalidOperationException("The native maximum-stamina prog returned an invalid capacity.");
		MaximumStamina = capacity;
		CurrentStamina = Math.Min(CurrentStamina, capacity);
	}
}
