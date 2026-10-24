#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.SpellTriggers;
using MudSharp.Movement;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicPersistentSensoryCombatTests
{
	private static readonly (string BuilderType, string SavedType)[] EffectTypes =
	[
		("burning", "burning"),
		("ignite", "burning"),
		("trackmark", "trackmark"),
		("tracktrail", "trackmark")
	];

	[TestInitialize]
	public void TestInitialize()
	{
		SpellTriggerFactory.SetupFactory();
		SpellEffectFactory.SetupFactory();
	}

	[TestMethod]
	public void SpellEffectFactory_RegistersPersistentSensoryCombatEffectTypes()
	{
		var spell = CreateSpellMock();
		foreach (var (builderType, savedType) in EffectTypes)
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
	public void SpellEffectFactory_LoadEffect_RoundTripsPersistentSensoryCombatXml()
	{
		var spell = CreateSpellMock();

		foreach (var definition in PersistentSensoryCombatDefinitions())
		{
			var effect = SpellEffectFactory.LoadEffect(definition, spell.Object);
			var saved = effect.SaveToXml();
			var expectedType = definition.Attribute("type")?.Value switch
			{
				"ignite" => "burning",
				"tracktrail" => "trackmark",
				var type => type
			};

			Assert.AreEqual(expectedType, saved.Attribute("type")?.Value);
		}
	}

	[TestMethod]
	[DataRow(1, 2.0)]
	[DataRow(7, 14.0)]
	public void BurningEffect_SelectedExternalGrade_DeterminesPersistentTickDamage(int grade, double expected)
	{
		var fixture = new MagicCastingFixture();
		typeof(MagicSpell).GetProperty("InvocationGrade", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(fixture.Spell, grade);
		var root = ArmageddonImmolateSpellContent.Create().Content.BuildDefinition(10, 11, 0);
		var effect = SpellEffectFactory.LoadEffect(root.Element("Effects")!.Elements().Last(), fixture.Spell);
		var target = CreateCharacter(8, "Target", fixture.World.Object);
		var parent = CreateParent(fixture.Spell, fixture.Actor.Object);
		var applied = effect.GetOrApplyEffect(fixture.Actor.Object, target.Object, OpposedOutcomeDegree.None,
			SpellPower.ExtremelyWeak, parent.Object, []);
		Assert.IsInstanceOfType(applied, typeof(SpellBurningEffect));
		Assert.AreEqual(expected, ((SpellBurningEffect)applied).DamagePerTick, 1e-9);
	}

	[TestMethod]
	public void SpellBurningEffect_RemovalDuringDamageCallback_DoesNotReschedule()
	{
		var fixture = CreateBurningTickFixture();
		fixture.Body.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Returns<IDamage>(_ =>
		{
			fixture.Effect.RemovalEffect();
			fixture.OwnerEffects.Remove(fixture.Effect);
			return Array.Empty<IWound>();
		});

		fixture.Effect.ExpireEffect();

		fixture.Body.Verify(x => x.PassiveSufferDamage(It.IsAny<IDamage>()), Times.Once);
		fixture.Target.Verify(x => x.Reschedule(fixture.Effect, It.IsAny<TimeSpan>()), Times.Never);
	}

	[TestMethod]
	public void SpellBurningEffect_BodySwapDuringSelection_RejectsStaleTick()
	{
		var fixture = CreateBurningTickFixture();
		fixture.Body.SetupGet(x => x.RandomBodypart).Returns(() =>
		{
			fixture.Target.SetupGet(x => x.Body).Returns(Mock.Of<IBody>());
			return fixture.Part.Object;
		});

		fixture.Effect.ExpireEffect();

		fixture.Body.Verify(x => x.PassiveSufferDamage(It.IsAny<IDamage>()), Times.Never);
		fixture.Target.Verify(x => x.Reschedule(fixture.Effect, It.IsAny<TimeSpan>()), Times.Never);
	}

	[TestMethod]
	public void SpellBurningEffect_DetachedParent_RejectsTick()
	{
		var fixture = CreateBurningTickFixture();
		fixture.OwnerEffects.Remove(fixture.Parent.Object);

		fixture.Effect.ExpireEffect();

		fixture.Body.Verify(x => x.PassiveSufferDamage(It.IsAny<IDamage>()), Times.Never);
		fixture.Target.Verify(x => x.Reschedule(fixture.Effect, It.IsAny<TimeSpan>()), Times.Never);
	}

	[TestMethod]
	public void SpellBurningEffect_NativeMutationGuard_RejectsDispelledTarget()
	{
		var fixture = CreateBurningTickFixture();
		fixture.Body.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Returns<IDamage>(damage =>
		{
			fixture.Effect.RemovalEffect();
			fixture.OwnerEffects.Remove(fixture.Effect);
			var rejected = false;
			try { damage.MutationGuard.Validate(fixture.Body.Object, damage); }
			catch (InvalidOperationException) { rejected = true; }
			Assert.IsTrue(rejected);
			return Array.Empty<IWound>();
		});

		fixture.Effect.ExpireEffect();

		fixture.Target.Verify(x => x.Reschedule(fixture.Effect, It.IsAny<TimeSpan>()), Times.Never);
	}

	[TestMethod]
	public void SpellBurningEffect_NativeGuardRefusal_DoesNotEscapeScheduledTick()
	{
		var fixture = CreateBurningTickFixture();
		fixture.Body.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Returns<IDamage>(damage =>
		{
			fixture.Effect.RemovalEffect();
			fixture.OwnerEffects.Remove(fixture.Effect);
			damage.MutationGuard.Validate(fixture.Body.Object, damage);
			return Array.Empty<IWound>();
		});

		fixture.Effect.ExpireEffect();

		fixture.Target.Verify(x => x.Reschedule(fixture.Effect, It.IsAny<TimeSpan>()), Times.Never);
	}

	[TestMethod]
	public void SpellBurningEffect_NativeMutationGuard_RejectsBodySwap()
	{
		var fixture = CreateBurningTickFixture();
		fixture.Body.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Returns<IDamage>(damage =>
		{
			fixture.Target.SetupGet(x => x.Body).Returns(Mock.Of<IBody>());
			var rejected = false;
			try { damage.MutationGuard.Validate(fixture.Body.Object, damage); }
			catch (InvalidOperationException) { rejected = true; }
			Assert.IsTrue(rejected);
			return Array.Empty<IWound>();
		});

		fixture.Effect.ExpireEffect();

		fixture.Target.Verify(x => x.Reschedule(fixture.Effect, It.IsAny<TimeSpan>()), Times.Never);
	}

	[TestMethod]
	public void SpellBurningEffect_AttachedNativeTick_ValidatesAndReschedulesOnce()
	{
		var fixture = CreateBurningTickFixture();
		fixture.Body.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Returns<IDamage>(damage =>
		{
			damage.MutationGuard.Validate(fixture.Body.Object, damage);
			return Array.Empty<IWound>();
		});

		fixture.Effect.ExpireEffect();

		fixture.Body.Verify(x => x.PassiveSufferDamage(It.IsAny<IDamage>()), Times.Once);
		fixture.Target.Verify(x => x.Reschedule(fixture.Effect, TimeSpan.FromSeconds(10)), Times.Once);
	}

	[TestMethod]
	public void SpellBurningEffect_NativeMutationGuard_AllowsCapturedWornArmourProjection()
	{
		var fixture = CreateBurningTickFixture();
		var armour = new Mock<IGameItem>();
		fixture.Body.SetupGet(x => x.WornItems).Returns([armour.Object]);
		fixture.Body.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Returns<IDamage>(damage =>
		{
			var projection = new Damage(damage) { Bodypart = null };
			projection.MutationGuard.Validate(armour.Object, projection);
			return Array.Empty<IWound>();
		});

		fixture.Effect.ExpireEffect();

		fixture.Target.Verify(x => x.Reschedule(fixture.Effect, TimeSpan.FromSeconds(10)), Times.Once);
	}

	[TestMethod]
	public void SpellBurningEffect_NativeMutationGuard_AllowsCapturedBoneProjection()
	{
		var fixture = CreateBurningTickFixture();
		var bone = new Mock<IBone>();
		fixture.Body.SetupGet(x => x.Bones).Returns([bone.Object]);
		fixture.Body.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Returns<IDamage>(damage =>
		{
			var projection = new Damage(damage) { Bodypart = bone.Object };
			projection.MutationGuard.Validate(fixture.Body.Object, projection);
			return Array.Empty<IWound>();
		});

		fixture.Effect.ExpireEffect();

		fixture.Target.Verify(x => x.Reschedule(fixture.Effect, TimeSpan.FromSeconds(10)), Times.Once);
	}

	[TestMethod]
	public void SpellBurningEffect_LostArmourCustody_StopsTickBeforeNativeLayerFence()
	{
		var fixture = CreateBurningTickFixture();
		var armour = new Mock<IGameItem>();
		var destroyed = false;
		armour.SetupGet(x => x.Destroyed).Returns(() => destroyed);
		IGameItem[] worn = [armour.Object];
		fixture.Body.SetupGet(x => x.WornItems).Returns(() => worn);
		fixture.Body.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Returns<IDamage>(damage =>
		{
			destroyed = true;
			worn = [];
			damage.MutationGuard.Validate(fixture.Body.Object, damage);
			return Array.Empty<IWound>();
		});

		fixture.Effect.ExpireEffect();

		fixture.Target.Verify(x => x.Reschedule(fixture.Effect, It.IsAny<TimeSpan>()), Times.Never);
	}

	[TestMethod]
	public void SpellBurningEffect_ProcessedArmourCanBeDestroyedBeforeRemainingBodyInjury()
	{
		var fixture = CreateBurningTickFixture();
		var first = new Mock<IGameItem>();
		var second = new Mock<IGameItem>();
		IGameItem[] worn = [first.Object, second.Object];
		fixture.Body.SetupGet(x => x.WornItems).Returns(() => worn);
		fixture.Body.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Returns<IDamage>(damage =>
		{
			var firstProjection = new Damage(damage) { Bodypart = null };
			firstProjection.MutationGuard.Validate(first.Object, firstProjection);
			worn = [second.Object];
			damage.MutationGuard.Validate(fixture.Body.Object, damage);
			return Array.Empty<IWound>();
		});

		fixture.Effect.ExpireEffect();

		fixture.Target.Verify(x => x.Reschedule(fixture.Effect, TimeSpan.FromSeconds(10)), Times.Once);
	}

	[TestMethod]
	public void PersistentSensoryCombatStatusEffects_ExposeRuntimeInterfacesAndSaveConfiguration()
	{
		var spell = CreateSpellMock();
		var caster = CreateCharacter(7, "Caster", spell.Object.Gameworld);
		var owner = CreatePerceivable(spell.Object.Gameworld);
		var parent = CreateParent(spell.Object, caster.Object).Object;

		var burning = new SpellBurningEffect(owner.Object, parent, null, DamageType.Burning,
			3.0, 2.0, 1.0, 0.5, 9.0, 0.2, true,
			"(burning)", "@ is burning.", Telnet.BoldRed);
		var trackmark = new SpellTrackMarkEffect(owner.Object, parent, null,
			2.0, 1.5, 0.5, 0.25, TrackCircumstances.MagicallyMarked,
			"(leaving luminous tracks)", Telnet.BoldCyan);

		Assert.IsInstanceOfType(burning, typeof(IDescriptionAdditionEffect));
		Assert.IsInstanceOfType(burning, typeof(ISDescAdditionEffect));
		Assert.AreEqual("SpellBurning", burning.SaveToXml(new Dictionary<IEffect, TimeSpan>())
		                                      .Element("Type")?.Value);
		Assert.IsInstanceOfType(trackmark, typeof(ITrackIntensityEffect));
		Assert.IsInstanceOfType(trackmark, typeof(ISDescAdditionEffect));
		Assert.AreEqual(2.0, trackmark.VisualTrackIntensityMultiplier);
		Assert.AreEqual(1.5, trackmark.OlfactoryTrackIntensityMultiplier);
		Assert.AreEqual(0.5, trackmark.VisualTrackIntensityBonus);
		Assert.AreEqual(0.25, trackmark.OlfactoryTrackIntensityBonus);
		Assert.IsTrue(trackmark.AdditionalTrackCircumstances.HasFlag(TrackCircumstances.MagicallyMarked));
		Assert.AreEqual("SpellTrackMark", trackmark.SaveToXml(new Dictionary<IEffect, TimeSpan>())
		                                           .Element("Type")?.Value);
	}

	[TestMethod]
	public void Movement_GetTrackIntensities_AppliesTrackIntensityEffects()
	{
		var race = new Mock<IRace>();
		race.SetupGet(x => x.TrackIntensityVisual).Returns(2.0);
		race.SetupGet(x => x.TrackIntensityOlfactory).Returns(0.5);
		var terrain = new Mock<ITerrain>();
		terrain.SetupGet(x => x.TrackIntensityMultiplierVisual).Returns(1.5);
		terrain.SetupGet(x => x.TrackIntensityMultiplierOlfactory).Returns(0.75);
		var room = new Mock<IRoom>();
		room.Setup(x => x.Terrain(It.IsAny<IPerceiver>())).Returns(terrain.Object);
		room.Setup(x => x.IsSwimmingLayer(RoomLayer.GroundLevel)).Returns(false);
		var effect = new Mock<ITrackIntensityEffect>();
		effect.SetupGet(x => x.VisualTrackIntensityMultiplier).Returns(2.0);
		effect.SetupGet(x => x.OlfactoryTrackIntensityMultiplier).Returns(3.0);
		effect.SetupGet(x => x.VisualTrackIntensityBonus).Returns(0.5);
		effect.SetupGet(x => x.OlfactoryTrackIntensityBonus).Returns(0.25);
		effect.SetupGet(x => x.AdditionalTrackCircumstances).Returns(TrackCircumstances.MagicallyMarked);
		effect.Setup(x => x.Applies()).Returns(true);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Race).Returns(race.Object);
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		actor.SetupGet(x => x.RidingMount).Returns((ICharacter?)null);
		actor.Setup(x => x.CombinedEffectsOfType<ITrackIntensityEffect>()).Returns([effect.Object]);

		var circumstance = TrackCircumstances.None;
		var noTracks = Movement.GetTrackIntensities(actor.Object, room.Object, ref circumstance,
			out var visual, out var olfactory);

		Assert.IsFalse(noTracks);
		Assert.AreEqual(6.5, visual, 0.0001);
		Assert.AreEqual(1.375, olfactory, 0.0001);
		Assert.IsTrue(circumstance.HasFlag(TrackCircumstances.MagicallyMarked));

		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.InAir);
		circumstance = TrackCircumstances.None;
		noTracks = Movement.GetTrackIntensities(actor.Object, room.Object, ref circumstance,
			out visual, out olfactory);

		Assert.IsFalse(noTracks);
		Assert.AreEqual(0.0, visual, 0.0001);
		Assert.AreEqual(1.375, olfactory, 0.0001);
		Assert.IsTrue(circumstance.HasFlag(TrackCircumstances.MagicallyMarked));
	}

	[TestMethod]
	public void DispelMagicEffect_BurningAndTrackmarkKeys_RemoveMatchingSpellParent()
	{
		foreach (var (key, createEffect) in new (string Key,
			         Func<IPerceivable, IMagicSpellEffectParent, IMagicSpellEffect> CreateEffect)[]
		         {
			         ("burning", (owner, parent) => new SpellBurningEffect(owner, parent, null, DamageType.Burning,
				         3.0, 2.0, 1.0, 0.0, 10.0, 0.1, true, "(burning)", "@ is burning.", Telnet.BoldRed)),
			         ("trackmark", (owner, parent) => new SpellTrackMarkEffect(owner, parent, null,
				         2.0, 1.0, 1.0, 0.0, TrackCircumstances.MagicallyMarked,
				         "(leaving luminous tracks)", Telnet.BoldCyan))
		         })
		{
			var spell = CreateSpellMock();
			var caster = CreateCharacter(7, "Caster", spell.Object.Gameworld);
			var target = CreatePerceivable(spell.Object.Gameworld);
			var parent = new MagicSpellParent(target.Object, spell.Object, caster.Object);
			parent.AddSpellEffect(createEffect(target.Object, parent));
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
				new XElement("EffectKey", new XCData(key))
			), spell.Object);

			dispel.GetOrApplyEffect(caster.Object, target.Object, OpposedOutcomeDegree.None,
				SpellPower.Insignificant, CreateParent(spell.Object, caster.Object).Object, []);

			target.Verify(x => x.RemoveEffect(parent, true), Times.Once,
				$"Dispel key {key} did not remove the matching spell parent.");
		}
	}

	private static IEnumerable<XElement> PersistentSensoryCombatDefinitions()
	{
		yield return new XElement("Effect", new XAttribute("type", "burning"),
			new XElement("DamageType", (int)DamageType.Burning),
			new XElement("DamageFormula", new XCData("power + outcome")),
			new XElement("PainFormula", new XCData("power")),
			new XElement("StunFormula", new XCData("outcome")),
			new XElement("ThermalFormula", new XCData("power * 0.25")),
			new XElement("TickSeconds", 7.0),
			new XElement("MinimumOxidation", 0.2),
			new XElement("SelfOxidising", true),
			new XElement("SDescAddendum", new XCData("(burning)")),
			new XElement("DescAddendum", new XCData("@ is burning with magical fire.")),
			new XElement("AddendumColour", "bold red"));
		yield return new XElement("Effect", new XAttribute("type", "ignite"));
		yield return new XElement("Effect", new XAttribute("type", "trackmark"),
			new XElement("VisualMultiplier", 2.5),
			new XElement("OlfactoryMultiplier", 1.5),
			new XElement("VisualBonus", 1.0),
			new XElement("OlfactoryBonus", 0.25),
			new XElement("MarkTracks", true),
			new XElement("SDescAddendum", new XCData("(leaving shining tracks)")),
			new XElement("AddendumColour", "bold cyan"));
		yield return new XElement("Effect", new XAttribute("type", "tracktrail"));
	}

	private static (Mock<ICharacter> Target, Mock<IBody> Body, Mock<IBodypart> Part,
		Mock<IMagicSpellEffectParent> Parent, SpellBurningEffect Effect, List<IEffect> OwnerEffects)
		CreateBurningTickFixture()
	{
		var spell = CreateSpellMock();
		var caster = CreateCharacter(7, "Caster", spell.Object.Gameworld);
		var target = CreateCharacter(8, "Target", spell.Object.Gameworld);
		var body = new Mock<IBody>();
		var part = new Mock<IBodypart>();
		body.SetupGet(x => x.Actor).Returns(target.Object);
		body.SetupGet(x => x.RandomBodypart).Returns(part.Object);
		body.SetupGet(x => x.Bodyparts).Returns([part.Object]);
		body.SetupGet(x => x.WornItems).Returns(Array.Empty<IGameItem>());
		body.SetupGet(x => x.Prosthetics).Returns(Array.Empty<IProsthetic>());
		target.SetupGet(x => x.Body).Returns(body.Object);
		var parent = CreateParent(spell.Object, caster.Object);
		parent.SetupGet(x => x.Owner).Returns(target.Object);
		var children = new List<IMagicSpellEffect>();
		parent.SetupGet(x => x.SpellEffects).Returns(children);
		parent.Setup(x => x.RemoveSpellEffect(It.IsAny<IMagicSpellEffect>()))
			.Callback<IMagicSpellEffect>(child => children.Remove(child));
		var effect = new SpellBurningEffect(target.Object, parent.Object, null, DamageType.Burning,
			2, 0, 0, 0, 10, 0, true, "(burning)", "@ is burning.", Telnet.BoldRed);
		children.Add(effect);
		var ownerEffects = new List<IEffect> { parent.Object, effect };
		target.SetupGet(x => x.Effects).Returns(ownerEffects);
		body.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Returns(Array.Empty<IWound>());
		return (target, body, part, parent, effect, ownerEffects);
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
		return perceivable;
	}

	private static Mock<ICharacter> CreateCharacter(long id, string name, IFuturemud gameworld)
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(id);
		character.SetupGet(x => x.Name).Returns(name);
		character.SetupGet(x => x.Gameworld).Returns(gameworld);
		character.SetupProperty(x => x.EffectsChanged);
		return character;
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
		school.SetupGet(x => x.Name).Returns("Persistent Sensory Combat");
		school.SetupGet(x => x.PowerListColour).Returns(Telnet.BoldCyan);
		return school;
	}

	private static Mock<IMagicSpell> CreateSpellMock()
	{
		var spell = new Mock<IMagicSpell>();
		spell.SetupGet(x => x.Gameworld).Returns(CreateGameworld().Object);
		spell.SetupGet(x => x.School).Returns(CreateSchool().Object);
		spell.SetupGet(x => x.Id).Returns(9005);
		spell.SetupGet(x => x.Name).Returns("Persistent Sensory Combat Test Spell");
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
