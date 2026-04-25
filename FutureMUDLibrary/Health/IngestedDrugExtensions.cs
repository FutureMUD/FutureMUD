using MudSharp.Body;
using MudSharp.Form.Material;
using MudSharp.GameItems.Interfaces;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace MudSharp.Health;

public static class IngestedDrugExtensions
{
	public static void ApplyIngestedDrugDoses(this IBody body, IEnumerable<FoodDrugDose> doses, double proportion, object? originator)
	{
		foreach (var dose in doses)
		{
			if (dose.Drug is null || dose.Grams <= 0.0 || proportion <= 0.0)
			{
				continue;
			}

			if (!dose.Drug.DrugVectors.HasFlag(DrugVector.Ingested))
			{
				continue;
			}

			body.Dose(dose.Drug, DrugVector.Ingested, dose.Grams * proportion, originator);
		}
	}

	public static void ApplyIngestedDrugDoses(this IBody body, LiquidMixture mixture, object? originator)
	{
		if (mixture is null || mixture.TotalVolume <= 0.0)
		{
			return;
		}

		foreach (var instance in mixture.Instances.Where(x => x.Liquid?.Drug is not null))
		{
			var drug = instance.Liquid.Drug;
			if (!drug.DrugVectors.HasFlag(DrugVector.Ingested))
			{
				continue;
			}

			var grams = instance.Amount * instance.Liquid.DrugGramsPerUnitVolume;
			if (grams <= 0.0)
			{
				continue;
			}

			body.Dose(drug, DrugVector.Ingested, grams, originator);
		}
	}
}
