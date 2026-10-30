#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.FutureProg.Variables;
using MudSharp.Character;

namespace MudSharp.FutureProg.Functions.Effects;

internal enum CombatFrenzyOperation
{
	Add,
	Remove,
	Has
}

internal sealed class CombatFrenzyFunction(IList<IFunction> parameters, CombatFrenzyOperation operation)
	: BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get; protected set; } =
		operation == CombatFrenzyOperation.Add ? ProgVariableTypes.Effect : ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = ParameterFunctions[0].Result?.GetObject as ICharacter;
		if (character is null)
		{
			Result = operation == CombatFrenzyOperation.Add
				? new NullVariable(ReturnType)
				: new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var allEffects = character.Effects.OfType<CombatFrenzyEffect>().ToList();
		var activeEffects = allEffects.Where(effect => effect.Applies()).ToList();
		if (operation == CombatFrenzyOperation.Has)
		{
			Result = new BooleanVariable(activeEffects.Any());
			return StatementResult.Normal;
		}

		if (operation == CombatFrenzyOperation.Remove)
		{
			foreach (var effect in allEffects)
			{
				character.RemoveEffect(effect);
			}

			Result = new BooleanVariable(activeEffects.Count > 0);
			return StatementResult.Normal;
		}

		var seconds = (double)(decimal)(ParameterFunctions[1].Result?.GetObject ?? 0m);
		var offenceBonus = (double)(decimal)(ParameterFunctions[2].Result?.GetObject ?? 0m);
		var defenceBonus = (double)(decimal)(ParameterFunctions[3].Result?.GetObject ?? 0m);
		var meleeDamageBonus = (double)(decimal)(ParameterFunctions[4].Result?.GetObject ?? 0m);
		var hungerMultiplier = (double)(decimal)(ParameterFunctions[5].Result?.GetObject ?? 1m);
		var sourceBody = character.Body;
		if (sourceBody is null || !character.IsEmbodied || character.State.HasFlag(CharacterState.Dead) ||
		    character.InstanceKind == CharacterInstanceKind.AstralProjection || sourceBody.CurrentStamina <= 0.0 ||
		    !double.IsFinite(seconds) || seconds <= 0.0 ||
		    seconds > TimeSpan.MaxValue.TotalSeconds / 2.0 || !double.IsFinite(offenceBonus) ||
		    !double.IsFinite(defenceBonus) || !double.IsFinite(meleeDamageBonus) ||
		    !double.IsFinite(hungerMultiplier) || hungerMultiplier < 0.0)
		{
			Result = new NullVariable(ReturnType);
			return StatementResult.Normal;
		}

		foreach (var effect in allEffects)
		{
			character.RemoveEffect(effect);
		}
		if (character.Effects.OfType<CombatFrenzyEffect>().Any())
		{
			Result = new NullVariable(ReturnType);
			return StatementResult.Normal;
		}
		var currentBody = character.Body;
		if (currentBody is null || currentBody.Id != sourceBody.Id || !character.IsEmbodied ||
		    character.State.HasFlag(CharacterState.Dead) ||
		    character.InstanceKind == CharacterInstanceKind.AstralProjection ||
		    currentBody.CurrentStamina <= 0.0)
		{
			Result = new NullVariable(ReturnType);
			return StatementResult.Normal;
		}

		var frenzy = new CombatFrenzyEffect(character, offenceBonus, defenceBonus, meleeDamageBonus, hungerMultiplier);
		character.AddEffect(frenzy, TimeSpan.FromSeconds(seconds));
		Result = frenzy;
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new("addcombatfrenzyeffect",
			[ProgVariableTypes.Character, ProgVariableTypes.Number, ProgVariableTypes.Number,
				ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Number],
			(parameters, _) => new CombatFrenzyFunction(parameters, CombatFrenzyOperation.Add),
			["character", "seconds", "offencebonus", "defencebonus", "meleedamagebonus", "hungermultiplier"],
			["The character to affect.", "Effect duration in real seconds.", "Bonus to offensive combat checks.",
				"Bonus to defensive combat checks.", "Fixed bonus to physical melee damage.", "Hunger-rate multiplier."],
			"Adds or refreshes a saved combat frenzy effect, scoped to the character's current body.",
			"Effects", ProgVariableTypes.Effect));
		FutureProg.RegisterBuiltInFunctionCompiler(new("removecombatfrenzyeffect",
			[ProgVariableTypes.Character],
			(parameters, _) => new CombatFrenzyFunction(parameters, CombatFrenzyOperation.Remove),
			["character"], ["The character whose frenzy should end."],
			"Removes all active combat frenzy effects from a character and returns whether one was removed.",
			"Effects", ProgVariableTypes.Boolean));
		FutureProg.RegisterBuiltInFunctionCompiler(new("hascombatfrenzyeffect",
			[ProgVariableTypes.Character],
			(parameters, _) => new CombatFrenzyFunction(parameters, CombatFrenzyOperation.Has),
			["character"], ["The character to check."],
			"Returns whether the character has an active combat frenzy effect on its current body.",
			"Effects", ProgVariableTypes.Boolean));
	}
}
