#nullable enable

using System.Runtime.CompilerServices;
using System.Threading;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Magic;

namespace MudSharp.NPC.AI;

/// <summary>Ephemeral provenance for selected actions created by an accepted ordered command.
/// No authority is serialized or inferred from a follower link.</summary>
internal sealed class CommandExecutionAuthority
{
	private static readonly AsyncLocal<CommandExecutionAuthority?> Current = new();
	private static readonly ConditionalWeakTable<ICombatMove, CommandExecutionAuthority> Moves = new();
	private readonly ICharacter _actor;
	private readonly ICharacter _commander;
	private readonly long _actorIdentity;
	private readonly long _commanderIdentity;
	private readonly long _instance;
	private readonly Func<bool> _policy;
	private readonly bool _spellOwned;
	private readonly SpellLifecycleOrigin? _grant;
	internal string CommandText { get; }
	internal bool RequiresGrant => _spellOwned;

	private CommandExecutionAuthority(ICharacter actor, ICharacter commander, string command, Func<bool> policy)
	{
		_actor = actor;
		_commander = commander;
		_actorIdentity = CharacterInstanceIdentityComparer.IdentityId(actor);
		_commanderIdentity = CharacterInstanceIdentityComparer.IdentityId(commander);
		_instance = actor.InstanceId;
		CommandText = command;
		_policy = policy;
		var service = actor.Gameworld.SpellOwnedCorpseAnimations;
		_spellOwned = service?.OwnsInstance(_instance) == true;
		_grant = _spellOwned ? service?.CommandGrant(_instance, _commanderIdentity) : null;
	}

	private CommandExecutionAuthority(CommandExecutionAuthority accepted, ICombat? combat)
	{
		_actor = accepted._actor;
		_commander = accepted._commander;
		_actorIdentity = accepted._actorIdentity;
		_commanderIdentity = accepted._commanderIdentity;
		_instance = accepted._instance;
		CommandText = accepted.CommandText;
		_spellOwned = accepted._spellOwned;
		_grant = accepted._grant;
		bool SameCombat() => ReferenceEquals(_actor.Combat, combat) &&
			(combat is null || combat.Combatants.Any(x => ReferenceEquals(x, _actor)));
		_policy = () => SameCombat() && accepted._policy() && SameCombat();
	}

	internal static IDisposable Enter(ICharacter actor, ICharacter commander, string command, Func<bool> policy)
	{
		return Enter(Prepare(actor, commander, command, policy));
	}

	internal static CommandExecutionAuthority Prepare(ICharacter actor, ICharacter commander, string command, Func<bool> policy) =>
		new(actor, commander, command, policy);

	internal static IDisposable Enter(CommandExecutionAuthority authority)
	{
		var previous = Current.Value;
		Current.Value = authority;
		return new Scope(previous);
	}

	private sealed class Scope(CommandExecutionAuthority? previous) : IDisposable
	{
		public void Dispose() => Current.Value = previous;
	}

	internal static CommandExecutionAuthority? Capture(ICharacter actor) =>
		ReferenceEquals(Current.Value?._actor, actor) ? Current.Value : null;
	internal static CommandExecutionAuthority? CaptureSelected(ICharacter actor) =>
		Capture(actor) is { } authority ? new(authority, actor.Combat) : null;

	// A secondary is identity-local, not in Actors. Verify its exact current object without loading.
	private static bool IsCurrent(ICharacter actor, bool commander)
	{
		var world = actor.Gameworld;
		var roots = world.Actors.Concat(world.Characters).Concat(world.NPCs);
		if (!commander) roots = roots.Concat(world.CachedActors);
		var id = CharacterInstanceIdentityComparer.IdentityId(actor);
		var identities = roots.Where(x => CharacterInstanceIdentityComparer.IdentityId(x) == id).Select(x => x.Identity).ToArray();
		return id > 0 && actor.Identity is { } identity && identity.Id == id &&
		       identities.Length > 0 && identities.All(x => ReferenceEquals(x, identity)) &&
		       identity.Instances.Any(x => ReferenceEquals(x, actor)) &&
		       ReferenceEquals(actor.Body?.Actor, actor) &&
		       actor.Location?.Characters.Any(x => ReferenceEquals(x, actor)) == true &&
		       !actor.State.IsDead() && !actor.State.IsInStatis();
	}

	internal bool MayExecute(ICharacter actor)
	{
		try
		{
			if (!ReferenceEquals(actor, _actor) || !ReferenceEquals(actor.Gameworld, _commander.Gameworld) ||
			    actor.InstanceId != _instance || CharacterInstanceIdentityComparer.IdentityId(actor) != _actorIdentity ||
			    CharacterInstanceIdentityComparer.IdentityId(_commander) != _commanderIdentity ||
			    !IsCurrent(actor, false) || !IsCurrent(_commander, true))
				return false;
			if (!_policy() || actor.InstanceId != _instance ||
			    CharacterInstanceIdentityComparer.IdentityId(actor) != _actorIdentity ||
			    CharacterInstanceIdentityComparer.IdentityId(_commander) != _commanderIdentity ||
			    !IsCurrent(actor, false) || !IsCurrent(_commander, true)) return false;
			var service = actor.Gameworld.SpellOwnedCorpseAnimations;
			if ((service?.OwnsInstance(_instance) == true) != _spellOwned) return false;
			if (_spellOwned && (!actor.IsEmbodied || _grant is null ||
			    service?.CommandGrant(_instance, _commanderIdentity) != _grant)) return false;
			return true;
		}
		catch (Exception)
		{
			// A failed policy or unavailable grant is never permission to execute an old order.
			return false;
		}
	}

	internal void Bind(ICombatMove? move)
	{
		if (move is not null) Moves.Add(move, new CommandExecutionAuthority(this, _actor.Combat));
	}

	internal static bool MayExecute(ICombatMove? move, ICharacter? actor) =>
		move is null || !Moves.TryGetValue(move, out var authority) ||
		(actor is not null && authority.MayExecute(actor));

	internal static void Inherit(ICombatMove parent, ICombatMove child)
	{
		if (Moves.TryGetValue(parent, out var authority)) Moves.GetValue(child, _ => authority);
	}
	internal static bool IsOrdered(ICombatMove move) => Moves.TryGetValue(move, out _);
}
