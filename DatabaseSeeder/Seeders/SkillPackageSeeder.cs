using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

internal class SkillPackageSeeder : SkillSeederBase
{
	#region Implementation of IDatabaseSeeder

	public override
		IEnumerable<(string Id, string Question,
			Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
			Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
		new List<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool>
			Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
		{
			("branching",
				"Do you want to enable skill branching on use? You might choose to disable this if you want to control skill branching by class for example.\n\nPlease answer #3yes#F or #3no#F. ",
				(context, answers) => true, (text, context) =>
				{
					if (!text.EqualToAny("yes", "y", "no", "n")) return (false, "Please answer #3yes#F or #3no#F.");

					return (true, string.Empty);
				}),
			("skillcapmodel",
				@"Skill cap models determine what is the maximum value a character's skills can rise to. These models can be customised once in game, and you can even go for a hybrid system or something different altogether.

You can choose from the following skill cap models:

#BRPI#F    - skill caps are based on character attributes, e.g. #65*intelligence#0 or #63*dexterity + 2*strength#0.
#BClass#F  - skill caps are determined by your class (obviously use with class system enabled)
#BFlat#F   - skill caps are flat across the board, the same for everyone (but you might shift with projects, merits, etc)

What is your selection? ", (context, answers) => true,
				(text, context) =>
				{
					switch (text.ToLowerInvariant())
					{
						case "rpi":
						case "class":
						case "flat":
							return (true, string.Empty);
					}

					return (false, "That is not a valid selection.");
				}
			),
			("skillgainmodel",
				@"You can choose from the following skill gain models:

#BRPI#F          - skills are branched by use and improve on failure
#BLabMUD#F       - skills are branched by use and improve on success, and require increasingly higher difficulties
#BArmageddon#F   - skills improve by use and improve on failure, and branch when pre-requisite skills are met
#BSuccessTree#F  - skills improve by use and improve on success with increasingly higher difficulties, and branch when specific pre-requisites are met

What is your selection? ", (context, answers) => true,
				(text, context) =>
				{
					switch (text.ToLowerInvariant())
					{
						case "rpi":
						case "labmud":
						case "armageddon":
						case "successtree":
							return (true, string.Empty);
					}

					return (false, "That is not a valid selection.");
				}
			),
			("complexity",
				@"You can choose to have your skills be either #6simple#0 or #6complex#0. For example, the athletics skills generated if you choose either of the options would be as follows:

#6Simple#0

Athletics, Armour Use

#6Complex#0

Swimming, Running, Climbing, Flying, Falling, Armour Use

This follows a similar pattern across other areas as well - skills are either lumped together into simple categories or are more specific.

Please choose either #6simple#0 or #6complex#0: ", (context, answers) => true,
				(text, context) =>
				{
					switch (text.ToLowerInvariant())
					{
						case "simple":
						case "complex":
							return (true, string.Empty);
					}

					return (false, "That is not a valid selection.");
				}
			),
			("gerund",
				"Do you want to use mostly gerund (ing-ending) names for skills, where possible? For example, #2running, swimming, blacksmithing, foraging#0 vs #1run, swim, blacksmith, forage#0.\n\nPlease answer #3yes#F or #3no#F. ",
				(context, answers) => true, (text, context) =>
				{
					if (!text.EqualToAny("yes", "y", "no", "n")) return (false, "Please answer #3yes#F or #3no#F.");

					return (true, string.Empty);
				}),
			("modern",
				"Do you want to include modern crafting and professional skills, such as electronics or gunsmithing?\n\nPlease answer #3yes#F or #3no#F. ",
				(context, answers) => true, (text, context) =>
				{
					if (!text.EqualToAny("yes", "y", "no", "n")) return (false, "Please answer #3yes#F or #3no#F.");

					return (true, string.Empty);
				}),
			("extendedprofessional",
				"Do you want to create skills for non-functional, non-crafting professional skills - for example labouring, lawyering, engineering etc? You might want to do this if you are making the kid of MUD where people with those skills can get paid for those skills by getting a job. Otherwise, you may wish to leave them out as nobody likes a wasted skill pick.\n\nPlease answer #3yes#F or #3no#F. ",
				(context, answers) => true, (text, context) =>
				{
					if (!text.EqualToAny("yes", "y", "no", "n")) return (false, "Please answer #3yes#F or #3no#F.");

					return (true, string.Empty);
				})
		};

	public record SkillDetails(string GerundName, string ImperativeName, string Group, string Formula,
		string Decorator, string Improver, bool
			Available, double BranchMultiplier, string HelpFile = "");

	private IEnumerable<SkillDetails> AthleticSkills =>
		new[]
		{
			new SkillDetails("Swimming", "Swim", "Athletic", "min(99,2*str + 3*con)", "General", "General", true, 1.0,
				@"The $0 skill covers the character's ability to swim. Swimming skill is necessary to move about in water and higher skills improve both speed of movement and reduce stamina use while swimming."),
			new SkillDetails("Running", "Run", "Athletic", "min(99,2*agi + 3*con)", "General", "General", true, 1.0,
				@"The $0 skill covers running, sprinting, fleeing and many forms of moving hastily. Running is checked when you try to flee from combat and higher values slightly reduce stamina use when running."),
			new SkillDetails("Climbing", "Climb", "Athletic", "min(99,1*str + 2*con + 2*agi)", "General", "General",
				true, 1.0,
				@"The $0 skill covers climbing trees, walls, cliffs and other vertical surfaces. The higher the climbing skill the more successful you will be at climbing, particularly in poor conditions like bad weather."),
			new SkillDetails("Flying", "Fly", "Athletic", "min(99,2*agi + 3*con)", "General", "General", false, 1.0,
				@"The $0 skill is used when you fly and its main purpose is to reduce the stamina expenditure associated with flying."),
			new SkillDetails("Falling", "Fall", "Athletic", "min(99,3*agi + 2*wil)", "General", "General", true, 1.0,
				@"The $0 skill covers how good you are at falling safely and minimising the damage when you hit the ground at the end of the fall."),
			new SkillDetails("Crutch Walking", "Crutch Use", "Athletic", "min(99,3*str + 2*con)", "General", "General",
				true, 1.0,
				@"The $0 skill covers your ability to walk with crutches when one of your legs is injured. The higher your skill, the less extra stamina this will take and the faster you will move."),
			new SkillDetails("Enduring", "Endurance", "Athletic", "min(99,3.5*con + 1.5*wil)", "General", "General",
				true, 1.0,
				@"The $0 skill covers your ability to endure punishment and bounce back, especially things that could stun or disorient you."),
			new SkillDetails("Carrying", "Armour Use", "Athletic", "min(99,3*str + 2*con)", "General", "General", true,
				1.0,
				@"The $0 skill covers your ability to move around while carrying heavy loads, such as a full set of armour, gear, or similar. The higher this skill, the less stamina you will use when moving while encumbered and the higher your encumbrance limits."),
			new SkillDetails("Riding", "Ride", "Athletic", "min(99,3*agi + 2*con)", "General", "General", true,
				1.0,
				@"The $0 skill covers your ability to handle and ride animal mounts.")
		};

	private IEnumerable<SkillDetails> BroadAthleticSkills =>
		new[]
		{
			new SkillDetails("Athletics", "Athletics", "Athletic", "min(99,1*str + 1*agi + 3*con)", "General",
				"General", true, 1.0,
				@"The $0 skill covers a broad range of athletic activities such as climbing, swimming, running and flying (if your race can do any of those). Generally speaking higher values will make you more successful, faster and use less stamina when doing these things."),
			new SkillDetails("Carrying", "Armour Use", "Athletic", "min(99,3*str + 2*con)", "General", "General", true,
				1.0,
				@"The $0 skill covers your ability to move around while carrying heavy loads, such as a full set of armour, gear, or similar. The higher this skill, the less stamina you will use when moving while encumbered and the higher your encumbrance limits.")
		};

	private IEnumerable<SkillDetails> PerceptionSkills =>
		new[]
		{
			new SkillDetails("Spotting", "Spot", "Perception", "min(99,5*per)", "General", "General", true, 1.0,
				@"The $0 skill is used to see hidden things and notice subtle actions that take place around you."),
			new SkillDetails("Listening", "Listen", "Perception", "min(99,5*per)", "General", "General", true, 1.0,
				@"The $0 skill is used to hear quiet things and also to successfully hear language in noisy environments."),
			new SkillDetails("Searching", "Search", "Perception", "min(99,4*per + 1*int)", "General", "General", true,
				1.0, @"The $0 skill is used specifically when you search for hidden things in your room."),
			new SkillDetails("Tracking", "Track", "Perception", "min(99,4*per + 1*int)", "General", "General", true,
				1.0,
				@"The $0 skill is used to search your area for signs of people and animals moving through the area.")
		};

	private IEnumerable<SkillDetails> BroadPerceptionSkills =>
		new[]
		{
			new SkillDetails("Perception", "Perception", "Perception", "min(99,5*per)", "General", "General", true, 1.0,
				@"The $0 skill is used as a counteraction to stealth actions, as well as to see and hear subtle or quiet things."),
			new SkillDetails("Tracking", "Track", "Perception", "min(99,4*per + 1*int)", "General", "General", true,
				1.0,
				@"The $0 skill is used to search your area for signs of people and animals moving through the area.")
		};

	private IEnumerable<SkillDetails> MedicalSkills =>
		new[]
		{
			new SkillDetails("Surgery", "Surgery", "Medical", "min(99,2*dex + 3*int)", "General", "General", true, 1.0,
				@"The $0 skill is used to undertake surgical procedures; amputations, resection of damaged organs, internal fixation of bones and the like."),
			new SkillDetails("Diagnosis", "Diagnosis", "Medical", "min(99,2*per + 3*int)", "General", "General", true,
				1.0,
				@"The $0 skill is used to triage patients and figure out what is wrong with them, as well as undertake examinations like physicals."),
			new SkillDetails("First Aid", "First Aid", "Medical", "min(99,2*per + 2*int + 1*wil)", "General", "General",
				true, 1.0,
				"The $0 skill is used for treating trauma in non-hospital settings, such as binding bleeding wounds, performing CPR, and removing lodged items."),
			new SkillDetails("Patient Care", "Patient Care", "Medical", "min(99, 4*int + 1*wil)", "General", "General",
				true, 1.0,
				@"The $0 skill is used by doctors and nurses to perform routine procedures and provide patient aftercare, such as cannulation/decannulation, suturing, cleaning wounds, and otherwise tending wounds."),
			new SkillDetails("Pharmacology", "Pharmacology", "Medical", "min(99, 4*int + 1*per)", "General", "General",
				true, 1.0,
				@"The $0 skill is used to administer medications to a patient, estimate doses and notice potential side effects.")
		};

	private IEnumerable<SkillDetails> SimpleMedicalSkills =>
		new[]
		{
			new SkillDetails("Medicine", "Medicine", "Medical", "min(99,2*dex + 3*int)", "General", "General", true,
				1.0,
				@"The $0 skill is a catch-all medical skill used to perform surgery, treat injured people and provide all other forms of medical care.")
		};

	private IEnumerable<SkillDetails> StealthSkills =>
		new[]
		{
			new SkillDetails("Hiding", "Hide", "Stealth", "min(99,3*agi + 1*int + 1*per)", "General", "General", true,
				1.0,
				@"The $0 skill is used when you try to hide yourself or an object, and the results of this skill check constitute the difficulty for a potential noticer."),
			new SkillDetails("Sneaking", "Sneak", "Stealth", "min(99,3*agi + 1*int + 1*per)", "General", "General",
				true, 1.0,
				@"The $0 skill is used when attempting to move about stealthily, and the results of this skill check constitute the difficulty for a potential noticer."),
			new SkillDetails("Palming", "Palm", "Stealth", "min(99,3*dex + 1*int + 1*per)", "General", "General", true,
				1.0,
				@"The $0 skill is used to subtly manipulate your inventory, such as taking something out of a container, putting it into a container, dropping it or the like."),
			new SkillDetails("Lockpicking", "Pick Locks", "Stealth", "min(99,3*dex + 1*int + 1*per)", "General",
				"General", true, 1.0, @"The $0 skill is used to unlock, lock and otherwise manipulate locks."),
			new SkillDetails("Stealing", "Steal", "Stealth", "min(99,3*dex + 1*int + 1*per)", "General", "General", true,
				1.0,
				@"The $0 skill is used to pick people's pockets, plant items on other people and other illegal activities."),
		};

	private IEnumerable<SkillDetails> SimpleStealthSkills =>
		new[]
		{
			new SkillDetails("Stealth", "Stealth", "Stealth", "min(99,3*agi + 1*int + 1*per)", "General", "General",
				true, 1.0,
				@"The $0 skill is used to hide, sneak and subtly hide your actions. Uses of this skill are always opposed by a target's perception."),
			new SkillDetails("Security", "Security", "Stealth", "min(99,3*dex + 1*int + 1*per)", "General", "General",
				true, 1.0,
				@"The $0 skill is used for a variety of nefarious security related actions, such as picking locks.")
		};

	private IEnumerable<SkillDetails> UniversalCraftSkills =>
		new[]
		{
			new SkillDetails("Foraging", "Forage", "Survival", "min(99,3*int + 2*per)", "Crafting", "General", true,
				1.0),
			new SkillDetails("Skinning", "Skin", "Survival", "min(99,4*dex + 1*per)", "Crafting", "General", true, 1.0),
			new SkillDetails("Butchering", "Butcher", "Crafting", "min(99,3*dex + 2*str)", "Crafting", "General", true,
				1.0),
			new SkillDetails("Salvaging", "Salvage", "Survival", "min(99,3*int + 2*per)", "Crafting", "General", true,
				1.0)
		};

	private IEnumerable<SkillDetails> SimpleUniversalCraftSkills =>
		new[]
		{
			new SkillDetails("Surviving", "Survival", "Survival", "min(99,3*int + 2*per)", "Crafting", "General", true,
				1.0)
		};

	private IEnumerable<SkillDetails> FunctionalCraftSkills =>
		new[]
		{
			new SkillDetails("Armourcrafting", "Armourer", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General",
				true, 1.0),
			new SkillDetails("Weaponcrafting", "Weaponsmith", "Crafting", "min(99,3*int + 2*wil)", "Crafting",
				"General", true, 1.0),
			new SkillDetails("Blacksmithing", "Blacksmith", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General",
				true, 1.0),
			new SkillDetails("Silversmithing", "Silversmith", "Crafting", "min(99,3*int + 2*wil)", "Crafting",
				"General", true, 1.0),
			new SkillDetails("Bowmaking", "Bowyer", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General", true,
				1.0),
			new SkillDetails("Fletching", "Fletcher", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General", true,
				1.0),
			new SkillDetails("Pottery", "Potter", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General", true,
				1.0),
			new SkillDetails("Weaving", "Weaver", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General", true,
				1.0),
			new SkillDetails("Threshing", "Thresher", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General", true,
				1.0),
			new SkillDetails("Milling", "Miller", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General", true,
				1.0),
			new SkillDetails("Baking", "Baker", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General", true, 1.0),
			new SkillDetails("Dyeing", "Dyer", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General", true, 1.0),
			new SkillDetails("Glassworking", "Glazier", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General", true, 1.0),
			new SkillDetails("Gemcraft", "Jeweller", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General", true, 1.0),
			new SkillDetails("Perfumery", "Perfumer", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General", true, 1.0),
			new SkillDetails("Brewing", "Brewer", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General", true,
				1.0),
			new SkillDetails("Distilling", "Distiller", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General",
				true, 1.0),
			new SkillDetails("Cooking", "Cook", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General", true, 1.0),
			new SkillDetails("Carpentry", "Carpenter", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General", true,
				1.0),
			new SkillDetails("Lumberjacking", "Lumberjack", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General",
				true, 1.0),
			new SkillDetails("Masonry", "Mason", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General", true, 1.0),
			new SkillDetails("Scrimshawing", "Scrimshaw", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General",
				true, 1.0),
			new SkillDetails("Tailoring", "Tailor", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General", true,
				1.0),
			new SkillDetails("Mechanics", "Mechanic", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General", true,
				1.0),
			new SkillDetails("Cobbling", "Cobbler", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General", true,
				1.0),
			new SkillDetails("Locksmithing", "Locksmith", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General",
				true, 1.0),
			new SkillDetails("Wheelmaking", "Wheelwright", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General",
				true, 1.0),
			new SkillDetails("Candlemaking", "Chandler", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General",
				true, 1.0),
			new SkillDetails("Leathermaking", "Tanner", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General",
				true, 1.0),
			new SkillDetails("Winemaking", "Winemaker", "Crafting", "min(99,3*int + 2*wil)", "Crafting", "General",
				true, 1.0)
		};

	private IEnumerable<SkillDetails> ModernCraftSkills =>
		new[]
		{
			new SkillDetails("Chemistry", "Chemist", "Crafting", "min(99,3*int + 2*wil)", "Crafting",
				"General", true, 1.0),
			new SkillDetails("Programming", "Programmer", "Crafting", "min(99,3*int + 2*wil)", "Crafting",
				"General", true, 1.0),
			new SkillDetails("Electronics", "Electronics", "Crafting", "min(99,3*int + 2*wil)", "Crafting",
				"General", true, 1.0),
			new SkillDetails("Electrician", "Electrician", "Crafting", "min(99,3*int + 2*wil)", "Crafting",
				"General", true, 1.0),
			new SkillDetails("Machine Operating", "Machining", "Crafting", "min(99,3*int + 2*wil)", "Crafting",
				"General", true, 1.0),
			new SkillDetails("Gunmaking", "Gunmaker", "Crafting", "min(99,3*int + 2*wil)", "Crafting",
				"General", true, 1.0),
			new SkillDetails("Munitioning", "Munitioner", "Crafting", "min(99,3*int + 2*wil)", "Crafting",
				"General", true, 1.0)
		};

	private IEnumerable<SkillDetails> UniversalProfessionalSkills =>
		new[]
		{
			new SkillDetails("Grooming", "Groom", "Professional", "min(99,3*dex + 2*str)", "Professional", "General",
				true, 1.0),
			new SkillDetails("Constructing", "Construction", "Professional", "min(99,3*con + 2*int)", "Professional",
				"General", true, 1.0),
			new SkillDetails("Tattooing", "Tattoo", "Professional", "min(99,4*dex + 1*per)", "Professional", "General",
				true, 1.0),
			new SkillDetails("Herbalism", "Herbalist", "Professional", "min(99,5*int)", "Professional", "General", true,
				1.0),
			new SkillDetails("Farming", "Farmer", "Professional", "min(99,5*int)", "Professional", "General", true,
				1.0),
			new SkillDetails("Mining", "Miner", "Professional", "min(99,5*int)", "Professional", "General", true, 1.0),
			new SkillDetails("Smelting", "Smelter", "Professional", "min(99,5*int)", "Professional", "General", true,
				1.0),
			new SkillDetails("Literacy", "Literacy", "Professional", "min(99,5*int)", "Professional", "General", true,
				1.0),
			new SkillDetails("Handwriting", "Handwriting", "Professional", "min(99,5*int)", "Professional", "General",
				true, 1.0)
		};

	private IEnumerable<SkillDetails> ProfessionalSkills =>
		new[]
		{
			new SkillDetails("Animal Breeding", "Husbandry", "Professional", "min(99,5*int)", "Professional", "General",
				true, 1.0),
			new SkillDetails("Beekeeping", "Beekeeper", "Professional", "min(99,5*int)", "Professional", "General",
				true, 1.0),
			new SkillDetails("Law", "Lawyer", "Professional", "min(99,5*int)", "Professional", "General", true, 1.0),
			new SkillDetails("Labouring", "Labourer", "Professional", "min(99,3*con + 2*wil)", "Professional",
				"General", true, 1.0),
			new SkillDetails("Supervising", "Supervisor", "Professional", "min(99,5*int)", "Professional", "General",
				true, 1.0),
			new SkillDetails("Painting", "Paint", "Professional", "min(99,5*int)", "Professional", "General", true,
				1.0),
			new SkillDetails("Civil Engineering", "Civil Engineer", "Professional", "min(99,5*int)", "Professional",
				"General", true, 1.0),
			new SkillDetails("Administration", "Administrator", "Professional", "min(99,5*int)", "Professional",
				"General", true, 1.0),
			new SkillDetails("Performance", "Performer", "Professional", "min(99,5*int)", "Professional", "General",
				true, 1.0),
			new SkillDetails("Investigation", "Investigator", "Professional", "min(99,5*int)", "Professional",
				"General", true, 1.0)
		};

	/// <inheritdoc />
	public override string SeedData(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		var templates = SeedCheckTemplates(context, questionAnswers["branching"].EqualToAny("yes", "y"));
		var (general, crafting, languageDecorator, veterancy, professional, languageImprover, generalImprover) =
			SeedSkillImprovers(context, questionAnswers["skillgainmodel"].ToLowerInvariant());
		var decorators = new Dictionary<string, TraitDecorator>(StringComparer.OrdinalIgnoreCase)
		{
			{ "general", general },
			{ "crafting", crafting },
			{ "language", languageDecorator },
			{ "professional", professional },
			{ "veterancy", veterancy }
		};
		var skills = SeedSkills(context, questionAnswers, decorators, generalImprover);
		SeedAdminLanguage(context, questionAnswers, decorators, languageImprover);
		SeedChecks(context, questionAnswers, templates, skills);
		return string.Empty;
	}

	private void SeedChecks(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers,
		IReadOnlyDictionary<string, CheckTemplate> templates, IReadOnlyDictionary<string, TraitDefinition> skills)
	{
		var attributes = context.TraitDefinitions.Where(x => x.Type == 1 || x.Type == 3)
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		var strAttribute =
			attributes.GetValueOrDefault("Strength") ??
			attributes.GetValueOrDefault("Physique") ??
			attributes["Body"];
		var conAttribute =
			attributes.GetValueOrDefault("Constitution") ??
			attributes.GetValueOrDefault("Physique") ??
			attributes.GetValueOrDefault("Endurance") ??
			attributes["Body"];
		var dexAttribute =
			attributes.GetValueOrDefault("Dexterity") ??
			attributes.GetValueOrDefault("Agility") ??
			attributes.GetValueOrDefault("Speed") ??
			attributes["Body"];
		var agiAttribute =
			attributes.GetValueOrDefault("Agility") ??
			attributes.GetValueOrDefault("Dexterity") ??
			attributes.GetValueOrDefault("Agility") ??
			attributes.GetValueOrDefault("Speed") ??
			attributes["Body"];
		var intAttribute =
			attributes.GetValueOrDefault("Intelligence") ??
			attributes.GetValueOrDefault("Wisdom") ??
			attributes["Intellect"];
		var perAttribute =
			attributes.GetValueOrDefault("Perception") ??
			attributes.GetValueOrDefault("Intelligence") ??
			attributes.GetValueOrDefault("Wisdom") ??
			attributes["Intellect"];
		var wilAttribute =
			attributes.GetValueOrDefault("Willpower") ??
			attributes.GetValueOrDefault("Wisdom") ??
			attributes["Spirit"];

		void AddCheck(CheckType type, TraitExpression expression, long templateId,
			Difficulty maximumImprovementDifficulty)
		{
			expression.Name = type == CheckType.None ? "Default Fallback Check" : $"{type.DescribeEnum(true)} Formula";
			context.TraitExpressions.Add(expression);
			context.SaveChanges();
			context.Checks.Add(new Check
			{
				Type = (int)type,
				CheckTemplateId = templateId,
				MaximumDifficultyForImprovement = (int)maximumImprovementDifficulty,
				TraitExpression = expression
			});
			context.SaveChanges();
		}

		var surgeryTrait = skills.GetValueOrDefault("Surgery") ??
						   skills["Medicine"];
		var patientCareTrait = skills.GetValueOrDefault("Patient Care") ??
							   skills["Medicine"];
		var firstAidTrait = skills.GetValueOrDefault("First Aid") ??
							skills["Medicine"];
		var diagnoseTrait = skills.GetValueOrDefault("Diagnosis") ??
							skills["Medicine"];
		var listenTrait = skills.GetValueOrDefault("Listen") ??
						  skills["Perception"];
		var spotTrait = skills.GetValueOrDefault("Spot") ??
						skills["Perception"];
		var searchTrait = skills.GetValueOrDefault("Search") ??
						  skills["Perception"];
		var hideTrait = skills.GetValueOrDefault("Hide") ??
						skills["Stealth"];
		var sneakTrait = skills.GetValueOrDefault("Sneak") ??
						 skills["Stealth"];
		var palmTrait = skills.GetValueOrDefault("Palm") ??
						skills["Stealth"];
		var lockpickTrait = skills.GetValueOrDefault("Pick Locks") ??
							skills["Security"];
		var swimTrait = skills.GetValueOrDefault("Sneak") ??
						skills["Athletics"];
		var runTrait = skills.GetValueOrDefault("Sneak") ??
					   skills["Athletics"];
		var climbTrait = skills.GetValueOrDefault("Sneak") ??
						 skills["Athletics"];
		var flyTrait = skills.GetValueOrDefault("Sneak") ??
					   skills["Athletics"];
		var fallTrait = skills.GetValueOrDefault("Sneak") ??
						skills["Athletics"];
		var crutchUseTrait = skills.GetValueOrDefault("Sneak") ??
							 skills["Athletics"];
		var forageTrait = skills.GetValueOrDefault("Forage") ??
						  skills["Survival"];
		var armourUseTrait = skills["Armour Use"];
		var trackTrait = skills.GetValueOrDefault("Track") ?? skills["Survival"];
		var lawTrait = skills.GetValueOrDefault("Law");

		foreach (var check in Enum.GetValues(typeof(CheckType)).OfType<CheckType>().Distinct().ToList())
			switch (check)
			{
				case CheckType.None:
					// Default Fall Back
					AddCheck(check, new TraitExpression { Expression = "variable" },
						templates["Skill Check No Improvement"].Id, Difficulty.Automatic);
					continue;
				case CheckType.ProjectLabourCheck:
					// Special Project Check
					AddCheck(check, new TraitExpression { Expression = "variable" }, templates["Project Check"].Id,
						Difficulty.Automatic);
					continue;
				case CheckType.HealingCheck:
					AddCheck(check,
						new TraitExpression { Expression = $"max(5.0, {conAttribute.Alias}:{conAttribute.Id}/2)" },
						templates["Health Check"].Id, Difficulty.Automatic);
					continue;
				case CheckType.StunRecoveryCheck:
					AddCheck(check,
						new TraitExpression
						{
							Expression =
								$"max(10.0, {conAttribute.Alias}:{conAttribute.Id} + {wilAttribute.Alias}:{wilAttribute.Id})"
						}, templates["Health Check"].Id, Difficulty.Automatic);
					continue;
				case CheckType.ShockRecoveryCheck:
					AddCheck(check,
						new TraitExpression
						{
							Expression =
								$"max(10.0, {conAttribute.Alias}:{conAttribute.Id} + {wilAttribute.Alias}:{wilAttribute.Id})"
						}, templates["Health Check"].Id, Difficulty.Automatic);
					continue;
				case CheckType.PainRecoveryCheck:
					AddCheck(check,
						new TraitExpression { Expression = $"max(10.0, {wilAttribute.Alias}:{wilAttribute.Id}*2)" },
						templates["Health Check"].Id, Difficulty.Automatic);
					continue;
				case CheckType.WoundCloseCheck:
					AddCheck(check, new TraitExpression { Expression = $"{conAttribute.Alias}:{conAttribute.Id}/10" },
						templates["Health Check"].Id, Difficulty.Automatic);
					continue;
				case CheckType.DreamCheck:
					AddCheck(check, new TraitExpression { Expression = "10" }, templates["Capability Check"].Id,
						Difficulty.Automatic);
					continue;
				case CheckType.GoToSleepCheck:
					AddCheck(check, new TraitExpression { Expression = "10" }, templates["Capability Check"].Id,
						Difficulty.Automatic);
					break;
				case CheckType.ExactTimeCheck:
					AddCheck(check, new TraitExpression { Expression = "0" }, templates["Capability Check"].Id,
						Difficulty.Automatic);
					continue;
				case CheckType.VagueTimeCheck:
					AddCheck(check, new TraitExpression { Expression = "0" }, templates["Capability Check"].Id,
						Difficulty.Automatic);
					continue;
				case CheckType.StyleCharacteristicCapabilityCheck:
					AddCheck(check, new TraitExpression { Expression = $"groom:{skills["Groom"].Id}+50" },
						templates["Capability Check"].Id, Difficulty.Automatic);
					continue;
				case CheckType.ImplantRecognitionCheck:
					AddCheck(check, new TraitExpression { Expression = $"60+surgery:{surgeryTrait.Id}" },
						templates["Capability Check"].Id, Difficulty.Automatic);
					continue;
				case CheckType.TreatmentItemRecognitionCheck:
					// Capability Checks
					AddCheck(check,
						new TraitExpression
						{ Expression = $"(care:{patientCareTrait.Id}+firstaid:{firstAidTrait.Id})*10" },
						templates["Capability Check"].Id, Difficulty.Automatic);
					continue;
				case CheckType.GenericAttributeCheck:
					AddCheck(check, new TraitExpression { Expression = "variable*5" },
						templates["Skill Check No Improvement"].Id, Difficulty.Automatic);
					continue;
				case CheckType.GenericSkillCheck:
					AddCheck(check, new TraitExpression { Expression = "variable" },
						templates["Skill Check No Improvement"].Id, Difficulty.Automatic);
					continue;
				case CheckType.GenericListenCheck:
					AddCheck(check, new TraitExpression { Expression = $"listen:{listenTrait.Id}+10" },
						templates["Perception Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.LanguageListenCheck:
					AddCheck(check, new TraitExpression { Expression = $"listen:{listenTrait.Id}+50" },
						templates["Perception Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.GenericSpotCheck:
					AddCheck(check, new TraitExpression { Expression = $"spot:{spotTrait.Id}+10" },
						templates["Perception Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.NoticeCheck:
					AddCheck(check, new TraitExpression { Expression = $"spot:{spotTrait.Id}+30" },
						templates["Perception Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.SpotSneakCheck:
					AddCheck(check,
						new TraitExpression
						{ Expression = $"(spot:{spotTrait.Id}*0.8)+(listen:{listenTrait.Id}*0.4)+10" },
						templates["Perception Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.ScanPerceptionCheck:
					AddCheck(check, new TraitExpression { Expression = $"spot:{spotTrait.Id}+10" },
						templates["Perception Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.QuickscanPerceptionCheck:
					AddCheck(check, new TraitExpression { Expression = $"spot:{spotTrait.Id}-10" },
						templates["Perception Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.LongscanPerceptionCheck:
					AddCheck(check, new TraitExpression { Expression = $"spot:{spotTrait.Id}+30" },
						templates["Perception Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.WatchLocation:
					AddCheck(check,
						new TraitExpression
						{ Expression = $"(spot:{spotTrait.Id}*0.8)+(listen:{listenTrait.Id}*0.4)+10" },
						templates["Perception Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.PassiveStealthCheck:
					AddCheck(check, new TraitExpression { Expression = $"search:{searchTrait.Id}-10" },
						templates["Perception Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.ActiveSearchCheck:
					AddCheck(check, new TraitExpression { Expression = $"search:{searchTrait.Id}+10" },
						templates["Perception Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.SpokenLanguageSpeakCheck:
				case CheckType.SpokenLanguageHearCheck:
					// Language Checks
					AddCheck(check, new TraitExpression { Expression = "variable" }, templates["Language Check"].Id,
						Difficulty.Impossible);
					break;
				case CheckType.AccentAcquireCheck:
				case CheckType.AccentImproveCheck:
					// Static Checks
					AddCheck(check, new TraitExpression { Expression = "2.0" }, templates["Static Check"].Id,
						Difficulty.Automatic);
					break;
				case CheckType.TraitBranchCheck:
					// Trait Branch Only
					AddCheck(check, new TraitExpression { Expression = "0.1" }, templates["Branch Check"].Id,
						Difficulty.Automatic);
					break;
				case CheckType.ProjectSkillUseAction:
					// Bonus-Absent Checks
					AddCheck(check, new TraitExpression { Expression = "variable" }, templates["Bonus Absent Check"].Id,
						Difficulty.Automatic);
					break;
				case CheckType.SpotStealthCheck:
					AddCheck(check,
						new TraitExpression
						{ Expression = $"(spot:{spotTrait.Id}*0.8)+(listen:{listenTrait.Id}*0.4)+10" },
						templates["Passive Perception Check"].Id, Difficulty.Impossible);
					break;
				case CheckType.HideCheck:
					AddCheck(check, new TraitExpression { Expression = $"hide:{hideTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.SneakCheck:
					AddCheck(check, new TraitExpression { Expression = $"sneak:{sneakTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.PalmCheck:
					AddCheck(check, new TraitExpression { Expression = $"palm:{palmTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.HideItemCheck:
					AddCheck(check, new TraitExpression { Expression = $"hide:{hideTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.UninstallDoorCheck:
					AddCheck(check, new TraitExpression { Expression = "variable" }, templates["Skill Check"].Id,
						Difficulty.Impossible);
					continue;
				case CheckType.SkillTeachCheck:
					AddCheck(check, new TraitExpression { Expression = $"int:{intAttribute.Id}*5" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.SkillLearnCheck:
					AddCheck(check, new TraitExpression { Expression = $"int:{intAttribute.Id}*5" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.KnowledgeTeachCheck:
					AddCheck(check, new TraitExpression { Expression = $"int:{intAttribute.Id}*5" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.KnowledgeLearnCheck:
					AddCheck(check, new TraitExpression { Expression = $"int:{intAttribute.Id}*5" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.ForageCheck:
					AddCheck(check, new TraitExpression { Expression = $"forage:{forageTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.ForageSpecificCheck:
					AddCheck(check, new TraitExpression { Expression = $"forage:{forageTrait.Id}-20" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.ForageTimeCheck:
					AddCheck(check, new TraitExpression { Expression = $"forage:{forageTrait.Id}+20" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.BindWoundCheck:
					AddCheck(check, new TraitExpression { Expression = $"firstaid:{firstAidTrait.Id}+30" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.SutureWoundCheck:
					AddCheck(check, new TraitExpression { Expression = $"firstaid:{firstAidTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.CleanWoundCheck:
					AddCheck(check, new TraitExpression { Expression = $"firstaid:{firstAidTrait.Id}+50" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.RemoveLodgedObjectCheck:
					AddCheck(check, new TraitExpression { Expression = $"firstaid:{firstAidTrait.Id}+10" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.MendCheck:
					AddCheck(check, new TraitExpression { Expression = $"con:{conAttribute.Id}*2" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.MeleeWeaponPenetrateCheck:
					AddCheck(check, new TraitExpression { Expression = "variable*0.2" }, templates["Skill Check"].Id,
						Difficulty.Impossible);
					continue;
				case CheckType.RangedWeaponPenetrateCheck:
					AddCheck(check, new TraitExpression { Expression = "variable*0.2" }, templates["Skill Check"].Id,
						Difficulty.Impossible);
					continue;
				case CheckType.PenetrationDefenseCheck:
					AddCheck(check, new TraitExpression { Expression = $"armour:{armourUseTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.CombatMoveCheck:
					AddCheck(check, new TraitExpression { Expression = "variable" }, templates["Skill Check"].Id,
						Difficulty.Impossible);
					continue;
				case CheckType.CombatRecoveryCheck:
					AddCheck(check,
						new TraitExpression { Expression = $"agi:{agiAttribute.Id}*4+str:{strAttribute.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.MedicalExaminationCheck:
					AddCheck(check, new TraitExpression { Expression = $"diag:{diagnoseTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.LocksmithingCheck:
					AddCheck(check, new TraitExpression { Expression = $"pick:{lockpickTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.NaturalWeaponAttack:
				case CheckType.DodgeCheck:
				case CheckType.ParryCheck:
				case CheckType.BlockCheck:
				case CheckType.FleeMeleeCheck:
				case CheckType.OpposeFleeMeleeCheck:
				case CheckType.Ward:
				case CheckType.WardDefense:
				case CheckType.WardIgnore:
				case CheckType.StartClinch:
				case CheckType.ResistClinch:
				case CheckType.BreakClinch:
				case CheckType.ResistBreakClinch:
				case CheckType.RescueCheck:
				case CheckType.OpposeRescueCheck:
				case CheckType.StaggeringBlowDefense:
				case CheckType.StruggleFreeFromDrag:
				case CheckType.OpposeStruggleFreeFromDrag:
				case CheckType.CounterGrappleCheck:
				case CheckType.StruggleFreeFromGrapple:
				case CheckType.OpposeStruggleFreeFromGrapple:
				case CheckType.ExtendGrappleCheck:
				case CheckType.InitiateGrapple:
				case CheckType.ScreechAttack:
				case CheckType.StrangleCheck:
				case CheckType.WrenchAttackCheck:
				case CheckType.TakedownCheck:
				case CheckType.BreakoutCheck:
				case CheckType.OpposeBreakoutCheck:
				case CheckType.TossItemCheck:
					// All of these checks are setup later in the combat seeder
					AddCheck(check, new TraitExpression { Expression = "50" }, templates["Skill Check"].Id,
						Difficulty.Impossible);
					continue;

				case CheckType.ExploratorySurgeryCheck:
					AddCheck(check, new TraitExpression { Expression = $"surgery:{surgeryTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.TriageCheck:
					AddCheck(check, new TraitExpression { Expression = $"diagnose:{diagnoseTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.AmputationCheck:
					AddCheck(check, new TraitExpression { Expression = $"surgery:{surgeryTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.ReplantationCheck:
					AddCheck(check, new TraitExpression { Expression = $"surgery:{surgeryTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.InvasiveProcedureFinalisation:
					AddCheck(check, new TraitExpression { Expression = $"surgery:{surgeryTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.TraumaControlSurgery:
					AddCheck(check, new TraitExpression { Expression = $"surgery:{surgeryTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.Defibrillate:
					AddCheck(check, new TraitExpression { Expression = $"firstaid:{firstAidTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.PerformCPR:
					AddCheck(check, new TraitExpression { Expression = $"firstaid:{firstAidTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.ArmourUseCheck:
					AddCheck(check, new TraitExpression { Expression = $"armouruse:{armourUseTrait.Id}+5" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.ReadTextImprovementCheck:
					AddCheck(check, new TraitExpression { Expression = $"literacy:{skills["Literacy"].Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.HandwritingImprovementCheck:
					AddCheck(check, new TraitExpression { Expression = $"literacy:{skills["Handwriting"].Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.CrutchWalking:
					AddCheck(check, new TraitExpression { Expression = $"literacy:{crutchUseTrait.Id}+10" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.OrganExtractionCheck:
					AddCheck(check, new TraitExpression { Expression = $"surgery:{surgeryTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.OrganTransplantCheck:
					AddCheck(check, new TraitExpression { Expression = $"surgery:{surgeryTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.CannulationProcedure:
					AddCheck(check,
						new TraitExpression
						{ Expression = $"max(surgery:{surgeryTrait.Id}, care:{patientCareTrait.Id})" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.DecannulationProcedure:
					AddCheck(check,
						new TraitExpression
						{ Expression = $"max(surgery:{surgeryTrait.Id}, care:{patientCareTrait.Id})" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.OrganStabilisationCheck:
					AddCheck(check, new TraitExpression { Expression = $"surgery:{surgeryTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.CraftOutcomeCheck:
					AddCheck(check, new TraitExpression { Expression = "variable" }, templates["Skill Check"].Id,
						Difficulty.Impossible);
					continue;
				case CheckType.CraftQualityCheck:
					AddCheck(check, new TraitExpression { Expression = "variable" }, templates["Skill Check"].Id,
						Difficulty.Impossible);
					continue;
				case CheckType.TendWoundCheck:
					AddCheck(check, new TraitExpression { Expression = $"care:{patientCareTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.RelocateBoneCheck:
					AddCheck(check, new TraitExpression { Expression = $"care:{patientCareTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.SurgicalSetCheck:
					AddCheck(check, new TraitExpression { Expression = $"surgery:{surgeryTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.RepairItemCheck:
					AddCheck(check, new TraitExpression { Expression = "variable" }, templates["Skill Check"].Id,
						Difficulty.Impossible);
					continue;
				case CheckType.InstallImplantSurgery:
					AddCheck(check, new TraitExpression { Expression = $"surgery:{surgeryTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.RemoveImplantSurgery:
					AddCheck(check, new TraitExpression { Expression = $"surgery:{surgeryTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.ConfigureImplantPowerSurgery:
					AddCheck(check, new TraitExpression { Expression = $"surgery:{surgeryTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.ButcheryCheck:
					AddCheck(check, new TraitExpression { Expression = "variable" }, templates["Skill Check"].Id,
						Difficulty.Impossible);
					continue;
				case CheckType.SkinningCheck:
					AddCheck(check, new TraitExpression { Expression = "variable" }, templates["Skill Check"].Id,
						Difficulty.Impossible);
					continue;
				case CheckType.ClimbCheck:
					AddCheck(check, new TraitExpression { Expression = $"climb:{climbTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.ConfigureImplantInterfaceSurgery:
					AddCheck(check, new TraitExpression { Expression = $"surgery:{surgeryTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.InkTattooCheck:
					AddCheck(check, new TraitExpression { Expression = $"tattoo:{skills["Tattoo"].Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.FallingImpactCheck:
					AddCheck(check, new TraitExpression { Expression = $"fall:{fallTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.ResistMagicChokePower:
					AddCheck(check, new TraitExpression { Expression = $"wil:{wilAttribute.Id}*5" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.ResistMagicAnesthesiaPower:
					AddCheck(check, new TraitExpression { Expression = $"wil:{wilAttribute.Id}*5" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.SwimmingCheck:
					AddCheck(check, new TraitExpression { Expression = $"swim:{swimTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.AvoidFallDueToWind:
					AddCheck(check, new TraitExpression { Expression = $"climb:{climbTrait.Id}+5" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.SwimStayAfloatCheck:
					AddCheck(check, new TraitExpression { Expression = $"swim:{swimTrait.Id}+5" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.FlyCheck:
					AddCheck(check, new TraitExpression { Expression = $"fly:{flyTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.CheatAtDiceCheck:
					AddCheck(check, new TraitExpression { Expression = $"palm:{palmTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.EvaluateDiceFairnessCheck:
					AddCheck(check, new TraitExpression { Expression = $"palm:{palmTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.SpillLiquidOnPerson:
					AddCheck(check, new TraitExpression { Expression = $"dex:{dexAttribute.Id}*5" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.DodgeSpillLiquidOnPerson:
					AddCheck(check, new TraitExpression { Expression = $"agi:{agiAttribute.Id}*5" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.DrawingImprovementCheck:
					AddCheck(check, new TraitExpression { Expression = "variable" }, templates["Skill Check"].Id,
						Difficulty.Impossible);
					continue;


				case CheckType.MeleeWeaponCheck:
				case CheckType.ThrownWeaponCheck:
				case CheckType.AimRangedWeapon:
				case CheckType.LoadMusket:
				case CheckType.UnjamGun:
				case CheckType.FireBow:
				case CheckType.FireCrossbow:
				case CheckType.FireFirearm:
				case CheckType.FireSling:
				case CheckType.KeepAimTargetMoved:
				case CheckType.ProgSkillUseCheck:
				case CheckType.MagicConcentrationOnWounded:
				case CheckType.ConnectMindPower:
				case CheckType.PsychicLanguageHearCheck:
				case CheckType.MindSayPower:
				case CheckType.MindBroadcastPower:
				case CheckType.MagicTelepathyCheck:
				case CheckType.MindLookPower:
				case CheckType.InvisibilityPower:
				case CheckType.MagicArmourPower:
				case CheckType.MagicAnesthesiaPower:
				case CheckType.MagicSensePower:
				case CheckType.MagicChokePower:
				case CheckType.MindAuditPower:
				case CheckType.MindBarrierPowerCheck:
				case CheckType.MindExpelPower:
				case CheckType.CastSpellCheck:
				case CheckType.ResistMagicSpellCheck:
				case CheckType.AuxiliaryMoveCheck:
					// Variable skills
					AddCheck(check, new TraitExpression { Expression = "variable" }, templates["Skill Check"].Id,
						Difficulty.Impossible);
					break;
				case CheckType.WritingComprehendCheck:
					AddCheck(check, new TraitExpression { Expression = $"variable + lit:{skills["Literacy"].Id}" }, templates["Capability Check"].Id,
						Difficulty.Impossible);
					break;
				case CheckType.InfectionHeartbeat:
					AddCheck(check, new TraitExpression { Expression = $"con:{conAttribute.Id}*5" },
						templates["Skill Check No Improvement"].Id, Difficulty.Automatic);
					continue;
				case CheckType.InfectionSpread:
					AddCheck(check, new TraitExpression { Expression = $"con:{conAttribute.Id}*5" },
						templates["Skill Check No Improvement"].Id, Difficulty.Automatic);
					continue;
				case CheckType.ReplantedBodypartRejectionCheck:
					AddCheck(check, new TraitExpression { Expression = $"con:{conAttribute.Id}*5" },
						templates["Skill Check No Improvement"].Id, Difficulty.Automatic);
					continue;
				case CheckType.StyleCharacteristicCheck:
					AddCheck(check, new TraitExpression { Expression = $"groom:{skills["Groom"].Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.AppraiseItemCheck:
					AddCheck(check, new TraitExpression { Expression = $"int:{intAttribute.Id}*5" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.ProsecuteLegalCase:
					AddCheck(check, new TraitExpression
						{
							Expression = 
								lawTrait is not null ?
								$"law:{lawTrait.Id}" :
								"50"
						},
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.DefendLegalCase:
					AddCheck(check, new TraitExpression {
							Expression =
								lawTrait is not null ?
									$"law:{lawTrait.Id}" :
									"50"
						},
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.ClimbTreetoTreeCheck:
					AddCheck(check, new TraitExpression { Expression = $"climb:{climbTrait.Id}+10" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.CheatAtCoinFlip:
					AddCheck(check, new TraitExpression { Expression = $"palm:{palmTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.SearchForTracksCheck:
					AddCheck(check, new TraitExpression { Expression = $"track:{trackTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				case CheckType.SearchForTracksByScentScheck:
					AddCheck(check, new TraitExpression { Expression = $"track:{trackTrait.Id}" },
						templates["Skill Check"].Id, Difficulty.Impossible);
					continue;
				default:
					throw new ArgumentOutOfRangeException();
			}

		context.SaveChanges();
	}

	private void SeedAdminLanguage(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers, IReadOnlyDictionary<string, TraitDecorator> decorators,
		Improver languageImprover)
	{
		var attributes = context.TraitDefinitions.Where(x => x.Type == 1 || x.Type == 3)
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		var intAttribute =
			attributes.GetValueOrDefault("Intelligence") ??
			attributes.GetValueOrDefault("Wisdom") ??
			attributes["Intellect"];
		var alwaysTrue = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");
		var alwaysFalse = context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse");
		var now = DateTime.UtcNow;

		var cap = new TraitExpression
		{
			Name = "Admin Speech Skill Cap",
			Expression = ""
		};
		switch (questionAnswers["skillcapmodel"].ToLowerInvariant())
		{
			case "rpi":
				cap.Expression = $"10 + 9.5 * {intAttribute.Alias}:{intAttribute.Id}";
				break;
			case "class":
				cap.Expression = "200";
				break;
			case "flat":
				cap.Expression = "200";
				break;
			default:
				goto case "rpi";
		}

		context.TraitExpressions.Add(cap);
		context.SaveChanges();

		var skill = new TraitDefinition
		{
			Name = "Admin Speech",
			Type = 0,
			DecoratorId = decorators["Language"].Id,
			TraitGroup = "Language",
			DerivedType = 0,
			ExpressionId = cap.Id,
			ImproverId = languageImprover.Id,
			Hidden = false,
			ChargenBlurb = string.Empty,
			BranchMultiplier = 0.1,
			TeachDifficulty = 7,
			LearnDifficulty = 7,
			AvailabilityProg = alwaysFalse,
			LearnableProg = alwaysTrue,
			TeachableProg = alwaysFalse
		};
		context.TraitDefinitions.Add(skill);
		context.SaveChanges();
		var language = new Language
		{
			Name = "Admin Speech",
			LinkedTrait = skill,
			UnknownLanguageDescription = "an unknown language",
			LanguageObfuscationFactor = 0.1,
			DifficultyModel = context.LanguageDifficultyModels.First().Id
		};
		context.Languages.Add(language);
		context.SaveChanges();
		var accent = new Accent
		{
			Name = "native",
			Language = language,
			Suffix = "with a native accent",
			VagueSuffix = "with a native accent",
			Difficulty = (int)Difficulty.Normal,
			Description = "This is the accent of a native speaker who grew up learning the language",
			Group = "native"
		};
		context.Accents.Add(accent);
		accent = new Accent
		{
			Name = "foreign",
			Language = language,
			Suffix = "with a foreign accent",
			VagueSuffix = "with a foreign accent",
			Difficulty = (int)Difficulty.Normal,
			Description = "This is the accent of a non-native speaker who is just beginning to learn the language",
			Group = "foreign"
		};
		context.Accents.Add(accent);
		language.DefaultLearnerAccent = accent;
		context.SaveChanges();
	}

	protected static Regex AttributeRegex = new("int|wil|per|agi|dex|str|con", RegexOptions.IgnoreCase);

	protected IReadOnlyDictionary<string, TraitDefinition> SeedSkills(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers, IReadOnlyDictionary<string, TraitDecorator> decorators,
		Improver generalImprover)
	{
		var skills = new Dictionary<string, TraitDefinition>(StringComparer.OrdinalIgnoreCase);
		var simple = questionAnswers["complexity"].EqualTo("simple");
		var gerund = questionAnswers["gerund"].EqualToAny("yes", "y");

		var attributes = context.TraitDefinitions.Where(x => x.Type == 1 || x.Type == 3)
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		var strAttribute =
			attributes.GetValueOrDefault("Strength") ??
			attributes.GetValueOrDefault("Physique") ??
			attributes["Body"];
		var conAttribute =
			attributes.GetValueOrDefault("Constitution") ??
			attributes.GetValueOrDefault("Physique") ??
			attributes.GetValueOrDefault("Endurance") ??
			attributes["Body"];
		var dexAttribute =
			attributes.GetValueOrDefault("Dexterity") ??
			attributes.GetValueOrDefault("Agility") ??
			attributes.GetValueOrDefault("Speed") ??
			attributes["Body"];
		var agiAttribute =
			attributes.GetValueOrDefault("Agility") ??
			attributes.GetValueOrDefault("Dexterity") ??
			attributes.GetValueOrDefault("Agility") ??
			attributes.GetValueOrDefault("Speed") ??
			attributes["Body"];
		var intAttribute =
			attributes.GetValueOrDefault("Intelligence") ??
			attributes.GetValueOrDefault("Wisdom") ??
			attributes["Intellect"];
		var perAttribute =
			attributes.GetValueOrDefault("Perception") ??
			attributes.GetValueOrDefault("Intelligence") ??
			attributes.GetValueOrDefault("Wisdom") ??
			attributes["Intellect"];
		var wilAttribute =
			attributes.GetValueOrDefault("Willpower") ??
			attributes.GetValueOrDefault("Wisdom") ??
			attributes["Spirit"];

		var alwaysTrue = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");
		var alwaysFalse = context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse");
		var now = DateTime.UtcNow;

		string ReplaceAttributeReferences(string traitFormula)
		{
			return AttributeRegex.Replace(traitFormula, m =>
			{
				switch (m.Groups[0].Value.ToLowerInvariant())
				{
					case "str":
						return $"{strAttribute!.Alias}:{strAttribute.Id}";
					case "dex":
						return $"{dexAttribute!.Alias}:{dexAttribute.Id}";
					case "agi":
						return $"{agiAttribute!.Alias}:{agiAttribute.Id}";
					case "con":
						return $"{conAttribute!.Alias}:{conAttribute.Id}";
					case "int":
						return $"{intAttribute!.Alias}:{intAttribute.Id}";
					case "wil":
						return $"{wilAttribute!.Alias}:{wilAttribute.Id}";
					case "per":
						return $"{perAttribute!.Alias}:{perAttribute.Id}";
					default:
						return m.Groups[0].Value;
				}
			});
		}

		void AddSkill(SkillDetails details)
		{
			var cap = new TraitExpression
			{
				Name = $"{(gerund ? details.GerundName : details.ImperativeName)} Skill Cap",
				Expression = ""
			};
			switch (questionAnswers["skillcapmodel"].ToLowerInvariant())
			{
				case "rpi":
					cap.Expression = ReplaceAttributeReferences(details.Formula);
					break;
				case "class":
					cap.Expression = "70 + ({example class=wizard,fighter} * 30)";
					break;
				case "flat":
					cap.Expression = "70";
					break;
				default:
					goto case "rpi";
			}

			context.TraitExpressions.Add(cap);
			context.SaveChanges();

			var skill = new TraitDefinition
			{
				Name = gerund ? details.GerundName : details.ImperativeName,
				Type = 0,
				DecoratorId = decorators[details.Decorator].Id,
				TraitGroup = details.Group,
				DerivedType = 0,
				ExpressionId = cap.Id,
				ImproverId = generalImprover.Id,
				Hidden = false,
				ChargenBlurb = string.Empty,
				BranchMultiplier = details.BranchMultiplier,
				TeachDifficulty = 7,
				LearnDifficulty = 7,
				AvailabilityProg = details.Available ? alwaysTrue : alwaysFalse,
				LearnableProg = alwaysTrue,
				TeachableProg = alwaysFalse
			};
			context.TraitDefinitions.Add(skill);
			context.SaveChanges();
			skills[details.ImperativeName] = skill;

			if (!string.IsNullOrEmpty(details.HelpFile))
			{
				var help = new Helpfile
				{
					Name = skill.Name,
					Category = "Skills",
					Subcategory = details.Group,
					TagLine = $"Help for the {skill.Name} skill",
					PublicText = details.HelpFile.Replace("$0", skill.Name),
					Keywords = $"skill {skill.Name}",
					LastEditedBy = "System",
					LastEditedDate = now
				};
				context.Helpfiles.Add(help);
				context.SaveChanges();
			}
		}

		if (simple)
			foreach (var skill in
					 BroadAthleticSkills
						 .Concat(BroadPerceptionSkills)
						 .Concat(SimpleStealthSkills)
						 .Concat(SimpleMedicalSkills)
						 .Concat(SimpleUniversalCraftSkills)
						 .Concat(FunctionalCraftSkills)
						 .Concat(UniversalProfessionalSkills)
					)
				AddSkill(skill);
		else
			foreach (var skill in
					 AthleticSkills
						 .Concat(PerceptionSkills)
						 .Concat(StealthSkills)
						 .Concat(MedicalSkills)
						 .Concat(UniversalCraftSkills)
						 .Concat(FunctionalCraftSkills)
						 .Concat(UniversalProfessionalSkills)
					)
				AddSkill(skill);

		if (questionAnswers["modern"].EqualToAny("yes", "y"))
			foreach (var skill in ModernCraftSkills)
				AddSkill(skill);

		if (questionAnswers["extendedprofessional"].EqualToAny("yes", "y"))
			foreach (var skill in ProfessionalSkills)
				AddSkill(skill);

		return skills;
	}

	/// <inheritdoc />
	public override ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.Accounts.Any() || context.TraitDefinitions.All(x => x.Type != 1))
			return ShouldSeedResult.PrerequisitesNotMet;

		if (context.TraitDefinitions.Any(x => x.Type == 0)) return ShouldSeedResult.MayAlreadyBeInstalled;

		return ShouldSeedResult.ReadyToInstall;
	}

	public override int SortOrder => 11;
	public override string Name => "Skill Package";
	public override string Tagline => "Sets up a complete package of skills";

	public override string FullDescription =>
		@"This package installs all of the supporting information you are going to require for your skill setup, as well as all the skills and checks you will need. 

This includes the following items:

  #3Check Templates#0 - a complete set of templates for check types, including different difficulties
  #3Improvers#0 - a complete set of templates for skill improvement
  #3Describers#0 - a complete set of skill value describers
  #3Skills#0 - a complete set of skills based on your choices
  #3Checks#0 - a complete set of checks

#1Warning: Don't run both this and the Skill Example Seeder.#0
";

	#endregion
}