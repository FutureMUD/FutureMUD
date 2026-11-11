#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Community;
using MudSharp.Community.Templates;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate.Intervals;
using Clan = MudSharp.Community.Clan;

namespace MudSharp_Unit_Tests.Community;

public partial class ClanElectionGovernanceTests
{
	[TestMethod]
	[DataRow("missing")]
	[DataRow("return-type")]
	[DataRow("parameters")]
	[DataRow("any-parameters")]
	public void TemplateVariant_InvalidAddressDependency_PreservesStateAndAllowsRetry(string failure)
	{
		using var f = new Fixture();
		var profile = SetUpAddressDependencyProfile(f, dependencyInVariant: true);
		var original = profile.ToComment();
		var predicate = SharedAddressPredicate(failure);
		if (failure != "missing") f.World.Object.Add(predicate.Object);
		var error = Assert.ThrowsExactly<InvalidOperationException>(() =>
			ClanTemplateProvisioning.ApplyVariant(f.World.Object, f.Clan, profile, "courtesy"));
		StringAssert.Contains(error.Message, failure == "missing" ? "Missing shared address predicate" : "Invalid shared address predicate");
		Assert.AreEqual(original, profile.ToComment());
		Assert.AreEqual(original, ClanTemplateProfile.Read(f.World.Object, f.Clan.Id)!.ToComment());
		Assert.AreEqual(original, f.Database.Context.FutureProgs.Single().FunctionComment);
		Assert.AreEqual("Member", f.Clan.Ranks.Single().TitlesAndProgs.Single().Item2);
		Assert.AreEqual("Member", f.Database.Context.Ranks.Single().RanksTitles.Single().Title);
		Assert.AreEqual(1, f.Office.MaximumSimultaneousHolders);
		Assert.AreEqual(1, f.Database.Context.Appointments.Single().MaximumSimultaneousHolders);
		Assert.IsEmpty(f.Office.FormsOfAddress);
		Assert.IsEmpty(f.Database.Context.AppointmentFormsOfAddress);
		if (failure == "missing") f.World.Object.Add(predicate.Object);
		predicate.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		predicate.SetupGet(x => x.AcceptsAnyParameters).Returns(false);
		predicate.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		ClanTemplateProvisioning.ApplyVariant(f.World.Object, f.Clan, profile, "courtesy");
		Assert.Contains("courtesy", profile.Variants);
		Assert.AreEqual(3, f.Office.MaximumSimultaneousHolders);
		Assert.AreEqual(predicate.Object.Id, f.Office.FormsOfAddress.Single().FutureProgId);
	}

	[TestMethod]
	[DataRow("missing")]
	[DataRow("return-type")]
	[DataRow("parameters")]
	[DataRow("any-parameters")]
	public void StagedClone_InvalidAddressDependency_RejectsBeforeCreatingRows(string failure)
	{
		using var f = new Fixture();
		var profile = SetUpAddressDependencyProfile(f, dependencyInVariant: false);
		if (failure != "missing") f.World.Object.Add(SharedAddressPredicate(failure).Object);
		var ids = f.World.Object.Clans.Select(x => x.Id).ToList();
		var metadata = profile.ToComment();
		typeof(ClanModule).GetMethod("ClanCreateStagedTemplate", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [f.Actor.Object, new StringStack("parish invalidaddress Invalid Address Copy")]);
		Assert.IsEmpty(f.Database.Context.Clans);
		CollectionAssert.AreEquivalent(ids, f.World.Object.Clans.Select(x => x.Id).ToList());
		Assert.HasCount(1, f.Database.Context.Ranks.ToList());
		Assert.HasCount(1, f.Database.Context.Appointments.ToList());
		Assert.HasCount(1, f.Database.Context.FutureProgs.ToList());
		Assert.AreEqual(metadata, ClanTemplateProfile.Read(f.World.Object, f.Clan.Id)!.ToComment());
	}

