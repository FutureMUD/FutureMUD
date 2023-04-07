using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public class AttributeSeeder : IDatabaseSeeder
{
	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
		new List<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool>
			Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
		{
			("choice",
				@"You can choose from the following packages of attributes:

#BSOI#F    - Strength, Agility, Dexterity, Intelligence, Willpower, Constitution, Aura
#BDND#F    - Strength, Dexterity, Constitution, Intelligence, Wisdom, Charisma
#BLabMUD#F - Strength, Dexterity, Constitution, Intelligence, Willpower, Perception
#BSplit#F  - Strength, Constitution, Coordination, Willpower, Charisma, Intelligence, Perception, each split into two sub-stats
#B3stats#F - Body, Mind, Spirit
#BRPI#F    - Strength, Agility, Dexterity, Intelligence, Willpower, Constitution, Luck
#BSimple#F - Physique, Speed, Intelligence, Spirit
#BArm#F    - Strength, Agility, Wisdom, Endurance

What is your selection? ", (context, answers) => true,
				(text, context) =>
				{
					switch (text.ToLowerInvariant())
					{
						case "soi":
						case "dnd":
						case "labmud":
						case "split":
						case "3stats":
						case "rpi":
						case "simple":
						case "arm":
							return (true, string.Empty);
					}

					return (false, "That is not a valid selection.");
				}
			),
			("decorator",
				@"You can choose from the following decorators (how stat scores are described):

#BRPI#F    - Range-based descriptions such as Average, Good, Great, Excellent, Super etc. Stat scale 3-25.
#BLabMUD#F - Range-based descriptions similar to RPI but customised per attribute. Stat scale 3-25.
#BModern#F - Range-based descriptions with 0-100 stat scale.
#BRaw#F    - Show the raw value for the attribute

What is your selection? ", (context, answers) => true,
				(text, context) =>
				{
					switch (text.ToLowerInvariant())
					{
						case "rpi":
						case "labmud":
						case "modern":
						case "raw":
							return (true, string.Empty);
					}

					return (false, "That is not a valid selection.");
				}
			)
		};

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		context.Database.BeginTransaction();
		SeedAttributes(context, questionAnswers);
		context.Database.CommitTransaction();
		return "The operation completed successfully.";
	}

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.Accounts.Any()) return ShouldSeedResult.PrerequisitesNotMet;

		if (context.TraitDefinitions.Any(x => x.Type == 1)) return ShouldSeedResult.MayAlreadyBeInstalled;

		return ShouldSeedResult.ReadyToInstall;
	}

	public int SortOrder => 10;
	public string Name => "Attributes";
	public string Tagline => "Sets up attributes for your game";

	public string FullDescription =>
		@"This package installs a default setup for attributes. This is a necessary pre-requisite for nearly everything that comes after. This package has several built-in options, but if none of them suit what you want to do you will need to manually place your attributes into the database before running any other package after this one.

