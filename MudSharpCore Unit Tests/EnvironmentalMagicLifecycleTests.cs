#nullable enable

using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Environment;
using MudSharp.TimeAndDate.Listeners;
using MudSharp.Work.Agriculture;
using MudSharp.Work.Foraging;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EnvironmentalMagicLifecycleTests
{
	[TestMethod]
	public void Futuremud_CellAddAndDestroy_NotifyCoordinatorAtOwningLifecycle()
	{
		var service = new Mock<IEnvironmentalMagicService>();
		using var world = CreateWorld(service.Object);
		var room = new Mock<IRoom>();
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(10L);
		cell.SetupGet(x => x.Room).Returns(room.Object);
		cell.SetupGet(x => x.Gameworld).Returns(world);

		world.Add(cell.Object);
		Assert.AreSame(cell.Object, world.Cells.Get(10));
		service.Verify(x => x.Register(cell.Object), Times.Once);
		world.Destroy(cell.Object);
		Assert.IsNull(world.Cells.Get(10));
		service.Verify(x => x.Unregister(cell.Object), Times.Once);
		room.Verify(x => x.Destroy(cell.Object), Times.Once);
		service.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void Futuremud_FieldAddReplacementAndDestroy_NotifyTheIndexedFieldIdentity()
	{
		var service = new Mock<IEnvironmentalMagicService>();
		using var world = CreateWorld(service.Object);
		var cell = Mock.Of<ICell>(x => x.Id == 10L);
		var first = Mock.Of<IAgricultureField>(x => x.Id == 1L && x.Cell == cell);
		var replacement = Mock.Of<IAgricultureField>(x => x.Id == 2L && x.Cell == cell);

		world.Add(first);
		world.Add(replacement);
		world.Destroy(first);
		Assert.IsNull(world.AgricultureFields.Get(1));
		Assert.AreSame(replacement, world.AgricultureFields.Get(2));
		world.Destroy(replacement);
		Assert.IsNull(world.AgricultureFields.Get(2));
		service.Verify(x => x.FieldChanged(first, false), Times.Once);
		service.Verify(x => x.FieldChanged(replacement, false), Times.Once);
		service.Verify(x => x.FieldChanged(first, true), Times.Once);
		service.Verify(x => x.FieldChanged(replacement, true), Times.Once);
		service.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void Futuremud_ReferencedDefinitionRegistryChanges_InvalidateResolvedSourceReferences()
	{
		var service = new Mock<IEnvironmentalMagicService>();
		using var world = CreateWorld(service.Object);
		var resource = Mock.Of<IMagicResource>(x => x.Id == 1L);
		var generator = Mock.Of<IMagicResourceRegenerator>(x => x.Id == 1L);
		var prog = Mock.Of<IFutureProg>(x => x.Id == 1L);
		var forage = Mock.Of<IForagableProfile>(x => x.Id == 1L && x.RevisionNumber == 1);

		world.Add(resource);
		world.Add(generator);
		world.Add(prog);
		world.Add(forage);
		service.Verify(x => x.SourceDefinitionChanged(), Times.Exactly(4));
		world.Destroy(prog);
		world.Destroy(forage);
		Assert.IsNull(world.FutureProgs.Get(1));
		Assert.IsNull(world.ForagableProfiles.Get(1));
		service.Verify(x => x.SourceDefinitionChanged(), Times.Exactly(6));
		service.VerifyNoOtherCalls();
	}

	private static Futuremud CreateWorld(IEnvironmentalMagicService service)
	{
		var world = TestObjectFactory.CreateUninitialized<Futuremud>();
		typeof(Futuremud).GetProperty(nameof(Futuremud.EnvironmentalMagic))!.SetValue(world, service);
		SetField(world, "_cells", new All<ICell>());
		SetField(world, "_listeners", new All<ITemporalListener>());
		SetField(world, "_agricultureFields", new All<IAgricultureField>());
		SetField(world, "_magicResources", new All<IMagicResource>());
		SetField(world, "_magicResourceRegenerators", new All<IMagicResourceRegenerator>());
		SetField(world, "_futureProgs", new All<IFutureProg>());
		SetField(world, "_foragableProfiles", new RevisableAll<IForagableProfile>());
		return world;
	}

	private static void SetField(Futuremud world, string name, object value) =>
		typeof(Futuremud).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(world, value);
}
