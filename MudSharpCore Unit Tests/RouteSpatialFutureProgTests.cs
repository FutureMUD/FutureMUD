#nullable enable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RouteSpatialFutureProgTests
{
	[TestMethod]
	public void RouteSpatialFunctions_RegisterCompleteReadOnlySurface()
	{
		FutureProgTestBootstrap.EnsureInitialised();

		var names = FutureProg.GetFunctionCompilerInformations()
			.Where(x => x.Category.EqualTo("RouteCells"))
			.Select(x => x.FunctionName.ToLowerInvariant())
			.ToHashSet();

		foreach (var expected in new[]
		         {
			         "isroutecell",
			         "routecelllength",
			         "routecelltopologyversion",
			         "routeposition",
			         "routeexactdistance",
			         "routeroomequivalentdistance",
			         "routerelativedirection",
			         "routenearestlandmark",
			         "routeportalaccessible"
		         })
		{
			Assert.IsTrue(names.Contains(expected), $"Missing FutureProg RouteCell function {expected}.");
		}
	}
}
