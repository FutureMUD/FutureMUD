#nullable enable

using MudSharp.Computers;
using MudSharp.Framework;
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

	public static bool TryResolveSignalComponentPrototype(IFuturemud gameworld, string componentIdentifier,
		out IGameItemComponentProto? prototype)
	{
		prototype = null;
		if (string.IsNullOrWhiteSpace(componentIdentifier))
		{
			return false;
		}

		prototype = long.TryParse(componentIdentifier, out var prototypeId)
			? gameworld.ItemComponentProtos.Get(prototypeId)
			: gameworld.ItemComponentProtos.GetByName(componentIdentifier);
		return prototype is not null;
	}

	public static string DescribeSignalComponent(IFuturemud gameworld, long sourceComponentId, string sourceComponentName)
	{
		if (sourceComponentId > 0)
		{
			var prototype = gameworld.ItemComponentProtos.Get(sourceComponentId);
			if (prototype is not null)
			{
				return prototype.Name;
			}
		}

		return sourceComponentName;
	}

	public static ISignalSourceComponent? FindSignalSource(IGameItem parent, long sourceComponentId,
		string sourceComponentName,
		IGameItemComponent? excludedComponent = null)
	{
		if (sourceComponentId <= 0 && string.IsNullOrWhiteSpace(sourceComponentName))
		{
			return null;
		}

		return parent.GetItemTypes<ISignalSourceComponent>()
			.FirstOrDefault(x =>
				!ReferenceEquals(x, excludedComponent) &&
				(sourceComponentId > 0
					? x.LocalSignalSourceIdentifier == sourceComponentId
					: ((IGameItemComponent)x).Name.Equals(sourceComponentName, StringComparison.InvariantCultureIgnoreCase)));
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
