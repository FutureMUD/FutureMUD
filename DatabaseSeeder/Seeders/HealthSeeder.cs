using Microsoft.EntityFrameworkCore.Infrastructure;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;

namespace DatabaseSeeder.Seeders
{
	internal class HealthSeeder : IDatabaseSeeder
	{
		public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
		new List<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool>
			Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
		{
			("techlevel",
				@"What should be the rough equivalent tech level of the surgeries, drugs and the like that you would like to install?

	#Bprimitive#0 - no replantation, transplantation, resection or implants
	#Bpre-modern#0 - no replantation, transplantation or implants
	#Bmodern#0 - all surgical procedures

Please answer #3primitive#F, #3pre-modern#0, or #3modern#F: ", (context, answers) => true,
				(answer, context) =>
				{
					if (!answer.EqualToAny("primitive", "pre-modern", "premodern", "pre modern", "modern")) return (false, "Please answer #3primitive#F, #3pre-modern#0, or #3modern#F.");

					return (true, string.Empty);
				})
		};

		private readonly Dictionary<string,Tag> _tags = new(StringComparer.OrdinalIgnoreCase);

		public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
		{
			_context = context;
			_questionAnswers = questionAnswers;
			foreach (var tag in _context.Tags.ToArray())
			{
				_tags[tag.Name] = tag;
			}
			context.Database.BeginTransaction();
			SeedKnowledges();
			SeedSurgery();
			SeedDrugs();
			context.SaveChanges();
			context.Database.CommitTransaction();

			return "Successfully set up Health Modules.";
		}

		public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
		{
			if (!context.Accounts.Any()) return ShouldSeedResult.PrerequisitesNotMet;

			if (!context.Races.Any(x => x.Name == "Organic Humanoid"))
			{
				return ShouldSeedResult.PrerequisitesNotMet;
			}

			if (context.SurgicalProcedures.Any()) return ShouldSeedResult.MayAlreadyBeInstalled;

			return ShouldSeedResult.ReadyToInstall;
		}

		public int SortOrder => 250;
		public string Name => "Health Seeder";
		public string Tagline => "Sets up Surgeries, Drugs, Medical Equipment";

		public string FullDescription => "";

		#region Implementation of IDatabaseSeeder

		/// <inheritdoc />
		public bool Enabled => false;

		#endregion

		private FuturemudDatabaseContext _context;
		private IReadOnlyDictionary<string, string> _questionAnswers;
		private readonly Dictionary<string, MudSharp.Models.Knowledge> _knowledges = new(StringComparer.OrdinalIgnoreCase);
		private readonly Dictionary<string, MudSharp.Models.SurgicalProcedure> _procedures = new(StringComparer.OrdinalIgnoreCase);

		private MudSharp.Models.FutureProg? _alwaysTrueProg;
		public MudSharp.Models.FutureProg AlwaysTrueProg
		{
			get
			{
				_alwaysTrueProg ??= _context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");
				return _alwaysTrueProg;
			}
		}

		private void AddSurgicalProcedure(string name, string procedureName, string school, SurgicalProcedureType type, double baseCheckBonus, long knowledgeId, CheckType check, string gerund, string emote, string description, string definition)
		{
			var dbitem = new MudSharp.Models.SurgicalProcedure { 
				Name = name, 
				ProcedureName = procedureName, 
				Procedure = (int)type,
				MedicalSchool = school,
				BaseCheckBonus = baseCheckBonus,
				KnowledgeRequiredId = knowledgeId,
				ProcedureBeginEmote = emote,
				ProcedureGerund = gerund,
				ProcedureDescriptionEmote = description,
				Check = (int)check,
				Definition = definition,
			};
			_context.SurgicalProcedures.Add(dbitem);
			_procedures[name] = dbitem;
		}

		private void AddSurgicalProcedurePhase(MudSharp.Models.SurgicalProcedure procedure, int phaseNumber, double seconds, string? special, string emote, string? template = null)
		{
			procedure.SurgicalProcedurePhases.Add(new MudSharp.Models.SurgicalProcedurePhase { 
				PhaseNumber = phaseNumber, 
				PhaseEmote= emote,
				PhaseSpecialEffects = special,
				BaseLengthInSeconds= seconds,
				SurgicalProcedureId = procedure.Id,
				InventoryActionPlan = template
			});
		}

