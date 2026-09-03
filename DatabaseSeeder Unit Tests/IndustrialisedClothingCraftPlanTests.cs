#nullable enable

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class IndustrialisedClothingCraftPlanTests
{
	internal static IndustrialisedClothingCatalogueDocument Document()
	{
		var d = IndustrialisedClothingCatalogueTests.Load(IndustrialisedClothingCatalogueTests.Fixture());
		return d with { CraftProducts = d.CraftProducts.Select(x => x.FailureProduct
			? x with { Kind = ClothingProductKind.Commodity, Reference = "cotton", Quantity = 125 } : x).ToArray() };
	}

	[TestMethod]
	public void Compile_AuthoredRecipe_PreservesTextAndBuildsStableProductsWithHonestFailureMass()
	{
		var d = Document();
		var craft = d.Crafts.Single();
		var spec = IndustrialisedClothingCraftPlan.Compile(d, craft);
		Assert.AreEqual(craft.StableReference, spec.StableReference);
		Assert.IsTrue(spec.PreserveAuthoredText);
		Assert.AreEqual(craft.Blurb, spec.Blurb);
		Assert.AreEqual(craft.ActiveItemDescription, spec.ActiveCraftItemSdesc);
		CollectionAssert.AreEqual(d.CraftPhases.Select(x => x.Echo).ToArray(), spec.Phases.Select(x => x.Echo).ToArray());
		CollectionAssert.AreEqual(d.CraftPhases.Select(x => x.FailEcho).ToArray(), spec.Phases.Select(x => x.FailEcho).ToArray());
		Assert.AreEqual("500 grams of cotton", spec.Inputs.Single().Details);
		Assert.AreEqual("characteristic Garment Colour any", spec.Inputs.Single().Options.Single());
		Assert.AreEqual("Held - an item with the Tools / Scissors tag", spec.Tools.Single().Details);
		Assert.AreEqual("1x @coat", spec.Products.Single().Details);
		CollectionAssert.AreEqual(new[] { "skin @trimmed_coat", "variable Garment Colour=$i1" }, spec.Products.Single().Options.ToArray());
		Assert.AreEqual("125 grams of cotton commodity", spec.FailProducts.Single().Details);
		Assert.IsNull(spec.Products.Single().MaterialDefiningInputIndex);
	}

	[TestMethod]
	public void Compile_UnskinnedSelectedColour_RequiresNoSyntheticSkinOrInheritedInput()
	{
		var d = Document();
		d = d with
		{
			CraftProducts = d.CraftProducts.Select(x => x with { SkinReference = "" }).ToArray(),
			CraftColours = d.CraftColours.Select(x => x with { Value = "cream", InputOrder = null }).ToArray()
		};
		var spec = IndustrialisedClothingCraftPlan.Compile(d, d.Crafts.Single());
		Assert.AreEqual("fixedvariable Garment Colour=cream", spec.Products.Single().Options.Single());
		Assert.AreEqual(0, spec.Inputs.Single().Options.Count);
		Assert.AreEqual("1x @coat", spec.Products.Single().Details);
	}

	[TestMethod]
	public void Compile_DecimalUnitsAndQualityWeights_AreInvariant()
	{
		var previous = CultureInfo.CurrentCulture;
		try
		{
			CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
			var d = Document();
			d = d with { CraftInputs = d.CraftInputs.Select(x => x with { Quantity = 500.25, QualityWeight = 2.5 }).ToArray() };
			var spec = IndustrialisedClothingCraftPlan.Compile(d, d.Crafts.Single());
			Assert.AreEqual("500.25 grams of cotton", spec.Inputs.Single().Details);
			Assert.AreEqual(2.5, spec.Inputs.Single().QualityWeight);
		}
		finally { CultureInfo.CurrentCulture = previous; }
	}

	[TestMethod]
	[DataRow("input")]
	[DataRow("tool")]
	[DataRow("product")]
	[DataRow("failure")]
	[DataRow("early-failure")]
	[DataRow("wrong-failure-product")]
	[DataRow("missing-tool")]
	[DataRow("missing-input")]
	[DataRow("missing-product")]
	[DataRow("unproduced-success")]
	[DataRow("unconsumed-colour")]
	public void Compile_InvalidPhaseGraph_HasSourceDiagnosticAndNeverPatchesProse(string fault)
	{
		var d = Document();
		var phases = d.CraftPhases.ToArray();
		switch (fault)
		{
			case "input": phases[1] = phases[1] with { Echo = "$0 use|uses $i99 and finish|finishes $p1." }; break;
			case "tool": phases[1] = phases[1] with { Echo = "$0 use|uses $t99 and finish|finishes $p1." }; break;
			case "product": phases[1] = phases[1] with { Echo = "$0 finish|finishes $p99." }; break;
			case "failure": phases[1] = phases[1] with { Echo = "$0 finish|finishes $f1." }; break;
			case "early-failure": phases[0] = phases[0] with { FailEcho = "$0 fail|fails." }; break;
			case "wrong-failure-product": phases[1] = phases[1] with { FailEcho = "$0 recover|recovers $f99." }; break;
			case "missing-tool": phases[0] = phases[0] with { Echo = "$0 cut|cuts $i1.", FailEcho = "$0 cut|cuts $i1." }; break;
			case "missing-input": phases[0] = phases[0] with { Echo = "$0 use|uses $t1.", FailEcho = "$0 use|uses $t1." }; break;
			case "missing-product": phases[1] = phases[1] with { Echo = "$0 finish|finishes." }; break;
			case "unproduced-success": phases[1] = phases[1] with { FailEcho = "$0 recover|recovers $f1 beside $p1." }; break;
			case "unconsumed-colour":
				phases[0] = phases[0] with { Echo = "$0 use|uses $t1 and finish|finishes $p1.", FailEcho = "$0 use|uses $t1 and finish|finishes $p1." };
				phases[1] = phases[1] with { Echo = "$0 use|uses $i1." };
				break;
		}
		d = d with { CraftPhases = phases };
		var before = phases.Select(x => x.Echo + x.FailEcho).ToArray();
		var ex = Assert.ThrowsException<InvalidDataException>(() => IndustrialisedClothingCraftPlan.Compile(d, d.Crafts.Single()));
		StringAssert.Contains(ex.Message, "Clothing/");
		CollectionAssert.AreEqual(before, phases.Select(x => x.Echo + x.FailEcho).ToArray());
	}

	[TestMethod]
	[DataRow("Item")]
	[DataRow("Liquid")]
	public void Compile_IncapableColourInput_IsRejected(string kind)
	{
		var d = Document();
		d = d with { CraftInputs = d.CraftInputs.Select(x => x with { Kind = Enum.Parse<ClothingInputKind>(kind) }).ToArray() };
		StringAssert.Contains(Assert.ThrowsException<InvalidDataException>(() => IndustrialisedClothingCraftPlan.Compile(d, d.Crafts.Single())).Message, "cannot transmit");
	}

	[TestMethod]
	[DataRow(0.29, "29%")]
	[DataRow(0.58, "58%")]
	[DataRow(0.125, "12.5%")]
	public void Compile_RecoveryFraction_DoesNotIntroduceBinaryRounding(double fraction, string percentage)
	{
		var d = Document();
		d = d with
		{
			CraftInputs = d.CraftInputs.Select(x => x with { Kind = ClothingInputKind.Item, Reference = "coat", Quantity = 10 }).ToArray(),
			CraftProducts = d.CraftProducts.Select(x => x.FailureProduct
				? x with { Kind = ClothingProductKind.UnusedInput, Reference = "1", Quantity = fraction } : x).ToArray(),
			CraftColours = d.CraftColours.Select(x => x with { Value = "cream", InputOrder = null }).ToArray()
		};
		var spec = IndustrialisedClothingCraftPlan.Compile(d, d.Crafts.Single());
		Assert.AreEqual($"{percentage} of 10x @coat ($i1)", spec.FailProducts.Single().Details);
	}

	[TestMethod]
	public void Compile_UnusedCommodityInput_IsNotPretendedToBeMassRecovery()
	{
		var d = IndustrialisedClothingCatalogueTests.Load(IndustrialisedClothingCatalogueTests.Fixture());
		StringAssert.Contains(Assert.ThrowsException<InvalidDataException>(() => IndustrialisedClothingCraftPlan.Compile(d, d.Crafts.Single())).Message, "not liquid/commodity mass");
	}
}
