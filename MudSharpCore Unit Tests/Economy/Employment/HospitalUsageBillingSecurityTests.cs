using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Commands;
using MudSharp.Commands.Trees;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Employment;
using MudSharp.Economy.Hospitals;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Health;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp_Unit_Tests.Economy.Employment;

public partial class UnifiedEmploymentDispatchTests
{
	private sealed class HospitalBillingTestDbScope : IDisposable
	{
		private readonly FMDBState _previous = CaptureFMDBState();
		public FuturemudDatabaseContext Database { get; } = BuildContext();

		public HospitalBillingTestDbScope() => PrimeFMDB(Database);

		public void Dispose()
		{
			RestoreFMDBState(_previous);
			Database.Dispose();
		}
	}

	private sealed class HospitalBillingFixture
	{
		public Mock<IHospital> Hospital { get; } = new();
		public Mock<IHospitalService> Service { get; } = new();
		public Mock<IHospitalService> BindingService { get; } = new();
		public Mock<ICharacter> Doctor { get; }
		public Mock<ICharacter> Patient { get; }
		public Mock<IWound> Wound { get; } = new();
		public Mock<ICharacterCommandManager> Commands { get; } = new();
		public List<Binding> BindingEffects { get; } = new();
		public HospitalPatientDebtAccount Account { get; }
		public HospitalServiceRequest Request { get; }
		public EmploymentActiveTask Task { get; }
		public EmploymentTaskContext Context { get; }
		public bool StartCommand { get; set; }
		public Action? BeforeCommand { get; set; }

