using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Employment;
using MudSharp.Framework;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.NPC;
using MudSharp.NPC.AI;

#nullable enable

namespace MudSharp.Economy.Hospitals;

public sealed record HospitalServiceAvailabilityResult(bool Available, string Reason)
{
	public string DescribeColoured()
	{
		return Available ? "Available".ColourValue() : $"Unavailable: {Reason}".ColourError();
	}
}

public static class HospitalServiceAvailability
{
	public static HospitalServiceAvailabilityResult Evaluate(IHospital hospital, IHospitalService service,
		ICharacter? actor = null, ICharacter? patient = null, string? procedureParameters = null)
	{
		if (!service.IsActive)
		{
			return Unavailable("inactive");
		}

		if (!hospital.IsTrading)
		{
			return Unavailable("closed");
		}

		if (!TryGetAvailableMedicalEmployee(hospital, out var employee, out var employeeReason))
		{
			return Unavailable(employeeReason);
		}

		if (!TryFindRequiredEquipmentBundle(hospital, service, actor, out var equipmentReason))
		{
			return Unavailable(equipmentReason);
		}

		var surgicalProcedure = service.SurgicalProcedure;
		if (HospitalMedicalServiceRunner.ServiceTypeToSurgicalProcedureType(service.ServiceType) is not null &&
		    !HasSurgicalFamilyProcedure(hospital, service, employee, patient, procedureParameters,
			    out surgicalProcedure, out var surgeryReason))
		{
			return Unavailable(surgeryReason);
		}

		if (surgicalProcedure?.RequiresUnconsciousPatient == true &&
		    service.AnesthesiaCannulationProcedure is not null &&
		    !HasAnesthesiaDripStock(hospital, service, patient, out var anesthesiaReason))
		{
			return Unavailable(anesthesiaReason);
		}

		if (service.ServiceType == HospitalServiceType.BloodDonation &&
		    (!CanPatientDonateBlood(service, patient, out var donationReason) ||
		     !HasDonationContainerStock(hospital, service, patient, out donationReason) ||
		     !CanFundDonationPayout(hospital, service, patient, out donationReason)))
		{
			return Unavailable(donationReason);
		}

		if (service.ServiceType == HospitalServiceType.BloodTransfusion && !HasTransfusionStock(hospital, service, patient,
			out var transfusionReason))
		{
			return Unavailable(transfusionReason);
		}

		return new HospitalServiceAvailabilityResult(true, string.Empty);
	}

	private static HospitalServiceAvailabilityResult Unavailable(string reason)
	{
		return new HospitalServiceAvailabilityResult(false, reason);
	}

	private static bool TryGetAvailableMedicalEmployee(IHospital hospital, out ICharacter? employee, out string reason)
	{
		var contracts = hospital.EmploymentContracts ?? Array.Empty<IEmploymentContract>();
		var activeTasks = hospital.TaskBoard?.ActiveTasks ?? Array.Empty<IEmploymentActiveTask>();
		foreach (var contract in contracts.Where(x =>
			         x.Status == EmploymentStatus.Active &&
			         x.Authority.Contains(EmploymentAuthority.PerformMedicalServices)))
		{
			var candidate = contract.Employee;
			if (!CharacterState.Able.HasFlag(candidate.State) || candidate.Location is null)
			{
				continue;
			}

			if (!CanPerformAutomatedMedicalService(hospital, candidate))
			{
				continue;
			}

			var employeeId = CharacterInstanceIdentityComparer.IdentityId(candidate);
			if (activeTasks.Any(x =>
				    CharacterInstanceIdentityComparer.IdentityId(x.AssignedEmployee) == employeeId &&
				    x.Status is EmploymentTaskStatus.Assigned or EmploymentTaskStatus.InProgress or
					    EmploymentTaskStatus.Blocked))
			{
				continue;
			}

			employee = candidate;
			reason = string.Empty;
			return true;
		}

		employee = null;
		reason = "no medical employee available";
		return false;
	}

