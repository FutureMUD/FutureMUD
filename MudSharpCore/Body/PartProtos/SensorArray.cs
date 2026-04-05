#nullable enable

using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp.Body.PartProtos;

public class SensorArray : InternalOrganProto
{
    protected SensorArray(InternalOrganProto rhs, string newName)
        : base(rhs, newName)
    {
    }

    public SensorArray(BodypartProto proto, IFuturemud game)
        : base(proto, game)
    {
    }

    public override BodypartTypeEnum BodypartType => BodypartTypeEnum.SensorArray;

    public override IBodypart Clone(string newName)
    {
        return new SensorArray(this, newName);
    }

    public override void HandleChangedOrganFunction(IBody body, double oldFunctionFactor, double newFunctionFactor)
    {
        if (newFunctionFactor == oldFunctionFactor)
        {
            return;
        }

        if (newFunctionFactor < oldFunctionFactor)
        {
            if (newFunctionFactor <= 0.0 && oldFunctionFactor > 0.0)
            {
                body.OutputHandler.Send(
                    "Your sensor array has failed completely and your passive sensor feeds cut out."
                        .Colour(Telnet.BoldMagenta));
                return;
            }

            if (newFunctionFactor <= 0.35 && oldFunctionFactor > 0.35)
            {
                body.OutputHandler.Send(
                    "Your sensor array is critically degraded and your environmental telemetry is unreliable."
                        .Colour(Telnet.BoldMagenta));
                return;
            }

            if (newFunctionFactor <= 0.7 && oldFunctionFactor > 0.7)
            {
                body.OutputHandler.Send(
                    "Damage to your sensor array is causing intermittent distortion in audio and visual feeds."
                        .Colour(Telnet.BoldMagenta));
            }

            return;
        }

        if (newFunctionFactor > 0.0 && oldFunctionFactor <= 0.0)
        {
            body.OutputHandler.Send("Your sensor array has come back online.".Colour(Telnet.BoldGreen));
        }

        if (newFunctionFactor >= 0.35 && oldFunctionFactor < 0.35)
        {
            body.OutputHandler.Send("Your sensor array is no longer critically degraded.".Colour(Telnet.BoldGreen));
        }

        if (newFunctionFactor >= 0.7 && oldFunctionFactor < 0.7)
        {
            body.OutputHandler.Send("Your sensor feeds are once again stable.".Colour(Telnet.BoldGreen));
        }
    }
}
