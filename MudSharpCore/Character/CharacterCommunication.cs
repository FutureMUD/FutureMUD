using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Traits;
using MudSharp.Communication.Language;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;
using MudSharp.Models;

namespace MudSharp.Character;

public partial class Character
{
	private bool _languagesChanged;

	public bool LanguagesChanged
	{
		get => _languagesChanged;
		protected set
		{
			if (value && !_languagesChanged)
			{
				Changed = true;
			}

			_languagesChanged = value;
		}
	}

	protected List<ILanguage> _languages = new();
	public IEnumerable<ILanguage> Languages => _languages;

	private ILanguage _currentLanguage;

	public ILanguage CurrentLanguage
	{
		get => _currentLanguage;
		set
		{
			_currentLanguage = value;
			LanguagesChanged = true;
		}
	}

	private ILanguage _currentWritingLanguage;

	public ILanguage CurrentWritingLanguage
	{
		get => _currentWritingLanguage;
		set
		{
			_currentWritingLanguage = value;
			LanguagesChanged = true;
		}
	}

	protected Dictionary<IAccent, Difficulty> _accents = new();
	public IEnumerable<IAccent> Accents => _accents.Keys;

	protected Dictionary<ILanguage, IAccent> _preferredAccents = new();

	private IAccent _currentAccent;

	public IAccent CurrentAccent
	{
		get => _currentAccent;
		set
		{
			_currentAccent = value;
			LanguagesChanged = true;
		}
	}

	public void SetPreferredAccent(IAccent accent)
	{
		_preferredAccents[accent.Language] = accent;
		LanguagesChanged = true;
	}

	public IAccent PreferredAccent(ILanguage language)
	{
		return _preferredAccents.ValueOrDefault(language, null);
	}

	private readonly List<IScript> _scripts = new();
	public IEnumerable<IScript> Scripts => _scripts;

	private IScript _currentScript;

	public IScript CurrentScript
	{
		get => _currentScript;
		set
		{
			_currentScript = value;
			LanguagesChanged = true;
		}
	}

	private WritingStyleDescriptors _writingStyle;

	public WritingStyleDescriptors WritingStyle
	{
		get => _writingStyle;
		set
		{
			_writingStyle = value;
			LanguagesChanged = true;
		}
	}

	public void LearnLanguage(ILanguage language)
	{
		_languages.Add(language);
		LanguagesChanged = true;
	}

	public void LearnAccent(IAccent accent, Difficulty difficulty)
	{
		if (_accents.ContainsKey(accent))
		{
			_accents[accent] = difficulty;
		}
		else
		{
			_accents.Add(accent, difficulty);
		}

		LanguagesChanged = true;
	}

	public void LearnScript(IScript script)
	{
		if (!_scripts.Contains(script))
		{
			_scripts.Add(script);
			LanguagesChanged = true;
		}
	}

	public void ForgetLanguage(ILanguage language)
	{
		_languages.Remove(language);
		foreach (var accent in _accents.Keys.Where(x => x.Language == language).ToList())
		{
			_accents.Remove(accent);
		}

		LanguagesChanged = true;
	}

	public void ForgetAccent(IAccent accent)
	{
		_accents.Remove(accent);
		LanguagesChanged = true;
	}

	public void ForgetScript(IScript script)
	{
		_scripts.Remove(script);
		if (CurrentScript == script)
		{
			CurrentScript = null;
			CurrentWritingLanguage = null;
		}

		LanguagesChanged = true;
	}

	protected void SaveLanguages(MudSharp.Models.Character dbchar)
	{
		dbchar.CurrentLanguageId = CurrentLanguage?.Id;
		dbchar.CurrentAccentId = CurrentAccent?.Id;
		dbchar.CurrentWritingLanguageId = CurrentWritingLanguage?.Id;
		dbchar.CurrentScriptId = CurrentScript?.Id;
		dbchar.WritingStyle = (int)WritingStyle;

		FMDB.Context.CharactersAccents.RemoveRange(dbchar.CharactersAccents);
		foreach (var accent in _accents)
		{
			var dbitem = new CharacterAccent
			{
				Character = dbchar,
				AccentId = accent.Key.Id,
				Familiarity = (int)accent.Value,
				IsPreferred = _preferredAccents.ContainsValue(accent.Key)
			};
			dbchar.CharactersAccents.Add(dbitem);
		}

		FMDB.Context.CharactersLanguages.RemoveRange(dbchar.CharactersLanguages);
		foreach (var language in _languages)
		{
			dbchar.CharactersLanguages.Add(new CharactersLanguages { Character = dbchar, LanguageId = language.Id });
		}

		FMDB.Context.CharactersScripts.RemoveRange(dbchar.CharactersScripts);
		foreach (var script in _scripts)
		{
			dbchar.CharactersScripts.Add(new CharactersScripts { Character = dbchar, ScriptId = script.Id });
		}

		LanguagesChanged = false;
	}

