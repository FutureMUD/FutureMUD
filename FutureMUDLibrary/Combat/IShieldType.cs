using MudSharp.Body.Traits;
using MudSharp.Framework;

namespace MudSharp.Combat {
    public interface IShieldType : IFrameworkItem {
        ITraitDefinition BlockTrait { get; }
        double BlockBonus { get; }
        double StaminaPerBlock { get; }
        IArmourType EffectiveArmourType { get; }
    }
}