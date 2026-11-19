#nullable enable

using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Law;
using MudSharp.RPG.Law.PunishmentStrategies;
using MudSharp.TimeAndDate;

namespace MudSharp_Unit_Tests.RPG.Law;

[TestClass]
public class PunishmentStrategyBuilderTests
{
	[TestMethod]
	[DataRow("multi", 1)]
	[DataRow("multi", 2)]
	[DataRow("hierarchy", 1)]
	[DataRow("hierarchy", 2)]
	public void BuildingCommand_CanEditLastChild(string type, int childCount)
	{
		var world = CreateWorld();
		var authority = Mock.Of<ILegalAuthority>();
		var builder = Mock.Of<ICharacter>(x => x.OutputHandler == Mock.Of<IOutputHandler>());
		var strategy = PunishmentStrategyFactory.LoadStrategy(world, CreateDefinition(type, childCount).ToString(),
			authority);

		Assert.IsTrue(strategy.BuildingCommand(builder, authority, new StringStack($"{childCount} length 7 days")));
		Assert.IsFalse(strategy.BuildingCommand(builder, authority, new StringStack($"{childCount + 1} length 1 day")));

		var saved = XElement.Parse(strategy.SaveResult());
		var lastLength = saved.Elements("Member").Last().Element("Punishment")!.Element("Length")!.Value;
		Assert.AreEqual(MudTimeSpan.FromDays(7), MudTimeSpan.Parse(lastLength));
	}

	[TestMethod]
	public void BuildingCommand_SwapsFirstAndLastHierarchyEntriesWithTheirConditions()
	{
		var world = CreateWorld();
		var authority = Mock.Of<ILegalAuthority>();
		var builder = Mock.Of<ICharacter>(x => x.OutputHandler == Mock.Of<IOutputHandler>());
		var strategy = PunishmentStrategyFactory.LoadStrategy(world, CreateDefinition("hierarchy", 2).ToString(),
			authority);

		Assert.IsTrue(strategy.BuildingCommand(builder, authority, new StringStack("swap 1 2")));

		var members = XElement.Parse(strategy.SaveResult()).Elements("Member").ToArray();
		Assert.AreEqual(2L, (long)members[0].Attribute("prog")!);
		Assert.AreEqual(1L, (long)members[1].Attribute("prog")!);
		var firstLength = members[0].Element("Punishment")!.Element("Length")!.Value;
		Assert.AreEqual(MudTimeSpan.FromDays(2), MudTimeSpan.Parse(firstLength));
	}

	[TestMethod]
	public void PunishmentPlanning_UsesSelectedChildrenAndParentWithoutRunningHooks()
	{
		var defendant = Mock.Of<ICharacter>();
		var crime = Mock.Of<ICrime>();
		var authority = Mock.Of<ILegalAuthority>();
		var condition = new Mock<IFutureProg>();
		condition.SetupGet(x => x.Id).Returns(1);
		condition.Setup(x => x.Execute<bool?>(defendant, crime)).Returns(true);
		var hook = new Mock<IFutureProg>();
		hook.SetupGet(x => x.Id).Returns(3);
		var otherHook = new Mock<IFutureProg>();
		otherHook.SetupGet(x => x.Id).Returns(4);
		var progs = new All<IFutureProg>
		{
			condition.Object, Mock.Of<IFutureProg>(x => x.Id == 2), hook.Object, otherHook.Object
		};
		var world = Mock.Of<IFuturemud>(x => x.FutureProgs == progs);
		var definition = CreateDefinition("hierarchy", 2);
		definition.Add(new XElement("OnPunish", 3));
		foreach (var member in definition.Elements("Member"))
		{
			member.Element("Punishment")!.Add(new XElement("onpunish", 4));
		}
		var strategy = PunishmentStrategyFactory.LoadStrategy(world, definition.ToString(), authority);
		var reloaded = PunishmentStrategyFactory.LoadStrategy(world, strategy.SaveResult(), authority);

		var options = reloaded.GetOptions(defendant, crime);
		var result = reloaded.GetResult(defendant, crime);

		CollectionAssert.AreEqual(new long[] { 4, 3 }, options.OnPunishHooks.Select(x => x.ProgId).ToArray());
		CollectionAssert.AreEqual(new long[] { 4, 3 }, result.OnPunishProgIds);
		Assert.AreEqual(MudTimeSpan.FromDays(1), result.GoodBehaviourBondLength);
		hook.Verify(x => x.Execute(It.IsAny<object[]>()), Times.Never);
		otherHook.Verify(x => x.Execute(It.IsAny<object[]>()), Times.Never);
	}

	[TestMethod]
	public void CombinedOptions_AddSameKindBoundsWithoutErasingOtherMinimums()
	{
		var options = new PunishmentOptions
		{
			MinimumFine = 5m, MaximumFine = 10m
		} + new PunishmentOptions
		{
			MinimumCustodialSentence = MudTimeSpan.FromDays(2),
			MaximumCustodialSentence = MudTimeSpan.FromDays(5)
		} + new PunishmentOptions
		{
			MinimumFine = 3m, MaximumFine = 7m
		};

		Assert.AreEqual(8m, options.MinimumFine);
		Assert.AreEqual(17m, options.MaximumFine);
		Assert.AreEqual(MudTimeSpan.FromDays(2), options.MinimumCustodialSentence);
		Assert.AreEqual(MudTimeSpan.FromDays(5), options.MaximumCustodialSentence);
	}

	private static IFuturemud CreateWorld()
	{
		var progs = new All<IFutureProg>
		{
			Mock.Of<IFutureProg>(x => x.Id == 1),
			Mock.Of<IFutureProg>(x => x.Id == 2)
		};
		return Mock.Of<IFuturemud>(x => x.FutureProgs == progs);
	}

	private static XElement CreateDefinition(string type, int count)
	{
		return new XElement("Punishment", new XAttribute("type", type),
			Enumerable.Range(1, count).Select(i => new XElement("Member",
				new XAttribute("prog", i),
				new XElement("Punishment", new XAttribute("type", "bond"),
					new XElement("Length", MudTimeSpan.FromDays(i).GetRoundTripParseText)))));
	}
}
