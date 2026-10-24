#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonSimpleStockContentTests
{
	private static readonly ArmageddonSimpleStockBindings Bindings = new(11, 44);

	[TestMethod]
	public void RequiredDefinitions_MatchLockedIdentitiesSchoolsFloorsTargetsAndLifetimes()
	{
		var expected = new (string Key, string School, double Floor, string Target, string Duration)[]
		{
			("fireworks", "fire", 7, "room", "60*grade"), ("distant_mirage", "water", 7, "room", "60*grade"),
			("shadowplay", "shadow", 7, "room", "60*grade"), ("sky_lantern", "lightning", 7, "room", "60*grade"),
			("phantasm", "void", 7, "room", "60*grade"), ("veil_of_elements", "void", 20, "room", "60*grade"),
			("buoyant_lift", "wind", 7, "character", "60*grade"), ("windborne_flight", "wind", 25, "character", "60*grade"),
			("gentle_descent", "wind", 20, "character", "60*grade"), ("sand_barrier", "earth", 10, "exit", "60*grade"),
			("wind_barrier", "wind", 10, "exit", "60*grade"), ("clouded_will", "earth", 20, "character", "0"),
			("arcane_mark", "void", 25, "item", "300*grade"), ("unmaking_ward", "void", 25, "character", "60*grade"),
			("bar_the_elements", "void", 33, "room", "60*grade")
		};
		var actual = ArmageddonSimpleStockContent.Create(Bindings);
		Assert.AreEqual(15, actual.Count); Assert.AreEqual(15, actual.Select(x => x.Content.Key).Distinct().Count());
		foreach (var row in expected)
		{
			var contribution = actual.Single(x => x.Content.Key == "arm.spell." + row.Key);
			var definition = contribution.Content.BuildDefinition(10, 1, 0);
			Assert.AreEqual("arm.capability." + row.School, contribution.NativeCapabilityKey);
			Assert.AreEqual(row.Floor, contribution.Content.MinimumEnergy); Assert.AreEqual(row.Duration, contribution.Content.DurationFormula);
			Assert.AreEqual(row.Target, definition.Element("Trigger")!.Attribute("type")!.Value);
			Assert.AreEqual(contribution.Content.Key, definition.Element("StockIdentity")!.Value);
			Assert.AreEqual(7, definition.Element("ControlledPower")!.Elements("Grade").Count());
			Assert.IsFalse(definition.Element("CasterEffects")!.HasElements);
			Assert.IsNull(definition.Element("ControlledPower")!.Element("Area"));
			Assert.IsFalse(contribution.Content.SpellRow(1, 1, 1).ScrollInscriptionAllowed);
		}
		Assert.IsFalse(actual.Any(x => x.Content.Key == "arm.spell.wardcraft"));
		var optional = ArmageddonSimpleStockContent.OptionalWardcraft().Single();
		Assert.AreEqual("arm.spell.wardcraft", optional.Key); Assert.AreEqual(4.0, optional.MinimumEnergy);
		Assert.AreEqual("personaltagward", optional.BuildDefinition(10, 1, 0).Element("Effects")!.Element("Effect")!.Attribute("type")!.Value);
	}

	[TestMethod]
	public void NativeFactories_AllRequiredAndOptionalDefinitionsLoadReadyAndRoundTrip()
	{
		foreach (var content in ArmageddonSimpleStockContent.Create(Bindings).Select(x => x.Content)
			.Concat(ArmageddonSimpleStockContent.OptionalWardcraft()))
		{
			var f = Fixture(); var spell = Load(f, content);
			Assert.IsTrue(spell.ReadyForGame, content.Key + ": " + (spell.ReadyForGame ? "" : spell.WhyNotReadyForGame(f.Actor.Object)));
			Assert.AreEqual(content.Key, spell.StockIdentity);
			var expected = content.BuildDefinition(10, 1, 0).Element("Effects")!.Elements().ToArray();
			var actual = spell.SpellEffects.ToArray(); Assert.AreEqual(expected.Length, actual.Length, content.Key);
			for (var index = 0; index < actual.Length; index++)
			{
				Assert.IsTrue(actual[index].IsCompatibleWithTrigger(spell.Trigger), content.Key);
				Assert.IsTrue(XNode.DeepEquals(expected[index], actual[index].SaveToXml()), content.Key);
				Assert.IsTrue(XNode.DeepEquals(expected[index], actual[index].Clone().SaveToXml()), content.Key);
			}
			var reloaded = new MagicSpell(spell.SnapshotModel(), f.World.Object);
			Assert.IsTrue(reloaded.ReadyForGame, content.Key);
			Assert.AreEqual(spell.AppliedEffectsAreExclusive, reloaded.AppliedEffectsAreExclusive);
			Assert.IsTrue(XNode.DeepEquals(spell.Trigger.SaveToXml(), reloaded.Trigger.SaveToXml()), content.Key);
			Assert.IsTrue(XNode.DeepEquals(new XElement("Effects", actual.Select(x => x.SaveToXml())),
				new XElement("Effects", reloaded.SpellEffects.Select(x => x.SaveToXml()))), content.Key);
		}
	}

	[TestMethod]
	[DataRow(1, SpellPower.ExtremelyStrong)]
	[DataRow(7, SpellPower.ExtremelyWeak)]
	public void SelectedGrade_BindsNativeDurationAndVeilLuxIndependentlyOfPower(int grade, SpellPower power)
	{
		foreach (var content in ArmageddonSimpleStockContent.Create(Bindings).Select(x => x.Content))
		{
			var f = Fixture(); var spell = Load(f, content);
			var copy = spell.CastingCopy(f.Actor.Object, f.Traits[0], grade, power, Difficulty.Normal);
			Assert.AreEqual((content.DurationFormula == "0" ? 0.0 : content.Key.EndsWith("arcane_mark") ? 300.0 : 60.0) * grade,
				copy.EffectDurationExpression.Evaluate(f.Actor.Object), content.Key);
			Assert.AreEqual(content.MinimumEnergy * grade, copy.CastingCosts.Single().Value.Evaluate(f.Actor.Object), content.Key);
			if (content.Key != ArmageddonSimpleStockContent.VeilOfElementsKey) continue;
			var veil = (RoomLightEffect)copy.SpellEffects.ElementAt(1);
			var child = (SpellRoomLightEffect)veil.GetOrApplyEffect(f.Actor.Object, f.Actor.Object.Location, default, power, null!, [])!;
			Assert.AreEqual(-5.0 * grade, child.GlowLux, 1e-10);
			Assert.AreEqual(-5.0, ((RoomLightEffect)spell.SpellEffects.ElementAt(1)).AddedLuxPerPower, "Casting copies must leave authoring defaults unchanged.");
		}
	}

	[TestMethod]
	public void PhantomAndMovementFactories_CreateNativePerceptionAndPermissionEffects()
	{
		foreach (var content in ArmageddonSimpleStockContent.Create(Bindings).Select(x => x.Content))
		{
			var f = Fixture(); var spell = Load(f, content); var template = spell.SpellEffects.First();
			if (template is PhantomIllusionEffect phantom)
			{
				var child = (SpellPhantomIllusionEffect)phantom.GetOrApplyEffect(f.Actor.Object, f.Actor.Object.Location, default, SpellPower.ExtremelyWeak, null!, [])!;
				Assert.AreEqual(phantom.Text, child.IllusionText); Assert.AreEqual(IllusionAudienceScope.Everyone, child.AudienceScope);
				Assert.AreEqual(100L, child.CasterId); Assert.AreEqual(500L, child.TargetId); Assert.AreEqual(content.Key, child.IllusionKey);
				Assert.IsFalse(child is ITraitBonusEffect); Assert.IsFalse(child is IFlightEffect);
			}
			if (content.Key.EndsWith("buoyant_lift")) Assert.IsInstanceOfType(template.GetOrApplyEffect(f.Actor.Object, f.Actor.Object, default, default, null!, []), typeof(SpellLevitationEffect));
			if (content.Key.EndsWith("windborne_flight")) Assert.IsInstanceOfType(template.GetOrApplyEffect(f.Actor.Object, f.Actor.Object, default, default, null!, []), typeof(SpellFlightEffect));
			if (content.Key.EndsWith("gentle_descent"))
			{
				var feather = (FeatherFallEffect)template;
				Assert.AreEqual(0.0, feather.FallDistanceMultiplier); Assert.AreEqual(0.0, feather.FallDamageMultiplier);
				Assert.IsInstanceOfType(feather.GetOrApplyEffect(f.Actor.Object, f.Actor.Object, default, default, null!, []), typeof(SpellFeatherFallEffect));
			}
		}
	}

	[TestMethod]
	public void BoundResourceAndWards_UseExactSuppliedNativeIdentitiesAndLimitedSelectors()
	{
		var f = Fixture(); var contents = ArmageddonSimpleStockContent.Create(Bindings);
		var loss = (MagicResourceDeltaEffect)Load(f, contents.Single(x => x.Content.Key.EndsWith("clouded_will")).Content).SpellEffects.Single();
		Assert.IsTrue(loss.ExactResourceDepletion); Assert.AreSame(f.Resources.Single(x => x.Id == 11), loss.Resource);
		Assert.AreEqual("-5*grade", loss.DeltaExpression.OriginalFormulaText); Assert.IsNull(loss.DefinitionError);
		foreach (var key in new[] { "unmaking_ward", "bar_the_elements" })
		{
			var ward = (WardSpellEffectBase)Load(f, contents.Single(x => x.Content.Key.EndsWith(key)).Content).SpellEffects.Single();
			Assert.AreEqual(44L, ward.School.Id); Assert.AreEqual(MagicInterdictionMode.Fail, ward.Mode);
			Assert.AreEqual(MagicInterdictionCoverage.Both, ward.Coverage); Assert.IsFalse(ward.IncludesSubschools); Assert.IsNull(ward.Prog);
		}
	}

	[TestMethod]
	public void ArcaneMark_TwoCastersKeepIndependentNativeParentAttributionWithoutReplacement()
	{
		var f = Fixture(); var content = ArmageddonSimpleStockContent.Create(Bindings).Single(x => x.Content.Key.EndsWith("arcane_mark")).Content;
		var spell = Load(f, content); f.Spells.Add(spell);
		Assert.IsFalse(spell.AppliedEffectsAreExclusive); Assert.IsFalse(new MagicSpell(spell.SnapshotModel(), f.World.Object).AppliedEffectsAreExclusive);
		Assert.IsTrue(ArmageddonStockSpellContent.ReadAttunement().SpellRow(1, 1, 1).AppliedEffectsAreExclusive, "The ordinary stock default remains exclusive.");
		var template = (ArcaneMarkEffect)spell.SpellEffects.Single();
		Assert.AreEqual("arcanemark", (string?)template.SaveToXml().Attribute("type"));
		var item = new Mock<IGameItem>(); item.SetupGet(x => x.Id).Returns(700); item.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		var other = new Mock<ICharacter>(); other.SetupGet(x => x.Id).Returns(101); other.SetupGet(x => x.InstanceId).Returns(101); other.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		f.World.Setup(x => x.TryGetCharacter(100, true)).Returns(f.Actor.Object); f.World.Setup(x => x.TryGetCharacter(101, true)).Returns(other.Object);
		MagicSpellParent.InitialiseEffectType(); SpellMagicTagEffect.InitialiseEffectType();
		var saved = new List<XElement>();
		foreach (var caster in new[] { f.Actor.Object, other.Object })
		{
			var parent = new MagicSpellParent(item.Object, spell, caster);
			var mark = new SpellMagicTagEffect(item.Object, parent, ArmageddonSimpleStockContent.ArcaneMarkTag,
				template.Value, savedRoomId: 501, remainingUses: 7);
			parent.AddSpellEffect(mark); Assert.AreSame(caster, mark.Caster); Assert.AreEqual(ArmageddonSimpleStockContent.ArcaneMarkTag, mark.Tag);
			Assert.AreEqual(501L, mark.SavedRoomId); Assert.AreEqual(700L, mark.ItemId); Assert.AreEqual(7, mark.RemainingUses);
			var row = parent.SaveToXml(new Dictionary<IEffect, TimeSpan> { [parent] = TimeSpan.FromSeconds(300) }); saved.Add(row);
			Assert.AreEqual(caster.Id.ToString(), row.Element("Effect")!.Element("Caster")!.Value);
			Assert.AreEqual(caster.InstanceId.ToString(), row.Element("Effect")!.Element("CasterInstance")!.Value);
			var loaded = (MagicSpellParent)Effect.LoadEffect(row, item.Object);
			var loadedMark = (SpellMagicTagEffect)loaded.SpellEffects.Single();
			Assert.AreSame(caster, loadedMark.Caster); Assert.AreEqual(mark.Tag, loadedMark.Tag); Assert.AreEqual(mark.Value, loadedMark.Value);
			Assert.AreEqual(501L, loadedMark.SavedRoomId); Assert.AreEqual(700L, loadedMark.ItemId); Assert.AreEqual(7, loadedMark.RemainingUses);
		}
		Assert.AreEqual(2, saved.Count); Assert.AreNotEqual(saved[0].Element("Effect")!.Element("Caster")!.Value, saved[1].Element("Effect")!.Element("Caster")!.Value);
		item.Verify(x => x.RemoveAllEffects<IMagicTagEffect>(It.IsAny<Predicate<IMagicTagEffect>>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void ArcaneMark_SavedRoomIsTypedAndExact_DirectCustodyRequiredForTravel()
	{
		var f = Fixture(); var content = ArmageddonSimpleStockContent.Create(Bindings)
			.Single(x => x.Content.Key == "arm.spell.arcane_mark").Content;
		var spell = Load(f, content); f.Spells.Add(spell);
		var source = f.Actor.Object.Location;
		var saved = new Mock<IRoom>(); saved.SetupGet(x => x.Id).Returns(501);
		saved.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		f.World.SetupGet(x => x.Rooms).Returns(MagicCastingFixture.Collection(() => new[] { source, saved.Object }));
		f.Body.SetupGet(x => x.Actor).Returns(f.Actor.Object);
		var item = new Mock<IGameItem>(); var effects = new List<IEffect>();
		item.SetupGet(x => x.Id).Returns(700); item.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		item.SetupGet(x => x.Effects).Returns(() => effects);
		item.SetupGet(x => x.TrueLocations).Returns(() => new[] { source });
		item.Setup(x => x.IsOwnedBy(f.Actor.Object)).Returns(true);
		MudSharp.Body.IBody? custodian = f.Body.Object;
		item.SetupGet(x => x.InInventoryOf).Returns(() => custodian!);
		f.World.SetupGet(x => x.Items).Returns(MagicCastingFixture.Collection(() => new[] { item.Object }));
		var parent = new MagicSpellParent(item.Object, spell, f.Actor.Object);
		var mark = new SpellMagicTagEffect(item.Object, parent, ArmageddonSimpleStockContent.ArcaneMarkTag,
			"An angular arcane glyph.", savedRoomId: 501, remainingUses: 2);
		parent.AddSpellEffect(mark); effects.Add(parent); effects.Add(mark);
		Assert.IsTrue(mark.IsActive);
		Assert.IsTrue(SpellTravelAnchorResolver.TryResolve(f.Actor.Object, item.Object,
			SpellTravelAnchorMode.ArcaneMarkedRoom, out var resolved, out var error), error);
		Assert.AreSame(saved.Object, resolved!.Destination);
		Assert.IsTrue(resolved.IsCurrentFor(f.Actor.Object));
		Assert.IsFalse(new SpellMagicTagEffect(item.Object, parent, ArmageddonSimpleStockContent.ArcaneMarkTag,
			"Legacy informational tag").IsActive);
		Assert.AreEqual(2, mark.RemainingUses);
		effects.Clear();
		Assert.IsFalse(resolved.IsCurrentFor(f.Actor.Object));
		var exhaustedParent = new MagicSpellParent(item.Object, spell, f.Actor.Object);
		var exhausted = new SpellMagicTagEffect(item.Object, exhaustedParent, ArmageddonSimpleStockContent.ArcaneMarkTag,
			"An exhausted glyph.", savedRoomId: 501, remainingUses: 0);
		exhaustedParent.AddSpellEffect(exhausted); effects.Add(exhaustedParent); effects.Add(exhausted);
		Assert.IsFalse(exhausted.IsActive);
		Assert.IsFalse(SpellTravelAnchorResolver.TryResolve(f.Actor.Object, item.Object,
			SpellTravelAnchorMode.ArcaneMarkedRoom, out _, out _));
		var exhaustedXml = exhaustedParent.SaveToXml(new Dictionary<IEffect, TimeSpan> { [exhaustedParent] = TimeSpan.FromSeconds(120) });
		var loaded = (MagicSpellParent)Effect.LoadEffect(exhaustedXml, item.Object);
		Assert.AreEqual(0, ((SpellMagicTagEffect)loaded.SpellEffects.Single()).RemainingUses);
		custodian = null;
		Assert.IsFalse(resolved.IsCurrentFor(f.Actor.Object));
		Assert.IsFalse(SpellTravelAnchorResolver.TryResolve(f.Actor.Object, item.Object,
			SpellTravelAnchorMode.ArcaneMarkedRoom, out _, out _));
	}

	[TestMethod]
	public void ArcaneMark_AllGradesFreezeDirectItemCustodyAndRoomBeforePayment()
	{
		for (var grade = 1; grade <= 7; grade++)
		{
			var f = Fixture(); var content = ArmageddonSimpleStockContent.Create(Bindings)
				.Single(x => x.Content.Key == "arm.spell.arcane_mark").Content;
			var spell = Load(f, content);
			var casting = spell.CastingCopy(f.Actor.Object, f.Traits[0], grade, SpellPower.Standard, Difficulty.Normal);
			var template = (ArcaneMarkEffect)casting.SpellEffects.Single();
			f.Body.SetupGet(x => x.Actor).Returns(f.Actor.Object);
			var room = f.Actor.Object.Location;
			f.World.SetupGet(x => x.Rooms).Returns(MagicCastingFixture.Collection(() => new[] { room }));
			var item = new Mock<IGameItem>();
			item.SetupGet(x => x.Id).Returns(700);
			item.SetupGet(x => x.Gameworld).Returns(f.World.Object);
			item.SetupGet(x => x.TrueLocations).Returns(() => new[] { room });
			MudSharp.Body.IBody? custodian = f.Body.Object;
			item.SetupGet(x => x.InInventoryOf).Returns(() => custodian!);
			f.World.SetupGet(x => x.Items).Returns(MagicCastingFixture.Collection(() => new[] { item.Object }));
			var token = template.CapturePreparedSelection(f.Actor.Object, item.Object);
			Assert.IsTrue(((IMagicSpellEffectPreparedSelectionRawToken)token).IsCurrent);
			Assert.IsTrue(template.TryPrepareApplication(f.Actor.Object, item.Object, default, default,
				TimeSpan.FromSeconds(300 * grade), out _, out var error), error);
			custodian = null;
			Assert.IsFalse(template.TryConfirmPreparedSelection(f.Actor.Object, item.Object, out _));
		}
	}

	[TestMethod]
	[DataRow(0L, 44L)]
	[DataRow(11L, 0L)]
	[DataRow(-1L, 44L)]
	[DataRow(11L, -1L)]
	public void InvalidTypedBindings_RefuseBeforeDefinitionConstruction(long reserve, long school) =>
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonSimpleStockContent.Create(new(reserve, school)));

	[TestMethod]
	public void NativeRoomAndExitTriggers_RejectUnsupportedFiltersInsteadOfLosingThemOnSave()
	{
		foreach (var content in ArmageddonSimpleStockContent.Create(Bindings).Select(x => x.Content))
		{
			var trigger = content.BuildDefinition(10, 1, 0).Element("Trigger")!;
			if (trigger.Attribute("type")!.Value is not ("room" or "exit")) continue;
			Assert.IsNull(trigger.Element("TargetFilterProg"));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => content.BuildDefinition(10, 1, 1));
		}
	}

	private static MagicCastingFixture Fixture()
	{
		var f = new MagicCastingFixture(); var fire = new Mock<IMagicSchool>(); fire.SetupGet(x => x.Id).Returns(44);
		fire.SetupGet(x => x.Name).Returns("Authored Fire"); fire.SetupGet(x => x.PowerListColour).Returns(Telnet.Red);
		f.World.SetupGet(x => x.MagicSchools).Returns(MagicCastingFixture.Collection(() => new[] { f.School, fire.Object }));
		Mock.Get(f.Actor.Object.Location).SetupGet(x => x.Id).Returns(500); Mock.Get(f.Actor.Object.Location).SetupGet(x => x.Gameworld).Returns(f.World.Object);
		Mock.Get(f.World.Object.LightModel).Setup(x => x.GetIlluminationDescription(It.IsAny<double>())).Returns("dim");
		return f;
	}

	private static MagicSpell Load(MagicCastingFixture f, ArmageddonUtilitySpellContent content)
	{
		f.Expressions.Clear(); var cost = content.CostRow(900); cost.Id = 1; var duration = content.DurationRow(900); duration.Id = 2;
		f.Expressions.Add(new TraitExpression(cost, f.World.Object)); f.Expressions.Add(new TraitExpression(duration, f.World.Object));
		var row = content.SpellRow(1, 1, 1); row.Id = 900; row.EffectDurationExpressionId = 2;
		row.Definition = content.BuildDefinition(10, 1, 0).ToString(); return new MagicSpell(row, f.World.Object);
	}
}
