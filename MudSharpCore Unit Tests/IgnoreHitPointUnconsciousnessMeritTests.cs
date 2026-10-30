#nullable enable

using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.FutureProg;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.CharacterMerits;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp_Unit_Tests;

[TestClass]
public class IgnoreHitPointUnconsciousnessMeritTests
{
	[TestMethod]
	public void MeritRegistrationLoadsCharacterScopedMarkerAndUsesApplicabilityProg()
	{
		IgnoreHitPointUnconsciousnessMerit.RegisterMeritInitialiser();
		var world = new Mock<IFuturemud>();
		var alwaysTrue = new Mock<IFutureProg>();
		alwaysTrue.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(true);
		var applies = new Mock<IFutureProg>();
		applies.SetupGet(x => x.Id).Returns(20);
		applies.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(false);
		var progs = new All<IFutureProg>();
		progs.Add(applies.Object);
		world.SetupGet(x => x.FutureProgs).Returns(progs);
		world.SetupGet(x => x.AlwaysTrueProg).Returns(alwaysTrue.Object);
		world.SetupGet(x => x.AlwaysFalseProg).Returns(alwaysTrue.Object);

		var row = new Merit
		{
			Id = 1,
			Name = "Ignore HP unconsciousness",
			Type = IgnoreHitPointUnconsciousnessMerit.MeritTypeName,
			MeritType = (int)MeritType.Merit,
			MeritScope = (int)MeritScope.Character,
			Definition = new XElement("Merit", new XElement("ApplicabilityProg", 20)).ToString()
		};

		var loaded = MeritFactory.LoadMerit(row, world.Object);
		Assert.IsInstanceOfType<IIgnoreHitPointUnconsciousnessMerit>(loaded);
		Assert.AreEqual(MeritScope.Character, loaded.MeritScope);
		var character = new Mock<ICharacter>();
		Assert.IsFalse(loaded.Applies(character.Object));
		applies.Verify(x => x.ExecuteBool(It.Is<object[]>(args => args.Length == 1 && ReferenceEquals(args[0], character.Object))), Times.Once);
	}
}
