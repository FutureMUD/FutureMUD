using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Magic;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class SubstanceDoseTests
{
	[TestMethod]
	public void Absorption_TinyDose_ConservesQuantityBeforeClearance()
	{
		foreach (var vector in new[] { DrugVector.Ingested, DrugVector.Injected, DrugVector.Touched, DrugVector.Inhaled })
		{
			var (latent, active) = SubstanceDose.Advance(0.00001, 0, vector, 0);
			Assert.AreEqual(0.00001, latent + active, 1e-12);
			Assert.IsTrue(latent >= 0 && active >= 0);
		}
	}
	[TestMethod]
	public void Charge_CopyAndReload_PreservesSpentAndSuppressedIndependently()
	{
		var entry = Guid.NewGuid(); var charge = new SubstanceCharge(); charge.Spent.Add(entry);
		var copy = SubstanceCharge.Load(XElement.Parse(charge.Save().ToString()));
		Assert.IsTrue(copy.CanMerge(charge));
		copy.Suppressed.Add(entry);
		Assert.IsFalse(copy.CanMerge(charge)); Assert.IsFalse(charge.Suppressed.Contains(entry));
	}
	[TestMethod]
	public void Liquid_SplitMixReload_DoesNotRefreshCharge()
	{
		var liquid = new Mock<ILiquid>(); liquid.SetupGet(x => x.Id).Returns(7); liquid.SetupGet(x => x.Density).Returns(1);
		var liquids = new Mock<IUneditableAll<ILiquid>>(); liquids.Setup(x => x.Get(7)).Returns(liquid.Object);
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.Liquids).Returns(liquids.Object);
		var entry = Guid.NewGuid(); var instance = new LiquidInstance { Liquid = liquid.Object, Amount = 10 };
		instance.MagicalCharges[1] = new(); instance.MagicalCharges[1].Spent.Add(entry);
		var split = instance.SplitVolume(3);
		Assert.AreEqual(10, split.Amount + instance.Amount, 1e-9);
		Assert.IsTrue(split.MagicalCharges[1].Spent.Contains(entry));
		var mix = new LiquidMixture(instance, world.Object); mix.AddLiquid(new LiquidMixture(split, world.Object));
		var restored = new LiquidMixture(XElement.Parse(mix.SaveToXml().ToString()), world.Object);
		Assert.AreEqual(10, restored.TotalVolume, 1e-9);
		Assert.IsTrue(restored.Instances.All(x => x.MagicalCharges[1].Spent.Contains(entry)));
	}
	[TestMethod]
	public void Liquid_MergeFreshWithSpent_RetainsDistinctConstituents()
	{
		var liquid = Mock.Of<ILiquid>();
		var first = new LiquidInstance { Liquid = liquid, Amount = 2 };
		var second = new LiquidInstance { Liquid = liquid, Amount = 3 };
		first.MagicalCharges[1] = new(); first.MagicalCharges[1].Spent.Add(Guid.NewGuid());
		Assert.IsFalse(first.CanMergeWith(second));
		var mixture = new LiquidMixture(new[] { first, second }, null!);
		Assert.AreEqual(2, mixture.Instances.Count());
	}
	[TestMethod]
	public void QuantityValidation_RejectsNonFiniteAndNonPositiveValues()
	{
		foreach (var value in new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity, 0, -1 }) Assert.IsFalse(SubstanceDose.IsPositive(value));
		Assert.IsTrue(SubstanceDose.IsPositive(0.001));
	}
}
