using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;

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
        if (body.EffectsOfType<IStopBreathing>().Any(x => x.Applies()))
        {
            return false;
        }

        IFluid breathingFluid = BreathingFluid(body);
        bool additionalBreathing = body.CombinedEffectsOfType<IAdditionalBreathableFluidEffect>()
            .Any(x => x.Applies() && x.AppliesToFluid(breathingFluid));
        if (!BreathingStrategyHelper.CanBreatheFluid(body, breathingFluid) && !additionalBreathing)
        {
            return false;
        }

        // TODO - effects
        double heart = body.OrganFunction<HeartProto>();
        if (heart <= 0.0)
        {
            return false;
        }

        double airwayTolerance = body.RespirationAirwayToleranceMultiplier();
        double lungFunction = body.OrganFunction<LungProto>();
        if (lungFunction < 0.5 / airwayTolerance)
        {
            return false;
        }

        double airwayBleeding = body.EffectsOfType<IInternalBleedingEffect>()
                                 .Where(x => x.Organ is LungProto || x.Organ is TracheaProto)
                                 .Select(x => x.BloodlossTotal)
                                 .DefaultIfEmpty(0)
                                 .Sum();
        if (airwayBleeding > 0.3 * airwayTolerance)
        {
            return false;
        }

        double trachea = body.OrganFunction<TracheaProto>();
        if (trachea <= 0.0)
        {
            return false;
        }

        double anasthesia = body.EffectsOfType<Anesthesia>().Select(x => x.IntensityPerGramMass).Sum();
        return !(anasthesia >= 5.0 * body.RespirationBreathingDriveMultiplier());
    }

    public void Breathe(IBody body)
    {
		// Resolve and withdraw a real supply before testing oxygen compatibility. An unbreathable
		// supplied gas can still enter a functioning airway; a failed withdrawal cannot.
		var suppliedMouth = body.Bodyparts.OfType<MouthProto>().FirstOrDefault();
		var suppliedSource = suppliedMouth is null ? null : GetBreathingGasSource(body, suppliedMouth);
		if (suppliedSource is not null && BreathingStrategyHelper.HasAirflow(body) &&
			(CanBreathe(body) || body.HeldBreathTime <= TimeSpan.Zero))
		{
			var suppliedGas = suppliedSource.Gas;
			if (suppliedSource.CanConsumeGas(body.Race.BreathingRate(body, suppliedGas)) &&
				suppliedSource.ConsumeGas(body.Race.BreathingRate(body, suppliedGas)))
			{
				BreathingStrategyHelper.ExposeToMagic(body, suppliedGas, true);
				body.HeldBreathTime = CanBreathe(body) ? TimeSpan.FromSeconds(Math.Max(0, body.HeldBreathTime.TotalSeconds - 10)) : body.HeldBreathTime + TimeSpan.FromSeconds(10);
			}
			else
			{
				EnvironmentalExposureService.For(body.Gameworld).NoInhalation(body);
				body.HeldBreathTime += TimeSpan.FromSeconds(10);
			}
			return;
		}
        if (!CanBreathe(body))
        {
			BreathingStrategyHelper.UnbreathableSample(body, BreathingFluid(body));
            if (body.HeldBreathTime <= TimeSpan.Zero)
            {
                body.OutputHandler.Send("You can't breathe, and have begun to hold your breath.");
            }

            body.HeldBreathTime += TimeSpan.FromSeconds(10);
            return;
        }

        MouthProto mouth = body.Bodyparts.OfType<MouthProto>().FirstOrDefault();
        if (mouth == null)
        {
			EnvironmentalExposureService.For(body.Gameworld).NoInhalation(body);
            return;
        }

        IProvideGasForBreathing gasSource = GetBreathingGasSource(body, mouth);
        if (gasSource != null)
        {
			EnvironmentalExposureService.For(body.Gameworld).NoInhalation(body);
            return;
        }

        BreathingStrategyHelper.ExposeToMagic(body, BreathingFluid(body));
        if (body.HeldBreathTime > TimeSpan.Zero)
        {
            body.HeldBreathTime -= TimeSpan.FromSeconds(10);
        }
    }

    public IFluid BreathingFluid(IBody body)
    {
        MouthProto mouth = body.Bodyparts.OfType<MouthProto>().FirstOrDefault();
        if (mouth == null)
        {
            return null;
        }

        IFluid underwaterFluid = body.Location.IsUnderwaterLayer(body.RoomLayer)
            ? body.Location?.Terrain(body.Actor).WaterFluid
            : null;

        IProvideGasForBreathing gasSource = GetBreathingGasSource(body, mouth, underwaterFluid);
        if (gasSource != null && (underwaterFluid == null || gasSource.WaterTight))
        {
            return gasSource.Gas;
        }

        return underwaterFluid ?? body.Location?.Atmosphere;
    }

    private static IProvideGasForBreathing GetBreathingGasSource(IBody body, MouthProto mouth, IFluid underwaterFluid = null)
    {
        underwaterFluid ??= body.Location.IsUnderwaterLayer(body.RoomLayer)
            ? body.Location?.Terrain(body.Actor).WaterFluid
            : null;

        return body.WornItemsFor(mouth)
                   .SelectNotNull(x => x.GetItemType<IProvideGasForBreathing>())
                   .FirstOrDefault(x => x.Gas != null && (underwaterFluid == null || x.WaterTight));
    }
}
