#nullable enable

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Knowledge;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpellOwnedNpcAdapterTests
{
	[TestMethod]
	public void DeferredStartingKnowledge_SuppressesItsOwnQueueUntilCommittedIdentityAndResumesNormally()
	{
		var saves = new Mock<ISaveManager>();
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.SaveManager).Returns(saves.Object);
		var character = new Mock<ICharacter>(); character.SetupGet(x => x.Gameworld).Returns(world.Object);
		var knowledge = new CharacterKnowledge(character.Object, Mock.Of<IKnowledge>(), "Chargen", deferInitialisation: true);
		Assert.AreEqual(0L, knowledge.Id);
		Assert.IsTrue(knowledge.GetNoSave());
		saves.Verify(x => x.Add(It.IsAny<ISaveable>()), Times.Never);
		saves.Verify(x => x.Flush(), Times.Never);
		knowledge.SetId(73); knowledge.SetNoSave(false); knowledge.TimesTaught = 2;
		Assert.AreEqual(73L, knowledge.Id);
		saves.Verify(x => x.Add(knowledge), Times.Once);
		knowledge.SetNoSave(true);
		Assert.IsFalse(knowledge.Changed);
		saves.Verify(x => x.Abort(knowledge), Times.Once);
		var ordinary = new CharacterKnowledge(character.Object, Mock.Of<IKnowledge>(), "Chargen");
		saves.Verify(x => x.Add(ordinary), Times.Once);
	}

	[TestMethod]
	public void Die_AlreadyDeadCharacter_ReturnsExistingRemainsWithoutWorldOrCallbacks()
	{
		var character = (MudSharp.Character.Character)RuntimeHelpers.GetUninitializedObject(typeof(MudSharp.Character.Character));
		typeof(MudSharp.Character.Character).GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(character, CharacterState.Dead);
		var item = Mock.Of<IGameItem>(); var corpse = new Mock<ICorpse>(); corpse.SetupGet(x => x.Parent).Returns(item);
		character.Corpse = corpse.Object;
		character.OnDeath += _ => Assert.Fail("Death was replayed.");
		Assert.AreSame(item, character.Die());
	}

	[DataTestMethod]
	[DataRow("99", "TemporaryCleanup")]
	[DataRow("invalid", "TemporaryCleanup")]
	[DataRow("1", "invalid")]
	public void Lifecycle_MalformedDefinition_RetainsFailureAcrossClone(string version, string mode)
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var spell = new Mock<IMagicSpell>(); spell.SetupGet(x => x.Gameworld).Returns(world.Object);
		var xml = new XElement("Effect", new XAttribute("type", "createnpc"), new XElement("NPCPrototypeId", 0),
			new XElement("OnLoadProg", 0), new XElement("Lifecycle", new XAttribute("version", version), new XAttribute("mode", mode), new XElement("Family", "guardian")));
		var effect = new CreateNPCEffect(xml, spell.Object);
		Assert.IsNotNull(effect.DefinitionError);
		Assert.IsTrue(XNode.DeepEquals(xml, effect.SaveToXml()));
		Assert.IsNotNull(((CreateNPCEffect)effect.Clone()).DefinitionError);
	}

	[TestMethod]
	public void Legacy_Admission_ReturnsExecutableApplicationAndPreservesDefinition()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var spell = new Mock<IMagicSpell>(); spell.SetupGet(x => x.Gameworld).Returns(world.Object);
		var xml = new XElement("Effect", new XAttribute("type", "createnpc"), new XElement("NPCPrototypeId", 0), new XElement("OnLoadProg", 0));
		var effect = new CreateNPCEffect(xml, spell.Object);
		Assert.IsTrue(effect.TryPrepareApplication(Mock.Of<ICharacter>(), Mock.Of<IPerceivable>(), default,
			default, TimeSpan.Zero, out var application, out var error));
		Assert.IsNotNull(application);
		Assert.IsNull(error);
		Assert.IsTrue(XNode.DeepEquals(xml, effect.SaveToXml()));
	}

	[DataTestMethod]
	[DataRow(0)]
	[DataRow(1001)]
	public void Reconcile_InvalidBound_RefusesBeforeDatabaseRead(int limit)
	{
		var service = new SpellOwnedNpcService(Mock.Of<IFuturemud>());
		Assert.ThrowsException<ArgumentException>(() => service.ReconcilePersistedDeaths(DateTime.UtcNow, limit));
		Assert.ThrowsException<ArgumentException>(() => service.ReconcilePersistedDeaths(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Local)));
	}

	[TestMethod]
	public void CompleteCommittedInitialisation_PublishesRegisteredIdentityOnce()
	{
		var item = new CommittedItem(); var notifications = 0;
		item.IdRegistered += _ => { Assert.IsTrue(item.IdHasBeenRegistered); Assert.AreEqual(73L, item.Id); notifications++; };
		Assert.IsFalse(item.IdHasBeenRegistered);
		item.Complete(73L);
		Assert.AreEqual(1, notifications);
		Assert.ThrowsException<InvalidOperationException>(() => item.Complete(74L));
		Assert.AreEqual(73L, item.Id);
	}

	private sealed class CommittedItem : LateInitialisingItem
	{
		public void Complete(long id) => CompleteCommittedInitialisation(id);
		public override string FrameworkItemType => "CommittedTest";
		public override void Save() => throw new NotSupportedException();
		public override object DatabaseInsert() => throw new NotSupportedException();
		public override void SetIDFromDatabase(object model) => _id = (long)model;
	}
}
