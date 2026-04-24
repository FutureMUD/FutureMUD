using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Planes;
using MudSharp.RPG.Checks;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellEffects;

public class PlanarStateSpellEffect : IMagicSpellEffectTemplate
{
	private readonly string _type;

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("planarstate", (root, spell) => new PlanarStateSpellEffect(root, spell, "planarstate"));
		SpellEffectFactory.RegisterBuilderFactory("planarstate", (commands, spell) => BuilderFactory(commands, spell, "planarstate"),
			"Applies a planar corporeality state to the target",
			HelpText,
			false,
			true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());

		SpellEffectFactory.RegisterLoadTimeFactory("planeshift", (root, spell) => new PlanarStateSpellEffect(root, spell, "planeshift"));
		SpellEffectFactory.RegisterBuilderFactory("planeshift", (commands, spell) => BuilderFactory(commands, spell, "planeshift"),
			"Moves a target's planar state to another plane",
			HelpText,
			false,
			true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell,
		string type)
	{
		var plane = commands.IsFinished ? spell.Gameworld.DefaultPlane : spell.Gameworld.Planes.GetByIdOrName(commands.PopSpeech());
		if (plane is null)
		{
			return (null, "There is no such plane.");
		}

		var state = commands.IsFinished ? "noncorporeal" : commands.PopSpeech().ToLowerInvariant();
		var definition = state switch
		{
			"corporeal" or "manifest" or "manifested" => PlanarPresenceDefinition.Manifested(plane),
			"noncorporeal" or "incorporeal" or "dissipated" => PlanarPresenceDefinition.NonCorporeal(plane),
			_ => null
		};
		if (definition is null)
		{
			return (null, "The state must be corporeal or noncorporeal.");
		}

		return (new PlanarStateSpellEffect(new XElement("Effect",
			new XAttribute("type", type),
			definition.SaveToXml()), spell, type), string.Empty);
	}

	private PlanarStateSpellEffect(XElement root, IMagicSpell spell, string type)
	{
		Spell = spell;
		_type = type;
		Definition = PlanarPresenceDefinition.FromXml(root.Element("PlanarData"), spell.Gameworld);
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public PlanarPresenceDefinition Definition { get; private set; }
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;
	public const string HelpText = @"You can use #3effect set <plane> <corporeal|noncorporeal>#0 to set this effect's plane and state.";

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", _type),
			Definition.SaveToXml());
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var plane = command.IsFinished ? null : actor.Gameworld.Planes.GetByIdOrName(command.PopSpeech());
		if (plane is null || command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a plane and corporeal or noncorporeal.");
			return false;
		}

		Definition = command.PopSpeech().ToLowerInvariant() switch
		{
			"corporeal" or "manifest" or "manifested" => PlanarPresenceDefinition.Manifested(plane),
			"noncorporeal" or "incorporeal" or "dissipated" => PlanarPresenceDefinition.NonCorporeal(plane),
			_ => null
		};
		if (Definition is null)
		{
			actor.OutputHandler.Send("The state must be corporeal or noncorporeal.");
			return false;
		}

		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect now applies {Definition.Describe(actor.Gameworld).ColourValue()}.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return $"Planar State - {Definition.Describe(actor.Gameworld)}";
	}

	public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		return target is null ? null : new SpellPlanarStateEffect(target, parent, Definition);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new PlanarStateSpellEffect(SaveToXml(), Spell, _type);
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return IsCompatibleWithTrigger(trigger.TargetTypes);
	}

	public static bool IsCompatibleWithTrigger(string types)
	{
		return types is "character" or "characters" or "item" or "items" or "perceivable" or "perceivables";
	}
}

public class RemovePlanarStateSpellEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("removeplanarstate", (root, spell) => new RemovePlanarStateSpellEffect(spell));
		SpellEffectFactory.RegisterBuilderFactory("removeplanarstate", BuilderFactory,
			"Removes planar state spell and saved overlays from the target",
			"Has no configurable options.",
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => PlanarStateSpellEffect.IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
	{
		return (new RemovePlanarStateSpellEffect(spell), string.Empty);
	}

	private RemovePlanarStateSpellEffect(IMagicSpell spell)
	{
		Spell = spell;
	}

	public IMagicSpell Spell { get; }
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;

	public XElement SaveToXml()
	{
		return new XElement("Effect", new XAttribute("type", "removeplanarstate"));
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send("This effect has no configurable options.");
		return false;
	}

	public string Show(ICharacter actor)
	{
		return "Remove Planar State";
	}

	public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		target?.RemoveAllEffects<PlanarStateEffect>(x => true, true);
		target?.RemoveAllEffects<SpellPlanarStateEffect>(x => true, true);
		return null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new RemovePlanarStateSpellEffect(Spell);
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return PlanarStateSpellEffect.IsCompatibleWithTrigger(trigger.TargetTypes);
	}
}
