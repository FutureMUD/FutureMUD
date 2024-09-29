using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp.CharacterCreation.Screens;

public class SkillPickerScreenStoryboard : ChargenScreenStoryboard
{
	private SkillPickerScreenStoryboard()
	{
	}

	public SkillPickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		Blurb = definition.Element("Blurb").Value;
		NumberOfSkillPicksProg = long.TryParse(definition.Element("NumberOfSkillPicksProg").Value, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.FirstOrDefault(
				x =>
					x.FunctionName.Equals(definition.Element("NumberOfSkillPicksProg").Value,
						StringComparison.InvariantCultureIgnoreCase));
		FreeSkillsProg = long.TryParse(definition.Element("FreeSkillsProg").Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.FirstOrDefault(
				x =>
					x.FunctionName.Equals(definition.Element("FreeSkillsProg").Value,
						StringComparison.InvariantCultureIgnoreCase));
	}

	protected SkillPickerScreenStoryboard(IFuturemud gameworld, IChargenScreenStoryboard storyboard) : base(gameworld,
		storyboard)
	{
		switch (storyboard)
		{
			case SkillSkipperScreenStoryboard skip:
				FreeSkillsProg = skip.FreeSkillsProg;
				NumberOfSkillPicksProg = Gameworld.FutureProgs.FirstOrDefault(x => x.FunctionName.EqualTo("AlwaysOne"));
				Blurb =
					"You may now select the skills that your character begins the game with. You can also learn new skills in game.";
				break;
			case SkillCostPickerScreenStoryboard cost:
				FreeSkillsProg = cost.FreeSkillsProg;
				Blurb = cost.SkillPickerBlurb;
				NumberOfSkillPicksProg = cost.NumberOfFreeSkillPicksProg;
				break;
			case SkillBoostSkipperScreenStoryboard boost:
				FreeSkillsProg = boost.FreeSkillsProg;
				NumberOfSkillPicksProg = Gameworld.FutureProgs.FirstOrDefault(x => x.FunctionName.EqualTo("AlwaysOne"));
				Blurb =
					"You may now select the skills that your character begins the game with. You can also learn new skills in game.";
				break;
		}

		SaveAfterTypeChange();
	}

	protected override string StoryboardName => "SkillPicker";

	public IFutureProg NumberOfSkillPicksProg { get; protected set; }
	public IFutureProg FreeSkillsProg { get; protected set; }

	public string Blurb { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectSkills;

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Blurb", new XCData(Blurb)),
			new XElement("NumberOfSkillPicksProg", NumberOfSkillPicksProg?.Id ?? 0),
			new XElement("FreeSkillsProg", FreeSkillsProg?.Id ?? 0)
		).ToString();
	}

