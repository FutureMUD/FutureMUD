#nullable enable

using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using System.Linq;
using System.Text;

namespace MudSharp.Health;

public static class DrugDosageDescriber
{
	public static string DescribeActiveAndLatentDrugDosages(ICharacter target, IPerceiver voyeur,
		bool includeEmptyMessage = false)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Drugs for {target.HowSeen(voyeur)}:");

		var activeDosages = target.Body.ActiveDrugDosages.ToList();
		var latentDosages = target.Body.LatentDrugDosages.ToList();

		foreach (var drug in activeDosages)
		{
			sb.AppendLine(
				$"\t{drug.Drug.Name.Colour(Telnet.Cyan)} (#{drug.Drug.Id}) @ {DescribeDosageMass(drug, voyeur).Colour(Telnet.Green)}");
		}

		foreach (var drug in latentDosages)
		{
			sb.AppendLine(
				$"\t{drug.Drug.Name.Colour(Telnet.Cyan)} (#{drug.Drug.Id}) @ {DescribeDosageMass(drug, voyeur).Colour(Telnet.Green)} via {drug.OriginalVector.Describe()} (latent)");
		}

		if (includeEmptyMessage && !activeDosages.Any() && !latentDosages.Any())
		{
			sb.AppendLine("\tNo active or latent drug effects detected.");
		}

		return sb.ToString();
	}

	private static string DescribeDosageMass(DrugDosage dosage, IPerceiver voyeur)
	{
		return voyeur.Gameworld.UnitManager.DescribeExact(
			dosage.Grams * 0.001 / voyeur.Gameworld.UnitManager.BaseWeightToKilograms,
			UnitType.Mass,
			voyeur);
	}
}
