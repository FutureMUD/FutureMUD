#nullable enable

using MudSharp.Body;
using MudSharp.Form.Material;

namespace MudSharp.Health.Breathing;

internal static class BreathingStrategyHelper
{
	public static bool CanBreatheFluid(IBody body, IFluid? fluid)
	{
		return fluid is not null && body.Race.CanBreatheFluid(fluid).Truth;
	}
}
