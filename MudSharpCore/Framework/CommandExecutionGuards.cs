using MudSharp.Accounts;

namespace MudSharp.Framework;

internal static class CommandExecutionGuards
{
	public const PermissionLevel AdminTargetThreshold = PermissionLevel.JuniorAdmin;

	public static bool CanForceTarget(ICharacter actor, ICharacter target)
	{
		if (actor.PermissionLevel >= PermissionLevel.Founder)
		{
			return true;
		}

		if (target.PermissionLevel < AdminTargetThreshold)
		{
			return true;
		}

		return target.PermissionLevel < actor.PermissionLevel;
	}

	public static bool CanUseAsTarget(ICharacter actor, ICharacter target)
	{
		return actor.PermissionLevel >= PermissionLevel.Founder ||
		       target.PermissionLevel < AdminTargetThreshold;
	}

	public static bool ExecuteForcedCommand(ICharacter target, string command)
	{
		if (!ShouldExecuteAsMortal(target))
		{
			return target.ExecuteCommand(command);
		}

		var originalPermission = target.PermissionLevel;
		target.ChangePermissionLevel(PermissionLevel.Player);
		try
		{
			return target.ExecuteCommand(command);
		}
		finally
		{
			target.ChangePermissionLevel(originalPermission);
		}
	}

	public static void OutOfContextExecuteForcedCommand(ICharacter target, string command)
	{
		if (!ShouldExecuteAsMortal(target))
		{
			target.OutOfContextExecuteCommand(command);
			return;
		}

		var originalPermission = target.PermissionLevel;
		target.ChangePermissionLevel(PermissionLevel.Player);
		try
		{
			target.OutOfContextExecuteCommand(command);
		}
		finally
		{
			target.ChangePermissionLevel(originalPermission);
		}
	}

	private static bool ShouldExecuteAsMortal(ICharacter target)
	{
		return target.IsPlayerCharacter && target.PermissionLevel > PermissionLevel.Player;
	}
}
