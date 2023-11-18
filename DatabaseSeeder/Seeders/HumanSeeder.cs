using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Accounts;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class HumanSeeder : IDatabaseSeeder
{
	private FuturemudDatabaseContext _context;
	private HeightWeightModel _humanFemaleHWModel;

	private HeightWeightModel _humanMaleHWModel;
	private IReadOnlyDictionary<string, string> _questionAnswers;

	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
		new List<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool>
			Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
		{
			("model", @"#DHealth Model#F

Which health model should humans use by default? This can be overriden for individual NPCs (so you can make HP-based mooks even if you use the full medical system) but the choice you make here will be applied to all player characters.

The valid choices are as follows:

#Bhp#F	- this system will use hitpoints or destruction of the brain only to determine death.
#Bhpplus#F	- this system uses hp and brain destruction, but also enables heart and organ damage.
#Bfull#F	- this system uses the full medical model, where the only way to die is via death of the brain.

Your choice: ",
				(context, answers) => true,
				(text, context) =>
				{
					switch (text.ToLowerInvariant())
					{
						case "hp":
						case "hpplus":
						case "full":
							return (true, string.Empty);
					}

					return (false, "That is not a valid selection.");
				}
			),
			("inventory", @"#DInventory System#F

Do you want to use a hands system for inventory (like RPI Engine) or a 'general inventory' unrelated to hands, like most traditional MUDs?

Please answer #Bhands#F or #Binventory#F: ", (context, answers) => true,
				(text, context) =>
				{
					switch (text.ToLowerInvariant())
					{
						case "hands":
						case "inv":
						case "inventory":
							return (true, string.Empty);
					}

					return (false, "That is not a valid selection.");
				}
			),
			("sever",
				"Do you want the bodyparts to be built as severable? If you choose no, then bodypart severing will effectively be disabled for humans.\n\nPlease answer #3yes#F or #3no#F: ",
				(context, answers) => !answers["model"].EqualTo("hp"),
				(text, context) =>
				{
					switch (text.ToLowerInvariant())
					{
						case "yes":
						case "y":
						case "no":
						case "n":
							return (true, string.Empty);
					}

					return (false, "Please answer yes or no.");
				}
			),
			("bones", @"You have several options for how to handle bones. The three options are as follows:

#Bfull#F - create separate, breakable bones in the model
#Bimplied#0 - don't create separate bone bodyparts, but change the base bodytypes to be ""bony"" and able to be broken
#Bnone#F - don't include bones, disable bone breaking mechanics

Please choose your answer: ", (context, answers) => true,
				(text, context) =>
				{
					switch (text.ToLowerInvariant())
					{
						case "full":
						case "implied":
						case "none":
							return (true, string.Empty);
					}

					return (false, "Please answer full, implied or none.");
				}
			),
			("distinctive", @"Do you want to include the 'distinctive feature' characteristic for your humans? 

This characteristic allows you to capture miscellaneous descriptors not otherwise covered by another characteristic like hair colour or eye shape.

The seeder comes with a number of these, such as 'bullnecked' or 'freckles', and you can feel free to add or remove these afterwards.

Please answer #3yes#F or #3no#F: ", (context, answers) => true,
				(text, context) =>
				{
					switch (text.ToLowerInvariant())
					{
						case "yes":
						case "y":
						case "no":
						case "n":
							return (true, string.Empty);
					}

					return (false, "Please answer yes or no.");
				}
			),
			("nonbinary",
				@"Do you want to include the 'non binary' gender selection (and associated building) for your humans? 

The non-binary gender can choose options from either gender for characteristics (where they would otherwise be gendered, like facial hair). 

If you choose no, the only gender options will be male and female.

Please answer #3yes#F or #3no#F: ", (context, answers) => true,
				(text, context) =>
				{
					switch (text.ToLowerInvariant())
					{
						case "yes":
						case "y":
						case "no":
						case "n":
							return (true, string.Empty);
					}

					return (false, "Please answer yes or no.");
				}
			),
			("includeextraperson",
				@"One element of the descriptions that will be generated is the 'person word'. This is usually something like 'man', 'woman', 'maiden', or the like.

Some of these are a little less formal than the others. This may suit the tone of some MUDs and not of others. You can elect not to include these less formal person words.

The full list is as follows: #6beefcake#0, #6punk#0, #6wretch#0, #6unit#0, #6frump#0, #6stud#0, #6hunk#0, #6specimen#0, #6gal#0, and #6diva#0.

Would you like to include these extra person words?

Please answer #3yes#F or #3no#F: ", (context, answers) => true,
				(text, context) =>
				{
					switch (text.ToLowerInvariant())
					{
						case "yes":
						case "y":
						case "no":
						case "n":
							return (true, string.Empty);
					}

					return (false, "Please answer yes or no.");
				})
		};

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		_context = context;
		_questionAnswers = questionAnswers;
		_context.Database.BeginTransaction();

		// Start by determining the appropriate health strategy

		#region Health Strategy

		var healthTrait = _context.TraitDefinitions
			.Where(x => x.Type == 1)
			.AsEnumerable()
			.First(x => x.Name.In("Constitution", "Body", "Physique", "Endurance", "Hardiness", "Stamina"));
		_context.StaticConfigurations.Find("DefaultHoldBreathExpression").Definition = $"90+(5*con:{healthTrait.Id})";
		_context.StaticConfigurations.Find("DefaultBreatheVolumeExpression").Definition =
			$"max(2,7-(0.15*con:{healthTrait.Id}))";
		var hpExpression = new TraitExpression
		{
			Name = "Human Max HP Formula",
			Expression = $"100+con:{healthTrait.Id}"
		};
		_context.TraitExpressions.Add(hpExpression);
		var hpTick = new TraitExpression
		{
			Name = "Human HP Heal Per Tick",
			Expression = $"100+con:{healthTrait.Id}"
		};
		_context.TraitExpressions.Add(hpTick);
		var secondaryTrait = _context.TraitDefinitions
			                     .Where(x => x.Type == 1)
			                     .AsEnumerable()
			                     .FirstOrDefault(x => x.Name.In("Willpower", "Resilience", "Mind")) ??
		                     healthTrait;

		var strengthTrait = _context.TraitDefinitions
			.Where(x => x.Type == 1)
			.AsEnumerable()
			.First(x => x.Name.In("Strength", "Physique", "Body", "Upper Body Strength"));
		_context.StaticConfigurations.Find("DefaultDragWeightExpression").Definition =
			$"str:{strengthTrait.Id} * 2500000)";
		_context.StaticConfigurations.Find("DefaultLiftWeightExpression").Definition =
			$"str:{strengthTrait.Id} * 1000000)";
		_context.SaveChanges();

		var hpStrategy = new HealthStrategy
		{
			Name = "Human HP",
			Type = "BrainHitpoints",
			Definition =
				$"<Definition> <MaximumHitPointsExpression>{hpExpression.Id}</MaximumHitPointsExpression> <HealingTickDamageExpression>{hpTick.Id}</HealingTickDamageExpression><PercentageHealthPerPenalty>0.2</PercentageHealthPerPenalty><LodgeDamageExpression>max(0, damage-15)</LodgeDamageExpression> <SeverityRanges> <Severity value=\"0\" lower=\"-1\" upper=\"0\"/> <Severity value=\"1\" lower=\"0\" upper=\"2\"/> <Severity value=\"2\" lower=\"2\" upper=\"4\"/> <Severity value=\"3\" lower=\"4\" upper=\"7\"/> <Severity value=\"4\" lower=\"7\" upper=\"12\"/> <Severity value=\"5\" lower=\"12\" upper=\"18\"/> <Severity value=\"6\" lower=\"18\" upper=\"27\"/> <Severity value=\"7\" lower=\"27\" upper=\"40\"/> <Severity value=\"8\" lower=\"40\" upper=\"100\"/> </SeverityRanges><CheckHeart>false</CheckHeart> <UseHypoxiaDamage>false</UseHypoxiaDamage><KnockoutOnCritical>true</KnockoutOnCritical><KnockoutDuration>240</KnockoutDuration></Definition>"
		};
		_context.HealthStrategies.Add(hpStrategy);
		var hpplusStrategy = new HealthStrategy
		{
			Name = "Human HP Plus",
			Type = "BrainHitpoints",
			Definition =
				$"<Definition> <MaximumHitPointsExpression>{hpExpression.Id}</MaximumHitPointsExpression> <HealingTickDamageExpression>{hpTick.Id}</HealingTickDamageExpression><PercentageHealthPerPenalty>0.2</PercentageHealthPerPenalty><LodgeDamageExpression>max(0,damage-15)</LodgeDamageExpression> <SeverityRanges> <Severity value=\"0\" lower=\"-1\" upper=\"0\"/> <Severity value=\"1\" lower=\"0\" upper=\"2\"/> <Severity value=\"2\" lower=\"2\" upper=\"4\"/> <Severity value=\"3\" lower=\"4\" upper=\"7\"/> <Severity value=\"4\" lower=\"7\" upper=\"12\"/> <Severity value=\"5\" lower=\"12\" upper=\"18\"/> <Severity value=\"6\" lower=\"18\" upper=\"27\"/> <Severity value=\"7\" lower=\"27\" upper=\"40\"/> <Severity value=\"8\" lower=\"40\" upper=\"100\"/> </SeverityRanges> <CheckHeart>true</CheckHeart> <UseHypoxiaDamage>true</UseHypoxiaDamage><KnockoutOnCritical>true</KnockoutOnCritical><KnockoutDuration>240</KnockoutDuration> </Definition>"
		};
		_context.HealthStrategies.Add(hpplusStrategy);
		var stunExpression = new TraitExpression
		{
			Name = "Human Max Stun Formula",
			Expression = $"100+(con:{healthTrait.Id}+wil:{secondaryTrait.Id})/2"
		};
		_context.TraitExpressions.Add(stunExpression);

		var painExpression = new TraitExpression
		{
			Name = "Human Max Pain Formula",
			Expression = $"100+wil:{secondaryTrait.Id}"
		};
		_context.TraitExpressions.Add(painExpression);

		var stunTick = new TraitExpression
		{
			Name = "Human Stun Heal Per Tick",
			Expression = $"100+con:{healthTrait.Id}"
		};
		_context.TraitExpressions.Add(stunTick);

		var painTick = new TraitExpression
		{
			Name = "Human Pain Heal Per Tick",
			Expression = $"100+con:{healthTrait.Id}"
		};
		_context.TraitExpressions.Add(painTick);
		_context.SaveChanges();
		var fullStrategy = new HealthStrategy
		{
			Name = "Human Full Model",
			Type = "ComplexLiving",
			Definition =
				$"<Definition> <MaximumHitPointsExpression>{hpExpression.Id}</MaximumHitPointsExpression> <MaximumStunExpression>{stunExpression.Id}</MaximumStunExpression> <MaximumPainExpression>{painExpression.Id}</MaximumPainExpression> <HealingTickDamageExpression>{hpTick.Id}</HealingTickDamageExpression> <HealingTickStunExpression>{stunTick.Id}</HealingTickStunExpression> <HealingTickPainExpression>{painTick.Id}</HealingTickPainExpression> <LodgeDamageExpression>max(0, damage - 30)</LodgeDamageExpression> <PercentageHealthPerPenalty>0.2</PercentageHealthPerPenalty> <PercentageStunPerPenalty>0.2</PercentageStunPerPenalty> <PercentagePainPerPenalty>0.2</PercentagePainPerPenalty> <SeverityRanges> <Severity value=\"0\" lower=\"-2\" upper=\"-1\" lowerpec=\"-100\" upperperc=\"0\"/> <Severity value=\"1\" lower=\"-1\" upper=\"2\" lowerpec=\"0\" upperperc=\"0.4\"/> <Severity value=\"2\" lower=\"2\" upper=\"4\" lowerpec=\"0.4\" upperperc=\"0.55\"/> <Severity value=\"3\" lower=\"4\" upper=\"7\" lowerpec=\"0.55\" upperperc=\"0.65\"/> <Severity value=\"4\" lower=\"7\" upper=\"12\" lowerpec=\"0.65\" upperperc=\"0.75\"/> <Severity value=\"5\" lower=\"12\" upper=\"18\" lowerpec=\"0.75\" upperperc=\"0.85\"/> <Severity value=\"6\" lower=\"18\" upper=\"27\" lowerpec=\"0.85\" upperperc=\"0.9\"/> <Severity value=\"7\" lower=\"27\" upper=\"40\" lowerpec=\"0.9\" upperperc=\"0.95\"/> <Severity value=\"8\" lower=\"40\" upper=\"100\" lowerpec=\"0.95\" upperperc=\"100\"/> </SeverityRanges> </Definition>"
		};
		_context.HealthStrategies.Add(fullStrategy);
		_context.SaveChanges();

		HealthStrategy strategy;
		switch (questionAnswers["model"].ToLowerInvariant())
		{
			case "hp":
				strategy = hpStrategy;
				break;
			case "hpplus":
				strategy = hpplusStrategy;
				break;
			case "full":
				strategy = fullStrategy;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		#endregion

		// Setup stamina
		var staminaRecoveryProg = new FutureProg
		{
			FunctionName = "HumanStaminaRecovery",
			Category = "Character",
			Subcategory = "Stamina",
			FunctionComment = "Determines the stamina gain per 10 seconds for humans",
			ReturnType = 2,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0,
			FunctionText = $"return GetTrait(@ch, ToTrait({healthTrait.Id}))"
		};
		staminaRecoveryProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = staminaRecoveryProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = 8200
		});
		_context.FutureProgs.Add(staminaRecoveryProg);
		var liverFunctionProg = new FutureProg
		{
			FunctionName = "LiverFunction",
			Category = "Character",
			Subcategory = "Biology",
			FunctionComment =
				"Determines the amount of alcohol and other contaminants that the liver can remove per hour, in grams",
			ReturnType = 2,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0,
			FunctionText = @"var raceFactor as Number
switch (@ch.Race)
  case (ToRace(""Human""))
    raceFactor = 1
  default
    raceFactor = 1
end switch
var genderFactor as Number
if (@ch.Gender == ToGender(""Male""))
  genderFactor = 1.2
else
                genderFactor = 1
end if
var meritFactor as Number
meritFactor = 1.0
return (0.01 * (@ch.Weight / 70000)) * @raceFactor * @genderFactor * @meritFactor"
		};
		liverFunctionProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = liverFunctionProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = 8200
		});
		_context.FutureProgs.Add(liverFunctionProg);

		var bloodVolumeProg = new FutureProg
		{
			FunctionName = "BloodVolume",
			Category = "Character",
			Subcategory = "Biology",
			FunctionComment = "Determines the amount of maximum blood in a character, in litres",
			ReturnType = 2,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0,
			FunctionText = @"if (SameRace(@ch.Race,ToRace(""Humanoid"")))
  // For humans, use Nadler's equation for blood volume
  if (@ch.Gender == ToGender(""Male""))
    return (0.3669 * ((@ch.Height / 100) ^ 3)) + (0.03219 * (@ch.Weight / 1000)) + 0.6041
  else                                                                                                    
    return (0.3561 * ((@ch.Height / 100) ^ 3)) + (0.03308 * (@ch.Weight / 1000)) + 0.1833
  end if
end if

// Default fallback for other animals and races
return @ch.Weight / 20000"
		};
		bloodVolumeProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = bloodVolumeProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = 8200
		});
		_context.FutureProgs.Add(bloodVolumeProg);

		var maximumAliasesProg = new FutureProg
		{
			FunctionName = "MaximumNumberOfAliases",
			Category = "Character",
			Subcategory = "Names",
			FunctionComment = "Determines the maximum number of aliases a character can adopt at once",
			ReturnType = 2,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0,
			FunctionText = @"return 10"
		};
		maximumAliasesProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = maximumAliasesProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = 8200
		});
		_context.FutureProgs.Add(maximumAliasesProg);

		var chargenNeedsProg = new FutureProg
		{
			FunctionName = "WhichNeedsModel",
			Category = "Character",
			Subcategory = "Biology",
			FunctionComment = "Determines the needs model to use for a character",
			ReturnType = (long)FutureProgVariableTypes.Text,
			AcceptsAnyParameters = false,
			Public = true,
			StaticType = 0,
			FunctionText = @"if (@ch.Guest)
  return ""Passive""
else
  if (not(@ch.NPC) or GetRegister(@ch.Race, ""UseActiveNeeds""))
    return ""Active""
  else
    return ""NoNeeds""
  end if
end if"
		};
		chargenNeedsProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = chargenNeedsProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = 8200
		});
		_context.FutureProgs.Add(chargenNeedsProg);

		_context.VariableDefinitions.Add(new VariableDefinition
		{
			OwnerType = (long)FutureProgVariableTypes.Race, ContainedType = (long)FutureProgVariableTypes.Boolean,
			Property = "useactiveneeds"
		});
		_context.VariableDefaults.Add(new VariableDefault
		{
			OwnerType = (long)FutureProgVariableTypes.Race, Property = "useactiveneeds",
			DefaultValue = "<var>False</var>"
		});

		_context.SaveChanges();

		_context.StaticConfigurations.Find("MaximumNumberOfAliasProg").Definition = maximumAliasesProg.Id.ToString();
		_context.StaticConfigurations.Find("TotalBloodVolumeProg").Definition = bloodVolumeProg.Id.ToString();
		_context.StaticConfigurations.Find("LiverFunctionProg").Definition = liverFunctionProg.Id.ToString();
		_context.StaticConfigurations.Add(new StaticConfiguration
			{ SettingName = "ChargenNeedsModelProg", Definition = chargenNeedsProg.Id.ToString() });


		var wearSize = new WearableSizeParameterRule
		{
			BodyProtoId = 0,
			MinHeightFactor = 0.5,
			MaxHeightFactor = 1.5,
			MinWeightFactor = 0.5,
			MaxWeightFactor = 1.5,
			IgnoreTrait = true,
			WeightVolumeRatios =
				"<Ratios><Ratio Item=\"5\" Min=\"0\" Max=\"0.75\"/><Ratio Item=\"4\" Min=\"0.75\" Max=\"0.95\"/><Ratio Item=\"3\" Min=\"0.95\" Max=\"0.99\"/><Ratio Item=\"2\" Min=\"0.99\" Max=\"1.01\"/><Ratio Item=\"3\" Min=\"1.01\" Max=\"1.05\"/><Ratio Item=\"1\" Min=\"1.05\" Max=\"1.3\"/><Ratio Item=\"0\" Min=\"1.3\" Max=\"1000\"/></Ratios>",
			TraitVolumeRatios =
				"<Ratios><Ratio Item=\"5\" Min=\"0\" Max=\"0.75\"/><Ratio Item=\"4\" Min=\"0.75\" Max=\"0.95\"/><Ratio Item=\"3\" Min=\"0.95\" Max=\"0.99\"/><Ratio Item=\"2\" Min=\"0.99\" Max=\"1.01\"/><Ratio Item=\"3\" Min=\"1.01\" Max=\"1.05\"/><Ratio Item=\"1\" Min=\"1.05\" Max=\"1.3\"/><Ratio Item=\"0\" Min=\"1.3\" Max=\"1000\"/></Ratios>",
			HeightLinearRatios =
				"<Ratios><Ratio Item=\"0\" Min=\"0\" Max=\"0.90\"/><Ratio Item=\"1\" Min=\"0.90\" Max=\"0.95\"/><Ratio Item=\"2\" Min=\"0.95\" Max=\"0.99\"/><Ratio Item=\"3\" Min=\"0.99\" Max=\"1.01\"/><Ratio Item=\"2\" Min=\"1.01\" Max=\"1.05\"/><Ratio Item=\"4\" Min=\"1.05\" Max=\"1.1\"/><Ratio Item=\"5\" Min=\"1.1\" Max=\"1000\"/></Ratios>"
		};
		_context.WearableSizeParameterRule.Add(wearSize);
		_context.SaveChanges();

		var nextId = _context.BodyProtos.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;
		var humanoidBody = new BodyProto
		{
			Id = nextId++,
			Name = "Humanoid",
			ConsiderString = "",
			WielderDescriptionSingle = "hand",
			WielderDescriptionPlural = "hands",
			StaminaRecoveryProgId = staminaRecoveryProg.Id,
			MinimumLegsToStand = 2,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		_context.BodyProtos.Add(humanoidBody);

		var organicBody = new BodyProto
		{
			Id = nextId++,
			Name = "Organic Humanoid",
			CountsAs = humanoidBody,
			ConsiderString =
				@"$skincolour[&he has @ skin][You cannot make out &his skin tone] and $frame[you would describe &his build as @][you cannot make out &his build].
$eyecolour[% eyecolour[&he has $eyeshape $eyecolourbasic eyes that are $eyecolourfancy][2 - 3:&he has % $eyeshape $eyecolourbasic eyes that are $eyecolourfancy][1:&he has a single $eyeshape $eyecolourbasic eye that is $eyecolourfancy][0:&he has no eyes, only empty sockets]][You cannot see & his eyes because & he is wearing $]
$?hairstyle[&he has &?a_an[$haircolour $hairstyle]][&he is completely bald].$?facialhairstyle[
&he has &?a_an[$facialhaircolour $facialhairstyle.]]
& he has % nose[&a_an[$nose]][0:no nose at all].
& he has % ears[$ears ears][1:a single $ears ear][0:no ears at all]." +
				(questionAnswers["distinctive"].EqualToAny("yes", "y") ? "\n& he has $distinctivefeature." : ""),
			WielderDescriptionSingle = "hand",
			WielderDescriptionPlural = "hands",
			StaminaRecoveryProgId = staminaRecoveryProg.Id,
			MinimumLegsToStand = 2,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		_context.BodyProtos.Add(organicBody);
		_context.SaveChanges();

		SetupSpeeds(humanoidBody);
		SetupBodyparts(humanoidBody, organicBody);
		var human = SetupRaces(humanoidBody, organicBody, strategy, healthTrait, strengthTrait);
		SetupCharacteristics(questionAnswers["distinctive"].EqualToAny("yes", "y"), human.ParentRace);
		SetupDescriptions();
		SetupHeightWeightModels();

		#region Avatar Creation
		var race = _context.Races.First(x => x.Name == "Human");
		var ethnicity = new Ethnicity
		{
			Name = "Admin",
			ChargenBlurb = "This is an ethnicity for admin avatars and should not be selected by others.",
			AvailabilityProg = _context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
			ParentRace = race,
			EthnicGroup = "Admin",
			PopulationBloodModel = _context.PopulationBloodModels.First(),
			TolerableTemperatureFloorEffect = 0,
			TolerableTemperatureCeilingEffect = 0
		};
		_context.Ethnicities.Add(ethnicity);

		var nameCulture = SetupNameCultures();

		var culture = new Culture
		{
			Name = "Admin",
			Description = "This is a culture for admin avatars and should not be selected by others.",
			PersonWordMale = "Admin",
			PersonWordFemale = "Admin",
			PersonWordIndeterminate = "Admin",
			PersonWordNeuter = "Admin",
			SkillStartingValueProg = _context.FutureProgs.First(x => x.FunctionName == "AlwaysOneHundred"),
			AvailabilityProg = _context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
			TolerableTemperatureCeilingEffect = 0,
			TolerableTemperatureFloorEffect = 0,
			PrimaryCalendarId = _context.Calendars.First().Id
		};
		culture.CulturesNameCultures.Add(new CulturesNameCultures
		{ Culture = culture, NameCulture = nameCulture, Gender = (short)Gender.Male });
		culture.CulturesNameCultures.Add(new CulturesNameCultures
		{ Culture = culture, NameCulture = nameCulture, Gender = (short)Gender.Female });
		culture.CulturesNameCultures.Add(new CulturesNameCultures
		{ Culture = culture, NameCulture = nameCulture, Gender = (short)Gender.Neuter });
		culture.CulturesNameCultures.Add(new CulturesNameCultures
		{ Culture = culture, NameCulture = nameCulture, Gender = (short)Gender.NonBinary });
		culture.CulturesNameCultures.Add(new CulturesNameCultures
		{ Culture = culture, NameCulture = nameCulture, Gender = (short)Gender.Indeterminate });
		_context.Cultures.Add(culture);

		var account = _context.Accounts.First(x => x.AuthorityGroup.AuthorityLevel == (int)PermissionLevel.Founder);

		var sdescPattern = _context.EntityDescriptionPatterns
			.Where(x => x.Type == 0 && x.ApplicabilityProg.FunctionName == "IsHumanoidNonFemale")
			.GetRandomElement();
		var fdescPattern = _context.EntityDescriptionPatterns
			.Where(x => x.Type == 1 && x.ApplicabilityProg.FunctionName == "IsHumanoidNonFemale")
			.GetRandomElement();

		var body = new Body
		{
			BodyPrototypeId = organicBody.Id,
			Height = 180,
			Weight = 90000,
			Position = 1,
			CurrentSpeed = 1,
			Race = race,
			CurrentStamina = 100,
			CurrentBloodVolume = 100,
			Ethnicity = ethnicity,
			Bloodtype = race.BloodModel.BloodModelsBloodtypes.First().Bloodtype,
			Gender = (short)Gender.Male,
			ShortDescription = sdescPattern.Pattern,
			ShortDescriptionPattern = sdescPattern,
			FullDescription = fdescPattern.Pattern,
			FullDescriptionPattern = fdescPattern,
			HeldBreathLength = 0,
			EffectData = "<Effects/>"
		};
		_context.Bodies.Add(body);
		_context.SaveChanges();

		foreach (var definition in race.RacesAdditionalCharacteristics.Concat(race.ParentRace
					 .RacesAdditionalCharacteristics))
		{
			var cdef = definition.CharacteristicDefinition;
			var value = cdef.CharacteristicValues.GetRandomElement();
			while (value == null)
			{
				if (cdef.Parent == null) throw new ApplicationException("Couldn't find a characteristic value.");

				cdef = cdef.Parent;
				value = cdef.CharacteristicValues.GetRandomElement();
			}

			body.Characteristics.Add(new Characteristic
			{ Body = body, Type = (int)definition.CharacteristicDefinitionId, CharacteristicValue = value });
			ethnicity.EthnicitiesCharacteristics.Add(new EthnicitiesCharacteristics
			{
				Ethnicity = ethnicity,
				CharacteristicDefinition = definition.CharacteristicDefinition,
				CharacteristicProfile = _context.CharacteristicProfiles.First(x =>
					x.TargetDefinition == definition.CharacteristicDefinition && x.Type == "All")
			});
		}

		foreach (var attribute in race.RacesAttributes.Concat(race.ParentRace.RacesAttributes))
			body.Traits.Add(new Trait
			{ Body = body, TraitDefinition = attribute.Attribute, Value = 25, AdditionalValue = 0 });

		var dateRegex = new Regex(@"(?<prelude>\d+/\w+/)(?<year>\d+)", RegexOptions.IgnoreCase);

		var character = new Character
		{
			Body = body,
			Account = account,
			Name = account.Name,
			CreationTime = DateTime.UtcNow,
			Status = 2,
			State = 1,
			Gender = (short)Gender.Male,
			Location = _context.Cells.First().Id,
			Culture = culture,
			EffectData = @"<Effects>
  <Effect>
    <ApplicabilityProg>0</ApplicabilityProg>
    <Type>Immwalk</Type>
    <Original>0</Original>
    <Remaining>0</Remaining>
    <Blank />
  </Effect>
  <Effect>
    <ApplicabilityProg>0</ApplicabilityProg>
    <Type>AdminTelepathy</Type>
    <Original>0</Original>
    <Remaining>0</Remaining>
    <Blank />
  </Effect>
  <Effect>
    <ApplicabilityProg>0</ApplicabilityProg>
    <Type>AdminSight</Type>
    <Original>0</Original>
    <Remaining>0</Remaining>
    <Blank />
  </Effect>
</Effects>",
			BirthdayCalendarId = _context.Calendars.First().Id,
			BirthdayDate = dateRegex.Replace(_context.Calendars.First().Date,
				m => $"{m.Groups["prelude"].Value}{int.Parse(m.Groups["year"].Value) - 70}"),
			IsAdminAvatar = true,
			TotalMinutesPlayed = 0,
			NeedsModel = "NoNeeds",
			ShownIntroductionMessage = true,
			PositionId = 1,
			PositionModifier = 5,
			WritingStyle = 8256,
			DominantHandAlignment = 3,
			NameInfo = @$"<Names>
   <PersonalName>
     <Name culture=""{culture.Id}"">
       <Element usage=""BirthName""><![CDATA[{account.Name}]]></Element>
     </Name>
   </PersonalName>
   <Aliases>
   </Aliases>
   <CurrentName>0</CurrentName>
 </Names>"
		};
		_context.Characters.Add(character);
		_context.SaveChanges();

		var language = _context.Languages.FirstOrDefault();
		if (language != null)
		{
			character.Body.Traits.Add(new Trait
			{
				Body = character.Body,
				TraitDefinition = language.LinkedTrait,
				Value = 200,
				AdditionalValue = 0
			});

			character.CharactersLanguages.Add(
				new CharactersLanguages { Character = character, Language = language });
			character.CharactersAccents.Add(new CharacterAccent
			{
				Character = character,
				Accent = language.DefaultLearnerAccent,
				Familiarity = 0,
				IsPreferred = true
			});
			character.CurrentLanguage = language;
			character.CurrentAccent = language.DefaultLearnerAccent;
			_context.SaveChanges();
		}
		#endregion

		_context.SaveChanges();

		_context.Database.CommitTransaction();
		return "The operation completed successfully";
	}

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.Accounts.Any() || !context.TraitDefinitions.Any(x => x.Type == 0) || !context.Calendars.Any())
			return ShouldSeedResult.PrerequisitesNotMet;

		if (context.Races.Any(x => x.Name == "Humanoid")) return ShouldSeedResult.MayAlreadyBeInstalled;

		return ShouldSeedResult.ReadyToInstall;
	}

	public int SortOrder => 50;
	public string Name => "Human Seeder";
	public string Tagline => "Adds a human race and associated data to the game";

	public string FullDescription =>
		@"This package installs a human race along with all necessary associated data such as wearables. This human race package should most likely be used by a majority of MUDs that have humans in them at all.";

	private NameCulture SetupNameCultures()
	{
		var admin = new NameCulture
		{
			Name = "Admin",
			Definition =
				@"<NameCulture>   <Counts>     <Count Usage=""0"" Min=""1"" Max=""1""/>   </Counts>   <Patterns>     <Pattern Style=""0"" Text=""{0}"" Params=""0""/>     <Pattern Style=""1"" Text=""{0}"" Params=""0""/>     <Pattern Style=""2"" Text=""{0}"" Params=""0""/>     <Pattern Style=""3"" Text=""{0}"" Params=""0""/>     <Pattern Style=""4"" Text=""{0}"" Params=""0""/>     <Pattern Style=""5"" Text=""{0}"" Params=""0""/>   </Patterns>   <Elements>     <Element Usage=""0"" MinimumCount=""1"" MaximumCount=""1"" Name=""Avatar Name""><![CDATA[This is the name by which your Admin Avatar will be known. Your admin Avatar name should ideally be thematically consistent with the world in which you are an administrator. You should consult with a senior administrator to determine the eligability of any name that you have in mind before you make this decision.]]></Element>   </Elements>   <NameEntryRegex><![CDATA[^(?<birthname>[\w '-]+)$]]></NameEntryRegex> </NameCulture>"
		};
		_context.NameCultures.Add(admin);
		_context.SaveChanges();

		return admin;
	}

	private void SetupHeightWeightModels()
	{
		_humanMaleHWModel = new HeightWeightModel
		{
			Name = "Human Male",
			MeanHeight = 177.8,
			MeanBmi = 25.6,
			StddevHeight = 7.6,
			StddevBmi = 3.7,
			Bmimultiplier = 0.1
		};
		_context.Add(_humanMaleHWModel);
		_humanFemaleHWModel = new HeightWeightModel
		{
			Name = "Human Female",
			MeanHeight = 159,
			MeanBmi = 25.6,
			StddevHeight = 5,
			StddevBmi = 4.9,
			Bmimultiplier = 0.1
		};
		_context.Add(_humanFemaleHWModel);
		_context.Add(new HeightWeightModel
		{
			Name = "Well-Fed Human Male",
			MeanHeight = 182,
			MeanBmi = 26.1,
			StddevHeight = 7.6,
			StddevBmi = 3.7,
			Bmimultiplier = 0.1
		});
		_context.Add(new HeightWeightModel
		{
			Name = "Well-Fed Human Female",
			MeanHeight = 163,
			MeanBmi = 26.1,
			StddevHeight = 7.6,
			StddevBmi = 3.7,
			Bmimultiplier = 0.1
		});
		_context.Add(new HeightWeightModel
		{
			Name = "Underfed Human Male",
			MeanHeight = 170,
			MeanBmi = 23.1,
			StddevHeight = 4.3,
			StddevBmi = 1.5,
			Bmimultiplier = 0.1
		});
		_context.Add(new HeightWeightModel
		{
			Name = "Underfed Human Female",
			MeanHeight = 152,
			MeanBmi = 23.1,
			StddevHeight = 4,
			StddevBmi = 1.65,
			Bmimultiplier = 0.1
		});
		_context.Add(new HeightWeightModel
		{
			Name = "Tall Human Male",
			MeanHeight = 187,
			MeanBmi = 25.6,
			StddevHeight = 7.6,
			StddevBmi = 3.7,
			Bmimultiplier = 0.1
		});
		_context.Add(new HeightWeightModel
		{
			Name = "Tall Human Female",
			MeanHeight = 169,
			MeanBmi = 25.6,
			StddevHeight = 7.6,
			StddevBmi = 3.7,
			Bmimultiplier = 0.1
		});
		_context.Add(new HeightWeightModel
		{
			Name = "Short Human Male",
			MeanHeight = 169,
			MeanBmi = 25.6,
			StddevHeight = 7.6,
			StddevBmi = 3.7,
			Bmimultiplier = 0.1
		});
		_context.Add(new HeightWeightModel
		{
			Name = "Short Human Female",
			MeanHeight = 152,
			MeanBmi = 25.6,
			StddevHeight = 7.6,
			StddevBmi = 3.7,
			Bmimultiplier = 0.1
		});
		_context.SaveChanges();
	}

	private void SetupSpeeds(BodyProto humanoidBody)
	{
		var nextId = _context.MoveSpeeds.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = humanoidBody, PositionId = 1, Alias = "stroll", FirstPersonVerb = "stroll",
			ThirdPersonVerb = "strolls", PresentParticiple = "strolling", Multiplier = 2, StaminaMultiplier = 0.4
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = humanoidBody, PositionId = 1, Alias = "walk", FirstPersonVerb = "walk",
			ThirdPersonVerb = "walks", PresentParticiple = "walking", Multiplier = 1, StaminaMultiplier = 0.8
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = humanoidBody, PositionId = 1, Alias = "jog", FirstPersonVerb = "jog",
			ThirdPersonVerb = "jogs", PresentParticiple = "jogging", Multiplier = 0.75, StaminaMultiplier = 1.2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = humanoidBody, PositionId = 1, Alias = "run", FirstPersonVerb = "run",
			ThirdPersonVerb = "runs", PresentParticiple = "running", Multiplier = 0.5, StaminaMultiplier = 1.9
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = humanoidBody, PositionId = 1, Alias = "sprint", FirstPersonVerb = "sprint",
			ThirdPersonVerb = "sprints", PresentParticiple = "sprinting", Multiplier = 0.33, StaminaMultiplier = 2.4
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = humanoidBody, PositionId = 6, Alias = "slowcrawl",
			FirstPersonVerb = "crawl slowly", ThirdPersonVerb = "crawls slowly", PresentParticiple = "crawling slowly",
			Multiplier = 7, StaminaMultiplier = 0.6
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = humanoidBody, PositionId = 6, Alias = "crawl", FirstPersonVerb = "crawl",
			ThirdPersonVerb = "crawls", PresentParticiple = "crawling", Multiplier = 5, StaminaMultiplier = 1.25
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = humanoidBody, PositionId = 6, Alias = "fastcrawl",
			FirstPersonVerb = "crawl quickly", ThirdPersonVerb = "crawls quickly",
			PresentParticiple = "crawling quickly", Multiplier = 3, StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = humanoidBody, PositionId = 7, Alias = "shuffle", FirstPersonVerb = "shuffle",
			ThirdPersonVerb = "shuffles", PresentParticiple = "shuffling", Multiplier = 7, StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = humanoidBody, PositionId = 15, Alias = "climb", FirstPersonVerb = "climb",
			ThirdPersonVerb = "climbs", PresentParticiple = "climbing", Multiplier = 3, StaminaMultiplier = 3
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = humanoidBody, PositionId = 16, Alias = "swim", FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims", PresentParticiple = "swimming", Multiplier = 1.5, StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = humanoidBody, PositionId = 16, Alias = "slowswim", FirstPersonVerb = "swim",
			ThirdPersonVerb = "swims", PresentParticiple = "swimming", Multiplier = 2, StaminaMultiplier = 2
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = humanoidBody, PositionId = 18, Alias = "fly", FirstPersonVerb = "fly",
			ThirdPersonVerb = "flies", PresentParticiple = "flying", Multiplier = 1.8, StaminaMultiplier = 15
		});
		_context.MoveSpeeds.Add(new MoveSpeed
		{
			Id = nextId++, BodyProto = humanoidBody, PositionId = 18, Alias = "franticfly",
			FirstPersonVerb = "franticly fly", ThirdPersonVerb = "franticly flies",
			PresentParticiple = "franticly flying", Multiplier = 1.4, StaminaMultiplier = 25
		});
		_context.SaveChanges();
	}

	private Race SetupRaces(BodyProto baseBody, BodyProto organicBody, HealthStrategy strategy,
		TraitDefinition healthTrait, TraitDefinition strengthTrait)
	{
		var distinctive = _questionAnswers["distinctive"].EqualToAny("yes", "y");

		#region Corpse Models

		var organicHumanCorpse = new CorpseModel
		{
			Name = "Organic Human Corpse",
			Type = "Standard",
			Description = "This corpse will decay over time and eventually disappear",
			Definition = distinctive
				? @"<?xml version=""1.0""?>
 <CorpseModel>
   <EdiblePercentage>0.35</EdiblePercentage>
   <Ranges>
     <Range state=""0"" lower=""0"" upper=""7200""/>
     <Range state=""1"" lower=""7200"" upper=""21600""/>
     <Range state=""2"" lower=""21600"" upper=""65000""/>
     <Range state=""3"" lower=""65000"" upper=""400000""/>
     <Range state=""4"" lower=""400000"" upper=""2650000""/>
     <Range state=""5"" lower=""2650000"" upper=""2650001""/>
   </Ranges>
   <Terrains default=""10"">
     <Terrain terrain=""void"" rate=""0""/>
   </Terrains>
   <Descriptions>
     <PartDescriptions>
 	  <Description state=""0""><![CDATA[&a_an[@@shorteat[, ]freshly severed {1} {0}]]]></Description>
 	  <Description state=""1""><![CDATA[&a_an[@@shorteat[, ]severed {1} {0}]]]></Description>
 	  <Description state=""2""><![CDATA[&a_an[@@shorteat[, ]decaying severed {1} {0}]]]></Description>
 	  <Description state=""3""><![CDATA[&a_an[@@shorteat[, ]decayed severed {0}]]]></Description>
 	  <Description state=""4""><![CDATA[&a_an[@@shorteat[, ]heavily decayed severed {0}]]]></Description>
 	  <Description state=""5""><![CDATA[the @@shorteat[, ]skeletal remains of &a_an[severed {0}]]]></Description>
 	</PartDescriptions>
     <ShortDescriptions>
       <Description state=""0""><![CDATA[the @@shorteat[, ]fresh corpse of @@sdesc]]></Description>
       <Description state=""1""><![CDATA[the @@shorteat[, ]corpse of @@sdesc]]></Description>
       <Description state=""2""><![CDATA[the @@shorteat[, ]decaying corpse of @@sdesc]]></Description>
       <Description state=""3""><![CDATA[the @@shorteat[, ]decayed corpse of &a_an[&male &race]]]></Description>
       <Description state=""4""><![CDATA[the @@shorteat[, ]heavily decayed &race corpse]]></Description>
       <Description state=""5""><![CDATA[the @@shorteat[, ]skeletal remains of &a_an[&race]]]></Description>
     </ShortDescriptions>
     <FullDescriptions>
       <Description state=""0""><![CDATA[@@desc
 #3Death has taken hold of this individual, but they still look much the same as they did in life.#0
 
 @@eaten[
 
 ]@@wounds
 
 @@inv]]></Description>
       <Description state=""1""><![CDATA[@@desc
 #3Rigor Mortis has set in with this individual, and the blood has begun to pool in the extremities. Flies and other carrion insects have begun to lay eggs in the skin.#0
 
 @@eaten[
 
 ]@@wounds
 
 @@inv]]></Description>
       <Description state=""2""><![CDATA[@@desc
 #3This corpse has begun to bloat and putrefy as decay sets in. Maggots and other carrion insects have firmly taken hold of the corpse.#0
 
 @@eaten[
 
 ]@@wounds
 
 @@inv]]></Description>
       <Description state=""3""><![CDATA[This is the decayed corpse of a &male &race. The features are no longer easily recognisable, but in life, &he had $skincolour coloured skin and $?hairstyle[&a_an[$haircolour $hairstyle]][was completely bald].$?facialhairstyle[ &he had &?a_an[$facialhaircolour $facialhairstyle.]][] &he was @@height tall and $framefancy. &he had $distinctivefeaturefancy.
 #3This corpse is well along the process of decay. The flesh has sloughed off in places and particularly soft parts such as the eyes have been eaten by carrion insects.#0
 
 @@eaten[
 
 ]@@wounds
 
 @@inv]]></Description>
       <Description state=""4""><![CDATA[This is the heavily decayed corpse of a &race of indeterminate gender. The individual had $skincolour skin, $?hairstyle[and $haircolour hair.][but was bald.]$?facialhairstyle[ They had $facialhaircolour facial hair.][] &he was @@height tall.
 #3This corpse in a very advanced state of decay. The flesh is almost entirely gone and the corpse has largely liquified. At this stage, the only parts remaining are what the carrion eaters find tough to digest.#0
 
 @@eaten[
 
 ]@@inv]]></Description>
 		<Description state=""5""><![CDATA[This is the skeletal remains of a &race that was @@height tall. Nothing remains of their other features.
 
 @@eaten[
 
 ]@@inv]]></Description>
     </FullDescriptions> &#xA0; &#xA0;
     <ContentsDescriptions>
       <Description state=""0""><![CDATA[@@desc
 #3Death has taken hold of this individual, but they still look much the same as they did in life.#0
 
 @@eaten[
 
 ]@@wounds
 
 @@inv]]></Description>
       <Description state=""1""><![CDATA[@@desc
 #3Rigor Mortis has set in with this individual, and the blood has begun to pool in the extremities. Flies and other carrion insects have begun to lay eggs in the skin.#0
 
 @@eaten[
 
 ]@@wounds
 
 @@inv]]></Description>
       <Description state=""2""><![CDATA[@@desc
 #3This corpse has begun to bloat and putrefy as decay sets in. Maggots and other carrion insects have firmly taken hold of the corpse.#0
 
 @@eaten[
 
 ]@@wounds
 
 @@inv]]></Description>
       <Description state=""3""><![CDATA[This is the decayed corpse of a &male &race. The features are no longer easily recognisable, but in life, &he had $skincolour coloured skin and $?hairstyle[&a_an[$haircolour $hairstyle]][was completely bald].$?facialhairstyle[ &he had &?a_an[$facialhaircolour $facialhairstyle.]][] &he was @@height tall and $framefancy. &he had $distinctivefeaturefancy.
 #3This corpse is well along the process of decay. The flesh has sloughed off in places and particularly soft parts such as the eyes have been eaten by carrion insects.#0
 
 @@eaten[
 
 ]@@wounds
 
 @@inv]]></Description>
       <Description state=""4""><![CDATA[This is the heavily decayed corpse of a &race of indeterminate gender. The individual had $skincolour skin, $?hairstyle[and $haircolour hair.][but was bald.]$?facialhairstyle[ They had $facialhaircolour facial hair.][] &he was @@height tall.
 #3This corpse in a very advanced state of decay. The flesh is almost entirely gone and the corpse has largely liquified. At this stage, the only parts remaining are what the carrion eaters find tough to digest.#0
 
 @@eaten[
 
 ]@@inv]]></Description>
 		<Description state=""5""><![CDATA[This is the skeletal remains of a &race that was @@height tall. Nothing remains of their other features.
 
 @@eaten[
 
 ]@@inv]]></Description>
     </ContentsDescriptions>
   </Descriptions>
 </CorpseModel>
 "
				: @"<?xml version=""1.0""?>
 <CorpseModel>
   <EdiblePercentage>0.35</EdiblePercentage>
   <Ranges>
     <Range state=""0"" lower=""0"" upper=""7200""/>
     <Range state=""1"" lower=""7200"" upper=""21600""/>
     <Range state=""2"" lower=""21600"" upper=""65000""/>
     <Range state=""3"" lower=""65000"" upper=""400000""/>
     <Range state=""4"" lower=""400000"" upper=""2650000""/>
     <Range state=""5"" lower=""2650000"" upper=""2650001""/>
   </Ranges>
   <Terrains default=""10"">
     <Terrain terrain=""void"" rate=""0""/>
   </Terrains>
   <Descriptions>
     <PartDescriptions>
 	  <Description state=""0""><![CDATA[&a_an[@@shorteat[, ]freshly severed {1} {0}]]]></Description>
 	  <Description state=""1""><![CDATA[&a_an[@@shorteat[, ]severed {1} {0}]]]></Description>
 	  <Description state=""2""><![CDATA[&a_an[@@shorteat[, ]decaying severed {1} {0}]]]></Description>
 	  <Description state=""3""><![CDATA[&a_an[@@shorteat[, ]decayed severed {0}]]]></Description>
 	  <Description state=""4""><![CDATA[&a_an[@@shorteat[, ]heavily decayed severed {0}]]]></Description>
 	  <Description state=""5""><![CDATA[the @@shorteat[, ]skeletal remains of &a_an[severed {0}]]]></Description>
 	</PartDescriptions>
     <ShortDescriptions>
       <Description state=""0""><![CDATA[the @@shorteat[, ]fresh corpse of @@sdesc]]></Description>
       <Description state=""1""><![CDATA[the @@shorteat[, ]corpse of @@sdesc]]></Description>
       <Description state=""2""><![CDATA[the @@shorteat[, ]decaying corpse of @@sdesc]]></Description>
       <Description state=""3""><![CDATA[the @@shorteat[, ]decayed corpse of &a_an[&male &race]]]></Description>
       <Description state=""4""><![CDATA[the @@shorteat[, ]heavily decayed &race corpse]]></Description>
       <Description state=""5""><![CDATA[the @@shorteat[, ]skeletal remains of &a_an[&race]]]></Description>
     </ShortDescriptions>
     <FullDescriptions>
       <Description state=""0""><![CDATA[@@desc
 #3Death has taken hold of this individual, but they still look much the same as they did in life.#0
 
 @@eaten[
 
 ]@@wounds
 
 @@inv]]></Description>
       <Description state=""1""><![CDATA[@@desc
 #3Rigor Mortis has set in with this individual, and the blood has begun to pool in the extremities. Flies and other carrion insects have begun to lay eggs in the skin.#0
 
 @@eaten[
 
 ]@@wounds
 
 @@inv]]></Description>
       <Description state=""2""><![CDATA[@@desc
 #3This corpse has begun to bloat and putrefy as decay sets in. Maggots and other carrion insects have firmly taken hold of the corpse.#0
 
 @@eaten[
 
 ]@@wounds
 
 @@inv]]></Description>
       <Description state=""3""><![CDATA[This is the decayed corpse of a &male &race. The features are no longer easily recognisable, but in life, &he had $skincolour coloured skin and $?hairstyle[&a_an[$haircolour $hairstyle]][was completely bald].$?facialhairstyle[ &he had &?a_an[$facialhaircolour $facialhairstyle.]][] &he was @@height tall and $framefancy.
 #3This corpse is well along the process of decay. The flesh has sloughed off in places and particularly soft parts such as the eyes have been eaten by carrion insects.#0
 
 @@eaten[
 
 ]@@wounds
 
 @@inv]]></Description>
       <Description state=""4""><![CDATA[This is the heavily decayed corpse of a &race of indeterminate gender. The individual had $skincolour skin, $?hairstyle[and $haircolour hair.][but was bald.]$?facialhairstyle[ They had $facialhaircolour facial hair.][] &he was @@height tall.
 #3This corpse in a very advanced state of decay. The flesh is almost entirely gone and the corpse has largely liquified. At this stage, the only parts remaining are what the carrion eaters find tough to digest.#0
 
 @@eaten[
 
 ]@@inv]]></Description>
 		<Description state=""5""><![CDATA[This is the skeletal remains of a &race that was @@height tall. Nothing remains of their other features.
 
 @@eaten[
 
 ]@@inv]]></Description>
     </FullDescriptions> &#xA0; &#xA0;
     <ContentsDescriptions>
       <Description state=""0""><![CDATA[@@desc
 #3Death has taken hold of this individual, but they still look much the same as they did in life.#0
 
 @@eaten[
 
 ]@@wounds
 
 @@inv]]></Description>
       <Description state=""1""><![CDATA[@@desc
 #3Rigor Mortis has set in with this individual, and the blood has begun to pool in the extremities. Flies and other carrion insects have begun to lay eggs in the skin.#0
 
 @@eaten[
 
 ]@@wounds
 
 @@inv]]></Description>
       <Description state=""2""><![CDATA[@@desc
 #3This corpse has begun to bloat and putrefy as decay sets in. Maggots and other carrion insects have firmly taken hold of the corpse.#0
 
 @@eaten[
 
 ]@@wounds
 
 @@inv]]></Description>
       <Description state=""3""><![CDATA[This is the decayed corpse of a &male &race. The features are no longer easily recognisable, but in life, &he had $skincolour coloured skin and $?hairstyle[&a_an[$haircolour $hairstyle]][was completely bald].$?facialhairstyle[ &he had &?a_an[$facialhaircolour $facialhairstyle.]][] &he was @@height tall and $framefancy.
 #3This corpse is well along the process of decay. The flesh has sloughed off in places and particularly soft parts such as the eyes have been eaten by carrion insects.#0
 
 @@eaten[
 
 ]@@wounds
 
 @@inv]]></Description>
       <Description state=""4""><![CDATA[This is the heavily decayed corpse of a &race of indeterminate gender. The individual had $skincolour skin, $?hairstyle[and $haircolour hair.][but was bald.]$?facialhairstyle[ They had $facialhaircolour facial hair.][] &he was @@height tall.
 #3This corpse in a very advanced state of decay. The flesh is almost entirely gone and the corpse has largely liquified. At this stage, the only parts remaining are what the carrion eaters find tough to digest.#0
 
 @@eaten[
 
 ]@@inv]]></Description>
 		<Description state=""5""><![CDATA[This is the skeletal remains of a &race that was @@height tall. Nothing remains of their other features.
 
 @@eaten[
 
 ]@@inv]]></Description>
     </ContentsDescriptions>
   </Descriptions>
 </CorpseModel>
 "
		};
		_context.CorpseModels.Add(organicHumanCorpse);

		#endregion region

		#region Progs

		var attributeBonusProg = new FutureProg
		{
			FunctionName = "HumanAttributeBonus",
			FunctionComment =
				"This prog is called for each attribute for humans at chargen time and the resulting value is applied as a modifier to that attribute.",
			Category = "Character",
			Subcategory = "Race",
			ReturnType = 2,
			AcceptsAnyParameters = false,
			StaticType = 0,
			FunctionText =
				@"// Replace this below example with your specific modifiers
if (@trait.Name == ""Example"")
  return 2
end if
return 0"
		};
		attributeBonusProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = attributeBonusProg,
			ParameterName = "trait",
			ParameterIndex = 0,
			ParameterType = 16384
		});
		_context.FutureProgs.Add(attributeBonusProg);

		#endregion

		#region Blood Models

		var bloodModel = new BloodModel
		{
			Name = "ABO+Rh"
		};
		_context.BloodModels.Add(bloodModel);

		var nextId = _context.BloodtypeAntigens.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;
		var aAntigen = new BloodtypeAntigen
		{
			Id = nextId++,
			Name = "A Antigen"
		};
		_context.BloodtypeAntigens.Add(aAntigen);
		var bAntigen = new BloodtypeAntigen
		{
			Id = nextId++,
			Name = "B Antigen"
		};
		_context.BloodtypeAntigens.Add(bAntigen);
		var rhAntigen = new BloodtypeAntigen
		{
			Id = nextId++,
			Name = "Rh Antigen"
		};
		_context.BloodtypeAntigens.Add(rhAntigen);

		var bloodTypes = new Dictionary<string, Bloodtype>(StringComparer.OrdinalIgnoreCase);
		nextId = _context.Bloodtypes.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;
		var bloodType = new Bloodtype { Id = nextId++, Name = "O+" };
		bloodModel.BloodModelsBloodtypes.Add(new BloodModelsBloodtypes
			{ BloodModel = bloodModel, Bloodtype = bloodType });
		bloodTypes[bloodType.Name] = bloodType;
		_context.Bloodtypes.Add(bloodType);

		bloodType = new Bloodtype { Id = nextId++, Name = "O-" };
		bloodModel.BloodModelsBloodtypes.Add(new BloodModelsBloodtypes
			{ BloodModel = bloodModel, Bloodtype = bloodType });
		bloodType.BloodtypesBloodtypeAntigens.Add(new BloodtypesBloodtypeAntigens
			{ Bloodtype = bloodType, BloodtypeAntigen = rhAntigen });
		bloodTypes[bloodType.Name] = bloodType;
		_context.Bloodtypes.Add(bloodType);

		bloodType = new Bloodtype { Id = nextId++, Name = "A+" };
		bloodModel.BloodModelsBloodtypes.Add(new BloodModelsBloodtypes
			{ BloodModel = bloodModel, Bloodtype = bloodType });
		bloodType.BloodtypesBloodtypeAntigens.Add(new BloodtypesBloodtypeAntigens
			{ Bloodtype = bloodType, BloodtypeAntigen = aAntigen });
		bloodTypes[bloodType.Name] = bloodType;
		_context.Bloodtypes.Add(bloodType);

		bloodType = new Bloodtype { Id = nextId++, Name = "A-" };
		bloodModel.BloodModelsBloodtypes.Add(new BloodModelsBloodtypes
			{ BloodModel = bloodModel, Bloodtype = bloodType });
		bloodType.BloodtypesBloodtypeAntigens.Add(new BloodtypesBloodtypeAntigens
			{ Bloodtype = bloodType, BloodtypeAntigen = aAntigen });
		bloodType.BloodtypesBloodtypeAntigens.Add(new BloodtypesBloodtypeAntigens
			{ Bloodtype = bloodType, BloodtypeAntigen = rhAntigen });
		bloodTypes[bloodType.Name] = bloodType;
		_context.Bloodtypes.Add(bloodType);

		bloodType = new Bloodtype { Id = nextId++, Name = "B+" };
		bloodModel.BloodModelsBloodtypes.Add(new BloodModelsBloodtypes
			{ BloodModel = bloodModel, Bloodtype = bloodType });
		bloodType.BloodtypesBloodtypeAntigens.Add(new BloodtypesBloodtypeAntigens
			{ Bloodtype = bloodType, BloodtypeAntigen = bAntigen });
		bloodTypes[bloodType.Name] = bloodType;
		_context.Bloodtypes.Add(bloodType);

		bloodType = new Bloodtype { Id = nextId++, Name = "B-" };
		bloodModel.BloodModelsBloodtypes.Add(new BloodModelsBloodtypes
			{ BloodModel = bloodModel, Bloodtype = bloodType });
		bloodType.BloodtypesBloodtypeAntigens.Add(new BloodtypesBloodtypeAntigens
			{ Bloodtype = bloodType, BloodtypeAntigen = bAntigen });
		bloodType.BloodtypesBloodtypeAntigens.Add(new BloodtypesBloodtypeAntigens
			{ Bloodtype = bloodType, BloodtypeAntigen = rhAntigen });
		bloodTypes[bloodType.Name] = bloodType;
		_context.Bloodtypes.Add(bloodType);

		bloodType = new Bloodtype { Id = nextId++, Name = "AB+" };
		bloodModel.BloodModelsBloodtypes.Add(new BloodModelsBloodtypes
			{ BloodModel = bloodModel, Bloodtype = bloodType });
		bloodType.BloodtypesBloodtypeAntigens.Add(new BloodtypesBloodtypeAntigens
			{ Bloodtype = bloodType, BloodtypeAntigen = bAntigen });
		bloodType.BloodtypesBloodtypeAntigens.Add(new BloodtypesBloodtypeAntigens
			{ Bloodtype = bloodType, BloodtypeAntigen = aAntigen });
		bloodTypes[bloodType.Name] = bloodType;
		_context.Bloodtypes.Add(bloodType);

		bloodType = new Bloodtype { Id = nextId++, Name = "AB-" };
		bloodModel.BloodModelsBloodtypes.Add(new BloodModelsBloodtypes
			{ BloodModel = bloodModel, Bloodtype = bloodType });
		bloodType.BloodtypesBloodtypeAntigens.Add(new BloodtypesBloodtypeAntigens
			{ Bloodtype = bloodType, BloodtypeAntigen = bAntigen });
		bloodType.BloodtypesBloodtypeAntigens.Add(new BloodtypesBloodtypeAntigens
			{ Bloodtype = bloodType, BloodtypeAntigen = aAntigen });
		bloodType.BloodtypesBloodtypeAntigens.Add(new BloodtypesBloodtypeAntigens
			{ Bloodtype = bloodType, BloodtypeAntigen = rhAntigen });
		bloodTypes[bloodType.Name] = bloodType;
		_context.Bloodtypes.Add(bloodType);
		_context.SaveChanges();

		nextId = _context.PopulationBloodModels.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;
		var popModel = new PopulationBloodModel { Id = nextId++, Name = "Overwhelmingly O" };
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["O+"], Weight = 70 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["O-"], Weight = 1.5 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["A+"], Weight = 15 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["A-"], Weight = 0.5 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["B+"], Weight = 7 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["B-"], Weight = 0.3 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["AB+"], Weight = 1 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["AB-"], Weight = 0.02 });
		_context.PopulationBloodModels.Add(popModel);

		popModel = new PopulationBloodModel { Id = nextId++, Name = "Majority O" };
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["O+"], Weight = 60 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["O-"], Weight = 3 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["A+"], Weight = 15 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["A-"], Weight = 1.3 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["B+"], Weight = 12 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["B-"], Weight = 0.7 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["AB+"], Weight = 2 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["AB-"], Weight = 0.2 });
		_context.PopulationBloodModels.Add(popModel);

		popModel = new PopulationBloodModel { Id = nextId++, Name = "Majority O Minor A" };
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["O+"], Weight = 60 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["O-"], Weight = 3 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["A+"], Weight = 25 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["A-"], Weight = 1.5 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["B+"], Weight = 5 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["B-"], Weight = 0.4 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["AB+"], Weight = 2 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["AB-"], Weight = 0.2 });
		_context.PopulationBloodModels.Add(popModel);

		popModel = new PopulationBloodModel { Id = nextId++, Name = "B Dominant" };
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["O+"], Weight = 30 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["O-"], Weight = 2.5 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["A+"], Weight = 20 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["A-"], Weight = 2.7 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["B+"], Weight = 35 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["B-"], Weight = 7 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["AB+"], Weight = 6 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["AB-"], Weight = 0.6 });
		_context.PopulationBloodModels.Add(popModel);

		popModel = new PopulationBloodModel { Id = nextId++, Name = "A Dominant" };
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["O+"], Weight = 30 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["O-"], Weight = 5 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["A+"], Weight = 35 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["A-"], Weight = 7 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["B+"], Weight = 15 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["B-"], Weight = 1.5 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["AB+"], Weight = 2 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["AB-"], Weight = 0.4 });
		_context.PopulationBloodModels.Add(popModel);

		popModel = new PopulationBloodModel { Id = nextId++, Name = "O-A High Negative" };
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["O+"], Weight = 50 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["O-"], Weight = 10 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["A+"], Weight = 25 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["A-"], Weight = 7 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["B+"], Weight = 9 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["B-"], Weight = 0.1 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["AB+"], Weight = 1 });
		popModel.PopulationBloodModelsBloodtypes.Add(new PopulationBloodModelsBloodtype
			{ PopulationBloodModel = popModel, Bloodtype = bloodTypes["AB-"], Weight = 0.2 });
		_context.PopulationBloodModels.Add(popModel);

		#endregion

		#region Blood and Sweat Liquids

		var driedBlood = new Material
		{
			Name = "Dried Human Blood",
			MaterialDescription = "dried blood",
			Density = 1520,
			Organic = true,
			Type = 0,
			BehaviourType = 19,
			ThermalConductivity = 0.2,
			ElectricalConductivity = 0.0001,
			SpecificHeatCapacity = 420,
			IgnitionPoint = 555.3722,
			HeatDamagePoint = 412.0389,
			ImpactFracture = 1000,
			ImpactYield = 1000,
			ImpactStrainAtYield = 2,
			ShearFracture = 1000,
			ShearYield = 1000,
			ShearStrainAtYield = 2,
			YoungsModulus = 0.1,
			SolventId = 1,
			SolventVolumeRatio = 4,
			ResidueDesc = "It is covered in {0}dried blood",
			ResidueColour = "red",
			Absorbency = 0
		};
		_context.Materials.Add(driedBlood);
		var blood = new Liquid
		{
			Name = "Human Blood",
			Description = "blood",
			LongDescription = "a virtually opaque dark red fluid",
			TasteText = "It has a sharply metallic, umami taste",
			VagueTasteText = "It has a metallic taste",
			SmellText = "It has a metallic, coppery smell",
			VagueSmellText = "It has a faintly metallic smell",
			TasteIntensity = 200,
			SmellIntensity = 10,
			AlcoholLitresPerLitre = 0,
			WaterLitresPerLitre = 0.8,
			DrinkSatiatedHoursPerLitre = 6,
			FoodSatiatedHoursPerLitre = 4,
			CaloriesPerLitre = 800,
			Viscosity = 1,
			Density = 1,
			Organic = true,
			ThermalConductivity = 0.609,
			ElectricalConductivity = 0.005,
			SpecificHeatCapacity = 4181,
			FreezingPoint = -20,
			BoilingPoint = 100,
			DisplayColour = "bold red",
			DampDescription = "It is damp with blood",
			WetDescription = "It is wet with blood",
			DrenchedDescription = "It is drenched with blood",
			DampShortDescription = "(blood damp)",
			WetShortDescription = "(bloody)",
			DrenchedShortDescription = "(blood drenched)",
			SolventId = 1,
			SolventVolumeRatio = 5,
			InjectionConsequence = (int)LiquidInjectionConsequence.BloodReplacement,
			ResidueVolumePercentage = 0.05,
			DriedResidue = driedBlood,
			CountAsQuality = (int)ItemQuality.Legendary,
			CountAs = _context.Liquids.First(x => x.Name == "Blood")
		};
		_context.Liquids.Add(blood);

		var driedSweat = new Material
		{
			Name = "Dried Human Sweat",
			MaterialDescription = "dried sweat",
			Density = 1520,
			Organic = true,
			Type = 0,
			BehaviourType = 19,
			ThermalConductivity = 0.2,
			ElectricalConductivity = 0.0001,
			SpecificHeatCapacity = 420,
			IgnitionPoint = 555.3722,
			HeatDamagePoint = 412.0389,
			ImpactFracture = 1000,
			ImpactYield = 1000,
			ImpactStrainAtYield = 2,
			ShearFracture = 1000,
			ShearYield = 1000,
			ShearStrainAtYield = 2,
			YoungsModulus = 0.1,
			SolventId = 1,
			SolventVolumeRatio = 3,
			ResidueDesc = "It is covered in {0}dried sweat",
			ResidueColour = "yellow",
			Absorbency = 0
		};
		_context.Materials.Add(driedSweat);
		var sweat = new Liquid
		{
			Name = "Human Sweat",
			Description = "sweat",
			LongDescription = "a relatively clear, translucent fluid that smells strongly of body odor",
			TasteText = "It tastes like a pungent, salty lick of someone's underarms",
			VagueTasteText = "It tastes very unpleasant, like underarm stench",
			SmellText = "It has the sharp, pungent smell of body odor",
			VagueSmellText = "It has the sharp, pungent smell of body odor",
			TasteIntensity = 200,
			SmellIntensity = 200,
			AlcoholLitresPerLitre = 0,
			WaterLitresPerLitre = 0.95,
			DrinkSatiatedHoursPerLitre = 5,
			FoodSatiatedHoursPerLitre = 0,
			CaloriesPerLitre = 0,
			Viscosity = 1,
			Density = 1,
			Organic = true,
			ThermalConductivity = 0.609,
			ElectricalConductivity = 0.005,
			SpecificHeatCapacity = 4181,
			FreezingPoint = -20,
			BoilingPoint = 100,
			DisplayColour = "yellow",
			DampDescription = "It is damp with sweat",
			WetDescription = "It is wet and smelly with sweat",
			DrenchedDescription = "It is soaking wet and smelly with sweat",
			DampShortDescription = "(sweat-damp)",
			WetShortDescription = "(sweaty)",
			DrenchedShortDescription = "(sweat-drenched)",
			SolventId = 1,
			SolventVolumeRatio = 5,
			InjectionConsequence = (int)LiquidInjectionConsequence.Harmful,
			ResidueVolumePercentage = 0.05,
			DriedResidue = driedSweat,
			CountAsQuality = (int)ItemQuality.Legendary,
			CountAs = _context.Liquids.First(x => x.Name == "Sweat")
		};
		_context.Liquids.Add(sweat);

		#endregion

		#region Breathable Air

		var air = new Gas
		{
			Id = 1, Name = "Breathable Atmosphere", Description = "Breathable Air", Density = 0.001205,
			ThermalConductivity = 0.0257, ElectricalConductivity = 0.000005, Organic = false,
			SpecificHeatCapacity = 1.005, BoilingPoint = -200, DisplayColour = "blue", Viscosity = 15,
			SmellIntensity = 0, SmellText = "It has no smell", VagueSmellText = "It has no smell"
		};
		_context.Gases.Add(air);
		_context.SaveChanges();

		_context.Terrains.First().AtmosphereId = air.Id;
		_context.Terrains.First().AtmosphereType = "Gas";
		_context.CellOverlays.First().AtmosphereId = air.Id;
		_context.CellOverlays.First().AtmosphereType = "Gas";

		#endregion

		_context.SaveChanges();

		var useNonBinary = _questionAnswers["nonbinary"].ToLowerInvariant().In("yes", "y");
		nextId = _context.Races.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;
		var humanoidRace = new Race
		{
			Id = nextId++,
			Name = "Humanoid",
			Description =
				"The base humanoid race. This race should never be the final race of something; it is designed to be a base race for other humanoids.",
			BaseBody = baseBody,
			AllowedGenders = "2 3",
			AttributeBonusProg = _context.FutureProgs.First(x => x.FunctionName == "AlwaysZero"),
			AttributeTotalCap = _context.TraitDefinitions.Count(x => x.Type == 1) * 12,
			IndividualAttributeCap = 20,
			DiceExpression = "3d6+1",
			IlluminationPerceptionMultiplier = 1.0,
			AvailabilityProg = _context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse"),
			CorpseModel = organicHumanCorpse,
			DefaultHealthStrategy = strategy,
			CanUseWeapons = true,
			CanAttack = true,
			CanDefend = true,
			NeedsToBreathe = true,
			SizeStanding = 6,
			SizeProne = 6,
			SizeSitting = 6,
			CommunicationStrategyType = "humanoid",
			HandednessOptions = "1 3",
			DefaultHandedness = 1,
			ChildAge = 3,
			YouthAge = 10,
			YoungAdultAge = 16,
			AdultAge = 25,
			ElderAge = 50,
			VenerableAge = 75,
			CanClimb = true,
			CanSwim = true,
			MinimumSleepingPosition = 4,
			BodypartHealthMultiplier = 1,
			BodypartSizeModifier = 0,
			TemperatureRangeCeiling = 40,
			TemperatureRangeFloor = 0,
			CanEatCorpses = false,
			CanEatMaterialsOptIn = false,
			BiteWeight = 1000,
			EatCorpseEmoteText = "",
			RaceUsesStamina = true,
			NaturalArmourQuality = 2,
			NaturalArmourType = _naturalArmour,
			SweatLiquid = sweat,
			SweatRateInLitresPerMinute = 0.8,
			BloodLiquid = blood,
			BloodModel = bloodModel,
			BreathingVolumeExpression = "7",
			HoldBreathLengthExpression = $"90+(5*con:{healthTrait.Id})",
			MaximumLiftWeightExpression = $"str:{strengthTrait.Id}*10000",
			MaximumDragWeightExpression = $"str:{strengthTrait.Id}*40000"
		};
		_context.Races.Add(humanoidRace);
		_context.SaveChanges();

		var organicHumanoidRace = new Race
		{
			Id = nextId++,
			Name = "Organic Humanoid",
			Description =
				"The base organic humanoid race. This race should never be the final race of something; it is designed to be a base race for other organic humanoids.",
			BaseBody = baseBody,
			AllowedGenders = "2 3",
			AttributeBonusProg = _context.FutureProgs.First(x => x.FunctionName == "AlwaysZero"),
			AttributeTotalCap = _context.TraitDefinitions.Count(x => x.Type == 1) * 12,
			IndividualAttributeCap = 20,
			DiceExpression = "3d6+1",
			IlluminationPerceptionMultiplier = 1.0,
			AvailabilityProg = _context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse"),
			CorpseModel = organicHumanCorpse,
			DefaultHealthStrategy = strategy,
			CanUseWeapons = true,
			CanAttack = true,
			CanDefend = true,
			NeedsToBreathe = true,
			SizeStanding = 6,
			SizeProne = 6,
			SizeSitting = 6,
			CommunicationStrategyType = "humanoid",
			HandednessOptions = "1 3",
			DefaultHandedness = 1,
			ChildAge = 3,
			YouthAge = 10,
			YoungAdultAge = 16,
			AdultAge = 25,
			ElderAge = 50,
			VenerableAge = 75,
			CanClimb = true,
			CanSwim = true,
			MinimumSleepingPosition = 4,
			BodypartHealthMultiplier = 1,
			BodypartSizeModifier = 0,
			TemperatureRangeCeiling = 40,
			TemperatureRangeFloor = 0,
			CanEatCorpses = false,
			CanEatMaterialsOptIn = false,
			BiteWeight = 1000,
			EatCorpseEmoteText = "",
			RaceUsesStamina = true,
			NaturalArmourQuality = 2,
			NaturalArmourType = _naturalArmour,
			SweatLiquid = sweat,
			SweatRateInLitresPerMinute = 0.8,
			BloodLiquid = blood,
			BloodModel = bloodModel,
			BreathingVolumeExpression = "7",
			HoldBreathLengthExpression = $"90+(5*con:{healthTrait.Id})",
			MaximumLiftWeightExpression = $"str:{strengthTrait.Id}*10000",
			MaximumDragWeightExpression = $"str:{strengthTrait.Id}*40000",
			ParentRace = humanoidRace
		};
		_context.Races.Add(organicHumanoidRace);
		organicHumanoidRace.DefaultHeightWeightModelMale = _humanMaleHWModel;
		organicHumanoidRace.DefaultHeightWeightModelFemale = _humanFemaleHWModel;
		organicHumanoidRace.DefaultHeightWeightModelNonBinary = _humanFemaleHWModel;
		organicHumanoidRace.DefaultHeightWeightModelNeuter = _humanMaleHWModel;
		_context.SaveChanges();

		var human = new Race
		{
			Id = nextId++,
			Name = "Human",
			Description =
				"In the reckonings of most worlds, humans are the youngest of the common races, late to arrive on the world scene and short-lived in comparison to dwarves, elves, and dragons. Perhaps it is because of their shorter lives that they strive to achieve as much as they can in the years they are given. Or maybe they feel they have something to prove to the elder races, and that’s why they build their mighty empires on the foundation of conquest and trade. Whatever drives them, humans are the innovators, the achievers, and the pioneers of the worlds.",
			BaseBody = organicBody,
			AllowedGenders = useNonBinary ? "2 3 4" : "2 3",
			AttributeBonusProg = attributeBonusProg,
			AttributeTotalCap = _context.TraitDefinitions.Count(x => x.Type == 1) * 12,
			IndividualAttributeCap = 20,
			DiceExpression = "3d6+1",
			IlluminationPerceptionMultiplier = 1.0,
			AvailabilityProg = _context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
			CorpseModel = organicHumanCorpse,
			DefaultHealthStrategy = strategy,
			CanUseWeapons = true,
			CanAttack = true,
			CanDefend = true,
			NeedsToBreathe = true,
			SizeStanding = 6,
			SizeProne = 6,
			SizeSitting = 6,
			CommunicationStrategyType = "humanoid",
			HandednessOptions = "1 3",
			DefaultHandedness = 1,
			ChildAge = 3,
			YouthAge = 10,
			YoungAdultAge = 16,
			AdultAge = 25,
			ElderAge = 50,
			VenerableAge = 75,
			CanClimb = true,
			CanSwim = true,
			MinimumSleepingPosition = 4,
			BodypartHealthMultiplier = 1,
			BodypartSizeModifier = 0,
			TemperatureRangeCeiling = 40,
			TemperatureRangeFloor = 0,
			CanEatCorpses = false,
			CanEatMaterialsOptIn = false,
			BiteWeight = 1000,
			EatCorpseEmoteText = "",
			RaceUsesStamina = true,
			NaturalArmourQuality = 2,
			NaturalArmourType = _naturalArmour,
			SweatLiquid = sweat,
			SweatRateInLitresPerMinute = 0.8,
			BloodLiquid = blood,
			BloodModel = bloodModel,
			BreathingVolumeExpression = "7",
			HoldBreathLengthExpression = $"90+(5*con:{healthTrait.Id})",
			MaximumLiftWeightExpression = $"str:{strengthTrait.Id}*10000",
			MaximumDragWeightExpression = $"str:{strengthTrait.Id}*40000",
			ParentRace = organicHumanoidRace
		};
		_context.Races.Add(human);
		human.DefaultHeightWeightModelMale = _humanMaleHWModel;
		human.DefaultHeightWeightModelFemale = _humanFemaleHWModel;
		human.DefaultHeightWeightModelNonBinary = _humanFemaleHWModel;
		human.DefaultHeightWeightModelNeuter = _humanMaleHWModel;
		_context.SaveChanges();

		_context.RacesBreathableGases.Add(new RacesBreathableGases
			{ Race = organicHumanoidRace, Gas = air, Multiplier = 1.0 });
		foreach (var attribute in _context.TraitDefinitions.Where(x => x.Type == 1 || x.Type == 3))
			_context.RacesAttributes.Add(new RacesAttributes
			{
				Race = organicHumanoidRace, Attribute = attribute,
				IsHealthAttribute = attribute.TraitGroup == "Physical"
			});
		_context.SaveChanges();
		return human;
	}

	private void SetupDescriptions()
	{
		var humanProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoid");
		var femaleProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidFemale");
		var nonFemaleProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidNonFemale");

		#region SDescs

		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$eyeshape[%eyeshape[$eyeshape-eyed][1:one-eyed][0:eyeless] ][]$person] with $eyecolour[%eyecolour[$eyecolour eyes][1:one $eyecolour eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$frame[@ ][]$person] with $eyecolour[%eyecolour[$eyecolour eyes][1:one $eyecolour eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$distinctivefeaturebasic[@ ][]$person] with $eyecolour[%eyecolour[$eyecolour eyes][1:one $eyecolour eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$ears[%ears[$earsbasic][1:one-eared][0:earless] ][]$person] with $eyecolour[%eyecolour[$eyecolour eyes][1:one $eyecolour eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$nose[%nose[$nosebasic][0:noseless] ][]$person] with $eyecolour[%eyecolour[$eyecolour eyes][1:one $eyecolour eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$person] with $eyecolour[%eyecolour[$eyecolour eyes][1:one $eyecolour eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$haircolourbasic[@-haired ][]$person] with $eyecolour[%eyecolour[$eyecolour eyes][1:one $eyecolour eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$hairstylebasic[@ ][]$person] with $eyecolour[%eyecolour[$eyecolour eyes][1:one $eyecolour eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$facialhairstylebasic[@ ][]$person] with $eyecolour[%eyecolour[$eyecolour eyes][1:one $eyecolour eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$skincolour[@-skinned ][]$person] with $eyecolour[%eyecolour[$eyecolour eyes][1:one $eyecolour eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[&tattoos $person] with $eyecolour[%eyecolour[$eyecolour eyes][1:one $eyecolour eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[&scars $person] with $eyecolour[%eyecolour[$eyecolour eyes][1:one $eyecolour eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$eyecolour[%eyecolour[$eyecolourbasic-eyed][1:one-eyed][0:eyeless] ][]$person] with $eyeshape[%eyeshape[$eyeshape eyes][1:one $eyeshape eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$frame[@ ][]$person] with $eyeshape[%eyeshape[$eyeshape eyes][1:one $eyeshape eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$distinctivefeaturebasic[@ ][]$person] with $eyeshape[%eyeshape[$eyeshape eyes][1:one $eyeshape eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$ears[%ears[$earsbasic][1:one-eared][0:earless] ][]$person] with $eyeshape[%eyeshape[$eyeshape eyes][1:one $eyeshape eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$nose[%nose[$nosebasic][0:noseless] ][]$person] with $eyeshape[%eyeshape[$eyeshape eyes][1:one $eyeshape eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$person] with $eyeshape[%eyeshape[$eyeshape eyes][1:one $eyeshape eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$haircolourbasic[@-haired ][]$person] with $eyeshape[%eyeshape[$eyeshape eyes][1:one $eyeshape eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$hairstylebasic[@ ][]$person] with $eyeshape[%eyeshape[$eyeshape eyes][1:one $eyeshape eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$facialhairstylebasic[@ ][]$person] with $eyeshape[%eyeshape[$eyeshape eyes][1:one $eyeshape eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$skincolour[@-skinned ][]$person] with $eyeshape[%eyeshape[$eyeshape eyes][1:one $eyeshape eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[&tattoos $person] with $eyeshape[%eyeshape[$eyeshape eyes][1:one $eyeshape eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[&scars $person] with $eyeshape[%eyeshape[$eyeshape eyes][1:one $eyeshape eye][0:no eyes]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$eyecolour[%eyecolour[$eyecolourbasic-eyed][1:one-eyed][0:eyeless] ][]$person] of $frame build"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$eyeshape[%eyeshape[$eyeshape-eyed][1:one-eyed][0:eyeless] ][]$person] of $frame build"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$distinctivefeaturebasic[@ ][]$person] of $frame build"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$ears[%ears[$earsbasic][1:one-eared][0:earless] ][]$person] of $frame build"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$nose[%nose[$nosebasic][0:noseless] ][]$person] of $frame build"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100, Pattern = "&a_an[$person] of $frame build"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$haircolourbasic[@-haired ][]$person] of $frame build"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$hairstylebasic[@ ][]$person] of $frame build"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern = "&a_an[$facialhairstylebasic[@ ][]$person] of $frame build"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$skincolour[@-skinned ][]$person] of $frame build"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[&tattoos $person] of $frame build"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[&scars $person] of $frame build"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$eyecolour[%eyecolour[$eyecolourbasic-eyed][1:one-eyed][0:eyeless] ][]$person] with &?a_an[$distinctivefeature]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$eyeshape[%eyeshape[$eyeshape-eyed][1:one-eyed][0:eyeless] ][]$person] with &?a_an[$distinctivefeature]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$frame[@ ][]$person] with &?a_an[$distinctivefeature]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$ears[%ears[$earsbasic][1:one-eared][0:earless] ][]$person] with &?a_an[$distinctivefeature]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$nose[%nose[$nosebasic][0:noseless] ][]$person] with &?a_an[$distinctivefeature]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$person] with &?a_an[$distinctivefeature]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$haircolourbasic[@-haired ][]$person] with &?a_an[$distinctivefeature]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$hairstylebasic[@ ][]$person] with &?a_an[$distinctivefeature]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern = "&a_an[$facialhairstylebasic[@ ][]$person] with &?a_an[$distinctivefeature]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$skincolour[@-skinned ][]$person] with &?a_an[$distinctivefeature]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[&tattoos $person] with &?a_an[$distinctivefeature]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[&scars $person] with &?a_an[$distinctivefeature]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$eyecolour[%eyecolour[$eyecolourbasic-eyed][1:one-eyed][0:eyeless] ][]$person] with $ears[%ears[$ears ears][1:one $ears ear][0:no ears]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$eyeshape[%eyeshape[$eyeshape-eyed][1:one-eyed][0:eyeless] ][]$person] with $ears[%ears[$ears ears][1:one $ears ear][0:no ears]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$frame[@ ][]$person] with $ears[%ears[$ears ears][1:one $ears ear][0:no ears]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$distinctivefeaturebasic[@ ][]$person] with $ears[%ears[$ears ears][1:one $ears ear][0:no ears]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$nose[%nose[$nosebasic][0:noseless] ][]$person] with $ears[%ears[$ears ears][1:one $ears ear][0:no ears]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$person] with $ears[%ears[$ears ears][1:one $ears ear][0:no ears]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$haircolourbasic[@-haired ][]$person] with $ears[%ears[$ears ears][1:one $ears ear][0:no ears]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$hairstylebasic[@ ][]$person] with $ears[%ears[$ears ears][1:one $ears ear][0:no ears]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$facialhairstylebasic[@ ][]$person] with $ears[%ears[$ears ears][1:one $ears ear][0:no ears]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$skincolour[@-skinned ][]$person] with $ears[%ears[$ears ears][1:one $ears ear][0:no ears]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[&tattoos $person] with $ears[%ears[$ears ears][1:one $ears ear][0:no ears]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[&scars $person] with $ears[%ears[$ears ears][1:one $ears ear][0:no ears]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$eyecolour[%eyecolour[$eyecolourbasic-eyed][1:one-eyed][0:eyeless] ][]$person] with $nose[%nose[&a_an[$nose]][0:no nose]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$eyeshape[%eyeshape[$eyeshape-eyed][1:one-eyed][0:eyeless] ][]$person] with $nose[%nose[&a_an[$nose]][0:no nose]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$frame[@ ][]$person] with $nose[%nose[&a_an[$nose]][0:no nose]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$distinctivefeaturebasic[@ ][]$person] with $nose[%nose[&a_an[$nose]][0:no nose]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$ears[%ears[$earsbasic][1:one-eared][0:earless] ][]$person] with $nose[%nose[&a_an[$nose]][0:no nose]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$person] with $nose[%nose[&a_an[$nose]][0:no nose]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$haircolourbasic[@-haired ][]$person] with $nose[%nose[&a_an[$nose]][0:no nose]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$hairstylebasic[@ ][]$person] with $nose[%nose[&a_an[$nose]][0:no nose]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern = "&a_an[$facialhairstylebasic[@ ][]$person] with $nose[%nose[&a_an[$nose]][0:no nose]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$skincolour[@-skinned ][]$person] with $nose[%nose[&a_an[$nose]][0:no nose]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[&tattoos $person] with $nose[%nose[&a_an[$nose]][0:no nose]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[&scars $person] with $nose[%nose[&a_an[$nose]][0:no nose]][$]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$eyecolour[%eyecolour[$eyecolourbasic-eyed][1:one-eyed][0:eyeless] ][]$person] with $?hairstyle[$haircolour[$haircolour hair][$]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$eyeshape[%eyeshape[$eyeshape-eyed][1:one-eyed][0:eyeless] ][]$person] with $?hairstyle[$haircolour[$haircolour hair][$]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$frame[@ ][]$person] with $?hairstyle[$haircolour[$haircolour hair][$]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$distinctivefeaturebasic[@ ][]$person] with $?hairstyle[$haircolour[$haircolour hair][$]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$ears[%ears[$earsbasic][1:one-eared][0:earless] ][]$person] with $?hairstyle[$haircolour[$haircolour hair][$]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$nose[%nose[$nosebasic][0:noseless] ][]$person] with $?hairstyle[$haircolour[$haircolour hair][$]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$person] with $?hairstyle[$haircolour[$haircolour hair][$]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$hairstylebasic[@ ][]$person] with $?hairstyle[$haircolour[$haircolour hair][$]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$facialhairstylebasic[@ ][]$person] with $?hairstyle[$haircolour[$haircolour hair][$]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$skincolour[@-skinned ][]$person] with $?hairstyle[$haircolour[$haircolour hair][$]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[&tattoos $person] with $?hairstyle[$haircolour[$haircolour hair][$]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[&scars $person] with $?hairstyle[$haircolour[$haircolour hair][$]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$eyecolour[%eyecolour[$eyecolourbasic-eyed][1:one-eyed][0:eyeless] ][]$person] with $?facialhairstyle[&?a_an[$facialhairstyle]][no beard]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$eyeshape[%eyeshape[$eyeshape-eyed][1:one-eyed][0:eyeless] ][]$person] with $?facialhairstyle[&?a_an[$facialhairstyle]][no beard]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern = "&a_an[$frame[@ ][]$person] with $?facialhairstyle[&?a_an[$facialhairstyle]][no beard]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$distinctivefeaturebasic[@ ][]$person] with $?facialhairstyle[&?a_an[$facialhairstyle]][no beard]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$ears[%ears[$earsbasic][1:one-eared][0:earless] ][]$person] with $?facialhairstyle[&?a_an[$facialhairstyle]][no beard]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$nose[%nose[$nosebasic][0:noseless] ][]$person] with $?facialhairstyle[&?a_an[$facialhairstyle]][no beard]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern = "&a_an[$person] with $?facialhairstyle[&?a_an[$facialhairstyle]][no beard]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$haircolourbasic[@-haired ][]$person] with $?facialhairstyle[&?a_an[$facialhairstyle]][no beard]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern = "&a_an[$hairstylebasic[@ ][]$person] with $?facialhairstyle[&?a_an[$facialhairstyle]][no beard]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$skincolour[@-skinned ][]$person] with $?facialhairstyle[&?a_an[$facialhairstyle]][no beard]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern = "&a_an[&tattoos $person] with $?facialhairstyle[&?a_an[$facialhairstyle]][no beard]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern = "&a_an[&scars $person] with $?facialhairstyle[&?a_an[$facialhairstyle]][no beard]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$eyecolour[%eyecolour[$eyecolourbasic-eyed][1:one-eyed][0:eyeless] ][]$person] with $?hairstyle[&?a_an[$hairstyle]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$eyeshape[%eyeshape[$eyeshape-eyed][1:one-eyed][0:eyeless] ][]$person] with $?hairstyle[&?a_an[$hairstyle]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$frame[@ ][]$person] with $?hairstyle[&?a_an[$hairstyle]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$distinctivefeaturebasic[@ ][]$person] with $?hairstyle[&?a_an[$hairstyle]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$ears[%ears[$earsbasic][1:one-eared][0:earless] ][]$person] with $?hairstyle[&?a_an[$hairstyle]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$nose[%nose[$nosebasic][0:noseless] ][]$person] with $?hairstyle[&?a_an[$hairstyle]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$person] with $?hairstyle[&?a_an[$hairstyle]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$haircolourbasic[@-haired ][]$person] with $?hairstyle[&?a_an[$hairstyle]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern = "&a_an[$facialhairstylebasic[@ ][]$person] with $?hairstyle[&?a_an[$hairstyle]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$skincolour[@-skinned ][]$person] with $?hairstyle[&?a_an[$hairstyle]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[&tattoos $person] with $?hairstyle[&?a_an[$hairstyle]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[&scars $person] with $?hairstyle[&?a_an[$hairstyle]][no hair]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$eyecolour[%eyecolour[$eyecolourbasic-eyed][1:one-eyed][0:eyeless] ][]$person] &withtattoos"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$eyeshape[%eyeshape[$eyeshape-eyed][1:one-eyed][0:eyeless] ][]$person] &withtattoos"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$frame[@ ][]$person] &withtattoos"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$distinctivefeaturebasic[@ ][]$person] &withtattoos"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$ears[%ears[$earsbasic][1:one-eared][0:earless] ][]$person] &withtattoos"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$nose[%nose[$nosebasic][0:noseless] ][]$person] &withtattoos"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
			{ Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100, Pattern = "&a_an[$person] &withtattoos" });
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$haircolourbasic[@-haired ][]$person] &withtattoos"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$hairstylebasic[@ ][]$person] &withtattoos"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern = "&a_an[$facialhairstylebasic[@ ][]$person] &withtattoos"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$skincolour[@-skinned ][]$person] &withtattoos"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[&scars $person] &withtattoos"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$eyecolour[%eyecolour[$eyecolourbasic-eyed][1:one-eyed][0:eyeless] ][]$person] with $skincolour skin"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern =
				"&a_an[$eyeshape[%eyeshape[$eyeshape-eyed][1:one-eyed][0:eyeless] ][]$person] with $skincolour skin"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$frame[@ ][]$person] with $skincolour skin"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$distinctivefeaturebasic[@ ][]$person] with $skincolour skin"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$ears[%ears[$earsbasic][1:one-eared][0:earless] ][]$person] with $skincolour skin"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$nose[%nose[$nosebasic][0:noseless] ][]$person] with $skincolour skin"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$person] with $skincolour skin"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$haircolourbasic[@-haired ][]$person] with $skincolour skin"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$hairstylebasic[@ ][]$person] with $skincolour skin"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern = "&a_an[$facialhairstylebasic[@ ][]$person] with $skincolour skin"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[&tattoos $person] with $skincolour skin"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[&scars $person] with $skincolour skin"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$eyecolour[%eyecolour[$eyecolourbasic-eyed][1:one-eyed][0:eyeless] ][]$person]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$eyeshape[%eyeshape[$eyeshape-eyed][1:one-eyed][0:eyeless] ][]$person]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
			{ Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100, Pattern = "&a_an[$frame[@ ][]$person]" });
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$distinctivefeaturebasic[@ ][]$person]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$ears[%ears[$earsbasic][1:one-eared][0:earless] ][]$person]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$nose[%nose[$nosebasic][0:noseless] ][]$person]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$haircolourbasic[@-haired ][]$person]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$hairstylebasic[@ ][]$person]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern = "&a_an[$facialhairstylebasic[@ ][]$person]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$skincolour[@-skinned ][]$person]"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
			{ Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100, Pattern = "&a_an[&tattoos $person]" });
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$eyecolour[%eyecolour[$eyecolourbasic-eyed][1:one-eyed][0:eyeless] ][]$person] &withscars"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$eyeshape[%eyeshape[$eyeshape-eyed][1:one-eyed][0:eyeless] ][]$person] &withscars"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$frame[@ ][]$person] &withscars"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$distinctivefeaturebasic[@ ][]$person] &withscars"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$ears[%ears[$earsbasic][1:one-eared][0:earless] ][]$person] &withscars"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$nose[%nose[$nosebasic][0:noseless] ][]$person] &withscars"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
			{ Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100, Pattern = "&a_an[$person] &withscars" });
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$haircolourbasic[@-haired ][]$person] &withscars"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$hairstylebasic[@ ][]$person] &withscars"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = nonFemaleProg, RelativeWeight = 100,
			Pattern = "&a_an[$facialhairstylebasic[@ ][]$person] &withscars"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[$skincolour[@-skinned ][]$person] &withscars"
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0, ApplicabilityProg = humanProg, RelativeWeight = 100,
			Pattern = "&a_an[&tattoos $person] &withscars"
		});

		#endregion

		#region FDescs

		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			RelativeWeight = 100,
			ApplicabilityProg = nonFemaleProg,
			Type = 1,
			Pattern =
				"This $person is &a_an[&male] &race that is &height tall and $?height[$height relative to you][about the same height as you]. You would describe &him as $frame, as &he is $framefancy. $eyecolour[%eyecolour[&he has $eyecolourbasic $eyeshape eyes that are $eyecolourfancy][2-3:&he has % $eyecolourbasic $eyeshape eyes that are $eyecolourfancy][1:&he has a single $eyecolourbasic $eyeshape eye that is $eyecolourfancy][0:&he has no eyes, only empty sockets]][You cannot see &his eyes because &he is wearing $], which $eyecolour[%eyecolour[sit][1:sits]][sit] above $nose[%nose[$nosefancy][0:a gaping hole where &his nose used to be]][$]. $hairstyle[$?hairstyle[&his hair is $haircolourfancy, and has been styled so that &he has &?a_an[$hairstylefancy]][&his head is bald, with no hair at all]][You cannot tell what sort of hair style or even hair colour &he has because &he is wearing $]. $?facialhairstyle[&he has &?a_an[$facialhairstyle], which is $facialhaircolourfancy][&he does not have any facial hair, with a clean, smooth chin]. $ears[%ears[&his ears are $earsfancy][1:&he only has one ear, which is $earsfancy][0:&he has no ears, just scars where the ears should be]][You cannot make out &his ears because &he is wearing $]. &he has $distinctivefeaturefancy."
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			RelativeWeight = 100,
			ApplicabilityProg = femaleProg,
			Type = 1,
			Pattern =
				"This $person is &a_an[&male] &race that is &height tall and $?height[$height relative to you][about the same height as you]. You would describe &him as $frame, as &he is $framefancy. $eyecolour[%eyecolour[&he has $eyecolourbasic $eyeshape eyes that are $eyecolourfancy][2-3:&he has % $eyecolourbasic $eyeshape eyes that are $eyecolourfancy][1:&he has a single $eyecolourbasic $eyeshape eye that is $eyecolourfancy][0:&he has no eyes, only empty sockets]][You cannot see &his eyes because &he is wearing $], which $eyecolour[%eyecolour[sit][1:sits]][sit] above $nose[%nose[$nosefancy][0:a gaping hole where &his nose used to be]][$]. $hairstyle[$?hairstyle[&his hair is $haircolourfancy, and has been styled so that &he has &?a_an[$hairstylefancy]][&his head is bald, with no hair at all]][You cannot tell what sort of hair style or even hair colour &he has because &he is wearing $]. $ears[%ears[&his ears are $earsfancy][1:&he only has one ear, which is $earsfancy][0:&he has no ears, just scars where the ears should be]][You cannot make out &his ears because &he is wearing $]. &he has $distinctivefeaturefancy."
		});

		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			RelativeWeight = 100,
			ApplicabilityProg = nonFemaleProg,
			Type = 1,
			Pattern =
				"This $person is &a_an[&male] &race that is &height tall and $?height[$height relative to you][about the same height as you]. You would describe &him as $frame, as &he is $framefancy. $hairstyle[$?hairstyle[&his hair is $haircolourfancy, and has been styled so that &he has &?a_an[$hairstylefancy]][&his head is bald, with no hair at all]][You cannot tell what sort of hair style or even hair colour &he has because &he is wearing $]. $?facialhairstyle[&he has &?a_an[$facialhairstyle], which is $facialhaircolourfancy][&he does not have any facial hair, with a clean, smooth chin]. $eyecolour[%eyecolour[&he has $eyecolourbasic $eyeshape eyes that are $eyecolourfancy][2-3:&he has % $eyecolourbasic $eyeshape eyes that are $eyecolourfancy][1:&he has a single $eyecolourbasic $eyeshape eye that is $eyecolourfancy][0:&he has no eyes, only empty sockets]][You cannot see &his eyes because &he is wearing $] and $nose[%nose[$nosefancy][0:a gaping hole where &his nose used to be]][$]. $ears[%ears[&his ears are $earsfancy][1:&he only has one ear, which is $earsfancy][0:&he has no ears, just scars where the ears should be]][You cannot make out &his ears because &he is wearing $]. &he has $distinctivefeaturefancy."
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			RelativeWeight = 100,
			ApplicabilityProg = femaleProg,
			Type = 1,
			Pattern =
				"This $person is &a_an[&male] &race that is &height tall and $?height[$height relative to you][about the same height as you]. You would describe &him as $frame, as &he is $framefancy. $hairstyle[$?hairstyle[&his hair is $haircolourfancy, and has been styled so that &he has &?a_an[$hairstylefancy]][&his head is bald, with no hair at all]][You cannot tell what sort of hair style or even hair colour &he has because &he is wearing $]. $eyecolour[%eyecolour[&he has $eyecolourbasic $eyeshape eyes that are $eyecolourfancy][2-3:&he has % $eyecolourbasic $eyeshape eyes that are $eyecolourfancy][1:&he has a single $eyecolourbasic $eyeshape eye that is $eyecolourfancy][0:&he has no eyes, only empty sockets]][You cannot see &his eyes because &he is wearing $] and $nose[%nose[$nosefancy][0:a gaping hole where &his nose used to be]][$]. $ears[%ears[&his ears are $earsfancy][1:&he only has one ear, which is $earsfancy][0:&he has no ears, just scars where the ears should be]][You cannot make out &his ears because &he is wearing $]. &he has $distinctivefeaturefancy."
		});

		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			RelativeWeight = 100,
			ApplicabilityProg = nonFemaleProg,
			Type = 1,
			Pattern =
				"This $person is &a_an[&male] &race that is &height tall and $?height[$height relative to you][about the same height as you]. &he has $distinctivefeaturefancy and is $framefancy. $hairstyle[$?hairstyle[&his hair is $haircolourfancy, and has been styled so that &he has &?a_an[$hairstylefancy]][&his head is bald, with no hair at all]][You cannot tell what sort of hair style or even hair colour &he has because &he is wearing $]. $?facialhairstyle[&he has &?a_an[$facialhairstyle], which is $facialhaircolourfancy][&he does not have any facial hair, with a clean, smooth chin]. $eyecolour[%eyecolour[&he has $eyecolourbasic $eyeshape eyes that are $eyecolourfancy][2-3:&he has % $eyecolourbasic $eyeshape eyes that are $eyecolourfancy][1:&he has a single $eyecolourbasic $eyeshape eye that is $eyecolourfancy][0:&he has no eyes, only empty sockets]][You cannot see &his eyes because &he is wearing $] and $nose[%nose[$nosefancy][0:a gaping hole where &his nose used to be]][$]. $ears[%ears[&his ears are $earsfancy][1:&he only has one ear, which is $earsfancy][0:&he has no ears, just scars where the ears should be]][You cannot make out &his ears because &he is wearing $]."
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			RelativeWeight = 100,
			ApplicabilityProg = femaleProg,
			Type = 1,
			Pattern =
				"This $person is &a_an[&male] &race that is &height tall and $?height[$height relative to you][about the same height as you]. &he has $distinctivefeaturefancy and is $framefancy. $hairstyle[$?hairstyle[&his hair is $haircolourfancy, and has been styled so that &he has &?a_an[$hairstylefancy]][&his head is bald, with no hair at all]][You cannot tell what sort of hair style or even hair colour &he has because &he is wearing $]. $eyecolour[%eyecolour[&he has $eyecolourbasic $eyeshape eyes that are $eyecolourfancy][2-3:&he has % $eyecolourbasic $eyeshape eyes that are $eyecolourfancy][1:&he has a single $eyecolourbasic $eyeshape eye that is $eyecolourfancy][0:&he has no eyes, only empty sockets]][You cannot see &his eyes because &he is wearing $] and $nose[%nose[$nosefancy][0:a gaping hole where &his nose used to be]][$]. $ears[%ears[&his ears are $earsfancy][1:&he only has one ear, which is $earsfancy][0:&he has no ears, just scars where the ears should be]][You cannot make out &his ears because &he is wearing $]."
		});

		#endregion

		_context.SaveChanges();
	}
}