#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Events.Hooks;
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
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DbEditableItem = MudSharp.Models.EditableItem;
using DbGameItemComponentProto = MudSharp.Models.GameItemComponentProto;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ConditionMaintenanceTests
{
	[TestMethod]
	public void Profile_LegacyXmlLoadsDisabledAndDoesNotPenaliseQuality()
	{
		var profile = new ConditionMaintenanceProfile("0.0005");
		profile.LoadFromXml(new XElement("Definition"));
		var parent = CreateConditionItem(condition: 0.01);

		profile.UseCondition(parent.Object, new ItemConditionUseContext(ItemConditionUseKind.MeleeAttack));

		Assert.IsFalse(profile.ConditionDegradesOnUse);
		Assert.AreEqual(0.01, parent.Object.Condition, 0.000001);
		Assert.AreEqual(0, profile.QualityPenaltyStages(parent.Object));
	}

	[TestMethod]
	public void Profile_QualityPenaltyDefaultOnlyAppliesBelowTwentyPercent()
	{
		var profile = EnabledProfile("0.0005");
		var parent = CreateConditionItem(condition: 1.0);

		parent.Object.Condition = 0.20;
		Assert.AreEqual(0, profile.QualityPenaltyStages(parent.Object));

		parent.Object.Condition = 0.199;
		Assert.AreEqual(-1, profile.QualityPenaltyStages(parent.Object));

		parent.Object.Condition = 0.119;
		Assert.AreEqual(-3, profile.QualityPenaltyStages(parent.Object));

		parent.Object.Condition = 0.039;
		Assert.AreEqual(-5, profile.QualityPenaltyStages(parent.Object));
	}

	[TestMethod]
	public void Profile_UseLossFormulaClampsConditionAtZero()
	{
		var profile = new ConditionMaintenanceProfile("0.0005");
		profile.LoadFromXml(new XElement("Definition",
			new XElement("ConditionDegradesOnUse", true),
			new XElement("ConditionUseFormula", new XCData("damage+absorbed+passed+degree"))));
		var parent = CreateConditionItem(condition: 0.05);

		profile.UseCondition(parent.Object,
			new ItemConditionUseContext(ItemConditionUseKind.ArmourAbsorb, Outcome.Pass, 0.02, 0.02, 0.02, 0.02));

		Assert.AreEqual(0.0, parent.Object.Condition, 0.000001);
	}

	[TestMethod]
	public void Profile_RangedMeleeDefaultUsesRangedLossOnlyForFiring()
	{
		var profile = EnabledProfile(ConditionMaintenanceProfile.DefaultRangedOrMeleeUseExpression);
		var parent = CreateConditionItem(condition: 1.0);

		profile.UseCondition(parent.Object, new ItemConditionUseContext(ItemConditionUseKind.RangedFire));
		Assert.AreEqual(0.9995, parent.Object.Condition, 0.000001);

		profile.UseCondition(parent.Object, new ItemConditionUseContext(ItemConditionUseKind.MeleeAttack));
		Assert.AreEqual(0.99925, parent.Object.Condition, 0.000001);

		profile.UseCondition(parent.Object, new ItemConditionUseContext(ItemConditionUseKind.Parry));
		Assert.AreEqual(0.999, parent.Object.Condition, 0.000001);
	}

	[TestMethod]
	public void Profile_ShieldDefaultUsesBlockLossOnlyForBlocks()
	{
		var profile = EnabledProfile(ConditionMaintenanceProfile.DefaultShieldOrMeleeUseExpression);
		var parent = CreateConditionItem(condition: 1.0);

		profile.UseCondition(parent.Object, new ItemConditionUseContext(ItemConditionUseKind.ShieldBlock));
		Assert.AreEqual(0.9995, parent.Object.Condition, 0.000001);

		profile.UseCondition(parent.Object, new ItemConditionUseContext(ItemConditionUseKind.MeleeAttack));
		Assert.AreEqual(0.99925, parent.Object.Condition, 0.000001);

		profile.UseCondition(parent.Object, new ItemConditionUseContext(ItemConditionUseKind.Parry));
		Assert.AreEqual(0.999, parent.Object.Condition, 0.000001);
	}

	[TestMethod]
	public void Profile_XmlRoundTripsEnabledUseAndQualityFormulas()
	{
		var profile = new ConditionMaintenanceProfile("0.0005");
		profile.LoadFromXml(new XElement("Definition",
			new XElement("ConditionDegradesOnUse", true),
			new XElement("ConditionUseFormula", new XCData("0.125")),
			new XElement("ConditionQualityFormula", new XCData("if(condition<0.2,-2,0)"))));

		var roundTripped = new ConditionMaintenanceProfile("0.0005");
		roundTripped.LoadFromXml(new XElement("Definition", profile.SaveToXml()));
		var parent = CreateConditionItem(condition: 0.19);

		Assert.IsTrue(roundTripped.ConditionDegradesOnUse);
		Assert.AreEqual("0.125", roundTripped.ConditionUseExpression.OriginalExpression);
		Assert.AreEqual("if(condition<0.2,-2,0)", roundTripped.ConditionQualityPenaltyExpression.OriginalExpression);
		Assert.AreEqual(-2, roundTripped.QualityPenaltyStages(parent.Object));
	}

	[TestMethod]
	public void GameItem_ConditionSetterClampsToValidRange()
	{
		var gameworld = CreateGameworld();
		var proto = new Mock<IGameItemProto>();
		proto.SetupGet(x => x.Id).Returns(10L);
		proto.SetupGet(x => x.Name).Returns("test item");
		proto.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		proto.SetupGet(x => x.Components).Returns(Array.Empty<IGameItemComponentProto>());
		proto.SetupGet(x => x.Morphs).Returns(false);
		proto.SetupGet(x => x.Keywords).Returns(["test", "item"]);

		var item = new GameItem(proto.Object, quality: ItemQuality.Standard)
		{
			Condition = 2.0
		};
		Assert.AreEqual(1.0, item.Condition, 0.000001);

		item.Condition = -0.5;
		Assert.AreEqual(0.0, item.Condition, 0.000001);
	}

	[TestMethod]
	public void MeasuringInstrument_ConditionMaintenanceConsumesPerMeasurement()
	{
		var gameworld = CreateGameworld();
		var actor = CreateActor(gameworld.Object);
		var proto = CreateMeasuringProto(gameworld.Object, useFormula: "0.1");
		var parent = CreateConditionItem(condition: 1.0);
		parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		parent.SetupGet(x => x.Quality).Returns(ItemQuality.Standard);
		var target = CreateConditionItem(condition: 1.0);
		target.SetupGet(x => x.Weight).Returns(1000.0);
		var component = (MeasuringInstrumentGameItemComponent)proto.CreateNew(parent.Object, temporary: true);

		component.Measure(actor.Object, target.Object);

		Assert.AreEqual(0.9, parent.Object.Condition, 0.000001);
		parent.Object.Condition = 0.10;
		Assert.AreEqual(-3, component.ItemQualityStages);
	}

	[TestMethod]
	public void BreathingFilter_ConsumableConditionClampsAndEvaluatesAtZero()
	{
		var filterProto = new Mock<IGameItemProto>();
		filterProto.SetupGet(x => x.Id).Returns(200L);
		var gameworld = CreateGameworld();
		var proto = CreateBreathingFilterProto(gameworld.Object, filterProto.Object.Id, volumePerFilter: 10.0);
		var parent = CreateConditionItem(condition: 1.0);
		parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		parent.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
				It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>()))
		      .Returns("a breathing mask");
		var consumable = CreateConditionItem(condition: 0.05);
		consumable.SetupGet(x => x.Prototype).Returns(filterProto.Object);
		consumable.SetupGet(x => x.Quantity).Returns(1);
		consumable.SetupProperty(x => x.ContainedIn);
		consumable.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
				It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>()))
		          .Returns("a spent filter");
		var actor = CreateActor(gameworld.Object);
		var component = (BreathingFilterGameItemComponent)proto.CreateNew(parent.Object, temporary: true);

		component.Put(null, consumable.Object);
		component.ConsumeGas(1.0);

		Assert.AreEqual(0.0, consumable.Object.Condition, 0.000001);
		Assert.IsTrue(component.DescriptionDecorator(DescriptionType.Evaluate));
		var evaluation = component.Decorate(actor.Object, "mask", "A mask.", DescriptionType.Evaluate, true,
			PerceiveIgnoreFlags.None);
		StringAssert.Contains(evaluation, "a spent filter");
		StringAssert.Contains(evaluation, 0.0.ToString("P0", actor.Object));
	}

	private static ConditionMaintenanceProfile EnabledProfile(string useFormula)
	{
		var profile = new ConditionMaintenanceProfile(useFormula);
		profile.LoadFromXml(new XElement("Definition", new XElement("ConditionDegradesOnUse", true)));
		return profile;
	}

	private static Mock<IGameItem> CreateConditionItem(double condition)
	{
		var parent = new Mock<IGameItem>();
		parent.SetupProperty(x => x.Condition, condition);
		parent.SetupGet(x => x.RawQuality).Returns(ItemQuality.Standard);
		parent.SetupGet(x => x.Skin).Returns((IGameItemSkin)null!);
		parent.SetupGet(x => x.Quality).Returns(ItemQuality.Standard);
		return parent;
	}

	private static MeasuringInstrumentGameItemComponentProto CreateMeasuringProto(IFuturemud gameworld,
		string useFormula)
	{
		return (MeasuringInstrumentGameItemComponentProto)Activator.CreateInstance(
			typeof(MeasuringInstrumentGameItemComponentProto),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[]
			{
				ComponentProtoModel(100, "MeasuringInstrument", new XElement("Definition",
					new XElement("Mode", MeasuringInstrumentMode.Weight.ToString()),
					new XElement("Precision", 0.01),
					new XElement("Capacity", 10000.0),
					new XElement("BaseDriftPerUse", 0.0005),
					new XElement("MaximumDrift", 0.10),
					new XElement("MaximumWrongCalibration", 0.50),
					new XElement("CalibrationInspectionDifficulty", (int)Difficulty.Normal),
					new XElement("ConditionDegradesOnUse", true),
					new XElement("ConditionUseFormula", new XCData(useFormula))).ToString()),
				gameworld
			},
			CultureInfo.InvariantCulture)!;
	}

	private static BreathingFilterGameItemComponentProto CreateBreathingFilterProto(IFuturemud gameworld,
		long filterProtoId, double volumePerFilter)
	{
		return (BreathingFilterGameItemComponentProto)Activator.CreateInstance(
			typeof(BreathingFilterGameItemComponentProto),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[]
			{
				ComponentProtoModel(101, "BreathingFilter", new XElement("Definition",
					new XElement("VolumePerFilter", volumePerFilter),
					new XElement("FilterProtoId", filterProtoId),
					new XElement("Gases")).ToString()),
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

	private static Mock<ICharacter> CreateActor(IFuturemud gameworld)
	{
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld);
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
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		gameworld.SetupGet(x => x.UnitManager).Returns(unitManager.Object);
		gameworld.SetupGet(x => x.DefaultHooks).Returns(Array.Empty<IDefaultHook>());
		gameworld.SetupGet(x => x.Gases).Returns(Repository<IGas>());
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
