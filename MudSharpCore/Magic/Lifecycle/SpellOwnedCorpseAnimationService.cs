#nullable enable

using System.Data;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Accounts;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.NPC.AI;

namespace MudSharp.Magic.Lifecycle;

/// <summary>Durable borrowing of a real corpse. Only the new secondary row is ever owned or deleted.</summary>
public sealed class SpellOwnedCorpseAnimationService(IFuturemud world) : ISpellOwnedCorpseAnimationService
{
	private const string Prefix = "<CorpseAnimation ";
	private const string RetirementHeld = "Corpse animation restoration held:";
	private const string RestoredPrefix = "<CorpseRestore ";
	private readonly SpellOwnedLifecycleStore _store = new();
	private readonly HashSet<long> _retiring = new();
	private long _cursor;
	private IEnumerable<ICharacter> LoadedInstances() => world.Actors.Concat(world.CachedActors).Concat(world.Characters)
		.SelectMany(x => x.Identity.Instances.OfType<ICharacter>()).DistinctBy(x => x.InstanceId);

	internal static bool CanPersistPresentation(string source) =>
		new Borrow(long.MaxValue, long.MaxValue, long.MaxValue, long.MaxValue, int.MinValue).Encode(source).Length <= 2048;

	private sealed record Borrow(long Corpse, long Owner, long Body, long Room, int Layer)
	{
		public string Encode(string source) => new XElement("CorpseAnimation", new XAttribute("version", 1),
			new XAttribute("corpse", Corpse), new XAttribute("owner", Owner), new XAttribute("body", Body),
			new XAttribute("cell", Room), new XAttribute("layer", Layer), new XElement("Source", source)).ToString(SaveOptions.DisableFormatting);
		public static Borrow Read(SpellOwnedLifecycle life)
		{
			var root = XElement.Parse(life.Origin.Provenance);
			if (root.Name != "CorpseAnimation" || (int?)root.Attribute("version") != 1 ||
				life.Origin.Mode != SpellLifecycleMode.TemporaryCleanup || life.Entities.Count != 1 ||
				life.Entities[0] is not { Kind: SpellOwnedEntityKind.CharacterInstance, Role: SpellOwnedEntityRole.CreatedEntity })
				throw new InvalidOperationException("This journal does not authorize a single borrowed-corpse secondary.");
			return new((long)root.Attribute("corpse")!, (long)root.Attribute("owner")!, (long)root.Attribute("body")!,
				(long)root.Attribute("cell")!, (int)root.Attribute("layer")!);
		}
	}

	public string? AdmissionError(IGameItem item)
	{
		if (item is not GameItem || !ReferenceEquals(item.Gameworld, world) || item.Deleted || item.GetItemType<ICorpse>() is not { } corpse ||
			corpse.OriginalCharacter?.Identity is not Character.Character || corpse.OriginalBody is not { } body ||
			item.Location is not { } room || item.InInventoryOf is not null || item.ContainedIn is not null)
			return "An available native corpse must be directly present in a cell.";
		if (!corpse.RepresentsFinalCharacterDeath)
			return "Durable animation currently requires final-death remains; abandoned bodies need a cold-load adapter.";
		if (!corpse.OriginalCharacter.Identity.PrimaryInstance.State.IsDead())
			return "Durable animation requires the original canonical character to remain dead.";
		if (item.PositionTarget is not null || item.TargetedBy.Any() || item.AttachedAndConnectedItems.Any() || item.LodgedItems.Any())
			return "Release the corpse's external attachments and position targets before animation.";
		if (item.AffectedBy<IAnimatedCorpseEffect>() || item.AffectedBy<ICorpsePossessionEffect>() || IsBorrowedCorpse(item.Id) ||
			corpse.OriginalCharacter.Identity.Instances.Any(x => ReferenceEquals(x.Body, body) && x.IsEmbodied &&
				x is ICharacter actor && !actor.State.IsDead() && !actor.State.HasFlag(CharacterState.Stasis)))
			return "That corpse body is already in use.";
		if (RouteSpatialService.Instance.GetEffectiveLocation(item).RoutePositionMetres is not null)
			return "Route-position corpse animation needs a topology adapter.";
		using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
		return SpellOwnedCreation.PersistedCorpseError(FMDB.Context, item.Id,
			CharacterInstanceIdentityComparer.IdentityId(corpse.OriginalCharacter), body.Id, room.Id);
	}

