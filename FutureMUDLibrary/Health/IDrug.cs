using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Models;

namespace MudSharp.Health
{

	[Flags]
	public enum DrugVector {
		None = 0,
		Injected = 1 << 0,
		Ingested = 1 << 1,
		Inhaled = 1 << 2,
		Touched = 1 << 3
	}

	public class DrugDosage {
		public IDrug Drug { get; init; }
		public double Grams { get; set; }
		public DrugVector OriginalVector { get; init; }
	}

	public abstract class DrugAdditionalInfo
	{
		public abstract string DatabaseString { get; }
	}

	public class NeutraliseDrugAdditionalInfo : DrugAdditionalInfo
	{
		public required List<DrugType> NeutralisedTypes { get; set; }

		#region Overrides of DrugAdditionalInfo

		/// <inheritdoc />
		public override string DatabaseString => NeutralisedTypes.Select(x => ((int)x).ToString("F")).ListToCommaSeparatedValues(" ");

		#endregion
	}

	public class NeutraliseSpecificDrugAdditionalInfo : DrugAdditionalInfo
	{
		public required List<long> NeutralisedIds { get; set; }

		#region Overrides of DrugAdditionalInfo

		/// <inheritdoc />
		public override string DatabaseString => NeutralisedIds.Select(x => x.ToString("F")).ListToCommaSeparatedValues(" ");

		#endregion
	}

	public class BodypartDamageAdditionalInfo : DrugAdditionalInfo
	{
		public required List<BodypartTypeEnum> BodypartTypes { get; set; }
		public override string DatabaseString => BodypartTypes.Select(x => ((int)x).ToString("F")).ListToCommaSeparatedValues(" ");
	}

	public class HealingRateAdditionalInfo : DrugAdditionalInfo
	{
		public required double HealingRateIntensity { get; set; }
		public required double HealingDifficultyIntensity { get; set; }

		#region Overrides of DrugAdditionalInfo

		/// <inheritdoc />
		public override string DatabaseString => $"{HealingRateIntensity:R} {HealingDifficultyIntensity:R}";

		#endregion
	}

	public class MagicAbilityAdditionalInfo : DrugAdditionalInfo
	{
		public required List<long> MagicCapabilityIds { get; set; }

		#region Overrides of DrugAdditionalInfo

		/// <inheritdoc />
		public override string DatabaseString => MagicCapabilityIds.Select(x => x.ToString("F")).ListToCommaSeparatedValues(" ");

		#endregion
	}

	public interface IDrug : IEditableItem, IProgVariable {
		DrugVector DrugVectors { get; }
		IEnumerable<DrugType> DrugTypes { get; }
		T AdditionalInfoFor<T>(DrugType type) where T : DrugAdditionalInfo;
		double IntensityPerGram { get; }
		double RelativeMetabolisationRate { get; }
		double IntensityForType(DrugType type);
		string DescribeEffect(DrugType type, IPerceiver voyeur);
		IDrug Clone(string newName);
	}

	public static class DrugExtensions {
		public static string Describe(this DrugVector type) {
			var list = new List<string>();
			foreach (var @enum in type.GetFlags()) {
				var subtype = (DrugVector) @enum;
				switch (subtype) {
					case DrugVector.Ingested:
						list.Add("Ingested");
						break;
					case DrugVector.Inhaled:
						list.Add("Inhaled");
						break;
					case DrugVector.Injected:
						list.Add("Injected");
						break;
					case DrugVector.Touched:
						list.Add("Touched");
						break;
				}
			}

			return list.ListToString(conjunction: "or ");
		}
	}
}