#nullable enable

using MudSharp.Communication.Language;

namespace MudSharp.CharacterCreation.Screens;

public class AccentSkipperScreenStoryboard : ChargenScreenStoryboard
{
	private AccentSkipperScreenStoryboard()
	{
	}

	public AccentSkipperScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		GiveAccentsProg = SkipperProgHelper.Load(gameworld, XElement.Parse(dbitem.StageDefinition), nameof(GiveAccentsProg));
	}

	private AccentSkipperScreenStoryboard(IFuturemud gameworld, IChargenScreenStoryboard storyboard)
		: base(gameworld, storyboard)
	{
		SaveAfterTypeChange();
	}

	protected override string StoryboardName => "AccentSkipper";
	public override ChargenStage Stage => ChargenStage.SelectAccents;
	public IFutureProg? GiveAccentsProg { get; private set; }

	protected override string SaveDefinition() => new XElement("Definition",
		new XElement(nameof(GiveAccentsProg), GiveAccentsProg?.Id ?? 0)).ToString();

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectAccents,
			new ChargenScreenStoryboardFactory("AccentSkipper",
				(game, dbitem) => new AccentSkipperScreenStoryboard(game, dbitem),
				(game, other) => new AccentSkipperScreenStoryboard(game, other)),
			"AccentSkipper", "Skip accent selection and assign accents by optional prog",
			new AccentSkipperScreenStoryboard().HelpText);
	}

	public override IChargenScreen GetScreen(IChargen chargen) => new AccentSkipperScreen(chargen, this);

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder(ShowHeader(voyeur));
		sb.AppendLine("Skips accent selection. The optional prog returns accents for known languages. Unassigned languages use the normal accent fallback among chargen-available accents.".Wrap(voyeur.InnerLineFormatLength));
		sb.AppendLine($"Give Accents Prog: {GiveAccentsProg?.MXPClickableFunctionNameWithId() ?? "None".ColourError()}");
		return sb.ToString();
	}

	public override string HelpText => $@"{BaseHelpText}
	#3accents <prog|none>#0 - sets the optional (chargen) -> accent collection prog";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "accents":
			case "giveaccents":
			case "giveaccentsprog":
				if (!SkipperProgHelper.TryBuildingCommand(Gameworld, actor, command,
					ProgVariableTypes.Collection | ProgVariableTypes.Accent, out var prog)) return false;
				GiveAccentsProg = prog;
				Changed = true;
				actor.OutputHandler.Send($"Give Accents Prog: {prog?.MXPClickableFunctionName() ?? "None".ColourValue()}");
				return true;
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private sealed class AccentSkipperScreen : ChargenScreen
	{
		public AccentSkipperScreen(IChargen chargen, AccentSkipperScreenStoryboard storyboard) : base(chargen, storyboard)
		{
			var languages = storyboard.Gameworld.Languages
				.Where(x => chargen.SelectedSkills.Contains(x.LinkedTrait)).ToList();
			if (chargen.SelectedNativeLanguage is not null && !languages.Contains(chargen.SelectedNativeLanguage))
			{
				chargen.SelectedNativeLanguage = null;
			}

			chargen.SelectedAccents.Clear();
			// Match the picker's automatic assignment before running builder logic.
			foreach (var language in languages)
			{
				var available = storyboard.Gameworld.Accents
					.Where(x => x.Language == language && x.IsAvailableInChargen(chargen)).ToList();
				if (available.Count == 1) chargen.SelectedAccents.Add(available[0]);
			}

			var accents = storyboard.GiveAccentsProg?.ExecuteCollection<IAccent>(chargen) ?? [];
			chargen.SelectedAccents.AddRange(accents
				.Where(x => x is not null && languages.Contains(x.Language))
				.Except(chargen.SelectedAccents).Distinct().ToList());
			foreach (var language in languages.Where(x => chargen.SelectedAccents.All(y => y.Language != x)))
			{
				var nativeLanguage = LanguageAcquisition.ResolveNative(languages,
					x => chargen.SkillValues.FirstOrDefault(y => y.Item1 == x.LinkedTrait).Item2,
					chargen.SelectedNativeLanguage, chargen.SelectedEthnicity?.NativeLanguage, chargen.SelectedCulture?.NativeLanguage);
				var accent = LanguageAcquisition.ResolveAccent(language, nativeLanguage,
					available: storyboard.Gameworld.Accents.Where(x => x.Language == language && x.IsAvailableInChargen(chargen)));
				if (accent is not null) chargen.SelectedAccents.Add(accent);
			}

			State = ChargenScreenState.Complete;
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectAccents;
		public override string Display() => "";
		public override string HandleCommand(string command) => "";
	}
}
