using MudSharp.GameItems.Inventory;

#nullable enable
namespace MudSharp.Magic.Vancian;

public sealed partial class VancianMagicService
{
	public VancianResult Cast(ICharacter actor, IVancianMagicCapability capability, Guid repertoire, Guid allowance,
		IMagicSpell spell, int? ordinal, StringStack targets) => Mutate(actor, capability, state =>
	{
		if (ActionError(actor) is { } physical) return VancianResult.Refused(physical);
		var available = CanCast(actor, capability, state, repertoire, allowance, spell, ordinal);
		if (!available.Available) return VancianResult.Refused(available.Reason);
		if (spell is not MagicSpell runtime) return VancianResult.Refused("That spell implementation does not support Vancian invocation.");
		var resolution = SpellTargetCapture.Resolve(actor, spell, available.Power, targets);
		if (resolution is null) return VancianResult.Refused("No valid target was resolved; nothing was spent.");
		var operation = NewOperation(state, "Cast", "Committing", new XElement("Cast",
			new XAttribute("spell", spell.Id), new XAttribute("repertoire", repertoire), new XAttribute("allowance", allowance),
			new XAttribute("castingLevel", available.CastingLevel), new XAttribute("power", (int)available.Power),
			available.Ordinal is { } position ? new XAttribute("ordinal", position) : null).ToString());
		var invocation = new SpellInvocationContext(SpellInvocationSource.VancianDirect, capability.ReliableOutcome, pay =>
		{
			var current = CanCast(actor, capability, state, repertoire, allowance, spell, available.Ordinal);
			if (!current.Available || current.Power != available.Power || current.CastingLevel != available.CastingLevel) return false;
			if (available.Ordinal is { } number) state.Slots.Single(x => x.AllowanceKey == allowance && x.Ordinal == number).Status = VancianSlotStatus.Spent;
			Store.Commit(state, state.Version, operation);
			try { pay(); _gameworld.SaveManager.Flush(); Store.Record(operation with { Status = "Consumed" }); return true; }
			catch (Exception ex) { Store.Record(operation with { Status = "NeedsReview", Diagnostic = ex.Message }); throw; }
		});
		var invocationSpell = runtime.InvocationCopy(new SpellNumericalContext(spell.SpellLevel, available.CastingLevel,
			CasterLevel(actor, capability), available.Power, capability.ReliableOutcome, false));
		try
		{
			invocationSpell.CastVancian(actor, resolution.Target, available.Power, invocation, resolution.Parameters);
			if (invocation.Status == MagicInvocationStatus.Refused) return VancianResult.Refused("The spell refused preflight; nothing was spent.");
			Store.Record(operation with { Status = "Completed", Diagnostic = invocation.Status.ToString() });
			return new(true, invocation.Status == MagicInvocationStatus.Succeeded ? "Casting committed." : "Casting committed; the spell was resisted or blocked.", operation.Id);
		}
		catch (Exception ex)
		{
			if (Store.Operation(operation.Id) is null) throw;
			Store.Record(operation with { Status = "NeedsReview", Diagnostic = ex.Message });
			return new(false, $"Casting was committed but interrupted. Operation {operation.Id} requires staff inspection; the casting cannot be replayed.", operation.Id);
		}
	});

	public bool KnowsThroughVancian(ICharacter actor, IMagicSpell spell)
	{
		if (spell.Trigger is not ICastMagicTrigger) return false;
		foreach (var capability in actor.Capabilities.OfType<IVancianMagicCapability>().Where(x => x.School.Id == spell.School.Id))
		{
			try
			{
				if (AccessError(actor, capability) is not null) continue;
				var state = State(actor, capability);
				if (state.DataError is not null) continue;
				foreach (var rule in capability.Repertoires.Where(x => Candidate(actor, capability, x, spell)))
				{
					if (rule.Source == VancianRepertoireSource.Selected && ActiveSelections(actor, capability, rule, state).Contains(spell.Id)) return true;
					if (rule.Source == VancianRepertoireSource.Spellbook && HasFormula(actor, capability, spell)) return true;
					if (state.Slots.Any(x => x.Preparation is { } preparation && preparation.RepertoireKey == rule.Key &&
						preparation.SpellId == spell.Id && x.Status is VancianSlotStatus.Prepared or VancianSlotStatus.Reserved &&
						Suspension(actor, capability, x) is null)) return true;
				}
			}
			catch { /* Invalid configurations grant no information route. */ }
		}
		return false;
	}
	public bool AvailableThroughVancian(ICharacter actor, IMagicSpell spell, SpellPower? power = null)
		=> AvailableRoutes(actor, spell, power).Any();

	internal IEnumerable<(IVancianMagicCapability Capability, VancianAvailability Availability)> AvailableRoutes(
		ICharacter actor, IMagicSpell spell, SpellPower? power = null)
	{
		foreach (var capability in actor.Capabilities.OfType<IVancianMagicCapability>().Where(x => x.School.Id == spell.School.Id))
			foreach (var allowance in capability.Allowances)
				foreach (var rule in allowance.RepertoireKeys)
				{
					var availability = CanCast(actor, capability, rule, allowance.Key, spell);
					if (availability.Available && (!power.HasValue || power.Value == availability.Power)) yield return (capability, availability);
				}
	}
}
