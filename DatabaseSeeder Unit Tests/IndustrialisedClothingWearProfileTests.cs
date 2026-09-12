#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Framework;
using MudSharp.GameItems.Inventory;
using MudSharp.Models;
using WearProfile = MudSharp.Models.WearProfile;

namespace MudSharp_Unit_Tests;

[TestClass]
public class IndustrialisedClothingWearProfileTests
{
	private static readonly ClothingSourceLocation Source = new("Clothing/bases.tsv", 9);
	private const string Flags = "Mandatory=\"true\" Transparent=\"false\" NoArmour=\"true\" PreventsRemoval=\"true\" HidesSevered=\"true\"";
	private static BodyProto[] Bodies() => [new() { Id = 1, Name = "Base" }, new() { Id = 2, Name = "Derived", CountsAsId = 1 }];
	private static BodypartProto[] Parts() =>
	[
		new() { Id = 11, BodyId = 1, Name = "leftleg", BodypartShapeId = 21, BodypartType = (int)BodypartTypeEnum.Standing },
		new() { Id = 12, BodyId = 2, Name = "rightleg", BodypartShapeId = 21, BodypartType = (int)BodypartTypeEnum.Standing }
	];
	private static BodypartShape[] Shapes() => [new() { Id = 21, Name = "Leg" }, new() { Id = 22, Name = "Wing" }];
	private static IndustrialisedClothingWearProfiles Snapshot() => new(Bodies(), Parts(), Shapes());
	private static WearProfile Profile(string type = "Direct", string? geometry = null) => new()
	{
		Id = 51, Name = "Trousers", BodyPrototypeId = 2, Type = type,
		WearlocProfiles = geometry ?? $"<Profiles><Profile Bodypart=\"leftleg\" {Flags}/><Profile Bodypart=\"12\" {Flags}/></Profiles>"
	};

	[TestMethod]
	public void Direct_ResolvesInheritedAndLocalBodypartsAndPreservesEveryRuntimeFlag()
	{
		var result = Snapshot().Bind(Profile(), Source);
		Assert.IsFalse(result.IsShape);
		Assert.AreEqual(2L, result.BodyId);
		CollectionAssert.AreEqual(new long[] { 11, 12 }, result.Locations.Select(x => x.TargetId).ToArray());
		Assert.IsTrue(result.Locations.All(x => x.Count == 1 && x.Mandatory && !x.Transparent && x.NoArmour && x.PreventsRemoval && x.HidesSevered));
		Assert.ThrowsException<NotSupportedException>(() => ((IList<ClothingWearLocationBinding>)result.Locations).Clear());
	}

	[TestMethod]
	public void Snapshot_IsIndependentOfLaterTrackedDatabaseMutations()
	{
		var bodies = Bodies();
		var parts = Parts();
		var shapes = Shapes();
		var snapshot = new IndustrialisedClothingWearProfiles(bodies, parts, shapes);
		bodies[1].CountsAsId = 2;
		parts[0].Name = "changed";
		shapes[0].Id = 99;
		Assert.AreEqual(2, snapshot.Bind(Profile(), Source).Locations.Count);
	}

