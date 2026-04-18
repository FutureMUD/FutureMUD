using MudSharp.Framework;
using MudSharp.Framework.Revision;
using System.Collections.Generic;

namespace MudSharp.CharacterCreation
{
    public interface IChargenStoryboard : IHaveFuturemud
    {
        IReadOnlyDictionary<ChargenStage, ChargenStage> DefaultNextStage { get; }
        IReadOnlyCollection<ChargenStage> DefaultOrder { get; }
        int OrderOf(ChargenStage stage);
        ChargenStage FirstStage { get; }
        IReadOnlyCollectionDictionary<ChargenStage, ChargenStage> StageDependencies { get; }
        IReadOnlyDictionary<ChargenStage, IChargenScreenStoryboard> StageScreenMap { get; }

        void ReorderStage(ChargenStage stage, ChargenStage afterStage);
        void AddDependency(ChargenStage stage, ChargenStage dependingStage);
        void RemoveDependency(ChargenStage stage, ChargenStage dependingStage);
        void SwapStoryboard(ChargenStage stage, string newType);
    }
}