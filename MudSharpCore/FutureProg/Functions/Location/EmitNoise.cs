#nullable enable

using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Location;

internal sealed class EmitNoise : BuiltInFunction
{
	private EmitNoise(IList<IFunction> parameterFunctions) : base(parameterFunctions)
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

		var source = ParameterFunctions[0].Result as IPerceiver ??
		             ParameterFunctions[0].Result?.GetObject as IPerceiver;
		var rawVolume = Convert.ToDecimal(ParameterFunctions[1].Result?.GetObject ?? -1M);
		var noiseType = ParameterFunctions[2].Result?.GetObject?.ToString()?.Trim() ?? string.Empty;
		var audioText = ParameterFunctions[3].Result?.GetObject?.ToString() ?? string.Empty;
		if (source?.Location is null ||
			rawVolume != decimal.Truncate(rawVolume) ||
			rawVolume <= (int)AudioVolume.Silent ||
			rawVolume > (int)AudioVolume.DangerouslyLoud ||
			string.IsNullOrWhiteSpace(noiseType) ||
			string.IsNullOrWhiteSpace(audioText))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var volume = (AudioVolume)(int)rawVolume;

		try
		{
			_ = string.Format(audioText, "nearby", volume.Describe());
		}
		catch (FormatException)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		source.Location.HandleAudioEcho(
			audioText,
			volume,
			source,
			source.RoomLayer,
			true,
			noiseType);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"emitnoise",
			[ProgVariableTypes.Perceivable, ProgVariableTypes.Number, ProgVariableTypes.Text,
				ProgVariableTypes.Text],
			(parameters, _) => new EmitNoise(parameters),
			["source", "volume", "type", "echo"],
			[
				"The character or item producing the noise.",
				"The AudioVolume value from 1 (Faint) to 7 (Dangerously Loud).",
				"A stable category such as impact, alarm, gunshot, or machinery.",
				"The distant-audio template. Use {0} for direction and optionally {1} for volume."
			],
			"Emits a propagated sound and one NoiseEmitted event at its origin. Hooks can use that event for game-specific reactions, such as making zombies investigate sounds, without hard-coding those reactions in the engine.",
			"Rooms",
			ProgVariableTypes.Boolean));
	}
}
