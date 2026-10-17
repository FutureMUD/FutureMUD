#nullable enable

using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>Reads only the cast target for its exact caster; ordinary casting owns resistance and lifetime.</summary>
public sealed class ReadAttunementSpellEffect : CharacterSpellEffectTemplateBase, IMagicSpellEffectOperation
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("readattunement", (root, spell) => new ReadAttunementSpellEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("readattunement",
			(commands, spell) => (new ReadAttunementSpellEffect(new XElement("Effect", new XAttribute("type", "readattunement")), spell), string.Empty),
			"Shows the target's assigned affinities and learned affiliations only to its caster.",
			"This effect has no options. The spell's ordinary duration controls the overlay.",
			false, true, StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private ReadAttunementSpellEffect(XElement root, IMagicSpell spell) : base(root, spell) { }
	protected override string BuilderEffectType => "readattunement";
	protected override string ShowText => "Read Attunement";
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		var child = GetOrApplyEffect(caster, target, outcome, power, parent, additionalParameters);
		return new(child is null ? MagicEffectOperationStatus.Rejected : MagicEffectOperationStatus.Applied, child);
	}
	protected override IMagicSpellEffect CreateEffect(ICharacter caster, ICharacter target,
		OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent,
		SpellAdditionalParameter[] additionalParameters) => new SpellReadAttunementEffect(target, parent,
			CharacterInstanceIdentityComparer.IdentityId(caster), caster.InstanceId);
	public override IMagicSpellEffectTemplate Clone() => new ReadAttunementSpellEffect(SaveToXml(), Spell);
}
