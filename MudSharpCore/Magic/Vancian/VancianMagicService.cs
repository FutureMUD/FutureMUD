using System.Runtime.CompilerServices;
using MudSharp.RPG.Checks;
using MudSharp.GameItems;

#nullable enable
namespace MudSharp.Magic.Vancian;

public sealed partial class VancianMagicService : IVancianMagicService
{
	private static readonly ConditionalWeakTable<IFuturemud, VancianMagicService> Services = new();
	public static VancianMagicService For(IFuturemud gameworld) => Services.GetValue(gameworld, game => new(game, new VancianStateStore()));
	private readonly IFuturemud _gameworld;
	internal readonly IVancianStateStore Store;
	internal readonly TimeProvider Clock;
	private readonly HashSet<(long Owner, long Capability)> _busy = [];
	private readonly object _guard = new();
	private readonly Func<ICharacter, IVancianMagicCapability, IMagicSpell, bool>? _bookAccess;
	private readonly Action<IGameItemComponent>? _persistComponent;
	public VancianMagicService(IFuturemud gameworld, IVancianStateStore store, TimeProvider? clock = null,
		Func<ICharacter, IVancianMagicCapability, IMagicSpell, bool>? bookAccess = null, Action<IGameItemComponent>? persistComponent = null)
	{ _gameworld = gameworld; Store = store; Clock = clock ?? TimeProvider.System; _bookAccess = bookAccess; _persistComponent = persistComponent; }
	internal DateTime UtcNow => Clock.GetUtcNow().UtcDateTime;
	public VancianCapabilityState State(ICharacter actor, IVancianMagicCapability capability)
	{
		var key = (VancianPolicy.Owner(actor).Id, capability.Id);
		lock (_guard) if (_busy.Contains(key)) return Store.Read(key.Item1, key.Item2);
		return ReadAndReconcile(key.Item1, key.Item2);
	}
	internal IFutureProg? Policy(IVancianMagicCapability capability, string name) => _gameworld.FutureProgs.Get(capability.PolicyProgs.GetValueOrDefault(name));
	internal string? AccessError(ICharacter actor, IVancianMagicCapability capability)
	{
		if (!actor.Capabilities.Any(x => x.Id == capability.Id)) return $"You do not currently have {capability.Name}.";
		var errors = capability.ConfigurationErrors();
		return errors.Count > 0 ? $"{capability.Name} is disabled: {string.Join("; ", errors)}" : null;
	}
	internal VancianResult Mutate(ICharacter actor, IVancianMagicCapability capability, Func<VancianCapabilityState, VancianResult> action, bool requireAccess = true)
	{
		var key = (VancianPolicy.Owner(actor).Id, capability.Id);
		lock (_guard) if (!_busy.Add(key)) return VancianResult.Refused("Another operation is changing this identity's capability. Nested mutations are not permitted.");
		try
		{
			if (requireAccess && AccessError(actor, capability) is { } error) return VancianResult.Refused(error);
			var state = ReadAndReconcile(key.Item1, key.Item2);
			if (state.DataError is { } dataError) return VancianResult.Refused($"State disabled; staff repair required: {dataError}");
			return action(state);
		}
		catch (Exception ex) { return VancianResult.Refused($"Vancian operation refused: {ex.Message}"); }
		finally { lock (_guard) _busy.Remove(key); }
	}

