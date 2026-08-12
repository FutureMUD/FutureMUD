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
		var driedPercentage = GetDriedPercentage();
		var liquid = _gameworld.Liquids.Get(liquidId);
		var bodypartText = ParameterFunctions[3].Result?.GetObject?.ToString() ?? string.Empty;
		var bodypart = character.Body.GetTargetBodypart(bodypartText);
		if (liquid is null || volume <= 0.0 || bodypart is not IExternalBodypart externalBodypart ||
		    driedPercentage is < 0.0 or > 100.0 ||
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
			character.Body.ExposeToLiquid(
				new LiquidMixture(liquid, freshVolume, _gameworld),
				externalBodypart,
				LiquidExposureDirection.Irrelevant);
		}

		if (driedVolume > 0.0)
		{
			var driedLiquid = new LiquidMixture(liquid, driedVolume, _gameworld);
			var wornItem = character.Body.WornItemsFor(externalBodypart).LastOrDefault();
			if (wornItem is not null)
			{
				wornItem.SurfaceLiquidState.TryAddDriedLiquid(driedLiquid);
				LiquidExposureStrategies.SurfaceReactions.Dry(wornItem, driedLiquid);
			}
			else
			{
				character.Body.SurfaceLiquidState.TryAddDriedLiquid(driedLiquid);
				LiquidExposureStrategies.SurfaceReactions.Dry(character.Body, driedLiquid, [externalBodypart]);
			}
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
		return ParameterFunctions.Count > 4
			? (double)((decimal?)ParameterFunctions[4].Result?.GetObject ?? -1.0M)
			: 0.0;
	}

	public static void RegisterFunctionCompiler()
	{
		RegisterOverload(
			ProgVariableTypes.Number,
			"The volume of liquid to apply in base fluid units.",
			false);
		RegisterOverload(
			ProgVariableTypes.Text,
			"The volume of liquid to apply, including its fluid-volume unit.",
			false);
		RegisterOverload(
			ProgVariableTypes.Number,
			"The volume of liquid to apply in base fluid units.",
			true);
		RegisterOverload(
			ProgVariableTypes.Text,
			"The volume of liquid to apply, including its fluid-volume unit.",
			true);
	}

	private static void RegisterOverload(
		ProgVariableTypes volumeType,
		string volumeHelp,
		bool includeDriedPercentage)
	{
		var parameterTypes = includeDriedPercentage
			? new[]
			{
				ProgVariableTypes.Character, ProgVariableTypes.Number, volumeType, ProgVariableTypes.Text,
				ProgVariableTypes.Number
			}
			: new[] { ProgVariableTypes.Character, ProgVariableTypes.Number, volumeType, ProgVariableTypes.Text };
		var parameterNames = includeDriedPercentage
			? new[] { "character", "liquidId", "volume", "bodypart", "driedPercentage" }
			: new[] { "character", "liquidId", "volume", "bodypart" };
		var parameterHelp = new List<string>
		{
			"The character whose bodypart should be exposed to the liquid.",
			"The ID of the liquid to apply.",
			volumeHelp,
			"The keyword or name of the external bodypart to expose."
		};
		if (includeDriedPercentage)
		{
			parameterHelp.Add("The percentage of the supplied volume to add as dried residue, from 0 to 100.");
		}

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"exposecharactertoliquid",
			parameterTypes,
			(parameters, gameworld) => new ExposeCharacterToLiquidFunction(parameters, gameworld),
			parameterNames,
			parameterHelp,
			"Exposes a named external bodypart on a character to liquid. An optional dried percentage splits the supplied volume between normal exposure and residue placed on the outermost covering garment, or on the body when uncovered. Returns false for invalid inputs or a dry request whose liquid has no configured residue.",
			"Characters",
			ProgVariableTypes.Boolean));
	}
}
