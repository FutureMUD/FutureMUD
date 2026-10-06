#nullable enable

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Combat;
using MudSharp.Community.Boards;
using MudSharp.Commands.Trees;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MudSharp.NPC;
using MudSharp.NPC.AI.Groups;
using MudSharp.NPC.Templates;
using MudSharp.RPG.Law;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Listeners;
using Db = MudSharp.Models;
using RuntimeNPC = MudSharp.NPC.NPC;
using RuntimeCharacter = MudSharp.Character.Character;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record ArchiveReader(string Database, long Character, long Body, Guid Lifecycle,
		long History, long ForeignWound, long ForeignItem, long Crime, int Archives, long Writing, long Drawing);
	private sealed record ArchiveControllerReceipt(NPCController Controller, List<IMonitorable> Observees);

	private static int RunNpcArchiveAcceptanceChecks()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString);
		Console.WriteLine($"ARM03B-created={database.Name}");
		var fixture = FixtureSeed.Create(database, "arm03b_archive", true);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			db.GetService<IMigrator>().Migrate("20261002225323_OrdinaryBodyRetirement");
			db.Database.Migrate();
			Require(!db.Database.HasPendingModelChanges() && !db.Database.GetPendingMigrations().Any(), "Archive migration/model parity failed.");
			Require(!db.Characters.Find(fixture.CharacterId)!.IsArchived && !db.CharacterArchives.Any(), "Upgrade archived an existing canonical.");
			var refused = false;
			try { db.Database.ExecuteSqlRaw("DELETE FROM Bodies WHERE Id={0}", fixture.BodyId); }
			catch (MySqlConnector.MySqlException ex) when (ex.Message.Contains("FK_Characters_Bodies")) { refused = true; }
			Require(refused && db.Bodies.Any(x => x.Id == fixture.BodyId) && db.Characters.Any(x => x.Id == fixture.CharacterId && x.BodyId == fixture.BodyId),
				"Body deletion cascaded into the canonical identity.");
		}
		Console.WriteLine("ARM03B-schema=passed maintained-snapshot-import empty-archive-down-to-prior-migration generated-upgrade model-parity existing-canonical-preserved restrictive-body-FK");
		// Distinct fixture identity ranges avoid conservative numeric-token collisions with prototype IDs.
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			foreach (var sql in new[] { "ALTER TABLE Characters AUTO_INCREMENT=1000000", "ALTER TABLE Bodies AUTO_INCREMENT=2000000",
				"ALTER TABLE CharacterInstances AUTO_INCREMENT=3000000", "ALTER TABLE Wounds AUTO_INCREMENT=4000000" }) db.Database.ExecuteSqlRaw(sql);
		}
		var native = NativeRuntime.Load(fixture, database.ConnectionString, true);
		ConfigureCastingWorld(native, database.ConnectionString, true); PrepareLifecycleRuntime(native);
		var roots = ArchiveRoots();
		ConfigureArchiveWorld(native, roots, fixture, database.ConnectionString);
		native.WorldMock.Setup(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()))
			.Returns<long, bool>((id, _) => id == fixture.CharacterId ? native.Actor : roots.TryGetCharacter(id, true));
		var spell = new MagicSpell("ARM03B archive provenance", native.Capability.School);
		var now = new DateTime(2026, 10, 3, 4, 0, 0, DateTimeKind.Utc);
		var store = new SpellOwnedLifecycleStore(); var archives = new CharacterArchiveService();
		native.WorldMock.SetupGet(x => x.CharacterArchives).Returns(archives);
		var templateId = CreateArchiveTemplate(database);
		var template = new Mock<INPCTemplate>(); template.SetupGet(x => x.Id).Returns(templateId);
		var templates = new Mock<IUneditableRevisableAll<INPCTemplate>>();
		templates.Setup(x => x.Get(templateId, 0)).Returns(template.Object);
		native.WorldMock.SetupGet(x => x.NpcTemplates).Returns(templates.Object);
		SpellLifecycleOrigin Origin(SpellLifecycleMode mode = SpellLifecycleMode.TemporaryCleanup) =>
			new(Guid.NewGuid(), spell.Id, 3, fixture.CharacterId, "archive-fixture", mode, now,
				mode == SpellLifecycleMode.Permanent ? null : now.AddMinutes(1), "ARM03B native archive boundary; template callback adapter deferred");
		var lifetime = CreateArchiveNpc(store, Origin(SpellLifecycleMode.DeathOnExpiry), fixture, templateId);
		long ownedWoundId;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var sourceWound = db.Wounds.AsNoTracking().Single(x => x.Id == fixture.ExistingWoundId);
			var wound = (Db.Wound)db.Entry(sourceWound).CurrentValues.ToObject(); wound.Id = 0;
			wound.BodyId = lifetime.Entities.Single(x => x.Kind == SpellOwnedEntityKind.Body).Id;
			wound.ActorOriginId = fixture.CharacterId; db.Wounds.Add(wound); db.SaveChanges(); ownedWoundId = wound.Id;
		}
		var npc = LoadArchiveNpc(native, roots, lifetime);
		var retainedControllers = new List<ArchiveControllerReceipt> { RetainArchiveController(npc) };
		var characterId = npc.Id; var bodyId = npc.Body.Id;
		lifetime = store.BeginRetirement(lifetime.Origin.Id, lifetime.Version, SpellRetirementReason.Expiry, now.AddMinutes(1));
		var preDeath = false; var nativeDeaths = 0;
		npc.OnDeath += actor =>
		{
			nativeDeaths++;
			using var read = NewIndependentContext(database.ConnectionString);
			Require(!((CharacterState)read.Characters.Find(characterId)!.State).HasFlag(CharacterState.Dead), "Pre-death event ran after persisted death.");
			Require(!archives.TryArchiveNpc(lifetime.Origin.Id, store.Find(lifetime.Origin.Id)!.Version, npc, now.AddMinutes(1), out _), "Pre-death event compacted a live graph.");
			preDeath = true;
		};
		Require(npc.Die() is null && preDeath && nativeDeaths == 1, "The actual no-remains NPC death path did not run once.");
		RequireDetachedController(retainedControllers[0]);
		lifetime = store.ObserveDeath(lifetime.Origin.Id, store.Find(lifetime.Origin.Id)!.Version, null, now.AddMinutes(1));
		Require(!lifetime.RequiresNativeDeath && roots.CachedActors.Has(characterId), "Native death was not persisted/cached before correlation.");
		Console.WriteLine("ARM03B-native-death=passed loaded-production-NPC-Die pre-death-callback-refusal persisted-death-time primary-instance-death ownership-correlation no-remains-model");
		long historyId, foreignItem;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var history = new Db.CharacterLog { CharacterId = characterId, CellId = fixture.CellId, Command = "archive attribution sentinel", Time = now };
			db.Set<Db.CharacterLog>().Add(history);
			db.Wounds.Find(fixture.ExistingWoundId)!.ActorOriginId = characterId;
			var item = NewLifecycleItem(); db.GameItems.Add(item); db.SaveChanges();
			historyId = history.Id; foreignItem = item.Id;
		}
		var historicalCrime = CreateArchiveCrime(database, native, fixture, characterId);
		Require(ReferenceEquals(historicalCrime.Criminal, npc), "The finalized-crime fixture did not cache the native NPC before archival.");
		void MustHold(string contains)
		{
			lifetime = store.Find(lifetime.Origin.Id)!;
			Require(!archives.TryArchiveNpc(lifetime.Origin.Id, lifetime.Version, npc, now.AddMinutes(2), out var reason) &&
				reason.Contains(contains, StringComparison.OrdinalIgnoreCase), "Expected archive hold: " + contains + "; actual: " + reason);
			using var read = NewIndependentContext(database.ConnectionString);
			Require(!npc.IsArchived && read.Bodies.Any(x => x.Id == bodyId) && !read.Characters.Find(characterId)!.IsArchived &&
				!read.CharacterArchives.Any(x => x.CharacterId == characterId) && read.GameItems.Any(x => x.Id == foreignItem) &&
				store.Find(lifetime.Origin.Id)!.Diagnostic == reason, "Hold changed physical, foreign or historical state or lost its durable reason.");
		}
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var crime = db.Crimes.Find(historicalCrime.Id)!; crime.IsFinalised = false; crime.IsStaleCrime = true; db.SaveChanges();
		}
		MustHold("enforcement");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var crime = db.Crimes.Find(historicalCrime.Id)!;
			Require(crime.WitnessIds == fixture.CharacterId.ToString() && !crime.IsFinalised && crime.IsStaleCrime,
				"The stale crime with a surviving witness was mutated by the hold.");
			crime.IsFinalised = true; db.SaveChanges();
		}
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var item = db.GameItems.Find(foreignItem)!; item.OwnerId = bodyId; item.OwnerType = "Body"; db.SaveChanges();
		}
		MustHold("polymorphic");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var item = db.GameItems.Find(foreignItem)!; item.OwnerId = null; item.OwnerType = "";
			db.GameItemComponents.Add(new() { GameItemId = foreignItem, Definition = $"<Definition><OriginalBody>{bodyId}</OriginalBody></Definition>" }); db.SaveChanges();
		}
		MustHold("serialized");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			db.GameItemComponents.Single(x => x.GameItemId == foreignItem).Definition = "<malformed"; db.SaveChanges();
		}
		MustHold("serialized");
		long drawingId;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			db.GameItemComponents.RemoveRange(db.GameItemComponents.Where(x => x.GameItemId == foreignItem));
			var drawing = new Db.Drawing { AuthorId = characterId, ShortDescription = "historical drawing", FullDescription = "A retained drawing." };
			db.Drawings.Add(drawing); db.SaveChanges(); drawingId = drawing.Id;
		}
		var writingId = CreateArchiveWriting(database, characterId);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			db.Writings.Find(writingId)!.TrueAuthorId = characterId; db.SaveChanges();
		}
		var groups = (All<IGroupAI>)native.World.GroupAIs; var group = new Mock<IGroupAI>();
		group.SetupGet(x => x.Id).Returns(77); group.SetupGet(x => x.GroupMembers).Returns(Array.Empty<ICharacter>());
		group.SetupGet(x => x.GroupRoles).Returns(new Dictionary<ICharacter, GroupRole> { [npc] = GroupRole.Adult }); groups.Add(group.Object);
		MustHold("runtime"); groups.Remove(group.Object);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			db.CharacterBodies.Add(new() { CharacterId = fixture.CharacterId, BodyId = bodyId, Alias = "foreign-unloaded", TransformationEcho = "" }); db.SaveChanges();
		}
		MustHold("foreign ownership");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(db.CharacterBodies.Any(x => x.CharacterId == fixture.CharacterId && x.BodyId == bodyId), "Foreign form was pruned by the hold.");
			db.CharacterBodies.RemoveRange(db.CharacterBodies.Where(x => x.CharacterId == fixture.CharacterId && x.BodyId == bodyId));
			db.CharacterBodySources.Add(new() { CharacterId = fixture.CharacterId, BodyId = bodyId, SourceKey = "foreign-unloaded" }); db.SaveChanges();
		}
		MustHold("foreign ownership");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(db.CharacterBodySources.Any(x => x.CharacterId == fixture.CharacterId && x.BodyId == bodyId), "Foreign source was pruned by the hold.");
			db.CharacterBodySources.RemoveRange(db.CharacterBodySources.Where(x => x.CharacterId == fixture.CharacterId && x.BodyId == bodyId));
			db.CharacterBodyRetirements.Add(new() { CharacterId = fixture.CharacterId, BodyId = bodyId, RetiredUtc = now }); db.SaveChanges();
		}
		MustHold("foreign ownership");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(db.CharacterBodyRetirements.Any(x => x.CharacterId == fixture.CharacterId && x.BodyId == bodyId), "Foreign retirement was pruned by the hold.");
			db.CharacterBodyRetirements.RemoveRange(db.CharacterBodyRetirements.Where(x => x.BodyId == bodyId)); db.SaveChanges();
		}
		Console.WriteLine("ARM03B-foreign-ownership=passed independently-unloaded-form-source-retirement-mismatch durable-hold foreign-rows-and-heavy-NPC-preserved");
		Console.WriteLine("ARM03B-holds=passed foreign-custody serialized-remains-reference malformed-serialization runtime-role-root durable-diagnostic no-graph-or-foreign-mutation historical-writing-and-drawing-retained-through-holds");
		using (var db = NewIndependentContext(database.ConnectionString))
			db.Database.ExecuteSqlRaw("CREATE TRIGGER arm03b_reject_archive BEFORE INSERT ON CharacterArchives FOR EACH ROW SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='ARM03B archive rollback fixture'");
		lifetime = store.Find(lifetime.Origin.Id)!;
		var rejected = false; var rollbackDiagnostic = "";
		try { archives.TryArchiveNpc(lifetime.Origin.Id, lifetime.Version, npc, now.AddMinutes(2), out rollbackDiagnostic); }
		catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("ARM03B archive rollback fixture") == true) { rejected = true; }
		finally { using var db = NewIndependentContext(database.ConnectionString); db.Database.ExecuteSqlRaw("DROP TRIGGER arm03b_reject_archive"); }
		using (var read = NewIndependentContext(database.ConnectionString))
			Require(rejected && !npc.IsArchived && !read.CharacterArchives.Any() && read.Bodies.Any(x => x.Id == bodyId) &&
				read.Npcs.Any(x => x.CharacterId == characterId) && read.CharacterInstances.Any(x => x.CharacterId == characterId),
				$"Provider fault did not roll back the whole graph: rejected={rejected}; diagnostic={rollbackDiagnostic}; archived={npc.IsArchived}; archiveRows={read.CharacterArchives.Count()}; body={read.Bodies.Any(x => x.Id == bodyId)}; npc={read.Npcs.Any(x => x.CharacterId == characterId)}; instance={read.CharacterInstances.Any(x => x.CharacterId == characterId)}.");
		Console.WriteLine("ARM03B-rollback=passed provider-insert-fault identity-body-NPC-instance-history-atomic runtime-release-after-commit-only");
		using (var callerScope = FMDB.BeginIndependentScope(requireWrites: true))
		using (var callerDb = new FMDB())
		{
			var caller = FMDB.Context; var stale = caller.Characters.Find(characterId)!;
			stale.Name = "stale writer must not restore the graph";
			native.WorldMock.Setup(x => x.ForgetArchivedCharacter(It.IsAny<ICharacter>()))
				.Callback<ICharacter>(_ => throw new InvalidOperationException("ARM03B post-commit runtime-release fault"));
			var released = false;
			try { archives.TryArchiveNpc(lifetime.Origin.Id, lifetime.Version, npc, now.AddMinutes(2), out _); }
			catch (InvalidOperationException ex) when (ex.Message == "ARM03B post-commit runtime-release fault") { released = true; }
			Require(released && archives.Find(characterId) is not null && npc.IsArchived && roots.CachedActors.Has(characterId),
				"Post-commit runtime fault lost durable archive or did not retain retryable cache state.");
			native.WorldMock.Setup(x => x.ForgetArchivedCharacter(It.IsAny<ICharacter>())).Callback<ICharacter>(roots.ForgetArchivedCharacter);
			Require(archives.TryArchiveNpc(lifetime.Origin.Id, lifetime.Version, npc, now.AddMinutes(2), out var reason), "Compaction retry failed: " + reason);
			Require(ReferenceEquals(caller, FMDB.Context), "Compaction failed to restore caller scope.");
			var refused = false;
			try { caller.SaveChanges(); }
			catch (DbUpdateConcurrencyException) { refused = true; }
			Require(refused, "Stale tracked identity overwrote the tombstone.");
			caller.ChangeTracker.Clear(); npc.Save(); npc.Body.Save(); caller.SaveChanges();
		}
		Console.WriteLine("ARM03B-release-retry=passed post-commit-runtime-fault durable-archive-retained physical-graph-gone cached-actor-released-on-exact-retry");
		Require(npc.IsArchived && archives.Find(characterId) is { OriginalBodyId: var oldBody } && oldBody == bodyId &&
			!roots.CachedActors.Has(characterId) && !roots.Actors.Has(characterId) && !roots.Bodies.Has(bodyId) &&
			!native.World.SaveManager.IsQueued(npc) && !native.World.SaveManager.IsQueued(npc.Body), "Runtime graph or queued saves survived compaction.");
		Require(roots.TryGetCharacter(characterId, true) is null && roots.TryGetCharacter(characterId) is null, "Archive materialized through the production cache loader.");
		Require(archives.TryArchiveNpc(lifetime.Origin.Id, lifetime.Version, npc, now.AddMinutes(2), out _), "Already-archived retry was not idempotent.");
		lifetime = store.Complete(lifetime.Origin.Id, lifetime.Version, now.AddMinutes(2));
		Require(store.Complete(lifetime.Origin.Id, lifetime.Version, now.AddMinutes(3)).Version == lifetime.Version && nativeDeaths == 1,
			"Completion retry advanced ownership or reran death.");
		Console.WriteLine("ARM03B-compaction=passed exact-archive canonical-ID-retained body-NPC-instance-released caller-restored stale-save-refused queued-saves-aborted real-cache-loader-no-materialization idempotent-retry");
		using (var read = NewIndependentContext(database.ConnectionString))
		{
			Require(read.Writings.Find(writingId) is { AuthorId: var author, TrueAuthorId: var trueAuthor } && author == characterId && trueAuthor == characterId &&
				read.Drawings.Find(drawingId) is { AuthorId: var drawingAuthor, FullDescription: "A retained drawing." } && drawingAuthor == characterId,
				"Archival changed or removed historical writing/drawing attribution.");
			using var wounds = JsonDocument.Parse(read.CharacterArchives.Find(characterId)!.WoundHistory);
			Require(wounds.RootElement.GetArrayLength() == 1 && wounds.RootElement[0].GetProperty("Id").GetInt64() == ownedWoundId &&
				!read.Wounds.Any(x => x.Id == ownedWoundId), "Owned wound state was not boundedly archived before removing the physical wound.");
		}
		Require(historicalCrime.Criminal is null && historicalCrime.ShowCrimeInfo(native.Actor).Contains("Archive Guardian") &&
			historicalCrime.DescribeCrimeAtTrial(native.Actor).Contains("Archive Guardian"),
			"Finalized crime history retained/materialized the heavy NPC or lost archived criminal/victim names.");
		Console.WriteLine("ARM03B-crime-history=passed stale-unfinished-witness-crime-durable-hold finalized-crime-weak-cache archived-criminal-victim-display-and-trial-name canonical-attribution-preserved");
		var completed = new List<Guid> { lifetime.Origin.Id }; var weak = new List<WeakReference<RuntimeNPC>>();
		for (var i = 0; i < 4; i++) weak.Add(ArchiveRepeatedNpc(database, native, roots, store, archives, Origin(), fixture, templateId, now, completed, retainedControllers, i == 3));
		foreach (var receipt in retainedControllers) RequireDetachedController(receipt);
		// Mock call receipts retain argument objects, including calls on recursively supplied catalogue services.
		// Clear only fixture mocks; production roots, save queues, controllers and subscriptions remain subject to the GC assertion.
		var clearedMocks = new HashSet<Mock>(ReferenceEqualityComparer.Instance);
		ClearArchiveFixtureMock(native.WorldMock, clearedMocks);
		foreach (var value in native.World.Races.Cast<object>().Concat(native.World.BodyPrototypes).Concat(native.World.BodypartPrototypes)
		         .Concat(native.World.Cultures).Concat(native.World.Cells).Concat(native.World.TraitExpressions))
			if (value is IMocked mocked) ClearArchiveFixtureMock(mocked.Mock, clearedMocks);
		foreach (var name in new[] { "TotalBloodVolumeProg", "LiverFunctionProg", "MaximumStaminaProg" })
			ClearArchiveFixtureMock(Mock.Get((IFutureProg)typeof(RuntimeCharacter).GetField(name, BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!), clearedMocks);
		Console.WriteLine($"ARM03B-fixture-receipts=cleared recursively-reached-mocks:{clearedMocks.Count} production-roots-and-queues-preserved-for-GC-check");
		GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
		Require(weak.All(x => !x.TryGetTarget(out _)), "Completed repeated NPC graphs remain strongly rooted.");
		using (var read = NewIndependentContext(database.ConnectionString))
		{
			Require(read.CharacterArchives.Count() == 5 && read.Bodies.Count() == 1 && !read.Npcs.Any() &&
				!read.CharacterInstances.Any(x => x.CharacterId != fixture.CharacterId) &&
				read.Set<Db.CharacterLog>().Find(historyId)!.CharacterId == characterId && read.Wounds.Find(fixture.ExistingWoundId)!.ActorOriginId == characterId &&
				read.GameItems.Any(x => x.Id == foreignItem), "Repeated archival retained heavy rows or lost foreign/history attribution.");
		}
		Console.WriteLine("ARM03B-steady-state=passed four-repeated-native-deaths heavy-body-NPC-instance-rows-at-baseline bounded-archives weak-actor-graphs-collected foreign-wound-and-item-preserved");
		Console.WriteLine("ARM03B-controller-release=passed monitored-native-death-and-persisted-dead-reload retained-controller-context-and-output-detached weak-actor-graphs-collected");
		var input = new ArchiveReader(database.Name, characterId, bodyId, lifetime.Origin.Id, historyId, fixture.ExistingWoundId!.Value, foreignItem, historicalCrime.Id, 5, writingId, drawingId);
		var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
		start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); start.ArgumentList.Add("--npc-archive-reader");
		start.ArgumentList.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(input))));
		using var process = Process.Start(start)!; var output = process.StandardOutput.ReadToEndAsync(); var error = process.StandardError.ReadToEndAsync();
		if (!process.WaitForExit(60000)) { process.Kill(true); throw new TimeoutException("Archive restart reader exceeded60 seconds."); }
		Console.Write(output.GetAwaiter().GetResult()); Require(process.ExitCode == 0, error.GetAwaiter().GetResult());
		Console.WriteLine("ARM03B-qualifier=controlled-catalogues no-remains-race native-persisted-NPC-load-and-Die EF-owned-creation fixture-distinct-ID-ranges template-before-callback corpse-Delete decay foreign-evacuation installed-session N14-N15-N16-NOT-RUN");
		return 0;
	}

	private static Futuremud ArchiveRoots()
	{
		var world = (Futuremud)RuntimeHelpers.GetUninitializedObject(typeof(Futuremud));
		foreach (var name in new[] { "_actors", "_characters", "_NPCs", "_cachedActors" }) SetPrivateField(world, name, new All<ICharacter>());
		SetPrivateField(world, "_bodies", new All<IBody>()); SetPrivateField(world, "_listeners", new All<ITemporalListener>());
		return world;
	}

	private static void ConfigureArchiveWorld(NativeRuntime native, Futuremud roots, FixtureIds fixture, string connection)
	{
		var world = native.WorldMock;
		world.SetupGet(x => x.Actors).Returns(roots.Actors); world.SetupGet(x => x.Characters).Returns(roots.Characters);
		world.SetupGet(x => x.NPCs).Returns(roots.NPCs); world.SetupGet(x => x.CachedActors).Returns(roots.CachedActors);
		world.SetupGet(x => x.Bodies).Returns(roots.Bodies); world.SetupGet(x => x.GroupAIs).Returns(new All<IGroupAI>());
		world.SetupGet(x => x.Boards).Returns(new All<IBoard>());
		// A recursive loose mock otherwise creates per-actor argument-matching setups that retain the NPC forever.
		var commandTree = new Mock<ICharacterCommandTree>();
		world.Setup(x => x.RetrieveAppropriateCommandTree(It.IsAny<ICharacter>())).Returns(commandTree.Object);
		world.SetupGet(x => x.CachedBodyguards).Returns(new Dictionary<long, List<ICharacter>>());
		world.Setup(x => x.Destroy(It.IsAny<ICharacter>())).Callback<ICharacter>(roots.Destroy);
		world.Setup(x => x.Destroy(It.IsAny<IBody>())).Callback<IBody>(roots.Destroy);
		world.Setup(x => x.ForgetArchivedCharacter(It.IsAny<ICharacter>())).Callback<ICharacter>(roots.ForgetArchivedCharacter);
		world.Setup(x => x.GetStaticString(It.IsAny<string>())).Returns("Fixture {0}");
		world.Setup(x => x.GetStaticString("RegularDeathEmote")).Returns("$0 collapses dead.");
		world.Setup(x => x.GetStaticString("DeathMessage")).Returns("The fixture ends.");
		var authority = new Mock<IAuthority>(); authority.SetupGet(x => x.Id).Returns(1);
		authority.SetupGet(x => x.Name).Returns("Player"); authority.SetupGet(x => x.Level).Returns(PermissionLevel.Player);
		var authorities = new All<IAuthority>(); authorities.Add(authority.Object);
		world.SetupGet(x => x.Authorities).Returns(authorities);
		world.SetupGet(x => x.ChargenResources).Returns(new All<IChargenResource>());
		DummyAccount.Instance.SetupGameworld(native.World);
		foreach (var race in native.World.Races) Mock.Get(race).SetupGet(x => x.CombatSettings).Returns(new RacialCombatSettings());
		world.SetupGet(x => x.CharacterCombatSettings).Returns(new All<ICharacterCombatSettings>());
		var names = new Mock<INameCulture>(); names.SetupGet(x => x.Id).Returns(1); names.SetupGet(x => x.Gameworld).Returns(native.World);
		names.Setup(x => x.NamePattern(It.IsAny<NameStyle>())).Returns(Tuple.Create("{0}", new List<NameUsage> { NameUsage.BirthName }));
		var cultures = new All<INameCulture>(); cultures.Add(names.Object); world.SetupGet(x => x.NameCultures).Returns(cultures);
		using var db = NewIndependentContext(connection); var calendarId = db.Characters.Find(fixture.CharacterId)!.BirthdayCalendarId;
		var calendar = new Mock<ICalendar>(); calendar.SetupGet(x => x.Id).Returns(calendarId); calendar.SetupGet(x => x.Weekdays).Returns(["day"]);
		var definition = new MonthDefinition { Alias = "month", FullName = "Month", ShortName = "M", NormalDays = 30 };
		calendar.SetupGet(x => x.Months).Returns([definition]); var month = new Month(definition, 2000); var year = new Year([month], 2000, calendar.Object);
		calendar.Setup(x => x.GetDate(It.IsAny<string>())).Returns(new MudDate(calendar.Object, 1, 2000, month, year, false));
		var calendars = new All<ICalendar>(); calendars.Add(calendar.Object); world.SetupGet(x => x.Calendars).Returns(calendars);
		foreach (var pair in new[] { ("TotalBloodVolumeProg", 5m), ("LiverFunctionProg", 0m), ("MaximumStaminaProg", 100m) })
		{
			var prog = new Mock<IFutureProg>(); prog.Setup(x => x.Execute(It.IsAny<object[]>())).Returns(pair.Item2);
			typeof(RuntimeCharacter).GetField(pair.Item1, BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, prog.Object);
		}
	}

	private static void ClearArchiveFixtureMock(Mock mock, HashSet<Mock> visited)
	{
		if (!visited.Add(mock)) return;
		foreach (var call in mock.Invocations.ToArray())
		{
			foreach (var value in call.Arguments.Append(call.ReturnValue))
				if (value is IMocked child) ClearArchiveFixtureMock(child.Mock, visited);
		}
		mock.Invocations.Clear();
	}

	private static ArchiveControllerReceipt RetainArchiveController(RuntimeNPC npc)
	{
		var controller = (NPCController)npc.CharacterController;
		Require(ReferenceEquals(controller.Actor, npc) && ReferenceEquals(controller.OutputHandler.Perceiver, npc),
			"The native controller fixture did not retain the actor through both focus and output.");
		var observees = new List<IMonitorable> { controller }; var monitor = new Mock<IMonitor>();
		monitor.Setup(x => x.RemoveObservee(controller)).Callback(() =>
		{
			observees.Remove(controller); ((IMonitorable)controller).RemoveObserver(monitor.Object);
		});
		((IMonitorable)controller).AddObserver(monitor.Object);
		return new(controller, observees);
	}

	private static void RequireDetachedController(ArchiveControllerReceipt receipt)
	{
		Require(receipt.Controller.Actor is null && receipt.Controller.OutputHandler.Perceiver is null &&
			receipt.Observees.Count == 0 && typeof(NPCController).GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(receipt.Controller) is null,
			"A retained NPC controller/monitor still exposes the dead or archived actor.");
		receipt.Controller.HandleCommand("look"); receipt.Controller.CuePrompt();
	}

	private static Crime CreateArchiveCrime(TestDatabase database, NativeRuntime native, FixtureIds fixture, long characterId)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var authority = new Db.LegalAuthority { Name = "Archive history jurisdiction", Currency = new Db.Currency { Name = "Archive fixture currency" } };
		var law = new Db.Law { Name = "Archive history law", LegalAuthority = authority, CrimeType = (int)CrimeTypes.Assault,
			EnforcementStrategy = "NoActiveEnforcement", PunishmentStrategy = "<Punishment />" };
		var row = new Db.Crime { Law = law, CriminalId = characterId, VictimId = characterId, TimeOfCrime = "Never", RealTimeOfCrime = DateTime.UtcNow,
			CriminalShortDescription = "an archive guardian", CriminalFullDescription = "The historical guardian description.", CriminalCharacteristics = "",
			WitnessIds = fixture.CharacterId.ToString(), IsFinalised = true, IsCriminalIdentityKnown = true };
		db.Crimes.Add(row); db.SaveChanges();
		var runtimeAuthority = new Mock<ILegalAuthority>(); runtimeAuthority.SetupGet(x => x.Gameworld).Returns(native.World);
		runtimeAuthority.SetupGet(x => x.Name).Returns(authority.Name);
		var runtimeLaw = new Mock<ILaw>(); runtimeLaw.SetupGet(x => x.Gameworld).Returns(native.World);
		runtimeLaw.SetupGet(x => x.Authority).Returns(runtimeAuthority.Object); runtimeLaw.SetupGet(x => x.Name).Returns(law.Name);
		runtimeLaw.SetupGet(x => x.CrimeType).Returns(CrimeTypes.Assault); runtimeLaw.SetupGet(x => x.ActivePeriod).Returns(TimeSpan.FromDays(1));
		return new Crime(row, runtimeLaw.Object, native.World);
	}

	private static long CreateArchiveWriting(TestDatabase database, long characterId)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var writing = new Db.Writing { AuthorId = characterId, WritingType = "composite", Definition = "<Definition><Text>Retained historical text.</Text><DrawingSize>0</DrawingSize><DrawingSkill>35</DrawingSkill><ShortDescription>historical graffiti</ShortDescription></Definition>",
			Language = new Db.Language { Name = "Archive writing language", LinkedTraitId = db.TraitDefinitions.First().Id,
				UnknownLanguageDescription = "unknown", DifficultyModelNavigation = new Db.LanguageDifficultyModels { Name = "Archive writing model", Type = "WordList", Definition = "<Definition />" } },
			Script = new Db.Script { Name = "Archive writing script", KnownScriptDescription = "known", UnknownScriptDescription = "unknown",
				Knowledge = new Db.Knowledge { Name = "Archive script knowledge", Description = "script", LongDescription = "script", Type = "script", Subtype = "fixture" } } };
		db.Writings.Add(writing); db.SaveChanges(); return writing.Id;
	}

	private static long CreateArchiveTemplate(TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var template = new Db.NpcTemplate { Name = "Archive fixture", Type = "simple", UniqueName = "archive-fixture", Definition = "<Definition />", BuilderNotes = "",
			EditableItem = new Db.EditableItem { BuilderAccountId = 0, BuilderDate = DateTime.UtcNow, BuilderComment = "", ReviewerComment = "" } };
		db.NpcTemplates.Add(template); db.SaveChanges(); return template.Id;
	}

	private static SpellOwnedLifecycle CreateArchiveNpc(SpellOwnedLifecycleStore store, SpellLifecycleOrigin origin, FixtureIds fixture, long templateId) =>
		store.Create(origin, creation =>
		{
			var body = AddLifecycleBody(creation.Context, fixture.BodyId); creation.Claim(SpellOwnedEntityKind.Body, body);
			var source = creation.Context.Characters.AsNoTracking().Single(x => x.Id == fixture.CharacterId);
			var identity = (Db.Character)creation.Context.Entry(source).CurrentValues.ToObject(); identity.Id = 0; identity.Body = body; identity.BodyId = body.Id;
			identity.Name = "Archive Guardian"; identity.NameInfo = "<Names><PersonalName><Name culture='1'><Element usage='BirthName'>Archive Guardian</Element></Name></PersonalName><Aliases /><CurrentName>0</CurrentName></Names>";
			identity.NeedsModel = "NoNeeds"; identity.IsArchived = false; identity.DeathTime = null;
			creation.Context.Characters.Add(identity); creation.Claim(SpellOwnedEntityKind.AutonomousCharacter, identity);
			creation.Context.Npcs.Add(new() { Character = identity, TemplateId = templateId });
		});

	private static RuntimeNPC LoadArchiveNpc(NativeRuntime native, Futuremud roots, SpellOwnedLifecycle lifetime, bool preserveDeadState = false)
	{
		using var isolated = FMDB.BeginIndependentScope(requireWrites: true); using var db = new FMDB();
		var id = lifetime.Entities.Single(x => x.Kind == SpellOwnedEntityKind.AutonomousCharacter).Id;
		var identity = FMDB.Context.Characters.Include(x => x.Body).Single(x => x.Id == id);
		var model = FMDB.Context.Npcs.Include(x => x.NpcsArtificialIntelligences).Single(x => x.CharacterId == id);
		var npc = new RuntimeNPC(model, identity, native.World);
		if (!preserveDeadState) npc.State = CharacterState.Awake;
		((All<ICharacter>)roots.Actors).Add(npc); ((All<ICharacter>)roots.NPCs).Add(npc); ((All<IBody>)roots.Bodies).Add(npc.Body);
		npc.SetupEventSubscriptions(); return npc;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static WeakReference<RuntimeNPC> ArchiveRepeatedNpc(TestDatabase database, NativeRuntime native, Futuremud roots,
		SpellOwnedLifecycleStore store, CharacterArchiveService archives, SpellLifecycleOrigin origin, FixtureIds fixture, long templateId,
		DateTime now, List<Guid> completed, List<ArchiveControllerReceipt> retainedControllers, bool reloadDead)
	{
		var life = CreateArchiveNpc(store, origin, fixture, templateId); var npc = LoadArchiveNpc(native, roots, life);
		retainedControllers.Add(RetainArchiveController(npc));
		npc.Die(); life = store.ObserveDeath(life.Origin.Id, life.Version, null, now.AddMinutes(2));
		if (reloadDead)
		{
			// Drop the fixture's old process roots before loading the persisted dead graph through the native constructor.
			native.World.SaveManager.Abort(npc); native.World.SaveManager.Abort(npc.Body);
			((All<ICharacter>)roots.CachedActors).Remove(npc); ((All<ICharacter>)roots.Actors).Remove(npc);
			((All<ICharacter>)roots.NPCs).Remove(npc); ((All<IBody>)roots.Bodies).Remove(npc.Body);
			npc = LoadArchiveNpc(native, roots, life, true); retainedControllers.Add(RetainArchiveController(npc));
		}
		Require(archives.TryArchiveNpc(life.Origin.Id, life.Version, npc, now.AddMinutes(2), out var reason), "Repeated archive failed: " + reason);
		life = store.Complete(life.Origin.Id, life.Version, now.AddMinutes(2)); completed.Add(life.Origin.Id);
		return new WeakReference<RuntimeNPC>(npc);
	}

	private static int RunNpcArchiveReader(string[] args)
	{
		var input = JsonSerializer.Deserialize<ArchiveReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args.Single())))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var archives = new CharacterArchiveService(); var roots = ArchiveRoots();
		using var read = NewIndependentContext(database.ConnectionString);
		Require(archives.Find(input.Character)?.OriginalBodyId == input.Body && read.CharacterArchives.Count() == input.Archives &&
			read.Characters.Find(input.Character) is { IsArchived: true, BodyId: null } && !read.Bodies.Any(x => x.Id == input.Body) &&
			!read.Npcs.Any(x => x.CharacterId == input.Character) && !read.CharacterInstances.Any(x => x.CharacterId == input.Character) &&
			read.Set<Db.CharacterLog>().Find(input.History)!.CharacterId == input.Character && read.Wounds.Find(input.ForeignWound)!.ActorOriginId == input.Character &&
			read.GameItems.Any(x => x.Id == input.ForeignItem) && read.Crimes.Find(input.Crime) is { CriminalId: var criminal, IsFinalised: true } &&
			criminal == input.Character && roots.TryGetCharacter(input.Character, true) is null,
			"Independent restart lost archive, history or foreign state, or materialized a physical actor.");
		Require(read.Writings.Find(input.Writing) is { AuthorId: var author, TrueAuthorId: var trueAuthor } && author == input.Character && trueAuthor == input.Character &&
			read.Drawings.Find(input.Drawing) is { AuthorId: var drawingAuthor, FullDescription: "A retained drawing." } && drawingAuthor == input.Character,
			"Independent restart lost historical authorship or drawing content.");
		var store = new SpellOwnedLifecycleStore(); var life = store.Find(input.Lifecycle)!;
		Require(store.Complete(input.Lifecycle, life.Version, life.UpdatedUtc).Version == life.Version && !store.Pending(life.UpdatedUtc).Any(),
			"Restart completion reopened or advanced the archive lifetime.");
		Console.WriteLine("ARM03B-reader=passed independent-process tombstone-and-archive history-attribution-preserved no-heavy-actor-materialization completed-idempotence no-pending-work");
		return 0;
	}
}
