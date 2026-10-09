#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Prototypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BulkyWearSecurityTests
{
	[DataTestMethod]
	[DataRow(false, true, true, false)]
	[DataRow(true, true, true, false)]
	[DataRow(false, false, true, true)]
	[DataRow(false, true, false, true)]
	public void CanWear_MatchedWornLocation_PreservesBulkEligibility(bool exactLocation, bool mandatory, bool bulky, bool permitted)
	{
		var (body, candidate, profile, records) = Fixture(exactLocation, mandatory, bulky);
		Assert.AreEqual(1, new List<IGameItem>(body.WornItemsFor(profile.AllProfiles.Keys.Single())).Count);
		Assert.AreEqual(permitted, candidate.CanWear(body, profile));
		Assert.AreEqual(permitted ? WhyCannotDrapeReason.Unknown : WhyCannotDrapeReason.TooBulky,
			candidate.WhyCannotWear(body, profile));
	}

	[TestMethod]
	public void CanWear_OneItemHasOptionalAndMandatoryMatchingRecords_UsesEachRecord()
	{
		var (body, candidate, profile, records) = Fixture(false, false, true);
		var first = records[0];
		records.Add((first.Item, first.Wearloc, Mock.Of<IWearlocProfile>(x => x.Mandatory)));
		Assert.IsFalse(candidate.CanWear(body, profile));
		Assert.AreEqual(WhyCannotDrapeReason.TooBulky, candidate.WhyCannotWear(body, profile));
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void MissingProfileMapping_RejectsComponentAndDiagnosticWithoutException(bool bulky)
	{
		var (body, candidate, _, _) = Fixture(false, true, true);
		typeof(WearableGameItemComponentProto).GetProperty(nameof(WearableGameItemComponentProto.Bulky))!
			.SetValue(candidate.Prototype, bulky);
		var profile = new Mock<IWearProfile>();
		profile.Setup(x => x.Profile(body)).Returns((Dictionary<IWear, IWearlocProfile>)null!);
		Assert.IsFalse(candidate.CanWear(body, profile.Object));
		Assert.AreEqual(WhyCannotDrapeReason.SpecificProfileNoMatch, candidate.WhyCannotWear(body, profile.Object));
		Assert.AreEqual(WhyCannotDrapeReason.SpecificProfileNoMatch,
			Array.Empty<IWear>().WhyCannotDrape(Mock.Of<IGameItem>(), profile.Object, body));
	}

	private static (Body Body, WearableGameItemComponent Candidate, IWearProfile Profile,
		List<(IGameItem Item, IWear Wearloc, IWearlocProfile Profile)> Records) Fixture(bool exactLocation, bool mandatory, bool bulky)
	{
		var specific = new Mock<IWear>();
		var stored = exactLocation ? specific.Object : Mock.Of<IWear>();
		specific.Setup(x => x.CountsAs(stored)).Returns(true);
		var worn = new Mock<IGameItem>();
		worn.Setup(x => x.GetItemType<IWearable>()).Returns(Mock.Of<IWearable>(x => x.Bulky == bulky));
		var records = new List<(IGameItem, IWear, IWearlocProfile)>
		{
			(worn.Object, stored, Mock.Of<IWearlocProfile>(x => x.Mandatory == mandatory))
		};
		var body = TestObjectFactory.CreateUninitialized<Body>();
		typeof(Body).GetField("_wornItems", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(body, records);
		var mapping = new Dictionary<IWear, IWearlocProfile> { [specific.Object] = Mock.Of<IWearlocProfile>(x => x.Mandatory) };
		var profile = new Mock<IWearProfile>();
		profile.SetupGet(x => x.AllProfiles).Returns(mapping);
		profile.Setup(x => x.Profile(body)).Returns(mapping);
		var proto = TestObjectFactory.CreateUninitialized<WearableGameItemComponentProto>();
		typeof(WearableGameItemComponentProto).GetProperty(nameof(WearableGameItemComponentProto.Bulky))!.SetValue(proto, true);
		var candidate = TestObjectFactory.CreateUninitialized<WearableGameItemComponent>();
		typeof(WearableGameItemComponent).GetField("_prototype", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(candidate, proto);
		return (body, candidate, profile.Object, records);
	}
}
