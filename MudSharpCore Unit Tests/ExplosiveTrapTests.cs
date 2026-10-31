#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Body;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Traps;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ExplosiveTrapTests
{
	[TestMethod]
	[DataRow(EventType.ItemOpened)]
	[DataRow(EventType.ItemAboutToBeJunked)]
	public void ItemTrap_ConsumesChargeBeforePayload_AndDoesNotRepeat(EventType eventType)
	{
		var world = new Mock<IFuturemud>();
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Gameworld).Returns(world.Object);
		var trigger = new TrapTriggerDefinition(eventType == EventType.ItemOpened ? TrapTriggerType.Openable : TrapTriggerType.Junked);
		var payload = new TrapPayloadDefinition(TrapPayloadType.ExecuteProg);
		payload.SetParameter("prog", "42");
		var template = new Mock<ITrapTemplate>();
		template.SetupGet(x => x.Id).Returns(1);
		template.SetupGet(x => x.Charges).Returns(1);
		template.SetupGet(x => x.Triggers).Returns([trigger]);
		template.SetupGet(x => x.Payloads).Returns([payload]);
		var templates = new RevisableAll<ITrapTemplate>(); templates.Add(template.Object);
		world.SetupGet(x => x.TrapTemplates).Returns(templates);
		var trap = new TrapEffect(item.Object, template.Object);
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.Id).Returns(42);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		var calls = 0;
		prog.Setup(x => x.Execute(It.IsAny<object[]>())).Callback(() =>
		{
			calls++;
			Assert.AreEqual(0, trap.ChargesRemaining);
			Assert.IsFalse(trap.ForceTrigger());
		}).Returns(true);
		var progs = new All<IFutureProg>(); progs.Add(prog.Object); world.SetupGet(x => x.FutureProgs).Returns(progs);
		Assert.IsTrue(trap.HandleEvent(eventType, null!, item.Object));
		Assert.IsFalse(trap.HandleEvent(eventType, null!, item.Object));
		Assert.AreEqual(1, calls);
		Assert.AreEqual(TrapState.Spent, trap.State);
		var saved = trap.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		Assert.AreEqual("Spent", saved.Descendants("State").Single().Value);
		Assert.AreEqual("0", saved.Descendants("ChargesRemaining").Single().Value);
		var loaded = (TrapEffect)Activator.CreateInstance(typeof(TrapEffect), BindingFlags.NonPublic | BindingFlags.Instance,
			null, [saved, item.Object], null)!;
		Assert.IsFalse(loaded.HandleEvent(eventType, null!, item.Object));
		Assert.AreEqual(1, calls);
	}

	[TestMethod]
	public void Template_OptionalPoliciesPersist_AndKnowledgeAliasDoesNotTogglePreassembly()
	{
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>()); world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		var template = new TrapTemplate(new MudSharp.Models.TrapTemplate
		{
			Id = 1, Name = "test", EditableItem = new MudSharp.Models.EditableItem(),
			Definition = "<TrapTemplate closeditem='true' preassembled='true' setupprog='42'><Triggers/><Payloads/><Components/></TrapTemplate>"
		}, world.Object);
		Assert.IsTrue(template.ClosedItemAnchor); Assert.IsTrue(template.Preassembled);
		var actor = new Mock<ICharacter>();
		Assert.IsTrue(template.BuildingCommand(actor.Object, new StringStack("knowledgeprog none")));
		Assert.IsTrue(template.Preassembled);
		var xml = (XElement)typeof(TrapTemplate).GetMethod("SaveDefinition", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(template, null)!;
		Assert.AreEqual("42", (string?)xml.Attribute("setupprog"));
		Assert.AreEqual("true", (string?)xml.Attribute("closeditem"));
		Assert.AreEqual("true", (string?)xml.Attribute("preassembled"));
	}

	[TestMethod]
	public void SharedDoorAnchor_IsAvailableFromEitherEndpoint()
	{
		var doorItem = new Mock<IGameItem>(); var door = new Mock<IDoor>(); var exit = new Mock<IExit>();
		doorItem.Setup(x => x.GetItemType<IDoor>()).Returns(door.Object);
		door.SetupGet(x => x.InstalledExit).Returns(exit.Object);
		var body = new Mock<IBody>(); body.SetupGet(x => x.ItemsInHands).Returns([]);
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.Body).Returns(body.Object);
		foreach (var room in new[] { new Mock<IRoom>(), new Mock<IRoom>() })
		{
			actor.SetupGet(x => x.Location).Returns(room.Object);
			var side = new Mock<IRoomExit>(); side.SetupGet(x => x.Exit).Returns(exit.Object);
			room.Setup(x => x.ExitsFor(actor.Object, true)).Returns([side.Object]);
			Assert.IsTrue(TrapModule.AnchorStillAvailable(actor.Object, doorItem.Object, null));
		}
	}
}
