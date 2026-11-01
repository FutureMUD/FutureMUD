#nullable enable

using MudSharp.RPG.Merits;

namespace MudSharp.CharacterCreation.Screens;

public class QuirkSkipperScreenStoryboard : ChargenScreenStoryboard
{
	private QuirkSkipperScreenStoryboard()
	{
	}

	public QuirkSkipperScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		GiveQuirksProg = SkipperProgHelper.Load(gameworld, XElement.Parse(dbitem.StageDefinition), nameof(GiveQuirksProg));
	}

	private QuirkSkipperScreenStoryboard(IFuturemud gameworld, IChargenScreenStoryboard storyboard)
		: base(gameworld, storyboard)
	{
		SaveAfterTypeChange();
	}

	protected override string StoryboardName => "QuirkSkipper";
	public override ChargenStage Stage => ChargenStage.SelectMerits;
	public IFutureProg? GiveQuirksProg { get; private set; }

	protected override string SaveDefinition() => new XElement("Definition",
		new XElement(nameof(GiveQuirksProg), GiveQuirksProg?.Id ?? 0)).ToString();

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectMerits,
			new ChargenScreenStoryboardFactory("QuirkSkipper",
				(game, dbitem) => new QuirkSkipperScreenStoryboard(game, dbitem),
				(game, other) => new QuirkSkipperScreenStoryboard(game, other)),
			"QuirkSkipper", "Skip quirk selection and give merits or flaws by optional prog",
			new QuirkSkipperScreenStoryboard().HelpText);
	}

	public override IChargenScreen GetScreen(IChargen chargen) => new QuirkSkipperScreen(chargen, this);

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder(ShowHeader(voyeur));
		sb.AppendLine("Skips quirk selection and grants merits and flaws from an optional prog, without pick limits or resource costs.".Wrap(voyeur.InnerLineFormatLength));
		sb.AppendLine($"Give Quirks Prog: {GiveQuirksProg?.MXPClickableFunctionNameWithId() ?? "None".ColourError()}");
		return sb.ToString();
	}

	public override string HelpText => $@"{BaseHelpText}
	#3quirks <prog|none>#0 - sets the optional (chargen) -> merit collection prog";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "quirks":
			case "givequirks":
			case "givequirksprog":
				if (!SkipperProgHelper.TryBuildingCommand(Gameworld, actor, command,
					ProgVariableTypes.Collection | ProgVariableTypes.Merit, out var prog)) return false;
				GiveQuirksProg = prog;
				Changed = true;
				actor.OutputHandler.Send($"Give Quirks Prog: {prog?.MXPClickableFunctionName() ?? "None".ColourValue()}");
				return true;
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private sealed class QuirkSkipperScreen : ChargenScreen
	{
		public QuirkSkipperScreen(IChargen chargen, QuirkSkipperScreenStoryboard storyboard) : base(chargen, storyboard)
		{
			chargen.SelectedMerits.Clear();
			chargen.SelectedMerits.AddRange((storyboard.GiveQuirksProg?.ExecuteCollection<IMerit>(chargen) ?? [])
				.OfType<ICharacterMerit>().Distinct().ToList());
			State = ChargenScreenState.Complete;
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectMerits;
		public override string Display() => "";
		public override string HandleCommand(string command) => "";
	}
}
