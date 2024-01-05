using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Communication.Language;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp.CharacterCreation.Screens;

public class AccentPickerScreenStoryboard : ChargenScreenStoryboard
{
	private AccentPickerScreenStoryboard()
	{
	}

	public AccentPickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		Blurb = definition.Element("Blurb").Value;
		var element = definition.Element("NumberOfPicks");
		NumberOfAccentsPerLanguage = element != null ? int.Parse(element.Value) : 1;
		element = definition.Element("AdditionalPicks");
		if (element != null)
		{
			AdditionalPickResource = Gameworld.ChargenResources.Get(long.Parse(element.Attribute("resource").Value));
			AdditionalPickCost = int.Parse(element.Attribute("cost").Value);
		}
	}

	protected override string StoryboardName => "AccentPicker";

	public string Blurb { get; protected set; }
	public int NumberOfAccentsPerLanguage { get; protected set; }

	public IChargenResource AdditionalPickResource { get; protected set; }
	public int AdditionalPickCost { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectAccents;

	public override string HelpText => $@"{BaseHelpText}
	#3picks <#>#0 - sets the number of picks per language
	#3extras none#0 - disables selecting extra picks
	#3extras <amount> <resource>#0 - sets the price for extra picks";

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		var definition = new XElement("Definition",
			new XElement("Blurb", new XCData(Blurb)),
			new XElement("NumberOfPicks", NumberOfAccentsPerLanguage)
		);

		if (AdditionalPickResource is not null)
		{
			definition.Add(new XElement("AdditionalPicks",
				new XAttribute("resource", AdditionalPickResource.Id),
				new XAttribute("cost", AdditionalPickCost)
			));
		}

		return definition.ToString();
	}

	#endregion

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine(
			"This screen allows a player to select their native accents or dialects of the languages that they have."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine($"# Accents per Language: {NumberOfAccentsPerLanguage.ToString("N0", voyeur).ColourValue()}");
		sb.AppendLine(
			$"Additional Pick Resource: {AdditionalPickResource?.PluralName.ColourValue() ?? "None".ColourError()}");
		sb.AppendLine($"Additional Pick Cost: {AdditionalPickCost.ToString("N0", voyeur).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(Blurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		return sb.ToString();
	}

	public override IEnumerable<(IChargenResource Resource, int Cost)> ChargenCosts(IChargen chargen)
	{
		if (AdditionalPickResource == null || AdditionalPickCost <= 0)
		{
			return base.ChargenCosts(chargen);
		}

		return new[]
		{
			(AdditionalPickResource,
				chargen.SelectedAccents.GroupBy(x => x.Language)
				       .Sum(x => Math.Max(0, x.Count() - NumberOfAccentsPerLanguage)))
		};
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new AccentPickerScreen(chargen, this);
	}

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectAccents,
			new ChargenScreenStoryboardFactory("AccentPicker",
				(game, dbitem) => new AccentPickerScreenStoryboard(game, dbitem)),
			"AccentPicker",
			"Pick # of accents per language",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	internal class AccentPickerScreen : ChargenScreen
	{
		protected bool GroupedAccentMode;
		protected IEnumerator<ILanguage> LanguageEnumerator;
		protected List<ILanguage> Languages;
		protected bool ShownIntroduction;
		protected AccentPickerScreenStoryboard Storyboard;

		internal AccentPickerScreen(IChargen chargen, AccentPickerScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
			DoInitialAccentSetup();
			if (State == ChargenScreenState.Complete)
			{
				return;
			}

			LanguageEnumerator = Languages.GetEnumerator();
			LanguageEnumerator.MoveNext();
			GroupedAccentMode = CurrentSelections.Count() > 15;
		}

		protected IEnumerable<IAccent> CurrentSelections
		{
			get
			{
				return
					Storyboard.Gameworld.Accents
					          .Where(x => x.Language == LanguageEnumerator.Current && x.IsAvailableInChargen(Chargen))
					          .OrderBy(x => x.Name)
					          .ToList();
			}
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectAccents;

		protected void DoInitialAccentSetup()
		{
			Languages = Chargen.SelectedSkills
			                   .SelectNotNull(x =>
				                   Storyboard.Gameworld.Languages.FirstOrDefault(y => y.LinkedTrait == x))
			                   .ToList();
			Chargen.SelectedAccents.Clear();
			foreach (var language in Languages.ToList())
			{
				var accents = Storyboard.Gameworld.Accents
				                        .Where(x => x.Language == language && x.IsAvailableInChargen(Chargen)).ToList();
				if (accents.Count < 1)
				{
					var accent = Storyboard.Gameworld.Accents.Where(x => x.Language == language).GetRandomElement();
					if (accent is null)
					{
						Languages.Remove(language);
						continue;
					}

					Chargen.SelectedAccents.Add(accent);
					continue;
				}

				if (accents.Count == 1)
				{
					Chargen.SelectedAccents.Add(accents.First());
					Languages.Remove(language);
				}
			}

			if (!Languages.Any())
			{
				State = ChargenScreenState.Complete;
			}
		}

		public override string Display()
		{
			if (!ShownIntroduction)
			{
				return
					$@"{"Accent Selection".Colour(Telnet.Cyan)}

{Storyboard.Blurb.SubstituteANSIColour().Wrap(Chargen.Account.InnerLineFormatLength)}

Type {"continue".Colour(Telnet.Yellow)} to begin, or {"reset".Colour(Telnet.Yellow)} at any time to return to this screen.";
			}

			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			if (GroupedAccentMode)
			{
				return DisplayGroupedMode();
			}

			var choice = 1;
			return
				$@"Accents for {LanguageEnumerator.Current.Name.ColourValue()}

{CurrentSelections.Select(x => $"{choice++}: {(Chargen.SelectedAccents.Contains(x) ? x.Name.TitleCase().Colour(Telnet.Green).Parentheses() : x.Name.TitleCase())}").ArrangeStringsOntoLines((uint)Account.LineFormatLength / 40, (uint)Account.LineFormatLength)}

{GetSelectionFooter()}";
		}

		public IAccent GetAccentForSelection(string selection)
		{
			var orderedSelections = GroupedAccentMode
				? CurrentSelections.GroupBy(x => x.Group).SelectMany(x => x).ToList()
				: CurrentSelections.ToList();
			if (int.TryParse(selection, out var value))
			{
				return orderedSelections.ElementAtOrDefault(value - 1);
			}

			return orderedSelections.FirstOrDefault(x => x.Name.EqualTo(selection)) ??
			       orderedSelections.FirstOrDefault(x =>
				       x.Name.StartsWith(selection, StringComparison.InvariantCultureIgnoreCase)) ??
			       orderedSelections.FirstOrDefault(x => x.Name.Contains(selection, StringComparison.InvariantCulture));
		}

		public string GetSelectionFooter()
		{
			if (Storyboard.AdditionalPickResource != null)
			{
				return
					$"You are allowed {Storyboard.NumberOfAccentsPerLanguage} free picks. Additional picks cost {$"{Storyboard.AdditionalPickCost} {Storyboard.AdditionalPickResource.Alias}".Colour(Telnet.Green)} each.\nYou can type {"help <accent>".Colour(Telnet.Yellow)} to view info on an accent.\nEnter the name or number of the accent you would like to select, and {"done".Colour(Telnet.Yellow)} to finish.";
			}

			return
				$"You are allowed {Storyboard.NumberOfAccentsPerLanguage} total picks. You can type {"help <accent>".Colour(Telnet.Yellow)} to view info on an accent.\nEnter the name or number of the accent you would like to select, and {"done".Colour(Telnet.Yellow)} to finish.";
		}

		public string DisplayGroupedMode()
		{
			var sb = new StringBuilder();
			sb.AppendLine($"Accents for {LanguageEnumerator.Current.Name}");
			sb.AppendLine();
			var choice = 1;
			var groupedSelections = CurrentSelections.GroupBy(x => x.Group);
			foreach (var group in groupedSelections)
			{
				sb.AppendLine();
				sb.AppendLine(group.Key.TitleCase().GetLineWithTitle(Account.LineFormatLength, Account.UseUnicode,
					Telnet.Cyan, Telnet.BoldYellow));
				sb.AppendLine();
				sb.Append(group
				          .Select(x =>
					          $"{choice++}: {(Chargen.SelectedAccents.Contains(x) ? x.Name.TitleCase().Colour(Telnet.Green).Parentheses() : x.Name.TitleCase())}")
				          .ArrangeStringsOntoLines((uint)Account.LineFormatLength / 40,
					          (uint)Account.LineFormatLength));
			}

			sb.AppendLine();
			sb.AppendLine(GetSelectionFooter());

			return sb.ToString();
		}

		public override string HandleCommand(string command)
		{
			if (string.IsNullOrEmpty(command))
			{
				return Display();
			}

			if (!ShownIntroduction)
			{
				if ("continue".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
				{
					ShownIntroduction = true;
					return Display();
				}

				return "Type " + "continue".Colour(Telnet.Yellow) + " to begin.";
			}

			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return HandleCommandChargenAdvice(command);
			}

			if ("reset".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				ShownIntroduction = false;
				DoInitialAccentSetup();
				if (State == ChargenScreenState.Complete)
				{
					return "";
				}

				LanguageEnumerator = Languages.GetEnumerator();
				LanguageEnumerator.MoveNext();
				return Display();
			}

			if ("done".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				if (Chargen.SelectedAccents.Any(x => x.Language == LanguageEnumerator.Current))
				{
					if (!LanguageEnumerator.MoveNext())
					{
						State = ChargenScreenState.Complete;
						return "";
					}

					GroupedAccentMode = CurrentSelections.Count() > 15;
					return Display();
				}

				return "You must select at least 1 accent for each language.";
			}

			if (command.StartsWith("help", StringComparison.InvariantCultureIgnoreCase) ||
			    command.StartsWith("info", StringComparison.InvariantCultureIgnoreCase) ||
			    command.StartsWith("show", StringComparison.InvariantCultureIgnoreCase))
			{
				var ss = new StringStack(command.RemoveFirstWord());
				if (ss.IsFinished)
				{
					return "What accent did you want to view help for?";
				}

				var hAccent =
					CurrentSelections.FirstOrDefault(
						x => x.Name.StartsWith(ss.SafeRemainingArgument, StringComparison.InvariantCultureIgnoreCase));
				return hAccent == null
					? "That is not a valid accent to see help for."
					: $"Info for Accent \"{hAccent.Name.TitleCase().Colour(Telnet.Green)}\" - Group ({hAccent.Group.TitleCase().Colour(Telnet.Green)})\nAccent String: {hAccent.AccentSuffix.Colour(Telnet.Green)}\nUnfamiliar Accent String: {hAccent.VagueSuffix.Colour(Telnet.Green)}\nDifficulty to Understand for those Unfamiliar with it: {hAccent.Difficulty.Describe().Colour(Telnet.Green)}\nDescription: {hAccent.Description.ProperSentences().Colour(Telnet.Green)}";
			}

			var selectedAccent = GetAccentForSelection(command);
			if (selectedAccent == null)
			{
				return "That is not a valid accent for you to pick.";
			}

			if (Chargen.SelectedAccents.Contains(selectedAccent))
			{
				Chargen.SelectedAccents.Remove(selectedAccent);
			}
			else
			{
				if (Storyboard.AdditionalPickResource == null &&
				    Chargen.SelectedAccents.Count(x => x.Language == LanguageEnumerator.Current) ==
				    Storyboard.NumberOfAccentsPerLanguage)
				{
					return "You have already selected the maximum number of accents for this language.";
				}

				Chargen.SelectedAccents.Add(selectedAccent);
			}

			Chargen.RecalculateCurrentCosts();
			return Display();
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
			case "picks":
				return BuildingCommandPicks(actor, command);
			case "extra":
			case "extras":
			case "additional":
			case "more":
			case "extrapicks":
				return BuildingCommandExtraPicks(actor, command);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandExtraPicks(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either enter an amount and type of resource for each extra pick to cost, or use {"none".ColourCommand()} to disable extra picks.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			AdditionalPickCost = 0;
			AdditionalPickResource = null;
			Changed = true;
			actor.OutputHandler.Send("Players will no longer be able to select extra accents for their languages.");
			return true;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must enter a valid positive number.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which account resource should extra picks cost?");
			return false;
		}

		var resource = Gameworld.ChargenResources.GetByIdOrName(command.SafeRemainingArgument);
		if (resource is null)
		{
			actor.OutputHandler.Send("There is no such account resource.");
			return false;
		}

		AdditionalPickCost = value;
		AdditionalPickResource = resource;
		Changed = true;
		actor.OutputHandler.Send(
			$"Players will now be able select additional accents for their languages at a cost of {$"{value.ToString("N0", actor)} {(value == 1 ? resource.Name : resource.PluralName)}".ColourValue()}.");
		return true;
	}

	private bool BuildingCommandPicks(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must enter a valid number of picks for accents per language.");
			return false;
		}

		NumberOfAccentsPerLanguage = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"Players will now be given {NumberOfAccentsPerLanguage.ToString("N0", actor).ColourValue()} free accent choices per language.");
		return true;
	}

	private bool BuildingCommandBlurb(ICharacter actor, StringStack command)
	{
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