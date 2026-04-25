namespace MudSharp.Effects.Interfaces;

public interface IZeroGravityAnchorEffect : IEffectSubtype
{
	bool AllowsZeroGravityPushOff { get; }
}
