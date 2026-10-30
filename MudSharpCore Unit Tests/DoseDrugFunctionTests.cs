#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.FutureProg.Functions.Health;
using MudSharp.Health;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DoseDrugFunctionTests
{
	[TestMethod]
	[DataRow("inhaled", 0.003, true)]
	[DataRow("ingested", 0.003, false)]
	[DataRow("Inhaled", 0.001, true)]
	[DataRow("inhaled", 0.0, false)]
	[DataRow("inhaled", -1.0, false)]
	[DataRow("inhaled", double.NaN, false)]
	[DataRow("inhaled", double.PositiveInfinity, false)]
	[DataRow("inhaled, injected", 0.001, false)]
	public void Dose_ValidatesVectorAndMassBeforeCallingNativeAbsorption(string vector, double grams, bool expected)
	{
		var actor = new Mock<ICharacter>();
		var body = new Mock<IBody>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		body.SetupGet(x => x.Actor).Returns(actor.Object);
		var drug = Mock.Of<IDrug>(x => x.DrugVectors == DrugVector.Inhaled);
		Assert.AreEqual(expected, DoseDrugFunction.TryDose(actor.Object, drug, grams, vector));
		body.Verify(x => x.Dose(drug, DrugVector.Inhaled, grams), expected ? Times.Once() : Times.Never());
	}

	[TestMethod]
	public void Dose_RejectsBodyOwnedByAnotherActor()
	{
		var body = Mock.Of<IBody>();
		var actor = Mock.Of<ICharacter>(x => x.Body == body);
		Assert.IsFalse(DoseDrugFunction.TryDose(actor, Mock.Of<IDrug>(x => x.DrugVectors == DrugVector.Inhaled), 0.001, "inhaled"));
		Mock.Get(body).Verify(x => x.Dose(It.IsAny<IDrug>(), It.IsAny<DrugVector>(), It.IsAny<double>()), Times.Never());
	}
}
