#nullable enable

using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.NPC.AI;

/// <summary>A single native teleport's exact participants and source/destination membership gaps.</summary>
internal sealed class NativeDisplacementReceipt : IDisposable
{
	private enum Phase { Source, Leaving, Entering, Arrived }
	private sealed class Participant(ICharacter actor)
	{
		internal readonly ICharacter Actor = actor;
		internal readonly IBody Body = actor.Body;
		internal readonly ICombat? Combat = actor.Combat;
		internal readonly ICell Source = actor.Location;
		internal readonly RoomLayer SourceLayer = actor.RoomLayer;
		internal readonly double? SourcePosition = actor.RoutePositionMetres;
		internal Phase State;
	}

	private readonly CommandExecutionScope _owner;
	private readonly List<Participant> _participants;
	internal ICell Destination { get; }
	internal RoomLayer Layer { get; }
	internal double? RoutePosition { get; }
	private bool _completed;
	private bool _disposed;
	private readonly List<(IGameItem Item, ICell Source, RoomLayer Layer, double? Position, Action Restore)> _items = [];

	internal NativeDisplacementReceipt(CommandExecutionScope owner, ICharacter actor, ICell destination, RoomLayer layer, double? routePosition)
	{
		_owner = owner;
		Destination = destination;
		Layer = layer;
		RoutePosition = destination.RouteDefinition is null ? routePosition : routePosition ?? destination.RouteDefinition.DefaultPositionMetres;
		_participants = [new Participant(actor)];
	}

	internal bool CaptureCompanions(IEnumerable<ICharacter> companions)
	{
		if (!Continue()) return false;
		foreach (var companion in companions)
		{
			if (_participants.Any(x => ReferenceEquals(x.Actor, companion))) continue;
			_participants.Add(new Participant(companion));
		}
		return Continue();
	}

	internal bool Continue() => !_disposed && _owner.ContinueDisplacement(this);
	internal bool CaptureItems(IEnumerable<IGameItem> items)
	{
		if (!Continue()) return false;
		foreach (var item in items)
		{
			if (item.Location is not ICustodyRollbackLocation source) return false;
			_items.Add((item, item.Location, item.RoomLayer, item.RoutePositionMetres,
				source.CaptureCustodyMembershipRollback([item])));
		}
		return Continue();
	}
	internal bool ExecutorsValid() => _participants.All(x =>
		ReferenceEquals(x.Actor.Body, x.Body) &&
		CommandExecutionAuthority.IsCurrent(x.Actor, false, this) &&
		ReferenceEquals(x.Actor.Combat, x.Combat) &&
		(x.Combat is null || x.Combat.Combatants.Any(c => ReferenceEquals(c, x.Actor))) &&
		(x.State == Phase.Source ? AtSource(x) :
		 x.State == Phase.Arrived ? AtDestination(x) : AtSource(x) || AtDestination(x)));

	private static bool AtSource(Participant x) => ReferenceEquals(x.Actor.Location, x.Source) &&
		x.Actor.RoomLayer == x.SourceLayer && x.Actor.RoutePositionMetres == x.SourcePosition;
	private bool AtDestination(Participant x) => ReferenceEquals(x.Actor.Location, Destination) &&
		x.Actor.RoomLayer == Layer && (Destination.RouteDefinition is null || x.Actor.RoutePositionMetres == RoutePosition);

	internal bool PermitsMembershipGap(ICharacter actor) => !_disposed && _participants.Any(x =>
		ReferenceEquals(x.Actor, actor) && x.State is Phase.Leaving or Phase.Entering &&
		ReferenceEquals(actor.Body, x.Body) && ReferenceEquals(actor.Location, x.Source) &&
		!x.Source.Characters.Any(c => ReferenceEquals(c, actor)));

	internal bool BeginLeave(ICharacter actor)
	{
		var participant = _participants.SingleOrDefault(x => ReferenceEquals(x.Actor, actor));
		if (participant is null || participant.State != Phase.Source || !Continue()) return false;
		participant.State = Phase.Leaving;
		return true;
	}

	internal bool BeginEnter(ICharacter actor, ICell destination)
	{
		var participant = _participants.SingleOrDefault(x => ReferenceEquals(x.Actor, actor));
		if (!ReferenceEquals(destination, Destination) || participant is null ||
		    participant.State != Phase.Leaving || !Continue()) return false;
		participant.State = Phase.Entering;
		return true;
	}

	internal bool AfterMoveTo(ICharacter actor)
	{
		var participant = _participants.Single(x => ReferenceEquals(x.Actor, actor));
		if (ReferenceEquals(actor.Location, Destination) && Destination.Characters.Any(x => ReferenceEquals(x, actor)))
		{
			participant.State = Phase.Arrived;
			CommandExecutionScope.MarkCommitted();
		}
		return Continue();
	}

	internal bool Complete()
	{
		_completed = _participants.All(x => x.State == Phase.Arrived) && Continue();
		return _completed;
	}

	public void Dispose()
	{
		if (_disposed) return;
		// Membership repair never authorizes callbacks, travel, fresh commands or a retired actor.
		foreach (var participant in _participants)
		{
			var actor = participant.Actor;
			// Repair checks current canonical liveness, independently of ordered body/grant validity.
			var live = CommandExecutionAuthority.IsCurrent(actor, false, this);
			var canonical = actor.Location;
			if (!live)
			{
				// IsCurrent can fail solely because of an interrupted membership gap. The explicit
				// identity-only helper below separates that case from actual retirement.
				live = CommandExecutionAuthority.IsCurrentWithoutCellMembership(actor);
			}
			if (live && ReferenceEquals(canonical, participant.Source) && participant.State != Phase.Source && actor is PerceivedItem nativeActor)
				nativeActor.RestoreInterruptedNativePosition(participant.Source, participant.SourceLayer, participant.SourcePosition);
			if (participant.Source is Cell source) source.ReconcileNativeCharacterMembership(actor, live && ReferenceEquals(canonical, source));
			if (Destination is Cell destination && !ReferenceEquals(destination, participant.Source))
				destination.ReconcileNativeCharacterMembership(actor, live && ReferenceEquals(canonical, destination));
		}
		_disposed = true;
		foreach (var entry in _items)
		{
			if (!entry.Item.Deleted && ReferenceEquals(entry.Item.Location, entry.Source) &&
			    entry.Item.Gameworld.Items.Any(x => ReferenceEquals(x, entry.Item)))
			{
				entry.Restore();
				if (entry.Item is PerceivedItem nativeItem)
					nativeItem.RestoreInterruptedNativePosition(entry.Source, entry.Layer, entry.Position);
			}
		}
		_owner.EndDisplacement(this, _completed);
	}
}
