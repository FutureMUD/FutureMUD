#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Work.Agriculture;
using MudSharp.Work.Projects;
using MudSharp.Work.Projects.Actions;
using MudSharp.Work.Projects.ConcreteTypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AgricultureProjectStartTests
{
	[TestMethod]
	public void CatalogueStart_RequiresFieldContextForAgricultureActionsInAnyPhase()
	{
		var definition = TestObjectFactory.CreateUninitialized<LocalProject>();
		typeof(EditableItem).GetProperty(nameof(Project.Status))!.SetValue(definition, RevisionStatus.Current);
		var policy = new Mock<IFutureProg>();
		policy.Setup(x => x.Execute<bool?>(It.IsAny<object[]>())).Returns(true);
		typeof(Project).GetProperty(nameof(Project.CanInitiateProg))!.SetValue(definition, policy.Object);
		var phases = new List<IProjectPhase> { Mock.Of<IProjectPhase>() };
		typeof(Project).GetField("_phases", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(definition, phases);
		var actor = Mock.Of<ICharacter>();
		Assert.IsTrue(definition.CanInitiateProject(actor));

		var fieldPhase = new Mock<IProjectPhase>();
		fieldPhase.SetupGet(x => x.CompletionActions).Returns(new[] { TestObjectFactory.CreateUninitialized<AgricultureOperationAction>() });
		phases.Add(fieldPhase.Object);
		Assert.IsFalse(definition.CanInitiateProject(actor));
		StringAssert.Contains(definition.WhyCannotInitiateProject(actor), "FIELD START");
		Assert.IsTrue(definition.CanInitiateProject(actor, hasFieldContext: true));
		policy.Setup(x => x.Execute<bool?>(It.IsAny<object[]>())).Returns(false);
		Assert.IsFalse(definition.CanInitiateProject(actor, hasFieldContext: true));
	}

	[TestMethod]
	public void FieldStart_DistantFieldRejectsBeforeCreatingWork()
	{
		var actor = Mock.Of<ICharacter>(x => x.Location == Mock.Of<IRoom>());
		var field = new Mock<IAgricultureField>();
		field.SetupGet(x => x.Room).Returns(Mock.Of<IRoom>());
		Assert.IsFalse(AgricultureProjectStarter.TryStart(actor, field.Object, Mock.Of<IAgricultureOperation>(), null, out var reason));
		StringAssert.Contains(reason, "at the field");
		field.Verify(x => x.CanBeginOperation(It.IsAny<ICharacter>(), It.IsAny<IAgricultureOperation>(),
			It.IsAny<IFrameworkItem>(), out It.Ref<string>.IsAny), Times.Never);
	}

	[TestMethod]
	[DoNotParallelize]
	public void FieldStart_OnsiteSavesSiteOperationAndTargetWithTheActiveProject()
	{
		using var database = new InMemoryDatabaseTestScope();
		var world = new Mock<IFuturemud>();
		var manager = new Mock<ISaveManager>();
		world.SetupGet(x => x.SaveManager).Returns(manager.Object);
		var room = Mock.Of<IRoom>(x => x.Id == 10);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		actor.SetupGet(x => x.Id).Returns(7);
		actor.SetupGet(x => x.Location).Returns(room);
		var phase = Mock.Of<IProjectPhase>(x => x.Id == 32);
		var template = new Mock<IProject>();
		template.SetupGet(x => x.Id).Returns(30);
		template.SetupGet(x => x.Phases).Returns(new[] { phase });
		template.SetupGet(x => x.Gameworld).Returns(world.Object);
		template.Setup(x => x.CanInitiateProject(actor.Object, true)).Returns(true);
		var operation = Mock.Of<IAgricultureOperation>(x => x.Id == 19 && x.Project == template.Object &&
			x.TargetType == AgricultureTargetType.Crop);
		var target = Mock.Of<IFrameworkItem>(x => x.Id == 12 && x.Name == "rye");
		var field = new Mock<IAgricultureField>();
		field.SetupGet(x => x.Id).Returns(11);
		field.SetupGet(x => x.Room).Returns(room);
		field.Setup(x => x.CanBeginOperation(actor.Object, operation, target, out It.Ref<string>.IsAny)).Returns(true);
		ILateInitialisingItem? pending = null;
		manager.Setup(x => x.AddInitialisation(It.IsAny<ILateInitialisingItem>()))
			.Callback<ILateInitialisingItem>(item => pending = item);
		void FlushPending()
		{
			if (pending is null) return;
			var item = pending;
			pending = null;
			var assignId = item.InitialiseItem();
			database.Context.SaveChanges();
			assignId();
		}

		manager.Setup(x => x.Flush()).Callback(FlushPending);
		manager.Setup(x => x.FlushInitialisationQueue()).Callback(FlushPending);
		Assert.IsTrue(AgricultureProjectStarter.TryStart(actor.Object, field.Object, operation, target, out var reason), reason);
		database.Context.ChangeTracker.Clear();
		var active = database.Context.ActiveProjects.Single();
		var context = database.Context.AgricultureProjectContexts.Single();
		Assert.AreEqual(room.Id, active.RoomId);
		Assert.AreEqual(active.Id, context.ActiveProjectId);
		Assert.AreEqual(field.Object.Id, context.AgricultureFieldId);
		Assert.AreEqual(operation.Id, context.OperationId);
		Assert.AreEqual(target.Id, context.TargetId);
		Assert.AreEqual(actor.Object.Id, context.ActorId);
	}
}
