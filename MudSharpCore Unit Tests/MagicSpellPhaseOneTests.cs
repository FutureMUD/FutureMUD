#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicSpellPhaseOneTests
{
	private static readonly string[] PhaseOneEffectTypes =
	[
		"silence",
		"removesilence",
		"sleep",
		"removesleep",
		"fear",
		"removefear",
		"paralysis",
		"removeparalysis",
		"flying",
		"removeflying",
		"waterbreathing",
		"removewaterbreathing",
		"poison",
		"removepoison",
		"disease",
		"removedisease",
		"curse",
		"removecurse",
		"detectinvisible",
		"removedetectinvisible",
		"detectethereal",
		"removedetectethereal",
		"detectmagick",
		"removedetectmagick",
		"infravision",
		"removeinfravision",
		"comprehendlanguage",
		"removecomprehendlanguage",
		"magicresourcedelta",
		"spellarmour",
		"roomflag",
		"removeroomflag"
	];

	[TestMethod]
	public void TeleportEffect_IsCompatibleWithTrigger_RoomTargetsOnly()
	{
		Assert.IsTrue(TeleportEffect.IsCompatibleWithTrigger("room"));
		Assert.IsTrue(TeleportEffect.IsCompatibleWithTrigger("rooms"));
		Assert.IsFalse(TeleportEffect.IsCompatibleWithTrigger("character"));
		Assert.IsFalse(TeleportEffect.IsCompatibleWithTrigger("characters"));
	}

	[TestMethod]
	public void SpellEffectFactory_LoadEffectFromBuilderInput_BuildsAllPhaseOneEffectTypes()
	{
		var context = CreateSpellContext();

		foreach (var type in PhaseOneEffectTypes)
		{
			var (trigger, error) =
				SpellEffectFactory.LoadEffectFromBuilderInput(type, new StringStack(string.Empty), context.Spell.Object);

			Assert.IsTrue(string.IsNullOrEmpty(error), $"Expected no builder error for {type}, but got: {error}");
			Assert.IsNotNull(trigger, $"Builder factory returned a null trigger for {type}.");
			Assert.AreEqual(type, trigger.SaveToXml().Attribute("type")?.Value, $"Builder for {type} saved the wrong type.");
			Assert.IsTrue(SpellEffectFactory.BuilderInfoForType(type).MatchingTriggers.Any(),
				$"Expected {type} to advertise at least one compatible trigger.");
		}
	}

	[TestMethod]
	public void SpellEffectFactory_LoadEffect_LoadsAllPhaseOneEffectTypesFromXml()
	{
		var context = CreateSpellContext();

		foreach (var definition in CreatePhaseOneDefinitions(context))
		{
			var effect = SpellEffectFactory.LoadEffect(definition.Value, context.Spell.Object);

			Assert.IsNotNull(effect, $"LoadEffect returned null for {definition.Key}.");
			Assert.AreEqual(definition.Key, effect.SaveToXml().Attribute("type")?.Value,
				$"Loaded effect for {definition.Key} saved back with the wrong type.");
		}
	}

	[TestMethod]
	public void MagicResourceDeltaEffect_LoadFromXml_PreservesConfiguredResourceAndFormula()
	{
		var context = CreateSpellContext();
		var definition = new XElement("Effect",
			new XAttribute("type", "magicresourcedelta"),
			new XElement("Resource", context.Resource.Object.Id),
			new XElement("Formula", new XCData("power*3-outcome"))
		);

		var loaded = SpellEffectFactory.LoadEffect(definition, context.Spell.Object);
		var saved = loaded.SaveToXml();

		Assert.AreEqual("magicresourcedelta", saved.Attribute("type")?.Value);
		Assert.AreEqual(context.Resource.Object.Id.ToString(), saved.Element("Resource")?.Value);
		Assert.AreEqual("power*3-outcome", saved.Element("Formula")?.Value);
	}

	[TestMethod]
	public void RoomFlagEffects_LoadFromXml_PreserveAlarmAndWardConfiguration()
	{
		var context = CreateSpellContext();
		var roomFlagDefinition = new XElement("Effect",
			new XAttribute("type", "roomflag"),
			new XElement("FlagType", (int)RoomFlagKind.Alarm),
			new XElement("DarknessLux", 250.5),
			new XElement("AlarmEcho", new XCData("@ trigger|triggers a warded alarm.")),
			new XElement("AlarmProg", context.AlarmProg.Object.Id),
			new XElement("WardTag", new XCData("obsidian-ward"))
		);
		var removeFlagDefinition = new XElement("Effect",
			new XAttribute("type", "removeroomflag"),
			new XElement("FlagType", (int)RoomFlagKind.WardTag),
			new XElement("WardTag", new XCData("obsidian-ward"))
		);

		var loadedRoomFlag = SpellEffectFactory.LoadEffect(roomFlagDefinition, context.Spell.Object);
		var loadedRemoveRoomFlag = SpellEffectFactory.LoadEffect(removeFlagDefinition, context.Spell.Object);

		var savedRoomFlag = loadedRoomFlag.SaveToXml();
		var savedRemoveRoomFlag = loadedRemoveRoomFlag.SaveToXml();

		Assert.AreEqual(((int)RoomFlagKind.Alarm).ToString(), savedRoomFlag.Element("FlagType")?.Value);
		Assert.AreEqual("250.5", savedRoomFlag.Element("DarknessLux")?.Value);
		Assert.AreEqual("@ trigger|triggers a warded alarm.", savedRoomFlag.Element("AlarmEcho")?.Value);
		Assert.AreEqual(context.AlarmProg.Object.Id.ToString(), savedRoomFlag.Element("AlarmProg")?.Value);
		Assert.AreEqual("obsidian-ward", savedRoomFlag.Element("WardTag")?.Value);

		Assert.AreEqual(((int)RoomFlagKind.WardTag).ToString(), savedRemoveRoomFlag.Element("FlagType")?.Value);
		Assert.AreEqual("obsidian-ward", savedRemoveRoomFlag.Element("WardTag")?.Value);
	}

	[TestMethod]
	public void SpellArmourEffect_LoadFromXml_PreservesConfiguredArmourValues()
	{
		var context = CreateSpellContext();
		var definition = new XElement("Effect",
			new XAttribute("type", "spellarmour"),
			new XElement("ArmourAppliesProg", context.AlarmProg.Object.Id),
			new XElement("Quality", (int)ItemQuality.Excellent),
			new XElement("ArmourType", context.ArmourType.Object.Id),
			new XElement("ArmourMaterial", context.Material.Object.Id),
			new XElement("FullDescriptionAddendum", new XCData("A shell of mirrored force ripples over @.")),
			new XElement("CanBeObscuredByInventory", true),
			new XElement("MaximumDamageAbsorbed", new XCData("power*7")),
			new XElement("BodypartShapes",
				new XElement("Shape", context.Shape.Object.Id)
			)
		);

		var loaded = SpellEffectFactory.LoadEffect(definition, context.Spell.Object);
		var saved = loaded.SaveToXml();

		Assert.AreEqual("spellarmour", saved.Attribute("type")?.Value);
		Assert.AreEqual(context.AlarmProg.Object.Id.ToString(), saved.Element("ArmourAppliesProg")?.Value);
		Assert.AreEqual(((int)ItemQuality.Excellent).ToString(), saved.Element("Quality")?.Value);
		Assert.AreEqual(context.ArmourType.Object.Id.ToString(), saved.Element("ArmourType")?.Value);
		Assert.AreEqual(context.Material.Object.Id.ToString(), saved.Element("ArmourMaterial")?.Value);
		Assert.AreEqual("A shell of mirrored force ripples over @.", saved.Element("FullDescriptionAddendum")?.Value);
		Assert.AreEqual(bool.TrueString.ToLowerInvariant(), saved.Element("CanBeObscuredByInventory")?.Value?.ToLowerInvariant());
		Assert.AreEqual("power*7", saved.Element("MaximumDamageAbsorbed")?.Value);
		Assert.AreEqual(context.Shape.Object.Id.ToString(),
			saved.Element("BodypartShapes")?.Elements("Shape").Single().Value);
	}

	[TestMethod]
	public void EffectHandler_GetPerception_CombinesGrantingAndDenying()
	{
		var perceivable = new Mock<IPerceivable>();
		var handler = new EffectHandler(perceivable.Object);

		var grantingEffect = new Mock<IEffect>();
		grantingEffect.Setup(x => x.Applies()).Returns(true);
		grantingEffect.SetupGet(x => x.PerceptionGranting).Returns(PerceptionTypes.SenseMagical);
		grantingEffect.SetupGet(x => x.PerceptionDenying).Returns(PerceptionTypes.None);
		grantingEffect.SetupGet(x => x.SavingEffect).Returns(false);

		var denyingEffect = new Mock<IEffect>();
		denyingEffect.Setup(x => x.Applies()).Returns(true);
		denyingEffect.SetupGet(x => x.PerceptionGranting).Returns(PerceptionTypes.None);
		denyingEffect.SetupGet(x => x.PerceptionDenying).Returns(PerceptionTypes.VisualLight);
		denyingEffect.SetupGet(x => x.SavingEffect).Returns(false);

		handler.AddEffect(grantingEffect.Object);
		handler.AddEffect(denyingEffect.Object);

		var result = handler.GetPerception(PerceptionTypes.VisualLight);

		Assert.AreEqual(PerceptionTypes.SenseMagical, result);
	}

	[TestMethod]
	public void MagicPerceptionUtilities_DescribeMagicAuras_ShowsDistinctMagicalSpellEffectsOnly()
	{
		var perceiver = new Mock<IPerceiver>();
		perceiver.SetupGet(x => x.NaturalPerceptionTypes).Returns(PerceptionTypes.None);
		perceiver.Setup(x => x.GetPerception(PerceptionTypes.None)).Returns(PerceptionTypes.SenseMagical);

		var magicalSpellEffect = new Mock<IMagicSpellEffect>();
		magicalSpellEffect.Setup(x => x.Applies()).Returns(true);
		magicalSpellEffect.Setup(x => x.Describe(perceiver.Object)).Returns("a shimmering spell aura");

		var duplicateMagicalSpellEffect = new Mock<IMagicSpellEffect>();
		duplicateMagicalSpellEffect.Setup(x => x.Applies()).Returns(true);
		duplicateMagicalSpellEffect.Setup(x => x.Describe(perceiver.Object)).Returns("a shimmering spell aura");

		var mundaneEffect = new Mock<IEffect>();
		mundaneEffect.Setup(x => x.Applies()).Returns(true);
		mundaneEffect.Setup(x => x.Describe(perceiver.Object)).Returns("something mundane");

		var output = MagicPerceptionUtilities.DescribeMagicAuras(perceiver.Object,
		[
			magicalSpellEffect.Object,
			duplicateMagicalSpellEffect.Object,
			mundaneEffect.Object
		], colour: false);

		Assert.IsTrue(output.StartsWith("You sense magical auras:"), "Expected detect-magick output to include its heading.");
		Assert.AreEqual(1, output.Split("a shimmering spell aura", StringSplitOptions.None).Length - 1,
			"Expected duplicate magical aura descriptions to be collapsed.");
		Assert.IsFalse(output.Contains("something mundane"),
			"Expected non-magical effects to be excluded from detect-magick output.");
	}

	private static Dictionary<string, XElement> CreatePhaseOneDefinitions(TestSpellContext context)
	{
		return new Dictionary<string, XElement>
		{
			["silence"] = new("Effect", new XAttribute("type", "silence")),
			["removesilence"] = new("Effect", new XAttribute("type", "removesilence")),
			["sleep"] = new("Effect", new XAttribute("type", "sleep")),
			["removesleep"] = new("Effect", new XAttribute("type", "removesleep")),
			["fear"] = new("Effect", new XAttribute("type", "fear")),
			["removefear"] = new("Effect", new XAttribute("type", "removefear")),
			["paralysis"] = new("Effect", new XAttribute("type", "paralysis")),
			["removeparalysis"] = new("Effect", new XAttribute("type", "removeparalysis")),
			["flying"] = new("Effect", new XAttribute("type", "flying")),
			["removeflying"] = new("Effect", new XAttribute("type", "removeflying")),
			["waterbreathing"] = new("Effect", new XAttribute("type", "waterbreathing")),
			["removewaterbreathing"] = new("Effect", new XAttribute("type", "removewaterbreathing")),
			["poison"] = new("Effect",
				new XAttribute("type", "poison"),
				new XElement("Drug", context.Drug.Object.Id),
				new XElement("Vector", (int)DrugVector.Ingested),
				new XElement("Formula", new XCData("power*outcome"))
			),
			["removepoison"] = new("Effect",
				new XAttribute("type", "removepoison"),
				new XElement("Drug", context.Drug.Object.Id),
				new XElement("Vector", (int)DrugVector.Ingested),
				new XElement("Formula", new XCData("power*outcome"))
			),
			["disease"] = new("Effect",
				new XAttribute("type", "disease"),
				new XElement("InfectionType", (int)InfectionType.Simple),
				new XElement("VirulenceDifficulty", (int)Difficulty.Normal),
				new XElement("IntensityFormula", new XCData("power*outcome*0.001")),
				new XElement("VirulenceFormula", new XCData("1.0"))
			),
			["removedisease"] = new("Effect",
				new XAttribute("type", "removedisease"),
				new XElement("InfectionType", (int)InfectionType.Simple),
				new XElement("VirulenceDifficulty", (int)Difficulty.Normal),
				new XElement("IntensityFormula", new XCData("power*outcome*0.001")),
				new XElement("VirulenceFormula", new XCData("1.0"))
			),
			["curse"] = new("Effect", new XAttribute("type", "curse")),
			["removecurse"] = new("Effect", new XAttribute("type", "removecurse")),
			["detectinvisible"] = new("Effect", new XAttribute("type", "detectinvisible")),
			["removedetectinvisible"] = new("Effect", new XAttribute("type", "removedetectinvisible")),
			["detectethereal"] = new("Effect", new XAttribute("type", "detectethereal")),
			["removedetectethereal"] = new("Effect", new XAttribute("type", "removedetectethereal")),
			["detectmagick"] = new("Effect", new XAttribute("type", "detectmagick")),
			["removedetectmagick"] = new("Effect", new XAttribute("type", "removedetectmagick")),
			["infravision"] = new("Effect", new XAttribute("type", "infravision")),
			["removeinfravision"] = new("Effect", new XAttribute("type", "removeinfravision")),
			["comprehendlanguage"] = new("Effect", new XAttribute("type", "comprehendlanguage")),
			["removecomprehendlanguage"] = new("Effect", new XAttribute("type", "removecomprehendlanguage")),
			["magicresourcedelta"] = new("Effect",
				new XAttribute("type", "magicresourcedelta"),
				new XElement("Resource", context.Resource.Object.Id),
				new XElement("Formula", new XCData("power*2"))
			),
			["spellarmour"] = new("Effect",
				new XAttribute("type", "spellarmour"),
				new XElement("ArmourAppliesProg", context.AlarmProg.Object.Id),
				new XElement("Quality", (int)ItemQuality.Standard),
				new XElement("ArmourType", context.ArmourType.Object.Id),
				new XElement("ArmourMaterial", context.Material.Object.Id),
				new XElement("FullDescriptionAddendum", new XCData("A veil of force shimmers here.")),
				new XElement("CanBeObscuredByInventory", false),
				new XElement("MaximumDamageAbsorbed", new XCData("power*5")),
				new XElement("BodypartShapes",
					new XElement("Shape", context.Shape.Object.Id)
				)
			),
			["roomflag"] = new("Effect",
				new XAttribute("type", "roomflag"),
				new XElement("FlagType", (int)RoomFlagKind.Alarm),
				new XElement("DarknessLux", 123.4),
				new XElement("AlarmEcho", new XCData("@ trigger|triggers a magical alarm.")),
				new XElement("AlarmProg", context.AlarmProg.Object.Id),
				new XElement("WardTag", new XCData("ward"))
			),
			["removeroomflag"] = new("Effect",
				new XAttribute("type", "removeroomflag"),
				new XElement("FlagType", (int)RoomFlagKind.WardTag),
				new XElement("WardTag", new XCData("ward"))
			)
		};
	}

	private static TestSpellContext CreateSpellContext()
	{
		var alwaysTrueProg = CreateFrameworkItemMock<IFutureProg>(1, "AlwaysTrueProg");
		var alarmProg = CreateFrameworkItemMock<IFutureProg>(101, "AlarmProg");
		var resource = CreateFrameworkItemMock<IMagicResource>(42, "Essence");
		var drug = CreateFrameworkItemMock<IDrug>(77, "Black Lotus");
		var armourType = CreateFrameworkItemMock<IArmourType>(11, "Force Mail");
		var material = CreateFrameworkItemMock<ISolid>(21, "Aetherglass");
		var shape = CreateFrameworkItemMock<IBodypartShape>(88, "Torso");

		var futureProgs = CreateCollection(alwaysTrueProg.Object, alarmProg.Object);
		var magicResources = CreateCollection(resource.Object);
		var drugs = CreateCollection(drug.Object);
		var armourTypes = CreateCollection(armourType.Object);
		var materials = CreateCollection(material.Object);
		var shapes = CreateCollection(shape.Object);

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.AlwaysTrueProg).Returns(alwaysTrueProg.Object);
		gameworld.SetupGet(x => x.FutureProgs).Returns(futureProgs.Object);
		gameworld.SetupGet(x => x.MagicResources).Returns(magicResources.Object);
		gameworld.SetupGet(x => x.Drugs).Returns(drugs.Object);
		gameworld.SetupGet(x => x.ArmourTypes).Returns(armourTypes.Object);
		gameworld.SetupGet(x => x.Materials).Returns(materials.Object);
		gameworld.SetupGet(x => x.BodypartShapes).Returns(shapes.Object);

		var spell = new Mock<IMagicSpell>();
		spell.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		spell.SetupGet(x => x.Id).Returns(9001);
		spell.SetupGet(x => x.Name).Returns("Phase One Test Spell");
		spell.SetupGet(x => x.FrameworkItemType).Returns("MagicSpell");
		spell.SetupProperty(x => x.Changed, false);

		return new TestSpellContext(spell, resource, drug, armourType, material, shape, alarmProg);
	}

	private static Mock<IUneditableAll<T>> CreateCollection<T>(params T[] items) where T : class, IFrameworkItem
	{
		var list = items.ToList();
		var mock = new Mock<IUneditableAll<T>>();

		mock.SetupGet(x => x.Count).Returns(() => list.Count);
		mock.As<IEnumerable<T>>().Setup(x => x.GetEnumerator()).Returns(() => list.GetEnumerator());
		mock.Setup(x => x.ForEach(It.IsAny<Action<T>>())).Callback<Action<T>>(action =>
		{
			foreach (var item in list)
			{
				action(item);
			}
		});
		mock.Setup(x => x.Has(It.IsAny<T>())).Returns<T>(value => list.Contains(value));
		mock.Setup(x => x.Has(It.IsAny<long>())).Returns<long>(id => list.Any(x => x.Id == id));
		mock.Setup(x => x.Has(It.IsAny<string>())).Returns<string>(name =>
			list.Any(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)));
		mock.Setup(x => x.Get(It.IsAny<long>())).Returns<long>(id => list.FirstOrDefault(x => x.Id == id));
		mock.Setup(x => x.Get(It.IsAny<string>())).Returns<string>(name =>
			list.Where(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).ToList());
		mock.Setup(x => x.GetByName(It.IsAny<string>())).Returns<string>(name =>
			list.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)));
		mock.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>())).Returns<string, bool>((value, _) =>
		{
			return long.TryParse(value, out var id)
				? list.FirstOrDefault(x => x.Id == id)
				: list.FirstOrDefault(x => x.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase));
		});

		return mock;
	}

	private static Mock<T> CreateFrameworkItemMock<T>(long id, string name) where T : class, IFrameworkItem
	{
		var mock = new Mock<T>();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(name);
		mock.SetupGet(x => x.FrameworkItemType).Returns(typeof(T).Name);
		return mock;
	}

	private sealed record TestSpellContext(
		Mock<IMagicSpell> Spell,
		Mock<IMagicResource> Resource,
		Mock<IDrug> Drug,
		Mock<IArmourType> ArmourType,
		Mock<ISolid> Material,
		Mock<IBodypartShape> Shape,
		Mock<IFutureProg> AlarmProg);
}
