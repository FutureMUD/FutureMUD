using System;
using System.Linq;
using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Economy.Hospitals;

public static class HospitalServiceBilling
{
	public static bool IsUsageBilledServiceType(HospitalServiceType serviceType)
	{
		return serviceType is HospitalServiceType.Stabilisation or HospitalServiceType.FullTreatment;
	}

	public static string DescribePrice(IHospital hospital, IHospitalService service, ICharacter actor)
	{
		return IsUsageBilledServiceType(service.ServiceType)
			? "as used"
			: hospital.Currency.Describe(service.Price, CurrencyDescriptionPatternType.ShortDecimal);
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
