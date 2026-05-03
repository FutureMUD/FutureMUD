using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Form.Audio;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Strategies.BodyStratagies;
using System;
using System.Linq;

namespace MudSharp.Body.CommunicationStrategies;

public class RobotCommunicationStrategy : HumanoidCommunicationStrategy, IBodyCommunicationStrategy
{
    protected RobotCommunicationStrategy()
    {
    }

    string IBodyCommunicationStrategy.Name => "Robot";

    public new static IBodyCommunicationStrategy Instance { get; } = new RobotCommunicationStrategy();

    public override string WhyCannotVocalise(IBody body)
    {
        if (body.OrganFunction<SpeechSynthesizer>() <= 0.0)
        {
            return $"You cannot speak because you do not have a functioning speech synthesizer.";
        }

        if (body.Actor.Merits.OfType<IMuteMerit>().Any(x => x.Applies(body.Actor)))
        {
            return $"You are mute and cannot speak.";
        }

        if (IsSilenced(body))
        {
            return SilenceReason;
        }

        throw new ApplicationException();
    }

    public override bool CanVocalise(IBody body)
    {
        if (body.OrganFunction<SpeechSynthesizer>() <= 0.0)
        {
            return false;
        }

        if (body.Actor.Merits.OfType<IMuteMerit>().Any(x => x.Applies(body.Actor)))
        {
            return false;
        }

        return !IsSilenced(body);
    }

    public override bool CanVocalise(IBody body, AudioVolume volume)
    {
        double synthFunction = body.OrganFunction<SpeechSynthesizer>();
        if (!HasSpeechSynthesizerFunctionForVolume(synthFunction, volume))
        {
            return false;
        }

        return CanVocalise(body);
    }

    public override string WhyCannotVocalise(IBody body, AudioVolume volume)
    {
        double synthFunction = body.OrganFunction<SpeechSynthesizer>();
        if (!HasSpeechSynthesizerFunctionForVolume(synthFunction, volume))
        {
            return "You are lacking speech synthesizer function for a vocalisation of that volume.";
        }

        return WhyCannotVocalise(body);
    }

    public override PermitLanguageOptions VocalisationOption(IBody body, AudioVolume volume)
    {
        if (body.Actor.Merits.OfType<IMuteMerit>().Any(x => x.Applies(body.Actor)))
        {
            return body.Actor.Merits.OfType<IMuteMerit>().First(x => x.Applies(body.Actor)).LanguageOptions;
        }

        if (IsBabbled(body))
        {
            return PermitLanguageOptions.LanguageIsBabbling;
        }

        if (IsSilenced(body))
        {
            return PermitLanguageOptions.LanguageIsBuzzing;
        }

        if (!CanVocalise(body, volume))
        {
            return PermitLanguageOptions.LanguageIsBuzzing;
        }

        return PermitLanguageOptions.PermitLanguage;
    }

    internal static bool HasSpeechSynthesizerFunctionForVolume(double synthFunction, AudioVolume volume)
    {
        return synthFunction >= RequiredSpeechSynthesizerFunction(volume);
    }

    internal static double RequiredSpeechSynthesizerFunction(AudioVolume volume)
    {
        return volume switch
        {
            AudioVolume.Silent => 0.1,
            AudioVolume.Faint => 0.2,
            AudioVolume.Quiet => 0.3,
            AudioVolume.Decent => 0.5,
            AudioVolume.Loud => 0.65,
            AudioVolume.VeryLoud => 0.8,
            AudioVolume.ExtremelyLoud => 0.9,
            AudioVolume.DangerouslyLoud => 1.0,
            _ => 0.0
        };
    }
}
