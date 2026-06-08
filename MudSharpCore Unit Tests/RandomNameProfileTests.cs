#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character.Name;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RandomNameProfileTests
{
	[TestMethod]
	public void GetRandomPersonalName_WhenDiceRequestsMoreUniqueNamesThanAvailable_ReturnsWithoutHanging()
	{
		Mock<IFuturemud> gameworld = new();
		Mock<IUneditableAll<IFutureProg>> futureProgs = new();
		futureProgs.Setup(x => x.Get(It.IsAny<long>())).Returns((IFutureProg?)null);
		gameworld.SetupGet(x => x.FutureProgs).Returns(futureProgs.Object);
		gameworld.SetupGet(x => x.AlwaysFalseProg).Returns(new Mock<IFutureProg>().Object);
		Mock<INameCulture> culture = new();
		culture.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		culture.Setup(x => x.NamePattern(It.IsAny<NameStyle>()))
			.Returns(Tuple.Create("{0}", new List<NameUsage> { NameUsage.BirthName }));
		MudSharp.Models.RandomNameProfile model = new()
		{
			Id = 1,
			Name = "Arena Stage Names",
			Gender = (int)Gender.Indeterminate
		};
		model.RandomNameProfilesDiceExpressions.Add(new MudSharp.Models.RandomNameProfilesDiceExpressions
		{
			NameUsage = (int)NameUsage.BirthName,
			DiceExpression = "2"
		});
		model.RandomNameProfilesElements.Add(new MudSharp.Models.RandomNameProfilesElements
		{
			NameUsage = (int)NameUsage.BirthName,
			Name = "alex",
			Weighting = 100
		});
		RandomNameProfile profile = new(model, culture.Object);

		IPersonalName personalName = profile.GetRandomPersonalName(nonSaving: true);

		Assert.AreEqual("Alex", personalName.GetName(NameStyle.SimpleFull));
	}
}
