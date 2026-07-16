#nullable enable

using DatabaseSeeder;
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
using AppointmentModel = MudSharp.Models.Appointment;
using Calendar = MudSharp.Models.Calendar;
using ClanModel = MudSharp.Models.Clan;
using RankModel = MudSharp.Models.Rank;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ClanSeederPrivilegeTests
{
	private static readonly string[] AdditionalTemplateNames =
	[
		"Extended Family Template",
		"Lineage Clan Template",
		"Tribal Council Template",
		"Norse Warband Template",
		"Steppe Horde Template",
		"Japanese Feudal Domain Template",
		"East Asian Imperial Bureaucracy Template",
		"East Asian Imperial Army Template",
		"Islamic Sultanate Court Template",
		"South Asian Royal Court Template",
		"West African Royal Court Template",
		"Roman Religious Cult Template",
		"Buddhist Temple Template",
		"Daoist Temple Template",
		"Sufi Order Template",
		"Hindu Temple Template",
		"Merchant Guild Template",
		"Craft Guild Template",
		"Pirate Crew Template",
		"University Template",
		"Hospital Template",
		"Fire and Rescue Service Template",
		"Intelligence Agency Template",
		"Political Party Template",
		"Labour Union Template",
		"Non-Governmental Organisation Template",
		"Space Colony Administration Template",
		"Civilian Starship Crew Template",
		"Exploration Corps Template",
		"Resistance Network Template"
	];

    [TestMethod]
    public void SeedData_PropertyCapableTemplates_ShouldAlsoGrantUseClanProperty()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedPrerequisites(context);

        string result = new ClanSeeder().SeedData(context, new System.Collections.Generic.Dictionary<string, string>());

        Assert.AreEqual("Created 50 new clans.", result);

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
	public void SeedData_AdditionalTemplates_ShouldCoverDistinctOrganisationFamilies()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedPrerequisites(context);

		string result = new ClanSeeder().SeedData(context, new Dictionary<string, string>());

		Assert.AreEqual("Created 50 new clans.", result);
		List<ClanModel> additionalTemplates = context.Clans
			.Where(x => AdditionalTemplateNames.Contains(x.Name))
			.ToList();
		Assert.AreEqual(AdditionalTemplateNames.Length, additionalTemplates.Count);
		Assert.IsTrue(additionalTemplates.All(x => x.IsTemplate));
		Assert.IsTrue(additionalTemplates.All(x => !string.IsNullOrWhiteSpace(x.Sphere)));
		CollectionAssert.IsSubsetOf(
			new[] { "Kinship", "Traditional Government", "Historical Military", "Religion", "Commerce",
				"Education", "Health", "Emergency Services", "Intelligence", "Politics", "Labour", "Civil Society",
				"Science Fiction", "Clandestine" },
			additionalTemplates.Select(x => x.Sphere).Distinct().ToArray());
	}

	[TestMethod]
	public void SeedData_AdditionalTemplates_ShouldHaveUsableRankAndAppointmentStructures()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedPrerequisites(context);

		new ClanSeeder().SeedData(context, new Dictionary<string, string>());

		foreach (string templateName in AdditionalTemplateNames)
		{
			ClanModel clan = context.Clans.Single(x => x.Name == templateName);
			List<RankModel> ranks = context.Ranks.Where(x => x.ClanId == clan.Id).ToList();
			List<AppointmentModel> appointments = context.Appointments.Where(x => x.ClanId == clan.Id).ToList();

			Assert.IsTrue(ranks.Count >= 4, $"{templateName} should provide at least four ranks.");
			Assert.IsTrue(appointments.Count >= 5, $"{templateName} should provide at least five appointments.");
			Assert.AreEqual(ranks.Count, ranks.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
				$"{templateName} should not contain duplicate rank names.");
			Assert.AreEqual(appointments.Count,
				appointments.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
				$"{templateName} should not contain duplicate appointment names.");
			Assert.IsTrue(appointments.Any(x => x.ParentAppointmentId.HasValue),
				$"{templateName} should provide an appointment hierarchy.");
		}
	}

	[TestMethod]
	public void SeedData_SecondRun_ShouldNotDuplicateExpandedTemplateCatalogue()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedPrerequisites(context);
		ClanSeeder seeder = new();

		string firstResult = seeder.SeedData(context, new Dictionary<string, string>());
		string secondResult = seeder.SeedData(context, new Dictionary<string, string>());

		Assert.AreEqual("Created 50 new clans.", firstResult);
		Assert.AreEqual("Created 0 new clans.", secondResult);
		Assert.AreEqual(50, context.Clans.Count());
		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, seeder.ShouldSeedData(context));
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
