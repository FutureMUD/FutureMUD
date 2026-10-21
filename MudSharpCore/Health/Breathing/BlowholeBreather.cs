using MudSharp.Construction;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Effects.Concrete;
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
        if (BreathingStrategyHelper.IsStoppedByEffect(body))
        {
            return false;
        }

        if (!BreathingStrategyHelper.CanBreatheFluid(body, BreathingFluid(body)))
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

        List<BlowholeProto> workingBlowholes = body.Bodyparts.OfType<BlowholeProto>().Where(x =>
            !body.CombinedEffectsOfType<IBodypartIneffectiveEffect>().Any(y => y.Bodypart == x)).ToList();
        if (workingBlowholes.Count < 1)
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

        BreathingStrategyHelper.ExposeToMagic(body, BreathingFluid(body), elapsed);
        if (body.HeldBreathTime > TimeSpan.Zero)
        {
            body.HeldBreathTime -= elapsed;
        }
    }

    public IFluid BreathingFluid(IBody body)
    {
        return TerrainImmersionHelper.RespiratoryFluid(body, body.Bodyparts.OfType<BlowholeProto>().FirstOrDefault());
    }
}
