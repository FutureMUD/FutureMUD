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

public class SpellInvisibilityEffect : MagicSpellEffectBase
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellInvisibility", (effect, owner) => new SpellInvisibilityEffect(effect, owner));
	}

	public SpellInvisibilityEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg prog) : base(owner,
		parent, prog)
	{
	}

	protected SpellInvisibilityEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var trueRoot = root.Element("Effect");
	}

	#region Overrides of Effect

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0)
		);
	}

	#endregion

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"Invisible - Filter: {ApplicabilityProg?.MXPClickableFunctionName() ?? "None".Colour(Telnet.Red)}";
	}

	protected override string SpecificEffectType => "SpellInvisibility";

	public override PerceptionTypes Obscuring => PerceptionTypes.DirectVisual;

	#endregion
}