using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Commands.Socials;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.Commands;

public class CommandManager : ICommandManager
{
	private readonly Dictionary<string, IExecutable> _commands = new();
	public IReadOnlyDictionary<string, IExecutable> Commands => _commands;

	public CommandManager(string failedToFindCommandString = "",
		PermissionLevel hasPermissionUpTo = PermissionLevel.Any)
	{
		FailedToFindCommand = failedToFindCommandString;
		HasPermissionUpTo = hasPermissionUpTo;
	}

	public string Name { get; protected set; }

	public PermissionLevel PermissionRequired => PermissionLevel.Any;

	public PermissionLevel HasPermissionUpTo { get; protected set; }

	public string FailedToFindCommand { get; init; }

	public bool ConsumesUsedCommand => true;

	public CharacterState AllowedStates => CharacterState.Any;

	public void Add(string entry, IExecutable.CommandMethodDelegate commandMethod,
		CharacterState state = CharacterState.Any,
		PermissionLevel permissionRequired = PermissionLevel.Any)
	{
		if (HasPermissionUpTo < permissionRequired)
		{
			return;
		}

		Add(entry, new Command(commandMethod, state, permissionRequired));
	}

	public void Add(IEnumerable<string> entries, IExecutable.CommandMethodDelegate commandMethod,
		CharacterState state = CharacterState.Any,
		PermissionLevel permissionRequired = PermissionLevel.Any)
	{
		if (HasPermissionUpTo < permissionRequired)
		{
			return;
		}

		Add(entries, new Command(commandMethod, state, permissionRequired));
	}

	public void Add(string entry, IExecutable command)
	{
		if (HasPermissionUpTo < command.PermissionRequired)
		{
			return;
		}

		if (_commands.ContainsKey(entry))
		{
			return;
		}

		_commands.Add(entry, command);
	}

	public void Add(IEnumerable<string> entries, IExecutable command)
	{
		if (HasPermissionUpTo < command.PermissionRequired)
		{
			return;
		}

		var list = entries.ToList();
		for (var i = list.Count - 1; i > -1; i--)
		{
			if (!_commands.ContainsKey(list[i]))
			{
				_commands.Add(list[i], command);
			}
			else
			{
				list.RemoveAt(i);
			}
		}
	}

	public void AddFrom(ICommandManager other)
	{
		foreach (
			var command in other.Commands.Where(command => HasPermissionUpTo >= command.Value.PermissionRequired))
		{
			_commands.Add(command.Key, command.Value);
		}
	}

	public void Override(string entry, IExecutable command)
	{
		if (_commands == null)
		{
			return;
		}

		_commands[entry] = command;
	}

	public void Override(string[] entries, IExecutable command)
	{
		if (_commands == null)
		{
			return;
		}

		foreach (var entry in entries)
		{
			_commands[entry] = command;
		}
	}

	public bool Remove(string entry, IExecutable command)
	{
		return _commands?.Remove(entry) == true;
	}

	public bool Remove(IEnumerable<string> entries, IExecutable command)
	{
		return _commands != null && entries.Aggregate(false, (current, entry) => current | _commands.Remove(entry));
	}

	public bool Execute(string playerInput, CharacterState state = CharacterState.Any,
		PermissionLevel permissionLevel = PermissionLevel.Any, IOutputHandler outputHandler = null)
	{
		var command = LocateCommand(ref playerInput);

		if (command != null)
		{
			return command.Execute(playerInput, state, permissionLevel, outputHandler);
		}

		outputHandler?.Send(FailedToFindCommand);
		return false;
	}

	public IEnumerable<string> ReportCommands(PermissionLevel authority = PermissionLevel.Any)
	{
		return null;
	}

	protected IExecutable LocateCommand(ref string input)
	{
		var splitCommands = new StringStack(input);

		_commands.TryGetValue(splitCommands.Pop().ToLowerInvariant(), out var command);

		if (command == null)
		{
			_commands.TryGetValue(input.ToLowerInvariant(), out command);
		}

		if (command == null && !string.IsNullOrWhiteSpace(splitCommands.Last))
		{
			var abbreviation =
				_commands.OrderBy(x => x.Key)
				         .FirstOrDefault(
					         x => x.Key.StartsWith(splitCommands.Last, StringComparison.InvariantCultureIgnoreCase));
			command = abbreviation.IsDefault() ? null : abbreviation.Value;
		}

		if (command?.ConsumesUsedCommand == true)
		{
			input = splitCommands.RemainingArgument.Length > 1 ? splitCommands.RemainingArgument : "";
		}

		return command;
	}

	public override string ToString()
	{
		var sb = new StringBuilder();

		return sb.ToString();
	}
}

public class CommandManager<T> : ICommandManager<T>
{
	protected readonly Dictionary<string, IExecutable<T>> _tCommands = new();
	public IReadOnlyDictionary<string, IExecutable<T>> TCommands => _tCommands;