		private void AddKnowledge(string name, string type, string subType, int sessions, Difficulty difficulty, string description, string longDescription)
		{
			var knowledge = new MudSharp.Models.Knowledge 
			{ 
				Name = name,
				Type = type,
				Subtype = subType,
				Description = description,
				LongDescription = longDescription,
				LearnableType = (int)(LearnableType.LearnableFromTeacher | LearnableType.LearnableAtChargen),
				LearnDifficulty = (int)difficulty,
				TeachDifficulty = (int)difficulty,
				LearningSessionsRequired = sessions,
				CanAcquireProg = AlwaysTrueProg,
				CanLearnProg = AlwaysTrueProg,
			};
			_context.Knowledges.Add(knowledge);
			_knowledges[name] = knowledge;
		}

		private void SeedKnowledges()
		{
			switch (_questionAnswers["techlevel"].ToLowerInvariant())
			{
				case "primitive":
					AddKnowledge("Medicine", "Medicine", "Human", 15, Difficulty.Hard, "Knowledge of the practice of treating the sick and injured", @"This knowledge is about knowing all the correct prayers, invocations and minstrations to treat the sick and injured. Those who hold this knowledge will be able to balance the humours of an imbalance individual and understand the nature of injuries that beset them.");
					break;
				case "pre-modern":
				case "premodern":
				case "pre modern":
					AddKnowledge("Chiurgery", "Medicine", "Human", 15, Difficulty.Hard, "Knowledge of the practice of performing surgeries and other medical procedures on humans", @"Chiurgery (also known as Surgery) translates to ""hand work"", and is the knowledge of doing practical medical interventions on the sick and injured. It is the knowledge of how to make major incisions to repair, remove, or replace various parts of the body, as well as associated minor treatments that go with this.");
					AddKnowledge("Physical Medicine", "Medicine", "Human", 20, Difficulty.VeryHard, "Knowledge of the higher medical principals such as anatomy and physiology of humans", "Physical Medicine is the academic knowledge of the higher principals of anatomy, medicine, and the practice of diagnosis.");
					break;
				case "modern":
					AddKnowledge("Diagnostic Medicine", "Medicine", "Human", 20, Difficulty.Hard, "Knowledge of the practice of diagnosing illness and trauma in humans", "This is a knowledge of medicine that helps you understand the necessary symptoms, descriptions and signs to diagnose disease and trauma in humans.");
					AddKnowledge("Clinical Medicine", "Medicine", "Human", 10, Difficulty.Normal, "Knowledge of the core clinical medical procedures used in human medicine", "This is a knowledge of common medical procedures used in clinical settings. This allows you to be a medical practioner and perform minor tasks.");
					AddKnowledge("Surgery", "Medicine", "Human", 15, Difficulty.Hard, "Knowledge of surgical procedures and techniques", "This is the knowledge of surgery, which allows you to perform various procedures to repair, remove, replace or otherwise alter the human body.");
					break;
			}

			_context.SaveChanges();
		}

		public void SeedSurgery()
		{
			var useTags = _context.Tags.Any(x => x.Name == "Surgical Tools");
			switch (_questionAnswers["techlevel"].ToLowerInvariant())
			{
				case "primitive":
					SeedPrimitiveSurgery();
					return;
				case "pre-modern":
				case "premodern":
				case "pre modern":
					SeedPreModernSurgery();
					return;
				case "modern":
					SeedModernSurgery();
					return;
			}
			
		}

		private void SeedPrimitiveSurgery()
		{
			AddSurgicalProcedure("Hasty Triage", "hasty triage", "Human Medicine", SurgicalProcedureType.Triage, -3.0, _knowledges["Medicine"].Id, CheckType.TriageCheck, "triaging", "@ begin|begins triaging $1=0", @"This procedure will use a variety of relatively non-invasive and non-surgical methods to guess at the state of injury and stability of a patient. Upon completion (and success), you will get a report of your patient's wounds (including ones that might not be apparent to a normal look), your patients bloodloss, an indication of locations of internal trauma, and a few indications of some specific conditions like liver failure, concussion and the like. It doesn't take as long as a more detailed triage, but you are also less likely to pick up problems. This might be useful in a mass casualty situation where time is off the essence.", "");
			AddSurgicalProcedurePhase(_procedures["Hasty Triage"], 1, 10, null, "@ observe|observes $1, noting any obvious bleeding or bruising");
			AddSurgicalProcedurePhase(_procedures["Hasty Triage"], 2, 10, null, "@ lightly prod|prods $1's vital areas, noting &1's reaction");
			AddSurgicalProcedurePhase(_procedures["Hasty Triage"], 3, 10, null, "@ feel|feels $1's forehead, checking for fever");
		}

