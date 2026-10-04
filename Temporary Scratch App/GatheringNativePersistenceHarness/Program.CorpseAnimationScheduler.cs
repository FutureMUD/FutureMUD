#nullable enable
using System.Reflection;
using System.Xml.Linq;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void RunCorpseAnimationSchedulerChecks(TestDatabase database, RetirementHost host, HarnessClock clock,
		EffectScheduler scheduler, MagicCastingService casting, SkillLevelBasedMagicCapability capability,
		MagicSpell spell, ICharacter owner, IGameItem corpse, IGameItem foreign)
	{
		var native = host.Native; var world = native.World; var caster = native.Actor;
		// The controlled world needs a light-description catalogue for the real glow
		// template. Its native effect, parent membership and timers remain unmodified.
		native.WorldMock.Setup(x => x.LightModel.GetIlluminationDescription(It.IsAny<double>())).Returns("bright");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			foreach (var seconds in new[] { 30, 180, 300 })
			{
				var row = new MudSharp.Models.TraitExpression { Name = $"ARM03D1P2 Duration {seconds}", Expression = seconds.ToString() };
				db.TraitExpressions.Add(row); db.SaveChanges();
				((All<ITraitExpression>)world.TraitExpressions).Add(new TraitExpression(row, world));
			}
		}
		Require(spell.BuildingCommand(caster, new StringStack("effect add glow")), "Scheduler fixture glow sibling refused.");
		foreach (var (parentSeconds, dispelAt) in new[] { (30, 0), (180, 0), (300, 0), (30, 60), (300, 15) })
		{
			clock.Advance(TimeSpan.FromMinutes(5)); scheduler.CheckSchedules();
			caster.AddResource(native.Resource, 100); world.SaveManager.Flush(); FlushCasting(native);
			Require(spell.BuildingCommand(caster, new StringStack($"duration ARM03D1P2 Duration {parentSeconds}")), "Scheduler fixture duration refused.");
			var start = RuntimeClock.UtcNow;
			var result = casting.Cast(new(caster, capability.Id, spell.Id, 3, false, "corpse"));
			Require(result.Status == MagicCastingStatus.Succeeded, "Scheduler matrix paid cast failed: " + result.Message);
			var actor = owner.Identity.Instances.OfType<ScriptedAiCharacterInstance>().Single();
			var lifecycle = ReadCorpseAnimationLife(database, actor.InstanceId);
			var animation = corpse.EffectsOfType<SpellAnimatedCorpseEffect>().Single();
			var glow = corpse.EffectsOfType<SpellGlowEffect>().Single();
			var parent = corpse.EffectsOfType<MagicSpellParent>().Single();
			Require(ReferenceEquals(animation.ParentEffect, parent) && ReferenceEquals(glow.ParentEffect, parent) &&
				animation.ExpiryUtc == start.AddSeconds(180) && lifecycle.Origin.DeadlineUtc == animation.ExpiryUtc &&
				scheduler.RemainingDuration(animation) == TimeSpan.FromSeconds(180) &&
				scheduler.RemainingDuration(parent) == TimeSpan.FromSeconds(parentSeconds), "Paid cast did not schedule independent native deadlines.");

			void At(int seconds)
			{
				clock.Advance(start.AddSeconds(seconds) - RuntimeClock.UtcNow);
				scheduler.CheckSchedules();
			}
			void Active(int seconds)
			{
				At(seconds);
				Require(owner.Identity.Instances.Any(x => x.InstanceId == actor.InstanceId) && corpse.Effects.Contains(animation) &&
					corpse.Effects.Contains(parent) && !corpse.TrueLocations.Any() &&
					ReadCorpseAnimationLife(database, actor.InstanceId).State == SpellLifecycleState.Active,
					$"Parent {parentSeconds}s collapsed durable animation at {seconds}s.");
				Require(corpse.Effects.Contains(glow) == (seconds < parentSeconds), "Ordinary glow sibling exceeded its duration.");
			}

			if (dispelAt == 0)
			{
				foreach (var seconds in new[] { Math.Min(parentSeconds, 180) - 1, Math.Min(parentSeconds, 179), 179 }.Distinct().Order()) Active(seconds);
				At(180);
				AssertCorpseAnimationRestored(database, host, lifecycle.Origin.Id, actor.InstanceId, corpse.Id, owner.Id, owner.Body.Id, foreign.Id);
				Require(ReadCorpseAnimationLife(database, actor.InstanceId).Reason == SpellRetirementReason.Expiry &&
					corpse.Effects.Contains(glow) == (parentSeconds > 180) && corpse.Effects.Contains(parent) == (parentSeconds > 180) &&
					!scheduler.IsScheduled(animation), "Deadline expiry changed a longer-lived sibling or retained its child timer.");
				At(Math.Max(180, parentSeconds));
				Require(!corpse.Effects.Contains(parent) && !corpse.Effects.Contains(glow), "Parent or sibling survived both deadlines.");
				Console.WriteLine($"ARM03D1P2-scheduler-{parentSeconds}=passed actual-paid-grade3-cast native-EffectScheduler 180-second-durable-deadline ordinary-glow-sibling-{parentSeconds}-seconds exact-corpse-body-foreign-gear-restored expiry-reason no-worker-reconciliation");
			}
			else
			{
				if (parentSeconds < dispelAt) { Active(parentSeconds - 1); Active(parentSeconds); }
				Active(dispelAt - 1); Active(dispelAt);
				var dispel = (DispelMagicEffect)typeof(DispelMagicEffect).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
					[typeof(XElement), typeof(IMagicSpell)], null)!.Invoke([new XElement("Effect", new XElement("EffectKey", "animatecorpse")), spell]);
				dispel.GetOrApplyEffect(caster, actor, default, default, null!, []);
				AssertCorpseAnimationRestored(database, host, lifecycle.Origin.Id, actor.InstanceId, corpse.Id, owner.Id, owner.Body.Id, foreign.Id);
				Require(ReadCorpseAnimationLife(database, actor.InstanceId).Reason == SpellRetirementReason.Dispel &&
					!corpse.Effects.Contains(parent) && !corpse.Effects.Contains(glow) && !scheduler.IsScheduled(animation), "Explicit dispel lost its wrapper or left scheduled children.");
				At(Math.Max(180, parentSeconds));
				AssertCorpseAnimationRestored(database, host, lifecycle.Origin.Id, actor.InstanceId, corpse.Id, owner.Id, owner.Body.Id, foreign.Id);
				Console.WriteLine($"ARM03D1P2-dispel-{parentSeconds}-{dispelAt}=passed actual-paid-cast native-proxy-DispelMagicEffect at-{dispelAt}-seconds parent-duration-{parentSeconds} exact-restoration no-late-expiry-replay");
			}
			world.SaveManager.Flush();
		}
		Require(spell.BuildingCommand(caster, new StringStack("effect remove 2")) &&
			spell.BuildingCommand(caster, new StringStack("duration ARM02 Duration")), "Scheduler matrix fixture restoration failed.");
	}
}
