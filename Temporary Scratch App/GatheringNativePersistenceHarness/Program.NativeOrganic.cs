#nullable enable

using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Commands.Modules;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Economy.Property;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.Health.Wounds;
using MudSharp.Magic;
using MudSharp.Magic.Environment;
using MudSharp.Magic.Generators;
using MudSharp.Magic.Gathering;
using MudSharp.Magic.Capabilities;
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
		RunLandActionPersistenceProbe(database.Name, database.ConnectionString, fixture.CoordinatorActorFixture,
			fixture.CoordinatorCropFieldId, coordinatorProfileId, fixture.EnvironmentalResourceId);
		RunLandReceiptPersistenceProbe(database.ConnectionString, fixture.FractionalCellId,
			fixture.EnvironmentalResourceId, coordinatorProfileId);
		Console.WriteLine("landHarnessSubstitutions=world catalogues and deterministic weather; C-P03 uses a persisted overlay and forage profile with a real Cell/coordinator/agriculture owner and a controllable apiary candidate; other land cases use an ecological scalar fixture; SaveManager and independent EF/MySQL observations are production paths");
		total.Stop();
		Console.WriteLine($"landHarnessElapsedMs={total.ElapsedMilliseconds}");
		return 0;
	}

	private static void RunLandReceiptPersistenceProbe(string connectionString, long cellId,
		long destinationId, long profileId)
	{
		Guid id = Guid.NewGuid();
		Guid childId = Guid.NewGuid();
		var receipt = new MagicGatheringReceipt(id, 900001, 900001, 900001, 900001,
			Guid.NewGuid(), 1, MagicGatheringMethodKind.Land, cellId, profileId, 1,
			null, destinationId, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0,
			false, false, false, false, false, "Committing", DateTime.UtcNow)
		{
			ParticipantKeys = ["forage:herbs", "field:17"],
			EcologicalChildId = childId,
			LandDetailJson = "{\"Version\":1,\"Stage\":\"Prepared\"}"
		};
		var store = new MagicGatheringReceiptStore();
		Require(store.TryCreate(receipt), "L-P06 could not create the durable Land marker.");
		using (var independent = NewIndependentContext(connectionString))
		{
			Db.MagicGatheringOperation row = independent.MagicGatheringOperations.AsNoTracking()
				.Single(x => x.Id == id);
			Require(row.EcologicalChildId == childId && row.LandDetailJson == receipt.LandDetailJson,
				"L-P06 separate context lost the linked child or versioned detail.");
			Require(independent.MagicGatheringParticipants.AsNoTracking().Count(x => x.OperationId == id) == 2,
				"L-P06 separate context lost indexed participant rows.");
		}
		Require(store.HasUnresolvedForParticipant(cellId, "forage:herbs") &&
		        !store.HasUnresolvedForParticipant(cellId, "forage:berries"),
			"L-P06 did not isolate overlapping from unrelated native source keys.");
		MagicGatheringReceipt reconstructed = store.Operation(id)!;
		Require(reconstructed.Kind == MagicGatheringMethodKind.Land &&
		        reconstructed.EcologicalChildId == childId,
			"L-P06 did not reload the Land receipt and linked child identity.");
		store.Record(reconstructed with { Status = "Acknowledged", UpdatedUtc = DateTime.UtcNow });
		Require(!store.HasUnresolvedForParticipant(cellId, "forage:herbs"),
			"L-P06 staff acknowledgement did not release the receipt-only participant block.");
		Console.WriteLine($"L-P06-receipt=passed operation:{id} child:{childId} cell:{cellId} participants:2 isolatedAndAcknowledged:True");
	}

	private static void RunLandActionPersistenceProbe(string databaseName, string connectionString, FixtureIds actorFixture,
		long fieldId, long profileId, long environmentalResourceId)
	{
		using (var authoring = NewIndependentContext(connectionString))
		{
			Db.MagicCapability authoredCapability = authoring.MagicCapabilities.Single(x => x.Id == actorFixture.CapabilityId);
			XElement definition = XElement.Parse(authoredCapability.Definition);
			XElement method = definition.Element("Gathering")!.Element("Method")!;
			method.SetAttributeValue("kind", MagicGatheringMethodKind.Land);
			method.SetAttributeValue("min", 1.0);
			method.SetAttributeValue("max", 1.0);
			method.SetAttributeValue("damage", 0.0);
			method.SetAttributeValue("pain", 0.0);
			method.SetAttributeValue("stun", 0.0);
			method.Add(new XElement("Land", new XAttribute("damage", 1.0),
				new XElement("Source", new XAttribute("key", Guid.NewGuid()),
					new XAttribute("selector", "crop"), new XAttribute("ratio", 0.25)),
				new XElement("Source", new XAttribute("key", Guid.NewGuid()),
					new XAttribute("selector", "forage:herbs"), new XAttribute("ratio", 0.5),
					new XAttribute("collateral", true))));
			authoredCapability.Definition = definition.ToString();
			authoring.SaveChanges();
		}
		NativeRuntime actorRuntime = NativeRuntime.Load(actorFixture, connectionString);
		Mock<IFuturemud> world = actorRuntime.WorldMock;
		using var read = NewIndependentContext(connectionString);
		Db.AgricultureField fieldModel = read.AgricultureFields.Include(x => x.AgricultureFieldCrop)
			.AsNoTracking().Single(x => x.Id == fieldId);
		Db.Cell cellModel = read.Cells.Include(x => x.CellOverlays).Include(x => x.CellsForagableYields)
			.Include(x => x.CellsMagicResources).Include(x => x.EnvironmentalState)
			.AsNoTracking().Single(x => x.Id == fieldModel.CellId);
		Db.ForagableProfile forageModel = read.ForagableProfiles.Include(x => x.EditableItem)
			.Include(x => x.ForagableProfilesMaximumYields)
			.Include(x => x.ForagableProfilesHourlyYieldGains).AsNoTracking()
			.Single(x => x.Id == cellModel.ForagableProfileId);
		Db.MagicGenerator generatorModel = read.MagicGenerators.AsNoTracking().Single(x => x.Id == profileId);
		Db.MagicResource environmentalModel = read.MagicResources.AsNoTracking()
			.Single(x => x.Id == environmentalResourceId);
		var resources = new All<IMagicResource>();
		resources.Add(actorRuntime.Resource);
		resources.Add(new CappedSimpleMagicResource(environmentalModel, world.Object));
		world.SetupGet(x => x.MagicResources).Returns(resources);
		var generators = new All<IMagicResourceRegenerator>();
		var environmentalProfile = new EnvironmentalMagicGenerator(generatorModel, world.Object);
		generators.Add(environmentalProfile);
		world.SetupGet(x => x.MagicResourceRegenerators).Returns(generators);
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
		SetPrivateMember(actorRuntime.Actor, "Location", cell);
		var fieldProfileModel = read.AgricultureFieldProfiles.AsNoTracking()
			.Single(x => x.Id == fieldModel.ProfileId);
		var fieldProfile = new Mock<IAgricultureFieldProfile>();
		fieldProfile.SetupGet(x => x.Id).Returns(fieldProfileModel.Id);
		fieldProfile.SetupGet(x => x.Name).Returns(fieldProfileModel.Name);
		fieldProfile.SetupGet(x => x.Gameworld).Returns(world.Object);
		fieldProfile.SetupGet(x => x.DefaultScores).Returns(new Dictionary<AgricultureScoreType, int>());
		fieldProfile.Setup(x => x.AllowsUse(It.IsAny<AgricultureFieldUse>())).Returns(true);
		var fieldProfiles = new All<IAgricultureFieldProfile>();
		fieldProfiles.Add(fieldProfile.Object);
		world.SetupGet(x => x.AgricultureFieldProfiles).Returns(fieldProfiles);
		Db.AgricultureCropDefinition cropModel = read.AgricultureCropDefinitions.AsNoTracking()
			.Single(x => x.Id == fieldModel.AgricultureFieldCrop!.CropDefinitionId);
		var crop = new Mock<IAgricultureCropDefinition>();
		crop.SetupGet(x => x.Id).Returns(cropModel.Id);
		crop.SetupGet(x => x.Name).Returns(cropModel.Name);
		crop.SetupGet(x => x.Gameworld).Returns(world.Object);
		crop.SetupGet(x => x.ScoreRanges).Returns(Array.Empty<AgricultureScoreRange>());
		crop.SetupGet(x => x.YieldOutputs).Returns(Array.Empty<AgricultureCommodityYield>());
		crop.SetupGet(x => x.SeedRequirements).Returns(Array.Empty<AgricultureCommodityYield>());
		var crops = new All<IAgricultureCropDefinition>();
		crops.Add(crop.Object);
		world.SetupGet(x => x.AgricultureCropDefinitions).Returns(crops);
		world.SetupGet(x => x.AgricultureWoodlandDefinitions).Returns(new All<IAgricultureWoodlandDefinition>());
		world.SetupGet(x => x.AgricultureHerdDefinitions).Returns(new All<IAgricultureHerdDefinition>());
		world.SetupGet(x => x.Properties).Returns(new All<IProperty>());
		var fields = new All<IAgricultureField>();
		world.SetupGet(x => x.AgricultureFields).Returns(fields);
		world.SetupGet(x => x.HeartbeatManager).Returns(new HeartbeatManager(world.Object));
		using var coordinator = new EnvironmentalMagicCoordinator(world.Object);
		world.SetupGet(x => x.EnvironmentalMagic).Returns(coordinator);
		var field = new AgricultureField(fieldModel, world.Object);
		fields.Add(field);
		coordinator.Initialise();
		var clock = new HarnessClock();
		var service = new MagicGatheringService(world.Object, clock: clock);
		MagicGatheringResult started = service.Begin(actorRuntime.Actor, actorRuntime.Capability, "draw", 1.0);
		Require(started.Success && started.OperationId.HasValue,
			$"L-P01 native Land action did not begin: {started.Message} {string.Join(", ", actorRuntime.Capability.GatheringConfigurationErrors())}");
		Guid operationId = started.OperationId ?? throw new InvalidOperationException("Land start did not issue a token.");
		double forageBefore = cell.GetForagableYield("herbs");
		var cropBefore = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		clock.Advance(TimeSpan.FromSeconds(1));
		MagicGatheringResult completed = service.Complete(actorRuntime.Actor, operationId);
		Require(completed.Success,
			$"L-P01 native Land action did not complete: {completed.Message}; receipt {new MagicGatheringReceiptStore().Operation(operationId)?.Diagnostic}");
		using var independent = NewIndependentContext(connectionString);
		Db.MagicGatheringOperation parent = independent.MagicGatheringOperations.AsNoTracking()
			.Single(x => x.Id == operationId);
		Db.EnvironmentalMagicOperation child = independent.EnvironmentalMagicOperations.AsNoTracking()
			.Single(x => x.Id == parent.EcologicalChildId);
		double credited = independent.CharactersMagicResources.AsNoTracking()
			.Single(x => x.CharacterId == actorFixture.CharacterId &&
				x.MagicResourceId == actorFixture.ResourceId).Amount;
		var cropAfter = NativeOrganicObservation.Read(connectionString, fieldId, NativeOrganicSourceKind.Crop);
		double forageAfter = ReadForageYield(connectionString, cell.Id);
		Require(parent.Status == "Completed" && parent.AccountingPersisted &&
		        parent.EcologicalApplied && child.Id == parent.EcologicalChildId &&
		        Same(credited, 1.0) && cropAfter.Prepaid == cropBefore.PrepaidFraction - 0.25m &&
		        Same(forageBefore - forageAfter, 0.5),
			"L-P01 separate context did not observe complete crop/forage debit, ecology and personal credit.");
		Console.WriteLine($"L-P01-native=passed operation:{operationId} cell:{cell.Id} field:{fieldId} cropPrepaid:{cropBefore.PrepaidFraction}->{cropAfter.Prepaid} forage:{forageBefore:F2}->{forageAfter:F2} child:{child.Id} credit:{credited:F2}");
		var capability = (SkillLevelBasedMagicCapability)actorRuntime.Capability;
		foreach (string command in new[]
		         {
			"gather land draw source 1 remove",
			"gather land draw source 1 remove",
			"gather land draw source add forage herbs 1",
			"gather land draw source add forage berries 1"
		         })
		{
			Require(capability.BuildingCommand(actorRuntime.Actor, new StringStack(command)),
				$"L-P01 could not author the two-forage method: {command}");
		}
		actorRuntime.World.SaveManager.Flush();
		Require(cell.TryConsumeYield("herbs", forageAfter - 0.25),
			"L-P01 could not set the first forage key's limited stock through ordinary consumption.");
		actorRuntime.World.SaveManager.Flush();
		Require(Same(ReadForageYield(connectionString, cell.Id), 0.25),
			"L-P01 ordinary forage consumption was not saved before the second action.");
		MagicGatheringResult secondStart = service.Begin(actorRuntime.Actor, actorRuntime.Capability, "draw", 1.0);
		Require(secondStart.Success && secondStart.OperationId.HasValue,
			$"L-P01 two-forage action did not begin: {secondStart.Message}");
		Guid secondOperationId = secondStart.OperationId ?? throw new InvalidOperationException("Two-forage start did not issue a token.");
		Require(Same(secondStart.Quote!.LandSources.Single(x => x.Selector == "forage:herbs").FundingUnits, 0.25) &&
		        Same(secondStart.Quote.LandSources.Single(x => x.Selector == "forage:berries").FundingUnits, 0.75),
			"L-P01 did not allocate both forage keys in one cell at the captured ratio.");
		clock.Advance(TimeSpan.FromSeconds(1));
		MagicGatheringResult secondComplete = service.Complete(actorRuntime.Actor, secondOperationId);
		Require(secondComplete.Success,
			$"L-P01 two-forage action did not complete: {secondComplete.Message}; receipt {new MagicGatheringReceiptStore().Operation(secondOperationId)?.Diagnostic}");
		double observedHerbs = ReadForageYield(connectionString, cell.Id);
		double observedBerries = ReadForageYield(connectionString, cell.Id, "berries");
		using var finalRead = NewIndependentContext(connectionString);
		double finalCredit = finalRead.CharactersMagicResources.AsNoTracking()
			.Single(x => x.CharacterId == actorFixture.CharacterId &&
				x.MagicResourceId == actorFixture.ResourceId).Amount;
		Require(Same(observedHerbs, 0.0) && Same(observedBerries, 99.25) &&
		        Same(finalCredit, 2.0) &&
		        finalRead.MagicGatheringOperations.AsNoTracking().Count(x => x.Status == "Completed" &&
				x.ActorId == actorFixture.CharacterId) == 2,
			"L-P01 independent reader did not observe both forage debits, one additional credit and two completed parents.");
		Console.WriteLine($"L-P01-two-forage=passed operation:{secondStart.OperationId} cell:{cell.Id} herbs:0.25->{observedHerbs:F2} berries:100.00->{observedBerries:F2} totalCredit:{finalCredit:F2}");
		foreach (string command in new[]
		         {
			"gather land draw source 1 remove",
			"gather land draw source 1 remove",
			$"gather land draw source add ambient {environmentalResourceId} 1",
			"gather land draw source add crop 0.25"
		         })
		{
			Require(capability.BuildingCommand(actorRuntime.Actor, new StringStack(command)),
				$"L-P01 could not author ambient plus crop funding: {command}");
		}
		actorRuntime.World.SaveManager.Flush();
		IMagicResource ambient = resources.Get(environmentalResourceId)!;
		Require(coordinator.TryInspectLandResource(cell, ambient, out var ambientBefore) && ambientBefore.IsValid,
			"L-P01 ambient funding output is not valid on the physical cell.");
		Require(coordinator.TryMutateResource(cell, ambient, EnvironmentalResourceMutation.Set,
			0.5, out bool staged) && staged,
			"L-P01 could not stage the exact managed ambient balance.");
		actorRuntime.World.SaveManager.Flush();
		Require(coordinator.TryInspectLandResource(cell, ambient, out var fundedAmbient) &&
		        Same(fundedAmbient.Balance, 0.5),
			$"L-P01 did not stage exactly half an ambient unit through the managed output (before {ambientBefore.Balance}, after {fundedAmbient.Balance}, maximum {fundedAmbient.Maximum}, diagnostic {fundedAmbient.Error}).");
		MagicGatheringResult thirdStart = service.Begin(actorRuntime.Actor, actorRuntime.Capability, "draw", 1.0);
		Require(thirdStart.Success && thirdStart.OperationId.HasValue,
			$"L-P01 ambient-plus-crop action did not begin: {thirdStart.Message}");
		Guid thirdOperationId = thirdStart.OperationId ?? throw new InvalidOperationException("Ambient-crop start did not issue a token.");
		Require(Same(thirdStart.Quote!.LandSources.Single(x => x.Selector == $"ambient:{environmentalResourceId}").FundingUnits, 0.5) &&
		        Same(thirdStart.Quote.LandSources.Single(x => x.Selector == "crop").FundingUnits, 0.125),
			"L-P01 ambient and crop did not share the requested credit at their configured ratios.");
		clock.Advance(TimeSpan.FromSeconds(1));
		MagicGatheringResult thirdComplete = service.Complete(actorRuntime.Actor, thirdOperationId);
		Require(thirdComplete.Success,
			$"L-P01 ambient-plus-crop action did not complete: {thirdComplete.Message}; receipt {new MagicGatheringReceiptStore().Operation(thirdOperationId)?.Diagnostic}");
		var cropThird = NativeOrganicObservation.Read(connectionString, fieldId, NativeOrganicSourceKind.Crop);
		using var thirdRead = NewIndependentContext(connectionString);
		Db.CellMagicResource savedAmbient = thirdRead.CellsMagicResources.AsNoTracking()
			.Single(x => x.CellId == cell.Id && x.MagicResourceId == environmentalResourceId);
		double thirdCredit = thirdRead.CharactersMagicResources.AsNoTracking()
			.Single(x => x.CharacterId == actorFixture.CharacterId && x.MagicResourceId == actorFixture.ResourceId).Amount;
		Db.MagicGatheringOperation thirdParent = thirdRead.MagicGatheringOperations.AsNoTracking()
			.Single(x => x.Id == thirdOperationId);
		using JsonDocument thirdDetail = JsonDocument.Parse(thirdParent.LandDetailJson!);
		JsonElement ambientPayment = thirdDetail.RootElement.GetProperty("AppliedSources")
			.EnumerateArray().Single(x => x.GetProperty("Selector").GetString() ==
				$"ambient:{environmentalResourceId}");
		Require(savedAmbient.Amount >= 0.0 && savedAmbient.Amount < 0.1 &&
		        Same(ambientPayment.GetProperty("FundingUnits").GetDouble(), 0.5) &&
		        cropThird.Prepaid == 0.125m &&
		        Same(thirdCredit, 3.0),
			$"L-P01 independent reader did not observe an exact half-unit ambient debit, fractional crop payment and exact credit (ambient residual {savedAmbient.Amount}, crop prepaid {cropThird.Prepaid}, credit {thirdCredit}).");
		Console.WriteLine($"L-P01-ambient-crop=passed operation:{thirdStart.OperationId} cell:{cell.Id} ambient:0.50->{savedAmbient.Amount:F2} cropPrepaid:0.25->{cropThird.Prepaid} totalCredit:{thirdCredit:F2}");
		foreach (string command in new[]
		         {
			"gather land draw source 1 remove",
			"gather land draw source 1 remove",
			"gather land draw source add forage berries 1",
			"gather set draw damage 3",
			"gather set draw pain 4",
			"gather set draw stun 5",
			"gather set draw healthseverity horrifying"
		         })
		{
			Require(capability.BuildingCommand(actorRuntime.Actor, new StringStack(command)),
				$"L-P02 could not author native body-priced Land method: {command}");
		}
		Require(capability.GatheringMethods.Single().OnGatheredProgId == 0,
			"L-P02 body-priced method must have no notification callback.");
		actorRuntime.World.SaveManager.Flush();
		long? firstWound = null;
		Guid lastOperationId = Guid.Empty;
		for (int repetition = 1; repetition <= 2; repetition++)
		{
			MagicGatheringResult healthStart = service.Begin(actorRuntime.Actor, actorRuntime.Capability,
				"draw", 1.0);
			Require(healthStart.Success && healthStart.OperationId.HasValue,
				$"L-P02 native body-priced action {repetition} did not begin: {healthStart.Message}");
			lastOperationId = healthStart.OperationId ?? throw new InvalidOperationException("Health-price start did not issue a token.");
			clock.Advance(TimeSpan.FromSeconds(1));
			MagicGatheringResult healthComplete = service.Complete(actorRuntime.Actor, lastOperationId);
			Require(healthComplete.Success,
				$"L-P02 native body-priced action {repetition} did not complete: {healthComplete.Message}; receipt {new MagicGatheringReceiptStore().Operation(lastOperationId)?.Diagnostic}");
			using var woundRead = NewIndependentContext(connectionString);
			Db.Wound[] wounds = woundRead.Wounds.AsNoTracking()
				.Where(x => x.BodyId == actorFixture.BodyId).ToArray();
			Require(wounds.Length == 1,
				"L-P02 did not retain one existing native wound across consecutive Land prices.");
			firstWound ??= wounds[0].Id;
			double expectedDamage = repetition * 3.0;
			double expectedPain = repetition * 4.0;
			double expectedStun = repetition * 5.0;
			double currentCredit = woundRead.CharactersMagicResources.AsNoTracking()
				.Single(x => x.CharacterId == actorFixture.CharacterId &&
					x.MagicResourceId == actorFixture.ResourceId).Amount;
			Require(wounds[0].Id == firstWound && Same(wounds[0].CurrentDamage, expectedDamage) &&
			        Same(wounds[0].CurrentPain, expectedPain) &&
			        Same(wounds[0].CurrentStun, expectedStun) && Same(currentCredit, 3.0 + repetition),
				$"L-P02 independent reader missed native wound/credit payment {repetition}: wound {wounds[0].CurrentDamage}/{wounds[0].CurrentPain}/{wounds[0].CurrentStun}, credit {currentCredit}.");
			Console.WriteLine($"L-P02=passed operation:{healthStart.OperationId} repetition:{repetition} wound:{firstWound} damage:{wounds[0].CurrentDamage:F2} pain:{wounds[0].CurrentPain:F2} stun:{wounds[0].CurrentStun:F2} credit:{currentCredit:F2}");
		}
		RunLandActionReaderProcess(databaseName, actorFixture,
			fieldId, environmentalResourceId, lastOperationId);
		RunLandPlayerCommandAndRepairProbe(actorRuntime, capability, service, clock, coordinator, cell,
			connectionString, actorFixture);
		RunLandConsumerAndReplacementProbe(actorRuntime, capability, service, clock, coordinator, cell,
			field, fieldId, environmentalProfile, environmentalResourceId, connectionString, actorFixture);
		RunLandCheckpointFailureProbe(actorRuntime, capability, service, clock, coordinator, cell,
			field, fieldId, connectionString, actorFixture);
		RunLandPrepaidAtZeroStockProbe(actorRuntime, capability, service, clock, cell,
			field, fieldId, connectionString, actorFixture);
		RunLandForageCheckpointFailureProbe(actorRuntime, capability, service, clock, cell,
			connectionString, actorFixture);
		RunLandNativeGrowthDuringWaitProbe(actorRuntime, capability, service, clock, cell,
			field, fieldId, connectionString, actorFixture);
	}

	private static void RunLandNativeGrowthDuringWaitProbe(NativeRuntime runtime,
		SkillLevelBasedMagicCapability capability, MagicGatheringService service, HarnessClock clock,
		Cell cell, AgricultureField field, long fieldId, string connectionString, FixtureIds fixture)
	{
		foreach (string command in new[]
		         {
			"gather land draw source 1 remove",
			"gather land draw source add crop 0.125"
		         })
		{
			Require(capability.BuildingCommand(runtime.Actor, new StringStack(command)),
				$"L-T33 could not author the crop growth method: {command}");
		}
		var before = NativeOrganicObservation.Read(connectionString, fieldId, NativeOrganicSourceKind.Crop);
		MagicGatheringResult started = service.Begin(runtime.Actor, runtime.Capability, "draw", 1.0);
		Require(started.Success && started.OperationId.HasValue &&
			Same(started.Quote!.LandSources.Single(x => x.Selector == "crop").FundingUnits, 0.125),
			$"L-T33 native growth action did not capture its allocation: {started.Message}");
		Guid operationId = started.OperationId ??
			throw new InvalidOperationException("Native growth start did not issue a token.");
		field.DailyTick();
		runtime.World.SaveManager.Flush();
		var grown = NativeOrganicObservation.Read(connectionString, fieldId, NativeOrganicSourceKind.Crop);
		Require(grown.HasCrop && grown.Generation == before.Generation && grown.Revision > before.Revision &&
			(grown.Stock > before.Stock || grown.HealthRemainder > before.HealthRemainder ||
			 grown.YieldRemainder > before.YieldRemainder || grown.BiomassRemainder > before.BiomassRemainder),
			$"L-T33 ordinary crop tick did not grow stock or a recovery remainder: stock {before.Stock}->{grown.Stock}, yield remainder {before.YieldRemainder}->{grown.YieldRemainder}.");
		clock.Advance(TimeSpan.FromSeconds(1));
		MagicGatheringResult completed = service.Complete(runtime.Actor, operationId);
		Require(completed.Success, $"L-T33 changed native revision invalidated a still affordable logical allocation: {completed.Message}");
		var paid = NativeOrganicObservation.Read(connectionString, fieldId, NativeOrganicSourceKind.Crop);
		using var read = NewIndependentContext(connectionString);
		double credit = read.CharactersMagicResources.AsNoTracking()
			.Single(x => x.CharacterId == fixture.CharacterId && x.MagicResourceId == fixture.ResourceId).Amount;
		Require(paid.Generation == grown.Generation && paid.Prepaid == grown.Prepaid - 0.125m &&
			Same(service.LandDetails(operationId)!["paid:crop"], 0.125) && Same(credit, 9.0),
			"L-T33 fresh native plan changed the captured mix, used the stale token or credited twice.");
		Console.WriteLine($"L-T33=passed operation:{started.OperationId} stock:{before.Stock}->{grown.Stock}->{paid.Stock} yieldRemainder:{before.YieldRemainder}->{grown.YieldRemainder} prepaid:{grown.Prepaid}->{paid.Prepaid} credit:{credit:F2}");
	}

	private static void RunLandForageCheckpointFailureProbe(NativeRuntime runtime,
		SkillLevelBasedMagicCapability capability, MagicGatheringService service, HarnessClock clock,
		Cell cell, string connectionString, FixtureIds fixture)
	{
		foreach (string command in new[]
		         {
			"gather land draw source 1 remove",
			"gather land draw source add forage berries 1"
		         })
		{
			Require(capability.BuildingCommand(runtime.Actor, new StringStack(command)),
				$"L-T37 could not author the forage checkpoint method: {command}");
		}
		runtime.World.SaveManager.Flush();
		double before = ReadForageYield(connectionString, cell.Id, "berries");
		MagicGatheringResult started = service.Begin(runtime.Actor, runtime.Capability, "draw", 1.0);
		Require(started.Success && started.OperationId.HasValue,
			$"L-T37 forage checkpoint action did not begin: {started.Message}");
		Guid operationId = started.OperationId ??
			throw new InvalidOperationException("Forage checkpoint start did not issue a token.");
		using (var connection = new MySqlConnection(connectionString))
		{
			connection.Open();
			using var command = connection.CreateCommand();
			command.CommandText =
				"CREATE TRIGGER `land_gather_forage_checkpoint_fail` BEFORE UPDATE ON `Cells_ForagableYields` FOR EACH ROW SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'owned Land forage checkpoint failure';";
			command.ExecuteNonQuery();
		}
		MagicGatheringResult result;
		try
		{
			clock.Advance(TimeSpan.FromSeconds(1));
			result = service.Complete(runtime.Actor, operationId);
		}
		finally
		{
			using var connection = new MySqlConnection(connectionString);
			connection.Open();
			using var command = connection.CreateCommand();
			command.CommandText = "DROP TRIGGER IF EXISTS `land_gather_forage_checkpoint_fail`;";
			command.ExecuteNonQuery();
		}
		using var read = NewIndependentContext(connectionString);
		Db.MagicGatheringOperation parent = read.MagicGatheringOperations.AsNoTracking()
			.Single(x => x.Id == operationId);
		double credit = read.CharactersMagicResources.AsNoTracking()
			.Single(x => x.CharacterId == fixture.CharacterId && x.MagicResourceId == fixture.ResourceId).Amount;
		Require(!result.Success && parent.Status == "NeedsReview" && parent.EcologicalApplied &&
			!parent.DestinationCredited && cell.YieldsChanged &&
			runtime.World.SaveManager.IsQueued(cell) && Same(ReadForageYield(connectionString, cell.Id, "berries"), before) &&
			Same(credit, 8.0),
			$"L-T37 forage checkpoint did not preserve the retryable dirty facet: {result.Message}");
		runtime.World.SaveManager.Flush();
		double after = ReadForageYield(connectionString, cell.Id, "berries");
		Require(Same(after, before - 1.0) && !cell.YieldsChanged &&
			!service.Complete(runtime.Actor, operationId).Success &&
			read.EnvironmentalMagicOperations.AsNoTracking().Count(x => x.Id == parent.EcologicalChildId) == 1 &&
			Same(read.CharactersMagicResources.AsNoTracking()
				.Single(x => x.CharacterId == fixture.CharacterId && x.MagicResourceId == fixture.ResourceId).Amount, 8.0),
			"L-T37 forage retry lost its paid state or replayed the payout or ecological child.");
		Console.WriteLine($"L-T37-forage-checkpoint=passed operation:{operationId} forage:{before:F2}->{after:F2} dirtyRecovered:True credit:{credit:F2} replayRefused:True");
		Require(service.Acknowledge(operationId).Success,
			"L-T37 reviewed forage owner must be released before the independent native growth probe.");
	}

	private static void RunLandPrepaidAtZeroStockProbe(NativeRuntime runtime,
		SkillLevelBasedMagicCapability capability, MagicGatheringService service, HarnessClock clock,
		Cell cell, AgricultureField field, long fieldId, string connectionString, FixtureIds fixture)
	{
		Require(capability.BuildingCommand(runtime.Actor, new StringStack("gather land draw crophealth 0")),
			"L-T15 could not disable the optional vegetation health cost for the prepaid-only probe.");
		var before = NativeOrganicObservation.Read(connectionString, fieldId, NativeOrganicSourceKind.Crop);
		Require(before.HasCrop && before.Stock > 0 && before.Prepaid == 0.875m,
			"L-T15 requires a living crop with paid remainder after the checkpoint retry.");
		int currentYield = field.CropYieldPotential;
		Console.WriteLine($"L-T15-setup savedStock:{before.Stock} ownerStock:{currentYield} cropHealth:{field.CropHealth} prepaid:{before.Prepaid}");
		Require(currentYield > 0, "L-T15 current crop owner has no whole yield to exhaust.");
		var craftInput = NativeAgricultureInput(runtime.World, field.CurrentCrop.Id, currentYield);
		craftInput.ReserveInput(cell);
		runtime.World.SaveManager.Flush();
		var depleted = NativeOrganicObservation.Read(connectionString, fieldId, NativeOrganicSourceKind.Crop);
		Require(depleted.HasCrop && depleted.Stock == 0 && depleted.Prepaid == before.Prepaid,
			"L-T15 native craft depletion changed the crop lifecycle or existing prepaid fraction.");
		MagicGatheringResult started = service.Begin(runtime.Actor, runtime.Capability, "draw", 1.0);
		Require(started.Success && started.OperationId.HasValue &&
			started.Quote!.LandSources.Single(x => x.Selector == "crop").WholeNativeDebit == 0,
			$"L-T15 prepaid-only Land action did not begin with zero whole debit: {started.Message}");
		clock.Advance(TimeSpan.FromSeconds(1));
		Require(service.Complete(runtime.Actor, started.OperationId!.Value).Success,
			"L-T15 prepaid-only Land action did not complete.");
		var after = NativeOrganicObservation.Read(connectionString, fieldId, NativeOrganicSourceKind.Crop);
		using var read = NewIndependentContext(connectionString);
		double credit = read.CharactersMagicResources.AsNoTracking()
			.Single(x => x.CharacterId == fixture.CharacterId && x.MagicResourceId == fixture.ResourceId).Amount;
		Require(after.HasCrop && after.Stock == 0 && after.Prepaid == 0.75m && Same(credit, 8.0),
			"L-T15 prepaid-only Land completion consumed whole native stock or lost exact credit.");
		Console.WriteLine($"L-T15=passed operation:{started.OperationId} cropStock:{depleted.Stock}->{after.Stock} prepaid:{depleted.Prepaid}->{after.Prepaid} wholeDebit:0 credit:{credit:F2}");
	}

	private static void RunLandConsumerAndReplacementProbe(NativeRuntime runtime,
		SkillLevelBasedMagicCapability capability, MagicGatheringService service, HarnessClock clock,
		EnvironmentalMagicCoordinator coordinator, Cell cell, AgricultureField field, long fieldId,
		EnvironmentalMagicGenerator environmentalProfile, long environmentalResourceId,
		string connectionString, FixtureIds fixture)
	{
		foreach (string command in new[]
		         {
			"gather land draw source 1 remove",
			$"gather land draw collateral add ambient {environmentalResourceId} 0.5",
			"gather land draw source add crop 0.25",
			"gather land draw crophealth 1",
			"gather set draw damage 0",
			"gather set draw pain 0",
			"gather set draw stun 0"
		         })
		{
			Require(capability.BuildingCommand(runtime.Actor, new StringStack(command)),
				$"L-P04 could not author the fractional crop method: {command}");
		}
		int healthBefore = field.CropHealth;
		Require(environmentalProfile.BuildingCommand(runtime.Actor,
			new StringStack("input living agriculture crophealth 1")) &&
			environmentalProfile.BuildingCommand(runtime.Actor,
				new StringStack($"output {environmentalResourceId} maximum living")),
			"L-T16 could not bind ambient maximum to the current crop health.");
		IMagicResource ambient = runtime.World.MagicResources.Get(environmentalResourceId)!;
		Require(coordinator.TryMutateResource(cell, ambient, EnvironmentalResourceMutation.Set,
			healthBefore, out bool staged) && staged,
			"L-T16 could not fill the ambient balance to the living maximum.");
		runtime.World.SaveManager.Flush();
		MagicGatheringResult started = service.Begin(runtime.Actor, runtime.Capability, "draw", 1.0);
		Require(started.Success && started.OperationId.HasValue,
			$"L-P04 fractional Land action did not begin: {started.Message}");
		Guid operationId = started.OperationId ?? throw new InvalidOperationException("L-P04 start did not issue a token.");
		clock.Advance(TimeSpan.FromSeconds(1));
		Require(service.Complete(runtime.Actor, operationId).Success,
			"L-P04 fractional Land action did not complete.");
		var paid = NativeOrganicObservation.Read(connectionString, fieldId, NativeOrganicSourceKind.Crop);
		Require(coordinator.TryInspectLandResource(cell, ambient, out var afterCap) && afterCap.IsValid &&
			Same(afterCap.Maximum, healthBefore - 1) && Same(afterCap.Balance, healthBefore - 1) &&
			Same(service.LandDetails(operationId)!["paid:ambient:" + environmentalResourceId], 0.5),
			"L-T16 vegetation maximum did not fall after the paid ambient collateral debit.");
		Console.WriteLine($"L-T16=passed operation:{operationId} ambientMaximum:{healthBefore}->{afterCap.Maximum:F2} effectiveBalance:{afterCap.Balance:F2} paidCollateral:0.50 credit:7.00");
		Require(capability.BuildingCommand(runtime.Actor,
			new StringStack("gather land draw source 1 remove")),
			"L-P04 could not remove the cap probe collateral before its lifecycle checks.");
		using var healthRead = NewIndependentContext(connectionString);
		int healthAfter = healthRead.AgricultureFieldCrops.AsNoTracking()
			.Single(x => x.AgricultureFieldId == fieldId).Health;
		Require(paid.Prepaid == 0.875m && paid.HasCrop && healthAfter == healthBefore - 1 &&
			Same(service.LandDetails(operationId)!["healthLoss:crop"], 1.0) &&
			Same(runtime.Actor.MagicResourceAmounts[runtime.Resource], 7.0),
			$"L-P04 did not persist its newly paid crop remainder and personal credit ({paid.Prepaid}).");
		MagicGatheringResult staleStart = service.Begin(runtime.Actor, runtime.Capability, "draw", 1.0);
		Require(staleStart.Success && staleStart.OperationId.HasValue,
			$"L-T10 could not start a timed action against the surviving crop: {staleStart.Message}");
		Require(coordinator.TryPlanOrganicDebit(cell, "crop", 0.125, out var oldPlan, out string? planError),
			planError ?? "L-P04 could not capture the old crop lifecycle plan.");
		IAgricultureCropDefinition crop = field.CurrentCrop;
		var craftInput = NativeAgricultureInput(runtime.World, crop.Id, 5);
		craftInput.ReserveInput(cell);
		Require(!coordinator.TryApplyOrganicDebit(cell, oldPlan, out _, out _),
			"L-P04 applied a plan made stale by the real craft reservation.");
		SetPrivateMember(field, "CropStage", AgricultureCropStage.Harvestable);
		field.Changed = true;
		var harvest = NativeOperation(runtime.World, AgricultureOperationType.Harvest,
			AgricultureFieldUse.Crop, AgricultureFieldUse.Fallow);
		Require(field.ApplyOperation(harvest, null!, null!, false, out string harvestResult),
			harvestResult);
		runtime.World.SaveManager.Flush();
		var removed = NativeOrganicObservation.Read(connectionString, fieldId, NativeOrganicSourceKind.Crop);
		Require(!removed.HasCrop && removed.Prepaid == 0m,
			"L-P04 native annual harvest did not discard the Land-paid crop remainder.");
		var sow = NativeOperation(runtime.World, AgricultureOperationType.Sow,
			AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureTargetType.Crop);
		Require(field.ApplyOperation(sow, crop, null!, false, out string sowResult), sowResult);
		runtime.World.SaveManager.Flush();
		var replaced = NativeOrganicObservation.Read(connectionString, fieldId, NativeOrganicSourceKind.Crop);
		clock.Advance(TimeSpan.FromSeconds(1));
		bool staleCompletionRefused = !service.Complete(runtime.Actor, staleStart.OperationId!.Value).Success;
		Require(replaced.HasCrop && replaced.Generation > paid.Generation && replaced.Prepaid == 0m &&
			!coordinator.TryApplyOrganicDebit(cell, oldPlan, out _, out _) && staleCompletionRefused &&
			Same(runtime.Actor.MagicResourceAmounts[runtime.Resource], 7.0),
			"L-P04 replacement reused the previous crop generation or Land-paid fraction.");
		Console.WriteLine($"L-P04=passed operation:{operationId} field:{fieldId} generation:{paid.Generation}->{replaced.Generation} paidPrepaid:{paid.Prepaid}->harvest:{removed.Prepaid}->replant:{replaced.Prepaid} cropHealth:{healthBefore}->{healthAfter} nativeCraft:5 credit:7 oldPlanRefused:True timedReplacementRefused:{staleCompletionRefused}");
	}

	private static void RunLandCheckpointFailureProbe(NativeRuntime runtime,
		SkillLevelBasedMagicCapability capability, MagicGatheringService service, HarnessClock clock,
		EnvironmentalMagicCoordinator coordinator, Cell cell, AgricultureField field, long fieldId,
		string connectionString, FixtureIds fixture)
	{
		SetPrivateMember(field, "CropStage", AgricultureCropStage.Growing);
		field.Changed = true;
		runtime.World.SaveManager.Flush();
		foreach (string command in new[]
		         {
			"gather land draw source 1 remove",
			"gather land draw source add crop 0.125",
			"gather land draw damage 1",
			"gather set draw damage 0",
			"gather set draw pain 0",
			"gather set draw stun 0"
		         })
		{
			Require(capability.BuildingCommand(runtime.Actor, new StringStack(command)),
				$"L-P06 could not author checkpoint probe method: {command}");
		}
		runtime.World.SaveManager.Flush();
		var before = NativeOrganicObservation.Read(connectionString, fieldId, NativeOrganicSourceKind.Crop);
		Require(before.Prepaid == 0m && before.HasCrop, "L-P06 replacement crop fixture was not established before injection.");
		MagicGatheringResult started = service.Begin(runtime.Actor, runtime.Capability, "draw", 1.0);
		Require(started.Success && started.OperationId.HasValue,
			$"L-P06 checkpoint action did not begin: {started.Message}");
		Guid operationId = started.OperationId ?? throw new InvalidOperationException("Checkpoint start did not issue a token.");
		using (var connection = new MySqlConnection(connectionString))
		{
			connection.Open();
			using var command = connection.CreateCommand();
			command.CommandText =
				"CREATE TRIGGER `land_gather_checkpoint_fail` BEFORE UPDATE ON `AgricultureFields` FOR EACH ROW SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'owned Land gathering checkpoint failure';";
			command.ExecuteNonQuery();
		}
		MagicGatheringResult result;
		try
		{
			clock.Advance(TimeSpan.FromSeconds(1));
			result = service.Complete(runtime.Actor, operationId);
		}
		finally
		{
			using var connection = new MySqlConnection(connectionString);
			connection.Open();
			using var command = connection.CreateCommand();
			command.CommandText = "DROP TRIGGER IF EXISTS `land_gather_checkpoint_fail`;";
			command.ExecuteNonQuery();
		}
		using var failedRead = NewIndependentContext(connectionString);
		Db.MagicGatheringOperation parent = failedRead.MagicGatheringOperations.AsNoTracking()
			.Single(x => x.Id == operationId);
		var afterFailure = NativeOrganicObservation.Read(connectionString, fieldId, NativeOrganicSourceKind.Crop);
		double failedCredit = failedRead.CharactersMagicResources.AsNoTracking()
			.Single(x => x.CharacterId == fixture.CharacterId && x.MagicResourceId == fixture.ResourceId).Amount;
		Require(!result.Success && parent.Status == "NeedsReview" && parent.EcologicalApplied &&
			!parent.DestinationCredited && field.Changed &&
			before.Stock == afterFailure.Stock && before.Prepaid == afterFailure.Prepaid &&
			before.Revision == afterFailure.Revision && Same(failedCredit, 7.0),
			$"L-P06 injected owner-save failure was not isolated: {result.Message}, receipt {parent.Status}, field changed {field.Changed}, prepaid {afterFailure.Prepaid}, credit {failedCredit}.");
		Console.WriteLine($"L-P06-retry-setup queued:{runtime.World.SaveManager.IsQueued(field)} ownerStock:{field.CropYieldPotential} savedStock:{afterFailure.Stock} ownerHealth:{field.CropHealth}");
		runtime.World.SaveManager.Flush();
		var afterRetry = NativeOrganicObservation.Read(connectionString, fieldId, NativeOrganicSourceKind.Crop);
		using var finalRead = NewIndependentContext(connectionString);
		double finalCredit = finalRead.CharactersMagicResources.AsNoTracking()
			.Single(x => x.CharacterId == fixture.CharacterId && x.MagicResourceId == fixture.ResourceId).Amount;
		Console.WriteLine($"L-P06-retry-result queued:{runtime.World.SaveManager.IsQueued(field)} changed:{field.Changed} savedStock:{afterRetry.Stock} ownerStock:{field.CropYieldPotential} savedPrepaid:{afterRetry.Prepaid} ownerPrepaid:{field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop).PrepaidFraction} credit:{finalCredit}");
		Require(afterRetry.Prepaid == 0.875m && afterRetry.Revision > before.Revision &&
			afterRetry.Stock == field.CropYieldPotential &&
			Same(finalCredit, 7.0) && !service.Complete(runtime.Actor, operationId).Success &&
			finalRead.EnvironmentalMagicOperations.AsNoTracking().Count(x => x.Id == parent.EcologicalChildId) == 1,
			"L-P06 owner-save retry repeated the logical payment, personal credit or ecological child.");
		Console.WriteLine($"L-P06-checkpoint=passed operation:{operationId} field:{fieldId} persistedPrepaid:{before.Prepaid}->{afterFailure.Prepaid}->{afterRetry.Prepaid} nativeStock:{before.Stock}->{afterFailure.Stock}->{afterRetry.Stock} ownerStock:{field.CropYieldPotential} receipt:{parent.Status} credit:{failedCredit:F2}->{finalCredit:F2} child:{parent.EcologicalChildId} replayRefused:True");
		Require(service.Acknowledge(operationId).Success,
			"L-P06 reviewed owner must be released without replaying its failed payment.");
	}

	private static void RunLandPlayerCommandAndRepairProbe(NativeRuntime runtime,
		SkillLevelBasedMagicCapability capability, MagicGatheringService service, HarnessClock clock,
		EnvironmentalMagicCoordinator coordinator, Cell cell, string connectionString, FixtureIds fixture)
	{
		runtime.WorldMock.SetupGet(x => x.MagicGathering).Returns(service);
		MagicModule.EnsureMagicSchoolVerbRegistered("draw");
		double openingScar = coordinator.InspectState(cell).State.ScarDamage;
		if (openingScar >= 19.0)
		{
			EnvironmentalMagicOperationResult priorRepair = coordinator.ApplyOperation(cell,
				new EnvironmentalMagicOperationRequest(Guid.NewGuid(), runtime.Actor.Id,
					"fixture prior-scar repair", Repair: openingScar - 10.0));
			Require(priorRepair.Success, "L-P05 could not establish valid native recovery before the player action.");
			openingScar = coordinator.InspectState(cell).State.ScarDamage;
		}
		double landDamage = 19.0 - openingScar;
		Require(landDamage > 0.0 && capability.BuildingCommand(runtime.Actor,
			new StringStack($"gather land draw damage {landDamage.ToString(CultureInfo.InvariantCulture)}")),
			"L-P05 could not author the high-scar Land method.");
		MagicModule.MagicGeneric(runtime.Actor, $"draw gather {fixture.CapabilityId} draw 1");
		MagicGatheringTimedAction action = runtime.Actor.Effects.OfType<MagicGatheringTimedAction>().Single();
		clock.Advance(TimeSpan.FromSeconds(1));
		MagicGatheringResult completed = service.Complete(runtime.Actor, action.OperationId);
		Require(completed.Success, $"L-P05 command-started Land action did not complete: {completed.Message}");
		using var paidRead = NewIndependentContext(connectionString);
		double paidCredit = paidRead.CharactersMagicResources.AsNoTracking()
			.Single(x => x.CharacterId == fixture.CharacterId && x.MagicResourceId == fixture.ResourceId).Amount;
		Require(Same(paidCredit, 6.0), "L-P05 player command did not persist exactly one additional personal credit.");
		NativeOrganicPenaltyEvaluation suppressed = ForageRecoveryPenalty(coordinator, cell);
		Console.WriteLine($"L-P05-suppression-diagnostic openingScar:{openingScar:F4} plannedDamage:{landDamage:F4} observedScar:{coordinator.InspectState(cell).State.ScarDamage:F4} factor:{suppressed.Factor:F4} valid:{suppressed.IsValid} error:{suppressed.Error}");
		Require(suppressed.IsValid && suppressed.Factor >= 0.0 && suppressed.Factor <= 0.051,
			$"L-P05 native forage recovery was not suppressed after real Land damage: {suppressed.Factor} {suppressed.Error}");
		MagicModule.MagicGeneric(runtime.Actor, $"draw gather {fixture.CapabilityId} draw 100");
		Require(!runtime.Actor.Effects.OfType<MagicGatheringTimedAction>().Any(),
			"L-P05 overdraw unexpectedly began a timed action.");
		MagicModule.MagicGeneric(runtime.Actor, $"draw gather {fixture.CapabilityId} draw 1");
		MagicGatheringTimedAction interrupted = runtime.Actor.Effects.OfType<MagicGatheringTimedAction>().Single();
		runtime.Actor.RemoveEffect(interrupted, true);
		clock.Advance(TimeSpan.FromSeconds(1));
		Require(!service.Complete(runtime.Actor, interrupted.OperationId).Success,
			"L-P05 interrupted player action completed after its effect was removed.");
		double stockBeforeRepair = ReadForageYield(connectionString, cell.Id, "berries");
		double scarBeforeRepair = coordinator.InspectState(cell).State.ScarDamage;
		EnvironmentalMagicOperationResult repaired = coordinator.ApplyOperation(cell,
			new EnvironmentalMagicOperationRequest(Guid.NewGuid(), runtime.Actor.Id, "explicit repair probe",
				Repair: scarBeforeRepair - 1.0));
		Require(repaired.Success, $"L-P05 existing explicit room repair refused: {repaired.Error}");
		NativeOrganicPenaltyEvaluation restored = ForageRecoveryPenalty(coordinator, cell);
		using var repairedRead = NewIndependentContext(connectionString);
		double repairCredit = repairedRead.CharactersMagicResources.AsNoTracking()
			.Single(x => x.CharacterId == fixture.CharacterId && x.MagicResourceId == fixture.ResourceId).Amount;
		Require(restored.IsValid && restored.Factor > 0.0 && Same(repairCredit, 6.0) &&
			Same(ReadForageYield(connectionString, cell.Id, "berries"), stockBeforeRepair),
			"L-P05 explicit repair failed to restore future recovery or granted an instant source/personal refill.");
		AgricultureField cropField = (AgricultureField)(coordinator.FieldFor(cell) ??
			throw new InvalidOperationException("L-P05 lost the indexed crop field."));
		SetPrivateMember(cropField, "CropStage", AgricultureCropStage.Growing);
		cropField.Changed = true;
		var crop = Mock.Get(cropField.CurrentCrop);
		crop.SetupGet(x => x.MinimumMoisture).Returns(0);
		crop.SetupGet(x => x.MaximumMoisture).Returns(100);
		crop.SetupGet(x => x.MinimumTemperature).Returns(0);
		crop.SetupGet(x => x.MaximumTemperature).Returns(40);
		crop.SetupGet(x => x.PlantingWindows).Returns(Array.Empty<AgriculturePlantingWindow>());
		crop.SetupGet(x => x.PollinationDependency).Returns(AgriculturePollinationDependency.Beneficial);
		crop.SetupGet(x => x.PollinationHealthBonus).Returns(3);
		var apiary = new Mock<IAgricultureFieldApiary>();
		apiary.SetupGet(x => x.PollinationRadius).Returns(1);
		apiary.SetupGet(x => x.PollinationStrength).Returns(50);
		bool pollinationActive = true;
		var pollinator = new Mock<IAgricultureField>();
		pollinator.SetupGet(x => x.Cell).Returns(cell);
		pollinator.SetupGet(x => x.HasActiveApiary).Returns(true);
		pollinator.SetupGet(x => x.IsApiaryHappy).Returns(() => pollinationActive);
		pollinator.SetupGet(x => x.Apiary).Returns(apiary.Object);
		coordinator.RefreshPollinationCandidate(pollinator.Object);
		NativeOrganicSourceSnapshot cropSource = coordinator.InspectOrganicSource(cell, "crop");
		Require(cropSource.Lifecycle is not null, "L-P05 lost the current crop lifecycle.");
		bool prepared = cropField.TryApplyLandHealthCost(NativeOrganicSourceKind.Crop, cropSource.Lifecycle!, 1,
			out int preparationLoss, out _, out string preparationError);
		Require(prepared && preparationLoss == 1,
			$"L-P05 could not establish a positive current crop recovery context: {preparationError}");
		runtime.World.SaveManager.Flush();
		var activeContext = cropField.InspectCurrentOrganicRecoveryContext(NativeOrganicPenaltyChannel.CropHealthRecovery);
		Require(activeContext?.BaselineIncrease == 4.0,
			$"L-P05 pollinated crop did not expose the delivered health-recovery baseline of four (stage {cropField.CropStage}, health {cropField.CropHealth}, baseline {activeContext?.BaselineIncrease}).");
		Require(capability.BuildingCommand(runtime.Actor, new StringStack("gather land draw source 1 remove")) &&
			capability.BuildingCommand(runtime.Actor, new StringStack("gather land draw source add crop 0.125")) &&
			capability.BuildingCommand(runtime.Actor, new StringStack("gather land draw damage 1")),
			"L-P05 could not author the dynamic-crop command probe.");
		Require(service.Preview(runtime.Actor, runtime.Capability, "draw", 1.0).Success,
			"L-P05 pollinated crop did not provide a valid baseline before invalidation.");
		var cropBefore = NativeOrganicObservation.Read(connectionString, cropField.Id, NativeOrganicSourceKind.Crop);
		pollinationActive = false;
		MagicGatheringResult invalidPreview = service.Preview(runtime.Actor, runtime.Capability, "draw", 1.0);
		MagicModule.MagicGeneric(runtime.Actor, $"draw gather {fixture.CapabilityId} draw 1");
		var cropAfter = NativeOrganicObservation.Read(connectionString, cropField.Id, NativeOrganicSourceKind.Crop);
		Console.WriteLine($"L-P05-crop-diagnostic baseline:{cropField.InspectCurrentOrganicRecoveryContext(NativeOrganicPenaltyChannel.CropHealthRecovery)?.BaselineIncrease} preview:{invalidPreview.Success} reason:{invalidPreview.Message} actionCount:{runtime.Actor.Effects.OfType<MagicGatheringTimedAction>().Count()} stock:{cropBefore.Stock}->{cropAfter.Stock} prepaid:{cropBefore.Prepaid}->{cropAfter.Prepaid} revision:{cropBefore.Revision}->{cropAfter.Revision} credit:{runtime.Actor.MagicResourceAmounts[runtime.Resource]}");
		Require(!runtime.Actor.Effects.OfType<MagicGatheringTimedAction>().Any() &&
			cropBefore.Stock == cropAfter.Stock && cropBefore.Prepaid == cropAfter.Prepaid &&
			cropBefore.Revision == cropAfter.Revision &&
			Same(runtime.Actor.MagicResourceAmounts[runtime.Resource], 6.0),
			"L-P05 invalid live crop/pollination factor changed native state or personal credit.");
		pollinationActive = true;
		Console.WriteLine($"L-P05-dynamic-crop=passed field:{cropField.Id} pollination:active->inactive->active nativeStock:{cropBefore.Stock}->{cropAfter.Stock} prepaid:{cropBefore.Prepaid}->{cropAfter.Prepaid} credit:6.00 invalidActionRefused:True");
		Console.WriteLine($"L-P05=passed operation:{action.OperationId} playerCredit:{paidCredit:F2} scar:{scarBeforeRepair:F2}->1.00 forageRecoveryFactor:{suppressed.Factor:F2}->{restored.Factor:F2} berries:{stockBeforeRepair:F2} interrupted:{interrupted.OperationId} overdrawRefused:True");
	}

	private static NativeOrganicPenaltyEvaluation ForageRecoveryPenalty(EnvironmentalMagicCoordinator coordinator,
		Cell cell) => coordinator.EvaluateOrganicPenalty(cell, NativeOrganicPenaltyChannel.ForageReplenishment,
			new NativeOrganicPenaltyContext(NativeOrganicSourceKind.Forage, "forage:berries",
				cell.GetForagableYield("berries"), 0.0, 0.0, 100.0, 0.0, 0.0));

	private static int RunLandActionReader(string[] arguments)
	{
		if (arguments.Length != 13 || !Guid.TryParse(arguments[12], out Guid operationId))
		{
			throw new ArgumentException("The Land action reader requires the owned database, fixture IDs, field, environmental resource and operation ID.");
		}

		var fixture = new FixtureIds(long.Parse(arguments[1]), long.Parse(arguments[2]),
			long.Parse(arguments[3]), long.Parse(arguments[4]), long.Parse(arguments[5]),
			long.Parse(arguments[6]), long.Parse(arguments[7]),
			long.TryParse(arguments[8], out long existingWound) && existingWound > 0 ? existingWound : null,
			long.Parse(arguments[9]));
		long fieldId = long.Parse(arguments[10]);
		long environmentalResourceId = long.Parse(arguments[11]);
		using var database = TestDatabase.OpenExistingOwned(arguments[0]);
		ConfigureNativeDatabase(database.ConnectionString);
		NativeRuntime runtime = NativeRuntime.Load(fixture, database.ConnectionString);
		SimpleOrganicWound wound = runtime.Actor.Wounds.OfType<SimpleOrganicWound>().Single();
		Require(Same(runtime.Actor.MagicResourceAmounts[runtime.Resource], 5.0) &&
			Same(wound.CurrentDamage, 6.0) && Same(wound.CurrentPain, 8.0) &&
			Same(wound.CurrentStun, 10.0),
			"L-P03 separate process did not reconstruct the exact personal credit and wound channels.");
		var field = NativeOrganicRuntime.Load(database.ConnectionString, fieldId).Field;
		Require(field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop).PrepaidFraction == 0.125m,
			"L-P03 separate process did not reconstruct the crop prepaid remainder.");
		using var read = NewIndependentContext(database.ConnectionString);
		Db.MagicGatheringOperation parent = read.MagicGatheringOperations.AsNoTracking()
			.Single(x => x.Id == operationId);
		Db.EnvironmentalMagicOperation child = read.EnvironmentalMagicOperations.AsNoTracking()
			.Single(x => x.Id == parent.EcologicalChildId);
		Db.CellMagicResource ambient = read.CellsMagicResources.AsNoTracking()
			.Single(x => x.CellId == fixture.CellId && x.MagicResourceId == environmentalResourceId);
		Require(parent.Status == "Completed" && parent.AccountingPersisted && parent.EcologicalApplied &&
			child.Id != parent.Id && child.Id == parent.EcologicalChildId &&
			read.MagicGatheringParticipants.AsNoTracking().Any(x => x.OperationId == operationId) &&
			ambient.Amount >= 0.0 && ambient.Amount < 0.1 &&
			Same(ReadForageYield(database.ConnectionString, fixture.CellId, "herbs"), 0.0) &&
			Same(ReadForageYield(database.ConnectionString, fixture.CellId, "berries"), 97.25),
			"L-P03 separate process did not reconstruct the completed parent, child and physical debits.");
		MagicGatheringResult replay = new MagicGatheringService(runtime.World).Complete(runtime.Actor, operationId);
		Require(!replay.Success && Same(runtime.Actor.MagicResourceAmounts[runtime.Resource], 5.0),
			"L-P03 replay accepted an already completed Land token.");
		Console.WriteLine($"L-P03=passed operation:{operationId} character:{fixture.CharacterId} body:{fixture.BodyId} field:{fieldId} credit:5 wound:6/8/10 child:{child.Id} replayRefused:True");
		return 0;
	}

	private static void RunLandActionReaderProcess(string databaseName, FixtureIds fixture, long fieldId,
		long environmentalResourceId, Guid operationId)
	{
		string executable = Assembly.GetExecutingAssembly().Location;
		string host = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet";
		string arguments = string.Join(' ', $"\"{executable}\"", "--land-action-reader",
			databaseName, fixture.CharacterId, fixture.BodyId, fixture.CellId, fixture.ResourceId,
			fixture.CapabilityId, fixture.HealthStrategyId, fixture.TraitExpressionId,
			fixture.ExistingWoundId?.ToString() ?? "0", fixture.BodypartId, fieldId,
			environmentalResourceId, operationId);
		using Process reader = Process.Start(new ProcessStartInfo(host, arguments)
		{
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			CreateNoWindow = true
		}) ?? throw new InvalidOperationException("The separate Land action reader could not be started.");
		string output = reader.StandardOutput.ReadToEnd();
		string error = reader.StandardError.ReadToEnd();
		reader.WaitForExit();
		Require(reader.ExitCode == 0, $"The separate Land action reader failed: {error} {output}".Trim());
		Console.Write(output);
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
		Require(created.BuildingCommand(actor.Object, new StringStack("organic source add forage berries")),
			"L-P01 could not author the second forage declaration through the real profile editor.");
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
		Require(reloaded.OrganicValidationErrors.Count == 0 && reloaded.OrganicSources.Count == 3 &&
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
		Require(originalSources == 3 && cloneSources == 2,
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

	private static double ReadForageYield(string connectionString, long cellId, string key = "herbs")
	{
		using var context = NewIndependentContext(connectionString);
		return context.CellsForagableYields.AsNoTracking()
			.Single(x => x.CellId == cellId && x.ForagableType == key).Yield;
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
		long ConstructorHalfCellId, long ConstructorZeroCellId, long CoordinatorCropFieldId,
		FixtureIds CoordinatorActorFixture);

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
			forageProfile.ForagableProfilesMaximumYields.Add(new Db.ForagableProfilesMaximumYields
			{
				ForageType = "berries", Yield = 100.0
			});
			forageProfile.ForagableProfilesHourlyYieldGains.Add(new Db.ForagableProfilesHourlyYieldGains
			{
				ForageType = "berries", Yield = 10.0
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
				constructorHalfBase.CellId, constructorZeroBase.CellId, coordinatorCrop.Id,
				coordinatorCropBase);
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
