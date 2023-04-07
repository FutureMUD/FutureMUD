using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Health.Breathing;

public class LungBreather : IBreathingStrategy
{
	public string Name => "simple";

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

		var mouth = body.Bodyparts.OfType<MouthProto>().FirstOrDefault();
		if (mouth == null)
		{
			return;
		}

		var rebreather = body.WornItemsFor(mouth).FirstOrDefault()?.GetItemType<IProvideGasForBreathing>();
		if (rebreather != null)
		{
			// TODO - things other than rebreathers that might have breath consumed
			if (rebreather.ConsumeGas(body.Race.BreathingRate(body, BreathingFluid(body))))
			{
				if (body.HeldBreathTime > TimeSpan.Zero)
				{
					body.HeldBreathTime -= TimeSpan.FromSeconds(10);
				}
			}

			return;
		}

		if (body.HeldBreathTime > TimeSpan.Zero)
		{
			body.HeldBreathTime -= TimeSpan.FromSeconds(10);
		}
	}

	public IFluid BreathingFluid(IBody body)
	{
		var mouth = body.Bodyparts.OfType<MouthProto>().FirstOrDefault();
		if (mouth == null)
		{
			return null;
		}

		var underwaterFluid = body.Location.IsUnderwaterLayer(body.RoomLayer)
			? body.Location?.Terrain(body.Actor).WaterFluid
			: null;

		var rebreather = body.WornItemsFor(mouth).SelectNotNull(x => x.GetItemType<IProvideGasForBreathing>())
		                     .FirstOrDefault();
		if (rebreather != null && (underwaterFluid == null || rebreather.WaterTight))
		{
			return rebreather.Gas;
		}

		return underwaterFluid ?? body.Location?.Atmosphere;
	}
}