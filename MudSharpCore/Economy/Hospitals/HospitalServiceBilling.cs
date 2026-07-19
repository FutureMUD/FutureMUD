using MudSharp.Economy.Currency;

#nullable enable

namespace MudSharp.Economy.Hospitals;

public static class HospitalServiceBilling
{
	public static bool IsDonorPaidServiceType(HospitalServiceType serviceType)
	{
		return serviceType == HospitalServiceType.BloodDonation;
	}

	public static bool IsUsageBilledServiceType(HospitalServiceType serviceType)
	{
		return serviceType is HospitalServiceType.Stabilisation or HospitalServiceType.FullTreatment;
	}

	public static string DescribePrice(IHospital hospital, IHospitalService service, ICharacter actor)
	{
		if (IsDonorPaidServiceType(service.ServiceType))
		{
			return service.Price > 0.0M
				? $"pays {hospital.Currency.Describe(service.Price, CurrencyDescriptionPatternType.ShortDecimal)} by default"
				: "paid by blood-stock policy";
		}

		return IsUsageBilledServiceType(service.ServiceType)
			? "as used"
			: hospital.Currency.Describe(service.Price, CurrencyDescriptionPatternType.ShortDecimal);
	}

	public static HospitalPaymentMethod NormalisePaymentMethod(IHospitalService service,
		HospitalPaymentMethod requestedMethod)
	{
		if (IsDonorPaidServiceType(service.ServiceType))
		{
			return HospitalPaymentMethod.DonorPayout;
		}

		return IsUsageBilledServiceType(service.ServiceType) && requestedMethod != HospitalPaymentMethod.Waived
			? HospitalPaymentMethod.Debt
			: requestedMethod;
	}

	public static decimal DonorPayout(IHospital hospital, IHospitalService service, ICharacter donor,
		double collectedLitres, double stockBeforeLitres)
	{
		if (!IsDonorPaidServiceType(service.ServiceType) || donor.Body.Bloodtype is not { } bloodtype)
		{
			return 0.0M;
		}

		var policy = hospital.BloodStockPolicyFor(bloodtype, false);
		if (policy is null)
		{
			return Math.Max(0.0M, service.Price);
		}

		if (policy.TargetLitres <= stockBeforeLitres || policy.PricePerLitre <= 0.0M)
		{
			return 0.0M;
		}

		var eligibleLitres = Math.Min(collectedLitres, policy.TargetLitres - stockBeforeLitres);
		return policy.PricePerLitre * (decimal)Math.Max(0.0, eligibleLitres);
	}

	public static decimal UnitPriceForServiceType(IHospital hospital, HospitalServiceType serviceType)
	{
		if (IsUsageBilledServiceType(serviceType))
		{
			return 0.0M;
		}

		return hospital.Services
		               .Where(x => x.IsActive)
		               .Where(x => x.ServiceType == serviceType)
		               .Where(HospitalMedicalServiceRunner.CanBeUsedByCombinedService)
		               .Where(x => !IsUsageBilledServiceType(x.ServiceType))
		               .OrderBy(x => x.SortOrder)
		               .ThenBy(x => x.Price)
		               .FirstOrDefault()
		               ?.Price ?? 0.0M;
	}

	public static string DescribeUsageLine(IHospital hospital, HospitalServiceType serviceType, int count, ICharacter actor)
	{
		var unitPrice = UnitPriceForServiceType(hospital, serviceType);
		var total = unitPrice * count;
		return $"{count.ToString("N0", actor).ColourValue()}x {serviceType.DescribeEnum().ColourName()} @ {hospital.Currency.Describe(unitPrice, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} = {hospital.Currency.Describe(total, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}";
	}

	public static string DescribeUsageTotal(IHospital hospital, decimal total)
	{
		return hospital.Currency.Describe(total, CurrencyDescriptionPatternType.ShortDecimal);
	}
}
