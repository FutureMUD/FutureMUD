using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Effects.Concrete;

public class LimbSpinalDamageEffect : Effect, ILimbIneffectiveEffect
{
	public IBody BodyOwner { get; protected set; }

	public LimbSpinalDamageEffect(IBody owner, ILimb limb) : base(owner)
	{
		BodyOwner = owner;
		Limb = limb;
	}

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"{Owner.HowSeen(voyeur, true, Form.Shape.DescriptionType.Possessive)} {Limb.Name} has non-functioning spinal cords.";
	}

	protected override string SpecificEffectType => "LimbSpinalDamageEffect";

	public ILimb Limb { get; set; }

	public bool AppliesToLimb(ILimb limb)
	{
		return Limb == limb;
	}

	public LimbIneffectiveReason Reason => LimbIneffectiveReason.SpinalDamage;
}