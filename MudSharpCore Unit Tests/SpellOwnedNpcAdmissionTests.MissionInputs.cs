#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.SpellTriggers;
using MudSharp.Magic.Vancian;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

public partial class SpellOwnedNpcAdmissionTests
{
	[TestMethod]
	[DataRow("<Mission version='2' kind='Seek'/>")]
	[DataRow("<Mission version='1' kind='unknown'/>")]
	[DataRow("<Mission xmlns='urn:unsupported' version='1' kind='Seek'/>")]
	[DataRow("<Mission version='1' kind='Seek'><Unexpected/></Mission>")]
	[DataRow("<Mission version='1' kind='Seek' unexpected='true'/>")]
	[DataRow("<Mission version='1' kind='Seek'/><Mission version='1' kind='Courier'/>")]
	public void Cast_MalformedMissionPolicy_PreservesSchemaAndRefusesBeforePayment(string policy)
	{
		var f = new MagicCastingFixture(); var spell = Configure(f, Template(f.World.Object)); PrepareSpatial(f);
		var xml = spell.SpellEffects.Single().SaveToXml();
		xml.Add(XElement.Parse("<Schemas>" + policy + "</Schemas>").Elements());
		var effect = new CreateNPCEffect(xml, spell);
		((List<IMagicSpellEffectTemplate>)typeof(MagicSpell).GetField("_spellEffects", BindingFlags.NonPublic | BindingFlags.Instance)!
			.GetValue(spell)!)[0] = effect;
		Assert.IsNotNull(effect.DefinitionError);
		var saved = effect.SaveToXml();
		var expectedPolicies = xml.Elements().Where(x => x.Name.LocalName == "Mission").ToArray();
		var savedPolicies = saved.Elements().Where(x => x.Name.LocalName == "Mission").ToArray();
		Assert.AreEqual(expectedPolicies.Length, savedPolicies.Length);
		Assert.IsTrue(expectedPolicies.Zip(savedPolicies).All(x => XNode.DeepEquals(x.First, x.Second)));
		var remainingInput = new XElement(xml); var remainingSaved = new XElement(saved);
		remainingInput.Elements().Where(x => x.Name.LocalName == "Mission").Remove();
		remainingSaved.Elements().Where(x => x.Name.LocalName == "Mission").Remove();
		Assert.IsTrue(XNode.DeepEquals(remainingInput, remainingSaved));
		Assert.AreEqual(effect.SaveToXml().ToString(), effect.Clone().SaveToXml().ToString());
		var result = f.Service.Cast(new(f.Actor.Object, f.Earth.Id, spell.Id, 3, true, ""));
		Assert.IsNull(result.OperationId); Assert.AreEqual(MagicCastingStatus.Refused, result.Status);
		Assert.IsTrue(f.Balances.Values.All(x => x == 100)); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.Store.Operations.Count);
	}

	[TestMethod]
	[DataRow("body")]
	[DataRow("instance")]
	[DataRow("state")]
	[DataRow("position")]
	[DataRow("quantity")]
	[DataRow("custody")]
	[DataRow("registry")]
	public void BoundMission_PhysicalInputMutation_InvalidatesCallbackFreeFence(string change)
	{
		var (f, effect, inputs, parcel) = MissionFixture(SpellNpcMissionKind.Courier);
		Assert.IsTrue(effect.TryBindInvocationInputs(f.Actor.Object, f.Actor.Object.Location, Parameters(inputs), out var error), error);
		var fence = MissionFence(effect, f.Actor.Object);
		Assert.IsTrue(fence());
		if (change == "body") f.Actor.SetupGet(x => x.Body).Returns(Mock.Of<IBody>(x => x.Id == 201));
		if (change == "instance") f.Actor.SetupGet(x => x.InstanceId).Returns(101);
		if (change == "state") f.Actor.SetupGet(x => x.State).Returns(CharacterState.Stasis);
		if (change == "position") f.Actor.SetupGet(x => x.RoutePositionMetres).Returns(1);
		if (change == "quantity") parcel.SetupGet(x => x.Quantity).Returns(6);
		if (change == "custody") f.Body.SetupGet(x => x.HeldOrWieldedItems).Returns([]);
		if (change == "registry") f.World.SetupGet(x => x.Items).Returns(new All<IGameItem>());
		Assert.IsFalse(fence());
		Assert.AreEqual(0, f.Store.Operations.Count); Assert.AreEqual(0, f.Rolls);
	}

	[TestMethod]
	[DataRow("body")]
	[DataRow("quantity")]
	public void InitialMissionBind_VisibilityCallbackChangesExactInputs_RefusesWithoutResampling(string change)
	{
		var (f, effect, inputs, parcel) = MissionFixture(SpellNpcMissionKind.Courier);
		var callbacks = 0;
		if (change == "body")
			f.Actor.Setup(x => x.CanSee(f.Actor.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true)
				.Callback(() => { callbacks++; f.Body.SetupGet(x => x.Id).Returns(201); });
		else
			f.Actor.Setup(x => x.CanSee(parcel.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true)
				.Callback(() => { callbacks++; parcel.SetupGet(x => x.Quantity).Returns(6); });
		Assert.IsFalse(effect.TryBindInvocationInputs(f.Actor.Object, f.Actor.Object.Location, Parameters(inputs), out var error));
		StringAssert.Contains(error!, "changed during binding callbacks");
		Assert.AreEqual(1, callbacks); Assert.AreEqual(0, f.Store.Operations.Count); Assert.AreEqual(0, f.Rolls);
		Assert.IsTrue(f.Balances.Values.All(x => x == 100));
	}

	[TestMethod]
	public void PreparedCreation_ConfirmationCallbackChangesParcel_RefusesBeforeNativeCreation()
	{
		var (f, effect, inputs, parcel) = MissionFixture(SpellNpcMissionKind.Courier);
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		context.CharacterInstances.Add(new MudSharp.Models.CharacterInstance
			{ Id = 100, CharacterId = 100, BodyId = 200, IsPrimary = true });
		context.SaveChanges();
		var property = typeof(FMDB).GetProperty(nameof(FMDB.Context), BindingFlags.Public | BindingFlags.Static)!;
		var previous = property.GetValue(null); property.SetValue(null, context);
		try
		{
			using var scope = new FMDB();
			Assert.IsTrue(effect.TryBindInvocationInputs(f.Actor.Object, f.Actor.Object.Location, Parameters(inputs), out var error), error);
			var selection = (IMagicSpellEffectPreparedSelectionRawToken)effect.CapturePreparedSelection(f.Actor.Object, f.Actor.Object.Location)!;
			Assert.IsTrue(effect.TryPrepareApplication(f.Actor.Object, f.Actor.Object.Location, OpposedOutcomeDegree.None,
				((MagicSpell)effect.Spell).GradeProfile!.Grades.Single(x => x.Grade == 3).Power, TimeSpan.Zero,
				out var application, out error), error);
			Assert.IsTrue(selection.IsCurrent);
			var callbacks = 0;
			f.Actor.Setup(x => x.CanSee(parcel.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true)
				.Callback(() => { callbacks++; parcel.SetupGet(x => x.Quantity).Returns(6); });
			var exception = Assert.ThrowsExactly<InvalidOperationException>(() => application!.Create(null!));
			StringAssert.Contains(exception.Message, "changed during group creation");
			Assert.AreEqual(1, callbacks); Assert.IsFalse(selection.IsCurrent);
			Assert.AreEqual(0, context.Characters.Count()); Assert.AreEqual(1, context.CharacterInstances.Count());
			Assert.AreEqual(0, f.Store.Operations.Count); Assert.AreEqual(0, f.Rolls);
		}
		finally { property.SetValue(null, previous); }
	}

	[TestMethod]
	[DataRow("visibility")]
	[DataRow("authority")]
	public void PreparedMission_CallbackChangesPaidPhysicalInput_RefusesBeforeMissionMutation(string callback)
	{
		var (f, effect, inputs, _) = MissionFixture(SpellNpcMissionKind.Seek);
		Assert.IsTrue(effect.TryBindInvocationInputs(f.Actor.Object, f.Actor.Object.Location, Parameters(inputs), out var error), error);
		var fence = MissionFence(effect, f.Actor.Object);
		var npc = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		npc.SetupGet(x => x.Id).Returns(300); npc.SetupGet(x => x.InstanceId).Returns(301);
		npc.SetupGet(x => x.Gameworld).Returns(f.World.Object); npc.SetupGet(x => x.State).Returns(CharacterState.Awake);
		npc.Setup(x => x.EffectsOfType<SpellNpcMissionEffect>()).Returns([]);
		npc.Setup(x => x.ColocatedWith(f.Actor.Object)).Returns(true);
		void Mutate() => f.Actor.SetupGet(x => x.Body).Returns(Mock.Of<IBody>(x => x.Id == 901));
		npc.Setup(x => x.CanSee(f.Actor.Object)).Returns(true).Callback(() => { if (callback == "visibility") Mutate(); });
		var service = new Mock<ISpellOwnedNpcService>();
		service.Setup(x => x.HasCommandAuthority(It.IsAny<ICharacter>(), It.IsAny<ICharacter>(), It.IsAny<Guid>(),
			It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<int>()))
			.Returns(true).Callback(() => { if (callback == "authority") Mutate(); });
		f.World.SetupGet(x => x.SpellOwnedNpcs).Returns(service.Object);
		var now = DateTime.UtcNow;
		var origin = new SpellLifecycleOrigin(Guid.NewGuid(), 2, 3, 100, "mission", SpellLifecycleMode.TemporaryCleanup, now, now.AddMinutes(6), "native-createnpc;");
		var controller = new SpellNpcControlEffect(npc.Object, origin, f.Actor.Object, false);
		Assert.IsFalse(controller.AssignPreparedMission(f.Actor.Object, inputs, fence));
		Assert.IsFalse(fence());
		npc.Verify(x => x.AddEffect(It.IsAny<IEffect>()), Times.Never);
	}

	[TestMethod]
	[DataRow(true, "primary")]
	[DataRow(false, "primary")]
	[DataRow(true, "deadline")]
	[DataRow(false, "deadline")]
	public void HauntAssignment_VisibilityCallbackInvalidatesDurablePrimaryOrDeadline_RefusesBeforeMutation(bool prepared, string change)
	{
		var (f, effect, inputs, _) = MissionFixture(SpellNpcMissionKind.Seek);
		Assert.IsTrue(effect.TryBindInvocationInputs(f.Actor.Object, f.Actor.Object.Location, Parameters(inputs), out var error), error);
		var fence = MissionFence(effect, f.Actor.Object);
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		context.Characters.Add(new MudSharp.Models.Character { Id = 100, IsArchived = false });
		var primary = new MudSharp.Models.CharacterInstance { Id = 100, CharacterId = 100, BodyId = 200, IsPrimary = true };
		context.CharacterInstances.Add(primary); context.SaveChanges();
		Assert.IsTrue(SpellNpcControlEffect.IsDurablyPrimaryTarget(context, f.Actor.Object));
		var clock = new MissionAssignmentClock(DateTimeOffset.Parse("2026-10-12T23:00:00Z"));
		using var time = RuntimeClock.Push(clock);
		var npc = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		npc.SetupGet(x => x.Id).Returns(300); npc.SetupGet(x => x.InstanceId).Returns(301);
		npc.SetupGet(x => x.Gameworld).Returns(f.World.Object); npc.SetupGet(x => x.State).Returns(CharacterState.Awake);
		npc.Setup(x => x.EffectsOfType<SpellNpcMissionEffect>()).Returns([]);
		npc.Setup(x => x.ColocatedWith(f.Actor.Object)).Returns(true);
		var callbacks = 0;
		npc.Setup(x => x.CanSee(f.Actor.Object)).Returns(true).Callback(() =>
		{
			callbacks++;
			if (change == "primary") { primary.IsPrimary = false; context.SaveChanges(); }
			else clock.Advance(TimeSpan.FromSeconds(11));
		});
		var service = new Mock<ISpellOwnedNpcService>();
		service.Setup(x => x.HasCommandAuthority(It.IsAny<ICharacter>(), It.IsAny<ICharacter>(), It.IsAny<Guid>(),
			It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<int>())).Returns(true);
		f.World.SetupGet(x => x.SpellOwnedNpcs).Returns(service.Object);
		var now = RuntimeClock.UtcNow;
		var origin = new SpellLifecycleOrigin(Guid.NewGuid(), 2, 3, 100, "mission", SpellLifecycleMode.TemporaryCleanup,
			now, now.AddSeconds(10), "native-createnpc;");
		var reads = 0;
		var controller = new SpellNpcControlEffect(npc.Object, origin, f.Actor.Object, false,
			target => { reads++; return SpellNpcControlEffect.IsDurablyPrimaryTarget(context, target); });
		var assigned = prepared
			? controller.AssignPreparedMission(f.Actor.Object, new(SpellNpcMissionKind.Haunt, f.Actor.Object), fence)
			: controller.AssignHaunting(f.Actor.Object, f.Actor.Object);
		Assert.IsFalse(assigned); Assert.AreEqual(1, callbacks); Assert.AreEqual(2, reads);
		Assert.IsTrue(fence(), "The physical frame did not change: the final durable/deadline guard must refuse.");
		npc.Verify(x => x.AddEffect(It.IsAny<IEffect>()), Times.Never);
		npc.Verify(x => x.Save(), Times.Never);
		Assert.AreEqual(0, f.Store.Operations.Count); Assert.AreEqual(0, f.Rolls);
	}

	private sealed class MissionAssignmentClock(DateTimeOffset now) : TimeProvider
	{
		private DateTimeOffset _now = now;
		public override DateTimeOffset GetUtcNow() => _now;
		public void Advance(TimeSpan duration) => _now += duration;
	}

	[TestMethod]
	[DataRow(SpellNpcMissionKind.Seek, "target")]
	[DataRow(SpellNpcMissionKind.Haunt, "target")]
	[DataRow(SpellNpcMissionKind.Courier, "parcel to target")]
	public void RoomMissionTrigger_CompleteNativeInputs_ResolveRoomAndExactPayload(SpellNpcMissionKind kind, string text)
	{
		var (f, effect, inputs, _) = MissionFixture(kind);
		var spell = (MagicSpell)effect.Spell;
		f.Actor.Setup(x => x.TargetActorOrCorpse("target", It.IsAny<PerceiveIgnoreFlags>())).Returns(f.Actor.Object);
		Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("trigger new roommission " + kind)));
		var resolution = SpellTargetCapture.Resolve(f.Actor.Object, spell, SpellPower.ExtremelyWeak, new StringStack(text), true);
		Assert.IsNotNull(resolution); Assert.AreSame(f.Actor.Object.Location, resolution.Target);
		Assert.AreEqual(1, resolution.Parameters.Length);
		var payload = (SpellNpcMissionInputs)resolution.Parameters[0].Item!;
		Assert.AreEqual(kind, payload.Kind); Assert.AreSame(inputs.ExactTarget, payload.ExactTarget); Assert.AreSame(inputs.ExactParcel, payload.ExactParcel);
		Assert.AreEqual(0, f.Store.Operations.Count); Assert.AreEqual(0, f.Rolls);
		Assert.IsTrue(ScrollSpellCompatibility.Errors(spell).Any(x => x.Contains("dedicated adapter", StringComparison.Ordinal)));
	}

	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void RoomMissionCourier_UsesHandSelectorAndRefusesParcelOutsideHands(bool inHands)
	{
		var (f, effect, inputs, parcel) = MissionFixture(SpellNpcMissionKind.Courier);
		var spell = (MagicSpell)effect.Spell;
		f.Actor.Setup(x => x.TargetActorOrCorpse("target", It.IsAny<PerceiveIgnoreFlags>())).Returns(f.Actor.Object);
		Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("trigger new roommission courier")));
		// The room selector is deliberately a different, visible item with the same keyword.
		f.Actor.Setup(x => x.TargetLocalItem("parcel")).Returns(Mock.Of<IGameItem>(x => x.Id == 501));
		f.Body.SetupGet(x => x.HeldOrWieldedItems).Returns(inHands ? [parcel.Object] : []);
		var resolution = SpellTargetCapture.Resolve(f.Actor.Object, spell, SpellPower.ExtremelyWeak,
			new StringStack("parcel to target"), true);
		if (inHands)
		{
			Assert.IsNotNull(resolution);
			Assert.AreSame(inputs.ExactParcel, ((SpellNpcMissionInputs)resolution.Parameters[0].Item!).ExactParcel);
		}
		else Assert.IsNull(resolution);
		f.Actor.Verify(x => x.TargetHeldItem("parcel"), Times.Once);
		f.Actor.Verify(x => x.TargetLocalItem(It.IsAny<string>()), Times.Never);
		Assert.AreEqual(0, f.Store.Operations.Count);
	}

	[TestMethod]
	public void Quote_ExactMissionInputsBindBeforePreparedSelectionWithoutMutation()
	{
		var (f, _, _, _) = MissionFixture(SpellNpcMissionKind.Courier);
		var spell = (MagicSpell)f.Spells.Single(x => x.Id == 2);
		f.Actor.Setup(x => x.TargetActorOrCorpse("target", It.IsAny<PerceiveIgnoreFlags>())).Returns(f.Actor.Object);
		Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("trigger new roommission courier")));
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		context.CharacterInstances.Add(new MudSharp.Models.CharacterInstance { Id = 100, CharacterId = 100, BodyId = 200, IsPrimary = true });
		context.SaveChanges();
		var property = typeof(FMDB).GetProperty(nameof(FMDB.Context), BindingFlags.Public | BindingFlags.Static)!;
		var previous = property.GetValue(null); property.SetValue(null, context);
		try
		{
			using var scope = new FMDB();
			var quote = f.Service.Quote(new(f.Actor.Object, f.Earth.Id, spell.Id, 3, true, "parcel to target"));
			Assert.IsTrue(quote.Allowed, quote.Reason);
			Assert.AreEqual(0, f.Store.Operations.Count); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.Flushes);
			Assert.IsTrue(f.Balances.Values.All(x => x == 100)); Assert.AreEqual(0, context.Characters.Count());
		}
		finally { property.SetValue(null, previous); }
	}

	[TestMethod]
	[DataRow(SpellNpcMissionKind.Seek, "")]
	[DataRow(SpellNpcMissionKind.Haunt, "")]
	[DataRow(SpellNpcMissionKind.Courier, "")]
	[DataRow(SpellNpcMissionKind.Courier, "parcel")]
	[DataRow(SpellNpcMissionKind.Courier, "parcel target")]
	[DataRow(SpellNpcMissionKind.Courier, "parcel to")]
	[DataRow(SpellNpcMissionKind.Courier, "parcel to missing")]
	public void RoomMissionTrigger_IncompleteInputs_RefuseWithoutOperation(SpellNpcMissionKind kind, string text)
	{
		var (f, effect, _, _) = MissionFixture(kind);
		var spell = (MagicSpell)effect.Spell;
		Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("trigger new roommission " + kind)));
		Assert.IsNull(SpellTargetCapture.Resolve(f.Actor.Object, spell, SpellPower.ExtremelyWeak, new StringStack(text), true));
		Assert.AreEqual(0, f.Store.Operations.Count); Assert.AreEqual(0, f.Rolls);
		Assert.IsTrue(f.Balances.Values.All(x => x == 100));
	}

	private static SpellAdditionalParameter[] Parameters(SpellNpcMissionInputs inputs) =>
		[new() { ParameterName = SpellNpcMissionInputs.ParameterName, Item = inputs }];
	private static Func<bool> MissionFence(CreateNPCEffect effect, ICharacter caster) => (Func<bool>)typeof(CreateNPCEffect)
		.GetMethod("CaptureMissionInputFence", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(effect, [caster])!;

	private static (MagicCastingFixture Fixture, CreateNPCEffect Effect, SpellNpcMissionInputs Inputs, Mock<IGameItem> Parcel)
		MissionFixture(SpellNpcMissionKind kind)
	{
		var f = new MagicCastingFixture(); var template = Template(f.World.Object);
		var purpose = kind switch { SpellNpcMissionKind.Seek => "owned_seek", SpellNpcMissionKind.Courier => "owned_courier", _ => "owned_haunt" };
		var aiModel = new MudSharp.Models.ArtificialIntelligence { Id = 11, Name = "Mission fixture", Type = "SpellCompanion",
			Definition = $"<Definition><Mode>companion</Mode><Purpose>{purpose}</Purpose><Permitted><Order>follow</Order><Order>stop</Order><Order>guard</Order><Order>attack</Order></Permitted></Definition>" };
		var constructor = typeof(SpellCompanionAI).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
			null, [typeof(MudSharp.Models.ArtificialIntelligence), typeof(IFuturemud)], null)!;
		template.ArtificialIntelligences.Add((SpellCompanionAI)constructor.Invoke([aiModel, f.World.Object]));
		var spell = Configure(f, template); PrepareSpatial(f);
		SetProperty(spell, nameof(MagicSpell.TargetNullEmote), "No valid mission target.");
		f.Actor.Setup(x => x.TargetActorOrCorpse(It.IsAny<string>(), It.IsAny<PerceiveIgnoreFlags>())).Returns((ICharacter)null!);
		var xml = spell.SpellEffects.Single().SaveToXml(); xml.Add(new XElement("Mission", new XAttribute("version", 1), new XAttribute("kind", kind)));
		((List<IMagicSpellEffectTemplate>)typeof(MagicSpell).GetField("_spellEffects", BindingFlags.NonPublic | BindingFlags.Instance)!
			.GetValue(spell)!)[0] = new CreateNPCEffect(xml, spell);
		var effect = (CreateNPCEffect)CopyAtGrade(f, spell, 3).SpellEffects.Single();
		f.Actor.Setup(x => x.CanSee(f.Actor.Object)).Returns(true); f.Actor.Setup(x => x.ColocatedWith(f.Actor.Object)).Returns(true);
		var parcel = new Mock<IGameItem>();
		parcel.SetupGet(x => x.Id).Returns(500); parcel.SetupGet(x => x.Gameworld).Returns(f.World.Object); parcel.SetupGet(x => x.Quantity).Returns(5);
		f.World.SetupGet(x => x.Items).Returns(MagicCastingFixture.Collection(() => new[] { parcel.Object }));
		f.Body.SetupGet(x => x.HeldOrWieldedItems).Returns([parcel.Object]);
		f.Actor.Setup(x => x.TargetHeldItem("parcel")).Returns(parcel.Object);
		f.Actor.Setup(x => x.CanSee(parcel.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		return (f, effect, new(kind, f.Actor.Object, kind == SpellNpcMissionKind.Courier ? parcel.Object : null), parcel);
	}
}
