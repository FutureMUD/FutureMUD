#nullable enable

namespace MudSharp.CharacterCreation.Screens;

public sealed class NoteSkipperScreenStoryboard : NotePickerScreenStoryboard
{
	public NoteSkipperScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(gameworld, dbitem)
	{
	}

	private NoteSkipperScreenStoryboard(IFuturemud gameworld, IChargenScreenStoryboard storyboard)
		: base(gameworld, storyboard)
	{
	}

	protected override string StoryboardName => "NoteSkipper";

	public new static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectNotes,
			new ChargenScreenStoryboardFactory("NoteSkipper",
				(game, row) => new NoteSkipperScreenStoryboard(game, row),
				(game, other) => new NoteSkipperScreenStoryboard(game, other)),
			"NoteSkipper", "Skip entering character notes", BaseHelpText);
	}

	public override string Show(ICharacter voyeur) => ShowHeader(voyeur) + "Skips entering character notes.";
	public override string HelpText => BaseHelpText;
	public override bool BuildingCommand(ICharacter actor, StringStack command) => BuildingCommandFallback(actor, command);
	public override IChargenScreen GetScreen(IChargen chargen) => new NoteSkipperScreen(chargen, this);

	private sealed class NoteSkipperScreen : ChargenScreen
	{
		public NoteSkipperScreen(IChargen chargen, NoteSkipperScreenStoryboard storyboard) : base(chargen, storyboard)
		{
			State = ChargenScreenState.Complete;
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectNotes;
		public override string Display() => string.Empty;
		public override string HandleCommand(string command) => string.Empty;
	}
}
