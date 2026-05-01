#nullable enable

using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellCorpsePreservationEffect : SimpleSpellStatusEffectBase, ICorpsePreservationEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellCorpsePreservation", (effect, owner) => new SpellCorpsePreservationEffect(effect, owner));
	}

	public SpellCorpsePreservationEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
	}

	private SpellCorpsePreservationEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
	}

	public bool PreserveCorpse => true;

	public override string Describe(IPerceiver voyeur)
	{
		return "Magically preserved from corpse decay.";
	}

	protected override string SpecificEffectType => "SpellCorpsePreservation";
}
