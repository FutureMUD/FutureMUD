#nullable enable

using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
[DoNotParallelize]
public class GameItemDescriptionOverrideTests
{
	[DataTestMethod]
	[DataRow(DescriptionType.Short, "a plain token", "a painted token", "a personal token")]
	[DataRow(DescriptionType.Full, "A plain token.", "A painted token.", "A personal token.")]
	[DataRow(DescriptionType.Contents, "A plain token.", "A painted token.", "A personal token.")]
	public void HowSeen_OverrideSkinAndPrototype_UsesNullablePrecedence(DescriptionType type,
		string prototypeText, string skinText, string overrideText)
	{
		var (item, _, skin, viewer) = CreateItem();
		Assert.IsNull(item.OverrideSdesc);
		Assert.IsNull(item.OverrideDesc);
		Assert.AreEqual(prototypeText, Describe(item, viewer.Object, type));

		item.Skin = skin.Object;
		Assert.AreEqual(skinText, Describe(item, viewer.Object, type));
		item.OverrideSdesc = "a personal token";
		item.OverrideDesc = "A personal token.";
		Assert.AreEqual(overrideText, Describe(item, viewer.Object, type));

		item.OverrideSdesc = null;
		item.OverrideDesc = null;
		Assert.AreEqual(skinText, Describe(item, viewer.Object, type));
		item.Skin = null!;
		Assert.AreEqual(prototypeText, Describe(item, viewer.Object, type));
	}

	[TestMethod]
	public void HowSeen_ConditionalPrototypeDescriptions_DoNotReplaceInstanceOrSkinText()
	{
		var (item, proto, skin, viewer) = CreateItem();
		var prog = new Mock<IFutureProg>();
		prog.Setup(x => x.Execute<bool?>(It.IsAny<object[]>())).Returns(true);
		proto.SetupGet(x => x.ExtraDescriptions).Returns(new (IFutureProg, string?, string?, string?)[]
		{
			(prog.Object, "a conditional token", "A conditional token.", string.Empty)
		});
		Assert.AreEqual("a conditional token", Describe(item, viewer.Object, DescriptionType.Short));
		Assert.AreEqual("A conditional token.", Describe(item, viewer.Object, DescriptionType.Full));

		item.Skin = skin.Object;
		Assert.AreEqual("a painted token", Describe(item, viewer.Object, DescriptionType.Short));
		Assert.AreEqual("A painted token.", Describe(item, viewer.Object, DescriptionType.Full));
		item.OverrideSdesc = "a personal token";
		item.OverrideDesc = "A personal token.";
		Assert.AreEqual("a personal token", Describe(item, viewer.Object, DescriptionType.Short));
		Assert.AreEqual("A personal token.", Describe(item, viewer.Object, DescriptionType.Full));
		Assert.AreEqual("A personal token.", Describe(item, viewer.Object, DescriptionType.Contents));
	}

	[TestMethod]
	public void HowSeen_OverrideMarkup_UsesNormalMaterialSubstitution()
	{
		var (item, proto, _, viewer) = CreateItem();
		Mock.Get(proto.Object.Material).SetupGet(x => x.Name).Returns("Brass");
		item.OverrideSdesc = "a @material token";
		item.OverrideDesc = "A token made of @material.";
		Assert.AreEqual("a brass token", Describe(item, viewer.Object, DescriptionType.Short));
		Assert.AreEqual("A token made of brass.", Describe(item, viewer.Object, DescriptionType.Full));
	}

	[TestMethod]
	public void Overrides_EmptyStrings_AreNotConvertedToNull()
	{
		var (item, _, _, viewer) = CreateItem();
		item.OverrideSdesc = string.Empty;
		item.OverrideDesc = string.Empty;
		Assert.AreEqual(string.Empty, Describe(item, viewer.Object, DescriptionType.Short));
		Assert.AreEqual(string.Empty, Describe(item, viewer.Object, DescriptionType.Full));
	}

	[TestMethod]
	public void Overrides_ChangingOrClearing_MarksItemForSaving()
	{
		var (item, _, _, _) = CreateItem();
		item.Changed = false;
		item.OverrideSdesc = "custom";
		Assert.IsTrue(item.Changed);
		item.Changed = false;
		item.OverrideDesc = "Custom.";
		Assert.IsTrue(item.Changed);
		item.Changed = false;
		item.OverrideSdesc = null;
		Assert.IsTrue(item.Changed);
		item.Changed = false;
		item.OverrideDesc = null;
		Assert.IsTrue(item.Changed);
	}

