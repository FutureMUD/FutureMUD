#nullable enable
using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Work.Crafts;
using MudSharp.Work.Crafts.Inputs;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SimpleItemEmptyLiquidInputTests
{
	[TestMethod]
	public void EmptyLiquidRequirement_IsOptionalAndRejectsNonContainers()
	{
		var model = new MudSharp.Models.CraftInput { Id = 1, Definition = "<Definition><TargetItemId>10</TargetItemId><Quantity>1</Quantity></Definition>" };
		var input = (SimpleItemInput)Activator.CreateInstance(typeof(SimpleItemInput), BindingFlags.Instance | BindingFlags.NonPublic,
			null, [model, Mock.Of<ICraft>(), Mock.Of<IFuturemud>()], null)!;
		var proto = new Mock<IGameItemProto>(); proto.SetupGet(x => x.Id).Returns(10);
		var item = new Mock<IGameItem>(); item.SetupGet(x => x.Prototype).Returns(proto.Object);
		Assert.IsTrue(input.IsInput(item.Object));
		input.RequireEmptyLiquidContainer = true;
		Assert.IsFalse(input.IsInput(item.Object));
		item.Setup(x => x.GetItemType<ILiquidContainer>()).Returns(Mock.Of<ILiquidContainer>());
		Assert.IsTrue(input.IsInput(item.Object));
		var filled = new Mock<ILiquidContainer>();
		filled.SetupGet(x => x.LiquidMixture).Returns(new LiquidMixture(Mock.Of<ILiquid>(), 1.0, Mock.Of<IFuturemud>()));
		item.Setup(x => x.GetItemType<ILiquidContainer>()).Returns(filled.Object);
		Assert.IsFalse(input.IsInput(item.Object), "A filled vial must not be consumed.");
		input.RequireEmptyLiquidContainer = false;
		Assert.IsTrue(input.IsInput(item.Object), "Legacy crafts retain their prior selection rules.");
		input.RequireEmptyLiquidContainer = true;
		var saved = (string)typeof(SimpleItemInput).GetMethod("SaveDefinition", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(input, null)!;
		Assert.IsTrue(saved.Contains("<RequireEmptyLiquidContainer>true</RequireEmptyLiquidContainer>"));
		model.Definition = saved;
		var reloaded = (SimpleItemInput)Activator.CreateInstance(typeof(SimpleItemInput), BindingFlags.Instance | BindingFlags.NonPublic,
			null, [model, Mock.Of<ICraft>(), Mock.Of<IFuturemud>()], null)!;
		Assert.IsTrue(reloaded.RequireEmptyLiquidContainer);
	}
}
