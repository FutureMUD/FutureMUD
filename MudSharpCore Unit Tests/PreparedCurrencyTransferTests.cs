#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Economy.Currency;
using MudSharp.Events.Hooks;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;

namespace MudSharp_Unit_Tests;

[TestClass]
[DoNotParallelize]
public class PreparedCurrencyTransferTests
{
	[DataTestMethod]
	[DataRow("tuple")]
	[DataRow("pair")]
	[DataRow("value-tuple")]
	public void TemporaryCurrencyFactory_AllOverloadsCreateOnlyUnpublishedNativeValue(string overload)
	{
		using var fixture = new CurrencyPreviewFixture();
		for (var i = 0; i < 3; i++)
		{
			var item = (GameItem)(overload switch
			{
				"tuple" => CurrencyGameItemComponentProto.CreateNewCurrencyPile(fixture.Currency, new[] { Tuple.Create(fixture.Coin, 2), Tuple.Create(fixture.Coin, 3) }, true),
				"pair" => CurrencyGameItemComponentProto.CreateNewCurrencyPile(fixture.Currency, new[] { new KeyValuePair<ICoin, int>(fixture.Coin, 2), new(fixture.Coin, 3) }, true),
				_ => CurrencyGameItemComponentProto.CreateNewCurrencyPile(fixture.Currency, new[] { (fixture.Coin, 2), (fixture.Coin, 3) }, true)
			});
			Assert.IsTrue(item.GetNoSave());
			Assert.IsFalse(item.IdHasBeenRegistered);
			Assert.IsFalse(item.Changed);
			Assert.IsNull(item.DirectLocation);
			Assert.IsNull(item.ContainedIn);
			Assert.IsNull(item.InInventoryOf);
			var pile = item.GetItemType<CurrencyGameItemComponent>();
			Assert.AreSame(fixture.Currency, pile.Currency);
			Assert.AreEqual(5, pile.Coins.Single().Item2);
			Assert.AreEqual(5m, pile.TotalValue);
			Assert.IsTrue(pile.GetNoSave());
			Assert.IsFalse(pile.Changed);
		}
		fixture.AssertUnpublished();
	}

	[TestMethod]
	public void CurrencyPreviewOwner_CopyDoesNotResolveOrPublishOwnership()
	{
		using var fixture = new CurrencyPreviewFixture();
		var item = (GameItem)CurrencyGameItemComponentProto.CreateNewCurrencyPile(fixture.Currency, new[] { (fixture.Coin, 4) }, true);
		var source = new Mock<IGameItem>();
		source.SetupGet(x => x.OwnershipReference).Returns(new ItemOwnershipReference("Character", 71));
		item.CopyCurrencyPreviewOwner(source.Object);
		Assert.AreEqual(source.Object.OwnershipReference, item.OwnershipReference);
		source.VerifyGet(x => x.Owner, Times.Never);
		Assert.IsFalse(item.Changed);
		fixture.AssertUnpublished();
	}

	[TestMethod]
	public void DeferredSpellConstructor_ApplicableHooksStillRejectInsteadOfUsingCurrencyPreview()
	{
		using var fixture = new CurrencyPreviewFixture();
		Assert.ThrowsException<InvalidOperationException>(() => new GameItem(fixture.Prototype, null, ItemQuality.Standard, true));
		fixture.Hook.Verify(x => x.Applies(It.IsAny<IProgVariable>(), "GameItem"), Times.Once);
		Assert.ThrowsException<ArgumentException>(() => new GameItem(fixture.Prototype, null, ItemQuality.Standard, true, true));
		Assert.AreEqual(0, fixture.Save.Invocations.Count);
		fixture.LoadProg.Verify(x => x.Execute(It.IsAny<object[]>()), Times.Never);
	}

	private sealed class CurrencyPreviewFixture : IDisposable
	{
		private readonly IGameItemProto _previous = CurrencyGameItemComponentProto.ItemPrototype;
		internal Mock<IFuturemud> World { get; } = new() { DefaultValue = DefaultValue.Mock };
		internal Mock<ISaveManager> Save { get; } = new();
		internal Mock<IDefaultHook> Hook { get; } = new();
		internal Mock<IFutureProg> LoadProg { get; } = new();
		internal GameItemProto Prototype { get; }
		internal ICurrency Currency { get; } = Mock.Of<ICurrency>();
		internal ICoin Coin { get; } = Mock.Of<ICoin>(x => x.Value == 1m);

		internal CurrencyPreviewFixture()
		{
			World.SetupGet(x => x.SaveManager).Returns(Save.Object);
			World.SetupGet(x => x.DefaultHooks).Returns([Hook.Object]);
			Hook.Setup(x => x.Applies(It.IsAny<IProgVariable>(), "GameItem")).Returns(true);
			Prototype = TestObjectFactory.CreateUninitialized<GameItemProto>();
			typeof(GameItemProto).GetProperty("Gameworld")!.SetValue(Prototype, World.Object);
			SetField(Prototype, "_name", "preview currency");
			SetField(Prototype, "_components", new List<IGameItemComponentProto>
			{
				TestObjectFactory.CreateUninitialized<CurrencyGameItemComponentProto>(),
				TestObjectFactory.CreateUninitialized<HoldableGameItemComponentProto>()
			});
			Prototype.OnLoadProgs = [LoadProg.Object];
			CurrencyGameItemComponentProto.ItemPrototype = Prototype;
		}

		internal void AssertUnpublished()
		{
			Assert.AreEqual(0, Save.Invocations.Count, "A query preview touched persistence.");
			Hook.Verify(x => x.Applies(It.IsAny<IProgVariable>(), It.IsAny<string>()), Times.Never);
			LoadProg.Verify(x => x.Execute(It.IsAny<object[]>()), Times.Never);
			Assert.IsFalse(World.Invocations.Any(x => x.Method.Name is "Add" or "Destroy"));
		}

		public void Dispose() => CurrencyGameItemComponentProto.ItemPrototype = _previous;
	}

	private static void SetField(object value, string name, object fieldValue)
	{
		for (var type = value.GetType(); type is not null; type = type.BaseType)
			if (type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly) is { } field)
			{
				field.SetValue(value, fieldValue);
				return;
			}
		Assert.Fail($"Missing native fixture field {name}.");
	}
}
