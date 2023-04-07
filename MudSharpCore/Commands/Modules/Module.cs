using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MudSharp.Accounts;
using MudSharp.Character;

namespace MudSharp.Commands.Modules;

[AttributeUsage(AttributeTargets.Method)]
public class PlayerCommand : Attribute
{
	public PlayerCommand(string name, params string[] commandWords)
	{
		Name = name;
		CommandWords = commandWords;
	}

	public string Name { get; protected set; }

	public IEnumerable<string> CommandWords { get; protected set; }
}

[AttributeUsage(AttributeTargets.Method)]
public class ConditionalCommandAppearance : Attribute
{
	public ConditionalCommandAppearance(Func<object, string, bool> condition)
	{
		Condition = condition;
	}

	public Func<object, string, bool> Condition { get; protected set; }
}

[AttributeUsage(AttributeTargets.Method)]
public class DelayBlock : Attribute
{
	/// <summary>
	/// Declares that this command will block if any effects with a delay block are on the executor
	/// </summary>
	/// <param name="delayType">The delay type which blocks the execution of the command</param>
	/// <param name="errorMessage">The error message to display to the player. Use {0} to represent the description of the blocking action.</param>
	public DelayBlock(string delayType, string errorMessage)
	{
		DelayType = delayType;
		ErrorMessage = errorMessage;
	}

	/// <summary>
	/// Declares that this command will block if any effects with a delay block are on the executor
	/// </summary>
	/// <param name="delayType">The delay type which blocks the execution of the command</param>
	/// <param name="delayExceptionType">A delay type which, if also present, will NOT block the execution of the command</param>
	/// <param name="errorMessage">The error message to display to the player. Use {0} to represent the description of the blocking action.</param>
	public DelayBlock(string delayType, string delayExceptionType, string errorMessage)
	{
		DelayType = delayType;
		ErrorMessage = errorMessage;
		DelayExceptionType = delayExceptionType;
	}

	public string DelayType { get; protected set; }
	public string ErrorMessage { get; protected set; }
	public string DelayExceptionType { get; protected set; }
}

[AttributeUsage(AttributeTargets.Method)]
public class HelpInfo : Attribute
{
	/// <summary>
	/// Provides an automated help file lookup for the command without having to build it into the command implementation. 
	/// </summary>
	/// <param name="helpFile">The help file title for this command.</param>
	/// <param name="defaultHelp">The text to show if there is no automated help file.</param>
	/// <param name="autoHelp">This enum establishes the circumstances in which the helpfile will be displayed.</param>
	/// <param name="adminHelp">If admins have a different helpfile than players, it can be specified here.</param>
	public HelpInfo(string helpFile, string defaultHelp, AutoHelp autoHelp, string adminHelp = null)
	{
		HelpFile = helpFile;
		DefaultHelp = defaultHelp;
		AutoHelpSetting = autoHelp;
		AdminHelp = adminHelp ?? defaultHelp;
	}

	public string HelpFile { get; protected set; }
	public string DefaultHelp { get; protected set; }
	public AutoHelp AutoHelpSetting { get; protected set; }
	public string AdminHelp { get; protected set; }
}

[AttributeUsage(AttributeTargets.Method)]
public class DisplayOptions : Attribute
{
	public DisplayOptions(CommandDisplayOptions options)
	{
		Options = options;
	}

	public CommandDisplayOptions Options { get; protected set; }
}

[AttributeUsage(AttributeTargets.Method)]
public class CommandPermission : Attribute
{
	public CommandPermission(PermissionLevel permissionLevel)
	{
		PermissionLevel = permissionLevel;
	}

	public PermissionLevel PermissionLevel { get; protected set; }
}

[AttributeUsage(AttributeTargets.Method)]
public class RequiredCharacterState : Attribute
{
	public RequiredCharacterState(CharacterState state)
	{
		State = state;
	}

	public CharacterState State { get; protected set; }
}

[AttributeUsage(AttributeTargets.Method)]
public class SubMethod : Attribute
{
}

/// <summary>
///     NoHideCommand flags a command as unusable while a character is attempting to remain hidden
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class NoHideCommand : Attribute
{
}

/// <summary>
///     NoCombatCommand flags a command as unusable while a character is in combat
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class NoCombatCommand : Attribute
{
}

/// <summary>
///     NoMeleeCombatCommand flags a command as unusable while a character is in melee combat
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class NoMeleeCombatCommand : Attribute
{
}

