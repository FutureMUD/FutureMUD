#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

public static class ArmageddonBaneStock
{
	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition castingTrait,
		IMagicResource resource, IFutureProg eligibility, ITraitDefinition resistanceTrait, Difficulty difficulty,
		double damagePerGrade, double maximumDamage, DamageType damageType)
	{
		if (world.Traits.Get(resistanceTrait.Id) != resistanceTrait || resistanceTrait.OwnerScope != TraitOwnerScope.Character)
			throw new InvalidOperationException("Select an existing character resistance trait in this world.");
		try
		{
			return ArmageddonUtilityStock.Create(world, school, castingTrait, resource,
				ArmageddonBaneContent.Create(resistanceTrait.Id, difficulty, damagePerGrade, maximumDamage, damageType), eligibility);
		}
		catch (ArgumentException ex) { throw new InvalidOperationException(ex.Message, ex); }
	}
}
