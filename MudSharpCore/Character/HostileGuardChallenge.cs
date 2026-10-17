using System.Threading;
using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp.Character;

public enum GuardChallengeAction { Get, Open, Exit }

/// <summary>
/// A command-owned capability, never an effect or a permission flag. Movement retains the same
/// object for its delayed crossing. The ambient adapter lets existing component access checks
/// consult this typed capability without changing their ordinary public APIs.
/// </summary>
public sealed class HostileGuardChallenge : IGuardChallengeContext
{
	public const string EnabledSetting = "ArmageddonHostileGuardAttemptsEnabled";
	private static readonly AsyncLocal<HostileGuardChallenge?> Active = new();
	internal static HostileGuardChallenge? Current => Active.Value;
	internal static HostileGuardChallenge? CurrentItem => Current is { _action: not GuardChallengeAction.Exit } item ? item : null;
	private readonly GuardChallengeAction _action;
	private readonly RoomLayer _layer;
	private readonly double? _routePosition;
	private readonly IBody _body;
	private readonly IRoomExit? _exit;
	private readonly IGameItem? _item;
	private readonly IGameItem? _container;
	private readonly List<(IGameItem Item, IGameItem? Parent, IRoom? Room, IBody? Owner, RoomLayer Layer)> _chain = [];
	private readonly List<(ICharacter Guard, IGuardItemEffect Effect, IGameItem Item)> _itemGuards = [];
	private readonly List<(ICharacter Guard, IGuardExitEffect Effect)> _exitGuards = [];
	private readonly HashSet<ICharacter> _cohort = new(ReferenceEqualityComparer.Instance);
	private readonly Dictionary<ICharacter, (IBody Body, ICharacter? Mount, ICharacter[] Riders, double? Position)> _cohortState = new(ReferenceEqualityComparer.Instance);
	private bool _resolved;
	private bool _blocked;
	private bool _disposed;
	private bool _cohortBound;

	private HostileGuardChallenge(ICharacter actor, GuardChallengeAction action, IGameItem? item = null,
		IGameItem? container = null, IRoomExit? exit = null)
	{
		Actor = actor;
		Origin = actor.Location;
		_body = actor.Body;
		_layer = actor.RoomLayer;
		_routePosition = actor.RoutePositionMetres;
		_action = action;
		_item = item;
		_container = container;
		_exit = exit;
	}

	internal bool HasBlockingExitGuards => _exitGuards.Count > 0;
    internal bool HasBlockingExitGuardsFor(IEnumerable<ICharacter> movers)
    {
        var cohort = movers.ToArray();
        return _exitGuards.Any(x => cohort.Any(ch => ExitGuardBlocks(x.Guard, x.Effect, ch, _exit!)));
    }
	public ICharacter Actor { get; }
	public IRoom Origin { get; }
	public bool IsValid => !_disposed && !_blocked &&
		Actor.Gameworld.GetStaticBool(EnabledSetting) && ReferenceEquals(Actor.Body, _body) &&
		ReferenceEquals(Actor.Location, Origin) && Actor.RoomLayer == _layer && Actor.RoutePositionMetres == _routePosition &&
		CharacterState.Able.HasFlag(Actor.State) && Actor.Combat is null &&
		(_item is null || Actor.CanSee(_item)) &&
		_chain.All(x => !x.Item.Deleted && !x.Item.Destroyed && ReferenceEquals(x.Item.ContainedIn, x.Parent) &&
			ReferenceEquals(x.Item.Location, x.Room) && ReferenceEquals(x.Item.InInventoryOf, x.Owner) && x.Item.RoomLayer == x.Layer) &&
		_itemGuards.All(x => Eligible(x.Guard, Actor) && x.Effect.Applies() &&
			x.Guard.EffectsOfType<IGuardItemEffect>().Any(y => ReferenceEquals(y, x.Effect)) &&
			ItemGuardBlocks(x.Guard, x.Effect, x.Item, Actor)) &&
		_exitGuards.All(x => Eligible(x.Guard, Actor) && x.Guard.EffectsOfType<IGuardExitEffect>().Any(y => ReferenceEquals(y, x.Effect)) &&
			x.Effect.Exit == _exit && x.Effect.Applies() && _cohort.Any(ch => ExitGuardBlocks(x.Guard, x.Effect, ch, _exit!))) &&
		(_exit is null || ReferenceEquals(_exit.Origin, Origin) && Actor.Body.CanSee(Origin, _exit) &&
			_cohort.All(x => ReferenceEquals(x.Location, Origin) && x.RoomLayer == _layer && Actor.ColocatedWith(x))) &&
		_cohortState.All(x => ReferenceEquals(x.Key.Body, x.Value.Body) && ReferenceEquals(x.Key.RidingMount, x.Value.Mount) &&
			x.Key.RoutePositionMetres == x.Value.Position &&
			new HashSet<ICharacter>(x.Key.Riders, ReferenceEqualityComparer.Instance).SetEquals(x.Value.Riders));

	public static bool Enabled(ICharacter actor)
	{
		if (actor.Gameworld.GetStaticBool(EnabledSetting)) return true;
		actor.OutputHandler.Send("Hostile guard challenges are disabled in this world.");
		return false;
	}

