using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;

namespace MudSharp.Health.Breathing;

public class BlowholeBreather : IBreathingStrategy
{
	public string Name => "blowhole";

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

		// TODO - effects
		var heart = body.OrganFunction<HeartProto>();
		if (heart <= 0.0)
		{
			return false;
		}

		var lungFunction = body.OrganFunction<LungProto>();
		if (lungFunction < 0.5)
		{
			return false;
		}

		var airwayBleeding = body.EffectsOfType<IInternalBleedingEffect>()
		                         .Where(x => x.Organ is LungProto || x.Organ is TracheaProto)
		                         .Select(x => x.BloodlossTotal)
		                         .DefaultIfEmpty(0)
		                         .Sum();
		if (airwayBleeding > 0.3)
		{
			return false;
		}

		var trachea = body.OrganFunction<TracheaProto>();
		if (trachea <= 0.0)
		{
			return false;
		}

		var workingBlowholes = body.Bodyparts.OfType<BlowholeProto>().Where(x =>
			!body.CombinedEffectsOfType<IBodypartIneffectiveEffect>().Any(y => y.Bodypart == x)).ToList();
		if (workingBlowholes.Count < 1)
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