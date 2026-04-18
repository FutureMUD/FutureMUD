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
	private const string AnimalEligibilityProgName = "ArenaAnimalEligible";
	private const string OutfitProgName = "ArenaDefaultOutfit";
	private const string IntroProgName = "ArenaStandardIntro";
	private const string BoxingScoringProgName = "ArenaBoxingScoring";
	private const string AnimalNpcLoaderProgName = "ArenaAnimalNpcLoader";

	private static readonly string[] StockArenaCombatantClasses =
	[
		"Gladiator",
		"Boxer",
		"Wrestler",
		"Arena Animal"
	];

	private static readonly string[] StockArenaEventTypes =
	[
		"Duel",
		"Team Skirmish",
		"Squad Skirmish",
		"Boxing Match",
		"Wrestling Match",
		"Champion Challenge",
		"Animal Bohort"
	];

	private static readonly string[] StockArenaProgNames =
	[
		EligibilityProgName,
		AnimalEligibilityProgName,
		OutfitProgName,
		IntroProgName,
		BoxingScoringProgName,
		AnimalNpcLoaderProgName
	];

	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions
		=> new List<(string, string, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool>,
			Func<string, FuturemudDatabaseContext, (bool, string)>)>
		{
			("arena-name",
				"What should the example arena be called? Leave blank for \"Grand Coliseum\".",
				(_, _) => true,
				(_, _) => (true, string.Empty)),
			("arena-zone",
				"Enter the ID of the economic zone that will operate this arena.",
				(context, _) => context.EconomicZones.Count() > 1,
				ValidateZone)
		};

	public int SortOrder => 110;

	public string Name => "Arena";

	public string Tagline => "Installs an example combat arena with gladiator, boxing, wrestling, champion, and animal formats.";

	public string FullDescription =>
		@"This package creates a ready-to-configure combat arena tied to one of your economic zones. It seeds
