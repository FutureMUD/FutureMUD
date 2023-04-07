using System.Globalization;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp.Effects.Concrete;

public class BodypartExcessivelyPainful : Effect, IBodypartIneffectiveEffect, ILimbIneffectiveEffect
{
	public BodypartExcessivelyPainful(IPerceivable owner, IBodypart bodypart,
		IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
		Bodypart = bodypart;
	}

	public BodypartExcessivelyPainful(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		Bodypart = Gameworld.BodypartPrototypes.Get(long.Parse(effect.Element("Bodypart").Value));
	}

	#region Implementation of IBodypartIneffectiveEffect

	public IBodypart Bodypart { get; set; }

	#endregion

	#region Overrides of Effect

	/// <summary>
	///     Fires when an effect is removed, including a matured scheduled effect
	/// </summary>
	public override void RemovalEffect()
	{
		base.RemovalEffect();
		Owner.Send($"You feel as if your {Bodypart.ShortDescription()} is no longer too painful to use.");
	}

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"{Owner.HowSeen(voyeur, true, DescriptionType.Possessive)} {Bodypart.FullDescription()} is too painful to be useful.";
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("BodypartExcessivelyPainful",
			(effect, owner) => new BodypartExcessivelyPainful(effect, owner));
	}

	protected override string SpecificEffectType { get; } = "BodypartExcessivelyPainful";

	protected override XElement SaveDefinition()
	{
		return new XElement("Bodypart", Bodypart.Id);
	}

	public override bool Applies(object target)
	{
		return base.Applies(target) && target == Bodypart;
	}

	#endregion

	#region Implementation of ILimbIneffectiveEffect

	public ILimb Limb => (Owner as IBody)?.GetLimbFor(Bodypart);

	public bool AppliesToLimb(ILimb limb)
	{
		return limb == Limb;
	}

	public LimbIneffectiveReason Reason => LimbIneffectiveReason.Pain;

	#endregion
}