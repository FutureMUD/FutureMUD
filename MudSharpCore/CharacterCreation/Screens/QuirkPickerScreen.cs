using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Merits;

namespace MudSharp.CharacterCreation.Screens;

public class QuirkPickerScreenStoryboard : ChargenScreenStoryboard
{
	private QuirkPickerScreenStoryboard()
	{
	}

	public QuirkPickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		Blurb = definition.Element("SelectionBlurb").Value;
		SkipScreenIfNoChoices = bool.Parse(definition.Element("SkipScreenIfNoChoices").Value);
		MaximumQuirks = int.Parse(definition.Element("MaximumQuirks").Value);
	}

	protected QuirkPickerScreenStoryboard(IFuturemud gameworld, IChargenScreenStoryboard storyboard) : base(gameworld,
		storyboard)
	{
		switch (storyboard)
		{
			case MeritPickerScreenStoryboard mps:
				Blurb = mps.Blurb;
				SkipScreenIfNoChoices = mps.SkipScreenIfNoChoices;
				MaximumQuirks = mps.MaximumMeritsAndFlaws;
				break;
		}

		SaveAfterTypeChange();
	}

	protected override string StoryboardName => "QuirkPicker";

	/// <summary>
	///     The text displayed above the Merits Selection auto-generated component
	/// </summary>
	public string Blurb { get; protected set; }

	public bool SkipScreenIfNoChoices { get; protected set; }

	public int MaximumQuirks { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectMerits;

	public override string HelpText => $@"{BaseHelpText}
	#3skipnone#0 - toggles skipping the screen if there is only one valid choice
	#3max <#>#0 - sets the maximum total quirks selectable";

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("SelectionBlurb", new XCData(Blurb)),
			new XElement("SkipScreenIfNoChoices", SkipScreenIfNoChoices),
			new XElement("MaximumQuirks", MaximumQuirks)
		).ToString();
	}

	#endregion

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectMerits,
			new ChargenScreenStoryboardFactory("QuirkPicker",
				(game, dbitem) => new QuirkPickerScreenStoryboard(game, dbitem),
				(game, other) => new QuirkPickerScreenStoryboard(game, other)
			),
			"QuirkPicker",
			"Select quirks up to a maximum",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new QuirkPickerScreen(chargen, this);
	}

	public override IEnumerable<(IChargenResource Resource, int Cost)> ChargenCosts(IChargen chargen)
	{
		foreach (var resource in Gameworld.ChargenResources)
		{
			var sum = 0;
			foreach (var merit in chargen.SelectedMerits)
			{
				sum += merit.ResourceCost(resource);
			}

			yield return (resource, sum);
		}
	}

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine(
			"This screen allows people to select any number of merits and flaws from a single screen. They are limited only by the maximum number of picks and what they can afford. If you're using this option, you would typically be having the merits and flaws cost/give some kind of resource for balance purposes."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine($"Skip Screen If No Choices: {SkipScreenIfNoChoices.ToColouredString()}");
		sb.AppendLine($"Maximum Quirk Selections: {MaximumQuirks.ToString("N0", voyeur).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(Blurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		return sb.ToString();
	}

	internal class QuirkPickerScreen : ChargenScreen
	{
		protected List<ICharacterMerit> SelectableMerits;
		protected ICharacterMerit SelectedMerit;
		protected bool ShownInitialScreen;

		protected QuirkPickerScreenStoryboard Storyboard;

		internal QuirkPickerScreen(IChargen chargen, QuirkPickerScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
			Chargen.SelectedMerits.Clear();
			SelectableMerits = GetSelectableMerits();
			if (!SelectableMerits.Any())
			{
				State = ChargenScreenState.Complete;
			}
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectMerits;

		public override string Display()
		{
			if (!ShownInitialScreen)
			{
				return
					$"{"Quirk Selection".Colour(Telnet.Cyan)}\n\n{Storyboard.Blurb.Wrap(Account.InnerLineFormatLength).SubstituteANSIColour()}\n\nPlease type {"continue".Colour(Telnet.Yellow)} to begin. At any time, you may type {"reset".Colour(Telnet.Yellow)} to return to the beginning.";
			}

			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			if (SelectedMerit != null)
			{
				return string.Format("Quirk: {1} ({0})\n\n{2}\n{3}Do you want to {4} this {0}? Type {5} or {6}.",
					SelectedMerit.MeritType == MeritType.Merit ? "Merit" : "Flaw",
					SelectedMerit.Name.TitleCase().Colour(Telnet.Green),
					SelectedMerit.ChargenBlurb.Wrap(Account.InnerLineFormatLength).SubstituteANSIColour(),
					Storyboard.Gameworld.ChargenResources.Any(x => SelectedMerit.ResourceCost(x) > 0)
						? $"This selection costs {Storyboard.Gameworld.ChargenResources.Where(x => SelectedMerit.ResourceCost(x) > 0).Select(x => Tuple.Create(x, SelectedMerit.ResourceCost(x))).Select(x => CommonStringUtilities.CultureFormat($"{x.Item2} {(x.Item2 == 1 ? x.Item1.Name.TitleCase() : x.Item1.PluralName.TitleCase())}", Account).Colour(Telnet.Green)).ListToString()}\n\n"
						: "",
					Chargen.SelectedMerits.Contains(SelectedMerit) ? "remove" : "select",
					"yes".Colour(Telnet.Yellow),
					"no".Colour(Telnet.Yellow)
				);
			}

			var index = 1;
			return
				$"{"Quirk Selection".Colour(Telnet.Cyan)}\n\n{SelectableMerits.Select(x => $"{index++}: {x.Name.TitleCase()}").ArrangeStringsOntoLines((uint)Account.LineFormatLength / 40, (uint)Account.LineFormatLength)}\n\n{(Chargen.SelectedMerits.Any() ? $"You have selected these quirks: {Chargen.SelectedMerits.Select(x => x.Name.TitleCase().Colour(Telnet.Green)).ListToString()}\n\n" : "")}Select the name or number of your desired quirk. Type {"done".Colour(Telnet.Yellow)} when you are done, or {"reset".Colour(Telnet.Yellow)} to start again.";
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

			if (SelectedMerit != null)
			{
				if ("yes".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
				{
					if (!Chargen.SelectedMerits.Contains(SelectedMerit) &&
					    Chargen.SelectedMerits.Count >= Storyboard.MaximumQuirks)
					{
						SelectedMerit = null;
						return
							CommonStringUtilities.CultureFormat(
								$"You are only allowed to select {Storyboard.MaximumQuirks} total quirks. That selection would put you over the limit.\n\n{Display()}",
								Account);
					}

					if (Chargen.SelectedMerits.Contains(SelectedMerit))
					{
						Chargen.SelectedMerits.Remove(SelectedMerit);
					}
					else
					{
						Chargen.SelectedMerits.Add(SelectedMerit);
					}

					SelectedMerit = null;
					SelectableMerits = GetSelectableMerits();
					return Display();
				}

				SelectedMerit = null;
				return Display();
			}

			if (!ShownInitialScreen && "continue".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				ShownInitialScreen = true;
				return Display();
			}

			if (ShownInitialScreen && "reset".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				ShownInitialScreen = false;
				Chargen.SelectedMerits.Clear();
				SelectableMerits = GetSelectableMerits();
				return Display();
			}

			if ("done".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				if (Storyboard.MaximumQuirks > 0 && Chargen.SelectedMerits.Count > Storyboard.MaximumQuirks)
				{
					return
						CommonStringUtilities.CultureFormat(
							$"You may only select a total of {Storyboard.MaximumQuirks:N0} quirks.", Account);
				}

				State = ChargenScreenState.Complete;
				return "";
			}

			var merit = int.TryParse(command, out var value)
				? SelectableMerits.ElementAtOrDefault(value - 1)
				: SelectableMerits.FirstOrDefault(
					x => x.Name.StartsWith(command, StringComparison.InvariantCultureIgnoreCase));

			if (merit == null)
			{
				merit =
					Chargen.SelectedMerits.FirstOrDefault(x =>
						x.Name.StartsWith(command, StringComparison.InvariantCultureIgnoreCase));
				if (merit == null)
				{
					return
						"That is not a valid selection. Please enter the number or name of the quirk you want to select.";
				}
			}

			SelectedMerit = merit;
			return Display();
		}

		private List<ICharacterMerit> GetSelectableMerits()
		{
			return
				Storyboard.Gameworld.Merits
				          .OrderBy(x => x.Name)
				          .OfType<ICharacterMerit>()
				          .Where(x => x.ChargenAvailable(Chargen))
				          .Except(Chargen.SelectedMerits).ToList();
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
			case "skipnone":
				return BuildingCommandSkipNone(actor);
			case "max":
				return BuildingCommandMax(actor, command);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandMax(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must enter a valid number of maximum quirk selections.");
			return false;
		}

		MaximumQuirks = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"The maximum number of quirks to be selected will now be {value.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandSkipNone(ICharacter actor)
	{
		SkipScreenIfNoChoices = !SkipScreenIfNoChoices;
		Changed = true;
		actor.OutputHandler.Send(
			$"This screen will {(SkipScreenIfNoChoices ? "now" : "no longer")} be skipped if no valid selections are available.");
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