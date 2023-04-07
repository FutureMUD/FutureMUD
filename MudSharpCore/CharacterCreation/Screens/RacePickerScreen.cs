using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.CharacterCreation.Screens;

public class RacePickerScreenStoryboard : ChargenScreenStoryboard
{
	private RacePickerScreenStoryboard()
	{
	}

	public RacePickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		Blurb = definition.Element("Blurb").Value;
		ShowUnselectableRacesAsBlanks = Convert.ToBoolean(definition.Element("ShowUnselectableRacesAsBlanks").Value);
		SkipScreenIfOnlyOneChoice = definition.Element("SkipScreenIfOnlyOneChoice") != null
			? bool.Parse(definition.Element("SkipScreenIfOnlyOneChoice").Value)
			: false;
	}

	protected override string StoryboardName => "RacePicker";

	/// <summary>
	///     The text displayed above the Race Selection auto-generated component
	/// </summary>
	public string Blurb { get; protected set; }

	/// <summary>
	///     Controls whether Unselectable Races appear as blank options, or whether they are removed from the list
	/// </summary>
	public bool ShowUnselectableRacesAsBlanks { get; protected set; }

	public bool SkipScreenIfOnlyOneChoice { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectRace;

	public override string HelpText => $@"{BaseHelpText}
	#3blurb#0 - drops you into an editor to change the blurb
	#3blanks#0 - toggles showing unselectable options as blank spots (e.g. old SOI)
	#3skipone#0 - toggles skipping the screen if there is only one valid choice";

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Blurb", new XCData(Blurb)),
			new XElement("ShowUnselectableRacesAsBlanks", ShowUnselectableRacesAsBlanks),
			new XElement("SkipScreenIfOnlyOneChoice", SkipScreenIfOnlyOneChoice)
		).ToString();
	}

	#endregion

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectRace,
			new ChargenScreenStoryboardFactory("RacePicker",
				(game, dbitem) => new RacePickerScreenStoryboard(game, dbitem)),
			"RacePicker",
			"Select a race from a list",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	#region Overrides of ChargenScreenStoryboard

	public override IEnumerable<(IChargenResource Resource, int Cost)> ChargenCosts(IChargen chargen)
	{
		foreach (var resource in Gameworld.ChargenResources)
		{
			if (chargen.SelectedRace?.ResourceCost(resource) is int cost and > 0)
			{
				yield return (resource, cost);
			}
		}
	}

	#endregion

	#region IChargenScreenStoryboard Members

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new RacePickerScreen(chargen, this);
	}

	#endregion IChargenScreenStoryboard Members

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine("This screen allows people to select their race from a list of available races."
		              .Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine($"Skip Screen If Only One Choice: {SkipScreenIfOnlyOneChoice.ToColouredString()}");
		sb.AppendLine($"Show Unselectable Races As Blanks: {ShowUnselectableRacesAsBlanks.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(Blurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		return sb.ToString();
	}

	internal class RacePickerScreen : ChargenScreen
	{
		protected IEnumerable<Tuple<IRace, bool>> Races;
		protected IRace SelectedRace;
		protected RacePickerScreenStoryboard Storyboard;

		public override ChargenStage AssociatedStage => ChargenStage.SelectRace;

		private IEnumerable<Tuple<IRace, bool>> GetRaceValidity()
		{
			return Storyboard.Gameworld.Races.OrderBy(x => x.Name).Select(x =>
				Tuple.Create(x, x.ChargenAvailable(Chargen))).ToList();
		}

		#region IChargenScreen Members

		internal RacePickerScreen(IChargen chargen, RacePickerScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
			Races = GetRaceValidity();

			// If we only have one choice, and the skip if single choice option is taken, finish the screen and skip
			if (Races.Count(x => x.Item2) == 1 && Storyboard.SkipScreenIfOnlyOneChoice)
			{
				Chargen.SelectedRace = Races.First(x => x.Item2).Item1;
				State = ChargenScreenState.Complete;
			}
		}

		public override string Display()
		{
			if (SelectedRace == null)
			{
				var index = 1;
				return
					string.Format(
						"Race Selection".Colour(Telnet.Cyan) +
						"\n\n{0}\n\n{1}\n\nType the name or number of the race that you would like to select.",
						Storyboard.Blurb.SubstituteANSIColour()
						          .Wrap(Chargen.Account.InnerLineFormatLength),
						(Storyboard.ShowUnselectableRacesAsBlanks ? Races : Races.Where(x => x.Item2)).Select(
							x => $"{index++}: {x.Item1.Name}")
						.ArrangeStringsOntoLines((uint)Account.LineFormatLength / 30,
							(uint)Account.LineFormatLength)
					);
			}

			return
				$"{"Race:".ColourName()} {SelectedRace.Name.ColourValue()}\n\n{SelectedRace.Description.SubstituteANSIColour().Wrap(Account.InnerLineFormatLength)}\n\n{(Storyboard.Gameworld.ChargenResources.Any(x => SelectedRace.ResourceCost(x) > 0) ? $"This selection costs {Storyboard.Gameworld.ChargenResources.Where(x => SelectedRace.ResourceCost(x) > 0).Select(x => Tuple.Create(x, SelectedRace.ResourceCost(x))).Select(x => CommonStringUtilities.CultureFormat($"{x.Item2} {(x.Item2 == 1 ? x.Item1.Name.TitleCase() : x.Item1.PluralName.TitleCase())}", Account).Colour(Telnet.Green)).ListToString()}\n\n" : "")}Do you want to select this race? Type {"yes".Colour(Telnet.Yellow)} or {"no".Colour(Telnet.Yellow)}.";
		}

		public override string HandleCommand(string command)
		{
			if (string.IsNullOrEmpty(command))
			{
				return Display();
			}

			if (SelectedRace != null)
			{
				if ("yes".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
				{
					Chargen.SelectedRace = SelectedRace;
					Chargen.Handedness = SelectedRace.DefaultHandedness;
					State = ChargenScreenState.Complete;
					return "You select the " + SelectedRace.Name.Colour(Telnet.Green) + " race.\n";
				}

				SelectedRace = null;
				return Display();
			}

			if (!Storyboard.ShowUnselectableRacesAsBlanks)
			{
				Races = Races.Where(x => x.Item2);
			}

			Tuple<IRace, bool> race = null;
			race = int.TryParse(command, out var value)
				? Races.ElementAtOrDefault(value - 1)
				: Races.FirstOrDefault(
					x => x.Item1.Name.StartsWith(command, StringComparison.InvariantCultureIgnoreCase));

			if (race == null)
			{
				return
					"That is not a valid selection. Please enter the number or name of the race you want to select.";
			}

			if (!race.Item2)
			{
				return
					"You are not allowed to select that race. Please enter the number or name of the race you want to select.";
			}

			SelectedRace = race.Item1;
			return Display();
		}

		#endregion IChargenScreen Members
	}

	#region Building Commands

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "blurb":
				return BuildingCommandBlurb(actor, command);
			case "blanks":
				return BuildingCommandBlanks(actor);
			case "skipone":
				return BuildingCommandSkipOne(actor);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandSkipOne(ICharacter actor)
	{
		SkipScreenIfOnlyOneChoice = !SkipScreenIfOnlyOneChoice;
		Changed = true;
		actor.OutputHandler.Send(
			$"This screen will {(SkipScreenIfOnlyOneChoice ? "now" : "no longer")} be skipped if only one valid selection is available.");
		return true;
	}

	private bool BuildingCommandBlanks(ICharacter actor)
	{
		ShowUnselectableRacesAsBlanks = !ShowUnselectableRacesAsBlanks;
		Changed = true;
		actor.OutputHandler.Send(
			$"This screen will {(ShowUnselectableRacesAsBlanks ? "now" : "no longer")} show unselectable races as blank lines.");
		return true;
	}

	private bool BuildingCommandBlurb(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send(
			$"Replacing the following text:\n\n{Blurb.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}\n\nEnter your new blurb below:\n");
		actor.EditorMode(PostBlurb, CancelBlurb, 1.0, Blurb, EditorOptions.None,
			new object[] { actor.Account.InnerLineFormatLength });
		return true;
	}

	private void CancelBlurb(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the blurb for this chargen screen.");
	}

	private void PostBlurb(string text, IOutputHandler handler, object[] args)
	{
		Blurb = text;
		Changed = true;
		handler.Send($"You set the blurb to the following:\n\n{text.Wrap((int)args[0]).SubstituteANSIColour()}");
	}

	#endregion
}