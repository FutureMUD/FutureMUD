using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp.Magic.Vancian;

public sealed partial class VancianMagicService
{
	private sealed record WritingWork(Guid Token, ICharacter Actor, IVancianMagicCapability Capability, MagicSpell Spell,
		IGameItem Destination, IGameItem? Source, Guid Repertoire, Guid Allowance, VancianAvailability Casting,
		long Version, DateTime Started, TimeSpan Duration, VancianOperation Operation);
	private readonly Dictionary<Guid, WritingWork> _writing = [];
	private readonly Dictionary<long, Guid> _writingItems = [];
	private void Persist(IGameItemComponent component)
	{
		if (_persistComponent is not null) { _persistComponent(component); return; }
		_gameworld.SaveManager.Flush();
		using (new FMDB()) { component.Save(); FMDB.Context.SaveChanges(); }
	}
	private bool ReserveWritingItem(IGameItem item, Guid token)
	{
		lock (_guard) { if (_writingItems.ContainsKey(item.Id)) return false; _writingItems.Add(item.Id, token); return true; }
	}
	private void ReleaseWritingItems(Guid token)
	{
		lock (_guard) foreach (var id in _writingItems.Where(x => x.Value == token).Select(x => x.Key).ToArray()) _writingItems.Remove(id);
	}
	private static void WritingOutput(ICharacter actor, string emote, IGameItem destination, IGameItem? source = null) =>
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(emote, actor, actor, destination, source ?? destination)));
	private string? ScrollItemError(ICharacter actor, SpellScrollGameItemComponent scroll, bool blank)
	{
		if (scroll.DataError is { } error) return $"Scroll data is disabled: {error}";
		if (blank ? !scroll.IsBlank : !scroll.IsCharged) return blank ? "That scroll is not blank." : "That scroll has no usable charge.";
		if (scroll.Parent.GetItemType<IStackable>() is not null || scroll.Parent.GetItemType<IContainer>() is not null || scroll.Parent.GetItemType<ISpellbook>() is not null)
			return "Spell scrolls cannot also be stackable, containers or spellbooks.";
		if (!VancianItemAccess.Usable(actor, scroll.Parent, (SpellScrollGameItemComponentProto)scroll.Prototype)) return "You cannot currently access, manipulate or read that scroll.";
		if (scroll.ChargeId is { } charge && Store.ItemConsumed(scroll.Parent.Id, charge)) return "That charge was already committed or consumed; it cannot be replayed.";
		return null;
	}
	public VancianResult BeginInscription(ICharacter actor, IVancianMagicCapability capability, Guid repertoire, Guid allowance,
		IMagicSpell spell, int? ordinal, IGameItem blank) => Mutate(actor, capability, state =>
	{
		if (ActionError(actor) is { } physical) return VancianResult.Refused(physical);
		if (actor.EffectsOfType<VancianTimedAction>().Any()) return VancianResult.Refused("Finish or cancel your current magical work first.");
		if (spell is not MagicSpell runtime || blank.GetItemType<ISpellScroll>() is not SpellScrollGameItemComponent scroll) return VancianResult.Refused("A runtime spell and blank spellscroll component are required.");
		if (ScrollItemError(actor, scroll, true) is { } itemError) return VancianResult.Refused(itemError);
		if (scroll.Reservation is not null) return VancianResult.Refused("That blank is already reserved; interrupted persisted operations require staff inspection.");
		var route = CanCast(actor, capability, state, repertoire, allowance, spell, ordinal);
		if (!route.Available) return VancianResult.Refused(route.Reason);
		var proto = (SpellScrollGameItemComponentProto)scroll.Prototype;
		if (InscriptionPermission(actor, capability, runtime, scroll, route) is { } policyError) return VancianResult.Refused(policyError);
		_ = runtime.ProductionCosts(actor, route.Power, route.CastingLevel, CasterLevel(actor, capability));
		using var plan = new VancianProductionPlan(actor, runtime.InventoryPlanTemplate, proto.ProductionPlan);
		if (plan.Validate(blank) is { } materials) return VancianResult.Refused(materials);
		var duration = proto.Duration(spell.SpellLevel, route.CastingLevel, CasterLevel(actor, capability));
		var operation = NewOperation(state, "Inscription", "Reserved") with { DestinationItem = blank.Id };
		if (!ReserveWritingItem(blank, operation.Id)) return VancianResult.Refused("That item is already in use by another writing operation.");
		try
		{
			if (!scroll.Reserve(operation.Id)) throw new InvalidOperationException("The blank was reserved by another operation.");
			if (route.Ordinal is { } number)
			{
				var slot = state.Slots.Single(x => x.AllowanceKey == allowance && x.Ordinal == number);
				slot.PreviousStatus = slot.Status; slot.Status = VancianSlotStatus.Reserved; slot.Reservation = operation.Id;
			}
			Store.Commit(state, state.Version, operation);
			Persist(scroll);
			var work = new WritingWork(operation.Id, actor, capability, runtime, blank, null, repertoire, allowance, route, state.Version, UtcNow, duration, operation);
			_writing.Add(operation.Id, work);
			StartWriting(work, proto);
			return new(true, $"You begin inscription, reserving the selected casting for {duration.Describe(actor)}.", operation.Id);
		}
		catch { ReleaseWritingItems(operation.Id); throw; }
	});
	private string? InscriptionPermission(ICharacter actor, IVancianMagicCapability capability, MagicSpell spell, SpellScrollGameItemComponent scroll, VancianAvailability route)
	{
		var errors = ScrollSpellCompatibility.Errors(spell);
		if (errors.Count > 0) return string.Join("\n", errors);
		var proto = (SpellScrollGameItemComponentProto)scroll.Prototype;
		return VancianPolicy.Permits(_gameworld.FutureProgs.Get(proto.EligibilityProgId), proto.EligibilityProgId == 0, actor, spell, route.CastingLevel) &&
			VancianPolicy.Permits(Policy(capability, "inscribe"), !capability.PolicyProgs.ContainsKey("inscribe"), actor, capability, spell, scroll.Parent, route.CastingLevel)
			? null : "The scroll or capability inscription policy refuses this spell.";
	}
	private void StartWriting(WritingWork work, VancianWritingComponentProto proto)
	{
		WritingOutput(work.Actor, proto.StartEmote, work.Destination, work.Source);
		work.Actor.AddEffect(new VancianTimedAction(work.Actor, work.Token, work.Source is null ? "inscribing a spell scroll" : "transcribing a spell formula", () =>
		{
			var result = CompleteWriting(work.Actor, work.Token);
			work.Actor.OutputHandler.Send(result.Message);
		}, () => CancelWriting(work.Actor, work.Token), () =>
			ActionError(work.Actor) is null && AccessError(work.Actor, work.Capability) is null &&
			VancianItemAccess.Usable(work.Actor, work.Destination, proto) &&
			(work.Source is null || VancianItemAccess.Accessible(work.Actor, work.Source)),
			new[] { work.Source, work.Destination }.OfType<IGameItem>()), work.Duration);
	}
	public VancianResult CompleteWriting(ICharacter actor, Guid token)
	{
		if (!_writing.TryGetValue(token, out var work) || !ReferenceEquals(actor, work.Actor)) return VancianResult.Refused("No live engine-issued reservation exists for this actor and token. Persisted incomplete work requires staff inspection.");
		if (UtcNow - work.Started < work.Duration) return VancianResult.Refused("The reserved production time has not elapsed.");
		var result = Mutate(actor, work.Capability, state => CompleteWriting(work, state));
		if (!result.Success && Store.Operation(token)?.Status == "Reserved") CancelWriting(actor, token);
		_writing.Remove(token); ReleaseWritingItems(token);
		foreach (var action in actor.EffectsOfType<VancianTimedAction>().Where(x => x.OperationId == token).ToArray()) action.FinishExternally();
		return result;
	}
	private VancianResult CompleteWriting(WritingWork work, VancianCapabilityState state)
	{
		var actor = work.Actor; var capability = work.Capability; var spell = work.Spell;
		if (ActionError(actor) is { } physical) return VancianResult.Refused(physical);
		if (state.Version != work.Version) return VancianResult.Refused("Capability state changed during writing. Restart the operation.");
		if (work.Source is not null) return CompleteTranscription(work, state);
		var scroll = (SpellScrollGameItemComponent)work.Destination.GetItemType<ISpellScroll>();
		if (ScrollItemError(actor, scroll, true) is { } error) return VancianResult.Refused(error);
		if (scroll.Reservation != work.Token) return VancianResult.Refused("The blank reservation changed.");
		if (work.Casting.Ordinal is { } ordinal)
		{
			var slot = state.Slots.Single(x => x.AllowanceKey == work.Allowance && x.Ordinal == ordinal);
			if (slot.Reservation != work.Token || slot.Status != VancianSlotStatus.Reserved) return VancianResult.Refused("The finite casting reservation changed.");
			slot.Status = slot.PreviousStatus!.Value; slot.Reservation = null; slot.PreviousStatus = null;
		}
		var route = CanCast(actor, capability, state, work.Repertoire, work.Allowance, spell, work.Casting.Ordinal);
		if (!route.Available || route.Power != work.Casting.Power || route.CastingLevel != work.Casting.CastingLevel) return VancianResult.Refused("The reserved casting is no longer available with the same level and power.");
		if (InscriptionPermission(actor, capability, spell, scroll, route) is { } denied) return VancianResult.Refused(denied);
		var proto = (SpellScrollGameItemComponentProto)scroll.Prototype;
		var costs = spell.ProductionCosts(actor, route.Power, route.CastingLevel, CasterLevel(actor, capability));
		using var plan = new VancianProductionPlan(actor, spell.InventoryPlanTemplate, proto.ProductionPlan);
		if (plan.Validate(work.Destination) is { } materials) return VancianResult.Refused(materials);
		var snapshot = StoredSpellSnapshot.Capture(spell, actor, capability, route.CastingLevel, route.Power, CasterLevel(actor, capability), UtcNow);
		if (route.Ordinal is { } number) state.Slots.Single(x => x.AllowanceKey == work.Allowance && x.Ordinal == number).Status = VancianSlotStatus.Spent;
		var committing = work.Operation with { Status = "Committing", Payload = snapshot.Save().ToString() };
		Store.Commit(state, state.Version, committing);
		try
		{
			foreach (var (resource, cost) in costs) actor.UseResource(resource, cost);
			plan.Execute(); spell.ApplyProductionLockouts(actor); _gameworld.SaveManager.Flush();
			scroll.Charge(work.Token, snapshot); Persist(scroll);
			WritingOutput(actor, proto.CompleteEmote, work.Destination);
			var completed = committing with { Status = Policy(capability, "oninscribe") is null ? "Completed" : "Pending" };
			Store.Record(completed);
			Notify(completed, Policy(capability, "oninscribe"), actor, capability, spell, scroll.Parent, route.CastingLevel);
			return new(true, "Inscription committed: costs paid and one stored charge created.", work.Token);
		}
		catch (Exception ex) { Store.Record(committing with { Status = "NeedsReview", Diagnostic = ex.Message }); throw; }
	}
	public VancianResult CancelWriting(ICharacter actor, Guid token)
	{
		if (!_writing.TryGetValue(token, out var work) || !ReferenceEquals(work.Actor, actor)) return VancianResult.Refused("No cancellable writing operation exists for this actor.");
		if (Store.Operation(token)?.Status != "Reserved") return VancianResult.Refused("Commitment already began. Staff review is required; cancellation cannot refund costs.");
		var result = Mutate(actor, work.Capability, state =>
		{
			foreach (var slot in state.Slots.Where(x => x.Reservation == token))
			{ slot.Status = slot.PreviousStatus ?? VancianSlotStatus.Spent; slot.PreviousStatus = null; slot.Reservation = null; }
			Store.Commit(state, state.Version, work.Operation with { Status = "Cancelled" });
			foreach (var item in new[] { work.Source, work.Destination }.Where(x => x is not null))
				if (item!.GetItemType<ISpellScroll>() is SpellScrollGameItemComponent scroll) { scroll.CancelReservation(token); Persist(scroll); }
			WritingOutput(actor, ((VancianWritingComponentProto)(work.Source is null ? work.Destination.GetItemType<ISpellScroll>().Prototype : work.Destination.GetItemType<ISpellbook>().Prototype)).CancelEmote, work.Destination, work.Source);
			return new VancianResult(true, "Writing cancelled. No formula or charge was created and no costs were consumed.", token);
		}, false);
		if (result.Success)
		{
			_writing.Remove(token); ReleaseWritingItems(token);
			foreach (var action in actor.EffectsOfType<VancianTimedAction>().Where(x => x.OperationId == token).ToArray()) action.FinishExternally();
		}
		return result;
	}
	public VancianResult BeginTranscription(ICharacter actor, IVancianMagicCapability capability, IGameItem source,
		IMagicSpell spell, IGameItem destination) => Mutate(actor, capability, state =>
	{
		if (spell is not MagicSpell runtime) return VancianResult.Refused("That spell has no runtime formula.");
		if (actor.EffectsOfType<VancianTimedAction>().Any()) return VancianResult.Refused("Finish or cancel your current magical work first.");
		if (TranscriptionError(actor, capability, source, runtime, destination) is { } error) return VancianResult.Refused(error);
		var book = destination.GetItemType<ISpellbook>(); var proto = (SpellbookGameItemComponentProto)book.Prototype;
		using var plan = new VancianProductionPlan(actor, proto.ProductionPlan);
		if (plan.Validate(source, destination) is { } materials) return VancianResult.Refused(materials);
		var duration = proto.Duration(spell.SpellLevel, spell.SpellLevel, CasterLevel(actor, capability));
		var operation = NewOperation(state, "Transcription", "Reserved") with { SourceItem = source.Id, DestinationItem = destination.Id };
		if (!ReserveWritingItem(destination, operation.Id)) return VancianResult.Refused("That destination is already in use.");
		if (!ReserveWritingItem(source, operation.Id)) { ReleaseWritingItems(operation.Id); return VancianResult.Refused("That source is already in use."); }
		try
		{
			if (source.GetItemType<ISpellScroll>() is SpellScrollGameItemComponent scroll)
			{
				if (!scroll.Reserve(operation.Id)) throw new InvalidOperationException("The source scroll is already reserved.");
				Persist(scroll);
			}
			Store.Record(operation);
			var work = new WritingWork(operation.Id, actor, capability, runtime, destination, source, Guid.Empty, Guid.Empty, new(false, ""), state.Version, UtcNow, duration, operation);
			_writing.Add(operation.Id, work); StartWriting(work, proto);
			return new(true, $"You begin copying the formula for {duration.Describe(actor)}.", operation.Id);
		}
		catch { ReleaseWritingItems(operation.Id); throw; }
	});
	private string? TranscriptionError(ICharacter actor, IVancianMagicCapability capability, IGameItem source, MagicSpell spell, IGameItem destination, Guid? reservation = null)
	{
		if (ActionError(actor) is { } physical) return physical;
		if (source == destination) return "Source and destination must be different items.";
		if (spell.School.Id != capability.School.Id || spell.Trigger is not ICastMagicTrigger || !spell.ReadyForGame) return "That is not an eligible ordinary spell in this school.";
		if (destination.GetItemType<ISpellbook>() is not SpellbookGameItemComponent book || !VancianItemAccess.UsableBook(actor, capability, book, spell)) return "The destination spellbook is not usable for this formula.";
		if (book.Formulae.Any(x => x.SpellId == spell.Id)) return "The destination already contains that formula.";
		if (book.Formulae.Count >= book.FormulaCapacity) return "The destination spellbook is full.";
		if (source.GetItemType<ISpellbook>() is { } sourceBook)
		{
			if (!sourceBook.Formulae.Any(x => x.SpellId == spell.Id) || !VancianItemAccess.UsableBook(actor, capability, sourceBook, spell)) return "The source has no accessible usable formula for that spell.";
		}
		else if (source.GetItemType<ISpellScroll>() is SpellScrollGameItemComponent scroll)
		{
			if (ScrollItemError(actor, scroll, false) is { } error) return error;
			if (scroll.SpellId != spell.Id || (scroll.Reservation is not null && scroll.Reservation != reservation)) return "The source scroll holds another spell or is reserved.";
			_ = scroll.Snapshot!.CreateSpell(_gameworld);
		}
		else return "The source must be a spellbook or charged spell scroll.";
		return VancianPolicy.Permits(Policy(capability, "transcribe"), !capability.PolicyProgs.ContainsKey("transcribe"), actor, capability, spell, source, destination)
			? null : "The capability's transcription policy refuses this formula.";
	}
	private VancianResult CompleteTranscription(WritingWork work, VancianCapabilityState state)
	{
		var source = work.Source!; var actor = work.Actor;
		if (TranscriptionError(actor, work.Capability, source, work.Spell, work.Destination, work.Token) is { } error) return VancianResult.Refused(error);
		var book = (SpellbookGameItemComponent)work.Destination.GetItemType<ISpellbook>();
		var proto = (SpellbookGameItemComponentProto)book.Prototype;
		using var plan = new VancianProductionPlan(actor, proto.ProductionPlan);
		if (plan.Validate(source, work.Destination) is { } materials) return VancianResult.Refused(materials);
		var operation = work.Operation with { Status = "Committing", Payload = new XElement("Formula", new XAttribute("spell", work.Spell.Id)).ToString() };
		Store.Record(operation);
		try
		{
			if (source.GetItemType<ISpellScroll>() is SpellScrollGameItemComponent scroll)
			{
				if (!Store.ClaimCharge(operation with { Id = scroll.ChargeId!.Value, Kind = "ScrollConsumption", Status = "Consumed" }))
					throw new InvalidOperationException("The charge was already claimed by another operation.");
				scroll.Consume(work.Token); Persist(scroll); source.Delete();
			}
			plan.Execute(); _gameworld.SaveManager.Flush();
			if (!book.AddFormula(work.Spell.Id, UtcNow, source.Id)) throw new InvalidOperationException("The destination formula changed during commitment.");
			Persist(book); Store.Record(operation with { Status = "Completed" });
			WritingOutput(actor, proto.CompleteEmote, work.Destination, source);
			return new(true, "Transcription committed. The destination now contains the formula.", work.Token);
		}
		catch (Exception ex) { Store.Record(operation with { Status = "NeedsReview", Diagnostic = ex.Message }); throw; }
	}
}
