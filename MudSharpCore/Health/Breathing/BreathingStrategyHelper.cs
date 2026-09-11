#nullable enable

using MudSharp.Body;
using MudSharp.Form.Material;

namespace MudSharp.Health.Breathing;

internal static class BreathingStrategyHelper
{
	public static void ExposeToMagic(IBody body, IFluid? fluid)
	{
		if (fluid is IGas gas) MudSharp.Magic.MagicalExposure.Carrier(body, MudSharp.Magic.SubstanceCarrier.Gas,
			gas.Id, body.Race.BreathingRate(body, gas), DrugVector.Inhaled);
	}

	public static bool CanBreatheFluid(IBody body, IFluid? fluid)
	{
		return fluid is not null && body.Race.CanBreatheFluid(fluid).Truth;
	}
}
