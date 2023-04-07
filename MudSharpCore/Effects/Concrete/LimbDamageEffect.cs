using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp.Effects.Concrete;

public class LimbDamageEffect : Effect, ILimbIneffectiveEffect
{
	public LimbDamageEffect(IPerceivable owner, ILimb limb, IFutureProg applicabilityProg = null)
		: base(owner, applicabilityProg)
	{
		Limb = limb;
	}

	public LimbDamageEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		LoadFromXml(effect.Element("Effect"));
	}

	protected override string SpecificEffectType => "LimbDamageEffect";

	public bool AppliesToLimb(ILimb limb)
	{
		return Limb == limb;
	}

	public ILimb Limb { get; set; }
	public LimbIneffectiveReason Reason => LimbIneffectiveReason.Damage;

	public override string Describe(IPerceiver voyeur)
	{
		return $"{Owner.HowSeen(voyeur, true)} has a critically damaged {Limb.Name}.";
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
		RegisterFactory("LimbDamageEffect", (effect, owner) => new LimbDamageEffect(effect, owner));
	}

	/// <summary>
	///     Fires when an effect is removed, including a matured scheduled effect
	/// </summary>
	public override void RemovalEffect()
	{
		base.RemovalEffect();
		Owner.Send($"You feel as if your {Limb.Name} is no longer too damaged to use.");
	}

	#endregion
}