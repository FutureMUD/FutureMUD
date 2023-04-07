using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.CharacterCreation.Screens;

public class MenuScreenStoryboard : ChargenScreenStoryboard
{
	private MenuScreenStoryboard()
	{
	}

	public MenuScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
	}

	protected override string StoryboardName => "Menu";

	public override ChargenStage Stage => ChargenStage.Menu;

	public override string HelpText => $@"The menu screen storyboard should not be edited";

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition").ToString();
	}

	#endregion

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.Menu,
			new ChargenScreenStoryboardFactory("Menu", (game, dbitem) => new MenuScreenStoryboard(game, dbitem)),
			"Menu",
			"The hard-coded default menu",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine(
			"This is a hard-coded menu screen that allows people to jump to different stages, resume where they left off etc. There is no customisation possible."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		return sb.ToString();
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new MenuScreen(chargen, this);
	}

	internal class MenuScreen : ChargenScreen
	{
		private readonly MenuScreenStoryboard Storyboard;

		internal MenuScreen(IChargen chargen, MenuScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
		}

		public override ChargenStage AssociatedStage => ChargenStage.Menu;

		public override string Display()
		{
			var costs = Chargen.ApplicationCosts;
			return
				string.Format(
					"Character Application Process".Colour(Telnet.Cyan) +
					"\n\n{1}{0}\nSelect an option from the list above.",
					Storyboard.Gameworld.ChargenStoryboard.DefaultOrder.Select(
						x =>
							x.Describe()
							 .Colour(Chargen.CompletedStages.Contains(x)
								 ? Telnet.Yellow
								 : Storyboard.Gameworld.ChargenStoryboard.StageDependencies[x].Any(
									 y => !Chargen.CompletedStages.Contains(y))
									 ? Telnet.Red
									 : Telnet.Green)).ArrangeStringsOntoLines(1),
					costs.Any()
						? $"This application currently costs {costs.Select(x => string.Format(Chargen.Account, "{0:N0} {1}", x.Value, x.Value == 1 ? x.Key.Name.TitleCase() : x.Key.PluralName.TitleCase())).ListToString()}.\n\n"
						: ""
				);
		}

		public override string HandleCommand(string command)
		{
			if (string.IsNullOrEmpty(command))
			{
				return Display();
			}

			var desiredStage = ChargenStage.Menu;
			switch (command.ToLowerInvariant())
			{
				case "submit":
					if (Chargen.CanSubmit)
					{
						Chargen.SetStage(ChargenStage.Submit);
						return Chargen.CurrentScreen.Display();
					}

					return "You cannot submit your application yet as it is incomplete.";
				case "welcome":
					desiredStage = ChargenStage.Welcome;
					break;
				case "special":
				case "specialapp":
				case "special app":
				case "special_app":
					desiredStage = ChargenStage.SpecialApplication;
					break;
				case "race":
					desiredStage = ChargenStage.SelectRace;
					break;

				case "culture":
					desiredStage = ChargenStage.SelectCulture;
					break;

				case "ethnicity":
					desiredStage = ChargenStage.SelectEthnicity;
					break;

				case "name":
					desiredStage = ChargenStage.SelectName;
					break;

				case "birthday":
					desiredStage = ChargenStage.SelectBirthday;
					break;

				case "attributes":
					desiredStage = ChargenStage.SelectAttributes;
					break;

				case "skills":
					desiredStage = ChargenStage.SelectSkills;
					break;

				case "characteristics":
					desiredStage = ChargenStage.SelectCharacteristics;
					break;

				case "gender":
					desiredStage = ChargenStage.SelectGender;
					break;

				case "height":
					desiredStage = ChargenStage.SelectHeight;
					break;

				case "weight":
					desiredStage = ChargenStage.SelectWeight;
					break;

				case "notes":
					desiredStage = ChargenStage.SelectNotes;
					break;

				case "description":
					desiredStage = ChargenStage.SelectDescription;
					break;

				case "accent":
				case "accents":
					desiredStage = ChargenStage.SelectAccents;
					break;
				case "startloc":
				case "location":
					desiredStage = ChargenStage.SelectStartingLocation;
					break;
				case "knowledge":
				case "knowledges":
					desiredStage = ChargenStage.SelectKnowledges;
					break;
				case "merits":
				case "flaws":
				case "merit":
				case "flaw":
				case "quirk":
				case "quirks":
					desiredStage = ChargenStage.SelectMerits;
					break;
				case "handedness":
					desiredStage = ChargenStage.SelectHandedness;
					break;
				case "disfigurements":
					desiredStage = ChargenStage.SelectDisfigurements;
					break;
				case "role":
					desiredStage = ChargenStage.SelectRole;
					break;
			}

			if (desiredStage != ChargenStage.Menu)
			{
				var dependencies = Storyboard.Gameworld.ChargenStoryboard.StageDependencies[desiredStage];
				if (dependencies.Any(x => !Chargen.CompletedStages.Contains(x)))
				{
					return "You cannot go to the " + desiredStage.Describe() +
					       " selection screen until you have completed " +
					       dependencies.Where(x => !Chargen.CompletedStages.Contains(x))
					                   .Select(x => x.Describe())
					                   .ListToString() + ".";
				}

				foreach (
					var stage in
					Storyboard.Gameworld.ChargenStoryboard.StageDependencies.Where(
						x => Chargen.CompletedStages.Contains(x.Key) && x.Value.Any(y => y == desiredStage)))
				{
					Chargen.ResetStage(stage.Key);
				}

				Chargen.SetStage(desiredStage);
				return Chargen.CurrentScreen.Display();
			}

			return "That is not a valid selection.";
		}
	}
}