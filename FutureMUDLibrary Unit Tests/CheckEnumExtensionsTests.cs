using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CheckEnumExtensionsTests
{
	[TestMethod]
	public void IsPhysicalActivityCheck_TreatsElectricalWorkAsPhysicalButNotProgramming()
	{
		Assert.IsTrue(CheckType.InstallElectricalComponentCheck.IsPhysicalActivityCheck());
		Assert.IsTrue(CheckType.ConfigureElectricalComponentCheck.IsPhysicalActivityCheck());
		Assert.IsFalse(CheckType.ProgrammingComponentCheck.IsPhysicalActivityCheck());
	}
}
