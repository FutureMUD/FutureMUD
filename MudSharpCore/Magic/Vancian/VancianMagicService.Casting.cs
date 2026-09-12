using MudSharp.GameItems.Inventory;

#nullable enable
namespace MudSharp.Magic.Vancian;

public sealed partial class VancianMagicService
{
	public VancianResult Cast(ICharacter actor, IVancianMagicCapability capability, Guid repertoire, Guid allowance,
		IMagicSpell spell, int? ordinal, StringStack targets) => Cast(actor, capability, repertoire, allowance, spell, ordinal,
			power => SpellTargetCapture.Resolve(actor, spell, power, targets));

	internal VancianResult CastFromPower(ICharacter actor, MagicSpell spell, IPerceivable? target, SpellPower power,
		SpellAdditionalParameter[] parameters)
	{
		var routes = AvailableRoutes(actor, spell, power).ToArray();
		if (routes.Length != 1) return VancianResult.Refused(routes.Length == 0
			? "No unspent Vancian casting supplies the requested spell and power."
			: $"Several Vancian routes supply this spell and power. Use {spell.School.SchoolVerb} vancian <capability> cast <repertoire> <allowance> to choose which casting to spend.");
		var route = routes[0];
		return Cast(actor, route.Capability, route.Repertoire, route.Allowance, spell, route.Availability.Ordinal,
			computed => computed == power ? new SpellTargetResolution(target, parameters) : null);
	}

	private VancianResult Cast(ICharacter actor, IVancianMagicCapability capability, Guid repertoire, Guid allowance,
		IMagicSpell spell, int? ordinal, Func<SpellPower, SpellTargetResolution?> resolve) => Mutate(actor, capability, state =>
	{
		if (CastingError(actor) is { } physical) return VancianResult.Refused(physical);
		var available = CanCast(actor, capability, state, repertoire, allowance, spell, ordinal);
		if (!available.Available) return VancianResult.Refused(available.Reason);
		if (spell is not MagicSpell runtime) return VancianResult.Refused("That spell implementation does not support Vancian invocation.");
		var resolution = resolve(available.Power);
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
		var adapter = SpellPowerInvocation.For(actor, runtime);
		using var adaptedInvocation = adapter is null ? null : new SpellPowerInvocation(actor, invocationSpell, adapter.Power);
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
		finally { adapter?.Complete(invocation.Status); }
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
					if (rule.Source == VancianRepertoireSource.Spellbook && state.LastPattern?.Any(entry =>
						entry.RepertoireKey == rule.Key && entry.SpellId == spell.Id && entry.Ordinal > 0 &&
						state.LastPattern.Count(x => x.AllowanceKey == entry.AllowanceKey && x.Ordinal == entry.Ordinal) == 1 &&
						capability.Allowances.Any(x => x.Key == entry.AllowanceKey && x.Mode == VancianAllowanceMode.Memorised) &&
						Suspension(actor, capability, new VancianSlot { AllowanceKey = entry.AllowanceKey,
							AllowanceVersion = entry.AllowanceVersion, Ordinal = entry.Ordinal, Level = entry.SlotLevel,
							Preparation = entry }) is null) == true) return true;
				}
			}
			catch { /* Invalid configurations grant no information route. */ }
		}
		return false;
	}
	public bool AvailableThroughVancian(ICharacter actor, IMagicSpell spell, SpellPower? power = null)
		=> AvailableRoutes(actor, spell, power).Any();

	internal IEnumerable<(IVancianMagicCapability Capability, Guid Repertoire, Guid Allowance, VancianAvailability Availability)> AvailableRoutes(
		ICharacter actor, IMagicSpell spell, SpellPower? power = null)
	{
		foreach (var capability in actor.Capabilities.OfType<IVancianMagicCapability>().Where(x => x.School.Id == spell.School.Id).DistinctBy(x => x.Id))
			foreach (var allowance in capability.Allowances)
				foreach (var rule in allowance.RepertoireKeys)
				{
					var availability = CanCast(actor, capability, rule, allowance.Key, spell);
					if (availability.Available && (!power.HasValue || power.Value == availability.Power)) yield return (capability, rule, allowance.Key, availability);
				}
	}
}
