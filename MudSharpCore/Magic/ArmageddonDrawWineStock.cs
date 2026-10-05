#nullable enable
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Form.Material;
using MudSharp.FutureProg;
using MudSharp.Planes;
namespace MudSharp.Magic;

public static class ArmageddonDrawWineStock
{
	public const string Key = ArmageddonReviewedProvisionContent.DrawWineKey;
	public const string Name = ArmageddonReviewedProvisionContent.DrawWineName;
	public const double MinimumEnergy = ArmageddonReviewedProvisionContent.MinimumEnergy;
	public const string Litres = ArmageddonReviewedProvisionContent.DrawWineLitres;
	internal const string EligibilitySource = ArmageddonReviewedProvisionContent.DrawWineEligibilitySource;
	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait,
		IMagicResource resource, ILiquid wine, IPlane? bonusPlane = null)
	{
		if (world.Liquids.Get(wine.Id) != wine || bonusPlane is not null && world.Planes.Get(bonusPlane.Id) != bonusPlane)
			throw new InvalidOperationException("Select an existing wine liquid and an optional Water bonus plane.");
		return ArmageddonUtilityStock.Create(world, school, trait, resource, ArmageddonReviewedProvisionContent.DrawWine(wine.Id, bonusPlane?.Id));
	}
	internal static XElement Definition(long resource, long cost, long filter, long wine, long? plane) =>
		ArmageddonReviewedProvisionContent.DrawWine(wine, plane).BuildDefinition(resource, cost, filter);
}