	private static Mock<IFutureProg> SharedAddressPredicate(string failure)
	{
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.Id).Returns(900);
		prog.SetupGet(x => x.FunctionName).Returns("SharedCourtesy");
		prog.SetupGet(x => x.Name).Returns("SharedCourtesy");
		prog.SetupGet(x => x.ReturnType).Returns(failure == "return-type" ? ProgVariableTypes.Text : ProgVariableTypes.Boolean);
		prog.SetupGet(x => x.AcceptsAnyParameters).Returns(failure == "any-parameters");
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(failure != "parameters");
		return prog;
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void TemplateAddressDependency_ClearedOrReplacedRule_DoesNotRequireObsoleteProg(bool replacement)
	{
		using var f = new Fixture();
		var profile = SetUpAddressDependencyProfile(f, dependencyInVariant: false);
		var rank = f.Clan.Ranks.Single();
		rank.FormsOfAddress.Clear();
		if (replacement) rank.FormsOfAddress.Add(new ClanAddressRule { Text = "Custom courtesy" });
		rank.Save();
		Assert.IsNull(ClanTemplateProvisioning.AddressCloneError(f.World.Object, f.Clan, profile.Definition));
		profile.Definition.AvailableVariants = [new() { Key = "retitle",
			RankOverrides = new() { ["member"] = new JsonObject { ["Title"] = "Fellow" } } }];
		ClanTemplateProvisioning.ApplyVariant(f.World.Object, f.Clan, profile, "retitle");
		Assert.AreEqual(replacement ? "Custom courtesy" : null, rank.FormsOfAddress.SingleOrDefault()?.Text);
		Assert.AreEqual(replacement ? "Custom courtesy" : null,
			f.Database.Context.RankFormsOfAddress.SingleOrDefault()?.Text);
	}

