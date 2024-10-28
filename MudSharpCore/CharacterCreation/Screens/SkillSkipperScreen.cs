using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.CharacterCreation.Screens;

public class SkillSkipperScreenStoryboard : ChargenScreenStoryboard
{
	private SkillSkipperScreenStoryboard()
	{
	}

	protected SkillSkipperScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem) : base(dbitem,
		gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		FreeSkillsProg = Gameworld.FutureProgs.GetByIdOrName(definition.Element("FreeSkillsProg").Value);
	}

	protected SkillSkipperScreenStoryboard(IFuturemud gameworld, IChargenScreenStoryboard storyboard) : base(gameworld,
		storyboard)
	{
		switch (storyboard)
		{
			case SkillPickerScreenStoryboard picker:
				FreeSkillsProg = picker.FreeSkillsProg;
				break;
			case SkillCostPickerScreenStoryboard cost:
				FreeSkillsProg = cost.FreeSkillsProg;
				break;
			case SkillBoostSkipperScreenStoryboard boost:
				FreeSkillsProg = boost.FreeSkillsProg;
				break;
		}

		SaveAfterTypeChange();
	}

	protected override string StoryboardName => "SkillSkipper";

	public override ChargenStage Stage => ChargenStage.SelectSkills;

	public IFutureProg FreeSkillsProg { get; protected set; }

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("FreeSkillsProg", FreeSkillsProg?.Id ?? 0)
		).ToString();
	}

	#endregion

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectSkills,
			new ChargenScreenStoryboardFactory("SkillSkipper",
				(game, dbitem) => new SkillSkipperScreenStoryboard(game, dbitem),
				(game, other) => new SkillSkipperScreenStoryboard(game, other)),
			"SkillSkipper",
			"Skip selecting skills and boosts entirely",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new SkillSkipperScreen(chargen, this);
	}

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine(
			"This screen does not display to the person creating a character, and instead simply skips over the process of selecting skills. You would usually use this screen when your MUD uses some other method of assigning skills to characters, like if they get them based on their class/subclass, or you have set up some kind of role-based storypath system. This screen also does not permit the user to select boosts."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine($"Free Skills Prog: {FreeSkillsProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		return sb.ToString();
	}

	#region Overrides of ChargenScreenStoryboard

	public override string HelpText => $@"{BaseHelpText}
	#3freeskills <prog>#0 - sets the free skills prog";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
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

	#endregion

	public class SkillSkipperScreen : ChargenScreen
	{
		internal SkillSkipperScreen(IChargen chargen, SkillSkipperScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Chargen.SelectedSkills.Clear();
			Chargen.SelectedSkillBoostCosts.Clear();
			Chargen.SelectedSkillBoosts.Clear();
			var freeSkills = storyboard.FreeSkillsProg?.ExecuteCollection<ITraitDefinition>(chargen) ?? new List<ITraitDefinition>();
			Chargen.SelectedSkills.AddRange(freeSkills);
			State = ChargenScreenState.Complete;
		}

		#region Overrides of ChargenScreen

		/// <inheritdoc />
		public override ChargenStage AssociatedStage => ChargenStage.SelectSkills;

		/// <inheritdoc />
		public override string Display()
		{
			return "";
		}

		/// <inheritdoc />
		public override string HandleCommand(string command)
		{
			return "";
		}

		#endregion
	}
}