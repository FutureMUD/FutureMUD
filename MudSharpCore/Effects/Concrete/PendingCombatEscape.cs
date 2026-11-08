#nullable enable

using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Events;
using MudSharp.Movement;
using MudSharp.Form.Shape;

namespace MudSharp.Effects.Concrete;

/// <summary>Reports a flee only once its actual exit traversal completes, with departure-time perception.</summary>
public sealed class PendingCombatEscape : Effect, IEffectRemovalNotification
{
	private readonly ICharacter _actor;
	private readonly IRoom _origin;
	private readonly IRoomExit _exit;
	private readonly ICharacter[] _visible;
	private bool _released;

	public PendingCombatEscape(ICharacter actor, IRoomExit exit) : base(actor)
	{
		_actor = actor;
		_origin = actor.Location;
		_exit = exit;
		_visible = _origin.Characters.Where(x => !ReferenceEquals(x, actor) && x.RoomLayer == actor.RoomLayer && actor.CanSee(x)).ToArray();
		actor.OnLocationChanged += ChangedLocation;
		actor.OnStopMove += Stopped;
		actor.OnQuit += Invalid;
		actor.OnDeath += Invalid;
		actor.OnDeleted += Invalid;
	}

	private void Invalid(IPerceivable actor) => _actor.RemoveEffect(this, true);
	private void Stopped(object? sender, MoveEventArgs args) => Invalid(_actor);
	private void ChangedLocation(ILocateable actor, IRoomExit? crossed)
	{
		if (_released) return;
		var completed = crossed is not null && ReferenceEquals(crossed.Exit, _exit.Exit) &&
			ReferenceEquals(_actor.Location, _exit.Destination) && !ReferenceEquals(_actor.Location, _origin);
		var left = _visible.Where(x => ReferenceEquals(x.Location, _origin) && !x.State.IsDead()).ToArray();
		Invalid(_actor);
		if (completed && !_actor.State.IsDead()) _actor.HandleEvent(EventType.CharacterEscapedCombat, _actor, _origin, left);
	}

	public override void RemovalEffect()
	{
		OnRemovedFromOwner();
		base.RemovalEffect();
	}

	public void OnRemovedFromOwner()
	{
		if (_released) return;
		_released = true;
		_actor.OnLocationChanged -= ChangedLocation;
		_actor.OnStopMove -= Stopped;
		_actor.OnQuit -= Invalid;
		_actor.OnDeath -= Invalid;
		_actor.OnDeleted -= Invalid;
	}
	protected override string SpecificEffectType => "PendingCombatEscape";
	public override string Describe(IPerceiver voyeur) => "Awaiting completion of a combat escape.";
}
