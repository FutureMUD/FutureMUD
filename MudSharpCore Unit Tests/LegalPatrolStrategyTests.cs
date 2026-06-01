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
}
