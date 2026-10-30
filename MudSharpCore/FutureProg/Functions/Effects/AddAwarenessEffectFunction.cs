#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Effects;

internal sealed class AddAwarenessEffectFunction(IList<IFunction> parameters, bool listening) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Effect; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var actor = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var bonus = ParameterFunctions.Count > 1 ? (double)(decimal)(ParameterFunctions[1].Result?.GetObject ?? 0m)
			: actor?.Gameworld.GetStaticInt("CheckBonusPerDifficultyLevel") ?? 0;
		if (actor is null || !double.IsFinite(bonus) || bonus is < -1000 or > 1000 || (ParameterFunctions.Count == 1 && bonus <= 0))
		{
			Result = new NullVariable(ReturnType); return StatementResult.Normal;
		}
		var effect = actor.Effects.OfType<SpottingAwarenessStatus>().FirstOrDefault(x => (x is ListeningAwarenessStatus) == listening);
		if (effect is null)
		{
			effect = listening ? new ListeningAwarenessStatus(actor, bonus) : new SpottingAwarenessStatus(actor, bonus);
			actor.AddEffect(effect);
		}
		Result = effect; return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler()
	{
		foreach (var listening in new[] { false, true })
		foreach (var explicitBonus in new[] { false, true })
			FutureProg.RegisterBuiltInFunctionCompiler(new(listening ? "addlisteningeffect" : "addspottingeffect",
				explicitBonus ? [ProgVariableTypes.Character, ProgVariableTypes.Number] : [ProgVariableTypes.Character],
				(parameters, _) => new AddAwarenessEffectFunction(parameters, listening),
				explicitBonus ? ["target", "bonus"] : ["target"],
				explicitBonus ? ["The character.", "Native check bonus within -1000..1000."] : ["The character."],
				$"Creates saved {(listening ? "listening" : "spotting")} awareness, defaulting to one native difficulty-step bonus. Uses only the corresponding native perception check while conscious, awake and outside combat. Listening also permits checked adjacent-room speech. Returns the existing status without stacking/refreshing, or null for invalid inputs.",
				"Effects", ProgVariableTypes.Effect));
	}
}
