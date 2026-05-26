using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Form.Material;
using MudSharp.Framework;
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
					.ApplyToCharacter(body.Actor, bodyparts ?? body.Bodyparts.OfType<IExternalBodypart>(), mixture)
					.ProcessPassiveWounds();
				return;
		}
	}

	public void Dry(IPerceivable owner, LiquidMixture driedLiquid, IEnumerable<IExternalBodypart>? bodyparts = null)
	{
		Expose(owner, driedLiquid, LiquidExposureDirection.Irrelevant, bodyparts);
	}
}

public static class LiquidExposureStrategies
{
	public static ILiquidExposureStrategy SurfaceReactions { get; } = new LiquidSurfaceReactionExposureStrategy();
}
