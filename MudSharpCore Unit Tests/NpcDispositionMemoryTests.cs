#nullable enable

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Functions.Effects;
using MudSharp.FutureProg.Variables;
using MudSharp.NPC.AI;
using MudSharp;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NpcDispositionMemoryTests
{
	[TestMethod]
	public void SilentNativeRemovalAlsoReleasesSessionSubscriptions()
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.EffectScheduler).Returns(Mock.Of<IEffectScheduler>());
		var owner = new Mock<ICharacter>(); var subject = new Mock<ICharacter>();
		owner.SetupGet(x => x.Gameworld).Returns(world.Object);
		var handler = new EffectHandler(owner.Object);
		owner.SetupGet(x => x.Effects).Returns(() => handler.Effects);
		var memory = new NpcDispositionMemory(owner.Object, subject.Object, NpcDisposition.Like);
		handler.AddEffect(memory);
		handler.RemoveEffect(memory);
		Assert.IsFalse(NpcDispositionMemory.Has(owner.Object, subject.Object, NpcDisposition.Like));
		owner.Raise(x => x.OnQuit += null, owner.Object);
		subject.Raise(x => x.OnQuit += null, subject.Object);
		owner.Verify(x => x.RemoveEffect(It.IsAny<IEffect>(), true), Times.Never);
	}

	[TestMethod]
	[DataRow(NpcDisposition.Fear)]
	[DataRow(NpcDisposition.Like)]
	public void DelayedEngagementRechecksOptInDisposition(NpcDisposition disposition)
	{
		var owner = new Mock<ICharacter>();
		var subject = Mock.Of<ICharacter>();
		var memory = new NpcDispositionMemory(owner.Object, subject, disposition);
		owner.SetupGet(x => x.Effects).Returns(new List<IEffect> { memory });
		PredatorAIHelpers.EngageTarget(owner.Object, subject, "", respectDispositionMemory: true);
		owner.Verify(x => x.Engage(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
		memory.RemovalEffect();
	}

	[TestMethod]
	public void ProgOperationsDoNotStackOrRemoveOtherReactions()
	{
		var owner = new Mock<ICharacter>();
		var subject = new Mock<ICharacter>();
		var subjectState = CharacterState.Awake;
		subject.SetupGet(x => x.State).Returns(() => subjectState);
		owner.SetupGet(x => x.GetObject).Returns(owner.Object);
		subject.SetupGet(x => x.GetObject).Returns(subject.Object);
		var effects = new List<IEffect>();
		owner.SetupGet(x => x.Effects).Returns(effects);
		owner.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effects.Add);
		owner.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), true)).Callback<IEffect, bool>((effect, _) =>
		{
			effects.Remove(effect);
			effect.RemovalEffect();
		});
		bool Run(string operation, string reaction)
		{
			var function = new NpcMemoryFunction([new ConstantFunction(owner.Object),
				new ConstantFunction(subject.Object), new ConstantFunction(new TextVariable(reaction))], operation);
			Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
			return (bool)function.Result.GetObject;
		}
		Assert.IsTrue(Run("remember", "hate"));
		Assert.IsTrue(Run("remember", "Hate"));
		Assert.AreEqual(1, effects.Count);
		Assert.IsTrue(Run("remember", "like"));
		Assert.IsFalse(Run("remember", "invalid"));
		Assert.IsTrue(Run("forget", "hate"));
		Assert.IsFalse(Run("remembers", "hate"));
		Assert.IsTrue(Run("remembers", "like"));
		Assert.IsFalse(Run("forget", "hate"));
		Assert.IsTrue(Run("forget", "like"));
		// Death callbacks can run after OnDeath but before the state setter transitions to Dead.
		subject.Raise(x => x.OnDeath += null, subject.Object);
		Assert.IsTrue(Run("remember", "fear"));
		subjectState = CharacterState.Dead;
		subject.Raise(x => x.OnStateChanged += null, subject.Object);
		Assert.IsFalse(Run("remembers", "fear"));
		Assert.AreEqual(0, effects.Count);
	}

	[TestMethod]
	[DataRow(true, "quit")]
	[DataRow(false, "quit")]
	[DataRow(true, "death")]
	[DataRow(false, "death")]
	[DataRow(true, "delete")]
	[DataRow(false, "delete")]
	public void EitherParticipantEndingSessionRemovesMemoryAndReleasesSubscriptions(bool ownerEnds, string reason)
	{
		var owner = new Mock<ICharacter>();
		var subject = new Mock<ICharacter>();
		var effects = new List<IEffect>();
		owner.SetupGet(x => x.Effects).Returns(effects);
		var memory = new NpcDispositionMemory(owner.Object, subject.Object, NpcDisposition.Hate);
		effects.Add(memory);
		owner.Setup(x => x.RemoveEffect(memory, true)).Callback(() =>
		{
			effects.Remove(memory);
			memory.RemovalEffect();
		});
		Assert.IsTrue(NpcDispositionMemory.Has(owner.Object, subject.Object, NpcDisposition.Hate));
		Assert.IsFalse(memory.SavingEffect);
		var participant = ownerEnds ? owner : subject;
		switch (reason)
		{
			case "quit": participant.Raise(x => x.OnQuit += null, participant.Object); break;
			case "death": participant.Raise(x => x.OnDeath += null, participant.Object); break;
			case "delete": participant.Raise(x => x.OnDeleted += null, participant.Object); break;
		}
		Assert.IsFalse(NpcDispositionMemory.Has(owner.Object, subject.Object, NpcDisposition.Hate));
		owner.Raise(x => x.OnQuit += null, owner.Object);
		subject.Raise(x => x.OnQuit += null, subject.Object);
		owner.Verify(x => x.RemoveEffect(memory, true), Times.Once);
	}

	[TestMethod]
	public void ReactionsAreIndependentAndReferToTheLiveInstance()
	{
		var owner = new Mock<ICharacter>();
		var subject = Mock.Of<ICharacter>(x => x.Id == 10);
		var replacement = Mock.Of<ICharacter>(x => x.Id == 10);
		var hate = new NpcDispositionMemory(owner.Object, subject, NpcDisposition.Hate);
		var like = new NpcDispositionMemory(owner.Object, subject, NpcDisposition.Like);
		owner.SetupGet(x => x.Effects).Returns(new List<IEffect> { hate, like });
		Assert.IsTrue(NpcDispositionMemory.Has(owner.Object, subject, NpcDisposition.Hate));
		Assert.IsTrue(NpcDispositionMemory.Has(owner.Object, subject, NpcDisposition.Like));
		Assert.IsFalse(NpcDispositionMemory.Has(owner.Object, subject, NpcDisposition.Fear));
		Assert.IsFalse(NpcDispositionMemory.Has(owner.Object, replacement, NpcDisposition.Hate));
		hate.RemovalEffect();
		Assert.IsFalse(NpcDispositionMemory.Has(owner.Object, subject, NpcDisposition.Hate));
		Assert.IsTrue(NpcDispositionMemory.Has(owner.Object, subject, NpcDisposition.Like));
		like.RemovalEffect();
	}
}
