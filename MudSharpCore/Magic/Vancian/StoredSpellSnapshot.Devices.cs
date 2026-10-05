using MudSharp.Body.Traits;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Checks;
using System.Text.Json;

#nullable enable
namespace MudSharp.Magic.Vancian;

public sealed partial class StoredSpellSnapshot
{
	/// <summary>Configuration and every frozen numerical input, excluding provenance and spendable IDs.</summary>
	internal string PotencyFingerprint => Hash(_model + "\n" + _duration + "\n" + _numbers);
	/// <summary>Proves at-least potency for deterministic damage/heal/duration fields over every legal opposed degree.
	/// Other adapters require equality; no monotonicity of arbitrary authored formulas is assumed.</summary>
	internal bool CanReproduceDevice(StoredSpellSnapshot stored, IFuturemud world)
	{
		if (PotencyFingerprint == stored.PotencyFingerprint) return true;
		var current = Numbers; var required = stored.Numbers;
		if (_model != stored._model || _duration != stored._duration || current.Grade != required.Grade ||
			current.Mastery < required.Mastery || current.Power != required.Power || current.Outcome != required.Outcome) return false;
		var currentEntries = current.Save().Elements("Expression").ToDictionary(x => (string)x.Attribute("location")!);
		var requiredEntries = required.Save().Elements("Expression").ToArray();
		if (currentEntries.Count != requiredEntries.Length) return false;
		foreach (var entry in requiredEntries)
		{
			var location = (string)entry.Attribute("location")!;
			if (!currentEntries.TryGetValue(location, out var live) || (string?)live.Element("Formula") != (string?)entry.Element("Formula") ||
				(string?)live.Element("Original") != (string?)entry.Element("Original") || (long?)live.Attribute("variable") != (long?)entry.Attribute("variable") ||
				(int?)live.Attribute("context") != (int?)entry.Attribute("context")) return false;
			var bindings = live.Elements("Binding").ToDictionary(x => (string)x.Attribute("name")!, x => (double)x.Attribute("value")!);
			if (bindings.Count != entry.Elements("Binding").Count() || entry.Elements("Binding").Any(x =>
				!bindings.TryGetValue((string)x.Attribute("name")!, out var value) || value < (double)x.Attribute("value")!)) return false;
			var expression = new TraitExpression((string)entry.Element("Original")!, world);
			if (expression.Formula.FunctionNames.Any(x => x.EqualToAny("rand", "drand", "dice") || !ExpressionEngine.Expression.IsSupportedFunction(x))) return false;
			if (location != "duration" && !location.EndsWith("/DamageExpression", StringComparison.Ordinal) && !location.EndsWith("/HealingAmount", StringComparison.Ordinal)) return false;
			var trait = (long)entry.Attribute("variable")! is var id && id != 0 ? world.Traits.Get(id) : null;
			var context = (TraitBonusContext)(int)entry.Attribute("context")!;
			var result = CheckOutcome.SimpleOutcome(CheckType.CastSpellCheck, required.Outcome);
			foreach (var degree in Enum.GetValues<OpposedOutcomeDegree>())
			{
				(string, object)[] inputs = [("outcome", (int)degree), ("degrees", result.CheckDegrees()), ("success", result.SuccessDegrees())];
				var before = required.Evaluate(location, expression, null!, trait, context, inputs);
				var after = current.Evaluate(location, expression, null!, trait, context, inputs);
				if (before < 0 || after < before) return false;
			}
		}
		return true;
	}

	/// <summary>Called only after the configured production service resolves current acquired entitlement.</summary>
	internal static StoredSpellSnapshot CaptureDevice(MagicSpell spell, ICharacter creator, ITraitDefinition trait,
		int grade, int mastery, SpellPower power, DateTime now)
	{
		var errors = ScrollSpellCompatibility.Errors(spell, false);
		if (errors.Count != 0) throw new InvalidOperationException(string.Join("\n", errors));
		var model = spell.SnapshotModel();
		model.CastingTraitDefinitionId = trait.Id;
		var definition = XElement.Parse(model.Definition);
		ScrollSpellCompatibility.ValidateReferences(definition, spell.Gameworld);
		definition.Element("Costs")!.RemoveNodes();
		definition.Element("Plan")!.ReplaceNodes(new XElement("Phase"));
		var scalarCaptures = new List<(string Location, TraitExpression Expression)>();
		foreach (var binding in spell.GradeProfile!.ScalarBindings)
		{
			var scalarExpression = new TraitExpression(binding.Expression, spell.Gameworld);
			scalarCaptures.Add(($"scalar/{binding.List}[{binding.Index}]/{binding.Field}", scalarExpression));
			var value = CastingNumerics.Bind(scalarExpression, trait, grade, power,
				$"{binding.List}[{binding.Index}]/{binding.Field}", spell.Gameworld, mastery).Evaluate(creator);
			if (!double.IsFinite(value)) throw new InvalidOperationException("Non-finite stored scalar potency.");
			definition.Element(binding.List == "target" ? "Effects" : "CasterEffects")!.Elements().ElementAt(binding.Index)
				.SetAttributeValue("bonus", value);
		}
		model.Definition = definition.ToString(SaveOptions.DisableFormatting);
		var context = new SpellNumericalContext(spell.SpellLevel, spell.SpellLevel, 0, power, Outcome.Pass, true, grade, mastery);
		foreach (var (location, expression) in scalarCaptures) context.Capture(location, expression, creator, captureVariable: trait, captureDeviceRaw: true);
		var copy = new MagicSpell(model, spell.Gameworld);
		if (spell.EffectDurationExpression is { } duration)
		{
			context.Capture("duration", duration, creator, trait, TraitBonusContext.SpellDuration, captureDeviceRaw: true);
			copy.EffectDurationExpression = duration;
		}
		foreach (var (effect, label) in copy.SpellEffects.Select((x, i) => (x, $"target[{i}]"))
			.Concat(copy.CasterSpellEffects.Select((x, i) => (x, $"caster[{i}]"))))
			foreach (var (field, expression, _) in ScrollSpellCompatibility.Expressions(effect))
				context.Capture($"{label}/{field}", expression, creator, captureVariable: trait, captureDeviceRaw: true);
		ScrollSpellCompatibility.Bind(copy, context, null);
		var json = JsonSerializer.Serialize(model);
		var durationText = spell.EffectDurationExpression?.OriginalFormulaText;
		return new(Guid.NewGuid(), spell.Id, spell.School.Id, MagicCastingService.Owner(creator).Id, now, json, durationText,
			context.Save().ToString(SaveOptions.DisableFormatting), Hash(json + "\n" + durationText));
	}
}
