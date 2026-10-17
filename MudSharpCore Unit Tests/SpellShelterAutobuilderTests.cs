#nullable enable

using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Autobuilder;
using MudSharp.Construction.Autobuilder.Rooms;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;
using MudSharp.Work.Foraging;

namespace MudSharp_Unit_Tests;

[TestClass]
public sealed class SpellShelterAutobuilderTests
{
	[TestMethod]
	public void NativeTerrainPresentation_ResolvesDefaultAndOverrideWithoutAllocatingRooms()
	{
		var f = new ShelterPresentationFixture();
		var initial = f.Template.ResolvePresentation();
		Assert.AreEqual("A Cave refuge", initial.RoomName); Assert.AreSame(f.Terrain.Object, initial.Terrain);
		var alternative = Mock.Of<ITerrain>(x => x.Id == 22 && x.Name == "Hallway");
		f.Template.TerrainInfos[alternative] = f.Template.DefaultInfo with
			{ DefaultTerrain = alternative, RoomName = "A $terrain shelter", RoomDescription = "A $terrain space.", AmbientLightFactor = 0.5 };
		var resolved = f.Template.ResolvePresentation(alternative);
		Assert.AreEqual("A Hallway shelter", resolved.RoomName); Assert.AreSame(alternative, resolved.Terrain);
		Assert.AreEqual(0.5, resolved.AmbientLightFactor);
		Assert.AreEqual(0, f.World.Object.Rooms.Count());
		f.World.Verify(x => x.Add(It.IsAny<IRoom>()), Times.Never);
		var frozen = f.Template.PresentationDefinition;
		f.Template.DefaultInfo = f.Template.DefaultInfo with { RoomDescription = "A builder edit." };
		Assert.AreNotEqual(frozen, f.Template.PresentationDefinition);
	}

