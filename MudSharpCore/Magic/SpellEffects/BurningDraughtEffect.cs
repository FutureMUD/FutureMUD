#nullable enable

using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>Admits a bounded per-sip burn on the exact living body before payment.</summary>
public sealed class BurningDraughtEffect : IMagicSpellEffectTemplate, IMagicSpellEffectAdmission
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("burningdraught", (root, spell) => new BurningDraughtEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("burningdraught", (_, spell) =>
			(new BurningDraughtEffect(new XElement("Effect", new XAttribute("type", "burningdraught"),
				new XAttribute("schema", 1), new XElement("DamagePerSip", 5.0)), spell), string.Empty),
			"Replaces a living character's drink refreshment with burning injury", HelpText,
			false, true, SpellTriggerFactory.MagicTriggerTypes.Where(x =>
				SpellTriggerFactory.BuilderInfoForType(x).TargetTypes == "character").ToArray());
	}

	private BurningDraughtEffect(XElement root, IMagicSpell spell)
	{
		if ((string?)root.Attribute("type") != "burningdraught" || (int?)root.Attribute("schema") != 1)
			throw new FormatException("Burning Draught requires a schema-1 effect.");
		DamagePerSip = double.Parse(root.Element("DamagePerSip")?.Value ?? "0");
		if (!double.IsFinite(DamagePerSip) || DamagePerSip is < 0.1 or > 100.0)
			throw new FormatException("Burning Draught damage per sip must be finite and between 0.1 and 100.");
		Spell = spell;
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public double DamagePerSip { get; private set; }
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger.TargetTypes == "character";
	public XElement SaveToXml() => new("Effect", new XAttribute("type", "burningdraught"),
		new XAttribute("schema", 1), new XElement("DamagePerSip", DamagePerSip));
	public IMagicSpellEffectTemplate Clone() => new BurningDraughtEffect(SaveToXml(), Spell);
	public string Show(ICharacter actor) => SpellEffectPresentation.Describe(actor, "Burning Draught",
		("Burning damage per accepted sip", DamagePerSip.ToString("N1", actor)));
	public const string HelpText = "Use damage <0.1-100> to set native Burning damage per accepted sip.";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (!command.PopSpeech().EqualTo("damage") ||
			!double.TryParse(command.PopSpeech(), out var value) ||
			!double.IsFinite(value) || value is < 0.1 or > 100.0)
		{
			actor.OutputHandler.Send(HelpText);
			return false;
		}
		DamagePerSip = value;
		Spell.Changed = true;
		actor.OutputHandler.Send($"Burning Draught now inflicts {value.ToString("N1", actor).ColourValue()} damage per accepted sip.");
		return true;
	}

	public bool TryPrepareApplication(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, TimeSpan resolvedDuration, out IMagicSpellEffectApplication? application, out string? error)
	{
		application = null;
		error = "Burning Draught needs one living character and a positive bounded duration.";
		if (target is not ICharacter character || character.Body is not { } body ||
			character.State.HasFlag(CharacterState.Dead) || resolvedDuration <= TimeSpan.Zero ||
			resolvedDuration > TimeSpan.FromDays(1)) return false;
		application = new Application(character, body, DamagePerSip);
		error = null;
		return true;
	}

	private sealed record Application(ICharacter Character, IBody Body, double DamagePerSip) : IMagicSpellEffectApplication
	{
		public IMagicSpellEffect Create(IMagicSpellEffectParent parent)
		{
			if (!CommandExecutionScope.TryContinue() || !ReferenceEquals(parent.Owner, Character) ||
				!ReferenceEquals(Character.Body, Body) || Character.State.HasFlag(CharacterState.Dead))
				throw new InvalidOperationException("The selected living body changed before Burning Draught application.");
			return new SpellBurningDraughtEffect(Character, parent, DamagePerSip);
		}
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target,
		OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent,
		SpellAdditionalParameter[] additionalParameters)
	{
		string? error = null;
		if (target is null || parent is not MagicSpellParent native ||
			!TryPrepareApplication(caster, target, outcome, power, native.ResolvedDuration,
				out var application, out error))
			throw new InvalidOperationException(error ?? "Burning Draught was not admitted.");
		return application!.Create(parent);
	}
}
