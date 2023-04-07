using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Economy.Payment;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.FutureProg.Statements;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Merits;

namespace MudSharp.CharacterCreation.Screens;

public class MeritPickerScreenStoryboard : ChargenScreenStoryboard
{
	private MeritPickerScreenStoryboard()
	{
	}

	public MeritPickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		Blurb = definition.Element("MeritSelectionBlurb").Value;
		SkipScreenIfNoChoices = bool.Parse(definition.Element("SkipScreenIfNoChoices").Value);
		ForceBalanceOfMeritsAndFlaws = bool.Parse(definition.Element("ForceBalanceOfMeritsAndFlaws").Value);
		MaximumMeritsAndFlaws = int.Parse(definition.Element("MaximumMeritsAndFlaws").Value);
	}

	protected MeritPickerScreenStoryboard(IFuturemud gameworld, IChargenScreenStoryboard storyboard) : base(gameworld,
		storyboard)
	{
		switch (storyboard)
		{
			case QuirkPickerScreenStoryboard qps:
				Blurb = qps.Blurb;
				SkipScreenIfNoChoices = qps.SkipScreenIfNoChoices;
				MaximumMeritsAndFlaws = qps.MaximumQuirks;
				ForceBalanceOfMeritsAndFlaws = false;
				break;
		}

		SaveAfterTypeChange();
	}

	protected override string StoryboardName => "MeritPicker";

	/// <summary>
	///     The text displayed above the Merits Selection auto-generated component
	/// </summary>
	public string Blurb { get; protected set; }

	public bool SkipScreenIfNoChoices { get; protected set; }

	public bool ForceBalanceOfMeritsAndFlaws { get; protected set; }

	public int MaximumMeritsAndFlaws { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectMerits;

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("MeritSelectionBlurb", new XCData(Blurb)),
			new XElement("SkipScreenIfNoChoices", SkipScreenIfNoChoices),
			new XElement("ForceBalanceOfMeritsAndFlaws", ForceBalanceOfMeritsAndFlaws),
			new XElement("MaximumMeritsAndFlaws", MaximumMeritsAndFlaws)
		).ToString();
	}

	#endregion

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectMerits,
			new ChargenScreenStoryboardFactory("MeritPicker",
				(game, dbitem) => new MeritPickerScreenStoryboard(game, dbitem),
				(game, other) => new MeritPickerScreenStoryboard(game, other)
			),
			"MeritPicker",
			"Pick separate merits and flaws, optional balancing",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new MeritPickerScreen(chargen, this);
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
			"This screen allows people to select merits, and then flaws separately on two screens. You can also make them have to balance the number of merits against flaws. This option works well if you are not using a build point resource."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine($"Skip Screen If No Choices: {SkipScreenIfNoChoices.ToColouredString()}");
		sb.AppendLine($"Maximum Merit/Flaw Selections: {MaximumMeritsAndFlaws.ToString("N0", voyeur).ColourValue()}");
		sb.AppendLine($"Force Balance of Merits and Flaws: {ForceBalanceOfMeritsAndFlaws.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(Blurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		return sb.ToString();
	}

	internal class MeritPickerScreen : ChargenScreen
	{
		protected bool HasSelectedMerits;
		protected List<ICharacterMerit> SelectableMerits;
		protected ICharacterMerit SelectedMerit;
		protected bool ShownInitialScreen;

		protected MeritPickerScreenStoryboard Storyboard;

		internal MeritPickerScreen(IChargen chargen, MeritPickerScreenStoryboard storyboard)
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
					$"{"Merits and Flaws Selection".Colour(Telnet.Cyan)}\n\n{Storyboard.Blurb.Wrap(Account.InnerLineFormatLength).SubstituteANSIColour()}\n\nPlease type {"continue".Colour(Telnet.Yellow)} to begin. At any time, you may type {"back".Colour(Telnet.Yellow)} to go back or {"reset".Colour(Telnet.Yellow)} to return to the beginning.";
			}

			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			if (SelectedMerit != null)
			{
				return string.Format("{0}: {1}\n\n{2}\n{3}Do you want to {4} this {0}? Type {5} or {6}.",
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
			if (!HasSelectedMerits)
			{
				return
					$"{"Merit Selection".Colour(Telnet.Cyan)}\n\n{SelectableMerits.Select(x => $"{index++}: {x.Name.TitleCase()}").ArrangeStringsOntoLines((uint)Account.LineFormatLength / 40, (uint)Account.LineFormatLength)}\n\n{(Chargen.SelectedMerits.Any() ? $"You have selected these merits: {Chargen.SelectedMerits.Select(x => x.Name.TitleCase().Colour(Telnet.Green)).ListToString()}\n\n" : "")}Select the name or number of your desired merit. Type {"done".Colour(Telnet.Yellow)} when you are done, or {"reset".Colour(Telnet.Yellow)} to start again.";
			}

			var selectedMerits = Chargen.SelectedMerits.Where(x => x.MeritType == MeritType.Merit).ToList();
			var selectedFlaws = Chargen.SelectedMerits.Where(x => x.MeritType == MeritType.Flaw).ToList();

			return
				$"{"Flaw Selection".Colour(Telnet.Cyan)}\n\n{SelectableMerits.Select(x => $"{index++}: {x.Name.TitleCase()}").ArrangeStringsOntoLines((uint)Account.LineFormatLength / 40, (uint)Account.LineFormatLength)}\n\n{(selectedMerits.Any() ? $"You have selected these merits: {selectedMerits.Select(x => x.Name.TitleCase().Colour(Telnet.Green)).ListToString()}\n" : "")}{(selectedFlaws.Any() ? $"You have selected these flaws: {selectedFlaws.Select(x => x.Name.TitleCase().Colour(Telnet.Green)).ListToString()}\n" : "")}{(Storyboard.ForceBalanceOfMeritsAndFlaws ? $"You have selected {selectedMerits.Count:N0} {(selectedMerits.Count == 1 ? "merit" : "merits")}, and so must select the same number of flaws.\n".Colour(Telnet.Yellow) : "")}Select the name or number of your desired flaw. Type {"done".Colour(Telnet.Yellow)} when you are done, {"back".Colour(Telnet.Yellow)} to return to merit selection, or {"reset".Colour(Telnet.Yellow)} to start again.";
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
					if (Storyboard.ForceBalanceOfMeritsAndFlaws && !Chargen.SelectedMerits.Contains(SelectedMerit) &&
					    Chargen.SelectedMerits.Count(
						    x => x.MeritType == (HasSelectedMerits ? MeritType.Flaw : MeritType.Merit)) >=
					    Storyboard.MaximumMeritsAndFlaws / 2)
					{
						SelectedMerit = null;
						return
							CommonStringUtilities.CultureFormat(
								$"You are only allowed to select {Storyboard.MaximumMeritsAndFlaws} total merits and flaws, and must balance merits with flaws. That selection would mean you could not do that.\n\n{Display()}",
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
				HasSelectedMerits = false;
				Chargen.SelectedMerits.Clear();
				SelectableMerits = GetSelectableMerits();
				return Display();
			}

			if (ShownInitialScreen && "back".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				if (HasSelectedMerits)
				{
					HasSelectedMerits = false;
					Chargen.SelectedMerits.Clear();
					SelectableMerits = GetSelectableMerits();
					return Display();
				}

				ShownInitialScreen = false;
				Chargen.SelectedMerits.Clear();
				SelectableMerits = GetSelectableMerits();
				return Display();
			}

			if ("done".StartsWith(command, StringComparison.InvariantCultureIgnoreCase))
			{
				if (HasSelectedMerits)
				{
					if (Storyboard.ForceBalanceOfMeritsAndFlaws &&
					    Chargen.SelectedMerits.Count(x => x.MeritType == MeritType.Merit) !=
					    Chargen.SelectedMerits.Count(x => x.MeritType == MeritType.Flaw))
					{
						return "You must select an equal number of merits and flaws.";
					}

					if (Storyboard.MaximumMeritsAndFlaws > 0 &&
					    Chargen.SelectedMerits.Count > Storyboard.MaximumMeritsAndFlaws)
					{
						return
							CommonStringUtilities.CultureFormat(
								$"You may only select a total of {Storyboard.MaximumMeritsAndFlaws:N0} merits and flaws.",
								Account);
					}

					State = ChargenScreenState.Complete;
					return "";
				}

				HasSelectedMerits = true;
				SelectableMerits = GetSelectableMerits();
				return Display();
			}

			var merit = int.TryParse(command, out var value)
				? SelectableMerits.ElementAtOrDefault(value - 1)
				: SelectableMerits.FirstOrDefault(
					x => x.Name.StartsWith(command, StringComparison.InvariantCultureIgnoreCase));

			if (merit == null)
			{
				merit =
					Chargen.SelectedMerits.Where(
						       x => x.MeritType == (HasSelectedMerits ? MeritType.Flaw : MeritType.Merit))
					       .FirstOrDefault(x =>
						       x.Name.StartsWith(command, StringComparison.InvariantCultureIgnoreCase));
				if (merit == null)
				{
					return
						$"That is not a valid selection. Please enter the number or name of the {(HasSelectedMerits ? "flaw" : "merit")} you want to select.";
				}
			}

			SelectedMerit = merit;
			return Display();
		}

		private List<ICharacterMerit> GetSelectableMerits()
		{
			return
				Storyboard.Gameworld.Merits.Where(
					          x => x.MeritType == (HasSelectedMerits ? MeritType.Flaw : MeritType.Merit))
				          .OrderBy(x => x.Name)
				          .OfType<ICharacterMerit>()
				          .Where(x => x.ChargenAvailable(Chargen))
				          .Except(Chargen.SelectedMerits).ToList();
		}
	}

	#region Building Commands

	public override string HelpText => $@"{BaseHelpText}
	#3skipnone#0 - toggles skipping the screen if there is only one valid choice
	#3balance#0 - toggles forcing balance in number of merits/flaws
	#3max <#>#0 - sets the maximum total merits and flaws";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "blurb":
				return BuildingCommandBlurb(actor, command);
			case "skipnone":
				return BuildingCommandSkipNone(actor);
			case "balance":
				return BuildingCommandBalance(actor);
			case "max":
				return BuildingCommandMax(actor, command);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandMax(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must enter a valid number of maximum merit and flaw selections.");
			return false;
		}

		MaximumMeritsAndFlaws = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"The maximum number of merits and flaws to be selected will now be {value.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandBalance(ICharacter actor)
	{
		ForceBalanceOfMeritsAndFlaws = !ForceBalanceOfMeritsAndFlaws;
		Changed = true;
		actor.OutputHandler.Send(
			$"This screen will {(SkipScreenIfNoChoices ? "now" : "no longer")} force picking a balanced number of merits and flaws.");
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