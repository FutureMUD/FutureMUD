using MudSharp.Effects.Concrete;

#nullable enable
namespace MudSharp.Magic.Vancian;

public sealed partial class VancianMagicService
{
	internal VancianResult AwardSleep(ICharacter actor, IVancianMagicCapability capability) => Mutate(actor, capability, state =>
	{
		if (state.SleepQualified) return new(true, "An earned qualification already exists.");
		state.SleepQualified = true; Store.Commit(state, state.Version);
		return new(true, "Sleep qualification earned.");
	});
	public VancianResult EditLoadout(ICharacter actor, IVancianMagicCapability capability, string operation, string name,
		string? newName = null, VancianAssignment? assignment = null, Guid? allowance = null, int? ordinal = null) => Mutate(actor, capability, state =>
	{
		var plan = state.Loadouts.Find(x => x.Name.EqualTo(name));
		if (operation != "new" && plan is null) return VancianResult.Refused("There is no loadout with that name.");
		if (operation is "new" or "copy" or "rename")
		{
			var proposed = operation == "new" ? name : newName;
			if (string.IsNullOrWhiteSpace(proposed) || proposed.Length > 80 || proposed.Any(char.IsControl) || state.Loadouts.Any(x => x.Name.EqualTo(proposed)))
				return VancianResult.Refused("Use a unique loadout name of 1-80 printable characters.");
			if (operation != "rename" && state.Loadouts.Count >= capability.MaximumSavedLoadouts) return VancianResult.Refused("You have reached the saved-loadout limit.");
		}
		switch (operation)
		{
			case "new": state.Loadouts.Add(new() { Name = name }); break;
			case "copy":
				var copied = new VancianLoadout { Name = newName! }; copied.Assignments.AddRange(plan!.Assignments); state.Loadouts.Add(copied); break;
			case "rename": plan!.Name = newName!; break;
			case "delete": state.Loadouts.Remove(plan!); if (state.SelectedLoadout == plan!.Id) state.SelectedLoadout = null; break;
			case "select": state.SelectedLoadout = plan!.Id; break;
			case "assign":
				if (assignment is null) return VancianResult.Refused("An explicit assignment is required.");
				var definition = capability.Allowances.FirstOrDefault(x => x.Key == assignment.AllowanceKey);
				if (definition is null || definition.Mode != VancianAllowanceMode.Memorised || assignment.Ordinal < 1 || assignment.Ordinal > Capacity(actor, capability, definition)) return VancianResult.Refused("Select a currently supported memorised slot ordinal.");
				plan!.Assignments.RemoveAll(x => x.AllowanceKey == assignment.AllowanceKey && x.Ordinal == assignment.Ordinal);
				plan.Assignments.Add(assignment); break;
			case "clear": plan!.Assignments.RemoveAll(x => x.AllowanceKey == allowance && x.Ordinal == ordinal); break;
			default: return VancianResult.Refused("Unknown loadout operation.");
		}
		Store.Commit(state, state.Version);
		return new(true, "Saved plan updated. Current memorisation and the last committed pattern are unchanged.");
	});
	public VancianResult SelectLoadout(ICharacter actor, IVancianMagicCapability capability, string name) => EditLoadout(actor, capability, "select", name);

