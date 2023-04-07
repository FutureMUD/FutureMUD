using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Communication.Language.DifficultyModels;

/// <summary>
///     A WordListDifficultyModel model makes the difficulty depend on the complexity of the sentences being uttered
/// </summary>
public class WordListDifficultyModel : LanguageDifficultyModel
{
	private readonly RankedRange<Difficulty> _sentenceDifficulties = new();

	private readonly Dictionary<string, Difficulty> _wordList = new();
	private Difficulty _defaultDifficulty = Difficulty.Normal;

	public WordListDifficultyModel(Models.LanguageDifficultyModels model)
	{
		_id = model.Id;
		LoadFromXml(XElement.Parse(model.Definition));
	}

	public override string FrameworkItemType => "WordListDifficultyModel";

	public override Difficulty RateDifficulty(ExplodedString text)
	{
		return
			(Difficulty)
			Math.Max(
				text.Words.Select(x => x.ToLowerInvariant())
				    .DefaultIfEmpty("")
				    .Max(x => (int)_wordList.ValueOrDefault(x, _defaultDifficulty)),
				(int)_sentenceDifficulties.Find(text.SentenceLengths.DefaultIfEmpty(0).Max()));
	}

	protected void LoadFromXml(XElement root)
	{
		var element = root.Element("DefaultDifficulty");
		if (element != null)
		{
			_defaultDifficulty = (Difficulty)Convert.ToInt32(element.Value);
		}

		element = root.Element("Name");
		if (element != null)
		{
			_name = element.Value;
		}

		element = root.Element("Words");
		if (element != null)
		{
			foreach (var sub in element.Elements("Word"))
			{
				_wordList[sub.Attribute("Text").Value.ToLowerInvariant()] =
					(Difficulty)Convert.ToInt32(sub.Attribute("Difficulty").Value);
			}
		}

		element = root.Element("Sentences");
		if (element != null)
		{
			foreach (var sub in element.Elements("Length"))
			{
				_sentenceDifficulties.Add((Difficulty)Convert.ToInt32(sub.Attribute("Difficulty").Value),
					Convert.ToInt32(sub.Attribute("Min").Value), Convert.ToInt32(sub.Attribute("Max").Value));
			}
		}
	}
}