/// <summary>
///     NoMovementCommand flags a command as unusable while a character is moving
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class NoMovementCommand : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
public class MustBeAnEnforcer : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
public class CustomModuleName : Attribute
{
	public CustomModuleName(string moduleName)
	{
		ModuleName = moduleName;
	}

	public string ModuleName { get; }
}

public abstract class Module<T> : IModule
{
	protected Module(string name)
	{
		Name = name;
		Commands = new CommandManager<T>("Huh?", PermissionLevel.Inaccessible);
		CompatibilityRules = new Dictionary<IModule, ModuleCompatibility.Test>();
		CompileMethods();
		CompileSubMethods();
	}

	public CommandManager<T> Commands { get; protected set; }

	protected bool IsNecessary { get; set; }

	public string Name { get; protected set; }

	public virtual int CommandsDisplayOrder => int.MaxValue;

	public Dictionary<IModule, ModuleCompatibility.Test> CompatibilityRules { get; protected set; }

	protected void CompileMethods()
	{
		foreach (var dissection in
		         GetType().GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Where(
			         x =>
				         !x.GetCustomAttributes(typeof(SubMethod), false).Any() &&
				         x.GetCustomAttributes(typeof(PlayerCommand), false).Any()).Select(DissectMethod))
		{
			Commands.Add(dissection.Item1, dissection.Item2);
		}
	}

	protected virtual void CompileSubMethods()
	{
	}

	protected Tuple<IEnumerable<string>, Command<T>> DissectMethod(MethodInfo commandMethod)
	{
		var playerCommand = (PlayerCommand)commandMethod.GetCustomAttributes(typeof(PlayerCommand), false)[0];
		var requiredCharacterState =
			(RequiredCharacterState)
			commandMethod.GetCustomAttributes(typeof(RequiredCharacterState), false).FirstOrDefault();
		var commandPermission =
			(CommandPermission)commandMethod.GetCustomAttributes(typeof(CommandPermission), false).FirstOrDefault();
		var delayBlock = (DelayBlock)commandMethod.GetCustomAttributes(typeof(DelayBlock), false).FirstOrDefault();
		var method =
			(IExecutable<T>.CommandGenericMethodDelegate)Delegate.CreateDelegate(
				typeof(IExecutable<T>.CommandGenericMethodDelegate), commandMethod);

		var helpInfoData = (HelpInfo)commandMethod.GetCustomAttribute(typeof(HelpInfo), false);
		var conditions =
			(ConditionalCommandAppearance)commandMethod.GetCustomAttribute(
				typeof(ConditionalCommandAppearance), false);

		CommandHelpInfo helpInfo = null;
		if (helpInfoData != null)
		{
			helpInfo = new CommandHelpInfo(helpInfoData.HelpFile, helpInfoData.DefaultHelp,
				helpInfoData.AutoHelpSetting, helpInfoData.AdminHelp);
		}

		var displayOptions =
			(DisplayOptions)commandMethod.GetCustomAttributes(typeof(DisplayOptions), false).FirstOrDefault();
		var commandDisplayOptions = displayOptions?.Options ?? CommandDisplayOptions.None;
		var requiredState = requiredCharacterState?.State ?? CharacterState.Any;
		var permission = commandPermission?.PermissionLevel ?? PermissionLevel.Any;

		return new Tuple<IEnumerable<string>, Command<T>>(playerCommand.CommandWords,
			new Command<T>(method, requiredState, permission, playerCommand.Name, commandDisplayOptions, delayBlock,
				commandMethod.GetCustomAttributes(typeof(NoCombatCommand), false).FirstOrDefault() != null,
				commandMethod.GetCustomAttributes(typeof(NoHideCommand), false).FirstOrDefault() != null,
				commandMethod.GetCustomAttributes(typeof(NoMovementCommand), false).FirstOrDefault() != null,
				commandMethod.GetCustomAttributes(typeof(NoMeleeCombatCommand), false).FirstOrDefault() != null,
				helpInfo,
				conditions?.Condition,
				commandMethod.GetCustomAttributes(typeof(MustBeAnEnforcer), false).FirstOrDefault() != null,
				commandMethod.GetCustomAttributes(typeof(CustomModuleName), false).OfType<CustomModuleName>()
				             .FirstOrDefault()?.ModuleName ?? Name
			));
	}

	public override string ToString()
	{
		var sb = new StringBuilder();

		sb.AppendLine("Module " + Name + " contains: ");

		foreach (var command in Commands.ReportCommands(PermissionLevel.Any, default))
		{
			sb.AppendLine(command);
		}

		return sb.ToString();
	}
}