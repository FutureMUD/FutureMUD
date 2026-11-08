#nullable enable

using MudSharp.Combat;
using MudSharp.Effects.Concrete;
using MudSharp.Movement;

namespace MudSharp.NPC.AI;

internal static class NpcMemoryReactions
{
	public static bool SuppressAttack(ICharacter actor, ICharacter subject) =>
		NpcDispositionMemory.Has(actor, subject, NpcDisposition.Fear) ||
		NpcDispositionMemory.Has(actor, subject, NpcDisposition.Like);

	/// <summary>Uses native combat flight or one ordinary escape step; never relocates an actor directly.</summary>
	public static bool TryAvoidFearedCharacter(ICharacter actor)
	{
		if (actor.EffectsOfType<IHideEffect>().Any()) return false;
		var feared = actor.Effects.OfType<NpcDispositionMemory>()
			.Where(x => x.Disposition == NpcDisposition.Fear && ReferenceEquals(x.Subject.Location, actor.Location) &&
				x.Subject.RoomLayer == actor.RoomLayer && actor.CanSee(x.Subject)).ToArray();
		if (feared.Length == 0) return false;
		if (actor.Combat is not null)
		{
			actor.CombatStrategyMode = CombatStrategyMode.Flee;
			return true;
		}
		if (actor.Movement is not null || !actor.CanMove(CanMoveFlags.None)) return true;
		var exit = actor.Location.ExitsFor(actor)
			.Where(x => actor.CanCross(x).Success && actor.CanMove(x)).GetRandomElement();
		if (exit is not null) actor.Move(exit);
		// Fear takes precedence even when no escape passage is available.
		return true;
	}
}
