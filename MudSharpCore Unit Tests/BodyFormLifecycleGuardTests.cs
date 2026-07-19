#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using ConcreteBody = MudSharp.Body.Implementations.Body;
using ConcreteCharacter = MudSharp.Character.Character;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BodyFormLifecycleGuardTests
{
	[TestMethod]
	public void BodyProcessStarters_DormantForm_DoNotTouchRuntimeSchedulers()
	{
		var body = TestObjectFactory.CreateUninitialized<ConcreteBody>();
		var activeBody = new Mock<IBody>();
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.CurrentBody).Returns(activeBody.Object);
		actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		body.Actor = actor.Object;

		body.StartHealthTick();
		body.StartStaminaTick();
		body.CheckDrugTick();
		body.CheckHealthStatus();
		body.DoBreathing();
	}

	[TestMethod]
	public void BodyProcessStarters_DeadCurrentBody_DoNotTouchRuntimeSchedulers()
	{
		var body = TestObjectFactory.CreateUninitialized<ConcreteBody>();
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.CurrentBody).Returns(body);
		actor.SetupGet(x => x.State).Returns(CharacterState.Dead);
		body.Actor = actor.Object;

		body.StartHealthTick();
		body.StartStaminaTick();
		body.CheckDrugTick();
		body.CheckHealthStatus();
		body.DoBreathing();
	}

	[TestMethod]
	public void CharacterNeedsHeartbeat_StasisCharacter_DoesNotTouchNeedsModel()
	{
		var character = TestObjectFactory.CreateUninitialized<ConcreteCharacter>();
		typeof(ConcreteCharacter)
			.GetField("_state", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(character, CharacterState.Stasis);

		character.StartNeedsHeartbeat();
		character.NeedsHeartbeat();
	}

	[TestMethod]
	public void BodySwitchBloodRatio_UsesPercentageRatherThanLitres()
	{
		Assert.AreEqual(0.5, ConcreteBody.BloodVolumeRatioForFormSwitch(0.25, 0.5), 0.000001);
		Assert.AreEqual(1.0, ConcreteBody.BloodVolumeRatioForFormSwitch(10.0, 5.0), 0.000001);
		Assert.AreEqual(0.0, ConcreteBody.BloodVolumeRatioForFormSwitch(-1.0, 5.0), 0.000001);
		Assert.AreEqual(1.0, ConcreteBody.BloodVolumeRatioForFormSwitch(0.0, 0.0), 0.000001);
	}

	[TestMethod]
	public void EquivalentAgeForLifeStage_MapsAdultProgressToTargetRace()
	{
		var human = CreateRaceWithAgeThresholds(3, 10, 16, 21, 55, 75).Object;
		var dog = CreateRaceWithAgeThresholds(1, 2, 3, 4, 10, 14).Object;

		Assert.AreEqual(7, ConcreteCharacter.EquivalentAgeForLifeStage(human, 38, dog));
	}

	[TestMethod]
	public void EquivalentAgeForLifeStage_PreservesAgeForSameRace()
	{
		var human = CreateRaceWithAgeThresholds(3, 10, 16, 21, 55, 75).Object;

		Assert.AreEqual(38, ConcreteCharacter.EquivalentAgeForLifeStage(human, 38, human));
	}

	private static Mock<IRace> CreateRaceWithAgeThresholds(int child, int youth, int youngAdult, int adult, int elder,
		int venerable)
	{
		var thresholds = new Dictionary<AgeCategory, int>
		{
			[AgeCategory.Baby] = 0,
			[AgeCategory.Child] = child,
			[AgeCategory.Youth] = youth,
			[AgeCategory.YoungAdult] = youngAdult,
			[AgeCategory.Adult] = adult,
			[AgeCategory.Elder] = elder,
			[AgeCategory.Venerable] = venerable
		};
		var race = new Mock<IRace>();
		race.Setup(x => x.MinimumAgeForCategory(It.IsAny<AgeCategory>()))
		    .Returns((AgeCategory category) => thresholds[category]);
		race.Setup(x => x.AgeCategory(It.IsAny<int>()))
		    .Returns((int age) =>
		    {
			    if (age >= venerable)
			    {
				    return AgeCategory.Venerable;
			    }

			    if (age >= elder)
			    {
				    return AgeCategory.Elder;
			    }

			    if (age >= adult)
			    {
				    return AgeCategory.Adult;
			    }

			    if (age >= youngAdult)
			    {
				    return AgeCategory.YoungAdult;
			    }

			    if (age >= youth)
			    {
				    return AgeCategory.Youth;
			    }

			    if (age >= child)
			    {
				    return AgeCategory.Child;
			    }

			    return AgeCategory.Baby;
		    });
		race.Setup(x => x.SameRace(It.IsAny<IRace>()))
		    .Returns((IRace other) => ReferenceEquals(other, race.Object));
		return race;
	}

}
