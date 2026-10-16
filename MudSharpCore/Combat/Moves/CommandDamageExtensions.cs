#nullable enable

using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.NPC.AI;

namespace MudSharp.Combat.Moves;

/// <summary>Check the ordered continuation immediately before each distinct damage operation.</summary>
internal static class CommandDamageExtensions
{
	internal static IEnumerable<IWound> CommandSufferDamage(this IHaveWounds target, IDamage damage)
	{
		if (!CommandExecutionScope.TryContinue()) return [];
		using var transfer = MudSharp.Effects.Concrete.SpellEffects.SpellLeechingEdgeEffect.BeginHit(target, damage, out var fraction);
		var wounds = target.PassiveSufferDamage(transfer is null ? damage : new Damage(damage) { VitalTransfer = transfer }).ToArray();
		if (wounds.Length > 0) CommandExecutionScope.MarkCommitted();
		transfer?.Settle(fraction, double.MaxValue);
		return wounds;
	}

	internal static IEnumerable<IWound> CommandSufferDamage(this IHaveWounds target, IExplosiveDamage damage,
		Proximity proximity, Facing facing)
	{
		if (!CommandExecutionScope.TryContinue()) return [];
		var wounds = target.PassiveSufferDamage(damage, proximity, facing).ToArray();
		if (wounds.Length > 0) CommandExecutionScope.MarkCommitted();
		return wounds;
	}
}
