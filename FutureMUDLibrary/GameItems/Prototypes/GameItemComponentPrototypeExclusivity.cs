using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public sealed record GameItemComponentPrototypeConflict(
	Type Capability,
	IGameItemComponentProto Existing,
	IGameItemComponentProto Candidate);

public static class GameItemComponentPrototypeExclusivity
{
	private static readonly Type ExclusiveMarkerType = typeof(IExclusiveGameItemComponentPrototype<>);

	public static IEnumerable<Type> ExclusiveComponentTypes(this IGameItemComponentProto proto)
	{
		return proto.GetType()
		            .GetInterfaces()
		            .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == ExclusiveMarkerType)
		            .Select(x => x.GetGenericArguments()[0])
		            .Distinct();
	}

	public static IReadOnlyList<GameItemComponentPrototypeConflict> FindConflicts(
		IEnumerable<IGameItemComponentProto> components)
	{
		var results = new List<GameItemComponentPrototypeConflict>();
		var seen = new Dictionary<Type, IGameItemComponentProto>();

		foreach (var component in components)
		{
			foreach (var capability in component.ExclusiveComponentTypes())
			{
				if (!seen.TryGetValue(capability, out var existing))
				{
					seen[capability] = component;
					continue;
				}

				results.Add(new GameItemComponentPrototypeConflict(capability, existing, component));
			}
		}

		return results;
	}

	public static bool CanAddComponent(
		IEnumerable<IGameItemComponentProto> existingComponents,
		IGameItemComponentProto candidate,
		out GameItemComponentPrototypeConflict? conflict)
	{
		var existingCapabilities = existingComponents
		                           .SelectMany(x => x.ExclusiveComponentTypes()
		                                            .Select(y => (Capability: y, Prototype: x)))
		                           .GroupBy(x => x.Capability)
		                           .ToDictionary(x => x.Key, x => x.First().Prototype);

		foreach (var capability in candidate.ExclusiveComponentTypes())
		{
			if (!existingCapabilities.TryGetValue(capability, out var existing))
			{
				continue;
			}

			conflict = new GameItemComponentPrototypeConflict(capability, existing, candidate);
			return false;
		}

		conflict = null;
		return true;
	}
}
