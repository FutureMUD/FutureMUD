using MudSharp.Body.Needs;
using MudSharp.Body;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

#nullable enable
public class NeedDeltaEffect : IMagicSpellEffectTemplate, IMagicSpellEffectOperation, IMagicSpellEffectPreparedSelection
{
	private string? _directAlcoholLoadError;
	private XElement? _invalidDirectAlcoholDefinition;
	private DirectAlcoholSelection? _directAlcoholSelection;
	private bool _applyingDirectAlcohol;
	private sealed record DirectAlcoholSelection(ICharacter Target, IBody Body, INeedsModel Needs)
		: IMagicSpellEffectPreparedSelectionRawToken
	{
		public bool IsCurrent => ReferenceEquals(Target.Body, Body) && ReferenceEquals(Body.Actor, Target) &&
			ReferenceEquals(Target.NeedsModel, Needs) && ReferenceEquals(Body.NeedsModel, Needs);
	}
    public static void RegisterFactory()
    {
        SpellEffectFactory.RegisterLoadTimeFactory("needdelta", (root, spell) => new NeedDeltaEffect(root, spell));
        SpellEffectFactory.RegisterBuilderFactory("needdelta", BuilderFactory,
            "Instantly alters hunger, thirst or drunkenness",
            HelpText,
            true,
            true,
            SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
    }

    private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
    {
        return (new NeedDeltaEffect(new XElement("Effect",
            new XAttribute("type", "needdelta"),
            new XElement("Hunger", 0.0),
            new XElement("Thirst", 0.0),
            new XElement("Drunk", 0.0)
        ), spell), string.Empty);
    }

    protected NeedDeltaEffect(XElement root, IMagicSpell spell)
    {
        Spell = spell;
        HungerDelta = MagicBuilderValidation.ClampFinite(MagicBuilderValidation.ParseFiniteOrDefault(root.Element("Hunger")?.Value, 0.0),
            -MagicBuilderValidation.MaximumNeedDeltaHours, MagicBuilderValidation.MaximumNeedDeltaHours, 0.0);
        ThirstDelta = MagicBuilderValidation.ClampFinite(MagicBuilderValidation.ParseFiniteOrDefault(root.Element("Thirst")?.Value, 0.0),
            -MagicBuilderValidation.MaximumNeedDeltaHours, MagicBuilderValidation.MaximumNeedDeltaHours, 0.0);
        DrunkDelta = MagicBuilderValidation.ClampFinite(MagicBuilderValidation.ParseFiniteOrDefault(root.Element("Drunk")?.Value, 0.0),
            -MagicBuilderValidation.MaximumNeedDeltaAlcoholLitres, MagicBuilderValidation.MaximumNeedDeltaAlcoholLitres, 0.0);
		if (root.Element("DirectAlcoholRemoval") is { } mode)
		{
			if (!bool.TryParse(mode.Value, out var direct))
				_directAlcoholLoadError = "DirectAlcoholRemoval must be an explicit boolean.";
			DirectAlcoholRemoval = direct;
			if (direct && (!MagicBuilderValidation.TryParseFiniteDoubleInRange(root.Element("Drunk")?.Value ?? "0",
				-MagicBuilderValidation.MaximumNeedDeltaAlcoholLitres, 0, out _) ||
				!MagicBuilderValidation.TryParseFiniteDoubleInRange(root.Element("Hunger")?.Value ?? "0", 0, 0, out _) ||
				!MagicBuilderValidation.TryParseFiniteDoubleInRange(root.Element("Thirst")?.Value ?? "0", 0, 0, out _)))
				_directAlcoholLoadError = "Direct alcohol removal requires finite nonpositive alcohol and zero hunger/thirst.";
			if (_directAlcoholLoadError is not null) _invalidDirectAlcoholDefinition = new XElement(root);
		}
    }

    public IMagicSpell Spell { get; }
    public double HungerDelta { get; set; }
    public double ThirstDelta { get; set; }
    public double DrunkDelta { get; set; }
	public bool DirectAlcoholRemoval { get; private set; }
	public string? DefinitionError => _directAlcoholLoadError ?? (DirectAlcoholRemoval &&
		(!double.IsFinite(DrunkDelta) || DrunkDelta > 0 ||
		 DrunkDelta < -MagicBuilderValidation.MaximumNeedDeltaAlcoholLitres || HungerDelta != 0 || ThirstDelta != 0)
		? "Direct alcohol removal requires finite nonpositive alcohol and zero hunger/thirst." : null);
    public IFuturemud Gameworld => Spell.Gameworld;

    public XElement SaveToXml()
    {
		if (_invalidDirectAlcoholDefinition is not null) return new XElement(_invalidDirectAlcoholDefinition);
        return new XElement("Effect",
            new XAttribute("type", "needdelta"),
            new XElement("Hunger", HungerDelta),
            new XElement("Thirst", ThirstDelta),
            new XElement("Drunk", DrunkDelta),
			DirectAlcoholRemoval ? new XElement("DirectAlcoholRemoval", true) : null
        );
    }

    public const string HelpText = @"Options:
    #3hunger <hours>#0 - change hunger satiation hours
    #3thirst <hours>#0 - change thirst satiation hours
    #3drunk <litres>#0 - change alcohol litres
	#3directalcohol true|false#0 - remove exact alcohol without need-rate modifiers (zero hunger/thirst required)";

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "hunger":
                return BuildingCommandHunger(actor, command);
            case "thirst":
                return BuildingCommandThirst(actor, command);
            case "drunk":
            case "alcohol":
                return BuildingCommandDrunk(actor, command);
			case "directalcohol":
				if (!bool.TryParse(command.SafeRemainingArgument, out var direct) || direct &&
					(HungerDelta != 0 || ThirstDelta != 0 || !double.IsFinite(DrunkDelta) || DrunkDelta > 0))
				{
					actor.OutputHandler.Send("Use true or false; exact removal requires zero hunger/thirst and nonpositive alcohol.");
					return false;
				}
				DirectAlcoholRemoval = direct;
				_directAlcoholLoadError = null;
				_invalidDirectAlcoholDefinition = null;
				Spell.Changed = true;
				actor.OutputHandler.Send($"Exact alcohol removal is now {direct.ToColouredString()}.");
				return true;
        }
        actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
        return false;
    }

    private bool BuildingCommandHunger(ICharacter actor, StringStack command)
    {
        if (command.IsFinished ||
            !MagicBuilderValidation.TryParseFiniteDoubleInRange(command.SafeRemainingArgument,
                -MagicBuilderValidation.MaximumNeedDeltaHours, MagicBuilderValidation.MaximumNeedDeltaHours,
                out double value))
        {
            actor.OutputHandler.Send($"You must enter a valid value in hours between {-MagicBuilderValidation.MaximumNeedDeltaHours:N0} and {MagicBuilderValidation.MaximumNeedDeltaHours:N0}.");
            return false;
        }
		if (DirectAlcoholRemoval && value != 0) return false;
        HungerDelta = value;
        Spell.Changed = true;
        actor.OutputHandler.Send($"Hunger change set to {value.ToStringN2(actor)} hours.");
        return true;
    }

    private bool BuildingCommandThirst(ICharacter actor, StringStack command)
    {
        if (command.IsFinished ||
            !MagicBuilderValidation.TryParseFiniteDoubleInRange(command.SafeRemainingArgument,
                -MagicBuilderValidation.MaximumNeedDeltaHours, MagicBuilderValidation.MaximumNeedDeltaHours,
                out double value))
        {
            actor.OutputHandler.Send($"You must enter a valid value in hours between {-MagicBuilderValidation.MaximumNeedDeltaHours:N0} and {MagicBuilderValidation.MaximumNeedDeltaHours:N0}.");
            return false;
        }
		if (DirectAlcoholRemoval && value != 0) return false;
        ThirstDelta = value;
        Spell.Changed = true;
        actor.OutputHandler.Send($"Thirst change set to {value.ToStringN2(actor)} hours.");
        return true;
    }

    private bool BuildingCommandDrunk(ICharacter actor, StringStack command)
    {
        if (command.IsFinished ||
            !MagicBuilderValidation.TryParseFiniteDoubleInRange(command.SafeRemainingArgument,
                -MagicBuilderValidation.MaximumNeedDeltaAlcoholLitres,
                MagicBuilderValidation.MaximumNeedDeltaAlcoholLitres, out double value))
        {
            actor.OutputHandler.Send($"You must enter a valid value in litres of alcohol between {-MagicBuilderValidation.MaximumNeedDeltaAlcoholLitres:N0} and {MagicBuilderValidation.MaximumNeedDeltaAlcoholLitres:N0}.");
            return false;
        }
		if (DirectAlcoholRemoval && value > 0) return false;
        DrunkDelta = value;
        Spell.Changed = true;
        actor.OutputHandler.Send($"Drunkenness change set to {value.ToStringN2(actor)} litres.");
        return true;
    }

    public string Show(ICharacter actor)
    {
        return SpellEffectPresentation.Describe(actor, "Need Delta",
            ("Hunger", HungerDelta.ToStringN2(actor).ColourValue()),
            ("Thirst", ThirstDelta.ToStringN2(actor).ColourValue()),
            ("Drunk", DrunkDelta.ToStringN2(actor).ColourValue()),
			("Exact Alcohol Removal", DirectAlcoholRemoval.ToColouredString()));
    }

    public bool IsInstantaneous => true;
    public bool RequiresTarget => true;

    public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
    {
		if (DirectAlcoholRemoval) return trigger.TargetTypes == "character";
        return IsCompatibleWithTrigger(trigger.TargetTypes);
    }

    public static bool IsCompatibleWithTrigger(string types)
    {
        switch (types)
        {
            case "character":
            case "characters":
                return true;
            default:
                return false;
        }
    }

    public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
    {
		if (DefinitionError is { } error) throw new InvalidOperationException(error);
		if (DirectAlcoholRemoval)
		{
			Apply(caster, target, outcome, power, parent, additionalParameters);
			return null!;
		}
        if (target is not ICharacter ch)
        {
            return null;
        }

        ch.Body.FulfilNeeds(new NeedFulfiller
        {
            SatiationPoints = HungerDelta,
            ThirstPoints = ThirstDelta,
            AlcoholLitres = DrunkDelta
        });
        return null;
    }

    public IMagicSpellEffectTemplate Clone()
    {
        return new NeedDeltaEffect(SaveToXml(), Spell);
    }

	public IMagicSpellEffectPreparedSelectionToken? CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (!DirectAlcoholRemoval || recipient is not ICharacter target || target.Body is null) return null;
		return _directAlcoholSelection = new(target, target.Body, target.Body.NeedsModel);
	}

	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster,
		IPerceivable recipient, out string? error)
	{
		error = null;
		if (!DirectAlcoholRemoval) return true;
		if (selection is not DirectAlcoholSelection captured || !ReferenceEquals(captured.Target, recipient) || !captured.IsCurrent)
		{
			error = "The selected alcohol recipient, physical body or needs model changed.";
			return false;
		}
		_directAlcoholSelection = captured;
		return true;
	}

	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = DirectAlcoholRemoval && (_directAlcoholSelection is not { IsCurrent: true } selected ||
			!ReferenceEquals(selected.Target, recipient)) ? "The selected alcohol recipient, physical body or needs model changed." : null;
		return error is null;
	}

	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (DefinitionError is { } error) throw new InvalidOperationException(error);
		if (!DirectAlcoholRemoval)
		{
			GetOrApplyEffect(caster, target!, outcome, power, parent, additionalParameters);
			return new(MagicEffectOperationStatus.Unknown, null);
		}
		if (target is not ICharacter character || character.Body is null) return new(MagicEffectOperationStatus.Rejected, null);
		var selected = _directAlcoholSelection ?? new DirectAlcoholSelection(character, character.Body, character.Body.NeedsModel);
		if (!ReferenceEquals(selected.Target, character) || !selected.IsCurrent) return new(MagicEffectOperationStatus.Rejected, null);
		if (_applyingDirectAlcohol) throw new InvalidOperationException("Alcohol removal cannot be re-entered during a native setter callback.");
		var before = selected.Needs.AlcoholLitres;
		if (!double.IsFinite(before) || before < 0) throw new InvalidOperationException("The selected alcohol reserve is invalid.");
		var amount = Math.Min(before, -DrunkDelta);
		if (amount <= 0) return new(MagicEffectOperationStatus.NoChange, null);
		var expected = before - amount;
		var save = selected.Needs.NeedsSave;
		if (!save) return new(MagicEffectOperationStatus.NoChange, null);
		if (!selected.IsCurrent) return new(MagicEffectOperationStatus.Rejected, null);
		_applyingDirectAlcohol = true;
		try
		{
			selected.Needs.AlcoholLitres = expected;
			var after = selected.Needs.AlcoholLitres;
			if (save && after != before) MarkDirty();
			if (!selected.IsCurrent || !double.IsFinite(after) || after != expected || before - after > amount + 1e-12)
				throw new InvalidOperationException("Alcohol removal state changed during a native setter callback; the paid write is uncertain.");
			return new(after < before ? MagicEffectOperationStatus.Applied : MagicEffectOperationStatus.NoChange, null);
		}
		catch
		{
			// A setter can write before its output callback throws. Preserve the exact owners for save/quarantine.
			try { if (selected.Needs.AlcoholLitres != before) MarkDirty(); }
			catch { MarkDirty(); }
			throw;
		}
		finally { _applyingDirectAlcohol = false; }

		void MarkDirty()
		{
			selected.Target.Changed = true;
			selected.Body.Changed = true;
		}
	}
}
