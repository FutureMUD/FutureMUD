#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellEffects;

public class DetectPoisonEffect : CharacterSpellEffectTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("detectpoison", (root, spell) => new DetectPoisonEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("detectpoison", BuilderFactory,
			"Detects active and latent poisons in a target",
			"This effect has no additional builder options.",
			true,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
		=> (new DetectPoisonEffect(new XElement("Effect", new XAttribute("type", "detectpoison")), spell),
			string.Empty);

	protected DetectPoisonEffect(XElement root, IMagicSpell spell) : base(root, spell)
	{
	}

	protected override string BuilderEffectType => "detectpoison";
	protected override string ShowText => "Detect Poison";
	public override bool IsInstantaneous => true;

	protected override IMagicSpellEffect? CreateEffect(ICharacter caster, ICharacter target,
		OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent,
		SpellAdditionalParameter[] additionalParameters)
	{
		caster.OutputHandler.Send(
			DrugDosageDescriber.DescribeActiveAndLatentDrugDosages(target, caster, includeEmptyMessage: true));
		return null;
	}

	public override IMagicSpellEffectTemplate Clone()
	{
		return new DetectPoisonEffect(SaveToXml(), Spell);
	}
}
