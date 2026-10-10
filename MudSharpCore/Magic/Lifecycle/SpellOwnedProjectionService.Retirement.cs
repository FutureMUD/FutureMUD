#nullable enable

using System.Data;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using RuntimeBody = MudSharp.Body.Implementations.Body;
using RuntimeCharacter = MudSharp.Character.Character;

namespace MudSharp.Magic.Lifecycle;

public sealed partial class SpellOwnedProjectionService
{
	internal static bool HasClaims(SpellOwnedLifecycle life)
	{
		if (life.Origin.Family != SpellProjectionAnchor.Family || life.Origin.Mode != SpellLifecycleMode.TemporaryCleanup ||
			life.Entities.Any(x => x.Role != SpellOwnedEntityRole.CreatedEntity)) return false;
		var c = SpellProjectionAnchor.Load(life.Origin.Provenance).Configuration;
		return life.Entities.Count == (c.Kind == SpellProjectionKind.SandEffigy ? 3 : 2) &&
			life.Entities.Count(x => x.Kind == SpellOwnedEntityKind.Body) == 1 && life.Entities.Count(x => x.Kind == SpellOwnedEntityKind.CharacterInstance) == 1 &&
			life.Entities.Count(x => x.Kind == SpellOwnedEntityKind.GameItem) == (c.Kind == SpellProjectionKind.SandEffigy ? 1 : 0);
	}
	public bool TryRetire(long instanceId, SpellRetirementReason reason, out string diagnostic)
	{
		using var authorization = FMDB.BeginIndependentScope(requireWrites: true);
		diagnostic = ""; var life = Find(instanceId);
		if (life is null) { diagnostic = "No owned projection exists for that exact instance."; return false; }
		if (life.State == SpellLifecycleState.Completed) return true;
		if (!_retiring.Add(instanceId)) { diagnostic = "Projection retirement is already running."; return false; }
		try
		{
			if (!HasClaims(life)) throw new InvalidOperationException("Projection ownership claims are unsupported or ambiguous.");
			var anchor = SpellProjectionAnchor.Load(life.Origin.Provenance);
			var bodyId = life.Entities.Single(x => x.Kind == SpellOwnedEntityKind.Body).Id;
			var itemId = life.Entities.SingleOrDefault(x => x.Kind == SpellOwnedEntityKind.GameItem)?.Id;
			if (world.TryGetCharacter(life.Origin.CreatorId, true) is not RuntimeCharacter owner || owner.Body.Id == bodyId || owner.InstanceId != anchor.InstanceId || owner.Body.Id != anchor.BodyId)
				throw new InvalidOperationException("The exact primary owner and anchor body must remain available; retain the temporary graph.");
			if (life.State == SpellLifecycleState.Active) life = _store.BeginRetirement(life.Origin.Id, life.Version, reason, Time(life));
			var projection = owner.Identity.Instances.OfType<ICharacter>().SingleOrDefault(x => x.InstanceId == instanceId);
			if (projection is not null && (projection.Body.Id != bodyId || projection.IsPrimaryInstance)) throw new InvalidOperationException("Loaded projection differs from its exact owned body.");
			Unbind(instanceId);
			owner.RemoveAllEffects<SpellProjectionTrance>(x => x.ProjectionInstanceId == instanceId, true);
			if (projection is not null)
			{
				CharacterInstanceFocusService.TryReturnFocusToPrimary(projection, "Your projection collapses and your focus returns to your primary body.", true, true);
				((RuntimeCharacter)projection).SetInstanceControllable(false);
			}
			// Journal the attempt before native health callbacks. An incomplete attempt is held for review, never replayed.
			if (life.Reason is SpellRetirementReason.AnchorSevered or SpellRetirementReason.ProjectionDamage && anchor.Configuration.BacklashDamage > 0)
			{
				if (life.Diagnostic.Contains("BacklashAttempt", StringComparison.Ordinal)) throw new InvalidOperationException("BacklashAttempt has incomplete native delivery; review without replay.");
				if (!life.Diagnostic.Contains("BacklashDelivered", StringComparison.Ordinal))
				{
					_store.Hold(life.Origin.Id, life.Version, "BacklashAttempt", Time(life)); life = _store.Find(life.Origin.Id)!;
					owner.SufferDamage(new Damage { DamageType = DamageType.Cellular, DamageAmount = anchor.Configuration.BacklashDamage,
						PainAmount = anchor.Configuration.BacklashDamage, Bodypart = owner.Body.Bodyparts.First(), TargetBody = owner.Body }).ToArray();
					owner.Body.Save();
					world.SaveManager.Flush();
					_store.Hold(life.Origin.Id, life.Version, "BacklashDelivered", Time(life)); life = _store.Find(life.Origin.Id)!;
				}
			}
			MudSharp.Models.Body? bodyRow; MudSharp.Models.CharacterInstance? instanceRow;
			using (var isolated = FMDB.BeginIndependentScope()) using (var db = new FMDB())
			{
				bodyRow = FMDB.Context.Bodies.Find(bodyId); instanceRow = FMDB.Context.CharacterInstances.Find(instanceId);
				if (instanceRow is not null && (instanceRow.IsPrimary || instanceRow.CharacterId != life.Origin.CreatorId || instanceRow.BodyId != bodyId ||
					!Guid.TryParse(XElement.Parse(instanceRow.EffectData).Element("OwnedProjection")?.Attribute("Lifecycle")?.Value, out var id) || id != life.Origin.Id))
					throw new InvalidOperationException("Persisted projection no longer matches its creation authority.");
				if (instanceRow is not null)
				{
					var metadata = XElement.Parse(instanceRow.EffectData);
					var claim = metadata.Element("OwnedProjection")!;
					if (metadata.Name != "Effects" || metadata.HasAttributes || metadata.Elements().Count() != 1 || claim.HasElements || claim.Attributes().Count() != 3 ||
						(long?)claim.Attribute("AnchorCharacterId") != life.Origin.CreatorId || (long?)claim.Attribute("AnchorInstanceId") != anchor.InstanceId ||
						instanceRow.InstanceKind != (int)(anchor.Configuration.Kind == SpellProjectionKind.SandEffigy ? CharacterInstanceKind.MagicalCopy : CharacterInstanceKind.AstralProjection) ||
						instanceRow.ControlPolicy != (int)CharacterInstanceControlPolicy.PlayerFocusable || instanceRow.DeathPolicy != (int)CharacterInstanceDeathPolicy.CollapseToAnchor ||
						instanceRow.PerceptionPolicy != (int)CharacterInstancePerceptionPolicy.PlanarProjection || instanceRow.PersistencePolicy != (int)CharacterInstancePersistencePolicy.DespawnOnReboot)
						throw new InvalidOperationException("Persisted projection policy or exact anchor metadata changed; retain it for recovery.");
				}
				if (FMDB.Context.Characters.Any(x => x.BodyId == bodyId) || FMDB.Context.CharacterInstances.Any(x => x.BodyId == bodyId && x.Id != instanceId) ||
					FMDB.Context.CharacterBodies.Any(x => x.BodyId == bodyId) || FMDB.Context.CharacterBodySources.Any(x => x.BodyId == bodyId))
					throw new InvalidOperationException("A canonical character, borrowed form or foreign instance retains this body.");
			}
			var body = projection?.Body as RuntimeBody ?? world.Bodies.Get(bodyId) as RuntimeBody;
			if (body is null && bodyRow is not null)
			{
				using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
				var currentBody = FMDB.Context.Bodies.Find(bodyId)!;
				body = new RuntimeBody(currentBody, world, owner); body.LoadInventory(currentBody); world.Add(body);
			}
			if (projection is null && body is not null && instanceRow is not null)
			{
				// Use the exact native secondary as an unexposed recovery custodian. The
				// primary's effects/controller must never become this body's custody context.
				projection = new PassiveCharacterInstance(owner, instanceRow, body);
				((RuntimeCharacter)projection).SetInstanceEmbodied(false);
				((RuntimeCharacter)projection).SetInstanceControllable(false);
			}
			if (body is not null && body.AllItems.Any())
			{
				var room = world.Rooms.Get(anchor.RoomId) ?? owner.Location;
				if (room is null || room.Temporary) throw new InvalidOperationException("Foreign goods require a stable native destination.");
				_custody.EvacuateBody(body, life, new SpatialLocation(room, (RoomLayer)anchor.Layer, anchor.RoutePosition),
					projection?.EffectsOfType<SpellProjectionBoundary>().Cast<MudSharp.Effects.IEffect>().ToArray() ?? []);
			}
			if (projection is not null)
			{
				// These native operations may run callbacks. Finish them before capturing
				// effects and checking references, while the durable graph still exists.
				projection.Movement?.CancelForMoverOnly(projection);
				projection.Combat?.LeaveCombat(projection);
				projection.CombatTarget = null;
				if (RuntimeDependencyError(projection) is { } dependencyError) throw new InvalidOperationException(dependencyError);
			}
			var ownedRuntimeEffects = projection?.Effects.Where(x => x.GetType() == typeof(SpellProjectionBoundary) ||
				x.GetType() == typeof(AdjacentToExit) && x is AdjacentToExit adjacent && ReferenceEquals(adjacent.Owner, projection) && adjacent.ApplicabilityProg is null && !adjacent.SavingEffect).ToArray() ?? [];
			if (projection is not null && projection.Effects.Any(x => !ownedRuntimeEffects.Contains(x)))
				throw new InvalidOperationException("Foreign projection effects require a cleanup adapter.");
			if (body is not null && !RetirementBodyEffects.TryCapture(body, out _))
				throw new InvalidOperationException("Foreign body effects require a cleanup adapter.");
			var runtimeTargets = new PhysicalReferenceTargets([new(PhysicalEntityKind.Body, bodyId, "owned body"), new(PhysicalEntityKind.CharacterInstance, instanceId, "owned projection")]);
			if (!PhysicalReferenceGuard.RuntimeReferencesAreClear(world, runtimeTargets))
				throw new InvalidOperationException("A live typed physical reference retains the projection.");
			// Typed physical references and declared EF relations are checked before any row removal.
			using (var isolated = FMDB.BeginIndependentScope(requireWrites: true)) using (var db = new FMDB())
			using (var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable))
			{
				var context = FMDB.Context;
				var current = context.MagicSpellLifecycles.Single(x => x.Id == life.Origin.Id);
				if (current.Version != life.Version || current.State != (int)SpellLifecycleState.Retiring) throw new InvalidOperationException("Projection retirement authority changed.");
				var wounds = context.Wounds.Where(x => x.BodyId == bodyId).ToArray();
				var targets = new PhysicalReferenceTargets(wounds.Select(x => new PhysicalEntityReference(PhysicalEntityKind.Wound, x.Id, "owned body wound"))
					.Append(new(PhysicalEntityKind.Body, bodyId, "owned body")).Append(new(PhysicalEntityKind.CharacterInstance, instanceId, "owned projection")));
				if (!PhysicalReferenceGuard.PersistedReferencesAreClear(context, targets, [], [bodyId], [instanceId], itemId, out var error)) throw new InvalidOperationException(error);
				var instance = context.CharacterInstances.Find(instanceId);
				var removed = new HashSet<object>(ReferenceEqualityComparer.Instance); foreach (var wound in wounds) removed.Add(wound); if (instance is not null) removed.Add(instance);
				if (!CharacterArchiveService.DeletedPrincipalReferencesAreClear(context, removed, out error)) throw new InvalidOperationException(error);
				if (context.Bodies.Find(bodyId) is { } ownedBody && !CharacterArchiveService.ProjectionBodyReferencesAreClear(context, ownedBody, removed, out error))
					throw new InvalidOperationException(error);
				if (instance is not null) context.CharacterInstances.Remove(instance);
				context.SaveChanges(); transaction.Commit();
			}
			if (projection is not null)
			{
				((RuntimeCharacter)projection).ReleaseCommittedProjectionRuntime(owner);
			}
			if (body is not null)
			{
				// Stop native body ticks even if a later database dependency holds cleanup.
				MudSharp.Form.Material.EnvironmentalExposureService.ForgetCommittedProjectionBody(world, body);
				body.ReleaseCommittedProjectionRuntime();
				body.Actor = owner;
				if (!owner.TryCleanupRetiredBody(body)) throw new InvalidOperationException("An exact body dependency still requires cleanup; preserve it for recovery.");
			}
			if (itemId is { } effigy)
			{
				var item = world.TryGetItem(effigy, true) as GameItem;
				if (item is not null)
				{
					if (item.DeepItems.Skip(1).Any() || item.AttachedAndConnectedItems.Any() || item.LodgedItems.Any() || item.Effects.Any() || item.Hooks.Any() || item.TargetedBy.Any())
						throw new InvalidOperationException("Foreign effigy dependencies require detachment; preserve the item.");
					using var isolated = FMDB.BeginIndependentScope(requireWrites: true); using var db = new FMDB();
					var row = FMDB.Context.GameItems.Find(effigy);
					if (row is not null)
					{
						if (!CharacterArchiveService.ProjectionItemReferencesAreClear(FMDB.Context, row, out var error)) throw new InvalidOperationException(error);
						FMDB.Context.GameItems.Remove(row); FMDB.Context.SaveChanges();
					}
					item.FinishCommittedSpellItemRemoval();
				}
			}
			_store.Complete(life.Origin.Id, life.Version, Time(life)); return true;
		}
		catch (Exception ex)
		{
			diagnostic = "Projection retirement held: " + ex.Message; life = _store.Find(life.Origin.Id)!;
			if (life.State != SpellLifecycleState.Completed) Hold(life, (life.Diagnostic.Contains("Backlash", StringComparison.Ordinal) ? life.Diagnostic + "; " : "") + diagnostic);
			return false;
		}
		finally { _retiring.Remove(instanceId); }
	}

	internal static string? RuntimeDependencyError(ICharacter projection)
	{
		if (projection.Movement is not null || projection.Combat is not null || projection.RidingMount is not null || projection.Riders.Any() ||
			projection.CharacterController is not null ||
			projection.CurrentProject.Project is not null || projection.Following is not null || projection.Party is not null ||
			projection.PositionTarget is not null || projection.TargetedBy.Any() ||
			projection.Gameworld.Vehicles.Any(x => x.IsOccupant(projection)) ||
			new MudSharp.Vehicles.VehicleHitchService().LinksInvolving(projection.Gameworld, projection).Any() ||
			projection.Gameworld.CombatArenas.SelectMany(x => x.ActiveEvents).Any(x =>
				x.Participants.Any(y => y.ActiveCharacter is { } actor && CharacterInstanceIdentityComparer.SamePhysicalInstance(projection, actor))))
			return "A native physical binding still retains the projection; detach it before cleanup.";
		return null;
	}
	public int ReconcileRetirements(DateTime nowUtc, int limit = 100)
	{
		if (nowUtc.Kind != DateTimeKind.Utc || limit is < 1 or > 1000) throw new ArgumentException("A bounded UTC projection pass is required.");
		using var authorization = FMDB.BeginIndependentScope(requireWrites: true);
		SpellOwnedLifecycle[] Read()
		{
			using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
			return FMDB.Context.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities).Where(x => x.Family == SpellProjectionAnchor.Family && x.State != (int)SpellLifecycleState.Completed &&
				x.Entities.Any(e => e.Kind == (int)SpellOwnedEntityKind.CharacterInstance && e.EntityId > _cursor))
				.OrderBy(x => x.Entities.Where(e => e.Kind == (int)SpellOwnedEntityKind.CharacterInstance).Select(e => e.EntityId).First()).Take(limit).AsEnumerable().Select(SpellOwnedLifecycleStore.Read).ToArray();
		}
		var pending = Read(); if (pending.Length == 0 && _cursor != 0) { _cursor = 0; pending = Read(); }
		foreach (var life in pending)
		{
			var id = life.Entities.Single(x => x.Kind == SpellOwnedEntityKind.CharacterInstance).Id; _cursor = id;
			if (life.State != SpellLifecycleState.Active) { TryRetire(id, life.Reason ?? SpellRetirementReason.Dismissal, out _); continue; }
			if (!string.IsNullOrEmpty(life.Diagnostic)) { TryRetire(id, SpellRetirementReason.Reboot, out _); continue; }
			if (life.Origin.DeadlineUtc <= nowUtc) { TryRetire(id, SpellRetirementReason.Expiry, out _); continue; }
			if (!_active.ContainsKey(id)) { TryRetire(id, SpellRetirementReason.Reboot, out _); continue; }
			var b = _active[id]; var anchor = SpellProjectionAnchor.Load(life.Origin.Provenance);
			if (b.Anchor.State.IsDead() || b.Anchor.Location?.Id != anchor.RoomId || (int)b.Anchor.RoomLayer != anchor.Layer || b.Anchor.Body.Id != anchor.BodyId ||
				b.Effigy is { } item && (item.Deleted || item.InInventoryOf is not null || item.ContainedIn is not null || item.Location?.Id != anchor.RoomId || (int)item.RoomLayer != anchor.Layer))
				TryRetire(id, SpellRetirementReason.AnchorSevered, out _);
		}
		return pending.Length;
	}
}
