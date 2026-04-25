using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellEffects;

public class MagicalTetherEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("magicaltether", (root, spell) => new MagicalTetherEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("magicaltether", BuilderFactory,
			"Creates a magical zero-gravity tether from the target to the caster",
			HelpText,
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
	{
		return (new MagicalTetherEffect(new XElement("Effect",
			new XAttribute("type", "magicaltether"),
			new XElement("MaximumRooms", 3)
		), spell), string.Empty);
	}

	protected MagicalTetherEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		MaximumRooms = int.Parse(root.Element("MaximumRooms")?.Value ?? "3");
	}

	public IMagicSpell Spell { get; }
	public int MaximumRooms { get; set; }

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "magicaltether"),
			new XElement("MaximumRooms", MaximumRooms)
		);
	}

	public const string HelpText = @"Options:
    #3rooms <number>#0 - set the maximum room length";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "rooms":
			case "length":
				return BuildingCommandRooms(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandRooms(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var rooms) || rooms < 0)
		{
			actor.OutputHandler.Send("Enter a non-negative maximum number of rooms.");
			return false;
		}

		MaximumRooms = rooms;
		Spell.Changed = true;
		actor.OutputHandler.Send($"Maximum tether length set to {MaximumRooms.ToString("N0", actor).ColourValue()} rooms.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return $"MagicalTether {MaximumRooms.ToString("N0", actor).ColourValue()} rooms";
	}

	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public bool IsCompatibleWithTrigger(IMagicTrigger types)
	{
		return IsCompatibleWithTrigger(types.TargetTypes);
	}

	public static bool IsCompatibleWithTrigger(string types)
	{
		return types is "character" or "characters" or "item" or "perceivables";
	}

	public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		return new SpellZeroGravityTetherEffect(target, parent, null, caster, MaximumRooms);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new MagicalTetherEffect(SaveToXml(), Spell);
	}
}