	[DataTestMethod]
	[DataRow(null, null)]
	[DataRow("a custom token", "Custom prose.")]
	[DataRow("", "")]
	public void DatabaseInsertSaveAndLoad_PreserveOverridesIncludingNull(string? shortText, string? fullText)
	{
		var (item, _, _, _) = CreateItem();
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
		var contextProperty = typeof(FMDB).GetProperty(nameof(FMDB.Context))!;
		var countProperty = typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!;
		var previousContext = contextProperty.GetValue(null);
		var previousCount = countProperty.GetValue(null);
		try
		{
			contextProperty.SetValue(null, context);
			countProperty.SetValue(null, 1u);
			using (new FMDB())
			{
				item.OverrideSdesc = shortText;
				item.OverrideDesc = fullText;
				var row = (MudSharp.Models.GameItem)item.DatabaseInsert();
				context.SaveChanges();
				context.ChangeTracker.Clear();
				var loaded = new GameItem(context.GameItems.Find(row.Id)!, item.Gameworld);
				Assert.AreEqual(shortText, loaded.OverrideSdesc);
				Assert.AreEqual(fullText, loaded.OverrideDesc);

				loaded.OverrideSdesc = "edited short";
				loaded.OverrideDesc = "Edited full.";
				loaded.Save();
				context.SaveChanges();
				context.ChangeTracker.Clear();
				loaded = new GameItem(context.GameItems.Find(row.Id)!, item.Gameworld);
				Assert.AreEqual("edited short", loaded.OverrideSdesc);
				Assert.AreEqual("Edited full.", loaded.OverrideDesc);

				loaded.OverrideSdesc = null;
				loaded.OverrideDesc = null;
				loaded.Save();
				context.SaveChanges();
				context.ChangeTracker.Clear();
				loaded = new GameItem(context.GameItems.Find(row.Id)!, item.Gameworld);
				Assert.IsNull(loaded.OverrideSdesc);
				Assert.IsNull(loaded.OverrideDesc);
			}
		}
		finally
		{
			contextProperty.SetValue(null, previousContext);
			countProperty.SetValue(null, previousCount);
		}
	}

	[TestMethod]
	public void CopyAndStackSplit_PreserveIndependentOverrideValues()
	{
		var (item, _, _, _) = CreateItem();
		item.OverrideSdesc = "custom short";
		item.OverrideDesc = "Custom full.";
		var stack = new StackableGameItemComponent((StackableGameItemComponentProto)null!, item) { Quantity = 10 };
		var components = (System.Collections.Generic.List<IGameItemComponent>)typeof(GameItem)
			.GetField("_components", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(item)!;
		components.Add(stack);

		foreach (var copy in new[] { new GameItem(item), stack.PeekSplit(2), stack.Split(2) })
		{
			Assert.AreEqual(item.OverrideSdesc, copy.OverrideSdesc);
			Assert.AreEqual(item.OverrideDesc, copy.OverrideDesc);
			copy.OverrideDesc = "Different.";
			Assert.AreEqual("Custom full.", item.OverrideDesc);
		}
		Assert.AreEqual(8, stack.Quantity);
	}

	[DataTestMethod]
	[DataRow(null, null, true)]
	[DataRow("same", "same", true)]
	[DataRow(null, "custom", false)]
	[DataRow("one", "two", false)]
	[DataRow(null, "", false)]
	public void CanMerge_DescriptionIdentity_MustMatch(string? first, string? second, bool expected)
	{
		var (item, _, _, _) = CreateItem();
		var other = new Mock<IGameItem>();
		other.SetupGet(x => x.Prototype).Returns(item.Prototype);
		other.Setup(x => x.IsItemType<IStackable>()).Returns(true);
		item.OverrideSdesc = first;
		other.SetupGet(x => x.OverrideSdesc).Returns(second);
		Assert.AreEqual(expected, item.CanMerge(other.Object));
		item.OverrideSdesc = null;
		other.SetupGet(x => x.OverrideSdesc).Returns((string?)null);
		item.OverrideDesc = first;
		other.SetupGet(x => x.OverrideDesc).Returns(second);
		Assert.AreEqual(expected, item.CanMerge(other.Object));
	}

	private static string Describe(GameItem item, ICharacter viewer, DescriptionType type) =>
		item.HowSeen(viewer, false, type, false, PerceiveIgnoreFlags.IgnoreLiquidsAndFlags).Trim();

	private static (GameItem Item, Mock<IGameItemProto> Proto, Mock<IGameItemSkin> Skin, Mock<ICharacter> Viewer) CreateItem()
	{
		var gameworld = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var proto = new Mock<IGameItemProto> { DefaultValue = DefaultValue.Mock };
		proto.SetupGet(x => x.Id).Returns(1L);
		proto.SetupGet(x => x.Name).Returns("token");
		proto.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		proto.SetupGet(x => x.ShortDescription).Returns("a plain token");
		proto.SetupGet(x => x.FullDescription).Returns("A plain token.");
		gameworld.Setup(x => x.ItemProtos.Get(1L, It.IsAny<int>())).Returns(proto.Object);
		var skin = new Mock<IGameItemSkin>();
		skin.SetupGet(x => x.Id).Returns(2L);
		skin.SetupGet(x => x.ShortDescription).Returns("a painted token");
		skin.SetupGet(x => x.FullDescription).Returns("A painted token.");
		gameworld.Setup(x => x.ItemSkins.Get(It.IsAny<long>())).Returns((long id) => id == 2L ? skin.Object : null!);
		var viewer = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		viewer.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		viewer.SetupGet(x => x.InnerLineFormatLength).Returns(120);
		return (new GameItem(proto.Object), proto, skin, viewer);
	}
}