		private void SeedPreModernSurgery()
		{
			AddSurgicalProcedure("Hasty Triage", "hasty triage", "Human Medicine", SurgicalProcedureType.Triage, -3.0, _knowledges["Physical Medicine"].Id, CheckType.TriageCheck, "triaging", "@ begin|begins triaging $1=0", @"This procedure will use a variety of relatively non-invasive and non-surgical methods to guess at the state of injury and stability of a patient. Upon completion (and success), you will get a report of your patient's wounds (including ones that might not be apparent to a normal look), your patients bloodloss, an indication of locations of internal trauma, and a few indications of some specific conditions like liver failure, concussion and the like. It doesn't take as long as a more detailed triage, but you are also less likely to pick up problems. This might be useful in a mass casualty situation where time is off the essence.", "");
			AddSurgicalProcedurePhase(_procedures["Hasty Triage"], 1, 10, null, "@ observe|observes $1, noting any obvious bleeding or bruising");
			AddSurgicalProcedurePhase(_procedures["Hasty Triage"], 2, 10, null, "@ lightly prod|prods $1's vital areas, noting &1's reaction");
			AddSurgicalProcedurePhase(_procedures["Hasty Triage"], 3, 10, null, "@ feel|feels $1's forehead, checking for fever");
		}

		private void SeedModernSurgery()
		{
			AddSurgicalProcedure("Hasty Triage", "hasty triage", "Human Medicine", SurgicalProcedureType.Triage, -3.0, _knowledges["Diagnostic Medicine"].Id, CheckType.TriageCheck, "triaging", "@ begin|begins triaging $1=0", @"This procedure will use a variety of relatively non-invasive and non-surgical methods to guess at the state of injury and stability of a patient. Upon completion (and success), you will get a report of your patient's wounds (including ones that might not be apparent to a normal look), your patients bloodloss, an indication of locations of internal trauma, and a few indications of some specific conditions like liver failure, concussion and the like. It doesn't take as long as a more detailed triage, but you are also less likely to pick up problems. This might be useful in a mass casualty situation where time is off the essence.", "");
			AddSurgicalProcedurePhase(_procedures["Hasty Triage"], 1, 10, null, "@ observe|observes $1, noting any obvious bleeding or bruising");
			AddSurgicalProcedurePhase(_procedures["Hasty Triage"], 2, 10, null, "@ lightly prod|prods $1's vital areas, noting &1's reaction");
			AddSurgicalProcedurePhase(_procedures["Hasty Triage"], 3, 10, null, "@ feel|feels $1's forehead, checking for fever");

			AddSurgicalProcedure("Triage", "triage", "Human Medicine", SurgicalProcedureType.Triage, 0.0, _knowledges["Diagnostic Medicine"].Id, CheckType.TriageCheck, "triaging", "@ begin|begins triaging $1=0", @"This procedure will use a variety of relatively non-invasive and non-surgical methods to guess at the state of injury and stability of a patient. Upon completion (and success), you will get a report of your patient's wounds (including ones that might not be apparent to a normal look), your patients bloodloss, an indication of locations of internal trauma, and a few indications of some specific conditions like liver failure, concussion and the like.", "");
			AddSurgicalProcedurePhase(_procedures["Triage"], 1, 15, null, "@ observe|observes $1, noting any obvious bleeding or bruising");
			AddSurgicalProcedurePhase(_procedures["Triage"], 2, 20, null, "@ put|puts &0's finger on $1's neck and mentally count|counts the pulse rate");
			AddSurgicalProcedurePhase(_procedures["Triage"], 3, 15, null, "@ lightly prod|prods $1's vital areas, noting &1's reaction");
			AddSurgicalProcedurePhase(_procedures["Triage"], 4, 20, null, "@ gently move|moves several of $1's limbs and joints, looking for a reaction and testing range of movement");
			AddSurgicalProcedurePhase(_procedures["Triage"], 5, 10, null, "@ feel|feels $1's forehead, checking for fever");

			AddSurgicalProcedure("Crude Physical", "crude physical", "Human Medicine", SurgicalProcedureType.DetailedExamination, -3.0, _knowledges["Diagnostic Medicine"].Id, CheckType.MedicalExaminationCheck, "examining", "@ begin|begins examining $1=0", @"This procedure is used to perform a basic physical examination of the patient, but absent any modern tools. At conclusion, and if successful, you will receive a report of the target's age, general health attributes, and current wounds.", "");
			AddSurgicalProcedurePhase(_procedures["Physical"], 1, 25, null, "@ begin|begins &0's physical examination of $1 by considering &1's physical form and estimating &1's height and weight");
			AddSurgicalProcedurePhase(_procedures["Physical"], 2, 25, null, "@ continue|continues &0's physical examination of $1 by taking &1's blood pressure with a mental count and a thumb on the patient's wrist");
			AddSurgicalProcedurePhase(_procedures["Physical"], 3, 25, null, "@ continue|continues &0's physical examination of $1 by listening to &1's breathing");
			AddSurgicalProcedurePhase(_procedures["Physical"], 4, 25, null, "@ continue|continues &0's examination of $1 by testing the flexibility of various joints and limbs by manual manipulation, and squeezing and prodding various parts");
			AddSurgicalProcedurePhase(_procedures["Physical"], 5, 25, null, "@ step|steps back from $1, having completed the physical examination");

			AddSurgicalProcedure("Physical", "physical", "Human Medicine", SurgicalProcedureType.DetailedExamination, 2.0, _knowledges["Diagnostic Medicine"].Id, CheckType.MedicalExaminationCheck, "examining", "@ begin|begins examining $1=0", @"This procedure is used to perform a basic physical examination of the patient. At conclusion, and if successful, you will receive a report of the target's age, general health attributes, and current wounds.", "");
			AddSurgicalProcedurePhase(_procedures["Physical"], 1, 25, null, "@ begin|begins &0's physical examination of $1 by asking &1 to stand on $i1", ProduceInventoryPlanDefinition((InventoryState.Dropped, "Mechanical Scale", 1)));
			AddSurgicalProcedurePhase(_procedures["Physical"], 2, 25, null, "@ continue|continues &0's physical examination of $1 by taking &1's blood pressure with $i1", ProduceInventoryPlanDefinition((InventoryState.Held, "Blood Pressure Monitor", 1)));
			AddSurgicalProcedurePhase(_procedures["Physical"], 3, 25, null, "@ continue|continues &0's physical examination of $1 by listening to &1's breathing with $i1, and checking &1's pulse", ProduceInventoryPlanDefinition((InventoryState.Held, "Stethoscope", 1)));
			AddSurgicalProcedurePhase(_procedures["Physical"], 4, 25, null, "@ continue|continues &0's examination of $1 by testing the flexibility of various joints and limbs with $i1, and squeezing and prodding various parts", ProduceInventoryPlanDefinition((InventoryState.Held, "Tendon Hammer", 1)));
			AddSurgicalProcedurePhase(_procedures["Physical"], 5, 25, null, "@ step|steps back from $1, having completed the physical examination");

			AddSurgicalProcedure("Stitch Up", "stitchup", "Human Medicine", SurgicalProcedureType.InvasiveProcedureFinalisation, 0.0, _knowledges["Surgery"].Id, CheckType.InvasiveProcedureFinalisation, "stiching-up", "@ begin|begins stitching up $1", @"This procedure is a surgical suture and tidy-up procedure. It is used after other procedures that make any kind of substantial incision, and effectively finalises the procedure. Without the successful completion of this procedure, the patient is likely to get rather nasty infections on their internal organs.", "");
			AddSurgicalProcedurePhase(_procedures["Stitch Up"], 1, 25, null, "@ begin|begins moving everything inside $1's {0} back to where it is supposed to be");
			AddSurgicalProcedurePhase(_procedures["Stitch Up"], 2, 25, null, "@ begin|begins the process of reattaching $1's larger blood vessels back to each other as needed using $i1 and $i2");
			AddSurgicalProcedurePhase(_procedures["Stitch Up"], 3, 25, null, "@ finish|finishes the process of attaching $1's larger blood vessels back to each other");
			AddSurgicalProcedurePhase(_procedures["Stitch Up"], 4, 25, null, "@ begin|begins to suture all of the muscle tissue in $1's {0} that has been separated back together with $i1 and $i2");
			AddSurgicalProcedurePhase(_procedures["Stitch Up"], 5, 25, null, "@ pull|pulls the skin closed on $1's {0} wound, then slowly, carefully, stitch|stitches it closed with $i1 and $i2");
                        AddSurgicalProcedurePhase(_procedures["Stitch Up"], 6, 25, null, "@ finish|finishes the last stitch on $1's {0} and clip|clips and tie|ties it off; the wound is closed");

                        AddSurgicalProcedure("Exploratory Surgery", "exploratory", "Human Medicine", SurgicalProcedureType.ExploratorySurgery, 0.0, _knowledges["Surgery"].Id, CheckType.ExploratorySurgeryCheck, "exploring", "@ begin|begins exploratory surgery on $1", "", "");
                        AddSurgicalProcedurePhase(_procedures["Exploratory Surgery"], 1, 30, "exposed", "@ begin|begins to clean and incise $1's {0} with $i1", ProduceInventoryPlanDefinition((InventoryState.Held, "Scalpel", 1)));
                        AddSurgicalProcedurePhase(_procedures["Exploratory Surgery"], 2, 30, null, "@ carefully probe|probes within $1's {0} using $i1", ProduceInventoryPlanDefinition((InventoryState.Held, "Forceps", 1)));
                        AddSurgicalProcedurePhase(_procedures["Exploratory Surgery"], 3, 30, null, "@ finish|finishes examining the area");

                        AddSurgicalProcedure("Arm Amputation", "arm amputation", "Human Medicine", SurgicalProcedureType.Amputation, 0.0, _knowledges["Surgery"].Id, CheckType.AmputationCheck, "amputating", "@ begin|begins to amputate $1's arm", @"The amputation procedure is used to remove an arm from a patient. It can be used on living or dead patients.", GetDefinitionForBodyparts("lupperarm","rupperarm","lforearm","rforearm","lhand","rhand"));
                        AddSurgicalProcedurePhase(_procedures["Arm Amputation"], 1, 30, "exposed", "@ mark|marks the site on $1's {0} and make|makes an incision with $i1", ProduceInventoryPlanDefinition((InventoryState.Held,"Scalpel",1)));
                        AddSurgicalProcedurePhase(_procedures["Arm Amputation"], 2, 30, null, "@ saw|saws through the limb with $i1", ProduceInventoryPlanDefinition((InventoryState.Held,"Bonesaw",1)));
                        AddSurgicalProcedurePhase(_procedures["Arm Amputation"], 3, 20, null, "@ remove|removes the severed limb");

                        AddSurgicalProcedure("Leg Amputation", "leg amputation", "Human Medicine", SurgicalProcedureType.Amputation, 0.0, _knowledges["Surgery"].Id, CheckType.AmputationCheck, "amputating", "@ begin|begins to amputate $1's leg", @"The amputation procedure removes a leg from the patient.", GetDefinitionForBodyparts("lthigh","rthigh","llowerleg","rlowerleg","lfoot","rfoot"));
                        AddSurgicalProcedurePhase(_procedures["Leg Amputation"], 1, 30, "exposed", "@ mark|marks the site on $1's {0} and make|makes an incision with $i1", ProduceInventoryPlanDefinition((InventoryState.Held,"Scalpel",1)));
                        AddSurgicalProcedurePhase(_procedures["Leg Amputation"], 2, 30, null, "@ saw|saws through the limb with $i1", ProduceInventoryPlanDefinition((InventoryState.Held,"Bonesaw",1)));
                        AddSurgicalProcedurePhase(_procedures["Leg Amputation"], 3, 20, null, "@ remove|removes the severed limb");

                        AddSurgicalProcedure("Replantation", "replantation", "Human Medicine", SurgicalProcedureType.Replantation, 0.0, _knowledges["Surgery"].Id, CheckType.ReplantationCheck, "replanting", "@ begin|begins replantation on $1", @"The replantation procedure reattaches a severed bodypart to the patient.", "");
                        AddSurgicalProcedurePhase(_procedures["Replantation"], 1, 30, "exposed", "@ clean|cleans the stump on $1 with $i1", ProduceInventoryPlanDefinition((InventoryState.Held,"Scalpel",1)));
                        AddSurgicalProcedurePhase(_procedures["Replantation"], 2, 30, null, "@ align|aligns the severed part and begin|begins suturing with $i1", ProduceInventoryPlanDefinition((InventoryState.Held,"Suture_Kit",1)));
                        AddSurgicalProcedurePhase(_procedures["Replantation"], 3, 30, null, "@ finish|finishes securing the part in place");

                        AddSurgicalProcedure("Cannulation", "cannulation", "Human Medicine", SurgicalProcedureType.Cannulation, 0.0, _knowledges["Clinical Medicine"].Id, CheckType.CannulationProcedure, "cannulating", "@ begin|begins a cannulation procedure on $1", @"Cannulation installs a cannula into a vein for IV access.", "");
                        AddSurgicalProcedurePhase(_procedures["Cannulation"], 1, 15, "exposed", "@ prepare|prepares $1's {0} and insert|inserts $i1", ProduceInventoryPlanDefinition((InventoryState.Held,"Cannula",1)));
                        AddSurgicalProcedurePhase(_procedures["Cannulation"], 2, 15, null, "@ secure|secures the cannula in place");

                        AddSurgicalProcedure("Decannulation", "decannulation", "Human Medicine", SurgicalProcedureType.Decannulation, 0.0, _knowledges["Clinical Medicine"].Id, CheckType.DecannulationProcedure, "decannulating", "@ begin|begins to decannulate $1", @"Decannulation removes a cannula from a patient's veins.", "");
                        AddSurgicalProcedurePhase(_procedures["Decannulation"], 1, 15, null, "@ gently withdraw|withdraws the cannula from $1's {0}");

                        AddSurgicalProcedure("Trauma Control", "trauma control", "Human Medicine", SurgicalProcedureType.TraumaControl, 0.0, _knowledges["Surgery"].Id, CheckType.TraumaControlSurgery, "patching up", "@ begin|begins trauma control with $1", @"This procedure stops internal bleeding in a bodypart.", "");
                        AddSurgicalProcedurePhase(_procedures["Trauma Control"], 1, 30, "exposed", "@ open|opens $1's {0} with $i1", ProduceInventoryPlanDefinition((InventoryState.Held,"Scalpel",1)));
                        AddSurgicalProcedurePhase(_procedures["Trauma Control"], 2, 30, null, "@ clamp|clamps bleeding vessels with $i1", ProduceInventoryPlanDefinition((InventoryState.Held,"Arterial Clamp",1)));
                        AddSurgicalProcedurePhase(_procedures["Trauma Control"], 3, 30, null, "@ close|closes up the wound with $i1", ProduceInventoryPlanDefinition((InventoryState.Held,"Suture_Kit",1)));

                        AddSurgicalProcedure("Organ Extraction", "organ extraction", "Human Medicine", SurgicalProcedureType.OrganExtraction, 0.0, _knowledges["Surgery"].Id, CheckType.OrganExtractionCheck, "cutting out", "@ begin|begins to perform an organ extraction on $1", "", "");
                        AddSurgicalProcedurePhase(_procedures["Organ Extraction"], 1, 30, "checkorgan exposed", "@ make|makes an incision with $i1", ProduceInventoryPlanDefinition((InventoryState.Held,"Scalpel",1)));
                        AddSurgicalProcedurePhase(_procedures["Organ Extraction"], 2, 30, null, "@ carefully detach|detaches the organ using $i1", ProduceInventoryPlanDefinition((InventoryState.Held,"Forceps",1)));
                        AddSurgicalProcedurePhase(_procedures["Organ Extraction"], 3, 30, null, "@ remove|removes the organ from $1");

                        AddSurgicalProcedure("Organ Transplant", "organ transplant", "Human Medicine", SurgicalProcedureType.OrganTransplant, 0.0, _knowledges["Surgery"].Id, CheckType.OrganTransplantCheck, "transplanting", "@ begin|begins an organ transplantation on $1", "", "");
                        AddSurgicalProcedurePhase(_procedures["Organ Transplant"], 1, 30, "checkspace", "@ prepare|prepares the cavity in $1's {0} with $i1", ProduceInventoryPlanDefinition((InventoryState.Held,"Scalpel",1)));
                        AddSurgicalProcedurePhase(_procedures["Organ Transplant"], 2, 30, "checkorgan", "@ place|places the organ and connect|connects vessels with $i1", ProduceInventoryPlanDefinition((InventoryState.Held,"Forceps",1)));
                        AddSurgicalProcedurePhase(_procedures["Organ Transplant"], 3, 30, null, "@ close|closes up the incision with $i1", ProduceInventoryPlanDefinition((InventoryState.Held,"Suture_Kit",1)));

                        AddSurgicalProcedure("Organ Stabilisation", "organ stabilisation", "Human Medicine", SurgicalProcedureType.OrganStabilisation, 0.0, _knowledges["Surgery"].Id, CheckType.OrganStabilisationCheck, "resecting", "@ begin|begins to perform a resection on $1", "", "");
                        AddSurgicalProcedurePhase(_procedures["Organ Stabilisation"], 1, 30, "checkorgan", "@ inspect|inspects $1's {0} for damage", null);
                        AddSurgicalProcedurePhase(_procedures["Organ Stabilisation"], 2, 30, null, "@ repair|repairs damaged tissue with $i1", ProduceInventoryPlanDefinition((InventoryState.Held,"Forceps",1)));
                        AddSurgicalProcedurePhase(_procedures["Organ Stabilisation"], 3, 30, null, "@ close|closes up with $i1", ProduceInventoryPlanDefinition((InventoryState.Held,"Suture_Kit",1)));

                        AddSurgicalProcedure("Bone Setting", "bone setting", "Human Medicine", SurgicalProcedureType.SurgicalBoneSetting, 0.0, _knowledges["Surgery"].Id, CheckType.SurgicalSetCheck, "setting", "@ begin|begins to set $1's bone", "", "");
                        AddSurgicalProcedurePhase(_procedures["Bone Setting"], 1, 30, "exposed checkbone", "@ examine|examines $1's {0}");
                        AddSurgicalProcedurePhase(_procedures["Bone Setting"], 2, 30, null, "@ manipulate|manipulates the bone back into alignment with $i1", ProduceInventoryPlanDefinition((InventoryState.Held,"Forceps",1)));
                        AddSurgicalProcedurePhase(_procedures["Bone Setting"], 3, 20, null, "@ secure|secures the bone with a splint");

                        AddSurgicalProcedure("Install Implant", "install implant", "Human Medicine", SurgicalProcedureType.InstallImplant, 0.0, _knowledges["Surgery"].Id, CheckType.InstallImplantSurgery, "installing", "@ begin|begins to install an implant in $1", "", "");
                        AddSurgicalProcedurePhase(_procedures["Install Implant"], 1, 30, "exposed checkspace", "@ open|opens $1's {0} with $i2", ProduceInventoryPlanDefinition((InventoryState.Held,"Implant",1),(InventoryState.Held,"Scalpel",1)));
                        AddSurgicalProcedurePhase(_procedures["Install Implant"], 2, 30, null, "@ insert|inserts $i1 and secure|secures it", ProduceInventoryPlanDefinition((InventoryState.Held,"Implant",1)));
                        AddSurgicalProcedurePhase(_procedures["Install Implant"], 3, 20, null, "@ close|closes the incision with $i2", ProduceInventoryPlanDefinition((InventoryState.Held,"Suture_Kit",1)));

                        AddSurgicalProcedure("Remove Implant", "remove implant", "Human Medicine", SurgicalProcedureType.RemoveImplant, 0.0, _knowledges["Surgery"].Id, CheckType.RemoveImplantSurgery, "removing", "@ begin|begins to remove an implant from $1", "", "");
                        AddSurgicalProcedurePhase(_procedures["Remove Implant"], 1, 30, "exposed", "@ open|opens $1's {0} with $i2", ProduceInventoryPlanDefinition((InventoryState.Held,"Scalpel",1)));
                        AddSurgicalProcedurePhase(_procedures["Remove Implant"], 2, 30, null, "@ extract|extracts the implant using $i1", ProduceInventoryPlanDefinition((InventoryState.Held,"Implant",1)));
                        AddSurgicalProcedurePhase(_procedures["Remove Implant"], 3, 20, null, "@ close|closes the incision with $i2", ProduceInventoryPlanDefinition((InventoryState.Held,"Suture_Kit",1)));

                        AddSurgicalProcedure("Configure Implant Power", "configure implant power", "Human Medicine", SurgicalProcedureType.ConfigureImplantPower, 0.0, _knowledges["Surgery"].Id, CheckType.ConfigureImplantPowerSurgery, "configuring", "@ begin|begins to configure power settings for $1's implants", "", "");
                        AddSurgicalProcedurePhase(_procedures["Configure Implant Power"], 1, 20, null, "@ adjust|adjusts wiring and power settings");

                        AddSurgicalProcedure("Configure Implant Interface", "configure implant interface", "Human Medicine", SurgicalProcedureType.ConfigureImplantInterface, 0.0, _knowledges["Surgery"].Id, CheckType.ConfigureImplantInterfaceSurgery, "configuring", "@ begin|begins to configure interface settings for $1's implants", "", "");
                        AddSurgicalProcedurePhase(_procedures["Configure Implant Interface"], 1, 20, null, "@ tune|tunes the interface connections");
		}

