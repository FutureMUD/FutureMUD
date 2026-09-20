#nullable enable

using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Economy.Property;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.Magic;
using MudSharp.Magic.Environment;
using MudSharp.Magic.Generators;
using MudSharp.PerceptionEngine;
using MudSharp.Work.Agriculture;
using MudSharp.Work.Crafts;
using MudSharp.Work.Crafts.Inputs;
using MudSharp.Work.Foraging;
using MySql.Data.MySqlClient;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static int RunNativeOrganicAcceptanceChecks()
	{
		var total = Stopwatch.StartNew();
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString);
		Console.WriteLine($"landCreated={database.Name}");
		Console.WriteLine($"landServerVersion={database.ServerVersion}");
		var fixture = NativeOrganicFixtureSeed.Create(database);
		var coordinatorProfileId = RunOrganicProfileAuthoringAndClone(database.ConnectionString,
			fixture.EnvironmentalResourceId);
		foreach (var (fieldId, factor, expected) in new[]
		         {
			         (fixture.PendingPastureFieldId, 0.5, 25),
			         (fixture.ZeroPastureFieldId, 0.0, 0)
		         })
		{
			Require(ReadPastureAssessment(database.ConnectionString, fieldId) == "pending",
				"C-P01 did not save the pending assessment before first pasture use.");
			var pending = NativeOrganicRuntime.Load(database.ConnectionString, fieldId, initialFactor: factor);
			var establish = NativeOperation(pending.World, AgricultureOperationType.Graze,
				AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture);
			Require(pending.Field.ApplyOperation(establish, null!, null!, false, out var establishResult),
				establishResult);
			pending.SaveManager.Flush();
			var observed = NativeOrganicObservation.Read(database.ConnectionString, fieldId,
				NativeOrganicSourceKind.Pasture);
			Require(observed.Stock == expected && observed.CurrentUse == AgricultureFieldUse.Pasture &&
			        ReadPastureAssessment(database.ConnectionString, fieldId) == "assessed",
				"C-P01 saved pasture stock or assessment marker did not match first establishment.");
			var reconstructedPasture = NativeOrganicRuntime.Load(database.ConnectionString, fieldId,
				initialFactor: factor);
			Require(reconstructedPasture.Field.Pasture == expected &&
			        reconstructedPasture.Field.InspectNativeOrganicSource(NativeOrganicSourceKind.Pasture).NativeStock == expected,
				"C-P01 reconstruction changed assessed pasture stock.");
			Console.WriteLine($"C-P01=passed field:{fieldId} staged:50 factor:{factor} expected:{expected} observed:{observed.Stock} assessment:assessed");
		}
		foreach (var (cellId, factor, expected) in new[]
		         { (fixture.ConstructorHalfCellId, 0.5, 25), (fixture.ConstructorZeroCellId, 0.0, 0) })
		{
			var owner = NativeOrganicRuntime.Load(database.ConnectionString, fixture.FractionalFieldId,
				initialFactor: factor);
			var newCell = new Mock<ICell>(MockBehavior.Loose);
			newCell.SetupGet(x => x.Id).Returns(cellId);
			newCell.SetupGet(x => x.Gameworld).Returns(owner.World);
			newCell.Setup(x => x.CurrentTemperature(It.IsAny<IPerceiver>())).Returns(20.0);
			var newField = new AgricultureField(newCell.Object, owner.Field.Profile);
			Require(ReadPastureAssessment(database.ConnectionString, newField.Id) == "pending",
				"C-P01 constructor did not persist pending assessment on its first insert.");
			var firstSave = NativeOrganicObservation.Read(database.ConnectionString, newField.Id,
				NativeOrganicSourceKind.Pasture);
			Require(firstSave.Stock == 50 && firstSave.CurrentUse == AgricultureFieldUse.Fallow,
				"C-P01 constructor initial save did not retain staged pasture 50.");
			var reconstructedConstructor = NativeOrganicRuntime.Load(database.ConnectionString, newField.Id,
				initialFactor: factor);
			var establish = NativeOperation(reconstructedConstructor.World, AgricultureOperationType.Graze,
				AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture);
			Require(reconstructedConstructor.Field.ApplyOperation(establish, null!, null!, false, out var result), result);
			reconstructedConstructor.SaveManager.Flush();
			var assessed = NativeOrganicObservation.Read(database.ConnectionString, newField.Id,
				NativeOrganicSourceKind.Pasture);
			Require(assessed.Stock == expected && ReadPastureAssessment(database.ConnectionString, newField.Id) == "assessed",
				"C-P01 constructor field did not save its first assessment.");
			var reentered = NativeOrganicRuntime.Load(database.ConnectionString, newField.Id,
				initialFactor: factor);
			Require(reentered.Field.Pasture == expected,
				"C-P01 reload reassessed the constructor field.");
			var exitPasture = NativeOperation(reentered.World, AgricultureOperationType.Clear,
				AgricultureFieldUse.Pasture, AgricultureFieldUse.Fallow);
			Require(reentered.Field.ApplyOperation(exitPasture, null!, null!, false, out var exitResult),
				exitResult);
			var reestablish = NativeOperation(reentered.World, AgricultureOperationType.Graze,
				AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture);
			Require(reentered.Field.ApplyOperation(reestablish, null!, null!, false, out var reentryResult),
				reentryResult);
			reentered.SaveManager.Flush();
			var afterReentry = NativeOrganicObservation.Read(database.ConnectionString, newField.Id,
				NativeOrganicSourceKind.Pasture);
			Require(afterReentry.Stock == expected && ReadPastureAssessment(database.ConnectionString, newField.Id) == "assessed",
				"C-P01 re-entry reassessed or refilled the constructor field.");
			Console.WriteLine($"C-P01-constructor=passed field:{newField.Id} cell:{cellId} initial:50 factor:{factor} saved:{assessed.Stock} assessment:assessed");
		}
		RunNativeOrganicCoordinatorAcceptance(database, fixture.CoordinatorCropFieldId, coordinatorProfileId,
			fixture.EnvironmentalResourceId);
		var orchardRuntime = NativeOrganicRuntime.Load(database.ConnectionString, fixture.OrchardFieldId,
			recoveryFactor: 0.5, perennial: true);
		var orchardDebit = PlanNativeDebit(orchardRuntime.Field, NativeOrganicSourceKind.Crop, 0.25m);
		Require(orchardRuntime.Field.TryApplyNativeOrganicDebit(orchardDebit, out var orchardReason), orchardReason);
		var orchardHarvest = NativeOperation(orchardRuntime.World, AgricultureOperationType.Harvest,
			AgricultureFieldUse.Orchard, AgricultureFieldUse.Orchard);
		var orchardOutcome = AgricultureWorkOutcome.FromSkill(600.0, 10.0, 0.0, 0.0);
		Require(orchardOutcome.CropYieldDelta == 5, "C-P02 fixture did not produce the required +5 work bonus.");
		Require(orchardRuntime.Field.ApplyOperation(orchardHarvest, null!, null!, false,
			orchardOutcome, out var orchardResult), orchardResult);
		orchardRuntime.SaveManager.Flush();
		var afterOrchardHarvest = NativeOrganicObservation.Read(database.ConnectionString,
			fixture.OrchardFieldId, NativeOrganicSourceKind.Crop);
		Require(afterOrchardHarvest.Stock == 81 && afterOrchardHarvest.Prepaid == 0.75m &&
		        afterOrchardHarvest.YieldRemainder == 0.5m && afterOrchardHarvest.Generation == 1 &&
		        afterOrchardHarvest.CurrentUse == AgricultureFieldUse.Orchard,
			"C-P02 did not persist the retained orchard's near-cap harvest and distinct fractions.");
		var orchardReloaded = NativeOrganicRuntime.Load(database.ConnectionString, fixture.OrchardFieldId,
			recoveryFactor: 0.5, perennial: true);
		Require(orchardReloaded.Field.CropYieldPotential == 81 &&
		        orchardReloaded.Field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop).PrepaidFraction == 0.75m,
			"C-P02 orchard reconstruction did not retain native stock or prepaid credit.");
		orchardReloaded.Field.DailyTick();
		orchardReloaded.SaveManager.Flush();
		var afterOrchardRecovery = NativeOrganicObservation.Read(database.ConnectionString,
			fixture.OrchardFieldId, NativeOrganicSourceKind.Crop);
		Require(afterOrchardRecovery.Stock == 82 && afterOrchardRecovery.YieldRemainder == 0m &&
		        afterOrchardRecovery.Prepaid == 0.75m,
			"C-P02 later recovery did not continue the saved remainder exactly once.");
		Console.WriteLine($"C-P02=passed field:{fixture.OrchardFieldId} opening:100 prepaidDebit:0.25 harvestBonus:5 factor:0.5 savedStock:{afterOrchardHarvest.Stock} savedPrepaid:{afterOrchardHarvest.Prepaid} savedYieldRemainder:{afterOrchardHarvest.YieldRemainder} recoveredStock:{afterOrchardRecovery.Stock}");

		var p01Watch = Stopwatch.StartNew();
		var p01 = NativeOrganicRuntime.Load(database.ConnectionString, fixture.FractionalFieldId);
		var opening = p01.Field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		Require(opening.NativeStock == 10.0 && opening.PrepaidFraction == 0m,
			"Y-P01 did not begin with the expected native crop owner state.");
		var quarter = PlanNativeDebit(p01.Field, NativeOrganicSourceKind.Crop, 0.25m);
		Require(p01.Field.TryApplyNativeOrganicDebit(quarter, out var applyReason),
			$"Y-P01 native owner rejected the quarter debit: {applyReason}");
		p01.SaveManager.Flush();
		var afterQuarter = NativeOrganicObservation.Read(database.ConnectionString, fixture.FractionalFieldId,
			NativeOrganicSourceKind.Crop);
		Require(afterQuarter.Stock == 9 && afterQuarter.Prepaid == 0.75m,
			"Y-P01 independent reader did not observe the reduced stock and prepaid fraction in one owner checkpoint.");
		RunNativeOrganicReaderProcess(database.Name, fixture.FractionalFieldId);
		var afterRemainder = NativeOrganicObservation.Read(database.ConnectionString, fixture.FractionalFieldId,
			NativeOrganicSourceKind.Crop);
		Require(afterRemainder.Stock == 9 && afterRemainder.Prepaid == 0m,
			"Y-P01 parent reader did not observe the separately reconstructed remainder debit exactly once.");
		p01Watch.Stop();
		Console.WriteLine(
			$"Y-P01=passed field:{fixture.FractionalFieldId} cell:{fixture.FractionalCellId} generation:{afterRemainder.Generation} openingStock:10 quarterObservedStock:{afterQuarter.Stock} quarterObservedPrepaid:{afterQuarter.Prepaid} closingStock:{afterRemainder.Stock} closingPrepaid:{afterRemainder.Prepaid} elapsedMs:{p01Watch.ElapsedMilliseconds}");

		var p02Watch = Stopwatch.StartNew();
		var p02 = NativeOrganicRuntime.Load(database.ConnectionString, fixture.FractionalFieldId,
			recoveryFactor: 0.25);
		var oldGeneration = p02.Field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop).Lifecycle!.Generation;
		var clear = NativeOperation(p02.World, AgricultureOperationType.Clear, AgricultureFieldUse.Crop,
			AgricultureFieldUse.Fallow);
		Require(p02.Field.ApplyOperation(clear, null!, null!, false, out var clearResult), clearResult);
		var sow = NativeOperation(p02.World, AgricultureOperationType.Sow, AgricultureFieldUse.Fallow,
			AgricultureFieldUse.Crop, AgricultureTargetType.Crop);
		Require(p02.Field.ApplyOperation(sow, p02.Crop, null!, false, out var sowResult), sowResult);
		Require(p02.Field.ConsumeCropYield(50, out var consumeReason), consumeReason);
		p02.Field.DailyTick();
		p02.SaveManager.Flush();
		var afterLifecycleTick = NativeOrganicObservation.Read(database.ConnectionString, fixture.FractionalFieldId,
			NativeOrganicSourceKind.Crop);
		Require(afterLifecycleTick.Generation == oldGeneration + 1 && afterLifecycleTick.Stock == 50 &&
		        afterLifecycleTick.HealthRemainder == 0.25m && afterLifecycleTick.YieldRemainder == 0.25m &&
		        afterLifecycleTick.GrowthDays == 1,
			"Y-P02 replacement/tick state was not persisted with the new lifecycle and fractional progress.");
		var p02Reloaded = NativeOrganicRuntime.Load(database.ConnectionString, fixture.FractionalFieldId,
			recoveryFactor: 0.25);
		var reconstructed = p02Reloaded.Field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		Require(reconstructed.Lifecycle!.Generation == afterLifecycleTick.Generation &&
		        reconstructed.RecoveryRemainders.Health == 0.25m &&
		        reconstructed.RecoveryRemainders.Yield == 0.25m,
			"Y-P02 reconstructed owner did not retain lifecycle and fractional recovery progress.");
		p02Reloaded.Field.DailyTick();
		p02Reloaded.SaveManager.Flush();
		var afterSecondTick = NativeOrganicObservation.Read(database.ConnectionString, fixture.FractionalFieldId,
			NativeOrganicSourceKind.Crop);
		Require(afterSecondTick.Stock == 50 && afterSecondTick.HealthRemainder == 0.5m &&
		        afterSecondTick.YieldRemainder == 0.5m && afterSecondTick.GrowthDays == 2,
			"Y-P02 reconstructed native tick did not continue the saved scar-factor progress.");
		p02Watch.Stop();
		Console.WriteLine(
			$"Y-P02=passed field:{fixture.FractionalFieldId} oldGeneration:{oldGeneration} newGeneration:{afterSecondTick.Generation} recoveryFactor:0.25 healthRemainder:{afterSecondTick.HealthRemainder} yieldRemainder:{afterSecondTick.YieldRemainder} growthDays:{afterSecondTick.GrowthDays} elapsedMs:{p02Watch.ElapsedMilliseconds}");

		var p03Watch = Stopwatch.StartNew();
		var cropRuntime = NativeOrganicRuntime.Load(database.ConnectionString, fixture.ConsumerCropFieldId);
		var firstCropDebit = PlanNativeDebit(cropRuntime.Field, NativeOrganicSourceKind.Crop, 0.25m);
		Require(cropRuntime.Field.TryApplyNativeOrganicDebit(firstCropDebit, out applyReason), applyReason);
		var preCraftPlan = PlanNativeDebit(cropRuntime.Field, NativeOrganicSourceKind.Crop, 0.5m);
		var craftInput = NativeAgricultureInput(cropRuntime.World, cropRuntime.Crop.Id, 5);
		craftInput.ReserveInput(cropRuntime.Cell);
		Require(!cropRuntime.Field.TryApplyNativeOrganicDebit(preCraftPlan, out _),
			"Y-P03 accepted a native plan made stale by a real craft input reservation.");
		var postCraftPlan = PlanNativeDebit(cropRuntime.Field, NativeOrganicSourceKind.Crop, 0.5m);
		Require(cropRuntime.Field.TryApplyNativeOrganicDebit(postCraftPlan, out applyReason), applyReason);
		var harvest = NativeOperation(cropRuntime.World, AgricultureOperationType.Harvest,
			AgricultureFieldUse.Crop, AgricultureFieldUse.Fallow);
		Require(cropRuntime.Field.ApplyOperation(harvest, null!, null!, false, out var harvestResult), harvestResult);
		cropRuntime.SaveManager.Flush();
		var afterHarvest = NativeOrganicObservation.Read(database.ConnectionString, fixture.ConsumerCropFieldId,
			NativeOrganicSourceKind.Crop);
		Require(afterHarvest.CurrentUse == AgricultureFieldUse.Fallow && !afterHarvest.HasCrop &&
		        afterHarvest.Prepaid == 0m,
			"Y-P03 annual harvest did not end the lifecycle and discard its old prepaid fraction.");

		var pastureRuntime = NativeOrganicRuntime.Load(database.ConnectionString, fixture.GrazingFieldId);
		var firstPastureDebit = PlanNativeDebit(pastureRuntime.Field, NativeOrganicSourceKind.Pasture, 0.25m);
		Require(pastureRuntime.Field.TryApplyNativeOrganicDebit(firstPastureDebit, out applyReason), applyReason);
		var preGrazePlan = PlanNativeDebit(pastureRuntime.Field, NativeOrganicSourceKind.Pasture, 0.5m);
		pastureRuntime.Field.DailyTick();
		Require(!pastureRuntime.Field.TryApplyNativeOrganicDebit(preGrazePlan, out _),
			"Y-P03 accepted a native plan made stale by real herd grazing.");
		var postGrazePlan = PlanNativeDebit(pastureRuntime.Field, NativeOrganicSourceKind.Pasture, 0.5m);
		Require(pastureRuntime.Field.TryApplyNativeOrganicDebit(postGrazePlan, out applyReason), applyReason);
		pastureRuntime.SaveManager.Flush();
		var afterGrazing = NativeOrganicObservation.Read(database.ConnectionString, fixture.GrazingFieldId,
			NativeOrganicSourceKind.Pasture);
		Require(afterGrazing.Stock == 7 && afterGrazing.Prepaid == 0.25m,
			"Y-P03 did not persist the single shared pasture after debit, native grazing and remainder use.");
		p03Watch.Stop();
		Console.WriteLine(
			$"Y-P03=passed cropField:{fixture.ConsumerCropFieldId} craftYieldConsumed:5 annualHarvestRemovedCrop:{!afterHarvest.HasCrop} pastureField:{fixture.GrazingFieldId} openingPasture:10 quarterDebitStock:9 nativeGrazeDebit:2 closingPasture:{afterGrazing.Stock} closingPrepaid:{afterGrazing.Prepaid} elapsedMs:{p03Watch.ElapsedMilliseconds}");

		var failureWatch = Stopwatch.StartNew();
		var beforeFailure = afterGrazing;
		var failurePlan = PlanNativeDebit(pastureRuntime.Field, NativeOrganicSourceKind.Pasture, 0.25m);
		Require(pastureRuntime.Field.TryApplyNativeOrganicDebit(failurePlan, out applyReason), applyReason);
		using (var connection = database.OpenOwnedConnection())
		using (var command = connection.CreateCommand())
		{
			command.CommandText =
				"CREATE TRIGGER `land_harness_fail_field_update` BEFORE UPDATE ON `AgricultureFields` FOR EACH ROW SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'owned harness commit failure';";
			command.ExecuteNonQuery();
		}
		var failureObserved = false;
		try
		{
			pastureRuntime.SaveManager.Flush();
		}
		catch (Exception)
		{
			failureObserved = true;
		}
		finally
		{
			using var connection = database.OpenOwnedConnection();
			using var command = connection.CreateCommand();
			command.CommandText = "DROP TRIGGER IF EXISTS `land_harness_fail_field_update`;";
			command.ExecuteNonQuery();
		}
		Require(failureObserved && pastureRuntime.Field.Changed &&
		        pastureRuntime.SaveManager.IsQueued(pastureRuntime.Field),
			"Y-T24 did not retain a retryable native owner after the provider rejected its commit.");
		var rolledBack = NativeOrganicObservation.Read(database.ConnectionString, fixture.GrazingFieldId,
			NativeOrganicSourceKind.Pasture);
		Require(rolledBack.Stock == beforeFailure.Stock && rolledBack.Prepaid == beforeFailure.Prepaid &&
		        rolledBack.Revision == beforeFailure.Revision,
			"Y-T24 provider failure did not roll back stock/accounting atomically.");
		pastureRuntime.SaveManager.Flush();
		var afterRetry = NativeOrganicObservation.Read(database.ConnectionString, fixture.GrazingFieldId,
			NativeOrganicSourceKind.Pasture);
		Require(afterRetry.Stock == 7 && afterRetry.Prepaid == 0m &&
		        afterRetry.Revision > rolledBack.Revision,
			"Y-T24 retry did not persist the recovered native owner state.");
		failureWatch.Stop();
		Console.WriteLine(
			$"Y-T24-provider=passed field:{fixture.GrazingFieldId} rolledBackStock:{rolledBack.Stock} rolledBackPrepaid:{rolledBack.Prepaid} retryStock:{afterRetry.Stock} retryPrepaid:{afterRetry.Prepaid} elapsedMs:{failureWatch.ElapsedMilliseconds}");
		Console.WriteLine("landHarnessSubstitutions=world catalogues and deterministic weather; C-P03 uses a persisted overlay and forage profile with a real Cell/coordinator/agriculture owner and a controllable apiary candidate; other land cases use an ecological scalar fixture; SaveManager and independent EF/MySQL observations are production paths");
		total.Stop();
		Console.WriteLine($"landHarnessElapsedMs={total.ElapsedMilliseconds}");
		return 0;
	}

	private static long RunOrganicProfileAuthoringAndClone(string connectionString, long resourceId)
	{
		using var context = NewIndependentContext(connectionString);
		var resourceModel = context.MagicResources.AsNoTracking().Single(x => x.Id == resourceId);
		var world = new Mock<IFuturemud>(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
		var saveManager = new SaveManager();
		world.SetupGet(x => x.SaveManager).Returns(saveManager);
		world.SetupGet(x => x.EnvironmentalMagic).Returns(Mock.Of<IEnvironmentalMagicService>(x =>
			x.UtcNow == DateTimeOffset.UnixEpoch));
		var resource = new CappedSimpleMagicResource(resourceModel, world.Object);
		var resources = new All<IMagicResource>();
		resources.Add(resource);
		world.SetupGet(x => x.MagicResources).Returns(resources);
		world.SetupGet(x => x.MagicResourceRegenerators).Returns(new All<IMagicResourceRegenerator>());
		world.SetupGet(x => x.FutureProgs).Returns(new All<MudSharp.FutureProg.IFutureProg>());

		var actor = new Mock<ICharacter>(MockBehavior.Loose);
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		actor.Setup(x => x.GetFormat(It.IsAny<Type>())).Returns<Type>(CultureInfo.InvariantCulture.GetFormat);
		actor.SetupGet(x => x.Account).Returns(Mock.Of<IAccount>(x => x.InnerLineFormatLength == 100));

		var created = BaseMagicResourceGenerator.LoadFromBuilderInput(actor.Object,
			new StringStack($"environmental {resourceId} Land Harness Organic Profile")) as EnvironmentalMagicGenerator ??
			throw new InvalidOperationException(
				"Y-T02 could not create an environmental profile through the real builder path.");
		Require(created.BuildingCommand(actor.Object, new StringStack("organic source add forage herbs")),
			"Y-T02 could not author the forage declaration through the real profile editor.");
		Require(created.BuildingCommand(actor.Object, new StringStack("organic source add crop")),
			"Y-T02 could not author the crop declaration through the real profile editor.");
		Require(created.BuildingCommand(actor.Object,
			new StringStack("organic penalty forage 1 - scardamage / 20")),
			"Y-T02 could not author the forage penalty through the real profile editor.");
		Require(created.BuildingCommand(actor.Object,
			new StringStack("organic penalty crophealth 2.0 / baselineincrease")),
			"C-P03 could not author the dynamic crop-health penalty through the real profile editor.");
		saveManager.Flush();

		var savedModel = context.MagicGenerators.AsNoTracking().Single(x => x.Id == created.Id);
		var reloaded = new EnvironmentalMagicGenerator(savedModel, world.Object);
		Require(reloaded.OrganicValidationErrors.Count == 0 && reloaded.OrganicSources.Count == 2 &&
		        reloaded.OrganicPenalties.Count == 2,
			"Y-T02 did not reload the authored organic subtree intact.");
		var clone = (EnvironmentalMagicGenerator)reloaded.Clone("Land Harness Organic Clone");
		Require(clone.BuildingCommand(actor.Object, new StringStack("organic source remove crop")),
			"Y-T02 could not independently edit the cloned organic definition.");
		saveManager.Flush();

		using var observation = NewIndependentContext(connectionString);
		var originalDefinition = XElement.Parse(observation.MagicGenerators.AsNoTracking()
			.Single(x => x.Id == created.Id).Definition);
		var cloneDefinition = XElement.Parse(observation.MagicGenerators.AsNoTracking()
			.Single(x => x.Id == clone.Id).Definition);
		var originalSources = originalDefinition.Element("Organic")!.Element("Sources")!.Elements("Source").Count();
		var cloneSources = cloneDefinition.Element("Organic")!.Element("Sources")!.Elements("Source").Count();
		Require(originalSources == 2 && cloneSources == 1,
			"Y-T02 clone editing changed the original profile or did not save the independent clone.");
		Console.WriteLine(
			$"Y-T02=passed createdProfile:{created.Id} clonedProfile:{clone.Id} originalSources:{originalSources} cloneSources:{cloneSources} penalties:{clone.OrganicPenalties.Count}");
		return created.Id;
	}

	private static void RunNativeOrganicCoordinatorAcceptance(TestDatabase database, long fieldId, long profileId,
		long resourceId)
	{
		var baseRuntime = NativeOrganicRuntime.Load(database.ConnectionString, fieldId);
		var world = Mock.Get(baseRuntime.World);
		var crop = Mock.Get(baseRuntime.Crop);
		crop.SetupGet(x => x.PollinationDependency).Returns(AgriculturePollinationDependency.Beneficial);
		crop.SetupGet(x => x.PollinationHealthBonus).Returns(3);
		using (var binding = NewIndependentContext(database.ConnectionString))
		{
			var persistedCell = binding.Cells.Single(x => x.Id == baseRuntime.Field.Cell.Id);
			persistedCell.EnvironmentalMagicBindingMode = (int)EnvironmentalMagicBindingMode.Explicit;
			persistedCell.EnvironmentalMagicProfileId = profileId;
			binding.SaveChanges();
		}
		using var read = NewIndependentContext(database.ConnectionString);
		var fieldModel = read.AgricultureFields.Include(x => x.AgricultureFieldCrop)
			.AsNoTracking().Single(x => x.Id == fieldId);
		var cellModel = read.Cells.Include(x => x.CellOverlays).Include(x => x.CellsForagableYields)
			.AsNoTracking().Single(x => x.Id == fieldModel.CellId);
		var forageModel = read.ForagableProfiles.Include(x => x.EditableItem)
			.Include(x => x.ForagableProfilesMaximumYields)
			.Include(x => x.ForagableProfilesHourlyYieldGains)
			.AsNoTracking().Single(x => x.Id == cellModel.ForagableProfileId);
		var generatorModel = read.MagicGenerators.AsNoTracking().Single(x => x.Id == profileId);
		var resourceModel = read.MagicResources.AsNoTracking().Single(x => x.Id == resourceId);
		var resources = new All<IMagicResource>();
		resources.Add(new CappedSimpleMagicResource(resourceModel, world.Object));
		world.SetupGet(x => x.MagicResources).Returns(resources);
		var generators = new All<IMagicResourceRegenerator>();
		generators.Add(new EnvironmentalMagicGenerator(generatorModel, world.Object));
		world.SetupGet(x => x.MagicResourceRegenerators).Returns(generators);
		world.SetupGet(x => x.FutureProgs).Returns(new All<MudSharp.FutureProg.IFutureProg>());
		var terrain = new Terrain(read.Terrains.AsNoTracking()
			.Single(x => x.Id == cellModel.CellOverlays.Single().TerrainId), world.Object);
		var terrains = new All<ITerrain>();
		terrains.Add(terrain);
		world.SetupGet(x => x.Terrains).Returns(terrains);
		var package = new Mock<ICellOverlayPackage>();
		package.SetupGet(x => x.Id).Returns(cellModel.CellOverlays.Single().CellOverlayPackageId);
		package.SetupGet(x => x.RevisionNumber).Returns(1);
		package.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		var packages = new RevisableAll<ICellOverlayPackage>();
		packages.Add(package.Object);
		world.SetupGet(x => x.CellOverlayPackages).Returns(packages);
		var forageProfiles = new RevisableAll<IForagableProfile>();
		forageProfiles.Add(new ForagableProfile(forageModel, world.Object));
		world.SetupGet(x => x.ForagableProfiles).Returns(forageProfiles);
		var zone = new Mock<IZone>();
		zone.SetupGet(x => x.Gameworld).Returns(world.Object);
		var room = new Mock<IRoom>();
		room.SetupGet(x => x.Gameworld).Returns(world.Object);
		room.SetupGet(x => x.Id).Returns(cellModel.RoomId);
		room.SetupGet(x => x.Zone).Returns(zone.Object);
		room.SetupGet(x => x.Areas).Returns(Array.Empty<IArea>());
		var cell = new Cell(cellModel, room.Object);
		cell.PostLoadTasks(cellModel);
		var cells = new All<ICell>();
		cells.Add(cell);
		world.SetupGet(x => x.Cells).Returns(cells);
		var fields = new All<IAgricultureField>();
		world.SetupGet(x => x.AgricultureFields).Returns(fields);
		world.SetupGet(x => x.HeartbeatManager).Returns(new HeartbeatManager(world.Object));
		using var coordinator = new EnvironmentalMagicCoordinator(world.Object);
		world.SetupGet(x => x.EnvironmentalMagic).Returns(coordinator);
		var field = new AgricultureField(fieldModel, world.Object);
		fields.Add(field);
		coordinator.Initialise();
		Require(cell.HasForagableProfile,
			$"C-P03 did not resolve persisted forage profile {cellModel.ForagableProfileId} for cell {cell.Id}.");
		Require(cell.GetForagableYield("herbs") == 100.0,
			"C-P03 did not load the native forage maximum before ordinary consumption.");
		cell.ConsumeYield("herbs", 10.0);
		Require(cell.GetForagableYield("herbs") == 90.0 && baseRuntime.SaveManager.IsQueued(cell),
			"C-P03 ordinary forage consumption did not queue the native cell owner.");
		baseRuntime.SaveManager.Flush();
		Require(ReadForageYield(database.ConnectionString, cell.Id) == 90.0,
			"C-P03 forage setup did not persist native stock before conversion.");
		Require(coordinator.TryPlanOrganicDebit(cell, "forage:herbs", 0.25, out var foragePlan,
			out var forageError), forageError ?? "C-P03 forage conversion failed.");
		var damage = coordinator.ApplyOperation(cell, new EnvironmentalMagicOperationRequest(
			Guid.NewGuid(), 1, "C-P03 forage damage", Damage: 30.0));
		Require(damage.Success, damage.Error ?? "C-P03 forage damage failed.");
		Require(!coordinator.TryPlanOrganicDebit(cell, "forage:herbs", 0.25, out _, out _),
			"C-P03 accepted forage planning with an invalid dynamic penalty.");
		Require(!coordinator.TryApplyOrganicDebit(cell, foragePlan, out _, out _),
			"C-P03 applied a stale forage plan after its penalty became invalid.");
		baseRuntime.SaveManager.Flush();
		Require(ReadForageYield(database.ConnectionString, cell.Id) == 90.0,
			"C-P03 refused forage conversions mutated persisted stock.");
		var repair = coordinator.ApplyOperation(cell, new EnvironmentalMagicOperationRequest(
			Guid.NewGuid(), 1, "C-P03 forage repair", Repair: 30.0));
		Require(repair.Success, repair.Error ?? "C-P03 forage repair failed.");
		Require(coordinator.TryPlanOrganicDebit(cell, "forage:herbs", 0.25, out var correctedForage,
			out forageError), forageError ?? "C-P03 forage conversion failed.");
		Require(coordinator.TryApplyOrganicDebit(cell, correctedForage, out _, out forageError), forageError ?? "C-P03 forage conversion failed.");
		baseRuntime.SaveManager.Flush();
		Require(Math.Abs(ReadForageYield(database.ConnectionString, cell.Id) - 89.75) < 1e-9,
			"C-P03 corrected forage conversion did not persist the exact quarter debit.");
		var apiary = new Mock<IAgricultureFieldApiary>();
		apiary.SetupGet(x => x.PollinationRadius).Returns(1);
		apiary.SetupGet(x => x.PollinationStrength).Returns(50);
		var pollinationActive = true;
		var pollinator = new Mock<IAgricultureField>();
		pollinator.SetupGet(x => x.Cell).Returns(cell);
		pollinator.SetupGet(x => x.HasActiveApiary).Returns(true);
		pollinator.SetupGet(x => x.IsApiaryHappy).Returns(() => pollinationActive);
		pollinator.SetupGet(x => x.Apiary).Returns(apiary.Object);
		coordinator.RefreshPollinationCandidate(pollinator.Object);
		Require(field.InspectCurrentOrganicRecoveryContext(NativeOrganicPenaltyChannel.CropHealthRecovery)?
			.BaselineIncrease == 4.0, "C-P03 did not supply the native pollinated health baseline of four.");
		Require(coordinator.TryPlanOrganicDebit(cell, "crop", 0.25, out var initial, out var error),
			error ?? "C-P03 initial crop conversion failed.");
		Require(coordinator.TryApplyOrganicDebit(cell, initial, out _, out error), error ?? "C-P03 crop conversion failed.");
		baseRuntime.SaveManager.Flush();
		var prepaid = NativeOrganicObservation.Read(database.ConnectionString, fieldId,
			NativeOrganicSourceKind.Crop);
		Require(prepaid.Stock == 9 && prepaid.Prepaid == 0.75m,
			"C-P03 initial real-coordinator debit did not persist native stock and prepaid credit.");
		Require(coordinator.TryPlanOrganicDebit(cell, "crop", 0.25, out var planned, out error), error ?? "C-P03 crop conversion failed.");
		pollinationActive = false;
		Require(!coordinator.TryPlanOrganicDebit(cell, "crop", 0.25, out _, out _),
			"C-P03 accepted planning with actual invalid factor 2 at baseline one.");
		Require(!coordinator.TryApplyOrganicDebit(cell, planned, out _, out _),
			"C-P03 applied a previously valid plan after its native context became invalid.");
		var refused = NativeOrganicObservation.Read(database.ConnectionString, fieldId,
			NativeOrganicSourceKind.Crop);
		Require(refused.Stock == prepaid.Stock && refused.Prepaid == prepaid.Prepaid &&
		        refused.Revision == prepaid.Revision,
			"C-P03 refusal changed persisted native stock, fraction or revision.");
		pollinationActive = true;
		Require(coordinator.TryPlanOrganicDebit(cell, "crop", 0.25, out var corrected, out error), error ?? "C-P03 crop conversion failed.");
		Require(coordinator.TryApplyOrganicDebit(cell, corrected, out _, out error), error ?? "C-P03 crop conversion failed.");
		baseRuntime.SaveManager.Flush();
		var accepted = NativeOrganicObservation.Read(database.ConnectionString, fieldId,
			NativeOrganicSourceKind.Crop);
		Require(accepted.Stock == 9 && accepted.Prepaid == 0.5m && accepted.Revision > refused.Revision,
			"C-P03 corrected conversion did not persist exactly one prepaid-credit debit.");
		Console.WriteLine($"C-P03=passed field:{fieldId} cell:{cell.Id} profile:{profileId} forage:90->90->89.75 baseline:4->1->4 persistedStock:{prepaid.Stock}->{refused.Stock}->{accepted.Stock} prepaid:{prepaid.Prepaid}->{refused.Prepaid}->{accepted.Prepaid} revision:{prepaid.Revision}->{refused.Revision}->{accepted.Revision}");
	}

	private static double ReadForageYield(string connectionString, long cellId)
	{
		using var context = NewIndependentContext(connectionString);
		return context.CellsForagableYields.AsNoTracking()
			.Single(x => x.CellId == cellId && x.ForagableType == "herbs").Yield;
	}

	private static int RunNativeOrganicReader(string[] arguments)
	{
		if (arguments.Length != 2 || !long.TryParse(arguments[1], out var fieldId))
		{
			throw new ArgumentException("The native organic reader requires the owned database name and field ID.");
		}

		using var database = TestDatabase.OpenExistingOwned(arguments[0]);
		ConfigureNativeDatabase(database.ConnectionString);
		var runtime = NativeOrganicRuntime.Load(database.ConnectionString, fieldId);
		var opening = runtime.Field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		Require(opening.NativeStock == 9.0 && opening.PrepaidFraction == 0.75m,
			"The separate native organic reader did not reconstruct the expected opening owner state.");
		var remainder = PlanNativeDebit(runtime.Field, NativeOrganicSourceKind.Crop, 0.75m);
		Require(runtime.Field.TryApplyNativeOrganicDebit(remainder, out var reason), reason);
		runtime.SaveManager.Flush();
		var observed = NativeOrganicObservation.Read(database.ConnectionString, fieldId,
			NativeOrganicSourceKind.Crop);
		Require(observed.Stock == 9 && observed.Prepaid == 0m,
			"The separate native organic reader did not persist the remainder-only debit.");
		Require(!runtime.Field.TryApplyNativeOrganicDebit(remainder, out _),
			"The separate native organic reader reused an already-applied remainder plan.");
		Console.WriteLine(
			$"landReader=passed field:{fieldId} generation:{observed.Generation} openingStock:9 openingPrepaid:0.75 requested:0.75 wholeDebit:0 closingStock:{observed.Stock} closingPrepaid:{observed.Prepaid}");
		return 0;
	}

	private static void RunNativeOrganicReaderProcess(string databaseName, long fieldId)
	{
		string executable = Assembly.GetExecutingAssembly().Location;
		string host = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet";
		using Process reader = Process.Start(new ProcessStartInfo(host,
			$"\"{executable}\" --land-reader {databaseName} {fieldId}")
		{
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			CreateNoWindow = true
		}) ?? throw new InvalidOperationException("The separate native organic reader process could not be started.");
		string output = reader.StandardOutput.ReadToEnd();
		string error = reader.StandardError.ReadToEnd();
		reader.WaitForExit();
		if (reader.ExitCode != 0)
		{
			throw new InvalidOperationException($"The separate native organic reader failed: {error} {output}".Trim());
		}

		Console.Write(output);
	}

	private static NativeOrganicDebitPlan PlanNativeDebit(AgricultureField field, NativeOrganicSourceKind kind,
		decimal requested)
	{
		var snapshot = field.InspectNativeOrganicSource(kind);
		Require(snapshot.IsEligible && snapshot.Lifecycle is not null,
			$"Native source {kind} was not eligible for a harness debit: {snapshot.Diagnostic}");
		Require(NativeOrganicAccountingMath.TryPlanInteger((int)snapshot.NativeStock, snapshot.PrepaidFraction,
			requested, out var wholeDebit, out var closingPrepaid, out var error), error ?? "Native debit planning failed.");
		return new NativeOrganicDebitPlan(snapshot.Selector, kind, snapshot.Lifecycle!, snapshot.SourceRevision,
			900001L, 1L, requested, snapshot.PrepaidFraction, wholeDebit, closingPrepaid, snapshot.NativeStock);
	}

	private static AgricultureOperation NativeOperation(IFuturemud world, AgricultureOperationType type,
		AgricultureFieldUse requiredUse, AgricultureFieldUse resultUse,
		AgricultureTargetType targetType = AgricultureTargetType.None)
	{
		return new AgricultureOperation(new Db.AgricultureOperation
		{
			Id = 910000L + (long)type,
			Name = $"Harness {type}",
			Description = "Native organic persistence harness operation.",
			OperationType = (int)type,
			TargetType = (int)targetType,
			RequiredUse = (int)requiredUse,
			ResultUse = (int)resultUse,
			Definition = "<Operation />"
		}, world);
	}

	private static string? ReadPastureAssessment(string connectionString, long fieldId)
	{
		using var context = NewIndependentContext(connectionString);
		var definition = context.AgricultureFields.AsNoTracking().Single(x => x.Id == fieldId).Definition;
		return XElement.Parse(definition).Element("NativeOrganicAccounting")?
			.Attribute("pastureAssessment")?.Value;
	}

	private static AgricultureFieldInput NativeAgricultureInput(IFuturemud world, long cropDefinitionId,
		int yieldConsumed)
	{
		var definition = new XElement("Definition",
			new XElement("RequiredUse", AgricultureFieldUse.Crop),
			new XElement("CropDefinitionId", cropDefinitionId),
			new XElement("WoodlandDefinitionId", 0),
			new XElement("MinimumFieldCondition", 0),
			new XElement("MinimumHealth", 0),
			new XElement("MinimumYield", yieldConsumed),
			new XElement("YieldConsumed", yieldConsumed));
		var model = new Db.CraftInput
		{
			Id = 920001L,
			InputQualityWeight = 1.0,
			OriginalAdditionTime = DateTime.UtcNow,
			Definition = definition.ToString()
		};
		return (AgricultureFieldInput)Activator.CreateInstance(typeof(AgricultureFieldInput),
			BindingFlags.Instance | BindingFlags.NonPublic, null,
			[model, new Mock<ICraft>().Object, world], null)!;
	}

	private sealed record NativeOrganicFixtureIds(long FractionalCellId, long FractionalFieldId,
		long ConsumerCropFieldId, long GrazingFieldId, long EnvironmentalResourceId,
		long PendingPastureFieldId, long ZeroPastureFieldId, long OrchardFieldId,
		long ConstructorHalfCellId, long ConstructorZeroCellId, long CoordinatorCropFieldId);

	private static class NativeOrganicFixtureSeed
	{
		public static NativeOrganicFixtureIds Create(TestDatabase database)
		{
			var fractionalBase = FixtureSeed.Create(database, "land_p01", false);
			var cropConsumerBase = FixtureSeed.Create(database, "land_p03_crop", false);
			var pastureBase = FixtureSeed.Create(database, "land_p03_pasture", false);
			var pendingPastureBase = FixtureSeed.Create(database, "land_c_p01_pasture", false);
			var zeroPastureBase = FixtureSeed.Create(database, "land_c_p01_zero", false);
			var orchardBase = FixtureSeed.Create(database, "land_c_p02_orchard", false);
			var constructorHalfBase = FixtureSeed.Create(database, "land_c_p01_constructor_half", false);
			var constructorZeroBase = FixtureSeed.Create(database, "land_c_p01_constructor_zero", false);
			var coordinatorCropBase = FixtureSeed.Create(database, "land_c_p03_crop", false);
			using var context = NewIndependentContext(database.ConnectionString);
			var profile = new Db.AgricultureFieldProfile
			{
				Name = "Land harness field profile",
				Description = "Disposable native organic persistence fixture.",
				Definition = "<Profile />"
			};
			var crop = new Db.AgricultureCropDefinition
			{
				Name = "Land harness crop",
				Description = "Disposable crop.",
				Category = "Harness",
				Definition = "<Crop />"
			};
			var herd = new Db.AgricultureHerdDefinition
			{
				Name = "Land harness grazers",
				Description = "Disposable grazing herd.",
				Definition = "<Herd />"
			};
			context.AgricultureFieldProfiles.Add(profile);
			context.AgricultureCropDefinitions.Add(crop);
			context.AgricultureHerdDefinitions.Add(herd);
			var environmentalResource = context.MagicResources.Single(x => x.Id == fractionalBase.ResourceId);
			environmentalResource.MagicResourceType =
				(int)(MagicResourceType.PlayerResource | MagicResourceType.LocationResource);
			context.SaveChanges();

			var fractional = CropField(fractionalBase.CellId, profile.Id, crop.Id, 10,
				AgricultureCropStage.Growing, nutrients: 100);
			var cropConsumer = CropField(cropConsumerBase.CellId, profile.Id, crop.Id, 20,
				AgricultureCropStage.Harvestable, nutrients: 50);
			var pasture = BaseField(pastureBase.CellId, profile.Id, AgricultureFieldUse.Pasture, 10, 50);
			var pendingPasture = BaseField(pendingPastureBase.CellId, profile.Id,
				AgricultureFieldUse.Fallow, 50, 50);
			pendingPasture.Definition = "<Field><NativeOrganicAccounting version=\"2\" pastureAssessment=\"pending\" /></Field>";
			var zeroPasture = BaseField(zeroPastureBase.CellId, profile.Id,
				AgricultureFieldUse.Fallow, 50, 50);
			zeroPasture.Definition = pendingPasture.Definition;
			var orchard = CropField(orchardBase.CellId, profile.Id, crop.Id, 100,
				AgricultureCropStage.Harvestable, nutrients: 100);
			var coordinatorCrop = CropField(coordinatorCropBase.CellId, profile.Id, crop.Id, 10,
				AgricultureCropStage.Growing, nutrients: 100);
			orchard.CurrentUse = (int)AgricultureFieldUse.Orchard;
			pasture.AgricultureFieldHerds.Add(new Db.AgricultureFieldHerd
			{
				HerdDefinitionId = herd.Id,
				HeadCount = 1,
				Condition = 50.0,
				Definition = "<Herd secondaryYield=\"0\" />"
			});
			context.AgricultureFields.AddRange(fractional, cropConsumer, pasture, pendingPasture, zeroPasture,
				orchard, coordinatorCrop);
			context.SaveChanges();
			var forageProfile = new Db.ForagableProfile
			{
				Id = 930001,
				Name = "Land harness forage profile", RevisionNumber = 1,
				EditableItem = new Db.EditableItem
				{
					RevisionNumber = 1, RevisionStatus = (int)RevisionStatus.Current,
					BuilderAccountId = 1, BuilderDate = DateTime.UtcNow
				}
			};
			forageProfile.ForagableProfilesMaximumYields.Add(new Db.ForagableProfilesMaximumYields
			{
				ForageType = "herbs", Yield = 100.0
			});
			forageProfile.ForagableProfilesHourlyYieldGains.Add(new Db.ForagableProfilesHourlyYieldGains
			{
				ForageType = "herbs", Yield = 10.0
			});
			context.ForagableProfiles.Add(forageProfile);
			var overlayPackage = new Db.CellOverlayPackage
			{
				Id = 930001,
				Name = "Land harness overlay package", RevisionNumber = 1,
				EditableItem = new Db.EditableItem
				{
					RevisionNumber = 1, RevisionStatus = (int)RevisionStatus.Current,
					BuilderAccountId = 1, BuilderDate = DateTime.UtcNow
				}
			};
			context.CellOverlayPackages.Add(overlayPackage);
			var terrain = new Db.Terrain
			{
				Id = 930001,
				Name = "Land harness coordinator terrain", TerrainBehaviourMode = "outdoors",
				MovementRate = 1.0
			};
			context.Terrains.Add(terrain);
			context.SaveChanges();
			terrain.ForagableProfileId = forageProfile.Id;
			var coordinatorCell = context.Cells.Single(x => x.Id == coordinatorCropBase.CellId);
			coordinatorCell.ForagableProfileId = forageProfile.Id;
			var overlay = new Db.CellOverlay
			{
				Id = 930001,
				Name = "Land harness coordinator overlay",
				CellName = "Land harness coordinator cell",
				CellDescription = "Disposable native organic acceptance cell.",
				CellId = coordinatorCell.Id,
				CellOverlayPackageId = overlayPackage.Id,
				CellOverlayPackageRevisionNumber = overlayPackage.RevisionNumber,
				TerrainId = terrain.Id, AmbientLightFactor = 1.0, SafeQuit = true
			};
			context.CellOverlays.Add(overlay);
			context.SaveChanges();
			coordinatorCell.CurrentOverlayId = overlay.Id;
			context.SaveChanges();
			return new NativeOrganicFixtureIds(fractionalBase.CellId, fractional.Id, cropConsumer.Id, pasture.Id,
				fractionalBase.ResourceId, pendingPasture.Id, zeroPasture.Id, orchard.Id,
				constructorHalfBase.CellId, constructorZeroBase.CellId, coordinatorCrop.Id);
		}

		private static Db.AgricultureField CropField(long cellId, long profileId, long cropId, int yield,
			AgricultureCropStage stage, int nutrients)
		{
			var field = BaseField(cellId, profileId, AgricultureFieldUse.Crop, 50, nutrients);
			field.AgricultureFieldCrop = new Db.AgricultureFieldCrop
			{
				CropDefinitionId = cropId,
				Stage = (int)stage,
				GrowthDays = stage == AgricultureCropStage.Harvestable ? 30 : 10,
				Health = 50,
				YieldPotential = yield,
				Definition = "<Crop harvestCount=\"0\" />"
			};
			return field;
		}

		private static Db.AgricultureField BaseField(long cellId, long profileId, AgricultureFieldUse use,
			int pasture, int nutrients)
		{
			return new Db.AgricultureField
			{
				CellId = cellId,
				ProfileId = profileId,
				CurrentUse = (int)use,
				Moisture = 50,
				Drainage = 50,
				Nutrients = nutrients,
				Salinity = 0,
				Topsoil = 50,
				Tilth = 50,
				Rockiness = 0,
				Weeds = 0,
				Pests = 0,
				Fence = 50,
				Pasture = pasture,
				Condition = 50,
				Definition = "<Field />"
			};
		}
	}

	private sealed class NativeOrganicRuntime
	{
		private NativeOrganicRuntime(IFuturemud world, SaveManager saveManager, Mock<ICell> cell,
			AgricultureField field, IAgricultureCropDefinition crop)
		{
			World = world;
			SaveManager = saveManager;
			CellMock = cell;
			Field = field;
			Crop = crop;
		}

		public IFuturemud World { get; }
		public SaveManager SaveManager { get; }
		public Mock<ICell> CellMock { get; }
		public ICell Cell => CellMock.Object;
		public AgricultureField Field { get; }
		public IAgricultureCropDefinition Crop { get; }

		public static NativeOrganicRuntime Load(string connectionString, long fieldId, double recoveryFactor = 1.0,
			double initialFactor = 1.0, bool perennial = false)
		{
			using var context = NewIndependentContext(connectionString);
			var model = context.AgricultureFields
				.Include(x => x.AgricultureFieldCrop)
				.Include(x => x.AgricultureFieldWoodland)
				.Include(x => x.AgricultureFieldHerds)
				.AsNoTracking()
				.Single(x => x.Id == fieldId);
			var cropModels = context.AgricultureCropDefinitions.AsNoTracking().ToList();
			var herdModels = context.AgricultureHerdDefinitions.AsNoTracking().ToList();
			var profileModel = context.AgricultureFieldProfiles.AsNoTracking().Single(x => x.Id == model.ProfileId);

			var world = new Mock<IFuturemud>(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
			var saves = new SaveManager();
			world.SetupGet(x => x.SaveManager).Returns(saves);
			var cell = new Mock<ICell>(MockBehavior.Loose);
			cell.SetupGet(x => x.Id).Returns(model.CellId);
			cell.SetupGet(x => x.Name).Returns($"Land harness cell {model.CellId}");
			cell.SetupGet(x => x.FrameworkItemType).Returns("Cell");
			cell.SetupGet(x => x.Gameworld).Returns(world.Object);
			cell.Setup(x => x.CurrentTemperature(It.IsAny<IPerceiver>())).Returns(20.0);
			cell.Setup(x => x.CurrentWeather(It.IsAny<IPerceiver>())).Returns(default(IWeatherEvent)!);
			var cells = new All<ICell>();
			cells.Add(cell.Object);
			world.SetupGet(x => x.Cells).Returns(cells);

			var profile = new Mock<IAgricultureFieldProfile>();
			profile.SetupGet(x => x.Id).Returns(profileModel.Id);
			profile.SetupGet(x => x.Name).Returns(profileModel.Name);
			profile.SetupGet(x => x.FrameworkItemType).Returns("AgricultureFieldProfile");
			profile.SetupGet(x => x.Gameworld).Returns(world.Object);
			profile.SetupGet(x => x.DefaultScores).Returns(new Dictionary<AgricultureScoreType, int>());
			profile.Setup(x => x.AllowsUse(It.IsAny<AgricultureFieldUse>())).Returns(true);
			var profiles = new All<IAgricultureFieldProfile>();
			profiles.Add(profile.Object);
			world.SetupGet(x => x.AgricultureFieldProfiles).Returns(profiles);

			var crops = new All<IAgricultureCropDefinition>();
			foreach (var cropModel in cropModels)
			{
				var crop = new Mock<IAgricultureCropDefinition>();
				crop.SetupGet(x => x.Id).Returns(cropModel.Id);
				crop.SetupGet(x => x.Name).Returns(cropModel.Name);
				crop.SetupGet(x => x.FrameworkItemType).Returns("AgricultureCropDefinition");
				crop.SetupGet(x => x.Gameworld).Returns(world.Object);
				crop.SetupGet(x => x.BaseGrowthDays).Returns(30);
				crop.SetupGet(x => x.HarvestWindowDays).Returns(5);
				crop.SetupGet(x => x.MinimumMoisture).Returns(0);
				crop.SetupGet(x => x.MaximumMoisture).Returns(100);
				crop.SetupGet(x => x.MinimumTemperature).Returns(0);
				crop.SetupGet(x => x.MaximumTemperature).Returns(40);
				crop.SetupGet(x => x.HarvestCycleDays).Returns(30);
				crop.SetupGet(x => x.IsPerennial).Returns(perennial);
				crop.SetupGet(x => x.PlantingWindows).Returns(Array.Empty<AgriculturePlantingWindow>());
				crop.SetupGet(x => x.ScoreRanges).Returns(Array.Empty<AgricultureScoreRange>());
				crop.SetupGet(x => x.YieldOutputs).Returns(Array.Empty<AgricultureCommodityYield>());
				crop.SetupGet(x => x.SeedRequirements).Returns(Array.Empty<AgricultureCommodityYield>());
				crops.Add(crop.Object);
			}
			world.SetupGet(x => x.AgricultureCropDefinitions).Returns(crops);

			var herds = new All<IAgricultureHerdDefinition>();
			foreach (var herdModel in herdModels)
			{
				var herd = new Mock<IAgricultureHerdDefinition>();
				herd.SetupGet(x => x.Id).Returns(herdModel.Id);
				herd.SetupGet(x => x.Name).Returns(herdModel.Name);
				herd.SetupGet(x => x.FrameworkItemType).Returns("AgricultureHerdDefinition");
				herd.SetupGet(x => x.Gameworld).Returns(world.Object);
				herd.SetupGet(x => x.AnimalUnits).Returns(1.0);
				herd.SetupGet(x => x.DailyGraze).Returns(2.0);
				herd.SetupGet(x => x.MaximumCondition).Returns(100);
				herd.SetupGet(x => x.SecondaryOutputs).Returns(Array.Empty<AgricultureCommodityYield>());
				herds.Add(herd.Object);
			}
			world.SetupGet(x => x.AgricultureHerdDefinitions).Returns(herds);
			world.SetupGet(x => x.AgricultureWoodlandDefinitions).Returns(new All<IAgricultureWoodlandDefinition>());
			world.SetupGet(x => x.Properties).Returns(new All<IProperty>());

			var environment = new Mock<IEnvironmentalMagicService>();
			environment.Setup(x => x.EvaluateOrganicPenalty(It.IsAny<ICell>(),
				It.IsAny<NativeOrganicPenaltyChannel>(), It.IsAny<NativeOrganicPenaltyContext>()))
				.Returns((ICell _, NativeOrganicPenaltyChannel channel, NativeOrganicPenaltyContext _) =>
					channel is NativeOrganicPenaltyChannel.CropHealthRecovery or
						NativeOrganicPenaltyChannel.CropYieldRecovery or
						NativeOrganicPenaltyChannel.WoodlandHealthRecovery or
						NativeOrganicPenaltyChannel.WoodlandYieldRecovery or
						NativeOrganicPenaltyChannel.PastureRecovery
						? new NativeOrganicPenaltyEvaluation(true, true, recoveryFactor, null)
						: channel == NativeOrganicPenaltyChannel.PastureInitialisation
							? new NativeOrganicPenaltyEvaluation(true, true, initialFactor, null)
							: NativeOrganicPenaltyEvaluation.Neutral);
			world.SetupGet(x => x.EnvironmentalMagic).Returns(environment.Object);

			var field = new AgricultureField(model, world.Object);
			cell.SetupGet(x => x.AgricultureField).Returns(field);
			var fields = new All<IAgricultureField>();
			fields.Add(field);
			world.SetupGet(x => x.AgricultureFields).Returns(fields);
			var primaryCrop = crops.Get(model.AgricultureFieldCrop?.CropDefinitionId ?? cropModels[0].Id)!;
			return new NativeOrganicRuntime(world.Object, saves, cell, field, primaryCrop);
		}
	}

	private sealed record NativeOrganicObservation(AgricultureFieldUse CurrentUse, bool HasCrop, int Stock,
		long Generation, long Revision, decimal Prepaid, decimal HealthRemainder, decimal YieldRemainder,
		decimal BiomassRemainder, int GrowthDays)
	{
		public static NativeOrganicObservation Read(string connectionString, long fieldId,
			NativeOrganicSourceKind kind)
		{
			using var context = NewIndependentContext(connectionString);
			var field = context.AgricultureFields
				.Include(x => x.AgricultureFieldCrop)
				.Include(x => x.AgricultureFieldWoodland)
				.AsNoTracking()
				.Single(x => x.Id == fieldId);
			var root = XElement.Parse(field.Definition);
			var source = root.Element("NativeOrganicAccounting")?.Elements("Source")
				.SingleOrDefault(x => string.Equals((string?)x.Attribute("kind"), kind.ToString(),
					StringComparison.OrdinalIgnoreCase))
				?? throw new InvalidOperationException($"Field #{fieldId} has no saved {kind} accounting source.");
			var stock = kind switch
			{
				NativeOrganicSourceKind.Crop => field.AgricultureFieldCrop?.YieldPotential ?? 0,
				NativeOrganicSourceKind.Woodland => field.AgricultureFieldWoodland?.YieldPotential ?? 0,
				NativeOrganicSourceKind.Pasture => field.Pasture,
				_ => 0
			};
			return new NativeOrganicObservation((AgricultureFieldUse)field.CurrentUse,
				field.AgricultureFieldCrop is not null, stock,
				long.Parse(source.Attribute("generation")!.Value, CultureInfo.InvariantCulture),
				long.Parse(source.Attribute("revision")!.Value, CultureInfo.InvariantCulture),
				decimal.Parse(source.Attribute("prepaid")!.Value, CultureInfo.InvariantCulture),
				decimal.Parse(source.Attribute("healthRemainder")!.Value, CultureInfo.InvariantCulture),
				decimal.Parse(source.Attribute("yieldRemainder")!.Value, CultureInfo.InvariantCulture),
				decimal.Parse(source.Attribute("biomassRemainder")!.Value, CultureInfo.InvariantCulture),
				field.AgricultureFieldCrop?.GrowthDays ?? field.AgricultureFieldWoodland?.GrowthDays ?? 0);
		}
	}
}
