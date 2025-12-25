#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MudSharp.Arenas;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public class ArenaSeeder : IDatabaseSeeder
{
	private const string DefaultArenaName = "Grand Coliseum";
	private const string EligibilityProgName = "ArenaAlwaysEligible";
	private const string OutfitProgName = "ArenaDefaultOutfit";
	private const string IntroProgName = "ArenaStandardIntro";

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
		using var transaction = context.Database.BeginTransaction();
		try
		{
			var zone = ResolveEconomicZone(context, questionAnswers);
			var arenaName = ResolveArenaName(questionAnswers);
			var now = DateTime.UtcNow;

			var eligibilityProg = EnsureProg(context,
				EligibilityProgName,
				"Arena",
				"Eligibility",
				ProgVariableTypes.Boolean,
				"Default eligibility prog that accepts all characters.",
				"return true;",
				(ProgVariableTypes.Character, "character"));

			var outfitProg = EnsureProg(context,
				OutfitProgName,
				"Arena",
				"Loadout",
				ProgVariableTypes.Void,
				"Placeholder outfit prog for bring-your-own events.",
				"return;",
				(ProgVariableTypes.Anything, "event"),
				(ProgVariableTypes.Number, "side"),
				(ProgVariableTypes.Character | ProgVariableTypes.Collection, "participants"));

			var introProg = EnsureProg(context,
				IntroProgName,
				"Arena",
				"Lifecycle",
				ProgVariableTypes.Void,
				"Placeholder intro prog that performs no announcements.",
				"return;",
				(ProgVariableTypes.Anything, "event"));

			context.SaveChanges();

			var arena = new Arena
			{
				Name = arenaName,
				EconomicZoneId = zone.Id,
				CurrencyId = zone.CurrencyId,
				BankAccountId = null,
				VirtualBalance = 0.0m,
				SignupEcho = "@ sign|signs up for the upcoming bout.",
				CreatedAt = now,
				IsDeleted = false
			};
			context.Arenas.Add(arena);
			context.SaveChanges();

			var combatantClasses = new List<ArenaCombatantClass>
			{
				new()
				{
					ArenaId = arena.Id,
					Name = "Gladiator",
					Description = "A well-rounded combatant suitable for the seeded arena formats.",
					EligibilityProgId = eligibilityProg.Id,
					ResurrectNpcOnDeath = true
				},
				new()
				{
					ArenaId = arena.Id,
					Name = "Pit Fighter",
					Description = "An aggressive archetype for team skirmishes or showcase bouts.",
					EligibilityProgId = eligibilityProg.Id,
					ResurrectNpcOnDeath = true
				}
			};
			context.ArenaCombatantClasses.AddRange(combatantClasses);
			context.SaveChanges();

			var duelEvent = new ArenaEventType
			{
				ArenaId = arena.Id,
				Name = "Duel",
				BringYourOwn = true,
				RegistrationDurationSeconds = 600,
				PreparationDurationSeconds = 120,
				TimeLimitSeconds = 900,
				BettingModel = (int)BettingModel.FixedOdds,
				AppearanceFee = 0.0m,
				VictoryFee = 0.0m,
				IntroProgId = introProg.Id
			};

			var skirmishEvent = new ArenaEventType
			{
				ArenaId = arena.Id,
				Name = "Team Skirmish",
				BringYourOwn = true,
				RegistrationDurationSeconds = 900,
				PreparationDurationSeconds = 180,
				TimeLimitSeconds = 1200,
				BettingModel = (int)BettingModel.PariMutuel,
				AppearanceFee = 0.0m,
				VictoryFee = 0.0m,
				IntroProgId = introProg.Id
			};

			context.ArenaEventTypes.AddRange(duelEvent, skirmishEvent);
			context.SaveChanges();

			var duelSides = new List<ArenaEventTypeSide>
			{
				new()
				{
					ArenaEventTypeId = duelEvent.Id,
					Index = 0,
					Capacity = 1,
					Policy = (int)ArenaSidePolicy.Open,
					AllowNpcSignup = true,
					AutoFillNpc = false,
					OutfitProgId = outfitProg.Id
				},
				new()
				{
					ArenaEventTypeId = duelEvent.Id,
					Index = 1,
					Capacity = 1,
					Policy = (int)ArenaSidePolicy.Open,
					AllowNpcSignup = true,
					AutoFillNpc = false,
					OutfitProgId = outfitProg.Id
				}
			};

			var skirmishSides = new List<ArenaEventTypeSide>
			{
				new()
				{
					ArenaEventTypeId = skirmishEvent.Id,
					Index = 0,
					Capacity = 2,
					Policy = (int)ArenaSidePolicy.Open,
					AllowNpcSignup = true,
					AutoFillNpc = false,
					OutfitProgId = outfitProg.Id
				},
				new()
				{
					ArenaEventTypeId = skirmishEvent.Id,
					Index = 1,
					Capacity = 2,
					Policy = (int)ArenaSidePolicy.Open,
					AllowNpcSignup = true,
					AutoFillNpc = false,
					OutfitProgId = outfitProg.Id
				}
			};

			context.ArenaEventTypeSides.AddRange(duelSides);
			context.ArenaEventTypeSides.AddRange(skirmishSides);
			context.SaveChanges();

			foreach (var side in duelSides.Concat(skirmishSides))
			{
				foreach (var cls in combatantClasses)
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

			var sb = new StringBuilder();
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

		if (context.Arenas.Any(x => x.Name == DefaultArenaName))
		{
			return ShouldSeedResult.MayAlreadyBeInstalled;
		}

		return ShouldSeedResult.ReadyToInstall;
	}

	private static EconomicZone ResolveEconomicZone(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (questionAnswers.TryGetValue("arena-zone", out var zoneAnswer) && !string.IsNullOrWhiteSpace(zoneAnswer) &&
		long.TryParse(zoneAnswer, out var zoneId))
		{
			return context.EconomicZones.Include(x => x.Currency).First(x => x.Id == zoneId);
		}

		return context.EconomicZones.Include(x => x.Currency).First();
	}

	private static string ResolveArenaName(IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (!questionAnswers.TryGetValue("arena-name", out var name) || string.IsNullOrWhiteSpace(name))
		{
			return DefaultArenaName;
		}

		return name.Trim();
	}

	private static (bool Success, string error) ValidateZone(string answer, FuturemudDatabaseContext context)
	{
		if (!long.TryParse(answer, out var zoneId))
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
		var zones = context.EconomicZones.OrderBy(x => x.Name).Select(x => $"{x.Id}: {x.Name}").ToList();
		if (!zones.Any())
		{
			return $"{prefix} No economic zones are defined.";
		}

		var sb = new StringBuilder();
		sb.AppendLine(prefix);
		sb.AppendLine("Available economic zones:");
		foreach (var line in zones)
		{
			sb.AppendLine($"  {line}");
		}

		return sb.ToString();
	}

	private static FutureProg EnsureProg(FuturemudDatabaseContext context, string functionName, string category,
		string subcategory, ProgVariableTypes returnType, string comment, string text,
		params (ProgVariableTypes Type, string Name)[] parameters)
	{
		var prog = context.FutureProgs.FirstOrDefault(x => x.FunctionName == functionName);
		if (prog != null)
		{
			return prog;
		}

		prog = new FutureProg
		{
			FunctionName = functionName,
			FunctionComment = comment,
			FunctionText = text,
			ReturnType = (long)returnType,
			Category = category,
			Subcategory = subcategory,
			Public = false,
			AcceptsAnyParameters = false,
			StaticType = (int)FutureProgStaticType.NotStatic
		};

		for (var index = 0; index < parameters.Length; index++)
		{
			var (type, name) = parameters[index];
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				ParameterIndex = index,
				ParameterType = (long)type,
				ParameterName = name,
				FutureProg = prog
			});
		}

		context.FutureProgs.Add(prog);
		return prog;
	}
}
