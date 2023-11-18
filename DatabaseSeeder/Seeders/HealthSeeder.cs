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

		public int SortOrder => 300;
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

		public void SeedDrugs()
		{

		}

	}
}
