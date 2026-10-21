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
        return TerrainImmersionHelper.RespiratoryFluid(body, null);
    }

    #endregion
}
