using System.Linq;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Form.Audio;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Strategies.BodyStratagies;

namespace MudSharp.Body.CommunicationStrategies;

public class MouthOnlyCommunicationStrategy : HumanoidCommunicationStrategy, IBodyCommunicationStrategy
{
    protected MouthOnlyCommunicationStrategy()
    {
    }

    string IBodyCommunicationStrategy.Name => "Mouth Only";

    public new static IBodyCommunicationStrategy Instance { get; } = new MouthOnlyCommunicationStrategy();

    #region Overrides of HumanoidCommunicationStrategy

    public override bool CanVocalise(IBody body)
    {
        if (!body.Bodyparts.Any(x => x is MouthProto) &&
            body.Prosthetics.All(x => !(x.TargetBodypart is MouthProto) || !x.Functional))
        {
            return false;
        }

        if (body.NeedsToBreathe && !body.CanBreathe)
        {
            return false;
        }

        return !body.Actor.Merits.OfType<IMuteMerit>().Any(x => x.Applies(body.Actor));
    }

    public override PermitLanguageOptions VocalisationOption(IBody body, AudioVolume volume)
    {
        if (!body.Bodyparts.Any(x => x is MouthProto) &&
            body.Prosthetics.All(x => !(x.TargetBodypart is MouthProto) || !x.Functional))
        {
            return PermitLanguageOptions.LanguageIsError;
        }

        if (body.NeedsToBreathe && !body.CanBreathe)
        {
            return PermitLanguageOptions.LanguageIsChoking;
        }

        if (body.Actor.Merits.OfType<IMuteMerit>().Any(x => x.Applies(body.Actor)))
        {
            return PermitLanguageOptions.LanguageIsMuffling;
        }

        return PermitLanguageOptions.PermitLanguage;
    }

    #endregion
}