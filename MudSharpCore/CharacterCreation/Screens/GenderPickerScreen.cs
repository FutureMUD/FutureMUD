using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Editor;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.CharacterCreation.Screens;

public class GenderPickerScreenStoryboard : ChargenScreenStoryboard
{
	private GenderPickerScreenStoryboard()
	{
	}

	public GenderPickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		Blurb = definition.Element("Blurb").Value;
	}

	protected override string StoryboardName => "GenderPicker";

	public string Blurb { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectGender;

	public override string HelpText => $@"{BaseHelpText}
	#3blurb#0 - drops you into an editor to change the blurb";

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Blurb", new XCData(Blurb))
		).ToString();
	}

	#endregion

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectGender,
			new ChargenScreenStoryboardFactory("GenderPicker",
				(game, dbitem) => new GenderPickerScreenStoryboard(game, dbitem)),
			"GenderPicker",
			"Select gender from a list",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new GenderPickerScreen(chargen, this);
	}

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine("This screen allows people to select their gender as appropriate for their race."
		              .Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine("Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(Blurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		return sb.ToString();
	}

	internal class GenderPickerScreen : ChargenScreen
	{
		protected GenderPickerScreenStoryboard Storyboard;

		internal GenderPickerScreen(IChargen chargen, GenderPickerScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectGender;

		public override string Display()
		{
			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			var number = 1;
			return
				string.Format(
					"Gender Selection".Colour(Telnet.Cyan) +
					"\n\n{0}\n\nThe following genders are available for your race:\n\n{1}\n\nType the name or number of the gender that you wish to select:",
					Storyboard.Blurb.SubstituteANSIColour().Wrap(Chargen.Account.InnerLineFormatLength),
					Chargen.SelectedRace.AllowedGenders.Select(
						       x => $"{number++}: {Gendering.Get(x).Name.Proper()}")
					       .ListToString(separator: "\n", conjunction: "\n", oxfordComma: false)
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

			var gender = int.TryParse(command, out var value)
				? Chargen.SelectedRace.AllowedGenders.ElementAtOrDefault(value - 1)
				: Chargen.SelectedRace.AllowedGenders.FirstOrDefault(
					x => Gendering.Get(x).Name.StartsWith(command, StringComparison.InvariantCultureIgnoreCase));

			if (gender == Gender.Indeterminate)
			{
				return "That is not a valid gender selection for you.";
			}

			Chargen.SelectedGender = gender;
			State = ChargenScreenState.Complete;
			return "You select the " + Gendering.Get(gender).Name.Colour(Telnet.Green) + " gender.\n";
		}
	}

	#region Building Commands

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "blurb":
				return BuildingCommandBlurb(actor, command);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
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