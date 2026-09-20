using System.Reflection;

namespace MudSharp.CharacterCreation.Screens;

public class CustomDescriptionPickerScreenStoryboard : DescriptionPickerScreenStoryboard
{
	private CustomDescriptionPickerScreenStoryboard()
	{
	}

	private CustomDescriptionPickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(gameworld, dbitem)
	{
	}

	private CustomDescriptionPickerScreenStoryboard(IFuturemud gameworld, IChargenScreenStoryboard storyboard)
		: base(gameworld, storyboard)
	{
	}

	protected override string StoryboardName => "CustomDescriptionPicker";

	protected override bool AlwaysUseCustomDescriptions => true;

	public static new void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectDescription,
			new ChargenScreenStoryboardFactory("CustomDescriptionPicker",
				(game, dbitem) => new CustomDescriptionPickerScreenStoryboard(game, dbitem),
				(game, storyboard) => new CustomDescriptionPickerScreenStoryboard(game, storyboard)),
			"CustomDescriptionPicker",
			"Always enter custom short and full descriptions",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override string HelpText => $@"{BaseHelpText}
	#3sdesc#0 - drops into an editor for the sdesc blurb
	#3desc#0 - drops into an editor for the full desc blurb";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (command.Peek().ToLowerInvariant() is "custom" or "pattern" or "patterns")
		{
			actor.OutputHandler.Send("This storyboard always uses custom descriptions. Its saved settings are retained for switching back to DescriptionPicker.");
			return false;
		}

		return base.BuildingCommand(actor, command);
	}

	public override string Show(ICharacter voyeur)
	{
		return base.Show(voyeur) + "\nThis storyboard always uses custom descriptions; the saved settings above apply if you switch back.\n";
	}
}
