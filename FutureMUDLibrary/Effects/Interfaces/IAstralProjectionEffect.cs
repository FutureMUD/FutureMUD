using MudSharp.Effects;

#nullable enable

namespace MudSharp.Effects.Interfaces;

public enum AstralProjectionAnchorPolicy
{
	None = 0,
	Helpless = 1,
	Sleep = 2,
	Stasis = 3
}

public interface IAstralProjectionEffect : IEffectSubtype
{
	long AnchorInstanceId { get; }
	long ProjectionInstanceId { get; }
	long ProjectionBodyId { get; }
	long PlaneId { get; }
	AstralProjectionAnchorPolicy AnchorPolicy { get; }
}
