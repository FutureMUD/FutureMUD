#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
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
public class MagicPhase3Tests
{
	private static readonly string[] Phase3EffectTypes =
	[
		"magictag",
		"removemagictag",
		"itemdamage",
		"destroyitem",
		"itemenchant",
		"corpsemark",
		"corpsepreserve",
		"corpseconsume",
		"corpsespawn",
		"portal",
		"forcecommand",
		"subjectivedesc",
		"subjectivesdesc"
	];

	[TestInitialize]
	public void TestInitialize()
	{
		SpellTriggerFactory.SetupFactory();
		SpellEffectFactory.SetupFactory();
	}

	[TestMethod]
	public void SpellEffectFactory_RegistersPhase3EffectTypes()
	{
		var spell = CreateSpellMock();
		foreach (var type in Phase3EffectTypes)
		{
			var (effect, error) = SpellEffectFactory.LoadEffectFromBuilderInput(type, new StringStack(string.Empty),
				spell.Object);

			Assert.AreEqual(string.Empty, error, $"Unexpected builder error for {type}.");
			Assert.IsNotNull(effect, $"No builder effect was created for {type}.");
			Assert.AreEqual(type, effect!.SaveToXml().Attribute("type")?.Value);
			Assert.IsTrue(SpellEffectFactory.BuilderInfoForType(type).MatchingTriggers.Any());
		}
	}

	[TestMethod]
	public void PlaneAndFormRecipeEffects_RemainBuilderVisibleForEngineV1()
	{
		Assert.IsTrue(PlanarStateSpellEffect.IsCompatibleWithTrigger("character"));
		Assert.IsTrue(PlanarStateSpellEffect.IsCompatibleWithTrigger("item"));
		Assert.IsTrue(PlanarStateSpellEffect.IsCompatibleWithTrigger("perceivables"));
		Assert.IsTrue(TransformFormEffect.IsCompatibleWithTrigger("character"));

		CollectionAssert.Contains(SpellEffectFactory.BuilderInfoForType("planarstate").MatchingTriggers, "character");
		CollectionAssert.Contains(SpellEffectFactory.BuilderInfoForType("planeshift").MatchingTriggers, "item");
		CollectionAssert.Contains(SpellEffectFactory.BuilderInfoForType("removeplanarstate").MatchingTriggers, "character");
		Assert.IsTrue(SpellEffectFactory.BuilderInfoForType("transformform").MatchingTriggers.Any());
	}

	[TestMethod]
	public void SpellEffectFactory_LoadEffect_RoundTripsPhase3Xml()
	{
		var spell = CreateSpellMock();
		foreach (var definition in Phase3Definitions())
		{
			var effect = SpellEffectFactory.LoadEffect(definition, spell.Object);
			var saved = effect.SaveToXml();

			Assert.AreEqual(definition.Attribute("type")?.Value, saved.Attribute("type")?.Value);
		}
	}

	[TestMethod]
	public void SpellMagicTagEffect_ExposesCasterSpellAndMetadata()
	{
		var caster = new Mock<ICharacter>();
		var spell = CreateSpellMock();
		var parent = new Mock<IMagicSpellEffectParent>();
		parent.SetupGet(x => x.Caster).Returns(caster.Object);
		parent.SetupGet(x => x.Spell).Returns(spell.Object);
		var owner = CreatePerceivable(CreateGameworld().Object);

		var effect = new SpellMagicTagEffect(owner.Object, parent.Object, "anchor", "north");

		Assert.AreEqual("anchor", effect.Tag);
		Assert.AreEqual("north", effect.Value);
		Assert.AreSame(caster.Object, effect.Caster);
		Assert.AreSame(spell.Object, effect.Spell);
	}

	[TestMethod]
	public void ItemEnchantmentEffect_ImplementsCombatAndArmourHooks()
	{
		var owner = CreatePerceivable(CreateGameworld().Object);
		var parent = CreateParent();
		var effect = new SpellItemEnchantmentEffect(owner.Object, parent.Object, "(glowing)",
			"It glows.", Telnet.BoldMagenta, 2.5, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0);

		Assert.AreEqual(1.0, effect.AttackCheckBonus);
		Assert.AreEqual(2.0, effect.QualityBonus);
		Assert.AreEqual(2.5, effect.ProvidedLux);

		var wounds = new List<IWound>();
		var reduced = effect.PassiveSufferDamage(new Damage
		{
			DamageAmount = 10.0,
			PainAmount = 9.0,
			StunAmount = 8.0,
			DamageType = DamageType.Crushing,
			PenetrationOutcome = Outcome.NotTested
		}, ref wounds);

		Assert.AreEqual(4.0, reduced.DamageAmount);
		Assert.AreEqual(3.0, reduced.PainAmount);
		Assert.AreEqual(2.0, reduced.StunAmount);
	}

	[TestMethod]
	public void TransientExitManager_RegistersAndRemovesPortalExitWithoutDatabaseId()
	{
		var gameworld = CreateGameworld();
		var manager = new ExitManager(gameworld.Object);
		gameworld.SetupGet(x => x.ExitManager).Returns(manager);
		var origin = CreateCell(1, "Origin", gameworld.Object);
		var destination = CreateCell(2, "Destination", gameworld.Object);

		var exit = new TransientExit(gameworld.Object, origin.Object, destination.Object, "enter", "portal", "portal",
			"a bright portal", "a bright portal", "through", "through", 1.0);

		manager.RegisterTransientExit(exit);

		Assert.AreSame(exit, manager.GetExitByID(exit.Id));
		Assert.AreSame(destination.Object, exit.CellExitFor(origin.Object)!.Destination);

		manager.UnregisterTransientExit(exit);

		Assert.IsNull(manager.GetExitByID(exit.Id));
	}

