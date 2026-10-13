#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MudSharp.Magic.SpellEffects;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class S4ExistingFactoryItemProjectionTests
{
	private static SpellProjectionConfiguration Policy() => new(SpellProjectionKind.SandEffigy, 21, 22, 60, 0, 0, RequiresHeldSourceItem: true);
	private sealed class Fixture
	{
		internal readonly MagicCastingFixture F = new();
		internal readonly Mock<ICharacterInstance> Caster = new();
		internal readonly GameItem Source = TestObjectFactory.CreateUninitialized<GameItem>();
		internal readonly GameItemProto Prototype = TestObjectFactory.CreateUninitialized<GameItemProto>();
		internal readonly HoldableGameItemComponent Hold = TestObjectFactory.CreateUninitialized<HoldableGameItemComponent>();
		internal readonly List<IGameItemComponent> Components;
		internal readonly Mock<ISpellOwnedProjectionService> Service = new();
		internal readonly Mock<IUneditableRevisableAll<IGameItemProto>> Catalogue = new();
		internal readonly All<IGameItem> Items = new();
		internal readonly CreateProjectionEffect Effect;
		internal Fixture(int grade = 1)
		{
			Caster.SetupGet(x => x.Id).Returns(100); Caster.SetupGet(x => x.InstanceId).Returns(100);
			Caster.SetupGet(x => x.Gameworld).Returns(F.World.Object); Caster.SetupGet(x => x.Body).Returns(F.Body.Object);
			Caster.SetupGet(x => x.Location).Returns(F.Actor.Object.Location); Caster.SetupGet(x => x.State).Returns(CharacterState.Awake);
			var identity = new Mock<ICharacterIdentity>(); identity.SetupGet(x => x.Id).Returns(100); identity.SetupGet(x => x.PrimaryInstance).Returns(Caster.Object);
			Caster.SetupGet(x => x.Identity).Returns(identity.Object); F.Body.SetupGet(x => x.Actor).Returns(Caster.Object);
			F.World.SetupGet(x => x.Characters).Returns(MagicCastingFixture.Collection<ICharacter>(() => [Caster.Object]));
			F.World.SetupGet(x => x.Bodies).Returns(MagicCastingFixture.Collection<IBody>(() => [F.Body.Object]));
			SetProperty(Prototype, nameof(Prototype.Gameworld), F.World.Object); SetProperty(Prototype, nameof(Prototype.Status), RevisionStatus.Current);
			SetField(typeof(FrameworkItem), Prototype, "_id", 93L);
			var component = TestObjectFactory.CreateUninitialized<HoldableGameItemComponentProto>();
			SetField(typeof(GameItemProto), Prototype, "_components", new List<IGameItemComponentProto> { component }); Prototype.OnLoadProgs = [];
			SetProperty(Source, nameof(Source.Gameworld), F.World.Object); SetProperty(Source, nameof(Source.Prototype), Prototype);
			Source.SetIDFromDatabase(new MudSharp.Models.GameItem { Id = 91 });
			SetField(typeof(LateKeywordedInitialisingItem), Source, "IdInitialised", true);
			SetField(typeof(HoldableGameItemComponent), Hold, "_prototype", component); SetProperty(Hold, nameof(Hold.Parent), Source);
			Hold.HeldBy = F.Body.Object; Components = [Hold]; SetField(typeof(GameItem), Source, "_components", Components);
			SetField(typeof(GameItem), Source, "_wounds", new List<IWound>());
			SetField(typeof(PerceivedItem), Source, "_installedHooks", new List<MudSharp.Events.Hooks.IHook>());
			SetField(typeof(PerceivedItem), Source, "_targetedBy", new List<IPerceivable>());
			SetProperty(Source, nameof(Source.EffectHandler), new EffectHandler(Source));
			F.Body.SetupGet(x => x.Id).Returns(80); F.Body.SetupGet(x => x.HeldItems).Returns(() => [Source]);
			F.Body.SetupGet(x => x.AllItems).Returns(() => [Source]); F.Body.SetupGet(x => x.WieldedItems).Returns([]); F.Body.SetupGet(x => x.WornItems).Returns([]);
			Items.Add(Source); F.World.SetupGet(x => x.Items).Returns(Items);
			Catalogue.Setup(x => x.Get(It.IsAny<long>())).Returns((long id) => id == 93 ? Prototype : null!);
			Catalogue.Setup(x => x.Get(It.IsAny<long>(), It.IsAny<int>())).Returns((long id, int revision) => id == 93 && revision == 0 ? Prototype : null!);
			F.World.SetupGet(x => x.ItemProtos).Returns(Catalogue.Object);
			F.World.SetupGet(x => x.SpellOwnedProjections).Returns(Service.Object);
			Service.Setup(x => x.AdmissionError(Caster.Object, It.IsAny<SpellProjectionConfiguration>(), grade, Source)).Returns((string?)null);
			var planes = new All<IPlane>(); planes.Add(Mock.Of<IPlane>(x => x.Id == 21)); F.World.SetupGet(x => x.Planes).Returns(planes);
			typeof(MagicSpell).GetProperty("InvocationGrade", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(F.Spell, grade);
			Effect = new(CreateProjectionEffect.Definition(Policy()), F.Spell);
		}
	}
	private static void SetProperty(object target, string name, object value) => target.GetType().GetProperty(name)!.SetValue(target, value);
	private static void SetField(Type type, object target, string name, object value) => type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(target, value);
	[DataTestMethod]
	[DataRow(1)] [DataRow(7)]
	public void ItemPolicy_GradeBoundaries_RequiresRealItemAndKeepsBorrowedSourceInColdJournal(int grade)
	{
		var f = new Fixture(grade); var frame = SpellOwnedProjectionService.CaptureSourceFrame(f.Caster.Object, f.Source, f.F.World.Object);
		Assert.IsTrue(frame.IsCurrent); Assert.AreEqual(91L, frame.Provenance.ItemId);
		var anchor = new SpellProjectionAnchor(f.Caster.Object.InstanceId, f.F.Body.Object.Id, 1, 0, null, Policy(), frame.Provenance);
		Assert.AreEqual(anchor, SpellProjectionAnchor.Load(anchor.Save().ToString()));
		Assert.IsTrue(f.Effect.IsCompatibleWithTrigger(Mock.Of<IMagicTrigger>(x => x.TargetTypes == "item")));
		Assert.IsFalse(f.Effect.IsCompatibleWithTrigger(Mock.Of<IMagicTrigger>(x => x.TargetTypes == "character")));
		Assert.IsFalse(f.Effect.TryPrepareApplication(f.Caster.Object, f.Caster.Object, default, SpellPower.ExtremelyWeak, TimeSpan.Zero, out _, out _));
		Assert.IsTrue(f.Effect.TryPrepareApplication(f.Caster.Object, f.Source, default, SpellPower.ExtremelyWeak, TimeSpan.Zero, out var application, out var error), error);
		Assert.IsNotNull(application);
		Assert.AreEqual(1, f.Source.Quantity); Assert.AreSame(f.F.Body.Object, f.Source.InInventoryOf);
		Assert.IsTrue(XNode.DeepEquals(f.Effect.SaveToXml(), f.Effect.Clone().SaveToXml()));
	}
	[DataTestMethod]
	[DataRow("registry")] [DataRow("holder")] [DataRow("body")] [DataRow("prototype")] [DataRow("quantity")] [DataRow("destroyed")] [DataRow("world")]
	public void CapturedBorrowedSource_ExactRawDrift_RefusesBeforeAdoption(string drift)
	{
		var f = new Fixture(); var token = f.Effect.CapturePreparedSelection(f.Caster.Object, f.Source)!;
		switch (drift)
		{
			case "registry": f.Items.Remove(f.Source); f.Items.Add(Mock.Of<IGameItem>(x => x.Id == 91)); break;
			case "holder": f.Hold.HeldBy = Mock.Of<IBody>(); break;
			case "body": f.Caster.SetupGet(x => x.Body).Returns(Mock.Of<IBody>(x => x.Id == 80)); break;
			case "prototype": f.Catalogue.Setup(x => x.Get(93)).Returns(Mock.Of<IGameItemProto>(x => x.Id == 93)); break;
			case "quantity": f.Components.Add(Mock.Of<IStackable>(x => x.Quantity == 2)); break;
			case "destroyed": f.Source.Destroyed = true; break;
			case "world": SetProperty(f.Source, nameof(f.Source.Gameworld), Mock.Of<IFuturemud>()); break;
		}
		Assert.IsFalse(((IMagicSpellEffectPreparedSelectionRawToken)token).IsCurrent);
		Assert.IsFalse(f.Effect.TryReusePreparedSelection(token, f.Caster.Object, f.Source, out _));
		Assert.IsFalse(f.Effect.TryPrepareApplication(f.Caster.Object, f.Source, default, SpellPower.ExtremelyStrong, TimeSpan.Zero, out _, out _));
		f.Service.Verify(x => x.Create(It.IsAny<ICharacter>(), It.IsAny<SpellProjectionConfiguration>(), It.IsAny<SpellLifecycleOrigin>(), It.IsAny<IGameItem>()), Times.Never);
	}
	[TestMethod]
	public void AdmissionCallback_ChangesOriginalHolder_RefusesRawFrameAfterCallback()
	{
		var f = new Fixture(); f.Service.Setup(x => x.AdmissionError(f.Caster.Object, It.IsAny<SpellProjectionConfiguration>(), 1, f.Source))
			.Callback(() => f.Hold.HeldBy = Mock.Of<IBody>()).Returns((string?)null);
		Assert.IsFalse(f.Effect.TryPrepareApplication(f.Caster.Object, f.Source, default, SpellPower.ExtremelyWeak, TimeSpan.Zero, out _, out _));
		f.Service.Verify(x => x.AdmissionError(f.Caster.Object, It.IsAny<SpellProjectionConfiguration>(), 1, f.Source), Times.Once);
		f.Service.Verify(x => x.Create(It.IsAny<ICharacter>(), It.IsAny<SpellProjectionConfiguration>(), It.IsAny<SpellLifecycleOrigin>(), It.IsAny<IGameItem>()), Times.Never);
	}
	[TestMethod]
	public void AdmissionCallback_MovesOriginalBody_RefusesInsteadOfCapturingNewRoom()
	{
		var f = new Fixture(); var originalRoom = f.Caster.Object.Location;
		f.Service.Setup(x => x.AdmissionError(f.Caster.Object, It.IsAny<SpellProjectionConfiguration>(), 1, f.Source))
			.Callback(() => f.Caster.SetupGet(x => x.Location).Returns(Mock.Of<MudSharp.Construction.IRoom>())).Returns((string?)null);
		Assert.IsFalse(f.Effect.TryPrepareApplication(f.Caster.Object, f.Source, default, SpellPower.ExtremelyWeak, TimeSpan.Zero, out _, out _));
		Assert.AreNotSame(originalRoom, f.Caster.Object.Location);
		f.Service.Verify(x => x.AdmissionError(f.Caster.Object, It.IsAny<SpellProjectionConfiguration>(), 1, f.Source), Times.Once);
		f.Service.Verify(x => x.Create(It.IsAny<ICharacter>(), It.IsAny<SpellProjectionConfiguration>(), It.IsAny<SpellLifecycleOrigin>(), It.IsAny<IGameItem>()), Times.Never);
	}
	[TestMethod]
	public void UnregisteredNativeSource_RefusesWithoutSaveFlush()
	{
		var f = new Fixture(); SetField(typeof(LateKeywordedInitialisingItem), f.Source, "IdInitialised", false);
		Assert.IsNotNull(SpellOwnedProjectionService.RawSourceError(f.Caster.Object, f.Source, f.F.World.Object));
		f.F.World.Verify(x => x.SaveManager.Flush(), Times.Never);
	}
	[TestMethod]
	public void ActiveSourceJournal_MalformedUnknownAndExactReuse_HoldsAdmission()
	{
		var source = new SpellProjectionBorrowedSource(91, 93, 0, 80, 100, 1);
		var anchor = new SpellProjectionAnchor(100, 80, 1, 0, null, Policy(), source);
		var saved = anchor.Save().ToString();
		Assert.IsNull(SpellOwnedProjectionService.SourceReuseError(Array.Empty<string>(), 91));
		Assert.IsNull(SpellOwnedProjectionService.SourceReuseError([saved], 92));
		Assert.AreEqual("That exact item already supplies an unfinished projection.", SpellOwnedProjectionService.SourceReuseError([saved], 91));
		Assert.AreEqual("An unknown or malformed active projection journal holds borrowed-source reuse.", SpellOwnedProjectionService.SourceReuseError([saved, "<ProjectionAnchor"], 91));
		Assert.IsNotNull(SpellOwnedProjectionService.SourceReuseError(["<ProjectionAnchor"], 92));
		var unknown = anchor.Save(); unknown.SetAttributeValue("version", 2);
		Assert.IsNotNull(SpellOwnedProjectionService.SourceReuseError([unknown.ToString()], 92));
		unknown = anchor.Save(); unknown.Element("BorrowedSourceItem")!.Name = XName.Get("BorrowedSourceItem", "urn:foreign");
		Assert.IsNotNull(SpellOwnedProjectionService.SourceReuseError([unknown.ToString()], 92));
		Assert.IsNotNull(SpellOwnedProjectionService.SourceReuseError(Enumerable.Repeat(saved, 1001).ToArray(), 92));
	}
	[TestMethod]
	public void PaidPreparedApplication_SourceDriftBeforeCreate_RefusesWithoutReplayOrSourceDeletion()
	{
		var f = new Fixture(); Assert.IsTrue(f.Effect.TryPrepareApplication(f.Caster.Object, f.Source, default, SpellPower.ExtremelyWeak, TimeSpan.Zero, out var application, out _));
		f.Hold.HeldBy = Mock.Of<IBody>();
		Assert.ThrowsException<InvalidOperationException>(() => application!.Create(null!));
		f.Service.Verify(x => x.Create(It.IsAny<ICharacter>(), It.IsAny<SpellProjectionConfiguration>(), It.IsAny<SpellLifecycleOrigin>(), It.IsAny<IGameItem>()), Times.Never);
		Assert.IsFalse(f.Source.Deleted);
	}
	[DataTestMethod]
	[DataRow(1)] [DataRow(7)]
	public void NativeCreationCallback_SourceDrift_RetainsPaidUncertaintyAndRequestsRetirement(int grade)
	{
		var f = new Fixture(grade); Assert.IsTrue(f.Effect.TryPrepareApplication(f.Caster.Object, f.Source, default, SpellPower.ExtremelyWeak, TimeSpan.Zero, out var application, out _));
		var projection = new Mock<ICharacter>(); projection.As<ICharacterInstance>().SetupGet(x => x.InstanceId).Returns(200);
		f.Service.Setup(x => x.Create(f.Caster.Object, It.IsAny<SpellProjectionConfiguration>(), It.IsAny<SpellLifecycleOrigin>(), f.Source))
			.Callback<ICharacter, SpellProjectionConfiguration, SpellLifecycleOrigin, IGameItem>((_, config, origin, source) =>
			{
				Assert.AreSame(f.Source, source); Assert.AreEqual(grade, origin.Grade); Assert.AreEqual(60 * grade, (origin.DeadlineUtc!.Value - origin.CreatedUtc).TotalSeconds);
				Assert.IsTrue(config.RequiresHeldSourceItem); f.Hold.HeldBy = Mock.Of<IBody>();
			}).Returns(projection.As<ICharacterInstance>().Object);
		Assert.ThrowsException<InvalidOperationException>(() => application!.Create(null!));
		f.Service.Verify(x => x.RequestRetirement(projection.Object, SpellRetirementReason.AnchorSevered), Times.Once);
		Assert.IsFalse(f.Source.Deleted); Assert.AreEqual(1, f.Source.Quantity);
	}
	[TestMethod]
	public void PersistedSource_ConflictingCustodyAndRevision_RefusesExactIdentity()
	{
		var source = new SpellProjectionBorrowedSource(91, 93, 0, 80, 100, 1);
		var row = new MudSharp.Models.GameItem { Id = 91, GameItemProtoId = 93, GameItemProtoRevision = 0 };
		var custody = new MudSharp.Models.BodiesGameItems { GameItemId = 91, BodyId = 80 };
		Assert.IsNull(SpellOwnedProjectionService.PersistedSourceError(row, [custody], [], source));
		Assert.IsNotNull(SpellOwnedProjectionService.PersistedSourceError(row, [custody, custody], [], source));
		Assert.IsNotNull(SpellOwnedProjectionService.PersistedSourceError(row, [custody], [5], source));
		row.GameItemProtoRevision = 1; Assert.IsNotNull(SpellOwnedProjectionService.PersistedSourceError(row, [custody], [], source));
	}
	[TestMethod]
	public void SourceJournalAndMetadata_StrictTypedBorrowing_NeverClaimsSourceAsCreated()
	{
		var source = new SpellProjectionBorrowedSource(91, 93, 0, 80, 100, 1);
		var anchor = new SpellProjectionAnchor(100, 80, 1, 0, null, Policy(), source);
		var changed = anchor.Save(); changed.Element("BorrowedSourceItem")!.SetAttributeValue("holderBodyId", 81);
		Assert.ThrowsException<FormatException>(() => SpellProjectionAnchor.Load(changed.ToString()));
		changed = anchor.Save(); changed.Element("BorrowedSourceItem")!.SetAttributeValue("quantity", 2);
		Assert.ThrowsException<FormatException>(() => SpellProjectionAnchor.Load(changed.ToString()));
		changed = anchor.Save(); changed.Add(new XElement(XName.Get("BorrowedSourceItem", "urn:foreign")));
		Assert.ThrowsException<FormatException>(() => SpellProjectionAnchor.Load(changed.ToString()));
		var references = PhysicalReferenceCodecs.Effects("<Effects><OwnedProjection AnchorCharacterId='50' AnchorInstanceId='100' BorrowedSourceItemId='91'/><Narrative>92</Narrative></Effects>", true).ToArray();
		Assert.IsTrue(references.Any(x => x.Kind == PhysicalEntityKind.GameItem && x.Id == 91)); Assert.IsFalse(references.Any(x => x.Id == 92));
		var now = new DateTime(2026, 10, 13, 0, 0, 0, DateTimeKind.Utc);
		var origin = new SpellLifecycleOrigin(Guid.NewGuid(), 1, 1, 50, SpellProjectionAnchor.Family, SpellLifecycleMode.TemporaryCleanup, now, now.AddSeconds(60), anchor.Save().ToString());
		var life = new SpellOwnedLifecycle(origin, [new(SpellOwnedEntityKind.Body, 5), new(SpellOwnedEntityKind.CharacterInstance, 6), new(SpellOwnedEntityKind.GameItem, 7)], SpellLifecycleState.Retiring,
			SpellRetirementReason.Reboot, null, null, now, 1, "");
		Assert.IsTrue(SpellOwnedProjectionService.HasClaims(life));
		Assert.IsFalse(SpellOwnedProjectionService.HasClaims(life with { Entities = [life.Entities[0], life.Entities[1], new(SpellOwnedEntityKind.GameItem, 91)] }));
	}
}

