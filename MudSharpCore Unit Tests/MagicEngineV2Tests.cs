#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.SpellTriggers;
using MudSharp.Models;
using MudSharp.Planes;
using MudSharp.RPG.Checks;
using MudSharp.Body.Traits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicEngineV2Tests
{
	[TestInitialize]
	public void TestInitialize()
	{
		SpellTriggerFactory.SetupFactory();
		SpellEffectFactory.SetupFactory();
	}

	[TestMethod]
	public void SpellEffectFactory_RegistersEngineV2DispelMagic()
	{
		var spell = CreateSpellMock();

		var (effect, error) = SpellEffectFactory.LoadEffectFromBuilderInput("dispelmagic",
			new StringStack(string.Empty), spell.Object);

		Assert.AreEqual(string.Empty, error);
		Assert.IsNotNull(effect);
		Assert.AreEqual("dispelmagic", effect!.SaveToXml().Attribute("type")?.Value);
		Assert.IsTrue(SpellEffectFactory.BuilderInfoForType("dispelmagic").MatchingTriggers.Any());
	}

	[TestMethod]
	public void DispelMagicEffect_ShortenModeShortensOwnedMatchingEffects()
	{
		var caster = CreateCharacter(10);
		var target = CreatePerceivable(CreateGameworld().Object);
		var existingSpell = CreateSpellMock(20);
		var parent = new MagicSpellParent(target.Object, existingSpell.Object, caster.Object);
		var dispelSpell = CreateSpellMock(30);
		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "dispelmagic"),
			new XElement("Mode", (int)DispelMagicMode.Shorten),
			new XElement("CasterPolicy", (int)DispelCasterPolicy.OwnOnly),
			new XElement("AllowHostile", false),
			new XElement("ShortenSeconds", 45.0),
			new XElement("EffectKey", new XCData("any"))), dispelSpell.Object);

		target.Setup(x => x.EffectsOfType<MagicSpellParent>(It.IsAny<Predicate<MagicSpellParent>>()))
		      .Returns<Predicate<MagicSpellParent>>(predicate => new[] { parent }.Where(x => predicate(x)));

		effect.GetOrApplyEffect(caster.Object, target.Object, OpposedOutcomeDegree.None, SpellPower.Insignificant,
			CreateParent(dispelSpell.Object, caster.Object).Object, []);

		target.Verify(x => x.RemoveDuration(parent, TimeSpan.FromSeconds(45.0), true), Times.Once);
		target.Verify(x => x.RemoveEffect(parent, It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void DispelMagicEffect_DefaultCasterPolicyDoesNotRemoveHostileEffects()
	{
		var caster = CreateCharacter(10);
		var otherCaster = CreateCharacter(11);
		var target = CreatePerceivable(CreateGameworld().Object);
		var existingSpell = CreateSpellMock(20);
		var parent = new MagicSpellParent(target.Object, existingSpell.Object, otherCaster.Object);
		var dispelSpell = CreateSpellMock(30);
		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "dispelmagic"),
			new XElement("Mode", (int)DispelMagicMode.Remove),
			new XElement("CasterPolicy", (int)DispelCasterPolicy.OwnOnly),
			new XElement("AllowHostile", false),
			new XElement("EffectKey", new XCData("any"))), dispelSpell.Object);

		target.Setup(x => x.EffectsOfType<MagicSpellParent>(It.IsAny<Predicate<MagicSpellParent>>()))
		      .Returns<Predicate<MagicSpellParent>>(predicate => new[] { parent }.Where(x => predicate(x)));

		effect.GetOrApplyEffect(caster.Object, target.Object, OpposedOutcomeDegree.None, SpellPower.Insignificant,
			CreateParent(dispelSpell.Object, caster.Object).Object, []);

		target.Verify(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>()), Times.Never);
		target.Verify(x => x.RemoveDuration(It.IsAny<IEffect>(), It.IsAny<TimeSpan>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void TransientExitManager_ExposesPortalMetadataForInspection()
	{
		var gameworld = CreateGameworld();
		var manager = new ExitManager(gameworld.Object);
		var caster = CreateCharacter(10);
		var spell = CreateSpellMock(20);
		var sourceEffect = new Mock<IEffect>();
		var origin = CreateCell(1, "Origin", gameworld.Object);
		var destination = CreateCell(2, "Destination", gameworld.Object);

		var exit = new TransientExit(gameworld.Object, origin.Object, destination.Object, "enter", "gate", "gate",
			"a bright gate", "a bright gate", "through", "through", 1.0, caster.Object, spell.Object,
			sourceEffect.Object);

		manager.RegisterTransientExit(exit);

		var portal = manager.TransientExits.OfType<IMagicPortalExit>().Single();
		Assert.AreSame(exit, portal.Exit);
		Assert.AreSame(origin.Object, portal.Source);
		Assert.AreSame(destination.Object, portal.Destination);
		Assert.AreSame(caster.Object, portal.Caster);
		Assert.AreSame(spell.Object, portal.Spell);
		Assert.AreSame(sourceEffect.Object, portal.SourceEffect);
		Assert.AreEqual("enter", portal.Verb);
	}

	[TestMethod]
	public void PortalSpellEffect_ResolvesCasterOwnedItemAnchors()
	{
		var plane = CreatePlane();
		var zone = new Mock<IZone>();
		var caster = CreateCharacter(10);
		var source = CreateCell(1, "Source", null, zone.Object);
		var destination = CreateCell(2, "Destination", null, zone.Object);
		var anchorItem = new Mock<IGameItem>();
		var tag = new Mock<IMagicTagEffect>();
		var gameworld = CreateGameworld([source.Object, destination.Object], [anchorItem.Object], plane.Object);
		var spell = CreateSpellMock(20, gameworld.Object);
		source.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		destination.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		caster.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		caster.SetupGet(x => x.Location).Returns(source.Object);
		tag.SetupGet(x => x.Caster).Returns(caster.Object);
		tag.SetupGet(x => x.Tag).Returns("travel-gate");
		tag.SetupGet(x => x.Value).Returns("south");
		anchorItem.SetupGet(x => x.Location).Returns(destination.Object);
		anchorItem.Setup(x => x.EffectsOfType<IMagicTagEffect>(It.IsAny<Predicate<IMagicTagEffect>>()))
		          .Returns<Predicate<IMagicTagEffect>>(predicate => new[] { tag.Object }.Where(x => predicate(x)));

		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "portal"),
			new XElement("Verb", new XCData("enter")),
			new XElement("OutboundKeyword", new XCData("gate")),
			new XElement("InboundKeyword", new XCData("gate")),
			new XElement("OutboundTarget", new XCData("a gate")),
			new XElement("InboundTarget", new XCData("a gate")),
			new XElement("OutboundDescription", new XCData("through")),
			new XElement("InboundDescription", new XCData("through")),
			new XElement("TimeMultiplier", 1.0),
			new XElement("AllowCrossZone", false),
			new XElement("AnchorTag", new XCData("travel-gate")),
			new XElement("AnchorValue", new XCData("south")),
			new XElement("DestinationProg", 0L)), spell.Object);

		var portal = (SpellPortalEffect?)effect.GetOrApplyEffect(caster.Object, null, OpposedOutcomeDegree.None,
			SpellPower.Insignificant, CreateParent(spell.Object, caster.Object).Object, []);

		Assert.IsNotNull(portal);
		Assert.AreEqual(source.Object.Id, portal!.SourceCellId);
		Assert.AreEqual(destination.Object.Id, portal.DestinationCellId);
	}

	[TestMethod]
	public void ItemEnchantEffect_RoundTripsEngineV2HookFields()
	{
		var spell = CreateSpellMock();
		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "itemenchant"),
			new XElement("SDescAddendum", new XCData("(storm-charged)")),
			new XElement("DescAddendum", new XCData("It crackles with stored force.")),
			new XElement("Colour", "bold magenta"),
			new XElement("GlowLux", 2.0),
			new XElement("AttackCheckBonus", 1.0),
			new XElement("QualityBonus", 2.0),
			new XElement("DamageBonus", 3.0),
			new XElement("PainBonus", 4.0),
			new XElement("StunBonus", 5.0),
			new XElement("ArmourDamageReduction", 6.0),
			new XElement("ProjectileQualityBonus", 7.0),
			new XElement("ProjectileDamageBonus", 8.0),
			new XElement("ProjectilePainBonus", 9.0),
			new XElement("ProjectileStunBonus", 10.0),
			new XElement("ToolFitnessBonus", 11.0),
			new XElement("ToolSpeedMultiplier", 0.75),
			new XElement("ToolUsageMultiplier", 0.5),
			new XElement("PowerProductionMultiplier", 1.25),
			new XElement("PowerConsumptionMultiplier", 0.8),
			new XElement("FuelUseMultiplier", 0.6),
			new XElement("ItemEventType", -1),
			new XElement("ItemEventProg", 0L),
			new XElement("ApplicabilityProg", 0L)), spell.Object);

		var saved = effect.SaveToXml();

		Assert.AreEqual("7", saved.Element("ProjectileQualityBonus")!.Value);
		Assert.AreEqual("8", saved.Element("ProjectileDamageBonus")!.Value);
		Assert.AreEqual("11", saved.Element("ToolFitnessBonus")!.Value);
		Assert.AreEqual("0.75", saved.Element("ToolSpeedMultiplier")!.Value);
		Assert.AreEqual("1.25", saved.Element("PowerProductionMultiplier")!.Value);
		Assert.AreEqual("0.6", saved.Element("FuelUseMultiplier")!.Value);
	}

	[TestMethod]
	public void MindConcealPower_LoadsAndConcealsMindContactIdentity()
	{
		var school = CreateSchool(1);
		var childSchool = CreateSchool(2);
		childSchool.Setup(x => x.IsChildSchool(school.Object)).Returns(true);
		var trait = CreateTrait();
		var prog = CreateProg(0, true);
		var gameworld = CreateGameworld(magicSchools: [school.Object, childSchool.Object], traits: [trait.Object],
			progs: [prog.Object]);
		var power = (MindConcealPower)MagicPowerFactory.LoadPower(CreateMindConcealModel(), gameworld.Object);
		var owner = CreateCharacter(10, gameworld.Object);
		var observer = CreateCharacter(11, gameworld.Object);
		var effect = new MindConcealmentEffect(owner.Object, power);

		Assert.IsTrue(effect.ConcealsIdentityFrom(owner.Object, observer.Object, school.Object));
		Assert.IsTrue(effect.ConcealsIdentityFrom(owner.Object, observer.Object, childSchool.Object));
		Assert.AreEqual("a veiled mind", effect.UnknownIdentityDescription);
		Assert.AreEqual(2, effect.AuditDifficultyStages);
	}

	[TestMethod]
	public void PlaneAndFormRecipes_RemainBuilderLoadableForEngineV2()
	{
		var plane = CreatePlane();
		var race = CreateRace();
		var gameworld = CreateGameworld(planes: [plane.Object], races: [race.Object]);
		var spell = CreateSpellMock(20, gameworld.Object);
		var etherealDefinition = PlanarPresenceDefinition.NonCorporeal(plane.Object).SaveToXml();
		var astralWalkingDefinition = PlanarPresenceDefinition.Manifested(plane.Object).SaveToXml();

		var recipes = new[]
		{
			new XElement("Effect", new XAttribute("type", "planarstate"), etherealDefinition),
			new XElement("Effect", new XAttribute("type", "removeplanarstate")),
			new XElement("Effect", new XAttribute("type", "planeshift"), etherealDefinition),
			new XElement("Effect", new XAttribute("type", "planarstate"), astralWalkingDefinition),
			new XElement("Effect", new XAttribute("type", "transformform"),
				new XElement("FormKey", "polymorph-wolf"),
				new XElement("Race", race.Object.Id),
				new XElement("Ethnicity", 0L),
				new XElement("Gender", -1),
				new XElement("Alias", "wolf form"),
				new XElement("SortOrder", string.Empty),
				new XElement("TraumaMode", (int)BodySwitchTraumaMode.Automatic),
				new XElement("TransformationEcho", new XAttribute("mode", "none"), string.Empty),
				new XElement("AllowVoluntarySwitch", false),
				new XElement("CanVoluntarilySwitchProg", 0L),
				new XElement("WhyCannotVoluntarilySwitchProg", 0L),
				new XElement("CanSeeFormProg", 0L),
				new XElement("ShortDescriptionPattern", 0L),
				new XElement("FullDescriptionPattern", 0L),
				new XElement("PriorityBand", (int)ForcedTransformationPriorityBand.SpellOrPower),
				new XElement("PriorityOffset", 0))
		};

		foreach (var recipe in recipes)
		{
			var effect = SpellEffectFactory.LoadEffect(recipe, spell.Object);

			Assert.AreEqual(recipe.Attribute("type")?.Value, effect.SaveToXml().Attribute("type")?.Value);
		}
	}

	private static MagicPower CreateMindConcealModel()
	{
		return new MagicPower
		{
			Id = 99,
			Name = "Veil Mind",
			Blurb = "Conceal mental identity",
			ShowHelp = "Conceal mental identity.",
			MagicSchoolId = 1,
			PowerModel = "mindconceal",
			Definition = new XElement("Definition",
				new XElement("CanInvokePowerProg", 0L),
				new XElement("WhyCantInvokePowerProg", 0L),
				new XElement("BeginVerb", "mindconceal"),
				new XElement("EndVerb", "endmindconceal"),
				new XElement("SkillCheckDifficulty", (int)Difficulty.Easy),
				new XElement("SkillCheckTrait", 1L),
				new XElement("MinimumSuccessThreshold", (int)Outcome.MinorFail),
				new XElement("AppliesToCharacterProg", 0L),
				new XElement("UnknownIdentityDescription", new XCData("a veiled mind")),
				new XElement("AuditDifficultyStages", 2),
				new XElement("IncludeSubschools", true),
				new XElement("EmoteForBegin", new XCData(string.Empty)),
				new XElement("EmoteForBeginSelf", new XCData("You veil your mind.")),
				new XElement("EmoteForEnd", new XCData(string.Empty)),
				new XElement("EmoteForEndSelf", new XCData("You lower your veil.")),
				new XElement("BeginWhenAlreadySustainingError", new XCData("Already concealed.")),
				new XElement("EndWhenNotSustainingError", new XCData("Not concealed.")),
				new XElement("IsPsionic", true),
				new XElement("ConcentrationPointsToSustain", 1.0),
				new XElement("SustainPenalty", 0.0),
				new XElement("DetectableWithDetectMagic", (int)Difficulty.Normal),
				new XElement("SustainResourceCosts")
			).ToString()
		};
	}

	private static Mock<ICharacter> CreateCharacter(long id, IFuturemud? gameworld = null)
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(id);
		character.SetupGet(x => x.Name).Returns($"Character {id}");
		character.SetupGet(x => x.FrameworkItemType).Returns("Character");
		if (gameworld is not null)
		{
			character.SetupGet(x => x.Gameworld).Returns(gameworld);
		}

		return character;
	}

	private static Mock<IPerceivable> CreatePerceivable(IFuturemud gameworld)
	{
		var perceivable = new Mock<IPerceivable>();
		perceivable.SetupGet(x => x.Id).Returns(100);
		perceivable.SetupGet(x => x.Name).Returns("Target");
		perceivable.SetupGet(x => x.FrameworkItemType).Returns("Perceivable");
		perceivable.SetupGet(x => x.Gameworld).Returns(gameworld);
		perceivable.SetupProperty(x => x.EffectsChanged);
		perceivable.Setup(x => x.EffectsOfType<IPlanarOverlayEffect>(It.IsAny<Predicate<IPlanarOverlayEffect>>()))
		           .Returns([]);
		return perceivable;
	}

	private static Mock<ICell> CreateCell(long id, string name, IFuturemud? gameworld = null, IZone? zone = null)
	{
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(id);
		cell.SetupGet(x => x.Name).Returns(name);
		cell.SetupGet(x => x.FrameworkItemType).Returns("Cell");
		if (gameworld is not null)
		{
			cell.SetupGet(x => x.Gameworld).Returns(gameworld);
		}

		if (zone is not null)
		{
			cell.SetupGet(x => x.Zone).Returns(zone);
		}

		cell.Setup(x => x.EffectsOfType<IMagicTagEffect>(It.IsAny<Predicate<IMagicTagEffect>>())).Returns([]);
		cell.Setup(x => x.EffectsOfType<IPlanarOverlayEffect>(It.IsAny<Predicate<IPlanarOverlayEffect>>()))
		    .Returns([]);
		return cell;
	}

	private static Mock<IMagicSpellEffectParent> CreateParent(IMagicSpell? spell = null, ICharacter? caster = null)
	{
		var parent = new Mock<IMagicSpellEffectParent>();
		parent.SetupGet(x => x.Spell).Returns(spell ?? CreateSpellMock().Object);
		if (caster is not null)
		{
			parent.SetupGet(x => x.Caster).Returns(caster);
		}

		return parent;
	}

	private static Mock<IFuturemud> CreateGameworld(IEnumerable<ICell>? cells = null,
		IEnumerable<IGameItem>? items = null,
		IPlane? defaultPlane = null,
		IEnumerable<IMagicSchool>? magicSchools = null,
		IEnumerable<ITraitDefinition>? traits = null,
		IEnumerable<IFutureProg>? progs = null,
		IEnumerable<IPlane>? planes = null,
		IEnumerable<IRace>? races = null)
	{
		var progList = (progs ?? [CreateProg(0, true).Object]).ToArray();
		var planeList = (planes ?? (defaultPlane is null ? [CreatePlane().Object] : [defaultPlane])).ToArray();
		var resolvedDefaultPlane = defaultPlane ?? planeList.First();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.FutureProgs).Returns(CreateCollectionMock(progList).Object);
		gameworld.SetupGet(x => x.MagicSchools).Returns(CreateCollectionMock((magicSchools ?? [CreateSchool().Object]).ToArray()).Object);
		gameworld.SetupGet(x => x.Traits).Returns(CreateCollectionMock((traits ?? [CreateTrait().Object]).ToArray()).Object);
		gameworld.SetupGet(x => x.Cells).Returns(CreateCollectionMock((cells ?? []).ToArray()).Object);
		gameworld.SetupGet(x => x.Items).Returns(CreateCollectionMock((items ?? []).ToArray()).Object);
		gameworld.SetupGet(x => x.Planes).Returns(CreateCollectionMock(planeList).Object);
		gameworld.SetupGet(x => x.DefaultPlane).Returns(resolvedDefaultPlane);
		gameworld.SetupGet(x => x.Races).Returns(CreateCollectionMock((races ?? [CreateRace().Object]).ToArray()).Object);
		gameworld.SetupGet(x => x.Ethnicities).Returns(CreateCollectionMock<IEthnicity>().Object);
		gameworld.SetupGet(x => x.EntityDescriptionPatterns).Returns(CreateCollectionMock<IEntityDescriptionPattern>().Object);
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

	private static Mock<IFutureProg> CreateProg(long id, bool truth)
	{
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.Id).Returns(id);
		prog.SetupGet(x => x.Name).Returns($"Prog {id}");
		prog.SetupGet(x => x.FunctionName).Returns($"Prog{id}");
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(truth);
		prog.Setup(x => x.Execute<bool?>(It.IsAny<object[]>())).Returns(truth);
		prog.Setup(x => x.Execute(It.IsAny<object[]>())).Returns(truth);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		prog.Setup(x => x.MXPClickableFunctionName()).Returns($"Prog{id}");
		return prog;
	}

	private static Mock<ITraitDefinition> CreateTrait()
	{
		var trait = new Mock<ITraitDefinition>();
		trait.SetupGet(x => x.Id).Returns(1);
		trait.SetupGet(x => x.Name).Returns("Psionics");
		return trait;
	}

	private static Mock<IPlane> CreatePlane(long id = 1)
	{
		var plane = new Mock<IPlane>();
		plane.SetupGet(x => x.Id).Returns(id);
		plane.SetupGet(x => x.Name).Returns($"Plane {id}");
		return plane;
	}

	private static Mock<IRace> CreateRace()
	{
		var race = new Mock<IRace>();
		race.SetupGet(x => x.Id).Returns(1);
		race.SetupGet(x => x.Name).Returns("Human");
		return race;
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
		return collection;
	}
}
