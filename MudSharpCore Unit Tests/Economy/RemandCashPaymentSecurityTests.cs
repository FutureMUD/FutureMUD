#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Payment;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp_Unit_Tests.Economy;

[TestClass]
[DoNotParallelize]
public class RemandCashPaymentSecurityTests
{
	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void LawyerCashPayment_ExhaustedRemotePile_ReturnsChangeToCustody(bool containerHasCapacity)
	{
		using var fixture = new PaymentFixture();
		fixture.Container.Setup(x => x.CanPut(fixture.Change.Object)).Returns(containerHasCapacity);
		var payment = fixture.LawyerPayment([fixture.Bundle.Object]);

		payment.TakePayment(10M);

		Assert.IsNull(fixture.Source.Object.ContainedIn, "The exhausted source pile loses its container.");
		Assert.AreSame(fixture.Bundle.Object, fixture.Change.Object.ContainedIn);
		fixture.Container.Verify(x => x.Put(null, fixture.Change.Object, true), Times.Once);
		fixture.Body.Verify(x => x.CanGet(It.IsAny<IGameItem>(), It.IsAny<int>(), It.IsAny<ItemCanGetIgnore>()), Times.Never);
		fixture.Body.Verify(x => x.Get(It.IsAny<IGameItem>(), It.IsAny<int>(), null, It.IsAny<bool>(), It.IsAny<ItemCanGetIgnore>()), Times.Never);
	}

	[TestMethod]
	public void OtherCashPayment_ExhaustedNormalPile_PreservesSourceContainerForChange()
	{
		using var fixture = new PaymentFixture();
		fixture.Body.SetupGet(x => x.ExternalItems).Returns([fixture.Bundle.Object]);
		fixture.Container.Setup(x => x.CanPut(fixture.Change.Object)).Returns(true);

		new OtherCashPayment(fixture.Currency.Object, fixture.Actor.Object).TakePayment(10M);

		Assert.AreSame(fixture.Bundle.Object, fixture.Change.Object.ContainedIn);
		fixture.Container.Verify(x => x.Put(null, fixture.Change.Object, true), Times.Once);
	}

	[TestMethod]
	public void LawyerCashPayment_NoCustodyBundle_PreservesCarriedCashChange()
	{
		using var fixture = new PaymentFixture();
		fixture.Source.SetupGet(x => x.ContainedIn).Returns((IGameItem?)null);
		fixture.Body.SetupGet(x => x.ExternalItems).Returns([fixture.Source.Object]);
		fixture.Body.Setup(x => x.CanGet(fixture.Change.Object, 0, ItemCanGetIgnore.None)).Returns(true);

		fixture.LawyerPayment([]).TakePayment(10M);

		fixture.Body.Verify(x => x.Get(fixture.Change.Object, 0, null, true, ItemCanGetIgnore.None), Times.Once);
		fixture.Container.Verify(x => x.Put(null, It.IsAny<IGameItem>(), true), Times.Never);
	}

	[TestMethod]
	public void LawyerCashPayment_ExactFee_DoesNotCreateChange()
	{
		using var fixture = new PaymentFixture();

		fixture.LawyerPayment([fixture.Bundle.Object]).TakePayment(100M);

		fixture.Change.Verify(x => x.Login(), Times.Never);
		fixture.Container.Verify(x => x.Put(null, It.IsAny<IGameItem>(), true), Times.Never);
	}

	private sealed class PaymentFixture : IDisposable
	{
		private readonly IGameItemProto _previousPrototype = CurrencyGameItemComponentProto.ItemPrototype;
		public Mock<ICurrency> Currency { get; } = new();
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<IBody> Body { get; } = new();
		public Mock<IGameItem> Bundle { get; } = new();
		public Mock<IContainer> Container { get; } = new();
		public Mock<IGameItem> Source { get; } = new();
		public Mock<IGameItem> Change { get; } = new();

		public PaymentFixture()
		{
			Actor.SetupGet(x => x.Body).Returns(Body.Object);
			Body.SetupGet(x => x.ExternalItems).Returns(Array.Empty<IGameItem>());
			Bundle.Setup(x => x.GetItemType<IContainer>()).Returns(Container.Object);
			Container.SetupGet(x => x.Contents).Returns([Source.Object]);
			Container.Setup(x => x.Put(null, Change.Object, true))
				.Callback(() => Change.SetupGet(x => x.ContainedIn).Returns(Bundle.Object));
			Source.SetupGet(x => x.ContainedIn).Returns(Bundle.Object);
			Source.Setup(x => x.Delete()).Callback(() => Source.SetupGet(x => x.ContainedIn).Returns((IGameItem?)null));
			var pile = new Mock<ICurrencyPile>();
			pile.SetupGet(x => x.Parent).Returns(Source.Object);
			Source.Setup(x => x.IsItemType<ICurrencyPile>()).Returns(true);
			Source.Setup(x => x.GetItemType<ICurrencyPile>()).Returns(pile.Object);
			var coin = Mock.Of<ICoin>(x => x.Value == 100M);
			Currency.Setup(x => x.FindCurrency(It.IsAny<IEnumerable<ICurrencyPile>>(), It.IsAny<decimal>()))
				.Returns<IEnumerable<ICurrencyPile>, decimal>((piles, _) => piles.ToDictionary(x => x, _ => new Dictionary<ICoin, int> { [coin] = 1 }));
			var exact = true;
			Currency.Setup(x => x.FindCoinsForAmount(90M, out exact)).Returns(new Dictionary<ICoin, int> { [Mock.Of<ICoin>(x => x.Value == 1M)] = 90 });
			Change.Setup(x => x.GetItemType<ICurrencyPile>()).Returns(Mock.Of<ICurrencyPile>());
			var prototype = new Mock<IGameItemProto>();
			prototype.Setup(x => x.CreateNew(null)).Returns(Change.Object);
			CurrencyGameItemComponentProto.ItemPrototype = prototype.Object;
		}

		public OtherCashPayment LawyerPayment(IReadOnlyList<IGameItem> bundles)
		{
			var module = typeof(OtherCashPayment).Assembly.GetType("MudSharp.Commands.Modules.CrimeModule")!;
			return (OtherCashPayment)module.GetMethod("CreateLawyerCashPayment", BindingFlags.Static | BindingFlags.NonPublic)!
				.Invoke(null, [Currency.Object, Actor.Object, bundles])!;
		}

		public void Dispose() => CurrencyGameItemComponentProto.ItemPrototype = _previousPrototype;
	}
}
