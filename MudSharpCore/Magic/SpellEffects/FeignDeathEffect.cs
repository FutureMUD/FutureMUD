#nullable enable

using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>A timed apparent-death effect admitted only for the exact living body.</summary>
public sealed class FeignDeathEffect : IMagicSpellEffectTemplate, IMagicSpellEffectAdmission
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("feigndeath", (root, spell) => new FeignDeathEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("feigndeath", (_, spell) =>
			(new FeignDeathEffect(new XElement("Effect", new XAttribute("type", "feigndeath"),
				new XAttribute("schema", 1)), spell), string.Empty),
			"Makes a living character appear dead to ordinary observers until action or expiry",
			HelpText, false, true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x =>
				SpellTriggerFactory.BuilderInfoForType(x).TargetTypes == "character").ToArray());
	}

	private FeignDeathEffect(XElement root, IMagicSpell spell)
	{
		if ((string?)root.Attribute("type") != "feigndeath" || (int?)root.Attribute("schema") != 1)
			throw new FormatException("Feign Death requires a schema-1 effect.");
		Spell = spell;
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger.TargetTypes == "character";
	public XElement SaveToXml() => new("Effect", new XAttribute("type", "feigndeath"), new XAttribute("schema", 1));
	public IMagicSpellEffectTemplate Clone() => new FeignDeathEffect(SaveToXml(), Spell);
	public string Show(ICharacter actor) => SpellEffectPresentation.Describe(actor, "Feign Death",
		("Appearance", "ordinary visual and vitals observers"), ("Break", "voluntary action"));
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send(HelpText);
		return false;
	}
	public const string HelpText = "The living-body, apparent-death and action-break policy has no builder options.";

	public bool TryPrepareApplication(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, TimeSpan resolvedDuration, out IMagicSpellEffectApplication? application, out string? error)
	{
		application = null;
		error = "Feign Death needs one living character and a positive bounded duration.";
		if (target is not ICharacter character || character.Body is not { } body ||
			character.State.HasFlag(CharacterState.Dead) || resolvedDuration <= TimeSpan.Zero ||
			resolvedDuration > TimeSpan.FromDays(1)) return false;
		application = new Application(character, body);
		error = null;
		return true;
	}

	private sealed record Application(ICharacter Character, IBody Body) : IMagicSpellEffectApplication
	{
		public IMagicSpellEffect Create(IMagicSpellEffectParent parent)
		{
			if (!CommandExecutionScope.TryContinue() || !ReferenceEquals(parent.Owner, Character) ||
				!ReferenceEquals(Character.Body, Body) || Character.State.HasFlag(CharacterState.Dead))
				throw new InvalidOperationException("The selected living body changed before Feign Death application.");
			return new SpellFeignedDeathEffect(Character, parent);
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
			throw new InvalidOperationException(error ?? "Feign Death was not admitted.");
		return application!.Create(parent);
	}
}
