using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp.Magic.Vancian;

public sealed partial class VancianMagicService
{
	public int NormalScrollCeiling(ICharacter actor, IVancianMagicCapability capability, IMagicSpell spell)
	{
		if (AccessError(actor, capability) is { } error) throw new InvalidOperationException(error);
		var maximum = -1;
		foreach (var allowance in capability.Allowances)
		{
			if (allowance.Mode == VancianAllowanceMode.AtWill)
			{
				if (allowance.RepertoireKeys.Any(rule => CanCast(actor, capability, rule, allowance.Key, spell).Available)) maximum = Math.Max(maximum, spell.SpellLevel);
				continue;
			}
			if (Capacity(actor, capability, allowance) <= 0) continue;
			if (!allowance.RepertoireKeys.Select(key => capability.Repertoires.Single(x => x.Key == key)).Any(rule =>
				Candidates(actor, capability, rule.Key).Any(candidate => AllowanceError(actor, capability, rule, allowance, candidate) is null))) continue;
			maximum = Math.Max(maximum, allowance.SlotLevel!.Value);
		}
		return maximum;
	}
	public Difficulty? ScrollControlDifficulty(ICharacter actor, IVancianMagicCapability capability, IMagicSpell spell, int storedLevel, int ceiling)
	{
		if (storedLevel <= ceiling) return null;
		if (capability.ScrollCheckTrait is null) throw new InvalidOperationException("The over-level scroll control trait is missing.");
		if (capability.PolicyProgs.ContainsKey("scrolldifficulty"))
		{
			var value = Policy(capability, "scrolldifficulty")?.Execute(actor, capability, spell, storedLevel, ceiling);
			if (value is not (decimal or double or float or int or long)) throw new InvalidOperationException("The scroll difficulty policy did not return a number.");
			var numeric = Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture);
			if (!double.IsFinite(numeric) || numeric != Math.Floor(numeric) || numeric < int.MinValue || numeric > int.MaxValue || !Enum.IsDefined((Difficulty)(int)numeric))
				throw new InvalidOperationException("The scroll difficulty policy must return a valid integral Difficulty value.");
			return (Difficulty)(int)numeric;
		}
		return (Difficulty)Math.Min((long)Difficulty.Impossible, (long)Difficulty.Normal + Math.Max(0L, (long)storedLevel - ceiling - 1));
	}
	public VancianResult ActivateScroll(ICharacter actor, IVancianMagicCapability capability, IGameItem item, StringStack targets) => Mutate(actor, capability, state =>
	{
		if (ActionError(actor) is { } physical) return VancianResult.Refused(physical);
		if (actor.EffectsOfType<VancianTimedAction>().Any()) return VancianResult.Refused("Finish or cancel your current magical work first.");
		if (item.GetItemType<ISpellScroll>() is not SpellScrollGameItemComponent scroll) return VancianResult.Refused("That item is not a spell scroll.");
		if (ScrollItemError(actor, scroll, false) is { } itemError) return VancianResult.Refused(itemError);
		if (scroll.Reservation is not null) return VancianResult.Refused("That exact scroll is reserved for another operation.");
		var snapshot = scroll.Snapshot!;
		if (snapshot.SchoolId != capability.School.Id) return VancianResult.Refused("The selected capability belongs to another school.");
		var spell = snapshot.CreateSpell(_gameworld);
		if (!VancianPolicy.Permits(Policy(capability, "cancast"), !capability.PolicyProgs.ContainsKey("cancast"), actor, capability, spell) ||
			!VancianPolicy.Permits(Policy(capability, "scrolluse"), !capability.PolicyProgs.ContainsKey("scrolluse"), actor, capability, spell, item))
			return VancianResult.Refused("The capability's hard spell or scroll-use policy prohibits activation.");
		var proto = (SpellScrollGameItemComponentProto)scroll.Prototype;
		if (!VancianPolicy.Permits(_gameworld.FutureProgs.Get(proto.EligibilityProgId), proto.EligibilityProgId == 0, actor, spell, snapshot.CastingLevel)) return VancianResult.Refused("The scroll prototype's eligibility policy refuses activation.");
		if (actor.CombinedEffectsOfType<MagicSpellLockout>().Any(x => x.Applies(spell.School))) return VancianResult.Refused("You are locked out from casting this school's spells.");
		var difficulty = ScrollControlDifficulty(actor, capability, spell, snapshot.CastingLevel, NormalScrollCeiling(actor, capability, spell));
		var target = SpellTargetCapture.Resolve(actor, spell, snapshot.Power, targets);
		if (target is null || target.Target is null && spell.SpellEffects.Any(x => x.RequiresTarget)) return VancianResult.Refused("No valid target was resolved; the scroll is intact.");
		var operation = NewOperation(state, "ScrollActivation", "Consumed") with { Id = snapshot.ChargeId, SourceItem = item.Id, Payload = snapshot.Save().ToString() };
		if (!ReserveWritingItem(item, operation.Id)) return VancianResult.Refused("That scroll is in use by another operation.");
		try
		{
			if (!scroll.Reserve(operation.Id)) return VancianResult.Refused("The scroll was reserved by another operation.");
			var failedControl = false;
			var invocation = new SpellInvocationContext(SpellInvocationSource.ScrollActivation, snapshot.Numbers.Outcome, _ =>
			{
				if (ScrollItemError(actor, scroll, false) is not null || !Store.ClaimCharge(operation)) return false;
				scroll.Consume(operation.Id); Persist(scroll); item.Delete();
				if (difficulty is { } checkDifficulty && _gameworld.GetCheck(CheckType.CastSpellCheck).Check(actor, checkDifficulty, capability.ScrollCheckTrait, target.Target).Outcome < capability.ScrollMinimumOutcome)
				{
					failedControl = true; spell.ApplyProductionLockouts(actor);
					WritingOutput(actor, proto.FizzleEmote, item); return false;
				}
				WritingOutput(actor, proto.ReleaseEmote, item); return true;
			});
			spell.CastVancian(actor, target.Target, snapshot.Power, invocation, target.Parameters);
			if (failedControl) { Store.Record(operation with { Status = "Completed", Diagnostic = "Over-level control check failed; no effects." }); return new(true, "The scroll was consumed and fizzled; no spell effects were released.", operation.Id); }
			if (invocation.Status == MagicInvocationStatus.Refused)
			{
				scroll.CancelReservation(operation.Id);
				return VancianResult.Refused("Activation refused before commitment; the scroll is intact.");
			}
			Store.Record(operation with { Status = "Completed", Diagnostic = invocation.Status.ToString() });
			return new(true, "The scroll was consumed and its stored spell released.", operation.Id);
		}
		catch (Exception ex)
		{
			if (Store.Operation(operation.Id) is not null) Store.Record(operation with { Status = "NeedsReview", Diagnostic = ex.Message });
			else scroll.CancelReservation(operation.Id);
			throw;
		}
		finally { ReleaseWritingItems(operation.Id); }
	});
}
