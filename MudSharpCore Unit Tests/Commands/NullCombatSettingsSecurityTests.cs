#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Commands.Modules;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
public class NullCombatSettingsSecurityTests
{
	[DataTestMethod]
	[DataRow("Combat", "combat show")]
	[DataRow("Combat", "combat set")]
	[DataRow("Combat", "combat config name New")]
	[DataRow("Combat", "combat targets")]
	[DataRow("Grapple", "grapple")]
	[DataRow("Strangle", "strangle")]
	[DataRow("Flee", "flee")]
	[DataRow("Ward", "ward")]
	[DataRow("Clinch", "clinch")]
	public void CombatCommand_NoCurrentSetting_ReportsRecoveryWithoutMutation(string method, string command)
	{
		var (actor, output, _) = Fixture();
		Invoke(method, actor.Object, command);
		output.Verify(x => x.Send(It.Is<string>(s => s.Contains("usable combat setting") && s.Contains("combat list")),
			It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
		actor.VerifySet(x => x.CombatStrategyMode = It.IsAny<CombatStrategyMode>(), Times.Never);
		actor.VerifySet(x => x.CombatSettings = It.IsAny<ICharacterCombatSettings>(), Times.Never);
	}

	[DataTestMethod]
	[DataRow("combat config help", "You can edit")]
	[DataRow("combat clone", "Which combat setting")]
	[DataRow("combat list", "Combat Settings")]
	public void CombatCommand_NoCurrentSetting_HelpAndListingRemainUsable(string command, string expected)
	{
		var (actor, output, _) = Fixture();
		Invoke("Combat", actor.Object, command);
		output.Verify(x => x.Send(It.Is<string>(s => s.Contains(expected)), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
	}

	[TestMethod]
	public void CombatCommand_NoCurrentSetting_CanShowAndAdoptOwnedAvailableSetting()
	{
		var (actor, output, repository) = Fixture();
		var setting = new Mock<ICharacterCombatSettings>();
		setting.SetupGet(x => x.Name).Returns("Usable");
		setting.SetupGet(x => x.CharacterOwnerId).Returns(42);
		setting.Setup(x => x.Show(actor.Object)).Returns("Setting details");
		repository.As<IEnumerable<ICharacterCombatSettings>>().Setup(x => x.GetEnumerator())
			.Returns(() => ((IEnumerable<ICharacterCombatSettings>)new[] { setting.Object }).GetEnumerator());
		repository.Setup(x => x.GetByName("Usable")).Returns(setting.Object);
		Invoke("Combat", actor.Object, "combat show Usable");
		output.Verify(x => x.Send("Setting details", It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
		Invoke("Combat", actor.Object, "combat set Usable");
		actor.VerifySet(x => x.CombatSettings = setting.Object, Times.Once);
	}

	private static (Mock<ICharacter> Actor, Mock<IOutputHandler> Output,
		Mock<IUneditableAll<ICharacterCombatSettings>> Repository) Fixture()
	{
		var actor = new Mock<ICharacter>();
		var output = new Mock<IOutputHandler>();
		var world = new Mock<IFuturemud>();
		var account = new Mock<IAccount>();
		var repository = new Mock<IUneditableAll<ICharacterCombatSettings>>();
		repository.As<IEnumerable<ICharacterCombatSettings>>().Setup(x => x.GetEnumerator())
			.Returns(() => ((IEnumerable<ICharacterCombatSettings>)Array.Empty<ICharacterCombatSettings>()).GetEnumerator());
		world.SetupGet(x => x.CharacterCombatSettings).Returns(repository.Object);
		world.Setup(x => x.GetStaticInt("MaximumCombatSettingsPerPlayer")).Returns(10);
		actor.SetupGet(x => x.Id).Returns(42);
		actor.SetupGet(x => x.LineFormatLength).Returns(120);
		actor.SetupGet(x => x.Account).Returns(account.Object);
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		return (actor, output, repository);
	}

	private static void Invoke(string method, ICharacter actor, string command)
	{
		typeof(CombatModule).GetMethod(method, BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, [actor, command]);
	}
}
