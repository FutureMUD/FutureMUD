#nullable enable

using System.Xml.Linq;
using MudSharp.Body.Traits;

namespace MudSharp.Magic;

public static class ArmageddonPierceConcealmentStock
{
	public const string Key = ArmageddonReviewedPierceContent.Key;
	public const string Name = ArmageddonReviewedPierceContent.Name;
	public const string LifetimeSeconds = ArmageddonReviewedPierceContent.LifetimeSeconds;
	public const double MinimumEnergy = ArmageddonReviewedPierceContent.MinimumEnergy;
	public const string LifetimeGroup = ArmageddonReviewedPierceContent.LifetimeGroup;
	internal const string EligibilitySource = ArmageddonReviewedPierceContent.EligibilitySource;

	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait, IMagicResource resource) =>
		ArmageddonUtilityStock.Create(world, school, trait, resource, ArmageddonReviewedPierceContent.PierceConcealment());

	internal static XElement Definition(long resource, long cost, long filter) =>
		ArmageddonReviewedPierceContent.PierceConcealment().BuildDefinition(resource, cost, filter);
}
