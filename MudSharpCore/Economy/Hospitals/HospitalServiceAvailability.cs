using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Economy.Employment;
using MudSharp.Framework;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;

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
		ICharacter? actor = null, ICharacter? patient = null)
	{
		if (!service.IsActive)
		{
			return Unavailable("inactive");
		}

		if (!hospital.IsTrading)
		{
			return Unavailable("closed");
		}

		if (!TryFindRequiredEquipmentBundle(hospital, service, actor, out var equipmentReason))
		{
			return Unavailable(equipmentReason);
		}

		if (service.SurgicalProcedure?.RequiresUnconsciousPatient == true &&
		    service.AnesthesiaCannulationProcedure is not null &&
		    !HasAnesthesiaDripStock(hospital, service, patient, out var anesthesiaReason))
		{
			return Unavailable(anesthesiaReason);
		}

		if (service.ServiceType == HospitalServiceType.BloodDonation && !HasDonationContainerStock(hospital, service, patient,
			out var donationReason))
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

	private static bool TryFindRequiredEquipmentBundle(IHospital hospital, IHospitalService service, ICharacter? actor,
		out string reason)
	{
		reason = string.Empty;
		if (!service.RequiredEquipment.Any())
		{
			return true;
		}

		foreach (var room in hospital.SupplyRooms)
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
		var amount = (service.BloodVolumeLitres > 0.0 ? service.BloodVolumeLitres : 0.5) /
		             hospital.Gameworld.UnitManager.BaseFluidToLitres;
		var containers = HospitalStockItems(hospital)
		                 .Select(x => x.GetItemType<ILiquidContainer>()).Where(x => x is not null).Select(x => x!)
		                 .Where(x => x.LiquidCapacity - x.LiquidVolume >= amount)
		                 .ToList();
		if (!containers.Any())
		{
			reason = "no empty blood container";
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

			reason = "no empty blood container";
			return false;
		}

		if (containers.Any(x => x.LiquidMixture is null || x.LiquidMixture.IsEmpty || x.LiquidMixture.CanMerge(bloodLiquid)))
		{
			reason = string.Empty;
			return true;
		}

		reason = "no empty or compatible blood container";
		return false;
	}

	private static bool HasTransfusionStock(IHospital hospital, IHospitalService service, ICharacter? patient,
		out string reason)
	{
		var containers = HospitalStockItems(hospital)
		                 .Select(x => x.GetItemType<ILiquidContainer>()).Where(x => x is not null).Select(x => x!)
		                 .ToList();
		if (!containers.Any(x => x.LiquidMixture?.Instances.OfType<BloodLiquidInstance>().Any() == true))
		{
			reason = "no blood stock";
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
		foreach (var container in containers)
		{
			var blood = container.LiquidMixture?.Instances.OfType<BloodLiquidInstance>().ToList() ?? [];
			if (!blood.Any())
			{
				continue;
			}

			if (blood.Any(x => x.BloodType is null || bloodtype.IsCompatibleWithDonorBlood(x.BloodType) == false))
			{
				continue;
			}

			if (blood.Sum(x => x.Amount) >= neededAmount)
			{
				reason = string.Empty;
				return true;
			}
		}

		reason = "no compatible blood";
		return false;
	}

	private static IEnumerable<IGameItem> HospitalStockItems(IHospital hospital)
	{
		return hospital.SupplyRooms
		               .SelectMany(x => x.GameItems.SelectMany(y => y.DeepItems.Append(y)))
		               .DistinctBy(x => x.Id);
	}

	private static double AnestheticLiquidAmount(ILiquidContainer container, IDrug drug)
	{
		return container.LiquidMixture?.Instances
		                .Where(x => x.Liquid.Drug?.Id == drug.Id && x.Liquid.DrugGramsPerUnitVolume > 0.0)
		                .Sum(x => x.Amount) ?? 0.0;
	}
}
