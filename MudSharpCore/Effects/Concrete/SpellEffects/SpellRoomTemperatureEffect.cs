using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.Effects.Interfaces;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellRoomTemperatureEffect : MagicSpellEffectBase, IDescriptionAdditionEffect,
	IAffectEnvironmentalTemperature
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellRoomTemperature", (effect, owner) => new SpellRoomTemperatureEffect(effect, owner));
	}

	public SpellRoomTemperatureEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg prog,
		double temperatureDelta, string descAddendum, ANSIColour addendumColour) : base(owner, parent, prog)
	{
		TemperatureDelta = temperatureDelta;
		DescAddendum = descAddendum;
		AddendumColour = addendumColour;
	}

	protected SpellRoomTemperatureEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		TemperatureDelta = double.Parse(trueRoot.Element("TemperatureDelta").Value);
		DescAddendum = trueRoot.Element("DescAddendum").Value;
		AddendumColour = Telnet.GetColour(trueRoot.Element("AddendumColour").Value);
	}

	#region Overrides of Effect

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			new XElement("TemperatureDelta", TemperatureDelta),
			new XElement("DescAddendum", new XCData(DescAddendum)),
			new XElement("AddendumColour", AddendumColour.Name)
		);
	}

	#endregion

	public string DescAddendum { get; set; }
	public ANSIColour AddendumColour { get; set; }

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Room Temperature - {Gameworld.UnitManager.DescribeBonus(TemperatureDelta, Framework.Units.UnitType.TemperatureDelta, voyeur).ColourValue()} - {GetAdditionalText(voyeur, true)}";
	}

	protected override string SpecificEffectType => "SpellRoomTemperature";

	#endregion

	#region Implementation of IDescriptionAdditionEffect

	public string GetAdditionalText(IPerceiver voyeur, bool colour)
	{
		if (string.IsNullOrEmpty(DescAddendum))
		{
			return string.Empty;
		}

		return colour ? DescAddendum.Colour(AddendumColour) : DescAddendum;
	}

	public bool PlayerSet => false;

	#endregion

	#region Implementation of IAffectEnvironmentalTemperature

	public double TemperatureDelta { get; private set; }

	#endregion
}