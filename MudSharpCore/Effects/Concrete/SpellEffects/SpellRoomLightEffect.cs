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

public class SpellRoomLightEffect : MagicSpellEffectBase, IDescriptionAdditionEffect, IAreaLightEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellRoomLight", (effect, owner) => new SpellRoomLightEffect(effect, owner));
	}

	public SpellRoomLightEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg prog, double glowLux,
		string descAddendum, ANSIColour glowAddendumColour) : base(owner, parent, prog)
	{
		GlowLux = glowLux;
		DescAddendum = descAddendum;
		GlowAddendumColour = glowAddendumColour;
	}

	protected SpellRoomLightEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		GlowLux = double.Parse(trueRoot.Element("GlowLux").Value);
		DescAddendum = trueRoot.Element("DescAddendum").Value;
		GlowAddendumColour = Telnet.GetColour(trueRoot.Element("GlowAddendumColour").Value);
	}

	#region Overrides of Effect

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			new XElement("GlowLux", GlowLux),
			new XElement("DescAddendum", new XCData(DescAddendum)),
			new XElement("GlowAddendumColour", GlowAddendumColour.Name)
		);
	}

	#endregion

	public double GlowLux { get; set; }
	public string DescAddendum { get; set; }
	public ANSIColour GlowAddendumColour { get; set; }

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"Room Light - {GlowLux.ToString("N3", voyeur).ColourValue()} lux - {GetAdditionalText(voyeur, true)}";
	}

	protected override string SpecificEffectType => "SpellGlow";

	#endregion

	#region Implementation of IDescriptionAdditionEffect

	public string GetAdditionalText(IPerceiver voyeur, bool colour)
	{
		if (string.IsNullOrEmpty(DescAddendum))
		{
			return string.Empty;
		}

		return colour ? DescAddendum.Colour(GlowAddendumColour) : DescAddendum;
	}

	public bool PlayerSet => false;

	#endregion

	#region Implementation of IAreaLightEffect

	public double AddedLight => GlowLux;

	#endregion
}