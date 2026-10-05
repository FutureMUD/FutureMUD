#nullable enable

using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation;
using MudSharp.Combat;
using MudSharp.Commands.Helpers;
using MudSharp.Commands.Trees;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Form.Shape;
using MudSharp.Form.Audio;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Lifecycle;
using MudSharp.NPC.AI;
using MudSharp.NPC;
using MudSharp.NPC.Templates;
using MudSharp.Planes;
using Db = MudSharp.Models;
using RuntimeNpc = MudSharp.NPC.NPC;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record StockAnimationReader(string Database, Guid Origin, long Instance, long Creator, DateTime Now,
		long Spell, DateTime Deadline, bool MayCommand);

	private static int RunRaiseServitorStockReader(string[] args)
	{
		var input = JsonSerializer.Deserialize<StockAnimationReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args.Single())))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime); using var time = RuntimeClock.Push(clock);
		var service = new SpellOwnedCorpseAnimationService(Mock.Of<IFuturemud>());
		Require(service.CanCommand(input.Instance, input.Creator) == input.MayCommand && !service.CanCommand(input.Instance, input.Creator + 10000000), "Persisted creator grant drifted in fresh process.");
		var life = new SpellOwnedLifecycleStore().Find(input.Origin)!;
		using var db = NewIndependentContext(database.ConnectionString);
		var definition = XElement.Parse(db.MagicSpells.AsNoTracking().Single(x => x.Id == input.Spell).Definition);
		Require(life.Origin.DeadlineUtc == input.Deadline && definition.Element("StockIdentity")?.Value == ArmageddonRaiseServitorStock.Key &&
			definition.Element("Effects")!.Element("Effect")!.Element("Lifecycle")!.Element("Control")!.Element("Seconds")!.Value == ArmageddonRaiseServitorStock.ControlSeconds,
			"Stock definition or lifetime metadata drifted in fresh process.");
		Console.WriteLine("ARM03D1Stock-reader=passed fresh-process durable-control-query persisted-stock-identity-formula-and-animation-deadline no-actor-materialization-or-replay");
		return 0;
	}
	private static int RunRaiseServitorStockChecks(bool queuedAuthorityOnly = false, bool queuedCallbackOnly = false, bool orderedCallbacks = false)
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_"); ConfigureNativeDatabase(database.ConnectionString);
		var fixture = FixtureSeed.Create(database, "arm03d1_stock", true);
		var clock = new HarnessClock(); using var time = RuntimeClock.Push(clock);
		using (var db = NewIndependentContext(database.ConnectionString)) db.Database.Migrate();
		var seed = NativeRuntime.Load(fixture, database.ConnectionString, true); ConfigureCastingWorld(seed, database.ConnectionString, true);
		SeedRetirementPrototypes(database, seed.World.Materials.First().Id);
		if (orderedCallbacks)
		{
			SeedCreatedWeaponPrototypes(database, seed.World.Materials.First().Id);
			SeedConsumables(database, fixture, seed.World.Materials.First().Id);
			SeedFlameFixture(database);
		}
		using var orderedGlobals = orderedCallbacks ? new ConsumableGlobals() : null;
		var host = PrepareRetirementHost(database, fixture, clock, corpseAnimationAnatomy: true,
			consumablesAnatomy: orderedCallbacks, wielding: orderedCallbacks);
		var native = host.Native; var world = native.World; var caster = native.Actor;
		// The fixture player must be present in native identity roots for live command authority.
		// The partial archive host omits production online-player statistics.
		((All<ICharacter>)world.Actors).Add(caster); ((All<ICharacter>)world.Characters).Add(caster);
		world.Add(caster.Body);
		var casterName = new PersonalName(new XElement("Name", new XAttribute("culture", 1), new XElement("Element", new XAttribute("usage", "BirthName"), "caster")), world);
		SetPrivateField(caster, "_personalName", casterName); SetPrivateField(caster, "_currentName", casterName);
		var effects = new EffectScheduler(world, clock); native.WorldMock.SetupGet(x => x.EffectScheduler).Returns(effects);
		var service = new SpellOwnedCorpseAnimationService(world); native.WorldMock.SetupGet(x => x.SpellOwnedCorpseAnimations).Returns(service);
		SpellAnimatedCorpseEffect.InitialiseEffectType(); CorpseAnimationDispelProxyEffect.InitialiseEffectType();
		CommandableAI.RegisterLoader(); CombatEndAI.RegisterLoader();
		var ais = new All<IArtificialIntelligence>(); native.WorldMock.SetupGet(x => x.AIs).Returns(ais);
		native.WorldMock.Setup(x => x.Add(It.IsAny<IArtificialIntelligence>())).Callback<IArtificialIntelligence>(x => ais.Add(x));
		native.WorldMock.Setup(x => x.Add(It.IsAny<IFutureProg>())).Callback<IFutureProg>(x => ((All<IFutureProg>)world.FutureProgs).Add(x));
		native.WorldMock.Setup(x => x.Add(It.IsAny<ITraitExpression>())).Callback<ITraitExpression>(x => ((All<ITraitExpression>)world.TraitExpressions).Add(x));
		native.WorldMock.Setup(x => x.Add(It.IsAny<IMagicSpell>())).Callback<IMagicSpell>(x => ((All<IMagicSpell>)world.MagicSpells).Add(x));
		native.WorldMock.SetupGet(x => x.AlwaysFalseProg).Returns(world.FutureProgs.GetByName("AlwaysFalse")!);
		native.WorldMock.Setup(x => x.RetrieveAppropriateCommandTree(It.IsAny<ICharacter>())).Returns<ICharacter>(ch =>
			ch is ScriptedAiCharacterInstance ? NPCCommandTree.Instance : PlayerCommandTree.Instance);
		Mock.Get(native.Body.Race).SetupGet(x => x.NaturalPerceptionTypes).Returns(PerceptionTypes.DirectVisual);
		Mock.Get(native.Body.Prototype).SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(world));
		// This disposable cell has no exits; recursive world defaults otherwise fabricate a door target.
		native.WorldMock.SetupGet(x => x.ExitManager).Returns(Mock.Of<IExitManager>());
		native.WorldMock.SetupGet(x => x.HearingProfiles).Returns(new All<IHearingProfile>());
		var cell = CreateAreaCell(native, database.ConnectionString, fixture.CellId, create: true);
		Mock.Get(cell.CurrentOverlay.Package).SetupGet(x => x.Name).Returns("Stock acceptance overlay");
		SetPrivateMember(caster, "Location", cell); ((List<ICharacter>)cell.Characters).Add(caster);
		var combatSettings = new CharacterCombatSettings(caster, "Stock acceptance combat");
		caster.CombatSettings = combatSettings;
		var templates = new RevisableAll<INPCTemplate>(); native.WorldMock.SetupGet(x => x.NpcTemplates).Returns(templates);
		RuntimeNpc Person(string name)
		{
			var data = new SimpleCharacterTemplate
			{
				Gameworld = world, SelectedName = new PersonalName(new XElement("Name", new XAttribute("culture", 1), new XElement("Element", new XAttribute("usage", "BirthName"), name)), world),
				SelectedRace = native.Body.Race, SelectedEthnicity = native.Body.Ethnicity, SelectedCulture = caster.Culture,
				SelectedBirthday = world.Calendars.First().GetDate("1-month-2000"), SelectedStartingLocation = cell,
				SelectedGender = native.Body.Gender.Enum, SelectedHeight = 1.8, SelectedWeight = 80, SelectedSdesc = "a " + name, SelectedFullDesc = "An ordinary native body.",
				SelectedAccents = [], SelectedAttributes = [], SelectedCharacteristics = [], SelectedEntityDescriptionPatterns = [], SkillValues = [], SelectedRoles = [], SelectedMerits = [],
				SelectedKnowledges = [], MissingBodyparts = [], SelectedDisfigurements = [], SelectedProstheses = []
			};
			var template = new SimpleNPCTemplate(world, DummyAccount.Instance, data, "Stock acceptance " + name);
			templates.Add(template);
			var person = (RuntimeNpc)template.CreateNewCharacter(cell); world.Add(person, true); world.Add(person.Body);
			person.CombatSettings = combatSettings; cell.Enter(person); world.SaveManager.Flush(); return person;
		}
		var owner = Person("remains"); var foe = Person("opponent");
		var foreign = host.Prototypes.Values.Single(x => x.Name == "ARM03B2B goods").CreateNew(caster); world.Add(foreign);
		owner.Body.Get(foreign, silent: true); world.SaveManager.Flush();
		var corpse = owner.Die() ?? throw new InvalidOperationException("Stock source death produced no corpse.");
		world.SaveManager.Flush();
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(db.CellsGameItems.Count(x => x.CellId == cell.Id && x.GameItemId == corpse.Id) == 1 && cell.GameItems.Contains(corpse),
				"Native death/Cell.Insert/Cell.Save did not persist the source corpse.");
		var cap = (SkillLevelBasedMagicCapability)native.Capability; var trait = world.Traits.GetByName("ARM02 Earth Proficiency")!;
		EditableItemHelper.MagicSpellHelper.EditableNewAction(caster, new StringStack($"stock raise-servitor {cap.School.Id} {trait.Id} {native.Resource.Id}"));
		var spell = (MagicSpell)world.MagicSpells.Single(x => x.Name == ArmageddonRaiseServitorStock.Name);
		Require(spell.StockIdentity == ArmageddonRaiseServitorStock.Key && spell.ReadyForGame && spell.GradeConfigurationErrors().Count == 0,
			"Builder-created stock is not ready: " + string.Join(";", spell.GradeConfigurationErrors()));
		var beforeDuplicate = world.MagicSpells.Count();
		EditableItemHelper.MagicSpellHelper.EditableNewAction(caster, new StringStack($"stock raise-servitor {cap.School.Id} {trait.Id} {native.Resource.Id}"));
		Require(world.MagicSpells.Count() == beforeDuplicate && ais.Count() == 2, "Duplicate stock authoring created more content.");
		using (new FMDB()) { spell.Save(); FMDB.Context.SaveChanges(); }
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var loaded = new MagicSpell(db.MagicSpells.AsNoTracking().Single(x => x.Id == spell.Id), world);
			Require(loaded.StockIdentity == spell.StockIdentity && loaded.GradeConfigurationErrors().Count == 0 &&
				loaded.SpellEffects.Single().SaveToXml().Element("Lifecycle")!.Element("Control")!.Element("Seconds")!.Value == ArmageddonRaiseServitorStock.ControlSeconds,
				"Native Save/load discarded stock identity or its independently bound control duration.");
			((All<IMagicSpell>)world.MagicSpells).Remove(spell); ((All<IMagicSpell>)world.MagicSpells).Add(loaded); spell = loaded;
		}
		foreach (var command in new[] { $"casting trait {trait.Id}", $"casting resources {native.Resource.Id} {native.Resource.Id} passive", $"casting entry add {spell.Id}", $"casting entry starting {spell.Id} on", "casting enable on" })
			Require(cap.BuildingCommand(caster, new StringStack(command)), "Stock route refused " + command);
		caster.SetMerits([NativeRuntime.NewCapabilityMerit(cap)]); caster.SetTraitValue(trait, 100); world.SaveManager.Flush();
		var staff = new Mock<ICharacter>(); staff.SetupGet(x => x.Id).Returns(999); staff.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
		var casting = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, random: () => 0.1, flush: () => { world.SaveManager.Flush(); FlushCasting(native); });
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(casting);
		Require(casting.Enrol(staff.Object, caster, cap.Id, "Stock acceptance").Allowed, "Stock route enrolment failed.");
		new MagicCastingStateStore().Write(acquired: casting.Acquisition(caster, spell.Id)! with { ControlledGrade = 7 });
		caster.AddResource(native.Resource, 100); FlushCasting(native);
		var preflightEnergy = caster.MagicResourceAmounts[native.Resource];
		Require(spell.BuildingCommand(caster, new StringStack("effect 1 control grade*3000")), "Control builder rejected a syntactically valid test formula.");
		var controlRefusal = casting.Cast(new(caster, cap.Id, spell.Id, 3, false, "corpse"));
		Require(controlRefusal.Status == MagicCastingStatus.Refused && controlRefusal.OperationId is null &&
			controlRefusal.Message.Contains("control duration") && caster.MagicResourceAmounts[native.Resource] == preflightEnergy,
			"Control exceeding animation lifetime was not refused before payment.");
		Require(spell.BuildingCommand(caster, new StringStack("effect 1 control " + ArmageddonRaiseServitorStock.ControlSeconds)), "Stock control formula restoration failed.");
		Console.WriteLine("ARM03D1Stock-install=passed normal-spell-builder atomic-stock-plus-two-real-AIs two-compiled-progs source-duration-formulas definition-save-reload control-bound-refused-before-payment no-reagent no-automatic-capability duplicate-refused");

		ScriptedAiCharacterInstance Cast()
		{
			caster.AddResource(native.Resource, 100); FlushCasting(native);
			var before = caster.MagicResourceAmounts[native.Resource];
			Require(caster.CanSee(corpse) && ReferenceEquals(caster.TargetItem("corpse"), corpse), "Real native item parser did not select the corpse.");
			var result = casting.Cast(new(caster, cap.Id, spell.Id, 3, false, "corpse"));
			Require(result.Status == MagicCastingStatus.Succeeded, "Stock paid cast failed: " + result.Message);
			var animation = owner.Instances.OfType<ScriptedAiCharacterInstance>().Single();
			var life = ReadCorpseAnimationLife(database, animation.InstanceId);
			Require(life.Origin.DeadlineUtc == RuntimeClock.UtcNow.AddSeconds(7799) && life.Origin.CreatorId == caster.Id &&
				ReferenceEquals(animation.Identity, owner) && animation.Body == owner.Body && animation.Body.ExternalItems.Contains(foreign) &&
				caster.MagicResourceAmounts[native.Resource] < before && animation.Following == caster && animation.AIs.Count() == 2,
				"Stock casting changed timing, canonical custody, payment or following.");
			world.SaveManager.Flush();
			using var db = NewIndependentContext(database.ConnectionString);
			Require(!db.CellsGameItems.Any(x => x.GameItemId == corpse.Id) && !cell.GameItems.Contains(corpse), "Deferred native Cell.Save resurrected a borrowed corpse.");
			return animation;
		}
		void Restored(ScriptedAiCharacterInstance animation)
		{
			world.SaveManager.Flush();
			AssertCorpseAnimationRestored(database, host, ReadCorpseAnimationLife(database, animation.InstanceId).Origin.Id,
				animation.InstanceId, corpse.Id, owner.Id, owner.Body.Id, foreign.Id);
			Require(cell.GameItems.Count(x => x.Id == corpse.Id) == 1 && animation.Following is null && animation.QueuedMoveCommands.Count == 0 &&
				animation.Combat is null && !cell.Characters.ContainsPhysicalInstance(animation), $"Stock retirement retained native runtime roots: corpse-count={cell.GameItems.Count(x => x.Id == corpse.Id)} following={animation.Following?.Id} queued={animation.QueuedMoveCommands.Count} combat={animation.Combat?.GetType().Name} cell-characters={string.Join(',', cell.Characters.Select(x => $"{x.Id}/{x.InstanceId}/{ReferenceEquals(x, animation)}"))}.");
		}
		void Order(ScriptedAiCharacterInstance animation, ICharacter issuer, string command) =>
			Require(animation.HandleEvent(EventType.CommandIssuedToCharacter, animation, issuer, command), "Stock command event was not handled.");
		var animated = Cast();
		if (orderedCallbacks)
			return RunOrderedNpcCallbacks(database, host, clock, animated, caster, foe, Cast, Restored, Order, fixture);
		if (queuedCallbackOnly)
			return RunQueuedCommandInternalCallbacks(database, world, clock, animated, caster, foe, foreign, Cast, Restored, Order);
		if (queuedAuthorityOnly)
			return RunQueuedCommandAuthorityChecks(database, world, clock, animated, caster, foe, foreign, Cast, Restored, Order);
		Require(service.CanCommand(animated.InstanceId, caster.Id) && !service.CanCommand(animated.InstanceId, owner.Id) && !service.CanCommand(animated.InstanceId, foe.Id), "Stock authority is not creator-only.");
		Order(animated, foe, "fol self"); Require(animated.Following == caster, "Stranger changed native following.");
		Order(animated, caster, "fol self"); Require(animated.Following is null, "Creator alias did not execute native follow.");
		Order(animated, caster, "quit"); Require(owner.Instances.Contains(animated) && animated.IsEmbodied, "Unlisted quit retired the animation.");
		animated.Follow(caster);
		Order(animated, caster, "hit opponent");
		Require(animated.Combat is SimpleMeleeCombat && animated.Combat == foe.Combat && animated.CombatTarget == foe && foe.CombatTarget == animated,
			"Native stock hit did not create a mutual combat with the real scripted instance.");
		Console.WriteLine("ARM03D1Stock-control-combat=passed creator-not-corpse-owner stranger-refused native-follow-alias unlisted-quit-refused real-hit-command mutual-SimpleMeleeCombat native-combat-schedules");
		var third = Person("third");
		var ongoingCombat = animated.Combat!; ongoingCombat.JoinCombat(third);
		third.CombatTarget = foe; foe.CombatTarget = third;
		animated.Body.Drop(foreign, silent: true);
		Require(foreign.InInventoryOf is null && cell.GameItems.Contains(foreign), "Native combat inventory setup failed to drop the exact foreign item.");
		var queuedAction = SelectedCombatAction.GetEffectGetItem(animated, foreign, null);
		Require(animated.TakeOrQueueCombatAction(queuedAction) && animated.Effects.Contains(queuedAction), "Native selected combat action was not queued.");
		bool Subscribed(string name) => ((Delegate?)typeof(CombatBase).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(ongoingCombat))?
			.GetInvocationList().Any(x => ReferenceEquals(x.Target, queuedAction)) == true;
		Require(Subscribed("CombatEnds") && Subscribed("CombatMerged"), "Selected combat action did not subscribe to ongoing native combat.");
		var selectedMove = animated.ChooseMove();
		Require(selectedMove is MudSharp.Combat.Moves.RetrieveItemMove && !animated.Effects.Contains(queuedAction) &&
			!Subscribed("CombatEnds") && !Subscribed("CombatMerged"), "Native strategy consumed an action without releasing its combat subscriptions.");
		Require(selectedMove.ResolveMove(null).MoveWasSuccessful && animated.Body.ExternalItems.Contains(foreign) &&
			!cell.GameItems.Contains(foreign), "Native selected retrieve action did not execute in the ongoing combat.");
		queuedAction = SelectedCombatAction.GetEffectGetItem(animated, foreign, null);
		Require(animated.TakeOrQueueCombatAction(queuedAction) && Subscribed("CombatEnds") && Subscribed("CombatMerged"), "Second native selected action was not queued.");
		var delayed = 0;
		animated.QueuedMoveCommands.Enqueue("north");
		animated.AddEffect(new BlockingDelayedAction(animated, _ => delayed++, "queued stock acceptance action", ["general"], null), TimeSpan.FromSeconds(2));
		host.Scheduler.AddSchedule(new Schedule<ICharacter>(animated, _ => delayed++, ScheduleType.Combat, TimeSpan.FromSeconds(2), "Stock retirement cancellation"));
		Require(service.TryRetire(animated.InstanceId, SpellRetirementReason.Dispel, out var diagnostic), diagnostic); Restored(animated);
		Require(third.Combat == ongoingCombat && foe.Combat == ongoingCombat && !ongoingCombat.Combatants.Contains(animated) &&
			!animated.Effects.Contains(queuedAction) && !Subscribed("CombatEnds") && !Subscribed("CombatMerged"), "Retirement retained selected-action subscriptions in the continuing native combat.");
		ongoingCombat.LeaveCombat(third); ongoingCombat.LeaveCombat(foe);
		clock.Advance(TimeSpan.FromSeconds(3)); effects.CheckSchedules(); host.Scheduler.CheckSchedules();
		Require(delayed == 0 && animated.ChooseMove() is null && !animated.Engage(foe, false), "Retired stock actor executed queued work or re-engaged.");
		Restored(animated);
		Console.WriteLine("ARM03D1Stock-native-cell-combat-retire=passed real-Cell-Insert-Extract-Save queued-save-after-hide-and-restore selected-inventory-action-executed consumed-and-pending-action-unsubscribed continuing-three-actor-combat native-combat-leave both-scheduler-queues-cancelled follow-unsubscribed exact-corpse-body-foreign-gear");

		animated = Cast(); var active = ReadCorpseAnimationLife(database, animated.InstanceId);
		clock.Advance(TimeSpan.FromSeconds(5398));
		Require(service.CanCommand(animated.InstanceId, caster.Id), "Stock control ended before the historical fresh-affect deadline.");
		clock.Advance(TimeSpan.FromSeconds(1));
		Require(!service.CanCommand(animated.InstanceId, caster.Id) && active.Origin.DeadlineUtc > RuntimeClock.UtcNow && animated.IsEmbodied,
			"Control expiry prematurely removed animation or retained command authority.");
		Order(animated, caster, "fol self"); Require(animated.Following == caster, "Expired control still issued a command or removed the independent source follower link.");
		clock.Advance(TimeSpan.FromSeconds(2400)); effects.CheckSchedules(); Restored(animated);
		Console.WriteLine("ARM03D1Stock-two-deadlines=passed grade3-control-5399s animation-7799s real-affect-seconds independent-follow-link paid-parent-effect-scheduler-expiry same-corpse");

		var normalTerrain = cell.CurrentOverlay.Terrain;
		foreach (var name in new[] { "Silt", "Shallows" })
		{
			using (var db = NewIndependentContext(database.ConnectionString))
			{
				var row = new Db.Terrain { Name = name, TerrainBehaviourMode = "outdoors", MovementRate = 1 }; db.Terrains.Add(row); db.SaveChanges();
				var terrain = new Terrain(row, world); ((All<ITerrain>)world.Terrains).Add(terrain); ((IEditableCellOverlay)cell.CurrentOverlay).Terrain = terrain;
			}
			animated = Cast();
			Require(!service.CanCommand(animated.InstanceId, caster.Id) && animated.Following == caster, "Source excluded terrain granted charm or omitted its independent follower link.");
			Order(animated, caster, "fol self"); Require(animated.Following == caster, "Excluded terrain accepted a creator order.");
			Require(service.TryRetire(animated.InstanceId, SpellRetirementReason.Dismissal, out diagnostic), diagnostic); Restored(animated);
		}
		((IEditableCellOverlay)cell.CurrentOverlay).Terrain = normalTerrain;
		Console.WriteLine("ARM03D1Stock-terrain=passed compiled-stock-prog actual-Silt-and-Shallows-terrains no-command-grant independent-native-follow-link");

		animated = Cast(); var heldLife = ReadCorpseAnimationLife(database, animated.InstanceId);
		RunItemReaderProcess(new StockAnimationReader(database.Name, heldLife.Origin.Id, animated.InstanceId, caster.Id,
			RuntimeClock.UtcNow, spell.Id, heldLife.Origin.DeadlineUtc!.Value, true), "--raise-servitor-stock-reader");
		Order(animated, caster, "hit opponent"); Require(animated.Combat is SimpleMeleeCombat, "Held-retirement setup did not enter combat.");
		animated.QueuedMoveCommands.Enqueue("north");
		animated.AddEffect(new BlockingDelayedAction(animated, _ => delayed++, "queued engagement", ["general"], null), TimeSpan.FromSeconds(2));
		host.Scheduler.AddSchedule(new Schedule<ICharacter>(animated, _ => delayed++, ScheduleType.Combat, TimeSpan.FromSeconds(2), "Held retirement cancellation"));
		using (var db = NewIndependentContext(database.ConnectionString))
			db.Database.ExecuteSqlRaw("CREATE TRIGGER armstock_retire_fault BEFORE DELETE ON CharacterInstances FOR EACH ROW SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Stock retirement held'");
		try
		{
			Require(!service.TryRetire(animated.InstanceId, SpellRetirementReason.Dismissal, out _) && animated.Combat is null && animated.Following is null &&
				animated.QueuedMoveCommands.Count == 0 && !service.CanCommand(animated.InstanceId, caster.Id), "Held retirement retained combat/follow/order roots.");
			clock.Advance(TimeSpan.FromSeconds(3)); effects.CheckSchedules(); host.Scheduler.CheckSchedules();
			Require(delayed == 0 && animated.ChooseMove() is null && !animated.Engage(foe, false) && !animated.HandleEvent(EventType.CommandIssuedToCharacter, animated, caster, "hit opponent"),
				"Held retirement executed work or accepted another order.");
			world.SaveManager.Flush();
			using var db = NewIndependentContext(database.ConnectionString);
			Require(db.CharacterInstances.Any(x => x.Id == animated.InstanceId) && !db.CellsGameItems.Any(x => x.GameItemId == corpse.Id), "Held restoration leaked a corpse or removed its recoverable secondary.");
			RunItemReaderProcess(new StockAnimationReader(database.Name, heldLife.Origin.Id, animated.InstanceId, caster.Id,
				RuntimeClock.UtcNow, spell.Id, heldLife.Origin.DeadlineUtc!.Value, false), "--raise-servitor-stock-reader");
		}
		finally { using var db = NewIndependentContext(database.ConnectionString); db.Database.ExecuteSqlRaw("DROP TRIGGER armstock_retire_fault"); }
		Require(service.TryRetire(animated.InstanceId, SpellRetirementReason.Dismissal, out diagnostic), diagnostic); Restored(animated);
		Require(service.TryRetire(animated.InstanceId, SpellRetirementReason.Dismissal, out diagnostic), diagnostic); Restored(animated);
		Console.WriteLine("ARM03D1Stock-held-retirement=passed actual-DELETE-refusal real-cell-save native-combat-leave no-queued-actions no-command-grant exact-retry repeated-retirement same-canonical-body-corpse-foreign-item");
		return 0;
	}
}
