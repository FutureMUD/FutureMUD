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

public class EthnicityPickerScreenStoryboard : ChargenScreenStoryboard
{
	private EthnicityPickerScreenStoryboard()
	{
	}

	public EthnicityPickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		Blurb = definition.Element("Blurb").Value;
		SkipScreenIfOnlyOneChoice = definition.Element("SkipScreenIfOnlyOneChoice") != null
			? bool.Parse(definition.Element("SkipScreenIfOnlyOneChoice").Value)
			: false;
	}

	protected override string StoryboardName => "EthnicityPicker";

	public string Blurb { get; protected set; }

	public bool SkipScreenIfOnlyOneChoice { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectEthnicity;

	public override string HelpText => $@"{BaseHelpText}
	#3skipone#0 - toggles skipping the screen if there is only one valid choice";

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Blurb", new XCData(Blurb)),
			new XElement("SkipScreenIfOnlyOneChoice", SkipScreenIfOnlyOneChoice)
		).ToString();
	}

	#endregion

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectEthnicity,
			new ChargenScreenStoryboardFactory("EthnicityPicker",
				(game, dbitem) => new EthnicityPickerScreenStoryboard(game, dbitem)),
			"EthnicityPicker",
			"Select an ethnicity from a list",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new EthnicityPickerScreen(chargen, this);
	}

	public override IEnumerable<(IChargenResource Resource, int Cost)> ChargenCosts(IChargen chargen)
	{
		foreach (var resource in Gameworld.ChargenResources)
		{
			if (chargen.SelectedEthnicity?.ResourceCost(resource) is int cost and > 0)
			{
				yield return (resource, cost);
			}
		}
	}

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine("This screen allows people to select their ethnicity from a list of available ethnicities."
		              .Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine($"Skip Screen If Only One Choice: {SkipScreenIfOnlyOneChoice.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(Blurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		return sb.ToString();
	}

	internal class EthnicityPickerScreen : ChargenScreen
	{
		protected IEnumerable<Tuple<IEthnicity, bool>> Ethnicities;
		protected IEthnicity SelectedEthnicity;
		protected EthnicityPickerScreenStoryboard Storyboard;

		internal EthnicityPickerScreen(IChargen chargen, EthnicityPickerScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
			Ethnicities = GetEthnicityValidity();

			// If we only have one choice, and the skip if single choice option is taken, finish the screen and skip
			if (Ethnicities.Count(x => x.Item2) == 1 && Storyboard.SkipScreenIfOnlyOneChoice)
			{
				Chargen.SelectedEthnicity = Ethnicities.First(x => x.Item2).Item1;
				State = ChargenScreenState.Complete;
			}
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectEthnicity;

		public override string Display()
		{
			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			if (SelectedEthnicity != null)
			{
				return
					$"{"Ethnicity:".ColourName()} {SelectedEthnicity.Name.ColourValue()}\n\n{SelectedEthnicity.ChargenBlurb.SubstituteANSIColour().Wrap(Account.InnerLineFormatLength)}\n\n{(Storyboard.Gameworld.ChargenResources.Any(x => SelectedEthnicity.ResourceCost(x) > 0) ? $"This selection costs {Storyboard.Gameworld.ChargenResources.Where(x => SelectedEthnicity.ResourceCost(x) > 0).Select(x => Tuple.Create(x, SelectedEthnicity.ResourceCost(x))).Select(x => CommonStringUtilities.CultureFormat($"{x.Item2} {(x.Item2 == 1 ? x.Item1.Name.TitleCase() : x.Item1.PluralName.TitleCase())}", Account).Colour(Telnet.Green)).ListToString()}\n\n" : "")}Do you want to select this ethnicity? Type {"yes".Colour(Telnet.Yellow)} or {"no".Colour(Telnet.Yellow)}.";
			}

			var index = 1;
			return
				string.Format(
					"Ethnicity Selection".Colour(Telnet.Cyan) +
					"\n\n{0}\n\n{1}\nEnter the name or number of the ethnicity you would like to select.",
					Storyboard.Blurb.SubstituteANSIColour().Wrap(Account.InnerLineFormatLength),
					GetEthnicityValidity()
						.Where(x => x.Item2)
						.Select(x => $"{index++}: {x.Item1.Name}")
						.ArrangeStringsOntoLines((uint)Account.LineFormatLength / 30, (uint)Account.LineFormatLength)
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

			if (SelectedEthnicity == null)
			{
				SelectedEthnicity = int.TryParse(command, out var value)
					? GetEthnicityValidity().Where(x => x.Item2).Select(x => x.Item1).ElementAtOrDefault(value - 1)
					: GetEthnicityValidity()
					  .Where(x => x.Item2)
					  .Select(x => x.Item1)
					  .FirstOrDefault(x => x.Name.StartsWith(command, StringComparison.InvariantCultureIgnoreCase));

				return SelectedEthnicity == null
					? "That is not a valid selection. Please enter the number or name of the ethnicity you would like to select."
					: Display();
			}

			if ("yes".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				Chargen.SelectedEthnicity = SelectedEthnicity;
				State = ChargenScreenState.Complete;
				return "You select the " + SelectedEthnicity.Name.Colour(Telnet.Green) + " ethnicity.\n";
			}

			SelectedEthnicity = null;
			return Display();
		}

		private IEnumerable<Tuple<IEthnicity, bool>> GetEthnicityValidity()
		{
			return Storyboard.Gameworld.Ethnicities
			                 .Where(x => x.ParentRace == Chargen.SelectedRace)
			                 .OrderBy(x => x.Name).Select(x =>
				                 Tuple.Create(
					                 x,
					                 x.ChargenAvailable(Chargen)
				                 )
			                 ).ToList();
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