	private static bool Eligible(ICharacter guard, ICharacter actor) =>
		!guard.SamePhysicalInstance(actor) && ReferenceEquals(guard.Location, actor.Location) &&
		guard.RoomLayer == actor.RoomLayer && guard.ColocatedWith(actor) &&
		CharacterState.Able.HasFlag(guard.State) && guard.CanSee(actor);

	// Keep the existing room predicate, including its existing ally and vicinity exemptions.
	internal static bool ItemGuardBlocks(ICharacter guard, IGuardItemEffect effect, IGameItem item, ICharacter actor) =>
		!guard.SamePhysicalInstance(actor) && !guard.TrustedAllyIDs.Contains(actor.Id) && !guard.AllyIDs.Contains(actor.Id) &&
		(actor.Location.RouteDefinition is null || guard.GetProximity(item) <= Proximity.Immediate) &&
		((effect.Applies() && effect.TargetItem == item) ||
		 actor.Location.LayerGameItems(actor.RoomLayer).Except(item)
			.Where(x => actor.Location.RouteDefinition is null || x.GetProximity(item) <= Proximity.Immediate)
			.Any(x => x == effect.TargetItem && x.InVicinity(item)));

	private static bool ExitGuardBlocks(ICharacter guard, IGuardExitEffect effect, ICharacter mover, IRoomExit exit) =>
		!mover.AffectedBy<IImmwalkEffect>() && mover.RidingMount?.AffectedBy<IImmwalkEffect>() != true &&
		!ReferenceEquals(guard, mover) && !ReferenceEquals(guard, mover.RidingMount) &&
		!mover.Riders.Any(x => ReferenceEquals(x, guard)) &&
		(!effect.PermittedToCross(mover, exit) || !effect.PermittedToCross(mover.RidingMount, exit) ||
		 mover.Riders.Any(x => !effect.PermittedToCross(x, exit)));

	public static HostileGuardChallenge? ForItem(ICharacter actor, IGameItem item, IGameItem? container,
		GuardChallengeAction action)
	{
		if (!Enabled(actor)) return null;
		if (action == GuardChallengeAction.Exit || item is null || actor.Combat is not null)
		{
			actor.OutputHandler.Send("You cannot challenge a guard with that action.");
			return null;
		}
		var result = new HostileGuardChallenge(actor, action, item, container);
		var seen = new HashSet<IGameItem>(ReferenceEqualityComparer.Instance);
		for (var current = item; current is not null; current = current.ContainedIn)
		{
			if (!seen.Add(current)) return null;
			result._chain.Add((current, current.ContainedIn, current.Location, current.InInventoryOf, current.RoomLayer));
		}
		var root = result._chain.Last().Item;
		if (root.InInventoryOf is not null ||
		    !(root.Location == actor.Location || root.GetItemType<IDoor>()?.InstalledExit?.Rooms.Contains(actor.Location) == true) ||
		    container is not null && item.ContainedIn != container)
		{
			actor.OutputHandler.Send("Guard challenges support only room items and their containers, or installed doors.");
			return null;
		}
		foreach (var guard in actor.Location.LayerCharacters(actor.RoomLayer).OrderBy(x => x.Id).ThenBy(x => x.InstanceId))
		{
			if (!Eligible(guard, actor)) continue;
			foreach (var effect in guard.EffectsOfType<IGuardItemEffect>())
			foreach (var entry in result._chain)
			{
				if (effect.Applies() && ItemGuardBlocks(guard, effect, entry.Item, actor)) result._itemGuards.Add((guard, effect, entry.Item));
			}
		}
		if (result._itemGuards.Count > 0) return result;
		actor.OutputHandler.Send("There is no eligible guard blocking that action to challenge.");
		return null;
	}

	public static HostileGuardChallenge? ForExit(ICharacter actor, IRoomExit exit)
	{
		if (!Enabled(actor)) return null;
		if (actor.Combat is not null || exit.Origin != actor.Location || !actor.Body.CanSee(actor.Location, exit)) return null;
		var result = new HostileGuardChallenge(actor, GuardChallengeAction.Exit, exit: exit);
		// Prospective members are used only for preflight. CreateMovement binds the actual cohort before rolling.
		void Add(ICharacter? ch)
		{
			if (ch is null || !actor.ColocatedWith(ch) || !result._cohort.Add(ch)) return;
			Add(ch.RidingMount);
			foreach (var rider in ch.Riders) Add(rider);
			foreach (var drag in ch.EffectsOfType<IDragParticipant>())
			{
				if (drag is not Dragging dragging) continue;
				Add(dragging.Target as ICharacter);
				foreach (var helper in dragging.Helpers) Add(helper);
			}
		}
		Add(actor);
		if (actor.Party?.Leader.SamePhysicalInstance(actor) == true)
			foreach (var member in actor.Party.CharacterMembers) Add(member);
		foreach (var guard in actor.Location.CharactersInImmediateVicinity(actor).OrderBy(x => x.Id).ThenBy(x => x.InstanceId))
		{
			if (!Eligible(guard, actor)) continue;
			foreach (var effect in guard.EffectsOfType<IGuardExitEffect>())
				if (effect.Exit == exit && effect.Applies() && result._cohort.Any(ch => ExitGuardBlocks(guard, effect, ch, exit)))
					result._exitGuards.Add((guard, effect));
		}
		return result;
	}

