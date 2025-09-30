using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public class SkillSeeder : SkillSeederBase
{
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

#BRPI#F    - skill caps are based on character attributes.
#BClass#F  - skill caps are determined by your class (obviously use with class system enabled)
#BFlat#F   - skill caps are flat across the board, the same for everyone (but you might shift with projects)

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
			("exampleskill",
				@"Please enter some names of example skills that you want to use (separate multiple skills with commas)

#1Note: Don't use combat skills as an example as you will set these up later in the combat seeder#f",
				(context, answers) => true,
				(text, context) => (!string.IsNullOrWhiteSpace(text), "That is not a valid selection.")),
			("skillattribute",
				@"Please enter the name of the attribute you want to tie the cap of that skill to: ",
				(context, answers) => true && answers["skillcapmodel"].EqualTo("rpi"),
				(text, context) => (
					context.TraitDefinitions.Any(x => x.Name == text) ||
					context.TraitDefinitions.Any(x => x.Alias == text), "That is not a valid selection.")),
			("examplelanguage",
				@"Please enter the name of an example language skill that you want to use, or enter a blank line to not install an example language (if you're intending to import a culture pack for example): ",
				(context, answers) => true,
				(text, context) => (true, string.Empty)),
			("languageattribute",
				@"Please enter the name of the attribute you want to tie the cap of language skills to: ",
				(context, answers) => !string.IsNullOrEmpty(answers["examplelanguage"]) &&
									  answers["skillcapmodel"].EqualTo("rpi"),
				(text, context) => (
					context.TraitDefinitions.Any(x => x.Name == text) ||
					context.TraitDefinitions.Any(x => x.Alias == text), "That is not a valid selection."))
		};

	public override int SortOrder => 11;
	public override string Name => "Skill Examples";
	public override string Tagline => "Sets up templates and examples for skills";

	public override string FullDescription =>
		@"This package installs much of the supporting information you are going to require for your skill setup, as well as a couple of examples of complete skills to show you what you need to do. 

This includes the following items:

  #3Check Templates#0 - a complete set of templates for check types, including different difficulties
  #3Improvers#0 - a complete set of templates for skill improvement
  #3Describers#0 - a complete set of skill value describers
  #3Example Skills#0 - depending on your choices, example general skills and a language

This package is most useful if you want to go in a radically different direction to what the other #DSkill Package Seeder#0 offers and you just need the templates and some examples to build your own.

Again, the choices you make here can be fixed later so don't stress it too greatly, it is really just to give you a basis to begin your own building.

