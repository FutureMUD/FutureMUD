#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Location;

internal sealed class CanHearNoise : BuiltInFunction
{
	private CanHearNoise(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var listener = ParameterFunctions[0].Result as ICharacter ??
		               ParameterFunctions[0].Result?.GetObject as ICharacter;
		var source = ParameterFunctions[1].Result as IPerceiver ??
		             ParameterFunctions[1].Result?.GetObject as IPerceiver;
		var rawVolume = Convert.ToDecimal(ParameterFunctions[2].Result?.GetObject ?? -1M);
		var rawProximity = Convert.ToDecimal(ParameterFunctions[3].Result?.GetObject ?? -1M);
		if (listener is null || source is null ||
			rawVolume != decimal.Truncate(rawVolume) ||
			rawVolume <= (int)AudioVolume.Silent ||
			rawVolume > (int)AudioVolume.DangerouslyLoud ||
			rawProximity != decimal.Truncate(rawProximity) ||
			rawProximity < (int)Proximity.Intimate ||
			rawProximity > (int)Proximity.Unapproximable)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		Result = new BooleanVariable(AudioPerception.CanHear(
			listener,
			source,
			(AudioVolume)(int)rawVolume,
			(Proximity)(int)rawProximity));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"canhearnoise",
			[ProgVariableTypes.Character, ProgVariableTypes.Perceivable, ProgVariableTypes.Number,
				ProgVariableTypes.Number],
			(parameters, _) => new CanHearNoise(parameters),
			["listener", "source", "volume", "proximity"],
			[
				"The character deciding whether the sound was heard.",
				"The source of the sound.",
				"The received AudioVolume value from the CharacterNoiseReceived event.",
				"The native Proximity numeric value from the CharacterNoiseReceived event."
			],
			"Uses the engine's native audibility, local Hearing Profile and GenericListenCheck rule to decide whether a received sound is noticed.",
			"Rooms",
			ProgVariableTypes.Boolean));
	}
}
