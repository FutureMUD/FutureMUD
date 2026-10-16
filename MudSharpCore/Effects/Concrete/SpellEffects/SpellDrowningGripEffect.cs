#nullable enable

using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Magic;

namespace MudSharp.Effects.Concrete.SpellEffects;

/// <summary>Saved through its spell parent; native breathing strategies observe IStopBreathing.</summary>
public sealed class SpellDrowningGripEffect : SimpleSpellStatusEffectBase, IStopBreathing
{
	public static void InitialiseEffectType() =>
		RegisterFactory("SpellDrowningGrip", (effect, owner) => new SpellDrowningGripEffect(effect, owner));

	public SpellDrowningGripEffect(IPerceivable owner, IMagicSpellEffectParent parent) : base(owner, parent) { }

	private SpellDrowningGripEffect(XElement root, IPerceivable owner) : base(root, owner) { }

	public override string Describe(IPerceiver voyeur) => "Magical pressure interrupts breathing.";

	protected override string SpecificEffectType => "SpellDrowningGrip";
}
