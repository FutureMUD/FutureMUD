#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Prototypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ClothingWearablePersistenceTests
{
	[DataTestMethod]
	[DataRow("en-AU")]
	[DataRow("fr-FR")]
	[DataRow("de-DE")]
	public void FractionalWearValues_RoundTripIndependentlyOfHostCulture(string culture)
	{
		var previous = CultureInfo.CurrentCulture;
		try
		{
			CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
			foreach (var weight in new[] { 0.0, 0.25, 0.5, 0.75, 1.0 })
			{
				var xml = new XElement("Definition", new XAttribute("Bulky", true), new XAttribute("DisplayInventoryWhenWorn", false),
					new XElement("Profiles", new XAttribute("Default", 2), new XElement("Profile", 1), new XElement("Profile", 2)),
					new XElement("LayerWeightConsumption", weight), new XElement("SeeThroughDamageRatio", 0.25),
					new XElement("Waterproof", new XAttribute("ratio", 0.75), true));
				var world = World();
				var loaded = new TestWearable(xml.ToString(), world);
				var reloaded = new TestWearable(loaded.Xml(), world);
				foreach (var item in new[] { loaded, reloaded })
				{
					Assert.AreEqual(weight, item.LayerWeightConsumption);
					Assert.AreEqual(0.25, item.SeeThroughDamageRatio);
					Assert.AreEqual(0.75, item.WaterproofDamageRatio);
					Assert.IsTrue(item.Waterproof);
					Assert.IsTrue(item.Bulky);
					Assert.IsFalse(item.DisplayInventoryWhenWorn);
					Assert.AreEqual(2L, item.DefaultProfile.Id);
					CollectionAssert.AreEqual(new long[] { 1, 2 }, item.Profiles.Select(x => x.Id).ToArray());
				}
			}
		}
		finally { CultureInfo.CurrentCulture = previous; }
	}

	[TestMethod]
	public void LegacyWearXml_StillUsesDefaultLayerAndDamageRatios()
	{
		var item = new TestWearable("<Definition><Profiles Default=\"1\"><Profile>1</Profile></Profiles></Definition>", World());
		Assert.AreEqual(1.0, item.LayerWeightConsumption);
		Assert.AreEqual(0.5, item.SeeThroughDamageRatio);
		Assert.AreEqual(0.5, item.WaterproofDamageRatio);
	}

	private static IFuturemud World()
	{
		var profiles = new long[] { 1, 2 }.Select(id =>
		{
			var profile = new Mock<IWearProfile>();
			profile.Setup(x => x.Id).Returns(id);
			return profile.Object;
		}).ToArray();
		var world = new Mock<IFuturemud>();
		var collection = new Mock<IUneditableAll<IWearProfile>>();
		collection.As<IEnumerable<IWearProfile>>().Setup(x => x.GetEnumerator()).Returns(() => profiles.AsEnumerable().GetEnumerator());
		collection.Setup(x => x.Get(It.IsAny<long>())).Returns((long id) => profiles.SingleOrDefault(x => x.Id == id)!);
		world.Setup(x => x.WearProfiles).Returns(collection.Object);
		world.Setup(x => x.FutureProgs).Returns(new Mock<IUneditableAll<MudSharp.FutureProg.IFutureProg>>().Object);
		return world.Object;
	}

	private sealed class TestWearable(string xml, IFuturemud world) : WearableGameItemComponentProto(new MudSharp.Models.GameItemComponentProto
	{
		Id = 41, Name = "Wear_Test_Layer", Description = "Test clothing configuration", Type = "Wearable", Definition = xml,
		EditableItem = new MudSharp.Models.EditableItem { RevisionStatus = 4 }
	}, world)
	{
		internal string Xml() => SaveToXml();
	}
}
