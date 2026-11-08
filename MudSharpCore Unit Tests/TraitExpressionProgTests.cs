#nullable enable

using System;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class TraitExpressionProgTests
{
	[TestMethod]
	[DataRow("de-DE")]
	[DataRow("fr-FR")]
	public void NumericRegisterXml_FractionsReloadIndependentlyOfServerCulture(string culture)
	{
		var previous = CultureInfo.CurrentCulture;
		try
		{
			CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
			var value = new VariableRegister.ValueVariableValue(new XElement("var", "59.495886858"), ProgVariableTypes.Number, Mock.Of<IFuturemud>());
			Assert.AreEqual(59.495886858m, value.GetVariable(Mock.Of<IFuturemud>()).GetObject);
		}
		finally { CultureInfo.CurrentCulture = previous; }
	}

	[TestMethod]
	public void NumericProg_BothConstructorsUseLiveValuesAndExistingTraitSyntax()
	{
		var world = new Mock<IFuturemud>();
		var progs = new All<IFutureProg>(); world.SetupGet(x => x.FutureProgs).Returns(progs);
		var trait = Mock.Of<ITraitDefinition>(x => x.Id == 52);
		world.SetupGet(x => x.Traits).Returns(new All<ITraitDefinition> { trait });
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.Id).Returns(7); prog.SetupGet(x => x.FunctionName).Returns("Capacity");
		prog.SetupGet(x => x.Name).Returns("Capacity");
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Number);
		prog.Setup(x => x.MatchesParameters(It.IsAny<System.Collections.Generic.IEnumerable<ProgVariableTypes>>())).Returns(true);
		double amount = 10;
		prog.Setup(x => x.ExecuteDouble(It.IsAny<object[]>())).Returns(() => amount);
		progs.Add(prog.Object);
		var actor = new Mock<ICharacter>(); var target = new Mock<ICharacter>();
		actor.Setup(x => x.TraitValue(trait, It.IsAny<TraitBonusContext>())).Returns(3);
		target.Setup(x => x.TraitValue(trait, It.IsAny<TraitBonusContext>())).Returns(5);
		const string formula = "perception:52 + defence:!52 + {capacity prog=7}";
		foreach (var expression in new[] { new TraitExpression(formula, world.Object), new TraitExpression(new MudSharp.Models.TraitExpression { Id = 9, Expression = formula }, world.Object) })
		{
			amount = 10; Assert.AreEqual(18d, expression.EvaluateWithTarget(actor.Object, target.Object));
			amount = 20; Assert.AreEqual(28d, expression.EvaluateWithTarget(actor.Object, target.Object));
			Assert.AreEqual(23d, expression.Evaluate(actor.Object));
		}
		Assert.AreEqual(20d, new TraitExpression("{capacity prog=Capacity}", world.Object).Evaluate(actor.Object));
		amount = double.NaN; Assert.AreEqual(0d, new TraitExpression("{capacity prog=7}", world.Object).Evaluate(actor.Object));
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		Assert.AreEqual(0d, new TraitExpression("{capacity prog=7}", world.Object).Evaluate(actor.Object));
	}

	[TestMethod]
	public void NumericProg_MissingInvalidAndRecursiveBindingsReturnZero()
	{
		var world = new Mock<IFuturemud>(); var progs = new All<IFutureProg>();
		world.SetupGet(x => x.FutureProgs).Returns(progs); world.SetupGet(x => x.Traits).Returns(new All<ITraitDefinition>());
		var actor = Mock.Of<ICharacter>();
		var expression = new TraitExpression("1+{capacity prog=7}", world.Object);
		Assert.AreEqual(1d, expression.Evaluate(actor));
		var prog = new Mock<IFutureProg>(); prog.SetupGet(x => x.Id).Returns(7);
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Number);
		prog.Setup(x => x.MatchesParameters(It.IsAny<System.Collections.Generic.IEnumerable<ProgVariableTypes>>())).Returns(true);
		prog.Setup(x => x.ExecuteDouble(It.IsAny<object[]>())).Returns(() => expression.Evaluate(actor));
		progs.Add(prog.Object);
		Assert.AreEqual(2d, expression.Evaluate(actor), "Recursive input contributes zero instead of exhausting the stack.");
		Assert.AreEqual(0d, new TraitExpressionProgOption("7", world.Object).Evaluate(Mock.Of<IHaveTraits>()));
	}
}
