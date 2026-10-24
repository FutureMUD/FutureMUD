#nullable enable
using MudSharp.Magic;
using MudSharp.Planes;
using MudSharp.Magic.Powers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public sealed class PsionicVanishEffect : PsionicSustainedPowerEffectBase<PsionicVanishPower>
{
	private readonly Dictionary<long, (DateTime Until, bool Hidden)> _observers = [];
	public MagicPowerDistance Distance { get; }
	public PsionicVanishEffect(ICharacter owner, PsionicVanishPower power, MagicPowerDistance distance) : base(owner, power) => Distance = distance;
	private PsionicVanishEffect(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		Distance = (MagicPowerDistance)(int)xml.Element("Effect")!.Element("Distance")!;
		if (Distance is not (MagicPowerDistance.SameLocationOnly or MagicPowerDistance.AdjacentLocationsOnly or MagicPowerDistance.SameZoneOnly or MagicPowerDistance.SameShardOnly))
			throw new ApplicationException("Invalid saved Vanish range.");
	}
	public static void InitialiseEffectType() => RegisterFactory("PsionicVanish", (xml, owner) => new PsionicVanishEffect(xml, owner));
	protected override string SpecificEffectType => "PsionicVanish";
	protected override XElement SaveDefinition() => SaveToXml(new XElement("Power", Power.Id), new XElement("Distance", (int)Distance));
	public override string Describe(IPerceiver voyeur) => $"Psionically vanished ({Distance.DescribeEnum()}).";
	public override PerceptionTypes Obscuring => PerceptionTypes.DirectVisual;
	public override bool Applies(object target, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		if (target is not ICharacter observer || observer == Owner || observer.IsAdministrator() ||
			flags.HasFlag(PerceiveIgnoreFlags.IgnoreHiding) || !base.Applies(target) ||
			!observer.SharesPlaneWith(CharacterOwner) || observer.Location is null || CharacterOwner.Location is null ||
			!Power.TargetIsInRange(CharacterOwner, observer, Distance) || PsionicSensePresenceEffect.Reveals(observer, CharacterOwner)) return false;
		var identity = CharacterInstanceIdentityComparer.IdentityId(observer);
		if (_observers.TryGetValue(identity, out var result) && result.Until > RuntimeClock.UtcNow) return result.Hidden;
		var conceal = Gameworld.GetCheck(CheckType.MagicTelepathyCheck).Check(CharacterOwner, Power.SkillCheckDifficulty, Power.SkillCheckTrait, observer);
		var notice = Gameworld.GetCheck(CheckType.MagicTelepathyCheck).Check(observer, Power.SkillCheckDifficulty, CharacterOwner);
		var hidden = conceal.Outcome >= Power.MinimumSuccessThreshold && new OpposedOutcome(conceal, notice).Outcome == OpposedOutcomeDirection.Proponent;
		if (_observers.Count >= 256) _observers.Remove(_observers.Keys.First());
		_observers[identity] = (RuntimeClock.UtcNow.AddSeconds(30), hidden);
		return hidden;
	}
}
