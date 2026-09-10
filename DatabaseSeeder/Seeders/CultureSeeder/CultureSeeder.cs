#nullable enable

using MudSharp.Database;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using DatabaseSeeder.Seeders.CultureToolkit;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder : IDatabaseSeeder
{
    public IEnumerable<(string Id, string Question,
        Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
        Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
        new List<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool>
            Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
        {
            ("culturepacks", @"#DCulture Packs#F

The FutureMUD Database Seeder currently has the following culture packs, which include full suites of naming conventions, languages and dialects and sometimes more for various real world or fictional settings.

#BEarth-Modern#F: This culture pack includes ethnicities, cultures, languages, scripts and accents from the modern Earth
#BAntiquity#F: Classical antiquity to about 500
#BDark Ages#F: About 500-1100
#BMedieval#F: About 1000-1400
#BRenaissance#F: About 1400-1600
#BEarly Modern#F: About 1600-1750
#BMiddle-Earth#F: This culture pack includes races, ethnicities, cultures, languages, scripts and dialects from J.R.R. Tolkien's Middle-Earth

Each historical era is a self-contained toolkit of social backgrounds, ethnicities, names, languages, scripts and accents for Europe and the neighbouring Near Eastern, North African and Black Sea regions. These overlapping toolkits do not require every entry to coexist in one year. Select one era.

#1Note: Even if you choose none of the above, some useful culture-related defaults will be installed to make things easier for you#F

You can either use 'none' to select none of the above, or use one of the pack names to install that pack.",
                (context, answers) => true, (text, context) =>
                {
                    if (NormalizeCulturePackAnswer(text) is not ("none" or "earthmodern" or "modern" or
                            "earthantiquity" or "antiquity" or "earthdarkagesandmedieval" or
                            "darkagesandmedieval" or "earthrenaissanceeurope" or "renaissanceeurope" or
                            "earthmedievaleurope" or "medievaleurope" or "earthrenaissanceworldexpansion" or
                            "renaissanceworldexpansion" or "middleearth" or "darkages" or "medieval" or "renaissance" or "earlymodern"))
                    {
                        return (false, "You must select one of the pack names, or use 'none' to select none of them.");
                    }

                    return (true, string.Empty);
                }),

            ("seednames",
                @"Would you like to install the naming cultures and random name generators from your chosen culture pack?

Please answer #3yes#f or #3no#f. ", (context, answers) => CulturePackInstallsOptionalContent(answers),
                (text, context) =>
                {
                    if (!text.EqualToAny("yes", "y", "no", "n")) { return (false, "Please choose yes or no."); } return (true, string.Empty);
                }),

            ("seedlanguages",
                @"Would you like to install the languages, accents, and scripts from your chosen culture pack?

Please answer #3yes#f or #3no#f. ", (context, answers) => CulturePackSupportsLanguages(answers),
                (text, context) =>
                {
                    if (!text.EqualToAny("yes", "y", "no", "n")) { return (false, "Please choose yes or no."); } return (true, string.Empty);
                }),

			("seedsignedlanguages",
				@"Would you like to install the signed languages and regional varieties from the Earth-Modern culture pack?

Signed languages are independent from spoken languages and may be installed even if you do not install spoken languages, accents, or scripts.

Please answer #3yes#f or #3no#f. ", (context, answers) => CulturePackSupportsModernSignedLanguages(answers),
				(text, context) =>
				{
					if (!text.EqualToAny("yes", "y", "no", "n")) { return (false, "Please choose yes or no."); } return (true, string.Empty);
				}),

            ("seedheritage",
                @"Would you like to install the races, ethnicities and cultures from your chosen culture pack?

Please answer #3yes#f or #3no#f. ", (context, answers) => CulturePackInstallsOptionalContent(answers),
                (text, context) =>
                {
                    if (!text.EqualToAny("yes", "y", "no", "n")) { return (false, "Please choose yes or no."); } return (true, string.Empty);
                })
        };

    private static string NormalizeCulturePackAnswer(string answer)
    {
        return answer.Replace("-", string.Empty)
            .Replace(" ", string.Empty)
            .ToLowerInvariant();
    }

    private static bool CulturePackInstallsOptionalContent(IReadOnlyDictionary<string, string> answers)
    {
        return !answers.TryGetValue("culturepacks", out var answer) ||
               NormalizeCulturePackAnswer(answer) != "none";
    }

    private static bool CulturePackSupportsLanguages(IReadOnlyDictionary<string, string> answers)
    {
        if (!answers.TryGetValue("culturepacks", out var answer))
        {
            return true;
        }

        return NormalizeCulturePackAnswer(answer) is "earthmodern" or "modern" or
            "earthantiquity" or "antiquity" or "earthdarkagesandmedieval" or
            "darkagesandmedieval" or "earthrenaissanceeurope" or
            "renaissanceeurope" or "earthmedievaleurope" or "medievaleurope" or
            "earthrenaissanceworldexpansion" or "renaissanceworldexpansion" or
            "middleearth" or "darkages" or "medieval" or "renaissance" or "earlymodern";
    }

	private static bool CulturePackSupportsModernSignedLanguages(IReadOnlyDictionary<string, string> answers)
	{
		return !answers.TryGetValue("culturepacks", out var answer) ||
		       NormalizeCulturePackAnswer(answer) is "earthmodern" or "modern";
	}

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		using var transaction = context.Database.BeginTransaction();
		var era = ToolkitEra(questionAnswers["culturepacks"]);
		if (era is not null)
		{
			bool Requested(string key) => !questionAnswers.TryGetValue(key, out var answer) || answer.EqualToAny("yes", "y");
			var elapsed = Stopwatch.StartNew();
			var report = CultureToolkitInstaller.Install(context, era, Requested("seednames"), Requested("seedlanguages"), Requested("seedheritage"),
				message => ConsoleUtilities.WriteLineConsole($"[{elapsed.Elapsed.TotalSeconds:N1}s] {message}"));
			transaction.Commit();
			var unresolvedNatives = report.NativeBindings.SelectMany(x => x.Unresolved).ToArray();
			return $"Installed {era}: {report.CultureIds.Count:N0} social backgrounds, {report.EthnicityIds.Count:N0} retained/overlay identities and {report.Groups.Count:N0} optional language groups." +
				(unresolvedNatives.Length == 0 ? "" : "\nUnresolved native-language bindings (identities without authored crosswalks are gated from chargen; rerunning cannot supply missing crosswalks):\n" + string.Join("\n", unresolvedNatives)) +
				(report.Conflicts.Count == 0 ? "" : "\nPreserved overrides or unresolved bindings:\n" + string.Join("\n", report.Conflicts));
		}
		_context = context;
		SeedSimple(context);
		if (!questionAnswers["culturepacks"].EqualToAny("none"))
		{
			SeedCulturePacks(context, questionAnswers);
		}

		var accentConflicts = new List<string>();
		foreach (var group in _accentMetadata.GroupBy(x => x.Accent.Id))
			CultureStockAccentRoles.ApplyLegacy(context, group.First().Accent, group.Any(x => x.Fresh), accentConflicts);
		_accentMetadata.Clear();
		RefreshExistingCultureRaceSatiationLimits();
		EnsureFallbackRandomNameProfiles();
		ChargenFreeKnowledgeProgReconcileResult freeKnowledgeResult =
			ChargenFreeKnowledgeProgReconciler.Reconcile(context);
		context.SaveChanges();

		transaction.Commit();
		var message = string.IsNullOrWhiteSpace(freeKnowledgeResult.Message)
			? "Completed successfully." : $"Completed successfully. {freeKnowledgeResult.Message}";
		return accentConflicts.Count == 0 ? message : message + "\n" + string.Join("\n", accentConflicts);
	}

	internal static string? ToolkitEra(string answer) => NormalizeCulturePackAnswer(answer) switch
	{
		"antiquity" or "earthantiquity" => "antiquity",
		"darkages" => "darkages",
		"medieval" or "earthdarkagesandmedieval" or "darkagesandmedieval" or "earthmedievaleurope" or "medievaleurope" => "medieval",
		"renaissance" or "earthrenaissanceeurope" or "renaissanceeurope" => "renaissance",
		"earlymodern" => "earlymodern",
		_ => null
	};

    public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
    {
        if (!context.Races.Any(x => x.Name == "Human") || !context.TraitDecorators.Any(x => x.Name.Contains("Skill")))
        {
            return ShouldSeedResult.PrerequisitesNotMet;
        }

        if (ChargenSizeProgMarkers.Any(marker => !context.FutureProgs.Any(x => x.FunctionName == marker)))
        {
            return ShouldSeedResult.PrerequisitesNotMet;
        }

        ShouldSeedResult repeatability = SeederRepeatabilityHelper.ClassifyByPresence(
            StockNameCultureMarkers.Select(marker => context.NameCultures.Any(x => x.Name == marker))
                .Concat(StockRandomProfileMarkers.Select(marker => context.RandomNameProfiles.Any(x => x.Name == marker)))
                .Concat(StockCulturePackageMarkers.Select(marker =>
                    context.Languages.Any(x => x.Name == marker) ||
                    context.Ethnicities.Any(x => x.Name == marker) ||
                    context.Cultures.Any(x => x.Name == marker))));
		if (repeatability == ShouldSeedResult.MayAlreadyBeInstalled &&
		    (HasCultureRaceSatiationLimitUpdates(context) ||
		     ChargenFreeKnowledgeProgReconciler.HasRepairableCultureDrift(context)))
		{
			return ShouldSeedResult.ExtraPackagesAvailable;
		}

		return repeatability;
    }

    public bool SafeToRunMoreThanOnce => true;
    public int SortOrder => 101;
    public string Name => "Culture Seeder";
    public string Tagline => "Add Name Cultures, Random Names and optionally culture packs";

    public string FullDescription =>
        @"This package will setup a few culture related items in the engine such as naming cultures, random name options and also optionally some earth-based cultural items such as languages and dialects.

1) Naming Cultures are prerequisites for all Cultures - a culture must have a naming culture. This defines how names from that culture are put together (what elements they have, what order they appear in and so forth). The seeder provides a few simple examples from the real world, which you can adapt to your own purposes if your own world does something differently.

2) Random Name Profiles are used to randomly generate a name from the naming culture. This is used when generating variable NPCs as well as for builders in the RANDOMNAME command if they want to come up with a culturally-appropriate name on the fly. The engine ships with several useful examples but you will almost certainly want to add your own.

3) If you are making a game from an IP that the seeder supports (or the real world) you may want to import one of the ""Culture Packages"". These include a list of languages and dialects appropriate to the time period you select from real earth.";
}