	public int CasterLevel(ICharacter actor, IVancianMagicCapability capability) =>
		VancianPolicy.Number(Policy(capability, "casterlevel"), VancianPolicy.Owner(actor), capability);
	public int Capacity(ICharacter actor, IVancianMagicCapability capability, VancianCastingAllowanceDefinition allowance)
	{
		if (allowance.Mode == VancianAllowanceMode.AtWill) return 0;
		var count = VancianPolicy.Number(_gameworld.FutureProgs.Get(allowance.SlotCountProgId), VancianPolicy.Owner(actor), capability, CasterLevel(actor, capability), allowance.SlotLevel!.Value);
		if (count > 100_000) throw new InvalidOperationException($"{allowance.Alias}: capacity exceeds the protective 100,000-entry limit.");
		return count;
	}
	public int SelectionLimit(ICharacter actor, IVancianMagicCapability capability, VancianRepertoireDefinition rule, int level) =>
		VancianPolicy.Number(_gameworld.FutureProgs.Get(rule.SelectionLimitProgId), VancianPolicy.Owner(actor), capability, CasterLevel(actor, capability), level);
	internal bool Candidate(ICharacter actor, IVancianMagicCapability capability, VancianRepertoireDefinition rule, IMagicSpell spell) =>
		spell.School.Id == capability.School.Id && spell.Trigger is ICastMagicTrigger && spell.ReadyForGame &&
		spell.SpellLevel >= rule.MinimumSpellLevel && spell.SpellLevel <= rule.MaximumSpellLevel &&
		VancianPolicy.Permits(_gameworld.FutureProgs.Get(rule.CandidateProgId), false, VancianPolicy.Owner(actor), capability, spell);
	public IReadOnlyList<IMagicSpell> Candidates(ICharacter actor, IVancianMagicCapability capability, Guid repertoire)
	{
		if (AccessError(actor, capability) is not null) return [];
		var rule = capability.Repertoires.FirstOrDefault(x => x.Key == repertoire);
		return rule is null ? [] : _gameworld.MagicSpells.Where(x => x.School.Id == capability.School.Id)
			.Where(x => Candidate(actor, capability, rule, x)).OrderBy(x => x.SpellLevel).ThenBy(x => x.Name).ToArray();
	}
	public IReadOnlyList<IMagicSpell> KnownSpells(ICharacter actor, IVancianMagicCapability capability, Guid? repertoire = null)
	{
		if (AccessError(actor, capability) is not null) return [];
		var state = State(actor, capability);
		return state.DataError is not null ? [] : state.Selections.Where(x => repertoire is null || x.Key == repertoire)
			.SelectMany(x => x.Value).Distinct().OrderBy(x => x).Select(x => _gameworld.MagicSpells.Get(x)).OfType<IMagicSpell>().ToArray();
	}
	internal HashSet<long> ActiveSelections(ICharacter actor, IVancianMagicCapability capability, VancianRepertoireDefinition rule, VancianCapabilityState state)
	{
		return (state.Selections.GetValueOrDefault(rule.Key) ?? []).Select(x => _gameworld.MagicSpells.Get(x))
			.OfType<IMagicSpell>().Where(x => Candidate(actor, capability, rule, x)).GroupBy(x => x.SpellLevel)
			.SelectMany(g => g.Take(SelectionLimit(actor, capability, rule, g.Key))).Select(x => x.Id).ToHashSet();
	}
	internal string? AllowanceError(ICharacter actor, IVancianMagicCapability capability, VancianRepertoireDefinition rule,
		VancianCastingAllowanceDefinition allowance, IMagicSpell spell)
	{
		if (!allowance.RepertoireKeys.Contains(rule.Key)) return "That allowance is not linked to that repertoire.";
		if (!Candidate(actor, capability, rule, spell)) return "That spell is not currently an eligible ordinary-cast candidate in this repertoire.";
		if (spell.SpellLevel < allowance.MinimumSpellLevel || spell.SpellLevel > allowance.MaximumSpellLevel ||
			(allowance.SlotLevel is { } level && spell.SpellLevel > level)) return "The spell level is incompatible with this allowance.";
		if (!VancianPolicy.Permits(_gameworld.FutureProgs.Get(allowance.SpellEligibilityProgId), allowance.SpellEligibilityProgId == 0,
			VancianPolicy.Owner(actor), capability, spell)) return "The allowance's eligibility policy prohibits that spell.";
		if (!VancianPolicy.Permits(Policy(capability, "cancast"), !capability.PolicyProgs.ContainsKey("cancast"), actor, capability, spell)) return "The capability's hard casting policy prohibits that spell.";
		return null;
	}
	internal string? Suspension(ICharacter actor, IVancianMagicCapability capability, VancianSlot slot)
	{
		var allowance = capability.Allowances.FirstOrDefault(x => x.Key == slot.AllowanceKey);
		if (allowance is null || allowance.StructuralVersion != slot.AllowanceVersion || allowance.SlotLevel != slot.Level) return "Removed or structurally changed allowance; refresh required.";
		if (slot.Ordinal > Capacity(actor, capability, allowance)) return "Suspended by reduced current capacity.";
		if (slot.Preparation is not { } preparation) return null;
		var spell = _gameworld.MagicSpells.Get(preparation.SpellId);
		var rule = capability.Repertoires.FirstOrDefault(x => x.Key == preparation.RepertoireKey);
		if (spell is null || rule is null || spell.SpellLevel != preparation.SpellLevel || spell.Trigger is not ICastMagicTrigger trigger ||
			preparation.Power < trigger.MinimumPower || preparation.Power > trigger.MaximumPower) return "Prepared spell, level or supported power changed incompatibly.";
		return AllowanceError(actor, capability, rule, allowance, spell);
	}
	public IReadOnlyList<VancianSlotView> Slots(ICharacter actor, IVancianMagicCapability capability)
	{
		var state = State(actor, capability);
		return state.Slots.Select(slot =>
		{
			string? reason;
			try { reason = state.DataError ?? AccessError(actor, capability) ?? Suspension(actor, capability, slot); }
			catch (Exception ex) { reason = ex.Message; }
			return new VancianSlotView(slot.AllowanceKey, slot.Ordinal, reason is null ? slot.Status : VancianSlotStatus.Suspended, slot.Preparation, reason);
		}).ToArray();
	}
	public VancianAvailability CanCast(ICharacter actor, IVancianMagicCapability capability, Guid repertoire, Guid allowance,
		IMagicSpell spell, int? ordinal = null)
	{
		try { return CanCast(actor, capability, State(actor, capability), repertoire, allowance, spell, ordinal); }
		catch (Exception ex) { return new(false, ex.Message); }
	}
	internal VancianAvailability CanCast(ICharacter actor, IVancianMagicCapability capability, VancianCapabilityState state,
		Guid repertoireKey, Guid allowanceKey, IMagicSpell spell, int? ordinal = null)
	{
		if ((state.DataError ?? AccessError(actor, capability)) is { } error) return new(false, error);
		if (CastingError(actor) is { } physical) return new(false, physical);
		if (actor.CombinedEffectsOfType<MudSharp.Effects.Concrete.MagicSpellLockout>().Any(x => x.Applies(capability.School)))
			return new(false, "You are locked out from casting spells of this school.");
		var rule = capability.Repertoires.FirstOrDefault(x => x.Key == repertoireKey);
		var allowance = capability.Allowances.FirstOrDefault(x => x.Key == allowanceKey);
		if (rule is null || allowance is null) return new(false, "No such repertoire or allowance.");
		if (AllowanceError(actor, capability, rule, allowance, spell) is { } routeError) return new(false, routeError);
		if (allowance.Mode != VancianAllowanceMode.Memorised && !ActiveSelections(actor, capability, rule, state).Contains(spell.Id))
			return new(false, "This spell is not in the active selected repertoire.");
		if (allowance.Mode == VancianAllowanceMode.AtWill)
		{
			var power = VancianPolicy.Power(capability, spell.SpellLevel, spell.SpellLevel);
			return ordinal.HasValue ? new(false, "Use atwill, not a finite slot ordinal.") : PowerAvailable(spell, null, spell.SpellLevel, power);
		}
		var matching = state.Slots.Where(x => x.AllowanceKey == allowanceKey && (!ordinal.HasValue || x.Ordinal == ordinal.Value)).OrderBy(x => x.Ordinal).ToArray();
		foreach (var slot in matching)
		{
			if (Suspension(actor, capability, slot) is not null) continue;
			if (allowance.Mode == VancianAllowanceMode.Memorised && slot.Status == VancianSlotStatus.Prepared &&
				slot.Preparation is { } prepared && prepared.SpellId == spell.Id && prepared.RepertoireKey == repertoireKey)
				return PowerAvailable(spell, slot.Ordinal, prepared.SlotLevel, prepared.Power);
			if (allowance.Mode == VancianAllowanceMode.Spontaneous && slot.Status == VancianSlotStatus.AvailableSpontaneous)
				return PowerAvailable(spell, slot.Ordinal, slot.Level, VancianPolicy.Power(capability, spell.SpellLevel, slot.Level));
		}
		return new(false, matching.Length == 0 ? "No refreshed slot exists in that allowance. Capacity increases require refresh." :
			"No matching unspent, unsuspended copy/slot is available in that allowance. Inspect prepared for details.");
	}
	private static VancianAvailability PowerAvailable(IMagicSpell spell, int? ordinal, int level, SpellPower power) =>
		spell.Trigger is ICastMagicTrigger trigger && power >= trigger.MinimumPower && power <= trigger.MaximumPower
			? new(true, "Available", ordinal, level, power) : new(false, "Computed power is outside the trigger's supported range.");

