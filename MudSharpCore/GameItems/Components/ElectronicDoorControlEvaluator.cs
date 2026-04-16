#nullable enable

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