	public ICharacter Create(IGameItem item, ICharacter caster, IReadOnlyCollection<IArtificialIntelligence> ais, SpellLifecycleOrigin origin)
	{
		origin.Validate();
		if (origin.Mode != SpellLifecycleMode.TemporaryCleanup || origin.CreatorId != CharacterInstanceIdentityComparer.IdentityId(caster) ||
			!ReferenceEquals(caster.Gameworld, world) || ais.Count == 0 || ais.Any(x => !x.IsReadyToBeUsed || !ReferenceEquals(x.Gameworld, world)))
			throw new InvalidOperationException("Corpse animation requires a finite temporary origin and selected ready AIs in this world.");
		if (AdmissionError(item) is { } error) throw new InvalidOperationException(error);
		var corpse = item.GetItemType<ICorpse>(); var identity = (Character.Character)corpse.OriginalCharacter.Identity;
		var borrow = new Borrow(item.Id, identity.Id, corpse.OriginalBody.Id, item.Location.Id, (int)item.RoomLayer);
		origin = origin with { Provenance = borrow.Encode(origin.Provenance) };
		MudSharp.Models.CharacterInstance? inserted = null;
		var life = _store.Create(origin, creation =>
		{
			inserted = new()
			{
				CharacterId = identity.Id, BodyId = borrow.Body, InstanceName = "animated corpse", IsPrimary = false,
				InstanceKind = (int)CharacterInstanceKind.AnimatedCorpse, ControlPolicy = (int)CharacterInstanceControlPolicy.ScriptOnly,
				DeathPolicy = (int)CharacterInstanceDeathPolicy.CollapseToAnchor, PerceptionPolicy = (int)CharacterInstancePerceptionPolicy.OrdinaryEmbodied,
				PersistencePolicy = (int)CharacterInstancePersistencePolicy.DespawnOnReboot, LocationId = borrow.Room, RoomLayer = borrow.Layer,
				PositionId = (int)PositionStanding.Instance.Id, PositionModifier = (int)PositionModifier.None, PositionEmote = "",
				State = (int)CharacterState.Awake, Status = (int)CharacterStatus.Active, IsEmbodied = true, IsControllable = true,
				CreatedDateTime = origin.CreatedUtc,
				EffectData = CharacterInstanceMetadata.CreateAnimatedCorpseEffectData(origin.CreatorId, caster.InstanceId,
					borrow.Corpse, borrow.Owner, borrow.Body, origin.SpellId, ais.Select(x => x.Id), CharacterInstancePersistencePolicy.DespawnOnReboot)
			};
			creation.Context.CharacterInstances.Add(inserted);
			creation.ClaimBorrowedCorpseAnimation(inserted, borrow.Corpse, borrow.Room);
		});
		try
		{
			if (Presentation(life, "Target") is { Length: > 0 } targetEcho) item.Handle(targetEcho.SubstituteANSIColour());
			foreach (var holder in identity.Instances.OfType<Character.Character>().Where(x => ReferenceEquals(x.Body, corpse.OriginalBody) && x.IsEmbodied))
			{ holder.SetInstanceEmbodied(false); holder.SetInstanceControllable(false); }
			using (ForeignCustodyTransferContext.EnterRemoval(null, [item], item.Location))
				((GameItem)item).HideBorrowedCorpseForAnimation();
			return (ICharacter)identity.MaterialiseSecondaryInstance(inserted!, corpse.OriginalBody);
		}
		catch (Exception ex)
		{
			Hold(life, "Corpse animation activation failed; exact committed instance needs restoration: " + ex.Message);
			TryRetire(inserted!.Id, SpellRetirementReason.Dismissal, out _);
			throw;
		}
	}

