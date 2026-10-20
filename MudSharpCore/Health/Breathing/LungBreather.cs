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
        if (BreathingStrategyHelper.IsStoppedByEffect(body))
        {
            return false;
        }

        IFluid breathingFluid = BreathingFluid(body);
        if (!BreathingStrategyHelper.CanBreatheFluid(body, breathingFluid, includeLegacyMagic: true))
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

    public void Breathe(IBody body) => Breathe(body, TimeSpan.FromSeconds(10));

    public void Breathe(IBody body, TimeSpan elapsed)
    {
        if (elapsed <= TimeSpan.Zero)
        {
            return;
        }

		// Resolve and withdraw a real supply before testing oxygen compatibility. An unbreathable
		// supplied gas can still enter a functioning airway; a failed withdrawal cannot.
		var suppliedMouth = body.Bodyparts.OfType<MouthProto>().FirstOrDefault();
		var suppliedSource = suppliedMouth is null ? null : GetBreathingGasSource(body, suppliedMouth);
		if (suppliedSource is not null && BreathingStrategyHelper.HasAirflow(body) &&
			(CanBreathe(body) || body.HeldBreathTime <= TimeSpan.Zero))
		{
			var suppliedGas = suppliedSource.Gas;
			var volume = BreathingStrategyHelper.BreathingVolume(body, suppliedGas, elapsed);
			var canBreathe = CanBreathe(body);
			if (suppliedSource.CanConsumeGas(volume) && suppliedSource.ConsumeGas(volume))
			{
				BreathingStrategyHelper.ExposeToMagic(body, suppliedGas, elapsed, true);
				// Judge the delivered gas before depletion changes the device's Gas property.
				body.HeldBreathTime = canBreathe
					? TimeSpan.FromSeconds(Math.Max(0, body.HeldBreathTime.TotalSeconds - elapsed.TotalSeconds))
					: body.HeldBreathTime + elapsed;
			}
			else
			{
				EnvironmentalExposureService.For(body.Gameworld).NoInhalation(body);
				body.HeldBreathTime += elapsed;
			}
			return;
		}
        if (!CanBreathe(body))
        {
			BreathingStrategyHelper.UnbreathableSample(body, BreathingFluid(body), elapsed);
            if (body.HeldBreathTime <= TimeSpan.Zero)
            {
                body.OutputHandler.Send("You can't breathe, and have begun to hold your breath.");
            }

            body.HeldBreathTime += elapsed;
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

        BreathingStrategyHelper.ExposeToMagic(body, BreathingFluid(body), elapsed);
        if (body.HeldBreathTime > TimeSpan.Zero)
        {
            body.HeldBreathTime -= elapsed;
        }
    }

    public IFluid BreathingFluid(IBody body)
    {
        MouthProto mouth = body.Bodyparts.OfType<MouthProto>().FirstOrDefault();
        if (mouth == null)
        {
            return null;
        }

        IFluid underwaterFluid = TerrainImmersionHelper.IsPartImmersed(body, mouth)
            ? TerrainImmersionHelper.RespiratoryFluid(body, mouth)
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
        underwaterFluid ??= TerrainImmersionHelper.IsPartImmersed(body, mouth)
            ? TerrainImmersionHelper.RespiratoryFluid(body, mouth)
            : null;

        return body.WornItemsFor(mouth)
                   .SelectNotNull(x => x.GetItemType<IProvideGasForBreathing>())
                   .FirstOrDefault(x => x.Gas != null && (underwaterFluid == null || x.WaterTight));
    }
}