	public CommandManager(string failedToFindCommandString = "",
		PermissionLevel hasPermissionUpTo = PermissionLevel.Any)
	{
		HasPermissionUpTo = hasPermissionUpTo;
	}

	public string Name { get; protected set; }

	public string ModuleName { get; protected set; }

	public PermissionLevel PermissionRequired => PermissionLevel.Any;

	public CommandDisplayOptions DisplayOptions { get; protected set; }

	public PermissionLevel HasPermissionUpTo { get; protected set; }

	private string _failedToFindCommand;

	public string FailedToFindCommand
	{
		get
		{
			if (_failedToFindCommand == null)
			{
				_failedToFindCommand = Futuremud.Games.First().GetStaticString("FailedToFindCommand");
			}

			return _failedToFindCommand;
		}
	}

	public bool ConsumesUsedCommand => true;

	public CharacterState AllowedStates => CharacterState.Any;

	public string DelayErrorMessage => string.Empty;

	public string DelayType => null;
	public string DelayExceptionType => null;

	public bool NoHideCommand => false;

	public bool NoCombatCommand => false;

	public bool NoMovementCommand => false;

	public bool NoMeleeCombatCommand => false;

	public bool MustBeAnEnforcer => false;

	public ICommandHelpInfo HelpInfo => null;

	public Func<object, string, bool> AppearInCommandsListCondition => null;

	public void Add(string entry, IExecutable<T>.CommandGenericMethodDelegate commandGenericMethod,
		CharacterState state = CharacterState.Any,
		PermissionLevel permissionRequired = PermissionLevel.Any)
	{
		if (HasPermissionUpTo < permissionRequired)
		{
			return;
		}

		Add(entry, new Command<T>(commandGenericMethod, state, permissionRequired));
	}

	public void Add(IEnumerable<string> entries, IExecutable<T>.CommandGenericMethodDelegate commandGenericMethod,
		CharacterState state = CharacterState.Any,
		PermissionLevel permissionRequired = PermissionLevel.Any)
	{
		if (HasPermissionUpTo < permissionRequired)
		{
			return;
		}

		Add(entries, new Command<T>(commandGenericMethod, state, permissionRequired));
	}

	public void Add(string entry, IExecutable<T> command)
	{
		if (HasPermissionUpTo < command.PermissionRequired)
		{
			return;
		}

		if (_tCommands.ContainsKey(entry))
		{
			return;
		}

		_tCommands.Add(entry, command);
	}

	public void Add(IEnumerable<string> entries, IExecutable<T> command)
	{
		if (HasPermissionUpTo < command.PermissionRequired)
		{
			return;
		}

		var list = entries.ToList();
		for (var i = list.Count - 1; i > -1; i--)
		{
			if (!_tCommands.ContainsKey(list[i]))
			{
				_tCommands.Add(list[i], command);
			}
			else
			{
				list.RemoveAt(i);
			}
		}
	}

	public void AddFrom(ICommandManager<T> other)
	{
		foreach (
			var command in other.TCommands.Where(command => HasPermissionUpTo >= command.Value.PermissionRequired))
		{
			_tCommands.Add(command.Key, command.Value);
		}
	}

	public void Override(string entry, IExecutable<T> command)
	{
		_tCommands[entry] = command;
	}

	public void Override(string[] entries, IExecutable<T> command)
	{
		foreach (var entry in entries)
		{
			_tCommands[entry] = command;
		}
	}

	public bool Remove(string entry, IExecutable<T> command)
	{
		return _tCommands.Remove(entry);
	}

	public bool Remove(IEnumerable<string> entries, IExecutable<T> command)
	{
		return entries.Aggregate(false, (current, entry) => current | _tCommands.Remove(entry));
	}

	public virtual bool Execute(T argument, string playerInput, CharacterState state = CharacterState.Any,
		PermissionLevel permissionLevel = PermissionLevel.Any, IOutputHandler outputHandler = null)
	{
		var command = LocateCommand(argument, ref playerInput);

		if (command != null)
		{
			return command.Execute(argument, playerInput, state, permissionLevel, outputHandler);
		}

		outputHandler?.Send(FailedToFindCommand);
		return false;
	}

	IEnumerable<string> IReportCommands.ReportCommands(PermissionLevel authority)
	{
		return
			_tCommands.Where(
				          command =>
					          command.Value.PermissionRequired <= authority &&
					          command.Value.DisplayOptions != CommandDisplayOptions.Hidden && command.Value.Name != null
			          )
			          .Select(
				          command =>
					          command.Value.DisplayOptions == CommandDisplayOptions.DisplayCommandWords
						          ? command.Key
						          : command.Value.Name)
			          .Distinct()
			          .Select(x => x.ToLowerInvariant())
			          .OrderBy(x => x);
	}

