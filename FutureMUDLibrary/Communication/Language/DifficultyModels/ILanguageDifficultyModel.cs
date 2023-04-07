using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Communication.Language.DifficultyModels {
    public interface ILanguageDifficultyModel : IFrameworkItem {
        Difficulty RateDifficulty(ExplodedString text);
    }
}