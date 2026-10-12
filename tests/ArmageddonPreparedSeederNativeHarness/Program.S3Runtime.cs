#nullable enable

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Events.Hooks;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Scheduling;
using MudSharp.Database;
using MySql.Data.MySqlClient;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Lifecycle;
using MudSharp.NPC.AI;
using MudSharp.NPC.Templates;
using Db = MudSharp.Models;
using RuntimeNpc = MudSharp.NPC.NPC;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static int RunS3RuntimeAcceptanceChecks()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString);
		var fixture = FixtureSeed.Create(database, "arm_s3_native_runtime", true);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			db.Database.Migrate();
			Require(!db.Database.HasPendingModelChanges(), "S3 native runtime fixture model parity failed.");
			db.CharacterInstances.Add(new Db.CharacterInstance
			{
				Id = fixture.CharacterId,
				CharacterId = fixture.CharacterId,
				BodyId = fixture.BodyId,
				InstanceName = "S3 fixture primary",
				InstanceKind = (int)CharacterInstanceKind.Primary,
				ControlPolicy = (int)CharacterInstanceControlPolicy.IdentityCoordinatorControlled,
				DeathPolicy = (int)CharacterInstanceDeathPolicy.FinalCharacterDeath,
				PerceptionPolicy = (int)CharacterInstancePerceptionPolicy.OrdinaryEmbodied,
				PersistencePolicy = (int)CharacterInstancePersistencePolicy.Persistent,
				LocationId = fixture.RoomId,
				PositionId = 1,
				PositionTargetType = string.Empty,
				PositionEmote = string.Empty,
				State = (int)CharacterState.Awake,
				IsPrimary = true,
				IsEmbodied = true,
				IsControllable = true,
				CreatedDateTime = RuntimeClock.UtcNow,
				EffectData = "<Effects/>"
			});
			db.SaveChanges();
		}
		var clock = new HarnessClock();
		using var time = RuntimeClock.Push(clock);
		var native = NativeRuntime.Load(fixture, database.ConnectionString, true);
		SetPrivateField(native.Actor, "_instanceId", fixture.CharacterId);
		SetPrivateField(native.Actor, "_isPrimaryInstance", true);
		SetPrivateField(native.Actor, "_isEmbodied", true);
		SetPrivateField(native.Actor, "_isControllable", true);
		ConfigureCastingWorld(native, database.ConnectionString, true);
		PrepareLifecycleRuntime(native);
		var archiveRoots = ArchiveRoots();
		ConfigureArchiveWorld(native, archiveRoots, fixture, database.ConnectionString);
		var actors = new All<ICharacter>();
		actors.Add(native.Actor);
		native.WorldMock.SetupGet(x => x.Actors).Returns(actors);
		var service = new SpellOwnedNpcService(native.World);
		var lifecycleStore = new SpellOwnedLifecycleStore();
		native.WorldMock.SetupGet(x => x.SpellOwnedNpcs).Returns(service);
		native.WorldMock.SetupGet(x => x.DefaultHooks).Returns(Array.Empty<IDefaultHook>());
		native.WorldMock.SetupGet(x => x.Currencies).Returns(new All<MudSharp.Economy.Currency.ICurrency>());
		native.WorldMock.Setup(x => x.GetStaticString("MaximumStaminaExpression")).Returns("100");
		Mock.Get(native.Body.Race).SetupGet(x => x.BaseBody).Returns(native.World.BodyPrototypes.First());
		Mock.Get(native.Body.Race).SetupGet(x => x.BreathingStrategy).Returns(new MudSharp.Health.Breathing.NonBreather());
		var templates = new Mock<IUneditableRevisableAll<INPCTemplate>>();
		templates.Setup(x => x.GetEnumerator()).Returns(() => Enumerable.Empty<INPCTemplate>().GetEnumerator());
		native.WorldMock.SetupGet(x => x.NpcTemplates).Returns(templates.Object);
		var rawSkill = CastingRequired(native.World.Traits.GetByName("ARM02 Earth Proficiency") as SkillDefinition);
		rawSkill.Cap = new MudSharp.Body.Traits.TraitExpression("50", native.World);
		var templateData = new SimpleCharacterTemplate
		{
			Gameworld = native.World,
			SelectedName = new MudSharp.Character.Name.PersonalName(new System.Xml.Linq.XElement("Name",
				new System.Xml.Linq.XAttribute("culture", 1), new System.Xml.Linq.XElement("Element",
					new System.Xml.Linq.XAttribute("usage", "BirthName"), "S3 Companion")), native.World),
			SelectedRace = native.Body.Race, SelectedEthnicity = native.Body.Ethnicity, SelectedCulture = native.Actor.Culture,
			SelectedBirthday = native.World.Calendars.First().GetDate("1-month-2000"), SelectedStartingLocation = native.Actor.Location,
			SelectedGender = native.Body.Gender.Enum, SelectedHeight = 1.8, SelectedWeight = 80,
			SelectedSdesc = "an S3 companion", SelectedFullDesc = "A fixture companion for native controller acceptance.",
			SelectedAccents = [], SelectedAttributes = [], SelectedCharacteristics = [], SelectedEntityDescriptionPatterns = [],
			SkillValues = [(rawSkill, 80d)], SelectedRoles = [], SelectedMerits = [], SelectedKnowledges = [], MissingBodyparts = [],
			SelectedDisfigurements = [], SelectedProstheses = []
		};
		var template = new SimpleNPCTemplate(native.World, DummyAccount.Instance, templateData, "ARM S3 native companion");
		SetPrivateMember(template, "Status", RevisionStatus.Current);
		var aiModel = new Db.ArtificialIntelligence
		{
			Name = "ARM S3 native companion AI", Type = "SpellCompanion",
			Definition = "<Definition><Mode>companion</Mode><Purpose>companion</Purpose><Permitted><Order>follow</Order><Order>stop</Order><Order>guard</Order><Order>attack</Order></Permitted><LossChanceCap>0.5</LossChanceCap></Definition>"
		};
		using (var db = NewIndependentContext(database.ConnectionString)) { db.ArtificialIntelligences.Add(aiModel); db.SaveChanges(); }
		var ai = (IArtificialIntelligence)typeof(SpellCompanionAI).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
			.Single(x => x.GetParameters() is [{ ParameterType: var modelType }, { ParameterType: var worldType }] &&
				modelType == typeof(Db.ArtificialIntelligence) && worldType == typeof(IFuturemud)).Invoke([aiModel, native.World]);
		template.ArtificialIntelligences.Add(ai);
		templates.Setup(x => x.Get(template.Id)).Returns(template);
		templates.Setup(x => x.Get(template.Id, 0)).Returns(template);
		templates.Setup(x => x.GetByIdOrName(template.Id.ToString(), true)).Returns(template);

		var capability = (MudSharp.Magic.Capabilities.SkillLevelBasedMagicCapability)native.Capability;
		var skill = CastingRequired(native.World.Traits.GetByName("ARM02 Earth Proficiency") as ITraitDefinition);
		native.Actor.AddTrait(skill, 42);
		native.Actor.SetTraitValue(skill, 42);
		foreach (var command in new[] { $"casting trait {skill.Id}", $"casting resources {native.Resource.Id} {native.Resource.Id} passive" })
			Require(capability.BuildingCommand(native.Actor, new StringStack(command)), $"S3 casting capability builder rejected {command}.");
		var spell = new MagicSpell("ARM S3 native runtime fixture", capability.School);
		((All<IMagicSpell>)native.World.MagicSpells).Add(spell);
		foreach (var command in new[] { "trigger new room", $"trait {skill.Id}", "difficulty easy", "threshold minorpass",
			"duration ARM02 Duration", $"cost {native.Resource.Id} ARM02 Cost",
			$"prog {CastingRequired(native.World.FutureProgs.GetByName("rejuvenation_known")).Id}",
			"castemote A fixture companion appears.", "failcastemote The fixture companion fails to appear.", "grades fixture",
			"effect add createnpc", $"effect 1 npc {template.Id}", "effect 1 lifecycle deathonexpiry",
			"effect 1 family s3-runtime-fixture", "effect 1 lifetime grade*60", "effect 1 guardcaster" })
			Require(spell.BuildingCommand(native.Actor, new StringStack(command)), $"S3 spell builder rejected {command}.");
		Require(capability.BuildingCommand(native.Actor, new StringStack($"casting entry add {spell.Id}")), "S3 explicit capability admission failed.");
		Require(capability.BuildingCommand(native.Actor, new StringStack($"casting entry starting {spell.Id} on")), "S3 starting admission failed.");
		Require(capability.BuildingCommand(native.Actor, new StringStack("casting enable on")), "S3 capability enable failed.");
		native.Actor.SetMerits([NativeRuntime.NewCapabilityMerit(capability)]);
		native.World.SaveManager.Flush();
		var staff = new Mock<ICharacter>(); staff.SetupGet(x => x.Id).Returns(999);
		staff.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
		var casting = new MagicCastingService(native.World, flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(casting);
		Require(casting.Enrol(staff.Object, native.Actor, capability.Id, "S3 native controller fixture").Allowed, "S3 caster enrolment failed.");
		native.Actor.AddResource(native.Resource, 100);
		FlushCasting(native);
		var acquired = casting.Acquisition(native.Actor, spell.Id) ?? throw new InvalidOperationException("S3 spell acquisition was not available.");
		new MagicCastingStateStore().Write(acquired: acquired with { ControlledGrade = 2 });
		var created = new System.DateTime(RuntimeClock.UtcNow.Ticks / 10 * 10, System.DateTimeKind.Utc);
		var origin = new SpellLifecycleOrigin(Guid.NewGuid(), spell.Id, 2, native.Actor.Id, "s3-runtime-fixture",
			SpellLifecycleMode.DeathOnExpiry, created, created.AddMinutes(2), "S3 native runtime acceptance fixture");
		native.WorldMock.Setup(x => x.Add(It.IsAny<ICharacter>(), true)).Callback<ICharacter, bool>((actor, _) => actors.Add(actor));
		var npc = (RuntimeNpc)template.CreateSpellOwnedCharacter(native.Actor.SpatialLocation, origin);
		actors.Add(npc);
		npc.AddEffect(new SpellNpcControlEffect(npc, origin, native.Actor, guardCreator: true));
		var control = npc.EffectsOfType<SpellNpcControlEffect>().Single();
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var primaryRows = db.CharacterInstances.AsNoTracking().Where(x => x.CharacterId == npc.Id && x.IsPrimary)
				.Select(x => $"{x.Id}/{x.CharacterId}/{x.BodyId}").ToArray();
			var life = lifecycleStore.Find(origin.Id);
			var meritApplies = native.Actor.Merits.OfType<MudSharp.RPG.Merits.Interfaces.IMagicCapabilityMerit>()
				.Any(x => x.Applies(native.Actor) && x.Capabilities.Contains(capability));
			var admissions = capability.CastingPolicy?.Admissions.ToArray() ?? [];
			var claims = life?.Entities.Select(x => $"{x.Kind}:{x.Id}").ToArray() ?? [];
			var creatorPrimary = db.CharacterInstances.AsNoTracking().Where(x => x.Id == native.Actor.InstanceId)
				.Select(x => $"{x.Id}/{x.CharacterId}/{x.BodyId}/{x.IsPrimary}").SingleOrDefault();
			Console.WriteLine($"ARM-S3-gates=npc:{npc.Id}/{npc.InstanceId}/{npc.Body.Id}/{npc.IsPrimaryInstance}/{npc.State} creator:{CharacterInstanceIdentityComparer.IdentityId(native.Actor)}/{native.Actor.InstanceId}/{native.Actor.Body.Id}/{native.Actor.IsPrimaryInstance} actors:{native.World.Actors.Any(x => ReferenceEquals(x, npc))}/{native.World.Actors.Any(x => ReferenceEquals(x, native.Actor))} spell:{spell.Id}/{spell.ReadyForGame}/{spell.GradeProfile?.Version}/{casting.Acquisition(native.Actor, spell.Id)?.ProfileVersion} cap:{capability.Id}/{native.Actor.Capabilities.Contains(capability)}/{meritApplies}/{capability.CastingPolicy?.Enabled}/{capability.CastingConfigurationErrors().Count}/routes:{string.Join(',', admissions.Select(x => $"{x.SpellId}:{x.MinimumGrade}-{x.MaximumGrade}"))} life:{life?.State}/{life?.Diagnostic}/{string.Join(',', claims)} creatorPrimary:{creatorPrimary} npcPrimary:{string.Join(',', primaryRows)}");
		}
		Require(service.HasCommandAuthority(npc, native.Actor, origin.Id, native.Actor.Id, native.Actor.InstanceId, spell.Id, 2),
			"An admitted live native companion did not pass command authority.");
		Require(control.SetOrder("follow", null, native.Actor) && control.Intent == "follow", "Native follow order was not durably accepted.");
		Require(control.SetOrder("none", null, native.Actor) && control.Intent == "none" && npc.Following is null,
			"Native stop did not persist and cease following.");
		var persistedControl = control.SaveToXml(new()).ToString();
		SpellNpcControlEffect.InitialiseEffectType();
		var restored = (SpellNpcControlEffect)Effect.LoadEffect(System.Xml.Linq.XElement.Parse(persistedControl), npc);
		Require(restored.LifecycleId == origin.Id && restored.CreatorId == native.Actor.Id &&
			restored.CreatorInstanceId == native.Actor.InstanceId && restored.SpellId == spell.Id && restored.Grade == 2 &&
			restored.Intent == "none" && !restored.ControlLost && restored.TargetId is null && restored.TargetInstanceId is null,
			"Cold-restored native control effect did not preserve its exact authority and stopped intent.");
		var wrongCreator = NativeHarnessCharacter.Create(native.World, native.Actor.Id + 1000000, native.Actor.Location,
			native.Actor.Culture);
		var beforeRejectedOrder = control.SaveToXml(new()).ToString();
		Require(!control.SetOrder("follow", null, wrongCreator) && control.SaveToXml(new()).ToString() == beforeRejectedOrder &&
			control.Intent == "none" && npc.Following is null,
			"A wrong creator changed the live or saved native control state.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var row = db.MagicSpellLifecycles.Single(x => x.Id == origin.Id);
			row.State = (int)SpellLifecycleState.Retiring;
			db.SaveChanges();
		}
		Require(!control.SetOrder("follow", null, native.Actor) && control.SaveToXml(new()).ToString() == beforeRejectedOrder &&
			control.Intent == "none" && npc.Following is null,
			"A held lifecycle changed the live or saved native control state.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var row = db.MagicSpellLifecycles.Single(x => x.Id == origin.Id);
			row.State = (int)SpellLifecycleState.Active;
			db.SaveChanges();
		}
		var targetOrigin = origin with { Id = Guid.NewGuid(), Family = "s3-cache-haunt-target" };
		var hauntTarget = (RuntimeNpc)template.CreateSpellOwnedCharacter(native.Actor.SpatialLocation, targetOrigin);
		actors.Add(hauntTarget);
		hauntTarget.AddEffect(new SpellNpcControlEffect(hauntTarget, targetOrigin, native.Actor));
		var fearParent = new MagicSpellParent(hauntTarget, spell, native.Actor) { RemoveWhenLoadedChildrenExpire = true };
		var fearExpiry = new SpellFearEffect(hauntTarget, fearParent, expiryUtc: targetOrigin.DeadlineUtc);
		fearParent.AddSpellEffect(fearExpiry);
		hauntTarget.AddEffect(fearExpiry);
		hauntTarget.AddEffect(fearParent, targetOrigin.DeadlineUtc!.Value - RuntimeClock.UtcNow);
		var unrelatedParent = new MagicSpellParent(hauntTarget, spell, native.Actor);
		var unrelatedFear = new SpellFearEffect(hauntTarget, unrelatedParent, expiryUtc: targetOrigin.DeadlineUtc);
		unrelatedParent.AddSpellEffect(unrelatedFear);
		hauntTarget.AddEffect(unrelatedFear);
		hauntTarget.AddEffect(unrelatedParent, targetOrigin.DeadlineUtc.Value - RuntimeClock.UtcNow);
		var hauntMission = new SpellNpcMissionEffect(npc, "haunt", hauntTarget, expiryUtc: origin.DeadlineUtc);
		SetPrivateMember(hauntMission, "Phase", "haunting");
		SetPrivateMember(hauntMission, "FearParentIdentity", fearParent.Identity);
		npc.AddEffect(hauntMission);
		using (var isolated = FMDB.BeginIndependentScope(requireWrites: true))
		using (var db = new FMDB())
		{
			hauntTarget.Save();
			FMDB.Context.SaveChanges();
		}
		native.World.SaveManager.Flush();
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var targetRow = db.Characters.Single(x => x.Id == hauntTarget.Id);
			targetRow.CurrentProjectHours = 12.5;
			db.SaveChanges();
		}
		SetPrivateField(hauntTarget, "_state", CharacterState.Stasis);
		actors.Remove(hauntTarget);
		var cachedActors = new All<ICharacter>();
		cachedActors.Add(hauntTarget);
		native.WorldMock.SetupGet(x => x.CachedActors).Returns(cachedActors);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var targetRow = db.Characters.AsNoTracking().Single(x => x.Id == hauntTarget.Id);
			var root = System.Xml.Linq.XElement.Parse(targetRow.EffectData);
			var owned = root.Elements("Effect").Where(x => x.Element("Type")?.Value == "MagicSpellParent" &&
				Guid.TryParse(x.Element("Effect")?.Element("Identity")?.Value, out var identity) && identity == fearParent.Identity).ToArray();
			var ownedRoot = owned.SingleOrDefault()?.Element("Effect");
			var primary = db.CharacterInstances.AsNoTracking().Where(x => x.CharacterId == hauntTarget.Id && x.IsPrimary)
				.Select(x => new { x.Id, x.CharacterId, x.BodyId, x.IsPrimary }).ToArray();
			var cachedParent = hauntTarget.EffectsOfType<MagicSpellParent>().Single(x => x.Identity == fearParent.Identity);
			var cachedXml = new System.Xml.Linq.XElement("Effects", cachedParent.SaveToXml(new())).ToString();
			var cleanupType = typeof(SpellNpcControlEffect).GetNestedType("FearParentCleanup", BindingFlags.NonPublic)!;
			var cleanup = Activator.CreateInstance(cleanupType, hauntTarget.Id, hauntTarget.InstanceId, hauntTarget.Body.Id,
				fearParent.Identity, spell.Id, native.Actor.Id, native.Actor.InstanceId)!;
			var validator = typeof(SpellNpcControlEffect).GetMethod("TryRemoveExactFearParentXml", BindingFlags.Static | BindingFlags.NonPublic)!;
			object?[] storedArgs = [targetRow.EffectData, cleanup, null, false];
			object?[] cachedArgs = [cachedXml, cleanup, null, false];
			var storedValid = (bool)validator.Invoke(null, storedArgs)!;
			var cachedValid = (bool)validator.Invoke(null, cachedArgs)!;
			var liveCount = native.World.Actors.Count(x => CharacterInstanceIdentityComparer.IdentityId(x) == hauntTarget.Id && x.InstanceId == hauntTarget.InstanceId);
			var cacheCount = native.World.CachedActors.Count(x => CharacterInstanceIdentityComparer.IdentityId(x) == hauntTarget.Id && x.InstanceId == hauntTarget.InstanceId);
			Console.WriteLine($"ARM-S3-haunt-diagnostic authority:{service.HasCommandAuthority(npc, native.Actor, origin.Id, native.Actor.Id, native.Actor.InstanceId, spell.Id, 2)} target:{CharacterInstanceIdentityComparer.IdentityId(hauntTarget)}/{hauntTarget.InstanceId}/{hauntTarget.Body.Id}/{hauntTarget.IsPrimaryInstance}/{hauntTarget.State} representations:{liveCount}/{cacheCount} primary:{string.Join(',', primary.Select(x => $"{x.Id}/{x.CharacterId}/{x.BodyId}/{x.IsPrimary}"))} parent:{owned.Length}/{ownedRoot?.Element("Spell")}/{ownedRoot?.Element("Caster")}/{ownedRoot?.Element("CasterInstance")}/{ownedRoot?.Element("RemoveWhenLoadedChildrenExpire")}/children:{string.Join(',', ownedRoot?.Element("Children")?.Elements("Effect").Select(x => x.Element("Type")?.Value) ?? [])} storedValidator:{storedValid}/{storedArgs[3]} cachedValidator:{cachedValid}/{cachedArgs[3]}");
			Require(owned.Length == 1 && ownedRoot?.Element("Spell")?.Value == spell.Id.ToString() &&
				ownedRoot.Element("Caster")?.Value == native.Actor.Id.ToString() &&
				ownedRoot.Element("CasterInstance")?.Value == native.Actor.InstanceId.ToString() &&
				ownedRoot.Element("RemoveWhenLoadedChildrenExpire")?.Value == "true" &&
				ownedRoot.Element("Children")?.Elements("Effect").Count() == 1 &&
				ownedRoot.Element("Children")!.Elements("Effect").Single().Element("Type")?.Value == "SpellFear",
				"The exact expected owned fear parent was not persisted before cached cleanup.");
			Require(storedValid && (bool)storedArgs[3]! && cachedValid && (bool)cachedArgs[3]!,
				"The exact fear-parent XML validator rejected stored or cached target state before cleanup.");
		}
		Require(!hauntTarget.Changed && !hauntTarget.EffectsChanged && !native.World.SaveManager.IsQueued(hauntTarget),
			"Cached haunt target was not clean and unqueued before creator stop.");
		Require(control.SetOrder("none", null, native.Actor), "Creator stop rejected a mission with a cached haunting target.");
		Require(!hauntTarget.Changed && !hauntTarget.EffectsChanged && !native.World.SaveManager.IsQueued(hauntTarget),
			"Exact cached fear-parent cleanup dirtied the unrelated cached character state.");
		Require(!hauntTarget.EffectsOfType<MagicSpellParent>().Any(x => x.Identity == fearParent.Identity) &&
			hauntTarget.EffectsOfType<MagicSpellParent>().Any(x => x.Identity == unrelatedParent.Identity),
			"Cached haunt cleanup did not remove only the exact fear parent from memory.");
		native.World.SaveManager.Flush();
		Require(!hauntTarget.Changed && !hauntTarget.EffectsChanged && !native.World.SaveManager.IsQueued(hauntTarget),
			"Cached target remained dirty or queued after the real save-manager flush.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var targetRow = db.Characters.AsNoTracking().Single(x => x.Id == hauntTarget.Id);
			Require(!targetRow.EffectData.Contains(fearParent.Identity.ToString(), StringComparison.Ordinal) &&
				targetRow.EffectData.Contains(unrelatedParent.Identity.ToString(), StringComparison.Ordinal) &&
				targetRow.CurrentProjectHours == 12.5,
				"Cached haunt cleanup did not preserve unrelated target effects and offline scalar state.");
			Require(!npc.EffectsOfType<SpellNpcMissionEffect>().Any(), "Creator stop retained its cached-target haunting mission.");
		}
		// A failed stop transaction must not remove a cached fear in memory or from its durable XML,
		// and a later broad save-manager flush must not publish the uncommitted removal.
		Require(control.SetOrder("guard", null, native.Actor), "Could not establish the failure-case guard intent.");
		native.World.SaveManager.Flush();
		var failedTargetOrigin = origin with { Id = Guid.NewGuid(), Family = "s3-cache-haunt-failed-stop" };
		var failedTarget = (RuntimeNpc)template.CreateSpellOwnedCharacter(native.Actor.SpatialLocation, failedTargetOrigin);
		actors.Add(failedTarget);
		failedTarget.AddEffect(new SpellNpcControlEffect(failedTarget, failedTargetOrigin, native.Actor));
		var failedOwnedParent = new MagicSpellParent(failedTarget, spell, native.Actor) { RemoveWhenLoadedChildrenExpire = true };
		var failedOwnedFear = new SpellFearEffect(failedTarget, failedOwnedParent, expiryUtc: failedTargetOrigin.DeadlineUtc);
		failedOwnedParent.AddSpellEffect(failedOwnedFear);
		failedTarget.AddEffect(failedOwnedFear);
		failedTarget.AddEffect(failedOwnedParent, failedTargetOrigin.DeadlineUtc!.Value - RuntimeClock.UtcNow);
		var failedSiblingParent = new MagicSpellParent(failedTarget, spell, native.Actor);
		var failedSiblingFear = new SpellFearEffect(failedTarget, failedSiblingParent, expiryUtc: failedTargetOrigin.DeadlineUtc);
		failedSiblingParent.AddSpellEffect(failedSiblingFear);
		failedTarget.AddEffect(failedSiblingFear);
		failedTarget.AddEffect(failedSiblingParent, failedTargetOrigin.DeadlineUtc.Value - RuntimeClock.UtcNow);
		var failedMission = new SpellNpcMissionEffect(npc, "haunt", failedTarget, expiryUtc: origin.DeadlineUtc);
		SetPrivateMember(failedMission, "Phase", "haunting");
		SetPrivateMember(failedMission, "FearParentIdentity", failedOwnedParent.Identity);
		var failedMissionIdentity = failedOwnedParent.Identity;
		npc.AddEffect(failedMission);
		using (var isolated = FMDB.BeginIndependentScope(requireWrites: true))
		using (var db = new FMDB())
		{
			failedTarget.Save();
			FMDB.Context.SaveChanges();
			npc.Save();
			FMDB.Context.SaveChanges();
		}
		SetPrivateField(failedTarget, "_state", CharacterState.Stasis);
		actors.Remove(failedTarget);
		cachedActors.Add(failedTarget);
		failedTarget.Changed = true;
		failedTarget.EffectsChanged = true;
		native.World.SaveManager.Add(failedTarget);
		Require(native.World.SaveManager.IsQueued(failedTarget), "Failure-case cached target was not dirty and queued before stop.");
		var triggerName = "arm_s3_stop_fail_" + Guid.NewGuid().ToString("N");
		using (var connection = database.OpenOwnedConnection())
		using (var command = connection.CreateCommand())
		{
			command.CommandText = $"CREATE TRIGGER `{triggerName}` BEFORE UPDATE ON `characters` FOR EACH ROW BEGIN IF NEW.Id = {npc.Id} THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'forced S3 stop transaction failure'; END IF; END";
			command.ExecuteNonQuery();
		}
		var failedStopAccepted = control.SetOrder("none", null, native.Actor);
		var uncertain = (bool)(typeof(SpellNpcControlEffect).GetField("_persistenceUncertain", BindingFlags.Instance | BindingFlags.NonPublic)
			?.GetValue(control) ?? false);
		Require(!failedStopAccepted && uncertain, "Injected durable stop failure was not rejected and latched as uncertain.");
		Require(failedTarget.EffectsOfType<MagicSpellParent>().Any(x => x.Identity == failedOwnedParent.Identity) &&
			failedTarget.EffectsOfType<MagicSpellParent>().Any(x => x.Identity == failedSiblingParent.Identity) &&
			native.World.SaveManager.IsQueued(failedTarget),
			"Failed stop mutated cached fear state or discarded its preexisting queued save.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var npcXml = db.Characters.AsNoTracking().Single(x => x.Id == npc.Id).EffectData;
			var targetXml = db.Characters.AsNoTracking().Single(x => x.Id == failedTarget.Id).EffectData;
			Require(npcXml.Contains("<Intent>guard</Intent>", StringComparison.Ordinal) &&
				npcXml.Contains($"<FearParentIdentity>{failedMissionIdentity}</FearParentIdentity>", StringComparison.Ordinal) &&
				npcXml.Contains($"<TargetId>{failedTarget.Id}</TargetId>", StringComparison.Ordinal),
				"Failed stop partially committed the NPC intent or mission cancellation.");
			Require(targetXml.Contains(failedOwnedParent.Identity.ToString(), StringComparison.Ordinal) &&
				targetXml.Contains(failedSiblingParent.Identity.ToString(), StringComparison.Ordinal),
				"Failed stop partially removed a cached fear parent from durable XML.");
		}
		using (var connection = database.OpenOwnedConnection())
		using (var command = connection.CreateCommand())
		{
			command.CommandText = $"DROP TRIGGER `{triggerName}`";
			command.ExecuteNonQuery();
		}
		native.World.SaveManager.Flush();
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var npcXml = db.Characters.AsNoTracking().Single(x => x.Id == npc.Id).EffectData;
			var targetXml = db.Characters.AsNoTracking().Single(x => x.Id == failedTarget.Id).EffectData;
			Require(npcXml.Contains("<Intent>guard</Intent>", StringComparison.Ordinal) &&
				npcXml.Contains($"<FearParentIdentity>{failedMissionIdentity}</FearParentIdentity>", StringComparison.Ordinal) &&
				npcXml.Contains($"<TargetId>{failedTarget.Id}</TargetId>", StringComparison.Ordinal),
				"Later SaveManager.Flush published a stop that never committed durably.");
			Require(targetXml.Contains(failedOwnedParent.Identity.ToString(), StringComparison.Ordinal) &&
				targetXml.Contains(failedSiblingParent.Identity.ToString(), StringComparison.Ordinal),
				"Later SaveManager.Flush persisted an uncommitted cached fear removal.");
		}
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var saved = db.Characters.AsNoTracking().Single(x => x.Id == npc.Id).EffectData;
			Require(saved.Contains("<Intent>guard</Intent>", StringComparison.Ordinal), "Failed native stop did not preserve the NPC's prior durable guard intent.");
			Require(lifecycleStore.Find(origin.Id)?.Entities.Count == 2, "S3 native lifecycle no longer retained the exact two entity claims.");
		}
		Require(!control.SetOrder("attack", npc, native.Actor), "A lifecycle owner authorized an attack against the controlled NPC itself.");
		Console.WriteLine("ARM-S3-runtime=passed concrete-native-NPC active-lifecycle exact-capability authority follow stop persistence cold-restore wrong-creator held-lifecycle refusal self-target refusal cached-haunt exact-parent cleanup sibling preservation offline-scalar preservation failed-stop rollback cached-queue retention uncertainty hold post-failure-flush preservation");
		return 0;
	}
}