	public IReadOnlyList<string> ValidatePattern(ICharacter actor, IVancianMagicCapability capability, VancianCapabilityState state, IReadOnlyList<VancianAssignment> pattern)
	{
		var errors = new List<string>();
		foreach (var duplicate in pattern.GroupBy(x => (x.AllowanceKey, x.Ordinal)).Where(x => x.Count() > 1)) errors.Add($"Duplicate assignment at {duplicate.Key}.");
		foreach (var a in pattern)
		{
			var allowance = capability.Allowances.FirstOrDefault(x => x.Key == a.AllowanceKey);
			var rule = capability.Repertoires.FirstOrDefault(x => x.Key == a.RepertoireKey);
			var spell = _gameworld.MagicSpells.Get(a.SpellId);
			var label = $"{allowance?.Alias ?? a.AllowanceKey.ToString()} slot {a.Ordinal}";
			if (allowance is null || rule is null || spell is null) { errors.Add($"{label}: missing allowance, repertoire or spell."); continue; }
			if (allowance.Mode != VancianAllowanceMode.Memorised || allowance.StructuralVersion != a.AllowanceVersion || allowance.SlotLevel != a.SlotLevel) errors.Add($"{label}: structural configuration changed; reassign this planned slot.");
			if (a.Ordinal < 1 || a.Ordinal > Capacity(actor, capability, allowance)) errors.Add($"{label}: insufficient current capacity.");
			if (a.SpellLevel != spell.SpellLevel) errors.Add($"{label}: the spell's base level changed; reassign it.");
			if (AllowanceError(actor, capability, rule, allowance, spell) is { } routeError) errors.Add($"{label}: {routeError}");
			if (a.SlotLevel >= spell.SpellLevel && !PowerAvailable(spell, a.Ordinal, a.SlotLevel, VancianPolicy.Power(capability, spell.SpellLevel, a.SlotLevel)).Available) errors.Add($"{label}: computed power is outside the trigger range.");
			if (rule.Source == VancianRepertoireSource.Selected)
			{
				if (!ActiveSelections(actor, capability, rule, state).Contains(spell.Id)) errors.Add($"{label}: spell is not in the active selected repertoire.");
			}
			else if ((rule.BookPolicy == VancianBookPolicy.EveryRefresh || !SameBookPattern(pattern, state.LastPattern ?? [], rule.Key)) && !HasFormula(actor, capability, spell))
				errors.Add($"{label}: no accessible usable spellbook supplies {spell.Name}.");
		}
		return errors.AsReadOnly();
	}
	public static bool SameBookPattern(IEnumerable<VancianAssignment> a, IEnumerable<VancianAssignment> b, Guid rule)
	{
		static Dictionary<(Guid, int, int, long), int> Pattern(IEnumerable<VancianAssignment> entries, Guid key) => entries.Where(x => x.RepertoireKey == key)
			.GroupBy(x => (x.AllowanceKey, x.AllowanceVersion, x.SlotLevel, x.SpellId)).ToDictionary(x => x.Key, x => x.Count());
		var left = Pattern(a, rule); var right = Pattern(b, rule);
		return left.Count == right.Count && left.All(x => right.GetValueOrDefault(x.Key) == x.Value);
	}
	internal bool HasFormula(ICharacter actor, IVancianMagicCapability capability, IMagicSpell spell) =>
		_bookAccess?.Invoke(actor, capability, spell) ?? VancianItemAccess.Books(actor)
			.Any(book => book.Formulae.Any(x => x.SpellId == spell.Id) && VancianItemAccess.UsableBook(actor, capability, book, spell));

