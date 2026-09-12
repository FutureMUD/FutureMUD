using MudSharp.GameItems;
using MudSharp.GameItems.Components;

#nullable enable
namespace MudSharp.Magic.Vancian;

public sealed partial class VancianMagicService
{
	public VancianResult AdministerKnown(ICharacter administrator, ICharacter target, IVancianMagicCapability capability, Guid repertoire, IMagicSpell spell, bool grant)
	{
		if (!administrator.IsAdministrator()) return VancianResult.Refused("Administrative permission is required.");
		return Mutate(target, capability, state =>
		{
			var selections = state.Selections.ToDictionary(x => x.Key, x => (IReadOnlyList<long>)x.Value.ToArray());
			var ids = selections.GetValueOrDefault(repertoire)?.ToList() ?? [];
			if (grant && !ids.Contains(spell.Id)) ids.Add(spell.Id); else if (!grant) ids.Remove(spell.Id);
			selections[repertoire] = ids;
			var errors = ValidateSelections(target, capability, selections);
			if (errors.Count > 0) return VancianResult.Refused(string.Join("\n", errors));
			var operation = NewOperation(state, "AdminKnown", "Completed", new XElement("Audit", new XAttribute("administrator", administrator.Id),
				new XAttribute("grant", grant), new XAttribute("spell", spell.Id), new XAttribute("rule", repertoire), state.Save().Element("Selections")).ToString());
			state.Selections[repertoire] = ids;
			Store.Commit(state, state.Version, operation);
			return new(true, "Administrative repertoire change recorded; player hooks were not invoked.", operation.Id);
		});
	}
	public VancianResult AdministerState(ICharacter administrator, ICharacter target, IVancianMagicCapability capability, string action)
	{
		if (!administrator.IsAdministrator()) return VancianResult.Refused("Administrative permission is required.");
		if (action == "refresh") return Mutate(target, capability, state =>
		{
			var pattern = RequestedPattern(state, false, capability) ?? state.LastPattern;
			if (pattern is null) return VancianResult.Refused("Select or explicitly create an empty plan first.");
			var audit = NewOperation(state, "AdminRefreshRequest", "Completed", $"Administrator #{administrator.Id}"); Store.Record(audit);
			return Refresh(state, target, capability, pattern, true);
		});
		if (action != "reset") return VancianResult.Refused("Use refresh or reset.");
		var key = (VancianPolicy.Owner(target).Id, capability.Id);
		lock (_guard) if (!_busy.Add(key)) return VancianResult.Refused("The capability is currently being mutated.");
		try
		{
			var previous = State(target, capability);
			if (Store.HasUnresolved(key.Item1, key.Item2))
				return VancianResult.Refused("Resolve outstanding reservations and indeterminate operations before resetting this capability.");
			var operation = NewOperation(previous, "AdminReset", "Completed", new XElement("Reset", new XAttribute("administrator", administrator.Id), new XElement("Original", previous.OriginalDefinition)).ToString());
			Store.Commit(new() { OwnerId = key.Item1, CapabilityId = key.Item2 }, previous.Version, operation);
			return new(true, "Capability state reset and original data retained in the audit record. No slots were granted; a normal refresh is required.", operation.Id);
		}
		catch (Exception ex) { return VancianResult.Refused(ex.Message); }
		finally { lock (_guard) _busy.Remove(key); }
	}
	public VancianResult AuthorBook(ICharacter administrator, IGameItem item, IMagicSpell spell, bool add)
	{
		if (!administrator.IsAdministrator()) return VancianResult.Refused("Administrative permission is required.");
		if (item.GetItemType<ISpellbook>() is not SpellbookGameItemComponent book || book.DataError is not null) return VancianResult.Refused("The item has no valid spellbook data.");
		if (spell.Trigger is not ICastMagicTrigger) return VancianResult.Refused("Only ordinary spell formulae can be authored in a book.");
		var capability = _gameworld.MagicCapabilities.OfType<IVancianMagicCapability>().FirstOrDefault(x => x.School.Id == spell.School.Id);
		if (capability is null) return VancianResult.Refused("Create a Vancian capability for that school before authoring its book formulae.");
		var token = Guid.NewGuid(); if (!ReserveWritingItem(item, token)) return VancianResult.Refused("That book is currently involved in writing work.");
		try
		{
			var changed = add ? book.AddFormula(spell.Id, UtcNow, null) : book.RemoveFormula(spell.Id);
			if (!changed) return VancianResult.Refused("No change: the formula is already present, absent, or the book is full.");
			var operation = new VancianOperation(token, VancianPolicy.Owner(administrator).Id, capability.Id, "AdminBook", "Committing", 0,
				$"Administrator #{administrator.Id}, {(add ? "add" : "remove")} spell #{spell.Id}", UtcNow, DestinationItem: item.Id);
			Store.Record(operation); Persist(book); Store.Record(operation with { Status = "Completed" });
			return new(true, "Spellbook instance formula updated and audited.", token);
		}
		catch (Exception ex) { return VancianResult.Refused($"Book update requires inspection: {ex.Message}"); }
		finally { ReleaseWritingItems(token); }
	}
	public VancianResult ResolveOperation(ICharacter administrator, Guid token, string action)
	{
		if (!administrator.IsAdministrator()) return VancianResult.Refused("Administrative permission is required.");
		var operation = Store.Operation(token);
		if (operation is null) return VancianResult.Refused("No such operation exists.");
		if (action is not ("acknowledge" or "cancel")) return VancianResult.Refused("Use acknowledge or cancel.");
		if (operation.Status is "Completed" or "Cancelled") return VancianResult.Refused("That operation is already terminal.");
		if (action == "cancel" && operation.Status != "Reserved") return VancianResult.Refused("Only a precommit Reserved operation may be cancelled; committed/indeterminate work cannot be refunded.");
		if (operation.Status == "Reserved" && action != "cancel") return VancianResult.Refused("Use cancel to release a reservation that never reached commitment.");
		if (_writing.TryGetValue(token, out var active)) return CancelWriting(active.Actor, token);
		var target = _gameworld.TryGetCharacter(operation.OwnerId, true);
		if (target is null) return VancianResult.Refused("The operation owner cannot be loaded; preserve the operation for database repair.");
		var capability = _gameworld.MagicCapabilities.Get(operation.CapabilityId) as IVancianMagicCapability;
		if (capability is null) return VancianResult.Refused("The operation's capability is missing; preserve it for database repair.");
		return Mutate(target, capability, state =>
		{
			if (action == "cancel")
			{
				foreach (var slot in state.Slots.Where(x => x.Reservation == token)) { slot.Status = slot.PreviousStatus ?? VancianSlotStatus.Spent; slot.Reservation = null; slot.PreviousStatus = null; }
				foreach (var id in new[] { operation.SourceItem, operation.DestinationItem }.Where(x => x.HasValue))
					if (_gameworld.TryGetItem(id!.Value, true)?.GetItemType<ISpellScroll>() is SpellScrollGameItemComponent scroll) { scroll.CancelReservation(token); Persist(scroll); }
			}
			Store.Commit(state, state.Version, operation with { Status = action == "cancel" ? "Cancelled" : "Completed", Diagnostic = $"Administrator #{administrator.Id} {action}d. Prior status {operation.Status}. No callback replay or replacement casting. {operation.Diagnostic}" });
			return new(true, "Operation resolution audited. No callback was rerun and no replacement casting was granted. Repair indeterminate prog side effects separately.", token);
		}, false);
	}
}
