using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Health;

namespace MudSharp.Effects.Concrete;

public class StablisedOrganFunction : Effect, IStablisedOrganFunction
{
	public double Grace { get; protected set; }

	protected StablisedOrganFunction(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		LoadFromXml(effect.Element("Effect"));
	}

	public StablisedOrganFunction(IBody owner, IOrganProto organ, double floor, ExertionLevel exertionCap,
		double grace = 0.0, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
		Organ = organ;
		Floor = floor;
		ExertionCap = exertionCap;
		Grace = grace;
		RegisterEvents(owner);
	}

	public override void Login()
	{
		RegisterEvents((IBody)Owner);
	}

	protected void RegisterEvents(IBody owner)
	{
		owner.OnWounded += Owner_OnWounded;
		owner.OnExertionChanged += Owner_OnExertionChanged;
		owner.OnHeal += Owner_OnHeal;
		owner.OnRemoveWound += Owner_OnRemoveWound;
	}

	private void Owner_OnRemoveWound(IMortalPerceiver wounded, IWound wound)
	{
		if (wound.Bodypart == Organ)
		{
			if (Organ.OrganFunctionFactor((IBody)Owner) > Floor)
			{
				RemovalEffect();
			}
		}
	}

	private void Owner_OnHeal(IMortalPerceiver wounded, IWound wound)
	{
		if (wound.Bodypart == Organ)
		{
			if (Organ.OrganFunctionFactor((IBody)Owner) > Floor)
			{
				RemovalEffect();
			}
		}
	}

	private void Owner_OnExertionChanged(ExertionLevel oldExertion, ExertionLevel newExertion)
	{
		if (newExertion > ExertionCap)
		{
			RemovalEffect();
		}
	}

	protected void ReleaseEvents()
	{
		var owner = (IBody)Owner;
		owner.OnWounded -= Owner_OnWounded;
		owner.OnExertionChanged -= Owner_OnExertionChanged;
		owner.OnHeal -= Owner_OnHeal;
		owner.OnRemoveWound -= Owner_OnRemoveWound;
	}

	private void Owner_OnWounded(IMortalPerceiver wounded, IWound wound)
	{
		if (wound.Bodypart == Organ)
		{
			if (!wound.DamageType.In(DamageType.Cellular, DamageType.Hypoxia) || wound.CurrentDamage > Grace)
			{
				RemovalEffect();
			}
		}
	}

	public override void RemovalEffect()
	{
		Owner.RemoveEffect(this, false);
		ReleaseEvents();
	}

	public ExertionLevel ExertionCap { get; set; }

	public double Floor { get; set; }

	public IOrganProto Organ { get; set; }

	public IBodypart Bodypart => Organ;

	public static void InitialiseEffectType()
	{
		RegisterFactory("StabilisedOrganFunction", (effect, owner) => new StablisedOrganFunction(effect, owner));
	}

	public void LoadFromXml(XElement root)
	{
		Organ = (IOrganProto)Gameworld.BodypartPrototypes.Get(long.Parse(root.Element("Organ").Value));
		Floor = double.Parse(root.Element("Floor").Value);
		ExertionCap = (ExertionLevel)int.Parse(root.Element("ExertionCap").Value);
		Grace = double.Parse(root.Element("Grace")?.Value ?? "0.0");
	}

	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("Organ", Organ.Id),
			new XElement("Floor", Floor),
			new XElement("ExertionCap", (int)ExertionCap),
			new XElement("Grace", Grace)
		);
	}

	protected override string SpecificEffectType => "StabilisedOrganFunction";

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"{Organ.FullDescription()} function floor of {Floor:P3} with exertion cap of {ExertionCap.Describe()} and grace of {Grace:N0}";
	}
}