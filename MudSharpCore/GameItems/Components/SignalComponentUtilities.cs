#nullable enable

using MudSharp.Computers;
using MudSharp.GameItems.Interfaces;
using System;
using System.Linq;

namespace MudSharp.GameItems.Components;

public static class SignalComponentUtilities
{
	public static bool SignalsEqual(ComputerSignal lhs, ComputerSignal rhs)
	{
		return Math.Abs(lhs.Value - rhs.Value) < 0.0000001 &&
		       lhs.Duration == rhs.Duration &&
		       lhs.PulseInterval == rhs.PulseInterval;
	}

	public static bool IsActiveSignal(double value, double threshold, bool activeWhenAboveThreshold)
	{
		return activeWhenAboveThreshold
			? value >= threshold
			: value < threshold;
	}

	public static ISignalSourceComponent? FindSignalSource(IGameItem parent, string sourceComponentName,
		IGameItemComponent? excludedComponent = null)
	{
		if (string.IsNullOrWhiteSpace(sourceComponentName))
		{
			return null;
		}

		return parent.GetItemTypes<ISignalSourceComponent>()
			.FirstOrDefault(x =>
				!ReferenceEquals(x, excludedComponent) &&
				((IGameItemComponent)x).Name.Equals(sourceComponentName, StringComparison.InvariantCultureIgnoreCase));
	}

	public static TTarget? FindSiblingComponent<TTarget>(IGameItem parent, string componentName,
		IGameItemComponent? excludedComponent = null)
		where TTarget : class, IGameItemComponent
	{
		if (string.IsNullOrWhiteSpace(componentName))
		{
			return null;
		}

		return parent.GetItemTypes<TTarget>()
			.FirstOrDefault(x =>
				!ReferenceEquals(x, excludedComponent) &&
				x.Name.Equals(componentName, StringComparison.InvariantCultureIgnoreCase));
	}
}
