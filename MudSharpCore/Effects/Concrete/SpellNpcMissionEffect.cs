#nullable enable

using System.Xml.Linq;
using System.Globalization;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Movement;
using MudSharp.RPG.Checks;
using MudSharp.Combat;
using MudSharp.Effects.Concrete.SpellEffects;

namespace MudSharp.Effects.Concrete;

/// <summary>Saved, exact-identity mission assignment for a native spell companion.</summary>
public sealed class SpellNpcMissionEffect : Effect
{
	public string Mission { get; private set; }
	public string Phase { get; private set; }
	public long TargetId { get; private set; }
	public long TargetInstanceId { get; private set; }
	public long TargetBodyId { get; private set; }
	public long RecipientId { get; private set; }
	public long RecipientInstanceId { get; private set; }
	public long ParcelId { get; private set; }
	public Guid? FearParentIdentity { get; private set; }
	public DateTime? DeadlineUtc { get; private set; }
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "SpellNpcMission";
	private ICharacter Creature => (ICharacter)Owner;

	public SpellNpcMissionEffect(ICharacter owner, string mission, ICharacter target, ICharacter? recipient = null, IGameItem? parcel = null, DateTime? expiryUtc = null) : base(owner)
	{
		Mission = mission; Phase = mission == "courier" ? "pickup" : "active";
		TargetId = mission == "courier" ? 0 : CharacterInstanceIdentityComparer.IdentityId(target);
		TargetInstanceId = mission == "courier" ? 0 : target.InstanceId;
		TargetBodyId = mission == "courier" ? 0 : target.Body?.Id ?? 0;
		RecipientId = recipient is null ? 0 : CharacterInstanceIdentityComparer.IdentityId(recipient);
		RecipientInstanceId = recipient?.InstanceId ?? 0; ParcelId = parcel?.Id ?? 0;
		DeadlineUtc = expiryUtc;
	}
	private SpellNpcMissionEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var data = root.Element("Effect")!;
		Mission = (string)data.Element("Mission")!; Phase = (string)data.Element("Phase")!;
		TargetId = (long?)data.Element("TargetId") ?? 0; TargetInstanceId = (long?)data.Element("TargetInstanceId") ?? 0;
		TargetBodyId = (long?)data.Element("TargetBodyId") ?? 0;
		RecipientId = (long?)data.Element("RecipientId") ?? 0; RecipientInstanceId = (long?)data.Element("RecipientInstanceId") ?? 0;
		ParcelId = (long?)data.Element("ParcelId") ?? 0;
		FearParentIdentity = Guid.TryParse(data.Element("FearParentIdentity")?.Value, out var fearIdentity) ? fearIdentity : null;
		DeadlineUtc = DateTime.TryParse(data.Element("DeadlineUtc")?.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var deadline) ? deadline : null;
		var phaseIsValid = Mission switch
		{
			"seek" => Phase is "active" or "complete",
			"courier" => Phase is "pickup" or "travel" or "delivered" or "return" or "complete",
			"haunt" => Phase is "active" or "haunting" or "complete",
			_ => false
		};
		var completedHasNoReferences = Phase != "complete" ||
			(TargetId == 0 && TargetInstanceId == 0 && TargetBodyId == 0 && RecipientId == 0 && RecipientInstanceId == 0 && ParcelId == 0 &&
			 !FearParentIdentity.HasValue && !DeadlineUtc.HasValue);
		var phaseHasRequiredReferences = Phase == "complete" || Mission switch
		{
			"seek" => TargetId > 0 && TargetInstanceId > 0 && TargetBodyId > 0 && RecipientId == 0 && RecipientInstanceId == 0 && ParcelId == 0 && !FearParentIdentity.HasValue,
			"courier" when Phase is "pickup" or "travel" => TargetId == 0 && TargetInstanceId == 0 && RecipientId > 0 && RecipientInstanceId > 0 && ParcelId > 0 && !FearParentIdentity.HasValue,
			"courier" => TargetId == 0 && TargetInstanceId == 0 && RecipientId == 0 && RecipientInstanceId == 0 && ParcelId == 0 && !FearParentIdentity.HasValue,
			"haunt" when Phase == "active" => TargetId > 0 && TargetInstanceId > 0 && TargetBodyId > 0 && RecipientId == 0 && RecipientInstanceId == 0 && ParcelId == 0 && !FearParentIdentity.HasValue && DeadlineUtc.HasValue,
			"haunt" when Phase == "haunting" => TargetId > 0 && TargetInstanceId > 0 && TargetBodyId > 0 && RecipientId == 0 && RecipientInstanceId == 0 && ParcelId == 0 && FearParentIdentity.HasValue && DeadlineUtc.HasValue,
			_ => false
		};
		if (!phaseIsValid || !completedHasNoReferences || !phaseHasRequiredReferences)
			throw new FormatException("Invalid spell NPC mission data.");
	}
	public static void InitialiseEffectType() => RegisterFactory("SpellNpcMission", (root, owner) => new SpellNpcMissionEffect(root, owner));
	protected override XElement SaveDefinition() => new("Effect", new XElement("Mission", Mission), new XElement("Phase", Phase),
		TargetId > 0 ? new XElement("TargetId", TargetId) : null, TargetInstanceId > 0 ? new XElement("TargetInstanceId", TargetInstanceId) : null,
		TargetBodyId > 0 ? new XElement("TargetBodyId", TargetBodyId) : null,
		RecipientId > 0 ? new XElement("RecipientId", RecipientId) : null,
		RecipientInstanceId > 0 ? new XElement("RecipientInstanceId", RecipientInstanceId) : null,
		new XElement("ParcelId", ParcelId),
		FearParentIdentity is { } identity ? new XElement("FearParentIdentity", identity) : null,
		DeadlineUtc.HasValue ? new XElement("DeadlineUtc", DeadlineUtc.Value) : null);
	public override IEnumerable<PhysicalEntityReference> PhysicalReferences
	{
		get
		{
			var references = new List<PhysicalEntityReference>();
			if (TargetId > 0) references.Add(new(PhysicalEntityKind.Character, TargetId, "TargetId"));
			if (TargetInstanceId > 0) references.Add(new(PhysicalEntityKind.CharacterInstance, TargetInstanceId, "TargetInstanceId"));
			if (TargetBodyId > 0) references.Add(new(PhysicalEntityKind.Body, TargetBodyId, "TargetBodyId"));
			if (RecipientId > 0) references.Add(new(PhysicalEntityKind.Character, RecipientId, "RecipientId"));
			if (RecipientInstanceId > 0) references.Add(new(PhysicalEntityKind.CharacterInstance, RecipientInstanceId, "RecipientInstanceId"));
			if (ParcelId > 0) references.Add(new(PhysicalEntityKind.GameItem, ParcelId, "ParcelId"));
			return references;
		}
	}
	public override string Describe(IPerceiver voyeur) => $"Native {Mission} mission ({Phase}).";

	internal ICharacter? ResolveTarget() => TargetId <= 0 || TargetInstanceId <= 0 || TargetBodyId <= 0 ? null : Gameworld.Actors.FirstOrDefault(x =>
		CharacterInstanceIdentityComparer.IdentityId(x) == TargetId && x.InstanceId == TargetInstanceId && x.Body?.Id == TargetBodyId &&
		!x.State.HasFlag(CharacterState.Dead) && !x.State.HasFlag(CharacterState.Stasis));
	internal ICharacter? ResolveRecipient() => RecipientId <= 0 || RecipientInstanceId <= 0 ? null : Gameworld.Actors.FirstOrDefault(x => CharacterInstanceIdentityComparer.IdentityId(x) == RecipientId && x.InstanceId == RecipientInstanceId && !x.State.HasFlag(CharacterState.Dead));
	internal IGameItem? ResolveParcel() => Gameworld.Items.Get(ParcelId);
	internal bool IsPhase(string phase) => Phase == phase;
	internal void SetPhase(string phase) { Phase = phase; Changed = true; }
	internal void Process(SpellNpcControlEffect control)
	{
		var creature = Creature;
		if (Phase == "complete") return;
		if (DeadlineUtc is { } deadline && deadline <= RuntimeClock.UtcNow)
		{
			var target = RemoveOwnedFearParent();
			CompleteAndRelease(control, target);
			return;
		}
		if (!control.CanPerformMission()) return;
		switch (Mission)
		{
			case "seek": if (ResolveTarget() is { } seekTarget) ProcessSeeking(control, creature, seekTarget); break;
			case "courier": ProcessCourier(control, creature); break;
			case "haunt": if (ResolveTarget() is { } hauntTarget) ProcessHaunting(control, creature, hauntTarget); break;
		}
	}
	private void ProcessSeeking(SpellNpcControlEffect control, ICharacter creature, ICharacter target)
	{
		if (creature.ColocatedWith(target)) return;
		var vision = Gameworld.GetCheck(CheckType.SearchForTracksCheck).CheckAgainstAllDifficulties(creature, Difficulty.Normal, null);
		var scent = Gameworld.GetCheck(CheckType.SearchForTracksByScentScheck).CheckAgainstAllDifficulties(creature, Difficulty.Normal, null);
		var exits = creature.Location.Tracks.Where(track => !track.Deleted && track.RoomLayer == creature.RoomLayer && !track.TurnedAround &&
			TrackMatchesExactTarget(track, TargetId, TargetInstanceId) && track.ToRoomExit is not null && track.MudDateTime.Calendar is { } calendar &&
			track.MudDateTime <= calendar.CurrentDateTime && calendar.CurrentDateTime - track.MudDateTime <= TimeSpan.FromHours(2) &&
			(vision[track.VisualTrackDifficulty(creature)].Outcome == Outcome.MajorPass || scent[track.OlfactoryTrackDifficulty(creature)].IsPass()))
			.Select(x => x.ToRoomExit!).Distinct().ToArray();
		if (exits.Length == 1 && creature.CanMove(exits[0])) creature.Move(exits[0]);
	}
	internal static bool TrackMatchesExactTarget(ITrack track, long characterId, long instanceId) =>
		track.CharacterId == characterId && track.CharacterInstanceId == instanceId && instanceId > 0;
	private void ProcessCourier(SpellNpcControlEffect control, ICharacter creature)
	{
		var creator = control.ResolveCreatorForMission();
		if (Phase is "delivered" or "return")
		{
			if (Phase == "delivered")
			{
				ReleaseCourierReferences(); SetPhase("return");
				if (!control.PersistMissionState()) control.HoldAfterUncertainMissionMutation();
			}
			if (Phase == "return" && creator is not null)
			{
				if (creature.ColocatedWith(creator)) CompleteAndRelease(control); else MoveOneBoundedExit(creature, creator);
			}
			return;
		}
		var parcel = ResolveParcel();
		if (parcel is null || parcel.Deleted || parcel.Destroyed) { control.HoldAfterUncertainMissionMutation(); return; }
		var recipient = Phase == "travel" ? ResolveRecipient() : null;
		if (Phase == "pickup")
		{
			if (IsHeld(creature.Body, parcel)) { SetPhase("travel"); PersistCustody(control, [creature], parcel); return; }
			if (creator is null || !creature.ColocatedWith(creator) || !IsHeld(creator.Body, parcel) || !creature.CanSee(parcel)) return;
			var quantity = parcel.Quantity;
			var transferred = creator.Body.GiveWithoutMerge(parcel, creature.Body);
			if (IsProvedCustodyTransfer(creator.Body, creature.Body, parcel, quantity, transferred))
			{ SetPhase("travel"); PersistCustody(control, [creator, creature], parcel); }
			else if (!IsHeld(creator.Body, parcel) || IsHeld(creature.Body, parcel)) control.HoldAfterUncertainMissionMutation();
			return;
		}
		if (Phase == "travel")
		{
			if (recipient is null) return;
			if (IsHeld(recipient.Body, parcel)) { SetPhase("return"); ReleaseCourierReferences(); PersistCustody(control, [creature, recipient], parcel); return; }
			if (!IsHeld(creature.Body, parcel)) { control.HoldAfterUncertainMissionMutation(); return; }
			if (!creature.ColocatedWith(recipient)) { MoveOneBoundedExit(creature, recipient); return; }
			if (!creature.CanSee(recipient) || !creature.Body.CanGive(parcel, recipient.Body)) return;
			var quantity = parcel.Quantity;
			var transferred = creature.Body.GiveWithoutMerge(parcel, recipient.Body);
			if (IsProvedCustodyTransfer(creature.Body, recipient.Body, parcel, quantity, transferred))
			{ SetPhase("return"); ReleaseCourierReferences(); PersistCustody(control, [creature, recipient], parcel); }
			else if (!IsHeld(creature.Body, parcel) || IsHeld(recipient.Body, parcel)) control.HoldAfterUncertainMissionMutation();
			return;
		}
	}
	private static bool IsHeld(IBody body, IGameItem parcel) => body.HeldOrWieldedItems.Any(x => ReferenceEquals(x, parcel));
	internal static bool IsProvedCustodyTransfer(IBody source, IBody destination, IGameItem parcel, int quantity, IGameItem? returned) =>
		ReferenceEquals(returned, parcel) && !parcel.Deleted && !parcel.Destroyed && parcel.Quantity == quantity &&
		parcel.ContainedIn is null && ReferenceEquals(parcel.InInventoryOf, destination) &&
		!IsHeld(source, parcel) && IsHeld(destination, parcel);
	private void PersistCustody(SpellNpcControlEffect control, IEnumerable<ICharacter> actors, IGameItem parcel)
	{
		if (!control.PersistMissionCustody(actors, parcel)) control.HoldAfterUncertainMissionMutation();
	}
	private void ProcessHaunting(SpellNpcControlEffect control, ICharacter creature, ICharacter target)
	{
		if (Phase == "haunting" && FearParentIdentity.HasValue) return;
		var targetBody = target.Body;
		var targetLocation = target.SpatialLocation;
		var caster = control.ResolveCreatorForMission();
		if (caster is null || !creature.ColocatedWith(target) || !creature.CanSee(target) || target.State.HasFlag(CharacterState.Dead)) return;
		if (Gameworld.MagicSpells.Get(control.SpellId) is not MagicSpell spell || !spell.ReadyForGame) return;
		var resistance = Gameworld.GetCheck(CheckType.ResistMagicSpellCheck).Check(target, spell.OpposedDifficulty ?? Difficulty.Normal, spell.OpposedTrait, caster);
		if (!creature.CanSee(target) || !control.CanPerformMission() || !ReferenceEquals(ResolveTarget(), target) || !ReferenceEquals(target.Body, targetBody) ||
			target.SpatialLocation != targetLocation || !creature.ColocatedWith(target)) return;
		if (resistance.IsPass()) { CompleteAndRelease(control); return; }
		if (DeadlineUtc is not { } deadline || deadline <= RuntimeClock.UtcNow) return;
		var parent = new MagicSpellParent(target, spell, caster);
		parent.RemoveWhenLoadedChildrenExpire = true;
		var fear = new SpellFearEffect(target, parent, expiryUtc: deadline);
		parent.AddSpellEffect(fear);
		FearParentIdentity = parent.Identity;
		target.AddEffect(fear);
		target.AddEffect(parent, deadline - RuntimeClock.UtcNow);
		SetPhase("haunting");
		if (!control.PersistMissionState(target)) control.HoldAfterUncertainMissionMutation();
	}
	private static void MoveOneBoundedExit(ICharacter creature, ICharacter target)
	{
		if (creature.Location.RouteDefinition is not null || target.Location.RouteDefinition is not null) return;
		var path = creature.PathBetween(target.Location, 6, _ => true).ToList();
		if (path.Count > 0 && creature.CanMove(path[0])) creature.Move(path[0]);
	}
	private void CompleteAndRelease(SpellNpcControlEffect control, ICharacter? relatedCharacter = null)
	{
		SetPhase("complete"); TargetId = TargetInstanceId = TargetBodyId = RecipientId = RecipientInstanceId = ParcelId = 0;
		FearParentIdentity = null; DeadlineUtc = null; Changed = true;
		if (relatedCharacter is null) control.PersistMissionState();
		else if (!control.PersistMissionState(relatedCharacter)) control.HoldAfterUncertainMissionMutation();
	}
	internal ICharacter? RemoveOwnedFearParent()
	{
		if (FearParentIdentity is not { } identity) return null;
		var target = ResolveTarget();
		var parent = target?.EffectsOfType<MagicSpellParent>().FirstOrDefault(x => x.Identity == identity);
		if (target is not null && parent is not null)
		{
			target.RemoveEffect(parent, true);
			return target;
		}
		var control = Creature.EffectsOfType<SpellNpcControlEffect>().SingleOrDefault();
		if (control is null || TargetId <= 0 || TargetInstanceId <= 0 || TargetBodyId <= 0 || control.CreatorId <= 0 || control.CreatorInstanceId <= 0)
			return null;
		control.QueueOfflineFearParentCleanup(new SpellNpcControlEffect.FearParentCleanup(TargetId, TargetInstanceId, TargetBodyId,
			identity, control.SpellId, control.CreatorId, control.CreatorInstanceId));
		return null;
	}
	private void ReleaseCourierReferences()
	{
		TargetId = TargetInstanceId = TargetBodyId = RecipientId = RecipientInstanceId = ParcelId = 0; Changed = true;
	}
	public override void RemovalEffect()
	{
		RemoveOwnedFearParent();
	}
}