		public HospitalBillingFixture(FuturemudDatabaseContext database, HospitalServiceType serviceType,
			decimal balance = 0.0M, decimal maximumDebt = 20.0M,
			HospitalPaymentMethod paymentMethod = HospitalPaymentMethod.Debt)
		{
			var currency = Currency();
			Doctor = Character(401, "Doctor");
			Patient = Character(402, "Patient");
			var world = Gameworld(currency.Object, new Dictionary<long, ICharacter>
			{
				[401] = Doctor.Object, [402] = Patient.Object
			});
			world.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
			Doctor.SetupGet(x => x.Gameworld).Returns(world.Object);
			Patient.SetupGet(x => x.Gameworld).Returns(world.Object);
			var room = Room(403, "clinic");
			room.Setup(x => x.LayerCharacters(It.IsAny<RoomLayer>())).Returns([Doctor.Object, Patient.Object]);
			Doctor.SetupGet(x => x.Location).Returns(room.Object);
			Patient.SetupGet(x => x.Location).Returns(room.Object);
			Doctor.Setup(x => x.ColocatedWith(Patient.Object)).Returns(true);
			Doctor.Setup(x => x.EffectsOfType<Binding>(It.IsAny<Predicate<Binding>>())).Returns(() => BindingEffects);
			var body = new Mock<IBody>();
			body.SetupGet(x => x.TotalBloodVolumeLitres).Returns(5.0);
			body.SetupGet(x => x.CurrentBloodVolumeLitres).Returns(5.0);
			Patient.SetupGet(x => x.Body).Returns(body.Object);
			Wound.SetupGet(x => x.BleedStatus).Returns(BleedStatus.Bleeding);
			Wound.Setup(x => x.CanBeTreated(It.IsAny<TreatmentType>())).Returns(Difficulty.Impossible);
			Wound.Setup(x => x.CanBeTreated(TreatmentType.Trauma)).Returns(Difficulty.Normal);
			Patient.SetupGet(x => x.Wounds).Returns([]);
			Patient.Setup(x => x.VisibleWounds(It.IsAny<IPerceiver>(), WoundExaminationType.Examination))
				.Returns([Wound.Object]);
			Hospital.SetupGet(x => x.Id).Returns(404);
			Hospital.SetupGet(x => x.Name).Returns("clinic");
			Hospital.SetupGet(x => x.FrameworkItemType).Returns("Hospital");
			Hospital.SetupGet(x => x.EmploymentHostType).Returns(EmploymentHostType.Hospital);
			Hospital.SetupGet(x => x.Gameworld).Returns(world.Object);
			Hospital.SetupGet(x => x.Currency).Returns(currency.Object);
			Service.SetupGet(x => x.Id).Returns(405);
			Service.SetupGet(x => x.Name).Returns(serviceType.ToString());
			Service.SetupGet(x => x.ServiceType).Returns(serviceType);
			Service.SetupGet(x => x.AllowDebt).Returns(true);
			Service.SetupGet(x => x.RequiredEquipment).Returns([]);
			BindingService.SetupGet(x => x.Id).Returns(406);
			BindingService.SetupGet(x => x.ServiceType).Returns(HospitalServiceType.Binding);
			BindingService.SetupGet(x => x.IsActive).Returns(true);
			BindingService.SetupGet(x => x.Price).Returns(12.5M);
			Hospital.SetupGet(x => x.Services).Returns([Service.Object, BindingService.Object]);
			var accountRecord = new MudSharp.Models.HospitalPatientDebtAccount
			{
				Id = 407, HospitalId = 404, PatientId = 402, PatientName = "Patient",
				Balance = balance, MaximumDebt = maximumDebt, LastUpdatedAtUtc = DateTime.UtcNow
			};
			var requestRecord = new MudSharp.Models.HospitalServiceRequest
			{
				Id = 408, HospitalId = 404, HospitalServiceId = 405,
				RequesterId = 402, PatientId = 402, RequesterName = "Patient", PatientName = "Patient",
				Status = (int)HospitalServiceRequestStatus.Queued, PaymentMethod = (int)paymentMethod,
				CreatedAtUtc = DateTime.UtcNow, LastUpdatedAtUtc = DateTime.UtcNow,
				OperationalNotes = string.Empty, ProcedureParameters = string.Empty
			};
			database.HospitalPatientDebtAccounts.Add(accountRecord);
			database.HospitalServiceRequests.Add(requestRecord);
			database.SaveChanges();
			Account = new HospitalPatientDebtAccount(accountRecord, Hospital.Object);
			Request = new HospitalServiceRequest(requestRecord, Hospital.Object);
			Hospital.Setup(x => x.DebtAccountFor(Patient.Object, true)).Returns(Account);
			var employment = EmploymentPersistenceStore.LoadOrCreate(Hospital.Object);
			Hospital.SetupGet(x => x.Employment).Returns(employment);
			Hospital.SetupGet(x => x.EmploymentRegister).Returns(employment.EmploymentRegister);
			Hospital.SetupGet(x => x.TaskBoard).Returns(employment.TaskBoard);
			Task = (EmploymentActiveTask)employment.TaskBoard.CreateActiveTask("treat patient",
				new EmploymentActionPlan([new HospitalServiceActionStep(Hospital.Object, Request)]), null);
			Task.Assign(Doctor.Object);
			Context = new EmploymentTaskContext(Hospital.Object);
			Context.HydrateTaskState(Task, 0);
			var tree = new Mock<ICharacterCommandTree>();
			tree.SetupGet(x => x.Commands).Returns(Commands.Object);
			Doctor.SetupGet(x => x.CommandTree).Returns(tree.Object);
			Commands.Setup(x => x.Execute(Doctor.Object, It.IsAny<string>(), It.IsAny<CharacterState>(),
					It.IsAny<PermissionLevel>(), It.IsAny<IOutputHandler>()))
				.Returns(() =>
				{
					BeforeCommand?.Invoke();
					if (StartCommand)
					{
						// Observe phase startup without running a timed medical effect in this unit fixture.
						var effect = (Binding)RuntimeHelpers.GetUninitializedObject(typeof(Binding));
						effect.TargetCharacter = Patient.Object;
						BindingEffects.Add(effect);
					}
					return StartCommand;
				});
		}

		public EmploymentActionStepResult Execute() => HospitalMedicalServiceRunner.ExecuteServiceRequest(
			Context, Doctor.Object, Hospital.Object, Request);

		public void Hydrate(string payload)
		{
			Task.MarkStep(0, EmploymentActionStepStatus.InProgress,
				new EmploymentActionStepOperationalState(OperationalPayload:
					$"hospitalservice;hospital=404;request=408;type={Service.Object.ServiceType};{payload}"));
			Context.HydrateTaskState(Task, 0);
		}
	}

