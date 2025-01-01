using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;
using MudSharp.Framework;

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
#BEarth-Antiquity#F: This culture pack includes languages, scripts and accents from European antiquity at roughly the time of the late republic
#BEarth-MedievalEurope#F: This culture pack includes ethnicities, names, languages, scripts and accents from Medieval Europe (and surrounds), roughly 15th century
#BMiddle-Earth#F: This culture pack includes races, ethnicities, cultures, languages, scripts and dialects from J.R.R. Tolkien's Middle-Earth

#1Note: Even if you choose none of the above, some useful culture-related defaults will be installed to make things easier for you#F

You can either use 'none' to select none of the above, or use one of the pack names to install that pack.",
				(context, answers) => true, (text, context) =>
				{
					if (!text.EqualToAny("none", "earth-modern", "earth-medievaleurope", "earth-antiquity",
						    "middle-earth"))
						return (false, "You must select one of the pack names, or use 'none' to select none of them.");

					return (true, string.Empty);
				}),

			("seednames",
				@"Would you like to install the naming cultures and random name generators from your chosen culture pack?

Please answer #3yes#f or #3no#f. ", (context, answers) => true,
				(text, context) =>
				{
					if (!text.EqualToAny("yes", "y", "no", "n")) return (false, "Please choose yes or no.");

					return (true, string.Empty);
				}),

			("seedlanguages",
				@"Would you like to install the languages, accents, and scripts from your chosen culture pack?

Please answer #3yes#f or #3no#f. ", (context, answers) => true,
				(text, context) =>
				{
					if (!text.EqualToAny("yes", "y", "no", "n")) return (false, "Please choose yes or no.");

					return (true, string.Empty);
				}),

			("seedheritage",
				@"Would you like to install the races, ethnicities and cultures from your chosen culture pack?

Please answer #3yes#f or #3no#f. ", (context, answers) => true,
				(text, context) =>
				{
					if (!text.EqualToAny("yes", "y", "no", "n")) return (false, "Please choose yes or no.");

					return (true, string.Empty);
				})
		};

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		context.Database.BeginTransaction();
		_context = context;
		SeedSimple(context);
		if (!questionAnswers["culturepacks"].EqualToAny("none")) SeedCulturePacks(context, questionAnswers);

		context.Database.CommitTransaction();
		return "Completed successfully.";
	}

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.Races.Any(x => x.Name == "Human") || !context.TraitDecorators.Any(x => x.Name.Contains("Skill")))
			return ShouldSeedResult.PrerequisitesNotMet;

		if (!context.FutureProgs.Any(x => x.FunctionName == "MaximumHeightChargen"))
		{
			return ShouldSeedResult.PrerequisitesNotMet;
		}

		if (context.RandomNameProfiles.Any()) return ShouldSeedResult.MayAlreadyBeInstalled;

		return ShouldSeedResult.ReadyToInstall;
	}

	public int SortOrder => 101;
	public string Name => "Culture Seeder";
	public string Tagline => "Add Name Cultures, Random Names and optionally culture packs";

	public string FullDescription =>
		@"This package will setup a few culture related items in the engine such as naming cultures, random name options and also optionally some earth-based cultural items such as languages and dialects.

1) Naming Cultures are prerequisites for all Cultures - a culture must have a naming culture. This defines how names from that culture are put together (what elements they have, what order they appear in and so forth). The seeder provides a few simple examples from the real world, which you can adapt to your own purposes if your own world does something differently.

2) Random Name Profiles are used to randomly generate a name from the naming culture. This is used when generating variable NPCs as well as for builders in the RANDOMNAME command if they want to come up with a culturally-appropriate name on the fly. The engine ships with several useful examples but you will almost certainly want to add your own.

3) If you are making a game from an IP that the seeder supports (or the real world) you may want to import one of the ""Culture Packages"". These include a list of languages and dialects appropriate to the time period you select from real earth.";
}