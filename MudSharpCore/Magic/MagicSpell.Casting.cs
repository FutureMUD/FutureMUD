using MudSharp.Body.Traits;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Vancian;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp.Magic;

public partial class MagicSpell
{
	internal int? InvocationGrade { get; private set; }
	internal Guid? InvocationOriginId { get; set; }
	public IReadOnlyDictionary<IMagicResource, ITraitExpression> CastingCosts => _castingCosts.AsReadOnly();
	internal MagicSpell CastingCopy(ICharacter actor, ITraitDefinition trait, int grade, SpellPower power, Difficulty difficulty, int? controlledGrade = null)
	{
		var model = SnapshotModel();
		model.CastingTraitDefinitionId = trait.Id;
		model.CastingDifficulty = (int)difficulty;
		var definition = XElement.Parse(model.Definition);
		foreach (var b in GradeProfile!.ScalarBindings)
		{
			var value = CastingNumerics.Bind(new TraitExpression(b.Expression, Gameworld), trait, grade, power,
				$"{b.List}[{b.Index}]/{b.Field}", Gameworld, controlledGrade).Evaluate(actor);
			definition.Element(b.List == "target" ? "Effects" : "CasterEffects")!.Elements().ElementAt(b.Index)
				.SetAttributeValue("bonus", value);
		}
		model.Definition = definition.ToString(SaveOptions.DisableFormatting);
		var copy = new MagicSpell(model, Gameworld);
		copy.InvocationGrade = grade;
		copy.SetNoSave(true);
		foreach (var (resource, expression) in copy._castingCosts.ToArray())
			copy._castingCosts[resource] = CastingNumerics.Bind(expression, trait, grade, power, $"cost/{resource.Id}", Gameworld, controlledGrade);
		if (EffectDurationExpression is { } duration)
			copy.EffectDurationExpression = CastingNumerics.Bind(duration, trait, grade, power, "duration", Gameworld, controlledGrade);
		foreach (var (effect, label) in copy.SpellEffects.Select((x, i) => (x, $"target[{i}]"))
			.Concat(copy.CasterSpellEffects.Select((x, i) => (x, $"caster[{i}]"))))
			foreach (var (field, expression, set) in ScrollSpellCompatibility.Expressions(effect))
				set(CastingNumerics.Bind(expression, trait, grade, power, $"{label}/{field}", Gameworld, controlledGrade));
		return copy;
	}
}
