using MudSharp.Body;
using MudSharp.Communication.Language;
using MudSharp.Form.Audio;
using MudSharp.GameItems;
using MudSharp.Strategies.BodyStratagies;

#nullable enable annotations

namespace MudSharp.Body.CommunicationStrategies;

public class NonCommunicatorCommunicationStrategy : IBodyCommunicationStrategy
{
    private NonCommunicatorCommunicationStrategy()
    {
    }

    string IBodyCommunicationStrategy.Name => "Non-Communicator";

    public static IBodyCommunicationStrategy Instance { get; } = new NonCommunicatorCommunicationStrategy();

    public void Emote(IBody body, string emote, bool permitSpeech = true,
        OutputFlags additionalConditions = OutputFlags.Normal)
    {
        PlayerEmote emoteData = new(emote, body.Actor, true, PermitLanguageOptions.IgnoreLanguage);
        if (emoteData.Valid)
        {
            body.OutputHandler.Handle(new EmoteOutput(emoteData, flags: additionalConditions));
			foreach (var token in emoteData.SignedLanguageTokens)
			{
				SignedCommunicationService.HandleEvents(body, null, token.SignText, token.Language, token.Variety,
					token.Outcome);
			}
        }
        else
        {
            body.OutputHandler.Send(emoteData.ErrorMessage);
        }
    }

    public bool CanVocalise(IBody body)
    {
        return false;
    }

    public bool CanVocalise(IBody body, AudioVolume volume)
    {
        return false;
    }

    public string WhyCannotVocalise(IBody body)
    {
        return "You are unable to speak.";
    }

    public string WhyCannotVocalise(IBody body, AudioVolume volume)
    {
        return WhyCannotVocalise(body);
    }

    public PermitLanguageOptions VocalisationOption(IBody body, AudioVolume volume)
    {
        return PermitLanguageOptions.IgnoreLanguage;
    }

    public void Say(IBody body, IPerceivable target, string message, IEmote? emote = null)
    {
        body.OutputHandler.Send("You are unable to speak.");
    }

	public void Sign(IBody body, IPerceivable target, string message, IEmote? emote = null) =>
		SignedCommunicationService.Sign(body, target, message, emote);

    public void LoudSay(IBody body, IPerceivable target, string message, IEmote? emote = null)
    {
        body.OutputHandler.Send("You are unable to speak.");
    }

    public void Talk(IBody body, IPerceivable target, string message, IEmote? emote = null)
    {
        body.OutputHandler.Send("You are unable to speak.");
    }

    public void Whisper(IBody body, IPerceivable target, string message, IEmote? emote = null)
    {
        body.OutputHandler.Send("You are unable to speak.");
    }

    public void Yell(IBody body, IPerceivable target, string message, IEmote? emote = null)
    {
        body.OutputHandler.Send("You are unable to speak.");
    }

    public void Shout(IBody body, IPerceivable target, string message, IEmote? emote = null)
    {
        body.OutputHandler.Send("You are unable to speak.");
    }

    public void Sing(IBody body, IPerceivable target, string message, IEmote? emote = null)
    {
        body.OutputHandler.Send("You are unable to speak.");
    }

    public void Transmit(IBody body, IGameItem target, string message, IEmote? emote = null)
    {
        body.OutputHandler.Send("You are unable to speak.");
    }
}
