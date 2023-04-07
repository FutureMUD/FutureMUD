using System;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Units;

namespace MudSharp.Effects.Concrete;

public class HarmfulBloodAdditive : Effect, IHarmfulBloodAdditiveEffect
{
	private double _volume;

	public HarmfulBloodAdditive(IBody owner, LiquidInjectionConsequence consequence, double volume) : base(owner)
	{
		Consequence = consequence;
		_volume = volume;
	}

	public HarmfulBloodAdditive(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		LoadFromXml(effect.Element("Effect"));
	}

	protected override string SpecificEffectType => "HarmfulBloodAdditive";
	public LiquidInjectionConsequence Consequence { get; set; }

	public double Volume
	{
		get => _volume;

		set
		{
			_volume = value;
			Changed = true;
		}
	}

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"{Owner.HowSeen(voyeur, true, DescriptionType.Possessive)} blood contains {Gameworld.UnitManager.DescribeExact(Volume, UnitType.FluidVolume, voyeur)} of harmful contaminants with consequence {Consequence.Describe()}.";
	}

	public override bool SavingEffect => true;

	private void LoadFromXml(XElement element)
	{
		Consequence =
			(LiquidInjectionConsequence)
			Enum.Parse(typeof(LiquidInjectionConsequence), element.Attribute("consequence").Value);
		_volume = double.Parse(element.Attribute("volume").Value);
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("HarmfulBloodAdditive", (effect, owner) => new HarmfulBloodAdditive(effect, owner));
	}

	protected override XElement SaveDefinition()
	{
		return
			new XElement("Effect", new XAttribute("consequence", Consequence), new XAttribute("volume", Volume))
			;
	}
}