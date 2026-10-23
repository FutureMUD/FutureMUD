#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands;
using MudSharp.Commands.Helpers;
using MudSharp.Commands.Modules;
using MudSharp.Commands.Trees;
using MudSharp.Economy;
using MudSharp.Economy.Markets;
using MudSharp.Framework;
using MudSharp.Help;
using MudSharp.PerceptionEngine;
using MudSharp.FutureProg;
using MudSharp.RPG.Law;

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
public class BuilderDiscoveryTests
{
	[DataTestMethod]
	[DataRow("+12", 1L)]
	[DataRow("-12", 12L)]
	public void CatalogueFilters_NumericKeywordsRetainInclusionAndExclusionMeaning(string input, long expectedId)
	{
		var items = new[]
		{
			Mock.Of<IFrameworkItem>(x => x.Id == 1 && x.Name == "District12"),
			Mock.Of<IFrameworkItem>(x => x.Id == 12 && x.Name == "Elsewhere")
		};
		Assert.IsTrue(BuilderListFilter.TryApply(items, new StringStack(input), out var matches, out var error), error);
		Assert.AreEqual(expectedId, matches.Single().Id);
	}

	[DataTestMethod]
	[DataRow("+port -closed", "1")]
	[DataRow("zone \"South Coast\"", "1,2")]
	[DataRow("zone 10 -closed", "1")]
	[DataRow("2", "2")]
	[DataRow("+missing", "")]
	public void CatalogueFilters_ComposeWithoutChangingSource(string input, string expectedIds)
	{
		var south = Mock.Of<IEconomicZone>(x => x.Id == 10 && x.Name == "South Coast");
		var north = Mock.Of<IEconomicZone>(x => x.Id == 20 && x.Name == "North Coast");
		var markets = new[]
		{
			Mock.Of<IMarket>(x => x.Id == 1 && x.Name == "Port" && x.EconomicZone == south),
			Mock.Of<IMarket>(x => x.Id == 2 && x.Name == "Port Closed" && x.EconomicZone == south),
			Mock.Of<IMarket>(x => x.Id == 3 && x.Name == "Hill" && x.EconomicZone == north)
		};
		Assert.IsTrue(BuilderListFilter.TryApply(markets, new StringStack(input), out var matches, out var error,
			item => new[] { item.Name, item.EconomicZone.Name },
			new Dictionary<string, Func<IMarket, string, bool>>
			{ ["zone"] = (item, value) => BuilderListFilter.IsIdOrName(item.EconomicZone, value) }), error);
		Assert.AreEqual(expectedIds, string.Join(",", matches.Select(x => x.Id)));
		Assert.AreEqual(3, markets.Length);
	}

	[DataTestMethod]
	[DataRow("bogus")]
	[DataRow("+")]
	[DataRow("- ")]
	[DataRow("\"\"")]
	[DataRow("zone")]
	[DataRow("zone \"\"")]
	public void CatalogueFilters_InvalidInputNeverReturnsUnfilteredData(string input)
	{
		var item = Mock.Of<IFrameworkItem>(x => x.Id == 1 && x.Name == "Port");
		Assert.IsFalse(BuilderListFilter.TryApply(new[] { item }, new StringStack(input), out var matches, out _,
			fields: new Dictionary<string, Func<IFrameworkItem, string, bool>> { ["zone"] = (_, _) => true }));
		Assert.AreEqual(0, matches.Count);
	}

	[TestMethod]
	public void MarketList_AppliesZoneAndKeywordFiltersThroughCommandHandler()
	{
		var world = BuilderWorkflowRegressionTests.World();
		var zone = Mock.Of<IEconomicZone>(x => x.Id == 10 && x.Name == "South Coast");
		var markets = new All<IMarket>();
		markets.Add(Mock.Of<IMarket>(x => x.Id == 1 && x.Name == "Port" && x.EconomicZone == zone));
		markets.Add(Mock.Of<IMarket>(x => x.Id == 2 && x.Name == "Closed" && x.EconomicZone == zone));
		world.SetupGet(x => x.Markets).Returns(markets);
		var actor = BuilderWorkflowRegressionTests.Actor(world.Object);
		var output = Capture(actor);
		BuilderWorkflowRegressionTests.Invoke(typeof(EconomyModule), "MarketList", actor.Object, new StringStack("zone \"South Coast\" -closed"));
		StringAssert.Contains(output.Single(), "Port");
		Assert.IsFalse(output.Single().Contains("Closed"));
	}

	[DataTestMethod]
	[DataRow(false, 120)]
	[DataRow(true, 120)]
	[DataRow(true, 40)]
	public void HelpAlias_CommandRegistrationLookupAndVisibilityRemainConsistent(bool administrator, int width)
	{
		var module = new DiscoveryTestModule();
		var help = (CommandHelpInfo)module.Commands.TCommands["zz"].HelpInfo!;
		Assert.AreEqual("Discovery", help.Subsystem);
		Assert.AreEqual("Building", help.Capability);
		Assert.AreEqual(PermissionLevel.Admin, help.PermissionRequired);
		Assert.AreEqual("discoveryexample", help.CommandName);
		var world = BuilderWorkflowRegressionTests.World();
		world.SetupGet(x => x.Helpfiles).Returns(new All<IHelpfile>());
		var actor = BuilderWorkflowRegressionTests.Actor(world.Object);
		actor.SetupGet(x => x.InnerLineFormatLength).Returns(width);
		actor.SetupGet(x => x.LineFormatLength).Returns(width);
		actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(administrator);
		var manager = new Mock<ICharacterCommandManager>();
		manager.SetupGet(x => x.CommandHelpInfos).Returns(new[] { help });
		var tree = new Mock<ICharacterCommandTree>(); tree.SetupGet(x => x.Commands).Returns(manager.Object);
		world.Setup(x => x.RetrieveAppropriateCommandTree(actor.Object)).Returns(tree.Object);
		var output = Capture(actor);
		BuilderWorkflowRegressionTests.Invoke(typeof(HelpModule), "Help", actor.Object, "help ZZ");
		Assert.AreEqual(administrator, output.Single().Contains("builder instructions"));
		Assert.AreEqual(!administrator, output.Single().Contains("No help files matched"));
	}

