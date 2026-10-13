#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class StaminaDeltaSpellEffect : IMagicSpellEffectTemplate, IMagicSpellEffectOperation, IMagicSpellEffectPreparedSelection
{
	private string? _directLoadError;
	private XElement? _invalidDirectDefinition;
	private bool _applyingDirect;
	private readonly Dictionary<IPerceivable, StaminaSelection> _selections = new(ReferenceEqualityComparer.Instance);
	private sealed record StaminaSelection(ICharacter Target, IBody Body, long InstanceId, long BodyId)
		: IMagicSpellEffectPreparedSelectionRawToken
	{
		public bool IsCurrent => Target.InstanceId == InstanceId && Body.Id == BodyId && ReferenceEquals(Target.Body, Body) && ReferenceEquals(Body.Actor, Target);
	}

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("staminadelta", (root, spell) => new StaminaDeltaSpellEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("staminadelta", BuilderFactory, "Instantly changes stamina", HelpText,
			true, true, SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
	}
	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell) =>
		(new StaminaDeltaSpellEffect(new XElement("Effect", new XAttribute("type", "staminadelta"),
			new XElement("Formula", new XCData("power*outcome"))), spell), string.Empty);

	protected StaminaDeltaSpellEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		AmountExpression = new TraitExpression(root.Element("Formula")!.Value, Gameworld);
		if (root.Element("DirectStaminaAdjustment") is { } mode)
		{
			if (!bool.TryParse(mode.Value, out var direct)) _directLoadError = "DirectStaminaAdjustment must be an explicit boolean.";
			DirectStaminaAdjustment = direct;
			if (_directLoadError is not null) _invalidDirectDefinition = new XElement(root);
		}
	}
	public IMagicSpell Spell { get; }
	public ITraitExpression AmountExpression { get; set; }
	public bool DirectStaminaAdjustment { get; private set; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public string? DefinitionError
	{
		get
		{
			if (_directLoadError is not null) return _directLoadError;
			if (!DirectStaminaAdjustment) return null;
			if (AmountExpression is null || AmountExpression.HasErrors()) return "Exact stamina adjustment requires a valid finite formula.";
			if (AmountExpression is TraitExpression native && native.Parameters.Count == 0 &&
				!native.Formula.ParameterNames.Any() && !native.Formula.FunctionNames.Any() &&
				!native.Formula.TryEvaluateDoubleWith(new Dictionary<string, object>(), out _, out _))
				return "Exact stamina adjustment requires a finite formula result.";
			return null;
		}
	}
	public XElement SaveToXml()
	{
		if (_invalidDirectDefinition is not null) return new XElement(_invalidDirectDefinition);
		return new XElement("Effect", new XAttribute("type", "staminadelta"),
			new XElement("Formula", new XCData(AmountExpression.OriginalFormulaText)),
			DirectStaminaAdjustment ? new XElement("DirectStaminaAdjustment", true) : null);
	}
	public const string HelpText = @"Options:
	#3formula <expr>#0 - sets stamina change formula
	#3directstamina true|false#0 - applies the signed change directly, bounded by zero and the native maximum; bypasses expenditure multipliers and second winds";
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (command.PeekSpeech().EqualTo("directstamina"))
		{
			command.PopSpeech();
			if (!bool.TryParse(command.SafeRemainingArgument, out var direct))
			{
				actor.OutputHandler.Send("Use true or false for exact native stamina adjustment.");
				return false;
			}
			var previousMode = DirectStaminaAdjustment; var loadError = _directLoadError; var definition = _invalidDirectDefinition;
			DirectStaminaAdjustment = direct; _directLoadError = null; _invalidDirectDefinition = null;
			if (DefinitionError is { } modeError)
			{
				DirectStaminaAdjustment = previousMode; _directLoadError = loadError; _invalidDirectDefinition = definition;
				actor.OutputHandler.Send(modeError.ColourError()); return false;
			}
			Spell.Changed = true;
			actor.OutputHandler.Send($"Exact native stamina adjustment is now {direct.ToColouredString()}.");
			return true;
		}
		// Keep the historical whole-expression shorthand as well as the documented formula command.
		if (command.PeekSpeech().EqualTo("formula")) command.PopSpeech();
		if (command.IsFinished) { actor.OutputHandler.Send("You must specify a formula."); return false; }
		var expression = new TraitExpression(command.SafeRemainingArgument, Gameworld);
		if (expression.HasErrors()) { actor.OutputHandler.Send(expression.Error); return false; }
		var previous = AmountExpression; AmountExpression = expression;
		if (DefinitionError is { } error) { AmountExpression = previous; actor.OutputHandler.Send(error.ColourError()); return false; }
		Spell.Changed = true;
		actor.OutputHandler.Send($"Formula set to {expression.OriginalFormulaText.ColourCommand()}.");
		return true;
	}
	public string Show(ICharacter actor) => SpellEffectPresentation.Describe(actor, "Stamina Delta",
		("Formula", AmountExpression.OriginalFormulaText.ColourCommand()),
		("Exact Native Adjustment", DirectStaminaAdjustment.ToColouredString()),
		("Validation", DefinitionError?.ColourError() ?? "Valid".ColourValue()));
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => IsCompatibleWithTrigger(trigger.TargetTypes);
	public static bool IsCompatibleWithTrigger(string types) => types is "character" or "characters";

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (DefinitionError is { } error) throw new InvalidOperationException(error);
		if (DirectStaminaAdjustment) { Apply(caster, target, outcome, power, parent, additionalParameters); return null; }
		if (target is not ICharacter character) return null;
		var amount = AmountExpression.EvaluateWith(caster, values: [("power", (int)power), ("outcome", (int)outcome)]);
		if (amount >= 0) character.GainStamina(amount); else character.SpendStamina(-amount);
		return null;
	}
	public IMagicSpellEffectTemplate Clone() => new StaminaDeltaSpellEffect(SaveToXml(), Spell);
	public IMagicSpellEffectPreparedSelectionToken? CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (!DirectStaminaAdjustment || recipient is not ICharacter target || target.Body is null) return null;
		if (!_selections.TryGetValue(recipient, out var selected))
			_selections[recipient] = selected = new(target, target.Body, target.InstanceId, target.Body.Id);
		return selected;
	}
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster,
		IPerceivable recipient, out string? error)
	{
		error = null;
		if (!DirectStaminaAdjustment) return true;
		if (selection is not StaminaSelection selected || !ReferenceEquals(selected.Target, recipient) || !selected.IsCurrent)
		{
			error = "The selected stamina recipient or physical body changed."; return false;
		}
		_selections[recipient] = selected; return true;
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = DefinitionError;
		if (error is not null) return false;
		if (!DirectStaminaAdjustment) return true;
		if (!_selections.TryGetValue(recipient, out var selected) || !selected.IsCurrent || !ValidState(selected.Body, out _, out _) || !selected.IsCurrent)
			error = "The selected stamina recipient, physical body or stamina bounds are invalid or changed.";
		return error is null;
	}
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (DefinitionError is { } error) throw new InvalidOperationException(error);
		if (!DirectStaminaAdjustment)
		{
			GetOrApplyEffect(caster, target!, outcome, power, parent, additionalParameters);
			return new(MagicEffectOperationStatus.Unknown, null);
		}
		if (target is not ICharacter character || character.Body is null) return new(MagicEffectOperationStatus.Rejected, null);
		var selected = (StaminaSelection?)CapturePreparedSelection(caster, character);
		if (selected is null || !selected.IsCurrent) return new(MagicEffectOperationStatus.Rejected, null);
		if (_applyingDirect) throw new InvalidOperationException("Exact stamina adjustment cannot be re-entered during a native callback.");
		_applyingDirect = true;
		try
		{
			var expression = AmountExpression;
			var amount = expression.EvaluateWith(caster, values: [("power", (int)power), ("outcome", (int)outcome)]);
			if (!double.IsFinite(amount)) throw new InvalidOperationException("Exact stamina adjustment requires a finite evaluated amount.");
			if (!selected.IsCurrent || !ReferenceEquals(expression, AmountExpression) || !ValidState(selected.Body, out var before, out var maximum) || !selected.IsCurrent)
				return new(MagicEffectOperationStatus.Rejected, null);
			var expected = amount >= 0 ? before + Math.Min(amount, maximum - before) : before - Math.Min(-amount, before);
			if (expected == before) return new(MagicEffectOperationStatus.NoChange, null);
			if (!selected.IsCurrent || selected.Body.CurrentStamina != before || selected.Body.MaximumStamina != maximum || !selected.IsCurrent)
				return new(MagicEffectOperationStatus.Rejected, null);
			try
			{
				selected.Body.CurrentStamina = expected;
				var after = selected.Body.CurrentStamina;
				if (after != before) MarkDirty();
				if (!selected.IsCurrent || selected.Body.MaximumStamina != maximum || !ReferenceEquals(expression, AmountExpression) ||
					!double.IsFinite(after) || after != expected)
					throw new InvalidOperationException("Stamina state changed during a native setter callback; the paid write is uncertain.");
				return new(after != before ? MagicEffectOperationStatus.Applied : MagicEffectOperationStatus.NoChange, null);
			}
			catch
			{
				try { if (selected.Body.CurrentStamina != before) MarkDirty(); } catch { }
				throw;
			}
			void MarkDirty() { selected.Target.Changed = true; selected.Body.Changed = true; }
		}
		finally { _applyingDirect = false; }
	}
	private static bool ValidState(IBody body, out double current, out double maximum)
	{
		current = body.CurrentStamina; maximum = body.MaximumStamina;
		return double.IsFinite(current) && double.IsFinite(maximum) && current >= 0 && maximum >= 0 && current <= maximum;
	}
}
