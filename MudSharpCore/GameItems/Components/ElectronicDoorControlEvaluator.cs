#nullable enable

using MudSharp.Construction;

namespace MudSharp.GameItems.Components;

internal enum ElectronicDoorControlAction
{
	None,
	Open,
	Close
}

internal readonly record struct ElectronicDoorControlOutcome(ElectronicDoorControlAction Action, bool RequiresRetry);

internal static class ElectronicDoorControlEvaluator
{
	/// <summary>
	/// Resolves an automatic actuator command. Unlike manual controls, automation is allowed to operate a door whose
	/// prototype excludes player interaction; it still must not open through a currently locked door.
	/// </summary>
	internal static ElectronicDoorControlOutcome EvaluateAutomatic(bool desiredOpen, DoorState state,
		bool isOpeningBlocked)
	{
		return Evaluate(desiredOpen, state == DoorState.Open, state == DoorState.Closed && !isOpeningBlocked,
			state == DoorState.Open);
	}

	internal static ElectronicDoorControlOutcome Evaluate(bool desiredOpen, bool isOpen, bool canOpen, bool canClose)
	{
		if (desiredOpen)
		{
			if (isOpen)
			{
				return new ElectronicDoorControlOutcome(ElectronicDoorControlAction.None, false);
			}

			return canOpen
				? new ElectronicDoorControlOutcome(ElectronicDoorControlAction.Open, false)
				: new ElectronicDoorControlOutcome(ElectronicDoorControlAction.None, true);
		}

		if (!isOpen)
		{
			return new ElectronicDoorControlOutcome(ElectronicDoorControlAction.None, false);
		}

		return canClose
			? new ElectronicDoorControlOutcome(ElectronicDoorControlAction.Close, false)
			: new ElectronicDoorControlOutcome(ElectronicDoorControlAction.None, true);
	}
}
