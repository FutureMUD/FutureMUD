#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.RPG.Law;
using MudSharp.RPG.Law.PatrolStrategies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class LegalPatrolStrategyTests
{
	[TestMethod]
	public void PatrolStrategyFactory_ShouldExposeCrimeTargetedStrategies()
	{
		CollectionAssert.Contains(PatrolStrategyFactory.Strategies.ToList(), "ReactivePatrol");
		CollectionAssert.Contains(PatrolStrategyFactory.Strategies.ToList(), "InvestigationPatrol");
		CollectionAssert.Contains(PatrolStrategyFactory.Strategies.ToList(), "DoorDuties");

		Assert.IsTrue(PatrolStrategyFactory.Strategies.Contains("ReactivePatrol"));
		Assert.IsTrue(PatrolStrategyFactory.Strategies.Contains("InvestigationPatrol"));
		Assert.IsTrue(PatrolStrategyFactory.Strategies.Contains("DoorDuties"));
	}

	[TestMethod]
	public void IsViolentCrime_ShouldIncludeSeriousPersonalViolence()
	{
		Assert.IsTrue(CrimeTypes.AssaultWithADeadlyWeapon.IsViolentCrime());
		Assert.IsTrue(CrimeTypes.Mayhem.IsViolentCrime());
		Assert.IsTrue(CrimeTypes.Kidnapping.IsViolentCrime());
		Assert.IsFalse(CrimeTypes.TaxEvasion.IsViolentCrime());
	}

	[TestMethod]
	public void ReactiveEligibility_ShouldRequireRecentKnownUnenforcedViolentCrime()
	{
		var violent = CreateCrime(CrimeTypes.AssaultWithADeadlyWeapon, DateTime.UtcNow.AddMinutes(-2), known: true);
		var oldViolent = CreateCrime(CrimeTypes.AssaultWithADeadlyWeapon, DateTime.UtcNow.AddHours(-2), known: true);
		var nonViolent = CreateCrime(CrimeTypes.TaxEvasion, DateTime.UtcNow.AddMinutes(-2), known: true);

		Assert.IsTrue(ReactivePatrolStrategy.IsEligibleResponseCrime(violent.Object, TimeSpan.FromMinutes(30)));
		Assert.IsFalse(ReactivePatrolStrategy.IsEligibleResponseCrime(oldViolent.Object, TimeSpan.FromMinutes(30)));
		Assert.IsFalse(ReactivePatrolStrategy.IsEligibleResponseCrime(nonViolent.Object, TimeSpan.FromMinutes(30)));

		violent.SetupGet(x => x.HasBeenEnforced).Returns(true);
		Assert.IsFalse(ReactivePatrolStrategy.IsEligibleResponseCrime(violent.Object, TimeSpan.FromMinutes(30)));
	}

	[TestMethod]
	public void InvestigationEligibility_ShouldRequireMissingCharacteristicEvidence()
	{
		var character = new Mock<ICharacter>();
		var definition = new Mock<ICharacteristicDefinition>();
		character.SetupGet(x => x.CharacteristicDefinitions).Returns(new[] { definition.Object });

		var crime = CreateCrime(CrimeTypes.Fraud, DateTime.UtcNow, known: true);
		crime.SetupGet(x => x.Criminal).Returns(character.Object);
		crime.SetupGet(x => x.CriminalIdentityIsKnown).Returns(false);
		crime.SetupGet(x => x.CriminalCharacteristics).Returns(new Dictionary<ICharacteristicDefinition, ICharacteristicValue>());

		Assert.IsTrue(InvestigationPatrolStrategy.NeedsInvestigation(crime.Object));

		crime.SetupGet(x => x.CriminalIdentityIsKnown).Returns(true);
		Assert.IsTrue(InvestigationPatrolStrategy.NeedsInvestigation(crime.Object));

		var value = new Mock<ICharacteristicValue>();
		crime.SetupGet(x => x.CriminalCharacteristics)
		     .Returns(new Dictionary<ICharacteristicDefinition, ICharacteristicValue> { [definition.Object] = value.Object });
		Assert.IsFalse(InvestigationPatrolStrategy.NeedsInvestigation(crime.Object));

		crime.SetupGet(x => x.CriminalIdentityIsKnown).Returns(false);
		Assert.IsFalse(InvestigationPatrolStrategy.NeedsInvestigation(crime.Object));
	}

	[TestMethod]
	public void PatrolTickActiveEnforcement_DragCustody_ClearsCombatBeforePathingToJail()
	{
		string source = File.ReadAllText(GetCoreSourcePath("RPG", "Law", "PatrolStrategies", "PatrolStrategyBase.cs"));
		int detainedStart = source.IndexOf("// Is criminal detained by an enforcer?", StringComparison.Ordinal);
		int moveToPrison = source.IndexOf("// Move to prison", detainedStart, StringComparison.Ordinal);
		Assert.IsTrue(detainedStart >= 0);
		Assert.IsTrue(moveToPrison > detainedStart);

		string detainedBlock = source[detainedStart..moveToPrison];
		StringAssert.Contains(detainedBlock, "foreach (ICharacter member in patrol.PatrolMembers)");
		StringAssert.Contains(detainedBlock, "LeaveCombatIfAble(member);");
		StringAssert.Contains(detainedBlock, "if (!leader.CouldMove(false, null).Success)");
	}

	[TestMethod]
	public void PatrolTickActiveEnforcement_HelplessTarget_ShouldDragBeforeCombatBranch()
	{
		string source = File.ReadAllText(GetCoreSourcePath("RPG", "Law", "PatrolStrategies", "PatrolStrategyBase.cs"));
		int helplessCheck = source.IndexOf("TryStartDraggingHelplessCriminal(patrol, criminal)", StringComparison.Ordinal);
		int combatCheck = source.IndexOf("// Is criminal in combat with enforcers?", StringComparison.Ordinal);

		Assert.IsTrue(helplessCheck >= 0, "The active-enforcement loop should explicitly handle helpless criminals.");
		Assert.IsTrue(combatCheck > helplessCheck, "Helpless arrest targets must be dragged before the combat branch can re-engage them.");
		StringAssert.Contains(source, "private bool TryStartDraggingHelplessCriminal(IPatrol patrol, ICharacter criminal)");
		StringAssert.Contains(source, "LeaveCombatIfAble(member);");
		StringAssert.Contains(source, "EnforcementCustodyHelper.BeginDragging(random, criminal");
		StringAssert.Contains(source, "EnforcementCustodyHelper.ReleaseGrapplesAndCombatAgainst(criminal, localPatrolMembers);");
		int helperStart = source.IndexOf("private bool TryStartDraggingHelplessCriminal(IPatrol patrol, ICharacter criminal)", StringComparison.Ordinal);
		int helperEnd = source.IndexOf("protected virtual void PatrolTickActiveEnforcement(IPatrol patrol)", helperStart, StringComparison.Ordinal);
		string helperBlock = source[helperStart..helperEnd];
		Assert.IsFalse(helperBlock.Contains("criminal.Combat?.Combatants.OfType<ICharacter>().Any(x => patrol.PatrolMembers.ContainsPhysicalInstance(x)) == true", StringComparison.Ordinal));
	}

	[TestMethod]
	public void EnforcementCustodyHelper_ShouldConvertHelplessTargetsToDragCustody()
	{
		string source = File.ReadAllText(GetCoreSourcePath("RPG", "Law", "EnforcementCustodyHelper.cs"));

		StringAssert.Contains(source, "public static ICrime SelectArrestableCrime(ILegalAuthority authority, ICharacter criminal)");
		StringAssert.Contains(source, ".Where(x => x.Law.EnforcementStrategy.IsArrestable())");
		StringAssert.Contains(source, "public static Dragging BeginDragging(ICharacter dragger, ICharacter criminal, IEnumerable<ICharacter> helpers)");
		StringAssert.Contains(source, "var drag = new Dragging(dragger, null, criminal);");
		StringAssert.Contains(source, "public static void ReleaseGrapplesAndCombatAgainst(ICharacter criminal, IEnumerable<ICharacter> enforcers)");
		StringAssert.Contains(source, "RemoveAllEffects<IGrappling>");
		StringAssert.Contains(source, "RemoveAllEffects<ClinchEffect>");
		StringAssert.Contains(source, "LeaveCombatIfAble(criminal);");
	}

	[TestMethod]
	public void EnforcerAI_ShouldTakeIndependentCustodyOfHelplessCriminals()
	{
		string source = File.ReadAllText(GetCoreSourcePath("NPC", "AI", "EnforcerAI.cs"));

		StringAssert.Contains(source, "return TargetIncapacitated((ICharacter)arguments[1], ch);");
		StringAssert.Contains(source, "private bool TryHandleIndependentCustody(ICharacter enforcer, EnforcerEffect effect)");
		StringAssert.Contains(source, "enforcer.CombinedEffectsOfType<PatrolMemberEffect>().Any()");
		StringAssert.Contains(source, "TryBeginIndependentCustody(character, victim, effect)");
		StringAssert.Contains(source, "EnforcementCustodyHelper.BeginDragging(enforcer, criminal, Enumerable.Empty<ICharacter>())");
		StringAssert.Contains(source, "MoveIndependentCustodyToPrison(enforcer, effect, criminal, crime)");
		StringAssert.Contains(source, "PathSearch.PathIncludeUnlockableDoors(enforcer)");
		StringAssert.Contains(source, "TryHandleIndependentCustody(enforcer, effect)");
	}

	[TestMethod]
	public void AutomatedTrials_ShouldKeepJudgeAndProsecutorRolesSeparate()
	{
		string judgeSource = File.ReadAllText(GetCoreSourcePath("NPC", "AI", "JudgeAI.cs"));
		string prosecutorSource = File.ReadAllText(GetCoreSourcePath("RPG", "Law", "PatrolStrategies", "ProsectutorPatrolStrategy.cs"));

		StringAssert.Contains(judgeSource, "EnsureAutomatedProsecutor(trialEffect);");
		StringAssert.Contains(judgeSource, "private static bool IsAutomatedProsecutor(ICharacter character, ILegalAuthority authority)");
		StringAssert.Contains(judgeSource, "x.Patrol.PatrolStrategy.Name == \"Prosecutor\"");
		StringAssert.Contains(judgeSource, "trialEffect.Prosecutor ??= court.Characters.FirstOrDefault");
		Assert.IsFalse(judgeSource.Contains("trialEffect.Prosecutor ??= enforcer;", StringComparison.Ordinal));
		Assert.IsFalse(judgeSource.Contains("trialEffect.HandleArgueCommand(enforcer, false);", StringComparison.Ordinal));

		StringAssert.Contains(prosecutorSource, "trial.Prosecutor = patrol.PatrolLeader;");
		StringAssert.Contains(prosecutorSource, "CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(trial.Prosecutor, patrol.PatrolLeader)");
		StringAssert.Contains(prosecutorSource, "trial.HandleArgueCommand(patrol.PatrolLeader, false);");
	}

	[TestMethod]
	public void ExecutionPatrolStrategy_ShouldAcceptHelplessOrSubmittedCondemnedAsSecured()
	{
		string source = File.ReadAllText(GetCoreSourcePath("RPG", "Law", "PatrolStrategies", "ExecutionPatrolStrategy.cs"));

		StringAssert.Contains(source, "private bool CondemnedIsSecuredForExecution(IPatrol patrol)");
		StringAssert.Contains(source, "AllLimbsIneffectiveFromRestraint();");
		StringAssert.Contains(source, "Where(x => x.Reason == LimbIneffectiveReason.Restrained)");
		StringAssert.Contains(source, "SetStage(ExecutionPatrolStage.SubduingPrisoner);");
		StringAssert.Contains(source, "ReleaseCondemnedFromPatrolDrag(patrol);");
		Assert.IsFalse(source.Contains("if (_condemned.Body.EffectsOfType<RestraintEffect>().Any())", StringComparison.Ordinal));
	}

	[TestMethod]
	public void ExecutionPatrolStrategy_ShouldSupportSpokenNamePlaceholders()
	{
		string source = File.ReadAllText(GetCoreSourcePath("RPG", "Law", "PatrolStrategies", "ExecutionPatrolStrategy.cs"));

		StringAssert.Contains(source, "\"@ announce|announces, \\\"%condemned% has been sentenced to death by lawful authority.\\\"\"");
		StringAssert.Contains(source, "Use normal emote targets outside speech: $0 is the executioner and $1 is the condemned.");
		StringAssert.Contains(source, "private static string ExpandExecutionEmotePlaceholders");
		StringAssert.Contains(source, ".Replace(CondemnedPlaceholder, condemnedName, StringComparison.OrdinalIgnoreCase)");
		StringAssert.Contains(source, ".Replace(ExecutionerPlaceholder, executionerName, StringComparison.OrdinalIgnoreCase)");
	}

	[TestMethod]
	public void ExecutionPatrolStrategy_ShouldReportFinalCorpseForRecovery()
	{
		string source = File.ReadAllText(GetCoreSourcePath("RPG", "Law", "PatrolStrategies", "ExecutionPatrolStrategy.cs"));

		StringAssert.Contains(source, "private void ReportExecutionCorpseForRecovery(IPatrol patrol)");
		StringAssert.Contains(source, "LegalAuthority.ReportCorpseToLocalAuthority(Gameworld, corpseItem, patrol.PatrolLeader, out _);");
		StringAssert.Contains(source, "ReportExecutionCorpseForRecovery(patrol);");
	}

	[TestMethod]
	public void ExecutionPatrolStrategy_ShouldRemandAndDelayAfterFailedExecutionAttempts()
	{
		string source = File.ReadAllText(GetCoreSourcePath("RPG", "Law", "PatrolStrategies", "ExecutionPatrolStrategy.cs"));
		int maxAttemptCheck = source.IndexOf("if (_executionAttempts >= MaximumExecutionAttempts)", StringComparison.Ordinal);
		int killingAttempt = source.IndexOf("bool attempted = Method switch", maxAttemptCheck, StringComparison.Ordinal);

		Assert.IsTrue(maxAttemptCheck >= 0, "The killing loop should still enforce a maximum attempts guard.");
		Assert.IsTrue(killingAttempt > maxAttemptCheck, "The maximum attempts guard should run before another killing attempt.");
		StringAssert.Contains(source[maxAttemptCheck..killingAttempt], "HandleFailedExecutionAttempts(patrol);");
		StringAssert.Contains(source, "public string FailureEmote { get; private set; }");
		StringAssert.Contains(source, "private void HandleFailedExecutionAttempts(IPatrol patrol)");
		StringAssert.Contains(source, "DoEmote(patrol.PatrolLeader, FailureEmote, _condemned);");
		StringAssert.Contains(source, "_condemned.RemoveAllEffects<ExecutionPatrolNoQuit>(x => x.Patrol == patrol, fireRemovalAction: true);");
		StringAssert.Contains(source, "DelayAwaitingExecution(patrol.LegalAuthority);");
		StringAssert.Contains(source, "patrol.LegalAuthority.SendCharacterToHoldingCell(_condemned);");
		StringAssert.Contains(source, "private void DelayAwaitingExecution(ILegalAuthority authority)");
		StringAssert.Contains(source, "MudDateTime retryDate = CurrentLegalTime(authority) + MudTimeSpan.FromDays(1);");
		StringAssert.Contains(source, "effect.ExtendSentence(retryDate - effect.ExecutionDate);");
	}

	[TestMethod]
	public void ExecutionPatrolStrategy_ShouldUseLeaderLedEquipmentPreparation()
	{
		string source = File.ReadAllText(GetCoreSourcePath("RPG", "Law", "PatrolStrategies", "ExecutionPatrolStrategy.cs"));
		int leaderMove = source.IndexOf("if (patrol.PatrolLeader.Location != equipment)", StringComparison.Ordinal);
		int memberLoop = source.IndexOf("foreach (ICharacter member in patrol.PatrolMembers)", leaderMove, StringComparison.Ordinal);

		Assert.IsTrue(leaderMove >= 0, "Execution preparation should move the leader to equipment first.");
		Assert.IsTrue(memberLoop > leaderMove, "Member equipment orders should wait until the leader is at the equipment room.");
		StringAssert.Contains(source[leaderMove..memberLoop], "MoveCharacterTo(patrol.PatrolLeader, equipment, 25);");
		StringAssert.Contains(source[leaderMove..memberLoop], "return;");
	}

	[TestMethod]
	public void DoorDutiesPatrolStrategy_ShouldPrepareKeysAndEnableDoorGuardMode()
	{
		string source = File.ReadAllText(GetCoreSourcePath("RPG", "Law", "PatrolStrategies", "DoorDutiesPatrolStrategy.cs"));
		string effectSource = File.ReadAllText(GetCoreSourcePath("Effects", "Concrete", "PatrolDoorguardMode.cs"));
		string factorySource = File.ReadAllText(GetCoreSourcePath("RPG", "Law", "PatrolStrategyFactory.cs"));
		string patrolSource = File.ReadAllText(GetCoreSourcePath("RPG", "Law", "Patrol.cs"));
		string movementSource = File.ReadAllText(GetCoreSourcePath("NPC", "AI", "Strategies", "MovementStrategyFactory.cs"));

		StringAssert.Contains(factorySource, "\"DoorDuties\"");
		StringAssert.Contains(factorySource, "case \"door duties\":");
		StringAssert.Contains(source, "public override string Name => \"DoorDuties\";");
		StringAssert.Contains(source, "PrepareKeysForCells(member, dutyCells);");
		StringAssert.Contains(source, "member == patrol.PatrolLeader");
		StringAssert.Contains(source, "HasKeysForCells(patrol.PatrolLeader, dutyCells)");
		StringAssert.Contains(source, "new PatrolDoorguardMode(member)");
		StringAssert.Contains(source, "RemoveAllEffects<PatrolDoorguardMode>(fireRemovalAction: true)");
		StringAssert.Contains(source, "public override void HandlePatrolCompleted(IPatrol patrol)");
		StringAssert.Contains(source, "public override void HandlePatrolAborted(IPatrol patrol)");
		StringAssert.Contains(patrolSource, "HandlePatrolCompleted(this)");
		StringAssert.Contains(patrolSource, "HandlePatrolAborted(this)");
		StringAssert.Contains(movementSource, "AffectedBy<IDoorguardModeEffect>()");
		Assert.IsFalse(movementSource.Contains("AffectedBy<DoorguardMode>()", StringComparison.Ordinal));
		StringAssert.Contains(source, "PathSearch.PathIncludeUnlockableDoors");
		StringAssert.Contains(source, "UseKeys = true");
		StringAssert.Contains(source, "OpenDoors = true");
		StringAssert.Contains(source, "DisableDoorGuardMode(patrol);");
		StringAssert.Contains(effectSource, "IDoorguardModeEffect");
		Assert.IsFalse(effectSource.Contains("SavingEffect => true", StringComparison.Ordinal));
	}

	[TestMethod]
	public void PatrolStrategies_ShouldShareKeyPreparationArchitecture()
	{
		string baseSource = File.ReadAllText(GetCoreSourcePath("RPG", "Law", "PatrolStrategies", "PatrolStrategyBase.cs"));
		string executionSource = File.ReadAllText(GetCoreSourcePath("RPG", "Law", "PatrolStrategies", "ExecutionPatrolStrategy.cs"));

		StringAssert.Contains(baseSource, "protected static IEnumerable<IKey> AccessibleKeys(ICharacter member)");
		StringAssert.Contains(baseSource, "protected static bool PrepareKeysForLocks(ICharacter member, IEnumerable<ILock> locks)");
		StringAssert.Contains(baseSource, "new InventoryPlanActionHold(member.Gameworld, 0, 0,");
		StringAssert.Contains(baseSource, "protected static bool PrepareKeysForCells(ICharacter member, IEnumerable<ICell> cells)");
		StringAssert.Contains(baseSource, "protected static void PrepareInventoryPlan(ICharacter member, IInventoryPlanTemplate template)");
		StringAssert.Contains(executionSource, "public bool RequireKeysForRetrieval { get; private set; }");
		StringAssert.Contains(executionSource, "#3keys [on|off]#0");
		StringAssert.Contains(executionSource, "PrepareRetrievalKeys(member);");
		StringAssert.Contains(executionSource, "private bool RetrievalKeysReady(IPatrol patrol)");
		StringAssert.Contains(executionSource, "new XAttribute(\"keys\", RequireKeysForRetrieval)");
		StringAssert.Contains(executionSource, "bool.TryParse(root.Attribute(\"keys\")?.Value, out bool keys)");
	}

	[TestMethod]
	public void CorpseReporting_ShouldUseSharedLocalAuthorityMechanism()
	{
		string legalAuthoritySource = File.ReadAllText(GetCoreSourcePath("RPG", "Law", "LegalAuthority.CorpseRecovery.cs"));
		string crimeModuleSource = File.ReadAllText(GetCoreSourcePath("Commands", "Modules", "CrimeModule.cs"));

		StringAssert.Contains(legalAuthoritySource, "public static ICorpseRecoveryReport ReportCorpseToLocalAuthority(IFuturemud gameworld, IGameItem corpse,");
		StringAssert.Contains(legalAuthoritySource, "corpse?.GetItemType<ICorpse>()");
		StringAssert.Contains(legalAuthoritySource, "corpseComponent.RepresentsFinalCharacterDeath");
		StringAssert.Contains(legalAuthoritySource, "gameworld.LegalAuthorities.FirstOrDefault(x => x.EnforcementZones.Contains(sourceCell.Zone))");
		StringAssert.Contains(legalAuthoritySource, "Estate.DetermineZone(gameworld, sourceCell)");
		StringAssert.Contains(legalAuthoritySource, "authority.ActiveCorpseRecoveryReport(corpse) != null");
		StringAssert.Contains(crimeModuleSource, "LegalAuthority.ReportCorpseToLocalAuthority(actor.Gameworld, corpseItem, actor, out string errorMessage)");
	}

	[TestMethod]
	public void EnforcerAI_ShouldAutomaticallyReportVisibleCorpsesWhileOnDuty()
	{
		string source = File.ReadAllText(GetCoreSourcePath("NPC", "AI", "EnforcerAI.cs"));

		StringAssert.Contains(source, "private void ReportVisibleCorpses(ICharacter enforcer)");
		StringAssert.Contains(source, "enforcer.Location.LayerGameItems(enforcer.RoomLayer)");
		StringAssert.Contains(source, ".Where(x => enforcer.CanSee(x))");
		StringAssert.Contains(source, "x.GetItemType<ICorpse>() is { RepresentsFinalCharacterDeath: true }");
		StringAssert.Contains(source, "MudSharp.RPG.Law.LegalAuthority.ReportCorpseToLocalAuthority(Gameworld, corpseItem, enforcer, out _) != null");
		StringAssert.Contains(source, "new Emote(\"@ report|reports a corpse to the authorities.\",");
		StringAssert.Contains(source, "ReportVisibleCorpses(enforcer);");
	}

	[TestMethod]
	public void LegalStatus_ShouldSurfaceLegalSetupAndEquipmentIssues()
	{
		string source = File.ReadAllText(GetCoreSourcePath("Commands", "Modules", "LegalModule.cs"));

		Assert.IsTrue(source.Contains("[PlayerCommand(\"LegalStatus\", \"legalstatus\", \"lawstatus\")]", StringComparison.Ordinal));
		Assert.IsTrue(source.Contains("case \"status\":", StringComparison.Ordinal));
		Assert.IsTrue(source.Contains("private sealed record LegalSetupIssue", StringComparison.Ordinal));
		Assert.IsTrue(source.Contains("BuildLegalStatusReport(actor, authorities, actor.IsAdministrator())", StringComparison.Ordinal));
		Assert.IsTrue(source.Contains("GetLegalAuthoritiesForLegalStatus", StringComparison.Ordinal));
		Assert.IsTrue(source.Contains("x.GetEnforcementAuthority(actor) is not null", StringComparison.Ordinal));
		Assert.IsTrue(source.Contains("WhyCannotStaffPatrolRoute", StringComparison.Ordinal));
		Assert.IsTrue(source.Contains("route.PatrolStrategy.SelectEnforcers(route, pool, requirement.Value)", StringComparison.Ordinal));
		Assert.IsTrue(source.Contains("No Judge patrol route is configured", StringComparison.Ordinal));
		Assert.IsTrue(source.Contains("No Prosecutor patrol route is configured", StringComparison.Ordinal));
		Assert.IsTrue(source.Contains("No Sheriff patrol route is configured", StringComparison.Ordinal));
		Assert.IsTrue(source.Contains("no ExecutionPatrol route is configured", StringComparison.Ordinal));
		Assert.IsTrue(source.Contains("IsExecutionMeleeWeapon", StringComparison.Ordinal));
		Assert.IsTrue(source.Contains("ComponentsInCell<IInject>(equipment, actor)", StringComparison.Ordinal));
		Assert.IsTrue(source.Contains("ComponentsInCell<IRestraint>(equipment, actor).Concat(ComponentsInCell<IRestraint>(executionLocation, actor))", StringComparison.Ordinal));
		Assert.IsTrue(source.Contains("strategy.DrugId <= 0 || actor.Gameworld.Drugs.Get(strategy.DrugId) is null", StringComparison.Ordinal));
		Assert.IsTrue(source.Contains("ExecutionRetrievalKeyIssues", StringComparison.Ordinal));
		Assert.IsTrue(source.Contains("DoorDutyKeyIssues", StringComparison.Ordinal));
		Assert.IsTrue(source.Contains("LocksMissingKeys", StringComparison.Ordinal));
		Assert.IsTrue(source.Contains("AccessibleStockItems", StringComparison.Ordinal));
		Assert.IsFalse(source.Contains("SelectMany(x => x.DeepItems)", StringComparison.Ordinal));
		Assert.IsTrue(source.Contains("legalstatus <legal authority>", StringComparison.Ordinal));
	}

	[TestMethod]
	public void PatrolRouteDiagnostics_ShouldExposeReasonWhenInactiveRouteCannotBegin()
	{
		string routeInterfaceSource = File.ReadAllText(GetLibrarySourcePath("RPG", "Law", "IPatrolRoute.cs"));
		string configurableStrategySource = File.ReadAllText(GetLibrarySourcePath("RPG", "Law", "IConfigurablePatrolStrategy.cs"));
		string routeSource = File.ReadAllText(GetCoreSourcePath("RPG", "Law", "PatrolRoute.cs"));
		string executionStrategySource = File.ReadAllText(GetCoreSourcePath("RPG", "Law", "PatrolStrategies", "ExecutionPatrolStrategy.cs"));
		string legalModuleSource = File.ReadAllText(GetCoreSourcePath("Commands", "Modules", "LegalModule.cs"));

		StringAssert.Contains(routeInterfaceSource, "string WhyCannotBeginPatrol();");
		StringAssert.Contains(configurableStrategySource, "string WhyCannotBegin(IPatrolRoute patrol)");
		StringAssert.Contains(routeSource, "public string WhyCannotBeginPatrol()");
		StringAssert.Contains(routeSource, "crime-targeted routes wait for a matching reported crime");
		StringAssert.Contains(routeSource, "the start patrol prog returned false");
		StringAssert.Contains(executionStrategySource, "there is no due condemned prisoner for this authority");
		StringAssert.Contains(legalModuleSource, "Reason: {whyCannotBegin.ColourError()}");
	}

	private static Mock<ICrime> CreateCrime(CrimeTypes crimeType, DateTime realTime, bool known)
	{
		var law = new Mock<ILaw>();
		law.SetupGet(x => x.CrimeType).Returns(crimeType);
		law.SetupGet(x => x.EnforcementStrategy).Returns(EnforcementStrategy.ArrestAndDetain);
		law.SetupGet(x => x.EnforcementPriority).Returns(10);

		var cell = new Mock<MudSharp.Construction.ICell>();
		var crime = new Mock<ICrime>();
		crime.SetupGet(x => x.IsKnownCrime).Returns(known);
		crime.SetupGet(x => x.HasBeenFinalised).Returns(false);
		crime.SetupGet(x => x.HasBeenEnforced).Returns(false);
		crime.SetupGet(x => x.CrimeLocation).Returns(cell.Object);
		crime.SetupGet(x => x.RealTimeOfCrime).Returns(realTime);
		crime.SetupGet(x => x.Law).Returns(law.Object);
		return crime;
	}

	private static string GetCoreSourcePath(params string[] segments)
	{
		return Path.GetFullPath(Path.Combine(
			new[]
			{
				AppContext.BaseDirectory,
				"..",
				"..",
				"..",
				"..",
				"MudSharpCore"
			}.Concat(segments).ToArray()));
	}

	private static string GetLibrarySourcePath(params string[] segments)
	{
		return Path.GetFullPath(Path.Combine(
			new[]
			{
				AppContext.BaseDirectory,
				"..",
				"..",
				"..",
				"..",
				"FutureMUDLibrary"
			}.Concat(segments).ToArray()));
	}
}
