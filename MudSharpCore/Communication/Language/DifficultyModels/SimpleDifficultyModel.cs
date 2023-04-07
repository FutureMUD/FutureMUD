using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Communication.Language.DifficultyModels;

public class SimpleDifficultyModel : LanguageDifficultyModel
{
	private const Difficulty Difficulty = RPG.Checks.Difficulty.Automatic;

	public SimpleDifficultyModel(Models.LanguageDifficultyModels model)
	{
		_name = "Simple";
	}

	public override string FrameworkItemType => "SimpleDifficultyModel";

	public override Difficulty RateDifficulty(ExplodedString text)
	{
		return Difficulty;
	}
}