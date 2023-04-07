using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Health.Breathing;

public class GillBreather : IBreathingStrategy
{
	public string Name => "gills";

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

		var damageeffects =
			body.CombinedEffectsOfType<IBodypartIneffectiveEffect>().Where(x => x.Bodypart is IOrganProto).ToList();

		// TODO - effects
		var heart = body.OrganFunction<HeartProto>();
		if (heart <= 0.0)
		{
			return false;
		}

		var workingGills = body.Bodyparts.OfType<GillProto>().Where(x =>
			!body.CombinedEffectsOfType<IBodypartIneffectiveEffect>().Any(y => y.Bodypart == x)).ToList();
		if (workingGills.Count < 1)
		{
			return false;
		}

		var anasthesia = body.EffectsOfType<Anesthesia>().Select(x => x.IntensityPerGramMass).Sum();
		return !(anasthesia >= 5.0);
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
}