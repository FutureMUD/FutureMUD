#nullable enable

using MudSharp.Character;

namespace MudSharp.Effects.Interfaces;

public interface IIdentifyLookEffect : IEffectSubtype
{
	long CasterId { get; }
	long CasterInstanceId { get; }
	long IdentifyProgId { get; }
	string? GetLookText(ICharacter target);
}

public interface IReciteProxyEffect : IEffectSubtype
{
	long CasterId { get; }
	long CasterInstanceId { get; }
	long LinkedCharacterId { get; }
	long LinkedInstanceId { get; }
	double RelayChance { get; }
	string ReciteEcho { get; }
}
