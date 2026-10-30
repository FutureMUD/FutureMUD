#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Economy.Currency;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Functions.BuiltIn;
using MudSharp.FutureProg.Functions.Currency;
using MudSharp.FutureProg.Functions.Manipulation;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;

namespace MudSharp_Unit_Tests;

[TestClass]
public class TradingProgFunctionTests
{
	private static ConstantFunction Argument(IProgVariable value) => new(value);
	private static Mock<ICharacter> Character(IBody body)
	{
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.GetObject).Returns(actor.Object);
		actor.SetupGet(x => x.Body).Returns(body); return actor;
	}
	[TestMethod]
	public void CashTransfer_QueryNeverMutatesAndSettlementUsesExistingCashExactly()
	{
		var body = new Mock<IBody>(); var recipient = Mock.Of<IBody>();
		var actor = Character(body.Object); var target = Character(recipient);
		var currency = new Mock<ICurrency>(); currency.SetupGet(x => x.GetObject).Returns(currency.Object);
		body.Setup(x => x.CanGive(currency.Object, recipient, 20m, true)).Returns(true);
		body.Setup(x => x.TryGiveCurrency(currency.Object, recipient, 20m, true, true)).Returns(true);
		IFunction[] parameters = [Argument(currency.Object), Argument(new NumberVariable(20)), Argument(actor.Object), Argument(target.Object)];
		var query = new TransferCurrencyFunction(parameters, true); query.Execute(Mock.Of<IVariableSpace>());
		Assert.AreEqual(true, query.Result.GetObject);
		body.Verify(x => x.TryGiveCurrency(It.IsAny<ICurrency>(), It.IsAny<IBody>(), It.IsAny<decimal>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
		var transfer = new TransferCurrencyFunction(parameters); transfer.Execute(Mock.Of<IVariableSpace>());
		Assert.AreEqual(true, transfer.Result.GetObject);
		body.Verify(x => x.TryGiveCurrency(currency.Object, recipient, 20m, true, true), Times.Once);
		foreach (var invalid in new[] { 0m, -1m })
		{
			parameters[1] = Argument(new NumberVariable(invalid));
			transfer = new TransferCurrencyFunction(parameters); transfer.Execute(Mock.Of<IVariableSpace>());
			Assert.AreEqual(false, transfer.Result.GetObject);
		}
		body.Verify(x => x.TryGiveCurrency(It.IsAny<ICurrency>(), It.IsAny<IBody>(), It.IsAny<decimal>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
	}
	[TestMethod]
	public void ItemExchange_PassesActualObjectsToNativeCommitAndRejectsSelfExchange()
	{
		var body = new Mock<IBody>(); var partner = Mock.Of<IBody>();
		var actor = Character(body.Object); var other = Character(partner);
		var offered = new Mock<IGameItem>(); offered.SetupGet(x => x.GetObject).Returns(offered.Object);
		var requested = new Mock<IGameItem>(); requested.SetupGet(x => x.GetObject).Returns(requested.Object);
		body.Setup(x => x.TryExchangeItems(offered.Object, partner, requested.Object)).Returns(true);
		var function = new ExchangeItemsFunction([Argument(actor.Object), Argument(offered.Object), Argument(other.Object), Argument(requested.Object)], true);
		function.Execute(Mock.Of<IVariableSpace>()); Assert.AreEqual(true, function.Result.GetObject);
		body.Verify(x => x.TryExchangeItems(offered.Object, partner, requested.Object), Times.Once);
		other.SetupGet(x => x.Body).Returns(body.Object);
		function.Execute(Mock.Of<IVariableSpace>()); Assert.AreEqual(false, function.Result.GetObject);
		body.Verify(x => x.TryExchangeItems(It.IsAny<IGameItem>(), It.IsAny<IBody>(), It.IsAny<IGameItem>()), Times.Once);
	}

	[TestMethod]
	public void CeaseCombat_UsesSelectiveNativeAdmissionAndNeverEndsUnrelatedFight()
	{
		var actor = Character(Mock.Of<IBody>());
		var combat = new Mock<ICombat>(); var selective = combat.As<ICombatSelectiveCessation>();
		var opponent = Mock.Of<ICharacter>(); var admission = Mock.Of<ICombatCessationAdmission>();
		actor.SetupGet(x => x.Combat).Returns(combat.Object); actor.SetupGet(x => x.CombatTarget).Returns(opponent);
		selective.Setup(x => x.PrepareCessation(actor.Object, opponent, combat.Object)).Returns(admission);
		selective.Setup(x => x.CeaseCombatFor(admission)).Returns(CombatCessationChanges.SubjectRemoved);
		var function = new CeaseCombatFunction([Argument(actor.Object)]);
		function.Execute(Mock.Of<IVariableSpace>()); Assert.AreEqual(true, function.Result.GetObject);
		selective.Verify(x => x.CeaseCombatFor(admission), Times.Once);
		actor.SetupGet(x => x.Combat).Returns(Mock.Of<ICombat>());
		function.Execute(Mock.Of<IVariableSpace>()); Assert.AreEqual(false, function.Result.GetObject);
		selective.Verify(x => x.CeaseCombatFor(admission), Times.Once);
	}
}
