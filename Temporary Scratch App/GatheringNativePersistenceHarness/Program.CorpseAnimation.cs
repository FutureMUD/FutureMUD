#nullable enable

using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Form.Shape;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Lifecycle;
using MudSharp.Magic.SpellEffects;
using MudSharp.NPC.AI;
using MudSharp.NPC.Templates;
using Db = MudSharp.Models;
using RuntimeNpc = MudSharp.NPC.NPC;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record CorpseAnimationReader(string Database, FixtureIds Fixture, DateTime Now, Guid Origin,
		long Instance, long Corpse, long Owner, long Body, long Foreign, string Action, DateTime Deadline, long Destination);

	private static IArtificialIntelligence ConfigureCorpseAnimationWorld(RetirementHost host, string connection)
	{
		var native = host.Native; var world = native.World;
		var service = new SpellOwnedCorpseAnimationService(world);
		native.WorldMock.SetupGet(x => x.SpellOwnedCorpseAnimations).Returns(service);
		Mock.Get(native.Body.Race).SetupGet(x => x.NaturalPerceptionTypes).Returns(PerceptionTypes.DirectVisual);
		Mock.Get(native.Actor.Location).SetupGet(x => x.Perceivables)
			.Returns(() => native.Actor.Location.GameItems.Cast<IPerceivable>().Append(native.Actor));
		SpellAnimatedCorpseEffect.InitialiseEffectType(); CorpseAnimationDispelProxyEffect.InitialiseEffectType();
		using var db = NewIndependentContext(connection);
		var progRow = db.FutureProgs.Include(x => x.FutureProgsParameters).Single(x => x.FunctionName == "arm03d1_ai_resource");
		var prog = new FutureProg(world, progRow.FunctionName, ProgVariableTypes.Number,
			[Tuple.Create(ProgVariableTypes.Character, "actor")], progRow.FunctionText) { Id = progRow.Id };
		Require(prog.Compile(), "Native AI resource program failed: " + prog.CompileError);
		((All<IFutureProg>)world.FutureProgs).Add(prog);
		var ai = (CombatEndAI)typeof(CombatEndAI).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
			null, [typeof(Db.ArtificialIntelligence), typeof(IFuturemud)], null)!.Invoke([db.ArtificialIntelligences.Single(x => x.Name == "ARM03D1 selected AI"), world]);
		var ais = new All<IArtificialIntelligence>(); ais.Add(ai); native.WorldMock.SetupGet(x => x.AIs).Returns(ais);
		return ai;
	}

	private static int RunCorpseAnimationChecks()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_"); ConfigureNativeDatabase(database.ConnectionString);
		Console.WriteLine($"ARM03D1-created={database.Name}");
		var fixture = FixtureSeed.Create(database, "arm03d1_animation", true);
		var clock = new HarnessClock(); using var time = RuntimeClock.Push(clock);
		using (var db = NewIndependentContext(database.ConnectionString)) db.Database.Migrate();
		var seed = NativeRuntime.Load(fixture, database.ConnectionString, true); ConfigureCastingWorld(seed, database.ConnectionString, true);
		SeedRetirementPrototypes(database, seed.World.Materials.First().Id);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var prog = new Db.FutureProg { FunctionName = "arm03d1_ai_resource", FunctionComment = "Native selected AI integration fixture",
				FunctionText = $"return addmagicresource(@actor, {seed.Resource.Id}, 1)", ReturnTypeDefinition = ProgVariableTypes.Number.ToStorageString(),
				Category = "Harness", Subcategory = "CorpseAnimation", Public = true };
			prog.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "actor", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
			db.FutureProgs.Add(prog); db.SaveChanges();
			db.ArtificialIntelligences.Add(new() { Name = "ARM03D1 selected AI", Type = "CombatEnd", Definition =
				$"<Definition><WillAcceptTruce>0</WillAcceptTruce><WillAcceptTargetIncapacitated>0</WillAcceptTargetIncapacitated><OnOfferedTruce>0</OnOfferedTruce><OnTargetIncapacitated>0</OnTargetIncapacitated><OnNoNaturalTargets>{prog.Id}</OnNoNaturalTargets></Definition>" });
			db.SaveChanges();
		}
		var host = PrepareRetirementHost(database, fixture, clock, corpseAnimationAnatomy: true); var native = host.Native; var world = native.World; var caster = native.Actor;
		var ai = ConfigureCorpseAnimationWorld(host, database.ConnectionString); var service = world.SpellOwnedCorpseAnimations!;
		var data = new SimpleCharacterTemplate
		{
			Gameworld = world, SelectedName = new PersonalName(new XElement("Name", new XAttribute("culture", 1), new XElement("Element", new XAttribute("usage", "BirthName"), "Borrowed Remains")), world),
			SelectedRace = native.Body.Race, SelectedEthnicity = native.Body.Ethnicity, SelectedCulture = caster.Culture,
			SelectedBirthday = world.Calendars.First().GetDate("1-month-2000"), SelectedStartingLocation = caster.Location,
			SelectedGender = native.Body.Gender.Enum, SelectedHeight = 1.8, SelectedWeight = 80, SelectedSdesc = "a borrowed body", SelectedFullDesc = "An ordinary native body.",
			SelectedAccents = [], SelectedAttributes = [], SelectedCharacteristics = [], SelectedEntityDescriptionPatterns = [], SkillValues = [], SelectedRoles = [], SelectedMerits = [],
			SelectedKnowledges = [], MissingBodyparts = [], SelectedDisfigurements = [], SelectedProstheses = []
		};
		var template = new SimpleNPCTemplate(world, DummyAccount.Instance, data, "ARM03D1 ordinary source");
		var owner = (RuntimeNpc)template.CreateNewCharacter(caster.Location); world.Add(owner, true); world.Add(owner.Body); world.SaveManager.Flush();
		var foreign = host.Prototypes.Values.Single(x => x.Name == "ARM03B2B goods").CreateNew(caster); world.Add(foreign);
		owner.Body.Get(foreign, silent: true); world.SaveManager.Flush();
		var corpse = owner.Die() ?? throw new InvalidOperationException("Ordinary native death did not create a source corpse."); world.SaveManager.Flush();
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			// The host controls its room catalogue. Its native-item membership is persisted explicitly.
			if (!db.CellsGameItems.Any(x => x.GameItemId == corpse.Id)) db.CellsGameItems.Add(new() { CellId = fixture.CellId, GameItemId = corpse.Id });
			db.SaveChanges();
		}
		Require(owner.Body.ExternalItems.Contains(foreign) && corpse.GetItemType<ICorpse>().OriginalBody.Id == owner.Body.Id,
			"Source death did not retain the real borrowed body and foreign item.");
		var cap = (SkillLevelBasedMagicCapability)native.Capability; var skill = world.Traits.GetByName("ARM02 Earth Proficiency")!;
		var spell = new MagicSpell("ARM03D1 Raise Servitor replacement", cap.School); ((All<IMagicSpell>)world.MagicSpells).Add(spell);
		foreach (var command in new[] { "trigger new item", $"trait {skill.Id}", "difficulty easy", "threshold minorpass", "duration ARM02 Duration",
			$"cost {native.Resource.Id} ARM02 Cost", $"prog {world.FutureProgs.GetByName("rejuvenation_known")!.Id}", "castemote A borrowed body stirs.", "failcastemote The corpse remains still.",
			"grades fixture", "effect add animatecorpse", $"effect 1 ai add {ai.Id}", "effect 1 lifecycle durable", "effect 1 family raise-servitor", "effect 1 lifetime grade*60" })
			Require(spell.BuildingCommand(caster, new StringStack(command)), "Corpse-animation builder refused " + command);
		foreach (var command in new[] { $"casting trait {skill.Id}", $"casting resources {native.Resource.Id} {native.Resource.Id} passive", $"casting entry add {spell.Id}", $"casting entry starting {spell.Id} on", "casting enable on" })
			Require(cap.BuildingCommand(caster, new StringStack(command)), "Corpse route builder refused " + command);
		caster.SetMerits([NativeRuntime.NewCapabilityMerit(cap)]); caster.SetTraitValue(skill, 100); world.SaveManager.Flush();
		var staff = new Mock<ICharacter>(); staff.SetupGet(x => x.Id).Returns(999); staff.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
		var casting = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, random: () => 0.1, flush: () => { world.SaveManager.Flush(); FlushCasting(native); });
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(casting);
		Require(casting.Enrol(staff.Object, caster, cap.Id, "Corpse-animation fixture").Allowed, "Corpse casting enrolment failed.");
		new MagicCastingStateStore().Write(acquired: casting.Acquisition(caster, spell.Id)! with { ControlledGrade = 7 });
		caster.AddResource(native.Resource, 100); FlushCasting(native); var beforePayment = caster.MagicResourceAmounts[native.Resource];
		Require(caster.CanSee(corpse) && ReferenceEquals(caster.TargetItem("corpse"), corpse), "The native item parser must discover the real visible source corpse.");
		Require(spell.BuildingCommand(caster, new StringStack("effect 1 lifetime 0")), "Zero-lifetime fixture edit failed.");
		var refused = casting.Cast(new(caster, cap.Id, spell.Id, 3, false, "corpse"));
		Require(refused.Status == MagicCastingStatus.Refused && refused.OperationId is null && refused.Message.Contains("lifetime") && caster.MagicResourceAmounts[native.Resource] == beforePayment,
			"Invalid corpse lifetime reached payment: " + refused.Message);
		Require(spell.BuildingCommand(caster, new StringStack("effect 1 lifetime grade*60")), "Lifetime fixture restoration failed.");
		var result = casting.Cast(new(caster, cap.Id, spell.Id, 3, false, "corpse"));
		Require(result.Status == MagicCastingStatus.Succeeded, "Paid corpse animation failed: " + result.Message);
		var animated = owner.Instances.OfType<ScriptedAiCharacterInstance>().Single();
		var life = ReadCorpseAnimationLife(database, animated.InstanceId);
		Require(life.Origin.Grade == 3 && life.Origin.CreatorId == caster.Id && life.Origin.DeadlineUtc == RuntimeClock.UtcNow.AddSeconds(180) &&
			life.Entities.Count == 1 && life.Entities[0].Kind == SpellOwnedEntityKind.CharacterInstance && !corpse.TrueLocations.Any() &&
			ReferenceEquals(animated.Identity, owner) && ReferenceEquals(animated.Body, owner.Body) && animated.Body.ExternalItems.Contains(foreign) &&
			caster.MagicResourceAmounts[native.Resource] < beforePayment && animated.AIs.Single().Id == ai.Id,
			"Paid animation changed ownership, deadline, identity or foreign custody.");
		Console.WriteLine("ARM03D1-paid-creation=passed actual-builder-paid-grade3-cast zero-lifetime-prepayment-refusal exact-one-new-secondary different-caster-and-original-identity same-body foreign-gear retained-source-corpse hidden atomic-cell-link 180-real-seconds-authored-replacement");
		var beforeAi = owner.MagicResourceAmounts[native.Resource];
		Require(animated.HandleEvent(EventType.NoNaturalTargets, animated) && owner.MagicResourceAmounts[native.Resource] == beforeAi + 1 &&
			animated.MagicResourceAmounts[native.Resource] == owner.MagicResourceAmounts[native.Resource], "Selected native CombatEndAI did not execute its real compiled resource prog against canonical state.");
		Console.WriteLine("ARM03D1-selected-ai=passed actual-ScriptedAiCharacterInstance native-CombatEndAI-event compiled-FutureProg mutates-canonical-resource exactly-once no-cloned-resource-pool combat-profile-NOT-QUALIFIED");
		var deathCallbacks = 0; corpse.OnDeath += _ => deathCallbacks++;
		corpse.Delete(); corpse.Die();
		Require(!corpse.Deleted && !corpse.Destroyed && deathCallbacks == 0 && owner.Body.ExternalItems.Contains(foreign), "Borrowed corpse removal reached destructive callbacks.");
		Console.WriteLine("ARM03D1-borrow-guard=passed native-Delete-and-Die-before-callback-refusal corpse-body-foreign-gear-retained");
		clock.Advance(TimeSpan.FromSeconds(181)); service.ReconcileRetirements(RuntimeClock.UtcNow);
		AssertCorpseAnimationRestored(database, host, life.Origin.Id, animated.InstanceId, corpse.Id, owner.Id, owner.Body.Id, foreign.Id);
		Console.WriteLine("ARM03D1-expiry=passed absolute-deadline worker same-real-corpse-and-body-and-canonical-identity restored one-secondary-removed no-duplicate-remains no-foreign-item-loss");

		ScriptedAiCharacterInstance CreateDirect(Guid? id = null, IReadOnlyCollection<IArtificialIntelligence>? selected = null) =>
			(ScriptedAiCharacterInstance)service.Create(corpse, caster, selected ?? [ai], new(id ?? Guid.NewGuid(), spell.Id, 3, caster.Id,
				"raise-servitor", SpellLifecycleMode.TemporaryCleanup, RuntimeClock.UtcNow, RuntimeClock.UtcNow.AddSeconds(180), "ARM03D1 direct service lifecycle fixture"));
		void Restored(ScriptedAiCharacterInstance actor) => AssertCorpseAnimationRestored(database, host,
			ReadCorpseAnimationLife(database, actor.InstanceId).Origin.Id, actor.InstanceId, corpse.Id, owner.Id, owner.Body.Id, foreign.Id);
		void Flush() { world.SaveManager.Flush(); FlushCasting(native); }
		CorpseAnimationReader Reader(ScriptedAiCharacterInstance actor, string action)
		{
			var journal = ReadCorpseAnimationLife(database, actor.InstanceId);
			return new(database.Name, fixture, RuntimeClock.UtcNow, journal.Origin.Id, actor.InstanceId, corpse.Id, owner.Id, owner.Body.Id, foreign.Id, action, journal.Origin.DeadlineUtc!.Value, corpse.Location?.Id ?? fixture.CellId);
		}

		var stable = Guid.NewGuid(); var duplicate = CreateDirect(stable);
		var duplicateRefused = false;
		try { CreateDirect(stable); } catch (InvalidOperationException) { duplicateRefused = true; }
		Require(duplicateRefused && owner.Instances.Count(x => !x.IsPrimaryInstance) == 1, "An already borrowed source produced another secondary.");
		Require(service.TryRetire(duplicate.InstanceId, SpellRetirementReason.Dismissal, out _), "Direct dismissal failed."); Restored(duplicate);
		var replayRefused = false;
		try { CreateDirect(stable); } catch (InvalidOperationException) { replayRefused = true; }
		Require(replayRefused && owner.Instances.All(x => x.IsPrimaryInstance), "Completed creation key was replayed.");
		Console.WriteLine("ARM03D1-idempotence=passed concurrent-same-source-refused completed-creation-key-refused explicit-dismissal exact-original-corpse-restored");

		var failedOrigin = Guid.NewGuid(); int beforeInstances;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			beforeInstances = db.CharacterInstances.Count();
			db.Database.ExecuteSqlRaw("CREATE TRIGGER arm03d1_claim_fault BEFORE INSERT ON MagicSpellOwnedEntities FOR EACH ROW SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='ARM03D1 claim insertion refusal'");
		}
		var claimRefused = false;
		try { CreateDirect(failedOrigin); } catch (DbUpdateException) { claimRefused = true; }
		finally { using var db = NewIndependentContext(database.ConnectionString); db.Database.ExecuteSqlRaw("DROP TRIGGER arm03d1_claim_fault"); }
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(claimRefused && db.CharacterInstances.Count() == beforeInstances && !db.MagicSpellLifecycles.Any(x => x.Id == failedOrigin) &&
				db.CellsGameItems.Count(x => x.GameItemId == corpse.Id) == 1 && corpse.Location == caster.Location && owner.Instances.All(x => x.IsPrimaryInstance),
				"Ownership claim failure did not roll back the inserted secondary and exact source cell link before exposure.");
		Console.WriteLine("ARM03D1-creation-rollback=passed actual-MySQL-owned-claim-INSERT-refusal atomic-secondary-and-source-cell-rollback no-runtime-exposure no-owned-canonical-or-body");

		// Fault injection uses a selected AI predicate; actor/control/body/heartbeat teardown remains native.
		var brokenAi = new Mock<IArtificialIntelligence>(); brokenAi.SetupGet(x => x.Id).Returns(99001); brokenAi.SetupGet(x => x.Gameworld).Returns(world);
		brokenAi.SetupGet(x => x.IsReadyToBeUsed).Returns(true);
		brokenAi.Setup(x => x.HandlesEvent(It.IsAny<EventType[]>())).Returns<EventType[]>(types => types.Contains(EventType.TenSecondTick)
			? throw new InvalidOperationException("ARM03D1 after-first-heartbeat activation failure") : types.Contains(EventType.FiveSecondTick));
		((All<IArtificialIntelligence>)world.AIs).Add(brokenAi.Object);
		var activationOrigin = Guid.NewGuid(); var activationFailed = false;
		try { CreateDirect(activationOrigin, [brokenAi.Object]); } catch (InvalidOperationException ex) when (ex.Message.Contains("after-first-heartbeat")) { activationFailed = true; }
		var ghostRoots = typeof(HeartbeatManager).GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
			.SelectMany(x => (x.GetValue(host.Heartbeats) as Delegate)?.GetInvocationList() ?? [])
			.Count(x => x.Target is ScriptedAiCharacterInstance actor && ReferenceEquals(actor.Identity, owner));
		Require(activationFailed && host.Store.Find(activationOrigin)!.State == SpellLifecycleState.Completed && ghostRoots == 0 &&
			ReferenceEquals(owner.Body.Actor, owner) && owner.Body.Controller is null && owner.Instances.All(x => x.IsPrimaryInstance), "Failed activation retained a secondary/controller/heartbeat root.");
		Console.WriteLine("ARM03D1-activation-rollback=passed injected-second-AI-predicate-failure-after-native-heartbeat-registration exact-corpse-restoration zero-secondary-controller-and-heartbeat-roots");
		var constructorOrigin = Guid.NewGuid(); var constructorFailed = false;
		native.WorldMock.Setup(x => x.RetrieveAppropriateCommandTree(It.IsAny<ICharacter>())).Throws(new InvalidOperationException("ARM03D1 base constructor failure"));
		try { CreateDirect(constructorOrigin); } catch (InvalidOperationException ex) when (ex.Message.Contains("base constructor")) { constructorFailed = true; }
		finally { native.WorldMock.Setup(x => x.RetrieveAppropriateCommandTree(It.IsAny<ICharacter>())).Returns(caster.CommandTree); }
		Require(constructorFailed && host.Store.Find(constructorOrigin)!.State == SpellLifecycleState.Completed && ReferenceEquals(owner.Body.Actor, owner) &&
			owner.Body.Controller is null && owner.Instances.All(x => x.IsPrimaryInstance), "Base constructor failure retained the borrowed-body actor or controller.");
		Console.WriteLine("ARM03D1-constructor-rollback=passed injected-command-tree-provider-failure-after-body-actor-assignment original-body-actor-and-control-restored exact-committed-secondary-retired");

		using (var db = NewIndependentContext(database.ConnectionString))
		{
			db.CharacterBodySources.Add(new() { CharacterId = caster.Id, BodyId = owner.Body.Id, SourceId = 123, SourceType = 0, SourceKey = "ARM03D1 foreign reference" }); db.SaveChanges();
		}
		var beforeForeignRefusal = caster.MagicResourceAmounts[native.Resource];
		var foreignRefusal = casting.Cast(new(caster, cap.Id, spell.Id, 3, false, "corpse"));
		Require(foreignRefusal.Status == MagicCastingStatus.Refused && foreignRefusal.OperationId is null && foreignRefusal.Message.Contains("exclusively") &&
			caster.MagicResourceAmounts[native.Resource] == beforeForeignRefusal, "Cached corpse with foreign persisted ownership reached payment.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var reference = db.CharacterBodySources.Single(x => x.CharacterId == caster.Id && x.SourceKey == "ARM03D1 foreign reference");
			Require(reference.BodyId == owner.Body.Id && db.CellsGameItems.Any(x => x.GameItemId == corpse.Id), "Foreign reference refusal mutated the source or foreign row.");
			db.CharacterBodySources.Remove(reference); db.SaveChanges();
		}
		Console.WriteLine("ARM03D1-foreign-owner-refusal=passed cached-native-corpse newly-inserted-foreign-source-reference prepayment-refusal unchanged-foreign-row body-and-corpse-retained");

		clock.Advance(TimeSpan.FromMinutes(5)); caster.AddResource(native.Resource, 100); Flush();
		var dispelCast = casting.Cast(new(caster, cap.Id, spell.Id, 3, false, "corpse"));
		Require(dispelCast.Status == MagicCastingStatus.Succeeded, "Dispel fixture paid cast failed: " + dispelCast.Message);
		var dispelled = owner.Instances.OfType<ScriptedAiCharacterInstance>().Single();
		var dispel = (DispelMagicEffect)typeof(DispelMagicEffect).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
			[typeof(XElement), typeof(IMagicSpell)], null)!.Invoke([new XElement("Effect", new XElement("EffectKey", "animatecorpse")), spell]);
		dispel.GetOrApplyEffect(caster, dispelled, default, default, null!, []); Restored(dispelled);
		Console.WriteLine("ARM03D1-dispel=passed actual-paid-spell-parent native-DispelMagicEffect via-animated-proxy same-corpse-restored owning-child-removed");
		var killed = CreateDirect(); var itemRows = 0;
		using (var db = NewIndependentContext(database.ConnectionString)) itemRows = db.GameItems.Count();
		Require(killed.Die() is null, "Animated death created extra remains."); Flush(); Restored(killed);
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(db.GameItems.Count() == itemRows && owner.State.IsDead() && ReadCorpseAnimationLife(database, killed.InstanceId).RemainsItemId is null, "Animated death consumed or cloned borrowed remains.");
		Console.WriteLine("ARM03D1-death=passed actual-ScriptedAiCharacterInstance-Die no-second-corpse original-canonical-death-unchanged borrowed-remains-never-owned native-save-after-removal");

		var held = CreateDirect();
		using (var db = NewIndependentContext(database.ConnectionString)) db.Database.ExecuteSqlRaw($"CREATE TRIGGER arm03d1_delete_fault BEFORE DELETE ON CharacterInstances FOR EACH ROW BEGIN IF OLD.Id={held.InstanceId} THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='ARM03D1 final secondary DELETE refusal'; END IF; END");
		try
		{
			Require(!service.TryRetire(held.InstanceId, SpellRetirementReason.Dismissal, out _), "Provider refusal unexpectedly completed.");
			var heldResource = owner.MagicResourceAmounts[native.Resource];
			Require(!held.HandleEvent(EventType.NoNaturalTargets, held) && owner.MagicResourceAmounts[native.Resource] == heldResource, "Retirement-held selected AI still acted.");
			Flush();
			using (var db = NewIndependentContext(database.ConnectionString)) Require(db.CharacterInstances.Any(x => x.Id == held.InstanceId) &&
				!db.CellsGameItems.Any(x => x.GameItemId == corpse.Id) && db.BodiesGameItems.Any(x => x.BodyId == owner.Body.Id && x.GameItemId == foreign.Id), "Final DELETE refusal did not roll back restoration placement.");
			RunItemReaderProcess(Reader(held, "held"), "--corpse-animation-reader");
		}
		finally { using var db = NewIndependentContext(database.ConnectionString); db.Database.ExecuteSqlRaw("DROP TRIGGER arm03d1_delete_fault"); }
		Require(service.TryRetire(held.InstanceId, SpellRetirementReason.Dismissal, out _), "Provider-held exact retry failed."); Restored(held);
		Console.WriteLine("ARM03D1-provider-retry=passed actual-final-secondary-DELETE-refusal atomic-rollback paused-selected-AI ordinary-save independent-held-reader exact-once-retry foreign-gear-conserved");

		var missing = CreateDirect();
		native.WorldMock.Setup(x => x.TryGetItem(corpse.Id, true)).Returns((IGameItem)null!);
		Require(!service.TryRetire(missing.InstanceId, SpellRetirementReason.Dismissal, out var missingWhy) && missingWhy.Contains("unavailable"), "Unavailable corpse did not produce recoverable holding.");
		native.WorldMock.Setup(x => x.TryGetItem(corpse.Id, true)).Returns(corpse);
		Require(service.TryRetire(missing.InstanceId, SpellRetirementReason.Dismissal, out _), "Unavailable-corpse retry failed."); Restored(missing);
		var noCell = CreateDirect(); noCell.ClearInstanceLocation(); ((All<ICell>)world.Cells).Remove(caster.Location);
		Require(!service.TryRetire(noCell.InstanceId, SpellRetirementReason.Dismissal, out var cellWhy) && cellWhy.Contains("safe cell"), "Unavailable destination did not preserve recovery.");
		((All<ICell>)world.Cells).Add(caster.Location);
		Require(service.TryRetire(noCell.InstanceId, SpellRetirementReason.Dismissal, out _), "Unavailable-destination retry failed."); Restored(noCell);
		Console.WriteLine("ARM03D1-missing-dependency=passed controlled-unavailable-corpse-and-cell recoverable-hold no-borrowed-deletion exact-retry original-cell-fallback");

		// Same real MoveTo callback used by Cell.Insert, in the controlled host's room catalogue.
		var placement = CreateDirect(); var originalCell = caster.Location; var placementCallbacks = 0; long finalCellId;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var newCell = new Db.Cell { RoomId = db.Cells.Single(x => x.Id == fixture.CellId).RoomId, EffectData = "<Effects/>" };
			db.Cells.Add(newCell); db.SaveChanges(); finalCellId = newCell.Id;
		}
		var finalCell = new Mock<ICell>(); var finalItems = new List<IGameItem>();
		finalCell.SetupGet(x => x.Id).Returns(finalCellId); finalCell.SetupGet(x => x.Gameworld).Returns(world);
		finalCell.SetupGet(x => x.GameItems).Returns(finalItems); finalCell.Setup(x => x.Extract(It.IsAny<IGameItem>())).Callback<IGameItem>(item => finalItems.Remove(item));
		((All<ICell>)world.Cells).Add(finalCell.Object); placement.MoveTo(finalCell.Object, placement.RoomLayer); placement.Save();
		finalCell.Setup(x => x.Insert(It.IsAny<IGameItem>(), It.IsAny<bool>())).Callback<IGameItem, bool>((item, _) =>
		{
			ForeignCustodyTransferContext.EnsureCell(finalCell.Object, item); item.MoveTo(finalCell.Object, item.RoomLayer);
			if (!finalItems.Contains(item)) finalItems.Add(item);
		});
		LocatableEvent callback = (_, _) => { placementCallbacks++; caster.Body.Get(corpse, silent: true); };
		corpse.OnLocationChanged += callback;
		Require(!service.TryRetire(placement.InstanceId, SpellRetirementReason.Dismissal, out _) && placementCallbacks == 1 && corpse.InInventoryOf is null,
			"Restoration allowed a callback to acquire the borrowed corpse.");
		corpse.OnLocationChanged -= callback;
		var placementLife = ReadCorpseAnimationLife(database, placement.InstanceId);
		Require(placementLife.State != SpellLifecycleState.Completed && placementLife.Diagnostic.StartsWith("<CorpseRestore "), "Committed restoration destination was lost after callback refusal.");
		Flush();
		RunItemReaderProcess(Reader(placement, "placement"), "--corpse-animation-reader");
		// Explicitly simulate a stale deferred cell-membership save after row deletion. The durable marker must retain the final cell.
		using (var db = NewIndependentContext(database.ConnectionString)) { db.CellsGameItems.RemoveRange(db.CellsGameItems.Where(x => x.GameItemId == corpse.Id)); db.SaveChanges(); }
		Require(service.TryRetire(placement.InstanceId, SpellRetirementReason.Dismissal, out _), "Placement retry failed."); Restored(placement);
		using (var db = NewIndependentContext(database.ConnectionString)) Require(db.CellsGameItems.Single(x => x.GameItemId == corpse.Id).CellId == finalCellId && corpse.Location.Id == finalCellId, "Retry forgot the moved actor's committed destination.");
		Console.WriteLine("ARM03D1-placement-guard=passed real-MoveTo-OnLocationChanged callback native-Body.Get refused-before-custody moved-actor-final-cell-checkpoint ordinary-save independent-reader explicit-stale-cell-join-loss-simulation exact-retry no-dual-custodian controlled-cell-insertion");
		finalCell.Object.Extract(corpse); originalCell.Insert(corpse, true);
		using (var db = NewIndependentContext(database.ConnectionString))
		{ db.CellsGameItems.RemoveRange(db.CellsGameItems.Where(x => x.GameItemId == corpse.Id)); db.CellsGameItems.Add(new() { CellId = fixture.CellId, GameItemId = corpse.Id }); db.SaveChanges(); }
		for (var cycle = 0; cycle < 5; cycle++)
		{
			var repeated = CreateDirect(); Require(repeated.Quit(true), "Repeated native Quit restoration failed."); Flush(); Restored(repeated);
			Require(owner.Instances.All(x => x.IsPrimaryInstance) && owner.Body.Controller is null, "Repeated cleanup retained a runtime instance/control root.");
		}
		Console.WriteLine("ARM03D1-repeated-retirement=passed five-native-Quit-and-save-cycles zero-secondary-and-controller-roots exact-body-corpse-and-gear-IDs high-volume-N16-NOT-QUALIFIED");

		var restart = CreateDirect(); Flush();
		RunItemReaderProcess(Reader(restart, "restart"), "--corpse-animation-reader");
		Console.WriteLine("ARM03D1-restart=passed independent-process saved-active-origin no-child-effect-required collapse-on-restart exact-deadline-and-source-identities no-rematerialized-AI old-host-quiescent-after-reader");
		return 0;
	}

	private static int RunCorpseAnimationReader(string[] args)
	{
		Require(args.Length == 1, "Corpse animation reader requires one receipt.");
		var input = JsonSerializer.Deserialize<CorpseAnimationReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args[0])))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime); using var time = RuntimeClock.Push(clock);
		var host = PrepareRetirementHost(database, input.Fixture, clock, corpseAnimationAnatomy: true); ConfigureCorpseAnimationWorld(host, database.ConnectionString);
		var world = host.Native.World; var corpse = world.TryGetItem(input.Corpse, true); var component = corpse.GetItemType<ICorpse>();
		Require(component.OriginalBody.Id == input.Body && component.OriginalCharacter.Id == input.Owner &&
			component.OriginalCharacter.Identity.Instances.All(x => x.InstanceId != input.Instance) && component.OriginalBody.AllItems.Any(x => x.Id == input.Foreign),
			"Cold native corpse load lost original body/gear or rematerialized the despawn-on-reboot AI.");
		if (input.Action == "placement")
		{
			using var db = NewIndependentContext(database.ConnectionString);
			Require(!db.CharacterInstances.Any(x => x.Id == input.Instance) && !db.BodiesGameItems.Any(x => x.GameItemId == input.Corpse) &&
				db.GameItems.Single(x => x.Id == input.Corpse).ContainerId is null && db.CellsGameItems.Single(x => x.GameItemId == input.Corpse).CellId == input.Destination &&
				host.Store.Find(input.Origin)!.Diagnostic.Contains($"cell=\"{input.Destination}\""), "Cold inspection lost committed final destination or gained foreign corpse custody.");
			Console.WriteLine("ARM03D1-reader-placement=passed fresh-process real-corpse-body-gear-load durable-final-cell after-callback-refusal-and-ordinary-save no-dual-custody no-owned-row-recreation");
			return 0;
		}
		world.SpellOwnedCorpseAnimations!.ReconcileRetirements(RuntimeClock.UtcNow);
		Require(host.Store.Find(input.Origin)!.Origin.DeadlineUtc == input.Deadline, "Restart changed the absolute deadline.");
		if (input.Action == "held")
		{
			using var db = NewIndependentContext(database.ConnectionString);
			Require(host.Store.Find(input.Origin)!.State != SpellLifecycleState.Completed && db.CharacterInstances.Any(x => x.Id == input.Instance) &&
				!db.CellsGameItems.Any(x => x.GameItemId == input.Corpse), "Cold provider-held recovery lost the exact instance or exposed the corpse.");
			Console.WriteLine("ARM03D1-reader-held=passed fresh-process actual-native-corpse-body-gear-loader no-AI-materialization durable-refusal exact-secondary-retained");
			return 0;
		}
		AssertCorpseAnimationRestored(database, host, input.Origin, input.Instance, input.Corpse, input.Owner, input.Body, input.Foreign);
		world.SaveManager.Flush(); AssertCorpseAnimationRestored(database, host, input.Origin, input.Instance, input.Corpse, input.Owner, input.Body, input.Foreign);
		Console.WriteLine("ARM03D1-reader-restored=passed fresh-process runtime-worker actual-native-final-corpse-body-gear-loader original-IDs ordinary-save exact-secondary-delete no-created-body-or-canonical-delete");
		return 0;
	}

	private static SpellOwnedLifecycle ReadCorpseAnimationLife(TestDatabase database, long instance)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var id = db.MagicSpellOwnedEntities.Single(x => x.Kind == (int)SpellOwnedEntityKind.CharacterInstance && x.EntityId == instance).LifecycleId;
		return new SpellOwnedLifecycleStore().Find(id)!;
	}

	private static void AssertCorpseAnimationRestored(TestDatabase database, RetirementHost host, Guid origin,
		long instance, long corpse, long owner, long body, long foreign)
	{
		using var db = NewIndependentContext(database.ConnectionString); var life = host.Store.Find(origin)!;
		Require(life.State == SpellLifecycleState.Completed && !db.CharacterInstances.Any(x => x.Id == instance) &&
			db.Characters.Any(x => x.Id == owner && !x.IsArchived) && db.Bodies.Any(x => x.Id == body) &&
			db.GameItems.Any(x => x.Id == corpse) && db.CellsGameItems.Count(x => x.GameItemId == corpse) == 1 &&
			db.BodiesGameItems.Any(x => x.BodyId == body && x.GameItemId == foreign) && !host.Native.World.TryGetItem(corpse, true).Deleted,
			"Animation restoration did not retain exact borrowed state: " + life.Diagnostic);
	}
}
