#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class OutfitTeachingNumericNameTests
{
	[DataTestMethod]
	[DataRow("travel", "travel1", "travel2")]
	[DataRow("travel2147483647", "travel2147483648", "travel2147483649")]
	[DataRow("travel999999999999999999999999", "travel1000000000000000000000000", "travel1000000000000000000000001")]
	[DataRow("999999999999999999999999", "1000000000000000000000000", "1000000000000000000000001")]
	public void OutfitTeach_AcceptOriginalName_RepeatedCollisionsRemainUnique(string proposed, string next, string expected)
	{
		VerifyTeaching(proposed, "", [proposed, next], expected);
	}

	[TestMethod]
	public void OutfitTeach_AcceptRenamedOutfit_OversizedSuffixRemainsUnique()
	{
		VerifyTeaching("travel", "renamed2147483648", ["renamed2147483648", "renamed2147483649"], "renamed2147483650");
	}

	[TestMethod]
	public void OutfitTeach_AcceptUniqueOversizedName_PreservesName()
	{
		const string name = "travel999999999999999999999999";
		VerifyTeaching(name, "", [], name);
	}

	private static void VerifyTeaching(string originalName, string acceptedName, string[] existingNames, string expected)
	{
		var actor = new Mock<ICharacter>();
		var target = new Mock<ICharacter>();
		actor.SetupGet(x => x.Id).Returns(1L);
		target.SetupGet(x => x.Id).Returns(2L);
		actor.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		target.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		actor.Setup(x => x.TargetActor("student", PerceiveIgnoreFlags.None)).Returns(target.Object);
		var outfit = new Mock<IOutfit>();
		outfit.SetupGet(x => x.Name).Returns(originalName);
		actor.SetupGet(x => x.Outfits).Returns([outfit.Object]);
		var outfits = new List<IOutfit>();
		foreach (var name in existingNames)
		{
			var existing = new Mock<IOutfit>();
			existing.SetupGet(x => x.Name).Returns(name);
			outfits.Add(existing.Object);
		}
		target.SetupGet(x => x.Outfits).Returns(outfits);
		Accept? proposal = null;
		target.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
			.Callback<IEffect, TimeSpan>((effect, _) => proposal = (Accept)effect);
		var copied = new Mock<IOutfit>().Object;
		outfit.Setup(x => x.CopyOutfit(target.Object, expected)).Returns(copied);
		var method = typeof(MudSharp.Character.Character).Assembly.GetType("MudSharp.Commands.Modules.InventoryModule")!
			.GetMethod("OutfitTeach", BindingFlags.Static | BindingFlags.NonPublic)!;

		method.Invoke(null, [actor.Object, new StringStack($"{originalName} student")]);
		Assert.IsNotNull(proposal);
		proposal!.Proposal.Accept(acceptedName);

		outfit.Verify(x => x.CopyOutfit(target.Object, expected), Times.Once);
		target.Verify(x => x.AddOutfit(copied), Times.Once);
	}
}
