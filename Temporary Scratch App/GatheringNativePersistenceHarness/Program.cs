using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.Form.Shape;
using MudSharp.Form.Material;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Health.Strategies;
using MudSharp.Health.Wounds;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Gathering;
using MudSharp.Magic.Resources;
using MudSharp.PerceptionEngine.Handlers;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Movement;
using MySql.Data.MySqlClient;
using Db = MudSharp.Models;
using RuntimeBody = MudSharp.Body.Implementations.Body;
using RuntimeCharacter = MudSharp.Character.Character;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private const string ConnectionEnvironmentVariable = "FUTUREMUD_GATHERING_TEST_CONNECTION";
	private const string SnapshotDatabasePlaceholder = "__FUTUREMUD_DATABASE__";
	private const string OwnershipTable = "__gathering_harness_ownership";

	private static int Main(string[] args)
	{
		try
		{
			return args switch
			{
				["--probe"] => Probe(),
				["--schema"] => InspectFreshSchema(),
				["--run"] => RunAcceptanceChecks(),
				["--reader", .. string[] readerArguments] => RunReader(readerArguments),
				["--land-run"] => RunNativeOrganicAcceptanceChecks(),
				["--land-reader", .. string[] readerArguments] => RunNativeOrganicReader(readerArguments),
				["--land-action-reader", .. string[] readerArguments] => RunLandActionReader(readerArguments),
				_ => Usage()
			};
		}
		catch (MySqlException ex)
		{
			Console.Error.WriteLine($"Harness failed: MySqlException (number={ex.Number}): {ex.Message}");
			return 1;
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine($"Harness failed: {ex}");
			return 1;
		}
	}

	private static int Usage()
	{
		Console.Error.WriteLine("Usage: GatheringNativePersistenceHarness --probe|--schema|--run|--reader <scenario arguments>|--land-run|--land-reader <scenario arguments>|--land-action-reader <scenario arguments>");
		return 2;
	}

	private static int Probe()
	{
		using var database = TestDatabase.OpenServerConnection();
		Console.WriteLine($"MySQL server reachable; version={database.ServerVersion}; test connection contains no selected database.");
		return 0;
	}

	private static int InspectFreshSchema()
	{
		using var database = TestDatabase.CreateFresh();
		Console.WriteLine($"created={database.Name}");
		Console.WriteLine($"serverVersion={database.ServerVersion}");
		Console.WriteLine("snapshot=DatabaseSeeder/Assets/Database/BlankDatabaseSnapshot.sql");

		foreach (string table in new[]
		         {
			         "characters", "bodies", "wounds", "characters_magicresources", "magicresources",
			         "magiccapabilities", "healthstrategies", "races", "ethnicities", "cells", "cultures", "calendars"
		         })
		{
			Console.WriteLine($"requiredColumns[{table}]={string.Join(',', database.RequiredColumns(table))}");
			Console.WriteLine($"foreignKeys[{table}]={string.Join(',', database.ForeignKeys(table))}");
		}

		Console.WriteLine("schemaInspection=passed");
		return 0;
	}

	private static int RunAcceptanceChecks()
	{
		using var database = TestDatabase.CreateFresh();
		ConfigureNativeDatabase(database.ConnectionString);
		Console.WriteLine($"created={database.Name}");
		Console.WriteLine($"serverVersion={database.ServerVersion}");

		ScenarioResult p01 = ExecuteScenario(database, "p01", existingWound: false);
		ScenarioResult p02 = ExecuteScenario(database, "p02", existingWound: true);
		Console.WriteLine($"GC-P01=passed operation:{p01.OperationId} wound:{p01.WoundId} expectedMana:{p01.ExpectedMana:F2} observedMana:{p01.ObservedMana:F2}");
		Console.WriteLine($"GC-P02=passed operation:{p02.OperationId} wound:{p02.WoundId} expectedDamage:{p02.ExpectedDamage:F2} observedDamage:{p02.ObservedDamage:F2}");
		Console.WriteLine("GC-P03=passed separate-reader-process-reconstructed-character-body-and-receipt");
		return 0;
	}

	private static int RunReader(string[] arguments)
	{
		if (arguments.Length != 16 || !Guid.TryParse(arguments[11], out Guid operationId))
		{
			throw new ArgumentException("The reader requires the owned database, fixture IDs, expected wound channels and operation ID.");
		}

		var fixture = new FixtureIds(
			long.Parse(arguments[2]), long.Parse(arguments[3]), long.Parse(arguments[4]), long.Parse(arguments[5]),
			long.Parse(arguments[6]), long.Parse(arguments[7]), long.Parse(arguments[8]),
			long.TryParse(arguments[9], out long existingWound) && existingWound > 0 ? existingWound : null,
			long.Parse(arguments[10]));
		double expectedMana = double.Parse(arguments[12], System.Globalization.CultureInfo.InvariantCulture);
		double expectedDamage = double.Parse(arguments[13], System.Globalization.CultureInfo.InvariantCulture);
		double expectedPain = double.Parse(arguments[14], System.Globalization.CultureInfo.InvariantCulture);
		double expectedStun = double.Parse(arguments[15], System.Globalization.CultureInfo.InvariantCulture);

		using TestDatabase database = TestDatabase.OpenExistingOwned(arguments[0]);
		ConfigureNativeDatabase(database.ConnectionString);
		NativeRuntime runtime = NativeRuntime.Load(fixture, database.ConnectionString);
		long woundId = fixture.ExistingWoundId ?? runtime.Actor.Wounds.Single().Id;
		SimpleOrganicWound wound = runtime.Actor.Wounds.OfType<SimpleOrganicWound>().Single(x => x.Id == woundId);
		Require(Same(runtime.Actor.MagicResourceAmounts[runtime.Resource], expectedMana),
			"The reconstructed character did not retain the gathered magic resource credit.");
		Require(Same(wound.CurrentDamage, expectedDamage) && Same(wound.CurrentPain, expectedPain) &&
			Same(wound.CurrentStun, expectedStun),
			"The reconstructed body did not retain every gathering wound channel.");
		MagicGatheringReceipt? receipt = new MagicGatheringReceiptStore().Operation(operationId);
		Require(receipt is { Status: "Completed", BodilyCostApplied: true, DestinationCredited: true,
			AccountingPersisted: true, NotificationCompleted: true },
			"The reconstructed native receipt store did not retain the completed benefit-and-cost receipt.");
		Console.WriteLine($"reader={arguments[1]} passed operation:{operationId} character:{fixture.CharacterId} body:{fixture.BodyId} wound:{woundId}");
		return 0;
	}

	private static ScenarioResult ExecuteScenario(TestDatabase database, string scenario, bool existingWound)
	{
		Console.WriteLine($"scenario[{scenario}]=seeding-fixture");
		FixtureIds fixture = FixtureSeed.Create(database, scenario, existingWound);
		WoundChannels? initialExistingWound = null;
		if (existingWound)
		{
			initialExistingWound = IndependentObservation.ReadWoundChannels(database.ConnectionString,
				fixture.ExistingWoundId!.Value);
			Require(Same(initialExistingWound.Damage, 7.0) && Same(initialExistingWound.Pain, 11.0) &&
				Same(initialExistingWound.Stun, 13.0),
				"GC-P02 did not start from the expected persisted wound channels.");
			Console.WriteLine($"scenario[{scenario}]=initial-existing-wound:{fixture.ExistingWoundId}:damage:{initialExistingWound.Damage:F2}:pain:{initialExistingWound.Pain:F2}:stun:{initialExistingWound.Stun:F2}");
		}
		Console.WriteLine($"scenario[{scenario}]=loading-native-runtime");
		NativeRuntime runtime = NativeRuntime.Load(fixture, database.ConnectionString);
		Console.WriteLine($"scenario[{scenario}]=health-strategy:{runtime.Actor.HealthStrategy.GetType().Name}:channels:{runtime.Actor.HealthStrategy.SupportedDirectHealthCostChannels}");
		if (existingWound)
		{
			SimpleOrganicWound loadedExisting = runtime.Actor.Wounds.OfType<SimpleOrganicWound>()
				.Single(x => x.Id == fixture.ExistingWoundId);
			IBodypart plannedBodypart = runtime.Body.Bodyparts.Single();
			Require(loadedExisting.DamageType == DamageType.Cellular &&
				ReferenceEquals(loadedExisting.Bodypart, plannedBodypart),
				"GC-P02 fixture did not reconstruct the persisted cellular wound on the available body part.");
		}
		var clock = new HarnessClock();
		var service = new MagicGatheringService(runtime.World, clock: clock);
		MagicGatheringResult begun = service.Begin(runtime.Actor, runtime.Capability, "draw", 2.0);
		Require(begun.Success && begun.OperationId.HasValue, $"{scenario} did not begin: {begun.Message}");
		Guid operationId = begun.OperationId ?? throw new InvalidOperationException($"{scenario} did not receive an operation ID.");
		if (!existingWound)
		{
			Require(runtime.Capability.GatheringMethods.Single().OnGatheredProgId == 0,
				"GC-P01 must run with no OnGathered callback.");
		}
		Console.WriteLine($"scenario[{scenario}]=begun-service-operation:{operationId}");
		clock.Advance(TimeSpan.FromSeconds(1));
		MagicGatheringResult completed = service.Complete(runtime.Actor, operationId);
		if (!completed.Success)
		{
			MagicGatheringReceipt? failedReceipt = new MagicGatheringReceiptStore().Operation(operationId);
			throw new InvalidOperationException($"{scenario} did not complete: {completed.Message} Receipt: {failedReceipt?.Status}; {failedReceipt?.Diagnostic}");
		}
		Console.WriteLine($"scenario[{scenario}]=completed-service-operation:{operationId}");

		long woundId = existingWound
			? fixture.ExistingWoundId!.Value
			: runtime.Actor.Wounds.OfType<SimpleOrganicWound>().Single().Id;
		double expectedMana = 2.0;
		double expectedDamage = (initialExistingWound?.Damage ?? 0.0) + 3.0;
		double expectedPain = (initialExistingWound?.Pain ?? 0.0) + 4.0;
		double expectedStun = (initialExistingWound?.Stun ?? 0.0) + 5.0;
		IndependentObservation observation = IndependentObservation.Read(database.ConnectionString, fixture, operationId, woundId);
		Require(Same(observation.Mana, expectedMana), $"{scenario} independent context did not observe the expected mana credit.");
		Require(Same(observation.Damage, expectedDamage) && Same(observation.Pain, expectedPain) && Same(observation.Stun, expectedStun),
			$"{scenario} independent context did not observe the expected persisted wound channels. " +
			$"Expected damage/pain/stun {expectedDamage:F2}/{expectedPain:F2}/{expectedStun:F2}; observed " +
			$"{observation.Damage:F2}/{observation.Pain:F2}/{observation.Stun:F2}.");
		Require(observation.ReceiptCompleted, $"{scenario} independent context did not observe a completed persisted receipt.");
		Require(observation.BodyWoundCount == 1,
			$"{scenario} observed an unexpected number of body wound rows.");

		RunReaderProcess(database.Name, scenario, fixture, operationId, expectedMana, expectedDamage, expectedPain, expectedStun);
		Console.WriteLine($"fixture[{scenario}]=character:{fixture.CharacterId},body:{fixture.BodyId},resource:{fixture.ResourceId},capability:{fixture.CapabilityId},wound:{woundId}");
		Console.WriteLine($"expected[{scenario}]=mana:{expectedMana:F2},damage:{expectedDamage:F2},pain:{expectedPain:F2},stun:{expectedStun:F2}");
		Console.WriteLine($"observed[{scenario}]=mana:{observation.Mana:F2},damage:{observation.Damage:F2},pain:{observation.Pain:F2},stun:{observation.Stun:F2},bodyWounds:{observation.BodyWoundCount},receipt:{observation.ReceiptStatus},receiptFullyPersisted:{observation.ReceiptCompleted}");
		return new ScenarioResult(operationId, woundId, expectedMana, observation.Mana, expectedDamage, observation.Damage);
	}

	private static void RunReaderProcess(string databaseName, string scenario, FixtureIds fixture, Guid operationId,
		double expectedMana, double expectedDamage, double expectedPain, double expectedStun)
	{
		string executable = Assembly.GetExecutingAssembly().Location;
		string host = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet";
		string arguments = string.Join(' ',
			$"\"{executable}\"", "--reader", databaseName, scenario, fixture.CharacterId, fixture.BodyId, fixture.CellId,
			fixture.ResourceId, fixture.CapabilityId, fixture.HealthStrategyId, fixture.TraitExpressionId,
			fixture.ExistingWoundId?.ToString() ?? "0", fixture.BodypartId, operationId,
			expectedMana.ToString(System.Globalization.CultureInfo.InvariantCulture),
			expectedDamage.ToString(System.Globalization.CultureInfo.InvariantCulture),
			expectedPain.ToString(System.Globalization.CultureInfo.InvariantCulture),
			expectedStun.ToString(System.Globalization.CultureInfo.InvariantCulture));
		using Process reader = Process.Start(new ProcessStartInfo(host, arguments)
		{
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			CreateNoWindow = true
		}) ?? throw new InvalidOperationException("The separate native reader process could not be started.");
		string output = reader.StandardOutput.ReadToEnd();
		string error = reader.StandardError.ReadToEnd();
		reader.WaitForExit();
		if (reader.ExitCode != 0)
		{
			throw new InvalidOperationException($"The separate native reader process failed for {scenario}: {error} {output}".Trim());
		}

		Console.Write(output);
	}

	private static bool Same(double left, double right) => Math.Abs(left - right) <= 0.000001;

	private static void Require(bool condition, string message)
	{
		if (!condition)
		{
			throw new InvalidOperationException(message);
		}
	}

	private sealed record ScenarioResult(Guid OperationId, long WoundId, double ExpectedMana, double ObservedMana,
		double ExpectedDamage, double ObservedDamage);

	private sealed record IndependentObservation(double Mana, double Damage, double Pain, double Stun, int BodyWoundCount,
		string ReceiptStatus, bool ReceiptCompleted)
	{
		public static WoundChannels ReadWoundChannels(string connectionString, long woundId)
		{
			using FuturemudDatabaseContext context = NewIndependentContext(connectionString);
			Db.Wound wound = context.Wounds.AsNoTracking().Single(x => x.Id == woundId);
			return new WoundChannels(wound.CurrentDamage, wound.CurrentPain, wound.CurrentStun);
		}

		public static IndependentObservation Read(string connectionString, FixtureIds fixture, Guid operationId, long woundId)
		{
			using FuturemudDatabaseContext context = NewIndependentContext(connectionString);
			double mana = context.CharactersMagicResources.AsNoTracking()
				.Single(x => x.CharacterId == fixture.CharacterId && x.MagicResourceId == fixture.ResourceId).Amount;
			Db.Wound wound = context.Wounds.AsNoTracking().Single(x => x.Id == woundId);
			Db.MagicGatheringOperation receipt = context.MagicGatheringOperations.AsNoTracking().Single(x => x.Id == operationId);
			int woundCount = context.Wounds.AsNoTracking().Count(x => x.BodyId == fixture.BodyId);
			return new IndependentObservation(mana, wound.CurrentDamage, wound.CurrentPain, wound.CurrentStun, woundCount,
				receipt.Status, receipt.Status == "Completed" && receipt.BodilyCostApplied && receipt.DestinationCredited &&
				receipt.AccountingPersisted && receipt.NotificationCompleted);
		}
	}

	private sealed record WoundChannels(double Damage, double Pain, double Stun);

	private static FuturemudDatabaseContext NewIndependentContext(string connectionString)
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static void ConfigureNativeDatabase(string connectionString)
	{
		FMDB.ConnectionString = connectionString;
		FMDB.Provider = "mysql";
	}

	private sealed record FixtureIds(
		long CharacterId,
		long BodyId,
		long CellId,
		long ResourceId,
		long CapabilityId,
		long HealthStrategyId,
		long TraitExpressionId,
		long? ExistingWoundId,
		long BodypartId);

	/// <summary>
	/// Inserts only the baseline, schema-valid entities needed to exercise the production gathering path.
	/// In particular, it never inserts a gathering receipt, resource credit, or action-produced wound cost.
	/// </summary>
	private static class FixtureSeed
	{
		private const long RuntimeBodypartId = 900001;

		public static FixtureIds Create(TestDatabase database, string scenario, bool existingWound)
		{
			using MySqlConnection connection = database.OpenOwnedConnection();
			long traitExpressionId = Insert(connection, "traitexpression", ("Name", $"{scenario} fixed hundred"),
				("Expression", "100"));
			long futureProgId = Insert(connection, "futureprogs", ("FunctionName", $"{scenario}_availability"),
				("FunctionComment", "Harness-only culture availability fixture."), ("FunctionText", "return 1;"),
				("Category", "Harness"), ("Subcategory", "Gathering"), ("Public", false),
				("AcceptsAnyParameters", false), ("StaticType", 0), ("ReturnTypeDefinition", ""));
			long healthStrategyId = Insert(connection, "healthstrategies", ("Name", $"{scenario} simple living"),
				("Type", "SimpleLiving"), ("Definition", SimpleLivingDefinition(traitExpressionId)));
			long corpseModelId = Insert(connection, "corpsemodels", ("Name", $"{scenario} corpse"),
				("Description", "Harness-only corpse model."), ("Definition", "<Definition />"), ("Type", "simple"));
			long wearableSizeRuleId = Insert(connection, "wearablesizeparameterrule", ("MinHeightFactor", 0.0),
				("MaxHeightFactor", 10.0), ("MinWeightFactor", 0.0), ("MaxWeightFactor", 10.0),
				("BodyProtoId", 0L), ("IgnoreTrait", true));
			long bodyPrototypeId = Insert(connection, "bodyprotos", ("Name", $"{scenario} body prototype"),
				("WearSizeParameterId", wearableSizeRuleId), ("ConsiderString", "a harness body"));
			long raceId = Insert(connection, "races", ("Name", $"{scenario} race"),
				("Description", "Harness-only race."), ("BaseBodyId", bodyPrototypeId), ("AllowedGenders", "0 1 2 3"),
				("AttributeTotalCap", 100), ("IndividualAttributeCap", 100), ("DiceExpression", "100"),
				("CorpseModelId", corpseModelId), ("DefaultHealthStrategyId", healthStrategyId),
				("NaturalArmourQuality", 0L), ("MaximumDragWeightExpression", "100"),
				("MaximumLiftWeightExpression", "100"), ("TemperatureRangeFloor", -100.0),
				("BodypartSizeModifier", 0), ("ChildAge", 1), ("YouthAge", 2), ("YoungAdultAge", 3),
				("AdultAge", 4), ("ElderAge", 80), ("VenerableAge", 100));
			long ethnicityId = Insert(connection, "ethnicities", ("Name", $"{scenario} ethnicity"),
				("ChargenBlurb", "Harness-only ethnicity."), ("ParentRaceId", raceId),
				("TolerableTemperatureFloorEffect", 0.0), ("TolerableTemperatureCeilingEffect", 0.0));
			long calendarId = Insert(connection, "calendars", ("Definition", "<Definition />"),
				("Date", "1/1/2000"), ("FeedClockId", 0L));
			long cultureId = Insert(connection, "cultures", ("Name", $"{scenario} culture"),
				("Description", "Harness-only culture."), ("PersonWordIndeterminate", "person"),
				("PrimaryCalendarId", calendarId), ("SkillStartingValueProgId", futureProgId),
				("TolerableTemperatureFloorEffect", 0.0), ("TolerableTemperatureCeilingEffect", 0.0));
			long skyTemplateId = Insert(connection, "skydescriptiontemplates", ("Name", $"{scenario} sky"));
			long shardId = Insert(connection, "shards", ("Name", $"{scenario} shard"),
				("MinimumTerrestrialLux", 0.0), ("SkyDescriptionTemplateId", skyTemplateId));
			long zoneId = Insert(connection, "zones", ("Name", $"{scenario} zone"), ("ShardId", shardId),
				("Latitude", 0.0), ("Longitude", 0.0), ("Elevation", 0.0), ("AmbientLightPollution", 0.0));
			long roomId = Insert(connection, "rooms", ("ZoneId", zoneId), ("X", 0), ("Y", 0), ("Z", 0));
			long cellId = Insert(connection, "cells", ("RoomId", roomId), ("EffectData", "<Effects />"));
			long bodyId = Insert(connection, "bodies", ("BodyPrototypeID", bodyPrototypeId), ("Height", 1.8),
				("Weight", 80.0), ("Position", 1L), ("RaceId", raceId), ("CurrentStamina", 100.0),
				("CurrentBloodVolume", 5.0), ("EthnicityId", ethnicityId), ("Gender", (short)Gender.Male),
				("HeldBreathLength", 120), ("EffectData", "<Effects />"), ("HealthStrategyId", healthStrategyId));
			long characterId = Insert(connection, "characters", ("Name", $"{scenario} actor"),
				("CreationTime", DateTime.UtcNow), ("Status", 0), ("State", (int)CharacterState.Awake),
				("Gender", (short)Gender.Male), ("Location", cellId), ("BodyId", bodyId), ("CultureId", cultureId),
				("EffectData", "<Effects />"), ("BirthdayDate", "1/1/2000"), ("BirthdayCalendarId", calendarId),
				("TotalMinutesPlayed", 0), ("AlcoholLitres", 0.0), ("WaterLitres", 0.0),
				("FoodSatiatedHours", 0.0), ("DrinkSatiatedHours", 0.0), ("PreferredDefenseType", 0),
				("PositionModifier", 0), ("CurrentProjectHours", 0.0), ("RoomLayer", 0));
			long resourceId = Insert(connection, "magicresources", ("Name", $"{scenario} mana"),
				("ShortName", "mana"), ("Type", "simple"), ("MagicResourceType", (int)MagicResourceType.PlayerResource),
				("Definition", SimpleResourceDefinition()));
			Execute(connection, "INSERT INTO `characters_magicresources` (`CharacterId`,`MagicResourceId`,`Amount`) VALUES (@characterId,@resourceId,@amount);",
				("@characterId", characterId), ("@resourceId", resourceId), ("@amount", 0.0));
			long schoolId = Insert(connection, "magicschools", ("Name", $"{scenario} school"), ("SchoolVerb", "draw"),
				("SchoolAdjective", "harness"), ("PowerListColour", ""));
			long capabilityId = Insert(connection, "magiccapabilities", ("Name", $"{scenario} capability"),
				("CapabilityModel", "skilllevel"), ("PowerLevel", 1), ("MagicSchoolId", schoolId),
				("Definition", CapabilityDefinition(resourceId)));

			long? woundId = null;
			if (existingWound)
			{
				woundId = Insert(connection, "wounds", ("BodyId", bodyId), ("OriginalDamage", 7.0),
					("CurrentDamage", 7.0), ("CurrentPain", 11.0), ("CurrentShock", 0.0), ("CurrentStun", 13.0),
					("DamageType", (int)DamageType.Cellular), ("Internal", false), ("BodypartProtoId", RuntimeBodypartId),
					("ExtraInformation", WoundExtras()), ("ActorOriginId", characterId), ("WoundType", "SimpleOrganic"),
					("RealTimeOfWound", DateTime.UtcNow));
			}

			return new FixtureIds(characterId, bodyId, cellId, resourceId, capabilityId, healthStrategyId,
				traitExpressionId, woundId, RuntimeBodypartId);
		}

		private static long Insert(MySqlConnection connection, string table, params (string Column, object? Value)[] values)
		{
			using MySqlCommand insert = connection.CreateCommand();
			string columns = string.Join(',', values.Select(x => $"`{x.Column}`"));
			string parameters = string.Join(',', values.Select((_, index) => $"@p{index}"));
			insert.CommandText = $"INSERT INTO `{table}` ({columns}) VALUES ({parameters});";
			for (int index = 0; index < values.Length; index++)
			{
				insert.Parameters.AddWithValue($"@p{index}", values[index].Value ?? DBNull.Value);
			}

			insert.ExecuteNonQuery();
			insert.Parameters.Clear();
			insert.CommandText = "SELECT LAST_INSERT_ID();";
			return Convert.ToInt64(insert.ExecuteScalar());
		}

		private static void Execute(MySqlConnection connection, string sql, params (string Name, object? Value)[] values)
		{
			using MySqlCommand command = connection.CreateCommand();
			command.CommandText = sql;
			foreach ((string name, object? value) in values)
			{
				command.Parameters.AddWithValue(name, value ?? DBNull.Value);
			}

			command.ExecuteNonQuery();
		}

		private static string SimpleLivingDefinition(long traitExpressionId) => new XElement("Definition",
			new XElement("MaximumHitPointsExpression", traitExpressionId),
			new XElement("MaximumStunExpression", traitExpressionId),
			new XElement("MaximumPainExpression", traitExpressionId),
			new XElement("HealingTickDamageExpression", traitExpressionId),
			new XElement("HealingTickStunExpression", traitExpressionId),
			new XElement("HealingTickPainExpression", traitExpressionId)).ToString();

		private static string SimpleResourceDefinition() => new XElement("Definition",
			new XElement("StartingResourceAmountLocationProg", 0),
			new XElement("StartingResourceAmountItemProg", 0),
			new XElement("StartingResourceAmountCharacterProg", 0),
			new XElement("ShouldStartWithResourceCharacterProg", 0),
			new XElement("ShouldStartWithResourceLocationProg", 0),
			new XElement("ShouldStartWithResourceItemProg", 0),
			new XElement("ResourceCapProg", 0)).ToString();

		private static string CapabilityDefinition(long resourceId) => new XElement("Definition",
			new XElement("ConcentrationTrait", 1),
			new XElement("ConcentrationCapabilityExpression", "100"),
			new XElement("ConcentrationDifficultyExpression", "5"),
			new XElement("Regenerators"),
			new XElement("Gathering", new XAttribute("version", 1),
				new XElement("Method", new XAttribute("key", Guid.NewGuid()), new XAttribute("alias", "draw"),
					new XAttribute("name", "Harness Draw"), new XAttribute("kind", MagicGatheringMethodKind.Self),
					new XAttribute("destination", resourceId), new XAttribute("source", 0), new XAttribute("min", 2.0),
					new XAttribute("max", 2.0), new XAttribute("duration", 1.0), new XAttribute("ratio", 1.0),
					new XAttribute("stamina", 0.0), new XAttribute("minimumStamina", 0.0),
					new XAttribute("damage", 3.0), new XAttribute("pain", 4.0), new XAttribute("stun", 5.0),
					new XAttribute("maximumHealthSeverity", WoundSeverity.Horrifying), new XAttribute("permission", 0),
					new XAttribute("durationProg", 0), new XAttribute("staminaProg", 0), new XAttribute("damageProg", 0),
					new XAttribute("painProg", 0), new XAttribute("stunProg", 0), new XAttribute("onGathered", 0),
					new XAttribute("structuralVersion", 1)))).ToString();

		private static string WoundExtras() => new XElement("Definition",
			new XElement("DamageDescription", "Tissue Death"), new XElement("Cleaned", false),
			new XElement("CleanAttempted", false), new XElement("AntisepticTreated", false),
			new XElement("BleedStatus", 0), new XElement("Tended", 0), new XElement("HadInfection", false),
			new XElement("ScarSurgicalProcedureType", -1), new XElement("ScarSurgeryCheckDegrees", 0),
			new XElement("TreatmentAttempts", 0), new XElement("IsFriendlyWound", false)).ToString();
	}

	/// <summary>
	/// Reconstructs the minimum live object graph required by the native service. Persisted domain models,
	/// the real Body, SimpleLivingHealthStrategy, SimpleOrganicWound and receipt store remain production types;
	/// only unrelated world catalogues are deliberately empty or narrow.
	/// </summary>
	private sealed class NativeRuntime
	{
		private NativeRuntime(Mock<IFuturemud> world, NativeHarnessCharacter actor, RuntimeBody body,
			IMagicResource resource, IMagicGatheringCapability capability)
		{
			WorldMock = world;
			Actor = actor;
			Body = body;
			Resource = resource;
			Capability = capability;
		}

		public Mock<IFuturemud> WorldMock { get; }
		public IFuturemud World => WorldMock.Object;
		public NativeHarnessCharacter Actor { get; }
		public RuntimeBody Body { get; }
		public IMagicResource Resource { get; }
		public IMagicGatheringCapability Capability { get; }

		public static NativeRuntime Load(FixtureIds fixture, string connectionString)
		{
			using FuturemudDatabaseContext context = NewIndependentContext(connectionString);
			Db.Character character = context.Characters
				.Include(x => x.CharactersMagicResources)
				.Single(x => x.Id == fixture.CharacterId);
			Db.Body bodyModel = context.Bodies
				.Include(x => x.Wounds)
				.ThenInclude(x => x.Infections)
				.Single(x => x.Id == fixture.BodyId);
			Db.HealthStrategy healthStrategyModel = context.HealthStrategies.Single(x => x.Id == fixture.HealthStrategyId);
			Db.MagicResource resourceModel = context.MagicResources.Single(x => x.Id == fixture.ResourceId);
			Db.MagicCapability capabilityModel = context.MagicCapabilities.Single(x => x.Id == fixture.CapabilityId);
			var world = new Mock<IFuturemud>(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };
			var saveManager = new SaveManager();
			var scheduler = new Mock<IEffectScheduler>(MockBehavior.Loose);
			var heartbeat = new Mock<IHeartbeatManager>(MockBehavior.Loose);
			world.SetupGet(x => x.SaveManager).Returns(saveManager);
			world.SetupGet(x => x.EffectScheduler).Returns(scheduler.Object);
			world.SetupGet(x => x.HeartbeatManager).Returns(heartbeat.Object);
			world.Setup(x => x.GetStaticBool(It.IsAny<string>())).Returns(false);
			world.Setup(x => x.GetStaticDouble(It.IsAny<string>())).Returns(0.0);
			world.Setup(x => x.GetStaticConfiguration(It.IsAny<string>())).Returns("0");

			Mock<ITraitExpression> expression = NewTraitExpression(fixture.TraitExpressionId, world.Object);
			var expressions = new All<ITraitExpression>();
			expressions.Add(expression.Object);
			world.SetupGet(x => x.TraitExpressions).Returns(expressions);

			Mock<IExternalBodypart> bodypart = NewBodypart(fixture.BodypartId, world.Object);
			var bodyparts = new All<IBodypart>();
			bodyparts.Add(bodypart.Object);
			world.SetupGet(x => x.BodypartPrototypes).Returns(bodyparts);
			Require(ReferenceEquals(bodyparts.Get(fixture.BodypartId), bodypart.Object),
				"The fixture did not register the supported body part in the native world catalogue.");

			BaseHealthStrategy.SetupHealthStrategies();
			IHealthStrategy healthStrategy = BaseHealthStrategy.LoadStrategy(healthStrategyModel, world.Object);
			var healthStrategies = new All<IHealthStrategy>();
			healthStrategies.Add(healthStrategy);
			world.SetupGet(x => x.HealthStrategies).Returns(healthStrategies);

			Mock<IRace> race = NewRace(bodyModel.RaceId, world.Object, healthStrategy);
			var races = new All<IRace>();
			races.Add(race.Object);
			world.SetupGet(x => x.Races).Returns(races);

			Mock<IEthnicity> ethnicity = NewEthnicity(bodyModel.EthnicityId, world.Object, race.Object);
			var ethnicities = new All<IEthnicity>();
			ethnicities.Add(ethnicity.Object);
			world.SetupGet(x => x.Ethnicities).Returns(ethnicities);

			Mock<IBodyPrototype> bodyPrototype = NewBodyPrototype(bodyModel.BodyPrototypeId, world.Object, bodypart.Object);
			var bodyPrototypes = new All<IBodyPrototype>();
			bodyPrototypes.Add(bodyPrototype.Object);
			world.SetupGet(x => x.BodyPrototypes).Returns(bodyPrototypes);

			Mock<ICulture> culture = NewCulture(character.CultureId, world.Object);
			var cultures = new All<ICulture>();
			cultures.Add(culture.Object);
			world.SetupGet(x => x.Cultures).Returns(cultures);

			Mock<ICell> cell = NewCell(fixture.CellId, world.Object);
			var cells = new All<ICell>();
			cells.Add(cell.Object);
			world.SetupGet(x => x.Cells).Returns(cells);

			Mock<ITraitDefinition> trait = NewTraitDefinition(1, world.Object);
			var traits = new All<ITraitDefinition>();
			traits.Add(trait.Object);
			world.SetupGet(x => x.Traits).Returns(traits);

			Mock<IMagicSchool> school = NewMagicSchool(capabilityModel.MagicSchoolId, world.Object);
			var schools = new All<IMagicSchool>();
			schools.Add(school.Object);
			world.SetupGet(x => x.MagicSchools).Returns(schools);

			var futureProgs = new All<IFutureProg>();
			var powers = new All<IMagicPower>();
			var regenerators = new All<IMagicResourceRegenerator>();
			world.SetupGet(x => x.FutureProgs).Returns(futureProgs);
			world.SetupGet(x => x.MagicPowers).Returns(powers);
			world.SetupGet(x => x.MagicResourceRegenerators).Returns(regenerators);
			world.SetupGet(x => x.Bloodtypes).Returns(new All<IBloodtype>());
			world.SetupGet(x => x.EntityDescriptionPatterns).Returns(new All<IEntityDescriptionPattern>());
			world.SetupGet(x => x.Merits).Returns(new All<IMerit>());
			world.SetupGet(x => x.Drugs).Returns(new All<IDrug>());

			IMagicResource resource = new CappedSimpleMagicResource(resourceModel, world.Object);
			var resources = new All<IMagicResource>();
			resources.Add(resource);
			world.SetupGet(x => x.MagicResources).Returns(resources);

			IMagicGatheringCapability capability = (IMagicGatheringCapability)MagicCapabilityFactory.LoadCapability(capabilityModel, world.Object);
			var capabilities = new All<IMagicCapability>();
			capabilities.Add(capability);
			world.SetupGet(x => x.MagicCapabilities).Returns(capabilities);

			NativeHarnessCharacter actor = NativeHarnessCharacter.Create(world.Object, character.Id, cell.Object, culture.Object);
			RuntimeBody body = new(bodyModel, world.Object, actor);
			actor.AttachBody(body);
			SetPrivateField(body, "_currentBloodVolumeLitres", 5.0);
			body.TotalBloodVolumeLitres = 5.0;
			SetPrivateField(body, "_healthTickActive", true);
			actor.LoadMagic(character);
			actor.SetMerits([NewCapabilityMerit(capability)]);

			return new NativeRuntime(world, actor, body, resource, capability);
		}

		private static Mock<ITraitExpression> NewTraitExpression(long id, IFuturemud world)
		{
			var expression = new Mock<ITraitExpression>(MockBehavior.Loose);
			expression.SetupGet(x => x.Id).Returns(id);
			expression.SetupGet(x => x.Name).Returns("Harness fixed hundred");
			expression.SetupGet(x => x.Gameworld).Returns(world);
			expression.Setup(x => x.Evaluate(It.IsAny<IHaveTraits>(), It.IsAny<ITraitDefinition>(), It.IsAny<TraitBonusContext>()))
				.Returns(100.0);
			expression.Setup(x => x.EvaluateMax(It.IsAny<IHaveTraits>())).Returns(100.0);
			return expression;
		}

		private static Mock<IExternalBodypart> NewBodypart(long id, IFuturemud world)
		{
			var bodypart = new Mock<IExternalBodypart>(MockBehavior.Loose);
			bodypart.SetupGet(x => x.Id).Returns(id);
			bodypart.SetupGet(x => x.IdHasBeenRegistered).Returns(true);
			bodypart.SetupGet(x => x.Name).Returns("harness bodypart");
			bodypart.SetupGet(x => x.Gameworld).Returns(world);
			bodypart.SetupGet(x => x.DamageModifier).Returns(1.0);
			bodypart.SetupGet(x => x.PainModifier).Returns(1.0);
			bodypart.SetupGet(x => x.StunModifier).Returns(1.0);
			bodypart.SetupGet(x => x.BleedModifier).Returns(0.0);
			bodypart.SetupGet(x => x.RelativeHitChance).Returns(1.0);
			bodypart.SetupGet(x => x.Significant).Returns(false);
			bodypart.SetupGet(x => x.Organs).Returns(Array.Empty<IOrganProto>());
			bodypart.SetupGet(x => x.Bones).Returns(Array.Empty<IBone>());
			bodypart.SetupGet(x => x.OrganInfo).Returns(new Dictionary<IOrganProto, BodypartInternalInfo>());
			bodypart.SetupGet(x => x.BoneInfo).Returns(new Dictionary<IBone, BodypartInternalInfo>());
			bodypart.Setup(x => x.PartDamageEffects(It.IsAny<IBody>(), It.IsAny<CanUseBodypartResult>())).Returns(false);
			return bodypart;
		}

		private static Mock<IBodyPrototype> NewBodyPrototype(long id, IFuturemud world, IBodypart bodypart)
		{
			var prototype = new Mock<IBodyPrototype>(MockBehavior.Loose);
			prototype.SetupGet(x => x.Id).Returns(id);
			prototype.SetupGet(x => x.Name).Returns("Harness body prototype");
			prototype.SetupGet(x => x.Gameworld).Returns(world);
			prototype.Setup(x => x.BodypartsFor(It.IsAny<IRace>(), It.IsAny<Gender>())).Returns(new[] { bodypart });
			prototype.SetupGet(x => x.AllBodyparts).Returns(new[] { bodypart });
			prototype.SetupGet(x => x.AllBodypartsBonesAndOrgans).Returns(new[] { bodypart });
			prototype.SetupGet(x => x.AllExternalBodyparts).Returns(new[] { (IExternalBodypart)bodypart });
			prototype.SetupGet(x => x.Organs).Returns(Array.Empty<IOrganProto>());
			prototype.SetupGet(x => x.Bones).Returns(Array.Empty<IBone>());
			prototype.SetupGet(x => x.Limbs).Returns(Array.Empty<ILimb>());
			prototype.SetupGet(x => x.DefaultSpeeds).Returns(new Dictionary<IPositionState, IMoveSpeed>());
			return prototype;
		}

		private static Mock<IRace> NewRace(long id, IFuturemud world, IHealthStrategy healthStrategy)
		{
			var race = new Mock<IRace>(MockBehavior.Loose);
			race.SetupGet(x => x.Id).Returns(id);
			race.SetupGet(x => x.Name).Returns("Harness race");
			race.SetupGet(x => x.Gameworld).Returns(world);
			race.SetupGet(x => x.DefaultHealthStrategy).Returns(healthStrategy);
			race.Setup(x => x.ModifiedHitpoints(It.IsAny<IBodypart>())).Returns(100.0);
			race.SetupGet(x => x.TemperatureRangeFloor).Returns(-100.0);
			race.SetupGet(x => x.TemperatureRangeCeiling).Returns(100.0);
			race.SetupGet(x => x.BloodLiquid).Returns((ILiquid)null!);
			return race;
		}

		private static Mock<IEthnicity> NewEthnicity(long id, IFuturemud world, IRace race)
		{
			var ethnicity = new Mock<IEthnicity>(MockBehavior.Loose);
			ethnicity.SetupGet(x => x.Id).Returns(id);
			ethnicity.SetupGet(x => x.Name).Returns("Harness ethnicity");
			ethnicity.SetupGet(x => x.Gameworld).Returns(world);
			ethnicity.SetupGet(x => x.ParentRace).Returns(race);
			ethnicity.SetupGet(x => x.TolerableTemperatureFloorEffect).Returns(0.0);
			ethnicity.SetupGet(x => x.TolerableTemperatureCeilingEffect).Returns(0.0);
			return ethnicity;
		}

		private static Mock<ICulture> NewCulture(long id, IFuturemud world)
		{
			var culture = new Mock<ICulture>(MockBehavior.Loose);
			culture.SetupGet(x => x.Id).Returns(id);
			culture.SetupGet(x => x.Name).Returns("Harness culture");
			culture.SetupGet(x => x.Gameworld).Returns(world);
			culture.SetupGet(x => x.TolerableTemperatureFloorEffect).Returns(0.0);
			culture.SetupGet(x => x.TolerableTemperatureCeilingEffect).Returns(0.0);
			return culture;
		}

		private static Mock<ICell> NewCell(long id, IFuturemud world)
		{
			var cell = new Mock<ICell>(MockBehavior.Loose);
			cell.SetupGet(x => x.Id).Returns(id);
			cell.SetupGet(x => x.Name).Returns("Harness cell");
			cell.SetupGet(x => x.Gameworld).Returns(world);
			cell.Setup(x => x.EventHandlersFor(It.IsAny<IPerceivable>())).Returns(Array.Empty<IHandleEvents>());
			return cell;
		}

		private static Mock<ITraitDefinition> NewTraitDefinition(long id, IFuturemud world)
		{
			var trait = new Mock<ITraitDefinition>(MockBehavior.Loose);
			trait.SetupGet(x => x.Id).Returns(id);
			trait.SetupGet(x => x.Name).Returns("Harness concentration trait");
			trait.SetupGet(x => x.Gameworld).Returns(world);
			return trait;
		}

		private static Mock<IMagicSchool> NewMagicSchool(long id, IFuturemud world)
		{
			var school = new Mock<IMagicSchool>(MockBehavior.Loose);
			school.SetupGet(x => x.Id).Returns(id);
			school.SetupGet(x => x.Name).Returns("Harness school");
			school.SetupGet(x => x.SchoolVerb).Returns("draw");
			school.SetupGet(x => x.Gameworld).Returns(world);
			return school;
		}

		private static IMerit NewCapabilityMerit(IMagicGatheringCapability capability)
		{
			var merit = new Mock<IMagicCapabilityMerit>(MockBehavior.Loose);
			merit.SetupGet(x => x.Capabilities).Returns(new[] { (IMagicCapability)capability });
			merit.Setup(x => x.Applies(It.IsAny<IHaveMerits>())).Returns(true);
			return merit.Object;
		}
	}

	private sealed class CappedSimpleMagicResource : SimpleMagicResource
	{
		public CappedSimpleMagicResource(Db.MagicResource resource, IFuturemud gameworld)
			: base(resource, gameworld)
		{
		}

		public override double ResourceCap(IHaveMagicResource thing) => 100.0;
	}

	private sealed class NativeHarnessCharacter : RuntimeCharacter
	{
		private NativeHarnessCharacter()
			: base(null!, null!)
		{
		}

		public static NativeHarnessCharacter Create(IFuturemud world, long id, ICell cell, ICulture culture)
		{
			var character = (NativeHarnessCharacter)RuntimeHelpers.GetUninitializedObject(typeof(NativeHarnessCharacter));
			SetPrivateField(character, "_id", id);
			SetPrivateField(character, "IdInitialised", true);
			SetPrivateField(character, "_noSave", true);
			SetPrivateMember(character, "Gameworld", world);
			SetPrivateMember(character, "EffectHandler", new EffectHandler(character));
			SetPrivateMember(character, "OutputHandler", new NonPlayerOutputHandler());
			SetPrivateMember(character, "Location", cell);
			SetPrivateMember(character, "Culture", culture);
			SetPrivateMember(character, "PermissionLevel", PermissionLevel.Player);
			SetPrivateField(character, "_account", NewFormattingAccount());
			SetPrivateField(character, "_state", CharacterState.Awake);
			SetPrivateField(character, "_merits", new List<IMerit>());
			SetPrivateField(character, "_magicResourceAmounts", new DoubleCounter<IMagicResource>());
			SetPrivateField(character, "_magicResourceGenerators", new List<IMagicResourceRegenerator>());
			SetPrivateField(character, "_generatorDelegateDictionary", new Dictionary<IMagicResourceRegenerator, HeartbeatManagerDelegate>());
			return character;
		}

		private static IAccount NewFormattingAccount()
		{
			var account = new Mock<IAccount>(MockBehavior.Loose);
			account.Setup(x => x.GetFormat(It.IsAny<Type>()))
				.Returns((Type formatType) => System.Globalization.CultureInfo.InvariantCulture.GetFormat(formatType));
			account.SetupGet(x => x.LineFormatLength).Returns(80);
			account.SetupGet(x => x.InnerLineFormatLength).Returns(78);
			return account.Object;
		}

		public void AttachBody(IBody body) => SetPrivateMember(this, "Body", body);

		public void SetMerits(IEnumerable<IMerit> merits) => SetPrivateField(this, "_merits", merits.ToList());

		// The isolated fixture has no installed hooks, combat, or attached items. The normal event pipeline therefore
		// has no observer work to perform after ProcessPassiveWound has run its native wound lifecycle.
		public override bool HandleEvent(EventType type, params dynamic[] arguments) => false;

		public override void Save()
		{
			Db.Character character = FMDB.Context.Characters
				.Include(x => x.CharactersMagicResources)
				.Single(x => x.Id == Id);
			if (MagicChanged || ResourcesChanged)
			{
				SaveMagic(character);
			}

			Changed = false;
		}
	}

	private sealed class HarnessClock : TimeProvider
	{
		private DateTimeOffset _utcNow = new(2026, 9, 15, 0, 0, 0, TimeSpan.Zero);
		private long _timestamp;

		public override long TimestampFrequency => TimeSpan.TicksPerSecond;
		public override DateTimeOffset GetUtcNow() => _utcNow;
		public override long GetTimestamp() => _timestamp;

		public void Advance(TimeSpan duration)
		{
			_utcNow += duration;
			_timestamp += duration.Ticks;
		}
	}

	private static void SetPrivateField(object target, string name, object? value)
	{
		for (Type? type = target.GetType(); type is not null; type = type.BaseType)
		{
			FieldInfo? field = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
			if (field is not null)
			{
				field.SetValue(target, value);
				return;
			}
		}

		throw new MissingFieldException(target.GetType().FullName, name);
	}

	private static void SetPrivateMember(object target, string propertyName, object? value)
	{
		SetPrivateField(target, $"<{propertyName}>k__BackingField", value);
	}

	private sealed class TestDatabase : IDisposable
	{
		private readonly MySqlConnectionStringBuilder _serverBuilder;
		private readonly string _ownershipToken;
		private bool _created;
		private bool _disposed;

		private TestDatabase(MySqlConnectionStringBuilder serverBuilder, string name, string ownershipToken, string serverVersion)
		{
			_serverBuilder = serverBuilder;
			Name = name;
			_ownershipToken = ownershipToken;
			ServerVersion = serverVersion;
		}

		public string Name { get; }
		public string ServerVersion { get; }
		public string ConnectionString => DatabaseConnectionString();

		public MySqlConnection OpenOwnedConnection() => OpenDatabase();

		public static TestDatabase OpenServerConnection()
		{
			MySqlConnectionStringBuilder builder = ServerConnectionBuilder();
			using var connection = new MySqlConnection(builder.ConnectionString);
			connection.Open();
			using MySqlCommand command = connection.CreateCommand();
			command.CommandText = "SELECT VERSION();";
			string version = Convert.ToString(command.ExecuteScalar()) ?? "unknown";
			return new TestDatabase(builder, string.Empty, string.Empty, version);
		}

		public static TestDatabase OpenExistingOwned(string name)
		{
			if (!HasOwnedPrefix(name))
			{
				throw new InvalidOperationException("The reader refuses to open a database that does not use this harness ownership prefix.");
			}

			MySqlConnectionStringBuilder builder = ServerConnectionBuilder();
			using var server = new MySqlConnection(builder.ConnectionString);
			server.Open();
			using MySqlCommand versionCommand = server.CreateCommand();
			versionCommand.CommandText = "SELECT VERSION();";
			string version = Convert.ToString(versionCommand.ExecuteScalar()) ?? "unknown";
			using MySqlCommand existsCommand = server.CreateCommand();
			existsCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @name;";
			existsCommand.Parameters.AddWithValue("@name", name);
			if (Convert.ToInt32(existsCommand.ExecuteScalar()) != 1)
			{
				throw new InvalidOperationException("The requested owned database no longer exists.");
			}

			var database = new TestDatabase(builder, name, string.Empty, version);
			using MySqlConnection connection = database.OpenDatabase();
			using MySqlCommand markerCommand = connection.CreateCommand();
			markerCommand.CommandText = $"SELECT COUNT(*) FROM `{OwnershipTable}`;";
			if (Convert.ToInt32(markerCommand.ExecuteScalar()) < 1)
			{
				throw new InvalidOperationException("The reader refuses a database without a harness ownership marker.");
			}

			return database;
		}

		public static TestDatabase CreateFresh(string prefix = "futuremud_gather_gc_")
		{
			if (prefix is not ("futuremud_gather_gc_" or "futuremud_land_"))
			{
				throw new InvalidOperationException("The harness refuses an unrecognised database ownership prefix.");
			}

			MySqlConnectionStringBuilder builder = ServerConnectionBuilder();
			string name = GenerateName(prefix);
			string token = Convert.ToHexString(RandomNumberGenerator.GetBytes(20)).ToLowerInvariant();
			using var server = new MySqlConnection(builder.ConnectionString);
			server.Open();
			string version;
			using (MySqlCommand versionCommand = server.CreateCommand())
			{
				versionCommand.CommandText = "SELECT VERSION();";
				version = Convert.ToString(versionCommand.ExecuteScalar()) ?? "unknown";
			}

			using (MySqlCommand existsCommand = server.CreateCommand())
			{
				existsCommand.CommandText =
					"SELECT COUNT(*) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @name;";
				existsCommand.Parameters.AddWithValue("@name", name);
				if (Convert.ToInt32(existsCommand.ExecuteScalar()) != 0)
				{
					throw new InvalidOperationException("Generated database name already exists; refusing to reuse it.");
				}
			}

			var database = new TestDatabase(builder, name, token, version);
			try
			{
				using (MySqlCommand createCommand = server.CreateCommand())
				{
					createCommand.CommandText = $"CREATE DATABASE `{EscapeIdentifier(name)}` CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;";
					createCommand.ExecuteNonQuery();
				}
				database._created = true;
				database.ImportSupportedSnapshot();
				database.WriteOwnershipMarker();
				return database;
			}
			catch
			{
				database.Dispose();
				throw;
			}
		}

		public IReadOnlyList<string> RequiredColumns(string table)
		{
			using MySqlConnection connection = OpenDatabase();
			using MySqlCommand command = connection.CreateCommand();
			command.CommandText =
				"""
				SELECT COLUMN_NAME
				FROM INFORMATION_SCHEMA.COLUMNS
				WHERE TABLE_SCHEMA = @databaseName
				  AND TABLE_NAME = @tableName
				  AND IS_NULLABLE = 'NO'
				  AND COLUMN_DEFAULT IS NULL
				  AND EXTRA NOT LIKE '%auto_increment%'
				ORDER BY ORDINAL_POSITION;
				""";
			command.Parameters.AddWithValue("@databaseName", Name);
			command.Parameters.AddWithValue("@tableName", table);
			using MySqlDataReader reader = command.ExecuteReader();
			var results = new List<string>();
			while (reader.Read())
			{
				results.Add(reader.GetString(0));
			}

			return results;
		}

		public IReadOnlyList<string> ForeignKeys(string table)
		{
			using MySqlConnection connection = OpenDatabase();
			using MySqlCommand command = connection.CreateCommand();
			command.CommandText =
				"""
				SELECT CONCAT(COLUMN_NAME, '->', REFERENCED_TABLE_NAME, '.', REFERENCED_COLUMN_NAME)
				FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
				WHERE TABLE_SCHEMA = @databaseName
				  AND TABLE_NAME = @tableName
				  AND REFERENCED_TABLE_NAME IS NOT NULL
				ORDER BY CONSTRAINT_NAME, ORDINAL_POSITION;
				""";
			command.Parameters.AddWithValue("@databaseName", Name);
			command.Parameters.AddWithValue("@tableName", table);
			using MySqlDataReader reader = command.ExecuteReader();
			var results = new List<string>();
			while (reader.Read())
			{
				results.Add(reader.GetString(0));
			}

			return results;
		}

		public void Dispose()
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;
			if (!_created)
			{
				return;
			}

			if (!HasOwnedPrefix(Name) || string.IsNullOrWhiteSpace(_ownershipToken))
			{
				throw new InvalidOperationException("Refusing to delete a database that was not generated by this harness.");
			}

			using var server = new MySqlConnection(_serverBuilder.ConnectionString);
			server.Open();
			if (!DatabaseExists(server, Name))
			{
				Console.WriteLine("cleanup=database-already-absent");
				return;
			}

			if (!OwnershipMarkerMatches())
			{
				throw new InvalidOperationException("Refusing to delete a database whose ownership marker does not match this run.");
			}

			using MySqlCommand dropCommand = server.CreateCommand();
			dropCommand.CommandText = $"DROP DATABASE `{EscapeIdentifier(Name)}`;";
			dropCommand.ExecuteNonQuery();
			Console.WriteLine("cleanup=deleted-owned-database");
		}

		private void ImportSupportedSnapshot()
		{
			string snapshot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "DatabaseSeeder", "Assets", "Database", "BlankDatabaseSnapshot.sql"));
			if (!File.Exists(snapshot))
			{
				throw new FileNotFoundException("The supported blank database snapshot was not found.", snapshot);
			}

			new DatabaseUpgradeCoordinator().ImportBlankDatabaseSnapshot(DatabaseConnectionString(), snapshot,
				SnapshotDatabasePlaceholder);
		}

		private void WriteOwnershipMarker()
		{
			using MySqlConnection connection = OpenDatabase();
			using (MySqlCommand create = connection.CreateCommand())
			{
				create.CommandText =
					$"CREATE TABLE `{OwnershipTable}` (RunToken varchar(64) NOT NULL PRIMARY KEY, CreatedUtc datetime(6) NOT NULL);";
				create.ExecuteNonQuery();
			}
			using MySqlCommand insert = connection.CreateCommand();
			insert.CommandText = $"INSERT INTO `{OwnershipTable}` (RunToken, CreatedUtc) VALUES (@token, UTC_TIMESTAMP(6));";
			insert.Parameters.AddWithValue("@token", _ownershipToken);
			insert.ExecuteNonQuery();
		}

		private bool OwnershipMarkerMatches()
		{
			try
			{
				using MySqlConnection connection = OpenDatabase();
				using MySqlCommand command = connection.CreateCommand();
				command.CommandText = $"SELECT COUNT(*) FROM `{OwnershipTable}` WHERE RunToken = @token;";
				command.Parameters.AddWithValue("@token", _ownershipToken);
				return Convert.ToInt32(command.ExecuteScalar()) == 1;
			}
			catch (MySqlException)
			{
				return false;
			}
		}

		private MySqlConnection OpenDatabase()
		{
			var connection = new MySqlConnection(DatabaseConnectionString());
			connection.Open();
			return connection;
		}

		private string DatabaseConnectionString()
		{
			var builder = new MySqlConnectionStringBuilder(_serverBuilder.ConnectionString)
			{
				Database = Name
			};
			return builder.ConnectionString;
		}

		private static bool DatabaseExists(MySqlConnection connection, string databaseName)
		{
			using MySqlCommand command = connection.CreateCommand();
			command.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @name;";
			command.Parameters.AddWithValue("@name", databaseName);
			return Convert.ToInt32(command.ExecuteScalar()) == 1;
		}

		private static MySqlConnectionStringBuilder ServerConnectionBuilder()
		{
			string? suppliedConnection = Environment.GetEnvironmentVariable(ConnectionEnvironmentVariable);
			if (string.IsNullOrWhiteSpace(suppliedConnection))
			{
				throw new InvalidOperationException($"Set {ConnectionEnvironmentVariable} to a dedicated server-level test connection before running this harness.");
			}

			return new MySqlConnectionStringBuilder(suppliedConnection)
			{
				Database = string.Empty
			};
		}

		private static string GenerateName(string prefix)
		{
			return $"{prefix}{DateTime.UtcNow:yyyyMMddHHmmss}_{Convert.ToHexString(RandomNumberGenerator.GetBytes(5)).ToLowerInvariant()}";
		}

		private static bool HasOwnedPrefix(string name)
		{
			return name.StartsWith("futuremud_gather_gc_", StringComparison.Ordinal) ||
			       name.StartsWith("futuremud_land_", StringComparison.Ordinal);
		}

		private static string EscapeIdentifier(string value) => value.Replace("`", "``", StringComparison.Ordinal);
	}
}
