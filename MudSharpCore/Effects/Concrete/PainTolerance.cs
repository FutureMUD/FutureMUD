using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Health;

namespace MudSharp.Effects.Concrete;

public class PainTolerance : Effect, IPreventPassOut
{
	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"Pain tolerance, removed at {Floor:N2} pain or {ExertionCap.Describe()} exertion.";
	}

	protected override string SpecificEffectType => "PainTolerance";

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
		if (wounded.Wounds.Sum(x => x.CurrentPain) <= Floor)
		{
			RemovalEffect();
		}
	}

	private void Owner_OnHeal(IMortalPerceiver wounded, IWound wound)
	{
		if (wounded.Wounds.Sum(x => x.CurrentPain) <= Floor)
		{
			RemovalEffect();
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
		if (wound.CurrentPain > 0.0)
		{
			RemovalEffect();
		}
	}

	public override void RemovalEffect()
	{
		Owner.RemoveEffect(this, false);
		ReleaseEvents();
	}

	public ExertionLevel ExertionCap { get; set; }

	public double Floor { get; set; }

	public static void InitialiseEffectType()
	{
		RegisterFactory("PainTolerance", (effect, owner) => new PainTolerance(effect, owner));
	}

	public void LoadFromXml(XElement root)
	{
		Floor = double.Parse(root.Element("Floor").Value);
		ExertionCap = (ExertionLevel)int.Parse(root.Element("ExertionCap").Value);
	}

	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("Floor", Floor),
			new XElement("ExertionCap", (int)ExertionCap)
		);
	}

	#endregion

	public PainTolerance(IBody owner, double floor, ExertionLevel exertionCap, IFutureProg applicabilityProg = null) :
		base(owner, applicabilityProg)
	{
		Floor = floor;
		ExertionCap = exertionCap;
		RegisterEvents(owner);
	}

	public PainTolerance(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		LoadFromXml(effect.Element("Effect"));
	}
}