combatant classes, stock event types for duels, team fights, boxing, wrestling, champion matches, and animal bouts,
plus baseline Arena FutureProgs for eligibility, outfits, intros, scoring, and NPC loaders. Physical rooms for the
arena still need to be constructed in-game.";

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
				"return true;",
				(ProgVariableTypes.Character, "character"));

			FutureProg animalEligibilityProg = EnsureProg(context,
				AnimalEligibilityProgName,
				"Arena",
				"Eligibility",
				ProgVariableTypes.Boolean,
				"Default arena animal eligibility prog that checks stock animal body lineage.",
				"return isanimal(@character);",
				(ProgVariableTypes.Character, "character"));

			FutureProg outfitProg = EnsureProg(context,
				OutfitProgName,
				"Arena",
				"Loadout",
				ProgVariableTypes.Void,
				"Placeholder outfit prog for seeded arena formats.",
				"return;",
				(ProgVariableTypes.Character | ProgVariableTypes.Collection, "participants"),
				(ProgVariableTypes.Number, "sideIndex"),
				(ProgVariableTypes.Location, "arenaCell"),
				(ProgVariableTypes.Text, "eventTypeName"),
				(ProgVariableTypes.Text, "arenaName"),
				(ProgVariableTypes.Text, "eventName"));

			FutureProg introProg = EnsureProg(context,
				IntroProgName,
				"Arena",
				"Lifecycle",
				ProgVariableTypes.Void,
				"Placeholder intro prog that performs no announcements.",
				"return;",
				(ProgVariableTypes.Character | ProgVariableTypes.Collection, "participants"),
				(ProgVariableTypes.Number | ProgVariableTypes.Collection, "sideIndices"),
				(ProgVariableTypes.Location, "arenaCell"),
				(ProgVariableTypes.Text, "eventTypeName"),
				(ProgVariableTypes.Text, "arenaName"),
				(ProgVariableTypes.Text, "eventName"));

			FutureProg boxingScoringProg = EnsureProg(context,
				BoxingScoringProgName,
				"Arena",
				"Scoring",
				ProgVariableTypes.Number | ProgVariableTypes.Collection,
				"Seeded boxing scoring prog that awards one point for landed undefended head or torso hits.",
				"return arenaboxingscores(@sideIndices, @scoringAttackerSides, @landedHits, @undefendedHits, @impactLocations);",
				(ProgVariableTypes.Character | ProgVariableTypes.Collection, "participants"),
				(ProgVariableTypes.Number | ProgVariableTypes.Collection, "sideIndices"),
				(ProgVariableTypes.Location, "arenaCell"),
				(ProgVariableTypes.Text, "eventTypeName"),
				(ProgVariableTypes.Text, "arenaName"),
				(ProgVariableTypes.Text, "eventName"),
				(ProgVariableTypes.Character | ProgVariableTypes.Collection, "scoringAttackers"),
				(ProgVariableTypes.Character | ProgVariableTypes.Collection, "scoringDefenders"),
				(ProgVariableTypes.Number | ProgVariableTypes.Collection, "scoringAttackerSides"),
				(ProgVariableTypes.Number | ProgVariableTypes.Collection, "scoringDefenderSides"),
				(ProgVariableTypes.Number | ProgVariableTypes.Collection, "landedHits"),
				(ProgVariableTypes.Number | ProgVariableTypes.Collection, "undefendedHits"),
				(ProgVariableTypes.Text | ProgVariableTypes.Collection, "impactLocations"),
				(ProgVariableTypes.Text | ProgVariableTypes.Collection, "impactBodyparts"));

			FutureProg animalNpcLoaderProg = EnsureProg(context,
				AnimalNpcLoaderProgName,
				"Arena",
				"NPC",
				ProgVariableTypes.Character | ProgVariableTypes.Collection,
				"Blank arena animal loader prog that intentionally returns no NPCs.",
				"return emptycharacters();",
				(ProgVariableTypes.Number, "sideIndex"),
				(ProgVariableTypes.Number, "slotsNeeded"),
				(ProgVariableTypes.Location, "arenaCell"),
				(ProgVariableTypes.Text, "eventTypeName"),
				(ProgVariableTypes.Text, "arenaName"),
				(ProgVariableTypes.Text, "eventName"));

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

			ArenaCombatantClass gladiator = EnsureCombatantClass(context, arena, "Gladiator",
				"A well-rounded combatant suitable for open gladiatorial arena formats.", eligibilityProg.Id);
			ArenaCombatantClass boxer = EnsureCombatantClass(context, arena, "Boxer",
				"A regulated boxer who fights unarmed and without carried inventory.", eligibilityProg.Id);
			ArenaCombatantClass wrestler = EnsureCombatantClass(context, arena, "Wrestler",
				"A grappling specialist for arena wrestling bouts decided by knockouts.", eligibilityProg.Id);
			ArenaCombatantClass animal = EnsureCombatantClass(context, arena, "Arena Animal",
				"An animal combatant class gated by the stock isanimal eligibility helper.", animalEligibilityProg.Id);
			context.SaveChanges();

			ArenaEventType duelEvent = EnsureEventType(context, arena, "Duel", true, 600, 120, 900, BettingModel.FixedOdds,
				introProg.Id, ArenaEliminationMode.NoElimination);
			ArenaEventType teamSkirmishEvent = EnsureEventType(context, arena, "Team Skirmish", true, 900, 180, 1200,
				BettingModel.PariMutuel, introProg.Id, ArenaEliminationMode.NoElimination);
			ArenaEventType squadSkirmishEvent = EnsureEventType(context, arena, "Squad Skirmish", true, 900, 180, 1200,
				BettingModel.PariMutuel, introProg.Id, ArenaEliminationMode.NoElimination);
			ArenaEventType boxingEvent = EnsureEventType(context, arena, "Boxing Match", false, 600, 120, 900,
				BettingModel.FixedOdds, introProg.Id, ArenaEliminationMode.PointsElimination, scoringProgId: boxingScoringProg.Id);
			ArenaEventType wrestlingEvent = EnsureEventType(context, arena, "Wrestling Match", false, 600, 120, 900,
				BettingModel.FixedOdds, introProg.Id, ArenaEliminationMode.Knockout);
			ArenaEventType championEvent = EnsureEventType(context, arena, "Champion Challenge", true, 600, 120, 900,
				BettingModel.FixedOdds, introProg.Id, ArenaEliminationMode.NoElimination);
			ArenaEventType animalBohortEvent = EnsureEventType(context, arena, "Animal Bohort", true, 900, 180, 1200,
				BettingModel.PariMutuel, introProg.Id, ArenaEliminationMode.NoElimination);
			context.SaveChanges();

			ArenaEventTypeSide duelSideOne = EnsureEventTypeSide(context, duelEvent, 0, 1, outfitProg.Id);
			ArenaEventTypeSide duelSideTwo = EnsureEventTypeSide(context, duelEvent, 1, 1, outfitProg.Id);
			ArenaEventTypeSide teamSideOne = EnsureEventTypeSide(context, teamSkirmishEvent, 0, 2, outfitProg.Id);
			ArenaEventTypeSide teamSideTwo = EnsureEventTypeSide(context, teamSkirmishEvent, 1, 2, outfitProg.Id);
			ArenaEventTypeSide squadSideOne = EnsureEventTypeSide(context, squadSkirmishEvent, 0, 3, outfitProg.Id);
			ArenaEventTypeSide squadSideTwo = EnsureEventTypeSide(context, squadSkirmishEvent, 1, 3, outfitProg.Id);
			ArenaEventTypeSide boxingSideOne = EnsureEventTypeSide(context, boxingEvent, 0, 1, outfitProg.Id);
			ArenaEventTypeSide boxingSideTwo = EnsureEventTypeSide(context, boxingEvent, 1, 1, outfitProg.Id);
			ArenaEventTypeSide wrestlingSideOne = EnsureEventTypeSide(context, wrestlingEvent, 0, 1, outfitProg.Id);
			ArenaEventTypeSide wrestlingSideTwo = EnsureEventTypeSide(context, wrestlingEvent, 1, 1, outfitProg.Id);
			ArenaEventTypeSide challengerSide = EnsureEventTypeSide(context, championEvent, 0, 1, outfitProg.Id);
			ArenaEventTypeSide championSide = EnsureEventTypeSide(context, championEvent, 1, 1, outfitProg.Id,
				policy: ArenaSidePolicy.Closed,
				allowNpcSignup: false,
				autoFillNpc: true,
				minimumRating: 1800.0m);
			ArenaEventTypeSide animalSideOne = EnsureEventTypeSide(context, animalBohortEvent, 0, 2, outfitProg.Id,
				autoFillNpc: true,
				npcLoaderProgId: animalNpcLoaderProg.Id);
			ArenaEventTypeSide animalSideTwo = EnsureEventTypeSide(context, animalBohortEvent, 1, 2, outfitProg.Id,
				autoFillNpc: true,
				npcLoaderProgId: animalNpcLoaderProg.Id);
			context.SaveChanges();

			ReplaceAllowedClasses(context, duelSideOne, gladiator);
			ReplaceAllowedClasses(context, duelSideTwo, gladiator);
			ReplaceAllowedClasses(context, teamSideOne, gladiator);
			ReplaceAllowedClasses(context, teamSideTwo, gladiator);
			ReplaceAllowedClasses(context, squadSideOne, gladiator);
			ReplaceAllowedClasses(context, squadSideTwo, gladiator);
			ReplaceAllowedClasses(context, boxingSideOne, boxer);
			ReplaceAllowedClasses(context, boxingSideTwo, boxer);
			ReplaceAllowedClasses(context, wrestlingSideOne, wrestler);
			ReplaceAllowedClasses(context, wrestlingSideTwo, wrestler);
			ReplaceAllowedClasses(context, challengerSide, gladiator);
			ReplaceAllowedClasses(context, championSide, gladiator);
			ReplaceAllowedClasses(context, animalSideOne, animal);
			ReplaceAllowedClasses(context, animalSideTwo, animal);

			context.SaveChanges();
			transaction.Commit();

			StringBuilder sb = new();
			sb.AppendLine($"Created combat arena '{arena.Name}' in economic zone '{zone.Name}'.");
			sb.AppendLine($"Added {StockArenaCombatantClasses.Length} stock combatant classes and {StockArenaEventTypes.Length} stock event types.");
			sb.AppendLine($"Installed {StockArenaProgNames.Length} Arena helper FutureProgs for eligibility, outfits, intros, scoring, and NPC loading.");
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

		List<bool> presence =
		[
			context.Arenas.Any(x => x.Name == DefaultArenaName)
		];
		presence.AddRange(StockArenaCombatantClasses.Select(name => context.ArenaCombatantClasses.Any(x => x.Name == name)));
		presence.AddRange(StockArenaEventTypes.Select(name => context.ArenaEventTypes.Any(x => x.Name == name)));
		presence.AddRange(StockArenaProgNames.Select(name => context.FutureProgs.Any(x => x.FunctionName == name)));
		return SeederRepeatabilityHelper.ClassifyByPresence(presence);
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
		foreach (string line in zones)
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
		combatantClass.AdminNpcLoaderProgId = null;
		combatantClass.DefaultStageNameProfileId = null;
		combatantClass.ResurrectNpcOnDeath = true;
		combatantClass.FullyRestoreNpcOnCompletion = true;
		combatantClass.DefaultSignatureColour = string.Empty;
		return combatantClass;
	}

	private static ArenaEventType EnsureEventType(FuturemudDatabaseContext context, Arena arena, string name,
		bool bringYourOwn, int registrationSeconds, int preparationSeconds, int? timeLimitSeconds, BettingModel bettingModel,
		long introProgId, ArenaEliminationMode eliminationMode, bool allowSurrender = false, long? scoringProgId = null)
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
		eventType.AutoScheduleIntervalSeconds = null;
		eventType.AutoScheduleReferenceTime = null;
		eventType.BettingModel = (int)bettingModel;
		eventType.EliminationMode = (int)eliminationMode;
		eventType.AllowSurrender = allowSurrender;
		eventType.AppearanceFee = 0.0m;
		eventType.VictoryFee = 0.0m;
		eventType.PayNpcAppearanceFee = false;
		eventType.IntroProgId = introProgId;
		eventType.ScoringProgId = scoringProgId;
		eventType.ResolutionOverrideProgId = null;
		eventType.EloStyle = (int)ArenaEloStyle.TeamAverage;
		eventType.EloKFactor = 32.0m;
		return eventType;
	}

	private static ArenaEventTypeSide EnsureEventTypeSide(FuturemudDatabaseContext context, ArenaEventType eventType,
		int index, int capacity, long? outfitProgId, ArenaSidePolicy policy = ArenaSidePolicy.Open,
		bool allowNpcSignup = true, bool autoFillNpc = false, decimal? minimumRating = null,
		decimal? maximumRating = null, long? npcLoaderProgId = null)
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
		side.Policy = (int)policy;
		side.MinimumRating = minimumRating;
		side.MaximumRating = maximumRating;
		side.AllowNpcSignup = allowNpcSignup;
		side.AutoFillNpc = autoFillNpc;
		side.OutfitProgId = outfitProgId;
		side.NpcLoaderProgId = npcLoaderProgId;
		return side;
	}

	private static void ReplaceAllowedClasses(FuturemudDatabaseContext context, ArenaEventTypeSide side,
		params ArenaCombatantClass[] allowedClasses)
	{
		foreach (ArenaEventTypeSideAllowedClass existing in context.ArenaEventTypeSideAllowedClasses
			         .Where(x => x.ArenaEventTypeSideId == side.Id)
			         .ToList())
		{
			context.ArenaEventTypeSideAllowedClasses.Remove(existing);
		}

		foreach (ArenaCombatantClass combatantClass in allowedClasses.DistinctBy(x => x.Id))
		{
			context.ArenaEventTypeSideAllowedClasses.Add(new ArenaEventTypeSideAllowedClass
			{
				ArenaEventTypeSideId = side.Id,
				ArenaCombatantClassId = combatantClass.Id
			});
		}
	}
}
