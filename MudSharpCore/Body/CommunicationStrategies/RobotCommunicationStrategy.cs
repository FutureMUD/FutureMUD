using System;
using System.Linq;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Form.Audio;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Strategies.BodyStratagies;

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

        return true;
    }

    public override bool CanVocalise(IBody body, AudioVolume volume)
    {
        var synthFunction = body.OrganFunction<SpeechSynthesizer>();
        switch (volume)
        {
            case AudioVolume.Silent:
                if (synthFunction < 0.1)
                {
                    return false;
                }

                break;
            case AudioVolume.Faint:
                if (synthFunction < 0.2)
                {
                    return false;
                }

                break;
            case AudioVolume.Quiet:
                if (synthFunction < 0.3)
                {
                    return false;
                }

                break;
            case AudioVolume.Decent:
                if (synthFunction < 0.5)
                {
                    return false;
                }

                break;
            case AudioVolume.Loud:
                if (synthFunction < 0.65)
                {
                    return false;
                }

                break;
            case AudioVolume.VeryLoud:
                if (synthFunction < 0.8)
                {
                    return false;
                }

                break;
            case AudioVolume.ExtremelyLoud:
                if (synthFunction < 0.9)
                {
                    return false;
                }

                break;
            case AudioVolume.DangerouslyLoud:
                if (synthFunction < 1.0)
                {
                    return false;
                }

                break;
        }

        return CanVocalise(body);
    }

    public override string WhyCannotVocalise(IBody body, AudioVolume volume)
    {
        var synthFunction = body.OrganFunction<SpeechSynthesizer>();
        switch (volume)
        {
            case AudioVolume.Silent:
                if (synthFunction < 0.1)
                {
                    return "You are lacking speech synthesizer function for a vocalisation of that volume.";
                }

                break;
            case AudioVolume.Faint:
                if (synthFunction < 0.2)
                {
                    return "You are lacking speech synthesizer function for a vocalisation of that volume.";
                }

                break;
            case AudioVolume.Quiet:
                if (synthFunction < 0.3)
                {
                    return "You are lacking speech synthesizer function for a vocalisation of that volume.";
                }

                break;
            case AudioVolume.Decent:
                if (synthFunction < 0.5)
                {
                    return "You are lacking speech synthesizer function for a vocalisation of that volume.";
                }

                break;
            case AudioVolume.Loud:
                if (synthFunction < 0.65)
                {
                    return "You are lacking speech synthesizer function for a vocalisation of that volume.";
                }

                break;
            case AudioVolume.VeryLoud:
                if (synthFunction < 0.8)
                {
                    return "You are lacking speech synthesizer function for a vocalisation of that volume.";
                }

                break;
            case AudioVolume.ExtremelyLoud:
                if (synthFunction < 0.9)
                {
                    return "You are lacking speech synthesizer function for a vocalisation of that volume.";
                }

                break;
            case AudioVolume.DangerouslyLoud:
                if (synthFunction < 1.0)
                {
                    return "You are lacking speech synthesizer function for a vocalisation of that volume.";
                }

                break;
        }

        return WhyCannotVocalise(body);
    }

    public override PermitLanguageOptions VocalisationOption(IBody body, AudioVolume volume)
    {
        if (body.Actor.Merits.OfType<IMuteMerit>().Any(x => x.Applies(body.Actor)))
        {
            return body.Actor.Merits.OfType<IMuteMerit>().First(x => x.Applies(body.Actor)).LanguageOptions;
        }

        if (!CanVocalise(body, volume))
        {
            return PermitLanguageOptions.LanguageIsBuzzing;
        }

        return PermitLanguageOptions.PermitLanguage;
    }
}