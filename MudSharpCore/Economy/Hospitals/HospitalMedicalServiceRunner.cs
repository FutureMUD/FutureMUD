using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.Health.Wounds;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp.Economy.Hospitals;

public static class HospitalMedicalServiceRunner
{
	private const string BloodDonationPhase = "blooddonation";
	private const string BloodTransfusionPhase = "bloodtransfusion";
	private const string BloodWorkflowStageCannulating = "cannulating";
	private const string BloodWorkflowStageDraining = "draining";
	private const string BloodWorkflowStageDripping = "dripping";
	private const string BloodWorkflowStageDecannulating = "decannulating";
	private const double BloodWorkflowToleranceLitres = 0.01;

	private sealed record UsageCharge(HospitalServiceType ServiceType, int Count);

	private sealed class BloodWorkflowProgress
	{
		public string Kind { get; set; } = string.Empty;
		public string Stage { get; set; } = string.Empty;
		public long? ContainerId { get; set; }
		public long? DripId { get; set; }
		public long? CannulaId { get; set; }
		public bool HospitalInsertedCannula { get; set; }
		public bool UseSubstitute { get; set; }
		public double TargetLitres { get; set; }
		public double StartingPatientBloodLitres { get; set; }
		public double StartingContainerVolume { get; set; }
		public double StockBeforeLitres { get; set; }
		public double CompletedLitres { get; set; }
		public HashSet<long> UsedContainerIds { get; } = new();

		public string ToPayload()
		{
			var parts = new List<string>
			{
				$"kind:{Kind}",
				$"stage:{Stage}",
				$"target:{TargetLitres.ToString("R", CultureInfo.InvariantCulture)}",
				$"startblood:{StartingPatientBloodLitres.ToString("R", CultureInfo.InvariantCulture)}",
				$"startcontainer:{StartingContainerVolume.ToString("R", CultureInfo.InvariantCulture)}",
				$"stockbefore:{StockBeforeLitres.ToString("R", CultureInfo.InvariantCulture)}",
				$"completed:{CompletedLitres.ToString("R", CultureInfo.InvariantCulture)}",
				$"inserted:{HospitalInsertedCannula.ToString(CultureInfo.InvariantCulture)}",
				$"substitute:{UseSubstitute.ToString(CultureInfo.InvariantCulture)}"
			};
			if (ContainerId is not null)
			{
				parts.Add($"container:{ContainerId.Value.ToString("F0", CultureInfo.InvariantCulture)}");
			}

			if (DripId is not null)
			{
				parts.Add($"drip:{DripId.Value.ToString("F0", CultureInfo.InvariantCulture)}");
			}

			if (CannulaId is not null)
			{
				parts.Add($"cannula:{CannulaId.Value.ToString("F0", CultureInfo.InvariantCulture)}");
			}

			if (UsedContainerIds.Any())
			{
				parts.Add($"used:{UsedContainerIds.OrderBy(x => x).Select(x => x.ToString("F0", CultureInfo.InvariantCulture)).ListToCommaSeparatedValues()}");
			}

			return string.Join('|', parts);
		}

		public static BloodWorkflowProgress? FromPayload(string payload)
		{
			var parts = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			foreach (var part in payload.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
			{
				var split = part.Split(':', 2, StringSplitOptions.TrimEntries);
				if (split.Length != 2)
				{
					continue;
				}

				parts[split[0]] = split[1];
			}

			if (!parts.TryGetValue("kind", out var kind) ||
			    !parts.TryGetValue("stage", out var stage))
			{
				return null;
			}

			var result = new BloodWorkflowProgress
			{
				Kind = kind,
				Stage = stage
			};

			if (parts.TryGetValue("container", out var container) &&
			    long.TryParse(container, NumberStyles.Any, CultureInfo.InvariantCulture, out var containerId))
			{
				result.ContainerId = containerId;
			}

			if (parts.TryGetValue("drip", out var drip) &&
			    long.TryParse(drip, NumberStyles.Any, CultureInfo.InvariantCulture, out var dripId))
			{
				result.DripId = dripId;
			}

			if (parts.TryGetValue("cannula", out var cannula) &&
			    long.TryParse(cannula, NumberStyles.Any, CultureInfo.InvariantCulture, out var cannulaId))
			{
				result.CannulaId = cannulaId;
			}

			if (parts.TryGetValue("target", out var target) &&
			    double.TryParse(target, NumberStyles.Any, CultureInfo.InvariantCulture, out var targetLitres))
			{
				result.TargetLitres = Math.Max(0.0, targetLitres);
			}

			if (parts.TryGetValue("startblood", out var startBlood) &&
			    double.TryParse(startBlood, NumberStyles.Any, CultureInfo.InvariantCulture, out var startBloodLitres))
			{
				result.StartingPatientBloodLitres = Math.Max(0.0, startBloodLitres);
			}

			if (parts.TryGetValue("startcontainer", out var startContainer) &&
			    double.TryParse(startContainer, NumberStyles.Any, CultureInfo.InvariantCulture, out var startContainerVolume))
			{
				result.StartingContainerVolume = Math.Max(0.0, startContainerVolume);
			}

			if (parts.TryGetValue("stockbefore", out var stockBefore) &&
			    double.TryParse(stockBefore, NumberStyles.Any, CultureInfo.InvariantCulture, out var stockBeforeLitres))
			{
				result.StockBeforeLitres = Math.Max(0.0, stockBeforeLitres);
			}

			if (parts.TryGetValue("completed", out var completed) &&
			    double.TryParse(completed, NumberStyles.Any, CultureInfo.InvariantCulture, out var completedLitres))
			{
				result.CompletedLitres = Math.Max(0.0, completedLitres);
			}

			if (parts.TryGetValue("inserted", out var inserted) &&
			    bool.TryParse(inserted, out var hospitalInsertedCannula))
			{
				result.HospitalInsertedCannula = hospitalInsertedCannula;
			}

			if (parts.TryGetValue("substitute", out var substitute) &&
			    bool.TryParse(substitute, out var useSubstitute))
			{
				result.UseSubstitute = useSubstitute;
			}

			if (parts.TryGetValue("used", out var used))
			{
				foreach (var idText in used.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
				{
					if (long.TryParse(idText, NumberStyles.Any, CultureInfo.InvariantCulture, out var usedId))
					{
						result.UsedContainerIds.Add(usedId);
					}
				}
			}

			return result;
		}
	}

	private sealed class HospitalTreatmentProgress
	{
		public string? ActivePhase { get; set; }
		public int ActiveExpectedCount { get; set; }
		public HashSet<string> CompletedPhases { get; } = new(StringComparer.InvariantCultureIgnoreCase);
		public Dictionary<HospitalServiceType, int> Charges { get; } = new();
		public BloodWorkflowProgress? BloodWorkflow { get; set; }

		public IReadOnlyList<UsageCharge> UsageCharges =>
			Charges.Select(x => new UsageCharge(x.Key, x.Value)).ToList();

		public void CompleteActivePhase(HospitalServiceType serviceType)
		{
			if (string.IsNullOrWhiteSpace(ActivePhase))
			{
				return;
			}

			CompletedPhases.Add(ActivePhase);
			if (ActiveExpectedCount > 0)
			{
				Charges[serviceType] = Charges.GetValueOrDefault(serviceType) + ActiveExpectedCount;
			}

			ActivePhase = null;
			ActiveExpectedCount = 0;
		}

		public void AddCharge(HospitalServiceType serviceType, int count = 1)
		{
			if (count <= 0)
			{
				return;
			}

			Charges[serviceType] = Charges.GetValueOrDefault(serviceType) + count;
		}

		public string ToPayload(IHospital hospital, IHospitalServiceRequest request, bool completed)
		{
			var parts = new List<string>
			{
				"hospitalservice",
				$"hospital={hospital.Id.ToString("F0", CultureInfo.InvariantCulture)}",
				$"request={request.Id.ToString("F0", CultureInfo.InvariantCulture)}",
				$"type={request.Service.ServiceType}",
				$"completed={completed.ToString(CultureInfo.InvariantCulture)}"
			};
			if (!string.IsNullOrWhiteSpace(ActivePhase))
			{
				parts.Add($"active={ActivePhase}");
				parts.Add($"activecount={ActiveExpectedCount.ToString("F0", CultureInfo.InvariantCulture)}");
			}

			if (CompletedPhases.Any())
			{
				parts.Add($"done={CompletedPhases.OrderBy(x => x).ListToCommaSeparatedValues()}");
			}

			if (Charges.Any())
			{
				parts.Add(
					$"charges={Charges.Select(x => $"{x.Key}:{x.Value.ToString("F0", CultureInfo.InvariantCulture)}").ListToCommaSeparatedValues()}");
			}

			if (BloodWorkflow is not null)
			{
				parts.Add($"blood={BloodWorkflow.ToPayload()}");
			}

			return string.Join(';', parts);
		}

		public static HospitalTreatmentProgress FromPayload(string? payload, IHospitalServiceRequest request)
		{
			var progress = new HospitalTreatmentProgress();
			if (string.IsNullOrWhiteSpace(payload))
			{
				return progress;
			}

			var parts = ParsePayload(payload);
			if (!parts.TryGetValue("request", out var requestText) ||
			    !long.TryParse(requestText, NumberStyles.Any, CultureInfo.InvariantCulture, out var requestId) ||
			    requestId != request.Id)
			{
				return progress;
			}

			if (parts.TryGetValue("active", out var active))
			{
				progress.ActivePhase = active;
			}

			if (parts.TryGetValue("activecount", out var activeCount) &&
			    int.TryParse(activeCount, NumberStyles.Any, CultureInfo.InvariantCulture, out var count))
			{
				progress.ActiveExpectedCount = Math.Max(0, count);
			}

			if (parts.TryGetValue("done", out var done))
			{
				foreach (var phase in done.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
				{
					progress.CompletedPhases.Add(phase);
				}
			}

			if (parts.TryGetValue("charges", out var charges))
			{
				foreach (var chargeText in charges.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
				{
					var pair = chargeText.Split(':', 2, StringSplitOptions.TrimEntries);
					if (pair.Length != 2 ||
					    !Enum.TryParse<HospitalServiceType>(pair[0], out var serviceType) ||
					    !int.TryParse(pair[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var chargeCount))
					{
						continue;
					}

					progress.Charges[serviceType] = Math.Max(0, chargeCount);
				}
			}

			if (parts.TryGetValue("blood", out var blood) &&
			    BloodWorkflowProgress.FromPayload(blood) is { } bloodWorkflow)
			{
				progress.BloodWorkflow = bloodWorkflow;
			}

			return progress;
		}
	}

	private sealed record ServiceExecutionResult(
		bool Success,
		string Message,
		string Resource,
		bool Completed = true,
		IReadOnlyList<UsageCharge>? UsageCharges = null,
		HospitalTreatmentProgress? Progress = null);

	private enum AnesthesiaPreparationResult
	{
		Ready,
		StartedCannulation,
		Failed
	}

	public static bool ShouldUseTreatmentTheatre(IHospitalService service)
	{
		return service.PreferOperatingTheatre ||
		       service.ServiceType is HospitalServiceType.Stabilisation or HospitalServiceType.FullTreatment;
	}

	public static bool UsesCommandRoutedWoundCare(IHospitalService service)
	{
		return service.ServiceType is HospitalServiceType.Binding or HospitalServiceType.WoundCleaning or
			HospitalServiceType.WoundClosing or HospitalServiceType.WoundTending or HospitalServiceType.BoneRelocation or
			HospitalServiceType.Stabilisation or HospitalServiceType.FullTreatment;
	}

	public static bool CanBeRequestedStandalone(IHospitalService service)
	{
		return service.OfferingMode is HospitalServiceOfferingMode.StandaloneAndCombined or
			HospitalServiceOfferingMode.StandaloneOnly;
	}

	public static bool CanBeUsedByCombinedService(IHospitalService service)
	{
		return service.OfferingMode is HospitalServiceOfferingMode.StandaloneAndCombined or
			HospitalServiceOfferingMode.CombinedOnly;
	}

	public static bool TryResolveSurgicalProcedureForService(ICharacter employee, ICharacter patient,
		IHospitalService service, string? procedureParameters, out ISurgicalProcedure? procedure)
	{
		procedure = service.SurgicalProcedure ??
		            ResolveSurgicalProcedureForService(employee, patient, service, procedureParameters);
		if (procedure is null)
		{
			return false;
		}

		if (service.ImplantItemPrototype is not null)
		{
			return true;
		}

		var args = ServiceProcedureArguments(employee, patient, service, procedureParameters, procedure, false)
			.ToArray();
		return procedure.CanPerformProcedure(employee, patient, args);
	}

	public static SurgicalProcedureType? ServiceTypeToSurgicalProcedureType(HospitalServiceType serviceType)
	{
		return serviceType switch
		{
			HospitalServiceType.Triage => SurgicalProcedureType.Triage,
			HospitalServiceType.DetailedExamination => SurgicalProcedureType.DetailedExamination,
			HospitalServiceType.ExploratorySurgery => SurgicalProcedureType.ExploratorySurgery,
			HospitalServiceType.TraumaControl => SurgicalProcedureType.TraumaControl,
			HospitalServiceType.OrganStabilisation => SurgicalProcedureType.OrganStabilisation,
			HospitalServiceType.OrganExtraction => SurgicalProcedureType.OrganExtraction,
			HospitalServiceType.OrganTransplant => SurgicalProcedureType.OrganTransplant,
			HospitalServiceType.Amputation => SurgicalProcedureType.Amputation,
			HospitalServiceType.Replantation => SurgicalProcedureType.Replantation,
			HospitalServiceType.SurgicalBoneSetting => SurgicalProcedureType.SurgicalBoneSetting,
			HospitalServiceType.Cannulation => SurgicalProcedureType.Cannulation,
			HospitalServiceType.Decannulation => SurgicalProcedureType.Decannulation,
			HospitalServiceType.InvasiveProcedureFinalisation => SurgicalProcedureType.InvasiveProcedureFinalisation,
			HospitalServiceType.InstallImplant => SurgicalProcedureType.InstallImplant,
			HospitalServiceType.RemoveImplant => SurgicalProcedureType.RemoveImplant,
			HospitalServiceType.ConfigureImplantPower => SurgicalProcedureType.ConfigureImplantPower,
			HospitalServiceType.ConfigureImplantInterface => SurgicalProcedureType.ConfigureImplantInterface,
			HospitalServiceType.InstallProsthetic => SurgicalProcedureType.InstallProsthetic,
			_ => null
		};
	}

	public static IReadOnlyCollection<TreatmentType> ImplicitTreatmentSupplyTypes(IHospitalService service)
	{
		return service.ServiceType switch
		{
			HospitalServiceType.Binding => [TreatmentType.Trauma],
			HospitalServiceType.WoundCleaning => [TreatmentType.Clean, TreatmentType.Antiseptic],
			HospitalServiceType.WoundClosing => [TreatmentType.Close],
			HospitalServiceType.WoundTending => [TreatmentType.Tend, TreatmentType.AntiInflammatory],
			HospitalServiceType.Stabilisation => [TreatmentType.Trauma, TreatmentType.Close],
			HospitalServiceType.FullTreatment =>
			[
				TreatmentType.Trauma,
				TreatmentType.Close,
				TreatmentType.Clean,
				TreatmentType.Antiseptic,
				TreatmentType.Tend,
				TreatmentType.AntiInflammatory
			],
			_ => []
		};
	}

	public static EmploymentActionStepResult ExecuteServiceRequest(IEmploymentTaskContext context, ICharacter employee,
		IHospital hospital, IHospitalServiceRequest request)
	{
		if (request.Status == HospitalServiceRequestStatus.Completed)
		{
			return CompletedStep(request, $"Hospital request #{request.Id.ToString("N0", CultureInfo.InvariantCulture)} has already completed.");
		}

		if (request.Status is HospitalServiceRequestStatus.Cancelled or HospitalServiceRequestStatus.Declined or HospitalServiceRequestStatus.Failed)
		{
			return EmploymentActionStepResult.Failed($"Hospital request #{request.Id.ToString("N0", CultureInfo.InvariantCulture)} is already {request.Status.DescribeEnum()}.");
		}

		if (request.Patient is not { } patient)
		{
			FailRequest(request, "The patient is no longer loaded or available for treatment.");
			return EmploymentActionStepResult.Failed("The hospital patient is no longer loaded or available for treatment.");
		}

		if (!employee.ColocatedWith(patient))
		{
			return EmploymentActionStepResult.Blocked("The assigned medical employee must be with the patient to perform the hospital service.");
		}

		if (request.Service.RequiredEquipment.Any() && !request.SupplyPrepared)
		{
			return EmploymentActionStepResult.Blocked("The required hospital supplies have not yet been prepared for this request.");
		}

		request.AssignedEmployeeId = CharacterInstanceIdentityComparer.IdentityId(employee);
		if (request.Status != HospitalServiceRequestStatus.InProgress)
		{
			request.MarkStatus(HospitalServiceRequestStatus.InProgress,
				$"{employee.PersonalName.GetName(MudSharp.Character.Name.NameStyle.FullName)} began treatment.");
		}

		var progress = HospitalTreatmentProgress.FromPayload(CurrentOperationalPayload(context), request);
		var result = request.Service.ServiceType switch
		{
			HospitalServiceType.Binding or HospitalServiceType.WoundCleaning or HospitalServiceType.WoundClosing or
				HospitalServiceType.WoundTending or HospitalServiceType.BoneRelocation =>
				PerformCommandRoutedWoundService(employee, patient, hospital, request, progress),
			HospitalServiceType.BoneSetting => TreatWorstBoneFracture(employee, patient, TreatmentType.SurgicalSet,
				CheckType.SurgicalSetCheck, "surgically set a fracture"),
			HospitalServiceType.SurgicalProcedure or HospitalServiceType.ImplantProcedure =>
				BeginSurgicalProcedure(context, employee, patient, hospital, request),
			HospitalServiceType.BloodDonation => PerformBloodDonation(context, employee, patient, request, progress),
			HospitalServiceType.BloodTransfusion => PerformBloodTransfusion(context, employee, patient, request, progress),
			HospitalServiceType.Stabilisation => PerformStabilisation(context, employee, patient, hospital, request, progress),
			HospitalServiceType.FullTreatment => PerformFullTreatment(context, employee, patient, hospital, request, progress),
			_ when ServiceTypeToSurgicalProcedureType(request.Service.ServiceType) is not null =>
				BeginSurgicalProcedure(context, employee, patient, hospital, request),
			_ => new ServiceExecutionResult(false, "Unsupported hospital service type.", string.Empty)
		};

		if (!result.Success)
		{
			FailRequest(request, result.Message);
			return EmploymentActionStepResult.Failed(result.Message);
		}

		if (!result.Completed)
		{
			context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, employee,
				$"Started staged hospital service request #{request.Id.ToString("N0", CultureInfo.InvariantCulture)}: {result.Message}",
				CurrentCorrelationId(context));
			return new EmploymentActionStepResult(true, result.Message, false,
				OperationalState(hospital, request, result));
		}

		if (!TryCompleteRequest(context, employee, hospital, request, result, out var completionMessage))
		{
			return EmploymentActionStepResult.Failed(completionMessage);
		}

		return new EmploymentActionStepResult(true, completionMessage, true, OperationalState(hospital, request, result));
	}

	public static bool CleanupBloodWorkflowForTerminalRequest(IEmploymentTaskContext context, ICharacter employee,
		IHospitalServiceRequest request)
	{
		var progress = HospitalTreatmentProgress.FromPayload(CurrentOperationalPayload(context), request);
		if (progress.BloodWorkflow is null || request.Patient is not { } patient)
		{
			return false;
		}

		NeutralizeBloodWorkflowGear(context, employee, patient, request, progress.BloodWorkflow);
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, employee,
			$"Cleaned up IV blood workflow gear for hospital request #{request.Id.ToString("N0", CultureInfo.InvariantCulture)} after terminal interruption.",
			CurrentCorrelationId(context));
		return true;
	}

	public static bool CleanupBloodWorkflowForTerminalRequest(IHospital hospital, IHospitalServiceRequest request,
		IEmploymentActiveTask? task, ICharacter employee)
	{
		if (task is not EmploymentActiveTask concrete)
		{
			return false;
		}

		var index = concrete.NextStepIndex;
		if (index < 0)
		{
			return false;
		}

		var context = new EmploymentTaskContext(hospital);
		context.HydrateTaskState(task, index);
		return CleanupBloodWorkflowForTerminalRequest(context, employee, request);
	}

	private static EmploymentActionStepResult CompletedStep(IHospitalServiceRequest request, string message)
	{
		return new EmploymentActionStepResult(true, message, true,
			new EmploymentActionStepOperationalState(
				SelectedResources: $"hospitalservice:request={request.Id.ToString("F0", CultureInfo.InvariantCulture)};patient={request.PatientId.ToString("F0", CultureInfo.InvariantCulture)};service={request.Service.Id.ToString("F0", CultureInfo.InvariantCulture)}",
				OperationalPayload: $"hospitalservice;hospital={request.Hospital.Id.ToString("F0", CultureInfo.InvariantCulture)};request={request.Id.ToString("F0", CultureInfo.InvariantCulture)};type={request.Service.ServiceType}"));
	}

	private static EmploymentActionStepOperationalState OperationalState(IHospital hospital,
		IHospitalServiceRequest request, ServiceExecutionResult result)
	{
		return new EmploymentActionStepOperationalState(
			SelectedResources: $"hospitalservice:request={request.Id.ToString("F0", CultureInfo.InvariantCulture)};patient={request.PatientId.ToString("F0", CultureInfo.InvariantCulture)};service={request.Service.Id.ToString("F0", CultureInfo.InvariantCulture)};resource={result.Resource}",
			OperationalPayload: result.Progress?.ToPayload(hospital, request, result.Completed) ??
			                    $"hospitalservice;hospital={hospital.Id.ToString("F0", CultureInfo.InvariantCulture)};request={request.Id.ToString("F0", CultureInfo.InvariantCulture)};type={request.Service.ServiceType};completed={result.Completed.ToString(CultureInfo.InvariantCulture)}");
	}

	private static bool TryCompleteRequest(IEmploymentTaskContext context, ICharacter employee, IHospital hospital,
		IHospitalServiceRequest request, ServiceExecutionResult result, out string completionMessage)
	{
		if (!TryApplyUsageBilling(context, employee, hospital, request, result, out var billingMessage))
		{
			FailRequest(request, billingMessage);
			context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, employee,
				$"Could not bill hospital service request #{request.Id.ToString("N0", CultureInfo.InvariantCulture)}: {billingMessage}",
				CurrentCorrelationId(context));
			completionMessage = billingMessage;
			return false;
		}

		completionMessage = string.IsNullOrWhiteSpace(billingMessage)
			? result.Message
			: $"{result.Message}\n{billingMessage}";
		request.MarkStatus(HospitalServiceRequestStatus.Completed, completionMessage);
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, employee,
			$"Completed hospital service request #{request.Id.ToString("N0", CultureInfo.InvariantCulture)}: {completionMessage}",
			CurrentCorrelationId(context));
		AnnounceRequestCompleted(employee, request, request.Service.Name);
		HospitalPatientFlow.TransferAfterTreatment(hospital, request, employee, "Hospital recovery routing");
		return true;
	}

