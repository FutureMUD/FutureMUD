#nullable enable

using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

/// <summary>Persistent provenance for finite psychic effects, including detection and ordinary dispelling.</summary>
public abstract class TimedPsychicEffect : Effect, IMagicEffect
{
	protected TimedPsychicEffect(IPerceivable owner, IMagicPower? power = null) : base(owner) => OriginPowerId = power?.Id ?? 0;
	protected TimedPsychicEffect(XElement xml, IPerceivable owner) : base(xml, owner) => OriginPowerId = (long?)xml.Element("Effect")?.Attribute("originPower") ?? 0;
	public long OriginPowerId { get; set; }
	public IMagicPower PowerOrigin => Gameworld.MagicPowers.Get(OriginPowerId)!;
	public IMagicSchool School => PowerOrigin?.School!;
	public Difficulty DetectMagicDifficulty => Difficulty.Normal;
	protected XElement WithOrigin(XElement definition)
	{
		definition.SetAttributeValue("originPower", OriginPowerId);
		return definition;
	}
}
