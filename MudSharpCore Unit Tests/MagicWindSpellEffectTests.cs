#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.SpellTriggers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicWindSpellEffectTests
{
	private static readonly (string BuilderType, string SavedType)[] WindEffectTypes =
	[
		("levitate", "levitate"),
		("featherfall", "featherfall"),
		("removeinvisibility", "removeinvisibility"),
		("dispelinvisibility", "removeinvisibility"),
		("forcedpathmovement", "forcedpathmovement"),
		("handsofwind", "forcedpathmovement"),
		("transference", "transference")
	];

	[TestInitialize]
	public void TestInitialize()
	{
		SpellTriggerFactory.SetupFactory();
		SpellEffectFactory.SetupFactory();
	}

	[TestMethod]
	public void SpellEffectFactory_RegistersWindEffectTypes()
	{
		var spell = CreateSpellMock();
		foreach (var (builderType, savedType) in WindEffectTypes)
		{
			var (effect, error) = SpellEffectFactory.LoadEffectFromBuilderInput(builderType,
				new StringStack(string.Empty), spell.Object);

			Assert.AreEqual(string.Empty, error, $"Unexpected builder error for {builderType}.");
			Assert.IsNotNull(effect, $"No builder effect was created for {builderType}.");
			Assert.AreEqual(savedType, effect!.SaveToXml().Attribute("type")?.Value);
			Assert.IsTrue(SpellEffectFactory.BuilderInfoForType(builderType).MatchingTriggers.Any());
		}
	}

	[TestMethod]
	public void SpellEffectFactory_LoadEffect_RoundTripsWindXml()
	{
		var spell = CreateSpellMock();
		foreach (var definition in WindDefinitions())
		{
			var effect = SpellEffectFactory.LoadEffect(definition, spell.Object);
			var saved = effect.SaveToXml();

			Assert.AreEqual(definition.Attribute("type")?.Value.EqualTo("dispelinvisibility") == true
					? "removeinvisibility"
					: definition.Attribute("type")?.Value,
				saved.Attribute("type")?.Value);
		}
	}

	[TestMethod]
	public void InvisibilityEffect_SaveAndLegacyGlowFallback_UseInvisibilityType()
	{
		var spell = CreateSpellMock();
		var (built, error) = SpellEffectFactory.LoadEffectFromBuilderInput("invisibility",
			new StringStack(string.Empty), spell.Object);

		Assert.AreEqual(string.Empty, error);
		Assert.AreEqual("invisibility", built!.SaveToXml().Attribute("type")?.Value);

		var legacyGlow = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "glow"),
			new XElement("FilterProg", 0)), spell.Object);

		Assert.AreEqual("invisibility", legacyGlow.SaveToXml().Attribute("type")?.Value);

		var normalGlow = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "glow"),
			new XElement("GlowLuxPerPower", 5.0),
			new XElement("SDescAddendum", new XCData("(glowing)")),
			new XElement("DescAddendum", new XCData("A {0} glow surrounds @.")),
			new XElement("GlowAddendumColour", "bold white")), spell.Object);

		Assert.AreEqual("glow", normalGlow.SaveToXml().Attribute("type")?.Value);
	}

	[TestMethod]
	public void RemoveInvisibilityEffect_GetOrApplyEffect_RemovesOnlySpellInvisibilityEffects()
	{
		var spell = CreateSpellMock();
		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "removeinvisibility")), spell.Object);
		var caster = CreateCharacter(7, "Caster", spell.Object.Gameworld);
		var target = CreatePerceivable(spell.Object.Gameworld);

		effect.GetOrApplyEffect(caster.Object, target.Object, OpposedOutcomeDegree.None,
			SpellPower.Insignificant, CreateParent(spell.Object, caster.Object).Object, []);

		target.Verify(x => x.RemoveAllEffects<SpellInvisibilityEffect>(null, true), Times.Once);
	}

	[TestMethod]
	public void DispelMagicEffect_InvisibilityKey_RemovesMatchingSpellParent()
	{
		var spell = CreateSpellMock();
		var caster = CreateCharacter(7, "Caster", spell.Object.Gameworld);
		var target = CreatePerceivable(spell.Object.Gameworld);
		var parent = new MagicSpellParent(target.Object, spell.Object, caster.Object);
		parent.AddSpellEffect(new SpellInvisibilityEffect(target.Object, parent, null));
		target.Setup(x => x.EffectsOfType<MagicSpellParent>(It.IsAny<Predicate<MagicSpellParent>>()))
		      .Returns<Predicate<MagicSpellParent>>(predicate => predicate(parent) ? [parent] : []);
		var dispel = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "dispelmagic"),
			new XElement("Mode", (int)DispelMagicMode.Remove),
			new XElement("CasterPolicy", (int)DispelCasterPolicy.OwnOnly),
			new XElement("AllowHostile", false),
			new XElement("IncludeSubschools", true),
			new XElement("ShortenSeconds", 60.0),
			new XElement("SpellId", 0L),
			new XElement("SchoolId", 0L),
			new XElement("Tag", new XCData(string.Empty)),
			new XElement("TagValue", new XCData(string.Empty)),
			new XElement("MatchTagValue", false),
			new XElement("EffectKey", new XCData("invisibility"))
		), spell.Object);

		dispel.GetOrApplyEffect(caster.Object, target.Object, OpposedOutcomeDegree.None,
			SpellPower.Insignificant, CreateParent(spell.Object, caster.Object).Object, []);

		target.Verify(x => x.RemoveEffect(parent, true), Times.Once);
	}

	[TestMethod]
	public void WindStatusEffects_ExposeRuntimeInterfacesAndSaveConfiguration()
	{
		var spell = CreateSpellMock();
		var caster = CreateCharacter(7, "Caster", spell.Object.Gameworld);
		var owner = CreatePerceivable(spell.Object.Gameworld);
		var parent = CreateParent(spell.Object, caster.Object).Object;

		var levitation = new SpellLevitationEffect(owner.Object, parent, null, false, RoomLayer.InAir,
			"(levitating)", "@ is suspended in the air.", Telnet.BoldCyan);
		var featherFall = new SpellFeatherFallEffect(owner.Object, parent, null, 0.25, 0.1,
			"(falling lightly)", "@ drifts through the air.", Telnet.BoldCyan);

		Assert.IsInstanceOfType(levitation, typeof(IPreventFallingEffect));
		Assert.IsInstanceOfType(levitation, typeof(ILevitationEffect));
		Assert.IsInstanceOfType(featherFall, typeof(IFallDamageMitigationEffect));
		Assert.AreEqual(0.25, featherFall.FallDistanceMultiplier);
		Assert.AreEqual(0.1, featherFall.FallDamageMultiplier);
		Assert.AreEqual("SpellLevitation", levitation.SaveToXml(new Dictionary<IEffect, TimeSpan>())
		                                             .Element("Type")?.Value);
		Assert.AreEqual("SpellFeatherFall", featherFall.SaveToXml(new Dictionary<IEffect, TimeSpan>())
		                                               .Element("Type")?.Value);
	}

	[TestMethod]
	public void TransferenceEffect_SameLocationAndLayer_DoesNotTeleport()
	{
		var spell = CreateSpellMock();
		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "transference"),
			new XElement("IncludeFollowers", false),
			new XElement("SwapLayers", true)), spell.Object);
		var location = CreateCell(spell.Object.Gameworld);
		var caster = CreateCharacter(7, "Caster", spell.Object.Gameworld);
		var target = CreateCharacter(8, "Target", spell.Object.Gameworld);
		caster.SetupGet(x => x.Location).Returns(location.Object);
		caster.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		target.SetupGet(x => x.Location).Returns(location.Object);
		target.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);

		effect.GetOrApplyEffect(caster.Object, target.Object, OpposedOutcomeDegree.None,
			SpellPower.Insignificant, CreateParent(spell.Object, caster.Object).Object, []);

		caster.Verify(x => x.Teleport(It.IsAny<ICell>(), It.IsAny<RoomLayer>(), It.IsAny<bool>(),
			It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
			It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
		target.Verify(x => x.Teleport(It.IsAny<ICell>(), It.IsAny<RoomLayer>(), It.IsAny<bool>(),
			It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
			It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
	}

	private static IEnumerable<XElement> WindDefinitions()
	{
		yield return new XElement("Effect", new XAttribute("type", "levitate"),
			new XElement("PreserveLayer", false),
			new XElement("TargetLayer", (int)RoomLayer.InAir),
			new XElement("SDescAddendum", new XCData("(levitating)")),
			new XElement("DescAddendum", new XCData("@ is suspended in the air.")),
			new XElement("AddendumColour", "bold cyan"));
		yield return new XElement("Effect", new XAttribute("type", "featherfall"),
			new XElement("FallDistanceMultiplier", 0.25),
			new XElement("FallDamageMultiplier", 0.1),
			new XElement("SDescAddendum", new XCData("(falling lightly)")),
			new XElement("DescAddendum", new XCData("@ drifts through the air.")),
			new XElement("AddendumColour", "bold cyan"));
		yield return new XElement("Effect", new XAttribute("type", "removeinvisibility"));
		yield return new XElement("Effect", new XAttribute("type", "dispelinvisibility"));
		yield return new XElement("Effect", new XAttribute("type", "forcedpathmovement"),
			new XElement("Steps", 3),
			new XElement("AllowFallExits", false));
		yield return new XElement("Effect", new XAttribute("type", "transference"),
			new XElement("IncludeFollowers", false),
			new XElement("SwapLayers", true));
	}

	private static Mock<IMagicSpellEffectParent> CreateParent(IMagicSpell spell, ICharacter caster)
	{
		var parent = new Mock<IMagicSpellEffectParent>();
		parent.SetupGet(x => x.Spell).Returns(spell);
		parent.SetupGet(x => x.Caster).Returns(caster);
		return parent;
	}

	private static Mock<IPerceivable> CreatePerceivable(IFuturemud gameworld)
	{
		var perceivable = new Mock<IPerceivable>();
		perceivable.SetupGet(x => x.Gameworld).Returns(gameworld);
		perceivable.SetupProperty(x => x.EffectsChanged);
		perceivable.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>()))
		          .Returns([]);
		return perceivable;
	}

	private static Mock<ICharacter> CreateCharacter(long id, string name, IFuturemud gameworld)
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(id);
		character.SetupGet(x => x.Name).Returns(name);
		character.SetupGet(x => x.Gameworld).Returns(gameworld);
		character.SetupProperty(x => x.EffectsChanged);
		character.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>()))
		         .Returns([]);
		return character;
	}

	private static Mock<ICell> CreateCell(IFuturemud gameworld)
	{
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Gameworld).Returns(gameworld);
		return cell;
	}

	private static Mock<IFuturemud> CreateGameworld()
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.FutureProgs).Returns(CreateCollectionMock<IFutureProg>().Object);
		gameworld.SetupGet(x => x.MagicSchools).Returns(CreateCollectionMock<IMagicSchool>().Object);
		return gameworld;
	}

	private static Mock<IMagicSchool> CreateSchool()
	{
		var school = new Mock<IMagicSchool>();
		school.SetupGet(x => x.Id).Returns(1);
		school.SetupGet(x => x.Name).Returns("Wind");
		school.SetupGet(x => x.PowerListColour).Returns(Telnet.BoldCyan);
		return school;
	}

	private static Mock<IMagicSpell> CreateSpellMock()
	{
		var spell = new Mock<IMagicSpell>();
		spell.SetupGet(x => x.Gameworld).Returns(CreateGameworld().Object);
		spell.SetupGet(x => x.School).Returns(CreateSchool().Object);
		spell.SetupGet(x => x.Id).Returns(9002);
		spell.SetupGet(x => x.Name).Returns("Wind Test Spell");
		spell.SetupProperty(x => x.Changed);
		return spell;
	}

	private static Mock<IUneditableAll<T>> CreateCollectionMock<T>(params T[] items) where T : class, IFrameworkItem
	{
		var byId = items.ToDictionary(x => x.Id, x => x);
		var collection = new Mock<IUneditableAll<T>>();
		collection.SetupGet(x => x.Count).Returns(items.Length);
		collection.As<IEnumerable<T>>().Setup(x => x.GetEnumerator()).Returns(() => ((IEnumerable<T>)items).GetEnumerator());
		collection.Setup(x => x.Get(It.IsAny<long>())).Returns<long>(id => byId.GetValueOrDefault(id));
		collection.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>())).Returns((T?)null);
		return collection;
	}
}
