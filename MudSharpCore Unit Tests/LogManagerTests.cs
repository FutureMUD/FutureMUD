#nullable enable

using System.Collections;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Logging;

namespace MudSharp_Unit_Tests;

[TestClass]
public class LogManagerTests
{
	[TestMethod]
	public void LogCharacterCommand_FocusedSecondary_QueuesIdentityCharacterId()
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticBool("LogNPCCommands")).Returns(false);
		var manager = new LogManager(gameworld.Object);
		var character = BuildFocusedSecondaryActor(identityId: 10, instanceId: 12345);

		manager.LogCharacterCommand(character.Object, "look");

		var queued = QueuedCommands(manager).Single();
		Assert.AreEqual(10L, ReadProperty<long>(queued, "CharacterId"));
		Assert.AreEqual(99L, ReadProperty<long>(queued, "CellId"));
		Assert.AreEqual("look", ReadProperty<string>(queued, "Command"));
	}

	private static Mock<ICharacter> BuildFocusedSecondaryActor(long identityId, long instanceId)
	{
		var identity = new Mock<ICharacterIdentity>();
		identity.SetupGet(x => x.Id).Returns(identityId);

		var account = new Mock<IAccount>();
		account.SetupGet(x => x.Id).Returns(5);

		var location = new Mock<ICell>();
		location.SetupGet(x => x.Id).Returns(99);

		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(instanceId);
		character.SetupGet(x => x.Identity).Returns(identity.Object);
		character.SetupGet(x => x.Account).Returns(account.Object);
		character.SetupGet(x => x.Location).Returns(location.Object);
		character.SetupGet(x => x.IsPlayerCharacter).Returns(true);
		return character;
	}

	private static object[] QueuedCommands(LogManager manager)
	{
		var field = typeof(LogManager).GetField("_commandQueue", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.IsNotNull(field);
		var queue = (IEnumerable)field.GetValue(manager)!;
		return queue.Cast<object>().ToArray();
	}

	private static T ReadProperty<T>(object target, string propertyName)
	{
		var property = target.GetType().GetProperty(propertyName,
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		Assert.IsNotNull(property);
		return (T)property.GetValue(target)!;
	}
}
