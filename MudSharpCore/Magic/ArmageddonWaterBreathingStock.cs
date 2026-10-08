#nullable enable

using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Form.Material;

namespace MudSharp.Magic;

/// <summary>Editable content; capability admission and acquisition belong to the installer.</summary>
public static class ArmageddonWaterBreathingStock
{
	public const string Key = "arm.spell.water_breathing";
	public const string Name = "Water Breathing";
	public const string PrerequisiteKey = ArmageddonDrawWineStock.Key;
	public const double PrerequisiteRaw = 80;
	public const int Opening = 30;
	public const double RawCap = 90;
	public const double BranchRaw = 80;
	public const double MinimumEnergy = 20;
	public const string LifetimeGroup = "armageddon.water_breathing";
	public const int UnitSeconds = 600;
	public const int MaximumUnits = 36;
	internal static readonly string EligibilitySource = ArmageddonReviewedWaterSeeContent.WaterBreathingEligibilitySource;

	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait,
		IMagicResource resource, IReadOnlyList<ILiquid> water)
	{
		ArgumentNullException.ThrowIfNull(water);
		if (water.Count == 0 || water.Count > 128 || water.Any(x => x is null || x.Id <= 0 || world.Liquids.Get(x.Id) != x) ||
			water.Select(x => x.Id).Distinct().Count() != water.Count)
			throw new InvalidOperationException("Select one to 128 distinct existing native water liquid definitions explicitly.");
		var ids = water.Select(x => x.Id).ToArray();
		return ArmageddonUtilityStock.Create(world, school, trait, resource,
			ArmageddonReviewedWaterSeeContent.WaterBreathing(ids));
	}

	internal static XElement Definition(long resource, long cost, long filter, IReadOnlyList<long> water) =>
		ArmageddonReviewedWaterSeeContent.WaterBreathing(water).BuildDefinition(resource, cost, filter);
}
