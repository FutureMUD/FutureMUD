using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate.Date;

namespace MudSharp.CharacterCreation.Screens;

public class AgePickerScreenStoryboard : ChargenScreenStoryboard
{
	private AgePickerScreenStoryboard()
	{
	}

	public AgePickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		AgeSelectionBlurb = definition.Element("AgeSelectionBlurb").Value;
		DateSelectionBlurb = definition.Element("DateSelectionBlurb").Value;
		MinimumAgeProg = long.TryParse(definition.Element("MinimumAgeProg").Value, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.FirstOrDefault(
				x =>
					x.FunctionName.Equals(definition.Element("MinimumAgeProg").Value,
						StringComparison.InvariantCultureIgnoreCase));
		MaximumAgeProg = long.TryParse(definition.Element("MaximumAgeProg").Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.FirstOrDefault(
				x =>
					x.FunctionName.Equals(definition.Element("MaximumAgeProg").Value,
						StringComparison.InvariantCultureIgnoreCase));
	}

	protected override string StoryboardName => "AgePicker";

	public string AgeSelectionBlurb { get; protected set; }

	public string DateSelectionBlurb { get; protected set; }

	public IFutureProg MaximumAgeProg { get; protected set; }

	public IFutureProg MinimumAgeProg { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectBirthday;

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("AgeSelectionBlurb", new XCData(AgeSelectionBlurb)),
			new XElement("DateSelectionBlurb", new XCData(DateSelectionBlurb)),
			new XElement("MinimumAgeProg", MinimumAgeProg?.Id ?? 0),
			new XElement("MaximumAgeProg", MaximumAgeProg?.Id ?? 0)
		).ToString();
	}