                private string ProduceInventoryPlanDefinition(params (InventoryState State, string Tag, int Quantity)[] actions)
                {
			return new XElement("Plan",
				new XElement("Phase",
					from action in actions
					select action.State switch
					{
						InventoryState.Held => new XElement("Action", new XAttribute("state", "held"), new XAttribute("tag", _tags[action.Tag].Id), new XAttribute("quantity", action.Quantity), new XAttribute("optionalquantity", false)),
						InventoryState.Wielded => new XElement("Action", new XAttribute("state", "wielded"), new XAttribute("tag", _tags[action.Tag].Id), new XAttribute("wieldstate", (int)AttackHandednessOptions.Any)),
						InventoryState.Worn => new XElement("Action", new XAttribute("state", "worn"), new XAttribute("tag", _tags[action.Tag].Id)),
						InventoryState.Dropped => new XElement("Action", new XAttribute("state", "dropped"), new XAttribute("tag", _tags[action.Tag].Id)),
						InventoryState.Sheathed => new XElement("Action", new XAttribute("state", "sheathed"), new XAttribute("tag", _tags[action.Tag].Id)),
						InventoryState.InContainer => new XElement("Action", new XAttribute("state", "incontainer"), new XAttribute("tag", _tags[action.Tag].Id)),
						InventoryState.Attached => new XElement("Action", new XAttribute("state", "held"), new XAttribute("tag", _tags[action.Tag].Id)),
						InventoryState.Prosthetic => new XElement("Action", new XAttribute("state", "prosthetic"), new XAttribute("tag", _tags[action.Tag].Id)),
						InventoryState.Implanted => new XElement("Action", new XAttribute("state", "implanted"), new XAttribute("tag", _tags[action.Tag].Id)),
						InventoryState.Consumed => new XElement("Action", new XAttribute("state", "consume"), new XAttribute("tag", _tags[action.Tag].Id), new XAttribute("quantity", action.Quantity)),
						InventoryState.ConsumedLiquid => new XElement("Action", new XAttribute("state", "consumeliquid"), new XAttribute("tag", _tags[action.Tag].Id)),

						_ => throw new NotImplementedException()
					}
				)
                        ).ToString();
                }

