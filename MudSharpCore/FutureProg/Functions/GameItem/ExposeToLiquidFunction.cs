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
		var driedPercentage = GetDriedPercentage();
		var liquid = _gameworld.Liquids.Get(liquidId);
		if (liquid is null || volume <= 0.0 || driedPercentage is < 0.0 or > 100.0 ||
		    driedPercentage > 0.0 &&
		    (liquid.DriedResidue is null || liquid.ResidueVolumePercentage <= 0.0 ||
		     !double.IsFinite(liquid.ResidueVolumePercentage)))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var driedVolume = volume * driedPercentage / 100.0;
		var freshVolume = volume - driedVolume;
		if (freshVolume > 0.0)
		{
			item.ExposeToLiquid(
				new LiquidMixture(liquid, freshVolume, _gameworld),
				null,
				LiquidExposureDirection.FromOnTop);
		}

		if (driedVolume > 0.0)
		{
			var driedLiquid = new LiquidMixture(liquid, driedVolume, _gameworld);
			item.SurfaceLiquidState.TryAddDriedLiquid(driedLiquid);
			LiquidExposureStrategies.SurfaceReactions.Dry(item, driedLiquid);
		}

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

	private double GetDriedPercentage()
	{
		return ParameterFunctions.Count > 3
			? (double)((decimal?)ParameterFunctions[3].Result?.GetObject ?? -1.0M)
			: 0.0;
	}

	public static void RegisterFunctionCompiler()
	{
		RegisterOverload(ProgVariableTypes.Number, "The volume of liquid to apply in base fluid units.", false);
		RegisterOverload(ProgVariableTypes.Text, "The volume of liquid to apply, including its fluid-volume unit.", false);
		RegisterOverload(ProgVariableTypes.Number, "The volume of liquid to apply in base fluid units.", true);
		RegisterOverload(ProgVariableTypes.Text, "The volume of liquid to apply, including its fluid-volume unit.", true);
	}

	private static void RegisterOverload(ProgVariableTypes volumeType, string volumeHelp, bool includeDriedPercentage)
	{
		var parameterTypes = includeDriedPercentage
			? new[] { ProgVariableTypes.Item, ProgVariableTypes.Number, volumeType, ProgVariableTypes.Number }
			: new[] { ProgVariableTypes.Item, ProgVariableTypes.Number, volumeType };
		var parameterNames = includeDriedPercentage
			? new[] { "item", "liquidId", "volume", "driedPercentage" }
			: new[] { "item", "liquidId", "volume" };
		var parameterHelp = includeDriedPercentage
			? new[]
			{
				"The item whose surface should be exposed to the liquid.",
				"The ID of the liquid to apply.",
				volumeHelp,
				"The percentage of the supplied volume to add as dried residue, from 0 to 100."
			}
			: new[]
			{
				"The item whose surface should be exposed to the liquid.",
				"The ID of the liquid to apply.",
				volumeHelp
			};

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"exposetoliquid",
			parameterTypes,
			(parameters, gameworld) => new ExposeToLiquidFunction(parameters, gameworld),
			parameterNames,
			parameterHelp,
			"Exposes an item's upper surface to liquid. An optional dried percentage splits the supplied volume between fresh liquid and configured dried residue; 0 preserves normal exposure and 100 adds residue only. Returns false for invalid inputs or a dry request whose liquid has no configured residue.",
			"Items",
			ProgVariableTypes.Boolean));
	}
}
