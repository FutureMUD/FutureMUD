#nullable enable

using System.Xml.Linq;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Magic;

namespace MudSharp.Effects.Concrete;

/// <summary>Per-NPC persisted creator bond and order state for native spell-owned companions.</summary>
public sealed partial class SpellNpcControlEffect : Effect, IGuardCharacterEffect, IAffectProximity, ISpellNpcMissionController
{
	private readonly Func<ICharacter, bool> _isDurablyPrimaryTarget = IsDurablyPrimaryTarget;
	private bool _active;
	private bool _retiring;
	private bool _followHeartbeat;
	private bool _persistenceUncertain;
	public Guid LifecycleId { get; }
	public long CreatorId { get; }
	public long CreatorInstanceId { get; }
	public long SpellId { get; }
	public int Grade { get; }
	public DateTime? DeadlineUtc { get; }
	public string Intent { get; private set; } = "none";
	public long? TargetId { get; private set; }
	public long? TargetInstanceId { get; private set; }
	public bool ControlLost { get; private set; }
	public bool Interdicting { get; set; } = true;
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "SpellNpcControl";
	private ICharacter Creature => (ICharacter)Owner;

	public SpellNpcControlEffect(ICharacter owner, SpellLifecycleOrigin origin, ICharacter creator, bool guardCreator = false)
		: this(owner, origin, creator, guardCreator, IsDurablyPrimaryTarget)
	{
	}

	internal SpellNpcControlEffect(ICharacter owner, SpellLifecycleOrigin origin, ICharacter creator,
		bool guardCreator, Func<ICharacter, bool> isDurablyPrimaryTarget) : base(owner)
	{
		_isDurablyPrimaryTarget = isDurablyPrimaryTarget;
		LifecycleId = origin.Id; CreatorId = origin.CreatorId; CreatorInstanceId = creator.InstanceId; DeadlineUtc = origin.DeadlineUtc;
		SpellId = origin.SpellId; Grade = origin.Grade;
		if (guardCreator)
		{
			Intent = "guard"; TargetId = CreatorId; TargetInstanceId = CreatorInstanceId;
		}
	}

	private SpellNpcControlEffect(XElement envelope, IPerceivable owner) : base(envelope, owner)
	{
		var root = envelope.Element("Effect")!;
		LifecycleId = Guid.Parse(root.Element("LifecycleId")!.Value);
		CreatorId = long.Parse(root.Element("CreatorId")!.Value);
		CreatorInstanceId = long.Parse(root.Element("CreatorInstanceId")!.Value);
		SpellId = long.Parse(root.Element("SpellId")!.Value);
		Grade = int.Parse(root.Element("Grade")!.Value);
		DeadlineUtc = DateTime.TryParse(root.Element("DeadlineUtc")?.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var deadline) ? deadline : null;
		Intent = root.Element("Intent")?.Value ?? "none";
		TargetId = long.TryParse(root.Element("TargetId")?.Value, out var targetId) ? targetId : null;
		TargetInstanceId = long.TryParse(root.Element("TargetInstanceId")?.Value, out var targetInstanceId) ? targetInstanceId : null;
		ControlLost = bool.Parse(root.Element("ControlLost")?.Value ?? "false");
		_retiring = bool.Parse(root.Element("Retiring")?.Value ?? "false");
		if (LifecycleId == Guid.Empty || CreatorId <= 0 || CreatorInstanceId <= 0 || SpellId <= 0 || Grade is < 1 or > 7 ||
			Intent is not ("none" or "follow" or "guard" or "attack") || (TargetId.HasValue != TargetInstanceId.HasValue))
			throw new FormatException("Invalid spell NPC control state.");
	}

	public static void InitialiseEffectType() => RegisterFactory("SpellNpcControl", (root, owner) => new SpellNpcControlEffect(root, owner));
	protected override XElement SaveDefinition() => new("Effect", new XElement("LifecycleId", LifecycleId),
		new XElement("CreatorId", CreatorId), new XElement("CreatorInstanceId", CreatorInstanceId),
		new XElement("SpellId", SpellId), new XElement("Grade", Grade), new XElement("Intent", Intent),
		DeadlineUtc.HasValue ? new XElement("DeadlineUtc", DeadlineUtc.Value) : null,
		TargetId.HasValue ? new XElement("TargetId", TargetId.Value) : null, TargetInstanceId.HasValue ? new XElement("TargetInstanceId", TargetInstanceId.Value) : null,
		new XElement("ControlLost", ControlLost), new XElement("Retiring", _retiring));
	public override IEnumerable<PhysicalEntityReference> PhysicalReferences
	{
		get
		{
			var references = new List<PhysicalEntityReference>
			{
				new(PhysicalEntityKind.Character, CreatorId, "CreatorId"),
				new(PhysicalEntityKind.CharacterInstance, CreatorInstanceId, "CreatorInstanceId")
			};
			if (TargetId is { } target) references.Add(new(PhysicalEntityKind.Character, target, "TargetId"));
			if (TargetInstanceId is { } instance) references.Add(new(PhysicalEntityKind.CharacterInstance, instance, "TargetInstanceId"));
			return references;
		}
	}

