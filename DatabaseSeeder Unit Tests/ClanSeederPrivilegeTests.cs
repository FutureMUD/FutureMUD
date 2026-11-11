#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Community;
using MudSharp.Community.Templates;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using AppointmentModel = MudSharp.Models.Appointment;
using Calendar = MudSharp.Models.Calendar;
using ClanModel = MudSharp.Models.Clan;
using RankModel = MudSharp.Models.Rank;

namespace MudSharp_Unit_Tests;

[TestClass]
public partial class ClanSeederPrivilegeTests
{

    [TestMethod]
    public void SeedData_PropertyCapableTemplates_ShouldAlsoGrantUseClanProperty()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedPrerequisites(context);

        string result = new ClanSeeder().SeedData(context, new System.Collections.Generic.Dictionary<string, string>());

        Assert.IsTrue(context.Clans.Any(), "First installation must create templates.");

        List<MudSharp.Models.Rank> propertyRanks = context.Ranks
            .Where(x => HasAnyPropertyPrivilege((ClanPrivilegeType)x.Privileges))
            .ToList();
        List<MudSharp.Models.Appointment> propertyAppointments = context.Appointments
            .Where(x => HasAnyPropertyPrivilege((ClanPrivilegeType)x.Privileges))
            .ToList();

        Assert.IsTrue(propertyRanks.Any(), "Expected the clan seeder to create at least one property-capable rank.");
        Assert.IsTrue(propertyAppointments.Any(),
            "Expected the clan seeder to create at least one property-capable appointment.");
        Assert.IsTrue(propertyRanks.All(x => ((ClanPrivilegeType)x.Privileges).HasFlag(ClanPrivilegeType.UseClanProperty)),
            "All property-capable ranks should also grant Use Clan Property.");
        Assert.IsTrue(
            propertyAppointments.All(x => ((ClanPrivilegeType)x.Privileges).HasFlag(ClanPrivilegeType.UseClanProperty)),
            "All property-capable appointments should also grant Use Clan Property.");
    }

	[TestMethod]
	public void SeedData_SecondRun_ShouldNotDuplicateExpandedTemplateCatalogue()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedPrerequisites(context);
		ClanSeeder seeder = new();

		string firstResult = seeder.SeedData(context, new Dictionary<string, string>());

		var installedIds = context.Clans.Select(x => x.Id).ToList();
		Assert.IsTrue(installedIds.Count > 0, "First installation must create templates.");
		var customised = context.Appointments.First();
		customised.CanResign = false;
		customised.MaximumSimultaneousHolders = 17;
		context.SaveChanges();
		var secondResult = seeder.SeedData(context, new Dictionary<string, string>());
		Assert.AreEqual("Created 0 new clans.", secondResult);
		CollectionAssert.AreEquivalent(installedIds, context.Clans.Select(x => x.Id).ToList());
		Assert.IsFalse(context.Appointments.Find(customised.Id)!.CanResign);
		Assert.AreEqual(17, context.Appointments.Find(customised.Id)!.MaximumSimultaneousHolders);
		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, seeder.ShouldSeedData(context));
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void CatholicInstall_MissingTemplateAndLegacyClan_PreservesExistingContent()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var legacy = new ClanModel { Name = "The Catholic Church", Alias = "legacychurch", FullName = "Builder Church", Description = "Keep me", PayIntervalReferenceDate = "1-1-1", PayIntervalReferenceTime = "UTC 0:0:0" };
		context.Clans.Add(legacy);
		context.SaveChanges();
		var seeder = new ClanSeeder();
		seeder.SeedData(context, new Dictionary<string, string>());
		var definition = CatholicTemplateCatalogue.Get(CatholicTemplateCatalogue.Keys.First());
		var template = context.Clans.Single(x => x.Name == definition.Name);
		Assert.IsTrue(context.Appointments.Any(x => x.ClanId == template.Id));
		var office = context.Appointments.First(x => x.ClanId == template.Id);
		office.MaximumSimultaneousHolders = 23;
		office.CanDirectlyAppoint = true;
		context.SaveChanges();
		var untouched = context.Clans.Where(x => x.Id != template.Id).Select(x => x.Id).ToList();
		var missing = context.Clans.Single(x => x.Name == CatholicTemplateCatalogue.Get(CatholicTemplateCatalogue.Keys.Last()).Name);
		untouched.Remove(missing.Id);
		context.Clans.Remove(missing);
		context.SaveChanges();
		Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, seeder.ShouldSeedData(context));
		seeder.SeedData(context, new Dictionary<string, string>());
		Assert.IsTrue(context.Clans.Any(x => x.Name == missing.Name));
		Assert.IsTrue(untouched.All(id => context.Clans.Any(x => x.Id == id)));
		Assert.AreEqual(23, context.Appointments.Find(office.Id)!.MaximumSimultaneousHolders);
		Assert.IsTrue(context.Appointments.Find(office.Id)!.CanDirectlyAppoint);
		Assert.AreEqual("Keep me", context.Clans.Find(legacy.Id)!.Description);
		Assert.IsFalse(context.ClanMemberships.Any());
		Assert.IsFalse(context.Elections.Any());
		Assert.IsFalse(context.ExternalClanControls.Any());
		var statusProg = context.FutureProgs.Single(x => x.FunctionName == "CatholicClanStatus");
		statusProg.FunctionText = "return \"Builder wording\"";
		context.FutureProgs.Remove(context.FutureProgs.Single(x => x.FunctionName == "CatholicClanExport"));
		context.SaveChanges();
		Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, seeder.ShouldSeedData(context));
		seeder.SeedData(context, new Dictionary<string, string>());
		Assert.IsTrue(context.FutureProgs.Any(x => x.FunctionName == "CatholicClanExport"));
		Assert.AreEqual("return \"Builder wording\"", context.FutureProgs.Find(statusProg.Id)!.FunctionText);
	}

	[TestMethod]
	public void CatholicInstall_IdentityConflict_RejectsBeforeAnyWrites()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var definition = CatholicTemplateCatalogue.Get(CatholicTemplateCatalogue.Keys.First());
		context.Clans.Add(new ClanModel { Name = "Independent", Alias = definition.Alias, PayIntervalReferenceDate = "1-1-1", PayIntervalReferenceTime = "UTC 0:0:0" });
		context.SaveChanges();
		var ids = context.Clans.Select(x => x.Id).ToList();
		Assert.ThrowsExactly<InvalidOperationException>(() => new ClanSeeder().SeedData(context, new Dictionary<string, string>()));
		CollectionAssert.AreEquivalent(ids, context.Clans.Select(x => x.Id).ToList());
		Assert.IsFalse(context.Ranks.Any());
		Assert.IsFalse(context.FutureProgs.Any());
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void CatholicInstall_PartialIdentity_ReportsWithoutRepairingOrInstallingOtherMissingTemplates()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new ClanSeeder();
		seeder.SeedData(context, new Dictionary<string, string>());
		var definition = CatholicTemplateCatalogue.Get(CatholicTemplateCatalogue.Keys.First());
		var template = context.Clans.Single(x => x.Name == definition.Name);
		context.Appointments.Remove(context.Appointments.First(x => x.ClanId == template.Id));
		context.Clans.Remove(context.Clans.Single(x => x.Name == CatholicTemplateCatalogue.Get(CatholicTemplateCatalogue.Keys.Last()).Name));
		context.SaveChanges();
		var remainingIds = context.Clans.Select(x => x.Id).ToList();
		Assert.ThrowsExactly<InvalidOperationException>(() => seeder.SeedData(context, new Dictionary<string, string>()));
		CollectionAssert.AreEquivalent(remainingIds, context.Clans.Select(x => x.Id).ToList());
	}

    private static bool HasAnyPropertyPrivilege(ClanPrivilegeType privileges)
    {
        return privileges.HasFlag(ClanPrivilegeType.CanAccessLeasedProperties) ||
               privileges.HasFlag(ClanPrivilegeType.CanManageClanProperty);
    }

    private static FuturemudDatabaseContext BuildContext()
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new FuturemudDatabaseContext(options);
    }

    private static void SeedPrerequisites(FuturemudDatabaseContext context)
    {
        context.Accounts.Add(new Account
        {
            Id = 1,
            Name = "SeederTest",
            Password = "password",
            Salt = 1,
            AccessStatus = 0,
            Email = "seeder@example.com",
            LastLoginIp = "127.0.0.1",
            FormatLength = 80,
            InnerFormatLength = 78,
            UseMxp = false,
            UseMsp = false,
            UseMccp = false,
            ActiveCharactersAllowed = 1,
            UseUnicode = true,
            TimeZoneId = "UTC",
            CultureName = "en-AU",
            RegistrationCode = string.Empty,
            IsRegistered = true,
            RecoveryCode = string.Empty,
            UnitPreference = "metric",
            CreationDate = DateTime.UtcNow,
            PageLength = 22,
            PromptType = 0,
            TabRoomDescriptions = false,
            CodedRoomDescriptionAdditionsOnNewLine = false,
            CharacterNameOverlaySetting = 0,
            AppendNewlinesBetweenMultipleEchoesPerPrompt = false,
            ActLawfully = false,
            HasBeenActiveInWeek = true,
            HintsEnabled = true,
            AutoReacquireTargets = false
        });

        context.Currencies.Add(new Currency
        {
            Id = 1,
            Name = "Test Crown",
            BaseCurrencyToGlobalBaseCurrencyConversion = 1.0m
        });

        Clock clock = new()
        {
            Id = 1,
            Definition = "<Clock />",
            Seconds = 0,
            Minutes = 0,
            Hours = 8,
            PrimaryTimezoneId = 1
        };
        Timezone timezone = new()
        {
            Id = 1,
            Name = "UTC",
            Description = "Test timezone",
            OffsetHours = 0,
            OffsetMinutes = 0,
            ClockId = 1,
            Clock = clock
        };
        clock.Timezones.Add(timezone);

        context.Clocks.Add(clock);
        context.Calendars.Add(new Calendar
        {
            Id = 1,
            Definition = "<Calendar />",
            Date = "1-1-1",
            FeedClockId = 1
        });
        context.Timezones.Add(timezone);
        context.SaveChanges();
    }
}
