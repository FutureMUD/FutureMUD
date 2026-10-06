#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpellAnimationSchedulerTests
{
	[DataTestMethod]
	[DataRow(30, false)]
	[DataRow(180, false)]
	[DataRow(180, true)]
	[DataRow(300, false)]
	public void Scheduler_DurableAnimationAndOrdinarySibling_KeepSeparateDeadlines(int parentSeconds, bool parentFirst)
	{
		using var f = new Fixture(parentSeconds, parentFirst: parentFirst);
		foreach (var seconds in new[] { parentSeconds - 1, parentSeconds, 179, 180, 300 }.Distinct().Order())
		{
			f.At(seconds);
			Assert.AreEqual(seconds < 180, f.Handler.Effects.Contains(f.Animation), $"Animation at {seconds}s");
			Assert.AreEqual(seconds < parentSeconds, f.Handler.Effects.Contains(f.Glow), $"Sibling at {seconds}s");
			Assert.AreEqual(seconds < Math.Max(180, parentSeconds), f.Handler.Effects.Contains(f.Parent));
		}
		CollectionAssert.AreEqual(new[] { SpellRetirementReason.Expiry }, f.Retired);
		Assert.IsNull(f.Scheduler.NextTriggerUtc);
	}

	[DataTestMethod]
	[DataRow(15)]
	[DataRow(60)]
	public void ParentRemoval_BeforeOrAfterOrdinaryExpiry_DispelsDurableChild(int seconds)
	{
		using var f = new Fixture(30);
		f.At(seconds);
		Assert.IsTrue(f.Handler.Effects.Contains(f.Parent));
		f.Handler.RemoveEffect(f.Parent, true);
		f.At(180);
		CollectionAssert.AreEqual(new[] { SpellRetirementReason.Dispel }, f.Retired);
		Assert.AreEqual(0, f.Handler.Effects.Count());
		Assert.IsNull(f.Scheduler.NextTriggerUtc);
	}

	[TestMethod]
	public void Scheduler_LegacyAnimation_HasNoIndependentDeadlineAndExpiresWithParent()
	{
		using var f = new Fixture(30, durable: false);
		Assert.IsNull(f.Animation.ExpiryUtc);
		Assert.IsFalse(f.Scheduler.IsScheduled(f.Animation));
		// No native identity teardown in this controlled unit fixture; native lifecycle
		// cleanup is qualified separately against real corpse/body/instance rows.
		f.World.Setup(x => x.TryGetCharacter(7, true)).Returns((ICharacter)null!);
		f.At(29); Assert.IsTrue(f.Handler.Effects.Contains(f.Animation));
		f.At(30); Assert.AreEqual(0, f.Handler.Effects.Count());
		Assert.AreEqual(0, f.Retired.Count);
	}

	[TestMethod]
	public void InitialEcho_ThrowsAfterPartialApplication_AnimationStillExpiresOnItsOwnSchedule()
	{
		using var f = new Fixture(30, throwingEcho: true);
		Assert.IsTrue(f.Scheduler.IsScheduled(f.Animation));
		f.At(30); Assert.IsTrue(f.Handler.Effects.Contains(f.Animation));
		f.At(180);
		CollectionAssert.AreEqual(new[] { SpellRetirementReason.Expiry }, f.Retired);
		Assert.AreEqual(0, f.Handler.Effects.Count());
	}

	[TestMethod]
	public void SavedAnimation_AfterOrdinaryExpiry_RetainsAbsoluteDeadlineAndParentMembership()
	{
		using var f = new Fixture(30);
		f.At(30);
		var xml = f.Parent.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		var child = xml.Element("Effect")!.Element("Children")!.Elements().Single();
		Assert.AreEqual(f.Animation.ExpiryUtc, (DateTime?)child.Element("Effect")!.Element("ExpiryUtc"));
		var loaded = (SpellAnimatedCorpseEffect)typeof(SpellAnimatedCorpseEffect).GetConstructor(
			BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(XElement), typeof(IPerceivable)], null)!.Invoke([child, f.Owner.Object]);
		Assert.AreEqual(f.Animation.OwnedLifecycleId, loaded.OwnedLifecycleId);
		Assert.AreEqual(f.Animation.ExpiryUtc, loaded.ExpiryUtc);
		f.At(180);
		Assert.AreEqual(0, f.Parent.SpellEffects.Count());
	}

	private sealed class Clock : TimeProvider
	{
		public DateTimeOffset Now = new(2042, 1, 1, 0, 0, 0, TimeSpan.Zero);
		public override DateTimeOffset GetUtcNow() => Now;
	}

	private sealed class Fixture : IDisposable
	{
		private readonly Clock _clock = new();
		private readonly DateTimeOffset _start;
		private readonly IDisposable _scope;
		public Mock<IFuturemud> World { get; } = new();
		public Mock<IPerceivable> Owner { get; } = new();
		public EffectHandler Handler { get; }
		public EffectScheduler Scheduler { get; }
		public MagicSpellParent Parent { get; }
		public SpellAnimatedCorpseEffect Animation { get; }
		public SpellGlowEffect Glow { get; }
		public List<SpellRetirementReason> Retired { get; } = [];

		public Fixture(int parentSeconds, bool durable = true, bool parentFirst = false, bool throwingEcho = false)
		{
			_start = _clock.Now; _scope = RuntimeClock.Push(_clock);
			Scheduler = new EffectScheduler(World.Object, _clock);
			World.SetupGet(x => x.EffectScheduler).Returns(Scheduler);
			World.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
			Owner.SetupGet(x => x.Gameworld).Returns(World.Object);
			Owner.SetupProperty(x => x.EffectsChanged);
			Handler = new EffectHandler(Owner.Object);
			Owner.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>(Handler.RemoveEffect);
			var actor = new Mock<ICharacter>(); var instance = actor.As<ICharacterInstance>();
			actor.SetupGet(x => x.InstanceId).Returns(9);
			actor.SetupGet(x => x.Identity).Returns(Mock.Of<ICharacterIdentity>(x => x.Instances == new[] { instance.Object }));
			World.Setup(x => x.TryGetCharacter(7, true)).Returns(actor.Object);
			if (throwingEcho)
			{
				actor.SetupGet(x => x.Location).Returns(Mock.Of<ICell>(x => x.Gameworld == World.Object));
				World.Setup(x => x.MediaChannelService.CaptureOutput(It.IsAny<ILocation>(), It.IsAny<IOutput>()))
					.Throws(new InvalidOperationException("Injected room-output callback failure."));
			}
			var service = new Mock<ISpellOwnedCorpseAnimationService>();
			service.Setup(x => x.TryRetire(9, It.IsAny<SpellRetirementReason>(), out It.Ref<string>.IsAny))
				.Callback(new Retire((long _, SpellRetirementReason reason, out string diagnostic) => { Retired.Add(reason); diagnostic = ""; }))
				.Returns(true);
			World.SetupGet(x => x.SpellOwnedCorpseAnimations).Returns(service.Object);
			Parent = new MagicSpellParent(Owner.Object, null!, null!);
			Animation = new SpellAnimatedCorpseEffect(Owner.Object, Parent, 1, 2, 3, 7, 8, 9, 10, 0, 11, [],
				CharacterInstancePersistencePolicy.TemporaryEffectBound, throwingEcho ? "An animation stirs." : "", "", "");
			if (durable) typeof(SpellAnimatedCorpseEffect).GetMethod("BindOwnedLifecycle", BindingFlags.NonPublic | BindingFlags.Instance)!
				.Invoke(Animation, [Guid.NewGuid(), _start.UtcDateTime.AddSeconds(180)]);
			Glow = new SpellGlowEffect(Owner.Object, Parent, null, 10, "glowing", "glowing", Telnet.White);
			Parent.AddSpellEffect(Animation); Parent.AddSpellEffect(Glow);
			if (parentFirst) Handler.AddEffect(Parent, TimeSpan.FromSeconds(parentSeconds));
			if (throwingEcho) Assert.ThrowsException<InvalidOperationException>(() => Handler.AddEffect(Animation));
			else Handler.AddEffect(Animation);
			Handler.AddEffect(Glow);
			if (!parentFirst) Handler.AddEffect(Parent, TimeSpan.FromSeconds(parentSeconds));
		}

		private delegate void Retire(long instance, SpellRetirementReason reason, out string diagnostic);
		public void At(int seconds) { _clock.Now = _start.AddSeconds(seconds); Scheduler.CheckSchedules(); }
		public void Dispose() => _scope.Dispose();
	}
}
