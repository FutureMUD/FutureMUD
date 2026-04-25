using MudSharp.Construction;

namespace MudSharp.Effects.Interfaces;

public interface IGravityOverrideEffect : IEffectSubtype
{
	GravityModel GravityModel { get; }
	int Priority { get; }
}
