using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Effects;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicResourceCapacityTests
{
	[TestMethod]
	[DataRow(0.0, true)]
	[DataRow(42.0, true)]
	[DataRow(-1.0, false)]
	[DataRow(double.NaN, false)]
	[DataRow(double.PositiveInfinity, false)]
	public void TryGetCap_LegacyCapContract_ValidatesWithoutMutatingHolder(double cap, bool valid)
	{
		var holder = new Mock<IHaveMagicResource>(MockBehavior.Strict);
		var resource = new Mock<IMagicResource>(); resource.Setup(x => x.ResourceCap(holder.Object)).Returns(cap);
		Assert.AreEqual(valid, MagicResourceCapacity.TryGetCap(resource.Object, holder.Object, out var actual, out var error));
		if (valid) { Assert.AreEqual(cap, actual); Assert.IsNull(error); }
		else Assert.IsFalse(string.IsNullOrEmpty(error));
		holder.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void TryGetCap_EvaluationException_IsDiagnosticRatherThanAZeroCapacitySuccess()
	{
		var holder = new Mock<IHaveMagicResource>(); var resource = new Mock<IMagicResource>();
		resource.Setup(x => x.ResourceCap(holder.Object)).Throws(new InvalidOperationException("missing attribute"));
		Assert.IsFalse(MagicResourceCapacity.TryGetCap(resource.Object, holder.Object, out _, out var error));
		StringAssert.Contains(error, "missing attribute");
	}

	[TestMethod]
	[DataRow(100.0, -10.0, 0.5, true, 45.0)]
	[DataRow(100.0, -110.0, 1.0, true, 0.0)]
	[DataRow(-10.0, 20.0, 1.0, false, 0.0)]
	[DataRow(100.0, double.NaN, 1.0, false, 0.0)]
	[DataRow(100.0, 0.0, -1.0, false, 0.0)]
	public void TryGetCap_AppliesCurrentBodyDrugCapacityBonusAndMultiplier(double baseCap, double bonus, double multiplier, bool valid, double expected)
	{
		var effect = new Mock<IMagicResourceCapacityBonusEffect>();
		effect.SetupGet(x => x.MagicResourceId).Returns(42);
		effect.SetupGet(x => x.MagicResourceCapacityBonus).Returns(bonus);
		effect.SetupGet(x => x.MagicResourceCapacityMultiplier).Returns(multiplier);
		var actor = new Mock<ICharacter>();
		var body = new Mock<IBody>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		body.Setup(x => x.EffectsOfType<IMagicResourceCapacityBonusEffect>(It.IsAny<Predicate<IMagicResourceCapacityBonusEffect>>()))
			.Returns(new[] { effect.Object });
		var resource = new Mock<IMagicResource>();
		resource.SetupGet(x => x.Id).Returns(42);
		resource.Setup(x => x.ResourceCap(actor.Object)).Returns(baseCap);

		Assert.AreEqual(valid, MagicResourceCapacity.TryGetCap(resource.Object, actor.Object, out var cap, out var error));
		if (valid)
		{
			Assert.AreEqual(expected, cap);
			Assert.IsNull(error);
		}
		else Assert.IsNotNull(error);
	}
}