	public override string Describe(IPerceiver voyeur) => ControlLost
		? $"Lost control of summoned creature #{Creature.Id.ToString("N0", voyeur)}."
		: $"Bound to creator #{CreatorId.ToString("N0", voyeur)}; intent: {Intent.ColourValue()}.";
	private ICharacter? ResolveCreator() => Gameworld.Actors.FirstOrDefault(x =>
		CharacterInstanceIdentityComparer.IdentityId(x) == CreatorId && x.InstanceId == CreatorInstanceId &&
		!x.State.HasFlag(CharacterState.Dead) && !x.State.HasFlag(CharacterState.Stasis));
	private bool HasCurrentAuthority()
	{
		var creator = ResolveCreator();
		return !_persistenceUncertain && !_retiring && creator is not null &&
			Gameworld.SpellOwnedNpcs?.HasPendingRetirement(Creature, CreatorId) != true &&
			Gameworld.SpellOwnedNpcs?.HasCommandAuthority(Creature, creator, LifecycleId, CreatorId,
				CreatorInstanceId, SpellId, Grade) == true;
	}
	public IEnumerable<ICharacter> Targets => _active && Intent == "guard" && !ControlLost && HasCurrentAuthority() && ResolveCreator() is { } creator
		? [creator]
		: [];
	public void AddTarget(ICharacter target) { }
	public void RemoveTarget(ICharacter target) { if (Targets.Contains(target)) Creature.RemoveEffect(this, true); }
	public bool ShouldRemove(IAffectedByChangeInGuarding effect) => !ReferenceEquals(effect, this);
	public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing) =>
		thing is ICharacter ch && Intent == "guard" && Targets.Contains(ch) ? (true, Proximity.Immediate) : (false, Proximity.Unapproximable);

	public override void InitialEffect() => Activate();
	public override void Login() => Activate();
	private void Activate()
	{
		if (_active || _retiring || Creature.State.HasFlag(CharacterState.Dead)) return;
		if (Gameworld.SpellOwnedNpcs?.HasPendingRetirement(Creature, CreatorId) == true)
		{
			PrepareRetirement(LifecycleId);
			return;
		}
		_active = true; Creature.OnQuit += OwnerQuit; Creature.OnDeath += OwnerDied;
		if (Intent is "follow" or "guard") { SetFollowHeartbeat(true); RefreshFollow(); }
	}
	private void SetFollowHeartbeat(bool enabled)
	{
		if (_followHeartbeat == enabled) return;
		_followHeartbeat = enabled;
		if (enabled) Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += RefreshFollow;
		else Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= RefreshFollow;
	}

	private void RefreshFollow()
	{
		if (!_active || _retiring || _persistenceUncertain || Intent is not ("follow" or "guard")) return;
		if (Gameworld.SpellOwnedNpcs?.HasPendingRetirement(Creature, CreatorId) == true)
		{
			PrepareRetirement(LifecycleId);
			return;
		}
		if (!HasCurrentAuthority() || ResolveCreator() is not { } creator || !Creature.ColocatedWith(creator)) return;
		if (!ReferenceEquals(Creature.Following, creator)) Creature.Follow(creator);
		if (Intent == "guard" && creator.Combat?.Combatants.OfType<ICharacter>()
			.FirstOrDefault(x => !ReferenceEquals(x, Creature) && ReferenceEquals(x.CombatTarget, creator)) is { } attacker)
			NotifyCreatorEngagement(attacker, creator);
	}

	public bool CanAcceptOrder(ICharacter commander) => !_persistenceUncertain && !ControlLost && !_retiring &&
		Gameworld.SpellOwnedNpcs?.HasCommandAuthority(Creature, commander, LifecycleId, CreatorId, CreatorInstanceId, SpellId, Grade) == true;
	public bool CanPerformMission() => !_persistenceUncertain && !ControlLost && HasCurrentAuthority();
	internal ICharacter? ResolveCreatorForMission() => ResolveCreator();
	internal bool PersistMissionState(params ICharacter[] relatedCharacters)
	{
		if (_persistenceUncertain) return false;
		try
		{
			using var isolated = FMDB.BeginIndependentScope(requireWrites: true);
			using var db = new FMDB();
			using var transaction = FMDB.Context.Database.BeginTransaction();
			if (!ApplyQueuedOfflineFearParentCleanup(out var cachedRemovals)) { _persistenceUncertain = true; return false; }
			SaveMissionActors([Creature, .. relatedCharacters]);
			// Body.Save may already flush inventory writes inside this transaction.
			FMDB.Context.SaveChanges();
			transaction.Commit();
			CompleteQueuedOfflineFearParentCleanup(cachedRemovals);
			return true;
		}
		catch { _persistenceUncertain = true; return false; }
	}
	internal bool PersistMissionCustody(IEnumerable<ICharacter> otherCharacters, IGameItem item)
	{
		try
		{
			using var isolated = FMDB.BeginIndependentScope(requireWrites: true);
			using var db = new FMDB();
			using var transaction = FMDB.Context.Database.BeginTransaction();
			if (!ApplyQueuedOfflineFearParentCleanup(out var cachedRemovals)) { _persistenceUncertain = true; return false; }
			SaveMissionActors([Creature, .. otherCharacters], item);
			// Body.Save may already flush inventory writes inside this transaction.
			FMDB.Context.SaveChanges();
			var owners = FMDB.Context.BodiesGameItems.AsNoTracking().Where(x => x.GameItemId == item.Id).Select(x => x.BodyId).ToArray();
			if (item.InInventoryOf is not { } holder || owners.Length != 1 || owners[0] != holder.Id ||
				FMDB.Context.RoomsGameItems.AsNoTracking().Any(x => x.GameItemId == item.Id) ||
				FMDB.Context.GameItems.AsNoTracking().Any(x => x.Id == item.Id && x.ContainerId != null))
			{ _persistenceUncertain = true; return false; }
			transaction.Commit();
			CompleteQueuedOfflineFearParentCleanup(cachedRemovals);
			return true;
		}
		catch { _persistenceUncertain = true; return false; }
	}
	internal void HoldAfterUncertainMissionMutation() => _persistenceUncertain = true;
	internal static void SaveMissionActors(IEnumerable<ICharacter> actors, IGameItem? item = null)
	{
		foreach (var character in actors.Distinct<ICharacter>(ReferenceEqualityComparer.Instance))
		{
			character.Save();
			character.Body.Save();
		}
		item?.Save();
	}
	public bool AssignSeeking(ICharacter commander, ICharacter exactTarget) => AssignMission(commander, "seek", exactTarget);
	public bool AssignHaunting(ICharacter commander, ICharacter exactTarget) => exactTarget is not null && DeadlineUtc is { } deadline && deadline > RuntimeClock.UtcNow &&
		_isDurablyPrimaryTarget(exactTarget) && AssignMission(commander, "haunt", exactTarget);
	internal static bool IsDurablyPrimaryTarget(ICharacter target)
	{
		var characterId = CharacterInstanceIdentityComparer.IdentityId(target);
		var bodyId = target.Body?.Id ?? 0;
		if (characterId <= 0 || target.InstanceId <= 0 || bodyId <= 0) return false;
		try
		{
			using var isolated = FMDB.BeginIndependentScope();
			using var db = new FMDB();
			return IsDurablyPrimaryTarget(FMDB.Context, target);
		}
		catch { return false; }
	}
	internal static bool IsDurablyPrimaryTarget(FuturemudDatabaseContext context, ICharacter target)
	{
		var characterId = CharacterInstanceIdentityComparer.IdentityId(target);
		var bodyId = target.Body?.Id ?? 0;
		if (characterId <= 0 || target.InstanceId <= 0 || bodyId <= 0 ||
			!context.Characters.AsNoTracking().Any(x => x.Id == characterId && !x.IsArchived)) return false;
		var primary = context.CharacterInstances.AsNoTracking()
			.Where(x => x.CharacterId == characterId && x.IsPrimary)
			.Select(x => new { x.Id, x.CharacterId, x.BodyId, x.IsPrimary })
			.ToArray();
		return primary.Length == 1 && IsExactPrimaryFearTarget(primary[0].Id, primary[0].CharacterId,
			primary[0].BodyId, primary[0].IsPrimary, target.InstanceId, characterId, bodyId);
	}
	public bool AssignCourier(ICharacter commander, IGameItem exactParcel, ICharacter exactRecipient)
		=> AssignCourierCore(commander, exactParcel, exactRecipient, null);
	private bool AssignCourierCore(ICharacter commander, IGameItem exactParcel, ICharacter exactRecipient, Func<bool>? inputsCurrent)
	{
		if (!CanAcceptOrder(commander) || exactParcel is null || exactRecipient is null || exactRecipient.Id == Creature.Id ||
			!ReferenceEquals(exactParcel.Gameworld, Gameworld) || !ReferenceEquals(Gameworld.Items.Get(exactParcel.Id), exactParcel) ||
			!commander.Body.HeldOrWieldedItems.Any(x => ReferenceEquals(x, exactParcel)) || !IsExactLoaded(exactRecipient) ||
			inputsCurrent?.Invoke() == false) return false;
		return ReplaceMission(new SpellNpcMissionEffect(Creature, "courier", exactRecipient, exactRecipient, exactParcel, DeadlineUtc));
	}
	/// <summary>Invocation-only assignment: preserve the paid physical frame after live authority/visibility callbacks.</summary>
	internal bool AssignPreparedMission(ICharacter commander, SpellNpcMissionInputs inputs, Func<bool> inputsCurrent)
	{
		bool Current() => inputsCurrent() && !Creature.EffectsOfType<SpellNpcMissionEffect>().Any();
		if (!Current()) return false;
		return inputs.Kind switch
		{
			SpellNpcMissionKind.Seek => AssignMission(commander, "seek", inputs.ExactTarget, Current),
			SpellNpcMissionKind.Courier when inputs.ExactParcel is { } parcel => AssignCourierCore(commander, parcel, inputs.ExactTarget, Current),
			SpellNpcMissionKind.Haunt when DeadlineUtc is { } deadline && deadline > RuntimeClock.UtcNow &&
				_isDurablyPrimaryTarget(inputs.ExactTarget) => AssignMission(commander, "haunt", inputs.ExactTarget, Current),
			_ => false
		};
	}

	public bool ClearMission(ICharacter commander)
	{
		if (!CanAcceptOrder(commander)) return false;
		var mission = Creature.EffectsOfType<SpellNpcMissionEffect>().SingleOrDefault();
		if (mission is null) return true;
		var relatedTarget = mission.RemoveOwnedFearParent();
		Creature.RemoveEffect(mission, true);
		if (PersistMissionState(relatedTarget is null ? [] : [relatedTarget])) return true;
		_persistenceUncertain = true;
		return false;
	}
	private bool AssignMission(ICharacter commander, string kind, ICharacter target, Func<bool>? inputsCurrent = null)
	{
		if (!CanAcceptOrder(commander) || target is null || target.Id == Creature.Id || !IsExactLoaded(target) ||
			!Creature.CanSee(target) || !Creature.ColocatedWith(target) || inputsCurrent?.Invoke() == false) return false;
		// Live callbacks may change persisted primary identity or consume the remaining lifetime.
		// Check both after those callbacks, then fence the paid physical frame after the database read.
		if (kind == "haunt" && (DeadlineUtc is not { } deadline || !_isDurablyPrimaryTarget(target) ||
			deadline <= RuntimeClock.UtcNow || inputsCurrent?.Invoke() == false)) return false;
		return ReplaceMission(new SpellNpcMissionEffect(Creature, kind, target, expiryUtc: DeadlineUtc));
	}
	private bool IsExactLoaded(ICharacter character) => ReferenceEquals(character.Gameworld, Gameworld) &&
		Gameworld.Actors.Any(x => ReferenceEquals(x, character)) &&
		CharacterInstanceIdentityComparer.IdentityId(character) > 0 && character.InstanceId > 0 &&
		!character.State.HasFlag(CharacterState.Dead) && !character.State.HasFlag(CharacterState.Stasis);
	private bool ReplaceMission(SpellNpcMissionEffect mission)
	{
		var existingMissions = Creature.EffectsOfType<SpellNpcMissionEffect>().ToArray();
		var relatedTargets = existingMissions.Select(x => x.RemoveOwnedFearParent()).Where(x => x is not null).Cast<ICharacter>().ToArray();
		foreach (var existing in existingMissions) Creature.RemoveEffect(existing, true);
		Creature.AddEffect(mission);
		if (PersistMissionState(relatedTargets)) return true;
		_persistenceUncertain = true;
		return false;
	}

	public bool LoseControl(ICharacter commander)
	{
		if (ControlLost || _retiring || !CanAcceptOrder(commander)) return false;
		ControlLost = true; Intent = "none"; TargetId = null; TargetInstanceId = null; Changed = true;
		if (!PersistState())
		{
			_persistenceUncertain = true;
			return false;
		}
		if (Creature.Following is ICharacter following && following.InstanceId == CreatorInstanceId &&
			CharacterInstanceIdentityComparer.IdentityId(following) == CreatorId) Creature.CeaseFollowing();
		AttemptHostileAction();
		return true;
	}

	public bool SetOrder(string intent, ICharacter? target, ICharacter commander)
	{
		if (_retiring || ControlLost || !CanAcceptOrder(commander) || intent is not ("none" or "follow" or "guard" or "attack")) return false;
		if (intent == "attack" && (target is null || ReferenceEquals(target, Creature) || !IsExactLoaded(target))) return false;
		if (intent is "follow" or "guard") target = ResolveCreator();
		Intent = intent; TargetId = target is null ? null : CharacterInstanceIdentityComparer.IdentityId(target);
		TargetInstanceId = target?.InstanceId; Changed = true;
		ICharacter[] relatedTargets = [];
		if (intent == "none")
		{
			var missions = Creature.EffectsOfType<SpellNpcMissionEffect>().ToArray();
			relatedTargets = missions.Select(x => x.RemoveOwnedFearParent()).Where(x => x is not null).Cast<ICharacter>().ToArray();
			foreach (var mission in missions) Creature.RemoveEffect(mission, true);
		}
		// The order and mission cancellation must survive together; a saved stop cannot retain a live mission.
		if (!PersistMissionState(relatedTargets)) { _persistenceUncertain = true; return false; }
		if (_active) SetFollowHeartbeat(Intent is "follow" or "guard");
		if (intent == "none" && Creature.Following is ICharacter following && following.InstanceId == CreatorInstanceId &&
			CharacterInstanceIdentityComparer.IdentityId(following) == CreatorId) Creature.CeaseFollowing();
		else if (intent == "follow" && target is not null && Creature.ColocatedWith(target)) Creature.Follow(target);
		else if (intent == "attack" && target is not null) EngageIfAllowed(target);
		return true;
	}

	private bool PersistState()
	{
		try
		{
			using var isolated = FMDB.BeginIndependentScope(requireWrites: true);
			using var db = new FMDB();
			using var transaction = FMDB.Context.Database.BeginTransaction();
			Creature.Save();
			if (FMDB.Context.SaveChanges() <= 0) return false;
			transaction.Commit();
			return true;
		}
		catch
		{
			return false;
		}
	}

	public bool PrepareRetirement(Guid lifecycleId)
	{
		if (LifecycleId != lifecycleId) return false;
		_retiring = true; Intent = "none"; TargetId = null; TargetInstanceId = null; Changed = true;
		Deactivate();
		var missions = Creature.EffectsOfType<SpellNpcMissionEffect>().ToArray();
		var relatedTargets = missions.Select(x => x.RemoveOwnedFearParent()).Where(x => x is not null).Cast<ICharacter>().ToArray();
		foreach (var mission in missions) Creature.RemoveEffect(mission, true);
		if (missions.Length > 0 && !PersistMissionState(relatedTargets)) return false;
		if (Creature.Following is ICharacter following && following.InstanceId == CreatorInstanceId && CharacterInstanceIdentityComparer.IdentityId(following) == CreatorId)
			Creature.CeaseFollowing();
		Creature.RemoveEffect(this, true); return true;
	}

	public void NotifyCreatorEngagement(ICharacter attacker, ICharacter target)
	{
		if (Intent == "guard" && !ControlLost && !_persistenceUncertain && target.InstanceId == CreatorInstanceId &&
			CharacterInstanceIdentityComparer.IdentityId(target) == CreatorId && HasCurrentAuthority()) EngageIfAllowed(attacker);
	}
	public void AttemptHostileAction()
	{
		if (!_retiring && ControlLost && !_persistenceUncertain && HasCurrentAuthority() && ResolveCreator() is { } creator) EngageIfAllowed(creator);
	}
	private void EngageIfAllowed(ICharacter target)
	{
		if (_persistenceUncertain || _retiring || Gameworld.SpellOwnedNpcs?.HasPendingRetirement(Creature, CreatorId) == true ||
			!HasCurrentAuthority() || !Creature.State.IsAble() || !Creature.CanSee(target) || !Creature.CanEngage(target) ||
			ReferenceEquals(Creature.CombatTarget, target)) return;
		Creature.Engage(target, ranged: !Creature.ColocatedWith(target));
	}
	private void OwnerDied(IPerceivable _) => Creature.RemoveEffect(this, true);
	private void OwnerQuit(IPerceivable _) => Deactivate();
	private void Deactivate()
	{
		if (!_active) return; _active = false; SetFollowHeartbeat(false); Creature.OnQuit -= OwnerQuit; Creature.OnDeath -= OwnerDied;
	}
	public override void RemovalEffect() => Deactivate();
}
