#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Community;
using MudSharp.Framework;
using MudSharp.Form.Shape;
using MudSharp.FutureProg.Variables;
using MudSharp.FutureProg;
using Clan = MudSharp.Community.Clan;

namespace MudSharp_Unit_Tests.Community;

public partial class ClanElectionGovernanceTests
{
	[TestMethod]
	[TestCategory("Integration")]
	public void OtherStockAddresses_NativeClone_PreservesConditionalRulesPrioritiesAndSharedPredicates()
	{
		using var f = new Fixture();
		var source = InstallOtherAddressFixture(f, "SetupPeerage");
		var clans = f.World.Object.Clans;
		typeof(ClanModule).GetMethod("ClanCreateStagedTemplate", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [f.Actor.Object, new StringStack($"{source.Alias} addresscopy Address Copy")]);
		var clone = clans.Single(x => x.Alias == "addresscopy");
		var original = source.Ranks.First(x => x.FormsOfAddress.Any(y => y.FutureProgId is not null));
		var copied = clone.Ranks.Single(x => x.Name == original.Name);
		CollectionAssert.AreEqual(original.FormsOfAddress.Select(x => (x.Text, x.Relations, x.IsPublic, x.FutureProgId)).ToList(),
			copied.FormsOfAddress.Select(x => (x.Text, x.Relations, x.IsPublic, x.FutureProgId)).ToList());
		Assert.AreNotEqual(original.FormsOfAddress[0].Id, copied.FormsOfAddress[0].Id);
		foreach (var office in source.Appointments.Where(x => x.FormsOfAddress.Count > 0))
			Assert.AreEqual(office.AddressPriority, clone.Appointments.Single(x => x.Name == office.Name).AddressPriority);
		clone.IsTemplate = false;
		var subject = new Mock<ICharacter>(); subject.SetupGet(x => x.Id).Returns(900);
		subject.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		subject.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
		subject.SetupGet(x => x.GetObject).Returns(subject.Object);
		subject.Setup(x => x.GetProperty("gender")).Returns(new GenderVariable(Gender.Female));
		var membership = Mock.Of<IClanMembership>(x => x.Clan == clone && x.Rank == copied && x.IsArchivedMembership == false);
		Mock.Get(membership).SetupGet(x => x.Appointments).Returns([]);
		subject.SetupGet(x => x.ClanMemberships).Returns([membership]);
		Assert.AreEqual(original.FormsOfAddress[0].Text, ClanAddressResolver.Resolve(null, subject.Object, clone).Text);
	}
	[TestMethod]
	[TestCategory("Integration")]
	[DataRow(Gender.Male, "Sir")]
	[DataRow(Gender.Female, "Ma'am")]
	[DataRow(Gender.NonBinary, "Captain")]
	public void OtherStockAddresses_OfficerCourtesy_UsesJuniorRelationAndRetainsRankFallbacks(Gender gender, string lowerAddress)
	{
		using var f = new Fixture();
		var source = InstallOtherAddressFixture(f, "SetupMilitaryClan");
		typeof(ClanModule).GetMethod("ClanCreateStagedTemplate", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [f.Actor.Object, new StringStack($"{source.Alias} officeraddresscopy Officer Address Copy")]);
		var clan = f.World.Object.Clans.Single(x => x.Alias == "officeraddresscopy");
		clan.IsTemplate = false;
		var captain = clan.Ranks.Single(x => x.Name == "Captain");
		var privateRank = clan.Ranks.Single(x => x.Name == "Private");
		var (subject, subjectMember) = Character(900, captain, gender);
		var (speaker, speakerMember) = Character(901, privateRank, Gender.Male);
		Assert.AreEqual(lowerAddress, ClanAddressResolver.Resolve(speaker.Object, subject.Object, clan).Text);
		speakerMember.Object.Rank = captain;
		Assert.AreEqual("Captain", ClanAddressResolver.Resolve(speaker.Object, subject.Object, clan).Text);
		speakerMember.Object.Rank = clan.Ranks.Single(x => x.Name == "Major");
		Assert.AreEqual("Captain", ClanAddressResolver.Resolve(speaker.Object, subject.Object, clan).Text);
		speaker.SetupGet(x => x.ClanMemberships).Returns([]);
		Assert.AreEqual("Captain", ClanAddressResolver.Resolve(speaker.Object, subject.Object, clan).Text);
		Assert.AreEqual("Captain", ClanAddressResolver.Resolve(null, subject.Object, clan).Text);
		speaker.SetupGet(x => x.ClanMemberships).Returns([speakerMember.Object]);
		speakerMember.Object.Rank = privateRank;
		subjectMember.Object.Rank = clan.Ranks.Single(x => x.Name == "General");
		subjectMember.Object.Appointments.Add(clan.Appointments.Single(x => x.Name == "General of the Army"));
		Assert.AreEqual(gender == Gender.NonBinary ? "General" : lowerAddress,
			ClanAddressResolver.Resolve(speaker.Object, subject.Object, clan).Text, "Appointment precedence must retain junior courtesy.");
		subjectMember.Object.Appointments.Clear();
		subjectMember.Object.Rank = clan.Ranks.Single(x => x.Name == "Sergeant");
		Assert.AreEqual("Sergeant", ClanAddressResolver.Resolve(speaker.Object, subject.Object, clan).Text,
			"NCO address must not inherit the commissioned/warrant-officer courtesy rules.");

		(Mock<ICharacter>, Mock<IClanMembership>) Character(long id, IRank rank, Gender sex)
		{
			var ch = new Mock<ICharacter>();
			ch.SetupGet(x => x.Id).Returns(id);
			ch.SetupGet(x => x.Gameworld).Returns(f.World.Object);
			ch.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
			ch.SetupGet(x => x.GetObject).Returns(ch.Object);
			ch.Setup(x => x.GetProperty("gender")).Returns(new GenderVariable(sex));
			ch.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
			var member = new Mock<IClanMembership>();
			member.SetupGet(x => x.Clan).Returns(clan);
			member.SetupProperty(x => x.Rank, rank);
			member.SetupGet(x => x.Appointments).Returns([]);
			member.SetupGet(x => x.NetPrivileges).Returns(ClanPrivilegeType.CanViewMembers);
			ch.SetupGet(x => x.ClanMemberships).Returns([member.Object]);
			return (ch, member);
		}
	}

