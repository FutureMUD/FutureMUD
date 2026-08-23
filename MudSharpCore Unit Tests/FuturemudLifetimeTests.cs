#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class FuturemudLifetimeTests
{
	[TestMethod]
	public void Dispose_PartiallyInitialisedInstance_DoesNotThrow()
	{
		var gameworld = TestObjectFactory.CreateUninitialized<Futuremud>();

		gameworld.Dispose();
	}
}
