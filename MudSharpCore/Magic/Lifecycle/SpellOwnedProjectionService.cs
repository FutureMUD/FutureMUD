#nullable enable

using System.Data;
using MudSharp.Accounts;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.NPC.Templates;
using RuntimeBody = MudSharp.Body.Implementations.Body;
using RuntimeCharacter = MudSharp.Character.Character;

namespace MudSharp.Magic.Lifecycle;

/// <summary>Owns only new temporary secondary bodies/instances and the optional exact effigy item.</summary>
public sealed partial class SpellOwnedProjectionService(IFuturemud world) : ISpellOwnedProjectionService
{
	private const string ActivationPending = "Projection activation pending";
	private readonly SpellOwnedLifecycleStore _store = new();
	private readonly Dictionary<long, Binding> _active = new();
	private readonly HashSet<long> _retiring = new();
	private readonly SpellOwnedNpcService _custody = new(world);
	private long _cursor;
	private sealed record Binding(SpellOwnedLifecycle Life, ICharacter Projection, ICharacter Anchor, IGameItem? Effigy,
		WoundEvent Damaged, LocatableEvent Moved, SpatialLocationEvent Positioned);

	public string? AdmissionError(ICharacter caster, SpellProjectionConfiguration c, int grade)
	{
		if (c.Error(grade) is { } error) return error;
		if (!ReferenceEquals(caster.Gameworld, world) || caster.Identity is not RuntimeCharacter || !caster.IsPrimaryInstance ||
			caster.Identity.PrimaryInstance.InstanceId != caster.InstanceId || !caster.IsPlayerCharacter || caster.IsGuest ||
			caster.State.IsDead() || caster.Location is null || caster.Location.Temporary || !caster.IsEmbodied || caster.Movement is not null || caster.RidingMount is not null || caster.Riders.Any())
			return "Projection creation requires a stationary, living player character focused on its primary body.";
		if (caster.RoutePositionMetres is not null || caster.Location.RouteDefinition is not null)
			return "Owned projections currently require a native room outside a route.";
		if (world.Planes.Get(c.PlaneId) is null || c.Kind == SpellProjectionKind.WalkingShadow && world.DefaultPlane.Id == c.PlaneId)
			return "Select an available nonmaterial plane for Walking Shadow.";
		if (c.Kind == SpellProjectionKind.SandEffigy && NativeItemCreationEligibility.Error(world.ItemProtos.Get(c.EffigyPrototypeId), world) is { } itemError)
			return itemError;
		if (caster.EffectsOfType<SpellProjectionTrance>().Any()) return "The existing shadow must collapse before creating another projection.";
		using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
		if (FMDB.Context.MagicSpellLifecycles.Any(x => x.CreatorId == caster.Identity.Id && x.Family == SpellProjectionAnchor.Family && x.State != (int)SpellLifecycleState.Completed))
			return "Your existing owned projection must finish retiring before another can be created.";
		if (!FMDB.Context.CharacterInstances.Any(x => x.Id == caster.InstanceId && x.IsPrimary && x.CharacterId == caster.Identity.Id && x.BodyId == caster.Body.Id))
			return "Projection creation requires the exact persisted primary instance and body.";
		return null;
	}

