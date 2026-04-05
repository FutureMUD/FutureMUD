#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Calendar = MudSharp.Models.Calendar;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ClanSeederPrivilegeTests
{
    [TestMethod]
    public void SeedData_PropertyCapableTemplates_ShouldAlsoGrantUseClanProperty()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedPrerequisites(context);

        string result = new ClanSeeder().SeedData(context, new System.Collections.Generic.Dictionary<string, string>());

        Assert.AreEqual("Created 20 new clans.", result);

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
