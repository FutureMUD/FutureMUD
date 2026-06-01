#nullable enable

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests.Economy;

[TestClass]
public class AuctionCommandSecurityTests
{
	[TestMethod]
	public void AuctionBuilderSubcommandsRequireAdministratorBeforeGenericBuilderDispatch()
	{
		var source = File.ReadAllText(GetSourcePath("MudSharpCore", "Commands", "Modules", "EconomyModule.cs"));
		var methodIndex = source.IndexOf("protected static void Auction(ICharacter actor, string command)", StringComparison.Ordinal);
		Assert.IsTrue(methodIndex > 0, "The player-facing auction command should exist.");

		var builderCaseIndex = source.IndexOf("case \"edit\":", methodIndex, StringComparison.Ordinal);
		Assert.IsTrue(builderCaseIndex > methodIndex, "The auction command should explicitly branch builder subcommands.");

		var builderDispatchIndex = source.IndexOf(
			"BuilderModule.GenericBuildingCommand(actor, ss.GetUndo(), EditableItemHelper.AuctionHelper);",
			builderCaseIndex,
			StringComparison.Ordinal);
		Assert.IsTrue(builderDispatchIndex > builderCaseIndex, "The auction command should still use the standard auction-house builder helper for admin subcommands.");

		var builderBranch = source[builderCaseIndex..builderDispatchIndex];
		StringAssert.Contains(builderBranch, "if (!actor.IsAdministrator())");
		StringAssert.Contains(builderBranch, "return;");
	}

	[TestMethod]
	public void AuctionHouseBuildingCommandRejectsNonAdministrators()
	{
		var source = File.ReadAllText(GetSourcePath("MudSharpCore", "Economy", "Auctions", "AuctionHouse.cs"));
		var methodIndex = source.IndexOf(
			"public bool BuildingCommand(ICharacter actor, StringStack command)",
			StringComparison.Ordinal);
		Assert.IsTrue(methodIndex > 0, "AuctionHouse should expose an editable-item BuildingCommand.");

		var switchIndex = source.IndexOf("switch (command.PopForSwitch())", methodIndex, StringComparison.Ordinal);
		Assert.IsTrue(switchIndex > methodIndex, "AuctionHouse BuildingCommand should dispatch builder subcommands.");

		var methodPreamble = source[methodIndex..switchIndex];
		StringAssert.Contains(methodPreamble, "if (!actor.IsAdministrator())");
		StringAssert.Contains(methodPreamble, "return false;");
	}

	private static string GetSourcePath(params string[] parts)
	{
		return Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts)));
	}
}