	public VancianResult CommitKnown(ICharacter actor, IVancianMagicCapability capability, long expectedVersion,
		IReadOnlyDictionary<Guid, IReadOnlyList<long>> selections) => Mutate(actor, capability, state =>
	{
		if (state.Version != expectedVersion) return VancianResult.Refused("The draft is stale. Begin again from current selections.");
		var errors = ValidateSelections(actor, capability, selections);
		if (errors.Count > 0) return VancianResult.Refused(string.Join("\n", errors));
		var keys = state.Selections.Keys.Concat(selections.Keys).Distinct();
		if (keys.All(key => (state.Selections.GetValueOrDefault(key) ?? []).ToHashSet().SetEquals(selections.GetValueOrDefault(key) ?? [])))
			return new(true, "The selected repertoire is unchanged; no policy hook ran.");
		if (Store.HasUnresolved(state.OwnerId, capability.Id, "Known"))
			return VancianResult.Refused("An unresolved known-change callback requires staff review before another change.");
		var oldSpells = SpellSnapshot(state.Selections.Values.SelectMany(x => x));
		var newSpells = SpellSnapshot(selections.Values.SelectMany(x => x));
		if (!VancianPolicy.Permits(Policy(capability, "canchangeknown"), false, VancianPolicy.Owner(actor), capability, oldSpells, newSpells))
			return VancianResult.Refused("The configured known-spell policy does not permit this change.");
		var payload = new XElement("KnownChange", new XElement("Before", state.Save().Element("Selections")),
			new XElement("After", selections.Select(x => new XElement("Rule", new XAttribute("key", x.Key), x.Value.Select(id => new XElement("Spell", id)))))).ToString();
		state.Selections.Clear(); foreach (var (key, spells) in selections) state.Selections.Add(key, spells.ToList());
		var operation = NewOperation(state, "Known", Policy(capability, "onchangeknown") is null ? "Completed" : "Pending", payload);
		Store.Commit(state, expectedVersion, operation);
		Notify(operation, Policy(capability, "onchangeknown"), VancianPolicy.Owner(actor), capability, oldSpells, newSpells);
		return new(true, "Selected repertoire committed. Existing slots and memorised copies are unchanged.", operation.Id);
	});

