using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class SelectableOption
{
	public SelectableOption()
	{
	}

	public SelectableOption(XElement definition, IFuturemud gameworld)
	{
		CanSelectProg = gameworld.FutureProgs.Get(long.Parse(definition.Element("CanSelectProg")?.Value ?? "0"));
		OnSelectProg = gameworld.FutureProgs.Get(long.Parse(definition.Element("OnSelectProg")?.Value ?? "0"));
		Keyword = definition.Element("Keyword")?.Value ?? "";
		Description = definition.Element("Description")?.Value ?? "";
	}

	public IFutureProg CanSelectProg { get; set; }
	public IFutureProg OnSelectProg { get; set; }
	public string Keyword { get; set; }
	public string Description { get; set; }

	public XElement SaveDefinition()
	{
		return new XElement("Option",
			new XElement("CanSelectProg", CanSelectProg?.Id ?? 0),
			new XElement("OnSelectProg", OnSelectProg?.Id ?? 0),
			new XElement("Keyword", new XCData(Keyword)),
			new XElement("Description", new XCData(Keyword)));
	}
}

public class SelectableGameItemComponentProto : GameItemComponentProto
{
	protected SelectableGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Selectable")
	{
	}

	protected SelectableGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public List<SelectableOption> Options { get; } = new();
	public override string TypeDescription => "Selectable";

	protected override void LoadFromXml(XElement root)
	{
		foreach (var option in root.Elements("Option"))
		{
			Options.Add(new SelectableOption(option, Gameworld));
		}
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tadd <keyword> <canselectprog> <onselectprog> <description> - adds a new option\n\tremove <keyword> - removes the option specified by keyword\n\tswap <keyword1> <keyword2> - swaps the position of two options in the display";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Pop().ToLowerInvariant())
		{
			case "add":
				return BuildingCommandAdd(actor, command);
			case "rem":
			case "remove":
				return BuildingCommandRemove(actor, command);
			case "swap":
				return BuildingCommandSwap(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What keyword do you want to give this selection?");
			return false;
		}

		var keyword = command.Pop().ToLowerInvariant();
		if (Options.Any(x => x.Keyword.Equals(keyword, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.Send(
				"There is already a selectable option with that keyword. You must remove the other one first.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.Send("What CanSelectProg do you want to use for this option?");
			return false;
		}

		var canSelectProg = long.TryParse(command.Pop(), out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (canSelectProg == null)
		{
			actor.Send("There is no such prog as that to use for the CanSelectProg.");
			return false;
		}

		if (canSelectProg.ReturnType != FutureProgVariableTypes.Boolean)
		{
			actor.Send("The CanSelectProg must return a boolean.");
			return false;
		}

		if (
			!canSelectProg.MatchesParameters(new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Character,
				FutureProgVariableTypes.Item
			}))
		{
			actor.Send("The CanSelectProg must accept a single character and item parameter.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.Send("What OnSelectProg do you want to use for this option?");
			return false;
		}

		var onSelectProg = long.TryParse(command.Pop(), out value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (onSelectProg == null)
		{
			actor.Send("There is no such prog as that to use for the OnSelectProg.");
			return false;
		}

		if (
			!onSelectProg.MatchesParameters(new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Character,
				FutureProgVariableTypes.Item
			}))
		{
			actor.Send("The OnSelectProg must accept a single character and item parameter.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.Send("What description of this option do you want to appear when the item is looked at?");
			return false;
		}

		var newOption = new SelectableOption
		{
			CanSelectProg = canSelectProg,
			OnSelectProg = onSelectProg,
			Description = command.SafeRemainingArgument.ProperSentences().Trim().SubstituteANSIColour(),
			Keyword = keyword
		};

		Options.Add(newOption);
		Changed = true;
		actor.Send(
			$"You add the following option: {newOption.Keyword.Colour(Telnet.Yellow)} - CanSelect: {newOption.CanSelectProg?.FunctionName ?? "None"} ({newOption.CanSelectProg?.Id ?? 0:N0}) - OnSelect: {newOption.OnSelectProg?.FunctionName ?? "None"} ({newOption.OnSelectProg?.Id ?? 0:N0}) - {newOption.Description}");
		return true;
	}

	private bool BuildingCommandRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which option do you want to remove?");
			return false;
		}

		var which = command.PopSpeech();
		var option = Options.FirstOrDefault(x => x.Keyword.EqualTo(which)) ?? Options.FirstOrDefault(x =>
			x.Keyword.StartsWith(which, StringComparison.InvariantCultureIgnoreCase));
		if (option == null)
		{
			actor.Send("There is no such option to remove.");
			return false;
		}

		Options.Remove(option);
		Changed = true;
		actor.OutputHandler.Send(
			$"You remove option {option.Keyword.Colour(Telnet.Yellow)} ({option.Description.Colour(Telnet.Cyan)}).");
		return true;
	}

	private bool BuildingCommandSwap(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which two options do you want to swap?");
			return false;
		}

		var which1 = command.PopSpeech();
		if (command.IsFinished)
		{
			actor.Send("Which two options do you want to swap?");
			return false;
		}

		var which2 = command.PopSpeech();

		var option1 = Options.FirstOrDefault(x => x.Keyword.EqualTo(which1)) ?? Options.FirstOrDefault(x =>
			x.Keyword.StartsWith(which1, StringComparison.InvariantCultureIgnoreCase));
		var option2 = Options.FirstOrDefault(x => x.Keyword.EqualTo(which2)) ?? Options.FirstOrDefault(x =>
			x.Keyword.StartsWith(which2, StringComparison.InvariantCultureIgnoreCase));
		if (option1 == null)
		{
			actor.Send("There is no such option as the first one you specified.");
			return false;
		}

		if (option2 == null)
		{
			actor.Send("There is no such option as the second one you specified.");
			return false;
		}

		if (option1 == option2)
		{
			actor.Send("You cannot swap an option with itself.");
			return false;
		}

		Options.Swap(option1, option2);
		Changed = true;
		actor.OutputHandler.Send(
			$"You swap the positions of option {option1.Keyword.Colour(Telnet.Yellow)} ({option1.Description.Colour(Telnet.Cyan)}) and {option2.Keyword.Colour(Telnet.Yellow)} ({option2.Description.Colour(Telnet.Cyan)}).");
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis has options that can be selected. It has the following options:\n{4}",
			"Selectable Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Options.Select(
				       x =>
					       $"Option {x.Keyword.Colour(Telnet.Yellow)} - CanSelect: {x.CanSelectProg?.FunctionName ?? "None"} ({x.CanSelectProg?.Id ?? 0:N0}) - OnSelect: {x.OnSelectProg?.FunctionName ?? "None"} ({x.OnSelectProg?.Id ?? 0:N0}) - {x.Description}")
			       .ListToString("\t", "\n", "", "\n")
		);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			from option in Options
			select option.SaveDefinition()
		).ToString();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("selectable", true,
			(gameworld, account) => new SelectableGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Selectable",
			(proto, gameworld) => new SelectableGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Selectable",
			$"The item can have options for the {"[select]".Colour(Telnet.Yellow)} command which fire progs when selected",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new SelectableGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new SelectableGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new SelectableGameItemComponentProto(proto, gameworld));
	}
}