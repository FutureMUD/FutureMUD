#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.Health.Surgery;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Knowledge;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RemainsReferenceCompatibilityTests
{
	private sealed class Fixture
	{
		public Mock<IFuturemud> World { get; } = new();
		public Mock<ICharacter> Owner { get; } = new();
		public Mock<IBody> Current { get; } = new();
		public Mock<IBody> Old { get; } = new();
		public Mock<IGameItem> Inventory { get; } = new();
		public Mock<IGameItem> Parent { get; } = new();
		public Mock<ICorpseModel> Model { get; } = new(MockBehavior.Strict);
		public All<IBody> Bodies { get; } = new();

		public Fixture(bool ownerExists = true)
		{
			Owner.SetupGet(x => x.Id).Returns(101);
			Owner.SetupGet(x => x.Body).Returns(Current.Object);
			Owner.SetupGet(x => x.CurrentBody).Returns(Current.Object);
			Owner.SetupGet(x => x.Status).Returns(CharacterStatus.Active);
			Owner.SetupGet(x => x.Bodies).Returns([Current.Object]);
			Current.SetupGet(x => x.Id).Returns(18);
			Old.SetupGet(x => x.Id).Returns(17);
			Inventory.SetupGet(x => x.Weight).Returns(5.0);
			var part = new Mock<IBodypart>();
			part.SetupGet(x => x.Id).Returns(1);
			part.SetupGet(x => x.IdHasBeenRegistered).Returns(true);
			part.SetupGet(x => x.RelativeHitChance).Returns(1.0);
			foreach (var body in new[] { Current, Old })
			{
				body.SetupGet(x => x.IdHasBeenRegistered).Returns(true);
				body.SetupGet(x => x.Actor).Returns(Owner.Object);
				body.SetupGet(x => x.Weight).Returns(60.0);
				body.SetupGet(x => x.ExternalItems).Returns([Inventory.Object]);
				body.SetupGet(x => x.AllItems).Returns([Inventory.Object]);
				body.SetupGet(x => x.Bodyparts).Returns([part.Object]);
			}
			Bodies.Add(Current.Object);
			World.SetupGet(x => x.Bodies).Returns(Bodies);
			World.Setup(x => x.TryGetCharacter(101, true)).Returns(ownerExists ? Owner.Object : null!);
			World.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
			World.SetupGet(x => x.HeartbeatManager).Returns(Mock.Of<IHeartbeatManager>());
			var parts = new All<IBodypart>(); parts.Add(part.Object);
			World.SetupGet(x => x.BodypartPrototypes).Returns(parts);
			Model.SetupGet(x => x.Id).Returns(1);
			Model.SetupGet(x => x.EdiblePercentage).Returns(0.5);
			Model.Setup(x => x.Describe(It.IsAny<DescriptionType>(), It.IsAny<DecayState>(), Owner.Object,
				It.IsAny<IBody>(), It.IsAny<IPerceiver>(), It.IsAny<double>())).Returns("exact corpse");
			Model.Setup(x => x.DescribeSevered(It.IsAny<DescriptionType>(), It.IsAny<DecayState>(), Owner.Object,
				It.IsAny<IBody>(), It.IsAny<IPerceiver>(), It.IsAny<ISeveredBodypart>(), It.IsAny<double>())).Returns("exact part");
			var models = new All<ICorpseModel>(); models.Add(Model.Object);
			World.SetupGet(x => x.CorpseModels).Returns(models);
			Parent.SetupGet(x => x.Gameworld).Returns(World.Object);
		}

		public GameItemComponent Load(bool corpse, long bodyId, BodyRemainsContext context = BodyRemainsContext.SleeveDeath,
			string contents = "")
		{
			var stored = new MudSharp.Models.GameItemComponent
			{
				Definition = corpse
					? $"<Definition><OriginalCharacter>101</OriginalCharacter><OriginalBody>{bodyId}</OriginalBody><RemainsContext>{(int)context}</RemainsContext><Model>1</Model><DecayPoints>0</DecayPoints><DecayState>0</DecayState><TimeOfDeath>2026-10-02T12:00:00Z</TimeOfDeath></Definition>"
					: $"<Definition><OriginalCharacterId>101</OriginalCharacterId><OriginalBodyId>{bodyId}</OriginalBodyId><Model>1</Model><DecayPoints>0</DecayPoints><DecayState>0</DecayState><Parts><Part>1</Part></Parts><Contents>{contents}</Contents><Wounds/></Definition>"
			};
			return corpse
				? new CorpseGameItemComponent(stored,
					(CorpseGameItemComponentProto)RuntimeHelpers.GetUninitializedObject(typeof(CorpseGameItemComponentProto)), Parent.Object)
				: new BodypartGameItemComponent(stored,
					(BodypartGameItemComponentProto)RuntimeHelpers.GetUninitializedObject(typeof(BodypartGameItemComponentProto)), Parent.Object);
		}
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void Load_MissingPositiveBody_PresentsAndReleasesWithoutRedirecting(bool corpse)
	{
		var f = new Fixture();
		var component = f.Load(corpse, 1000017);
		var remains = (IButcherable)component;
		Assert.IsNull(remains.OriginalBody);
		Assert.IsNull(remains.OriginalRace);
		Assert.AreEqual(1000017L, remains.OriginalBodyId);
		foreach (var type in new[] { DescriptionType.Short, DescriptionType.Full })
			StringAssert.Contains(component.Decorate(f.Owner.Object, "", "", type, false, PerceiveIgnoreFlags.None), "unidentifiable");
		Assert.AreEqual(0.0, component.ComponentWeight);
		Assert.AreEqual(0.0, component.ComponentBuoyancy(1.0));
		Assert.AreEqual(0.0, corpse ? ((ICorpse)component).RemainingEdibleWeight : ((ISeveredBodypart)component).RemainingEdibleWeight);
		Assert.IsFalse(remains.Butcher(f.Owner.Object));
		remains.Skin(f.Owner.Object);
		component.Delete();
		f.Owner.Verify(x => x.TryCleanupRetiredBody(It.IsAny<IBody>(), It.IsAny<IGameItem>()), Times.Never);
		f.Inventory.Verify(x => x.Delete(), Times.Never);
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void Load_MissingOwner_DoesNotBindCachedBodyOrCrash(bool corpse)
	{
		var f = new Fixture(false); f.Bodies.Add(f.Old.Object);
		var component = f.Load(corpse, 17);
		Assert.IsNull(((IButcherable)component).OriginalBody);
		Assert.AreEqual(0.0, component.ComponentWeight);
		StringAssert.Contains(component.Decorate(f.Owner.Object, "", "", DescriptionType.Full, false, PerceiveIgnoreFlags.None), "unidentifiable");
		component.Delete();
		f.Inventory.Verify(x => x.Delete(), Times.Never);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Load_NonFinalCorpseCachedCurrentOrForeignBody_RefusesAndPreservesInventory(bool foreign)
	{
		var f = new Fixture();
		var bodyId = 18L;
		if (foreign)
		{
			var other = new Mock<ICharacter>(); other.SetupGet(x => x.Id).Returns(102);
			f.Old.SetupGet(x => x.Actor).Returns(other.Object); f.Bodies.Add(f.Old.Object); bodyId = 17;
		}
		var component = f.Load(true, bodyId);
		Assert.IsNull(((ICorpse)component).OriginalBody);
		Assert.AreEqual(bodyId, ((ICorpse)component).OriginalBodyId);
		component.Delete();
		f.Inventory.Verify(x => x.Delete(), Times.Never);
		f.Owner.Verify(x => x.TryCleanupRetiredBody(It.IsAny<IBody>(), It.IsAny<IGameItem>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void Load_ExactInactiveCachedBody_UsesReferenceWithoutChangingItsId(bool corpse)
	{
		var f = new Fixture(); f.Bodies.Add(f.Old.Object);
		var component = f.Load(corpse, 17);
		Assert.AreSame(f.Old.Object, ((IButcherable)component).OriginalBody);
		Assert.AreEqual(17L, ((IButcherable)component).OriginalBodyId);
		Assert.AreEqual(corpse ? "exact corpse" : "exact part",
			component.Decorate(f.Owner.Object, "", "", DescriptionType.Full, false, PerceiveIgnoreFlags.None));
		Assert.IsTrue(component.ComponentWeight > 0);
		component.Delete();
		f.Owner.Verify(x => x.TryCleanupRetiredBody(f.Old.Object, f.Parent.Object), Times.Once);
		f.Inventory.Verify(x => x.Delete(), corpse ? Times.Once() : Times.Never());
	}

	[TestMethod]
	public void Load_LivingOwnersSeveredPart_AllowsExactCurrentBodyAndPreservesItsInventory()
	{
		var f = new Fixture(); var component = f.Load(false, 18);
		Assert.AreSame(f.Current.Object, ((IButcherable)component).OriginalBody);
		Assert.AreEqual("exact part", component.Decorate(f.Owner.Object, "", "", DescriptionType.Full, false, PerceiveIgnoreFlags.None));
		Assert.AreEqual(30.0, component.ComponentWeight);
		component.Delete(); f.Inventory.Verify(x => x.Delete(), Times.Never);
	}

	[TestMethod]
	public void Load_FinalCorpseExactCurrentBody_PreservesInventoryOfResurrectedOwner()
	{
		var f = new Fixture(); var component = f.Load(true, 18, BodyRemainsContext.FinalCharacterDeath);
		Assert.AreSame(f.Current.Object, ((ICorpse)component).Body);
		Assert.AreEqual("exact corpse", component.Decorate(f.Owner.Object, "", "", DescriptionType.Full, false, PerceiveIgnoreFlags.None));
		component.Delete(); f.Inventory.Verify(x => x.Delete(), Times.Never);
		f.Owner.Verify(x => x.TryCleanupRetiredBody(It.IsAny<IBody>(), It.IsAny<IGameItem>()), Times.Never);
	}

	[TestMethod]
	public void Load_NonFinalZeroBodyId_DoesNotInferSurvivorsBody()
	{
		var f = new Fixture(); var component = f.Load(true, 0);
		Assert.IsNull(((ICorpse)component).Body);
		Assert.AreEqual(0L, ((ICorpse)component).OriginalBodyId);
		component.Delete(); f.Inventory.Verify(x => x.Delete(), Times.Never);
	}

	[TestMethod]
	public void Load_UnresolvedPartWithKnownContents_PreservesTheirWeightAndReleasesThem()
	{
		var f = new Fixture(); var content = new Mock<IGameItem>();
		content.SetupGet(x => x.Weight).Returns(7.0); content.Setup(x => x.Buoyancy(1.0)).Returns(2.0);
		f.World.Setup(x => x.TryGetItem(7, true)).Returns(content.Object);
		var component = f.Load(false, 1000017, contents: "<Item>7</Item>");
		Assert.AreEqual(7.0, component.ComponentWeight); Assert.AreEqual(2.0, component.ComponentBuoyancy(1.0));
		component.Delete(); content.Verify(x => x.Delete(), Times.Once); f.Inventory.Verify(x => x.Delete(), Times.Never);
	}

	[TestMethod]
	public void MissingCorpseBody_ItemIlluminationDamageAndBodyDelegates_AreSafe()
	{
		var f = new Fixture(); var corpse = (CorpseGameItemComponent)f.Load(true, 1000017);
		var item = (GameItem)RuntimeHelpers.GetUninitializedObject(typeof(GameItem));
		typeof(GameItem).GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(item, new List<IGameItemComponent> { corpse });
		typeof(GameItem).GetField("_overridingWoundBehaviourComponent", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(item, corpse);
		typeof(PerceivedItem).GetProperty(nameof(PerceivedItem.EffectHandler))!.SetValue(item, new EffectHandler(item));
		Assert.AreEqual(0.0, item.IlluminationProvided);
		Assert.AreEqual(0, item.PassiveSufferDamage(Mock.Of<IDamage>()).Count());
		Assert.AreEqual(0, corpse.SufferDamage(Mock.Of<IDamage>()).Count());
		Assert.AreEqual(0, corpse.VisibleWounds(f.Owner.Object, WoundExaminationType.Look).Count());
		Assert.AreEqual(0, corpse.Wounds.Count()); Assert.AreEqual(0, corpse.Parts.Count());
		Assert.IsNull(corpse.HealthStrategy);
		Assert.AreEqual(0.0, ((IHaveWeight)corpse).Weight); ((IHaveWeight)corpse).Weight = 70;
		Assert.AreEqual(0.0, ((IHaveHeight)corpse).Height); ((IHaveHeight)corpse).Height = 170;
		Assert.AreEqual(Alignment.Irrelevant, ((IHaveABody)corpse).Handedness);
		corpse.CureAllWounds(); corpse.EvaluateWounds(); corpse.StartHealthTick(); corpse.EndHealthTick();
		f.Current.Verify(x => x.CureAllWounds(), Times.Never);
	}

	private sealed class TestTransplant : OrganTransplantProcedure
	{
		public TestTransplant() : base(null!, null!) { }
		protected override object[] GetProcessedAdditionalArguments(ICharacter surgeon, ICharacter patient, params object[] additionalArguments) => additionalArguments;
	}

	private sealed class TestReplant : ReplantationProcedure
	{
		public TestReplant() : base(null!, null!) { }
		protected override object[] GetProcessedAdditionalArguments(ICharacter surgeon, ICharacter patient, params object[] additionalArguments) => additionalArguments;
	}

	[TestMethod]
	public void Surgery_MissingPartBody_RefusesEligibilityWithReadableReason()
	{
		var f = new Fixture(); var part = (ISeveredBodypart)f.Load(false, 1000017);
		f.Parent.Setup(x => x.GetItemType<ISeveredBodypart>()).Returns(part);
		var transplant = (TestTransplant)RuntimeHelpers.GetUninitializedObject(typeof(TestTransplant));
		var replant = (TestReplant)RuntimeHelpers.GetUninitializedObject(typeof(TestReplant));
		Assert.IsFalse(transplant.CanPerformProcedure(f.Owner.Object, f.Owner.Object, f.Parent.Object, Mock.Of<IBodypart>()));
		StringAssert.Contains(transplant.WhyCannotPerformProcedure(f.Owner.Object, f.Owner.Object, f.Parent.Object, Mock.Of<IBodypart>()), "can no longer be identified");
		Assert.IsFalse(replant.CanPerformProcedure(f.Owner.Object, f.Owner.Object, f.Parent.Object));
		StringAssert.Contains(replant.WhyCannotPerformProcedure(f.Owner.Object, f.Owner.Object, f.Parent.Object), "can no longer be identified");
	}

	[TestMethod]
	public void Resurrection_MissingFinalCorpseBody_DoesNotResurrectOrDeleteRemains()
	{
		var f = new Fixture(); var corpse = (ICorpse)f.Load(true, 1000017, BodyRemainsContext.FinalCharacterDeath);
		f.Parent.Setup(x => x.GetItemType<ICorpse>()).Returns(corpse);
		var effect = (ResurrectionEffect)RuntimeHelpers.GetUninitializedObject(typeof(ResurrectionEffect));
		Assert.IsNull(effect.GetOrApplyEffect(f.Owner.Object, f.Parent.Object, default, default, Mock.Of<IMagicSpellEffectParent>(), Array.Empty<SpellAdditionalParameter>()));
		f.Parent.Verify(x => x.Delete(), Times.Never);
		f.Owner.Verify(x => x.Resurrect(It.IsAny<MudSharp.Construction.ICell>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void SurgeryCommand_UnresolvedFinalCorpse_RefusesBeforeSelectingCurrentBody(bool missingOwner)
	{
		var f = new Fixture(!missingOwner);
		var corpse = (ICorpse)f.Load(true, 1000017, BodyRemainsContext.FinalCharacterDeath);
		f.Parent.Setup(x => x.GetItemType<ICorpse>()).Returns(corpse);
		InvokeCorpseSurgery(f, "original body or owner");
	}

	[TestMethod]
	public void SurgeryCommand_FinalCorpseDifferentExactBody_RefusesToOperateOnSurvivingOwner()
	{
		var f = new Fixture(); f.Bodies.Add(f.Old.Object);
		var corpse = (ICorpse)f.Load(true, 17, BodyRemainsContext.FinalCharacterDeath);
		f.Parent.Setup(x => x.GetItemType<ICorpse>()).Returns(corpse);
		InvokeCorpseSurgery(f, "surviving owner");
	}

	private static void InvokeCorpseSurgery(Fixture f, string refusal)
	{
		var surgeon = new Mock<ICharacter>();
		var knowledge = Mock.Of<IKnowledge>();
		var procedure = new Mock<ISurgicalProcedure>(MockBehavior.Strict);
		procedure.SetupGet(x => x.Id).Returns(1);
		procedure.SetupGet(x => x.Name).Returns("test");
		procedure.SetupGet(x => x.KnowledgeRequired).Returns(knowledge);
		procedure.SetupGet(x => x.RequiresLivingPatient).Returns(false);
		var procedures = new All<ISurgicalProcedure>(); procedures.Add(procedure.Object);
		f.World.SetupGet(x => x.SurgicalProcedures).Returns(procedures);
		surgeon.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		surgeon.SetupGet(x => x.Knowledges).Returns([knowledge]);
		surgeon.Setup(x => x.TargetLocal("corpse")).Returns(f.Parent.Object);
		string? output = null;
		var handler = new Mock<IOutputHandler>();
		handler.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
			.Callback<string, bool, bool>((text, _, _) => output = text).Returns(true);
		surgeon.SetupGet(x => x.OutputHandler).Returns(handler.Object);
		typeof(HealthModule).GetMethod("SurgeryPerform", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [surgeon.Object, new StringStack("test corpse")]);
		Assert.IsNotNull(output); StringAssert.Contains(output, refusal);
		procedure.Verify(x => x.CanPerformProcedure(It.IsAny<ICharacter>(), It.IsAny<ICharacter>(), It.IsAny<object[]>()), Times.Never);
		procedure.Verify(x => x.PerformProcedure(It.IsAny<ICharacter>(), It.IsAny<ICharacter>(), It.IsAny<object[]>()), Times.Never);
		f.Current.VerifyGet(x => x.Prototype, Times.Never);
		f.Inventory.Verify(x => x.Delete(), Times.Never);
	}
}
