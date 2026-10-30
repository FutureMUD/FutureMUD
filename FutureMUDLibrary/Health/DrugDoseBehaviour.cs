using System;
using System.Linq;
using MudSharp.Body;

#nullable enable
namespace MudSharp.Health;

[Flags]
public enum DrugDoseBehaviour
{
	Normal = 0,
	FixedDose = 1,
	IgnoreAdditionalExposure = 2
}

/// <summary>Drug-wide administration policy, independent of individual symptom or pharmacological effect types.</summary>
public static class DrugDosePolicy
{
	public static double Normalise(IDrug drug, double grams)
	{
		if (!double.IsFinite(grams) || grams <= 0.0) return 0.0;
		var amount = drug.DoseBehaviour.HasFlag(DrugDoseBehaviour.FixedDose) ? drug.FixedDoseGrams : grams;
		return double.IsFinite(amount) && amount > 0.0 ? amount : 0.0;
	}

	public static bool CanAccept(IBody body, IDrug drug) =>
		!drug.DoseBehaviour.HasFlag(DrugDoseBehaviour.IgnoreAdditionalExposure) ||
		(!body.ActiveDrugDosages.Concat(body.LatentDrugDosages).Any(x => x.Drug.Id == drug.Id && x.Grams > 0.0) &&
		 !body.PartInfections.OfType<IDrugSymptomInfection>().Any(x => x.DefinitionDrugId == drug.Id));
}
