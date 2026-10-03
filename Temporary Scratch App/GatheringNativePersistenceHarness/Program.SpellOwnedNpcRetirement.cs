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
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.Health.Breathing;
using MudSharp.Health.Corpses;
using MudSharp.Health.Strategies;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MudSharp.NPC.Templates;
using MudSharp.Planes;
using MudSharp.Work.Projects;
using Db = MudSharp.Models;
using RuntimeNpc = MudSharp.NPC.NPC;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record RetirementReader(string Database, FixtureIds Fixture, Guid Lifecycle, DateTime Now,
		long Corpse, long[] Foreign, string Action, long ExternalItem = 0);
	private sealed record RetirementHost(NativeRuntime Native, Futuremud Roots, SpellOwnedNpcService Service,
		SpellOwnedLifecycleStore Store, Scheduler Scheduler, HeartbeatManager Heartbeats, All<IGameItem> Items,
		Dictionary<long, GameItemProto> Prototypes);

	private static RetirementHost PrepareRetirementHost(TestDatabase database, FixtureIds fixture, HarnessClock clock)
	{
		var native = NativeRuntime.Load(fixture, database.ConnectionString, true);
		ConfigureCastingWorld(native, database.ConnectionString, false); PrepareLifecycleRuntime(native);
		var roots = ArchiveRoots(); ConfigureArchiveWorld(native, roots, fixture, database.ConnectionString);
		var scheduler = new Scheduler(clock); var heartbeats = new HeartbeatManager(native.World);
		native.WorldMock.SetupGet(x => x.Scheduler).Returns(scheduler);
		native.WorldMock.SetupGet(x => x.HeartbeatManager).Returns(heartbeats);
		var service = new SpellOwnedNpcService(native.World); var store = new SpellOwnedLifecycleStore();
		native.WorldMock.SetupGet(x => x.SpellOwnedNpcs).Returns(service);
		native.WorldMock.SetupGet(x => x.CharacterArchives).Returns(new CharacterArchiveService());
		native.WorldMock.SetupGet(x => x.DefaultHooks).Returns(Array.Empty<MudSharp.Events.Hooks.IDefaultHook>());
		native.WorldMock.SetupGet(x => x.Currencies).Returns(new All<ICurrency>());
		native.WorldMock.SetupGet(x => x.Accents).Returns(new All<IAccent>());
		native.WorldMock.SetupGet(x => x.ActiveProjects).Returns(new All<IActiveProject>());
		native.WorldMock.Setup(x => x.GetStaticString("MaximumStaminaExpression")).Returns("100");
		Mock.Get(native.Body.Race).SetupGet(x => x.BaseBody).Returns(native.World.BodyPrototypes.First());
		Mock.Get(native.Body.Race).SetupGet(x => x.BreathingStrategy).Returns(new NonBreather());
		var items = new All<IGameItem>(); native.WorldMock.SetupGet(x => x.Items).Returns(items);
		native.WorldMock.Setup(x => x.Add(It.IsAny<IGameItem>())).Callback<IGameItem>(x => { if (!items.Has(x.Id)) items.Add(x); });
		native.WorldMock.Setup(x => x.Destroy(It.IsAny<IGameItem>())).Callback<IGameItem>(x => items.Remove(x));
		native.WorldMock.Setup(x => x.Add(It.IsAny<ICharacter>(), It.IsAny<bool>())).Callback<ICharacter, bool>(roots.Add);
		native.WorldMock.Setup(x => x.Add(It.IsAny<IBody>())).Callback<IBody>(roots.Add);
		var cell = native.Actor.Location; var cellItems = new List<IGameItem>();
		var cellMock = Mock.Get(cell);
		cellMock.SetupGet(x => x.GameItems).Returns(cellItems);
		cellMock.As<ICustodyRollbackLocation>().Setup(x => x.CaptureCustodyMembershipRollback(It.IsAny<IReadOnlyCollection<IGameItem>>()))
			.Returns<IReadOnlyCollection<IGameItem>>(graph =>
			{
				var captured = new HashSet<IGameItem>(graph, ReferenceEqualityComparer.Instance);
				var present = cellItems.Where(captured.Contains).ToArray();
				return () => { cellItems.RemoveAll(captured.Contains); cellItems.AddRange(present); };
			});
		// A held item's Location resolves through its body to this cell. Native inventory
		// reload consults the room's pickup/access policy even after setting HeldBy.
		cellMock.Setup(x => x.CanGet(It.IsAny<IGameItem>(), It.IsAny<ICharacter>())).Returns(true);
		cellMock.Setup(x => x.CanGetAccess(It.IsAny<IGameItem>(), It.IsAny<ICharacter>())).Returns(true);
		cellMock.Setup(x => x.Insert(It.IsAny<IGameItem>(), It.IsAny<bool>())).Callback<IGameItem, bool>((item, _) =>
		{
			ForeignCustodyTransferContext.EnsureCell(cell, item);
			if (!cellItems.Contains(item)) cellItems.Add(item);
			item.Drop(cell);
		});
		cellMock.Setup(x => x.Extract(It.IsAny<IGameItem>())).Callback<IGameItem>(x => cellItems.Remove(x));
		var terrain = Mock.Of<ITerrain>();
		cellMock.SetupGet(x => x.CurrentOverlay).Returns(Mock.Of<ICellOverlay>(x => x.Terrain == terrain));
		var templates = new Mock<IUneditableRevisableAll<INPCTemplate>>();
		templates.Setup(x => x.GetEnumerator()).Returns(() => Enumerable.Empty<INPCTemplate>().GetEnumerator());
		templates.Setup(x => x.Get(It.IsAny<long>(), It.IsAny<int>())).Returns<long, int>((id, _) => Mock.Of<INPCTemplate>(x => x.Id == id));
		native.WorldMock.SetupGet(x => x.NpcTemplates).Returns(templates.Object);
		var componentPrototypes = new Dictionary<long, IGameItemComponentProto>();
		var prototypes = new Dictionary<long, GameItemProto>();
		var componentCatalogue = new Mock<IUneditableRevisableAll<IGameItemComponentProto>>();
		componentCatalogue.Setup(x => x.Get(It.IsAny<long>(), It.IsAny<int>())).Returns<long, int>((id, _) => componentPrototypes.GetValueOrDefault(id)!);
		native.WorldMock.SetupGet(x => x.ItemComponentProtos).Returns(componentCatalogue.Object);
		var catalogue = new Mock<IUneditableRevisableAll<IGameItemProto>>();
		catalogue.Setup(x => x.Get(It.IsAny<long>(), It.IsAny<int>())).Returns<long, int>((id, _) => prototypes.GetValueOrDefault(id)!);
		native.WorldMock.SetupGet(x => x.ItemProtos).Returns(catalogue.Object);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			((All<IHealthStrategy>)native.World.HealthStrategies).Add(BaseHealthStrategy.LoadStrategy(
				db.HealthStrategies.Single(x => x.Name == "ARM03B2B item health"), native.World));
			foreach (var model in db.GameItemComponentProtos.Include(x => x.EditableItem).Where(x => x.Name.StartsWith("ARM03B2B")))
			{
				var type = model.Type switch
				{
					"Corpse" => typeof(CorpseGameItemComponentProto), "Container" => typeof(ContainerGameItemComponentProto),
					"Holdable" => typeof(HoldableGameItemComponentProto), "Belt" => typeof(BeltGameItemComponentProto),
					"Beltable" => typeof(BeltableGameItemComponentProto), "Stackable" => typeof(StackableGameItemComponentProto), "Simple Lock" => typeof(SimpleLockGameItemComponentProto),
					_ => throw new InvalidOperationException("Unknown owned fixture component")
				};
				componentPrototypes.Add(model.Id, (IGameItemComponentProto)type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
					null, [typeof(Db.GameItemComponentProto), typeof(IFuturemud)], null)!.Invoke([model, native.World]));
			}
			foreach (var model in db.GameItemProtos.Include(x => x.EditableItem).Include(x => x.GameItemProtosGameItemComponentProtos).Where(x => x.Name.StartsWith("ARM03B2B")))
				prototypes.Add(model.Id, new GameItemProto(model, native.World));
		}
		if (prototypes.Values.SingleOrDefault(x => x.Name == "ARM03B2B corpse") is { } corpsePrototype)
			CorpseGameItemComponentProto.ItemProto = corpsePrototype;
		StandardCorpseModel.RegisterTypeLoader();
		var modelXml = new XElement("Definition", new XElement("Ranges", new XElement("Range", new XAttribute("state", 0), new XAttribute("lower", 0), new XAttribute("upper", 1000000))),
			new XElement("Terrains", new XAttribute("default", 1)), new XElement("Descriptions",
				new[] { "ShortDescriptions", "FullDescriptions", "ContentsDescriptions", "PartDescriptions" }.Select(group =>
					new XElement(group, new XElement("Description", new XAttribute("state", 0), "a native corpse")))),
			new XElement("CorpseMaterials", new XElement("CorpseMaterial", new XAttribute("state", 0), native.World.Materials.First().Id)));
		var corpseModel = CorpseModelFactory.LoadCorpseModel(new Db.CorpseModel { Id = 71, Type = "Standard", Name = "ARM03B2B native decay", Definition = modelXml.ToString() }, native.World);
		Mock.Get(native.Body.Race).SetupGet(x => x.CorpseModel).Returns(corpseModel);
		var models = new All<ICorpseModel>(); models.Add(corpseModel); native.WorldMock.SetupGet(x => x.CorpseModels).Returns(models);
		native.WorldMock.Setup(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>())).Returns<long, bool>((id, _) =>
		{
			if (id == fixture.CharacterId) return native.Actor;
			var loaded = roots.Actors.Concat(roots.CachedActors).FirstOrDefault(x => x.Id == id);
			if (loaded is not null) return loaded;
			using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
			var identity = FMDB.Context.Characters.Include(x => x.Body).SingleOrDefault(x => x.Id == id && !x.IsArchived);
			var npcModel = FMDB.Context.Npcs.SingleOrDefault(x => x.CharacterId == id);
			if (identity is null || npcModel is null) return null!;
			var npc = new RuntimeNpc(npcModel, identity, native.World);
			roots.Add(npc, true); roots.Add(npc.Body); npc.SetupEventSubscriptions(); return npc;
		});
		native.WorldMock.Setup(x => x.TryGetItem(It.IsAny<long>(), It.IsAny<bool>())).Returns<long, bool>((id, _) =>
		{
			if (items.Get(id) is { } loaded) return loaded;
			using var scope = FMDB.BeginIndependentScope(); using var db = new FMDB();
			var row = FMDB.Context.GameItems.SingleOrDefault(x => x.Id == id);
			if (row is null) return null!;
			var item = new GameItem(row, native.World); items.Add(item); return item;
		});
		return new(native, roots, service, store, scheduler, heartbeats, items, prototypes);
	}

	private static void SeedRetirementPrototypes(TestDatabase database, long material)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var componentId = db.GameItemComponentProtos.Max(x => (long?)x.Id) ?? 0;
		var prototypeId = db.GameItemProtos.Max(x => (long?)x.Id) ?? 0;
		db.HealthStrategies.Add(new() { Name = "ARM03B2B item health", Type = "GameItem",
			Definition = "<Definition><LodgeDamageExpression>0</LodgeDamageExpression></Definition>" });
		db.SaveChanges();
		Db.GameItemComponentProto Component(string type, string definition)
		{
			var component = new Db.GameItemComponentProto { Id = ++componentId, Name = "ARM03B2B " + type, Type = type, Description = type,
				Definition = definition, EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
			db.GameItemComponentProtos.Add(component); db.SaveChanges(); return component;
		}
		var hold = Component("Holdable", "<Definition/>");
		var corpse = Component("Corpse", "<Definition/>");
		var container = Component("Container", "<Definition Weight='1000' MaxSize='3' Preposition='in' Closable='false' Transparent='true' OnceOnly='false'/>");
		var belt = Component("Belt", "<Definition MaximumNumberOfBeltedItems='4' MaximumSize='3'/>");
		var beltable = Component("Beltable", "<Definition/>");
		var stackable = Component("Stackable", "<Definition Decorator='0'/>");
		var lockXml = new XElement("Definition", new XElement("ForceDifficulty", 5), new XElement("PickDifficulty", 5), new XElement("LockType", "fixture"),
			new[] { "LockEmote", "UnlockEmote", "LockEmoteNoActor", "UnlockEmoteNoActor", "LockEmoteOtherSide", "UnlockEmoteOtherSide" }.Select(name => new XElement(name, "$0 clicks.")));
		var simpleLock = Component("Simple Lock", lockXml.ToString());
		void Prototype(string name, int morph, params Db.GameItemComponentProto[] components)
		{
			var proto = new Db.GameItemProto { Id = ++prototypeId, Name = "ARM03B2B " + name, Keywords = name, ShortDescription = "a " + name, FullDescription = "Native foreign fixture",
				MaterialId = material, Size = 1, Weight = 1, BaseItemQuality = (int)ItemQuality.Standard, MorphTimeSeconds = morph, MorphEmote = "$0 decays.",
				EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
			foreach (var component in components) proto.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component.Id });
			db.GameItemProtos.Add(proto); db.SaveChanges();
		}
		Prototype("corpse", 240, corpse); Prototype("bag", 0, hold, container); Prototype("belt", 0, hold, belt);
		Prototype("goods", 0, hold, beltable); Prototype("stack", 0, hold, stackable); Prototype("lock", 0, hold, simpleLock);
	}

	private static int RunSpellOwnedNpcRetirementChecks()
	{
		var p2Probe = Environment.GetEnvironmentVariable("FUTUREMUD_RETIREMENT_P2_PROBE") ?? string.Empty;
		using var database = TestDatabase.CreateFresh("futuremud_land_"); ConfigureNativeDatabase(database.ConnectionString);
		Console.WriteLine($"ARM03B2B-created={database.Name}");
		var fixture = FixtureSeed.Create(database, "arm03b2b_retirement", true);
		var clock = new HarnessClock(); using var time = RuntimeClock.Push(clock);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			db.Database.Migrate();
			foreach (var sql in new[] { "ALTER TABLE Characters AUTO_INCREMENT=1000000", "ALTER TABLE Bodies AUTO_INCREMENT=2000000", "ALTER TABLE CharacterInstances AUTO_INCREMENT=3000000" }) db.Database.ExecuteSqlRaw(sql);
		}
		var seedNative = NativeRuntime.Load(fixture, database.ConnectionString, true);
		ConfigureCastingWorld(seedNative, database.ConnectionString, true);
		SeedRetirementPrototypes(database, seedNative.World.Materials.First().Id);
		var host = PrepareRetirementHost(database, fixture, clock); var native = host.Native;
		var data = new SimpleCharacterTemplate
		{
			Gameworld = native.World, SelectedName = new PersonalName(new XElement("Name", new XAttribute("culture", 1), new XElement("Element", new XAttribute("usage", "BirthName"), "Retiring Guardian")), native.World),
			SelectedRace = native.Body.Race, SelectedEthnicity = native.Body.Ethnicity, SelectedCulture = native.Actor.Culture,
			SelectedBirthday = native.World.Calendars.First().GetDate("1-month-2000"), SelectedStartingLocation = native.Actor.Location,
			SelectedGender = native.Body.Gender.Enum, SelectedHeight = 1.8, SelectedWeight = 80, SelectedSdesc = "a retiring guardian", SelectedFullDesc = "A native guardian.",
			SelectedAccents = [], SelectedAttributes = [], SelectedCharacteristics = [], SelectedEntityDescriptionPatterns = [], SkillValues = [], SelectedRoles = [], SelectedMerits = [],
			SelectedKnowledges = [], MissingBodyparts = [], SelectedDisfigurements = [], SelectedProstheses = []
		};
		var template = new SimpleNPCTemplate(native.World, DummyAccount.Instance, data, "ARM03B2B guardian");
		var spell = new MagicSpell("ARM03B2B retirement", native.Capability.School);
		SpellLifecycleOrigin Origin(SpellLifecycleMode mode) => new(Guid.NewGuid(), spell.Id, 3, fixture.CharacterId, "native-retirement", mode,
			RuntimeClock.UtcNow, mode == SpellLifecycleMode.Permanent ? null : RuntimeClock.UtcNow.AddMinutes(1), "ARM03B2B actual native lifecycle fixture");
		RuntimeNpc Create(SpellLifecycleMode mode)
		{
			var npc = (RuntimeNpc)template.CreateSpellOwnedCharacter(RouteSpatialService.Instance.GetEffectiveLocation(native.Actor), Origin(mode));
			native.World.Add(npc, true); native.World.Add(npc.Body);
			return npc;
		}
		SpellOwnedLifecycle Life(RuntimeNpc npc) => host.Store.Find(ReadNativeNpcLifecycle(database, npc.Id))!;
		void Completed(RuntimeNpc npc)
		{
			var life = Life(npc); using var db = NewIndependentContext(database.ConnectionString);
			Require(life.State == SpellLifecycleState.Completed && life.DeathObservedUtc is not null && db.Characters.Find(npc.Id) is { IsArchived: true, BodyId: null } &&
				!db.Bodies.Any(x => x.Id == npc.Body.Id) && !db.Npcs.Any(x => x.CharacterId == npc.Id) && !db.CharacterInstances.Any(x => x.CharacterId == npc.Id) &&
				!host.Roots.Actors.Has(npc.Id) && !host.Roots.CachedActors.Has(npc.Id) && !host.Roots.NPCs.Has(npc.Id) && !host.Roots.Bodies.Has(npc.Body.Id),
				"Native completion did not retire the exact heavy graph: " + life.Diagnostic);
		}
		var permanent = Create(SpellLifecycleMode.Permanent); var temporary = Create(SpellLifecycleMode.TemporaryCleanup); var expiry = Create(SpellLifecycleMode.DeathOnExpiry);
		var deaths = new Dictionary<long, int>(); foreach (var npc in new[] { permanent, temporary, expiry }) { deaths[npc.Id] = 0; npc.OnDeath += _ => deaths[npc.Id]++; }
		Require(host.Service.ReconcileRetirements(RuntimeClock.UtcNow) == 0, "A native lifetime expired early.");
		clock.Advance(TimeSpan.FromSeconds(61)); host.Service.ReconcileRetirements(RuntimeClock.UtcNow);
		Completed(temporary); var expiryLife = Life(expiry); var expiryCorpse = expiry.Corpse!.Parent;
		Require(deaths[temporary.Id] == 1 && temporary.Corpse is null && deaths[expiry.Id] == 1 && expiryLife.State == SpellLifecycleState.RemainsPending && expiryLife.RemainsItemId == expiryCorpse.Id &&
			Life(permanent).State == SpellLifecycleState.Completed && !permanent.State.HasFlag(CharacterState.Dead) && deaths[permanent.Id] == 0,
			"Same-template permanent, dissipation and native death-on-expiry modes diverged from their policy.");
		expiryCorpse.Delete(); host.Service.ReconcileRetirements(RuntimeClock.UtcNow); Completed(expiry);
		host.Service.ReconcileRetirements(RuntimeClock.UtcNow); Require(deaths[expiry.Id] == 1 && deaths[temporary.Id] == 1, "Retry repeated native death.");
		Console.WriteLine("ARM03B2B-three-modes=passed actual-same-SimpleNPCTemplate permanent-durable-no-expiry default-no-corpse-dissipation death-on-expiry-native-corpse actual-GameItem-Delete actual-Character-Quit archival one-terminal-death-each no-replay");
		var suppressed = Create(SpellLifecycleMode.DeathOnExpiry); var suppressedDeaths = 0; suppressed.OnDeath += _ => suppressedDeaths++;
		clock.Advance(TimeSpan.FromSeconds(61)); var suppressedLife = Life(suppressed);
		suppressedLife = host.Store.BeginRetirement(suppressedLife.Origin.Id, suppressedLife.Version, SpellRetirementReason.Expiry, RuntimeClock.UtcNow);
		var suppressedCensus = RetirementRuntimeCensus(host);
		var actorQueued = native.World.SaveManager.IsQueued(suppressed); var bodyQueued = native.World.SaveManager.IsQueued(suppressed.Body);
		using (FMDB.BeginIsolatedScope(suppressEfWrites: true))
		{
			var caller = FMDB.Context; var refused = false;
			try { host.Service.ReconcileRetirements(RuntimeClock.UtcNow); }
			catch (InvalidOperationException ex) when (ex.Message.Contains("suppresses database writes")) { refused = true; }
			Require(refused && ReferenceEquals(caller, FMDB.Context) && FMDB.WritesAreSuppressed,
				"Suppressed retirement did not refuse before native work or retain its caller scope.");
		}
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(!suppressed.State.HasFlag(CharacterState.Dead) && suppressed.Corpse is null && suppressedDeaths == 0 &&
				db.Characters.Find(suppressed.Id)!.DeathTime is null && Life(suppressed).Version == suppressedLife.Version &&
				RetirementRuntimeCensus(host) == suppressedCensus && native.World.SaveManager.IsQueued(suppressed) == actorQueued &&
				native.World.SaveManager.IsQueued(suppressed.Body) == bodyQueued,
				"Read-only retirement changed live state, death events, custody, queues, subscriptions or durable intent.");
		host.Service.ReconcileRetirements(RuntimeClock.UtcNow); suppressed.Corpse!.Parent.Delete(); host.Service.ReconcileRetirements(RuntimeClock.UtcNow); Completed(suppressed);
		Require(suppressedDeaths == 1, "Writable retry did not perform exactly one native death.");
		Console.WriteLine("ARM03B2B-suppressed-retirement=passed persisted-Retiring-live-DeathOnExpiry read-only-refusal-before-native-Die-or-loading caller-scope-state-events-corpse-queues-roots-timers-and-subscriptions-unchanged writable-retry-one-death");

		var controlled = Create(SpellLifecycleMode.DeathOnExpiry); var controlledDeaths = 0; controlled.OnDeath += _ => controlledDeaths++;
		var nativeController = controlled.CharacterController;
		typeof(MudSharp.Character.Character).GetProperty(nameof(MudSharp.Character.Character.Controller))!.SetValue(controlled, Mock.Of<ICharacterController>());
		clock.Advance(TimeSpan.FromSeconds(61)); host.Service.ReconcileRetirements(RuntimeClock.UtcNow, 1);
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(controlledDeaths == 0 && controlled.Corpse is null && !controlled.State.HasFlag(CharacterState.Dead) && db.Characters.Find(controlled.Id)!.DeathTime is null && Life(controlled).Diagnostic.Contains("Connected controllers"),
				"An unadapted foreign controller reached native expiry death before its dependency hold.");
		var heldVersion = Life(controlled).Version;
		clock.Advance(TimeSpan.FromSeconds(1));
		var laterEligible = Create(SpellLifecycleMode.TemporaryCleanup);
		Require(Life(laterEligible).UpdatedUtc > Life(controlled).UpdatedUtc, "Fairness regression requires a strictly older stable hold.");
		clock.Advance(TimeSpan.FromSeconds(61)); host.Service.ReconcileRetirements(RuntimeClock.UtcNow, 1); Completed(laterEligible);
		Require(!controlled.State.HasFlag(CharacterState.Dead) && Life(controlled).Version == heldVersion,
			"A stable held row was changed to make bounded processing appear fair.");
		Console.WriteLine("ARM03B2B-fair-batches=passed limit-one stable-controller-hold-preserved later-eligible-native-expiry-completed-on-next-bounded-pass no-hold-starvation");
		typeof(MudSharp.Character.Character).GetProperty(nameof(MudSharp.Character.Character.Controller))!.SetValue(controlled, nativeController);
		host.Service.ReconcileRetirements(RuntimeClock.UtcNow); controlled.Corpse!.Parent.Delete(); host.Service.ReconcileRetirements(RuntimeClock.UtcNow); Completed(controlled);
		Require(controlledDeaths == 1, "Controller release did not resume exactly one native death.");
		Console.WriteLine("ARM03B2B-controller-hold=passed expired-real-native-NPC foreign-controller-refused-before-Die-or-evacuation persisted-live-state-and-zero-corpse-conserved release-resumes-exactly-one-death-and-retirement");

		var early = Create(SpellLifecycleMode.DeathOnExpiry); var earlyDeaths = 0; early.OnDeath += _ => earlyDeaths++;
		IGameItem New(string name) { var item = host.Prototypes.Values.Single(x => x.Name == "ARM03B2B " + name).CreateNew(); native.World.SaveManager.Flush(); native.World.Add(item); return item; }
		var bag = New("bag"); var belt = New("belt"); var goods = New("goods"); var installedLock = New("lock");
		bag.GetItemType<IContainer>()!.Put(null, belt, allowMerge: false);
		bag.GetItemType<ILockable>()!.InstallLock(installedLock.GetItemType<ILock>()!);
		belt.GetItemType<IBelt>()!.AddConnectedItem(goods.GetItemType<IBeltable>()!);
		early.Body.Get(bag, silent: true);
		Require(early.Body.AllItems.Contains(bag), "Native body refused foreign bag fixture."); native.World.SaveManager.Flush();
		var foreign = new[] { bag.Id, belt.Id, goods.Id, installedLock.Id };
		var corpse = early.Die()!; native.World.SaveManager.Flush();
		using (var db = NewIndependentContext(database.ConnectionString)) { db.CellsGameItems.Add(new() { CellId = fixture.CellId, GameItemId = corpse.Id }); db.SaveChanges(); }
		var earlyLife = Life(early); clock.Advance(TimeSpan.FromSeconds(61)); host.Service.ReconcileRetirements(RuntimeClock.UtcNow);
		Require(earlyDeaths == 1 && !corpse.Deleted && Life(early).RemainsItemId == corpse.Id && early.Body.AllItems.Any(x => x.Id == bag.Id), "Original expiry killed again or discarded configured remains/custody.");
		native.World.SaveManager.Flush();
		// Exit the old controlled host before the reader reloads the graph. Ordinary Quit
		// retains persisted custody and stops its timers; process exit drops the host's caches.
		corpse.Quit(); early.Quit(silent: true); native.World.SaveManager.Flush();
		((All<ICharacter>)host.Roots.CachedActors).Remove(early); ((All<ICharacter>)host.Roots.Actors).Remove(early);
		((All<ICharacter>)host.Roots.NPCs).Remove(early); ((All<IBody>)host.Roots.Bodies).Remove(early.Body);
		foreach (var item in host.Items.Where(x => foreign.Contains(x.Id) || x.Id == corpse.Id).ToArray()) host.Items.Remove(item);
		RunOwnedRetirementReader(new(database.Name, fixture, earlyLife.Origin.Id, RuntimeClock.UtcNow, corpse.Id, foreign, "decay"));
		host.Service.ReconcileRetirements(RuntimeClock.UtcNow); Completed(early);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(earlyDeaths == 1 && foreign.All(id => db.GameItems.Any(x => x.Id == id)) && db.CellsGameItems.Count(x => x.GameItemId == bag.Id && x.CellId == fixture.CellId) == 1 &&
				!db.BodiesGameItems.Any(x => x.BodyId == early.Body.Id) && !db.GameItems.Any(x => x.Id == corpse.Id), "Restart retirement lost or duplicated foreign custody.");
		}
		Console.WriteLine("ARM03B2B-early-death=passed actual-native-NPC-Die configured-remains-kept-across-original-expiry native-Quit-old-host-cache-exit separate-process-native-body-and-items-reload positive-corpse-minute-decay actual-scheduled-morph-to-nothing-and-Delete nested-container-installed-lock-belt-attachment-conserved exact-IDs one-OnDeath");

		var earlyDefault = Create(SpellLifecycleMode.TemporaryCleanup); var defaultForeign = New("goods");
		earlyDefault.Body.Get(defaultForeign, silent: true); native.World.SaveManager.Flush();
		var defaultDeaths = 0; earlyDefault.OnDeath += _ => defaultDeaths++;
		Require(earlyDefault.Die() is null, "Default early-dead temporary creation produced valuable ordinary remains.");
		host.Service.ReconcileRetirements(RuntimeClock.UtcNow); Completed(earlyDefault);
		clock.Advance(TimeSpan.FromSeconds(61)); host.Service.ReconcileRetirements(RuntimeClock.UtcNow);
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(defaultDeaths == 1 && db.GameItems.Any(x => x.Id == defaultForeign.Id) && db.CellsGameItems.Count(x => x.GameItemId == defaultForeign.Id) == 1,
				"Default early-death/expiry replay lost foreign goods or repeated death.");
		Console.WriteLine("ARM03B2B-default-early-death=passed actual-native-Die suppressed-corpse foreign-item-conserved before-original-expiry later-deadline-no-redeath-or-recreation-or-removal");
		var noDestination = Create(SpellLifecycleMode.TemporaryCleanup); var heldForeign = New("goods");
		noDestination.Body.Get(heldForeign, silent: true); native.World.SaveManager.Flush();
		var noDestinationDeaths = 0; noDestination.OnDeath += _ => noDestinationDeaths++;
		var locationProperty = typeof(MudSharp.Character.Character).GetProperty(nameof(MudSharp.Character.Character.Location))!;
		locationProperty.SetValue(noDestination, null);
		clock.Advance(TimeSpan.FromSeconds(61)); host.Service.ReconcileRetirements(RuntimeClock.UtcNow);
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(noDestinationDeaths == 0 && !noDestination.State.HasFlag(CharacterState.Dead) && noDestination.Body.AllItems.Contains(heldForeign) &&
				Life(noDestination).Diagnostic.Contains("no validated persisted destination") && db.BodiesGameItems.Any(x => x.BodyId == noDestination.Body.Id && x.GameItemId == heldForeign.Id) &&
				!db.CellsGameItems.Any(x => x.GameItemId == heldForeign.Id) && !heldForeign.Deleted,
				"Missing evacuation destination changed native death, foreign custody or recoverability.");
		locationProperty.SetValue(noDestination, native.Actor.Location);
		host.Service.ReconcileRetirements(RuntimeClock.UtcNow); Completed(noDestination);
		Require(noDestinationDeaths == 1 && heldForeign.Location == native.Actor.Location && !heldForeign.Deleted, "A restored safe destination failed to resume conserved native retirement.");
		Console.WriteLine("ARM03B2B-destination-hold=passed missing-safe-destination retains-live-owned-body-and-exact-persisted-foreign-custody before-Die-or-transfer explicit-location-recovery resumes-one-death-and-conserved-retirement");
		foreach (var restartRemoval in new[] { false, true })
		{
			var removalNpc = Create(SpellLifecycleMode.DeathOnExpiry); var removalDeaths = 0; removalNpc.OnDeath += _ => removalDeaths++;
			var removalForeign = New("goods"); removalNpc.Body.Get(removalForeign, silent: true); native.World.SaveManager.Flush();
			var removalCorpse = removalNpc.Die()!; var removalNotifications = 0; removalCorpse.OnDeleted += _ => removalNotifications++;
			var refusalSql = "CREATE TRIGGER arm03b2c_delete_refusal BEFORE DELETE ON GameItems FOR EACH ROW BEGIN IF OLD.Id=" + removalCorpse.Id.ToString(System.Globalization.CultureInfo.InvariantCulture) + " THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='ARM03B2C final corpse deletion refusal'; END IF; END";
			using (var db = NewIndependentContext(database.ConnectionString)) db.Database.ExecuteSqlRaw(refusalSql);
			var removalRefused = false;
			try { removalCorpse.Delete(); }
			catch (DbUpdateException) { removalRefused = true; }
			finally { using var db = NewIndependentContext(database.ConnectionString); db.Database.ExecuteSqlRaw("DROP TRIGGER arm03b2c_delete_refusal"); }
			using (var db = NewIndependentContext(database.ConnectionString))
				Require(removalRefused && db.GameItems.Any(x => x.Id == removalCorpse.Id) && removalDeaths == 1 && removalNotifications == 1 &&
					db.GameItems.Any(x => x.Id == removalForeign.Id), "Final corpse DELETE refusal must leave its persisted row and conserve foreign possessions.");
			native.World.SaveManager.Flush();
			var pendingRemoval = Life(removalNpc);
			Require(pendingRemoval.RemainsRemovalRequestedUtc is not null && pendingRemoval.RemainsNotificationAttemptedUtc is not null &&
				pendingRemoval.RemainsNotificationCompletedUtc is not null && pendingRemoval.RemainsItemId == removalCorpse.Id,
				"Successful removal admission did not durably retain exact intent and completed observer progress after provider refusal.");
			if (restartRemoval)
			{
				// A real restart drops the old host. Exit its exact native graph normally
				// after the ordinary flush, before the new process completes the journal.
				removalNpc.Quit(silent: true); native.World.SaveManager.Flush();
				((All<ICharacter>)host.Roots.CachedActors).Remove(removalNpc); ((All<ICharacter>)host.Roots.Actors).Remove(removalNpc);
				((All<ICharacter>)host.Roots.NPCs).Remove(removalNpc); ((All<IBody>)host.Roots.Bodies).Remove(removalNpc.Body);
				removalForeign.Quit(); host.Items.Remove(removalForeign);
				RunOwnedRetirementReader(new(database.Name, fixture, pendingRemoval.Origin.Id, RuntimeClock.UtcNow, removalCorpse.Id, [removalForeign.Id], "removal"));
			}
			host.Service.ReconcileRetirements(RuntimeClock.UtcNow);
			using (var db = NewIndependentContext(database.ConnectionString))
				Console.WriteLine($"ARM03B2C-removal-observed=state:{Life(removalNpc).State} remains-row:{db.GameItems.Any(x => x.Id == removalCorpse.Id)} deaths:{removalDeaths} callbacks:{removalNotifications} diagnostic:{Life(removalNpc).Diagnostic}");
			Completed(removalNpc);
			Require(removalDeaths == 1 && removalNotifications == 1, "Removal retry repeated native death or deletion notification.");
			using (var db = NewIndependentContext(database.ConnectionString))
				Require(db.GameItems.Any(x => x.Id == removalForeign.Id) && db.CellsGameItems.Count(x => x.GameItemId == removalForeign.Id) == 1 &&
					!db.BodiesGameItems.Any(x => x.GameItemId == removalForeign.Id) && !removalForeign.Deleted,
					"Durable removal retry lost or duplicated exact foreign custody.");
			Console.WriteLine("ARM03B2C-removal-provider-" + (restartRemoval ? "restart" : "same-process") + "=passed final-GameItems-DELETE-provider-refusal ordinary-save-flush exact-durable-removal-and-observer-journal native-reconciliation exact-foreign-conservation no-death-or-callback-replay");
		}

		if (p2Probe == "Removal") return 0;

		var topologyNpc = Create(SpellLifecycleMode.TemporaryCleanup);
		var outerBag = New("bag"); var innerBag = New("bag"); var nestedGoods = New("goods");
		var outerContainer = outerBag.GetItemType<IContainer>()!; var innerContainer = innerBag.GetItemType<IContainer>()!;
		innerContainer.Put(null, nestedGoods, allowMerge: false); outerContainer.Put(null, innerBag, allowMerge: false);
		topologyNpc.Body.Get(outerBag, silent: true); topologyNpc.PositionState = PositionStanding.Instance; native.World.SaveManager.Flush();
		var topologyIds = new[] { outerBag.Id, innerBag.Id, nestedGoods.Id };
		Dictionary<long, string> originalDefinitions;
		using (var db = NewIndependentContext(database.ConnectionString)) originalDefinitions = db.GameItemComponents
			.Where(x => topologyIds.Contains(x.GameItemId)).ToDictionary(x => x.Id, x => x.Definition);
		var topologyDeaths = 0; var topologyCallbacks = 0; var callbackClearedFlags = false;
		topologyNpc.OnDeath += _ => topologyDeaths++;
		InventoryChangeEvent reparent = (_, _, item) =>
		{
			if (!ReferenceEquals(item, outerBag) || topologyCallbacks != 0) return;
			topologyCallbacks++;
			topologyNpc.PositionState = PositionSprawled.Instance;
			innerContainer.Take(null!, nestedGoods, 0); outerContainer.Put(null, nestedGoods, allowMerge: false);
			// Exercise a callback which explicitly saves and clears both dirty flags inside
			// the existing evacuation transaction. Comparing item IDs or flags alone misses it.
			using var db = new FMDB(); innerContainer.Save(); outerContainer.Save(); FMDB.Context.SaveChanges();
			callbackClearedFlags = !innerContainer.Changed && !outerContainer.Changed;
		};
		topologyNpc.Body.OnInventoryChange += reparent;
		clock.Advance(TimeSpan.FromSeconds(61)); host.Service.ReconcileRetirements(RuntimeClock.UtcNow);
		topologyNpc.Body.OnInventoryChange -= reparent;
		Require(SpellOwnedNpcService.TryCaptureForeignCustody(topologyNpc.Body, out _, out var movedGraph, out _, [outerBag]) &&
			movedGraph.Select(x => x.Id).Order().SequenceEqual(topologyIds.Order()), "Topology regression must conserve the same captured item identity set.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var persisted = db.GameItemComponents.Where(x => topologyIds.Contains(x.GameItemId)).ToDictionary(x => x.Id, x => x.Definition);
			Console.WriteLine($"ARM03B2B-topology-observed=state:{Life(topologyNpc).State} deaths:{topologyDeaths} callbacks:{topologyCallbacks} callback-cleared-flags:{callbackClearedFlags} body-joins:{db.BodiesGameItems.Count(x => x.BodyId == topologyNpc.Body.Id)} cell-joins:{db.CellsGameItems.Count(x => x.GameItemId == outerBag.Id)} original-XML:{originalDefinitions.All(x => persisted.GetValueOrDefault(x.Key) == x.Value)}");
			Require(topologyCallbacks == 1 && callbackClearedFlags && topologyDeaths == 0 && !topologyNpc.State.HasFlag(CharacterState.Dead) &&
				Life(topologyNpc).State == SpellLifecycleState.Retiring && Life(topologyNpc).Diagnostic.Contains("callback changed custody") &&
				db.BodiesGameItems.Any(x => x.BodyId == topologyNpc.Body.Id && x.GameItemId == outerBag.Id) &&
				!db.CellsGameItems.Any(x => x.GameItemId == outerBag.Id) && db.GameItems.Find(innerBag.Id)!.ContainerId == outerBag.Id &&
				db.GameItems.Find(nestedGoods.Id)!.ContainerId == innerBag.Id && originalDefinitions.Count == persisted.Count &&
				originalDefinitions.All(x => persisted.GetValueOrDefault(x.Key) == x.Value) && innerContainer.Changed && outerContainer.Changed,
				"Same-ID native reparenting committed partial custody, entered death or lost its recoverable structural hold.");
		}
		native.World.SaveManager.Flush();
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var persisted = db.GameItemComponents.Where(x => topologyIds.Contains(x.GameItemId)).ToDictionary(x => x.Id, x => x.Definition);
			Console.WriteLine($"ARM03B2C-custody-flush-observed=body-joins:{db.BodiesGameItems.Count(x => x.BodyId == topologyNpc.Body.Id)} cell-joins:{db.CellsGameItems.Count(x => x.GameItemId == outerBag.Id)} original-XML:{originalDefinitions.All(x => persisted.GetValueOrDefault(x.Key) == x.Value)} runtime-original-child:{innerContainer.Contents.Contains(nestedGoods)}");
			Require(db.BodiesGameItems.Any(x => x.BodyId == topologyNpc.Body.Id && x.GameItemId == outerBag.Id) &&
				!db.CellsGameItems.Any(x => x.GameItemId == outerBag.Id) && db.GameItems.Find(nestedGoods.Id)!.ContainerId == innerBag.Id &&
				originalDefinitions.All(x => persisted.GetValueOrDefault(x.Key) == x.Value) && innerContainer.Contents.Contains(nestedGoods) && topologyNpc.PositionState == PositionStanding.Instance && topologyNpc.Body.PositionState == PositionStanding.Instance &&
				db.Bodies.Find(topologyNpc.Body.Id)!.Position == PositionStanding.Instance.Id,
				"Ordinary save flush persisted rejected foreign topology or runtime custody was not restored.");
		}
		if (p2Probe == "Custody") return 0;
		RunOwnedRetirementReader(new(database.Name, fixture, Life(topologyNpc).Origin.Id, RuntimeClock.UtcNow, 0, topologyIds, "topology"));
		host.Service.ReconcileRetirements(RuntimeClock.UtcNow); Completed(topologyNpc);
		Require(topologyDeaths == 1 && topologyCallbacks == 1, "Recovered same-process custody failed one native death or replayed a topology callback.");
		Console.WriteLine("ARM03B2B-topology-callback=passed actual-Body-OnInventoryChange reparents-same-ID-child-between-native-containers explicit-component-Save-clears-flags typed-edges-refuse-before-custody-commit-or-Die runtime-native-fields-and-XML-restored ordinary-save-flush-before-separate-process-original-topology-reload no-manual-queue-dropping same-process-conserved-retry-one-death");

		foreach (var mutation in new[] { "outside-acquisition", "child-delete", "partial-stack", "rollback-failure" })
		{
			var guardedNpc = Create(SpellLifecycleMode.TemporaryCleanup); var guardedBag = New("bag"); var guardedChild = New(mutation == "partial-stack" ? "stack" : "goods");
			if (mutation == "partial-stack") guardedChild.GetItemType<IStackable>()!.Quantity = 7;
			var roomGoods = New("goods"); var guardedContainer = guardedBag.GetItemType<IContainer>()!;
			guardedContainer.Put(null, guardedChild, allowMerge: false); guardedNpc.Body.Get(guardedBag, silent: true);
			roomGoods.InsertAtSpatialLocation(RouteSpatialService.Instance.GetEffectiveLocation(native.Actor), newStack: true);
			native.World.SaveManager.Flush();
			// The controlled cell host has no EF Cell.Save. Persist its exact room fixture
			// join explicitly before attempting the callback acquisition.
			using (var db = NewIndependentContext(database.ConnectionString))
			{
				db.CellsGameItems.Add(new() { CellId = fixture.CellId, GameItemId = roomGoods.Id }); db.SaveChanges();
				Require(db.CellsGameItems.Count(x => x.GameItemId == roomGoods.Id) == 1, "External room goods fixture was not persisted before the guarded callback.");
			}
			var guardedIds = new[] { guardedBag.Id, guardedChild.Id };
			Dictionary<long, string> guardedDefinitions;
			using (var db = NewIndependentContext(database.ConnectionString)) guardedDefinitions = db.GameItemComponents
				.Where(x => guardedIds.Contains(x.GameItemId)).ToDictionary(x => x.Id, x => x.Definition);
			int originalItemRows; using (var db = NewIndependentContext(database.ConnectionString)) originalItemRows = db.GameItems.Count();
			var guardedDeaths = 0; var guardedCallbacks = 0; var childNotifications = 0;
			guardedNpc.OnDeath += _ => guardedDeaths++; guardedChild.OnDeleted += _ => childNotifications++;
			InventoryChangeEvent mutationCallback = (_, _, item) =>
			{
				if (!ReferenceEquals(item, guardedBag)) return;
				guardedCallbacks++;
				if (mutation == "outside-acquisition") guardedNpc.Body.Get(roomGoods, silent: true);
				else if (mutation == "child-delete") guardedChild.Delete();
				else if (mutation == "partial-stack") guardedChild.Get(guardedNpc.Body, 2);
				else
				{
					// Closing this exact owned provider connection makes explicit rollback fail.
					// The native restore must still run before ordinary save/gameplay resumes.
					using var db = new FMDB(); FMDB.Context.Database.GetDbConnection().Close();
					throw new InvalidOperationException("ARM03B2C deliberate owned transaction connection closure");
				}
			};
			guardedNpc.Body.OnInventoryChange += mutationCallback;
			clock.Advance(TimeSpan.FromSeconds(61)); host.Service.ReconcileRetirements(RuntimeClock.UtcNow);
			guardedNpc.Body.OnInventoryChange -= mutationCallback;
			native.World.SaveManager.Flush();
			using (var db = NewIndependentContext(database.ConnectionString))
			{
				var persisted = db.GameItemComponents.Where(x => guardedIds.Contains(x.GameItemId)).ToDictionary(x => x.Id, x => x.Definition);
				Console.WriteLine($"ARM03B2C-guard-observed={mutation} callbacks:{guardedCallbacks} deaths:{guardedDeaths} child-notifications:{childNotifications} root-held:{guardedNpc.Body.AllItems.Contains(guardedBag)} child-contained:{guardedContainer.Contents.Contains(guardedChild)} external-held:{roomGoods.InInventoryOf is not null} root-body-joins:{db.BodiesGameItems.Count(x => x.GameItemId == guardedBag.Id)} root-cell-joins:{db.CellsGameItems.Count(x => x.GameItemId == guardedBag.Id)} external-cell-joins:{db.CellsGameItems.Count(x => x.GameItemId == roomGoods.Id)} XML-original:{guardedDefinitions.All(x => persisted.GetValueOrDefault(x.Key) == x.Value)} item-rows:{db.GameItems.Count()} original-rows:{originalItemRows}");
				Require(guardedCallbacks == 1 && guardedDeaths == 0 && childNotifications == 0 && !guardedChild.Deleted &&
					guardedNpc.Body.AllItems.Contains(guardedBag) && guardedContainer.Contents.Contains(guardedChild) &&
					ReferenceEquals(guardedChild.ContainedIn, guardedBag) && roomGoods.InInventoryOf is null && roomGoods.Location == native.Actor.Location &&
					db.BodiesGameItems.Count(x => x.BodyId == guardedNpc.Body.Id && x.GameItemId == guardedBag.Id) == 1 &&
					!db.CellsGameItems.Any(x => x.GameItemId == guardedBag.Id) && db.CellsGameItems.Count(x => x.GameItemId == roomGoods.Id) == 1 &&
					!db.BodiesGameItems.Any(x => x.GameItemId == roomGoods.Id) && db.GameItems.Find(guardedChild.Id)!.ContainerId == guardedBag.Id &&
					guardedDefinitions.All(x => persisted.GetValueOrDefault(x.Key) == x.Value) &&
					(mutation != "partial-stack" || guardedChild.Quantity == 7 && db.GameItems.Count() == originalItemRows),
					"Guarded callback or provider rollback failure lost usable native custody after ordinary flush: " + mutation + ": " + Life(guardedNpc).Diagnostic);
			}
			RunOwnedRetirementReader(new(database.Name, fixture, Life(guardedNpc).Origin.Id, RuntimeClock.UtcNow, 0, guardedIds, "guard-custody", roomGoods.Id));
			host.Service.ReconcileRetirements(RuntimeClock.UtcNow); Completed(guardedNpc);
			Require(guardedDeaths == 1 && guardedCallbacks == 1 && childNotifications == 0 && !guardedChild.Deleted && !roomGoods.Deleted,
				"Ordinary native retry repeated a callback or lost guarded foreign items.");
			Console.WriteLine("ARM03B2C-custody-" + mutation + "=passed actual-inventory-callback original-runtime-and-persisted-custody-conserved ordinary-flush separate-process-reload same-process-retry-one-death no-foreign-delete-or-callback-replay");
		}

		var callbackNpc = Create(SpellLifecycleMode.DeathOnExpiry); var callbackCorpse = callbackNpc.Die()!;
		IGameItem? callbackForeign = null; var callbackCount = 0;
		callbackCorpse.OnQuit += _ => throw new InvalidOperationException("Held removal sent native quit.");
		callbackCorpse.OnDeleted += _ =>
		{
			callbackCount++; callbackForeign = New("goods");
			using var db = NewIndependentContext(database.ConnectionString);
			db.BodiesGameItems.Add(new() { BodyId = callbackNpc.Body.Id, GameItemId = callbackForeign.Id }); db.SaveChanges();
		};
		callbackCorpse.Delete(); callbackCorpse.Delete();
		Require(!callbackCorpse.Deleted && callbackCount == 1 && callbackForeign is not null &&
			typeof(PerceivedItem).GetField("OnDeleted", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(callbackCorpse) is not null &&
			typeof(PerceivedItem).GetField("OnQuit", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(callbackCorpse) is not null,
			"Post-callback conservation hold finalized runtime deletion or replayed a native deletion observer.");
		var decayBeforeHold = callbackCorpse.GetItemType<ICorpse>()!.DecayPoints;
		clock.Advance(TimeSpan.FromMinutes(1)); host.Heartbeats.ManuallyFireHeartbeatMinute();
		Require(callbackCorpse.GetItemType<ICorpse>()!.DecayPoints > decayBeforeHold, "Held native corpse lost its live minute-decay subscription.");
		// An explicit fixture recovery supplies the missing loaded-custody destination. The
		// production guard must never invent this correction or silently delete the unloaded row.
		callbackForeign!.InsertAtSpatialLocation(RouteSpatialService.Instance.GetEffectiveLocation(native.Actor), newStack: true);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			db.BodiesGameItems.RemoveRange(db.BodiesGameItems.Where(x => x.BodyId == callbackNpc.Body.Id));
			db.CellsGameItems.Add(new() { CellId = fixture.CellId, GameItemId = callbackForeign!.Id }); db.SaveChanges();
		}
		callbackCorpse.Delete(); host.Service.ReconcileRetirements(RuntimeClock.UtcNow); Completed(callbackNpc);
		Require(callbackCount == 1 && callbackCorpse.Deleted && !callbackForeign!.Deleted, "Native deletion retry replayed notification or destroyed foreign recovery goods.");
		Console.WriteLine("ARM03B2B-removal-callback=passed actual-native-GameItem-Delete OnDeleted-injected-unloaded-foreign-join postcallback-refusal persisted-corpse-and-live-events-minute-decay-retained notification-not-replayed explicit-fixture-recovery native-Delete-and-Quit-complete foreign-item-conserved");

		var morphPrototype = host.Prototypes.Values.Single(x => x.Name == "ARM03B2B corpse");
		var replacementId = host.Prototypes.Values.Single(x => x.Name == "ARM03B2B goods").Id;
		SetPrivateField(morphPrototype, "_onMorphGameItemProto", replacementId);
		var morphNpc = Create(SpellLifecycleMode.DeathOnExpiry); var morphCorpse = morphNpc.Die()!;
		int itemCount; using (var db = NewIndependentContext(database.ConnectionString)) itemCount = db.GameItems.Count();
		clock.Advance(TimeSpan.FromSeconds(241)); host.Scheduler.CheckSchedules();
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(!morphCorpse.Deleted && db.GameItems.Count() == itemCount && Life(morphNpc).Diagnostic.StartsWith("Native remains morph held:") &&
				db.GameItems.Any(x => x.Id == morphCorpse.Id), "Unsupported owned remains replacement morphed before conservation admission.");
		SetPrivateField(morphPrototype, "_onMorphGameItemProto", 0L);
		morphCorpse.Delete(); host.Service.ReconcileRetirements(RuntimeClock.UtcNow); Completed(morphNpc);
		Console.WriteLine("ARM03B2B-replacement-morph-hold=passed actual-native-Scheduler-fired positive-target-Morph refused-before-new-item-output-transfer-or-activation exact-item-row-count-and-original-corpse-retained explicit-replacement-adapter-hold ordinary-removal-still-works");

		var faulted = Create(SpellLifecycleMode.TemporaryCleanup); var faultForeign = New("goods");
		faulted.Body.Get(faultForeign, silent: true); native.World.SaveManager.Flush(); var faultLife = Life(faulted);
		clock.Advance(TimeSpan.FromSeconds(61));
		using (var db = NewIndependentContext(database.ConnectionString)) db.Database.ExecuteSqlRaw("CREATE TRIGGER arm03b2b_evacuation_refusal BEFORE INSERT ON Cells_GameItems FOR EACH ROW SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'ARM03B2B evacuation rollback fixture'");
		try { host.Service.ReconcileRetirements(RuntimeClock.UtcNow); }
		finally { using var db = NewIndependentContext(database.ConnectionString); db.Database.ExecuteSqlRaw("DROP TRIGGER arm03b2b_evacuation_refusal"); }
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(!faulted.State.HasFlag(CharacterState.Dead) && Life(faulted).State == SpellLifecycleState.Retiring && db.BodiesGameItems.Any(x => x.BodyId == faulted.Body.Id && x.GameItemId == faultForeign.Id) &&
				!db.CellsGameItems.Any(x => x.GameItemId == faultForeign.Id), "Failed foreign transfer did not retain durable intent and rolled-back native custody before death.");
		native.World.SaveManager.Flush();
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(faulted.Body.AllItems.Contains(faultForeign) && ReferenceEquals(faultForeign.InInventoryOf, faulted.Body) &&
				db.BodiesGameItems.Count(x => x.BodyId == faulted.Body.Id && x.GameItemId == faultForeign.Id) == 1 &&
				!db.CellsGameItems.Any(x => x.GameItemId == faultForeign.Id), "Provider-refused evacuation leaked through an ordinary save flush.");
		faulted.Quit(silent: true); native.World.SaveManager.Flush();
		((All<ICharacter>)host.Roots.CachedActors).Remove(faulted); ((All<ICharacter>)host.Roots.Actors).Remove(faulted);
		((All<ICharacter>)host.Roots.NPCs).Remove(faulted); ((All<IBody>)host.Roots.Bodies).Remove(faulted.Body);
		faultForeign.Quit(); host.Items.Remove(faultForeign);
		RunOwnedRetirementReader(new(database.Name, fixture, faultLife.Origin.Id, RuntimeClock.UtcNow, 0, [faultForeign.Id], "retire"));
		host.Service.ReconcileRetirements(RuntimeClock.UtcNow); Completed(faulted);
		Console.WriteLine("ARM03B2B-crash-retry=passed actual-provider-refused-cell-join after-native-runtime-transfer transaction-rollback original-persisted-body-join-retained no-native-death-before-commit separate-process-reload-and-expiry conservation-before-death one-cell-join canonical-completion old-process-runtime-release");

		var baseline = RetirementRuntimeCensus(host); var bodyBaseline = HeavyCensus(database);
		for (var i = 0; i < 16; i++)
		{
			var npc = Create(i % 2 == 0 ? SpellLifecycleMode.TemporaryCleanup : SpellLifecycleMode.DeathOnExpiry);
			var life = Life(npc); if (i % 3 == 0) npc.Die();
			clock.Advance(TimeSpan.FromSeconds(61)); host.Service.ReconcileRetirements(RuntimeClock.UtcNow);
			if (npc.Corpse is { } remains) remains.Parent.Delete();
			host.Service.ReconcileRetirements(RuntimeClock.UtcNow); Completed(npc);
			Require(HeavyCensus(database) == bodyBaseline && RetirementRuntimeCensus(host) == baseline,
				"Repeated native retirement leaked heavy rows, runtime roots, timers or heartbeat subscriptions.");
		}
		Console.WriteLine("ARM03B2B-repeated=passed sixteen-native-create-kill-or-expire-quit-delete-archive-cycles exact-heavy-row-runtime-root-timer-heartbeat-steady-state permanent-retained-explained canonical-identity-history-growth-only");
		Console.WriteLine("ARM03B2B-qualifier=controlled-world-catalogues-cell-and-effect-scheduler actual-native-NPC-Body-SimpleNPCTemplate-GameItem-components-Scheduler-HeartbeatManager-Die-Quit-Delete-and-EF no-installed-command-session no-high-volume-N16 no-occupied-host-or-cross-body-attachment-evacuation");
		return 0;
	}

	private static (int Bodies, int Npcs, int Instances) HeavyCensus(TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString); return (db.Bodies.Count(), db.Npcs.Count(), db.CharacterInstances.Count());
	}
	private static (int Actors, int Cached, int Npcs, int Bodies, int Schedules, int Subscribers) RetirementRuntimeCensus(RetirementHost host)
	{
		var heap = typeof(Scheduler).GetField("_schedules", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(host.Scheduler)!;
		var schedules = (int)heap.GetType().GetProperty("Count")!.GetValue(heap)!;
		var subscribers = typeof(HeartbeatManager).GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
			.Sum(field => (field.GetValue(host.Heartbeats) as Delegate)?.GetInvocationList().Length ?? 0);
		return (host.Roots.Actors.Count(), host.Roots.CachedActors.Count(), host.Roots.NPCs.Count(), host.Roots.Bodies.Count(), schedules, subscribers);
	}

	private static void RunOwnedRetirementReader(RetirementReader input)
	{
		using (var boundary = NewIndependentContext(FMDB.ConnectionString))
		{
			boundary.Database.OpenConnection();
			OwnedConnections.Validate("process-retirement-reader", boundary.Database.GetDbConnection(), requireActive: true);
		}
		var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
		start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); start.ArgumentList.Add("--spell-owned-retirement-reader");
		start.ArgumentList.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(input))));
		using var process = Process.Start(start)!; var output = process.StandardOutput.ReadToEndAsync(); var errors = process.StandardError.ReadToEndAsync();
		if (!process.WaitForExit(60000)) { process.Kill(true); throw new TimeoutException("Owned native retirement reader exceeded 60 seconds."); }
		Console.Write(output.GetAwaiter().GetResult()); Require(process.ExitCode == 0, errors.GetAwaiter().GetResult());
	}

	private static int RunSpellOwnedRetirementReader(string[] args)
	{
		var input = JsonSerializer.Deserialize<RetirementReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args.Single())))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime); using var time = RuntimeClock.Push(clock);
		var host = PrepareRetirementHost(database, input.Fixture, clock); var life = host.Store.Find(input.Lifecycle)!;
		var npc = (RuntimeNpc)host.Native.World.TryGetCharacter(life.Entities.Single(x => x.Kind == SpellOwnedEntityKind.AutonomousCharacter).Id, true);
		if (input.Action == "topology")
		{
			var root = npc.Body.AllItems.Single(x => x.Id == input.Foreign[0]);
			var nested = root.GetItemType<IContainer>()!.Contents.Single(); var child = nested.GetItemType<IContainer>()!.Contents.Single();
			Require(nested.Id == input.Foreign[1] && child.Id == input.Foreign[2] && ReferenceEquals(nested.ContainedIn, root) &&
				ReferenceEquals(child.ContainedIn, nested), "Restart did not retain the original nested foreign topology after callback rollback.");
			using var topologyDb = NewIndependentContext(database.ConnectionString);
			Require(input.Foreign.All(id => topologyDb.GameItems.Any(x => x.Id == id)) &&
				topologyDb.BodiesGameItems.Count(x => x.BodyId == npc.Body.Id && x.GameItemId == root.Id) == 1 &&
				!topologyDb.CellsGameItems.Any(x => x.GameItemId == root.Id) && !npc.State.HasFlag(CharacterState.Dead),
				"Read-only restart inspection lost original held custody after ordinary flush.");
			Console.WriteLine("ARM03B2B-reader-topology=passed separate-owned-process original-A-to-B-to-child-loaded-after-ordinary-save-flush exact-native-edges-and-IDs-and-original-body-join no-death-or-retirement-in-inspection-reader");
			return 0;
		}
		if (input.Action == "guard-custody")
		{
			var root = npc.Body.AllItems.Single(x => x.Id == input.Foreign[0]); var child = root.GetItemType<IContainer>()!.Contents.Single();
			var external = host.Native.World.TryGetItem(input.ExternalItem, true);
			using var guardedDb = NewIndependentContext(database.ConnectionString);
			Require(child.Id == input.Foreign[1] && !child.Deleted && ReferenceEquals(child.ContainedIn, root) &&
				external.InInventoryOf is null && guardedDb.CellsGameItems.Count(x => x.GameItemId == input.ExternalItem) == 1 &&
				!guardedDb.BodiesGameItems.Any(x => x.GameItemId == input.ExternalItem) &&
				guardedDb.BodiesGameItems.Count(x => x.BodyId == npc.Body.Id && x.GameItemId == root.Id) == 1 &&
				!guardedDb.CellsGameItems.Any(x => x.GameItemId == root.Id), "Restarted guarded custody did not retain original body, child and external room goods.");
			Console.WriteLine("ARM03B2C-reader-guard-custody=passed separate-owned-process exact-original-body-child-and-unacquired-room-item-reloaded-after-ordinary-flush");
			return 0;
		}
		if (input.Action == "removal")
		{
			var remains = host.Native.World.TryGetItem(input.Corpse, true); var deaths = 0; var notifications = 0;
			npc.OnDeath += _ => deaths++; remains.OnDeleted += _ => notifications++;
			Require(life.RemainsRemovalRequestedUtc is not null && life.RemainsNotificationCompletedUtc is not null &&
				npc.State.HasFlag(CharacterState.Dead), "Restart did not retain exact durable removal and completed observer intent.");
			host.Service.ReconcileRetirements(RuntimeClock.UtcNow); life = host.Store.Find(input.Lifecycle)!;
			using var removalDb = NewIndependentContext(database.ConnectionString);
			Require(life.State == SpellLifecycleState.Completed && deaths == 0 && notifications == 0 && !removalDb.GameItems.Any(x => x.Id == input.Corpse) &&
				input.Foreign.All(id => removalDb.GameItems.Any(x => x.Id == id)) &&
				removalDb.CellsGameItems.Count(x => x.GameItemId == input.Foreign[0]) == 1 && !removalDb.Bodies.Any(x => x.Id == npc.Body.Id),
				"Restarted admitted remains removal failed idempotent completion or repeated native callbacks: " + life.Diagnostic);
			Console.WriteLine("ARM03B2C-reader-removal=passed separate-owned-process pending-final-DELETE-durable-journal native-deletion-and-canonical-archive exact-foreign-conservation zero-native-death-and-observer-replay");
			return 0;
		}

		if (input.Action == "retire")
		{
			var deaths = 0; npc.OnDeath += _ => deaths++;
			using (var trigger = NewIndependentContext(database.ConnectionString)) trigger.Database.ExecuteSqlRaw("CREATE TRIGGER arm03b2b_completion_refusal BEFORE UPDATE ON MagicSpellLifecycles FOR EACH ROW BEGIN IF NEW.State=3 THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'ARM03B2B post-archive completion refusal'; END IF; END");
			try { host.Service.ReconcileRetirements(RuntimeClock.UtcNow); }
			finally { using var trigger = NewIndependentContext(database.ConnectionString); trigger.Database.ExecuteSqlRaw("DROP TRIGGER arm03b2b_completion_refusal"); }
			using var completed = NewIndependentContext(database.ConnectionString); life = host.Store.Find(input.Lifecycle)!;
			Require(deaths == 1 && life.State == SpellLifecycleState.RemainsPending && completed.CharacterArchives.Any(x => x.LifecycleId == input.Lifecycle) && input.Foreign.All(id => completed.GameItems.Any(x => x.Id == id)) &&
				completed.CellsGameItems.Count(x => x.GameItemId == input.Foreign[0]) == 1 && !completed.Bodies.Any(x => x.Id == npc.Body.Id),
				"Restart did not resume committed retirement intent with exact conserved custody: " + life.Diagnostic);
			Console.WriteLine("ARM03B2B-reader-retry=passed separate-owned-process actual-native-live-NPC-and-foreign-item-reload persisted-intent-resumed exact-one-Die no-corpse foreign-cell-join-once real-Quit canonical-archive committed-archive-with-completion-update-provider-refused-and-left-pending-for-crash-retry");
			return 0;
		}
		var corpse = host.Native.World.TryGetItem(input.Corpse, true);
		corpse.InsertAtSpatialLocation(RouteSpatialService.Instance.GetEffectiveLocation(host.Native.Actor), newStack: true); corpse.Login();
		using (var custody = NewIndependentContext(database.ConnectionString))
		{
			var joined = custody.BodiesGameItems.Count(x => x.BodyId == npc.Body.Id);
			Console.WriteLine($"ARM03B2B-reload-observed=state:{(int)npc.State} death-observed:{life.DeathObservedUtc is not null} persisted-body-joins:{joined} runtime-body-items:{string.Join(',', npc.Body.AllItems.Select(x => x.Id))} root-cell:{host.Native.World.TryGetItem(input.Foreign[0], true).Location?.Id}");
			if (!npc.Body.AllItems.Any(x => x.Id == input.Foreign[0]))
			{
				var foreignRoot = host.Native.World.TryGetItem(input.Foreign[0], true);
				var manual = npc.Body.CanPerformManualAction(out var manualReason);
				var planar = npc.CanInteractPlanar(foreignRoot, PlanarInteractionKind.Inventory, out var planarReason);
				Console.WriteLine($"ARM03B2B-reload-eligibility=components:{string.Join(',', foreignRoot.Components.Select(x => x.GetType().Name))} item-get:{foreignRoot.CanGet(0, ItemCanGetIgnore.IgnoreWeight)} manual:{manual}:{manualReason} planar:{planar}:{planarReason} hold-locations:{string.Join(',', npc.Body.HoldLocs.Select(x => $"{x.Id}:{npc.Body.CanUseBodypart(x)}:{x.CanGrab(foreignRoot, npc.Body)}"))}");
			}
			Require(npc.State.HasFlag(CharacterState.Dead) && life.DeathObservedUtc is not null && npc.Body.AllItems.Any(x => x.Id == input.Foreign[0]), "Restart did not load native dead custody.");
		}
		var component = corpse.GetItemType<ICorpse>()!; var before = component.DecayPoints;
		clock.Advance(TimeSpan.FromMinutes(1)); host.Heartbeats.ManuallyFireHeartbeatMinute();
		Require(component.DecayPoints > before, "Actual corpse minute heartbeat did not decay native remains.");
		clock.Advance(TimeSpan.FromMinutes(5)); host.Scheduler.CheckSchedules();
		Require(corpse.Deleted, "Actual scheduled native morph-to-nothing did not remove the corpse.");
		host.Service.ReconcileRetirements(RuntimeClock.UtcNow);
		life = host.Store.Find(input.Lifecycle)!;
		using var db = NewIndependentContext(database.ConnectionString);
		Require(life.State == SpellLifecycleState.Completed && input.Foreign.All(id => db.GameItems.Any(x => x.Id == id)) &&
			db.GameItems.Find(input.Foreign[1])!.ContainerId == input.Foreign[0] && db.CellsGameItems.Count(x => x.GameItemId == input.Foreign[0]) == 1 &&
			!db.Bodies.Any(x => x.Id == npc.Body.Id) && !db.GameItems.Any(x => x.Id == input.Corpse), "Native restarted retirement did not conserve and retire its exact graph: " + life.Diagnostic);
		var bag = host.Native.World.TryGetItem(input.Foreign[0], true); var belt = bag.GetItemType<IContainer>()!.Contents.Single();
		Require(bag.GetItemType<ILockable>()!.Locks.Single().Parent.Id == input.Foreign[3] && belt.GetItemType<IBelt>()!.ConnectedItems.Single().Parent.Id == input.Foreign[2], "Foreign native lock or belt attachment was changed.");
		Console.WriteLine("ARM03B2B-reader-decay=passed independent-owned-process actual-dead-NPC-body-nested-item-components-reload real-corpse-minute-heartbeat-positive-decay real-Scheduler-Morph-Delete guarded-foreign-transfer actual-Quit canonical-archive one-cell-join exact-lock-belt-and-item-identities");
		return 0;
	}
}
