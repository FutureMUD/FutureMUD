#nullable enable

using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health.Corpses;
using MudSharp.Health;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health.Breathing;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MudSharp.Magic.SpellEffects;
using MudSharp.NPC.Templates;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;
using Db = MudSharp.Models;
using RuntimeNpc = MudSharp.NPC.NPC;
using RuntimeCharacter = MudSharp.Character.Character;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record NativeNpcReader(string Database, Guid Death, Guid Held, long HeldCharacter, long Corpse,
		long ForeignItem, long Body, int Characters, int Bodies, int Lifecycles, int CharacterKnowledges, long OwnedCharacter, long LegacyCharacter, long Skill);

	private static int RunSpellOwnedNpcAcceptanceChecks()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString);
		Console.WriteLine($"ARM03B2A-created={database.Name}");
		var fixture = FixtureSeed.Create(database, "arm03b2a_native", true);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			db.Database.Migrate();
			Require(!db.Database.HasPendingModelChanges(), "Native adapter model parity failed.");
			db.Database.ExecuteSqlRaw("ALTER TABLE Characters AUTO_INCREMENT=1000000");
			db.Database.ExecuteSqlRaw("ALTER TABLE Bodies AUTO_INCREMENT=2000000");
			db.Database.ExecuteSqlRaw("ALTER TABLE CharacterInstances AUTO_INCREMENT=3000000");
		}
		var native = NativeRuntime.Load(fixture, database.ConnectionString, true);
		ConfigureCastingWorld(native, database.ConnectionString, true); PrepareLifecycleRuntime(native);
		var roots = ArchiveRoots(); ConfigureArchiveWorld(native, roots, fixture, database.ConnectionString);
		var service = new SpellOwnedNpcService(native.World); var store = new SpellOwnedLifecycleStore();
		native.WorldMock.SetupGet(x => x.SpellOwnedNpcs).Returns(service);
		native.WorldMock.SetupGet(x => x.DefaultHooks).Returns(Array.Empty<IDefaultHook>());
		native.WorldMock.SetupGet(x => x.Currencies).Returns(new All<ICurrency>());
		native.WorldMock.Setup(x => x.GetStaticString("MaximumStaminaExpression")).Returns("100");
		Mock.Get(native.Body.Race).SetupGet(x => x.BaseBody).Returns(native.World.BodyPrototypes.First());
		Mock.Get(native.Body.Race).SetupGet(x => x.BreathingStrategy).Returns(new NonBreather());
		var templates = new Mock<IUneditableRevisableAll<INPCTemplate>>();
		templates.Setup(x => x.GetEnumerator()).Returns(() => Enumerable.Empty<INPCTemplate>().GetEnumerator());
		native.WorldMock.SetupGet(x => x.NpcTemplates).Returns(templates.Object);
		var rawSkill = native.World.Traits.GetByName("ARM02 Earth Proficiency") as SkillDefinition
			?? throw new InvalidOperationException("The native raw-skill fixture definition is missing.");
		rawSkill.Cap = new TraitExpression("50", native.World);
		var templateData = new SimpleCharacterTemplate
		{
			Gameworld = native.World, SelectedName = new PersonalName(new XElement("Name", new XAttribute("culture", 1),
				new XElement("Element", new XAttribute("usage", "BirthName"), "Native Guardian")), native.World),
			SelectedRace = native.Body.Race, SelectedEthnicity = native.Body.Ethnicity, SelectedCulture = native.Actor.Culture,
			SelectedBirthday = native.World.Calendars.First().GetDate("1-month-2000"), SelectedStartingLocation = native.Actor.Location,
			SelectedGender = native.Body.Gender.Enum, SelectedHeight = 1.8, SelectedWeight = 80,
			SelectedSdesc = "a native guardian", SelectedFullDesc = "A guardian created through the native template adapter.",
			SelectedAccents = [], SelectedAttributes = [], SelectedCharacteristics = [], SelectedEntityDescriptionPatterns = [],
			SkillValues = [(rawSkill, 80d)], SelectedRoles = [], SelectedMerits = [], SelectedKnowledges = [], MissingBodyparts = [],
			SelectedDisfigurements = [], SelectedProstheses = []
		};
		var template = new SimpleNPCTemplate(native.World, DummyAccount.Instance, templateData, "ARM03B2A native template");
		Db.Knowledge knowledgeModel;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			knowledgeModel = new() { Name = "ARM03B2A native lore", Description = "Native starting knowledge", LongDescription = "Native starting knowledge", Type = "Lore", Subtype = "Native" };
			db.Knowledges.Add(knowledgeModel); db.SaveChanges();
		}
		var knowledge = new Mock<IKnowledge>(); knowledge.SetupGet(x => x.Id).Returns(knowledgeModel.Id);
		knowledge.SetupGet(x => x.Name).Returns(knowledgeModel.Name); knowledge.SetupGet(x => x.Gameworld).Returns(native.World);
		var knowledges = new All<IKnowledge>(); knowledges.Add(knowledge.Object); native.WorldMock.SetupGet(x => x.Knowledges).Returns(knowledges);
		Require(template.BuildingCommand(native.Actor, new StringStack($"knowledge {knowledgeModel.Id}")), "The native knowledge builder refused its persisted catalogue.");
		using (var isolated = FMDB.BeginIndependentScope(requireWrites: true))
		using (var db = new FMDB()) { template.Save(); FMDB.Context.SaveChanges(); }
		native.World.SaveManager.Abort(template);
		templates.Setup(x => x.Get(template.Id)).Returns(template); templates.Setup(x => x.Get(template.Id, 0)).Returns(template);
		templates.Setup(x => x.GetByIdOrName(template.Id.ToString(), true)).Returns(template);
		var spell = new MagicSpell("ARM03B2A native provenance", native.Capability.School);
		var location = native.Actor.SpatialLocation;
		SpellLifecycleOrigin Origin(SpellLifecycleMode mode = SpellLifecycleMode.DeathOnExpiry)
		{
			// The native schema stores microseconds. Use that precision for exact round-trip comparison.
			var created = new DateTime(RuntimeClock.UtcNow.Ticks / 10 * 10, DateTimeKind.Utc);
			return new(Guid.NewGuid(), spell.Id, 3, fixture.CharacterId, "native-guardian", mode, created,
				mode == SpellLifecycleMode.Permanent ? null : created.AddMinutes(5), "native-simple-template adapter fixture");
		}
		var liveAdded = 0;
		native.WorldMock.Setup(x => x.Add(It.IsAny<ICharacter>(), true)).Callback<ICharacter, bool>((actor, _) =>
		{
			using var read = NewIndependentContext(database.ConnectionString);
			Require(((ILateInitialisingItem)actor).IdHasBeenRegistered && actor.Body.IdHasBeenRegistered && read.Characters.Find(actor.Id)!.State == (int)CharacterState.Awake &&
				read.Bodies.Find(actor.Body.Id)!.CurrentBloodVolume == 5 && read.MagicSpellOwnedEntities.Any(x => x.EntityId == actor.Id && x.Kind == 1),
				"World insertion preceded committed native identity, vitals or ownership.");
			roots.Add(actor, true); roots.Add(actor.Body); liveAdded++;
		});
		native.WorldMock.Setup(x => x.Add(It.IsAny<IGameItem>())).Callback<IGameItem>(item => ((All<IGameItem>)native.World.Items).Add(item));
		// A real pending save must survive construction; any global flush would hit this sentinel.
		var sentinel = new Mock<ISaveable>(); sentinel.Setup(x => x.Save()).Throws(new InvalidOperationException("Unrelated global save was flushed."));
		native.World.SaveManager.Add(sentinel.Object);
		var born = Origin(); var npc = (RuntimeNpc)template.CreateSpellOwnedCharacter(location, born);
		Require(native.World.SaveManager.IsQueued(sentinel.Object) && !native.World.SaveManager.IsQueued(npc) &&
			!native.World.SaveManager.IsQueued(npc.Body) && !npc.CharacterKnowledges.Any(native.World.SaveManager.IsQueued) && liveAdded == 0 && npc.Id > 0 && npc.InstanceId > 0,
			"Atomic native construction exposed roots, flushed unrelated saves, or left its own initialization pending.");
		sentinel.Verify(x => x.Save(), Times.Never); native.World.SaveManager.Abort(sentinel.Object);
		var birth = store.Find(born.Id)!;
		Require(birth.Version == 2 && birth.Diagnostic == "" && birth.Entities.Count == 2 && birth.Origin == born,
			"Native creation did not atomically finalize its exact actor/body journal.");
		Console.WriteLine("ARM03B2A-creation=passed real-SimpleNPCTemplate native-new-NPC-Body-primary-instance production-DatabaseInsert atomic-claims activation-persisted-before-exposure unrelated-real-save-queue-conserved");
		using (var read = NewIndependentContext(database.ConnectionString))
			Require(npc.CharacterKnowledges.Single() is CharacterKnowledge { Id: > 0 } starting && !starting.GetNoSave() &&
				read.CharacterKnowledges.Single(x => x.CharacterId == npc.Id).Id == starting.Id &&
				read.CharacterKnowledges.Single(x => x.CharacterId == npc.Id).KnowledgeId == knowledgeModel.Id,
				"Starting knowledge did not receive its committed native row or resume ordinary saving.");
		Console.WriteLine("ARM03B2A-starting-knowledge=passed actual-native-builder-and-CharacterKnowledge graph-inserted-before-identity-resumed no-precommit-child-save no-global-flush no-pending-private-knowledge");
		var legacyNpc = (RuntimeNpc)template.CreateNewCharacter(location);
		native.World.SaveManager.Flush(); // Exercise ordinary native insertion separately, without a pending sentinel.
		using (var read = NewIndependentContext(database.ConnectionString))
			Require(npc.GetTrait(rawSkill) is { RawValue: 80, Value: 50 } && legacyNpc.GetTrait(rawSkill) is { RawValue: 80, Value: 50 } &&
				read.CharacterTraits.Find(npc.Id, rawSkill.Id)!.Value == 80 && read.CharacterTraits.Find(legacyNpc.Id, rawSkill.Id)!.Value == 80,
				"Native activation trimmed authored raw skill history relative to legacy creation.");
		Console.WriteLine("ARM03B2A-raw-skills=passed native-owned-and-legacy-template raw80-dynamic-cap50 persisted80 real-save-isolation retained-authored-history");
		Refuse(() => template.CreateSpellOwnedCharacter(location, born), "Native creation replay created another NPC.");
		using (FMDB.BeginIsolatedScope(suppressEfWrites: true))
			Refuse(() => template.CreateSpellOwnedCharacter(location, Origin()), "Suppressed caller reached native creation.");
		Console.WriteLine("ARM03B2A-replay=passed stable-creation-key-refusal write-suppressed-preparation-refusal no-recreated-native-graph");

		(int Characters, int Bodies, int Lifecycles, int CharacterKnowledges) Census()
		{
			using var read = NewIndependentContext(database.ConnectionString);
			return (read.Characters.Count(), read.Bodies.Count(), read.MagicSpellLifecycles.Count(), read.CharacterKnowledges.Count());
		}
		HashSet<object> QueuedRoots() => new(new[] { "_saveStack", "_delayedSaveStack", "_initialisationQueue", "_lazyLoaders" }
			.SelectMany(field => ((System.Collections.IEnumerable)typeof(SaveManager).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance)!
				.GetValue(native.World.SaveManager)!).Cast<object>()), ReferenceEqualityComparer.Instance);
		var before = Census(); var beforeQueues = QueuedRoots(); var failed = Origin();
		using (var db = NewIndependentContext(database.ConnectionString))
			db.Database.ExecuteSqlRaw("CREATE TRIGGER arm03b2a_creation_refusal BEFORE INSERT ON MagicSpellLifecycles FOR EACH ROW SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'ARM03B2A ownership rollback fixture'");
		var rollback = false;
		try { template.CreateSpellOwnedCharacter(location, failed); }
		catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("ARM03B2A ownership rollback fixture") == true) { rollback = true; }
		finally { using var db = NewIndependentContext(database.ConnectionString); db.Database.ExecuteSqlRaw("DROP TRIGGER arm03b2a_creation_refusal"); }
		Require(rollback && Census() == before && beforeQueues.SetEquals(QueuedRoots()) && store.Find(failed.Id) is null && liveAdded == 0,
			"Failed ownership save leaked native rows, child save roots or world activation.");
		Console.WriteLine("ARM03B2A-rollback=passed provider-refused-journal-write native-character-body-instance-NPC-and-starting-knowledge-insert-rolled-back exact-real-save-queues-conserved no-world-exposure");

		var hookCalls = 0; RuntimeNpc? unpublished = null;
		var capture = new Mock<IFutureProg>(); capture.Setup(x => x.Execute(It.IsAny<object[]>())).Returns<object[]>(values =>
		{
			unpublished = values[0] as RuntimeNpc;
			return 0m;
		});
		var priorLiver = typeof(RuntimeCharacter).GetField("LiverFunctionProg", BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null);
		typeof(RuntimeCharacter).GetField("LiverFunctionProg", BindingFlags.NonPublic | BindingFlags.Static)!.SetValue(null, capture.Object);
		var firstHook = new Mock<IHook>(); firstHook.SetupGet(x => x.Type).Returns(EventType.FiveSecondTick);
		firstHook.SetupGet(x => x.Function).Returns((EventType _, object[] _) => { hookCalls++; return false; });
		var firstDefault = new Mock<IDefaultHook>(); firstDefault.SetupGet(x => x.Hook).Returns(firstHook.Object);
		firstDefault.Setup(x => x.Applies(It.IsAny<IProgVariable>(), "Character")).Returns(true);
		var failingDefault = new Mock<IDefaultHook>();
		failingDefault.Setup(x => x.Applies(It.IsAny<IProgVariable>(), "Character")).Throws(new InvalidOperationException("ARM03B2A hook activation refusal"));
		native.WorldMock.SetupGet(x => x.DefaultHooks).Returns([firstDefault.Object, failingDefault.Object]);
		var held = Origin(); var activationFailed = false;
		try { template.CreateSpellOwnedCharacter(location, held); }
		catch (InvalidOperationException ex) when (ex.Message.Contains("ARM03B2A hook activation refusal")) { activationFailed = true; }
		finally
		{
			native.WorldMock.SetupGet(x => x.DefaultHooks).Returns(Array.Empty<IDefaultHook>());
			typeof(RuntimeCharacter).GetField("LiverFunctionProg", BindingFlags.NonPublic | BindingFlags.Static)!.SetValue(null, priorLiver);
		}
		Mock.Get(native.World.HeartbeatManager).Raise(x => x.FuzzyFiveSecondHeartbeat += null!);
		var heldLife = store.Find(held.Id)!; var heldId = heldLife.Entities.Single(x => x.Kind == SpellOwnedEntityKind.AutonomousCharacter).Id;
		using (var read = NewIndependentContext(database.ConnectionString))
			Require(activationFailed && unpublished is not null && hookCalls == 0 && !native.World.SaveManager.IsQueued(unpublished) &&
				!native.World.SaveManager.IsQueued(unpublished.Body) && !unpublished.CharacterKnowledges.Any(native.World.SaveManager.IsQueued) &&
				unpublished.CharacterKnowledges.OfType<SaveableItem>().All(x => x.GetNoSave()) && unpublished.Hooks.Count() == 0 && heldLife.Diagnostic.StartsWith("Native NPC activation pending") &&
				read.Characters.Find(heldId)!.State == (int)CharacterState.Stasis &&
				read.CharacterInstances.Single(x => x.CharacterId == heldId && x.IsPrimary).State == (int)CharacterState.Stasis,
				"Failed activation leaked a heartbeat/save root or an active persisted NPC.");
		Refuse(() => template.CreateSpellOwnedCharacter(location, held), "Failed activation replay created another native graph.");
		Console.WriteLine("ARM03B2A-activation-hold=passed ordered-real-hook-install-then-predicate-failure real-heartbeat-raised-no-ghost-callback save-roots-released durable-character-and-primary-stasis replay-refused");

		// Exercise the actual effect, numerical binding and native template overload. Load callbacks inspect
		// committed provenance through independent readers, rather than writing their own journal.
		var loadCallbacks = 0;
		var onload = new Mock<IFutureProg>(); onload.SetupGet(x => x.Id).Returns(90000001);
		onload.SetupGet(x => x.Public).Returns(true); onload.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Void);
		onload.SetupGet(x => x.FunctionName).Returns("native_load_check");
		onload.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		onload.Setup(x => x.Execute(It.IsAny<object[]>())).Returns<object[]>(values =>
		{
			var loaded = (ICharacter)values[0];
			using var read = NewIndependentContext(database.ConnectionString);
			var row = read.MagicSpellLifecycles.Include(x => x.Entities).Single(x => x.Entities.Any(e => e.Kind == 1 && e.EntityId == loaded.Id));
			Require(row.Grade == 3 && row.CreatorId == fixture.CharacterId && row.DeadlineUtc > row.CreatedUtc && row.Diagnostic == "" &&
				read.Characters.Find(loaded.Id)!.State == (int)CharacterState.Awake, "On-load program preceded durable selected-grade provenance or activation.");
			loadCallbacks++; return null!;
		});
		((All<IFutureProg>)native.World.FutureProgs).Add(onload.Object); template.OnLoadProg = onload.Object;
		foreach (var command in new[] { "grades fixture", "effect add createnpc", $"effect 1 npc {template.Id}",
			"effect 1 lifecycle deathonexpiry", "effect 1 family native-guardian", "effect 1 lifetime grade*60", $"effect 1 prog {onload.Object.Id}" })
			Require(spell.BuildingCommand(native.Actor, new StringStack(command)), "Native builder refused " + command);
		var trait = native.World.Traits.GetByName("ARM02 Earth Proficiency");
		var copy = (MagicSpell)typeof(MagicSpell).GetMethod("CastingCopy", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(spell, [native.Actor, trait, 3, SpellPower.Weak, Difficulty.Easy, 3])!;
		var effect = (CreateNPCEffect)copy.SpellEffects.Single();
		var adjustedRole = new Mock<IChargenRole>();
		adjustedRole.SetupGet(x => x.TraitAdjustments).Returns(new Dictionary<ITraitDefinition, (double amount, bool giveIfMissing)> { [rawSkill] = (5, false) });
		var beforeRole = Census(); var roleQueues = QueuedRoots(); template.SelectedRoles.Add(adjustedRole.Object);
		try
		{
			Require(effect.DefinitionError?.Contains("role trait adjustments") == true &&
				!effect.TryPrepareApplication(native.Actor, native.Actor.Location, default, SpellPower.Weak, TimeSpan.FromSeconds(1), out _, out _),
				"Unadapted role trait mutation reached spell application admission.");
			Refuse(() => template.CreateSpellOwnedCharacter(location, Origin()), "Unadapted role trait mutation reached native construction.");
			Require(Census() == beforeRole && roleQueues.SetEquals(QueuedRoots()) && liveAdded == 0,
				"Role admission refusal changed persisted rows, save roots or world exposure.");
		}
		finally { template.SelectedRoles.Remove(adjustedRole.Object); }
		Console.WriteLine("ARM03B2A-role-admission=passed unadapted-trait-adjustment-refused-before-effect-application-and-native-constructor exact-persisted-census-and-real-save-queues-conserved legacy-role-semantics-retained");
		Require(effect.TryPrepareApplication(native.Actor, native.Actor.Location, default, SpellPower.Weak, TimeSpan.FromSeconds(1), out var application, out var preparationError),
			"Native effect admission failed: " + preparationError);
		application!.Create(new MagicSpellParent(native.Actor.Location, copy, native.Actor));
		Require(loadCallbacks == 2 && liveAdded == 1, "The actual native effect did not run both on-load programs after one committed spawn.");
		var spawned = (RuntimeNpc)roots.NPCs.Single();
		var effectLife = store.Find(ReadNativeNpcLifecycle(database, spawned.Id))!;
		Require(effectLife.Origin.Grade == 3 && (effectLife.Origin.DeadlineUtc - effectLife.Origin.CreatedUtc)!.Value == TimeSpan.FromMinutes(3),
			"Grade-bound NPC lifetime was confused with spell/control duration.");
		Refuse(() => application.Create(new MagicSpellParent(native.Actor.Location, copy, native.Actor)), "Prepared creation application replay spawned another NPC.");
		Console.WriteLine("ARM03B2A-effect=passed actual-builder-createnpc CastingCopy-grade-binding production-effect-native-overload world-before-template-and-effect-onload callbacks-read-committed-provenance lifetime-independent stable-application-key");

		var permanent = Origin(SpellLifecycleMode.Permanent); var permanentNpc = (RuntimeNpc)template.CreateSpellOwnedCharacter(location, permanent);
		Require(store.Find(permanent.Id) is { State: SpellLifecycleState.Completed, Origin.DeadlineUtc: null } && permanentNpc.State == CharacterState.Awake,
			"Permanent native creation did not release to ordinary durable existence.");
		Refuse(() => store.BeginRetirement(permanent.Id, store.Find(permanent.Id)!.Version, SpellRetirementReason.Expiry, RuntimeClock.UtcNow.AddDays(1)), "Permanent creation was expired.");
		Console.WriteLine("ARM03B2A-permanent=passed native-permanent-creation durable-completed-provenance no-deadline ordinary-live-NPC expiry-refused N15-full-mode-retirement-NOT-RUN");

		var previousCorpseProto = CorpseGameItemComponentProto.ItemProto;
		var corpsePrototype = BuildNativeNpcCorpsePrototype(database, native);
		var nativeDeaths = 0; spawned.OnDeath += _ =>
		{
			nativeDeaths++;
			using var read = NewIndependentContext(database.ConnectionString);
			Require((read.Characters.Find(spawned.Id)!.State & (int)CharacterState.Dead) == 0 && store.Find(effectLife.Origin.Id)!.DeathObservedUtc is null,
				"Pre-death callback was treated as persisted death proof.");
		};
		long foreign;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var item = NewLifecycleItem(); db.GameItems.Add(item); db.SaveChanges(); foreign = item.Id;
			db.BodiesGameItems.Add(new() { BodyId = spawned.Body.Id, GameItemId = foreign }); db.SaveChanges();
			db.Database.ExecuteSqlRaw("CREATE TRIGGER arm03b2a_death_journal_refusal BEFORE UPDATE ON MagicSpellLifecycles FOR EACH ROW BEGIN IF NEW.DeathObservedUtc IS NOT NULL AND OLD.DeathObservedUtc IS NULL THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='ARM03B2A death journal retry fixture'; END IF; END");
		}
		IGameItem corpse;
		try { CorpseGameItemComponentProto.ItemProto = corpsePrototype; corpse = spawned.Die(); }
		finally
		{
			CorpseGameItemComponentProto.ItemProto = previousCorpseProto;
			using var db = NewIndependentContext(database.ConnectionString); db.Database.ExecuteSqlRaw("DROP TRIGGER arm03b2a_death_journal_refusal");
		}
		Require(corpse is GameItem && corpse.GetItemType<ICorpse>()!.OriginalBodyId == spawned.Body.Id && nativeDeaths == 1 &&
			store.Find(effectLife.Origin.Id)!.DeathObservedUtc is null && store.Find(effectLife.Origin.Id)!.Diagnostic.Contains("retry"),
			"Actual native death/remains or durable failed-correlation hold did not occur.");
		using (var read = NewIndependentContext(database.ConnectionString))
			Require((read.Characters.Find(spawned.Id)!.State & (int)CharacterState.Dead) != 0 &&
				read.GameItemComponents.Any(x => x.GameItemId == corpse.Id) && read.GameItems.Any(x => x.Id == foreign) &&
				read.BodiesGameItems.Any(x => x.BodyId == spawned.Body.Id && x.GameItemId == foreign), "Death/remains were not durable or foreign custody was changed.");
		Console.WriteLine("ARM03B2A-native-death=passed actual-NPC-Die actual-GameItemProto-CreateNew actual-corpse-factory-component-and-SaveManager predeath-not-proof persisted-dead-character-and-primary real-remains-XML foreign-unloaded-join-retained forced-correlation-failure-held");
		// Correlation cannot guess which remains to own, or infer absence from malformed persisted XML.
		long probeId;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var original = db.GameItemComponents.Single(x => x.GameItemId == corpse.Id);
			var probe = new Db.GameItemComponent { GameItemId = foreign, GameItemComponentProtoId = original.GameItemComponentProtoId,
				GameItemComponentProtoRevision = original.GameItemComponentProtoRevision, Definition = original.Definition };
			db.GameItemComponents.Add(probe); db.SaveChanges(); probeId = probe.Id;
		}
		Require(service.ReconcilePersistedDeaths(RuntimeClock.UtcNow, 1) == 1 &&
			store.Find(effectLife.Origin.Id) is { DeathObservedUtc: null } ambiguous && ambiguous.Diagnostic.Contains("Multiple persisted remains"),
			"Ambiguous native remains were automatically correlated.");
		Require(service.ReconcilePersistedDeaths(RuntimeClock.UtcNow, 1) == 0, "The bounded reconciliation cursor did not end its pass.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			db.GameItemComponents.Find(probeId)!.Definition = $"<Definition><OriginalBody>{spawned.Body.Id}";
			db.SaveChanges();
		}
		Require(service.ReconcilePersistedDeaths(RuntimeClock.UtcNow, 1) == 1 && store.Find(effectLife.Origin.Id)!.DeathObservedUtc is null,
			"Malformed matching XML proved remains absence or bypassed a durable hold.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(db.GameItems.Any(x => x.Id == foreign) && db.BodiesGameItems.Any(x => x.BodyId == spawned.Body.Id && x.GameItemId == foreign) &&
				db.GameItemComponents.Any(x => x.GameItemId == corpse.Id), "Correlation holds changed native remains or foreign custody.");
			db.GameItemComponents.Remove(db.GameItemComponents.Find(probeId)!); db.SaveChanges(); // Remove only the injected fixture probe.
		}
		Console.WriteLine("ARM03B2A-reconcile-holds=passed bounded-DB-only ambiguous-two-remains-refusal malformed-matching-XML-refusal native-corpse-and-foreign-item-join-conserved no-automatic-ownership-or-delete");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var component = db.GameItemComponents.Single(x => x.GameItemId == corpse.Id);
			var token = spawned.Body.Id.ToString(System.Globalization.CultureInfo.InvariantCulture);
			var encoded = string.Concat(token.Select(character => $"&#{(int)character};"));
			component.Definition = component.Definition.Replace(token, encoded);
			Require(!component.Definition.Contains(token) && (long?)XElement.Parse(component.Definition).Element("OriginalBody") == spawned.Body.Id,
				"Encoded native remains regression did not preserve the exact parsed body while hiding raw digits.");
			db.SaveChanges();
		}
		var census = Census();
		RunNativeNpcReaderProcess(new(database.Name, effectLife.Origin.Id, held.Id, heldId, corpse.Id, foreign, spawned.Body.Id,
			census.Characters, census.Bodies, census.Lifecycles, census.CharacterKnowledges, npc.Id, legacyNpc.Id, rawSkill.Id));
		var observed = store.Find(effectLife.Origin.Id)!;
		Require(observed.State == SpellLifecycleState.RemainsPending && observed.RemainsItemId == corpse.Id && !observed.RequiresNativeDeath,
			"Separate-process restart did not correlate native death/remains.");
		Console.WriteLine("ARM03B2A-encoded-remains=passed actual-native-corpse-XML-body-digits-encoded character-references-parsed separate-process-exact-corpse-correlation no-false-absence");
		Require(ReferenceEquals(spawned.Die(), corpse) && nativeDeaths == 1 && store.Find(effectLife.Origin.Id)!.Version == observed.Version,
			"Native already-dead retry replayed death or changed its correlated remains.");
		Require(service.ReconcilePersistedDeaths(effectLife.Origin.DeadlineUtc!.Value.AddMinutes(1)) == 0 && nativeDeaths == 1 && Census() == census,
			"Passing original expiry replayed death, creation or destruction.");
		Console.WriteLine("ARM03B2A-death-replay=passed actual-already-dead-NPC-Die existing-remains-returned one-OnDeath no-second-corpse no-creation-or-delete-after-deadline");
		Console.WriteLine("ARM03B2A-qualifier=controlled-world-catalogues-and-cell-host real-native-template-new-character-body-item-prototype-corpse-factory-save-and-Die DB-only-restart no-full-GameItem-Delete no-timed-decay no-foreign-evacuation no-Character-Quit no-expiry-death-worker no-installed-command-session N14-N15-N16-NOT-RUN");
		return 0;
	}

	private static Guid ReadNativeNpcLifecycle(TestDatabase database, long id)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		return db.MagicSpellOwnedEntities.Single(x => x.Kind == 1 && x.EntityId == id).LifecycleId;
	}

	private static GameItemProto BuildNativeNpcCorpsePrototype(TestDatabase database, NativeRuntime native)
	{
		Db.GameItemComponentProto componentModel;
		Db.GameItemProto itemModel;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			componentModel = new() { Name = "Native corpse component", Description = "Native test corpse", Type = "Corpse", Definition = "<Definition/>",
				EditableItem = new() { BuilderAccountId = 0, BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
			db.GameItemComponentProtos.Add(componentModel); db.SaveChanges();
			itemModel = new() { Name = "Native corpse", Keywords = "corpse", ShortDescription = "a corpse", FullDescription = "Native corpse",
				MaterialId = native.World.Materials.First().Id, Size = 3, Weight = 1, BaseItemQuality = (int)ItemQuality.Standard,
				MorphEmote = "$0 decays.", EditableItem = new() { BuilderAccountId = 0, BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
			itemModel.GameItemProtosGameItemComponentProtos.Add(new() { GameItemProto = itemModel, GameItemComponentProtoId = componentModel.Id });
			db.GameItemProtos.Add(itemModel); db.SaveChanges();
		}
		var component = (CorpseGameItemComponentProto)typeof(CorpseGameItemComponentProto).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
			null, [typeof(Db.GameItemComponentProto), typeof(IFuturemud)], null)!.Invoke([componentModel, native.World]);
		var components = new Mock<IUneditableRevisableAll<IGameItemComponentProto>>();
		components.Setup(x => x.Get(component.Id, 0)).Returns(component); native.WorldMock.SetupGet(x => x.ItemComponentProtos).Returns(components.Object);
		var proto = new GameItemProto(itemModel, native.World);
		var prototypes = new Mock<IUneditableRevisableAll<IGameItemProto>>();
		prototypes.Setup(x => x.Get(proto.Id, 0)).Returns(proto); native.WorldMock.SetupGet(x => x.ItemProtos).Returns(prototypes.Object);
		NonDecayingCorpseModel.RegisterTypeLoader();
		var model = CorpseModelFactory.LoadCorpseModel(new Db.CorpseModel { Id = 1, Type = "NonDecaying", Name = "Native fixture remains", Description = "Native retained remains",
			Definition = $"<Definition><SDesc>a corpse of {{0}}</SDesc><FDesc>The corpse of {{0}}</FDesc><PartDesc>a part</PartDesc><CorpseMaterial>{native.World.Materials.First().Id}</CorpseMaterial></Definition>" }, native.World);
		Mock.Get(native.Body.Race).SetupGet(x => x.CorpseModel).Returns(model);
		var models = new All<ICorpseModel>(); models.Add(model); native.WorldMock.SetupGet(x => x.CorpseModels).Returns(models);
		return proto;
	}

	private static void RunNativeNpcReaderProcess(NativeNpcReader input)
	{
		var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
		start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); start.ArgumentList.Add("--spell-owned-npc-reader");
		start.ArgumentList.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(input))));
		using var process = Process.Start(start)!; var output = process.StandardOutput.ReadToEndAsync(); var error = process.StandardError.ReadToEndAsync();
		if (!process.WaitForExit(60000)) { process.Kill(true); throw new TimeoutException("Native NPC restart reader exceeded 60 seconds."); }
		Console.Write(output.GetAwaiter().GetResult()); Require(process.ExitCode == 0, error.GetAwaiter().GetResult());
	}

	private static int RunSpellOwnedNpcReader(string[] args)
	{
		var input = JsonSerializer.Deserialize<NativeNpcReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args.Single())))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var store = new SpellOwnedLifecycleStore(); var service = new SpellOwnedNpcService(Mock.Of<IFuturemud>());
		var life = store.Find(input.Death)!; var now = life.Origin.DeadlineUtc!.Value.AddMinutes(-1);
		Require(life.DeathObservedUtc is null && life.State == SpellLifecycleState.Active && service.ReconcilePersistedDeaths(now, 1) == 1,
			"Restart did not find the unobserved early-dead Active NPC before expiry.");
		life = store.Find(input.Death)!;
		Require(life.DeathObservedUtc is not null && life.RemainsItemId == input.Corpse && life.State == SpellLifecycleState.RemainsPending && !life.RequiresNativeDeath,
			"Restart did not independently correlate the exact native remains.");
		var held = store.Find(input.Held)!; var roots = ArchiveRoots();
		using var read = NewIndependentContext(database.ConnectionString);
		// Reload actual native skills from the separately persisted creation rows, without constructing
		// any dead actor/body graph. A cap rise must reveal the retained authored history.
		var skillWorld = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var skill = new SkillDefinition(read.TraitDefinitions.Find(input.Skill)!, skillWorld.Object) { Cap = new TraitExpression("50", skillWorld.Object) };
		foreach (var characterId in new[] { input.OwnedCharacter, input.LegacyCharacter })
		{
			var row = read.CharacterTraits.Find(characterId, input.Skill)!;
			var loaded = skill.LoadTrait(new Db.Trait { Value = row.Value, AdditionalValue = row.AdditionalValue }, Mock.Of<ICharacter>());
			Require(loaded.RawValue == 80 && loaded.Value == 50, "Separate-process native skill reload lost raw template history.");
			skill.Cap = new TraitExpression("100", skillWorld.Object);
			Require(loaded.RawValue == 80 && loaded.Value == 80 && row.Value == 80, "Raising the native cap failed to reveal preserved raw history.");
			skill.Cap = new TraitExpression("50", skillWorld.Object);
		}
		Require(read.Characters.Find(input.HeldCharacter)!.State == (int)CharacterState.Stasis && held.Diagnostic.StartsWith("Native NPC activation pending") &&
			roots.TryGetCharacter(input.HeldCharacter, true) is null && read.GameItems.Any(x => x.Id == input.ForeignItem) &&
			read.BodiesGameItems.Any(x => x.BodyId == input.Body && x.GameItemId == input.ForeignItem) &&
			read.Characters.Count() == input.Characters && read.Bodies.Count() == input.Bodies && read.MagicSpellLifecycles.Count() == input.Lifecycles &&
			read.CharacterKnowledges.Count() == input.CharacterKnowledges &&
			service.ReconcilePersistedDeaths(now, 1) == 0 && store.Find(input.Death)!.Version == life.Version,
			"Restart recreated, materialized an incomplete graph, changed foreign custody, or replayed death observation.");
		Console.WriteLine("ARM03B2A-reader=passed independent-process early-dead-active-before-expiry DB-only-correlation exact-body-remains no-heavy-dead-NPC-materialization held-stasis-TryGetCharacter-refusal no-recreation foreign-ID-and-join-conserved idempotent-bounded-retry actual-skill-reload-owned-and-legacy raw80-cap50-raised100-reveals80");
		return 0;
	}
}
