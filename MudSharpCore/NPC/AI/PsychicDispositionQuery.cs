#nullable enable

using MudSharp.Effects.Concrete;

namespace MudSharp.NPC.AI;

/// <summary>Disposition influences discretionary choices only. Legal duties remain authoritative.</summary>
public static class PsychicDispositionQuery
{
	public static bool HasLegalDuties(ICharacter actor) => actor.Gameworld.LegalAuthorities.Any(x => x.GetEnforcementAuthority(actor) is not null);
	public static double ForDecision(ICharacter actor, ICharacter subject) => HasLegalDuties(actor) ? 0 : PsychicEmotionEffect.Disposition(actor, subject);
	public static bool WillCooperate(ICharacter actor, ICharacter subject, bool permitted) => permitted && ForDecision(actor, subject) >= 0;
}
