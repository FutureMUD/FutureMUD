using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class $safeitemrootname$ : IMagicSpellEffectTemplate
{
	public IFuturemud Gameworld => Spell.Gameworld;
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("$safeitemrootname$".ToLowerInvariant(), (root, spell) => new $safeitemrootname$(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("$safeitemrootname$".ToLowerInvariant(), BuilderFactory);
	}
	
	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new GlowEffect(new XElement("Effect",
			new XAttribute("type", "$safeitemrootname$".ToLowerInvariant())
			), spell), string.Empty);
	}

	public IMagicSpell Spell { get; }
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

#region Constructors and Saving

	public $safeitemrootname$(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		// Load all the different properties
	}

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "$safeitemrootname$")
			// Further xattributes
		);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new $safeitemrootname$(SaveToXml(), Spell);
	}
#endregion

	public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
			SpellPower power, IMagicSpellEffectParent parent)
	{
		// Return null if no spell effect

		// Remove or change if target is not character
		if (target is not ICharacter)
		{
			return null;
		}

		return new $safeitemrootname$(target, parent, null);
	}

#region Building Commands
	public string Show(ICharacter actor)
	{
		// A one-line description of the spell effect
		return $"$safeitemrootname$";
	}

	public const string HelpText = @"You can use the following options with this spell effect:

	#3command info#0 - explanation";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}
#endregion
}