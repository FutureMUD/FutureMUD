using MudSharp.Body.Traits;
using MudSharp.Framework;
using MudSharp.Framework.Revision;

namespace MudSharp.Combat {
    public interface IShieldType : IEditableItem {
        ITraitDefinition BlockTrait { get; }
        double BlockBonus { get; }
        double StaminaPerBlock { get; }
        IArmourType EffectiveArmourType { get; }
        IShieldType Clone(string name);
    }
}