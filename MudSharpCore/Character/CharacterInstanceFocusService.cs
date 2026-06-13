using MudSharp.Framework;
using System.Linq;

#nullable enable

namespace MudSharp.Character;

public sealed record CharacterInstanceFocusResult(
	bool Success,
	string Message,
	ICharacterInstance? Target = null
);

public static class CharacterInstanceFocusService
{
	public static CharacterInstanceFocusResult CanFocus(ICharacter actor, ICharacterInstance? target)
	{
		if (target is null)
		{
			return new CharacterInstanceFocusResult(false, "There is no such instance.");
		}

		var identity = actor.Identity;
		if (identity.Id != target.Identity.Id)
		{
			return new CharacterInstanceFocusResult(false, "You can only focus instances that belong to your own identity.");
		}

		if (identity.PrimaryInstance is not ICharacter primary || !primary.IsPlayerCharacter || primary.IsGuest)
		{
			return new CharacterInstanceFocusResult(false, "Only player characters can focus between instances.");
		}

		if (!identity.Instances.Any(x => x.InstanceId == target.InstanceId))
		{
			return new CharacterInstanceFocusResult(false, "That instance is not currently loaded.");
		}

		if (!target.IsEmbodied)
		{
			return new CharacterInstanceFocusResult(false, "That instance is not currently embodied.");
		}

		if (target.State.HasFlag(CharacterState.Dead))
		{
			return new CharacterInstanceFocusResult(false, "You cannot focus a dead instance.");
		}

		if (target.State.HasFlag(CharacterState.Stasis))
		{
			return new CharacterInstanceFocusResult(false, "You cannot focus an instance that is in stasis.");
		}

		if (!target.IsControllable)
		{
			return new CharacterInstanceFocusResult(false, "That instance is not controllable.");
		}

		if (!target.IsPrimaryInstance && target.ControlPolicy != CharacterInstanceControlPolicy.PlayerFocusable)
		{
			return new CharacterInstanceFocusResult(false, "That instance is not player-focusable.");
		}

		return new CharacterInstanceFocusResult(true, string.Empty, target);
	}

	public static CharacterInstanceFocusResult Focus(ICharacter actor, ICharacterInstance? target,
		bool sendSuccessMessage = true)
	{
		var validation = CanFocus(actor, target);
		if (!validation.Success || validation.Target is null)
		{
			return validation;
		}

		target = validation.Target;
		var controller = actor.CharacterController ?? target.CharacterController;
		if (controller is null)
		{
			return new CharacterInstanceFocusResult(false, "You do not currently have a controller to switch focus with.");
		}

		var alreadyFocused = actor.Identity.FocusedInstance?.InstanceId == target.InstanceId;
		if (alreadyFocused)
		{
			return new CharacterInstanceFocusResult(true, "You are already focused on that instance.", target);
		}

		if (actor.Identity is Character identity)
		{
			identity.SetFocusedInstance(target.IsPrimaryInstance ? null : target);
		}

		controller.SetContext(target);
		if (sendSuccessMessage)
		{
			target.OutputHandler.Send($"Your focus shifts to {DescribeInstanceForFocus(target)}.");
		}

		return new CharacterInstanceFocusResult(true, "Focus switched.", target);
	}

	public static bool TryReturnFocusToPrimary(ICharacter actor, string message, bool sendMessage)
	{
		if (actor.Identity.PrimaryInstance is not ICharacterInstance primary)
		{
			return false;
		}

		if (actor.Identity.FocusedInstance?.InstanceId == primary.InstanceId)
		{
			if (actor.Identity is Character focusedIdentity)
			{
				focusedIdentity.SetFocusedInstance(null);
			}

			return false;
		}

		var controller = actor.CharacterController ??
		                 (actor.Identity.FocusedInstance as ICharacter)?.CharacterController ??
		                 primary.CharacterController;

		if (actor.Identity is Character identity)
		{
			identity.SetFocusedInstance(null);
		}

		if (controller is not null)
		{
			controller.SetContext(primary);
			if (sendMessage && !string.IsNullOrWhiteSpace(message))
			{
				primary.OutputHandler.Send(message);
			}
		}

		return true;
	}

	public static ICharacter GetLogoutIdentityActor(ICharacter actor)
	{
		return actor.Identity.PrimaryInstance as ICharacter ?? actor;
	}

	public static bool QuitThroughPrimary(ICharacter actor, bool silent = false)
	{
		var primary = GetLogoutIdentityActor(actor);
		if (!ReferenceEquals(primary, actor))
		{
			TryReturnFocusToPrimary(actor, string.Empty, false);
		}

		return primary.Quit(silent);
	}

	public static bool IsFocusedSecondary(ICharacterInstance instance)
	{
		return !instance.IsPrimaryInstance &&
		       instance.Identity.FocusedInstance?.InstanceId == instance.InstanceId;
	}

	private static string DescribeInstanceForFocus(ICharacterInstance instance)
	{
		if (instance.IsPrimaryInstance)
		{
			return "your primary body".ColourName();
		}

		var form = instance.Identity.Forms.FirstOrDefault(x => ReferenceEquals(x.Body, instance.Body));
		var name = form?.Alias ?? instance.Body.Prototype.Name;
		return $"{name} (#{instance.InstanceId.ToString("N0", instance)})".ColourName();
	}
}
