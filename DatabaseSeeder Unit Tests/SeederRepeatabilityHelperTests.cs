#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Celestial;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CrimeTypes = MudSharp.RPG.Law.CrimeTypes;
using EnforcementStrategy = MudSharp.RPG.Law.EnforcementStrategy;
using ProgVariableTypes = MudSharp.FutureProg.ProgVariableTypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SeederRepeatabilityHelperTests
{
    private sealed class WitnessProfileGuardContext : FuturemudDatabaseContext
    {
        public WitnessProfileGuardContext(DbContextOptions<FuturemudDatabaseContext> options)
            : base(options)
        {
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AssertNoIncompleteWitnessProfiles();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override int SaveChanges()
        {
            AssertNoIncompleteWitnessProfiles();
            return base.SaveChanges();
        }

        private void AssertNoIncompleteWitnessProfiles()
        {
            WitnessProfile? incompleteProfile = ChangeTracker.Entries<WitnessProfile>()
                .Where(x => x.State == EntityState.Added)
                .Select(x => x.Entity)
                .FirstOrDefault(x => x.IdentityKnownProg is null || x.ReportingMultiplierProg is null);
            if (incompleteProfile is null)
            {
                return;
            }

            throw new AssertFailedException(
                $"Witness profile '{incompleteProfile.Name}' was saved before both required progs were assigned.");
        }
    }

    private static FuturemudDatabaseContext BuildContext()
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new FuturemudDatabaseContext(options);
    }

    private static FuturemudDatabaseContext BuildWitnessProfileGuardContext()
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new WitnessProfileGuardContext(options);
    }

    private static void SetSeederProperty<T>(LawSeeder seeder, string propertyName, T value)
    {
        typeof(LawSeeder)
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .GetSetMethod(true)!
            .Invoke(seeder, [value]);
    }

    private static void AddLawSeederPrerequisites(FuturemudDatabaseContext context)
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
            Name = "Bits",
            BaseCurrencyToGlobalBaseCurrencyConversion = 1.0m
        });
        context.SaveChanges();
    }

    private static Dictionary<string, string> BuildLawSeederAnswers(
        string punishmentLevel = "western",
        string createAi = "no",
        string classes = "noble citizen enforcer")
    {
        return new Dictionary<string, string>
        {
            ["name"] = "TestAuthority",
            ["currency"] = "1",
            ["createai"] = createAi,
            ["separatepowers"] = "yes",
            ["punishmentlevel"] = punishmentLevel,
            ["classes"] = classes,
            ["religiouslaws"] = "no",
            ["penaltyunits"] = "100"
        };
    }

    private static string SeedLawSeeder(
        FuturemudDatabaseContext context,
        string punishmentLevel = "western",
        string createAi = "no",
        string classes = "noble citizen enforcer")
    {
        LawSeeder seeder = new();
        return seeder.SeedData(context, BuildLawSeederAnswers(punishmentLevel, createAi, classes));
    }

    [TestMethod]
    public void EnsureEntity_UsesScopedClientEvaluation_ForCaseInsensitiveMatches()
    {
        using FuturemudDatabaseContext context = BuildContext();
        context.Timezones.Add(new Timezone
        {
            Id = 1,
            Name = "utc",
            Description = "Existing timezone",
            ClockId = 42,
            OffsetHours = 0,
            OffsetMinutes = 0
        });
        context.SaveChanges();

        bool created = false;
        Timezone timezone = SeederRepeatabilityHelper.EnsureEntity(
            context.Timezones,
            x => x.ClockId == 42 && string.Equals(x.Name, "UTC", StringComparison.OrdinalIgnoreCase),
            x => x.ClockId == 42,
            () =>
            {
                created = true;
                Timezone entity = new()
                {
                    Id = 2,
                    Name = "UTC",
                    ClockId = 42
                };
                context.Timezones.Add(entity);
                return entity;
            });

        Assert.IsFalse(created);
        Assert.AreEqual(1L, timezone.Id);
        Assert.AreEqual(1, context.Timezones.Count());
    }

    [TestMethod]
    public void EnsureNamedEntity_ReusesExistingRecord_IrrespectiveOfCase()
    {
        using FuturemudDatabaseContext context = BuildContext();
        context.Currencies.Add(new Currency
        {
            Id = 1,
            Name = "bits",
            BaseCurrencyToGlobalBaseCurrencyConversion = 1.0m
        });
        context.SaveChanges();

        bool created = false;
        Currency currency = SeederRepeatabilityHelper.EnsureNamedEntity(
            context.Currencies,
            "Bits",
            x => x.Name,
            () =>
            {
                created = true;
                Currency entity = new()
                {
                    Id = 2,
                    Name = "Bits",
                    BaseCurrencyToGlobalBaseCurrencyConversion = 1.0m
                };
                context.Currencies.Add(entity);
                return entity;
            });

        Assert.IsFalse(created);
        Assert.AreEqual(1L, currency.Id);
        Assert.AreEqual(1, context.Currencies.Count());
    }

    [TestMethod]
    public void TimeSeeder_SeedData_ReusesExistingTimezoneWithDifferentCase()
    {
        using FuturemudDatabaseContext context = BuildContext();
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
        context.Clocks.Add(new Clock
        {
            Id = 1,
            Definition = "<Clock><Alias>utc</Alias></Clock>",
            Seconds = 0,
            Minutes = 0,
            Hours = 0
        });
        context.Timezones.Add(new Timezone
        {
            Id = 1,
            Name = "utc",
            Description = "Existing timezone",
            ClockId = 1,
            OffsetHours = 0,
            OffsetMinutes = 0
        });
        context.SaveChanges();

        TimeSeeder seeder = new();
        string result = seeder.SeedData(context, new Dictionary<string, string>
        {
            ["secondsmultiplier"] = "2",
            ["mode"] = "gregorian-uk",
            ["startyear"] = "2010"
        });

        Assert.AreEqual("Successfully set up clock and calendar.", result);
        Assert.AreEqual(1, context.Timezones.Count(x => x.ClockId == 1));
        Assert.AreEqual("UTC", context.Timezones.Single(x => x.ClockId == 1).Name);
    }

    [TestMethod]
    public void EconomySeeder_ResolutionHelpers_AreCaseInsensitive()
    {
        using FuturemudDatabaseContext context = BuildContext();
        context.Currencies.Add(new Currency
        {
            Id = 1,
            Name = "Bits",
            BaseCurrencyToGlobalBaseCurrencyConversion = 1.0m
        });
        context.Zones.Add(new Zone
        {
            Id = 1,
            Name = "Test Zone",
            ShardId = 1,
            Latitude = 0.0,
            Longitude = 0.0,
            Elevation = 0.0,
            AmbientLightPollution = 0.0
        });
        context.SaveChanges();

        Currency? currency = (Currency?)typeof(EconomySeeder)
            .GetMethod("ResolveCurrencyOrNull", BindingFlags.NonPublic | BindingFlags.Static)!
            .Invoke(null, [context, "bits"]);
        Zone? zone = (Zone?)typeof(EconomySeeder)
            .GetMethod("ResolveZoneOrNull", BindingFlags.NonPublic | BindingFlags.Static)!
            .Invoke(null, [context, "test zone"]);

        Assert.IsNotNull(currency);
        Assert.AreEqual(1L, currency.Id);
        Assert.IsNotNull(zone);
        Assert.AreEqual(1L, zone.Id);
    }

    [TestMethod]
    public void LawSeeder_CurrencyValidator_AcceptsCaseInsensitivePrefixes()
    {
        using FuturemudDatabaseContext context = BuildContext();
        context.Currencies.Add(new Currency
        {
            Id = 1,
            Name = "Bits",
            BaseCurrencyToGlobalBaseCurrencyConversion = 1.0m
        });
        context.SaveChanges();

        Func<string, FuturemudDatabaseContext, (bool Success, string error)> validator = new LawSeeder().SeederQuestions.Single(x => x.Id == "currency").Validator;
        (bool Success, string error) result = validator("bit", context);

        Assert.IsTrue(result.Success);
        Assert.AreEqual(string.Empty, result.error);
    }

    [TestMethod]
    public void LawSeeder_AddWitnessProfile_DoesNotFlushIncompleteWitnessProfiles()
    {
        using FuturemudDatabaseContext context = BuildWitnessProfileGuardContext();
        LegalAuthority authority = new()
        {
            Id = 1,
            Name = "Test Authority"
        };
        context.LegalAuthorities.Add(authority);
        context.SaveChanges();

        LawSeeder seeder = new();
        SetSeederProperty(seeder, nameof(LawSeeder.Context), context);
        SetSeederProperty(seeder, nameof(LawSeeder.Authority), authority);
        SetSeederProperty(seeder, nameof(LawSeeder.AuthorityName), authority.Name);
        SetSeederProperty(
            seeder,
            nameof(LawSeeder.Classes),
            new Dictionary<string, LegalClass>(StringComparer.OrdinalIgnoreCase));

        typeof(LawSeeder)
            .GetMethod("AddWitnessProfile", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(
                seeder,
                [
                    "Always Report Everything",
                    1.0,
                    1.0,
                    new[] { TimeOfDay.Morning, TimeOfDay.Afternoon },
                    Array.Empty<string>(),
                    Array.Empty<string>()
                ]);

        context.SaveChanges();

        WitnessProfile profile = context.WitnessProfiles.Single();
        Assert.IsNotNull(profile.IdentityKnownProg);
        Assert.IsNotNull(profile.ReportingMultiplierProg);
        Assert.AreEqual(2, context.FutureProgs.Count());
        Assert.AreEqual(1, context.WitnessProfiles.Count());
    }

    [TestMethod]
    public void LawSeeder_SeedData_WithCreateAi_SeedsAuthorityAiAndRerunsIdempotently()
    {
        using FuturemudDatabaseContext context = BuildContext();
        AddLawSeederPrerequisites(context);

        string firstResult = SeedLawSeeder(context, createAi: "yes");
        string secondResult = SeedLawSeeder(context, createAi: "yes");

        Assert.AreEqual("Successfully set up a legal authority.", firstResult);
        Assert.AreEqual("Successfully set up a legal authority.", secondResult);
        Assert.AreEqual(3, context.ArtificialIntelligences.Count());
        Assert.AreEqual(1, context.ArtificialIntelligences.Count(x => x.Name == "EnforcerTestAuthority"));
        Assert.AreEqual(1, context.ArtificialIntelligences.Count(x => x.Name == "JudgeTestAuthority"));
        Assert.AreEqual(1, context.ArtificialIntelligences.Count(x => x.Name == "LawyerTestAuthority"));
        Assert.AreEqual(1, context.FutureProgs.Count(x => x.FunctionName == "LawyerCanHireTemplateTestAuthority"));
        Assert.AreEqual(1, context.FutureProgs.Count(x => x.FunctionName == "LawyerFeeTemplateTestAuthority"));
        Assert.AreEqual(1, context.FutureProgs.Count(x => x.FunctionName == "LawyerHomeBaseTemplateTestAuthority"));
        Assert.AreEqual(1, context.FutureProgs.Count(x => x.FunctionName == "LawyerBankAccountTemplateTestAuthority"));

        ArtificialIntelligence judgeAi = context.ArtificialIntelligences.Single(x => x.Name == "JudgeTestAuthority");
        ArtificialIntelligence lawyerAi = context.ArtificialIntelligences.Single(x => x.Name == "LawyerTestAuthority");
        FutureProg identifyProg = context.FutureProgs.Single(x => x.FunctionName == "IsIdentityKnownTestAuthority");
        FutureProg feeProg = context.FutureProgs.Single(x => x.FunctionName == "LawyerFeeTemplateTestAuthority");
        FutureProg homeProg = context.FutureProgs.Single(x => x.FunctionName == "LawyerHomeBaseTemplateTestAuthority");
        FutureProg bankProg = context.FutureProgs.Single(x => x.FunctionName == "LawyerBankAccountTemplateTestAuthority");
        FutureProg hireProg = context.FutureProgs.Single(x => x.FunctionName == "LawyerCanHireTemplateTestAuthority");

        StringAssert.Contains(judgeAi.Definition, $"<IdentityProg>{identifyProg.Id}</IdentityProg>");
        StringAssert.Contains(judgeAi.Definition, "<IntroductionDelay>15</IntroductionDelay>");
        StringAssert.Contains(lawyerAi.Definition, $"<CanBeHiredProg>{hireProg.Id}</CanBeHiredProg>");
        StringAssert.Contains(lawyerAi.Definition, $"<FeeProg>{feeProg.Id}</FeeProg>");
        StringAssert.Contains(lawyerAi.Definition, $"<HomeBaseProg>{homeProg.Id}</HomeBaseProg>");
        StringAssert.Contains(lawyerAi.Definition, $"<BankAccountProg>{bankProg.Id}</BankAccountProg>");
    }

    [TestMethod]
    public void LawSeeder_SeedData_AssignsLawAppliesProgToEverySeededLaw()
    {
        using FuturemudDatabaseContext context = BuildContext();
        AddLawSeederPrerequisites(context);

        SeedLawSeeder(context);

        Assert.IsTrue(context.Laws.Any());
        Assert.AreEqual(context.Laws.Count(), context.Laws.Count(x => x.LawAppliesProgId != null));

        Law murderLaw = context.Laws.Single(x => x.Name == "Murder");
        FutureProg murderApplicabilityProg = context.FutureProgs.Single(x => x.Id == murderLaw.LawAppliesProgId);

        Assert.AreEqual("return true", murderApplicabilityProg.FunctionText);
    }

    [TestMethod]
    public void LawSeeder_SeedData_Tiered_SplitsVictimBasedCrimesAndLeavesVictimlessCrimesFlat()
    {
        using FuturemudDatabaseContext context = BuildContext();
        AddLawSeederPrerequisites(context);

        SeedLawSeeder(context, punishmentLevel: "tiered");

        Law murderLaw = context.Laws.Single(x => x.Name == "Murder");
        Law murderAgainstInferiorLaw = context.Laws.Single(x => x.Name == "Murder Against Inferior");
        FutureProg murderApplicabilityProg = context.FutureProgs.Single(x => x.Id == murderLaw.LawAppliesProgId);
        FutureProg murderAgainstInferiorApplicabilityProg =
            context.FutureProgs.Single(x => x.Id == murderAgainstInferiorLaw.LawAppliesProgId);

        Assert.AreEqual(EnforcementStrategy.LethalForceArrestAndDetain.DescribeEnum(), murderLaw.EnforcementStrategy);
        Assert.AreEqual(EnforcementStrategy.NoActiveEnforcement.DescribeEnum(), murderAgainstInferiorLaw.EnforcementStrategy);
        Assert.IsFalse(murderAgainstInferiorLaw.CanBeArrested);
        Assert.IsFalse(murderAgainstInferiorLaw.CanBeOfferedBail);
        StringAssert.Contains(murderAgainstInferiorLaw.PunishmentStrategy, "type=\"fine\"");
        Assert.IsFalse(murderAgainstInferiorLaw.PunishmentStrategy.Contains("type=\"jail\""));
        Assert.IsFalse(murderAgainstInferiorLaw.PunishmentStrategy.Contains("type=\"execute\""));
        StringAssert.Contains(murderApplicabilityProg.FunctionText, "isnull(@victim)");
        StringAssert.Contains(murderAgainstInferiorApplicabilityProg.FunctionText, "isnull(@victim)");

        Law theftAgainstInferiorLaw = context.Laws.Single(x => x.Name == "Theft Against Inferior");
        StringAssert.Contains(theftAgainstInferiorLaw.PunishmentStrategy, "type=\"fine\"");
        Assert.AreEqual(EnforcementStrategy.NoActiveEnforcement.DescribeEnum(), theftAgainstInferiorLaw.EnforcementStrategy);

        List<Law> gamblingLaws = context.Laws.Where(x => x.CrimeType == (int)CrimeTypes.Gambling).ToList();
        Assert.AreEqual(1, gamblingLaws.Count);
        Assert.AreEqual(0, context.Laws.Count(x => x.Name == "Gambling Against Inferior"));

        FutureProg gamblingApplicabilityProg =
            context.FutureProgs.Single(x => x.Id == gamblingLaws.Single().LawAppliesProgId);
        Assert.AreEqual("return true", gamblingApplicabilityProg.FunctionText);
        Assert.AreEqual(EnforcementStrategy.ArrestAndDetain.DescribeEnum(), gamblingLaws.Single().EnforcementStrategy);
    }
}
