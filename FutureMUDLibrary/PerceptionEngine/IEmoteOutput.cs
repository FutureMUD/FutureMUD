using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp.PerceptionEngine
{
    public interface IEmoteOutput : IOutput
    {
        IEmote DefaultEmote { get; }
        IPerceiver DefaultSource { get; }
        bool AllValid { get; }
        Difficulty NoticeCheckDifficulty { get; set; }
        void AssignEmote(string emote, bool forceSourceInclusion = false, params IPerceiver[] perceivers);

        void AssignEmote(IPerceiver source, string emote, bool forceSourceInclusion = false,
            params IPerceiver[] perceivers);
    }
}