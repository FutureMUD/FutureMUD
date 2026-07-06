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
	private sealed record UsageCharge(HospitalServiceType ServiceType, int Count);

	private sealed class HospitalTreatmentProgress
	{
		public string? ActivePhase { get; set; }
		public int ActiveExpectedCount { get; set; }
		public HashSet<string> CompletedPhases { get; } = new(StringComparer.InvariantCultureIgnoreCase);
		public Dictionary<HospitalServiceType, int> Charges { get; } = new();

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
			HospitalServiceType.BloodDonation => PerformBloodDonation(context, employee, patient, request),
			HospitalServiceType.BloodTransfusion => PerformBloodTransfusion(context, employee, patient, request),
			HospitalServiceType.Stabilisation => PerformStabilisation(context, employee, patient, hospital, request, progress),
			HospitalServiceType.FullTreatment => PerformFullTreatment(context, employee, patient, hospital, request, progress),
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
		HospitalPatientFlow.TransferAfterTreatment(hospital, request, employee, "Hospital recovery routing");
		return true;
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
			var transfusion = PerformBloodTransfusion(context, employee, patient, request);
			if (!transfusion.Success)
			{
				return progress.CompletedPhases.Any()
					? CompleteCommandRoutedService(employee, patient, hospital, request, progress)
					: transfusion with { Progress = progress };
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
			var transfusion = PerformBloodTransfusion(context, employee, patient, request);
			if (transfusion.Success)
			{
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
		if (service.SurgicalProcedure is not { } procedure)
		{
			return new ServiceExecutionResult(false, $"Hospital service {service.Name} has no surgical procedure configured.", string.Empty);
		}

		return BeginConfiguredSurgicalProcedure(context, employee, patient, hospital, request, procedure);
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

		var args = ServiceProcedureArguments(employee, patient, service).ToArray();
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

		if (request.Service.SurgicalProcedure is not { } mainProcedure)
		{
			var message = $"Hospital service {request.Service.Name} has no surgical procedure configured after {completedProcedure.ProcedureName}.";
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
		if (employee.Body.ItemsInHands.Any(x => x.Id == item.Id))
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
		var theatreItems = request.OperatingTheatreCellId is { } theatreId
			? request.Hospital.OperatingTheatres.FirstOrDefault(x => x.Id == theatreId)?.GameItems.SelectMany(x => x.DeepItems.Append(x)) ?? Enumerable.Empty<IGameItem>()
			: Enumerable.Empty<IGameItem>();
		var seed = context.CarriedTaskItems(employee)
		                  .Concat(employee.Inventory)
		                  .Concat(employee.Body.ItemsInHands)
		                  .Concat(employee.Location?.GameItems.SelectMany(x => x.DeepItems.Append(x)) ?? Enumerable.Empty<IGameItem>())
		                  .Concat(theatreItems)
		                  .DistinctBy(x => x.Id)
		                  .ToList();
		var connected = seed.SelectMany(x => x.GetItemTypes<IConnectable>())
		                    .SelectMany(x => x.ConnectedItems.Select(y => y.Item2.Parent));
		return seed.Concat(connected).DistinctBy(x => x.Id);
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
		if (service.ServiceType != HospitalServiceType.ImplantProcedure || service.ImplantItemPrototype is null ||
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
		if (request.Service.ServiceType != HospitalServiceType.ImplantProcedure || suppliedImplant is null)
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
		ICharacter donor, IHospitalServiceRequest request)
	{
		if (donor.Body.BloodLiquid is null || donor.Body.Bloodtype is null || donor.Body.TotalBloodVolumeLitres <= 0.0)
		{
			return new ServiceExecutionResult(false, $"{donor.HowSeen(employee, true)} cannot donate blood.", string.Empty);
		}

		var amountLitres = request.Service.BloodVolumeLitres > 0.0 ? request.Service.BloodVolumeLitres : 0.5;
		var safeMinimum = donor.Body.TotalBloodVolumeLitres * 0.8;
		if (donor.Body.CurrentBloodVolumeLitres - amountLitres < safeMinimum)
		{
			return new ServiceExecutionResult(false,
				$"Removing {amountLitres.ToString("N2", employee)}L of blood would put {donor.HowSeen(employee)} below the safe donation threshold.", string.Empty);
		}

		var liquidAmount = amountLitres / donor.Gameworld.UnitManager.BaseFluidToLitres;
		var container = CandidateLiquidContainers(context, employee, request)
		                .FirstOrDefault(x => x.LiquidCapacity - x.LiquidVolume >= liquidAmount &&
		                                     (x.LiquidMixture is null || x.LiquidMixture.IsEmpty ||
		                                      x.LiquidMixture.CanMerge(donor.Body.BloodLiquid)));
		if (container is null)
		{
			return new ServiceExecutionResult(false,
				$"There is no prepared liquid container with room for {amountLitres.ToString("N2", employee)}L of blood.", string.Empty);
		}

		var stockBefore = CurrentBloodStockLitres(request.Hospital, donor.Body.Bloodtype);
		container.MergeLiquid(new LiquidMixture(new BloodLiquidInstance(donor, liquidAmount), donor.Gameworld), employee,
			"hospitaldonation");
		donor.Body.CurrentBloodVolumeLitres -= amountLitres;
		employee.OutputHandler.Send(new EmoteOutput(new Emote(
			"@ draw|draws blood from $0 into $1.", employee, donor, container.Parent)));
		var payout = TryPayBloodDonor(context, employee, donor, request, amountLitres, stockBefore, out var payoutMessage);
		return new ServiceExecutionResult(true,
			$"{employee.HowSeen(employee, true)} collected {amountLitres.ToString("N2", employee)}L of blood from {donor.HowSeen(employee)}{payoutMessage}",
			payout ? $"{container.Parent.Id.ToString("F0", CultureInfo.InvariantCulture)};payout=true" : container.Parent.Id.ToString("F0", CultureInfo.InvariantCulture));
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
		ICharacter recipient, IHospitalServiceRequest request)
	{
		if (recipient.Body.BloodLiquid is null || recipient.Body.Bloodtype is null || recipient.Body.TotalBloodVolumeLitres <= 0.0)
		{
			return new ServiceExecutionResult(false, $"{recipient.HowSeen(employee, true)} cannot receive a blood transfusion.", string.Empty);
		}

		var neededLitres = Math.Min(request.Service.BloodVolumeLitres > 0.0 ? request.Service.BloodVolumeLitres : 0.5,
			recipient.Body.TotalBloodVolumeLitres - recipient.Body.CurrentBloodVolumeLitres);
		if (neededLitres <= 0.0)
		{
			return new ServiceExecutionResult(false, $"{recipient.HowSeen(employee, true)} does not need additional blood volume.", string.Empty);
		}

		var liquidAmount = neededLitres / recipient.Gameworld.UnitManager.BaseFluidToLitres;
		var sawIncompatible = false;
		foreach (var container in CandidateLiquidContainers(context, employee, request))
		{
			var blood = container.LiquidMixture?.Instances.OfType<BloodLiquidInstance>().ToList() ?? [];
			if (!blood.Any())
			{
				continue;
			}

			if (blood.Any(x => x.BloodType is null || recipient.Body.Bloodtype.IsCompatibleWithDonorBlood(x.BloodType) == false))
			{
				sawIncompatible = true;
				continue;
			}

			if (blood.Sum(x => x.Amount) < liquidAmount)
			{
				continue;
			}

			var removal = BuildBloodRemovalMixture(blood, liquidAmount, recipient.Gameworld);
			container.LiquidMixture!.RemoveLiquid(removal);
			recipient.HealthStrategy.InjectedLiquid(recipient, removal);
			if (recipient.Body.CurrentBloodVolumeLitres > recipient.Body.TotalBloodVolumeLitres)
			{
				recipient.Body.CurrentBloodVolumeLitres = recipient.Body.TotalBloodVolumeLitres;
			}

			employee.OutputHandler.Send(new EmoteOutput(new Emote(
				"@ transfuse|transfuses blood from $1 into $0.", employee, recipient, container.Parent)));
			return new ServiceExecutionResult(true,
				$"{employee.HowSeen(employee, true)} transfused {neededLitres.ToString("N2", employee)}L of compatible blood into {recipient.HowSeen(employee)}.",
				container.Parent.Id.ToString("F0", CultureInfo.InvariantCulture));
		}

		return new ServiceExecutionResult(false, sawIncompatible
			? "The prepared blood product is not compatible with the recipient."
			: $"No prepared blood product contains {neededLitres.ToString("N2", employee)}L of compatible blood.", string.Empty);
	}

	private static LiquidMixture BuildBloodRemovalMixture(IEnumerable<BloodLiquidInstance> blood, double amount,
		IFuturemud gameworld)
	{
		var remaining = amount;
		var instances = new List<LiquidInstance>();
		foreach (var instance in blood)
		{
			var take = Math.Min(remaining, instance.Amount);
			if (take <= 0.0)
			{
				continue;
			}

			var copy = (BloodLiquidInstance)instance.Copy();
			copy.Amount = take;
			instances.Add(copy);
			remaining -= take;
			if (remaining <= 0.0)
			{
				break;
			}
		}

		return new LiquidMixture(instances, gameworld);
	}

	private static IEnumerable<ILiquidContainer> CandidateLiquidContainers(IEmploymentTaskContext context,
		ICharacter employee, IHospitalServiceRequest request)
	{
		return CandidateHospitalItems(context, employee, request)
		       .Select(x => x.GetItemType<ILiquidContainer>()).Where(x => x is not null).Select(x => x!);
	}

	private static IEnumerable<object> ServiceProcedureArguments(ICharacter employee, ICharacter patient,
		IHospitalService service)
	{
		if (service.ImplantItemPrototype is not null)
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

		if (string.IsNullOrWhiteSpace(service.ProcedureParameters))
		{
			yield break;
		}

		foreach (var token in service.ProcedureParameters.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
		{
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

	private static string HeldItemSelector(ICharacter employee, IGameItem item)
	{
		var held = employee.Body.ItemsInHands.ToList();
		var index = held.FindIndex(x => x.Id == item.Id);
		return index >= 0
			? $"#{(index + 1).ToString("N0", CultureInfo.InvariantCulture)}"
			: item.Keywords.FirstOrDefault() ?? item.Name;
	}

	private static ITreatment? BestTreatmentItem(ICharacter employee, TreatmentType treatment, Difficulty difficulty)
	{
		return (employee.Body?.HeldItems ?? Enumerable.Empty<IGameItem>())
		               .SelectNotNull(x => x!.GetItemType<ITreatment>())
		               .Where(x => x.IsTreatmentType(treatment))
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
