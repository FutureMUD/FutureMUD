#nullable enable

using MudSharp.FutureProg.Variables;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.NPC.AI;

namespace MudSharp.FutureProg.Functions.Health;

/// <summary>Delivers an explicitly measured native drug dose through normal absorption and metabolism.</summary>
internal sealed class DoseDrugFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var actor = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var drug = ParameterFunctions[1].Result?.GetObject as IDrug;
		var grams = Convert.ToDouble(ParameterFunctions[2].Result?.GetObject ?? 0m);
		var vector = ParameterFunctions[3].Result?.GetObject?.ToString();
		Result = new BooleanVariable(TryDose(actor, drug, grams, vector));
		return StatementResult.Normal;
	}

	internal static bool TryDose(ICharacter? actor, IDrug? drug, double grams, string? vectorText)
	{
		if (actor?.Body is not { } body || drug is null || !double.IsFinite(grams) || grams <= 0 ||
			!Enum.TryParse<DrugVector>(vectorText, true, out var vector) ||
			vector is not (DrugVector.Inhaled or DrugVector.Ingested or DrugVector.Injected or DrugVector.Touched) ||
			!drug.DrugVectors.HasFlag(vector) || !ReferenceEquals(body.Actor, actor) ||
			actor.State.HasFlag(CharacterState.Dead) || actor.EffectsOfType<ISpellProjectionBoundary>().Any() ||
			!CommandExecutionScope.TryContinue(actor) || !DrugDosePolicy.CanAccept(body, drug) ||
			DrugDosePolicy.Normalise(drug, grams) <= 0.0) return false;
		body.Dose(drug, vector, grams);
		return true;
	}

	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"dosedrug", [ProgVariableTypes.Character, ProgVariableTypes.Drug, ProgVariableTypes.Number, ProgVariableTypes.Text],
		(parameters, _) => new DoseDrugFunction(parameters), ["character", "drug", "grams", "vector"],
		["The physical recipient.", "The explicitly selected native drug.", "A positive finite mass in grams.",
			"One administration vector: inhaled, ingested, injected or touched."],
		"Queues a native drug dose for normal absorption and metabolism. Refuses invalid masses, unsupported vectors, dead recipients and projected bodies. Does not consume any item; the calling prog owns its delivery interaction.",
		"Health", ProgVariableTypes.Boolean));
}
