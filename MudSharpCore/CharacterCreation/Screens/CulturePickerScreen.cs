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

public class CulturePickerScreenStoryboard : ChargenScreenStoryboard
{
	private CulturePickerScreenStoryboard()
	{
	}

	public CulturePickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		Blurb = definition.Element("Blurb").Value;
		ShowUnselectableCulturesAsBlanks =
			Convert.ToBoolean(definition.Element("ShowUnselectableCulturesAsBlanks").Value);
		SkipScreenIfOnlyOneChoice = definition.Element("SkipScreenIfOnlyOneChoice") != null
			? bool.Parse(definition.Element("SkipScreenIfOnlyOneChoice").Value)
			: false;
	}

	protected override string StoryboardName => "CulturePicker";

	/// <summary>
	///     The text displayed above the Cultures Selection auto-generated component
	/// </summary>
	public string Blurb { get; protected set; }

	/// <summary>
	///     Controls whether Unselectable Cultures appear as blank options, or whether they are removed from the list
	/// </summary>
	public bool ShowUnselectableCulturesAsBlanks { get; protected set; }

	public bool SkipScreenIfOnlyOneChoice { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectCulture;

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Blurb", new XCData(Blurb)),
			new XElement("ShowUnselectableCulturesAsBlanks", ShowUnselectableCulturesAsBlanks),
			new XElement("SkipScreenIfOnlyOneChoice", SkipScreenIfOnlyOneChoice)
		).ToString();
	}

	#endregion

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectCulture,
			new ChargenScreenStoryboardFactory("CulturePicker",
				(game, dbitem) => new CulturePickerScreenStoryboard(game, dbitem)),
			"CulturePicker",
			"Select a culture from a list",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new CulturePickerScreen(chargen, this);
	}

	public override IEnumerable<(IChargenResource Resource, int Cost)> ChargenCosts(IChargen chargen)
	{
		foreach (var resource in Gameworld.ChargenResources)
		{
			if (chargen.SelectedCulture?.ResourceCost(resource) is int cost and > 0)
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
		sb.AppendLine("This screen allows people to select their culture from a list of available cultures."
		              .Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine($"Skip Screen If Only One Choice: {SkipScreenIfOnlyOneChoice.ToColouredString()}");
		sb.AppendLine($"Show Unselectable Cultures As Blanks: {ShowUnselectableCulturesAsBlanks.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(Blurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		return sb.ToString();
	}

	internal class CulturePickerScreen : ChargenScreen
	{
		protected IEnumerable<Tuple<ICulture, bool>> Cultures;
		protected ICulture SelectedCulture;
		protected CulturePickerScreenStoryboard Storyboard;

		internal CulturePickerScreen(IChargen chargen, CulturePickerScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
			Cultures = GetCultureValidity();

			// If we only have one choice, and the skip if single choice option is taken, finish the screen and skip
			if (Cultures.Count(x => x.Item2) == 1 && Storyboard.SkipScreenIfOnlyOneChoice)
			{
				Chargen.SelectedCulture = Cultures.First(x => x.Item2).Item1;
				State = ChargenScreenState.Complete;
			}
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectCulture;

		public override string Display()
		{
			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			if (SelectedCulture == null)
			{
				var index = 1;
				return
					$"{"Culture Selection".Colour(Telnet.Cyan)}\n\n{Storyboard.Blurb.SubstituteANSIColour().Wrap(Chargen.Account.InnerLineFormatLength)}\n\n{(Storyboard.ShowUnselectableCulturesAsBlanks ? Cultures : Cultures.Where(x => x.Item2)).Select(x => $"{index++}: {x.Item1.Name}").ArrangeStringsOntoLines((uint)Account.LineFormatLength / 30, (uint)Account.LineFormatLength)}\n\nType the number or name of the culture you would like to select.";
			}

			return
				$"{"Culture:".ColourName()} {SelectedCulture.Name.ColourValue()}\n\n{SelectedCulture.Description.SubstituteANSIColour().Wrap(Account.InnerLineFormatLength)}\n\n{(Storyboard.Gameworld.ChargenResources.Any(x => SelectedCulture.ResourceCost(x) > 0) ? $"This selection costs {Storyboard.Gameworld.ChargenResources.Where(x => SelectedCulture.ResourceCost(x) > 0).Select(x => Tuple.Create(x, SelectedCulture.ResourceCost(x))).Select(x => CommonStringUtilities.CultureFormat($"{x.Item2} {(x.Item2 == 1 ? x.Item1.Name.TitleCase() : x.Item1.PluralName.TitleCase())}", Account).Colour(Telnet.Green)).ListToString()}\n\n" : "")}Do you want to select this culture? Type {"yes".Colour(Telnet.Yellow)} or {"no".Colour(Telnet.Yellow)}.";
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

			if (SelectedCulture != null)
			{
				if ("yes".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
				{
					Chargen.SelectedCulture = SelectedCulture;
					State = ChargenScreenState.Complete;
					return "You select the " + SelectedCulture.Name.Colour(Telnet.Green) + " culture.\n";
				}

				SelectedCulture = null;
				return Display();
			}

			if (!Storyboard.ShowUnselectableCulturesAsBlanks)
			{
				Cultures = Cultures.Where(x => x.Item2);
			}

			Tuple<ICulture, bool> culture = null;
			culture = int.TryParse(command, out var value)
				? Cultures.ElementAtOrDefault(value - 1)
				: Cultures.FirstOrDefault(
					x => x.Item1.Name.StartsWith(command, StringComparison.InvariantCultureIgnoreCase));

			if (culture == null)
			{
				return
					"That is not a valid selection. Please enter the number or name of the culture you want to select.";
			}

			if (!culture.Item2)
			{
				return
					"You are not allowed to select that culture. Please enter the number or name of the culture you want to select.";
			}

			SelectedCulture = culture.Item1;
			return Display();
		}

		private IEnumerable<Tuple<ICulture, bool>> GetCultureValidity()
		{
			return Storyboard.Gameworld.Cultures.OrderBy(x => x.Name).Select(x =>
				Tuple.Create(
					x,
					x.ChargenAvailable(Chargen)
				)
			).ToList();
		}
	}

	#region Building Commands

	public override string HelpText => $@"{BaseHelpText}
	#3blurb#0 - drops you into an editor to change the blurb
	#3blanks#0 - toggles showing unselectable options as blank spots (e.g. old SOI)
	#3skipone#0 - toggles skipping the screen if there is only one valid choice";

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
		ShowUnselectableCulturesAsBlanks = !ShowUnselectableCulturesAsBlanks;
		Changed = true;
		actor.OutputHandler.Send(
			$"This screen will {(ShowUnselectableCulturesAsBlanks ? "now" : "no longer")} show unselectable cultures as blank lines.");
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