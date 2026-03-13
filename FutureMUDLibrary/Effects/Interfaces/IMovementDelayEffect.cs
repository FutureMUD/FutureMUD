#nullable enable

using MudSharp.Movement;

namespace MudSharp.Effects.Interfaces;

public interface IMovementDelayEffect : IEffectSubtype
{
	double DelayMultiplier(IMoveSpeed speed);
}
