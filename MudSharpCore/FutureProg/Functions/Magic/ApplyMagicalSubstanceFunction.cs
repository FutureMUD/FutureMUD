#nullable enable

using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.NPC.AI;

namespace MudSharp.FutureProg.Functions.Magic;

internal sealed class ApplyMagicalSubstanceFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var actor = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var id = (decimal?)ParameterFunctions[1].Result?.GetObject ?? 0m;
		var amount = (double)((decimal?)ParameterFunctions[2].Result?.GetObject ?? 0m);
		var vectorName = ParameterFunctions[3].Result?.GetObject as string;
		var substance = id > 0 && id <= long.MaxValue && decimal.Truncate(id) == id ? actor?.Gameworld.MagicalSubstances.Get((long)id) : null;
		Result = new BooleanVariable(actor?.Body is { } body && ReferenceEquals(body.Actor, actor) && substance is not null &&
			!actor.State.HasFlag(CharacterState.Dead) && !actor.EffectsOfType<ISpellProjectionBoundary>().Any() &&
			CommandExecutionScope.TryContinue(actor) && Enum.TryParse<DrugVector>(vectorName, true, out var vector) &&
			MagicalExposure.Scripted(actor, substance, amount, vector));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"applymagicalsubstance", [ProgVariableTypes.Character, ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Text],
		(parameters, _) => new ApplyMagicalSubstanceFunction(parameters), ["recipient", "substanceId", "quantity", "vector"],
		["A living physical recipient.", "An installed magical substance ID.", "Positive quantity in the substance's configured units.", "A supported exposure vector, e.g. ingested."],
		"Delivers a prepared magical substance using native dose, interdiction, resistance and effect rules. Returns whether delivery was admitted, not whether an effect succeeded. Does not cast a spell or charge Mana.",
		"Magic", ProgVariableTypes.Boolean));
}
