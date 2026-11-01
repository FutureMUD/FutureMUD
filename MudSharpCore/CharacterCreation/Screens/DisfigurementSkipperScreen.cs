#nullable enable

using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.GameItems;
using MudSharp.Health;

namespace MudSharp.CharacterCreation.Screens;

public class DisfigurementSkipperScreenStoryboard : ChargenScreenStoryboard
{
	private DisfigurementSkipperScreenStoryboard()
	{
	}

	public DisfigurementSkipperScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		MissingBodypartsProg = SkipperProgHelper.Load(gameworld, definition, nameof(MissingBodypartsProg));
		GiveProsthesesProg = SkipperProgHelper.Load(gameworld, definition, nameof(GiveProsthesesProg));
		GiveScarsProg = SkipperProgHelper.Load(gameworld, definition, nameof(GiveScarsProg));
		GiveTattoosProg = SkipperProgHelper.Load(gameworld, definition, nameof(GiveTattoosProg));
	}

	private DisfigurementSkipperScreenStoryboard(IFuturemud gameworld, IChargenScreenStoryboard storyboard)
		: base(gameworld, storyboard)
	{
		SaveAfterTypeChange();
	}

	protected override string StoryboardName => "DisfigurementSkipper";
	public override ChargenStage Stage => ChargenStage.SelectDisfigurements;
	public IFutureProg? MissingBodypartsProg { get; private set; }
	public IFutureProg? GiveProsthesesProg { get; private set; }
	public IFutureProg? GiveScarsProg { get; private set; }
	public IFutureProg? GiveTattoosProg { get; private set; }

	protected override string SaveDefinition() => new XElement("Definition",
		new XElement(nameof(MissingBodypartsProg), MissingBodypartsProg?.Id ?? 0),
		new XElement(nameof(GiveProsthesesProg), GiveProsthesesProg?.Id ?? 0),
		new XElement(nameof(GiveScarsProg), GiveScarsProg?.Id ?? 0),
		new XElement(nameof(GiveTattoosProg), GiveTattoosProg?.Id ?? 0)).ToString();

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectDisfigurements,
			new ChargenScreenStoryboardFactory("DisfigurementSkipper",
				(game, dbitem) => new DisfigurementSkipperScreenStoryboard(game, dbitem),
				(game, other) => new DisfigurementSkipperScreenStoryboard(game, other)),
			"DisfigurementSkipper", "Skip disfigurement selection with optional per-section progs",
			new DisfigurementSkipperScreenStoryboard().HelpText);
	}

	public override IChargenScreen GetScreen(IChargen chargen) => new DisfigurementSkipperScreen(chargen, this);

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder(ShowHeader(voyeur));
		sb.AppendLine("Skips disfigurement selection, preserving existing selections. Optional void progs run in the order below with the chargen as their sole parameter. Grants have no resource cost.".Wrap(voyeur.InnerLineFormatLength));
		sb.AppendLine($"Missing Bodyparts Prog: {MissingBodypartsProg?.MXPClickableFunctionNameWithId() ?? "None".ColourError()}");
		sb.AppendLine($"Give Prostheses Prog: {GiveProsthesesProg?.MXPClickableFunctionNameWithId() ?? "None".ColourError()}");
		sb.AppendLine($"Give Scars Prog: {GiveScarsProg?.MXPClickableFunctionNameWithId() ?? "None".ColourError()}");
		sb.AppendLine($"Give Tattoos Prog: {GiveTattoosProg?.MXPClickableFunctionNameWithId() ?? "None".ColourError()}");
		return sb.ToString();
	}

	public override string HelpText => $@"{BaseHelpText}
	#3missingbodyparts <prog|none>#0 - sets the missing bodyparts hook
	#3prostheses <prog|none>#0 - sets the prostheses hook
	#3scars <prog|none>#0 - sets the scars hook
	#3tattoos <prog|none>#0 - sets the tattoos hook
All hooks are optional void progs taking one chargen parameter. Use AddChargenMissingBodypart,
AddChargenProsthesis, AddChargenScar and AddChargenTattoo in these progs.";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var option = command.PopForSwitch();
		Action<IFutureProg?> setter;
		switch (option)
		{
			case "missingbodyparts":
			case "missingbodypartsprog":
				setter = prog => MissingBodypartsProg = prog;
				break;
			case "prostheses":
			case "giveprosthesesprog":
				setter = prog => GiveProsthesesProg = prog;
				break;
			case "scars":
			case "givescarsprog":
				setter = prog => GiveScarsProg = prog;
				break;
			case "tattoos":
			case "givetattoosprog":
				setter = prog => GiveTattoosProg = prog;
				break;
			default:
				return BuildingCommandFallback(actor, command.GetUndo());
		}

		if (!SkipperProgHelper.TryBuildingCommand(Gameworld, actor, command, ProgVariableTypes.Void, out var selected)) return false;
		setter(selected);
		Changed = true;
		actor.OutputHandler.Send($"The {option.ColourName()} hook is now {selected?.MXPClickableFunctionName() ?? "None".ColourValue()}.");
		return true;
	}

	private sealed class DisfigurementSkipperScreen : ChargenScreen
	{
		public DisfigurementSkipperScreen(IChargen chargen, DisfigurementSkipperScreenStoryboard storyboard) : base(chargen, storyboard)
		{
			// The picker preserves selections and ensures all of these collections exist.
			chargen.SelectedDisfigurements ??= new List<(IDisfigurementTemplate Disfigurement, IBodypart Bodypart)>();
			chargen.SelectedTattoos ??= new List<ISelectedTattoo>();
			chargen.SelectedProstheses ??= new List<IGameItemProto>();
			chargen.MissingBodyparts ??= new List<IBodypart>();
			chargen.SelectedScars ??= new List<IScar>();
			// Migrate legacy tattoo selections just as the picker does when it updates chargen.
			foreach (var (template, part) in chargen.SelectedDisfigurements.Where(x => x.Disfigurement is ITattooTemplate))
			{
				if (chargen.SelectedTattoos.All(x => x.Tattoo != template || x.Bodypart != part))
				{
					chargen.SelectedTattoos.Add(new SelectedTattoo((ITattooTemplate)template, part,
						new Dictionary<string, ITattooTextValue>(StringComparer.InvariantCultureIgnoreCase)));
				}
			}

			chargen.SelectedDisfigurements.Clear();
			storyboard.MissingBodypartsProg?.Execute(chargen);
			storyboard.GiveProsthesesProg?.Execute(chargen);
			storyboard.GiveScarsProg?.Execute(chargen);
			storyboard.GiveTattoosProg?.Execute(chargen);
			State = ChargenScreenState.Complete;
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectDisfigurements;
		public override string Display() => "";
		public override string HandleCommand(string command) => "";
	}
}
