#nullable enable

namespace MudSharp.Health;

/// <summary>Additional equipment absorption after armour; leaves nonmechanical damage and other packet fields intact.</summary>
internal static class EquipmentDamageAbsorption
{
	internal static IDamage Apply(IDamage damage, double absorption)
	{
		if (!double.IsFinite(absorption) || absorption <= 0 || damage.DamageType is not
			(DamageType.Slashing or DamageType.Chopping or DamageType.Crushing or DamageType.Piercing or
			 DamageType.Ballistic or DamageType.Shockwave or DamageType.Bite or DamageType.Claw or
			 DamageType.Shearing or DamageType.BallisticArmourPiercing or DamageType.Wrenching or
			 DamageType.Shrapnel or DamageType.Falling or DamageType.ArmourPiercing)) return damage;
		return new Damage(damage)
		{
			DamageAmount = Math.Max(0, damage.DamageAmount - absorption),
			StunAmount = Math.Max(0, damage.StunAmount - absorption)
		};
	}
}
