using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using NCalc;
using Expression = ExpressionEngine.Expression;

namespace MudSharp.CharacterCreation.Screens;

public class SkillCostPickerScreenStoryboard : ChargenScreenStoryboard
{
	private SkillCostPickerScreenStoryboard()
	{
	}

	public SkillCostPickerScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		SkillPickerBlurb = definition.Element("SkillPickerBlurb").Value;
		SkillBoostBlurb = definition.Element("SkillBoostBlurb").Value;
		NumberOfFreeSkillPicksProg =
			long.TryParse(definition.Element("NumberOfFreeSkillPicksProg").Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.FirstOrDefault(
					x =>
						x.FunctionName.Equals(definition.Element("NumberOfFreeSkillPicksProg").Value,
							StringComparison.InvariantCultureIgnoreCase));
		FreeSkillsProg = long.TryParse(definition.Element("FreeSkillsProg").Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.FirstOrDefault(
				x =>
					x.FunctionName.Equals(definition.Element("FreeSkillsProg").Value,
						StringComparison.InvariantCultureIgnoreCase));
		BaseBoostCostProg = long.TryParse(definition.Element("BoostCostProg").Value, out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.FirstOrDefault(
				x =>
					x.FunctionName.Equals(definition.Element("BoostCostProg").Value,
						StringComparison.InvariantCultureIgnoreCase));
		BoostCostExpression = new Expression(definition.Element("BoostCostExpression").Value);
		AdditionalSkillsCostExpression = new Expression(definition.Element("AdditionalSkillsCostExpression").Value);
		BoostResource = Gameworld.ChargenResources.Get(long.Parse(definition.Element("BoostResource").Value));
		MaximumBoosts = int.Parse(definition.Element("MaximumBoosts").Value);
		FreeBoostResource = int.Parse(definition.Element("FreeBoostResource").Value);
	}

	protected SkillCostPickerScreenStoryboard(IFuturemud gameworld, IChargenScreenStoryboard storyboard) : base(
		gameworld,
		storyboard)
	{
		BoostCostExpression = new Expression("base * Pow(boosts,2)");
		AdditionalSkillsCostExpression = new Expression("50 * Pow(picks,2)");
		switch (storyboard)
		{
			case SkillSkipperScreenStoryboard skip:
				FreeSkillsProg = skip.FreeSkillsProg;
				SkillPickerBlurb =
					"You may now select the skills that your character begins the game with. You can also learn new skills in game.";
				SkillBoostBlurb =
					@"The next step is deciding whether to apply any boosts to your character's starting skills. This is a totally optional process, and costs a large amount of build points. Each character also gets one free boost, so even new players can boost an important skill.  Each skill boost will push your starting skill value up approximately one ""rank"". It is mostly designed so that after the first few characters, when players have started to accumulate some build points, they can avoid some of the starting grind, but ""troll"" players who consistently roll red-shirt characters to try and PK don't get the same leg up.";
				BoostResource = Gameworld.ChargenResources.FirstOrDefault();
				BaseBoostCostProg = Gameworld.FutureProgs.FirstOrDefault(x => x.FunctionName.EqualTo("AlwaysOne"));
				NumberOfFreeSkillPicksProg =
					Gameworld.FutureProgs.FirstOrDefault(x => x.FunctionName.EqualTo("AlwaysOne"));
				MaximumBoosts = 0;
				FreeBoostResource = 0;
				break;
			case SkillPickerScreenStoryboard picker:
				FreeSkillsProg = picker.FreeSkillsProg;
				SkillPickerBlurb = picker.Blurb;
				SkillBoostBlurb =
					@"The next step is deciding whether to apply any boosts to your character's starting skills. This is a totally optional process, and costs a large amount of build points. Each character also gets one free boost, so even new players can boost an important skill.  Each skill boost will push your starting skill value up approximately one ""rank"". It is mostly designed so that after the first few characters, when players have started to accumulate some build points, they can avoid some of the starting grind, but ""troll"" players who consistently roll red-shirt characters to try and PK don't get the same leg up.";
				BoostResource = Gameworld.ChargenResources.FirstOrDefault();
				BaseBoostCostProg = Gameworld.FutureProgs.FirstOrDefault(x => x.FunctionName.EqualTo("AlwaysOne"));
				NumberOfFreeSkillPicksProg =
					Gameworld.FutureProgs.FirstOrDefault(x => x.FunctionName.EqualTo("AlwaysOne"));
				MaximumBoosts = 0;
				FreeBoostResource = 0;
				break;
			case SkillBoostSkipperScreenStoryboard boost:
				FreeSkillsProg = boost.FreeSkillsProg;
				SkillPickerBlurb =
					"You may now select the skills that your character begins the game with. You can also learn new skills in game.";
				NumberOfFreeSkillPicksProg =
					Gameworld.FutureProgs.FirstOrDefault(x => x.FunctionName.EqualTo("AlwaysOne"));
				SkillBoostBlurb = boost.Blurb;
				BoostCostExpression = new Expression(boost.BoostCostExpression.OriginalExpression);
				BaseBoostCostProg = boost.BaseBoostCostProg;
				BoostResource = boost.BoostResource;
				FreeBoostResource = boost.FreeBoostResource;
				MaximumBoosts = boost.MaximumBoosts;
				break;
		}

		SaveAfterTypeChange();
	}

	protected override string StoryboardName => "SkillCostPicker";

	public IFutureProg NumberOfFreeSkillPicksProg { get; protected set; }
	public IFutureProg FreeSkillsProg { get; protected set; }
	public IFutureProg BaseBoostCostProg { get; protected set; }
	public Expression BoostCostExpression { get; protected set; }
	public Expression AdditionalSkillsCostExpression { get; protected set; }
	public IChargenResource BoostResource { get; protected set; }
	public int MaximumBoosts { get; protected set; }
	public int FreeBoostResource { get; protected set; }

	public string SkillPickerBlurb { get; protected set; }
	public string SkillBoostBlurb { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectSkills;

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("SkillPickerBlurb", new XCData(SkillPickerBlurb)),
			new XElement("NumberOfFreeSkillPicksProg", NumberOfFreeSkillPicksProg?.Id ?? 0),
			new XElement("AdditionalSkillsCostExpression",
				new XCData(AdditionalSkillsCostExpression.OriginalExpression)),
			new XElement("SkillBoostBlurb", new XCData(SkillBoostBlurb)),
			new XElement("FreeSkillsProg", FreeSkillsProg?.Id ?? 0),
			new XElement("BoostCostProg", BaseBoostCostProg?.Id ?? 0),
			new XElement("BoostCostExpression", new XCData(BoostCostExpression.OriginalExpression)),
			new XElement("BoostResource", BoostResource?.Id ?? 0),
			new XElement("MaximumBoosts", MaximumBoosts),
			new XElement("FreeBoostResource", FreeBoostResource)
		).ToString();
	}

	#endregion

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine(
			"This screen has two stages. Firstly, the player picks skills for the character in line with the resources they have to spend. Next, they are presented with a screen where they can spend further resources to boost the starting values for those skills."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine($"Free Skills Prog: {FreeSkillsProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Costs Resource: {BoostResource?.PluralName.ColourValue() ?? "None".ColourError()}");
		sb.AppendLine();
		sb.AppendLine("Skill Screen".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(
			$"Free Picks Prog: {NumberOfFreeSkillPicksProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine(
			$"Additional Skill Cost Prog: {AdditionalSkillsCostExpression?.OriginalExpression.ColourCommand().ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Boost Screen".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Maximum Boosts: {MaximumBoosts.ToString("N0", voyeur).ColourValue()}");
		sb.AppendLine($"Free Boost Resource: {FreeBoostResource.ToString("N0", voyeur).ColourValue()}");
		sb.AppendLine($"Base Boost Cost Prog: {BaseBoostCostProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine(
			$"Boost Cost Expression: {BoostCostExpression?.OriginalExpression.ColourCommand() ?? "None".ColourError()}");
		sb.AppendLine();
		sb.AppendLine("Skill Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(SkillPickerBlurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		sb.AppendLine();
		sb.AppendLine("Boost Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(SkillBoostBlurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		return sb.ToString();
	}

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(
			ChargenStage.SelectSkills,
			new ChargenScreenStoryboardFactory("SkillCostPicker",
				(game, dbitem) => new SkillCostPickerScreenStoryboard(game, dbitem),
				(game, other) => new SkillCostPickerScreenStoryboard(game, other)),
			"SkillCostPicker",
			"Select skills and boosts based on a cost",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new SkillCostPickerScreen(chargen, this);
	}

	public override IEnumerable<(IChargenResource Resource, int Cost)> ChargenCosts(IChargen chargen)
	{
		var freeSkills = FreeSkillsProg?.ExecuteCollection<ITraitDefinition>(chargen) ?? new List<ITraitDefinition>();
		var nonFreeSkills = chargen.SelectedSkills.Except(freeSkills).ToList();
		foreach (var resource in Gameworld.ChargenResources)
		{
			var sum = 0;
			foreach (var skill in nonFreeSkills)
			{
				sum += skill.ResourceCost(resource);
			}


			if (resource == BoostResource)
			{
				var freePicks = (int)Convert.ToDouble(NumberOfFreeSkillPicksProg.Execute(chargen));
				if (nonFreeSkills.Count > freePicks)
				{
					AdditionalSkillsCostExpression.Parameters["picks"] = nonFreeSkills.Count - freePicks;
					sum += Convert.ToInt32(AdditionalSkillsCostExpression.Evaluate());
				}

				sum += chargen.SelectedSkillBoosts.Sum(x => BoostCostForSkill(x.Key, chargen, x.Value));
			}

			yield return (resource, sum);
		}
	}

	private int BoostCostForSkill(ITraitDefinition skill, IChargen chargen, int boosts)
	{
		BoostCostExpression.Parameters["base"] =
			(double)((decimal?)BaseBoostCostProg.Execute(skill, chargen) ?? 1.0M);
		BoostCostExpression.Parameters["boosts"] = boosts;
		return Convert.ToInt32(BoostCostExpression.Evaluate());
	}

	internal class SkillCostPickerScreen : ChargenScreen
	{
		protected List<ITraitDefinition> CurrentSelectables = new();
		protected IEnumerable<ITraitDefinition> FreeSkills;
		protected List<ITraitDefinition> LastCurrentSelectables;
		protected Dictionary<ITraitDefinition, int> SelectedBoostCosts;
		protected Dictionary<ITraitDefinition, int> SelectedBoosts;
		protected bool SkillBoostStage;
		protected SkillCostPickerScreenStoryboard Storyboard;

		internal SkillCostPickerScreen(IChargen chargen, SkillCostPickerScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
			Chargen.SelectedSkills.Clear();
			Chargen.SelectedSkillBoostCosts.Clear();
			Chargen.SelectedSkillBoosts.Clear();
			FreeSkills =
				((IList<IProgVariable>)Storyboard.FreeSkillsProg.Execute(chargen)).Cast<ITraitDefinition>();
			Chargen.SelectedSkills.AddRange(FreeSkills);
			SetCurrentSelectables();
			LastCurrentSelectables = CurrentSelectables;
			SelectedBoosts = new Dictionary<ITraitDefinition, int>();
			SelectedBoostCosts = new Dictionary<ITraitDefinition, int>();
		}

		public override ChargenStage AssociatedStage => ChargenStage.SelectSkills;

		private int BoostCostForSkill(ITraitDefinition skill)
		{
			Storyboard.BoostCostExpression.Parameters["base"] =
				(double)((decimal?)Storyboard.BaseBoostCostProg.Execute(skill, Chargen) ?? 1.0M);
			Storyboard.BoostCostExpression.Parameters["boosts"] = SelectedBoosts[skill];
			return Convert.ToInt32(Storyboard.BoostCostExpression.Evaluate());
		}

		protected string DisplaySkillBoostStage()
		{
			var sb = new StringBuilder();
			sb.AppendLine("Skill Boosts".Colour(Telnet.Cyan));
			sb.AppendLine();
			sb.AppendLine(Storyboard.SkillBoostBlurb);
			sb.AppendLine();
			sb.AppendLine("You picked the following skills:");
			sb.AppendLine();
			var skillStrings = new List<string>();
			foreach (var skill in Chargen.SelectedSkills)
			{
				skillStrings.Add(
					$"{skill.Name.TitleCase().Colour(Telnet.Green)} ({skill.Group.TitleCase()}): x{SelectedBoosts[skill]}{(SelectedBoosts[skill] > 0 ? $" [{SelectedBoostCosts[skill]} {Storyboard.BoostResource.Alias}]" : "")}");
			}

			sb.AppendLineColumns((uint)Account.LineFormatLength, 2, skillStrings.ToArray());
			sb.AppendLine();
			sb.AppendLine(
				$"Total Cost of Boosts: {SelectedBoostCosts.Sum(x => x.Value)} {Storyboard.BoostResource.Alias}");
			sb.AppendLine(
				$"Type the name of the skill you want to boost, {"done".Colour(Telnet.Yellow)} to finish, or type {"help".Colour(Telnet.Yellow)} for more detailed syntax options.");
			return sb.ToString();
		}

		public override string Display()
		{
			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return DisplayChargenAdvice();
			}

			if (SkillBoostStage)
			{
				return DisplaySkillBoostStage();
			}

			return DisplaySkillPickStage();
		}

		private string DisplaySkillPickStage()
		{
			var selectedSkills = Chargen.SelectedSkills.Except(FreeSkills).ToList();
			var languages = CurrentSelectables.Where(x => x.Group.EqualTo("language")).ToList();
			var nonLanguages = CurrentSelectables.Where(x => !languages.Contains(x)).ToList();
			if (languages.Any())
			{
				return DisplayLanguages(selectedSkills, languages, nonLanguages);
			}

			return DisplayNoLanguages(selectedSkills);
		}

		private string DisplayNoLanguages(List<ITraitDefinition> selectedSkills)
		{
			return
				$@"{"Skill Selection".Colour(Telnet.Cyan)}

{Storyboard.SkillPickerBlurb.SubstituteANSIColour().Wrap(Chargen.Account.InnerLineFormatLength)}

You get the following skills for free:
{FreeSkills.Where(x => !x.Hidden).Select(x => x.Name.Colour(Telnet.Green)).DefaultIfEmpty("None".ColourCommand()).ListToString().Wrap(Chargen.Account.InnerLineFormatLength)}

You can select from the following skills:

{CurrentSelectables.Select(x => x.Name.Colour(Chargen.SelectedSkills.Contains(x) ? Telnet.Green : Telnet.White).FluentAppend($" [{x.ResourceCosts.Select(y => $"{y.cost} {y.resource.Alias}").ListToString(conjunction: "", twoItemJoiner: ", ")}]", x.HasResourceCosts).Parentheses(Chargen.SelectedSkills.Contains(x)).FluentTagMXP(LastCurrentSelectables.Contains(x) ? null : "B").FluentTagMXP(LastCurrentSelectables.Contains(x) ? null : "I")).ArrangeStringsOntoLines((uint)Account.LineFormatLength / 30, (uint)Account.LineFormatLength)}

You have selected {selectedSkills.Count} skills, and get {Convert.ToDouble(Storyboard.NumberOfFreeSkillPicksProg.Execute(Chargen)):N0} free picks{(Storyboard.Gameworld.ChargenResources.Any(x => selectedSkills.Any(y => y.ResourceCost(x) > 0)) ? $", costing {Storyboard.Gameworld.ChargenResources.Where(x => selectedSkills.Sum(y => y.ResourceCost(x)) > 0).Select(x => $"{selectedSkills.Sum(y => y.ResourceCost(x))} {x.Alias}".Colour(Telnet.Green)).ListToString()}" : "")}.
Type the name of the skill you would like to select, or type {"done".Colour(Telnet.Yellow)} to finish.";
		}

		private string DisplayLanguages(List<ITraitDefinition> selectedSkills, List<ITraitDefinition> languages,
			List<ITraitDefinition> nonLanguages)
		{
			return
				$@"{"Skill Selection".Colour(Telnet.Cyan)}

{Storyboard.SkillPickerBlurb.SubstituteANSIColour().Wrap(Chargen.Account.InnerLineFormatLength)}

You get the following skills for free: 
{FreeSkills.Where(x => !x.Hidden).Select(x => x.Name.Colour(Telnet.Green)).DefaultIfEmpty("None".ColourCommand()).ListToString().Wrap(Chargen.Account.InnerLineFormatLength)}

You can select from the following skills:

{nonLanguages.Select(x => x.Name.Colour(Chargen.SelectedSkills.Contains(x) ? Telnet.Green : Telnet.White).Parentheses(Chargen.SelectedSkills.Contains(x)).FluentTagMXP(LastCurrentSelectables.Contains(x) ? null : "B").FluentTagMXP(LastCurrentSelectables.Contains(x) ? null : "I")).ArrangeStringsOntoLines((uint)Account.LineFormatLength / 30, (uint)Account.LineFormatLength)}

You also have the following languages available:

{languages.Select(x => x.Name.Colour(Chargen.SelectedSkills.Contains(x) ? Telnet.Green : Telnet.White).Parentheses(Chargen.SelectedSkills.Contains(x)).FluentTagMXP(LastCurrentSelectables.Contains(x) ? null : "B").FluentTagMXP(LastCurrentSelectables.Contains(x) ? null : "I")).ArrangeStringsOntoLines((uint)Account.LineFormatLength / 30, (uint)Account.LineFormatLength)}

You have selected {selectedSkills.Count} skills and get {Convert.ToDouble(Storyboard.NumberOfFreeSkillPicksProg.Execute(Chargen))} free picks{(Storyboard.Gameworld.ChargenResources.Any(x => selectedSkills.Any(y => y.ResourceCost(x) > 0)) ? $", costing {Storyboard.Gameworld.ChargenResources.Where(x => selectedSkills.Sum(y => y.ResourceCost(x)) > 0).Select(x => $"{selectedSkills.Sum(y => y.ResourceCost(x))} {x.Alias}".Colour(Telnet.Green)).ListToString()}" : "")}.
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
			if (SkillBoostStage)
			{
				return true;
			}

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
			if (string.IsNullOrWhiteSpace(command))
			{
				return Display();
			}

			if (!ShownChargenAdvice && HasChargenAdvice())
			{
				return HandleCommandChargenAdvice(command);
			}

			if (SkillBoostStage)
			{
				return HandleCommandSkillBoost(command);
			}

			return HandleCommandSkillPick(command);
		}

		private string HandleCommandSkillBoost(string command)
		{
			var lcommand = command.ToLowerInvariant();
			if (lcommand == "help")
			{
				return
					"You can use the following commands on this screen:\n\n\tdone - finish and proceed to the next stage of chargen\n\treset - reset all your current boosts\n\tback - go back to skill selection\n\nThe following arguments can all accept an option number of times to do the action:\n\n\t<skill> [<times>] - boost skill [optionally x times]\n\t-<skill> [<times>] - remove x boosts from skill\n\t*<group> [<times>] - boost all skills in the specified skill group x times.\n\t-*<group> [<times>] - remove x boosts from all skills in the group\n\tall [<times>]- boost all skills x times\n\t-all [<times>] - remove x boosts from all skills";
			}

			if (lcommand == "done")
			{
				if (!CanProgress())
				{
					return WhyCannotProgress();
				}

				Chargen.SelectedSkillBoosts = SelectedBoosts;
				State = ChargenScreenState.Complete;
				return "";
			}

			if (lcommand == "back")
			{
				SelectedBoosts.Clear();
				SelectedBoostCosts.Clear();
				Chargen.SelectedSkillBoosts.Clear();
				Chargen.SelectedSkillBoostCosts.Clear();
				SkillBoostStage = false;
				return Display();
			}

			if (lcommand == "clear" || lcommand == "reset")
			{
				foreach (var skill in SelectedBoosts.ToList())
				{
					SelectedBoosts[skill.Key] = 0;
					SelectedBoostCosts[skill.Key] = 0;
					Chargen.SelectedSkillBoostCosts.Clear();
					Chargen.SelectedSkillBoosts.Clear();
				}

				return Display();
			}

			var ss = new StringStack(lcommand);
			lcommand = ss.PopSpeech(); // TODO _ pop end?
			var amount = 1;
			if (!ss.IsFinished)
			{
				if (!int.TryParse(ss.RemainingArgument, out amount) || amount < 1)
				{
					return "You must either leave the second argument blank, or specify a number of times to boost";
				}
			}


			if (lcommand == "all")
			{
				foreach (var skill in SelectedBoosts.ToList())
				{
					SelectedBoosts[skill.Key] += amount;
					SelectedBoostCosts[skill.Key] = BoostCostForSkill(skill.Key);
					Chargen.SelectedSkillBoosts[skill.Key] = SelectedBoosts[skill.Key];
				}

				Chargen.SelectedSkillBoostCosts = new Dictionary<IChargenResource, int>
				{
					{ Storyboard.BoostResource, SelectedBoostCosts.Values.Sum() }
				};
				return Display();
			}

			if (lcommand == "-all")
			{
				foreach (var skill in SelectedBoosts.ToList())
				{
					SelectedBoosts[skill.Key] = Math.Max(0, SelectedBoosts[skill.Key] - amount);
					SelectedBoostCosts[skill.Key] = BoostCostForSkill(skill.Key);
					Chargen.SelectedSkillBoosts[skill.Key] = SelectedBoosts[skill.Key];
				}

				Chargen.SelectedSkillBoostCosts = new Dictionary<IChargenResource, int>
				{
					{ Storyboard.BoostResource, SelectedBoostCosts.Values.Sum() }
				};
				return Display();
			}

			var add = true;
			if (lcommand[0] == '-')
			{
				add = false;
				lcommand = lcommand.Substring(1);
				if (string.IsNullOrWhiteSpace(lcommand))
				{
					return "You must specify a skill after the - that you wish to remove a boost from.";
				}
			}

			var groupReference = false;
			if (lcommand[0] == '*')
			{
				groupReference = true;
				lcommand = lcommand.Substring(1);
				if (string.IsNullOrWhiteSpace(lcommand))
				{
					return "You must specify a group after the * that you wish to boost.";
				}
			}

			var skillsToChange = new List<ITraitDefinition>();
			if (groupReference)
			{
				skillsToChange.AddRange(Chargen.SelectedSkills.Where(
					x => x.Group.StartsWith(
						lcommand, StringComparison.InvariantCultureIgnoreCase)));
			}
			else
			{
				var skill = Chargen.SelectedSkills.FirstOrDefault(
					x => x.Name.StartsWith(lcommand, StringComparison.InvariantCultureIgnoreCase));
				if (skill == null)
				{
					return "You have not selected any such skill";
				}

				skillsToChange.Add(skill);
			}

			foreach (var skill in skillsToChange)
			{
				SelectedBoosts[skill] = Math.Min(Storyboard.MaximumBoosts,
					Math.Max(0, SelectedBoosts[skill] + (add ? 1 : -1) * amount));
				SelectedBoostCosts[skill] = BoostCostForSkill(skill);
				Chargen.SelectedSkillBoosts[skill] = SelectedBoosts[skill];
			}

			Chargen.SelectedSkillBoostCosts = new Dictionary<IChargenResource, int>
			{
				{ Storyboard.BoostResource, SelectedBoostCosts.Values.Sum() }
			};
			return Display();
		}

		private string HandleCommandSkillPick(string command)
		{
			var lcommand = command.ToLowerInvariant();
			if (lcommand == "done")
			{
				if (!CanProgress())
				{
					return WhyCannotProgress();
				}

				Chargen.SelectedSkills = Chargen.SelectedSkills.Distinct().ToList();
				SkillBoostStage = true;
				SelectedBoosts = new Dictionary<ITraitDefinition, int>();
				SelectedBoostCosts = new Dictionary<ITraitDefinition, int>();
				foreach (var trait in Chargen.SelectedSkills)
				{
					SelectedBoosts[trait] = 0;
					SelectedBoostCosts[trait] = 0;
				}

				return Display();
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

#5Skill-Screen Options#0

	#3freeskills <prog>#0 - sets the free skills prog
	#3resource <which>#0 - sets the resource used to pay for boosts and extra skill picks
	#3picks <prog>#0 - sets a prog for how many free skill picks the player gets
	#3skillcost <expression>#0 - sets the expression for the cost of extra skill picks

Note, the boost cost expression can use the following parameters:

	#Bpicks#0 - the number of extra picks selected beyond the free ones

#5Boost-Screen Options#0

	#3boostblurb#0 - drops you into an editor to change the blurb
	#3baseboostcost <prog>#0 - sets the base boost cost prog
	#3maxboosts <#>#0 - sets the maximum number of total boosts per skill
	#3freeboostresource <#>#0 - sets the free resources available for boosts
	#3boostcost <expression>#0 - sets the expression for the cost of the boosts

Note, the boost cost expression can use the following parameters:

	#Bbase#0 - the base cost of boosts as determined by the prog
	#Bboosts#0 - the total number of boosts selected by the application";


	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "picks":
				return BuildingCommandPicks(actor, command);
			case "skillcost":
			case "skillcosts":
			case "skillscost":
				return BuildingCommandSkillCost(actor, command);
			case "skillblurb":
				return BuildingCommandSkillBlurb(actor, command);
			case "boostblurb":
				return BuildingCommandBoostBlurb(actor, command);
			case "freeskills":
			case "freeskillsprog":
			case "freeskill":
			case "freeskillprog":
			case "skillsprog":
			case "skillprog":
				return BuildingCommandFreeSkills(actor, command);
			case "baseboostcost":
			case "baseboostscost":
			case "baseboostscosts":
				return BuildingCommandBoostCosts(actor, command);
			case "maxboosts":
				return BuildingCommandMaxBoosts(actor, command);
			case "freeboostresource":
			case "freeboost":
			case "freeresource":
				return BuildingCommandFreeBoostResource(actor, command);
			case "resource":
				return BuildingCommandResource(actor, command);
			case "boostcost":
			case "boostcosts":
				return BuildingCommandCost(actor, command);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandSkillCost(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the expression for the total cost of extra skill picks?");
			return false;
		}

		var expression = new Expression(command.SafeRemainingArgument);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send(expression.Error);
			return false;
		}

		AdditionalSkillsCostExpression = expression;
		Changed = true;
		actor.OutputHandler.Send(
			$"The expression for total cost of extra skill picks is now {expression.OriginalExpression.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandPicks(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control the number of free picks available to players?");
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

		NumberOfFreeSkillPicksProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The prog {prog.MXPClickableFunctionName()} will now control the number of free picks available to players.");
		return true;
	}

	private bool BuildingCommandCost(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the expression for the total cost of all applied boosts?");
			return false;
		}

		var expression = new Expression(command.SafeRemainingArgument);
		if (expression.HasErrors())
		{
			actor.OutputHandler.Send(expression.Error);
			return false;
		}

		BoostCostExpression = expression;
		Changed = true;
		actor.OutputHandler.Send(
			$"The expression for total cost of applied boosts is now {expression.OriginalExpression.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandResource(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which account resource should be spent to acquire boosts?");
			return false;
		}

		var resource = Gameworld.ChargenResources.GetByIdOrNames(command.SafeRemainingArgument);
		if (resource is null)
		{
			actor.OutputHandler.Send("There is no such account resource.");
			return false;
		}

		BoostResource = resource;
		Changed = true;
		actor.OutputHandler.Send(
			$"The account resource {resource.PluralName.ColourName()} will now be spent to acquire boosts.");
		return true;
	}

	private bool BuildingCommandFreeBoostResource(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"How much free resource should each player get when applying boosts to their character?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must supply a valid number.");
			return false;
		}

		FreeBoostResource = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"Players will now have {value.ToString("N0", actor).ColourValue()} free {BoostResource?.PluralName.ColourName() ?? "Resources".ColourName()} to spend on boosts.");
		return true;
	}

