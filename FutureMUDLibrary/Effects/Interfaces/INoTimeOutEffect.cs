#nullable enable

namespace MudSharp.Effects.Interfaces;

public interface INoTimeOutEffect : IEffectSubtype
{
	string NoTimeOutReason { get; }
}
