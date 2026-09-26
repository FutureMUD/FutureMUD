using MudSharp.Body;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.Health;

#nullable enable

namespace MudSharp.Form.Material;

public sealed class LiquidSurfaceReactionExposureStrategy : ILiquidExposureStrategy
{
	public void Expose(IPerceivable owner, LiquidMixture mixture, LiquidExposureDirection direction, IEnumerable<IExternalBodypart>? bodyparts = null)
	{
		if (mixture.IsEmpty)
		{
			return;
		}

		switch (owner)
		{
			case IGameItem item:
				LiquidSurfaceReactionHelper.ApplyToItem(item, mixture).ProcessPassiveWounds();
				return;
			case IBody body:
				LiquidSurfaceReactionHelper
					.ApplyToBody(body, bodyparts ?? body.Bodyparts.OfType<IExternalBodypart>(), mixture)
					.ProcessPassiveWounds();
				return;
		}
	}

	public void Dry(IPerceivable owner, LiquidMixture driedLiquid, IEnumerable<IExternalBodypart>? bodyparts = null)
	{
		// Drying and inspection never create a new contact dose.
	}
}

public static class LiquidExposureStrategies
{
	public static ILiquidExposureStrategy SurfaceReactions { get; } = new LiquidSurfaceReactionExposureStrategy();
}
