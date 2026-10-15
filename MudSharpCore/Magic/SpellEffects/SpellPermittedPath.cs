#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;

namespace MudSharp.Magic.SpellEffects;

/// <summary>A bounded native route query. It neither opens doors nor substitutes travel for route information.</summary>
internal static class SpellPermittedPath
{
	public static bool Admitted(ICharacter caster, IRoomExit edge) => edge.Exit.Id > 0 &&
		edge.Exit.Door?.IsOpen != false && caster.CanSee(edge.Exit) && caster.CanSee(edge.Origin, edge) &&
		caster.CanMoveForPathPlanning(edge) && caster.CanCross(edge).Success;

	internal static Func<bool> CaptureRawPermissions(ICharacter caster, IReadOnlyList<IRoomExit> path)
	{
		var checks = new List<Func<bool>>();
		void Value<T>(Func<T> read) { var before = read(); checks.Add(() => EqualityComparer<T>.Default.Equals(before, read())); }
		void Sequence<T>(Func<IEnumerable<T>> read) where T : class
		{ var before = read().ToArray(); checks.Add(() => before.SequenceEqual(read(), ReferenceEqualityComparer.Instance)); }
		void Effects(IPerceivable owner)
		{
			Sequence(() => owner.Effects);
			foreach (var effect in owner.Effects)
			{
				var prog = effect.ApplicabilityProg; checks.Add(() => ReferenceEquals(prog, effect.ApplicabilityProg));
				Value(() => prog?.FunctionText);
				if (effect is IExitHiddenEffect hidden) { var exit = hidden.Exit; checks.Add(() => ReferenceEquals(hidden.Exit, exit)); Value(() => hidden.HiddenTypes); }
				if (effect is IExitBarrierTraversalEffect barrier) Value(() => barrier.IsActive);
				if (effect is IMagicSpellEffect child)
				{
					var parent = child.ParentEffect; checks.Add(() => ReferenceEquals(child.ParentEffect, parent));
					if (parent is not null) Sequence(() => parent.SpellEffects);
				}
				if (effect is IGuardExitEffect guard)
				{
					var exit = guard.Exit; checks.Add(() => ReferenceEquals(guard.Exit, exit));
					Value(() => guard.PermitAllies); Value(() => string.Join("\0", guard.Exemptions));
				}
			}
		}
		var party = caster.Party; checks.Add(() => ReferenceEquals(caster.Party, party));
		if (party is not null) Sequence(() => party.ActiveCharacterMembers);
		foreach (var room in path.Select(x => x.Origin).Distinct<IRoom>(ReferenceEqualityComparer.Instance))
		{
			Effects(room); Sequence(() => room.Characters);
			var characters = room.Characters.Take(513).ToArray();
			if (characters.Length > 512) throw new InvalidOperationException("The permitted route exceeded its bounded guard snapshot.");
			foreach (var actor in characters)
			{
				Effects(actor); Value(() => actor.RoomLayer); var location = actor.Location;
				checks.Add(() => ReferenceEquals(actor.Location, location)); Value(() => string.Join(",", actor.AllyIDs));
			}
		}
		foreach (var edge in path)
		{
			Effects(edge.Exit);
			Value(() => edge.Exit.MaximumSizeToEnter); Value(() => edge.Exit.MaximumSizeToEnterUpright);
			Value(() => edge.Exit.TimeMultiplier); Value(() => edge.IsClimbExit); Value(() => edge.IsFlyExit); Value(() => edge.IsFallExit);
			Value(() => edge.ClimbDifficulty); Value(() => string.Join(",", edge.Exit.BlockedLayers));
		}
		return () => checks.All(x => x());
	}
	public static IReadOnlyList<IRoomExit>? Find(ICharacter caster, IRoom source, IRoom destination, int maximum)
	{
		if (maximum is < 1 or > 140) throw new ArgumentOutOfRangeException(nameof(maximum));
		if (ReferenceEquals(source, destination)) return [];
		var queue = new Queue<(IRoom Room, int Depth)>(); queue.Enqueue((source, 0));
		var previous = new Dictionary<IRoom, IRoomExit?>(ReferenceEqualityComparer.Instance) { [source] = null };
		var scanned = 0;
		while (queue.TryDequeue(out var current))
		{
			if (current.Depth >= maximum) continue;
			var exits = current.Room.ExitsFor(caster).Take(513).ToArray();
			if (exits.Length > 512) throw new InvalidOperationException("The permitted route query exceeded its exit bound.");
			foreach (var edge in exits)
			{
				if (++scanned > 65536 || previous.Count > 8192) throw new InvalidOperationException("The permitted route query exceeded its bounded search budget.");
				if (!ReferenceEquals(edge.Origin, current.Room) || previous.ContainsKey(edge.Destination) || !Admitted(caster, edge)) continue;
				previous.Add(edge.Destination, edge);
				if (ReferenceEquals(edge.Destination, destination))
				{
					var path = new List<IRoomExit>(); var room = destination;
					while (!ReferenceEquals(room, source)) { var step = previous[room]!; path.Add(step); room = step.Origin; }
					path.Reverse(); return path.AsReadOnly();
				}
				queue.Enqueue((edge.Destination, current.Depth + 1));
			}
		}
		return null;
	}
}
