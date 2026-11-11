#nullable enable

using System.Linq;
using System.Reflection;
using MudSharp.Commands.Modules;
using MudSharp.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Community;

namespace MudSharp_Unit_Tests.Community;

public partial class ClanElectionGovernanceTests
{
	[TestMethod]
	[TestCategory("Integration")]
	public void CatholicAddresses_ClonedSubjectPredicates_FollowLiveOrdinationAndKeepProfessionIndependent()
	{
		using var setup = new CatholicFixture();
		var church = setup.Clone("catholic.church", "addresschurch"); setup.Activate(church);
		var order = setup.Clone("catholic.dominican.order", "addressorder");
		setup.Bind(order, "church", church); setup.Activate(order);
		var subject = setup.Member(order, "professed");
		setup.Member(church, "lay_member", character: subject);
		subject.SetupGet(x => x.ClanMemberships).Returns(() => setup.F.World.Object.Clans
			.SelectMany(x => x.Memberships).Where(x => x.MemberId == subject.Object.Id));
		var profession = setup.Rank(order, "professed");
		var initial = ClanAddressResolver.Resolve(null, subject.Object, order);
		Assert.AreEqual(ClanAddressStatus.Text, initial.Status);
		Assert.AreEqual("Brother", initial.Text);
		var churchMember = church.Memberships.Single(x => x.MemberId == subject.Object.Id);
		churchMember.Rank = setup.Rank(church, "priest");
		Assert.AreEqual("Father", ClanAddressResolver.Resolve(null, subject.Object, order).Text);
		var priestRule = profession.FormsOfAddress.Single(x => x.TemplateKey?.EndsWith(":priest") == true);
		var boundKey = priestRule.TemplateKey;
		typeof(ClanModule).GetMethod("ClanEditRank", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [setup.F.Actor.Object, new StringStack($"\"{profession.Name}\" address rule {priestRule.Id} text \"Reverend Father\""), order]);
		Assert.AreEqual(boundKey, profession.FormsOfAddress.Single(x => x.Id == priestRule.Id).TemplateKey);
		Assert.AreEqual("Reverend Father", ClanAddressResolver.Resolve(null, subject.Object, order).Text);
		Assert.AreSame(profession, order.Memberships.Single(x => x.MemberId == subject.Object.Id).Rank);
		churchMember.IsArchivedMembership = true;
		Assert.AreEqual("Brother", ClanAddressResolver.Resolve(null, subject.Object, order).Text);
		churchMember.IsArchivedMembership = false;
		var secondChurch = setup.Clone("catholic.church", "otheraddresschurch"); setup.Activate(secondChurch);
		var profile = ClanTemplateProfile.Read(setup.F.World.Object, order.Id)!;
		profile.Bindings["church"].ClanId = secondChurch.Id;
		ClanTemplateProvisioning.SaveProfile(setup.F.World.Object, profile);
		Assert.AreEqual("Brother", ClanAddressResolver.Resolve(null, subject.Object, order).Text,
			"A predicate must use its current bound Church, not the template or an unrelated affiliation.");
		var before = setup.F.Database.Context.Clans.Select(x => x.Id).ToList();
		var editedRule = profession.FormsOfAddress.Single(x => x.Id == priestRule.Id);
		editedRule.TemplateKey = null;
		typeof(ClanModule).GetMethod("ClanCreateStagedTemplate", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [setup.F.Actor.Object, new StringStack($"{order.Alias} badbinding Bad Binding")]);
		CollectionAssert.AreEquivalent(before, setup.F.Database.Context.Clans.Select(x => x.Id).ToList());
		editedRule.TemplateKey = boundKey;
		typeof(ClanModule).GetMethod("ClanCreateStagedTemplate", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [setup.F.Actor.Object, new StringStack($"{order.Alias} secondaddressorder Edited Source Copy")]);
		var clone = setup.F.World.Object.Clans.Single(x => x.Alias == "secondaddressorder");
		var copied = setup.Rank(clone, "professed");
		Assert.AreEqual("Reverend Father", copied.FormsOfAddress.Single(x => x.TemplateKey == boundKey).Text);
		Assert.AreNotEqual(profession.FormsOfAddress[0].Id, copied.FormsOfAddress[0].Id);
		Assert.AreNotEqual(profession.FormsOfAddress[0].FutureProgId, copied.FormsOfAddress[0].FutureProgId);
		Assert.IsTrue(copied.FormsOfAddress.Where(x => x.FutureProgId is not null)
			.All(x => setup.F.World.Object.FutureProgs.Get(x.FutureProgId!.Value).Compile()));
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void CatholicAddresses_FemaleHouseVariant_ChangesCourtesyRulesWithoutRemovingBuilderExceptions()
	{
		using var setup = new CatholicFixture();
		var abbey = setup.Clone("catholic.generic.abbey", "addressabbey");
		var professed = setup.Rank(abbey, "professed");
		var custom = new ClanAddressRule { Text = "Beloved Teacher", Relations = ClanAddressRelation.Higher, IsPublic = true };
		professed.FormsOfAddress.Insert(0, custom);
		setup.Variant(abbey, "female_monastic_house");
		Assert.Contains(custom, professed.FormsOfAddress);
		Assert.AreEqual("Sister", professed.FormsOfAddress.Last().Text);
		Assert.AreEqual("Mother Abbess", setup.Office(abbey, "superior").FormsOfAddress.Last().Text);
		var row = setup.F.Database.Context.Ranks.Find(professed.Id)!;
		Assert.IsTrue(row.FormsOfAddress.Any(x => x.Id == custom.Id));
		Assert.AreEqual("Sister", row.FormsOfAddress.OrderBy(x => x.Order).Last().Text);
	}
}
