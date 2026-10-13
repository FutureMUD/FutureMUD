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

	/// <summary>Null means deferred. Never read traits or run functions before the casting roll.</summary>
	internal bool TryEvaluateFixedInputs(out double? value, out string? error)
	{
		value = null;
		error = null;
		var inputs = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
		{
			["grade"] = grade, ["power"] = (int)power, ["mastery"] = controlledGrade
		};
		if (_expression.Parameters.Count != 0 || _expression.Formula.FunctionNames.Any() ||
			_expression.Formula.ParameterNames.Any(name => !inputs.ContainsKey(name))) return true;
		if (!_expression.Formula.TryEvaluateDoubleWith(inputs, out var result, out var diagnostic))
		{
			error = $"{location}: {diagnostic}";
			return false;
		}
		value = result;
		return true;
	}

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
			if (effect is DiseaseEffect { DefinitionError: { } diseaseError }) yield return $"{label}/disease: {diseaseError}";
			if (effect is PoisonEffect { DefinitionError: { } poisonError }) yield return $"{label}/poison: {poisonError}";
			if (effect is RemovePoisonEffect { DefinitionError: { } removePoisonError }) yield return $"{label}/removepoison: {removePoisonError}";
			if (effect is DamageEffect { DefinitionError: { } damageError }) yield return $"{label}/damage: {damageError}";
			if (effect is RemoveDiseaseEffect { DefinitionError: { } cureError }) yield return $"{label}/removedisease: {cureError}";
			if (effect is NeedDeltaEffect { DefinitionError: { } needsError }) yield return $"{label}/needdelta: {needsError}";
			if (effect is StaminaDeltaSpellEffect { DefinitionError: { } staminaError }) yield return $"{label}/staminadelta: {staminaError}";
			if (effect is MagicResourceDeltaEffect { DefinitionError: { } resourceDeltaError }) yield return $"{label}/magicresourcedelta: {resourceDeltaError}";
			if (effect is CreateNPCEffect { DefinitionError: { } creationError }) yield return $"{label}/createnpc: {creationError}";
			if (effect is CreateShelterEffect { DefinitionError: { } shelterError }) yield return $"{label}/createshelter: {shelterError}";
			if (effect is CreateLiquidEffect { DefinitionError: { } liquidError }) yield return $"{label}/createliquid: {liquidError}";
			if (effect is CreateItemEffect { DefinitionError: { } itemCreationError }) yield return $"{label}/createitem: {itemCreationError}";
			if (effect is AnimateCorpseSpellEffect { DefinitionError: { } animationError }) yield return $"{label}/animatecorpse: {animationError}";
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
			if (b.Index < 0 || b.Index >= effects.Length || !SupportsScalarField(effects[b.Index], b.Effect, b.Field))
				yield return $"{location}: invalid index, effect token, field or typed dependency for the supported scalar adapter.";
			foreach (var error in Errors(new TraitExpression(b.Expression, spell.Gameworld), location)) yield return error;
		}
	}

	internal static bool SupportsScalarField(IMagicSpellEffectTemplate effect, string token, string field) => (effect, token, field) switch
	{
		(TraitBoostEffect { Trait: not null }, "boost", "Bonus") => true,
		(RoomLightEffect, "roomlight", "AddedLuxPerPower") => true,
		(RoomTemperatureEffect, "roomtemperature", "TemperatureDeltaPerPower") => true,
		(ItemEnchantEffect, "itemenchant", "AttackCheckBonus") => true,
		(TrackMarkEffect, "trackmark", "VisualTrackIntensityMultiplier") => true,
		(HealingRateSpellEffect, "healingrate", "Multiplier") => true,
		(NeedDeltaEffect, "needdelta", "DrunkDelta") => true,
		(NeedDeltaEffect, "needdelta", "ThirstDelta") => true,
		(ForcedPathMovementEffect, "forcedpathmovement", "Steps") => true,
		_ => false
	};

	internal static void WriteScalar(XElement effect, string token, string field, double value)
	{
		if (!double.IsFinite(value)) throw new InvalidOperationException($"{token}.{field}: non-finite scalar potency.");
		if (effect.Name != "Effect" || (string?)effect.Attribute("type") != token)
			throw new InvalidOperationException("The scalar effect discriminator changed.");
		switch (token, field)
		{
			case ("boost", "Bonus"): effect.SetAttributeValue("bonus", value); break;
			case ("roomlight", "AddedLuxPerPower"): effect.SetElementValue("AddedLuxPerPower", value); break;
			case ("roomtemperature", "TemperatureDeltaPerPower"): effect.SetElementValue("TemperatureDeltaPerPower", value); break;
			case ("itemenchant", "AttackCheckBonus"): effect.SetElementValue("AttackCheckBonus", value); break;
			case ("trackmark", "VisualTrackIntensityMultiplier"): effect.SetElementValue("VisualMultiplier", value); break;
			case ("healingrate", "Multiplier"): effect.SetElementValue("Multiplier", value); break;
			case ("needdelta", "DrunkDelta"):
				if (Math.Abs(value) > MagicBuilderValidation.MaximumNeedDeltaAlcoholLitres)
					throw new InvalidOperationException("Alcohol change exceeds the native need limit before payment.");
				if ((bool?)effect.Element("DirectAlcoholRemoval") == true && value > 0)
					throw new InvalidOperationException("Exact alcohol removal requires a nonpositive bounded scalar before payment.");
				effect.SetElementValue("Drunk", value); break;
			case ("needdelta", "ThirstDelta"):
				if (Math.Abs(value) > MagicBuilderValidation.MaximumNeedDeltaHours)
					throw new InvalidOperationException("Thirst change exceeds the native need limit before payment.");
				effect.SetElementValue("Thirst", value); break;
			case ("forcedpathmovement", "Steps"):
				if (value < 1 || value > MagicBuilderValidation.MaximumForcedPathMovementSteps || value != Math.Truncate(value))
					throw new InvalidOperationException("Forced path steps must be a whole number within the native builder limit before payment.");
				effect.SetElementValue("Steps", (int)value); break;
			default: throw new InvalidOperationException($"{token}.{field}: unsupported scalar field.");
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

	internal static void ValidateOwnedDiseaseInputs(IMagicSpellEffectTemplate effect, int grade)
	{
		if (effect is not DiseaseEffect { OwnedFamily: not null } disease) return;
		if (disease.IntensityExpression is not CastingExpression intensity ||
			!intensity.TryEvaluateFixedInputs(out var strength, out _) || strength != grade ||
			disease.VirulenceExpression is not CastingExpression virulence ||
			!virulence.TryEvaluateFixedInputs(out var multiplier, out _) || multiplier != 1.0)
			throw new InvalidOperationException("Owned sickness requires fixed selected-grade intensity and unit virulence before payment.");
	}

	internal static void ValidateOwnedPoisonInputs(IMagicSpellEffectTemplate effect)
	{
		if (effect is not PoisonEffect { OwnedFamily: not null } poison) return;
		if (poison.GramsExpression is not CastingExpression grams ||
			!grams.TryEvaluateFixedInputs(out var amount, out _) ||
			amount is null || !double.IsFinite(amount.Value) || amount <= 0)
			throw new InvalidOperationException("Owned poison requires a fixed, finite positive selected-grade dose before payment.");
	}

	public static ITraitExpression Bind(ITraitExpression expression, ITraitDefinition trait, int grade, SpellPower power,
		string field, IFuturemud world, int? controlledGrade = null) => new CastingExpression(expression, trait, grade, power, field, world, controlledGrade ?? grade);
}
