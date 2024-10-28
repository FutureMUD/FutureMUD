using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Models;
using System.Text;
using MudSharp.PerceptionEngine;

namespace MudSharp.RPG.Dreams;

public class Dream : SaveableItem, IDream
{
	public Dream(MudSharp.Models.Dream dream, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = dream.Id;
		_name = dream.Name;
		_onceOnly = dream.OnlyOnce;
		Priority = dream.Priority;
		_characters.AddRange(dream.DreamsCharacters.Select(x => x.CharacterId).ToList());
		_haveDreamedBefore.AddRange(dream.DreamsAlreadyDreamt.Select(x => x.CharacterId).ToList());
		_dreamStages.AddRange(dream.DreamPhases.OrderBy(x => x.PhaseId).Select(x => new DreamStage(x)).ToList());
		CanDreamProg = gameworld.FutureProgs.Get(dream.CanDreamProgId ?? 0);
		OnDreamProg = gameworld.FutureProgs.Get(dream.OnDreamProgId ?? 0);
		OnWakeDuringDreamProg = gameworld.FutureProgs.Get(dream.OnWakeDuringDreamingProgId ?? 0);
	}

	public Dream(string name, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_name = name;
		_onceOnly = false;
		Priority = 1;
		using (new FMDB())
		{
			var dbitem = new Models.Dream
			{
				Name = name,
				OnlyOnce = _onceOnly,
				Priority = Priority
			};
			FMDB.Context.Dreams.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public IDream Clone(string newName)
	{
		using (new FMDB())
		{
			var dbitem = new Models.Dream
			{
				Name = newName,
				OnlyOnce = OnceOnly,
				Priority = Priority,
				CanDreamProgId = CanDreamProg?.Id,
				OnDreamProgId = OnDreamProg?.Id,
				OnWakeDuringDreamingProgId = OnWakeDuringDreamProg?.Id
			};
			foreach (var stage in DreamStages)
			{
				dbitem.DreamPhases.Add(new DreamPhase
				{
					Dream = dbitem,
					DreamerCommand = stage.DreamerCommand,
					DreamerText = stage.DreamerText,
					PhaseId = stage.PhaseID,
					WaitSeconds = stage.WaitSeconds
				});
			}

			FMDB.Context.Dreams.Add(dbitem);
			FMDB.Context.SaveChanges();
			return new Dream(dbitem, Gameworld);
		}
	}

	#region Overrides of Item

	public override string FrameworkItemType { get; } = "Dream";

	#endregion

	#region Overrides of SaveableItem

	/// <summary>
	///     Tells the object to perform whatever save action it needs to do
	/// </summary>
	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.Dreams.Find(Id);
			dbitem.Name = Name;
			dbitem.CanDreamProgId = CanDreamProg?.Id;
			dbitem.OnDreamProgId = OnDreamProg?.Id;
			dbitem.OnWakeDuringDreamingProgId = OnWakeDuringDreamProg?.Id;
			dbitem.OnlyOnce = _onceOnly;
			dbitem.Priority = Priority;
			FMDB.Context.DreamPhases.RemoveRange(dbitem.DreamPhases);
			foreach (var phase in DreamStages)
			{
				var dbphase = new DreamPhase();
				dbitem.DreamPhases.Add(dbphase);
				dbphase.DreamerCommand = phase.DreamerCommand;
				dbphase.DreamerText = phase.DreamerText;
				dbphase.WaitSeconds = phase.WaitSeconds;
				dbphase.PhaseId = phase.PhaseID;
			}

			FMDB.Context.DreamsCharacters.RemoveRange(dbitem.DreamsCharacters);
			foreach (var character in _characters)
			{
				dbitem.DreamsCharacters.Add(new DreamsCharacters { Dream = dbitem, CharacterId = character });
			}

			FMDB.Context.DreamsAlreadyDreamt.RemoveRange(dbitem.DreamsAlreadyDreamt);
			foreach (var character in _haveDreamedBefore)
			{
				dbitem.DreamsAlreadyDreamt.Add(new DreamsAlreadyDreamt { Dream = dbitem, CharacterId = character });
			}

			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	#endregion

	#region Implementation of IDream

	public void GiveDream(ICharacter character)
	{
		if (!_characters.Contains(character.Id))
		{
			_characters.Add(character.Id);
			_haveDreamedBefore.Remove(character.Id);
			Changed = true;
		}
	}

	public void RemoveDream(ICharacter character)
	{
		if (_characters.Contains(character.Id))
		{
			_characters.Remove(character.Id);
			Changed = true;
		}
	}

	private readonly List<DreamStage> _dreamStages = new();
	public IEnumerable<DreamStage> DreamStages => _dreamStages;

	private readonly List<long> _characters = new();
	private readonly List<long> _haveDreamedBefore = new();

	private bool _onceOnly;
	public bool OnceOnly => _onceOnly;

	public bool CanDream(ICharacter character)
	{
		return (!_onceOnly || !_haveDreamedBefore.Contains(character.Id)) &&
		       (_characters.Contains(character.Id) || ((bool?)CanDreamProg?.Execute(character) ?? true));
	}

	public void FinishDream(ICharacter character)
	{
		if (_onceOnly && !_haveDreamedBefore.Contains(character.Id))
		{
			_haveDreamedBefore.Add(character.Id);
		}

		OnDreamProg?.Execute(character, Id);
		if (!character.EffectsOfType<INoDreamEffect>().Any())
		{
			character.AddEffect(new NoDreamEffect(character), TimeSpan.FromSeconds(120));
		}
	}

	public IFutureProg CanDreamProg { get; set; }
	public IFutureProg OnWakeDuringDreamProg { get; set; }
	public IFutureProg OnDreamProg { get; set; }
	public int Priority { get; set; }

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Dream #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine($"Once Only: {_onceOnly.ToColouredString()}");
		sb.AppendLine($"Priority Weighting: {Priority.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Can Dream Prog: {CanDreamProg?.MXPClickableFunctionName() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"On Dream Prog: {OnDreamProg?.MXPClickableFunctionName() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"On Wake During Prog: {OnWakeDuringDreamProg?.MXPClickableFunctionName() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine();
		sb.AppendLine("Stages:");
		foreach (var stage in _dreamStages)
		{
			sb.AppendLine();
			sb.AppendLine(
				$"Stage {stage.PhaseID.ToString("N0", actor)} - {stage.WaitSeconds.ToString("N0", actor)} {"second".Pluralise(stage.WaitSeconds != 1)}{(!string.IsNullOrEmpty(stage.DreamerCommand) ? $" - Command: {stage.DreamerCommand.ColourCommand()}" : "")}"
					.Colour(Telnet.Cyan));
			sb.AppendLine();
			sb.AppendLine(stage.DreamerText.Wrap(actor.InnerLineFormatLength));
		}

		return sb.ToString();
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "once":
				return BuildingCommandOnce(actor);
			case "priority":
			case "weight":
			case "weighting":
			case "chance":
				return BuildingCommandPriority(actor, command);
			case "candreamprog":
			case "candream":
			case "can":
			case "canprog":
				return BuildingCommandCanDreamProg(actor, command);
			case "ondreamprog":
			case "ondream":
			case "on":
			case "onprog":
				return BuildingCommandOnDreamProg(actor, command);
			case "onwakeprog":
			case "onwake":
			case "wake":
			case "wakeprog":
				return BuildingCommandOnWakeProg(actor, command);
			case "phase":
			case "stage":
				return BuildingCommandStage(actor, command);
			default:
				actor.OutputHandler.Send(@"");
				return false;
		}
	}

	private bool BuildingCommandStage(ICharacter actor, StringStack command)
	{
		var arg = command.PopForSwitch();
		switch (arg)
		{
			case "add":
			case "new":
			case "create":
				return BuildingCommandStageNew(actor, command);
			case "delete":
			case "del":
			case "remove":
			case "rem":
			case "swap":
			case "command":
			case "text":
			case "delay":
			case "time":
			case "seconds":
				break;
			default:
				actor.OutputHandler.Send(@"You can use the following options with the stage command:

    add - drops you into an editor to add a new stage
    remove <#> - removes a stage
    swap <#1> <#2> - swaps the order of two stages
    command <#> <command> - sets a command to have the dreamer execute at that stage
    delay <#> <seconds> - sets the delay between this stage and the next stage
    text <#> - drops you into an editor to edit a stage");
				return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify which dream stage you want to edit.");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value < 1 || value > _dreamStages.Count)
		{
			actor.OutputHandler.Send(
				$"You must specify a value between {1.ToString("N0", actor)} and {_dreamStages.Count.ToString("N0", actor)}.");
			return false;
		}

		switch (arg)
		{
			case "delete":
			case "del":
			case "remove":
			case "rem":
				return BuildingCommandRemove(actor, value, command);
			case "swap":
				return BuildingCommandSwap(actor, value, command);
			case "command":
				return BuildingCommandCommand(actor, value, command);
			case "text":
				return BuildingCommandText(actor, value);
			case "delay":
			case "time":
			case "seconds":
				return BuildingCommandDelay(actor, value, command);
		}

		return false;
	}

	private bool BuildingCommandDelay(ICharacter actor, int value, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many seconds should be between that stage and the next stage's echo?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var seconds) || value < 1 || value > 120)
		{
			actor.OutputHandler.Send(
				$"You must enter a valid number of seconds between {1.ToString("N0", actor)} and {120.ToString("N0", actor)}.");
			return false;
		}

		_dreamStages[value - 1].WaitSeconds = seconds;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {value.ToOrdinal()} stage will now have a delay of {seconds.ToString("N0", actor)} {"second".Pluralise(seconds != 1)} between it and the next stage.");
		return true;
	}

	private bool BuildingCommandText(ICharacter actor, int value)
	{
		var existingOptions = _dreamStages
		                      .Select(x => x.DreamerText)
		                      .SelectMany(x => Dreaming.DreamRegex.Matches(x))
		                      .Where(x => x.Groups["linked"].Length > 0)
		                      .Select(x => (LinkNum: int.Parse(x.Groups["linknum"].Value),
			                      Options: x.Groups["options"].Value.Split("|").Length))
		                      .ToList();
		var sb = new StringBuilder();
		sb.AppendLine("Please enter the text that will be sent to the dreamer in the editor below.");
		sb.AppendLine();
		sb.AppendLine("Replacing:");
		sb.AppendLine(_dreamStages[value - 1].DreamerText.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine();
		sb.AppendLine(
			$"You can use the syntax {"{some text|some other text|a third option}".ColourCommand()} to add random elements.");
		sb.AppendLine(
			$"You can use the syntax {"*x{some text|some other text|a third option}".ColourCommand()} where x is a number.");
		sb.AppendLine(
			"If you use this second option in multiple places or phases, the choice presented will be the same position in your list of options each time.");
		if (existingOptions.Any())
		{
			sb.AppendLine($"The dream already has the following persistent references:");
			foreach (var option in existingOptions.OrderBy(x => x.LinkNum))
			{
				sb.AppendLine(
					$"\t*{option.LinkNum.ToString("F0", actor)} - {option.Options.ToString("N0", actor).ColourValue()} options");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
		actor.EditorMode(PostDreamPhaseText, CancelDreamPhaseText, 1.0, _dreamStages[value - 1].DreamerText,
			Editor.EditorOptions.None, new object[] { _dreamStages[value - 1] });
		return true;
	}

	private void CancelDreamPhaseText(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to edit the text of that dream stage.");
	}

	private void PostDreamPhaseText(string text, IOutputHandler handler, object[] args)
	{
		var stage = (DreamStage)args[0];
		stage.DreamerText = text;
		Changed = true;
		handler.Send("You change the text of that stage.");
	}

	private bool BuildingCommandCommand(ICharacter actor, int value, StringStack command)
	{
		_dreamStages[value - 1].DreamerCommand = command.SafeRemainingArgument.ParseSpecialCharacters();
		Changed = true;
		if (string.IsNullOrEmpty(command.SafeRemainingArgument))
		{
			actor.OutputHandler.Send(
				$"The {value.ToOrdinal()} stage will no longer have the dreamer execute any commands.");
		}
		else
		{
			actor.OutputHandler.Send(
				$"The {value.ToOrdinal()} stage will now execute the following commands:\n\n{_dreamStages[value - 1].DreamerCommand.ColourCommand()}");
		}

		return true;
	}

	private bool BuildingCommandSwap(ICharacter actor, int value1, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What other dream stage do you want to swap the order of that one with?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value2) || value2 < 1 || value2 > _dreamStages.Count)
		{
			actor.OutputHandler.Send(
				$"You must specify a value between {1.ToString("N0", actor)} and {_dreamStages.Count.ToString("N0", actor)} for the second value.");
			return false;
		}

		if (value1 == value2)
		{
			actor.OutputHandler.Send("You cannot swap a stage with itself.");
			return false;
		}

		_dreamStages.Swap(value1 - 1, value2 - 1);
		Changed = true;
		actor.OutputHandler.Send(
			$"You swap the order of the {value1.ToOrdinal()} and {value2.ToOrdinal()} dream stages.");
		return true;
	}

	private bool BuildingCommandRemove(ICharacter actor, int value, StringStack command)
	{
		var stage = _dreamStages[value - 1];
		actor.OutputHandler.Send(
			$"Are you sure you want to permanently remove the following dream stage:\n\n{stage.DreamerText.Wrap(actor.InnerLineFormatLength, "\t")}\n\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				_dreamStages.Remove(stage);
				Changed = true;
				actor.OutputHandler.Send($"You remove the dream stage.");
			},
			RejectAction = text => { actor.OutputHandler.Send("You decide not to delete the dream stage."); },
			ExpireAction = () => { actor.OutputHandler.Send("You decide not to delete the dream stage."); },
			Keywords = new List<string> { "remove", "dream", "stage" },
			DescriptionString = "deleting a dream stage"
		}), TimeSpan.FromSeconds(120));
		return true;
	}

	private bool BuildingCommandStageNew(ICharacter actor, StringStack command)
	{
		var existingOptions = _dreamStages
		                      .Select(x => x.DreamerText)
		                      .SelectMany(x => Dreaming.DreamRegex.Matches(x))
		                      .Where(x => x.Groups["linked"].Length > 0)
		                      .Select(x => (LinkNum: int.Parse(x.Groups["linknum"].Value),
			                      Options: x.Groups["options"].Value.Split("|").Length))
		                      .ToList();
		var sb = new StringBuilder();
		sb.AppendLine("Please enter the text that will be sent to the dreamer in the editor below.");
		sb.AppendLine(
			$"You can use the syntax {"{some text|some other text|a third option}".ColourCommand()} to add random elements.");
		sb.AppendLine(
			$"You can use the syntax {"*x{some text|some other text|a third option}".ColourCommand()} where x is a number.");
		sb.AppendLine(
			"If you use this second option in multiple places or phases, the choice presented will be the same position in your list of options each time.");
		if (existingOptions.Any())
		{
			sb.AppendLine($"The dream already has the following persistent references:");
			foreach (var option in existingOptions.OrderBy(x => x.LinkNum))
			{
				sb.AppendLine(
					$"\t*{option.LinkNum.ToString("F0", actor)} - {option.Options.ToString("N0", actor).ColourValue()} options");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
		actor.EditorMode(PostDreamPhaseAdd, CancelDreamPhaseAdd, 1.0, string.Empty, Editor.EditorOptions.None);
		return true;
	}

	private void CancelDreamPhaseAdd(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to add a dream stage.");
	}

	private void PostDreamPhaseAdd(string text, IOutputHandler handler, object[] args)
	{
		var stage = new DreamStage
		{
			DreamerText = text,
			PhaseID = DreamStages.Select(x => x.PhaseID).DefaultIfEmpty(0).Max() + 1,
			DreamerCommand = string.Empty,
			WaitSeconds = 20
		};
		_dreamStages.Add(stage);
		Changed = true;
		handler.Send($"You add a new dream stage, the {_dreamStages.Count.ToOrdinal()} in order.");
	}

	private bool BuildingCommandOnWakeProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a prog to assign, or use the keyword {"clear".ColourCommand()} to clear it.");
			return false;
		}

		if (command.Peek().EqualToAny("clear", "none", "remove"))
		{
			OnWakeDuringDreamProg = null;
			Changed = true;
			actor.OutputHandler.Send(
				$"This dream will no longer launch any prog when the dreamer wakes up during the dream.");
			return true;
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Character }) &&
		    !prog.MatchesParameters(new List<ProgVariableTypes>
			    { ProgVariableTypes.Character, ProgVariableTypes.Number }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that takes either a single character or a character and a number as a parameter, whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		OnWakeDuringDreamProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This dream will now launch the {prog.MXPClickableFunctionNameWithId()} prog when the dreamer wakes up during the dream.");
		return true;
	}

	private bool BuildingCommandOnDreamProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a prog to assign, or use the keyword {"clear".ColourCommand()} to clear it.");
			return false;
		}

		if (command.Peek().EqualToAny("clear", "none", "remove"))
		{
			OnDreamProg = null;
			Changed = true;
			actor.OutputHandler.Send(
				$"This dream will no longer launch any prog when the dreamer completes the dream.");
			return true;
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Character }) &&
		    !prog.MatchesParameters(new List<ProgVariableTypes>
			    { ProgVariableTypes.Character, ProgVariableTypes.Number }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that takes either a single character or a character and a number as a parameter, whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		OnDreamProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This dream will now launch the {prog.MXPClickableFunctionNameWithId()} prog when the dreamer completes the dream.");
		return true;
	}

	private bool BuildingCommandCanDreamProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a prog to assign, or use the keyword {"clear".ColourCommand()} to clear it.");
			return false;
		}

		if (command.Peek().EqualToAny("clear", "none", "remove"))
		{
			CanDreamProg = null;
			Changed = true;
			actor.OutputHandler.Send(
				$"This dream will no longer use a prog to determine who can have this dream, and will instead rely on dreamers being manually assigned.");
			return true;
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that takes a single character as a parameter, whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		CanDreamProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This dream will now use the {prog.MXPClickableFunctionNameWithId()} prog to determine who can have the dream.");
		return true;
	}

	private bool BuildingCommandPriority(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("What numerical weighting should this dream have in random dream selection?");
			return false;
		}

		Priority = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This dream will now have a weighting of {value.ToString("N0", actor).ColourValue()} for the purposes of random selection of dreams.");
		return true;
	}

	private bool BuildingCommandOnce(ICharacter actor)
	{
		_onceOnly = !_onceOnly;
		Changed = true;
		actor.OutputHandler.Send($"This dream will {(_onceOnly ? "now" : "no longer")} be dreamt only one time.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this dream?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.Dreams.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a dream with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename this dream from {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	#endregion
}