	#endregion

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectBirthday,
			new ChargenScreenStoryboardFactory("BirthdayPicker",
				(game, dbitem) => new AgePickerScreenStoryboard(game, dbitem)),
			"BirthdayPicker",
			"Specify an age and birthday",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new AgePickerScreen(chargen, this);
	}

	internal class AgePickerScreen : ChargenScreen
	{
		protected int SelectedAge;
		protected MudDate SelectedDate;
		protected AgePickerScreenStoryboard Storyboard;

		internal AgePickerScreen(IChargen chargen, AgePickerScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectBirthday;

		public override string Display()
		{
			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			if (SelectedAge == 0)
			{
				return string.Format(
					"Age Selection".Colour(Telnet.Cyan) +
					"\n\n{0}\n\nSelect an age for your character between {1:N0} and {2:N0}.",
					Storyboard.AgeSelectionBlurb.SubstituteANSIColour().Wrap(Chargen.Account.InnerLineFormatLength),
					Convert.ToDouble(Storyboard.MinimumAgeProg.Execute(Chargen)),
					Convert.ToDouble(Storyboard.MaximumAgeProg.Execute(Chargen))
				);
			}

			return string.Format(
				"Birthday Selection".Colour(Telnet.Cyan) +
				"\n\n{0}\n\nEnter the day and month on which your character was born, or {1} to select one at random.",
				Storyboard.DateSelectionBlurb.SubstituteANSIColour().Wrap(Chargen.Account.InnerLineFormatLength),
				"random".Colour(Telnet.Yellow)
			);
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

			if (SelectedAge == 0)
			{
				if (!int.TryParse(command, out var value))
				{
					return "You must enter your age as a number.";
				}

				var minimumValue = Convert.ToInt32(Storyboard.MinimumAgeProg.Execute(Chargen));
				var maximumValue = Convert.ToInt32(Storyboard.MaximumAgeProg.Execute(Chargen));
				if (value < minimumValue || value > maximumValue)
				{
					return $"You must select an age between {minimumValue} and {maximumValue}.";
				}

				SelectedAge = value;
				return Display();
			}

			if (command.Equals("random", StringComparison.InvariantCultureIgnoreCase))
			{
				Chargen.SelectedBirthday = Chargen.SelectedCulture.PrimaryCalendar.GetRandomBirthday(SelectedAge);
				State = ChargenScreenState.Complete;
				return "Your character was born on " +
				       Chargen.SelectedBirthday.Calendar.DisplayDate(Chargen.SelectedBirthday,
					       CalendarDisplayMode.Long) + ".\n";
			}

			int day;
			string month;
			var ss = new StringStack(command);
			var cmd = ss.Pop();
			if (cmd.GetIntFromOrdinal() != null)
			{
				day = cmd.GetIntFromOrdinal() ?? 0;
				month = ss.Pop();
			}
			else
			{
				month = cmd;
				if (ss.Pop().GetIntFromOrdinal() == null)
				{
					return "You must enter the day and month on which your character was born.";
				}

				day = ss.Last.GetIntFromOrdinal() ?? 0;
			}

			string errorMessage = null;
			try
			{
				SelectedDate = Chargen.SelectedCulture.PrimaryCalendar.GetBirthday(day, month.ToLowerInvariant(),
					SelectedAge);
			}
			catch (MUDDateException e)
			{
				errorMessage = e.Message;
			}

			if (!string.IsNullOrEmpty(errorMessage))
			{
				return errorMessage;
			}

			Chargen.SelectedBirthday = SelectedDate;
			State = ChargenScreenState.Complete;
			return "Your character was born on " +
			       SelectedDate.Calendar.DisplayDate(SelectedDate, CalendarDisplayMode.Long) + ".\n";
		}
	}

	#region Building Commands

	public override string HelpText => $@"{BaseHelpText}
	#3age#0 - drops into an editor to edit the age blurb
	#3date#0 - drops into an editor to edit the date blurb
	#3maxage <prog>#0 - sets the prog that controls the maximum age
	#3minage <prog>#0 - sets the prog that controls the minimum age";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "ageblurb":
			case "age":
				return BuildingCommandAgeBlurb(actor, command);
			case "date":
			case "dateblurb":
				return BuildingCommandDateBlurb(actor, command);
			case "min":
			case "minage":
			case "minimum":
			case "minimumage":
				return BuildingCommandMinimumAge(actor, command);
			case "max":
			case "maxage":
			case "maximum":
			case "maximumage":
				return BuildingCommandMaximumAge(actor, command);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandMaximumAge(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control the maximum age for character creation?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Number, new List<ProgVariableTypes>
			{
				ProgVariableTypes.Chargen
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		MaximumAgeProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {prog.MXPClickableFunctionName()} prog will now be used to control the maximum age for character creation.");
		return true;
	}

	private bool BuildingCommandMinimumAge(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control the minimum age for character creation?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Number, new List<ProgVariableTypes>
			{
				ProgVariableTypes.Chargen
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		MinimumAgeProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {prog.MXPClickableFunctionName()} prog will now be used to control the minimum age for character creation.");
		return true;
	}

	private bool BuildingCommandAgeBlurb(ICharacter actor, StringStack command)
	{
		actor.EditorMode(PostAgeBlurb, CancelAgeBlurb, 1.0, AgeSelectionBlurb, EditorOptions.None,
			new object[] { actor.Account.InnerLineFormatLength });
		return true;
	}

	private void CancelAgeBlurb(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the age blurb for this chargen screen.");
	}

	private void PostAgeBlurb(string text, IOutputHandler handler, object[] args)
	{
		AgeSelectionBlurb = text;
		Changed = true;
		handler.Send($"You set the age blurb to the following:\n\n{text.Wrap((int)args[0]).SubstituteANSIColour()}");
	}

	private bool BuildingCommandDateBlurb(ICharacter actor, StringStack command)
	{
		actor.EditorMode(PostDateBlurb, CancelDateBlurb, 1.0, DateSelectionBlurb, EditorOptions.None,
			new object[] { actor.Account.InnerLineFormatLength });
		return true;
	}

	private void CancelDateBlurb(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the date blurb for this chargen screen.");
	}

	private void PostDateBlurb(string text, IOutputHandler handler, object[] args)
	{
		DateSelectionBlurb = text;
		Changed = true;
		handler.Send($"You set the date blurb to the following:\n\n{text.Wrap((int)args[0]).SubstituteANSIColour()}");
	}

	#endregion
}