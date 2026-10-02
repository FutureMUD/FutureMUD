using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Improvement;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.Logging;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Checks;

#nullable enable
namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record PracticeReader(string Database, FixtureIds Fixture, long Spell, long Capability,
		long Trait, Guid Operation, DateTime Deadline, DateTime SkillDeadline, DateTime MasteryDeadline);

	private static int RunPracticeAcceptanceChecks()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString);
		var first = FixtureSeed.Create(database, "arm_practice", false);
		var second = FixtureSeed.Create(database, "arm_practice_bystander", true);
		var native = NativeRuntime.Load(first, database.ConnectionString, true);
		ConfigureCastingWorld(native, database.ConnectionString, true);
		var improver = ConfigurePracticeImprovement(native, database.ConnectionString, true);
		var actor = native.Actor; var world = native.World;
		var trait = CastingRequired(world.Traits.GetByName("ARM02 Earth Proficiency"));
		var cap = (SkillLevelBasedMagicCapability)native.Capability; var source = native.Resource;
		var spell = new MagicSpell("ARM Practice Cap60", cap.School);
		((All<IMagicSpell>)world.MagicSpells).Add(spell);
		foreach (var command in new[] { "trigger new self", $"trait {trait.Id}", "difficulty easy", "threshold minorpass",
			"duration ARM02 Duration", $"cost {source.Id} ARM02 Cost", "prog rejuvenation_known", "castemote The fixture manifests.",
			"failcastemote The fixture fails.", "effect add damage", "effect 1 type burning", "effect 1 formula grade*5",
			$"effect 1 bodypart {native.Body.Bodyparts.First().Id}", "castereffect add spellarmour", "grades fixture",
			"grades efficiency source 7 1", "grades overreach 1 1", "grades practice fixture" })
			Require(spell.BuildingCommand(actor, new StringStack(command)), "Practice spell builder refused: " + command);
		foreach (var command in new[] { $"casting trait {trait.Id}", $"casting resources {source.Id} {source.Id} gather",
			$"casting entry add {spell.Id}", $"casting entry starting {spell.Id} on", $"casting entry skill {spell.Id} 30 60 relative", "casting enable on" })
			Require(cap.BuildingCommand(actor, new StringStack(command)), "Practice capability builder refused: " + command);
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(cap)]);
		Require(spell.ReadyForGame && !actor.IsAdministrator(), "Practice requires a ready spell and non-admin actor.");
		var now = new DateTime(2026, 10, 2, 0, 0, 0, DateTimeKind.Utc); var samples = 0;
		var service = new MagicCastingService(world, clock: () => now, random: () => { samples++; return 0.1; }, flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		var enrol = new FutureProg(world, "armPracticeEnrol", ProgVariableTypes.Boolean,
			[Tuple.Create(ProgVariableTypes.Character, "actor"), Tuple.Create(ProgVariableTypes.MagicCapability, "cap")],
			"return enrolchannelcasting(@actor, @cap, \"native practice fixture\")");
		Require(enrol.Compile() && enrol.ExecuteBool(actor, cap), "Authored practice enrolment failed.");
		Require(actor.GetTrait(trait) is Skill && actor.TraitRawValue(trait) == 30 && service.Acquisition(actor, spell.Id)!.ControlledGrade == 1,
			"Practice did not begin at legitimate opening 30 and controlled grade 1.");

		var bystander = NativeRuntime.Load(second, database.ConnectionString, true);
		SetPrivateMember(bystander.Actor, "Location", actor.Location);
		var wardParent = new MagicSpellParent(bystander.Actor, spell, actor);
		var ward = new SpellPersonalWardEffect(bystander.Actor, wardParent, cap.School, MagicInterdictionMode.Fail, MagicInterdictionCoverage.Incoming, true, null);
		wardParent.AddSpellEffect(ward); bystander.Actor.AddEffect(wardParent); bystander.Actor.AddEffect(ward);
		Mock.Get(actor.Location).SetupGet(x => x.Characters).Returns([actor, bystander.Actor]);
		((All<ICharacter>)world.Characters).Add(actor); ((All<ICharacter>)world.Characters).Add(bystander.Actor);
		var woundBefore = bystander.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun);
		Require(woundBefore > 0 && ward.ShouldInterdict(actor, cap.School), "The nearby native injured/warded fixture is invalid.");
		native.WorldMock.Setup(x => x.GetCheck(CheckType.ResistMagicSpellCheck)).Throws(new InvalidOperationException("Practice invoked target resistance."));
		var store = new MagicCastingStateStore();

		MagicPracticeAction Begin(int grade, bool overreach)
		{
			now += TimeSpan.FromMinutes(11); actor.AddResource(source, 100);
			var before = actor.MagicResourceAmounts[source]; var writes = store.Unresolved(actor.Id).Count;
			var quote = service.Quote(new(actor, cap.Id, spell.Id, grade, overreach, "", MagicCastingMode.Practice));
			Require(quote.Allowed && store.Unresolved(actor.Id).Count == writes && actor.MagicResourceAmounts[source] == before, "Practice quote was not pure.");
			MagicModule.MagicGeneric(actor, $"{cap.School.SchoolVerb} practice \"{spell.Name}\" grade {grade}{(overreach ? " overreach" : "")} via {cap.Id}");
			var action = actor.EffectsOfType<MagicPracticeAction>().Single();
			Require(actor.MagicResourceAmounts[source] == before - quote.Invocation!.Costs.Single().Amount, "Practice did not pay the full configured cost at start.");
			Require(!action.SavingEffect && (DateTime)System.Xml.Linq.XElement.Parse(store.Operation(action.OperationId)!.Definition).Attribute("deadlineUtc")! == now.AddSeconds(30),
				"Practice lacks its durable deadline or saved an auto-resumable timer.");
			Require(!actor.EffectsOfType<MagicSpellParent>().Any() && !native.Body.Wounds.Any(), "Practice ran caster/target effects at start.");
			return action;
		}
		void Finish(MagicPracticeAction action)
		{
			now += TimeSpan.FromSeconds(30); action.ExpireEffect(); action.ExpireEffect();
			Require(store.Operation(action.OperationId)!.Stage == "Completed" && !actor.EffectsOfType<MagicPracticeAction>().Any(), "Practice did not complete exactly once.");
			Require(!actor.EffectsOfType<MagicSpellParent>().Any() && !native.Body.Wounds.Any() &&
				bystander.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun) == woundBefore && bystander.Actor.EffectsOfType<SpellPersonalWardEffect>().Contains(ward),
				"Practice affected an injured bystander, its ward or the caster.");
		}
		Require(improver.BuildingCommand(actor, new StringStack("interval 1")), "Native restrictive interval refused.");
		Finish(Begin(1, false));
		Require(actor.TraitRawValue(trait) == 30, "Native interval restriction did not inhibit actual skill gain.");
		Require(improver.BuildingCommand(actor, new StringStack("interval 20")), "Native permissive interval refused.");
		for (var grade = 1; grade <= 7; grade++)
		{
			var raw = actor.TraitRawValue(trait); var action = Begin(grade, grade > 1);
			Require(actor.TraitRawValue(trait) == raw, "Practice improved before its deadline.");
			Finish(action);
			Require(actor.TraitRawValue(trait) == Math.Min(60, 30 + grade * 10) && service.Acquisition(actor, spell.Id)!.ControlledGrade == grade,
				$"Native practice progression failed at grade {grade}: raw {actor.TraitRawValue(trait)}.");
			Console.WriteLine($"ARM-PRACTICE-grade=passed grade:{grade} raw:{actor.TraitRawValue(trait)} cap:60 balance:{actor.MagicResourceAmounts[source]} samples:{samples} duration:30 accelerated-clock");
		}
		Require(samples == 6 && spell.GradeProfile!.Practice!.MaximumGrade is null, "Practice maximum or sample count changed.");
		Require(spell.BuildingCommand(actor, new StringStack("grades practice max 6")) &&
			!service.Quote(new(actor, cap.Id, spell.Id, 7, false, "", MagicCastingMode.Practice)).Allowed, "Builder practice maximum did not restrict the route.");
		Require(spell.BuildingCommand(actor, new StringStack("grades practice max none")) && spell.BuildingCommand(actor, new StringStack("grades practice enabled false")) &&
			!service.Quote(new(actor, cap.Id, spell.Id, 1, false, "", MagicCastingMode.Practice)).Allowed, "Builder disabled practice did not restrict the route.");
		Require(spell.BuildingCommand(actor, new StringStack("grades practice enabled true")), "Practice re-enable refused.");
		Console.WriteLine("ARM-PRACTICE-restrictions=passed native-interval:1-blocks/20-improves maximum:6-refuses7 disabled:refuses");

		foreach (var signal in new[] { "stop", "focus", "speech", "capability", "quit" })
		{
			var action = Begin(1, false); var paid = actor.MagicResourceAmounts[source];
			switch (signal)
			{
				case "stop": actor.RemoveEffect(action, true); break;
				case "focus": typeof(MudSharp.Character.Character).GetMethod("SetFocusedInstance", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(actor, [null]); break;
				case "speech":
					Mock.Get(native.Body.Communications).Setup(x => x.CanVocalise(native.Body)).Returns(false);
					Mock.Get(world.HeartbeatManager).Raise(x => x.FuzzyFiveSecondHeartbeat += null);
					Mock.Get(native.Body.Communications).Setup(x => x.CanVocalise(native.Body)).Returns(true); break;
				case "capability": actor.SetMerits([]); service.Reconcile(actor); actor.SetMerits([NativeRuntime.NewCapabilityMerit(cap)]); break;
				case "quit": typeof(PerceivedItem).GetMethod("PerceivableQuit", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(actor, []); break;
			}
			now += TimeSpan.FromSeconds(30); action.ExpireEffect(); action.RemovalEffect();
			Require(store.Operation(action.OperationId)!.Stage == "PracticeInterrupted" && actor.MagicResourceAmounts[source] == paid &&
				samples == 6 && actor.TraitRawValue(trait) == 60 && !actor.EffectsOfType<MagicPracticeAction>().Any(), "Native interruption refunded, gained or replayed: " + signal);
			Console.WriteLine($"ARM-PRACTICE-interrupt=passed signal:{signal} balance:{paid} no-progress no-refund no-replay");
		}
		var pending = Begin(1, false); var state = service.Acquisition(actor, spell.Id)!;
		FlushCasting(native);
		var descriptor = new PracticeReader(database.Name, first, spell.Id, cap.Id, trait.Id, pending.OperationId,
			now.AddSeconds(30), store.Opportunity(actor.Id, trait.Id)!.NextUtc, state.NextMasteryUtc);
		RunPracticeReaderProcess(descriptor);
		Require(store.Operation(pending.OperationId)!.Stage == "Reconciled", "Independent recovery did not finalise the pending operation.");
		now += TimeSpan.FromSeconds(30); pending.ExpireEffect(); pending.RemovalEffect();
		Require(store.Operation(pending.OperationId)!.Stage == "Reconciled" && samples == 6 && actor.MagicResourceAmounts[source] == 93,
			"A stale timer rewrote recovered work or awarded progress.");
		Console.WriteLine("ARM-PRACTICE-native=passed raw:30-to-60 controlled:1-to-7 actual-ClassicImprovement shared-deadlines target/caster/resistance/ward-purity separate-process-recovery stale-action-terminal-preserved stock-installed-world:NOT_RUN material-plan:explicit-empty");
		return 0;
	}

	private static ClassicImprovement ConfigurePracticeImprovement(NativeRuntime native, string connection, bool create)
	{
		using var db = NewIndependentContext(connection); var world = native.World;
		if (create)
		{
			db.Improvers.Add(new() { Name = "ARM practice native use", Type = "classic",
				Definition = "<Definition Chance='1' Expression='10' ImproveOnFail='true' ImproveOnSuccess='true' DifficultyThresholdInterval='20' NoGainSecondsDiceExpression='0'/>" });
			db.TraitExpressions.Add(new() { Name = "ARM practice native cap", Expression = "100" }); db.SaveChanges();
		}
		var model = db.Improvers.Single(x => x.Name == "ARM practice native use");
		var cap = db.TraitExpressions.Single(x => x.Name == "ARM practice native cap");
		var improver = new ClassicImprovement(world, model); var models = new All<IImprovementModel>(); models.Add(improver);
		native.WorldMock.SetupGet(x => x.ImprovementModels).Returns(models); native.WorldMock.SetupGet(x => x.LogManager).Returns(Mock.Of<ILogManager>());
		if (world.TraitExpressions.Get(cap.Id) is null) ((All<ITraitExpression>)world.TraitExpressions).Add(new TraitExpression(cap, world));
		var traits = new All<ITraitDefinition>();
		foreach (var definition in db.TraitDefinitions.Where(x => x.TraitGroup == "ARM02"))
		{
			if (create && definition.Type == (int)TraitType.Skill) { definition.ImproverId = model.Id; definition.ExpressionId = cap.Id; }
			TraitDefinition loaded = definition.Type == (int)TraitType.Skill ? new SkillDefinition(definition, world) : new AttributeDefinition(definition, world);
			loaded.Initialise(definition); traits.Add(loaded);
		}
		db.SaveChanges(); native.WorldMock.SetupGet(x => x.Traits).Returns(traits);
		SetPrivateField(native.Actor, "_characterTraits", db.CharacterTraits.AsNoTracking().Where(x => x.CharacterId == native.Actor.Id).ToArray()
			.Select(x => CastingRequired(traits.Get(x.TraitDefinitionId)).LoadTrait(new MudSharp.Models.Trait { Value = x.Value, AdditionalValue = x.AdditionalValue }, native.Actor)).ToList());
		return improver;
	}

	private static void RunPracticeReaderProcess(PracticeReader input)
	{
		var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
		start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); start.ArgumentList.Add("--practice-reader");
		start.ArgumentList.Add(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(input))));
		using var process = Process.Start(start)!;
		var output = process.StandardOutput.ReadToEndAsync(); var error = process.StandardError.ReadToEndAsync();
		if (!process.WaitForExit(60000)) { process.Kill(true); throw new TimeoutException("Practice reader exceeded 60 seconds."); }
		Require(process.ExitCode == 0, output.GetAwaiter().GetResult() + error.GetAwaiter().GetResult()); Console.Write(output.GetAwaiter().GetResult());
	}

	private static int RunPracticeReader(string[] args)
	{
		var input = JsonSerializer.Deserialize<PracticeReader>(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args.Single())))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var native = NativeRuntime.Load(input.Fixture, database.ConnectionString, true); ConfigureCastingWorld(native, database.ConnectionString, false);
		ConfigurePracticeImprovement(native, database.ConnectionString, false);
		var actor = native.Actor; var store = new MagicCastingStateStore();
		var service = new MagicCastingService(native.World, clock: () => input.Deadline.AddHours(1),
			random: () => throw new InvalidOperationException("Restart must not reroll practice"), flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		var op = store.Operation(input.Operation)!; var xml = System.Xml.Linq.XElement.Parse(op.Definition);
		Require(op.Stage == "Practising" && (DateTime)xml.Attribute("deadlineUtc")! == input.Deadline &&
			store.Opportunity(actor.Id, input.Trait)!.NextUtc == input.SkillDeadline && service.Acquisition(actor, input.Spell)!.NextMasteryUtc == input.MasteryDeadline,
			"Independent restart lost prepaid practice or shared deadlines.");
		Require(!actor.EffectsOfType<MagicPracticeAction>().Any() && actor.TraitRawValue(native.World.Traits.Get(input.Trait)) == 60 &&
			actor.MagicResourceAmounts[native.Resource] == 93 && service.Acquisition(actor, input.Spell)!.ControlledGrade == 7,
			"Independent restart resumed a timer, refunded payment or changed progress.");
		Require(!service.Quote(new(actor, input.Capability, input.Spell, 1, false, "", MagicCastingMode.Practice)).Allowed &&
			service.Cast(new(actor, input.Capability, input.Spell, 1, false, "", MagicCastingMode.Practice)).Status == MagicCastingStatus.Refused,
			"Restart did not quarantine prepaid pending practice.");
		var staff = new Mock<ICharacter>(); staff.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
		Require(service.ReconcileOperation(staff.Object, actor, input.Operation, "Process interrupted pending practice; no sample").Allowed &&
			store.Operation(input.Operation)!.Stage == "Reconciled" && actor.MagicResourceAmounts[native.Resource] == 93,
			"Recovery replayed or refunded pending practice.");
		Console.WriteLine($"ARM-PRACTICE-reader=passed process:{Environment.ProcessId} operation:{input.Operation} raw:60 grade:7 balance:93 deadline:{input.Deadline:O} no-resume/refund/reroll");
		return 0;
	}
}
