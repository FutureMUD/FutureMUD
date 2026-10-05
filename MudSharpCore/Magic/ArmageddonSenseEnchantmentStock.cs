#nullable enable
using System.Xml.Linq;
using MudSharp.Body.Traits;
namespace MudSharp.Magic;

public static class ArmageddonSenseEnchantmentStock
{
	public const string Key=ArmageddonReviewedUtilityContent.SenseEnchantmentKey;
	public const string Name=ArmageddonReviewedUtilityContent.SenseEnchantmentName;
	public const string LifetimeSeconds=ArmageddonReviewedUtilityContent.SenseEnchantmentLifetimeSeconds;
	public const double MinimumEnergy=ArmageddonReviewedUtilityContent.SenseEnchantmentMinimumEnergy;
	internal const string EligibilitySource=ArmageddonReviewedUtilityContent.SenseEnchantmentEligibilitySource;
	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait, IMagicResource resource) =>
		ArmageddonUtilityStock.Create(world, school, trait, resource, ArmageddonReviewedUtilityContent.SenseEnchantment());
	internal static XElement Definition(long resource,long cost,long filter)=> ArmageddonReviewedUtilityContent.SenseEnchantment().BuildDefinition(resource, cost, filter);
}
