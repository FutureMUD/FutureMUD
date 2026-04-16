#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Computers;
using MudSharp.GameItems;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SignalAutomationTests
{
	[ClassInitialize]
	public static void ClassInitialise(TestContext _)
	{
		FutureProgTestBootstrap.EnsureInitialised();
	}

	[TestMethod]
	public void MicrocontrollerLogicCompiler_CompilesExecutableLogic()
	{
		var (prog, error) = MicrocontrollerLogicCompiler.Compile(
			FutureProgTestBootstrap.Gameworld,
			"TestMicrocontroller",
			new[] { "Signal1", "ToggleValue" },
			@"if (@signal1 == 0)
	return 0
end if
return @togglevalue");

		Assert.IsNotNull(prog, error);
		Assert.AreEqual(string.Empty, error);
		Assert.AreEqual(0.0m, prog.ExecuteDecimal(0.0m, 0.0m, 7.5m));
		Assert.AreEqual(7.5m, prog.ExecuteDecimal(0.0m, 1.0m, 7.5m));
	}

	[TestMethod]
	public void MicrocontrollerLogicCompiler_RejectsDuplicateVariableNamesIgnoringCase()
	{
		var (prog, error) = MicrocontrollerLogicCompiler.Compile(
			FutureProgTestBootstrap.Gameworld,
			"DuplicateInputs",
			new[] { "Signal1", "signal1" },
			"return 0");

		Assert.IsNull(prog);
		StringAssert.Contains(error, "duplicate");
	}

	[TestMethod]
	public void MicrocontrollerLogicCompiler_RejectsInvalidVariableNames()
	{
		var (prog, error) = MicrocontrollerLogicCompiler.Compile(
			FutureProgTestBootstrap.Gameworld,
			"InvalidInputs",
			new[] { "1badname" },
			"return 0");

		Assert.IsNull(prog);
		StringAssert.Contains(error, "not valid");
	}

	[TestMethod]
	public void GameItemComponentManager_RegistersSignalAutomationComponentTypes()
	{
		var manager = new GameItemComponentManager();
		var primaryTypes = manager.PrimaryTypes.ToList();
		var helpTypes = manager.TypeHelpInfo.Select(x => x.Name).ToList();

		CollectionAssert.IsSubsetOf(
			new[] { "pushbutton", "toggleswitch", "microcontroller", "signallight", "electroniclock" },
			primaryTypes);
		CollectionAssert.IsSubsetOf(
			new[] { "PushButton", "ToggleSwitch", "Microcontroller", "SignalLight", "ElectronicLock" },
			helpTypes);
	}
}
