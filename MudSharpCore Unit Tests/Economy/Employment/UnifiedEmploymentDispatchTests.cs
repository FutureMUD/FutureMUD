using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Arenas;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Commands.Modules;
using MudSharp.Community;
using MudSharp.Community.Boards;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Economy;
using MudSharp.Economy.Auctions;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Economy.Hospitals;
using MudSharp.Economy.Hotels;
using MudSharp.Economy.Property;
using MudSharp.Economy.Shops;
using MudSharp.Economy.Stables;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.Vehicles;
using MudSharp.Work.Crafts;

#nullable enable

namespace MudSharp_Unit_Tests.Economy.Employment;

[TestClass]
[DoNotParallelize]
public class UnifiedEmploymentDispatchTests
{
	private sealed record FMDBState(FuturemudDatabaseContext? Context, object? Connection, uint InstanceCount);

	private sealed record PermanentShopFixture(Mock<IPermanentShop> Shop, EmploymentHostState State);

	private sealed record HospitalSupplyTestFixture(
		Mock<IHospital> Hospital,
		Mock<IHospitalServiceRequest> Request,
		EmploymentTaskContext Context,
		ICharacter Doctor);

	private delegate bool TryCollectTaskItemsCallback(ICharacter actor,
		IReadOnlyCollection<(IGameItem Item, ICell Source)> items, out string reason);
	private delegate bool CanChargeCallback(decimal amount, out string reason);

	[TestMethod]
	public void HostShells_MinimumHostTypes_ImplementEmploymentHost()
	{
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(IShop)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(Shop)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(PermanentShop)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(IAuctionHouse)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(AuctionHouse)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(ICombatArena)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(CombatArena)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(IBank)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(Bank)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(IStable)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(Stable)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(IHotel)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(Hotel)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(IClan)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(Clan)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(IHospital)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(Hospital)));
		Assert.AreEqual(6, (int)EmploymentHostType.Other);
		Assert.AreEqual(7, (int)EmploymentHostType.Clan);
		Assert.AreEqual(8, (int)EmploymentHostType.Hospital);
	}

	[TestMethod]
	public void HospitalActions_AreRegisteredForMedicalEmployment()
	{
		var service = EmploymentActionCatalog.Get("hospitalservice");

		Assert.IsNotNull(service);
		Assert.AreEqual(EmploymentActionCatalogStatus.Executable, service!.Status);
		Assert.AreEqual(EmploymentActionStepType.HospitalService, service.StepType);
		Assert.IsTrue(service.RequiredAuthority.Contains(EmploymentAuthority.PerformMedicalServices));
		CollectionAssert.Contains(service.RequiredCapabilities.ToArray(), EmploymentAICapability.CanPerformMedicalServices);
		Assert.AreSame(service, EmploymentActionCatalog.Get("medicalservice"));

		var admin = EmploymentActionCatalog.Get("hospitaladmin");

		Assert.IsNotNull(admin);
		Assert.AreEqual(EmploymentActionCatalogStatus.Executable, admin!.Status);
		Assert.AreEqual(EmploymentActionStepType.HospitalAdministration, admin.StepType);
		Assert.IsTrue(admin.RequiredAuthority.Contains(EmploymentAuthority.ManageMedicalServices));
		Assert.IsTrue(admin.RequiredAuthority.Contains(EmploymentAuthority.ManageHospitalAccounts));
		Assert.IsTrue(admin.RequiredAuthority.Contains(EmploymentAuthority.ManageHospitalFacilities));
		CollectionAssert.Contains(admin.RequiredCapabilities.ToArray(), EmploymentAICapability.CanPerformMedicalServices);
		Assert.AreSame(admin, EmploymentActionCatalog.Get("medicaladmin"));

		var prep = EmploymentActionCatalog.Get("hospitalprep");

		Assert.IsNotNull(prep);
		Assert.AreEqual(EmploymentActionCatalogStatus.Executable, prep!.Status);
		Assert.AreEqual(EmploymentActionStepType.HospitalPatientPreparation, prep.StepType);
		Assert.IsTrue(prep.RequiredAuthority.Contains(EmploymentAuthority.PerformMedicalServices));
		CollectionAssert.Contains(prep.RequiredCapabilities.ToArray(), EmploymentAICapability.CanPerformMedicalServices);
		Assert.AreSame(prep, EmploymentActionCatalog.Get("patientprep"));

		var supply = EmploymentActionCatalog.Get("hospitalsupply");

		Assert.IsNotNull(supply);
		Assert.AreEqual(EmploymentActionCatalogStatus.Executable, supply!.Status);
		Assert.AreEqual(EmploymentActionStepType.HospitalSupplyPreparation, supply.StepType);
		Assert.IsTrue(supply.RequiredAuthority.Contains(EmploymentAuthority.PrepareMedicalSupplies));
		CollectionAssert.Contains(supply.RequiredCapabilities.ToArray(), EmploymentAICapability.CanPrepareHospitalSupplies);
		Assert.AreSame(supply, EmploymentActionCatalog.Get("medicalsupply"));
	}

	[TestMethod]
	public void HospitalSupplyPreparation_PrefersOrderlyOverDoctor()
	{
		var fixture = HospitalSupplyFixture(includeOrderly: true);
		var step = new HospitalSupplyPreparationActionStep(fixture.Hospital.Object, fixture.Request.Object);

		Assert.IsFalse(step.CanExecute(fixture.Context, fixture.Doctor, out var reason));
		StringAssert.Contains(reason, "non-medical supply employee");
	}

	[TestMethod]
	public void HospitalSupplyPreparation_AllowsDoctorFallbackWhenNoOrderlyIsAvailable()
	{
		var fixture = HospitalSupplyFixture(includeOrderly: false);
		var step = new HospitalSupplyPreparationActionStep(fixture.Hospital.Object, fixture.Request.Object);

		Assert.IsTrue(step.CanExecute(fixture.Context, fixture.Doctor, out var reason), reason);
	}

	[TestMethod]
	public void HospitalSupplyPreparation_AllowsDoctorFallbackWhenOrderlyIsBusy()
	{
		var fixture = HospitalSupplyFixture(includeOrderly: true, orderlyBusy: true);
		var step = new HospitalSupplyPreparationActionStep(fixture.Hospital.Object, fixture.Request.Object);

		Assert.IsTrue(step.CanExecute(fixture.Context, fixture.Doctor, out var reason), reason);
	}

	[TestMethod]
	public void HospitalSupplyPreparation_PrefersOrderlyForImplicitTreatmentSupplies()
	{
		var fixture = HospitalImplicitTreatmentSupplyFixture(includeOrderly: true);
		var step = new HospitalSupplyPreparationActionStep(fixture.Hospital.Object, fixture.Request.Object);

		Assert.IsFalse(step.CanExecute(fixture.Context, fixture.Doctor, out var reason));
		StringAssert.Contains(reason, "non-medical supply employee");
	}

	[TestMethod]
	public void HospitalSupplyPreparation_DetectsImplicitTreatmentStockForCommandRoutedServices()
	{
		var supplyRoom = Cell(721, "supply room");
		var treatment = new Mock<ITreatment>();
		treatment.Setup(x => x.IsTreatmentType(It.IsAny<TreatmentType>())).Returns(true);
		var treatmentItem = Item(722, "treatment kit");
		treatmentItem.Setup(x => x.GetItemType<ITreatment>()).Returns(treatment.Object);
		supplyRoom.SetupGet(x => x.GameItems).Returns([treatmentItem.Object]);
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.SupplyRooms).Returns([supplyRoom.Object]);
		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.ServiceType).Returns(HospitalServiceType.FullTreatment);
		service.SetupGet(x => x.RequiredEquipment).Returns([]);
		var request = new Mock<IHospitalServiceRequest>();
		request.SetupGet(x => x.Service).Returns(service.Object);

		Assert.IsTrue(HospitalSupplyPreparationActionStep.HasPreparatorySupplyWork(hospital.Object, request.Object));

		supplyRoom.SetupGet(x => x.GameItems).Returns([]);
		Assert.IsFalse(HospitalSupplyPreparationActionStep.HasPreparatorySupplyWork(hospital.Object, request.Object));
	}

	[TestMethod]
	public void HospitalRequestPlanner_PrioritisesPatientPreparationBeforeSupplyForTheatreTreatment()
	{
		var supplyRoom = Cell(741, "supply room");
		var theatre = Cell(742, "operating theatre");
		theatre.SetupGet(x => x.Characters).Returns(Array.Empty<ICharacter>());
		var treatment = new Mock<ITreatment>();
		treatment.Setup(x => x.IsTreatmentType(It.IsAny<TreatmentType>())).Returns(true);
		var treatmentItem = Item(743, "treatment kit");
		treatmentItem.Setup(x => x.GetItemType<ITreatment>()).Returns(treatment.Object);
		supplyRoom.SetupGet(x => x.GameItems).Returns([treatmentItem.Object]);
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.Id).Returns(744);
		hospital.SetupGet(x => x.Name).Returns("central clinic");
		hospital.SetupGet(x => x.SupplyRooms).Returns([supplyRoom.Object]);
		hospital.SetupGet(x => x.OperatingTheatres).Returns([theatre.Object]);
		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.ServiceType).Returns(HospitalServiceType.Stabilisation);
		service.SetupGet(x => x.RequiredEquipment).Returns([]);
		var request = new Mock<IHospitalServiceRequest>();
		request.SetupGet(x => x.Id).Returns(745);
		request.SetupGet(x => x.Hospital).Returns(hospital.Object);
		request.SetupGet(x => x.Service).Returns(service.Object);

		var method = typeof(EconomyModule).GetMethod("ServiceRequestActionSteps",
			BindingFlags.NonPublic | BindingFlags.Static);

		Assert.IsNotNull(method);
		var steps = ((IEnumerable<IEmploymentActionStep>)method!.Invoke(null, [hospital.Object, request.Object])!).ToList();
		CollectionAssert.AreEqual(
			new[]
			{
				EmploymentActionStepType.HospitalPatientPreparation,
				EmploymentActionStepType.HospitalSupplyPreparation,
				EmploymentActionStepType.HospitalService
			},
			steps.Select(x => x.StepType).ToArray());
	}

	[TestMethod]
	public void EmploymentTaskAssignmentAudit_UsesCurrentStepRequirementsForMultiRoleHospitalTasks()
	{
		var currency = Currency();
		var supplyRoom = Cell(731, "supply room");
		var theatre = Cell(732, "operating theatre");
		var (hospital, state) = HospitalEmploymentHost(733, "central clinic", currency.Object,
			[supplyRoom.Object], [theatre.Object], 100.0M);
		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.Id).Returns(734);
		service.SetupGet(x => x.Name).Returns("configured service");
		service.SetupGet(x => x.PreferOperatingTheatre).Returns(true);
		service.SetupGet(x => x.RequiredEquipment).Returns([
			new HospitalServiceEquipmentRequirement(1, EmploymentItemSelector.ForPrototype(500))
		]);
		var request = new Mock<IHospitalServiceRequest>();
		request.SetupGet(x => x.Id).Returns(735);
		request.SetupGet(x => x.Hospital).Returns(hospital.Object);
		request.SetupGet(x => x.Service).Returns(service.Object);
		request.SetupGet(x => x.Status).Returns(HospitalServiceRequestStatus.Queued);
		request.SetupProperty(x => x.OperatingTheatreCellId);
		request.SetupProperty(x => x.UsedInPlaceFallback);
		var orderly = Character(736, "Orderly").Object;
		state.Hire(orderly, Offer(currency.Object, EmploymentRole.HospitalOrderly,
			EmploymentAuthority.PrepareMedicalSupplies), null);
		var task = (EmploymentActiveTask)state.TaskBoard.CreateActiveTask("prepare configured service",
			new EmploymentActionPlan([
				new HospitalSupplyPreparationActionStep(hospital.Object, request.Object),
				new HospitalServiceActionStep(hospital.Object, request.Object)
			]), null);
		task.Assign(orderly);

		var results = state.TaskBoard.AuditActiveTaskAssignments();

		Assert.AreEqual(0, results.Count);
		Assert.AreEqual(EmploymentTaskStatus.Assigned, task.Status);
		Assert.AreSame(orderly, task.AssignedEmployee);
	}

	[TestMethod]
	public void HospitalServiceAvailability_BlocksServiceWhenRequiredEquipmentMissing()
	{
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.Name).Returns("central clinic");
		hospital.SetupGet(x => x.IsTrading).Returns(true);
		hospital.SetupGet(x => x.SupplyRooms).Returns([Cell(703, "supply room").Object]);
		SetupAvailableMedicalEmployee(hospital);

		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.IsActive).Returns(true);
		service.SetupGet(x => x.ServiceType).Returns(HospitalServiceType.Binding);
		service.SetupGet(x => x.RequiredEquipment).Returns([
			new HospitalServiceEquipmentRequirement(1, EmploymentItemSelector.ForPrototype(500))
		]);

		var result = HospitalServiceAvailability.Evaluate(hospital.Object, service.Object);

		Assert.IsFalse(result.Available);
		StringAssert.Contains(result.Reason, "required equipment");
	}

	[TestMethod]
	public void HospitalServiceAvailability_AllowsServiceWhenRequiredEquipmentIsStocked()
	{
		var supplyItems = new List<IGameItem> { Item(9000, "bandage roll", prototypeId: 500).Object };
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.Name).Returns("central clinic");
		hospital.SetupGet(x => x.IsTrading).Returns(true);
		hospital.SetupGet(x => x.SupplyRooms).Returns([PhysicalCell(704, "supply room", supplyItems).Object]);
		SetupAvailableMedicalEmployee(hospital);

		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.IsActive).Returns(true);
		service.SetupGet(x => x.ServiceType).Returns(HospitalServiceType.Binding);
		service.SetupGet(x => x.RequiredEquipment).Returns([
			new HospitalServiceEquipmentRequirement(1, EmploymentItemSelector.ForPrototype(500))
		]);

		var result = HospitalServiceAvailability.Evaluate(hospital.Object, service.Object);

		Assert.IsTrue(result.Available, result.Reason);
	}

	[TestMethod]
	public void HospitalServiceAvailability_BlocksBloodDonationWhenKnownDonorNeedsEmptyContainer()
	{
		var unitManager = new Mock<MudSharp.Framework.Units.IUnitManager>();
		unitManager.SetupGet(x => x.BaseFluidToLitres).Returns(1.0);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.UnitManager).Returns(unitManager.Object);

		var fillerLiquid = new Mock<ILiquid>();
		fillerLiquid.SetupGet(x => x.Density).Returns(1.0);
		var mixture = new LiquidMixture(fillerLiquid.Object, 0.1, gameworld.Object);
		var liquidContainer = new Mock<ILiquidContainer>();
		liquidContainer.SetupGet(x => x.LiquidCapacity).Returns(1.0);
		liquidContainer.SetupGet(x => x.LiquidVolume).Returns(0.1);
		liquidContainer.SetupGet(x => x.LiquidMixture).Returns(mixture);
		var containerItem = Item(9001, "used blood bag");
		containerItem.Setup(x => x.GetItemType<ILiquidContainer>()).Returns(liquidContainer.Object);

		var supplyItems = new List<IGameItem> { containerItem.Object };
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.Name).Returns("central clinic");
		hospital.SetupGet(x => x.IsTrading).Returns(true);
		hospital.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		hospital.SetupGet(x => x.SupplyRooms).Returns([PhysicalCell(705, "supply room", supplyItems).Object]);
		SetupAvailableMedicalEmployee(hospital);

		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.IsActive).Returns(true);
		service.SetupGet(x => x.ServiceType).Returns(HospitalServiceType.BloodDonation);
		service.SetupGet(x => x.RequiredEquipment).Returns([]);
		service.SetupGet(x => x.BloodVolumeLitres).Returns(0.5);

		var body = new Mock<MudSharp.Body.IBody>();
		body.SetupGet(x => x.BloodLiquid).Returns((ILiquid)null!);
		var donor = Character(6, "Donor", gameworld: gameworld.Object);
		donor.SetupGet(x => x.Body).Returns(body.Object);

		var result = HospitalServiceAvailability.Evaluate(hospital.Object, service.Object, patient: donor.Object);

		Assert.IsFalse(result.Available);
		StringAssert.Contains(result.Reason, "empty blood container");
	}

	[TestMethod]
	public void HospitalServiceAvailability_BlocksServiceWhenNoMedicalEmployeeIsAvailable()
	{
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.Name).Returns("central clinic");
		hospital.SetupGet(x => x.IsTrading).Returns(true);
		hospital.SetupGet(x => x.EmploymentContracts).Returns(Array.Empty<IEmploymentContract>());
		var taskBoard = new Mock<IEmploymentTaskBoard>();
		taskBoard.SetupGet(x => x.ActiveTasks).Returns(Array.Empty<IEmploymentActiveTask>());
		hospital.SetupGet(x => x.TaskBoard).Returns(taskBoard.Object);

		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.IsActive).Returns(true);
		service.SetupGet(x => x.ServiceType).Returns(HospitalServiceType.Binding);
		service.SetupGet(x => x.RequiredEquipment).Returns([]);

		var result = HospitalServiceAvailability.Evaluate(hospital.Object, service.Object);

		Assert.IsFalse(result.Available);
		StringAssert.Contains(result.Reason, "medical employee");
	}

	[TestMethod]
	public void HospitalServiceAvailability_BlocksServiceWhenMedicalEmployeeCannotClaimMedicalTasks()
	{
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.Id).Returns(7903);
		hospital.SetupGet(x => x.Name).Returns("central clinic");
		hospital.SetupGet(x => x.FrameworkItemType).Returns("Hospital");
		hospital.SetupGet(x => x.EmploymentHostType).Returns(EmploymentHostType.Hospital);
		hospital.SetupGet(x => x.IsTrading).Returns(true);
		var state = new EmploymentHostState(hospital.Object);
		var taskBoard = new Mock<IEmploymentTaskBoard>();
		taskBoard.SetupGet(x => x.ActiveTasks).Returns(Array.Empty<IEmploymentActiveTask>());
		hospital.SetupGet(x => x.Employment).Returns(state);
		hospital.SetupGet(x => x.TaskBoard).Returns(taskBoard.Object);
		hospital.SetupGet(x => x.EmploymentContracts).Returns(() => state.EmploymentContracts);
		var doctorCell = Cell(7904, "staff room");
		var doctor = NpcCharacter(7905, "Doctor", [EmploymentWorkerAi(EmploymentAICapability.CanDeliverItems)]);
		doctor.SetupGet(x => x.Location).Returns(doctorCell.Object);
		state.Hire(doctor.Object, Offer(Currency().Object, EmploymentRole.MedicalWorker,
			EmploymentAuthority.PerformMedicalServices), null);

		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.IsActive).Returns(true);
		service.SetupGet(x => x.ServiceType).Returns(HospitalServiceType.Binding);
		service.SetupGet(x => x.RequiredEquipment).Returns([]);

		var result = HospitalServiceAvailability.Evaluate(hospital.Object, service.Object);

		Assert.IsFalse(result.Available);
		StringAssert.Contains(result.Reason, "medical employee");
	}

	[TestMethod]
	public void HospitalMedicalServiceRunner_DispatchesStabilisationService()
	{
		AssertCombinedHospitalServiceDispatch(HospitalServiceType.Stabilisation,
			"no visible immediately life-threatening wounds or blood loss");
	}

	[TestMethod]
	public void HospitalMedicalServiceRunner_DispatchesFullTreatmentService()
	{
		AssertCombinedHospitalServiceDispatch(HospitalServiceType.FullTreatment,
			"no visible wounds, fractures or blood loss");
	}

	[TestMethod]
	public void HospitalMedicalServiceRunner_ChargesUsageBilledServiceToDebt()
	{
		var currency = Currency();
		var bindingService = new Mock<IHospitalService>();
		bindingService.SetupGet(x => x.Id).Returns(9120);
		bindingService.SetupGet(x => x.Name).Returns("Binding");
		bindingService.SetupGet(x => x.ServiceType).Returns(HospitalServiceType.Binding);
		bindingService.SetupGet(x => x.IsActive).Returns(true);
		bindingService.SetupGet(x => x.Price).Returns(12.5M);
		bindingService.SetupGet(x => x.SortOrder).Returns(0);
		var fullTreatment = new Mock<IHospitalService>();
		fullTreatment.SetupGet(x => x.Id).Returns(9121);
		fullTreatment.SetupGet(x => x.Name).Returns("Full Treatment");
		fullTreatment.SetupGet(x => x.ServiceType).Returns(HospitalServiceType.FullTreatment);
		fullTreatment.SetupGet(x => x.AllowDebt).Returns(true);
		fullTreatment.SetupGet(x => x.RequiredEquipment).Returns(Array.Empty<HospitalServiceEquipmentRequirement>());
		fullTreatment.SetupGet(x => x.RequiresRecovery).Returns(false);

		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.Id).Returns(9122);
		hospital.SetupGet(x => x.Name).Returns("central clinic");
		hospital.SetupGet(x => x.Currency).Returns(currency.Object);
		hospital.SetupGet(x => x.Services).Returns([bindingService.Object, fullTreatment.Object]);
		hospital.SetupGet(x => x.EmploymentRegister).Returns(new Mock<IEmploymentRegister>().Object);

		var debtAccount = new Mock<IHospitalPatientDebtAccount>();
		debtAccount.SetupGet(x => x.PatientName).Returns("Patient");
		debtAccount.Setup(x => x.CanCharge(It.IsAny<decimal>(), out It.Ref<string>.IsAny))
		           .Returns(new CanChargeCallback((decimal _, out string reason) =>
		           {
			           reason = string.Empty;
			           return true;
		           }));
		hospital.Setup(x => x.DebtAccountFor(It.IsAny<ICharacter>(), true)).Returns(debtAccount.Object);

		var employee = Character(9123, "Doctor");
		var patient = Character(9124, "Patient");
		employee.Setup(x => x.ColocatedWith(patient.Object)).Returns(true);
		var body = new Mock<MudSharp.Body.IBody>();
		body.SetupGet(x => x.TotalBloodVolumeLitres).Returns(5.0);
		body.SetupGet(x => x.CurrentBloodVolumeLitres).Returns(5.0);
		patient.SetupGet(x => x.Body).Returns(body.Object);
		patient.SetupGet(x => x.Wounds).Returns(Array.Empty<IWound>());
		patient.Setup(x => x.VisibleWounds(It.IsAny<IPerceiver>(), WoundExaminationType.Examination))
		       .Returns(Array.Empty<IWound>());

		var request = new Mock<IHospitalServiceRequest>();
		request.SetupGet(x => x.Id).Returns(9126);
		request.SetupGet(x => x.Hospital).Returns(hospital.Object);
		request.SetupGet(x => x.Service).Returns(fullTreatment.Object);
		request.SetupGet(x => x.Status).Returns(HospitalServiceRequestStatus.Queued);
		request.SetupGet(x => x.PaymentMethod).Returns(HospitalPaymentMethod.Debt);
		request.SetupGet(x => x.PatientId).Returns(patient.Object.Id);
		request.SetupGet(x => x.Patient).Returns(patient.Object);
		var task = new EmploymentActiveTask(hospital.Object, "Treat patient",
			new EmploymentActionPlan([new HospitalServiceActionStep(hospital.Object, request.Object)]), Guid.NewGuid());
		task.Assign(employee.Object);
		task.MarkStep(0, EmploymentActionStepStatus.InProgress,
			new EmploymentActionStepOperationalState(
				OperationalPayload:
				"hospitalservice;hospital=9122;request=9126;type=FullTreatment;completed=False;done=bind;charges=Binding:1"));
		var context = new EmploymentTaskContext(hospital.Object);
		context.HydrateTaskState(task, 0);

		var result = HospitalMedicalServiceRunner.ExecuteServiceRequest(context, employee.Object, hospital.Object, request.Object);

		Assert.IsTrue(result.Success, result.Message);
		debtAccount.Verify(x => x.Charge(12.5M, It.Is<string>(text => text.Contains("Full Treatment"))), Times.Once);
		request.Verify(x => x.MarkCharged(0.0M, 12.5M, 12.5M), Times.Once);
		request.Verify(x => x.MarkStatus(HospitalServiceRequestStatus.Completed,
			It.Is<string>(text => text.Contains("Usage-billed hospital charge"))), Times.Once);
	}

	[TestMethod]
	public void HospitalPatientFlow_BlocksReservedAndOccupiedTheatres()
	{
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.Name).Returns("central clinic");
		var theatre = Cell(702, "operating theatre");
		var request = new Mock<IHospitalServiceRequest>();
		request.SetupGet(x => x.Id).Returns(1);
		request.SetupGet(x => x.Patient).Returns((ICharacter?)null);

		var other = new Mock<IHospitalServiceRequest>();
		other.SetupGet(x => x.Id).Returns(2);
		other.SetupGet(x => x.OperatingTheatreCellId).Returns(theatre.Object.Id);
		hospital.SetupGet(x => x.ActiveServiceRequests).Returns([other.Object]);
		theatre.SetupGet(x => x.Characters).Returns(Array.Empty<ICharacter>());

		Assert.IsFalse(HospitalPatientFlow.IsTheatreAvailable(hospital.Object, request.Object, theatre.Object, out var reason));
		StringAssert.Contains(reason, "already reserved");

		var visitor = Character(3, "Visitor");
		hospital.SetupGet(x => x.ActiveServiceRequests).Returns(Array.Empty<IHospitalServiceRequest>());
		hospital.Setup(x => x.IsEmployee(It.IsAny<ICharacter>())).Returns(false);
		theatre.SetupGet(x => x.Characters).Returns([visitor.Object]);

		Assert.IsFalse(HospitalPatientFlow.IsTheatreAvailable(hospital.Object, request.Object, theatre.Object, out reason));
		StringAssert.Contains(reason, "occupied");

		hospital.Setup(x => x.IsEmployee(It.IsAny<ICharacter>())).Returns(true);

		Assert.IsTrue(HospitalPatientFlow.IsTheatreAvailable(hospital.Object, request.Object, theatre.Object, out reason), reason);
	}

	[TestMethod]
	public void HospitalPatientFlow_AllowsOwnReservedTheatreAndPatientIdentity()
	{
		var theatre = Cell(706, "operating theatre");
		var actualPatient = Character(707, "Patient");
		var requestPatient = Character(708, "Patient Record");
		theatre.SetupGet(x => x.Characters).Returns([actualPatient.Object]);
		var taskId = Guid.NewGuid();
		var request = new Mock<IHospitalServiceRequest>();
		request.SetupGet(x => x.Id).Returns(10);
		request.SetupGet(x => x.PatientId).Returns(actualPatient.Object.Id);
		request.SetupGet(x => x.Patient).Returns(requestPatient.Object);
		request.SetupGet(x => x.EmploymentTaskId).Returns(taskId);

		var mirroredRequest = new Mock<IHospitalServiceRequest>();
		mirroredRequest.SetupGet(x => x.Id).Returns(11);
		mirroredRequest.SetupGet(x => x.OperatingTheatreCellId).Returns(theatre.Object.Id);
		mirroredRequest.SetupGet(x => x.EmploymentTaskId).Returns(taskId);

		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.ActiveServiceRequests).Returns([mirroredRequest.Object]);
		hospital.Setup(x => x.IsEmployee(actualPatient.Object)).Returns(false);

		Assert.IsTrue(HospitalPatientFlow.IsTheatreAvailable(hospital.Object, request.Object, theatre.Object, out var reason), reason);
	}

	[TestMethod]
	public void HospitalPatientFlow_FullTreatmentUsesTheatreEvenWhenPreferenceDisabled()
	{
		var waitingRoom = Cell(760, "waiting room");
		var theatre = Cell(761, "operating theatre");
		theatre.SetupGet(x => x.Characters).Returns(Array.Empty<ICharacter>());
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.Name).Returns("central clinic");
		hospital.SetupGet(x => x.OperatingTheatres).Returns([theatre.Object]);
		hospital.SetupGet(x => x.ActiveServiceRequests).Returns(Array.Empty<IHospitalServiceRequest>());
		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.ServiceType).Returns(HospitalServiceType.FullTreatment);
		service.SetupGet(x => x.PreferOperatingTheatre).Returns(false);
		var patient = Character(762, "Patient");
		patient.SetupGet(x => x.Location).Returns(waitingRoom.Object);
		var request = new Mock<IHospitalServiceRequest>();
		request.SetupGet(x => x.Id).Returns(763);
		request.SetupGet(x => x.Service).Returns(service.Object);
		request.SetupGet(x => x.Patient).Returns(patient.Object);
		request.SetupGet(x => x.PatientId).Returns(patient.Object.Id);
		request.SetupProperty(x => x.OperatingTheatreCellId);
		request.SetupProperty(x => x.UsedInPlaceFallback);

		var result = HospitalPatientFlow.TryReserveTreatmentLocation(hospital.Object, request.Object, out var location,
			out var reason);

		Assert.IsTrue(result, reason);
		Assert.AreSame(theatre.Object, location);
		Assert.AreEqual(theatre.Object.Id, request.Object.OperatingTheatreCellId);
	}

	[TestMethod]
	public void HospitalServiceActionStep_DoesNotReturnToSupplyRoomWhenCarriedBundleContainsTreatments()
	{
		var doctorCell = Cell(920, "hospital foyer");
		var patientCell = Cell(921, "hospital ward");
		var supplyRoom = Cell(922, "hospital supply room");
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.Id).Returns(923);
		hospital.SetupGet(x => x.Name).Returns("central clinic");
		hospital.SetupGet(x => x.FrameworkItemType).Returns("Hospital");
		hospital.SetupGet(x => x.IsTrading).Returns(true);
		hospital.SetupGet(x => x.SupplyRooms).Returns([supplyRoom.Object]);

		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.Id).Returns(924);
		service.SetupGet(x => x.Name).Returns("full treatment");
		service.SetupGet(x => x.ServiceType).Returns(HospitalServiceType.FullTreatment);
		service.SetupGet(x => x.RequiredEquipment).Returns([]);

		var patient = Character(925, "Patient");
		patient.SetupGet(x => x.Location).Returns(patientCell.Object);
		var request = new Mock<IHospitalServiceRequest>();
		request.SetupGet(x => x.Id).Returns(926);
		request.SetupGet(x => x.Hospital).Returns(hospital.Object);
		request.SetupGet(x => x.Service).Returns(service.Object);
		request.SetupGet(x => x.Status).Returns(HospitalServiceRequestStatus.Queued);
		request.SetupGet(x => x.Patient).Returns(patient.Object);

		var doctorBody = new Mock<MudSharp.Body.IBody>();
		doctorBody.SetupGet(x => x.ItemsInHands).Returns([]);
		var doctor = Character(927, "Doctor");
		doctor.SetupGet(x => x.Location).Returns(doctorCell.Object);
		doctor.SetupGet(x => x.Inventory).Returns([]);
		doctor.SetupGet(x => x.Body).Returns(doctorBody.Object);
		doctor.Setup(x => x.ColocatedWith(patient.Object)).Returns(false);

		var treatment = new Mock<ITreatment>();
		treatment.Setup(x => x.IsTreatmentType(It.IsAny<TreatmentType>())).Returns(true);
		var treatmentItem = Item(928, "universal treatment supply");
		treatmentItem.Setup(x => x.GetItemType<ITreatment>()).Returns(treatment.Object);
		var bundle = Item(929, "treatment supply bundle");
		bundle.SetupGet(x => x.DeepItems).Returns(() => [bundle.Object, treatmentItem.Object]);
		bundle.Setup(x => x.GetItemType<ITreatment>()).Returns((ITreatment)null!);

		var context = new Mock<IEmploymentTaskContext>();
		context.Setup(x => x.CarriedTaskItems(doctor.Object)).Returns([bundle.Object]);
		context.Setup(x => x.AvailableItems(supplyRoom.Object)).Returns([treatmentItem.Object]);

		var step = new HospitalServiceActionStep(hospital.Object, request.Object);

		var hints = step.ExecutionLocationHints(context.Object, doctor.Object);

		Assert.AreEqual(1, hints.Count);
		Assert.AreSame(patientCell.Object, hints.Single());
	}

	[TestMethod]
	public void HospitalServiceActionStep_CollectsSuppliesWhenAlreadyInSupplyRoom()
	{
		var patientCell = Cell(930, "hospital foyer");
		var supplyRoom = Cell(931, "hospital supply room");
		var theatre = Cell(932, "operating theatre");
		theatre.SetupGet(x => x.Characters).Returns([]);
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.Id).Returns(933);
		hospital.SetupGet(x => x.Name).Returns("central clinic");
		hospital.SetupGet(x => x.FrameworkItemType).Returns("Hospital");
		hospital.SetupGet(x => x.IsTrading).Returns(true);
		hospital.SetupGet(x => x.SupplyRooms).Returns([supplyRoom.Object]);
		hospital.SetupGet(x => x.OperatingTheatres).Returns([theatre.Object]);
		hospital.SetupGet(x => x.ActiveServiceRequests).Returns([]);
		hospital.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		        .Returns(true);

		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.Id).Returns(934);
		service.SetupGet(x => x.Name).Returns("full treatment");
		service.SetupGet(x => x.ServiceType).Returns(HospitalServiceType.FullTreatment);
		service.SetupGet(x => x.PreferOperatingTheatre).Returns(false);
		service.SetupGet(x => x.RequiredEquipment).Returns([]);

		var patient = Character(935, "Patient");
		patient.SetupGet(x => x.Location).Returns(patientCell.Object);
		var request = new Mock<IHospitalServiceRequest>();
		request.SetupGet(x => x.Id).Returns(936);
		request.SetupGet(x => x.Hospital).Returns(hospital.Object);
		request.SetupGet(x => x.Service).Returns(service.Object);
		request.SetupGet(x => x.Status).Returns(HospitalServiceRequestStatus.Queued);
		request.SetupGet(x => x.Patient).Returns(patient.Object);
		request.SetupProperty(x => x.OperatingTheatreCellId);
		request.SetupProperty(x => x.UsedInPlaceFallback);

		var doctorBody = new Mock<MudSharp.Body.IBody>();
		doctorBody.SetupGet(x => x.ItemsInHands).Returns([]);
		var doctor = Character(937, "Doctor");
		doctor.SetupGet(x => x.Location).Returns(supplyRoom.Object);
		doctor.SetupGet(x => x.Inventory).Returns([]);
		doctor.SetupGet(x => x.Body).Returns(doctorBody.Object);
		doctor.Setup(x => x.ColocatedWith(patient.Object)).Returns(false);

		var treatment = new Mock<ITreatment>();
		treatment.Setup(x => x.IsTreatmentType(It.IsAny<TreatmentType>())).Returns(true);
		var treatmentItem = Item(938, "universal treatment supply");
		treatmentItem.Setup(x => x.GetItemType<ITreatment>()).Returns(treatment.Object);
		supplyRoom.SetupGet(x => x.GameItems).Returns([treatmentItem.Object]);

		var context = new Mock<IEmploymentTaskContext>();
		context.SetupGet(x => x.Employer).Returns(hospital.Object);
		context.Setup(x => x.CarriedTaskItems(doctor.Object)).Returns([]);
		context.Setup(x => x.AvailableItems(theatre.Object)).Returns([]);
		context.Setup(x => x.AvailableItems(supplyRoom.Object)).Returns([treatmentItem.Object]);
		context.Setup(x => x.CanPath(doctor.Object, It.IsAny<ICell>())).Returns(true);
		var collected = false;
		var collectReason = string.Empty;
		context.Setup(x => x.TryCollectTaskItems(
			       doctor.Object,
			       It.IsAny<IReadOnlyCollection<(IGameItem Item, ICell Source)>>(),
			       out collectReason))
		       .Returns(new TryCollectTaskItemsCallback((ICharacter _, IReadOnlyCollection<(IGameItem Item, ICell Source)> items, out string reason) =>
		       {
			       collected = true;
			       Assert.AreEqual(1, items.Count);
			       Assert.AreSame(treatmentItem.Object, items.Single().Item);
			       Assert.AreSame(supplyRoom.Object, items.Single().Source);
			       reason = string.Empty;
			       return true;
		       }));

		var step = new HospitalServiceActionStep(hospital.Object, request.Object);

		var result = step.Execute(context.Object, doctor.Object);

		Assert.IsTrue(result.Success, result.Message);
		Assert.IsTrue(collected);
		Assert.IsFalse(result.Completed);
		StringAssert.Contains(result.Message, "Collected treatment supplies");
	}

	[TestMethod]
	public void HospitalMedicalServiceRunner_FullTreatmentDoesNotRepeatCompletedTendingPhase()
	{
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.Id).Returns(764);
		hospital.SetupGet(x => x.Name).Returns("central clinic");
		hospital.SetupGet(x => x.Services).Returns(Array.Empty<IHospitalService>());
		hospital.SetupGet(x => x.EmploymentRegister).Returns(new Mock<IEmploymentRegister>().Object);
		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.Id).Returns(765);
		service.SetupGet(x => x.Name).Returns("Full Treatment");
		service.SetupGet(x => x.ServiceType).Returns(HospitalServiceType.FullTreatment);
		service.SetupGet(x => x.RequiredEquipment).Returns(Array.Empty<HospitalServiceEquipmentRequirement>());
		service.SetupGet(x => x.RequiresRecovery).Returns(false);
		var employee = Character(766, "Doctor");
		var patient = Character(767, "Patient");
		employee.Setup(x => x.ColocatedWith(patient.Object)).Returns(true);
		var body = new Mock<MudSharp.Body.IBody>();
		body.SetupGet(x => x.TotalBloodVolumeLitres).Returns(5.0);
		body.SetupGet(x => x.CurrentBloodVolumeLitres).Returns(5.0);
		patient.SetupGet(x => x.Body).Returns(body.Object);
		var wound = new Mock<IWound>();
		wound.SetupGet(x => x.Severity).Returns(WoundSeverity.Moderate);
		wound.Setup(x => x.CanBeTreated(TreatmentType.AntiInflammatory)).Returns(Difficulty.Normal);
		wound.Setup(x => x.CanBeTreated(It.Is<TreatmentType>(type => type != TreatmentType.AntiInflammatory)))
		     .Returns(Difficulty.Impossible);
		patient.SetupGet(x => x.Wounds).Returns(Array.Empty<IWound>());
		patient.Setup(x => x.VisibleWounds(It.IsAny<IPerceiver>(), WoundExaminationType.Examination))
		       .Returns([wound.Object]);
		var request = new Mock<IHospitalServiceRequest>();
		request.SetupGet(x => x.Id).Returns(768);
		request.SetupGet(x => x.Hospital).Returns(hospital.Object);
		request.SetupGet(x => x.Service).Returns(service.Object);
		request.SetupGet(x => x.Status).Returns(HospitalServiceRequestStatus.InProgress);
		request.SetupGet(x => x.PaymentMethod).Returns(HospitalPaymentMethod.Waived);
		request.SetupGet(x => x.PatientId).Returns(patient.Object.Id);
		request.SetupGet(x => x.Patient).Returns(patient.Object);
		var task = new EmploymentActiveTask(hospital.Object, "Treat patient",
			new EmploymentActionPlan([new HospitalServiceActionStep(hospital.Object, request.Object)]), Guid.NewGuid());
		task.Assign(employee.Object);
		task.MarkStep(0, EmploymentActionStepStatus.InProgress,
			new EmploymentActionStepOperationalState(
				OperationalPayload:
				"hospitalservice;hospital=764;request=768;type=FullTreatment;completed=False;done=tend"));
		var context = new EmploymentTaskContext(hospital.Object);
		context.HydrateTaskState(task, 0);

		var result = HospitalMedicalServiceRunner.ExecuteServiceRequest(context, employee.Object, hospital.Object,
			request.Object);

		Assert.IsTrue(result.Success, result.Message);
		Assert.IsTrue(result.Completed);
		wound.Verify(x => x.Treat(
			It.IsAny<IPerceiver>(),
			It.IsAny<TreatmentType>(),
			It.IsAny<ITreatment>(),
			It.IsAny<Outcome>(),
			It.IsAny<bool>()), Times.Never);
		request.Verify(x => x.MarkStatus(HospitalServiceRequestStatus.Completed,
			It.Is<string>(text => text.Contains("Full Treatment"))), Times.Once);
	}

	[TestMethod]
	public void HospitalPatientFlow_RecordsAuditNoteWhenRecoveryRoomIsMissing()
	{
		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.RequiresRecovery).Returns(true);
		var patient = Character(4, "Patient");
		patient.SetupGet(x => x.IsHelpless).Returns(true);
		var request = new Mock<IHospitalServiceRequest>();
		request.SetupGet(x => x.Service).Returns(service.Object);
		request.SetupGet(x => x.Patient).Returns(patient.Object);
		request.SetupGet(x => x.Status).Returns(HospitalServiceRequestStatus.Completed);
		var notes = new List<string>();
		request.Setup(x => x.MarkStatus(It.IsAny<HospitalServiceRequestStatus>(), It.IsAny<string>()))
		       .Callback<HospitalServiceRequestStatus, string>((_, note) => notes.Add(note));
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.RecoveryRooms).Returns(Array.Empty<ICell>());
		hospital.SetupGet(x => x.WaitingRooms).Returns(Array.Empty<ICell>());

		HospitalPatientFlow.TransferAfterTreatment(hospital.Object, request.Object, null, "Hospital recovery routing");

		Assert.AreEqual(1, notes.Count);
		StringAssert.Contains(notes.Single(), "no recovery room");
	}

	[TestMethod]
	public void HospitalPatientFlow_MovesHelplessPatientToRecoveryRoom()
	{
		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.RequiresRecovery).Returns(true);
		var origin = Cell(710, "operating theatre");
		var recovery = Cell(711, "recovery room");
		recovery.SetupGet(x => x.GameItems).Returns(Array.Empty<IGameItem>());
		var patient = Character(5, "Patient");
		patient.SetupGet(x => x.IsHelpless).Returns(true);
		patient.SetupGet(x => x.Location).Returns(origin.Object);
		patient.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		var request = new Mock<IHospitalServiceRequest>();
		request.SetupGet(x => x.Service).Returns(service.Object);
		request.SetupGet(x => x.Patient).Returns(patient.Object);
		request.SetupProperty(x => x.RecoveryRoomCellId);
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.RecoveryRooms).Returns([recovery.Object]);
		hospital.SetupGet(x => x.WaitingRooms).Returns(Array.Empty<ICell>());

		HospitalPatientFlow.TransferAfterTreatment(hospital.Object, request.Object, null, "Hospital recovery routing");

		Assert.AreEqual(recovery.Object.Id, request.Object.RecoveryRoomCellId);
		origin.Verify(x => x.Leave(patient.Object), Times.Once);
		recovery.Verify(x => x.Enter(patient.Object, null, true, RoomLayer.GroundLevel), Times.Once);
	}

	[TestMethod]
	public void HospitalServiceActionStep_StaysWithPatientBeforeTheatreTransfer()
	{
		var waitingRoom = Cell(720, "waiting room");
		var theatre = Cell(721, "operating theatre");
		theatre.SetupGet(x => x.Characters).Returns(Array.Empty<ICharacter>());
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.Id).Returns(722);
		hospital.SetupGet(x => x.Name).Returns("central clinic");
		hospital.SetupGet(x => x.OperatingTheatres).Returns([theatre.Object]);
		hospital.SetupGet(x => x.ActiveServiceRequests).Returns(Array.Empty<IHospitalServiceRequest>());
		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.PreferOperatingTheatre).Returns(true);
		var patient = Character(723, "Patient");
		patient.SetupGet(x => x.Location).Returns(waitingRoom.Object);
		var request = new Mock<IHospitalServiceRequest>();
		request.SetupGet(x => x.Id).Returns(724);
		request.SetupGet(x => x.Hospital).Returns(hospital.Object);
		request.SetupGet(x => x.Service).Returns(service.Object);
		request.SetupGet(x => x.Patient).Returns(patient.Object);
		request.SetupProperty(x => x.OperatingTheatreCellId);
		request.SetupProperty(x => x.UsedInPlaceFallback);
		var doctor = Character(725, "Doctor");
		doctor.SetupGet(x => x.Location).Returns(waitingRoom.Object);
		doctor.Setup(x => x.ColocatedWith(patient.Object)).Returns(true);
		var step = new HospitalServiceActionStep(hospital.Object, request.Object);

		var hints = step.ExecutionLocationHints(new EmploymentTaskContext(hospital.Object), doctor.Object);

		Assert.AreEqual(1, hints.Count);
		Assert.AreSame(waitingRoom.Object, hints.Single());
		Assert.IsNull(request.Object.OperatingTheatreCellId);
	}

	[TestMethod]
	public void HospitalPatientPreparation_MovesPatientAndDoctorToReservedTheatre()
	{
		var waitingRoom = Cell(746, "waiting room");
		var theatre = Cell(747, "operating theatre");
		theatre.SetupGet(x => x.Characters).Returns(Array.Empty<ICharacter>());
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.Id).Returns(748);
		hospital.SetupGet(x => x.Name).Returns("central clinic");
		hospital.SetupGet(x => x.FrameworkItemType).Returns("Hospital");
		hospital.SetupGet(x => x.IsTrading).Returns(true);
		hospital.SetupGet(x => x.OperatingTheatres).Returns([theatre.Object]);
		hospital.SetupGet(x => x.SupplyRooms).Returns(Array.Empty<ICell>());
		hospital.SetupGet(x => x.EmploymentRegister).Returns(new Mock<IEmploymentRegister>().Object);
		hospital.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), EmploymentAuthority.PerformMedicalServices))
		        .Returns(true);
		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.Id).Returns(749);
		service.SetupGet(x => x.Name).Returns("Stabilisation");
		service.SetupGet(x => x.ServiceType).Returns(HospitalServiceType.Stabilisation);
		service.SetupGet(x => x.RequiredEquipment).Returns([]);
		service.SetupGet(x => x.PreferOperatingTheatre).Returns(false);
		var patient = Character(750, "Patient");
		patient.SetupGet(x => x.Location).Returns(waitingRoom.Object);
		patient.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		var doctor = Character(751, "Doctor");
		doctor.SetupGet(x => x.Location).Returns(waitingRoom.Object);
		doctor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		doctor.Setup(x => x.ColocatedWith(patient.Object)).Returns(true);
		var request = new Mock<IHospitalServiceRequest>();
		request.SetupGet(x => x.Id).Returns(752);
		request.SetupGet(x => x.Hospital).Returns(hospital.Object);
		request.SetupGet(x => x.Service).Returns(service.Object);
		request.SetupGet(x => x.Patient).Returns(patient.Object);
		request.SetupGet(x => x.PatientId).Returns(patient.Object.Id);
		request.SetupGet(x => x.Status).Returns(HospitalServiceRequestStatus.Queued);
		request.SetupProperty(x => x.OperatingTheatreCellId);
		request.SetupProperty(x => x.ReturnCellId);
		request.SetupProperty(x => x.UsedInPlaceFallback);
		hospital.SetupGet(x => x.ActiveServiceRequests).Returns([request.Object]);
		hospital.Setup(x => x.IsEmployee(It.IsAny<ICharacter>()))
		        .Returns<ICharacter>(actor => actor.Id == doctor.Object.Id);
		var step = new HospitalPatientPreparationActionStep(hospital.Object, request.Object);

		var result = step.Execute(new EmploymentTaskContext(hospital.Object), doctor.Object);

		Assert.IsTrue(result.Success);
		Assert.IsTrue(result.Completed);
		Assert.AreEqual(theatre.Object.Id, request.Object.OperatingTheatreCellId);
		Assert.AreEqual(waitingRoom.Object.Id, request.Object.ReturnCellId);
		waitingRoom.Verify(x => x.Leave(patient.Object), Times.Once);
		waitingRoom.Verify(x => x.Leave(doctor.Object), Times.Once);
		theatre.Verify(x => x.Enter(patient.Object, null, true, RoomLayer.GroundLevel), Times.Once);
		theatre.Verify(x => x.Enter(doctor.Object, null, true, RoomLayer.GroundLevel), Times.Once);
		request.Verify(x => x.MarkStatus(HospitalServiceRequestStatus.Assigned,
			It.Is<string>(message => message.Contains("moved") && message.Contains("operating theatre"))), Times.Once);
	}

	[TestMethod]
	public void HospitalServiceActionStep_FailedRequestFailsActiveTask()
	{
		var waitingRoom = Cell(729, "waiting room");
		var theatre = Cell(728, "operating theatre");
		theatre.SetupGet(x => x.Characters).Returns(Array.Empty<ICharacter>());
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.Id).Returns(730);
		hospital.SetupGet(x => x.Name).Returns("central clinic");
		hospital.SetupGet(x => x.FrameworkItemType).Returns("Hospital");
		hospital.SetupGet(x => x.IsTrading).Returns(true);
		hospital.SetupGet(x => x.OperatingTheatres).Returns([theatre.Object]);
		hospital.SetupGet(x => x.ActiveServiceRequests).Returns(Array.Empty<IHospitalServiceRequest>());
		hospital.SetupGet(x => x.SupplyRooms).Returns(Array.Empty<ICell>());
		hospital.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), EmploymentAuthority.PerformMedicalServices))
		        .Returns(true);
		hospital.SetupGet(x => x.EmploymentRegister).Returns(new Mock<IEmploymentRegister>().Object);
		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.Id).Returns(731);
		service.SetupGet(x => x.Name).Returns("Full Treatment");
		service.SetupGet(x => x.ServiceType).Returns(HospitalServiceType.FullTreatment);
		service.SetupGet(x => x.RequiredEquipment).Returns(Array.Empty<HospitalServiceEquipmentRequirement>());
		service.SetupGet(x => x.PreferOperatingTheatre).Returns(false);
		var doctor = Character(732, "Doctor");
		var patient = Character(733, "Patient");
		doctor.SetupGet(x => x.Location).Returns(waitingRoom.Object);
		patient.SetupGet(x => x.Location).Returns(waitingRoom.Object);
		doctor.Setup(x => x.ColocatedWith(patient.Object)).Returns(true);
		var body = new Mock<MudSharp.Body.IBody>();
		body.SetupGet(x => x.TotalBloodVolumeLitres).Returns(5.0);
		body.SetupGet(x => x.CurrentBloodVolumeLitres).Returns(5.0);
		patient.SetupGet(x => x.Body).Returns(body.Object);
		patient.SetupGet(x => x.Wounds).Returns(Array.Empty<IWound>());
		patient.Setup(x => x.VisibleWounds(It.IsAny<IPerceiver>(), WoundExaminationType.Examination))
		       .Returns(Array.Empty<IWound>());
		var request = new Mock<IHospitalServiceRequest>();
		request.SetupGet(x => x.Id).Returns(734);
		request.SetupGet(x => x.Hospital).Returns(hospital.Object);
		request.SetupGet(x => x.Service).Returns(service.Object);
		request.SetupGet(x => x.Status).Returns(HospitalServiceRequestStatus.Queued);
		request.SetupGet(x => x.PatientId).Returns(patient.Object.Id);
		request.SetupGet(x => x.Patient).Returns(patient.Object);
		var task = new EmploymentActiveTask(hospital.Object, "Treat patient",
			new EmploymentActionPlan([new HospitalServiceActionStep(hospital.Object, request.Object)]),
			Guid.NewGuid());
		task.Assign(doctor.Object);

		var result = new EmploymentTaskDispatcher().AdvanceTask(task, new EmploymentTaskContext(hospital.Object));

		Assert.IsFalse(result.Success);
		Assert.IsTrue(result.Completed);
		Assert.AreEqual(EmploymentTaskStatus.Failed, task.Status);
		Assert.AreEqual(EmploymentActionStepStatus.Failed, task.StepStates[0]);
		request.Verify(x => x.MarkStatus(HospitalServiceRequestStatus.Failed,
			It.Is<string>(text => text.Contains("no visible wounds"))), Times.Once);
	}

	[TestMethod]
	public void EmploymentWorkerAI_PrimaryWorkCellPrefersHospitalStaffRoomThenWaitingRoom()
	{
		var staffRoom = Cell(712, "staff room");
		var waitingRoom = Cell(713, "waiting room");
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.StaffRooms).Returns([staffRoom.Object]);
		hospital.SetupGet(x => x.WaitingRooms).Returns([waitingRoom.Object]);
		hospital.SetupGet(x => x.OperatingTheatres).Returns(Array.Empty<ICell>());
		hospital.SetupGet(x => x.SupplyRooms).Returns(Array.Empty<ICell>());
		var method = typeof(EmploymentWorkerAI).GetMethod("PrimaryWorkCell",
			BindingFlags.Static | BindingFlags.NonPublic)!;

		Assert.AreSame(staffRoom.Object, method.Invoke(null, [hospital.Object]));

		hospital.SetupGet(x => x.StaffRooms).Returns(Array.Empty<ICell>());

		Assert.AreSame(waitingRoom.Object, method.Invoke(null, [hospital.Object]));
	}

	[TestMethod]
	public void EmploymentWorkerAI_CanReachTreatsSameCellIdAsCurrentLocation()
	{
		var current = Cell(753, "operating theatre");
		var rehydrated = Cell(753, "operating theatre copy");
		var worker = Character(754, "Orderly");
		worker.SetupGet(x => x.Location).Returns(current.Object);
		var ai = EmploymentWorkerAi();
		var method = typeof(EmploymentWorkerAI).GetMethod("CanReach",
			BindingFlags.Instance | BindingFlags.NonPublic)!;

		Assert.IsTrue((bool)method.Invoke(ai, [worker.Object, rehydrated.Object])!);
	}

	[TestMethod]
	public void ClanHallCellEntity_MapsToClanCellJoinTable()
	{
		using var context = BuildContext();

		var entityType = context.Model.FindEntityType(typeof(MudSharp.Models.ClanHallCell));

		Assert.IsNotNull(entityType);
		Assert.AreEqual("Clans_HallCells", entityType!.GetTableName());
		CollectionAssert.AreEqual(
			new[] { "ClanId", "CellId" },
			entityType.FindPrimaryKey()!.Properties.Select(x => x.Name).ToArray());
		Assert.IsNotNull(entityType.FindNavigation(nameof(MudSharp.Models.ClanHallCell.Clan)));
		Assert.IsNotNull(entityType.FindNavigation(nameof(MudSharp.Models.ClanHallCell.Cell)));
	}

	[TestMethod]
	public void EmploymentHostContract_DoesNotDependOnExistingPcJobSystem()
	{
		var forbidden = new[] { typeof(IJobListing), typeof(IActiveJob) };
		var exposedTypes = typeof(IEmploymentHost)
		                   .GetProperties()
		                   .Select(x => x.PropertyType)
		                   .Concat(typeof(IEmploymentHost)
		                           .GetMethods()
		                           .SelectMany(x => x.GetParameters().Select(y => y.ParameterType).Append(x.ReturnType)))
		                   .ToList();

		foreach (var type in exposedTypes)
		foreach (var forbiddenType in forbidden)
		{
			Assert.IsFalse(ContainsType(type, forbiddenType), $"{type} unexpectedly exposes {forbiddenType}.");
		}
	}

	[TestMethod]
	public void HireFire_UsesSimpleEmploymentStatusAndRecordsRegisterEntries()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("market shop", currency.Object);
		var manager = Character(1, "Manager").Object;
		var employee = Character(2, "Employee").Object;

		Assert.IsFalse(Enum.GetNames<EmploymentStatus>().Contains("Probation"));

		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.HireEmployees | EmploymentAuthority.FireEmployees), null);
		var contract = host.Hire(employee, Offer(currency.Object, EmploymentRole.Employee), manager);

		Assert.AreEqual(EmploymentStatus.Active, contract.Status);
		host.Fire(contract, EmploymentTerminationReason.Fired, manager);
		Assert.AreEqual(EmploymentStatus.Ended, contract.Status);
		Assert.AreEqual(EmploymentTerminationReason.Fired, contract.EndReason);
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ContractHired));
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ContractEnded));
	}

	[TestMethod]
	public void ManagerAuthority_ChecksDelegatedAuthorityForOpeningsAndGoals()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("bank", currency.Object);
		var clerk = Character(1, "Clerk").Object;
		var manager = Character(2, "Manager").Object;

		host.Hire(clerk, Offer(currency.Object, EmploymentRole.Employee), null);
		Assert.ThrowsException<InvalidOperationException>(() =>
			host.Employment.CreateJobOpening(Opening(currency.Object), clerk));

		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateJobOpenings | EmploymentAuthority.CreateManagerGoals), null);
		var opening = host.Employment.CreateJobOpening(Opening(currency.Object), manager);
		Assert.AreEqual(JobOpeningStatus.Open, opening.Status);

		var goal = new ManagerGoalDefinition(
			ManagerGoalType.MaintainMinimumStock,
			EmploymentAuthority.PostToHostBoard,
			new ManagerGoalConfiguration("post restock note", new EmploymentActionPlan([
				new BoardPostActionStep("Stock", "Please restock.")
			])),
			1,
			TimeSpan.FromMinutes(10));
		Assert.ThrowsException<InvalidOperationException>(() => host.ManagerGoalBoard.CreateGoal(goal, manager));
	}

	[TestMethod]
	public void JobOpeningCandidateMatching_FiltersRequirementsReservationWageAndPaymentMethods()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(1, "Manager").Object;
		var applicant = Character(2, "Applicant").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthority.CreateJobOpenings), null);
		var opening = host.Employment.CreateJobOpening(Opening(currency.Object), manager);

		var underpaid = Profile(applicant, 20.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanPurchaseCommodities), skills: new Dictionary<string, double> { ["haggling"] = 50.0 });
		var rejected = host.Employment.Apply(opening, underpaid);

		Assert.AreEqual(JobApplicationStatus.Rejected, rejected.Status);
		Assert.IsTrue(rejected.DecisionReason?.Contains("reservation wage") == true);

		var qualified = Profile(applicant, 5.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanPurchaseCommodities), skills: new Dictionary<string, double> { ["haggling"] = 50.0 });
		var acceptedForReview = host.Employment.Apply(opening, qualified);

		Assert.AreEqual(JobApplicationStatus.Pending, acceptedForReview.Status);
		var selection = EmploymentPaymentSelector.SelectPaymentMethod(opening, qualified);
		Assert.IsTrue(selection.Success);
		Assert.AreEqual(PaymentMethodKind.Cash, selection.PaymentMethod?.MethodKind);
	}

	[TestMethod]
	public void EmploymentLegacyBridgeReporter_ReportsReadOnlyLegacyAndNewDivergence()
	{
		var currency = Currency();
		var legacyWorker = Character(30, "Legacy Worker").Object;
		var newWorker = Character(31, "New Worker").Object;
		var manager = Character(32, "Manager").Object;
		var (shop, state) = ShopHost(30, "bridge shop", currency.Object, null);
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager), null);
		state.Hire(newWorker, Offer(currency.Object, EmploymentRole.Employee), null);
		var legacyListing = new Mock<IJobListing>();
		var legacyJob = new Mock<IActiveJob>();
		legacyListing.SetupGet(x => x.Id).Returns(30);
		legacyListing.SetupGet(x => x.Name).Returns("legacy shop job");
		legacyListing.SetupGet(x => x.Employer).Returns(shop.Object);
		legacyListing.SetupGet(x => x.ActiveJobs).Returns([legacyJob.Object]);
		legacyJob.SetupGet(x => x.Id).Returns(30);
		legacyJob.SetupGet(x => x.Name).Returns("legacy shop job");
		legacyJob.SetupGet(x => x.Listing).Returns(legacyListing.Object);
		legacyJob.SetupGet(x => x.Character).Returns(legacyWorker);
		legacyJob.SetupGet(x => x.IsJobComplete).Returns(false);
		var jobListings = new All<IJobListing>();
		jobListings.Add(legacyListing.Object);
		var activeJobs = new All<IActiveJob>();
		activeJobs.Add(legacyJob.Object);
		var gameworld = Gameworld(
			currency.Object,
			new Dictionary<long, ICharacter> { [legacyWorker.Id] = legacyWorker, [newWorker.Id] = newWorker, [manager.Id] = manager },
			shopsToAdd: [shop.Object]);
		gameworld.SetupGet(x => x.JobListings).Returns(jobListings);
		gameworld.SetupGet(x => x.ActiveJobs).Returns(activeJobs);

		var report = EmploymentLegacyBridgeReporter.Build(gameworld.Object);

		Assert.AreEqual(1, report.EmploymentHostCount);
		Assert.AreEqual(2, report.ActiveEmploymentContractCount);
		Assert.AreEqual(1, report.LegacyJobListingCount);
		Assert.AreEqual(1, report.ActiveLegacyJobCount);
		Assert.IsTrue(report.Divergences.Any(x =>
			x.Kind == EmploymentLegacyBridgeDivergenceKind.LegacyActiveJobWithoutContract &&
			x.Character.Contains("Legacy Worker")));
		Assert.IsTrue(report.Divergences.Any(x =>
			x.Kind == EmploymentLegacyBridgeDivergenceKind.EmploymentContractWithoutLegacyJob &&
			x.Character.Contains("New Worker")));
		Assert.AreEqual(1, activeJobs.Count());
		Assert.AreEqual(2, state.EmploymentContracts.Count);
	}

	[TestMethod]
	public void PaymentSelection_PrefersBankAccountFallsBackToCashAndCanFail()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var opening = new JobOpening(1, host, EmploymentRole.Employee, JobRequirementSet.None, Pay(currency.Object),
			WorkSchedule.AnyTime, EmploymentDuration.Indefinite, JobOpeningStatus.Open, 1, true,
			new PaymentMethod(PaymentMethodKind.EmployeeBankAccount), EmploymentAuthoritySet.Empty);
		var applicant = Character(1, "Applicant").Object;
		var bankAccount = new Mock<IBankAccount>();

		var bankProfile = Profile(applicant, 1.0M, PaymentMethodKind.EmployeeBankAccount, Caps());
		var bank = EmploymentPaymentSelector.SelectPaymentMethod(opening, bankProfile, bankAccount.Object);
		Assert.IsTrue(bank.Success);
		Assert.AreSame(bankAccount.Object, bank.PaymentMethod?.BankAccount);

		var cashProfile = Profile(applicant, 1.0M, PaymentMethodKind.Cash, Caps());
		var cash = EmploymentPaymentSelector.SelectPaymentMethod(opening, cashProfile);
		Assert.IsTrue(cash.Success);
		Assert.AreEqual(PaymentMethodKind.Cash, cash.PaymentMethod?.MethodKind);

		var failureProfile = Profile(applicant, 1.0M, PaymentMethodKind.PaymentItem, Caps());
		var failure = EmploymentPaymentSelector.SelectPaymentMethod(opening, failureProfile);
		Assert.IsFalse(failure.Success);
	}

	[TestMethod]
	public void Payroll_AccruesOverdueDaysAndSettlementClearsEmployerReputationAfterContractEnds()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var (shop, state) = ShopHost(401, "stable", currency.Object, null);
		IEmploymentHost host = shop.Object;
		var employee = Character(10, "Employee").Object;
		var compensation = new CompensationTerms(
			new MoneyAmount(currency.Object, 10.0M),
			null,
			PayCadence.Daily,
			new MoneyAmount(currency.Object, 10.0M),
			PaymentSource.HostCash);
		var contract = state.Hire(employee, new EmploymentOffer(
			EmploymentRole.Employee,
			compensation,
			WorkSchedule.AnyTime,
			EmploymentDuration.Indefinite,
			new PaymentMethod(PaymentMethodKind.Cash),
			EmploymentAuthoritySet.Empty), null);
		var evaluationTime = contract.StartedAt.AddDays(10).AddHours(1);

		var created = host.Payroll.EvaluatePayroll(evaluationTime);

		Assert.AreEqual(10, created.Count);
		Assert.AreEqual(9, host.Payroll.MaximumOverdueDays(evaluationTime));
		VirtualCashLedger.Credit(shop.Object, currency.Object, created.Sum(x => x.Amount.Amount), null, shop.Object,
			"Seed", "Seed payroll balance");
		host.Fire(contract, EmploymentTerminationReason.Resigned, null);
		Assert.IsTrue(host.Payroll.TrySettlePayables(host.Payroll.OutstandingLiabilities, null, false,
			"Settled after resignation.", out var message), message);
		Assert.AreEqual(0, host.Payroll.MaximumOverdueDays(evaluationTime));
		Assert.IsTrue(host.Payroll.Payables.All(x => x.Status == EmploymentPayableStatus.Settled));
		Assert.IsTrue(host.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.Wage));
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.WageSettled));
		Assert.AreEqual(0.0M, VirtualCashLedger.Balance(shop.Object, currency.Object));
	}

	[TestMethod]
	public void Payroll_HourlyScheduledShiftAccruesScheduledHoursRatherThanElapsedContractHours()
	{
		var currency = Currency();
		var (shop, state) = ShopHost(407, "scheduled shop", currency.Object, null);
		IEmploymentHost host = shop.Object;
		var employee = Character(11, "Employee").Object;
		var compensation = new CompensationTerms(
			new MoneyAmount(currency.Object, 10.0M),
			null,
			PayCadence.Hourly,
			new MoneyAmount(currency.Object, 10.0M),
			PaymentSource.HostCash);
		var contract = state.Hire(employee, new EmploymentOffer(
			EmploymentRole.Employee,
			compensation,
			new WorkSchedule("Day shift", TimeSpan.FromHours(9.0), TimeSpan.FromHours(17.0)),
			EmploymentDuration.Indefinite,
			new PaymentMethod(PaymentMethodKind.Cash),
			EmploymentAuthoritySet.Empty), null);

		var created = host.Payroll.EvaluatePayroll(contract.StartedAt.AddDays(1.0).AddMinutes(1.0));

		Assert.AreEqual(1, created.Count);
		var payable = created.Single();
		Assert.AreEqual(80.0M, payable.Amount.Amount);
		Assert.AreEqual(PayCadence.Hourly, payable.Cadence);
		Assert.AreEqual(TimeSpan.FromDays(1.0), payable.PayPeriodEnd - payable.PayPeriodStart);
	}

	[TestMethod]
	public void Payroll_RejectsUnsupportedCadencesWithoutExplicitEarnings()
	{
		var currency = Currency();
		foreach (var cadence in new[] { PayCadence.PerTask, PayCadence.Commission, PayCadence.Mixed })
		{
			var (_, state) = ShopHost(450 + (int)cadence, $"unsupported {cadence}", currency.Object, null);
			var employee = Character(80 + (int)cadence, "Employee").Object;
			var compensation = new CompensationTerms(
				new MoneyAmount(currency.Object, 10.0M),
				null,
				cadence,
				new MoneyAmount(currency.Object, 10.0M),
				PaymentSource.HostCash);

			var contractException = Assert.ThrowsException<InvalidOperationException>(() => state.Hire(employee, new EmploymentOffer(
				EmploymentRole.Employee,
				compensation,
				WorkSchedule.AnyTime,
				EmploymentDuration.Indefinite,
				new PaymentMethod(PaymentMethodKind.Cash),
				EmploymentAuthoritySet.Empty), null));
			StringAssert.Contains(contractException.Message, "explicit earning records");

			var openingException = Assert.ThrowsException<InvalidOperationException>(() => state.CreateJobOpening(
				new JobOpeningDefinition(
					EmploymentRole.Employee,
					JobRequirementSet.None,
					compensation,
					WorkSchedule.AnyTime,
					EmploymentDuration.Indefinite,
					1,
					true,
					new PaymentMethod(PaymentMethodKind.Cash),
					EmploymentAuthoritySet.Empty), null));
			StringAssert.Contains(openingException.Message, "explicit earning records");
		}
	}

	[TestMethod]
	public void Payroll_EvaluateExpiresFixedTermAndAccruesFinalPartialPay()
	{
		var currency = Currency();
		var (shop, state) = ShopHost(408, "fixed term shop", currency.Object, null);
		IEmploymentHost host = shop.Object;
		var employee = Character(12, "Employee").Object;
		var compensation = new CompensationTerms(
			new MoneyAmount(currency.Object, 10.0M),
			null,
			PayCadence.Daily,
			new MoneyAmount(currency.Object, 10.0M),
			PaymentSource.HostCash);
		var contract = state.Hire(employee, new EmploymentOffer(
			EmploymentRole.Employee,
			compensation,
			WorkSchedule.AnyTime,
			new EmploymentDuration(EmploymentDurationType.FixedTerm, TimeSpan.FromHours(12.0)),
			new PaymentMethod(PaymentMethodKind.Cash),
			EmploymentAuthoritySet.Empty), null);

		var created = host.Payroll.EvaluatePayroll(contract.StartedAt.AddDays(1.0));

		Assert.AreEqual(EmploymentStatus.Ended, contract.Status);
		Assert.AreEqual(EmploymentTerminationReason.Expired, contract.EndReason);
		Assert.AreEqual(contract.StartedAt.AddHours(12.0), contract.EndsAt);
		Assert.AreEqual(1, created.Count);
		var payable = created.Single();
		Assert.AreEqual(5.0M, payable.Amount.Amount);
		Assert.AreEqual(contract.EndsAt, payable.PayPeriodEnd);
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.ContractEnded));
	}

	[TestMethod]
	public void EmploymentHostOperationsScheduler_EvaluatesRulesGoalsPayrollAndLifecycle()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("operations shop", currency.Object);
		var manager = Character(1, "Manager").Object;
		var employee = Character(2, "Employee").Object;
		host.Hire(manager, new EmploymentOffer(
			EmploymentRole.Manager,
			new CompensationTerms(null, null, PayCadence.Unpaid, null, PaymentSource.HostCash),
			WorkSchedule.AnyTime,
			EmploymentDuration.Indefinite,
			new PaymentMethod(PaymentMethodKind.Cash),
			EmploymentAuthority.CreateScheduledRules |
			EmploymentAuthority.CreateManagerGoals |
			EmploymentAuthority.AssignTasks), null);
		var compensation = new CompensationTerms(
			new MoneyAmount(currency.Object, 10.0M),
			null,
			PayCadence.Daily,
			new MoneyAmount(currency.Object, 10.0M),
			PaymentSource.HostCash);
		var contract = host.Hire(employee, new EmploymentOffer(
			EmploymentRole.Employee,
			compensation,
			WorkSchedule.AnyTime,
			new EmploymentDuration(EmploymentDurationType.FixedTerm, TimeSpan.FromHours(12.0)),
			new PaymentMethod(PaymentMethodKind.Cash),
			EmploymentAuthoritySet.Empty), null);
		host.TaskBoard.CreateScheduledRule(
			"scheduled report",
			"scheduled-report",
			[],
			new EmploymentActionPlan([new CataloguedActionShellStep("report", "scheduled work")]),
			TimeSpan.FromHours(1.0),
			manager);
		host.ManagerGoalBoard.CreateGoal(new ManagerGoalDefinition(
			ManagerGoalType.MaintainHotelOperations,
			EmploymentAuthority.AssignTasks,
			new ManagerGoalConfiguration("goal report", new EmploymentActionPlan([
				new CataloguedActionShellStep("report", "goal work")
			])),
			5,
			TimeSpan.Zero), manager);

		var result = EmploymentHostOperationsScheduler.EvaluateHost(host, contract.StartedAt.AddDays(1.0));

		Assert.AreEqual(1, result.ScheduledRuleTaskCount);
		Assert.AreEqual(1, result.ManagerGoalTaskCount);
		Assert.AreEqual(1, result.PayableCount);
		Assert.AreEqual(1, result.ContractEndCount);
		Assert.AreEqual(EmploymentStatus.Ended, contract.Status);
		Assert.AreEqual(EmploymentTerminationReason.Expired, contract.EndReason);
		Assert.IsTrue(result.ScheduledRuleTasks.Single().SourceKind == EmploymentTaskSourceKind.ScheduledRule);
		Assert.IsTrue(result.ManagerGoalTasks.Single().SourceKind == EmploymentTaskSourceKind.ManagerGoal);
	}

	[TestMethod]
	public void Payroll_SettlementRequiresBackedEmployerFunds()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var (shop, state) = ShopHost(402, "stable", currency.Object, null);
		IEmploymentHost host = shop.Object;
		var employee = Character(10, "Employee").Object;
		var compensation = new CompensationTerms(
			new MoneyAmount(currency.Object, 10.0M),
			null,
			PayCadence.Daily,
			new MoneyAmount(currency.Object, 10.0M),
			PaymentSource.HostCash);
		var contract = state.Hire(employee, new EmploymentOffer(
			EmploymentRole.Employee,
			compensation,
			WorkSchedule.AnyTime,
			EmploymentDuration.Indefinite,
			new PaymentMethod(PaymentMethodKind.Cash),
			EmploymentAuthoritySet.Empty), null);
		host.Payroll.EvaluatePayroll(contract.StartedAt.AddDays(1).AddMinutes(1));

		var settled = host.Payroll.TrySettlePayables(host.Payroll.OutstandingLiabilities, null, true,
			"Settle without funds.", out var message);

		Assert.IsFalse(settled);
		StringAssert.Contains(message, "available for payroll settlement");
		Assert.IsTrue(host.Payroll.Payables.All(x => x.Status == EmploymentPayableStatus.Accrued));
	}

	[TestMethod]
	public void Payroll_BankSettlementDebitsEmployerAndCreditsEmployeeAccount()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var employeeAccount = BankAccount(currency.Object, 5.0M, 1_000.0M);
		var (shop, state) = ShopHost(403, "payroll shop", currency.Object, null);
		IEmploymentHost host = shop.Object;
		var employee = Character(11, "Employee").Object;
		var compensation = new CompensationTerms(
			new MoneyAmount(currency.Object, 10.0M),
			null,
			PayCadence.Daily,
			new MoneyAmount(currency.Object, 10.0M),
			PaymentSource.HostCash);
		var contract = state.Hire(employee, new EmploymentOffer(
			EmploymentRole.Employee,
			compensation,
			WorkSchedule.AnyTime,
			EmploymentDuration.Indefinite,
			new PaymentMethod(PaymentMethodKind.EmployeeBankAccount, employeeAccount.Object),
			EmploymentAuthoritySet.Empty), null);
		host.Payroll.EvaluatePayroll(contract.StartedAt.AddDays(1).AddMinutes(1));
		VirtualCashLedger.Credit(shop.Object, currency.Object, 10.0M, null, shop.Object,
			"Seed", "Seed payroll balance");

		Assert.IsTrue(host.Payroll.TrySettlePayables(host.Payroll.OutstandingLiabilities, null, false,
			"Bank payroll.", out var message), message);

		Assert.AreEqual(EmploymentPayableStatus.Settled, host.Payroll.Payables.Single().Status);
		Assert.AreEqual(0.0M, VirtualCashLedger.Balance(shop.Object, currency.Object));
		Assert.AreEqual(15.0M, employeeAccount.Object.CurrentBalance);
		Assert.AreEqual(1_010.0M, employeeAccount.Object.Bank.CurrencyReserves[currency.Object]);
		Assert.IsTrue(host.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.Wage));
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.WageSettled));
	}

	[TestMethod]
	public void Payroll_FailedBankDestinationLeavesPayableOutstandingAndFundsUntouched()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var (shop, state) = ShopHost(404, "payroll shop", currency.Object, null);
		IEmploymentHost host = shop.Object;
		var employee = Character(12, "Employee").Object;
		var compensation = new CompensationTerms(
			new MoneyAmount(currency.Object, 10.0M),
			null,
			PayCadence.Daily,
			new MoneyAmount(currency.Object, 10.0M),
			PaymentSource.HostCash);
		var contract = state.Hire(employee, new EmploymentOffer(
			EmploymentRole.Employee,
			compensation,
			WorkSchedule.AnyTime,
			EmploymentDuration.Indefinite,
			new PaymentMethod(PaymentMethodKind.EmployeeBankAccount),
			EmploymentAuthoritySet.Empty), null);
		host.Payroll.EvaluatePayroll(contract.StartedAt.AddDays(1).AddMinutes(1));
		VirtualCashLedger.Credit(shop.Object, currency.Object, 10.0M, null, shop.Object,
			"Seed", "Seed payroll balance");

		var settled = host.Payroll.TrySettlePayables(host.Payroll.OutstandingLiabilities, null, false,
			"Bank payroll.", out var message);

		Assert.IsFalse(settled);
		StringAssert.Contains(message, "destination bank account");
		Assert.AreEqual(EmploymentPayableStatus.Accrued, host.Payroll.Payables.Single().Status);
		Assert.AreEqual(10.0M, VirtualCashLedger.Balance(shop.Object, currency.Object));
	}

	[TestMethod]
	public void Payroll_FormerEmployeeCashSettlementCanBecomeClaimable()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var (shop, state) = ShopHost(405, "payroll shop", currency.Object, null);
		IEmploymentHost host = shop.Object;
		var employee = Character(13, "Employee").Object;
		var compensation = new CompensationTerms(
			new MoneyAmount(currency.Object, 10.0M),
			null,
			PayCadence.Daily,
			new MoneyAmount(currency.Object, 10.0M),
			PaymentSource.HostCash);
		var contract = state.Hire(employee, new EmploymentOffer(
			EmploymentRole.Employee,
			compensation,
			WorkSchedule.AnyTime,
			EmploymentDuration.Indefinite,
			new PaymentMethod(PaymentMethodKind.Cash),
			EmploymentAuthoritySet.Empty), null);
		host.Payroll.EvaluatePayroll(contract.StartedAt.AddDays(1).AddMinutes(1));
		host.Fire(contract, EmploymentTerminationReason.Resigned, null);
		VirtualCashLedger.Credit(shop.Object, currency.Object, 10.0M, null, shop.Object,
			"Seed", "Seed payroll balance");

		Assert.IsTrue(host.Payroll.TrySettlePayables(host.Payroll.OutstandingLiabilities, null, true,
			"Fund cash payroll.", out var message), message);

		Assert.AreEqual(EmploymentPayableStatus.ReadyToClaim, host.Payroll.Payables.Single().Status);
		Assert.AreEqual(0.0M, VirtualCashLedger.Balance(shop.Object, currency.Object));
		StringAssert.Contains(message, "ready for employee claim");
	}

	[TestMethod]
	public void HostBoard_BoardPostActionStepCreatesPostAndRegisterEntry()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("stable", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.PostToHostBoard), null);

		var step = new BoardPostActionStep("Deposit complete", "The deposit has been made.");
		var task = host.TaskBoard.CreateActiveTask("post notice", new EmploymentActionPlan([step]), manager);
		var context = new EmploymentTaskContext(host);
		var dispatcher = new EmploymentTaskDispatcher();
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanPostToBoard));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out _));
		var result = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(result.Success);
		Assert.AreEqual(1, host.Board.Posts.Count());
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.BoardPostCreated));
	}

	[TestMethod]
	public void ScheduledRule_ComposesConditionsAndSpawnsActiveTaskOncePerWindow()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateScheduledRules |
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.PostToHostBoard |
			EmploymentAuthority.ManageStockRules), null);
		var context = new EmploymentTaskContext(host);
		context.SetStockLevel("butter", 2);
		context.SetManualOrder("restock-butter", true);

		var plan = new EmploymentActionPlan([new BoardPostActionStep("Butter", "Butter task spawned.")]);
		host.TaskBoard.CreateScheduledRule("buy butter", "butter-low",
			[
				new StockThresholdCondition("butter", 5, true),
				new ManualOrderCondition("restock-butter")
			],
			plan,
			TimeSpan.FromHours(1),
			manager);

		var first = host.TaskBoard.EvaluateScheduledRules(context, DateTimeOffset.UtcNow);
		var second = host.TaskBoard.EvaluateScheduledRules(context, DateTimeOffset.UtcNow.AddMinutes(1));

		Assert.AreEqual(1, first.Count);
		Assert.AreEqual(0, second.Count);
		Assert.AreEqual(EmploymentActionStepStatus.Pending, first.Single().StepStates.Single());
	}

	[TestMethod]
	public void ScheduledRuleAdministrationActionStep_PausesRuleThroughTaskBoard()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(21, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.CreateScheduledRules |
			EmploymentAuthority.ModifyScheduledRules |
			EmploymentAuthority.PostToHostBoard), null);
		var rule = host.TaskBoard.CreateScheduledRule("pause target", "pause-target",
			[],
			new EmploymentActionPlan([new BoardPostActionStep("Pause", "Should not spawn.")]),
			TimeSpan.Zero,
			manager);
		var step = new ScheduledRuleAdministrationActionStep(
			ScheduledRuleAdministrationActionKind.Pause,
			rule.Id,
			rule.Name,
			"Pause while stocktaking.");
		var task = host.TaskBoard.CreateActiveTask("pause scheduled rule", new EmploymentActionPlan([step]), manager);
		((EmploymentActiveTask)task).Assign(manager);
		var context = new EmploymentTaskContext(host);
		var dispatcher = new EmploymentTaskDispatcher();

		var result = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(result.Success, result.Message);
		Assert.AreEqual(EmploymentScheduledRuleStatus.Paused, rule.Status);
		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		StringAssert.Contains(task.StepOperationalStates.Single().OperationalPayload!, "rule:pause=");
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ScheduledRulePaused));
	}

	[TestMethod]
	public void ScheduledRuleAdministrationActionStep_EvaluatesRuleWithManualTrigger()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(22, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.CreateScheduledRules |
			EmploymentAuthority.ModifyScheduledRules |
			EmploymentAuthority.PostToHostBoard), null);
		var rule = host.TaskBoard.CreateScheduledRule("manual restock", "manual-restock",
			[new ManualOrderCondition("restock-now")],
			new EmploymentActionPlan([new BoardPostActionStep("Restock", "Restock now.")]),
			TimeSpan.Zero,
			manager);
		var step = new ScheduledRuleAdministrationActionStep(
			ScheduledRuleAdministrationActionKind.Evaluate,
			rule.Id,
			rule.Name,
			manualKey: "restock-now");
		var adminTask = host.TaskBoard.CreateActiveTask("evaluate scheduled rule", new EmploymentActionPlan([step]), manager);
		((EmploymentActiveTask)adminTask).Assign(manager);
		var context = new EmploymentTaskContext(host);
		var dispatcher = new EmploymentTaskDispatcher();

		var result = dispatcher.AdvanceTask(adminTask, context);

		Assert.IsTrue(result.Success, result.Message);
		Assert.AreEqual(EmploymentTaskStatus.Completed, adminTask.Status);
		var spawned = host.TaskBoard.ActiveTasks.Single(x => x.SourceRuleId == rule.Id);
		Assert.AreEqual(EmploymentTaskSourceKind.ScheduledRule, spawned.SourceKind);
		Assert.AreEqual(rule.Id, spawned.SourceRuleId);
		StringAssert.Contains(adminTask.StepOperationalStates.Single().OperationalPayload!, "spawned=1");
		StringAssert.Contains(adminTask.StepOperationalStates.Single().OperationalPayload!, "manual=restock-now");
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ScheduledRuleEvaluated));
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.AuditActionRecorded &&
			x.Description.Contains("Evaluated scheduled task rule manual restock", StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	public void ManagerGoalAdministrationActionStep_EvaluatesGoalThroughGoalBoard()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(41, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.CreateManagerGoals |
			EmploymentAuthority.ModifyManagerGoals |
			EmploymentAuthority.PostToHostBoard), null);
		var goal = host.ManagerGoalBoard.CreateGoal(new ManagerGoalDefinition(
			ManagerGoalType.MaintainHotelOperations,
			EmploymentAuthority.PostToHostBoard,
			new ManagerGoalConfiguration("post room notice", new EmploymentActionPlan([
				new BoardPostActionStep("Rooms", "Room work required.")
			])),
			1,
			TimeSpan.Zero), manager);
		var step = new ManagerGoalAdministrationActionStep(
			ManagerGoalAdministrationActionKind.Evaluate,
			goal.Id,
			goal.Configuration.Description);
		var adminTask = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("evaluate manager goal",
			new EmploymentActionPlan([step]), manager);
		adminTask.Assign(manager);
		var context = new EmploymentTaskContext(host);
		var dispatcher = new EmploymentTaskDispatcher();

		var result = dispatcher.AdvanceTask(adminTask, context);

		Assert.IsTrue(result.Success, result.Message);
		Assert.AreEqual(EmploymentTaskStatus.Completed, adminTask.Status);
		var spawned = host.TaskBoard.ActiveTasks.Single(x => x.SourceGoalId == goal.Id);
		Assert.AreEqual(EmploymentTaskSourceKind.ManagerGoal, spawned.SourceKind);
		Assert.AreEqual(goal.Id, spawned.SourceGoalId);
		StringAssert.Contains(adminTask.StepOperationalStates.Single().OperationalPayload!, "goal:evaluate=1");
		StringAssert.Contains(adminTask.StepOperationalStates.Single().OperationalPayload!, "spawned=1");
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ManagerGoalEvaluated));
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.AuditActionRecorded &&
			x.Description.Contains("Evaluated manager goal", StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	public void ManagerGoalAdministrationActionStep_ReactivatesBlockedGoalAndKeepsCancelledTerminal()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(42, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.CreateManagerGoals |
			EmploymentAuthority.ModifyManagerGoals |
			EmploymentAuthority.PostToHostBoard), null);
		var goal = host.ManagerGoalBoard.CreateGoal(new ManagerGoalDefinition(
			ManagerGoalType.MaintainHotelOperations,
			EmploymentAuthority.PostToHostBoard,
			new ManagerGoalConfiguration("post room notice", new EmploymentActionPlan([
				new BoardPostActionStep("Rooms", "Room work required.")
			])),
			1,
			TimeSpan.Zero), manager);
		((ManagerGoal)goal).Block("Waiting on manager review.");
		var adminTask = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("recover manager goal",
			new EmploymentActionPlan([
				new ManagerGoalAdministrationActionStep(
					ManagerGoalAdministrationActionKind.Reactivate,
					goal.Id,
					goal.Configuration.Description,
					"Manager review complete."),
				new ManagerGoalAdministrationActionStep(
					ManagerGoalAdministrationActionKind.Cancel,
					goal.Id,
					goal.Configuration.Description,
					"Replaced by a new goal.")
			]), manager);
		adminTask.Assign(manager);
		var context = new EmploymentTaskContext(host);
		var dispatcher = new EmploymentTaskDispatcher();

		var reactivate = dispatcher.AdvanceTask(adminTask, context);
		var cancel = dispatcher.AdvanceTask(adminTask, context);
		var reactivateCancelled = new ManagerGoalAdministrationActionStep(
			ManagerGoalAdministrationActionKind.Reactivate,
			goal.Id,
			goal.Configuration.Description,
			"Do not reopen cancelled work.");

		Assert.IsTrue(reactivate.Success, reactivate.Message);
		Assert.IsTrue(cancel.Success, cancel.Message);
		Assert.AreEqual(ManagerGoalStatus.Cancelled, goal.Status);
		Assert.AreEqual(EmploymentTaskStatus.Completed, adminTask.Status);
		Assert.IsFalse(reactivateCancelled.CanExecute(context, manager, out var reason));
		StringAssert.Contains(reason, "cannot be reactivated");
		StringAssert.Contains(adminTask.StepOperationalStates[0].OperationalPayload!, "goal:reactivate=1");
		StringAssert.Contains(adminTask.StepOperationalStates[1].OperationalPayload!, "goal:cancel=1");
	}

	[TestMethod]
	public void ActiveTaskAdministrationActionStep_CancelsTargetTaskThroughBoard()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(31, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.CancelTasks), null);
		var target = host.TaskBoard.CreateActiveTask("obsolete report",
			new EmploymentActionPlan([new CataloguedActionShellStep("report", "obsolete work")]), manager);
		var step = new ActiveTaskAdministrationActionStep(
			ActiveTaskAdministrationActionKind.Cancel,
			target.Id,
			target.Name,
			reason: "No longer needed.");
		var adminTask = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("cancel obsolete report",
			new EmploymentActionPlan([step]), manager);
		adminTask.Assign(manager);
		var context = new EmploymentTaskContext(host);
		var dispatcher = new EmploymentTaskDispatcher();

		var result = dispatcher.AdvanceTask(adminTask, context);

		Assert.IsTrue(result.Success, result.Message);
		Assert.AreEqual(EmploymentTaskStatus.Cancelled, target.Status);
		Assert.AreEqual(EmploymentTaskStatus.Completed, adminTask.Status);
		StringAssert.Contains(adminTask.StepOperationalStates.Single().OperationalPayload!, "taskadmin:cancel=");
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ActiveTaskCancelled));
	}

	[TestMethod]
	public void ActiveTaskAdministrationActionStep_RetriesBlockedTaskAndClearsFailure()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(32, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks), null);
		var target = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("blocked report",
			new EmploymentActionPlan([new CataloguedActionShellStep("report", "blocked work")]), manager);
		target.MarkStep(0, EmploymentActionStepStatus.Blocked,
			new EmploymentActionStepOperationalState(FailureDiagnostic: "blocked by manager review"));
		target.Block("blocked by manager review");
		var step = new ActiveTaskAdministrationActionStep(
			ActiveTaskAdministrationActionKind.Retry,
			target.Id,
			target.Name,
			reason: "Manager cleared the blocker.");
		var adminTask = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("retry blocked report",
			new EmploymentActionPlan([step]), manager);
		adminTask.Assign(manager);
		var context = new EmploymentTaskContext(host);
		var dispatcher = new EmploymentTaskDispatcher();

		var result = dispatcher.AdvanceTask(adminTask, context);

		Assert.IsTrue(result.Success, result.Message);
		Assert.AreEqual(EmploymentTaskStatus.Pending, target.Status);
		Assert.AreEqual(EmploymentActionStepStatus.Pending, target.StepStates.Single());
		Assert.IsTrue(string.IsNullOrWhiteSpace(target.StepOperationalStates.Single().FailureDiagnostic));
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ActiveTaskRequeued));
	}

	[TestMethod]
	public void ActiveTaskAdministrationActionStep_RequeuesAndAssignsTargetTask()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(33, "Manager").Object;
		var firstWorker = Character(34, "First Worker").Object;
		var secondWorker = Character(35, "Second Worker").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.HireEmployees), null);
		host.Hire(firstWorker, Offer(currency.Object, EmploymentRole.Employee,
			EmploymentAuthority.AssignTasks), manager);
		host.Hire(secondWorker, Offer(currency.Object, EmploymentRole.Employee,
			EmploymentAuthority.AssignTasks), manager);
		var target = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("assigned report",
			new EmploymentActionPlan([new CataloguedActionShellStep("report", "assigned work")]), manager);
		target.Assign(firstWorker);
		var adminTask = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("rebalance report",
			new EmploymentActionPlan([
				new ActiveTaskAdministrationActionStep(
					ActiveTaskAdministrationActionKind.Requeue,
					target.Id,
					target.Name,
					reason: "Release for reassignment."),
				new ActiveTaskAdministrationActionStep(
					ActiveTaskAdministrationActionKind.Assign,
					target.Id,
					target.Name,
					secondWorker.Id,
					secondWorker.Name,
					"Assign to available worker.")
			]), manager);
		adminTask.Assign(manager);
		var context = new EmploymentTaskContext(host);
		var dispatcher = new EmploymentTaskDispatcher();

		var requeue = dispatcher.AdvanceTask(adminTask, context);
		var assign = dispatcher.AdvanceTask(adminTask, context);

		Assert.IsTrue(requeue.Success, requeue.Message);
		Assert.IsTrue(assign.Success, assign.Message);
		Assert.AreEqual(EmploymentTaskStatus.Assigned, target.Status);
		Assert.AreSame(secondWorker, target.AssignedEmployee);
		Assert.AreEqual(EmploymentTaskStatus.Completed, adminTask.Status);
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ActiveTaskRequeued));
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ActiveTaskAssigned));
	}

	[TestMethod]
	public void ArenaEventAdministrationActionStep_CreatesNativeArenaEventAndRecordsAudit()
	{
		var currency = Currency();
		var arena = new Mock<ICombatArena>();
		var state = new EmploymentHostState(arena.Object);
		arena.SetupGet(x => x.Id).Returns(400);
		arena.SetupGet(x => x.Name).Returns("red sands");
		arena.SetupGet(x => x.FrameworkItemType).Returns("CombatArena");
		arena.SetupGet(x => x.EmploymentHostName).Returns("red sands");
		arena.SetupGet(x => x.EmploymentHostType).Returns(EmploymentHostType.Arena);
		arena.SetupGet(x => x.Market).Returns((IMarket?)null);
		arena.SetupGet(x => x.Employment).Returns(state);
		arena.SetupGet(x => x.EmploymentRegister).Returns(state.EmploymentRegister);
		arena.SetupGet(x => x.TaskBoard).Returns(state.TaskBoard);
		arena.SetupGet(x => x.ManagerGoalBoard).Returns(state.ManagerGoalBoard);
		arena.SetupGet(x => x.Payroll).Returns(state.Payroll);
		arena.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		      .Returns((ICharacter actor, EmploymentAuthority authority) => state.HasAuthority(actor, authority));

		var eventType = new Mock<IArenaEventType>();
		eventType.SetupGet(x => x.Id).Returns(401);
		eventType.SetupGet(x => x.Name).Returns("trial bout");
		eventType.SetupGet(x => x.Arena).Returns(arena.Object);
		var scheduled = new DateTime(2099, 1, 1, 10, 0, 0, DateTimeKind.Utc);
		var created = new Mock<IArenaEvent>();
		created.SetupGet(x => x.Id).Returns(402);
		created.SetupGet(x => x.Name).Returns("trial bout #1");
		created.SetupGet(x => x.State).Returns(ArenaEventState.Scheduled);
		arena.SetupGet(x => x.EventTypes).Returns([eventType.Object]);
		arena.SetupGet(x => x.ActiveEvents).Returns(Array.Empty<IArenaEvent>());
		arena.Setup(x => x.IsReadyToHost(eventType.Object)).Returns((true, string.Empty));
		arena.Setup(x => x.CreateEvent(eventType.Object, scheduled, It.IsAny<IEnumerable<IArenaReservation>?>()))
		      .Returns(created.Object);

		var manager = Character(40, "Arena Manager").Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.CreateScheduledRules), null);
		var task = (EmploymentActiveTask)state.TaskBoard.CreateActiveTask("create arena event",
			new EmploymentActionPlan([new ArenaEventAdministrationActionStep(arena.Object, eventType.Object, scheduled)]),
			manager);
		task.Assign(manager);
		var context = new EmploymentTaskContext(arena.Object);
		var dispatcher = new EmploymentTaskDispatcher();

		var result = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(result.Success, result.Message);
		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		arena.Verify(x => x.CreateEvent(eventType.Object, scheduled, It.IsAny<IEnumerable<IArenaReservation>?>()), Times.Once);
		StringAssert.Contains(task.StepOperationalStates.Single().SelectedResources!, "arenaevent:create");
		Assert.IsTrue(state.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.AuditActionRecorded));
	}

	[TestMethod]
	public void ArenaEventAdministrationActionStep_TransitionsAndAbortsNativeArenaEvent()
	{
		var currency = Currency();
		var lifecycle = new Mock<IArenaLifecycleService>();
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>());
		gameworld.SetupGet(x => x.ArenaLifecycleService).Returns(lifecycle.Object);
		var arena = new Mock<ICombatArena>();
		var state = new EmploymentHostState(arena.Object);
		arena.SetupGet(x => x.Id).Returns(410);
		arena.SetupGet(x => x.Name).Returns("red sands");
		arena.SetupGet(x => x.FrameworkItemType).Returns("CombatArena");
		arena.SetupGet(x => x.EmploymentHostName).Returns("red sands");
		arena.SetupGet(x => x.EmploymentHostType).Returns(EmploymentHostType.Arena);
		arena.SetupGet(x => x.Market).Returns((IMarket?)null);
		arena.SetupGet(x => x.Employment).Returns(state);
		arena.SetupGet(x => x.EmploymentRegister).Returns(state.EmploymentRegister);
		arena.SetupGet(x => x.TaskBoard).Returns(state.TaskBoard);
		arena.SetupGet(x => x.ManagerGoalBoard).Returns(state.ManagerGoalBoard);
		arena.SetupGet(x => x.Payroll).Returns(state.Payroll);
		arena.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		      .Returns((ICharacter actor, EmploymentAuthority authority) => state.HasAuthority(actor, authority));

		var currentState = ArenaEventState.Scheduled;
		var arenaEvent = new Mock<IArenaEvent>();
		arenaEvent.SetupGet(x => x.Id).Returns(411);
		arenaEvent.SetupGet(x => x.Name).Returns("trial bout #2");
		arenaEvent.SetupGet(x => x.Arena).Returns(arena.Object);
		arenaEvent.SetupGet(x => x.State).Returns(() => currentState);
		arena.SetupGet(x => x.ActiveEvents).Returns([arenaEvent.Object]);
		lifecycle.Setup(x => x.Transition(arenaEvent.Object, ArenaEventState.Preparing))
		         .Callback(() => currentState = ArenaEventState.Preparing);
		var manager = Character(41, "Arena Manager", gameworld: gameworld.Object).Object;
		arena.Setup(x => x.AbortEvent(arenaEvent.Object, "rain delay", manager))
		      .Callback(() => currentState = ArenaEventState.Aborted);
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ModifyScheduledRules), null);
		var task = (EmploymentActiveTask)state.TaskBoard.CreateActiveTask("manage arena event",
			new EmploymentActionPlan([
				new ArenaEventAdministrationActionStep(arena.Object, arenaEvent.Object, ArenaEventState.Preparing),
				new ArenaEventAdministrationActionStep(arena.Object, arenaEvent.Object, "rain delay")
			]), manager);
		task.Assign(manager);
		var context = new EmploymentTaskContext(arena.Object);
		var dispatcher = new EmploymentTaskDispatcher();

		var phase = dispatcher.AdvanceTask(task, context);
		var abort = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(phase.Success, phase.Message);
		Assert.IsTrue(abort.Success, abort.Message);
		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		lifecycle.Verify(x => x.Transition(arenaEvent.Object, ArenaEventState.Preparing), Times.Once);
		arena.Verify(x => x.AbortEvent(arenaEvent.Object, "rain delay", manager), Times.Once);
		StringAssert.Contains(task.StepOperationalStates[0].OperationalPayload!, "arenaevent:phase");
		StringAssert.Contains(task.StepOperationalStates[1].OperationalPayload!, "arenaevent:abort");
	}

	[TestMethod]
	public void ShopStocktakeActionStep_RecordsNativeShopStocktake()
	{
		var currency = Currency();
		var (shop, state) = ShopHost(50, "Stocktake Shop", currency.Object, null);
		var manager = Character(43, "Manager").Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageStockRules), null);
		var itemMerchandise = new Mock<IMerchandise>();
		itemMerchandise.SetupGet(x => x.Id).Returns(101);
		itemMerchandise.SetupGet(x => x.Name).Returns("linen bundle");
		itemMerchandise.SetupGet(x => x.ListDescription).Returns("linen bundle");
		itemMerchandise.SetupGet(x => x.MerchandiseType).Returns(MerchandiseType.Item);
		itemMerchandise.SetupGet(x => x.Shop).Returns(shop.Object);
		var commodityMerchandise = new Mock<IMerchandise>();
		commodityMerchandise.SetupGet(x => x.Id).Returns(102);
		commodityMerchandise.SetupGet(x => x.Name).Returns("grain sacks");
		commodityMerchandise.SetupGet(x => x.ListDescription).Returns("grain sacks");
		commodityMerchandise.SetupGet(x => x.MerchandiseType).Returns(MerchandiseType.Commodity);
		commodityMerchandise.SetupGet(x => x.Shop).Returns(shop.Object);
		shop.SetupGet(x => x.Merchandises).Returns([itemMerchandise.Object, commodityMerchandise.Object]);
		shop.Setup(x => x.StocktakeMerchandise(itemMerchandise.Object)).Returns((3, 5));
		shop.Setup(x => x.StocktakeMerchandiseWeight(commodityMerchandise.Object)).Returns((12.5, 20.0));
		var task = state.TaskBoard.CreateActiveTask("stocktake shop",
			new EmploymentActionPlan([
				new ShopStocktakeActionStep(ShopStocktakeScope.Merchandise, "101", "linen bundle"),
				new ShopStocktakeActionStep()
			]), manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(shop.Object);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash, Caps());

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		var targeted = dispatcher.AdvanceTask(task, context);
		var full = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(targeted.Success, targeted.Message);
		Assert.IsTrue(full.Success, full.Message);
		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		shop.Verify(x => x.StocktakeMerchandise(itemMerchandise.Object), Times.Exactly(2));
		shop.Verify(x => x.StocktakeMerchandiseWeight(commodityMerchandise.Object), Times.Once);
		StringAssert.Contains(task.StepOperationalStates[0].OperationalPayload!, "stocktake:merchandise=101");
		StringAssert.Contains(task.StepOperationalStates[0].OperationalPayload!, "101:count:3.00:5.00");
		StringAssert.Contains(task.StepOperationalStates[1].OperationalPayload!, "101:count:3.00:5.00");
		StringAssert.Contains(task.StepOperationalStates[1].OperationalPayload!, "102:weight:12.50:20.00");
		Assert.IsTrue(shop.Object.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.AuditActionRecorded &&
			x.Description.Contains("Stocktook", StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	public void ShopCashReconciliationActionStep_RecordsNativeCashCheck()
	{
		var currency = Currency();
		var (shop, state) = ShopHost(52, "Cash Shop", currency.Object, null);
		var manager = Character(45, "Manager").Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.DepositBusinessCash |
			EmploymentAuthority.WithdrawBusinessCash), null);
		var expectedCash = 12.0M;
		shop.SetupGet(x => x.ExpectedCashBalance).Returns(() => expectedCash);
		shop.SetupSet(x => x.ExpectedCashBalance = It.IsAny<decimal>())
		    .Callback<decimal>(value => expectedCash = value);
		shop.SetupGet(x => x.CashBalance).Returns(5.0M);
		var pile = new Mock<ICurrencyPile>();
		pile.SetupGet(x => x.Currency).Returns(currency.Object);
		pile.SetupGet(x => x.TotalValue).Returns(9.0M);
		shop.Setup(x => x.GetCurrencyPilesForShop()).Returns([pile.Object]);
		shop.Setup(x => x.CheckFloat()).Callback(() => expectedCash = 14.0M);
		var task = state.TaskBoard.CreateActiveTask("cash check",
			new EmploymentActionPlan([new ShopCashReconciliationActionStep("closing count")]), manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(shop.Object);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanHandleCash));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		var result = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(result.Success, result.Message);
		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		shop.Verify(x => x.CheckFloat(), Times.Once);
		StringAssert.Contains(task.StepOperationalStates[0].OperationalPayload!, "expected=12.00");
		StringAssert.Contains(task.StepOperationalStates[0].OperationalPayload!, "virtual=5.00");
		StringAssert.Contains(task.StepOperationalStates[0].OperationalPayload!, "physical=9.00");
		StringAssert.Contains(task.StepOperationalStates[0].OperationalPayload!, "variance=2.00");
		StringAssert.Contains(task.StepOperationalStates[0].OperationalPayload!, "adjusted=14.00");
		StringAssert.Contains(task.StepOperationalStates[0].OperationalPayload!, "aftervariance=0.00");
		Assert.IsTrue(shop.Object.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.AuditActionRecorded &&
			x.Description.Contains("Reconciled cash", StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	public void SupplierSelectionActionStep_RecordsCheapestSupplierPreview()
	{
		var currency = Currency();
		var (buyer, state) = ShopHost(51, "Buyer Shop", currency.Object, null);
		var supplierLocation = Cell(610, "supplier market").Object;
		var (cheapSupplier, _) = ShopHost(61, "Cheap Supplier", currency.Object, null);
		var (expensiveSupplier, _) = ShopHost(62, "Expensive Supplier", currency.Object, null);
		var cheapMerchandise = Merchandise(611, "apple");
		var expensiveMerchandise = Merchandise(612, "apple");
		cheapMerchandise.SetupGet(x => x.ListDescription).Returns("apple");
		expensiveMerchandise.SetupGet(x => x.ListDescription).Returns("apple");
		cheapMerchandise.SetupGet(x => x.MerchandiseType).Returns(MerchandiseType.Item);
		expensiveMerchandise.SetupGet(x => x.MerchandiseType).Returns(MerchandiseType.Item);
		cheapMerchandise.SetupGet(x => x.Shop).Returns(cheapSupplier.Object);
		expensiveMerchandise.SetupGet(x => x.Shop).Returns(expensiveSupplier.Object);
		var cheapStock = Item(613, "cheap apples");
		var expensiveStock = Item(614, "expensive apples");
		cheapStock.SetupGet(x => x.Quantity).Returns(3);
		expensiveStock.SetupGet(x => x.Quantity).Returns(3);
		cheapSupplier.SetupGet(x => x.CurrentLocations).Returns([supplierLocation]);
		expensiveSupplier.SetupGet(x => x.CurrentLocations).Returns([supplierLocation]);
		cheapSupplier.SetupGet(x => x.Merchandises).Returns([cheapMerchandise.Object]);
		expensiveSupplier.SetupGet(x => x.Merchandises).Returns([expensiveMerchandise.Object]);
		cheapSupplier.Setup(x => x.StockedItems(cheapMerchandise.Object)).Returns([cheapStock.Object]);
		expensiveSupplier.Setup(x => x.StockedItems(expensiveMerchandise.Object)).Returns([expensiveStock.Object]);
		cheapSupplier.Setup(x => x.PriceForMerchandise(It.IsAny<ICharacter?>(), cheapMerchandise.Object, 2)).Returns(5.0M);
		expensiveSupplier.Setup(x => x.PriceForMerchandise(It.IsAny<ICharacter?>(), expensiveMerchandise.Object, 2)).Returns(8.0M);
		var managerMock = Character(44, "Manager");
		var manager = managerMock.Object;
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter> { [manager.Id] = manager },
			[supplierLocation], shopsToAdd: [cheapSupplier.Object, expensiveSupplier.Object]);
		buyer.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		managerMock.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageStockRules), null);
		var task = state.TaskBoard.CreateActiveTask("find apples",
			new EmploymentActionPlan([
				new SupplierSelectionActionStep(new PurchaseActionStep(2, "apple", "any", currency.Object,
					new MoneyAmount(currency.Object, 10.0M)))
			]), manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(buyer.Object);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanPurchaseCommodities));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		var result = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(result.Success, result.Message);
		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		StringAssert.Contains(task.StepOperationalStates[0].OperationalPayload!, "supplier=61");
		StringAssert.Contains(task.StepOperationalStates[0].OperationalPayload!, "merchandise=611");
		StringAssert.Contains(task.StepOperationalStates[0].OperationalPayload!, "price=5.00");
		StringAssert.Contains(task.StepOperationalStates[0].SelectedResources!, "610:supplier market");
		Assert.IsTrue(buyer.Object.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.AuditActionRecorded &&
			x.Description.Contains("Selected supplier Cheap Supplier", StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	public void ScheduledRuleExpression_OrNotAndNamedPredicate_ControlSpawning()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(11, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateScheduledRules |
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.PostToHostBoard |
			EmploymentAuthority.ManageStockRules), null);
		host.TaskBoard.CreateConditionPredicate("manual-window",
			[new ManualOrderCondition("manager-window")],
			EmploymentConditionExpression.Condition(1),
			manager);

		var plan = new EmploymentActionPlan([new BoardPostActionStep("Expression", "Expression task spawned.")]);
		host.TaskBoard.CreateScheduledRule("expression restock", "expression-restock",
			[
				new StockThresholdCondition("butter", 5, true),
				new ManualOrderCondition("override"),
				new ManualOrderCondition("blocked")
			],
			EmploymentConditionExpression.All(
			[
				EmploymentConditionExpression.Any(
				[
					EmploymentConditionExpression.Condition(1),
					EmploymentConditionExpression.Condition(2),
					EmploymentConditionExpression.Predicate("manual-window")
				]),
				EmploymentConditionExpression.Not(EmploymentConditionExpression.Condition(3))
			]),
			plan,
			TimeSpan.Zero,
			manager);

		var context = new EmploymentTaskContext(host);
		context.SetStockLevel("butter", 10);
		Assert.AreEqual(0, host.TaskBoard.EvaluateScheduledRules(context, DateTimeOffset.UtcNow).Count);

		context.SetManualOrder("manager-window", true);
		var spawned = host.TaskBoard.EvaluateScheduledRules(context, DateTimeOffset.UtcNow.AddMinutes(1));
		Assert.AreEqual(1, spawned.Count);
	}

	[TestMethod]
	public void ScheduledRuleExpression_NestedPredicateAuthority_IsRequired()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var fullManager = Character(13, "Full Manager").Object;
		var limitedManager = Character(14, "Limited Manager").Object;
		host.Hire(fullManager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateScheduledRules |
			EmploymentAuthority.PostToHostBoard |
			EmploymentAuthority.ManageStockRules), null);
		host.Hire(limitedManager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateScheduledRules |
			EmploymentAuthority.PostToHostBoard), null);
		host.TaskBoard.CreateConditionPredicate("stock-sensitive",
			[new StockThresholdCondition("butter", 5, true)],
			EmploymentConditionExpression.Condition(1),
			fullManager);
		host.TaskBoard.CreateConditionPredicate("wrapper",
			[],
			EmploymentConditionExpression.Predicate("stock-sensitive"),
			fullManager);
		var plan = new EmploymentActionPlan([new BoardPostActionStep("Expression", "Expression task spawned.")]);

		var ex = Assert.ThrowsException<InvalidOperationException>(() =>
			host.TaskBoard.CreateScheduledRule("expression restock", "expression-restock",
				[],
				EmploymentConditionExpression.Predicate("wrapper"),
				plan,
				TimeSpan.Zero,
				limitedManager));
		StringAssert.Contains(ex.Message, nameof(EmploymentAuthority.ManageStockRules));
	}

	[TestMethod]
	public void ScheduledRuleExpression_LegacyNullExpressionKeepsImplicitAnd()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(12, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateScheduledRules |
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.PostToHostBoard |
			EmploymentAuthority.ManageStockRules), null);
		var context = new EmploymentTaskContext(host);
		context.SetStockLevel("butter", 2);
		context.SetManualOrder("restock-butter", false);

		host.TaskBoard.CreateScheduledRule("legacy and", "legacy-and",
			[
				new StockThresholdCondition("butter", 5, true),
				new ManualOrderCondition("restock-butter")
			],
			new EmploymentActionPlan([new BoardPostActionStep("Legacy", "Legacy task spawned.")]),
			TimeSpan.Zero,
			manager);

		Assert.AreEqual(0, host.TaskBoard.EvaluateScheduledRules(context, DateTimeOffset.UtcNow).Count);
		context.SetManualOrder("restock-butter", true);
		Assert.AreEqual(1, host.TaskBoard.EvaluateScheduledRules(context, DateTimeOffset.UtcNow.AddMinutes(1)).Count);
	}

	[TestMethod]
	public void ScheduledRuleEvaluationService_EvaluatesRulesWithoutWorkerHeartbeat()
	{
		var currency = Currency();
		var (shop, state) = ShopHost(77, "Central Scheduler Shop", currency.Object, null);
		var (clan, clanState) = ClanHost(78, "Central Scheduler Clan", currency.Object, null);
		var (templateClan, templateClanState) = ClanHost(79, "Central Scheduler Clan Template", currency.Object, null);
		templateClan.SetupGet(x => x.IsTemplate).Returns(true);
		var manager = Character(77, "Manager").Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateScheduledRules |
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.PostToHostBoard), null);
		clanState.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateScheduledRules |
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.PostToHostBoard), null);
		templateClanState.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateScheduledRules |
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.PostToHostBoard), null);
		state.TaskBoard.CreateScheduledRule("central restock", "central-restock",
			[],
			new EmploymentActionPlan([new BoardPostActionStep("Restock", "Please restock.")]),
			TimeSpan.Zero,
			manager);
		clanState.TaskBoard.CreateScheduledRule("central clan report", "central-clan-report",
			[],
			new EmploymentActionPlan([new BoardPostActionStep("Clan", "Please inspect the hall.")]),
			TimeSpan.Zero,
			manager);
		templateClanState.TaskBoard.CreateScheduledRule("central template report", "central-template-report",
			[],
			new EmploymentActionPlan([new BoardPostActionStep("Template", "Please inspect the template hall.")]),
			TimeSpan.Zero,
			manager);
		var shops = new All<IShop> { shop.Object };
		var clans = new All<IClan> { clan.Object, templateClan.Object };
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Shops).Returns(shops);
		gameworld.SetupGet(x => x.AuctionHouses).Returns(new All<IAuctionHouse>());
		gameworld.SetupGet(x => x.CombatArenas).Returns(new All<ICombatArena>());
		gameworld.SetupGet(x => x.Banks).Returns(new All<IBank>());
		gameworld.SetupGet(x => x.Stables).Returns(new All<IStable>());
		gameworld.SetupGet(x => x.Clans).Returns(clans);
		gameworld.SetupGet(x => x.Properties).Returns(new All<IProperty>());

		var spawned = EmploymentScheduledRuleEvaluationService.EvaluateAll(gameworld.Object);

		Assert.AreEqual(2, spawned);
		Assert.AreEqual(1, state.TaskBoard.ActiveTasks.Count);
		Assert.AreEqual(1, clanState.TaskBoard.ActiveTasks.Count);
		Assert.AreEqual(0, templateClanState.TaskBoard.ActiveTasks.Count);
	}

	[TestMethod]
	public void ScheduledRuleEvaluationService_DoesNotAccessLazyPropertyHotelDuringDiscovery()
	{
		var property = new Mock<IProperty>();
		property.SetupGet(x => x.Id).Returns(999);
		property.SetupGet(x => x.Hotel).Throws(new AssertFailedException("Passive scheduled-rule discovery must not lazy-load property hotels."));
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Shops).Returns(new All<IShop>());
		gameworld.SetupGet(x => x.AuctionHouses).Returns(new All<IAuctionHouse>());
		gameworld.SetupGet(x => x.CombatArenas).Returns(new All<ICombatArena>());
		gameworld.SetupGet(x => x.Banks).Returns(new All<IBank>());
		gameworld.SetupGet(x => x.Stables).Returns(new All<IStable>());
		gameworld.SetupGet(x => x.Properties).Returns(new All<IProperty> { property.Object });

		var spawned = EmploymentScheduledRuleEvaluationService.EvaluateAll(gameworld.Object);

		Assert.AreEqual(0, spawned);
	}

	[TestMethod]
	public void TaxOwingCondition_UsesSupportedShopTaxState()
	{
		var currency = Currency();
		var (shop, _) = ShopHost(78, "Taxed Shop", currency.Object, null);
		var zone = new Mock<IEconomicZone>();
		zone.SetupGet(x => x.Currency).Returns(currency.Object);
		zone.Setup(x => x.OutstandingTaxesForShop(shop.Object)).Returns(25.0M);
		shop.SetupGet(x => x.EconomicZone).Returns(zone.Object);
		var condition = new TaxOwingCondition(10.0M, true);

		var satisfied = condition.IsSatisfied(new EmploymentTaskContext(shop.Object), DateTimeOffset.UtcNow,
			out var reason);

		Assert.IsTrue(satisfied, reason);
	}

	[TestMethod]
	public void Dispatcher_BlocksImpossibleTasksWithUsefulReason()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("auction house", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ApprovePurchases), null);
		var purchase = new PurchaseActionStep("10kg butter", new MoneyAmount(currency.Object, 10.0M));
		var task = host.TaskBoard.CreateActiveTask("purchase butter", new EmploymentActionPlan([purchase]), manager);
		var context = new EmploymentTaskContext(host);
		var dispatcher = new EmploymentTaskDispatcher();
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanPurchaseCommodities));

		var assigned = dispatcher.TryAssignTask(task, [profile], context, out var reason);

		Assert.IsFalse(assigned);
		Assert.AreEqual(EmploymentTaskStatus.Blocked, task.Status);
		Assert.IsTrue(reason.Contains("eligible"));
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ActiveTaskBlocked));
	}

	[TestMethod]
	public void Dispatcher_RequeuesStepBoundaryForEligibleCraftWorkerWhenNoCustodyExists()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("workshop", currency.Object);
		var planner = Character(1, "Planner");
		var crafter = Character(2, "Crafter");
		var manager = Character(3, "Manager");
		var workshop = Cell(31, "workshop").Object;
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>
		{
			[planner.Object.Id] = planner.Object,
			[crafter.Object.Id] = crafter.Object,
			[manager.Object.Id] = manager.Object
		}, [workshop]);
		planner.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		planner.SetupGet(x => x.Location).Returns(workshop);
		crafter.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		crafter.SetupGet(x => x.Location).Returns(workshop);
		manager.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		manager.SetupGet(x => x.Location).Returns(workshop);
		host.Hire(planner.Object, Offer(currency.Object, EmploymentRole.Employee,
			EmploymentAuthority.AssignTasks), null);
		host.Hire(crafter.Object, Offer(currency.Object, EmploymentRole.Employee,
			EmploymentAuthority.ManageCraftRules), null);
		host.Hire(manager.Object, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageCraftRules), null);
		var task = host.TaskBoard.CreateActiveTask("plan then craft",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("report", "materials are ready"),
				new CraftStationActionStep("here")
			]),
			manager.Object);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(host);
		var plannerProfile = Profile(planner.Object, 1.0M, PaymentMethodKind.Cash, Caps());
		var crafterProfile = Profile(crafter.Object, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanCraft));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [plannerProfile], context, out var assignReason), assignReason);
		Assert.AreSame(planner.Object, task.AssignedEmployee);

		var report = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(report.Success, report.Message);
		Assert.AreEqual(EmploymentTaskStatus.Pending, task.Status);
		Assert.IsNull(task.AssignedEmployee);
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.ActiveTaskRequeued &&
			x.CorrelationId == task.CorrelationId));
		Assert.IsTrue(dispatcher.TryAssignTask(task, [crafterProfile], context, out var craftAssignReason),
			craftAssignReason);
		Assert.AreSame(crafter.Object, task.AssignedEmployee);
	}

	[TestMethod]
	public void TaskAssignmentAudit_RequeuesWhenEmployeeIsFiredBeforePhysicalCustody()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("stable", currency.Object);
		var manager = Character(1, "Manager").Object;
		var employee = Character(2, "Employee").Object;
		var destination = Cell(27, "yard").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes |
			EmploymentAuthority.HireEmployees | EmploymentAuthority.FireEmployees), null);
		var contract = host.Hire(employee, Offer(currency.Object, EmploymentRole.Employee,
			EmploymentAuthority.ManageDeliveryRoutes), manager);
		var task = host.TaskBoard.CreateActiveTask("walk to yard",
			new EmploymentActionPlan([new MovementDeliveryActionStep("walk to the yard", destination)]),
			manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(host);
		var profile = Profile(employee, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanDeliverItems));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		host.Fire(contract, EmploymentTerminationReason.Fired, manager);

		Assert.AreEqual(EmploymentTaskStatus.Pending, task.Status);
		Assert.IsNull(task.AssignedEmployee);
		StringAssert.Contains(task.BlockedReason, "returned to pending");
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.ActiveTaskRequeued &&
			x.CorrelationId == task.CorrelationId));
	}

	[TestMethod]
	public void TaskAssignmentAudit_BlocksWhenEmployeeIsFiredWhileHoldingTaskItems()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("stable", currency.Object);
		var manager = Character(1, "Manager").Object;
		var employee = Character(2, "Employee").Object;
		var source = Cell(28, "stockroom").Object;
		var destination = Cell(29, "yard").Object;
		var item = Item(101, "feed sack", prototypeId: 500);
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes |
			EmploymentAuthority.HireEmployees | EmploymentAuthority.FireEmployees), null);
		var contract = host.Hire(employee, Offer(currency.Object, EmploymentRole.Employee,
			EmploymentAuthority.ManageDeliveryRoutes), manager);
		var task = host.TaskBoard.CreateActiveTask("move feed",
			new EmploymentActionPlan([
				new GetItemsByIdActionStep(1, [500], [source]),
				new DeliverItemsActionStep(destination)
			]),
			manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(host);
		context.SetAvailableItems(source, [item.Object]);
		var profile = Profile(employee, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanDeliverItems));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		host.Fire(contract, EmploymentTerminationReason.Fired, manager);

		Assert.AreEqual(EmploymentTaskStatus.Blocked, task.Status);
		Assert.AreSame(employee, task.AssignedEmployee);
		StringAssert.Contains(task.BlockedReason, "physical task items");
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.ActiveTaskBlocked &&
			x.CorrelationId == task.CorrelationId));
	}

	[TestMethod]
	public void TaskAssignmentAudit_BlocksWhenCarriedTaskItemIsMissingFromWorker()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("stable", currency.Object);
		var manager = Character(1, "Manager").Object;
		var employeeMock = Character(2, "Employee");
		var employee = employeeMock.Object;
		var source = Cell(31, "stockroom").Object;
		var destination = Cell(32, "yard").Object;
		var item = Item(102, "feed sack", prototypeId: 501);
		var body = new Mock<MudSharp.Body.IBody>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.TryGetItem(102, true)).Returns(item.Object);
		employeeMock.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		employeeMock.SetupGet(x => x.Body).Returns(body.Object);
		employeeMock.SetupGet(x => x.Inventory).Returns([]);
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes |
			EmploymentAuthority.HireEmployees), null);
		host.Hire(employee, Offer(currency.Object, EmploymentRole.Employee,
			EmploymentAuthority.ManageDeliveryRoutes), manager);
		var task = host.TaskBoard.CreateActiveTask("move feed",
			new EmploymentActionPlan([
				new GetItemsByIdActionStep(1, [501], [source]),
				new DeliverItemsActionStep(destination)
			]),
			manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(host);
		context.SetAvailableItems(source, [item.Object]);
		var profile = Profile(employee, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanDeliverItems));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		var audit = host.TaskBoard.AuditActiveTaskAssignments();

		Assert.AreEqual(EmploymentTaskStatus.Blocked, task.Status);
		Assert.AreEqual(1, audit.Count);
		StringAssert.Contains(task.BlockedReason, "no longer carrying");
	}

	[TestMethod]
	public void TaskAssignmentAudit_RequeuesWhenAssignedNpcNoLongerHasWorkerAI()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(1, "Manager").Object;
		var worker = NpcCharacter(2, "Worker").Object;
		var destination = Cell(30, "counter").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes |
			EmploymentAuthority.HireEmployees), null);
		host.Hire(worker, Offer(currency.Object, EmploymentRole.Employee,
			EmploymentAuthority.ManageDeliveryRoutes), manager);
		var task = host.TaskBoard.CreateActiveTask("walk to counter",
			new EmploymentActionPlan([new MovementDeliveryActionStep("walk to the counter", destination)]),
			manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(host);
		var profile = Profile(worker, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanDeliverItems));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		var audit = host.TaskBoard.AuditActiveTaskAssignments();

		Assert.AreEqual(EmploymentTaskStatus.Pending, task.Status);
		Assert.IsNull(task.AssignedEmployee);
		Assert.AreEqual(1, audit.Count);
		Assert.AreEqual(EmploymentTaskAssignmentAuditOutcome.Requeued, audit.Single().Outcome);
		StringAssert.Contains(audit.Single().Reason, "EmploymentWorkerAI");
	}

	[TestMethod]
	public void FinancialActionSteps_RecordLedgerRegisterAndExistingFinancialRecordReuse()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("bank", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.DepositBusinessCash |
			EmploymentAuthority.WithdrawBusinessCash |
			EmploymentAuthority.UseStoreAccount |
			EmploymentAuthority.ManageCraftRules |
			EmploymentAuthority.ManageDeliveryRoutes), null);
		var steps = new IEmploymentActionStep[]
		{
			new PurchaseActionStep("purchase supplies", new MoneyAmount(currency.Object, 5.0M), "shop-transaction-1"),
			new StoreAccountPaymentActionStep("linen supplier", new MoneyAmount(currency.Object, 2.0M), "store-account-1")
		};
		var context = new EmploymentTaskContext(host);
		foreach (var step in steps.Where(x => x.RequiresPaymentAuthorisation))
		{
			context.AuthorisePaymentFor(step);
		}

		var task = host.TaskBoard.CreateActiveTask("financial work", new EmploymentActionPlan(steps), manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(
				EmploymentAICapability.CanPurchaseCommodities,
				EmploymentAICapability.CanUseBankAccount,
				EmploymentAICapability.CanHandleCash,
				EmploymentAICapability.CanDeliverItems));
		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out _));

		while (task.Status is not EmploymentTaskStatus.Completed and not EmploymentTaskStatus.Failed)
		{
			dispatcher.AdvanceTask(task, context);
		}

		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		Assert.IsTrue(host.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.Purchase));
		Assert.IsTrue(host.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.StoreAccountPayment));
		Assert.IsTrue(host.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.ExistingFinancialRecordReuse));
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.PaymentAuthorisationUsed));
	}

	[TestMethod]
	public void BankAdministrationActionStep_ReserveMovementsMutateNativeReservesAndAudit()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var reserves = new DecimalCounter<ICurrency> { [currency.Object] = 100.0M };
		var (bank, state, audit) = BankHost(44, "Finance Bank", currency.Object, reserves);
		var manager = Character(1, "Manager").Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.DepositBusinessCash |
			EmploymentAuthority.WithdrawBusinessCash), null);
		VirtualCashLedger.Credit(bank.Object, currency.Object, 25.0M, null, null, "Seed", "Seed bank cash counterbalance");
		var task = state.TaskBoard.CreateActiveTask("bank reserve movement",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("authorise", "bank reserve movement", new MoneyAmount(currency.Object, 15.0M)),
				new BankAdministrationActionStep(bank.Object, BankAdministrationActionKind.ReserveDeposit,
					new MoneyAmount(currency.Object, 10.0M)),
				new BankAdministrationActionStep(bank.Object, BankAdministrationActionKind.ReserveWithdrawal,
					new MoneyAmount(currency.Object, 5.0M))
			]),
			manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(bank.Object);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanHandleCash));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		while (task.Status is not EmploymentTaskStatus.Completed and not EmploymentTaskStatus.Failed)
		{
			var result = dispatcher.AdvanceTask(task, context);
			Assert.IsTrue(result.Success, result.Message);
		}

		Assert.AreEqual(105.0M, bank.Object.CurrencyReserves[currency.Object]);
		Assert.AreEqual(20.0M, VirtualCashLedger.Balance(bank.Object, currency.Object));
		Assert.IsTrue(audit.Any(x => x.Contains("Deposited", StringComparison.OrdinalIgnoreCase)));
		Assert.IsTrue(audit.Any(x => x.Contains("Withdrew", StringComparison.OrdinalIgnoreCase)));
		Assert.IsTrue(state.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.BankDeposit));
		Assert.IsTrue(state.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.BankWithdrawal));
		Assert.AreEqual(2, state.EmploymentRegister.Entries.Count(x =>
			x.EntryType == EmploymentRegisterEntryType.PaymentAuthorisationUsed));
		StringAssert.Contains(task.StepOperationalStates[1].OperationalPayload!, "bankadmin:reserve-deposit");
		StringAssert.Contains(task.StepOperationalStates[2].OperationalPayload!, "bankadmin:reserve-withdrawal");
	}

	[TestMethod]
	public void BankAdministrationActionStep_AccountAndBranchActionsUseNativeBankAudit()
	{
		var currency = Currency();
		var account = BankAccount(currency.Object, 10.0M, accountNumber: 123, accountName: "customer ledger");
		var branchA = Cell(701, "north branch").Object;
		var branchB = Cell(702, "south branch").Object;
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>(), [branchA, branchB],
			bankAccountsToAdd: [account.Object]);
		var (bank, state, audit) = BankHost(45, "Branch Bank", currency.Object,
			accounts: [account.Object], branches: [branchA, branchB], gameworld: gameworld.Object);
		var manager = Character(2, "Manager", gameworld: gameworld.Object).Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.UseStoreAccount |
			EmploymentAuthority.ManageDeliveryRoutes), null);
		var task = state.TaskBoard.CreateActiveTask("bank account and branch work",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("authorise", "account correction", new MoneyAmount(currency.Object, 7.0M)),
				new BankAdministrationActionStep(bank.Object, BankAdministrationActionKind.AccountCredit,
					new MoneyAmount(currency.Object, 7.0M), account.Object.AccountReference, reason: "fee reversal"),
				new BankAdministrationActionStep(bank.Object, BankAdministrationActionKind.AccountStatus,
					accountSelector: account.Object.AccountReference, targetStatus: BankAccountStatus.Suspended, reason: "fraud review"),
				new BankAdministrationActionStep(bank.Object, BankAdministrationActionKind.AccountClose,
					accountSelector: account.Object.AccountReference, reason: "customer request"),
				new BankAdministrationActionStep(bank.Object, BankAdministrationActionKind.BranchPost,
					sourceBranch: branchA, reason: "teller roster posted"),
				new BankAdministrationActionStep(bank.Object, BankAdministrationActionKind.BranchCourier,
					sourceBranch: branchA, destinationBranch: branchB, reason: "sealed reserve bag logged")
			]),
			manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(bank.Object);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanUseBankAccount, EmploymentAICapability.CanDeliverItems));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		while (task.Status is not EmploymentTaskStatus.Completed and not EmploymentTaskStatus.Failed)
		{
			var result = dispatcher.AdvanceTask(task, context);
			Assert.IsTrue(result.Success, result.Message);
		}

		Assert.AreEqual(17.0M, account.Object.CurrentBalance);
		Assert.AreEqual(BankAccountStatus.Closed, account.Object.AccountStatus);
		account.Verify(x => x.DoAccountCredit(7.0M, "fee reversal"), Times.Once);
		account.Verify(x => x.SetStatus(BankAccountStatus.Suspended), Times.Once);
		account.Verify(x => x.SetStatus(BankAccountStatus.Closed), Times.Once);
		Assert.AreEqual(5, audit.Count);
		Assert.IsTrue(state.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.BankAccountCredit));
		Assert.AreEqual(5, state.EmploymentRegister.Entries.Count(x =>
			x.EntryType == EmploymentRegisterEntryType.AuditActionRecorded));
		StringAssert.Contains(task.StepOperationalStates[4].OperationalPayload!, "bankadmin:branch-post");
		StringAssert.Contains(task.StepOperationalStates[5].OperationalPayload!, "bankadmin:branch-courier");
	}

	[TestMethod]
	public void EmploymentFinanceSteps_ReserveAndMoveNativeBankAndVirtualCash()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var bankAccount = BankAccount(currency.Object, 25.0M, 1_000.0M);
		var (shop, state) = ShopHost(44, "Finance Shop", currency.Object, bankAccount.Object);
		var manager = Character(1, "Manager").Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.ManageStockRules |
			EmploymentAuthority.DepositBusinessCash |
			EmploymentAuthority.WithdrawBusinessCash), null);
		VirtualCashLedger.Credit(shop.Object, currency.Object, 20.0M, null, null, "Seed", "Seed balance");
		var task = state.TaskBoard.CreateActiveTask("bank movement",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("authorise", "bank movement", new MoneyAmount(currency.Object, 15.0M)),
				new CataloguedActionShellStep("reserve", "deposit float", new MoneyAmount(currency.Object, 10.0M)),
				new BankDepositActionStep(new MoneyAmount(currency.Object, 10.0M), "deposit-native-1"),
				new CataloguedActionShellStep("reserve", "withdraw operating cash", new MoneyAmount(currency.Object, 5.0M)),
				new BankWithdrawalActionStep(new MoneyAmount(currency.Object, 5.0M), "withdraw-native-1")
			]),
			manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(shop.Object);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanUseBankAccount, EmploymentAICapability.CanHandleCash));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		while (task.Status is not EmploymentTaskStatus.Completed and not EmploymentTaskStatus.Failed)
		{
			var result = dispatcher.AdvanceTask(task, context);
			Assert.IsTrue(result.Success, result.Message);
		}

		Assert.AreEqual(15.0M, VirtualCashLedger.Balance(shop.Object, currency.Object));
		Assert.AreEqual(30.0M, bankAccount.Object.CurrentBalance);
		Assert.AreEqual(1_005.0M, bankAccount.Object.Bank.CurrencyReserves[currency.Object]);
		Assert.IsTrue(state.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.PaymentAuthorisation));
		Assert.IsTrue(state.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.BankDeposit));
		Assert.IsTrue(state.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.BankWithdrawal));
		Assert.IsTrue(state.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.ExistingFinancialRecordReuse));
		Assert.IsTrue(task.StepOperationalStates[2].TransactionReference?.Contains("TST:123") == true);
		Assert.IsTrue(task.StepOperationalStates[2].ReservationReference?.Contains("op=consume") == true);
		Assert.IsTrue(task.StepOperationalStates[4].TransactionReference?.Contains("TST:123") == true);
	}

	[TestMethod]
	public void EmploymentFinanceSteps_TransfersBetweenNativeBankAccounts()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var sourceAccount = BankAccount(currency.Object, 25.0M, 1_000.0M, id: 901, accountNumber: 123,
			accountName: "operating");
		var targetAccount = BankAccount(currency.Object, 3.0M, 1_000.0M, id: 902, accountNumber: 456,
			accountName: "supplier");
		var (shop, state) = ShopHost(144, "Transfer Shop", currency.Object, sourceAccount.Object);
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>(),
			bankAccountsToAdd: [sourceAccount.Object, targetAccount.Object]);
		shop.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var manager = Character(1, "Manager", gameworld: gameworld.Object).Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.ManageStockRules |
			EmploymentAuthority.UseStoreAccount), null);
		var task = state.TaskBoard.CreateActiveTask("account transfer",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("authorise", "supplier transfer", new MoneyAmount(currency.Object, 10.0M)),
				new CataloguedActionShellStep("reserve", "supplier transfer", new MoneyAmount(currency.Object, 10.0M)),
				new BankAccountTransferActionStep(targetAccount.Object.AccountReference,
					new MoneyAmount(currency.Object, 10.0M), "transfer-native-1")
			]),
			manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(shop.Object);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanUseBankAccount));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		while (task.Status is not EmploymentTaskStatus.Completed and not EmploymentTaskStatus.Failed)
		{
			var result = dispatcher.AdvanceTask(task, context);
			Assert.IsTrue(result.Success, result.Message);
		}

		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		Assert.AreEqual(15.0M, sourceAccount.Object.CurrentBalance);
		Assert.AreEqual(13.0M, targetAccount.Object.CurrentBalance);
		sourceAccount.Verify(x => x.WithdrawFromTransfer(10.0M, "TST", 456,
			It.Is<string>(reference => reference.Contains("employment account transfer"))), Times.Once);
		targetAccount.Verify(x => x.DepositFromTransfer(10.0M, "TST", 123,
			It.Is<string>(reference => reference.Contains("employment account transfer"))), Times.Once);
		Assert.IsTrue(state.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.AccountTransfer));
		Assert.IsTrue(state.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.ExistingFinancialRecordReuse));
		Assert.IsTrue(state.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.PaymentAuthorisationUsed));
		Assert.IsTrue(task.StepOperationalStates[2].TransactionReference?.Contains("TST:123->TST:456") == true);
		Assert.IsTrue(task.StepOperationalStates[2].ReservationReference?.Contains("op=consume") == true);
	}

	[TestMethod]
	public void EmploymentFinanceSteps_BlocksTransferToDifferentCurrencyAccount()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var foreignCurrency = Currency(2, "foreign credits");
		var sourceAccount = BankAccount(currency.Object, 25.0M, 1_000.0M, id: 901, accountNumber: 123,
			accountName: "operating");
		var targetAccount = BankAccount(foreignCurrency.Object, 3.0M, 1_000.0M, id: 902, accountNumber: 456,
			bankCode: "FGN", accountName: "foreign supplier");
		var (shop, state) = ShopHost(145, "Currency Transfer Shop", currency.Object, sourceAccount.Object);
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>(),
			bankAccountsToAdd: [sourceAccount.Object, targetAccount.Object]);
		shop.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var manager = Character(1, "Manager", gameworld: gameworld.Object).Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.ManageStockRules |
			EmploymentAuthority.UseStoreAccount), null);
		var task = state.TaskBoard.CreateActiveTask("blocked account transfer",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("authorise", "supplier transfer", new MoneyAmount(currency.Object, 10.0M)),
				new CataloguedActionShellStep("reserve", "supplier transfer", new MoneyAmount(currency.Object, 10.0M)),
				new BankAccountTransferActionStep(targetAccount.Object.AccountReference,
					new MoneyAmount(currency.Object, 10.0M))
			]),
			manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(shop.Object);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanUseBankAccount));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		var transfer = AdvanceTaskOrAssignmentFailure(dispatcher, task, context, profile);

		Assert.IsFalse(transfer.Success);
		StringAssert.Contains(transfer.Message, "foreign credits");
		Assert.AreEqual(EmploymentTaskStatus.Blocked, task.Status);
		Assert.AreEqual(25.0M, sourceAccount.Object.CurrentBalance);
		Assert.AreEqual(3.0M, targetAccount.Object.CurrentBalance);
		Assert.IsFalse(state.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.AccountTransfer));
		sourceAccount.Verify(x => x.WithdrawFromTransfer(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<int>(),
			It.IsAny<string>()), Times.Never);
		targetAccount.Verify(x => x.DepositFromTransfer(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<int>(),
			It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void EmploymentFinanceSteps_BlocksTransferWhenGrantCounterpartyDoesNotMatch()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var sourceAccount = BankAccount(currency.Object, 25.0M, 1_000.0M, id: 911, accountNumber: 123,
			accountName: "operating");
		var allowedAccount = BankAccount(currency.Object, 3.0M, 1_000.0M, id: 912, accountNumber: 456,
			accountName: "approved supplier");
		var targetAccount = BankAccount(currency.Object, 7.0M, 1_000.0M, id: 913, accountNumber: 789,
			accountName: "unapproved supplier");
		var (shop, state) = ShopHost(150, "Scoped Transfer Shop", currency.Object, sourceAccount.Object);
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>(),
			bankAccountsToAdd: [sourceAccount.Object, allowedAccount.Object, targetAccount.Object]);
		shop.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var manager = Character(1, "Manager", gameworld: gameworld.Object).Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.ManageStockRules |
			EmploymentAuthority.UseStoreAccount), null);
		var actionPlan = new EmploymentActionPlan([
			new CataloguedActionShellStep("reserve", "supplier transfer", new MoneyAmount(currency.Object, 10.0M)),
			new BankAccountTransferActionStep(targetAccount.Object.AccountReference,
				new MoneyAmount(currency.Object, 10.0M))
		]);
		var correlationId = Guid.NewGuid();
		var provenance = ScopedFinancialProvenance(manager, actionPlan, correlationId, currency.Object, 10.0M,
			[$"host:shop:{shop.Object.Id}:linked-bank-account"],
			[$"bank-account:{allowedAccount.Object.AccountReference}"]);
		var task = state.TaskBoard.CreateActiveTask("blocked scoped transfer", actionPlan, manager, correlationId,
			provenance: provenance);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(shop.Object);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanUseBankAccount));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		var transfer = AdvanceTaskOrAssignmentFailure(dispatcher, task, context, profile);

		Assert.IsFalse(transfer.Success);
		StringAssert.Contains(transfer.Message, "requires an auditable payment authorisation");
		Assert.AreEqual(EmploymentTaskStatus.Blocked, task.Status);
		Assert.AreEqual(25.0M, sourceAccount.Object.CurrentBalance);
		Assert.AreEqual(7.0M, targetAccount.Object.CurrentBalance);
		sourceAccount.Verify(x => x.WithdrawFromTransfer(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<int>(),
			It.IsAny<string>()), Times.Never);
		targetAccount.Verify(x => x.DepositFromTransfer(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<int>(),
			It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void EmploymentFinanceSteps_SettlesFundsBetweenEmploymentHosts()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var (sourceShop, sourceState) = ShopHost(146, "Source Shop", currency.Object, null);
		var (targetShop, targetState) = ShopHost(147, "Target Shop", currency.Object, null);
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>(), shopsToAdd: [targetShop.Object]);
		sourceShop.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		targetShop.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var manager = Character(1, "Manager", gameworld: gameworld.Object).Object;
		sourceState.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.ManageStockRules |
			EmploymentAuthority.SettleHostAccounts), null);
		VirtualCashLedger.Credit(sourceShop.Object, currency.Object, 25.0M, null, null, "Seed", "Seed balance");
		var task = sourceState.TaskBoard.CreateActiveTask("host settlement",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("authorise", "host settlement", new MoneyAmount(currency.Object, 12.0M)),
				new CataloguedActionShellStep("reserve", "host settlement", new MoneyAmount(currency.Object, 12.0M)),
				new HostSettlementActionStep("shop:Target Shop", new MoneyAmount(currency.Object, 12.0M),
					"settlement-native-1")
			]),
			manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(sourceShop.Object);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanUseBankAccount));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		while (task.Status is not EmploymentTaskStatus.Completed and not EmploymentTaskStatus.Failed)
		{
			var result = dispatcher.AdvanceTask(task, context);
			Assert.IsTrue(result.Success, result.Message);
		}

		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		Assert.AreEqual(13.0M, VirtualCashLedger.Balance(sourceShop.Object, currency.Object));
		Assert.AreEqual(12.0M, VirtualCashLedger.Balance(targetShop.Object, currency.Object));
		Assert.IsTrue(sourceState.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.HostSettlement));
		Assert.IsTrue(targetState.BusinessLedger.Entries.Any(x =>
			x.EntryType == EmploymentLedgerEntryType.HostSettlement && x.CorrelationId == task.CorrelationId));
		Assert.IsTrue(sourceState.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.ExistingFinancialRecordReuse));
		Assert.IsTrue(sourceState.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.PaymentAuthorisationUsed));
		Assert.IsTrue(targetState.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.AuditActionRecorded && x.CorrelationId == task.CorrelationId));
		Assert.IsTrue(task.StepOperationalStates[2].TransactionReference?.Contains("Source Shop->Target Shop") == true);
		Assert.IsTrue(task.StepOperationalStates[2].ReservationReference?.Contains("op=consume") == true);
	}

	[TestMethod]
	public void EmploymentFinanceSteps_SettlesFundsToClanVirtualTreasury()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var clanBankAccount = BankAccount(currency.Object, 100.0M, id: 9101, accountNumber: 9101,
			accountName: "clan operating");
		var (sourceShop, sourceState) = ShopHost(246, "Source Shop", currency.Object, null);
		var (targetClan, targetState) = ClanHost(247, "Merchant League", currency.Object, clanBankAccount.Object);
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>(),
			clansToAdd: [targetClan.Object]);
		sourceShop.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		targetClan.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var manager = Character(1, "Manager", gameworld: gameworld.Object).Object;
		sourceState.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.ManageStockRules |
			EmploymentAuthority.SettleHostAccounts), null);
		VirtualCashLedger.Credit(sourceShop.Object, currency.Object, 25.0M, null, null, "Seed", "Seed balance");
		var task = sourceState.TaskBoard.CreateActiveTask("clan settlement",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("authorise", "clan settlement", new MoneyAmount(currency.Object, 12.0M)),
				new CataloguedActionShellStep("reserve", "clan settlement", new MoneyAmount(currency.Object, 12.0M)),
				new HostSettlementActionStep("clan:Merchant League", new MoneyAmount(currency.Object, 12.0M),
					"settlement-clan-1")
			]),
			manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(sourceShop.Object);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanUseBankAccount));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		while (task.Status is not EmploymentTaskStatus.Completed and not EmploymentTaskStatus.Failed)
		{
			if (task.AssignedEmployee is null)
			{
				Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out reason), reason);
			}

			var result = dispatcher.AdvanceTask(task, context);
			Assert.IsTrue(result.Success, result.Message);
		}

		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		Assert.AreEqual(13.0M, VirtualCashLedger.Balance(sourceShop.Object, currency.Object));
		Assert.AreEqual(12.0M, VirtualCashLedger.Balance(targetClan.Object, currency.Object));
		Assert.AreEqual(100.0M, clanBankAccount.Object.CurrentBalance);
		Assert.IsTrue(targetState.BusinessLedger.Entries.Any(x =>
			x.EntryType == EmploymentLedgerEntryType.HostSettlement && x.CorrelationId == task.CorrelationId));
		Assert.IsTrue(targetState.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.AuditActionRecorded && x.CorrelationId == task.CorrelationId));
		Assert.IsTrue(task.StepOperationalStates[2].TransactionReference?.Contains("Source Shop->Merchant League") == true);
	}

	[TestMethod]
	public void EmploymentFinanceSteps_BlocksSettlementToTemplateClan()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var clanBankAccount = BankAccount(currency.Object, 100.0M, id: 9102, accountNumber: 9102,
			accountName: "template operating");
		var (sourceShop, sourceState) = ShopHost(248, "Source Template Shop", currency.Object, null);
		var (templateClan, _) = ClanHost(249, "Template League", currency.Object, clanBankAccount.Object);
		templateClan.SetupGet(x => x.IsTemplate).Returns(true);
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>(),
			clansToAdd: [templateClan.Object]);
		sourceShop.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var manager = Character(1, "Manager", gameworld: gameworld.Object).Object;
		sourceState.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.ManageStockRules |
			EmploymentAuthority.SettleHostAccounts), null);
		VirtualCashLedger.Credit(sourceShop.Object, currency.Object, 25.0M, null, null, "Seed", "Seed balance");
		var task = sourceState.TaskBoard.CreateActiveTask("template clan settlement",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("authorise", "template settlement", new MoneyAmount(currency.Object, 12.0M)),
				new CataloguedActionShellStep("reserve", "template settlement", new MoneyAmount(currency.Object, 12.0M)),
				new HostSettlementActionStep("clan:Template League", new MoneyAmount(currency.Object, 12.0M),
					"settlement-template-clan-1")
			]),
			manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(sourceShop.Object);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanUseBankAccount));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		if (task.AssignedEmployee is null)
		{
			Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out reason), reason);
		}

		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		if (task.AssignedEmployee is null)
		{
			Assert.IsFalse(dispatcher.TryAssignTask(task, [profile], context, out reason));
			StringAssert.Contains(reason, "Clan templates cannot be used as employment settlement targets");
		}
		else
		{
			var result = dispatcher.AdvanceTask(task, context);
			Assert.IsFalse(result.Success);
			StringAssert.Contains(result.Message, "Clan templates cannot be used as employment settlement targets");
		}

		Assert.AreEqual(0.0M, VirtualCashLedger.Balance(templateClan.Object, currency.Object));
	}

	[TestMethod]
	public void EmploymentFinanceSteps_BlocksSettlementToDifferentCurrencyHost()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var foreignCurrency = Currency(2, "foreign credits");
		var (sourceShop, sourceState) = ShopHost(148, "Source Currency Shop", currency.Object, null);
		var (targetShop, targetState) = ShopHost(149, "Foreign Target Shop", foreignCurrency.Object, null);
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>(), shopsToAdd: [targetShop.Object]);
		sourceShop.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		targetShop.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var manager = Character(1, "Manager", gameworld: gameworld.Object).Object;
		sourceState.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.ManageStockRules |
			EmploymentAuthority.SettleHostAccounts), null);
		VirtualCashLedger.Credit(sourceShop.Object, currency.Object, 25.0M, null, null, "Seed", "Seed balance");
		var task = sourceState.TaskBoard.CreateActiveTask("blocked host settlement",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("authorise", "host settlement", new MoneyAmount(currency.Object, 12.0M)),
				new CataloguedActionShellStep("reserve", "host settlement", new MoneyAmount(currency.Object, 12.0M)),
				new HostSettlementActionStep("shop:Foreign Target Shop", new MoneyAmount(currency.Object, 12.0M))
			]),
			manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(sourceShop.Object);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanUseBankAccount));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		var settlement = AdvanceTaskOrAssignmentFailure(dispatcher, task, context, profile);

		Assert.IsFalse(settlement.Success);
		StringAssert.Contains(settlement.Message, "foreign credits");
		Assert.AreEqual(EmploymentTaskStatus.Blocked, task.Status);
		Assert.AreEqual(25.0M, VirtualCashLedger.Balance(sourceShop.Object, currency.Object));
		Assert.AreEqual(0.0M, VirtualCashLedger.Balance(targetShop.Object, foreignCurrency.Object));
		Assert.IsFalse(sourceState.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.HostSettlement));
		Assert.IsFalse(targetState.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.HostSettlement));
	}

	[TestMethod]
	public void EmploymentFinanceSteps_BlocksSettlementWhenGrantCounterpartyDoesNotMatch()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var (sourceShop, sourceState) = ShopHost(151, "Scoped Source Shop", currency.Object, null);
		var (allowedShop, allowedState) = ShopHost(152, "Approved Target Shop", currency.Object, null);
		var (targetShop, targetState) = ShopHost(153, "Unapproved Target Shop", currency.Object, null);
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>(),
			shopsToAdd: [allowedShop.Object, targetShop.Object]);
		sourceShop.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		allowedShop.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		targetShop.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var manager = Character(1, "Manager", gameworld: gameworld.Object).Object;
		sourceState.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.ManageStockRules |
			EmploymentAuthority.SettleHostAccounts), null);
		VirtualCashLedger.Credit(sourceShop.Object, currency.Object, 25.0M, null, null, "Seed", "Seed balance");
		var actionPlan = new EmploymentActionPlan([
			new CataloguedActionShellStep("reserve", "host settlement", new MoneyAmount(currency.Object, 12.0M)),
			new HostSettlementActionStep("shop:Unapproved Target Shop", new MoneyAmount(currency.Object, 12.0M))
		]);
		var correlationId = Guid.NewGuid();
		var provenance = ScopedFinancialProvenance(manager, actionPlan, correlationId, currency.Object, 12.0M,
			[$"host:shop:{sourceShop.Object.Id}:available-funds"],
			[$"employment-host:shop:{allowedShop.Object.Name}"]);
		var task = sourceState.TaskBoard.CreateActiveTask("blocked scoped settlement", actionPlan, manager,
			correlationId, provenance: provenance);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(sourceShop.Object);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanUseBankAccount));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		var settlement = AdvanceTaskOrAssignmentFailure(dispatcher, task, context, profile);

		Assert.IsFalse(settlement.Success);
		StringAssert.Contains(settlement.Message, "requires an auditable payment authorisation");
		Assert.AreEqual(EmploymentTaskStatus.Blocked, task.Status);
		Assert.AreEqual(25.0M, VirtualCashLedger.Balance(sourceShop.Object, currency.Object));
		Assert.AreEqual(0.0M, VirtualCashLedger.Balance(targetShop.Object, currency.Object));
		Assert.IsFalse(sourceState.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.HostSettlement));
		Assert.IsFalse(targetState.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.HostSettlement));
		Assert.IsFalse(allowedState.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.HostSettlement));
	}

	[TestMethod]
	public void EmploymentPurchaseByItemSelector_UsesExactStockSalePath()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var (shop, state) = ShopHost(45, "Exact Stock Shop", currency.Object, null);
		var actor = Character(2, "Purchaser");
		var gameworld = new Mock<IFuturemud>();
		var shops = new All<IShop> { shop.Object };
		gameworld.SetupGet(x => x.Shops).Returns(shops);
		shop.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(456);
		actor.SetupGet(x => x.Location).Returns(cell.Object);
		var proto = new Mock<IGameItemProto>();
		proto.SetupGet(x => x.Id).Returns(700);
		var merchandise = Merchandise(10, "matching goods");
		var selectedItem = new Mock<IGameItem>();
		selectedItem.SetupGet(x => x.Id).Returns(702);
		selectedItem.SetupGet(x => x.Name).Returns("selected item");
		selectedItem.SetupGet(x => x.Quantity).Returns(1);
		selectedItem.SetupGet(x => x.Prototype).Returns(proto.Object);
		shop.SetupGet(x => x.CurrentLocations).Returns(new[] { cell.Object });
		shop.SetupGet(x => x.Merchandises).Returns(new[] { merchandise.Object });
		shop.Setup(x => x.StockedItems(merchandise.Object)).Returns(new[] { selectedItem.Object });
		shop.Setup(x => x.PriceForMerchandise(It.IsAny<ICharacter?>(), merchandise.Object, 1)).Returns(7.0M);
		shop.Setup(x => x.CanBuyExact(
			    actor.Object,
			    merchandise.Object,
			    1,
			    It.IsAny<IPaymentMethod>(),
			    It.Is<IEnumerable<IGameItem>>(items => items.Single().Id == selectedItem.Object.Id)))
		    .Returns((true, string.Empty));
		shop.Setup(x => x.BuyExact(
			    actor.Object,
			    merchandise.Object,
			    1,
			    It.IsAny<IPaymentMethod>(),
			    It.Is<IEnumerable<IGameItem>>(items => items.Single().Id == selectedItem.Object.Id)))
		    .Returns(new[] { selectedItem.Object });
		state.Hire(actor.Object, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ApprovePurchases | EmploymentAuthority.ManageStockRules), null);
		VirtualCashLedger.Credit(shop.Object, currency.Object, 20.0M, null, null, "Seed", "Seed balance");
		var task = state.TaskBoard.CreateActiveTask("exact purchase",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("authorise", "buy exact stock", new MoneyAmount(currency.Object, 10.0M)),
				new CataloguedActionShellStep("reserve", "buy exact stock", new MoneyAmount(currency.Object, 7.0M)),
				new PurchaseActionStep(1, EmploymentItemSelector.ForPrototype(700), "any", currency.Object,
					new MoneyAmount(currency.Object, 10.0M))
			]), actor.Object);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(shop.Object);
		var profile = Profile(actor.Object, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanPurchaseCommodities));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		while (task.Status is not EmploymentTaskStatus.Completed and not EmploymentTaskStatus.Failed)
		{
			var result = dispatcher.AdvanceTask(task, context);
			Assert.IsTrue(result.Success, result.Message);
		}

		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		shop.Verify(x => x.BuyExact(
			actor.Object,
			merchandise.Object,
			1,
			It.IsAny<IPaymentMethod>(),
			It.Is<IEnumerable<IGameItem>>(items => items.Single().Id == selectedItem.Object.Id)), Times.Once);
		shop.Verify(x => x.Buy(actor.Object, merchandise.Object, 1, It.IsAny<IPaymentMethod>(), It.IsAny<string?>()),
			Times.Never);
	}

	[TestMethod]
	public void EmploymentPurchaseByCommoditySelector_UsesWeightedCommoditySalePath()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var (shop, state) = ShopHost(47, "Weighted Commodity Shop", currency.Object, null);
		var actor = Character(2, "Purchaser");
		var gameworld = new Mock<IFuturemud>();
		var shops = new All<IShop> { shop.Object };
		gameworld.SetupGet(x => x.Shops).Returns(shops);
		shop.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(457);
		actor.SetupGet(x => x.Location).Returns(cell.Object);
		var material = new Mock<ISolid>();
		material.SetupGet(x => x.Id).Returns(800);
		material.SetupGet(x => x.Name).Returns("iron");
		var commodity = new Mock<ICommodity>();
		commodity.SetupGet(x => x.Material).Returns(material.Object);
		commodity.SetupGet(x => x.Weight).Returns(5.0);
		commodity.SetupGet(x => x.Tag).Returns((ITag?)null);
		commodity.SetupGet(x => x.CommodityCharacteristics).Returns(new Dictionary<MudSharp.Form.Characteristics.ICharacteristicDefinition, MudSharp.Form.Characteristics.ICharacteristicValue>());
		var merchandise = Merchandise(11, "iron commodity");
		merchandise.SetupGet(x => x.MerchandiseType).Returns(MerchandiseType.Commodity);
		merchandise.SetupGet(x => x.CommodityMaterial).Returns(material.Object);
		merchandise.SetupGet(x => x.CommodityTag).Returns((ITag)null!);
		merchandise.SetupGet(x => x.CommodityCharacteristicRequirements).Returns(new Dictionary<MudSharp.Form.Characteristics.ICharacteristicDefinition, MudSharp.Form.Characteristics.ICharacteristicValue>());
		merchandise.SetupGet(x => x.CommodityPricingWeight).Returns(1.0);
		merchandise.Setup(x => x.IsMerchandiseForCommodity(commodity.Object)).Returns(true);
		var selectedItem = new Mock<IGameItem>();
		selectedItem.SetupGet(x => x.Id).Returns(703);
		selectedItem.SetupGet(x => x.Name).Returns("iron ingots");
		selectedItem.Setup(x => x.GetItemType<ICommodity>()).Returns(commodity.Object);
		var purchasedItem = new Mock<IGameItem>();
		purchasedItem.SetupGet(x => x.Id).Returns(704);
		purchasedItem.SetupGet(x => x.Name).Returns("split iron");
		shop.SetupGet(x => x.CurrentLocations).Returns(new[] { cell.Object });
		shop.SetupGet(x => x.Merchandises).Returns(new[] { merchandise.Object });
		shop.Setup(x => x.StockedItems(merchandise.Object)).Returns(new[] { selectedItem.Object });
		shop.Setup(x => x.PriceForMerchandiseWeight(It.IsAny<ICharacter?>(), merchandise.Object, 2.5)).Returns(6.0M);
		shop.Setup(x => x.CanBuyCommodityWeight(
			    actor.Object,
			    merchandise.Object,
			    2.5,
			    It.IsAny<IPaymentMethod>(),
			    It.Is<IEnumerable<IGameItem>>(items => items.Single().Id == selectedItem.Object.Id)))
		    .Returns((true, string.Empty));
		shop.Setup(x => x.BuyCommodityWeight(
			    actor.Object,
			    merchandise.Object,
			    2.5,
			    It.IsAny<IPaymentMethod>(),
			    It.Is<IEnumerable<IGameItem>>(items => items.Single().Id == selectedItem.Object.Id)))
		    .Returns(new[] { purchasedItem.Object });
		state.Hire(actor.Object, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ApprovePurchases | EmploymentAuthority.ManageStockRules), null);
		VirtualCashLedger.Credit(shop.Object, currency.Object, 20.0M, null, null, "Seed", "Seed balance");
		var task = state.TaskBoard.CreateActiveTask("weighted purchase",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("authorise", "buy weighted stock", new MoneyAmount(currency.Object, 10.0M)),
				new CataloguedActionShellStep("reserve", "buy weighted stock", new MoneyAmount(currency.Object, 6.0M)),
				new PurchaseActionStep(2.5, "iron", "any", currency.Object,
					new MoneyAmount(currency.Object, 10.0M))
			]), actor.Object);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(shop.Object);
		var profile = Profile(actor.Object, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanPurchaseCommodities));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		while (task.Status is not EmploymentTaskStatus.Completed and not EmploymentTaskStatus.Failed)
		{
			var result = dispatcher.AdvanceTask(task, context);
			Assert.IsTrue(result.Success, result.Message);
		}

		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		shop.Verify(x => x.BuyCommodityWeight(
			actor.Object,
			merchandise.Object,
			2.5,
			It.IsAny<IPaymentMethod>(),
			It.Is<IEnumerable<IGameItem>>(items => items.Single().Id == selectedItem.Object.Id)), Times.Once);
		shop.Verify(x => x.Buy(actor.Object, merchandise.Object, It.IsAny<int>(), It.IsAny<IPaymentMethod>(), It.IsAny<string?>()),
			Times.Never);
	}

	[TestMethod]
	public void EmploymentPurchaseByMerchandiseSelector_DoesNotCountPriceCommodityMerchandise()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var (shop, _) = ShopHost(48, "Commodity Shop", currency.Object, null);
		var actor = Character(2, "Purchaser");
		var gameworld = new Mock<IFuturemud>();
		var shops = new All<IShop> { shop.Object };
		gameworld.SetupGet(x => x.Shops).Returns(shops);
		shop.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var cell = Cell(458, "market").Object;
		actor.SetupGet(x => x.Location).Returns(cell);
		var merchandise = Merchandise(12, "iron commodity");
		merchandise.SetupGet(x => x.MerchandiseType).Returns(MerchandiseType.Commodity);
		shop.SetupGet(x => x.CurrentLocations).Returns(new[] { cell });
		shop.SetupGet(x => x.Merchandises).Returns(new[] { merchandise.Object });
		var purchase = new PurchaseActionStep(1, "iron commodity", "any", currency.Object,
			new MoneyAmount(currency.Object, 10.0M));
		var context = new EmploymentTaskContext(shop.Object);

		Assert.IsFalse(context.CanPurchase(purchase, out var reason));
		StringAssert.Contains(reason, "stock matching");
		shop.Verify(x => x.PriceForMerchandise(It.IsAny<ICharacter?>(), merchandise.Object, It.IsAny<int>()),
			Times.Never);
	}

	[TestMethod]
	public void EmploymentPurchaseRequiresEmployeeAtSupplierLocation()
	{
		var currency = Currency();
		var (shop, _) = ShopHost(49, "Local Shop", currency.Object, null);
		var actor = Character(2, "Purchaser");
		var gameworld = new Mock<IFuturemud>();
		var shops = new All<IShop> { shop.Object };
		gameworld.SetupGet(x => x.Shops).Returns(shops);
		shop.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var shopCell = Cell(459, "market").Object;
		var remoteCell = Cell(460, "warehouse").Object;
		actor.SetupGet(x => x.Location).Returns(remoteCell);
		shop.SetupGet(x => x.CurrentLocations).Returns(new[] { shopCell });
		var purchase = new PurchaseActionStep(1, "thread", "any", currency.Object,
			new MoneyAmount(currency.Object, 10.0M));
		var context = new EmploymentTaskContext(shop.Object);

		Assert.IsFalse(context.TryPurchase(actor.Object, purchase, out var reason, out _));
		StringAssert.Contains(reason, "current location");
	}

	[TestMethod]
	public void EmploymentFinanceSteps_BlockWhenAuthorisationAmountDoesNotCoverStep()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var bankAccount = BankAccount(currency.Object, 25.0M, 1_000.0M);
		var (shop, state) = ShopHost(46, "Under Authorised Shop", currency.Object, bankAccount.Object);
		var manager = Character(1, "Manager").Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.ManageStockRules |
			EmploymentAuthority.DepositBusinessCash), null);
		VirtualCashLedger.Credit(shop.Object, currency.Object, 20.0M, null, null, "Seed", "Seed balance");
		var task = state.TaskBoard.CreateActiveTask("under authorised deposit",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("authorise", "small deposit approval", new MoneyAmount(currency.Object, 1.0M)),
				new CataloguedActionShellStep("reserve", "deposit float", new MoneyAmount(currency.Object, 10.0M)),
				new BankDepositActionStep(new MoneyAmount(currency.Object, 10.0M))
			]),
			manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(shop.Object);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanUseBankAccount, EmploymentAICapability.CanHandleCash));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		var deposit = AdvanceTaskOrAssignmentFailure(dispatcher, task, context, profile);

		Assert.IsFalse(deposit.Success);
		StringAssert.Contains(deposit.Message, "requires an auditable payment authorisation");
		Assert.AreEqual(EmploymentTaskStatus.Blocked, task.Status);
		Assert.AreEqual(20.0M, VirtualCashLedger.Balance(shop.Object, currency.Object));
		Assert.AreEqual(25.0M, bankAccount.Object.CurrentBalance);
		Assert.IsFalse(state.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.BankDeposit));
	}

	[TestMethod]
	public void EmploymentFinanceSteps_BlockWithoutReservationOrSupportedFinanceHost()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		IEmploymentHost unsupportedHost = new TestEmploymentHost("bank", currency.Object);
		var manager = Character(1, "Manager").Object;
		unsupportedHost.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.DepositBusinessCash |
			EmploymentAuthority.ManageStockRules), null);
		var unsupportedTask = unsupportedHost.TaskBoard.CreateActiveTask("unsupported deposit",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("authorise", "deposit", new MoneyAmount(currency.Object, 5.0M)),
				new CataloguedActionShellStep("reserve", "deposit", new MoneyAmount(currency.Object, 5.0M)),
				new BankDepositActionStep(new MoneyAmount(currency.Object, 5.0M))
			]),
			manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(unsupportedHost);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanUseBankAccount, EmploymentAICapability.CanHandleCash));

		Assert.IsTrue(dispatcher.TryAssignTask(unsupportedTask, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(unsupportedTask, context).Success);
		var unsupportedReserve = AdvanceTaskOrAssignmentFailure(dispatcher, unsupportedTask, context, profile);
		Assert.IsFalse(unsupportedReserve.Success);
		StringAssert.Contains(unsupportedReserve.Message, "finance adapter");

		var (shop, state) = ShopHost(45, "No Reserve Shop", currency.Object, BankAccount(currency.Object, 10.0M).Object);
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.DepositBusinessCash), null);
		VirtualCashLedger.Credit(shop.Object, currency.Object, 10.0M, null, null, "Seed", "Seed balance");
		var unreservedTask = state.TaskBoard.CreateActiveTask("unreserved deposit",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("authorise", "deposit", new MoneyAmount(currency.Object, 5.0M)),
				new BankDepositActionStep(new MoneyAmount(currency.Object, 5.0M))
			]),
			manager);
		var shopContext = new EmploymentTaskContext(shop.Object);

		Assert.IsTrue(dispatcher.TryAssignTask(unreservedTask, [profile], shopContext, out reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(unreservedTask, shopContext).Success);
		var deposit = AdvanceTaskOrAssignmentFailure(dispatcher, unreservedTask, shopContext, profile);

		Assert.IsFalse(deposit.Success);
		StringAssert.Contains(deposit.Message, "reserved");
		Assert.AreEqual(10.0M, VirtualCashLedger.Balance(shop.Object, currency.Object));
	}

	[TestMethod]
	public void GetItemsByIdActionStep_CollectsMatchingItemsFromSourceLocations()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var sourceOne = Cell(10, "stock room").Object;
		var sourceTwo = Cell(11, "warehouse").Object;
		var requestedOne = Item(100, "first crate", prototypeId: 9001).Object;
		var ignored = Item(101, "wrong crate", prototypeId: 9002).Object;
		var requestedTwo = Item(102, "second crate", prototypeId: 9001).Object;
		var context = new EmploymentTaskContext(host);
		context.SetAvailableItems(sourceOne, [requestedOne, ignored]);
		context.SetAvailableItems(sourceTwo, [requestedTwo]);
		var step = new GetItemsByIdActionStep(2, [requestedOne.Prototype.Id], [sourceOne, sourceTwo]);
		var task = host.TaskBoard.CreateActiveTask("collect crates", new EmploymentActionPlan([step]), manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanDeliverItems));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out _));
		var result = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(result.Success);
		CollectionAssert.AreEquivalent(new[] { requestedOne.Id, requestedTwo.Id }, context.CarriedTaskItems(manager).Select(x => x.Id).ToArray());
		CollectionAssert.DoesNotContain(context.AvailableItems(sourceOne).Select(x => x.Id).ToArray(), requestedOne.Id);
		CollectionAssert.DoesNotContain(context.AvailableItems(sourceTwo).Select(x => x.Id).ToArray(), requestedTwo.Id);
		var completionEntries = host.EmploymentRegister.Entries.Where(x =>
			x.EntryType == EmploymentRegisterEntryType.ActionStepCompleted &&
			x.Description.Contains("Collected", StringComparison.InvariantCultureIgnoreCase)).ToList();
		Assert.AreEqual(1, completionEntries.Count);
		Assert.AreEqual(task.CorrelationId, completionEntries.Single().CorrelationId);
	}

	[TestMethod]
	public void GetItemsByIdActionStep_RecordsActualPostCollectionCustodyForTransportBundles()
	{
		var manager = Character(1, "Manager").Object;
		var source = Cell(12, "stock room").Object;
		var requestedOne = Item(201, "first crate", prototypeId: 9001).Object;
		var requestedTwo = Item(202, "second crate", prototypeId: 9001).Object;
		var transportBundle = Item(299, "transport bundle", prototypeId: 9900).Object;
		var collected = false;
		var context = new Mock<IEmploymentTaskContext>();
		context.Setup(x => x.CanPath(manager, source)).Returns(true);
		context.Setup(x => x.AvailableItems(source)).Returns([requestedOne, requestedTwo]);
		context.Setup(x => x.CarriedTaskItems(manager))
		       .Returns(() => collected ? [transportBundle] : []);
		var collectReason = string.Empty;
		context.Setup(x => x.TryCollectTaskItems(
			       manager,
			       It.IsAny<IReadOnlyCollection<(IGameItem Item, ICell Source)>>(),
			       out collectReason))
		       .Returns(new TryCollectTaskItemsCallback((ICharacter _, IReadOnlyCollection<(IGameItem Item, ICell Source)> _, out string reason) =>
		       {
			       collected = true;
			       reason = string.Empty;
			       return true;
		       }));
		var step = new GetItemsByIdActionStep(2, [requestedOne.Prototype.Id], [source]);

		var result = step.Execute(context.Object, manager);

		Assert.IsTrue(result.Success, result.Message);
		Assert.IsNotNull(result.OperationalState);
		StringAssert.Contains(result.OperationalState.SelectedResources, "items=299");
		StringAssert.Contains(result.OperationalState.SelectedResources, "bundles=299");
		Assert.IsFalse(result.OperationalState.SelectedResources?.Contains("items=201,202") == true);
	}

	[TestMethod]
	public void GetItemsByIdActionStep_CollectsSpecificItemIdsWhenRequested()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var source = Cell(12, "stock room").Object;
		var requested = Item(103, "first crate", prototypeId: 9001).Object;
		var ignoredSamePrototype = Item(104, "other crate", prototypeId: 9001).Object;
		var context = new EmploymentTaskContext(host);
		context.SetAvailableItems(source, [requested, ignoredSamePrototype]);
		var step = new GetItemsByIdActionStep(1, [], [source], [requested.Id]);
		var task = host.TaskBoard.CreateActiveTask("collect crate", new EmploymentActionPlan([step]), manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanDeliverItems));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out _));
		var result = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(result.Success);
		CollectionAssert.AreEquivalent(new[] { requested.Id }, context.CarriedTaskItems(manager).Select(x => x.Id).ToArray());
		CollectionAssert.Contains(context.AvailableItems(source).Select(x => x.Id).ToArray(), ignoredSamePrototype.Id);
	}

	[TestMethod]
	public void GetItemsByIdActionStep_RequiresEnoughMatchingPrototypeInstances()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var source = Cell(12, "stock room").Object;
		var requested = Item(103, "first crate", prototypeId: 9001).Object;
		var ignored = Item(104, "wrong crate", prototypeId: 9002).Object;
		var context = new EmploymentTaskContext(host);
		context.SetAvailableItems(source, [requested, ignored]);
		var step = new GetItemsByIdActionStep(2, [requested.Prototype.Id], [source]);

		Assert.IsFalse(step.CanExecute(context, manager, out var reason));
		StringAssert.Contains(reason, "only 1 matching item");
		StringAssert.Contains(reason, "2 requested");
	}

	[TestMethod]
	public void GetItemsByTagActionStep_CollectsTaggedItemsOnly()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("stable", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var source = Cell(20, "feed store").Object;
		var taggedOne = Item(200, "oat sack").Object;
		var taggedTwo = Item(201, "second oat sack").Object;
		var ignored = Item(202, "linen sack").Object;
		var context = new EmploymentTaskContext(host);
		context.SetAvailableItems(source, [taggedOne, ignored, taggedTwo]);
		context.SetItemTags(taggedOne, "feed");
		context.SetItemTags(taggedTwo, "feed");
		context.SetItemTags(ignored, "linen");
		var step = new GetItemsByTagActionStep(2, "feed", [source]);
		var task = host.TaskBoard.CreateActiveTask("collect feed", new EmploymentActionPlan([step]), manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanDeliverItems));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out _));
		var result = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(result.Success);
		CollectionAssert.AreEquivalent(new[] { taggedOne.Id, taggedTwo.Id }, context.CarriedTaskItems(manager).Select(x => x.Id).ToArray());
		CollectionAssert.Contains(context.AvailableItems(source).Select(x => x.Id).ToArray(), ignored.Id);
	}

	[TestMethod]
	public void GetCommodityActionStep_CollectsRequiredMaterialWeight()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("auction house", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var source = Cell(30, "materials store").Object;
		var ironOne = Item(300, "iron bundle").Object;
		var ironTwo = Item(301, "second iron bundle").Object;
		var copper = Item(302, "copper bundle").Object;
		var context = new EmploymentTaskContext(host);
		context.SetAvailableItems(source, [ironOne, copper, ironTwo]);
		context.SetCommodityWeight(ironOne, "iron", 2.5, "ingot", new Dictionary<string, string> { ["grade"] = "refined" });
		context.SetCommodityWeight(ironTwo, "iron", 4.0, "ingot", new Dictionary<string, string> { ["grade"] = "refined" });
		context.SetCommodityWeight(copper, "copper", 8.0, "ingot", new Dictionary<string, string> { ["grade"] = "refined" });
		var step = new GetCommodityActionStep(6.0, "iron", "ingot",
			new Dictionary<string, string> { ["grade"] = "refined" }, [source]);
		var task = host.TaskBoard.CreateActiveTask("collect iron", new EmploymentActionPlan([step]), manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanDeliverItems));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out _));
		var result = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(result.Success);
		CollectionAssert.AreEquivalent(new[] { ironOne.Id, ironTwo.Id }, context.CarriedTaskItems(manager).Select(x => x.Id).ToArray());
		CollectionAssert.Contains(context.AvailableItems(source).Select(x => x.Id).ToArray(), copper.Id);
	}

	[TestMethod]
	public void DeliverItemsActionStep_DeliversCarriedTaskItemsToContainerTag()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("hotel", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var source = Cell(40, "linen store").Object;
		var destination = Cell(41, "laundry").Object;
		var linenOne = Item(400, "linen bundle").Object;
		var linenTwo = Item(401, "second linen bundle").Object;
		var hamperContents = new List<IGameItem>();
		var hamper = ContainerItem(402, "clean hamper", [destination], [], hamperContents).Object;
		var context = new EmploymentTaskContext(host);
		context.SetAvailableItems(source, [linenOne, linenTwo]);
		context.SetAvailableItems(destination, [hamper]);
		context.SetItemTags(linenOne, "linen");
		context.SetItemTags(linenTwo, "linen");
		context.SetItemTags(hamper, "clean-linen-container");
		var plan = new EmploymentActionPlan([
			new GetItemsByTagActionStep(2, "linen", [source]),
			new DeliverItemsActionStep(destination, containerTag: "clean-linen-container")
		]);
		var task = host.TaskBoard.CreateActiveTask("move linen", plan, manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanDeliverItems));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out _));
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);

		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		Assert.AreEqual(0, context.CarriedTaskItems(manager).Count);
		CollectionAssert.AreEquivalent(new[] { linenOne.Id, linenTwo.Id }, context.ContainedItems(hamper).Select(x => x.Id).ToArray());
	}

	[TestMethod]
	public void LoadAndUnloadActionSteps_TrackLoadedTaskItems()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("stable", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var source = Cell(45, "tack room").Object;
		var crateLocation = Cell(46, "wagon bay").Object;
		var saddle = Item(450, "saddle").Object;
		var bridle = Item(451, "bridle").Object;
		var crateContents = new List<IGameItem>();
		var crate = ContainerItem(452, "cargo crate", [crateLocation], [], crateContents).Object;
		var context = new EmploymentTaskContext(host);
		context.SetAvailableItems(source, [saddle, bridle]);
		context.SetAvailableItems(crateLocation, [crate]);
		context.SetItemTags(saddle, "tack");
		context.SetItemTags(bridle, "tack");
		context.SetItemTags(crate, "cargo");
		var plan = new EmploymentActionPlan([
			new GetItemsByTagActionStep(2, "tack", [source]),
			new LoadItemsActionStep(EmploymentItemSelector.ForPrototype(crate.Prototype.Id), crateLocation),
			new UnloadItemsActionStep(EmploymentItemSelector.ForTag("cargo"), crateLocation)
		]);
		var task = host.TaskBoard.CreateActiveTask("load tack", plan, manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanDeliverItems));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		var load = dispatcher.AdvanceTask(task, context);
		Assert.IsTrue(load.Success);
		Assert.AreEqual(0, context.CarriedTaskItems(manager).Count);
		CollectionAssert.AreEquivalent(new[] { saddle.Id, bridle.Id }, context.LoadedTaskItems(manager, crate).Select(x => x.Id).ToArray());
		StringAssert.Contains(task.StepOperationalStates[1].LoadedAssets, $"container={crate.Id}");
		StringAssert.Contains(task.StepOperationalStates[1].ReservationReference, "op=reserve");
		StringAssert.Contains(task.StepOperationalStates[1].ReservationReference, $"container={crate.Id}");
		StringAssert.Contains(task.StepOperationalStates[1].ReservationReference, "count=2");

		var unload = dispatcher.AdvanceTask(task, context);
		Assert.IsTrue(unload.Success);
		StringAssert.Contains(task.StepOperationalStates[2].ReservationReference, "op=consume");
		StringAssert.Contains(task.StepOperationalStates[2].ReservationReference, $"container={crate.Id}");
		StringAssert.Contains(task.StepOperationalStates[2].ReservationReference, "count=2");
		CollectionAssert.AreEquivalent(new[] { saddle.Id, bridle.Id }, context.CarriedTaskItems(manager).Select(x => x.Id).ToArray());
		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
	}

	[TestMethod]
	public void TaskBoard_CreateActiveTaskRequiresActionPlanAuthority()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(1, "Manager").Object;
		var destination = Cell(50, "shopfront").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthority.AssignTasks), null);

		Assert.ThrowsException<InvalidOperationException>(() =>
			host.TaskBoard.CreateActiveTask("move stock",
				new EmploymentActionPlan([new DeliverItemsActionStep(destination)]),
				manager));
	}

	[TestMethod]
	public void ShopEmploymentTaskService_CreatesAndExecutesStockroomRestockMovement()
	{
		var currency = Currency();
		var manager = Character(1, "Manager").Object;
		var stockroomItems = new List<IGameItem>();
		var shopfrontItems = new List<IGameItem>();
		var stockroom = PhysicalCell(60, "stockroom", stockroomItems).Object;
		var shopfront = PhysicalCell(61, "shopfront", shopfrontItems).Object;
		var movedOne = Item(600, "linen bundle", trueLocations: [stockroom]).Object;
		var movedTwo = Item(601, "second linen bundle", trueLocations: [stockroom]).Object;
		stockroomItems.AddRange([movedOne, movedTwo]);
		var containerContents = new List<IGameItem>();
		var displayTag = Tag(700, "display-basket").Object;
		var container = ContainerItem(602, "display basket", [shopfront], [displayTag], containerContents).Object;
		shopfrontItems.Add(container);
		var merchandise = Merchandise(800, "linen");
		var shop = PermanentShop(900, "linen shop", currency.Object, manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes,
			stockroom, [shopfront], [container], [merchandise.Object], [movedOne, movedTwo]);
		var service = new ShopEmploymentTaskService();

		var created = service.TryCreateStockroomRestockTask(manager, shop.Shop.Object, merchandise.Object, 2,
			out var task, out var message, containerTag: "display-basket");

		Assert.IsTrue(created, message);
		Assert.IsNotNull(task);
		Assert.AreEqual(2, task.ActionPlan.Steps.Count);
		Assert.AreEqual(EmploymentActionStepType.GetItemsById, task.ActionPlan.Steps[0].StepType);
		Assert.AreEqual(EmploymentActionStepType.DeliverItems, task.ActionPlan.Steps[1].StepType);
		Assert.IsFalse(task.ActionPlan.Steps.Any(x => x.StepType == EmploymentActionStepType.BoardPost));

		var dispatcher = new EmploymentTaskDispatcher();
		var context = service.CreatePhysicalContext(shop.Shop.Object);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanDeliverItems));
		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);

		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		Assert.AreEqual(0, stockroomItems.Count);
		CollectionAssert.AreEquivalent(new[] { movedOne.Id, movedTwo.Id }, context.ContainedItems(container).Select(x => x.Id).ToArray());
		Assert.AreEqual(0, shop.Shop.Object.Board.Posts.Count());
		Assert.IsTrue(shop.State.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ActiveTaskCreated));
		Assert.IsTrue(shop.State.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ActiveTaskCompleted));
	}

	[TestMethod]
	public void ShopEmploymentTaskService_CreatesExternalStockTransferWithPairedEvidence()
	{
		var currency = Currency();
		var managerMock = Character(1, "Manager");
		var manager = managerMock.Object;
		var sourceStockroomItems = new List<IGameItem>();
		var sourceShopfrontItems = new List<IGameItem>();
		var targetStockroomItems = new List<IGameItem>();
		var targetShopfrontItems = new List<IGameItem>();
		var sourceStockroom = PhysicalCell(80, "source stockroom", sourceStockroomItems).Object;
		var sourceShopfront = PhysicalCell(81, "source shopfront", sourceShopfrontItems).Object;
		var targetStockroom = PhysicalCell(82, "target stockroom", targetStockroomItems).Object;
		var targetShopfront = PhysicalCell(83, "target shopfront", targetShopfrontItems).Object;
		var targetContents = new List<IGameItem>();
		var targetContainer = ContainerItem(804, "transfer crate", [targetStockroom], [], targetContents).Object;
		targetStockroomItems.Add(targetContainer);
		var movedItemMock = Item(800, "linen bundle", trueLocations: [sourceStockroom], prototypeId: 9000);
		var movedItem = movedItemMock.Object;
		sourceStockroomItems.Add(movedItem);
		var sourceMerchandise = Merchandise(801, "source linen");
		var targetMerchandise = Merchandise(802, "target linen");
		targetMerchandise.Setup(x => x.IsMerchandiseFor(movedItem, true)).Returns(true);
		var source = PermanentShop(900, "source shop", currency.Object, manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes,
			sourceStockroom, [sourceShopfront], [], [sourceMerchandise.Object], [movedItem]);
		var target = PermanentShop(901, "target shop", currency.Object, manager,
			EmploymentAuthority.ManageDeliveryRoutes,
			targetStockroom, [targetShopfront], [targetContainer], [targetMerchandise.Object], []);
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter> { [manager.Id] = manager },
			[sourceStockroom, sourceShopfront, targetStockroom, targetShopfront],
			new Dictionary<long, IGameItem> { [movedItem.Id] = movedItem, [targetContainer.Id] = targetContainer },
			shopsToAdd: [source.Shop.Object, target.Shop.Object]);
		managerMock.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		movedItemMock.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var stockEffects = new List<ItemOnDisplayInShop>
		{
			new(movedItem, source.Shop.Object, sourceMerchandise.Object)
		};
		movedItemMock.Setup(x => x.EffectsOfType<ItemOnDisplayInShop>()).Returns(() => stockEffects.ToList());
		source.Shop.Setup(x => x.DisposeFromStock(manager, movedItem))
		      .Callback(() => stockEffects.RemoveAll(x => x.Shop?.Id == source.Shop.Object.Id));
		target.Shop.Setup(x => x.AddToStock(manager, movedItem, targetMerchandise.Object))
		      .Callback(() => stockEffects.Add(new ItemOnDisplayInShop(movedItem, target.Shop.Object,
			      targetMerchandise.Object)));
		var service = new ShopEmploymentTaskService();

		var created = service.TryCreateStockTransferTask(manager, source.Shop.Object, sourceMerchandise.Object, 1,
			target.Shop.Object, targetMerchandise.Object, out var task, out var message, targetStockroom,
			targetContainer);

		Assert.IsTrue(created, message);
		Assert.IsNotNull(task);
		Assert.AreEqual(EmploymentActionStepType.GetItemsById, task.ActionPlan.Steps[0].StepType);
		Assert.AreEqual(EmploymentActionStepType.ShopStockTransfer, task.ActionPlan.Steps[1].StepType);
		Assert.IsTrue(target.State.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.ActiveTaskCreated && x.CorrelationId == task.CorrelationId));

		var dispatcher = new EmploymentTaskDispatcher();
		var context = service.CreatePhysicalTransferContext(source.Shop.Object, target.Shop.Object);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanDeliverItems));
		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);

		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		Assert.AreEqual(0, sourceStockroomItems.Count);
		CollectionAssert.AreEquivalent(new[] { movedItem.Id }, context.ContainedItems(targetContainer).Select(x => x.Id).ToArray());
		source.Shop.Verify(x => x.DisposeFromStock(manager, movedItem), Times.Once);
		target.Shop.Verify(x => x.AddToStock(manager, movedItem, targetMerchandise.Object), Times.Once);
		Assert.IsFalse(stockEffects.Any(x => x.Shop?.Id == source.Shop.Object.Id));
		Assert.IsTrue(stockEffects.Any(x => x.Shop?.Id == target.Shop.Object.Id &&
		                                x.Merchandise?.Id == targetMerchandise.Object.Id));
		Assert.IsTrue(source.State.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.AuditActionRecorded && x.CorrelationId == task.CorrelationId));
		Assert.IsTrue(target.State.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.AuditActionRecorded && x.CorrelationId == task.CorrelationId));
		StringAssert.Contains(task.StepOperationalStates[1].OperationalPayload!, "stocktransfer:source=900;target=901");
	}

	[TestMethod]
	public void PhysicalEmploymentLogistics_RejectsItemsOutsideHostLocations()
	{
		var currency = Currency();
		var manager = Character(1, "Manager").Object;
		var stockroomItems = new List<IGameItem>();
		var shopfrontItems = new List<IGameItem>();
		var externalItems = new List<IGameItem>();
		var stockroom = PhysicalCell(60, "stockroom", stockroomItems).Object;
		var shopfront = PhysicalCell(61, "shopfront", shopfrontItems).Object;
		var externalCell = PhysicalCell(62, "private room", externalItems).Object;
		var item = Item(610, "private ledger", trueLocations: [externalCell]).Object;
		externalItems.Add(item);
		var merchandise = Merchandise(800, "linen");
		var shop = PermanentShop(900, "linen shop", currency.Object, manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes,
			stockroom, [shopfront], [], [merchandise.Object], []);
		var context = new ShopEmploymentTaskService().CreatePhysicalContext(shop.Shop.Object);

		Assert.IsFalse(context.TryCollectTaskItem(manager, item, externalCell, out var reason));
		StringAssert.Contains(reason, "assigned work locations");
		Assert.AreEqual(1, externalItems.Count);
		Assert.IsFalse(context.AvailableItems(externalCell).Any());
	}

	[TestMethod]
	public void ShopEmploymentTaskService_RequiresDelegatedDeliveryAuthority()
	{
		var currency = Currency();
		var manager = Character(1, "Manager").Object;
		var stockroomItems = new List<IGameItem>();
		var shopfrontItems = new List<IGameItem>();
		var stockroom = PhysicalCell(70, "stockroom", stockroomItems).Object;
		var shopfront = PhysicalCell(71, "shopfront", shopfrontItems).Object;
		var item = Item(700, "linen bundle", trueLocations: [stockroom]).Object;
		stockroomItems.Add(item);
		var merchandise = Merchandise(801, "linen");
		var shop = PermanentShop(901, "linen shop", currency.Object, manager, EmploymentAuthority.AssignTasks,
			stockroom, [shopfront], [], [merchandise.Object], [item]);
		var service = new ShopEmploymentTaskService();

		var created = service.TryCreateStockroomRestockTask(manager, shop.Shop.Object, merchandise.Object, 1,
			out var task, out var message);

		Assert.IsFalse(created);
		Assert.IsNull(task);
		StringAssert.Contains(message, "delegated authority");
		Assert.AreEqual(0, shop.Shop.Object.TaskBoard.ActiveTasks.Count);
	}

	[TestMethod]
	public void ManagerGoals_CreateTasksOnlyWhenManagerHasRequiredAuthority()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("hotel", currency.Object);
		var manager = Character(1, "Manager").Object;
		var limited = Character(2, "Limited").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateManagerGoals | EmploymentAuthority.PostToHostBoard), null);
		host.Hire(limited, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthority.CreateManagerGoals), null);
		var definition = new ManagerGoalDefinition(
			ManagerGoalType.MaintainHotelOperations,
			EmploymentAuthority.PostToHostBoard,
			new ManagerGoalConfiguration("post room notice", new EmploymentActionPlan([
				new BoardPostActionStep("Rooms", "Room work required.")
			])),
			1,
			TimeSpan.Zero);

		Assert.ThrowsException<InvalidOperationException>(() => host.ManagerGoalBoard.CreateGoal(definition, limited));
		host.ManagerGoalBoard.CreateGoal(definition, manager);
		var tasks = host.ManagerGoalBoard.EvaluateGoals(new EmploymentTaskContext(host), DateTimeOffset.UtcNow);

		Assert.AreEqual(1, tasks.Count);
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ManagerGoalEvaluated));
	}

	[TestMethod]
	public void ManagerGoals_EvaluateByPriorityAndStampSpawnedTaskProvenance()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("hotel", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateManagerGoals | EmploymentAuthority.PostToHostBoard), null);
		var low = host.ManagerGoalBoard.CreateGoal(new ManagerGoalDefinition(
			ManagerGoalType.MaintainHotelOperations,
			EmploymentAuthority.PostToHostBoard,
			new ManagerGoalConfiguration("a low priority notice", new EmploymentActionPlan([
				new BoardPostActionStep("Rooms", "Low priority work.")
			])),
			1,
			TimeSpan.Zero), manager);
		var high = host.ManagerGoalBoard.CreateGoal(new ManagerGoalDefinition(
			ManagerGoalType.MaintainHotelOperations,
			EmploymentAuthority.PostToHostBoard,
			new ManagerGoalConfiguration("z high priority notice", new EmploymentActionPlan([
				new BoardPostActionStep("Rooms", "High priority work.")
			])),
			9,
			TimeSpan.Zero), manager);

		var tasks = host.ManagerGoalBoard.EvaluateGoals(new EmploymentTaskContext(host), DateTimeOffset.UtcNow).ToList();

		Assert.AreEqual(2, tasks.Count);
		Assert.AreEqual("z high priority notice", tasks[0].Name);
		Assert.AreEqual("a low priority notice", tasks[1].Name);
		Assert.AreEqual(9, tasks[0].Priority);
		Assert.AreEqual(1, tasks[1].Priority);
		Assert.AreEqual(EmploymentTaskSourceKind.ManagerGoal, tasks[0].SourceKind);
		Assert.AreEqual(high.Id, tasks[0].SourceGoalId);
		Assert.AreEqual(EmploymentPrincipalKind.ManagerGoal, tasks[0].CreatedByPrincipal.Kind);
		Assert.AreEqual(EmploymentPrincipalKind.ManagerGoal, tasks[0].AuthorisedByPrincipal.Kind);
		Assert.AreEqual(EmploymentPrincipalKind.ManagerGoal, tasks[0].AuthorisationGrant.IssuerPrincipal.Kind);
		Assert.AreEqual(high.CorrelationId, tasks[0].AuthorisationGrant.CorrelationId);
		Assert.AreEqual(low.Id, tasks[1].SourceGoalId);
	}

	[TestMethod]
	public void ManagerGoals_CannotUnderDeclareActionPlanAuthority()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("hotel", currency.Object);
		var limited = Character(2, "Limited").Object;
		var destination = Cell(90, "storeroom").Object;
		host.Hire(limited, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthority.CreateManagerGoals), null);
		var definition = new ManagerGoalDefinition(
			ManagerGoalType.MaintainHotelOperations,
			EmploymentAuthority.None,
			new ManagerGoalConfiguration("move room stock", new EmploymentActionPlan([
				new DeliverItemsActionStep(destination)
			])),
			1,
			TimeSpan.Zero);

		Assert.ThrowsException<InvalidOperationException>(() => host.ManagerGoalBoard.CreateGoal(definition, limited));
	}

	[TestMethod]
	public void ManagerGoals_RejectBudgetPolicyExceededAtCreation()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("hotel", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateManagerGoals |
			EmploymentAuthority.DepositBusinessCash), null);
		var definition = new ManagerGoalDefinition(
			ManagerGoalType.MaintainHotelOperations,
			EmploymentAuthority.DepositBusinessCash,
			new ManagerGoalConfiguration("deposit float", new EmploymentActionPlan([
				new BankDepositActionStep(new MoneyAmount(currency.Object, 15.0M))
			])),
			1,
			TimeSpan.Zero,
			new ManagerGoalPolicy([new MoneyAmount(currency.Object, 10.0M)]));

		var exception = Assert.ThrowsException<InvalidOperationException>(() =>
			host.ManagerGoalBoard.CreateGoal(definition, manager));

		StringAssert.Contains(exception.Message, "budget");
		Assert.AreEqual(0, host.ManagerGoalBoard.Goals.Count);
	}

	[TestMethod]
	public void ManagerGoals_RejectActionPlanExceedingStepRiskLimit()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("hotel", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateManagerGoals |
			EmploymentAuthority.PostToHostBoard), null);
		var definition = new ManagerGoalDefinition(
			ManagerGoalType.MaintainHotelOperations,
			EmploymentAuthority.PostToHostBoard,
			new ManagerGoalConfiguration("post two notices", new EmploymentActionPlan([
				new BoardPostActionStep("Notice One", "Check the front room."),
				new BoardPostActionStep("Notice Two", "Check the stock room.")
			])),
			1,
			TimeSpan.Zero,
			new ManagerGoalPolicy(null, new ManagerGoalRiskLimits(MaximumActionSteps: 1)));

		var exception = Assert.ThrowsException<InvalidOperationException>(() =>
			host.ManagerGoalBoard.CreateGoal(definition, manager));

		StringAssert.Contains(exception.Message, "risk limit");
		Assert.AreEqual(0, host.ManagerGoalBoard.Goals.Count);
	}

	[TestMethod]
	public void ManagerGoals_RiskLimitSkipsWhenActiveTaskLimitReached()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("hotel", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateManagerGoals |
			EmploymentAuthority.PostToHostBoard), null);
		var goal = host.ManagerGoalBoard.CreateGoal(new ManagerGoalDefinition(
			ManagerGoalType.MaintainHotelOperations,
			EmploymentAuthority.PostToHostBoard,
			new ManagerGoalConfiguration("post room notice", new EmploymentActionPlan([
				new BoardPostActionStep("Rooms", "Room work required.")
			])),
			1,
			TimeSpan.Zero,
			new ManagerGoalPolicy(null, new ManagerGoalRiskLimits(MaximumActiveTasks: 1))), manager);
		var context = new EmploymentTaskContext(host);
		var now = DateTimeOffset.UtcNow;

		var first = host.ManagerGoalBoard.EvaluateGoals(context, now);
		var second = host.ManagerGoalBoard.EvaluateGoals(context, now.AddMinutes(1));

		Assert.AreEqual(1, first.Count);
		Assert.AreEqual(0, second.Count);
		Assert.AreEqual(1, host.TaskBoard.ActiveTasks.Count);
		StringAssert.Contains(goal.LastEvaluationResult!, "Risk limit permits");
	}

	[TestMethod]
	public void ManagerGoals_MarkSatisfiedAndReactivateWhenConditionsBecomeTrue()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("hotel", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateManagerGoals |
			EmploymentAuthority.PostToHostBoard), null);
		var goal = host.ManagerGoalBoard.CreateGoal(new ManagerGoalDefinition(
			ManagerGoalType.MaintainHotelOperations,
			EmploymentAuthority.PostToHostBoard,
			new ManagerGoalConfiguration("post room notice", new EmploymentActionPlan([
				new BoardPostActionStep("Rooms", "Room work required.")
			]), [new ManualOrderCondition("rooms-needed")]),
			1,
			TimeSpan.Zero), manager);
		var context = new EmploymentTaskContext(host);
		var now = DateTimeOffset.UtcNow;

		var idle = host.ManagerGoalBoard.EvaluateGoals(context, now);

		Assert.AreEqual(0, idle.Count);
		Assert.AreEqual(ManagerGoalStatus.Satisfied, goal.Status);
		StringAssert.Contains(goal.LastEvaluationResult!, "Manual order rooms-needed is not active");

		context.SetManualOrder("rooms-needed", true);
		var spawned = host.ManagerGoalBoard.EvaluateGoals(context, now.AddMinutes(1));

		Assert.AreEqual(1, spawned.Count);
		Assert.AreEqual(ManagerGoalStatus.Active, goal.Status);
		StringAssert.Contains(goal.LastEvaluationResult!, "Created active task");
	}

	[TestMethod]
	public void ManagerGoals_RichConditionExpressionSupportsOrAndReactivation()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("hotel", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateManagerGoals |
			EmploymentAuthority.PostToHostBoard), null);
		var goal = host.ManagerGoalBoard.CreateGoal(new ManagerGoalDefinition(
			ManagerGoalType.MaintainHotelOperations,
			EmploymentAuthority.PostToHostBoard,
			new ManagerGoalConfiguration("post optional room notice", new EmploymentActionPlan([
				new BoardPostActionStep("Rooms", "Optional room work required.")
			]),
			[
				new ManualOrderCondition("primary-room-work"),
				new ManualOrderCondition("fallback-room-work")
			],
			EmploymentConditionExpression.Any([
				EmploymentConditionExpression.Condition(1),
				EmploymentConditionExpression.Condition(2)
			])),
			1,
			TimeSpan.Zero), manager);
		var context = new EmploymentTaskContext(host);
		var now = DateTimeOffset.UtcNow;

		var idle = host.ManagerGoalBoard.EvaluateGoals(context, now);

		Assert.AreEqual(0, idle.Count);
		Assert.AreEqual(ManagerGoalStatus.Satisfied, goal.Status);
		StringAssert.Contains(goal.LastEvaluationResult!, "Manual order primary-room-work is not active");

		context.SetManualOrder("fallback-room-work", true);
		var spawned = host.ManagerGoalBoard.EvaluateGoals(context, now.AddMinutes(1));

		Assert.AreEqual(1, spawned.Count);
		Assert.AreEqual(ManagerGoalStatus.Active, goal.Status);
		Assert.AreEqual(goal.Id, spawned.Single().SourceGoalId);
		StringAssert.Contains(goal.LastEvaluationResult!, "Created active task");
	}

	[TestMethod]
	public void ManagerGoals_RejectInvalidConditionExpressionAtCreation()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("hotel", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateManagerGoals |
			EmploymentAuthority.PostToHostBoard), null);
		var definition = new ManagerGoalDefinition(
			ManagerGoalType.MaintainHotelOperations,
			EmploymentAuthority.PostToHostBoard,
			new ManagerGoalConfiguration("post room notice", new EmploymentActionPlan([
				new BoardPostActionStep("Rooms", "Room work required.")
			]), [new ManualOrderCondition("rooms-needed")], EmploymentConditionExpression.Condition(2)),
			1,
			TimeSpan.Zero);

		var exception = Assert.ThrowsException<InvalidOperationException>(() =>
			host.ManagerGoalBoard.CreateGoal(definition, manager));

		StringAssert.Contains(exception.Message, "does not exist");
		Assert.AreEqual(0, host.ManagerGoalBoard.Goals.Count);
	}

	[TestMethod]
	public void ManagerGoals_FailWhenSpawnedTaskFails()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("hotel", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateManagerGoals |
			EmploymentAuthority.PostToHostBoard), null);
		var goal = host.ManagerGoalBoard.CreateGoal(new ManagerGoalDefinition(
			ManagerGoalType.MaintainHotelOperations,
			EmploymentAuthority.PostToHostBoard,
			new ManagerGoalConfiguration("post room notice", new EmploymentActionPlan([
				new BoardPostActionStep("Rooms", "Room work required.")
			])),
			1,
			TimeSpan.Zero), manager);
		var context = new EmploymentTaskContext(host);
		var now = DateTimeOffset.UtcNow;
		var task = (EmploymentActiveTask)host.ManagerGoalBoard.EvaluateGoals(context, now).Single();

		task.MarkStep(0, EmploymentActionStepStatus.Failed,
			new EmploymentActionStepOperationalState(FailureDiagnostic: "notice delivery failed"));
		var followUp = host.ManagerGoalBoard.EvaluateGoals(context, now.AddMinutes(1));

		Assert.AreEqual(0, followUp.Count);
		Assert.AreEqual(ManagerGoalStatus.Failed, goal.Status);
		StringAssert.Contains(goal.LastEvaluationResult!, "notice delivery failed");
	}

	[TestMethod]
	public void ManagerGoals_FailWhenNoActionPlanIsAvailable()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("hotel", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateManagerGoals), null);
		var goal = host.ManagerGoalBoard.CreateGoal(new ManagerGoalDefinition(
			ManagerGoalType.MaintainHotelOperations,
			EmploymentAuthority.None,
			new ManagerGoalConfiguration("empty goal"),
			1,
			TimeSpan.Zero), manager);

		var tasks = host.ManagerGoalBoard.EvaluateGoals(new EmploymentTaskContext(host), DateTimeOffset.UtcNow);

		Assert.AreEqual(0, tasks.Count);
		Assert.AreEqual(ManagerGoalStatus.Failed, goal.Status);
		StringAssert.Contains(goal.LastEvaluationResult!, "no action plan");
	}

	[TestMethod]
	public void ManagerGoals_CraftMaterialsSpawnReplenishmentTaskWhenCommodityBelowThreshold()
	{
		var currency = Currency();
		var manager = Character(1, "Manager").Object;
		var stockroom = Cell(920, "materials store").Object;
		var shopfront = Cell(921, "workshop floor").Object;
		var shop = PermanentShop(30, "workshop", currency.Object, manager,
			EmploymentAuthority.CreateManagerGoals |
			EmploymentAuthority.ManageStockRules |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.ManageDeliveryRoutes,
			stockroom, [shopfront], [], [], []);
		IEmploymentHost host = shop.Shop.Object;
		var amount = new MoneyAmount(currency.Object, 10.0M);
		var materialKey = CommodityThresholdCondition.CreateKey("iron", null,
			new Dictionary<string, string>(), stockroom.Id, null);
		var definition = new ManagerGoalDefinition(
			ManagerGoalType.MaintainCraftMaterialSupply,
			new EmploymentAuthoritySet(EmploymentAuthority.ManageStockRules |
				EmploymentAuthority.ApprovePurchases |
				EmploymentAuthority.ManageDeliveryRoutes),
			new ManagerGoalConfiguration("replenish iron materials", new EmploymentActionPlan([
				new CataloguedActionShellStep("authorise", "buy iron materials", amount),
				new CataloguedActionShellStep("reserve", "buy iron materials", amount),
				new PurchaseActionStep("iron commodity stock", amount)
			]), [new CommodityThresholdCondition(materialKey, 5.0M, true)]),
			3,
			TimeSpan.FromMinutes(5));
		host.ManagerGoalBoard.CreateGoal(definition, manager);
		var context = new EmploymentTaskContext(host);
		context.SetAvailableItems(stockroom, []);

		var tasks = host.ManagerGoalBoard.EvaluateGoals(context, DateTimeOffset.UtcNow).ToList();

		Assert.AreEqual(1, tasks.Count);
		Assert.AreEqual(EmploymentTaskSourceKind.ManagerGoal, tasks.Single().SourceKind);
		Assert.AreEqual(ManagerGoalType.MaintainCraftMaterialSupply, host.ManagerGoalBoard.Goals.Single().GoalType);
		Assert.IsInstanceOfType(tasks.Single().ActionPlan.Steps[2], typeof(PurchaseActionStep));
		Assert.AreEqual(3, tasks.Single().Priority);
	}

	[TestMethod]
	public void ManagerGoals_HospitalConsumablesSpawnPurchaseTaskForServiceDeficits()
	{
		var currency = Currency();
		var manager = Character(931, "Hospital Manager").Object;
		var supplyItems = new List<IGameItem>
		{
			Item(9301, "bandage roll", prototypeId: 500).Object
		};
		var supplyRoom = PhysicalCell(9300, "clinic stockroom", supplyItems).Object;
		var (hospital, state) = HospitalEmploymentHost(930, "central clinic", currency.Object,
			[supplyRoom], [], 120.0M);
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateManagerGoals |
			EmploymentAuthority.ManageStockRules |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.ManageDeliveryRoutes), null);
		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.Id).Returns(9302);
		service.SetupGet(x => x.Name).Returns("wound binding");
		service.SetupGet(x => x.IsActive).Returns(true);
		service.SetupGet(x => x.RequiredEquipment).Returns([
			new HospitalServiceEquipmentRequirement(2, EmploymentItemSelector.ForPrototype(500),
				HospitalServiceSupplyItemType.Consumable)
		]);
		hospital.SetupGet(x => x.ActiveServices).Returns([service.Object]);
		var goal = hospital.Object.ManagerGoalBoard.CreateGoal(new ManagerGoalDefinition(
			ManagerGoalType.MaintainHospitalConsumableStock,
			new EmploymentAuthoritySet(EmploymentAuthority.ManageStockRules |
				EmploymentAuthority.ApprovePurchases |
				EmploymentAuthority.ManageDeliveryRoutes),
			new ManagerGoalConfiguration("replenish consumables", null,
			[
				new HospitalSupplyStockCondition(HospitalServiceSupplyItemType.Consumable, 3, "any", 10.0M)
			]),
			2,
			TimeSpan.Zero), manager);
		var context = new EmploymentTaskContext(hospital.Object);
		context.SetAvailableItems(supplyRoom, supplyItems);

		var tasks = hospital.Object.ManagerGoalBoard.EvaluateGoals(context, DateTimeOffset.UtcNow).ToList();

		Assert.AreEqual(1, tasks.Count);
		Assert.AreEqual(ManagerGoalStatus.Active, goal.Status);
		Assert.AreEqual(4, tasks.Single().ActionPlan.Steps.Count);
		Assert.IsInstanceOfType(tasks.Single().ActionPlan.Steps[0], typeof(CataloguedActionShellStep));
		Assert.IsInstanceOfType(tasks.Single().ActionPlan.Steps[1], typeof(CataloguedActionShellStep));
		Assert.IsInstanceOfType(tasks.Single().ActionPlan.Steps[2], typeof(PurchaseActionStep));
		Assert.IsInstanceOfType(tasks.Single().ActionPlan.Steps[3], typeof(DeliverItemsActionStep));
		var purchase = (PurchaseActionStep)tasks.Single().ActionPlan.Steps[2];
		Assert.AreEqual(5, purchase.Quantity);
		Assert.AreEqual(EmploymentPurchaseTargetKind.Item, purchase.TargetKind);
		Assert.AreEqual(EmploymentItemSelectorKind.PrototypeId, purchase.ItemSelector!.Kind);
		Assert.AreEqual(500, purchase.ItemSelector.Id);
		Assert.AreEqual("any", purchase.SupplierSelector);
		Assert.AreEqual(2, tasks.Single().Priority);
	}

	[TestMethod]
	public void HospitalSupplyStockCondition_ReusableToolsCountTheatreAndMedicalStaffInventory()
	{
		var currency = Currency();
		var supplyRoom = PhysicalCell(9400, "clinic stockroom", []).Object;
		var theatreItems = new List<IGameItem>
		{
			Item(9401, "surgical clamp", prototypeId: 700).Object
		};
		var theatre = PhysicalCell(9402, "operating theatre", theatreItems).Object;
		var (hospital, state) = HospitalEmploymentHost(940, "central clinic", currency.Object,
			[supplyRoom], [theatre], 120.0M);
		var carriedTool = Item(9403, "spare surgical clamp", prototypeId: 700).Object;
		var doctor = Character(9404, "Doctor").Object;
		Mock.Get(doctor).SetupGet(x => x.Inventory).Returns([carriedTool]);
		state.Hire(doctor, Offer(currency.Object, EmploymentRole.MedicalWorker,
			EmploymentAuthority.PerformMedicalServices), null);
		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.Id).Returns(9405);
		service.SetupGet(x => x.Name).Returns("minor surgery");
		service.SetupGet(x => x.IsActive).Returns(true);
		service.SetupGet(x => x.RequiredEquipment).Returns([
			new HospitalServiceEquipmentRequirement(1, EmploymentItemSelector.ForPrototype(700),
				HospitalServiceSupplyItemType.ReusableTool)
		]);
		hospital.SetupGet(x => x.ActiveServices).Returns([service.Object]);
		var condition = new HospitalSupplyStockCondition(HospitalServiceSupplyItemType.ReusableTool, 2, "any", null);
		var context = new EmploymentTaskContext(hospital.Object);
		context.SetAvailableItems(supplyRoom, []);
		context.SetAvailableItems(theatre, theatreItems);

		var satisfied = condition.IsSatisfied(context, DateTimeOffset.UtcNow, out var reason);

		Assert.IsFalse(satisfied);
		StringAssert.Contains(reason, "already covers");
	}

	[TestMethod]
	public void HospitalSupplyStockCondition_LiveKeywordSelectorsAreNotStockRequirements()
	{
		var currency = Currency();
		var supplyRoom = PhysicalCell(9520, "clinic stockroom", []).Object;
		var (hospital, _) = HospitalEmploymentHost(952, "central clinic", currency.Object, [supplyRoom], [], 120.0M);
		var visibleItem = Item(9521, "visible bandage roll", prototypeId: 500).Object;
		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.Id).Returns(9522);
		service.SetupGet(x => x.Name).Returns("wound binding");
		service.SetupGet(x => x.IsActive).Returns(true);
		service.SetupGet(x => x.RequiredEquipment).Returns([
			new HospitalServiceEquipmentRequirement(1, EmploymentItemSelector.ForItem(visibleItem, "bandage"),
				HospitalServiceSupplyItemType.Consumable)
		]);
		hospital.SetupGet(x => x.ActiveServices).Returns([service.Object]);
		var condition = new HospitalSupplyStockCondition(HospitalServiceSupplyItemType.Consumable, 3, "any", null);
		var context = new EmploymentTaskContext(hospital.Object);
		context.SetAvailableItems(supplyRoom, []);

		var satisfied = condition.IsSatisfied(context, DateTimeOffset.UtcNow, out var reason);

		Assert.IsFalse(satisfied);
		StringAssert.Contains(reason, "no active service");
	}
	[TestMethod]
	public void HospitalSupplyStockCondition_NoSupplyRoomsDoesNotSatisfyCondition()
	{
		var currency = Currency();
		var (hospital, _) = HospitalEmploymentHost(950, "central clinic", currency.Object, [], [], 120.0M);
		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.Id).Returns(9501);
		service.SetupGet(x => x.Name).Returns("wound binding");
		service.SetupGet(x => x.IsActive).Returns(true);
		service.SetupGet(x => x.RequiredEquipment).Returns([
			new HospitalServiceEquipmentRequirement(1, EmploymentItemSelector.ForPrototype(500),
				HospitalServiceSupplyItemType.Consumable)
		]);
		hospital.SetupGet(x => x.ActiveServices).Returns([service.Object]);
		var condition = new HospitalSupplyStockCondition(HospitalServiceSupplyItemType.Consumable, 3, "any", null);
		var context = new EmploymentTaskContext(hospital.Object);

		var satisfied = condition.IsSatisfied(context, DateTimeOffset.UtcNow, out var reason);

		Assert.IsFalse(satisfied);
		StringAssert.Contains(reason, "no supply rooms");
	}

	[TestMethod]
	public void ManagerGoals_HospitalConsumablesBlockWhenNoSupplyRoomConfigured()
	{
		var currency = Currency();
		var manager = Character(951, "Hospital Manager").Object;
		var (hospital, state) = HospitalEmploymentHost(9510, "central clinic", currency.Object, [], [], 120.0M);
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateManagerGoals |
			EmploymentAuthority.ManageStockRules |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.ManageDeliveryRoutes), null);
		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.Id).Returns(9511);
		service.SetupGet(x => x.Name).Returns("wound binding");
		service.SetupGet(x => x.IsActive).Returns(true);
		service.SetupGet(x => x.RequiredEquipment).Returns([
			new HospitalServiceEquipmentRequirement(1, EmploymentItemSelector.ForPrototype(500),
				HospitalServiceSupplyItemType.Consumable)
		]);
		hospital.SetupGet(x => x.ActiveServices).Returns([service.Object]);
		var goal = hospital.Object.ManagerGoalBoard.CreateGoal(new ManagerGoalDefinition(
			ManagerGoalType.MaintainHospitalConsumableStock,
			new EmploymentAuthoritySet(EmploymentAuthority.ManageStockRules |
				EmploymentAuthority.ApprovePurchases |
				EmploymentAuthority.ManageDeliveryRoutes),
			new ManagerGoalConfiguration("replenish consumables", null,
			[
				new HospitalSupplyStockCondition(HospitalServiceSupplyItemType.Consumable, 3, "any", 10.0M)
			]),
			2,
			TimeSpan.Zero), manager);
		var context = new EmploymentTaskContext(hospital.Object);

		var tasks = hospital.Object.ManagerGoalBoard.EvaluateGoals(context, DateTimeOffset.UtcNow).ToList();

		Assert.AreEqual(0, tasks.Count);
		Assert.AreEqual(ManagerGoalStatus.Blocked, goal.Status);
		StringAssert.Contains(goal.LastEvaluationResult, "no supply rooms");
	}
	[TestMethod]
	public void ManagerGoals_AdministratorsCanCreateGoalsWithoutEmploymentContract()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("hotel", currency.Object);
		var admin = Character(3, "Admin", administrator: true).Object;
		var definition = new ManagerGoalDefinition(
			ManagerGoalType.MaintainHotelOperations,
			EmploymentAuthority.PostToHostBoard,
			new ManagerGoalConfiguration("post room notice", new EmploymentActionPlan([
				new BoardPostActionStep("Rooms", "Room work required.")
			])),
			1,
			TimeSpan.Zero);

		var goal = host.ManagerGoalBoard.CreateGoal(definition, admin);

		Assert.AreEqual(ManagerGoalStatus.Active, goal.Status);
		Assert.AreEqual(1, host.ManagerGoalBoard.Goals.Count);
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ManagerGoalCreated));
	}

	[TestMethod]
	public void HotelEntity_LinksToPropertyOwnershipLocationAndExistingHotelXmlState()
	{
		var currency = Currency();
		var zone = new Mock<IEconomicZone>();
		zone.SetupGet(x => x.Currency).Returns(currency.Object);
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(123);
		var property = new Mock<IProperty>();
		property.SetupGet(x => x.Id).Returns(42);
		property.SetupGet(x => x.Name).Returns("Blue House");
		property.SetupGet(x => x.EconomicZone).Returns(zone.Object);
		property.SetupGet(x => x.PropertyLocations).Returns([cell.Object]);
		property.SetupGet(x => x.HotelRooms).Returns([]);
		property.SetupGet(x => x.IsApprovedHotel).Returns(true);
		property.SetupGet(x => x.HotelCashBalance).Returns(12.5M);
		property.SetupGet(x => x.HotelAvailableFunds).Returns(20.0M);

		var hotel = new Hotel(property.Object);

		Assert.AreSame(property.Object, hotel.Property);
		Assert.AreEqual(EmploymentHostType.Hotel, hotel.EmploymentHostType);
		Assert.IsTrue(hotel.CanAccessHotelLocation(cell.Object));
		Assert.AreEqual(12.5M, hotel.CashBalance);
		Assert.AreEqual(20.0M, hotel.AvailableFunds);
		Assert.IsNotNull(typeof(IProperty).GetProperty(nameof(IProperty.Hotel)));
	}

	[TestMethod]
	public void ClanEmploymentLocations_UseAnySharePropertyCellsAndHallCells()
	{
		var ownedCell = Cell(130, "guild office").Object;
		var overlapCell = Cell(131, "shared guild hall").Object;
		var hallCell = Cell(132, "field headquarters").Object;
		var unrelatedCell = Cell(133, "private warehouse").Object;
		var properties = new All<IProperty>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Properties).Returns(properties);
		var clan = new Mock<IClan>();
		clan.SetupGet(x => x.Id).Returns(10);
		clan.SetupGet(x => x.Name).Returns("merchant league");
		clan.SetupGet(x => x.ClanHallCells).Returns([hallCell, overlapCell]);
		clan.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var otherClan = new Mock<IClan>();
		otherClan.SetupGet(x => x.Id).Returns(11);
		var owner = new Mock<IPropertyOwner>();
		owner.SetupGet(x => x.Owner).Returns(clan.Object);
		owner.SetupGet(x => x.ShareOfOwnership).Returns(0.05M);
		var otherOwner = new Mock<IPropertyOwner>();
		otherOwner.SetupGet(x => x.Owner).Returns(otherClan.Object);
		otherOwner.SetupGet(x => x.ShareOfOwnership).Returns(1.0M);
		var ownedProperty = new Mock<IProperty>();
		ownedProperty.SetupGet(x => x.Id).Returns(210);
		ownedProperty.SetupGet(x => x.Name).Returns("guild office property");
		ownedProperty.SetupGet(x => x.PropertyOwners).Returns([owner.Object]);
		ownedProperty.SetupGet(x => x.PropertyLocations).Returns([ownedCell, overlapCell]);
		var unrelatedProperty = new Mock<IProperty>();
		unrelatedProperty.SetupGet(x => x.Id).Returns(211);
		unrelatedProperty.SetupGet(x => x.Name).Returns("private warehouse property");
		unrelatedProperty.SetupGet(x => x.PropertyOwners).Returns([otherOwner.Object]);
		unrelatedProperty.SetupGet(x => x.PropertyLocations).Returns([unrelatedCell]);
		properties.Add(ownedProperty.Object);
		properties.Add(unrelatedProperty.Object);

		var locations = clan.Object.EmploymentHostLocations();

		CollectionAssert.AreEquivalent(
			new[] { ownedCell.Id, overlapCell.Id, hallCell.Id },
			locations.Select(x => x.Id).ToArray());
		Assert.AreEqual(3, locations.Count);
	}

	[TestMethod]
	public void EmploymentPersistence_FirstAccessCreatesBoardAndEmptyHostState()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>());
			IEmploymentHost host = new PersistedEmploymentHost(10, "Copper Kettle", gameworld.Object, currency.Object);

			var employment = host.Employment;

			Assert.AreEqual(1, context.Boards.Count());
			Assert.AreEqual(1, context.EmploymentHostStates.Count());
			var state = context.EmploymentHostStates.Single();
			Assert.AreEqual(host.Id, state.HostId);
			Assert.AreEqual(EmploymentHostType.Shop.ToString(), state.HostType);
			Assert.AreEqual(state.BoardId, employment.Board.Id);
			Assert.IsFalse(context.EmploymentContracts.Any());
			Assert.IsFalse(context.EmploymentJobOpenings.Any());
			Assert.IsFalse(context.EmploymentActiveTasks.Any());
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void EmploymentPersistence_RoundTripsCoreEmploymentTasksGoalsAndAuditRows()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var manager = Character(100, "Manager").Object;
			var employee = Character(101, "Employee").Object;
			var characters = new Dictionary<long, ICharacter>
			{
				[manager.Id] = manager,
				[employee.Id] = employee
			};
			var gameworld = Gameworld(currency.Object, characters);
			IEmploymentHost host = new PersistedEmploymentHost(20, "Blue Bank", gameworld.Object, currency.Object);
			var managerOffer = Offer(currency.Object, EmploymentRole.Manager,
				EmploymentAuthority.HireEmployees |
				EmploymentAuthority.CreateJobOpenings |
				EmploymentAuthority.CreateScheduledRules |
				EmploymentAuthority.ModifyScheduledRules |
				EmploymentAuthority.AssignTasks |
				EmploymentAuthority.CreateManagerGoals |
				EmploymentAuthority.PostToHostBoard |
				EmploymentAuthority.ManageStockRules |
				EmploymentAuthority.DepositBusinessCash);
			host.Hire(manager, managerOffer, null);
			host.Hire(employee, Offer(currency.Object, EmploymentRole.Employee), manager);
			var opening = host.Employment.CreateJobOpening(Opening(currency.Object), manager);
			host.Employment.Apply(opening, Profile(employee, 1.0M, PaymentMethodKind.Cash,
				Caps(EmploymentAICapability.CanPurchaseCommodities),
				new Dictionary<string, double> { ["haggling"] = 40.0 }));

			var scheduledPlan = new EmploymentActionPlan([
				new BoardPostActionStep("Restock", "Restock request spawned.")
			]);
			host.TaskBoard.CreateConditionPredicate("weather-window",
				[new WeatherLevelCondition(WeatherLevelCondition.CreatePrecipitationKey("rain"))],
				EmploymentConditionExpression.Condition(1),
				manager);
			var scheduledRule = host.TaskBoard.CreateScheduledRule("restock butter", "restock-butter",
				[
					new StockThresholdCondition("butter", 5, true),
					new ManualOrderCondition("restock-butter"),
					new ItemThresholdCondition(
						ItemThresholdCondition.CreateKey(EmploymentItemSelector.ForPrototype(26), 397, null), 2, true),
					new CommodityThresholdCondition(
						CommodityThresholdCondition.CreateKey("Iron", "Nails",
							new Dictionary<string, string> { ["grade"] = "refined" }, 397, null), 5.0M, false),
					new WeatherLevelCondition(WeatherLevelCondition.CreatePrecipitationKey("rain"))
				],
				EmploymentConditionExpression.Any(
				[
					EmploymentConditionExpression.All(
					[
						EmploymentConditionExpression.Condition(1),
						EmploymentConditionExpression.Condition(2)
					]),
					EmploymentConditionExpression.Predicate("weather-window")
				]),
				scheduledPlan,
				TimeSpan.FromHours(1),
				manager);
			host.TaskBoard.CreateScheduledRuleTemplate("restock template", "restock-template",
				[
					new StockThresholdCondition("butter", 5, true),
					new ManualOrderCondition("restock-butter")
				],
				EmploymentConditionExpression.Any(
				[
					EmploymentConditionExpression.Condition(1),
					EmploymentConditionExpression.Condition(2)
				]),
				scheduledPlan,
				TimeSpan.FromHours(2),
				manager);
			host.TaskBoard.PauseScheduledRule(scheduledRule, manager, "pause for persistence test");
			var activeTask = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("deposit float",
				new EmploymentActionPlan([
					new BankDepositActionStep(new MoneyAmount(currency.Object, 12.0M), "bank-ledger-1")
				]),
				manager);
			activeTask.Assign(manager);
			activeTask.MarkStep(0, EmploymentActionStepStatus.Completed);
			host.ManagerGoalBoard.CreateGoal(new ManagerGoalDefinition(
				ManagerGoalType.MaintainMinimumStock,
				EmploymentAuthority.PostToHostBoard,
				new ManagerGoalConfiguration("post stock notice", scheduledPlan,
					[new AccountBalanceCondition("operating", 5.0M, true)]),
				1,
				TimeSpan.FromMinutes(15)), manager);
			host.BusinessLedger.Record(EmploymentLedgerEntryType.Purchase, manager,
				new MoneyAmount(currency.Object, 3.0M), "seed purchase audit");

			var reloadedHost = new PersistedEmploymentHost(20, "Blue Bank", gameworld.Object, currency.Object);
			var reloaded = reloadedHost.Employment;

			Assert.AreEqual(2, reloaded.EmploymentContracts.Count);
			Assert.AreEqual(1, reloaded.JobOpenings.Count);
			Assert.AreEqual(1, reloaded.Applications.Count);
			Assert.AreEqual(1, reloaded.TaskBoard.ScheduledRules.Count);
			Assert.AreEqual(EmploymentScheduledRuleStatus.Paused, reloaded.TaskBoard.ScheduledRules.Single().Status);
			Assert.IsNotNull(reloaded.TaskBoard.ScheduledRules.Single().ConditionExpression);
			Assert.IsTrue(reloaded.TaskBoard.ScheduledRules.Single().Conditions.Any(x => x is ItemThresholdCondition));
			Assert.IsTrue(reloaded.TaskBoard.ScheduledRules.Single().Conditions.Any(x => x is CommodityThresholdCondition));
			Assert.IsTrue(reloaded.TaskBoard.ScheduledRules.Single().Conditions.Any(x => x is WeatherLevelCondition));
			Assert.AreEqual(1, reloaded.TaskBoard.ConditionPredicates.Count);
			Assert.AreEqual("weather-window", reloaded.TaskBoard.ConditionPredicates.Single().Name);
			Assert.IsNotNull(reloaded.TaskBoard.ConditionPredicates.Single().ConditionExpression);
			Assert.AreEqual(1, reloaded.TaskBoard.ScheduledRuleTemplates.Count);
			Assert.AreEqual("restock template", reloaded.TaskBoard.ScheduledRuleTemplates.Single().Name);
			Assert.IsNotNull(reloaded.TaskBoard.ScheduledRuleTemplates.Single().ConditionExpression);
			Assert.AreEqual(1, reloaded.TaskBoard.ActiveTasks.Count);
			Assert.AreEqual(EmploymentTaskStatus.Completed, reloaded.TaskBoard.ActiveTasks.Single().Status);
			Assert.AreEqual(EmploymentActionStepStatus.Completed, reloaded.TaskBoard.ActiveTasks.Single().StepStates.Single());
			Assert.AreEqual(1, reloaded.ManagerGoalBoard.Goals.Count);
			Assert.AreEqual(1, reloaded.ManagerGoalBoard.Goals.Single().Priority);
			Assert.IsTrue(reloaded.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.Purchase));
			Assert.IsTrue(reloaded.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ContractHired));
			Assert.AreEqual(context.EmploymentHostStates.Single().BoardId, reloaded.Board.Id);
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void EmploymentPersistence_RoundTripsApplicationContractOriginsForOpeningCapacity()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var manager = Character(181, "Manager").Object;
			var first = Character(182, "First Applicant").Object;
			var second = Character(183, "Second Applicant").Object;
			var characters = new Dictionary<long, ICharacter>
			{
				[manager.Id] = manager,
				[first.Id] = first,
				[second.Id] = second
			};
			var gameworld = Gameworld(currency.Object, characters);
			IEmploymentHost host = new PersistedEmploymentHost(181, "Capacity Shop", gameworld.Object, currency.Object);
			host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
				EmploymentAuthority.CreateJobOpenings | EmploymentAuthority.HireEmployees), null);
			var opening = host.Employment.CreateJobOpening(Opening(currency.Object), manager);
			var firstApplication = host.Employment.Apply(opening, Profile(first, 1.0M, PaymentMethodKind.Cash,
				Caps(EmploymentAICapability.CanPurchaseCommodities),
				new Dictionary<string, double> { ["haggling"] = 40.0 }));
			var secondApplication = host.Employment.Apply(opening, Profile(second, 1.0M, PaymentMethodKind.Cash,
				Caps(EmploymentAICapability.CanPurchaseCommodities),
				new Dictionary<string, double> { ["haggling"] = 40.0 }));
			host.Employment.AcceptApplication(firstApplication, manager);

			var reloadedHost = new PersistedEmploymentHost(181, "Capacity Shop", gameworld.Object, currency.Object);
			var reloaded = reloadedHost.Employment;
			var reloadedOpening = reloaded.JobOpenings.Single();
			var reloadedContract = reloaded.EmploymentContracts.Single(x => x.Employee.Id == first.Id);
			var reloadedSecondApplication = reloaded.Applications.Single(x => x.Id == secondApplication.Id);

			Assert.AreEqual(reloadedOpening.Id, reloadedContract.OriginOpeningId);
			Assert.AreEqual(firstApplication.Id, reloadedContract.OriginApplicationId);
			Assert.AreEqual(1, reloadedOpening.OccupiedPositions);
			Assert.IsFalse(reloadedOpening.AcceptsApplications);
			var ex = Assert.ThrowsException<InvalidOperationException>(() =>
				reloaded.AcceptApplication(reloadedSecondApplication, manager));
			StringAssert.Contains(ex.Message, "capacity");
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void EmploymentPersistence_RoundTripsPendingApplicationRevisionAndCandidateProfile()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var manager = Character(184, "Manager").Object;
			var applicant = Character(185, "Applicant").Object;
			var characters = new Dictionary<long, ICharacter>
			{
				[manager.Id] = manager,
				[applicant.Id] = applicant
			};
			var gameworld = Gameworld(currency.Object, characters);
			IEmploymentHost host = new PersistedEmploymentHost(184, "Snapshot Shop", gameworld.Object, currency.Object);
			host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
				EmploymentAuthority.CreateJobOpenings | EmploymentAuthority.ModifyJobOpenings | EmploymentAuthority.HireEmployees), null);
			var definition = Opening(currency.Object);
			var opening = host.Employment.CreateJobOpening(definition, manager);
			var application = host.Employment.Apply(opening, Profile(applicant, 1.0M, PaymentMethodKind.Cash,
				Caps(EmploymentAICapability.CanPurchaseCommodities),
				new Dictionary<string, double> { ["haggling"] = 40.0 }));
			host.Employment.ModifyJobOpening(opening, definition with { MaxPositions = 2 }, manager, "revision test");

			var reloadedHost = new PersistedEmploymentHost(184, "Snapshot Shop", gameworld.Object, currency.Object);
			var reloaded = reloadedHost.Employment;
			var reloadedOpening = reloaded.JobOpenings.Single();
			var reloadedApplication = reloaded.Applications.Single();
			var profile = (EmploymentCandidateProfile?)reloadedApplication.GetType()
			                                                       .GetProperty("CandidateProfile",
				                                                       BindingFlags.Instance | BindingFlags.NonPublic)!
			                                                       .GetValue(reloadedApplication);

			Assert.AreEqual(2, reloadedOpening.RevisionNumber);
			Assert.AreEqual(application.OfferedOpeningRevision, reloadedApplication.OfferedOpeningRevision);
			Assert.IsNotNull(profile);
			Assert.AreEqual(40.0, profile!.Skills["haggling"]);
			Assert.IsTrue(profile.AcceptedPaymentMethods.Contains(PaymentMethodKind.Cash));
			var ex = Assert.ThrowsException<InvalidOperationException>(() =>
				reloaded.AcceptApplication(reloadedApplication, manager));
			StringAssert.Contains(ex.Message, "terms have changed");
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void EmploymentPersistence_LoadedTasksWithoutPersistedGrantFailClosedForFinancialAuthority()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var manager = Character(171, "Manager").Object;
			var characters = new Dictionary<long, ICharacter>
			{
				[manager.Id] = manager
			};
			var gameworld = Gameworld(currency.Object, characters);
			IEmploymentHost host = new PersistedEmploymentHost(171, "Grant Shop", gameworld.Object, currency.Object);
			host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
				EmploymentAuthority.AssignTasks |
				EmploymentAuthority.ApprovePurchases |
				EmploymentAuthority.DepositBusinessCash), null);
			host.TaskBoard.CreateActiveTask("persisted deposit",
				new EmploymentActionPlan([
					new CataloguedActionShellStep("authorise", "deposit approval", new MoneyAmount(currency.Object, 5.0M)),
					new BankDepositActionStep(new MoneyAmount(currency.Object, 5.0M))
				]),
				manager);

			IEmploymentHost reloadedHost = new PersistedEmploymentHost(171, "Grant Shop", gameworld.Object, currency.Object);
			var reloadedTask = reloadedHost.TaskBoard.ActiveTasks.Single();
			var dispatcher = new EmploymentTaskDispatcher();
			var taskContext = new EmploymentTaskContext(reloadedHost);
			var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
				Caps(EmploymentAICapability.CanUseBankAccount, EmploymentAICapability.CanHandleCash));

			Assert.AreEqual(EmploymentTaskSourceKind.HostSystem, reloadedTask.SourceKind);
			Assert.AreEqual(EmploymentAuthority.None, reloadedTask.AuthorisationGrant.Authority.Authorities);
			Assert.IsFalse(reloadedTask.AuthorisationGrant.AllowsUnboundedFinancialSteps);
			Assert.AreEqual(0, reloadedTask.AuthorisationGrant.AmountLimits.Count);
			Assert.IsTrue(dispatcher.TryAssignTask(reloadedTask, [profile], taskContext, out var reason), reason);
			Assert.IsTrue(dispatcher.AdvanceTask(reloadedTask, taskContext).Success);
			var deposit = AdvanceTaskOrAssignmentFailure(dispatcher, reloadedTask, taskContext, profile);

			Assert.IsFalse(deposit.Success);
			StringAssert.Contains(deposit.Message, "requires an auditable payment authorisation");
			Assert.AreEqual(EmploymentTaskStatus.Blocked, reloadedTask.Status);
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void EmploymentPersistence_RoundTripsPayrollLiabilitiesAndSettledWageLedgerRows()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var employee = Character(151, "Employee").Object;
			var characters = new Dictionary<long, ICharacter>
			{
				[employee.Id] = employee
			};
			var gameworld = Gameworld(currency.Object, characters);
			IEmploymentHost host = new PersistedEmploymentHost(151, "Payroll Shop", gameworld.Object, currency.Object);
			VirtualCashLedger.Credit(host, currency.Object, 750.0M, null, host, "Seed", "Seed payroll balance");
			var compensation = new CompensationTerms(
				new MoneyAmount(currency.Object, 10.0M),
				null,
				PayCadence.Daily,
				new MoneyAmount(currency.Object, 10.0M),
				PaymentSource.HostCash);
			var contract = host.Hire(employee, new EmploymentOffer(
				EmploymentRole.Employee,
				compensation,
				WorkSchedule.AnyTime,
				EmploymentDuration.Indefinite,
				new PaymentMethod(PaymentMethodKind.Cash),
				EmploymentAuthoritySet.Empty), null);
			host.Payroll.EvaluatePayroll(contract.StartedAt.AddDays(3).AddMinutes(1));
			host.Fire(contract, EmploymentTerminationReason.Resigned, null);
			Assert.IsTrue(host.Payroll.TrySettlePayables(host.Payroll.OutstandingLiabilities, null, false,
				"Settled after resignation.", out var message), message);

			IEmploymentHost reloadedHost = new PersistedEmploymentHost(151, "Payroll Shop", gameworld.Object, currency.Object);
			var reloaded = reloadedHost.Employment;

			Assert.AreEqual(3, reloaded.Payroll.Payables.Count);
			Assert.IsTrue(reloaded.Payroll.Payables.All(x => x.Status == EmploymentPayableStatus.Settled));
			Assert.IsTrue(reloaded.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.Wage));
			Assert.IsTrue(reloaded.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.WageSettled));
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void EmploymentPersistence_RoundTripsInventoryActionSteps()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var manager = Character(250, "Manager").Object;
			var source = Cell(800, "stock room").Object;
			var destination = Cell(801, "sales floor").Object;
			var container = Item(802, "display basket").Object;
			var characters = new Dictionary<long, ICharacter>
			{
				[manager.Id] = manager
			};
			var gameworld = Gameworld(currency.Object, characters, [source, destination],
				new Dictionary<long, IGameItem> { [container.Id] = container });
			IEmploymentHost host = new PersistedEmploymentHost(251, "Stock Shop", gameworld.Object, currency.Object);
			host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
				EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
			host.TaskBoard.CreateActiveTask("inventory movement",
				new EmploymentActionPlan([
					new GetItemsByIdActionStep(1, [9001], [source]),
					new GetItemsByTagActionStep(2, "linen", [source]),
					new GetCommodityActionStep(5.0, "iron", "ingot",
						new Dictionary<string, string> { ["grade"] = "refined" }, [source]),
					new DeliverItemsActionStep(destination, container, "display-container"),
					new LoadItemsActionStep(container, null, destination),
					new UnloadItemsActionStep(container, null, destination),
					new ReturnAssetActionStep(container, null, source, container)
				]),
				manager);

			var reloadedHost = new PersistedEmploymentHost(251, "Stock Shop", gameworld.Object, currency.Object);
			var steps = reloadedHost.Employment.TaskBoard.ActiveTasks.Single().ActionPlan.Steps;

			Assert.AreEqual(7, steps.Count);
			Assert.AreEqual(EmploymentActionStepType.GetItemsById, steps[0].StepType);
			Assert.AreEqual(EmploymentActionStepType.GetItemsByTag, steps[1].StepType);
			Assert.AreEqual(EmploymentActionStepType.GetCommodity, steps[2].StepType);
			Assert.AreEqual(EmploymentActionStepType.DeliverItems, steps[3].StepType);
			Assert.AreEqual(EmploymentActionStepType.LoadItems, steps[4].StepType);
			Assert.AreEqual(EmploymentActionStepType.UnloadItems, steps[5].StepType);
			Assert.AreEqual(EmploymentActionStepType.ReturnAsset, steps[6].StepType);
			Assert.AreEqual(9001, ((GetItemsByIdActionStep)steps[0]).ItemPrototypeIds.Single());
			Assert.AreEqual("linen", ((GetItemsByTagActionStep)steps[1]).TagName);
			Assert.AreEqual("iron", ((GetCommodityActionStep)steps[2]).MaterialName);
			Assert.AreEqual("refined", ((GetCommodityActionStep)steps[2]).Characteristics["grade"]);
			Assert.AreSame(destination, ((DeliverItemsActionStep)steps[3]).Destination);
			Assert.AreSame(container, ((DeliverItemsActionStep)steps[3]).Container);
			Assert.AreSame(container, ((LoadItemsActionStep)steps[4]).TargetContainer);
			Assert.AreSame(container, ((UnloadItemsActionStep)steps[5]).SourceContainer);
			Assert.AreSame(source, ((ReturnAssetActionStep)steps[6]).Destination);
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void EmploymentPersistence_LoadsInventoryActionStepsWithNullSourceLocationPayloads()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>());
			var board = new MudSharp.Models.Board
			{
				Id = 370,
				Name = "stale employment board",
				ShowOnLogin = false
			};
			var hostState = new MudSharp.Models.EmploymentHostState
			{
				Id = 371,
				HostType = EmploymentHostType.Shop.ToString(),
				HostId = 372,
				BoardId = board.Id,
				Board = board,
				CreatedAt = DateTime.UtcNow,
				LastUpdatedAt = DateTime.UtcNow
			};
			var plan = new MudSharp.Models.EmploymentActionPlanRecord
			{
				Id = 373,
				EmploymentHostState = hostState,
				EmploymentHostStateId = hostState.Id,
				Name = "stale inventory plan"
			};
			plan.Steps.Add(new MudSharp.Models.EmploymentActionStepRecord
			{
				Id = 374,
				EmploymentActionPlan = plan,
				EmploymentActionPlanId = plan.Id,
				SortOrder = 0,
				StepType = (int)EmploymentActionStepType.GetItemsById,
				RequiredAuthority = (long)EmploymentAuthority.ManageDeliveryRoutes,
				RequiredCapabilities = string.Empty,
				Description = "legacy get item step",
				BoardText = "{\"Quantity\":1,\"ItemPrototypeIds\":[9001],\"SourceLocationIds\":null}"
			});
			plan.Steps.Add(new MudSharp.Models.EmploymentActionStepRecord
			{
				Id = 375,
				EmploymentActionPlan = plan,
				EmploymentActionPlanId = plan.Id,
				SortOrder = 1,
				StepType = (int)EmploymentActionStepType.GetItemsByTag,
				RequiredAuthority = (long)EmploymentAuthority.ManageDeliveryRoutes,
				RequiredCapabilities = string.Empty,
				Description = "legacy get tag step",
				BoardText = "{\"Quantity\":2,\"TagName\":\"linen\",\"SourceLocationIds\":null}"
			});
			plan.Steps.Add(new MudSharp.Models.EmploymentActionStepRecord
			{
				Id = 376,
				EmploymentActionPlan = plan,
				EmploymentActionPlanId = plan.Id,
				SortOrder = 2,
				StepType = (int)EmploymentActionStepType.GetCommodity,
				RequiredAuthority = (long)EmploymentAuthority.ManageDeliveryRoutes,
				RequiredCapabilities = string.Empty,
				Description = "legacy get commodity step",
				BoardText = "{\"RequiredWeight\":5.0,\"MaterialName\":\"iron\",\"TagName\":\"ingot\",\"Characteristics\":null,\"SourceLocationIds\":null}"
			});
			context.Boards.Add(board);
			context.EmploymentHostStates.Add(hostState);
			context.EmploymentActionPlans.Add(plan);
			context.EmploymentActiveTasks.Add(new MudSharp.Models.EmploymentActiveTaskRecord
			{
				Id = 377,
				PublicId = Guid.NewGuid().ToString("D"),
				EmploymentHostState = hostState,
				EmploymentHostStateId = hostState.Id,
				Name = "stale inventory task",
				EmploymentActionPlan = plan,
				EmploymentActionPlanId = plan.Id,
				Status = (int)EmploymentTaskStatus.Pending,
				CorrelationId = Guid.NewGuid().ToString("D"),
				IdempotencyKey = "stale-inventory-task"
			});
			context.SaveChanges();

			IEmploymentHost reloadedHost = new PersistedEmploymentHost(372, "Stale Stock Shop", gameworld.Object,
				currency.Object);
			var steps = reloadedHost.Employment.TaskBoard.ActiveTasks.Single().ActionPlan.Steps;

			Assert.AreEqual(3, steps.Count);
			Assert.AreEqual(9001, ((GetItemsByIdActionStep)steps[0]).ItemPrototypeIds.Single());
			Assert.AreEqual(0, ((GetItemsByIdActionStep)steps[0]).SourceLocations.Count);
			Assert.AreEqual("linen", ((GetItemsByTagActionStep)steps[1]).TagName);
			Assert.AreEqual(0, ((GetItemsByTagActionStep)steps[1]).SourceLocations.Count);
			Assert.AreEqual("iron", ((GetCommodityActionStep)steps[2]).MaterialName);
			Assert.AreEqual(0, ((GetCommodityActionStep)steps[2]).Characteristics.Count);
			Assert.AreEqual(0, ((GetCommodityActionStep)steps[2]).SourceLocations.Count);
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void EmploymentPersistence_RoundTripsCatalogueShellAndAuditActionSteps()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var manager = Character(260, "Manager").Object;
			var destination = Cell(810, "sales floor").Object;
			var staging = Cell(811, "staging room").Object;
			var arena = new Mock<ICombatArena>();
			arena.SetupGet(x => x.Id).Returns(812);
			arena.SetupGet(x => x.Name).Returns("persisted arena");
			arena.SetupGet(x => x.FrameworkItemType).Returns("CombatArena");
			var eventType = new Mock<IArenaEventType>();
			eventType.SetupGet(x => x.Id).Returns(813);
			eventType.SetupGet(x => x.Name).Returns("persisted arena bout");
			eventType.SetupGet(x => x.Arena).Returns(arena.Object);
			var scheduledArenaEvent = new DateTime(2099, 2, 1, 10, 0, 0, DateTimeKind.Utc);
			var bankReserves = new DecimalCounter<ICurrency> { [currency.Object] = 50.0M };
			var (bank, _, _) = BankHost(814, "persisted bank", currency.Object, bankReserves);
			var stableCell = Cell(815, "persisted stable stall").Object;
			var stable = new Mock<IStable>();
			stable.SetupGet(x => x.Id).Returns(816);
			stable.SetupGet(x => x.Name).Returns("persisted stable");
			stable.SetupGet(x => x.FrameworkItemType).Returns("Stable");
			stable.SetupGet(x => x.Location).Returns(stableCell);
			var stableStay = new Mock<IStableStay>();
			stableStay.SetupGet(x => x.Id).Returns(817);
			stableStay.SetupGet(x => x.Stable).Returns(stable.Object);
			stableStay.SetupGet(x => x.IsActive).Returns(true);
			var stableAccount = new Mock<IStableAccount>();
			stableAccount.SetupGet(x => x.Id).Returns(818);
			stableAccount.SetupGet(x => x.Name).Returns("stable ledger");
			stableAccount.SetupGet(x => x.AccountName).Returns("stable ledger");
			stableAccount.SetupGet(x => x.Stable).Returns(stable.Object);
			stable.SetupGet(x => x.Stays).Returns([stableStay.Object]);
			stable.SetupGet(x => x.ActiveStays).Returns([stableStay.Object]);
			stable.SetupGet(x => x.StableAccounts).Returns([stableAccount.Object]);
			var hotelRoomCell = Cell(819, "persisted hotel room").Object;
			var property = new Mock<IProperty>();
			property.SetupGet(x => x.Id).Returns(820);
			property.SetupGet(x => x.Name).Returns("persisted inn");
			var hotel = new Mock<IHotel>();
			hotel.SetupGet(x => x.Id).Returns(821);
			hotel.SetupGet(x => x.Name).Returns("persisted inn hotel");
			hotel.SetupGet(x => x.FrameworkItemType).Returns("Hotel");
			hotel.SetupGet(x => x.Property).Returns(property.Object);
			property.SetupGet(x => x.Hotel).Returns(hotel.Object);
			var hotelRoom = new Mock<IHotelRoom>();
			hotelRoom.SetupGet(x => x.Name).Returns("persisted room");
			hotelRoom.SetupGet(x => x.Cell).Returns(hotelRoomCell);
			var lostProperty = new Mock<IHotelLostProperty>();
			lostProperty.SetupGet(x => x.BundleId).Returns(822);
			lostProperty.SetupGet(x => x.Description).Returns("persisted valise");
			lostProperty.SetupGet(x => x.Status).Returns(HotelLostPropertyStatus.Held);
			hotel.SetupGet(x => x.Rooms).Returns([hotelRoom.Object]);
			property.SetupGet(x => x.HotelLostProperties).Returns([lostProperty.Object]);
			property.SetupGet(x => x.HotelPatronBalances).Returns([]);
			var characters = new Dictionary<long, ICharacter>
			{
				[manager.Id] = manager
			};
			var gameworld = Gameworld(currency.Object, characters, [staging, destination, stableCell, hotelRoomCell], combatArenasToAdd: [arena.Object], banksToAdd: [bank.Object], stablesToAdd: [stable.Object], propertiesToAdd: [property.Object]);
			IEmploymentHost host = new PersistedEmploymentHost(261, "Audit Shop", gameworld.Object, currency.Object);
			host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthoritySet.All.Authorities), null);
			var task = host.TaskBoard.CreateActiveTask("catalogue actions",
				new EmploymentActionPlan([
					new CataloguedActionShellStep("route", "check the delivery route", [staging, destination]),
					new CataloguedActionShellStep("report", "stock counted"),
					new CommandActionStep("say", "hello", destination),
					new BoardPostActionStep("Notice", "Check the shelves."),
					new PurchaseActionStep("purchase thread", new MoneyAmount(currency.Object, 5.0M)),
					new BankDepositActionStep(new MoneyAmount(currency.Object, 4.0M)),
					new BankWithdrawalActionStep(new MoneyAmount(currency.Object, 3.0M)),
					new StoreAccountPaymentActionStep("supplier", new MoneyAmount(currency.Object, 2.0M)),
					new CraftTriggerActionStep("craft linen bundles"),
					new TaxPaymentActionStep(new MoneyAmount(currency.Object, 1.0M)),
					new ShopFloatAdjustmentActionStep(true, new MoneyAmount(currency.Object, 1.0M)),
					new PhysicalFloatActionStep(PhysicalFloatOperation.Issue, new MoneyAmount(currency.Object, 1.0M), "bank"),
					new CraftStationActionStep("here"),
					new MovementDeliveryActionStep("move to the sales floor", destination),
					new ArenaEventAdministrationActionStep(arena.Object, eventType.Object, scheduledArenaEvent),
					new BankAdministrationActionStep(bank.Object, BankAdministrationActionKind.ReserveDeposit,
						new MoneyAmount(currency.Object, 6.0M)),
					new StableAdministrationActionStep(stable.Object, StableAdministrationActionKind.CareGroom,
						stay: stableStay.Object, note: "brush down"),
					new StableAdministrationActionStep(stable.Object, StableAdministrationActionKind.AccountReconciliation,
						account: stableAccount.Object, note: "ledger review"),
					new HotelAdministrationActionStep(hotel.Object, HotelAdministrationActionKind.RoomReady,
						room: hotelRoom.Object, note: "guest ready"),
					new HotelAdministrationActionStep(hotel.Object, HotelAdministrationActionKind.LostPropertyAudit,
						lostProperty: lostProperty.Object, note: "owner match")
				]),
				manager);
			var dispatcher = new EmploymentTaskDispatcher();
			var taskContext = new EmploymentTaskContext(host);
			var profile = Profile(manager, 10.0M, PaymentMethodKind.Cash,
				Caps(
					EmploymentAICapability.CanDeliverItems,
					EmploymentAICapability.CanExecuteCommandTask,
					EmploymentAICapability.CanPostToBoard,
					EmploymentAICapability.CanPurchaseCommodities,
					EmploymentAICapability.CanUseBankAccount,
					EmploymentAICapability.CanHandleCash,
					EmploymentAICapability.CanCraft));
			Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], taskContext, out var reason), reason);
			var routeResult = dispatcher.AdvanceTask(task, taskContext);
			Assert.IsTrue(routeResult.Success);
			StringAssert.Contains(task.StepOperationalStates[0].RouteResult, "delivery route");
			StringAssert.Contains(task.StepOperationalStates[0].RouteResult, "stops=811,810");
			StringAssert.Contains(task.StepOperationalStates[0].SelectedResources, "operation=route");

			var reloadedHost = new PersistedEmploymentHost(261, "Audit Shop", gameworld.Object, currency.Object);
			var reloadedTask = reloadedHost.Employment.TaskBoard.ActiveTasks.Single();
			var steps = reloadedTask.ActionPlan.Steps;

			Assert.AreEqual(20, steps.Count);
			Assert.IsInstanceOfType(steps[0], typeof(CataloguedActionShellStep));
			Assert.AreEqual("route", ((CataloguedActionShellStep)steps[0]).ActionKey);
			Assert.AreSame(destination, ((CataloguedActionShellStep)steps[0]).TargetLocation);
			CollectionAssert.AreEqual(new[] { staging.Id, destination.Id }, ((CataloguedActionShellStep)steps[0]).RouteStops.Select(x => x.Id).ToArray());
			StringAssert.Contains(reloadedTask.StepOperationalStates[0].RouteResult, "delivery route");
			Assert.IsInstanceOfType(steps[2], typeof(CommandActionStep));
			Assert.AreSame(destination, ((CommandActionStep)steps[2]).ExecutionLocation);
			Assert.IsInstanceOfType(steps[3], typeof(BoardPostActionStep));
			Assert.IsInstanceOfType(steps[4], typeof(PurchaseActionStep));
			Assert.IsInstanceOfType(steps[5], typeof(BankDepositActionStep));
			Assert.IsInstanceOfType(steps[6], typeof(BankWithdrawalActionStep));
			Assert.IsInstanceOfType(steps[7], typeof(StoreAccountPaymentActionStep));
			Assert.IsInstanceOfType(steps[8], typeof(CraftTriggerActionStep));
			Assert.IsInstanceOfType(steps[9], typeof(TaxPaymentActionStep));
			Assert.IsInstanceOfType(steps[10], typeof(ShopFloatAdjustmentActionStep));
			Assert.IsInstanceOfType(steps[11], typeof(PhysicalFloatActionStep));
			Assert.IsInstanceOfType(steps[12], typeof(CraftStationActionStep));
			Assert.IsInstanceOfType(steps[13], typeof(MovementDeliveryActionStep));
			Assert.IsInstanceOfType(steps[14], typeof(ArenaEventAdministrationActionStep));
			Assert.AreSame(arena.Object, ((ArenaEventAdministrationActionStep)steps[14]).Arena);
			Assert.IsInstanceOfType(steps[15], typeof(BankAdministrationActionStep));
			var reloadedBankAdmin = (BankAdministrationActionStep)steps[15];
			Assert.AreSame(bank.Object, reloadedBankAdmin.Bank);
			Assert.AreEqual(BankAdministrationActionKind.ReserveDeposit, reloadedBankAdmin.Operation);
			Assert.AreEqual(6.0M, reloadedBankAdmin.Amount!.Amount);
			Assert.IsInstanceOfType(steps[16], typeof(StableAdministrationActionStep));
			var reloadedStableCare = (StableAdministrationActionStep)steps[16];
			Assert.AreSame(stable.Object, reloadedStableCare.Stable);
			Assert.AreSame(stableStay.Object, reloadedStableCare.Stay);
			Assert.AreEqual(StableAdministrationActionKind.CareGroom, reloadedStableCare.Operation);
			Assert.AreEqual("brush down", reloadedStableCare.Note);
			Assert.IsInstanceOfType(steps[17], typeof(StableAdministrationActionStep));
			var reloadedStableAccount = (StableAdministrationActionStep)steps[17];
			Assert.AreSame(stableAccount.Object, reloadedStableAccount.Account);
			Assert.AreEqual(StableAdministrationActionKind.AccountReconciliation, reloadedStableAccount.Operation);
			Assert.IsInstanceOfType(steps[18], typeof(HotelAdministrationActionStep));
			var reloadedHotelRoom = (HotelAdministrationActionStep)steps[18];
			Assert.AreSame(hotel.Object, reloadedHotelRoom.Hotel);
			Assert.AreSame(hotelRoom.Object, reloadedHotelRoom.Room);
			Assert.AreEqual(HotelAdministrationActionKind.RoomReady, reloadedHotelRoom.Operation);
			Assert.AreEqual("guest ready", reloadedHotelRoom.Note);
			Assert.IsInstanceOfType(steps[19], typeof(HotelAdministrationActionStep));
			var reloadedHotelLost = (HotelAdministrationActionStep)steps[19];
			Assert.AreSame(lostProperty.Object, reloadedHotelLost.LostProperty);
			Assert.AreEqual(HotelAdministrationActionKind.LostPropertyAudit, reloadedHotelLost.Operation);
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void CataloguedShellStep_EnforcesAuthorityCapabilityAndLocationAndRecordsAudit()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(1, "Manager").Object;
		var destination = Cell(820, "sales floor").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var dispatcher = new EmploymentTaskDispatcher();

		var missingCapabilityTask = host.TaskBoard.CreateActiveTask("route check",
			new EmploymentActionPlan([new CataloguedActionShellStep("route", "check route", destination)]),
			manager);
		var context = new EmploymentTaskContext(host);
		var noCapability = Profile(manager, 1.0M, PaymentMethodKind.Cash, Caps());

		Assert.IsFalse(dispatcher.TryAssignTask(missingCapabilityTask, [noCapability], context, out var reason,
			blockWhenNoCandidateMatches: false));
		StringAssert.Contains(reason, "missing");

		var blockedPathTask = host.TaskBoard.CreateActiveTask("blocked route",
			new EmploymentActionPlan([new CataloguedActionShellStep("route", "check route", destination)]),
			manager);
		context.SetPathBlocked(destination);
		var deliveryProfile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanDeliverItems));

		Assert.IsFalse(dispatcher.TryAssignTask(blockedPathTask, [deliveryProfile], context, out reason,
			blockWhenNoCandidateMatches: false));
		StringAssert.Contains(reason, "cannot path");

		var clearContext = new EmploymentTaskContext(host);
		var task = host.TaskBoard.CreateActiveTask("audit report",
			new EmploymentActionPlan([new CataloguedActionShellStep("report", "stock counted")]),
			manager);
		var reportProfile = Profile(manager, 1.0M, PaymentMethodKind.Cash, Caps());

		Assert.IsTrue(dispatcher.TryAssignTask(task, [reportProfile], clearContext, out reason), reason);
		var result = dispatcher.AdvanceTask(task, clearContext);

		Assert.IsTrue(result.Success);
		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.AuditActionRecorded &&
			x.Description.Contains("stock counted", StringComparison.InvariantCultureIgnoreCase)));
	}

	[TestMethod]
	public void RouteBatchStep_RecordsMultiStopAllocationAndBlocksOversizedPlans()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(1, "Manager").Object;
		var stopOne = Cell(850, "north stall").Object;
		var stopTwo = Cell(851, "south stall").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(host);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanDeliverItems));
		var task = host.TaskBoard.CreateActiveTask("daily delivery batch",
			new EmploymentActionPlan([new CataloguedActionShellStep("routebatch",
				"total 100 each 40 daily carrot route", [stopOne, stopTwo])]),
			manager);

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		var result = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(result.Success, result.Message);
		StringAssert.Contains(task.StepOperationalStates[0].SelectedResources, "operation=routebatch");
		StringAssert.Contains(task.StepOperationalStates[0].SelectedResources, "stops=850,851");
		StringAssert.Contains(task.StepOperationalStates[0].SelectedResources, "total=100");
		StringAssert.Contains(task.StepOperationalStates[0].SelectedResources, "each=40");
		StringAssert.Contains(task.StepOperationalStates[0].SelectedResources, "planned=80");
		StringAssert.Contains(task.StepOperationalStates[0].SelectedResources, "remainder=20");
		StringAssert.Contains(task.StepOperationalStates[0].RouteResult, "daily carrot route");

		var oversized = host.TaskBoard.CreateActiveTask("oversized delivery batch",
			new EmploymentActionPlan([new CataloguedActionShellStep("routebatch",
				"total 50 each 30 oversized carrot route", [stopOne, stopTwo])]),
			manager);

		Assert.IsFalse(dispatcher.TryAssignTask(oversized, [profile], context, out reason,
			blockWhenNoCandidateMatches: false));
		StringAssert.Contains(reason, "allocates");
	}

	[TestMethod]
	public void TripCheckStep_RecordsTransportPoliciesAndPathChecksStops()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(1, "Manager").Object;
		var stopOne = Cell(852, "north road").Object;
		var stopTwo = Cell(853, "river depot").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(host);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanDeliverItems));
		var task = host.TaskBoard.CreateActiveTask("dawn transport check",
			new EmploymentActionPlan([new CataloguedActionShellStep("tripcheck",
				"fuel topped feed packed maintenance inspected rest scheduled dawn route", [stopOne, stopTwo])]),
			manager);

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		var result = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(result.Success, result.Message);
		StringAssert.Contains(task.StepOperationalStates[0].SelectedResources, "operation=tripcheck");
		StringAssert.Contains(task.StepOperationalStates[0].SelectedResources, "fuel=topped");
		StringAssert.Contains(task.StepOperationalStates[0].SelectedResources, "feed=packed");
		StringAssert.Contains(task.StepOperationalStates[0].SelectedResources, "maintenance=inspected");
		StringAssert.Contains(task.StepOperationalStates[0].SelectedResources, "rest=scheduled");
		StringAssert.Contains(task.StepOperationalStates[0].SelectedResources, "stops=852,853");
		StringAssert.Contains(task.StepOperationalStates[0].RouteResult, "dawn route");

		var blocked = host.TaskBoard.CreateActiveTask("blocked transport check",
			new EmploymentActionPlan([new CataloguedActionShellStep("tripcheck",
				"fuel topped feed packed maintenance inspected rest scheduled blocked route", [stopOne])]),
			manager);
		context.SetPathBlocked(stopOne);

		Assert.IsFalse(dispatcher.TryAssignTask(blocked, [profile], context, out reason,
			blockWhenNoCandidateMatches: false));
		StringAssert.Contains(reason, "cannot path");
	}

	[TestMethod]
	public void CataloguedInspectStep_RecordsTaskCustodyItemIds()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("workshop", currency.Object);
		var manager = Character(1, "Manager").Object;
		var source = Cell(840, "workbench").Object;
		var widget = Item(841, "finished widget", prototypeId: 9001);
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes | EmploymentAuthority.ManageCraftRules), null);
		var task = host.TaskBoard.CreateActiveTask("inspect widget",
			new EmploymentActionPlan([
				new GetItemsByIdActionStep(1, [widget.Object.Prototype.Id], [source]),
				new CataloguedActionShellStep("inspect", "check seams and finish")
			]),
			manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(host);
		context.SetAvailableItems(source, [widget.Object]);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanDeliverItems, EmploymentAICapability.CanCraft));
		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);

		var inspect = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(inspect.Success, inspect.Message);
		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		StringAssert.Contains(task.StepOperationalStates[1].OperationalPayload, "check seams and finish");
		StringAssert.Contains(task.StepOperationalStates[1].SelectedResources, "operation=inspect");
		StringAssert.Contains(task.StepOperationalStates[1].SelectedResources, widget.Object.Id.ToString("F0"));
		CollectionAssert.AreEquivalent(new[] { widget.Object.Id }, context.CarriedTaskItems(manager).Select(x => x.Id).ToArray());
	}

	[TestMethod]
	public void CataloguedBatchStep_RecordsDemandStoragePlanAndBlocksOversizedBatches()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("workshop", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageCraftRules), null);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(host);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanCraft));
		var task = host.TaskBoard.CreateActiveTask("batch linen",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("batch", "demand 12 storage 8 size 6 linen shirts before market day")
			]),
			manager);

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		var result = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(result.Success, result.Message);
		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		StringAssert.Contains(task.StepOperationalStates[0].OperationalPayload, "demand=12");
		StringAssert.Contains(task.StepOperationalStates[0].OperationalPayload, "storage=8");
		StringAssert.Contains(task.StepOperationalStates[0].OperationalPayload, "size=6");
		StringAssert.Contains(task.StepOperationalStates[0].SelectedResources, "operation=batch");

		var oversized = host.TaskBoard.CreateActiveTask("oversized batch",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("batch", "demand 12 storage 8 size 9 too many shirts")
			]),
			manager);

		Assert.IsFalse(dispatcher.TryAssignTask(oversized, [profile], context, out reason,
			blockWhenNoCandidateMatches: false));
		StringAssert.Contains(reason, "storage capacity");
	}

	[TestMethod]
	public void VehicleAssignStep_RecordsDriverVehicleAndBlocksCompetingActiveAssignment()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("transport yard", currency.Object);
		var driver = Character(1, "Driver").Object;
		var secondDriver = Character(2, "Second Driver").Object;
		var yard = Cell(900, "transport yard").Object;
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Id).Returns(901);
		vehicle.SetupGet(x => x.Name).Returns("delivery wagon");
		vehicle.SetupGet(x => x.FrameworkItemType).Returns("Vehicle");
		vehicle.SetupGet(x => x.Disabled).Returns(false);
		vehicle.SetupGet(x => x.Destroyed).Returns(false);
		vehicle.SetupGet(x => x.Location).Returns(yard);
		host.Hire(driver, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		host.Hire(secondDriver, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var task = host.TaskBoard.CreateActiveTask("assign wagon",
			new EmploymentActionPlan([
				new VehicleOperationActionStep(vehicle.Object),
				new CataloguedActionShellStep("report", "wagon assigned")
			]),
			driver);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(host);
		var driverProfile = Profile(driver, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanUseVehicles));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [driverProfile], context, out var reason), reason);
		var assignment = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(assignment.Success, assignment.Message);
		Assert.AreEqual(EmploymentTaskStatus.InProgress, task.Status);
		StringAssert.Contains(task.StepOperationalStates[0].SelectedResources, "operation=vehicleassign");
		StringAssert.Contains(task.StepOperationalStates[0].SelectedResources, "vehicle=901");
		StringAssert.Contains(task.StepOperationalStates[0].SelectedResources, "driver=1");

		var competing = host.TaskBoard.CreateActiveTask("competing wagon",
			new EmploymentActionPlan([new VehicleOperationActionStep(vehicle.Object)]),
			secondDriver);
		var secondProfile = Profile(secondDriver, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanUseVehicles));

		Assert.IsFalse(dispatcher.TryAssignTask(competing, [secondProfile], context, out reason,
			blockWhenNoCandidateMatches: false));
		StringAssert.Contains(reason, "already assigned");

		var report = dispatcher.AdvanceTask(task, context);
		Assert.IsTrue(report.Success, report.Message);
		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		var released = host.TaskBoard.CreateActiveTask("released wagon",
			new EmploymentActionPlan([new VehicleOperationActionStep(vehicle.Object)]),
			secondDriver);
		Assert.IsTrue(dispatcher.TryAssignTask(released, [secondProfile], context, out reason), reason);
	}

	[TestMethod]
	public void TaskAssignmentAudit_RequeuesWhenAssignedVehicleBecomesUnavailable()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("transport yard", currency.Object);
		var driverMock = Character(1, "Driver");
		var driver = driverMock.Object;
		var yard = Cell(920, "transport yard").Object;
		var vehicleDestroyed = false;
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Id).Returns(921);
		vehicle.SetupGet(x => x.Name).Returns("delivery wagon");
		vehicle.SetupGet(x => x.FrameworkItemType).Returns("Vehicle");
		vehicle.SetupGet(x => x.Disabled).Returns(false);
		vehicle.SetupGet(x => x.Destroyed).Returns(() => vehicleDestroyed);
		vehicle.SetupGet(x => x.Location).Returns(yard);
		var vehicles = new All<IVehicle>();
		vehicles.Add(vehicle.Object);
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>
		{
			[driver.Id] = driver
		}, [yard]);
		gameworld.SetupGet(x => x.Vehicles).Returns(vehicles);
		driverMock.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		driverMock.SetupGet(x => x.Location).Returns(yard);
		driverMock.SetupGet(x => x.State).Returns(CharacterState.Awake);
		host.Hire(driver, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var task = host.TaskBoard.CreateActiveTask("assign wagon",
			new EmploymentActionPlan([
				new VehicleOperationActionStep(vehicle.Object),
				new CataloguedActionShellStep("report", "wagon assigned")
			]),
			driver);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(host);
		var profile = Profile(driver, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanUseVehicles));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		vehicleDestroyed = true;
		var audit = host.TaskBoard.AuditActiveTaskAssignments();

		Assert.AreEqual(EmploymentTaskStatus.Pending, task.Status);
		Assert.IsNull(task.AssignedEmployee);
		Assert.AreEqual(1, audit.Count);
		Assert.AreEqual(EmploymentTaskAssignmentAuditOutcome.Requeued, audit.Single().Outcome);
		StringAssert.Contains(audit.Single().Reason, "destroyed");
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.ActiveTaskRequeued &&
			x.CorrelationId == task.CorrelationId));
	}

	[TestMethod]
	public void TaskAssignmentAudit_BlocksWhenVehicleUnavailableWithTaskItemCustody()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("transport yard", currency.Object);
		var driverMock = Character(1, "Driver");
		var driver = driverMock.Object;
		var yard = Cell(922, "transport yard").Object;
		var stockroom = Cell(923, "stockroom").Object;
		var body = new Mock<MudSharp.Body.IBody>();
		var item = Item(924, "cargo crate", prototypeId: 925);
		item.SetupGet(x => x.InInventoryOf).Returns(body.Object);
		driverMock.SetupGet(x => x.Body).Returns(body.Object);
		driverMock.SetupGet(x => x.Inventory).Returns([item.Object]);
		var vehicleDestroyed = false;
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Id).Returns(926);
		vehicle.SetupGet(x => x.Name).Returns("freight wagon");
		vehicle.SetupGet(x => x.FrameworkItemType).Returns("Vehicle");
		vehicle.SetupGet(x => x.Disabled).Returns(false);
		vehicle.SetupGet(x => x.Destroyed).Returns(() => vehicleDestroyed);
		vehicle.SetupGet(x => x.Location).Returns(yard);
		var vehicles = new All<IVehicle>();
		vehicles.Add(vehicle.Object);
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>
		{
			[driver.Id] = driver
		}, [yard, stockroom], new Dictionary<long, IGameItem>
		{
			[item.Object.Id] = item.Object
		});
		gameworld.SetupGet(x => x.Vehicles).Returns(vehicles);
		driverMock.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		driverMock.SetupGet(x => x.Location).Returns(stockroom);
		driverMock.SetupGet(x => x.State).Returns(CharacterState.Awake);
		host.Hire(driver, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var task = host.TaskBoard.CreateActiveTask("collect cargo",
			new EmploymentActionPlan([
				new VehicleOperationActionStep(vehicle.Object),
				new GetItemsByIdActionStep(1, [925], [stockroom]),
				new CataloguedActionShellStep("report", "cargo collected")
			]),
			driver);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(host);
		context.SetAvailableItems(stockroom, [item.Object]);
		var profile = Profile(driver, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanUseVehicles, EmploymentAICapability.CanDeliverItems));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		vehicleDestroyed = true;
		var audit = host.TaskBoard.AuditActiveTaskAssignments();

		Assert.AreEqual(EmploymentTaskStatus.Blocked, task.Status);
		Assert.AreSame(driver, task.AssignedEmployee);
		Assert.AreEqual(1, audit.Count);
		Assert.AreEqual(EmploymentTaskAssignmentAuditOutcome.Blocked, audit.Single().Outcome);
		StringAssert.Contains(task.BlockedReason, "destroyed");
		StringAssert.Contains(task.BlockedReason, "physical task items");
	}

	[TestMethod]
	public void TaskAssignmentAudit_RequeuesWhenAssignedAnimalBecomesUnavailable()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("stable", currency.Object);
		var handlerMock = Character(1, "Handler");
		var handler = handlerMock.Object;
		var yard = Cell(930, "yard").Object;
		var paddock = Cell(931, "paddock").Object;
		var mountState = CharacterState.Awake;
		var mount = new Mock<ICharacter>();
		mount.SetupGet(x => x.Id).Returns(932);
		mount.SetupGet(x => x.Name).Returns("bay mare");
		mount.SetupGet(x => x.FrameworkItemType).Returns("Character");
		mount.SetupGet(x => x.State).Returns(() => mountState);
		mount.SetupGet(x => x.Location).Returns(yard);
		mount.Setup(x => x.CanEverBeMounted(handler)).Returns(true);
		mount.Setup(x => x.CanBeMountedBy(handler)).Returns(true);
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>
		{
			[handler.Id] = handler,
			[mount.Object.Id] = mount.Object
		}, [yard, paddock]);
		handlerMock.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		handlerMock.SetupGet(x => x.Location).Returns(yard);
		handlerMock.SetupGet(x => x.State).Returns(CharacterState.Awake);
		host.Hire(handler, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var task = host.TaskBoard.CreateActiveTask("lead mount",
			new EmploymentActionPlan([
				new StableAnimalOperationActionStep(EmploymentAnimalOperationKind.Lead, mount.Object, destination: paddock),
				new CataloguedActionShellStep("report", "mare moved")
			]),
			handler);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(host);
		var profile = Profile(handler, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanManageStableAnimals));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		mountState = CharacterState.Dead;
		var audit = host.TaskBoard.AuditActiveTaskAssignments();

		Assert.AreEqual(EmploymentTaskStatus.Pending, task.Status);
		Assert.AreEqual(1, audit.Count);
		Assert.AreEqual(EmploymentTaskAssignmentAuditOutcome.Requeued, audit.Single().Outcome);
		StringAssert.Contains(audit.Single().Reason, "dead");
	}

	[TestMethod]
	public void TaskAssignmentAudit_RequeuesWhenRecordedRouteDestinationIsMissing()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("transport yard", currency.Object);
		var driverMock = Character(1, "Driver");
		var driver = driverMock.Object;
		var destination = Cell(940, "dock").Object;
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>
		{
			[driver.Id] = driver
		});
		driverMock.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		driverMock.SetupGet(x => x.Location).Returns(destination);
		driverMock.SetupGet(x => x.State).Returns(CharacterState.Awake);
		host.Hire(driver, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var task = host.TaskBoard.CreateActiveTask("plan route",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("route", "dock run", [destination]),
				new CataloguedActionShellStep("report", "route planned")
			]),
			driver);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(host);
		var profile = Profile(driver, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanDeliverItems));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		var audit = host.TaskBoard.AuditActiveTaskAssignments();

		Assert.AreEqual(EmploymentTaskStatus.Pending, task.Status);
		Assert.AreEqual(1, audit.Count);
		Assert.AreEqual(EmploymentTaskAssignmentAuditOutcome.Requeued, audit.Single().Outcome);
		StringAssert.Contains(audit.Single().Reason, "destination");
	}

	[TestMethod]
	public void VehicleCargoStep_SelectsCompartmentProjectionAndChecksCarriedCapacity()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("transport yard", currency.Object);
		var driver = Character(1, "Driver").Object;
		var source = Cell(902, "warehouse").Object;
		var yard = Cell(903, "wagon bay").Object;
		host.Hire(driver, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);

		Mock<IVehicle> Vehicle(long id, string name)
		{
			var vehicle = new Mock<IVehicle>();
			vehicle.SetupGet(x => x.Id).Returns(id);
			vehicle.SetupGet(x => x.Name).Returns(name);
			vehicle.SetupGet(x => x.FrameworkItemType).Returns("Vehicle");
			vehicle.SetupGet(x => x.Disabled).Returns(false);
			vehicle.SetupGet(x => x.Destroyed).Returns(false);
			vehicle.SetupGet(x => x.Location).Returns(yard);
			return vehicle;
		}

		Mock<IVehicleCargoSpace> CargoSpace(IVehicle vehicle, long id, string name, long compartmentId,
			IGameItem projection)
		{
			var compartment = new Mock<IVehicleCompartmentPrototype>();
			compartment.SetupGet(x => x.Id).Returns(compartmentId);
			compartment.SetupGet(x => x.Name).Returns("main compartment");
			compartment.SetupGet(x => x.FrameworkItemType).Returns("VehicleCompartmentPrototype");
			var prototype = new Mock<IVehicleCargoSpacePrototype>();
			prototype.SetupGet(x => x.Id).Returns(id + 1_000);
			prototype.SetupGet(x => x.Name).Returns($"{name} prototype");
			prototype.SetupGet(x => x.FrameworkItemType).Returns("VehicleCargoSpacePrototype");
			prototype.SetupGet(x => x.Compartment).Returns(compartment.Object);
			var cargoSpace = new Mock<IVehicleCargoSpace>();
			cargoSpace.SetupGet(x => x.Id).Returns(id);
			cargoSpace.SetupGet(x => x.Name).Returns(name);
			cargoSpace.SetupGet(x => x.FrameworkItemType).Returns("VehicleCargoSpace");
			cargoSpace.SetupGet(x => x.Vehicle).Returns(vehicle);
			cargoSpace.SetupGet(x => x.Prototype).Returns(prototype.Object);
			cargoSpace.SetupGet(x => x.ProjectionItem).Returns(projection);
			cargoSpace.SetupGet(x => x.ProjectionItemId).Returns(projection.Id);
			cargoSpace.SetupGet(x => x.IsDisabled).Returns(false);
			var accessReason = string.Empty;
			cargoSpace.Setup(x => x.CanAccess(It.IsAny<ICharacter>(), out accessReason)).Returns(true);
			return cargoSpace;
		}

		var wagon = Vehicle(900, "delivery wagon");
		var cargoItem = Item(904, "cargo crate").Object;
		var cargoHold = ContainerItem(905, "wagon cargo hold", [yard], [], []).Object;
		var cargoSpace = CargoSpace(wagon.Object, 906, "rear hold", 907, cargoHold);
		var context = new EmploymentTaskContext(host);
		context.SetAvailableItems(source, [cargoItem]);
		context.SetItemTags(cargoItem, "freight");
		var plan = new EmploymentActionPlan([
			new GetItemsByTagActionStep(1, "freight", [source]),
			new VehicleOperationActionStep(wagon.Object, cargoSpace.Object)
		]);
		var task = host.TaskBoard.CreateActiveTask("select cargo hold", plan, driver);
		var dispatcher = new EmploymentTaskDispatcher();
		var profile = Profile(driver, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanDeliverItems, EmploymentAICapability.CanUseVehicles));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		var cargoSelection = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(cargoSelection.Success, cargoSelection.Message);
		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		StringAssert.Contains(task.StepOperationalStates[1].SelectedResources, "operation=vehiclecargo");
		StringAssert.Contains(task.StepOperationalStates[1].SelectedResources, "vehicle=900");
		StringAssert.Contains(task.StepOperationalStates[1].SelectedResources, "cargo=906");
		StringAssert.Contains(task.StepOperationalStates[1].SelectedResources, "compartment=907");
		StringAssert.Contains(task.StepOperationalStates[1].SelectedResources, "projection=905");
		StringAssert.Contains(task.StepOperationalStates[1].SelectedResources, "capacity=checked");
		StringAssert.Contains(task.StepOperationalStates[1].SelectedResources, "carried=1");
		CollectionAssert.AreEquivalent(new[] { cargoItem.Id }, context.CarriedTaskItems(driver).Select(x => x.Id).ToArray());

		var oversizedItem = Item(908, "oversized cargo crate").Object;
		var blockedProjection = Item(909, "small side box", [yard]);
		var rejectingContainer = new Mock<IContainer>();
		rejectingContainer.Setup(x => x.CanPut(oversizedItem)).Returns(false);
		blockedProjection.Setup(x => x.GetItemType<IContainer>()).Returns(rejectingContainer.Object);
		var blockedWagon = Vehicle(910, "small delivery cart");
		var blockedCargoSpace = CargoSpace(blockedWagon.Object, 911, "side box", 912, blockedProjection.Object);
		var blockedContext = new EmploymentTaskContext(host);
		blockedContext.SetAvailableItems(source, [oversizedItem]);
		blockedContext.SetItemTags(oversizedItem, "oversized");
		var blockedTask = host.TaskBoard.CreateActiveTask("reject cargo hold", new EmploymentActionPlan([
			new GetItemsByTagActionStep(1, "oversized", [source]),
			new VehicleOperationActionStep(blockedWagon.Object, blockedCargoSpace.Object)
		]), driver);

		Assert.IsTrue(dispatcher.TryAssignTask(blockedTask, [profile], blockedContext, out reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(blockedTask, blockedContext).Success);
		var blockedSelection = dispatcher.AdvanceTask(blockedTask, blockedContext);

		Assert.IsFalse(blockedSelection.Success);
		StringAssert.Contains(blockedSelection.Message, "cannot contain oversized cargo crate");
	}

	[TestMethod]
	public void StableAnimalOperationSteps_RecordLeadRideLodgeAndReturnEvidence()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("stable", currency.Object);
		var handler = Character(1, "Handler").Object;
		var yard = Cell(910, "yard").Object;
		var paddock = Cell(911, "paddock").Object;
		var stableCell = Cell(912, "stable stall").Object;
		var mount = new Mock<ICharacter>();
		mount.SetupGet(x => x.Id).Returns(913);
		mount.SetupGet(x => x.Name).Returns("bay mare");
		mount.SetupGet(x => x.FrameworkItemType).Returns("Character");
		mount.SetupGet(x => x.Location).Returns(yard);
		mount.Setup(x => x.CanEverBeMounted(handler)).Returns(true);
		mount.Setup(x => x.CanBeMountedBy(handler)).Returns(true);
		mount.Setup(x => x.WhyCannotBeMountedBy(handler)).Returns("not mountable");
		mount.Setup(x => x.Mount(handler)).Returns(true);
		var stable = new Mock<IStable>();
		stable.SetupGet(x => x.Id).Returns(914);
		stable.SetupGet(x => x.Name).Returns("north stable");
		stable.SetupGet(x => x.FrameworkItemType).Returns("Stable");
		stable.SetupGet(x => x.Location).Returns(stableCell);
		var stay = new Mock<IStableStay>();
		stay.SetupGet(x => x.Id).Returns(915);
		stay.SetupGet(x => x.Stable).Returns(stable.Object);
		stay.SetupGet(x => x.Mount).Returns(mount.Object);
		stay.SetupGet(x => x.MountId).Returns(mount.Object.Id);
		stay.SetupGet(x => x.MountInstanceId).Returns((long?)null);
		stay.SetupGet(x => x.IsActive).Returns(true);
		stable.SetupGet(x => x.Stays).Returns([stay.Object]);
		stable.Setup(x => x.CanLodge(handler, mount.Object)).Returns((true, string.Empty));
		stable.Setup(x => x.Lodge(handler, mount.Object)).Returns(stay.Object);
		host.Hire(handler, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var task = host.TaskBoard.CreateActiveTask("manage mount",
			new EmploymentActionPlan([
				new StableAnimalOperationActionStep(EmploymentAnimalOperationKind.Lead, mount.Object, destination: paddock),
				new StableAnimalOperationActionStep(EmploymentAnimalOperationKind.Ride, mount.Object),
				new StableAnimalOperationActionStep(EmploymentAnimalOperationKind.Lodge, mount.Object, stable.Object),
				new StableAnimalOperationActionStep(EmploymentAnimalOperationKind.Return, stable: stable.Object,
					stay: stay.Object, waiveFees: true)
			]),
			handler);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(host);
		var profile = Profile(handler, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanManageStableAnimals));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		for (var i = 0; i < 4; i++)
		{
			var result = dispatcher.AdvanceTask(task, context);
			Assert.IsTrue(result.Success, result.Message);
		}

		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		StringAssert.Contains(task.StepOperationalStates[0].SelectedResources, "operation=animallead");
		StringAssert.Contains(task.StepOperationalStates[0].SelectedResources, "destination=911");
		StringAssert.Contains(task.StepOperationalStates[2].SelectedResources, "operation=animallodge");
		StringAssert.Contains(task.StepOperationalStates[2].SelectedResources, "stay=915");
		StringAssert.Contains(task.StepOperationalStates[3].SelectedResources, "operation=animalreturn");
		mount.Verify(x => x.Mount(handler), Times.Once);
		stable.Verify(x => x.Lodge(handler, mount.Object), Times.Once);
		stable.Verify(x => x.Release(handler, stay.Object, true), Times.Once);
	}


	[TestMethod]
	public void StableAdministrationActionSteps_AssessFeesAndRecordCareEvidence()
	{
		var currency = Currency();
		var stableCell = Cell(950, "stable stall").Object;
		var stable = new Mock<IStable>();
		stable.SetupGet(x => x.Id).Returns(951);
		stable.SetupGet(x => x.Name).Returns("north stable");
		stable.SetupGet(x => x.FrameworkItemType).Returns("Stable");
		stable.SetupGet(x => x.EmploymentHostName).Returns("north stable");
		stable.SetupGet(x => x.EmploymentHostType).Returns(EmploymentHostType.Stable);
		stable.SetupGet(x => x.Market).Returns((IMarket?)null);
		stable.SetupGet(x => x.Currency).Returns(currency.Object);
		stable.SetupGet(x => x.Location).Returns(stableCell);
		var state = new EmploymentHostState(stable.Object);
		stable.SetupGet(x => x.Employment).Returns(state);
		stable.SetupGet(x => x.BusinessLedger).Returns(state.BusinessLedger);
		stable.SetupGet(x => x.EmploymentRegister).Returns(state.EmploymentRegister);
		stable.SetupGet(x => x.TaskBoard).Returns(state.TaskBoard);
		stable.SetupGet(x => x.ManagerGoalBoard).Returns(state.ManagerGoalBoard);
		stable.SetupGet(x => x.Payroll).Returns(state.Payroll);
		stable.SetupGet(x => x.EmploymentContracts).Returns(() => state.EmploymentContracts);
		stable.SetupGet(x => x.JobOpenings).Returns(() => state.JobOpenings);
		stable.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		      .Returns((ICharacter actor, EmploymentAuthority authority) => state.HasAuthority(actor, authority));
		var owing = 4.0M;
		var stay = new Mock<IStableStay>();
		stay.SetupGet(x => x.Id).Returns(952);
		stay.SetupGet(x => x.Name).Returns("bay mare stay");
		stay.SetupGet(x => x.Stable).Returns(stable.Object);
		stay.SetupGet(x => x.IsActive).Returns(true);
		stay.SetupGet(x => x.AmountOwing).Returns(() => owing);
		var account = new Mock<IStableAccount>();
		account.SetupGet(x => x.Id).Returns(953);
		account.SetupGet(x => x.Name).Returns("account1");
		account.SetupGet(x => x.AccountName).Returns("account1");
		account.SetupGet(x => x.Stable).Returns(stable.Object);
		account.SetupGet(x => x.Currency).Returns(currency.Object);
		account.SetupGet(x => x.Balance).Returns(12.0M);
		account.SetupGet(x => x.CreditAvailable).Returns(30.0M);
		stable.SetupGet(x => x.Stays).Returns([stay.Object]);
		stable.SetupGet(x => x.ActiveStays).Returns([stay.Object]);
		stable.SetupGet(x => x.StableAccounts).Returns([account.Object]);
		stable.Setup(x => x.AssessFees(stay.Object)).Callback(() => owing += 5.0M);
		var manager = Character(1, "Stable Manager").Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ManageDeliveryRoutes |
			EmploymentAuthority.UseStoreAccount), null);
		var task = state.TaskBoard.CreateActiveTask("stable administration",
			new EmploymentActionPlan([
				new StableAdministrationActionStep(stable.Object, StableAdministrationActionKind.CareFeed,
					stay: stay.Object, note: "hay ration topped"),
				new StableAdministrationActionStep(stable.Object, StableAdministrationActionKind.FeeAssessment,
					stay: stay.Object, note: "daily fee"),
				new StableAdministrationActionStep(stable.Object, StableAdministrationActionKind.FeeAssessment,
					note: "daily sweep"),
				new StableAdministrationActionStep(stable.Object, StableAdministrationActionKind.AccountReconciliation,
					account: account.Object, note: "monthly check")
			]),
			manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(stable.Object);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanManageStableAnimals));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		while (task.Status is not EmploymentTaskStatus.Completed and not EmploymentTaskStatus.Failed)
		{
			var result = dispatcher.AdvanceTask(task, context);
			Assert.IsTrue(result.Success, result.Message);
		}

		Assert.AreEqual(9.0M, owing);
		stable.Verify(x => x.AssessFees(stay.Object), Times.Once);
		stable.Verify(x => x.AssessAllActiveStays(), Times.Once);
		Assert.AreEqual(4, state.EmploymentRegister.Entries.Count(x =>
			x.EntryType == EmploymentRegisterEntryType.AuditActionRecorded));
		StringAssert.Contains(task.StepOperationalStates[0].OperationalPayload!, "stableadmin:CareFeed");
		StringAssert.Contains(task.StepOperationalStates[1].SelectedResources, "stay=952");
		StringAssert.Contains(task.StepOperationalStates[3].SelectedResources, "account=953");
	}

	[TestMethod]
	public void HotelAdministrationActionSteps_CheckLostPropertyAndRecordRoomEvidence()
	{
		var currency = Currency();
		var property = new Mock<IProperty>();
		property.SetupGet(x => x.Id).Returns(960);
		property.SetupGet(x => x.Name).Returns("harbour house");
		var hotel = new Mock<IHotel>();
		hotel.SetupGet(x => x.Id).Returns(9600);
		hotel.SetupGet(x => x.Name).Returns("harbour house hotel");
		hotel.SetupGet(x => x.FrameworkItemType).Returns("Hotel");
		hotel.SetupGet(x => x.EmploymentHostName).Returns("harbour house hotel");
		hotel.SetupGet(x => x.EmploymentHostType).Returns(EmploymentHostType.Hotel);
		hotel.SetupGet(x => x.Market).Returns((IMarket?)null);
		hotel.SetupGet(x => x.Currency).Returns(currency.Object);
		hotel.SetupGet(x => x.Property).Returns(property.Object);
		property.SetupGet(x => x.Hotel).Returns(hotel.Object);
		var state = new EmploymentHostState(hotel.Object);
		hotel.SetupGet(x => x.Employment).Returns(state);
		hotel.SetupGet(x => x.BusinessLedger).Returns(state.BusinessLedger);
		hotel.SetupGet(x => x.EmploymentRegister).Returns(state.EmploymentRegister);
		hotel.SetupGet(x => x.TaskBoard).Returns(state.TaskBoard);
		hotel.SetupGet(x => x.ManagerGoalBoard).Returns(state.ManagerGoalBoard);
		hotel.SetupGet(x => x.Payroll).Returns(state.Payroll);
		hotel.SetupGet(x => x.EmploymentContracts).Returns(() => state.EmploymentContracts);
		hotel.SetupGet(x => x.JobOpenings).Returns(() => state.JobOpenings);
		hotel.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		     .Returns((ICharacter actor, EmploymentAuthority authority) => state.HasAuthority(actor, authority));
		var roomCell = Cell(961, "blue room").Object;
		var room = new Mock<IHotelRoom>();
		room.SetupGet(x => x.Name).Returns("blue room");
		room.SetupGet(x => x.Cell).Returns(roomCell);
		room.SetupGet(x => x.Property).Returns(property.Object);
		hotel.SetupGet(x => x.Rooms).Returns([room.Object]);
		var lost = new Mock<IHotelLostProperty>();
		lost.SetupGet(x => x.BundleId).Returns(962);
		lost.SetupGet(x => x.Description).Returns("forgotten valise");
		lost.SetupGet(x => x.Status).Returns(HotelLostPropertyStatus.Held);
		var balance = new Mock<IHotelPatronBalance>();
		balance.SetupGet(x => x.PatronId).Returns(963);
		balance.SetupGet(x => x.Balance).Returns(17.5M);
		property.SetupGet(x => x.HotelLostProperties).Returns([lost.Object]);
		property.SetupGet(x => x.HotelPatronBalances).Returns([balance.Object]);
		var manager = Character(1, "Hotel Manager").Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ManageDeliveryRoutes |
			EmploymentAuthority.UseStoreAccount), null);
		var task = state.TaskBoard.CreateActiveTask("hotel administration",
			new EmploymentActionPlan([
				new HotelAdministrationActionStep(hotel.Object, HotelAdministrationActionKind.RoomClean,
					room: room.Object, note: "linen refreshed"),
				new HotelAdministrationActionStep(hotel.Object, HotelAdministrationActionKind.LostPropertyCheck,
					note: "retention sweep"),
				new HotelAdministrationActionStep(hotel.Object, HotelAdministrationActionKind.LostPropertyAudit,
					lostProperty: lost.Object, note: "owner matched"),
				new HotelAdministrationActionStep(hotel.Object, HotelAdministrationActionKind.PatronBalanceAudit,
					patronBalance: balance.Object, patronSelector: "963", note: "monthly check")
			]),
			manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(hotel.Object);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanManageHotelRooms));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		while (task.Status is not EmploymentTaskStatus.Completed and not EmploymentTaskStatus.Failed)
		{
			var result = dispatcher.AdvanceTask(task, context);
			Assert.IsTrue(result.Success, result.Message);
		}

		property.Verify(x => x.CheckHotelLostProperty(), Times.Once);
		Assert.AreEqual(4, state.EmploymentRegister.Entries.Count(x =>
			x.EntryType == EmploymentRegisterEntryType.AuditActionRecorded));
		StringAssert.Contains(task.StepOperationalStates[0].OperationalPayload!, "hoteladmin:RoomClean");
		StringAssert.Contains(task.StepOperationalStates[0].SelectedResources, "room=961");
		StringAssert.Contains(task.StepOperationalStates[2].SelectedResources, "lost=962");
		StringAssert.Contains(task.StepOperationalStates[3].SelectedResources, "patron=963");
	}

	[TestMethod]
	public void CataloguedAuthoriseStep_AuthorisesLaterFinancialSteps()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ApprovePurchases), null);
		var task = host.TaskBoard.CreateActiveTask("authorised purchase",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("authorise", "approve thread purchase"),
				new PurchaseActionStep("purchase thread", new MoneyAmount(currency.Object, 5.0M))
			]),
			manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(host);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanPurchaseCommodities));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		var purchaseResult = AdvanceTaskOrAssignmentFailure(dispatcher, task, context, profile);

		Assert.IsTrue(purchaseResult.Success);
		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		Assert.IsTrue(host.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.Purchase));
	}

	[TestMethod]
	public void CataloguedAuthoriseStep_DoesNotSelfAuthoriseWithoutTaskGrant()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ApprovePurchases), null);
		var correlationId = Guid.NewGuid();
		var principal = EmploymentPrincipal.ForCharacter(manager);
		var provenance = new EmploymentTaskProvenance(
			EmploymentTaskSourceKind.Manual,
			null,
			null,
			principal,
			principal,
			new EmploymentAuthorisationGrant(
				Guid.NewGuid(),
				principal,
				EmploymentAuthority.AssignTasks,
				new Dictionary<long, decimal>(),
				false,
				DateTimeOffset.UtcNow,
				null,
				correlationId,
				new HashSet<string>(StringComparer.InvariantCultureIgnoreCase),
				new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)),
			0,
			DateTimeOffset.UtcNow);
		var task = host.TaskBoard.CreateActiveTask("self authorised purchase",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("authorise", "approve thread purchase", new MoneyAmount(currency.Object, 10.0M)),
				new PurchaseActionStep("purchase thread", new MoneyAmount(currency.Object, 5.0M))
			]),
			manager,
			correlationId,
			provenance: provenance);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(host);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanPurchaseCommodities));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		var purchaseResult = AdvanceTaskOrAssignmentFailure(dispatcher, task, context, profile);

		Assert.IsFalse(purchaseResult.Success);
		StringAssert.Contains(purchaseResult.Message, "requires an auditable payment authorisation");
		Assert.AreEqual(EmploymentTaskStatus.Blocked, task.Status);
		Assert.IsFalse(host.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.Purchase));
	}

	[TestMethod]
	public void ActiveTaskStepState_ClearsFailureDiagnosticWhenRetryCompletes()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(1, "Manager").Object;
		var destination = Cell(830, "sales floor").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var task = host.TaskBoard.CreateActiveTask("route after report",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("report", "stock counted"),
				new CataloguedActionShellStep("route", "check route", destination)
			]),
			manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(host);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanDeliverItems));
		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		var blockedContext = new EmploymentTaskContext(host);
		blockedContext.SetPathBlocked(destination);

		var blocked = dispatcher.AdvanceTask(task, blockedContext);

		Assert.IsFalse(blocked.Success);
		Assert.IsFalse(string.IsNullOrWhiteSpace(task.StepOperationalStates[1].FailureDiagnostic));

		var retry = dispatcher.AdvanceTask(task, new EmploymentTaskContext(host));

		Assert.IsTrue(retry.Success);
		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		Assert.IsTrue(string.IsNullOrWhiteSpace(task.StepOperationalStates[1].FailureDiagnostic));
	}

	[TestMethod]
	public void EmploymentCraftReservations_CancelActiveTaskReleasesReservedItems()
	{
		var currency = Currency();
		var manager = Character(1, "Manager");
		var input = ReservationTrackedItem(101, "reserved ore", out var inputPredicates);
		var tool = ReservationTrackedItem(102, "reserved hammer", out var toolPredicates);
		var station = ReservationTrackedItem(103, "reserved forge", out var stationPredicates);
		var items = new Dictionary<long, IGameItem>
		{
			[input.Object.Id] = input.Object,
			[tool.Object.Id] = tool.Object,
			[station.Object.Id] = station.Object
		};
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>
		{
			[manager.Object.Id] = manager.Object
		}, items: items);
		manager.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		host.Hire(manager.Object, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.CancelTasks), null);
		var task = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("abandoned craft",
			new EmploymentActionPlan([new CataloguedActionShellStep("report", "craft abandoned")]),
			manager.Object);
		task.MarkStep(0, EmploymentActionStepStatus.Pending,
			new EmploymentActionStepOperationalState(
				CraftJobReference: CraftReservationStateJson(input.Object.Id, tool.Object.Id, station.Object.Id)));

		Assert.IsTrue(host.TaskBoard.CancelActiveTask(task, manager.Object, "craft was abandoned"));

		AssertReservationCleanup(input, inputPredicates, task);
		AssertReservationCleanup(tool, toolPredicates, task);
		AssertReservationCleanup(station, stationPredicates, task);
	}

	[TestMethod]
	public void EmploymentCraftReservations_CompletedTaskReleasesReservedItems()
	{
		var currency = Currency();
		var manager = Character(1, "Manager");
		var input = ReservationTrackedItem(201, "reserved ore", out var inputPredicates);
		var tool = ReservationTrackedItem(202, "reserved hammer", out var toolPredicates);
		var station = ReservationTrackedItem(203, "reserved forge", out var stationPredicates);
		var items = new Dictionary<long, IGameItem>
		{
			[input.Object.Id] = input.Object,
			[tool.Object.Id] = tool.Object,
			[station.Object.Id] = station.Object
		};
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>
		{
			[manager.Object.Id] = manager.Object
		}, items: items);
		manager.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		host.Hire(manager.Object, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks), null);
		var task = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("finish craft",
			new EmploymentActionPlan([new CataloguedActionShellStep("report", "craft finished")]),
			manager.Object);
		task.MarkStep(0, EmploymentActionStepStatus.Pending,
			new EmploymentActionStepOperationalState(
				CraftJobReference: CraftReservationStateJson(input.Object.Id, tool.Object.Id, station.Object.Id)));
		task.Assign(manager.Object);
		var dispatcher = new EmploymentTaskDispatcher();

		var result = dispatcher.AdvanceTask(task, new EmploymentTaskContext(host));

		Assert.IsTrue(result.Success);
		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		AssertReservationCleanup(input, inputPredicates, task);
		AssertReservationCleanup(tool, toolPredicates, task);
		AssertReservationCleanup(station, stationPredicates, task);
	}

	[TestMethod]
	public void EmploymentCraftReservations_CraftStationReservationUsesFiniteExpiry()
	{
		var currency = Currency();
		var manager = Character(1, "Manager");
		var cell = Cell(10, "workshop").Object;
		var station = Item(301, "reserved forge");
		var addedEffects = new List<EmploymentCraftReservationEffect>();
		var addedDurations = new List<TimeSpan>();
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>
		{
			[manager.Object.Id] = manager.Object
		}, [cell], new Dictionary<long, IGameItem>
		{
			[station.Object.Id] = station.Object
		});
		gameworld.Setup(x => x.GetStaticDouble("EmploymentCraftReservationDurationMinutes")).Returns(7.0);
		manager.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		manager.SetupGet(x => x.Location).Returns(cell);
		manager.Setup(x => x.TargetLocalOrHeldItem("forge")).Returns(station.Object);
		station.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		station.Setup(x => x.AddEffect(It.IsAny<MudSharp.Effects.IEffect>(), It.IsAny<TimeSpan>()))
		       .Callback<MudSharp.Effects.IEffect, TimeSpan>((effect, duration) =>
		       {
			       addedEffects.Add((EmploymentCraftReservationEffect)effect);
			       addedDurations.Add(duration);
		       });
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		host.Hire(manager.Object, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageCraftRules), null);
		var task = host.TaskBoard.CreateActiveTask("reserve forge",
			new EmploymentActionPlan([new CraftStationActionStep("forge")]),
			manager.Object);
		var context = new EmploymentTaskContext(host);
		context.HydrateTaskState(task, 0);

		Assert.IsTrue(context.TryUseCraftStation(manager.Object, "forge", out var reason, out var state), reason);

		Assert.IsFalse(string.IsNullOrWhiteSpace(state.CraftJobReference));
		Assert.AreEqual(1, addedEffects.Count);
		Assert.AreEqual(task.CorrelationId, addedEffects.Single().CorrelationId);
		Assert.IsTrue(addedDurations.Single() > TimeSpan.FromMinutes(6.5));
		Assert.IsTrue(addedDurations.Single() <= TimeSpan.FromMinutes(7.0));
	}

	[TestMethod]
	public void EmploymentCraftReservations_CraftStationReservationRejectsConflictingActiveLock()
	{
		var currency = Currency();
		var manager = Character(1, "Manager");
		var cell = Cell(10, "workshop").Object;
		var station = Item(302, "reserved forge");
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>
		{
			[manager.Object.Id] = manager.Object
		}, [cell], new Dictionary<long, IGameItem>
		{
			[station.Object.Id] = station.Object
		});
		gameworld.Setup(x => x.GetStaticDouble("EmploymentCraftReservationDurationMinutes")).Returns(7.0);
		manager.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		manager.SetupGet(x => x.Location).Returns(cell);
		manager.Setup(x => x.TargetLocalOrHeldItem("forge")).Returns(station.Object);
		station.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var existingReservation = new EmploymentCraftReservationEffect(
			station.Object,
			Guid.NewGuid(),
			Guid.NewGuid(),
			"other craft task",
			"craft station forge",
			DateTimeOffset.UtcNow.AddMinutes(5));
		SetupCraftReservationEffects(station, [existingReservation]);
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		host.Hire(manager.Object, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageCraftRules), null);
		var task = host.TaskBoard.CreateActiveTask("reserve forge",
			new EmploymentActionPlan([new CraftStationActionStep("forge")]),
			manager.Object);
		var context = new EmploymentTaskContext(host);
		context.HydrateTaskState(task, 0);

		Assert.IsFalse(context.TryUseCraftStation(manager.Object, "forge", out var reason, out _));

		StringAssert.Contains(reason, "1/1 employment craft station reservations");
		station.Verify(x => x.AddEffect(It.IsAny<MudSharp.Effects.IEffect>(), It.IsAny<TimeSpan>()), Times.Never);
	}

	[TestMethod]
	public void EmploymentCraftReservations_CraftStationCapacityAllowsConfiguredConcurrentReservations()
	{
		var currency = Currency();
		var manager = Character(1, "Manager");
		var cell = Cell(10, "workshop").Object;
		var station = Item(303, "shared forge");
		var addedEffects = new List<EmploymentCraftReservationEffect>();
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>
		{
			[manager.Object.Id] = manager.Object
		}, [cell], new Dictionary<long, IGameItem>
		{
			[station.Object.Id] = station.Object
		});
		gameworld.Setup(x => x.GetStaticDouble("EmploymentCraftReservationDurationMinutes")).Returns(7.0);
		gameworld.Setup(x => x.GetStaticDouble("EmploymentCraftStationReservationCapacity")).Returns(2.0);
		manager.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		manager.SetupGet(x => x.Location).Returns(cell);
		manager.Setup(x => x.TargetLocalOrHeldItem("forge")).Returns(station.Object);
		station.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		station.Setup(x => x.AddEffect(It.IsAny<MudSharp.Effects.IEffect>(), It.IsAny<TimeSpan>()))
		       .Callback<MudSharp.Effects.IEffect, TimeSpan>((effect, _) =>
		       {
			       addedEffects.Add((EmploymentCraftReservationEffect)effect);
		       });
		var existingReservation = new EmploymentCraftReservationEffect(
			station.Object,
			Guid.NewGuid(),
			Guid.NewGuid(),
			"other craft task",
			"craft station forge",
			DateTimeOffset.UtcNow.AddMinutes(5));
		SetupCraftReservationEffects(station, [existingReservation]);
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		host.Hire(manager.Object, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageCraftRules), null);
		var task = host.TaskBoard.CreateActiveTask("reserve shared forge",
			new EmploymentActionPlan([new CraftStationActionStep("forge")]),
			manager.Object);
		var context = new EmploymentTaskContext(host);
		context.HydrateTaskState(task, 0);

		Assert.IsTrue(context.TryUseCraftStation(manager.Object, "forge", out var reason, out var state), reason);

		Assert.AreEqual(1, addedEffects.Count);
		Assert.AreEqual(task.CorrelationId, addedEffects.Single().CorrelationId);
		Assert.IsTrue(state.ReservationReference?.Contains("capacity=2") == true);
	}

	[TestMethod]
	public void EmploymentCraftReservations_CraftStartRejectsConflictingResourceLock()
	{
		var currency = Currency();
		var manager = Character(1, "Manager");
		var cell = Cell(10, "workshop").Object;
		var reservedInput = Item(402, "reserved ore");
		var craft = new Mock<ICraft>();
		craft.SetupGet(x => x.Id).Returns(501);
		craft.SetupGet(x => x.Name).Returns("reserve craft");
		craft.SetupGet(x => x.FrameworkItemType).Returns("Craft");
		craft.SetupGet(x => x.RevisionNumber).Returns(1);
		craft.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		craft.Setup(x => x.AppearInCraftsList(manager.Object)).Returns(true);
		craft.Setup(x => x.CanDoCraft(manager.Object, It.IsAny<IActiveCraftGameItemComponent>(), true, false))
		     .Returns((true, string.Empty));
		craft.Setup(x => x.CreateResourceReservation(
				manager.Object,
				It.IsAny<IActiveCraftGameItemComponent>(),
				It.IsAny<int>(),
				It.IsAny<int>()))
		     .Returns((true, string.Empty, new CraftResourceReservation(
			     craft.Object.Id,
			     craft.Object.RevisionNumber,
			     craft.Object.Name,
			     1,
			     int.MaxValue,
			     [
				     new CraftInputReservation(
					     501,
					     "ore",
					     "simple",
					     reservedInput.Object.Id,
					     "GameItem",
					     reservedInput.Object.Name,
					     1,
					     [reservedInput.Object.Id])
			     ],
			     [])));
		var crafts = new RevisableAll<ICraft>();
		crafts.Add(craft.Object);
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>
		{
			[manager.Object.Id] = manager.Object
		}, [cell], new Dictionary<long, IGameItem>
		{
			[reservedInput.Object.Id] = reservedInput.Object
		});
		gameworld.SetupGet(x => x.Crafts).Returns(crafts);
		gameworld.Setup(x => x.GetStaticDouble("EmploymentCraftReservationDurationMinutes")).Returns(7.0);
		manager.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		manager.SetupGet(x => x.Location).Returns(cell);
		manager.SetupGet(x => x.State).Returns(CharacterState.Awake);
		manager.SetupGet(x => x.Effects).Returns([]);
		manager.Setup(x => x.EffectsOfType<IActiveCraftEffect>(It.IsAny<Predicate<IActiveCraftEffect>>()))
		       .Returns(Array.Empty<IActiveCraftEffect>());
		var existingReservation = new EmploymentCraftReservationEffect(
			reservedInput.Object,
			Guid.NewGuid(),
			Guid.NewGuid(),
			"other craft task",
			"craft input ore",
			DateTimeOffset.UtcNow.AddMinutes(5));
		SetupCraftReservationEffects(reservedInput, [existingReservation]);
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		host.Hire(manager.Object, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageCraftRules), null);
		var task = host.TaskBoard.CreateActiveTask("start craft",
			new EmploymentActionPlan([new CraftTriggerActionStep("reserve craft")]),
			manager.Object);
		var context = new EmploymentTaskContext(host);
		context.HydrateTaskState(task, 0);

		Assert.IsFalse(context.TryStartCraft(manager.Object, "reserve craft", out var reason, out _));

		StringAssert.Contains(reason, "already reserved");
		craft.Verify(x => x.BeginCraft(manager.Object), Times.Never);
		reservedInput.Verify(x => x.AddEffect(It.IsAny<MudSharp.Effects.IEffect>(), It.IsAny<TimeSpan>()),
			Times.Never);
	}

	[TestMethod]
	public void EmploymentCraftReservations_FailedCraftStartReleasesFreshReservationLocks()
	{
		var currency = Currency();
		var manager = Character(1, "Manager");
		var cell = Cell(10, "workshop").Object;
		var reservedInput = ReservationTrackedItem(401, "reserved ore", out var inputPredicates);
		var addedEffects = new List<EmploymentCraftReservationEffect>();
		var craft = new Mock<ICraft>();
		craft.SetupGet(x => x.Id).Returns(500);
		craft.SetupGet(x => x.Name).Returns("reserve craft");
		craft.SetupGet(x => x.FrameworkItemType).Returns("Craft");
		craft.SetupGet(x => x.RevisionNumber).Returns(1);
		craft.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		craft.Setup(x => x.AppearInCraftsList(manager.Object)).Returns(true);
		craft.Setup(x => x.CanDoCraft(manager.Object, It.IsAny<IActiveCraftGameItemComponent>(), true, false))
		     .Returns((true, string.Empty));
		craft.Setup(x => x.CreateResourceReservation(
				manager.Object,
				It.IsAny<IActiveCraftGameItemComponent>(),
				It.IsAny<int>(),
				It.IsAny<int>()))
		     .Returns((true, string.Empty, new CraftResourceReservation(
			     craft.Object.Id,
			     craft.Object.RevisionNumber,
			     craft.Object.Name,
			     1,
			     int.MaxValue,
			     [
				     new CraftInputReservation(
					     501,
					     "ore",
					     "simple",
					     reservedInput.Object.Id,
					     "GameItem",
					     reservedInput.Object.Name,
					     1,
					     [reservedInput.Object.Id])
			     ],
			     [])));
		var crafts = new RevisableAll<ICraft>();
		crafts.Add(craft.Object);
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>
		{
			[manager.Object.Id] = manager.Object
		}, [cell], new Dictionary<long, IGameItem>
		{
			[reservedInput.Object.Id] = reservedInput.Object
		});
		gameworld.SetupGet(x => x.Crafts).Returns(crafts);
		gameworld.Setup(x => x.GetStaticDouble("EmploymentCraftReservationDurationMinutes")).Returns(7.0);
		manager.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		manager.SetupGet(x => x.Location).Returns(cell);
		manager.SetupGet(x => x.State).Returns(CharacterState.Awake);
		manager.SetupGet(x => x.Effects).Returns([]);
		manager.Setup(x => x.EffectsOfType<IActiveCraftEffect>(It.IsAny<Predicate<IActiveCraftEffect>>()))
		       .Returns(Array.Empty<IActiveCraftEffect>());
		SetupCraftReservationEffects(reservedInput, []);
		reservedInput.Setup(x => x.AddEffect(It.IsAny<MudSharp.Effects.IEffect>(), It.IsAny<TimeSpan>()))
		             .Callback<MudSharp.Effects.IEffect, TimeSpan>((effect, _) =>
			             addedEffects.Add((EmploymentCraftReservationEffect)effect));
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		host.Hire(manager.Object, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageCraftRules), null);
		var task = host.TaskBoard.CreateActiveTask("start craft",
			new EmploymentActionPlan([new CraftTriggerActionStep("reserve craft")]),
			manager.Object);
		var context = new EmploymentTaskContext(host);
		context.HydrateTaskState(task, 0);

		Assert.IsFalse(context.TryStartCraft(manager.Object, "reserve craft", out var reason, out _));

		StringAssert.Contains(reason, "did not create a native active craft effect");
		Assert.AreEqual(1, addedEffects.Count);
		Assert.AreEqual(task.CorrelationId, addedEffects.Single().CorrelationId);
		AssertReservationCleanup(reservedInput, inputPredicates, task);
	}

	[TestMethod]
	public void EmploymentCraftProductionChains_RecordPriorOutputsAsNextCraftInputs()
	{
		var currency = Currency();
		var crafter = Character(1, "Crafter");
		var cell = Cell(10, "workshop").Object;
		var intermediate = Item(701, "intermediate blank");
		var firstActiveItem = Item(702, "first active craft");
		var secondActiveItem = Item(703, "second active craft");
		var activeEffects = new List<IActiveCraftEffect>();

		Mock<ICraft> CreateChainCraft(long id, string name, IGameItem activeItem)
		{
			var craft = new Mock<ICraft>();
			craft.SetupGet(x => x.Id).Returns(id);
			craft.SetupGet(x => x.Name).Returns(name);
			craft.SetupGet(x => x.FrameworkItemType).Returns("Craft");
			craft.SetupGet(x => x.RevisionNumber).Returns(1);
			craft.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
			craft.Setup(x => x.AppearInCraftsList(crafter.Object)).Returns(true);
			craft.Setup(x => x.CanDoCraft(crafter.Object, It.IsAny<IActiveCraftGameItemComponent>(), true, false))
			     .Returns((true, string.Empty));
			craft.Setup(x => x.CreateResourceReservation(
					crafter.Object,
					It.IsAny<IActiveCraftGameItemComponent>(),
					It.IsAny<int>(),
					It.IsAny<int>()))
			     .Returns((true, string.Empty, new CraftResourceReservation(
				     id,
				     1,
				     name,
				     1,
				     int.MaxValue,
				     [],
				     [])));
			var component = new Mock<IActiveCraftGameItemComponent>();
			component.SetupGet(x => x.Craft).Returns(craft.Object);
			component.SetupGet(x => x.Parent).Returns(activeItem);
			component.SetupGet(x => x.Phase).Returns(1);
			component.SetupGet(x => x.HasFinished).Returns(false);
			var effect = new Mock<IActiveCraftEffect>();
			effect.SetupGet(x => x.Component).Returns(component.Object);
			craft.Setup(x => x.BeginCraft(crafter.Object))
			     .Callback(() => activeEffects.Add(effect.Object));
			return craft;
		}

		var firstCraft = CreateChainCraft(601, "make blank", firstActiveItem.Object);
		var secondCraft = CreateChainCraft(602, "finish blank", secondActiveItem.Object);
		var crafts = new RevisableAll<ICraft>();
		crafts.Add(firstCraft.Object);
		crafts.Add(secondCraft.Object);
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>
		{
			[crafter.Object.Id] = crafter.Object
		}, [cell], new Dictionary<long, IGameItem>
		{
			[intermediate.Object.Id] = intermediate.Object
		});
		gameworld.SetupGet(x => x.Crafts).Returns(crafts);
		gameworld.Setup(x => x.GetStaticDouble("EmploymentCraftReservationDurationMinutes")).Returns(7.0);
		crafter.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		crafter.SetupGet(x => x.Location).Returns(cell);
		crafter.SetupGet(x => x.State).Returns(CharacterState.Awake);
		crafter.SetupGet(x => x.Effects).Returns([]);
		crafter.Setup(x => x.EffectsOfType<IActiveCraftEffect>(It.IsAny<Predicate<IActiveCraftEffect>>()))
		       .Returns((Predicate<IActiveCraftEffect> predicate) => activeEffects
		           .Where(x => predicate?.Invoke(x) ?? true)
		           .ToList());
		IEmploymentHost host = new TestEmploymentHost("workshop", currency.Object);
		host.Hire(crafter.Object, Offer(currency.Object, EmploymentRole.Employee,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageCraftRules), null);
		var task = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("chain crafts",
			new EmploymentActionPlan([
				new CraftTriggerActionStep("make blank"),
				new CraftTriggerActionStep("finish blank")
			]),
			crafter.Object);
		var context = new EmploymentTaskContext(host);
		context.HydrateTaskState(task, 0);
		Assert.IsTrue(context.TryStartCraft(crafter.Object, "make blank", out var reason, out var firstStart), reason);
		task.MarkStep(0, EmploymentActionStepStatus.InProgress, firstStart);
		activeEffects.Clear();
		context.SetAvailableItems(cell, [intermediate.Object]);
		context.HydrateTaskState(task, 0);

		Assert.IsTrue(context.TryStartCraft(crafter.Object, "make blank", out reason, out var firstComplete), reason);
		task.MarkStep(0, EmploymentActionStepStatus.Completed, firstComplete);
		context.HydrateTaskState(task, 1);

		CollectionAssert.AreEquivalent(new[] { intermediate.Object.Id }, context.CarriedTaskItems(crafter.Object).Select(x => x.Id).ToArray());
		Assert.IsTrue(context.TryStartCraft(crafter.Object, "finish blank", out reason, out var secondStart), reason);
		StringAssert.Contains(secondStart.CraftJobReference, "TaskInputItemIds");
		StringAssert.Contains(secondStart.CraftJobReference, intermediate.Object.Id.ToString("F0"));
	}

	[TestMethod]
	public void EmploymentCraftRecovery_FailedActiveCraftAdoptsSalvageAndFailsTask()
	{
		var currency = Currency();
		var crafter = Character(1, "Crafter");
		var cell = Cell(10, "workshop").Object;
		var salvage = Item(801, "warped blank");
		var activeItem = Item(802, "active craft");
		var activeEffects = new List<IActiveCraftEffect>();
		var failed = false;
		var craft = new Mock<ICraft>();
		craft.SetupGet(x => x.Id).Returns(701);
		craft.SetupGet(x => x.Name).Returns("risky craft");
		craft.SetupGet(x => x.FrameworkItemType).Returns("Craft");
		craft.SetupGet(x => x.RevisionNumber).Returns(1);
		craft.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		craft.Setup(x => x.AppearInCraftsList(crafter.Object)).Returns(true);
		craft.Setup(x => x.CanDoCraft(crafter.Object, It.IsAny<IActiveCraftGameItemComponent>(), true, false))
		     .Returns((true, string.Empty));
		craft.Setup(x => x.CreateResourceReservation(
				crafter.Object,
				It.IsAny<IActiveCraftGameItemComponent>(),
				It.IsAny<int>(),
				It.IsAny<int>()))
		     .Returns((true, string.Empty, new CraftResourceReservation(
			     craft.Object.Id,
			     craft.Object.RevisionNumber,
			     craft.Object.Name,
			     1,
			     int.MaxValue,
			     [],
			     [])));
		var component = new Mock<IActiveCraftGameItemComponent>();
		component.SetupGet(x => x.Craft).Returns(craft.Object);
		component.SetupGet(x => x.Parent).Returns(activeItem.Object);
		component.SetupGet(x => x.Phase).Returns(1);
		component.SetupGet(x => x.HasFinished).Returns(false);
		component.SetupGet(x => x.HasFailed).Returns(() => failed);
		var effect = new Mock<IActiveCraftEffect>();
		effect.SetupGet(x => x.Component).Returns(component.Object);
		craft.Setup(x => x.BeginCraft(crafter.Object))
		     .Callback(() => activeEffects.Add(effect.Object));
		var crafts = new RevisableAll<ICraft>();
		crafts.Add(craft.Object);
		var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>
		{
			[crafter.Object.Id] = crafter.Object
		}, [cell], new Dictionary<long, IGameItem>
		{
			[salvage.Object.Id] = salvage.Object
		});
		gameworld.SetupGet(x => x.Crafts).Returns(crafts);
		gameworld.Setup(x => x.GetStaticDouble("EmploymentCraftReservationDurationMinutes")).Returns(7.0);
		crafter.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		crafter.SetupGet(x => x.Location).Returns(cell);
		crafter.SetupGet(x => x.State).Returns(CharacterState.Awake);
		crafter.SetupGet(x => x.Effects).Returns([]);
		crafter.Setup(x => x.EffectsOfType<IActiveCraftEffect>(It.IsAny<Predicate<IActiveCraftEffect>>()))
		       .Returns((Predicate<IActiveCraftEffect> predicate) => activeEffects
		           .Where(x => predicate?.Invoke(x) ?? true)
		           .ToList());
		IEmploymentHost host = new TestEmploymentHost("workshop", currency.Object);
		host.Hire(crafter.Object, Offer(currency.Object, EmploymentRole.Employee,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageCraftRules), null);
		var task = host.TaskBoard.CreateActiveTask("recover craft",
			new EmploymentActionPlan([new CraftTriggerActionStep("risky craft")]),
			crafter.Object);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(host);
		var profile = Profile(crafter.Object, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanCraft));
		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		failed = true;
		context.SetAvailableItems(cell, [salvage.Object]);

		var recovered = dispatcher.AdvanceTask(task, context);

		Assert.IsFalse(recovered.Success);
		Assert.IsTrue(recovered.Completed);
		Assert.AreEqual(EmploymentTaskStatus.Failed, task.Status);
		CollectionAssert.AreEquivalent(new[] { salvage.Object.Id }, context.CarriedTaskItems(crafter.Object).Select(x => x.Id).ToArray());
		StringAssert.Contains(task.StepOperationalStates[0].OperationalPayload, "craft-status=failed");
		StringAssert.Contains(task.StepOperationalStates[0].FailureDiagnostic, "salvage");
	}

	[TestMethod]
	public void EmploymentPersistence_AllowsHostLocalRuntimeIdsAcrossDifferentHosts()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var employee1 = Character(201, "Employee One").Object;
			var employee2 = Character(202, "Employee Two").Object;
			var characters = new Dictionary<long, ICharacter>
			{
				[employee1.Id] = employee1,
				[employee2.Id] = employee2
			};
			var gameworld = Gameworld(currency.Object, characters);
			IEmploymentHost first = new PersistedEmploymentHost(301, "First Shop", gameworld.Object, currency.Object);
			IEmploymentHost second = new PersistedEmploymentHost(302, "Second Shop", gameworld.Object, currency.Object);

			first.Hire(employee1, Offer(currency.Object, EmploymentRole.Employee), null);
			second.Hire(employee2, Offer(currency.Object, EmploymentRole.Employee), null);
			first.Employment.CreateJobOpening(Opening(currency.Object), null);
			second.Employment.CreateJobOpening(Opening(currency.Object), null);

			Assert.AreEqual(2, context.EmploymentContracts.Count());
			Assert.AreEqual(2, context.EmploymentJobOpenings.Count());
			CollectionAssert.AreEquivalent(new[] { 1L, 1L },
				context.EmploymentContracts.Select(x => x.RuntimeId).ToArray());
			Assert.AreEqual(2, context.EmploymentContracts.Select(x => x.Id).Distinct().Count());
			Assert.AreEqual(2, context.EmploymentJobOpenings.Select(x => x.Id).Distinct().Count());
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void HotelPersistence_LoadIfExistsDoesNotCreateMissingRoot()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var property = new Mock<IProperty>();
			property.SetupGet(x => x.Id).Returns(700);

			var hotel = HotelPersistenceStore.LoadIfExists(property.Object);

			Assert.IsNull(hotel);
			Assert.AreEqual(0, context.Hotels.Count());
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void HotelPersistence_LazilyCreatesRootPersistsNormalisedHotelStateAndDelegatesToProperty()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var zone = new Mock<IEconomicZone>();
			zone.SetupGet(x => x.Currency).Returns(currency.Object);
			var cell = new Mock<ICell>();
			cell.SetupGet(x => x.Id).Returns(987);
			var key = new Mock<IPropertyKey>();
			key.SetupGet(x => x.Id).Returns(44);
			var furnishing = new Mock<IHotelFurnishing>();
			furnishing.SetupGet(x => x.GameItemId).Returns(55);
			furnishing.SetupGet(x => x.Description).Returns("a brass bed");
			furnishing.SetupProperty(x => x.ReplacementValue, 12.5M);
			furnishing.SetupGet(x => x.OriginalCondition).Returns(0.9);
			furnishing.SetupGet(x => x.OriginalDamageCondition).Returns(0.8);
			var room = new Mock<IHotelRoom>();
			room.SetupGet(x => x.Cell).Returns(cell.Object);
			room.SetupGet(x => x.Name).Returns("Blue Room");
			room.SetupGet(x => x.Listed).Returns(true);
			room.SetupGet(x => x.PricePerDay).Returns(6.0M);
			room.SetupGet(x => x.SecurityDeposit).Returns(2.0M);
			room.SetupGet(x => x.MinimumDuration).Returns(TimeSpan.FromDays(1));
			room.SetupGet(x => x.MaximumDuration).Returns(TimeSpan.FromDays(7));
			room.SetupGet(x => x.Keys).Returns([key.Object]);
			room.SetupGet(x => x.Furnishings).Returns([furnishing.Object]);
			room.SetupGet(x => x.ActiveRental).Returns((IHotelRoomRental)null!);
			var balance = new Mock<IHotelPatronBalance>();
			balance.SetupGet(x => x.PatronId).Returns(77);
			balance.SetupProperty(x => x.Balance, 3.5M);
			var lost = new Mock<IHotelLostProperty>();
			lost.SetupGet(x => x.Room).Returns(room.Object);
			lost.SetupGet(x => x.OwnerId).Returns(88);
			lost.SetupGet(x => x.BundleId).Returns(99);
			lost.SetupProperty(x => x.StoredUntil, MudDateTime.Never);
			lost.SetupProperty(x => x.Status, HotelLostPropertyStatus.Held);
			lost.SetupProperty(x => x.AuctionHouseId, (long?)null);
			lost.SetupProperty(x => x.ReservePrice, 9.5M);
			lost.SetupGet(x => x.Description).Returns("a forgotten satchel");
			var property = new Mock<IProperty>();
			property.SetupGet(x => x.Id).Returns(500);
			property.SetupGet(x => x.Name).Returns("Harbour House");
			property.SetupGet(x => x.EconomicZone).Returns(zone.Object);
			property.SetupGet(x => x.PropertyLocations).Returns([cell.Object]);
			property.SetupGet(x => x.HotelRooms).Returns([room.Object]);
			property.SetupGet(x => x.HotelLostProperties).Returns([lost.Object]);
			property.SetupGet(x => x.HotelPatronBalances).Returns([balance.Object]);
			property.SetupGet(x => x.HotelBannedPatronIds).Returns([66]);
			property.SetupGet(x => x.HotelLicenseStatus).Returns(HotelLicenseStatus.Approved);
			property.SetupGet(x => x.HotelLostPropertyRetention).Returns(MudTimeSpan.FromDays(14));
			property.SetupGet(x => x.HotelOutstandingTaxes).Returns(4.5M);
			property.SetupGet(x => x.HotelCashBalance).Returns(22.0M);
			property.SetupGet(x => x.HotelAvailableFunds).Returns(33.0M);

			var hotel = HotelPersistenceStore.LoadOrCreate(property.Object);

			var row = context.Hotels.Single();
			Assert.AreEqual(property.Object.Id, row.PropertyId);
			Assert.AreEqual(hotel.Id, row.Id);
			Assert.AreEqual((int)HotelLicenseStatus.Approved, row.LicenseStatus);
			Assert.AreEqual(4.5M, row.OutstandingTaxes);
			var roomRow = context.HotelRooms.Single();
			Assert.AreEqual(row.Id, roomRow.HotelId);
			Assert.AreEqual(cell.Object.Id, roomRow.CellId);
			Assert.AreEqual("Blue Room", roomRow.Name);
			Assert.AreEqual(1, context.HotelRoomKeys.Count());
			Assert.AreEqual(1, context.HotelRoomFurnishings.Count());
			Assert.AreEqual(1, context.HotelLostProperties.Count());
			Assert.AreEqual(1, context.HotelPatronBalances.Count());
			Assert.AreEqual(1, context.HotelBannedPatrons.Count());
			var roomId = roomRow.Id;
			var lastUpdatedAt = row.LastUpdatedAt;
			Assert.AreSame(property.Object, hotel.Property);
			Assert.AreEqual(22.0M, hotel.CashBalance);
			Assert.AreEqual(33.0M, hotel.AvailableFunds);
			Assert.IsTrue(hotel.CanAccessHotelLocation(cell.Object));

			property.SetupGet(x => x.HotelRooms).Returns([]);
			property.SetupGet(x => x.HotelLostProperties).Returns([]);
			property.SetupGet(x => x.HotelPatronBalances).Returns([]);
			property.SetupGet(x => x.HotelBannedPatronIds).Returns([]);
			var readOnlyHotel = HotelPersistenceStore.LoadOrCreate(property.Object);
			Assert.AreEqual(row.Id, readOnlyHotel.Id);
			Assert.AreEqual(lastUpdatedAt, context.Hotels.Single().LastUpdatedAt);
			Assert.AreEqual(1, context.HotelRooms.Count());
			Assert.AreEqual(roomId, context.HotelRooms.Single().Id);
			Assert.AreEqual(1, context.HotelLostProperties.Count());
			Assert.AreEqual(1, context.HotelPatronBalances.Count());
			Assert.AreEqual(1, context.HotelBannedPatrons.Count());

			property.SetupGet(x => x.HotelRooms).Returns([room.Object]);
			property.SetupGet(x => x.HotelLostProperties).Returns([lost.Object]);
			property.SetupGet(x => x.HotelPatronBalances).Returns([balance.Object]);
			property.SetupGet(x => x.HotelBannedPatronIds).Returns([66]);
			balance.Object.Balance = 8.25M;
			HotelPersistenceStore.Save(property.Object);
			context.SaveChanges();

			Assert.AreEqual(1, context.HotelPatronBalances.Count());
			Assert.AreEqual(8.25M, context.HotelPatronBalances.Single().Balance);
			Assert.AreEqual(1, context.HotelBannedPatrons.Count());
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	private static (Mock<IHospital> Hospital, IEmploymentHostState State) HospitalEmploymentHost(long id, string name,
		ICurrency currency, IEnumerable<ICell> supplyRooms, IEnumerable<ICell> operatingTheatres, decimal availableFunds)
	{
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.Id).Returns(id);
		hospital.SetupGet(x => x.Name).Returns(name);
		hospital.SetupGet(x => x.FrameworkItemType).Returns("Hospital");
		hospital.SetupGet(x => x.EmploymentHostName).Returns(name);
		hospital.SetupGet(x => x.EmploymentHostType).Returns(EmploymentHostType.Hospital);
		hospital.SetupGet(x => x.Market).Returns((IMarket?)null);
		hospital.SetupGet(x => x.Currency).Returns(currency);
		hospital.SetupGet(x => x.BankAccount).Returns((IBankAccount?)null);
		hospital.SetupGet(x => x.CashBalance).Returns(availableFunds);
		hospital.SetupGet(x => x.AvailableFunds).Returns(availableFunds);
		hospital.SetupGet(x => x.IsTrading).Returns(true);
		hospital.SetupGet(x => x.SupplyRooms).Returns(supplyRooms.ToList());
		hospital.SetupGet(x => x.OperatingTheatres).Returns(operatingTheatres.ToList());
		hospital.SetupGet(x => x.WaitingRooms).Returns([]);
		hospital.SetupGet(x => x.RecoveryRooms).Returns([]);
		hospital.SetupGet(x => x.StaffRooms).Returns([]);
		hospital.SetupGet(x => x.Locations).Returns(supplyRooms.Concat(operatingTheatres).ToList());
		hospital.SetupGet(x => x.Services).Returns([]);
		hospital.SetupGet(x => x.ActiveServices).Returns([]);
		hospital.SetupGet(x => x.ServiceRequests).Returns([]);
		hospital.SetupGet(x => x.ActiveServiceRequests).Returns([]);
		var state = new EmploymentHostState(hospital.Object);
		hospital.SetupGet(x => x.Employment).Returns(state);
		hospital.SetupGet(x => x.BusinessLedger).Returns(state.BusinessLedger);
		hospital.SetupGet(x => x.EmploymentRegister).Returns(state.EmploymentRegister);
		hospital.SetupGet(x => x.TaskBoard).Returns(state.TaskBoard);
		hospital.SetupGet(x => x.ManagerGoalBoard).Returns(state.ManagerGoalBoard);
		hospital.SetupGet(x => x.Payroll).Returns(state.Payroll);
		hospital.SetupGet(x => x.EmploymentContracts).Returns(() => state.EmploymentContracts);
		hospital.SetupGet(x => x.JobOpenings).Returns(() => state.JobOpenings);
		hospital.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		        .Returns((ICharacter actor, EmploymentAuthority authority) => state.HasAuthority(actor, authority));
		hospital.Setup(x => x.IsEmployee(It.IsAny<ICharacter>()))
		        .Returns((ICharacter actor) => state.EmploymentContracts.Any(x =>
			        x.Status == EmploymentStatus.Active && x.Employee.Id == actor.Id));
		return (hospital, state);
	}

	private static HospitalSupplyTestFixture HospitalSupplyFixture(bool includeOrderly, bool orderlyBusy = false)
	{
		var currency = Currency();
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.Id).Returns(77);
		hospital.SetupGet(x => x.Name).Returns("central clinic");
		hospital.SetupGet(x => x.FrameworkItemType).Returns("Hospital");
		hospital.SetupGet(x => x.EmploymentHostType).Returns(EmploymentHostType.Hospital);
		hospital.SetupGet(x => x.Market).Returns((IMarket?)null);
		hospital.SetupGet(x => x.Currency).Returns(currency.Object);
		hospital.SetupGet(x => x.IsTrading).Returns(true);

		var state = new EmploymentHostState(hospital.Object);
		var activeTasks = new List<IEmploymentActiveTask>();
		var taskBoard = new Mock<IEmploymentTaskBoard>();
		taskBoard.SetupGet(x => x.ActiveTasks).Returns(activeTasks);
		hospital.SetupGet(x => x.Employment).Returns(state);
		hospital.SetupGet(x => x.TaskBoard).Returns(taskBoard.Object);
		hospital.SetupGet(x => x.EmploymentContracts).Returns(() => state.EmploymentContracts);
		hospital.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		        .Returns((ICharacter actor, EmploymentAuthority authority) => state.HasAuthority(actor, authority));
		hospital.Setup(x => x.IsEmployee(It.IsAny<ICharacter>()))
		        .Returns((ICharacter actor) => state.EmploymentContracts.Any(x =>
			        x.Status == EmploymentStatus.Active && x.Employee.Id == actor.Id));

		var source = Cell(701, "supply room");
		var theatre = Cell(702, "operating theatre");
		source.SetupGet(x => x.Characters).Returns(Array.Empty<ICharacter>());
		theatre.SetupGet(x => x.Characters).Returns(Array.Empty<ICharacter>());
		hospital.SetupGet(x => x.SupplyRooms).Returns([source.Object]);
		hospital.SetupGet(x => x.OperatingTheatres).Returns([theatre.Object]);
		hospital.SetupGet(x => x.WaitingRooms).Returns(Array.Empty<ICell>());
		hospital.SetupGet(x => x.RecoveryRooms).Returns(Array.Empty<ICell>());

		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.Id).Returns(88);
		service.SetupGet(x => x.Name).Returns("implant preparation");
		service.SetupGet(x => x.PreferOperatingTheatre).Returns(true);
		service.SetupGet(x => x.RequiredEquipment).Returns([
			new HospitalServiceEquipmentRequirement(1, EmploymentItemSelector.ForPrototype(500))
		]);

		var request = new Mock<IHospitalServiceRequest>();
		request.SetupGet(x => x.Id).Returns(900);
		request.SetupGet(x => x.Hospital).Returns(hospital.Object);
		request.SetupGet(x => x.Service).Returns(service.Object);
		request.SetupGet(x => x.Status).Returns(HospitalServiceRequestStatus.Queued);
		request.SetupGet(x => x.Patient).Returns((ICharacter?)null);
		request.SetupProperty(x => x.OperatingTheatreCellId);
		request.SetupProperty(x => x.UsedInPlaceFallback);
		request.SetupProperty(x => x.SupplyPrepared, false);
		hospital.SetupGet(x => x.ActiveServiceRequests).Returns([request.Object]);

		var doctorMock = Character(1, "Doctor");
		doctorMock.SetupGet(x => x.State).Returns(CharacterState.Awake);
		doctorMock.SetupGet(x => x.Location).Returns(source.Object);
		var doctor = doctorMock.Object;
		state.Hire(doctor, Offer(currency.Object, EmploymentRole.Employee,
			EmploymentAuthority.PerformMedicalServices | EmploymentAuthority.PrepareMedicalSupplies), null);
		if (includeOrderly)
		{
			var orderlyMock = Character(2, "Orderly");
			orderlyMock.SetupGet(x => x.State).Returns(CharacterState.Awake);
			orderlyMock.SetupGet(x => x.Location).Returns(source.Object);
			var orderly = orderlyMock.Object;
			state.Hire(orderly, Offer(currency.Object, EmploymentRole.HospitalOrderly,
				EmploymentAuthority.PrepareMedicalSupplies), null);

			if (orderlyBusy)
			{
				var busyTask = new EmploymentActiveTask(hospital.Object, "already preparing supplies",
					new EmploymentActionPlan([new HospitalSupplyPreparationActionStep(hospital.Object, request.Object)]),
					Guid.NewGuid());
				busyTask.Assign(orderly);
				activeTasks.Add(busyTask);
			}
		}

		var context = new EmploymentTaskContext(hospital.Object);
		context.SetAvailableItems(source.Object, [Item(800, "surgical tray", prototypeId: 500).Object]);
		return new HospitalSupplyTestFixture(hospital, request, context, doctor);
	}

	private static HospitalSupplyTestFixture HospitalImplicitTreatmentSupplyFixture(bool includeOrderly)
	{
		var currency = Currency();
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.Id).Returns(177);
		hospital.SetupGet(x => x.Name).Returns("central clinic");
		hospital.SetupGet(x => x.FrameworkItemType).Returns("Hospital");
		hospital.SetupGet(x => x.EmploymentHostType).Returns(EmploymentHostType.Hospital);
		hospital.SetupGet(x => x.Market).Returns((IMarket?)null);
		hospital.SetupGet(x => x.Currency).Returns(currency.Object);
		hospital.SetupGet(x => x.IsTrading).Returns(true);

		var state = new EmploymentHostState(hospital.Object);
		var taskBoard = new Mock<IEmploymentTaskBoard>();
		taskBoard.SetupGet(x => x.ActiveTasks).Returns([]);
		hospital.SetupGet(x => x.Employment).Returns(state);
		hospital.SetupGet(x => x.TaskBoard).Returns(taskBoard.Object);
		hospital.SetupGet(x => x.EmploymentContracts).Returns(() => state.EmploymentContracts);
		hospital.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		        .Returns((ICharacter actor, EmploymentAuthority authority) => state.HasAuthority(actor, authority));
		hospital.Setup(x => x.IsEmployee(It.IsAny<ICharacter>()))
		        .Returns((ICharacter actor) => state.EmploymentContracts.Any(x =>
			        x.Status == EmploymentStatus.Active && x.Employee.Id == actor.Id));

		var source = Cell(801, "supply room");
		var theatre = Cell(802, "operating theatre");
		source.SetupGet(x => x.Characters).Returns(Array.Empty<ICharacter>());
		theatre.SetupGet(x => x.Characters).Returns(Array.Empty<ICharacter>());
		hospital.SetupGet(x => x.SupplyRooms).Returns([source.Object]);
		hospital.SetupGet(x => x.OperatingTheatres).Returns([theatre.Object]);
		hospital.SetupGet(x => x.WaitingRooms).Returns(Array.Empty<ICell>());
		hospital.SetupGet(x => x.RecoveryRooms).Returns(Array.Empty<ICell>());

		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.Id).Returns(188);
		service.SetupGet(x => x.Name).Returns("full treatment");
		service.SetupGet(x => x.ServiceType).Returns(HospitalServiceType.FullTreatment);
		service.SetupGet(x => x.PreferOperatingTheatre).Returns(true);
		service.SetupGet(x => x.RequiredEquipment).Returns([]);

		var request = new Mock<IHospitalServiceRequest>();
		request.SetupGet(x => x.Id).Returns(901);
		request.SetupGet(x => x.Hospital).Returns(hospital.Object);
		request.SetupGet(x => x.Service).Returns(service.Object);
		request.SetupGet(x => x.Status).Returns(HospitalServiceRequestStatus.Queued);
		request.SetupGet(x => x.Patient).Returns((ICharacter?)null);
		request.SetupProperty(x => x.OperatingTheatreCellId);
		request.SetupProperty(x => x.UsedInPlaceFallback);
		request.SetupProperty(x => x.SupplyPrepared, false);
		hospital.SetupGet(x => x.ActiveServiceRequests).Returns([request.Object]);

		var doctorMock = Character(101, "Doctor");
		doctorMock.SetupGet(x => x.State).Returns(CharacterState.Awake);
		doctorMock.SetupGet(x => x.Location).Returns(source.Object);
		var doctor = doctorMock.Object;
		state.Hire(doctor, Offer(currency.Object, EmploymentRole.Employee,
			EmploymentAuthority.PerformMedicalServices | EmploymentAuthority.PrepareMedicalSupplies), null);
		if (includeOrderly)
		{
			var orderlyMock = Character(102, "Orderly");
			orderlyMock.SetupGet(x => x.State).Returns(CharacterState.Awake);
			orderlyMock.SetupGet(x => x.Location).Returns(source.Object);
			state.Hire(orderlyMock.Object, Offer(currency.Object, EmploymentRole.HospitalOrderly,
				EmploymentAuthority.PrepareMedicalSupplies), null);
		}

		var treatment = new Mock<ITreatment>();
		treatment.Setup(x => x.IsTreatmentType(It.IsAny<TreatmentType>())).Returns(true);
		var treatmentItem = Item(803, "treatment kit");
		treatmentItem.Setup(x => x.GetItemType<ITreatment>()).Returns(treatment.Object);
		var context = new EmploymentTaskContext(hospital.Object);
		context.SetAvailableItems(source.Object, [treatmentItem.Object]);
		return new HospitalSupplyTestFixture(hospital, request, context, doctor);
	}

	private static void SetupAvailableMedicalEmployee(Mock<IHospital> hospital)
	{
		hospital.SetupGet(x => x.Id).Returns(7900);
		hospital.SetupGet(x => x.Name).Returns("central clinic");
		hospital.SetupGet(x => x.FrameworkItemType).Returns("Hospital");
		hospital.SetupGet(x => x.EmploymentHostType).Returns(EmploymentHostType.Hospital);
		hospital.SetupGet(x => x.Market).Returns((IMarket?)null);
		var state = new EmploymentHostState(hospital.Object);
		var taskBoard = new Mock<IEmploymentTaskBoard>();
		taskBoard.SetupGet(x => x.ActiveTasks).Returns(Array.Empty<IEmploymentActiveTask>());
		hospital.SetupGet(x => x.Employment).Returns(state);
		hospital.SetupGet(x => x.TaskBoard).Returns(taskBoard.Object);
		hospital.SetupGet(x => x.EmploymentContracts).Returns(() => state.EmploymentContracts);
		hospital.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		        .Returns((ICharacter actor, EmploymentAuthority authority) => state.HasAuthority(actor, authority));
		var doctorCell = Cell(7901, "staff room");
		var doctor = NpcCharacter(7902, "Doctor",
			[EmploymentWorkerAi(EmploymentAICapability.CanPerformMedicalServices)]);
		doctor.SetupGet(x => x.Location).Returns(doctorCell.Object);
		state.Hire(doctor.Object, Offer(Currency().Object, EmploymentRole.MedicalWorker,
			EmploymentAuthority.PerformMedicalServices), null);
	}

	private static EmploymentWorkerAI EmploymentWorkerAi(params EmploymentAICapability[] capabilities)
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Currencies).Returns(new All<ICurrency>());
		var capabilityXml = string.Join(string.Empty, capabilities.Select(x => $"<Capability>{x}</Capability>"));
		var model = new MudSharp.Models.ArtificialIntelligence
		{
			Id = 1,
			Name = "worker",
			Type = "EmploymentWorker",
			Definition = $"""
			              <Definition>
			                <ReservationWage>0</ReservationWage>
			                <TaskingEnabled>true</TaskingEnabled>
			                <HostTypeFilter>Hospital</HostTypeFilter>
			                <AcceptedPaymentMethods><Method>Cash</Method></AcceptedPaymentMethods>
			                <Capabilities>{capabilityXml}</Capabilities>
			              </Definition>
			              """
		};
		return (EmploymentWorkerAI)Activator.CreateInstance(
			typeof(EmploymentWorkerAI),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[] { model, gameworld.Object },
			CultureInfo.InvariantCulture)!;
	}

	private static bool ContainsType(Type type, Type target)
	{
		if (type == target)
		{
			return true;
		}

		if (type.IsGenericType && type.GetGenericArguments().Any(x => ContainsType(x, target)))
		{
			return true;
		}

		return type.HasElementType && type.GetElementType() is { } elementType && ContainsType(elementType, target);
	}

	private static Mock<ICurrency> Currency(long id = 1, string name = "test dollars")
	{
		var currency = new Mock<ICurrency>();
		currency.SetupGet(x => x.Id).Returns(id);
		currency.SetupGet(x => x.Name).Returns(name);
		currency.Setup(x => x.Describe(It.IsAny<decimal>(), It.IsAny<CurrencyDescriptionPatternType>()))
		        .Returns((decimal amount, CurrencyDescriptionPatternType _) => $"{amount:N2} {name}");
		return currency;
	}

	private static EmploymentTaskProvenance ScopedFinancialProvenance(ICharacter actor, EmploymentActionPlan actionPlan,
		Guid correlationId, ICurrency currency, decimal amount, IEnumerable<string> paymentSourceScopes,
		IEnumerable<string> counterpartyScopes)
	{
		var principal = EmploymentPrincipal.ForCharacter(actor);
		var now = DateTimeOffset.UtcNow;
		return new EmploymentTaskProvenance(
			EmploymentTaskSourceKind.Manual,
			null,
			null,
			principal,
			principal,
			new EmploymentAuthorisationGrant(
				Guid.NewGuid(),
				principal,
				actionPlan.RequiredAuthority,
				new Dictionary<long, decimal> { [currency.Id] = amount },
				false,
				now,
				null,
				correlationId,
				ScopeSet(paymentSourceScopes),
				ScopeSet(counterpartyScopes)),
			0,
			now);
	}

	private static HashSet<string> ScopeSet(IEnumerable<string> scopes)
	{
		return scopes
		       .Select(EmploymentAuthorisationGrant.NormaliseScope)
		       .ToHashSet(StringComparer.InvariantCultureIgnoreCase);
	}
	private static (Mock<IShop> Shop, IEmploymentHostState State) ShopHost(long id, string name, ICurrency currency,
		IBankAccount? bankAccount)
	{
		var shop = new Mock<IShop>();
		shop.SetupGet(x => x.Id).Returns(id);
		shop.SetupGet(x => x.Name).Returns(name);
		shop.SetupGet(x => x.FrameworkItemType).Returns("Shop");
		shop.SetupGet(x => x.EmploymentHostName).Returns(name);
		shop.SetupGet(x => x.EmploymentHostType).Returns(EmploymentHostType.Shop);
		shop.SetupGet(x => x.Market).Returns((IMarket?)null);
		shop.SetupGet(x => x.Currency).Returns(currency);
		shop.SetupGet(x => x.BankAccount).Returns(() => bankAccount!);
		shop.SetupGet(x => x.CashBalance).Returns(() => VirtualCashLedger.Balance(shop.Object, currency));
		var state = new EmploymentHostState(shop.Object);
		shop.SetupGet(x => x.Employment).Returns(state);
		shop.SetupGet(x => x.BusinessLedger).Returns(state.BusinessLedger);
		shop.SetupGet(x => x.EmploymentRegister).Returns(state.EmploymentRegister);
		shop.SetupGet(x => x.TaskBoard).Returns(state.TaskBoard);
		shop.SetupGet(x => x.ManagerGoalBoard).Returns(state.ManagerGoalBoard);
		shop.SetupGet(x => x.Payroll).Returns(state.Payroll);
		shop.SetupGet(x => x.EmploymentContracts).Returns(() => state.EmploymentContracts);
		shop.SetupGet(x => x.JobOpenings).Returns(() => state.JobOpenings);
		shop.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		    .Returns((ICharacter actor, EmploymentAuthority authority) => state.HasAuthority(actor, authority));
		return (shop, state);
	}

	private static (Mock<IClan> Clan, IEmploymentHostState State) ClanHost(long id, string name, ICurrency currency,
		IBankAccount? bankAccount)
	{
		var clan = new Mock<IClan>();
		clan.SetupGet(x => x.Id).Returns(id);
		clan.SetupGet(x => x.Name).Returns(name);
		clan.SetupGet(x => x.FullName).Returns(name);
		clan.SetupGet(x => x.Alias).Returns(name);
		clan.SetupGet(x => x.Names).Returns([name]);
		clan.SetupGet(x => x.IsTemplate).Returns(false);
		clan.SetupGet(x => x.FrameworkItemType).Returns("Clan");
		clan.SetupGet(x => x.EmploymentHostName).Returns(name);
		clan.SetupGet(x => x.EmploymentHostType).Returns(EmploymentHostType.Clan);
		clan.SetupGet(x => x.Market).Returns((IMarket?)null);
		clan.SetupGet(x => x.ClanBankAccount).Returns(() => bankAccount!);
		clan.SetupGet(x => x.ClanHallCells).Returns([]);
		var state = new EmploymentHostState(clan.Object);
		clan.SetupGet(x => x.Employment).Returns(state);
		clan.SetupGet(x => x.BusinessLedger).Returns(state.BusinessLedger);
		clan.SetupGet(x => x.EmploymentRegister).Returns(state.EmploymentRegister);
		clan.SetupGet(x => x.TaskBoard).Returns(state.TaskBoard);
		clan.SetupGet(x => x.ManagerGoalBoard).Returns(state.ManagerGoalBoard);
		clan.SetupGet(x => x.Payroll).Returns(state.Payroll);
		clan.SetupGet(x => x.EmploymentContracts).Returns(() => state.EmploymentContracts);
		clan.SetupGet(x => x.JobOpenings).Returns(() => state.JobOpenings);
		clan.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		    .Returns((ICharacter actor, EmploymentAuthority authority) => state.HasAuthority(actor, authority));
		return (clan, state);
	}

	private static (Mock<IBank> Bank, IEmploymentHostState State, List<string> Audit) BankHost(long id, string name,
		ICurrency currency, DecimalCounter<ICurrency>? reserves = null, IEnumerable<IBankAccount>? accounts = null,
		IEnumerable<ICell>? branches = null, IFuturemud? gameworld = null)
	{
		var bank = new Mock<IBank>();
		bank.SetupGet(x => x.Id).Returns(id);
		bank.SetupGet(x => x.Name).Returns(name);
		bank.SetupGet(x => x.FrameworkItemType).Returns("Bank");
		bank.SetupGet(x => x.EmploymentHostName).Returns(name);
		bank.SetupGet(x => x.EmploymentHostType).Returns(EmploymentHostType.Bank);
		bank.SetupGet(x => x.Market).Returns((IMarket?)null);
		bank.SetupGet(x => x.Code).Returns("TST");
		bank.SetupGet(x => x.PrimaryCurrency).Returns(currency);
		var reserveCounter = reserves ?? new DecimalCounter<ICurrency> { [currency] = 0.0M };
		bank.SetupGet(x => x.CurrencyReserves).Returns(reserveCounter);
		bank.SetupGet(x => x.BankAccounts).Returns(accounts ?? []);
		bank.SetupGet(x => x.BranchLocations).Returns(branches ?? []);
		bank.SetupProperty(x => x.Changed);
		if (gameworld is not null)
		{
			bank.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(gameworld);
		}

		var audit = new List<string>();
		bank.Setup(x => x.RecordManagerAuditLog(It.IsAny<ICharacter>(), It.IsAny<string>()))
		    .Callback<ICharacter, string>((_, detail) => audit.Add(detail));
		var state = new EmploymentHostState(bank.Object);
		bank.SetupGet(x => x.Employment).Returns(state);
		bank.SetupGet(x => x.BusinessLedger).Returns(state.BusinessLedger);
		bank.SetupGet(x => x.EmploymentRegister).Returns(state.EmploymentRegister);
		bank.SetupGet(x => x.TaskBoard).Returns(state.TaskBoard);
		bank.SetupGet(x => x.ManagerGoalBoard).Returns(state.ManagerGoalBoard);
		bank.SetupGet(x => x.Payroll).Returns(state.Payroll);
		bank.SetupGet(x => x.EmploymentContracts).Returns(() => state.EmploymentContracts);
		bank.SetupGet(x => x.JobOpenings).Returns(() => state.JobOpenings);
		bank.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		    .Returns((ICharacter actor, EmploymentAuthority authority) => state.HasAuthority(actor, authority));
		return (bank, state, audit);
	}

	private static Mock<IBankAccount> BankAccount(ICurrency currency, decimal balance, decimal reserves = 1_000.0M,
		long id = 901, int accountNumber = 123, string bankCode = "TST", string? accountName = null,
		BankAccountStatus status = BankAccountStatus.Active, long bankId = 900)
	{
		var currentBalance = balance;
		var accountStatus = status;
		var currencyReserves = new DecimalCounter<ICurrency>
		{
			[currency] = reserves
		};
		var bank = new Mock<IBank>();
		bank.SetupGet(x => x.Id).Returns(bankId);
		bank.SetupGet(x => x.Name).Returns("test bank");
		bank.SetupGet(x => x.FrameworkItemType).Returns("Bank");
		bank.SetupGet(x => x.Code).Returns(bankCode);
		bank.SetupGet(x => x.PrimaryCurrency).Returns(currency);
		bank.SetupGet(x => x.CurrencyReserves).Returns(currencyReserves);
		bank.SetupProperty(x => x.Changed);
		var account = new Mock<IBankAccount>();
		account.SetupGet(x => x.Id).Returns(id);
		account.SetupGet(x => x.Name).Returns(accountName ?? $"test account {id:N0}");
		account.SetupGet(x => x.FrameworkItemType).Returns("BankAccount");
		account.SetupGet(x => x.AccountNumber).Returns(accountNumber);
		account.SetupGet(x => x.AccountReference).Returns($"{bankCode.ToUpperInvariant()}:{accountNumber}");
		account.SetupGet(x => x.NameWithAlias).Returns($"{bankCode.ToUpperInvariant()}:{accountNumber} [{accountName ?? $"test account {id:N0}"}]");
		account.SetupGet(x => x.AccountStatus).Returns(() => accountStatus);
		account.SetupGet(x => x.Currency).Returns(currency);
		account.SetupGet(x => x.Bank).Returns(bank.Object);
		account.SetupGet(x => x.CurrentBalance).Returns(() => currentBalance);
		account.Setup(x => x.MaximumWithdrawal()).Returns(() => Math.Max(0.0M, currentBalance));
		account.Setup(x => x.CanWithdraw(It.IsAny<decimal>(), It.IsAny<bool>()))
		       .Returns((decimal amount, bool ignoreReserves) =>
		       {
			       if (accountStatus != BankAccountStatus.Active)
			       {
				       return (false, $"That account is currently {accountStatus.DescribeEnum()}.");
			       }

			       if (amount > currentBalance)
			       {
				       return (false, "Your account has insufficient funds for that withdrawal.");
			       }

			       if (!ignoreReserves && currencyReserves[currency] < amount)
			       {
				       return (false, "The bank has insufficient currency reserves to honour your withdrawal.");
			       }

			       return (true, string.Empty);
		       });
		account.Setup(x => x.WithdrawFromTransaction(It.IsAny<decimal>(), It.IsAny<string>()))
		       .Callback<decimal, string>((amount, _) => currentBalance -= amount);
		account.Setup(x => x.DepositFromTransaction(It.IsAny<decimal>(), It.IsAny<string>()))
		       .Callback<decimal, string>((amount, _) => currentBalance += amount);
		account.Setup(x => x.DoAccountCredit(It.IsAny<decimal>(), It.IsAny<string>()))
		       .Callback<decimal, string>((amount, _) => currentBalance += amount);
		account.Setup(x => x.SetStatus(It.IsAny<BankAccountStatus>()))
		       .Callback<BankAccountStatus>(newStatus => accountStatus = newStatus);
		account.Setup(x => x.WithdrawFromTransfer(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
		       .Callback<decimal, string, int, string>((amount, _, _, _) => currentBalance -= amount);
		account.Setup(x => x.DepositFromTransfer(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
		       .Callback<decimal, string, int, string>((amount, _, _, _) => currentBalance += amount);
		return account;
	}

	private static void AssertCombinedHospitalServiceDispatch(HospitalServiceType serviceType, string expectedFailure)
	{
		var hospital = new Mock<IHospital>();
		hospital.SetupGet(x => x.Id).Returns(9100);
		var service = new Mock<IHospitalService>();
		service.SetupGet(x => x.Id).Returns(9101);
		service.SetupGet(x => x.ServiceType).Returns(serviceType);
		service.SetupGet(x => x.RequiredEquipment).Returns(Array.Empty<HospitalServiceEquipmentRequirement>());

		var employee = Character(9102, "Doctor");
		var patient = Character(9103, "Patient");
		employee.Setup(x => x.ColocatedWith(patient.Object)).Returns(true);
		var body = new Mock<MudSharp.Body.IBody>();
		body.SetupGet(x => x.TotalBloodVolumeLitres).Returns(5.0);
		body.SetupGet(x => x.CurrentBloodVolumeLitres).Returns(5.0);
		patient.SetupGet(x => x.Body).Returns(body.Object);
		patient.SetupGet(x => x.Wounds).Returns(Array.Empty<MudSharp.Health.IWound>());
		patient.Setup(x => x.VisibleWounds(It.IsAny<IPerceiver>(), MudSharp.Health.WoundExaminationType.Examination))
		       .Returns(Array.Empty<MudSharp.Health.IWound>());

		var request = new Mock<IHospitalServiceRequest>();
		request.SetupGet(x => x.Id).Returns(9104);
		request.SetupGet(x => x.Hospital).Returns(hospital.Object);
		request.SetupGet(x => x.Service).Returns(service.Object);
		request.SetupGet(x => x.Status).Returns(HospitalServiceRequestStatus.Queued);
		request.SetupGet(x => x.PatientId).Returns(patient.Object.Id);
		request.SetupGet(x => x.Patient).Returns(patient.Object);
		var context = new Mock<IEmploymentTaskContext>();

		var result = HospitalMedicalServiceRunner.ExecuteServiceRequest(context.Object, employee.Object, hospital.Object, request.Object);

		Assert.IsFalse(result.Success);
		Assert.IsTrue(result.Completed);
		request.Verify(x => x.MarkStatus(HospitalServiceRequestStatus.Failed,
			It.Is<string>(message => message.Contains(expectedFailure) &&
			                         !message.Contains("Unsupported hospital service type"))), Times.Once);
	}

	private static Mock<ICharacter> Character(long id, string name, bool administrator = false, IFuturemud? gameworld = null)
	{
		var personalName = new Mock<IPersonalName>();
		personalName.Setup(x => x.GetName(It.IsAny<NameStyle>())).Returns(name);
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(id);
		character.SetupGet(x => x.Name).Returns(name);
		character.SetupGet(x => x.PersonalName).Returns(personalName.Object);
		character.SetupGet(x => x.CurrentName).Returns(personalName.Object);
		if (gameworld is not null)
		{
			character.SetupGet(x => x.Gameworld).Returns(gameworld);
		}
		character.Setup(x => x.HowSeen(
				It.IsAny<IPerceiver>(),
				It.IsAny<bool>(),
				It.IsAny<DescriptionType>(),
				It.IsAny<bool>(),
				It.IsAny<PerceiveIgnoreFlags>()))
		         .Returns(name);
		character.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(administrator);
		return character;
	}

	private static Mock<INPC> NpcCharacter(long id, string name,
		IEnumerable<IArtificialIntelligence>? artificialIntelligences = null,
		CharacterState state = CharacterState.Awake)
	{
		var personalName = new Mock<IPersonalName>();
		personalName.Setup(x => x.GetName(It.IsAny<NameStyle>())).Returns(name);
		var character = new Mock<INPC>();
		character.SetupGet(x => x.Id).Returns(id);
		character.SetupGet(x => x.Name).Returns(name);
		character.SetupGet(x => x.PersonalName).Returns(personalName.Object);
		character.SetupGet(x => x.CurrentName).Returns(personalName.Object);
		character.SetupGet(x => x.State).Returns(state);
		character.SetupGet(x => x.AIs).Returns(artificialIntelligences ?? []);
		character.Setup(x => x.HowSeen(
				It.IsAny<IPerceiver>(),
				It.IsAny<bool>(),
				It.IsAny<DescriptionType>(),
				It.IsAny<bool>(),
				It.IsAny<PerceiveIgnoreFlags>()))
		         .Returns(name);
		character.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(false);
		return character;
	}

	private static Mock<ICell> Cell(long id, string name)
	{
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(id);
		cell.SetupGet(x => x.Name).Returns(name);
		cell.SetupGet(x => x.GameItems).Returns(Array.Empty<IGameItem>());
		cell.Setup(x => x.GetFriendlyReference(It.IsAny<IPerceiver>())).Returns(name);
		return cell;
	}

	private static Mock<ICell> PhysicalCell(long id, string name, List<IGameItem> items)
	{
		var cell = Cell(id, name);
		cell.SetupGet(x => x.GameItems).Returns(() => items);
		cell.Setup(x => x.Insert(It.IsAny<IGameItem>(), It.IsAny<bool>()))
		    .Callback<IGameItem, bool>((item, _) => items.Add(item));
		cell.Setup(x => x.Extract(It.IsAny<IGameItem>()))
		    .Callback<IGameItem>(item => items.RemoveAll(x => x.Id == item.Id));
		cell.Setup(x => x.GetFriendlyReference(It.IsAny<IPerceiver>())).Returns(name);
		return cell;
	}

	private static Mock<IGameItem> Item(long id, string name, IEnumerable<ICell>? trueLocations = null,
		IEnumerable<ITag>? tags = null, long? prototypeId = null)
	{
		var proto = new Mock<IGameItemProto>();
		proto.SetupGet(x => x.Id).Returns(prototypeId ?? id);
		proto.SetupGet(x => x.ShortDescription).Returns(name);
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.Name).Returns(name);
		item.SetupGet(x => x.Prototype).Returns(proto.Object);
		item.SetupGet(x => x.Tags).Returns(tags ?? []);
		item.SetupGet(x => x.DeepItems).Returns(() => [item.Object]);
		item.SetupGet(x => x.TrueLocations).Returns(trueLocations ?? []);
		item.Setup(x => x.GetItemType<IContainer>()).Returns((IContainer)null!);
		item.Setup(x => x.HowSeen(
				It.IsAny<IPerceiver>(),
				It.IsAny<bool>(),
				It.IsAny<DescriptionType>(),
				It.IsAny<bool>(),
				It.IsAny<PerceiveIgnoreFlags>()))
		    .Returns(name);
		return item;
	}

	private static Mock<IGameItem> ReservationTrackedItem(long id, string name,
		out List<Predicate<EmploymentCraftReservationEffect>> cleanupPredicates)
	{
		cleanupPredicates = [];
		var capturedPredicates = cleanupPredicates;
		var item = Item(id, name);
		item.Setup(x => x.RemoveAllEffects(It.IsAny<Predicate<EmploymentCraftReservationEffect>>(), true))
		    .Callback<Predicate<EmploymentCraftReservationEffect>, bool>((predicate, _) =>
			    capturedPredicates.Add(predicate))
		    .Returns(true);
		return item;
	}

	private static void SetupCraftReservationEffects(Mock<IGameItem> item,
		IReadOnlyCollection<EmploymentCraftReservationEffect> effects)
	{
		item.Setup(x => x.EffectsOfType<EmploymentCraftReservationEffect>(
				It.IsAny<Predicate<EmploymentCraftReservationEffect>>()))
		    .Returns((Predicate<EmploymentCraftReservationEffect> predicate) =>
			    predicate is null
				    ? effects.ToList()
				    : effects.Where(x => predicate(x)).ToList());
	}

	private static void AssertReservationCleanup(Mock<IGameItem> item,
		IReadOnlyCollection<Predicate<EmploymentCraftReservationEffect>> cleanupPredicates,
		IEmploymentActiveTask task)
	{
		item.Verify(x => x.RemoveAllEffects(It.IsAny<Predicate<EmploymentCraftReservationEffect>>(), true),
			Times.Once);
		var predicate = cleanupPredicates.Single();
		Assert.IsTrue(predicate(new EmploymentCraftReservationEffect(item.Object, task.Id, task.CorrelationId,
			task.Name, "test", DateTimeOffset.UtcNow.AddMinutes(5))));
		Assert.IsFalse(predicate(new EmploymentCraftReservationEffect(item.Object, Guid.NewGuid(), Guid.NewGuid(),
			"other", "test", DateTimeOffset.UtcNow.AddMinutes(5))));
	}

	private static string CraftReservationStateJson(long inputItemId, long toolItemId, long stationItemId)
	{
		var reservedAt = DateTimeOffset.UtcNow;
		var expiresAt = reservedAt.AddMinutes(30);
		return JsonSerializer.Serialize(new
		{
			Version = "craft-v2",
			CraftId = 42L,
			Revision = 3,
			CraftName = "test craft",
			ActiveItemId = 9001L,
			PreExistingItemIds = Array.Empty<long>(),
			ReservedAt = reservedAt,
			ExpiresAt = expiresAt,
			Reservation = new
			{
				CraftId = 42L,
				RevisionNumber = 3,
				CraftName = "test craft",
				FromPhase = 1,
				ToPhase = 1,
				Inputs = new[]
				{
					new
					{
						InputId = 4201L,
						InputName = "ore",
						InputType = "simple",
						PerceivableId = inputItemId,
						PerceivableType = "GameItem",
						PerceivableDescription = "reserved ore",
						ConsumedPhase = 1,
						ItemIds = new[] { inputItemId }
					}
				},
				Tools = new[]
				{
					new
					{
						ToolId = 4202L,
						ToolName = "hammer",
						ToolType = "simple",
						ItemId = toolItemId,
						ItemDescription = "reserved hammer",
						Phase = 1
					}
				}
			},
			Station = new
			{
				Version = "craft-station-v1",
				Selector = "forge",
				CellId = 1L,
				ItemId = stationItemId,
				Description = "reserved forge",
				ReservedAt = reservedAt,
				ExpiresAt = expiresAt
			},
			OutputItemIds = Array.Empty<long>()
		});
	}

	private static Mock<IGameItem> ContainerItem(long id, string name, IEnumerable<ICell> trueLocations,
		IEnumerable<ITag> tags, List<IGameItem> contents)
	{
		var container = new Mock<IContainer>();
		container.SetupGet(x => x.Contents).Returns(() => contents);
		container.Setup(x => x.CanPut(It.IsAny<IGameItem>())).Returns(true);
		container.Setup(x => x.Put(It.IsAny<ICharacter?>(), It.IsAny<IGameItem>(), It.IsAny<bool>()))
		         .Callback<ICharacter?, IGameItem, bool>((_, item, _) => contents.Add(item));
		var item = Item(id, name, trueLocations, tags);
		item.SetupGet(x => x.DeepItems).Returns(() => [item.Object]);
		item.Setup(x => x.GetItemType<IContainer>()).Returns(container.Object);
		return item;
	}

	private static Mock<ITag> Tag(long id, string name)
	{
		var tag = new Mock<ITag>();
		tag.SetupGet(x => x.Id).Returns(id);
		tag.SetupGet(x => x.Name).Returns(name);
		tag.SetupGet(x => x.FullName).Returns(name);
		return tag;
	}

	private static Mock<IMerchandise> Merchandise(long id, string name)
	{
		var merchandise = new Mock<IMerchandise>();
		merchandise.SetupGet(x => x.Id).Returns(id);
		merchandise.SetupGet(x => x.Name).Returns(name);
		return merchandise;
	}

	private static PermanentShopFixture PermanentShop(long id, string name, ICurrency currency, ICharacter manager,
		EmploymentAuthority managerAuthority, ICell stockroom, IEnumerable<ICell> shopfronts,
		IEnumerable<IGameItem> displayContainers, IEnumerable<IMerchandise> merchandises,
		IEnumerable<IGameItem> stockedItems)
	{
		var shop = new Mock<IPermanentShop>();
		shop.SetupGet(x => x.Id).Returns(id);
		shop.SetupGet(x => x.Name).Returns(name);
		shop.SetupGet(x => x.EmploymentHostName).Returns(name);
		shop.SetupGet(x => x.FrameworkItemType).Returns("PermanentShop");
		shop.SetupGet(x => x.EmploymentHostType).Returns(MudSharp.Economy.Employment.EmploymentHostType.Shop);
		shop.SetupGet(x => x.Market).Returns((IMarket?)null);
		shop.SetupGet(x => x.Currency).Returns(currency);
		var shopfrontList = shopfronts.ToList();
		var displayContainerList = displayContainers.ToList();
		var merchandiseList = merchandises.ToList();
		shop.SetupGet(x => x.StockroomCell).Returns(stockroom);
		shop.SetupGet(x => x.ShopfrontCells).Returns(shopfrontList);
		shop.SetupGet(x => x.AllShopCells).Returns(() => shopfrontList.Concat(new[] { stockroom }).DistinctBy(x => x.Id).ToList());
		shop.SetupGet(x => x.DisplayContainers).Returns(displayContainerList);
		shop.SetupGet(x => x.Merchandises).Returns(merchandiseList);
		var employment = new EmploymentHostState(shop.Object);
		shop.SetupGet(x => x.Employment).Returns(employment);
		shop.SetupGet(x => x.TaskBoard).Returns(employment.TaskBoard);
		shop.SetupGet(x => x.EmploymentRegister).Returns(employment.EmploymentRegister);
		shop.SetupGet(x => x.BusinessLedger).Returns(employment.BusinessLedger);
		shop.SetupGet(x => x.ManagerGoalBoard).Returns(employment.ManagerGoalBoard);
		shop.SetupGet(x => x.Board).Returns(employment.Board);
		shop.SetupGet(x => x.EmploymentContracts).Returns(employment.EmploymentContracts);
		shop.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		    .Returns((ICharacter actor, EmploymentAuthority authority) => employment.HasAuthority(actor, authority));
		shop.Setup(x => x.StockedItems(It.IsAny<IMerchandise>()))
		    .Returns((IMerchandise merchandise) => stockedItems.Where(x => merchandiseList.Any(y => y.Id == merchandise.Id)).ToList());
		employment.Hire(manager, Offer(currency, EmploymentRole.Manager, managerAuthority), null);
		return new PermanentShopFixture(shop, employment);
	}

	private static CompensationTerms Pay(ICurrency currency, decimal amount = 10.0M)
	{
		return new CompensationTerms(
			new MoneyAmount(currency, amount),
			null,
			PayCadence.Hourly,
			new MoneyAmount(currency, amount),
			PaymentSource.HostCash);
	}

	private static EmploymentOffer Offer(ICurrency currency, EmploymentRole role,
		EmploymentAuthority authority = EmploymentAuthority.None)
	{
		return new EmploymentOffer(
			role,
			Pay(currency),
			WorkSchedule.AnyTime,
			EmploymentDuration.Indefinite,
			new PaymentMethod(PaymentMethodKind.Cash),
			authority);
	}

	private static JobOpeningDefinition Opening(ICurrency currency)
	{
		return new JobOpeningDefinition(
			EmploymentRole.Employee,
			new JobRequirementSet(
				[new SkillRequirement("haggling", 25.0)],
				[],
				[new AICapabilityRequirement(EmploymentAICapability.CanPurchaseCommodities)],
				[]),
			Pay(currency),
			WorkSchedule.AnyTime,
			EmploymentDuration.Indefinite,
			1,
			true,
			new PaymentMethod(PaymentMethodKind.Cash),
			EmploymentAuthoritySet.Empty);
	}

	private static IReadOnlySet<EmploymentAICapability> Caps(params EmploymentAICapability[] capabilities)
	{
		return new HashSet<EmploymentAICapability>(capabilities);
	}

	private static EmploymentCandidateProfile Profile(ICharacter candidate, decimal reservationWage,
		PaymentMethodKind paymentMethod, IReadOnlySet<EmploymentAICapability> capabilities,
		IReadOnlyDictionary<string, double>? skills = null)
	{
		return new EmploymentCandidateProfile(
			candidate,
			reservationWage,
			skills ?? new Dictionary<string, double>(),
			new HashSet<string>(),
			capabilities,
			new HashSet<string>(),
			[paymentMethod]);
	}

	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		              .UseInMemoryDatabase(Guid.NewGuid().ToString())
		              .Options;
		return new FuturemudDatabaseContext(options);
	}

	private static FMDBState CaptureFMDBState()
	{
		return new FMDBState(
			(FuturemudDatabaseContext?)typeof(FMDB).GetProperty("Context", BindingFlags.Public | BindingFlags.Static)!
			                                  .GetValue(null),
			typeof(FMDB).GetProperty("Connection", BindingFlags.Public | BindingFlags.Static)!.GetValue(null),
			(uint)typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!
			                  .GetValue(null)!);
	}

	private static void PrimeFMDB(FuturemudDatabaseContext context)
	{
		typeof(FMDB).GetProperty("Context", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, context);
		typeof(FMDB).GetProperty("Connection", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, null);
		typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!.SetValue(null, 1u);
	}

	private static void RestoreFMDBState(FMDBState state)
	{
		typeof(FMDB).GetProperty("Context", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, state.Context);
		typeof(FMDB).GetProperty("Connection", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, state.Connection);
		typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!
		            .SetValue(null, state.InstanceCount);
	}

	private static Mock<IFuturemud> Gameworld(ICurrency currency, IReadOnlyDictionary<long, ICharacter> characters,
		IEnumerable<ICell>? cellsToAdd = null, IReadOnlyDictionary<long, IGameItem>? items = null,
		IEnumerable<IBankAccount>? bankAccountsToAdd = null, IEnumerable<IShop>? shopsToAdd = null,
		IEnumerable<ICombatArena>? combatArenasToAdd = null, IEnumerable<IBank>? banksToAdd = null,
		IEnumerable<IStable>? stablesToAdd = null, IEnumerable<IClan>? clansToAdd = null,
		IEnumerable<IProperty>? propertiesToAdd = null)
	{
		var boards = new All<IBoard>();
		var currencies = new All<ICurrency>();
		currencies.Add(currency);
		var bankAccounts = new All<IBankAccount>();
		foreach (var account in bankAccountsToAdd ?? [])
		{
			bankAccounts.Add(account);
		}
		var shops = new All<IShop>();
		foreach (var shop in shopsToAdd ?? [])
		{
			shops.Add(shop);
		}
		var combatArenas = new All<ICombatArena>();
		foreach (var arena in combatArenasToAdd ?? [])
		{
			combatArenas.Add(arena);
		}
		var banks = new All<IBank>();
		foreach (var bank in banksToAdd ?? [])
		{
			banks.Add(bank);
		}
		var stables = new All<IStable>();
		foreach (var stable in stablesToAdd ?? [])
		{
			stables.Add(stable);
		}
		var clans = new All<IClan>();
		foreach (var clan in clansToAdd ?? [])
		{
			clans.Add(clan);
		}
		var properties = new All<IProperty>();
		foreach (var property in propertiesToAdd ?? [])
		{
			properties.Add(property);
		}
		var cells = new All<ICell>();
		foreach (var cell in cellsToAdd ?? [])
		{
			cells.Add(cell);
		}

		var calendars = new All<ICalendar>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Boards).Returns(boards);
		gameworld.SetupGet(x => x.Currencies).Returns(currencies);
		gameworld.SetupGet(x => x.BankAccounts).Returns(bankAccounts);
		gameworld.SetupGet(x => x.Shops).Returns(shops);
		gameworld.SetupGet(x => x.CombatArenas).Returns(combatArenas);
		gameworld.SetupGet(x => x.Banks).Returns(banks);
		gameworld.SetupGet(x => x.Stables).Returns(stables);
		gameworld.SetupGet(x => x.Clans).Returns(clans);
		gameworld.SetupGet(x => x.Properties).Returns(properties);
		gameworld.SetupGet(x => x.Cells).Returns(cells);
		gameworld.SetupGet(x => x.Calendars).Returns(calendars);
		gameworld.Setup(x => x.Add(It.IsAny<IBoard>())).Callback<IBoard>(board => boards.Add(board));
		gameworld.Setup(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()))
		         .Returns((long id, bool _) => characters.TryGetValue(id, out var character) ? character : null!);
		gameworld.Setup(x => x.TryGetItem(It.IsAny<long>(), It.IsAny<bool>()))
		         .Returns((long id, bool _) => items is not null && items.TryGetValue(id, out var item) ? item : null!);
		return gameworld;
	}

	private static EmploymentActionStepResult AdvanceTaskOrAssignmentFailure(EmploymentTaskDispatcher dispatcher,
		IEmploymentActiveTask task, IEmploymentTaskContext context, params EmploymentCandidateProfile[] profiles)
	{
		if (task.AssignedEmployee is null &&
		    !dispatcher.TryAssignTask(task, profiles, context, out var reason))
		{
			return EmploymentActionStepResult.Blocked(reason);
		}

		return dispatcher.AdvanceTask(task, context);
	}

	private sealed class TestEmploymentHost : FrameworkItem, IEmploymentHost
	{
		public TestEmploymentHost(string name, ICurrency currency)
		{
			_id = 1;
			_name = name;
			Currency = currency;
			Employment = new EmploymentHostState(this);
		}

		public override string FrameworkItemType => "TestEmploymentHost";
		public ICurrency Currency { get; }
		public EmploymentHostType EmploymentHostType => MudSharp.Economy.Employment.EmploymentHostType.Other;
		public IMarket? Market => null;
		public IEmploymentHostState Employment { get; }
	}

	private sealed class PersistedEmploymentHost : FrameworkItem, IEmploymentHost, IHaveFuturemud, IHaveCurrency
	{
		private IEmploymentHostState? _employment;

		public PersistedEmploymentHost(long id, string name, IFuturemud gameworld, ICurrency currency)
		{
			_id = id;
			_name = name;
			Gameworld = gameworld;
			Currency = currency;
		}

		public override string FrameworkItemType => "PersistedEmploymentHost";
		public IFuturemud Gameworld { get; }
		public ICurrency Currency { get; set; }
		public EmploymentHostType EmploymentHostType => MudSharp.Economy.Employment.EmploymentHostType.Shop;
		public IMarket? Market => null;
		public IEmploymentHostState Employment => _employment ??= EmploymentPersistenceStore.LoadOrCreate(this);
	}
}
