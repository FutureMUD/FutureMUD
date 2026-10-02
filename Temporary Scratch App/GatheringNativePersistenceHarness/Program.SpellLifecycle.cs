#nullable enable

using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MySql.Data.MySqlClient;
using Db = MudSharp.Models;
using RuntimeBody = MudSharp.Body.Implementations.Body;
using RuntimeCharacter = MudSharp.Character.Character;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record LifecycleReader(string Database, FixtureIds Fixture, Guid Pending,
		Guid Permanent, Guid MissingDeath, Guid[] Completed, long ForeignItem, int Bodies);

	private static int RunLifecycleAcceptanceChecks()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString);
		Console.WriteLine($"ARM03-created={database.Name}");
		using (var schema = NewIndependentContext(database.ConnectionString))
		{
			schema.Database.Migrate();
			Require(!schema.Database.GetPendingMigrations().Any() && !schema.Database.HasPendingModelChanges(), "Lifecycle migration/model parity failed.");
			Require(!schema.MagicSpellLifecycles.Any() && !schema.MagicSpellOwnedEntities.Any(), "Lifecycle tables were not initially empty.");
			Console.WriteLine("ARM03-schema=passed generated-EF9-migration-from-maintained-snapshot two-empty-tables model-parity");
		}
		var fixture = FixtureSeed.Create(database, "arm03_lifecycle", true);
		var native = NativeRuntime.Load(fixture, database.ConnectionString, true);
		ConfigureCastingWorld(native, database.ConnectionString, true);
		PrepareLifecycleRuntime(native);
		var spell = new MagicSpell("ARM03 lifecycle provenance", native.Capability.School);
		var now = new DateTime(2026, 10, 2, 21, 0, 0, DateTimeKind.Utc);
		var store = new SpellOwnedLifecycleStore();
		SpellLifecycleOrigin Origin(SpellLifecycleMode mode = SpellLifecycleMode.TemporaryCleanup) =>
			new(Guid.NewGuid(), spell.Id, 3, fixture.CharacterId, "guardian-fixture", mode, now,
				mode == SpellLifecycleMode.Permanent ? null : now.AddMinutes(1), "ARM03 controlled dependency checkpoint");
		int BodyCount() { using var read = NewIndependentContext(database.ConnectionString); return read.Bodies.Count(); }
		int LifecycleCount() { using var read = NewIndependentContext(database.ConnectionString); return read.MagicSpellLifecycles.Count(); }
		var initialBodies = BodyCount();
		var borrowed = Origin();
		Refuse(() => store.Create(borrowed, creation => creation.Claim(SpellOwnedEntityKind.Body,
			creation.Context.Bodies.Find(fixture.BodyId)!)), "Borrowed body was claimed.");
		Require(store.Find(borrowed.Id) is null && BodyCount() == initialBodies, "Borrowed ownership refusal changed rows.");
		Console.WriteLine("ARM03-borrowed=passed existing-body-refused no-ownership-or-entity-change");

		using (FMDB.BeginIsolatedScope())
		using (new FMDB())
		{
			var caller = FMDB.Context;
			var failed = Origin();
			Refuse(() => store.Create(failed, creation =>
			{
				creation.Claim(SpellOwnedEntityKind.Body, AddLifecycleBody(creation.Context, fixture.BodyId));
				throw new InvalidOperationException("controlled pre-save creation failure");
			}), "Creation callback failure escaped.");
			Require(ReferenceEquals(caller, FMDB.Context), "Creation did not restore its isolated caller session.");
			caller.SaveChanges();
			Require(store.Find(failed.Id) is null && BodyCount() == initialBodies, "Failed creation leaked tracked rows into the caller's later save.");
		}
		using (var connection = database.OpenOwnedConnection())
		{
			using var trigger = connection.CreateCommand();
			trigger.CommandText = "CREATE TRIGGER arm03_reject_receipt BEFORE INSERT ON MagicSpellLifecycles FOR EACH ROW SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='ARM03 controlled post-entity-save failure'";
			trigger.ExecuteNonQuery();
			var failed = Origin();
			try
			{
				store.Create(failed, creation => creation.Claim(SpellOwnedEntityKind.Body, AddLifecycleBody(creation.Context, fixture.BodyId)));
				throw new InvalidOperationException("The controlled receipt provider rejection did not fire.");
			}
			catch (DbUpdateException) { }
			trigger.CommandText = "DROP TRIGGER arm03_reject_receipt"; trigger.ExecuteNonQuery();
			Require(store.Find(failed.Id) is null && BodyCount() == initialBodies, "Provider failure retained unowned created rows.");
		}
		Console.WriteLine("ARM03-rollback=passed callback-and-post-save-provider-fault atomic-entity-plus-ownership caller-context-restored");

		var primary = Origin();
		Refuse(() => store.Create(primary, creation =>
		{
			var row = NewLifecycleInstance(fixture, fixture.BodyId); row.IsPrimary = true;
			creation.Context.CharacterInstances.Add(row); creation.Claim(SpellOwnedEntityKind.CharacterInstance, row);
		}), "Primary identity instance was claimed.");
		Require(store.Find(primary.Id) is null && LifecycleCount() == 0, "Rejected primary ownership was persisted.");
		Console.WriteLine("ARM03-primary=passed new-primary-instance-refused canonical-character-and-wound-preserved");
		var equipment = store.Create(Origin(), creation =>
		{
			var item = NewLifecycleItem(); creation.Context.GameItems.Add(item);
			creation.Claim(SpellOwnedEntityKind.GameItem, item, SpellOwnedEntityRole.GeneratedPossession);
		});
		Require(equipment.Entities.Single().Role == SpellOwnedEntityRole.GeneratedPossession, "Generated item lost its explicit ownership role.");
		equipment = store.BeginRetirement(equipment.Origin.Id, equipment.Version, SpellRetirementReason.Dispel, now);
		Refuse(() => store.Complete(equipment.Origin.Id, equipment.Version, now), "Live generated item completed.");
		using (var db = NewIndependentContext(database.ConnectionString)) { db.GameItems.Remove(db.GameItems.Find(equipment.Entities.Single().Id)!); db.SaveChanges(); }
		equipment = store.Complete(equipment.Origin.Id, equipment.Version, now);
		Console.WriteLine("ARM03-generated=passed exact-added-item-claim explicit-generated-possession-role live-row-blocks-completion");
		var stalePointer = store.Create(Origin(), creation => creation.Claim(SpellOwnedEntityKind.Body, AddLifecycleBody(creation.Context, fixture.BodyId)));
		stalePointer = store.BeginRetirement(stalePointer.Origin.Id, stalePointer.Version, SpellRetirementReason.Dismissal, now);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			db.Characters.Find(fixture.CharacterId)!.BodyId = stalePointer.Entities.Single().Id; db.SaveChanges();
		}
		Require(native.Actor.TryCleanupRetiredBody(LoadLifecycleBody(database, native, stalePointer.Entities.Single().Id)), "Stale canonical body pointer prevented safe retirement.");
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(db.Characters.Single(x => x.Id == fixture.CharacterId).BodyId == fixture.BodyId &&
				!db.Bodies.Any(x => x.Id == stalePointer.Entities.Single().Id), "Body cascade deleted or redirected the canonical identity.");
		stalePointer = store.Complete(stalePointer.Origin.Id, stalePointer.Version, now);
		Console.WriteLine("ARM03-canonical-pointer=passed stale-persisted-pointer-moved-to-current-body-before-retirement canonical-identity-survives-cascade-boundary");

		var permanent = store.Create(Origin(SpellLifecycleMode.Permanent), creation =>
			creation.Claim(SpellOwnedEntityKind.Body, AddLifecycleBody(creation.Context, fixture.BodyId)));
		Refuse(() => store.BeginRetirement(permanent.Origin.Id, permanent.Version, SpellRetirementReason.Expiry, now.AddDays(1)), "Permanent creation expired.");
		permanent = store.Complete(permanent.Origin.Id, permanent.Version, now);
		Require(!store.Pending(now.AddDays(1)).Any(x => x.Origin.Id == permanent.Origin.Id), "Permanent creation acquired a pending expiry.");
		Console.WriteLine("ARM03-permanent=passed no-expiry released-as-ordinary-durable-creation body-survives");

		var temporary = store.Create(Origin(), creation => creation.Claim(SpellOwnedEntityKind.Body, AddLifecycleBody(creation.Context, fixture.BodyId)));
		var tempBodyId = temporary.Entities.Single().Id;
		Refuse(() => store.BeginRetirement(temporary.Origin.Id, temporary.Version, SpellRetirementReason.Expiry, now), "Early deadline expired.");
		var creations = LifecycleCount(); var callbackRan = false;
		Refuse(() => store.Create(temporary.Origin, _ => callbackRan = true), "Duplicate operation created again.");
		Require(!callbackRan && LifecycleCount() == creations, "Duplicate key reached the creation callback.");
		temporary = store.BeginRetirement(temporary.Origin.Id, temporary.Version, SpellRetirementReason.Expiry, now.AddMinutes(1));
		Refuse(() => store.Hold(temporary.Origin.Id, 1, "stale writer", now.AddMinutes(1)), "Stale lifecycle version overwrote retirement.");
		Refuse(() => store.Complete(temporary.Origin.Id, temporary.Version, now.AddMinutes(1)), "Live owned body completed.");
		Console.WriteLine("ARM03-intent=passed immutable-creation-key absolute-deadline durable-retiring stale-version-and-live-completion-refused");

		var body = LoadLifecycleBody(database, native, tempBodyId);
		using (FMDB.BeginIsolatedScope(suppressEfWrites: true))
		using (new FMDB())
		{
			var caller = FMDB.Context; var suppressed = Origin(); var ran = false;
			Refuse(() => store.Create(suppressed, _ => ran = true), "Suppressed caller enabled creation writes.");
			Refuse(() => store.Hold(temporary.Origin.Id, temporary.Version, "suppressed update", now.AddMinutes(1)), "Suppressed caller enabled retirement writes.");
			Require(!ran && !native.Actor.TryCleanupRetiredBody(body), "Suppressed caller reached creation or body deletion.");
			Require(store.Find(temporary.Origin.Id)!.Version == temporary.Version && ReferenceEquals(caller, FMDB.Context) && FMDB.WritesAreSuppressed,
				"Independent lookup changed or escaped its caller's suppression scope.");
			caller.SaveChanges();
			Require(store.Find(suppressed.Id) is null && BodyCount() == initialBodies + 2, "Suppressed scope persisted creation or cleanup.");
		}
		Console.WriteLine("ARM03-suppression=passed sandbox-create-update-and-body-retirement-refused readonly-nested-lookup-restores-suppressed-caller no-persistent-state-change");
		long foreignItem;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var item = NewLifecycleItem(); db.GameItems.Add(item); db.SaveChanges(); foreignItem = item.Id;
			db.BodiesGameItems.Add(new() { BodyId = body.Id, GameItemId = foreignItem }); db.SaveChanges();
		}
		Require(!native.Actor.TryCleanupRetiredBody(body), "An unloaded foreign possession did not protect its body.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(db.Bodies.Any(x => x.Id == body.Id) && db.BodiesGameItems.Any(x => x.GameItemId == foreignItem) && db.GameItems.Any(x => x.Id == foreignItem), "Rejected inventory cleanup deleted or orphaned possessions.");
			db.BodiesGameItems.RemoveRange(db.BodiesGameItems.Where(x => x.BodyId == body.Id));
			db.CellsGameItems.Add(new() { CellId = fixture.CellId, GameItemId = foreignItem }); db.SaveChanges();
		}
		Console.WriteLine("ARM03-possession=passed native-character-and-body unloaded-foreign-join-refuses-cleanup intact-item explicitly-rehomed-to-existing-cell");

		long blocker;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var instance = NewLifecycleInstance(fixture, fixture.BodyId);
			instance.EffectData = $"<Effects><Effect><BackupBodyId>{body.Id}</BackupBodyId></Effect></Effects>";
			db.CharacterInstances.Add(instance); db.SaveChanges(); blocker = instance.Id;
		}
		Require(!native.Actor.TryCleanupRetiredBody(body), "An unloaded secondary effect reference did not protect its body.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			db.CharacterInstances.Remove(db.CharacterInstances.Find(blocker)!); db.SaveChanges();
			var component = new Db.GameItemComponent { GameItemId = foreignItem, Definition = $"<Definition><OriginalBody>{body.Id}</OriginalBody></Definition>" };
			db.GameItemComponents.Add(component); db.SaveChanges(); blocker = component.Id;
		}
		Require(!native.Actor.TryCleanupRetiredBody(body), "A persisted unloaded remains body reference did not protect its body.");
		temporary = store.Hold(temporary.Origin.Id, temporary.Version, "Waiting for persisted remains reference release", now.AddMinutes(1));
		Console.WriteLine("ARM03-references=passed unloaded-secondary-effect-and-serialized-remains retain-body-with-durable-hold");

		var death = CreateLifecycleActor(store, Origin(SpellLifecycleMode.DeathOnExpiry), fixture);
		Refuse(() => store.ObserveDeath(death.Origin.Id, death.Version, null, now), "Living actor was accepted as dead.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var instance = db.CharacterInstances.Find(death.Entities.Single(x => x.Kind == SpellOwnedEntityKind.CharacterInstance).Id)!;
			instance.State = (int)CharacterState.Dead; db.SaveChanges();
		}
		long remainsId;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var remains = NewLifecycleItem(); db.GameItems.Add(remains); db.SaveChanges(); remainsId = remains.Id;
			db.GameItemComponents.Add(new() { GameItemId = remains.Id, Definition = $"<Definition><OriginalBody>{fixture.BodyId}</OriginalBody></Definition>" }); db.SaveChanges();
		}
		Refuse(() => store.ObserveDeath(death.Origin.Id, death.Version, remainsId, now), "Foreign body remains were accepted.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			db.GameItemComponents.Single(x => x.GameItemId == remainsId).Definition = $"<Definition><OriginalBody>{death.Entities.Single(x => x.Kind == SpellOwnedEntityKind.Body).Id}</OriginalBody></Definition>"; db.SaveChanges();
		}
		death = store.ObserveDeath(death.Origin.Id, death.Version, remainsId, now);
		var repeated = store.ObserveDeath(death.Origin.Id, death.Version, remainsId, now.AddMinutes(2));
		Require(repeated.Version == death.Version && repeated.State == SpellLifecycleState.RemainsPending && !repeated.RequiresNativeDeath,
			"Repeated early death reopened or advanced the lifetime.");
		death = store.BeginRetirement(death.Origin.Id, death.Version, SpellRetirementReason.Expiry, now.AddMinutes(2));
		Require(death.State == SpellLifecycleState.RemainsPending && !death.RequiresNativeDeath, "Expiry after persisted early death requested a second death.");
		RemoveLifecycleInstance(database, death);
		Require(!native.Actor.TryCleanupRetiredBody(LoadLifecycleBody(database, native, death.Entities.Single(x => x.Kind == SpellOwnedEntityKind.Body).Id)), "Dependent remains did not retain the dead body.");
		Refuse(() => store.Complete(death.Origin.Id, death.Version, now.AddMinutes(2)), "Dependent remains completed before release.");
		using (var db = NewIndependentContext(database.ConnectionString)) { db.GameItems.Remove(db.GameItems.Find(remainsId)!); db.SaveChanges(); }
		Require(native.Actor.TryCleanupRetiredBody(LoadLifecycleBody(database, native, death.Entities.Single(x => x.Kind == SpellOwnedEntityKind.Body).Id)), "Reference-free death-correlated body did not retire.");
		death = store.Complete(death.Origin.Id, death.Version, now.AddMinutes(2));
		Console.WriteLine("ARM03-death-journal=passed persisted-secondary-dead-state-correlated early-death-cancels-later-death-intent idempotent-observation canonical-identity-survives");

		var missing = CreateLifecycleActor(store, Origin(SpellLifecycleMode.DeathOnExpiry), fixture);
		missing = store.BeginRetirement(missing.Origin.Id, missing.Version, SpellRetirementReason.Expiry, now.AddMinutes(1));
		RemoveLifecycleInstance(database, missing);
		Require(native.Actor.TryCleanupRetiredBody(LoadLifecycleBody(database, native, missing.Entities.Single(x => x.Kind == SpellOwnedEntityKind.Body).Id)), "Owned missing-actor body did not retire.");
		Refuse(() => store.Complete(missing.Origin.Id, missing.Version, now.AddMinutes(1)), "Absent actor bypassed required native death proof.");
		missing = store.Hold(missing.Origin.Id, missing.Version, "Actor missing before native death correlation; adapter investigation required", now.AddMinutes(1));
		Console.WriteLine("ARM03-missing-death=passed absent-owned-rows-cannot-complete-unobserved-native-death recoverable-diagnostic");

		var completed = new List<Guid> { death.Origin.Id, equipment.Origin.Id, stalePointer.Origin.Id };
		for (var i = 0; i < 16; i++)
		{
			var row = store.Create(Origin(), creation => creation.Claim(SpellOwnedEntityKind.Body, AddLifecycleBody(creation.Context, fixture.BodyId)));
			row = store.BeginRetirement(row.Origin.Id, row.Version, SpellRetirementReason.Dismissal, now);
			Require(native.Actor.TryCleanupRetiredBody(LoadLifecycleBody(database, native, row.Entities.Single().Id)), "Repeated eligible owned body did not retire.");
			row = store.Complete(row.Origin.Id, row.Version, now); completed.Add(row.Origin.Id);
			Require(store.Complete(row.Origin.Id, row.Version, now).Version == row.Version, "Repeated completion changed state.");
		}
		Require(BodyCount() == initialBodies + 2, "Eligible owned body rows accumulated; expected only canonical, permanent and dependency-held bodies.");
		Console.WriteLine("ARM03-steady-state=passed 16-native-body-create-retire-complete-cycles only-permanent-and-reference-held-physical-rows-retained lightweight-ownership-receipts");
		RunLifecycleReaderProcess(new(database.Name, fixture, temporary.Origin.Id, permanent.Origin.Id,
			missing.Origin.Id, completed.ToArray(), foreignItem, initialBodies + 2));
		using (var read = NewIndependentContext(database.ConnectionString))
		{
			Require(read.Characters.Single(x => x.Id == fixture.CharacterId).BodyId == fixture.BodyId &&
				read.Wounds.Single(x => x.Id == fixture.ExistingWoundId).ActorOriginId == fixture.CharacterId,
				"Canonical identity or native wound attribution changed.");
			Console.WriteLine($"ARM03-row-counts=passed characters:{read.Characters.Count()} bodies:{read.Bodies.Count()} instances:{read.CharacterInstances.Count()} items:{read.GameItems.Count()} wounds:{read.Wounds.Count()} lifecycles:{read.MagicSpellLifecycles.Count()} claims:{read.MagicSpellOwnedEntities.Count()} pending:{store.Pending(now.AddDays(1)).Count}");
			Require(read.Bodies.Count() == initialBodies + 1 && read.GameItems.Any(x => x.Id == foreignItem) &&
				read.CellsGameItems.Any(x => x.GameItemId == foreignItem && x.CellId == fixture.CellId), "Restart cleanup lost foreign possessions or accumulated eligible bodies.");
		}
		Console.WriteLine("ARM03-limits=recorded foundation-protocol-and-retired-body-boundary only; native-NPC-death-corpse-decay-control-projection-adapters-and-world-timer-steady-state-remain-unqualified");
		return 0;
	}

	private static void PrepareLifecycleRuntime(NativeRuntime native)
	{
		SetPrivateField(native.Actor, "_forms", new List<ICharacterForm>());
		var sources = typeof(RuntimeCharacter).GetField("_formSources", BindingFlags.Instance | BindingFlags.NonPublic)!;
		sources.SetValue(native.Actor, Activator.CreateInstance(sources.FieldType));
		var characters = new All<ICharacter>(); characters.Add(native.Actor); native.WorldMock.SetupGet(x => x.Characters).Returns(characters);
		native.WorldMock.SetupGet(x => x.Items).Returns(new All<IGameItem>());
		native.WorldMock.SetupGet(x => x.Scheduler).Returns(new Mock<IScheduler>().Object);
	}

	private static Db.Body AddLifecycleBody(FuturemudDatabaseContext context, long sourceBody)
	{
		var source = context.Bodies.AsNoTracking().Single(x => x.Id == sourceBody);
		var row = (Db.Body)context.Entry(source).CurrentValues.ToObject(); row.Id = 0;
		context.Bodies.Add(row); return row;
	}
	private static RuntimeBody LoadLifecycleBody(TestDatabase database, NativeRuntime native, long bodyId)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		return new RuntimeBody(db.Bodies.AsNoTracking().Single(x => x.Id == bodyId), native.World, native.Actor);
	}
	private static Db.GameItem NewLifecycleItem() => new()
	{
		Quality = 1, Condition = 1, Size = 1, EffectData = "<Effects/>", PositionTargetType = "", PositionEmote = "", OwnerType = ""
	};
	private static Db.CharacterInstance NewLifecycleInstance(FixtureIds fixture, long bodyId) => new()
	{
		CharacterId = fixture.CharacterId, BodyId = bodyId, InstanceName = "ARM03 isolated secondary", InstanceKind = (int)CharacterInstanceKind.Other,
		PersistencePolicy = (int)CharacterInstancePersistencePolicy.TemporaryEffectBound, LocationId = fixture.CellId,
		State = (int)CharacterState.Awake, PositionId = 1, PositionTargetType = "", PositionEmote = "",
		CreatedDateTime = DateTime.UtcNow, CreatedBySourceKey = "ARM03 fixture", EffectData = "<Effects/>"
	};
	private static SpellOwnedLifecycle CreateLifecycleActor(SpellOwnedLifecycleStore store, SpellLifecycleOrigin origin, FixtureIds fixture) =>
		store.Create(origin, creation =>
		{
			var body = AddLifecycleBody(creation.Context, fixture.BodyId); creation.Claim(SpellOwnedEntityKind.Body, body);
			var instance = NewLifecycleInstance(fixture, 0); instance.Body = body;
			creation.Context.CharacterInstances.Add(instance); creation.Claim(SpellOwnedEntityKind.CharacterInstance, instance);
		});
	private static void RemoveLifecycleInstance(TestDatabase database, SpellOwnedLifecycle row)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		db.CharacterInstances.Remove(db.CharacterInstances.Find(row.Entities.Single(x => x.Kind == SpellOwnedEntityKind.CharacterInstance).Id)!); db.SaveChanges();
	}
	private static void Refuse(Action action, string failure)
	{
		try { action(); }
		catch (InvalidOperationException) { return; }
		throw new InvalidOperationException(failure);
	}
	private static void RunLifecycleReaderProcess(LifecycleReader input)
	{
		var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
		start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); start.ArgumentList.Add("--lifecycle-reader");
		start.ArgumentList.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(input))));
		using var process = Process.Start(start)!; var output = process.StandardOutput.ReadToEndAsync(); var error = process.StandardError.ReadToEndAsync();
		if (!process.WaitForExit(60000)) { process.Kill(true); throw new TimeoutException("Lifecycle reader exceeded 60 seconds."); }
		Console.Write(output.GetAwaiter().GetResult()); Require(process.ExitCode == 0, error.GetAwaiter().GetResult());
	}
	private static int RunLifecycleReader(string[] args)
	{
		var input = JsonSerializer.Deserialize<LifecycleReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args.Single())))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var native = NativeRuntime.Load(input.Fixture, database.ConnectionString, true); ConfigureCastingWorld(native, database.ConnectionString, false); PrepareLifecycleRuntime(native);
		var store = new SpellOwnedLifecycleStore(); var row = store.Find(input.Pending)!;
		Require(row.State == SpellLifecycleState.Retiring && row.Diagnostic.Contains("remains"), "Restart lost retirement intent or hold reason.");
		Require(store.Find(input.Permanent) is { State: SpellLifecycleState.Completed, MayRemoveOwnedEntities: false }, "Restart lost permanent creation policy.");
		Require(store.Find(input.MissingDeath) is { RequiresNativeDeath: true } missing && missing.Diagnostic.Contains("missing"), "Restart lost missing native death quarantine.");
		Require(input.Completed.All(x => store.Find(x)!.State == SpellLifecycleState.Completed), "Restart reopened completed ownership records.");
		Require(store.Pending(row.Origin.CreatedUtc.AddDays(1)).Select(x => x.Origin.Id).ToHashSet().SetEquals([input.Pending, input.MissingDeath]), "Restart pending query lost or replayed lifecycles.");
		var body = LoadLifecycleBody(database, native, row.Entities.Single().Id);
		Require(!native.Actor.TryCleanupRetiredBody(body), "Restart bypassed an unloaded remains dependency.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(db.Bodies.Count() == input.Bodies && db.GameItems.Any(x => x.Id == input.ForeignItem), "Reader startup changed heavy rows or foreign goods.");
			var component = db.GameItemComponents.Single(x => x.GameItemId == input.ForeignItem);
			component.Definition = $"<Definition><Bodypart>{body.Id}</Bodypart><OverridenBodypart>{body.Id}</OverridenBodypart></Definition>";
			db.SaveChanges();
		}
		Require(native.Actor.TryCleanupRetiredBody(body), "An anatomy ID collision prevented eligible retirement.");
		row = store.Complete(row.Origin.Id, row.Version, row.Origin.CreatedUtc.AddDays(1));
		Require(store.Complete(row.Origin.Id, row.Version, row.UpdatedUtc).Version == row.Version && !store.Pending(row.UpdatedUtc).Any(x => x.Origin.Id == row.Origin.Id), "Completed restart retirement remained pending.");
		Console.WriteLine("ARM03-reader=passed separate-process reconstructs-intent-ownership-permanent-policy-and-missing-death-hold no-recreation preserves-reference-until-release anatomy-collision-safe idempotent-completion");
		return 0;
	}
}
