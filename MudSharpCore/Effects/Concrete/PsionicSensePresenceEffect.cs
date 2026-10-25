#nullable enable
using MudSharp.Magic.Powers;
using MudSharp.Planes;

namespace MudSharp.Effects.Concrete;

public sealed class PsionicSensePresenceEffect : PsionicSustainedPowerEffectBase<PsionicSensePresencePower>
{
	public PsionicSensePresenceEffect(ICharacter owner, PsionicSensePresencePower power) : base(owner, power) { }
	private PsionicSensePresenceEffect(XElement xml, IPerceivable owner) : base(xml, owner) { }
	public static void InitialiseEffectType() => RegisterFactory("PsionicSensePresence", (xml, owner) => new PsionicSensePresenceEffect(xml, owner));
	protected override string SpecificEffectType => "PsionicSensePresence";
	public override string Describe(IPerceiver voyeur) => "Sensing nearby minds.";
	public static double SneakSpottingBonus(ICharacter observer, ICharacter mover) =>
		mover.Capabilities.Any(x => x.AllPowers.Any(p => p.IsPsionic))
			? observer.CombinedEffectsOfType<PsionicSensePresenceEffect>().Where(x => x.Applies())
				.Select(x => Math.Clamp(observer.TraitValue(x.Power.SkillCheckTrait), 0, 100)).DefaultIfEmpty(0).Max()
			: 0;
	public static bool Reveals(ICharacter observer, ICharacter target) => observer.Location == target.Location &&
		observer.SharesPlaneWith(target) && observer.CombinedEffectsOfType<PsionicSensePresenceEffect>().Any(x => x.Applies()) &&
		!target.CombinedEffectsOfType<MindConcealmentEffect>().Any(x => x.ConcealsIdentityFrom(target, observer, x.School));
}
