#nullable enable

using System.Globalization;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MudSharp.Magic.SpellEffects;
using MudSharp.NPC.AI;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete.SpellEffects;

public sealed class SpellStormLocusEffect : MagicSpellEffectBase, IIndependentlyExpiringSpellEffect
{
	private bool _registered;
	private bool _checking;
	private bool _removed;
	private readonly XElement _policy;
	public SpellLifecycleOrigin Origin { get; }
	public DateTime? ExpiryUtc => Origin.DeadlineUtc;
	public static void InitialiseEffectType() => RegisterFactory("SpellStormLocus", (xml, owner) => new SpellStormLocusEffect(xml, owner));
	public SpellStormLocusEffect(IRoom owner, IMagicSpellEffectParent parent, SpellLifecycleOrigin origin, XElement policy) : base(owner, parent, null)
	{ Origin = origin; _policy = new(policy); }
	private SpellStormLocusEffect(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		var root = xml.Element("Effect")!; var anchor = SpellReturnAnchor.Parse(root.Element("Anchor")!.Value);
		Origin = new(Guid.Parse(root.Element("Origin")!.Value), (long)root.Element("Spell")!, (int)root.Element("Grade")!, anchor.CharacterId,
			SpellRoomHazardJournal.Family, SpellLifecycleMode.TemporaryCleanup,
			DateTime.Parse(root.Element("CreatedUtc")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).ToUniversalTime(),
			DateTime.Parse(root.Element("DeadlineUtc")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).ToUniversalTime(), anchor.Save());
		SpellOwnedLifecycleStore.ValidateHazardJournal(Origin); _policy = new(root.Element("Policy")!.Elements().Single());
	}
	private bool Attached() => !_removed && ParentEffect is { } parent && ReferenceEquals(parent.Owner, Owner) &&
		Owner.Effects.Any(x => ReferenceEquals(x, this)) && Owner.Effects.Any(x => ReferenceEquals(x, parent)) &&
		parent.SpellEffects.Any(x => ReferenceEquals(x, this));
	public override void InitialEffect() => Register(false);
	public override void Login() => Register(true);
	private void Register(bool resume)
	{
		if (_registered || _removed) return;
		_registered = true; Gameworld.HeartbeatManager.SecondHeartbeat += Pulse;
		if (!resume || Gameworld.SpellRoomHazards is not { } journal) return;
		var now = RuntimeClock.UtcNow;
		if (now >= Origin.DeadlineUtc) { Stop(SpellRetirementReason.Expiry); return; }
		// Consume an overdue cursor on load without inflicting offline catch-up damage.
		if (journal.Find(Origin.Id) is { State: SpellLifecycleState.Active } life && life.RoomHazardNextPulseUtc <= now)
		{
			try { journal.ClaimPulse(Origin.Id, life.Version, now); }
			catch (Exception ex) { journal.Hold(Origin.Id, now, ex.Message); Owner.RemoveEffect(this, true); }
		}
	}
	public void Pulse()
	{
		if (_checking || !Attached() || Owner is not IRoom room || Gameworld.SpellRoomHazards is not { } journal) return;
		_checking = true;
		try
		{
			var now = RuntimeClock.UtcNow; var life = journal.Find(Origin.Id);
			if (life is null || life.Origin != Origin || life.State != SpellLifecycleState.Active || life.Diagnostic.StartsWith(SpellRoomHazardJournal.HeldPrefix, StringComparison.Ordinal))
			{ Owner.RemoveEffect(this, true); return; }
			if (now >= Origin.DeadlineUtc) { Stop(SpellRetirementReason.Expiry); return; }
			if (life.RoomHazardNextPulseUtc > now) return;
			// A durable claim precedes every effect callback and native wound. Uncertain commits never replay.
			try { journal.ClaimPulse(Origin.Id, life.Version, now); }
			catch (Exception ex) { journal.Hold(Origin.Id, now, ex.Message); Owner.RemoveEffect(this, true); return; }
			var anchor = SpellReturnAnchor.Parse(Origin.Provenance);
			var caster = Gameworld.Actors.SingleOrDefault(x => CharacterInstanceIdentityComparer.IdentityId(x) == anchor.CharacterId &&
				x.InstanceId == anchor.InstanceId && x.Body?.Id == anchor.BodyId);
			// No lazy offline actor lookup, substitute source, or per-tick casting/progression.
			if (caster is null || !CommandExecutionAuthority.IsCurrent(caster, false)) return;
			if (room.Id != anchor.RoomId || room.CurrentOverlay.Id != anchor.OverlayId || room.CurrentOverlay.Terrain.Id != anchor.TerrainId ||
				!StormLocusEffect.RoomAllowed(caster, room, Spell, _policy)) { Stop(SpellRetirementReason.AnchorSevered); return; }
			var occupants = room.Characters.Take(257).ToArray();
			if (occupants.Length > 256) { journal.Hold(Origin.Id, now, "The room exceeds the 256-occupant pulse bound."); return; }
			var victims = occupants.Where(x => x.Body is { Id: > 0 } && !x.IsAdministrator() && !x.State.IsDead() &&
				x is ICharacterInstance { InstanceKind: CharacterInstanceKind.Primary } &&
				caster.CanInteractPlanar(x, PlanarInteractionKind.Magic) && !x.GetPlanarPresence().SuspendsPhysicalContact &&
				MagicInterdictionHelper.GetInterdiction(caster, x, Spell.School, false) is null)
				.DistinctBy(CharacterInstanceIdentityComparer.IdentityId).ToArray();
			foreach (var victim in victims)
			{
				if (!Attached()) return;
				var body = victim.Body; var snapshot = new SpellTravelSnapshot(Gameworld, [caster, victim], [room, caster.Location]);
				bool Current() => Attached() && snapshot.AtCapturedPositions && ReferenceEquals(victim.Location, room) &&
					StormLocusEffect.RoomAllowed(caster, room, Spell, _policy) &&
					MagicInterdictionHelper.GetInterdiction(caster, victim, Spell.School, false) is null && snapshot.AtCapturedPositions;
				var part = body.RandomBodypart;
				if (part is null || !Current() || !body.Bodyparts.Any(x => ReferenceEquals(x, part))) continue;
				try
				{
					using var independent = CommandExecutionScope.EnterIndependent();
					var damage = new Damage { ActorOrigin = caster, TargetBody = body, Bodypart = part, DamageType = DamageType.Electrical,
						DamageAmount = 2 * Origin.Grade, PainAmount = 0, StunAmount = 0, ShockAmount = 0,
						AngleOfIncidentRadians = Math.PI * 0.5, PenetrationOutcome = new CheckOutcome { Outcome = Outcome.MajorPass },
						MutationGuard = new PulseGuard(body, part, Current) };
					body.PassiveSufferDamage(damage).ProcessPassiveWounds(body);
				}
				catch (StalePulseException) { /* This committed pulse never retries a changed body or custody. */ }
			}
		}
		catch (Exception ex)
		{
			journal.Hold(Origin.Id, RuntimeClock.UtcNow, ex.Message);
			Gameworld.SystemMessage("Room hazard pulse needs review: " + ex.Message, true);
		}
		finally { _checking = false; }
	}
	private sealed class StalePulseException : InvalidOperationException { }
	private sealed class PulseGuard(IBody body, IBodypart part, Func<bool> current) : IDamageMutationGuard
	{
		private readonly IGameItem[] _items = body.WornItems.Concat(body.Prosthetics.Select(x => x.Parent)).ToArray();
		private readonly HashSet<IGameItem> _processed = new(ReferenceEqualityComparer.Instance);
		public void Validate(IHaveWounds recipient, IDamage damage)
		{
			bool Worn(IGameItem item) => body.WornItems.Concat(body.Prosthetics.Select(x => x.Parent)).Any(x => ReferenceEquals(x, item));
			var projected = recipient is IGameItem item && _items.Any(x => ReferenceEquals(x, item)) && !item.Deleted && !item.Destroyed && Worn(item);
			var partValid = body.Bodyparts.Any(x => ReferenceEquals(x, part)) &&
				(projected && damage.Bodypart is null || damage.Bodypart is not null && body.Bodyparts.Concat(body.Bones).Concat(body.Organs).Any(x => ReferenceEquals(x, damage.Bodypart)));
			if (!current() || !_items.All(x => _processed.Contains(x) || Worn(x)) || !ReferenceEquals(damage.MutationGuard, this) ||
				!ReferenceEquals(damage.TargetBody, body) || !(ReferenceEquals(recipient, body) || projected) || !partValid) throw new StalePulseException();
			if (recipient is IGameItem processed) _processed.Add(processed);
		}
	}
	private void Stop(SpellRetirementReason reason)
	{
		Gameworld.SpellRoomHazards?.Retire(Origin.Id, reason, RuntimeClock.UtcNow); Owner.RemoveEffect(this, true);
	}
	public override void RemovalEffect()
	{
		_removed = true;
		if (_registered) { _registered = false; Gameworld.HeartbeatManager.SecondHeartbeat -= Pulse; }
		Gameworld.SpellRoomHazards?.Retire(Origin.Id, SpellRetirementReason.Dispel, RuntimeClock.UtcNow); base.RemovalEffect();
	}
	protected override string SpecificEffectType => "SpellStormLocus";
	protected override XElement SaveDefinition() => new("Effect", new XElement("Origin", Origin.Id), new XElement("Spell", Origin.SpellId),
		new XElement("Grade", Origin.Grade), new XElement("CreatedUtc", Origin.CreatedUtc.ToString("O", CultureInfo.InvariantCulture)),
		new XElement("DeadlineUtc", Origin.DeadlineUtc!.Value.ToString("O", CultureInfo.InvariantCulture)), new XElement("Anchor", new XCData(Origin.Provenance)), new XElement("Policy", new XElement(_policy)));
	public override string Describe(IPerceiver voyeur) => $"An electrical storm locus (grade {Origin.Grade.ToString("N0", voyeur)}) until {Origin.DeadlineUtc?.ToString("u", voyeur)}.";
}
