using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Celestial;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Law;
using MudSharp.TimeAndDate;

namespace DatabaseSeeder.Seeders;

public class LawSeeder : IDatabaseSeeder
{
	public FuturemudDatabaseContext Context { get; private set; }
	public IReadOnlyDictionary<string, string> QuestionAnswers { get; private set; }
	public string AuthorityName { get; private set; }
	public LegalAuthority Authority { get; private set; }
	public IReadOnlyDictionary<string, LegalClass> Classes { get; private set; }
	public Dictionary<string, FutureProg> ProgLookup { get; } = new();

	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
		new List<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool>
			Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
		{
			("name", @"What name do you want to give to this enforcement authority?: ", (context, answers) => true,
				(answer, context) =>
				{
					if (string.IsNullOrWhiteSpace(answer))
						return (false, "You must give a name to the enforcement authority.");

					return (true, string.Empty);
				}),
			("currency",
				@"Which currency do you want to use for the payment of fines? Please specify the name or ID of the currency: 

Note: The ID is most likely to be 1 if you have only installed 1 currency. ", (context, answers) => true,
				(answer, context) =>
				{
					if (long.TryParse(answer, out var id))
					{
						if (context.Currencies.Any(x => x.Id == id)) return (true, string.Empty);

						return (false, $"There is no currency with an id of {id}.");
					}

					if (context.Currencies.Any(x =>
						    x.Name.StartsWith(answer, StringComparison.InvariantCultureIgnoreCase)))
						return (true, string.Empty);

					return (false, $"There is no currency called '{answer}'.");
				}),
			("createai", @"Would you like to create a set of generic AI for enforcers to go with this legal authority?

Please answer #3yes#F or #3no#F: ", (context, answers) => true,
				(answer, context) =>
				{
					if (!answer.EqualToAny("yes", "y", "no", "n")) return (false, "Please choose yes or no.");

					return (true, string.Empty);
				}),
			("separatepowers",
				@"Would you like to separate the power to accuse someone of a crime and arrest them from the power to convinct or pardon a person of a crime?

For example, if you select yes, there will be separate 'police' and 'judge' enforcement roles created. Otherwise, they will be the same.

Please answer #3yes#F or #3no#F: ", (context, answers) => true,
				(answer, context) =>
				{
					if (!answer.EqualToAny("yes", "y", "no", "n")) return (false, "Please choose yes or no.");

					return (true, string.Empty);
				}),
			("punishmentlevel",
				@"The suite of possible crimes will be set up by this seeder with a number of default punishments, which can include good behaviour bonds, fines, prison time, mutilation (blinding, cutting off hands, etc) and execution.

You can change all of these values yourself later but the starting values will be determined by your choice.

You can choose from the following ""themes"" of punishment styles, as per below:

#BTiered#F - mostly fines for crimes against lower classes, capital punishment for lower classes #9[Not Yet Supported]#F
#BWeregild#F - all but the most serious punishments come with a fine
#BWestern#F - fines for misdemeanours, prison time for serious crimes, capital punishment for the worst
#BLiberal#F - fines for misdemeanours, prison time for serious crimes, no capital punishment
#BTheocracy#F - similar to the western option, but with capital punishment for religious crimes

What is your choice? ", (context, answers) => true,
				(answer, context) =>
				{
					if (!answer.EqualToAny("tiered", "weregild", "western", "liberal", "theocracy"))
						return (false, $"The option '{answer.ToLowerInvariant()}' is not a valid selection.");

					return (true, string.Empty);
				}),
			("classes",
				@"The legal system is driven by the concept of legal classes. All beings must be placed into exactly one legal class, and this legal class determines what crimes someone can commit or be a victim of.

Each of the following legal classes can be created by this seeder:

#BImmune#F - none of the crimes will be able to be commited by this class
#BSovereign#F - only a very limited set of crimes and very restricted enforcement
#BNoble#F - different crimes against lower classes, restricted enforcement
#BOfficer#F - a military officer, with military-specific crimes
#BSoldier#F - a non-officer soldier, with military-specific crimes
#BEnforcer#F - a separate legal class for enforcers, different penalties for crimes against
#BCitizen#F - a standard legal class for regular folk
#BNon-Citizen#F - a lower legal class than citizen
#BSlave#F - someone with very few rights and punishments for others are about fines
#BPet#F - animals with enhanced rights, like domestic pets and higher order animals
#BFelon#F - a felon is someone who has previously been convicted of a serious crime
#BCriminal#F - criminals are people who are currently serving a custodial sentence or held in remand
#BOther#F - everything else - robots, wildlife, aliens...

#3Note: You can rename and customise these. They are just examples, try to pick the ones that are closest to what you want#0

Please enter the classes that you want to be created, separated by spaces. The #BOther#F option is always required and so is included by default.

What is your choice? ", (context, answers) => true,
				(text, context) =>
				{
					var split = text.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (var item in split)
						switch (item.ToLowerInvariant())
						{
							case "immune":
							case "sovereign":
							case "noble":
							case "enforcer":
							case "citizen":
							case "non-citizen":
							case "noncitizen":
							case "slave":
							case "criminal":
							case "felon":
							case "pet":
							case "soldier":
							case "officer":
								continue;
							case "other":
								return (false,
									"The option 'other' is included implicitly, you can not explicitly include it.");
							default:
								return (false,
									$"The option '{item.ToLowerInvariant()}' is not a valid legal class selection.");
						}

					return (true, string.Empty);
				}
			),
			("religiouslaws", @"Would you like to include religious laws, for example apostacy, blasphemy, etc?

Please answer #3yes#F or #3no#F: ", (context, answers) => true,
				(answer, context) =>
				{
					if (!answer.EqualToAny("yes", "y", "no", "n")) return (false, "Please choose yes or no.");

					return (true, string.Empty);
				}),
			("penaltyunits",
				@"All punishments with fines will have an initial value that is determined by a value you specify called penalty units.

You must enter a numerical value, which is tied to the currency you chose. Assuming you used a currency from the seeder, here are some examples below to help you decide on a value:

#BDollars#F: 1 = 1 cent, 100 = 1 dollar
#BPounds#F: 1 = 1 farthing, 4 = 1 penny, 48 = 1 shilling, 960 = 1 pound
#BFantasy#F: 1 = 1 brass, 10 = 1 copper, 100 = 1 silver, 1000 = 1 gold, 10000 = 1 platinum
#BRoman#F: 1 = 1 uncia, 12 = 1 asarius, 48 = 1 sestertius, 192 = 1 denarius, 4800 = 1 aureus
#BBits#F: The number you enter corresponds 1:1 with the bits
#BGondor#F: 1 = 1 farthing, 4 = 1 penny, 400 = 1 crown

The number that you enter will be multiplied by various amounts between 1 and 100 to arrive at the final fine. 

As an example, the fine for trespassing is going to be #51 * units#0 whereas manslaughter might be #510 * units#0 and treason could run as high as #550 * units#0.

Please enter your penalty unit: ", (context, answers) => true,
				(answer, context) =>
				{
					if (!uint.TryParse(answer, out _)) return (false, "Please enter a positive integer.");

					return (true, string.Empty);
				})
		};

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.Accounts.Any() || !context.Currencies.Any()) return ShouldSeedResult.PrerequisitesNotMet;

		if (context.LegalAuthorities.Any()) return ShouldSeedResult.MayAlreadyBeInstalled;

		return ShouldSeedResult.ReadyToInstall;
	}

	public int SortOrder => 25;
	public string Name => "Law Seeder";
	public string Tagline => "Sets up Legal Enforcement, Laws, and some related AI.";

	public string FullDescription =>
		"This seeder will set up a legal authority with some laws, legal classes, enforcer setups and AI. It is strongly recommended that you use this seeder to set up a new enforcement zone over doing it manually.";

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		Context = context;
		QuestionAnswers = questionAnswers;
		AuthorityName = questionAnswers["name"];
		context.Database.BeginTransaction();
		Currency currency;
		if (long.TryParse(questionAnswers["currency"], out var id))
			currency = context.Currencies.Find(id);
		else
			currency = context.Currencies.First(x => x.Name == questionAnswers["currency"]);

		var authority = new LegalAuthority
		{
			Name = AuthorityName,
			CurrencyId = currency.Id,
			AutomaticallyConvict = false,
			AutomaticConvictionTime = 60 * 60 * 24,
			PlayersKnowTheirCrimes = true
		};
		context.LegalAuthorities.Add(authority);
		context.SaveChanges();
		Authority = authority;

		#region Progs

		var onreleaseprog = new FutureProg
		{
			FunctionName = $"OnRelease{AuthorityName.CollapseString()}",
			Category = "Law",
			Subcategory = AuthorityName.CollapseString(),
			FunctionComment = "Executed when a character is released from prison or custody",
			ReturnType = 0,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText = ""
		};

		onreleaseprog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = onreleaseprog,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = 8200
		});
		context.FutureProgs.Add(onreleaseprog);
		authority.OnReleaseProg = onreleaseprog;
		ProgLookup["onrelease"] = onreleaseprog;

		var onholdprog = new FutureProg
		{
			FunctionName = $"OnIncarcerate{AuthorityName.CollapseString()}",
			Category = "Law",
			Subcategory = AuthorityName.CollapseString(),
			FunctionComment = "Executed when a character is incarcerated (held in custody)",
			ReturnType = 0,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText = ""
		};

		onholdprog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = onholdprog,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = 8200
		});
		context.FutureProgs.Add(onholdprog);
		authority.OnHoldProg = onholdprog;
		ProgLookup["onhold"] = onholdprog;

		var onimprisonprog = new FutureProg
		{
			FunctionName = $"OnImprison{AuthorityName.CollapseString()}",
			Category = "Law",
			Subcategory = AuthorityName.CollapseString(),
			FunctionComment = "Executed when a character is sentenced to a prison term",
			ReturnType = 0,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText = ""
		};

		onimprisonprog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = onimprisonprog,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = 8200
		});
		context.FutureProgs.Add(onimprisonprog);
		authority.OnImprisonProg = onimprisonprog;
		ProgLookup["onimprison"] = onimprisonprog;

		var bailprog = new FutureProg
		{
			FunctionName = $"BailCalculation{AuthorityName.CollapseString()}",
			Category = "Law",
			Subcategory = AuthorityName.CollapseString(),
			FunctionComment = "Determines the bail payment for a particular crime",
			ReturnType = 2,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText = @"// This is an example of how you could split up your bail amounts
if (@crime.ismajorcrime)
  return 0
end if
if (@crime.isviolentcrime)
  return 0
end if
if (@crime.ismoralcrime)
  return 0
end if
return 0"
		};

		bailprog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = bailprog,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = 8200
		});
		bailprog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = bailprog,
			ParameterIndex = 1,
			ParameterName = "crime",
			ParameterType = (long)FutureProgVariableTypes.Crime
		});
		context.FutureProgs.Add(bailprog);
		authority.BailCalculationProg = bailprog;
		context.SaveChanges();
		ProgLookup["bail"] = bailprog;

		var isongoodbehaviourprog = new FutureProg
		{
			FunctionName = $"IsOnGoodBehaviour{AuthorityName.CollapseString()}",
			Category = "Law",
			Subcategory = AuthorityName.CollapseString(),
			FunctionComment = "True if a character has a good behaviour bond",
			ReturnType = (long)FutureProgVariableTypes.Boolean,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText = $"return OnGoodBehaviourBond(@ch, ToLegalAuthority({authority.Id}))"
		};

		isongoodbehaviourprog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isongoodbehaviourprog,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		context.FutureProgs.Add(isongoodbehaviourprog);
		ProgLookup["isgood"] = isongoodbehaviourprog;

		#endregion

		#region Legal Classes

		var classNames = questionAnswers["classes"].Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
			.Select(x => x.ToLowerInvariant()).ToList();
		classNames.Add("other");
		var classes = new Dictionary<string, LegalClass>(StringComparer.OrdinalIgnoreCase);
		SetupClasses(classNames);

		#endregion

		#region Enforcement Classes

		SetupEnforcers();

		#endregion

		#region Witness Profiles

		SetupWitnesses();

		#endregion

		#region Laws

		SetupLaws();

		#endregion

		#region Artificial Intelligence

		SetupAI();

		#endregion

		context.SaveChanges();
		context.Database.CommitTransaction();

		return "Successfully set up a legal authority.";
	}

	private void AddWitnessProfile(string name, double reliability, double baseReportChance,
		IEnumerable<TimeOfDay> activeTimes, IEnumerable<string> ignoreOffenders, IEnumerable<string> ignoreVictims)
	{
		var profile = new WitnessProfile
		{
			Name = name,
			MinimumSkillToDetermineBiases = 30,
			MinimumSkillToDetermineTimeOfDay = 30,
			ReportingReliability = reliability,
			BaseReportingChanceMorning = baseReportChance * (activeTimes.Contains(TimeOfDay.Morning) ? 1 : 0.33),
			BaseReportingChanceAfternoon = baseReportChance * (activeTimes.Contains(TimeOfDay.Morning) ? 1 : 0.33),
			BaseReportingChanceDawn = baseReportChance * (activeTimes.Contains(TimeOfDay.Morning) ? 1 : 0.33),
			BaseReportingChanceDusk = baseReportChance * (activeTimes.Contains(TimeOfDay.Morning) ? 1 : 0.33),
			BaseReportingChanceNight = baseReportChance * (activeTimes.Contains(TimeOfDay.Morning) ? 1 : 0.33)
		};
		Context.WitnessProfiles.Add(profile);

		profile.WitnessProfilesCooperatingAuthorities.Add(new WitnessProfilesCooperatingAuthorities
			{ LegalAuthority = Authority, WitnessProfile = profile });
		foreach (var item in ignoreOffenders)
			profile.WitnessProfilesIgnoredCriminalClasses.Add(new WitnessProfilesIgnoredCriminalClasses
			{
				LegalClass = Classes[item],
				WitnessProfile = profile
			});
		foreach (var item in ignoreVictims)
			profile.WitnessProfilesIgnoredVictimClasses.Add(new WitnessProfilesIgnoredVictimClasses
			{
				LegalClass = Classes[item],
				WitnessProfile = profile
			});

		var knownProg = new FutureProg
		{
			FunctionName = $"{name.CollapseString()}KnowsIdentity",
			ReturnType = (long)FutureProgVariableTypes.Boolean,
			Category = "Law",
			Subcategory = AuthorityName.CollapseString(),
			FunctionComment =
				"Determines if VNPC witnesses know the identity of a criminal or only their characteristics",
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText =
				@"// You could consider checking a character's ethnicity, culture, merits, clan affiliations, skills, reputation (could be stored as a register variable) etc
return true"
		};
		Context.FutureProgs.Add(knownProg);
		knownProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = knownProg, ParameterIndex = 0, ParameterName = "criminal",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		profile.IdentityKnownProg = knownProg;

		var multiplierProg = new FutureProg
		{
			FunctionName = $"{name.CollapseString()}ReportMultiplier",
			ReturnType = (long)FutureProgVariableTypes.Number,
			Category = "Law",
			Subcategory = AuthorityName.CollapseString(),
			FunctionComment = "A multiplier to the base reporting chance for an individual",
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText =
				@"// You could consider checking a character's ethnicity, culture, merits, clan affiliations, skills, reputation (could be stored as a register variable) etc
return 1.0"
		};
		Context.FutureProgs.Add(multiplierProg);
		multiplierProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = multiplierProg, ParameterIndex = 0, ParameterName = "criminal",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		multiplierProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = multiplierProg, ParameterIndex = 1, ParameterName = "victim",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		multiplierProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = multiplierProg, ParameterIndex = 2, ParameterName = "crime",
			ParameterType = (long)FutureProgVariableTypes.Crime
		});
		profile.ReportingMultiplierProg = multiplierProg;
	}

	private void SetupWitnesses()
	{
		AddWitnessProfile("Always Report Everything", 1.0, 1.0,
			new List<TimeOfDay>
				{ TimeOfDay.Morning, TimeOfDay.Dawn, TimeOfDay.Afternoon, TimeOfDay.Dusk, TimeOfDay.Night },
			Enumerable.Empty<string>(), Enumerable.Empty<string>());

		AddWitnessProfile("Compliant Daytime Hours", 1.0, 0.75,
			new List<TimeOfDay> { TimeOfDay.Morning, TimeOfDay.Afternoon }, Enumerable.Empty<string>(),
			Enumerable.Empty<string>());
		AddWitnessProfile("Compliant Non-Night Hours", 1.0, 0.75,
			new List<TimeOfDay> { TimeOfDay.Morning, TimeOfDay.Afternoon, TimeOfDay.Dawn, TimeOfDay.Dusk },
			Enumerable.Empty<string>(), Enumerable.Empty<string>());
		AddWitnessProfile("Compliant Night Hours", 1.0, 0.75, new List<TimeOfDay> { TimeOfDay.Night },
			Enumerable.Empty<string>(), Enumerable.Empty<string>());
		AddWitnessProfile("Compliant Non-Day Hours", 1.0, 0.75,
			new List<TimeOfDay> { TimeOfDay.Night, TimeOfDay.Dawn, TimeOfDay.Dusk }, Enumerable.Empty<string>(),
			Enumerable.Empty<string>());
		AddWitnessProfile("Compliant All Hours", 1.0, 0.75,
			new List<TimeOfDay>
				{ TimeOfDay.Morning, TimeOfDay.Afternoon, TimeOfDay.Dawn, TimeOfDay.Dusk, TimeOfDay.Night },
			Enumerable.Empty<string>(), Enumerable.Empty<string>());

		AddWitnessProfile("Hesitant Daytime Hours", 1.0, 0.38,
			new List<TimeOfDay> { TimeOfDay.Morning, TimeOfDay.Afternoon }, Enumerable.Empty<string>(),
			Enumerable.Empty<string>());
		AddWitnessProfile("Hesitant Non-Night Hours", 1.0, 0.38,
			new List<TimeOfDay> { TimeOfDay.Morning, TimeOfDay.Afternoon, TimeOfDay.Dawn, TimeOfDay.Dusk },
			Enumerable.Empty<string>(), Enumerable.Empty<string>());
		AddWitnessProfile("Hesitant Night Hours", 1.0, 0.38, new List<TimeOfDay> { TimeOfDay.Night },
			Enumerable.Empty<string>(), Enumerable.Empty<string>());
		AddWitnessProfile("Hesitant Non-Day Hours", 1.0, 0.38,
			new List<TimeOfDay> { TimeOfDay.Night, TimeOfDay.Dawn, TimeOfDay.Dusk }, Enumerable.Empty<string>(),
			Enumerable.Empty<string>());
		AddWitnessProfile("Hesitant All Hours", 1.0, 0.75,
			new List<TimeOfDay>
				{ TimeOfDay.Morning, TimeOfDay.Afternoon, TimeOfDay.Dawn, TimeOfDay.Dusk, TimeOfDay.Night },
			Enumerable.Empty<string>(), Enumerable.Empty<string>());

		if (Classes.ContainsKey("noble"))
		{
			AddWitnessProfile("Noble Friendly Daytime Hours", 1.0, 0.75,
				new List<TimeOfDay> { TimeOfDay.Morning, TimeOfDay.Afternoon }, new[] { "noble" },
				Enumerable.Empty<string>());
			AddWitnessProfile("Noble Friendly Non-Night Hours", 1.0, 0.75,
				new List<TimeOfDay> { TimeOfDay.Morning, TimeOfDay.Afternoon, TimeOfDay.Dawn, TimeOfDay.Dusk },
				new[] { "noble" }, Enumerable.Empty<string>());
			AddWitnessProfile("Noble Friendly Night Hours", 1.0, 0.75, new List<TimeOfDay> { TimeOfDay.Night },
				new[] { "noble" }, Enumerable.Empty<string>());
			AddWitnessProfile("Noble Friendly Non-Day Hours", 1.0, 0.75,
				new List<TimeOfDay> { TimeOfDay.Night, TimeOfDay.Dawn, TimeOfDay.Dusk }, new[] { "noble" },
				Enumerable.Empty<string>());
			AddWitnessProfile("Noble Friendly All Hours", 1.0, 0.75,
				new List<TimeOfDay>
					{ TimeOfDay.Morning, TimeOfDay.Afternoon, TimeOfDay.Dawn, TimeOfDay.Dusk, TimeOfDay.Night },
				new[] { "noble" }, Enumerable.Empty<string>());
		}

		if (Classes.ContainsKey("enforcer"))
		{
			AddWitnessProfile("Enforcer Friendly Daytime Hours", 1.0, 0.75,
				new List<TimeOfDay> { TimeOfDay.Morning, TimeOfDay.Afternoon }, new[] { "enforcer" },
				Enumerable.Empty<string>());
			AddWitnessProfile("Enforcer Friendly Non-Night Hours", 1.0, 0.75,
				new List<TimeOfDay> { TimeOfDay.Morning, TimeOfDay.Afternoon, TimeOfDay.Dawn, TimeOfDay.Dusk },
				new[] { "enforcer" }, Enumerable.Empty<string>());
			AddWitnessProfile("Enforcer Friendly Night Hours", 1.0, 0.75, new List<TimeOfDay> { TimeOfDay.Night },
				new[] { "enforcer" }, Enumerable.Empty<string>());
			AddWitnessProfile("Enforcer Friendly Non-Day Hours", 1.0, 0.75,
				new List<TimeOfDay> { TimeOfDay.Night, TimeOfDay.Dawn, TimeOfDay.Dusk }, new[] { "enforcer" },
				Enumerable.Empty<string>());
			AddWitnessProfile("Enforcer Friendly All Hours", 1.0, 0.75,
				new List<TimeOfDay>
					{ TimeOfDay.Morning, TimeOfDay.Afternoon, TimeOfDay.Dawn, TimeOfDay.Dusk, TimeOfDay.Night },
				new[] { "enforcer" }, Enumerable.Empty<string>());
		}

		if (Classes.ContainsKey("slave"))
		{
			AddWitnessProfile("Ignore Slave Victims Daytime Hours", 1.0, 0.75,
				new List<TimeOfDay> { TimeOfDay.Morning, TimeOfDay.Afternoon }, Enumerable.Empty<string>(),
				new[] { "slave" });
			AddWitnessProfile("Ignore Slave Victims Non-Night Hours", 1.0, 0.75,
				new List<TimeOfDay> { TimeOfDay.Morning, TimeOfDay.Afternoon, TimeOfDay.Dawn, TimeOfDay.Dusk },
				Enumerable.Empty<string>(), new[] { "slave" });
			AddWitnessProfile("Ignore Slave Victims Night Hours", 1.0, 0.75, new List<TimeOfDay> { TimeOfDay.Night },
				Enumerable.Empty<string>(), new[] { "slave" });
			AddWitnessProfile("Ignore Slave Victims Non-Day Hours", 1.0, 0.75,
				new List<TimeOfDay> { TimeOfDay.Night, TimeOfDay.Dawn, TimeOfDay.Dusk }, Enumerable.Empty<string>(),
				new[] { "slave" });
			AddWitnessProfile("Ignore Slave Victims All Hours", 1.0, 0.75,
				new List<TimeOfDay>
					{ TimeOfDay.Morning, TimeOfDay.Afternoon, TimeOfDay.Dawn, TimeOfDay.Dusk, TimeOfDay.Night },
				new[] { "slave" }, Enumerable.Empty<string>());
		}

		Context.SaveChanges();
	}

	private void SetupAI()
	{
		var identifyProg = new FutureProg
		{
			FunctionName = $"IsIdentityKnown{AuthorityName.CollapseString()}",
			Category = "AI",
			Subcategory = "Law",
			FunctionComment = "Determines whether an enforcer can tell a character's identity",
			ReturnType = (long)FutureProgVariableTypes.Boolean,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText =
				@"// You might consider things like how prominent a character is, whether they are disguised, merits/flaws etc
return true"
		};
		identifyProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = identifyProg,
			ParameterIndex = 0,
			ParameterName = "enforcer",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		identifyProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = identifyProg,
			ParameterIndex = 1,
			ParameterName = "criminal",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		Context.FutureProgs.Add(identifyProg);

		var warnEchoProg = new FutureProg
		{
			FunctionName = $"WarnEcho{AuthorityName.CollapseString()}",
			Category = "AI",
			Subcategory = "Law",
			FunctionComment = "A prog that is executed when the enforcer needs to warn someone to surrender",
			ReturnType = (long)FutureProgVariableTypes.Text,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText =
				@"return ""yellat "" + BestKeyword(@enforcer, @criminal) + "" You are under arrest for the crime of "" + @crime.Name + ""! Your compliance is required. Do not resist."""
		};

		warnEchoProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = warnEchoProg,
			ParameterIndex = 0,
			ParameterName = "enforcer",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		warnEchoProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = warnEchoProg,
			ParameterIndex = 1,
			ParameterName = "criminal",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		warnEchoProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = warnEchoProg,
			ParameterIndex = 2,
			ParameterName = "crime",
			ParameterType = (long)FutureProgVariableTypes.Crime
		});
		Context.FutureProgs.Add(warnEchoProg);

		var warnMoveProg = new FutureProg
		{
			FunctionName = $"WarnMoveEcho{AuthorityName.CollapseString()}",
			Category = "AI",
			Subcategory = "Law",
			FunctionComment =
				"A prog that is executed when the enforcer needs to warn someone not to move away from the area",
			ReturnType = (long)FutureProgVariableTypes.Text,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText =
				@"return ""yellat "" + BestKeyword(@enforcer, @criminal) + "" You must immediately stop and surrender, or you will be resisting arrest!"""
		};

		warnMoveProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = warnMoveProg,
			ParameterIndex = 0,
			ParameterName = "enforcer",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		warnMoveProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = warnMoveProg,
			ParameterIndex = 1,
			ParameterName = "criminal",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		warnMoveProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = warnMoveProg,
			ParameterIndex = 2,
			ParameterName = "crime",
			ParameterType = (long)FutureProgVariableTypes.Crime
		});
		Context.FutureProgs.Add(warnMoveProg);

		var failToCompyProg = new FutureProg
		{
			FunctionName = $"FailComplyEcho{AuthorityName.CollapseString()}",
			Category = "AI",
			Subcategory = "Law",
			FunctionComment = "A prog that is executed when someone fails to comply with an enforcer",
			ReturnType = (long)FutureProgVariableTypes.Text,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText =
				@"return ""tell "" + BestKeyword(@enforcer, @criminal) + "" Stop resisting arrest! You will be apprehended by force!"""
		};

		failToCompyProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = failToCompyProg,
			ParameterIndex = 0,
			ParameterName = "enforcer",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		failToCompyProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = failToCompyProg,
			ParameterIndex = 1,
			ParameterName = "criminal",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		failToCompyProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = failToCompyProg,
			ParameterIndex = 2,
			ParameterName = "crime",
			ParameterType = (long)FutureProgVariableTypes.Crime
		});
		Context.FutureProgs.Add(failToCompyProg);

		var throwInCellProg = new FutureProg
		{
			FunctionName = $"ThrowInCellEcho{AuthorityName.CollapseString()}",
			Category = "AI",
			Subcategory = "Law",
			FunctionComment = "A prog that is executed when the enforcer throws someone in a cell",
			ReturnType = (long)FutureProgVariableTypes.Text,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText =
				@"return ""tell "" + BestKeyword(@enforcer, @criminal) + "" You are being held in remand until a judge hears your case.\nemote opens up a cell door and throws ~"" + BestKeyword(@enforcer, @criminal) + "" into a cell."""
		};

		throwInCellProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = throwInCellProg,
			ParameterIndex = 0,
			ParameterName = "enforcer",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		throwInCellProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = throwInCellProg,
			ParameterIndex = 1,
			ParameterName = "criminal",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		throwInCellProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = throwInCellProg,
			ParameterIndex = 2,
			ParameterName = "crime",
			ParameterType = (long)FutureProgVariableTypes.Crime
		});
		Context.FutureProgs.Add(throwInCellProg);

		Context.SaveChanges();

		Context.ArtificialIntelligences.Add(new ArtificialIntelligence
		{
			Name = $"Enforcer{AuthorityName.CollapseString()}",
			Type = "Enforcer",
			Definition = @$"<AI>
   <IdentityProg>{identifyProg.Id}</IdentityProg>
   <WarnEchoProg>{warnEchoProg.Id}</WarnEchoProg>
   <WarnStartMoveEchoProg>{warnMoveProg.Id}</WarnStartMoveEchoProg>
   <FailToComplyEchoProg>{failToCompyProg.Id}</FailToComplyEchoProg>
   <ThrowInPrisonEchoProg>{throwInCellProg.Id}</ThrowInPrisonEchoProg>
 </AI>"
		});
		Context.SaveChanges();
	}

	private void SetupEnforcers()
	{
		var separatepowers = QuestionAnswers["separatepowers"].EqualToAny("y", "yes");
		var enforcerProg = new FutureProg
		{
			FunctionName = $"IsEnforcer{AuthorityName.CollapseString()}",
			Category = "Law",
			Subcategory = AuthorityName.CollapseString(),
			FunctionComment = "Determines if a character is an enforcer",
			ReturnType = 4,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText = "return false"
		};
		enforcerProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = enforcerProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Character
		});
		Context.FutureProgs.Add(enforcerProg);

		var enforcer = new EnforcementAuthority
		{
			Name = "Enforcer",
			CanAccuse = true,
			CanForgive = true,
			CanConvict = !separatepowers,
			LegalAuthority = Authority,
			FilterProg = enforcerProg,
			Priority = 0
		};
		Context.EnforcementAuthorities.Add(enforcer);
		foreach (var legalClass in Classes)
			switch (legalClass.Key)
			{
				case "immune":
				case "sovereign":
				case "noble":
					continue;
				case "enforcer":
				case "citizen":
				case "non-citizen":
				case "noncitizen":
				case "slave":
				case "criminal":
				case "felon":
				case "pet":
					enforcer.EnforcementAuthoritiesArrestableLegalClasses.Add(
						new EnforcementAuthoritiesArrestableLegalClasses
						{
							EnforcementAuthority = enforcer,
							LegalClass = legalClass.Value
						});
					enforcer.EnforcementAuthoritiesAccusableClasses.Add(new EnforcementAuthoritiesAccusableClasses
					{
						EnforcementAuthority = enforcer,
						LegalClass = legalClass.Value
					});
					break;
			}

		if (separatepowers)
		{
			enforcerProg = new FutureProg
			{
				FunctionName = $"IsJudge{AuthorityName.CollapseString()}",
				Category = "Law",
				Subcategory = AuthorityName.CollapseString(),
				FunctionComment = "Determines if a character is a judge",
				ReturnType = 4,
				AcceptsAnyParameters = false,
				Public = false,
				StaticType = 0,
				FunctionText = "return false"
			};
			enforcerProg.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = enforcerProg,
				ParameterIndex = 0,
				ParameterName = "ch",
				ParameterType = (long)FutureProgVariableTypes.Character
			});
			Context.FutureProgs.Add(enforcerProg);

			enforcer = new EnforcementAuthority
			{
				Name = "Judge",
				CanAccuse = false,
				CanForgive = false,
				CanConvict = true,
				LegalAuthority = Authority,
				FilterProg = enforcerProg,
				Priority = 1
			};
			Context.EnforcementAuthorities.Add(enforcer);
			foreach (var legalClass in Classes)
				switch (legalClass.Key)
				{
					case "immune":
					case "sovereign":
					case "noble":
						continue;
					case "enforcer":
					case "soldier":
					case "officer":
					case "citizen":
					case "non-citizen":
					case "noncitizen":
					case "slave":
					case "criminal":
					case "felon":
					case "pet":
						enforcer.EnforcementAuthoritiesArrestableLegalClasses.Add(
							new EnforcementAuthoritiesArrestableLegalClasses
							{
								EnforcementAuthority = enforcer,
								LegalClass = legalClass.Value
							});
						enforcer.EnforcementAuthoritiesAccusableClasses.Add(new EnforcementAuthoritiesAccusableClasses
						{
							EnforcementAuthority = enforcer,
							LegalClass = legalClass.Value
						});
						break;
				}
		}

		// Military-specific Enforcers
		if (Classes.ContainsKey("soldier") || Classes.ContainsKey("officer"))
		{
			enforcerProg = new FutureProg
			{
				FunctionName = $"IsMilitaryEnforcer{AuthorityName.CollapseString()}",
				Category = "Law",
				Subcategory = AuthorityName.CollapseString(),
				FunctionComment = "Determines if a character is a military enforcer",
				ReturnType = 4,
				AcceptsAnyParameters = false,
				Public = false,
				StaticType = 0,
				FunctionText = "return false"
			};
			enforcerProg.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = enforcerProg,
				ParameterIndex = 0,
				ParameterName = "ch",
				ParameterType = (long)FutureProgVariableTypes.Character
			});
			Context.FutureProgs.Add(enforcerProg);

			enforcer = new EnforcementAuthority
			{
				Name = "Military Enforcer",
				CanAccuse = true,
				CanForgive = true,
				CanConvict = !separatepowers,
				LegalAuthority = Authority,
				FilterProg = enforcerProg,
				Priority = 2
			};
			Context.EnforcementAuthorities.Add(enforcer);
			foreach (var legalClass in Classes)
				switch (legalClass.Key)
				{
					case "soldier":
					case "officer":
						enforcer.EnforcementAuthoritiesArrestableLegalClasses.Add(
							new EnforcementAuthoritiesArrestableLegalClasses
							{
								EnforcementAuthority = enforcer,
								LegalClass = legalClass.Value
							});
						enforcer.EnforcementAuthoritiesAccusableClasses.Add(new EnforcementAuthoritiesAccusableClasses
						{
							EnforcementAuthority = enforcer,
							LegalClass = legalClass.Value
						});
						break;
				}

			if (separatepowers)
			{
				enforcerProg = new FutureProg
				{
					FunctionName = $"IsMilitaryJudge{AuthorityName.CollapseString()}",
					Category = "Law",
					Subcategory = AuthorityName.CollapseString(),
					FunctionComment = "Determines if a character is a military judge",
					ReturnType = 4,
					AcceptsAnyParameters = false,
					Public = false,
					StaticType = 0,
					FunctionText = "return false"
				};
				enforcerProg.FutureProgsParameters.Add(new FutureProgsParameter
				{
					FutureProg = enforcerProg,
					ParameterIndex = 0,
					ParameterName = "ch",
					ParameterType = (long)FutureProgVariableTypes.Character
				});
				Context.FutureProgs.Add(enforcerProg);

				enforcer = new EnforcementAuthority
				{
					Name = "Military Judge",
					CanAccuse = false,
					CanForgive = false,
					CanConvict = true,
					LegalAuthority = Authority,
					FilterProg = enforcerProg,
					Priority = 3
				};
				Context.EnforcementAuthorities.Add(enforcer);
				foreach (var legalClass in Classes)
					switch (legalClass.Key)
					{
						case "soldier":
						case "officer":
							enforcer.EnforcementAuthoritiesArrestableLegalClasses.Add(
								new EnforcementAuthoritiesArrestableLegalClasses
								{
									EnforcementAuthority = enforcer,
									LegalClass = legalClass.Value
								});
							enforcer.EnforcementAuthoritiesAccusableClasses.Add(
								new EnforcementAuthoritiesAccusableClasses
								{
									EnforcementAuthority = enforcer,
									LegalClass = legalClass.Value
								});
							break;
					}
			}
		}

		// Noble-specific enforcers
		if (Classes.ContainsKey("noble") || Classes.ContainsKey("sovereign"))
		{
			enforcerProg = new FutureProg
			{
				FunctionName = $"IsNobleEnforcer{AuthorityName.CollapseString()}",
				Category = "Law",
				Subcategory = AuthorityName.CollapseString(),
				FunctionComment = "Determines if a character is a noble enforcer",
				ReturnType = 4,
				AcceptsAnyParameters = false,
				Public = false,
				StaticType = 0,
				FunctionText = "return false"
			};
			enforcerProg.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = enforcerProg,
				ParameterIndex = 0,
				ParameterName = "ch",
				ParameterType = (long)FutureProgVariableTypes.Character
			});
			Context.FutureProgs.Add(enforcerProg);

			enforcer = new EnforcementAuthority
			{
				Name = "Noble Enforcer",
				CanAccuse = true,
				CanForgive = true,
				CanConvict = !separatepowers,
				LegalAuthority = Authority,
				FilterProg = enforcerProg,
				Priority = 2
			};
			Context.EnforcementAuthorities.Add(enforcer);
			foreach (var legalClass in Classes)
				switch (legalClass.Key)
				{
					case "immune":
						continue;
					case "sovereign":
					case "noble":
					case "enforcer":
					case "soldier":
					case "officer":
					case "citizen":
					case "non-citizen":
					case "noncitizen":
					case "slave":
					case "criminal":
					case "felon":
					case "pet":
						enforcer.EnforcementAuthoritiesArrestableLegalClasses.Add(
							new EnforcementAuthoritiesArrestableLegalClasses
							{
								EnforcementAuthority = enforcer,
								LegalClass = legalClass.Value
							});
						enforcer.EnforcementAuthoritiesAccusableClasses.Add(new EnforcementAuthoritiesAccusableClasses
						{
							EnforcementAuthority = enforcer,
							LegalClass = legalClass.Value
						});
						break;
				}

			if (separatepowers)
			{
				enforcerProg = new FutureProg
				{
					FunctionName = $"IsNobleJudge{AuthorityName.CollapseString()}",
					Category = "Law",
					Subcategory = AuthorityName.CollapseString(),
					FunctionComment = "Determines if a character is a noble judge",
					ReturnType = 4,
					AcceptsAnyParameters = false,
					Public = false,
					StaticType = 0,
					FunctionText = "return false"
				};
				enforcerProg.FutureProgsParameters.Add(new FutureProgsParameter
				{
					FutureProg = enforcerProg,
					ParameterIndex = 0,
					ParameterName = "ch",
					ParameterType = (long)FutureProgVariableTypes.Character
				});
				Context.FutureProgs.Add(enforcerProg);

				enforcer = new EnforcementAuthority
				{
					Name = "Noble Judge",
					CanAccuse = false,
					CanForgive = false,
					CanConvict = true,
					LegalAuthority = Authority,
					FilterProg = enforcerProg,
					Priority = 3
				};
				Context.EnforcementAuthorities.Add(enforcer);
				foreach (var legalClass in Classes)
					switch (legalClass.Key)
					{
						case "immune":
							continue;
						case "sovereign":
						case "noble":
						case "enforcer":
						case "soldier":
						case "officer":
						case "citizen":
						case "non-citizen":
						case "noncitizen":
						case "slave":
						case "criminal":
						case "felon":
						case "pet":
							enforcer.EnforcementAuthoritiesArrestableLegalClasses.Add(
								new EnforcementAuthoritiesArrestableLegalClasses
								{
									EnforcementAuthority = enforcer,
									LegalClass = legalClass.Value
								});
							enforcer.EnforcementAuthoritiesAccusableClasses.Add(
								new EnforcementAuthoritiesAccusableClasses
								{
									EnforcementAuthority = enforcer,
									LegalClass = legalClass.Value
								});
							break;
					}
			}
		}

		Context.SaveChanges();
	}

	private void SetupClasses(List<string> classNames)
	{
		var classes = new Dictionary<string, LegalClass>();
		foreach (var className in classNames)
		{
			FutureProg classProg;
			LegalClass legalClass;
			switch (className)
			{
				case "immune":
					classProg = new FutureProg
					{
						FunctionName = $"IsLegalClass{AuthorityName.CollapseString()}Immune",
						Category = "Law",
						Subcategory = AuthorityName.CollapseString(),
						FunctionComment =
							"Determines if a character belongs to the legal class that is immune from all law",
						ReturnType = 4,
						AcceptsAnyParameters = false,
						Public = false,
						StaticType = 0,
						FunctionText = "return false"
					};
					classProg.FutureProgsParameters.Add(new FutureProgsParameter
					{
						FutureProg = classProg,
						ParameterIndex = 0,
						ParameterName = "ch",
						ParameterType = (long)FutureProgVariableTypes.Character
					});
					legalClass = new LegalClass
					{
						Name = "Immune",
						LegalAuthority = Authority,
						MembershipProg = classProg,
						CanBeDetainedUntilFinesPaid = false,
						LegalClassPriority = 100
					};
					break;
				case "sovereign":
					classProg = new FutureProg
					{
						FunctionName = $"IsLegalClass{AuthorityName.CollapseString()}Sovereign",
						Category = "Law",
						Subcategory = AuthorityName.CollapseString(),
						FunctionComment = "Determines if a character belongs to the sovereign legal class",
						ReturnType = 4,
						AcceptsAnyParameters = false,
						Public = false,
						StaticType = 0,
						FunctionText = "return false"
					};
					classProg.FutureProgsParameters.Add(new FutureProgsParameter
					{
						FutureProg = classProg,
						ParameterIndex = 0,
						ParameterName = "ch",
						ParameterType = (long)FutureProgVariableTypes.Character
					});
					legalClass = new LegalClass
					{
						Name = "Sovereign",
						LegalAuthority = Authority,
						MembershipProg = classProg,
						CanBeDetainedUntilFinesPaid = false,
						LegalClassPriority = 90
					};
					break;
				case "noble":
					classProg = new FutureProg
					{
						FunctionName = $"IsLegalClass{AuthorityName.CollapseString()}Noble",
						Category = "Law",
						Subcategory = AuthorityName.CollapseString(),
						FunctionComment = "Determines if a character belongs to the noble legal class",
						ReturnType = 4,
						AcceptsAnyParameters = false,
						Public = false,
						StaticType = 0,
						FunctionText = "return false"
					};
					classProg.FutureProgsParameters.Add(new FutureProgsParameter
					{
						FutureProg = classProg,
						ParameterIndex = 0,
						ParameterName = "ch",
						ParameterType = (long)FutureProgVariableTypes.Character
					});
					legalClass = new LegalClass
					{
						Name = "Noble",
						LegalAuthority = Authority,
						MembershipProg = classProg,
						CanBeDetainedUntilFinesPaid = false,
						LegalClassPriority = 80
					};
					break;

				case "soldier":
					classProg = new FutureProg
					{
						FunctionName = $"IsLegalClass{AuthorityName.CollapseString()}Soldier",
						Category = "Law",
						Subcategory = AuthorityName.CollapseString(),
						FunctionComment = "Determines if a character belongs to the soldier legal class",
						ReturnType = 4,
						AcceptsAnyParameters = false,
						Public = false,
						StaticType = 0,
						FunctionText = "return false"
					};
					classProg.FutureProgsParameters.Add(new FutureProgsParameter
					{
						FutureProg = classProg,
						ParameterIndex = 0,
						ParameterName = "ch",
						ParameterType = (long)FutureProgVariableTypes.Character
					});
					legalClass = new LegalClass
					{
						Name = "Soldier",
						LegalAuthority = Authority,
						MembershipProg = classProg,
						CanBeDetainedUntilFinesPaid = true,
						LegalClassPriority = 70
					};
					break;
				case "officer":
					classProg = new FutureProg
					{
						FunctionName = $"IsLegalClass{AuthorityName.CollapseString()}Officer",
						Category = "Law",
						Subcategory = AuthorityName.CollapseString(),
						FunctionComment = "Determines if a character belongs to the officer legal class",
						ReturnType = 4,
						AcceptsAnyParameters = false,
						Public = false,
						StaticType = 0,
						FunctionText = "return false"
					};
					classProg.FutureProgsParameters.Add(new FutureProgsParameter
					{
						FutureProg = classProg,
						ParameterIndex = 0,
						ParameterName = "ch",
						ParameterType = (long)FutureProgVariableTypes.Character
					});
					legalClass = new LegalClass
					{
						Name = "Officer",
						LegalAuthority = Authority,
						MembershipProg = classProg,
						CanBeDetainedUntilFinesPaid = true,
						LegalClassPriority = 75
					};
					break;
				case "enforcer":
					classProg = new FutureProg
					{
						FunctionName = $"IsLegalClass{AuthorityName.CollapseString()}Enforcer",
						Category = "Law",
						Subcategory = AuthorityName.CollapseString(),
						FunctionComment = "Determines if a character belongs to the enforcer legal class",
						ReturnType = 4,
						AcceptsAnyParameters = false,
						Public = false,
						StaticType = 0,
						FunctionText = $"return IsEnforcer(@ch, ToLegalAuthority({Authority.Id}))"
					};
					classProg.FutureProgsParameters.Add(new FutureProgsParameter
					{
						FutureProg = classProg,
						ParameterIndex = 0,
						ParameterName = "ch",
						ParameterType = (long)FutureProgVariableTypes.Character
					});
					legalClass = new LegalClass
					{
						Name = "Enforcer",
						LegalAuthority = Authority,
						MembershipProg = classProg,
						CanBeDetainedUntilFinesPaid = true,
						LegalClassPriority = 60
					};
					break;
				case "citizen":
					classProg = new FutureProg
					{
						FunctionName = $"IsLegalClass{AuthorityName.CollapseString()}Citizen",
						Category = "Law",
						Subcategory = AuthorityName.CollapseString(),
						FunctionComment = "Determines if a character belongs to the citizen legal class",
						ReturnType = 4,
						AcceptsAnyParameters = false,
						Public = false,
						StaticType = 0,
						FunctionText = "return false"
					};
					classProg.FutureProgsParameters.Add(new FutureProgsParameter
					{
						FutureProg = classProg,
						ParameterIndex = 0,
						ParameterName = "ch",
						ParameterType = (long)FutureProgVariableTypes.Character
					});
					legalClass = new LegalClass
					{
						Name = "Citizen",
						LegalAuthority = Authority,
						MembershipProg = classProg,
						CanBeDetainedUntilFinesPaid = true,
						LegalClassPriority = 50
					};
					break;
				case "noncitizen":
				case "non-citizen":
					classProg = new FutureProg
					{
						FunctionName = $"IsLegalClass{AuthorityName.CollapseString()}NonCitizen",
						Category = "Law",
						Subcategory = AuthorityName.CollapseString(),
						FunctionComment = "Determines if a character belongs to the non-citizen legal class",
						ReturnType = 4,
						AcceptsAnyParameters = false,
						Public = false,
						StaticType = 0,
						FunctionText = "return false"
					};
					classProg.FutureProgsParameters.Add(new FutureProgsParameter
					{
						FutureProg = classProg,
						ParameterIndex = 0,
						ParameterName = "ch",
						ParameterType = (long)FutureProgVariableTypes.Character
					});
					legalClass = new LegalClass
					{
						Name = "Non-Citizen",
						LegalAuthority = Authority,
						MembershipProg = classProg,
						CanBeDetainedUntilFinesPaid = true,
						LegalClassPriority = 40
					};
					break;
				case "slave":
					classProg = new FutureProg
					{
						FunctionName = $"IsLegalClass{AuthorityName.CollapseString()}Slave",
						Category = "Law",
						Subcategory = AuthorityName.CollapseString(),
						FunctionComment = "Determines if a character belongs to the slave legal class",
						ReturnType = 4,
						AcceptsAnyParameters = false,
						Public = false,
						StaticType = 0,
						FunctionText = "return false"
					};
					classProg.FutureProgsParameters.Add(new FutureProgsParameter
					{
						FutureProg = classProg,
						ParameterIndex = 0,
						ParameterName = "ch",
						ParameterType = (long)FutureProgVariableTypes.Character
					});
					legalClass = new LegalClass
					{
						Name = "Slave",
						LegalAuthority = Authority,
						MembershipProg = classProg,
						CanBeDetainedUntilFinesPaid = true,
						LegalClassPriority = 30
					};
					break;
				case "felon":
					classProg = new FutureProg
					{
						FunctionName = $"IsLegalClass{AuthorityName.CollapseString()}Felon",
						Category = "Law",
						Subcategory = AuthorityName.CollapseString(),
						FunctionComment = "Determines if a character belongs to the felon legal class",
						ReturnType = 4,
						AcceptsAnyParameters = false,
						Public = false,
						StaticType = 0,
						FunctionText = $"return IsFelon(@ch, ToLegalAuthority({Authority.Id}))"
					};
					classProg.FutureProgsParameters.Add(new FutureProgsParameter
					{
						FutureProg = classProg,
						ParameterIndex = 0,
						ParameterName = "ch",
						ParameterType = (long)FutureProgVariableTypes.Character
					});
					legalClass = new LegalClass
					{
						Name = "Felon",
						LegalAuthority = Authority,
						MembershipProg = classProg,
						CanBeDetainedUntilFinesPaid = true,
						LegalClassPriority = 55
					};
					break;
				case "criminal":
					classProg = new FutureProg
					{
						FunctionName = $"IsLegalClass{AuthorityName.CollapseString()}Criminal",
						Category = "Law",
						Subcategory = AuthorityName.CollapseString(),
						FunctionComment = "Determines if a character belongs to the criminal legal class",
						ReturnType = 4,
						AcceptsAnyParameters = false,
						Public = false,
						StaticType = 0,
						FunctionText =
							$"return KnownCrimes(@ch, ToLegalAuthority({Authority.Id})).Any(x, @x.isarrestable or @x.iskillable)"
					};
					classProg.FutureProgsParameters.Add(new FutureProgsParameter
					{
						FutureProg = classProg,
						ParameterIndex = 0,
						ParameterName = "ch",
						ParameterType = (long)FutureProgVariableTypes.Character
					});
					legalClass = new LegalClass
					{
						Name = "Criminal",
						LegalAuthority = Authority,
						MembershipProg = classProg,
						CanBeDetainedUntilFinesPaid = true,
						LegalClassPriority = 54
					};
					break;
				case "pet":
					classProg = new FutureProg
					{
						FunctionName = $"IsLegalClass{AuthorityName.CollapseString()}Pet",
						Category = "Law",
						Subcategory = AuthorityName.CollapseString(),
						FunctionComment = "Determines if a character belongs to the pet legal class",
						ReturnType = 4,
						AcceptsAnyParameters = false,
						Public = false,
						StaticType = 0,
						FunctionText = "return false"
					};
					classProg.FutureProgsParameters.Add(new FutureProgsParameter
					{
						FutureProg = classProg,
						ParameterIndex = 0,
						ParameterName = "ch",
						ParameterType = (long)FutureProgVariableTypes.Character
					});
					legalClass = new LegalClass
					{
						Name = "Pet",
						LegalAuthority = Authority,
						MembershipProg = classProg,
						CanBeDetainedUntilFinesPaid = true,
						LegalClassPriority = 10
					};
					break;
				case "other":
					classProg = new FutureProg
					{
						FunctionName = $"IsLegalClass{AuthorityName.CollapseString()}Other",
						Category = "Law",
						Subcategory = AuthorityName.CollapseString(),
						FunctionComment = "Determines if a character does not belong to any other legal class",
						ReturnType = 4,
						AcceptsAnyParameters = false,
						Public = false,
						StaticType = 0,
						FunctionText = "return true"
					};
					classProg.FutureProgsParameters.Add(new FutureProgsParameter
					{
						FutureProg = classProg,
						ParameterIndex = 0,
						ParameterName = "ch",
						ParameterType = (long)FutureProgVariableTypes.Character
					});
					legalClass = new LegalClass
					{
						Name = "Other",
						LegalAuthority = Authority,
						MembershipProg = classProg,
						CanBeDetainedUntilFinesPaid = true,
						LegalClassPriority = 0
					};
					break;
				default:
					continue;
			}

			Context.LegalClasses.Add(legalClass);
			Context.FutureProgs.Add(classProg);
			classes[className] = legalClass;
			ProgLookup[$"is{className}"] = classProg;
		}

		Context.SaveChanges();
		Classes = classes;
	}

	private TimeSpan DefaultActivePeriod(CrimeTypes type)
	{
		if (type.IsMajorCrime()) return TimeSpan.FromDays(3650);
		if (type.IsViolentCrime()) return TimeSpan.FromDays(730);
		if (type.IsMoralCrime()) return TimeSpan.FromDays(365);
		return TimeSpan.FromDays(7);
	}

	private int DefaultEnforcementPriority(CrimeTypes type)
	{
		if (type.IsMajorCrime()) return 1000;
		if (type.IsViolentCrime()) return 500;
		if (type.IsMoralCrime()) return 200;
		return 100;
	}

	private void CreateLaw(string name, CrimeTypes type, EnforcementStrategy enforcement, IEnumerable<string> victims,
		IEnumerable<string> offenders, CrimeContext context)
	{
		string punishmentStrategy;
		if (context.BondLength > MudTimeSpan.Zero)
		{
			if (context.UseCapitalPunishment && context.Execute)
				punishmentStrategy =
					$"<Strategy type=\"hierarchy\"><Member prog=\"{ProgLookup["isgood"].Id}\"><Strategy type=\"execute\"></Strategy></Member><Member prog=\"1\"><Strategy type=\"bond\"><Length>{context.BondLength.GetRoundTripParseText}</Length></Strategy></Member></Strategy>";
			else if (context.UseImprisonment && context.MinimumImprisonmentLength > MudTimeSpan.Zero)
				punishmentStrategy =
					$"<Strategy type=\"hierarchy\"><Member prog=\"{ProgLookup["isgood"].Id}\"><Strategy type=\"jail\"><Minimum>{context.MinimumImprisonmentLength.GetRoundTripParseText}</Minimum><Maximum>{context.MaximumImprisonmentLength.GetRoundTripParseText}</Maximum></Strategy></Member><Member prog=\"1\"><Strategy type=\"bond\"><Length>{context.BondLength.GetRoundTripParseText}</Length></Strategy></Member></Strategy>";
			else if (context.PenaltyUnitMultiplier > 0)
				punishmentStrategy =
					$"<Strategy type=\"hierarchy\"><Member prog=\"{ProgLookup["isgood"].Id}\"><Strategy type=\"fine\"><DefaultFineAmount>{context.BasePenaltyUnits * context.PenaltyUnitMultiplier}</DefaultFineAmount><MaximumFineAmount>{context.BasePenaltyUnits * context.PenaltyUnitMultiplier * 2}</MaximumFineAmount></Strategy></Member><Member prog=\"1\"><Strategy type=\"bond\"><Length>{context.BondLength.GetRoundTripParseText}</Length></Strategy></Member></Strategy>";
			else
				punishmentStrategy =
					$"<Strategy type=\"bond\"><Length>{context.BondLength.GetRoundTripParseText}</Length></Strategy></Member></Strategy>";
		}
		else
		{
			if (context.UseCapitalPunishment && context.Execute)
				punishmentStrategy = "<Strategy type=\"execute\"></Strategy>";
			else if (context.UseImprisonment && context.MinimumImprisonmentLength > MudTimeSpan.Zero)
				punishmentStrategy =
					$"<Strategy type=\"jail\"><Minimum>{context.MinimumImprisonmentLength.GetRoundTripParseText}</Minimum><Maximum>{context.MaximumImprisonmentLength.GetRoundTripParseText}</Maximum></Strategy>";
			else if (context.PenaltyUnitMultiplier > 0)
				punishmentStrategy =
					$"<Strategy type=\"fine\"><DefaultFineAmount>{context.BasePenaltyUnits * context.PenaltyUnitMultiplier}</DefaultFineAmount><MaximumFineAmount>{context.BasePenaltyUnits * context.PenaltyUnitMultiplier * 2}</MaximumFineAmount></Strategy>";
			else
				return; // No punishments set
		}


		var law = new Law
		{
			Name = name,
			LegalAuthority = Authority,
			CrimeType = (int)type,
			CanBeAppliedAutomatically = context.Automatic,
			CanBeArrested = context.CanBeArrested,
			CanBeOfferedBail = context.CanBeOfferedBail,
			DoNotAutomaticallyApplyRepeats = context.NoRepeats,
			EnforcementPriority = DefaultEnforcementPriority(type),
			ActivePeriod = DefaultActivePeriod(type).TotalSeconds,
			EnforcementStrategy = enforcement.DescribeEnum(),
			PunishmentStrategy = punishmentStrategy
		};
		foreach (var @class in victims)
			law.LawsVictimClasses.Add(new LawsVictimClasses
			{
				Law = law,
				LegalClass = Classes[@class]
			});
		foreach (var @class in offenders)
			law.LawsOffenderClasses.Add(new LawsOffenderClasses
			{
				Law = law,
				LegalClass = Classes[@class]
			});
		Context.Add(law);
	}

	protected void SetupLaws()
	{
		var penaltyUnits = uint.Parse(QuestionAnswers["penaltyunits"]);
		var punishmentLevel = QuestionAnswers["punishmentlevel"];

		switch (punishmentLevel.ToLowerInvariant())
		{
			case "tiered":
				//SetupTieredLaws(penaltyUnits, useReligiousLaws);
				break;
			case "weregild":
				SetupFlatLaws(new CrimeContext(penaltyUnits, false, false, true, MudTimeSpan.Zero, MudTimeSpan.Zero,
					MudTimeSpan.Zero));
				break;
			case "western":
				SetupFlatLaws(new CrimeContext(penaltyUnits, false, true, true, MudTimeSpan.Zero, MudTimeSpan.Zero,
					MudTimeSpan.Zero));
				break;
			case "liberal":
				SetupFlatLaws(new CrimeContext(penaltyUnits, false, true, false, MudTimeSpan.Zero, MudTimeSpan.Zero,
					MudTimeSpan.Zero));
				break;
			case "theocracy":
				SetupFlatLaws(new CrimeContext(penaltyUnits, true, true, true, MudTimeSpan.Zero, MudTimeSpan.Zero,
					MudTimeSpan.Zero));
				break;
		}

		Context.SaveChanges();
	}

	private void SetupFlatLaws(CrimeContext crimeContext)
	{
		var sophontPerps = Classes
			.Select(x => x.Key)
			.Where(x => x.EqualToAny("noble", "enforcer", "criminal", "felon", "citizen", "noncitizen", "slave",
				"soldier", "officer"))
			.ToList();
		var sophontVictims = Classes
			.Select(x => x.Key)
			.Where(x => x.EqualToAny("noble", "enforcer", "criminal", "felon", "citizen", "noncitizen", "slave",
				"soldier", "officer", "immune", "sovereign"))
			.ToList();
		var animalsAndOther = Classes
			.Select(x => x.Key)
			.Where(x => x.EqualToAny("other", "pet"))
			.ToList();
		var soldiers = Classes
			.Select(x => x.Key)
			.Where(x => x.EqualToAny("soldier", "officer", "slave"))
			.ToList();
		var nobles = Classes
			.Select(x => x.Key)
			.Where(x => x.EqualToAny("noble", "sovereign"))
			.ToList();

		foreach (var crime in Enum.GetValues(typeof(CrimeTypes)).OfType<CrimeTypes>())
			switch (crime)
			{
				case CrimeTypes.Assault:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.ArrestAndDetain, sophontVictims,
						sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = true,
							BondLength = MudTimeSpan.FromMonths(6),
							MinimumImprisonmentLength = MudTimeSpan.FromWeeks(2),
							MaximumImprisonmentLength = MudTimeSpan.FromMonths(6),
							PenaltyUnitMultiplier = 5
						});

					break;
				case CrimeTypes.AssaultWithADeadlyWeapon:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.LethalForceArrestAndDetain,
						sophontVictims, sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = false,
							BondLength = MudTimeSpan.FromMonths(6),
							MinimumImprisonmentLength = MudTimeSpan.FromWeeks(4),
							MaximumImprisonmentLength = MudTimeSpan.FromMonths(6),
							PenaltyUnitMultiplier = 10
						});
					break;
				case CrimeTypes.Battery:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.ArrestAndDetain, sophontVictims,
						sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = true,
							BondLength = MudTimeSpan.FromMonths(6),
							MinimumImprisonmentLength = MudTimeSpan.FromWeeks(2),
							MaximumImprisonmentLength = MudTimeSpan.FromMonths(6),
							PenaltyUnitMultiplier = 5
						});
					break;
				case CrimeTypes.AttemptedMurder:
					break;
				case CrimeTypes.Murder:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.LethalForceArrestAndDetain,
						sophontVictims, sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = false,
							BondLength = MudTimeSpan.Zero,
							MinimumImprisonmentLength = MudTimeSpan.FromMonths(8),
							MaximumImprisonmentLength = MudTimeSpan.FromMonths(48),
							PenaltyUnitMultiplier = 50
						});
					break;
				case CrimeTypes.Manslaughter:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.ArrestAndDetain, sophontVictims,
						sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = true,
							BondLength = MudTimeSpan.Zero,
							MinimumImprisonmentLength = MudTimeSpan.FromWeeks(2),
							MaximumImprisonmentLength = MudTimeSpan.FromMonths(6),
							PenaltyUnitMultiplier = 20
						});
					CreateLaw("Involuntary Manslaughter", crime, EnforcementStrategy.ArrestAndDetain, sophontVictims,
						sophontPerps, crimeContext with
						{
							Automatic = false,
							CanBeArrested = true,
							CanBeOfferedBail = true,
							BondLength = MudTimeSpan.Zero,
							MinimumImprisonmentLength = MudTimeSpan.FromWeeks(1),
							MaximumImprisonmentLength = MudTimeSpan.FromMonths(3),
							PenaltyUnitMultiplier = 10
						});
					break;
				case CrimeTypes.Torture:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.ArrestAndDetain, sophontVictims,
						sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = true,
							BondLength = MudTimeSpan.Zero,
							MinimumImprisonmentLength = MudTimeSpan.FromWeeks(2),
							MaximumImprisonmentLength = MudTimeSpan.FromMonths(6),
							PenaltyUnitMultiplier = 10
						});
					break;
				case CrimeTypes.GreviousBodilyHarm:
				case CrimeTypes.Mayhem:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.LethalForceArrestAndDetain,
						sophontVictims, sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = false,
							BondLength = MudTimeSpan.FromMonths(6),
							MinimumImprisonmentLength = MudTimeSpan.FromWeeks(4),
							MaximumImprisonmentLength = MudTimeSpan.FromMonths(6),
							PenaltyUnitMultiplier = 10
						});
					break;
				case CrimeTypes.Theft:
				case CrimeTypes.Fraud:
				case CrimeTypes.Forgery:
				case CrimeTypes.Racketeering:
				case CrimeTypes.CartelCollusion:
				case CrimeTypes.UnauthorisedDealing:
				case CrimeTypes.Embezzlement:
				case CrimeTypes.PossessingStolenGoods:
				case CrimeTypes.TaxEvasion:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.ArrestAndDetain, sophontVictims,
						sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = true,
							BondLength = MudTimeSpan.FromMonths(6),
							MinimumImprisonmentLength = MudTimeSpan.FromWeeks(2),
							MaximumImprisonmentLength = MudTimeSpan.FromMonths(6),
							PenaltyUnitMultiplier = 10
						});
					break;
				case CrimeTypes.SellingContraband:
				case CrimeTypes.PossessingContraband:
				case CrimeTypes.TrafficingContraband:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.ArrestAndDetain, sophontVictims,
						sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = true,
							BondLength = MudTimeSpan.FromMonths(3),
							MinimumImprisonmentLength = MudTimeSpan.FromDays(2),
							MaximumImprisonmentLength = MudTimeSpan.FromMonths(1),
							PenaltyUnitMultiplier = 2
						});
					break;
				case CrimeTypes.Vandalism:
				case CrimeTypes.BreakAndEnter:
				case CrimeTypes.DestructionOfProperty:
				case CrimeTypes.Arson:
				case CrimeTypes.Negligence:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.ArrestAndDetain, sophontVictims,
						sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = true,
							BondLength = MudTimeSpan.FromMonths(6),
							MinimumImprisonmentLength = MudTimeSpan.FromWeeks(2),
							MaximumImprisonmentLength = MudTimeSpan.FromMonths(6),
							PenaltyUnitMultiplier = 5
						});
					break;
				case CrimeTypes.Loitering:
				case CrimeTypes.Littering:
				case CrimeTypes.Trespassing:
				case CrimeTypes.Vagrancy:
				case CrimeTypes.Gambling:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.ArrestAndDetain, sophontVictims,
						sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = true,
							BondLength = MudTimeSpan.FromMonths(1),
							MinimumImprisonmentLength = MudTimeSpan.Zero,
							MaximumImprisonmentLength = MudTimeSpan.Zero,
							PenaltyUnitMultiplier = 1
						});
					break;
				case CrimeTypes.Libel:
				case CrimeTypes.Slander:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.ArrestAndDetain, sophontVictims,
						sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = true,
							BondLength = MudTimeSpan.Zero,
							MinimumImprisonmentLength = MudTimeSpan.Zero,
							MaximumImprisonmentLength = MudTimeSpan.Zero,
							PenaltyUnitMultiplier = 5
						});
					break;
				case CrimeTypes.Treason:
				case CrimeTypes.Rebellion:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.LethalForceArrestAndDetainNoWarning,
						sophontVictims, sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = false,
							BondLength = MudTimeSpan.Zero,
							MinimumImprisonmentLength = MudTimeSpan.FromYears(10),
							MaximumImprisonmentLength = MudTimeSpan.FromYears(100),
							PenaltyUnitMultiplier = 50,
							Execute = true
						});
					break;

				case CrimeTypes.Rioting:
				case CrimeTypes.Conspiracy:
				case CrimeTypes.Sedition:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.ArrestAndDetain, sophontVictims,
						sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = false,
							BondLength = MudTimeSpan.Zero,
							MinimumImprisonmentLength = MudTimeSpan.FromWeeks(1),
							MaximumImprisonmentLength = MudTimeSpan.FromMonths(2),
							PenaltyUnitMultiplier = 12
						});
					break;
				case CrimeTypes.Intimidation:
				case CrimeTypes.Blackmail:
				case CrimeTypes.Extortion:
				case CrimeTypes.Harassment:
				case CrimeTypes.Smuggling:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.ArrestAndDetain, sophontVictims,
						sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = true,
							BondLength = MudTimeSpan.FromMonths(12),
							MinimumImprisonmentLength = MudTimeSpan.FromWeeks(1),
							MaximumImprisonmentLength = MudTimeSpan.FromMonths(2),
							PenaltyUnitMultiplier = 3
						});
					break;
				case CrimeTypes.ResistArrest:
				case CrimeTypes.DisobeyLegalInstruction:
				case CrimeTypes.ViolateParole:
				case CrimeTypes.ViolateBail:
				case CrimeTypes.ContemptOfCourt:
				case CrimeTypes.Bribery:
				case CrimeTypes.Perjury:
				case CrimeTypes.ObstructionOfJustice:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.ArrestAndDetain, sophontVictims,
						sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = false,
							BondLength = MudTimeSpan.Zero,
							MinimumImprisonmentLength = MudTimeSpan.FromWeeks(1),
							MaximumImprisonmentLength = MudTimeSpan.FromMonths(2),
							PenaltyUnitMultiplier = 5
						});
					break;
				case CrimeTypes.EscapeCaptivity:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.LethalForceArrestAndDetainNoWarning,
						sophontVictims, sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = false,
							BondLength = MudTimeSpan.Zero,
							MinimumImprisonmentLength = MudTimeSpan.FromYears(5),
							MaximumImprisonmentLength = MudTimeSpan.FromYears(20),
							PenaltyUnitMultiplier = 100,
							Execute = true
						});
					break;
				case CrimeTypes.Indecency:
				case CrimeTypes.PublicIntoxication:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.ArrestAndDetain, sophontVictims,
						sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = true,
							BondLength = MudTimeSpan.FromMonths(6),
							MinimumImprisonmentLength = MudTimeSpan.Zero,
							MaximumImprisonmentLength = MudTimeSpan.Zero,
							PenaltyUnitMultiplier = 5
						});
					break;
				case CrimeTypes.Immorality:
				case CrimeTypes.Profanity:
				case CrimeTypes.Adultery:
				case CrimeTypes.Prostitution:
					if (!crimeContext.UseReligiousLaws) continue;
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.ArrestAndDetain, sophontVictims,
						sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = true,
							BondLength = MudTimeSpan.FromYears(1),
							MinimumImprisonmentLength = MudTimeSpan.FromWeeks(12),
							MaximumImprisonmentLength = MudTimeSpan.FromYears(2),
							PenaltyUnitMultiplier = 7
						});
					break;
				case CrimeTypes.Blasphemy:
				case CrimeTypes.Apostasy:
				case CrimeTypes.Sodomy:
				case CrimeTypes.Fornication:
					if (!crimeContext.UseReligiousLaws) continue;
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.LethalForceArrestAndDetain,
						sophontVictims, sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = true,
							BondLength = MudTimeSpan.Zero,
							MinimumImprisonmentLength = MudTimeSpan.FromYears(1),
							MaximumImprisonmentLength = MudTimeSpan.FromYears(10),
							PenaltyUnitMultiplier = 50,
							Execute = true
						});
					break;
				case CrimeTypes.Aiding:
				case CrimeTypes.Abetting:
				case CrimeTypes.Accessory:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.ArrestAndDetain, sophontVictims,
						sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = true,
							BondLength = MudTimeSpan.FromMonths(12),
							MinimumImprisonmentLength = MudTimeSpan.FromWeeks(1),
							MaximumImprisonmentLength = MudTimeSpan.FromMonths(2),
							PenaltyUnitMultiplier = 4
						});
					break;
				case CrimeTypes.Tyranny:
					if (!nobles.Any()) continue;
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.LethalForceArrestAndDetain,
						sophontVictims, nobles, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = true,
							BondLength = MudTimeSpan.Zero,
							MinimumImprisonmentLength = MudTimeSpan.FromYears(10),
							MaximumImprisonmentLength = MudTimeSpan.FromYears(100),
							PenaltyUnitMultiplier = 1000,
							Execute = true
						});
					break;
				case CrimeTypes.Rape:
				case CrimeTypes.SexualAssault:
				case CrimeTypes.Kidnapping:
				case CrimeTypes.Slavery:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.LethalForceArrestAndDetain,
						sophontVictims, soldiers.Any() ? soldiers : sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = false,
							BondLength = MudTimeSpan.Zero,
							MinimumImprisonmentLength = MudTimeSpan.FromYears(1),
							MaximumImprisonmentLength = MudTimeSpan.FromYears(10),
							PenaltyUnitMultiplier = 30,
							Execute = true
						});
					break;
				case CrimeTypes.Desertion:
				case CrimeTypes.Mutiny:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.LethalForceArrestAndDetain,
						sophontVictims, soldiers.Any() ? soldiers : sophontPerps, crimeContext with
						{
							Automatic = true,
							CanBeArrested = true,
							CanBeOfferedBail = true,
							BondLength = MudTimeSpan.Zero,
							MinimumImprisonmentLength = MudTimeSpan.FromYears(10),
							MaximumImprisonmentLength = MudTimeSpan.FromYears(100),
							PenaltyUnitMultiplier = 1000,
							Execute = true
						});
					break;
				case CrimeTypes.AnimalCruelty:
					CreateLaw(crime.DescribeEnum(true), crime, EnforcementStrategy.ArrestAndDetain, animalsAndOther,
						sophontPerps, crimeContext with
						{
							Automatic = false,
							CanBeArrested = true,
							CanBeOfferedBail = true,
							BondLength = MudTimeSpan.Zero,
							MinimumImprisonmentLength = MudTimeSpan.Zero,
							MaximumImprisonmentLength = MudTimeSpan.Zero,
							PenaltyUnitMultiplier = 10
						});
					break;
			}
	}

	private record CrimeContext(uint BasePenaltyUnits, bool UseReligiousLaws, bool UseImprisonment,
		bool UseCapitalPunishment, MudTimeSpan BondLength, MudTimeSpan MinimumImprisonmentLength,
		MudTimeSpan MaximumImprisonmentLength, uint PenaltyUnitMultiplier = 0, bool CanBeArrested = false,
		bool CanBeOfferedBail = false, bool Automatic = false, bool NoRepeats = false, bool Execute = false);
}