	public IEnumerable<string> ReportCommands(PermissionLevel authority, T reference)
	{
		return
			_tCommands.Where(
				          command =>
					          command.Value.PermissionRequired <= authority &&
					          command.Value.DisplayOptions != CommandDisplayOptions.Hidden &&
					          command.Value.Name != null &&
					          command.Value.AppearInCommandsListCondition?.Invoke(reference, command.Key) != false
			          )
			          .Select(
				          command =>
					          command.Value.DisplayOptions == CommandDisplayOptions.DisplayCommandWords
						          ? command.Key
						          : command.Value.Name)
			          .Distinct()
			          .Select(x => x.ToLowerInvariant())
			          .OrderBy(x => x);
	}

	public IEnumerable<IGrouping<string, string>> ReportCommandsInGroups(PermissionLevel authority,
		ICharacter reference)
	{
		return _tCommands.Where(
			                 command =>
				                 command.Value.PermissionRequired <= authority &&
				                 command.Value.DisplayOptions != CommandDisplayOptions.Hidden &&
				                 command.Value.Name != null &&
				                 command.Value.AppearInCommandsListCondition?.Invoke(reference, command.Key) != false
		                 )
		                 .GroupBy(x => x.Value.ModuleName ?? "Other", command =>
			                 command.Value.DisplayOptions == CommandDisplayOptions.DisplayCommandWords
				                 ? command.Key
				                 : command.Value.Name);
	}

	public void Add(string entry, IEnumerable<string> entries,
		IExecutable<T>.CommandGenericMethodDelegate commandGenericMethod,
		CharacterState state = CharacterState.Any,
		PermissionLevel permissionRequired = PermissionLevel.Any,
		CommandDisplayOptions options = CommandDisplayOptions.None)
	{
		if (HasPermissionUpTo < permissionRequired)
		{
			return;
		}

		Add(entries, new Command<T>(commandGenericMethod, state, permissionRequired, entry, options));
	}

	public void Clear()
	{
		_tCommands.Clear();
	}

	protected void CheckCommandConditions(ref IExecutable<T> command, T reference, string commandText)
	{
		if (command?.AppearInCommandsListCondition?.Invoke(reference, commandText) == false)
		{
			command = null;
		}
	}

	public virtual IExecutable<T> LocateCommand(T reference, ref string input)
	{
		var splitCommands = new StringStack(input);
		var commandText = splitCommands.PopSpeech().ToLowerInvariant();
		_tCommands.TryGetValue(commandText, out var command);
		CheckCommandConditions(ref command, reference, commandText);

		if (command == null && !string.IsNullOrWhiteSpace(commandText))
		{
			var abbreviation =
				_tCommands.OrderBy(x => x.Key)
				          .FirstOrDefault(
					          x => x.Key.StartsWith(splitCommands.Last, StringComparison.InvariantCultureIgnoreCase));
			command = abbreviation.IsDefault() ? null : abbreviation.Value;
			CheckCommandConditions(ref command, reference, abbreviation.Key);
		}

		if (command != null && command.ConsumesUsedCommand)
		{
			input = splitCommands.RemainingArgument.Length > 1 ? splitCommands.RemainingArgument : "";
		}

		return command;
	}

	public override string ToString()
	{
		var sb = new StringBuilder();

		return sb.ToString();
	}

	public IEnumerable<string> ReportCommands(PermissionLevel minimumAuthority, PermissionLevel maximumAuthority,
		T reference)
	{
		return
			_tCommands.Where(
				          command =>
					          command.Value.PermissionRequired >= minimumAuthority &&
					          command.Value.PermissionRequired <= maximumAuthority &&
					          command.Value.DisplayOptions != CommandDisplayOptions.Hidden &&
					          command.Value.Name != null &&
					          command.Value.AppearInCommandsListCondition?.Invoke(reference, command.Key) != false
			          )
			          .Select(
				          command =>
					          command.Value.DisplayOptions == CommandDisplayOptions.DisplayCommandWords
						          ? command.Key
						          : command.Value.Name)
			          .Distinct()
			          .Select(x => x.ToLowerInvariant())
			          .OrderBy(x => x);
	}
}

public class CharacterCommandManager : CommandManager<ICharacter>, ICharacterCommandManager
{
	public CharacterCommandManager(string failedToFindCommandString = "",
		PermissionLevel hasPermissionUpTo = PermissionLevel.Any)
		: base(failedToFindCommandString, hasPermissionUpTo)
	{
	}

	public IEnumerable<ISocial> Socials { get; set; }

	public void AddSocials(IEnumerable<ISocial> socials)
	{
		Socials = socials;
	}

	public IEnumerable<ICommandHelpInfo> CommandHelpInfos => _tCommands.Values.SelectNotNull(x => x.HelpInfo);

