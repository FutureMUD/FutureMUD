#nullable enable

using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework.Units;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Characters;

internal class ExposeCharacterToLiquidFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private ExposeCharacterToLiquidFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
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

		if (ParameterFunctions[0].Result is not ICharacter character)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var liquidId = (long)((decimal?)ParameterFunctions[1].Result?.GetObject ?? 0.0M);
		var volume = GetVolume();
		var liquid = _gameworld.Liquids.Get(liquidId);
		var bodypartText = ParameterFunctions[3].Result?.GetObject?.ToString() ?? string.Empty;
		var bodypart = character.Body.GetTargetBodypart(bodypartText);
		if (liquid is null || volume <= 0.0 || bodypart is not IExternalBodypart)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		character.Body.ExposeToLiquid(
			new LiquidMixture(liquid, volume, _gameworld),
			bodypart,
			LiquidExposureDirection.Irrelevant);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	private double GetVolume()
	{
		if (ParameterFunctions[2].ReturnType.CompatibleWith(ProgVariableTypes.Number))
		{
			return (double)((decimal?)ParameterFunctions[2].Result?.GetObject ?? 0.0M);
		}

		var text = ParameterFunctions[2].Result?.GetObject?.ToString() ?? string.Empty;
		var volume = _gameworld.UnitManager.GetBaseUnits(text, UnitType.FluidVolume, out var success);
		return success ? volume : 0.0;
	}

	public static void RegisterFunctionCompiler()
	{
		RegisterOverload(
			ProgVariableTypes.Number,
			"The volume of liquid to apply in base fluid units.",
			"Exposes a named external bodypart on a character to a base-unit volume of liquid");
		RegisterOverload(
			ProgVariableTypes.Text,
			"The volume of liquid to apply, including its fluid-volume unit.",
			"Exposes a named external bodypart on a character to a text-specified volume of liquid");
	}

	private static void RegisterOverload(
		ProgVariableTypes volumeType,
		string volumeHelp,
		string functionHelp)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"exposecharactertoliquid",
			[ProgVariableTypes.Character, ProgVariableTypes.Number, volumeType, ProgVariableTypes.Text],
			(parameters, gameworld) => new ExposeCharacterToLiquidFunction(parameters, gameworld),
			["character", "liquidId", "volume", "bodypart"],
			[
				"The character whose bodypart should be exposed to the liquid.",
				"The ID of the liquid to apply.",
				volumeHelp,
				"The keyword or name of the external bodypart to expose."
			],
			$"{functionHelp}, using the normal worn-item, held-item, cleaning, contamination, drying and residue rules. Returns false for a null character, unknown liquid, invalid bodypart, invalid unit expression or non-positive volume.",
			"Characters",
			ProgVariableTypes.Boolean));
	}
}