	private static bool CanFundDonationPayout(IHospital hospital, IHospitalService service, ICharacter? donor,
		out string reason)
	{
		reason = string.Empty;
		if (donor?.Body.Bloodtype is null)
		{
			return true;
		}

		var amountLitres = service.BloodVolumeLitres > 0.0 ? service.BloodVolumeLitres : 0.5;
		var stockBefore = HospitalMedicalServiceRunner.CurrentBloodStockLitres(hospital, donor.Body.Bloodtype);
		var payout = HospitalServiceBilling.DonorPayout(hospital, service, donor, amountLitres, stockBefore);
		if (VirtualCashLedger.CanDebit(hospital, hospital.Currency, payout, hospital.BankAccount, out var error))
		{
			return true;
		}

		reason = $"the hospital cannot fund the donor payout: {error}";
		return false;
	}

	private static bool CanPerformAutomatedMedicalService(IHospital hospital, ICharacter employee)
	{
		if (employee is not INPC npc)
		{
			return false;
		}

		return npc.AIs
		          .OfType<EmploymentWorkerAI>()
		          .Any(x =>
			          x.TaskingEnabled &&
			          (x.HostTypeFilter is null || x.HostTypeFilter.Value == hospital.EmploymentHostType) &&
			          x.Capabilities.Contains(EmploymentAICapability.CanPerformMedicalServices));
	}

	private static bool TryFindRequiredEquipmentBundle(IHospital hospital, IHospitalService service, ICharacter? actor,
		out string reason)
	{
		reason = string.Empty;
		if (!service.RequiredEquipment.Any())
		{
			return true;
		}

		foreach (var room in HospitalStockRooms(hospital))
		{
			var available = room.GameItems
			                    .SelectMany(x => x.DeepItems.Append(x))
			                    .DistinctBy(x => x.Id)
			                    .ToList();
			var used = new HashSet<long>();
			var failed = false;
			foreach (var requirement in service.RequiredEquipment)
			{
				var matched = available
				              .Where(x => !used.Contains(x.Id) && MatchesSelector(hospital, actor, x, requirement.Selector))
				              .Take(requirement.Quantity)
				              .ToList();
				if (matched.Count < requirement.Quantity)
				{
					failed = true;
					break;
				}

				foreach (var item in matched)
				{
					used.Add(item.Id);
				}
			}

			if (!failed)
			{
				return true;
			}
		}

		reason = "missing required equipment";
		return false;
	}

	private static bool HasSurgicalFamilyProcedure(IHospital hospital, IHospitalService service, ICharacter? employee,
		ICharacter? patient, string? procedureParameters, out ISurgicalProcedure? procedure, out string reason)
	{
		reason = string.Empty;
		procedure = service.SurgicalProcedure;
		if (HospitalMedicalServiceRunner.ServiceTypeToSurgicalProcedureType(service.ServiceType) is not { } procedureType)
		{
			return true;
		}

		if (employee is not null && patient is not null)
		{
			if (HospitalMedicalServiceRunner.TryResolveSurgicalProcedureForService(employee, patient, service,
				    procedureParameters, out procedure))
			{
				return true;
			}

			reason = $"no usable {procedureType.DescribeEnum()} procedure for this patient and doctor";
			return false;
		}

		if (hospital.Gameworld.SurgicalProcedures.Any(x =>
			    x.Procedure == procedureType &&
			    (patient is null || x.TargetBodyType is null || patient.Body.Prototype.CountsAs(x.TargetBodyType))))
		{
			return true;
		}

		reason = $"no {procedureType.DescribeEnum()} procedure";
		return false;
	}

	private static bool MatchesSelector(IHospital hospital, ICharacter? actor, IGameItem item,
		EmploymentItemSelector selector)
	{
		return selector.Kind switch
		{
			EmploymentItemSelectorKind.PrototypeId => item.Prototype.Id == selector.Id,
			EmploymentItemSelectorKind.ItemId => item.Id == selector.Id,
			EmploymentItemSelectorKind.Tag => !string.IsNullOrWhiteSpace(selector.Text) && ItemHasTag(item, selector.Text),
			EmploymentItemSelectorKind.Keyword => !string.IsNullOrWhiteSpace(selector.Text) && MatchesKeyword(item,
				selector.Text, actor),
			_ => false
		};
	}

