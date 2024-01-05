using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Editor;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.CharacterCreation.Screens;

public class DescriptionPickerScreenStoryboard : ChargenScreenStoryboard
{
	private DescriptionPickerScreenStoryboard()
	{
	}

	public DescriptionPickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		AllowCustomDescription = Convert.ToBoolean(definition.Element("AllowCustomDescription").Value);
		AllowEntityDescriptionPatterns = Convert.ToBoolean(definition.Element("AllowEntityDescriptionPatterns").Value);
		FullDescBlurb = definition.Element("FullDescBlurb").Value;
		SDescBlurb = definition.Element("SDescBlurb").Value;
	}

	protected override string StoryboardName => "DescriptionPicker";

	public bool AllowCustomDescription { get; protected set; }

	public bool AllowEntityDescriptionPatterns { get; protected set; }

	public string FullDescBlurb { get; protected set; }

	public string SDescBlurb { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectDescription;

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("SDescBlurb", new XCData(SDescBlurb)),
			new XElement("FullDescBlurb", new XCData(FullDescBlurb)),
			new XElement("AllowEntityDescriptionPatterns", AllowEntityDescriptionPatterns),
			new XElement("AllowCustomDescription", AllowCustomDescription)
		).ToString();
	}

	#endregion

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectDescription,
			new ChargenScreenStoryboardFactory("DescriptionPicker",
				(game, dbitem) => new DescriptionPickerScreenStoryboard(game, dbitem)),
			"DescriptionPicker",
			"Pick a short and full description from a list",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine(
			"This screen is where players choose the short description and full description for their character."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine($"Allow Patterns: {AllowEntityDescriptionPatterns.ToColouredString()}");
		sb.AppendLine($"Allow Custom Descriptions: {AllowCustomDescription.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Short Description Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(SDescBlurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		sb.AppendLine();
		sb.AppendLine("Full Description Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(FullDescBlurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		return sb.ToString();
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new DescriptionPickerScreen(chargen, this);
	}

	internal class DescriptionPickerScreen : ChargenScreen
	{
		private readonly List<IEntityDescriptionPattern> SelectedEntityDescriptionPatterns =
			new();

		private readonly DescriptionPickerScreenStoryboard Storyboard;
		private readonly IList<IEntityDescriptionPattern> ValidPatterns;
		private bool InCustomMode;
		private List<IEntityDescriptionPattern> RandomPatterns;
		private string SelectedDesc;
		private IEntityDescriptionPattern SelectedEntityDescriptionPattern;
		private string SelectedSdesc;

		internal DescriptionPickerScreen(IChargen chargen, DescriptionPickerScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
			if (!storyboard.AllowEntityDescriptionPatterns)
			{
				InCustomMode = true;
			}

			ValidPatterns =
				Storyboard.Gameworld.EntityDescriptionPatterns.Where(
					x => x.IsValidSelection(chargen)).ToList();
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectDescription;

		public override string Display()
		{
			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			if (string.IsNullOrEmpty(SelectedSdesc))
			{
				if (InCustomMode)
				{
					return "Enter the custom short description that you would like to use.";
				}

				if (SelectedEntityDescriptionPattern == null)
				{
					var index = 1;
					RandomPatterns =
						ValidPatterns.Where(x => x.Type == EntityDescriptionType.ShortDescription).ToList();
					if (RandomPatterns.Count > 20)
					{
						RandomPatterns = RandomPatterns.PickRandom(20).ToList();
					}

					return
						string.Format(
							"Short Description Selection".Colour(Telnet.Cyan) +
							"\n\n{0}\n\n{1}\n\n{2}Enter the number of the short description you'd like to use{3}.",
							Storyboard.SDescBlurb.SubstituteANSIColour()
							          .Wrap(Chargen.Account.InnerLineFormatLength),
							RandomPatterns.Select(
								              x =>
									              $"{index++}: {IHaveCharacteristicsExtensions.ParseCharacteristicsAbsolute(x.Pattern, Chargen.SelectedCharacteristics, Gendering.Get(Chargen.SelectedGender), Chargen.Gameworld, Chargen.SelectedRace, Chargen.SelectedCulture, Chargen.SelectedEthnicity, Chargen.SelectedBirthday?.Calendar.CurrentDate.YearsDifference(Chargen.SelectedBirthday ?? Chargen.SelectedCulture?.PrimaryCalendar.CurrentDate) ?? 0, Chargen.SelectedHeight).Colour(Telnet.Magenta)}")
							              .ArrangeStringsOntoLines(1, (uint)Account.LineFormatLength),
							ValidPatterns.Count(x => x.Type == EntityDescriptionType.ShortDescription) > 20
								? $"This is a random sample of 20 of the {ValidPatterns.Count(x => x.Type == EntityDescriptionType.ShortDescription)} available patterns. Type {"shuffle".Colour(Telnet.Yellow)} to view another random sample.\n"
								: "",
							Storyboard.AllowCustomDescription
								? ", or 0 to enter your own custom description."
								: ""
						);
				}

				return
					$"You have selected the following pattern:\n\n{IHaveCharacteristicsExtensions.ParseCharacteristicsAbsolute(SelectedEntityDescriptionPattern.Pattern, Chargen.SelectedCharacteristics, Gendering.Get(Chargen.SelectedGender), Chargen.Gameworld, Chargen.SelectedRace, Chargen.SelectedCulture, Chargen.SelectedEthnicity, Chargen.SelectedBirthday?.Calendar.CurrentDate.YearsDifference(Chargen.SelectedBirthday ?? Chargen.SelectedCulture?.PrimaryCalendar.CurrentDate) ?? 0, Chargen.SelectedHeight).Colour(Telnet.Magenta).Wrap(Chargen.Account.InnerLineFormatLength)}\n\nType {"yes".Colour(Telnet.Yellow)} to proceed or {"no".Colour(Telnet.Yellow)} to select again.";
			}

			if (InCustomMode)
			{
				return "Enter the custom description that you would like to use.";
			}

			if (SelectedEntityDescriptionPattern == null)
			{
				var index = 1;
				RandomPatterns = ValidPatterns.Where(x => x.Type == EntityDescriptionType.FullDescription).ToList();
				if (RandomPatterns.Count > 4)
				{
					RandomPatterns = RandomPatterns.PickRandom(4).ToList();
				}

				return
					string.Format(
						"Full Description Selection".Colour(Telnet.Cyan) +
						"\n\n{0}\n\n{1}\n\n{2}Enter the number of the description you'd like to use{3}.",
						Storyboard.FullDescBlurb.SubstituteANSIColour(),
						RandomPatterns.Select(
							              x =>
								              $"{index++}:\n{IHaveCharacteristicsExtensions.ParseCharacteristicsAbsolute(x.Pattern, Chargen.SelectedCharacteristics, Gendering.Get(Chargen.SelectedGender), Chargen.Gameworld, Chargen.SelectedRace, Chargen.SelectedCulture, Chargen.SelectedEthnicity, Chargen.SelectedBirthday?.Calendar.CurrentDate.YearsDifference(Chargen.SelectedBirthday ?? Chargen.SelectedCulture?.PrimaryCalendar.CurrentDate) ?? 0, Chargen.SelectedHeight).ProperSentences().Wrap(Account.InnerLineFormatLength, "\t").NoWrap()}")
						              .ListToString(separator: "\n\n", conjunction: "\n\n", oxfordComma: false),
						ValidPatterns.Count(x => x.Type == EntityDescriptionType.FullDescription) > 4
							? $"This is a random sample of 4 of the {ValidPatterns.Count(x => x.Type == EntityDescriptionType.FullDescription)} available patterns. Type {"shuffle".Colour(Telnet.Yellow)} to view another random sample.\n"
							: "",
						Storyboard.AllowCustomDescription
							? ", or 0 to enter your own custom description."
							: ""
					);
			}

			return
				$"You have selected the following pattern:\n\n{IHaveCharacteristicsExtensions.ParseCharacteristicsAbsolute(SelectedEntityDescriptionPattern.Pattern, Chargen.SelectedCharacteristics, Gendering.Get(Chargen.SelectedGender), Chargen.Gameworld, Chargen.SelectedRace, Chargen.SelectedCulture, Chargen.SelectedEthnicity, Chargen.SelectedBirthday?.Calendar.CurrentDate.YearsDifference(Chargen.SelectedBirthday ?? Chargen.SelectedCulture?.PrimaryCalendar.CurrentDate) ?? 0, Chargen.SelectedHeight).ProperSentences().Wrap(80, "\t").NoWrap()}\n\nType {"yes".Colour(Telnet.Yellow)} to proceed or {"no".Colour(Telnet.Yellow)} to pick again.";
		}

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

			if (string.IsNullOrEmpty(SelectedSdesc))
			{
				if (InCustomMode)
				{
					if (command.Length == 0)
					{
						return "You must enter a short description";
					}

					SelectedSdesc = command.ToLowerInvariant().Trim();
					InCustomMode = false;
					return Display();
				}

				if (SelectedEntityDescriptionPattern == null)
				{
					if (int.TryParse(command, out var value))
					{
						if (value == 0 && Storyboard.AllowCustomDescription)
						{
							InCustomMode = true;
							return Display();
						}

						SelectedEntityDescriptionPattern = RandomPatterns.ElementAtOrDefault(value - 1);
					}
					else
					{
						return command.ToLowerInvariant() == "shuffle"
							? Display()
							: "You must enter the number of the pattern that you wish to select.";
					}

					return SelectedEntityDescriptionPattern == null ? "That is not a valid selection." : Display();
				}

				if ("yes".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
				{
					SelectedSdesc = SelectedEntityDescriptionPattern.Pattern;
					SelectedEntityDescriptionPatterns.Add(SelectedEntityDescriptionPattern);
				}

				SelectedEntityDescriptionPattern = null;
				return Display();
			}

			if (SelectedEntityDescriptionPattern == null)
			{
				if (int.TryParse(command, out var value))
				{
					if (value == 0 && Storyboard.AllowCustomDescription)
					{
						Chargen.SetEditor(new EditorController(Chargen.Menu, null, PostCustomDescription,
							CancelCustomDescription, EditorOptions.None));
					}

					SelectedEntityDescriptionPattern = RandomPatterns.ElementAtOrDefault(value - 1);
				}
				else
				{
					return command.ToLowerInvariant() == "shuffle"
						? Display()
						: "You must enter the number of the pattern that you wish to select.";
				}

				return SelectedEntityDescriptionPattern == null ? "That is not a valid selection." : Display();
			}

			if ("yes".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				SelectedDesc = SelectedEntityDescriptionPattern.Pattern;
				SelectedEntityDescriptionPatterns.Add(SelectedEntityDescriptionPattern);
				FinaliseScreen();
				return "\n";
			}

			SelectedEntityDescriptionPattern = null;
			return Display();
		}

		private void CancelCustomDescription(IOutputHandler outputHandler, object[] arguments)
		{
			outputHandler.Send("You decide not to enter a custom description.");
		}

		private void FinaliseScreen()
		{
			Chargen.SelectedSdesc = SelectedSdesc;
			Chargen.SelectedFullDesc = SelectedDesc;
			Chargen.SelectedEntityDescriptionPatterns = SelectedEntityDescriptionPatterns;
			State = ChargenScreenState.Complete;
		}

		private void PostCustomDescription(string description, IOutputHandler outputHandler, object[] arguments)
		{
			SelectedDesc = description;
			outputHandler.Send("You set your description to:\n" + SelectedDesc);
		}
	}

	#region Building Commands

	public override string HelpText => $@"{BaseHelpText}
	#3sdesc#0 - drops into an editor for the sdesc blurb
	#3desc#0 - drops into an editor for the full desc blurb
	#3custom#0 - toggles custom descriptions
	#3patterns#0 - toggles selection of patterns";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "sdescblurb":
			case "sdesc":
				return BuildingCommandBlurb(actor, command);
			case "descblurb":
			case "fullblurb":
			case "fdescblurb":
			case "fdesc":
			case "desc":
			case "fulldesc":
				return BuildingCommandFDescBlurb(actor, command);
			case "custom":
				return BuildingCommandCustom(actor);
			case "pattern":
			case "patterns":
				return BuildingCommandPatterns(actor);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandPatterns(ICharacter actor)
	{
		AllowEntityDescriptionPatterns = !AllowEntityDescriptionPatterns;
		if (!AllowEntityDescriptionPatterns)
		{
			AllowCustomDescription = true;
		}

		Changed = true;
		actor.OutputHandler.Send(
			$"Players will {(AllowEntityDescriptionPatterns ? "now" : "no longer")} be allowed to enter their own custom descriptions.");
		return true;
	}

	private bool BuildingCommandCustom(ICharacter actor)
	{
		AllowCustomDescription = !AllowCustomDescription;
		if (!AllowCustomDescription)
		{
			AllowEntityDescriptionPatterns = true;
		}

		Changed = true;
		actor.OutputHandler.Send(
			$"Players will {(AllowEntityDescriptionPatterns ? "now" : "no longer")} be allowed to pick from a selection of description patterns.");
		return true;
	}

	private bool BuildingCommandFDescBlurb(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send(
			$"You are replacing the following existing text:\n\n{FullDescBlurb.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}\nEnter the new blurb below:\n");
		actor.EditorMode(PostFDescBlurb, CancelFDescBlurb, 1.0, FullDescBlurb, EditorOptions.None,
			new object[] { actor.Account.InnerLineFormatLength });
		return true;
	}


	private void CancelFDescBlurb(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the full description blurb for this chargen screen.");
	}

	private void PostFDescBlurb(string text, IOutputHandler handler, object[] args)
	{
		FullDescBlurb = text;
		Changed = true;
		handler.Send(
			$"You set the full description blurb to the following:\n\n{text.Wrap((int)args[0]).SubstituteANSIColour()}");
	}

	private bool BuildingCommandBlurb(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send(
			$"You are replacing the following existing text:\n\n{SDescBlurb.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}\nEnter the new blurb below:\n");
		actor.EditorMode(PostSDescBlurb, CancelSDescBlurb, 1.0, SDescBlurb, EditorOptions.None,
			new object[] { actor.Account.InnerLineFormatLength });
		return true;
	}

	private void CancelSDescBlurb(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the short description blurb for this chargen screen.");
	}

	private void PostSDescBlurb(string text, IOutputHandler handler, object[] args)
	{
		SDescBlurb = text;
		Changed = true;
		handler.Send(
			$"You set the short description blurb to the following:\n\n{text.Wrap((int)args[0]).SubstituteANSIColour()}");
	}

	#endregion
}