#nullable enable

using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Interfaces;

public enum IllusionAudienceScope
{
	Caster = 0,
	Target = 1,
	Everyone = 2,
	SameCell = 3,
	SameZone = 4,
	Party = 5,
	Clan = 6
}

public interface IIllusionEffect : IEffectSubtype
{
	string IllusionKey { get; }
	int IllusionPriority { get; }
	IllusionAudienceScope AudienceScope { get; }
	long CasterId { get; }
	long TargetId { get; }
	long? ClanId { get; }
	IFutureProg? ViewerProg { get; }
	bool IllusionApplies(IPerceiver voyeur);
}
