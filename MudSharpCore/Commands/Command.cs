using System;
using System.Collections.Generic;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using Org.BouncyCastle.Asn1;

namespace MudSharp.Commands;

public class Command : IExecutable
{
	public readonly IExecutable.CommandMethodDelegate _executeCommandMethod;

	public Command(IExecutable.CommandMethodDelegate commandMethod, CharacterState states = CharacterState.Any,
		PermissionLevel permissionRequired = PermissionLevel.Any, string name = null)
	{
		Name = name;
		_executeCommandMethod = commandMethod;
		PermissionRequired = permissionRequired;
		AllowedStates = states;
	}

	public string Name { get; protected set; }

	public PermissionLevel PermissionRequired { get; protected set; }

	public string FailedToFindCommand { get; private set; }

	public bool ConsumesUsedCommand => false;

	public CharacterState AllowedStates { get; }

	public virtual bool Execute(string playerInput, CharacterState state = CharacterState.Any,
		PermissionLevel permissionLevel = PermissionLevel.Any, IOutputHandler outputHandler = null)
	{
		if (permissionLevel < PermissionRequired)
		{
			return false;
		}

		_executeCommandMethod(playerInput);
		return true;
	}

	public IEnumerable<string> ReportCommands(PermissionLevel authority = PermissionLevel.Any)
	{
		return null;
	}
}

public class Command<T> : IExecutable<T>
{
	public readonly IExecutable<T>.CommandGenericMethodDelegate _executeCommandGenericMethod;

	public Command(IExecutable<T>.CommandGenericMethodDelegate commandGenericMethod,
		CharacterState states = CharacterState.Any,
		PermissionLevel permissionRequired = PermissionLevel.Any,
		string name = null,
		CommandDisplayOptions displayOptions = CommandDisplayOptions.None,
		DelayBlock delayBlock = null,
		bool noCombatCommand = false,
		bool noHidecommand = false,
		bool noMovementCommand = false,
		bool noMeleeCombatCommand = false,
		CommandHelpInfo helpInfo = null,
		Func<object, string, bool> condition = null,
		bool mustBeAnEnforcer = false,
		string moduleName = null)
	{
		Name = name;
		_executeCommandGenericMethod = commandGenericMethod;
		PermissionRequired = permissionRequired;
		AllowedStates = states;
		DisplayOptions = displayOptions;
		if (delayBlock != null)
		{
			DelayType = delayBlock.DelayType;
			DelayErrorMessage = delayBlock.ErrorMessage;
			DelayExceptionType = delayBlock.DelayExceptionType;
		}

		NoCombatCommand = noCombatCommand;
		NoHideCommand = noHidecommand;
		NoMovementCommand = noMovementCommand;
		NoMeleeCombatCommand = noMeleeCombatCommand;
		HelpInfo = helpInfo;
		AppearInCommandsListCondition = condition;
		MustBeAnEnforcer = mustBeAnEnforcer;
		ModuleName = moduleName;
	}

	public string Name { get; protected set; }

	public CommandDisplayOptions DisplayOptions { get; protected set; }

	public PermissionLevel PermissionRequired { get; protected set; }

	public string FailedToFindCommand { get; private set; }

	public bool ConsumesUsedCommand => false;

	public CharacterState AllowedStates { get; }

	public string DelayType { get; }

	public string DelayErrorMessage { get; }
	public string DelayExceptionType { get; }

	public bool NoCombatCommand { get; set; }

	public bool NoMeleeCombatCommand { get; set; }

	public bool NoHideCommand { get; set; }

	public string ModuleName { get; set; }

	public bool NoMovementCommand { get; set; }

	public ICommandHelpInfo HelpInfo { get; set; }

	public Func<object, string, bool> AppearInCommandsListCondition { get; }

	public bool MustBeAnEnforcer { get; set; }

	public virtual bool Execute(T argument, string playerInput, CharacterState state = CharacterState.Any,
		PermissionLevel permissionLevel = PermissionLevel.Any, IOutputHandler outputHandler = null)
	{
		if (permissionLevel < PermissionRequired)
		{
			return false;
		}

		if (!AllowedStates.HasFlag(state))
		{
			return false;
		}

		_executeCommandGenericMethod(argument, playerInput);
		return true;
	}

	public IEnumerable<string> ReportCommands(PermissionLevel authority = PermissionLevel.Any)
	{
		return null;
	}
}

public class TaggedCommand<T> : IExecutable, IDisposable
{
	public delegate void MethodDelegate(T argument, string playerInput);

	public MethodDelegate ExecuteMethod;
	public T Tag;

	public TaggedCommand(T taggedData, MethodDelegate method, CharacterState states = CharacterState.Any,
		PermissionLevel permissionRequired = PermissionLevel.Any, string name = null)
	{
		Name = name;
		ExecuteMethod = method;
		PermissionRequired = permissionRequired;
		Tag = taggedData;
		AllowedStates = states;
	}

	public void Dispose()
	{
		Tag = default;
		GC.SuppressFinalize(this);
	}

	public string Name { get; protected set; }

	public PermissionLevel PermissionRequired { get; protected set; }

	public string FailedToFindCommand { get; private set; }

	public bool ConsumesUsedCommand => false;

	public CharacterState AllowedStates { get; }

	public virtual bool Execute(string playerInput, CharacterState state = CharacterState.Any,
		PermissionLevel permissionLevel = PermissionLevel.Any, IOutputHandler outputHandler = null)
	{
		if (permissionLevel < PermissionRequired)
		{
			return false;
		}

		ExecuteMethod(Tag, playerInput);
		return true;
	}

	public IEnumerable<string> ReportCommands(PermissionLevel authority = PermissionLevel.Any)
	{
		return null;
	}
}