	private static void AnnounceRequestCompleted(ICharacter employee, IHospitalServiceRequest request,
		string procedureName)
	{
		if (request.Patient is not { } patient)
		{
			return;
		}

		patient.Location?.HandleRoomEcho(new EmoteOutput(new Emote(
			$"@ finish|finishes {procedureName.ColourName()} for $0.", employee, patient)));
		patient.OutputHandler.Send(
			$"Your {procedureName.ColourName()} hospital procedure is complete.");
	}

	private static bool TryApplyUsageBilling(IEmploymentTaskContext context, ICharacter employee, IHospital hospital,
		IHospitalServiceRequest request, ServiceExecutionResult result, out string message)
	{
		message = string.Empty;
		if (!HospitalServiceBilling.IsUsageBilledServiceType(request.Service.ServiceType))
		{
			return true;
		}

		var charges = (result.UsageCharges ?? Array.Empty<UsageCharge>())
		              .Where(x => x.Count > 0)
		              .ToList();
		var total = charges.Sum(x => HospitalServiceBilling.UnitPriceForServiceType(hospital, x.ServiceType) * x.Count);
		request.MarkCharged(0.0M, 0.0M, total);
		if (total <= 0.0M)
		{
			message = "No usage-billed hospital charge was due.";
			return true;
		}

		if (request.PaymentMethod == HospitalPaymentMethod.Waived)
		{
			context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, employee,
				$"Waived {hospital.Currency.Describe(total, CurrencyDescriptionPatternType.ShortDecimal)} usage-billed hospital charge for request #{request.Id.ToString("N0", CultureInfo.InvariantCulture)}.",
				CurrentCorrelationId(context));
			message = $"Usage-billed hospital charge of {hospital.Currency.Describe(total, CurrencyDescriptionPatternType.ShortDecimal)} was waived.";
			return true;
		}

		if (!request.Service.AllowDebt)
		{
			message = $"{request.Service.Name} is usage-billed but does not allow hospital debt.";
			return false;
		}

		if (request.Patient is not { } patient)
		{
			message = "The patient is no longer available for usage-billed hospital charging.";
			return false;
		}

		var account = hospital.DebtAccountFor(patient, true)!;
		if (!account.CanCharge(total, out message))
		{
			message = $"The usage-billed hospital charge of {hospital.Currency.Describe(total, CurrencyDescriptionPatternType.ShortDecimal)} could not be charged: {message}";
			return false;
		}

