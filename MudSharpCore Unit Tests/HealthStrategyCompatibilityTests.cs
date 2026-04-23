#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Health;
using MudSharp.Health.Strategies;
using System.Collections.Generic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class HealthStrategyCompatibilityTests
{
	[TestMethod]
	public void CanTransferBodyStateTo_SameHealthStateModel_ReturnsTrue()
	{
		var organicA = new TestHealthStrategy(HealthStateModel.Organic);
		var organicB = new TestHealthStrategy(HealthStateModel.Organic);

		Assert.IsTrue(organicA.CanTransferBodyStateTo(organicB));
		Assert.IsTrue(organicB.CanTransferBodyStateTo(organicA));
	}

	[TestMethod]
	public void CanTransferBodyStateTo_DifferentHealthStateModel_ReturnsFalse()
	{
		var organic = new TestHealthStrategy(HealthStateModel.Organic);
		var robot = new TestHealthStrategy(HealthStateModel.Robot);
		var construct = new TestHealthStrategy(HealthStateModel.Construct);
		var gameItem = new TestHealthStrategy(HealthStateModel.GameItem);

		Assert.IsFalse(organic.CanTransferBodyStateTo(robot));
		Assert.IsFalse(robot.CanTransferBodyStateTo(construct));
		Assert.IsFalse(construct.CanTransferBodyStateTo(gameItem));
		Assert.IsFalse(gameItem.CanTransferBodyStateTo(organic));
	}

	private sealed class TestHealthStrategy : BaseHealthStrategy
	{
		public TestHealthStrategy(HealthStateModel healthStateModel)
		{
			HealthStateModel = healthStateModel;
		}

		public override string HealthStrategyType => "Test";
		public override HealthStrategyOwnerType OwnerType => HealthStrategyOwnerType.Character;
		public override HealthStateModel HealthStateModel { get; }

		public override IHealthStrategy Clone(string name)
		{
			return new TestHealthStrategy(HealthStateModel);
		}

		public override IEnumerable<IWound> SufferDamage(IHaveWounds owner, IDamage damage, IBodypart bodypart)
		{
			return [];
		}

		public override HealthTickResult PerformHealthTick(IHaveWounds thing)
		{
			return HealthTickResult.None;
		}

		public override HealthTickResult EvaluateStatus(IHaveWounds thing)
		{
			return HealthTickResult.None;
		}

		public override string ReportConditionPrompt(IHaveWounds owner, PromptType type)
		{
			return string.Empty;
		}

		public override double MaxHP(IHaveWounds owner)
		{
			return 1.0;
		}

		protected override void SaveSubtypeDefinition(System.Xml.Linq.XElement root)
		{
		}
	}
}
