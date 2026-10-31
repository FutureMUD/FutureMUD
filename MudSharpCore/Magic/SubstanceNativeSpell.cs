#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.SpellTriggers;
using MudSharp.Magic.Vancian;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

/// <summary>Opt-in fixed-grade preparations reuse native spell admission and effect execution.</summary>
internal static class SubstanceNativeSpell
{
	private static readonly HashSet<string> AdditionalEffects = new(StringComparer.OrdinalIgnoreCase)
	{
		"dispelmagic", "waystep", "returntether", "createshelter", "phantomillusion",
		"sourcefury", "sourcecalm", "levitate", "movementtempo", "powertagsuppression", "poison", "delayedlocationhint"
	};

	internal static IEnumerable<string> Errors(IMagicSpell? source, SubstanceEffectEntry entry)
	{
		if (source is not MagicSpell spell) { yield return "Missing native spell."; yield break; }
		if (entry.NativeSpellGrade is < 1 or > 7 || spell.GradeProfile?.Grades.All(x => x.Grade != entry.NativeSpellGrade) != false)
			yield return "Choose an available native spell grade from 1 to 7.";
		if (entry.Lifecycle != SubstanceLifecycle.Activation || entry.Scaling != SubstanceScaling.Magnitude ||
			!double.IsFinite(entry.MinimumDose) || entry.MinimumDose <= 0 || !double.IsFinite(entry.MaximumDose) || entry.MaximumDose < entry.MinimumDose)
			yield return "Native spell entries require activation, magnitude scaling and a positive finite dose threshold and cap.";
		if (spell.CastingTrait is null) yield return "The native spell has no casting trait definition.";
		if (spell.Trigger is not (CastingTriggerCharacter or CastingTriggerSelf or CastingTriggerRoom or CastingTriggerWaystepDestination))
			yield return "Native substance spells must target their recipient, the recipient's room, or a random Waystep destination.";
		if (spell.GradeProfile?.Area is not null || spell.CasterSpellEffects.Any() || !spell.SpellEffects.Any())
			yield return "Native substance spells require a complete non-area target payload with no separate caster effects.";
		foreach (var error in CastingNumerics.ConfigurationErrors(spell)) yield return error;
		foreach (var error in spell.GradeConfigurationErrors()) yield return error;
		if (spell.LifetimeConfigurationError is { } lifetimeError) yield return lifetimeError;
		if (spell.Trigger is CastingTriggerCharacter character && character.TargetFilterError is { } filterError)
			yield return filterError;
		if (spell.EffectDurationExpression is null && spell.SpellEffects.Any(x => !x.IsInstantaneous))
			yield return "Persistent native substance effects require a duration expression.";
		foreach (var effect in spell.SpellEffects)
		{
			if (spell.Trigger is null || !effect.IsCompatibleWithTrigger(spell.Trigger))
				yield return "The native effect is incompatible with its spell trigger.";
			if (!SubstanceSpellResolver.Supported(effect) && !AdditionalEffects.Contains((string?)effect.SaveToXml().Attribute("type") ?? ""))
				yield return "The complete native spell contains an effect without an admitted substance adapter.";
			foreach (var (_, expression, _) in ScrollSpellCompatibility.Expressions(effect))
				foreach (var error in FixedInputErrors(expression)) yield return error;
		}
		if (spell.EffectDurationExpression is { } duration)
			foreach (var error in FixedInputErrors(duration)) yield return error;
		foreach (var scalar in spell.GradeProfile?.ScalarBindings ?? [])
			foreach (var error in FixedInputErrors(new TraitExpression(scalar.Expression, spell.Gameworld))) yield return error;
	}

	private static IEnumerable<string> FixedInputErrors(ITraitExpression expression)
	{
		if (expression.Parameters.Count != 0 || expression.NonTraitParameters.Any(x =>
			!new[] { "grade", "power", "mastery", "outcome", "degrees", "success" }.Contains(x, StringComparer.OrdinalIgnoreCase)))
			yield return "Prepared native spell potency cannot depend on a consumer's traits or other variable inputs.";
	}

	internal static void Apply(ICharacter actor, IMagicSpell? source, SubstanceEffectEntry entry)
	{
		if (actor.Body is null || !ReferenceEquals(actor.Body.Actor, actor) || actor.Location is null ||
			actor.State.HasFlag(CharacterState.Dead) || actor.EffectsOfType<ISpellProjectionBoundary>().Any() ||
			Errors(source, entry).Any()) return;
		var original = (MagicSpell)source!;
		try
		{
			var power = original.GradeProfile!.Grades.Single(x => x.Grade == entry.NativeSpellGrade).Power;
			var spell = original.CastingCopy(actor, original.CastingTrait!, entry.NativeSpellGrade, power, Difficulty.Normal);
			IPerceivable target = spell.Trigger is CastingTriggerRoom ? actor.Location : actor;
			if (spell.Trigger is CastingTriggerCharacter filter && !filter.AllowsTarget(actor, actor)) return;
			SpellAdditionalParameter[] parameters = spell.Trigger is CastingTriggerWaystepDestination
				? [new() { ParameterName = SpellTravelDestinationRequest.ParameterName, Item = new SpellTravelDestinationRequest(null) }]
				: [];
			spell.BindInvocationInputs(actor, target, parameters);
			var prepared = spell.SpellEffects.OfType<IMagicSpellEffectPreparedSelection>().ToArray();
			foreach (var effect in prepared) effect.CapturePreparedSelection(actor, target);
			var invocation = new SpellInvocationContext(SpellInvocationSource.SubstanceActivation, Outcome.Pass, confirm =>
			{
				if (actor.State.HasFlag(CharacterState.Dead) || !ReferenceEquals(actor.Gameworld.MagicSpells.Get(original.Id), original) ||
					prepared.Any(x => !x.TryConfirmPreparedSelection(actor, target, out _))) return false;
				confirm();
				return true;
			});
			spell.CastVancian(actor, target, power, invocation, parameters);
		}
		catch (Exception error) when (error is InvalidOperationException or OverflowException)
		{
			// Native eligibility and prepared-selection refusals consume this entry's activation only.
			// Later entries remain independent, matching an ordered preparation's complete payload.
		}
	}
}
