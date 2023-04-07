using System.Collections.Generic;
using MudSharp.Character;

namespace MudSharp.GameItems.Interfaces {
    public interface ITable : IGameItemComponent {
        IEnumerable<IChair> Chairs { get; }
        int MaximumChairSlots { get; }
        void AddChair(ICharacter character, IChair chair);
        bool CanAddChair(ICharacter character, IChair chair);
        string WhyCannotAddChair(ICharacter character, IChair chair);
        bool CanRemoveChair(ICharacter character, IChair chair);
        string WhyCannotRemoveChair(ICharacter character, IChair chair);
        void RemoveChair(ICharacter character, IChair chair);
    }
}