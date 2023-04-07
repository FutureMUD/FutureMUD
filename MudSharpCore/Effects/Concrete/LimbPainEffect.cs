using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp.Effects.Concrete;

public class LimbPainEffect : Effect, ILimbIneffectiveEffect
{
	public LimbPainEffect(IPerceivable owner, ILimb limb, IFutureProg applicabilityProg = null)
		: base(owner, applicabilityProg)
	{
		Limb = limb;
	}

	public LimbPainEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		LoadFromXml(effect.Element("Effect"));
	}

	protected override string SpecificEffectType => "LimbPainEffect";

	public bool AppliesToLimb(ILimb limb)
	{
		return Limb == limb;
	}

	public ILimb Limb { get; set; }
	public LimbIneffectiveReason Reason => LimbIneffectiveReason.Pain;

	public override string Describe(IPerceiver voyeur)
	{
		return $"{Owner.HowSeen(voyeur, true)} has an extremely painful {Limb.Name}.";
	}

	private void LoadFromXml(XElement root)
	{
		Limb = Gameworld.Limbs.Get(long.Parse(root.Element("Limb").Value));
	}

	#region Overrides of Effect

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect", new XElement("Limb", Limb.Id));
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("LimbPainEffect", (effect, owner) => new LimbPainEffect(effect, owner));
	}

	/// <summary>
	///     Fires when an effect is removed, including a matured scheduled effect
	/// </summary>
	public override void RemovalEffect()
	{
		base.RemovalEffect();
		Owner.Send($"You feel as if your {Limb.Name} is no longer too painful to use.");
	}

	#endregion
}