#1Warning: Don't run both this and the Skill Package Seeder.#0";

	public override string SeedData(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		context.Database.BeginTransaction();
		SeedChecks(context, questionAnswers);
		SeedSkills(context, questionAnswers);
		context.Database.CommitTransaction();
		return "The operation completed successfully.";
	}

	private void SeedChecks(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		var branching = questionAnswers["branching"].EqualToAny("yes", "y");
		SeedCheckTemplates(context, branching);

		void AddCheck(CheckType type, TraitExpression expression, long templateId,
			Difficulty maximumImprovementDifficulty)
		{
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

		var checks = Enum.GetValues(typeof(CheckType)).OfType<CheckType>().ToList();
		var uniquechecks = Enum.GetValues(typeof(CheckType)).OfType<CheckType>().Distinct().ToList();
		foreach (var check in Enum.GetValues(typeof(CheckType)).OfType<CheckType>().Distinct().ToList())
			switch (check)
			{
				case CheckType.None:
					// Default Fall Back
					AddCheck(check, new TraitExpression { Name = "Default Fallback Check", Expression = "variable" }, 3,
						Difficulty.Automatic);
					continue;
				case CheckType.ProjectLabourCheck:
					// Special Project Check
					AddCheck(check,
						new TraitExpression { Name = $"{check.DescribeEnum(true)}", Expression = "variable" }, 9,
						Difficulty.Automatic);
					break;
				case CheckType.HealingCheck:
				case CheckType.StunRecoveryCheck:
				case CheckType.ShockRecoveryCheck:
				case CheckType.PainRecoveryCheck:
				case CheckType.WoundCloseCheck:
					// Health Checks
					AddCheck(check,
						new TraitExpression { Name = $"{check.DescribeEnum(true)}", Expression = "max(5.0, 0.0)" }, 11,
						Difficulty.Automatic);
					break;

				case CheckType.DreamCheck:
				case CheckType.GoToSleepCheck:
					// Special Dream-Related checks
					AddCheck(check, new TraitExpression { Name = $"{check.DescribeEnum(true)}", Expression = "100" }, 8,
						Difficulty.Automatic);
					break;
				case CheckType.ExactTimeCheck:
				case CheckType.VagueTimeCheck:
				case CheckType.StyleCharacteristicCapabilityCheck:
				case CheckType.ImplantRecognitionCheck:
				case CheckType.TreatmentItemRecognitionCheck:
					// Capability Checks
					AddCheck(check, new TraitExpression { Name = $"{check.DescribeEnum(true)}", Expression = "0" }, 8,
						Difficulty.Automatic);
					continue;
				case CheckType.GenericAttributeCheck:
					AddCheck(check,
						new TraitExpression { Name = $"{check.DescribeEnum(true)}", Expression = "variable*5" }, 3,
						Difficulty.Automatic);
					continue;
				case CheckType.GenericSkillCheck:
					AddCheck(check,
						new TraitExpression { Name = $"{check.DescribeEnum(true)}", Expression = "variable" }, 3,
						Difficulty.Automatic);
					continue;
				case CheckType.GenericListenCheck:
				case CheckType.LanguageListenCheck:
				case CheckType.GenericSpotCheck:
				case CheckType.NoticeCheck:
				case CheckType.SpotSneakCheck:
				case CheckType.ScanPerceptionCheck:
				case CheckType.QuickscanPerceptionCheck:
				case CheckType.LongscanPerceptionCheck:
				case CheckType.WatchLocation:
				case CheckType.PassiveStealthCheck:
				case CheckType.ActiveSearchCheck:
					// Perception Checks
					AddCheck(check, new TraitExpression { Name = $"{check.DescribeEnum(true)}", Expression = "50" }, 5,
						Difficulty.Impossible);
					break;
				case CheckType.SpokenLanguageSpeakCheck:
				case CheckType.SpokenLanguageHearCheck:
					// Language Checks
					AddCheck(check,
						new TraitExpression { Name = $"{check.DescribeEnum(true)}", Expression = "variable" }, 4,
						Difficulty.Impossible);
					break;
				case CheckType.AccentAcquireCheck:
				case CheckType.AccentImproveCheck:
					// Static Checks
					AddCheck(check, new TraitExpression { Name = $"{check.DescribeEnum(true)}", Expression = "2.0" },
						12, Difficulty.Automatic);
					break;
				case CheckType.WritingComprehendCheck:
					AddCheck(check, new TraitExpression { Name = $"{check.DescribeEnum(true)}", Expression = "variable" },
						8, Difficulty.Automatic);
					break;
				case CheckType.TraitBranchCheck:
					// Trait Branch Only
					AddCheck(check, new TraitExpression { Name = $"{check.DescribeEnum(true)}", Expression = "0.1" }, 6,
						Difficulty.Automatic);
					break;
				case CheckType.ProjectSkillUseAction:
					// Bonus-Absent Checks
					AddCheck(check,
						new TraitExpression { Name = $"{check.DescribeEnum(true)}", Expression = "variable" }, 10,
						Difficulty.Automatic);
					break;
				case CheckType.SpotStealthCheck:
					AddCheck(check, new TraitExpression { Name = $"{check.DescribeEnum(true)}", Expression = "50" }, 7,
						Difficulty.Impossible);
					break;
				case CheckType.HideCheck:
				case CheckType.SneakCheck:
				case CheckType.PalmCheck:
				case CheckType.HideItemCheck:
				case CheckType.UninstallDoorCheck:
				case CheckType.SkillTeachCheck:
				case CheckType.SkillLearnCheck:
				case CheckType.KnowledgeTeachCheck:
				case CheckType.KnowledgeLearnCheck:
				case CheckType.ForageCheck:
				case CheckType.ForageSpecificCheck:
				case CheckType.ForageTimeCheck:
				case CheckType.BindWoundCheck:
				case CheckType.SutureWoundCheck:
				case CheckType.CleanWoundCheck:
				case CheckType.RemoveLodgedObjectCheck:
				case CheckType.MendCheck:
				case CheckType.MeleeWeaponPenetrateCheck:
				case CheckType.RangedWeaponPenetrateCheck:
				case CheckType.PenetrationDefenseCheck:
				case CheckType.CombatRecoveryCheck:
				case CheckType.MedicalExaminationCheck:
				case CheckType.LocksmithingCheck:
				case CheckType.NaturalWeaponAttack:
				case CheckType.DodgeCheck:
				case CheckType.ParryCheck:
				case CheckType.BlockCheck:
				case CheckType.FleeMeleeCheck:
				case CheckType.OpposeFleeMeleeCheck:
				case CheckType.FleeMovementUnmountedCheck:
				case CheckType.FleeMovementMountedCheck:
				case CheckType.PursuitMovementUnmountedCheck:
				case CheckType.PursuitMovementMountedCheck:
				case CheckType.Ward:
				case CheckType.WardDefense:
				case CheckType.WardIgnore:
				case CheckType.StartClinch:
				case CheckType.ResistClinch:
				case CheckType.BreakClinch:
				case CheckType.ResistBreakClinch:
				case CheckType.ExploratorySurgeryCheck:
				case CheckType.TriageCheck:
				case CheckType.AmputationCheck:
				case CheckType.ReplantationCheck:
				case CheckType.InvasiveProcedureFinalisation:
				case CheckType.TraumaControlSurgery:
				case CheckType.RescueCheck:
				case CheckType.OpposeRescueCheck:
				case CheckType.Defibrillate:
				case CheckType.PerformCPR:
				case CheckType.ArmourUseCheck:
				case CheckType.ReadTextImprovementCheck:
				case CheckType.HandwritingImprovementCheck:
				case CheckType.StaggeringBlowDefense:
				case CheckType.StruggleFreeFromDrag:
				case CheckType.OpposeStruggleFreeFromDrag:
				case CheckType.CounterGrappleCheck:
				case CheckType.StruggleFreeFromGrapple:
				case CheckType.OpposeStruggleFreeFromGrapple:
				case CheckType.ExtendGrappleCheck:
				case CheckType.InitiateGrapple:
				case CheckType.ScreechAttack:
				case CheckType.CrutchWalking:
				case CheckType.OrganExtractionCheck:
				case CheckType.OrganTransplantCheck:
				case CheckType.CannulationProcedure:
				case CheckType.DecannulationProcedure:
				case CheckType.StrangleCheck:
				case CheckType.WrenchAttackCheck:
				case CheckType.OrganStabilisationCheck:
				case CheckType.CraftOutcomeCheck:
				case CheckType.CraftQualityCheck:
				case CheckType.TendWoundCheck:
				case CheckType.RelocateBoneCheck:
				case CheckType.SurgicalSetCheck:
				case CheckType.RepairItemCheck:
				case CheckType.InstallImplantSurgery:
				case CheckType.RemoveImplantSurgery:
				case CheckType.ConfigureImplantPowerSurgery:
				case CheckType.ButcheryCheck:
				case CheckType.SkinningCheck:
				case CheckType.TossItemCheck:
				case CheckType.ClimbCheck:
				case CheckType.ConfigureImplantInterfaceSurgery:
				case CheckType.InkTattooCheck:
				case CheckType.FallingImpactCheck:
				case CheckType.ResistMagicChokePower:
				case CheckType.ResistMagicAnesthesiaPower:
				case CheckType.SwimmingCheck:
				case CheckType.AvoidFallDueToWind:
				case CheckType.SwimStayAfloatCheck:
				case CheckType.FlyCheck:
				case CheckType.CheatAtDiceCheck:
				case CheckType.EvaluateDiceFairnessCheck:
				case CheckType.SpillLiquidOnPerson:
				case CheckType.DodgeSpillLiquidOnPerson:
				case CheckType.DrawingImprovementCheck:
				case CheckType.TakedownCheck:
				case CheckType.BreakoutCheck:
				case CheckType.OpposeBreakoutCheck:
				case CheckType.StyleCharacteristicCheck:
				case CheckType.AppraiseItemCheck:
					// Non-variable skills
					AddCheck(check, new TraitExpression { Name = $"{check.DescribeEnum(true)}", Expression = "50" }, 1,
						Difficulty.Impossible);
					break;

				case CheckType.MeleeWeaponCheck:
				case CheckType.ThrownWeaponCheck:
				case CheckType.AimRangedWeapon:
				case CheckType.FireBow:
				case CheckType.LoadMusket:
				case CheckType.UnjamGun:
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
				case CheckType.CombatMoveCheck:
				case CheckType.CastSpellCheck:
				case CheckType.AuxiliaryMoveCheck:
				case CheckType.ResistMagicSpellCheck:
					// Variable skills
					AddCheck(check,
						new TraitExpression { Name = $"{check.DescribeEnum(true)}", Expression = "variable" }, 1,
						Difficulty.Impossible);
					break;

				case CheckType.InfectionHeartbeat:
				case CheckType.InfectionSpread:
				case CheckType.ReplantedBodypartRejectionCheck:
					// Non-Improving Skill Checks
					AddCheck(check, new TraitExpression { Name = $"{check.DescribeEnum(true)}", Expression = "50" }, 3,
						Difficulty.Automatic);
					break;
			}

		context.SaveChanges();
	}

	private void SeedSkills(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		var skillGainModel = questionAnswers["skillgainmodel"].ToLowerInvariant();

		var (general, _, languageDecorator, _, _, languageImprover, generalImprover) =
			SeedSkillImprovers(context, skillGainModel);

		var skills = questionAnswers["exampleskill"].Split(',', StringSplitOptions.RemoveEmptyEntries)
			.Select(x => x.Trim().TitleCase()).ToList();

		TraitExpression languageCap = null;
		var skillsCaps = new Dictionary<string, TraitExpression>();
		switch (questionAnswers["skillcapmodel"].ToLowerInvariant())
		{
			case "rpi":
				if (!string.IsNullOrEmpty(questionAnswers["examplelanguage"]))
				{
					var langAttrText = questionAnswers["languageattribute"];
					var langAttribute = context.TraitDefinitions.FirstOrDefault(x =>
						x.Name == langAttrText) ?? context.TraitDefinitions.First(x =>
						x.Alias == langAttrText);
					languageCap = new TraitExpression
					{
						Name = $"{questionAnswers["examplelanguage"]} Skill Cap",
						Expression = $"10 + (9.5 * {langAttribute.Alias.ToLowerInvariant()}:{langAttribute.Id})"
					};
				}

				var attrText = questionAnswers["skillattribute"];
				var attribute = context.TraitDefinitions.FirstOrDefault(x =>
									x.Name == attrText) ??
								context.TraitDefinitions.First(x =>
									x.Alias == attrText);
				foreach (var skill in skills)
				{
					var cap = new TraitExpression
					{
						Name = $"{skill} Skill Cap",
						Expression = $"min(99, 5.5 * {attribute.Alias.ToLowerInvariant()}:{attribute.Id})"
					};
					context.TraitExpressions.Add(cap);
					skillsCaps[skill] = cap;
				}

				break;
			case "class":
				if (!string.IsNullOrEmpty(questionAnswers["examplelanguage"]))
				{
					languageCap = new TraitExpression
					{
						Name = $"{questionAnswers["examplelanguage"]} Skill Cap",
						Expression = "130 + ({learned class=wizard,merchant} * 60)"
					};

					context.TraitExpressions.Add(languageCap);
				}

				foreach (var skill in skills)
				{
					var cap = new TraitExpression
					{
						Name = $"{skill} Skill Cap",
						Expression =
							"30 + ({martials class=warrior,ranger,barbarian} * 30) + ({rogues class=thief,bard,} * 10)"
					};
					context.TraitExpressions.Add(cap);
					skillsCaps[skill] = cap;
				}

				break;
			case "flat":
				if (!string.IsNullOrEmpty(questionAnswers["examplelanguage"]))
				{
					languageCap = new TraitExpression
					{
						Name = $"{questionAnswers["examplelanguage"]} Skill Cap",
						Expression = "200"
					};

					context.TraitExpressions.Add(languageCap);
				}

				foreach (var skill in skills)
				{
					var cap = new TraitExpression
					{
						Name = $"{skill} Skill Cap",
						Expression = "70"
					};
					context.TraitExpressions.Add(cap);
					skillsCaps[skill] = cap;
				}

				break;
			default:
				goto case "rpi";
		}


		context.SaveChanges();

		foreach (var skill in skills)
		{
			context.Add(new TraitDefinition
			{
				Name = skill,
				Type = 0,
				DecoratorId = general.Id,
				TraitGroup = "General",
				AvailabilityProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
				TeachableProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse"),
				LearnableProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
				TeachDifficulty = 7,
				LearnDifficulty = 7,
				Hidden = false,
				Expression = skillsCaps[skill],
				ImproverId = generalImprover.Id,
				DerivedType = 0,
				ChargenBlurb = string.Empty,
				BranchMultiplier = 1.0
			});
			context.SaveChanges();
		}


		if (!string.IsNullOrEmpty(questionAnswers["examplelanguage"]))
		{
			var languageSkill = new TraitDefinition
			{
				Name = "Admin",
				Type = 0,
				DecoratorId = languageDecorator.Id,
				TraitGroup = "Language",
				AvailabilityProg = context.FutureProgs.First(x =>
					x.FunctionName == "AlwaysTrue"),
				TeachableProg = context.FutureProgs.First(x =>
					x.FunctionName == "AlwaysFalse"),
				LearnableProg = context.FutureProgs.First(x =>
					x.FunctionName == "AlwaysTrue"),
				TeachDifficulty = 7,
				LearnDifficulty = 7,
				Hidden = false,
				Expression = languageCap,
				ImproverId = languageImprover.Id,
				DerivedType = 0,
				ChargenBlurb = string.Empty,
				BranchMultiplier = 0.1
			};
			context.Add(languageSkill);
			context.SaveChanges();

			var language = new Language
			{
				Name = questionAnswers["examplelanguage"],
				LinkedTrait = languageSkill,
				UnknownLanguageDescription = "an unknown language",
				LanguageObfuscationFactor = 0.1,
				DifficultyModel = context.LanguageDifficultyModels.First().Id
			};
			context.Languages.Add(language);
			context.SaveChanges();
			var accent = new Accent
			{
				Name = "foreign",
				Language = language,
				Suffix = "with a foreign accent",
				VagueSuffix = "with a foreign accent",
				Difficulty = (int)Difficulty.Normal,
				Description = "This is the accent of a non-native speaker who is just beginning to learn the language",
				Group = "foreign"
			};
			language.DefaultLearnerAccent = accent;
			context.Accents.Add(accent);
			context.SaveChanges();
		}
	}


	public override ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.Accounts.Any() || context.TraitDefinitions.All(x => x.Type != 1))
			return ShouldSeedResult.PrerequisitesNotMet;

		if (context.TraitDefinitions.Any(x => x.Type == 0)) return ShouldSeedResult.MayAlreadyBeInstalled;

		return ShouldSeedResult.ReadyToInstall;
	}
}