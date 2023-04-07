using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellGlowEffect : MagicSpellEffectBase, IDescriptionAdditionEffect, ISDescAdditionEffect,
	IProduceIllumination
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellGlow", (effect, owner) => new SpellGlowEffect(effect, owner));
	}

	public SpellGlowEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg prog, double glowLux,
		string sDescAddendum, string descAddendum, ANSIColour glowAddendumColour) : base(owner, parent, prog)
	{
		GlowLux = glowLux;
		SDescAddendum = sDescAddendum;
		DescAddendum = descAddendum;
		GlowAddendumColour = glowAddendumColour;
	}

	protected SpellGlowEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		GlowLux = double.Parse(trueRoot.Element("GlowLux").Value);
		SDescAddendum = trueRoot.Element("SDescAddendum").Value;
		DescAddendum = trueRoot.Element("DescAddendum").Value;
		GlowAddendumColour = Telnet.GetColour(trueRoot.Element("GlowAddendumColour").Value);
	}

	#region Overrides of Effect

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			new XElement("GlowLux", GlowLux),
			new XElement("SDescAddendum", new XCData(SDescAddendum)),
			new XElement("DescAddendum", new XCData(DescAddendum)),
			new XElement("GlowAddendumColour", GlowAddendumColour.Name)
		);
	}

	#endregion

	public double GlowLux { get; set; }
	public string SDescAddendum { get; set; }
	public string DescAddendum { get; set; }
	public ANSIColour GlowAddendumColour { get; set; }

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"Glowing - {GlowLux.ToString("N3", voyeur).ColourValue()} lux - {GetAddendumText(true)}";
	}

	protected override string SpecificEffectType => "SpellGlow";

	#endregion

	#region Implementation of IDescriptionAdditionEffect

	public string GetAdditionalText(IPerceiver voyeur, bool colour)
	{
		return colour ? DescAddendum.Colour(GlowAddendumColour) : DescAddendum;
	}

	public bool PlayerSet => false;

	#endregion

	#region Implementation of ISDescAdditionEffect

	public string AddendumText => SDescAddendum;

	public string GetAddendumText(bool colour)
	{
		return colour ? SDescAddendum.Colour(GlowAddendumColour) : SDescAddendum;
	}

	#endregion

	#region Implementation of IProduceIllumination

	public double ProvidedLux => GlowLux;

	#endregion
}