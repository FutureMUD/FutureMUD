using System.Linq;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete;

public class LimbSeveredEffect : Effect, ILimbIneffectiveEffect
{
	public IBody BodyOwner { get; protected set; }

	public LimbSeveredEffect(IBody owner, ILimb limb, IFutureProg applicabilityProg = null)
		: base(owner, applicabilityProg)
	{
		Limb = limb;
		BodyOwner = owner;
	}

	public LimbSeveredEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		BodyOwner = (IBody)owner;
		LoadFromXml(effect.Element("Effect"));
	}

	protected override string SpecificEffectType => "LimbSeveredEffect";

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
			                .All(x => !x.TargetBodypart.DownstreamOfPart(Limb.RootBodypart) &&
			                          x.TargetBodypart != Limb.RootBodypart);
		}

		return base.Applies(target);
	}

	#endregion

	public ILimb Limb { get; set; }
	public LimbIneffectiveReason Reason => LimbIneffectiveReason.Severing;

	public override string Describe(IPerceiver voyeur)
	{
		return $"{Owner.HowSeen(voyeur, true)} has a severed {Limb.Name}.";
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
		RegisterFactory("LimbSeveredEffect", (effect, owner) => new LimbSeveredEffect(effect, owner));
	}

	#endregion
}