	private static IClan InstallOtherAddressFixture(Fixture f, string setupMethod)
	{
		MudSharp.FutureProg.FutureProg.Initialise();
		f.Actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(true);
		var context = f.Database.Context;
		context.Clans.Add(new MudSharp.Models.Clan { Id = 1, Name = "Fixture", Alias = "fixture",
			PayIntervalReferenceDate = f.Now.Date.GetDateString(), PayIntervalReferenceTime = f.Now.Time.GetTimeString() });
		var clock = new MudSharp.Models.Clock { Id = 1, PrimaryTimezoneId = 1, Definition = "<Clock />" };
		context.Timezones.Add(new MudSharp.Models.Timezone { Id = 1, Clock = clock, Name = "UTC", Description = "Test" });
		context.Calendars.Add(new MudSharp.Models.Calendar { Id = 1, FeedClockId = clock.Id, Date = f.Now.Date.GetDateString(), Definition = "<Calendar />" });
		foreach (var (name, gender) in new[] { ("IsFemale", "female"), ("IsMale", "male") })
		{
			var predicate = new MudSharp.Models.FutureProg { FunctionName = name, Category = "Tests", Subcategory = "Address utilities",
				FunctionComment = "Synthetic character-compatible gender predicate", ReturnType = (long)ProgVariableTypes.Boolean,
				FunctionText = $"return @ch.gender == ToGender(\"{gender}\")", AcceptsAnyParameters = false };
			predicate.FutureProgsParameters.Add(new MudSharp.Models.FutureProgsParameter { ParameterIndex = 0, ParameterName = "ch",
				ParameterType = (long)(ProgVariableTypes.Character | ProgVariableTypes.Chargen) });
			context.FutureProgs.Add(predicate);
		}
		context.SaveChanges();
		var before = context.Clans.Select(x => x.Id).ToHashSet();
		var seeder = new ClanSeeder();
		typeof(ClanSeeder).GetMethod(setupMethod, BindingFlags.NonPublic | BindingFlags.Instance)!
			.Invoke(seeder, [context, new Dictionary<string, string>()]);
		typeof(ClanSeeder).GetMethod("SeedNewTemplateAddresses", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [context, before]);
		var progs = new All<IFutureProg>();
		f.World.SetupGet(x => x.FutureProgs).Returns(progs);
		foreach (var predicate in context.FutureProgs)
		{
			var shared = new MudSharp.FutureProg.FutureProg(predicate, f.World.Object);
			Assert.IsTrue(shared.Compile(), shared.CompileError); progs.Add(shared);
		}
		var clans = new All<IClan> { f.Clan };
		f.World.SetupGet(x => x.Clans).Returns(clans);
		f.World.Setup(x => x.Add(It.IsAny<IClan>())).Callback<IClan>(x => clans.Add(x));
		var row = context.Clans.Single(x => !before.Contains(x.Id));
		var source = new Clan(row, f.World.Object); clans.Add(source);
		source.FinaliseLoad(row, [], loadElections: false);
		return source;
	}

}
