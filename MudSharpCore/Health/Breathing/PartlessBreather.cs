using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
using MudSharp.Form.Material;

namespace MudSharp.Health.Breathing;

public class PartlessBreather : IBreathingStrategy
{
	#region Implementation of IBreathingStrategy

	public string Name => "partless";
	public bool NeedsToBreathe => true;

	public bool IsBreathing(IBody body)
	{
		return CanBreathe(body);
	}

	public bool CanBreathe(IBody body)
	{
		if (!body.Race.BreathableFluids.Contains(BreathingFluid(body)))
		{
			return false;
		}

		return true;
	}

	public void Breathe(IBody body)
	{
		if (!CanBreathe(body))
		{
			if (body.HeldBreathTime <= TimeSpan.Zero)
			{
				body.OutputHandler.Send("You can't breathe, and have begun to hold your breath.");
			}

			body.HeldBreathTime += TimeSpan.FromSeconds(10);
			return;
		}

		if (body.HeldBreathTime > TimeSpan.Zero)
		{
			body.HeldBreathTime -= TimeSpan.FromSeconds(10);
		}
	}

	public IFluid BreathingFluid(IBody body)
	{
		var underwaterFluid = body.Location.IsUnderwaterLayer(body.RoomLayer)
			? body.Location?.Terrain(body.Actor).WaterFluid
			: null;

		return underwaterFluid ?? body.Location?.Atmosphere;
	}

	#endregion
}