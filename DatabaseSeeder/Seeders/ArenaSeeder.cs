#nullable enable
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MudSharp.Arenas;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseSeeder.Seeders;

public class ArenaSeeder : IDatabaseSeeder
{
    private const string DefaultArenaName = "Grand Coliseum";
    private const string EligibilityProgName = "ArenaAlwaysEligible";
    private const string OutfitProgName = "ArenaDefaultOutfit";
    private const string IntroProgName = "ArenaStandardIntro";
    private static readonly string[] StockArenaCombatantClasses = ["Gladiator", "Pit Fighter"];
    private static readonly string[] StockArenaEventTypes = ["Duel", "Team Skirmish"];

    public IEnumerable<(string Id, string Question,
        Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
        Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions
        => new List<(string, string, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool>,
            Func<string, FuturemudDatabaseContext, (bool, string)>)>
        {
            ("arena-name",
                "What should the example arena be called? Leave blank for \"Grand Coliseum\".",
                (context, answers) => true,
                (answer, context) => (true, string.Empty)),
            ("arena-zone",
                "Enter the ID of the economic zone that will operate this arena.",
                (context, answers) => context.EconomicZones.Count() > 1,
                ValidateZone)
        };

    public int SortOrder => 110;

    public string Name => "Arena";

    public string Tagline => "Installs an example combat arena with duel and skirmish events.";

    public string FullDescription =>
        @"This package creates a ready-to-configure combat arena tied to one of your economic zones. It seeds
combatant classes, example event types (1v1 and 2v2), and baseline FutureProgs in the ""Arena"" category that
builders can expand upon. Physical rooms for the arena still need to be constructed in-game.";

    public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
    {
        using IDbContextTransaction transaction = context.Database.BeginTransaction();
        try
        {
            EconomicZone zone = ResolveEconomicZone(context, questionAnswers);
            string arenaName = ResolveArenaName(questionAnswers);
            DateTime now = DateTime.UtcNow;

            FutureProg eligibilityProg = EnsureProg(context,
                EligibilityProgName,
                "Arena",
                "Eligibility",
                ProgVariableTypes.Boolean,
                "Default eligibility prog that accepts all characters.",
                "return true",
                (ProgVariableTypes.Character, "character"));

            FutureProg outfitProg = EnsureProg(context,
                OutfitProgName,
                "Arena",
                "Loadout",
                ProgVariableTypes.Void,
                "Placeholder outfit prog for bring-your-own events.",
                "return",
                (ProgVariableTypes.Anything, "event"),
                (ProgVariableTypes.Number, "side"),
                (ProgVariableTypes.Character | ProgVariableTypes.Collection, "participants"));

            FutureProg introProg = EnsureProg(context,
                IntroProgName,
                "Arena",
                "Lifecycle",
                ProgVariableTypes.Void,
                "Placeholder intro prog that performs no announcements.",
                "return",
                (ProgVariableTypes.Anything, "event"));

            context.SaveChanges();

            Arena arena = SeederRepeatabilityHelper.EnsureNamedEntity(
                context.Arenas,
                arenaName,
                x => x.Name,
                () =>
                {
                    Arena created = new();
                    context.Arenas.Add(created);
                    return created;
                });
            arena.Name = arenaName;
            arena.EconomicZoneId = zone.Id;
            arena.CurrencyId = zone.CurrencyId;
            arena.SignupEcho = "@ sign|signs up for the upcoming bout.";
            arena.IsDeleted = false;
            arena.CreatedAt = arena.CreatedAt == default ? now : arena.CreatedAt;
            context.SaveChanges();

            List<ArenaCombatantClass> combatantClasses = new()
            {
                EnsureCombatantClass(context, arena, "Gladiator",
                    "A well-rounded combatant suitable for the seeded arena formats.", eligibilityProg.Id),
                EnsureCombatantClass(context, arena, "Pit Fighter",
                    "An aggressive archetype for team skirmishes or showcase bouts.", eligibilityProg.Id)
            };
            context.SaveChanges();

            ArenaEventType duelEvent = EnsureEventType(context, arena, "Duel", true, 600, 120, 900, (int)BettingModel.FixedOdds, introProg.Id);
            ArenaEventType skirmishEvent = EnsureEventType(context, arena, "Team Skirmish", true, 900, 180, 1200, (int)BettingModel.PariMutuel, introProg.Id);
            context.SaveChanges();

            List<ArenaEventTypeSide> duelSides = new()
            {
                EnsureEventTypeSide(context, duelEvent, 0, 1, outfitProg.Id),
                EnsureEventTypeSide(context, duelEvent, 1, 1, outfitProg.Id)
            };
            List<ArenaEventTypeSide> skirmishSides = new()
            {
                EnsureEventTypeSide(context, skirmishEvent, 0, 2, outfitProg.Id),
                EnsureEventTypeSide(context, skirmishEvent, 1, 2, outfitProg.Id)
            };
            context.SaveChanges();

            foreach (ArenaEventTypeSide? side in duelSides.Concat(skirmishSides))
            {
                foreach (ArenaEventTypeSideAllowedClass? existing in context.ArenaEventTypeSideAllowedClasses
                             .Where(x => x.ArenaEventTypeSideId == side.Id)
                             .ToList())
                {
                    context.ArenaEventTypeSideAllowedClasses.Remove(existing);
                }

                foreach (ArenaCombatantClass cls in combatantClasses)
                {
                    context.ArenaEventTypeSideAllowedClasses.Add(new ArenaEventTypeSideAllowedClass
                    {
                        ArenaEventTypeSideId = side.Id,
                        ArenaCombatantClassId = cls.Id
                    });
                }
            }

            context.SaveChanges();
            transaction.Commit();

            StringBuilder sb = new();
            sb.AppendLine($"Created combat arena '{arena.Name}' in economic zone '{zone.Name}'.");
            sb.AppendLine($"Added {combatantClasses.Count} combatant classes and 2 example event types.");
            sb.AppendLine("Installed baseline Arena FutureProgs for eligibility, outfits, and intros.");
            return sb.ToString();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }



    public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
    {
        if (!context.EconomicZones.Any())
        {
            return ShouldSeedResult.PrerequisitesNotMet;
        }

        return SeederRepeatabilityHelper.ClassifyByPresence(
        [
            context.Arenas.Any(x => x.Name == DefaultArenaName),
            context.ArenaCombatantClasses.Any(x => StockArenaCombatantClasses.Contains(x.Name)),
            context.ArenaEventTypes.Any(x => StockArenaEventTypes.Contains(x.Name)),
            context.FutureProgs.Any(x => x.FunctionName == EligibilityProgName),
            context.FutureProgs.Any(x => x.FunctionName == OutfitProgName),
            context.FutureProgs.Any(x => x.FunctionName == IntroProgName)
        ]);
    }

    private static EconomicZone ResolveEconomicZone(FuturemudDatabaseContext context,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        if (questionAnswers.TryGetValue("arena-zone", out string? zoneAnswer) && !string.IsNullOrWhiteSpace(zoneAnswer) &&
        long.TryParse(zoneAnswer, out long zoneId))
        {
            return context.EconomicZones.Include(x => x.Currency).First(x => x.Id == zoneId);
        }

        return context.EconomicZones.Include(x => x.Currency).First();
    }

    private static string ResolveArenaName(IReadOnlyDictionary<string, string> questionAnswers)
    {
        if (!questionAnswers.TryGetValue("arena-name", out string? name) || string.IsNullOrWhiteSpace(name))
        {
            return DefaultArenaName;
        }

        return name.Trim();
    }

    private static (bool Success, string error) ValidateZone(string answer, FuturemudDatabaseContext context)
    {
        if (!long.TryParse(answer, out long zoneId))
        {
            return (false, BuildZoneError(context, "You must enter a numeric economic zone ID."));
        }

        if (!context.EconomicZones.Any(x => x.Id == zoneId))
        {
            return (false, BuildZoneError(context, "There is no economic zone with that ID."));
        }

        return (true, string.Empty);
    }

    private static string BuildZoneError(FuturemudDatabaseContext context, string prefix)
    {
        List<string> zones = context.EconomicZones.OrderBy(x => x.Name).Select(x => $"{x.Id}: {x.Name}").ToList();
        if (!zones.Any())
        {
            return $"{prefix} No economic zones are defined.";
        }

        StringBuilder sb = new();
        sb.AppendLine(prefix);
        sb.AppendLine("Available economic zones:");
        foreach (string? line in zones)
        {
            sb.AppendLine($"  {line}");
        }

        return sb.ToString();
    }

    private static FutureProg EnsureProg(FuturemudDatabaseContext context, string functionName, string category,
        string subcategory, ProgVariableTypes returnType, string comment, string text,
        params (ProgVariableTypes Type, string Name)[] parameters)
    {
        return SeederRepeatabilityHelper.EnsureProg(
            context,
            functionName,
            category,
            subcategory,
            returnType,
            comment,
            text,
            false,
            false,
            FutureProgStaticType.NotStatic,
            parameters);
    }

    private static ArenaCombatantClass EnsureCombatantClass(FuturemudDatabaseContext context, Arena arena, string name,
        string description, long eligibilityProgId)
    {
        ArenaCombatantClass combatantClass = SeederRepeatabilityHelper.EnsureEntity(
            context.ArenaCombatantClasses,
            x => x.ArenaId == arena.Id && x.Name == name,
            () =>
            {
                ArenaCombatantClass created = new();
                context.ArenaCombatantClasses.Add(created);
                return created;
            });

        combatantClass.ArenaId = arena.Id;
        combatantClass.Name = name;
        combatantClass.Description = description;
        combatantClass.EligibilityProgId = eligibilityProgId;
        combatantClass.ResurrectNpcOnDeath = true;
        return combatantClass;
    }

    private static ArenaEventType EnsureEventType(FuturemudDatabaseContext context, Arena arena, string name,
        bool bringYourOwn, int registrationSeconds, int preparationSeconds, int? timeLimitSeconds, int bettingModel,
        long introProgId)
    {
        ArenaEventType eventType = SeederRepeatabilityHelper.EnsureEntity(
            context.ArenaEventTypes,
            x => x.ArenaId == arena.Id && x.Name == name,
            () =>
            {
                ArenaEventType created = new();
                context.ArenaEventTypes.Add(created);
                return created;
            });

        eventType.ArenaId = arena.Id;
        eventType.Name = name;
        eventType.BringYourOwn = bringYourOwn;
        eventType.RegistrationDurationSeconds = registrationSeconds;
        eventType.PreparationDurationSeconds = preparationSeconds;
        eventType.TimeLimitSeconds = timeLimitSeconds;
        eventType.BettingModel = bettingModel;
        eventType.AppearanceFee = 0.0m;
        eventType.VictoryFee = 0.0m;
        eventType.IntroProgId = introProgId;
        return eventType;
    }

    private static ArenaEventTypeSide EnsureEventTypeSide(FuturemudDatabaseContext context, ArenaEventType eventType,
        int index, int capacity, long outfitProgId)
    {
        ArenaEventTypeSide side = SeederRepeatabilityHelper.EnsureEntity(
            context.ArenaEventTypeSides,
            x => x.ArenaEventTypeId == eventType.Id && x.Index == index,
            () =>
            {
                ArenaEventTypeSide created = new();
                context.ArenaEventTypeSides.Add(created);
                return created;
            });

        side.ArenaEventTypeId = eventType.Id;
        side.Index = index;
        side.Capacity = capacity;
        side.Policy = (int)ArenaSidePolicy.Open;
        side.AllowNpcSignup = true;
        side.AutoFillNpc = false;
        side.OutfitProgId = outfitProgId;
        return side;
    }
}
