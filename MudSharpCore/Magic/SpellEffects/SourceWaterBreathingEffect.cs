#nullable enable

using System.Globalization;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Magic.WaterBreathing;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>Editable source adapter, deliberately distinct from the historical broad native grant.</summary>
public sealed partial class SourceWaterBreathingEffect : WaterBreathingEffect,
	IMagicSpellEffectLifetimePolicy, IMagicSpellEffectPreparedSelection, IMagicSpellEffectOperation
{
	private XElement? _invalidPolicy;
	private XElement? _invalidScope;
	internal WaterBreathingFluidScope? Scope { get; private set; }
	public MagicSpellLifetimePolicy? LifetimePolicy { get; private set; }
	public string? LifetimePolicyError { get; private set; }
	private string? _scopeError;
	internal string? ConfigurationError => LifetimePolicyError ?? _scopeError ??
		(LifetimePolicy is null ? "Select an accumulated lifetime policy." :
		 Scope is null ? "Map at least one native water liquid explicitly." : null);

	public new static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("sourcewaterbreathing", (root, spell) => new SourceWaterBreathingEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("sourcewaterbreathing", (_, spell) =>
			(new SourceWaterBreathingEffect(new XElement("Effect", DetectInvisibleEffect.WritePolicy(
				new("armageddon.water_breathing", 600, 36))), spell), string.Empty),
			"Source water breathing with explicit liquid mappings", "", false, true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	internal SourceWaterBreathingEffect(XElement root, IMagicSpell spell) : base(root, spell)
	{
		if (root.Element("LifetimePolicy") is { } policy)
		{
			try { LifetimePolicy = DetectInvisibleEffect.ReadPolicy(policy); }
			catch (Exception error) when (error is FormatException or OverflowException or ArgumentException)
			{ LifetimePolicyError = error.Message; _invalidPolicy = new(policy); }
		}
		if (root.Element("WaterScope") is { } scope)
		{
			try { Scope = WaterBreathingScopeXml.Read(scope, Gameworld); }
			catch (Exception error) when (error is FormatException or OverflowException or ArgumentException)
			{ _scopeError = error.Message; _invalidScope = new(scope); }
		}
	}

	protected override string BuilderEffectType => "sourcewaterbreathing";
	protected override string ShowText => "Source Water Breathing";
	protected override void SaveToXml(XElement root)
	{
		if (_invalidPolicy is not null) root.Add(new XElement(_invalidPolicy));
		else if (LifetimePolicy is { } policy) root.Add(DetectInvisibleEffect.WritePolicy(policy));
		if (_invalidScope is not null) root.Add(new XElement(_invalidScope));
		else if (Scope is { } scope) root.Add(WaterBreathingScopeXml.Write(scope));
	}
	public override IMagicSpellEffectTemplate Clone() => new SourceWaterBreathingEffect(SaveToXml(), Spell);

	public override string Show(ICharacter actor) => base.Show(actor) +
		$"\nLifetime: {LifetimePolicy?.Group ?? "unconfigured"}; {LifetimePolicy?.UnitSeconds} seconds/unit, cap {LifetimePolicy?.MaximumUnits}; retain strongest source grade." +
		$"\nWater liquid IDs: {string.Join(", ", Scope?.LiquidIds ?? [])}." +
		$"\nSource duration: max(1, inclusive random(floor(grade/2), 3*grade)) units, selected once per invocation." +
		(ConfigurationError is { } error ? $"\nInvalid configuration: {error}" : "");

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var mode = command.PopSpeech();
		if (mode.EqualTo("lifetime"))
		{
			var group = command.PopSpeech(); var seconds = command.PopSpeech(); var maximum = command.PopSpeech();
			if (!command.IsFinished || !int.TryParse(seconds, NumberStyles.Integer, CultureInfo.InvariantCulture, out var unit) ||
				!int.TryParse(maximum, NumberStyles.Integer, CultureInfo.InvariantCulture, out var cap)) return Help(actor);
			var policy = new MagicSpellLifetimePolicy(group, unit, cap);
			if (DetectInvisibleEffect.PolicyError(policy) is { } error) { actor.OutputHandler.Send(error); return false; }
			LifetimePolicy = policy; LifetimePolicyError = null; _invalidPolicy = null;
		}
		else if (mode.EqualTo("water"))
		{
			var action = command.PopSpeech();
			var liquid = Gameworld.Liquids.GetByIdOrName(command.SafeRemainingArgument);
			if (liquid is null || (!action.EqualTo("add") && !action.EqualTo("remove"))) return Help(actor);
			var ids = Scope?.LiquidIds.ToList() ?? [];
			if (action.EqualTo("add")) { if (!ids.Contains(liquid.Id)) ids.Add(liquid.Id); }
			else ids.Remove(liquid.Id);
			Scope = ids.Count == 0 ? null : new(ids.Select(id => Gameworld.Liquids.Get(id)));
			_scopeError = null; _invalidScope = null;
		}
		else return Help(actor);
		_selection = null; Spell.Changed = true; actor.OutputHandler.Send(Show(actor)); return true;
	}

	private static bool Help(ICharacter actor)
	{
		actor.OutputHandler.Send("Use water add/remove <native liquid ID or name>, or lifetime <group> <seconds per unit> <maximum units>. The spell requires one exclusive target effect, a character/self trigger, no save, and duration expression 0.");
		return false;
	}

	protected override IMagicSpellEffect CreateEffect(ICharacter caster, ICharacter target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		((MagicSpell)Spell).ConfirmWaterBinding(this, caster, target);
		return new SpellScopedWaterBreathingEffect(target, parent, Scope!);
	}
}
