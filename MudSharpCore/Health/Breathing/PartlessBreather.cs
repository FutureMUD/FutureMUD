using MudSharp.Construction;
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
        if (BreathingStrategyHelper.IsStoppedByEffect(body))
        {
            return false;
        }

        if (!BreathingStrategyHelper.CanBreatheFluid(body, BreathingFluid(body)))
        {
            return false;
        }

        return true;
    }

    public void Breathe(IBody body)
    {
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

        BreathingStrategyHelper.ExposeToMagic(body, BreathingFluid(body));
        if (body.HeldBreathTime > TimeSpan.Zero)
        {
            body.HeldBreathTime -= TimeSpan.FromSeconds(10);
        }
    }

    public IFluid BreathingFluid(IBody body)
    {
        return TerrainImmersionHelper.RespiratoryFluid(body, null);
    }

    #endregion
}
