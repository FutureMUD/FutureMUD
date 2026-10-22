#nullable enable

using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ConditionalMagicItemPersistenceTests
{
	[TestMethod]
	public void Restorer_CapacityRevisionClampsExcessWithoutGrantingChargesOnExpansionOrCopy()
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var parent = Mock.Of<IGameItem>(x => x.Gameworld == world.Object);
		var original = Prototype(world.Object, 6);
		var component = (ResourceRestorerGameItemComponent)original.LoadComponent(
			new MudSharp.Models.GameItemComponent { Id = 42, Definition = "<Definition charges=\"6\" />" }, parent);
		Assert.AreEqual(6, component.Charges);
		var update = typeof(ConditionalMagicItemGameItemComponent).GetMethod("UpdateComponentNewPrototype", BindingFlags.Instance | BindingFlags.NonPublic)!;
		update.Invoke(component, [Prototype(world.Object, 3)]);
		Assert.AreEqual(3, component.Charges);
		Assert.IsTrue(component.Changed);
		update.Invoke(component, [Prototype(world.Object, 12)]);
		Assert.AreEqual(3, component.Charges);
		Assert.AreEqual(0, ((ResourceRestorerGameItemComponent)component.Copy(parent, temporary: true)).Charges);
		var saved = (string)typeof(ConditionalMagicItemGameItemComponent).GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(component, null)!;
		var reloaded = (ResourceRestorerGameItemComponent)component.Prototype.LoadComponent(
			new MudSharp.Models.GameItemComponent { Id = 42, Definition = saved }, parent);
		Assert.AreEqual(3, reloaded.Charges);
		Assert.IsFalse(reloaded.Changed);
	}

	[TestMethod]
	public void Restorer_ColdRevisionTransitionClampsOldBankAndStillRejectsNegativeData()
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var parent = Mock.Of<IGameItem>(x => x.Gameworld == world.Object);
		var proto = Prototype(world.Object, 3);
		var component = (ResourceRestorerGameItemComponent)proto.LoadComponent(
			new MudSharp.Models.GameItemComponent { Id = 42, Definition = "<Definition charges=\"6\" />" }, parent);
		Assert.AreEqual(3, component.Charges);
		Assert.IsTrue(component.Changed);
		Assert.ThrowsException<FormatException>(() => proto.LoadComponent(
			new MudSharp.Models.GameItemComponent { Id = 43, Definition = "<Definition charges=\"-1\" />" }, parent));
	}

	private static ResourceRestorerGameItemComponentProto Prototype(IFuturemud world, int capacity) =>
		(ResourceRestorerGameItemComponentProto)new GameItemComponentManager().GetProto(new MudSharp.Models.GameItemComponentProto
		{
			Id = 7001, Name = "Unrelated reserve", Type = "ResourceRestorer",
			Definition = new ConditionalMagicItemPolicy { Kind = ConditionalMagicItemKind.ResourceRestorer, Capacity = capacity, ResourceId = 1, EligibilityProgId = 1 }.Save().ToString(),
			EditableItem = new MudSharp.Models.EditableItem { BuilderDate = DateTime.UtcNow }
		}, world);
}