	private static ClanTemplateProfile SetUpAddressDependencyProfile(Fixture f, bool dependencyInVariant)
	{
		MudSharp.FutureProg.FutureProg.Initialise();
		f.Actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(true);
		f.Clan.IsTemplate = true;
		f.Office.ElectionConfirmationAppointment = null;
		f.World.SetupGet(x => x.Clans).Returns(new All<IClan> { f.Clan });
		var row = new MudSharp.Models.Rank { Id = 1, ClanId = f.Clan.Id, Name = "Member", RankPath = "General" };
		row.RanksTitles.Add(new MudSharp.Models.RanksTitle { Title = "Member", Order = 0 });
		f.Database.Context.Ranks.Add(row);
		f.Clan.Ranks.Add(new Rank(row, f.Clan));
		f.Database.Context.SaveChanges();
		var progs = new All<IFutureProg>();
		f.World.SetupGet(x => x.FutureProgs).Returns(progs);
		f.World.Setup(x => x.Add(It.IsAny<IFutureProg>())).Callback<IFutureProg>(x => progs.Add(x));
		var rule = new ClanTemplateAddressRule { Key = "courtesy", Text = "Friend", ProgName = "SharedCourtesy" };
		var profile = new ClanTemplateProfile
		{
			ClanId = f.Clan.Id, TemplateKey = "example.club", Ranks = new() { ["member"] = 1 },
			Appointments = new() { ["head"] = f.Office.Id }, Definition = new()
			{
				Key = "example.club", Ranks = [new() { Key = "member", Name = "Member", Title = "Member", RankPath = "General" }],
				Appointments = [new() { Key = "head", Name = "Priest", MinimumRankKey = "member", MaximumSimultaneousHolders = 1,
					IsAppointedByElection = true, NominationPeriodMinutes = 1440, VotingPeriodMinutes = 1440 }],
				AvailableVariants = [new() { Key = "courtesy",
					RankOverrides = new() { ["member"] = new JsonObject { ["Title"] = "Fellow" } },
					AppointmentOverrides = new() { ["head"] = new JsonObject { ["MaximumSimultaneousHolders"] = 3,
						["FormsOfAddress"] = JsonSerializer.SerializeToNode(new[] { rule }) } } }]
			}
		};
		if (!dependencyInVariant)
		{
			profile.Definition.Ranks.Single().FormsOfAddress.Add(rule);
			var rank = f.Clan.Ranks.Single();
			rank.FormsOfAddress.Add(new ClanAddressRule { Text = rule.Text, FutureProgId = 900,
				TemplateKey = ClanTemplateProvisioning.AddressKey("rank", "member", rule.Key) });
			rank.Save();
		}
		ClanTemplateProvisioning.SaveProfile(f.World.Object, profile);
		return profile;
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void StagedClone_UnprofiledTemplate_OmitsFounderOnlyForAdministrators(bool administrator)
	{
		using var f = new Fixture();
		f.Clan.IsTemplate = true;
		f.Clan.Description = "An ordinary template";
		f.Clan.PayInterval = new RecurringInterval { Type = IntervalType.Daily, IntervalAmount = 7 };
		f.Office.ElectionConfirmationAppointment = null;
		f.Database.Context.Clans.Add(new MudSharp.Models.Clan
		{
			Id = f.Clan.Id, Name = "Parish", Alias = "parish", CalendarId = 1,
			PayIntervalReferenceDate = f.Now.Date.GetDateString(), PayIntervalReferenceTime = f.Now.Time.GetTimeString()
		});
		var rankRow = new MudSharp.Models.Rank { Id = 1, ClanId = f.Clan.Id, Name = "Member", RankPath = "General" };
		f.Database.Context.Ranks.Add(rankRow);
		f.Clan.Ranks.Add(new Rank(rankRow, f.Clan));
		f.Database.Context.SaveChanges();
		f.Actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(administrator);
		var clans = new All<IClan> { f.Clan };
		f.World.SetupGet(x => x.Clans).Returns(clans);
		f.World.Setup(x => x.Add(It.IsAny<IClan>())).Callback<IClan>(x => clans.Add(x));
		typeof(ClanModule).GetMethod("ClanCreateStagedTemplate", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [f.Actor.Object, new StringStack("parish stagedcopy Staged Copy")]);
		var clone = clans.SingleOrDefault(x => x.Alias == "stagedcopy");
		if (!administrator)
		{
			Assert.IsNull(clone);
			Assert.AreEqual(1, f.Database.Context.Clans.Count());
			return;
		}
		Assert.IsNotNull(clone);
		Assert.IsTrue(clone.IsTemplate);
		Assert.IsEmpty(clone.Memberships);
		Assert.IsFalse(f.Database.Context.ClanMemberships.Any());
		Assert.IsFalse(f.Database.Context.Elections.Any());
		Assert.IsNull(ClanTemplateProfile.Read(f.World.Object, clone.Id));
	}

	[TestMethod]
	public void TemplateEligibility_ArbitraryClubConditions_WorkWithoutCatalogueOrSeeder()
	{
		MudSharp.FutureProg.FutureProg.Initialise();
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var clan = new Mock<IClan>();
		clan.SetupGet(x => x.Id).Returns(7);
		var junior = new Mock<IRank>(); junior.SetupGet(x => x.Id).Returns(10); junior.SetupGet(x => x.RankNumber).Returns(0);
		var expert = new Mock<IRank>(); expert.SetupGet(x => x.Id).Returns(11); expert.SetupGet(x => x.RankNumber).Returns(1);
		var office = new Mock<IAppointment>(); office.SetupGet(x => x.Id).Returns(8); office.SetupGet(x => x.Clan).Returns(clan.Object);
		clan.SetupGet(x => x.Ranks).Returns([junior.Object, expert.Object]);
		clan.SetupGet(x => x.Appointments).Returns([office.Object]);
		clan.SetupGet(x => x.ExternalControls).Returns([]);
		var member = new Mock<IClanMembership>();
		member.SetupGet(x => x.MemberId).Returns(42);
		member.SetupProperty(x => x.Rank, junior.Object);
		member.SetupGet(x => x.Appointments).Returns([]);
		clan.SetupGet(x => x.Memberships).Returns([member.Object]);
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(42);
		character.SetupGet(x => x.IsPrimaryInstance).Returns(true);
		character.SetupGet(x => x.GetObject).Returns(character.Object);
		character.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
		var profile = new ClanTemplateProfile
		{
			ClanId = 7, TemplateKey = "example.club", Activated = true,
			Ranks = new() { ["junior"] = 10, ["expert"] = 11 }, Appointments = new() { ["chair"] = 8 },
			Definition = new ClanTemplateDefinition
			{
				Key = "example.club",
				Ranks = [new() { Key = "junior", Name = "Junior" }, new() { Key = "expert", Name = "Expert" }],
				BaseConditions = [new() { Kind = "membership" }],
				Appointments = [new()
				{
					Key = "chair", Name = "Chair", MinimumRankKey = "junior", MaximumSimultaneousHolders = 1,
					CandidateConditions = [new() { Kind = "exactrank", Key = "expert", Error = "An expert must chair this club." }],
					VoterConditions = [new() { Kind = "exactrank", Key = "junior", Error = "This club gives its junior members the vote." }]
				}]
			}
		};
		ClanTemplateDefinitions.Validate(profile.Definition);
		var metadata = Mock.Of<IFutureProg>(x => x.Id == 99 && x.FunctionName == ClanTemplateProfile.ProgName(7) && x.FunctionComment == profile.ToComment());
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg> { metadata });
		world.SetupGet(x => x.Clans).Returns(new All<IClan> { clan.Object });
		var row = new MudSharp.Models.FutureProg
		{
			FunctionName = "ClubCandidate", FunctionText = "return ClanTemplateEligibilityError(@ch, 7, 8, \"nominate\") == \"\"",
			ReturnType = (long)ProgVariableTypes.Boolean
		};
		row.FutureProgsParameters.Add(new MudSharp.Models.FutureProgsParameter
		{ ParameterName = "ch", ParameterIndex = 0, ParameterType = (long)ProgVariableTypes.Character });
		var prog = new MudSharp.FutureProg.FutureProg(row, world.Object);
		Assert.IsTrue(prog.Compile(), prog.CompileError);
		Assert.IsFalse(prog.ExecuteBool(character.Object));
		Assert.AreEqual(string.Empty, ClanTemplateEligibility.Error(world.Object, character.Object, 7, 8, "votes"));
		member.Object.Rank = expert.Object;
		Assert.IsTrue(prog.ExecuteBool(character.Object));
		Assert.AreEqual("This club gives its junior members the vote.", ClanTemplateEligibility.Error(world.Object, character.Object, 7, 8, "votes"));
	}