	private static bool ItemHasTag(IGameItem item, string tagName)
	{
		return item.Tags.Any(x =>
			x.Name.EqualTo(tagName) ||
			x.FullName.EqualTo(tagName) ||
			x.Id.ToString("F0").EqualTo(tagName));
	}

	private static bool MatchesKeyword(IGameItem item, string text, ICharacter? actor)
	{
		var keywords = text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (actor is not null)
		{
			return item.HasKeywords(keywords, actor, true, true);
		}

		return keywords.All(keyword => item.Keywords.Any(x =>
			x.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase) ||
			x.Contains(keyword, StringComparison.InvariantCultureIgnoreCase)));
	}

	private static bool HasAnesthesiaDripStock(IHospital hospital, IHospitalService service, ICharacter? patient,
		out string reason)
	{
		if (service.AnesthesiaDrug is not { } drug)
		{
			reason = "no anesthetic configured";
			return false;
		}

		if (!drug.DrugTypes.Contains(DrugType.Anesthesia) || !drug.DrugVectors.HasFlag(DrugVector.Injected))
		{
			reason = "invalid anesthetic";
			return false;
		}

		var stock = HospitalStockItems(hospital).ToList();
		var hasPatientCannula = patient?.Body.Implants.Any(x => x.Parent.GetItemType<ICannula>() is not null) == true;
		var hasCannula = hasPatientCannula || stock.Any(x => x.GetItemType<ICannula>() is { } cannula &&
			(patient is null || patient.Body.Prototype.CountsAs(cannula.TargetBody)));
		if (!hasCannula)
		{
			reason = "missing cannula";
			return false;
		}

		if (stock.All(x => x.GetItemType<IDrip>() is null))
		{
			reason = "missing IV drip";
			return false;
		}

		if (stock.Select(x => x.GetItemType<ILiquidContainer>()).Where(x => x is not null).Select(x => x!)
		         .All(x => AnestheticLiquidAmount(x, drug) <= 0.0))
		{
			reason = "missing IV anesthetic";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private static bool HasDonationContainerStock(IHospital hospital, IHospitalService service, ICharacter? donor,
		out string reason)
	{
		var stock = HospitalStockItems(hospital).ToList();
		if (!HasBloodAccessStock(stock, donor, "drain", out reason))
		{
			return false;
		}

		var containers = stock
		                 .Where(x => HospitalMedicalServiceRunner.IsIvCapableLiquidContainerItem(x, "drain"))
		                 .Select(x => x.GetItemType<ILiquidContainer>()).Where(x => x is not null).Select(x => x!)
		                 .Where(HasAnySpareLiquidCapacity)
		                 .ToList();
		if (!containers.Any())
		{
			reason = "no empty IV blood container";
			return false;
		}

		if (donor is null)
		{
			reason = string.Empty;
			return true;
		}

		if (donor.Body.BloodLiquid is not { } bloodLiquid)
		{
			if (containers.Any(x => x.LiquidMixture is null || x.LiquidMixture.IsEmpty))
			{
				reason = string.Empty;
				return true;
			}

			reason = "no empty IV blood container";
			return false;
		}

		if (containers.Any(x => x.LiquidMixture is null || x.LiquidMixture.IsEmpty || x.LiquidMixture.CanMerge(bloodLiquid)))
		{
			reason = string.Empty;
			return true;
		}

		reason = "no empty or compatible IV blood container";
		return false;
	}

	private static bool HasTransfusionStock(IHospital hospital, IHospitalService service, ICharacter? patient,
		out string reason)
	{
		var stock = HospitalStockItems(hospital).ToList();
		if (!HasBloodAccessStock(stock, patient, "drip", out reason))
		{
			return false;
		}

		var containers = stock
		                 .Where(x => HospitalMedicalServiceRunner.IsIvCapableLiquidContainerItem(x, "drip"))
		                 .Select(x => x.GetItemType<ILiquidContainer>()).Where(x => x is not null).Select(x => x!)
		                 .ToList();
		if (!containers.Any(x => x.LiquidMixture?.Instances.OfType<BloodLiquidInstance>().Any() == true))
		{
			reason = "no IV blood stock";
			return false;
		}

		if (patient?.Body.Bloodtype is not { } bloodtype)
		{
			reason = string.Empty;
			return true;
		}

		var needed = Math.Min(service.BloodVolumeLitres > 0.0 ? service.BloodVolumeLitres : 0.5,
			Math.Max(0.0, patient.Body.TotalBloodVolumeLitres - patient.Body.CurrentBloodVolumeLitres));
		if (needed <= 0.0)
		{
			reason = string.Empty;
			return true;
		}

		var neededAmount = needed / hospital.Gameworld.UnitManager.BaseFluidToLitres;
		var compatibleAmount = containers
		                       .Select(x => x.LiquidMixture?.Instances.OfType<BloodLiquidInstance>().ToList() ?? [])
		                       .Where(x => x.Any())
		                       .Where(x => x.All(y => y.BloodType is not null &&
		                                           bloodtype.IsCompatibleWithDonorBlood(y.BloodType)))
		                       .Sum(x => x.Sum(y => y.Amount));
		if (compatibleAmount >= neededAmount)
		{
			reason = string.Empty;
			return true;
		}

		reason = "insufficient compatible blood";
		return false;
	}

	private static bool HasAnySpareLiquidCapacity(ILiquidContainer container)
	{
		return container.LiquidCapacity - container.LiquidVolume > 0.0;
	}

	private static bool HasBloodAccessStock(IReadOnlyCollection<IGameItem> stock, ICharacter? patient,
		string switchMode, out string reason)
	{
		var hasPatientCannula = patient?.Body.Implants.Any(x => x.Parent.GetItemType<ICannula>() is not null) == true;
		var hasCannula = hasPatientCannula || stock.Any(x => x.GetItemType<ICannula>() is { } cannula &&
			(patient is null || patient.Body.Prototype.CountsAs(cannula.TargetBody)));
		if (!hasCannula)
		{
			reason = "missing cannula";
			return false;
		}

		if (stock.All(x => x.GetItemType<IDrip>() is null))
		{
			reason = "missing IV drip";
			return false;
		}

		if (stock.All(x => !HospitalMedicalServiceRunner.IsIvCapableLiquidContainerItem(x, switchMode)))
		{
			reason = $"missing IV container with {switchMode} mode";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private static IEnumerable<IGameItem> HospitalStockItems(IHospital hospital)
	{
		return HospitalStockRooms(hospital)
		               .SelectMany(x => x.GameItems.SelectMany(y => y.DeepItems.Append(y)))
		               .DistinctBy(x => x.Id);
	}

	private static bool CanPatientDonateBlood(IHospitalService service, ICharacter? donor, out string reason)
	{
		reason = string.Empty;
		if (donor is null)
		{
			return true;
		}

		if (donor.Body.BloodLiquid is null || donor.Body.Bloodtype is null ||
		    donor.Body.TotalBloodVolumeLitres <= 0.0)
		{
			reason = "the patient cannot donate blood";
			return false;
		}

		var amountLitres = service.BloodVolumeLitres > 0.0 ? service.BloodVolumeLitres : 0.5;
		var safeMinimum = donor.Body.TotalBloodVolumeLitres *
		                  HospitalMedicalServiceRunner.MinimumBloodFractionForDonation;
		if (donor.Body.CurrentBloodVolumeLitres - amountLitres >= safeMinimum)
		{
			return true;
		}

		reason = $"donating {amountLitres.ToString("N2", donor)}L would put the patient below the safe donation threshold";
		return false;
	}

	private static IEnumerable<ICell> HospitalStockRooms(IHospital hospital)
	{
		return (hospital.SupplyRooms ?? Enumerable.Empty<ICell>())
		       .Concat(hospital.OperatingTheatres ?? Enumerable.Empty<ICell>())
		       .DistinctBy(x => x.Id);
	}

	private static double AnestheticLiquidAmount(ILiquidContainer container, IDrug drug)
	{
		return container.LiquidMixture?.Instances
		                .Where(x => x.Liquid.Drug?.Id == drug.Id && x.Liquid.DrugGramsPerUnitVolume > 0.0)
		                .Sum(x => x.Amount) ?? 0.0;
	}
}
