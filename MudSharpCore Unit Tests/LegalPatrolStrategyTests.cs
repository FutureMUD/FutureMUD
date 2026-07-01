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

		Assert.IsTrue(PatrolStrategyFactory.Strategies.Contains("ReactivePatrol"));
		Assert.IsTrue(PatrolStrategyFactory.Strategies.Contains("InvestigationPatrol"));
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
		StringAssert.Contains(source, "criminal.Combat?.Combatants.OfType<ICharacter>().Any(x => patrol.PatrolMembers.ContainsPhysicalInstance(x)) == true");
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