Keep in mind that you can always change the names or details of any of these attributes afterwards. At this point the most important decision you will make is about the ""shape"" of your attributes.";

	private static void SeedAttributes(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		var improver = new Improver
		{
			Name = "Non-Improving",
			Definition = "<Definition/>",
			Type = "non-improving"
		};
		context.Improvers.Add(improver);

		switch (questionAnswers["decorator"].ToLowerInvariant())
		{
			case "rpi":
				context.TraitDecorators.Add(
					new TraitDecorator
					{
						Name = "Attribute",
						Type = "Range",
						Contents =
							@"<ranges name=""General Attribute Range"" prefix="""" suffix="""" colour_default=""true"" colour_buffed=""true"" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Godlike""/></ranges>"
					}
				);
				break;
			case "labmud":
				switch (questionAnswers["choice"].ToLowerInvariant())
				{
					case "soi":
					case "rpi":
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Strength Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Strength Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Herculean""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Dexterity Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Dexterity Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Dolosian""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Agility Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Agility Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Achillean""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Constitution Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Constitution Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Atlassian""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Intelligence Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Intelligence Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Odyssian""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Willpower Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Willpower Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Promethean""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = questionAnswers["choice"].EqualTo("soi") ? "Aura Attribute" : "Luck Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Aura Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Divine""/></ranges>"
							}
						);
						break;
					case "dnd":
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Strength Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Strength Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Herculean""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Dexterity Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Dexterity Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Achillean""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Constitution Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Constitution Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Atlassian""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Intelligence Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Intelligence Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Odyssian""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Wisdom Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Wisdom Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Athenan""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Charisma Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Charisma Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Eurynomean""/></ranges>"
							}
						);
						break;
					case "labmud":
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Strength Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Strength Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Herculean""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Dexterity Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Dexterity Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Achillean""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Constitution Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Constitution Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Atlassian""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Intelligence Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Intelligence Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Odyssian""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Willpower Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Willpower Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Promethean""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Perception Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Perception Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Epicurean""/></ranges>"
							}
						);
						break;
					case "split":
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Strength Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Strength Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Herculean""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Dexterity Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Dexterity Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Achillean""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Constitution Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Constitution Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Atlassian""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Intelligence Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Intelligence Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Odyssian""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Willpower Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Willpower Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Promethean""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Perception Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Perception Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Epicurean""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Charisma Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Charisma Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Eurynomean""/></ranges>"
							}
						);
						break;
					case "3stats":
					case "owod":
					case "nwod":
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Body Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Strength Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Godlike""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Mind Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Mind Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Godlike""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Spirit Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Spirit Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Godlike""/></ranges>"
							}
						);
						break;

					case "simple":
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Physique Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Physique Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Godlike""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Speed Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Speed Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Godlike""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Intelligence Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Intelligence Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Godlike""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Spirit Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Spirit Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Godlike""/></ranges>"
							}
						);
						break;
					case "arm":
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Strength Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Strength Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Herculean""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Endurance Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Endurance Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Atlassian""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Agility Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Strength Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Achillean""/></ranges>"
							}
						);
						context.TraitDecorators.Add(
							new TraitDecorator
							{
								Name = "Wisdom Attribute",
								Type = "Range",
								Contents =
									@"<ranges name=""Strength Attribute Range"" prefix="""" suffix="""" colour_capped=""false""><range low=""-25"" high=""0"" text=""Abysmal""/><range low=""0"" high=""3"" text=""Terrible""/><range low=""3"" high=""6"" text=""Bad""/><range low=""6"" high=""9"" text=""Poor""/><range low=""9"" high=""11"" text=""Average""/><range low=""11"" high=""13"" text=""Good""/><range low=""13"" high=""15"" text=""Great""/><range low=""15"" high=""17"" text=""Excellent""/><range low=""17"" high=""20"" text=""Super""/><range low=""20"" high=""23"" text=""Epic""/><range low=""23"" high=""25"" text=""Legendary""/><range low=""25"" high=""30"" text=""Athenan""/></ranges>"
							}
						);
						break;
				}

				break;
			case "modern":
			case "raw":
				context.TraitDecorators.Add(
					new TraitDecorator
					{
						Name = "Numeric",
						Type = "SimpleNumeric",
						Contents = ""
					}
				);
				break;
		}


		context.SaveChanges();

		TraitDefinition staminaTrait;

		switch (questionAnswers["choice"].ToLowerInvariant())
		{
			case "soi":
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Strength",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's raw strength and lifting power.",
					BranchMultiplier = 1.0,
					Alias = "str",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Strength Attribute").Id
						: context.TraitDecorators.First().Id
				});
				staminaTrait = new TraitDefinition
				{
					Name = "Constitution",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb =
						"Measures a character's ability to withstand punishment and abuse, as well as physical fitness.",
					BranchMultiplier = 1.0,
					Alias = "con",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Constitution Attribute").Id
						: context.TraitDecorators.First().Id
				};
				context.TraitDefinitions.Add(staminaTrait);
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Agility",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's speed and flexibility.",
					BranchMultiplier = 1.0,
					Alias = "agi",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Agility Attribute").Id
						: context.TraitDecorators.First().Id
				});
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Dexterity",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's hand-eye coordination and ability to manipulate things.",
					BranchMultiplier = 1.0,
					Alias = "dex",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Dexterity Attribute").Id
						: context.TraitDecorators.First().Id
				});
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Intelligence",
					Type = 1,
					TraitGroup = "Mental",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's ability for learning and reason.",
					BranchMultiplier = 1.0,
					Alias = "int",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Intelligence Attribute").Id
						: context.TraitDecorators.First().Id
				});
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Willpower",
					Type = 1,
					TraitGroup = "Mental",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's ability to override their primal, instinctual responses.",
					BranchMultiplier = 1.0,
					Alias = "wil",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Willpower Attribute").Id
						: context.TraitDecorators.First().Id
				});
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Aura",
					Type = 1,
					TraitGroup = "Spiritual",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's raw strength and lifting power.",
					BranchMultiplier = 1.0,
					Alias = "aur",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Aura Attribute").Id
						: context.TraitDecorators.First().Id
				});
				break;
			case "dnd":
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Strength",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's raw strength and lifting power.",
					BranchMultiplier = 1.0,
					Alias = "str",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Strength Attribute").Id
						: context.TraitDecorators.First().Id
				});
				staminaTrait = new TraitDefinition
				{
					Name = "Constitution",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb =
						"Measures a character's ability to withstand punishment and abuse, as well as physical fitness.",
					BranchMultiplier = 1.0,
					Alias = "con",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Constitution Attribute").Id
						: context.TraitDecorators.First().Id
				};
				context.TraitDefinitions.Add(staminaTrait);
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Dexterity",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's hand-eye coordination and flexibility.",
					BranchMultiplier = 1.0,
					Alias = "dex",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Dexterity Attribute").Id
						: context.TraitDecorators.First().Id
				});
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Intelligence",
					Type = 1,
					TraitGroup = "Mental",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's ability for learning and reason.",
					BranchMultiplier = 1.0,
					Alias = "int",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Intelligence Attribute").Id
						: context.TraitDecorators.First().Id
				});
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Wisdom",
					Type = 1,
					TraitGroup = "Mental",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's common-sense and self-control.",
					BranchMultiplier = 1.0,
					Alias = "wis",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Wisdom Attribute").Id
						: context.TraitDecorators.First().Id
				});
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Charisma",
					Type = 1,
					TraitGroup = "Mental",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's strength of personality and charm.",
					BranchMultiplier = 1.0,
					Alias = "cha",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Charisma Attribute").Id
						: context.TraitDecorators.First().Id
				});
				break;
			case "labmud":
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Strength",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's raw strength and lifting power.",
					BranchMultiplier = 1.0,
					Alias = "str",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Strength Attribute").Id
						: context.TraitDecorators.First().Id
				});
				staminaTrait = new TraitDefinition
				{
					Name = "Constitution",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb =
						"Measures a character's ability to withstand punishment and abuse, as well as physical fitness.",
					BranchMultiplier = 1.0,
					Alias = "con",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Constitution Attribute").Id
						: context.TraitDecorators.First().Id
				};
				context.TraitDefinitions.Add(staminaTrait);
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Dexterity",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's hand-eye coordination and flexibility.",
					BranchMultiplier = 1.0,
					Alias = "dex",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Dexterity Attribute").Id
						: context.TraitDecorators.First().Id
				});
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Intelligence",
					Type = 1,
					TraitGroup = "Mental",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's ability for learning and reason.",
					BranchMultiplier = 1.0,
					Alias = "int",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Intelligence Attribute").Id
						: context.TraitDecorators.First().Id
				});
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Willpower",
					Type = 1,
					TraitGroup = "Mental",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's ability to override their primal, instinctual responses.",
					BranchMultiplier = 1.0,
					Alias = "wil",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Willpower Attribute").Id
						: context.TraitDecorators.First().Id
				});
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Perception",
					Type = 1,
					TraitGroup = "Mental",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's ability to make sense of their surroundings.",
					BranchMultiplier = 1.0,
					Alias = "per",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Perception Attribute").Id
						: context.TraitDecorators.First().Id
				});
				break;
			case "split":
				var td1 = new TraitDefinition
				{
					Name = "Upper Body Strength",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's power with their upper body.",
					BranchMultiplier = 1.0,
					Alias = "ust",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Strength Attribute").Id
						: context.TraitDecorators.First().Id,
					DisplayOrder = 2,
					DisplayAsSubAttribute = true,
					ShowInScoreCommand = false
				};
				var td2 = new TraitDefinition
				{
					Name = "Lower Body Strength",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's power with their lower body.",
					BranchMultiplier = 1.0,
					Alias = "lst",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Strength Attribute").Id
						: context.TraitDecorators.First().Id,
					DisplayOrder = 3,
					DisplayAsSubAttribute = true,
					ShowInScoreCommand = false
				};
				context.TraitDefinitions.Add(td1);
				context.TraitDefinitions.Add(td2);
				context.SaveChanges();
				var expression = new TraitExpression
				{
					Name = "Strength Attribute Formula",
					Expression = $"(ustr:{td1.Id} + lstr:{td2.Id})/2"
				};
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Strength",
					Type = (int)TraitType.DerivedAttribute,
					DerivedType = 0,
					TraitGroup = "Physical",
					Hidden = true,
					ChargenBlurb = "Measures a character's overall strength, both upper and lower body.",
					BranchMultiplier = 1.0,
					Alias = "str",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Strength Attribute").Id
						: context.TraitDecorators.First().Id,
					Expression = expression,
					DisplayOrder = 1
				});
				context.SaveChanges();

				staminaTrait = new TraitDefinition
				{
					Name = "Hardiness",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb =
						"Measures a character's ability to withstand punishment and abuse, as well as physical fitness.",
					BranchMultiplier = 1.0,
					Alias = "hrd",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Constitution Attribute").Id
						: context.TraitDecorators.First().Id,
					DisplayOrder = 5,
					DisplayAsSubAttribute = true,
					ShowInScoreCommand = false
				};
				context.TraitDefinitions.Add(staminaTrait);
				td2 = new TraitDefinition
				{
					Name = "Resilience",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb =
						"Measures a character's ability to withstand punishment and abuse, as well as physical fitness.",
					BranchMultiplier = 1.0,
					Alias = "res",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Constitution Attribute").Id
						: context.TraitDecorators.First().Id,
					DisplayOrder = 6,
					DisplayAsSubAttribute = true,
					ShowInScoreCommand = false
				};
				context.TraitDefinitions.Add(td2);
				context.SaveChanges();
				expression = new TraitExpression
				{
					Name = "Constitution Attribute Formula",
					Expression = $"(hrd:{staminaTrait.Id} + res:{td2.Id})/2"
				};
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Constitution",
					Type = (int)TraitType.DerivedAttribute,
					DerivedType = 0,
					TraitGroup = "Physical",
					Hidden = true,
					ChargenBlurb = "Measures a character's overall constitution.",
					BranchMultiplier = 1.0,
					Alias = "con",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Constitution Attribute").Id
						: context.TraitDecorators.First().Id,
					Expression = expression,
					DisplayOrder = 4
				});
				context.SaveChanges();

				td1 = new TraitDefinition
				{
					Name = "Dexterity",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's hand-eye coordination and ability to manipulate things.",
					BranchMultiplier = 1.0,
					Alias = "dex",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Dexterity Attribute").Id
						: context.TraitDecorators.First().Id,
					DisplayOrder = 8,
					DisplayAsSubAttribute = true,
					ShowInScoreCommand = false
				};
				td2 = new TraitDefinition
				{
					Name = "Speed",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's speed and flexibility.",
					BranchMultiplier = 1.0,
					Alias = "spd",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Dexterity Attribute").Id
						: context.TraitDecorators.First().Id,
					DisplayOrder = 9,
					DisplayAsSubAttribute = true,
					ShowInScoreCommand = false
				};
				context.TraitDefinitions.Add(td1);
				context.TraitDefinitions.Add(td2);
				context.SaveChanges();

				expression = new TraitExpression
				{
					Name = "Coordination Attribute Formula",
					Expression = $"(dex:{td1.Id} + spd:{td2.Id})/2"
				};
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Coordination",
					Type = (int)TraitType.DerivedAttribute,
					DerivedType = 0,
					TraitGroup = "Physical",
					Hidden = true,
					ChargenBlurb = "Measures a character's overall flexibility, agility, speed and coordination.",
					BranchMultiplier = 1.0,
					Alias = "coo",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Dexterity Attribute").Id
						: context.TraitDecorators.First().Id,
					Expression = expression,
					DisplayOrder = 7
				});
				context.SaveChanges();

				td1 = new TraitDefinition
				{
					Name = "Intellect",
					Type = 1,
					TraitGroup = "Mental",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's ability for learning and reason.",
					BranchMultiplier = 1.0,
					Alias = "int",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Intelligence Attribute").Id
						: context.TraitDecorators.First().Id,
					DisplayOrder = 11,
					DisplayAsSubAttribute = true,
					ShowInScoreCommand = false
				};
				td2 = new TraitDefinition
				{
					Name = "Wisdom",
					Type = 1,
					TraitGroup = "Mental",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's street smarts, experience and common sense.",
					BranchMultiplier = 1.0,
					Alias = "wis",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Intelligence Attribute").Id
						: context.TraitDecorators.First().Id,
					DisplayOrder = 12,
					DisplayAsSubAttribute = true,
					ShowInScoreCommand = false
				};

				context.TraitDefinitions.Add(td1);
				context.TraitDefinitions.Add(td2);
				context.SaveChanges();

				expression = new TraitExpression
				{
					Name = "Intelligence Attribute Formula",
					Expression = $"(int:{td1.Id} + wis:{td2.Id})/2"
				};
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Intelligence",
					Type = (int)TraitType.DerivedAttribute,
					DerivedType = 0,
					TraitGroup = "Mental",
					Hidden = true,
					ChargenBlurb = "Measures a character's overall level of intelligence.",
					BranchMultiplier = 1.0,
					Alias = "ign",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Intelligence Attribute").Id
						: context.TraitDecorators.First().Id,
					Expression = expression,
					DisplayOrder = 10
				});
				context.SaveChanges();

				td1 = new TraitDefinition
				{
					Name = "Sanity",
					Type = 1,
					TraitGroup = "Mental",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb =
						"Measures a character's ability to endure horror, loss, and persevere in the face of hopelessness.",
					BranchMultiplier = 1.0,
					Alias = "san",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Willpower Attribute").Id
						: context.TraitDecorators.First().Id,
					DisplayOrder = 14,
					DisplayAsSubAttribute = true,
					ShowInScoreCommand = false
				};
				td2 = new TraitDefinition
				{
					Name = "Tenacity",
					Type = 1,
					TraitGroup = "Mental",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb =
						"Measures a character's ability to impose their will upon themselves and the world around them.",
					BranchMultiplier = 1.0,
					Alias = "ten",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Willpower Attribute").Id
						: context.TraitDecorators.First().Id,
					DisplayOrder = 15,
					DisplayAsSubAttribute = true,
					ShowInScoreCommand = false
				};

				context.TraitDefinitions.Add(td1);
				context.TraitDefinitions.Add(td2);
				context.SaveChanges();

				expression = new TraitExpression
				{
					Name = "Willpower Attribute Formula",
					Expression = $"(san:{td1.Id} + ten:{td2.Id})/2"
				};
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Willpower",
					Type = (int)TraitType.DerivedAttribute,
					DerivedType = 0,
					TraitGroup = "Mental",
					Hidden = true,
					ChargenBlurb = "Measures a character's overall level of willpower and mental toughness.",
					BranchMultiplier = 1.0,
					Alias = "wil",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Willpower Attribute").Id
						: context.TraitDecorators.First().Id,
					Expression = expression,
					DisplayOrder = 13
				});
				context.SaveChanges();

				td1 = new TraitDefinition
				{
					Name = "Confidence",
					Type = 1,
					TraitGroup = "Mental",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's confidence in themselves and their ability.",
					BranchMultiplier = 1.0,
					Alias = "cnf",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Charisma Attribute").Id
						: context.TraitDecorators.First().Id,
					DisplayOrder = 17,
					DisplayAsSubAttribute = true,
					ShowInScoreCommand = false
				};
				td2 = new TraitDefinition
				{
					Name = "Aura",
					Type = 1,
					TraitGroup = "Mental",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's magnetism and strength of personality.",
					BranchMultiplier = 1.0,
					Alias = "aur",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Charisma Attribute").Id
						: context.TraitDecorators.First().Id,
					DisplayOrder = 18,
					DisplayAsSubAttribute = true,
					ShowInScoreCommand = false
				};

				context.TraitDefinitions.Add(td1);
				context.TraitDefinitions.Add(td2);
				context.SaveChanges();

				expression = new TraitExpression
				{
					Name = "Charisma Attribute Formula",
					Expression = $"(cnf:{td1.Id} + aur:{td2.Id})/2"
				};
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Charisma",
					Type = (int)TraitType.DerivedAttribute,
					DerivedType = 0,
					TraitGroup = "Mental",
					Hidden = true,
					ChargenBlurb = "Measures a character's overall level of charisma and external persona.",
					BranchMultiplier = 1.0,
					Alias = "cha",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Charisma Attribute").Id
						: context.TraitDecorators.First().Id,
					Expression = expression,
					DisplayOrder = 16
				});
				context.SaveChanges();

				td1 = new TraitDefinition
				{
					Name = "Sensory Acuity",
					Type = 1,
					TraitGroup = "Mental",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's senses and ability to interpret them.",
					BranchMultiplier = 1.0,
					Alias = "sns",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Perception Attribute").Id
						: context.TraitDecorators.First().Id,
					DisplayOrder = 20,
					DisplayAsSubAttribute = true,
					ShowInScoreCommand = false
				};
				td2 = new TraitDefinition
				{
					Name = "Cognitive Empathy",
					Type = 1,
					TraitGroup = "Mental",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb =
						"Measures a character's ability to perceive the emotional and mental states of others.",
					BranchMultiplier = 1.0,
					Alias = "cog",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Perception Attribute").Id
						: context.TraitDecorators.First().Id,
					DisplayOrder = 21,
					DisplayAsSubAttribute = true,
					ShowInScoreCommand = false
				};

				context.TraitDefinitions.Add(td1);
				context.TraitDefinitions.Add(td2);
				context.SaveChanges();

				expression = new TraitExpression
				{
					Name = "Perception Attribute Formula",
					Expression = $"(sns:{td1.Id} + cog:{td2.Id})/2"
				};
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Perception",
					Type = (int)TraitType.DerivedAttribute,
					DerivedType = 0,
					TraitGroup = "Mental",
					Hidden = true,
					ChargenBlurb = "Measures a character's overall level of perceptive ability.",
					BranchMultiplier = 1.0,
					Alias = "per",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Perception Attribute").Id
						: context.TraitDecorators.First().Id,
					Expression = expression,
					DisplayOrder = 19
				});
				context.SaveChanges();
				break;
			case "3stats":
				staminaTrait = new TraitDefinition
				{
					Name = "Body",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's strength, fitness and endurance.",
					BranchMultiplier = 1.0,
					Alias = "bod",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Body Attribute").Id
						: context.TraitDecorators.First().Id
				};
				context.TraitDefinitions.Add(staminaTrait);
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Intelligence",
					Type = 1,
					TraitGroup = "Mental",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's raw brain power and capacity for learning.",
					BranchMultiplier = 1.0,
					Alias = "int",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Intelligence Attribute").Id
						: context.TraitDecorators.First().Id
				});
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Spirit",
					Type = 1,
					TraitGroup = "Spiritual",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's magical ability and connection to the divine.",
					BranchMultiplier = 1.0,
					Alias = "spi",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Spirit Attribute").Id
						: context.TraitDecorators.First().Id
				});
				break;
			case "owod":
				throw new NotImplementedException();
			case "nwod":
				throw new NotImplementedException();
			case "rpi":
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Strength",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's raw strength and lifting power.",
					BranchMultiplier = 1.0,
					Alias = "str",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Strength Attribute").Id
						: context.TraitDecorators.First().Id
				});
				staminaTrait = new TraitDefinition
				{
					Name = "Constitution",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb =
						"Measures a character's ability to withstand punishment and abuse, as well as physical fitness.",
					BranchMultiplier = 1.0,
					Alias = "con",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Constitution Attribute").Id
						: context.TraitDecorators.First().Id
				};
				context.TraitDefinitions.Add(staminaTrait);
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Agility",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's speed and flexibility.",
					BranchMultiplier = 1.0,
					Alias = "agi",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Agility Attribute").Id
						: context.TraitDecorators.First().Id
				});
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Dexterity",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's hand-eye coordination and ability to manipulate things.",
					BranchMultiplier = 1.0,
					Alias = "dex",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Dexterity Attribute").Id
						: context.TraitDecorators.First().Id
				});
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Intelligence",
					Type = 1,
					TraitGroup = "Mental",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's ability for learning and reason.",
					BranchMultiplier = 1.0,
					Alias = "int",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Intelligence Attribute").Id
						: context.TraitDecorators.First().Id
				});
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Willpower",
					Type = 1,
					TraitGroup = "Mental",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's ability to override their primal, instinctual responses.",
					BranchMultiplier = 1.0,
					Alias = "wil",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Willpower Attribute").Id
						: context.TraitDecorators.First().Id
				});
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Luck",
					Type = 1,
					TraitGroup = "Spiritual",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's luck or lack thereof.",
					BranchMultiplier = 1.0,
					Alias = "luc",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Luck Attribute").Id
						: context.TraitDecorators.First().Id
				});
				break;
			case "simple":
				staminaTrait = new TraitDefinition
				{
					Name = "Physique",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's strength, fitness and endurance.",
					BranchMultiplier = 1.0,
					Alias = "phs",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Physique Attribute").Id
						: context.TraitDecorators.First().Id
				};
				context.TraitDefinitions.Add(staminaTrait);
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Speed",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's speed and flexibility.",
					BranchMultiplier = 1.0,
					Alias = "spd",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Speed Attribute").Id
						: context.TraitDecorators.First().Id
				});
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Intelligence",
					Type = 1,
					TraitGroup = "Mental",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's raw brain power and capacity for learning.",
					BranchMultiplier = 1.0,
					Alias = "int",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Intelligence Attribute").Id
						: context.TraitDecorators.First().Id
				});
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Spirit",
					Type = 1,
					TraitGroup = "Spiritual",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's magical ability and connection to the divine.",
					BranchMultiplier = 1.0,
					Alias = "spi",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Spirit Attribute").Id
						: context.TraitDecorators.First().Id
				});
				break;
			case "arm":
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Strength",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's raw strength and lifting power.",
					BranchMultiplier = 1.0,
					Alias = "str",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Strength Attribute").Id
						: context.TraitDecorators.First().Id
				});
				staminaTrait = new TraitDefinition
				{
					Name = "Endurance",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb =
						"Measures a character's ability to withstand punishment and abuse, as well as physical fitness.",
					BranchMultiplier = 1.0,
					Alias = "end",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Endurance Attribute").Id
						: context.TraitDecorators.First().Id
				};
				context.TraitDefinitions.Add(staminaTrait);
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Agility",
					Type = 1,
					TraitGroup = "Physical",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's speed and flexibility.",
					BranchMultiplier = 1.0,
					Alias = "agi",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Agility Attribute").Id
						: context.TraitDecorators.First().Id
				});
				context.TraitDefinitions.Add(new TraitDefinition
				{
					Name = "Wisdom",
					Type = 1,
					TraitGroup = "Mental",
					DerivedType = 0,
					ImproverId = improver.Id,
					Hidden = false,
					ChargenBlurb = "Measures a character's capacity for learning new things.",
					BranchMultiplier = 1.0,
					Alias = "wis",
					TeachDifficulty = 11,
					LearnDifficulty = 11,
					DecoratorId = questionAnswers["decorator"] == "labmud"
						? context.TraitDecorators.First(x => x.Name == "Wisdom Attribute").Id
						: context.TraitDecorators.First().Id
				});
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		context.SaveChanges();

		var maxStaminaProg = new FutureProg
		{
			FunctionName = "MaximumStamina",
			Category = "Character",
			Subcategory = "Stamina",
			FunctionComment = "Determines the maximum stamina for all characters in game",
			ReturnType = 2,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0,
			FunctionText = $"return 100+GetTrait(@ch, ToTrait({staminaTrait.Id}))*10"
		};
		maxStaminaProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = maxStaminaProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = 8200
		});
		context.FutureProgs.Add(maxStaminaProg);
		context.SaveChanges();

		var staminaProgConfig = context.StaticConfigurations.FirstOrDefault(x => x.SettingName == "MaximumStaminaProg");
		if (staminaProgConfig == null)
		{
			staminaProgConfig = new StaticConfiguration { SettingName = "MaximumStaminaProg" };
			context.StaticConfigurations.Add(staminaProgConfig);
		}

		staminaProgConfig.Definition = maxStaminaProg.Id.ToString();
		context.SaveChanges();
	}
}