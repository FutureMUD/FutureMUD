using MudSharp.Body.Traits;
using MudSharp.Framework;

namespace MudSharp.Combat;

public class ShieldType : FrameworkItem, IShieldType
{
	public ShieldType(MudSharp.Models.ShieldType type, IFuturemud gameworld)
	{
		_id = type.Id;
		_name = type.Name;
		BlockTrait = gameworld.Traits.Get(type.BlockTraitId);
		EffectiveArmourType = gameworld.ArmourTypes.Get(type.EffectiveArmourTypeId ?? 0);
		StaminaPerBlock = type.StaminaPerBlock;
		BlockBonus = type.BlockBonus;
	}

	#region Overrides of Item

	public override string FrameworkItemType => "ShieldType";

	#endregion

	#region Implementation of IShieldType

	public ITraitDefinition BlockTrait { get; set; }
	public double BlockBonus { get; set; }
	public double StaminaPerBlock { get; set; }
	public IArmourType EffectiveArmourType { get; set; }

	#endregion
}