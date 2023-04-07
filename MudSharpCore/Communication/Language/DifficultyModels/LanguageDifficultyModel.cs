using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Communication.Language.DifficultyModels;

public abstract class LanguageDifficultyModel : FrameworkItem, ILanguageDifficultyModel
{
	public abstract Difficulty RateDifficulty(ExplodedString text);

	public static ILanguageDifficultyModel LoadModel(Models.LanguageDifficultyModels model)
	{
		switch (model.Type)
		{
			case "Simple":
				return new SimpleDifficultyModel(model);
			case "WordList":
				return new WordListDifficultyModel(model);
			default:
				return null;
		}
	}
}