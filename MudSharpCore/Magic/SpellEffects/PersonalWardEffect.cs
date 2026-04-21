using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellEffects;

public class PersonalWardEffect : WardSpellEffectBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("personalward", (root, spell) => new PersonalWardEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("personalward", BuilderFactory,
			"Applies a personal ward that can block or reflect matching magic schools",
			HelpText,
			false,
			true,
			SpellTriggerFactory.MagicTriggerTypes
			                   .Where(x => CompatibleTypes.Contains(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
			                   .ToArray());
	}

	private static readonly string[] CompatibleTypes = ["character", "characters", "character&room", "character&exit"];

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
	{
		return (new PersonalWardEffect(new XElement("Effect",
			new XAttribute("type", "personalward"),
			new XElement("School", spell.School.Id),
			new XElement("Mode", MagicInterdictionMode.Fail),
			new XElement("Coverage", MagicInterdictionCoverage.Both),
			new XElement("IncludesSubschools", true),
			new XElement("Prog", 0L)
		), spell), string.Empty);
	}

	protected PersonalWardEffect(XElement root, IMagicSpell spell)
		: base(root, spell)
	{
	}

	protected override string EffectType => "personalward";
	protected override string EffectName => "Personal Ward";
	protected override string[] CompatibleTargetTypes => CompatibleTypes;

	protected override IMagicSpellEffect CreateWardEffect(IPerceivable target, IMagicSpellEffectParent parent)
	{
		return target is ICharacter
			? new SpellPersonalWardEffect(target, parent, School, Mode, Coverage, IncludesSubschools, Prog)
			: null;
	}

	protected override IMagicSpellEffectTemplate CloneEffect(XElement root, IMagicSpell spell)
	{
		return new PersonalWardEffect(root, spell);
	}
}
