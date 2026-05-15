using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;
using MudSharp.Work.Butchering;
using System.Collections.Generic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ButcheryExtensionsTests
{
	[TestMethod]
	public void MatchesButcherySubcategory_MainBreakdownSkipsAlreadyExtractedSubcategory()
	{
		var product = new Mock<IButcheryProduct>();
		product.SetupGet(x => x.Subcategory).Returns("venom");

		Assert.IsFalse(product.Object.MatchesButcherySubcategory(string.Empty, new[] { "venom" }));
	}

	[TestMethod]
	public void MatchesButcherySubcategory_MainBreakdownIncludesUnextractedSubcategory()
	{
		var product = new Mock<IButcheryProduct>();
		product.SetupGet(x => x.Subcategory).Returns("venom");

		Assert.IsTrue(product.Object.MatchesButcherySubcategory(string.Empty, new[] { "organs" }));
	}

	[TestMethod]
	public void MatchesButcherySubcategory_RequestedSubcategoryOnlyMatchesThatCategory()
	{
		var product = new Mock<IButcheryProduct>();
		product.SetupGet(x => x.Subcategory).Returns("venom");

		Assert.IsTrue(product.Object.MatchesButcherySubcategory("VENOM", new List<string>()));
		Assert.IsFalse(product.Object.MatchesButcherySubcategory("organs", new List<string>()));
	}

	[TestMethod]
	public void ShouldUseDamagedButcheryProduct_UsesDamagedOutputForFailedCheckOrDamage()
	{
		Assert.IsTrue(ButcheryExtensions.ShouldUseDamagedButcheryProduct(0.1, 0.5, Outcome.Fail));
		Assert.IsTrue(ButcheryExtensions.ShouldUseDamagedButcheryProduct(0.6, 0.5, Outcome.Pass));
		Assert.IsFalse(ButcheryExtensions.ShouldUseDamagedButcheryProduct(0.1, 0.5, Outcome.Pass));
	}
}
