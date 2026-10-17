#nullable enable

using System.Threading;
using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects;
using MudSharp.NPC.AI;
using MudSharp.Planes;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems;

namespace MudSharp.Movement;

/// <summary>Only Movement's admitted action calls Contact. CanCross remains a pure query.</summary>
internal static class ExitBarrierTraversal
{
	private sealed record Query(ICharacter Actor, IRoomExit Exit, IMovement Movement, IExitBarrierTraversalEffect[] Effects);
	private static readonly AsyncLocal<Query?> CurrentQuery = new();
	internal static (bool Success, IEmoteOutput FailureOutput) CanCrossForMovement(ICharacter actor, IRoomExit exit, IMovement movement)
	{
		if (exit?.Exit is not { } boundary) return actor.CanCross(exit!);
		var barriers = boundary.Effects.OfType<IExitBarrierTraversalEffect>()
			.Where(x => x.IsActive && x.OccupiesLayerAndPlane(actor)).ToArray();
		if (barriers.Length == 0) return actor.CanCross(exit);
		var prior = CurrentQuery.Value;
		CurrentQuery.Value = new(actor, exit, movement, barriers);
		try { return actor.CanCross(exit); }
		finally { CurrentQuery.Value = prior; }
	}

	internal static bool Blocks(IExitBarrierEffect barrier, ICharacter actor, IRoomExit exit)
	{
		if (barrier is not IExitBarrierTraversalEffect contact) return true;
		if (!contact.IsActive) return false;
		if (CurrentQuery.Value is { } query && ReferenceEquals(query.Actor, actor) && ReferenceEquals(query.Exit, exit) && ReferenceEquals(actor.Movement, query.Movement) &&
			query.Effects.Any(x => ReferenceEquals(x, contact))) return false;
		return contact.OccupiesLayerAndPlane(actor);
	}

	/// <returns>True if this actor's movement must stop, including invalidated attempts.</returns>
	internal static bool Contact(ICharacter actor, IRoomExit exit, IMovement movement)
	{
		if (exit?.Exit is not { } boundary) return false;
		var barriers = boundary.Effects.OfType<IExitBarrierTraversalEffect>().Where(x => x.IsActive).ToArray();
		if (barriers.Length == 0) return false;
		using var frame = new Attempt(actor, exit, movement);
		if (!frame.IsCurrent) return true;
		barriers = barriers.Where(x => x.OccupiesLayerAndPlane(actor)).ToArray();
		if (!frame.IsCurrent) return true;
		if (barriers.Length == 0 || actor.AffectedBy<IImmwalkEffect>() || actor.RidingMount?.AffectedBy<IImmwalkEffect>() == true)
			return false;
		var prior = CurrentQuery.Value;
		CurrentQuery.Value = new(actor, exit, movement, barriers);
		try
		{
			// Skip only these exact active barriers while asking the existing door/guard rules.
			if (!actor.CanCross(exit).Success) return true;
			if (!frame.Continue()) return true;
			foreach (var barrier in barriers)
			{
				if (!frame.Continue() || !barrier.IsActive) break;
				barrier.Contact(actor, exit, () => frame.Continue() && barrier.IsActive);
			}
			return true;
		}
		catch (InvalidOperationException)
		{
			// Contact was already consumed by Movement; a stale physical operation cannot retry.
			return true;
		}
		finally { CurrentQuery.Value = prior; }
	}

