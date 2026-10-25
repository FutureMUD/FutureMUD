#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;

namespace DatabaseSeeder.Seeders;

public partial class CelestialSeeder
{
	private const string PackageSelectionPrompt = @"#DCelestial Packages#F

#3EarthSun#F - Earth-facing daylight sun, named The Sun.
#3EarthMoon#F - The Moon, lunar phases and linked moon-surface sky views; requires EarthSun.
#3Jupiter#F - Ganymede viewpoint with Jupiter dominating the sky and distant Sol.
#3Armageddon#F - Suk-Krath, Jihae and Lirathu; requires Armageddon time.
#3Rails#F - RailSun and RailMoon authored examples.
#3Scripts#F - ScriptedSun and ScriptedMoon authored examples.
#3MorningStar#F - Authored Morning Star example.
#3MorningGlow#F - Authored Morning Glow example.

Authored examples remain unattached until a builder selects them.
Choose one or more option names separated by spaces, or #3none#F to skip.
Only selected, incomplete packages need follow-up questions.";

	private static readonly string[] PackageOptions =
	[
		"EarthSun", "EarthMoon", "Jupiter", "Rails", "Scripts", "MorningStar", "MorningGlow", "Armageddon"
	];

	private static IReadOnlyDictionary<string, string> ResolveAutomaticAnswers(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> answers)
	{
		var resolved = new Dictionary<string, string>(answers, StringComparer.OrdinalIgnoreCase)
		{
			["sunname"] = "The Sun",
			["moonname"] = "The Moon"
		};
		var calendars = context.Calendars.Select(x => x.Id).Take(2).ToList();
		if (calendars.Count == 1)
		{
			foreach (var key in new[] { "suncalendar", "mooncalendar", "gasgiantcalendar", "authoredcalendar" })
			{
				resolved.TryAdd(key, calendars[0].ToString(System.Globalization.CultureInfo.InvariantCulture));
			}
		}

		return resolved;
	}

	private static string[] SelectedPackages(IReadOnlyDictionary<string, string> answers) =>
		answers.TryGetValue("packages", out var selection)
			? selection.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
			: [];

	private static bool PackageSelected(IReadOnlyDictionary<string, string> answers, string option) =>
		SelectedPackages(answers).Contains(option, StringComparer.OrdinalIgnoreCase);

	private static IEnumerable<string> SelectedAuthoredPresets(IReadOnlyDictionary<string, string> answers)
	{
		if (PackageSelected(answers, "Rails"))
		{
			yield return "RailSun";
			yield return "RailMoon";
		}

		if (PackageSelected(answers, "Scripts"))
		{
			yield return "ScriptedSun";
			yield return "ScriptedMoon";
		}

		foreach (var option in new[] { "MorningStar", "MorningGlow" })
		{
			if (PackageSelected(answers, option))
			{
				yield return option;
			}
		}
	}

	private static bool HasSelectedAuthoredPackages(IReadOnlyDictionary<string, string> answers) =>
		SelectedAuthoredPresets(answers).Any();

	private static bool HasMissingSelectedAuthoredPresets(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> answers) =>
		SelectedAuthoredPresets(answers).Except(AuthoredPresetsInstalled(context), StringComparer.OrdinalIgnoreCase).Any();

	private static (bool Success, string error) ValidatePackages(string answer, FuturemudDatabaseContext context)
	{
		var selections = answer.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
		if (selections.Length == 1 && selections[0].Equals("none", StringComparison.OrdinalIgnoreCase))
		{
			return (true, string.Empty);
		}

		if (selections.Length == 0 || selections.Any(x => !PackageOptions.Contains(x, StringComparer.OrdinalIgnoreCase)))
		{
			return (false, $"Choose option names separated by spaces: {string.Join(" ", PackageOptions)}, or none by itself.");
		}

		if (selections.Contains("EarthMoon", StringComparer.OrdinalIgnoreCase) && !HasEarthMoonPackage(context) &&
			!HasSingleEarthSunCandidate(context) &&
			!(selections.Contains("EarthSun", StringComparer.OrdinalIgnoreCase) && !HasEarthSunPackage(context)))
		{
			return (false, "EarthMoon requires exactly one Earth-facing Sun. Select EarthSun too on a fresh world, or resolve existing ambiguous Sun records first.");
		}

		return selections.Contains("Armageddon", StringComparer.OrdinalIgnoreCase)
			? ValidateArmageddonInstall(context)
			: (true, string.Empty);
	}

	internal static string? ResolvePackagesDefault(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> answers)
	{
		if (ArmageddonCalendar(context) is not null)
		{
			return HasArmageddonPackage(context) ? "none" : "Armageddon";
		}

		var options = new List<string>();
		if (!HasEarthSunPackage(context)) options.Add("EarthSun");
		if (!HasEarthMoonPackage(context) && (!HasEarthSunPackage(context) || HasSingleEarthSunCandidate(context)))
			options.Add("EarthMoon");
		return options.Count == 0 ? "none" : string.Join(" ", options);
	}

	internal static ConsoleQuestionDisplay ResolvePackagesDisplay(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> answers)
	{
		var installed = new List<string>();
		if (HasEarthSunPackage(context)) installed.Add("EarthSun");
		if (HasEarthMoonPackage(context)) installed.Add("EarthMoon");
		if (HasGasGiantPackage(context)) installed.Add("Jupiter");
		if (HasArmageddonPackage(context)) installed.Add("Armageddon");
		foreach (var option in PackageOptions.Skip(3).Where(x => x != "Armageddon"))
		{
			if (!HasMissingSelectedAuthoredPresets(context, new Dictionary<string, string> { ["packages"] = option }))
				installed.Add(option);
		}

		return new ConsoleQuestionDisplay(PackageSelectionPrompt +
			$"\n\nAlready installed: {(installed.Count == 0 ? "none" : string.Join(", ", installed))}.",
			ResolvePackagesDefault(context, answers));
	}
}
