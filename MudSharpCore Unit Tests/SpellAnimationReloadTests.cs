#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Character;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpellAnimationReloadTests
{
	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void SavedParent_LoadAndEarlyLogin_DeferRecoveryUntilBootEndsAndCorpseIsRegistered(bool expired)
	{
		using var f = new Fixture(expired);
		f.Login(); f.Tick();
		Assert.AreEqual(0, f.Reasons.Count);
		f.Items.Add(f.Corpse.Object); f.Tick();
		Assert.AreEqual(0, f.Reasons.Count);
		Assert.IsTrue(f.Handler.Effects.Contains(f.Child));
		f.Save.Object.MudBootingMode = false; f.Tick();
		CollectionAssert.AreEqual(new[] { expired ? SpellRetirementReason.Expiry : SpellRetirementReason.Logout }, f.Reasons);
		Assert.AreEqual(0, f.Handler.Effects.Count());
		Assert.IsFalse(f.Scheduler.IsScheduled(f.Child));
		f.VerifyNoCharacterResolution();
	}

	[TestMethod]
	public void SavedParent_TransientRead_DropsRetryAndRecoversOnRegisteredLogin()
	{
		using var f = new Fixture(); f.Save.Object.MudBootingMode = false;
		f.Tick();
		Assert.IsFalse(f.Scheduler.IsScheduled(f.Child));
		Assert.AreEqual(0, f.Reasons.Count);
		f.Items.Add(f.Corpse.Object); f.Login(); f.Tick();
		Assert.AreEqual(1, f.Reasons.Count);
		Assert.AreEqual(0, f.Handler.Effects.Count());
		f.VerifyNoCharacterResolution();
	}

	[TestMethod]
	public void SavedParent_DifferentRegisteredObjectWithSameId_DoesNotRecoverTransientCopy()
	{
		using var f = new Fixture(); f.Save.Object.MudBootingMode = false;
		f.Items.Add(Mock.Of<IGameItem>(x => x.Id == 3)); f.Tick();
		Assert.AreEqual(0, f.Reasons.Count);
		Assert.IsFalse(f.Scheduler.IsScheduled(f.Child));
		Assert.IsTrue(f.Handler.Effects.Contains(f.Child));
	}

	[TestMethod]
	public void SavedParent_InitialProviderLookupThrows_RetainsRecoveryScheduleAndRetries()
	{
		using var f = new Fixture(); f.Items.Add(f.Corpse.Object); f.Save.Object.MudBootingMode = false;
		var calls = 0;
		f.Service.Setup(x => x.TryRetire(9, It.IsAny<SpellRetirementReason>(), out It.Ref<string>.IsAny))
			.Returns(() => ++calls == 1 ? throw new InvalidOperationException("Initial provider lookup failed.") : true);
		f.Tick();
		Assert.IsTrue(f.Handler.Effects.Contains(f.Child));
		Assert.AreEqual(TimeSpan.FromSeconds(60), f.Scheduler.RemainingDuration(f.Child));
		f.Tick(60);
		Assert.AreEqual(2, calls);
		Assert.AreEqual(0, f.Handler.Effects.Count());
		f.VerifyNoCharacterResolution();
	}

	[TestMethod]
	public void SavedParent_ExplicitDispelBeforeRecovery_RemovesParentAndPendingTimer()
	{
		using var f = new Fixture(); f.Items.Add(f.Corpse.Object); f.Save.Object.MudBootingMode = false;
		f.Handler.RemoveEffect(f.Parent, true); f.Tick();
		CollectionAssert.AreEqual(new[] { SpellRetirementReason.Dispel }, f.Reasons);
		Assert.AreEqual(0, f.Handler.Effects.Count());
		Assert.IsFalse(f.Scheduler.IsScheduled(f.Child));
	}

	private sealed class Clock : TimeProvider
	{
		public DateTimeOffset Now = new(2042, 1, 1, 0, 0, 0, TimeSpan.Zero);
		public override DateTimeOffset GetUtcNow() => Now;
	}
	private sealed class Fixture : IDisposable
	{
		private readonly Clock _clock = new();
		private readonly IDisposable _scope;
		private readonly Mock<IFuturemud> _world = new();
		public Mock<IGameItem> Corpse { get; } = new();
		public Mock<ISaveManager> Save { get; } = new();
		public Mock<ISpellOwnedCorpseAnimationService> Service { get; } = new();
		public All<IGameItem> Items { get; } = new();
		public EffectHandler Handler { get; }
		public EffectScheduler Scheduler { get; }
		public MagicSpellParent Parent { get; }
		public SpellAnimatedCorpseEffect Child { get; }
		public List<SpellRetirementReason> Reasons { get; } = [];
		public Fixture(bool expired = false)
		{
			_scope = RuntimeClock.Push(_clock);
			Scheduler = new EffectScheduler(_world.Object, _clock);
			Save.SetupProperty(x => x.MudBootingMode, true);
			_world.SetupGet(x => x.SaveManager).Returns(Save.Object);
			_world.SetupGet(x => x.EffectScheduler).Returns(Scheduler);
			_world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
			_world.SetupGet(x => x.Items).Returns(Items);
			_world.SetupGet(x => x.SpellOwnedCorpseAnimations).Returns(Service.Object);
			_world.Setup(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>())).Throws(new InvalidOperationException("Character loading is forbidden during construction."));
			Service.Setup(x => x.TryRetire(9, It.IsAny<SpellRetirementReason>(), out It.Ref<string>.IsAny))
				.Callback(new Retire((long _, SpellRetirementReason reason, out string diagnostic) => { Reasons.Add(reason); diagnostic = ""; })).Returns(true);
			Corpse.SetupGet(x => x.Id).Returns(3); Corpse.SetupGet(x => x.Gameworld).Returns(_world.Object);
			Corpse.SetupProperty(x => x.EffectsChanged);
			Handler = new EffectHandler(Corpse.Object);
			Corpse.SetupGet(x => x.Effects).Returns(() => Handler.Effects);
			Corpse.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(Handler.AddEffect);
			Corpse.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>(Handler.RemoveEffect);
			var spells = new All<IMagicSpell>(); var spell = Mock.Of<IMagicSpell>(x => x.Id == 11); spells.Add(spell);
			_world.SetupGet(x => x.MagicSpells).Returns(spells);
			var source = new MagicSpellParent(Corpse.Object, spell, null!);
			var child = new SpellAnimatedCorpseEffect(Corpse.Object, source, 1, 2, 3, 7, 8, 9, 10, 0, 11, [],
				CharacterInstancePersistencePolicy.DespawnOnReboot, "", "", "");
			typeof(SpellAnimatedCorpseEffect).GetMethod("BindOwnedLifecycle", BindingFlags.NonPublic | BindingFlags.Instance)!
				.Invoke(child, [Guid.NewGuid(), _clock.Now.UtcDateTime.AddSeconds(expired ? -1 : 180)]);
			source.AddSpellEffect(child);
			SpellAnimatedCorpseEffect.InitialiseEffectType();
			Parent = (MagicSpellParent)typeof(MagicSpellParent).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
				null, [typeof(XElement), typeof(IPerceivable)], null)!.Invoke([source.SaveToXml(new Dictionary<IEffect, TimeSpan>()), Corpse.Object]);
			Handler.AddEffect(Parent); Child = Parent.SpellEffects.OfType<SpellAnimatedCorpseEffect>().Single();
			Assert.AreEqual(child.ExpiryUtc, Child.ExpiryUtc);
			Assert.AreEqual(child.OwnedLifecycleId, Child.OwnedLifecycleId);
			Assert.AreEqual(0, Reasons.Count); VerifyNoCharacterResolution();
		}
		private delegate void Retire(long instance, SpellRetirementReason reason, out string diagnostic);
		public void Login() { foreach (var effect in Handler.Effects.ToArray()) effect.Login(); }
		public void Tick(int seconds = 1) { _clock.Now = _clock.Now.AddSeconds(seconds); Scheduler.CheckSchedules(); }
		public void VerifyNoCharacterResolution() => _world.Verify(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()), Times.Never);
		public void Dispose() => _scope.Dispose();
	}
}