	private bool BuildingCommandMaxBoosts(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much boosts should each skill be allowed to have?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 1)
		{
			actor.OutputHandler.Send("You must supply a valid number that is 1 or greater.");
			return false;
		}

		MaximumBoosts = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"Each skill will now be able to be boosted {value.ToString("N0", actor)} {"time".Pluralise(value != 1)}.");
		return true;
	}

	private bool BuildingCommandBoostCosts(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog do you want to use to control the base price per boost?");
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

		BaseBoostCostProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The prog {prog.MXPClickableFunctionName()} will now be used to control the base cost of boosts.");
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
			ProgVariableTypes.Collection | ProgVariableTypes.Trait, new List<ProgVariableTypes>
			{
				ProgVariableTypes.Chargen
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

	private bool BuildingCommandBoostBlurb(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send(
			$"Replacing the following text:\n\n{SkillBoostBlurb.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}\n\nEnter your new blurb below:\n");
		actor.EditorMode(PostBoostBlurb, CancelBoostBlurb, 1.0, SkillBoostBlurb, EditorOptions.None,
			new object[] { actor.Account.InnerLineFormatLength });
		return true;
	}

	private void CancelBoostBlurb(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the skill boost blurb for this chargen screen.");
	}

	private void PostBoostBlurb(string text, IOutputHandler handler, object[] args)
	{
		SkillBoostBlurb = text;
		Changed = true;
		handler.Send(
			$"You set the skill boost blurb to the following:\n\n{text.Wrap((int)args[0]).SubstituteANSIColour()}");
	}

	private bool BuildingCommandSkillBlurb(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send(
			$"Replacing the following text:\n\n{SkillPickerBlurb.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength)}\n\nEnter your new blurb below:\n");
		actor.EditorMode(PostSkillBlurb, CancelSkillBlurb, 1.0, SkillPickerBlurb, EditorOptions.None,
			new object[] { actor.Account.InnerLineFormatLength });
		return true;
	}

	private void CancelSkillBlurb(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the skill picker blurb for this chargen screen.");
	}

	private void PostSkillBlurb(string text, IOutputHandler handler, object[] args)
	{
		SkillPickerBlurb = text;
		Changed = true;
		handler.Send(
			$"You set the skill picker blurb to the following:\n\n{text.Wrap((int)args[0]).SubstituteANSIColour()}");
	}

	#endregion
}