#nullable enable
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.FutureProg;
namespace MudSharp.Magic;

public static class ArmageddonMendFleshStock
{
	public const string Key=ArmageddonReviewedUtilityContent.MendFleshKey;
	public const string Name=ArmageddonReviewedUtilityContent.MendFleshName;
	public const double MinimumEnergy=ArmageddonReviewedUtilityContent.MendFleshMinimumEnergy;
	public const string HealingAmount=ArmageddonReviewedUtilityContent.MendFleshHealingAmount;
	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait, IMagicResource resource, IFutureProg eligibility)=>
		ArmageddonUtilityStock.Create(world, school, trait, resource, ArmageddonReviewedUtilityContent.MendFlesh(), existingFilter: eligibility);
	internal static XElement Definition(long resource,long cost,long filter)=> ArmageddonReviewedUtilityContent.MendFlesh().BuildDefinition(resource, cost, filter);
}
