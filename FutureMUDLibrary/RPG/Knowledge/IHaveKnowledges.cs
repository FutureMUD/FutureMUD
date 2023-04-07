using System.Collections.Generic;

namespace MudSharp.RPG.Knowledge {
    public interface IHaveKnowledges {
        IEnumerable<IKnowledge> Knowledges { get; }
        IEnumerable<ICharacterKnowledge> CharacterKnowledges { get; }
        void AddKnowledge(ICharacterKnowledge knowledge);
        void RemoveKnowledge(IKnowledge knowledge);
    }
}