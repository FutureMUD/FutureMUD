#nullable enable
using System.Xml.Linq;
using MudSharp.Body.Traits;
namespace MudSharp.Magic;

public static class ArmageddonUnravelEnchantmentStock
{
	public const string Key=ArmageddonReviewedUtilityContent.UnravelEnchantmentKey;
	public const string Name=ArmageddonReviewedUtilityContent.UnravelEnchantmentName;
	public const double MinimumEnergy=ArmageddonReviewedUtilityContent.UnravelEnchantmentMinimumEnergy;
	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait, IMagicResource resource)=>
		ArmageddonUtilityStock.Create(world, school, trait, resource, ArmageddonReviewedUtilityContent.UnravelEnchantment());
	internal static XElement Definition(long resource,long cost,long filter)=> ArmageddonReviewedUtilityContent.UnravelEnchantment().BuildDefinition(resource, cost, filter);
}
