using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body.Implementations;
using MudSharp.Body.CommunicationStrategies;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.PerceptionEngine;
using MudSharp.Planes;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;

#nullable enable
namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record AreaWound(long Body, double Damage, double Pain, double Stun);
	private sealed record AreaOperation(Guid Id, long Spell, string Definition);
	private sealed record AreaReader(string Database, FixtureIds Fixture, long Language, long Capability, long Trait,
		double Balance, DateTime SkillDeadline, IReadOnlyList<AreaOperation> Operations, IReadOnlyList<AreaWound> Wounds);

	private static int RunAreaAcceptanceChecks()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString);
		var fixture = FixtureSeed.Create(database, "arm_area", false);
		var native = NativeRuntime.Load(fixture, database.ConnectionString, true, vocalAnatomy: true);
		ConfigureCastingWorld(native, database.ConnectionString, true);
		var actor = native.Actor; var world = native.World;
		Mock.Get(native.Body.Race).SetupGet(x => x.CommunicationStrategy).Returns(HumanoidCommunicationStrategy.Instance);
		native.Body.CalculateOrganFunctions(true);
		var language = ConfigureSpeechLanguage(native, database.ConnectionString, true); PrepareSpeechActor(actor, language);
		var room = CreateAreaRoom(native, database.ConnectionString, fixture.RoomId, create: true);
		SetPrivateMember(actor, "Location", room);
		var members = (List<ICharacter>)room.Characters; members.Add(actor);
		var participants = new List<NativeHarnessCharacter> { actor };
		SetPrivateField(actor, "_allyIDs", new HashSet<long>()); SetPrivateField(actor, "_trustedAllyIDs", new HashSet<long>());
		var utterances = new List<SpokenLanguageInfo>(); var texts = new List<string>();
		ConfigureSpeechOutput(actor, utterances, texts); ConfigureSpeechCommands(actor);
		NativeHarnessCharacter Target(string name)
		{
			var target = NewSpeechListener(database, native, "area_" + name, language, true);
			SetPrivateField(target, "_allyIDs", new HashSet<long>()); SetPrivateField(target, "_trustedAllyIDs", new HashSet<long>());
			ConfigureSpeechOutput(target, [], []); participants.Add(target); members.Add(target);
			using var db = NewIndependentContext(database.ConnectionString);
			var body = db.Bodies.Find(target.Body.Id)!; body.BodyPrototypeId = native.Body.Prototype.Id;
			body.RaceId = native.Body.Race.Id; body.EthnicityId = native.Body.Ethnicity.Id;
			db.Characters.Find(target.Id)!.Location = room.Id; db.SaveChanges();
			return target;
		}
		var ally = Target("ally"); actor.SetAlly(ally);
		var protectedActor = Target("protected"); var warded = Target("warded"); var flying = Target("flying");
		flying.PositionState = PositionFlying.Instance;
		var elevated = Target("layer"); elevated.RoomLayer = RoomLayer.InTrees;
		var staffActor = Target("staff"); SetPrivateMember(staffActor, "PermissionLevel", PermissionLevel.JuniorAdmin);
		staffActor.AddEffect(new AdminSight(staffActor));
		var planar = Target("plane");
		var cap = (SkillLevelBasedMagicCapability)native.Capability; var trait = CastingRequired(world.Traits.GetByName("ARM02 Earth Proficiency"));
		long filterId;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var model = new Db.FutureProg { FunctionName = "armAreaProtection", FunctionComment = "Authored area fixture protection; not a stock race/ward binding.",
				FunctionText = $"return @target.id != {protectedActor.Id}", ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(),
				Category = "Harness", Subcategory = "Rejuvenation", StaticType = (int)FutureProgStaticType.NotStatic };
			model.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "target", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
			model.FutureProgsParameters.Add(new() { ParameterIndex = 1, ParameterName = "caster", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
			db.FutureProgs.Add(model); db.SaveChanges(); filterId = model.Id;
		}
		var filter = new FutureProg(world, "armAreaProtection", ProgVariableTypes.Boolean,
			[Tuple.Create(ProgVariableTypes.Character, "target"), Tuple.Create(ProgVariableTypes.Character, "caster")],
			$"return @target.id != {protectedActor.Id}") { Id = filterId, StaticType = FutureProgStaticType.NotStatic };
		Require(filter.Compile(), "Authored native area filter failed: " + filter.CompileError); ((All<IFutureProg>)world.FutureProgs).Add(filter);
		var now = new DateTime(2026, 10, 2, 16, 0, 0, DateTimeKind.Utc); var samples = 0; var draws = 0;
		Action? committedMutation = null;
		var picks = new Queue<int>(); var store = new MagicCastingStateStore(); var operations = new List<AreaOperation>();
		void Flush()
		{
			world.SaveManager.Flush();
			using (new FMDB())
			{
				foreach (var participant in participants) { participant.Save(); participant.Body.Save(); foreach (var t in participant.CharacterTraits) t.Save(); }
				FMDB.Context.SaveChanges();
			}
		}
		var service = new MagicCastingService(world, clock: () => now, random: () => { samples++; return 0.1; }, flush: Flush,
			checkpoint: stage => { if (stage == "Committed") committedMutation?.Invoke(); },
			areaRandom: bound => { draws++; return picks.Dequeue(); }); native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		MagicSpell Author(string name, string policy)
		{
			var spell = new MagicSpell("ARM Area " + name, cap.School); ((All<IMagicSpell>)world.MagicSpells).Add(spell);
			foreach (var command in new[] { "trigger new character", $"trait {trait.Id}", "difficulty easy", "threshold minorpass", "duration ARM02 Duration", "nonexclusivedelay 2",
				$"cost {native.Resource.Id} ARM02 Cost", "prog rejuvenation_known", "castemote The fixture gestures as its area spell forms.",
				"failcastemote The fixture gestures without success.", "targetemote The fixture area spell reaches $1.", "effect add damage", "effect 1 type burning",
				"effect 1 formula grade*4", $"effect 1 bodypart {fixture.BodypartId}", "grades fixture", "grades area fixture " + policy,
				$"grades incantation fixture {language.Id} fm-local fm-earth fm-area fm-{name.ToLowerInvariant()} {name.ToLowerInvariant()}" })
				Require(spell.BuildingCommand(actor, new StringStack(command)), "Area builder refused: " + command);
			Require(spell.ReadyForGame, spell.ReadyForGame ? "Area spell ready." : spell.WhyNotReadyForGame(actor));
			foreach (var command in new[] { $"casting trait {trait.Id}", $"casting resources {native.Resource.Id} {native.Resource.Id} gather",
				$"casting entry add {spell.Id}", $"casting entry starting {spell.Id} on", "casting enable on" })
				Require(cap.BuildingCommand(actor, new StringStack(command)), "Area admission refused: " + command);
			return spell;
		}
		var earthquake = Author("Earthquake", "earthquake"); var chain = Author("Chain", "chainlightning"); var fireball = Author("Fireball", "roomfireball");
		var enrol = new FutureProg(world, "armAreaEnrol", ProgVariableTypes.Boolean,
			[Tuple.Create(ProgVariableTypes.Character, "actor"), Tuple.Create(ProgVariableTypes.MagicCapability, "cap")],
			"return enrolchannelcasting(@actor, @cap, \"native area fixture\")");
		Require(enrol.Compile() && enrol.ExecuteBool(actor, cap), "Authored native area enrolment failed."); actor.SetTraitValue(trait, 62);
		foreach (var spell in new[] { earthquake, chain, fireball }) store.Write(acquired: service.Acquisition(actor, spell.Id)! with { ControlledGrade = 2 });
		Require(earthquake.BuildingCommand(actor, new StringStack($"grades area filter {filter.Id}")), "Source protection filter refused.");
		var wardParent = new MagicSpellParent(warded, earthquake, actor);
		var ward = new SpellPersonalWardEffect(warded, wardParent, cap.School, MagicInterdictionMode.Fail, MagicInterdictionCoverage.Incoming, true, null);
		wardParent.AddSpellEffect(ward); warded.AddEffect(wardParent); warded.AddEffect(ward);
		var planarParent = new MagicSpellParent(planar, earthquake, actor);
		var planeEffect = new SpellPlanarStateEffect(planar, planarParent, PlanarPresenceDefinition.DefaultMaterial(2));
		planarParent.AddSpellEffect(planeEffect); planar.AddEffect(planarParent); planar.AddEffect(planeEffect);
		SpellPersonalWardEffect.InitialiseEffectType(); SpellPlanarStateEffect.InitialiseEffectType(); Flush();
		int Checks() => Mock.Get(world.GetCheck(CheckType.CastSpellCheck)).Invocations.Count(x => x.Method.Name == "CheckAgainstAllDifficulties");
		void Reset()
		{
			now += TimeSpan.FromMinutes(11); actor.RemoveAllEffects<MagicSpellLockout>(); actor.AddResource(native.Resource, 100);
			utterances.Clear(); texts.Clear();
		}
		MagicCastingIntent Intent(MagicSpell spell, bool overreach = true) => new(actor, cap.Id, spell.Id, 3, overreach, "here", MagicCastingMode.Area);
		MagicCastingResult Cast(MagicSpell spell, bool overreach = true)
		{
			var before = Checks(); var beforeSamples = samples;
			var result = service.Cast(Intent(spell, overreach)); Require(result.Status == MagicCastingStatus.Succeeded, result.Message + " " + string.Join(" | ", texts));
			Require(Checks() == before + 1 && samples == beforeSamples + (overreach ? 1 : 0) && actor.MagicResourceAmounts[native.Resource] == (overreach ? 77.5 : 85),
				"Area payment/check/progression was duplicated.");
			Require(utterances.Count == 1 && store.Opportunity(actor.Id, trait.Id)!.NextUtc == now.AddSeconds(60), "Area native utterance or trait deadline duplicated.");
			var operation = store.Operation(result.OperationId!.Value)!; Require(operation.Stage == "Completed", "Area journal not completed.");
			operations.Add(new(operation.Id, spell.Id, operation.Definition)); return result;
		}

		Reset(); var beforeWounds = participants.ToDictionary(x => x.Body.Id, x => AreaWounds(x.Body));
		members.Add(ally); // Duplicate catalogue entry must not duplicate damage.
		var earthquakeQuote = service.Quote(Intent(earthquake)); Require(earthquakeQuote.Allowed && draws == 0, earthquakeQuote.Reason);
		Require(earthquakeQuote.Invocation!.Area!.Candidates.Count == 3, "Earthquake failed source exclusions or deduplication; candidates " +
			string.Join(",", earthquakeQuote.Invocation.Area.Candidates.Select(x => $"{x.CharacterId}/{x.BodyId}")) + "; physical inputs " +
			string.Join(";", participants.Select(x => $"{x.Id}/{x.InstanceId}/{x.Body.Id} layer:{x.RoomLayer} position:{x.PositionState?.Name} staff:{x.IsAdministrator()} magic:{actor.CanInteractPlanar(x, PlanarInteractionKind.Magic)} physical:{actor.CanInteractPlanar(x, PlanarInteractionKind.Physical)} filter:{filter.ExecuteBool(x, actor)}")));
		Cast(earthquake); members.RemoveAt(members.Count - 1);
		Require(AreaDelta(actor, beforeWounds) == 4 && AreaDelta(ally, beforeWounds) == 12 && AreaDelta(warded, beforeWounds) == 0 &&
			new[] { protectedActor, flying, elevated, staffActor, planar }.All(x => AreaDelta(x, beforeWounds) == 0), "Earthquake source ratios/protection/ground/layer/staff/plane changed.");
		Console.WriteLine("ARM-AREA-earthquake=passed concrete-Cell native-wounds caster:4 ally:12 protected/flying/layer/staff/plane:0 ward:0 physical-dedup one-payment/check/mastery");

		Reset(); beforeWounds = participants.ToDictionary(x => x.Body.Id, x => AreaWounds(x.Body));
		var chainQuote = service.Quote(Intent(chain)); Require(chainQuote.Allowed && draws == 0, chainQuote.Reason);
		var candidates = chainQuote.Invocation!.Area!.Candidates.ToList();
		picks.Enqueue(candidates.FindIndex(x => x.BodyId == actor.Body.Id)); picks.Enqueue(candidates.FindIndex(x => x.BodyId == ally.Body.Id)); picks.Enqueue(candidates.FindIndex(x => x.BodyId == ally.Body.Id));
		var chainResult = Cast(chain); var chainReceipt = XElement.Parse(store.Operation(chainResult.OperationId!.Value)!.Definition).Element("Area")!;
		Require(AreaDelta(actor, beforeWounds) == 3 && AreaDelta(ally, beforeWounds) == 24 && draws == 3 &&
			chainReceipt.Elements("Application").Select(x => (long)x.Attribute("body")!).SequenceEqual(new[] { actor.Body.Id, ally.Body.Id, ally.Body.Id }), "Chain source repeated hits/caster quarter changed.");
		Console.WriteLine("ARM-AREA-chain=passed caster:3 ally:24 repeat-body-twice random-at-commit:3 journal-selected-order one-payment/check/mastery provisional-three-hit-count");

		Reset(); beforeWounds = participants.ToDictionary(x => x.Body.Id, x => AreaWounds(x.Body));
		Cast(fireball); Require(AreaDelta(actor, beforeWounds) == 0 && AreaDelta(ally, beforeWounds) == 12 && actor.IsAlly(ally) && AreaDelta(staffActor, beforeWounds) == 0 && AreaDelta(planar, beforeWounds) == 0,
			"Room Fireball excluded an ally, included caster/staff or crossed forbidden planes.");
		Console.WriteLine("ARM-AREA-fireball=passed room-variant caster:0 native-ally:12 staff/plane:0 independent-of-selected-character-trigger");

		var sibling = Target("secondbody");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			db.CharacterInstances.Add(new Db.CharacterInstance { Id = actor.InstanceId + 100000, CharacterId = actor.Id,
				BodyId = sibling.Body.Id, EmbodiedBodyId = sibling.Body.Id, InstanceName = "Area distinct physical body",
				InstanceKind = (int)CharacterInstanceKind.PhysicalClone, ControlPolicy = (int)CharacterInstanceControlPolicy.PlayerFocusable,
				PersistencePolicy = (int)CharacterInstancePersistencePolicy.Persistent, IsEmbodied = true, IsControllable = true,
				LocationId = room.Id, State = (int)CharacterState.Awake, PositionId = (int)PositionStanding.Instance.Id,
				PositionTargetType = "", PositionEmote = "", CreatedBySourceKey = "Area isolated acceptance", CreatedDateTime = now, EffectData = "<Effects/>" });
			db.SaveChanges();
		}
		sibling.HarnessIdentity = actor; SetPrivateField(sibling, "_id", actor.Id); SetPrivateField(sibling, "_instanceId", actor.InstanceId + 100000);
		SetPrivateField(actor, "_secondaryInstances", new List<ICharacterInstance> { sibling });
		Reset(); members.Clear(); members.Add(actor); members.Add(sibling); members.Add(sibling);
		beforeWounds = participants.ToDictionary(x => x.Body.Id, x => AreaWounds(x.Body)); Cast(fireball, overreach: false);
		Require(CharacterInstanceIdentityComparer.SameIdentity(actor, sibling) && actor.InstanceId != sibling.InstanceId &&
			AreaDelta(actor, beforeWounds) == 0 && AreaDelta(sibling, beforeWounds) == 12 && actor.MagicResourceAmounts[native.Resource] == 85,
			"Physical area conflated canonical sibling bodies or charged a second shared reserve.");
		Console.WriteLine("ARM-AREA-second-body=passed shared-canonical-identity distinct-instance/body caster:0 sibling:12 duplicate-sibling-once canonical-payment:15");
		members.Clear(); members.AddRange(participants);

		// Membership changes at payment commitment occur after selection but before any damage.
		var removed = Target("removed"); var arrival = Target("arrival"); members.RemoveAll(x => ReferenceEquals(x, arrival));
		Require(fireball.BuildingCommand(actor, new StringStack("grades area include allies false")), "Explicit ally exclusion refused.");
		var changed = false;
		committedMutation = () =>
		{
			if (!changed) { changed = true; members.RemoveAll(x => ReferenceEquals(x, removed)); members.Add(arrival); }
		};
		Reset(); beforeWounds = participants.ToDictionary(x => x.Body.Id, x => AreaWounds(x.Body)); Cast(fireball, overreach: false);
		Require(changed && AreaDelta(removed, beforeWounds) == 0 && AreaDelta(arrival, beforeWounds) == 0 && AreaDelta(ally, beforeWounds) == 0,
			"Dynamic area appended arrivals or damaged removed/explicitly excluded actors.");
		committedMutation = null;
		Console.WriteLine("ARM-AREA-dynamic=passed paid-snapshot removed:0 new-arrival:0 explicit-ally-exclusion:0 concrete-cell-list-mutation-bounded");

		Reset(); members.Clear(); members.Add(actor); members.Add(warded); picks.Enqueue(0); picks.Enqueue(0); picks.Enqueue(0);
		Require(chain.BuildingCommand(actor, new StringStack("grades area include caster false")), "Explicit chain caster exclusion refused.");
		warded.RemoveEffect(wardParent, true);
		var reflectionParent = new MagicSpellParent(warded, chain, actor);
		var reflectionWard = new SpellPersonalWardEffect(warded, reflectionParent, cap.School, MagicInterdictionMode.Reflect, MagicInterdictionCoverage.Incoming, true, null);
		reflectionParent.AddSpellEffect(reflectionWard); warded.AddEffect(reflectionParent); warded.AddEffect(reflectionWard);
		beforeWounds = participants.ToDictionary(x => x.Body.Id, x => AreaWounds(x.Body)); var priorSamples = samples; var priorChecks = Checks();
		var rejected = service.Cast(new(actor, cap.Id, chain.Id, 4, true, "here", MagicCastingMode.Area));
		Require(rejected.Status == MagicCastingStatus.Failed && samples == priorSamples && Checks() == priorChecks + 1 &&
			actor.MagicResourceAmounts[native.Resource] == 70 && actor.EffectsOfType<MagicSpellLockout>().Any() &&
			AreaDelta(actor, beforeWounds) == 0 && AreaDelta(warded, beforeWounds) == 0 && service.Acquisition(actor, chain.Id)!.ControlledGrade == 3,
			$"All-rejected area lost payment/lockout, reflected recursively or advanced mastery. status:{rejected.Status} message:{rejected.Message} samples:{samples}/{priorSamples} checks:{Checks()}/{priorChecks} balance:{actor.MagicResourceAmounts[native.Resource]} lockouts:{actor.EffectsOfType<MagicSpellLockout>().Count()} casterDamage:{AreaDelta(actor, beforeWounds)} wardedDamage:{AreaDelta(warded, beforeWounds)} grade:{service.Acquisition(actor, chain.Id)!.ControlledGrade}");
		var rejectedOp = store.Operation(rejected.OperationId!.Value)!; operations.Add(new(rejectedOp.Id, chain.Id, rejectedOp.Definition));
		Console.WriteLine("ARM-AREA-all-rejected=passed native-reflect-ward-becomes-fail depth:0 three-selected-hits cost:30 lockout-retained no-native-wound/mastery");

		Reset(); members.Clear(); members.Add(actor); var balance = actor.MagicResourceAmounts[native.Resource]; var count = Checks();
		var empty = service.Cast(Intent(fireball, false)); Require(empty.Status == MagicCastingStatus.Refused && Checks() == count && actor.MagicResourceAmounts[native.Resource] == balance,
			"Empty pre-payment target set paid or checked.");
		Console.WriteLine("ARM-AREA-empty=passed no-payment/check/operation for caster-excluding-empty-room");
		members.AddRange(participants.Skip(1)); Flush();
		var woundReceipts = participants.Select(x => AreaWounds(x.Body)).ToArray();
		using (var read = NewIndependentContext(database.ConnectionString))
			foreach (var expected in woundReceipts)
			{
				var rows = read.Wounds.AsNoTracking().Where(x => x.BodyId == expected.Body).ToArray();
				Require(Same(rows.Sum(x => x.CurrentDamage), expected.Damage) && Same(rows.Sum(x => x.CurrentPain), expected.Pain) && Same(rows.Sum(x => x.CurrentStun), expected.Stun), "Native area wound channels did not persist.");
			}
		RunAreaReaderProcess(new(database.Name, fixture, language.Id, cap.Id, trait.Id, balance,
			store.Opportunity(actor.Id, trait.Id)!.NextUtc, operations.AsReadOnly(), woundReceipts));
		Console.WriteLine("ARM-AREA-persistence=passed native-damage/pain/stun independently-read all-completed-application-journals immutable-paid-origin-restart");
		return 0;
	}

	private static AreaWound AreaWounds(MudSharp.Body.IBody body) => new(body.Id, body.Wounds.Sum(x => x.CurrentDamage), body.Wounds.Sum(x => x.CurrentPain), body.Wounds.Sum(x => x.CurrentStun));
	private static double AreaDelta(ICharacter actor, IReadOnlyDictionary<long, AreaWound> before) => actor.Wounds.Sum(x => x.CurrentDamage) - before[actor.Body.Id].Damage;

	private static Room CreateAreaRoom(NativeRuntime native, string connection, long cellId, bool create)
	{
		RecentSpeechContextEffect.InitialiseEffectType();
		using var db = NewIndependentContext(connection);
		if (create)
		{
			var package = new Db.RoomOverlayPackage { Id = 930001, Name = "Area fixture overlay", RevisionNumber = 1,
				EditableItem = new Db.EditableItem { RevisionNumber = 1, RevisionStatus = (int)RevisionStatus.Current, BuilderAccountId = 1, BuilderDate = DateTime.UtcNow } };
			var terrain = new Db.Terrain { Id = 930001, Name = "Area fixture terrain", TerrainBehaviourMode = "outdoors", MovementRate = 1 };
			db.RoomOverlayPackages.Add(package); db.Terrains.Add(terrain); db.SaveChanges();
			var overlay = new Db.RoomOverlay { Id = 930001, Name = "Area fixture", RoomName = "A disposable area acceptance cell",
				RoomDescription = "An isolated test cell.", RoomId = cellId, RoomOverlayPackageId = package.Id, RoomOverlayPackageRevisionNumber = 1,
				TerrainId = terrain.Id, AmbientLightFactor = 1, SafeQuit = true };
			db.RoomOverlays.Add(overlay); db.SaveChanges(); db.Rooms.Find(cellId)!.CurrentOverlayId = overlay.Id; db.SaveChanges();
		}
		var model = db.Rooms.Include(x => x.RoomOverlays).Include(x => x.RoomsMagicResources).AsNoTracking().Single(x => x.Id == cellId);
		var terrains = new All<ITerrain>(); terrains.Add(new Terrain(db.Terrains.AsNoTracking().Single(x => x.Id == model.RoomOverlays.Single().TerrainId), native.World));
		native.WorldMock.SetupGet(x => x.Terrains).Returns(terrains);
		var overlayPackage = new Mock<IRoomOverlayPackage>(); overlayPackage.SetupGet(x => x.Id).Returns(model.RoomOverlays.Single().RoomOverlayPackageId);
		overlayPackage.SetupGet(x => x.RevisionNumber).Returns(1); overlayPackage.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		var packages = new RevisableAll<IRoomOverlayPackage>(); packages.Add(overlayPackage.Object); native.WorldMock.SetupGet(x => x.RoomOverlayPackages).Returns(packages);
		var zone = new Mock<IZone>(); zone.SetupGet(x => x.Gameworld).Returns(native.World);
		zone.SetupGet(x => x.Id).Returns(model.ZoneId);
		var room = new Room(model, zone.Object); var rooms = new All<IRoom>(); rooms.Add(room); native.WorldMock.SetupGet(x => x.Rooms).Returns(rooms); room.PostLoadTasks(model);
		return room;
	}

	private static void RunAreaReaderProcess(AreaReader input)
	{
		var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
		start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); start.ArgumentList.Add("--area-reader");
		start.ArgumentList.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(input)))); using var process = Process.Start(start)!;
		var output = process.StandardOutput.ReadToEndAsync(); var error = process.StandardError.ReadToEndAsync();
		if (!process.WaitForExit(60000)) { process.Kill(true); throw new TimeoutException("Area reader exceeded 60 seconds."); }
		Console.Write(output.GetAwaiter().GetResult()); Require(process.ExitCode == 0, error.GetAwaiter().GetResult());
	}
	private static int RunAreaReader(string[] args)
	{
		var input = JsonSerializer.Deserialize<AreaReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args.Single())))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var native = NativeRuntime.Load(input.Fixture, database.ConnectionString, true, vocalAnatomy: true); ConfigureCastingWorld(native, database.ConnectionString, false);
		var language = ConfigureSpeechLanguage(native, database.ConnectionString, false); PrepareSpeechActor(native.Actor, language);
		var room = CreateAreaRoom(native, database.ConnectionString, input.Fixture.RoomId, create: false); SetPrivateMember(native.Actor, "Location", room);
		var service = new MagicCastingService(native.World, flush: () => throw new InvalidOperationException("Restart replay flushed"), areaRandom: _ => throw new InvalidOperationException("Restart replay selected again"));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service); var store = new MagicCastingStateStore();
		Require(native.Actor.MagicResourceAmounts[native.Resource] == input.Balance && store.Opportunity(native.Actor.Id, input.Trait)!.NextUtc == input.SkillDeadline, "Area balance/deadline changed on reload.");
		foreach (var expected in input.Operations)
		{
			var op = store.Operation(expected.Id)!; var spell = (MagicSpell)CastingRequired(native.World.MagicSpells.Get(expected.Spell));
			Require(op.Stage == "Completed" && op.Definition == expected.Definition && spell.GradeProfile?.Area is not null && spell.GradeConfigurationErrors().Count == 0,
				"Area profile or immutable receipt did not reload.");
			Require(service.Cast(new(native.Actor, input.Capability, expected.Spell, 3, false, "here", MagicCastingMode.Area, OriginId: expected.Id)).Status == MagicCastingStatus.Refused,
				"Area paid origin replay was not refused.");
		}
		using var db = NewIndependentContext(database.ConnectionString);
		foreach (var expected in input.Wounds)
		{
			var model = db.Bodies.Include(x => x.Wounds).ThenInclude(x => x.Infections).AsNoTracking().Single(x => x.Id == expected.Body);
			var actor = expected.Body == native.Body.Id ? native.Actor : NativeHarnessCharacter.Create(native.World,
				db.Characters.AsNoTracking().Single(x => x.BodyId == expected.Body).Id, room, native.Actor.Culture);
			var body = expected.Body == native.Body.Id ? native.Body : new Body(model, native.World, actor); actor.AttachBody(body);
			var actual = AreaWounds(body); Require(Same(actual.Damage, expected.Damage) && Same(actual.Pain, expected.Pain) && Same(actual.Stun, expected.Stun), "Area native body/wound reconstruction drifted.");
		}
		Console.WriteLine($"ARM-AREA-reader=passed process:{Environment.ProcessId} concrete-cell native-bodies:{input.Wounds.Count} immutable-completed-origins:{input.Operations.Count} no-selection/replay");
		return 0;
	}
}