	#endregion

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine(
			"This screen allows a player to select a number of skills to start with, with no extras permitted and no boosts screen."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine($"Free Skills Prog: {FreeSkillsProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"# Picks Prog: {FreeSkillsProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine();
		sb.AppendLine("Skill Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(Blurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		return sb.ToString();
	}

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectSkills,
			new ChargenScreenStoryboardFactory("SkillPicker",
				(game, dbitem) => new SkillPickerScreenStoryboard(game, dbitem),
				(game, other) => new SkillPickerScreenStoryboard(game, other)),
			"SkillPicker",
			"Pick a # of skills and boosts",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new SkillPickerScreen(chargen, this);
	}

	public override IEnumerable<(IChargenResource Resource, int Cost)> ChargenCosts(IChargen chargen)
	{
		foreach (var resource in Gameworld.ChargenResources)
		{
			var sum = 0;
			foreach (var skill in chargen.SelectedSkills)
			{
				sum += skill.ResourceCost(resource);
			}

			yield return (resource, sum);
		}
	}

	internal class SkillPickerScreen : ChargenScreen
	{
		protected List<ITraitDefinition> CurrentSelectables = new();
		protected IEnumerable<ITraitDefinition> FreeSkills;
		protected List<ITraitDefinition> LastCurrentSelectables;
		protected SkillPickerScreenStoryboard Storyboard;

		internal SkillPickerScreen(IChargen chargen, SkillPickerScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
			Chargen.SelectedSkills.Clear();
			FreeSkills = storyboard.FreeSkillsProg?.ExecuteCollection<ITraitDefinition>(chargen) ?? new List<ITraitDefinition>();
			Chargen.SelectedSkills.AddRange(FreeSkills);
			SetCurrentSelectables();
			LastCurrentSelectables = CurrentSelectables;
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectSkills;

		public override string Display()
		{
			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			var selectedSkills = Chargen.SelectedSkills.Except(FreeSkills).ToList();
			var languages = CurrentSelectables.Where(x => x.Group.EqualTo("language")).ToList();
			var nonLanguages = CurrentSelectables.Where(x => !languages.Contains(x)).ToList();
			if (languages.Any())
			{
				return DisplayLanguages(selectedSkills, languages, nonLanguages);
			}

			return
				$@"{"Skill Selection".Colour(Telnet.Cyan)}

{Storyboard.Blurb.SubstituteANSIColour().Wrap(Chargen.Account.InnerLineFormatLength)}

You get the following skills for free:
{FreeSkills.Where(x => !x.Hidden).Select(x => x.Name.Colour(Telnet.Green)).DefaultIfEmpty("None".Colour(Telnet.Yellow)).ListToString().Wrap(Chargen.Account.InnerLineFormatLength)}

You can select from the following skills:

{CurrentSelectables.Select(x => x.Name.Colour(Chargen.SelectedSkills.Contains(x) ? Telnet.Green : Telnet.White).Parentheses(Chargen.SelectedSkills.Contains(x)).FluentTagMXP(LastCurrentSelectables.Contains(x) ? null : "B").FluentTagMXP(LastCurrentSelectables.Contains(x) ? null : "I")).ArrangeStringsOntoLines((uint)Account.LineFormatLength / 30, (uint)Account.LineFormatLength)}

You have selected {selectedSkills.Count} of a possible {Convert.ToDouble(Storyboard.NumberOfSkillPicksProg.Execute(Chargen))} skills{(Storyboard.Gameworld.ChargenResources.Any(x => selectedSkills.Any(y => y.ResourceCost(x) > 0)) ? $", costing {Storyboard.Gameworld.ChargenResources.Where(x => selectedSkills.Sum(y => y.ResourceCost(x)) > 0).Select(x => $"{selectedSkills.Sum(y => y.ResourceCost(x))} {x.Alias}".Colour(Telnet.Green)).ListToString()}" : "")}.
Type the name of the skill you would like to select, or type {"done".Colour(Telnet.Yellow)} to finish.";
		}

		private string DisplayLanguages(List<ITraitDefinition> selectedSkills, List<ITraitDefinition> languages,
			List<ITraitDefinition> nonLanguages)
		{
			return
				$@"{"Skill Selection".Colour(Telnet.Cyan)}

{Storyboard.Blurb.SubstituteANSIColour().Wrap(Chargen.Account.InnerLineFormatLength)}

You get the following skills for free: 
{FreeSkills.Where(x => !x.Hidden).Select(x => x.Name.Colour(Telnet.Green)).DefaultIfEmpty("None".Colour(Telnet.Yellow)).ListToString().Wrap(Chargen.Account.InnerLineFormatLength)}

You can select from the following skills:

{nonLanguages.Select(x => x.Name.Colour(Chargen.SelectedSkills.Contains(x) ? Telnet.Green : Telnet.White).Parentheses(Chargen.SelectedSkills.Contains(x)).FluentTagMXP(LastCurrentSelectables.Contains(x) ? null : "B").FluentTagMXP(LastCurrentSelectables.Contains(x) ? null : "I")).ArrangeStringsOntoLines((uint)Account.LineFormatLength / 30, (uint)Account.LineFormatLength)}

You also have the following languages available:

{languages.Select(x => x.Name.Colour(Chargen.SelectedSkills.Contains(x) ? Telnet.Green : Telnet.White).Parentheses(Chargen.SelectedSkills.Contains(x)).FluentTagMXP(LastCurrentSelectables.Contains(x) ? null : "B").FluentTagMXP(LastCurrentSelectables.Contains(x) ? null : "I")).ArrangeStringsOntoLines((uint)Account.LineFormatLength / 30, (uint)Account.LineFormatLength)}

You have selected {selectedSkills.Count} of a possible {Convert.ToDouble(Storyboard.NumberOfSkillPicksProg.Execute(Chargen))} skills{(Storyboard.Gameworld.ChargenResources.Any(x => selectedSkills.Any(y => y.ResourceCost(x) > 0)) ? $", costing {Storyboard.Gameworld.ChargenResources.Where(x => selectedSkills.Sum(y => y.ResourceCost(x)) > 0).Select(x => $"{selectedSkills.Sum(y => y.ResourceCost(x))} {x.Alias}".Colour(Telnet.Green)).ListToString()}" : "")}.
Type the name of the skill you would like to select, or type {"done".Colour(Telnet.Yellow)} to finish.";
		}

		private void SetCurrentSelectables()
		{
			LastCurrentSelectables = CurrentSelectables;
			CurrentSelectables =
				Storyboard.Gameworld.Traits.Where(
					          x => x.TraitType == TraitType.Skill && x.ChargenAvailable(Chargen))
				          .Where(
					          x =>
						          !FreeSkills.Contains(x)).OrderBy(x => x.Name).ToList();
		}

		private bool CanProgress()
		{
			return !Chargen.SelectedSkills.All(x => Chargen.Gameworld.Languages.All(y => y.LinkedTrait != x));

			// TODO - other reasons why skill selection couldn't continue
		}

		private string WhyCannotProgress()
		{
			if (Chargen.SelectedSkills.All(x => Chargen.Gameworld.Languages.All(y => y.LinkedTrait != x)))
			{
				return
					"You must select at least one language before you can advance past skill selection, and you have not selected any.";
			}

			// TODO - other reasons why skill selection couldn't continue

			throw new NotImplementedException();
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

			var lcommand = command.ToLowerInvariant();
			if (lcommand == "done")
			{
				if (!CanProgress())
				{
					return WhyCannotProgress();
				}

				Chargen.SelectedSkills = Chargen.SelectedSkills.Distinct().ToList();
				// TODO - why is this necessary?
				State = ChargenScreenState.Complete;
				return "";
			}

			if (lcommand.Equals("help", StringComparison.InvariantCultureIgnoreCase) ||
			    lcommand.StartsWith("help ", StringComparison.InvariantCultureIgnoreCase))
			{
				var ss = new StringStack(lcommand.RemoveFirstWord());
				if (ss.IsFinished)
				{
					return "What skill do you want to see help for?";
				}

				var argument = ss.SafeRemainingArgument;
				var hskill =
					CurrentSelectables.FirstOrDefault(
						x => x.Name.StartsWith(argument, StringComparison.InvariantCultureIgnoreCase));
				if (hskill == null)
				{
					return "That is not a valid skill to see help for.";
				}

				var helpfile =
					Chargen.Gameworld.Helpfiles.FirstOrDefault(
						x => x.Name.Equals(hskill.Name, StringComparison.InvariantCultureIgnoreCase));
				return helpfile == null
					? $"There is no helpfile for the {hskill.Name.Proper()} skill. Sorry!"
					: helpfile.DisplayHelpFile(Chargen);
			}

			var skill =
				CurrentSelectables.FirstOrDefault(
					x => x.Name.StartsWith(lcommand, StringComparison.InvariantCultureIgnoreCase));
			if (skill == null)
			{
				return "That is not a valid skill.";
			}

			if (FreeSkills.Contains(skill))
			{
				return "You receive that skill automatically, and cannot select or unselect it.";
			}

			if (Chargen.SelectedSkills.Contains(skill))
			{
				Chargen.SelectedSkills.Remove(skill);
				Chargen.SelectedSkills.RemoveAll(x => !FreeSkills.Contains(x) && !x.ChargenAvailable(Chargen));
				SetCurrentSelectables();
			}
			else
			{
				if (Chargen.SelectedSkills.Except(FreeSkills).Count() >=
				    Convert.ToDouble(Storyboard.NumberOfSkillPicksProg.Execute(Chargen)))
				{
					return "You cannot pick any more skills. Type " + "done".Colour(Telnet.Yellow) +
					       " if you are done.";
				}

				Chargen.SelectedSkills.Add(skill);
				SetCurrentSelectables();
			}

			Chargen.RecalculateCurrentCosts();
			return Display();
		}
	}

	#region Building Commands

	public override string HelpText => $@"{BaseHelpText}
	#3blurb#0 - drops you into an editor to change the blurb
	#3freeskills <prog>#0 - sets the free skills prog
	#3picks <prog>#0 - sets a prog for how many skill picks the player gets";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "blurb":
				return BuildingCommandBlurb(actor, command);
			case "picks":
				return BuildingCommandPicks(actor, command);
			case "freeskills":
			case "freeskillsprog":
			case "freeskill":
			case "freeskillprog":
			case "skillsprog":
			case "skillprog":
				return BuildingCommandFreeSkills(actor, command);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandPicks(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control the number of skill picks available to players?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Number, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Chargen
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		NumberOfSkillPicksProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The prog {prog.MXPClickableFunctionName()} will now control the number of skill picks available to players.");
		return true;
	}

	private bool BuildingCommandFreeSkills(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog do you want to use to control which skills a character gets for free?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Collection | FutureProgVariableTypes.Trait, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Chargen
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		FreeSkillsProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The prog {prog.MXPClickableFunctionName()} will now be used to control which skills characters get for free.");
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