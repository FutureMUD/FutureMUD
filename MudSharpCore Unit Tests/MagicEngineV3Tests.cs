#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.SpellTriggers;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicEngineV3Tests
{
	private static readonly Dictionary<string, string> EngineV3EffectTypes = new(StringComparer.InvariantCultureIgnoreCase)
	{
		["detectpoison"] = "detectpoison",
		["insomnia"] = "insomnia",
		["removeinsomnia"] = "removeinsomnia",
		["removeblindness"] = "removeblindness",
		["cureblindness"] = "removeblindness"
	};

	[TestInitialize]
	public void TestInitialize()
	{
		SpellTriggerFactory.SetupFactory();
		SpellEffectFactory.SetupFactory();
	}

	[TestMethod]
	public void SpellEffectFactory_RegistersEngineV3EffectTypes()
	{
		var spell = CreateSpellMock();

		foreach (var (type, savedType) in EngineV3EffectTypes)
		{
			var (effect, error) = SpellEffectFactory.LoadEffectFromBuilderInput(type, new StringStack(string.Empty),
				spell.Object);

			Assert.AreEqual(string.Empty, error, $"Unexpected builder error for {type}.");
			Assert.IsNotNull(effect, $"No builder effect was created for {type}.");
			Assert.AreEqual(savedType, effect!.SaveToXml().Attribute("type")?.Value);
			Assert.IsTrue(SpellEffectFactory.BuilderInfoForType(type).MatchingTriggers.Any());
		}
	}

	[TestMethod]
	public void SpellEffectFactory_LoadEffect_RoundTripsEngineV3Xml()
	{
		var spell = CreateSpellMock();
		var definitions = new Dictionary<string, string>
		{
			["detectpoison"] = "detectpoison",
			["insomnia"] = "insomnia",
			["removeinsomnia"] = "removeinsomnia",
			["removeblindness"] = "removeblindness",
			["cureblindness"] = "removeblindness"
		};

		foreach (var (type, savedType) in definitions)
		{
			var effect = SpellEffectFactory.LoadEffect(new XElement("Effect", new XAttribute("type", type)),
				spell.Object);

			Assert.AreEqual(savedType, effect.SaveToXml().Attribute("type")?.Value);
		}

		var dispel = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "dispelmagic"),
			new XElement("Contest", true),
			new XElement("ContestBonus", 3),
			new XElement("EffectKey", new XCData("any"))), spell.Object);

		var saved = dispel.SaveToXml();
		Assert.AreEqual(bool.TrueString.ToLowerInvariant(), saved.Element("Contest")?.Value.ToLowerInvariant());
		Assert.AreEqual("3", saved.Element("ContestBonus")?.Value);
	}

	[TestMethod]
	public void DetectPoisonEffect_ReportsActiveAndLatentDrugDosages()
	{
		var gameworld = CreateGameworldWithUnits();
		var output = new Mock<IOutputHandler>();
		var caster = CreateCharacter(1, gameworld.Object);
		caster.SetupGet(x => x.OutputHandler).Returns(output.Object);
		var target = CreateCharacter(2, gameworld.Object);
		var body = new Mock<IBody>();
		var activeDrug = CreateDrug(10, "Black Lotus");
		var latentDrug = CreateDrug(11, "Glass Dust");
		body.SetupGet(x => x.ActiveDrugDosages).Returns([
			new DrugDosage { Drug = activeDrug.Object, Grams = 2.0, OriginalVector = DrugVector.Injected }
		]);
		body.SetupGet(x => x.LatentDrugDosages).Returns([
			new DrugDosage { Drug = latentDrug.Object, Grams = 1.5, OriginalVector = DrugVector.Ingested }
		]);
		target.SetupGet(x => x.Body).Returns(body.Object);
		target.Setup(x => x.HowSeen(caster.Object, It.IsAny<bool>(), It.IsAny<DescriptionType>(),
				It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>()))
		      .Returns("the test subject");
		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect", new XAttribute("type", "detectpoison")),
			CreateSpellMock(gameworld: gameworld.Object).Object);

		effect.GetOrApplyEffect(caster.Object, target.Object, OpposedOutcomeDegree.None,
			SpellPower.Standard, CreateParent().Object, []);

		output.Verify(x => x.Send(It.Is<string>(text =>
				text.Contains("Drugs for the test subject:") &&
				text.Contains("Black Lotus") &&
				text.Contains("#10") &&
				text.Contains("Glass Dust") &&
				text.Contains("#11") &&
				text.Contains("localized mass") &&
				text.Contains("via Ingested (latent)")),
			true, false), Times.Once);
	}

	[TestMethod]
	public void DetectPoisonEffect_ReportsNoDrugEffects()
	{
		var gameworld = CreateGameworldWithUnits();
		var output = new Mock<IOutputHandler>();
		var caster = CreateCharacter(1, gameworld.Object);
		caster.SetupGet(x => x.OutputHandler).Returns(output.Object);
		var target = CreateCharacter(2, gameworld.Object);
		var body = new Mock<IBody>();
		body.SetupGet(x => x.ActiveDrugDosages).Returns([]);
		body.SetupGet(x => x.LatentDrugDosages).Returns([]);
		target.SetupGet(x => x.Body).Returns(body.Object);
		target.Setup(x => x.HowSeen(caster.Object, It.IsAny<bool>(), It.IsAny<DescriptionType>(),
				It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>()))
		      .Returns("the test subject");
		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect", new XAttribute("type", "detectpoison")),
			CreateSpellMock(gameworld: gameworld.Object).Object);

		effect.GetOrApplyEffect(caster.Object, target.Object, OpposedOutcomeDegree.None,
			SpellPower.Standard, CreateParent().Object, []);

		output.Verify(x => x.Send(It.Is<string>(text =>
				text.Contains("No active or latent drug effects detected.")),
			true, false), Times.Once);
	}

	[TestMethod]
	public void RemoveBlindnessEffect_RemovesSpellBlindness()
	{
		var target = CreateCharacter(2);
		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect", new XAttribute("type", "removeblindness")),
			CreateSpellMock().Object);

		effect.GetOrApplyEffect(CreateCharacter(1).Object, target.Object, OpposedOutcomeDegree.None,
			SpellPower.Standard, CreateParent().Object, []);

		target.Verify(x => x.RemoveAllEffects<SpellBlindnessEffect>(null, true), Times.Once);
	}

	[TestMethod]
	public void SleepEffect_DoesNotApplyWhenInsomniaBlocksSleep()
	{
		var output = new Mock<IOutputHandler>();
		var target = CreateCharacter(2);
		var blocker = new Mock<IPreventSleepEffect>();
		blocker.SetupGet(x => x.SleepPreventionEcho).Returns("No sleep.");
		target.SetupGet(x => x.OutputHandler).Returns(output.Object);
		target.Setup(x => x.EffectsOfType<IPreventSleepEffect>(It.IsAny<Predicate<IPreventSleepEffect>>()))
		      .Returns<Predicate<IPreventSleepEffect>>(predicate => new[] { blocker.Object }.Where(x => predicate?.Invoke(x) ?? true));
		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect", new XAttribute("type", "sleep")),
			CreateSpellMock().Object);

		var applied = effect.GetOrApplyEffect(CreateCharacter(1).Object, target.Object, OpposedOutcomeDegree.None,
			SpellPower.Standard, CreateParent().Object, []);

		Assert.IsNull(applied);
		output.Verify(x => x.Send("No sleep.", true, false), Times.Once);
	}

	[TestMethod]
	public void DispelMagicEffect_DefaultContestOffStillRemovesStrongerSpell()
	{
		var caster = CreateCharacter(1);
		var target = CreatePerceivable();
		var existingSpell = CreateSpellMock(20);
		var parent = new MagicSpellParent(target.Object, existingSpell.Object, caster.Object,
			SpellPower.RecklesslyPowerful, OpposedOutcomeDegree.Total);
		target.Setup(x => x.EffectsOfType<MagicSpellParent>(It.IsAny<Predicate<MagicSpellParent>>()))
		      .Returns<Predicate<MagicSpellParent>>(predicate => new[] { parent }.Where(x => predicate(x)));
		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "dispelmagic"),
			new XElement("EffectKey", new XCData("any"))), CreateSpellMock(30).Object);

		effect.GetOrApplyEffect(caster.Object, target.Object, OpposedOutcomeDegree.None,
			SpellPower.Insignificant, CreateParent().Object, []);

		target.Verify(x => x.RemoveEffect(parent, true), Times.Once);
	}

	[TestMethod]
	public void DispelMagicEffect_ContestBlocksWeakerDispel()
	{
		var caster = CreateCharacter(1);
		var target = CreatePerceivable();
		var existingSpell = CreateSpellMock(20);
		var parent = new MagicSpellParent(target.Object, existingSpell.Object, caster.Object,
			SpellPower.RecklesslyPowerful, OpposedOutcomeDegree.Total);
		target.Setup(x => x.EffectsOfType<MagicSpellParent>(It.IsAny<Predicate<MagicSpellParent>>()))
		      .Returns<Predicate<MagicSpellParent>>(predicate => new[] { parent }.Where(x => predicate(x)));
		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "dispelmagic"),
			new XElement("Contest", true),
			new XElement("ContestBonus", 0),
			new XElement("EffectKey", new XCData("any"))), CreateSpellMock(30).Object);

		effect.GetOrApplyEffect(caster.Object, target.Object, OpposedOutcomeDegree.None,
			SpellPower.Insignificant, CreateParent().Object, []);

		target.Verify(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>()), Times.Never);
		target.Verify(x => x.RemoveDuration(It.IsAny<IEffect>(), It.IsAny<TimeSpan>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void DispelMagicEffect_ContestTieRemovesSpell()
	{
		var caster = CreateCharacter(1);
		var target = CreatePerceivable();
		var existingSpell = CreateSpellMock(20);
		var parent = new MagicSpellParent(target.Object, existingSpell.Object, caster.Object,
			SpellPower.Standard, OpposedOutcomeDegree.Minor);
		target.Setup(x => x.EffectsOfType<MagicSpellParent>(It.IsAny<Predicate<MagicSpellParent>>()))
		      .Returns<Predicate<MagicSpellParent>>(predicate => new[] { parent }.Where(x => predicate(x)));
		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "dispelmagic"),
			new XElement("Contest", true),
			new XElement("ContestBonus", 0),
			new XElement("EffectKey", new XCData("any"))), CreateSpellMock(30).Object);

		effect.GetOrApplyEffect(caster.Object, target.Object, OpposedOutcomeDegree.Minor,
			SpellPower.Standard, CreateParent().Object, []);

		target.Verify(x => x.RemoveEffect(parent, true), Times.Once);
	}

	private static Mock<ICharacter> CreateCharacter(long id, IFuturemud? gameworld = null)
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(id);
		character.SetupGet(x => x.Name).Returns($"Character {id}");
		character.SetupGet(x => x.FrameworkItemType).Returns("Character");
		character.SetupGet(x => x.Gameworld).Returns(gameworld ?? CreateGameworld().Object);
		return character;
	}

	private static Mock<IPerceivable> CreatePerceivable()
	{
		var perceivable = new Mock<IPerceivable>();
		perceivable.SetupGet(x => x.Id).Returns(100);
		perceivable.SetupGet(x => x.Name).Returns("Target");
		perceivable.SetupGet(x => x.FrameworkItemType).Returns("Perceivable");
		perceivable.SetupGet(x => x.Gameworld).Returns(CreateGameworld().Object);
		return perceivable;
	}

	private static Mock<IMagicSpellEffectParent> CreateParent()
	{
		var parent = new Mock<IMagicSpellEffectParent>();
		parent.SetupGet(x => x.Spell).Returns(CreateSpellMock().Object);
		return parent;
	}

	private static Mock<IFuturemud> CreateGameworld()
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.FutureProgs).Returns(CreateCollectionMock<IFutureProg>().Object);
		gameworld.SetupGet(x => x.MagicSchools).Returns(CreateCollectionMock(CreateSchool().Object).Object);
		return gameworld;
	}

	private static Mock<IFuturemud> CreateGameworldWithUnits()
	{
		var gameworld = CreateGameworld();
		var unitManager = new Mock<IUnitManager>();
		unitManager.SetupGet(x => x.BaseWeightToKilograms).Returns(1.0);
		unitManager.Setup(x => x.DescribeExact(It.IsAny<double>(), UnitType.Mass, It.IsAny<IPerceiver>()))
		           .Returns("localized mass");
		gameworld.SetupGet(x => x.UnitManager).Returns(unitManager.Object);
		return gameworld;
	}

	private static Mock<IMagicSchool> CreateSchool(long id = 1)
	{
		var school = new Mock<IMagicSchool>();
		school.SetupGet(x => x.Id).Returns(id);
		school.SetupGet(x => x.Name).Returns($"School {id}");
		school.SetupGet(x => x.PowerListColour).Returns(Telnet.Green);
		school.Setup(x => x.IsChildSchool(It.IsAny<IMagicSchool>())).Returns(false);
		return school;
	}

	private static Mock<IMagicSpell> CreateSpellMock(long id = 1, IFuturemud? gameworld = null)
	{
		var spell = new Mock<IMagicSpell>();
		spell.SetupGet(x => x.Id).Returns(id);
		spell.SetupGet(x => x.Name).Returns($"Spell {id}");
		spell.SetupGet(x => x.Gameworld).Returns(gameworld ?? CreateGameworld().Object);
		spell.SetupGet(x => x.School).Returns(CreateSchool().Object);
		spell.SetupProperty(x => x.Changed);
		return spell;
	}

	private static Mock<IDrug> CreateDrug(long id, string name)
	{
		var drug = new Mock<IDrug>();
		drug.SetupGet(x => x.Id).Returns(id);
		drug.SetupGet(x => x.Name).Returns(name);
		drug.SetupGet(x => x.FrameworkItemType).Returns("Drug");
		return drug;
	}

	private static Mock<IUneditableAll<T>> CreateCollectionMock<T>(params T[] items) where T : class, IFrameworkItem
	{
		var byId = items.ToDictionary(x => x.Id, x => x);
		var collection = new Mock<IUneditableAll<T>>();
		collection.SetupGet(x => x.Count).Returns(items.Length);
		collection.Setup(x => x.Get(It.IsAny<long>())).Returns<long>(id => byId.GetValueOrDefault(id));
		collection.Setup(x => x.GetByName(It.IsAny<string>())).Returns<string>(name =>
			items.FirstOrDefault(x => x.Name.EqualTo(name)));
		collection.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>())).Returns<string, bool>((text, _) =>
			long.TryParse(text, out var id) ? byId.GetValueOrDefault(id) : items.FirstOrDefault(x => x.Name.EqualTo(text)));
		collection.Setup(x => x.GetEnumerator()).Returns(() => ((IEnumerable<T>)items).GetEnumerator());
		collection.As<IEnumerable<T>>().Setup(x => x.GetEnumerator()).Returns(() => ((IEnumerable<T>)items).GetEnumerator());
		return collection;
	}
}
