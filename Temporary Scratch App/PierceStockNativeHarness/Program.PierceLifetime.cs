#nullable enable

using System.Reflection;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Checks;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static MagicSpell PierceSelectedCopy(NativeRuntime native, MagicSpell source, int grade)
	{
		var power = source.GradeProfile!.Grades.Single(x => x.Grade == grade).Power;
		return (MagicSpell)typeof(MagicSpell).GetMethod("CastingCopy", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(source, [native.Actor, source.CastingTrait, grade, power, Difficulty.Normal, 7])!;
	}

	private static void VerifyPierceLifetimeNative(NativeRuntime native, string connection, MagicSpell stock,
		HarnessClock clock, EffectScheduler scheduler, Func<int, bool, bool, MagicCastingResult> cast,
		Action<MagicSpell, string, string> refuse)
	{
		var actor = native.Actor; var policy = new MagicSpellLifetimePolicy(ArmageddonPierceConcealmentStock.LifetimeGroup, 600, 48);
		MagicSpellParent GroupParent() => actor.EffectsOfType<MagicSpellParent>().Single(x => x.LifetimeGroup == policy.Group);
		var state = new MagicCastingStateStore();
		var casting = native.World.MagicCasting as MagicCastingService ?? throw new InvalidOperationException("Native casting service missing");
		state.Write(acquired: casting.Acquisition(actor, stock.Id)! with { ControlledGrade = 1, NextMasteryUtc = DateTime.UnixEpoch });
		var capped = GroupParent(); var cappedDeadline = scheduler.ScheduledExpiry(capped);
		var noChange = cast(2, true, false);
		Require(scheduler.ScheduledExpiry(GroupParent()) == cappedDeadline && GroupParent().LifetimeState!.Grade == 7 &&
			casting.Acquisition(actor, stock.Id)!.ControlledGrade == 1 &&
			XElement.Parse(state.Operation(noChange.OperationId!.Value)!.Definition).Attribute("masterySample") is null,
			"At-cap paid unchanged deadline/strength sampled mastery or changed expiry");
		state.Write(acquired: casting.Acquisition(actor, stock.Id)! with { ControlledGrade = 7 });
		Console.WriteLine("PIERCE-cap-nochange=passed paid-overreach actual-observed-deadline source-grade7 unchanged no-mastery real-parent-attached");

		MagicSpellParent Seed(MagicSpellLifetimePolicy? authored, bool permanent = false)
		{
			var parent = new MagicSpellParent(actor, stock, actor, SpellPower.Weak)
			{ LifetimeState = authored is null ? null : new(authored, 1) };
			var child = new SpellDetectInvisibleEffect(actor, parent); parent.AddSpellEffect(child); actor.AddEffect(child);
			if (permanent) actor.AddEffect(parent); else actor.AddEffect(parent, TimeSpan.FromSeconds(6000));
			return parent;
		}
		var untagged = Seed(null); var different = Seed(new("native.independent.detection", 600, 48));
		var independentExpiry = scheduler.ScheduledExpiry(different);
		MudSharp.Models.MagicSpell model;
		using (var db = NewIndependentContext(connection))
		{
			model = db.MagicSpells.AsNoTracking().Single(x => x.Id == stock.Id); model.Id = 0;
			model.Name = "Pierce historical group prepared fixture";
			var xml = XElement.Parse(model.Definition); xml.Element("StockIdentity")?.Remove(); model.Definition = xml.ToString();
			db.MagicSpells.Add(model); db.SaveChanges();
		}
		var other = new MagicSpell(model, native.World); ((All<IMagicSpell>)native.World.MagicSpells).Add(other);
		var old = GroupParent(); var oldPower = old.Power; var balance = actor.MagicResourceAmounts[native.Resource];
		var copy = PierceSelectedCopy(native, other, 1);
		copy.ResolveTriggeredSpell(actor, actor, other.GradeProfile!.Grades.Single(x => x.Grade == 1).Power);
		var replacement = GroupParent();
		Require(replacement.Spell.Id == other.Id && replacement.Identity != old.Identity && replacement.Caster == actor &&
			replacement.LifetimeState!.Grade == 7 && replacement.Power == oldPower && scheduler.ScheduledExpiry(replacement) == cappedDeadline &&
			!actor.Effects.Contains(old) && !old.SpellEffects.Any() && !scheduler.IsScheduled(old) &&
			actor.Effects.Contains(untagged) && actor.Effects.Contains(different) && scheduler.ScheduledExpiry(different) == independentExpiry &&
			actor.MagicResourceAmounts[native.Resource] == balance, "Prepared accumulation/group/strength/cleanup/independence or payment mismatch");
		clock.Advance(TimeSpan.FromSeconds(601)); cast(1, false, true);
		Require(GroupParent().Spell.Id == stock.Id && GroupParent().LifetimeState!.Grade == 7 && GroupParent().Power == oldPower &&
			scheduler.ScheduledExpiry(GroupParent()) == RuntimeClock.UtcNow.AddSeconds(28800) &&
			!actor.Effects.Contains(replacement) && !scheduler.IsScheduled(replacement), "Configured low-grade cast failed to accumulate retained source grade7 to cap48");
		actor.RemoveEffect(untagged, true); actor.RemoveEffect(different, true);
		Console.WriteLine("PIERCE-lifetime-routes=passed configured-paid-low-high prepared-selected-grade cross-spell-source-group fresh-UUID-current-provenance max-grade-power old-children-schedules-cleaned independent-untagged-other-group");

		var permanent = Seed(policy, true); refuse(stock, "me", "grouped permanent parent has no provable expiry");
		Require(actor.Effects.Contains(permanent) && permanent.SpellEffects.Count() == 1 && !scheduler.IsScheduled(permanent), "Refusal deleted permanent grouped parent");
		actor.RemoveEffect(permanent, true);
		var conflict = Seed(new(policy.Group, 601, 48)); refuse(stock, "me", "conflicting grouped lifetime metadata");
		Require(actor.Effects.Contains(conflict) && scheduler.IsScheduled(conflict), "Refusal deleted conflicting grouped parent");
		actor.RemoveEffect(conflict, true);
		Console.WriteLine("PIERCE-lifetime-admission=passed permanent-and-conflicting-metadata refused-before-debit-and-operation original-parent-child-schedule-preserved");
	}

	private static (DateTime Expiry, double Balance) VerifyPierceLifetimeRestart(NativeRuntime native,
		MagicSpellParent restored, long capability, HarnessClock clock, EffectScheduler scheduler)
	{
		var actor = native.Actor; var world = native.World;
		var stock = world.MagicSpells.Get(restored.Spell.Id) as MagicSpell ?? throw new InvalidOperationException("Fresh stock missing");
		Require(restored.LifetimeState!.Grade == 7 && restored.Power == stock.GradeProfile!.Grades.Single(x => x.Grade == 7).Power,
			"Fresh source strength/power was not retained");
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(native.Capability)]);
		var terrain = Mock.Get(actor.Location.CurrentOverlay.Terrain);
		terrain.SetupGet(x => x.Type).Returns(ProgVariableTypes.Terrain); terrain.SetupGet(x => x.GetObject).Returns(terrain.Object);
		terrain.SetupGet(x => x.Name).Returns("Desert"); terrain.Setup(x => x.GetProperty("name")).Returns(new TextVariable("Desert"));
		Mock.Get(actor.Location).SetupGet(x => x.Type).Returns(ProgVariableTypes.Location);
		Mock.Get(actor.Location).SetupGet(x => x.GetObject).Returns(actor.Location);
		Mock.Get(actor.Location).Setup(x => x.GetProperty("terrain")).Returns(terrain.Object);
		clock.Advance(TimeSpan.FromSeconds(601)); scheduler.CheckSchedules();
		var units = Math.Min(48, Math.Ceiling(scheduler.RemainingDuration(restored).TotalSeconds / 600) + 5);
		var expected = RuntimeClock.UtcNow.AddSeconds(units * 600);
		actor.RemoveAllEffects<MagicSpellLockout>(null, true); actor.AddResource(native.Resource, 118); FlushCasting(native);
		var casting = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, random: () => 0.1, flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(casting);
		var intent = new MagicCastingIntent(actor, capability, stock.Id, 1, false, "me");
		var quote = casting.Quote(intent); Require(quote.Allowed, "Fresh ordinary cast quote: " + quote.Reason);
		var before = actor.MagicResourceAmounts[native.Resource]; var result = casting.Cast(intent);
		Require(result.Status == MagicCastingStatus.Succeeded && before - actor.MagicResourceAmounts[native.Resource] == quote.Invocation!.Costs.Single().Amount,
			"Fresh ordinary low-grade debit/cast: " + result.Message);
		var current = actor.EffectsOfType<MagicSpellParent>().Single(x => x.LifetimeGroup == ArmageddonPierceConcealmentStock.LifetimeGroup);
		Require(current.LifetimeState!.Grade == 7 && current.Power == restored.Power && current.Identity != restored.Identity &&
			scheduler.ScheduledExpiry(current) == expected && !actor.Effects.Contains(restored) && !scheduler.IsScheduled(restored) &&
			!restored.SpellEffects.Any() && current.SpellEffects.Count() == 1 &&
			(bool)XElement.Parse(new MagicCastingStateStore().Operation(result.OperationId!.Value)!.Definition).Attribute("applied")!,
			"Fresh accumulation lost exact quantised deadline, source grade/power, replacement or application report");
		Console.WriteLine($"PIERCE-restart-accumulation=passed saved-remaining-native-offline-adaptation source-grade7 retained fresh-paid-grade1 exact-quantised-units:{units} cost:{before - actor.MagicResourceAmounts[native.Resource]} old-parent-child-schedule-cleaned");
		return (expected, actor.MagicResourceAmounts[native.Resource]);
	}
}
