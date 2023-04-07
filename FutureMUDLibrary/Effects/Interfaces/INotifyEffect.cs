using MudSharp.Character;

namespace MudSharp.Effects.Interfaces {
    public interface INotifyEffect : IEffectSubtype {
        bool ClanNotification { get; }
        ICharacter NotifyTarget { get; }
    }
}