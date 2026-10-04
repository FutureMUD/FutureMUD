#nullable enable

using System;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Commands;
using MudSharp.Commands.Trees;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Movement;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CommandableAiTests
{
	[DataTestMethod]
	[DataRow("allowed", 1)]
	[DataRow("combat", 0)]
	[DataRow("moving", 0)]
	[DataRow("state", 0)]
	[DataRow("delay", 0)]
	[DataRow("unauthorised", 0)]
	[DataRow("unlisted", 0)]
	[DataRow("unknown", 0)]
	public void Commandable_ResolvesAliasThenUsesNativeCommandGuards(string condition, int expected)
	{
		var world = new Mock<IFuturemud>();
		var allow = new Mock<IFutureProg>(); allow.SetupGet(x => x.Id).Returns(1);
		allow.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(condition != "unauthorised");
		var progs = new All<IFutureProg>(); progs.Add(allow.Object); world.SetupGet(x => x.FutureProgs).Returns(progs);
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		actor.SetupGet(x => x.PermissionLevel).Returns(PermissionLevel.NPC);
		actor.SetupGet(x => x.State).Returns(condition == "state" ? CharacterState.Sleeping : CharacterState.Awake);
		if (condition == "combat") actor.SetupGet(x => x.Combat).Returns(Mock.Of<ICombat>());
		if (condition == "moving") actor.SetupGet(x => x.Movement).Returns(Mock.Of<IMovement>());
		if (condition == "delay") actor.Setup(x => x.EffectsOfType<ICommandDelay>()).Returns([Mock.Of<ICommandDelay>(x => x.IsDelayed("Follow") == true)]);
		var calls = 0; var manager = new CharacterCommandManager();
		manager.Add(new[] { "follow", "fol" }, new Command<ICharacter>((_, text) => { Assert.AreEqual("fol caster", text); calls++; },
			states: CharacterState.Awake, name: "Follow", noCombatCommand: true, noMovementCommand: true));
		actor.SetupGet(x => x.CommandTree).Returns(Mock.Of<ICharacterCommandTree>(x => x.Commands == manager));
		var ai = (CommandableAI)typeof(CommandableAI).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
			null, [typeof(MudSharp.Models.ArtificialIntelligence), typeof(IFuturemud)], null)!.Invoke([
			new MudSharp.Models.ArtificialIntelligence { Id = 1, Name = "test", Type = "Commandable", Definition = new XElement("Definition",
				new XElement("CanCommandProg", 1), new XElement("IncludedCommands", new XElement("Command", condition == "unlisted" ? "hit" : "follow"))).ToString() }, world.Object]);
		Assert.IsTrue(ai.HandleEvent(EventType.CommandIssuedToCharacter, actor.Object, Mock.Of<ICharacter>(), condition == "unknown" ? "nonesuch" : "fol caster"));
		Assert.AreEqual(expected, calls);
	}
}
