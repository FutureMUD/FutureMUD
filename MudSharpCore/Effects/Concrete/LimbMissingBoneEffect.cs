using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

public class LimbMissingBoneEffect : Effect, ILimbIneffectiveEffect
{
	public LimbMissingBoneEffect(IBody owner, ILimb limb) : base(owner)
	{
		Limb = limb;
		BodyOwner = owner;
	}

	protected LimbMissingBoneEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		BodyOwner = (IBody)owner;
		LoadFromXml(effect.Element("Effect"));
	}

	protected override string SpecificEffectType => "LimbMissingBoneEffect";

	public bool AppliesToLimb(ILimb limb)
	{
		return Limb == limb;
	}

	#region Overrides of Effect

	public override bool Applies(object target)
	{
		if (target is ILimb limb)
		{
			return Limb == limb &&
			       BodyOwner.Prosthetics.Where(x => x.Functional)
			                .All(x => !x.TargetBodypart.DownstreamOfPart(Limb.RootBodypart));
		}

		return base.Applies(target);
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect", new XElement("Limb", Limb.Id));
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("LimbMissingBoneEffect", (effect, owner) => new LimbMissingBoneEffect(effect, owner));
	}

	#endregion

	public ILimb Limb { get; set; }
	public IBody BodyOwner { get; protected set; }
	public LimbIneffectiveReason Reason => LimbIneffectiveReason.Boneless;

	public override string Describe(IPerceiver voyeur)
	{
		return $"{Owner.HowSeen(voyeur, true)} has a major bone missing from {Limb.Name}.";
	}

	private void LoadFromXml(XElement root)
	{
		Limb = Gameworld.Limbs.Get(long.Parse(root.Element("Limb").Value));
	}
}