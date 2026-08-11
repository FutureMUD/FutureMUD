#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.RPG.Checks;
using MudSharp.Traps;

namespace MudSharp.Magic.SpellEffects;

/// <summary>
/// An instantaneous spell effect for clearing a magical or dispellable trap on an item, character, or cell.
/// It deliberately removes only the trap effect, leaving the anchor item and all unrelated effects intact.
/// </summary>
public sealed class RemoveTrapSpellEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("removetrap", (root, spell) => new RemoveTrapSpellEffect(root, spell));
		SpellEffectFactory.RegisterLoadTimeFactory("dispeltrap", (root, spell) => new RemoveTrapSpellEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory(
			"removetrap",
			BuilderFactory,
			"Removes a magical or dispellable trap from the target",
			string.Empty,
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes
				.Where(x => IsCompatibleTargetType(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
				.ToArray());
		SpellEffectFactory.RegisterBuilderFactory(
			"dispeltrap",
			BuilderFactory,
			"Removes a magical or dispellable trap from the target",
			string.Empty,
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes
				.Where(x => IsCompatibleTargetType(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
				.ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new RemoveTrapSpellEffect(new XElement("Effect", new XAttribute("type", "removetrap")), spell),
			string.Empty);
	}

	private RemoveTrapSpellEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;

	public XElement SaveToXml()
	{
		return new XElement("Effect", new XAttribute("type", "removetrap"));
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		actor.Send("This spell effect has no options.");
		return false;
	}

	public string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Remove Trap");
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return IsCompatibleTargetType(trigger.TargetTypes);
	}

	private static bool IsCompatibleTargetType(string targetTypes)
	{
		return targetTypes is "character" or "characters" or "item" or "room" or "perceivables";
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target,
		OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent,
		SpellAdditionalParameter[] additionalParameters)
	{
		var trap = target?.EffectsOfType<TrapEffect>()
			.FirstOrDefault(x => x.SourceKind == TrapSourceKind.Magical ||
			                     x.Template?.DisarmPolicy == TrapDisarmPolicy.Dispellable);
		if (trap is not null &&
		    caster.Gameworld.GetCheck(CheckType.DispelTrapCheck).Check(caster, Difficulty.Normal, caster).Outcome.IsPass())
		{
			target!.RemoveEffect(trap, true);
		}

		return null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new RemoveTrapSpellEffect(SaveToXml(), Spell);
	}
}
