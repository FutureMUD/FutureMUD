#nullable enable
using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class Spell$safeitemrootname$Effect : MagicSpellEffectBase
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("Spell$safeitemrootname$", (effect, owner) => new Spell$safeitemrootname$Effect(effect, owner));
	}

	public Spell$safeitemrootname$Effect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg prog)
		: base(owner, parent, prog)
	{
	}

	protected Spell$safeitemrootname$Effect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var trueRoot = root.Element("Effect");
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0)
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "A spell effect.";
	}

	protected override string SpecificEffectType => "Spell$safeitemrootname$";
}
