#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Community;
using MudSharp.Community.Templates;
using MudSharp.Framework;
using MudSharp.FutureProg;
using Clan = MudSharp.Community.Clan;

namespace MudSharp_Unit_Tests.Community;

public partial class ClanElectionGovernanceTests
{
	[TestMethod]
	[TestCategory("Integration")]
	public void CatholicClone_StagesWithoutFounder_RebindsLocalAuthorityAndCompilesProgs()
	{
		using var setup = new CatholicFixture();
		var clone = setup.Clone("catholic.diocese", "local");
		var profile = ClanTemplateProfile.Read(setup.F.World.Object, clone.Id)!;
		Assert.IsTrue(clone.IsTemplate);
		Assert.IsEmpty(clone.Memberships);
		Assert.IsFalse(setup.F.Database.Context.ClanMemberships.Any());
		Assert.IsFalse(setup.F.Database.Context.Elections.Any());
		var dean = setup.Office(clone, "cathedral_dean");
		Assert.AreSame(setup.Office(clone, "ordinary"), dean.ElectionConfirmationAppointment);
		Assert.IsTrue(dean.CanParentDismiss);
		Assert.IsFalse(dean.CanDirectlyAppoint);
		Assert.IsFalse(dean.CanNominateProg.ExecuteBool(setup.F.Actor.Object));
		Assert.AreEqual(0, dean.NumberOfVotesProg.ExecuteInt(setup.F.Actor.Object));
		Assert.IsFalse(string.IsNullOrWhiteSpace(dean.WhyCantNominateProg.ExecuteString(setup.F.Actor.Object)));
		Assert.IsTrue(dean.CanNominateProg.FunctionText.Contains($", {clone.Id}, {dean.Id},", StringComparison.Ordinal));
		Assert.IsTrue(profile.GeneratedProgs.Values.All(id => setup.F.World.Object.FutureProgs.Get(id).Compile()));
		var errors = ClanTemplateProvisioning.Activate(setup.F.World.Object, clone, profile);
		Assert.IsNotEmpty(errors);
		Assert.IsTrue(clone.IsTemplate);
		Assert.IsFalse(setup.F.Database.Context.Elections.Any());
		var status = setup.F.World.Object.FutureProgs.Single(x => x.FunctionName == "CatholicClanStatus")
			.ExecuteString(setup.F.Actor.Object, clone);
		Assert.IsTrue(status.Contains("church", StringComparison.Ordinal));
		var exported = setup.F.World.Object.FutureProgs.Single(x => x.FunctionName == "CatholicClanExport")
			.ExecuteString(setup.F.Actor.Object, clone);
		Assert.AreEqual(profile.TemplateKey, System.Text.Json.JsonSerializer.Deserialize<ClanTemplateProfile>(exported)!.TemplateKey);
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void CatholicPapacy_OnlyLocalActiveCardinalsVote_BishopCardinalsStand()
	{
		using var setup = new CatholicFixture();
		var church = setup.Clone("catholic.church", "churcha");
		setup.Activate(church);
		var pope = setup.Office(church, "pope");
		var priest = setup.Member(church, "priest", "cardinal");
		Assert.AreEqual(1, pope.NumberOfVotesProg.ExecuteInt(priest.Object));
		Assert.IsFalse(pope.CanNominateProg.ExecuteBool(priest.Object));
		var bishop = setup.Member(church, "bishop", "cardinal");
		Assert.IsTrue(pope.CanNominateProg.ExecuteBool(bishop.Object));
		Assert.AreEqual(string.Empty, pope.WhyCantNominateProg.ExecuteString(bishop.Object));
		Assert.AreEqual(1, pope.NumberOfVotesProg.ExecuteInt(bishop.Object));
		var customRank = new Mock<IRank>();
		customRank.SetupGet(x => x.Id).Returns(99999);
		customRank.SetupGet(x => x.RankNumber).Returns(99);
		var bishopMembership = church.Memberships.Single(x => x.MemberId == bishop.Object.Id);
		bishopMembership.Rank = customRank.Object;
		Assert.IsFalse(pope.CanNominateProg.ExecuteBool(bishop.Object));
		bishopMembership.Rank = setup.Rank(church, "bishop");
		var lay = setup.Member(church, "lay_member");
		Assert.AreEqual(0, pope.NumberOfVotesProg.ExecuteInt(lay.Object));
		var second = setup.Clone("catholic.church", "churchb");
		setup.Activate(second);
		var foreign = setup.Member(second, "bishop", "cardinal");
		Assert.AreEqual(0, pope.NumberOfVotesProg.ExecuteInt(foreign.Object));
		var membership = church.Memberships.Single(x => x.MemberId == priest.Object.Id);
		membership.IsArchivedMembership = true;
		Assert.AreEqual(0, pope.NumberOfVotesProg.ExecuteInt(priest.Object));
		bishop.SetupGet(x => x.IsPrimaryInstance).Returns(false);
		Assert.AreEqual(0, pope.NumberOfVotesProg.ExecuteInt(bishop.Object));
		Assert.IsTrue(pope.Elections.Any());
		Assert.IsTrue(setup.Sources["catholic.church"].IsTemplate);
		Assert.IsEmpty(setup.Sources["catholic.church"].Appointments.SelectMany(x => x.Elections).ToList());
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void CatholicReligious_SeparateProfessionAndPriesthood_RecallUsesProfessedElectorate()
	{
		using var setup = new CatholicFixture();
		var church = setup.Clone("catholic.church", "church");
		setup.Activate(church);
		var order = setup.Clone("catholic.dominican.order", "order");
		setup.Bind(order, "church", church);
		setup.Variant(order, "religious_recall_game_variant");
		setup.Activate(order);
		var head = setup.Office(order, "superior");
		var religious = setup.Member(order, "professed");
		setup.Member(church, "lay_member", character: religious);
		Assert.AreEqual(1, head.NumberOfVotesProg.ExecuteInt(religious.Object));
		Assert.IsFalse(head.CanNominateProg.ExecuteBool(religious.Object));
		Assert.IsTrue(head.CanInitiateRecallProg.ExecuteBool(religious.Object));
		Assert.AreEqual(1, head.RecallVotesProg.ExecuteInt(religious.Object));
		Assert.IsNotEmpty(ClanTemplateEligibility.Error(setup.F.World.Object, religious.Object, order.Id, head.Id, "preflight"));
		var local = church.Memberships.Single(x => x.MemberId == religious.Object.Id);
		local.Rank = setup.Rank(church, "priest");
		Assert.IsTrue(head.CanNominateProg.ExecuteBool(religious.Object));
		var novice = setup.Member(order, "novice");
		setup.Member(church, "priest", character: novice);
		Assert.AreEqual(0, head.NumberOfVotesProg.ExecuteInt(novice.Object));
		Assert.IsFalse(head.CanInitiateRecallProg.ExecuteBool(novice.Object));
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void CatholicBindings_RejectCrossChurchAndSelfAuthority_StaysStaged()
	{
		using var setup = new CatholicFixture();
		var first = setup.Clone("catholic.church", "first"); setup.Activate(first);
		var second = setup.Clone("catholic.church", "second"); setup.Activate(second);
		var diocese = setup.Clone("catholic.diocese", "diocese");
		setup.Bind(diocese, "church", second); setup.Activate(diocese);
		var parish = setup.Clone("catholic.parish", "parishcopy");
		setup.Bind(parish, "church", first); setup.Bind(parish, "diocese", diocese);
		var profile = ClanTemplateProfile.Read(setup.F.World.Object, parish.Id)!;
		var errors = ClanTemplateProvisioning.Activate(setup.F.World.Object, parish, profile);
		Assert.IsTrue(errors.Any(x => x.Contains("same church affiliation", StringComparison.Ordinal)));
		Assert.IsTrue(parish.IsTemplate);
		profile.Bindings["diocese"].ClanId = parish.Id;
		ClanTemplateProvisioning.SaveProfile(setup.F.World.Object, profile);
		Assert.IsNotEmpty(ClanTemplateProvisioning.Activate(setup.F.World.Object, parish, profile));
		Assert.IsFalse(parish.ExternalControls.Any());
		var custom = new Mock<IClan>();
		var customOffice = new Mock<IAppointment>();
		custom.SetupGet(x => x.Id).Returns(90000);
		custom.SetupGet(x => x.Name).Returns("Custom Authority");
		custom.SetupGet(x => x.FullName).Returns("Custom Authority");
		custom.SetupGet(x => x.Alias).Returns("customauthority");
		custom.SetupGet(x => x.Appointments).Returns([customOffice.Object]);
		custom.SetupGet(x => x.ExternalControls).Returns([]);
		customOffice.SetupGet(x => x.Id).Returns(90001);
		customOffice.SetupGet(x => x.Clan).Returns(custom.Object);
		((All<IClan>)setup.F.World.Object.Clans).Add(custom.Object);
		var abbey = setup.Clone("catholic.generic.abbey", "customabbey");
		setup.Bind(abbey, "church", first);
		setup.Bind(abbey, "canonical_superior", custom.Object, customOffice.Object);
		var abbeyProfile = ClanTemplateProfile.Read(setup.F.World.Object, abbey.Id)!;
		Assert.IsTrue(ClanTemplateEligibility.ValidateBindings(setup.F.World.Object, abbey, abbeyProfile, false)
			.Any(x => x.Contains("same church affiliation", StringComparison.Ordinal)));
		abbeyProfile.Bindings["canonical_superior"].AffiliationId = first.Id;
		Assert.IsEmpty(ClanTemplateEligibility.ValidateBindings(setup.F.World.Object, abbey, abbeyProfile, false).ToList());
		abbeyProfile.Bindings["canonical_superior"].AffiliationId = second.Id;
		Assert.IsNotEmpty(ClanTemplateEligibility.ValidateBindings(setup.F.World.Object, abbey, abbeyProfile, false).ToList());
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void CatholicChapterVariant_RemovesOnlyOrdinaryReservation_AndPreservesConfirmation()
	{
		using var setup = new CatholicFixture();
		var church = setup.Clone("catholic.church", "church"); setup.Activate(church);
		var diocese = setup.Clone("catholic.diocese", "diocese");
		setup.Bind(diocese, "church", church);
		setup.Variant(diocese, "diocesan_chapter_election");
		var stagedProfile = ClanTemplateProfile.Read(setup.F.World.Object, diocese.Id)!;
		stagedProfile.Bindings["church"].AppointmentId = setup.Office(church, "cardinal").Id;
		Assert.IsNotEmpty(ClanTemplateProvisioning.Activate(setup.F.World.Object, diocese, stagedProfile));
		Assert.IsTrue(diocese.IsTemplate);
		setup.Activate(diocese);
		var ordinary = setup.Office(diocese, "ordinary");
		Assert.IsTrue(ordinary.IsAppointedByElection);
		Assert.AreSame(setup.Office(church, "pope"), ordinary.ElectionConfirmationAppointment);
		Assert.IsTrue(ordinary.CanDirectlyAppoint);
		Assert.IsFalse(diocese.ExternalControls.Any(x => x.ControlledAppointment == ordinary));
		Assert.IsTrue(diocese.ExternalControls.Any(x => x.ControlledAppointment == setup.Office(diocese, "auxiliary_bishop")));
		Assert.AreEqual(setup.Office(church, "pope").Id,
			ClanTemplateProfile.Read(setup.F.World.Object, diocese.Id)!.Bindings["church"].AppointmentId);
		var bishop = setup.Member(diocese, "bishop", "cathedral_canon");
		setup.Member(church, "bishop", character: bishop);
		Assert.IsTrue(ordinary.CanNominateProg.ExecuteBool(bishop.Object));
		Assert.AreEqual(1, ordinary.NumberOfVotesProg.ExecuteInt(bishop.Object));
		var count = ordinary.Elections.Count();
		var profile = ClanTemplateProfile.Read(setup.F.World.Object, diocese.Id)!;
		Assert.IsNotEmpty(ClanTemplateProvisioning.Activate(setup.F.World.Object, diocese, profile));
		Assert.AreEqual(count, ordinary.Elections.Count());
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void CatholicFemaleVariant_PreservesNonOrdainedStanding_AndExactVacantConfirmer()
	{
		using var setup = new CatholicFixture();
		var church = setup.Clone("catholic.church", "church"); setup.Activate(church);
		var abbey = setup.Clone("catholic.benedictine.abbey", "abbey");
		setup.Bind(abbey, "church", church);
		setup.Bind(abbey, "canonical_superior", church, setup.Office(church, "pope"));
		setup.Variant(abbey, "female_monastic_house");
		setup.Activate(abbey);
		var head = setup.Office(abbey, "superior");
		Assert.AreSame(setup.Office(church, "pope"), head.ElectionConfirmationAppointment);
		Assert.IsFalse(head.CanDirectlyAppoint);
		var nun = setup.Member(abbey, "professed");
		setup.Member(church, "lay_member", character: nun);
		Assert.IsTrue(head.CanNominateProg.ExecuteBool(nun.Object));
		Assert.AreEqual(string.Empty, ClanTemplateEligibility.Error(setup.F.World.Object, nun.Object, abbey.Id, head.Id, "preflight"));
		Assert.AreSame(setup.Rank(church, "lay_member"), church.Memberships.Single(x => x.MemberId == nun.Object.Id).Rank);
		var bound = ClanTemplateProfile.Read(setup.F.World.Object, abbey.Id)!;
		Assert.IsEmpty(ClanTemplateEligibility.ValidateBindings(setup.F.World.Object, abbey, bound).ToList());
		var pope = setup.Office(church, "pope");
		pope.ElectionConfirmationAppointment = head;
		Assert.IsTrue(ClanTemplateEligibility.ValidateBindings(setup.F.World.Object, abbey, bound)
			.Any(x => x.Contains("Cyclic", StringComparison.Ordinal)));
		Assert.IsFalse(head.CanNominateProg.ExecuteBool(nun.Object));
		pope.ElectionConfirmationAppointment = null;
		church.IsTemplate = true;
		Assert.AreEqual(0, head.NumberOfVotesProg.ExecuteInt(nun.Object));
		Assert.IsFalse(head.CanNominateProg.ExecuteBool(nun.Object));
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void CatholicManualVariant_ClearsElectionFields_AndMinorOrdersUseRuntimeOrdinals()
	{
		using var setup = new CatholicFixture();
		var church = setup.Clone("catholic.church", "church");
		setup.Variant(church, "historical_minor_orders");
		setup.Variant(church, "papacy_manual_conclave");
		setup.Activate(church);
		var pope = setup.Office(church, "pope");
		Assert.IsFalse(pope.IsAppointedByElection);
		Assert.IsNull(pope.CanNominateProg);
		var row = setup.F.Database.Context.Appointments.Find(pope.Id)!;
		Assert.IsNull(row.ElectionTermMinutes);
		Assert.IsNull(row.NumberOfVotesProgId);
		Assert.IsFalse(setup.F.Database.Context.Elections.Any());
		var cardinal = setup.Office(church, "cardinal");
		Assert.AreSame(setup.Rank(church, "minor_cleric"), cardinal.MinimumRankToHold);
		Assert.IsTrue(setup.Rank(church, "minor_cleric").RankNumber < setup.Rank(church, "deacon").RankNumber);
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void CatholicSource_RenamedTemplateStillStages_AndMissingOfficeRejectsBeforeWrites()
	{
		using var setup = new CatholicFixture();
		var source = setup.Sources["catholic.church"];
		source.Alias = "renamed";
		source.FullName = "Renamed Church";
		var beforeOrdinaryCopy = setup.F.Database.Context.Clans.Select(x => x.Id).ToList();
		typeof(ClanModule).GetMethod("ClanCreateTemplate", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [setup.F.Actor.Object, new StringStack("renamed ordinary Ordinary Copy")]);
		CollectionAssert.AreEquivalent(beforeOrdinaryCopy, setup.F.Database.Context.Clans.Select(x => x.Id).ToList());
		var clone = setup.Clone("catholic.church", "copy");
		Assert.IsTrue(clone.IsTemplate);
		Assert.IsEmpty(clone.Memberships);
		Assert.IsNotNull(ClanTemplateProfile.Read(setup.F.World.Object, clone.Id));
		var ids = setup.F.Database.Context.Clans.Select(x => x.Id).ToList();
		source.Appointments.RemoveAt(0);
		typeof(ClanModule).GetMethod("ClanCreateStagedTemplate", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [setup.F.Actor.Object, new StringStack("renamed incomplete Incomplete Copy")]);
		CollectionAssert.AreEquivalent(ids, setup.F.Database.Context.Clans.Select(x => x.Id).ToList());
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void CatholicActivation_SaveFailureLeavesNoTrackedReservations_AndRetrySucceeds()
	{
		using var setup = new CatholicFixture();
		var church = setup.Clone("catholic.church", "church"); setup.Activate(church);
		var diocese = setup.Clone("catholic.diocese", "diocese");
		setup.Bind(diocese, "church", church);
		var profile = ClanTemplateProfile.Read(setup.F.World.Object, diocese.Id)!;
		void Fail(object? sender, SavingChangesEventArgs args)
		{
			if (setup.F.Database.Context.ChangeTracker.Entries<MudSharp.Models.ExternalClanControl>().Any(x => x.State == EntityState.Added))
				throw new InvalidOperationException("Injected activation write failure");
		}
		setup.F.Database.Context.SavingChanges += Fail;
		Assert.ThrowsExactly<InvalidOperationException>(() => ClanTemplateProvisioning.Activate(setup.F.World.Object, diocese, profile));
		setup.F.Database.Context.SavingChanges -= Fail;
		setup.F.Database.Context.SaveChanges();
		Assert.IsTrue(diocese.IsTemplate);
		Assert.IsTrue(setup.F.Database.Context.Clans.Find(diocese.Id)!.IsTemplate);
		Assert.IsFalse(ClanTemplateProfile.Read(setup.F.World.Object, diocese.Id)!.Activated);
		Assert.IsFalse(setup.F.Database.Context.ExternalClanControls.Any());
		Assert.IsEmpty(diocese.ExternalControls);
		setup.Activate(diocese);
		Assert.IsFalse(diocese.IsTemplate);
		Assert.IsTrue(diocese.ExternalControls.Any());
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void CatholicStockSource_CannotActivateOrApplyCopyVariants()
	{
		using var setup = new CatholicFixture();
		var source = setup.Sources["catholic.church"];
		var profile = ClanTemplateProfile.Read(setup.F.World.Object, source.Id)!;
		Assert.IsNotEmpty(ClanTemplateProvisioning.Activate(setup.F.World.Object, source, profile));
		Assert.ThrowsExactly<InvalidOperationException>(() => ClanTemplateProvisioning.ApplyVariant(setup.F.World.Object, source, profile, "papacy_manual_conclave"));
		Assert.IsTrue(source.IsTemplate);
		Assert.IsFalse(setup.F.Database.Context.Elections.Any());
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void CatholicProgSuite_NonAdministratorCannotProvisionCopy()
	{
		using var setup = new CatholicFixture();
		var copy = setup.Clone("catholic.church", "copy");
		setup.F.Actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(false);
		var result = setup.F.World.Object.FutureProgs.Single(x => x.FunctionName == "CatholicClanActivate")
			.ExecuteString(setup.F.Actor.Object, copy);
		Assert.AreEqual("Only administrators may provision template copies.", result);
		Assert.IsTrue(copy.IsTemplate);
		Assert.IsFalse(ClanTemplateProfile.Read(setup.F.World.Object, copy.Id)!.Activated);
		Assert.IsFalse(setup.F.Database.Context.Elections.Any());
	}

	private sealed class CatholicFixture : IDisposable
	{
		public Fixture F { get; } = new();
		public Dictionary<string, Clan> Sources { get; } = [];
		private long _characterId = 1000;
		public CatholicFixture()
		{
			MudSharp.FutureProg.FutureProg.Initialise();
			F.Actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(true);
			F.Actor.SetupGet(x => x.GetObject).Returns(F.Actor.Object);
			F.Actor.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
			F.Database.Context.Clans.Add(new MudSharp.Models.Clan { Id = 1, Name = "Fixture", Alias = "fixture",
				PayIntervalReferenceDate = F.Now.Date.GetDateString(), PayIntervalReferenceTime = F.Now.Time.GetTimeString() });
			var clock = new MudSharp.Models.Clock { Id = 1, Definition = "<Clock />", PrimaryTimezoneId = 1 };
			F.Database.Context.Clocks.Add(clock);
			F.Database.Context.Timezones.Add(new MudSharp.Models.Timezone { Id = 1, Name = "UTC", Description = "Test", Clock = clock });
			F.Database.Context.Calendars.Add(new MudSharp.Models.Calendar { Id = 1, Date = F.Now.Date.GetDateString(), Definition = "<Calendar />", FeedClockId = 1 });
			F.Database.Context.SaveChanges();
			typeof(ClanSeeder).GetMethod("SetupCatholicTemplates", BindingFlags.NonPublic | BindingFlags.Instance)!
				.Invoke(new ClanSeeder(), [F.Database.Context]);
			var progs = new All<IFutureProg>();
			F.World.SetupGet(x => x.FutureProgs).Returns(progs);
			F.World.Setup(x => x.Add(It.IsAny<IFutureProg>())).Callback<IFutureProg>(x => progs.Add(x));
			foreach (var prog in F.Database.Context.FutureProgs)
			{
				var runtime = new MudSharp.FutureProg.FutureProg(prog, F.World.Object);
				Assert.IsTrue(runtime.Compile(), runtime.CompileError);
				progs.Add(runtime);
			}
			var clans = new All<IClan>();
			F.World.SetupGet(x => x.Clans).Returns(clans);
			F.World.Setup(x => x.Add(It.IsAny<IClan>())).Callback<IClan>(x => clans.Add(x));
			foreach (var key in CatholicTemplateCatalogue.Keys)
			{
				var definition = CatholicTemplateCatalogue.Get(key);
				var row = F.Database.Context.Clans.Single(x => x.Name == definition.Name);
				var runtime = new Clan(row, F.World.Object);
				Sources[key] = runtime;
				clans.Add(runtime);
			}
			foreach (var clan in Sources.Values)
				clan.FinaliseLoad(F.Database.Context.Clans.Find(clan.Id)!, [], loadElections: false);
		}
		public Clan Clone(string key, string alias)
		{
			typeof(ClanModule).GetMethod("ClanCreateStagedTemplate", BindingFlags.NonPublic | BindingFlags.Static)!
				.Invoke(null, [F.Actor.Object, new StringStack($"{Sources[key].Alias} {alias} {alias} copy")]);
			return (Clan)F.World.Object.Clans.Single(x => x.Alias == alias);
		}
		public IAppointment Office(IClan clan, string key) => clan.Appointments.Single(x => x.Id == ClanTemplateProfile.Read(F.World.Object, clan.Id)!.Appointments[key]);
		public IRank Rank(IClan clan, string key) => clan.Ranks.Single(x => x.Id == ClanTemplateProfile.Read(F.World.Object, clan.Id)!.Ranks[key]);
		public void Bind(IClan clan, string role, IClan authority, IAppointment? office = null)
		{
			var arguments = $"{role} {authority.Id}" + (office is null ? "" : $" {office.Id}");
			Assert.AreEqual("Binding recorded. Review status before activation.",
				F.World.Object.FutureProgs.Single(x => x.FunctionName == "CatholicClanBind").ExecuteString(F.Actor.Object, clan, arguments));
		}
		public void Variant(IClan clan, string variant) => Assert.AreEqual("Variant applied to the staged copy; progs regenerated.",
			F.World.Object.FutureProgs.Single(x => x.FunctionName == "CatholicClanVariant").ExecuteString(F.Actor.Object, clan, variant));
		public void Activate(IClan clan) => Assert.AreEqual("Template copy activated. Native elections now run.",
			F.World.Object.FutureProgs.Single(x => x.FunctionName == "CatholicClanActivate").ExecuteString(F.Actor.Object, clan));
		public Mock<ICharacter> Member(IClan clan, string rank, string? office = null, Mock<ICharacter>? character = null)
		{
			if (character is null)
			{
				character = new Mock<ICharacter>();
				character.SetupGet(x => x.Id).Returns(_characterId++);
				character.SetupGet(x => x.IsPrimaryInstance).Returns(true);
				character.SetupGet(x => x.Gameworld).Returns(F.World.Object);
				character.SetupGet(x => x.GetObject).Returns(character.Object);
				character.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
			}
			var membership = new Mock<IClanMembership>();
			membership.SetupProperty(x => x.IsArchivedMembership);
			membership.SetupProperty(x => x.Rank, Rank(clan, rank));
			membership.SetupGet(x => x.MemberId).Returns(character.Object.Id);
			membership.SetupGet(x => x.Clan).Returns(clan);
			membership.SetupGet(x => x.Appointments).Returns(office is null ? [] : [Office(clan, office)]);
			clan.Memberships.Add(membership.Object);
			return character;
		}
		public void Dispose() => F.Dispose();
	}
}
