using MudSharp.Body;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;

namespace MudSharp.Strategies.BodyStratagies {
    public interface IBodyCommunicationStrategy {
        string Name { get; }
        void Emote(IBody body, string emote, bool permitSpeech = true, OutputFlags additionalConditions = OutputFlags.Normal);
        bool CanVocalise(IBody body);
        bool CanVocalise(IBody body, AudioVolume volume);
        string WhyCannotVocalise(IBody body);
        string WhyCannotVocalise(IBody body, AudioVolume volume);
        PermitLanguageOptions VocalisationOption(IBody body, AudioVolume volume);
        void Say(IBody body, IPerceivable target, string message, IEmote emote = null);
        void LoudSay(IBody body, IPerceivable target, string message, IEmote emote = null);
        void Talk(IBody body, IPerceivable target, string message, IEmote emote = null);
        void Whisper(IBody body, IPerceivable target, string message, IEmote emote = null);
        void Yell(IBody body, IPerceivable target, string message, IEmote emote = null);
        void Shout(IBody body, IPerceivable target, string message, IEmote emote = null);
        void Sing(IBody body, IPerceivable target, string message, IEmote emote = null);
        void Transmit(IBody body, IGameItem target, string message, IEmote emote = null);
    }
}