	[TestMethod]
	public void TemplateVariant_ChangesExistingElectionAndRank_PersistsAllDeclaredFields()
	{
		using var f = new Fixture();
		MudSharp.FutureProg.FutureProg.Initialise();
		f.Clan.IsTemplate = true;
		f.Office.ElectionConfirmationAppointment = null;
		var rankRow = new MudSharp.Models.Rank { Id = 1, ClanId = f.Clan.Id, Name = "Member", RankPath = "General" };
		f.Database.Context.Ranks.Add(rankRow);
		f.Clan.Ranks.Add(new Rank(rankRow, f.Clan));
		f.Database.Context.SaveChanges();
		var progs = new All<IFutureProg>();
		f.World.SetupGet(x => x.FutureProgs).Returns(progs);
		f.World.Setup(x => x.Add(It.IsAny<IFutureProg>())).Callback<IFutureProg>(x => progs.Add(x));
		var profile = new ClanTemplateProfile
		{
			ClanId = f.Clan.Id, TemplateKey = "example.club",
			Ranks = new() { ["member"] = 1 }, Appointments = new() { ["head"] = f.Office.Id },
			Definition = new ClanTemplateDefinition
			{
				Key = "example.club", Ranks = [new() { Key = "member", Name = "Member", RankPath = "General", Title = "Member" }],
				Appointments = [new()
				{
					Key = "head", Name = "Priest", MinimumRankKey = "member", MaximumSimultaneousHolders = 1,
					IsAppointedByElection = true, NominationPeriodMinutes = 1440, VotingPeriodMinutes = 1440, ElectionTermMinutes = 0
				}],
				AvailableVariants = [new()
				{
					Key = "short-term",
					AppointmentOverrides = new() { ["head"] = new JsonObject
					{
						["ElectionTermMinutes"] = 42, ["MaximumSimultaneousHolders"] = 3, ["CanParentDismiss"] = true,
						["PrivilegeNames"] = new JsonArray("CanViewMembers")
					} },
					RankOverrides = new() { ["member"] = new JsonObject { ["RankPath"] = "Qualified", ["Title"] = "Fellow" } }
				}]
			}
		};
		ClanTemplateProvisioning.SaveProfile(f.World.Object, profile);
		ClanTemplateProvisioning.ApplyVariant(f.World.Object, f.Clan, profile, "short-term");
		Assert.IsTrue(f.Office.IsAppointedByElection);
		Assert.AreEqual(TimeSpan.FromMinutes(42), f.Office.ElectionTerm);
		Assert.AreEqual(3, f.Office.MaximumSimultaneousHolders);
		Assert.IsTrue(f.Office.CanParentDismiss);
		Assert.AreEqual(ClanPrivilegeType.CanViewMembers, f.Office.Privileges);
		Assert.AreEqual("Qualified", f.Clan.Ranks.Single().RankPath);
		var persisted = f.Database.Context.Appointments.Find(f.Office.Id)!;
		Assert.AreEqual(42.0, persisted.ElectionTermMinutes);
		Assert.AreEqual(3, persisted.MaximumSimultaneousHolders);
		Assert.IsTrue(persisted.CanParentDismiss);
		var exported = ClanTemplateProfile.Read(f.World.Object, f.Clan.Id)!;
		Assert.AreEqual(42.0, exported.Definition.Appointments.Single().ElectionTermMinutes);
		Assert.AreEqual("Qualified", exported.Definition.Ranks.Single().RankPath);
	}
}
