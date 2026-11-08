#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonHeritage
{
	internal sealed record PhysicalProfile(int Youth, int YoungAdult, int Adult, int MiddleAged, int Elder,
		int Venerable, int MaximumCreationAge, double MinimumHeightInches, double MaximumHeightInches,
		double MinimumWeightKg, double MaximumWeightKg, SizeCategory Standing, SizeCategory Lower, int PartSize);

	internal static PhysicalProfile PhysicalFor(string profile) => profile switch
	{
		"human" => new(10, 20, 30, 41, 61, 81, 80, 62, 78, 60, 90, SizeCategory.Normal, SizeCategory.Small, 0),
		"city_elf" or "desert_elf" => new(12, 24, 35, 48, 71, 94, 93, 74, 90, 70, 90, SizeCategory.Large, SizeCategory.Normal, 0),
		"dwarf" => new(22, 44, 66, 91, 135, 179, 178, 50, 58, 80, 100, SizeCategory.Small, SizeCategory.VerySmall, -1),
		"half_elf" => new(11, 22, 33, 45, 66, 88, 87, 70, 82, 60, 90, SizeCategory.Normal, SizeCategory.Small, 0),
		"half_giant" => new(18, 36, 54, 74, 110, 146, 145, 125, 155, 750, 900, SizeCategory.VeryLarge, SizeCategory.Large, 2),
		"mul" => new(8, 15, 23, 31, 46, 61, 60, 60, 72, 100, 150, SizeCategory.Normal, SizeCategory.Small, 0),
		_ => throw new ArgumentOutOfRangeException(nameof(profile))
	};

	private static readonly (ChargenStage Stage, string[] Fields)[] PhysicalScreens =
	[
		(ChargenStage.SelectBirthday, ["MinimumAgeProg", "MaximumAgeProg"]),
		(ChargenStage.SelectHeight, ["MinimumHeightProg", "MaximumHeightProg"]),
		(ChargenStage.SelectWeight, ["MinimumWeightProg", "MaximumWeightProg"])
	];

	private static string PhysicalProgKey(long storyboard, string field) => $"arm.chargen.physical.{storyboard}.{field.ToLowerInvariant()}";
	private static string PhysicalProgName(long storyboard, string field) => $"Armageddon{field}{storyboard}";

	private static void PreflightPhysical(FuturemudDatabaseContext db)
	{
		Preflight<HeightWeightModel>(db, Profiles.Select(x => ($"arm.physical.{x.Key}.model", $"Armageddon {x.Name} Physical Model")));
		Preflight<FutureProg>(db, Profiles.Select(x => ($"arm.age.{x.Key}.description", "ArmageddonAgeDescription" + x.Key.Replace("_", ""))));
		foreach (var (stage, fields) in PhysicalScreens)
		{
			var screen = db.ChargenScreenStoryboards.SingleOrDefault(x => x.ChargenStage == (int)stage)
				?? throw new InvalidOperationException($"Armageddon physical profiles require the {stage} chargen screen.");
			var xml = XElement.Parse(screen.StageDefinition);
			foreach (var field in fields)
			{
				var reference = xml.Element(field)?.Value ?? throw new InvalidOperationException($"{stage} is missing {field}.");
				var prog = long.TryParse(reference, out var id) ? db.FutureProgs.SingleOrDefault(x => x.Id == id) : db.FutureProgs.SingleOrDefault(x => x.FunctionName == reference);
				var parameters = prog is null ? [] : db.FutureProgsParameters.Where(x => x.FutureProgId == prog.Id).OrderBy(x => x.ParameterIndex).ToArray();
				if (prog is null || prog.ReturnType != (long)ProgVariableTypes.Number || !prog.AcceptsAnyParameters &&
					(parameters.Length != 1 || parameters[0].ParameterType != (long)ProgVariableTypes.Toon && parameters[0].ParameterType != (long)ProgVariableTypes.Chargen))
					throw new InvalidOperationException($"{stage}/{field} must reference number(toon/chargen). Its configured pointer and body were preserved.");
			}
			Preflight<FutureProg>(db, fields.Select(x => (PhysicalProgKey(screen.Id, x), PhysicalProgName(screen.Id, x))));
		}
	}

	private static string AgeDescriptionBody(PhysicalProfile profile)
	{
		var body = new StringBuilder();
		foreach (var (start, label) in new[] { (profile.YoungAdult, "young"), (profile.Adult, "adult"),
			(profile.MiddleAged, "mature"), (profile.Elder, "middle-aged"), (profile.Venerable, "old") })
			body.AppendLine($"if (@age < {start})\n  return \"{label}\"\nend if");
		return body.AppendLine("return \"ancient\"").ToString();
	}

	private static void PhysicalDefaults(FuturemudDatabaseContext db, Race race, string profile, ICollection<string> messages)
	{
		var physical = PhysicalFor(profile);
		var model = Apply(db, $"arm.physical.{profile}.model", new HeightWeightModel
		{
			Name = $"Armageddon {Profiles.Single(x => x.Key == profile).Name} Physical Model",
			// Authored neutral distribution; approved PC limits are enforced separately by chargen progs.
			MeanHeight = (physical.MinimumHeightInches + physical.MaximumHeightInches) * 2.54 / 2,
			StddevHeight = (physical.MaximumHeightInches - physical.MinimumHeightInches) * 2.54 / 6,
			MeanWeight = (physical.MinimumWeightKg + physical.MaximumWeightKg) * 1000 / 2,
			StddevWeight = (physical.MaximumWeightKg - physical.MinimumWeightKg) * 1000 / 6,
			MeanBmi = 22, StddevBmi = 0, Bmimultiplier = 0.1,
			SkewnessHeight = 0, SkewnessWeight = 0, SkewnessBMI = 0
		}, messages);
		var describer = Prog(db, $"arm.age.{profile}.description", "ArmageddonAgeDescription" + profile.Replace("_", ""),
			AgeDescriptionBody(physical), ProgVariableTypes.Text, messages, (ProgVariableTypes.Number, "age"));
		race.DefaultHeightWeightModelMaleId = model.Id;
		race.DefaultHeightWeightModelFemaleId = model.Id;
		race.DefaultHeightWeightModelNeuterId = model.Id;
		race.DefaultHeightWeightModelNonBinaryId = model.Id;
		race.ChildAge = 3; race.YouthAge = physical.Youth; race.YoungAdultAge = physical.YoungAdult;
		race.AdultAge = physical.Adult; race.ElderAge = physical.Elder; race.VenerableAge = physical.Venerable;
		race.AgeDescriptionProgId = describer.Id;
		race.SizeStanding = (int)physical.Standing;
		race.SizeSitting = race.SizeProne = (int)physical.Lower;
		race.BodypartSizeModifier = physical.PartSize;
		race.BodypartHealthMultiplier = 1; race.PainToleranceMultiplier = 1;
		race.NaturalArmourTypeId = null; race.NaturalArmourMaterialId = null;
		race.TemperatureRangeFloor = 0; race.TemperatureRangeCeiling = 40;
		race.BreathingVolumeExpression = "7";
	}

	private static double PhysicalBound(PhysicalProfile profile, string field) => field switch
	{
		"MinimumAgeProg" => profile.YoungAdult,
		"MaximumAgeProg" => profile.MaximumCreationAge,
		"MinimumHeightProg" => profile.MinimumHeightInches * 2.54,
		"MaximumHeightProg" => profile.MaximumHeightInches * 2.54,
		"MinimumWeightProg" => profile.MinimumWeightKg * 1000,
		"MaximumWeightProg" => profile.MaximumWeightKg * 1000,
		_ => throw new ArgumentOutOfRangeException(nameof(field))
	};

	private static void WirePhysicalScreens(FuturemudDatabaseContext db, IReadOnlyDictionary<string, Race> races, ICollection<string> messages)
	{
		const string marker = "// Armageddon physical delegate id: ";
		foreach (var (stage, fields) in PhysicalScreens)
		{
			var screen = db.ChargenScreenStoryboards.Single(x => x.ChargenStage == (int)stage);
			var xml = XElement.Parse(screen.StageDefinition);
			var current = new Dictionary<string, string>();
			var desired = new Dictionary<string, string>();
			foreach (var field in fields)
			{
				var reference = xml.Element(field)!;
				current[field] = Json(reference.Value);
				var key = PhysicalProgKey(screen.Id, field);
				var previous = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
				long delegateId;
				if (previous is null)
					delegateId = long.TryParse(reference.Value, out var id) ? id : db.FutureProgs.Single(x => x.FunctionName == reference.Value).Id;
				else
				{
					var savedBody = JsonSerializer.Deserialize<string>(JsonSerializer.Deserialize<Dictionary<string, string>>(previous.SeedBaseline!)!["FunctionText"])!;
					var first = savedBody.Replace("\r\n", "\n").Split('\n')[0];
					if (!first.StartsWith(marker, StringComparison.Ordinal) || !long.TryParse(first[marker.Length..], out delegateId))
						throw new InvalidOperationException($"{key}: missing original physical-policy delegate marker; preserve and resolve composition.");
				}
				var original = db.FutureProgs.Single(x => x.Id == delegateId);
				var signature = db.FutureProgsParameters.Where(x => x.FutureProgId == delegateId).OrderBy(x => x.ParameterIndex).ToArray();
				if (original.ReturnType != (long)ProgVariableTypes.Number || !original.AcceptsAnyParameters &&
					(signature.Length != 1 || signature[0].ParameterType != (long)ProgVariableTypes.Toon && signature[0].ParameterType != (long)ProgVariableTypes.Chargen))
					throw new InvalidOperationException($"{key}: original physical-policy delegate has an incompatible signature; preserve and resolve it explicitly.");
				var body = new StringBuilder(marker + delegateId + "\n");
				foreach (var profile in Profiles)
					body.AppendLine($"if (@ch.Race == ToRace({races[profile.Key].Id}))\n  return {PhysicalBound(PhysicalFor(profile.Key), field).ToString("R", CultureInfo.InvariantCulture)}\nend if");
				body.AppendLine($"return @{original.FunctionName}(@ch)");
				var parameterType = original.AcceptsAnyParameters ? ProgVariableTypes.Toon : (ProgVariableTypes)signature[0].ParameterType;
				var wrapper = Prog(db, key, PhysicalProgName(screen.Id, field), body.ToString(), ProgVariableTypes.Number, messages, (parameterType, "ch"));
				desired[field] = Json(wrapper.Id.ToString(CultureInfo.InvariantCulture));
			}
			var merged = ArmageddonSettingLinks.Reconcile(db, $"arm.chargen.physical-wiring.{screen.Id}", nameof(ChargenScreenStoryboard), screen.Id, current, desired, messages);
			foreach (var field in fields) xml.Element(field)!.Value = JsonSerializer.Deserialize<string>(merged[field])!;
			screen.StageDefinition = xml.ToString();
		}
	}
}
