#nullable enable
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class $safeitemrootname$Effect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("$safeitemrootname$".ToLowerInvariant(),
			(root, spell) => new $safeitemrootname$Effect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("$safeitemrootname$".ToLowerInvariant(), BuilderFactory,
			"Describe this spell effect.",
			HelpText,
			false,
			true,
			SpellTriggerFactory.MagicTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new $safeitemrootname$Effect(
			new XElement("Effect", new XAttribute("type", "$safeitemrootname$".ToLowerInvariant())),
			spell), string.Empty);
	}

	public $safeitemrootname$Effect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		// Load template configuration from XML here.
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "$safeitemrootname$".ToLowerInvariant())
		);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new $safeitemrootname$Effect(SaveToXml(), Spell);
	}

	public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		return new Spell$safeitemrootname$Effect(target, parent, null);
	}

	public string Show(ICharacter actor)
	{
		return "$safeitemrootname$Effect";
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3command info#0 - explain what this option should do";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}
}
