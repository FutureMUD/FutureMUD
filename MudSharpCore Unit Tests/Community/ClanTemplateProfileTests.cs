#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