                private string GetDefinitionForBodyparts(params string[] parts)
                {
                        return new XElement("Definition",
                                new XElement("Parts",
                                        new XAttribute("forbidden", false),
                                        from part in parts
                                        let id = _context.BodypartProtos.First(x => x.Name == part).Id
                                        select new XElement("Part", id)
                                )
                        ).ToString();
                }

                public void SeedDrugs()
                {
                        var anaesthetic = new Drug
                        {
                                Name = "General Anaesthetic",
                                IntensityPerGram = 1.0,
                                RelativeMetabolisationRate = 0.1,
                                DrugVectors = (int)(DrugVector.Injected | DrugVector.Inhaled)
                        };
                        _context.Drugs.Add(anaesthetic);
                        _context.SaveChanges();
                        _context.DrugIntensities.Add(new DrugIntensity
                        {
                                DrugId = anaesthetic.Id,
                                DrugType = (int)DrugType.Anesthesia,
                                RelativeIntensity = 1.0,
                                AdditionalEffects = string.Empty
                        });

                        var painkiller = new Drug
                        {
                                Name = "Basic Analgesic",
                                IntensityPerGram = 1.0,
                                RelativeMetabolisationRate = 0.2,
                                DrugVectors = (int)DrugVector.Ingested
                        };
                        _context.Drugs.Add(painkiller);
                        _context.SaveChanges();
                        _context.DrugIntensities.Add(new DrugIntensity
                        {
                                DrugId = painkiller.Id,
                                DrugType = (int)DrugType.Analgesic,
                                RelativeIntensity = 1.0,
                                AdditionalEffects = string.Empty
                        });
                        _context.SaveChanges();
                }

	}
}