	[TestMethod]
	[DataRow(HospitalServiceType.Stabilisation)]
	[DataRow(HospitalServiceType.FullTreatment)]
	public void HospitalUsageBilling_InsufficientCredit_PreventsTreatment(HospitalServiceType serviceType)
	{
		using var scope = new HospitalBillingTestDbScope();
		var fixture = new HospitalBillingFixture(scope.Database, serviceType, balance: 15.0M);
		var result = fixture.Execute();
		Assert.IsFalse(result.Success);
		StringAssert.Contains(result.Message, "usage charge could not be secured");
		Assert.AreEqual(15.0M, fixture.Account.Balance);
		Assert.AreEqual(0.0M, fixture.Request.DebtCharged);
		fixture.Commands.Verify(x => x.Execute(It.IsAny<ICharacter>(), It.IsAny<string>(), It.IsAny<CharacterState>(),
			It.IsAny<PermissionLevel>(), It.IsAny<IOutputHandler>()), Times.Never);
		fixture.Wound.Verify(x => x.Treat(It.IsAny<IPerceiver>(), It.IsAny<TreatmentType>(),
			It.IsAny<ITreatment>(), It.IsAny<Outcome>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void HospitalUsageBilling_ReservationAndTaskCheckpoint_AreSavedBeforeCommand()
	{
		using var scope = new HospitalBillingTestDbScope();
		var fixture = new HospitalBillingFixture(scope.Database, HospitalServiceType.FullTreatment) { StartCommand = true };
		fixture.BeforeCommand = () =>
		{
			Assert.AreEqual(12.5M, scope.Database.HospitalPatientDebtAccounts.AsNoTracking().Single().Balance);
			Assert.AreEqual(12.5M, scope.Database.HospitalServiceRequests.AsNoTracking().Single().DebtCharged);
			StringAssert.Contains(scope.Database.EmploymentActiveTaskStepStates.AsNoTracking().Single().OperationalPayload,
				"reserved=bind:1:12.5");
		};
		Assert.IsTrue(fixture.Execute().Success);
		Assert.AreEqual(12.5M, fixture.Account.Balance);
		Assert.IsTrue(fixture.Execute().Success);
		Assert.AreEqual(12.5M, fixture.Account.Balance, "A repeated active-phase heartbeat must not debit again.");
		fixture.BindingService.SetupGet(x => x.Price).Returns(500.0M);
		fixture.BindingEffects.Clear();
		fixture.Patient.Setup(x => x.VisibleWounds(It.IsAny<IPerceiver>(), WoundExaminationType.Examination)).Returns([]);
		var completed = fixture.Execute();
		Assert.IsTrue(completed.Success, completed.Message);
		Assert.IsTrue(completed.Completed);
		Assert.AreEqual(12.5M, fixture.Request.Price, "Delivered care keeps its reserved price.");
		Assert.AreEqual(12.5M, fixture.Request.DebtCharged);
	}

	[TestMethod]
	public void HospitalUsageBilling_UnstartedPhase_RefundsPrepaidCreditExactly()
	{
		using var scope = new HospitalBillingTestDbScope();
		var fixture = new HospitalBillingFixture(scope.Database, HospitalServiceType.FullTreatment, balance: -25.0M);
		Assert.IsFalse(fixture.Execute().Success);
		Assert.AreEqual(-25.0M, fixture.Account.Balance);
		Assert.AreEqual(0.0M, fixture.Request.DebtCharged);
		Assert.AreEqual(-25.0M, scope.Database.HospitalPatientDebtAccounts.AsNoTracking().Single().Balance);
		Assert.IsFalse(fixture.Task.StepOperationalStates[0].OperationalPayload?.Contains("reserved=bind") ?? false);
	}

	[TestMethod]
	public void HospitalUsageBilling_WaivedPhase_DoesNotAccessDebt()
	{
		using var scope = new HospitalBillingTestDbScope();
		var fixture = new HospitalBillingFixture(scope.Database, HospitalServiceType.FullTreatment,
			paymentMethod: HospitalPaymentMethod.Waived) { StartCommand = true };
		fixture.Service.SetupGet(x => x.AllowDebt).Returns(false);
		fixture.Account.IsSuspended = true;
		Assert.IsTrue(fixture.Execute().Success);
		Assert.AreEqual(0.0M, fixture.Request.DebtCharged);
		Assert.AreEqual(12.5M, fixture.Request.Price);
		fixture.Hospital.Verify(x => x.DebtAccountFor(It.IsAny<ICharacter>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void HospitalUsageBilling_LegacyUsage_IsReconciledOnlyOnce()
	{
		using var scope = new HospitalBillingTestDbScope();
		var fixture = new HospitalBillingFixture(scope.Database, HospitalServiceType.FullTreatment);
		fixture.Patient.Setup(x => x.VisibleWounds(It.IsAny<IPerceiver>(), WoundExaminationType.Examination)).Returns([]);
		fixture.Hydrate("done=bind;charges=Binding:1");
		Assert.IsTrue(fixture.Execute().Success);
		Assert.AreEqual(12.5M, fixture.Account.Balance);
		Assert.AreEqual(12.5M, fixture.Request.DebtCharged);
		fixture.Request.MarkStatus(HospitalServiceRequestStatus.InProgress, "resume legacy checkpoint");
		fixture.Hydrate("done=bind;charges=Binding:1;reserved=bind:1:12.5");
		Assert.IsTrue(fixture.Execute().Success);
		Assert.AreEqual(12.5M, fixture.Account.Balance);
	}

	[TestMethod]
	public void HospitalUsageBilling_StartedPhaseCancellation_RetainsCharge()
	{
		using var scope = new HospitalBillingTestDbScope();
		var fixture = new HospitalBillingFixture(scope.Database, HospitalServiceType.FullTreatment) { StartCommand = true };
		Assert.IsTrue(fixture.Execute().Success);
		fixture.Request.MarkStatus(HospitalServiceRequestStatus.Cancelled, "patient cancelled");
		Assert.IsFalse(fixture.Execute().Success);
		Assert.AreEqual(12.5M, fixture.Account.Balance);
		Assert.AreEqual(12.5M, fixture.Request.DebtCharged);
	}

	[TestMethod]
	public void HospitalUsageBilling_InterleavedRequests_CannotReuseReservedCredit()
	{
		using var scope = new HospitalBillingTestDbScope();
		var fixture = new HospitalBillingFixture(scope.Database, HospitalServiceType.FullTreatment) { StartCommand = true };
		Assert.IsTrue(fixture.Execute().Success);
		var secondRequest = new HospitalServiceRequest(fixture.Hospital.Object, fixture.Service.Object,
			fixture.Patient.Object, fixture.Patient.Object, HospitalPaymentMethod.Debt);
		var secondTask = (EmploymentActiveTask)fixture.Hospital.Object.TaskBoard.CreateActiveTask("second treatment",
			new EmploymentActionPlan([new HospitalServiceActionStep(fixture.Hospital.Object, secondRequest)]), null);
		secondTask.Assign(fixture.Doctor.Object);
		var secondContext = new EmploymentTaskContext(fixture.Hospital.Object);
		secondContext.HydrateTaskState(secondTask, 0);
		var result = HospitalMedicalServiceRunner.ExecuteServiceRequest(secondContext, fixture.Doctor.Object,
			fixture.Hospital.Object, secondRequest);
		Assert.IsFalse(result.Success);
		StringAssert.Contains(result.Message, "usage charge could not be secured");
		Assert.AreEqual(12.5M, fixture.Account.Balance);
		Assert.AreEqual(0.0M, secondRequest.DebtCharged);
		fixture.Commands.Verify(x => x.Execute(It.IsAny<ICharacter>(), It.IsAny<string>(), It.IsAny<CharacterState>(),
			It.IsAny<PermissionLevel>(), It.IsAny<IOutputHandler>()), Times.Once);
	}

	[TestMethod]
	[DataRow(true, true)]
	[DataRow(false, false)]
	public void HospitalUsageBilling_UnavailableDebt_PreventsCare(bool suspended, bool allowDebt)
	{
		using var scope = new HospitalBillingTestDbScope();
		var fixture = new HospitalBillingFixture(scope.Database, HospitalServiceType.FullTreatment) { StartCommand = true };
		fixture.Account.IsSuspended = suspended;
		fixture.Service.SetupGet(x => x.AllowDebt).Returns(allowDebt);
		Assert.IsFalse(fixture.Execute().Success);
		Assert.AreEqual(0.0M, fixture.Account.Balance);
		fixture.Commands.Verify(x => x.Execute(It.IsAny<ICharacter>(), It.IsAny<string>(), It.IsAny<CharacterState>(),
			It.IsAny<PermissionLevel>(), It.IsAny<IOutputHandler>()), Times.Never);
	}

	[TestMethod]
	[DataRow(HospitalServiceType.Stabilisation)]
	[DataRow(HospitalServiceType.FullTreatment)]
	public void HospitalUsageBilling_LegacyActiveTransfusion_RequiresCreditBeforeResumption(HospitalServiceType serviceType)
	{
		using var scope = new HospitalBillingTestDbScope();
		var fixture = new HospitalBillingFixture(scope.Database, serviceType, balance: 15.0M);
		fixture.BindingService.SetupGet(x => x.ServiceType).Returns(HospitalServiceType.BloodTransfusion);
		fixture.Hydrate("active=bloodtransfusion;activecount=0;blood=kind:bloodtransfusion|stage:dripping|target:0.5");
		var result = fixture.Execute();
		Assert.IsFalse(result.Success);
		StringAssert.Contains(result.Message, "usage charge could not be secured");
		Assert.AreEqual(15.0M, fixture.Account.Balance);
		Assert.AreEqual(0.0M, fixture.Request.DebtCharged);
		Mock.Get(fixture.Patient.Object.Body).VerifySet(x => x.CurrentBloodVolumeLitres = It.IsAny<double>(), Times.Never);
	}
}
