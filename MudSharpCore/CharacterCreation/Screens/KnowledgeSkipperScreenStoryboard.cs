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
using MudSharp.RPG.Knowledge;

namespace MudSharp.CharacterCreation.Screens;

internal class KnowledgeSkipperScreenStoryboard : ChargenScreenStoryboard
{
	private KnowledgeSkipperScreenStoryboard()
	{
	}

	private KnowledgeSkipperScreenStoryboard(IFuturemud gameworld, IChargenScreenStoryboard storyboard) : base(gameworld,
		storyboard)
	{
		switch (storyboard)
		{
			case KnowledgePickerBySkillScreenStoryboard picker:
				FreeKnowledgesProg = picker.FreeKnowledgesProg;
				break;
		}

		SaveAfterTypeChange();
	}

	private KnowledgeSkipperScreenStoryboard(IFuturemud gameworld, Models.ChargenScreenStoryboard dbitem)
		: base(dbitem, gameworld)
	{
		var definition = XElement.Parse(dbitem.StageDefinition);
		
		var element = definition.Element("FreeKnowledgesProg");
		FreeKnowledgesProg = long.TryParse(element?.Value ?? "0", out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(element.Value);
	}

	protected override string StoryboardName => "KnowledgePickerBySkill";
	public IFutureProg FreeKnowledgesProg { get; protected set; }

	public override ChargenStage Stage => ChargenStage.SelectKnowledges;

	#region Overrides of ChargenScreenStoryboard

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("FreeKnowledgesProg", FreeKnowledgesProg?.Id ?? 0)
		).ToString();
	}

	#endregion

	public override IChargenScreen GetScreen(IChargen chargen)
	{
		return new KnowledgeSkipperScreen(chargen, this);
	}

	public override IEnumerable<(IChargenResource Resource, int Cost)> ChargenCosts(IChargen chargen)
	{
		return [];
	}

	public static void RegisterFactory()
	{
		ChargenStoryboard.RegisterFactory(ChargenStage.SelectKnowledges,
			new ChargenScreenStoryboardFactory("KnowledgeSkipper",
				(game, dbitem) => new KnowledgeSkipperScreenStoryboard(game, dbitem),
				(game, other) => new KnowledgeSkipperScreenStoryboard(game, other)
				),
			"KnowledgeSkipper",
			"Skip the screen and give knowledges by prog",
			((ChargenScreenStoryboard)Activator.CreateInstance(MethodBase.GetCurrentMethod().DeclaringType, true))
			.HelpText);
	}

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.Append(ShowHeader(voyeur));
		sb.AppendLine();
		sb.AppendLine(
			"This screen does not display and instead just automatically gives a character knowledges."
				.Wrap(voyeur.InnerLineFormatLength).ColourCommand());
		sb.AppendLine();
		sb.AppendLine(
			$"Free Knowledges Prog: {FreeKnowledgesProg?.MXPClickableFunctionNameWithId() ?? "None".ColourError()}");
	
		return sb.ToString();
	}

	internal class KnowledgeSkipperScreen : ChargenScreen
	{
		protected KnowledgeSkipperScreenStoryboard Storyboard;

		internal KnowledgeSkipperScreen(IChargen chargen, KnowledgeSkipperScreenStoryboard storyboard)
			: base(chargen, storyboard)
		{
			Storyboard = storyboard;
			ResetChargenScreen();
			State = ChargenScreenState.Complete;
		}

		public override ChargenStage AssociatedStage => Storyboard.Stage;

		public void ResetChargenScreen()
		{
			Chargen.SelectedKnowledges.Clear();
			var knowledges =
				Storyboard.FreeKnowledgesProg?.ExecuteCollection<IKnowledge>(Chargen)
				.ToList() ?? [];
			Chargen.SelectedKnowledges.AddRange(knowledges);
		}


		public override string Display()
		{
			return "";
		}

		public override string HandleCommand(string command)
		{
			return "";
		}
	}

	#region Building Commands

	public override string HelpText => $@"{BaseHelpText}
	#3free <prog>#0 - sets the prog for free knowledges";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "free":
				return BuildingCommandFree(actor, command);
		}

		return BuildingCommandFallback(actor, command.GetUndo());
	}

	private bool BuildingCommandFree(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog should control the knowledges a character gets for free?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Collection | ProgVariableTypes.Knowledge, new List<ProgVariableTypes>
			{
				ProgVariableTypes.Chargen
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		FreeKnowledgesProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"The prog {prog.MXPClickableFunctionName()} will now be used to control the knowledges a character gets for free in character creation.");
		return true;
	}
	#endregion
}
