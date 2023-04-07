using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Climate.ClimateModels;
using MudSharp.Framework;

namespace MudSharp.Climate;

public static class ClimateModelFactory
{
	public static IClimateModel LoadClimateModel(Models.ClimateModel model, IFuturemud gameworld)
	{
		switch (model.Type)
		{
			case "terrestrial":
				return new TerrestrialClimateModel(model, gameworld);
		}

		throw new ApplicationException($"Unknown ClimateModel type {model.Type}");
	}
}