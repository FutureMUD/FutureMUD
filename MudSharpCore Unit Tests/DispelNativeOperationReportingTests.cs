#nullable enable
using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework.Scheduling;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DispelNativeOperationReportingTests
{
	[DataTestMethod]
	[DataRow(0.0,false)]
	[DataRow(0.000000001,false)]
	[DataRow(10.0,true)]
	public void NativeCountdown_ShorteningObservesExpiryAndNoopDoesNotSampleMastery(double seconds,bool applied)
		=> VerifyNativeShortening(seconds,applied,true);

	[TestMethod]
	public void UnobservableScheduler_ActualMutationRemainsUnprovenAndCannotGrantMastery()
		=> VerifyNativeShortening(10,false,false);

	private static void VerifyNativeShortening(double seconds,bool applied,bool observable)
	{
		var f=new MagicCastingFixture();var clock=new DispelClock(f.Now);using var time=RuntimeClock.Push(clock);
		var scheduler=new EffectScheduler(f.World.Object,clock);
		var opaque=new Mock<IEffectScheduler>();
		opaque.Setup(x=>x.IsScheduled(It.IsAny<IEffect>())).Returns<IEffect>(scheduler.IsScheduled);
		opaque.Setup(x=>x.RemainingDuration(It.IsAny<IEffect>())).Returns<IEffect>(scheduler.RemainingDuration);
		opaque.Setup(x=>x.Reschedule(It.IsAny<IEffect>(),It.IsAny<TimeSpan>())).Callback<IEffect,TimeSpan>(scheduler.Reschedule);
		f.World.SetupGet(x=>x.EffectScheduler).Returns(observable?scheduler:opaque.Object);
		var handler=new EffectHandler(f.Actor.Object);
		f.Actor.Setup(x=>x.EffectsOfType<MagicSpellParent>(It.IsAny<Predicate<MagicSpellParent>>()))
			.Returns<Predicate<MagicSpellParent>>(predicate=>handler.EffectsOfType(predicate));
		f.Actor.Setup(x=>x.EffectsOfType<IDispelMagicProxyEffect>(null)).Returns([]);
		f.Actor.Setup(x=>x.RemoveDuration(It.IsAny<IEffect>(),It.IsAny<TimeSpan>(),true))
			.Callback<IEffect,TimeSpan,bool>((effect,duration,fire)=>handler.RemoveDuration(effect,duration,fire));
		f.Actor.Setup(x=>x.RemoveEffect(It.IsAny<IEffect>(),true)).Callback<IEffect,bool>(handler.RemoveEffect);
		var parent=new MagicSpellParent(f.Actor.Object,f.Spell,f.Actor.Object);handler.AddEffect(parent);
		scheduler.AddSchedule(new EffectSchedule(parent,TimeSpan.FromSeconds(100)));clock.Advance(TimeSpan.FromSeconds(50));
		var originalDeadline=scheduler.NextTriggerUtc;
		var spell=f.NewSpell(1,"Native shorten reporting",$"<Effect type='dispelmagic'><Mode>1</Mode><CasterPolicy>0</CasterPolicy><EffectKey>any</EffectKey><ShortenSeconds>{seconds.ToString(System.Globalization.CultureInfo.InvariantCulture)}</ShortenSeconds></Effect>");
		f.Spells.Remove(f.Spell);Assert.IsTrue(spell.BuildingCommand(f.Actor.Object,new MudSharp.Framework.StringStack("grades fixture")));f.Acquire();
		var before=f.Balances[f.Resources[1]];var result=f.Service.Cast(f.Intent(3,true));
		Assert.AreEqual(MagicCastingStatus.Succeeded,result.Status,result.Message);
		Assert.AreEqual(originalDeadline-(seconds>=10?TimeSpan.FromSeconds(10):TimeSpan.Zero),scheduler.NextTriggerUtc);
		Assert.AreEqual(applied?1:0,f.Samples);Assert.AreEqual(applied?3:2,f.Service.Acquisition(f.Actor.Object,1)!.ControlledGrade);
		Assert.IsTrue(f.Balances[f.Resources[1]]<before);
		Assert.AreEqual(applied,(bool)XElement.Parse(f.Store.Operations[result.OperationId!.Value].Definition).Attribute("applied")!);
	}
	private sealed class DispelClock(DateTime now):TimeProvider
	{
		private DateTime _now=now;
		public override DateTimeOffset GetUtcNow()=>new(_now);
		public void Advance(TimeSpan amount)=>_now+=amount;
	}
}
