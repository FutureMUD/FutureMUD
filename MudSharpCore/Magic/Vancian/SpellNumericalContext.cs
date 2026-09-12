using ExpressionEngine;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp.Magic.Vancian;

public sealed record CapturedSpellExpression(string Location, string Original, string Formula, long VariableTrait,
	TraitBonusContext Context, IReadOnlyDictionary<string, double> Bindings);

public sealed class SpellNumericalContext : ISpellNumericalContext
{
	private readonly Dictionary<string, CapturedSpellExpression> _captured = new(StringComparer.Ordinal);
	public int SpellLevel { get; }
	public int CastingLevel { get; }
	public int CasterLevel { get; }
	public bool IsStored { get; }
	public Outcome Outcome { get; }
	public SpellPower Power { get; }
	public SpellNumericalContext(int spellLevel, int castingLevel, int casterLevel, SpellPower power, Outcome outcome, bool stored)
	{
		if (spellLevel < 0 || castingLevel < spellLevel || casterLevel < 0 || !Enum.IsDefined(power) || outcome is not (Outcome.MinorPass or Outcome.Pass or Outcome.MajorPass)) throw new FormatException("Invalid spell numerical context.");
		SpellLevel = spellLevel; CastingLevel = castingLevel; CasterLevel = casterLevel; Power = power; Outcome = outcome; IsStored = stored;
	}
	public void Capture(string location, ITraitExpression expression, ICharacter creator, ITraitDefinition? variable = null, TraitBonusContext bonusContext = TraitBonusContext.None)
	{
		if (!IsStored) return;
		if (expression is not TraitExpression traitExpression || expression.HasErrors()) throw new InvalidOperationException($"{location}: unsupported or invalid trait expression.");
		_captured.Add(location, new(location, expression.OriginalFormulaText, expression.Formula.OriginalExpression,
			variable?.Id ?? 0, bonusContext, traitExpression.CaptureNumericalBindings(creator, variable!, bonusContext)));
	}
	public double Evaluate(string location, ITraitExpression expression, IHaveTraits actor, ITraitDefinition? variable,
		TraitBonusContext context, IEnumerable<(string Name, object Value)> values)
	{
		var result = CheckOutcome.SimpleOutcome(CheckType.CastSpellCheck, Outcome);
		var numbers = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
		{
			["spelllevel"] = SpellLevel, ["castinglevel"] = CastingLevel, ["casterlevel"] = CasterLevel,
			["power"] = (int)Power, ["degrees"] = result.CheckDegrees(), ["success"] = result.SuccessDegrees()
		};
		foreach (var (name, value) in values) numbers[name] = value;
		if (!IsStored) return expression.EvaluateWith(actor, variable!, context, numbers.Select(x => (x.Key, x.Value)).ToArray());
		if (!_captured.TryGetValue(location, out var captured) || captured.Context != context || captured.VariableTrait != (variable?.Id ?? 0))
			throw new InvalidOperationException($"No compatible stored numerical binding for {location} / {context}.");
		foreach (var (name, value) in captured.Bindings) numbers[name] = value;
		var valueResult = new Expression(captured.Formula).EvaluateDoubleWith(numbers);
		if (!double.IsFinite(valueResult)) throw new InvalidOperationException($"{location}: formula returned a non-finite value.");
		return valueResult;
	}
	public void ValidateBinding(string location, ITraitExpression expression, ITraitDefinition? variable = null, TraitBonusContext context = TraitBonusContext.None)
	{
		if (!IsStored) return;
		if (!_captured.TryGetValue(location, out var captured) || captured.Context != context || captured.VariableTrait != (variable?.Id ?? 0) ||
			captured.Original != expression.OriginalFormulaText || captured.Formula != expression.Formula.OriginalExpression || expression.HasErrors())
			throw new InvalidOperationException($"Missing, corrupt or incompatible numerical binding at {location} / {context}.");
		// Validate completeness without evaluating any expression, options, target work or randomness.
		if (expression is not TraitExpression traitExpression || traitExpression.NumericalBindingNames.Any(key => !captured.Bindings.ContainsKey(key)))
			throw new InvalidOperationException($"Incomplete stored trait bindings at {location}.");
	}
	public XElement Save() => new("Numbers", new XAttribute("version", 1), new XAttribute("spelllevel", SpellLevel),
		new XAttribute("castinglevel", CastingLevel), new XAttribute("casterlevel", CasterLevel), new XAttribute("power", (int)Power),
		new XAttribute("outcome", (int)Outcome), new XAttribute("stored", IsStored), _captured.OrderBy(x => x.Key).Select(x =>
			new XElement("Expression", new XAttribute("location", x.Key), new XAttribute("variable", x.Value.VariableTrait),
				new XAttribute("context", (int)x.Value.Context), new XElement("Original", x.Value.Original), new XElement("Formula", x.Value.Formula),
				x.Value.Bindings.OrderBy(v => v.Key).Select(v => new XElement("Binding", new XAttribute("name", v.Key), new XAttribute("value", v.Value))))));
	public static SpellNumericalContext Load(XElement root)
	{
		if ((int?)root.Attribute("version") != 1) throw new FormatException("Unsupported numerical snapshot version.");
		var context = new SpellNumericalContext((int)root.Attribute("spelllevel")!, (int)root.Attribute("castinglevel")!, (int)root.Attribute("casterlevel")!,
			(SpellPower)(int)root.Attribute("power")!, (Outcome)(int)root.Attribute("outcome")!, (bool)root.Attribute("stored")!);
		foreach (var entry in root.Elements("Expression"))
		{
			var bindings = entry.Elements("Binding").ToDictionary(x => (string)x.Attribute("name")!, x => (double)x.Attribute("value")!, StringComparer.OrdinalIgnoreCase);
			if (bindings.Values.Any(x => !double.IsFinite(x))) throw new FormatException("Invalid stored numerical binding.");
			var expression = new CapturedSpellExpression((string)entry.Attribute("location")!, (string)entry.Element("Original")!,
				(string)entry.Element("Formula")!, (long)entry.Attribute("variable")!, (TraitBonusContext)(int)entry.Attribute("context")!, bindings.AsReadOnly());
			context._captured.Add(expression.Location, expression);
		}
		return context;
	}
}

/// <summary>Installed only on a detached invocation spell, never onto shared catalogue templates.</summary>
public sealed class ContextualSpellExpression : TraitExpression
{
	public ITraitExpression Source { get; }
	public SpellNumericalContext NumericalContext { get; }
	public string Location { get; }
	public ContextualSpellExpression(ITraitExpression source, SpellNumericalContext context, string location, IFuturemud gameworld)
		: base("0", gameworld) { Source = source; NumericalContext = context; Location = location; SetNoSave(true); }
	public override double Evaluate(IHaveTraits owner, ITraitDefinition variable = null!, TraitBonusContext context = TraitBonusContext.None) =>
		EvaluateWith(owner, variable, context);
	public override double EvaluateWith(IHaveTraits owner, ITraitDefinition variable = null!, TraitBonusContext context = TraitBonusContext.None, params (string Name, object Value)[] values) =>
		NumericalContext.Evaluate(Location, Source, owner, variable, context, values);
	public XElement SaveContext() => new("ContextualExpression", new XAttribute("location", Location), new XElement("Source", Source.OriginalFormulaText), NumericalContext.Save());
	public static ContextualSpellExpression LoadContext(XElement root, IFuturemud gameworld) => new(new TraitExpression((string)root.Element("Source")!, gameworld),
		SpellNumericalContext.Load(root.Element("Numbers")!), (string)root.Attribute("location")!, gameworld);
}