	[TestMethod]
	public void ShelterPresentation_AutobuilderIdNeverResolvesAsRoomId_UsesUnsavedTypedRows()
	{
		var f = new ShelterPresentationFixture();
		var service = new SpellOwnedShelterService(f.World.Object);
		Assert.IsNull(service.ResolveTemplateOverlay(f.Anchor.Object, f.Configuration, out var row, out var forage));
		Assert.IsNotNull(row); Assert.AreEqual(0L, row.Id); Assert.IsNull(row.Room);
		Assert.AreEqual("A Cave refuge", row.RoomName); Assert.AreEqual(f.Terrain.Object.Id, row.TerrainId);
		Assert.AreEqual(11L, row.RoomOverlayPackageId); Assert.AreEqual(2, row.RoomOverlayPackageRevisionNumber);
		Assert.AreEqual(8L, row.AtmosphereId); Assert.AreEqual("gas", row.AtmosphereType); Assert.IsNull(forage);
		f.Rooms.Verify(x => x.Get(f.Template.Id), Times.Never);
		f.World.Verify(x => x.Add(It.IsAny<IRoom>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow("missing")]
	[DataRow("dual")]
	[DataRow("unsupported")]
	[DataRow("outdoor")]
	[DataRow("liquid")]
	[DataRow("foreign-terrain")]
	[DataRow("foreign-gas")]
	[DataRow("withdrawn-package")]
	[DataRow("invalid-light")]
	public void ShelterPresentation_UnsupportedOrStaleBindings_RefuseWithoutTopology(string change)
	{
		var f = new ShelterPresentationFixture(); var config = f.Configuration;
		if (change == "missing") f.Templates.Setup(x => x.Get(f.Template.Id)).Returns((IAutobuilderRoom)null!);
		if (change == "dual") config = config with { TemplateRoomId = 7 };
		if (change == "unsupported") f.Templates.Setup(x => x.Get(f.Template.Id)).Returns(Mock.Of<IAutobuilderRoom>());
		if (change == "outdoor") f.Template.DefaultInfo = f.Template.DefaultInfo with { OutdoorsType = RoomOutdoorsType.Outdoors };
		if (change == "liquid") f.Terrain.SetupGet(x => x.Atmosphere).Returns(Mock.Of<ILiquid>());
		if (change == "foreign-terrain") f.World.SetupGet(x => x.Terrains).Returns(MagicCastingFixture.Collection<ITerrain>(() => []));
		if (change == "foreign-gas") f.World.SetupGet(x => x.Gases).Returns(MagicCastingFixture.Collection<IGas>(() => []));
		if (change == "withdrawn-package") f.Package.SetupGet(x => x.Status).Returns(RevisionStatus.UnderDesign);
		if (change == "invalid-light") f.Template.DefaultInfo = f.Template.DefaultInfo with { AmbientLightFactor = double.NaN };
		Assert.IsNotNull(new SpellOwnedShelterService(f.World.Object).ResolveTemplateOverlay(f.Anchor.Object, config, out var row, out _));
		Assert.IsNull(row); f.World.Verify(x => x.Add(It.IsAny<IRoom>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(1)]
	[DataRow(2)]
	[DataRow(3)]
	[DataRow(4)]
	[DataRow(5)]
	[DataRow(6)]
	[DataRow(7)]
	public void SelectedGradeShelter_AbsoluteOriginFreezesApprovedCapacityAndLifetime(int grade)
	{
		var f = new ShelterPresentationFixture(); var cast = new MagicCastingFixture();
		cast.Actor.SetupGet(x => x.Location).Returns(f.Anchor.Object);
		cast.World.SetupGet(x => x.Rooms).Returns(f.Rooms.Object);
		cast.World.SetupGet(x => x.AutobuilderRooms).Returns(f.Templates.Object);
		var service = new Mock<ISpellOwnedShelterService>();
		cast.World.SetupGet(x => x.SpellOwnedShelters).Returns(service.Object);
		SpellLifecycleOrigin? created = null;
		service.Setup(x => x.Create(cast.Actor.Object, f.Anchor.Object, It.IsAny<SpellShelterConfiguration>(), It.IsAny<SpellLifecycleOrigin>()))
			.Returns(Mock.Of<IRoom>()).Callback<ICharacter, IRoom, SpellShelterConfiguration, SpellLifecycleOrigin>((_, _, _, origin) => created = origin);
		var configuration = f.Configuration with { SecondsPerGrade = 300 };
		var spell = cast.NewSpell(2, "Shelter", CreateShelterEffect.Definition(configuration).ToString());
		Assert.IsTrue(spell.BuildingCommand(cast.Actor.Object, new StringStack("grades fixture")));
		var copy = (MagicSpell)typeof(MagicSpell).GetMethod("CastingCopy", BindingFlags.NonPublic | BindingFlags.Instance)!
			.Invoke(spell, [cast.Actor.Object, cast.Traits[0], grade, spell.GradeProfile!.Grades.Single(x => x.Grade == grade).Power,
				Difficulty.Automatic, grade])!;
		var effect = (CreateShelterEffect)copy.SpellEffects.Single();
		Assert.IsTrue(effect.TryPrepareApplication(cast.Actor.Object, f.Anchor.Object, OpposedOutcomeDegree.None, SpellPower.Standard,
			TimeSpan.Zero, out var application, out var error), error);
		((IMagicSpellEffectApplicationOperation)application!).Apply(null!);
		Assert.IsNotNull(created); Assert.AreEqual(grade, created.Grade);
		Assert.AreEqual(grade, SpellShelterAnchor.Load(created.Provenance).MaximumOccupants);
		Assert.AreEqual(TimeSpan.FromSeconds(300 * grade), created.DeadlineUtc!.Value - created.CreatedUtc);
		Assert.AreEqual(1, configuration.MaximumOccupants); Assert.IsTrue(configuration.CapacityByGrade);
		var roundTrip = new CreateShelterEffect(effect.SaveToXml(), spell).Configuration();
		Assert.AreEqual(7L, roundTrip.AutobuilderTemplateId); Assert.AreEqual(0L, roundTrip.TemplateRoomId);
		Assert.AreEqual(grade, roundTrip.CapacityForGrade(grade));
	}

	[DataTestMethod]
	[DataRow("definition")]
	[DataRow("registry")]
	[DataRow("terrain-name")]
	[DataRow("terrain-atmosphere")]
	[DataRow("same-id-terrain")]
	public void PreparedAutobuilderPresentation_ChangesInvalidateRetainedSelection(string change)
	{
		var f = new ShelterPresentationFixture(); var cast = new MagicCastingFixture();
		cast.Actor.SetupGet(x => x.Location).Returns(f.Anchor.Object);
		cast.World.SetupGet(x => x.Rooms).Returns(f.Rooms.Object);
		cast.World.SetupGet(x => x.AutobuilderRooms).Returns(f.Templates.Object);
		cast.World.SetupGet(x => x.SpellOwnedShelters).Returns(Mock.Of<ISpellOwnedShelterService>());
		var spell = cast.NewSpell(2, "Shelter", CreateShelterEffect.Definition(f.Configuration).ToString());
		Assert.IsTrue(spell.BuildingCommand(cast.Actor.Object, new StringStack("grades fixture")));
		var copy = (MagicSpell)typeof(MagicSpell).GetMethod("CastingCopy", BindingFlags.NonPublic | BindingFlags.Instance)!
			.Invoke(spell, [cast.Actor.Object, cast.Traits[0], 3, spell.GradeProfile!.Grades.Single(x => x.Grade == 3).Power,
				Difficulty.Automatic, 3])!;
		var effect = (CreateShelterEffect)copy.SpellEffects.Single();
		var token = (IMagicSpellEffectPreparedSelectionRawToken)effect.CapturePreparedSelection(cast.Actor.Object, f.Anchor.Object)!;
		Assert.IsTrue(token.IsCurrent);
		if (change == "definition") f.Template.DefaultInfo = f.Template.DefaultInfo with { RoomDescription = "Edited after quote." };
		else if (change == "registry") f.Templates.Setup(x => x.Get(f.Template.Id)).Returns((IAutobuilderRoom)null!);
		else if (change == "terrain-name") f.Terrain.SetupGet(x => x.Name).Returns("Renamed cave");
		else if (change == "same-id-terrain")
			f.Template.DefaultInfo = f.Template.DefaultInfo with { DefaultTerrain = Mock.Of<ITerrain>(x => x.Id == 21) };
		else
		{
			var before = f.Template.PresentationDefinition;
			var previous = f.Terrain.Object.Atmosphere;
			var replacement = Mock.Of<IGas>(x => x.Id == 9);
			f.World.SetupGet(x => x.Gases).Returns(MagicCastingFixture.Collection(() => new[] { (IGas)previous, replacement }));
			f.Terrain.SetupGet(x => x.Atmosphere).Returns(replacement);
			Assert.AreEqual(before, f.Template.PresentationDefinition, "The XML alone cannot detect changed native physics.");
		}
		Assert.IsFalse(token.IsCurrent);
		Assert.IsFalse(effect.TryConfirmPreparedSelection(cast.Actor.Object, f.Anchor.Object, out _));
		Assert.IsFalse(effect.TryPrepareApplication(cast.Actor.Object, f.Anchor.Object, OpposedOutcomeDegree.None, SpellPower.Standard,
			TimeSpan.Zero, out _, out _));
		Assert.AreEqual(0, cast.Store.Operations.Count); Assert.AreEqual(0, cast.Rolls);
	}

	[DataTestMethod]
	[DataRow("CapacityByGrade")]
	[DataRow("AutobuilderTemplate")]
	[DataRow("MaximumOccupants")]
	public void ShelterSchema_NamespacedKnownField_PreservesAndRefusesInsteadOfLoadingDefault(string field)
	{
		var xml = CreateShelterEffect.Definition(new ShelterPresentationFixture().Configuration);
		var selected = xml.Element(field)!; selected.Name = XName.Get(field, "urn:unsupported");
		var effect = new CreateShelterEffect(xml, Mock.Of<IMagicSpell>());
		Assert.IsNotNull(effect.DefinitionError);
		Assert.IsTrue(XNode.DeepEquals(xml, effect.SaveToXml()));
		Assert.IsTrue(XNode.DeepEquals(xml, effect.Clone().SaveToXml()));
	}

	[DataTestMethod]
	[DataRow("confirmation")]
	[DataRow("native-creation")]
	public void PreparedShelter_AdmissionCallbackChangesValidPresentation_RefusesBeforeTopology(string phase)
	{
		var f = new ShelterPresentationFixture(); var cast = new MagicCastingFixture();
		cast.Actor.SetupGet(x => x.Location).Returns(f.Anchor.Object);
		cast.World.SetupGet(x => x.Rooms).Returns(f.Rooms.Object);
		cast.World.SetupGet(x => x.AutobuilderRooms).Returns(f.Templates.Object);
		var admission = new Mock<ISpellOwnedShelterService>();
		cast.World.SetupGet(x => x.SpellOwnedShelters).Returns(admission.Object);
		var spell = cast.NewSpell(2, "Shelter", CreateShelterEffect.Definition(f.Configuration).ToString());
		Assert.IsTrue(spell.BuildingCommand(cast.Actor.Object, new StringStack("grades fixture")));
		var copy = (MagicSpell)typeof(MagicSpell).GetMethod("CastingCopy", BindingFlags.NonPublic | BindingFlags.Instance)!
			.Invoke(spell, [cast.Actor.Object, cast.Traits[0], 3, spell.GradeProfile!.Grades.Single(x => x.Grade == 3).Power,
				Difficulty.Automatic, 3])!;
		var effect = (CreateShelterEffect)copy.SpellEffects.Single();
		var token = (IMagicSpellEffectPreparedSelectionRawToken)effect.CapturePreparedSelection(cast.Actor.Object, f.Anchor.Object)!;
		var callbacks = 0;
		void Mutate() { callbacks++; f.Template.DefaultInfo = f.Template.DefaultInfo with { RoomDescription = "A different valid shelter." }; }
		Assert.IsTrue(token.IsCurrent);
		if (phase == "confirmation")
		{
			admission.Setup(x => x.AdmissionError(cast.Actor.Object, f.Anchor.Object, It.IsAny<SpellShelterConfiguration>(), 3))
				.Returns((string?)null).Callback(Mutate);
			Assert.IsFalse(effect.TryConfirmPreparedSelection(cast.Actor.Object, f.Anchor.Object, out _));
		}
		else
		{
			var native = new SpellOwnedShelterService(f.World.Object, (_, _, _, _) => { Mutate(); return null; });
			var now = DateTime.UtcNow;
			var metadata = new SpellShelterAnchor(SpellShelterKind.SandShelter, 1, 2, null, 3, 31);
			var origin = new SpellLifecycleOrigin(Guid.NewGuid(), 2, 3, 100, SpellShelterAnchor.Family,
				SpellLifecycleMode.TemporaryCleanup, now, now.AddSeconds(900), metadata.Save());
			var error = Assert.ThrowsException<InvalidOperationException>(() =>
				native.CreatePrepared(cast.Actor.Object, f.Anchor.Object, f.Configuration, origin, () => token.IsCurrent));
			StringAssert.Contains(error.Message, "changed during native creation");
		}
		Assert.AreEqual(1, callbacks); Assert.IsFalse(token.IsCurrent);
		f.World.Verify(x => x.Add(It.IsAny<IRoom>()), Times.Never);
		Assert.AreEqual(0, cast.Store.Operations.Count); Assert.AreEqual(0, cast.Rolls);
	}

	[DataTestMethod]
	[DataRow(SpellShelterKind.SpringHaven, 7.0)]
	[DataRow(SpellShelterKind.BurrowRefuge, 20.0)]
	[DataRow(SpellShelterKind.SandShelter, 7.0)]
	[DataRow(SpellShelterKind.SeveringRefuge, 9.0)]
	public void CanonicalShelterContent_LockedFloorAndTypedGradeCapacity_RoundTripThroughRuntime(SpellShelterKind kind, double minimum)
	{
		var contribution = ArmageddonShelterSpellContent.Contributions.Single(x => x.Kind == kind);
		var terrains = new long[] { 21, 22 };
		var schools = new long[] { 31, 32, 33, 34, 35, 36, 37 };
		var content = contribution.Bind(7, terrains, 73, kind == SpellShelterKind.SpringHaven ? 41 : 0,
			kind == SpellShelterKind.SpringHaven ? 51 : 0, kind == SpellShelterKind.SeveringRefuge ? schools : null);
		terrains[0] = 999; schools[0] = 999;
		var root = content.BuildDefinition(11, 12, 0);
		var effect = new CreateShelterEffect(root.Element("Effects")!.Elements().Single(), Mock.Of<IMagicSpell>());
		Assert.IsNull(effect.DefinitionError); Assert.AreEqual(minimum, content.MinimumEnergy);
		Assert.AreEqual(minimum, ArmageddonShelterStock.MinimumEnergy(kind));
		Assert.AreEqual(minimum, (double)root.Element("ControlledPower")!.Element("Efficiency")!.Attribute("minimum")!);
		var config = effect.Configuration();
		CollectionAssert.AreEqual(new long[] { 21, 22 }, config.AllowedTerrainIds.ToArray());
		Assert.AreEqual(7L, config.AutobuilderTemplateId); Assert.AreEqual(0L, config.TemplateRoomId);
		Assert.AreEqual(300.0, config.SecondsPerGrade); Assert.IsTrue(config.CapacityByGrade);
		foreach (var grade in Enumerable.Range(1, 7)) Assert.AreEqual(grade, config.CapacityForGrade(grade));
		if (kind == SpellShelterKind.SpringHaven) Assert.AreEqual(0.5, config.LitresPerGrade);
		if (kind == SpellShelterKind.SeveringRefuge)
			CollectionAssert.AreEqual(new long[] { 31, 32, 33, 34, 35, 36, 37 }, config.Ward!.SchoolIds.ToArray());
		Assert.IsTrue(XNode.DeepEquals(effect.SaveToXml(), effect.Clone().SaveToXml()));
	}

	private sealed class ShelterPresentationFixture
	{
		public Mock<IFuturemud> World { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<ITerrain> Terrain { get; } = new();
		public Mock<IRoom> Anchor { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IRoomOverlayPackage> Package { get; } = new();
		public Mock<IUneditableAll<IRoom>> Rooms { get; } = new();
		public Mock<IUneditableAll<IAutobuilderRoom>> Templates { get; } = new();
		public AutobuilderRoomByTerrain Template { get; }
		public SpellShelterConfiguration Configuration { get; } = new(SpellShelterKind.SandShelter, 0, [21], 2, 300, 1,
			AutobuilderTemplateId: 7, CapacityByGrade: true);
		public ShelterPresentationFixture()
		{
			var gas = Mock.Of<IGas>(x => x.Id == 8);
			Terrain.SetupGet(x => x.Id).Returns(21); Terrain.SetupGet(x => x.Name).Returns("Cave");
			Terrain.SetupGet(x => x.TerrainLayers).Returns([RoomLayer.GroundLevel]); Terrain.SetupGet(x => x.Atmosphere).Returns(gas);
			World.SetupGet(x => x.Terrains).Returns(MagicCastingFixture.Collection(() => new[] { Terrain.Object }));
			World.SetupGet(x => x.Gases).Returns(MagicCastingFixture.Collection(() => new[] { gas }));
			World.SetupGet(x => x.ForagableProfiles).Returns(Mock.Of<IUneditableRevisableAll<IForagableProfile>>());
			World.SetupGet(x => x.Rooms).Returns(Rooms.Object); Rooms.Setup(x => x.GetEnumerator()).Returns(() => Array.Empty<IRoom>().AsEnumerable().GetEnumerator());
			Package.SetupGet(x => x.Id).Returns(11); Package.SetupGet(x => x.RevisionNumber).Returns(2); Package.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
			var overlay = new Mock<IRoomOverlay>(); overlay.SetupGet(x => x.Id).Returns(31);
			overlay.SetupGet(x => x.Package).Returns(Package.Object); overlay.SetupGet(x => x.Terrain).Returns(Terrain.Object);
			Anchor.SetupGet(x => x.Id).Returns(1); Anchor.SetupGet(x => x.Gameworld).Returns(World.Object); Anchor.SetupGet(x => x.CurrentOverlay).Returns(overlay.Object);
			Rooms.Setup(x => x.Get(2)).Returns(Mock.Of<IRoom>(x => x.Id == 2));
			Template = new(new MudSharp.Models.AutobuilderRoomTemplate { Id = 7, Name = "Owned indoor presentation", TemplateType = "room by terrain",
				Definition = "<Template><ApplyAutobuilderTagsAsFrameworkTags>false</ApplyAutobuilderTagsAsFrameworkTags><Default><Terrain><DefaultTerrain>21</DefaultTerrain><RoomName>A $terrain refuge</RoomName><RoomDescription>A protected $terrain space.</RoomDescription><OutdoorsType>2</OutdoorsType><CellLightMultiplier>1</CellLightMultiplier><ForagableProfile>0</ForagableProfile></Terrain></Default><Terrains/></Template>" }, World.Object);
			Template.DefaultInfo = Template.DefaultInfo with { OutdoorsType = RoomOutdoorsType.IndoorsNoLight };
			Templates.Setup(x => x.Get(7)).Returns(Template); World.SetupGet(x => x.AutobuilderRooms).Returns(Templates.Object);
		}
	}
}