	[TestMethod]
	public void HelpEditorList_RetainsRestrictedCatalogueEntriesWithoutExposingReaderHelp()
	{
		var world = BuilderWorkflowRegressionTests.World();
		var actor = BuilderWorkflowRegressionTests.Actor(world.Object);
		actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(true);
		var prog = Mock.Of<IFutureProg>(x => x.Id == 1 && x.Name == "Restricted" && x.FunctionName == "Restricted");
		var helpfile = Mock.Of<IHelpfile>(x => x.Id == 1 && x.Name == "Hidden Topic" && x.Category == "Building" &&
			x.Subcategory == "Notes" && x.TagLine == "Restricted notes" && x.Rule == prog && !x.CanView(actor.Object));
		var helpfiles = new All<IHelpfile>(); helpfiles.Add(helpfile);
		world.SetupGet(x => x.Helpfiles).Returns(helpfiles);
		var output = Capture(actor);
		BuilderWorkflowRegressionTests.Invoke(typeof(HelpModule), "HEditList", actor.Object, new StringStack("prog 1"));
		StringAssert.Contains(output.Single(), "Hidden Topic");
		Assert.AreEqual(0, HelpModule.FindHelpMatches([helpfile], actor.Object, "", "Hidden Topic").Count);
	}

	[TestMethod]
	public void LegalClassList_ConsumesQuotedAuthorityBeforeFilteringItsChildren()
	{
		var world = BuilderWorkflowRegressionTests.World();
		var authority = new Mock<ILegalAuthority>();
		authority.SetupGet(x => x.Id).Returns(1); authority.SetupGet(x => x.Name).Returns("City Law");
		authority.SetupGet(x => x.LegalClasses).Returns(new[]
		{
			Mock.Of<ILegalClass>(x => x.Id == 1 && x.Name == "Citizens"),
			Mock.Of<ILegalClass>(x => x.Id == 2 && x.Name == "Visitors")
		});
		var authorities = new All<ILegalAuthority>(); authorities.Add(authority.Object);
		world.SetupGet(x => x.LegalAuthorities).Returns(authorities);
		var actor = BuilderWorkflowRegressionTests.Actor(world.Object);
		var output = Capture(actor);
		BuilderWorkflowRegressionTests.Invoke(typeof(LegalModule), "LegalAuthorityClasses", actor.Object, new StringStack("\"City Law\" +citizen"));
		StringAssert.Contains(output.Single(), "Citizens");
		Assert.IsFalse(output.Single().Contains("Visitors"));
	}

	[TestMethod]
	public void HelpMatching_ExactAliasWinsOverPrefixesButAuthoredNamesAndCategoriesRemainAuthoritative()
	{
		var actor = BuilderWorkflowRegressionTests.Actor(BuilderWorkflowRegressionTests.World().Object);
		var help = new CommandHelpInfo("expression", "instructions", AutoHelp.HelpArg, "instructions", false)
			.WithMetadata("Expression", ["expression", "te"], "Building", PermissionLevel.Any);
		var decoy = Mock.Of<IHelpInformation>(x => x.HelpName == "testing" && x.Category == "Lore" && x.CanView(actor.Object));
		var exact = Mock.Of<IHelpInformation>(x => x.HelpName == "te" && x.Category == "Lore" && x.CanView(actor.Object));
		Assert.AreSame(help, HelpModule.FindHelpMatches([decoy, help], actor.Object, "", "TE").Single());
		Assert.AreSame(exact, HelpModule.FindHelpMatches([exact, help], actor.Object, "", "TE").Single());
		Assert.AreSame(decoy, HelpModule.FindHelpMatches([decoy, help], actor.Object, "Lore", "te").Single());
		Assert.AreEqual(0, HelpModule.FindHelpMatches([help], actor.Object, "Lore", "te").Count);
	}

	private static List<string> Capture(Mock<ICharacter> actor)
	{
		var output = new List<string>();
		Mock.Get(actor.Object.OutputHandler).Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
			.Callback<string, bool, bool>((text, _, _) => output.Add(text)).Returns(true);
		return output;
	}

	private sealed class DiscoveryTestModule : Module<ICharacter>
	{
		public DiscoveryTestModule() : base("Discovery") { }
		[PlayerCommand("InternalIdentifier", "discoveryexample", "zz")]
		[CommandPermission(PermissionLevel.Admin)]
		[HelpInfo("discoveryexample", "builder instructions", AutoHelp.HelpArg, Capability = "Building", Summary = "A test builder.")]
		private static void Example(ICharacter actor, string input) { }
	}
}
