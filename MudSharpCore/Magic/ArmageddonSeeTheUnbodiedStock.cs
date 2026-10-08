#nullable enable

using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Construction;

namespace MudSharp.Magic;

/// <summary>Editable source content; capability admission and acquisition are authored separately.</summary>
public static class ArmageddonSeeTheUnbodiedStock
{
	public const string Key = "arm.spell.see_the_unbodied";
	public const string Name = "See the Unbodied";
	public const string PrerequisiteKey = ArmageddonPierceConcealmentStock.Key;
	public const double PrerequisiteRaw = 80;
	public const int Opening = 30;
	public const double RawCap = 90;
	public const double BranchRaw = 80;
	public const double MinimumEnergy = 7;
	public const string LifetimeSeconds = "1800*grade";
	public const string LifetimeGroup = "armageddon.detect_ethereal";
	public const int UnitSeconds = 600;
	public const int MaximumUnits = 36;
	public const string ComponentReference = "armageddon.see_unbodied.divination";

	internal static readonly string EligibilitySource = ArmageddonReviewedWaterSeeContent.SeeTheUnbodiedEligibilitySource;

	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait,
		IMagicResource resource, ITerrain silt, ITerrain shadow, IReadOnlyList<ITag> divinationRanks)
	{
		ArgumentNullException.ThrowIfNull(divinationRanks);
		if (silt is null || shadow is null || silt.Id <= 0 || shadow.Id <= 0 ||
			!ReferenceEquals(world.Terrains.Get(silt.Id), silt) || !ReferenceEquals(world.Terrains.Get(shadow.Id), shadow))
			throw new InvalidOperationException("Select existing native Silt and Shadow terrain definitions explicitly.");
		if (divinationRanks.Count != 5 || divinationRanks.Any(x => x is null || x.Id <= 0 || !ReferenceEquals(world.Tags.Get(x.Id), x)) ||
			divinationRanks.Select(x => x.Id).Distinct().Count() != 5 ||
			divinationRanks.Skip(1).Where((tag, index) => !tag.IsA(divinationRanks[index])).Any())
			throw new InvalidOperationException("Select five distinct native Divination rank tags, zero through four, with each higher rank a descendant of the previous rank.");
		var ranks = divinationRanks.Select(x => x.Id).ToArray();
		return ArmageddonUtilityStock.Create(world, school, trait, resource,
			ArmageddonReviewedWaterSeeContent.SeeTheUnbodied(silt.Id, shadow.Id, ranks));
	}

	internal static XElement Definition(long resource, long cost, long filter, long silt, long shadow, IReadOnlyList<long> ranks) =>
		ArmageddonReviewedWaterSeeContent.SeeTheUnbodied(silt, shadow, ranks).BuildDefinition(resource, cost, filter);
}
