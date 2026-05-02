using MudSharp.Form.Material;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Interfaces;

public interface ISilencedEffect : IEffectSubtype
{
}

public interface ISleepEffect : IEffectSubtype
{
}

public interface IFearEffect : IEffectSubtype
{
}

public interface IFlightEffect : IEffectSubtype
{
}

public interface IPreventFallingEffect : IEffectSubtype
{
}

public interface ILevitationEffect : IPreventFallingEffect
{
}

public interface IFallDamageMitigationEffect : IEffectSubtype
{
	double FallDistanceMultiplier { get; }
	double FallDamageMultiplier { get; }
}

public interface IAdditionalBreathableFluidEffect : IEffectSubtype
{
	bool AppliesToFluid(IFluid fluid);
}

public interface IDarksightEffect : IEffectSubtype
{
	Difficulty MinimumEffectiveDifficulty { get; }
}

public interface IComprehendLanguageEffect : IEffectSubtype
{
}

public interface ICurseEffect : IEffectSubtype
{
}
