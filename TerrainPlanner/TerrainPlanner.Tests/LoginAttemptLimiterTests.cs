using TerrainPlanner.Server.Authentication;

namespace TerrainPlanner.Tests;

[TestClass]
public class LoginAttemptLimiterTests
{
	[TestMethod]
	public void LimitsByAddressAndNormalizedAccountName()
	{
		var limiter = new LoginAttemptLimiter();
		for (var attempt = 0; attempt < 5; attempt++)
		{
			Assert.IsTrue(limiter.TryAcquire("192.0.2.10", attempt % 2 == 0 ? " Builder " : "builder"));
		}

		Assert.IsFalse(limiter.TryAcquire("192.0.2.10", "BUILDER"));
		Assert.IsFalse(limiter.TryAcquire("192.0.2.11", "builder"), "The account-name limit must span addresses.");
		Assert.IsFalse(limiter.TryAcquire("192.0.2.10", "different-builder"), "The address limit must span account names.");
		Assert.IsTrue(limiter.TryAcquire("192.0.2.12", "different-builder"));
	}

	[TestMethod]
	public void SuccessfulLoginResetClearsTheWindow()
	{
		var limiter = new LoginAttemptLimiter();
		for (var attempt = 0; attempt < 6; attempt++)
		{
			limiter.TryAcquire("192.0.2.10", "builder");
		}

		limiter.Reset("192.0.2.10", " BUILDER ");

		Assert.IsTrue(limiter.TryAcquire("192.0.2.10", "builder"));
	}
}
