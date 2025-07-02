using MudSharp.Combat;

namespace MudSharp.Combat.ScatterStrategies;

public static class RangedScatterStrategyFactory
{
    public static IRangedScatterStrategy GetStrategy(RangedScatterType type)
    {
        return type switch
        {
            RangedScatterType.Arcing => ArcingScatterStrategy.Instance,
            RangedScatterType.Ballistic => BallisticScatterStrategy.Instance,
            RangedScatterType.Light => LightScatterStrategy.Instance,
            RangedScatterType.Spread => SpreadScatterStrategy.Instance,
            _ => ArcingScatterStrategy.Instance
        };
    }

    public static IRangedScatterStrategy GetStrategy(IRangedWeaponType type)
    {
        return GetStrategy(type.RangedWeaponType switch
        {
            RangedWeaponType.Firearm => RangedScatterType.Ballistic,
            RangedWeaponType.ModernFirearm => RangedScatterType.Ballistic,
            RangedWeaponType.Musket => RangedScatterType.Ballistic,
            RangedWeaponType.Laser => RangedScatterType.Light,
            _ => RangedScatterType.Arcing
        });
    }
}
