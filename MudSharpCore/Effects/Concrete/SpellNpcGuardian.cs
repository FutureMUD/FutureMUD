#nullable enable

using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;

namespace MudSharp.Effects.Concrete;

/// <summary>A persistent creator bond using ordinary following, engagement and guard combat moves.</summary>
public sealed class SpellNpcGuardian : Effect, IGuardCharacterEffect, IAffectProximity
{
	private ICharacter? _creator;
	private bool _active;
	private bool _protecting;
	private bool _retiring;
	private ICharacter Guardian => (ICharacter)Owner;
	public long CreatorId { get; }
	public long CreatorInstanceId { get; }
	public bool Interdicting { get; set; } = true;
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "SpellNpcGuardian";

	public SpellNpcGuardian(ICharacter owner, ICharacter creator) : base(owner)
	{
		CreatorId = CharacterInstanceIdentityComparer.IdentityId(creator);
		CreatorInstanceId = creator.InstanceId;
	}

	private SpellNpcGuardian(XElement envelope, IPerceivable owner) : base(envelope, owner)
	{
		var root = envelope.Element("Effect")!;
		CreatorId = long.Parse(root.Element("CreatorId")!.Value);
		CreatorInstanceId = long.Parse(root.Element("CreatorInstanceId")!.Value);
		Interdicting = bool.Parse(root.Element("Interdicting")!.Value);
		_retiring = bool.Parse(root.Element("Retiring")?.Value ?? "false");
	}

	public static void InitialiseEffectType() =>
		RegisterFactory("SpellNpcGuardian", (root, owner) => new SpellNpcGuardian(root, owner));

	protected override XElement SaveDefinition() => new("Effect", new XElement("CreatorId", CreatorId),
		new XElement("CreatorInstanceId", CreatorInstanceId), new XElement("Interdicting", Interdicting), new XElement("Retiring", _retiring));

	public override IEnumerable<PhysicalEntityReference> PhysicalReferences =>
		[new(PhysicalEntityKind.Character, CreatorId, "CreatorId"),
		 new(PhysicalEntityKind.CharacterInstance, CreatorInstanceId, "CreatorInstanceId")];

	// Resolve loaded instances only. Reading archival references never calls this getter.
	public IEnumerable<ICharacter> Targets => _retiring ? [] : Gameworld.Actors
		.Where(x => CharacterInstanceIdentityComparer.IdentityId(x) == CreatorId && x.InstanceId == CreatorInstanceId &&
			!x.State.HasFlag(CharacterState.Dead));

	public void AddTarget(ICharacter target) { /* The creator bond has exactly one immutable recipient. */ }
	public void RemoveTarget(ICharacter target)
	{
		if (Targets.Contains(target)) Owner.RemoveEffect(this, true);
	}
	public bool ShouldRemove(IAffectedByChangeInGuarding effect) => !ReferenceEquals(effect, this);
	public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing) =>
		thing is ICharacter character && Targets.Contains(character) ? (true, Proximity.Immediate) : (false, Proximity.Unapproximable);
	public override string Describe(IPerceiver voyeur) =>
		$"Protecting creator #{CreatorId.ToString("N0", voyeur)}; interposing: {Interdicting.ToColouredString()}.";

	public override void InitialEffect() => Activate();
	public override void Login() => Activate();
	private void Activate()
	{
		if (_active || _retiring || Guardian.State.HasFlag(CharacterState.Dead)) return;
		// Durable intent also covers a crash before the owner's effect snapshot was saved.
		if (Gameworld.SpellOwnedNpcs?.HasPendingRetirement(Guardian, CreatorId) == true)
		{
			PrepareRetirement(CreatorId);
			return;
		}
		_active = true;
		Guardian.OnQuit += OwnerQuit;
		Guardian.OnDeath += OwnerDied;
		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += Refresh;
		Refresh();
	}

	private void Refresh()
	{
		if (Guardian.State.HasFlag(CharacterState.Dead)) { Owner.RemoveEffect(this, true); return; }
		var creator = Targets.FirstOrDefault();
		if (!ReferenceEquals(_creator, creator))
		{
			Unbind();
			_creator = creator;
			if (creator is not null)
			{
				creator.OnDeath += CreatorGone;
				creator.OnQuit += CreatorGone;
				creator.OnDeleted += CreatorGone;
				creator.OnJoinCombat += CreatorCombat;
			}
		}
		if (creator is null || !Guardian.ColocatedWith(creator)) return;
		if (!ReferenceEquals(Guardian.Following, creator)) Guardian.Follow(creator);
		Protect();
	}

	private void Protect()
	{
		if (_creator is null || !Guardian.ColocatedWith(_creator)) return;
		var attacker = _creator.Combat?.Combatants.OfType<ICharacter>()
			.FirstOrDefault(x => !ReferenceEquals(x, Guardian) && ReferenceEquals(x.CombatTarget, _creator));
		if (attacker is not null) ProtectAgainst(attacker);
	}
	private void ProtectAgainst(ICharacter attacker)
	{
		if (_retiring || _protecting || !CharacterState.Able.HasFlag(Guardian.State) ||
			!Targets.Any(x => ReferenceEquals(attacker.CombatTarget, x) && Guardian.ColocatedWith(x)) ||
			ReferenceEquals(Guardian.CombatTarget, attacker) || !Guardian.CanSee(attacker) || !Guardian.CanEngage(attacker)) return;
		_protecting = true;
		try { Guardian.Engage(attacker, ranged: !Guardian.ColocatedWith(attacker)); }
		finally { _protecting = false; }
	}

	/// <summary>Targets are assigned before this notification; guardians respond before the first attack action.</summary>
	public static void NotifyEngagement(ICharacter attacker, ICharacter target)
	{
		foreach (var guardian in target.Location.LayerCharacters(target.RoomLayer).ToArray())
			foreach (var bond in guardian.EffectsOfType<SpellNpcGuardian>().ToArray()) bond.ProtectAgainst(attacker);
	}
	private void CreatorCombat(IPerceivable _) => Protect();
	private void CreatorGone(IPerceivable _) => Unbind();
	private void OwnerDied(IPerceivable _) => Owner.RemoveEffect(this, true);
	private void OwnerQuit(IPerceivable _) => Deactivate();
	/// <summary>Release this creation's exact outgoing follow bond after durable retirement intent.</summary>
	public bool PrepareRetirement(long lifecycleCreatorId)
	{
		if (CreatorId != lifecycleCreatorId) return false;
		if (!_retiring) { _retiring = true; Changed = true; }
		Deactivate();
		if (Guardian.Following is ICharacter following && CharacterInstanceIdentityComparer.IdentityId(following) == CreatorId &&
			following.InstanceId == CreatorInstanceId) Guardian.CeaseFollowing();
		return true;
	}
	private void Unbind()
	{
		if (_creator is null) return;
		_creator.OnDeath -= CreatorGone;
		_creator.OnQuit -= CreatorGone;
		_creator.OnDeleted -= CreatorGone;
		_creator.OnJoinCombat -= CreatorCombat;
		if (ReferenceEquals(Guardian.Following, _creator)) Guardian.CeaseFollowing();
		_creator = null;
	}
	private void Deactivate()
	{
		if (!_active) return;
		_active = false;
		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= Refresh;
		Guardian.OnQuit -= OwnerQuit;
		Guardian.OnDeath -= OwnerDied;
		Unbind();
	}
	public override void RemovalEffect() => Deactivate();
}