	public IReadOnlyList<string> ValidateSelections(ICharacter actor, IVancianMagicCapability capability, IReadOnlyDictionary<Guid, IReadOnlyList<long>> selections)
	{
		var errors = new List<string>();
		foreach (var (key, ids) in selections)
		{
			var rule = capability.Repertoires.FirstOrDefault(x => x.Key == key && x.Source == VancianRepertoireSource.Selected);
			if (rule is null) { errors.Add($"Unknown Selected repertoire {key}."); continue; }
			if (ids.Count != ids.Distinct().Count()) errors.Add($"{rule.Alias}: duplicate spell selection.");
			var spells = ids.Select(x => _gameworld.MagicSpells.Get(x)).ToArray();
			foreach (var spell in spells) if (spell is null || !Candidate(actor, capability, rule, spell)) errors.Add($"{rule.Alias}: unavailable or ineligible spell #{spell?.Id}.");
			foreach (var level in spells.OfType<IMagicSpell>().GroupBy(x => x.SpellLevel))
				if (level.Count() > SelectionLimit(actor, capability, rule, level.Key)) errors.Add($"{rule.Alias}: too many level {level.Key} selections.");
		}
		return errors.AsReadOnly();
	}
	private IReadOnlyList<IMagicSpell> SpellSnapshot(IEnumerable<long> ids) => Array.AsReadOnly(ids.Distinct().OrderBy(x => x).Select(x => _gameworld.MagicSpells.Get(x)).OfType<IMagicSpell>().ToArray());
	internal VancianOperation NewOperation(VancianCapabilityState state, string kind, string status, string payload = "") =>
		new(Guid.NewGuid(), state.OwnerId, state.CapabilityId, kind, status, state.Version, payload, UtcNow);
	internal void Notify(VancianOperation operation, IFutureProg? prog, params object[] parameters)
	{
		if (prog is null) return;
		Store.Record(operation with { Status = "Invoking" });
		try
		{
			if (_gameworld.SaveManager.Flushing) throw new InvalidOperationException("A callback cannot establish a durable save boundary during another save.");
			if (!prog.ExecuteWithStatus(out _, parameters)) throw new InvalidOperationException($"Prog #{prog.Id} reported execution failure.");
			_gameworld.SaveManager.Flush();
			if (_gameworld.VariableRegister.Changed || _gameworld.SaveManager.IsQueued(_gameworld.VariableRegister))
				throw new InvalidOperationException("Callback register bookkeeping has not been durably saved.");
			Store.Record(operation with { Status = "Completed" });
		}
		catch (Exception ex) { Store.Record(operation with { Status = "NeedsReview", Diagnostic = ex.Message }); }
	}
}
