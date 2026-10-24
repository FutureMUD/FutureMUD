#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.GameItems;

namespace MudSharp_Unit_Tests;

[TestClass]
public class TerrainImmersionProfileTests
{
	[TestMethod]
	[DataRow(SizeCategory.VerySmall, ImmersionExtent.Full)]
	[DataRow(SizeCategory.Small, ImmersionExtent.Full)]
	[DataRow(SizeCategory.Normal, ImmersionExtent.Partial)]
	[DataRow(SizeCategory.Large, ImmersionExtent.Partial)]
	[DataRow(SizeCategory.VeryLarge, ImmersionExtent.Partial)]
	public void ImmersionSize_IsLargestCompletelyCoveredBottomSubject(SizeCategory subject, ImmersionExtent expected)
	{
		Assert.AreEqual(expected, new TerrainImmersionProfile(TerrainMedium.Liquid, SizeCategory.Small).ContactFor(subject).Extent);
	}

	[TestMethod]
	[DataRow(SizeCategory.Normal, Orientation.High, true)]
	[DataRow(SizeCategory.Large, Orientation.Centre, true)]
	[DataRow(SizeCategory.VeryLarge, Orientation.Low, false)]
	[DataRow(SizeCategory.Huge, Orientation.Lowest, false)]
	public void PartialContact_UsesCategoricalBandsAndWadingRange(SizeCategory subject, Orientation band, bool wading)
	{
		var contact = new TerrainImmersionProfile(TerrainMedium.Liquid, SizeCategory.Small).ContactFor(subject);
		Assert.AreEqual(band, contact.HighestImmersed);
		Assert.AreEqual(wading, contact.IsWading);
		Assert.IsTrue(contact.Covers(band));
		Assert.IsFalse(contact.Covers(Orientation.Highest));
		Assert.IsFalse(contact.Covers(Orientation.Appendage));
		Assert.IsFalse(ImmersionContact.None.Covers(Orientation.Lowest));
	}

}
