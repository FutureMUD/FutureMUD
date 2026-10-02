using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Commands.Modules;
using MudSharp.Communication.Language;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Generators;
using MudSharp.RPG.Checks;
using MySql.Data.MySqlClient;
using Db = MudSharp.Models;

#nullable enable
namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static T CastingRequired<T>(T? value, [CallerArgumentExpression(nameof(value))] string? expression = null) where T : class =>
		value ?? throw new InvalidOperationException($"Missing ARM02 fixture reference: {expression}");

	private sealed record CastingReader(string Database, FixtureIds Earth, FixtureIds Sorcerer, long Spell,
		long EarthCapability, long SorcererCapability, long EarthSkill, long SorcererSkill, Guid? Operation,
		int Grade, double Balance, int Unresolved, bool VerifyEffects = false, FixtureIds? SecondBody = null, long? SecondInstance = null,
		DateTime? SkillDeadline = null, DateTime? MasteryDeadline = null, double RawSkill = 42, bool VerifyCapLoss = false,
		long? SupportTrait = null, long? IdentifySpell = null, Guid? SupportGrantKey = null, bool SupportCapRemoved = false,
		long? CapacityAttribute = null, long? CapacityExpression = null, double? Capacity = null, bool CapacityRaw = true);

	private static int RunAllCastingAcceptanceChecks()
	{
		var baseline = RunCastingAcceptanceChecks();
		if (baseline != 0) return baseline;
		var progression = RunCompletionProgressionAcceptanceChecks();
		if (progression != 0) return progression;
		var support = RunSupportProgressionAcceptanceChecks();
		return support == 0 ? RunCapacityAcceptanceChecks() : support;
	}

	private static int RunCastingAcceptanceChecks()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		Console.WriteLine($"ARM02-created={database.Name}");
		ConfigureNativeDatabase(database.ConnectionString);
		using (var schema = NewIndependentContext(database.ConnectionString))
		{
			schema.Database.Migrate();
			Require(!schema.Database.GetPendingMigrations().Any(), "ARM02 schema has pending migrations.");
			Require(!schema.Database.HasPendingModelChanges(), "ARM02 model/migration mismatch.");
			Require(!schema.CharacterAcquiredSpells.Any() && !schema.CharacterCastingEnrolments.Any() &&
				!schema.CharacterMagicSkillOpportunities.Any() && !schema.MagicCastingOperations.Any(),
				"New casting state must be empty after importing the maintained blank snapshot.");
			Console.WriteLine("ARM02-schema=passed blank-snapshot-import empty-casting-tables generated-migration-and-model-parity");
		}
		var earthFixture = FixtureSeed.Create(database, "arm02_earth", false);
		var sorcererFixture = FixtureSeed.Create(database, "arm02_sorcerer", false);
		var native = NativeRuntime.Load(earthFixture, database.ConnectionString, true);
		ConfigureCastingWorld(native, database.ConnectionString, true);
		var world = native.World;
		var actor = native.Actor;
		var earthSkill = CastingRequired(world.Traits.GetByName("ARM02 Earth Proficiency"));
		var sorcererSkill = CastingRequired(world.Traits.GetByName("ARM02 Sorcerer Proficiency"));
		var agility = CastingRequired(world.Traits.GetByName("ARM02 Agility"));
		actor.AddTrait(agility, 10);
		var earth = (SkillLevelBasedMagicCapability)native.Capability;
		var source = native.Resource;
		var sorcererResource = CastingRequired(world.MagicResources.Get(sorcererFixture.ResourceId));
		var spell = new MagicSpell("ARM02 Stone Skin", earth.School);
		((All<IMagicSpell>)world.MagicSpells).Add(spell);
		var known = CastingRequired(world.FutureProgs.GetByName("rejuvenation_known"));
		foreach (var command in new[] { "trigger new self", $"trait {earthSkill.Id}", "difficulty easy", "threshold minorpass",
			"duration ARM02 Duration", $"cost {source.Id} ARM02 Cost", $"prog {known.Id}", "castemote A stone shell forms.",
			"failcastemote The stone shell crumbles.", "effect add spellarmour", "effect 1 absorb grade*10+variable",
			"effect add boost", $"effect 2 trait {agility.Id}", "effect 2 bonus 0", "grades fixture",
			"grades scalar add target 1 boost Bonus -grade" })
			Require(spell.BuildingCommand(actor, new StringStack(command)), $"ARM02 builder refused {command}");
		Require(spell.ReadyForGame, spell.ReadyForGame ? "" : spell.WhyNotReadyForGame(actor));
		foreach (var command in new[] { $"casting trait {earthSkill.Id}", $"casting resources {source.Id} {source.Id} passive",
			$"casting entry add {spell.Id}", $"casting entry starting {spell.Id} on", "casting enable on" })
			Require(earth.BuildingCommand(actor, new StringStack(command)), $"ARM02 capability builder refused {command}");
		var generator = new LinearTimeBasedGenerator(world, "ARM02 Earth Passive", source) { AmountPerMinute = 2 };
		((All<IMagicResourceRegenerator>)world.MagicResourceRegenerators).Add(generator);
		Require(generator.BuildingCommand(actor, new StringStack("conscious")), "Generator conscious policy failed.");
		Require(generator.BuildingCommand(actor, new StringStack("restmultiplier 2")), "Generator resting policy failed.");
		Require(earth.BuildingCommand(actor, new StringStack($"regenerator {generator.Id}")), "Earth generator binding failed.");
		var sorcerer = (SkillLevelBasedMagicCapability)earth.Clone("ARM02 Sorcerer");
		((All<IMagicCapability>)world.MagicCapabilities).Add(sorcerer);
		Require(sorcerer.BuildingCommand(actor, new StringStack($"casting trait {sorcererSkill.Id}")), "Sorcerer trait binding failed.");
		Require(sorcerer.BuildingCommand(actor, new StringStack($"casting resources {source.Id} {sorcererResource.Id} gather")), "Sorcerer reserve binding failed.");
		Require(earth.CastingPolicy!.Identity != sorcerer.CastingPolicy!.Identity && earth.CastingPolicy.Admissions.Single().Key != sorcerer.CastingPolicy.Admissions.Single().Key,
			"Capability clone copied local identities.");
		var secondAcquisition = new MagicSpell(spell, "ARM02 Secondary Acquisition");
		((All<IMagicSpell>)world.MagicSpells).Add(secondAcquisition);
		Require(earth.BuildingCommand(actor, new StringStack($"casting entry add {secondAcquisition.Id}")), "Second-body admission failed.");
		Require(earth.BuildingCommand(actor, new StringStack($"casting entry trait {secondAcquisition.Id} {CastingRequired(world.Traits.GetByName("ARM02 Secondary Proficiency")).Id}")), "Second-body proficiency binding failed.");
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(earth), NativeRuntime.NewCapabilityMerit(sorcerer)]);
		world.SaveManager.Flush();
		var staff = new Mock<ICharacter>(); staff.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true); staff.SetupGet(x => x.Id).Returns(999);
		var now = new DateTime(2026, 9, 27, 2, 0, 0, DateTimeKind.Utc);
		var rolls = 0;
		var service = new MagicCastingService(world, clock: () => now, random: () => { rolls++; return 0.1; }, flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		Require(!actor.IsAdministrator(), "Native casting fixture must remain non-admin.");
		Require(service.Enrol(staff.Object, actor, earth.Id, "ARM02 disposable enrolment").Allowed, "Earth enrolment failed.");
		Require(service.Enrol(staff.Object, actor, sorcerer.Id, "ARM02 disposable alternate route").Allowed, "Sorcerer enrolment failed.");
		Require(actor.GetTrait(earthSkill) is Skill && actor.TraitRawValue(earthSkill) == 10, "Acquisition did not open the real native Skill.");
		actor.SetTraitValue(earthSkill, 42); actor.SetTraitValue(sorcererSkill, 62);
		actor.AddResource(source, 100); actor.AddResource(sorcererResource, 100); FlushCasting(native);
		var store = new MagicCastingStateStore();
		store.Write(acquired: service.Acquisition(actor, spell.Id)! with { ControlledGrade = 2 });
		var quote = service.Quote(new(actor, earth.Id, spell.Id, 3, true, "self"));
		Require(quote.Allowed && quote.Invocation!.Costs.Single().Amount == 22.5, "Native grade-three quote must be 22.5.");
		MagicModule.MagicGeneric(actor, $"{earth.School.SchoolVerb} cast \"{spell.Name}\" grade 3 overreach on self via {earth.Id}");
		Require(service.Acquisition(actor, spell.Id)!.ControlledGrade == 3 && actor.MagicResourceAmounts[source] == 77.5 && rolls == 1,
			"Native player command did not pay and advance exactly once.");
		FlushCasting(native);
		using (var read = NewIndependentContext(database.ConnectionString))
		{
			Require(read.CharacterTraits.AsNoTracking().Single(x => x.CharacterId == actor.Id && x.TraitDefinitionId == earthSkill.Id).Value == 42,
				"Exact native skill did not persist.");
			Require(read.CharacterCastingEnrolments.Count(x => x.CharacterId == actor.Id) == 2, "Missing canonical enrolments.");
			Require(read.CharacterMagicSkillOpportunities.Single(x => x.CharacterId == actor.Id && x.TraitDefinitionId == earthSkill.Id).NextOpportunityUtc == now.AddSeconds(60), "Skill clock mismatch.");
			Require(read.CharacterAcquiredSpells.Single(x => x.CharacterId == actor.Id).NextMasteryUtc == now.AddSeconds(600), "Mastery clock mismatch.");
			var receipt = read.MagicCastingOperations.AsNoTracking().Single(x => x.CharacterId == actor.Id);
			Require(receipt.Stage == "Completed" && receipt.BodyId == actor.Body.Id && receipt.TraitDefinitionId == earthSkill.Id,
				"The paid receipt lost its completed native body/trait binding.");
			Console.WriteLine($"ARM02-receipt=passed operation:{receipt.Id} character:{receipt.CharacterId} actor:{receipt.ActorId} body:{receipt.BodyId} capability:{receipt.MagicCapabilityId} spell:{receipt.MagicSpellId} trait:{receipt.TraitDefinitionId} reserve:{receipt.ReserveId} stage:{receipt.Stage}");
		}
		var descriptor = new CastingReader(database.Name, earthFixture, sorcererFixture, spell.Id, earth.Id, sorcerer.Id, earthSkill.Id, sorcererSkill.Id, null, 3, 77.5, 0);
		RunCastingReaderProcess(descriptor);
		Console.WriteLine("ARM02-native-cast=passed non-admin-command real-Skill real-Body payment:22.5 grade:3 separate-process-reload");
		VerifyCastingCharacterAndLifecycle(database, native, sorcererFixture, descriptor, now);
		var shared = new MagicSpell(spell, "ARM02 Shared Trait Sentinel");
		((All<IMagicSpell>)world.MagicSpells).Add(shared);
		foreach (var capability in new[] { earth, sorcerer })
		{
			Require(capability.BuildingCommand(actor, new StringStack($"casting entry add {shared.Id}")), "Shared spell admission failed.");
			Require(service.Grant(staff.Object, actor, capability.Id, shared.Id, "ARM02 shared-trait quarantine fixture").Allowed, "Shared spell grant failed.");
		}
		world.SaveManager.Flush();

		// Fault stages all use the production MySQL receipt/progress store. Reconciliation is acknowledgement only.
		foreach (var checkpoint in new[] { "BeforePayment", "PaymentMutated", "EffectsExecuted", "MasterySampledBeforeWrite", "MasterySampleRecorded" })
		{
			now = now.AddHours(1); actor.RemoveAllEffects<MagicSpellParent>(null, true); actor.RemoveAllEffects<MagicSpellLockout>(null, true);
			actor.AddResource(source, 100); FlushCasting(native);
			store.Write(acquired: store.Acquisition(actor.Id, spell.Id)! with { ControlledGrade = 2, NextMasteryUtc = DateTime.UnixEpoch });
			service = new MagicCastingService(world, clock: () => now, random: () => { rolls++; return 0.1; },
				checkpoint: s => { if (s == checkpoint) throw new IOException("ARM02 injected failure " + s); }, flush: () => FlushCasting(native));
			native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
			var failure = service.Cast(new(actor, earth.Id, spell.Id, 3, true, "self"));
			Require(failure.Status == (checkpoint == "BeforePayment" ? MagicCastingStatus.Refused : MagicCastingStatus.NeedsReview), failure.Message);
			FlushCasting(native);
			var expectedBalance = checkpoint == "BeforePayment" ? 100.0 : 77.5;
			Require(actor.MagicResourceAmounts[source] == expectedBalance, "Fault payment mismatch.");
			RunCastingReaderProcess(descriptor with { Operation = failure.OperationId, Grade = 2, Balance = expectedBalance, Unresolved = failure.OperationId.HasValue ? 1 : 0 });
			if (failure.OperationId is { } id)
			{
				var samples = rolls;
				Require(service.Preflight(actor, sorcerer.Id, spell.Id, 3, true) is not null, "Alternate route escaped uncertainty.");
				Require(service.QuarantineReason(actor, traitId: earthSkill.Id) is not null && service.QuarantineReason(actor, reserveId: source.Id) is not null,
					"Touched trait/reserve was not quarantined.");
				actor.RemoveAllEffects<MagicSpellLockout>(null, true);
				Require(service.Cast(new(actor, earth.Id, shared.Id, 1, false, "self")).Status == MagicCastingStatus.Refused,
					"A second spell escaped shared trait/reserve uncertainty.");
				Require(service.Quote(new(actor, sorcerer.Id, shared.Id, 1, false, "self")).Allowed, "An untouched alternate trait/reserve was incorrectly quarantined.");
				Require(service.ReconcileOperation(staff.Object, actor, id, "Audited the isolated native fixture").Allowed, "Could not reconcile.");
				Require(rolls == samples && actor.MagicResourceAmounts[source] == expectedBalance, "Recovery rerolled or refunded.");
				Require(service.Acquisition(actor, spell.Id)!.ControlledGrade == (checkpoint == "MasterySampleRecorded" ? 3 : 2), "Recovered an unproven sample or lost a durable sample.");
			}
			Console.WriteLine($"ARM02-fault=passed stage:{checkpoint} status:{failure.Status} operation:{failure.OperationId?.ToString() ?? "none"} no-replay-no-refund-no-reroll");
		}
		// An actual provider diagnostic at the outcome-write boundary, after a sampled success.
		using (var connection = database.OpenOwnedConnection())
		using (var trigger = connection.CreateCommand())
		{
			trigger.CommandText = "CREATE TRIGGER arm02_reject_sample BEFORE UPDATE ON MagicCastingOperations FOR EACH ROW BEGIN IF NEW.Stage='MasterySampleRecorded' THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='ARM02 controlled provider sample failure'; END IF; END";
			trigger.ExecuteNonQuery();
		}
		now = now.AddHours(1); actor.RemoveAllEffects<MagicSpellParent>(null, true); actor.AddResource(source, 100);
		store.Write(acquired: store.Acquisition(actor.Id, spell.Id)! with { ControlledGrade = 2, NextMasteryUtc = DateTime.UnixEpoch });
		service = new(world, clock: () => now, random: () => 0.1, flush: () => FlushCasting(native)); native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		var providerFailure = service.Cast(new(actor, earth.Id, spell.Id, 3, true, "self"));
		Require(providerFailure.Status == MagicCastingStatus.NeedsReview, providerFailure.Message);
		var providerOperation = store.Operation(providerFailure.OperationId!.Value)!;
		Require(providerOperation.Diagnostic.Contains("ARM02 controlled provider sample failure"), "Real provider diagnostic was not retained.");
		Console.WriteLine($"ARM02-provider=passed operation:{providerOperation.Id} {providerOperation.Diagnostic}");
		RunCastingReaderProcess(descriptor with { Operation = providerOperation.Id, Grade = 2, Balance = 77.5, Unresolved = 1 });
		Require(service.ReconcileOperation(staff.Object, actor, providerOperation.Id, "Provider failure inspected").Allowed, "Provider reconciliation failed.");
		using (var connection = database.OpenOwnedConnection())
		using (var command = connection.CreateCommand()) { command.CommandText = "DROP TRIGGER arm02_reject_sample"; command.ExecuteNonQuery(); }
		VerifyCastingAuxiliaryFixtures(native, database.ConnectionString, staff.Object, spell, earth, sorcererSkill);
		Console.WriteLine("ARM02-native=passed");
		return 0;
	}

	private static void ConfigureCastingWorld(NativeRuntime runtime, string connection, bool create)
	{
		// Reuse the existing native spell catalogue/compiled-Prog substrate; no environmental action is started.
		ConfigureRejuvenationSpellWorld(runtime, connection, false);
		var world = runtime.World; var mock = runtime.WorldMock;
		mock.SetupGet(x => x.Characters).Returns(new All<ICharacter>());
		mock.SetupGet(x => x.Languages).Returns(new All<ILanguage>()); mock.SetupGet(x => x.SignedLanguages).Returns(new All<ISignedLanguage>());
		mock.SetupGet(x => x.DefaultPlane).Returns((MudSharp.Planes.IPlane)null!);
		using var db = NewIndependentContext(connection);
		var schools = (All<IMagicSchool>)world.MagicSchools;
		foreach (var school in db.MagicSchools.AsNoTracking().Where(x => x.Id != runtime.Capability.School.Id))
			schools.Add(NativeRuntime.NewMagicSchool(school.Id, world).Object);
		if (create)
		{
			foreach (var pair in new[] { ("ARM02 Cost", "5*grade"), ("ARM02 Duration", "600+variable+10*grade") })
				db.TraitExpressions.Add(new() { Name = pair.Item1, Expression = pair.Item2 });
			db.SaveChanges();
			foreach (var name in new[] { "ARM02 Earth Proficiency", "ARM02 Sorcerer Proficiency", "ARM02 Secondary Proficiency" })
				db.TraitDefinitions.Add(new() { Name = name, Type = (int)TraitType.Skill, OwnerScope = (int)TraitOwnerScope.Character,
					Alias = "", TraitGroup = "ARM02", ChargenBlurb = "", ValueExpression = "100", ExpressionId = runtime.World.TraitExpressions.First().Id });
			db.TraitDefinitions.Add(new() { Name = "ARM02 Agility", Type = (int)TraitType.Attribute, OwnerScope = (int)TraitOwnerScope.Body,
				Alias = "agi", TraitGroup = "ARM02", ChargenBlurb = "Disposable native fixture agility.", ValueExpression = "100" });
			db.SaveChanges();
		}
		var expressions = new All<ITraitExpression>();
		foreach (var model in db.TraitExpressions.Include(x => x.TraitExpressionParameters).AsNoTracking()) expressions.Add(new TraitExpression(model, world));
		mock.SetupGet(x => x.TraitExpressions).Returns(expressions);
		var traits = new All<ITraitDefinition>();
		foreach (var model in db.TraitDefinitions.AsNoTracking().Where(x => x.TraitGroup == "ARM02"))
		{
			TraitDefinition definition = model.Type == (int)TraitType.Attribute ? new AttributeDefinition(model, world) : new SkillDefinition(model, world);
			definition.Initialise(model); traits.Add(definition);
		}
		mock.SetupGet(x => x.Traits).Returns(traits);
		var loadedSkills = db.CharacterTraits.AsNoTracking().Where(x => x.CharacterId == runtime.Actor.Id).ToArray()
			.Select(x => CastingRequired(traits.Get(x.TraitDefinitionId)).LoadTrait(new Db.Trait { Value = x.Value, AdditionalValue = x.AdditionalValue }, runtime.Actor)).ToList();
		SetPrivateField(runtime.Actor, "_characterTraits", loadedSkills);
		var bodyTraits = db.Traits.AsNoTracking().Where(x => x.BodyId == runtime.Body.Id).ToArray()
			.Select(x => CastingRequired(traits.Get(x.TraitDefinitionId)).LoadTrait(x, runtime.Body)).ToList();
		SetPrivateField(runtime.Body, "_traits", bodyTraits);
		var resources = new All<IMagicResource>();
		foreach (var model in db.MagicResources.AsNoTracking()) resources.Add(world.MagicResources.Get(model.Id) ?? new CappedSimpleMagicResource(model, world));
		mock.SetupGet(x => x.MagicResources).Returns(resources);
		var armourTypes = new All<IArmourType>(); mock.SetupGet(x => x.ArmourTypes).Returns(armourTypes);
		if (create) armourTypes.Add(new ArmourType(world, "ARM02 Stone Armour"));
		else foreach (var model in db.ArmourTypes.AsNoTracking()) armourTypes.Add(new ArmourType(model, world));
		var materials = new All<ISolid>(); mock.SetupGet(x => x.Materials).Returns(materials);
		if (create) materials.Add(new Solid("ARM02 Stone", MaterialBehaviourType.Stone, world));
		else foreach (var model in db.Materials.AsNoTracking()) materials.Add(new Solid(model, world));
		mock.SetupGet(x => x.AlwaysTrueProg).Returns(CastingRequired(world.FutureProgs.GetByName("rejuvenation_known")));
		SpellArmourProtectionEffect.InitialiseEffectType(); SpellTraitBoostEffect.InitialiseEffectType();
		var generators = (All<IMagicResourceRegenerator>)world.MagicResourceRegenerators;
		foreach (var model in db.MagicGenerators.AsNoTracking().Where(x => x.Name.StartsWith("ARM02")))
			generators.Add(new LinearTimeBasedGenerator(model, world));
		var capabilities = (All<IMagicCapability>)world.MagicCapabilities;
		foreach (var model in db.MagicCapabilities.AsNoTracking().Where(x => x.Id != runtime.Capability.Id))
			capabilities.Add(MagicCapabilityFactory.LoadCapability(model, world));
		// The primary capability was loaded before its generator catalogue. Reload that reference's generator list too.
		SetPrivateField(runtime.Capability, "_resourceRegenerators", generators.Where(x =>
			System.Xml.Linq.XElement.Parse(db.MagicCapabilities.Find(runtime.Capability.Id)!.Definition).Element("Regenerators")?.Elements()
				.Any(e => e.Value == x.Id.ToString()) == true).ToList());
		foreach (var model in db.MagicSpells.AsNoTracking()) ((All<IMagicSpell>)world.MagicSpells).Add(new MagicSpell(model, world));
		SetPrivateField(runtime.Actor, "_positionState", MudSharp.Body.Position.PositionStates.PositionStanding.Instance);
		var check = new Mock<ICheck>();
		check.SetupGet(x => x.MaximumDifficultyForImprovement).Returns(Difficulty.Impossible);
		check.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(),
			It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(() => Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.CastSpellCheck, Outcome.Pass)));
		mock.Setup(x => x.GetCheck(It.IsAny<CheckType>())).Returns(check.Object);
	}

	private static void FlushCasting(NativeRuntime runtime)
	{
		// Native wounds/items use late initialisation in the production save queue.
		// The harness character also needs its explicit save because it intentionally bypasses login.
		runtime.World.SaveManager.Flush();
		using (new FMDB())
		{
			runtime.Actor.Save(); runtime.Body.Save();
			foreach (var trait in runtime.Actor.CharacterTraits) trait.Save();
			FMDB.Context.SaveChanges();
		}
	}

	private static void RunCastingReaderProcess(CastingReader input)
	{
		var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
		start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); start.ArgumentList.Add("--casting-reader");
		start.ArgumentList.Add(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(input))));
		using var process = Process.Start(start)!;
		var output = process.StandardOutput.ReadToEndAsync(); var error = process.StandardError.ReadToEndAsync();
		if (!process.WaitForExit(60000)) { process.Kill(true); throw new TimeoutException("ARM02 reader exceeded 60 seconds."); }
		Require(process.ExitCode == 0, output.GetAwaiter().GetResult() + error.GetAwaiter().GetResult());
		Console.Write(output.GetAwaiter().GetResult());
	}
	private static int RunCastingReader(string[] args)
	{
		var input = JsonSerializer.Deserialize<CastingReader>(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args.Single())))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database);
		ConfigureNativeDatabase(database.ConnectionString);
		var store = new MagicCastingStateStore();
		var state = store.Acquisition(input.Earth.CharacterId, input.Spell)!;
		Require(state.ControlledGrade == input.Grade, "Restart grade mismatch.");
		Require(store.Unresolved(input.Earth.CharacterId).Count == input.Unresolved, "Restart receipt mismatch.");
		using var read = NewIndependentContext(database.ConnectionString);
		Require(read.CharacterTraits.AsNoTracking().Single(x => x.CharacterId == input.Earth.CharacterId && x.TraitDefinitionId == input.EarthSkill).Value == input.RawSkill, "Restart native skill mismatch.");
		Require(read.CharactersMagicResources.AsNoTracking().Single(x => x.CharacterId == input.Earth.CharacterId && x.MagicResourceId == input.Earth.ResourceId).Amount == input.Balance, "Restart balance mismatch.");
		Require(read.CharacterCastingEnrolments.Count(x => x.CharacterId == input.Earth.CharacterId) == 2, "Restart enrolment mismatch.");
		if (input.SkillDeadline is { } skillDeadline) Require(store.Opportunity(input.Earth.CharacterId, input.EarthSkill)!.NextUtc == skillDeadline, "Restart skill deadline mismatch.");
		if (input.MasteryDeadline is { } masteryDeadline) Require(state.NextMasteryUtc == masteryDeadline, "Restart mastery deadline mismatch.");
		if (input.Operation.HasValue)
		{
			Require(store.Operation(input.Operation.Value)?.Stage == "NeedsReview", "Restart lost uncertainty.");
			VerifyCastingQuarantineReload(database, input);
		}
		if (input.VerifyEffects || input.SecondBody is not null) VerifyCastingRuntimeReload(database, input);
		if (input.VerifyCapLoss) VerifyCompletionCapLossReload(database, input);
		if (input.SupportTrait.HasValue) VerifySupportProgressionReload(database, input);
		if (input.Capacity.HasValue) VerifyCapacityReload(database, input);
		Console.WriteLine($"ARM02-reader=passed grade:{state.ControlledGrade} balance:{input.Balance} unresolved:{input.Unresolved} operation:{input.Operation?.ToString() ?? "none"} process:{Environment.ProcessId}");
		return 0;
	}
}
