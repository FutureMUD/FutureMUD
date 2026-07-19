#nullable enable

using MudSharp.Construction;
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public sealed class MagicAllspeakEffect : PsionicSustainedPowerEffectBase<AllspeakPower>, IComprehendLanguageEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("MagicAllspeak", (effect, owner) => new MagicAllspeakEffect(effect, owner));
	}

	public MagicAllspeakEffect(ICharacter owner, AllspeakPower power) : base(owner, power)
	{
	}

	private MagicAllspeakEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
	}

	protected override string SpecificEffectType => "MagicAllspeak";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Comprehending languages via the {Power.Name.Colour(Power.School.PowerListColour)} power.";
	}
}