	public ICharacterInstance Create(ICharacter caster, SpellProjectionConfiguration c, SpellLifecycleOrigin origin)
	{
		origin.Validate();
		using var authorization = FMDB.BeginIndependentScope(requireWrites: true);
		if (origin.Mode != SpellLifecycleMode.TemporaryCleanup || origin.Family != SpellProjectionAnchor.Family || origin.CreatorId != caster.Identity.Id ||
			origin.DeadlineUtc != origin.CreatedUtc.AddSeconds(c.SecondsPerGrade * origin.Grade)) throw new InvalidOperationException("Invalid owned projection origin.");
		if (AdmissionError(caster, c, origin.Grade) is { } error) throw new InvalidOperationException(error);
		var identity = (RuntimeCharacter)caster.Identity;
		var anchor = new SpellProjectionAnchor(caster.InstanceId, caster.Body.Id, caster.Location.Id, (int)caster.RoomLayer, caster.RoutePositionMetres, c);
		origin = origin with { Provenance = anchor.Save().ToString(SaveOptions.DisableFormatting) };
		// The existing native body constructor receives only physical template state. No inventory, roles or canonical skills are inserted.
		var template = (SimpleCharacterTemplate)identity.GetCharacterTemplate();
		template = template with { SkillValues = [], SelectedRoles = [], SelectedKnowledges = [], SelectedMerits = [], SelectedProstheses = [],
			SelectedDisfigurements = [], SelectedScars = [], SelectedTattoos = [], MissingBodyparts = [], SelectedEntityDescriptionPatterns = [],
			SelectedSdesc = c.Kind == SpellProjectionKind.SandEffigy ? $"a sand effigy of {identity.PersonalName.GetName(MudSharp.Character.Name.NameStyle.FullName)}" : "a walking shadow",
			SelectedFullDesc = c.Kind == SpellProjectionKind.SandEffigy ? "An unmistakable sand effigy bound to a small figurine." : "A detached shadow, sustained by a distant consciousness." };
		var body = new RuntimeBody(world, identity, template, deferInitialisation: true);
		GameItem? item = c.Kind == SpellProjectionKind.SandEffigy ? new GameItem(world.ItemProtos.Get(c.EffigyPrototypeId), caster, ItemQuality.Standard, deferSpellInitialisation: true) : null;
		MudSharp.Models.Body? bodyRow = null; MudSharp.Models.CharacterInstance? instanceRow = null; MudSharp.Models.GameItem? itemRow = null;
		var components = new List<(GameItemComponent Component, MudSharp.Models.GameItemComponent Row)>();
		SpellOwnedLifecycle life;
		try
		{
			life = _store.Create(origin, birth =>
			{
				if (birth.Context.MagicSpellLifecycles.Any(x => x.CreatorId == identity.Id && x.Family == SpellProjectionAnchor.Family && x.State != (int)SpellLifecycleState.Completed))
					throw new InvalidOperationException("An unfinished projection already belongs to this identity.");
				bodyRow = (MudSharp.Models.Body)body.DatabaseInsert(); birth.Claim(SpellOwnedEntityKind.Body, bodyRow);
				instanceRow = new() { CharacterId = identity.Id, Body = bodyRow, IsPrimary = false, InstanceName = c.Kind.DescribeEnum(),
					InstanceKind = (int)(c.Kind == SpellProjectionKind.SandEffigy ? CharacterInstanceKind.MagicalCopy : CharacterInstanceKind.AstralProjection),
					ControlPolicy = (int)CharacterInstanceControlPolicy.PlayerFocusable, DeathPolicy = (int)CharacterInstanceDeathPolicy.CollapseToAnchor,
					PerceptionPolicy = (int)CharacterInstancePerceptionPolicy.PlanarProjection, PersistencePolicy = (int)CharacterInstancePersistencePolicy.DespawnOnReboot,
					LocationId = anchor.RoomId, RoomLayer = anchor.Layer, PositionId = (int)PositionStanding.Instance.Id, PositionModifier = (int)PositionModifier.None,
					PositionEmote = "", State = (int)CharacterState.Awake, Status = (int)CharacterStatus.Active, IsEmbodied = true, IsControllable = true,
					CreatedDateTime = origin.CreatedUtc, EffectData = new XElement("Effects", new XElement("OwnedProjection", new XAttribute("Lifecycle", origin.Id),
						new XAttribute("AnchorCharacterId", origin.CreatorId), new XAttribute("AnchorInstanceId", anchor.InstanceId))).ToString(SaveOptions.DisableFormatting) };
				birth.Context.CharacterInstances.Add(instanceRow); birth.Claim(SpellOwnedEntityKind.CharacterInstance, instanceRow);
				if (item is null) return;
				itemRow = (MudSharp.Models.GameItem)item.DatabaseInsert(); birth.Claim(SpellOwnedEntityKind.GameItem, itemRow);
				foreach (var component in item.Components.Cast<GameItemComponent>()) components.Add((component, (MudSharp.Models.GameItemComponent)component.DatabaseInsert()));
				birth.Context.RoomsGameItems.Add(new() { RoomId = anchor.RoomId, GameItem = itemRow }); itemRow.RoomLayer = anchor.Layer;
			}, ActivationPending);
		}
		catch { body.ReleaseArchivedRuntime(); throw; }
		try
		{
			body.SetIDFromCommittedProjection(bodyRow!);
			if (item is not null)
			{
				((SpellOwnedItemService)world.SpellOwnedItems!).RegisterClaimedItem(itemRow!.Id);
				item.ActivateCommittedSpellItem(itemRow!, origin, components); world.Add(item); item.RoomLayer = caster.RoomLayer; item.InsertAtSource(caster); item.Login();
			}
			var projection = (ICharacter)identity.MaterialiseSecondaryInstance(instanceRow!, body, instance =>
			{
				var presence = (ICharacter)instance;
				presence.AddEffect(new SpellProjectionBoundary(presence, anchor));
				if (c.Kind == SpellProjectionKind.WalkingShadow) identity.AddEffect(new SpellProjectionTrance(identity, instance.InstanceId));
				Bind(life, presence, caster, item);
			});
			world.Add(body);
			using var isolated = FMDB.BeginIndependentScope(requireWrites: true); using var db = new FMDB();
			var row = FMDB.Context.MagicSpellLifecycles.Single(x => x.Id == life.Origin.Id);
			if (row.Version != life.Version || row.State != (int)SpellLifecycleState.Active || row.Diagnostic != ActivationPending) throw new InvalidOperationException("Projection activation authority changed.");
			row.Diagnostic = ""; row.Version++; row.UpdatedUtc = RuntimeClock.UtcNow; FMDB.Context.SaveChanges();
			return (ICharacterInstance)projection;
		}
		catch (Exception ex)
		{
			if (_active.TryGetValue(instanceRow!.Id, out var binding))
			{
				CharacterInstanceFocusService.TryReturnFocusToPrimary(binding.Projection, "Projection activation failed; focus returns to your primary body.", true, true);
				((RuntimeCharacter)binding.Projection).SetInstanceControllable(false);
			}
			var failed = _store.Find(origin.Id)!;
			if (failed.State == SpellLifecycleState.Active) failed = _store.BeginRetirement(failed.Origin.Id, failed.Version, SpellRetirementReason.Reboot, Time(failed));
			Hold(failed, "Projection activation failed; creation must not replay: " + ex.Message);
			throw;
		}
	}