	public Difficulty AccentDifficulty(IAccent accent, bool canImprove = true)
	{
		if (canImprove && !AffectedBy<INoAccentGainEffect>(accent))
		{
			var effect = EffectHandler.EffectsOfType<NoAccentGain>().FirstOrDefault();
			if (effect == null)
			{
				effect = new NoAccentGain(this, accent);
				AddEffect(effect, TimeSpan.FromHours(3));
			}
			else
			{
				effect.Accents.Add(accent);
				effect.Changed = true;
			}

			EffectHandler.RescheduleIfLonger(effect, TimeSpan.FromHours(3));

			if (_accents.ContainsKey(accent))
			{
				if (Gameworld.GetCheck(CheckType.AccentImproveCheck).Check(this, accent.Difficulty).IsPass())
				{
					if (Gameworld.GetStaticBool("AllowAccentsToGetToAutomatic") ||
					    _accents[accent] > Difficulty.Trivial)
					{
						_accents[accent] = _accents[accent].StageDown(1);
						LanguagesChanged = true;
					}
				}
			}
			else
			{
				if (Languages.Contains(accent.Language))
				{
					if (Gameworld.GetCheck(CheckType.AccentAcquireCheck).Check(this, accent.Difficulty).IsPass())
					{
						_accents[accent] = accent.Difficulty;
						LanguagesChanged = true;
					}
				}
			}
		}

		var difficulty = (Difficulty)Math.Min((int)accent.Difficulty,
			(int)(_accents.ContainsKey(accent) ? _accents[accent] : Difficulty.Impossible));
		if (_accents.MinCountOrAll(
			    x => x.Key != accent && x.Key.Group.EqualTo(accent.Group) &&
			         x.Value.In(Difficulty.Automatic, Difficulty.Trivial), 3))
		{
			difficulty = difficulty.StageDown(2);
		}
		else if (_accents.MinCountOrAll(
			         x => x.Key != accent && x.Key.Group.EqualTo(accent.Group) && x.Value <= Difficulty.Easy, 1))
		{
			difficulty = difficulty.StageDown(1);
		}

		return difficulty;
	}

	public bool IsLiterate
	{
		get
		{
			// TODO - merits and flaws
			if (Gameworld.GetStaticBool("CharactersLiterateByDefault"))
			{
				return true;
			}

			if (Traits.Any(x => x.Definition == Gameworld.Traits.Get(Gameworld.GetStaticLong("LiteracySkillId"))))
			{
				return true;
			}

			return false;
		}
	}

	public ITraitDefinition LanguageForReadCheck(IWriting writing)
	{
		if (Languages.Contains(writing.Language))
		{
			return writing.Language.LinkedTrait;
		}

		var mutual = Languages.FirstMin(x => x.MutualIntelligability(writing.Language));
		return mutual?.LinkedTrait ?? writing.Language.LinkedTrait;
	}

	public bool CanRead(IWriting writing)
	{
		if (IsAdministrator() == true)
		{
			return true;
		}

		if (!IsLiterate)
		{
			return false;
		}

		if (!Scripts.Contains(writing.Script))
		{
			return false;
		}

		var difficulty = WritingDifficulty(writing);
		var result = Gameworld.GetCheck(CheckType.WritingComprehendCheck).Check(this, difficulty, LanguageForReadCheck(writing));
		if (result.IsFail())
		{
			return false;
		}

		return true;
	}

	public string WhyCannotRead(IWriting writing)
	{
		if (!IsLiterate)
		{
			return "You cannot read that because you are illiterate.";
		}

		if (!Scripts.Contains(writing.Script))
		{
			return "You cannot read that because you are unfamiliar with the script in which it is written.";
		}

		if (!Languages.Contains(writing.Language))
		{
			return "Although you can read the script, you are unfamiliar with the language in which that is written.";
		}

		return "This piece of writing is too hard for you to understand.";
	}

	public string GetWritingHeader(IWriting writing)
	{
		return
			$"Language: {writing.Language.Name.Colour(Telnet.Green)} {writing.Language.LinkedTrait.Decorator.Decorate(writing.LanguageSkill).Colour(Telnet.Cyan)}, Script: {writing.Script.Name.Colour(Telnet.Green)} {$"({writing.Style.Describe().TitleCase()})".Colour(Telnet.Cyan)}\nWritten in {writing.ImplementType.Describe(writing.WritingColour, Telnet.Green)}."
				.ColourIncludingReset(Telnet.Yellow);
	}

	private Difficulty WritingDifficulty(IWriting writing)
	{
		return writing.WritingDifficulty(this);
	}

	public bool Read(IWriting writing)
	{
		var difficulty = WritingDifficulty(writing);
		Gameworld.GetCheck(CheckType.ReadTextImprovementCheck).Check(this, difficulty);
		return true;
	}

	public bool CanWrite()
	{
		throw new NotImplementedException();
	}

	public bool CanIdentifyLanguage(ILanguage language)
	{
		return
			Languages.Contains(language) ||
			Languages.Any(x => x.MutualIntelligability(language) != Difficulty.Impossible);
	}
}