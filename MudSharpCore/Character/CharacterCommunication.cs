using MudSharp.Body.Traits;
using MudSharp.Communication.Language;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Models;
using MudSharp.RPG.Checks;

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

	private ILanguage _nativeLanguage;
	private readonly Dictionary<ILanguage, IAccent> _acquisitionAccents = new();
	public ILanguage NativeLanguage
	{
		get => _nativeLanguage;
		set
		{
			if (value is not null && !_languages.Contains(value)) throw new ArgumentException("Native language must be known.", nameof(value));
			_nativeLanguage = value;
			if (value is null) ResolveNativeLanguage();
			LanguagesChanged = true;
		}
	}

	private void ResolveNativeLanguage(params ILanguage[] preferences)
	{
		_nativeLanguage = LanguageAcquisition.ResolveNative(_languages,
			language => GetTrait(language.LinkedTrait)?.RawValue ?? 0.0, preferences);
		LanguagesChanged = true;
	}

	public IAccent AcquisitionAccent(ILanguage language) => language is null ? null : _acquisitionAccents.GetValueOrDefault(language);

	private void EnsureAcquisitionAccent(ILanguage language, IAccent heard = null, IEnumerable<IAccent> available = null, bool inferFromKnown = true)
	{
		if (_acquisitionAccents.ContainsKey(language)) return;
		var accent = inferFromKnown ? PreferredAccent(language) ?? _accents.Where(x => x.Key.Language == language)
			.OrderBy(x => x.Value).ThenBy(x => x.Key.Id).Select(x => x.Key).FirstOrDefault() : null;
		accent ??= LanguageAcquisition.ResolveAccent(language, NativeLanguage, heard, available);
		if (accent is null) return;
		_acquisitionAccents[language] = accent;
		if (!_accents.ContainsKey(accent)) LearnAccent(accent, Difficulty.Normal);
		if (inferFromKnown) _preferredAccents.TryAdd(language, accent);
		else _preferredAccents[language] = accent;
		if (CurrentLanguage == language && CurrentAccent is null) CurrentAccent = accent;
		LanguagesChanged = true;
	}

	private Difficulty AccentFamiliarityFloor(IAccent accent)
	{
		if (accent.Role != AccentRole.Native || accent.Language == NativeLanguage) return Difficulty.Automatic;
		var trait = GetTrait(accent.Language.LinkedTrait);
		var capped = trait is not null && trait.RawValue >= trait.MaxValue;
		var acquisition = AcquisitionAccent(accent.Language);
		var mastery = Gameworld.GetStaticBool("AllowAccentsToGetToAutomatic") ? Difficulty.Automatic : Difficulty.Trivial;
		if (capped && (acquisition == accent || acquisition is not null &&
			_accents.GetValueOrDefault(acquisition, Difficulty.Impossible) <= mastery)) return Difficulty.Automatic;
		var configured = Gameworld.GetStaticConfiguration("NativeAccentFamiliarityFloor");
		var floor = Enum.TryParse<Difficulty>(configured, true, out var parsed) && Enum.IsDefined(parsed) ? parsed : Difficulty.Easy;
		// Never worsen familiarity already earned or loaded from an older game.
		return (Difficulty)Math.Min((int)floor, (int)_accents.GetValueOrDefault(accent, Difficulty.Impossible));
	}

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

	protected readonly List<ISignedLanguage> _signedLanguages = [];
	public IEnumerable<ISignedLanguage> SignedLanguages => _signedLanguages;
	protected readonly Dictionary<ISignedLanguageVariety, Difficulty> _signedLanguageVarieties = [];
	public IEnumerable<ISignedLanguageVariety> SignedLanguageVarieties => _signedLanguageVarieties.Keys;

	private ISignedLanguage _currentSignedLanguage;
	public ISignedLanguage CurrentSignedLanguage
	{
		get => _currentSignedLanguage;
		set
		{
			_currentSignedLanguage = value;
			if (_currentSignedLanguageVariety?.Language != value)
			{
				_currentSignedLanguageVariety = null;
			}
			LanguagesChanged = true;
		}
	}

	private ISignedLanguageVariety _currentSignedLanguageVariety;
	public ISignedLanguageVariety CurrentSignedLanguageVariety
	{
		get => _currentSignedLanguageVariety;
		set
		{
			_currentSignedLanguageVariety = value?.Language == CurrentSignedLanguage ? value : null;
			LanguagesChanged = true;
		}
	}

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
		return language is null ? null : _preferredAccents.ValueOrDefault(language, null);
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
		if (_languages.Contains(language)) return;
		if (_nativeLanguage is null && _languages.Count > 0) ResolveNativeLanguage();
        _languages.Add(language);
		_nativeLanguage ??= language;
		CurrentLanguage ??= language;
		EnsureAcquisitionAccent(language, LanguageAcquisitionContext.HeardBy(this) ?? LanguageAcquisitionContext.HeardBy(Body), inferFromKnown: false);
        LanguagesChanged = true;
    }

	public void LearnSignedLanguage(ISignedLanguage language)
	{
		if (_signedLanguages.Contains(language))
		{
			return;
		}
		_signedLanguages.Add(language);
		CurrentSignedLanguage ??= language;
		LanguagesChanged = true;
	}

	public void ForgetSignedLanguage(ISignedLanguage language)
	{
		_signedLanguages.Remove(language);
		foreach (var variety in _signedLanguageVarieties.Keys.Where(x => x.Language == language).ToList())
		{
			_signedLanguageVarieties.Remove(variety);
		}
		if (CurrentSignedLanguage == language)
		{
			CurrentSignedLanguage = _signedLanguages.FirstOrDefault();
		}
		LanguagesChanged = true;
	}

	public void LearnSignedLanguageVariety(ISignedLanguageVariety variety)
	{
		_signedLanguageVarieties.TryAdd(variety, variety.RecognitionDifficulty);
		LanguagesChanged = true;
	}

	public void ForgetSignedLanguageVariety(ISignedLanguageVariety variety)
	{
		_signedLanguageVarieties.Remove(variety);
		if (CurrentSignedLanguageVariety == variety)
		{
			CurrentSignedLanguageVariety = null;
		}
		LanguagesChanged = true;
	}

	public Difficulty SignedLanguageVarietyDifficulty(ISignedLanguageVariety variety) =>
		_signedLanguageVarieties.GetValueOrDefault(variety, Difficulty.Impossible);

    public void LearnAccent(IAccent accent, Difficulty difficulty)
    {
		if (accent is null) return;
		var floor = AccentFamiliarityFloor(accent);
		_accents[accent] = (Difficulty)Math.Max((int)difficulty, (int)floor);
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
		_acquisitionAccents.Remove(language);
		_preferredAccents.Remove(language);
		if (_nativeLanguage == language) ResolveNativeLanguage();
		if (CurrentLanguage == language) CurrentLanguage = _nativeLanguage;
		if (CurrentWritingLanguage == language) CurrentWritingLanguage = _nativeLanguage;
		if (CurrentAccent?.Language == language) CurrentAccent = PreferredAccent(CurrentLanguage);
        foreach (IAccent accent in _accents.Keys.Where(x => x.Language == language).ToList())
        {
            _accents.Remove(accent);
        }

        LanguagesChanged = true;
    }

    public void ForgetAccent(IAccent accent)
    {
        _accents.Remove(accent);
		if (_preferredAccents.GetValueOrDefault(accent.Language) == accent) _preferredAccents.Remove(accent.Language);
		if (CurrentAccent == accent) CurrentAccent = _accents.Keys.Where(x => x.Language == CurrentLanguage).OrderBy(x => x.Id).FirstOrDefault();
		// Keep the acquisition identity: forgetting it must not bypass its mastery requirement.
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
		dbchar.NativeLanguageId = NativeLanguage?.Id;
        dbchar.CurrentLanguageId = CurrentLanguage?.Id;
        dbchar.CurrentAccentId = CurrentAccent?.Id;
        dbchar.CurrentWritingLanguageId = CurrentWritingLanguage?.Id;
        dbchar.CurrentScriptId = CurrentScript?.Id;
        dbchar.WritingStyle = (int)WritingStyle;
		dbchar.CurrentSignedLanguageId = CurrentSignedLanguage?.Id;
		dbchar.CurrentSignedLanguageVarietyId = CurrentSignedLanguageVariety?.Id;

        FMDB.Context.CharactersAccents.RemoveRange(dbchar.CharactersAccents);
        foreach (KeyValuePair<IAccent, Difficulty> accent in _accents)
        {
            CharacterAccent dbitem = new()
            {
                Character = dbchar,
                AccentId = accent.Key.Id,
                Familiarity = (int)accent.Value,
                IsPreferred = _preferredAccents.ContainsValue(accent.Key)
            };
            dbchar.CharactersAccents.Add(dbitem);
        }

        FMDB.Context.CharactersLanguages.RemoveRange(dbchar.CharactersLanguages);
        foreach (ILanguage language in _languages)
        {
			dbchar.CharactersLanguages.Add(new CharactersLanguages { Character = dbchar, LanguageId = language.Id, AcquisitionAccentId = AcquisitionAccent(language)?.Id });
        }

		FMDB.Context.CharactersSignedLanguages.RemoveRange(dbchar.CharactersSignedLanguages);
		foreach (var language in _signedLanguages)
		{
			dbchar.CharactersSignedLanguages.Add(new CharactersSignedLanguage
			{
				Character = dbchar,
				SignedLanguageId = language.Id
			});
		}

		FMDB.Context.CharactersSignedLanguageVarieties.RemoveRange(dbchar.CharactersSignedLanguageVarieties);
		foreach (var variety in _signedLanguageVarieties)
		{
			dbchar.CharactersSignedLanguageVarieties.Add(new CharacterSignedLanguageVariety
			{
				Character = dbchar,
				SignedLanguageVarietyId = variety.Key.Id,
				Familiarity = (int)variety.Value
			});
		}

        FMDB.Context.CharactersScripts.RemoveRange(dbchar.CharactersScripts);
        foreach (IScript script in _scripts)
        {
            dbchar.CharactersScripts.Add(new CharactersScripts { Character = dbchar, ScriptId = script.Id });
        }

        LanguagesChanged = false;
    }

    public Difficulty AccentDifficulty(IAccent accent, bool canImprove = true)
    {
		if (accent is null)
		{
			return Difficulty.Automatic;
		}

        if (canImprove && !AffectedBy<INoAccentGainEffect>(accent))
        {
            NoAccentGain effect = EffectHandler.EffectsOfType<NoAccentGain>().FirstOrDefault();
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
						_accents[accent] = (Difficulty)Math.Max((int)_accents[accent].StageDown(1), (int)AccentFamiliarityFloor(accent));
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
						_accents[accent] = (Difficulty)Math.Max((int)accent.Difficulty, (int)AccentFamiliarityFloor(accent));
                        LanguagesChanged = true;
                    }
                }
            }
        }

        Difficulty difficulty = (Difficulty)Math.Min((int)accent.Difficulty,
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

		return (Difficulty)Math.Max((int)difficulty, (int)AccentFamiliarityFloor(accent));
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
        if (EffectsOfType<IComprehendLanguageEffect>().Any(x => x.Applies()))
        {
            return writing.Language.LinkedTrait;
        }

        if (Languages.Contains(writing.Language))
        {
            return writing.Language.LinkedTrait;
        }

        ILanguage mutual = Languages.FirstMin(x => x.MutualIntelligability(writing.Language));
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

        if (EffectsOfType<IComprehendLanguageEffect>().Any(x => x.Applies()))
        {
            return true;
        }

        Difficulty difficulty = WritingDifficulty(writing);
        CheckOutcome result = Gameworld.GetCheck(CheckType.WritingComprehendCheck).Check(this, difficulty, LanguageForReadCheck(writing));
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

        if (EffectsOfType<IComprehendLanguageEffect>().Any(x => x.Applies()))
        {
            return string.Empty;
        }

        if (!Languages.Contains(writing.Language))
        {
            return "Although you can read the script, you are unfamiliar with the language in which that is written.";
        }

        return "This piece of writing is too hard for you to understand.";
    }

    public string GetWritingHeader(IWriting writing)
    {
        var provenance = writing.Author is null
            ? $" Source: {(writing.GetProperty("provenance")?.GetObject as string).IfNullOrWhiteSpace("unspecified").Colour(Telnet.Cyan)}"
            : string.Empty;
        return
            $"Language: {writing.Language.Name.Colour(Telnet.Green)} {writing.Language.LinkedTrait.Decorator.Decorate(writing.LanguageSkill).Colour(Telnet.Cyan)}, Script: {writing.Script.Name.Colour(Telnet.Green)} {$"({writing.Style.Describe().TitleCase()})".Colour(Telnet.Cyan)}\nWritten in {writing.ImplementType.Describe(writing.WritingColour, Telnet.Green)}.{provenance}"
                .ColourIncludingReset(Telnet.Yellow);
    }

    private Difficulty WritingDifficulty(IWriting writing)
    {
        return writing.WritingDifficulty(this);
    }

    public bool Read(IWriting writing)
    {
        Difficulty difficulty = WritingDifficulty(writing);
        Gameworld.GetCheck(CheckType.ReadTextImprovementCheck).Check(this, difficulty);
        return true;
    }

    public bool CanWrite()
    {
        throw new NotImplementedException();
    }

    public bool CanIdentifyLanguage(ILanguage language)
    {
        if (EffectsOfType<IComprehendLanguageEffect>().Any(x => x.Applies()))
        {
            return true;
        }

        return
            Languages.Contains(language) ||
            Languages.Any(x => x.MutualIntelligability(language) != Difficulty.Impossible);
    }

	public bool CanIdentifySignedLanguage(ISignedLanguage language)
	{
		if (EffectsOfType<IComprehendLanguageEffect>().Any(x => x.Applies()))
		{
			return true;
		}

		return SignedLanguages.Contains(language) ||
		       SignedLanguages.Any(x => x.MutualIntelligability(language) != Difficulty.Impossible);
	}
}
