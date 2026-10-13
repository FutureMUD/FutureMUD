#nullable enable

using MudSharp.Health;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class DetectPoisonEffect : CharacterSpellEffectTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("detectpoison", (root, spell) => new DetectPoisonEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("detectpoison", BuilderFactory,
			"Detects active and latent poisons in a target",
			"#3retained true|false#0 - grants a timed inspection permission instead of reporting once",
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
	protected override string ShowText => RetainedPermission ? "Detect Poison - retained inspection" : "Detect Poison - immediate report";
	public bool RetainedPermission { get; private set; }
	public override bool IsInstantaneous => !RetainedPermission;
	protected override void LoadFromXml(XElement root) => RetainedPermission = (bool?)root.Element("RetainedPermission") ?? false;
	protected override void SaveToXml(XElement root)
	{
		if (RetainedPermission) root.Add(new XElement("RetainedPermission", true));
	}
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (!command.PopSpeech().EqualTo("retained") ||
			!bool.TryParse(command.SafeRemainingArgument, out var enabled))
		{
			actor.OutputHandler.Send("Use retained true or retained false to choose a timed inspection permission or one immediate report.");
			return false;
		}
		RetainedPermission = enabled;
		Spell.Changed = true;
		actor.OutputHandler.Send(enabled ? "This effect now grants a timed poison inspection permission." :
			"This effect now reports poison once when cast.");
		return true;
	}

	protected override IMagicSpellEffect? CreateEffect(ICharacter caster, ICharacter target,
		OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent,
		SpellAdditionalParameter[] additionalParameters)
	{
		if (RetainedPermission) return new SpellDetectPoisonEffect(target, parent);
		caster.OutputHandler.Send(
			DrugDosageDescriber.DescribeActiveAndLatentDrugDosages(target, caster, includeEmptyMessage: true));
		return null;
	}

	public override IMagicSpellEffectTemplate Clone()
	{
		return new DetectPoisonEffect(SaveToXml(), Spell);
	}
}
