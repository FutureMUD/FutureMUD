using MudSharp.GameItems.Interfaces;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

/// <summary>
/// Describes a runtime item capability that must be supplied by another component on the same item prototype.
/// </summary>
/// <param name="Capability">A public interface derived from <see cref="IGameItemComponent"/>.</param>
/// <param name="Reason">Optional builder-facing context explaining why the sibling capability is required.</param>
public sealed record GameItemComponentPrototypeRequirement(Type Capability, string? Reason = null);

/// <summary>
/// Implemented by component prototypes that depend on capabilities supplied by sibling components.
/// </summary>
public interface IGameItemComponentPrototypeRequirementProvider
{
	IReadOnlyCollection<GameItemComponentPrototypeRequirement> RequiredSiblingComponents { get; }
}

public sealed record MissingGameItemComponentPrototypeRequirement(
	IGameItemComponentProto RequiringComponent,
	GameItemComponentPrototypeRequirement Requirement);

/// <summary>
/// Shared item-prototype composition checks for component requirements.
/// </summary>
public static class GameItemComponentPrototypeRequirements
{
	private static readonly Type ComponentPrototypeMarkerType = typeof(IGameItemComponentPrototype<>);

	public static IEnumerable<Type> ComponentTypes(this IGameItemComponentProto proto)
	{
		return proto.GetType()
		            .GetInterfaces()
		            .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == ComponentPrototypeMarkerType)
		            .Select(x => x.GetGenericArguments()[0])
		            .Distinct();
	}

	public static IReadOnlyList<MissingGameItemComponentPrototypeRequirement> FindMissingRequirements(
		IEnumerable<IGameItemComponentProto> components)
	{
		var componentList = components.ToList();
		var results = new List<MissingGameItemComponentPrototypeRequirement>();

		foreach (var requiringComponent in componentList
			         .OfType<IGameItemComponentPrototypeRequirementProvider>())
		{
			var prototype = (IGameItemComponentProto)requiringComponent;
			foreach (var requirement in requiringComponent.RequiredSiblingComponents
			                                            .DistinctBy(x => x.Capability))
			{
				if (!IsComponentCapability(requirement.Capability))
				{
					results.Add(new MissingGameItemComponentPrototypeRequirement(prototype, requirement));
					continue;
				}

				var isSatisfied = componentList
				                  .Where(x => !ReferenceEquals(x, prototype))
				                  .SelectMany(x => x.ComponentTypes())
				                  .Any(requirement.Capability.IsAssignableFrom);
				if (!isSatisfied)
				{
					results.Add(new MissingGameItemComponentPrototypeRequirement(prototype, requirement));
				}
			}
		}

		return results;
	}

	public static bool IsComponentCapability(Type type)
	{
		return type is { IsInterface: true } &&
		       type != typeof(IGameItemComponent) &&
		       typeof(IGameItemComponent).IsAssignableFrom(type);
	}

	public static Type? ResolveComponentCapability(string text)
	{
		var normalized = text.Replace(" ", string.Empty);
		return typeof(IGameItemComponent).Assembly
		                                  .GetTypes()
		                                  .Where(IsComponentCapability)
		                                  .FirstOrDefault(x =>
			                                  string.Equals(x.FullName, normalized,
				                                  StringComparison.InvariantCultureIgnoreCase) ||
			                                  string.Equals(x.Name, normalized,
				                                  StringComparison.InvariantCultureIgnoreCase) ||
			                                  string.Equals(x.Name[1..], normalized,
				                                  StringComparison.InvariantCultureIgnoreCase));
	}

	public static string DescribeComponentCapability(Type capability)
	{
		var name = capability.Name;
		if (name.Length > 1 && name[0] == 'I' && char.IsUpper(name[1]))
		{
			name = name[1..];
		}

		return name.SplitCamelCase();
	}
}
