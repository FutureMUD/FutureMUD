#nullable enable

using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems.Inventory;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders
{
	internal class HealthSeeder : IDatabaseSeeder
	{
		private static readonly string[] RequiredToolTags =
		[
			"Arterial Clamp",
			"Bonesaw",
			"Forceps",
			"Scalpel",
			"Surgical Suture Needle"
		];
		private static readonly string[] StockHealthKnowledges =
		[
			"Medicine",
			"Chiurgery",
			"Physical Medicine",
			"Diagnostic Medicine",
			"Clinical Medicine",
			"Surgery",
			"Animal Medicine",
			"Veterinary Medicine",
			"Veterinary Chiurgery",
			"Veterinary Surgery"
		];
		private static readonly string[] StockHealthProcedures =
		[
			"Triage",
			"Exploratory Surgery",
			"Arm Amputation",
			"Leg Amputation",
			"Digit Amputation",
			"Trauma Control",
			"Organ Extraction",
			"Bone Setting",
			"Stitch Up"
		];
		private static readonly string[] StockHealthDrugs =
		[
			"Willow Bark Tea",
			"Laudanum",
			"General Anaesthetic",
			"Antibiotic Ointment",
			"Burn Gel"
		];
		private static readonly string[] StockDrugDeliveryMarkers =
		[
			"Pill_Willow_Bark_Tea",
			"Pill_Laudanum",
			"TopicalCream_Antibiotic_Ointment",
			"TopicalCream_Burn_Gel"
		];

		private static readonly string[] HumanArmParts =
		[
			"lupperarm",
			"rupperarm",
			"lforearm",
			"rforearm",
			"lhand",
			"rhand"
		];

		private static readonly string[] HumanLegParts =
		[
			"lthigh",
			"rthigh",
			"lshin",
			"rshin",
			"lfoot",
			"rfoot"
		];

		private static readonly string[] HumanDigitParts =
		[
			"lthumb",
			"rthumb",
			"lindexfinger",
			"rindexfinger",
			"lmiddlefinger",
			"rmiddlefinger",
			"lringfinger",
			"rringfinger",
			"lpinkyfinger",
			"rpinkyfinger",
			"lbigtoe",
			"rbigtoe",
			"lindextoe",
			"rindextoe",
			"lmiddletoe",
			"rmiddletoe",
			"lringtoe",
			"rringtoe",
			"lpinkytoe",
			"rpinkytoe"
		];

		private static readonly string[] HumanBrainTargets = ["brain"];
		private static readonly string[] HumanHeartTargets = ["heart"];
		private static readonly string[] HumanLiverTargets = ["liver"];
		private static readonly string[] HumanSpleenTargets = ["spleen"];
		private static readonly string[] HumanStomachTargets = ["stomach"];
		private static readonly string[] HumanIntestineTargets = ["lintestines", "sintestines"];
		private static readonly string[] HumanKidneyTargets = ["rkidney", "lkidney"];
		private static readonly string[] HumanLungTargets = ["rlung", "llung"];
		private static readonly string[] HumanTracheaTargets = ["trachea"];
		private static readonly string[] HumanEsophagusTargets = ["esophagus"];
		private static readonly string[] HumanSpinalTargets = ["uspinalcord", "mspinalcord", "lspinalcord"];
		private static readonly string[] HumanInnerEarTargets = ["rinnerear", "linnerear"];

		private static readonly string[] QuadrupedForelegParts =
		[
			"ruforeleg",
			"luforeleg",
			"rfknee",
			"lfknee",
			"rlforeleg",
			"llforeleg",
			"rfhock",
			"lfhock"
		];

		private static readonly string[] QuadrupedHindlegParts =
		[
			"ruhindleg",
			"luhindleg",
			"rrknee",
			"rlknee",
			"rlhindleg",
			"llhindleg",
			"rrhock",
			"lrhock"
		];

		public IEnumerable<(string Id, string Question,
			Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
			Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
			new List<(string Id, string Question,
				Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
				Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
			{
				("techlevel",
					@"What should be the rough equivalent tech level of the surgeries, drugs and the like that you would like to install?

	#Bprimitive#0 - no replantation, transplantation, resection or implants
	#Bpre-modern#0 - no replantation, transplantation or implants
	#Bmodern#0 - all surgical procedures

Please answer #3primitive#F, #3pre-modern#0, or #3modern#F: ",
					(context, answers) => true,
					(answer, context) =>
					{
						return NormaliseTechLevel(answer) switch
						{
							"primitive" or "pre-modern" or "modern" => (true, string.Empty),
							_ => (false, "Please answer #3primitive#F, #3pre-modern#0, or #3modern#F.")
						};
					})
			};

		private readonly Dictionary<string, Tag> _tags = new(StringComparer.OrdinalIgnoreCase);
		private readonly Dictionary<string, MudSharp.Models.Knowledge> _knowledges = new(StringComparer.OrdinalIgnoreCase);
		private readonly Dictionary<string, MudSharp.Models.SurgicalProcedure> _procedures =
			new(StringComparer.OrdinalIgnoreCase);

		private FuturemudDatabaseContext _context = null!;
		private IReadOnlyDictionary<string, string> _questionAnswers = null!;
		private BodyProto _humanBody = null!;
		private BodyProto? _quadrupedBody;
		private MudSharp.Models.FutureProg? _alwaysTrueProg;

		private MudSharp.Models.FutureProg AlwaysTrueProg
		{
			get
			{
				_alwaysTrueProg ??= _context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");
				return _alwaysTrueProg;
			}
		}

		public int SortOrder => 250;
		public string Name => "Health Seeder";
		public string Tagline => "Sets up surgeries, drugs, and medical starter content";
		public string FullDescription =>
			"Installs a release-ready medical starter set for the selected tech level, including human surgery, a broader drug catalogue, and basic veterinary procedures for stock mammal bodies when those bodies are available.";
		public bool Enabled => true;

		public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
		{
			_context = context;
			_questionAnswers = questionAnswers;
			_humanBody = _context.BodyProtos.First(x => x.Name == "Organic Humanoid");
			_quadrupedBody = _context.BodyProtos.FirstOrDefault(x => x.Name == "Quadruped Base");

			_tags.Clear();
			foreach (var tag in _context.Tags.ToArray())
			{
				_tags[tag.Name] = tag;
			}

			context.Database.BeginTransaction();
			SeedKnowledges();
			SeedSurgery();
			SeedDrugs();
			SeedDrugDeliveryExamples();
			context.SaveChanges();
			context.Database.CommitTransaction();

			return "Successfully set up Health Modules.";
		}

		public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
		{
			if (!context.Accounts.Any())
			{
				return ShouldSeedResult.PrerequisitesNotMet;
			}

			if (!context.Races.Any(x => x.Name == "Organic Humanoid"))
			{
				return ShouldSeedResult.PrerequisitesNotMet;
			}

			if (RequiredToolTags.Any(tag => !context.Tags.Any(x => x.Name == tag)))
			{
				return ShouldSeedResult.PrerequisitesNotMet;
			}

			return SeederRepeatabilityHelper.ClassifyByPresence(
			[
				.. StockHealthKnowledges.Select(name => context.Knowledges.Any(x => x.Name == name)),
				.. StockHealthProcedures.Select(name => context.SurgicalProcedures.Any(x => x.Name == name)),
				.. StockHealthDrugs.Select(name => context.Drugs.Any(x => x.Name == name)),
				.. ExpectedStockDrugDeliveryMarkers(context)
					.Select(name => context.GameItemComponentProtos.Any(x => x.Name == name))
			]);
		}

		internal void SeedDrugDeliveryExamplesForTesting(FuturemudDatabaseContext context)
		{
			_context = context;
			SeedDrugDeliveryExamples();
			_context.SaveChanges();
		}

		private static string NormaliseTechLevel(string answer)
		{
			return answer.Trim().ToLowerInvariant() switch
			{
				"primitive" => "primitive",
				"pre-modern" => "pre-modern",
				"premodern" => "pre-modern",
				"pre modern" => "pre-modern",
				"modern" => "modern",
				_ => string.Empty
			};
		}

		private string SelectedTechLevel => NormaliseTechLevel(_questionAnswers["techlevel"]);

		private void AddKnowledge(string name, string type, string subType, int sessions, Difficulty difficulty,
			string description, string longDescription)
		{
			var knowledge = SeederRepeatabilityHelper.EnsureNamedEntity(
				_context.Knowledges,
				name,
				x => x.Name,
				() =>
				{
					var created = new MudSharp.Models.Knowledge();
					_context.Knowledges.Add(created);
					return created;
				});

			knowledge.Name = name;
			knowledge.Type = type;
			knowledge.Subtype = subType;
			knowledge.Description = description;
			knowledge.LongDescription = longDescription;
			knowledge.LearnableType = (int)(LearnableType.LearnableFromTeacher | LearnableType.LearnableAtChargen);
			knowledge.LearnDifficulty = (int)difficulty;
			knowledge.TeachDifficulty = (int)difficulty;
			knowledge.LearningSessionsRequired = sessions;
			knowledge.CanAcquireProg = AlwaysTrueProg;
			knowledge.CanLearnProg = AlwaysTrueProg;
			_knowledges[name] = knowledge;
		}

		private void SeedKnowledges()
		{
			switch (SelectedTechLevel)
			{
				case "primitive":
					AddKnowledge(
						"Medicine",
						"Medicine",
						"Human",
						15,
						Difficulty.Hard,
						"Knowledge of treating the sick and injured",
						"This knowledge covers anatomy by tradition, injury care, and the rough hands-on procedures used to keep the badly hurt alive.");
					AddOptionalVeterinaryKnowledge(
						"Animal Medicine",
						15,
						Difficulty.Hard,
						"Knowledge of treating domesticated and wild mammals",
						"This knowledge covers the fieldcraft and practical treatment techniques used to diagnose and stabilise injured mammals.");
					break;

				case "pre-modern":
					AddKnowledge(
						"Chiurgery",
						"Medicine",
						"Human",
						15,
						Difficulty.Hard,
						"Knowledge of performing surgery and major interventions on humans",
						"Chiurgery is the practical craft of opening, repairing, removing, and closing injured portions of the body.");
					AddKnowledge(
						"Physical Medicine",
						"Medicine",
						"Human",
						20,
						Difficulty.VeryHard,
						"Knowledge of anatomy, diagnosis, and practical medicine for humans",
						"Physical Medicine is the academic understanding of anatomy, physiology, diagnosis, and the preparation that supports surgery.");
					AddOptionalVeterinaryKnowledge(
						"Veterinary Medicine",
						18,
						Difficulty.Hard,
						"Knowledge of diagnosing and treating mammalian patients",
						"This knowledge covers practical veterinary assessment, bandaging, fracture care, and stabilisation for mammals.");
					AddOptionalVeterinaryKnowledge(
						"Veterinary Chiurgery",
						18,
						Difficulty.VeryHard,
						"Knowledge of surgical interventions on mammalian patients",
						"This knowledge covers the surgical craft required to open, repair, or remove damaged tissue in mammalian patients.");
					break;

				case "modern":
					AddKnowledge(
						"Diagnostic Medicine",
						"Medicine",
						"Human",
						20,
						Difficulty.Hard,
						"Knowledge of diagnosing illness and trauma in humans",
						"This knowledge covers the signs, symptoms, and assessments used to diagnose disease and injury in human patients.");
					AddKnowledge(
						"Clinical Medicine",
						"Medicine",
						"Human",
						10,
						Difficulty.Normal,
						"Knowledge of routine clinical procedures used in human medicine",
						"This knowledge covers bedside practice, patient handling, and the common procedures used in clinical environments.");
					AddKnowledge(
						"Surgery",
						"Medicine",
						"Human",
						15,
						Difficulty.Hard,
						"Knowledge of surgical procedures and techniques",
						"This knowledge covers the operative procedures used to expose, repair, remove, replace, and reconstruct parts of the human body.");
					AddOptionalVeterinaryKnowledge(
						"Veterinary Medicine",
						15,
						Difficulty.Hard,
						"Knowledge of modern veterinary diagnosis and care for mammals",
						"This knowledge covers diagnostic work, clinical care, and stabilisation for common mammalian veterinary patients.");
					AddOptionalVeterinaryKnowledge(
						"Veterinary Surgery",
						18,
						Difficulty.Hard,
						"Knowledge of surgical procedures and techniques for mammals",
						"This knowledge covers operative veterinary work for mammalian patients, including trauma care and limb surgery.");
					break;
			}

			_context.SaveChanges();
		}

		private void AddOptionalVeterinaryKnowledge(string name, int sessions, Difficulty difficulty, string description,
			string longDescription)
		{
			if (_quadrupedBody is null)
			{
				return;
			}

			AddKnowledge(name, "Medicine", "Animal", sessions, difficulty, description, longDescription);
		}

		private void SeedSurgery()
		{
			switch (SelectedTechLevel)
			{
				case "primitive":
					SeedPrimitiveHumanSurgery();
					SeedPrimitiveVeterinarySurgery();
					break;
				case "pre-modern":
					SeedPreModernHumanSurgery();
					SeedPreModernVeterinarySurgery();
					break;
				case "modern":
					SeedModernHumanSurgery();
					SeedModernVeterinarySurgery();
					break;
			}
		}

		private void SeedPrimitiveHumanSurgery()
		{
			AddHastyTriage("Medicine", "Human Medicine", _humanBody, -3.0);
			AddPrimitivePhysical("Medicine", "Human Medicine", _humanBody, -5.0);
			AddPrimitiveStitching("Primitive Stitching", "primitive stitching", "Medicine", "Human Medicine", _humanBody,
				-3.0, "A rough closure procedure that uses crude needles and bindings to close an opened wound.");
			AddPrimitiveExploratory("Exploratory Surgery", "exploratory", "Medicine", "Human Medicine", _humanBody, -5.0);
			AddAmputation("Arm Amputation", "arm amputation", "Medicine", "Human Medicine", _humanBody, -5.0,
				"The amputation procedure removes an arm or major section of an arm from the patient.", HumanArmParts);
			AddAmputation("Leg Amputation", "leg amputation", "Medicine", "Human Medicine", _humanBody, -5.0,
				"The amputation procedure removes a leg or major section of a leg from the patient.", HumanLegParts);
			AddAmputation("Digit Amputation", "digit amputation", "Medicine", "Human Medicine", _humanBody, -4.0,
				"This procedure removes ruined fingers or toes that can no longer be saved.", HumanDigitParts);
			AddPrimitiveTraumaControl("Trauma Control", "trauma control", "Medicine", "Human Medicine", _humanBody, -5.0);
			AddPrimitiveOrganExtraction("Organ Extraction", "organ extraction", "Medicine", "Human Medicine", _humanBody,
				-5.0);
			AddPrimitiveOrganRepair("Crude Organ Repair", "organ repair", "Medicine", "Human Medicine", _humanBody, -5.0,
				"A crude attempt to pack, trim, and secure damaged internal organs.", "<Surgery requireunconcious='true'/>");
			AddPrimitiveOrganRepair("Trepanation", "trepanation", "Medicine", "Human Medicine", _humanBody, -3.5,
				"A specialised primitive cranial procedure used to relieve pressure and work on the brain.",
				GetDefinitionForTargets(_humanBody, HumanBrainTargets));
			AddPrimitiveOrganRepair("Windpipe Repair", "windpipe repair", "Medicine", "Human Medicine", _humanBody, -3.5,
				"A specialised primitive procedure for repairing the airway and trachea.",
				GetDefinitionForTargets(_humanBody, HumanTracheaTargets));
			AddBoneSetting("Bone Setting", "bone setting", "Medicine", "Human Medicine", _humanBody, -4.0,
				"A hands-on surgical setting procedure for badly displaced fractures.");
		}

		private void SeedPreModernHumanSurgery()
		{
			AddHastyTriage("Physical Medicine", "Human Medicine", _humanBody, -2.0);
			AddTriage("Triage", "triage", "Physical Medicine", "Human Medicine", _humanBody, -0.5,
				"A fuller examination of urgent injuries using practical tools and observation.");
			AddPreModernPhysical("Crude Physical", "crude physical", "Physical Medicine", "Human Medicine", _humanBody,
				-2.5, false);
			AddPreModernStitching("Stitch Up", "stitch up", "Chiurgery", "Human Medicine", _humanBody, -1.5,
				"A surgical closure procedure that restores a clean incision to a closed wound.");
			AddPreModernExploratory("Exploratory Surgery", "exploratory", "Chiurgery", "Human Medicine", _humanBody,
				-1.5);
			AddAmputation("Arm Amputation", "arm amputation", "Chiurgery", "Human Medicine", _humanBody, -1.5,
				"The amputation procedure removes an arm or major section of an arm from the patient.", HumanArmParts);
			AddAmputation("Leg Amputation", "leg amputation", "Chiurgery", "Human Medicine", _humanBody, -1.5,
				"The amputation procedure removes a leg or major section of a leg from the patient.", HumanLegParts);
			AddAmputation("Digit Amputation", "digit amputation", "Chiurgery", "Human Medicine", _humanBody, -1.0,
				"This procedure removes ruined fingers or toes that can no longer be saved.", HumanDigitParts);
			AddPreModernTraumaControl("Trauma Control", "trauma control", "Chiurgery", "Human Medicine", _humanBody, -1.5);
			AddPreModernOrganExtraction("Organ Extraction", "organ extraction", "Chiurgery", "Human Medicine", _humanBody,
				-1.5);
			AddPreModernOrganRepair("General Organ Repair", "organ repair", "Chiurgery", "Human Medicine", _humanBody,
				-1.5, "A general pre-modern procedure for repairing or excising damaged internal tissue.", string.Empty);
			AddHumanSpecificOrganRepairs("Chiurgery", "Human Medicine", _humanBody, -0.5, "Trepanation",
				GetDefinitionForTargets(_humanBody, HumanBrainTargets));
			AddHumanSpecificOrganRepairs("Chiurgery", "Human Medicine", _humanBody, -0.25, "Cardiac Repair",
				GetDefinitionForTargets(_humanBody, HumanHeartTargets));
			AddHumanSpecificOrganRepairs("Chiurgery", "Human Medicine", _humanBody, -0.25, "Liver Resection",
				GetDefinitionForTargets(_humanBody, HumanLiverTargets));
			AddHumanSpecificOrganRepairs("Chiurgery", "Human Medicine", _humanBody, -0.25, "Splenic Repair",
				GetDefinitionForTargets(_humanBody, HumanSpleenTargets));
			AddHumanSpecificOrganRepairs("Chiurgery", "Human Medicine", _humanBody, -0.25, "Gastric Repair",
				GetDefinitionForTargets(_humanBody, HumanStomachTargets));
			AddHumanSpecificOrganRepairs("Chiurgery", "Human Medicine", _humanBody, -0.25, "Intestinal Resection",
				GetDefinitionForTargets(_humanBody, HumanIntestineTargets));
			AddHumanSpecificOrganRepairs("Chiurgery", "Human Medicine", _humanBody, -0.25, "Kidney Repair",
				GetDefinitionForTargets(_humanBody, HumanKidneyTargets));
			AddHumanSpecificOrganRepairs("Chiurgery", "Human Medicine", _humanBody, -0.25, "Lung Resection",
				GetDefinitionForTargets(_humanBody, HumanLungTargets));
			AddHumanSpecificOrganRepairs("Chiurgery", "Human Medicine", _humanBody, -0.25, "Tracheal Repair",
				GetDefinitionForTargets(_humanBody, HumanTracheaTargets));
			AddHumanSpecificOrganRepairs("Chiurgery", "Human Medicine", _humanBody, -0.25, "Esophageal Repair",
				GetDefinitionForTargets(_humanBody, HumanEsophagusTargets));
			AddHumanSpecificOrganRepairs("Chiurgery", "Human Medicine", _humanBody, -0.25, "Spinal Stabilisation",
				GetDefinitionForTargets(_humanBody, HumanSpinalTargets));
			AddHumanSpecificOrganRepairs("Chiurgery", "Human Medicine", _humanBody, -0.25, "Inner Ear Repair",
				GetDefinitionForTargets(_humanBody, HumanInnerEarTargets));
			AddBoneSetting("Bone Setting", "bone setting", "Chiurgery", "Human Medicine", _humanBody, -1.0,
				"A surgical fracture-setting procedure with splints and better alignment techniques.");
		}

		private void SeedModernHumanSurgery()
		{
			AddHastyTriage("Diagnostic Medicine", "Human Medicine", _humanBody, -1.0);
			AddTriage("Triage", "triage", "Diagnostic Medicine", "Human Medicine", _humanBody, 0.5,
				"A complete triage procedure for urgent patients that combines clinical observation with practical checks.");
			AddModernPhysical("Crude Physical", "crude physical", "Diagnostic Medicine", "Human Medicine", _humanBody,
				-1.0, false);
			AddModernPhysical("Physical", "physical", "Diagnostic Medicine", "Human Medicine", _humanBody, 2.0, true);
			AddModernStitching("Stitch Up", "stitch up", "Surgery", "Human Medicine", _humanBody, 0.0,
				"A surgical closure and clean-up procedure used to finalise invasive operations.");
			AddModernExploratory("Exploratory Surgery", "exploratory", "Surgery", "Human Medicine", _humanBody, 0.0);
			AddAmputation("Arm Amputation", "arm amputation", "Surgery", "Human Medicine", _humanBody, 0.0,
				"The amputation procedure removes an arm or major section of an arm from the patient.", HumanArmParts);
			AddAmputation("Leg Amputation", "leg amputation", "Surgery", "Human Medicine", _humanBody, 0.0,
				"The amputation procedure removes a leg or major section of a leg from the patient.", HumanLegParts);
			AddAmputation("Digit Amputation", "digit amputation", "Surgery", "Human Medicine", _humanBody, 0.5,
				"This procedure removes ruined fingers or toes that can no longer be saved.", HumanDigitParts);
			AddModernReplantation();
			AddModernCannulation();
			AddModernDecannulation();
			AddModernTraumaControl("Trauma Control", "trauma control", "Surgery", "Human Medicine", _humanBody, 0.25);
			AddModernOrganExtraction("Organ Extraction", "organ extraction", "Surgery", "Human Medicine", _humanBody,
				0.25);
			AddModernOrganTransplant();
			AddModernOrganRepair("General Organ Repair", "organ repair", "Surgery", "Human Medicine", _humanBody, 0.25,
				"A general surgical repair or resection procedure for damaged internal organs.", string.Empty);
			AddHumanSpecificOrganRepairs("Surgery", "Human Medicine", _humanBody, 0.75, "Brain Surgery",
				GetDefinitionForTargets(_humanBody, HumanBrainTargets));
			AddHumanSpecificOrganRepairs("Surgery", "Human Medicine", _humanBody, 0.5, "Cardiac Repair",
				GetDefinitionForTargets(_humanBody, HumanHeartTargets));
			AddHumanSpecificOrganRepairs("Surgery", "Human Medicine", _humanBody, 0.5, "Liver Resection",
				GetDefinitionForTargets(_humanBody, HumanLiverTargets));
			AddHumanSpecificOrganRepairs("Surgery", "Human Medicine", _humanBody, 0.5, "Splenic Repair",
				GetDefinitionForTargets(_humanBody, HumanSpleenTargets));
			AddHumanSpecificOrganRepairs("Surgery", "Human Medicine", _humanBody, 0.5, "Gastric Repair",
				GetDefinitionForTargets(_humanBody, HumanStomachTargets));
			AddHumanSpecificOrganRepairs("Surgery", "Human Medicine", _humanBody, 0.5, "Intestinal Resection",
				GetDefinitionForTargets(_humanBody, HumanIntestineTargets));
			AddHumanSpecificOrganRepairs("Surgery", "Human Medicine", _humanBody, 0.5, "Kidney Repair",
				GetDefinitionForTargets(_humanBody, HumanKidneyTargets));
			AddHumanSpecificOrganRepairs("Surgery", "Human Medicine", _humanBody, 0.5, "Lung Resection",
				GetDefinitionForTargets(_humanBody, HumanLungTargets));
			AddHumanSpecificOrganRepairs("Surgery", "Human Medicine", _humanBody, 0.5, "Tracheal Repair",
				GetDefinitionForTargets(_humanBody, HumanTracheaTargets));
			AddHumanSpecificOrganRepairs("Surgery", "Human Medicine", _humanBody, 0.5, "Esophageal Repair",
				GetDefinitionForTargets(_humanBody, HumanEsophagusTargets));
			AddHumanSpecificOrganRepairs("Surgery", "Human Medicine", _humanBody, 0.5, "Spinal Stabilisation",
				GetDefinitionForTargets(_humanBody, HumanSpinalTargets));
			AddHumanSpecificOrganRepairs("Surgery", "Human Medicine", _humanBody, 0.5, "Inner Ear Repair",
				GetDefinitionForTargets(_humanBody, HumanInnerEarTargets));
			AddBoneSetting("Bone Setting", "bone setting", "Surgery", "Human Medicine", _humanBody, 0.5,
				"A modern surgical fracture-setting procedure.");
			AddModernImplantProcedures();
		}

		private void SeedPrimitiveVeterinarySurgery()
		{
			if (_quadrupedBody is null || !_knowledges.ContainsKey("Animal Medicine"))
			{
				return;
			}

			AddHastyTriage("Animal Medicine", "Veterinary Medicine", _quadrupedBody, -3.5, "Veterinary Hasty Triage",
				"veterinary hasty triage");
			AddPrimitivePhysical("Animal Medicine", "Veterinary Medicine", _quadrupedBody, -5.0,
				"Veterinary Crude Physical", "veterinary crude physical");
			AddPrimitiveStitching("Veterinary Stitching", "veterinary stitching", "Animal Medicine", "Veterinary Medicine",
				_quadrupedBody, -3.5, "A crude closure procedure for mammalian veterinary patients.");
			AddPrimitiveExploratory("Veterinary Exploratory Surgery", "veterinary exploratory", "Animal Medicine",
				"Veterinary Medicine", _quadrupedBody, -5.0);
			AddPrimitiveTraumaControl("Veterinary Trauma Control", "veterinary trauma control", "Animal Medicine",
				"Veterinary Medicine", _quadrupedBody, -5.0);
			AddBoneSetting("Veterinary Bone Setting", "veterinary bone setting", "Animal Medicine",
				"Veterinary Medicine", _quadrupedBody, -4.0,
				"A primitive fracture-setting procedure for mammalian veterinary patients.");
			AddAmputation("Foreleg Amputation", "foreleg amputation", "Animal Medicine", "Veterinary Medicine",
				_quadrupedBody, -5.0, "This procedure removes a badly ruined foreleg from a mammalian patient.",
				QuadrupedForelegParts);
			AddAmputation("Hindleg Amputation", "hindleg amputation", "Animal Medicine", "Veterinary Medicine",
				_quadrupedBody, -5.0, "This procedure removes a badly ruined hindleg from a mammalian patient.",
				QuadrupedHindlegParts);
		}

		private void SeedPreModernVeterinarySurgery()
		{
			if (_quadrupedBody is null || !_knowledges.ContainsKey("Veterinary Chiurgery"))
			{
				return;
			}

			AddHastyTriage("Veterinary Medicine", "Veterinary Medicine", _quadrupedBody, -2.5, "Veterinary Hasty Triage",
				"veterinary hasty triage");
			AddTriage("Veterinary Triage", "veterinary triage", "Veterinary Medicine", "Veterinary Medicine",
				_quadrupedBody, -0.5, "A fuller examination of an urgent mammalian patient.");
			AddPreModernPhysical("Veterinary Physical", "veterinary physical", "Veterinary Medicine", "Veterinary Medicine",
				_quadrupedBody, -2.0, false);
			AddPreModernStitching("Veterinary Stitch Up", "veterinary stitch up", "Veterinary Chiurgery",
				"Veterinary Medicine", _quadrupedBody, -1.5,
				"A clean closure procedure for mammalian veterinary surgery.");
			AddPreModernExploratory("Veterinary Exploratory Surgery", "veterinary exploratory", "Veterinary Chiurgery",
				"Veterinary Medicine", _quadrupedBody, -1.5);
			AddPreModernTraumaControl("Veterinary Trauma Control", "veterinary trauma control", "Veterinary Chiurgery",
				"Veterinary Medicine", _quadrupedBody, -1.5);
			AddBoneSetting("Veterinary Bone Setting", "veterinary bone setting", "Veterinary Chiurgery",
				"Veterinary Medicine", _quadrupedBody, -1.0,
				"A surgical fracture-setting procedure for mammalian veterinary patients.");
			AddAmputation("Foreleg Amputation", "foreleg amputation", "Veterinary Chiurgery", "Veterinary Medicine",
				_quadrupedBody, -1.5, "This procedure removes a badly ruined foreleg from a mammalian patient.",
				QuadrupedForelegParts);
			AddAmputation("Hindleg Amputation", "hindleg amputation", "Veterinary Chiurgery", "Veterinary Medicine",
				_quadrupedBody, -1.5, "This procedure removes a badly ruined hindleg from a mammalian patient.",
				QuadrupedHindlegParts);
		}

		private void SeedModernVeterinarySurgery()
		{
			if (_quadrupedBody is null || !_knowledges.ContainsKey("Veterinary Surgery"))
			{
				return;
			}

			AddHastyTriage("Veterinary Medicine", "Veterinary Medicine", _quadrupedBody, -1.5, "Veterinary Hasty Triage",
				"veterinary hasty triage");
			AddTriage("Veterinary Triage", "veterinary triage", "Veterinary Medicine", "Veterinary Medicine",
				_quadrupedBody, 0.0, "A complete triage procedure for mammalian veterinary patients.");
			AddModernPhysical("Veterinary Physical", "veterinary physical", "Veterinary Medicine", "Veterinary Medicine",
				_quadrupedBody, 1.0, true);
			AddModernStitching("Veterinary Stitch Up", "veterinary stitch up", "Veterinary Surgery",
				"Veterinary Medicine", _quadrupedBody, 0.0,
				"A modern closure procedure for mammalian veterinary surgery.");
			AddModernExploratory("Veterinary Exploratory Surgery", "veterinary exploratory", "Veterinary Surgery",
				"Veterinary Medicine", _quadrupedBody, 0.0);
			AddModernTraumaControl("Veterinary Trauma Control", "veterinary trauma control", "Veterinary Surgery",
				"Veterinary Medicine", _quadrupedBody, 0.25);
			AddBoneSetting("Veterinary Bone Setting", "veterinary bone setting", "Veterinary Surgery",
				"Veterinary Medicine", _quadrupedBody, 0.25,
				"A modern fracture-setting procedure for mammalian veterinary patients.");
			AddAmputation("Foreleg Amputation", "foreleg amputation", "Veterinary Surgery", "Veterinary Medicine",
				_quadrupedBody, 0.0, "This procedure removes a badly ruined foreleg from a mammalian patient.",
				QuadrupedForelegParts);
			AddAmputation("Hindleg Amputation", "hindleg amputation", "Veterinary Surgery", "Veterinary Medicine",
				_quadrupedBody, 0.0, "This procedure removes a badly ruined hindleg from a mammalian patient.",
				QuadrupedHindlegParts);
		}

		private void AddHastyTriage(string knowledgeName, string school, BodyProto targetBody, double baseCheckBonus,
			string name = "Hasty Triage", string procedureName = "hasty triage")
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.Triage,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.TriageCheck,
				"triaging",
				"@ begin|begins triaging $1=0",
				"This procedure uses rapid observation and touch to estimate the severity of a patient's injuries and stability.",
				string.Empty,
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 10, null,
				"@ observe|observes $1, noting obvious bleeding, bruising, and distress");
			AddSurgicalProcedurePhase(_procedures[name], 2, 10, null,
				"@ lightly prod|prods $1's vital areas, watching for a reaction");
			AddSurgicalProcedurePhase(_procedures[name], 3, 10, null,
				"@ check|checks $1's breathing and pulse before making a quick judgement");
		}

		private void AddTriage(string name, string procedureName, string knowledgeName, string school, BodyProto targetBody,
			double baseCheckBonus, string description)
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.Triage,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.TriageCheck,
				"triaging",
				"@ begin|begins triaging $1=0",
				description,
				string.Empty,
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 15, null,
				"@ observe|observes $1, noting obvious bleeding, bruising, and posture");
			AddSurgicalProcedurePhase(_procedures[name], 2, 20, null,
				"@ take|takes $1's pulse and count|counts breaths with practiced focus");
			AddSurgicalProcedurePhase(_procedures[name], 3, 20, null,
				"@ test|tests several joints and major body areas for pain and instability");
			AddSurgicalProcedurePhase(_procedures[name], 4, 20, null,
				"@ listen|listens closely to $1's breathing and chest sounds");
			AddSurgicalProcedurePhase(_procedures[name], 5, 10, null,
				"@ finish|finishes the triage and forms a clearer picture of $1's condition");
		}

		private void AddPrimitivePhysical(string knowledgeName, string school, BodyProto targetBody, double baseCheckBonus,
			string name = "Crude Physical", string procedureName = "crude physical")
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.DetailedExamination,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.MedicalExaminationCheck,
				"examining",
				"@ begin|begins examining $1=0",
				"This procedure performs a slow physical examination using touch, posture, and close observation.",
				string.Empty,
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 25, null,
				"@ circle|circles $1, studying posture, stance, and obvious injuries");
			AddSurgicalProcedurePhase(_procedures[name], 2, 25, null,
				"@ press|presses along $1's limbs and belly, noting any pain or resistance");
			AddSurgicalProcedurePhase(_procedures[name], 3, 25, null,
				"@ count|counts $1's pulse with patient fingers");
			AddSurgicalProcedurePhase(_procedures[name], 4, 25, null,
				"@ lean|leans close to listen to $1's breathing");
			AddSurgicalProcedurePhase(_procedures[name], 5, 25, null,
				"@ step|steps back from $1, finishing the examination");
		}

		private void AddPreModernPhysical(string name, string procedureName, string knowledgeName, string school,
			BodyProto targetBody, double baseCheckBonus, bool useScale)
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.DetailedExamination,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.MedicalExaminationCheck,
				"examining",
				"@ begin|begins examining $1=0",
				"This procedure performs a full physical examination using simple clinical tools and direct observation.",
				string.Empty,
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 25, null,
				"@ measure|measures $1 and takes note of overall build");
			AddSurgicalProcedurePhase(_procedures[name], 2, 25, null,
				useScale
					? "@ weigh|weighs $1 and notes the result with practiced care"
					: "@ estimate|estimates $1's weight and general condition");
			AddSurgicalProcedurePhase(_procedures[name], 3, 25, null,
				"@ listen|listens to $1's breathing and heartbeat");
			AddSurgicalProcedurePhase(_procedures[name], 4, 25, null,
				"@ test|tests $1's reflexes and limb motion");
			AddSurgicalProcedurePhase(_procedures[name], 5, 25, null,
				"@ finish|finishes the examination of $1");
		}

		private void AddModernPhysical(string name, string procedureName, string knowledgeName, string school,
			BodyProto targetBody, double baseCheckBonus, bool useClinicalTools)
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.DetailedExamination,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.MedicalExaminationCheck,
				"examining",
				"@ begin|begins examining $1=0",
				"This procedure performs a full physical examination, ranging from bedside assessment to a proper instrumented check-up.",
				string.Empty,
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 25, null,
				useClinicalTools
					? "@ measure|measures $1's weight, build, and stance using routine clinical methods"
					: "@ visually assess|assesses $1's build, posture, and movement");
			AddSurgicalProcedurePhase(_procedures[name], 2, 25, null,
				useClinicalTools
					? "@ take|takes $1's vital signs in a routine clinical pass"
					: "@ count|counts $1's pulse and breathing with steady attention");
			AddSurgicalProcedurePhase(_procedures[name], 3, 25, null,
				useClinicalTools
					? "@ listen|listens to $1's chest and breathing in a routine clinical pass"
					: "@ listen|listens to $1's breathing without instruments");
			AddSurgicalProcedurePhase(_procedures[name], 4, 25, null,
				useClinicalTools
					? "@ test|tests $1's reflexes and limb response in a routine clinical pass"
					: "@ test|tests $1's reflexes and range of movement manually");
			AddSurgicalProcedurePhase(_procedures[name], 5, 25, null,
				"@ step|steps back from $1, having completed the examination");
		}

		private void AddPrimitiveStitching(string name, string procedureName, string knowledgeName, string school,
			BodyProto targetBody, double baseCheckBonus, string description)
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.InvasiveProcedureFinalisation,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.InvasiveProcedureFinalisation,
				"stitching",
				"@ begin|begins stitching up $1",
				description,
				string.Empty,
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 30, "bleeding 0.05 0.03 0.02 0.01 0.005 0",
				"@ push|pushes everything back into $1's {0}");
			AddSurgicalProcedurePhase(_procedures[name], 2, 30, null,
				"@ stitch|stitches the wound closed with rough, careful motions", SutureNeedlePlan());
			AddSurgicalProcedurePhase(_procedures[name], 3, 25, null,
				"@ bind|binds the closed wound tightly and checks that it will hold");
		}

		private void AddPreModernStitching(string name, string procedureName, string knowledgeName, string school,
			BodyProto targetBody, double baseCheckBonus, string description)
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.InvasiveProcedureFinalisation,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.InvasiveProcedureFinalisation,
				"stitching",
				"@ begin|begins stitching up $1",
				description,
				string.Empty,
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 25, "bleeding 0.04 0.025 0.015 0.008 0.004 0",
				"@ arrange|arranges tissue and organs back into $1's {0}");
			AddSurgicalProcedurePhase(_procedures[name], 2, 25, null,
				"@ tie|ties off larger vessels and bleeding points with practiced care", ClampPlan());
			AddSurgicalProcedurePhase(_procedures[name], 3, 30, null,
				"@ stitch|stitches the wound closed with $i1", SutureNeedlePlan());
		}

		private void AddModernStitching(string name, string procedureName, string knowledgeName, string school,
			BodyProto targetBody, double baseCheckBonus, string description)
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.InvasiveProcedureFinalisation,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.InvasiveProcedureFinalisation,
				"stitching",
				"@ begin|begins stitching up $1",
				description,
				string.Empty,
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 25, null,
				"@ reposition|repositions tissue and organs within $1's {0}");
			AddSurgicalProcedurePhase(_procedures[name], 2, 25, null,
				"@ clamp|clamps and secures bleeding vessels with $i1", ClampPlan());
			AddSurgicalProcedurePhase(_procedures[name], 3, 25, null,
				"@ suture|sutures deeper layers closed with $i1", SutureNeedlePlan());
			AddSurgicalProcedurePhase(_procedures[name], 4, 25, null,
				"@ close|closes the surface wound with neat finishing stitches", SutureNeedlePlan());
		}

		private void AddPrimitiveExploratory(string name, string procedureName, string knowledgeName, string school,
			BodyProto targetBody, double baseCheckBonus)
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.ExploratorySurgery,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.ExploratorySurgeryCheck,
				"exploring",
				"@ begin|begins exploratory surgery on $1",
				"This procedure opens the patient to search for unseen internal trauma.",
				string.Empty,
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 40, "bleeding 0.08 0.05 0.03 0.02 0.01 0",
				"@ slice|slices into $1's {0} with $i1", ScalpelPlan());
			AddSurgicalProcedurePhase(_procedures[name], 2, 40, null,
				"@ probe|probes within the wound, searching for hidden damage");
			AddSurgicalProcedurePhase(_procedures[name], 3, 30, null,
				"@ withdraw|withdraws, having finished the exploration");
		}

		private void AddPreModernExploratory(string name, string procedureName, string knowledgeName, string school,
			BodyProto targetBody, double baseCheckBonus)
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.ExploratorySurgery,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.ExploratorySurgeryCheck,
				"exploring",
				"@ begin|begins exploratory surgery on $1",
				"This procedure opens the patient for a deliberate search for hidden internal trauma.",
				string.Empty,
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 35, "bleeding 0.06 0.04 0.025 0.015 0.01 0",
				"@ open|opens $1's {0} with $i1", ScalpelPlan());
			AddSurgicalProcedurePhase(_procedures[name], 2, 35, null,
				"@ part|parts tissue with $i1 and inspect|inspects within", ForcepsPlan());
			AddSurgicalProcedurePhase(_procedures[name], 3, 25, null,
				"@ pack|packs the wound and prepare|prepares to close the patient");
		}

		private void AddModernExploratory(string name, string procedureName, string knowledgeName, string school,
			BodyProto targetBody, double baseCheckBonus)
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.ExploratorySurgery,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.ExploratorySurgeryCheck,
				"exploring",
				"@ begin|begins exploratory surgery on $1",
				"This procedure opens the patient to search systematically for hidden injuries and internal damage.",
				string.Empty,
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 30, "exposed",
				"@ draw|draws a clean incision across $1's {0} with $i1", ScalpelPlan());
			AddSurgicalProcedurePhase(_procedures[name], 2, 35, null,
				"@ retract|retracts tissue with $i1 and inspect|inspects the cavity", ForcepsPlan());
			AddSurgicalProcedurePhase(_procedures[name], 3, 25, null,
				"@ pack|packs the wound and prepare|prepares the patient for closure");
		}

		private void AddPrimitiveTraumaControl(string name, string procedureName, string knowledgeName, string school,
			BodyProto targetBody, double baseCheckBonus)
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.TraumaControl,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.TraumaControlSurgery,
				"controlling trauma",
				"@ begin|begins crude trauma control on $1",
				"This procedure attempts to stop dangerous internal bleeding in a bodypart.",
				string.Empty,
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 35, "bleeding 0.06 0.04 0.03 0.015 0.01 0",
				"@ cut|cuts into $1's {0} with $i1", ScalpelPlan());
			AddSurgicalProcedurePhase(_procedures[name], 2, 35, null,
				"@ clamp|clamps and crushes bleeding vessels with $i1", ClampPlan());
			AddSurgicalProcedurePhase(_procedures[name], 3, 25, null,
				"@ bind|binds and dresses the area as best as possible");
		}

		private void AddPreModernTraumaControl(string name, string procedureName, string knowledgeName, string school,
			BodyProto targetBody, double baseCheckBonus)
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.TraumaControl,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.TraumaControlSurgery,
				"controlling trauma",
				"@ begin|begins trauma control on $1",
				"This procedure attempts to stop internal bleeding and secure damaged tissue in a bodypart.",
				string.Empty,
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 35, "bleeding 0.05 0.03 0.02 0.01 0.005 0",
				"@ open|opens $1's {0} with $i1", ScalpelPlan());
			AddSurgicalProcedurePhase(_procedures[name], 2, 35, null,
				"@ clamp|clamps bleeding vessels with $i1", ClampPlan());
			AddSurgicalProcedurePhase(_procedures[name], 3, 30, null,
				"@ secure|secures the wound for later closure");
		}

		private void AddModernTraumaControl(string name, string procedureName, string knowledgeName, string school,
			BodyProto targetBody, double baseCheckBonus)
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.TraumaControl,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.TraumaControlSurgery,
				"controlling trauma",
				"@ begin|begins trauma control on $1",
				"This procedure surgically stops dangerous bleeding and stabilises traumatic internal damage in a bodypart.",
				string.Empty,
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 30, "exposed",
				"@ open|opens $1's {0} with $i1", ScalpelPlan());
			AddSurgicalProcedurePhase(_procedures[name], 2, 30, null,
				"@ clamp|clamps and secures bleeding vessels with $i1", ClampPlan());
			AddSurgicalProcedurePhase(_procedures[name], 3, 25, null,
				"@ pack|packs the wound and ready|readies the site for closure");
		}

		private void AddPrimitiveOrganExtraction(string name, string procedureName, string knowledgeName, string school,
			BodyProto targetBody, double baseCheckBonus)
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.OrganExtraction,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.OrganExtractionCheck,
				"extracting",
				"@ begin|begins to perform an organ extraction on $1",
				"This procedure brutally cuts an organ free from the patient's body.",
				string.Empty,
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 40, "bleeding 0.08 0.05 0.04 0.02 0.01 0",
				"@ carve|carves into $1's {0} with $i1", ScalpelPlan());
			AddSurgicalProcedurePhase(_procedures[name], 2, 40, null,
				"@ tug|tugs the organ free with $i1", ForcepsPlan());
			AddSurgicalProcedurePhase(_procedures[name], 3, 25, null,
				"@ rip|rips the organ away from the remaining tissue");
		}

		private void AddPreModernOrganExtraction(string name, string procedureName, string knowledgeName, string school,
			BodyProto targetBody, double baseCheckBonus)
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.OrganExtraction,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.OrganExtractionCheck,
				"extracting",
				"@ begin|begins to perform an organ extraction on $1",
				"This procedure cuts an organ free from the patient's body.",
				string.Empty,
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 35, "checkorgan",
				"@ open|opens $1's {0} to reach the {1} with $i1", ScalpelPlan());
			AddSurgicalProcedurePhase(_procedures[name], 2, 35, null,
				"@ cut|cuts the {1} free with $i1", ForcepsPlan());
			AddSurgicalProcedurePhase(_procedures[name], 3, 25, null,
				"@ lift|lifts the {1} clear of the wound");
		}

		private void AddModernOrganExtraction(string name, string procedureName, string knowledgeName, string school,
			BodyProto targetBody, double baseCheckBonus)
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.OrganExtraction,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.OrganExtractionCheck,
				"extracting",
				"@ begin|begins to perform an organ extraction on $1",
				"This procedure cleanly removes a targeted organ from the patient's body.",
				string.Empty,
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 30, "checkorgan exposed",
				"@ make|makes a precise opening to reach the {1} in $1's {0} with $i1", ScalpelPlan());
			AddSurgicalProcedurePhase(_procedures[name], 2, 35, null,
				"@ free|frees the {1} from connective tissue with $i1", ForcepsPlan());
			AddSurgicalProcedurePhase(_procedures[name], 3, 25, null,
				"@ lift|lifts the {1} free of the body");
		}

		private void AddPrimitiveOrganRepair(string name, string procedureName, string knowledgeName, string school,
			BodyProto targetBody, double baseCheckBonus, string description, string definition)
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.OrganStabilisation,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.OrganStabilisationCheck,
				"repairing",
				"@ begin|begins to repair $1's injured organ",
				description,
				definition,
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 35, "checkorgan bleeding 0.05 0.03 0.02 0.01 0.005 0",
				"@ open|opens $1's {0} to reach the {1}");
			AddSurgicalProcedurePhase(_procedures[name], 2, 35, null,
				"@ pack|packs, trims, and secures the damaged {1}");
			AddSurgicalProcedurePhase(_procedures[name], 3, 25, null,
				"@ bind|binds and stabilises the wound as best as possible");
		}

		private void AddPreModernOrganRepair(string name, string procedureName, string knowledgeName, string school,
			BodyProto targetBody, double baseCheckBonus, string description, string definition)
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.OrganStabilisation,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.OrganStabilisationCheck,
				"repairing",
				"@ begin|begins to repair $1's injured organ",
				description,
				definition,
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 35, "checkorgan",
				"@ open|opens $1's {0} to reach the {1} with $i1", ScalpelPlan());
			AddSurgicalProcedurePhase(_procedures[name], 2, 35, null,
				"@ excise|excises and repairs damaged portions of the {1} with $i1", ForcepsPlan());
			AddSurgicalProcedurePhase(_procedures[name], 3, 30, null,
				"@ secure|secures the repaired tissue and prepares the wound for closure");
		}

		private void AddModernOrganRepair(string name, string procedureName, string knowledgeName, string school,
			BodyProto targetBody, double baseCheckBonus, string description, string definition)
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.OrganStabilisation,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.OrganStabilisationCheck,
				"repairing",
				"@ begin|begins to repair $1's injured organ",
				description,
				definition,
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 30, "checkorgan exposed",
				"@ open|opens $1's {0} to expose the {1} with $i1", ScalpelPlan());
			AddSurgicalProcedurePhase(_procedures[name], 2, 35, null,
				"@ repair|repairs and trims damaged portions of the {1} with $i1", ForcepsPlan());
			AddSurgicalProcedurePhase(_procedures[name], 3, 25, null,
				"@ secure|secures haemostasis and prepares the repaired organ for closure", ClampPlan());
		}

		private void AddHumanSpecificOrganRepairs(string knowledgeName, string school, BodyProto targetBody,
			double baseCheckBonus, string name, string definition)
		{
			var procedureName = name.ToLowerInvariant();
			if (SelectedTechLevel == "modern")
			{
				AddModernOrganRepair(
					name,
					procedureName,
					knowledgeName,
					school,
					targetBody,
					baseCheckBonus,
					$"This specialised surgical procedure focuses on {name.ToLowerInvariant()} with a better than generic outcome.",
					definition);
				return;
			}

			AddPreModernOrganRepair(
				name,
				procedureName,
				knowledgeName,
				school,
				targetBody,
				baseCheckBonus,
				$"This specialised surgical procedure focuses on {name.ToLowerInvariant()} with a better than generic outcome.",
				definition);
		}

		private void AddBoneSetting(string name, string procedureName, string knowledgeName, string school,
			BodyProto targetBody, double baseCheckBonus, string description)
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.SurgicalBoneSetting,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.SurgicalSetCheck,
				"setting",
				"@ begin|begins to set $1's bone",
				description,
				string.Empty,
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 25, "checkbone",
				"@ examine|examines $1's {0} to find the damaged {1}");
			AddSurgicalProcedurePhase(_procedures[name], 2, 30, null,
				"@ align|aligns the damaged {1} and works it back into place");
			AddSurgicalProcedurePhase(_procedures[name], 3, 20, null,
				"@ secure|secures the bone with splints and wrappings");
		}

		private void AddAmputation(string name, string procedureName, string knowledgeName, string school,
			BodyProto targetBody, double baseCheckBonus, string description, params string[] parts)
		{
			AddSurgicalProcedure(
				name,
				procedureName,
				school,
				SurgicalProcedureType.Amputation,
				baseCheckBonus,
				_knowledges[knowledgeName].Id,
				CheckType.AmputationCheck,
				"amputating",
				"@ begin|begins an amputation on $1",
				description,
				GetDefinitionForTargets(targetBody, parts),
				targetBody);
			AddSurgicalProcedurePhase(_procedures[name], 1, 35, "bleeding 0.10 0.06 0.04 0.02 0.01 0",
				"@ incise|incises around $1's {0} with $i1", ScalpelPlan());
			AddSurgicalProcedurePhase(_procedures[name], 2, 35, null,
				"@ saw|saws through the limb with $i1", BonesawPlan());
			AddSurgicalProcedurePhase(_procedures[name], 3, 20, null,
				"@ remove|removes the severed part");
		}

		private void AddModernReplantation()
		{
			AddSurgicalProcedure(
				"Replantation",
				"replantation",
				"Human Medicine",
				SurgicalProcedureType.Replantation,
				0.25,
				_knowledges["Surgery"].Id,
				CheckType.ReplantationCheck,
				"replanting",
				"@ begin|begins replantation on $1",
				"This procedure reattaches a severed bodypart to the patient.",
				string.Empty,
				_humanBody);
			AddSurgicalProcedurePhase(_procedures["Replantation"], 1, 30, "exposed",
				"@ clean|cleans and prepares the stump for {2}", ScalpelPlan());
			AddSurgicalProcedurePhase(_procedures["Replantation"], 2, 30, null,
				"@ align|aligns {2} to the patient and begins securing it");
			AddSurgicalProcedurePhase(_procedures["Replantation"], 3, 30, null,
				"@ anchor|anchors the replanted tissue with careful sutures", SutureNeedlePlan());
		}

		private void AddModernCannulation()
		{
			AddSurgicalProcedure(
				"Cannulation",
				"cannulation",
				"Human Medicine",
				SurgicalProcedureType.Cannulation,
				0.5,
				_knowledges["Clinical Medicine"].Id,
				CheckType.CannulationProcedure,
				"cannulating",
				"@ begin|begins a cannulation procedure on $1",
				"Cannulation installs a cannula into an appropriate bodypart for IV access.",
				string.Empty,
				_humanBody);
			AddSurgicalProcedurePhase(_procedures["Cannulation"], 1, 15, "exposed",
				"@ swab|swabs $1's {1} and positions {0}");
			AddSurgicalProcedurePhase(_procedures["Cannulation"], 2, 15, null,
				"@ slide|slides {0} into the vessel and secure|secures it in place");
		}

		private void AddModernDecannulation()
		{
			AddSurgicalProcedure(
				"Decannulation",
				"decannulation",
				"Human Medicine",
				SurgicalProcedureType.Decannulation,
				0.5,
				_knowledges["Clinical Medicine"].Id,
				CheckType.DecannulationProcedure,
				"decannulating",
				"@ begin|begins to decannulate $1",
				"This procedure removes an installed cannula from the patient.",
				string.Empty,
				_humanBody);
			AddSurgicalProcedurePhase(_procedures["Decannulation"], 1, 15, null,
				"@ gently withdraw|withdraws the cannula from $1's {0}");
		}

		private void AddModernOrganTransplant()
		{
			AddSurgicalProcedure(
				"Organ Transplant",
				"organ transplant",
				"Human Medicine",
				SurgicalProcedureType.OrganTransplant,
				0.25,
				_knowledges["Surgery"].Id,
				CheckType.OrganTransplantCheck,
				"transplanting",
				"@ begin|begins an organ transplant on $1",
				"This procedure implants a compatible replacement organ into a patient.",
				string.Empty,
				_humanBody);
			AddSurgicalProcedurePhase(_procedures["Organ Transplant"], 1, 30, "checkspace",
				"@ prepare|prepares $1's {0} to receive the new {1} with $i1", ScalpelPlan());
			AddSurgicalProcedurePhase(_procedures["Organ Transplant"], 2, 35, "checkorgan",
				"@ seat|seats the donor {1} and begin|begins securing it");
			AddSurgicalProcedurePhase(_procedures["Organ Transplant"], 3, 30, null,
				"@ connect|connects vessels and stabilises the transplant", ClampPlan());
		}

		private void AddModernImplantProcedures()
		{
			AddSurgicalProcedure(
				"Install Implant",
				"install implant",
				"Human Medicine",
				SurgicalProcedureType.InstallImplant,
				0.25,
				_knowledges["Surgery"].Id,
				CheckType.InstallImplantSurgery,
				"installing",
				"@ begin|begins to install an implant in $1",
				"This procedure installs an implant item into a compatible bodypart.",
				string.Empty,
				_humanBody);
			AddSurgicalProcedurePhase(_procedures["Install Implant"], 1, 30, "exposed checkspace",
				"@ open|opens $1's {1} to make room for {0}", ScalpelPlan());
			AddSurgicalProcedurePhase(_procedures["Install Implant"], 2, 30, null,
				"@ insert|inserts {0} and settle|settles it into position");
			AddSurgicalProcedurePhase(_procedures["Install Implant"], 3, 20, null,
				"@ secure|secures the implant site for closure");

			AddSurgicalProcedure(
				"Remove Implant",
				"remove implant",
				"Human Medicine",
				SurgicalProcedureType.RemoveImplant,
				0.25,
				_knowledges["Surgery"].Id,
				CheckType.RemoveImplantSurgery,
				"removing",
				"@ begin|begins to remove an implant from $1",
				"This procedure removes an implant item from a compatible bodypart.",
				string.Empty,
				_humanBody);
			AddSurgicalProcedurePhase(_procedures["Remove Implant"], 1, 30, "exposed",
				"@ open|opens $1's {1} to reach the implant", ScalpelPlan());
			AddSurgicalProcedurePhase(_procedures["Remove Implant"], 2, 30, null,
				"@ extract|extracts {0} from the surgical site");
			AddSurgicalProcedurePhase(_procedures["Remove Implant"], 3, 20, null,
				"@ secure|secures the site for closure");

			AddSurgicalProcedure(
				"Configure Implant Power",
				"configure implant power",
				"Human Medicine",
				SurgicalProcedureType.ConfigureImplantPower,
				0.25,
				_knowledges["Surgery"].Id,
				CheckType.ConfigureImplantPowerSurgery,
				"configuring",
				"@ begin|begins to configure implant power in $1",
				"This procedure adjusts the power settings of an installed implant.",
				string.Empty,
				_humanBody);
			AddSurgicalProcedurePhase(_procedures["Configure Implant Power"], 1, 20, null,
				"@ adjust|adjusts power settings on the exposed implant");

			AddSurgicalProcedure(
				"Configure Implant Interface",
				"configure implant interface",
				"Human Medicine",
				SurgicalProcedureType.ConfigureImplantInterface,
				0.25,
				_knowledges["Surgery"].Id,
				CheckType.ConfigureImplantInterfaceSurgery,
				"configuring",
				"@ begin|begins to configure implant interfaces in $1",
				"This procedure adjusts the interface settings of an installed implant.",
				string.Empty,
				_humanBody);
			AddSurgicalProcedurePhase(_procedures["Configure Implant Interface"], 1, 20, null,
				"@ tune|tunes the exposed implant's interface settings");
		}

		private void AddSurgicalProcedure(string name, string procedureName, string school, SurgicalProcedureType type,
			double baseCheckBonus, long knowledgeId, CheckType check, string gerund, string emote, string description,
			string definition, BodyProto targetBody)
		{
			var procedure = SeederRepeatabilityHelper.EnsureNamedEntity(
				_context.SurgicalProcedures,
				name,
				x => x.Name,
				() =>
				{
					var created = new MudSharp.Models.SurgicalProcedure();
					_context.SurgicalProcedures.Add(created);
					return created;
				});

			foreach (var phase in procedure.SurgicalProcedurePhases.ToList())
			{
				_context.SurgicalProcedurePhases.Remove(phase);
			}

			procedure.Name = name;
			procedure.ProcedureName = procedureName;
			procedure.Procedure = (int)type;
			procedure.MedicalSchool = school;
			procedure.BaseCheckBonus = baseCheckBonus;
			procedure.KnowledgeRequiredId = knowledgeId;
			procedure.ProcedureBeginEmote = emote;
			procedure.ProcedureGerund = gerund;
			procedure.ProcedureDescriptionEmote = description;
			procedure.Check = (int)check;
			procedure.Definition = definition;
			procedure.TargetBodyTypeId = targetBody.Id;
			_procedures[name] = procedure;
		}

		private void AddSurgicalProcedurePhase(MudSharp.Models.SurgicalProcedure procedure, int phaseNumber, double seconds,
			string? special, string emote, string? template = null)
		{
			procedure.SurgicalProcedurePhases.Add(new MudSharp.Models.SurgicalProcedurePhase
			{
				PhaseNumber = phaseNumber,
				PhaseEmote = emote,
				PhaseSpecialEffects = special,
				BaseLengthInSeconds = seconds,
				SurgicalProcedureId = procedure.Id,
				InventoryActionPlan = template
			});
		}

		private string ProduceInventoryPlanDefinition(params (InventoryState State, string Tag, int Quantity)[] actions)
		{
			var elements = new List<XElement>();
			foreach (var action in actions)
			{
				if (!_tags.TryGetValue(action.Tag, out var tag))
				{
					throw new InvalidOperationException(
						$"HealthSeeder requires the tag '{action.Tag}', but it has not been installed.");
				}

				elements.Add(action.State switch
				{
					InventoryState.Held => new XElement("Action",
						new XAttribute("state", "held"),
						new XAttribute("tag", tag.Id),
						new XAttribute("quantity", action.Quantity),
						new XAttribute("optionalquantity", false)),
					InventoryState.Wielded => new XElement("Action",
						new XAttribute("state", "wielded"),
						new XAttribute("tag", tag.Id),
						new XAttribute("wieldstate", (int)AttackHandednessOptions.Any)),
					InventoryState.Worn => new XElement("Action",
						new XAttribute("state", "worn"),
						new XAttribute("tag", tag.Id)),
					InventoryState.Dropped => new XElement("Action",
						new XAttribute("state", "dropped"),
						new XAttribute("tag", tag.Id)),
					InventoryState.Sheathed => new XElement("Action",
						new XAttribute("state", "sheathed"),
						new XAttribute("tag", tag.Id)),
					InventoryState.InContainer => new XElement("Action",
						new XAttribute("state", "incontainer"),
						new XAttribute("tag", tag.Id)),
					InventoryState.Attached => new XElement("Action",
						new XAttribute("state", "held"),
						new XAttribute("tag", tag.Id)),
					InventoryState.Prosthetic => new XElement("Action",
						new XAttribute("state", "prosthetic"),
						new XAttribute("tag", tag.Id)),
					InventoryState.Implanted => new XElement("Action",
						new XAttribute("state", "implanted"),
						new XAttribute("tag", tag.Id)),
					InventoryState.Consumed => new XElement("Action",
						new XAttribute("state", "consume"),
						new XAttribute("tag", tag.Id),
						new XAttribute("quantity", action.Quantity)),
					InventoryState.ConsumedLiquid => new XElement("Action",
						new XAttribute("state", "consumeliquid"),
						new XAttribute("tag", tag.Id)),
					_ => throw new NotImplementedException()
				});
			}

			return new XElement("Plan", new XElement("Phase", elements)).ToString();
		}

		internal static string BuildTargetDefinition(FuturemudDatabaseContext context, BodyProto targetBody, params string[] partNames)
		{
			var bodyChain = EnumerateTargetBodies(context, targetBody).ToList();
			var targetParts = new List<BodypartProto>();
			var missingParts = new List<string>();

			foreach (var partName in partNames)
			{
				var targetPart = bodyChain
					.Select(body => context.BodypartProtos.Local.FirstOrDefault(x => x.BodyId == body.Id && x.Name == partName) ??
					                context.BodypartProtos.FirstOrDefault(x => x.BodyId == body.Id && x.Name == partName))
					.FirstOrDefault(x => x is not null);

				if (targetPart is null)
				{
					missingParts.Add(partName);
					continue;
				}

				targetParts.Add(targetPart);
			}

			if (missingParts.Any())
			{
				throw new InvalidOperationException(
					$"Unable to find surgical target alias(es) {missingParts.ListToCommaSeparatedValues()} for body {targetBody.Name}. The resolver checked {bodyChain.Select(x => x.Name).ListToCommaSeparatedValues()}.");
			}

			return new XElement(
				"Definition",
				new XElement(
					"Parts",
					new XAttribute("forbidden", false),
					from targetPart in targetParts
					select new XElement("Part", targetPart.Id)
				)
			).ToString();
		}

		internal static IReadOnlyList<string> ValidateDefaultSurgeryTargetAliasesForTesting()
		{
			var bodyCatalogue = new Dictionary<string, (string? Parent, HashSet<string> Parts)>(StringComparer.OrdinalIgnoreCase)
			{
				["Humanoid"] = (
					null,
					new HashSet<string>(StringComparer.OrdinalIgnoreCase)
					{
						"lupperarm",
						"rupperarm",
						"lforearm",
						"rforearm",
						"lhand",
						"rhand",
						"lthigh",
						"rthigh",
						"lshin",
						"rshin",
						"lfoot",
						"rfoot",
						"lthumb",
						"rthumb",
						"lindexfinger",
						"rindexfinger",
						"lmiddlefinger",
						"rmiddlefinger",
						"lringfinger",
						"rringfinger",
						"lpinkyfinger",
						"rpinkyfinger",
						"lbigtoe",
						"rbigtoe",
						"lindextoe",
						"rindextoe",
						"lmiddletoe",
						"rmiddletoe",
						"lringtoe",
						"rringtoe",
						"lpinkytoe",
						"rpinkytoe"
					}
				),
				["Organic Humanoid"] = (
					"Humanoid",
					new HashSet<string>(StringComparer.OrdinalIgnoreCase)
					{
						"brain",
						"heart",
						"liver",
						"spleen",
						"stomach",
						"lintestines",
						"sintestines",
						"rkidney",
						"lkidney",
						"rlung",
						"llung",
						"trachea",
						"esophagus",
						"uspinalcord",
						"mspinalcord",
						"lspinalcord",
						"rinnerear",
						"linnerear"
					}
				),
				["Quadruped Base"] = (
					null,
					new HashSet<string>(StringComparer.OrdinalIgnoreCase)
					{
						"ruforeleg",
						"luforeleg",
						"rfknee",
						"lfknee",
						"rlforeleg",
						"llforeleg",
						"rfhock",
						"lfhock",
						"ruhindleg",
						"luhindleg",
						"rrknee",
						"rlknee",
						"rlhindleg",
						"llhindleg",
						"rrhock",
						"lrhock"
					}
				)
			};

			var targetGroups = new (string BodyName, string GroupName, IReadOnlyList<string> Parts)[]
			{
				("Organic Humanoid", nameof(HumanArmParts), HumanArmParts),
				("Organic Humanoid", nameof(HumanLegParts), HumanLegParts),
				("Organic Humanoid", nameof(HumanDigitParts), HumanDigitParts),
				("Organic Humanoid", nameof(HumanBrainTargets), HumanBrainTargets),
				("Organic Humanoid", nameof(HumanHeartTargets), HumanHeartTargets),
				("Organic Humanoid", nameof(HumanLiverTargets), HumanLiverTargets),
				("Organic Humanoid", nameof(HumanSpleenTargets), HumanSpleenTargets),
				("Organic Humanoid", nameof(HumanStomachTargets), HumanStomachTargets),
				("Organic Humanoid", nameof(HumanIntestineTargets), HumanIntestineTargets),
				("Organic Humanoid", nameof(HumanKidneyTargets), HumanKidneyTargets),
				("Organic Humanoid", nameof(HumanLungTargets), HumanLungTargets),
				("Organic Humanoid", nameof(HumanTracheaTargets), HumanTracheaTargets),
				("Organic Humanoid", nameof(HumanEsophagusTargets), HumanEsophagusTargets),
				("Organic Humanoid", nameof(HumanSpinalTargets), HumanSpinalTargets),
				("Organic Humanoid", nameof(HumanInnerEarTargets), HumanInnerEarTargets),
				("Quadruped Base", nameof(QuadrupedForelegParts), QuadrupedForelegParts),
				("Quadruped Base", nameof(QuadrupedHindlegParts), QuadrupedHindlegParts)
			};

			var issues = new List<string>();
			foreach (var (bodyName, groupName, parts) in targetGroups)
			{
				foreach (var part in parts)
				{
					if (BodyChainContainsPart(bodyName, part))
					{
						continue;
					}

					issues.Add($"{groupName} references '{part}', but it is not seeded on {bodyName} or any parent body.");
				}
			}

			return issues;

			bool BodyChainContainsPart(string bodyName, string partName)
			{
				for (var currentBody = bodyName; currentBody is not null; currentBody = bodyCatalogue[currentBody].Parent)
				{
					if (bodyCatalogue[currentBody].Parts.Contains(partName))
					{
						return true;
					}
				}

				return false;
			}
		}

		private string GetDefinitionForTargets(BodyProto targetBody, params string[] partNames)
		{
			return BuildTargetDefinition(_context, targetBody, partNames);
		}

		private static IEnumerable<BodyProto> EnumerateTargetBodies(FuturemudDatabaseContext context, BodyProto targetBody)
		{
			var seen = new HashSet<long>();
			var currentBody = targetBody;

			while (currentBody is not null && seen.Add(currentBody.Id))
			{
				yield return currentBody;
				currentBody = currentBody.CountsAs ??
				              (currentBody.CountsAsId is long parentId
					              ? context.BodyProtos.Local.FirstOrDefault(x => x.Id == parentId) ??
					                context.BodyProtos.FirstOrDefault(x => x.Id == parentId)
					              : null);
			}
		}

		private string ScalpelPlan() => ProduceInventoryPlanDefinition((InventoryState.Held, "Scalpel", 1));
		private string BonesawPlan() => ProduceInventoryPlanDefinition((InventoryState.Held, "Bonesaw", 1));
		private string ForcepsPlan() => ProduceInventoryPlanDefinition((InventoryState.Held, "Forceps", 1));
		private string ClampPlan() => ProduceInventoryPlanDefinition((InventoryState.Held, "Arterial Clamp", 1));
		private string SutureNeedlePlan() => ProduceInventoryPlanDefinition((InventoryState.Held, "Surgical Suture Needle", 1));

		public void SeedDrugs()
		{
			switch (SelectedTechLevel)
			{
				case "primitive":
					SeedPrimitiveDrugs();
					break;
				case "pre-modern":
					SeedPreModernDrugs();
					break;
				case "modern":
					SeedModernDrugs();
					break;
			}

			_context.SaveChanges();
		}

		private void SeedDrugDeliveryExamples()
		{
			var account = _context.Accounts.First();
			var now = DateTime.UtcNow;
			var nextId = _context.GameItemComponentProtos.Any() ? _context.GameItemComponentProtos.Max(x => x.Id) + 1 : 1;

			void UpsertComponent(string type, string name, string description, XElement definition)
			{
				var component = SeederRepeatabilityHelper.EnsureNamedEntity(
					_context.GameItemComponentProtos,
					name,
					x => x.Name,
					() =>
					{
						var editableItem = new EditableItem
						{
							RevisionNumber = 0,
							RevisionStatus = 4,
							BuilderAccountId = account.Id,
							BuilderDate = now,
							BuilderComment = "Auto-generated by the system",
							ReviewerAccountId = account.Id,
							ReviewerComment = "Auto-generated by the system",
							ReviewerDate = now
						};
						_context.EditableItems.Add(editableItem);
						var created = new GameItemComponentProto
						{
							Id = nextId++,
							RevisionNumber = 0,
							EditableItem = editableItem
						};
						_context.GameItemComponentProtos.Add(created);
						return created;
					});

				component.Type = type;
				component.Name = name;
				component.Description = description;
				component.Definition = definition.ToString();
			}

			foreach (var drug in _context.Drugs
				         .Where(x => ((DrugVector)x.DrugVectors).HasFlag(DrugVector.Ingested))
				         .OrderBy(x => x.Name)
				         .ToList())
			{
				UpsertComponent("Pill", $"Pill_{SanitizeDrugComponentName(drug.Name)}",
					$"Turns an item into a pill containing {drug.Name}.",
					new XElement("Definition",
						new XElement("GramsPerPill", 0.5),
						new XElement("Drug", drug.Id),
						new XElement("OnSwallowProg", 0)));
			}

			foreach (var drug in _context.Drugs
				         .Where(x => ((DrugVector)x.DrugVectors).HasFlag(DrugVector.Touched))
				         .OrderBy(x => x.Name)
				         .ToList())
			{
				UpsertComponent("TopicalCream", $"TopicalCream_{SanitizeDrugComponentName(drug.Name)}",
					$"Turns an item into a topical cream delivering {drug.Name}.",
					new XElement("Definition",
						new XElement("TotalGrams", 0.05),
						new XElement("OnApplyProg", 0),
						new XElement("Drugs",
							new XElement("Drug",
								new XAttribute("id", drug.Id),
								new XAttribute("grams", 0.1),
								new XAttribute("absorption", 0.75)))));
			}
		}

		private static string SanitizeDrugComponentName(string text)
		{
			return new string(text.Select(x => char.IsLetterOrDigit(x) ? x : '_').ToArray())
				.Trim('_')
				.Replace("__", "_");
		}

		private static IEnumerable<string> ExpectedStockDrugDeliveryMarkers(FuturemudDatabaseContext context)
		{
			var stockDrugNames = new HashSet<string>(StockHealthDrugs, StringComparer.OrdinalIgnoreCase);
			var markerSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			foreach (var drug in context.Drugs
				         .AsEnumerable()
				         .Where(x => stockDrugNames.Contains(x.Name)))
			{
				var vectors = (DrugVector)drug.DrugVectors;
				if (vectors.HasFlag(DrugVector.Ingested))
				{
					markerSet.Add($"Pill_{SanitizeDrugComponentName(drug.Name)}");
				}

				if (vectors.HasFlag(DrugVector.Touched))
				{
					markerSet.Add($"TopicalCream_{SanitizeDrugComponentName(drug.Name)}");
				}
			}

			foreach (var marker in StockDrugDeliveryMarkers.Where(marker => markerSet.Contains(marker)))
			{
				yield return marker;
			}
		}

		private void SeedPrimitiveDrugs()
		{
			AddDrug("Willow Bark Tea", 0.8, 0.15, DrugVector.Ingested,
				(DrugType.Analgesic, 0.55, string.Empty));
			AddDrug("Mandrake Draught", 0.9, 0.08, DrugVector.Ingested | DrugVector.Inhaled,
				(DrugType.Anesthesia, 0.45, string.Empty),
				(DrugType.Nausea, 0.30, string.Empty),
				(DrugType.VisionImpairment, 0.25, string.Empty));
			AddDrug("Honey Poultice", 0.7, 0.10, DrugVector.Touched,
				(DrugType.Antibiotic, 0.18, string.Empty),
				(DrugType.HealingRate, 0.20,
					new HealingRateAdditionalInfo { HealingRateIntensity = 0.15, HealingDifficultyIntensity = 0.0 }.DatabaseString));
			AddDrug("Garlic Salve", 0.6, 0.12, DrugVector.Touched | DrugVector.Ingested,
				(DrugType.Antifungal, 0.22, string.Empty),
				(DrugType.Nausea, 0.05, string.Empty));
			AddDrug("Mint Infusion", 0.5, 0.20, DrugVector.Ingested,
				(DrugType.NeutraliseDrugEffect, 0.40,
					new NeutraliseDrugAdditionalInfo { NeutralisedTypes = [DrugType.Nausea] }.DatabaseString));
			AddDrug("Ephedra Brew", 0.8, 0.18, DrugVector.Ingested,
				(DrugType.StaminaRegen, 0.35, string.Empty),
				(DrugType.Adrenaline, 0.20, string.Empty),
				(DrugType.ThermalImbalance, 0.05, string.Empty));
			AddDrug("Foxglove Tincture", 0.5, 0.06, DrugVector.Ingested,
				(DrugType.OrganFunction, 0.25,
					new OrganFunctionAdditionalInfo { OrganTypes = [BodypartTypeEnum.Heart] }.DatabaseString),
				(DrugType.Nausea, 0.25, string.Empty),
				(DrugType.VisionImpairment, 0.10, string.Empty));
		}

		private void SeedPreModernDrugs()
		{
			AddDrug("Laudanum", 0.9, 0.10, DrugVector.Ingested,
				(DrugType.Analgesic, 1.05, string.Empty),
				(DrugType.Pacifism, 0.15, string.Empty),
				(DrugType.Nausea, 0.25, string.Empty));
			AddDrug("Ether Anaesthetic", 1.0, 0.08, DrugVector.Inhaled,
				(DrugType.Anesthesia, 0.80, string.Empty),
				(DrugType.Nausea, 0.20, string.Empty),
				(DrugType.VisionImpairment, 0.10, string.Empty));
			AddDrug("Mould Poultice", 0.8, 0.10, DrugVector.Touched,
				(DrugType.Antibiotic, 0.40, string.Empty),
				(DrugType.HealingRate, 0.20,
					new HealingRateAdditionalInfo { HealingRateIntensity = 0.10, HealingDifficultyIntensity = 1.0 }.DatabaseString));
			AddDrug("Distilled Antiseptic", 0.7, 0.15, DrugVector.Touched | DrugVector.Ingested,
				(DrugType.Antibiotic, 0.22, string.Empty),
				(DrugType.Antifungal, 0.12, string.Empty),
				(DrugType.Nausea, 0.08, string.Empty));
			AddDrug("Mint and Ginger Tonic", 0.5, 0.18, DrugVector.Ingested,
				(DrugType.NeutraliseDrugEffect, 0.55,
					new NeutraliseDrugAdditionalInfo { NeutralisedTypes = [DrugType.Nausea] }.DatabaseString));
			AddDrug("Digitalis Tincture", 0.5, 0.05, DrugVector.Ingested,
				(DrugType.OrganFunction, 0.40,
					new OrganFunctionAdditionalInfo { OrganTypes = [BodypartTypeEnum.Heart] }.DatabaseString),
				(DrugType.Nausea, 0.20, string.Empty));
			AddDrug("Curare Paste", 0.6, 0.06, DrugVector.Touched | DrugVector.Injected,
				(DrugType.Paralysis, 0.60, string.Empty),
				(DrugType.Nausea, 0.10, string.Empty));
			AddDrug("Herbal Burn Salve", 0.7, 0.12, DrugVector.Touched,
				(DrugType.Analgesic, 0.20, string.Empty),
				(DrugType.HealingRate, 0.25,
					new HealingRateAdditionalInfo { HealingRateIntensity = 0.15, HealingDifficultyIntensity = 1.0 }.DatabaseString));
			AddDrug("Bronchial Smoke", 0.7, 0.08, DrugVector.Inhaled,
				(DrugType.OrganFunction, 0.30,
					new OrganFunctionAdditionalInfo { OrganTypes = [BodypartTypeEnum.Lung, BodypartTypeEnum.Trachea] }.DatabaseString),
				(DrugType.VisionImpairment, 0.05, string.Empty));
		}

		private void SeedModernDrugs()
		{
			var generalAnaesthetic = AddDrug("General Anaesthetic", 1.0, 0.08, DrugVector.Injected | DrugVector.Inhaled,
				(DrugType.Anesthesia, 1.0, string.Empty));
			var opioid = AddDrug("Opioid Analgesic", 0.9, 0.10, DrugVector.Injected | DrugVector.Ingested,
				(DrugType.Analgesic, 1.35, string.Empty),
				(DrugType.Pacifism, 0.15, string.Empty),
				(DrugType.Nausea, 0.20, string.Empty));
			var muscleRelaxant = AddDrug("Muscle Relaxant", 0.7, 0.08, DrugVector.Injected,
				(DrugType.Paralysis, 0.85, string.Empty));

			AddDrug("Local Anaesthetic", 0.8, 0.10, DrugVector.Injected | DrugVector.Touched,
				(DrugType.Analgesic, 0.85, string.Empty),
				(DrugType.Paralysis, 0.15, string.Empty));
			AddDrug("Broad-Spectrum Antibiotic", 0.9, 0.14, DrugVector.Injected | DrugVector.Ingested,
				(DrugType.Antibiotic, 1.0, string.Empty));
			AddDrug("Antibiotic Ointment", 0.7, 0.12, DrugVector.Touched,
				(DrugType.Antibiotic, 0.75, string.Empty),
				(DrugType.HealingRate, 0.20,
					new HealingRateAdditionalInfo { HealingRateIntensity = 0.10, HealingDifficultyIntensity = 0.5 }.DatabaseString));
			AddDrug("Antifungal Course", 0.8, 0.14, DrugVector.Ingested | DrugVector.Touched,
				(DrugType.Antifungal, 0.90, string.Empty));
			AddDrug("Burn Gel", 0.7, 0.12, DrugVector.Touched,
				(DrugType.Analgesic, 0.35, string.Empty),
				(DrugType.HealingRate, 0.30,
					new HealingRateAdditionalInfo { HealingRateIntensity = 0.20, HealingDifficultyIntensity = 1.0 }.DatabaseString));
			AddDrug("Antiemetic", 0.6, 0.18, DrugVector.Injected | DrugVector.Ingested,
				(DrugType.NeutraliseDrugEffect, 1.0,
					new NeutraliseDrugAdditionalInfo { NeutralisedTypes = [DrugType.Nausea] }.DatabaseString));
			AddDrug("Immunosuppressant", 0.7, 0.06, DrugVector.Injected | DrugVector.Ingested,
				(DrugType.Immunosuppressive, 0.90, string.Empty));
			AddDrug("Adrenaline Shot", 0.7, 0.08, DrugVector.Injected,
				(DrugType.Adrenaline, 1.0, string.Empty),
				(DrugType.StaminaRegen, 0.70, string.Empty),
				(DrugType.ThermalImbalance, 0.20, string.Empty));
			AddDrug("Bronchodilator", 0.7, 0.12, DrugVector.Inhaled | DrugVector.Ingested,
				(DrugType.OrganFunction, 0.60,
					new OrganFunctionAdditionalInfo { OrganTypes = [BodypartTypeEnum.Lung, BodypartTypeEnum.Trachea] }.DatabaseString));
			AddDrug("Cardiac Support Agent", 0.7, 0.08, DrugVector.Injected,
				(DrugType.OrganFunction, 0.60,
					new OrganFunctionAdditionalInfo { OrganTypes = [BodypartTypeEnum.Heart] }.DatabaseString));
			AddDrug("Healing Accelerant", 0.7, 0.10, DrugVector.Injected | DrugVector.Ingested,
				(DrugType.HealingRate, 0.55,
					new HealingRateAdditionalInfo { HealingRateIntensity = 0.35, HealingDifficultyIntensity = 2.0 }.DatabaseString));
			AddDrug("Antipyretic", 0.7, 0.16, DrugVector.Ingested | DrugVector.Injected,
				(DrugType.Analgesic, 0.25, string.Empty),
				(DrugType.NeutraliseDrugEffect, 0.70,
					new NeutraliseDrugAdditionalInfo { NeutralisedTypes = [DrugType.ThermalImbalance] }.DatabaseString));
			AddDrug("Overdose Antagonist", 0.6, 0.18, DrugVector.Injected,
				(DrugType.NeutraliseSpecificDrug, 1.0,
					new NeutraliseSpecificDrugAdditionalInfo
					{
						NeutralisedIds = [opioid.Id, generalAnaesthetic.Id, muscleRelaxant.Id]
					}.DatabaseString));
		}

		private Drug AddDrug(string name, double intensityPerGram, double relativeMetabolisationRate, DrugVector vectors,
			params (DrugType Type, double Intensity, string AdditionalEffects)[] effects)
		{
			var drug = SeederRepeatabilityHelper.EnsureNamedEntity(
				_context.Drugs,
				name,
				x => x.Name,
				() =>
				{
					var created = new Drug();
					_context.Drugs.Add(created);
					return created;
				});

			drug.Name = name;
			drug.IntensityPerGram = intensityPerGram;
			drug.RelativeMetabolisationRate = relativeMetabolisationRate;
			drug.DrugVectors = (int)vectors;
			_context.SaveChanges();

			foreach (var intensity in _context.DrugsIntensities.Where(x => x.DrugId == drug.Id).ToList())
			{
				_context.DrugsIntensities.Remove(intensity);
			}
			_context.SaveChanges();

			foreach (var effect in effects)
			{
				_context.DrugsIntensities.Add(new DrugIntensity
				{
					DrugId = drug.Id,
					DrugType = (int)effect.Type,
					RelativeIntensity = effect.Intensity,
					AdditionalEffects = effect.AdditionalEffects
				});
			}

			_context.SaveChanges();
			return drug;
		}
	}
}