	private string? RefreshError(ICharacter actor, IVancianMagicCapability capability, VancianCapabilityState state, IReadOnlyList<VancianAssignment> pattern, bool administrative = false)
	{
		if (AccessError(actor, capability) is { } access) return access;
		if (!administrative && ActionError(actor) is { } physical) return physical;
		if (!administrative && state.LastRefreshUtc is { } last && UtcNow - last < capability.MinimumRefreshInterval) return "The minimum refresh interval has not elapsed.";
		if (!administrative && capability.RecoveryMode != VancianRecoveryMode.PreparationAction && !state.SleepQualified) return "You have no earned sleep qualification.";
		if (Store.HasUnresolved(state.OwnerId, capability.Id, "Refresh", "AdminRefresh")) return "An unresolved refresh callback requires staff review before another refresh.";
		if (state.Slots.Any(x => x.Status == VancianSlotStatus.Reserved)) return "A reserved casting must be completed or cancelled before refresh.";
		if (!administrative && !VancianPolicy.Permits(Policy(capability, "canrefresh"), !capability.PolicyProgs.ContainsKey("canrefresh"), VancianPolicy.Owner(actor), capability)) return "The refresh policy refuses this preparation.";
		var errors = ValidatePattern(actor, capability, state, pattern);
		return errors.Count == 0 ? null : string.Join("\n", errors);
	}
	internal static string? ActionError(ICharacter actor)
	{
		if (!actor.State.IsConscious() || actor.State.HasFlag(CharacterState.Sleeping) || actor.State.HasFlag(CharacterState.Stasis) || actor.Combat is not null || actor.Movement is not null)
			return "You must be awake, conscious, stationary and out of combat.";
		if (actor.Identity?.FocusedInstance is { } focused && !ReferenceEquals(focused, actor) && actor.IsPlayerCharacter) return "You must focus on the acting instance.";
		return null;
	}
	private static IReadOnlyList<VancianAssignment>? RequestedPattern(VancianCapabilityState state, bool last, IVancianMagicCapability capability)
	{
		if (last) return state.LastPattern?.ToArray();
		if (state.SelectedLoadout is { } selected) return state.Loadouts.Find(x => x.Id == selected)?.Assignments.ToArray();
		return capability.Allowances.Any(x => x.Mode == VancianAllowanceMode.Memorised) ? null : [];
	}
	public bool CanRefresh(ICharacter actor, IVancianMagicCapability capability)
	{
		try { var state = State(actor, capability); var pattern = RequestedPattern(state, false, capability); return state.DataError is null && pattern is not null && RefreshError(actor, capability, state, pattern) is null; }
		catch { return false; }
	}
	public VancianResult RequestRefresh(ICharacter actor, IVancianMagicCapability capability, bool last = false) => Mutate(actor, capability, state =>
	{
		var pattern = RequestedPattern(state, last, capability);
		if (pattern is null) return VancianResult.Refused("Select an explicit populated or empty loadout first, or use refresh last when a committed pattern exists.");
		if (RefreshError(actor, capability, state, pattern) is { } error) return VancianResult.Refused(error);
		if (capability.RecoveryMode == VancianRecoveryMode.SleepAutomatic) return Refresh(state, actor, capability, pattern);
		if (actor.EffectsOfType<VancianTimedAction>().Any()) return VancianResult.Refused("You already have a Vancian action in progress.");
		var version = state.Version;
		var start = UtcNow;
		var token = Guid.NewGuid();
		actor.AddEffect(new VancianTimedAction(actor, token, "preparing spells", () =>
		{
			var result = CompletePreparation(actor, capability, version, pattern, start);
			actor.OutputHandler.Send(result.Message);
		}, stillValid: () => State(actor, capability).Version == version && RefreshError(actor, capability, State(actor, capability), pattern) is null,
			observedItems: VancianItemAccess.Books(actor).Select(x => x.Parent)), capability.PreparationDuration);
		return new(true, $"You begin preparing spells for {capability.PreparationDuration.Describe(actor)}. The captured plan takes effect only on successful completion.", token);
	});
	private VancianResult CompletePreparation(ICharacter actor, IVancianMagicCapability capability, long expectedVersion,
		IReadOnlyList<VancianAssignment> pattern, DateTime startedUtc) => Mutate(actor, capability, state =>
	{
		if (state.Version != expectedVersion) return VancianResult.Refused("Your state or saved plan changed during preparation. Restart the action.");
		if (UtcNow - startedUtc < capability.PreparationDuration) return VancianResult.Refused("Preparation has not completed.");
		return Refresh(state, actor, capability, pattern);
	});
	private VancianResult Refresh(VancianCapabilityState state, ICharacter actor, IVancianMagicCapability capability,
		IReadOnlyList<VancianAssignment> pattern, bool administrative = false)
	{
		if (RefreshError(actor, capability, state, pattern, administrative) is { } error) return VancianResult.Refused(error);
		var capacities = capability.Allowances.Where(x => x.Mode != VancianAllowanceMode.AtWill).ToDictionary(x => x.Key, x => Capacity(actor, capability, x));
		if (capacities.Values.Sum(x => (long)x) > 100_000) return VancianResult.Refused("Total capacity exceeds the protective 100,000-entry limit.");
		var committed = pattern.Select(x => x with { Power = VancianPolicy.Power(capability, x.SpellLevel, x.SlotLevel) }).ToList();
		state.Slots.Clear();
		foreach (var allowance in capability.Allowances.Where(x => x.Mode != VancianAllowanceMode.AtWill))
			for (var ordinal = 1; ordinal <= capacities[allowance.Key]; ordinal++)
			{
				var assignment = committed.FirstOrDefault(x => x.AllowanceKey == allowance.Key && x.Ordinal == ordinal);
				state.Slots.Add(new() { AllowanceKey = allowance.Key, AllowanceVersion = allowance.StructuralVersion, Ordinal = ordinal,
					Level = allowance.SlotLevel!.Value, Preparation = assignment, Status = allowance.Mode == VancianAllowanceMode.Spontaneous
						? VancianSlotStatus.AvailableSpontaneous : assignment is null ? VancianSlotStatus.Unassigned : VancianSlotStatus.Prepared });
			}
		state.LastPattern = committed;
		state.Generation = checked(state.Generation + 1); state.LastRefreshUtc = UtcNow; state.SleepQualified = false;
		var operation = NewOperation(state, administrative ? "AdminRefresh" : "Refresh", Policy(capability, "onrefresh") is null ? "Completed" : "Pending");
		Store.Commit(state, state.Version, operation);
		Notify(operation, Policy(capability, "onrefresh"), VancianPolicy.Owner(actor), capability);
		return new(true, $"{capability.Name} refreshed. Finite slots and the last committed pattern are now generation {state.Generation.ToString("N0", actor)}.", operation.Id);
	}
}
