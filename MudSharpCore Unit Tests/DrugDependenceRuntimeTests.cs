#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConcreteBody = MudSharp.Body.Implementations.Body;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DrugDependenceRuntimeTests
{
	[TestMethod]
	public void DependenceRecordRequiresHeartbeat_TreatsStoredExposureAsPendingWork()
	{
		Assert.IsFalse(ConcreteBody.DependenceRecordRequiresHeartbeat(0.0, 0.0, 0.0));
		Assert.IsTrue(ConcreteBody.DependenceRecordRequiresHeartbeat(1.0, 0.0, 0.0));
		Assert.IsTrue(ConcreteBody.DependenceRecordRequiresHeartbeat(0.0, 1.0, 0.0));
		Assert.IsTrue(ConcreteBody.DependenceRecordRequiresHeartbeat(0.0, 0.0, 1.0));
	}

	[TestMethod]
	public void DependenceActiveDoseGainForHeartbeat_DoesNotScaleByOfflineElapsedTime()
	{
		var oneHeartbeat = ConcreteBody.DependenceActiveDoseGainForHeartbeat(1.5, 0.4,
			TimeSpan.FromSeconds(10.0));
		var afterOfflineTime = ConcreteBody.DependenceActiveDoseGainForHeartbeat(1.5, 0.4,
			TimeSpan.FromDays(30.0));

		Assert.AreEqual(0.6, oneHeartbeat, 0.000001);
		Assert.AreEqual(oneHeartbeat, afterOfflineTime, 0.000001);
	}
}
