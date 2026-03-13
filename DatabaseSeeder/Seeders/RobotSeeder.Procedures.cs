#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Combat;
using MudSharp.GameItems.Inventory;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public partial class RobotSeeder
{
	private static readonly string[] HumanoidArmTargets = ["rupperarm", "lupperarm", "rforearm", "lforearm", "rhand", "lhand"];
	private static readonly string[] HumanoidLegTargets = ["rthigh", "lthigh", "rshin", "lshin", "rfoot", "lfoot"];
	private static readonly string[] QuadrupedTargets =
	[
		"ruforeleg",
		"luforeleg",
		"rlforeleg",
		"llforeleg",
		"ruhindleg",
		"luhindleg",
		"rlhindleg",
		"llhindleg",
		"rfpaw",
		"lfpaw",
		"rrpaw",
		"lrpaw"
	];
	private static readonly string[] InsectoidTargets =
	[
		"rleg1",
		"lleg1",
		"rleg2",
		"lleg2",
		"rleg3",
		"lleg3",
		"mandibles"
	];
	private static readonly string[] UtilityTargets = ["rdrivewheel", "ldrivewheel", "rtrack", "ltrack"];

	private sealed record ProcedureTarget(string Label, BodyProto Body, IReadOnlyList<string> Organs, IReadOnlyList<string> Limbs);

	private void SeedRobotProcedures(IReadOnlyDictionary<string, BodyProto> bodyCatalogue, RobotSeedSummary summary)
	{
		var procedureSets = new[]
		{
			new ProcedureTarget("Humanoid", bodyCatalogue["Robot Humanoid"], HumanoidRobotOrganAliases, HumanoidArmTargets.Concat(HumanoidLegTargets).ToArray()),
			new ProcedureTarget("Quadruped", bodyCatalogue["Robot Quadruped"], NonSpeakingRobotOrganAliases, QuadrupedTargets),
			new ProcedureTarget("Insectoid", bodyCatalogue["Robot Insectoid"], NonSpeakingRobotOrganAliases, InsectoidTargets),
			new ProcedureTarget("Utility", bodyCatalogue["Robot Utility"], NonSpeakingRobotOrganAliases, UtilityTargets)
		};

		foreach (var target in procedureSets)
		{
			summary.ProceduresAdded += SeedProcedureSet(target);
		}
	}

	private int SeedProcedureSet(ProcedureTarget target)
	{
		var added = 0;
		added += AddProcedureIfMissing(
			$"Robot Diagnostics ({target.Label})",
			$"robot diagnostics {target.Label.ToLowerInvariant()}",
			"Robotics",
			SurgicalProcedureType.Triage,
			CheckType.TriageCheck,
			"diagnosing",
			"@ begin|begins robot diagnostics on $1",
			"This procedure performs a rapid diagnostic sweep of a robotic patient.",
			target.Body,
			"Robot Diagnostics",
			string.Empty,
			procedure =>
			{
				AddSurgicalProcedurePhase(procedure, 1, 10, null, "@ inspect|inspects $1 for obvious leaks, scorched panels, and fractured plating");
				AddSurgicalProcedurePhase(procedure, 2, 10, null, "@ test|tests actuator response and sensor status");
				AddSurgicalProcedurePhase(procedure, 3, 10, null, "@ complete|completes the diagnostic sweep");
			});
		added += AddProcedureIfMissing(
			$"Robot Maintenance Examination ({target.Label})",
			$"robot maintenance examination {target.Label.ToLowerInvariant()}",
			"Robotics",
			SurgicalProcedureType.DetailedExamination,
			CheckType.MedicalExaminationCheck,
			"examining",
			"@ begin|begins a maintenance examination of $1",
			"This procedure performs a more detailed maintenance inspection on a robotic patient.",
			target.Body,
			"Robot Diagnostics",
			string.Empty,
			procedure =>
			{
				AddSurgicalProcedurePhase(procedure, 1, 20, null, "@ inspect|inspects panel seams, bearings, and actuator housings");
				AddSurgicalProcedurePhase(procedure, 2, 20, null, "@ trace|traces power feeds and hydraulic lines with practiced care", ForcepsPlan());
				AddSurgicalProcedurePhase(procedure, 3, 20, null, "@ verify|verifies sensor calibration and chassis integrity");
			});
		added += AddProcedureIfMissing(
			$"Robot Exploratory Maintenance ({target.Label})",
			$"robot exploratory maintenance {target.Label.ToLowerInvariant()}",
			"Robotics",
			SurgicalProcedureType.ExploratorySurgery,
			CheckType.ExploratorySurgeryCheck,
			"exploring",
			"@ begin|begins exploratory maintenance on $1",
			"This procedure opens and inspects an inaccessible area of a robotic patient.",
			target.Body,
			"Robot Maintenance",
			string.Empty,
			procedure =>
			{
				AddSurgicalProcedurePhase(procedure, 1, 20, "exposed", "@ open|opens an access panel with $i1", ScalpelPlan());
				AddSurgicalProcedurePhase(procedure, 2, 25, null, "@ probe|probes wiring trunks, hydraulic lines, and component mounts with $i1", ForcepsPlan());
				AddSurgicalProcedurePhase(procedure, 3, 15, null, "@ assess|assesses the exposed internals");
			});
		added += AddProcedureIfMissing(
			$"Robot Leak Control ({target.Label})",
			$"robot leak control {target.Label.ToLowerInvariant()}",
			"Robotics",
			SurgicalProcedureType.TraumaControl,
			CheckType.TraumaControlSurgery,
			"controlling leaks",
			"@ begin|begins leak control on $1",
			"This procedure clamps hydraulic or oil leaks and stabilises damaged robotic internals.",
			target.Body,
			"Robot Maintenance",
			string.Empty,
			procedure =>
			{
				AddSurgicalProcedurePhase(procedure, 1, 20, null, "@ clamp|clamps leaking lines and isolates damaged channels with $i1", ClampPlan());
				AddSurgicalProcedurePhase(procedure, 2, 20, null, "@ pack|packs the damaged cavity and seals the worst leaks");
				AddSurgicalProcedurePhase(procedure, 3, 15, null, "@ confirm|confirms that the leak is under control");
			});
		added += AddProcedureIfMissing(
			$"Robot Chassis Closure ({target.Label})",
			$"robot chassis closure {target.Label.ToLowerInvariant()}",
			"Robotics",
			SurgicalProcedureType.InvasiveProcedureFinalisation,
			CheckType.InvasiveProcedureFinalisation,
			"closing",
			"@ begin|begins sealing $1's chassis",
			"This procedure closes and seals an opened robotic chassis.",
			target.Body,
			"Robot Maintenance",
			string.Empty,
			procedure =>
			{
				AddSurgicalProcedurePhase(procedure, 1, 15, null, "@ realign|realigns panel edges and internal runs");
				AddSurgicalProcedurePhase(procedure, 2, 20, null, "@ secure|secures the chassis and fasteners with $i1", ClampPlan());
				AddSurgicalProcedurePhase(procedure, 3, 20, null, "@ seal|seals and closes the chassis with careful final work", SutureNeedlePlan());
			});
		added += AddProcedureIfMissing(
			$"Robot Organ Extraction ({target.Label})",
			$"robot organ extraction {target.Label.ToLowerInvariant()}",
			"Robotics",
			SurgicalProcedureType.OrganExtraction,
			CheckType.OrganExtractionCheck,
			"extracting",
			"@ begin|begins extracting a robotic component from $1",
			"This procedure extracts a robot organ such as a power core or sensor array.",
			target.Body,
			"Robot Surgery",
			GetDefinitionForTargets(target.Body, target.Organs.ToArray()),
			procedure =>
			{
				AddSurgicalProcedurePhase(procedure, 1, 25, "exposed", "@ open|opens $1's {0} with $i1", ScalpelPlan());
				AddSurgicalProcedurePhase(procedure, 2, 25, "checkorgan", "@ disconnect|disconnects the {1} from its mounts with $i1", ForcepsPlan());
				AddSurgicalProcedurePhase(procedure, 3, 20, null, "@ extract|extracts the {1} from $1");
			});
		added += AddProcedureIfMissing(
			$"Robot Organ Replacement ({target.Label})",
			$"robot organ replacement {target.Label.ToLowerInvariant()}",
			"Robotics",
			SurgicalProcedureType.OrganTransplant,
			CheckType.OrganTransplantCheck,
			"replacing",
			"@ begin|begins replacing a robotic component in $1",
			"This procedure installs a compatible robot organ or replacement component.",
			target.Body,
			"Robot Surgery",
			GetDefinitionForTargets(target.Body, target.Organs.ToArray()),
			procedure =>
			{
				AddSurgicalProcedurePhase(procedure, 1, 25, "checkspace", "@ prepare|prepares $1's {0} to receive the new {1}", ScalpelPlan());
				AddSurgicalProcedurePhase(procedure, 2, 30, "checkorgan", "@ seat|seats the replacement {1} and connect|connects it with $i1", ForcepsPlan());
				AddSurgicalProcedurePhase(procedure, 3, 20, null, "@ stabilise|stabilises the replacement and secures its fittings", ClampPlan());
			});
		added += AddProcedureIfMissing(
			$"Robot Limb Detachment ({target.Label})",
			$"robot limb detachment {target.Label.ToLowerInvariant()}",
			"Robotics",
			SurgicalProcedureType.Amputation,
			CheckType.AmputationCheck,
			"detaching",
			"@ begin|begins detaching part of $1",
			"This procedure detaches a damaged robotic limb or other major assembly from the patient.",
			target.Body,
			"Robot Surgery",
			string.Empty,
			procedure =>
			{
				AddSurgicalProcedurePhase(procedure, 1, 25, "exposed", "@ open|opens the connection around $1's {0} with $i1", ScalpelPlan());
				AddSurgicalProcedurePhase(procedure, 2, 30, null, "@ cut|cuts through the limb assembly with $i1", BonesawPlan());
				AddSurgicalProcedurePhase(procedure, 3, 15, null, "@ remove|removes the detached assembly");
			});
		added += AddProcedureIfMissing(
			$"Robot Limb Reattachment ({target.Label})",
			$"robot limb reattachment {target.Label.ToLowerInvariant()}",
			"Robotics",
			SurgicalProcedureType.Replantation,
			CheckType.ReplantationCheck,
			"reattaching",
			"@ begin|begins reattaching a detached assembly to $1",
			"This procedure reattaches a detached robotic limb or assembly.",
			target.Body,
			"Robot Surgery",
			string.Empty,
			procedure =>
			{
				AddSurgicalProcedurePhase(procedure, 1, 20, "exposed", "@ clean|cleans and prepares the damaged mounts with $i1", ScalpelPlan());
				AddSurgicalProcedurePhase(procedure, 2, 25, null, "@ align|aligns the detached assembly to $1");
				AddSurgicalProcedurePhase(procedure, 3, 25, null, "@ secure|secures the reattached assembly and its connectors", SutureNeedlePlan());
			});

		return added;
	}
}
