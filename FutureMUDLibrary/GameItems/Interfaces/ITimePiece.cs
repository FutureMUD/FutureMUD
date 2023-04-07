using MudSharp.Character;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.GameItems.Interfaces {
    public interface ITimePiece : IGameItemComponent {
        IClock Clock { get; }
        IMudTimeZone TimeZone { get; }
        int SecondsOffset { get; }
        bool CanSetTime(ICharacter actor);
        void SetTime(MudTime time);
        MudTime CurrentTime { get; }

        string TimeDisplayString { get; }
    }
}