	private SpellOwnedLifecycle? Find(long instanceId)
	{
		using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
		return FMDB.Context.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities)
			.SingleOrDefault(x => x.Provenance.StartsWith(Prefix) && x.Entities.Any(e => e.Kind == (int)SpellOwnedEntityKind.CharacterInstance && e.EntityId == instanceId))
			is { } row ? SpellOwnedLifecycleStore.Read(row) : null;
	}

	public bool OwnsInstance(long instanceId) => Find(instanceId) is not null;
	public bool CanCommand(long instanceId, long commanderIdentityId) =>
		CommandGrant(instanceId, commanderIdentityId) is not null;
	public SpellLifecycleOrigin? CommandGrant(long instanceId, long commanderIdentityId) =>
		Find(instanceId) is { } life && HasCommandGrant(life, instanceId, commanderIdentityId, RuntimeClock.UtcNow)
			? life.Origin : null;

	internal static bool HasCommandGrant(SpellOwnedLifecycle life, long instanceId, long commanderIdentityId, DateTime now)
	{
		if (life.State != SpellLifecycleState.Active || life.Origin.CreatorId != commanderIdentityId ||
			now < life.Origin.CreatedUtc || now >= life.Origin.DeadlineUtc ||
			life.Entities.Count != 1 || life.Entities[0] is not { Kind: SpellOwnedEntityKind.CharacterInstance, Role: SpellOwnedEntityRole.CreatedEntity } entity || entity.Id != instanceId)
			return false;
		try
		{
			_ = Borrow.Read(life);
			var source = XElement.Parse(XElement.Parse(life.Origin.Provenance).Element("Source")!.Value);
			return source.Element("ControlUntilUtc") is { } element &&
				DateTime.TryParse(element.Value, System.Globalization.CultureInfo.InvariantCulture,
					System.Globalization.DateTimeStyles.RoundtripKind, out var until) &&
				until.Kind == DateTimeKind.Utc && until > life.Origin.CreatedUtc &&
				until <= life.Origin.DeadlineUtc && now < until;
		}
		catch (Exception ex) when (ex is System.Xml.XmlException or InvalidOperationException or NullReferenceException or FormatException)
		{ return false; }
	}
	public bool IsBorrowedCorpse(long corpseId)
	{
		using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
		var token = $" corpse=\"{corpseId}\"";
		return FMDB.Context.MagicSpellLifecycles.Any(x => x.State != (int)SpellLifecycleState.Completed &&
			x.Provenance.StartsWith(Prefix) && x.Provenance.Contains(token));
	}

	public bool TryRetire(long instanceId, SpellRetirementReason reason, out string diagnostic)
	{
		using var authorization = FMDB.BeginIndependentScope(requireWrites: true);
		diagnostic = "";
		var life = Find(instanceId);
		if (life is null) { diagnostic = "No owned corpse animation exists for that instance."; return false; }
		if (life.State == SpellLifecycleState.Completed) return true;
		if (!_retiring.Add(instanceId)) { diagnostic = "Corpse restoration is already in progress."; return false; }
		try
		{
			var borrow = Borrow.Read(life);
			var alreadyCommitted = life.Diagnostic.StartsWith(RestoredPrefix, StringComparison.Ordinal);
			var actor = LoadedInstances()
				.FirstOrDefault(x => x.InstanceId == instanceId && CharacterInstanceIdentityComparer.IdentityId(x) == borrow.Owner);
			if (life.State == SpellLifecycleState.Active)
			{
				if (reason == SpellRetirementReason.EarlyDeath)
				{
					actor?.Save();
					life = _store.ObserveDeath(life.Origin.Id, life.Version, null, Now(life));
				}
				else life = _store.BeginRetirement(life.Origin.Id, life.Version, reason, Now(life));
			}
			if (actor is ScriptedAiCharacterInstance controlled) controlled.SuspendForSpellRetirement();
			long savedRoom = borrow.Room; var savedLayer = borrow.Layer;
			using (var isolated = FMDB.BeginIndependentScope())
			using (var db = new FMDB())
			{
				var placed = FMDB.Context.RoomsGameItems.AsNoTracking().Where(x => x.GameItemId == borrow.Corpse).Select(x => (long?)x.RoomId).SingleOrDefault();
				var saved = FMDB.Context.CharacterInstances.AsNoTracking().SingleOrDefault(x => x.Id == instanceId);
				if (placed is { } placedRoom)
				{ savedRoom = placedRoom; savedLayer = FMDB.Context.GameItems.Where(x => x.Id == borrow.Corpse).Select(x => x.RoomLayer).Single(); }
				else if (saved?.LocationId is { } previousRoom)
				{ savedRoom = previousRoom; savedLayer = saved.RoomLayer; }
				if (saved?.RoutePosition is not null || actor?.RoutePositionMetres is not null)
					throw new InvalidOperationException("Route-position restoration requires a topology adapter; retain the body and corpse.");
			}
			if (alreadyCommitted)
			{
				var restored = XElement.Parse(life.Diagnostic.Split('\n')[0]);
				savedRoom = (long)restored.Attribute("cell")!; savedLayer = (int)restored.Attribute("layer")!;
			}
			var corpse = world.TryGetItem(borrow.Corpse, true) ?? throw new InvalidOperationException("The borrowed corpse is unavailable; preserve its body and inventory.");
			if (corpse.GetItemType<ICorpse>() is not { } component || component.OriginalBody?.Id != borrow.Body ||
				CharacterInstanceIdentityComparer.IdentityId(component.OriginalCharacter) != borrow.Owner || !component.OriginalCharacter.State.IsDead())
				throw new InvalidOperationException("The loaded corpse no longer has the exact borrowed body and identity.");
			if (actor is not null && (actor.CurrentProject.Project is not null || actor.RidingMount is not null || actor.Riders.Any() ||
				world.Vehicles.Any(x => x.IsOccupant(actor)) || new MudSharp.Vehicles.VehicleHitchService().LinksInvolving(world, actor).Any()))
				throw new InvalidOperationException("An occupied vehicle, riding or project binding needs release before restoration.");
			var destination = alreadyCommitted ? world.Rooms.Get(savedRoom) : actor?.Location ?? corpse.Location ?? world.Rooms.Get(savedRoom) ?? world.Rooms.Get(borrow.Room);
			if (destination is null) throw new InvalidOperationException("No loaded safe cell exists; retain this recoverable animation.");
			var layer = alreadyCommitted ? (RoomLayer)savedLayer : actor?.RoomLayer ?? (RoomLayer)savedLayer;
			CommitRestoration(life, borrow, destination.Id, layer);
			// The row deletion and exact corpse placement are durable before any runtime callback can throw.
			using (ForeignCustodyTransferContext.FreezeCustody())
				if (actor is ICharacterInstance physical && !CharacterInstanceService.RetireCore(physical, out var whyNot, false, actor.State.IsDead(), false, bypassCorpseLifecycle: true))
					throw new InvalidOperationException(whyNot);
			using (ForeignCustodyTransferContext.EnterRemoval(null, [corpse], destination))
			{
				if (corpse.Location is { } previous && !ReferenceEquals(previous, destination)) previous.Extract(corpse);
				corpse.RoomLayer = layer;
				if (!destination.GameItems.Contains(corpse)) destination.Insert(corpse, true);
				if (!ReferenceEquals(corpse.Location, destination) || corpse.RoomLayer != layer ||
					corpse.InInventoryOf is not null || corpse.ContainedIn is not null || corpse.Deleted || !destination.GameItems.Contains(corpse))
					throw new InvalidOperationException("Restoration callbacks changed the borrowed corpse's exact destination custody.");
			}
			corpse.RemoveAllEffects<IAnimatedCorpseEffect>(x => x.AnimatedInstanceId == instanceId, true);
			// A borrowed corpse has no world-item placement during boot and may be loaded here
			// without Login. Resume its retained timer without repeating component/effect login.
			if (corpse.CachedMorphTime is not null) corpse.StartMorphTimer();
			if (!alreadyCommitted && actor is not null)
			{
				using var custody = ForeignCustodyTransferContext.FreezeCustody();
				foreach (var echo in new[] { Presentation(life, "Collapse"), Presentation(life, "Restore") }.Where(x => !string.IsNullOrWhiteSpace(x)))
					destination.Handle(layer, new EmoteOutput(new Emote(echo, actor, actor, corpse), flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource));
			}
			life = _store.Find(life.Origin.Id)!;
			_store.Complete(life.Origin.Id, life.Version, Now(life));
			return true;
		}
		catch (Exception ex)
		{
			diagnostic = RetirementHeld + " " + ex.Message;
			Hold(_store.Find(life.Origin.Id)!, diagnostic);
			return false;
		}
		finally { _retiring.Remove(instanceId); }
	}

	private static void CommitRestoration(SpellOwnedLifecycle life, Borrow borrow, long cellId, RoomLayer layer)
	{
		using var isolated = FMDB.BeginIndependentScope(requireWrites: true); using var db = new FMDB();
		using var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable);
		var journal = FMDB.Context.MagicSpellLifecycles.Single(x => x.Id == life.Origin.Id);
		if (journal.Version != life.Version || journal.State != (int)life.State || !life.MayRemoveOwnedEntities)
			throw new InvalidOperationException("The retirement journal changed; reload before restoration.");
		var row = FMDB.Context.CharacterInstances.Find(life.Entities[0].Id);
		if (SpellOwnedCreation.BorrowedBodyOwnershipError(FMDB.Context, borrow.Owner, borrow.Body) is { } ownershipError)
			throw new InvalidOperationException(ownershipError);
		if (row is not null && (row.IsPrimary || row.CharacterId != borrow.Owner || row.BodyId != borrow.Body ||
			row.InstanceKind != (int)CharacterInstanceKind.AnimatedCorpse))
			throw new InvalidOperationException("The exact created secondary no longer matches its ownership proof.");
		if (FMDB.Context.CharacterInstances.AsNoTracking().Where(x => x.BodyId == borrow.Body && x.Id != life.Entities[0].Id && x.IsEmbodied)
			.AsEnumerable().Any(x => !((CharacterState)x.State).IsDead() && !((CharacterState)x.State).HasFlag(CharacterState.Stasis)))
			throw new InvalidOperationException("Another live instance has acquired the borrowed body; retain recovery state.");
		var corpse = FMDB.Context.GameItems.SingleOrDefault(x => x.Id == borrow.Corpse);
		if (corpse is null || !FMDB.Context.Rooms.Any(x => x.Id == cellId) ||
			!FMDB.Context.Characters.Any(x => x.Id == borrow.Owner) || !FMDB.Context.Bodies.Any(x => x.Id == borrow.Body) ||
			!FMDB.Context.GameItemComponents.Where(x => x.GameItemId == borrow.Corpse).Select(x => x.Definition)
				.AsEnumerable().Any(x => SpellOwnedCreation.IsExactCorpseDefinition(x, borrow.Owner, borrow.Body)))
			throw new InvalidOperationException("The borrowed corpse, canonical identity, body or destination is missing.");
		if (corpse.ContainerId is not null || FMDB.Context.BodiesGameItems.Any(x => x.GameItemId == borrow.Corpse))
			throw new InvalidOperationException("The corpse acquired a foreign custodian; release it before restoration.");
		var links = FMDB.Context.RoomsGameItems.Where(x => x.GameItemId == borrow.Corpse).ToArray();
		if (links.Any(x => x.RoomId != cellId)) throw new InvalidOperationException("The corpse is already placed in another cell.");
		if (links.Length == 0) FMDB.Context.RoomsGameItems.Add(new() { GameItemId = borrow.Corpse, RoomId = cellId });
		corpse.RoomLayer = (int)layer; corpse.RoutePosition = null;
		if (row is not null) FMDB.Context.CharacterInstances.Remove(row);
		// Preserve the committed destination independently of deferred cell saves and runtime callbacks.
		journal.Diagnostic = new XElement("CorpseRestore", new XAttribute("cell", cellId), new XAttribute("layer", (int)layer)).ToString(SaveOptions.DisableFormatting);
		journal.Version = checked(journal.Version + 1); journal.UpdatedUtc = Now(life);
		FMDB.Context.SaveChanges(); transaction.Commit();
	}

	private static string Presentation(SpellOwnedLifecycle life, string field)
	{
		var source = XElement.Parse(life.Origin.Provenance).Element("Source")?.Value ?? "";
		return source.StartsWith("<Presentation", StringComparison.Ordinal) ? XElement.Parse(source).Element(field)?.Value ?? "" : "";
	}

	public int ReconcileRetirements(DateTime nowUtc, int limit = 100)
	{
		if (nowUtc.Kind != DateTimeKind.Utc || limit is < 1 or > 1000) throw new ArgumentException("A bounded UTC reconciliation is required.");
		SpellOwnedLifecycle[] Read()
		{
			using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
			return FMDB.Context.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities)
				.Where(x => x.State != (int)SpellLifecycleState.Completed && x.Provenance.StartsWith(Prefix) &&
					x.Entities.Any(e => e.Kind == (int)SpellOwnedEntityKind.CharacterInstance && e.EntityId > _cursor))
				.OrderBy(x => x.Entities.Min(e => e.EntityId)).Take(limit).AsEnumerable().Select(SpellOwnedLifecycleStore.Read).ToArray();
		}
		var pending = Read();
		if (pending.Length == 0 && _cursor != 0) { _cursor = 0; pending = Read(); }
		foreach (var life in pending)
		{
			_cursor = life.Entities.Single().Id;
			var loaded = LoadedInstances().FirstOrDefault(x => x.InstanceId == _cursor);
			if (life.State == SpellLifecycleState.Active && life.Origin.DeadlineUtc > nowUtc && loaded?.IsEmbodied == true &&
				!life.Diagnostic.StartsWith(RetirementHeld, StringComparison.Ordinal)) continue;
			TryRetire(_cursor, life.Origin.DeadlineUtc <= nowUtc ? SpellRetirementReason.Expiry : SpellRetirementReason.Logout, out _);
		}
		return pending.Length;
	}

	private static DateTime Now(SpellOwnedLifecycle life) => RuntimeClock.UtcNow < life.UpdatedUtc ? life.UpdatedUtc : RuntimeClock.UtcNow;
	private void Hold(SpellOwnedLifecycle life, string diagnostic)
	{
		if (life.Diagnostic.StartsWith(RestoredPrefix, StringComparison.Ordinal)) diagnostic = life.Diagnostic.Split('\n')[0] + "\n" + diagnostic;
		if (life.State != SpellLifecycleState.Completed)
			_store.Hold(life.Origin.Id, life.Version, diagnostic[..Math.Min(2048, diagnostic.Length)], Now(life));
	}
}
