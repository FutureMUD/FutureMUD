#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DbEditableItem = MudSharp.Models.EditableItem;
using DbGameItemComponent = MudSharp.Models.GameItemComponent;
using DbGameItemComponentProto = MudSharp.Models.GameItemComponentProto;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SealAndMeasurementComponentTests
{
	[TestMethod]
	public void GameItemComponentManager_RegistersSealAndMeasuringTypes()
	{
		var manager = new GameItemComponentManager();
		var primaryTypes = manager.PrimaryTypes.ToList();
		var helpTypes = manager.TypeHelpInfo.Select(x => x.Name).ToList();

		CollectionAssert.Contains(primaryTypes, "sealstamp");
		CollectionAssert.Contains(primaryTypes, "sealable");
		CollectionAssert.Contains(primaryTypes, "measuringinstrument");
		CollectionAssert.Contains(helpTypes, "SealStamp");
		CollectionAssert.Contains(helpTypes, "Sealable");
		CollectionAssert.Contains(helpTypes, "MeasuringInstrument");
	}

	[TestMethod]
	public void ManipulationModule_ExposesSealAndMeasurementCommands()
	{
		var commands = typeof(ManipulationModule)
		               .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
		               .Select(x => x.GetCustomAttribute<PlayerCommand>())
		               .OfType<PlayerCommand>()
		               .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

		foreach (var name in new[] { "Seal", "Break", "Inspect", "Compare", "Weigh", "Measure", "Calibrate" })
		{
			Assert.IsTrue(commands.ContainsKey(name), $"Expected {name} command to be registered.");
		}
	}

	[TestMethod]
	public void Sealable_SealsBreaksSavesCopiesAndCompares()
	{
		var gameworld = CreateGameworld();
		var targetParent = CreateParent(gameworld.Object, 10L, "sealed tablet");
		var stampParent = CreateStampParent(gameworld.Object, 11L, "bronze");
		var actor = CreateActor(gameworld.Object, 20L);
		var stampProto = CreateSealStampProto(gameworld.Object, "a lion seal", "Temple", "bronze");
		var sealableProto = CreateSealableProto(gameworld.Object);
		var stamp = (SealStampGameItemComponent)stampProto.CreateNew(stampParent.Object, temporary: true);
		var sealable = (SealableGameItemComponent)sealableProto.CreateNew(targetParent.Object, temporary: true);

		Assert.IsTrue(sealable.CanSeal(actor.Object, stamp, null, out var error), error);

		sealable.Seal(actor.Object, stamp, null);

		Assert.IsTrue(sealable.IsSealed);
		Assert.AreEqual("a lion seal", sealable.CurrentSeal?.SealDesign);
		Assert.IsTrue(sealable.SealMatches(stamp));
		StringAssert.Contains(sealable.InspectSeal(actor.Object), "a lion seal");

		sealable.HandleEvent(MudSharp.Events.EventType.ItemOpened);

		Assert.IsFalse(sealable.IsSealed);
		Assert.IsTrue(sealable.SealBroken);
		Assert.IsTrue(sealable.HasSealResidue);
		StringAssert.Contains(InvokeSaveToXml(sealable), "<SealBroken>true</SealBroken>");

		var copy = (SealableGameItemComponent)sealable.Copy(CreateParent(gameworld.Object, 12L, "copy").Object, true);
		Assert.IsTrue(copy.SealBroken);
		Assert.AreEqual("a lion seal", copy.CurrentSeal?.SealDesign);
	}

	[TestMethod]
	public void MeasuringInstrument_DriftCalibrationAndWrongCalibrationPersist()
	{
		var gameworld = CreateGameworld();
		var actor = CreateActor(gameworld.Object, 30L);
		var proto = CreateMeasuringProto(gameworld.Object, "Weight", 0.001, 10000.0, 0.001);
		var poor = (MeasuringInstrumentGameItemComponent)proto.CreateNew(
			CreateParent(gameworld.Object, 20L, "poor scale", quality: ItemQuality.Terrible).Object, temporary: true);
		var excellent = (MeasuringInstrumentGameItemComponent)proto.CreateNew(
			CreateParent(gameworld.Object, 22L, "excellent scale", quality: ItemQuality.Legendary).Object, temporary: true);
		var target = CreateParent(gameworld.Object, 40L, "weight", weight: 1000.0);

		var poorResult = poor.Measure(actor.Object, target.Object);
		var excellentResult = excellent.Measure(actor.Object, target.Object);

		Assert.AreEqual(1, poor.UsesSinceCalibration);
		Assert.IsTrue(Math.Abs(poorResult.Drift) > Math.Abs(excellentResult.Drift));

		Assert.IsTrue(poor.CalibrateWrong(actor.Object, 0.10, true, out var error), error);
		Assert.IsTrue(poor.HasDeliberateBias);
		Assert.IsTrue(poor.CalibrationBiasIsPercentage);
		Assert.AreEqual(0.10, poor.CalibrationBias, 0.000001);

		var biasedResult = poor.Measure(actor.Object, target.Object);
		Assert.IsTrue(Math.Abs(biasedResult.CalibrationBias) > 0.0);
		StringAssert.Contains(poor.InspectCalibration(actor.Object), "Deliberate Bias");

		poor.Calibrate(actor.Object);

		Assert.IsFalse(poor.HasDeliberateBias);
		Assert.AreEqual(0, poor.UsesSinceCalibration);
		StringAssert.Contains(InvokeSaveToXml(poor), "<HasDeliberateBias>false</HasDeliberateBias>");
	}

	[TestMethod]
	public void MeasuringInstrument_LoadsSavesAndCopiesCalibrationState()
	{
		var gameworld = CreateGameworld();
		var proto = CreateMeasuringProto(gameworld.Object, "FluidVolume", 0.01, 2.0, 0.0005);
		var component = (MeasuringInstrumentGameItemComponent)proto.LoadComponent(
			new DbGameItemComponent
			{
				Definition = new XElement("Definition",
					new XElement("CalibrationBias", -0.05),
					new XElement("CalibrationBiasIsPercentage", true),
					new XElement("HasDeliberateBias", true),
					new XElement("UsesSinceCalibration", 7),
					new XElement("DriftDirection", -1)).ToString()
			},
			CreateParent(gameworld.Object, 50L, "cup").Object);

		Assert.AreEqual(MeasuringInstrumentMode.FluidVolume, component.Mode);
		Assert.AreEqual(-0.05, component.CalibrationBias, 0.000001);
		Assert.IsTrue(component.CalibrationBiasIsPercentage);
		Assert.IsTrue(component.HasDeliberateBias);
		Assert.AreEqual(7, component.UsesSinceCalibration);

		var copy = (MeasuringInstrumentGameItemComponent)component.Copy(
			CreateParent(gameworld.Object, 51L, "copy").Object, true);

		Assert.AreEqual(component.CalibrationBias, copy.CalibrationBias, 0.000001);
		Assert.AreEqual(component.UsesSinceCalibration, copy.UsesSinceCalibration);
		StringAssert.Contains(InvokeSaveToXml(component), "<UsesSinceCalibration>7</UsesSinceCalibration>");
	}

	private static SealStampGameItemComponentProto CreateSealStampProto(IFuturemud gameworld, string design,
		string issuer, string material)
	{
		return (SealStampGameItemComponentProto)Activator.CreateInstance(
			typeof(SealStampGameItemComponentProto),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[]
			{
				ComponentProtoModel(100, "SealStamp", new XElement("Definition",
					new XElement("SealDesign", new XCData(design)),
					new XElement("IssuerText", new XCData(issuer)),
					new XElement("OwnerText", new XCData(string.Empty)),
					new XElement("ClanText", new XCData(string.Empty)),
					new XElement("OfficeText", new XCData("scribe")),
					new XElement("StampMaterial", new XCData(material)),
					new XElement("ForgeryDifficulty", (int)Difficulty.Hard),
					new XElement("AuthorityProg", 0)).ToString()),
				gameworld
			},
			CultureInfo.InvariantCulture)!;
	}

	private static SealableGameItemComponentProto CreateSealableProto(IFuturemud gameworld)
	{
		return (SealableGameItemComponentProto)Activator.CreateInstance(
			typeof(SealableGameItemComponentProto),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[]
			{
				ComponentProtoModel(101, "Sealable", new XElement("Definition",
					new XElement("AllowedMedia", new XElement("Medium", new XCData("wax"))),
					new XElement("InspectionDifficulty", (int)Difficulty.Normal),
					new XElement("BrokenSealLeavesResidue", true)).ToString()),
				gameworld
			},
			CultureInfo.InvariantCulture)!;
	}

	private static MeasuringInstrumentGameItemComponentProto CreateMeasuringProto(IFuturemud gameworld, string mode,
		double precision, double capacity, double baseDrift)
	{
		return (MeasuringInstrumentGameItemComponentProto)Activator.CreateInstance(
			typeof(MeasuringInstrumentGameItemComponentProto),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[]
			{
				ComponentProtoModel(102, "MeasuringInstrument", new XElement("Definition",
					new XElement("Mode", mode),
					new XElement("Precision", precision),
					new XElement("Capacity", capacity),
					new XElement("BaseDriftPerUse", baseDrift),
					new XElement("MaximumDrift", 0.10),
					new XElement("MaximumWrongCalibration", 0.50),
					new XElement("CalibrationInspectionDifficulty", (int)Difficulty.Normal)).ToString()),
				gameworld
			},
			CultureInfo.InvariantCulture)!;
	}

	private static DbGameItemComponentProto ComponentProtoModel(long id, string type, string definition)
	{
		return new DbGameItemComponentProto
		{
			Id = id,
			Type = type,
			Name = type,
			Description = $"{type} component",
			Definition = definition,
			RevisionNumber = 1,
			EditableItem = new DbEditableItem
			{
				Id = id,
				BuilderAccountId = 1,
				BuilderDate = DateTime.UtcNow,
				RevisionNumber = 1,
				RevisionStatus = (int)RevisionStatus.Current
			}
		};
	}

	private static string InvokeSaveToXml(GameItemComponent component)
	{
		return (string)component.GetType()
		                .GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!
		                .Invoke(component, Array.Empty<object>())!;
	}

	private static Mock<IGameItem> CreateParent(IFuturemud gameworld, long id, string seen, double weight = 0.0,
		ItemQuality quality = ItemQuality.Standard)
	{
		var proto = new Mock<IGameItemProto>();
		proto.SetupGet(x => x.Id).Returns(id + 1000);
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Id).Returns(id);
		parent.SetupGet(x => x.Gameworld).Returns(gameworld);
		parent.SetupGet(x => x.Prototype).Returns(proto.Object);
		parent.SetupGet(x => x.Quality).Returns(quality);
		parent.SetupGet(x => x.Weight).Returns(weight);
		parent.Setup(x => x.GetItemType<ILiquidContainer>()).Returns((ILiquidContainer)null!);
		parent.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
				It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>()))
		      .Returns(seen);
		return parent;
	}

	private static Mock<IGameItem> CreateStampParent(IFuturemud gameworld, long id, string materialName)
	{
		var solid = new Mock<ISolid>();
		solid.SetupGet(x => x.Name).Returns(materialName);
		var parent = CreateParent(gameworld, id, "stamp");
		parent.SetupGet(x => x.Material).Returns(solid.Object);
		return parent;
	}

	private static Mock<ICharacter> CreateActor(IFuturemud gameworld, long id)
	{
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Id).Returns(id);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld);
		actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(true);
		actor.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
				It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>()))
		     .Returns("Tester");
		return actor;
	}

	private static Mock<IFuturemud> CreateGameworld()
	{
		var saveManager = new Mock<ISaveManager>();
		var unitManager = new Mock<IUnitManager>();
		unitManager.Setup(x => x.DescribeExact(It.IsAny<double>(), It.IsAny<UnitType>(), It.IsAny<IPerceiver>()))
		           .Returns<double, UnitType, IPerceiver>((value, type, _) => $"{value:N3} {type}");
		unitManager.Setup(x => x.DescribeExact(It.IsAny<double>(), It.IsAny<UnitType>(), It.IsAny<string>(),
				It.IsAny<IFormatProvider>()))
		           .Returns<double, UnitType, string, IFormatProvider>((value, type, _, _) => $"{value:N3} {type}");
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		gameworld.SetupGet(x => x.UnitManager).Returns(unitManager.Object);
		gameworld.SetupGet(x => x.FutureProgs).Returns(Repository<IFutureProg>());
		return gameworld;
	}

	private static IUneditableAll<T> Repository<T>(params T[] items) where T : class, IFrameworkItem
	{
		var repository = new Mock<IUneditableAll<T>>();
		repository.Setup(x => x.Get(It.IsAny<long>()))
		          .Returns<long>(id => items.FirstOrDefault(x => x.Id == id));
		repository.Setup(x => x.GetByName(It.IsAny<string>()))
		          .Returns<string>(name => items.FirstOrDefault(x => x.Name.EqualTo(name)));
		repository.Setup(x => x.Get(It.IsAny<string>()))
		          .Returns<string>(name => items.Where(x => x.Name.EqualTo(name)).ToList());
		repository.Setup(x => x.GetEnumerator())
		          .Returns(() => ((IEnumerable<T>)items).GetEnumerator());
		repository.SetupGet(x => x.Count).Returns(items.Length);
		return repository.Object;
	}
}