	public override IExecutable<ICharacter> LocateCommand(ICharacter reference, ref string input)
	{
		var splitCommands = new StringStack(input);

		var commandText = splitCommands.PopSpeech().ToLowerInvariant();
		_tCommands.TryGetValue(commandText, out var command);
		CheckCommandConditions(ref command, reference, commandText);

		if (command == null)
		{
			var social = Socials?.FirstOrDefault(x => x.Applies(reference, splitCommands.Last, false));
			if (social != null)
			{
				command = social.GetCommand();
			}
		}

		if (command == null && !string.IsNullOrWhiteSpace(splitCommands.Last))
		{
			var abbrevCommands =
				_tCommands.Concat(
					Socials?.Where(x => x.Applies(reference, splitCommands.Last, true))
					       .ToDictionary(x => x.Name, x => x.GetCommand()) ??
					Enumerable.Empty<KeyValuePair<string, IExecutable<ICharacter>>>());
			var abbreviation =
				abbrevCommands.OrderBy(x => x.Key)
				              .FirstOrDefault(
					              x => x.Key.StartsWith(splitCommands.Last,
						              StringComparison.InvariantCultureIgnoreCase));
			command = abbreviation.IsDefault() ? null : abbreviation.Value;
			CheckCommandConditions(ref command, reference, abbreviation.Key);
		}

		if (command != null && command.ConsumesUsedCommand)
		{
			input = splitCommands.RemainingArgument.Length > 1 ? splitCommands.RemainingArgument : "";
		}

		return command;
	}

	public bool Execute(ICharacter argument, IExecutable<ICharacter> command, string playerInput,
		CharacterState state = CharacterState.Any,
		PermissionLevel permissionLevel = PermissionLevel.Any, IOutputHandler outputHandler = null)
	{
		if (permissionLevel < command.PermissionRequired)
		{
			outputHandler?.Send(FailedToFindCommand);
			return false;
		}

		if (command.MustBeAnEnforcer)
		{
			if (!argument.IsAdministrator() &&
			    !argument.Gameworld.LegalAuthorities.Any(x => x.GetEnforcementAuthority(argument) is not null))
			{
				outputHandler?.Send(FailedToFindCommand);
				return false;
			}
		}

		if (command.HelpInfo != null)
		{
			if (command.HelpInfo.CheckHelp(argument, playerInput, outputHandler))
				//The user got shown help, stop attempting to execute command here.
			{
				return true;
			}
		}

		if (argument.EffectsOfType<ICommandDelay>().Any(x => x.IsDelayed(command.Name)))
		{
			outputHandler?.Send(argument.EffectsOfType<ICommandDelay>().First(x => x.IsDelayed(command.Name)).Message);
			return false;
		}

		if (command.DelayType != null)
		{
			var delayingEffect = argument.Effects.FirstOrDefault(x =>
				x.IsBlockingEffect(command.DelayType) && (string.IsNullOrEmpty(command.DelayExceptionType) ||
				                                          !x.IsBlockingEffect(command.DelayExceptionType)));
			if (delayingEffect != null)
			{
				outputHandler?.Send(string.Format(command.DelayErrorMessage,
					delayingEffect.BlockingDescription(command.DelayType, argument)));
				return false;
			}
		}

		if (!command.AllowedStates.HasFlag(state))
		{
			outputHandler?.Send($"You cannot do that while you are {state.Describe().ToLowerInvariant()}.");
			return false;
		}

		if (command.NoHideCommand && argument.AffectedBy<IHideEffect>())
		{
			outputHandler?.Send("You cannot do that while you continue to remain hidden.");
			return false;
		}

		if (command.NoCombatCommand && argument.Combat != null)
		{
			outputHandler?.Send("You are too busy fighting to worry about that!");
			return false;
		}

		if (command.NoMeleeCombatCommand && argument.Combat != null && argument.MeleeRange)
		{
			outputHandler?.Send("You are too busy fighting in melee combat to worry about that!");
			return false;
		}

		if (command.NoMovementCommand && argument.Movement != null)
		{
			outputHandler?.Send("You must stop moving before you can do that.");
			return false;
		}

		return command.Execute(argument, playerInput, state, permissionLevel, outputHandler);
	}

	public override bool Execute(ICharacter argument, string playerInput, CharacterState state = CharacterState.Any,
		PermissionLevel permissionLevel = PermissionLevel.Any, IOutputHandler outputHandler = null)
	{
		var command = LocateCommand(argument, ref playerInput);
		if (command != null)
		{
			return Execute(argument, command, playerInput, state, permissionLevel, outputHandler);
		}

		if (!string.IsNullOrWhiteSpace(playerInput))
		{
			outputHandler?.Send(FailedToFindCommand);
		}
		else
		{
			outputHandler?.Send("");
		}
		
		return false;
	}
}