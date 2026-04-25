using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System.Linq;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.Magic.SpellEffects;

public class ExitBarrierEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("exitbarrier", (root, spell) => new ExitBarrierEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("exitbarrier", BuilderFactory,
			"Applies a persistent magical barrier to an exit",
			HelpText,
			false,
			true,
			SpellTriggerFactory.MagicTriggerTypes
			                   .Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
			                   .ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
	{
		return (new ExitBarrierEffect(new XElement("Effect", new XAttribute("type", "exitbarrier")), spell), string.Empty);
	}

	protected ExitBarrierEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;

	public XElement SaveToXml()
	{
		return new XElement("Effect", new XAttribute("type", "exitbarrier"));
	}

	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return IsCompatibleWithTrigger(trigger.TargetTypes);
	}

	public static bool IsCompatibleWithTrigger(string types)
	{
		return types == "exit";
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		return target is IExit ? new SpellExitBarrierEffect(target, parent) : null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new ExitBarrierEffect(SaveToXml(), Spell);
	}

	public string Show(ICharacter actor)
	{
		return "Exit Barrier".ColourName();
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	public const string HelpText = "This effect has no additional builder options.";
}
