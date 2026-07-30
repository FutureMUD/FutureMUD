#nullable enable

using MudSharp.Form.Material;
using MudSharp.Framework.Units;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class ExposeToLiquidFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private ExposeToLiquidFunction(IList<IFunction> parameters, IFuturemud gameworld)
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

		if (ParameterFunctions[0].Result is not IGameItem item)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var liquidId = (long)((decimal?)ParameterFunctions[1].Result?.GetObject ?? 0.0M);
		var volume = GetVolume();
		var liquid = _gameworld.Liquids.Get(liquidId);
		if (liquid is null || volume <= 0.0)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		item.ExposeToLiquid(
			new LiquidMixture(liquid, volume, _gameworld),
			null,
			LiquidExposureDirection.FromOnTop);
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
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"exposetoliquid",
			[ProgVariableTypes.Item, ProgVariableTypes.Number, ProgVariableTypes.Number],
			(parameters, gameworld) => new ExposeToLiquidFunction(parameters, gameworld),
			["item", "liquidId", "volume"],
			[
				"The item whose surface should be exposed to the liquid.",
				"The ID of the liquid to apply.",
				"The volume of liquid to apply in base fluid units."
			],
			"Exposes an item's upper surface to a volume of liquid, using the normal absorption, contamination, drying and residue rules. Returns false for a null item, unknown liquid or non-positive volume.",
			"Items",
			ProgVariableTypes.Boolean));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"exposetoliquid",
			[ProgVariableTypes.Item, ProgVariableTypes.Number, ProgVariableTypes.Text],
			(parameters, gameworld) => new ExposeToLiquidFunction(parameters, gameworld),
			["item", "liquidId", "volume"],
			[
				"The item whose surface should be exposed to the liquid.",
				"The ID of the liquid to apply.",
				"The volume of liquid to apply, including its fluid-volume unit."
			],
			"Exposes an item's upper surface to a text-specified volume of liquid, using the normal absorption, contamination, drying and residue rules. Returns false for a null item, unknown liquid, invalid unit expression or non-positive volume.",
			"Items",
			ProgVariableTypes.Boolean));
	}
}
