using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellExitBarrierEffect : MagicSpellEffectBase, IExitBarrierEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellExitBarrier", (effect, owner) => new SpellExitBarrierEffect(effect, owner));
	}

	public SpellExitBarrierEffect(IPerceivable owner, IMagicSpellEffectParent parent)
		: base(owner, parent, null)
	{
	}

	protected SpellExitBarrierEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Exit Barrier - {Spell.Name.Colour(Spell.School.PowerListColour)}";
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect");
	}

	protected override string SpecificEffectType => "SpellExitBarrier";
}
