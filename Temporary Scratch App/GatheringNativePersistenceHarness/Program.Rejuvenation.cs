#nullable enable

using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Environment;
using MudSharp.Magic.Generators;
using MudSharp.Magic.SpellEffects;
using MudSharp.PerceptionEngine;
using MudSharp.Planes;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;
using MudSharp.Work.Agriculture;
using MudSharp.Work.Foraging;
using MySql.Data.MySqlClient;
using CompiledFutureProg = MudSharp.FutureProg.FutureProg;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static int RunRejuvenationAcceptanceChecks()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString);
		using (var schema = NewIndependentContext(database.ConnectionString))
		{
			var before = schema.Database.GetAppliedMigrations().Last();
			schema.Database.Migrate();
			Require(!schema.Database.GetPendingMigrations().Any(), "Rejuvenation schema did not reach the current migration.");
			Console.WriteLine($"R-schema=passed imported:{before} current:{schema.Database.GetAppliedMigrations().Last()}");
		}
		var fixture = NativeOrganicFixtureSeed.Create(database);
		var profileId = RunOrganicProfileAuthoringAndClone(database.ConnectionString, fixture.EnvironmentalResourceId);
		using (var binding = NewIndependentContext(database.ConnectionString))
		{
			var cell = binding.Cells.Single(x => x.Id == fixture.CoordinatorActorFixture.CellId);
			cell.EnvironmentalMagicBindingMode = (int)EnvironmentalMagicBindingMode.Explicit;
			cell.EnvironmentalMagicProfileId = profileId;
			binding.SaveChanges();
		}
		RunLandActionPersistenceProbe(database.Name, database.ConnectionString, fixture.CoordinatorActorFixture,
			fixture.GentleActorFixture, fixture.CoordinatorCropFieldId, profileId, fixture.EnvironmentalResourceId, true);
		Console.WriteLine("rejuvenationNative=passed");
		return 0;
	}

	private static void ConfigureRejuvenationSpellWorld(NativeRuntime runtime, string connectionString)
	{
		var world = runtime.WorldMock;
		world.Setup(x => x.SystemMessage(It.IsAny<string>(), It.IsAny<bool>()))
			.Callback<string, bool>((message, _) => Console.WriteLine($"rejuvenation-system: {message.StripANSIColour()}"));
		Mock.Get(runtime.Body.Prototype).SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1L));
		var output = new Mock<IOutputHandler>();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
			.Callback<string, bool, bool>((message, _, _) => Console.WriteLine($"rejuvenation-command: {message.StripANSIColour()}"));
		SetPrivateMember(runtime.Actor, "OutputHandler", output.Object);
		SetPrivateField(runtime.Actor, "_learnedPowers", new List<IMagicPower>());
		var actors = new All<ICharacter>();
		actors.Add(runtime.Actor);
		world.SetupGet(x => x.Actors).Returns(actors);
		world.SetupGet(x => x.LegalAuthorities).Returns(new All<ILegalAuthority>());
		var spells = new All<IMagicSpell>();
		world.SetupGet(x => x.MagicSpells).Returns(spells);
		world.Setup(x => x.Add(It.IsAny<IMagicSpell>())).Callback<IMagicSpell>(spell => { spells.Add(spell); });
		var check = new Mock<ICheck>();
		check.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.CastSpellCheck, Outcome.Pass)));
		world.Setup(x => x.GetCheck(It.IsAny<CheckType>())).Returns(check.Object);
		CompiledFutureProg.Initialise();
		using var context = NewIndependentContext(connectionString);
		if (!context.FutureProgs.Any(x => x.FunctionName == "rejuvenation_known"))
		{
			var known = new Db.FutureProg { FunctionName = "rejuvenation_known", FunctionComment = "Owned native spell admission policy.",
				FunctionText = "return true", ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(), Category = "Harness",
				Subcategory = "Rejuvenation", StaticType = (int)FutureProgStaticType.NotStatic };
			known.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "caster", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
			known.FutureProgsParameters.Add(new() { ParameterIndex = 1, ParameterName = "spell", ParameterTypeDefinition = ProgVariableTypes.MagicSpell.ToStorageString() });
			context.FutureProgs.Add(known);
			context.FutureProgs.Add(new() { FunctionName = "AlwaysFalse", FunctionComment = "Owned constructor default.", FunctionText = "return false",
				ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(), Category = "Harness", Subcategory = "Rejuvenation",
				StaticType = (int)FutureProgStaticType.NotStatic });
			context.SaveChanges();
		}
		var progs = (All<IFutureProg>)runtime.World.FutureProgs;
		foreach (var model in context.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking()
			.Where(x => x.Subcategory == "Rejuvenation"))
		{
			var prog = new CompiledFutureProg(runtime.World, model.FunctionName, ProgVariableTypes.FromStorageString(model.ReturnTypeDefinition),
				model.FutureProgsParameters.OrderBy(x => x.ParameterIndex).Select(x => Tuple.Create(ProgVariableTypes.FromStorageString(x.ParameterTypeDefinition), x.ParameterName)), model.FunctionText)
			{ Id = model.Id, StaticType = FutureProgStaticType.NotStatic };
			Require(prog.Compile(), prog.CompileError);
			progs.Add(prog);
		}
		var expressions = new All<ITraitExpression>();
		foreach (var model in context.TraitExpressions.Include(x => x.TraitExpressionParameters).AsNoTracking())
			expressions.Add(new TraitExpression(model, runtime.World));
		world.SetupGet(x => x.TraitExpressions).Returns(expressions);
		MagicSpellParent.InitialiseEffectType();
		SpellRejuvenateLandEffect.InitialiseEffectType();
		foreach (var model in context.MagicSpells.AsNoTracking()) spells.Add(new MagicSpell(model, runtime.World));
	}

	private static MagicSpell AuthorRejuvenationSpell(NativeRuntime runtime, string connectionString)
	{
		ConfigureRejuvenationSpellWorld(runtime, connectionString);
		var actor = runtime.Actor;
		using (var context = NewIndependentContext(connectionString))
		{
			if (!context.TraitDefinitions.Any(x => x.Id == 1))
			{
				context.TraitDefinitions.Add(new Db.TraitDefinition { Id = 1, Name = "Rejuvenation Casting", Alias = "restoration",
					TraitGroup = "Harness", ChargenBlurb = "", ValueExpression = "100" });
				context.SaveChanges();
			}
			foreach (var (name, formula) in new[] { ("Rejuvenation Duration", "600"), ("Rejuvenation Cost", "0.25") })
			{
				var model = new Db.TraitExpression { Name = name, Expression = formula };
				context.TraitExpressions.Add(model);
				context.SaveChanges();
				((All<ITraitExpression>)runtime.World.TraitExpressions).Add(new TraitExpression(model, runtime.World));
			}
		}
		var known = runtime.World.FutureProgs.Single(x => x.Name == "rejuvenation_known");
		var spell = new MagicSpell("Restore Land", runtime.Capability.School);
		((All<IMagicSpell>)runtime.World.MagicSpells).Add(spell);
		foreach (var command in new[]
		{
			"trigger new room", "trait 1", "duration Rejuvenation Duration", $"cost {runtime.Resource.Id} Rejuvenation Cost",
			$"prog {known.Id}", "castemote A restorative treatment begins.", "failcastemote The treatment fails.",
			"effect add rejuvenateland", "effect 1 budget 12", "effect 1 rate 2",
			"effect 1 desc The scarred ground is slowly mending.", "effect 1 local off"
		}) Require(spell.BuildingCommand(actor, new StringStack(command)), $"R-P01 rejected builder command: {command}");
		runtime.World.SaveManager.Flush();
		using var read = NewIndependentContext(connectionString);
		var reconstructed = new MagicSpell(read.MagicSpells.AsNoTracking().Single(x => x.Id == spell.Id), runtime.World);
		if (!reconstructed.ReadyForGame) throw new InvalidOperationException(reconstructed.WhyNotReadyForGame(actor));
		((All<IMagicSpell>)runtime.World.MagicSpells).Remove(spell);
		((All<IMagicSpell>)runtime.World.MagicSpells).Add(reconstructed);
		Console.WriteLine($"R-P01-author=passed spell:{spell.Id} builder:room/budget/rate/duration/cost/desc/local source-reloaded:True");
		return reconstructed;
	}

	private static void RunRejuvenationSpellProbes(string databaseName, string connectionString, FixtureIds fixture,
		NativeRuntime runtime, Cell cell, AgricultureField field, EnvironmentalMagicGenerator profile,
		EnvironmentalMagicCoordinator coordinator, HarnessClock clock, RejuvenationAcceptanceStore store, Guid landOperation)
	{
		var actor = runtime.Actor;
		var scheduler = new EffectScheduler(runtime.World, clock);
		runtime.WorldMock.SetupGet(x => x.EffectScheduler).Returns(scheduler);
		var spell = AuthorRejuvenationSpell(runtime, connectionString);
		Require(profile.BuildingCommand(actor, new StringStack("magicalrepaircap 1")), "Unable to set slow repair cap.");
		Require(profile.BuildingCommand(actor, new StringStack("repair 0")), "Unable to isolate magical repair from natural recovery.");
		Require(profile.BuildingCommand(actor, new StringStack("output 1 rate 0")), "Unable to isolate mana production.");
		Require(profile.BuildingCommand(actor, new StringStack("output 1 maximum max(0,100-scardamage)")), "Unable to configure scar-dependent maximum.");
		runtime.World.SaveManager.Flush();
		coordinator.Pump();
		var ambient = profile.Outputs.Single().Resource!;
		Require(coordinator.TryMutateResource(cell, ambient, EnvironmentalResourceMutation.Set, 30, out var set) && set,
			"Unable to set the acceptance ambient balance.");
		actor.AddResource(runtime.Resource, 20.0);
		runtime.World.SaveManager.Flush();
		var nativeBefore = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		var forageBefore = cell.GetForagableYield("herbs");
		var beforeState = cell.EnvironmentState;
		var ambientBefore = cell.MagicResourceAmounts.ToDictionary(x => x.Key.Id, x => x.Value);
		Require(beforeState.ScarDamage == 20.0, "R-P02 native Land did not create twenty scars.");
		SpellRejuvenateLandEffect Cast()
		{
			MagicModule.MagicGeneric(actor, $"{spell.School.SchoolVerb} cast \"{spell.Name}\" standard");
			return cell.Effects.OfType<SpellRejuvenateLandEffect>().Single();
		}
		void Tick(double seconds) { clock.Advance(TimeSpan.FromSeconds(seconds)); for (var i = 0; i < 10; i++) coordinator.Pump(); }
		LandRejuvenationProgress Read(Guid id) => store.FindTreatment(id) ?? throw new InvalidOperationException("Missing durable treatment.");
		var mana = actor.MagicResourceAmounts[runtime.Resource];
		var child = Cast();
		Require(actor.MagicResourceAmounts[runtime.Resource] == mana - 0.25 && cell.EnvironmentState.ScarDamage == 20,
			"R-P01 legacy cast did not pay once or repaired during installation.");
		runtime.World.SaveManager.Flush(); // Save parent before work. Later repair proofs deliberately have no flush.
		Tick(60);
		var progress = Read(child.TreatmentId);
		using (var independent = NewIndependentContext(connectionString))
		{
			var state = independent.CellEnvironmentalStates.AsNoTracking().Single(x => x.CellId == cell.Id);
			var receipt = independent.EnvironmentalMagicOperations.AsNoTracking().Single(x => x.Id == progress.LastOperationId);
			Require(state.ScarDamage == 19 && progress.RemainingBudget == 11 && progress.TotalRepaired == 1 &&
				progress.AcknowledgedSequence == 1 && receipt.AppliedRepair == 1, "R-P02 atomic repair/checkpoint was not independently visible.");
		}
		var nativeAfter = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		Require(nativeAfter.NativeStock == nativeBefore.NativeStock && nativeAfter.PrepaidFraction == nativeBefore.PrepaidFraction &&
			nativeAfter.RecoveryRemainders == nativeBefore.RecoveryRemainders && nativeAfter.Lifecycle == nativeBefore.Lifecycle && cell.GetForagableYield("herbs") == forageBefore &&
			cell.EnvironmentState.LastDefileUtc == beforeState.LastDefileUtc && ambientBefore.All(x => cell.MagicResourceAmounts.Single(y => y.Key.Id == x.Key).Value == x.Value),
			"R-P02 repair changed native stock, prepaid credit, mana or destructive history.");
		Require(coordinator.Inspect(cell).Outputs.Single().Maximum == 81 && cell.MagicResourceAmounts[ambient] == 30,
			"R-P06 repaired capacity granted immediate mana.");
		var nativeTick = typeof(Cell).GetMethod("YieldTick", BindingFlags.NonPublic | BindingFlags.Instance)!;
		nativeTick.Invoke(cell, null);
		var recoveredForage = cell.GetForagableYield("herbs");
		Require(recoveredForage > forageBefore, "R-P06 later native recovery did not improve after scar repair.");
		Console.WriteLine($"R-P06-recovery=passed capacity:80->81 mana:30->30 immediate-yield:{forageBefore} later-native-yield:{recoveredForage} native-crop-unchanged:True");
		Console.WriteLine($"R-P02=passed land:{landOperation} treatment:{child.TreatmentId} repair:{progress.LastOperationId} scar:20->19 budget:12->11 repaired:1 native-stock:{nativeBefore.NativeStock} prepaid:{nativeBefore.PrepaidFraction} no-saving-shutdown:True");
		var originalParent = child.ParentEffect;
		Require(ReferenceEquals(Cast().ParentEffect, originalParent), "R-P05 exclusive recast replaced the active treatment.");
		var otherSpell = new MagicSpell(spell, "Other Restoration");
		((All<IMagicSpell>)runtime.World.MagicSpells).Add(otherSpell);
		MagicModule.MagicGeneric(actor, $"{spell.School.SchoolVerb} cast \"{otherSpell.Name}\" standard");
		Require(ReferenceEquals(cell.Effects.OfType<SpellRejuvenateLandEffect>().Single().ParentEffect, originalParent),
			"R-P05 a different source spell replaced or stacked the treatment.");
		clock.Advance(TimeSpan.FromSeconds(30));
		var dispel = SpellEffectFactory.LoadEffectFromBuilderInput("dispelmagic", new StringStack(""), spell).Trigger;
		dispel.GetOrApplyEffect(actor, cell, OpposedOutcomeDegree.Moderate, SpellPower.Standard,
			new MagicSpellParent(cell, spell, actor), []);
		Require(cell.EnvironmentState.ScarDamage == 19 && Read(child.TreatmentId).CancellationRequested,
			"R-P05 dispel applied a burst or failed to persist cancellation.");
		RunRejuvenationReaderProcess(new(databaseName, fixture, cell.Id, field.Id, profile.Id, spell.Id, child.TreatmentId,
			19, 11, 1, Read(child.TreatmentId).RemainingSeconds, nativeBefore.NativeStock, nativeBefore.PrepaidFraction, false));
		child = Cast();
		using (var connection = new MySqlConnection(connectionString))
		{
			connection.Open();
			using var command = connection.CreateCommand();
			command.CommandText = "CREATE TRIGGER rejuvenation_rollback BEFORE UPDATE ON cellenvironmentalstates FOR EACH ROW SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='owned rejuvenation rollback probe'";
			command.ExecuteNonQuery();
			try { Tick(60); }
			finally { command.CommandText = "DROP TRIGGER rejuvenation_rollback"; command.ExecuteNonQuery(); }
		}
		progress = Read(child.TreatmentId);
		var prepared = progress.PendingRequest ?? throw new InvalidOperationException("R-P04 rollback lost its prepared request.");
		Require(progress.RemainingBudget == 12 && store.Find(prepared.OperationId) is null && cell.EnvironmentState.ScarDamage == 19,
			"R-P04 rollback partially committed the scar, receipt or budget.");
		using (var rejected = NewIndependentContext(connectionString))
			Require(rejected.CellEnvironmentalStates.AsNoTracking().Single(x => x.CellId == cell.Id).ScarDamage == 19 &&
				!rejected.EnvironmentalMagicOperations.Any(x => x.Id == prepared.OperationId) &&
				JsonSerializer.Deserialize<LandRejuvenationProgress>(rejected.LandRejuvenationTreatments.AsNoTracking().Single(x => x.Id == progress.Id).Checkpoint)!.RemainingBudget == 12,
				"R-P04 fresh context did not observe the complete provider rollback.");
		Require(coordinator.ConfirmTreatment(cell, child.TreatmentId, out var confirmation), confirmation ?? "Confirmation failed.");
		Require(cell.EnvironmentState.ScarDamage == 19, "R-P04 staff confirmation applied repair.");
		Tick(60);
		progress = Read(child.TreatmentId);
		Require(progress.LastOperationId == prepared.OperationId && progress.RemainingBudget == 11 && cell.EnvironmentState.ScarDamage == 18,
			"R-P04 retry did not use the exact rolled-back identity once.");
		store.LoseNextAcknowledgement = true;
		Tick(60);
		progress = Read(child.TreatmentId);
		Require(progress.TotalRepaired == 2 && progress.RemainingBudget == 10 && cell.EnvironmentState.ScarDamage == 18,
			"R-P04 lost acknowledgement did not leave a committed authoritative checkpoint and quarantined runtime.");
		using (var acknowledged = NewIndependentContext(connectionString))
			Require(acknowledged.CellEnvironmentalStates.AsNoTracking().Single(x => x.CellId == cell.Id).ScarDamage == 17 &&
				acknowledged.EnvironmentalMagicOperations.Single(x => x.Id == progress.LastOperationId).AppliedRepair == 1 &&
				JsonSerializer.Deserialize<LandRejuvenationProgress>(acknowledged.LandRejuvenationTreatments.AsNoTracking().Single(x => x.Id == progress.Id).Checkpoint)!.RemainingBudget == 10,
				"R-P04 fresh context did not observe the committed lost-response boundary.");
		cell.RemoveEffect(child.ParentEffect, true);
		Require(coordinator.ConfirmTreatment(cell, child.TreatmentId, out confirmation), confirmation ?? "Confirmation failed.");
		Require(cell.EnvironmentState.ScarDamage == 17 && Read(child.TreatmentId).CancellationRequested,
			"R-P05 uncertain dispel did not settle the old durable repair without resurrecting work.");
		Console.WriteLine($"R-P04=passed provider-rollback:{prepared.OperationId} exact-retry:True response-lost-after-commit:{progress.LastOperationId} independently-read:True scar:19->18->17 terminal-cancellation:True");
		RunRejuvenationVancianProbe(runtime, connectionString, cell, spell, clock, coordinator);
		child = Cast();
		var endedId = child.TreatmentId;
		Require(coordinator.ApplyOperation(cell, new(Guid.NewGuid(), actor.Id, "R-P05 explicit zero boundary", Repair: 100)).Success,
			"R-P05 could not reach zero scars through the native ecological boundary.");
		var gathering = new MudSharp.Magic.Gathering.MagicGatheringService(runtime.World, clock: clock);
		var begun = gathering.Begin(actor, runtime.Capability, "draw", 1);
		Require(begun.Success, begun.Message);
		clock.Advance(TimeSpan.FromSeconds(1));
		var gathered = gathering.Complete(actor, begun.OperationId!.Value);
		Require(gathered.Success, gathered.Message);
		Tick(60);
		Require(cell.EnvironmentState.ScarDamage == 20 && Read(endedId).IsTerminal && !cell.Effects.OfType<SpellRejuvenateLandEffect>().Any(),
			"R-P05 zero-scar treatment revived to repair later native Land damage.");
		var template = (RejuvenateLandEffect)spell.SpellEffects.Single();
		Require(template.BuildingCommand(actor, new StringStack("local on")), "Could not author presence-dependent mode.");
		child = Cast();
		((All<ICharacter>)runtime.World.Actors).Remove(actor);
		Tick(60);
		Require(Read(child.TreatmentId).IsTerminal && cell.EnvironmentState.ScarDamage == 20,
			"R-P06 local treatment continued without its original active instance.");
		((All<ICharacter>)runtime.World.Actors).Add(actor);
		Require(template.BuildingCommand(actor, new StringStack("local off")), "Could not restore independent mode.");
		Console.WriteLine("R-P05/R-P06-presence=passed same-spell:refused different-spell:refused zero-then-native-Land:terminal local-absent:terminal creator-auto-load:False");
		child = Cast();
		runtime.World.SaveManager.Flush();
		Tick(60);
		progress = Read(child.TreatmentId);
		Require(cell.EnvironmentState.ScarDamage == 19 && progress.RemainingBudget == 11, "R-P03 reload staging failed.");
		// Parent XML still precedes the last commit. A separate process must use the checkpoint, not stale XML.
		var finalNative = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		RunRejuvenationReaderProcess(new(databaseName, fixture, cell.Id, field.Id, profile.Id, spell.Id, child.TreatmentId,
			19, 11, 1, 540, finalNative.NativeStock, finalNative.PrepaidFraction, true));
		runtime.WorldMock.Verify(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()), Times.Never);
	}

	private sealed record RejuvenationReaderInput(string Database, FixtureIds Fixture, long CellId, long FieldId,
		long ProfileId, long SpellId, Guid TreatmentId, double Scar, double Budget, double Total, double Seconds,
		double Stock, decimal Prepaid, bool Active);

	private static void RunRejuvenationReaderProcess(RejuvenationReaderInput input)
	{
		var info = new ProcessStartInfo(Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet")
		{ UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
		info.ArgumentList.Add(Assembly.GetExecutingAssembly().Location);
		info.ArgumentList.Add("--rejuvenation-reader");
		info.ArgumentList.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(input))));
		using var reader = Process.Start(info) ?? throw new InvalidOperationException("Could not launch the independent repair reader.");
		var output = reader.StandardOutput.ReadToEnd();
		var error = reader.StandardError.ReadToEnd();
		reader.WaitForExit();
		Require(reader.ExitCode == 0, $"R-P03 reader failed: {error} {output}");
		Console.Write(output);
	}

	private static int RunRejuvenationReader(string[] arguments)
	{
		Require(arguments.Length == 1, "The rejuvenation reader requires exactly one owned fixture descriptor.");
		var input = JsonSerializer.Deserialize<RejuvenationReaderInput>(Encoding.UTF8.GetString(Convert.FromBase64String(arguments[0])))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database);
		ConfigureNativeDatabase(database.ConnectionString);
		var runtime = NativeRuntime.Load(input.Fixture, database.ConnectionString);
		ConfigureRejuvenationSpellWorld(runtime, database.ConnectionString);
		((All<ICharacter>)runtime.World.Actors).Remove(runtime.Actor); // Independent mode must never load an absent caster.
		var clock = new HarnessClock();
		runtime.WorldMock.SetupGet(x => x.EffectScheduler).Returns(new EffectScheduler(runtime.World, clock));
		using var coordinator = new EnvironmentalMagicCoordinator(runtime.World, clock);
		runtime.WorldMock.SetupGet(x => x.EnvironmentalMagic).Returns(coordinator);
		using var read = NewIndependentContext(database.ConnectionString);
		var model = read.Cells.Include(x => x.CellOverlays).Include(x => x.CellsMagicResources).Include(x => x.EnvironmentalState)
			.Include(x => x.CellsForagableYields).AsNoTracking().Single(x => x.Id == input.CellId);
		var resources = (All<IMagicResource>)runtime.World.MagicResources;
		foreach (var resource in read.MagicResources.AsNoTracking().Where(x => x.Id != runtime.Resource.Id)) resources.Add(new CappedSimpleMagicResource(resource, runtime.World));
		var profile = new EnvironmentalMagicGenerator(read.MagicGenerators.AsNoTracking().Single(x => x.Id == input.ProfileId), runtime.World);
		((All<IMagicResourceRegenerator>)runtime.World.MagicResourceRegenerators).Add(profile);
		var terrains = new All<ITerrain>();
		terrains.Add(new Terrain(read.Terrains.AsNoTracking().Single(x => x.Id == model.CellOverlays.Single().TerrainId), runtime.World));
		runtime.WorldMock.SetupGet(x => x.Terrains).Returns(terrains);
		var package = new Mock<ICellOverlayPackage>();
		package.SetupGet(x => x.Id).Returns(model.CellOverlays.Single().CellOverlayPackageId);
		package.SetupGet(x => x.RevisionNumber).Returns(1);
		package.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		var packages = new RevisableAll<ICellOverlayPackage>(); packages.Add(package.Object);
		runtime.WorldMock.SetupGet(x => x.CellOverlayPackages).Returns(packages);
		var forages = new RevisableAll<IForagableProfile>();
		forages.Add(new ForagableProfile(read.ForagableProfiles.Include(x => x.EditableItem).Include(x => x.ForagableProfilesMaximumYields)
			.Include(x => x.ForagableProfilesHourlyYieldGains).AsNoTracking().Single(x => x.Id == model.ForagableProfileId), runtime.World));
		runtime.WorldMock.SetupGet(x => x.ForagableProfiles).Returns(forages);
		var room = new Mock<IRoom>();
		room.SetupGet(x => x.Gameworld).Returns(runtime.World); room.SetupGet(x => x.Id).Returns(model.RoomId);
		room.SetupGet(x => x.Areas).Returns(Array.Empty<IArea>()); room.SetupGet(x => x.Zone).Returns(Mock.Of<IZone>());
		var cell = new Cell(model, room.Object);
		var cells = new All<ICell>(); cells.Add(cell); runtime.WorldMock.SetupGet(x => x.Cells).Returns(cells);
		cell.PostLoadTasks(model);
		coordinator.Register(cell);
		for (var i = 0; i < 10; i++) coordinator.Pump();
		var store = new DatabaseEnvironmentalMagicOperationStore();
		var p = store.FindTreatment(input.TreatmentId)!;
		Require(cell.EnvironmentState.ScarDamage == input.Scar && p.RemainingBudget == input.Budget && p.TotalRepaired == input.Total &&
			p.RemainingSeconds == input.Seconds && coordinator.ActiveTreatmentCount == (input.Active ? 1 : 0), "R-P03 load advanced repair, reset budget, or lost the parent/child.");
		var native = NativeOrganicRuntime.Load(database.ConnectionString, input.FieldId).Field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		Require(native.NativeStock == input.Stock && native.PrepaidFraction == input.Prepaid, "R-P03 reload rewrote native accounting.");
		clock.Advance(TimeSpan.FromSeconds(60));
		// A fresh process may spend its first soft-budget slice warming unrelated environment code.
		// Drain bounded heartbeat slices at the same monotonic time, without granting more elapsed work.
		for (var i = 0; i < 10; i++) coordinator.Pump();
		p = store.FindTreatment(input.TreatmentId)!;
		var expectedRepair = input.Active ? 1 : 0;
		Require(p.RemainingBudget == input.Budget - expectedRepair && p.TotalRepaired == input.Total + expectedRepair && cell.EnvironmentState.ScarDamage == input.Scar - expectedRepair,
			$"R-P03 fresh online interval did not respect resumed/terminated state: scar {cell.EnvironmentState.ScarDamage}, budget {p.RemainingBudget}, repaired {p.TotalRepaired}, status {p.Status}, visits {coordinator.TreatmentVisits}, diagnostic {coordinator.InspectTreatment(cell, p.Id)?.Diagnostic}.");
		runtime.WorldMock.Verify(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()), Times.Never);
		Console.WriteLine($"R-P03=passed separate-process:True active:{input.Active} stale-parent-XML:True caster-absent:True load-scar:{input.Scar} load-budget:{input.Budget} resumed-scar:{cell.EnvironmentState.ScarDamage} resumed-budget:{p.RemainingBudget} no-saving-shutdown:True");
		return 0;
	}

	private sealed class RejuvenationAcceptanceStore : IEnvironmentalMagicOperationStore
	{
		private readonly DatabaseEnvironmentalMagicOperationStore _inner = new();
		public bool LoseNextAcknowledgement { get; set; }
		public LandRejuvenationProgress? FindTreatment(Guid id) => _inner.FindTreatment(id);
		public IReadOnlyList<LandRejuvenationProgress> TreatmentsFor(long cellId) => _inner.TreatmentsFor(cellId);
		public void SaveTreatment(LandRejuvenationProgress progress, long? revision) => _inner.SaveTreatment(progress, revision);
		public StoredEnvironmentalMagicOperation? Find(Guid id) => _inner.Find(id);
		public StoredEnvironmentalMagicState Load(Cell cell) => _inner.Load(cell);
		public void Commit(Cell cell, EnvironmentalMagicOperationRequest request, EnvironmentalMagicOperationResult result,
			EnvironmentalMagicState state, DateTimeOffset utc, IReadOnlyDictionary<IMagicResource, double>? balances = null)
			=> _inner.Commit(cell, request, result, state, utc, balances);
		public void CommitRepair(Cell cell, EnvironmentalMagicOperationRequest request, EnvironmentalMagicOperationResult result,
			EnvironmentalMagicState state, DateTimeOffset utc, IReadOnlyDictionary<IMagicResource, double> balances,
			LandRejuvenationProgress progress, long revision)
		{
			_inner.CommitRepair(cell, request, result, state, utc, balances, progress, revision);
			if (!LoseNextAcknowledgement) return;
			LoseNextAcknowledgement = false;
			throw new IOException("Owned acceptance probe: repair committed, response lost.");
		}
	}
}
