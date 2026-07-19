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
        if (!body.Race.BreathableFluids.Contains(breathingFluid) && !additionalBreathing)
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
        if (!CanBreathe(body))
        {
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
            return;
        }

        IProvideGasForBreathing gasSource = GetBreathingGasSource(body, mouth);
        if (gasSource != null)
        {
            if (gasSource.ConsumeGas(body.Race.BreathingRate(body, gasSource.Gas)))
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
