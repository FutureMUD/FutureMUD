#nullable enable

using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Checks;
using System.Xml.Linq;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed class PierceRemovalProbe : SpellDetectInvisibleEffect
	{
		private readonly Action _removed;
		public PierceRemovalProbe(IPerceivable owner, IMagicSpellEffectParent parent, Action removed)
			: base(owner, parent) => _removed = removed;
		public override void RemovalEffect()
		{
			base.RemovalEffect();
			_removed();
		}
	}

	private static long VerifyPierceSharedRuntime(NativeRuntime native, string connection, MagicSpell original,
		HarnessClock clock, EffectScheduler scheduler, IPerceivable visibleThroughDetection)
	{
		var actor = native.Actor;
		var inactiveBlindness = new SpellBlindnessEffect(actor, new MagicSpellParent(actor, original, actor), native.World.AlwaysFalseProg);
		Require(!inactiveBlindness.Applies(), "Native false applicability Prog did not evaluate false");
		actor.AddEffect(inactiveBlindness);
		Require(actor.CanSee(visibleThroughDetection), "Inapplicable character blindness blocked visibility");
		actor.RemoveEffect(inactiveBlindness, true);
		Console.WriteLine("PIERCE-applicable-blindness=passed applicable-character-and-body-block inapplicable-character-does-not-block self-and-ignore-exceptions-retained");

		// A separate unowned fixture spell exercises the prepared route with a fixed
		// duration, independently of the still-pending historical stock lifetime policy.
		MudSharp.Models.MagicSpell model;
		using (var db = NewIndependentContext(connection))
		{
			var duration = new MudSharp.Models.TraitExpression { Name = "Pierce lifecycle control duration", Expression = "60" };
			db.TraitExpressions.Add(duration); db.SaveChanges();
			((All<MudSharp.Body.Traits.ITraitExpression>)native.World.TraitExpressions).Add(new MudSharp.Body.Traits.TraitExpression(duration, native.World));
			model = db.MagicSpells.AsNoTracking().Single(x => x.Id == original.Id);
			model.Id = 0; model.Name = "Pierce lifecycle prepared control";
			model.EffectDurationExpressionId = duration.Id;
			var definition = XElement.Parse(model.Definition); definition.Element("StockIdentity")?.Remove(); model.Definition = definition.ToString();
			db.MagicSpells.Add(model); db.SaveChanges();
		}
		var spell = new MagicSpell(model, native.World);
		((All<IMagicSpell>)native.World.MagicSpells).Add(spell);
		Require(spell.StockIdentity is null && spell.AppliedEffectsAreExclusive, "Prepared fixture retained stock ownership or wrong policy");
		MagicSpellParent[] Parents() => actor.EffectsOfType<MagicSpellParent>().Where(x => x.Spell.Id == spell.Id).ToArray();
		SpellDetectInvisibleEffect[] Children() => actor.EffectsOfType<SpellDetectInvisibleEffect>().Where(x => x.ParentEffect.Spell.Id == spell.Id).ToArray();
		void Resolve()
		{
			var balance = actor.MagicResourceAmounts[native.Resource];
			spell.ResolveTriggeredSpell(actor, actor, SpellPower.Standard);
			Require(actor.MagicResourceAmounts[native.Resource] == balance, "Prepared payload unexpectedly paid casting resources");
		}

		Resolve();
		var old = Parents().Single(); var oldChild = Children().Single();
		SpellDetectInvisibleEffect? callbackSawNewChild = null; var callbackCount = 0;
		var probe = new PierceRemovalProbe(actor, old, () =>
		{
			callbackCount++;
			callbackSawNewChild = Children().Single(x => !ReferenceEquals(x.ParentEffect, old));
			Require(!ReferenceEquals(callbackSawNewChild.ParentEffect, old) && callbackSawNewChild.ParentEffect.SpellEffects.Contains(callbackSawNewChild),
				"Old-parent cleanup callback removed the new prepared child/ownership");
		});
		old.AddSpellEffect(probe); actor.AddEffect(probe);
		clock.Advance(TimeSpan.FromSeconds(1)); Resolve();
		var replacement = Parents().Single();
		Require(callbackCount == 1 && !ReferenceEquals(replacement, old) && Children().Length == 1 &&
			ReferenceEquals(Children().Single(), callbackSawNewChild) && !actor.Effects.Contains(oldChild) && !actor.Effects.Contains(probe) &&
			!old.SpellEffects.Any() && ((IEffectExpiryObserver)scheduler).ScheduledExpiry(old) is null && scheduler.OriginalDuration(replacement) == TimeSpan.FromSeconds(60),
			"Prepared exclusive replacement did not remove only the old parent/children/schedule");
		clock.Advance(TimeSpan.FromSeconds(61)); scheduler.CheckSchedules();
		Require(Parents().Length == 0 && Children().Length == 0, "Prepared exclusive replacement expired with an orphan");
		Console.WriteLine("PIERCE-prepared-exclusive=passed old-parent-child-schedule-removed removal-callback-new-child-survives exact-one-parent-child normal-expiry-no-orphan no-extra-payment");

		Require(spell.BuildingCommand(actor, new StringStack("exclusiveeffect")), "Prepared nonexclusive builder toggle");
		Resolve(); var first = Parents().Single(); var firstChild = Children().Single();
		clock.Advance(TimeSpan.FromSeconds(1)); Resolve(); var second = Parents().Single(x => !ReferenceEquals(x, first));
		Require(Parents().Length == 2 && Children().Length == 2 && actor.Effects.Contains(firstChild) && scheduler.IsScheduled(first) && scheduler.IsScheduled(second),
			"Prepared nonexclusive stacking was replaced or unscheduled");
		clock.Advance(TimeSpan.FromSeconds(59.5)); scheduler.CheckSchedules();
		Require(Parents().Length == 1 && ReferenceEquals(Parents().Single(), second) && Children().Length == 1 && !actor.Effects.Contains(firstChild),
			"Nonexclusive first expiry removed its sibling or retained its child");
		clock.Advance(TimeSpan.FromSeconds(1)); scheduler.CheckSchedules();
		Require(Parents().Length == 0 && Children().Length == 0, "Nonexclusive final expiry left parent/child");
		Console.WriteLine("PIERCE-prepared-nonexclusive=passed two-independent-parents-children-deadlines first-expiry-preserves-second final-expiry-no-orphan");

		// Global default removal remains opt-in. This deliberately detached test
		// wrapper is explicitly cleaned afterwards; it is never saved as an orphan.
		var defaultParent = new MagicSpellParent(actor, spell, actor);
		var defaultChild = new SpellDetectInvisibleEffect(actor, defaultParent);
		defaultParent.AddSpellEffect(defaultChild); actor.AddEffect(defaultChild); actor.AddEffect(defaultParent, TimeSpan.FromSeconds(60));
		actor.RemoveEffect(defaultParent);
		Require(actor.Effects.Contains(defaultChild) && ((IEffectExpiryObserver)scheduler).ScheduledExpiry(defaultParent) is null, "Global default removal semantics changed");
		defaultParent.RemovalEffect();
		Require(!actor.Effects.Contains(defaultChild), "Explicit fixture default-removal cleanup failed");
		Console.WriteLine("PIERCE-global-default=passed removal-action-remains-opt-in explicit-owned-fixture-cleanup");

		// Two phases of one self-cast must retain both their new parents. Only prior
		// cast parents are replacement candidates, regardless of identical spell IDs.
		Require(spell.BuildingCommand(actor, new StringStack("exclusiveeffect")), "Restore paired fixture exclusive policy");
		using (new MudSharp.Database.FMDB()) { spell.Save(); MudSharp.Database.FMDB.Context.SaveChanges(); }
		using (var db = NewIndependentContext(connection))
		{
			model = db.MagicSpells.Single(x => x.Id == spell.Id);
			var definition = XElement.Parse(model.Definition);
			definition.Element("CasterEffects")!.Add(new XElement("Effect", new XAttribute("type", "detectinvisible")));
			model.Definition = definition.ToString(); db.SaveChanges();
			((All<IMagicSpell>)native.World.MagicSpells).Remove(spell);
			spell = new MagicSpell(model, native.World); ((All<IMagicSpell>)native.World.MagicSpells).Add(spell);
		}
		var capability = (SkillLevelBasedMagicCapability)native.Capability;
		foreach (var command in new[] { $"casting entry add {spell.Id}", $"casting entry skill {spell.Id} 30 90 relative" })
			Require(capability.BuildingCommand(actor, new StringStack(command)), "Paid paired fixture admission " + command);
		var staff = Mock.Of<ICharacter>(x => x.Id == 999 && x.IsAdministrator(PermissionLevel.JuniorAdmin));
		var casting = native.World.MagicCasting as MagicCastingService ?? throw new InvalidOperationException("Native fixture has no casting service");
		Require(casting.Grant(staff, actor, capability.Id, spell.Id, "native self-caster lifecycle control").Allowed, "Paid paired fixture authorised grant");
		CastPierceSelfCaster(native, spell.Id);
		var oldParents = Parents(); var oldChildren = Children();
		Require(oldParents.Length == 2 && oldChildren.Length == 2 && oldParents.All(x => x.SpellEffects.Count() == 1), "Same-cast caster phase removed new primary parent/child");
		clock.Advance(TimeSpan.FromSeconds(1)); CastPierceSelfCaster(native, spell.Id);
		Require(Parents().Length == 2 && Children().Length == 2 && oldParents.All(x => !actor.Effects.Contains(x) && !x.SpellEffects.Any() && !scheduler.IsScheduled(x)) &&
			oldChildren.All(x => !actor.Effects.Contains(x)), "Paired recast retained old parents/children/schedules or removed new pair");
		clock.Advance(TimeSpan.FromSeconds(61)); scheduler.CheckSchedules();
		Require(Parents().Length == 0 && Children().Length == 0, "Paired self-cast expiry left orphaned phase");
		Console.WriteLine("PIERCE-paid-self-caster=passed primary-and-caster-new-parents-survive each-paid-recast-removes-both-old-parents-children-schedules normal-expiry-no-orphan");
		return spell.Id;
	}

	private static void CastPierceSelfCaster(NativeRuntime native, long spellId)
	{
		var actor = native.Actor;
		var casting = native.World.MagicCasting as MagicCastingService ?? throw new InvalidOperationException("Native fixture has no casting service");
		actor.RemoveAllEffects<MagicSpellLockout>(null, true); actor.AddResource(native.Resource, 118); FlushCasting(native);
		var before = actor.MagicResourceAmounts[native.Resource];
		var intent = new MagicCastingIntent(actor, native.Capability.Id, spellId, 1, false, "me");
		var quote = casting.Quote(intent); Require(quote.Allowed, quote.Reason);
		var result = casting.Cast(intent);
		Require(result.Status == MagicCastingStatus.Succeeded && before - actor.MagicResourceAmounts[native.Resource] == quote.Invocation!.Costs.Single().Amount,
			"Paired fixture ordinary paid cast failed or quote/debit disagreed: " + result.Message);
		Require((bool)XElement.Parse(new MagicCastingStateStore().Operation(result.OperationId!.Value)!.Definition).Attribute("applied")!, "Paired primary operation not reported Applied");
	}
}
