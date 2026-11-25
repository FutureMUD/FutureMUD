#nullable enable

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.NPC.AI.Groups;

namespace MudSharp_Unit_Tests;

public partial class DiscordRemoteServiceTests
{
	[TestMethod]
	public void NpcOverview_CountsEachNpcOncePerTypeAndShowsGroupOnlyMembersAsUnassigned()
	{
		var f = new Fixture();
		var npcs = new All<ICharacter>();
		var groups = new All<IGroupAI>();
		f.World.SetupGet(x => x.NPCs).Returns(npcs);
		f.World.SetupGet(x => x.GroupAIs).Returns(groups);
		var empty = f.Service.Process(f.Request("npc.overview")).Text;
		StringAssert.Contains(empty, "Active NPCs: 0");
		StringAssert.Contains(empty, "Active group controllers (0)");

		var first = Npc(1, "Wanderer", "Aggressor", "Wanderer");
		var second = Npc(2, "Wanderer");
		var groupOnly = Npc(3);
		npcs.Add(first);
		npcs.Add(second);
		npcs.Add(groupOnly);
		groups.Add(Mock.Of<IGroupAI>(g => g.Id == 42 && g.Name == "North Herd" &&
			g.GroupAIType.Name == "Herd" && g.GroupMembers == new[] { groupOnly }));
		var overview = f.Service.Process(f.Request("npc.overview"));
		Assert.AreEqual("completed", overview.Status);
		StringAssert.Contains(overview.Text, "Active NPCs: 3 | No individual AI assigned: 1 | Individual AI assignments: 4");
		StringAssert.Contains(overview.Text, "Wanderer — 2 / 3");
		StringAssert.Contains(overview.Text, "Aggressor — 1 / 1");
		StringAssert.Contains(overview.Text, "#42 North Herd — Herd, members: 1");
		StringAssert.Contains(overview.Text, "/npc group id:");
		var secondBody = Npc(1, "Wanderer");
		Mock.Get(secondBody).SetupGet(x => x.InstanceId).Returns(21);
		var dormant = Npc(1, "Unused");
		Mock.Get(dormant).SetupGet(x => x.InstanceId).Returns(22);
		Mock.Get(dormant).SetupGet(x => x.IsEmbodied).Returns(false);
		Mock.Get(first.Identity).SetupGet(x => x.Instances).Returns([
			(ICharacterInstance)first, (ICharacterInstance)secondBody, (ICharacterInstance)dormant
		]);
		var expanded = f.Service.Process(f.Request("npc.overview")).Text;
		StringAssert.Contains(expanded, "Active NPCs: 4 | No individual AI assigned: 1 | Individual AI assignments: 5");
		StringAssert.Contains(expanded, "Wanderer — 3 / 4");
		Assert.IsFalse(expanded.Contains("Unused"));
		for (var id = 4; id <= 10; id++) { npcs.Add(Npc(id, $"Other{id}")); }
		StringAssert.Contains(f.Service.Process(f.Request("npc.overview")).Text, "/npc overview page:2");
		var pageTwo = f.Service.Process(f.Request("npc.overview") with { Arguments = """{"page":2}""" }).Text;
		StringAssert.Contains(pageTwo, "Other9 — 1 / 1");
		Assert.IsFalse(pageTwo.Contains("North Herd") || pageTwo.Contains("Wanderer —"));

		static INPC Npc(long id, params string[] types)
		{
			var assignments = types.Select(type => Mock.Of<IArtificialIntelligence>(x => x.AIType == type)).ToArray();
			var npc = new Mock<INPC>();
			var instance = npc.As<ICharacterInstance>();
			npc.SetupGet(x => x.Id).Returns(id);
			npc.SetupGet(x => x.InstanceId).Returns(id);
			npc.SetupGet(x => x.IsEmbodied).Returns(true);
			npc.SetupGet(x => x.AIs).Returns(assignments);
			var identity = new Mock<ICharacterIdentity>();
			identity.SetupGet(x => x.Instances).Returns([instance.Object]);
			npc.SetupGet(x => x.Identity).Returns(identity.Object);
			return npc.Object;
		}
	}

	[TestMethod]
	public void NpcGroup_ListsOnlySelectedMembersByPhysicalInstanceAcrossPages()
	{
		var f = new Fixture();
		var members = Enumerable.Range(1, 9).Reverse()
			.Select(i => Mock.Of<ICharacter>(x => x.Id == 11 && x.InstanceId == i && x.Name == $"Wolf {i}"))
			.ToList();
		var roles = new Dictionary<ICharacter, GroupRole>(CharacterPhysicalInstanceEqualityComparer.Instance)
			{ [members[0]] = GroupRole.Leader };
		var group = Mock.Of<IGroupAI>(x => x.Id == 9 && x.Name == "Winter Pack" && x.GroupAIType.Name == "Pack" &&
			x.GroupMembers == members && x.GroupRoles == roles);
		var groups = new All<IGroupAI> { group };
		for (var i = 1; i < 9; i++)
		{
			groups.Add(Mock.Of<IGroupAI>(x => x.Id == i && x.Name == $"Empty {i}" && x.GroupAIType.Name == "Pack"));
		}
		f.World.SetupGet(x => x.NPCs).Returns(new All<ICharacter>());
		f.World.SetupGet(x => x.GroupAIs).Returns(groups);
		var second = f.Service.Process(f.Request("npc.overview") with { Arguments = """{"page":2}""" }).Text;
		StringAssert.Contains(second, "#9 Winter Pack — Pack, members: 9");
		string Roster(string arguments) => f.Service.Process(f.Request("npc.group") with { Arguments = arguments }).Text;
		var firstPage = Roster("""{"id":9}""");
		StringAssert.Contains(firstPage, "#11:1 Wolf 1 — Adult");
		StringAssert.Contains(firstPage, "/npc group id:9 page:2");
		Assert.IsFalse(firstPage.Contains("Wolf 9"));
		var last = Roster("""{"id":9,"page":2}""");
		StringAssert.Contains(last, "#11:9 Wolf 9 — Leader");
		Assert.IsFalse(last.Contains("Wolf 1"));
		StringAssert.Contains(Roster("""{"id":1}"""), "Members (0)");
		StringAssert.Contains(Roster("""{"id":12}"""), "Unknown group controller");
		StringAssert.Contains(Roster("""{"id":9,"page":0}"""), "start at 1");
		StringAssert.Contains(Roster("""{"id":9,"page":2147483647}"""), "past the end");
	}

	[TestMethod]
	public void NpcReports_RequireCurrentCensusPermissionBeforeReadingWorldData()
	{
		var f = new Fixture();
		foreach (var operation in new[] { "npc.overview", "npc.group" })
		{
			f.Authority.SetupGet(x => x.Level).Returns(PermissionLevel.Admin);
			Assert.AreEqual("denied", f.Service.Process(f.Request(operation)).Status);
			f.Authority.SetupGet(x => x.Level).Returns(PermissionLevel.SeniorAdmin);
			f.Account.SetupGet(x => x.AccountStatus).Returns(AccountStatus.Suspended);
			Assert.AreEqual("denied", f.Service.Process(f.Request(operation)).Status);
			f.Account.SetupGet(x => x.AccountStatus).Returns(AccountStatus.Normal);
		}
		f.Store.Link = null;
		Assert.AreEqual("denied", f.Service.Process(f.Request("npc.group")).Status);
		f.World.VerifyGet(x => x.NPCs, Times.Never);
		f.World.VerifyGet(x => x.GroupAIs, Times.Never);
	}
}
