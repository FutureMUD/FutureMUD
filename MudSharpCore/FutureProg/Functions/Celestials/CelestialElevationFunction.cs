using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Celestial;
using MudSharp.Construction;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Celestials;

#nullable enable

internal class CelestialElevationFunction : BuiltInFunction
{
	public CelestialElevationFunction(IList<IFunction> parameters) : base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Number;
	public override string ErrorMessage => ParameterFunctions.First().ErrorMessage;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var obj = ParameterFunctions[0].Result?.GetObject;
		var zone = obj as IZone;
		if (zone is null && obj is ICell cell)
		{
			zone = cell.Zone;
		}

		if (zone is null)
		{
			Result = new NumberVariable(0.0);
			return StatementResult.Normal;
		}

		var id = Convert.ToInt64(ParameterFunctions[1].Result?.GetObject ?? 0L);
		var celestial = zone.Celestials.FirstOrDefault(x => x.Id == id);
		if (celestial is null)
		{
			Result = new NumberVariable(0.0);
			return StatementResult.Normal;
		}

		CelestialInformation info = zone.GetInfo(celestial);
		Result = new NumberVariable(info.LastAscensionAngle);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"celestialelevation",
			new[] { ProgVariableTypes.Location, ProgVariableTypes.Number },
			(pars, gameworld) => new CelestialElevationFunction(pars),
			new List<string> { "locationOrZone", "celestialId" },
			new List<string> { "The room or zone whose celestial collection is searched.", "The ID of the celestial object to inspect." },
			"Looks up a celestial object by ID in the supplied room or zone and returns its current elevation above or below the horizon in radians. Positive values are above the horizon. Returns 0 if the zone or celestial object cannot be found.",
			"Celestials",
			ProgVariableTypes.Number
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"celestialelevation",
			new[] { ProgVariableTypes.Zone, ProgVariableTypes.Number },
			(pars, gameworld) => new CelestialElevationFunction(pars),
			new List<string> { "locationOrZone", "celestialId" },
			new List<string> { "The room or zone whose celestial collection is searched.", "The ID of the celestial object to inspect." },
			"Looks up a celestial object by ID in the supplied room or zone and returns its current elevation above or below the horizon in radians. Positive values are above the horizon. Returns 0 if the zone or celestial object cannot be found.",
			"Celestials",
			ProgVariableTypes.Number
		));
	}
}