		account.Charge(total,
			$"Usage-billed hospital service {request.Service.Name} for request #{request.Id.ToString("N0", CultureInfo.InvariantCulture)}");
		request.MarkCharged(0.0M, total, total);
		var details = charges.Select(x => HospitalServiceBilling.DescribeUsageLine(hospital, x.ServiceType, x.Count, employee)).ListToString();
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, employee,
			$"Charged {hospital.Currency.Describe(total, CurrencyDescriptionPatternType.ShortDecimal)} to {account.PatientName} for usage-billed hospital request #{request.Id.ToString("N0", CultureInfo.InvariantCulture)} ({details.StripANSIColour()}).",
			CurrentCorrelationId(context));
		message = $"Usage-billed hospital charge: {details}. Charged {hospital.Currency.Describe(total, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} to the patient's hospital debt account.";
		return true;
	}

	private static ServiceExecutionResult TreatWorstWound(ICharacter employee,
		ICharacter patient, TreatmentType treatment, CheckType checkType, Func<IWound, bool> filter, string description)
	{
		var wound = patient.VisibleWounds(employee, WoundExaminationType.Examination)
		                   .Where(filter)
		                   .Where(x => x.CanBeTreated(treatment) != Difficulty.Impossible)
		                   .OrderByDescending(x => x.Severity)
		                   .ThenBy(x => x.CanBeTreated(treatment))
		                   .FirstOrDefault();
		if (wound is null)
		{
			return new ServiceExecutionResult(false, $"{patient.HowSeen(employee, true)} has no visible wounds that can be {treatment.Describe().ToLowerInvariant()}.", string.Empty);
		}

		var treatmentItem = BestTreatmentItem(employee, treatment, wound.CanBeTreated(treatment));
		var outcome = employee.Gameworld.GetCheck(checkType).Check(employee, wound.CanBeTreated(treatment), patient);
		wound.Treat(employee, treatment, treatmentItem, outcome.Outcome, false);
		return new ServiceExecutionResult(true,
			$"{employee.HowSeen(employee, true)} {description} for {patient.HowSeen(employee)} with {outcome.Outcome.DescribeEnum()} outcome.",
			wound.Id.ToString("F0", CultureInfo.InvariantCulture));
	}

	private static ServiceExecutionResult CleanWorstWound(ICharacter employee, ICharacter patient)
	{
		var wounds = patient.VisibleWounds(employee, WoundExaminationType.Examination)
		                    .Where(x => x.CanBeTreated(TreatmentType.Clean) != Difficulty.Impossible ||
		                                x.CanBeTreated(TreatmentType.Antiseptic) != Difficulty.Impossible)
		                    .ToList();
		if (!wounds.Any())
		{
			return new ServiceExecutionResult(false, $"{patient.HowSeen(employee, true)} has no visible wounds that can benefit from cleaning.", string.Empty);
		}

		var antiseptic = wounds
		                .Where(x => x.CanBeTreated(TreatmentType.Antiseptic) != Difficulty.Impossible)
		                .OrderByDescending(x => x.Severity)
		                .ThenBy(x => x.CanBeTreated(TreatmentType.Antiseptic))
		                .FirstOrDefault();
		if (antiseptic is not null)
		{
			var treatmentItem = BestTreatmentItem(employee, TreatmentType.Antiseptic,
				antiseptic.CanBeTreated(TreatmentType.Antiseptic));
			if (treatmentItem is not null)
			{
				var outcome = employee.Gameworld.GetCheck(CheckType.CleanWoundCheck)
				                      .Check(employee, antiseptic.CanBeTreated(TreatmentType.Antiseptic), patient);
				antiseptic.Treat(employee, TreatmentType.Antiseptic, treatmentItem, outcome.Outcome, false);
				return new ServiceExecutionResult(true,
					$"{employee.HowSeen(employee, true)} treated a visible wound on {patient.HowSeen(employee)} with antiseptic care and {outcome.Outcome.DescribeEnum()} outcome.",
					antiseptic.Id.ToString("F0", CultureInfo.InvariantCulture));
			}
		}

		var wound = wounds
		           .Where(x => x.CanBeTreated(TreatmentType.Clean) != Difficulty.Impossible)
		           .OrderByDescending(x => x.Severity)
		           .ThenBy(x => x.CanBeTreated(TreatmentType.Clean))
		           .FirstOrDefault();
		if (wound is null)
		{
			return new ServiceExecutionResult(false, $"{patient.HowSeen(employee, true)} needs antiseptic treatment, but the employee has no antiseptic supplies.", string.Empty);
		}

		var cleanOutcome = employee.Gameworld.GetCheck(CheckType.CleanWoundCheck)
		                           .Check(employee, wound.CanBeTreated(TreatmentType.Clean), patient);
		wound.Treat(employee, TreatmentType.Clean, BestTreatmentItem(employee, TreatmentType.Clean,
			wound.CanBeTreated(TreatmentType.Clean)), cleanOutcome.Outcome, false);
		return new ServiceExecutionResult(true,
			$"{employee.HowSeen(employee, true)} cleaned a visible wound on {patient.HowSeen(employee)} with {cleanOutcome.Outcome.DescribeEnum()} outcome.",
			wound.Id.ToString("F0", CultureInfo.InvariantCulture));
	}

	private static ServiceExecutionResult TendWorstWound(ICharacter employee, ICharacter patient)
	{
		var wound = patient.VisibleWounds(employee, WoundExaminationType.Examination)
		                   .Where(x => x.CanBeTreated(TreatmentType.Tend) != Difficulty.Impossible)
		                   .OrderByDescending(x => x.Severity)
		                   .ThenBy(x => x.CanBeTreated(TreatmentType.Tend))
		                   .FirstOrDefault();
		if (wound is not null)
		{
			return TreatWorstWound(employee, patient, TreatmentType.Tend, CheckType.TendWoundCheck,
				x => x.Id == wound.Id, "tended a wound");
		}

		return TreatWorstWound(employee, patient, TreatmentType.AntiInflammatory, CheckType.CleanWoundCheck,
			x => true, "provided anti-inflammatory wound care");
	}

	private static ServiceExecutionResult TreatWorstBoneFracture(ICharacter employee,
		ICharacter patient, TreatmentType treatment, CheckType checkType, string description)
	{
		var wound = patient.Wounds
		                   .OfType<BoneFracture>()
		                   .Where(x => x.CanBeTreated(treatment) != Difficulty.Impossible)
		                   .OrderByDescending(x => x.Severity)
		                   .ThenBy(x => x.CanBeTreated(treatment))
		                   .FirstOrDefault();
		if (wound is null)
		{
			return new ServiceExecutionResult(false, $"{patient.HowSeen(employee, true)} has no fractures that can be {treatment.Describe().ToLowerInvariant()}.", string.Empty);
		}

		var outcome = employee.Gameworld.GetCheck(checkType).Check(employee, wound.CanBeTreated(treatment), patient);
		wound.Treat(employee, treatment, null, outcome.Outcome, false);
		return new ServiceExecutionResult(true,
			$"{employee.HowSeen(employee, true)} {description} for {patient.HowSeen(employee)} with {outcome.Outcome.DescribeEnum()} outcome.",
			wound.Id.ToString("F0", CultureInfo.InvariantCulture));
	}

	private static ServiceExecutionResult PerformCommandRoutedWoundService(ICharacter employee, ICharacter patient,
		IHospital hospital, IHospitalServiceRequest request, HospitalTreatmentProgress progress)
	{
		if (CompleteFinishedCommandPhase(employee, patient, progress))
		{
			return CompleteCommandRoutedService(employee, patient, hospital, request, progress);
		}

		return request.Service.ServiceType switch
		{
			HospitalServiceType.Binding => TryStartTreatmentPhase(employee, patient, hospital, request, progress,
				"bind", HospitalServiceType.Binding),
			HospitalServiceType.WoundCleaning => TryStartTreatmentPhase(employee, patient, hospital, request, progress,
				"clean", HospitalServiceType.WoundCleaning),
			HospitalServiceType.WoundClosing => TryStartTreatmentPhase(employee, patient, hospital, request, progress,
				"suture", HospitalServiceType.WoundClosing),
			HospitalServiceType.WoundTending => TryStartTreatmentPhase(employee, patient, hospital, request, progress,
				"tend", HospitalServiceType.WoundTending),
			HospitalServiceType.BoneRelocation => TryStartTreatmentPhase(employee, patient, hospital, request, progress,
				"relocate", HospitalServiceType.BoneRelocation),
			_ => new ServiceExecutionResult(false, "Unsupported command-routed hospital wound service.", string.Empty,
				Progress: progress)
		};
	}

	private static ServiceExecutionResult PerformStabilisation(IEmploymentTaskContext context, ICharacter employee,
		ICharacter patient, IHospital hospital, IHospitalServiceRequest request, HospitalTreatmentProgress progress)
	{
		if (CompleteFinishedCommandPhase(employee, patient, progress))
		{
			return ContinueStabilisation(context, employee, patient, hospital, request, progress);
		}

		return ContinueStabilisation(context, employee, patient, hospital, request, progress);
	}

	private static ServiceExecutionResult ContinueStabilisation(IEmploymentTaskContext context, ICharacter employee,
		ICharacter patient, IHospital hospital, IHospitalServiceRequest request, HospitalTreatmentProgress progress)
	{
		if (!progress.CompletedPhases.Contains("bind") && HasBindableWounds(employee, patient))
		{
			return TryStartTreatmentPhase(employee, patient, hospital, request, progress, "bind",
				HospitalServiceType.Binding);
		}

		if (!progress.CompletedPhases.Contains("suture") && HasSuturableWounds(employee, patient))
		{
			return TryStartTreatmentPhase(employee, patient, hospital, request, progress, "suture",
				HospitalServiceType.WoundClosing);
		}

		if (!progress.CompletedPhases.Contains("transfusion") &&
		    patient.Body.TotalBloodVolumeLitres > 0.0 &&
		    patient.Body.CurrentBloodVolumeLitres < patient.Body.TotalBloodVolumeLitres * 0.75)
		{
			var transfusion = PerformBloodTransfusion(context, employee, patient, request, progress, true);
			if (!transfusion.Success)
			{
				return progress.CompletedPhases.Any()
					? CompleteCommandRoutedService(employee, patient, hospital, request, progress)
					: transfusion with { Progress = progress };
			}

			if (!transfusion.Completed)
			{
				return transfusion;
			}

			progress.CompletedPhases.Add("transfusion");
			progress.AddCharge(HospitalServiceType.BloodTransfusion);
		}

		if (!progress.CompletedPhases.Any())
		{
			return new ServiceExecutionResult(false,
				$"{patient.HowSeen(employee, true)} has no visible immediately life-threatening wounds or blood loss that this service can stabilise.",
				string.Empty,
				Progress: progress);
		}

		return CompleteCommandRoutedService(employee, patient, hospital, request, progress);
	}

	private static ServiceExecutionResult PerformFullTreatment(IEmploymentTaskContext context, ICharacter employee,
		ICharacter patient, IHospital hospital, IHospitalServiceRequest request, HospitalTreatmentProgress progress)
	{
		if (CompleteFinishedCommandPhase(employee, patient, progress))
		{
			return ContinueFullTreatment(context, employee, patient, hospital, request, progress);
		}

		return ContinueFullTreatment(context, employee, patient, hospital, request, progress);
	}

	private static ServiceExecutionResult ContinueFullTreatment(IEmploymentTaskContext context, ICharacter employee,
		ICharacter patient, IHospital hospital, IHospitalServiceRequest request, HospitalTreatmentProgress progress)
	{
		if (!progress.CompletedPhases.Contains("bind") && HasBindableWounds(employee, patient))
		{
			return TryStartTreatmentPhase(employee, patient, hospital, request, progress, "bind",
				HospitalServiceType.Binding);
		}

		if (!progress.CompletedPhases.Contains("suture") && HasSuturableWounds(employee, patient))
		{
			return TryStartTreatmentPhase(employee, patient, hospital, request, progress, "suture",
				HospitalServiceType.WoundClosing);
		}

		if (!progress.CompletedPhases.Contains("clean") && HasCleanableWounds(employee, patient))
		{
			return TryStartTreatmentPhase(employee, patient, hospital, request, progress, "clean",
				HospitalServiceType.WoundCleaning);
		}

		if (!progress.CompletedPhases.Contains("tend") && HasTendableWounds(employee, patient))
		{
			return TryStartTreatmentPhase(employee, patient, hospital, request, progress, "tend",
				HospitalServiceType.WoundTending);
		}

		if (!progress.CompletedPhases.Contains("relocate") && HasRelocatableFractures(patient))
		{
			return TryStartTreatmentPhase(employee, patient, hospital, request, progress, "relocate",
				HospitalServiceType.BoneRelocation);
		}

		if (!progress.CompletedPhases.Contains("transfusion") &&
		    patient.Body.TotalBloodVolumeLitres > 0.0 &&
		    patient.Body.CurrentBloodVolumeLitres < patient.Body.TotalBloodVolumeLitres)
		{
			var transfusion = PerformBloodTransfusion(context, employee, patient, request, progress, true);
			if (transfusion.Success)
			{
				if (!transfusion.Completed)
				{
					return transfusion;
				}

				progress.CompletedPhases.Add("transfusion");
				progress.AddCharge(HospitalServiceType.BloodTransfusion);
			}
		}

		if (!progress.CompletedPhases.Any())
		{
			return new ServiceExecutionResult(false,
				$"{patient.HowSeen(employee, true)} has no visible wounds, fractures or blood loss that this service can treat.",
				string.Empty,
				Progress: progress);
		}

		return CompleteCommandRoutedService(employee, patient, hospital, request, progress);
	}

	private static ServiceExecutionResult TryStartTreatmentPhase(ICharacter employee, ICharacter patient,
		IHospital hospital, IHospitalServiceRequest request, HospitalTreatmentProgress progress, string phase,
		HospitalServiceType usageServiceType)
	{
		if (!string.IsNullOrWhiteSpace(progress.ActivePhase))
		{
			if (HasActiveCommandPhase(employee, patient, progress.ActivePhase))
			{
				return new ServiceExecutionResult(true,
					$"{employee.HowSeen(employee, true)} is still performing {DescribePhase(progress.ActivePhase)} for {patient.HowSeen(employee)}.",
					progress.ActivePhase, false, Progress: progress);
			}

			progress.CompleteActivePhase(UsageServiceTypeForPhase(progress.ActivePhase));
		}

		var expectedCount = ExpectedTreatmentCount(employee, patient, phase);
		if (expectedCount <= 0)
		{
			return new ServiceExecutionResult(false, NoValidTreatmentMessage(employee, patient, usageServiceType),
				string.Empty, Progress: progress);
		}

		if (!TryBuildTreatmentCommand(employee, patient, phase, out var command, out var reason))
		{
			return new ServiceExecutionResult(false, reason, string.Empty, Progress: progress);
		}

		EnsureHospitalTreatmentPermission(patient, employee, request);
		var existingEffects = ActivePhaseEffects(employee, patient, phase).ToHashSet();
		if (!employee.CommandTree.Commands.Execute(employee, command, employee.State, employee.PermissionLevel,
			    employee.OutputHandler))
		{
			return new ServiceExecutionResult(false,
				$"{employee.HowSeen(employee, true)} could not start {DescribePhase(phase)} for {patient.HowSeen(employee)}.",
				string.Empty,
				Progress: progress);
		}

		if (!ActivePhaseEffects(employee, patient, phase).Any(x => !existingEffects.Contains(x)))
		{
			return new ServiceExecutionResult(false,
				$"{employee.HowSeen(employee, true)} could not start {DescribePhase(phase)} for {patient.HowSeen(employee)}.",
				string.Empty,
				Progress: progress);
		}

		progress.ActivePhase = phase;
		progress.ActiveExpectedCount = expectedCount;
		return new ServiceExecutionResult(true,
			$"{employee.HowSeen(employee, true)} began {DescribePhase(phase)} for {patient.HowSeen(employee)}.",
			phase,
			false,
			Progress: progress);
	}

	private static ServiceExecutionResult CompleteCommandRoutedService(ICharacter employee, ICharacter patient,
		IHospital hospital, IHospitalServiceRequest request, HospitalTreatmentProgress progress)
	{
		var summaries = progress.Charges
		                        .Where(x => x.Value > 0)
		                        .Select(x => HospitalServiceBilling.DescribeUsageLine(hospital, x.Key, x.Value, employee).StripANSIColour())
		                        .ToList();
		var summary = summaries.Any()
			? summaries.ListToString()
			: progress.CompletedPhases.Select(DescribePhase).ListToString();
		return new ServiceExecutionResult(true,
			$"{employee.HowSeen(employee, true)} completed {request.Service.Name.ColourName()} for {patient.HowSeen(employee)}: {summary}.",
			string.Join(';', progress.CompletedPhases),
			UsageCharges: progress.UsageCharges,
			Progress: progress);
	}

	private static bool CompleteFinishedCommandPhase(ICharacter employee, ICharacter patient,
		HospitalTreatmentProgress progress)
	{
		if (string.IsNullOrWhiteSpace(progress.ActivePhase))
		{
			return false;
		}

		if (HasActiveCommandPhase(employee, patient, progress.ActivePhase))
		{
			return false;
		}

		progress.CompleteActivePhase(UsageServiceTypeForPhase(progress.ActivePhase));
		return true;
	}

	private static bool HasActiveCommandPhase(ICharacter employee, ICharacter patient, string phase)
	{
		return ActivePhaseEffects(employee, patient, phase).Any();
	}

	private static IEnumerable<Effect> ActivePhaseEffects(ICharacter employee, ICharacter patient, string phase)
	{
		return phase.ToLowerInvariant() switch
		{
			"bind" => employee.EffectsOfType<Binding>()
			                  .Where(x => SameHospitalPatient(x.TargetCharacter, patient))
			                  .Cast<Effect>(),
			"suture" => employee.EffectsOfType<Suturing>()
			                    .Where(x => SameHospitalPatient(x.TargetCharacter, patient))
			                    .Cast<Effect>(),
			"clean" => employee.EffectsOfType<CleaningWounds>()
			                  .Where(x => SameHospitalPatient(x.TargetCharacter, patient))
			                  .Cast<Effect>(),
			"tend" => employee.EffectsOfType<TendingWounds>()
			                 .Where(x => SameHospitalPatient(x.TargetCharacter, patient))
			                 .Cast<Effect>(),
			"relocate" => employee.EffectsOfType<RelocatingBone>()
			                     .Where(x => SameHospitalPatient(x.TargetCharacter, patient))
			                     .Cast<Effect>(),
			_ => []
		};
	}

	private static void EnsureHospitalTreatmentPermission(ICharacter patient, ICharacter employee,
		IHospitalServiceRequest request)
	{
		if (patient.EffectsOfType<HospitalTreatmentPermissionEffect>()
		           .Any(x => CharacterInstanceIdentityComparer.SamePhysicalInstance(x.Medic, employee) &&
		                     x.RequestId == request.Id))
		{
			return;
		}

		patient.AddEffect(new HospitalTreatmentPermissionEffect(patient, employee, request), TimeSpan.FromMinutes(30));
	}

	private static bool TryBuildTreatmentCommand(ICharacter employee, ICharacter patient, string phase,
		out string command, out string reason)
	{
		command = string.Empty;
		reason = string.Empty;
		if (!TryPatientCommandSelector(employee, patient, out var selector))
		{
			reason = $"{patient.HowSeen(employee, true)} is not visible to {employee.HowSeen(employee)} for treatment.";
			return false;
		}

		command = phase.ToLowerInvariant() switch
		{
			"bind" => $"bind {selector}",
			"suture" => $"suture {selector}",
			"clean" => $"cleanwounds {selector}",
			"tend" => $"tend {selector}",
			"relocate" when RelocationBodypartToken(patient) is { } bodypart => $"relocate {selector} \"{bodypart}\"",
			"relocate" => string.Empty,
			_ => string.Empty
		};

		if (!string.IsNullOrWhiteSpace(command))
		{
			return true;
		}

		reason = phase.EqualTo("relocate")
			? $"{patient.HowSeen(employee, true)} has no visible fracture that can be relocated."
			: $"Unsupported hospital treatment phase {phase}.";
		return false;
	}

	private static bool TryPatientCommandSelector(ICharacter employee, ICharacter patient, out string selector)
	{
		selector = "me";
		if (CharacterInstanceIdentityComparer.SamePhysicalInstance(employee, patient))
		{
			return true;
		}

		var candidates = employee.Location?.LayerCharacters(employee.RoomLayer)
		                         .Except(employee)
		                         .Where(x => employee.CanSee(x))
		                         .ToList() ?? [];
		var index = candidates.FindIndex(x => SameHospitalPatient(x, patient));
		if (index < 0)
		{
			return false;
		}

		selector = $"#{(index + 1).ToString("N0", CultureInfo.InvariantCulture)}";
		return true;
	}

	private static bool SameHospitalPatient(ICharacter? candidate, ICharacter patient)
	{
		return candidate is not null &&
		       (CharacterInstanceIdentityComparer.SamePhysicalInstance(candidate, patient) ||
		        CharacterInstanceIdentityComparer.SameIdentity(candidate, patient));
	}

	private static string? RelocationBodypartToken(ICharacter patient)
	{
		return patient.Wounds
		              .OfType<BoneFracture>()
		              .Where(x => x.CanBeTreated(TreatmentType.Relocation) != Difficulty.Impossible)
		              .OrderByDescending(x => x.Severity)
		              .ThenBy(x => x.CanBeTreated(TreatmentType.Relocation))
		              .Select(x => x.Bone.Name)
		              .FirstOrDefault();
	}

	private static HospitalServiceType UsageServiceTypeForPhase(string? phase)
	{
		return phase?.ToLowerInvariant() switch
		{
			"bind" => HospitalServiceType.Binding,
			"suture" => HospitalServiceType.WoundClosing,
			"clean" => HospitalServiceType.WoundCleaning,
			"tend" => HospitalServiceType.WoundTending,
			"relocate" => HospitalServiceType.BoneRelocation,
			"transfusion" => HospitalServiceType.BloodTransfusion,
			_ => HospitalServiceType.FullTreatment
		};
	}

	private static int ExpectedTreatmentCount(ICharacter employee, ICharacter patient, string phase)
	{
		return phase.ToLowerInvariant() switch
		{
			"bind" => patient.VisibleWounds(employee, WoundExaminationType.Examination)
			                 .Count(x => x.BleedStatus == BleedStatus.Bleeding &&
			                             x.CanBeTreated(TreatmentType.Trauma) != Difficulty.Impossible),
			"suture" => patient.VisibleWounds(employee, WoundExaminationType.Examination)
			                   .Count(x => x.BleedStatus == BleedStatus.TraumaControlled &&
			                               x.CanBeTreated(TreatmentType.Close) != Difficulty.Impossible),
			"clean" => patient.VisibleWounds(employee, WoundExaminationType.Examination)
			                 .Sum(x => (x.CanBeTreated(TreatmentType.Clean) != Difficulty.Impossible ? 1 : 0) +
			                           (x.CanBeTreated(TreatmentType.Antiseptic) != Difficulty.Impossible ? 1 : 0)),
			"tend" => patient.VisibleWounds(employee, WoundExaminationType.Examination)
			                .Sum(x => (x.CanBeTreated(TreatmentType.Tend) != Difficulty.Impossible ? 1 : 0) +
			                          (x.CanBeTreated(TreatmentType.AntiInflammatory) != Difficulty.Impossible ? 1 : 0)),
			"relocate" => HasRelocatableFractures(patient) ? 1 : 0,
			_ => 0
		};
	}

	private static bool HasBindableWounds(ICharacter employee, ICharacter patient)
	{
		return ExpectedTreatmentCount(employee, patient, "bind") > 0;
	}

	private static bool HasSuturableWounds(ICharacter employee, ICharacter patient)
	{
		return ExpectedTreatmentCount(employee, patient, "suture") > 0;
	}

	private static bool HasCleanableWounds(ICharacter employee, ICharacter patient)
	{
		return CleaningWounds.PeekCanClean(employee, patient, WoundSeverity.None, true).Success ||
		       CleaningWounds.PeekCanClean(employee, patient, WoundSeverity.None, false).Success;
	}

	private static bool HasTendableWounds(ICharacter employee, ICharacter patient)
	{
		return ExpectedTreatmentCount(employee, patient, "tend") > 0;
	}

	private static bool HasRelocatableFractures(ICharacter patient)
	{
		return patient.Wounds
		              .OfType<BoneFracture>()
		              .Any(x => x.CanBeTreated(TreatmentType.Relocation) != Difficulty.Impossible);
	}

	private static string DescribePhase(string? phase)
	{
		return phase?.ToLowerInvariant() switch
		{
			"bind" => "binding bleeding wounds",
			"suture" => "suturing traumatic wounds",
			"clean" => "cleaning wounds",
			"tend" => "tending wounds",
			"relocate" => "relocating fractures",
			"transfusion" => "blood transfusion",
			_ => "hospital treatment"
		};
	}

	private static string NoValidTreatmentMessage(ICharacter employee, ICharacter patient, HospitalServiceType serviceType)
	{
		return serviceType switch
		{
			HospitalServiceType.Binding =>
				$"{patient.HowSeen(employee, true)} has no visible bleeding wounds that can be bound.",
			HospitalServiceType.WoundClosing =>
				$"{patient.HowSeen(employee, true)} has no visible traumatic wounds that can be sutured.",
			HospitalServiceType.WoundCleaning =>
				$"{patient.HowSeen(employee, true)} has no visible wounds that can benefit from cleaning.",
			HospitalServiceType.WoundTending =>
				$"{patient.HowSeen(employee, true)} has no visible wounds that can benefit from tending or anti-inflammatory care.",
			HospitalServiceType.BoneRelocation =>
				$"{patient.HowSeen(employee, true)} has no visible fractures that can be relocated.",
			_ => $"{patient.HowSeen(employee, true)} has no visible injuries that this hospital service can treat."
		};
	}

	private static ServiceExecutionResult BeginSurgicalProcedure(IEmploymentTaskContext context, ICharacter employee,
		ICharacter patient, IHospital hospital, IHospitalServiceRequest request)
	{
		var service = request.Service;
		var procedure = service.SurgicalProcedure ?? ResolveSurgicalProcedureForService(employee, patient, request);
		if (procedure is null)
		{
			return new ServiceExecutionResult(false, SurgicalResolverFailureMessage(service), string.Empty);
		}

		return BeginConfiguredSurgicalProcedure(context, employee, patient, hospital, request, procedure);
	}

	private static string SurgicalResolverFailureMessage(IHospitalService service)
	{
		return ServiceTypeToSurgicalProcedureType(service.ServiceType) is { } procedureType
			? $"Hospital service {service.Name} has no available {procedureType.DescribeEnum()} procedure for this patient and doctor."
			: $"Hospital service {service.Name} has no surgical procedure configured.";
	}

	private static ISurgicalProcedure? ResolveSurgicalProcedureForService(ICharacter employee, ICharacter patient,
		IHospitalServiceRequest request)
	{
		return ResolveSurgicalProcedureForService(employee, patient, request.Service, request.ProcedureParameters);
	}

	private static ISurgicalProcedure? ResolveSurgicalProcedureForService(ICharacter employee, ICharacter patient,
		IHospitalService service, string? procedureParameters)
	{
		if (ServiceTypeToSurgicalProcedureType(service.ServiceType) is not { } procedureType)
		{
			return null;
		}

		return employee.Gameworld.SurgicalProcedures
		               .Where(x => x.Procedure == procedureType)
		               .Where(x => x.TargetBodyType is null || patient.Body.Prototype.CountsAs(x.TargetBodyType))
		               .OrderBy(x => x.Name)
		               .FirstOrDefault(x =>
		               {
			               if (service.ImplantItemPrototype is not null)
			               {
				               return true;
			               }

			               var args = ServiceProcedureArguments(employee, patient, service, procedureParameters, x, false)
				               .ToArray();
			               return x.CanPerformProcedure(employee, patient, args);
		               });
	}

	private static ServiceExecutionResult BeginConfiguredSurgicalProcedure(IEmploymentTaskContext context,
		ICharacter employee, ICharacter patient, IHospital hospital, IHospitalServiceRequest request,
		ISurgicalProcedure procedure)
	{
		var service = request.Service;
		if (procedure.RequiresUnconsciousPatient && !patient.IsHelpless)
		{
			var anesthesia = PrepareSurgicalAnesthesia(context, employee, patient, hospital, service, request,
				out var anesthesiaMessage);
			switch (anesthesia)
			{
				case AnesthesiaPreparationResult.StartedCannulation:
					return new ServiceExecutionResult(true, anesthesiaMessage,
						service.AnesthesiaCannulationProcedure?.Id.ToString("F0", CultureInfo.InvariantCulture) ?? string.Empty,
						false);
				case AnesthesiaPreparationResult.Failed:
					return new ServiceExecutionResult(false, anesthesiaMessage, string.Empty);
			}
		}

		var args = ServiceProcedureArguments(employee, patient, request, procedure, true).ToArray();
		var suppliedImplant = ServiceSuppliedImplant(employee, service, args);
		if (!TryBeginTrackedProcedure(context, employee, patient, hospital, request, procedure, args, suppliedImplant,
			out var reason))
		{
			return new ServiceExecutionResult(false, reason, string.Empty);
		}

		return new ServiceExecutionResult(true,
			$"{employee.HowSeen(employee, true)} began {procedure.ProcedureName.ColourName()} for {patient.HowSeen(employee)}.",
			procedure.Id.ToString("F0", CultureInfo.InvariantCulture), false);
	}

	private static void ResolveStagedProcedure(IEmploymentTaskContext context, ICharacter employee, IHospital hospital,
		IHospitalServiceRequest request, ISurgicalProcedure procedure, bool completed, CheckOutcome outcome,
		IGameItem? suppliedImplant)
	{
		var progress = HospitalTreatmentProgress.FromPayload(CurrentOperationalPayload(context), request);
		if (progress.BloodWorkflow is not null &&
		    procedure.Procedure is SurgicalProcedureType.Cannulation or SurgicalProcedureType.Decannulation)
		{
			if (completed)
			{
				context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, employee,
					$"Continued staged hospital service request #{request.Id.ToString("N0", CultureInfo.InvariantCulture)}: {procedure.ProcedureName} completed with {outcome.Outcome.DescribeEnum()} outcome for the IV blood workflow.",
					CurrentCorrelationId(context));
				return;
			}

			if (request.Patient is { } patient)
			{
				NeutralizeBloodWorkflowGear(context, employee, patient, request, progress.BloodWorkflow);
			}

			request.MarkStatus(HospitalServiceRequestStatus.Failed,
				$"{procedure.ProcedureName} aborted with {outcome.Outcome.DescribeEnum()} outcome during IV blood workflow.");
			context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, employee,
				$"Failed staged hospital service request #{request.Id.ToString("N0", CultureInfo.InvariantCulture)}: {procedure.ProcedureName} aborted with {outcome.Outcome.DescribeEnum()} outcome during IV blood workflow.",
				CurrentCorrelationId(context));
			return;
		}

		if (completed)
		{
			if (request.Service.AnesthesiaCannulationProcedure?.Id == procedure.Id)
			{
				ContinueAfterAnesthesiaCannulation(context, employee, hospital, request, procedure, outcome);
				return;
			}

			switch (TryBeginNextImplantPipelineStep(context, employee, hospital, request, procedure, suppliedImplant,
				out var pipelineMessage))
			{
				case ImplantPipelineResolution.Started:
					context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, employee,
						$"Continued staged hospital service request #{request.Id.ToString("N0", CultureInfo.InvariantCulture)}: {pipelineMessage}",
						CurrentCorrelationId(context));
					return;
				case ImplantPipelineResolution.Failed:
					request.MarkStatus(HospitalServiceRequestStatus.Failed, pipelineMessage);
					context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, employee,
						$"Failed staged hospital service request #{request.Id.ToString("N0", CultureInfo.InvariantCulture)}: {pipelineMessage}",
						CurrentCorrelationId(context));
					return;
			}

			request.MarkStatus(HospitalServiceRequestStatus.Completed,
				$"{procedure.ProcedureName} completed with {outcome.Outcome.DescribeEnum()} outcome.");
			context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, employee,
				$"Completed staged hospital service request #{request.Id.ToString("N0", CultureInfo.InvariantCulture)}: {procedure.ProcedureName} completed with {outcome.Outcome.DescribeEnum()} outcome.",
				CurrentCorrelationId(context));
			AnnounceRequestCompleted(employee, request, procedure.ProcedureName);
			HospitalPatientFlow.TransferAfterTreatment(hospital, request, employee, "Hospital recovery routing");
			return;
		}

		request.MarkStatus(HospitalServiceRequestStatus.Failed,
			$"{procedure.ProcedureName} aborted with {outcome.Outcome.DescribeEnum()} outcome.");
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, employee,
			$"Failed staged hospital service request #{request.Id.ToString("N0", CultureInfo.InvariantCulture)}: {procedure.ProcedureName} aborted with {outcome.Outcome.DescribeEnum()} outcome.",
			CurrentCorrelationId(context));
	}

	private static void ContinueAfterAnesthesiaCannulation(IEmploymentTaskContext context, ICharacter employee,
		IHospital hospital, IHospitalServiceRequest request, ISurgicalProcedure completedProcedure, CheckOutcome outcome)
	{
		if (request.Patient is not { } patient)
		{
			const string message = "The anesthesia patient is no longer loaded or available for surgery.";
			request.MarkStatus(HospitalServiceRequestStatus.Failed, message);
			context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, employee,
				$"Failed staged hospital service request #{request.Id.ToString("N0", CultureInfo.InvariantCulture)}: {message}",
				CurrentCorrelationId(context));
			return;
		}

		var mainProcedure = request.Service.SurgicalProcedure ?? ResolveSurgicalProcedureForService(employee, patient, request);
		if (mainProcedure is null)
		{
			var message = $"{SurgicalResolverFailureMessage(request.Service)} after {completedProcedure.ProcedureName}.";
			request.MarkStatus(HospitalServiceRequestStatus.Failed, message);
			context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, employee,
				$"Failed staged hospital service request #{request.Id.ToString("N0", CultureInfo.InvariantCulture)}: {message}",
				CurrentCorrelationId(context));
			return;
		}

		var result = BeginConfiguredSurgicalProcedure(context, employee, patient, hospital, request, mainProcedure);
		if (!result.Success)
		{
			request.MarkStatus(HospitalServiceRequestStatus.Failed, result.Message);
			context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, employee,
				$"Failed staged hospital service request #{request.Id.ToString("N0", CultureInfo.InvariantCulture)} after {completedProcedure.ProcedureName}: {result.Message}",
				CurrentCorrelationId(context));
			return;
		}

		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, employee,
			$"Continued staged hospital service request #{request.Id.ToString("N0", CultureInfo.InvariantCulture)} after {completedProcedure.ProcedureName} completed with {outcome.Outcome.DescribeEnum()} outcome: {result.Message}",
			CurrentCorrelationId(context));
	}

	private enum ImplantPipelineResolution
	{
		NoNextStep,
		Started,
		Failed
	}

	private static AnesthesiaPreparationResult PrepareSurgicalAnesthesia(IEmploymentTaskContext context,
		ICharacter employee, ICharacter patient, IHospital hospital, IHospitalService service,
		IHospitalServiceRequest request, out string message)
	{
		message = string.Empty;
		if (service.AnesthesiaDrug is not { } drug)
		{
			message = $"{service.Name} requires the patient to be helpless or unconscious, but no anesthesia drug is configured.";
			return AnesthesiaPreparationResult.Failed;
		}

		if (!drug.DrugTypes.Contains(DrugType.Anesthesia) || !drug.DrugVectors.HasFlag(DrugVector.Injected))
		{
			message = $"{drug.Name} is not an injectable anesthesia drug.";
			return AnesthesiaPreparationResult.Failed;
		}

		if (service.AnesthesiaCannulationProcedure is { } cannulationProcedure)
		{
			if (cannulationProcedure.Procedure != SurgicalProcedureType.Cannulation)
			{
				message = $"{cannulationProcedure.ProcedureName} is not a cannulation procedure.";
				return AnesthesiaPreparationResult.Failed;
			}

			if (PatientCannula(patient) is null)
			{
				return TryBeginAnesthesiaCannulation(context, employee, patient, hospital, request, cannulationProcedure,
					out message)
					? AnesthesiaPreparationResult.StartedCannulation
					: AnesthesiaPreparationResult.Failed;
			}

			return PrepareAnesthesiaViaDrip(context, employee, patient, request, drug, out message)
				? AnesthesiaPreparationResult.Ready
				: AnesthesiaPreparationResult.Failed;
		}

		return PrepareDirectInjectedAnesthesia(employee, patient, service, request, drug, out message)
			? AnesthesiaPreparationResult.Ready
			: AnesthesiaPreparationResult.Failed;
	}

	private static bool PrepareDirectInjectedAnesthesia(ICharacter employee, ICharacter patient, IHospitalService service,
		IHospitalServiceRequest request, IDrug drug, out string message)
	{
		if (!TryCalculateAnesthesiaDose(employee, patient, service, drug, out var grams, out message))
		{
			return false;
		}

		if (grams > 0.0)
		{
			patient.Body.DoseImmediate(drug, DrugVector.Injected, grams, request);
			employee.OutputHandler.Send(new EmoteOutput(new Emote(
				"@ administer|administers a measured anesthetic dose to $0 before surgery.", employee, patient)));
		}

		return ValidateAnesthesiaOutcome(employee, patient, out message);
	}

	private static bool TryBeginAnesthesiaCannulation(IEmploymentTaskContext context, ICharacter employee,
		ICharacter patient, IHospital hospital, IHospitalServiceRequest request, ISurgicalProcedure cannulationProcedure,
		out string message)
	{
		var cannulaItem = CandidateHospitalItems(context, employee, request)
		                  .FirstOrDefault(x => x.GetItemType<ICannula>() is { } cannula &&
		                                       patient.Body.Prototype.CountsAs(cannula.TargetBody));
		if (cannulaItem is null)
		{
			message = "No prepared cannula suitable for the patient is available for anesthesia.";
			return false;
		}

		if (!EnsureHeld(employee, cannulaItem, out message))
		{
			return false;
		}

		foreach (var bodypart in patient.Body.Bodyparts)
		{
			var args = new object[] { HeldItemSelector(employee, cannulaItem), bodypart.Name };
			if (!cannulationProcedure.CanPerformProcedure(employee, patient, args))
			{
				continue;
			}

			if (!TryBeginTrackedProcedure(context, employee, patient, hospital, request, cannulationProcedure, args, null,
				out message))
			{
				return false;
			}

			message = $"{employee.HowSeen(employee, true)} began {cannulationProcedure.ProcedureName.ColourName()} for anesthesia access on {patient.HowSeen(employee)}.";
			return true;
		}

		message = cannulationProcedure.WhyCannotPerformProcedure(employee, patient,
			HeldItemSelector(employee, cannulaItem), patient.Body.Bodyparts.FirstOrDefault()?.Name ?? string.Empty);
		return false;
	}

	private static bool PrepareAnesthesiaViaDrip(IEmploymentTaskContext context, ICharacter employee, ICharacter patient,
		IHospitalServiceRequest request, IDrug drug, out string message)
	{
		if (PatientCannula(patient) is not { } cannula)
		{
			message = $"{patient.HowSeen(employee, true)} does not have a cannula installed for IV anesthesia.";
			return false;
		}

		if (!TryCalculateAnesthesiaDose(employee, patient, request.Service, drug, out var grams, out message))
		{
			return false;
		}

		var items = CandidateHospitalItems(context, employee, request).Append(cannula.Parent).DistinctBy(x => x.Id).ToList();
		var drip = items.Select(x => x.GetItemType<IDrip>()).Where(x => x is not null).Select(x => x!).FirstOrDefault();
		if (drip is null)
		{
			message = "No prepared IV drip is available for anesthesia.";
			return false;
		}

		var container = items.Select(x => x.GetItemType<ILiquidContainer>()).Where(x => x is not null).Select(x => x!)
		                     .Where(x => AnestheticConcentration(x, drug) > 0.0)
		                     .FirstOrDefault(x => grams <= 0.0 ||
		                                          x.LiquidVolume * AnestheticConcentration(x, drug) >= grams);
		if (container is null)
		{
			message = $"No prepared IV liquid container has enough {drug.Name} for anesthesia.";
			return false;
		}

		if (container is not IConnectable containerConnection)
		{
			message = $"{container.Parent.HowSeen(employee, true)} cannot be connected as an IV source.";
			return false;
		}

		if (!ConnectIfNeeded(employee, containerConnection, drip, out message) ||
		    !ConnectIfNeeded(employee, drip, cannula, out message))
		{
			return false;
		}

		var concentration = AnestheticConcentration(container, drug);
		var primingVolume = grams <= 0.0 ? 0.0 : grams / concentration;
		if (primingVolume > container.LiquidVolume)
		{
			message = $"{container.Parent.HowSeen(employee, true)} does not contain enough {drug.Name} for the calculated anesthetic dose.";
			return false;
		}

		if (!SetDripRate(employee, drip, primingVolume, out message))
		{
			return false;
		}

		if (container.Parent.GetItemType<ISwitchable>() is not { } switchable)
		{
			message = $"{container.Parent.HowSeen(employee, true)} cannot be switched into drip mode.";
			return false;
		}

		if (primingVolume > 0.0)
		{
			var primed = container.RemoveLiquidAmount(primingVolume, employee, "hospitalanesthesia");
			patient.Body.HealthStrategy.InjectedLiquid(patient, primed);
		}

		if (!switchable.Switch(employee, "drip"))
		{
			message = switchable.WhyCannotSwitch(employee, "drip");
			return false;
		}

		employee.OutputHandler.Send(new EmoteOutput(new Emote(
			"@ connect|connects $1 through $2 to $0's cannula and start|starts the anesthetic drip.",
			employee, patient, container.Parent, drip.Parent)));

		return ValidateAnesthesiaOutcome(employee, patient, out message);
	}

	private static bool TryCalculateAnesthesiaDose(ICharacter employee, ICharacter patient, IHospitalService service,
		IDrug drug, out double grams, out string message)
	{
		grams = 0.0;
		message = string.Empty;
		var targetIntensity = Math.Clamp(service.AnesthesiaIntensity <= 0.0 ? 1.25 : service.AnesthesiaIntensity, 1.0, 2.45);
		var currentIntensity = patient.Body.EffectsOfType<Anesthesia>()
		                              .Select(x => x.IntensityPerGramMass)
		                              .DefaultIfEmpty(0.0)
		                              .Sum();
		var deficit = targetIntensity - currentIntensity;
		if (deficit <= 0.0)
		{
			return true;
		}

		var intensityPerGram = drug.IntensityForType(DrugType.Anesthesia);
		if (intensityPerGram <= 0.0)
		{
			message = $"{drug.Name} does not have a positive anesthesia intensity.";
			return false;
		}

		var patientMassFactor = patient.Body.Weight * patient.Gameworld.UnitManager.BaseWeightToKilograms * 0.001;
		if (patientMassFactor <= 0.0)
		{
			message = $"{patient.HowSeen(employee, true)} has no valid body mass for anesthesia dosing.";
			return false;
		}

		grams = deficit * patientMassFactor / intensityPerGram;
		return true;
	}

	private static bool ValidateAnesthesiaOutcome(ICharacter employee, ICharacter patient, out string message)
	{
		if (patient.NeedsToBreathe && !patient.IsBreathing)
		{
			message = $"{patient.HowSeen(employee, true)} is not breathing safely after anesthesia.";
			return false;
		}

		if (!patient.IsHelpless)
		{
			message = $"{patient.HowSeen(employee, true)} was not made helpless or unconscious by the configured anesthesia dose.";
			return false;
		}

		message = $"{patient.HowSeen(employee, true)} has been anesthetised for surgery.";
		return true;
	}

	private static ICannula? PatientCannula(ICharacter patient)
	{
		return patient.Body.Implants
		              .Select(x => x.Parent.GetItemType<ICannula>()).Where(x => x is not null).Select(x => x!)
		              .FirstOrDefault();
	}

	private static bool EnsureHeld(ICharacter employee, IGameItem item, out string reason)
	{
		if (EmploymentWorkerItemLocator.IsHeldOrWielded(employee, item))
		{
			reason = string.Empty;
			return true;
		}

		if (!employee.Body.CanGet(item, 0, ItemCanGetIgnore.IgnoreWeight))
		{
			reason = $"{employee.HowSeen(employee, true)} cannot pick up {item.HowSeen(employee)} for the procedure.";
			return false;
		}

		employee.Body.Get(item, silent: true, ignoreFlags: ItemCanGetIgnore.IgnoreWeight);
		reason = string.Empty;
		return true;
	}

	private static bool ConnectIfNeeded(ICharacter employee, IConnectable source, IConnectable target, out string reason)
	{
		if (source.ConnectedItems.Any(x => x.Item2 == target))
		{
			reason = string.Empty;
			return true;
		}

		if (!source.CanConnect(employee, target))
		{
			reason = source.WhyCannotConnect(employee, target);
			return false;
		}

		source.Connect(employee, target);
		reason = string.Empty;
		return true;
	}

	private static void DisconnectIfNeeded(ICharacter employee, IConnectable source, IConnectable target)
	{
		if (source.ConnectedItems.All(x => x.Item2 != target))
		{
			return;
		}

		if (source.CanDisconnect(employee, target))
		{
			source.Disconnect(employee, target);
			return;
		}

		source.RawDisconnect(target, true);
	}

	private static bool SetDripRate(ICharacter employee, IDrip drip, double primingVolume, out string reason)
	{
		reason = string.Empty;
		if (drip is not ISelectable selectable)
		{
			return true;
		}

		var minimum = 0.0005 / employee.Gameworld.UnitManager.BaseFluidToLitres;
		var maximum = 0.01 / employee.Gameworld.UnitManager.BaseFluidToLitres;
		var rate = primingVolume > 0.0 ? Math.Clamp(primingVolume / 30.0, minimum, maximum) : minimum;
		var rateText = employee.Gameworld.UnitManager.DescribeExact(rate, UnitType.FluidVolume, employee);
		if (selectable.Select(employee, rateText, new Emote(string.Empty, employee), true))
		{
			return true;
		}

		reason = $"{drip.Parent.HowSeen(employee, true)} could not be set to a safe anesthetic drip rate.";
		return false;
	}

	public static bool IsIvCapableLiquidContainerItem(IGameItem item, string switchMode)
	{
		return item.GetItemType<ILiquidContainer>() is IConnectable &&
		       item.GetItemType<ISwitchable>() is { } switchable &&
		       switchable.SwitchSettings.Any(x => x.EqualTo(switchMode));
	}

	private static bool IsIvCapableLiquidContainer(ILiquidContainer container, string switchMode)
	{
		return container is IConnectable &&
		       container.Parent.GetItemType<ISwitchable>() is { } switchable &&
		       switchable.SwitchSettings.Any(x => x.EqualTo(switchMode));
	}

	private static IEnumerable<ILiquidContainer> CandidateIvLiquidContainers(IEmploymentTaskContext context,
		ICharacter employee, IHospitalServiceRequest request, string switchMode)
	{
		return CandidateLiquidContainers(context, employee, request)
		       .Where(x => IsIvCapableLiquidContainer(x, switchMode));
	}

	private static double AnestheticConcentration(ILiquidContainer container, IDrug drug)
	{
		if (container.LiquidMixture is null || container.LiquidMixture.TotalVolume <= 0.0)
		{
			return 0.0;
		}

		var drugGrams = container.LiquidMixture.Instances
		                         .Where(x => x.Liquid.Drug?.Id == drug.Id && x.Liquid.DrugGramsPerUnitVolume > 0.0)
		                         .Sum(x => x.Amount * x.Liquid.DrugGramsPerUnitVolume);
		return drugGrams / container.LiquidMixture.TotalVolume;
	}

	private static IEnumerable<IGameItem> CandidateHospitalItems(IEmploymentTaskContext context,
		ICharacter employee, IHospitalServiceRequest request)
	{
		var theatre = request.OperatingTheatreCellId is { } theatreId
			? request.Hospital.OperatingTheatres.FirstOrDefault(x => x.Id == theatreId)
			: null;
		var theatreItems = theatre is null
			? Enumerable.Empty<IGameItem>()
			: (context.AvailableItems(theatre) ?? Enumerable.Empty<IGameItem>())
			  .Concat(theatre.GameItems ?? Enumerable.Empty<IGameItem>())
			  .SelectMany(DeepItemsOrSelf);
		var seed = EmploymentWorkerItemLocator.TaskHeldItems(context, employee)
		                  .SelectMany(DeepItemsOrSelf)
		                  .Concat(employee.Location?.GameItems.SelectMany(x => x.DeepItems.Append(x)) ?? Enumerable.Empty<IGameItem>())
		                  .Concat(theatreItems)
		                  .DistinctBy(x => x.Id)
		                  .ToList();
		var connected = seed.SelectMany(x => x.GetItemTypes<IConnectable>())
		                    .SelectMany(x => x.ConnectedItems.Select(y => y.Item2.Parent));
		return seed.Concat(connected).DistinctBy(x => x.Id);
	}

	private static ISurgicalProcedure? ResolveBloodAccessProcedure(ICharacter employee, ICharacter patient,
		IHospitalServiceRequest request, SurgicalProcedureType procedureType)
	{
		var serviceType = procedureType switch
		{
			SurgicalProcedureType.Cannulation => HospitalServiceType.Cannulation,
			SurgicalProcedureType.Decannulation => HospitalServiceType.Decannulation,
			_ => (HospitalServiceType?)null
		};

		if (serviceType is not null)
		{
			foreach (var service in request.Hospital.ActiveServices
			                               .Where(x => x.ServiceType == serviceType.Value)
			                               .Where(CanBeUsedByCombinedService))
			{
				var procedure = service.SurgicalProcedure ??
				                ResolveSurgicalProcedureForService(employee, patient, service, service.ProcedureParameters);
				if (procedure?.Procedure == procedureType)
				{
					return procedure;
				}
			}
		}

		return employee.Gameworld.SurgicalProcedures
		               .Where(x => x.Procedure == procedureType)
		               .Where(x => x.TargetBodyType is null || patient.Body.Prototype.CountsAs(x.TargetBodyType))
		               .OrderBy(x => x.Name)
		               .FirstOrDefault();
	}

	private static bool TryBeginBloodCannulation(IEmploymentTaskContext context, ICharacter employee,
		ICharacter patient, IHospitalServiceRequest request, HospitalTreatmentProgress progress,
		BloodWorkflowProgress workflow, out string message)
	{
		if (ResolveBloodAccessProcedure(employee, patient, request, SurgicalProcedureType.Cannulation) is not { } procedure)
		{
			message = "No hospital cannulation procedure is available for IV blood access.";
			return false;
		}

		var cannulaItem = CandidateHospitalItems(context, employee, request)
		                  .FirstOrDefault(x => x.GetItemType<ICannula>() is { } cannula &&
		                                       patient.Body.Prototype.CountsAs(cannula.TargetBody));
		if (cannulaItem is null)
		{
			message = "No prepared cannula suitable for the patient is available for IV blood access.";
			return false;
		}

		if (!EnsureHeld(employee, cannulaItem, out message))
		{
			return false;
		}

		foreach (var bodypart in patient.Body.Bodyparts)
		{
			var args = new object[] { HeldItemSelector(employee, cannulaItem), bodypart.Name };
			if (!procedure.CanPerformProcedure(employee, patient, args))
			{
				continue;
			}

			workflow.Stage = BloodWorkflowStageCannulating;
			workflow.CannulaId = cannulaItem.Id;
			workflow.HospitalInsertedCannula = true;
			progress.BloodWorkflow = workflow;
			if (!TryBeginTrackedProcedure(context, employee, patient, request.Hospital, request, procedure, args, null,
				out message))
			{
				progress.BloodWorkflow = null;
				return false;
			}

			message = $"{employee.HowSeen(employee, true)} began {procedure.ProcedureName.ColourName()} for IV blood access on {patient.HowSeen(employee)}.";
			return true;
		}

		message = procedure.WhyCannotPerformProcedure(employee, patient, HeldItemSelector(employee, cannulaItem),
			patient.Body.Bodyparts.FirstOrDefault()?.Name ?? string.Empty);
		return false;
	}

	private static bool TryBeginBloodDecannulation(IEmploymentTaskContext context, ICharacter employee,
		ICharacter patient, IHospitalServiceRequest request, HospitalTreatmentProgress progress,
		BloodWorkflowProgress workflow, out string message)
	{
		if (PatientCannula(patient) is not { } cannula)
		{
			message = string.Empty;
			return false;
		}

		if (ResolveBloodAccessProcedure(employee, patient, request, SurgicalProcedureType.Decannulation) is not { } procedure)
		{
			message = "No hospital decannulation procedure is available to remove the IV cannula.";
			return false;
		}

		var args = new object[] { cannula.Parent.Keywords.FirstOrDefault() ?? cannula.Parent.Name };
		if (!procedure.CanPerformProcedure(employee, patient, args))
		{
			message = procedure.WhyCannotPerformProcedure(employee, patient, args);
			return false;
		}

		workflow.Stage = BloodWorkflowStageDecannulating;
		workflow.CannulaId = cannula.Parent.Id;
		progress.BloodWorkflow = workflow;
		if (!TryBeginTrackedProcedure(context, employee, patient, request.Hospital, request, procedure, args, null,
			out message))
		{
			return false;
		}

		message = $"{employee.HowSeen(employee, true)} began {procedure.ProcedureName.ColourName()} for {patient.HowSeen(employee)} after IV blood work.";
		return true;
	}

	private static bool TryPrepareBloodIvCircuit(IEmploymentTaskContext context, ICharacter employee,
		ICharacter patient, IHospitalServiceRequest request, HospitalTreatmentProgress progress,
		BloodWorkflowProgress workflow, ILiquidContainer container, string switchMode, out string message)
	{
		if (PatientCannula(patient) is not { } cannula)
		{
			message = $"{patient.HowSeen(employee, true)} does not have a cannula installed for IV blood access.";
			return false;
		}

		if (container is not IConnectable containerConnection)
		{
			message = $"{container.Parent.HowSeen(employee, true)} cannot be connected as an IV container.";
			return false;
		}

		var items = CandidateHospitalItems(context, employee, request)
		            .Append(container.Parent)
		            .Append(cannula.Parent)
		            .DistinctBy(x => x.Id)
		            .ToList();
		var drip = workflow.DripId is { } dripId
			? items.FirstOrDefault(x => x.Id == dripId)?.GetItemType<IDrip>()
			: null;
		drip ??= items.Select(x => x.GetItemType<IDrip>()).Where(x => x is not null).Select(x => x!).FirstOrDefault();
		if (drip is null)
		{
			message = "No prepared IV drip is available for IV blood access.";
			return false;
		}

		if (!ConnectIfNeeded(employee, containerConnection, drip, out message) ||
		    !ConnectIfNeeded(employee, drip, cannula, out message))
		{
			return false;
		}

		var targetAmount = workflow.TargetLitres / employee.Gameworld.UnitManager.BaseFluidToLitres;
		if (!SetDripRate(employee, drip, targetAmount, out message))
		{
			return false;
		}

		if (container.Parent.GetItemType<ISwitchable>() is not { } switchable)
		{
			message = $"{container.Parent.HowSeen(employee, true)} cannot be switched into {switchMode} mode.";
			return false;
		}

		if (!switchable.Switch(employee, switchMode))
		{
			message = switchable.WhyCannotSwitch(employee, switchMode);
			return false;
		}

		workflow.ContainerId = container.Parent.Id;
		workflow.DripId = drip.Parent.Id;
		workflow.CannulaId = cannula.Parent.Id;
		workflow.StartingPatientBloodLitres = patient.Body.CurrentBloodVolumeLitres;
		workflow.StartingContainerVolume = container.LiquidVolume;
		workflow.Stage = switchMode.EqualTo("drain") ? BloodWorkflowStageDraining : BloodWorkflowStageDripping;
		progress.BloodWorkflow = workflow;
		employee.OutputHandler.Send(new EmoteOutput(new Emote(
			switchMode.EqualTo("drain")
				? "@ connect|connects $1 through $2 to $0's cannula and start|starts drawing blood."
				: "@ connect|connects $1 through $2 to $0's cannula and start|starts the transfusion.",
			employee, patient, container.Parent, drip.Parent)));
		message = string.Empty;
		return true;
	}

	private static void NeutralizeBloodWorkflowGear(IEmploymentTaskContext context, ICharacter employee,
		ICharacter patient, IHospitalServiceRequest request, BloodWorkflowProgress workflow)
	{
		var items = CandidateHospitalItems(context, employee, request)
		            .Concat(patient.Body.Implants.Select(x => x.Parent))
		            .DistinctBy(x => x.Id)
		            .ToList();
		var containerItem = workflow.ContainerId is { } containerId
			? items.FirstOrDefault(x => x.Id == containerId)
			: null;
		var dripItem = workflow.DripId is { } dripId
			? items.FirstOrDefault(x => x.Id == dripId)
			: null;
		var cannulaItem = workflow.CannulaId is { } cannulaId
			? items.FirstOrDefault(x => x.Id == cannulaId)
			: PatientCannula(patient)?.Parent;

		if (containerItem?.GetItemType<ISwitchable>() is { } switchable &&
		    switchable.SwitchSettings.Any(x => x.EqualTo("neutral")) &&
		    switchable.CanSwitch(employee, "neutral"))
		{
			switchable.Switch(employee, "neutral");
		}

		var containerConnection = containerItem?.GetItemType<ILiquidContainer>() as IConnectable;
		var drip = dripItem?.GetItemType<IDrip>();
		var cannula = cannulaItem?.GetItemType<ICannula>();
		if (containerConnection is not null && drip is not null)
		{
			DisconnectIfNeeded(employee, containerConnection, drip);
		}

		if (drip is not null && cannula is not null)
		{
			DisconnectIfNeeded(employee, drip, cannula);
		}
	}

	private static IEnumerable<IGameItem> DeepItemsOrSelf(IGameItem item)
	{
		var deepItems = item.DeepItems?.ToList();
		return deepItems?.Any() == true ? deepItems : [item];
	}

	private static bool TryBeginTrackedProcedure(IEmploymentTaskContext context, ICharacter employee, ICharacter patient,
		IHospital hospital, IHospitalServiceRequest request, ISurgicalProcedure procedure, object[] args,
		IGameItem? suppliedImplant, out string reason)
	{
		if (procedure.RequiresUnconsciousPatient && !patient.IsHelpless)
		{
			reason = $"{procedure.ProcedureName} requires the patient to be helpless or unconscious.";
			return false;
		}

		if (!procedure.CanPerformProcedure(employee, patient, args))
		{
			reason = procedure.WhyCannotPerformProcedure(employee, patient, args);
			return false;
		}

		var existing = employee.CombinedEffectsOfType<SurgicalProcedureEffect>().ToHashSet();
		procedure.PerformProcedure(employee, patient, args);
		var effect = employee.CombinedEffectsOfType<SurgicalProcedureEffect>()
		                     .FirstOrDefault(x => !existing.Contains(x) &&
		                                          CharacterInstanceIdentityComparer.SamePhysicalInstance(x.Patient, patient));
		if (effect is not null)
		{
			effect.OnProcedureResolved = (_, completed, outcome) => ResolveStagedProcedure(context, employee, hospital,
				request, procedure, completed, outcome, suppliedImplant);
		}

		reason = string.Empty;
		return true;
	}

	private static IGameItem? ServiceSuppliedImplant(ICharacter employee, IHospitalService service, object[] args)
	{
		if (service.ServiceType is not (HospitalServiceType.ImplantProcedure or HospitalServiceType.InstallImplant) ||
		    service.ImplantItemPrototype is null ||
		    args.FirstOrDefault() is not string selector)
		{
			return null;
		}

		return employee.TargetHeldItem(selector);
	}

	private static ImplantPipelineResolution TryBeginNextImplantPipelineStep(IEmploymentTaskContext context,
		ICharacter employee, IHospital hospital, IHospitalServiceRequest request, ISurgicalProcedure completedProcedure,
		IGameItem? suppliedImplant, out string message)
	{
		message = string.Empty;
		if (request.Service.ServiceType is not (HospitalServiceType.ImplantProcedure or HospitalServiceType.InstallImplant) ||
		    suppliedImplant is null)
		{
			return ImplantPipelineResolution.NoNextStep;
		}

		var nextProcedure = NextImplantPipelineProcedure(request.Service, completedProcedure);
		if (nextProcedure is null)
		{
			return ImplantPipelineResolution.NoNextStep;
		}

		if (request.Patient is not { } patient)
		{
			message = "The implant patient is no longer loaded or available for follow-up configuration.";
			return ImplantPipelineResolution.Failed;
		}

		if (!TryBuildImplantPipelineArguments(patient, suppliedImplant, nextProcedure, out var args, out message))
		{
			return ImplantPipelineResolution.Failed;
		}

		EnsurePipelineSurgicalAccess(patient, args);
		if (!TryBeginTrackedProcedure(context, employee, patient, hospital, request, nextProcedure, args, suppliedImplant,
			out message))
		{
			return ImplantPipelineResolution.Failed;
		}

		employee.OutputHandler.Send(new EmoteOutput(new Emote(
			$"@ continue|continues with {nextProcedure.ProcedureName.ColourName()} for $0.", employee, patient)));
		message = $"{nextProcedure.ProcedureName} began after implant installation.";
		return ImplantPipelineResolution.Started;
	}

	private static ISurgicalProcedure? NextImplantPipelineProcedure(IHospitalService service,
		ISurgicalProcedure completedProcedure)
	{
		if (service.SurgicalProcedure?.Id == completedProcedure.Id)
		{
			return service.ImplantPowerProcedure ?? service.ImplantInterfaceProcedure;
		}

		if (service.ImplantPowerProcedure?.Id == completedProcedure.Id)
		{
			return service.ImplantInterfaceProcedure;
		}

		return null;
	}

	private static bool TryBuildImplantPipelineArguments(ICharacter patient, IGameItem suppliedImplant,
		ISurgicalProcedure procedure, out object[] args, out string reason)
	{
		return procedure.Procedure switch
		{
			SurgicalProcedureType.ConfigureImplantPower => TryBuildImplantPowerArguments(patient, suppliedImplant,
				out args, out reason),
			SurgicalProcedureType.ConfigureImplantInterface => TryBuildImplantInterfaceArguments(patient, suppliedImplant,
				out args, out reason),
			_ => UnsupportedPipelineProcedure(procedure, out args, out reason)
		};
	}

	private static bool UnsupportedPipelineProcedure(ISurgicalProcedure procedure, out object[] args, out string reason)
	{
		args = [];
		reason = $"{procedure.ProcedureName} is not a supported hospital implant follow-up procedure.";
		return false;
	}

	private static bool TryBuildImplantPowerArguments(ICharacter patient, IGameItem suppliedImplant, out object[] args,
		out string reason)
	{
		args = [];
		IGameItem? supplyItem;
		IGameItem? plantItem;
		if (suppliedImplant.GetItemType<IImplantPowerSupply>() is not null)
		{
			supplyItem = suppliedImplant;
			plantItem = patient.Body.Implants
			                   .Select(x => x.Parent)
			                   .FirstOrDefault(x => x.Id != suppliedImplant.Id &&
			                                        x.GetItemType<IImplantPowerPlant>() is not null);
		}
		else if (suppliedImplant.GetItemType<IImplantPowerPlant>() is not null)
		{
			plantItem = suppliedImplant;
			supplyItem = patient.Body.Implants
			                    .Select(x => x.Parent)
			                    .FirstOrDefault(x => x.Id != suppliedImplant.Id &&
			                                         x.GetItemType<IImplantPowerSupply>() is not null);
		}
		else
		{
			reason = $"{suppliedImplant.Name} is not an implant power supply or power plant.";
			return false;
		}

		if (supplyItem is null || plantItem is null)
		{
			reason = "No compatible installed implant power pair was found for configuration.";
			return false;
		}

		if (!TryInstalledImplantSelector(patient, supplyItem, out var supplySelector, out reason) ||
		    !TryInstalledImplantSelector(patient, plantItem, out var plantSelector, out reason))
		{
			return false;
		}

		args = [supplySelector.BodypartToken, supplySelector.Selector, plantSelector.BodypartToken, plantSelector.Selector];
		reason = string.Empty;
		return true;
	}

	private static bool TryBuildImplantInterfaceArguments(ICharacter patient, IGameItem suppliedImplant, out object[] args,
		out string reason)
	{
		args = [];
		IGameItem? targetItem;
		IGameItem? neuralItem;
		if (suppliedImplant.GetItemType<IImplantNeuralLink>() is null)
		{
			targetItem = suppliedImplant;
			neuralItem = patient.Body.Implants
			                    .Select(x => x.Parent)
			                    .FirstOrDefault(x => x.Id != suppliedImplant.Id &&
			                                         x.GetItemType<IImplantNeuralLink>() is not null);
		}
		else
		{
			neuralItem = suppliedImplant;
			targetItem = patient.Body.Implants
			                    .Select(x => x.Parent)
			                    .Where(x => x.Id != suppliedImplant.Id && x.GetItemType<IImplantNeuralLink>() is null)
			                    .FirstOrDefault(x => x.GetItemType<IImplantRespondToCommands>() is not null) ??
			             patient.Body.Implants
			                    .Select(x => x.Parent)
			                    .FirstOrDefault(x => x.Id != suppliedImplant.Id &&
			                                         x.GetItemType<IImplantNeuralLink>() is null);
		}

		if (targetItem is null || neuralItem is null)
		{
			reason = "No compatible installed neural interface pair was found for configuration.";
			return false;
		}

		if (!TryInstalledImplantSelector(patient, targetItem, out var targetSelector, out reason) ||
		    !TryInstalledImplantSelector(patient, neuralItem, out var neuralSelector, out reason))
		{
			return false;
		}

		args = [targetSelector.BodypartToken, targetSelector.Selector, neuralSelector.BodypartToken, neuralSelector.Selector];
		reason = string.Empty;
		return true;
	}

	private static bool TryInstalledImplantSelector(ICharacter patient, IGameItem implantItem,
		out (string BodypartToken, string Selector) selector, out string reason)
	{
		selector = default;
		var implant = implantItem.GetItemType<IImplant>();
		if (implant is null)
		{
			reason = $"{implantItem.Name} is not an installed implant.";
			return false;
		}

		var bodypart = implant.TargetBodypart is IOrganProto organ
			? patient.Body.Bodyparts.FirstOrDefault(x => x.Organs.Contains(organ))
			: implant.TargetBodypart;
		if (bodypart is null)
		{
			reason = $"The bodypart for {implantItem.Name} could not be resolved.";
			return false;
		}

		var candidates = patient.Body.Implants
		                        .Where(x => x.TargetBodypart == bodypart ||
		                                    x.TargetBodypart is IOrganProto op && bodypart.Organs.Contains(op))
		                        .Select(x => x.Parent)
		                        .ToList();
		var index = candidates.FindIndex(x => x.Id == implantItem.Id) + 1;
		if (index <= 0)
		{
			reason = $"{implantItem.Name} is not installed in {patient.HowSeen(patient)}.";
			return false;
		}

		selector = (bodypart.Name, $"#{index.ToString("N0", CultureInfo.InvariantCulture)}");
		reason = string.Empty;
		return true;
	}

	private static void EnsurePipelineSurgicalAccess(ICharacter patient, object[] args)
	{
		foreach (var index in new[] { 0, 2 })
		{
			if (args.ElementAtOrDefault(index) is not { } token)
			{
				continue;
			}

			var bodypart = patient.Body.GetTargetBodypart(token.ToString());
			if (bodypart is null || patient.EffectsOfType<SurgeryFinalisationRequired>().Any(x => x.Bodypart == bodypart))
			{
				continue;
			}

			patient.AddEffect(new SurgeryFinalisationRequired(patient, bodypart, Difficulty.Normal), TimeSpan.FromSeconds(600));
		}
	}
	private static ServiceExecutionResult PerformBloodDonation(IEmploymentTaskContext context, ICharacter employee,
		ICharacter donor, IHospitalServiceRequest request, HospitalTreatmentProgress progress)
	{
		if (donor.Body.BloodLiquid is null || donor.Body.Bloodtype is null || donor.Body.TotalBloodVolumeLitres <= 0.0)
		{
			return new ServiceExecutionResult(false, $"{donor.HowSeen(employee, true)} cannot donate blood.", string.Empty);
		}

		var amountLitres = request.Service.BloodVolumeLitres > 0.0 ? request.Service.BloodVolumeLitres : 0.5;
		var safeMinimum = donor.Body.TotalBloodVolumeLitres * 0.8;
		var workflow = progress.BloodWorkflow;
		var hasActiveWorkflow = workflow is not null && workflow.Kind.EqualTo(BloodDonationPhase);
		if (!hasActiveWorkflow && donor.Body.CurrentBloodVolumeLitres - amountLitres < safeMinimum)
		{
			return new ServiceExecutionResult(false,
				$"Removing {amountLitres.ToString("N2", employee)}L of blood would put {donor.HowSeen(employee)} below the safe donation threshold.", string.Empty);
		}

		if (workflow is null || !workflow.Kind.EqualTo(BloodDonationPhase))
		{
			workflow = new BloodWorkflowProgress
			{
				Kind = BloodDonationPhase,
				TargetLitres = amountLitres,
				StartingPatientBloodLitres = donor.Body.CurrentBloodVolumeLitres,
				StockBeforeLitres = CurrentBloodStockLitres(request.Hospital, donor.Body.Bloodtype)
			};
			progress.ActivePhase = BloodDonationPhase;
			progress.ActiveExpectedCount = 0;
			progress.BloodWorkflow = workflow;
		}

		if (string.IsNullOrWhiteSpace(workflow.Stage))
		{
			return StartDonationWorkflow(context, employee, donor, request, progress, workflow);
		}

		if (workflow.Stage.EqualTo(BloodWorkflowStageCannulating))
		{
			if (PatientCannula(donor) is null)
			{
				return new ServiceExecutionResult(true,
					$"{employee.HowSeen(employee, true)} is waiting for IV cannulation before blood donation from {donor.HowSeen(employee)}.",
					workflow.CannulaId?.ToString("F0", CultureInfo.InvariantCulture) ?? string.Empty, false,
					Progress: progress);
			}

			return StartDonationWorkflow(context, employee, donor, request, progress, workflow);
		}

		if (workflow.Stage.EqualTo(BloodWorkflowStageDecannulating))
		{
			if (PatientCannula(donor) is not null)
			{
				return new ServiceExecutionResult(true,
					$"{employee.HowSeen(employee, true)} is waiting to remove the hospital-inserted cannula from {donor.HowSeen(employee)}.",
					workflow.CannulaId?.ToString("F0", CultureInfo.InvariantCulture) ?? string.Empty, false,
					Progress: progress);
			}

			return CompleteDonationWorkflow(context, employee, donor, request, progress, workflow);
		}

		if (!workflow.Stage.EqualTo(BloodWorkflowStageDraining))
		{
			return new ServiceExecutionResult(false, "The blood donation workflow is in an unknown state.", string.Empty,
				Progress: progress);
		}

		var container = BloodWorkflowContainer(context, employee, request, workflow, "drain");
		if (container is null)
		{
			NeutralizeBloodWorkflowGear(context, employee, donor, request, workflow);
			return new ServiceExecutionResult(false, "The active blood donation IV container is no longer available.",
				string.Empty, Progress: progress);
		}

		var collectedLitres = Math.Max(0.0,
			(container.LiquidVolume - workflow.StartingContainerVolume) *
			donor.Gameworld.UnitManager.BaseFluidToLitres);
		var remainingCapacity = container.LiquidCapacity - container.LiquidVolume;
		if (collectedLitres + BloodWorkflowToleranceLitres < workflow.TargetLitres &&
		    remainingCapacity > 0.0 &&
		    donor.Body.CurrentBloodVolumeLitres > safeMinimum + BloodWorkflowToleranceLitres)
		{
			return new ServiceExecutionResult(true,
				$"{employee.HowSeen(employee, true)} is drawing blood from {donor.HowSeen(employee)} into {container.Parent.HowSeen(employee)}.",
				container.Parent.Id.ToString("F0", CultureInfo.InvariantCulture), false, Progress: progress);
		}

		NeutralizeBloodWorkflowGear(context, employee, donor, request, workflow);
		workflow.CompletedLitres = Math.Min(workflow.TargetLitres, collectedLitres);
		workflow.UsedContainerIds.Add(container.Parent.Id);
		if (workflow.CompletedLitres <= BloodWorkflowToleranceLitres)
		{
			progress.BloodWorkflow = null;
			progress.ActivePhase = null;
			return new ServiceExecutionResult(false, "No blood was collected before the donation workflow stopped.",
				string.Empty, Progress: progress);
		}

		if (workflow.HospitalInsertedCannula && PatientCannula(donor) is not null)
		{
			if (!TryBeginBloodDecannulation(context, employee, donor, request, progress, workflow, out var decannulationMessage))
			{
				progress.BloodWorkflow = null;
				progress.ActivePhase = null;
				return new ServiceExecutionResult(false, decannulationMessage, string.Empty, Progress: progress);
			}

			return new ServiceExecutionResult(true, decannulationMessage,
				container.Parent.Id.ToString("F0", CultureInfo.InvariantCulture), false, Progress: progress);
		}

		return CompleteDonationWorkflow(context, employee, donor, request, progress, workflow);
	}

	private static ServiceExecutionResult StartDonationWorkflow(IEmploymentTaskContext context, ICharacter employee,
		ICharacter donor, IHospitalServiceRequest request, HospitalTreatmentProgress progress,
		BloodWorkflowProgress workflow)
	{
		if (PatientCannula(donor) is not { } cannula)
		{
			if (!TryBeginBloodCannulation(context, employee, donor, request, progress, workflow, out var cannulationMessage))
			{
				progress.BloodWorkflow = null;
				progress.ActivePhase = null;
				return new ServiceExecutionResult(false, cannulationMessage, string.Empty, Progress: progress);
			}

			return new ServiceExecutionResult(true, cannulationMessage,
				workflow.CannulaId?.ToString("F0", CultureInfo.InvariantCulture) ?? string.Empty, false,
				Progress: progress);
		}

		workflow.HospitalInsertedCannula = workflow.HospitalInsertedCannula && workflow.CannulaId == cannula.Parent.Id;
		workflow.CannulaId = cannula.Parent.Id;
		var liquidAmount = workflow.TargetLitres / donor.Gameworld.UnitManager.BaseFluidToLitres;
		var container = CandidateIvLiquidContainers(context, employee, request, "drain")
		                .FirstOrDefault(x => x.LiquidCapacity - x.LiquidVolume >= liquidAmount &&
		                                     (x.LiquidMixture is null || x.LiquidMixture.IsEmpty ||
		                                      x.LiquidMixture.CanMerge(donor.Body.BloodLiquid)));
		if (container is null)
		{
			progress.BloodWorkflow = null;
			progress.ActivePhase = null;
			return new ServiceExecutionResult(false,
				$"There is no prepared IV blood container with room for {workflow.TargetLitres.ToString("N2", employee)}L of blood.",
				string.Empty, Progress: progress);
		}

		if (!TryPrepareBloodIvCircuit(context, employee, donor, request, progress, workflow, container, "drain",
			out var message))
		{
			progress.BloodWorkflow = null;
			progress.ActivePhase = null;
			return new ServiceExecutionResult(false, message, string.Empty, Progress: progress);
		}

		return new ServiceExecutionResult(true,
			$"{employee.HowSeen(employee, true)} prepared an IV blood donation from {donor.HowSeen(employee)} into {container.Parent.HowSeen(employee)}.",
			container.Parent.Id.ToString("F0", CultureInfo.InvariantCulture), false, Progress: progress);
	}

	private static ServiceExecutionResult CompleteDonationWorkflow(IEmploymentTaskContext context, ICharacter employee,
		ICharacter donor, IHospitalServiceRequest request, HospitalTreatmentProgress progress,
		BloodWorkflowProgress workflow)
	{
		progress.ActivePhase = null;
		progress.ActiveExpectedCount = 0;
		progress.CompletedPhases.Add(BloodDonationPhase);
		progress.BloodWorkflow = null;
		var amountLitres = workflow.CompletedLitres;
		var containerId = workflow.UsedContainerIds.OrderBy(x => x).LastOrDefault();
		employee.OutputHandler.Send(new EmoteOutput(new Emote(
			"@ finish|finishes drawing blood from $0 and secure|secures the IV collection gear.", employee, donor)));
		var payout = TryPayBloodDonor(context, employee, donor, request, amountLitres, workflow.StockBeforeLitres,
			out var payoutMessage);
		return new ServiceExecutionResult(true,
			$"{employee.HowSeen(employee, true)} collected {amountLitres.ToString("N2", employee)}L of blood from {donor.HowSeen(employee)}{payoutMessage}",
			payout
				? $"{containerId.ToString("F0", CultureInfo.InvariantCulture)};payout=true"
				: containerId.ToString("F0", CultureInfo.InvariantCulture),
			Progress: progress);
	}

	public static double CurrentBloodStockLitres(IHospital hospital, IBloodtype bloodtype)
	{
		return hospital.Locations
		               .SelectMany(x => x.GameItems.SelectMany(y => y.DeepItems.Append(y)))
		               .DistinctBy(x => x.Id)
		               .Where(x => x is not null)
		               .SelectNotNull(x => x!.GetItemType<ILiquidContainer>())
		               .Sum(x => (x.LiquidMixture?.Instances
		                         .OfType<BloodLiquidInstance>()
		                         .Where(y => y.BloodType?.Id == bloodtype.Id)
		                         .Sum(y => y.Amount) ?? 0.0) * hospital.Gameworld.UnitManager.BaseFluidToLitres);
	}

	private static bool TryPayBloodDonor(IEmploymentTaskContext context, ICharacter employee, ICharacter donor,
		IHospitalServiceRequest request, double amountLitres, double stockBefore, out string message)
	{
		message = string.Empty;
		if (donor.Body.Bloodtype is not { } bloodtype)
		{
			return false;
		}

		var policy = request.Hospital.BloodStockPolicyFor(bloodtype, false);
		if (policy is null || policy.TargetLitres <= stockBefore || policy.PricePerLitre <= 0.0M)
		{
			return false;
		}

		var eligibleLitres = Math.Min(amountLitres, policy.TargetLitres - stockBefore);
		var payout = policy.PricePerLitre * (decimal)eligibleLitres;
		if (payout <= 0.0M)
		{
			return false;
		}

		if (!VirtualCashLedger.Debit(request.Hospital, request.Hospital.Currency, payout, employee, donor,
			"BloodDonor", $"Blood donation from {donor.PersonalName.GetName(MudSharp.Character.Name.NameStyle.FullName)}",
			request.Hospital.BankAccount, request.Hospital.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime,
			out var error, request, $"Hospital request #{request.Id.ToString("N0", CultureInfo.InvariantCulture)}"))
		{
			request.OperationalNotes = string.Concat(request.OperationalNotes,
				$"\nBlood donor payout failed: {error}").Trim();
			request.Changed = true;
			context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, employee,
				$"Blood donor payout for hospital request #{request.Id.ToString("N0", CultureInfo.InvariantCulture)} failed: {error}",
				CurrentCorrelationId(context));
			message = ", but the hospital could not pay the configured donor price";
			return false;
		}

		var cash = CurrencyGameItemComponentProto.CreateNewCurrencyPile(request.Hospital.Currency,
			request.Hospital.Currency.FindCoinsForAmount(payout, out _));
		if (donor.Body.CanGet(cash, 0, ItemCanGetIgnore.IgnoreWeight))
		{
			donor.Body.Get(cash, silent: true, ignoreFlags: ItemCanGetIgnore.IgnoreWeight);
		}
		else if (donor.Location is not null)
		{
			cash.RoomLayer = donor.RoomLayer;
			donor.Location.Insert(cash, true);
		}

		donor.OutputHandler.Send($"{request.Hospital.Name.ColourName()} pays you {request.Hospital.Currency.Describe(payout, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} for your blood donation.");
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, employee,
			$"Paid {request.Hospital.Currency.Describe(payout, CurrencyDescriptionPatternType.ShortDecimal)} to {donor.PersonalName.GetName(MudSharp.Character.Name.NameStyle.FullName)} for {eligibleLitres.ToString("N2", CultureInfo.InvariantCulture)}L of {bloodtype.Name} blood.",
			CurrentCorrelationId(context));
		message = $", and paid {request.Hospital.Currency.Describe(payout, CurrencyDescriptionPatternType.ShortDecimal)} to the donor";
		return true;
	}
	private static ServiceExecutionResult PerformBloodTransfusion(IEmploymentTaskContext context, ICharacter employee,
		ICharacter recipient, IHospitalServiceRequest request, HospitalTreatmentProgress progress,
		bool allowBloodVolumeSubstitute = false)
	{
		if (recipient.Body.BloodLiquid is null || recipient.Body.Bloodtype is null || recipient.Body.TotalBloodVolumeLitres <= 0.0)
		{
			return new ServiceExecutionResult(false, $"{recipient.HowSeen(employee, true)} cannot receive a blood transfusion.", string.Empty);
		}

		var workflow = progress.BloodWorkflow;
		var hasActiveWorkflow = workflow is not null && workflow.Kind.EqualTo(BloodTransfusionPhase);
		var neededLitres = Math.Min(request.Service.BloodVolumeLitres > 0.0 ? request.Service.BloodVolumeLitres : 0.5,
			recipient.Body.TotalBloodVolumeLitres - recipient.Body.CurrentBloodVolumeLitres);
		if (neededLitres <= 0.0 && !hasActiveWorkflow)
		{
			return new ServiceExecutionResult(false, $"{recipient.HowSeen(employee, true)} does not need additional blood volume.", string.Empty);
		}

		if (workflow is null || !workflow.Kind.EqualTo(BloodTransfusionPhase))
		{
			var liquidAmount = neededLitres / recipient.Gameworld.UnitManager.BaseFluidToLitres;
			var candidates = CompatibleBloodProductCandidates(context, employee, request, recipient)
			                 .OrderByDescending(x => x.ExactMatch)
			                 .ThenByDescending(x => x.Amount)
			                 .ToList();
			var totalCompatible = candidates.Sum(x => x.Amount);
			var canUseSubstitute = allowBloodVolumeSubstitute &&
			                       totalCompatible < liquidAmount &&
			                       BloodVolumeSubstituteAmount(context, employee, request) >= liquidAmount;
			if (totalCompatible < liquidAmount && !canUseSubstitute)
			{
				var sawIncompatible = CandidateIvLiquidContainers(context, employee, request, "drip")
				                      .Any(x => (x.LiquidMixture?.Instances.OfType<BloodLiquidInstance>().Any(y =>
					                      y.BloodType is null ||
					                      !recipient.Body.Bloodtype.IsCompatibleWithDonorBlood(y.BloodType)) ?? false));
				return new ServiceExecutionResult(false, sawIncompatible
					? "The prepared IV blood product is not compatible with the recipient."
					: $"No prepared IV blood product contains {neededLitres.ToString("N2", employee)}L of compatible blood.", string.Empty,
					Progress: progress);
			}

			workflow = new BloodWorkflowProgress
			{
				Kind = BloodTransfusionPhase,
				TargetLitres = neededLitres,
				StartingPatientBloodLitres = recipient.Body.CurrentBloodVolumeLitres,
				UseSubstitute = canUseSubstitute
			};
			progress.ActivePhase = BloodTransfusionPhase;
			progress.ActiveExpectedCount = 0;
			progress.BloodWorkflow = workflow;
		}

		if (string.IsNullOrWhiteSpace(workflow.Stage))
		{
			return StartTransfusionWorkflow(context, employee, recipient, request, progress, workflow);
		}

		if (workflow.Stage.EqualTo(BloodWorkflowStageCannulating))
		{
			if (PatientCannula(recipient) is null)
			{
				return new ServiceExecutionResult(true,
					$"{employee.HowSeen(employee, true)} is waiting for IV cannulation before transfusion into {recipient.HowSeen(employee)}.",
					workflow.CannulaId?.ToString("F0", CultureInfo.InvariantCulture) ?? string.Empty, false,
					Progress: progress);
			}

			return StartTransfusionWorkflow(context, employee, recipient, request, progress, workflow);
		}

		if (workflow.Stage.EqualTo(BloodWorkflowStageDecannulating))
		{
			if (PatientCannula(recipient) is not null)
			{
				return new ServiceExecutionResult(true,
					$"{employee.HowSeen(employee, true)} is waiting to remove the hospital-inserted cannula from {recipient.HowSeen(employee)}.",
					workflow.CannulaId?.ToString("F0", CultureInfo.InvariantCulture) ?? string.Empty, false,
					Progress: progress);
			}

			return CompleteTransfusionWorkflow(employee, recipient, progress, workflow);
		}

		if (!workflow.Stage.EqualTo(BloodWorkflowStageDripping))
		{
			return new ServiceExecutionResult(false, "The blood transfusion workflow is in an unknown state.",
				string.Empty, Progress: progress);
		}

		var container = BloodWorkflowContainer(context, employee, request, workflow, "drip");
		if (container is null)
		{
			NeutralizeBloodWorkflowGear(context, employee, recipient, request, workflow);
			return new ServiceExecutionResult(false, "The active transfusion IV container is no longer available.",
				string.Empty, Progress: progress);
		}

		var deliveredThisContainer = Math.Max(0.0,
			recipient.Body.CurrentBloodVolumeLitres - workflow.StartingPatientBloodLitres);
		var deliveredTotal = Math.Min(workflow.TargetLitres, workflow.CompletedLitres + deliveredThisContainer);
		if (deliveredTotal + BloodWorkflowToleranceLitres >= workflow.TargetLitres ||
		    recipient.Body.CurrentBloodVolumeLitres >= recipient.Body.TotalBloodVolumeLitres - BloodWorkflowToleranceLitres)
		{
			workflow.CompletedLitres = deliveredTotal;
			workflow.UsedContainerIds.Add(container.Parent.Id);
			NeutralizeBloodWorkflowGear(context, employee, recipient, request, workflow);
			if (workflow.HospitalInsertedCannula && PatientCannula(recipient) is not null)
			{
				if (!TryBeginBloodDecannulation(context, employee, recipient, request, progress, workflow,
					out var decannulationMessage))
				{
					progress.BloodWorkflow = null;
					progress.ActivePhase = null;
					return new ServiceExecutionResult(false, decannulationMessage, string.Empty, Progress: progress);
				}

				return new ServiceExecutionResult(true, decannulationMessage,
					container.Parent.Id.ToString("F0", CultureInfo.InvariantCulture), false, Progress: progress);
			}

			return CompleteTransfusionWorkflow(employee, recipient, progress, workflow);
		}

		if (TransfusionLiquidAmount(container, recipient, workflow.UseSubstitute) > 0.0)
		{
			return new ServiceExecutionResult(true,
				$"{employee.HowSeen(employee, true)} is administering {(workflow.UseSubstitute ? "blood-volume substitute" : "compatible blood")} to {recipient.HowSeen(employee)} through {container.Parent.HowSeen(employee)}.",
				container.Parent.Id.ToString("F0", CultureInfo.InvariantCulture), false, Progress: progress);
		}

		workflow.CompletedLitres = deliveredTotal;
		workflow.UsedContainerIds.Add(container.Parent.Id);
		NeutralizeBloodWorkflowGear(context, employee, recipient, request, workflow);
		var nextContainer = NextTransfusionContainer(context, employee, request, recipient, workflow);
		if (nextContainer is null)
		{
			progress.BloodWorkflow = null;
			progress.ActivePhase = null;
			return new ServiceExecutionResult(false,
				$"The transfusion stopped after {workflow.CompletedLitres.ToString("N2", employee)}L because no further compatible IV blood product was available.",
				string.Empty, Progress: progress);
		}

		if (!TryPrepareBloodIvCircuit(context, employee, recipient, request, progress, workflow, nextContainer, "drip",
			out var nextMessage))
		{
			progress.BloodWorkflow = null;
			progress.ActivePhase = null;
			return new ServiceExecutionResult(false, nextMessage, string.Empty, Progress: progress);
		}

		return new ServiceExecutionResult(true,
			$"{employee.HowSeen(employee, true)} continued the transfusion for {recipient.HowSeen(employee)} with {nextContainer.Parent.HowSeen(employee)}.",
			nextContainer.Parent.Id.ToString("F0", CultureInfo.InvariantCulture), false, Progress: progress);
	}

	private static ServiceExecutionResult StartTransfusionWorkflow(IEmploymentTaskContext context, ICharacter employee,
		ICharacter recipient, IHospitalServiceRequest request, HospitalTreatmentProgress progress,
		BloodWorkflowProgress workflow)
	{
		if (PatientCannula(recipient) is not { } cannula)
		{
			if (!TryBeginBloodCannulation(context, employee, recipient, request, progress, workflow, out var cannulationMessage))
			{
				progress.BloodWorkflow = null;
				progress.ActivePhase = null;
				return new ServiceExecutionResult(false, cannulationMessage, string.Empty, Progress: progress);
			}

			return new ServiceExecutionResult(true, cannulationMessage,
				workflow.CannulaId?.ToString("F0", CultureInfo.InvariantCulture) ?? string.Empty, false,
				Progress: progress);
		}

		workflow.HospitalInsertedCannula = workflow.HospitalInsertedCannula && workflow.CannulaId == cannula.Parent.Id;
		workflow.CannulaId = cannula.Parent.Id;
		var container = NextTransfusionContainer(context, employee, request, recipient, workflow);
		if (container is null)
		{
			progress.BloodWorkflow = null;
			progress.ActivePhase = null;
			return new ServiceExecutionResult(false,
				workflow.UseSubstitute
					? $"No prepared IV blood-volume substitute contains {workflow.TargetLitres.ToString("N2", employee)}L of injectable volume."
					: $"No prepared IV blood product contains {workflow.TargetLitres.ToString("N2", employee)}L of compatible blood.",
				string.Empty, Progress: progress);
		}

		if (!TryPrepareBloodIvCircuit(context, employee, recipient, request, progress, workflow, container, "drip",
			out var message))
		{
			progress.BloodWorkflow = null;
			progress.ActivePhase = null;
			return new ServiceExecutionResult(false, message, string.Empty, Progress: progress);
		}

		return new ServiceExecutionResult(true,
			workflow.UseSubstitute
				? $"{employee.HowSeen(employee, true)} prepared an IV blood-volume substitute for {recipient.HowSeen(employee)} because compatible blood was insufficient."
				: $"{employee.HowSeen(employee, true)} prepared compatible IV blood for transfusion into {recipient.HowSeen(employee)}.",
			container.Parent.Id.ToString("F0", CultureInfo.InvariantCulture), false, Progress: progress);
	}

	private static ServiceExecutionResult CompleteTransfusionWorkflow(ICharacter employee, ICharacter recipient,
		HospitalTreatmentProgress progress, BloodWorkflowProgress workflow)
	{
		progress.ActivePhase = null;
		progress.ActiveExpectedCount = 0;
		progress.CompletedPhases.Add(BloodTransfusionPhase);
		progress.BloodWorkflow = null;
		var amountLitres = Math.Min(workflow.TargetLitres, workflow.CompletedLitres);
		employee.OutputHandler.Send(new EmoteOutput(new Emote(
			workflow.UseSubstitute
				? "@ finish|finishes administering an IV blood-volume substitute to $0 and secure|secures the IV gear."
				: "@ finish|finishes transfusing compatible blood into $0 and secure|secures the IV gear.",
			employee, recipient)));
		return new ServiceExecutionResult(true,
			workflow.UseSubstitute
				? $"{employee.HowSeen(employee, true)} administered {amountLitres.ToString("N2", employee)}L of blood-volume substitute to {recipient.HowSeen(employee)} because compatible blood was insufficient."
				: $"{employee.HowSeen(employee, true)} transfused {amountLitres.ToString("N2", employee)}L of compatible blood into {recipient.HowSeen(employee)} from {workflow.UsedContainerIds.Count.ToString("N0", employee)} container{(workflow.UsedContainerIds.Count == 1 ? "" : "s")}.",
			workflow.UsedContainerIds.Select(x => x.ToString("F0", CultureInfo.InvariantCulture)).ListToCommaSeparatedValues(),
			Progress: progress);
	}

	private sealed record BloodProductCandidate(ILiquidContainer Container, List<BloodLiquidInstance> Blood,
		double Amount, bool ExactMatch);

	private static ILiquidContainer? BloodWorkflowContainer(IEmploymentTaskContext context, ICharacter employee,
		IHospitalServiceRequest request, BloodWorkflowProgress workflow, string switchMode)
	{
		if (workflow.ContainerId is not { } containerId)
		{
			return null;
		}

		return CandidateIvLiquidContainers(context, employee, request, switchMode)
		       .FirstOrDefault(x => x.Parent.Id == containerId);
	}

	private static ILiquidContainer? NextTransfusionContainer(IEmploymentTaskContext context, ICharacter employee,
		IHospitalServiceRequest request, ICharacter recipient, BloodWorkflowProgress workflow)
	{
		if (workflow.UseSubstitute)
		{
			return CandidateIvLiquidContainers(context, employee, request, "drip")
			       .Where(x => !workflow.UsedContainerIds.Contains(x.Parent.Id))
			       .Where(x => TransfusionLiquidAmount(x, recipient, true) > 0.0)
			       .OrderByDescending(x => TransfusionLiquidAmount(x, recipient, true))
			       .FirstOrDefault();
		}

		return CompatibleBloodProductCandidates(context, employee, request, recipient)
		       .Where(x => !workflow.UsedContainerIds.Contains(x.Container.Parent.Id))
		       .OrderByDescending(x => x.ExactMatch)
		       .ThenByDescending(x => x.Amount)
		       .Select(x => x.Container)
		       .FirstOrDefault();
	}

	private static double TransfusionLiquidAmount(ILiquidContainer container, ICharacter recipient, bool useSubstitute)
	{
		if (useSubstitute)
		{
			return container.LiquidMixture?.Instances
			                .Where(x => x.Liquid.InjectionConsequence == LiquidInjectionConsequence.BloodVolume)
			                .Sum(x => x.Amount) ?? 0.0;
		}

		if (recipient.Body.Bloodtype is not { } bloodtype)
		{
			return 0.0;
		}

		var blood = container.LiquidMixture?.Instances.OfType<BloodLiquidInstance>().ToList() ?? [];
		if (!blood.Any() ||
		    blood.Any(x => x.BloodType is null || !bloodtype.IsCompatibleWithDonorBlood(x.BloodType)))
		{
			return 0.0;
		}

		return blood.Sum(x => x.Amount);
	}

	private static IEnumerable<BloodProductCandidate> CompatibleBloodProductCandidates(IEmploymentTaskContext context,
		ICharacter employee, IHospitalServiceRequest request, ICharacter recipient)
	{
		foreach (var container in CandidateIvLiquidContainers(context, employee, request, "drip"))
		{
			var blood = container.LiquidMixture?.Instances.OfType<BloodLiquidInstance>().ToList() ?? [];
			if (!blood.Any() ||
			    blood.Any(x => x.BloodType is null ||
			                   !recipient.Body.Bloodtype!.IsCompatibleWithDonorBlood(x.BloodType)))
			{
				continue;
			}

			yield return new BloodProductCandidate(container, blood, blood.Sum(x => x.Amount),
				blood.All(x => x.BloodType?.Id == recipient.Body.Bloodtype!.Id));
		}
	}

	private static double BloodVolumeSubstituteAmount(IEmploymentTaskContext context, ICharacter employee,
		IHospitalServiceRequest request)
	{
		return CandidateIvLiquidContainers(context, employee, request, "drip")
		       .Sum(x => x.LiquidMixture?.Instances
		                 .Where(y => y.Liquid.InjectionConsequence == LiquidInjectionConsequence.BloodVolume)
		                 .Sum(y => y.Amount) ?? 0.0);
	}

	private static IEnumerable<ILiquidContainer> CandidateLiquidContainers(IEmploymentTaskContext context,
		ICharacter employee, IHospitalServiceRequest request)
	{
		return CandidateHospitalItems(context, employee, request)
		       .Select(x => x.GetItemType<ILiquidContainer>()).Where(x => x is not null).Select(x => x!);
	}

	private static IEnumerable<object> ServiceProcedureArguments(ICharacter employee, ICharacter patient,
		IHospitalServiceRequest request, ISurgicalProcedure procedure, bool createServiceSuppliedItem)
	{
		return ServiceProcedureArguments(employee, patient, request.Service, request.ProcedureParameters, procedure,
			createServiceSuppliedItem);
	}

	private static IEnumerable<object> ServiceProcedureArguments(ICharacter employee, ICharacter patient,
		IHospitalService service, string? procedureParameters, ISurgicalProcedure procedure, bool createServiceSuppliedItem)
	{
		if (service.ImplantItemPrototype is not null && createServiceSuppliedItem)
		{
			var implant = service.ImplantItemPrototype.CreateNew(employee);
			employee.Gameworld.Add(implant);
			if (employee.Body.CanGet(implant, 0, ItemCanGetIgnore.IgnoreWeight))
			{
				employee.Body.Get(implant, silent: true, ignoreFlags: ItemCanGetIgnore.IgnoreWeight);
			}
			else
			{
				employee.Location?.Insert(implant, true);
			}

			yield return HeldItemSelector(employee, implant);
		}

		var parameterText = string.IsNullOrWhiteSpace(procedureParameters)
			? service.ProcedureParameters
			: procedureParameters;
		if (string.IsNullOrWhiteSpace(parameterText))
		{
			foreach (var argument in AutomaticProcedureArguments(employee, patient, procedure))
			{
				yield return argument;
			}

			yield break;
		}

		var stack = new StringStack(parameterText);
		while (!stack.IsFinished)
		{
			var token = stack.PopSpeech();
			if (patient.Body.GetTargetBodypart(token) is { } bodypart)
			{
				yield return bodypart;
				continue;
			}

			if (patient.Body.Organs.FirstOrDefault(x => x.Name.EqualTo(token) || x.FullDescription().EqualTo(token)) is { } organ)
			{
				yield return organ;
				continue;
			}

			yield return token;
		}
	}

	private static IEnumerable<object> AutomaticProcedureArguments(ICharacter employee, ICharacter patient,
		ISurgicalProcedure procedure)
	{
		switch (procedure.Procedure)
		{
			case SurgicalProcedureType.Triage:
			case SurgicalProcedureType.DetailedExamination:
				yield break;
			case SurgicalProcedureType.Decannulation:
				if (PatientCannula(patient)?.Parent is { } cannulaItem)
				{
					yield return cannulaItem.Keywords.FirstOrDefault() ?? cannulaItem.Name;
				}
				yield break;
			case SurgicalProcedureType.Cannulation:
			case SurgicalProcedureType.ExploratorySurgery:
			case SurgicalProcedureType.TraumaControl:
			case SurgicalProcedureType.Amputation:
			case SurgicalProcedureType.Replantation:
			case SurgicalProcedureType.SurgicalBoneSetting:
			case SurgicalProcedureType.InvasiveProcedureFinalisation:
			case SurgicalProcedureType.RemoveImplant:
			case SurgicalProcedureType.ConfigureImplantPower:
			case SurgicalProcedureType.ConfigureImplantInterface:
				if (BestSurgicalBodypartTarget(patient, procedure) is { } bodypart)
				{
					yield return bodypart;
				}
				yield break;
			case SurgicalProcedureType.InstallImplant:
				if (BestSurgicalBodypartTarget(patient, procedure) is { } implantBodypart)
				{
					yield return implantBodypart;
				}
				yield break;
			case SurgicalProcedureType.InstallProsthetic:
				yield break;
			case SurgicalProcedureType.OrganStabilisation:
			case SurgicalProcedureType.OrganExtraction:
				if (BestOrganSurgeryTarget(patient) is var (access, organ) && access is not null && organ is not null)
				{
					yield return access;
					yield return organ;
				}
				yield break;
			case SurgicalProcedureType.OrganTransplant:
				if (BestSurgicalBodypartTarget(patient, procedure) is { } transplantAccess)
				{
					yield return transplantAccess;
				}
				yield break;
		}
	}

	private static IBodypart? BestSurgicalBodypartTarget(ICharacter patient, ISurgicalProcedure procedure)
	{
		if (procedure.Procedure == SurgicalProcedureType.InvasiveProcedureFinalisation)
		{
			if (patient.EffectsOfType<SurgeryFinalisationRequired>()
			           .Select(x => x.Bodypart)
			           .FirstOrDefault() is { } finalisationBodypart)
			{
				return finalisationBodypart;
			}
		}

		if (patient.Wounds
		           .Where(x => x.Bodypart is IBodypart)
		           .Select(x => (IBodypart)x.Bodypart)
		           .FirstOrDefault() is { } woundBodypart)
		{
			return woundBodypart;
		}

		return patient.Body.Bodyparts.FirstOrDefault();
	}

	private static (IBodypart? AccessBodypart, IOrganProto? Organ) BestOrganSurgeryTarget(ICharacter patient)
	{
		var organ = patient.Wounds
		                   .Where(x => x.Bodypart is IOrganProto)
		                   .OrderByDescending(x => x.CurrentDamage)
		                   .Select(x => (IOrganProto)x.Bodypart)
		                   .FirstOrDefault() ??
		            patient.Body.EffectsOfType<IInternalBleedingEffect>()
		                   .OrderByDescending(x => x.BloodlossPerTick)
		                   .Select(x => x.Organ)
		                   .FirstOrDefault();
		if (organ is null)
		{
			return (null, null);
		}

		var access = patient.Body.Bodyparts
		                    .Where(x => x.Organs.Contains(organ))
		                    .FirstMax(x => (x.Alignment.FrontRearOnly() == Alignment.Front ? 10 : 1) *
		                                   x.RelativeHitChance);
		return (access, organ);
	}

	private static string HeldItemSelector(ICharacter employee, IGameItem item)
	{
		var held = EmploymentWorkerItemLocator.HeldOrWieldedItems(employee).ToList();
		var index = held.FindIndex(x => x.Id == item.Id);
		return index >= 0
			? $"#{(index + 1).ToString("N0", CultureInfo.InvariantCulture)}"
			: item.Keywords.FirstOrDefault() ?? item.Name;
	}

	private static ITreatment? BestTreatmentItem(ICharacter employee, TreatmentType treatment, Difficulty difficulty)
	{
		return EmploymentWorkerItemLocator.HeldOrWieldedItems(employee)
		               .Select(x => x.GetItemType<ITreatment>())
		               .Where(x => x is not null && x.IsTreatmentType(treatment))
		               .Cast<ITreatment>()
		               .FirstMin(x => x.GetTreatmentDifficulty(difficulty));
	}

	private static string? CurrentOperationalPayload(IEmploymentTaskContext context)
	{
		return context is EmploymentTaskContext concrete && concrete.CurrentTask is not null
			? concrete.CurrentTask.StepOperationalStates.ElementAtOrDefault(concrete.CurrentStepIndex)?.OperationalPayload
			: null;
	}

	private static Dictionary<string, string> ParsePayload(string payload)
	{
		var result = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
		foreach (var part in payload.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
		{
			var split = part.Split('=', 2, StringSplitOptions.TrimEntries);
			if (split.Length != 2)
			{
				continue;
			}

			result[split[0]] = split[1];
		}

		return result;
	}

	private static void FailRequest(IHospitalServiceRequest request, string reason)
	{
		request.MarkStatus(HospitalServiceRequestStatus.Failed, reason);
	}

	private static Guid? CurrentCorrelationId(IEmploymentTaskContext context)
	{
		return context is EmploymentTaskContext concrete && concrete.CurrentTask is not null
			? concrete.CurrentTask.CorrelationId
			: null;
	}
}
