using MudSharp.Body.Traits;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.Vancian;

#nullable enable
namespace MudSharp.Magic.Casting;

/// <summary>Live route bindings are installed only on an invocation's detached spell.</summary>
internal sealed class CastingExpression(ITraitExpression source, ITraitDefinition trait, int grade,
	SpellPower power, string location, IFuturemud gameworld, int controlledGrade) : TraitExpression("0", gameworld)
{
	private readonly TraitExpression _expression = Snapshot(source, gameworld, location);

	private static TraitExpression Snapshot(ITraitExpression expression, IFuturemud world, string field)
	{
		if (expression is not TraitExpression native)
			throw new InvalidOperationException($"{field}: unsupported numerical expression implementation.");
		lock (native)
		{
			return new TraitExpression(new Models.TraitExpression
			{
				Name = native.Name,
				Expression = native.OriginalFormulaText,
				TraitExpressionParameters = native.Parameters.Select(x => new Models.TraitExpressionParameters
				{
					Parameter = x.Key, TraitDefinitionId = x.Value.Trait.Id,
					CanImprove = x.Value.CanImprove, CanBranch = x.Value.CanBranch
				}).ToList()
			}, world);
		}
	}

	public override double Evaluate(IHaveTraits owner, ITraitDefinition variable = null!, TraitBonusContext context = TraitBonusContext.None)
		=> EvaluateWith(owner, variable, context);
	public override double EvaluateWith(IHaveTraits owner, ITraitDefinition variable = null!, TraitBonusContext context = TraitBonusContext.None,
		params (string Name, object Value)[] values)
	{
		var inputs = _expression.CaptureNumericalBindings(owner, trait, context)
			.ToDictionary(x => x.Key, x => (object)x.Value, StringComparer.OrdinalIgnoreCase);
		foreach (var (name, value) in values) inputs[name] = value;
		inputs["grade"] = grade; inputs["power"] = (int)power;
		inputs["mastery"] = controlledGrade;
		if (!_expression.Formula.TryEvaluateDoubleWith(inputs, out var result, out var error))
			throw new InvalidOperationException($"{location}: {error}");
		return result;
	}
}

internal static class CastingNumerics
{
	public static IEnumerable<string> ConfigurationErrors(MagicSpell spell)
	{
		foreach (var (resource, expression) in spell.CastingCosts)
			foreach (var error in Errors(expression, $"cost/{resource.Id}", "self")) yield return error;
		if (spell.EffectDurationExpression is { } duration)
			foreach (var error in Errors(duration, "duration", "degrees", "success")) yield return error;
		foreach (var (effect, label) in spell.SpellEffects.Select((x, i) => (x, $"target[{i}]"))
			.Concat(spell.CasterSpellEffects.Select((x, i) => (x, $"caster[{i}]"))))
		{
			var bindings = ScrollSpellCompatibility.Expressions(effect).ToArray();
			foreach (var (field, expression, _) in bindings)
				foreach (var error in Errors(expression, $"{label}/{field}", "outcome")) yield return error;
			// Inspection detects unadapted public expression fields; all writes use the typed adapters above.
			foreach (var property in effect.GetType().GetProperties().Where(x => typeof(ITraitExpression).IsAssignableFrom(x.PropertyType)))
				if (property.GetValue(effect) is ITraitExpression expression && !bindings.Any(x => ReferenceEquals(x.Expression, expression)))
					yield return $"{label}/{property.Name}: unsupported route-bound numerical field.";
			if (effect is TraitBoostEffect { Trait: null }) yield return $"{label}/boost.Trait: missing trait.";
			if (effect is CreateNPCEffect { DefinitionError: { } creationError }) yield return $"{label}/createnpc: {creationError}";
			if (effect is SpellArmourEffect armour && (armour.ArmourConfiguration.ArmourType is null || armour.ArmourConfiguration.ArmourMaterial is null))
				yield return $"{label}/spellarmour: missing armour type/material.";
		}
		if (spell.GradeProfile is not { } p) yield break;
		if (p.ScalarBindings.Select(x => (x.List, x.Index, x.Field)).Distinct().Count() != p.ScalarBindings.Count)
			yield return "ScalarBindings: duplicate field bindings.";
		foreach (var b in p.ScalarBindings)
		{
			var location = $"ScalarBindings/{b.List}[{b.Index}]/{b.Effect}.{b.Field}";
			var effects = b.List == "target" ? spell.SpellEffects.ToArray() : b.List == "caster" ? spell.CasterSpellEffects.ToArray() : [];
			if (b.Index < 0 || b.Index >= effects.Length || b.Effect != "boost" || b.Field != "Bonus" || effects[b.Index] is not TraitBoostEffect { Trait: not null })
				yield return $"{location}: invalid index, effect token, field or trait reference; only boost.Bonus is supported.";
			foreach (var error in Errors(new TraitExpression(b.Expression, spell.Gameworld), location)) yield return error;
		}
	}

	private static IEnumerable<string> Errors(ITraitExpression? expression, string location, params string[] contextParameters)
	{
		if (expression is null || expression.HasErrors()) { yield return $"{location}: missing or invalid expression."; yield break; }
		if (expression is not TraitExpression) yield return $"{location}: unsupported numerical expression implementation.";
		if (expression.Parameters.Values.Any(x => x.Trait is null)) yield return $"{location}: missing trait reference.";
		var allowed = new[] { "variable", "grade", "power", "mastery" }.Concat(contextParameters);
		foreach (var name in expression.NonTraitParameters.Where(x => !allowed.Contains(x, StringComparer.OrdinalIgnoreCase)))
			yield return $"{location}: unsupported numerical parameter {name}.";
	}

	public static ITraitExpression Bind(ITraitExpression expression, ITraitDefinition trait, int grade, SpellPower power,
		string field, IFuturemud world, int? controlledGrade = null) => new CastingExpression(expression, trait, grade, power, field, world, controlledGrade ?? grade);
}
