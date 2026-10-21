#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Framework;
using MudSharp.FutureProg;
using NativeRace = MudSharp.Character.Heritage.Race;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RaceLifecycleTests
{
	internal static NativeRace Race(params int[] starts)
	{
		var race = TestObjectFactory.CreateUninitialized<NativeRace>();
		var ranges = new RankedRange<AgeCategory>();
		var lower = double.MinValue;
		for (var i = 0; i <= starts.Length; i++)
		{
			var upper = i == starts.Length ? double.MaxValue : starts[i];
			ranges.Add((AgeCategory)i, lower, upper);
			lower = upper;
		}
		typeof(NativeRace).GetField("<Ages>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(race, ranges);
		return race;
	}

	[DataTestMethod]
	[DataRow(3, 10, 20, 30, 61, 81)]
	[DataRow(3, 12, 24, 35, 71, 94)]
	[DataRow(3, 22, 44, 66, 135, 179)]
	[DataRow(3, 11, 22, 33, 66, 88)]
	[DataRow(3, 18, 36, 54, 110, 146)]
	[DataRow(3, 8, 15, 23, 46, 61)]
	public void Lifecycle_ExactStartsAndNeighbouringYearsAgreeWithMinimumGetters(int child, int youth, int youngAdult, int adult, int elder, int venerable)
	{
		int[] starts = [child, youth, youngAdult, adult, elder, venerable];
		var race = Race(starts);
		Assert.AreEqual(AgeCategory.Baby, race.AgeCategory(0));
		for (var i = 0; i < starts.Length; i++)
		{
			var category = (AgeCategory)(i + 1);
			Assert.AreEqual(category, race.AgeCategory(starts[i]));
			Assert.AreEqual((AgeCategory)i, race.AgeCategory(starts[i] - 1));
			Assert.AreEqual(category, race.AgeCategory(starts[i] + 1));
			Assert.AreEqual(starts[i], race.MinimumAgeForCategory(category));
		}
		Assert.AreEqual(AgeCategory.Venerable, race.AgeCategory(10000));
	}

	[TestMethod]
	public void Lifecycle_BuilderRangeChangesTakeEffectWithoutChangingGenericRankedRangeSemantics()
	{
		var race = Race(3, 10, 20, 30, 61, 81);
		Assert.IsTrue(race.Ages.ChangeLowerBounds(AgeCategory.YoungAdult, 21));
		Assert.AreEqual(AgeCategory.Youth, race.AgeCategory(20));
		Assert.AreEqual(AgeCategory.YoungAdult, race.AgeCategory(21));
		Assert.AreEqual(21, race.MinimumAgeForCategory(AgeCategory.YoungAdult));
		var generic = new RankedRange<string>();
		generic.Add("first", 0, 20); generic.Add("second", 20, 30);
		Assert.AreEqual("first", generic.Find(20));
	}

	[TestMethod]
	public void AgeDescription_UsesCurrentNumericAgeAndFallsBackWithoutChangingMechanicalStage()
	{
		var race = Race(3, 10, 20, 30, 61, 81);
		var world = new Mock<IFuturemud>();
		var progs = new All<IFutureProg>();
		world.SetupGet(x => x.FutureProgs).Returns(progs);
		typeof(NativeRace).GetProperty(nameof(race.Gameworld))!.SetValue(race, world.Object);
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.Id).Returns(19);
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Text);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		prog.Setup(x => x.ExecuteString(It.IsAny<object[]>())).Returns<object[]>(x => Convert.ToInt32(x[0]) < 41 ? "mature" : "middle-aged");
		progs.Add(prog.Object);
		typeof(NativeRace).GetField("_ageDescriptionProgId", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(race, (long?)19);
		Assert.AreEqual("mature", race.DescribeAge(40));
		Assert.AreEqual("middle-aged", race.DescribeAge(41));
		Assert.AreEqual(AgeCategory.Adult, race.AgeCategory(40));
		Assert.AreEqual(AgeCategory.Adult, race.AgeCategory(41));
		prog.Setup(x => x.ExecuteString(It.IsAny<object[]>())).Returns(" ");
		Assert.AreEqual("adult", race.DescribeAge(41));
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Number);
		Assert.AreEqual("adult", race.DescribeAge(41));
		progs.Remove(prog.Object);
		Assert.AreEqual("adult", race.DescribeAge(41));
	}
}