	internal bool BindCohort(IEnumerable<ICharacter> movers)
	{
		if (_resolved || _cohortBound) return false;
		var actual = movers.ToArray();
		if (actual.Any(x => !_cohort.Contains(x))) return false;
		_cohort.Clear();
		foreach (var mover in actual) _cohort.Add(mover);
		_exitGuards.RemoveAll(x => !_cohort.Any(ch => ExitGuardBlocks(x.Guard, x.Effect, ch, _exit!)));
		foreach (var mover in actual) _cohortState[mover] = (mover.Body, mover.RidingMount, mover.Riders.ToArray(), mover.RoutePositionMetres);
		_cohortBound = true;
		return true;
	}

	internal bool MatchesCohort(IEnumerable<ICharacter> movers) => _cohort.SetEquals(movers);
	internal HostileGuardChallenge? PendingMovement(ICharacter mover, IRoomExit exit) =>
		_action == GuardChallengeAction.Exit && !_cohortBound && _exit == exit &&
		(ReferenceEquals(mover, Actor) || ReferenceEquals(mover, Actor.RidingMount)) ? this : null;

	internal static IDisposable Suspend()
	{
		var previous = Active.Value;
		if (previous is null) return NoScope;
		Active.Value = null;
		return new Scope(() => Active.Value = previous);
	}

	public bool Bypasses(ICharacter mover, IRoomExit exit, IGuardExitEffect guard) =>
		IsValid && _exit == exit && _cohort.Contains(mover) && _exitGuards.Any(x => ReferenceEquals(x.Effect, guard));

	public bool Bypasses(ICharacter actor, IGameItem item, IGuardItemEffect guard) =>
		IsValid && ReferenceEquals(actor, Actor) && _itemGuards.Any(x => ReferenceEquals(x.Item, item) && ReferenceEquals(x.Effect, guard));

	/// <summary>Called once after ordinary preflight; previews and subsequent access checks never roll.</summary>
	public bool Resolve()
	{
		if (_resolved) return !_blocked && IsValid;
		_resolved = true;
		if (!IsValid) { _blocked = true; return false; }
		var guards = _exitGuards.Select(x => x.Guard).Concat(_itemGuards.Select(x => x.Guard))
			.Distinct(ReferenceEqualityComparer.Instance).Cast<ICharacter>().OrderBy(x => x.Id).ThenBy(x => x.InstanceId).ToArray();
		if (guards.Length == 0)
		{
			_blocked = true;
			Actor.OutputHandler.Send("There is no eligible guard blocking that action to challenge.");
			return false;
		}
		var check = Actor.Gameworld.GetCheck(_action == GuardChallengeAction.Exit
			? CheckType.GuardingExitChallengeCheck : CheckType.GuardingItemChallengeCheck);
		foreach (var guard in guards)
		{
			if (!Eligible(guard, Actor)) { _blocked = true; return false; }
			var blocks = check.Check(guard, Difficulty.Normal, target: Actor).IsPass();
			_blocked |= blocks;
			Actor.OutputHandler.Handle(new EmoteOutput(new Emote(blocks
				? "@ challenge|challenges $1's guard, but $1 $1|intercept|intercepts &0."
				: "@ challenge|challenges $1's guard and slip|slips past &1's interception.", Actor, Actor, guard),
				flags: OutputFlags.SuppressObscured));
		}
		if (_blocked && _action == GuardChallengeAction.Exit)
			Actor.AddEffect(new BlockingCommandDelay(Actor, ["Move"], ["movement"],
				"recovering from a blocked guard challenge"), TimeSpan.FromSeconds(2));
		return !_blocked && IsValid;
	}

	public IDisposable Enter()
	{
		var previous = Active.Value;
		Active.Value = this;
		return new Scope(() => Active.Value = previous);
	}

	internal static bool CanCommitItem(ICharacter actor, IGameItem item, IGameItem? container, GuardChallengeAction action)
	{
		return CurrentItem?.CanCommitItemAction(actor, item, container, action) ?? true;
	}
	internal bool CanCommitItemAction(ICharacter actor, IGameItem item, IGameItem? container, GuardChallengeAction action) =>
		ReferenceEquals(Current, this) && _action == action && _resolved && IsValid &&
		ReferenceEquals(Actor, actor) && ReferenceEquals(_item, item) && ReferenceEquals(_container, container);

	internal static void CompleteItem()
	{
		if (Current is { } current && current._action != GuardChallengeAction.Exit) current.Dispose();
	}
	public void Dispose()
	{
		_disposed = true;
		if (ReferenceEquals(Active.Value, this)) Active.Value = null;
	}
	private sealed class Scope(Action restore) : IDisposable { public void Dispose() => restore(); }
	private static readonly IDisposable NoScope = new Scope(static () => { });
}
