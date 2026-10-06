#nullable enable
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Form.Material;
using MudSharp.FutureProg;
using MudSharp.Planes;
namespace MudSharp.Magic;

public static class ArmageddonDrawWaterStock
{
	public const string Key=ArmageddonReviewedUtilityContent.DrawWaterKey;
	public const string Name=ArmageddonReviewedUtilityContent.DrawWaterName;
	public const double MinimumEnergy=ArmageddonReviewedUtilityContent.DrawWaterMinimumEnergy;
	public const string Litres=ArmageddonReviewedUtilityContent.DrawWaterLitres;
	internal const string EligibilitySource=ArmageddonReviewedUtilityContent.DrawWaterEligibilitySource;
	public static MagicSpell Create(IFuturemud world,IMagicSchool school,ITraitDefinition trait,IMagicResource resource,ILiquid water,IPlane? bonusPlane=null)
	{
		if(world.Liquids.Get(water.Id)!=water || bonusPlane is not null && world.Planes.Get(bonusPlane.Id)!=bonusPlane)
			throw new InvalidOperationException("Select existing clean water and an optional water bonus plane.");
		return ArmageddonUtilityStock.Create(world, school, trait, resource, ArmageddonReviewedUtilityContent.DrawWater(water.Id, bonusPlane?.Id));
	}
	internal static XElement Definition(long resource,long cost,long filter,long water,long? plane)=> ArmageddonReviewedUtilityContent.DrawWater(water, plane).BuildDefinition(resource, cost, filter);
}