	[TestMethod]
	public void ForceCommandEffect_RespectsIgnoreForceEffect()
	{
		var spell = CreateSpellMock();
		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "forcecommand"),
			new XElement("Command", new XCData("look"))), spell.Object);
		var caster = new Mock<ICharacter>();
		var target = new Mock<ICharacter>();
		target.Setup(x => x.AffectedBy<IIgnoreForceEffect>()).Returns(true);

		effect.GetOrApplyEffect(caster.Object, target.Object, OpposedOutcomeDegree.None, SpellPower.Insignificant,
			CreateParent().Object, []);

		target.Verify(x => x.ExecuteCommand(It.IsAny<string>()), Times.Never);
	}

	private static IEnumerable<XElement> Phase3Definitions()
	{
		yield return new XElement("Effect", new XAttribute("type", "magictag"),
			new XElement("Tag", new XCData("anchor")), new XElement("Value", new XCData("north")),
			new XElement("ReplaceExisting", true));
		yield return new XElement("Effect", new XAttribute("type", "removemagictag"),
			new XElement("Tag", new XCData("anchor")), new XElement("Value", new XCData("north")),
			new XElement("MatchValue", true));
		yield return new XElement("Effect", new XAttribute("type", "itemdamage"),
			new XElement("DamageFormula", new XCData("power * 7")), new XElement("PainFormula", new XCData("1")),
			new XElement("StunFormula", new XCData("2")), new XElement("DamageType", (int)DamageType.Crushing));
		yield return new XElement("Effect", new XAttribute("type", "destroyitem"),
			new XElement("RespectPurgeWarnings", true));
		yield return new XElement("Effect", new XAttribute("type", "itemenchant"),
			new XElement("SDescAddendum", new XCData("(enchanted)")),
			new XElement("DescAddendum", new XCData("It shines.")), new XElement("Colour", "bold magenta"),
			new XElement("GlowLux", 1.0), new XElement("AttackCheckBonus", 1.0),
			new XElement("QualityBonus", 1.0), new XElement("DamageBonus", 1.0), new XElement("PainBonus", 1.0),
			new XElement("StunBonus", 1.0), new XElement("ArmourDamageReduction", 1.0),
			new XElement("ApplicabilityProg", 0));
		yield return new XElement("Effect", new XAttribute("type", "corpsemark"),
			new XElement("Tag", new XCData("corpsemark")), new XElement("Value", new XCData("")),
			new XElement("ReplaceExisting", true));
		yield return new XElement("Effect", new XAttribute("type", "corpsepreserve"));
		yield return new XElement("Effect", new XAttribute("type", "corpseconsume"));
		yield return new XElement("Effect", new XAttribute("type", "corpsespawn"),
			new XElement("NPCPrototypeId", 0), new XElement("ItemPrototypeId", 0), new XElement("Quantity", 1),
			new XElement("ConsumeCorpse", false));
		yield return new XElement("Effect", new XAttribute("type", "portal"),
			new XElement("Verb", new XCData("enter")), new XElement("OutboundKeyword", new XCData("portal")),
			new XElement("InboundKeyword", new XCData("portal")),
			new XElement("OutboundTarget", new XCData("a portal")),
			new XElement("InboundTarget", new XCData("a portal")),
			new XElement("OutboundDescription", new XCData("through")),
			new XElement("InboundDescription", new XCData("through")),
			new XElement("TimeMultiplier", 1.0), new XElement("AllowCrossZone", false),
			new XElement("AnchorTag", new XCData("anchor")), new XElement("AnchorValue", new XCData("north")),
			new XElement("DestinationProg", 0));
		yield return new XElement("Effect", new XAttribute("type", "forcecommand"),
			new XElement("Command", new XCData("look")));
		yield return new XElement("Effect", new XAttribute("type", "subjectivedesc"),
			new XElement("Description", new XCData("A changed description.")), new XElement("FixedViewer", true),
			new XElement("ApplicabilityProg", 0));
		yield return new XElement("Effect", new XAttribute("type", "subjectivesdesc"),
			new XElement("Description", new XCData("someone changed")), new XElement("FixedViewer", true),
			new XElement("ApplicabilityProg", 0));
	}

	private static Mock<IMagicSpellEffectParent> CreateParent()
	{
		var parent = new Mock<IMagicSpellEffectParent>();
		parent.SetupGet(x => x.Spell).Returns(CreateSpellMock().Object);
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

	private static Mock<ICell> CreateCell(long id, string name, IFuturemud gameworld)
	{
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(id);
		cell.SetupGet(x => x.Name).Returns(name);
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
		school.SetupGet(x => x.Name).Returns("Magic");
		school.SetupGet(x => x.PowerListColour).Returns(Telnet.Green);
		return school;
	}

	private static Mock<IMagicSpell> CreateSpellMock()
	{
		var spell = new Mock<IMagicSpell>();
		spell.SetupGet(x => x.Gameworld).Returns(CreateGameworld().Object);
		spell.SetupGet(x => x.School).Returns(CreateSchool().Object);
		spell.SetupProperty(x => x.Changed);
		return spell;
	}

	private static Mock<IUneditableAll<T>> CreateCollectionMock<T>(params T[] items) where T : class, IFrameworkItem
	{
		var byId = items.ToDictionary(x => x.Id, x => x);
		var collection = new Mock<IUneditableAll<T>>();
		collection.SetupGet(x => x.Count).Returns(items.Length);
		collection.Setup(x => x.Get(It.IsAny<long>())).Returns<long>(id => byId.GetValueOrDefault(id));
		collection.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>())).Returns((T?)null);
		collection.Setup(x => x.GetEnumerator()).Returns(((IEnumerable<T>)items).GetEnumerator());
		return collection;
	}
}
