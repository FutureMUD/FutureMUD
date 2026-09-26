#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.FutureProg;
using MudSharp.Magic.Environment;
using MudSharp.Magic.Vancian;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public sealed class RejuvenateLandEffect : IMagicSpellEffectTemplate, IMagicSpellEffectAdmission
{
	public const string HelpText = @"Gradually repairs existing scars on a physical room, once per cell.
	#3budget <expression>#0 - total scar repair, capped to damage at installation
	#3rate <expression>#0 - scar units per real minute
	#3eligibility <prog|none>#0 - NotStatic boolean (character, location) admission policy
	#3continuation <prog|none>#0 - same signature; requires the active original caster
	#3local <on|off>#0 - require the original conscious acting instance in its captured cell/plane/layer
	#3desc <text|none>#0 - plain description addendum (no substitution placeholders)
	#3colour <colour>#0 - addendum colour

Power is the invocation's SpellPower integer; outcome is its opposed-outcome degree integer.
Vancian direct invocations also supply spelllevel, castinglevel, casterlevel, degrees and success.
Budget and rate are captured once. A finite positive spell duration is required. A second
treatment is refused without replacing the first. Dispel discards uncommitted time;
normal expiry accounts its final interval. Completed scar repair is permanent.
Stored scrolls and magical substances are unsupported.";

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("rejuvenateland", (xml, spell) => new RejuvenateLandEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("rejuvenateland", (_, spell) =>
			(new RejuvenateLandEffect(new XElement("Effect", new XAttribute("type", "rejuvenateland"),
				new XAttribute("version", 1), new XElement("Budget", "1"), new XElement("Rate", "1")), spell), string.Empty),
			"Establishes one bounded, gradual scar-repair treatment in a physical cell", HelpText, false, true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => SpellTriggerFactory.BuilderInfoForType(x).TargetTypes is "room" or "rooms").ToArray());
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public ITraitExpression BudgetExpression { get; internal set; }
	public ITraitExpression RateExpression { get; internal set; }
	public long? EligibilityProgId { get; private set; }
	public long? ContinuationProgId { get; private set; }
	public bool RequiresPresence { get; private set; }
	public string Description { get; private set; }
	public ANSIColour Colour { get; private set; }
	private readonly int _version;
	private readonly string? _loadError;

	public RejuvenateLandEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		_version = int.TryParse(root.Attribute("version")?.Value, out var version) ? version : 0;
		BudgetExpression = new TraitExpression(root.Element("Budget")?.Value ?? "", Gameworld);
		RateExpression = new TraitExpression(root.Element("Rate")?.Value ?? "", Gameworld);
		try
		{
			EligibilityProgId = (long?)root.Element("EligibilityProg");
			ContinuationProgId = (long?)root.Element("ContinuationProg");
			RequiresPresence = (bool?)root.Element("Local") ?? false;
		}
		catch (FormatException) { _loadError = "Malformed policy or locality configuration."; }
		Description = root.Element("Description")?.Value ?? string.Empty;
		Colour = Telnet.GetColour(root.Element("Colour")?.Value ?? "green") ?? Telnet.Green;
	}

	private static ITraitExpression Source(ITraitExpression expression) => expression is ContextualSpellExpression contextual ? contextual.Source : expression;
	private string? ExpressionError(ITraitExpression expression)
	{
		var source = Source(expression);
		if (source.HasErrors()) return "Invalid trait expression: " + source.Error;
		if (source.NonTraitParameters.Except(new[] { "power", "outcome", "spelllevel", "castinglevel", "casterlevel", "degrees", "success", "variable" }, StringComparer.OrdinalIgnoreCase).Any())
			return "Unknown numerical parameter in repair expression.";
		if (source.Parameters.Any(x => x.Value.Trait is null)) return "A referenced trait is missing.";
		return null;
	}

	public static bool ValidPolicy(IFutureProg? prog) => prog is not null && prog.StaticType == FutureProgStaticType.NotStatic &&
		prog.ReturnType == ProgVariableTypes.Boolean && string.IsNullOrEmpty(prog.CompileError) &&
		!prog.AcceptsAnyParameters && prog.Parameters.SequenceEqual([ProgVariableTypes.Character, ProgVariableTypes.Location]);

	public string? DefinitionError => _version != 1 ? "Unsupported rejuvenateland template version." : _loadError ??
		ExpressionError(BudgetExpression) ?? ExpressionError(RateExpression) ??
		(EligibilityProgId is { } eligibility && !ValidPolicy(Gameworld.FutureProgs.Get(eligibility)) ? "Invalid eligibility prog reference/signature." : null) ??
		(ContinuationProgId is { } continuation && !ValidPolicy(Gameworld.FutureProgs.Get(continuation)) ? "Invalid continuation prog reference/signature." : null) ??
		(Description.IndexOfAny(['{', '}', '$']) >= 0 ? "Description addenda accept plain text without substitution placeholders." : null);

	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger.TargetTypes is "room" or "rooms";
	public IMagicSpellEffectTemplate Clone() => new RejuvenateLandEffect(SaveToXml(), Spell);
	public XElement SaveToXml() => new("Effect", new XAttribute("type", "rejuvenateland"), new XAttribute("version", _version),
		new XElement("Budget", Source(BudgetExpression).OriginalFormulaText), new XElement("Rate", Source(RateExpression).OriginalFormulaText),
		EligibilityProgId.HasValue ? new XElement("EligibilityProg", EligibilityProgId.Value) : null,
		ContinuationProgId.HasValue ? new XElement("ContinuationProg", ContinuationProgId.Value) : null,
		new XElement("Local", RequiresPresence), new XElement("Description", new XCData(Description)), new XElement("Colour", Colour.Name));

	public bool TryPrepareApplication(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, TimeSpan resolvedDuration, out IMagicSpellEffectApplication? application, out string? error)
	{
		application = null;
		error = DefinitionError;
		if (error is not null) return false;
		if (target is not ICell cell || !ReferenceEquals(cell.Gameworld, Gameworld) || !ReferenceEquals(caster.Gameworld, Gameworld))
		{ error = "Rejuvenation requires an actual physical cell in the caster's gameworld."; return false; }
		if (resolvedDuration <= TimeSpan.Zero || resolvedDuration == TimeSpan.MaxValue)
		{ error = "Rejuvenation requires a finite positive resolved spell duration."; return false; }
		var service = Gameworld.EnvironmentalMagic;
		if (service is null || !service.CanInstallTreatment(cell, out error)) return false;
		if (EligibilityProgId is { } id && !service.EvaluateRepairPolicy(cell, caster, Gameworld.FutureProgs.Get(id)!, out error)) return false;
		try
		{
			foreach (var expression in new[] { BudgetExpression, RateExpression })
				if (expression is not ContextualSpellExpression && Source(expression).NonTraitParameters.Any(x =>
					x.EqualToAny("spelllevel", "castinglevel", "casterlevel", "degrees", "success")))
				{ error = "This expression requires numerical bindings supplied by a Vancian direct invocation."; return false; }
			var castingTrait = (Spell as MagicSpell)?.CastingTrait;
			var budget = BudgetExpression.EvaluateWith(caster, castingTrait!, TraitBonusContext.SpellDuration,
				("power", (int)power), ("outcome", (int)outcome));
			var rate = RateExpression.EvaluateWith(caster, castingTrait!, TraitBonusContext.SpellDuration,
				("power", (int)power), ("outcome", (int)outcome));
			if (!double.IsFinite(budget) || !double.IsFinite(rate) || budget < 0.0 || rate < 0.0 || !double.IsFinite(rate * (resolvedDuration.TotalSeconds / 60.0)))
			{ error = "Repair budget/rate must be finite and non-negative, without lifetime arithmetic overflow."; return false; }
			if (budget == 0.0 || rate == 0.0) { error = "Zero budget or rate establishes no treatment."; return false; }
			budget = Math.Min(budget, service.InspectState(cell).State.ScarDamage);
			var progress = new LandRejuvenationProgress
			{
				Id = Guid.NewGuid(), CellId = cell.Id, SpellId = Spell.Id, CasterId = caster.Id, ActingInstanceId = caster.InstanceId,
				PlaneIds = caster.GetPlanarPresence().PresencePlaneIds.OrderBy(x => x).ToArray(),
				Layer = (int)caster.RoomLayer, ProfileId = service.InspectRepairPolicy(cell).ProfileId!.Value,
				RequiresPresence = RequiresPresence, ContinuationProgId = ContinuationProgId, Rate = rate,
				InitialBudget = budget, RemainingBudget = budget, RemainingSeconds = resolvedDuration.TotalSeconds
			};
			application = new Application(cell, caster, progress, Description, Colour);
			return true;
		}
		catch (Exception ex) { error = $"Rejuvenation evaluation failed: {ex.Message}"; return false; }
	}

	private sealed record Application(ICell Cell, ICharacter Caster, LandRejuvenationProgress Progress, string Description,
		ANSIColour Colour) : IMagicSpellEffectApplication
	{
		public IMagicSpellEffect Create(IMagicSpellEffectParent parent) => new SpellRejuvenateLandEffect(Cell, parent, Caster,
			Progress with { ParentId = ((MagicSpellParent)parent).Identity }, Description, Colour);
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters) => null;

	public string Show(ICharacter actor) => SpellEffectPresentation.Describe(actor, "Bounded Land Rejuvenation",
		("Budget", Source(BudgetExpression).OriginalFormulaText.ColourCommand()), ("Rate / real minute", Source(RateExpression).OriginalFormulaText.ColourCommand()),
		("Eligibility", (EligibilityProgId?.ToString(actor) ?? "none").ColourValue()), ("Continuation", (ContinuationProgId?.ToString(actor) ?? "none").ColourValue()),
		("Local", RequiresPresence.ToColouredString()), ("Description", Description.Length == 0 ? "none" : Description.Colour(Colour)),
		("Validation", DefinitionError?.ColourError() ?? "Valid".ColourValue()));

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var setting = command.PopForSwitch();
		var text = command.SafeRemainingArgument;
		switch (setting)
		{
			case "budget":
			case "rate":
				var expression = new TraitExpression(text, Gameworld);
				if (ExpressionError(expression) is { } problem) { actor.OutputHandler.Send(problem.ColourError()); return false; }
				if (setting == "budget") BudgetExpression = expression; else RateExpression = expression;
				break;
			case "eligibility":
			case "continuation":
				var prog = text.EqualTo("none") ? null : Gameworld.FutureProgs.GetByIdOrName(text);
				if (!text.EqualTo("none") && !ValidPolicy(prog)) { actor.OutputHandler.Send("Use a compiled NotStatic boolean (character, location) prog or none."); return false; }
				if (setting == "eligibility") EligibilityProgId = prog?.Id; else ContinuationProgId = prog?.Id;
				break;
			case "local":
				if (!text.EqualToAny("on", "off")) { actor.OutputHandler.Send("Use local <on|off>."); return false; }
				RequiresPresence = text.EqualTo("on");
				break;
			case "desc":
				if (text.IndexOfAny(['{', '}', '$']) >= 0 || text.Length == 0) { actor.OutputHandler.Send("Use desc <plain text|none>, without substitution placeholders."); return false; }
				Description = text.EqualTo("none") ? string.Empty : text.Sanitise();
				break;
			case "colour":
				if (Telnet.GetColour(text) is not { } colour) { actor.OutputHandler.Send("Choose a valid presentation colour."); return false; }
				Colour = colour;
				break;
			default: actor.OutputHandler.Send(HelpText.SubstituteANSIColour()); return false;
		}
		Spell.Changed = true;
		actor.OutputHandler.Send($"Rejuvenation {setting.ColourName()} set to {text.ColourValue()}.");
		return true;
	}
}