	private void Bind(SpellOwnedLifecycle life, ICharacter projection, ICharacter owner, IGameItem? item)
	{
		void Damage(IMortalPerceiver _, IWound wound)
		{
			if (wound.CurrentDamage <= 0 && wound.CurrentStun <= 0) return;
			RequestRetirement(projection, SpellRetirementReason.ProjectionDamage);
		}
		void Move(MudSharp.Form.Shape.ILocateable _, IRoomExit? __) => RequestRetirement(projection, SpellRetirementReason.AnchorSevered);
		void Position(MudSharp.Form.Shape.ILocateable _, SpatialLocation __, SpatialLocation ___) => RequestRetirement(projection, SpellRetirementReason.AnchorSevered);
		projection.Body.OnWounded += Damage; owner.OnLocationChanged += Move; owner.OnSpatialPositionChanged += Position;
		owner.Body.OnWounded += Damage; owner.OnDeath += AnchorDied;
		if (item is not null) { item.OnLocationChanged += Move; item.OnSpatialPositionChanged += Position; item.OnWounded += Damage; }
		_active.Add(projection.InstanceId, new(life, projection, owner, item, Damage, Move, Position));
		void AnchorDied(IPerceivable _) => RequestRetirement(projection, SpellRetirementReason.AnchorSevered);
		// Removal owns the exact OnDeath delegate too.
		_deathHandlers.Add(projection.InstanceId, AnchorDied);
	}
	private readonly Dictionary<long, PerceivableEvent> _deathHandlers = new();
	private void Unbind(long instanceId)
	{
		if (!_active.Remove(instanceId, out var b)) return;
		b.Projection.Body.OnWounded -= b.Damaged; b.Anchor.Body.OnWounded -= b.Damaged;
		b.Anchor.OnLocationChanged -= b.Moved; b.Anchor.OnSpatialPositionChanged -= b.Positioned;
		if (_deathHandlers.Remove(instanceId, out var died)) b.Anchor.OnDeath -= died;
		if (b.Effigy is not null) { b.Effigy.OnLocationChanged -= b.Moved; b.Effigy.OnSpatialPositionChanged -= b.Positioned; b.Effigy.OnWounded -= b.Damaged; }
	}
	public void RequestRetirement(ICharacter projection, SpellRetirementReason reason)
	{
		var life = Find(projection.InstanceId); if (life is null || life.State == SpellLifecycleState.Completed) return;
		if (life.State == SpellLifecycleState.Active) _store.BeginRetirement(life.Origin.Id, life.Version, reason, Time(life));
		CharacterInstanceFocusService.TryReturnFocusToPrimary(projection, "Your projection collapses and your focus returns to your primary body.", true, true);
		if (projection is RuntimeCharacter native) native.SetInstanceControllable(false);
		// Never delete a wound/body from inside its damage callback; the bounded world pulse finishes retirement.
	}
	private SpellOwnedLifecycle? Find(long id)
	{
		using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
		return FMDB.Context.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities).SingleOrDefault(x => x.Family == SpellProjectionAnchor.Family &&
			x.Entities.Any(e => e.Kind == (int)SpellOwnedEntityKind.CharacterInstance && e.EntityId == id)) is { } row ? SpellOwnedLifecycleStore.Read(row) : null;
	}
	public bool OwnsInstance(long instanceId) => Find(instanceId) is not null;
	public bool RequestAnchorRemoval(long itemId)
	{
		using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
		var row = FMDB.Context.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities).SingleOrDefault(x => x.Family == SpellProjectionAnchor.Family &&
			x.Entities.Any(e => e.Kind == (int)SpellOwnedEntityKind.GameItem && e.EntityId == itemId));
		if (row is null) return false;
		TryRetire(row.Entities.Single(x => x.Kind == (int)SpellOwnedEntityKind.CharacterInstance).EntityId, SpellRetirementReason.AnchorSevered, out _); return true;
	}
	private static DateTime Time(SpellOwnedLifecycle life) => RuntimeClock.UtcNow < life.UpdatedUtc ? life.UpdatedUtc : RuntimeClock.UtcNow;
	private void Hold(SpellOwnedLifecycle life, string diagnostic) => _store.Hold(life.Origin.Id, life.Version, diagnostic[..Math.Min(2048, diagnostic.Length)], Time(life));
}
