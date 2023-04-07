using System.Xml.Linq;
using MudSharp.Body.Needs;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class DelayedNeedsFulfillment : Effect, IEffectSubtype
{
	public DelayedNeedsFulfillment(IPerceivable owner, NeedFulfiller payload)
		: base(owner)
	{
		Payload = payload;
	}

	protected DelayedNeedsFulfillment(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
		var definition = effect.Element("Effect");
		Payload = new NeedFulfiller
		{
			Calories = double.Parse(definition.Attribute("Calories").Value),
			SatiationPoints = double.Parse(definition.Attribute("SatiationPoints").Value),
			ThirstPoints = double.Parse(definition.Attribute("ThirstPoints").Value),
			WaterLitres = double.Parse(definition.Attribute("WaterLitres").Value),
			AlcoholLitres = double.Parse(definition.Attribute("AlcoholLitres").Value)
		};
	}

	public NeedFulfiller Payload { get; set; }

	public override bool SavingEffect => true;

	protected override string SpecificEffectType => "DelayedNeedsFulfillment";

	public static void InitialiseEffectType()
	{
		RegisterFactory("DelayedNeedsFulfillment", (effect, owner) => new DelayedNeedsFulfillment(effect, owner));
	}

	protected override XElement SaveDefinition()
	{
		return
			new XElement("Effect", new XAttribute("SatiationPoints", Payload.SatiationPoints),
				new XAttribute("ThirstPoints", Payload.ThirstPoints),
				new XAttribute("AlcoholLitres", Payload.AlcoholLitres),
				new XAttribute("WaterLitres", Payload.WaterLitres), new XAttribute("Calories", Payload.Calories));
	}

	public override void ExpireEffect()
	{
		var needs = Owner as IHaveNeeds;
		needs?.FulfilNeeds(Payload, true);

		Owner.RemoveEffect(this);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Delayed Needs Fulfillment - {Payload.Calories:N0} {Payload.WaterLitres:N4} {Payload.SatiationPoints:N2} {Payload.ThirstPoints:N2} {Payload.AlcoholLitres:N4}";
	}

	public override string ToString()
	{
		return
			$"Delayed Needs Fulfillment - {Payload.Calories:N0} {Payload.WaterLitres:N4} {Payload.SatiationPoints:N2} {Payload.ThirstPoints:N2} {Payload.AlcoholLitres:N4}";
	}
}