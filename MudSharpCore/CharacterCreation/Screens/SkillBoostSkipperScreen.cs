using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MailKit.Net.Imap;
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

internal class SkillBoostSkipperScreenStoryboard : ChargenScreenStoryboard
{
	private SkillBoostSkipperScreenStoryboard()
	{
	}

	protected SkillBoostSkipperScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem) : base(
		dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		FreeSkillsProg = Gameworld.FutureProgs.GetByIdOrName(definition.Element("FreeSkillsProg").Value);
		Blurb = definition.Element("SkillBoostBlurb").Value;
		BaseBoostCostProg = Gameworld.FutureProgs.GetByIdOrName(definition.Element("BoostCostProg").Value);
		BoostCostExpression = new Expression(definition.Element("BoostCostExpression").Value);
		BoostResource = Gameworld.ChargenResources.Get(long.Parse(definition.Element("BoostResource").Value));
		MaximumBoosts = int.Parse(definition.Element("MaximumBoosts").Value);
		FreeBoostResource = int.Parse(definition.Element("FreeBoostResource").Value);
	}

	protected SkillBoostSkipperScreenStoryboard(IFuturemud gameworld, IChargenScreenStoryboard storyboard) : base(
		gameworld,
		storyboard)
	{
		BoostCostExpression = new Expression("base * Pow(boosts,2)");
		switch (storyboard)
		{
			case SkillSkipperScreenStoryboard skip:
				FreeSkillsProg = skip.FreeSkillsProg;
				Blurb =
					@"The next step is deciding whether to apply any boosts to your character's starting skills. This is a totally optional process, and costs a large amount of build points. Each character also gets one free boost, so even new players can boost an important skill.  Each skill boost will push your starting skill value up approximately one ""rank"". It is mostly designed so that after the first few characters, when players have started to accumulate some build points, they can avoid some of the starting grind, but ""troll"" players who consistently roll red-shirt characters to try and PK don't get the same leg up.";
				BoostResource = Gameworld.ChargenResources.FirstOrDefault();
				BaseBoostCostProg = Gameworld.FutureProgs.FirstOrDefault(x => x.FunctionName.EqualTo("AlwaysOne"));
				MaximumBoosts = 0;
				FreeBoostResource = 0;
				break;
			case SkillPickerScreenStoryboard picker:
				FreeSkillsProg = picker.FreeSkillsProg;
				Blurb =
					@"The next step is deciding whether to apply any boosts to your character's starting skills. This is a totally optional process, and costs a large amount of build points. Each character also gets one free boost, so even new players can boost an important skill.  Each skill boost will push your starting skill value up approximately one ""rank"". It is mostly designed so that after the first few characters, when players have started to accumulate some build points, they can avoid some of the starting grind, but ""troll"" players who consistently roll red-shirt characters to try and PK don't get the same leg up.";
				BoostResource = Gameworld.ChargenResources.FirstOrDefault();
				BaseBoostCostProg = Gameworld.FutureProgs.FirstOrDefault(x => x.FunctionName.EqualTo("AlwaysOne"));
				MaximumBoosts = 0;
				FreeBoostResource = 0;
				break;
			case SkillCostPickerScreenStoryboard cost:
				FreeSkillsProg = cost.FreeSkillsProg;
				Blurb = cost.SkillBoostBlurb;
				BoostCostExpression =
					new Expression(cost.BoostCostExpression.OriginalExpression);
				BaseBoostCostProg = cost.BaseBoostCostProg;
				BoostResource = cost.BoostResource;
				FreeBoostResource = cost.FreeBoostResource;
				MaximumBoosts = cost.MaximumBoosts;
				break;
		}

		SaveAfterTypeChange();
	}

	protected override string StoryboardName => "SkillBoostSkipper";

	public override ChargenStage Stage => ChargenStage.SelectSkills;

	public IFutureProg FreeSkillsProg { get; protected set; }
	public IFutureProg BaseBoostCostProg { get; protected set; }
	public Expression BoostCostExpression { get; protected set; }
	public IChargenResource BoostResource { get; protected set; }
	public int MaximumBoosts { get; protected set; }
	public int FreeBoostResource { get; protected set; }
	public string Blurb { get; protected set; }

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("SkillBoostBlurb", new XCData(Blurb)),
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
			"This screen does not display to the person creating a character, and instead simply skips over the process of selecting skills. You would usually use this screen when your MUD uses some other method of assigning skills to characters, like if they get them based on their class/subclass, or you have set up some kind of role-based storypath system. This version of the skill skipper does however allow the player to select boosts for the skills that they do get."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine($"Free Skills Prog: {FreeSkillsProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Boost Resource: {BoostResource?.Name.ColourValue() ?? "None".ColourError()}");
		sb.AppendLine($"Maximum Boosts: {MaximumBoosts.ToString("N0", voyeur).ColourValue()}");
		sb.AppendLine($"Free Boost Resource: {FreeBoostResource.ToString("N0", voyeur).ColourValue()}");
		sb.AppendLine($"Base Boost Cost Prog: {BaseBoostCostProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine(
			$"Boost Cost Expression: {BoostCostExpression?.OriginalExpression.ColourCommand() ?? "None".ColourError()}");
		sb.AppendLine();
		sb.AppendLine("Blurb".GetLineWithTitle(voyeur, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(Blurb.Wrap(voyeur.InnerLineFormatLength).SubstituteANSIColour());
		return sb.ToString();
	}

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectSkills,
			new ChargenScreenStoryboardFactory("SkillBoostSkipper",
				(game, dbitem) => new SkillBoostSkipperScreenStoryboard(game, dbitem),
				(game, other) => new SkillBoostSkipperScreenStoryboard(game, other)),
			"SkillBoostSkipper",
			"Skips skill selection but still pick boosts",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new SkillBoostSkipperScreen(chargen, this);
	}

	public override IEnumerable<(IChargenResource Resource, int Cost)> ChargenCosts(IChargen chargen)
	{
		foreach (var resource in Gameworld.ChargenResources)
		{
			var sum = 0;
			if (resource == BoostResource)
			{
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

	public class SkillBoostSkipperScreen : ChargenScreen
	{
		protected Dictionary<ITraitDefinition, int> SelectedBoostCosts;
		protected Dictionary<ITraitDefinition, int> SelectedBoosts;

		protected SkillBoostSkipperScreenStoryboard Storyboard;

		internal SkillBoostSkipperScreen(IChargen chargen, SkillBoostSkipperScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
			Chargen.SelectedSkills.Clear();
			Chargen.SelectedSkillBoostCosts.Clear();
			Chargen.SelectedSkillBoosts.Clear();
			var freeSkills = storyboard.FreeSkillsProg?.ExecuteCollection<ITraitDefinition>(chargen) ?? new List<ITraitDefinition>();
			Chargen.SelectedSkills.AddRange(freeSkills);
			SelectedBoosts = new Dictionary<ITraitDefinition, int>();
			SelectedBoostCosts = new Dictionary<ITraitDefinition, int>();
		}

		private int BoostCostForSkill(ITraitDefinition skill)
		{
			Storyboard.BoostCostExpression.Parameters["base"] =
				(double)((decimal?)Storyboard.BaseBoostCostProg.Execute(skill, Chargen) ?? 1.0M);
			Storyboard.BoostCostExpression.Parameters["boosts"] = SelectedBoosts[skill];
			return Convert.ToInt32(Storyboard.BoostCostExpression.Evaluate());
		}

		#region Overrides of ChargenScreen

		/// <inheritdoc />
		public override ChargenStage AssociatedStage => ChargenStage.SelectSkills;

		/// <inheritdoc />
		public override string Display()
		{
			var sb = new StringBuilder();
			sb.AppendLine("Skill Boosts".Colour(Telnet.Cyan));
			sb.AppendLine();
			sb.AppendLine(Storyboard.Blurb);
			sb.AppendLine();
			sb.AppendLine("You have the following skills:");
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

		/// <inheritdoc />
		public override string HandleCommand(string command)
		{
			if (string.IsNullOrWhiteSpace(command))
			{
				return Display();
			}

			var lcommand = command.ToLowerInvariant();
			if (lcommand == "help")
			{
				return
					@"You can use the following commands on this screen:

	#3done#0 - finish and proceed to the next stage of chargen
	#3reset#0 - reset all your current boosts

The following arguments can all accept an option number of times to do the action:

	#6<skill> [<times>]#0 - boost skill [optionally x times]
	#6-<skill> [<times>]#0 - remove x boosts from skill
	#6*<group> [<times>]#0 - boost all skills in the specified skill group x times.
	#6-*<group> [<times>]#0 - remove x boosts from all skills in the group
	#6all [<times>]#0 - boost all skills x times
	#6-all [<times>]#0 - remove x boosts from all skills".SubstituteANSIColour();
			}

			if (lcommand == "done")
			{
				Chargen.SelectedSkillBoosts = SelectedBoosts;
				State = ChargenScreenState.Complete;
				return "";
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
					return "You do not have any such skill";
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

		#endregion
	}

	#region Building Commands

	public override string HelpText => $@"{BaseHelpText}
	#3blurb#0 - drops you into an editor to change the blurb
	#3freeskills <prog>#0 - sets the free skills prog
	#3baseboostcost <prog>#0 - sets the base boost cost prog
	#3maxboosts <#>#0 - sets the maximum number of total boosts per skill
	#3freeboostresource <#>#0 - sets the free resources available for boosts
	#3resource <which>#0 - sets the resource used to pay for boosts
	#3cost <expression>#0 - sets the expression for the cost of the boosts

Note, the cost expression can use the following parameters:

	#Bbase#0 - the base cost of boosts as determined by the prog
	#Bboosts#0 - the total number of boosts selected by the application";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "blurb":
				return BuildingCommandBlurb(actor, command);
			case "freeskills":
			case "freeskillsprog":
			case "freeskill":
			case "freeskillprog":
			case "skillsprog":
			case "skillprog":
				return BuildingCommandFreeSkills(actor, command);
			case "baseboostcost":
			case "boostcost":
			case "boostscost":
			case "boostcosts":
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
			case "cost":
			case "costs":
				return BuildingCommandCost(actor, command);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
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