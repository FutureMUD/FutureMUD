using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Magic;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicResourceCapacityTests
{
	[DataTestMethod]
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
}
