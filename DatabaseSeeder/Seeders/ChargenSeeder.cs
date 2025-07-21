using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Accounts;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public class ChargenSeeder : IDatabaseSeeder
{
	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
		new List<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool>
			Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
		{
			("rpp",
				@"Do you want to use an 'account resource' such as Roleplay Points, Karma, etc?

Please answer #3yes#F or #3no#F: ",
				(context, answers) => true,
				(answer, context) =>
				{
					if (!answer.EqualToAny("yes", "y", "no", "n")) return (false, "Invalid selection.");
					return (true, string.Empty);
				}),
			("rppname",
				@"What name do you want to give to your RPP/Karma resource? Please enter a name an an alias separated by a slash, e.g. #3Roleplay Point/RPP#F or #3Karma/Karma#f

Please answer: ",
				(context, answers) => answers["rpp"].EqualToAny("yes", "y"),
				(answer, context) =>
				{
					if (!answer.Contains('/')) return (false, "Invalid selection - did not include a slash.");
					if (answer.Split('/').Length != 2) return (false, "Invalid selection - more than one slash");
					if (answer.Split('/').Any(x => string.IsNullOrWhiteSpace(x)))
						return (false, "Invalid selection - cannot have any empty names or aliases.");
					return (true, string.Empty);
				}),
			("bp",
				@"Do you want to use a 'Build Points' account resource that regenerates with playtime? This can be useful as an alternative to permanently spending RPP/Karma on chargen options for example.

Please answer #3yes#F or #3no#F: ",
				(context, answers) => true,
				(answer, context) =>
				{
					if (!answer.EqualToAny("yes", "y", "no", "n")) return (false, "Invalid selection.");
					return (true, string.Empty);
				}),
			("class",
				@"Do you want to use character classes in this MUD? For example, you could have classes like 'warrior', 'ranger', 'mage' etc. 

If you do enable classes, characters will be required to choose one during character creation. You'll have to build the classes yourself.

Please answer #3yes#F or #3no#F: ",
				(context, answers) => true,
				(answer, context) =>
				{
					if (!answer.EqualToAny("yes", "y", "no", "n")) return (false, "Invalid selection.");
					return (true, string.Empty);
				}),
			("subclass",
				@"Do you want to use subclasses for your classes? E.g. If you answer yes, people might choose 'warrior' and then choose 'bodyguard' as a subclass. Subclasses are mandatory if enabled.

Please answer #3yes#F or #3no#F: ",
				(context, answers) => { return answers["class"].EqualToAny("y", "yes"); },
				(answer, context) =>
				{
					if (!answer.EqualToAny("yes", "y", "no", "n")) return (false, "Invalid selection.");
					return (true, string.Empty);
				}),
			("role-first",
				@"#DCharacter Creation Order#f

You can customise the character creation experience to happen in any order, but realistically there are two main ways in which RPI MUDs will usually structure their character creation.

The choice primary comes down to whether you want to present the choice of a race, culture and ethnicity before you ask people to select roles (including classes) and the roles are filtered by these heritage criteria, or whether they choose the role/class first and then the other options are filtered by your role choice.

My own observation is that most RPIs are traditionally race-first, but many other roleplaying games offer the player the class choice first (obviously more relevant if you're using classes).

Consider these two examples:

#ARace-First#f: Your player has 2 Roleplay Points and are presented with a bunch of race options for that RPP level. They pick Ork, then choose Isengard Ork as their culture. Later they are presented with a list of roles specific to Isengard Orks.

#ARole-First#f: The first choice your player makes is their class and roles. Your player decides that they want to play a Water Mage and select a role about being a student at the Villageton City College of Wizardry. Later, they are shown the valid races, cultures and ethnicities that would be admitted to the college.

If you're unhappy with the decision you can change it later but it does influence how you ""restrict"" options (e.g. what progs you need to write to filter available choices).

With all this in mind, do you want to use ""role first"" or ""race first"" ordering?

Please answer #3role#F or #3race#f: ",
				(context, answers) => true,
				(answer, context) =>
				{
					if (!answer.EqualToAny("role", "race")) return (false, "Invalid selection.");
					return (true, string.Empty);
				}),
			("attributemode",
				@"There are various options that you can use for the attribute selection screen. They are explained below.

#BOrder#F: This screen permits the player to choose the order of the attributes only, but rolls the actual values. This is fairly traditional for RPIs (Armageddon, Shadows of Isildur and Atonement all use/used this method for example).
#BPoints#F: This screen allows the player to spend points on their attributes. It works best when paired with something like a build-point setup, but that is not absolutely essential.

Please answer #3order#f or #3points#f: ",
				(context, answers) => { return true; },
				(answer, context) =>
				{
					if (!answer.EqualToAny("order", "points")) return (false, "Invalid selection.");
					return (true, string.Empty);
				}),
			("skillmode",
				@"There are various options that you can use for the skill selection screen. They are explained below.

#BPicker#f: This screen permits the player to pick a pre-defined number of skills (e.g. classic SOI).
#BBoosts#F: This screen permits the player to pick a number of skills plus any additional they can pay for, and also lets them select boosts.

#9Note: The ""Boosts"" mode does not work well if you did not select either a Karma/RPP option or BP. It is designed to work with at least one of these, ideally BP.#0

Please answer #3picker#f or #3boosts#f: ",
				(context, answers) => { return true; },
				(answer, context) =>
				{
					if (!answer.EqualToAny("picker", "boosts")) return (false, "Invalid selection.");
					return (true, string.Empty);
				}),
			("merits",
				@"#DMerits and Flaws or Quirks#F

The engine has a system for things alternately called #AMerits and Flaws#F or #AQuirks#F. Players will select these in character creation and they can also be given to races, cultures, ethnicities, roles and classes by default.

Your choice here is about how these are presented to the character. 

If you choose the #3merit#F option they will first pick the number of merits that you nominate and then they must balance these with an equal number of flaws.

If you choose the #3quirk#f option all the options are presented as quirks, they are not required to balance them, and you have some alternate form of balancing them (like making them cost build points or only offering positive choices).

You can choose either #3merit#F to use the balanced merits/flaws system or choose #3quirk#f to use the unified approach.

Please answer with your choice: ",
				(context, answers) => true,
				(answer, context) =>
				{
					if (!answer.EqualToAny("merit", "quirk")) return (false, "Invalid selection.");
					return (true, string.Empty);
				}),
			("customdescs",
				@"#DCustom Descriptions in Chargen#F

Do you want to permit people to enter their own custom short and full descriptions? I highly recommend that you do not do this because of the way the FutureMUD markup is designed to work with things like items, spells and natural changes (e.g. haircuts, severed bodyparts etc). The markup is complex and if people write static descriptions that do not change with them then it somewhat devalues the whole process.

On the other hand, some people really like their custom descriptions. Admins can always override people's descriptions once they get in game, even if you disable this during character creation. The custom description entering screen includes a description of the markup, but it is very complex.

At the end of the day the decision is up to you. You can of course change your mind later if you don't like the decision, it simply applies to characters made after that point. 

With that in mind, do you want to enable custom descriptions in chargen?

Please answer #3yes#f or #3no#f: ",
				(context, answers) => true,
				(answer, context) =>
				{
					if (!answer.EqualToAny("yes", "y", "no", "n")) return (false, "Invalid selection.");
					return (true, string.Empty);
				})
		};

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		context.Database.BeginTransaction();

		#region Resources

		var rppresource = default(ChargenResource);
		if (questionAnswers["rpp"].EqualToAny("yes", "y"))
		{
			var split = questionAnswers["rppname"].Split('/', StringSplitOptions.RemoveEmptyEntries);
			var name = split[0].Trim();
			var ss = new StringStack(name);
			ss.PopAll();
			var pluralisingWord = ss.Last;
			var plural =
				$"{ss.Memory.Take(ss.Memory.Count() - 1).ListToCommaSeparatedValues(" ")} {pluralisingWord.Pluralise()}";
			var resource = new ChargenResource
			{
				Name = name,
				PluralName = plural,
				Alias = split[1].Trim().ToLowerInvariant(),
				MinimumTimeBetweenAwards = 43200,
				MaximumNumberAwardedPerAward = 1,
				PermissionLevelRequiredToAward = (int)PermissionLevel.JuniorAdmin,
				PermissionLevelRequiredToCircumventMinimumTime = (int)PermissionLevel.SeniorAdmin,
				ShowToPlayerInScore = true,
				MaximumResourceFormula = "-1",
				Type = "Simple",
				TextDisplayedToPlayerOnAward =
					$"Congratulations, you have been awarded {Name.A_An()} for your excellent conduct.",
				TextDisplayedToPlayerOnDeduct =
					$"You have been penalised for improper behaviour, and have had {Name.A_An()} deducted from your account."
			};
			rppresource = resource;
			context.ChargenResources.Add(resource);
		}

		var bpresource = default(ChargenResource);
		if (questionAnswers["bp"].EqualToAny("yes", "y"))
		{
			var resource = new ChargenResource
			{
				Name = "Build Point",
				PluralName = "Build Points",
				Alias = "bp",
				MinimumTimeBetweenAwards = 15,
				MaximumNumberAwardedPerAward = 5,
				PermissionLevelRequiredToAward = (int)PermissionLevel.Founder,
				PermissionLevelRequiredToCircumventMinimumTime = (int)PermissionLevel.Founder,
				ShowToPlayerInScore = true,
				Type = "Regenerating",
				MaximumResourceFormula = "1000",
				TextDisplayedToPlayerOnAward = "You have been awarded build points",
				TextDisplayedToPlayerOnDeduct = "You have been penalised build points"
			};
			context.ChargenResources.Add(resource);
			bpresource = resource;
		}

		context.SaveChanges();
		bpresource ??= rppresource;

		#endregion

		#region Progs

		var prog = new FutureProg
		{
			FunctionName = "MaximumAgeChargen",
			Category = "Chargen",
			Subcategory = "Age",
			FunctionComment = "Used to determine the maximum age for characters in character creation",
			ReturnType = (int)ProgVariableTypes.Number,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText = @"return @ch.Race.VenerableAge * 1.1"
		};
		context.FutureProgs.Add(prog);
		prog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = prog, ParameterIndex = 0, ParameterName = "ch",
			ParameterType = (int)ProgVariableTypes.Toon
		});

		prog = new FutureProg
		{
			FunctionName = "MinimumAgeChargen",
			Category = "Chargen",
			Subcategory = "Age",
			FunctionComment = "Used to determine the minimum age for characters in character creation",
			ReturnType = (int)ProgVariableTypes.Number,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText = @"return @ch.Race.YoungAdultAge"
		};
		context.FutureProgs.Add(prog);
		prog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = prog, ParameterIndex = 0, ParameterName = "ch",
			ParameterType = (int)ProgVariableTypes.Toon
		});

		prog = new FutureProg
		{
			FunctionName = "MaximumHeightChargen",
			Category = "Chargen",
			Subcategory = "Height",
			FunctionComment =
				"Used to determine the maximum height for characters in character creation. Result is in centimetres. Google a conversion if you need to.",
			ReturnType = (int)ProgVariableTypes.Number,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText = @"// You will need to expand this as you add new playable races
switch (@ch.Race)
  case (ToRace(""Human""))
	if (@ch.Gender == ToGender(""Male""))
	  // 6'10.5""
	  return 210
	else
	  // 6'8""
	  return 203
	end if
end switch
return 200"
		};
		context.FutureProgs.Add(prog);
		prog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = prog, ParameterIndex = 0, ParameterName = "ch",
			ParameterType = (int)ProgVariableTypes.Toon
		});

		prog = new FutureProg
		{
			FunctionName = "MinimumHeightChargen",
			Category = "Chargen",
			Subcategory = "Height",
			FunctionComment =
				"Used to determine the minimum height for characters in character creation. Result is in centimetres. Google a conversion if you need to.",
			ReturnType = (int)ProgVariableTypes.Number,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText = @"// You will need to expand this as you add new playable races
switch (@ch.Race)
  case (ToRace(""Human""))
	if (@ch.Gender == ToGender(""Male""))
	  // 4'11""
	  return 149
	else
	  // 4'9.5""
	  return 145
	end if
end switch
return 149"
		};
		context.FutureProgs.Add(prog);
		prog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = prog, ParameterIndex = 0, ParameterName = "ch",
			ParameterType = (int)ProgVariableTypes.Toon
		});

		prog = new FutureProg
		{
			FunctionName = "MaximumWeightChargen",
			Category = "Chargen",
			Subcategory = "Weight",
			FunctionComment =
				"Used to determine the maximum weight for characters in character creation. Result is in grams. Google a conversion if you need to, or use BMI as presented.",
			ReturnType = (int)ProgVariableTypes.Number,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText = @"// You will need to expand this as you add new playable races
var bmi as number
switch (@ch.Race)
  case (ToRace(""Human""))
	bmi = 50
  default
	bmi = 50
end switch
return (((@ch.Height / 100) ^ 2) * @bmi) * 1000"
		};
		context.FutureProgs.Add(prog);
		prog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = prog, ParameterIndex = 0, ParameterName = "ch",
			ParameterType = (int)ProgVariableTypes.Toon
		});

		prog = new FutureProg
		{
			FunctionName = "MinimumWeightChargen",
			Category = "Chargen",
			Subcategory = "Weight",
			FunctionComment =
				"Used to determine the minimum weight for characters in character creation. Result is in grams. Google a conversion if you need to, or use BMI as presented.",
			ReturnType = (int)ProgVariableTypes.Number,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText = @"// You will need to expand this as you add new playable races
var bmi as number
switch (@ch.Race)
  case (ToRace(""Human""))
	bmi = 16
  default
	bmi = 16
end switch
return (((@ch.Height / 100) ^ 2) * @bmi) * 1000"
		};
		context.FutureProgs.Add(prog);
		prog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = prog, ParameterIndex = 0, ParameterName = "ch",
			ParameterType = (int)ProgVariableTypes.Toon
		});

		prog = new FutureProg
		{
			FunctionName = "ChargenFreeSkills",
			Category = "Chargen",
			Subcategory = "Skills",
			FunctionComment = "Returns a list of skills that a character gets for free",
			ReturnType = (int)(ProgVariableTypes.Trait | ProgVariableTypes.Collection),
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText = @"var skills as trait collection
// Universal Skills (usually perception, athletics, that kind of stuff)
// additem skills ToTrait(""Skill Name"")

// Racial skills
switch (@ch.Race)
  case (ToRace(""Human""))
	// additem skills ToTrait(""Skill Name"")
	break
end switch

// Class-Based skills?

// Merits-Based, Culture-Based, Role-Based, etc etc

return @skills"
		};
		context.FutureProgs.Add(prog);
		prog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = prog, ParameterIndex = 0, ParameterName = "ch",
			ParameterType = (int)ProgVariableTypes.Toon
		});

		prog = new FutureProg
		{
			FunctionName = "ChargenNumberOfSkillPicks",
			Category = "Chargen",
			Subcategory = "Skills",
			FunctionComment = "Determines the number of skill picks that are given at character creation",
			ReturnType = (int)ProgVariableTypes.Number,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText = @"return 5"
		};
		context.FutureProgs.Add(prog);
		prog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = prog, ParameterIndex = 0, ParameterName = "ch",
			ParameterType = (int)ProgVariableTypes.Toon
		});

		prog = new FutureProg
		{
			FunctionName = "ChargenNumberOfKnowledgePicks",
			Category = "Chargen",
			Subcategory = "Knowledges",
			FunctionComment = "Determines the number of knowledge picks that are given at character creation",
			ReturnType = (int)ProgVariableTypes.Number,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText = @"return 1"
		};
		context.FutureProgs.Add(prog);
		prog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = prog, ParameterIndex = 0, ParameterName = "ch",
			ParameterType = (int)ProgVariableTypes.Toon
		});
		prog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = prog, ParameterIndex = 1, ParameterName = "trait",
			ParameterType = (int)ProgVariableTypes.Trait
		});

		prog = new FutureProg
		{
			FunctionName = "ChargenFreeKnowledges",
			Category = "Chargen",
			Subcategory = "Skills",
			FunctionComment = "Returns a list of knowledges that a character gets for free",
			ReturnType = (long)(ProgVariableTypes.Knowledge | ProgVariableTypes.Collection),
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText = @"var knowledges as knowledge collection
// If you have any surgerical procedures you need to add the related knowledge based on those skills
// if (@ch.Skills.Any(x, @x.Name == ""Surgery""))
//   additem knowledges ToKnowledge(""SurgicalKnowledge"")
// end if

// This is also the place to add scripts to literate characters
// if (@ch.Skills.Any(x, @x.Name == ""Literature""))
//   if (@ch.Skills.Any(x, @x.Name == ""English""))
//     additem knowledges ToKnowledge(""Latin Script"")
//   end if
// end if

// You can also add clan-based, racial, merit-based etc or anything you can think of
return @knowledges"
		};
		context.FutureProgs.Add(prog);
		prog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = prog, ParameterIndex = 0, ParameterName = "ch",
			ParameterType = (int)ProgVariableTypes.Toon
		});

		if (questionAnswers["attributemode"].EqualTo("points"))
		{
			prog = new FutureProg
			{
				FunctionName = "MaximumAttributeBoosts",
				Category = "Chargen",
				Subcategory = "Attributes",
				FunctionComment =
					"Determines the maximum number of boosts that may be put into a single attribute at character creation time",
				ReturnType = (int)ProgVariableTypes.Number,
				AcceptsAnyParameters = false,
				Public = false,
				StaticType = 0,
				FunctionText = questionAnswers["bp"].EqualToAny("y", "yes") ? @"return 10" : "return 6"
			};
			context.FutureProgs.Add(prog);
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog, ParameterIndex = 0, ParameterName = "ch",
				ParameterType = (int)ProgVariableTypes.Toon
			});
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog, ParameterIndex = 1, ParameterName = "trait",
				ParameterType = (int)ProgVariableTypes.Trait
			});

			prog = new FutureProg
			{
				FunctionName = "MaximumFreeAttributeBoosts",
				Category = "Chargen",
				Subcategory = "Attributes",
				FunctionComment =
					"Determines the maximum number of free boosts that may be put into a single attribute at character creation time without paying any build points",
				ReturnType = (int)ProgVariableTypes.Number,
				AcceptsAnyParameters = false,
				Public = false,
				StaticType = 0,
				FunctionText = @"return 6"
			};
			context.FutureProgs.Add(prog);
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog, ParameterIndex = 0, ParameterName = "ch",
				ParameterType = (int)ProgVariableTypes.Toon
			});
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog, ParameterIndex = 1, ParameterName = "trait",
				ParameterType = (int)ProgVariableTypes.Trait
			});

			prog = new FutureProg
			{
				FunctionName = "MaximumAttributeMinuses",
				Category = "Chargen",
				Subcategory = "Attributes",
				FunctionComment =
					"Determines the maximum number of times that a single attribute may be penalised at character creation time",
				ReturnType = (int)ProgVariableTypes.Number,
				AcceptsAnyParameters = false,
				Public = false,
				StaticType = 0,
				FunctionText = @"return 3"
			};
			context.FutureProgs.Add(prog);
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog, ParameterIndex = 0, ParameterName = "ch",
				ParameterType = (int)ProgVariableTypes.Toon
			});
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog, ParameterIndex = 1, ParameterName = "trait",
				ParameterType = (int)ProgVariableTypes.Trait
			});

			prog = new FutureProg
			{
				FunctionName = "FreeAttributeBoosts",
				Category = "Chargen",
				Subcategory = "Attributes",
				FunctionComment =
					"Determines the number of free (no build points) boosts to attributes at character creation time",
				ReturnType = (int)ProgVariableTypes.Number,
				AcceptsAnyParameters = false,
				Public = false,
				StaticType = 0,
				FunctionText = @"return 12"
			};
			context.FutureProgs.Add(prog);
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog, ParameterIndex = 0, ParameterName = "ch",
				ParameterType = (int)ProgVariableTypes.Toon
			});

			prog = new FutureProg
			{
				FunctionName = "AttributeBaseValue",
				Category = "Chargen",
				Subcategory = "Attributes",
				FunctionComment =
					"Determines the starting value of an attribute before boosts at character creation time, excluding racial bonuses which are added separately",
				ReturnType = (int)ProgVariableTypes.Number,
				AcceptsAnyParameters = false,
				Public = false,
				StaticType = 0,
				FunctionText = @"return 10"
			};
			context.FutureProgs.Add(prog);
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog, ParameterIndex = 0, ParameterName = "ch",
				ParameterType = (int)ProgVariableTypes.Toon
			});
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog, ParameterIndex = 1, ParameterName = "trait",
				ParameterType = (int)ProgVariableTypes.Trait
			});
		}

		if (questionAnswers["skillmode"].EqualTo("boosts"))
		{
			prog = new FutureProg
			{
				FunctionName = "ChargenSkillBoostCost",
				Category = "Chargen",
				Subcategory = "Skills",
				FunctionComment = "Determines the base cost of boosts to individual skills",
				ReturnType = (int)ProgVariableTypes.Number,
				AcceptsAnyParameters = false,
				Public = false,
				StaticType = 0,
				FunctionText =
					@"// This is just an example of how you might set this up. The limit is your own imagination.
if (@skill.Group == ""Combat"")
  return 15
end if
if (@skill.Group == ""Language"")
  return 5
end if
return 10"
			};
			context.FutureProgs.Add(prog);
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog, ParameterIndex = 0, ParameterName = "ch",
				ParameterType = (int)ProgVariableTypes.Toon
			});
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog, ParameterIndex = 1, ParameterName = "skill",
				ParameterType = (int)ProgVariableTypes.Trait
			});
		}

		context.SaveChanges();

		#endregion

		#region Screens

		var stages = new Dictionary<ChargenStage, ChargenScreenStoryboard>();
		var stageDependencies = new CollectionDictionary<ChargenScreenStoryboard, ChargenStage>();
		var order = 0;
		var nextId = 1L;

		void AddStage(ChargenStage stage, string type, ChargenStage nextStage, string definition,
			params ChargenStage[] dependencies)
		{
			var storyboard = new ChargenScreenStoryboard
			{
				Id = nextId++,
				ChargenStage = (int)stage,
				ChargenType = type,
				Order = order,
				NextStage = (int)nextStage,
				StageDefinition = definition
			};
			context.ChargenScreenStoryboards.Add(storyboard);
			stages[stage] = storyboard;
			stageDependencies.AddRange(storyboard, dependencies);
			order += 10;
		}

		AddStage(ChargenStage.Welcome, "WelcomeScreen", ChargenStage.SpecialApplication,
			"<Screen><Blurb><![CDATA[This is the welcome screen for your MUD. This is the first screen players will see during character creation. Usually it is the place to give a broad introduction to the concepts of the MUD, link players to your website and/or discord server, and also potentially a link to any documentation that they need to consider.]]></Blurb><Blurb><![CDATA[You can have multiple welcome screens if you want to split up the information. You can simply add new <Blurb> elements after the first. You can also safely delete this Blurb element if you only want a single welcome screen.]]></Blurb></Screen>");

		var useClasses = questionAnswers["class"].EqualToAny("y", "yes");
		var useSubclasses = useClasses && questionAnswers["subclass"].EqualToAny("yes", "y");
		var usingNonbinary = context.Races.First(x => x.Name == "Human").AllowedGenders == "2 3 4";

		if (questionAnswers["role-first"].EqualToAny("role"))
		{
			AddStage(ChargenStage.SpecialApplication, "SpecialApplication", ChargenStage.SelectRole,
				$"<Screen><Blurb><![CDATA[You may sometimes elect to submit a character application as a \"Special Application\". When you choose to do so, you will be presented with choices as if you had an additional #22 Roleplay Points#0 more than you currently have. {(bpresource != null ? "It will cost you #1750BP#0 to submit a special application in addition to any other costs, and you" : "You")} may only submit a special application once every 3 months. Submitting a special application automatically incurs a higher level of scrutiny and staff must still be satisfied that you meet the requirements for playing the role before approving you.]]></Blurb></Screen>");
			AddStage(ChargenStage.SelectRole, "RolePicker", ChargenStage.SelectRace,
				$"<Screen><IntroductionBlurb><![CDATA[You will now be invited to select roles for your character. Roles help flesh out your character's place in the world, and may be used by the staff at times to make decisions about your character.]]>    </IntroductionBlurb>   <RoleTypes>     <RoleType Type=\"Class\" Name=\"Class\" CanSelectNone=\"{(useClasses ? "false" : "true")}\"><![CDATA[Your class represents, in a broad sense, the capabilities of your character. It will appear in your SCORE, and can be referenced in various places by user-customised code. For instance, which skills are available to you, or which spells you learn could all be influenced by class. Class is entirely softcoded however, so it is up to the end user to establish what classes do.]]></RoleType><RoleType Type=\"Subclass\" Name=\"Sub Class\" CanSelectNone=\"{(useSubclasses ? "false" : "true")}\"><![CDATA[Your subclass represents a refinement of your class, and would extend the capabilities of your character. Like class, it appears in SCORE and can be referenced in various places by user-customised code. The use of subclasses is optional - just because you use classes, does not mean that you must use subclasses. Like class, it is entirely softcoded.]]> </RoleType><RoleType Type=\"Profession\" Name=\"Profession\" CanSelectNone=\"false\"><![CDATA[Your profession represents a job, employment, or vocation that your character has coming into the game. It might be used to give you additional starting skills, a starting clan, starting money, or something of that nature.]]></RoleType><RoleType Type=\"Family\" Name=\"Family\" CanSelectNone=\"true\"><![CDATA[Family roles are designed to represent things related to the family origins of the character, whether they be a noble character selecting a great house to which they belong, or even just a player-sponsored role to make an in-character family relation. Typically a family role would be more likely to give clanning and possibly gear or money than skill boosts, for example.]]></RoleType><RoleType Type=\"Story\" Name=\"Story\" CanSelectNone=\"true\"><![CDATA[Story roles represent unique opportunities for the character, or plot-driven backstories. If you choose to take one of these roles, you have some unique role in the story and metaplot.]]></RoleType></RoleTypes></Screen>",
				ChargenStage.SpecialApplication);
			AddStage(ChargenStage.SelectRace, "RacePicker", ChargenStage.SelectEthnicity,
				"<Screen><Blurb><![CDATA[Races vary from one another with sufficient differences to be considered different species, biologically speaking. You must choose a Race for your character to be, which will affect the availability of certain choices throughout the character creation process.]]></Blurb><ShowUnselectableRacesAsBlanks>false</ShowUnselectableRacesAsBlanks>   <SkipScreenIfOnlyOneChoice>true</SkipScreenIfOnlyOneChoice></Screen>",
				ChargenStage.SelectRole);
			AddStage(ChargenStage.SelectEthnicity, "EthnicityPicker", ChargenStage.SelectGender,
				"<Screen><Blurb><![CDATA[Ethnicity or ethnic group is a social group of people who identify with each other based on common ancestral, cultural, social, or national experience. Membership of an ethnic group tends to be associated with shared cultural heritage, ancestry, history, homeland, language (dialect) or ideology, and with symbolic systems such as religion, mythology and ritual, cuisine, dressing style, physical appearance, etc. Ethnicity is primarily used to determine what range of physical characteristics such as hair, eye and skin colour are naturally available to your character.]]></Blurb></Screen>",
				ChargenStage.SelectRace);
			AddStage(ChargenStage.SelectGender, "GenderPicker", ChargenStage.SelectCulture,
				$"<Screen><Blurb><![CDATA[Gender is primary used to determine which pronouns are applied to your character by default when they feature in system messages. Male characters will use he/him/his, female characters will use she/her, and {(usingNonbinary ? "non-binary or indeterminately-gendered" : "neuter or indeterminately-gendered")} (depending on race) individuals will use they/them/their. It also affects the starting range of values for height and weight, and the availability of the facial hair characteristic.]]></Blurb></Screen>",
				ChargenStage.SelectRace);
			AddStage(ChargenStage.SelectCulture, "CulturePicker", ChargenStage.SelectHandedness,
				"<Screen><Blurb><![CDATA[Your culture is, broadly speaking, the society in which you were raised and mechanically the main thing that it determines is the naming culture that you use and which calendar you use to mark your birthday. Much of the time cultures will align with ethnicities but they do not necessarily need to. You can also set up cultures to represent social classes.]]> </Blurb><ShowUnselectableCulturesAsBlanks>false</ShowUnselectableCulturesAsBlanks><SkipScreenIfOnlyOneChoice>true</SkipScreenIfOnlyOneChoice></Screen>",
				ChargenStage.SelectEthnicity, ChargenStage.SelectGender);
		}
		else
		{
			AddStage(ChargenStage.SpecialApplication, "SpecialApplication", ChargenStage.SelectRace,
				$"<Screen><Blurb><![CDATA[You may sometimes elect to submit a character application as a \"Special Application\". When you choose to do so, you will be presented with choices as if you had an additional #22 Roleplay Points#0 more than you currently have. {(bpresource != null ? "It will cost you #1750BP#0 to submit a special application in addition to any other costs, and you" : "You")} may only submit a special application once every 3 months. Submitting a special application automatically incurs a higher level of scrutiny and staff must still be satisfied that you meet the requirements for playing the role before approving you.]]></Blurb></Screen>");
			AddStage(ChargenStage.SelectRace, "RacePicker", ChargenStage.SelectEthnicity,
				"<Screen><Blurb><![CDATA[Races vary from one another with sufficient differences to be considered different species, biologically speaking. You must choose a Race for your character to be, which will affect the availability of certain choices throughout the character creation process.]]></Blurb><ShowUnselectableRacesAsBlanks>false</ShowUnselectableRacesAsBlanks>   <SkipScreenIfOnlyOneChoice>true</SkipScreenIfOnlyOneChoice></Screen>",
				ChargenStage.SpecialApplication);
			AddStage(ChargenStage.SelectEthnicity, "EthnicityPicker", ChargenStage.SelectGender,
				"<Screen><Blurb><![CDATA[Ethnicity or ethnic group is a social group of people who identify with each other based on common ancestral, cultural, social, or national experience. Membership of an ethnic group tends to be associated with shared cultural heritage, ancestry, history, homeland, language (dialect) or ideology, and with symbolic systems such as religion, mythology and ritual, cuisine, dressing style, physical appearance, etc. Ethnicity is primarily used to determine what range of physical characteristics such as hair, eye and skin colour are naturally available to your character.]]></Blurb></Screen>",
				ChargenStage.SelectRace);
			AddStage(ChargenStage.SelectGender, "GenderPicker", ChargenStage.SelectCulture,
				$"<Screen><Blurb><![CDATA[Gender is primary used to determine which pronouns are applied to your character by default when they feature in system messages. Male characters will use he/him/his, female characters will use she/her, and {(usingNonbinary ? "non-binary or indeterminately-gendered" : "neuter or indeterminately-gendered")} (depending on race) individuals will use they/them/their. It also affects the starting range of values for height and weight, and the availability of the facial hair characteristic.]]></Blurb></Screen>",
				ChargenStage.SelectRace);
			AddStage(ChargenStage.SelectCulture, "CulturePicker", ChargenStage.SelectRole,
				"<Screen><Blurb><![CDATA[Your culture is, broadly speaking, the society in which you were raised and mechanically the main thing that it determines is the naming culture that you use and which calendar you use to mark your birthday. Much of the time cultures will align with ethnicities but they do not necessarily need to. You can also set up cultures to represent social classes.]]> </Blurb><ShowUnselectableCulturesAsBlanks>false</ShowUnselectableCulturesAsBlanks><SkipScreenIfOnlyOneChoice>true</SkipScreenIfOnlyOneChoice></Screen>",
				ChargenStage.SelectEthnicity, ChargenStage.SelectGender);
			AddStage(ChargenStage.SelectRole, "RolePicker", ChargenStage.SelectHandedness,
				$"<Screen><IntroductionBlurb><![CDATA[You will now be invited to select roles for your character. Roles help flesh out your character's place in the world, and may be used by the staff at times to make decisions about your character.]]>    </IntroductionBlurb>   <RoleTypes>     <RoleType Type=\"Class\" Name=\"Class\" CanSelectNone=\"{(useClasses ? "false" : "true")}\"><![CDATA[Your class represents, in a broad sense, the capabilities of your character. It will appear in your SCORE, and can be referenced in various places by user-customised code. For instance, which skills are available to you, or which spells you learn could all be influenced by class. Class is entirely softcoded however, so it is up to the end user to establish what classes do.]]></RoleType><RoleType Type=\"Subclass\" Name=\"Sub Class\" CanSelectNone=\"{(useSubclasses ? "false" : "true")}\"><![CDATA[Your subclass represents a refinement of your class, and would extend the capabilities of your character. Like class, it appears in SCORE and can be referenced in various places by user-customised code. The use of subclasses is optional - just because you use classes, does not mean that you must use subclasses. Like class, it is entirely softcoded.]]> </RoleType><RoleType Type=\"Profession\" Name=\"Profession\" CanSelectNone=\"false\"><![CDATA[Your profession represents a job, employment, or vocation that your character has coming into the game. It might be used to give you additional starting skills, a starting clan, starting money, or something of that nature.]]></RoleType><RoleType Type=\"Family\" Name=\"Family\" CanSelectNone=\"true\"><![CDATA[Family roles are designed to represent things related to the family origins of the character, whether they be a noble character selecting a great house to which they belong, or even just a player-sponsored role to make an in-character family relation. Typically a family role would be more likely to give clanning and possibly gear or money than skill boosts, for example.]]></RoleType><RoleType Type=\"Story\" Name=\"Story\" CanSelectNone=\"true\"><![CDATA[Story roles represent unique opportunities for the character, or plot-driven backstories. If you choose to take one of these roles, you have some unique role in the story and metaplot.]]></RoleType></RoleTypes></Screen>",
				ChargenStage.SelectCulture);
		}

		AddStage(ChargenStage.SelectHandedness, "HandednessPicker", ChargenStage.SelectBirthday,
			"<Screen><Blurb><![CDATA[Every character has a dominant hand with which they are most comfortable holding and using things. If you are not ambidextrous, you will suffer some penalties for using things in your non-dominant hand (unless that thing has been specifically designed to be used in the off-hand, like shields). Even ambidextrous characters need to select a dominant hand, they merely have the penalties removed.]]></Blurb></Screen>",
			ChargenStage.SelectRace);
		AddStage(ChargenStage.SelectBirthday, "BirthdayPicker", ChargenStage.SelectHeight,
			"<Screen><AgeSelectionBlurb><![CDATA[You must now select an age for your character. You must enter a number in years, after which you will be given the opportunity to pick the specific date of your character's birthday.]]>    </AgeSelectionBlurb>   <DateSelectionBlurb><![CDATA[You must now choose a valid date on which your character was born.]]>    </DateSelectionBlurb>   <MinimumAgeProg>MinimumAgeChargen</MinimumAgeProg>   <MaximumAgeProg>MaximumAgeChargen</MaximumAgeProg></Screen>",
			ChargenStage.SelectCulture);
		AddStage(ChargenStage.SelectHeight, "HeightPicker", ChargenStage.SelectWeight,
			"<Screen><Blurb><![CDATA[Height is the measure of how tall your character is. Your maximum and minimum selectable heights are based on your race and gender.]]>    </Blurb>   <MaximumHeightProg>MaximumHeightChargen</MaximumHeightProg>   <MinimumHeightProg>MinimumHeightChargen</MinimumHeightProg></Screen>",
			ChargenStage.SelectRace, ChargenStage.SelectGender);
		AddStage(ChargenStage.SelectWeight, "WeightPicker", ChargenStage.SelectName,
			"<Screen><Blurb><![CDATA[Weight is the force exhibited by the mass of an individual due to the gravity of the celestial body they currently inhabit. Plainly, it determines how heavy you are, or aren't. Your maximum and minimum selectable weights are based on your height, your race, and your gender.]]>    </Blurb>   <MaximumWeightProg>MaximumWeightChargen</MaximumWeightProg>   <MinimumWeightProg>MinimumWeightChargen</MinimumWeightProg></Screen>",
			ChargenStage.SelectHeight);
		AddStage(ChargenStage.SelectName, "NamePicker", ChargenStage.SelectDisfigurements, "<Screen></Screen>",
			ChargenStage.SelectCulture);
		AddStage(ChargenStage.SelectDisfigurements, "DisfigurementPicker", ChargenStage.SelectMerits,
			@"<Screen><ScarBlurb><![CDATA[You may now pick scars for your character. Scars are permanent disfigurements that add flavour to your character. If you would like to design a scar that does not appear in this list, please submit one to the staff on Discord.]]></ScarBlurb><TattooBlurb><![CDATA[You may now pick tattoos for your character. Tattoos are permanent decorations that add flavour to your character. If you would like to design a tattoo that does not appear in this list, please submit one to the staff on Discord.]]></TattooBlurb><BodypartsBlurb><![CDATA[You may on this screen select bodyparts to begin as severed. For example, you could begin play with a missing eye or a missing hand. Unless you know what you are doing, it is highly recommended that you do not select any options on this screen.]]></BodypartsBlurb><ProstheticsBlurb><![CDATA[On this screen you may pick prosthetics to offset the disfigurements you selected in the previous missing bodyparts screen.]]></ProstheticsBlurb><AllowPickingScars>true</AllowPickingScars><AllowPickingTattoos>true</AllowPickingTattoos><AllowPickingMissingBodyparts>true</AllowPickingMissingBodyparts><Prostheses><!-- What follows is an example of how to insert your own prosthetics once you have built them in game 
	<Prosthetic>
			<Item>ITEM ID</Item>
			<Costs><Cost resource=""RESOURCE ID"" amount=""RESOURCE AMOUNT""/></Costs>
			<CanSelectProg>PROG NAME/ID - must take a single 'TOON' as a parameter and return boolean</CanSelectProg>
	</Prosthetic>
	--></Prostheses></Screen>", ChargenStage.SelectCulture);

		if (questionAnswers["merits"].EqualToAny("merit"))
			AddStage(ChargenStage.SelectMerits, "MeritPicker", ChargenStage.SelectAttributes,
				"<Screen><MeritSelectionBlurb><![CDATA[In addition to picking attributes and skills, you have the opportunity to pick from a number of merits and flaws. These traits can help flesh out your character's natural advantages and disadvantages, and most have coded benefits and penalties attributed to them. Merits are positive traits; usually they boost a stat or skill, or aid your character in any number of beneficial ways: increased strength, resistance to poisons, attractiveness, etc. Flaws, on the other hand, are typically negative traits that are, in some way, harmful to your character: allergies, cowardice, being weak-willed, among others. Picking merits and flaws is purely optional; if you don't want to, you may select none.]]></MeritSelectionBlurb><SkipScreenIfNoChoices>true</SkipScreenIfNoChoices><ForceBalanceOfMeritsAndFlaws>true</ForceBalanceOfMeritsAndFlaws><MaximumMeritsAndFlaws>4</MaximumMeritsAndFlaws></Screen>",
				ChargenStage.SelectDisfigurements);
		else
			AddStage(ChargenStage.SelectMerits, "QuirkPicker", ChargenStage.SelectAttributes,
				"<Screen><SelectionBlurb><![CDATA[In addition to picking attributes and skills, you have the opportunity to pick from a number of quirks. These traits can help flesh out your character's natural advantages and disadvantages, and most have coded benefits and penalties attributed to them. Picking quirks is purely optional; if you don't want to, you may select none.]]></SelectionBlurb><SkipScreenIfNoChoices>true</SkipScreenIfNoChoices><MaximumQuirks>4</MaximumQuirks></Screen>",
				ChargenStage.SelectDisfigurements);

		switch (questionAnswers["attributemode"].ToLowerInvariant())
		{
			case "order":
				AddStage(ChargenStage.SelectAttributes, "AttributeOrderer", ChargenStage.SelectSkills,
					"<Screen><Blurb><![CDATA[Attributes determine your innate physical, mental and spiritual characteristics. They are used in some checks, as well as potentially in determining the potential maximum values of many of your skills. You will select the starting values for these attributes below.]]></Blurb></Screen>",
					ChargenStage.SelectMerits);
				break;
			case "points":
				AddStage(ChargenStage.SelectAttributes, "AttributePointBuy", ChargenStage.SelectSkills,
					$"<Screen><Blurb><![CDATA[Attributes determine your innate physical, mental and spiritual characteristics. They are used in some checks, as well as potentially determining the potential maximum values of many of your skills. You will select the starting values for these attributes below.]]></Blurb><MaximumBoostsProg>MaximumAttributeBoosts</MaximumBoostsProg><MaximumFreeBoostsProg>MaximumFreeAttributeBoosts</MaximumFreeBoostsProg><MaximumMinusesProg>MaximumAttributeMinuses</MaximumMinusesProg><FreeBoostsProg>FreeAttributeBoosts</FreeBoostsProg><AttributeBaseValueProg>AttributeBaseValue</AttributeBaseValueProg><BoostCostExpression>pow(2, max(0,boosts-1)) * 100</BoostCostExpression><BoostResource>{bpresource?.Id ?? 0}</BoostResource><MaximumExtraBoosts>6</MaximumExtraBoosts></Screen>",
					ChargenStage.SelectMerits);
				break;
			default:
				throw new ArgumentOutOfRangeException("Unsupported attribute mode.");
		}

		switch (questionAnswers["skillmode"].ToLowerInvariant())
		{
			case "picker":
				AddStage(ChargenStage.SelectSkills, "SkillPicker", ChargenStage.SelectAccents,
					"<Screen><Blurb><![CDATA[You may now select the skills that your character begins the game with. You can also learn new skills in game.]]></Blurb><NumberOfSkillPicksProg>ChargenNumberOfSkillPicks</NumberOfSkillPicksProg><FreeSkillsProg>ChargenFreeSkills</FreeSkillsProg></Screen>",
					ChargenStage.SelectAttributes);
				break;
			case "boosts":
				AddStage(ChargenStage.SelectSkills, "SkillCostPicker", ChargenStage.SelectAccents,
					$"<Screen><SkillPickerBlurb><![CDATA[Skills measure your ability to accomplish tasks - to be \"good at something.\" Skills may be learned, with effort, at any time in game.\nNote: Some skill picks are nested and require the selection of another skill first before they are visible in the list. To those with MXP enabled, these nested skills will appear italicized.]]></SkillPickerBlurb>   <SkillBoostBlurb><![CDATA[The next step is deciding whether to apply any boosts to your character's starting skills. This is a totally optional process, and costs a large amount of build points. Each character also gets one free boost, so even new players can boost an important skill.  Each skill boost will push your starting skill value up approximately one \"rank\". It is mostly designed so that after the first few characters, when players have started to accumulate some build points, they can avoid some of the starting grind, but \"troll\" players who consistently roll red-shirt characters to try and PK don't get the same leg up.]]></SkillBoostBlurb>   <NumberOfFreeSkillPicksProg>ChargenNumberOfSkillPicks</NumberOfFreeSkillPicksProg>   <FreeSkillsProg>ChargenFreeSkills</FreeSkillsProg><BoostCostExpression>base * Pow(boosts,2)</BoostCostExpression><AdditionalSkillsCostExpression>50 * Pow(picks,2)</AdditionalSkillsCostExpression><MaximumBoosts>5</MaximumBoosts><BoostResource>{bpresource?.Id ?? 0}</BoostResource><FreeBoostResource>25</FreeBoostResource><BoostCostProg>ChargenSkillBoostCost</BoostCostProg></Screen>",
					ChargenStage.SelectAttributes);
				break;
			default:
				throw new ArgumentOutOfRangeException("Unsupported skill mode.");
		}

		AddStage(ChargenStage.SelectAccents, "AccentPicker", ChargenStage.SelectKnowledges,
			$"<Screen><AdditionalPicks resource=\"{bpresource?.Id ?? 0}\" cost=\"10\"/>   <Blurb><![CDATA[You will now be given the opportunity to pick which accents you natively employ with any language you have chosen. These are the accents in which you learned to speak the language in question, and represent how you will naturally speak the language. Other accents can be learned by exposure and practice.]]></Blurb></Screen>",
			ChargenStage.SelectSkills);
		AddStage(ChargenStage.SelectKnowledges, "KnowledgePickerBySkill", ChargenStage.SelectCharacteristics,
			"<Screen><Blurb><![CDATA[Knowledges are supplements to skills that represent specific areas of training that you have. Generally speaking, knowledges can be taught and learned in game, and they gate things like crafts and surgical procedures.]]></Blurb><NumberOfPicksProg>ChargenNumberOfKnowledgePicks</NumberOfPicksProg><FreeKnowledgesProg>ChargenFreeKnowledges</FreeKnowledgesProg></Screen>",
			ChargenStage.SelectSkills);
		AddStage(ChargenStage.SelectCharacteristics, "CharacteristicPicker", ChargenStage.SelectDescription,
			"<Screen><Blurb> \u00a0 \u00a0 \u00a0<![CDATA[Characteristics are a way to categorise the various aspects of your character's appearance, for example eye colour, or hair style. Your characteristics that are chosen at this stage of the character creation process (with the exception of hair style / length) represent your natural, intrinsic values for these characteristics, but can be changed, hidden or altered later in game by items, skills and effects.]]> \u00a0 \u00a0</Blurb></Screen>",
			ChargenStage.SelectMerits);
		AddStage(ChargenStage.SelectDescription, "DescriptionPicker", ChargenStage.SelectStartingLocation,
			$"<Screen><SDescBlurb><![CDATA[Your short description is how you are seen by other people when performing actions, for example, #5a short, blue-eyed youth#0.]]></SDescBlurb>   <FullDescBlurb><![CDATA[Your full description is how you are seen when people look directly at you, and provides a more detailed overview of your appearance.]]></FullDescBlurb>   <AllowCustomDescription>{(questionAnswers["customdescs"].EqualToAny("yes", "y") ? "true" : "false")}</AllowCustomDescription>   <AllowEntityDescriptionPatterns>true</AllowEntityDescriptionPatterns></Screen>",
			ChargenStage.SelectCharacteristics);
		var role = new ChargenRole
		{
			Name = "Default Starting Location",
			Type = (int)ChargenRoleType.StartingLocation,
			PosterId = 1,
			MaximumNumberAlive = 0,
			MaximumNumberTotal = 0,
			ChargenBlurb = "This is the default starting location that has not been described.",
			Expired = false,
			MinimumAuthorityToApprove = 0,
			MinimumAuthorityToView = 0
		};
		context.ChargenRoles.Add(role);
		context.SaveChanges();
		AddStage(ChargenStage.SelectStartingLocation, "StartingLocationPicker", ChargenStage.SelectNotes,
			@$"<Screen><Blurb><![CDATA[Your must now select a starting location for your character. This reflects where your character will begin once in the game, but does not mean you are restricted to only that area.]]>    </Blurb>   <Locations>     <Location>       <Name>Guest Lounge</Name>       <Blurb><![CDATA[As you have not yet set up any other starting areas, the default starting area will be the guest lounge.]]></Blurb>       <Location>1</Location>       <Role>{role.Id}</Role><OnCommenceProg>0</OnCommenceProg>     </Location></Locations>   <SkipScreenIfOnlyOneChoice>true</SkipScreenIfOnlyOneChoice></Screen>",
			ChargenStage.SelectRole);
		AddStage(ChargenStage.SelectNotes, "NotePicker", ChargenStage.Submit,
			"<Stage><Note Name=\"Background Comment\">\u00a0 \u00a0 \u00a0<Blurb><![CDATA[Please give a brief overview of your character's background and/or personality, in order to give admin storytellers something to work with roleplaying with you. Typically we would expect a minimum of four sentences or dot points.]]></Blurb>\u00a0 \u00a0 \u00a0<Prog>AlwaysTrue</Prog>\u00a0 \u00a0</Note></Stage>",
			ChargenStage.SelectSkills);
		AddStage(ChargenStage.Submit, "Submit", ChargenStage.None, "<Stage></Stage>");
		AddStage(ChargenStage.Menu, "Menu", ChargenStage.None, "<Stage></Stage>");
		context.SaveChanges();

		foreach (var stage in stageDependencies)
		foreach (var dependency in stage.Value)
			stage.Key.DependentStages.Add(new ChargenScreenStoryboardDependentStage
				{ Owner = stage.Key, Dependency = (int)dependency });
		context.SaveChanges();

		#endregion

		context.Database.CommitTransaction();
		return "Character creation has been successfully set up.";
	}

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.Races.Any(x => x.Name == "Human")) return ShouldSeedResult.PrerequisitesNotMet;

		if (context.ChargenScreenStoryboards.Any()) return ShouldSeedResult.MayAlreadyBeInstalled;

		return ShouldSeedResult.ReadyToInstall;
	}

	public int SortOrder => 100;
	public string Name => "Character Creation";
	public string Tagline => "Sets up Character Creation and Guest Logins";

	public string FullDescription =>
		@"This package will create all the storyboards for character creation so that people can make characters on your game (not to mention other admin avatars other than your own). It will set up resources such as RPP and Karma. You absolutely must set up a human race first.

Note that it will put placeholder blurb text for each of the chargen stages in by default. You need to go and edit these later to make them specific to your MUD. You can find them in the database in the ChargenScreenStoryboards table.";
}