	[DataTestMethod]
	[DataRow("Leg")]
	[DataRow("21")]
	public void Shape_ResolvesNamesAndIdsWithExactCountsAndLegacyHidesSeveredDefault(string target)
	{
		var profile = Profile("Shape", $"<Profiles><Shape ShapeId=\"{target}\" Count=\"2\" {Flags.Replace(" HidesSevered=\"true\"", "")}/></Profiles>");
		var result = Snapshot().Bind(profile, Source);
		Assert.IsTrue(result.IsShape);
		Assert.AreEqual(21L, result.Locations.Single().TargetId);
		Assert.AreEqual(2, result.Locations.Single().Count);
		Assert.IsFalse(result.Locations.Single().HidesSevered);
	}

	[DataTestMethod]
	[DataRow("empty", "nonempty Profile")]
	[DataRow("wrong-root", "Profiles geometry root")]
	[DataRow("mixed-elements", "unknown location elements")]
	[DataRow("unknown-type", "unknown runtime type")]
	[DataRow("missing-body", "missing or ambiguous body")]
	[DataRow("duplicate-body", "missing or ambiguous body")]
	[DataRow("body-cycle", "cyclic")]
	[DataRow("missing-parent", "missing or ambiguous body")]
	[DataRow("non-wearable", "non-wearable bodypart")]
	[DataRow("other-body", "non-wearable bodypart")]
	[DataRow("duplicate-part-name", "ambiguous")]
	[DataRow("duplicate-location", "repeats the same")]
	[DataRow("missing-flag", "required Mandatory")]
	[DataRow("invalid-flag", "Invalid geometry")]
	[DataRow("missing-shape", "missing or ambiguous shape")]
	public void Direct_RejectsInvalidGeometryWithSourceDiagnostics(string fault, string diagnostic)
	{
		var bodies = Bodies().ToList();
		var parts = Parts().ToList();
		var shapes = Shapes().ToList();
		var profile = Profile();
		switch (fault)
		{
			case "empty": profile.WearlocProfiles = "<Profiles/>"; break;
			case "wrong-root": profile.WearlocProfiles = "<Definition/>"; break;
			case "mixed-elements": profile.WearlocProfiles = profile.WearlocProfiles.Replace("</Profiles>", "<Shape/></Profiles>"); break;
			case "unknown-type": profile.Type = "Guess"; break;
			case "missing-body": bodies.Clear(); break;
			case "duplicate-body": bodies.Add(new BodyProto { Id = 2 }); break;
			case "body-cycle": bodies[0].CountsAsId = 2; break;
			case "missing-parent": bodies[0].CountsAsId = 99; break;
			case "non-wearable": parts[0].BodypartType = (int)BodypartTypeEnum.Tongue; break;
			case "other-body": parts[0].BodyId = 3; break;
			case "duplicate-part-name": parts[1].Name = "leftleg"; break;
			case "duplicate-location": profile.WearlocProfiles = profile.WearlocProfiles.Replace("Bodypart=\"12\"", "Bodypart=\"11\""); break;
			case "missing-flag": profile.WearlocProfiles = profile.WearlocProfiles.Replace("Mandatory=\"true\"", ""); break;
			case "invalid-flag": profile.WearlocProfiles = profile.WearlocProfiles.Replace("Mandatory=\"true\"", "Mandatory=\"maybe\""); break;
			case "missing-shape": shapes.Clear(); break;
		}
		Failure(() => new IndustrialisedClothingWearProfiles(bodies, parts, shapes).Bind(profile, Source), diagnostic);
	}

	[DataTestMethod]
	[DataRow("99", "1", "true", "missing or ambiguous shape")]
	[DataRow("21", "0", "true", "positive shape count")]
	[DataRow("21", "-1", "true", "positive shape count")]
	[DataRow("21", "three", "true", "Invalid geometry")]
	[DataRow("21", "3", "true", "unavailable on its designed body")]
	[DataRow("22", "1", "false", "no wearable locations")]
	public void Shape_RejectsUnresolvableOrImpossibleGeometry(string target, string count, string mandatory, string diagnostic)
	{
		var profile = Profile("Shape", $"<Profiles><Shape ShapeId=\"{target}\" Count=\"{count}\" {Flags.Replace("Mandatory=\"true\"", $"Mandatory=\"{mandatory}\"")}/></Profiles>");
		Failure(() => Snapshot().Bind(profile, Source), diagnostic);
	}

	[TestMethod]
	public void WearLocationClassification_MatchesEveryRuntimeFactoryType()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		foreach (var type in Enum.GetValues<BodypartTypeEnum>())
		{
			var part = new BodypartProto { Id = 1, BodypartType = (int)type, Name = "fixture", Description = "fixture" };
			var runtime = BodypartPrototype.LoadFromDatabase(part, world.Object);
			Assert.AreEqual(runtime is IWear, IndustrialisedClothingWearProfiles.IsWearLocation(type), type.ToString());
			Assert.AreEqual(runtime is IExternalBodypart, IndustrialisedClothingWearProfiles.IsExternalLocation(type), type.ToString());
		}
		Assert.IsFalse(IndustrialisedClothingWearProfiles.IsWearLocation((BodypartTypeEnum)999));
		Assert.IsFalse(IndustrialisedClothingWearProfiles.IsExternalLocation((BodypartTypeEnum)999));
	}

	[DataTestMethod]
	[DataRow("Direct")]
	[DataRow("Shape")]
	public void BoundGeometry_AgreesWithRuntimeProfileLoader(string type)
	{
		var world = new Mock<IFuturemud>();
		var body = new Mock<IBodyPrototype>();
		body.Setup(x => x.Id).Returns(2);
		var shape = new Mock<MudSharp.Form.Shape.IBodypartShape>();
		shape.Setup(x => x.Id).Returns(21);
		shape.Setup(x => x.Name).Returns("Leg");
		var parts = Parts().Select(p =>
		{
			var part = new Mock<IExternalBodypart>();
			part.Setup(x => x.Id).Returns(p.Id);
			part.Setup(x => x.Name).Returns(p.Name);
			part.Setup(x => x.Shape).Returns(shape.Object);
			part.As<IWear>();
			return part.Object;
		}).ToArray();
		body.Setup(x => x.AllExternalBodyparts).Returns(parts);
		body.Setup(x => x.AllBodyparts).Returns(parts);
		world.Setup(x => x.BodyPrototypes).Returns(Collection(body.Object));
		world.Setup(x => x.BodypartShapes).Returns(Collection(shape.Object));
		var profile = type == "Direct" ? Profile() : Profile("Shape", $"<Profiles><Shape ShapeId=\"21\" Count=\"2\" {Flags}/></Profiles>");
		var bound = Snapshot().Bind(profile, Source);
		var runtime = MudSharp.GameItems.Inventory.WearProfile.LoadWearProfile(profile, world.Object);
		Assert.AreEqual(2, runtime.AllProfiles.Count);
		foreach (var location in runtime.AllProfiles)
		{
			var expected = bound.IsShape ? bound.Locations.Single() : bound.Locations.Single(x => x.TargetId == location.Key.Id);
			Assert.AreEqual(expected.Mandatory, location.Value.Mandatory);
			Assert.AreEqual(expected.Transparent, location.Value.Transparent);
			Assert.AreEqual(expected.NoArmour, location.Value.NoArmour);
			Assert.AreEqual(expected.PreventsRemoval, location.Value.PreventsRemoval);
			Assert.AreEqual(expected.HidesSevered, location.Value.HidesSeveredBodyparts);
		}
	}

	[TestMethod]
	public void OptionalExternalNonWearLocation_MatchesRuntimeLoadAndActualWearerFootprint()
	{
		var rows = Parts();
		rows[1].BodypartType = (int)BodypartTypeEnum.Tongue;
		var profile = Profile(geometry: $"<Profiles><Profile Bodypart=\"11\" {Flags}/><Profile Bodypart=\"12\" {Flags.Replace("Mandatory=\"true\"", "Mandatory=\"false\"")}/></Profiles>");
		var bound = new IndustrialisedClothingWearProfiles(Bodies(), rows, Shapes()).Bind(profile, Source);
		Assert.AreEqual(2, bound.Locations.Count, "Retain the optional reference for CountsAs-aware wearer resolution.");
		Assert.IsTrue(bound.Locations[0].IsWearLocation);
		Assert.IsFalse(bound.Locations[1].IsWearLocation);
		Assert.IsFalse(bound.Locations[1].Mandatory);
		var shape = new Mock<MudSharp.Form.Shape.IBodypartShape>();
		shape.Setup(x => x.Id).Returns(21);
		var leg = new Mock<IExternalBodypart>();
		leg.Setup(x => x.Id).Returns(11);
		leg.Setup(x => x.Shape).Returns(shape.Object);
		leg.Setup(x => x.CountsAs(It.IsAny<IBodypart>())).Returns((IBodypart part) => part.Id == 11);
		leg.As<IWear>();
		var tongue = new Mock<IExternalBodypart>();
		tongue.Setup(x => x.Id).Returns(12);
		tongue.Setup(x => x.Shape).Returns(shape.Object);
		var body = new Mock<IBodyPrototype>();
		body.Setup(x => x.Id).Returns(2);
		body.Setup(x => x.AllExternalBodyparts).Returns([leg.Object, tongue.Object]);
		var world = new Mock<IFuturemud>();
		world.Setup(x => x.BodyPrototypes).Returns(Collection(body.Object));
		var wearer = new Mock<IBody>();
		wearer.Setup(x => x.WearLocs).Returns([(IWear)leg.Object]);
		var runtime = MudSharp.GameItems.Inventory.WearProfile.LoadWearProfile(profile, world.Object);
		CollectionAssert.AreEqual(new long[] { 11 }, runtime.AllProfiles.Keys.Select(x => x.Id).ToArray());
		CollectionAssert.AreEqual(new long[] { 11 }, runtime.Profile(wearer.Object).Keys.Select(x => x.Id).ToArray());

		profile.WearlocProfiles = profile.WearlocProfiles.Replace("Mandatory=\"false\"", "Mandatory=\"true\"");
		Failure(() => new IndustrialisedClothingWearProfiles(Bodies(), rows, Shapes()).Bind(profile, Source), "requires non-wearable");
	}

	[DataTestMethod]
	[DataRow(BodypartTypeEnum.Liver)]
	[DataRow(BodypartTypeEnum.Bone)]
	public void OptionalInternalLocation_RemainsInvalidForRuntimeDirectLoader(BodypartTypeEnum type)
	{
		var parts = Parts();
		parts[1].BodypartType = (int)type;
		var profile = Profile(geometry: $"<Profiles><Profile Bodypart=\"11\" {Flags}/><Profile Bodypart=\"12\" {Flags.Replace("Mandatory=\"true\"", "Mandatory=\"false\"")}/></Profiles>");
		Failure(() => new IndustrialisedClothingWearProfiles(Bodies(), parts, Shapes()).Bind(profile, Source), "non-wearable bodypart");
	}

	private static IUneditableAll<T> Collection<T>(params T[] items) where T : class, IFrameworkItem
	{
		var collection = new Mock<IUneditableAll<T>>();
		collection.As<IEnumerable<T>>().Setup(x => x.GetEnumerator()).Returns(() => ((IEnumerable<T>)items).GetEnumerator());
		collection.Setup(x => x.Get(It.IsAny<long>())).Returns((long id) => items.SingleOrDefault(x => x.Id == id)!);
		return collection.Object;
	}

	[TestMethod]
	public void LayerLimit_UsesRuntimeDefaultOrExactOverrideWithoutMutatingSettings()
	{
		var expected = double.Parse(DefaultStaticSettings.DefaultStaticConfigurations["MaximumLayerWeight"], System.Globalization.CultureInfo.InvariantCulture);
		Assert.AreEqual(expected, IndustrialisedClothingWearProfiles.MaximumLayerWeight([], Source));
		var setting = new StaticConfiguration { SettingName = "MaximumLayerWeight", Definition = "2.5" };
		Assert.AreEqual(2.5, IndustrialisedClothingWearProfiles.MaximumLayerWeight([setting], Source));
		Assert.AreEqual("2.5", setting.Definition);
		Failure(() => IndustrialisedClothingWearProfiles.MaximumLayerWeight([setting, setting], Source), "ambiguous");
		setting.SettingName = "maximumlayerweight";
		Failure(() => IndustrialisedClothingWearProfiles.MaximumLayerWeight([setting], Source), "Missing exact");
	}

	[DataTestMethod]
	[DataRow("-1")]
	[DataRow("NaN")]
	[DataRow("Infinity")]
	[DataRow("4,0")]
	public void LayerLimit_RejectsInvalidConfiguration(string value) => Failure(() => IndustrialisedClothingWearProfiles.MaximumLayerWeight(
		[new StaticConfiguration { SettingName = "MaximumLayerWeight", Definition = value }], Source), "finite nonnegative");

	private static ClothingWornEntryBinding Entry(string key, double weight = 1, bool bulky = false) => new(Source, key,
		new(1, 0, 51, [51], bulky, weight), Snapshot().Bind(Profile(), Source));

	[TestMethod]
	public void MandatoryLayers_AccountPerBodyLocationAndPermitExactLimit()
	{
		var entries = new[] { Entry("undershirt"), Entry("shirt"), Entry("vest"), Entry("coat") };
		IndustrialisedClothingWearProfiles.ValidateMandatoryLayers(entries, 4);
		Failure(() => IndustrialisedClothingWearProfiles.ValidateMandatoryLayers(entries.Append(Entry("overcoat")), 4), "overcoat exceeds MaximumLayerWeight");
		var elsewhere = Entry("hat") with { Profile = new(52, "Hat", 2, false, [new(99, 1, true, false, false, true, false)]) };
		IndustrialisedClothingWearProfiles.ValidateMandatoryLayers(entries.Append(elsewhere), 4);
		Failure(() => IndustrialisedClothingWearProfiles.ValidateMandatoryLayers([Entry("heavy", 4.5)], 4), "alone exceeds");
	}

	[TestMethod]
	public void MandatoryBulkyConflicts_RequireBothGarmentsToBeBulkyAndMandatory()
	{
		Failure(() => IndustrialisedClothingWearProfiles.ValidateMandatoryLayers([Entry("inner", bulky: true), Entry("outer", bulky: true)], 4), "Bulky outfit entries inner and outer");
		IndustrialisedClothingWearProfiles.ValidateMandatoryLayers([Entry("inner"), Entry("outer", bulky: true)], 4);
		var optional = Entry("optional", bulky: true);
		optional = optional with { Profile = optional.Profile with { Locations = optional.Profile.Locations.Select(x => x with { Mandatory = false }).ToArray() } };
		IndustrialisedClothingWearProfiles.ValidateMandatoryLayers([optional, Entry("outer", bulky: true)], 4);
	}

	private static void Failure(Action action, string diagnostic)
	{
		var error = Assert.ThrowsException<InvalidDataException>(action);
		StringAssert.Contains(error.Message, Source.ToString());
		StringAssert.Contains(error.Message, diagnostic);
	}
}
