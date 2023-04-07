using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.CharacterCreation.Screens;

public class HandednessPickerScreenStoryboard : ChargenScreenStoryboard
{
	private HandednessPickerScreenStoryboard()
	{
	}

	protected HandednessPickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem) : base(
		dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		Blurb = definition.Element("Blurb")?.Value;
		SkipScreen = definition.Element("SkipScreen") != null
			? bool.Parse(definition.Element("SkipScreen").Value)
			: false;
	}

	protected override string StoryboardName => "HandednessPicker";

	public string Blurb { get; set; }

	public bool SkipScreen { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectHandedness;

	public override string HelpText => $@"{BaseHelpText}
	#3blurb#0 - drops you into an editor to change the blurb
	#3skip0 - toggles skipping this screen entirely";

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Blurb", new XCData(Blurb)),
			new XElement("SkipScreen", SkipScreen)
		).ToString();
	}

	#endregion

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectHandedness,
			new ChargenScreenStoryboardFactory("HandednessPicker",
				(game, dbitem) => new HandednessPickerScreenStoryboard(game, dbitem)),
			"HandednessPicker",
			"Select handedness from a list",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new HandednessPickerScreen(chargen, this);
	}

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine("This screen allows people to select their handedness (left, right, etc)."
		              .Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine($"Disable Selection and Skip Screen: {SkipScreen.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(Blurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		return sb.ToString();
	}

	internal class HandednessPickerScreen : ChargenScreen
	{
		protected HandednessPickerScreenStoryboard Storyboard;

		internal HandednessPickerScreen(IChargen chargen, HandednessPickerScreenStoryboard storyboard) : base(chargen,
			storyboard)
		{
			Storyboard = storyboard;
			Chargen.Handedness = Alignment.Irrelevant;

			// If we don't care about handedness, just pick the first one from the race's allowable handedness.
			if (Storyboard.SkipScreen)
			{
				Chargen.Handedness = Chargen.SelectedRace.HandednessOptions.First();
				State = ChargenScreenState.Complete;
			}
		}

		public override ChargenStage AssociatedStage => Storyboard.Stage;

		public override string HandleCommand(string command)
		{
			if (string.IsNullOrEmpty(command))
			{
				return Display();
			}

			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return HandleCommandChargenAdvice(command);
			}

			var option = int.TryParse(command, out var value)
				? Chargen.SelectedRace.HandednessOptions.ElementAtOrDefault(value - 1)
				: Chargen.SelectedRace.HandednessOptions.FirstOrDefault(
					x => x.Describe().StartsWith(command, StringComparison.InvariantCultureIgnoreCase));

			if (option == Alignment.Irrelevant)
			{
				return "That is not a valid handedness selection for you.";
			}

			Chargen.Handedness = option;
			State = ChargenScreenState.Complete;
			return $"You select the {option.Describe().Colour(Telnet.Green)} handedness option for your character.";
		}

		public override string Display()
		{
			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			var number = 1;
			return string.Format(Chargen.Account,
				"{0}\n\n{1}\n\nThe following handedness options are available to you:\n\n{2}\n\nType the name or number of the handedness you want to select.",
				"Handedness Selection".Colour(Telnet.Cyan),
				Storyboard.Blurb.SubstituteANSIColour().Wrap(Chargen.Account.InnerLineFormatLength),
				Chargen.SelectedRace.HandednessOptions.Select(x => $"{number++}: {x.Describe()}")
				       .ListToString(separator: "\n", conjunction: "\n", oxfordComma: false)
			);
		}
	}

	#region Building Commands

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "blurb":
				return BuildingCommandBlurb(actor, command);
			case "skip":
				return BuildingCommandSkip(actor);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandSkip(ICharacter actor)
	{
		SkipScreen = !SkipScreen;
		Changed = true;
		actor.OutputHandler.Send(
			$"This screen will {(SkipScreen ? "now be entirely skipped" : "no longer be skipped, and players will choose handedness again")}.");
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