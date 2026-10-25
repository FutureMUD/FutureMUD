#nullable enable

extern alias EngineCompiler;

using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.TimeAndDate.Date;
using EngineCompiler::MudSharp.Celestial.Authored;

namespace AuthoredCelestialSmokeWorld;

/// <summary>Runs only on the wrapper's identity-checked disposable instance, after the complete replay/refusal gate.</summary>
internal static class ArmageddonTimeSkyVerification
{
	public static void Run(string connection, string receiptPath)
	{
		FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql(connection, ServerVersion.AutoDetect(connection)).Options);
		void Require(bool condition, string message)
		{
			if (!condition) throw new InvalidOperationException(message);
		}

		var timeAnswers = new Dictionary<string, string> { ["mode"] = "Armageddon", ["secondsmultiplier"] = "6", ["startyear"] = "1" };
		var skyAnswers = new Dictionary<string, string>
		{
			["installsun"] = "no", ["installmoon"] = "no", ["installgasgiantmoon"] = "no",
			["installauthored"] = "no", ["installarmageddon"] = "yes"
		};
		var assembly = typeof(TimeSeeder).Assembly;
		var execute = assembly.GetType("DatabaseSeeder.SeederExecutionService")!.GetMethod("Execute", BindingFlags.Static | BindingFlags.NonPublic)!;
		void Execute(IDatabaseSeeder seeder, IReadOnlyDictionary<string, string> answers)
		{
			using var db = Context();
			var questions = SeederQuestionRegistry.GetQuestions(seeder, seeder.SeederQuestions).ToArray();
			foreach (var question in questions.Where(x => x.Filter(db, answers)))
			{
				Require(answers.TryGetValue(question.Id, out var answer), $"Missing active {seeder.Name} answer {question.Id}.");
				var validation = question.Validator(answer!, db);
				Require(validation.Success, validation.error);
			}
			var result = execute.Invoke(null, [db, seeder, questions, answers, assembly.GetName().Version!])!;
			var success = (bool)result.GetType().GetProperty("Success")!.GetValue(result)!;
			if (!success) throw new InvalidOperationException($"Native {seeder.Name} execution failed.",
				(Exception?)result.GetType().GetProperty("Exception")!.GetValue(result));
		}

		Execute(new TimeSeeder(), timeAnswers);
		Execute(new CelestialSeeder(), skyAnswers);
		ArmageddonWeatherVerification.Run(connection, Path.Combine(Path.GetDirectoryName(receiptPath)!, "armageddon-weather.json"));
		long calendarId, clockId, timezoneId;
		string calendarDefinition, clockDefinition, sunDefinition;
		long[] retainedIds;
		long removedId;
		using (var db = Context())
		{
			var calendar = db.Calendars.AsEnumerable().Single(x => XElement.Parse(x.Definition).Element("alias")?.Value == "armageddon");
			calendarId = calendar.Id;
			clockId = calendar.FeedClockId;
			var names = XElement.Parse(calendar.Definition).Element("namedyearcycle")!;
			var cycle = new NamedYearCycle(names.Attribute("name")!.Value, (int)names.Attribute("epochyear")!, names.Elements("year").Select(x => x.Value));
			Require(cycle.YearNames.Count == 77 && cycle.GetYear(78) == new NamedYearInfo("Jihae's Anger", 1, 2), "Persisted year-cycle labels did not reload.");
			var bodies = db.Celestials.Where(x => x.FeedClockId == clockId).OrderBy(x => x.Id).ToArray();
			Require(bodies.Length == 3, "Expected precisely three persisted Armageddon bodies.");
			foreach (var body in bodies)
			{
				var definition = AuthoredCelestialFormat.Parse(body.Definition);
				Require(definition.CalendarId == calendarId, "Persisted celestial context did not reload.");
				var compiled = new CompiledAuthoredCelestial(definition, 60, 60, 9);
				Require(compiled.Evaluate(77L * 540 * 60).Elevation < 0, "Midnight conjunction should be below the horizon.");
			}
			var bodyIds = bodies.Select(b => b.Id).ToArray();
			Require(!db.ShardsCelestials.Any(x => bodyIds.Contains(x.CelestialId)), "Armageddon objects were attached automatically.");
			Require(db.SeederChoices.Any(x => x.Choice == "installarmageddon" && x.Answer == "yes"), "New answer did not persist through the shared executor.");

			// Deliberate fixture customisations across separate contexts exercise the preserving rerun contract.
			var root = XElement.Parse(calendar.Definition);
			root.Element("alias")!.Value = "Armageddon";
			root.Element("namedyearcycle")!.Elements("year").First().Value = "Builder's First Year";
			calendar.Definition = root.ToString();
			calendar.Date = "17/low/80";
			calendarDefinition = calendar.Definition;
			var clock = db.Clocks.Find(clockId)!;
			var clockRoot = XElement.Parse(clock.Definition);
			clockRoot.Element("InGameSecondsPerRealSecond")!.Value = "3";
			clock.Definition = clockRoot.ToString();
			clock.Hours = 4;
			clockDefinition = clock.Definition;
			var timezone = db.Timezones.Single(x => x.ClockId == clockId);
			timezoneId = timezone.Id;
			timezone.Name = "Builder Time";
			timezone.OffsetHours = 2;
			var sun = AuthoredCelestialFormat.Parse(bodies[0].Definition);
			sun.Name = "Builder's Sun";
			bodies[0].Definition = AuthoredCelestialFormat.Serialize(sun);
			sunDefinition = bodies[0].Definition;
			retainedIds = bodies.Take(2).Select(x => x.Id).ToArray();
			removedId = bodies[2].Id;
			db.Celestials.Remove(bodies[2]);
			db.SaveChanges();
		}

		Execute(new TimeSeeder(), timeAnswers);
		Execute(new CelestialSeeder(), skyAnswers);
		using (var db = Context())
		{
			var calendar = db.Calendars.Find(calendarId)!;
			var clock = db.Clocks.Find(clockId)!;
			Require(calendar.Definition == calendarDefinition && calendar.Date == "17/low/80", "Native rerun replaced calendar customisations/date.");
			Require(clock.Definition == clockDefinition && clock.Hours == 4 && clock.PrimaryTimezoneId == timezoneId, "Native rerun replaced clock state or primary timezone.");
			var timezone = db.Timezones.Find(timezoneId)!;
			Require(timezone.Name == "Builder Time" && timezone.OffsetHours == 2, "Native rerun replaced timezone customisations.");
			Require(db.Timezones.Count(x => x.ClockId == clockId) == 2, "Missing stock timezone was not added separately.");
			Require(db.Celestials.Find(retainedIds[0])!.Definition == sunDefinition && db.Celestials.Find(retainedIds[1]) is not null, "Native rerun replaced retained celestial IDs/definitions.");
			Require(db.Celestials.Count(x => x.FeedClockId == clockId) == 3 && db.Celestials.Find(removedId) is null, "Native rerun did not restore exactly the missing member.");
		}

		File.WriteAllText(receiptPath, JsonSerializer.Serialize(new
		{
			status = "PASS", scenario = "Armageddon time/sky native install, cold-context reload and preserving partial rerun",
			calendarId, clockId, retainedIds, removedId, savedAnswers = true, automaticAttachment = false,
			gameplaySmoke = "NOT_RUN"
		}, new JsonSerializerOptions { WriteIndented = true }));
		Console.WriteLine("PASS: Armageddon time/sky native reload and preserving partial rerun.");
	}
}
