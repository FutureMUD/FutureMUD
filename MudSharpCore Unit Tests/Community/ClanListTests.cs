#nullable enable

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Community;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests.Community;

[TestClass]
public class ClanListTests
{
	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ClanList_UnresolvedMembers_PreservesMembershipTotalAndCountsResolvedCharacters(bool allUnresolved)
	{
		var world = new Mock<IFuturemud>();
		var actor = new Mock<ICharacter>();
		var output = new Mock<IOutputHandler>();
		string result = "";
		output.Setup(x => x.Send(It.IsAny<string>(), true, false)).Callback<string, bool, bool>((text, _, _) => result = text);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		actor.SetupGet(x => x.Account).Returns(Mock.Of<IAccount>(x => x.LineFormatLength == 200));
		actor.Setup(x => x.GetFormat(It.IsAny<Type>())).Returns<Type>(CultureInfo.InvariantCulture.GetFormat);
		actor.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
		actor.SetupGet(x => x.ClanMemberships).Returns([]);
		var clan = new Mock<IClan>();
		clan.SetupGet(x => x.Id).Returns(17);
		clan.SetupGet(x => x.FullName).Returns("Test Clan");
		clan.SetupGet(x => x.Alias).Returns("test");
		var missing = new Mock<IClanMembership>();
		var alive = new Mock<IClanMembership>();
		var dead = new Mock<IClanMembership>();
		alive.SetupGet(x => x.MemberCharacter).Returns(Mock.Of<ICharacter>(x => x.State == CharacterState.Awake && x.LastLogoutDateTime == DateTime.UtcNow.AddDays(-1)));
		dead.SetupGet(x => x.MemberCharacter).Returns(Mock.Of<ICharacter>(x => x.State == CharacterState.Dead && x.LastLogoutDateTime == DateTime.UtcNow.AddDays(-90)));
		clan.SetupGet(x => x.Memberships).Returns(allUnresolved ? [missing.Object] : [missing.Object, alive.Object, dead.Object]);
		var clans = new All<IClan>(); clans.Add(clan.Object);
		world.SetupGet(x => x.Clans).Returns(clans);

		typeof(ClanModule).GetMethod("ClanList", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, [actor.Object, new StringStack("")]);

		var row = result.RawText().Split('\n').Single(x => x.Contains("Test Clan"));
		var columns = row.Split('|').Select(x => x.Trim()).ToArray();
		Assert.AreEqual(allUnresolved ? "1" : "3", columns[4]);
		Assert.AreEqual(allUnresolved ? "0" : "1", columns[5]);
		Assert.AreEqual(allUnresolved ? "0" : "1", columns[6]);
		missing.VerifyGet(x => x.MemberCharacter, Times.Once);
	}
}
