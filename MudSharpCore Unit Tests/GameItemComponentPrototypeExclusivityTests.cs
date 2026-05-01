#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class GameItemComponentPrototypeExclusivityTests
{
	private static readonly HashSet<Type> AggregateComponentTypes =
	[
		typeof(IAffectQuality),
		typeof(ICanConnectToElectricalGrid),
		typeof(ICanConnectToGrid),
		typeof(ICanConnectToLiquidGrid),
		typeof(ICanConnectToTelecommunicationsGrid),
		typeof(IConnectable),
		typeof(IConsumePower),
		typeof(IOnOff),
		typeof(IProduceHeat),
		typeof(IProducePower),
		typeof(IRuntimeConfigurableSignalSinkComponent),
		typeof(ISignalSinkComponent),
		typeof(ISignalSourceComponent),
		typeof(ISwitchable),
		typeof(ITelephoneNumberOwner)
	];

	[TestMethod]
	public void FindConflicts_DuplicateExclusiveContainer_FindsConflict()
	{
		var first = CreatePrototype<IContainerPrototype>(1, "wooden box");
		var second = CreatePrototype<IContainerPrototype>(2, "metal chest");

		var conflict = GameItemComponentPrototypeExclusivity.FindConflicts([first.Object, second.Object])
		                                                    .Single();

		Assert.AreEqual(typeof(IContainer), conflict.Capability);
		Assert.AreSame(first.Object, conflict.Existing);
		Assert.AreSame(second.Object, conflict.Candidate);
	}

	[TestMethod]
	public void FindConflicts_SingleComponentWithMultipleExclusiveMarkers_DoesNotConflictWithItself()
	{
		var component = CreatePrototype<ILiquidContainerPrototype>(1, "canteen");

		Assert.IsFalse(GameItemComponentPrototypeExclusivity.FindConflicts([component.Object]).Any());
	}

	[TestMethod]
	public void FindConflicts_SameExclusivePrototypeAttachedTwice_FindsConflict()
	{
		var component = CreatePrototype<IContainerPrototype>(1, "wooden box");

		var conflict = GameItemComponentPrototypeExclusivity.FindConflicts([component.Object, component.Object])
		                                                    .Single();

		Assert.AreEqual(typeof(IContainer), conflict.Capability);
		Assert.AreSame(component.Object, conflict.Existing);
		Assert.AreSame(component.Object, conflict.Candidate);
	}

	[TestMethod]
	public void FindConflicts_DuplicateAggregateConnectable_DoesNotConflict()
	{
		var first = CreatePrototype<IConnectablePrototype>(1, "left connector");
		var second = CreatePrototype<IConnectablePrototype>(2, "right connector");

		Assert.IsFalse(GameItemComponentPrototypeExclusivity.FindConflicts([first.Object, second.Object]).Any());
	}

	[TestMethod]
	public void CanAddComponent_DuplicateExclusiveContainer_ReportsCandidateConflict()
	{
		var existing = CreatePrototype<IContainerPrototype>(1, "wooden box");
		var candidate = CreatePrototype<IContainerPrototype>(2, "metal chest");

		Assert.IsFalse(GameItemComponentPrototypeExclusivity.CanAddComponent([existing.Object], candidate.Object,
			out var conflict));
		Assert.IsNotNull(conflict);
		Assert.AreEqual(typeof(IContainer), conflict!.Capability);
		Assert.AreSame(existing.Object, conflict.Existing);
		Assert.AreSame(candidate.Object, conflict.Candidate);
	}

	[TestMethod]
	public void ComponentInterfaces_AllHavePrototypeMarkersAndClassification()
	{
		var runtimeInterfaces = typeof(IGameItemComponent).Assembly
		                                                 .GetTypes()
		                                                 .Where(x => x is { IsInterface: true, IsPublic: true } &&
		                                                             x != typeof(IGameItemComponent) &&
		                                                             typeof(IGameItemComponent).IsAssignableFrom(x))
		                                                 .OrderBy(x => x.FullName)
		                                                 .ToList();

		foreach (var runtimeInterface in runtimeInterfaces)
		{
			var marker = GetPrototypeMarker(runtimeInterface);
			Assert.IsNotNull(marker, $"{runtimeInterface.FullName} is missing a prototype marker interface.");

			var classification = (AggregateComponentTypes.Contains(runtimeInterface)
				? typeof(IAggregateGameItemComponentPrototype<>)
				: typeof(IExclusiveGameItemComponentPrototype<>)).MakeGenericType(runtimeInterface);

			Assert.IsTrue(classification.IsAssignableFrom(marker),
				$"{marker!.FullName} does not implement {classification.Name}.");
		}
	}

	[TestMethod]
	public void ComponentPrototypes_ExposeMarkersForTheirRuntimeComponentInterfaces()
	{
		var coreAssembly = typeof(GameItemComponentProto).Assembly;
		var protoTypes = coreAssembly
		                 .GetTypes()
		                 .Where(x => x is { IsClass: true, IsAbstract: false } &&
		                             typeof(GameItemComponentProto).IsAssignableFrom(x))
		                 .OrderBy(x => x.FullName)
		                 .ToList();

		foreach (var protoType in protoTypes)
		{
			var runtimeType = ResolveRuntimeComponentType(coreAssembly, protoType);
			Assert.IsNotNull(runtimeType, $"{protoType.FullName} does not have a matching runtime component type.");

			var runtimeInterfaces = runtimeType!
			                        .GetInterfaces()
			                        .Where(x => x != typeof(IGameItemComponent) &&
			                                    typeof(IGameItemComponent).IsAssignableFrom(x))
			                        .OrderBy(x => x.FullName);

			foreach (var runtimeInterface in runtimeInterfaces)
			{
				var marker = GetPrototypeMarker(runtimeInterface);
				Assert.IsNotNull(marker, $"{runtimeInterface.FullName} is missing a prototype marker interface.");
				Assert.IsTrue(marker!.IsAssignableFrom(protoType),
					$"{protoType.FullName} does not implement {marker.FullName} for runtime interface {runtimeInterface.FullName}.");
			}
		}
	}

	private static Mock<T> CreatePrototype<T>(long id, string name)
		where T : class, IGameItemComponentProto
	{
		var mock = new Mock<T>();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.RevisionNumber).Returns(0);
		mock.SetupGet(x => x.Name).Returns(name);
		return mock;
	}

	private static Type? GetPrototypeMarker(Type runtimeInterface)
	{
		return typeof(IGameItemComponentPrototype<>).Assembly
		                                           .GetType($"MudSharp.GameItems.Prototypes.{runtimeInterface.Name}Prototype");
	}

	private static Type? ResolveRuntimeComponentType(System.Reflection.Assembly assembly, Type protoType)
	{
		var protoName = protoType.Name;
		if (!protoName.EndsWith("GameItemComponentProto", StringComparison.Ordinal))
		{
			return null;
		}

		var runtimeName = protoName.Replace("GameItemComponentProto", "GameItemComponent");
		return assembly.GetType($"MudSharp.GameItems.Components.{runtimeName}");
	}
}