	private sealed class Attempt : IDisposable
	{
		private readonly ICharacter _actor;
		private readonly IBody? _body;
		private readonly IGameItem[] _wornItems;
		private readonly IProsthetic[] _prosthetics;
		private readonly IGameItem[] _prostheticParents;
		private readonly IRoomExit _exit;
		private readonly IExit _boundary;
		private readonly IRoom _origin;
		private readonly IRoom _destination;
		private readonly IFuturemud _world;
		private readonly IMovement _movement;
		private readonly RoomLayer _layer;
		private readonly long _instance;
		private readonly CharacterState _state;
		private readonly ICharacterIdentity? _identity;
		private readonly IEffect[] _actorEffects;
		private readonly IEffect[] _bodyEffects;
		private readonly IEffect[] _exitEffects;
		private readonly string _plane;
		private readonly string _basePlane;
		private readonly IDoor? _door;
		private readonly bool? _doorOpen;
		private readonly ICharacter[] _occupants;
		private readonly IEffect[][] _occupantEffects;
		private bool _invalid;
		internal Attempt(ICharacter actor, IRoomExit exit, IMovement movement)
		{
			_actor = actor; _body = actor.Body; _exit = exit; _boundary = exit.Exit; _origin = exit.Origin;
			_destination = exit.Destination; _world = actor.Gameworld; _movement = movement; _layer = actor.RoomLayer;
			_instance = actor.InstanceId; _identity = actor.Identity; _state = actor.State;
			_actorEffects = actor.Effects.ToArray(); _bodyEffects = _body?.Effects.ToArray() ?? [];
			_wornItems = _body?.WornItems.ToArray() ?? [];
			_prosthetics = _body?.Prosthetics.ToArray() ?? [];
			_prostheticParents = _prosthetics.Select(x => x.Parent).ToArray();
			_exitEffects = _boundary.Effects.ToArray();
			_door = _boundary.Door; _doorOpen = _door?.IsOpen;
			_occupants = _origin.Characters.ToArray();
			_occupantEffects = _occupants.Select(x => x.Effects.ToArray()).ToArray();
			_actor.CurrentBodyChanged += BodyChanged;
			_actor.OnQuit += Invalidated; _actor.OnDeleted += Invalidated;
			_basePlane = _body?.BasePlanarPresence.SaveToXml().ToString(SaveOptions.DisableFormatting) ?? "";
			_plane = _body is null ? "" : actor.GetPlanarPresence().Definition.SaveToXml().ToString(SaveOptions.DisableFormatting);
		}
		private void BodyChanged(ICharacter actor, IBody oldBody, IBody newBody) => _invalid = true;
		private void Invalidated(IPerceivable actor) => _invalid = true;
		public void Dispose()
		{ _actor.CurrentBodyChanged -= BodyChanged; _actor.OnQuit -= Invalidated; _actor.OnDeleted -= Invalidated; }
		internal bool IsCurrent => !_invalid && _body is not null && ReferenceEquals(_actor.Body, _body) && ReferenceEquals(_body.Actor, _actor) &&
			ReferenceEquals(_actor.Gameworld, _world) && ReferenceEquals(_origin.Gameworld, _world) && ReferenceEquals(_destination.Gameworld, _world) &&
			_body.BasePlanarPresence.SaveToXml().ToString(SaveOptions.DisableFormatting) == _basePlane &&
			ReferenceEquals(_boundary.Gameworld, _world) && ReferenceEquals(_actor.Location, _origin) && _actor.RoomLayer == _layer &&
			_actor.InstanceId == _instance && ReferenceEquals(_actor.Identity, _identity) && _actor.State == _state && !_actor.State.IsDead() &&
			ReferenceEquals(_actor.Movement, _movement) && !_movement.Cancelled && _movement.Phase == MovementPhase.OriginalRoom &&
			_movement.CharacterMovers.Any(x => ReferenceEquals(x, _actor)) && ReferenceEquals(_movement.Exit, _exit) &&
			ReferenceEquals(_exit.Exit, _boundary) && ReferenceEquals(_exit.Origin, _origin) && ReferenceEquals(_exit.Destination, _destination) &&
			ReferenceEquals(_boundary.RoomExitFor(_origin), _exit) && ReferenceEquals(_exit.Opposite?.Exit, _boundary) &&
			ReferenceEquals(_exit.Opposite?.Origin, _destination) && ReferenceEquals(_exit.Opposite?.Destination, _origin) &&
			ReferenceEquals(_boundary.Door, _door) && _door?.IsOpen == _doorOpen &&
			_occupants.SequenceEqual(_origin.Characters, ReferenceEqualityComparer.Instance) &&
			_occupants.Select((x, i) => _occupantEffects[i].SequenceEqual(x.Effects, ReferenceEqualityComparer.Instance)).All(x => x) &&
			_actorEffects.SequenceEqual(_actor.Effects, ReferenceEqualityComparer.Instance) &&
			_bodyEffects.SequenceEqual(_body.Effects, ReferenceEqualityComparer.Instance) &&
			_wornItems.SequenceEqual(_body.WornItems, ReferenceEqualityComparer.Instance) &&
			_prosthetics.SequenceEqual(_body.Prosthetics, ReferenceEqualityComparer.Instance) &&
			_prostheticParents.SequenceEqual(_prosthetics.Select(x => x.Parent), ReferenceEqualityComparer.Instance) &&
			_exitEffects.SequenceEqual(_boundary.Effects, ReferenceEqualityComparer.Instance);
		internal bool Continue()
		{
			if (!IsCurrent || !CommandExecutionScope.TryContinue() || !IsCurrent) { _invalid = true; return false; }
			var presence = _actor.GetPlanarPresence();
			var admitted = _actor.CanCross(_exit).Success;
			if (!admitted || presence.CanCrossMagicalBarriers || presence.SuspendsPhysicalContact ||
				presence.Definition.SaveToXml().ToString(SaveOptions.DisableFormatting) != _plane || !IsCurrent)
			{ _invalid = true; return false; }
			return true;
		}
	}
}
